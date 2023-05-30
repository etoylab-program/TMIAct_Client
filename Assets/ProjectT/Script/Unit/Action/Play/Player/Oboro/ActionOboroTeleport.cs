
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionOboroTeleport : ActionTeleport
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        superArmor = Unit.eSuperArmor.Invincible;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        m_owner.PlayAniImmediate(eAnimation.EvadeDouble);
    }

    public override IEnumerator UpdateAction()
    {
        if (mTargetCollider)
        {
            m_owner.ShowMesh(false);
            float v = mDistance / Mathf.Ceil(Duration / Time.fixedDeltaTime);

            //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
            while (m_checkTime < Duration)
            {
                m_checkTime += Time.fixedDeltaTime;
                m_owner.rigidBody.MovePosition(m_owner.rigidBody.position + (mDir * v));

                yield return mWaitForFixedUpdate;
            }
            
            m_owner.ShowMesh(true);
        }
        else
        {
            m_owner.actionSystem.CancelCurrentAction();
        }
    }
}
