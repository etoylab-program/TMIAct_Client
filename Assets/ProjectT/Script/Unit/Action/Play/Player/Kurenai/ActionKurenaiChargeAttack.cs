
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionKurenaiChargeAttack : ActionChargeAttack {
	private eAnimation			mCurAni					= eAnimation.ChargeEnd;
	private bool				mIsSetProjectileAtkAttr	= true;
	private List<Projectile>	mOriginProjectileList	= null;
	private Projectile			mNewPjt					= null;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		cancelActionCommand = new eActionCommand[1];
		cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

		mCurAni = eAnimation.ChargeEnd;

		if ( mValue3 > 0.0f ) {
			mCurAni = eAnimation.ChargeEnd2;
		}

		mOriginProjectileList = m_owner.aniEvent.GetAllProjectile( mCurAni );

		m_psStart = GameSupport.CreateParticle( "Effect/Character/prf_fx_kurenai_charge_step01.prefab", m_owner.transform );

		string projectileName = mValue3 > 0.0f ? "pjt_character_kurenai_charge_skill_weapon" : "pjt_character_kurenai_charge_weapon";
		mNewPjt = GameSupport.CreateProjectile( $"Projectile/{projectileName}.prefab" );
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		mDir = Vector3.zero;
		m_psStart.gameObject.SetActive( true );

		if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Training ) {
			ExecuteStartSkillBO();
		}
	}

	public override IEnumerator UpdateAction() {
		float chargingTime = 0.0f;

		while ( m_endUpdate == false ) {
			if ( m_chargeCount < World.Instance.UIPlay.btnAtk.m_maxChargeCount ) {
				chargingTime += m_owner.fixedDeltaTime;

				m_chargeCount = Mathf.Clamp( (int)( chargingTime / World.Instance.UIPlay.btnAtk.m_chargeTime ), 1, World.Instance.UIPlay.btnAtk.m_maxChargeCount );
				if ( m_chargeCount != m_beforeChargeCount ) {
					PlayEffCharge( m_chargeCount - 1 );
				}

				m_beforeChargeCount = m_chargeCount;
			}

			UpdateMove( m_owner.speed );
			yield return mWaitForFixedUpdate;
		}

		StopAllEffCharge();

		if ( m_chargeCount <= 0 ) {
			m_owner.actionSystem.CancelCurrentAction();
		}
		else {
			if ( FSaveData.Instance.AutoTargetingSkill ) {
				Unit target = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
				if ( target ) {
					m_owner.LookAtTarget( target.transform.position );
				}
			}
			else {
				m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
			}

			StartChargeAttack();
			ShowSkillNames( m_data );

			if ( SetAddAction ) {
				m_owner.aniEvent.ChangeAllProjectile( mCurAni, mNewPjt );

				List<Projectile> projectileList = m_owner.aniEvent.GetAllProjectile( mCurAni );
				for ( int i = 0; i < projectileList.Count; i++ ) {
					projectileList[i].SetAddAtkRatio( AddActionValue1 );
				}

				mIsSetProjectileAtkAttr = true;

				mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Buff;

				if ( World.Instance.ListPlayer.Count > 0 ) {
					for ( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
						mBuffEvt.Set( m_data.ID, eEventSubject.Self, eEventType.EVENT_BUFF_SPEED_UP, World.Instance.ListPlayer[i],
									  AddActionValue1, 0.0f, 0.0f, AddActionDuration, 0.0f, mBuffEvt.effId, mBuffEvt.effId2, eBuffIconType.Buff_Speed );

						EventMgr.Instance.SendEvent( mBuffEvt );
					}
				}
				else {
					mBuffEvt.Set( m_data.ID, eEventSubject.Self, eEventType.EVENT_BUFF_SPEED_UP, m_owner,
								  AddActionValue1, 0.0f, 0.0f, AddActionDuration, 0.0f, mBuffEvt.effId, mBuffEvt.effId2, eBuffIconType.Buff_Speed );

					EventMgr.Instance.SendEvent( mBuffEvt );
				}
			}

			if ( mIsSetProjectileAtkAttr ) {
				if ( mValue2 > 0.0f ) {
					List<Projectile> projectileList = m_owner.aniEvent.GetAllProjectile( mCurAni );
					for ( int i = 0; i < projectileList.Count; i++ ) {
						projectileList[i].SetProjectileAtkAttr( Projectile.EProjectileAtkAttr.UPPER );
					}
				}
				else if ( mValue1 > 0.0f ) {
					List<Projectile> projectileList = m_owner.aniEvent.GetAllProjectile( mCurAni );
					for ( int i = 0; i < projectileList.Count; i++ ) {
						projectileList[i].SetProjectileAtkAttr( Projectile.EProjectileAtkAttr.STUN );
					}
				}

				mIsSetProjectileAtkAttr = false;
			}

			m_endUpdate = false;
			m_checkTime = 0.0f;
			m_aniLength = m_owner.PlayAniImmediate( mCurAni );

			while ( !m_endUpdate ) {
				m_checkTime += m_owner.fixedDeltaTime;
				if ( m_checkTime >= m_aniLength ) {
					m_endUpdate = true;
				}

				yield return mWaitForFixedUpdate;
			}
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

	public override float GetAtkRange() {
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( mCurAni );
		if ( evt == null ) {
			return 0.0f;
		}

		return evt.visionRange;
	}

	public override void ResetAddAction() {
		base.ResetAddAction();

		if (mOriginProjectileList == null) {
			return;
		}

		m_owner.aniEvent.ChangeAllProjectile( mCurAni, mOriginProjectileList );

		mIsSetProjectileAtkAttr = true;
	}
}
