
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionRinkoTeleportAttack : ActionTeleport
{
    public enum eState
    {
        Start,
        //End,
        Attack,
    }


    private State   mState      = new State();
    private float   mMoveTime   = 0.0f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

		mState.Init(2);
        mState.Bind(eState.Start, ChangeStartState);
        //mState.Bind(eState.End, ChangeEndState);
        mState.Bind(eState.Attack, ChangeAttackState);

        if(mValue1 > 0.0f)
        {
            List<AniEvent.sEvent> list = m_owner.aniEvent.GetAllAttackEvent(eAnimation.EvadeDouble_1);
            for(int i = 0; i < list.Count; i++)
            {
                list[i].behaviour = eBehaviour.StunAttack;
            }
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        mTargetCollider = m_owner.GetMainTargetCollider(true);
        if (mTargetCollider)
        {
            m_owner.LookAtTarget(mTargetCollider.GetCenterPos());
            
            m_owner.SetMainTarget(mTargetCollider.Owner);
            //World.Instance.InGameCamera.TurnToTarget(mTargetCollider.GetCenterPos(), 2.0f);
        }

        mState.ChangeState(eState.Start, true);

        Vector3 destPos = GetDestPos();
        Vector3 dir = (destPos - m_owner.transform.position).normalized;

        mMoveTime = m_aniLength;
        m_owner.StartStepForward(mMoveTime, destPos, EndStartState);
    }

    public override IEnumerator UpdateAction()
    {
        m_checkTime = 0.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            m_checkTime += m_owner.fixedDeltaTime;

            switch ((eState)mState.current)
            {
                case eState.Start:
                    if(m_owner.holdPositionRef > 0 || Vector3.Distance(m_owner.transform.position, GetDestPos()) <= 1.5f || m_checkTime > (mMoveTime * 1.5f))
                    {
                        EndStartState();
                    }
                    break;

                /*case eState.End:
                    if(m_checkTime >= m_aniLength)
                    {
                        mState.ChangeState(eState.Attack, true);
                    }
                    break;*/

                case eState.Attack:
                    if (m_checkTime >= m_aniLength)
                    {
                        m_endUpdate = true;
                    }
                    break;
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnCancel()
    {
        base.OnCancel();
        m_endUpdate = true;
    }

    private bool ChangeStartState(bool changeAni)
    {
        m_checkTime = 0.0f;
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.EvadeDouble);
        
        return true;
    }

    /*private bool ChangeEndState(bool changeAni)
    {
        m_checkTime = 0.0f;
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.TeleportEnd);

        return true;
    }*/

    private bool ChangeAttackState(bool changeAni)
    {
        m_checkTime = 0.0f;
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.EvadeDouble_1);

        return true;
    }

    private void EndStartState()
    {
        m_owner.StopStepForward();
        mState.ChangeState(eState.Attack, true);
    }

    private Vector3 GetDestPos()
    {
        Vector3 targetPos = m_owner.transform.position;

        if (mTargetCollider)
        {
            targetPos = m_owner.GetTargetCapsuleEdgePosForTeleport( mTargetCollider.Owner );
            targetPos.y = m_owner.transform.position.y;

            Vector3 v = (m_owner.transform.position - targetPos).normalized;
            targetPos += v;

            float dist = Vector3.Distance(m_owner.transform.position, targetPos);
            if (Physics.Raycast(m_owner.transform.position, (targetPos - m_owner.transform.position).normalized, out RaycastHit hitInfo,
                                dist, (1 << (int)eLayer.Wall) /*| (1 << (int)eLayer.Floor)*/))
            {
                targetPos = hitInfo.point;
                targetPos.y = m_owner.transform.position.y;
            }
        }

        return targetPos;
    }
}
