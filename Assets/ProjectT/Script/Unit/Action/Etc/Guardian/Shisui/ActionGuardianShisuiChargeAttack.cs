using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionGuardianShisuiChargeAttack : ActionGuardianChargeAttack {
	private eAnimation mCurAni = eAnimation.AttackCharge;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		cancelActionCommand = new eActionCommand[1];
		cancelActionCommand[0] = eActionCommand.TimingHoldAttack;
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		if ( mValue2 > 0.0f ) {
			mCurAni = eAnimation.AttackCharge_1;
		}

		if ( mValue1 > 0.0f ) {
			List<AniEvent.sEvent> attackEventList = m_owner.aniEvent.GetAllAttackEvent( mCurAni );
			for ( int i = 0; i < attackEventList.Count; i++ ) {
				attackEventList[i].behaviour = eBehaviour.UpperAttack;
			}
		}

		Unit target = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
		if ( target ) {
			m_owner.LookAtTarget( target.transform.position );
		}

		m_checkTime = 0.0f;
		m_aniLength = m_owner.PlayAniImmediate( mCurAni );

		ShowSkillNames( m_data );
	}

	public override IEnumerator UpdateAction() {
		m_endUpdate = false;

		while ( !m_endUpdate ) {
			m_checkTime += m_owner.fixedDeltaTime;
			if ( m_checkTime >= m_aniLength ) {
				m_endUpdate = true;
			}

			yield return mWaitForFixedUpdate;
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
