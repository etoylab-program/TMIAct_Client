
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionNagiDashAttack : ActionSelectSkillBase {
	private Unit			mTarget             = null;
	private eAnimation		mCurAni             = eAnimation.DashAttack;
	private float			mDist               = 12.0f;
	private float			mLookAtTargetAngle  = 180.0f;
	private float			mTurnTime           = 0.2f;
	private Coroutine		mCr					= null;
	private WaitForSeconds	mWaitForSec			= null;


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

		m_aniLength = m_owner.aniEvent.GetAniLength( mCurAni );
		m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength( mCurAni );

		mWaitForSec = new WaitForSeconds( m_aniCutFrameLength );
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		ShowSkillNames( m_data );

		Utility.StopCoroutine( this, ref mCr );

		for( int i = 0; i < (int)mValue1; i++ ) {
			m_owner.ShowClone( i, transform.position, transform.rotation, true );
		}

		m_owner.PlayAniImmediate( mCurAni, 0.0f, false, true );

		if( FSaveData.Instance.AutoTargetingSkill ) {
			mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, mDist, mLookAtTargetAngle );
			if( mTarget ) {
				m_owner.LookAtTarget( mTarget.transform.position );
			}
		}
		else {
			m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
		}
	}

	public override IEnumerator UpdateAction() {
		float checkmTurnTime = 0.0f;

		while( m_endUpdate == false ) {
			checkmTurnTime += m_owner.fixedDeltaTime;

			if( checkmTurnTime >= mTurnTime ) {
				if( FSaveData.Instance.AutoTargetingSkill ) {
					mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, mDist, mLookAtTargetAngle );
					if( mTarget ) {
						m_owner.LookAtTarget( mTarget.transform.position );
					}
				}
				else {
					m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
				}

				checkmTurnTime = 0.2f;
			}

			m_checkTime += m_owner.fixedDeltaTime;
			if( m_checkTime >= m_aniCutFrameLength ) {
				m_endUpdate = true;
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override void OnEnd() {
		base.OnEnd();
		mCr = StartCoroutine( HideClone() );
	}

	public override void OnCancel() {
		base.OnCancel();

		m_owner.StopStepForward();
		mCr = StartCoroutine( HideClone() );
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

	private IEnumerator HideClone() {
		yield return mWaitForSec;

		for( int i = 0; i < (int)mValue1; i++ ) {
			m_owner.HideClone( i );
		}
	}
}
