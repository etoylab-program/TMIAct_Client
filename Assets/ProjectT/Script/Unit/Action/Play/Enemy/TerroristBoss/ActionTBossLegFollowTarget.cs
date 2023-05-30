
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionTBossLegFollowTarget : ActionEnemyBase
{
    public float    MaxAngle    = 45.0f;
    public float    RotateSpeed = 5.0f;

    private UnitCollider    mTargetCollider = null;
    private Vector3         mDir            = Vector3.zero;
    private float           mAngle          = 0.0f;
    private bool            mSkip           = false;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.TBossLegFollowTarget;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        mTargetCollider = m_owner.GetMainTargetCollider(true);
        if (mTargetCollider == null)
            mTargetCollider = World.Instance.Player.MainCollider;

        mDir = (mTargetCollider.Owner.transform.position - m_owner.transform.position).normalized;
        mSkip = false;

        mAngle = Vector3.SignedAngle(m_owner.transform.forward, mDir, Vector3.up);
        if (Mathf.Abs(mAngle) <= (MaxAngle * 0.5f) || (m_owner.child && m_owner.child.actionSystem.IsCurrentAnyAttackAction()))
            mSkip = true;

        if(!mSkip)
        {
            if (mAngle < 0.0f)
                m_aniLength = m_owner.PlayAniImmediate(eAnimation.TurnLeft);
            else
                m_aniLength = m_owner.PlayAniImmediate(eAnimation.TurnRight);

            float f = 1.0f;
            if (mAngle < 0.0f)
                f = -1.0f;

            float abs = Mathf.Abs(mAngle);
            mAngle = Mathf.Clamp(abs, 0.0f, MaxAngle) * f;
        }
    }

    public override IEnumerator UpdateAction()
    {
        Quaternion lookRot = Quaternion.Euler(0.0f, m_owner.transform.eulerAngles.y + mAngle, 0.0f);

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            if (mSkip)
                m_endUpdate = true;
            else
            {
                m_checkTime += m_owner.fixedDeltaTime;
                if (m_checkTime >= m_aniLength)
                    m_endUpdate = true;
                else
                    m_owner.transform.rotation = Quaternion.Slerp(m_owner.transform.rotation, lookRot, Mathf.Clamp01(RotateSpeed * m_owner.fixedDeltaTime));
            }

            yield return mWaitForFixedUpdate;
        }
    }
}
