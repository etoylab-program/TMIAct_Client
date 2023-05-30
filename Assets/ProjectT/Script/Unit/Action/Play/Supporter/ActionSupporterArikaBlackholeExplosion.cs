
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSupporterArikaBlackholeExplosion : ActionSupporterSkillBase
{
    private List<Unit>		mListTarget			= null;
    private float			mHitInterval		= 0.25f;
    private float			mHitDistance		= 2.0f;
    private bool			mExplosion			= false;
    private Vector3			mBlackholePos		= Vector3.zero;
    private float			mAttackPower		= 0.0f;
    private float			mExplosionPower		= 0.0f;
	private AniEvent.sEvent	mAniEvt				= null;


	public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.SupporterArikaBlackholeExplosion;
	}

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        mParamFromBO = param as ActionParamFromBO;

        if (mParamFromBO.battleOptionData.effId1 > 0)
        {
            mBlackholePos = m_owner.transform.position + (m_owner.transform.forward * 1.25f);
            mBlackholePos.y = m_owner.transform.position.y + 1.0f;

            EffectManager.Instance.Play(mBlackholePos, mParamFromBO.battleOptionData.effId1, (EffectManager.eType)mParamFromBO.battleOptionData.effType);
        }

        mExplosion = false;

        mListTarget = m_owner.GetEnemyList(true);
        if (mListTarget != null && mListTarget.Count > 0)
        {
            for (int i = 0; i < mListTarget.Count; i++)
            {
                Unit target = mListTarget[i];
                if (target.CurrentSuperArmor >= Unit.eSuperArmor.Lv2 || target.IsImmuneFloat() || target.IsImmuneKnockback())
                {
                    continue;
                }

                target.actionSystem.CancelCurrentAction();
                target.StopStepForward();
            }

            mAttackPower = m_owner.attackPower * mParamFromBO.battleOptionData.value;
            mExplosionPower = mAttackPower + (mAttackPower * mParamFromBO.battleOptionData.value2);
        }

		if(mAniEvt == null)
		{
			mAniEvt = m_owner.aniEvent.CreateEvent(eBehaviour.Attack, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 1.0f);
		}
    }

    public override IEnumerator UpdateAction()
    {
        if(mListTarget == null || mListTarget.Count <= 0)
        {
            yield break;
        }
        
        //AniEvent.sEvent evt = m_owner.aniEvent.CreateEvent(eBehaviour.Attack, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 1.0f);
        //eHitState hitState = eHitState.Success;

        float checkHitTime = 0.0f;
        bool hit = false;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!mExplosion)
        {
            if (m_checkTime >= 3.0f || Director.IsPlaying || World.Instance.IsEndGame || World.Instance.ProcessingEnd )
            {
                mExplosion = true;
            }
            else
            {
                checkHitTime += m_owner.fixedDeltaTime;
                if (checkHitTime >= mHitInterval)
                {
                    m_checkTime += checkHitTime;

                    checkHitTime = 0.0f;
                    hit = true;
                }

                for (int i = 0; i < mListTarget.Count; i++)
                {
                    Unit target = mListTarget[i];
                    if(target == null || target.MainCollider == null || target.curHp <= 0.0f || target.cmptMovement == null)
                    {
                        continue;
                    }

                    if (hit && Vector3.Distance(m_owner.GetTargetCapsuleEdgePos(target), mBlackholePos) <= mHitDistance)
                    {
						mAniEvt.behaviour = eBehaviour.Attack;

						mAtkEvt.SetWithSingleTarget(eEventType.EVENT_BATTLE_ON_DIRECT_HIT, m_owner, BattleOption.eToExecuteType.Supporter, mAniEvt, 
													mAttackPower, eAttackDirection.Skip, false, 0, EffectManager.eType.None, target.MainCollider, 0.0f, true);

                        EventMgr.Instance.SendEvent(mAtkEvt);
                    }

                    if (target.CurrentSuperArmor >= Unit.eSuperArmor.Lv2 || target.IsImmuneFloat() || target.IsImmuneKnockback())
                    {
                        continue;
                    }

                    target.StopStepForward();

                    Vector3 v = (mBlackholePos - target.transform.position).normalized;
                    v.y = 0.0f;

                    target.cmptMovement.UpdatePosition(v, Mathf.Max(GameInfo.Instance.BattleConfig.BlackholeMinSpeed, target.speed * 1.2f), false);
                }

                hit = false;
            }

            yield return mWaitForFixedUpdate;
        }

        if(mExplosion)
        {
            if (mParamFromBO.battleOptionData.effId2 > 0)
            {
                EffectManager.Instance.Play(mBlackholePos, mParamFromBO.battleOptionData.effId2, (EffectManager.eType)mParamFromBO.battleOptionData.effType);
            }

			/*for (int i = 0; i < mListTarget.Count; i++)
            {
                Unit target = mListTarget[i];
                if (target.superArmor.Value >= Unit.eSuperArmor.Lv1 || !target.IsActivate() || target.curHp <= 0.0f)
                    continue;

                target.OnHit(m_owner, evt, mExplosionPower, false, ref hitState);
            }*/

			mAniEvt.behaviour = eBehaviour.KnockBackAttack;

			mAtkEvt.Set(eEventType.EVENT_BATTLE_ON_DIRECT_HIT, m_owner, BattleOption.eToExecuteType.Supporter, mAniEvt, mExplosionPower, 
						eAttackDirection.Skip, false, 0, EffectManager.eType.None, m_owner.GetEnemyColliderList(), 0.0f, true);

            EventMgr.Instance.SendEvent(mAtkEvt);
        }
    }

    public override void OnCancel()
    {
        base.OnCancel();

        m_endUpdate = true;
        m_checkTime = 3.0f;
    }
}
