
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionKurenaiAirDownAttack : ActionAirDownAttack
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

        if (mValue1 > 0.0f)
        {
            AniEvent.sEvent lastAtkEvt = m_owner.aniEvent.GetLastAttackEvent(eAnimation.GestureSkill03);
            if(lastAtkEvt != null)
            {
                lastAtkEvt.behaviour = eBehaviour.KnockBackAttack;
            }
        }
    }
}
