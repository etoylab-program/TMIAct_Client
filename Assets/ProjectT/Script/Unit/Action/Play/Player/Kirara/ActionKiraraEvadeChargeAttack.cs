
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionKiraraEvadeChargeAttack : ActionSelectSkillBase
{
    public enum eState
    {
        Jump = 0,
        AttackStart,
        Falling,
        AttackEnd,
        End,
    }


    private Player          mPlayer         = null;
    private State           mState          = new State();
    private eAnimation      mCurAni         = eAnimation.EvadeCharge;
    private UnitCollider    mTargetCollider = null;
    private Vector3         mDestPos        = Vector3.zero;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.HoldingDefBtnAttack;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        extraCancelCondition = new eActionCondition[3];
        extraCancelCondition[0] = eActionCondition.UseSkill;
        extraCancelCondition[1] = eActionCondition.UseQTE;
        extraCancelCondition[2] = eActionCondition.UseUSkill;

        superArmor = Unit.eSuperArmor.Lv1;

		mState.Init(5);
        mState.Bind(eState.Jump, ChangeJumpState);
        mState.Bind(eState.AttackStart, ChangeAttackStartState);
        mState.Bind(eState.Falling, ChangeFallingState);
        mState.Bind(eState.AttackEnd, ChangeAttackEndState);
        mState.Bind(eState.End, ChangeEndState);

        mPlayer = m_owner as Player;

        mCurAni = eAnimation.EvadeCharge;
        if (mValue1 >= 1.0f)
        {
            List<Projectile> list = m_owner.aniEvent.GetAllProjectile(mCurAni);
            for (int i = 0; i < list.Count; i++)
            {
                list[i].SetProjectileAtkAttr(Projectile.EProjectileAtkAttr.STUN);
            }
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        ShowSkillNames(m_data);

        mDestPos = m_owner.transform.position;

        mTargetCollider = m_owner.GetMainTargetCollider(true, 0.0f, false, false);
        if (mTargetCollider)
        {
            mDestPos = m_owner.GetTargetCapsuleEdgePos(mTargetCollider.Owner);
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
                    if (m_owner.isFalling || m_owner.isGrounded)
                    {
                        mState.ChangeState(eState.AttackStart, true);
                    }
                    break;

                case eState.AttackStart:
                    m_checkTime += m_owner.fixedDeltaTime;
                    if (m_checkTime >= m_aniLength)
                    {
                        mState.ChangeState(eState.Falling, false);
                    }
                    break;

                case eState.Falling:
                    if (m_owner.isGrounded == true)
                    {
                        mState.ChangeState(eState.AttackEnd, true);
                    }
                    break;

                case eState.AttackEnd:
                    if (m_owner.aniEvent.IsAniPlaying(eAnimation.Jump03) == eAniPlayingState.End)
                    {
                        mState.ChangeState(eState.End, true);
                    }
                    break;

                case eState.End:
                    m_endUpdate = true;
                    break;
            }

            if ((eState)mState.current != eState.AttackStart)
            {
                m_owner.cmptJump.UpdateJump();
            }

            /*
            if (mTargetCollider && (eState)mState.current == eState.Jump)
            {
                if (Vector3.Distance(m_owner.transform.position, new Vector3(mDestPos.x, m_owner.transform.position.y, mDestPos.z)) > mTargetCollider.radius)
                {
                    Vector3 v = (mTargetCollider.Owner.transform.position + (mTargetCollider.Owner.transform.forward * mTargetCollider.radius)) - m_owner.transform.position;
                    m_owner.cmptMovement.UpdatePosition(v, m_owner.speed * 10.0f, true);
                }
            }
            */

            yield return mWaitForFixedUpdate;
        }
    }

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( mCurAni );
        if (evt == null)
        {
            Debug.LogError(mCurAni.ToString() + " 공격 이벤트가 없네??");
            return 0.0f;
        }
        else if (evt.visionRange <= 0.0f)
        {
            Debug.LogError(mCurAni.ToString() + " Vistion Range가 0이네??");
        }

        return evt.visionRange;
    }

    private bool ChangeJumpState(bool changeAni)
    {
        Utility.IgnorePhysics(eLayer.Player, eLayer.Enemy);
        Utility.IgnorePhysics(eLayer.Player, eLayer.EnemyGate);
        
        if(mTargetCollider)
        {
            Vector3 v = m_owner.transform.position;

            v = Vector3.Lerp(mDestPos, v, 0.4f);
            v.y = m_owner.transform.position.y;

            m_owner.rigidBody.position = v;
        }

        m_owner.PlayAniImmediate(eAnimation.Jump01);
        m_owner.cmptJump.StartJump(m_owner.cmptJump.m_jumpPower);

        return true;
    }

    private bool ChangeAttackStartState(bool changeAni)
    {
        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.Enemy);
        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.EnemyGate);

        m_aniLength = m_owner.PlayAniImmediate(mCurAni);

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

        return true;
    }

    private bool ChangeFallingState(bool changeAni)
    {
        m_owner.PlayAniImmediate(eAnimation.Jump02);
        return true;
    }

    private bool ChangeAttackEndState(bool changeAni)
    {
        m_owner.PlayAniImmediate(eAnimation.Jump03);
        return true;
    }

    private bool ChangeEndState(bool changeAni)
    {
        return true;
    }
}
