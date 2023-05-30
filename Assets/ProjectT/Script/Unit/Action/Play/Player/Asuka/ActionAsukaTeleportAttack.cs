
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionAsukaTeleportAttack : ActionSelectSkillBase
{
    public enum eState
    {
        Jump = 0,

        DownAttackStart,
        DownAttackDoing,
        DownAttackEnd,

        End,
    }


    private Player          mPlayer         = null;
    private ActionDash      mActionDash     = null;
    private State           mState          = new State();
    private eAnimation      mCurAni         = eAnimation.EvadeDouble_1;
    private UnitCollider    mTargetCollider = null;
    private Vector3         mDestPos        = Vector3.zero;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Teleport;

        conditionActionCommand = new eActionCommand[1];
        conditionActionCommand[0] = eActionCommand.Defence;

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.Defence;

        superArmor = Unit.eSuperArmor.Lv1;

		mState.Init(5);
        mState.Bind(eState.Jump, ChangeJumpState);
        mState.Bind(eState.DownAttackStart, ChangeDownAttackStartState);
        mState.Bind(eState.DownAttackDoing, ChangeDownAttackDoingState);
        mState.Bind(eState.DownAttackEnd, ChangeDownAttackEndState);
        mState.Bind(eState.End, ChangeEndState);

        mActionDash = m_owner.actionSystem.GetAction<ActionDash>(eActionCommand.Defence);
        mPlayer = m_owner as Player;

        mCurAni = eAnimation.EvadeDouble_1;
        if(mValue2 > 0.0f)
        {
            mCurAni = eAnimation.EvadeDouble_2;
        }

        if (mValue1 > 0.0f)
        {
            List<AniEvent.sEvent> listEvent = m_owner.aniEvent.GetAllAttackEvent(mCurAni);
            for (int i = 0; i < listEvent.Count; i++)
            {
                listEvent[i].behaviour = eBehaviour.UpperAttack;
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
            mDestPos = m_owner.GetTargetCapsuleEdgePosForTeleport( mTargetCollider.Owner );
        }

        mState.ChangeState(eState.Jump, true);
    }

    public override IEnumerator UpdateAction()
    {
        float startCutFrameLength = m_owner.aniEvent.GetCutFrameLength(eAnimation.EvadeDouble);
        bool fastFall = false;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            switch ((eState)mState.current)
            {
                case eState.Jump:
                    if (m_owner.isFalling || m_owner.isGrounded)
                    {
                        mState.ChangeState(eState.DownAttackStart, true);
                    }
                    break;

                case eState.DownAttackStart:
                    m_checkTime += m_owner.fixedDeltaTime;
                    if (m_checkTime >= startCutFrameLength)
                    {
                        mState.ChangeState(eState.DownAttackDoing, false);
                    }
                    break;

                case eState.DownAttackDoing:
                    m_checkTime += m_owner.fixedDeltaTime;
                    if (m_checkTime >= m_aniLength)
                    {
                        if (m_owner.isGrounded == false)
                        {
                            mState.ChangeState(eState.DownAttackEnd, true);
                        }
                    }
                    
                    if (m_owner.isGrounded == true)
                    {
                        mState.ChangeState(eState.DownAttackEnd, true);
                    }
                    break;

                case eState.DownAttackEnd:
                    if (m_owner.aniEvent.IsAniPlaying(mCurAni) == eAniPlayingState.End)
                    {
                        mState.ChangeState(eState.End, true);
                    }
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

            if (mTargetCollider && (eState)mState.current == eState.DownAttackDoing)
            {
                if (Vector3.Distance(m_owner.transform.position, new Vector3(mDestPos.x, m_owner.transform.position.y, mDestPos.z)) > mTargetCollider.radius)
                {
                    Vector3 v = (mTargetCollider.Owner.transform.position + (mTargetCollider.Owner.transform.forward * mTargetCollider.radius)) - m_owner.transform.position;
                    m_owner.cmptMovement.UpdatePosition(v, m_owner.speed * 10.0f, true);
                }
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        mActionDash.InitChainCount();
    }

    public override void OnCancel()
    {
        base.OnCancel();
        mActionDash.InitChainCount();
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

            //m_owner.rigidBody.MovePosition(m_owner.rigidBody.position + (v - m_owner.transform.position));
            m_owner.rigidBody.position = v;
            //m_owner.transform.position = v;
        }

        m_owner.PlayAniImmediate(eAnimation.Jump01);
        m_owner.cmptJump.StartJump(m_owner.cmptJump.m_jumpPower);

        m_owner.ShowMesh(false);
        return true;
    }

    private bool ChangeDownAttackStartState(bool changeAni)
    {
        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.Enemy);
        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.EnemyGate);

        m_owner.ShowMesh(true);
        return true;
    }

    private bool ChangeDownAttackDoingState(bool changeAni)
    {
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.EvadeDouble);
        m_checkTime = 0;
        return true;
    }

    private bool ChangeDownAttackEndState(bool changeAni)
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetEventOnEndAction(mCurAni);
        if (evt != null)
        {
            m_owner.OnAttackOnEndAction(evt);
        }

        m_owner.PlayAniImmediate(mCurAni);
        return true;
    }

    private bool ChangeEndState(bool changeAni)
    {
        return true;
    }
}
