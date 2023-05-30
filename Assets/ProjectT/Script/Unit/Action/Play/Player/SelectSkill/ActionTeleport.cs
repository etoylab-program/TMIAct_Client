
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionTeleport : ActionSelectSkillBase
{
    [Header("[Property]")]
    public float Duration = 0.15f;

    protected ActionDash    mActionDash     = null;
    protected UnitCollider  mTargetCollider = null;
    protected float         mDistance       = 0.0f;
    protected eAnimation    mCurAni         = eAnimation.None;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Teleport;

        conditionActionCommand = new eActionCommand[1];
        conditionActionCommand[0] = eActionCommand.Defence;

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.Defence;

        mActionDash = m_owner.actionSystem.GetAction<ActionDash>(eActionCommand.Defence);
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        Utility.IgnorePhysics(eLayer.Player, eLayer.Enemy);
        Utility.IgnorePhysics(eLayer.Player, eLayer.EnemyGate);

        if (m_owner.isGrounded)
        {
            mCurAni = eAnimation.Dash;
        }
        else
        {
            mCurAni = eAnimation.AirDash;
            m_owner.SetFallingRigidBody();
        }

        mTargetCollider = m_owner.GetMainTargetCollider(true);
        if (mTargetCollider)
        {
            ShowSkillNames(m_data);
            m_owner.LookAtTarget(mTargetCollider.GetCenterPos());

            Vector3 dest = mTargetCollider.Owner.transform.position - (mTargetCollider.Owner.transform.forward);
            dest.y = m_owner.transform.position.y;
            mDistance = Vector3.Distance(dest, m_owner.transform.position);

            mDir = (dest - m_owner.transform.position).normalized;

            RaycastHit hitInfo;
            if (Physics.Raycast(m_owner.transform.position, mDir, out hitInfo, mDistance,
                                (1 << (int)eLayer.EnvObject) | (1 << (int)eLayer.Wall) /*| (1 << (int)eLayer.Wall_Inside)*/) == true)
            {
                if (hitInfo.distance < mDistance)
                    mDistance = hitInfo.distance <= 1.0f ? 0.0f : hitInfo.distance;
            }

            mDir.y = 0.0f;
            m_owner.PlayAniImmediate(mCurAni);
        }
        else
        {
            SetNextAction(eActionCommand.Defence, null);
        }
    }

    public override IEnumerator UpdateAction()
    {
        if (mTargetCollider)
        {
            //m_owner.SetSuperArmor(Unit.eSuperArmor.Invincible, GetType());
            m_owner.TemporaryInvincible = true;

            float v = mDistance / Mathf.Ceil(Duration / Time.fixedDeltaTime);

            //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
            while (m_checkTime < Duration)
            {
                m_checkTime += Time.fixedDeltaTime;
                m_owner.rigidBody.MovePosition(m_owner.rigidBody.position + (mDir * v));

                yield return mWaitForFixedUpdate;
            }

            //m_owner.RestoreSuperArmor(Unit.eSuperArmor.Invincible, GetType());
            m_owner.TemporaryInvincible = false;
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        mActionDash.InitChainCount();

        if (mTargetCollider)
        {
            m_owner.LookAtTarget(mTargetCollider.GetCenterPos());
        }
    }

    public override void OnCancel()
    {
        base.OnCancel();
        mActionDash.InitChainCount();
    }

    public override float GetAtkRange()
    {
        return 20.0f;
    }
}
