
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionEnemyTeleport : ActionEnemyBase
{
    protected UnitCollider  mTargetCollider = null;
    protected Vector3       mDir            = Vector3.zero;
    protected float         mDistance       = 0.0f;
    protected float         mDuration       = 0.15f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Teleport;

        extraCondition = new eActionCondition[1];
        extraCondition[0] = eActionCondition.Grounded;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        if(m_owner.holdPositionRef > 0)
        {
            return;
        }

        Utility.IgnorePhysics(eLayer.Player, eLayer.Enemy);

        mTargetCollider = m_owner.GetMainTargetCollider(true);
        if (mTargetCollider.Owner)
        {
            m_owner.LookAtTarget(mTargetCollider.Owner.transform.position);

            Vector3 dest = mTargetCollider.Owner.transform.position - mTargetCollider.Owner.transform.forward;
            dest.y = m_owner.transform.position.y;
            mDistance = Vector3.Distance(dest, m_owner.transform.position);

            float minusDistance = 0;
            foreach (KeyValuePair<AniEvent.sAniInfo, List<AniEvent.sEvent>> aniEvent in m_owner.aniEvent.dicAniEvent)
            {
                if (aniEvent.Key.stateName.Equals(eAnimation.Dash.ToString()) == false)
                {
                    continue;
                }

                foreach (AniEvent.sEvent info in aniEvent.Value)
                {
                    if (info.behaviour != eBehaviour.StepForward)
                    {
                        continue;
                    }

                    minusDistance += info.stepForwardDist;
                }

                break;
            }

            mDistance -= minusDistance;
            if (mDistance < 0)
            {
                mDistance = 0;
            }

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
            m_owner.PlayAniImmediate(eAnimation.Dash);
        }
    }

    public override IEnumerator UpdateAction()
    {
        if (mTargetCollider && m_owner.holdPositionRef <= 0)
        {
            //m_owner.SetSuperArmor(Unit.eSuperArmor.Invincible, GetType());
            m_owner.TemporaryInvincible = true;

            float v = mDistance / Mathf.Ceil(mDuration / Time.fixedDeltaTime);

            //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
            while (m_checkTime < mDuration)
            {
                m_checkTime += Time.fixedDeltaTime;
                m_owner.rigidBody.MovePosition(m_owner.rigidBody.position + (mDir * v));

                yield return mWaitForFixedUpdate;
            }

            //m_owner.SetSuperArmor(Unit.eSuperArmor.Invincible, GetType());
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
}
