
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionYukikazeJumpSlowFallingAttack : ActionSelectSkillBase
{
    public enum eState
    {
        Jump = 0,
        Start,
        Doing,
        End,
    }


    private State m_state = new State();
    private bool m_slowFalling = false;
    //private bool m_repeat = false;
    private float m_slowFallingGravityRatio = 0.7f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        
        actionCommand = eActionCommand.HoldingDefBtnAttack;
        //IsRepeatSkill = true;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        superArmor = Unit.eSuperArmor.Lv1;

		m_state.Init(4);
        m_state.Bind(eState.Jump, ChangeJumpState);
        m_state.Bind(eState.Start, ChangeStartState);
        m_state.Bind(eState.Doing, ChangeDoingState);
        m_state.Bind(eState.End, ChangeEndState);

        m_slowFallingGravityRatio = Mathf.Clamp(m_slowFallingGravityRatio + (m_slowFallingGravityRatio * (mValue1 / (float)eCOUNT.MAX_BO_FUNC_VALUE)), 0.7f, 0.903f);
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        m_state.ChangeState(eState.Jump, true);
    }

    public override IEnumerator UpdateAction()
    {
        Vector3 v = Vector3.zero;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            switch ((eState)m_state.current)
            {
                case eState.Jump:
                    if (m_owner.isFalling || m_checkTime >= m_aniLength)
                    {
                        m_state.ChangeState(eState.Start, true);
                    }
                    break;

                case eState.Start:
                    if (m_checkTime >= m_aniLength)
                        m_state.ChangeState(eState.Doing, true);
                    break;

                case eState.Doing:
                    if (m_owner.IsBeforeGround(0.3f) || m_owner.isGrounded)
                    {
                        m_state.ChangeState(eState.End, true);
                    }
                    else if (m_checkTime >= m_aniLength)
                    {
                        //if(m_repeat)
                        m_state.ChangeState(eState.Doing, true);
                        //else
                        //    m_slowFalling = false;
                    }

                    break;

                case eState.End:
                    //if (m_owner.aniEvent.IsAniPlaying(eAnimation.FinishSlowFallingAttack) == eAniPlayingState.End)
                    //    m_endUpdate = true;
                    if (m_owner.aniEvent.curAniType != eAnimation.FinishSlowFallingAttack)
                    {
                        m_state.ChangeState(eState.End, true);
                    }
                    else  if (m_checkTime >= m_aniLength)
                    {
                        m_endUpdate = true;
                    }
                    break;
            }

            if ((eState)m_state.current == eState.Doing && m_slowFalling)
            {
                if(m_owner.rigidBody.velocity == Vector3.zero)
                {
                    m_owner.rigidBody.velocity = v;
                }

                m_owner.rigidBody.AddForce(Vector3.up * (-Physics.gravity.y * m_slowFallingGravityRatio) * Time.fixedDeltaTime, ForceMode.VelocityChange);

                v = m_owner.rigidBody.velocity;
            }
            else if ((eState)m_state.current <= eState.Start)
                m_owner.cmptJump.UpdateJump();

            yield return mWaitForFixedUpdate;
        }
    }

	/*
    public override void OnUpdating(IActionBaseParam param)
    {
        if ((eState)m_state.current != eState.Doing)
            return;

        m_repeat = true;
    }
    */

	public override float GetAtkRange() {
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( eAnimation.PrepareSlowFallingAttack );
		if ( evt == null ) {
			return 0.0f;
		}

        if ( m_owner.AI ) {
            return evt.visionRange * 1.5f;
        }

		return evt.visionRange;
	}

	private bool ChangeJumpState(bool changeAni)
    {
        Utility.IgnorePhysics(eLayer.Player, eLayer.Enemy);
        Utility.IgnorePhysics(eLayer.Player, eLayer.EnemyGate);

        m_aniLength = m_owner.PlayAniImmediate(eAnimation.Jump01);
        m_owner.cmptJump.StartJump(m_owner.cmptJump.m_jumpPower);

        return true;
    }

    private bool ChangeStartState(bool changeAni)
    {
        m_owner.cmptJump.StartShortJump(m_owner.cmptJump.m_shortJumpPowerRatio * 0.7f, false, 0.0f);

        m_checkTime = 0.0f;
        m_slowFalling = true;

        m_aniLength = m_owner.PlayAni(eAnimation.PrepareSlowFallingAttack);
        ShowSkillNames(m_data);

        return true;
    }

    private bool ChangeDoingState(bool changeAni)
    {
        m_owner.SetFallingRigidBody();

        m_checkTime = 0.0f;
        m_slowFalling = true;
        //m_repeat = false;

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
