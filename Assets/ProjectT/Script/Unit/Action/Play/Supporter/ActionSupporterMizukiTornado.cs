
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSupporterMizukiTornado : ActionSupporterSkillBase
{
    private List<Unit>		mListTarget         = null;
    private List<Unit>		mListFloatingTarget = new List<Unit>();
	private AniEvent.sEvent mAniEvt				= null;


	public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.SupporterMizukiTornado;
	}

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        mParamFromBO = param as ActionParamFromBO;

		if(mAniEvt == null)
		{
			mAniEvt = m_owner.aniEvent.CreateEvent(eBehaviour.UpperAttack, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 1.0f);
		}

        float damage = m_owner.attackPower * mParamFromBO.battleOptionData.value;
        mListTarget = m_owner.GetEnemyList(true);

		mAniEvt.behaviour = eBehaviour.UpperAttack;

		mAtkEvt.Set(eEventType.EVENT_BATTLE_ON_DIRECT_HIT, m_owner, BattleOption.eToExecuteType.Supporter, mAniEvt, damage, eAttackDirection.Skip, false, 0, 
                    EffectManager.eType.None, m_owner.GetEnemyColliderList(), 0.0f, true);

        EventMgr.Instance.SendEvent(mAtkEvt);
    }

    public override IEnumerator UpdateAction()
    {
        if (mListTarget != null)
        {
            mListFloatingTarget.Clear();

            for (int i = 0; i < mListTarget.Count; i++)
            {
                Unit target = mListTarget[i];
                if (!target.IsActivate() || target.curHp <= 0.0f)
                {
                    continue;
                }

                if (mParamFromBO.battleOptionData.effId1 > 0)
                {   // 아래의 조건에 만족하지 않는다해도 스킬 사용 이펙트는 표시, 표시조차 안하면 기능 오류로 느껴짐
                    EffectManager.Instance.Play(target, mParamFromBO.battleOptionData.effId1, (EffectManager.eType)mParamFromBO.battleOptionData.effType);
                }

                ActionHit actionHit = target.actionSystem.GetCurrentAction<ActionHit>();
                if (actionHit == null || actionHit.State != ActionHit.eState.Float)
                {
                    continue;
                }

                mListFloatingTarget.Add(target);
            }

            int fastFallCount = 0;

            //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
            while (fastFallCount < mListFloatingTarget.Count)
            {
                if(m_endUpdate || Director.IsPlaying || World.Instance.IsEndGame || World.Instance.ProcessingEnd )
                {
                    OnCancel();
                    yield break;
                }

                for (int i = 0; i < mListFloatingTarget.Count; i++)
                {
                    Unit target = mListFloatingTarget[i];
                    if (target.isFalling)
                    {
                        target.cmptJump.SetFastFall();
                        ++fastFallCount;
                    }
                    else if (target.IsImmuneFloat())
                    {
                        ++fastFallCount;
                    }
                }

                yield return mWaitForFixedUpdate;
            }

            mAniEvt.behaviour = eBehaviour.DownAttack;
            List<UnitCollider> list = new List<UnitCollider>();

            while (mListFloatingTarget.Count > 0)
            {
                if (m_endUpdate || Director.IsPlaying || World.Instance.IsEndGame || World.Instance.ProcessingEnd )
                {
                    OnCancel();
                    yield break;
                }

                list.Clear();
                for (int i = 0; i < mListFloatingTarget.Count; i++)
                {
                    Unit target = mListFloatingTarget[i];
                    if (target.isGrounded)
                    {
                        target.DownDuration = mParamFromBO.battleOptionData.value3;
                        target.floatingJumpPowerRatio = Unit.FLOATING_JUMP_POWER_RATIO;

                        list.Add(target.MainCollider);

                        mListFloatingTarget.Remove(target);
                    }
                }

                mAtkEvt.Set(eEventType.EVENT_BATTLE_ON_DIRECT_HIT, m_owner, BattleOption.eToExecuteType.Supporter, mAniEvt, 0.0f, eAttackDirection.Skip, 
							false, 0, EffectManager.eType.None, list, 0.0f, true);

                EventMgr.Instance.SendEvent(mAtkEvt);
                yield return mWaitForFixedUpdate;
            }
        }
    }

    public override void OnCancel()
    {
        base.OnCancel();

        for (int i = 0; i < mListFloatingTarget.Count; i++)
        {
            mListFloatingTarget[i].LockDie(false);
            mListFloatingTarget[i].actionSystem.CancelCurrentAction();
        }

        m_endUpdate = true;
    }
}
