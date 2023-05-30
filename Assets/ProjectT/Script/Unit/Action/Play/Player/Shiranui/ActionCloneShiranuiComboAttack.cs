using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCloneShiranuiComboAttack : ActionComboAttack {
	public bool		AttackAddAction	{ private get; set; } = false;
	public float	AttackRatio		{ private get; set; } = 0.0f;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		attackAnimations = new eAnimation[5];
		attackAnimations[0] = eAnimation.Attack01;
		attackAnimations[1] = eAnimation.Attack02;
		attackAnimations[2] = eAnimation.Attack03;
		attackAnimations[3] = eAnimation.Attack04_1;
		attackAnimations[4] = eAnimation.Attack05_1;

		mOriginalAtkAnis = new eAnimation[attackAnimations.Length];
		for ( int i = 0; i < attackAnimations.Length; i++ ) {
			mOriginalAtkAnis[i] = attackAnimations[i];
		}

		for ( int i = 0; i < attackAnimations.Length; i++ ) {
			List<AniEvent.sEvent> listAtkEvt = m_owner.aniEvent.GetAllAttackEvent( attackAnimations[i] );
			for ( int j = 0; j < listAtkEvt.Count; j++ ) {
				listAtkEvt[j].atkRatio += listAtkEvt[j].atkRatio * ( AttackRatio / (float)eCOUNT.MAX_BO_FUNC_VALUE );
			}
		}
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		attackAnimations[4] = mOriginalAtkAnis[4];

		if ( AttackAddAction ) {
			int rand = Random.Range( 0, (int)eCOUNT.MAX_BO_FUNC_VALUE ) + 1;
			if ( rand <= ( AddActionValue2 * (float)eCOUNT.MAX_BO_FUNC_VALUE ) ) {
				attackAnimations[4] = eAnimation.Attack05_2;
			}

			List<AniEvent.sEvent> listAtkEvt = m_owner.aniEvent.GetAllAttackEvent( attackAnimations[attackAnimations.Length - 1] );
			for ( int i = 0; i < listAtkEvt.Count; i++ ) {
				listAtkEvt[i].atkRatio += listAtkEvt[i].atkRatio * ( AttackRatio / (float)eCOUNT.MAX_BO_FUNC_VALUE );
				listAtkEvt[i].atkRatio += listAtkEvt[i].atkRatio * AddActionValue1;
			}
		}
		else {
			List<AniEvent.sEvent> listAtkEvt = m_owner.aniEvent.GetAllAttackEvent( attackAnimations[attackAnimations.Length - 1] );
			for ( int i = 0; i < listAtkEvt.Count; i++ ) {
				listAtkEvt[i].atkRatio += listAtkEvt[i].atkRatio * ( AttackRatio / (float)eCOUNT.MAX_BO_FUNC_VALUE );
			}
		}
	}
}
