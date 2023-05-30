
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionKiraraEvadeCharge2Attack : ActionSelectSkillBase
{
    private Unit					mTarget				= null;
	private eAnimation				mCurAni				= eAnimation.EvadeCharge2;
	private bool					mbStartSetAddAction	= false;


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
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        ShowSkillNames(m_data);

        mCurAni = eAnimation.EvadeCharge2;
        if (mValue1 >= 1.0f)
        {
            mCurAni = eAnimation.EvadeCharge2_1;
        }

		if (FSaveData.Instance.AutoTargetingSkill)
		{
			mTarget = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
			if (mTarget)
			{
				m_owner.LookAtTarget(mTarget.transform.position);
			}
		}
		else
		{
			m_owner.cmptRotate.UpdateRotation(m_owner.Input.GetDirection(), true);
		}

		m_aniLength = m_owner.PlayAniImmediate(mCurAni);

		mbStartSetAddAction = false;
		float useSp = (AddActionValue1 * (float)eCOUNT.MAX_BO_FUNC_VALUE);

		if (SetAddAction)
		{
			mbStartSetAddAction = m_owner.curSp >= useSp;
		}

		List<AniEvent.sEvent> list = m_owner.aniEvent.GetAllAttackEvent(mCurAni);
		for (int i = 0; i < list.Count; ++i)
		{
			list[i].IsCritical = mbStartSetAddAction;
		}

		AniEvent.sEvent evt = m_owner.aniEvent.GetLastAttackEvent(mCurAni);
		if(evt != null)
		{
			evt.atkAttr = mbStartSetAddAction ? EAttackAttr.Freeze : EAttackAttr.NONE;
			evt.AtkAttrValue1 = mbStartSetAddAction ? AddActionValue2 : 0.0f;
		}

		if (mbStartSetAddAction)
		{
			m_owner.UseSp(useSp);
		}
    }

    public override IEnumerator UpdateAction()
    {
        //float firstAtkEventLength = m_owner.aniEvent.GetFirstAttackEventLength(mCurAni);
		//float lastAtkEventLength = m_owner.aniEvent.GetLastAttackEventLength(mCurAni);

        while (m_endUpdate == false)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            /*if(m_checkTime <= firstAtkEventLength)
            {
            }
            else*/ if (m_checkTime >= m_aniLength)
            {
                m_endUpdate = true;
            }

			if (FSaveData.Instance.AutoTargetingSkill)
			{
				if (mTarget)
				{
					if (mTarget.curHp <= 0.0f)
					{
						mTarget = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
					}

					if (mTarget)
					{
						m_owner.LookAtTarget(mTarget.transform.position);
					}
				}
				else if (mTarget == null)
				{
					mTarget = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
				}
			}
			else
			{
				m_owner.cmptRotate.UpdateRotation(m_owner.Input.GetDirection(), true);
			}

			yield return mWaitForFixedUpdate;
        }
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
