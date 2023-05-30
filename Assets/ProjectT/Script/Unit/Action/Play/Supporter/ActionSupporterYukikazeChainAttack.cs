
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSupporterYukikazeChainAttack : ActionSupporterSkillBase
{
    private AniEvent.sEvent mAniEvt             = null;
    private UnitCollider    mTargetCollider     = null;
    private Coroutine       mCr                 = null;
    private bool            mbCheckingTargetDie = false;
    private float           mAttackDelay        = 0.0f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.SupporterYukikazeChainAttack;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        mParamFromBO = param as ActionParamFromBO;

		if (mAniEvt == null)
		{
			mAniEvt = m_owner.aniEvent.CreateEvent(eBehaviour.Attack, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 1.0f);
		}

        mbCheckingTargetDie = false;
        mAttackDelay = mParamFromBO.battleOptionData.value2;
    }

    public override IEnumerator UpdateAction()
    {
        bool doAttack = true;
        bool chainAttack = false;

        m_checkTime = 0.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while(!m_endUpdate)
        {
            if(doAttack)
            {
                if (DoAttack())
                {
                    doAttack = false;
                }
                else
                {
                    m_endUpdate = true;
                }
            }

            if (mbCheckingTargetDie)
            {
                if (!doAttack && !chainAttack)
                {
                    if (mTargetCollider.Owner.curHp <= 0.0f)
                    {
                        chainAttack = true;
                        doAttack = false;
                        mbCheckingTargetDie = false;

                        m_owner.ExecuteBattleOption(BattleOption.eBOTimingType.ManualCall, 0, null);
                    }
                    else
                    {
                        m_endUpdate = true;
                    }
                }
            }
            else if (!doAttack && !chainAttack && mTargetCollider.Owner.curHp <= 0.0f) // 서포터 스킬에 맞기 전에 적이 죽을 경우
            {
                m_endUpdate = true;
            }

            if (chainAttack)
            {
                m_checkTime += Time.fixedDeltaTime;
                if (m_checkTime >= mAttackDelay)
                {
                    doAttack = true;
                    chainAttack = false;

                    m_checkTime = 0.0f;
                }
            }

            yield return mWaitForFixedUpdate;
        }
    }

    private bool DoAttack()
    {
        mTargetCollider = m_owner.GetMainTargetCollider(true);
        if(mTargetCollider == null)
        {
            return false;
        }

        EffectManager.Instance.Play(mTargetCollider.Owner, mParamFromBO.battleOptionData.effId2, EffectManager.eType.Each_Monster_Normal_Hit, 0.0f, false, true);

        Utility.StopCoroutine(this, ref mCr);
        mCr = StartCoroutine(DelayedDoAttack(mAttackDelay));

        return true;
    }

    private IEnumerator DelayedDoAttack(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (mTargetCollider.Owner.curHp > 0.0f)
        {
            float damage = m_owner.attackPower * mParamFromBO.battleOptionData.value;

            mAtkEvt.SetWithSingleTarget(eEventType.EVENT_BATTLE_ON_DIRECT_HIT, m_owner, BattleOption.eToExecuteType.Supporter, mAniEvt, damage,
                                        eAttackDirection.Skip, false, 0, EffectManager.eType.None,
                                        mTargetCollider, 0.0f, true);

            EventMgr.Instance.SendEvent(mAtkEvt);

            mbCheckingTargetDie = true;
        }
    }
}
