using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionShisuiAttackHoldAttack : ActionSelectSkillBase {
	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		actionCommand = eActionCommand.AttackDuringAttack;

		extraCondition = new eActionCondition[4];
		extraCondition[0] = eActionCondition.Grounded;
		extraCondition[1] = eActionCondition.NoUsingSkill;
		extraCondition[2] = eActionCondition.NoUsingQTE;
		extraCondition[3] = eActionCondition.NoUsingUSkill;

		cancelActionCommand = new eActionCommand[1];
		cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

		superArmor = Unit.eSuperArmor.Lv1;
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		mOwnerPlayer.Guardian.PlayAction( actionCommand, m_owner.mainTarget );

		m_checkTime = 0.0f;
		m_aniLength = m_owner.PlayAniImmediate( eAnimation.AttackHold );

		ShowSkillNames( m_data );
	}

	public override IEnumerator UpdateAction() {
		while ( m_endUpdate == false ) {
			m_checkTime += Time.fixedDeltaTime;

			if ( m_checkTime >= m_aniLength ) {
				m_endUpdate = true;
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override float GetAtkRange() {
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( eAnimation.AttackHold );
		if ( evt == null ) {
			Debug.LogError( eAnimation.AttackHold.ToString() + "공격 이벤트가 없네??" );
			return 0.0f;
		}
		else if ( evt.visionRange <= 0.0f ) {
			Debug.LogError( eAnimation.AttackHold.ToString() + "Vistion Range가 0이네??" );
		}

		return evt.visionRange;
	}
}
