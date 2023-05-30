
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Sakura : Player
{
    protected override void OnEventDefence( IActionBaseParam param = null )
    {
        if (m_actionSystem.IsCurrentUSkillAction() == true)
            return;

        ActionDash actionDash = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionDash;
        ActionTeleport actionTeleport = m_actionSystem.GetAction<ActionTeleport>(eActionCommand.Teleport);
        if (actionTeleport && actionTeleport.PossibleToUse && actionDash && actionDash.IsPossibleToDashAttack() && GetMainTargetCollider(true))
        {
            float checkTime = actionDash.GetEvadeCutFrameLength();
            if (World.Instance.UIPlay.btnDash.deltaTime < checkTime)
            {
                CommandAction(eActionCommand.Teleport, null);
                return;
            }
        }

        CommandAction( eActionCommand.Defence, param );
    }
}
