using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ActionGuardianMoveToOwner : ActionBase {
	private PlayerGuardian	mGuardian		= null;
	private Player			mOwnerPlayer	= null;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		actionCommand = eActionCommand.MoveToOwner;
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		mParamAI = param as ActionParamAI;

		if ( m_owner.aniEvent.HasAni( eAnimation.RunFast ) ) {
			m_owner.PlayAni( eAnimation.RunFast );
		}
		else {
			m_owner.PlayAni( eAnimation.Run );
		}

		//m_owner.aniEvent.SetShaderAlpha( "_Color", 0.5f );

		PlayerGuardian playerGuardian = m_owner as PlayerGuardian;
		if ( playerGuardian != null ) {
			mGuardian = playerGuardian;
			mOwnerPlayer = playerGuardian.OwnerPlayer;
		}

		m_owner.MainCollider.Enable( false );
	}

	public override IEnumerator UpdateAction() {
		if ( mOwnerPlayer == null || !mOwnerPlayer.IsShowMesh || mGuardian == null ) {
			m_endUpdate = true;
		}
		else {
			m_endUpdate = IsMinDistance();
		}

		while ( m_endUpdate == false ) {
			LookAtTarget( mOwnerPlayer );

			Vector3 dir = ( mOwnerPlayer.transform.position - m_owner.transform.position ).normalized;
			m_owner.cmptMovement.UpdatePosition( dir, m_owner.speed, false );

			m_endUpdate = IsMinDistance();

			yield return mWaitForFixedUpdate;
		}
	}

	public override void OnCancel() {
		base.OnCancel();

		m_endUpdate = true;
		m_owner.MainCollider.Enable( true );
	}

	public override void OnEnd() {
		base.OnEnd();

		m_endUpdate = true;
		m_owner.MainCollider.Enable( true );
	}

	private bool IsMinDistance() {
		if ( mOwnerPlayer == null ) {
			return true;
		}

		return Vector3.Distance( mOwnerPlayer.transform.position, m_owner.transform.position ) <= mGuardian.MinDistance;
	}
}
