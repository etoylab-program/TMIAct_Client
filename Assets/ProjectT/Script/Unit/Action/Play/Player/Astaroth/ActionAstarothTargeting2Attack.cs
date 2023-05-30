
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionAstarothTargeting2Attack : ActionTargetingAttack
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        AttackAnimations = new eAnimation[3];
        AttackAnimations[0] = eAnimation.Attack07;
        AttackAnimations[1] = eAnimation.Attack08;
        AttackAnimations[2] = eAnimation.Attack09;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        m_owner.SetSpeedRate(mValue1 / (float)eCOUNT.MAX_BO_FUNC_VALUE);
    }

    public override void OnCancel()
    {
        base.OnCancel();
        m_owner.SetSpeedRate(0.0f);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        m_owner.SetSpeedRate(0.0f);
    }
}
