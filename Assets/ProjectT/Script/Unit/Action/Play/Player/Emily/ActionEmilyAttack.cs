
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionEmilyAttack : ActionSelectSkillBase
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Attack01;
    }

	public override float GetAtkRange() {
		return 20.0f;
	}
}
