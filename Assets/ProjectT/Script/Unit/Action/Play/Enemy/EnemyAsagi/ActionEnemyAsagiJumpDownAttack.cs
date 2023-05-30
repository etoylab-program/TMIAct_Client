
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionEnemyAsagiJumpDownAttack : ActionEnemyTaimaninBase
{
    public enum eState
    {
        Jump = 0,

        DownAttackStart,
        DownAttackDoing,
        DownAttackEnd,

        End,
    }


    private State mState = new State();


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.HoldingDefBtnAttack;

		mState.Init(5);
        mState.Bind(eState.Jump, ChangeJumpState);
        mState.Bind(eState.DownAttackStart, ChangeDownAttackStartState);
        mState.Bind(eState.DownAttackDoing, ChangeDownAttackDoingState);
        mState.Bind(eState.DownAttackEnd, ChangeDownAttackEndState);
        mState.Bind(eState.End, ChangeEndState);
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        mState.ChangeState(eState.Jump, true);
    }

    public override IEnumerator UpdateAction()
    {
        float startCutFrameLength = m_owner.aniEvent.GetCutFrameLength(eAnimation.PrepareGestureSkill03);
        bool fastFall = false;

        UnitCollider targetCollider = m_owner.GetMainTargetCollider(true, 0.0f, false, false);
        Vector3 destPos = m_owner.GetTargetCapsuleEdgePos(targetCollider.Owner);

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            switch ((eState)mState.current)
            {
                case eState.Jump:
                    if (m_owner.isFalling || m_owner.isGrounded)
                        mState.ChangeState(eState.DownAttackStart, true);
                    break;

                case eState.DownAttackStart:
                    m_checkTime += m_owner.fixedDeltaTime;
                    if (m_checkTime >= startCutFrameLength)
                        mState.ChangeState(eState.DownAttackDoing, false);
                    break;

                case eState.DownAttackDoing:
                    if (m_owner.isGrounded == true)
                        mState.ChangeState(eState.DownAttackEnd, true);
                    break;

                case eState.DownAttackEnd:
                    if (m_owner.aniEvent.IsAniPlaying(eAnimation.GestureSkill03) == eAniPlayingState.End)
                        mState.ChangeState(eState.End, true);
                    break;

                case eState.End:
                    m_endUpdate = true;
                    break;
            }

            if ((eState)mState.current >= eState.DownAttackDoing && (eState)mState.current <= eState.DownAttackEnd)
            {
                if (!fastFall && m_owner.isFalling)
                {
                    m_owner.cmptJump.SetFastFall();
                    fastFall = true;
                }
            }

            m_owner.cmptJump.UpdateJump();

            if (targetCollider && (eState)mState.current < eState.DownAttackEnd)
            {
                if (Vector3.Distance(m_owner.transform.position, new Vector3(destPos.x, m_owner.transform.position.y, destPos.z)) > targetCollider.radius)
                {
                    Vector3 v = (targetCollider.Owner.transform.position + (targetCollider.Owner.transform.forward * targetCollider.radius)) - m_owner.transform.position;
                    m_owner.cmptMovement.UpdatePosition(v, m_owner.speed * 5.0f, true);
                }
            }

            yield return mWaitForFixedUpdate;
        }
    }

    private bool ChangeJumpState(bool changeAni)
    {
        Utility.IgnorePhysics(eLayer.Player, eLayer.Enemy);

        m_owner.PlayAniImmediate(eAnimation.Jump01);
        m_owner.cmptJump.StartJump(m_owner.cmptJump.m_jumpPower);
        m_owner.ShowMesh(false);

        return true;
    }

    private bool ChangeDownAttackStartState(bool changeAni)
    {
        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.Enemy);
        m_owner.ShowMesh(true);

        return true;
    }

    private bool ChangeDownAttackDoingState(bool changeAni)
    {
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.PrepareGestureSkill03);
        m_owner.cmptJump.StartShortJump(m_owner.cmptJump.m_shortJumpPowerRatio, false, 0.0f);

        return true;
    }

    private bool ChangeDownAttackEndState(bool changeAni)
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetEventOnEndAction(eAnimation.GestureSkill03);
        if (evt != null)
            m_owner.OnAttackOnEndAction(evt);

        m_owner.PlayAniImmediate(eAnimation.GestureSkill03);
        return true;
    }

    private bool ChangeEndState(bool changeAni)
    {
        return true;
    }
}
