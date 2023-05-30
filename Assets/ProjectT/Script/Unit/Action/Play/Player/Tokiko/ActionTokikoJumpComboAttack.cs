
using UnityEngine;
using System.Collections;


public class ActionTokikoJumpComboAttack : ActionJumpComboAttack
{
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
