using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionGuardianShisuiComboAttack : ActionGuardianComboAttack {
	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		_AttackAnimations = new eAnimation[4];
		_AttackAnimations[0] = eAnimation.Attack01;
		_AttackAnimations[1] = eAnimation.Attack02;
		_AttackAnimations[2] = eAnimation.Attack03;
		_AttackAnimations[3] = eAnimation.Attack04;

		mOriginAttackAnimations = new eAnimation[_AttackAnimations.Length];
		for ( int i = 0; i < _AttackAnimations.Length; i++ ) {
			mOriginAttackAnimations[i] = _AttackAnimations[i];
		}
	}
}
