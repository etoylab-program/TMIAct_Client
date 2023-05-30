
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionJingleiDash : ActionDash
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        cancelDuringSameActionCount = 2;
    }
}
