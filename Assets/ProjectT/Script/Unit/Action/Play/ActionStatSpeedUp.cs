
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionStatSpeedUp : ActionBase
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.StatSpeedUp;
    }
}
