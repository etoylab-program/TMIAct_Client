
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;


public class Murasaki : Player {
	public override float GetAirAttackJumpPower() {
		if( m_cmptJump == null ) {
			return 0.0f;
		}

		return m_cmptJump.m_jumpPower;
	}

	public override bool OnHit( Unit attacker, BattleOption.eToExecuteType toExecuteType, AniEvent.sEvent attackerAniEvt, ObscuredFloat damage, 
								ref bool isCritical, ref eHitState hitState, Projectile projectile, bool isUltimateSkill, bool skipMaxDamageRecord ) {
		ActionMurasakiCounterAttack actionCounterAttack = null;
		if ( hitState != eHitState.OnlyEffect ) {
			actionCounterAttack = m_actionSystem.GetCurrentAction<ActionMurasakiCounterAttack>();
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

	protected override void OnEventAtk() {
		if( !m_actionSystem.IsCurrentUSkillAction() ) {
			ActionBase currentAction = m_actionSystem.currentAction;

			ActionDash actionDash = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionDash;
			if( actionDash && actionDash.IsPossibleToDashAttack() ) {
				CommandAction( eActionCommand.RushAttack, null );
			}
			else {
				ActionMurasakiChainRushAttack actionChainRushAtk = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionMurasakiChainRushAttack;
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

	protected override void OnEventDefence( IActionBaseParam param = null ) {
		if( m_actionSystem.IsCurrentUSkillAction() == true ) {
			return;
		}

		if( m_actionSystem.HasAction( eActionCommand.TimingHoldAttack ) ) {
			AniEvent.sAniInfo aniInfo = m_aniEvent.GetAniInfo(m_aniEvent.curAniType);
			if( aniInfo.timingHoldFrame > 0.0f && m_aniEvent.GetCurrentFrame() >= aniInfo.timingHoldFrame ) {
				CommandAction( eActionCommand.TimingHoldAttack, null );
				return;
			}
		}

		ActionDash actionDash = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionDash;
		ActionSelectSkillBase action = m_actionSystem.GetAction<ActionSelectSkillBase>(eActionCommand.Teleport);

		if( action && action.PossibleToUse && actionDash && actionDash.IsPossibleToDashAttack() && GetMainTargetCollider( true ) ) {
			float checkTime = actionDash.GetEvadeCutFrameLength();
		
			if( World.Instance.UIPlay.btnDash.deltaTime < checkTime ) {
				CommandAction( eActionCommand.Teleport, null );
				return;
			}
		}

		if( actionDash ) {
			ActionMurasakiHookAttack actionHookAttack = m_actionSystem.GetAction<ActionMurasakiHookAttack>(eActionCommand.Teleport);
			if( actionHookAttack ) {
				if( actionHookAttack.PossibleToUse && actionDash.IsPossibleToDashAttack() && GetMainTargetCollider( true ) ) {
					float checkTime = actionDash.GetEvadeCutFrameLength();

					if( World.Instance.UIPlay.btnDash.deltaTime < checkTime ) {
						CommandAction( eActionCommand.Teleport, null );
						return;
					}
				}
			}
		}

		CommandAction( eActionCommand.Defence, param );
	}

	protected override void OnEventSpecialAtk() {
		if( m_actionSystem.IsCurrentSkillAction() || m_actionSystem.IsCurrentUSkillAction() ) {
			return;
		}

		ActionUpperAttack actionUpperAtk = m_actionSystem.GetAction<ActionUpperAttack>(eActionCommand.AttackDuringAttack);
		if( actionUpperAtk == null ) {
			return;
		}

		ActionComboAttack actionComboAttack = m_actionSystem.GetCurrentAction<ActionComboAttack>();
		if( actionComboAttack != null && actionComboAttack.IsLastAttack() ) {
			if( !IsHelper ) {
				World.Instance.UIPlay.btnAtk.lockCharge = true;
			}

			CommandAction( eActionCommand.AttackDuringAttack, null );
		}
	}

	protected override void OnEventChargeAtkStart( IActionBaseParam param = null ) {
		ActionMurasakiComboAttack actionComboAttack = m_actionSystem.GetCurrentAction<ActionMurasakiComboAttack>();
		if( actionComboAttack && !actionComboAttack.IsLastAttack() ) {
			actionComboAttack.StartCharging();
		}
		else {
			base.OnEventChargeAtkStart( param );
		}
	}

	protected override void OnEventChargeAtkEnd( IActionBaseParam param = null ) {
		ActionMurasakiComboAttack actionComboAttack = m_actionSystem.GetCurrentAction<ActionMurasakiComboAttack>();
		if( actionComboAttack && actionComboAttack.IsCharging ) {
			actionComboAttack.EndCharging();
		}
		else {
			base.OnEventChargeAtkEnd( param );
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
}
