using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSaikaEvadeDouble2Attack : ActionSelectSkillBase {
	public enum eStateType {
		JUMP,
		FALLING,
		FINISH_ATTACK,
	}

	private eAnimation		mCurAni			= eAnimation.Jump01;
	private State			mState			= new State();
	private UnitCollider	mTargetCollider = null;
	private Vector3			mDestPos		= Vector3.zero;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );
		actionCommand = eActionCommand.Teleport;

		conditionActionCommand = new eActionCommand[1];
		conditionActionCommand[0] = eActionCommand.Defence;

		cancelActionCommand = new eActionCommand[1];
		cancelActionCommand[0] = eActionCommand.Defence;

		superArmor = Unit.eSuperArmor.Lv1;

		mState.Init( 3 );
		mState.Bind( eStateType.JUMP, ChangeJumpState );
		mState.Bind( eStateType.FALLING, ChangeFallingState );
		mState.Bind( eStateType.FINISH_ATTACK, ChangeFinishAttackState );
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		ShowSkillNames( m_data );

		mState.ChangeState( eStateType.JUMP, true );
	}

	public override IEnumerator UpdateAction() {
		bool fastFall = false;

		while ( !m_endUpdate ) {
			switch ( (eStateType)mState.current ) {
				case eStateType.JUMP:
					m_checkTime += m_owner.fixedDeltaTime;
					if ( m_checkTime >= m_aniLength ) {
						mState.ChangeState( eStateType.FALLING, true );
					}
					break;

				case eStateType.FALLING:
					if ( m_owner.isGrounded ) {
						mState.ChangeState( eStateType.FINISH_ATTACK, true );
					}
					break;

				case eStateType.FINISH_ATTACK:
					m_checkTime += m_owner.fixedDeltaTime;
					if ( m_checkTime >= m_aniLength ) {
						m_endUpdate = true;
					}
					break;
			}

			if ( (eStateType)mState.current == eStateType.FALLING ) {
				if ( !fastFall && m_owner.isFalling ) {
					m_checkTime += m_owner.fixedDeltaTime;
					if ( m_checkTime >= m_aniLength ) {
						m_owner.cmptJump.SetFastFall();
						fastFall = true;
					}
				}
			}

			m_owner.cmptJump.UpdateJump();

			if ( fastFall && mTargetCollider != null && (eStateType)mState.current == eStateType.FALLING ) {
				if ( Vector3.Distance( m_owner.transform.position, new Vector3( mDestPos.x, m_owner.transform.position.y, mDestPos.z ) ) > mTargetCollider.radius ) {
					Vector3 v = mTargetCollider.Owner.transform.position + ( mTargetCollider.Owner.transform.forward * mTargetCollider.radius ) - m_owner.transform.position;
					m_owner.cmptMovement.UpdatePosition( v, m_owner.speed * 10.0f, true );
				}
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override float GetAtkRange() {
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( eAnimation.EvadeDouble2_1 );
		if ( evt == null ) {
			Debug.LogError( eAnimation.EvadeDouble2_1.ToString() + "공격 이벤트가 없네??" );
			return 0.0f;
		}
		else if ( evt.visionRange <= 0.0f ) {
			Debug.LogError( eAnimation.EvadeDouble2_1.ToString() + "Vistion Range가 0이네??" );
		}

		return evt.visionRange;
	}

	public override void OnEnd() {
		base.OnEnd();
		mOwnerPlayer.ResetPenetrate();
	}

	public override void OnCancel() {
		base.OnCancel();
		mOwnerPlayer.ResetPenetrate();
	}

	private bool ChangeJumpState( bool changeAni ) {
		mCurAni = eAnimation.Jump01;

		m_checkTime = 0.0f;
		m_aniLength = m_owner.PlayAniImmediate( mCurAni );
		m_owner.cmptJump.StartJump( 15.0f, true, m_aniLength );

		return true;
	}

	private bool ChangeFallingState( bool changeAni ) {
		Unit target = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
		if ( target ) {
			m_owner.LookAtTarget( target.transform.position );
			mTargetCollider = target.MainCollider;

			mDestPos = m_owner.transform.position;
			mTargetCollider = m_owner.GetMainTargetCollider( true );
			if ( mTargetCollider ) {
				mDestPos = m_owner.GetTargetCapsuleEdgePos( mTargetCollider.Owner );
			}
		}

		mCurAni = eAnimation.EvadeDouble2;

		m_checkTime = 0.0f;
		m_aniLength = m_owner.PlayAniImmediate( mCurAni );

		return true;
	}

	private bool ChangeFinishAttackState( bool changeAni ) {
		mCurAni = eAnimation.EvadeDouble2_1;

		m_checkTime = 0.0f;
		m_aniLength = m_owner.PlayAniImmediate( mCurAni );

		if ( mValue2 > 0.0f ) {
			ChangeAttackBehaviour( mCurAni, eBehaviour.DownAttack );
		}

		if ( mValue3 > 0.0f ) {
			ChangeAttackBehaviour( mCurAni, eBehaviour.KnockBackAttack );
		}

		return true;
	}

	private void ChangeAttackBehaviour( eAnimation animation, eBehaviour behaviour ) {
		List<AniEvent.sEvent> allAttackEventList = m_owner.aniEvent.GetAllAttackEvent( animation );
		for ( int i = 0; i < allAttackEventList.Count; i++ ) {
			allAttackEventList[i].behaviour = behaviour;
		}
	}
}
