
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionMurasakiComboAttack : ActionComboAttack {

	public bool IsCharging { get; protected set; } = false;

	//== Const
	private const float FIRST_CHARGE_TIME		= 0.3f;
	private const float FIRST_CHARGE_ATK_RATIO	= 1.2f;
	private const float SECOND_CHARGE_TIME		= 0.3f;
	private const float	SECOND_CHARGE_ATK_RATIO	= 1.3f;
	//==
	private TimeSpan	mTsStart;
	private float		mAtkRatio				= 1.0f;
	private bool		mStartEffect1			= false;
	private bool		mStartEffect2			= false;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		attackAnimations = new eAnimation[3];
		attackAnimations[0] = eAnimation.Attack01;
		attackAnimations[1] = eAnimation.Attack02;
		attackAnimations[2] = eAnimation.Attack03;

		mOriginalAtkAnis = new eAnimation[attackAnimations.Length];
		for ( int i = 0; i < attackAnimations.Length; i++ ) {
			mOriginalAtkAnis[i] = attackAnimations[i];
		}

		superArmor = Unit.eSuperArmor.Lv1;
	}

	/*
	public override void OnStart( IActionBaseParam param ) {
		mAtkRatio = 1.0f;
		for ( int i = 0; i < attackAnimations.Length; i++ ) {
			List<AniEvent.sEvent> listAtkEvt = m_owner.aniEvent.GetAllAttackEvent( attackAnimations[i] );
			for ( int j = 0; j < listAtkEvt.Count; j++ ) {
				listAtkEvt[j].atkRatio = mAtkRatio;
			}
		}

		base.OnStart( param );
	}
	*/

	public override IEnumerator UpdateAction() {
		double deltaTime = 0;

		while ( !m_endUpdate ) {
			if ( m_currentAni != eAnimation.Attack02_Loop && m_currentAni != eAnimation.Attack03_Loop ) {
				m_checkTime += m_owner.fixedDeltaTime;

				if ( m_nextAttackIndex > CurrentAttackIndex ) {
					if ( m_checkTime >= m_owner.aniEvent.GetCutFrameLength( m_currentAni ) ) {
						m_endUpdate = true;
					}
				}
				else {
					if ( m_checkTime >= m_owner.aniEvent.GetAniLength( m_currentAni ) ) {
						m_endUpdate = true;
					}
				}
			}
			else {
				deltaTime = ( TimeSpan.FromTicks( DateTime.Now.Ticks ) - mTsStart ).TotalSeconds;

				if ( deltaTime >= FIRST_CHARGE_TIME && !mStartEffect1 ) {
					StopEff();

					EffectManager.Instance.Play( m_owner, 30007, EffectManager.eType.Common, 0.0f, true );
					EffectManager.Instance.Play( m_owner, 30008, EffectManager.eType.Common, 0.0f, true );

					mStartEffect1 = true;
					deltaTime = 0;
				}
				else if ( deltaTime >= SECOND_CHARGE_TIME && mStartEffect1 && !mStartEffect2 ) {
					StopEff();

					EffectManager.Instance.Play( m_owner, 30009, EffectManager.eType.Common, 0.0f, true );
					EffectManager.Instance.Play( m_owner, 30010, EffectManager.eType.Common, 0.0f, true );

					mStartEffect2 = true;
				}
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override void OnCancel() {
		base.OnCancel();

		IsCharging = false;
		m_reserved = false;

		StopEff();
		ManualExecuteBattleOption( 0.0f );
	}

	public override void OnEnd() {
		base.OnEnd();

		StopEff();
		ManualExecuteBattleOption( 0.0f );
	}

	public void StartCharging() {
		IsCharging = true;
		m_reserved = true;

		m_nextAttackIndex = CurrentAttackIndex + 1;

		mTsStart = TimeSpan.FromTicks( DateTime.Now.Ticks );
		mStartEffect1 = false;
		mStartEffect2 = false;
	}

	public void EndCharging() {
		IsCharging = false;
		m_reserved = false;

		if ( World.Instance.IsPause ) {
			m_endUpdate = true;
			StopEff();

			return;
		}

		double deltaTime = ( TimeSpan.FromTicks( DateTime.Now.Ticks ) - mTsStart ).TotalSeconds;
		if ( deltaTime < FIRST_CHARGE_TIME ) {
			if ( m_currentAni == eAnimation.Attack02_Loop || m_currentAni == eAnimation.Attack03_Loop ) {
				m_nextAttackIndex = CurrentAttackIndex;
				CurrentAttackIndex = Mathf.Max( 0, m_nextAttackIndex - 1 );

				m_endUpdate = true;
			}
			else {
				m_nextAttackIndex = CurrentAttackIndex + 1;
			}

			Debug.Log( "무라사키 기본 모으기 공격 캔슬!!!!!!!!!!!!!!!!!!!!!!!!!" );
			return;
		}

		if ( mStartEffect1 && !mStartEffect2 ) {
			mAtkRatio = FIRST_CHARGE_ATK_RATIO;
			ManualExecuteBattleOption( mValue1 / (float)eCOUNT.MAX_BO_FUNC_VALUE );
		}
		else if ( mStartEffect1 && mStartEffect2 ) {
			mAtkRatio = SECOND_CHARGE_ATK_RATIO;
			ManualExecuteBattleOption( mValue2 / (float)eCOUNT.MAX_BO_FUNC_VALUE );
		}
		else {
			mAtkRatio = 1.0f;
			ManualExecuteBattleOption( 0.0f );
		}

		for ( int i = 0; i < attackAnimations.Length; i++ ) {
			List<AniEvent.sEvent> listAtkEvt = m_owner.aniEvent.GetAllAttackEvent( attackAnimations[i] );
			for ( int j = 0; j < listAtkEvt.Count; j++ ) {
				listAtkEvt[j].atkRatio = listAtkEvt[j].originalAtkRatio * mAtkRatio;
			}
		}

		StopEff();
		StartAttack();
		LookAtTarget();
	}

	public override float GetAtkRange() {
		eAnimation atkAni = m_currentAni;
		if ( atkAni == eAnimation.None || atkAni == eAnimation.Attack02_Loop || atkAni == eAnimation.Attack03_Loop ) {
			atkAni = attackAnimations[0];
		}

		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( atkAni );
		if ( evt == null ) {
			Debug.LogError( atkAni.ToString() + "공격 이벤트가 없네??" );
			return 0.0f;
		}
		else if ( evt.visionRange <= 0.0f ) {
			Debug.LogError( atkAni.ToString() + "Vistion Range가 0이네??" );
		}

		return evt.visionRange;
	}

	public override void RestoreAttackAnimations() {
		attackAnimations = new eAnimation[mOriginalAtkAnis.Length];
		for ( int i = 0; i < mOriginalAtkAnis.Length; i++ ) {
			attackAnimations[i] = mOriginalAtkAnis[i];
		}
	}

	protected override void StartAttack() {
		if ( IsCharging ) {
			m_currentAni = GetCurAni();

			if ( m_currentAni == eAnimation.Attack02 ) {
				m_currentAni = eAnimation.Attack02_Loop;
			}
			else if ( m_currentAni == eAnimation.Attack03 ) {
				m_currentAni = eAnimation.Attack03_Loop;
			}

			m_owner.PlayAni( m_currentAni );
		}
		else {
			base.StartAttack();
		}
	}

	private void ManualExecuteBattleOption( float value ) {
		BOCharSkill.ChangeBattleOptionValue1( m_data.CharAddBOSetID2, TableId, value );
		mOwnerPlayer.ExecuteBattleOption( BattleOption.eBOTimingType.ManualCall, TableId, null );
	}

	private void StopEff() {
		EffectManager.Instance.StopEffImmediate( 30007, EffectManager.eType.Common, null );
		EffectManager.Instance.StopEffImmediate( 30008, EffectManager.eType.Common, null );
		EffectManager.Instance.StopEffImmediate( 30009, EffectManager.eType.Common, null );
		EffectManager.Instance.StopEffImmediate( 30010, EffectManager.eType.Common, null );
	}
}
