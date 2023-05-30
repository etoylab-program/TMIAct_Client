
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ActionJumpComboAttack : ActionSelectSkillBase
{
    public enum eState
    {
        Attack,
        StartFall,
        Falling,
        EndFall,
        JumpDownAttack,
    }


    [Header("[Set Animation]")]
    public eAnimation[] attackAnimations;

    public bool reserved { get { return m_reserved; } }

    protected State m_state = new State();

    protected eAnimation m_currentAni = eAnimation.None;
    protected int m_currentAttackIndex = 0;
    protected int m_nextAttackIndex = 0;
    protected bool m_reserved = false;
    protected float m_atkRange = 0.0f;
    protected UnitCollider mTargetCollider = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.JumpAttack;

        ExplicitStartCoolTime = true;
        IsRepeatSkill = true;

        extraCondition = new eActionCondition[1];
        extraCondition[0] = eActionCondition.Jumping;

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.Defence;

        extraCancelCondition = new eActionCondition[3];
        extraCancelCondition[0] = eActionCondition.UseSkill;
        extraCancelCondition[1] = eActionCondition.UseQTE;
        extraCancelCondition[2] = eActionCondition.UseUSkill;

        attackAnimations = new eAnimation[4];
        attackAnimations[0] = eAnimation.AirAttack01;
        attackAnimations[1] = eAnimation.AirAttack02;
        attackAnimations[2] = eAnimation.AirAttack03;
        attackAnimations[3] = eAnimation.AirAttack04;

        superArmor = Unit.eSuperArmor.Lv1;

		m_state.Init(5);
        m_state.Bind(eState.Attack, ChangeAttackState);
        m_state.Bind(eState.StartFall, null);
        m_state.Bind(eState.Falling, ChangeFallingState);
        m_state.Bind(eState.EndFall, ChangeEndFallState);
        m_state.Bind(eState.JumpDownAttack, ChangeJumpDownAttackState);
    }

    public bool IsLastAttack()
    {
        if (m_currentAttackIndex == attackAnimations.Length - 1)
            return true;

        return false;
    }

    protected virtual void StartAttack()
    {
        m_currentAni = attackAnimations[m_currentAttackIndex];
        m_aniLength = m_owner.PlayAniImmediate(m_currentAni);

        m_atkRange = m_owner.aniEvent.GetFirstAttackEventRange(m_currentAni);
    }

    protected virtual bool IsStartFall()
    {
        return m_currentAttackIndex >= attackAnimations.Length;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        m_owner.ShowMesh(true);
        m_state.ChangeState(eState.Attack, true);
    }

    public override IEnumerator UpdateAction()
    {
        float checkAniLength = 0.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            switch ((eState)m_state.current)
            {
                case eState.Attack:
                    if (IsStartFall())
                        m_state.ChangeState(eState.StartFall, true);
                    else
                    {
                        m_checkTime += m_owner.fixedDeltaTime;

                        if (m_nextAttackIndex > m_currentAttackIndex)
                            checkAniLength = m_owner.aniEvent.GetCutFrameLength(m_currentAni);
                        else
                            checkAniLength = m_owner.aniEvent.GetAniLength(m_currentAni);

                        if (m_checkTime >= checkAniLength)
                        {
                            if (!m_owner.isGrounded && !m_owner.IsBeforeGround(0.3f))
                            {
                                if (m_reserved && m_nextAttackIndex > 0)
                                {
                                    m_reserved = false;
                                    m_checkTime = 0.0f;
                                    m_currentAttackIndex = m_nextAttackIndex;

                                    m_state.ChangeState(eState.Attack, true);
                                }
                                else
                                    m_state.ChangeState(eState.StartFall, true);
                            }
                            else
                                m_state.ChangeState(eState.EndFall, true);
                        }

                        m_owner.rigidBody.velocity = new Vector3(0.0f, (-Physics.gravity.y * 0.993f) * m_owner.fixedDeltaTime, 0.0f);
                    }

                    if(!FSaveData.Instance.AutoTargeting)
                    {
                        m_owner.cmptRotate.UpdateRotation(m_owner.Input.GetDirection(), true);
                    }

                    break;

                case eState.StartFall:
                    if (m_owner.isGrounded == false)
                        m_state.ChangeState(eState.Falling, true);
                    else
                        m_state.ChangeState(eState.EndFall, true);
                    break;

                case eState.Falling:
                    if (m_owner.isGrounded == true)
                        m_state.ChangeState(eState.EndFall, true);
                    break;

                case eState.EndFall:
                    if (m_owner.aniEvent.IsAniPlaying(eAnimation.Jump03) == eAniPlayingState.End)
                        m_endUpdate = true;
                    break;
            }

            if ((eState)m_state.current == eState.Falling)
                m_owner.rigidBody.AddForce(-Vector3.up * 50.0f * m_owner.fixedDeltaTime, ForceMode.VelocityChange);

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnUpdating(IActionBaseParam param)
    {
        if (m_reserved == true)
            return;

        m_nextAttackIndex = m_currentAttackIndex + 1;
        m_reserved = true;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        isPlaying = false;

        m_currentAttackIndex = 0;
        m_nextAttackIndex = 0;
        m_reserved = false;
    }

    public override void OnCancel()
    {
        isPlaying = false;
        isCancel = true;

        m_currentAttackIndex = 0;
        m_nextAttackIndex = 0;
        m_reserved = false;

        m_owner.RestoreSuperArmor(mChangedSuperArmorId);
    }

    protected virtual bool ChangeAttackState(bool changeAni)
    {
        if (m_owner.isGrounded || m_owner.IsBeforeGround(0.3f))
        {
            SetNextAction(eActionCommand.None, null);
            m_endUpdate = true;
        }
        else
        {
            if (m_nextAttackIndex < attackAnimations.Length)
            {
                mTargetCollider = m_owner.GetMainTargetCollider(true, 0.0f, false, true);
                StartAttack();

                if (mTargetCollider)
                {
                    LookAtTarget(mTargetCollider.Owner);
                }

                m_owner.SetFallingRigidBody();
            }
        }

        return true;
    }

    private bool ChangeFallingState(bool changeAni)
    {
        m_owner.PlayAni(eAnimation.Jump02);
        return true;
    }

    private bool ChangeEndFallState(bool changeAni)
    {
        SetNextAction(eActionCommand.None, null);
        m_owner.PlayAni(eAnimation.Jump03);

        return true;
    }

    private bool ChangeJumpDownAttackState(bool changeAni)
    {
        if(m_owner.actionSystem.GetAction(eActionCommand.AirDownAttack) == null)
        {
            return false;
        }

        m_endUpdate = true;
        SetNextAction(eActionCommand.AirDownAttack, null);

        return true;
    }
}
