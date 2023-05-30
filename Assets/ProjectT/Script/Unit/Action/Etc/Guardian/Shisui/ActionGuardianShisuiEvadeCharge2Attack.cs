using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionGuardianShisuiEvadeCharge2Attack : ActionGuardianBase {
	private eAnimation		mCurAni	= eAnimation.EvadeCharge2;
	private Unit			mTarget = null;

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

		extraCancelCondition = new eActionCondition[3];
		extraCancelCondition[0] = eActionCondition.UseSkill;
		extraCancelCondition[1] = eActionCondition.UseQTE;
		extraCancelCondition[2] = eActionCondition.UseUSkill;
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		if ( mValue1 > 0.0f ) {
			mCurAni = eAnimation.EvadeCharge2_1;
		}

		if ( mValue2 > 0.0f ) {
			mCurAni = eAnimation.EvadeCharge2_2;
		}

		mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
		if ( mTarget ) {
			m_owner.LookAtTarget( mTarget.transform.position );
		}

		m_aniLength = m_owner.PlayAniImmediate( mCurAni );
		m_aniCutFrameLength = m_owner.aniEvent.GetCurCutFrameLength();

		ShowSkillNames( m_data );
	}

	public override IEnumerator UpdateAction() {
		while ( !m_endUpdate ) {
			m_checkTime += m_owner.fixedDeltaTime;
			if ( m_checkTime >= m_aniLength ) {
				m_endUpdate = true;
			}

			if ( mTarget && mTarget.curHp <= 0.0f ) {
				mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
				if ( mTarget ) {
					m_owner.LookAtTarget( mTarget.transform.position );
				}
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override float GetAtkRange() {
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( mCurAni );
		if ( evt == null ) {
			Debug.LogError( mCurAni + "공격 이벤트가 없네??" );
			return 0.0f;
		}
		else if ( evt.visionRange <= 0.0f ) {
			Debug.LogError( mCurAni + "Vistion Range가 0이네??" );
		}

		return evt.visionRange;
	}
}
