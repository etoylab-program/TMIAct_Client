
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionTargetingAttack : ActionSelectSkillBase {
	[Header("[Set Animation]")]
	public eAnimation[] AttackAnimations;

	protected InGameCamera  mCamera             = null;
	protected UnitCollider  mTargetCollider     = null;
	protected int           mCurrentAniIndex    = 0;
	protected float			mDirCheckTime		= 0.0f;

	private bool mSetPlayableCamera = false;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );
		actionCommand = eActionCommand.Attack01;

		conditionActionCommand = new eActionCommand[1];
		conditionActionCommand[0] = eActionCommand.MoveByDirection;

		extraCondition = new eActionCondition[1];
		extraCondition[0] = eActionCondition.Grounded;

		cancelActionCommand = new eActionCommand[2];
		cancelActionCommand[0] = eActionCommand.Defence;
		cancelActionCommand[1] = eActionCommand.Jump;

		extraCancelCondition = new eActionCondition[3];
		extraCancelCondition[0] = eActionCondition.UseSkill;
		extraCancelCondition[1] = eActionCondition.UseQTE;
		extraCancelCondition[2] = eActionCondition.UseUSkill;
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		mTargetCollider = m_owner.GetMainTargetCollider( true );
		if( mTargetCollider ) {
			m_owner.LookAtTarget( mTargetCollider.Owner.transform.position );
		}

		if( mCamera == null ) {
			mCamera = World.Instance.InGameCamera;
		}

		mSetPlayableCamera = false;

		if( !mOwnerPlayer.IsHelper ) {
			SetPlayableCamera();
		}

		m_aniLength = m_owner.PlayAniImmediate( AttackAnimations[mCurrentAniIndex] );
		mCurrentAniIndex = 0;
		mDirCheckTime = 0.0f;
	}

	public override IEnumerator UpdateAction() {
		float time = 0.0f;
		bool bChangeSmoothTime = false;

		while( !m_endUpdate ) {
			m_checkTime += m_owner.fixedDeltaTime;
			if( m_checkTime >= m_aniLength ) {
				if( mCurrentAniIndex < ( AttackAnimations.Length - 1 ) ) {
					++mCurrentAniIndex;
					m_aniLength = m_owner.PlayAniImmediate( AttackAnimations[mCurrentAniIndex] );
				}

				m_checkTime = 0.0f;
			}

			if( World.Instance.StageType == eSTAGETYPE.STAGE_PVP || m_owner.AI ) {
				mTargetCollider = m_owner.GetMainTargetCollider( true );
				if( mTargetCollider ) {
					m_owner.LookAtTarget( mTargetCollider.Owner.transform.position );
				}
			}

			UpdateMoveOnAttack( m_owner.speed );

			if( !mOwnerPlayer.IsHelper ) {
				if( !mSetPlayableCamera ) {
					SetPlayableCamera();
				}

				if( mCamera.Mode == InGameCamera.EMode.FOLLOW_PLAYER && !bChangeSmoothTime ) {
					time += Time.fixedDeltaTime;
					if( time >= 0.5f ) {
						mCamera.FollowPlayerSetting.SmoothTime = 0.1f;
						bChangeSmoothTime = true;
					}
				}
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override void OnCancel() {
		m_endUpdate = true;
		base.OnCancel();

		if( mSetPlayableCamera && mCamera.Mode == InGameCamera.EMode.FOLLOW_PLAYER ) {
			mCamera.SetDefaultMode();
		}
	}

	public override void OnEnd() {
		base.OnEnd();

		if( mSetPlayableCamera && mCamera.Mode == InGameCamera.EMode.FOLLOW_PLAYER ) {
			mCamera.SetDefaultMode();
		}
	}

	public override float GetAtkRange() {
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(AttackAnimations[0]);
		if( evt == null ) {
			return 0.0f;
		}

		return evt.visionRange;
	}

	private void SetPlayableCamera() {
		if( mCamera.Mode != InGameCamera.EMode.DEFAULT ) {
			return;
		}

		Vector3 distance = mCamera.DefaultSetting.Distance;
		Vector2 lookAt = mCamera.DefaultSetting.LookAt;
		distance.y *= 2.0f;
		lookAt.y *= 1.5f;

		mCamera.SetFollowPlayerMode( distance, lookAt, 0.5f );
		mSetPlayableCamera = true;
	}

	private void UpdateMoveOnAttack( float speed ) {
		mDirCheckTime += Time.fixedDeltaTime;

		if( m_owner.AI ) {
			if( m_owner.mainTarget == null ) {
				m_owner.Input.SendEvent( eEventType.EVENT_PLAYER_INPUT_ATK_TOUCH_END, false );
			}
			else if( mDirCheckTime >= 1.5f ) {
				float atkRange = GetAtkRange();
				float dist = Utility.GetDistanceWithoutY( m_owner.transform.position, m_owner.mainTarget.transform.position );

				if( dist > atkRange ) {
					mDir = Utility.GetDirectionVector( m_owner, eDirection.ToTarget );
				}
				else if( dist <= atkRange ) {
					mDir = Utility.GetDirectionVector( m_owner, eDirection.FromTarget );

					int rand = Random.Range( 0, 3 );
					if( rand == 1 ) {
						mDir = ( mDir + m_owner.transform.right ).normalized;
					}
					else if( rand == 2 ) {
						mDir = ( mDir - m_owner.transform.right ).normalized;
					}
				}

				if( Physics.Raycast( m_owner.transform.position, -m_owner.transform.forward, m_owner.MainCollider.radius, 
									 ( 1 << (int)eLayer.EnvObject ) | ( 1 << (int)eLayer.Wall ) ) ) {
					mDir = Utility.GetDirectionVector( m_owner, Random.Range( 0, 2 ) == 0 ? eDirection.RightForward : eDirection.LeftForward );
				}

				mDirCheckTime = 0.0f;
			}
		}
		else {
			if( m_owner.Input.GetRawDirection() == Vector3.zero ) {
				return;
			}

			Vector3 inputDir = m_owner.Input.GetRawDirection();

			Vector3 cameraRight = mCamera.transform.right;
			Vector3 cameraForward = mCamera.transform.forward;

			mDir = Vector3.zero;
			mDir.x = ( inputDir.x * cameraRight.x ) + ( inputDir.y * cameraForward.x );
			mDir.z = ( inputDir.x * cameraRight.z ) + ( inputDir.y * cameraForward.z );
			mDir.y = 0.0f;
		}

		m_owner.cmptMovement.UpdatePosition( mDir, speed, false );
	}
}
