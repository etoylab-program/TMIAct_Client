﻿
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ActionRinkoJumpComboAttack : ActionJumpComboAttack
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        attackAnimations = new eAnimation[3];
        attackAnimations[0] = eAnimation.AirAttack01;
        attackAnimations[1] = eAnimation.AirAttack02;
        attackAnimations[2] = eAnimation.AirAttack03;
    }

    protected override bool ChangeAttackState(bool changeAni)
    {
        if (m_owner.isGrounded || m_owner.IsBeforeGround(0.3f))
        {
            SetNextAction(eActionCommand.None, null);
            m_endUpdate = true;
        }
        else
        {
            if (m_nextAttackIndex >= attackAnimations.Length)
            {
                m_state.ChangeState(eState.JumpDownAttack, true);
            }
            else
            {
                StartAttack();

                if (FSaveData.Instance.AutoTargeting)
                {
                    mTargetCollider = m_owner.GetMainTargetCollider(true, 0.0f, false, true);
                    if (mTargetCollider)
                    {
                        LookAtTarget(mTargetCollider.Owner);
                    }
                }

                m_owner.SetFallingRigidBody();
            }
        }

        return true;
    }
}
