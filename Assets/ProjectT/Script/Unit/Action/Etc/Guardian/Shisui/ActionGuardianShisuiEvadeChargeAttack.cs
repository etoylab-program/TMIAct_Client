using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionGuardianShisuiEvadeChargeAttack : ActionGuardianBase {
	private eAnimation	mCurAni			= eAnimation.EvadeCharge;
	private Vector2		mTeleportRatio	= Vector2.zero;


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

		mTeleportRatio = new Vector2( 2.0f, 0.0f );
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		if ( mValue2 > 0.0f ) {
			mCurAni = eAnimation.EvadeCharge_1;
		}

		ShowSkillNames( m_data );
	}

	public override IEnumerator UpdateAction() {
		if ( mPlayerGuardian != null && mPlayerGuardian.IsCommandAction ) {
			yield return mPlayerGuardian.TeleportGuardian( PlayerGuardian.eArrowType.BACK, mTeleportRatio, false );
		}

		m_aniLength = m_owner.PlayAniImmediate( mCurAni );

		while ( !m_endUpdate ) {
			m_checkTime += m_owner.fixedDeltaTime;
			if ( m_checkTime >= m_aniLength ) {
				m_endUpdate = true;
			}

			m_owner.cmptRotate.UpdateRotation( GetTargetDir(), true );

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
