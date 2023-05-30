
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionTokikoComboAttack : ActionComboAttack
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        attackAnimations = new eAnimation[3];
        attackAnimations[0] = eAnimation.Attack05;
        attackAnimations[1] = eAnimation.Attack06;
        attackAnimations[2] = eAnimation.Attack07;

        mOriginalAtkAnis = new eAnimation[attackAnimations.Length];
        for (int i = 0; i < attackAnimations.Length; i++)
        {
            mOriginalAtkAnis[i] = attackAnimations[i];
        }
    }

    public override void OnEnd()
    {
        isPlaying = false;

        if (mListBehaviour.Count > 0 && m_owner.lastAttackKnockBack && IsLastAttack())
        {
            List<AniEvent.sEvent> listAtkEvt = m_owner.aniEvent.GetAllAttackEvent(attackAnimations[CurrentAttackIndex]);
            for (int i = 0; i < listAtkEvt.Count; i++)
            {
                listAtkEvt[i].behaviour = mListBehaviour[i];
            }
        }

        if (m_nextAttackIndex == CurrentAttackIndex)
        {
            if (mOwnerPlayer.CancelComboAttack)
            {
                ResetComboAttackIndex();
            }
            else
            {
                ContinueComboAttackIndex();
            }

            m_owner.SetSpeedRateByBuff(0.0f);
            base.OnEnd();
        }
        else
        {
            CurrentAttackIndex = m_nextAttackIndex;
            m_nextAction = eActionCommand.Attack01;
        }

        m_reserved = false;
        m_owner.RestoreSuperArmor(mChangedSuperArmorId);
    }

    public override void OnCancel()
    {
        base.OnCancel();
        m_owner.SetSpeedRateByBuff(0.0f);
    }

    protected override void StartAttack()
    {
        base.StartAttack();

        if (CurrentAttackIndex == 0)
        {
            m_owner.SetSpeedRateByBuff(mValue1 / (float)eCOUNT.MAX_BO_FUNC_VALUE);
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
