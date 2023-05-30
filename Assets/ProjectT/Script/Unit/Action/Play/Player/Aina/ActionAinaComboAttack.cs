
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionAinaComboAttack : ActionComboAttack {
	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		attackAnimations = new eAnimation[5];
		attackAnimations[0] = eAnimation.Attack06;
		attackAnimations[1] = eAnimation.Attack02;
		attackAnimations[2] = eAnimation.Attack03;
		attackAnimations[3] = eAnimation.Attack07;
		attackAnimations[4] = eAnimation.Attack05;

		mOriginalAtkAnis = new eAnimation[attackAnimations.Length];
		for ( int i = 0; i < attackAnimations.Length; i++ ) {
			mOriginalAtkAnis[i] = attackAnimations[i];
		}
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		m_owner.SetSpeedRate( mValue1 / (float)eCOUNT.MAX_BO_FUNC_VALUE );
	}

	public override void OnCancel() {
		base.OnCancel();
		m_owner.SetSpeedRate( 0.0f );
	}

	public override void OnEnd() {
		base.OnEnd();
		m_owner.SetSpeedRate( 0.0f );
	}

	public override void RestoreAttackAnimations() {
		attackAnimations = new eAnimation[mOriginalAtkAnis.Length];
		for ( int i = 0; i < mOriginalAtkAnis.Length; i++ ) {
			attackAnimations[i] = mOriginalAtkAnis[i];
		}
	}
}
