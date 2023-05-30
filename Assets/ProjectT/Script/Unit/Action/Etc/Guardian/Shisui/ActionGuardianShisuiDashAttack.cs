using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionGuardianShisuiDashAttack : ActionGuardianBase {
	private eAnimation	mCurAni			= eAnimation.DashAttack;
	private Vector2		mTeleportRatio	= Vector2.zero;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		actionCommand = eActionCommand.RushAttack;

		conditionActionCommand = new eActionCommand[1];
		conditionActionCommand[0] = eActionCommand.Defence;

		extraCondition = new eActionCondition[1];
		extraCondition[0] = eActionCondition.Grounded;

		cancelActionCommand = new eActionCommand[1];
		cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

		extraCancelCondition = new eActionCondition[3];
		extraCancelCondition[0] = eActionCondition.UseSkill;
		extraCancelCondition[1] = eActionCondition.UseQTE;
		extraCancelCondition[2] = eActionCondition.UseUSkill;

		mTeleportRatio = new Vector2( 2.0f, 0.0f );
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		ShowSkillNames( m_data );
	}

	public override IEnumerator UpdateAction() {
		if ( mPlayerGuardian != null && mPlayerGuardian.IsCommandAction ) {
			yield return mPlayerGuardian.TeleportGuardian( PlayerGuardian.eArrowType.BACK, mTeleportRatio, false );
		}

		m_owner.cmptRotate.UpdateRotation( GetTargetDir(), true );

		m_aniLength = m_owner.PlayAniImmediate( mCurAni );

		while ( m_endUpdate == false ) {
			m_checkTime += m_owner.fixedDeltaTime;
			if ( m_checkTime >= m_aniLength ) {
				m_endUpdate = true;
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override void OnCancel() {
		base.OnCancel();

		m_owner.StopStepForward();
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
}
