
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionEnemyAsagiCloneAttack : ActionEnemyTaimaninBase
{
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

        m_owner.PlayAniImmediate(eAnimation.ChargeEnd);
        m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(eAnimation.ChargeEnd);
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while(!m_endUpdate)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if(m_checkTime >= m_aniCutFrameLength)
            {
                m_endUpdate = true;
            }

            yield return mWaitForFixedUpdate;
        }

        Unit clone = m_owner.GetClone(0);
        if (clone)
        {
            ActionCloneHomingAttack action = clone.actionSystem.GetAction<ActionCloneHomingAttack>(eActionCommand.CloneHomingAttack);
            if (action)
            {
                m_owner.ShowClone(0, m_owner.transform.position, m_owner.transform.rotation);
                clone.CommandAction(eActionCommand.CloneHomingAttack, new ActionParamAttack(0, mTargetCollider.Owner, 0.0f));
            }
        }
    }
}
