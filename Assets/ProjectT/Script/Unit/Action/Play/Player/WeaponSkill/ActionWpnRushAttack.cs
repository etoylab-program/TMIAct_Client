
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionWpnRushAttack : ActionWeaponSkillBase
{
    private ActionParamFromBO mActionParamFromBO = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.WpnRushAttack;
    }

    public override void OnStart(IActionBaseParam param)
    {
        ActionSelectSkillBase actionRushAttack = m_owner.actionSystem.GetAction<ActionSelectSkillBase>(eActionCommand.RushAttack);
        if (!actionRushAttack.PossibleToUse)
        {
            return;
        }

        base.OnStart(param);
        mActionParamFromBO = param as ActionParamFromBO;

        SetNextAction(eActionCommand.RushAttack, null);
    }
}
