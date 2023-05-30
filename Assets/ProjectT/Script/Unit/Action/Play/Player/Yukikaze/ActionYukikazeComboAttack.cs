
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionYukikazeComboAttack : ActionComboAttack
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        IsRepeatSkill = true;

        attackAnimations = new eAnimation[4];
        attackAnimations[0] = eAnimation.Attack01;
        attackAnimations[1] = eAnimation.Attack02;
        attackAnimations[2] = eAnimation.Attack03;
        attackAnimations[3] = eAnimation.Attack04;

        mOriginalAtkAnis = new eAnimation[attackAnimations.Length];
        for (int i = 0; i < attackAnimations.Length; i++)
        {
            mOriginalAtkAnis[i] = attackAnimations[i];
        }
    }

    public override IEnumerator UpdateAction()
    {
        ActionYukikazeGroundSkillCombo groundSkillCombo = m_owner.actionSystem.GetAction<ActionYukikazeGroundSkillCombo>(eActionCommand.GroundSkillCombo);

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            m_checkTime += m_owner.fixedDeltaTime;

            if (m_nextAttackIndex > CurrentAttackIndex)
            {
                if (m_checkTime >= m_owner.aniEvent.GetCutFrameLength(m_currentAni))
                {
                    m_endUpdate = true;
                }
            }
            else
            {
                if (m_owner.AI && groundSkillCombo && IsAfterLastAttackCutFrame())
                {
                    SetNextAction(eActionCommand.GroundSkillCombo, null);
                    m_endUpdate = true;
                }
                else if (m_checkTime >= m_owner.aniEvent.GetAniLength(m_currentAni))
                {
                    m_endUpdate = true;
                }
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        m_owner.unitBuffStats.RemoveBuffStat(m_data.ID * 10);
    }

    public override void OnCancel()
    {
        base.OnCancel();
        m_owner.unitBuffStats.RemoveBuffStat(m_data.ID * 10);
    }

    public override void RestoreAttackAnimations()
    {
        attackAnimations = new eAnimation[mOriginalAtkAnis.Length];
        for (int i = 0; i < mOriginalAtkAnis.Length; i++)
        {
            attackAnimations[i] = mOriginalAtkAnis[i];
        }
    }

    protected override void StartAttack()
    {
        base.StartAttack();

        /*
        if (!m_noneTarget && mTargetCollider && !mTargetCollider.Owner.isVisible)
        {
            World.Instance.InGameCamera.TurnToTarget(mTargetCollider.GetCenterPos(), 2.0f);
        }
        */

        float dmgDownRatio = mValue1 / (float)eCOUNT.MAX_BO_FUNC_VALUE;
        m_owner.unitBuffStats.AddBuffStat(eBuffDebuffType.Buff, m_data.ID * 10, UnitBuffStats.eBuffStatType.Damage, UnitBuffStats.eIncreaseType.Decrease, 
                                          dmgDownRatio, -1, BattleOption.eToExecuteType.Unit, BattleOption.eBOConditionType.None, BattleOption.eBOConditionType.None,
                                          BattleOption.eBOAtkConditionType.None, TableId, false);
    }

    protected override eAnimation GetCurAni()
    {
        //if (m_currentAttackIndex == 0)
        //    ShowSkillNames(m_data);

        return base.GetCurAni();
    }

    public bool IsAfterLastAttackCutFrame()
    {
        if (!IsLastAttack())
            return false;

        if (m_checkTime < m_aniCutFrameLength)
            return false;

        return true;
    }
}
