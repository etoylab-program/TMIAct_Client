
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionAsagiJumpDownAttack : ActionSelectSkillBase
{
    public enum eState
    {
        Jump = 0,

        DownAttackStart,
        DownAttackDoing,
        DownAttackEnd,

        End,
    }


    protected Player            mPlayer         = null;
    protected State             mState          = new State();
    protected ParticleSystem    mEffJumpStart   = null;
    protected ParticleSystem    mEffJumpEnd     = null;


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

		mState.Init(5);
        mState.Bind(eState.Jump, ChangeJumpState);
        mState.Bind(eState.DownAttackStart, ChangeDownAttackStartState);
        mState.Bind(eState.DownAttackDoing, ChangeDownAttackDoingState);
        mState.Bind(eState.DownAttackEnd, ChangeDownAttackEndState);
        mState.Bind(eState.End, ChangeEndState);

        mPlayer = m_owner as Player;

        mEffJumpStart = GameSupport.CreateParticle("Effect/Character/prf_fx_asagi_teleport_start.prefab", mPlayer.transform);
        mEffJumpEnd = GameSupport.CreateParticle("Effect/Character/prf_fx_asagi_teleport_finish.prefab", mPlayer.transform);
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        ShowSkillNames(m_data);

        mState.ChangeState(eState.Jump, true);
        //World.Instance.InGameCamera.SetDefaultMode(new Vector3(0.0f, 4.0f, -5.0f), new Vector2(0.0f, 1.0f), false, 1.0f);
    }

    public override IEnumerator UpdateAction()
    {
        float startCutFrameLength = m_owner.aniEvent.GetCutFrameLength(eAnimation.PrepareGestureSkill03);
        bool fastFall = false;

        Vector3 destPos = m_owner.transform.position;

        UnitCollider targetCollider = m_owner.GetMainTargetCollider(true, 0.0f, false, false);
        if(targetCollider)
        {
            destPos = m_owner.GetTargetCapsuleEdgePos(targetCollider.Owner);
        }

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
                    if (m_owner.aniEvent.IsAniPlaying(eAnimation.EvadeCharge) == eAniPlayingState.End)
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

            if (/*mValue1 > 0 &&*/ targetCollider && (eState)mState.current < eState.DownAttackEnd)
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

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(eAnimation.EvadeCharge);
        if (evt == null)
        {
            Debug.LogError( "EvadeCharge 공격 이벤트가 없네??" );
            return 0.0f;
        }
        else if (evt.visionRange <= 0.0f)
        {
            Debug.LogError( "EvadeCharge Vistion Range가 0이네??" );
        }

        return evt.visionRange;
    }

    private bool ChangeJumpState(bool changeAni)
    {
        Utility.IgnorePhysics(eLayer.Player, eLayer.Enemy);
        Utility.IgnorePhysics(eLayer.Player, eLayer.EnemyGate);

        m_owner.PlayAniImmediate(eAnimation.Jump01);
        m_owner.cmptJump.StartJump(m_owner.cmptJump.m_jumpPower);
        m_owner.ShowMesh(false);

        mEffJumpStart.gameObject.SetActive(true);
        EffectManager.Instance.RegisterStopEff(mEffJumpStart, mPlayer.transform);
        
        return true;
    }

    private bool ChangeDownAttackStartState(bool changeAni)
    {
        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.Enemy);
        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.EnemyGate);

        m_owner.ShowMesh(true);

        mEffJumpEnd.gameObject.SetActive(true);
        EffectManager.Instance.RegisterStopEff(mEffJumpEnd, mPlayer.transform);

        return true;
    }

    private bool ChangeDownAttackDoingState(bool changeAni)
    {
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.PrepareGestureSkill03);
        m_owner.cmptJump.StartShortJump(m_owner.cmptJump.m_shortJumpPowerRatio, false, 0.0f);

        //World.Instance.playerCam.SetControlMode(PlayerCamera.eOtherControlMode.JumpDownAtk, 1.5f);
        return true;
    }

    private bool ChangeDownAttackEndState(bool changeAni)
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetEventOnEndAction(eAnimation.EvadeCharge);
        if (evt != null)
            m_owner.OnAttackOnEndAction(evt);

        m_owner.PlayAniImmediate(eAnimation.EvadeCharge );

        if( SetAddAction ) {
            float duration = AddActionValue1 * (float)eCOUNT.MAX_BO_FUNC_VALUE;
            float speedRate = AddActionValue2 / (float)eCOUNT.MAX_BO_FUNC_VALUE;

            mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
            mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.Each_Monster_Normal_Hit;
            mBuffEvt.Set( m_data.ID, eEventSubject.ActiveEnemies, eEventType.EVENT_DEBUFF_SPEED_DOWN, m_owner, speedRate, 0.0f, 0.0f, duration, 0.0f,
                          mBuffEvt.effId, mBuffEvt.effId2, eBuffIconType.Debuff_Speed );

            EventMgr.Instance.SendEvent( mBuffEvt );
        }

        return true;
    }

    private bool ChangeEndState(bool changeAni)
    {
        return true;
    }
}
