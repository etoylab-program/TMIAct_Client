
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionEnemyRinkoTeleportAttack : ActionEnemyTaimaninBase
{
    public enum eState
    {
        Start,
        Attack,
    }


    private State mState = new State();


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.TeleportAttack;

		mState.Init(2);
        mState.Bind(eState.Start, ChangeStartState);
        mState.Bind(eState.Attack, ChangeAttackState);
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        mTargetCollider = m_owner.GetMainTargetCollider(true);
        if (mTargetCollider)
        {
            m_owner.LookAtTarget(mTargetCollider.GetCenterPos());
            m_owner.SetMainTarget(mTargetCollider.Owner);
        }

        mState.ChangeState(eState.Start, true);

        Vector3 destPos = GetDestPos();
        Vector3 dir = (destPos - m_owner.transform.position).normalized;
        m_owner.StartStepForward(m_aniLength, destPos, EndStartState);
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
                    break;

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

    private bool ChangeStartState(bool changeAni)
    {
        m_checkTime = 0.0f;
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.Teleport);

        return true;
    }

    private bool ChangeAttackState(bool changeAni)
    {
        m_checkTime = 0.0f;
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.TeleportAttack);

        return true;
    }

    private void EndStartState()
    {
        mState.ChangeState(eState.Attack, true);
    }

    private Vector3 GetDestPos()
    {
        Vector3 targetPos = m_owner.transform.position;

        if (mTargetCollider)
        {
            targetPos = m_owner.GetTargetCapsuleEdgePos(mTargetCollider.Owner);
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
