
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionMurasakiAirDownAttack : ActionAirDownAttack
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        if (mValue1 > 0.0f)
        {
            List<AniEvent.sEvent> list = m_owner.aniEvent.GetAllMeleeAttackEvent(eAnimation.GestureSkill03);
            for (int i = 0; i < list.Count; i++)
            {
                list[i].behaviour = eBehaviour.DownAttack;
            }
        }
    }
}
