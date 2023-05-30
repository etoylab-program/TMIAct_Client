using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionShisuiChargeAttack : ActionChargeAttack {
	private eAnimation mCurAni = eAnimation.AttackCharge;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		cancelActionCommand = new eActionCommand[1];
		cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

		m_psStart = GameSupport.CreateParticle( "Effect/Character/prf_fx_saika_charge_step01.prefab", m_owner.transform ); // Test - LeeSeungJin - June Renewal - After Change Code

		if ( mValue2 > 0.0f ) {
			mCurAni = eAnimation.AttackCharge_1;
		}
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		mDir = Vector3.zero;
		m_psStart.gameObject.SetActive( true );
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
			Unit target = null;
			if ( FSaveData.Instance.AutoTargetingSkill ) {
				target = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
				if ( target ) {
					m_owner.LookAtTarget( target.transform.position );
				}
			}
			else {
				m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
			}

			mOwnerPlayer.Guardian.PlayAction( actionCommand, target );

			StartChargeAttack();
			ShowSkillNames( m_data );

			m_endUpdate = false;
			m_checkTime = 0.0f;
			m_aniLength = m_owner.PlayAniImmediate( mCurAni );
			m_aniCutFrameLength = m_owner.aniEvent.GetCurCutFrameLength();

			while ( !m_endUpdate ) {
				m_checkTime += m_owner.fixedDeltaTime;
				if ( m_checkTime >= m_aniCutFrameLength ) {
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
}
