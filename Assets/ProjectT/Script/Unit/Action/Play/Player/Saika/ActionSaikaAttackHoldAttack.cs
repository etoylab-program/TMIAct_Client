using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSaikaAttackHoldAttack : ActionSelectSkillBase {
	private enum eStateType {
		ATTACK_01,
		ATTACK_02,
		ATTACK_03,
	}

	private eAnimation	mCurAni					= eAnimation.AttackHold;
	private Unit		mTarget					= null;
	private State		mState					= new State();
	private int			mSecondAttackCount		= 1;
	private float		mLastAniLength			= 0.0f;
	private float		mIncreaseLastAttackRate	= 0.0f;
	private int			mCurComboCount			= 0;
	private int			mComboCount				= 0;


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

		superArmor = Unit.eSuperArmor.Lv1;

		mState.Init( 3 );
		mState.Bind( eStateType.ATTACK_01, FirstAttack );
		mState.Bind( eStateType.ATTACK_02, SecondAttack );
		mState.Bind( eStateType.ATTACK_03, ThirdAttack );

		if ( mValue1 > 0.0f ) {
			mSecondAttackCount = (int)mValue1;
		}
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		ShowSkillNames( m_data );

		mIncreaseLastAttackRate = 0.0f;
		mCurComboCount = 0;
		mComboCount = 0;

		if ( SetAddAction ) {
			mIncreaseLastAttackRate = AddActionValue1;
			mCurComboCount = mOwnerPlayer.comboCount;
		}

		if ( FSaveData.Instance.AutoTargetingSkill ) {
			mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
			if ( mTarget ) {
				m_owner.LookAtTarget( mTarget.transform.position );
			}
		}
		else {
			m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
		}

		mState.ChangeState( eStateType.ATTACK_01, true );
	}

	public override IEnumerator UpdateAction() {
		int secondAttackCount = 1;
		while ( m_endUpdate == false ) {
			m_checkTime += m_owner.fixedDeltaTime;

			switch ( (eStateType)mState.current ) {
				case eStateType.ATTACK_01: {
					if ( m_checkTime >= m_aniLength ) {
						mState.ChangeState( eStateType.ATTACK_02, true );
					}
				}
				break;

				case eStateType.ATTACK_02: {
					if ( m_checkTime >= m_aniLength ) {
						++secondAttackCount;
						if ( mSecondAttackCount < secondAttackCount ) {
							mState.ChangeState( eStateType.ATTACK_03, true );
						}
						else {
							mState.ChangeState( eStateType.ATTACK_02, true );
						}
					}
				}
				break;

				case eStateType.ATTACK_03: {
					if ( m_checkTime >= mLastAniLength ) {
						m_endUpdate = true;
					}
				}
				break;

				default: {

				}
				break;
			}

			if ( FSaveData.Instance.AutoTargetingSkill ) {
				if ( mTarget && mTarget.curHp > 0.0f ) {
					m_owner.LookAtTarget( mTarget.transform.position );
				}
				else {
					mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
				}
			}
			else {
				m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
			}

			if ( Director.IsPlaying || World.Instance.IsEndGame || World.Instance.ProcessingEnd ) {
				OnCancel();
				yield break;
			}

			if ( mIncreaseLastAttackRate > 0.0f && ( eStateType)mState.current < eStateType.ATTACK_03) {
				if ( 0 < mOwnerPlayer.comboCount && mCurComboCount != mOwnerPlayer.comboCount ) {
					int max = Mathf.Max( mCurComboCount, mOwnerPlayer.comboCount );
					int min = Mathf.Min( mCurComboCount, mOwnerPlayer.comboCount );

					mComboCount += ( max - min );

					mCurComboCount = mOwnerPlayer.comboCount;
				}
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override void OnCancel() {
		base.OnCancel();
		
		m_owner.StopStepForward();
		if ( m_owner.afterImg != null ) {
			m_owner.afterImg.Stop();
		}
	}

	public override float GetAtkRange() {
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( eAnimation.AttackHold_1 );
		if ( evt == null ) {
			Debug.LogError( eAnimation.AttackHold_1.ToString() + "공격 이벤트가 없네??" );
			return 0.0f;
		}
		else if ( evt.visionRange <= 0.0f ) {
			Debug.LogError( eAnimation.AttackHold_1.ToString() + "Vistion Range가 0이네??" );
		}

		return evt.visionRange;
	}

	private bool FirstAttack( bool ani ) {
		mCurAni = eAnimation.AttackHold;
		m_checkTime = 0.0f;
		m_aniLength = m_owner.PlayAniImmediate( mCurAni );
		return true;
	}

	private bool SecondAttack( bool ani ) {
		mCurAni = eAnimation.AttackHold_1;
		m_checkTime = 0.0f;
		m_aniLength = m_owner.PlayAniImmediate( mCurAni );
		m_owner.afterImg.Play( m_owner.aniEvent.listSkinnedMesh.ToArray(), m_aniLength, 0.3f, m_aniLength, Color.white );
		return true;
	}

	private bool ThirdAttack( bool ani ) {
		if ( mValue2 > 0.0f || mIncreaseLastAttackRate > 0.0f ) {
			mCurAni = eAnimation.AttackHold_3;

			List<AniEvent.sEvent> lastAttackEventList = m_owner.aniEvent.GetAllAttackEvent( mCurAni );
			for ( int i = 0; i < lastAttackEventList.Count; i++ ) {
				lastAttackEventList[i].atkRatio += lastAttackEventList[i].atkRatio * ( mComboCount * mIncreaseLastAttackRate );
			}
		}
		else {
			mCurAni = eAnimation.AttackHold_2;
		}

		m_checkTime = 0.0f;
		m_aniLength = m_owner.PlayAniImmediate( mCurAni );
		m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength( mCurAni );

		mLastAniLength = mCurAni == eAnimation.AttackHold_3 ? m_aniLength : m_aniCutFrameLength;
		return true;
	}
}
