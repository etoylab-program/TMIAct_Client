using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionGuardianShisuiEvadeDouble2Attack : ActionGuardianBase {
	public enum eStateType {
		JUMP,
		FLY,
		FALLING,
	}

	private eAnimation		mCurAni			= eAnimation.Jump01;
	private State			mState			= new State();
	private Vector2			mTeleportRatio	= Vector2.zero;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		actionCommand = eActionCommand.Teleport;

		conditionActionCommand = new eActionCommand[1];
		conditionActionCommand[0] = eActionCommand.Defence;

		cancelActionCommand = new eActionCommand[1];
		cancelActionCommand[0] = eActionCommand.Defence;

		mTeleportRatio = new Vector2( 1.0f, 0.0f );

		mState.Init( 4 );
		mState.Bind( eStateType.JUMP, ChangeJumpState );
		mState.Bind( eStateType.FLY, ChangeFlyState );
		mState.Bind( eStateType.FALLING, ChangeFallingState );
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		ShowSkillNames( m_data );
	}

	public override IEnumerator UpdateAction() {
		if ( mPlayerGuardian != null && mPlayerGuardian.OwnerPlayer && mPlayerGuardian.IsCommandAction == false ) {
			if ( CancelActionOwnerPlayer() ) {
				mPlayerGuardian.OwnerPlayer.CommandAction( actionCommand, null );
			}
		}

		if ( mPlayerGuardian != null ) {
			yield return mPlayerGuardian.TeleportGuardian( PlayerGuardian.eArrowType.BACK, mTeleportRatio, false );
		}

		Unit actionTarget = mPlayerGuardian.GetActionTarget();
		if ( actionTarget != null ) {
			m_owner.LookAtTarget( actionTarget.transform.position );
		}

		mState.ChangeState( eStateType.JUMP, true );

		bool fastFall = false;

		while ( !m_endUpdate ) {
			m_checkTime += m_owner.fixedDeltaTime;

			switch ( (eStateType)mState.current ) {
				case eStateType.JUMP:
					if ( m_checkTime >= m_aniLength ) {
						mState.ChangeState( eStateType.FLY, true );
					}
					break;

				case eStateType.FLY:
					if ( m_checkTime >= m_aniLength ) {
						mState.ChangeState( eStateType.FALLING, true );
					}
					break;
				case eStateType.FALLING: {
					if ( m_checkTime >= m_aniLength ) {
						m_endUpdate = true;
					}
				}
				break;
			}

			if ( (eStateType)mState.current == eStateType.FALLING ) {
				if ( !fastFall && m_owner.isFalling ) {
					fastFall = true;
					m_owner.cmptJump.SetFastFall();
					m_owner.alwaysKinematic = false;
					m_owner.rigidBody.isKinematic = false;
				}
			}

			m_owner.cmptJump.UpdateJump();

			if ( (eStateType)mState.current == eStateType.FALLING && m_owner.transform.position.y <= 0.0f ) {
				m_endUpdate = true;
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override float GetAtkRange() {
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( eAnimation.EvadeDouble2_3 );
		if ( evt == null ) {
			Debug.LogError( eAnimation.EvadeDouble2_3.ToString() + "공격 이벤트가 없네??" );
			return 0.0f;
		}
		else if ( evt.visionRange <= 0.0f ) {
			Debug.LogError( eAnimation.EvadeDouble2_3.ToString() + "Vistion Range가 0이네??" );
		}

		return evt.visionRange;
	}

	private bool ChangeJumpState( bool changeAni ) {
		mCurAni = eAnimation.EvadeDouble2;

		m_checkTime = 0.0f;
		m_aniLength = m_owner.PlayAniImmediate( mCurAni );
		m_owner.cmptJump.StartJump( 15.0f, true, m_aniLength );

		return true;
	}

	private bool ChangeFlyState( bool changeAni ) {
		mCurAni = eAnimation.EvadeDouble2_1;

		m_checkTime = 0.0f;
		m_aniLength = m_owner.PlayAniImmediate( mCurAni );

		return true;
	}

	private bool ChangeFallingState( bool changeAni ) {
		mCurAni = eAnimation.EvadeDouble2_2;

		m_checkTime = 0.0f;
		m_aniLength = m_owner.PlayAniImmediate( mCurAni );

		return true;
	}

	private bool CancelActionOwnerPlayer() {
		if ( mPlayerGuardian.OwnerPlayer.actionSystem.currentAction && mPlayerGuardian.OwnerPlayer.actionSystem.currentAction.actionCommand == eActionCommand.Die ) {
			return false;
		}

		mPlayerGuardian.OwnerPlayer.actionSystem.CancelCurrentAction();
		return true;
	}
}
