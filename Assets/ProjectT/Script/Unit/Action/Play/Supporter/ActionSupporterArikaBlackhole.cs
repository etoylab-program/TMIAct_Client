
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSupporterArikaBlackhole : ActionSupporterSkillBase
{
    private List<Unit>		mListTarget     = null;
    private float			mHitInterval    = 0.25f;
    private float			mHitDistance    = 2.0f;
    private Vector3			mBlackholePos   = Vector3.zero;
    private float			mAttackPower    = 0.0f;
	private AniEvent.sEvent mAniEvt			= null;

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.SupporterArikaBlackhole;
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

        mListTarget = m_owner.GetEnemyList(true);
        for(int i = 0; i < mListTarget.Count; i++)
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

		if(mAniEvt == null)
		{
			mAniEvt = m_owner.aniEvent.CreateEvent(eBehaviour.Attack, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 1.0f);
		}
    }

    public override IEnumerator UpdateAction()
    {
        float checkHitTime = 0.0f;
        bool hit = false;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            if (m_checkTime >= 3.0f || Director.IsPlaying || World.Instance.IsEndGame || World.Instance.ProcessingEnd )
            {
                m_endUpdate = true;
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
					if (target == null || target.MainCollider == null || target.curHp <= 0.0f || target.cmptMovement == null)
					{
                        continue;
                    }

                    if (hit && Vector3.Distance(m_owner.GetTargetCapsuleEdgePos(target), mBlackholePos) <= mHitDistance)
                    {
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
    }

    public override void OnCancel()
    {
        base.OnCancel();

        m_checkTime = 3.0f;
        m_endUpdate = true;
    }
}
