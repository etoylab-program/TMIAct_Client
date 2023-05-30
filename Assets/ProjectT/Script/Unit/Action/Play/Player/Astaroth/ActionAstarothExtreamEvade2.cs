
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionAstarothExtreamEvade2 : ActionExtreamEvade {
	private float				mDuration		= 0.0f;
	private List<PlayerMinion>	mListMinion		= new List<PlayerMinion>();
	private int					mMinionCount	= 0;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		mDuration = GameInfo.Instance.BattleConfig.BuffDuration + mValue1;
		mMinionCount = ( (int)m_data.IncValue1 ) + 1;

		for ( int i = 0; i < mMinionCount; i++ ) {
			int id = 16;
			if ( mValue2 > 0.0f ) {
				id = 17;
			}

			PlayerMinion minion = GameSupport.CreatePlayerMinion( id, mOwnerPlayer );
			mListMinion.Add( minion );
		}
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		StartEvadingBuff();
	}

	private void StartEvadingBuff() {
		mOwnerPlayer.ExtreamEvading = true;

		StopCoroutine( "EndEvadingBuff" );
		StartCoroutine( "EndEvadingBuff", mDuration );
	}

	public override void OnEnd() {
		base.OnEnd();
		IsSkillEnd = false;
	}

	public override void OnCancel() {
		base.OnCancel();
		AllMinionDeactivate();
	}

	protected override IEnumerator ContinueDash() {
		mOwnerPlayer.ContinuingDash = true;
		ActionDash actionDash = mOwnerPlayer.actionSystem.GetAction<ActionDash>( eActionCommand.Defence );

		m_checkTime = 0.0f;
		bool end = false;
		float dashAniLength = m_owner.PlayAniImmediate( actionDash.CurAni );

		if ( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
			m_owner.Input.LockBtnFlag( InputController.ELockBtnFlag.ATTACK | InputController.ELockBtnFlag.DASH | InputController.ELockBtnFlag.USKILL |
									  InputController.ELockBtnFlag.WEAPON | InputController.ELockBtnFlag.SUPPORTER );
		}

		m_owner.Input.LockDirection( true );
		m_owner.TemporaryInvincible = true;

		if ( m_owner.isGrounded == true ) {
			if ( actionDash.ShowMeshOnFrontDash && !actionDash.IsNoDir ) {
				m_owner.ShowMesh( false );
			}
		}

		while ( !end ) {
			m_checkTime += Time.fixedDeltaTime;

			if ( m_checkTime < actionDash.DashTime ) {
				m_owner.cmptMovement.UpdatePosition( actionDash.Dir, m_owner.originalSpeed * actionDash.CurDashSpeedRatio, true );
			}
			else if ( m_checkTime >= dashAniLength ) {
				end = true;
			}
			else if ( m_checkTime >= actionDash.DashTime ) {
				m_owner.ShowMesh( true );
			}

			yield return mWaitForFixedUpdate;
		}

		if ( !m_owner.isGrounded ) {
			m_owner.SetFallingRigidBody();
			while ( !m_owner.isGrounded ) {
				yield return null;
			}
		}

		m_owner.Input.LockDirection( false );

		if ( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
			m_owner.Input.LockBtnFlag( InputController.ELockBtnFlag.NONE );
		}

		mOwnerPlayer.TemporaryInvincible = false;
		mOwnerPlayer.ContinuingDash = false;
	}

	private IEnumerator EndEvadingBuff( float duration ) {
		StartSummon();
		yield return StartCoroutine( ContinueDash() );

		m_checkTime = 0.0f;

		while ( true ) {
			if ( !World.Instance.IsPause ) {
				m_checkTime += Time.fixedDeltaTime;
			}

			if ( m_checkTime >= duration ) {
				mOwnerPlayer.ExtreamEvading = false;
				IsSkillEnd = true;

				break;
			}

			yield return mWaitForFixedUpdate;
		}

		if ( !World.Instance.IsEndGame && !World.Instance.ProcessingEnd ) {
			float aniLength = 0.0f;
			for ( int i = 0; i < mListMinion.Count; i++ ) {
				PlayerMinion minion = mListMinion[i];
				if ( !minion.IsActivate() ) {
					continue;
				}

				minion.StopBT();

				aniLength = minion.PlayAniImmediate( eAnimation.Die );
				minion.StartDissolve( aniLength, false, new Color( 0.169f, 0.0f, 0.47f ) );
			}

			yield return new WaitForSeconds( aniLength );
		}

		AllMinionDeactivate();
	}

	private void StartSummon() {
		AllMinionDeactivate();

		Vector3 centerPos = World.Instance.EnemyMgr.GetCenterPosOfEnemies( mOwnerPlayer );

		float aniSpeed = 1.0f;
		if ( SetAddAction ) {
			aniSpeed = 1.0f + AddActionValue1;
		}

		for ( int i = 0; i < mMinionCount; i++ ) {
			PlayerMinion minion = mListMinion[i];
			Utility.SetLayer( minion.gameObject, (int)eLayer.PlayerClone, true );

			Vector3 pos = centerPos;
			pos.y = m_owner.posOnGround.y;

			int randX = UnityEngine.Random.Range( -30, 31 );
			int randZ = UnityEngine.Random.Range( -30, 31 );

			pos.x += (float)randX / 10.0f;
			pos.z += (float)randZ / 10.0f;

			minion.SetInitialPosition( pos, mOwnerPlayer.transform.rotation );
			minion.SetMinionAttackPower( mOwnerPlayer.gameObject.layer, mOwnerPlayer.attackPower * mValue3 );
			minion.Activate();
			minion.StartBT();

			minion.aniEvent.SetAniSpeed( aniSpeed );

			minion.PlayAniImmediate( eAnimation.Appear );
			minion.StartDissolve( 0.5f, true, new Color( 0.169f, 0.0f, 0.47f ) );
		}
	}

	private void AllMinionDeactivate() {
		for ( int i = 0; i < mListMinion.Count; i++ ) {
			mListMinion[i].StopBT();
			mListMinion[i].Deactivate();
		}
	}
}
