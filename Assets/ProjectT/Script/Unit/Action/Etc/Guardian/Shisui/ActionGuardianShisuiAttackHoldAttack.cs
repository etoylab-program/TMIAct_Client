using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionGuardianShisuiAttackHoldAttack : ActionGuardianBase {
	public enum eStateType {
		ATTACK01,
		ATTACK02,
		FALLING,
	}

	private eAnimation	mCurAni			= eAnimation.AttackHold;
	private State		mState			= new State();
	private Vector2		mTeleportRatio	= Vector2.zero;
	private int			mActionCount	= 1;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		actionCommand = eActionCommand.AttackDuringAttack;

		extraCondition = new eActionCondition[4];
		extraCondition[0] = eActionCondition.Grounded;
		extraCondition[1] = eActionCondition.NoUsingSkill;
		extraCondition[2] = eActionCondition.NoUsingQTE;
		extraCondition[3] = eActionCondition.NoUsingUSkill;

		cancelActionCommand = new eActionCommand[1];
		cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

		mTeleportRatio = new Vector2( 3.0f, 0.0f );

		mState.Init( 3 );
		mState.Bind( eStateType.ATTACK01, Attack01State );
		mState.Bind( eStateType.ATTACK02, Attack02State );
		mState.Bind( eStateType.FALLING, Falling );
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		if ( mValue3 > 0.0f ) {
			mActionCount = 2;
		}

		ShowSkillNames( m_data );
	}

	public override IEnumerator UpdateAction() {
		if ( mPlayerGuardian != null && mPlayerGuardian.IsCommandAction && mPlayerGuardian.GetActionTarget() ) {
			yield return mPlayerGuardian.TeleportGuardian( PlayerGuardian.eArrowType.UP_FORWARD, mTeleportRatio, true );
		}

		m_owner.cmptRotate.UpdateRotation( GetTargetDir(), true );

		mState.ChangeState( eStateType.ATTACK01, true );

		bool isHolding = false;
		bool isFastFalling = false;
		int currentActionCount = 1;

		while ( m_endUpdate == false ) {
			m_checkTime += Time.fixedDeltaTime;

			switch ( (eStateType)mState.current ) {
				case eStateType.ATTACK01: {					
					if ( currentActionCount < mActionCount ) {
						if ( m_checkTime >= m_aniLength ) {
							mState.ChangeState( eStateType.ATTACK02, true );
						}
					}
					else {
						if ( m_checkTime >= m_aniCutFrameLength ) {
							mState.ChangeState( eStateType.FALLING, true );
						}
					}
				}
				break;

				case eStateType.ATTACK02: {
					if ( m_checkTime >= m_aniCutFrameLength ) {
						mState.ChangeState( eStateType.FALLING, true );
					}
				}
				break;

				case eStateType.FALLING: {
					if ( m_checkTime >= m_aniLength ) {
						m_endUpdate = true;
					}
				}
				break;
			}

			if ( mPlayerGuardian != null && mPlayerGuardian.IsCommandAction ) {
				if ( (eStateType)mState.current >= eStateType.FALLING ) {
					if ( isFastFalling == false && m_owner.isFalling ) {
						isFastFalling = true;
						m_owner.cmptJump.SetFastFall();
						m_owner.alwaysKinematic = false;
						m_owner.rigidBody.isKinematic = false;
					}
				}

				if ( (eStateType)mState.current < eStateType.FALLING || m_owner.isGrounded == false ) {
					m_owner.cmptJump.UpdateJump();
				}

				if ( isHolding == false && m_owner.isGrounded == false && m_owner.cmptJump.highest >= 0.8f ) {
					m_owner.cmptJump.Holding();
					isHolding = true;
				}
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override float GetAtkRange() {
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( eAnimation.AttackHold );
		if ( evt == null ) {
			Debug.LogError( eAnimation.AttackHold.ToString() + "공격 이벤트가 없네??" );
			return 0.0f;
		}
		else if ( evt.visionRange <= 0.0f ) {
			Debug.LogError( eAnimation.AttackHold.ToString() + "Vistion Range가 0이네??" );
		}

		return evt.visionRange;
	}

	private bool Attack01State( bool changeAni ) {
		mCurAni = eAnimation.AttackHold;

		m_checkTime = 0.0f;
		m_aniLength = m_owner.PlayAniImmediate( mCurAni );
		m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength( mCurAni );

		if ( mPlayerGuardian != null && mPlayerGuardian.IsCommandAction ) {
			m_owner.cmptJump.StartJump( 12.0f, true, m_aniLength );
		}

		return true;
	}

	private bool Attack02State( bool changeAni ) {
		mCurAni = eAnimation.AttackHold_1;

		m_checkTime = 0.0f;
		m_aniLength = m_owner.PlayAniImmediate( mCurAni );
		m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength( mCurAni );

		return true;
	}

	private bool Falling( bool changeAni ) {
		m_owner.cmptJump.EndHolding();

		return true;
	}
}
