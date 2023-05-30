
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSupporterKayoruStunTornado : ActionSupporterSkillBase
{
    private List<Unit>          mListTarget         = null;
    private float               mTornadoInterval    = 3.0f;
    private List<UnitCollider>  mList               = new List<UnitCollider>();
	private AniEvent.sEvent		mAniEvt				= null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.SupporterKayoruStunTornado;
	}

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        mParamFromBO = param as ActionParamFromBO;

		if(mAniEvt == null)
		{
			mAniEvt = m_owner.aniEvent.CreateEvent(eBehaviour.StunAttack, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 1.0f);
		}

        mListTarget = m_owner.GetEnemyList(true);
        if (mListTarget == null || mListTarget.Count <= 0)
        {
            m_endUpdate = true;
        }
        else
        {
            Stun();
        }
    }

    public override IEnumerator UpdateAction()
    {
        //AniEvent.sEvent evt = m_owner.aniEvent.CreateEvent(eBehaviour.DownAttack, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 1.0f);

        if (!m_endUpdate)
        {
            yield return new WaitForSeconds(mTornadoInterval);

            if(Director.IsPlaying) // 서포터 스킬 쓰고 바로 오의를 쓰면 플레이어 actionSystem2에 현재 액션이 없어서 캔슬을 못함. 연출 중인지로 체크
            {
                OnCancel();
                yield break;
            }

            StartTornado();
            yield return new WaitForEndOfFrame();

            for (int i = 0; i < mList.Count; i++)
            {
                Unit target = mList[i].Owner;
                if (!target.IsActivate())// || target.curHp <= 0.0f)
                {
                    continue;
                }

                if (mParamFromBO.battleOptionData.effId2 > 0)
                {
                    EffectManager.Instance.Play(target, mParamFromBO.battleOptionData.effId2, (EffectManager.eType)mParamFromBO.battleOptionData.effType);
                }
            }

            int fastFallCount = 0;

            //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
            while (fastFallCount < mList.Count)
            {
                if(m_endUpdate)
                {
                    break;
                }

                for (int i = 0; i < mList.Count; i++)
                {
                    Unit target = mList[i].Owner;
                    if (target.isFalling || !target.IsActivate() || target.curHp <= 0.0f)
                    {
                        target.cmptJump.SetFastFall();
                        ++fastFallCount;
                    }
                    else if(target.IsImmuneFloat())
                    {
                        ++fastFallCount;
                    }
                }

                yield return mWaitForFixedUpdate;
            }
        }
    }

    private void Stun()
    {
        float damage = m_owner.attackPower * mParamFromBO.battleOptionData.value;

        List<UnitCollider> list = new List<UnitCollider>();
        for (int i = 0; i < mListTarget.Count; i++)
        {
            if (mParamFromBO.battleOptionData.effId1 > 0)
            {
                EffectManager.Instance.Play(mListTarget[i], mParamFromBO.battleOptionData.effId1, (EffectManager.eType)mParamFromBO.battleOptionData.effType);
            }

            list.Add(mListTarget[i].MainCollider);
        }

		mAniEvt.behaviour = eBehaviour.StunAttack;

		mAtkEvt.Set(eEventType.EVENT_BATTLE_ON_DIRECT_HIT, m_owner, BattleOption.eToExecuteType.Supporter, mAniEvt, damage, eAttackDirection.Skip, false, 0, 
                    EffectManager.eType.None, list, 0.0f, true);

        EventMgr.Instance.SendEvent(mAtkEvt);
    }

    private void StartTornado()
    {
        float damage = m_owner.attackPower * mParamFromBO.battleOptionData.value;
        damage += damage * mParamFromBO.battleOptionData.value2;

        mList.Clear();
        for (int i = 0; i < mListTarget.Count; i++)
        {
            Unit target = mListTarget[i];
            if (!target.IsActivate() || target.curHp <= 0.0f)
                continue;

            ActionHit actionHit = target.actionSystem.GetCurrentAction<ActionHit>();
            if (actionHit == null || actionHit.State != ActionHit.eState.Stun)
            {
                continue;
            }

            target.floatingJumpPowerRatio = 1.4f;
            mList.Add(target.MainCollider);
        }

		mAniEvt.behaviour = eBehaviour.UpperAttack;

        mAtkEvt.Set(eEventType.EVENT_BATTLE_ON_DIRECT_HIT, m_owner, BattleOption.eToExecuteType.Supporter, mAniEvt, damage, eAttackDirection.Skip, false, 0, 
                    EffectManager.eType.None, mList, 0.0f, true);

        EventMgr.Instance.SendEvent(mAtkEvt);
    }

    public override void OnCancel()
    {
        base.OnCancel();

        for (int i = 0; i < mList.Count; i++)
        {
            mList[i].Owner.LockDie(false);
            mList[i].Owner.actionSystem.CancelCurrentAction();
        }
            
        m_endUpdate = true;
    }
}
