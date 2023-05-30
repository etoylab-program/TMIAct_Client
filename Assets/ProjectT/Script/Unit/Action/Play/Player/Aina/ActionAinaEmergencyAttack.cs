
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionAinaEmergencyAttack : ActionSelectSkillBase {
	private eAnimation mCurAni = eAnimation.EvadeCounter;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );
		actionCommand = eActionCommand.EmergencyAttack;

		conditionActionCommand = new eActionCommand[1];
		conditionActionCommand[0] = eActionCommand.Defence;

		if ( mValue2 > 0.0f ) {
			List<AniEvent.sEvent> attackEventList = m_owner.aniEvent.GetAllAttackEvent( mCurAni );
			for ( int i = 0; i < attackEventList.Count; i++ ) {
				attackEventList[i].behaviour = eBehaviour.GroggyAttack;
			}
		}
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		ShowSkillNames( m_data );

		m_aniLength = m_owner.PlayAniImmediate( mCurAni );

		if ( FSaveData.Instance.AutoTargetingSkill ) {
			Unit target = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
			if ( target ) {
				m_owner.LookAtTarget( target.transform.position );
			}
		}
		else {
			m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
		}
	}

	public override IEnumerator UpdateAction() {
		while ( !m_endUpdate ) {
			m_checkTime += m_owner.fixedDeltaTime;
			if ( m_checkTime >= m_aniLength ) {
				m_endUpdate = true;
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override float GetAtkRange() {
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( mCurAni );
		if ( evt == null ) {
			Debug.LogError( mCurAni + " 공격 이벤트가 없네??" );
			return 0.0f;
		}
		else if ( evt.visionRange <= 0.0f ) {
			Debug.LogError( mCurAni + " Vistion Range가 0이네??" );
		}

		return evt.visionRange;
	}
}
