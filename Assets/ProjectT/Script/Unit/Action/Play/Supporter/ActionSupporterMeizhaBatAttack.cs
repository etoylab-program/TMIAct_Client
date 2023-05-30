
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSupporterMeizhaBatAttack : ActionSupporterSkillBase
{
    private UnitCollider    mTargetCollider     = null;
    private float           mAttackPower        = 0.0f;
    private ParticleSystem  mEffBatting         = null;
    private Animation       mAniBatting         = null;
    private AnimationEvent  mAniBattingEvent    = null;
    private int             mAttackCnt          = 0;
	private AniEvent.sEvent mAniEvt				= null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.SupporterMeizhaBatAttack;
	}

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        mParamFromBO = param as ActionParamFromBO;

		if(mAniEvt == null)
		{
			mAniEvt = m_owner.aniEvent.CreateEvent(eBehaviour.Attack, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 1.0f);
		}

        mEffBatting = EffectManager.Instance.GetEffectOrNull(mParamFromBO.battleOptionData.effId1, (EffectManager.eType)mParamFromBO.battleOptionData.effType);
        mAniBatting = mEffBatting.GetComponentInChildren<Animation>();
        mAniBattingEvent = mEffBatting.GetComponentInChildren<AnimationEvent>();

        mAniBattingEvent.OnFunc01 = Attack;
        mAniBattingEvent.OnFunc02 = Attack;
        mAniBattingEvent.OnFunc03 = Attack;

        mAttackCnt = 0;

        mEffBatting.gameObject.SetActive(false);

        mTargetCollider = m_owner.GetRandomTargetCollider(true);
        if(mTargetCollider == null)
        {
            m_endUpdate = true;
        }
        else
        {
            mAttackPower = mOwnerPlayer.attackPower * mParamFromBO.battleOptionData.value;
            if (mTargetCollider.Owner.monType == Unit.eMonType.Machine)
                mAttackPower += mAttackPower * mParamFromBO.battleOptionData.value2;
        }
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        if (!m_endUpdate)
        {
            //Transform tfHitPos = mTargetCollider.Owner.aniEvent.GetBoneByName("HitPos");
            //Vector3 effPos = Vector3.zero;

            EffectManager.Instance.Play(mTargetCollider.Owner, mParamFromBO.battleOptionData.effId1, (EffectManager.eType)mParamFromBO.battleOptionData.effType);
            mAniBatting.Play();

            m_checkTime = 0.0f;
            while(m_checkTime <= mEffBatting.main.duration)
            {
                if(Director.IsPlaying)
                {
                    OnCancel();
                    break;
                }
                else if(mTargetCollider.Owner.curHp <= 0.0f)
                {
                    EffectManager.Instance.StopEffImmediate(mParamFromBO.battleOptionData.effId1, (EffectManager.eType)mParamFromBO.battleOptionData.effType, null);
                    m_endUpdate = true;

                    break;
                }

                m_checkTime += Time.fixedDeltaTime;
                /*if (tfHitPos == null)
                {
                    effPos = mTargetCollider.GetCenterPos();
                }
                else
                {
                    effPos = tfHitPos.position;
                }

                mEffBatting.transform.localPosition = new Vector3(effPos.x, effPos.y - (mTargetCollider.GetCenterPos().y + 0.1f), effPos.z);*/

                mEffBatting.transform.position = new Vector3(mTargetCollider.Owner.transform.position.x, 
                                                             mTargetCollider.Owner.transform.position.y - 0.5f,
                                                             mTargetCollider.Owner.transform.position.z);
                yield return mWaitForFixedUpdate;
            }

            mEffBatting.gameObject.SetActive(false);
        }
    }

    private bool Attack(UnityEngine.AnimationEvent arg)
    {
        mAttackCnt++;


		if (mAttackCnt == 1) //올려치고
		{
			mAniEvt.behaviour = eBehaviour.UpperAttack;
		}
		else if (mAttackCnt == 2) //내려친 후
		{
			mAniEvt.behaviour = eBehaviour.FastFallAttack;
		}
		else //다운어택
		{
			mAniEvt.behaviour = eBehaviour.DownAttack;
			//EffectManager.Instance.Detach(mTargetCollider.Owner, mParamFromBO.battleOptionData.effId1, (EffectManager.eType)mParamFromBO.battleOptionData.effType);
		}

        //eHitState hitState = eHitState.Success;

        //mTargetCollider.Owner.actionSystem.CancelCurrentAction();
        //mTargetCollider.Owner.OnHit(m_owner, evt, mAttackPower * arg.floatParameter, false, ref hitState);

        mAtkEvt.SetWithSingleTarget(eEventType.EVENT_BATTLE_ON_DIRECT_HIT, m_owner, BattleOption.eToExecuteType.Supporter, mAniEvt, 
									mAttackPower * arg.floatParameter, eAttackDirection.Skip, false, 0, EffectManager.eType.None, mTargetCollider, 0.0f, true);

        EventMgr.Instance.SendEvent(mAtkEvt);

        return true;
    }

    public override void OnCancel()
    {
        base.OnCancel();
        m_endUpdate = true;

        if (mTargetCollider && mTargetCollider.Owner)
        {
            mTargetCollider.Owner.LockDie(false);
            mTargetCollider.Owner.actionSystem.CancelCurrentAction();
        }

        m_checkTime = mEffBatting.main.duration;
        EffectManager.Instance.StopEffImmediate(mParamFromBO.battleOptionData.effId1, (EffectManager.eType)mParamFromBO.battleOptionData.effType, null);
    }
}
