
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Maika : Player
{
	protected override void OnEventAtk()
	{
		if (!m_actionSystem.IsCurrentUSkillAction())
		{
			bool isJumpAttack = false;

			ActionBase currentAction = m_actionSystem.currentAction;
			if (currentAction != null && (currentAction.actionCommand == eActionCommand.Jump || currentAction.actionCommand == eActionCommand.UpperJump ||
										  currentAction.actionCommand == eActionCommand.JumpAttack) && isGrounded == false && m_cmptJump.highest >= 0.8f)
			{
				if (World.Instance.EnemyMgr.HasFloatingEnemy())
				{
					if (currentAction != null && currentAction.actionCommand == eActionCommand.JumpAttack)
					{
						isJumpAttack = true;
						currentAction.OnUpdating(null);
					}
					else
					{
						isJumpAttack = true;
						CommandAction(eActionCommand.JumpAttack, null);
					}
				}
			}

			if (!isJumpAttack)
			{
				ActionDash actionDash = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionDash;
				if (actionDash && actionDash.IsPossibleToDashAttack())
				{
					CommandAction(eActionCommand.RushAttack, null);
				}
				else
				{
					if (currentAction != null && currentAction.actionCommand == eActionCommand.Attack01)
					{
						currentAction.OnUpdating(null);
					}
					else
					{
						CommandAction(eActionCommand.Attack01, null);
					}
				}
			}
		}
	}

	protected override void OnEventDefence( IActionBaseParam param = null )
	{
		if (m_actionSystem.IsCurrentUSkillAction() == true)
		{
			return;
		}

		ActionDash actionDash = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionDash;
		ActionSelectSkillBase action = m_actionSystem.GetAction<ActionSelectSkillBase>(eActionCommand.Teleport);
		if (action && action.PossibleToUse && actionDash && actionDash.IsPossibleToDashAttack() && GetMainTargetCollider(true))
		{
			float checkTime = actionDash.GetEvadeCutFrameLength();
			if (World.Instance.UIPlay.btnDash.deltaTime < checkTime)
			{
				CommandAction(eActionCommand.Teleport, null);
				return;
			}
		}

		CommandAction( eActionCommand.Defence, param );
	}

	protected override void OnEventSpecialAtk() {
		if( m_actionSystem.IsCurrentSkillAction() || m_actionSystem.IsCurrentUSkillAction() ) {
			ActionMaikaChargeAttack action = m_actionSystem.GetCurrentAction<ActionMaikaChargeAttack>();
			if( action ) {
				action.SetNextAttackDuringAttackAction();
			}

			return;
		}

		if( m_actionSystem.IsCurrentAction( eActionCommand.Attack01 ) ) {
			ActionRandomAttack action = m_actionSystem.GetCurrentAction<ActionRandomAttack>();
			if( action ) {
				if( !IsHelper ) {
					World.Instance.UIPlay.btnAtk.lockCharge = true;
				}

				action.ChangeEndUpdateTime();
				action.SetNextAction( eActionCommand.AttackDuringAttack, null );
			}
		}
	}

	public override float GetAirAttackJumpPower()
	{
		if (m_cmptJump == null)
		{
			return 0.0f;
		}

		return m_cmptJump.m_jumpPower;
	}
}
