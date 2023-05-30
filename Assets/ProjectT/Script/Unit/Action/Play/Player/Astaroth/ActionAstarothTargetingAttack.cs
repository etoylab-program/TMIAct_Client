
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionAstarothTargetingAttack : ActionTargetingAttack
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        AttackAnimations = new eAnimation[3];
        AttackAnimations[0] = eAnimation.Attack04;
        AttackAnimations[1] = eAnimation.Attack05;
        AttackAnimations[2] = eAnimation.Attack06;

        for (int i = 0; i < AttackAnimations.Length; i++)
        {
            List<AniEvent.sEvent> listAtkEvt = m_owner.aniEvent.GetAllAttackEvent(AttackAnimations[i]);
            for (int j = 0; j < listAtkEvt.Count; j++)
            {
                listAtkEvt[j].atkRatio += listAtkEvt[j].atkRatio * (mValue1 / (float)eCOUNT.MAX_BO_FUNC_VALUE);
            }
        }
    }
}
