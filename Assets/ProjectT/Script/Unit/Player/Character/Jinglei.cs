
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;


public class Jinglei : Player {
	public override void Init( int tableId, eCharacterType type, string faceAniControllerPath ) {
		base.Init( tableId, type, faceAniControllerPath );
		CancelComboAttack = false;
	}

	public override bool OnHit( Unit attacker, BattleOption.eToExecuteType toExecuteType, AniEvent.sEvent attackerAniEvt, ObscuredFloat damage, 
								ref bool isCritical, ref eHitState hitState, Projectile projectile, bool isUltimateSkill, bool skipMaxDamageRecord ) {
		bool breakShield = base.OnHit( attacker, toExecuteType, attackerAniEvt, damage, ref isCritical, ref hitState, 
									   projectile, isUltimateSkill, skipMaxDamageRecord );

		if( hitState == eHitState.Success && Utility.IsSpecialAttack( attackerAniEvt, projectile ) ) {
			ResetComboAttackIndex();
		}

		return breakShield;
	}

	public override void OnStartSkill( ActionSelectSkillBase currentSkillAction ) {
		if( currentSkillAction == null || currentSkillAction.actionCommand == eActionCommand.Attack01 ) {
			return;
		}

		ResetComboAttackIndex();
	}

	public override void OnStartUSkill() {
		ResetComboAttackIndex();
	}

	public override void OnEndGame() {
		base.OnEndGame();

		if( UsingUltimateSkill ) {
			PlayAniImmediate( eAnimation.Idle01 );

			EffectManager.Instance.StopEffImmediate( 30011, EffectManager.eType.Common, null );
			EffectManager.Instance.StopEffImmediate( 30012, EffectManager.eType.Common, null );
			EffectManager.Instance.StopEffImmediate( 30013, EffectManager.eType.Common, null );
			EffectManager.Instance.StopEffImmediate( 30014, EffectManager.eType.Common, null );
		}
	}

	public override void OnPVPSurrender() {
		base.OnPVPSurrender();

		if( UsingUltimateSkill ) {
			ActionJingleiUSkill action = m_actionSystem.GetAction<ActionJingleiUSkill>(eActionCommand.USkill01);
			if( action ) {
				action.ForceEnd( true );
			}
		}
	}

	protected override void OnEventDefence( IActionBaseParam param = null ) {
		if( m_actionSystem.IsCurrentUSkillAction() == true ) {
			return;
		}

		if( CheckTimingHoldAttack() ) {
			return;
		}

		ActionDash actionDash = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionDash;
		ActionJingleiTeleportAttack actionTeleport = m_actionSystem.GetAction<ActionJingleiTeleportAttack>(eActionCommand.Teleport);
		if( actionTeleport && actionTeleport.PossibleToUse && actionDash && actionDash.IsPossibleToDashAttack() && GetMainTargetCollider( true ) ) {
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
			EffectManager.Instance.Play( this, 30015, EffectManager.eType.Common );

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

	private void ResetComboAttackIndex() {
		ActionComboAttack action = m_actionSystem.GetAction<ActionComboAttack>(eActionCommand.Attack01);
		if( action == null ) {
			Debug.LogError( "진레이 콤보 어택 액션이 왜 없어???" );
			return;
		}

		action.ResetComboAttackIndex();
	}
}
