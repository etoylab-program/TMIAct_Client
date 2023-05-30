
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSoraTimingHoldAttack : ActionSelectSkillBase {
	public enum eState {
		PREPARE,
		TELEPORT,
		ATTACK,
	}


	private State		mState		= new State();
	private Unit		mTarget		= null;
	private eAnimation	mCurAni		= eAnimation.EvadeTiming_1;
	private float		mDistance	= 3.0f;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );
		actionCommand = eActionCommand.TimingHoldAttack;

		extraCondition = new eActionCondition[1];
		extraCondition[0] = eActionCondition.Grounded;

		superArmor = Unit.eSuperArmor.Lv1;

		mState.Init( 3 );
		mState.Bind( eState.PREPARE, ChangePrepareState );
		mState.Bind( eState.TELEPORT, ChangeTeleportState );
		mState.Bind( eState.ATTACK, ChangeAttackState );

		if( mValue1 > 0.0f ) {
			mCurAni = eAnimation.EvadeTiming_2;
		}
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		ShowSkillNames( m_data );

		mState.ChangeState( eState.PREPARE, true );
	}

	public override IEnumerator UpdateAction() {
		while( !m_endUpdate ) {
			m_checkTime += m_owner.fixedDeltaTime;

			switch( (eState)mState.current ) {
				case eState.PREPARE:
					if( m_checkTime >= m_aniCutFrameLength ) {
						mState.ChangeState( eState.TELEPORT, true );
					}
					break;

				case eState.TELEPORT:
					if( m_checkTime >= m_aniLength ) {
						mState.ChangeState( eState.ATTACK, true );
					}
					break;

				case eState.ATTACK:
					if( m_checkTime >= m_aniCutFrameLength ) {
						m_endUpdate = true;
					}
					break;
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override float GetAtkRange() {
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( mCurAni );
		if( evt == null ) {
			Debug.LogError( mCurAni.ToString() + "공격 이벤트가 없네??" );
			return 0.0f;
		}
		else if( evt.visionRange <= 0.0f ) {
			Debug.LogError( mCurAni.ToString() + "Vistion Range가 0이네??" );
		}

		return evt.visionRange;
	}

	public override void OnEnd() {
		base.OnEnd();

		m_owner.ShowMesh( true );
		m_owner.SetGroundedRigidBody();

		Utility.SetPhysicsLayerCollision( eLayer.Player, eLayer.Enemy );
		Utility.SetPhysicsLayerCollision( eLayer.Player, eLayer.EnemyGate );
	}

	public override void OnCancel() {
		base.OnCancel();

		m_owner.ShowMesh( true );
		m_owner.SetGroundedRigidBody();

		Utility.SetPhysicsLayerCollision( eLayer.Player, eLayer.Enemy );
		Utility.SetPhysicsLayerCollision( eLayer.Player, eLayer.EnemyGate );
	}

	private bool ChangePrepareState( bool changeAni ) {
		m_aniLength = m_owner.PlayAniImmediate( eAnimation.EvadeTiming );
		m_aniCutFrameLength = m_owner.aniEvent.GetCurCutFrameLength();

		m_checkTime = 0.0f;
		return true;
	}

	private bool ChangeTeleportState( bool changeAni ) {
		Utility.IgnorePhysics( eLayer.Player, eLayer.Enemy );
		Utility.IgnorePhysics( eLayer.Player, eLayer.EnemyGate );

		m_owner.ShowMesh( false );
		m_owner.SetKinematicRigidBody();
		m_owner.checkRayCollision = false;

		if( m_owner.holdPositionRef <= 0 ) {
			mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
			if( mTarget ) {
				Vector3 inputDir = Vector3.zero;

				if( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
					inputDir = m_owner.Input.GetDirection();
				}

				if( inputDir == Vector3.zero ) {
					inputDir = -m_owner.transform.forward;
				}

				Vector3 dir = inputDir * mDistance;

				if( !mTarget.alwaysKinematic && !Physics.Raycast( m_owner.transform.position, inputDir, mDistance, 
																  ( 1 << (int)eLayer.Wall ) | ( 1 << (int)eLayer.EnvObject ) ) ) {
					Vector3 pos = m_owner.transform.position + dir;

					if( pos.y < m_owner.posOnGround.y ) {
						pos.y = m_owner.posOnGround.y;
					}

					m_owner.rigidBody.MovePosition( pos );
				}
			}
		}

		m_aniLength = 0.1f;
		m_checkTime = 0.0f;

		return true;
	}

	private bool ChangeAttackState( bool changeAni ) {
		m_owner.SetGroundedRigidBody();
		m_owner.checkRayCollision = true;
		m_owner.ShowMesh( true );

		/*
		if( mTarget ) {
			m_owner.LookAtTarget( mTarget.transform.position );
		}
		*/

		m_aniLength = m_owner.PlayAniImmediate( mCurAni );
		m_aniCutFrameLength = m_owner.aniEvent.GetCurCutFrameLength();
		m_checkTime = 0.0f;

		Utility.SetPhysicsLayerCollision( eLayer.Player, eLayer.Enemy );
		Utility.SetPhysicsLayerCollision( eLayer.Player, eLayer.EnemyGate );

		return true;
	}
}
