
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionOboroJumpComboAttack : ActionJumpComboAttack
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        attackAnimations = new eAnimation[4];
        attackAnimations[0] = eAnimation.AirAttack01;
        attackAnimations[1] = eAnimation.AirAttack02;
        attackAnimations[2] = eAnimation.AirAttack03;
        attackAnimations[3] = eAnimation.AirAttack04;
    }

    protected override bool ChangeAttackState(bool changeAni)
    {
        if (m_owner.isGrounded || m_owner.IsBeforeGround(0.3f))
        {
            SetNextAction(eActionCommand.None, null);
            m_state.ChangeState(eState.EndFall, true);
        }
        else
        {
            if (m_nextAttackIndex >= attackAnimations.Length)
                m_state.ChangeState(eState.JumpDownAttack, true);
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
