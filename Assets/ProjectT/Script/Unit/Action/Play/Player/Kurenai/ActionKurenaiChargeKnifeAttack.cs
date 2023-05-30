
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionKurenaiChargeKnifeAttack : ActionChargeAttack {

	private eAnimation	mCurAni			= eAnimation.AttackCharge2;
	private bool		mbBlackholeOnce	= false;
	private Coroutine	mCr				= null;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		cancelActionCommand = new eActionCommand[1];
		cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

		m_psStart = GameSupport.CreateParticle( "Effect/Character/prf_fx_kurenai_charge_step02.prefab", m_owner.transform );
		mCurAni = eAnimation.AttackCharge2;

		List<Projectile> list = m_owner.aniEvent.GetAllProjectile( mCurAni );
		for ( int i = 0; i < list.Count; i++ ) {
			list[i].OnHitFunc = OnProjectileHit;
		}
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		m_psStart.gameObject.SetActive( true );
		mbBlackholeOnce = false;
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
			Debug.LogError( mCurAni.ToString() + "공격 이벤트가 없네??" );
			return 0.0f;
		}
		else if ( evt.visionRange <= 0.0f ) {
			Debug.LogError( mCurAni.ToString() + "Vistion Range가 0이네??" );
		}

		return evt.visionRange;
	}

	private bool OnProjectileHit( Unit hitTarget ) {
		if ( mbBlackholeOnce || mValue1 < 1.0f ) {
			return false;
		}

		mbBlackholeOnce = true;

		Utility.StopCoroutine( this, ref mCr );
		mCr = StartCoroutine( UpdateBlackhole( hitTarget ) );

		return true;
	}

	private IEnumerator UpdateBlackhole( Unit hitTarget ) {
		Vector3 blackholePos = hitTarget.transform.position;

		mListTarget = m_owner.GetEnemyList( true );
		for ( int i = 0; i < mListTarget.Count; i++ ) {
			Unit target = mListTarget[i];
			if ( target.CurrentSuperArmor >= Unit.eSuperArmor.Lv2 || target.IsImmuneFloat() || target.IsImmuneKnockback() ) {
				continue;
			}

			target.actionSystem.CancelCurrentAction();
			target.StopStepForward();
		}

		float checkTime = 2.7f;

		while ( checkTime > 0.0f ) {
			checkTime -= Time.fixedDeltaTime;

			for ( int i = 0; i < mListTarget.Count; i++ ) {
				Unit target = mListTarget[i];
				if ( target.MainCollider == null ) {
					continue;
				}

				if ( target.CurrentSuperArmor >= Unit.eSuperArmor.Lv2 || target.IsImmuneFloat() || target.IsImmuneKnockback() ) {
					continue;
				}

				target.StopStepForward();

				Vector3 v = ( blackholePos - target.transform.position ).normalized;
				v.y = 0.0f;

				target.cmptMovement.UpdatePosition( v, Mathf.Max( GameInfo.Instance.BattleConfig.BlackholeMinSpeed, target.speed * 1.2f ), false );
			}

			yield return mWaitForFixedUpdate;
		}
	}
}
