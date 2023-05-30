
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Asagi : Player
{
    /*protected override void AddQTEComponent()
    {
        ActionQTEBase actionQTE = gameObject.AddComponent<ActionQTECloneAtk>();
        if(actionQTE == null)
        {
            Debug.LogError("ActionQTECloneAtk이 없습니다.");
            return;
        }

        actionQTE.qteType = ActionQTEBase.eQTEType.Pounding;
        actionQTE.executeCondition = ActionQTEBase.eExecuteCondition.None;
        actionQTE.executeButton = ActionQTEBase.eQTEButton.Attack;
        actionQTE.duration = 3.0f;

        actionQTE.commands = new ActionQTEBase.sCommandInfo[1];
        actionQTE.commands[0].command = ActionQTEBase.eQTEButton.Attack;
        actionQTE.commands[0].aniLength = 0.0f;

        actionQTE.isAllUnitPause = false;
        actionQTE.aniPrepare = eAnimation.None;
    }*/

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

            if(!isJumpAttack)
            {
                ActionDash actionDash = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionDash;
                if (actionDash && actionDash.IsPossibleToDashAttack())
                {
                    CommandAction(eActionCommand.RushAttack, null);
                }
                else
                {
                    ActionAsagiChainRushAttack actionChainRushAtk = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionAsagiChainRushAttack;
                    if (actionChainRushAtk)
                    {
                        actionChainRushAtk.OnUpdating(null);
                    }
                    else if (currentAction != null && currentAction.actionCommand == eActionCommand.Attack01)
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
            return;

        ActionDash actionDash = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionDash;
        ActionAsagiTeleport actionTeleport = m_actionSystem.GetAction<ActionAsagiTeleport>(eActionCommand.Teleport);
        if (actionTeleport && actionTeleport.PossibleToUse && actionDash && actionDash.IsPossibleToDashAttack() && GetMainTargetCollider(true))
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

    public override float GetAirAttackJumpPower()
    {
        if (m_cmptJump == null)
            return 0.0f;

        //ActionAsagiUpperAttack action = m_actionSystem.GetAction<ActionAsagiUpperAttack>(eActionCommand.AttackDuringAttack);
        //if (action == null || action.passiveDatas[0].level == eActionLevel.None)
            return m_cmptJump.m_jumpPower;

        //return action.highJumpPower;
    }

	public override void OnGameStart()
	{
		base.OnGameStart();

		for (int i = 0; i < m_listClone.Count; ++i)
		{
			m_listClone[i].SendBattleOptionToOwner = true;
		}
	}
}
