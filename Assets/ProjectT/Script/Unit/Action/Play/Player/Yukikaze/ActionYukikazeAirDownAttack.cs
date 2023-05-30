
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionYukikazeAirDownAttack : ActionAirDownAttack
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        m_curAni = eAnimation.GestureSkill03_1;
    }

    protected override eAnimation GetEndStateAni()
    {
        if (mValue2 > 0.0f)
        {
            return m_curAni = eAnimation.GestureSkill03_2;
        }
        else
        {
            return m_curAni = eAnimation.GestureSkill03_1;
        }
    }
}
