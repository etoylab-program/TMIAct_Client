
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionShiranuiSpearAttack : ActionSelectSkillBase
{
	private Unit						mTarget		= null;
	private eAnimation					mCurAni		= eAnimation.DownAttack;
	private Projectile[]				mPjt		= new Projectile[2];
	private AniEvent.sEvent				mAniEvt		= null;
	private AniEvent.sProjectileInfo[]	mPjtInfo	= new AniEvent.sProjectileInfo[2];


	public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.HoldingDefBtnAttack;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        extraCancelCondition = new eActionCondition[3];
        extraCancelCondition[0] = eActionCondition.UseSkill;
        extraCancelCondition[1] = eActionCondition.UseQTE;
        extraCancelCondition[2] = eActionCondition.UseUSkill;

        superArmor = Unit.eSuperArmor.Lv1;

		if(mValue1 > 0.0f)
		{
			mPjt[0] = GameSupport.CreateProjectile("Projectile/pjt_character_shiranui_sting_skill.prefab");
			mPjt[1] = GameSupport.CreateProjectile("Projectile/pjt_character_shiranui_sting_tiger.prefab");

			mAniEvt = m_owner.aniEvent.CreateEvent(eBehaviour.Projectile, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 1.3f);

			for (int i = 0; i < mPjtInfo.Length; ++i)
			{
				mPjtInfo[i] = m_owner.aniEvent.CreateProjectileInfo(mPjt[i]);
				mPjtInfo[i].attach = false;
				mPjtInfo[i].boneName = "Bip001 Prop1";
				mPjtInfo[i].addedPosition = new Vector3(0.0f, 0.3f, 1.5f);
				mPjtInfo[i].followParentRot = true;
				mPjtInfo[i].ignoreYAxis = true;
			}
		}
	}

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        ShowSkillNames(m_data);

        if (mValue1 <= 0.0f)
        {
            mCurAni = eAnimation.DownAttack;
        }
        else
        {
            mCurAni = eAnimation.DownAttack2;
        }

        m_aniLength = m_owner.PlayAniImmediate(mCurAni);
		m_aniCutFrameLength = m_owner.aniEvent.GetCurCutFrameLength();

		mTarget = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
		if (FSaveData.Instance.AutoTargetingSkill)
        {
            if (mTarget)
            {
                m_owner.LookAtTarget(mTarget.transform.position);
            }
        }
        else
        {
            m_owner.cmptRotate.UpdateRotation(m_owner.Input.GetDirection(), true);
        }
    }

    public override IEnumerator UpdateAction()
    {
		bool fire = false;

		int index = 0;
		//if(SetAddAction)
		WeaponData data = mOwnerPlayer.GetCurrentWeaponDataOrNull();
		if (data != null && data.TableID == 7025)
		{
			index = 1;
		}

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime >= m_aniLength)
            {
                m_endUpdate = true;
            }
			else if(mValue1 > 0.0f && !fire && m_checkTime >= m_aniCutFrameLength)
			{
				mPjt[index].Fire(m_owner, BattleOption.eToExecuteType.Unit, mAniEvt, mPjtInfo[index], mTarget, TableId);
				fire = true;
			}

            yield return mWaitForFixedUpdate;
        }
    }

	public override void OnCancel()
	{
		if (isPlaying == false)
		{
			return;
		}

		isCancel = true;
		isPlaying = false;

		m_owner.ShowMesh(true);
		m_owner.RestoreSuperArmor(mChangedSuperArmorId);

		if (!m_owner.isGrounded)
		{
			m_owner.SetFallingRigidBody();
		}

		if( m_owner.AI ) {
			m_owner.Input.CommandDirection( Vector3.zero );
		}

		OnEndCallback?.Invoke();

		IsSkillEnd = true;

		Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.Enemy);
		Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.EnemyGate);
	}

	public override void OnEnd()
	{
		if (MaxCoolTime > 0.0f && Child == null || (Child != null && IsLastSkill && actionCommand != eActionCommand.Attack01))
		{
			if (mTarget && mTarget.AttackerActionOnHit == this)
			{
				m_owner.ExecuteBattleOption(BattleOption.eBOTimingType.OnSkillEnd, 0, null);
			}
		}

		if( m_owner.AI ) {
			m_owner.Input.CommandDirection( Vector3.zero );
		}

		isPlaying = false;
		m_curCancelDuringSameActionCount = 0;

		m_owner.ShowMesh(true);
		m_owner.RestoreSuperArmor(mChangedSuperArmorId);

		if (m_owner.curHp <= 0.0f)
		{
			ActionParamDie paramDie = null;

			ActionHit actionHit = m_owner.actionSystem.GetAction<ActionHit>(eActionCommand.Hit);
			if (actionHit && (actionHit.State == ActionHit.eState.Float || actionHit.State == ActionHit.eState.Down))
			{
				paramDie = new ActionParamDie(ActionParamDie.eState.Down, null);
			}

			SetNextAction(eActionCommand.Die, paramDie);
		}

		OnEndCallback?.Invoke();

		IsSkillEnd = true;

		Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.Enemy);
		Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.EnemyGate);
	}

	public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( mCurAni );
        if (evt == null)
        {
            Debug.LogError(mCurAni.ToString() + "공격 이벤트가 없네??");
            return 0.0f;
        }
        else if (evt.visionRange <= 0.0f)
        {
            Debug.LogError(mCurAni.ToString() + "Vistion Range가 0이네??");
        }

        return evt.visionRange;
    }
}
