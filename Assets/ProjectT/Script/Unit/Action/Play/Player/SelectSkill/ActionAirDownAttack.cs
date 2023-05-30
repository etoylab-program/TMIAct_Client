
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionAirDownAttack : ActionSelectSkillBase
{
    public enum eState
    {
        Start = 0,
        Doing,
        End,
    }


    protected State m_state = new State();
    protected eAnimation m_curAni = eAnimation.GestureSkill03;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.AirDownAttack;

        ExplicitStartCoolTime = true;

        extraCondition = new eActionCondition[3];
        extraCondition[0] = eActionCondition.Jumping;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingUSkill;

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

        superArmor = Unit.eSuperArmor.Lv1;

		m_state.Init(3);
        m_state.Bind(eState.Start, ChangeStartState);
        m_state.Bind(eState.Doing, null);
        m_state.Bind(eState.End, ChangeEndState);

        m_jumpAttack = true;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        if (m_owner.isGrounded == false)
            m_owner.SetFloatingRigidBody();

        m_state.ChangeState(eState.Start, true);
        //World.Instance.InGameCamera.SetDefaultMode(new Vector3(0.0f, 4.0f, -5.0f), new Vector2(0.0f, 1.0f), false, 1.0f);
    }

    public override IEnumerator UpdateAction()
    {
        float startCutFrameLength = m_owner.aniEvent.GetCutFrameLength(eAnimation.PrepareGestureSkill03);
        bool fastFall = false;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            switch((eState)m_state.current)
            {
                case eState.Start:
                    m_checkTime += m_owner.fixedDeltaTime;
                    if (m_checkTime >= startCutFrameLength)
                        m_state.ChangeState(eState.Doing, false);
                    break;

                case eState.Doing:
                    if (m_owner.isGrounded == true)
                        m_state.ChangeState(eState.End, true);
                    break;

                case eState.End:
                    //if (m_owner.aniEvent.IsAniPlaying(m_curAni) == eAniPlayingState.End)
                    m_checkTime += m_owner.fixedDeltaTime;
                    if (m_checkTime >= m_aniLength)
                    {
                        m_endUpdate = true;
                    }
                    break;
            }

            if (!fastFall && m_owner.isFalling)
            {
                m_owner.cmptJump.SetFastFall();
                fastFall = true;
            }

            m_owner.cmptJump.UpdateJump();
            yield return mWaitForFixedUpdate;
        }
    }

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(m_curAni);
        if (evt == null)
        {
            Debug.LogError(m_curAni.ToString() + "공격 이벤트가 없네??");
            return 0.0f;
        }
        else if(evt.visionRange <= 0.0f)
        {
            Debug.LogError(m_curAni.ToString() + "Vistion Range가 0이네??");
        }

        return evt.visionRange;
    }

    protected virtual bool ChangeStartState(bool changeAni)
    {
        m_owner.ShowMesh(true);

        m_curAni = eAnimation.PrepareGestureSkill03;
        m_aniLength = m_owner.PlayAniImmediate(m_curAni);

        m_owner.cmptJump.StartShortJump(m_owner.cmptJump.m_shortJumpPowerRatio, false, 0.0f);
        
        //World.Instance.playerCam.SetControlMode(PlayerCamera.eOtherControlMode.JumpDownAtk, 1.5f);
        return true;
    }

    protected virtual bool ChangeEndState(bool changeAni)
    {
        m_curAni = GetEndStateAni();

        AniEvent.sEvent evt = m_owner.aniEvent.GetEventOnEndAction(m_curAni);
        if (evt != null)
            m_owner.OnAttackOnEndAction(evt);

        m_aniLength = m_owner.PlayAniImmediate(m_curAni);
        m_checkTime = 0.0f;

        //World.Instance.playerCam.SetControlMode(PlayerCamera.eOtherControlMode.Battle);
        return true;
    }

    protected virtual eAnimation GetEndStateAni()
    {
        return eAnimation.GestureSkill03;
    }
}
