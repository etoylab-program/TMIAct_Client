
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionEnemyTeleportBack : ActionEnemyBase
{
    protected float mDuration = 0.1f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.TeleportBack;

        extraCondition = new eActionCondition[1];
        extraCondition[0] = eActionCondition.Grounded;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        if (m_owner.holdPositionRef > 0)
        {
            return;
        }
        
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.Teleport);
    }

    public override IEnumerator UpdateAction()
    {
        if (m_owner.holdPositionRef > 0)
        {
            yield break;
        }
     
        yield return new WaitForSeconds(m_aniLength);

        m_owner.ShowMesh(false);
        //m_owner.SetSuperArmor(Unit.eSuperArmor.Invincible, GetType());
        m_owner.TemporaryInvincible = true;

        Utility.IgnorePhysics(eLayer.Player, eLayer.Enemy);

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_checkTime < mDuration)
        {
            m_checkTime += Time.fixedDeltaTime;
            yield return mWaitForFixedUpdate;
        }

        //m_owner.RestoreSuperArmor(Unit.eSuperArmor.Invincible, GetType());
        m_owner.TemporaryInvincible = false;
    }

    public override void OnEnd()
    {
        base.OnEnd();

        if (m_owner.holdPositionRef <= 0)
        {
            UnitCollider targetCollider = m_owner.GetMainTargetCollider(true);

            Transform[] edgePoints = World.Instance.EnemyMgr.GetEdgePoints();
            if (edgePoints != null && edgePoints.Length > 0)
            {
                int index = 0;
                float compare = 0.0f;

                Vector3 comparePos = m_owner.transform.position;
                if (targetCollider)
                {
                    comparePos = targetCollider.GetCenterPos();
                }

                for (int i = 0; i < edgePoints.Length; i++)
                {
                    float dist = Vector3.Distance(comparePos, edgePoints[i].position);
                    if (dist > compare)
                    {
                        index = i;
                        compare = dist;
                    }
                }

                m_owner.SetInitialPosition(edgePoints[index].position, m_owner.transform.rotation);
                if (targetCollider)
                {
                    m_owner.LookAtTarget(targetCollider.GetCenterPos());
                }
            }
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
