
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ActionHitBase : ActionBase
{
    protected ActionParamHit m_hitParam;
    protected eAnimation m_hitAni;
    protected bool m_pauseFrame;

    public ActionHitBase Parent { get; set; }


    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        m_hitParam = m_param as ActionParamHit;

        if (m_hitParam != null && !m_hitParam.skip && m_hitParam.hitState == eHitState.Success && m_hitParam.hitEffId > 0)
            EffectManager.Instance.Play(m_owner, m_hitParam.hitEffId, m_hitParam.critical == true ? EffectManager.eType.Each_Monster_Critical_Hit : EffectManager.eType.Each_Monster_Normal_Hit);

        m_pauseFrame = false;

        if (m_hitParam != null)
            m_hitAni = m_owner.GetHitAni(m_hitParam.hitDir);
        else
            m_hitAni = m_owner.GetHitAni(eHitDirection.None);

        m_owner.SetStepForwardRatio(1.0f, 1.0f);
        World.Instance.InGameCamera.EndUserSetting();
    }

    protected IEnumerator StartCheckPauseFrame()
    {
        m_owner.StopPauseFrame();
        if (m_owner.attacker != null)
            m_owner.attacker.StopPauseFrame();

        m_pauseFrame = false;

        // 히트 애니에 설정돼있는 CutFrame부터 멈춰야함
        float cutFrameLength = m_owner.aniEvent.GetCutFrameLength(m_hitAni);
        yield return new WaitForSeconds(cutFrameLength);// m_aniLength * GameInfo.Instance.BattleConfig.StartPauseFrameHitLengthRatio);

        CheckPauseFrame();
        yield return null;
    }

    protected void CheckPauseFrame()
    {
        if (m_owner.aniEvent.aniSpeed < 1.0f || m_owner.attackerAniEvt == null)
            return;

        if (m_pauseFrame == true || m_owner.CompareTag("Player"))
            return;

        float cutFrameLength = m_owner.aniEvent.GetCutFrameLength(m_hitAni);

        if (m_owner.attackerAniEvt.pauseFrame > 0.0f)
        {
            m_owner.StartPauseFrame(m_hitParam.critical, m_owner.attackerAniEvt.pauseFrame, false);

            if (m_owner.attacker != null)
                m_owner.attacker.StartPauseFrame(m_hitParam.critical, m_owner.attackerAniEvt.pauseFrame, true);

            m_pauseFrame = true;
        }
    }

    public override void OnUpdating(IActionBaseParam param)
    {
        m_hitParam = param as ActionParamHit;
        if (!m_hitParam.skip && m_hitParam.hitEffId > 0)
            EffectManager.Instance.Play(m_owner, m_hitParam.hitEffId, m_hitParam.critical == true ? EffectManager.eType.Each_Monster_Critical_Hit : EffectManager.eType.Each_Monster_Normal_Hit);

        if (!m_hitParam.skip)
        {
            StopCoroutine("StartCheckPauseFrame");
            StartCoroutine("StartCheckPauseFrame");
        }
    }

    public override void OnCancel()
    {
        m_owner.StopPauseFrame();

        if (m_owner.attacker != null)
            m_owner.attacker.StopPauseFrame();

        if (m_owner.isGrounded == false)
            m_owner.SetFallingRigidBody();

        base.OnCancel();
    }

    /*protected void SetNextDownDie()
    {
        ActionDie action = m_owner.actionSystem.GetAction<ActionDie>(eActionCommand.Die);
        if (action == null)
            return;

        action.SetDownDie();
        SetNextAction(eActionCommand.Die);
    }*/
}
