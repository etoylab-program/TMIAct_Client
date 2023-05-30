
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionEmilyJumpThrowDrone : ActionSelectSkillBase
{
    public enum eState
    {
        Jump = 0,
        AttackStart,
        Falling,
        EndFall,
    }


    private State mState = new State();
    private eAnimation mCurAni = eAnimation.JumpAttack;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.HoldingDefBtnAttack;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        superArmor = Unit.eSuperArmor.Lv1;

		mState.Init(4);
        mState.Bind(eState.Jump, ChangeJumpState);
        mState.Bind(eState.AttackStart, ChangeAttackStartState);
        mState.Bind(eState.Falling, ChangeFallingState);
        mState.Bind(eState.EndFall, ChangeEndFallState);

        if (mValue1 > 0.0f)
        {
            mCurAni = eAnimation.JumpAttack2;
        }
        else
        {
            mCurAni = eAnimation.JumpAttack;
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        ShowSkillNames(m_data);

        if (FSaveData.Instance.AutoTargetingSkill)
        {
            Unit target = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
            if (target)
            {
                m_owner.LookAtTarget(target.transform.position);
            }
        }
        else
        {
            m_owner.cmptRotate.UpdateRotation(m_owner.Input.GetDirection(), true);
        }

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
                        mState.ChangeState(eState.AttackStart, true);
                    break;

                case eState.AttackStart:
                    m_checkTime += m_owner.fixedDeltaTime;
                    if (m_checkTime >= m_aniLength)
                        mState.ChangeState(eState.Falling, true);
                    else
                        m_owner.rigidBody.velocity = new Vector3(0.0f, (-Physics.gravity.y * 0.993f) * m_owner.fixedDeltaTime, 0.0f);
                    break;

                case eState.Falling:
                    if (m_owner.isGrounded == true)
                        mState.ChangeState(eState.EndFall, true);
                    break;

                case eState.EndFall:
                    m_checkTime += m_owner.fixedDeltaTime;
                    if (m_checkTime >= m_aniLength)
                    {
                        m_endUpdate = true;
                    }
                    break;
            }

            if((eState)mState.current != eState.AttackStart)
                m_owner.cmptJump.UpdateJump();

            yield return mWaitForFixedUpdate;
        }
    }

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( mCurAni );
        if (evt == null)
        {
            Debug.LogError(mCurAni.ToString() + "공격 이벤트가 없네??");
            return 0.0f;
        }
        else if (evt.visionRange <= 0.0f)
        {
            Debug.LogError(mCurAni.ToString() + "Vistion Range가 0이네??");
        }

        return evt.visionRange;
    }

    private bool ChangeJumpState(bool changeAni)
    {
        Utility.IgnorePhysics(eLayer.Player, eLayer.Enemy);
        Utility.IgnorePhysics(eLayer.Player, eLayer.EnemyGate);

        m_owner.PlayAniImmediate(eAnimation.Jump01);
        m_owner.cmptJump.StartJump(m_owner.cmptJump.m_jumpPower);

        return true;
    }

    private bool ChangeAttackStartState(bool changeAni)
    {
        m_aniLength = m_owner.PlayAniImmediate(mCurAni);
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
        m_aniLength = m_owner.PlayAni(eAnimation.Jump03);
        m_checkTime = 0.0f;

        return true;
    }
}
