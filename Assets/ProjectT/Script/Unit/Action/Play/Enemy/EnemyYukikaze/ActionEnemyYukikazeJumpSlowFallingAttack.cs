
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionEnemyYukikazeJumpSlowFallingAttack : ActionEnemyTaimaninBase
{
    public enum eState
    {
        Jump = 0,
        Start,
        Doing,
        End,
    }


    private State   mState                      = new State();
    private bool    mSlowFalling                = false;
    private float   mSlowFallingGravityRatio    = 0.7f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.HoldingDefBtnAttack;

		mState.Init(4);
        mState.Bind(eState.Jump, ChangeJumpState);
        mState.Bind(eState.Start, ChangeStartState);
        mState.Bind(eState.Doing, ChangeDoingState);
        mState.Bind(eState.End, ChangeEndState);

        mSlowFallingGravityRatio = Mathf.Clamp(mSlowFallingGravityRatio, 0.7f, 0.903f);
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
            m_checkTime += m_owner.fixedDeltaTime;

            switch ((eState)mState.current)
            {
                case eState.Jump:
                    if (m_owner.isFalling)
                    {
                        mState.ChangeState(eState.Start, true);
                    }
                    break;

                case eState.Start:
                    if (m_checkTime >= m_aniLength)
                    {
                        mState.ChangeState(eState.Doing, true);
                    }
                    break;

                case eState.Doing:
                    if (m_owner.IsBeforeGround(0.3f))
                    {
                        mState.ChangeState(eState.End, true);
                    }
                    else if (m_checkTime >= m_aniLength)
                    {
                        mState.ChangeState(eState.Doing, true);
                    }

                    break;

                case eState.End:
                    //if (m_owner.aniEvent.IsAniPlaying(eAnimation.FinishSlowFallingAttack) == eAniPlayingState.End)
                    if (m_checkTime >= m_aniLength)
                    {
                        m_endUpdate = true;
                    }
                    break;
            }

            if ((eState)mState.current == eState.Doing && mSlowFalling)
            {
                m_owner.rigidBody.AddForce(Vector3.up * (-Physics.gravity.y * mSlowFallingGravityRatio) * Time.fixedDeltaTime, ForceMode.VelocityChange);
            }
            else if ((eState)mState.current <= eState.Start)
            {
                m_owner.cmptJump.UpdateJump();
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnCancel()
    {
        base.OnCancel();
        mState.ChangeState(eState.Jump, true);
    }

    private bool ChangeJumpState(bool changeAni)
    {
        Utility.IgnorePhysics(eLayer.Player, eLayer.Enemy);

        m_owner.PlayAniImmediate(eAnimation.Jump01);
        m_owner.cmptJump.StartJump(m_owner.cmptJump.m_jumpPower);

        return true;
    }

    private bool ChangeStartState(bool changeAni)
    {
        m_checkTime = 0.0f;
        mSlowFalling = true;

        m_owner.cmptJump.StartShortJump(m_owner.cmptJump.m_shortJumpPowerRatio * 0.7f, false, 0.0f);
        m_aniLength = m_owner.PlayAni(eAnimation.PrepareSlowFallingAttack);
        
        return true;
    }

    private bool ChangeDoingState(bool changeAni)
    {
        m_owner.SetFallingRigidBody();

        m_checkTime = 0.0f;
        mSlowFalling = true;

        m_aniLength = m_owner.PlayAniImmediate(eAnimation.SlowFallingAttack);
        return true;
    }

    private bool ChangeEndState(bool changeAni)
    {
        m_checkTime = 0.0f;
        m_aniLength = m_owner.PlayAni(eAnimation.FinishSlowFallingAttack);

        return true;
    }
}
