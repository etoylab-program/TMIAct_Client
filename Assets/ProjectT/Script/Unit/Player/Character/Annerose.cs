
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;


public class Annerose : Player {
	protected override void OnEventDefence( IActionBaseParam param = null ) {
		if( m_actionSystem.IsCurrentUSkillAction() == true ) {
			return;
		}

		if( CheckTimingHoldAttack() ) {
			return;
		}

		ActionDash actionDash = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionDash;
		ActionSelectSkillBase actionSkill = m_actionSystem.GetAction<ActionSelectSkillBase>(eActionCommand.Teleport);

		if( actionSkill && actionSkill.PossibleToUse && actionDash && actionDash.IsPossibleToDashAttack() && GetMainTargetCollider( true ) ) {
			float checkTime = actionDash.GetEvadeCutFrameLength();
		
			if( World.Instance.UIPlay.btnDash.deltaTime < checkTime ) {
				CommandAction( eActionCommand.Teleport, null );
				return;
			}
		}

		CommandAction( eActionCommand.Defence, param );
	}

	protected override void OnEventSpecialAtk() {
		if( m_actionSystem.IsCurrentSkillAction() || m_actionSystem.IsCurrentUSkillAction() ) {
			return;
		}

		ActionBase action = m_actionSystem.GetAction(eActionCommand.AttackDuringAttack);
		if( action == null ) {
			return;
		}

		if( m_actionSystem.IsCurrentAction( eActionCommand.Attack01 ) ) {
			if( !IsHelper ) {
				World.Instance.UIPlay.btnAtk.lockCharge = true;
			}

			CommandAction( eActionCommand.AttackDuringAttack, null );
		}
	}

	public override bool OnHit( Unit attacker, BattleOption.eToExecuteType toExecuteType, AniEvent.sEvent attackerAniEvt, ObscuredFloat damage,
								ref bool isCritical, ref eHitState hitState, Projectile projectile, bool isUltimateSkill, bool skipMaxDamageRecord ) {
		ActionAnneroseEvadeCharge2Attack actionCounterAttack = null;
		if ( hitState != eHitState.OnlyEffect ) {
			actionCounterAttack = m_actionSystem.GetCurrentAction<ActionAnneroseEvadeCharge2Attack>();
			if ( actionCounterAttack != null && CurrentSuperArmor == eSuperArmor.None && hitState == eHitState.Success ) {
				ForceSetSuperArmor( eSuperArmor.Lv2 );
				hitState = eHitState.OnlyDamage;
			}
		}

		bool breakShield = base.OnHit( attacker, toExecuteType, attackerAniEvt, damage, ref isCritical, ref hitState, 
									   projectile, isUltimateSkill, skipMaxDamageRecord );

		if ( actionCounterAttack != null ) {
			actionCounterAttack.OnUpdating( null );
		}

		return breakShield;
	}

	protected override void FixedUpdate() {
		base.FixedUpdate();

		if( IsHelper || !m_actionSystem.HasAction( eActionCommand.TimingHoldAttack ) ) {
			return;
		}

		AniEvent.sAniInfo aniInfo = m_aniEvent.GetAniInfo( m_aniEvent.curAniType );
		if( aniInfo.timingHoldFrame <= 0.0f ) {
			if( World.Instance.UIPlay.EffDashFlash.gameObject.activeSelf ) {
				World.Instance.UIPlay.EffDashFlash.gameObject.SetActive( false );
			}

			return;
		}

		if( m_aniEvent.GetCurrentFrame() >= aniInfo.timingHoldFrame ) {
			EffectManager.Instance.Play( this, 1032, EffectManager.eType.Common );

			if( !World.Instance.UIPlay.EffDashFlash.gameObject.activeSelf ) {
				World.Instance.UIPlay.EffDashFlash.gameObject.SetActive( true );
			}
		}
		else {
			if( World.Instance.UIPlay.EffDashFlash.gameObject.activeSelf ) {
				World.Instance.UIPlay.EffDashFlash.gameObject.SetActive( false );
			}
		}
	}
}
