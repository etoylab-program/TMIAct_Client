﻿
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ActionRinComboAttack : ActionComboAttack
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        attackAnimations = new eAnimation[5];
        attackAnimations[0] = eAnimation.Attack06;
        attackAnimations[1] = eAnimation.Attack07;
        attackAnimations[2] = eAnimation.Attack08;
        attackAnimations[3] = eAnimation.Attack09;
		attackAnimations[4] = eAnimation.Attack10;

		mOriginalAtkAnis = new eAnimation[attackAnimations.Length];
        for (int i = 0; i < attackAnimations.Length; i++)
        {
            mOriginalAtkAnis[i] = attackAnimations[i];
        }

        for (int i = 0; i < attackAnimations.Length; i++)
        {
            List<AniEvent.sEvent> listAtkEvt = m_owner.aniEvent.GetAllAttackEvent(attackAnimations[i]);
            for (int j = 0; j < listAtkEvt.Count; j++)
                listAtkEvt[j].atkRatio += listAtkEvt[j].atkRatio * (mValue1 / (float)eCOUNT.MAX_BO_FUNC_VALUE);
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

    protected override eAnimation GetCurAni()
    {
        return base.GetCurAni();
    }
}
