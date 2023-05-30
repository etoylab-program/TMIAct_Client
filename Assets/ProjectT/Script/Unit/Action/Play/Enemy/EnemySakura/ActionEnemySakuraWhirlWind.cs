
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionEnemySakuraWhirlWind : ActionEnemyTaimaninBase
{
    [Header("[WhirlWind Property]")]
    public float RotateCount = 3;

    private float mCurRotateCount = 0;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.ChargingAttack;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        mTargetCollider = m_owner.GetMainTargetCollider(true);
        if (mTargetCollider)
        {
            m_owner.LookAtTarget(mTargetCollider.GetCenterPos());
        }

        mCurRotateCount = RotateCount;
    }

    public override IEnumerator UpdateAction()
    {
        if(mTargetCollider == null)
        {
            m_endUpdate = false;
            yield break;
        }

        GoToTarget(mTargetCollider.Owner);

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (mCurRotateCount > 0)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime >= m_aniLength)
            {
                m_checkTime = 0.0f;

                m_aniLength = m_owner.PlayAniImmediate(eAnimation.ChargingAttack);
                if (Vector3.Distance(mTargetCollider.Owner.transform.position, m_owner.transform.position) > 1.5f)
                {
                    GoToTarget(mTargetCollider.Owner);
                }

                --mCurRotateCount;
            }

            yield return mWaitForFixedUpdate;
        }

        m_aniLength = m_owner.PlayAniImmediate(eAnimation.ChargeEnd2);
        yield return new WaitForSeconds(m_aniLength);
    }

    private void GoToTarget(Unit target)
    {
        Vector3 targetPos = m_owner.GetTargetCapsuleEdgePos(target);
        targetPos.y = m_owner.transform.position.y;

        float dist = Vector3.Distance(m_owner.transform.position, targetPos);
        if (Physics.Raycast(m_owner.transform.position, (targetPos - m_owner.transform.position).normalized, out RaycastHit hitInfo,
                            dist, (1 << (int)eLayer.Wall) /*| (1 << (int)eLayer.Floor)*/))
        {
            targetPos = hitInfo.point;
            targetPos.y = m_owner.transform.position.y;
        }

        Quaternion q = Quaternion.LookRotation(targetPos);
        q = Quaternion.Euler(0.0f, q.eulerAngles.y, 0.0f);
        m_owner.SetInitialPosition(targetPos, q);
    }
}
