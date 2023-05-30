
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionKurenaiCounterAttack : ActionCounterAttack
{
    [Header("[Property]")]
    public float EndTime = 3.0f;

    private eAnimation  mCurAni     = eAnimation.CounterAttack;
    private float       mCurEndTime = 0.0f;
    private bool        mAttack     = false;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.HoldingDefBtnAttack;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

        superArmor = Unit.eSuperArmor.Lv2;

        if (mValue1 > 0.0f)
        {
            List<AniEvent.sEvent> listAtkEvt = m_owner.aniEvent.GetAllAttackEvent(eAnimation.CounterAttack);
            for (int j = 0; j < listAtkEvt.Count; j++)
            {
                listAtkEvt[j].behaviour = eBehaviour.GroggyAttack;
            }
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        //BOCharSkill.ChangeBattleOptionDuration(BattleOption.eBOTimingType.DuringSkill, TableId, EndTime);

        base.OnStart(param);
        m_owner.PlayAniImmediate(eAnimation.Counter);

        mCurEndTime = EndTime;
        mAttack = false;

        ShowSkillNames(m_data);
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while(!m_endUpdate)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime >= mCurEndTime)
            {
                m_endUpdate = true;
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnUpdating(IActionBaseParam param)
    {
        if(mAttack)
        {
            return;
        }

        mAttack = true;
        m_checkTime = 0.0f;

        if (FSaveData.Instance.AutoTargetingSkill)
        {
            Unit target = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
            if (target)
            {
                m_owner.LookAtTarget(target.transform.position);
            }
        }
        else
        {
            m_owner.cmptRotate.UpdateRotation(m_owner.Input.GetDirection(), true);
        }

        mCurEndTime = m_owner.PlayAniImmediate(eAnimation.CounterAttack);
        //BOCharSkill.ChangeBattleOptionDuration(BattleOption.eBOTimingType.DuringSkill, TableId, mCurEndTime);
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
            return 0.0f;
        }

        return evt.visionRange;
    }
}
