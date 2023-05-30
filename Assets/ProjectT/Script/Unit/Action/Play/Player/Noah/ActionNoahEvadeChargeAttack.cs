
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionNoahEvadeChargeAttack : ActionSelectSkillBase {
	private int                     mMaxAttackCount = 2;
	private eAnimation              mCurAni         = eAnimation.EvadeCharge;
	private List<AniEvent.sEvent>   mListAtkEvt     = new List<AniEvent.sEvent>();
	private List<AniEvent.sEvent>   mListLastAtkEvt = new List<AniEvent.sEvent>();


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );
		actionCommand = eActionCommand.HoldingDefBtnAttack;

		extraCondition = new eActionCondition[4];
		extraCondition[0] = eActionCondition.Grounded;
		extraCondition[1] = eActionCondition.NoUsingSkill;
		extraCondition[2] = eActionCondition.NoUsingQTE;
		extraCondition[3] = eActionCondition.NoUsingUSkill;

		cancelActionCommand = new eActionCommand[1];
		cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

		superArmor = Unit.eSuperArmor.Lv1;

		mMaxAttackCount += (int)mValue1;
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		ShowSkillNames( m_data );
		LookAtTarget();

		if( mListAtkEvt.Count == 0 ) {
			List<AniEvent.sEvent> list = m_owner.aniEvent.GetAllAttackEvent( mCurAni );
			mListAtkEvt.AddRange( list );
		}

		if( mListLastAtkEvt.Count == 0 ) {
			List<AniEvent.sEvent> list = m_owner.aniEvent.GetAllAttackEvent( eAnimation.EvadeCharge_1 );
			mListAtkEvt.AddRange( list );
		}
	}

	public override IEnumerator UpdateAction() {
		int attackCount = 1;

		m_aniLength = m_owner.PlayAniImmediate( mCurAni );
		m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength( mCurAni );

		if( SetAddAction ) {
			List<AniEvent.sEvent> list = m_owner.aniEvent.GetAllAttackEvent( mCurAni );
			for( int i = 0; i < list.Count; i++ ) {
				list[i].atkRatio += ( mListAtkEvt[i].atkRatio * AddActionValue1 );
			}

			list = m_owner.aniEvent.GetAllAttackEvent( eAnimation.EvadeCharge_1 );
			for( int i = 0; i < list.Count; i++ ) {
				list[i].atkRatio += ( mListAtkEvt[i].atkRatio * AddActionValue1 );
			}
		}

		while( !m_endUpdate ) {
			m_checkTime += m_owner.fixedDeltaTime;
			if( m_checkTime >= m_aniCutFrameLength ) {
				if( attackCount >= mMaxAttackCount ) {
					m_endUpdate = true;
				}

				m_checkTime = 0;

				LookAtTarget();
				m_aniLength = m_owner.PlayAniImmediate( mCurAni );

				++attackCount;
			}

			yield return mWaitForFixedUpdate;
		}

		LookAtTarget();

		m_owner.PlayAniImmediate( eAnimation.EvadeCharge_1 );
		m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength( eAnimation.EvadeCharge_1 );

		yield return new WaitForSeconds( m_aniCutFrameLength );
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

	private void LookAtTarget() {
		if( FSaveData.Instance.AutoTargetingSkill ) {
			Unit target = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
			if( target ) {
				m_owner.LookAtTarget( target.transform.position );
			}
		}
		else {
			m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
		}
	}
}
