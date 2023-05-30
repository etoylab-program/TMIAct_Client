using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionAinaDashAttack : ActionSelectSkillBase {
	private Unit		mTarget				= null;
	private eAnimation	mCurAni				= eAnimation.DashAttack;
	private float		mDist				= 12.0f;
	private float		mLookAtTargetAngle	= 180.0f;
	private bool		mManualRot			= false;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );
		actionCommand = eActionCommand.RushAttack;

		conditionActionCommand = new eActionCommand[1];
		conditionActionCommand[0] = eActionCommand.Defence;

		extraCondition = new eActionCondition[1];
		extraCondition[0] = eActionCondition.Grounded;

		cancelActionCommand = new eActionCommand[1];
		cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

		extraCancelCondition = new eActionCondition[3];
		extraCancelCondition[0] = eActionCondition.UseSkill;
		extraCancelCondition[1] = eActionCondition.UseQTE;
		extraCancelCondition[2] = eActionCondition.UseUSkill;

		superArmor = Unit.eSuperArmor.Lv1;
		mManualRot = true;

		if ( mValue1 > 0.0f ) {
			mCurAni = eAnimation.DashAttack_1;
		}

		if ( mValue2 > 0.0f ) {
			mCurAni = eAnimation.DashAttack_2;
			mManualRot = false;
		}

		if ( mValue3 > 0.0f ) {
			mCurAni = eAnimation.DashAttack_3;
		}
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		ShowSkillNames( m_data );

		m_aniLength = m_owner.PlayAniImmediate( mCurAni );

		if ( FSaveData.Instance.AutoTargetingSkill ) {
			mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, mDist, mLookAtTargetAngle );
			if ( mTarget ) {
				m_owner.LookAtTarget( mTarget.transform.position );
			}
		}
		else {
			m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
		}
	}

	public override IEnumerator UpdateAction() {
		//float checkmTurnTime = 0.0f;

		while ( m_endUpdate == false ) {
			/*
			checkmTurnTime += m_owner.fixedDeltaTime;
			if ( checkmTurnTime >= mTurnTime ) {
				if ( FSaveData.Instance.AutoTargetingSkill ) {
					mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, mDist, mLookAtTargetAngle );
					if ( mTarget ) {
						m_owner.LookAtTarget( mTarget.transform.position );
					}
				}
				else {
					m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
				}

				checkmTurnTime = 0.2f;
			}
			*/

			m_checkTime += m_owner.fixedDeltaTime;
			if ( m_checkTime >= m_aniLength ) {
				m_endUpdate = true;
			}

			if ( mManualRot ) {
				UpdateMove( m_owner.speed );
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override void OnCancel() {
		base.OnCancel();
		m_owner.StopStepForward();
	}

	public override float GetAtkRange() {
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( mCurAni );
		if ( evt == null ) {
			Debug.LogError( mCurAni.ToString() + "공격 이벤트가 없네??" );
			return 0.0f;
		}
		else if ( evt.visionRange <= 0.0f ) {
			Debug.LogError( mCurAni.ToString() + "Vistion Range가 0이네??" );
		}

		return evt.visionRange;
	}

	protected override void UpdateMove( float speed ) {
		Vector3 dir = m_owner.Input.GetRawDirection();
		bool isAI = false;

		if ( m_param != null ) {
			mParamAI = m_param as ActionParamAI;

			if ( mParamAI != null ) {
				dir = Utility.GetDirectionVector( m_owner, mParamAI.Direction );
				isAI = true;
			}
		}

		if ( dir != Vector3.zero ) {
			if ( !isAI ) {
				Vector3 cameraRight = World.Instance.InGameCamera.transform.right;
				Vector3 cameraForward = World.Instance.InGameCamera.transform.forward;

				mDir = Vector3.zero;
				mDir.x = ( dir.x * cameraRight.x ) + ( dir.y * cameraForward.x );
				mDir.z = ( dir.x * cameraRight.z ) + ( dir.y * cameraForward.z );
				mDir.y = 0.0f;
			}
			else {
				mDir = dir;
			}

			m_owner.cmptRotate.UpdateRotation( mDir, true );
		}
	}
}
