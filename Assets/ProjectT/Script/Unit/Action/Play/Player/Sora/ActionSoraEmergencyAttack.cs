using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSoraEmergencyAttack : ActionSelectSkillBase {
	private List<Projectile>	mListPjt	= null;
	private int					mEffId		= 1029;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );
		actionCommand = eActionCommand.EmergencyAttack;

		conditionActionCommand = new eActionCommand[1];
		conditionActionCommand[0] = eActionCommand.Defence;

		if( mValue1 > 0.0f ) {
			mEffId = 1030;
		}
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		ShowSkillNames( m_data );

		m_aniLength = m_owner.PlayAniImmediate( eAnimation.EvadeCounter );
		m_aniCutFrameLength = m_owner.aniEvent.GetCurCutFrameLength();

		if( mListPjt == null ) {
			mListPjt = m_owner.aniEvent.GetAllProjectile( eAnimation.EvadeCounter );
			for( int i = 0; i < mListPjt.Count; i++ ) {
				mListPjt[i].OnHitFunc = OnProjectileHit;
			}
		}

		if( FSaveData.Instance.AutoTargetingSkill ) {
			Unit target = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
			if( target ) {
				m_owner.LookAtTarget( target.transform.position );
			}
		}
		else {
			m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
		}
	}

	public override IEnumerator UpdateAction() {
		while( !m_endUpdate ) {
			m_checkTime += m_owner.fixedDeltaTime;
			if( m_checkTime >= m_aniCutFrameLength ) {
				m_endUpdate = true;
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override float GetAtkRange() {
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( eAnimation.EvadeCounter );
		if( evt == null ) {
			Debug.LogError( "EvadeCounter 공격 이벤트가 없네??" );
			return 0.0f;
		}
		else if( evt.visionRange <= 0.0f ) {
			Debug.LogError( "EvadeCounter Vistion Range가 0이네??" );
		}

		return evt.visionRange;
	}

	private void TargetHoldPosition( Unit target ) {
		mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
		mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.Each_Monster_Normal_Hit;
		mBuffEvt.SelectTarget = target;
		mBuffEvt.Set( m_data.ID, eEventSubject.HitTargetList, eEventType.EVENT_DEBUFF_HOLD_POSITION, m_owner, 0.0f, 0.0f, 0.0f, mValue1, 0.0f, mEffId, 0, eBuffIconType.Debuff_Nomove );

		EventMgr.Instance.SendEvent( mBuffEvt );
	}

	private bool OnProjectileHit( Unit target ) {
		TargetHoldPosition( target );
		return true;
	}
}
