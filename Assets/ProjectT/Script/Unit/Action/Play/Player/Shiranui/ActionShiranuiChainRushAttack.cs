
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionShiranuiChainRushAttack : ActionSelectSkillBase {
	private enum eState {
		START,
		REPEAT,
		FINISH,
	}

	private float mDist = 12.0f;
	private float mLookAtTargetAngle = 180.0f;
	private Unit mTarget = null;
	private int mExtraHitNum = 0;
	private int mCurHitCount = 0;
	private State mState = new State();
	private eAnimation mRepeatAnim = eAnimation.RushAttackRepeat;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		actionCommand = eActionCommand.RushAttack;
		IsRepeatSkill = true;

		conditionActionCommand = new eActionCommand[1];
		conditionActionCommand[0] = eActionCommand.Defence;

		extraCondition = new eActionCondition[1];
		extraCondition[0] = eActionCondition.Grounded;

		extraCancelCondition = new eActionCondition[3];
		extraCancelCondition[0] = eActionCondition.UseSkill;
		extraCancelCondition[1] = eActionCondition.UseQTE;
		extraCancelCondition[2] = eActionCondition.UseUSkill;

		superArmor = Unit.eSuperArmor.Lv1;

		mExtraHitNum = (int)mValue1;

		if ( mValue2 > 0.0f ) {
			mRepeatAnim = eAnimation.RushAttack2;
		}

		mState.Init( 3 );
		mState.Bind( eState.START, StartCallback );
		mState.Bind( eState.REPEAT, RepeatCallback );
		mState.Bind( eState.FINISH, FinishCallback );
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		ShowSkillNames( m_data );

		mCurHitCount = 1;

		mState.ChangeState( eState.START, true );
	}

	public override IEnumerator UpdateAction() {
		while ( m_endUpdate == false ) {
			m_checkTime += m_owner.fixedDeltaTime;

			switch ( mState.current ) {
				case eState.START: {
					if ( m_checkTime >= m_aniCutFrameLength ) {
						mState.ChangeState( eState.REPEAT, true );
					}
				}
				break;

				case eState.REPEAT: {
					if ( m_checkTime >= m_aniCutFrameLength ) {
						++mCurHitCount;
						if ( mCurHitCount > mExtraHitNum ) {
							mState.ChangeState( eState.FINISH, true );
						}
						else {
							mState.ChangeState( eState.REPEAT, true );
						}
					}
				}
				break;

				case eState.FINISH: {
					if ( m_checkTime >= m_aniLength ) {
						m_endUpdate = true;
					}
				}
				break;

				default: {

				}
				break;
			}

			if ( mTarget && mTarget.curHp <= 0.0f ) {
				mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
				if ( mTarget ) {
					m_owner.LookAtTarget( mTarget.transform.position );
				}
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override float GetAtkRange() {
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( eAnimation.RushAttack );
		if ( evt == null ) {
			Debug.LogError( "RushAttack 공격 이벤트가 없네??" );
			return 0.0f;
		}
		else if ( evt.visionRange <= 0.0f ) {
			Debug.LogError( "RushAttack Vistion Range가 0이네??" );
		}

		return evt.visionRange;
	}

	private bool StartCallback( bool ani ) {
		m_aniLength = m_owner.PlayAniImmediate( eAnimation.RushAttack );
		m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength( eAnimation.RushAttack );
		m_checkTime = 0.0f;

		if ( FSaveData.Instance.AutoTargetingSkill ) {
			mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, mDist, mLookAtTargetAngle );
			if ( mTarget ) {
				m_owner.LookAtTarget( mTarget.transform.position );
			}
		}

		return true;
	}

	private bool RepeatCallback( bool ani ) {
		m_aniLength = m_owner.PlayAniImmediate( mRepeatAnim );
		m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength( mRepeatAnim );
		m_checkTime = 0.0f;

		if ( FSaveData.Instance.AutoTargetingSkill ) {
			mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, mDist, mLookAtTargetAngle );
			if ( mTarget ) {
				m_owner.LookAtTarget( mTarget.transform.position );
			}
		}
		else {
			m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
		}

		return true;
	}

	private bool FinishCallback( bool ani ) {
		m_aniLength = m_owner.PlayAniImmediate( eAnimation.RushAttackRepeatFinish );
		m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength( eAnimation.RushAttackRepeatFinish );
		m_checkTime = 0.0f;

		if ( FSaveData.Instance.AutoTargetingSkill ) {
			mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, mDist, mLookAtTargetAngle );
			if ( mTarget ) {
				m_owner.LookAtTarget( mTarget.transform.position );
			}
		}

		return true;
	}
}
