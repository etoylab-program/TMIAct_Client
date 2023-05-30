
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Astaroth : Player
{
    private ActionAstarothAttackHoldAttack mActionAtkHoldAtk = null;


    public override void OnAfterChangeWeapon()
    {
        base.OnAfterChangeWeapon();
        mActionAtkHoldAtk = m_actionSystem.GetAction<ActionAstarothAttackHoldAttack>(eActionCommand.AttackDuringAttack);
    }

    public override void OnAfterCreateInPVP()
    {
        mActionAtkHoldAtk = m_actionSystem.GetAction<ActionAstarothAttackHoldAttack>(eActionCommand.AttackDuringAttack);
    }

	public override void OnAfterChangePlayableChar() {
		base.OnAfterChangePlayableChar();
        OnEventAtkTouchEnd();
    }

	protected override void OnEventAtk()
    {
        ActionDash actionDash = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionDash;
        if (actionDash && actionDash.IsPossibleToDashAttack())
        {
            CommandAction(eActionCommand.RushAttack, null);
        }
    }

    protected override void OnEventDefence( IActionBaseParam param = null )
    {
        if (m_actionSystem.IsCurrentUSkillAction() == true)
        {
            return;
        }

        ActionDash actionDash = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionDash;
        ActionSelectSkillBase actionTeleport = m_actionSystem.GetAction<ActionSelectSkillBase>(eActionCommand.Teleport);
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

    protected override void OnEventAtkPressStart()
    {
        if (skipAttack)
        {
            return;
        }

        CommandAction(eActionCommand.Attack01, null);
    }

    protected override void OnEventAtkTouchEnd()
    {
        if (m_actionSystem.IsCurrentAction(eActionCommand.Attack01) && m_curHp > 0.0f)
        {
            m_actionSystem.CancelCurrentAction();

            if (Input.GetRawDirection() != Vector3.zero)
            {
                CommandAction(eActionCommand.MoveByDirection, new ActionParamMoveByDirection(Input.GetRawDirection(), true));
            }
        }
    }

	protected override void OnEventSpecialAtk() {
    }

	protected override void OnEventChargeAtkStart( IActionBaseParam param = null ) {
    }

	protected override void OnEventChargeAtkEnd( IActionBaseParam param = null ) {
    }

	protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if(mActionAtkHoldAtk && mActionAtkHoldAtk.PossibleToUse && m_actionSystem.IsCurrentAction(eActionCommand.Attack01))
        {
            mActionAtkHoldAtk.ManuallyStart();
        }
        else if(m_actionSystem.IsCurrentAction(eActionCommand.Attack01) && Input.isPause)
        {
            OnEventAtkTouchEnd();
        }
    }
}
