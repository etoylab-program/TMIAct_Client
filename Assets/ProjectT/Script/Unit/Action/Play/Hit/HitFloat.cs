
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class HitFloat : ActionHitBase
{
    public enum eState
    {
        Start = 0,
        Float,
        Fall,
        FloatEnd,
        Bounding,
        DownIdle,
        End
    }


    public eState state { get { return (eState)m_state.current; } }

    protected State m_state = new State();

    private float   mHitLength      = 0.0f;
    private eState  mNextFallState  = eState.FloatEnd;
    private float   mDownIdleTime   = 0.5f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Floating;

		m_state.Init(7);
        m_state.Bind(eState.Start, ChangeStartState);
        m_state.Bind(eState.Float, ChangeFloatState);
        m_state.Bind(eState.Fall, ChangeFallState);
        m_state.Bind(eState.FloatEnd, ChangeFloatEndState);
        m_state.Bind(eState.Bounding, ChangeBoundingState);
        m_state.Bind(eState.DownIdle, ChangeDownIdleState);
        m_state.Bind(eState.End, ChangeEndState);
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        m_state.ChangeState(eState.Start, true);
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            switch ((eState)m_state.current)
            {
                case eState.Start:
                    if (m_checkTime >= m_aniLength)
                        m_state.ChangeState(eState.Float, true);
                    break;

                case eState.Float:
                    if (m_owner.isFalling || m_owner.isGrounded)
                        m_state.ChangeState(eState.Fall, true);
                    break;

                case eState.Fall:
                    if (m_owner.isGrounded == true)
                        m_state.ChangeState(mNextFallState, true);
                    break;

                case eState.FloatEnd:
                case eState.Bounding:
                    if (m_checkTime >= m_aniLength)
                    {
                        if (m_owner.isGrounded == true)
                            m_state.ChangeState(eState.DownIdle, true);
                        else
                            m_state.ChangeState(eState.Fall, true);
                    }
                    break;

                case eState.DownIdle:
                    if (m_checkTime >= mDownIdleTime)
                    {
                        m_state.ChangeState(eState.End, true);
                    }
                    break;
            }

            if ((eState)m_state.current != eState.FloatEnd && (eState)m_state.current != eState.DownIdle)
                m_owner.cmptJump.UpdateJump();

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnUpdating(IActionBaseParam param)
    {
        m_hitParam = param as ActionParamHit;
        if (m_hitParam.attackerBehaviour == eBehaviour.FastFallAttack)
        {
            m_hitParam.skip = true;
        }
        else
        {
            m_hitParam.skip = false;
        }

        base.OnUpdating(m_hitParam);

        if (m_owner.IsBeforeGround(0.3f))
        {
            return;
        }

        if (!m_owner.isGrounded)
        {
            mHitLength = m_owner.PlayAniImmediate(eAnimation.FloatingHit);

            if (m_hitParam.attackerBehaviour == eBehaviour.FastFallAttack || m_hitParam.attackerBehaviour == eBehaviour.DownAttack)
            {
                m_owner.cmptJump.SetFastFall();

                if (m_owner.aniEvent.HasAni(eAnimation.Bounding))
                    mNextFallState = eState.Bounding;

                m_state.ChangeState(eState.Fall, false);
            }
            else
            {
                m_owner.cmptJump.StartShortJump(0.1f, true, 0.5f);
                m_state.ChangeState(eState.Float, false);
            }
        }
        else
        {
            m_owner.LockDie(false);
            mHitLength = m_owner.PlayAniImmediate(eAnimation.DownHit);
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();

        m_endUpdate = true;
        m_owner.LockDie(false);

        if (m_owner as Player && m_owner.AI)
        {
            m_owner.ResetBT();
        }
    }

    public override void OnCancel()
    {
        if (m_owner.isGrounded == false)
            m_owner.SetFallingRigidBody();

        m_endUpdate = true;
        isPlaying = false;

        m_owner.LockDie(false);
        m_owner.RestoreSuperArmor(mChangedSuperArmorId);

        if (m_owner as Player && m_owner.AI)
        {
            m_owner.ResetBT();
        }
    }

    public void SetDownIdleTime(float time)
    {
        mDownIdleTime = time;
    }

    private bool ChangeStartState(bool changeAni)
    {
        mNextFallState = eState.FloatEnd;

        if(m_owner as Player && m_owner.AI)
        {
            m_owner.StopBT();
        }

        m_owner.cmptJump.InitPos();
        //m_cmptJump.StartJump(true, 10.0f / Application.targetFrameRate);
        m_owner.cmptJump.StartJump(m_hitParam.attackerJumpPower, true, 10.0f / Application.targetFrameRate);

        LookAtTarget(m_owner.attacker);

        if (changeAni == true)
            m_aniLength = m_owner.PlayAniImmediate(eAnimation.FloatingStart);

        m_checkTime = 0.0f;
        mDownIdleTime = 0.5f;

        return true;
    }

    private bool ChangeFloatState(bool changeAni)
    {
        if (changeAni == true)
            m_aniLength = m_owner.PlayAni(eAnimation.Floating);

        m_checkTime = 0.0f;
        return true;
    }

    private bool ChangeFallState(bool changeAni)
    {
        if (changeAni == true)
        {
            eAnimation ani = eAnimation.FloatingFall;
            if (m_owner.aniEvent.HasAni(ani) == false)
                ani = eAnimation.Floating;

            m_aniLength = m_owner.PlayAni(ani);
        }

        m_checkTime = 0.0f;
        return true;
    }

    private bool ChangeFloatEndState(bool changeAni)
    {
        if (changeAni)
            m_aniLength = m_owner.PlayAniImmediate(eAnimation.FloatingEnd);

        m_checkTime = 0.0f;
        return true;
    }

    private bool ChangeBoundingState(bool changeAni)
    {
        if (changeAni)
            m_aniLength = m_owner.PlayAniImmediate(eAnimation.Bounding);

        m_checkTime = 0.0f;
        return true;
    }

    private bool ChangeDownIdleState(bool changeAni)
    {
        if (changeAni == true)
            m_aniLength = m_owner.PlayAni(eAnimation.DownIdle);

        m_checkTime = 0.0f;
        return true;
    }

    private bool ChangeEndState(bool changeAni)
    {
        m_endUpdate = true;
        return true;
    }
}
