
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionMomochiThrowChain : ActionEnemyAttackBase
{
    public enum EState
    {
        THROW = 0,
        BRING_BACK,
    }


    [Header("[Property]")]
    public Transform        StartThrowBone;
    public float            ChainSpeed          = 25.0f;
    public float            ChainHoldingTime    = 0.5f;
    public ParticleSystem   PSCHain;
    public int              BindCommonEffIndex  = 40005;

    private State           mState          = new State();
    private ParticleSystem  mEffChain       = null;
    private TrailRenderer   mTrailChain     = null;
    private BoxCollider     mChainBoxCol    = null;
    private UnitCollider    mTargetCollider = null;
    private Vector3         mTargetPos      = Vector3.zero;
    private Vector3         mChainDir       = Vector3.zero;
    private Vector3         mChainDest      = Vector3.zero;

    private AniEvent.sEvent mStunAtkEvt     = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.ThrowChain;

        extraCondition = new eActionCondition[1];
        extraCondition[0] = eActionCondition.Grounded;

		mState.Init(2);
        mState.Bind(EState.THROW, ChangeThrowState);
        mState.Bind(EState.BRING_BACK, ChangeBringBackState);

        mEffChain = GameSupport.CreateParticle(PSCHain, null);
        mTrailChain = mEffChain.GetComponent<TrailRenderer>();
        mChainBoxCol = mEffChain.GetComponent<BoxCollider>();

        mStunAtkEvt = new AniEvent.sEvent();
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        AniEvent.CopyEventForAttack(m_owner.aniEvent.GetFirstAttackEvent(eAnimation.Attack01), mStunAtkEvt);
        mStunAtkEvt.behaviour = eBehaviour.StunAttack;

        mState.ChangeState(EState.THROW, true);
    }

    public override IEnumerator UpdateAction()
    {
        bool throwChain = false;
        bool bringBack = false;
        bool onHit = false;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            m_checkTime += m_owner.fixedDeltaTime;

            switch((EState)mState.current)
            {
                case EState.THROW:
                    if (!throwChain && m_checkTime > m_aniCutFrameLength)
                    {
                        throwChain = true;
                        StartThrowChain();
                    }
                    else if (m_checkTime >= m_aniLength)
                    {
                        mState.ChangeState(EState.BRING_BACK, true);
                    }
                    else if (m_checkTime >= m_aniLength - (ChainHoldingTime * 1.5f))
                    {
                    }
                    else if(!onHit)
                    {
                        Collider[] cols = Physics.OverlapBox(mChainBoxCol.transform.position, mChainBoxCol.size * 0.5f, mChainBoxCol.transform.rotation, 1 << (int)eLayer.Player);
                        if (cols == null || cols.Length <= 0)
                        {
                            mEffChain.transform.position += mChainDir * ChainSpeed * m_owner.fixedDeltaTime;
                        }
                        else if(mTargetCollider.Owner.CurrentSuperArmor <= Unit.eSuperArmor.Lv1 && mTargetCollider.Owner.curHp > 0.0f)
                        {
                            EffectManager.Instance.Play(mTargetCollider.Owner, BindCommonEffIndex, EffectManager.eType.Common);

                            mAtkEvt.SetWithSingleTarget(eEventType.EVENT_BATTLE_ON_DIRECT_HIT, m_owner, BattleOption.eToExecuteType.Unit, mStunAtkEvt, m_owner.attackPower, 
                                                        eAttackDirection.Skip, false, 0, EffectManager.eType.None, mTargetCollider, 0.0f, true);

                            EventMgr.Instance.SendEvent(mAtkEvt);
                            onHit = true;
                        }
                    }
                    break;

                case EState.BRING_BACK:
                    if (!bringBack && m_checkTime >= m_aniCutFrameLength)
                    {
                        bringBack = true;
                        BringBackChain();
                    }
                    else if (m_checkTime >= m_aniLength)
                    {
                        m_endUpdate = true;
                    }
                    else
                    {
                        if (Vector3.Distance(mEffChain.transform.position, mChainDest) > 0.3f)
                        {
                            mEffChain.transform.position += mChainDir * ChainSpeed * m_owner.fixedDeltaTime;

                            if (onHit)
                            {
                                mTargetCollider.Owner.cmptMovement.UpdatePosition(mChainDir, ChainSpeed, false);
                            }
                        }
                    }
                    break;

                default:
                    Debug.LogError("ActionMomochiThrowChain::UpdateAction 엄한 상태가 있는데??");
                    break;
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnCancel()
    {
        base.OnCancel();

        mTrailChain.Clear();
        mEffChain.Stop();
        mEffChain.gameObject.SetActive(false);
    }

    public override void OnEnd()
    {
        base.OnEnd();

        mTrailChain.Clear();
        mEffChain.Stop();
        mEffChain.gameObject.SetActive(false);
    }

    public bool ChangeThrowState(bool bChangeAni)
    {
        mEffChain.transform.position = StartThrowBone.position;
        mEffChain.transform.rotation = m_owner.transform.rotation;

        mTargetCollider = m_owner.GetMainTargetCollider(true);
        mTargetPos = mTargetCollider.GetCenterPos();//mTargetCollider.Owner.transform.position + (mTargetCollider.Owner.transform.right * 0.3f);
        //mTargetPos.y = m_owner.transform.position.y;

        m_owner.LookAtTarget(mTargetPos);

        m_aniLength = m_owner.PlayAni(eAnimation.StartThrowChain) + ChainHoldingTime;
        m_aniCutFrameLength = m_owner.aniEvent.GetCurCutFrameLength();

        m_checkTime = 0.0f;
        return true;
    }

    public bool ChangeBringBackState(bool bChangeAni)
    {
        m_aniLength = m_owner.PlayAni(eAnimation.EndThrowChain);
        m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(eAnimation.EndThrowChain);

        m_checkTime = 0.0f;
        return true;
    }

    private void StartThrowChain()
    {
        mTrailChain.Clear();
        mTrailChain.time = Mathf.Infinity;

        mEffChain.transform.position = StartThrowBone.position;
        mEffChain.transform.rotation = m_owner.transform.rotation;

        mChainDir = m_owner.transform.forward;//m_owner.transform.position).normalized;
        mChainDir.y = 0.0f;
        mChainDest = StartThrowBone.position;//mTargetPos;

        mEffChain.Stop();
        mEffChain.gameObject.SetActive(true);
    }

    private void BringBackChain()
    {
        mTrailChain.time = 0.2f;

        //Vector3 v = m_owner.transform.position;
        //v.y = mEffChain.transform.position.y;

        //mChainDir = (v - mEffChain.transform.position).normalized;
        mChainDir = -mChainDir;
        //mChainDest = StartThrowBone.position;//v;
    }
}
