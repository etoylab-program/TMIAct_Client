using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionShisuiEmergencyAttack : ActionSelectSkillBase {
	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		actionCommand = eActionCommand.EmergencyAttack;

		conditionActionCommand = new eActionCommand[1];
		conditionActionCommand[0] = eActionCommand.Defence;

		cancelActionCommand = new eActionCommand[1];
		cancelActionCommand[0] = eActionCommand.EmergencyAttack;

		if ( mValue3 > 0.0f ) {
			mDecreaseSkillCoolTimeSec = mValue3;
		}
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		mOwnerPlayer.Guardian.PlayAction( actionCommand, null );

		ShowSkillNames( m_data );
	}
}
