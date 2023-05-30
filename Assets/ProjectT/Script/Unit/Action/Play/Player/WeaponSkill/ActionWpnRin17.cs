
using System;
using System.Collections.Generic;
using UnityEngine;


public class ActionWpnRin17 : ActionWeaponSkillBase {
	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );
		actionCommand = eActionCommand.WpnRin17;

		extraCondition = new eActionCondition[4];
		extraCondition[0] = eActionCondition.Grounded;
		extraCondition[1] = eActionCondition.NoUsingSkill;
		extraCondition[2] = eActionCondition.NoUsingQTE;
		extraCondition[3] = eActionCondition.NoUsingUSkill;
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		ActionParamFromBO paramFromBO = param as ActionParamFromBO;

		ActionRinChargeAttack action = m_owner.actionSystem.GetAction<ActionRinChargeAttack>( eActionCommand.ChargingAttack );
		if ( action ) {
			action.SetAddAction = false;

			if ( paramFromBO.battleOptionData.timingType == BattleOption.eBOTimingType.UseAction ) {
				action.SetAddActionAutoRelease( paramFromBO.battleOptionData.value3, paramFromBO.battleOptionData.effId1 );

				action.AddActionValue1 = paramFromBO.battleOptionData.value;
				action.AddActionValue2 = paramFromBO.battleOptionData.value2;
			}
		}
	}
}
