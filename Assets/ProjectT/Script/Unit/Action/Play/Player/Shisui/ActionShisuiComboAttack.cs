using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionShisuiComboAttack : ActionComboAttack {
	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		actionCommand = eActionCommand.Attack01;

		attackAnimations = new eAnimation[4];
		attackAnimations[0] = eAnimation.Attack01;
		attackAnimations[1] = eAnimation.Attack02;
		attackAnimations[2] = eAnimation.Attack03;
		attackAnimations[3] = eAnimation.Attack04;

		mOriginalAtkAnis = new eAnimation[attackAnimations.Length];
		for ( int i = 0; i < attackAnimations.Length; i++ ) {
			mOriginalAtkAnis[i] = attackAnimations[i];
		}

		mOwnerPlayer.AutoGuardianSkill = true;

		if ( mValue1 > 0.0f ) {
			if ( mOwnerPlayer ) {
				mOwnerPlayer.SetAttackPowerUpRate( mValue1 );
			}
		}

		if ( mValue2 > 0.0f ) {
			Shisui shisui = m_owner as Shisui;
			if ( shisui != null ) {
				shisui.SetAuto( true );
			}
		}

		if ( mValue3 > 0.0f ) {
			if ( mOwnerPlayer ) {
				mOwnerPlayer.SetGuardianAttackPowerRate( mValue3 );
			}
		}
	}
}
