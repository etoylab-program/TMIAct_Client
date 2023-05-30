
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionNoahAttackHoldAttack : ActionSelectSkillBase
{
    private eAnimation  mCurAni         = eAnimation.AttackHold;
    private int         mAttackCount    = 0;
    private List<Unit>  mListTarget     = null;
    private Vector3     mBlackholePos   = Vector3.zero;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.AttackDuringAttack;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

        superArmor = Unit.eSuperArmor.Lv1;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        ShowSkillNames(m_data);
        m_aniLength = m_owner.PlayAniImmediate(mCurAni);

        if (mValue2 > 0.0f)
        {
            mListTarget = m_owner.GetEnemyList(true);
            for (int i = 0; i < mListTarget.Count; i++)
            {
                Unit target = mListTarget[i];
                if (target == null || target.CurrentSuperArmor >= Unit.eSuperArmor.Lv2 || target.IsImmuneFloat() || target.IsImmuneKnockback() || target.curHp <= 0.0f)
                {
                    continue;
                }

                target.actionSystem.CancelCurrentAction();
                target.StopStepForward();
            }

            mBlackholePos = m_owner.transform.position;
        }
    }

    public override IEnumerator UpdateAction()
    {
        mAttackCount = 1;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            if (mAttackCount >= mValue1)
            {
                m_endUpdate = true;
            }
            else
            {
                m_checkTime += m_owner.fixedDeltaTime;
                if (m_checkTime >= m_aniLength)
                {
                    m_checkTime = 0.0f;
                    m_aniLength = m_owner.PlayAniImmediate(mCurAni);

                    ++mAttackCount;
                }

                if (mValue2 > 0.0f)
                {
                    for (int i = 0; i < mListTarget.Count; i++)
                    {
                        Unit target = mListTarget[i];
                        if (target == null || target.MainCollider == null || target.cmptMovement == null)
                        {
                            continue;
                        }

                        if (target.CurrentSuperArmor >= Unit.eSuperArmor.Lv2 || target.IsImmuneFloat() || target.IsImmuneKnockback() || target.curHp <= 0.0f)
                        {
                            continue;
                        }

                        if(target.actionSystem && target.actionSystem.IsCurrentAction(eActionCommand.Appear))
                        {
                            continue;
                        }

                        target.StopStepForward();

                        Vector3 v = (mBlackholePos - target.transform.position).normalized;
                        v.y = 0.0f;

                        target.cmptMovement.UpdatePosition(v, Mathf.Max(GameInfo.Instance.BattleConfig.BlackholeMinSpeed, target.speed * 1.2f), false);
                    }
                }
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        BOCharSkill.EndBattleOption(BattleOption.eBOTimingType.DuringSkill, TableId);
    }

    public override void OnCancel()
    {
        base.OnCancel();
        BOCharSkill.EndBattleOption(BattleOption.eBOTimingType.DuringSkill, TableId);
    }

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( mCurAni );
        if (evt == null)
        {
            Debug.LogError(mCurAni.ToString() + "공격 이벤트가 없네??");
            return 0.0f;
        }
        else if (evt.visionRange <= 0.0f)
        {
            Debug.LogError(mCurAni.ToString() + "Vistion Range가 0이네??");
        }

        return evt.visionRange;
    }
}
