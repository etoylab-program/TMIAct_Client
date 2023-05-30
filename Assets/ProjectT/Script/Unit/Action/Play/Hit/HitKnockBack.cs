
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class HitKnockBack : ActionHitBase
{
    public enum eState
    {
        Start = 0,
        DownIdle,
        DownHit,
        End
    }


    private static readonly float DOWN_IDLE_TIME = 1.0f;

    protected State m_state = new State();
    private float m_downTime = 0.0f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.KnockBack;

		m_state.Init(4);
        m_state.Bind(eState.Start, ChangeStartState);
        m_state.Bind(eState.DownIdle, ChangeDownIdleState);
        m_state.Bind(eState.DownHit, ChangeDownHitState);
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
            if (m_owner.GetKnockBackType() == Unit.eKnockBackType.KnockDown)
            {
                if ((eState)m_state.current == eState.DownIdle || (eState)m_state.current == eState.DownHit)
                    m_downTime += m_owner.fixedDeltaTime;

                m_checkTime += m_owner.fixedDeltaTime;
                switch ((eState)m_state.current)
                {
                    case eState.Start:
                        if (m_checkTime >= m_aniLength)
                            m_state.ChangeState(eState.DownIdle, true);
                        break;

                    case eState.DownIdle:
                        if (m_downTime >= DOWN_IDLE_TIME)
                            m_state.ChangeState(eState.End, true);
                        break;

                    case eState.DownHit:
                        if (m_checkTime >= m_aniLength)
                            m_state.ChangeState(eState.DownIdle, true);
                        break;

                    case eState.End:
                        m_endUpdate = true;
                        break;
                }
            }
            else //if (m_owner.aniEvent.IsAniPlaying(eAnimation.KnockBack) == eAniPlayingState.End)
            {
                m_checkTime += m_owner.fixedDeltaTime;
                if (m_checkTime >= m_aniLength)
                {
                    m_endUpdate = true;
                }
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnUpdating(IActionBaseParam param)
    {
        if ((eState)m_state.current != eState.DownIdle)
            return;

        m_hitParam = param as ActionParamHit;
        if (m_hitParam.attackerBehaviour == eBehaviour.FastFallAttack)
            m_hitParam.skip = true;

        base.OnUpdating(m_hitParam);
        m_state.ChangeState(eState.DownHit, true);
    }

    public override void OnCancel()
    {
        base.OnCancel();
        m_owner.reserveStun = false;
    }

    public bool ChangeStartState(bool changeAni)
    {
        if (m_owner.isGrounded == false)
            m_owner.SetFallingRigidBody();

        m_owner.LookAtTarget(m_owner.attacker.transform.position);

        m_aniLength = m_owner.PlayAniImmediate(eAnimation.KnockBack);
        m_downTime = 0.0f;

        return true;
    }

    public bool ChangeDownIdleState(bool changeAni)
    {
        m_aniLength = m_owner.PlayAni(eAnimation.DownIdle);
        return true;
    }

    public bool ChangeDownHitState(bool changeAni)
    {
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.DownHit);
        m_checkTime = 0.0f;

        return true;
    }

    public bool ChangeEndState(bool changeAni)
    {
        return true;
    }
}
