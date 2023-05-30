
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionKiraraComboAttack : ActionComboAttack
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        attackAnimations = new eAnimation[4];
        attackAnimations[0] = eAnimation.Attack01;
        attackAnimations[1] = eAnimation.Attack02;
        attackAnimations[2] = eAnimation.Attack03;
        attackAnimations[3] = eAnimation.Attack04_1;

        mOriginalAtkAnis = new eAnimation[attackAnimations.Length];
        for (int i = 0; i < attackAnimations.Length; i++)
        {
            mOriginalAtkAnis[i] = attackAnimations[i];
        }

        for (int i = 0; i < attackAnimations.Length; i++)
        {
            List<AniEvent.sEvent> listAtkEvt = m_owner.aniEvent.GetAllAttackEvent(attackAnimations[i]);
            for (int j = 0; j < listAtkEvt.Count; j++)
            {
                listAtkEvt[j].atkRatio += listAtkEvt[j].atkRatio * (mValue1 / (float)eCOUNT.MAX_BO_FUNC_VALUE);
            }
        }
    }

    public override void RestoreAttackAnimations()
    {
        attackAnimations = new eAnimation[mOriginalAtkAnis.Length];
        for (int i = 0; i < mOriginalAtkAnis.Length; i++)
        {
            attackAnimations[i] = mOriginalAtkAnis[i];
        }
    }
}
