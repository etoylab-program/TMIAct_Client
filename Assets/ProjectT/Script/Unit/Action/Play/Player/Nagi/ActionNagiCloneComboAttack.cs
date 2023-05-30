
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ActionNagiCloneComboAttack : ActionComboAttack {
	private Coroutine	mCrHideClone	= null;
	private Unit		mClone			= null;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		attackAnimations = new eAnimation[4];
		attackAnimations[0] = eAnimation.Attack01;
		attackAnimations[1] = eAnimation.Attack02;
		attackAnimations[2] = eAnimation.Attack03;
		attackAnimations[3] = eAnimation.Attack04;

		mOriginalAtkAnis = new eAnimation[attackAnimations.Length];
		for( int i = 0; i < attackAnimations.Length; i++ ) {
			mOriginalAtkAnis[i] = attackAnimations[i];
		}

		for( int i = 0; i < attackAnimations.Length; i++ ) {
			List<AniEvent.sEvent> listAtkEvt = m_owner.aniEvent.GetAllAttackEvent(attackAnimations[i]);
			for( int j = 0; j < listAtkEvt.Count; j++ ) {
				listAtkEvt[j].atkRatio += listAtkEvt[j].atkRatio * ( mValue1 / (float)eCOUNT.MAX_BO_FUNC_VALUE );
			}
		}
	}

	public override void OnEnd() {
		base.OnEnd();

		Utility.StopCoroutine( this, ref mCrHideClone );
		mCrHideClone = StartCoroutine( HideClone() );
	}

	public override void OnCancel() {
		base.OnCancel();

		Utility.StopCoroutine( this, ref mCrHideClone );
		mCrHideClone = StartCoroutine( HideClone() );
	}

	public override void RestoreAttackAnimations() {
		attackAnimations = new eAnimation[mOriginalAtkAnis.Length];
		for( int i = 0; i < mOriginalAtkAnis.Length; i++ ) {
			attackAnimations[i] = mOriginalAtkAnis[i];
		}
	}

	protected override void StartAttack() {
		Utility.StopCoroutine( this, ref mCrHideClone );

		if( mClone == null ) {
			mClone = m_owner.GetClone( 0 );
		}

		mOwnerPlayer.ShowClone( 0, transform.position, transform.rotation, true );
		mClone.LockFollowAni = false;

		m_currentAni = GetCurAni();
		m_aniLength = m_owner.PlayAniImmediate( m_currentAni, 0.0f, false, true );

		m_atkRange = m_owner.aniEvent.GetFirstAttackEventRange( m_currentAni );
		m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength( m_currentAni );

		if( mLastAtkSuperArmor > Unit.eSuperArmor.None && IsLastAttack() ) {
			mChangedSuperArmorId = m_owner.SetSuperArmor( mLastAtkSuperArmor );
		}
	}

	protected override eAnimation GetCurAni() {
		return base.GetCurAni();
	}

	private IEnumerator HideClone() {
		yield return new WaitForSeconds( m_aniCutFrameLength );
		mOwnerPlayer.HideClone( 0 );
	}
}
