
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionTokikoCharge2Attack : ActionChargeAttack {

	private int mRotationCount = 0;
	private eActionCommand[] mOriginalCancelCommand = null;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		cancelActionCommand = new eActionCommand[1];
		cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

		m_psStart = GameSupport.CreateParticle( "Effect/Character/prf_fx_tokiko_charge_step_start.prefab", m_owner.transform );
		m_listPSCharge.Add( GameSupport.CreateParticle( "Effect/Character/prf_fx_tokiko_charge_step_start.prefab", m_owner.transform ) );

		mOriginalCancelCommand = cancelActionCommand;
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		mTargetCollider = m_owner.GetMainTargetCollider( true );
		if ( mTargetCollider ) {
			m_owner.LookAtTarget( mTargetCollider.GetCenterPos() );
		}

		mRotationCount += (int)mValue1;
		m_psStart.gameObject.SetActive( true );
	}

	public override IEnumerator UpdateAction() {
		float chargingTime = 0.0f;

		while ( m_endUpdate == false ) {
			if ( m_chargeCount < World.Instance.UIPlay.btnAtk.m_maxChargeCount ) {
				chargingTime += m_owner.fixedDeltaTime;

				m_chargeCount = Mathf.Clamp( (int)( chargingTime / World.Instance.UIPlay.btnAtk.m_chargeTime ), 1, World.Instance.UIPlay.btnAtk.m_maxChargeCount );
				if ( m_chargeCount != m_beforeChargeCount )
					PlayEffCharge( m_chargeCount - 1 );

				m_beforeChargeCount = m_chargeCount;
			}

			UpdateMove( m_owner.speed );
			yield return mWaitForFixedUpdate;
		}

		StopAllEffCharge();

		if ( m_chargeCount > 0 ) {
			StartChargeAttack();
			ShowSkillNames( m_data );

			if ( mTargetCollider && FSaveData.Instance.AutoTargeting ) {
				m_owner.LookAtTarget( mTargetCollider.GetCenterPos() );
			}

			m_owner.rigidBody.velocity = Vector3.zero;

			m_checkTime = 0.0f;
			m_aniLength = m_owner.PlayAniImmediate( eAnimation.AttackCharge2 );

			Unit target = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
			if ( target ) {
				GoToTarget( target );
			}

			while ( mRotationCount > 0 ) {
				m_checkTime += m_owner.fixedDeltaTime;
				if ( m_checkTime >= m_aniLength ) {
					--mRotationCount;
					if ( mRotationCount <= 0 ) {
						break;
					}

					m_checkTime = 0.0f;
					m_owner.rigidBody.velocity = Vector3.zero;

					LookAtTarget( target );
					m_aniLength = m_owner.PlayAniImmediate( eAnimation.AttackCharge2 );
				}

				if ( !target || target.curHp <= 0.0f || !target.IsActivate() ) {
					target = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
					if ( target ) {
						GoToTarget( target );
					}
					else {
						mRotationCount = 0;
					}
				}
				else if ( target && target.curHp > 0.0f && Vector3.Distance( target.transform.position, m_owner.transform.position ) > 1.5f ) {
					GoToTarget( target );
				}

				yield return mWaitForFixedUpdate;
			}

			if ( SetAddAction ) {
				target = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
				if ( target ) {
					mOwnerPlayer.ExecuteBattleOption( BattleOption.eBOTimingType.OnStartAddAction, 0, null );

					cancelActionCommand = new eActionCommand[1];
					cancelActionCommand[0] = eActionCommand.Defence;

					float fChargePerSec = GameInfo.Instance.BattleConfig.USMaxSP / GameInfo.Instance.BattleConfig.USSPRegenSpeedTime;
					fChargePerSec += fChargePerSec * mOwnerPlayer.SPRegenIncRate;

					while ( m_owner.curSp > fChargePerSec ) {
						m_checkTime += m_owner.fixedDeltaTime;
						if ( m_checkTime >= m_aniLength ) {
							m_checkTime = 0.0f;
							m_owner.rigidBody.velocity = Vector3.zero;

							m_aniLength = m_owner.PlayAniImmediate( eAnimation.AttackCharge2 );

							if ( !target || target.curHp <= 0.0f || !target.IsActivate() ) {
								target = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
								if ( target ) {
									GoToTarget( target );
								}
								else {
									m_owner.cmptBuffDebuff.RemoveDebuff( eEventType.EVENT_DEBUFF_DECREASE_SP );
									break;
								}
							}
							else if ( Vector3.Distance( target.transform.position, m_owner.transform.position ) > 1.5f ) {
								GoToTarget( target );
							}
						}

						yield return mWaitForFixedUpdate;
					}
				}
			}
		}
		else {
			m_owner.actionSystem.CancelCurrentAction();
		}
	}

	public override void OnUpdating( IActionBaseParam param ) {
		if ( World.Instance.IsPause || mbStartAttack ) {
			if ( World.Instance.IsPause && !mbStartAttack ) {
				OnCancel();
			}

			return;
		}

		m_endUpdate = true;
	}

	public override void OnEnd() {
		base.OnEnd();

		mRotationCount = 0;
		cancelActionCommand = mOriginalCancelCommand;
	}

	public override void OnCancel() {
		if ( SetAddAction ) {
			m_owner.cmptBuffDebuff.RemoveDebuff( eEventType.EVENT_DEBUFF_DECREASE_SP );
		}

		base.OnCancel();

		mRotationCount = 0;
		cancelActionCommand = mOriginalCancelCommand;
	}

	public override float GetAtkRange() {
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( eAnimation.AttackCharge2 );
		if ( evt == null ) {
			Debug.LogError( "ChargingAttack 공격 이벤트가 없네??" );
			return 0.0f;
		}
		else if ( evt.visionRange <= 0.0f ) {
			Debug.LogError( "ChargingAttack Vistion Range가 0이네??" );
		}

		return evt.visionRange;
	}

	private void GoToTarget( Unit target ) {
		m_owner.SetMainTarget( target );

		Vector3 targetPos = m_owner.GetTargetCapsuleEdgePosForTeleport( target );
		targetPos.y = m_owner.transform.position.y;

		float dist = Vector3.Distance( m_owner.transform.position, targetPos );
		if ( Physics.Raycast( m_owner.transform.position, ( targetPos - m_owner.transform.position ).normalized, out RaycastHit hitInfo,
							dist, ( 1 << (int)eLayer.Wall ) ) ) {
			targetPos = hitInfo.point;
			targetPos.y = m_owner.transform.position.y;
		}

		m_owner.SetInitialPosition( targetPos, m_owner.transform.rotation );
		LookAtTarget( target );
	}
}
