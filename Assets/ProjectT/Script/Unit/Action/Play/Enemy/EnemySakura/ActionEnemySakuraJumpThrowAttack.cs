
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionEnemySakuraJumpThrowAttack : ActionEnemyTaimaninBase
{
    public enum eState
    {
        Jump = 0,
        AttackStart,
        Falling,
        EndFall,
    }


    private State mState = new State();


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.HoldingDefBtnAttack;

		mState.Init(4);
        mState.Bind(eState.Jump, ChangeJumpState);
        mState.Bind(eState.AttackStart, ChangeAttackStartState);
        mState.Bind(eState.Falling, ChangeFallingState);
        mState.Bind(eState.EndFall, ChangeEndFallState);
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        mState.ChangeState(eState.Jump, true);
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            switch ((eState)mState.current)
            {
                case eState.Jump:
                    if (m_owner.isFalling)
                    {
                        mState.ChangeState(eState.AttackStart, true);
                    }
                    break;

                case eState.AttackStart:
                    m_checkTime += m_owner.fixedDeltaTime;

                    if (m_checkTime >= m_aniLength)
                    {
                        mState.ChangeState(eState.Falling, true);
                    }
                    else
                    {
                        m_owner.rigidBody.velocity = new Vector3(0.0f, (-Physics.gravity.y * 0.993f) * m_owner.fixedDeltaTime, 0.0f);
                    }
                    break;

                case eState.Falling:
                    if (m_owner.isGrounded == true)
                    {
                        mState.ChangeState(eState.EndFall, true);
                    }
                    break;

                case eState.EndFall:
                    if (m_owner.aniEvent.IsAniPlaying(eAnimation.Jump03) == eAniPlayingState.End)
                    {
                        m_endUpdate = true;
                    }
                    break;
            }

            if ((eState)mState.current != eState.AttackStart)
            {
                m_owner.cmptJump.UpdateJump();
            }

            yield return mWaitForFixedUpdate;
        }
    }

    private bool ChangeJumpState(bool changeAni)
    {
        Utility.IgnorePhysics(eLayer.Player, eLayer.Enemy);

        m_owner.PlayAniImmediate(eAnimation.Jump01);
        m_owner.cmptJump.StartJump(m_owner.cmptJump.m_jumpPower);

        return true;
    }

    private bool ChangeAttackStartState(bool changeAni)
    {
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.JumpAttack);
        m_owner.SetFallingRigidBody();

        return true;
    }

    private bool ChangeFallingState(bool changeAni)
    {
        m_owner.PlayAni(eAnimation.Jump02);
        return true;
    }

    private bool ChangeEndFallState(bool changeAni)
    {
        m_owner.PlayAni(eAnimation.Jump03);
        return true;
    }
}
