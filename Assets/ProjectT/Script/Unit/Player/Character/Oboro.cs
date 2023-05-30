
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;


public class Oboro : Player {
	protected override void OnEventAtk() {
		if( !m_actionSystem.IsCurrentUSkillAction() ) {
			bool isJumpAttack = false;

			ActionBase currentAction = m_actionSystem.currentAction;
			if( currentAction != null && ( currentAction.actionCommand == eActionCommand.Jump || currentAction.actionCommand == eActionCommand.UpperJump ||
										   currentAction.actionCommand == eActionCommand.JumpAttack ) && isGrounded == false && m_cmptJump.highest >= 0.8f ) {
				if( World.Instance.EnemyMgr.HasFloatingEnemy() ) {
					if( currentAction != null && currentAction.actionCommand == eActionCommand.JumpAttack ) {
						isJumpAttack = true;
						currentAction.OnUpdating( null );
					}
					else {
						isJumpAttack = true;
						CommandAction( eActionCommand.JumpAttack, null );
					}
				}
			}

			if( !isJumpAttack ) {
				ActionDash actionDash = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionDash;
				if( actionDash && actionDash.IsPossibleToDashAttack() ) {
					CommandAction( eActionCommand.RushAttack, null );
				}
				else {
					ActionAsagiChainRushAttack actionChainRushAtk = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionAsagiChainRushAttack;
					if( actionChainRushAtk ) {
						actionChainRushAtk.OnUpdating( null );
					}
					else if( currentAction != null && currentAction.actionCommand == eActionCommand.Attack01 ) {
						currentAction.OnUpdating( null );
					}
					else {
						CommandAction( eActionCommand.Attack01, null );
					}
				}
			}
		}
	}

	protected override void OnEventDefence( IActionBaseParam param = null ) {
		if( m_actionSystem.IsCurrentUSkillAction() == true ) {
			return;
		}

		ActionDash actionDash = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionDash;
		ActionTeleport actionTeleport = m_actionSystem.GetAction<ActionTeleport>(eActionCommand.Teleport);

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

	public override bool OnHit( Unit attacker, BattleOption.eToExecuteType toExecuteType, AniEvent.sEvent attackerAniEvt, ObscuredFloat damage, 
								ref bool isCritical, ref eHitState hitState, Projectile projectile, bool isUltimateSkill, bool skipMaxDamageRecord ) {
		ActionOboroCounterAttack actionCounterAttack = null;
		if ( hitState != eHitState.OnlyEffect ) {
			actionCounterAttack = m_actionSystem.GetCurrentAction<ActionOboroCounterAttack>();
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
}
