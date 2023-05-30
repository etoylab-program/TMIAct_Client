
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionRinkoDash : ActionDash
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        conditionActionCommand = new eActionCommand[4];
        conditionActionCommand[0] = eActionCommand.MoveByDirection;
        conditionActionCommand[1] = eActionCommand.Attack01;
        conditionActionCommand[2] = eActionCommand.Hit;
        conditionActionCommand[3] = eActionCommand.Defence;
    }
}
