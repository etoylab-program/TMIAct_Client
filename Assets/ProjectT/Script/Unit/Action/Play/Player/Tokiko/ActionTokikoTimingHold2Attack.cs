
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionTokikoTimingHold2Attack : ActionSelectSkillBase {
	private Unit					mTarget				= null;
	private eAnimation				mCurAni				= eAnimation.EvadeTiming2;
	private List<AniEvent.sEvent>	mAllAtkEventList	= null;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );
		actionCommand = eActionCommand.TimingHoldAttack;

		extraCondition = new eActionCondition[2];
		extraCondition[0] = eActionCondition.Grounded;
		extraCondition[1] = eActionCondition.NoUsingUSkill;

		superArmor = Unit.eSuperArmor.Lv1;

		mAllAtkEventList = m_owner.aniEvent.GetAllAttackEvent( mCurAni );
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		ShowSkillNames( m_data );

		if( SetAddAction ) {
			for( int i = 0; i < mAllAtkEventList.Count; i++ ) {
				mAllAtkEventList[i].atkRatio = mAllAtkEventList[i].originalAtkRatio + ( mAllAtkEventList[i].originalAtkRatio * AddActionValue1 );
			}
		}

		m_aniLength = m_owner.PlayAniImmediate( mCurAni );

		if( FSaveData.Instance.AutoTargetingSkill ) {
			mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
			if( mTarget ) {
				m_owner.LookAtTarget( mTarget.transform.position );
			}
		}
		else {
			m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
		}
	}

	public override IEnumerator UpdateAction() {
		while( m_endUpdate == false ) {
			m_checkTime += m_owner.fixedDeltaTime;
			if( m_checkTime >= m_aniLength ) {
				m_endUpdate = true;
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override float GetAtkRange() {
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( mCurAni );
		if( evt == null ) {
			Debug.LogError( mCurAni.ToString() + "공격 이벤트가 없네??" );
			return 0.0f;
		}
		else if( evt.visionRange <= 0.0f ) {
			Debug.LogError( mCurAni.ToString() + "Vistion Range가 0이네??" );
		}

		return evt.visionRange;
	}
}
