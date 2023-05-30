using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSoraExtreamEvade2 : ActionExtreamEvade {

	public override void OnStart( IActionBaseParam param ) {
		BOCharSkill.ChangeBattleOptionDuration( BattleOption.eBOTimingType.DuringSkill, TableId, mValue1 );

		base.OnStart( param );
		StartEvadingBuff();
	}

	private void StartEvadingBuff() {
		float duration = 6.0f;
		mOwnerPlayer.ExtreamEvading = true;

		StopCoroutine( "EndEvadingBuff" );
		StartCoroutine( "EndEvadingBuff", duration );
	}

	public override void OnEnd() {
		base.OnEnd();
		IsSkillEnd = false;
	}

	public bool OnProjectileHit( Unit hitTarget ) {
		if( mValue3 <= 0.0f ) {
			return false;
		}

		m_owner.AddHp( BattleOption.eToExecuteType.Unit, hitTarget.LastDamage * 0.3f, false );
		return true;
	}

	private IEnumerator EndEvadingBuff( float duration ) {
		yield return StartCoroutine( ContinueDash() );

		EffectManager.Instance.Play( m_owner, 30037, EffectManager.eType.Common, 0.0f, true );

		bool checkHit = false;
		
		AniEvent.sEvent evt = new AniEvent.sEvent();
		evt.behaviour = eBehaviour.Attack;
		evt.hitEffectId = 1;
		evt.hitDir = eHitDirection.None;
		evt.atkDirection = eAttackDirection.Skip;
		evt.atkRatio = 1.0f;
		evt.SkipCalcDamage = true;

		m_checkTime = 0.0f;
		//bool forOnlyDamage = false;

        while( m_owner.curHp > 0.0f && m_checkTime < duration ) {
			m_checkTime += Time.fixedDeltaTime;

			if( !checkHit && m_owner.attacker && m_owner.IsHit ) {
				evt.behaviour = eBehaviour.Attack;

				mAtkEvt.SetWithSingleTarget( eEventType.EVENT_BATTLE_ON_DIRECT_HIT, m_owner, BattleOption.eToExecuteType.Unit, evt,
											m_owner.LastDamage * mValue1, eAttackDirection.Skip, false, 0, EffectManager.eType.None,
											m_owner.attacker.MainCollider, 0.0f, true, false );

				EventMgr.Instance.SendEvent( mAtkEvt );
				checkHit = true;
			}
			else if( checkHit && !m_owner.IsHit ) {
				checkHit = false;
			}

			/*
			if( !checkHit && m_owner.attacker && ( m_owner.actionSystem.IsCurrentAction( eActionCommand.Hit ) || m_owner.IsOnlyDamage ) ) {
				evt.behaviour = eBehaviour.Attack;

				mAtkEvt.SetWithSingleTarget( eEventType.EVENT_BATTLE_ON_DIRECT_HIT, m_owner, BattleOption.eToExecuteType.Unit, evt,
											m_owner.LastDamage * mValue1, eAttackDirection.Skip, false, 0, EffectManager.eType.None,
											m_owner.attacker.MainCollider, 0.0f, true, false, m_owner.IsOnlyDamage );

				EventMgr.Instance.SendEvent( mAtkEvt );

				forOnlyDamage = m_owner.IsOnlyDamage;
				checkHit = true;
			}
			else if( checkHit && !forOnlyDamage && !m_owner.actionSystem.IsCurrentAction( eActionCommand.Hit ) ) {
				checkHit = false;
			}
			else if( checkHit && forOnlyDamage && !m_owner.IsOnlyDamage ) {
				checkHit = false;
				forOnlyDamage = false;
			}
			*/

			yield return mWaitForFixedUpdate;
        }

        mOwnerPlayer.ExtreamEvading = false;
		IsSkillEnd = true;

		EffectManager.Instance.StopEffImmediate( 30037, EffectManager.eType.Common, null );
	}
}
