
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionEnemyTeleportForward : ActionEnemyBase
{
    protected UnitCollider  mTargetCollider = null;
    protected Vector3       mDir            = Vector3.zero;
    protected float         mDistance       = 0.0f;
    protected float         mDuration       = 0.1f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.TeleportForward;

        extraCondition = new eActionCondition[1];
        extraCondition[0] = eActionCondition.Grounded;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        mTargetCollider = m_owner.GetMainTargetCollider(true);
        if (m_owner.holdPositionRef <= 0 && mTargetCollider.Owner)
        {
            m_owner.LookAtTarget(mTargetCollider.Owner.transform.position);
            m_aniLength = m_owner.PlayAniImmediate(eAnimation.Teleport);
        }
    }

    public override IEnumerator UpdateAction()
    {
        if (mTargetCollider)
        {
            yield return new WaitForSeconds(m_aniLength);
            SetDest();

            m_owner.ShowMesh(false);
            //m_owner.SetSuperArmor(Unit.eSuperArmor.Invincible, GetType());
            m_owner.TemporaryInvincible = true;

            Utility.IgnorePhysics(eLayer.Player, eLayer.Enemy);

            float v = mDistance / Mathf.Ceil(mDuration / Time.fixedDeltaTime);

            //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
            while (m_owner.holdPositionRef <= 0 && m_checkTime < mDuration)
            {
                m_checkTime += Time.fixedDeltaTime;
                m_owner.rigidBody.MovePosition(m_owner.rigidBody.position + (mDir * v));

                yield return mWaitForFixedUpdate;
            }

            //m_owner.RestoreSuperArmor(Unit.eSuperArmor.Invincible, GetType());
            m_owner.TemporaryInvincible = false;
        }
        else
        {
            m_owner.actionSystem.CancelCurrentAction();
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        
        if (mTargetCollider)
        {
            m_owner.LookAtTarget(mTargetCollider.Owner.transform.position);
        }

        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.Enemy);
    }

    public override void OnCancel()
    {
		m_owner.TemporaryInvincible = false;

        base.OnCancel();
        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.Enemy);
    }

    private void SetDest()
    {
        Vector3 dest = mTargetCollider.Owner.transform.position - mTargetCollider.Owner.transform.forward;
        dest.y = m_owner.transform.position.y;
        mDistance = Vector3.Distance(dest, m_owner.transform.position);

        mDir = (dest - m_owner.transform.position).normalized;

        RaycastHit hitInfo;
        if (Physics.Raycast(m_owner.transform.position, mDir, out hitInfo, mDistance, (1 << (int)eLayer.EnvObject) | (1 << (int)eLayer.Wall)))
        {
            if (hitInfo.distance < mDistance)
            {
                mDistance = hitInfo.distance <= 1.0f ? 0.0f : hitInfo.distance;
            }
        }

        mDir.y = 0.0f;
    }
}
