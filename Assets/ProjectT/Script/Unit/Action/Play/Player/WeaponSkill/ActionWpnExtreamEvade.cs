
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionWpnExtreamEvade : ActionWeaponSkillBase
{
    private ActionParamFromBO   mActionParamFromBO  = null;
    private Unit                mTarget             = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.WpnExtreamEvade;
    }

    public override void OnStart(IActionBaseParam param)
    {
        mTarget = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
        if(mTarget == null)
        {
            return;
        }

        ActionSelectSkillBase actionExtreamEvade = m_owner.actionSystem.GetAction<ActionSelectSkillBase>(eActionCommand.ExtreamEvade);
        if (!actionExtreamEvade.PossibleToUse)
        {
            return;
        }

        base.OnStart(param);
        mActionParamFromBO = param as ActionParamFromBO;

        StartEvading();
    }

    private float StartEvading()
    {
        EffectManager.Instance.Play(mOwnerPlayer, mActionParamFromBO.battleOptionData.effId1, (EffectManager.eType)mActionParamFromBO.battleOptionData.effType);
        m_owner.SetEvadedTarget(mTarget);

        ActionDash actionDash = m_owner.actionSystem.GetAction<ActionDash>(eActionCommand.Defence);

        float length = m_owner.aniEvent.GetAniLength(eAnimation.BackDash);
        float slowTime = length * GameInfo.Instance.BattleConfig.EvadeSlowMotionAniRate;

        World.Instance.SetSlowTime(GameInfo.Instance.BattleConfig.EvadeSlowTimeScale, slowTime);
        World.Instance.InGameCamera.EnableMotionBlur(slowTime);

        eAnimation dashAni = eAnimation.BackDash;
        float dashSpeedRatio = actionDash.BackDashSpeedRatio;
        Vector3 dir = -m_owner.transform.forward;

        if (m_owner.Input && m_owner.Input.GetDirection() != Vector3.zero)
        {
            dashAni = eAnimation.Dash;
            dir = m_owner.Input.GetDirection();
            dashSpeedRatio = actionDash.DashSpeedRatio;
        }

        float addedDuration = mActionParamFromBO.battleOptionData.value * (float)eCOUNT.MAX_BO_FUNC_VALUE;
        ActionParamExtreamEvade actionParamExtreamEvade = new ActionParamExtreamEvade(dashAni, addedDuration, dir, dashSpeedRatio);

        SetNextAction(eActionCommand.ExtreamEvade, actionParamExtreamEvade);
        return slowTime;
    }
}
