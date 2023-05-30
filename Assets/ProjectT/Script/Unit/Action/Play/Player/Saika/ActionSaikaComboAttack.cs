using System.Collections.Generic;


public class ActionSaikaComboAttack : ActionComboAttack {

	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		attackAnimations = new eAnimation[6];
		attackAnimations[0] = eAnimation.Attack01;
		attackAnimations[1] = eAnimation.Attack02;
		attackAnimations[2] = eAnimation.Attack03;
		attackAnimations[3] = eAnimation.Attack04;
		attackAnimations[4] = eAnimation.Attack05;
		attackAnimations[5] = eAnimation.Attack06;

		mOriginalAtkAnis = new eAnimation[attackAnimations.Length];
		for ( int i = 0; i < attackAnimations.Length; i++ ) {
			mOriginalAtkAnis[i] = attackAnimations[i];
		}

		mForceLastAttackCutFrame = true;
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

	protected override eAnimation GetCurAni() {
		return base.GetCurAni();
	}
}
