
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class HitDown : ActionHitBase
{
    public enum eState
    {
        Start = 0,
        Idle,
        Hit,
        End
    }


    private static readonly float END_TIME = 3.0f;

    protected State m_state = new State();
    private float m_checkEndTime = 0.0f;
    private bool m_delayedStart = false;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Down;

		m_state.Init(4);
        m_state.Bind(eState.Start, ChangeStartState);
        m_state.Bind(eState.Idle, ChangeIdleState);
        m_state.Bind(eState.Hit, ChangeHitState);
        m_state.Bind(eState.End, ChangeEndState);
    }

    public override void OnStart(IActionBaseParam param)
    {
        ActionParamHit hitParam = param as ActionParamHit;
        hitParam.skip = true;

        base.OnStart(hitParam);

        m_delayedStart = false;
        if (m_owner.isGrounded)
            m_state.ChangeState(eState.Start, true);
        else
            m_delayedStart = true;
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        if (m_delayedStart)
        {
            while (!m_owner.isGrounded)
            {
                yield return mWaitForFixedUpdate;
            }

            m_state.ChangeState(eState.Start, true);
        }

        while (m_endUpdate == false)
        {
            m_owner.rigidBody.velocity = Vector3.zero;

            if((eState)m_state.current != eState.Start)
                m_checkEndTime += Time.fixedDeltaTime;

            if (m_checkEndTime >= m_owner.DownDuration)
                m_state.ChangeState(eState.End, true);
            else
            {
                m_checkTime += Time.fixedDeltaTime;

                switch ((eState)m_state.current)
                {
                    case eState.Start:
                        if (m_checkTime >= m_aniLength)
                            m_state.ChangeState(eState.Idle, true);
                        break;

                    case eState.Hit:
                        if (m_checkTime >= m_aniLength)
                            m_state.ChangeState(eState.Idle, true);
                        break;
                }
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnUpdating(IActionBaseParam param)
    {
        base.OnUpdating(param);
        m_state.ChangeState(eState.Hit, true);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        m_owner.LockDie(false);
    }

    public override void OnCancel()
    {
        if (m_owner.isGrounded == false)
            m_owner.SetFallingRigidBody();

        m_endUpdate = true;
        isPlaying = false;

        m_owner.LockDie(false);
        m_owner.RestoreSuperArmor(mChangedSuperArmorId);
    }

    private bool ChangeStartState(bool changeAni)
    {
        m_checkTime = 0.0f;
        m_checkEndTime = 0.0f;

        m_aniLength = m_owner.PlayAniImmediate(eAnimation.Down);
        return true;
    }

    private bool ChangeIdleState(bool changeAni)
    {
        if (changeAni == true)
            m_aniLength = m_owner.PlayAni(eAnimation.DownIdle);

        m_checkTime = 0.0f;
        return true;
    }

    private bool ChangeHitState(bool changeAni)
    {
        if (changeAni)
        {
            m_owner.LockDie(false);
            m_aniLength = m_owner.PlayAniImmediate(eAnimation.DownHit);
        }

        m_checkTime = 0.0f;
        return true;
    }

    private bool ChangeEndState(bool changeAni)
    {
        m_endUpdate = true;
        m_owner.DownDuration = END_TIME;

        return true;
    }
}
