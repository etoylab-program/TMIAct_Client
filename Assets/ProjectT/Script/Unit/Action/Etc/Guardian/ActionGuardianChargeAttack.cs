using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionGuardianChargeAttack : ActionGuardianBase {


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		actionCommand = eActionCommand.ChargingAttack;

		conditionActionCommand = new eActionCommand[2];
		conditionActionCommand[0] = eActionCommand.Idle;
		conditionActionCommand[1] = eActionCommand.MoveByDirection;

		extraCondition = new eActionCondition[4];
		extraCondition[0] = eActionCondition.Grounded;
		extraCondition[1] = eActionCondition.NoUsingSkill;
		extraCondition[2] = eActionCondition.NoUsingQTE;
		extraCondition[3] = eActionCondition.NoUsingUSkill;

		superArmor = Unit.eSuperArmor.Lv1;
	}

	public override void OnEnd() {
		base.OnEnd();

		SkipConditionCheck = false;
	}

	public override void OnCancel() {
		base.OnCancel();

		SkipConditionCheck = false;
		m_endUpdate = true;
	}
}
