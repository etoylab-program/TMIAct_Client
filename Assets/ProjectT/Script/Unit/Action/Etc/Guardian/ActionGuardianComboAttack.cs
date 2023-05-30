using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionGuardianComboAttack : ActionGuardianBase {
	[Header( "[Set Animation]" )]
	[SerializeField] protected eAnimation[] _AttackAnimations = null;

	protected List<eBehaviour>	mBehaviourList			= new List<eBehaviour>();
	protected eAnimation[]		mOriginAttackAnimations	= null;
	protected eAnimation		mCurrentAnimation		= eAnimation.None;
	protected UnitCollider		mTargetCollider			= null;
	protected int				mNextAttackIndex		= 0;
	protected int				mCurrentAttackIndex		= 0;
	protected float				mAttackRange			= 0.0f;
	protected bool				mNoneTarget				= false;
	protected bool				mReserved				= false;


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

		if ( _AttackAnimations != null ) {
			mOriginAttackAnimations = new eAnimation[_AttackAnimations.Length];
			for ( int i = 0; i < _AttackAnimations.Length; i++ ) {
				mOriginAttackAnimations[i] = _AttackAnimations[i];
			}
		}
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		StartAttack();
		LookAtTarget();
	}

	public override IEnumerator UpdateAction() {
		float endAniTime;

		while ( m_endUpdate == false ) {
			m_checkTime += m_owner.fixedDeltaTime;

			if ( IsLastAttack() ) {
				endAniTime = m_owner.aniEvent.GetAniLength( mCurrentAnimation );
			}
			else {
				endAniTime = m_owner.aniEvent.GetCutFrameLength( mCurrentAnimation );
			}

			if ( ( m_owner.AI && !IsLastAttack() ) || mNextAttackIndex > mCurrentAttackIndex ) {
				if ( m_checkTime >= endAniTime ) {
					if ( m_owner.AI ) {
						bool isSkip = false;
						if ( m_owner.mainTarget ) {
							float dist = Utility.GetDistanceWithoutY( m_owner.transform.position, m_owner.mainTarget.transform.position );
							if ( dist >= GetAtkRange() * 1.5f ) {
								dist = Utility.GetDistanceWithoutY( m_owner.transform.position, m_owner.GetTargetCapsuleEdgePos( m_owner.mainTarget ) );
								if ( dist >= GetAtkRange() * 1.5f ) {
									isSkip = true;
								}
							}
						}

						if ( !isSkip ) {
							OnUpdating( null );
						}
					}

					m_endUpdate = true;
				}
			}
			else if ( m_checkTime >= endAniTime ) {
				m_endUpdate = true;
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override void OnUpdating( IActionBaseParam param ) {
		if ( mReserved ) {
			return;
		}

		mReserved = true;

		if ( IsLastAttack() == true ) {
			mNextAttackIndex = mCurrentAttackIndex;
		}
		else {
			mNextAttackIndex = mCurrentAttackIndex + 1;
		}
	}

	public override void OnEnd() {
		isPlaying = false;

		if ( mBehaviourList.Count > 0 && m_owner.lastAttackKnockBack && IsLastAttack() ) {
			List<AniEvent.sEvent> listAtkEvt = m_owner.aniEvent.GetAllAttackEvent( _AttackAnimations[mCurrentAttackIndex] );
			for ( int i = 0; i < listAtkEvt.Count; i++ ) {
				listAtkEvt[i].behaviour = mBehaviourList[i];
			}
		}

		if ( mNextAttackIndex == mCurrentAttackIndex ) {
			ResetComboAttackIndex();
			base.OnEnd();
		}
		else {
			mCurrentAttackIndex = mNextAttackIndex;
			m_nextAction = eActionCommand.Attack01;
		}

		mReserved = false;
	}

	public override void OnCancel() {
		isPlaying = false;
		isCancel = true;

		ResetComboAttackIndex();

		mReserved = false;
	}

	public override float GetAtkRange() {
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( _AttackAnimations[0] );
		if ( evt == null ) {
			return 0.0f;
		}

		if ( evt.visionRange == 0.0f && evt.behaviour == eBehaviour.Projectile ) {
			return 10.0f;
		}

		return evt.visionRange;
	}

	protected virtual void StartAttack() {
		mCurrentAnimation = GetCurAni();
		m_aniLength = m_owner.PlayAniImmediate( mCurrentAnimation );

		mAttackRange = m_owner.aniEvent.GetFirstAttackEventRange( mCurrentAnimation );
		m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength( mCurrentAnimation );
	}

	protected virtual eAnimation GetCurAni() {
		if ( m_owner.onlyLastAttack ) {
			return _AttackAnimations[_AttackAnimations.Length - 1];
		}
		if ( IsLastAttack() && m_owner.lastAttackKnockBack ) {
			mBehaviourList.Clear();

			List<AniEvent.sEvent> listAtkEvt = m_owner.aniEvent.GetAllAttackEvent( _AttackAnimations[mCurrentAttackIndex] );
			for ( int i = 0; i < listAtkEvt.Count; i++ ) {
				mBehaviourList.Add( listAtkEvt[i].behaviour );
				listAtkEvt[i].behaviour = eBehaviour.KnockBackAttack;
			}
		}

		return _AttackAnimations[mCurrentAttackIndex];
	}

	public virtual void LookAtTarget() {
		AniEvent.sEvent evt = m_owner.aniEvent.GetNextAttackEvent( mCurrentAnimation );
		if ( evt == null ) {
			return;
		}

		mNoneTarget = true;

		mTargetCollider = FSaveData.Instance.AutoTargeting ? m_owner.GetMainTargetCollider( false, evt.atkRange ) : null;
		if ( mTargetCollider ) {
			mNoneTarget = false;
			LookAtTarget( mTargetCollider.Owner );
		}
		else if ( m_owner.Input != null ) {
			m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
		}
	}

	public virtual bool IsLastAttack() {
		return mCurrentAttackIndex == ( _AttackAnimations.Length - 1 );
	}

	public void ResetComboAttackIndex() {
		mCurrentAttackIndex = 0;
		mNextAttackIndex = 0;
	}
}
