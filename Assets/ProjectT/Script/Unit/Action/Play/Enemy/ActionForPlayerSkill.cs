
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionForPlayerSkill : ActionEnemyBase
{
    private Enemy m_enemy = null;
    private ActionParamForPlayerSkill m_forPlayerSkillParam;
    private eAnimation m_hitAni = eAnimation.None;
    private eAnimation m_normalAni = eAnimation.None;
    private bool m_lastAttack = false;
    private HitStandUp m_actionStandUp = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.ActionForPlayerSkill;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        ActionParamForPlayerSkill forPlayerSkillParam = param as ActionParamForPlayerSkill;
        m_enemy = m_owner as Enemy;

        m_enemy.StopBT();
        m_enemy.LockDie(true);
        m_enemy.SetKinematicRigidBody(true);

        if (!m_enemy.isGrounded)
        {
            Vector3 attackerPos = forPlayerSkillParam.attacker.transform.position;
            Vector3 pos = m_owner.transform.position;
            pos.y = attackerPos.y;

            m_enemy.SetInitialPosition(pos, m_owner.transform.rotation);
        }
        
        //m_enemy.SetCapsuleColDirection(Unit.eCapsuleColAxis.Y);
        m_lastAttack = false;

        m_forPlayerSkillParam = param as ActionParamForPlayerSkill;
        switch (m_forPlayerSkillParam.state)
        {
            case ActionParamForPlayerSkill.eState.Down:
                m_owner.PlayAniImmediate(eAnimation.DownIdle);
                //m_owner.ShowMesh(false);
                break;

            case ActionParamForPlayerSkill.eState.Stun:
                m_owner.PlayAniImmediate(eAnimation.Stun);
                break;
        }

        m_actionStandUp = null;
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            if(m_aniLength > 0.0f)
            {
                m_checkTime += m_owner.fixedDeltaTime;
                if(m_checkTime >= m_aniLength)
                {
                    if (!m_lastAttack)
                    {
                        m_owner.PlayAniImmediate(m_normalAni);

                        m_checkTime = 0.0f;
                        m_aniLength = 0.0f;
                    }
                    else
                        m_endUpdate = true;
                }
            }

            yield return mWaitForFixedUpdate;
        }

        if(m_forPlayerSkillParam.state == ActionParamForPlayerSkill.eState.Down)
        {
            m_actionStandUp = m_owner.GetComponent<HitStandUp>();
            if(m_actionStandUp)
            {
                m_actionStandUp.OnStart(null);
                yield return m_actionStandUp.UpdateAction();
            }
        }
    }

    public override void OnUpdating(IActionBaseParam param)
    {
        if(m_actionStandUp)
        {
            m_actionStandUp.OnUpdating(param);
            return;
        }

        ActionParamHit hitParam = param as ActionParamHit;
        if (hitParam.hitEffId > 0)
            EffectManager.Instance.Play(m_owner, hitParam.hitEffId, EffectManager.eType.Each_Monster_Normal_Hit);

        if (m_owner.HasHitAni())
        {
            /*
            if (hitParam.attackerBehaviour == eBehaviour.StrongAttack) // 막타
            {
                switch (m_forPlayerSkillParam.state)
                {
                    case ActionParamForPlayerSkill.eState.Down:
                        m_hitAni = eAnimation.DownHit;
                        m_normalAni = eAnimation.DownIdle;
                        break;

                    case ActionParamForPlayerSkill.eState.Stun:
                        m_hitAni = eAnimation.Hit;
                        m_normalAni = eAnimation.Stun;
                        break;
                }

                m_aniLength = m_owner.PlayAniImmediate(m_hitAni);
                m_lastAttack = true;
            }
            else
            */
            {
                switch (m_forPlayerSkillParam.state)
                {
                    case ActionParamForPlayerSkill.eState.Down:
                        m_hitAni = eAnimation.DownHit;
                        m_normalAni = eAnimation.DownIdle;
                        break;

                    case ActionParamForPlayerSkill.eState.Stun:
                        m_hitAni = eAnimation.Hit;
                        m_normalAni = eAnimation.Stun;
                        break;
                }

                m_aniLength = m_owner.PlayAniImmediate(m_hitAni);
            }

            StopCoroutine("StartCheckPauseFrame");
            StartCoroutine("StartCheckPauseFrame");
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();

        //m_enemy.ShowMesh(true);
        m_enemy.SetGroundedRigidBody();
        m_enemy.LockDie(false);
        m_enemy.SetKinematicRigidBody(false);

        if (m_enemy.curHp <= 0.0f)
            SetNextAction(eActionCommand.Die, null);
        else
            m_enemy.ResetBT();
    }

    public void EndImmediate()
    {
        m_endUpdate = true;
    }

    protected IEnumerator StartCheckPauseFrame()
    {
        m_owner.StopPauseFrame();
        if (m_owner.attacker != null)
            m_owner.attacker.StopPauseFrame();

        // 히트 애니에 설정돼있는 CutFrame부터 멈춰야함
        float cutFrameLength = m_owner.aniEvent.GetCutFrameLength(m_hitAni);
        yield return new WaitForSeconds(cutFrameLength);// m_aniLength * GameInfo.Instance.BattleConfig.StartPauseFrameHitLengthRatio);

        CheckPauseFrame();
        yield return null;
    }

    protected void CheckPauseFrame()
    {
        if (m_owner.aniEvent.aniSpeed < 1.0f)
            return;

        if (m_owner.CompareTag("Player"))
            return;

        float cutFrameLength = m_owner.aniEvent.GetCutFrameLength(m_hitAni);

        if (m_owner.attackerAniEvt.pauseFrame > 0.0f)
        {
            m_owner.StartPauseFrame(false, m_owner.attackerAniEvt.pauseFrame, false);
            if (m_owner.attacker != null)
                m_owner.attacker.StartPauseFrame(false, m_owner.attackerAniEvt.pauseFrame, true);
        }
    }
}
