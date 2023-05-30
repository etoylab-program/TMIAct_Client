
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionMurasakiHookAttack : ActionTeleport
{
    public enum eState
    {
        Start,
        Move,
        MoveEnd,
        Attack,
    }


    private State           mState          = new State();
    private ParticleSystem  mEffHook        = null;
    private TrailRenderer[] mTrails         = null;
    private Transform       mTrStartBone    = null;
    private BoxCollider     mBoxCol         = null;
    private Vector3         mHookDir        = Vector3.zero;
    private float           mMoveTime       = 0.0f;

    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

		mState.Init(4);
        mState.Bind(eState.Start, ChangeStartState);
        mState.Bind(eState.Move, ChangeMoveState);
        mState.Bind(eState.MoveEnd, ChangeMoveEndState);
        mState.Bind(eState.Attack, ChangeAttackState);

        mEffHook = GameSupport.CreateParticle( "Effect/Character/prf_fx_murasaki_attack_hook_r.prefab", null);
        mTrails = mEffHook.GetComponentsInChildren<TrailRenderer>();
        mBoxCol = mEffHook.GetComponent<BoxCollider>();

        mTrStartBone = m_owner.aniEvent.GetBoneByName("Bip001 L Hand");
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        mTargetCollider = m_owner.GetMainTargetCollider(true);
        if (mTargetCollider)
        {
            m_owner.LookAtTarget(mTargetCollider.GetCenterPos());
            
            m_owner.SetMainTarget(mTargetCollider.Owner);
            //World.Instance.InGameCamera.TurnToTarget(mTargetCollider.GetCenterPos(), 2.0f, true);

            mHookDir = (mTargetCollider.GetCenterPos() - mTrStartBone.position).normalized;
            mHookDir.y = 0.0f;
        }

        mState.ChangeState(eState.Start, true);
    }

    public override IEnumerator UpdateAction()
    {
        m_checkTime = 0.0f;

        float totalTime = 0.0f;
        float maxTotalTime = 2.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            totalTime += m_owner.fixedDeltaTime;
            if ( totalTime >= maxTotalTime ) {
                m_endUpdate = true;
            }
            else {
                m_checkTime += m_owner.fixedDeltaTime;

                switch ( (eState)mState.current ) {
                    case eState.Start:
                        if ( m_checkTime >= m_aniCutFrameLength ) {
                            mState.ChangeState( eState.Move, true );
                        }
                        else if ( m_checkTime >= m_aniCutFrameLength * 0.7f ) {
                            if ( !mEffHook.gameObject.activeSelf ) {
                                ShowHook( true );
                            }

                            Collider[] cols = Physics.OverlapBox( mBoxCol.transform.position, mBoxCol.size * 0.5f, mBoxCol.transform.rotation, 1 << (int)eLayer.Enemy | 1 << (int)eLayer.EnemyGate );
                            if ( cols == null || cols.Length <= 0 ) {
                                mHookDir = ( mTargetCollider.GetCenterPos() - mTrStartBone.position ).normalized;
                                mHookDir.y = 0.0f;

                                mEffHook.transform.position += mHookDir * 50.0f * m_owner.fixedDeltaTime;
                            }
                            else {
                                mEffHook.transform.position = mTargetCollider.GetCenterPos();
                            }
                        }
                        break;

                    case eState.Move:
                        if ( m_owner.holdPositionRef > 0 || Vector3.Distance( m_owner.transform.position, GetDestPos() ) <= 1.5f || m_checkTime > ( mMoveTime * 1.5f ) ) {
                            EndStepForward();
                        }
                        break;

                    case eState.MoveEnd:
                    case eState.Attack:
                        if ( m_checkTime >= m_aniLength ) {
                            m_endUpdate = true;
                        }
                        break;
                }
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnCancel()
    {
        base.OnCancel();
        ShowHook(false);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        ShowHook(false);
    }

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(eAnimation.EvadeDouble_2);
        if (evt == null)
        {
            Debug.LogError("HookAttack 공격 이벤트가 없네??");
            return 0.0f;
        }
        else if (evt.visionRange <= 0.0f)
        {
            Debug.LogError("HookAttack Vistion Range가 0이네??");
        }

        return evt.visionRange;
    }

    private bool ChangeStartState(bool changeAni)
    {
        m_checkTime = 0.0f;

        if (mTargetCollider == null)
        {
            m_endUpdate = true;
            return false;
        }

        m_aniLength = m_owner.PlayAniImmediate(eAnimation.EvadeDouble);
        m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(eAnimation.EvadeDouble);

        mEffHook.transform.position = mTrStartBone.position;
        mEffHook.transform.rotation = m_owner.transform.rotation;
        
        return true;
    }

    private bool ChangeMoveState(bool changeAni)
    {
        m_checkTime = 0.0f;
        mMoveTime = 0.1f;//m_aniLength - m_aniCutFrameLength;

        ShowHook(false);
        m_owner.StartStepForward(mMoveTime, GetDestPos(), null);

        if (mValue3 > 0.0f)
        {
            mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
            mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.Each_Monster_Normal_Hit;
            mBuffEvt.Set(m_data.ID, eEventSubject.MainTarget, eEventType.EVENT_DEBUFF_SPEED_DOWN, m_owner, mValue2, 0.0f, 0.0f, mValue3, 0.0f,
                         0, 0, eBuffIconType.Debuff_Speed);

            EventMgr.Instance.SendEvent(mBuffEvt);
        }

        return true;
    }

    private void EndStepForward()
    {
        if (mValue1 <= 0.0f)
        {
            mState.ChangeState(eState.MoveEnd, true);
        }
        else
        {
            mState.ChangeState(eState.Attack, true);
        }
    }

    private bool ChangeMoveEndState(bool changeAni)
    {
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.EvadeDouble_1);
        return true;
    }

    private bool ChangeAttackState(bool changeAni)
    {
        m_checkTime = 0.0f;

        if (mTargetCollider)
        {
            m_owner.LookAtTarget(mTargetCollider.GetCenterPos());
        }

        m_aniLength = m_owner.PlayAniImmediate(eAnimation.EvadeDouble_2);
        return true;
    }

    private Vector3 GetDestPos()
    {
        Vector3 targetPos = m_owner.transform.position;

        if (mTargetCollider)
        {
            targetPos = m_owner.GetTargetCapsuleEdgePos(mTargetCollider.Owner);
            targetPos.y = m_owner.transform.position.y;

            float dist = Vector3.Distance(m_owner.transform.position, targetPos);
            if (Physics.Raycast(m_owner.transform.position, (targetPos - m_owner.transform.position).normalized, out RaycastHit hitInfo,
                                dist, (1 << (int)eLayer.Wall) /*| (1 << (int)eLayer.Floor)*/))
            {
                targetPos = hitInfo.point;
                targetPos.y = m_owner.transform.position.y;
            }

            Vector3 v = (m_owner.transform.position - targetPos).normalized;
            targetPos += (v * 1.5f);
        }

        return targetPos;
    }

    private void ShowHook(bool show)
    {
        for (int i = 0; i < mTrails.Length; i++)
        {
            mTrails[i].Clear();
        }

        mEffHook.Stop();
        mEffHook.gameObject.SetActive(show);
    }
}
