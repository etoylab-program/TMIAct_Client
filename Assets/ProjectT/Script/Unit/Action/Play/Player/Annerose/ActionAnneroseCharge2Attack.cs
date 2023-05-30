
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionAnneroseCharge2Attack : ActionChargeAttack {
	private eAnimation	mCurAni		= eAnimation.AttackCharge2;
	private Projectile	mPjt		= null;
	private float		mDuration	= 4.0f;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		cancelActionCommand = new eActionCommand[1];
		cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

		if( mValue1 > 0.0f ) {
			mCurAni = eAnimation.AttackCharge2_1;
			mDuration = 6.0f;
		}

		List<Projectile> list = m_owner.aniEvent.GetAllProjectile( mCurAni );
		mPjt = list[0];
		mPjt.OnHideFunc = OnProjectileHide;

		m_psStart = GameSupport.CreateParticle( "Effect/Character/prf_fx_annerose_charge_step01.prefab", m_owner.transform );
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		mDir = Vector3.zero;
		m_psStart.gameObject.SetActive( true );
	}

	public override IEnumerator UpdateAction() {
		float chargingTime = 0.0f;

		while( m_endUpdate == false ) {
			if( m_chargeCount < World.Instance.UIPlay.btnAtk.m_maxChargeCount ) {
				chargingTime += m_owner.fixedDeltaTime;

				m_chargeCount = Mathf.Clamp( (int)( chargingTime / World.Instance.UIPlay.btnAtk.m_chargeTime ), 1, World.Instance.UIPlay.btnAtk.m_maxChargeCount );
				if( m_chargeCount != m_beforeChargeCount ) {
					PlayEffCharge( m_chargeCount - 1 );
				}

				m_beforeChargeCount = m_chargeCount;
			}

			UpdateMove( m_owner.speed );
			yield return mWaitForFixedUpdate;
		}

		StopAllEffCharge();

		if( m_chargeCount <= 0 ) {
			m_owner.actionSystem.CancelCurrentAction();
		}
		else {
			if( FSaveData.Instance.AutoTargetingSkill ) {
				Unit target = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
				if( target ) {
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

			while( !m_endUpdate ) {
				m_checkTime += m_owner.fixedDeltaTime;
				if( m_checkTime >= m_aniLength ) {
					m_endUpdate = true;
				}

				yield return mWaitForFixedUpdate;
			}
		}
	}

	public override void OnUpdating( IActionBaseParam param ) {
		if( World.Instance.IsPause || mbStartAttack ) {
			if( World.Instance.IsPause && !mbStartAttack ) {
				OnCancel();
			}

			return;
		}

		m_endUpdate = true;
	}

	public override float GetAtkRange() {
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( mCurAni );
		if( evt == null ) {
			return 0.0f;
		}

		return evt.visionRange;
	}

	private void OnProjectileHide() {
		mListTarget = m_owner.GetEnemyList( true );

		for( int i = 0; i < mListTarget.Count; i++ ) {
			Unit enemy = mListTarget[i];
			if( enemy.CurrentSuperArmor >= Unit.eSuperArmor.Lv2 || enemy.IsImmuneFloat() || enemy.IsImmuneKnockback() ) {
				continue;
			}

			enemy.actionSystem.CancelCurrentAction();
			enemy.StopStepForward();
		}

		StopCoroutine( "UpdateBlackhole" );
		StartCoroutine( "UpdateBlackhole" );
	}

	private IEnumerator UpdateBlackhole() {
		float checkTime = 0.0f;

		Vector3 pos = mPjt.transform.position;
		pos.y = m_owner.posOnGround.y;

		while( m_owner.curHp > 0.0f && checkTime < mDuration ) {
			checkTime += Time.fixedDeltaTime;

			for( int i = 0; i < mListTarget.Count; i++ ) {
				Unit target = mListTarget[i];

				if( target == null || target.MainCollider == null || target.curHp <= 0.0f || target.cmptMovement == null ) {
					continue;
				}

				if( target.CurrentSuperArmor >= Unit.eSuperArmor.Lv2 || target.IsImmuneFloat() || target.IsImmuneKnockback() ) {
					continue;
				}

				target.StopStepForward();

				Vector3 v = (pos - target.transform.position).normalized;
				v.y = 0.0f;

				target.cmptMovement.UpdatePosition( v, Mathf.Max( GameInfo.Instance.BattleConfig.BlackholeMinSpeed, target.speed * 1.2f ), false );
			}

			yield return mWaitForFixedUpdate;
		}
	}
}
