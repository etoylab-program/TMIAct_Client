
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionMurasakiCounterAttack : ActionCounterAttack
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

        superArmor = Unit.eSuperArmor.Lv2;

        /*/ 반격 범위 증가
        if(mValue1 > 0.0f)
        {
            List<AniEvent.sEvent> listAtkEvt = m_owner.aniEvent.GetAllAttackEvent(eAnimation.CounterAttack);
            for (int j = 0; j < listAtkEvt.Count; j++)
            {
                listAtkEvt[j].atkRange += (listAtkEvt[j].atkRange * mValue1);
            }
        }*/

        // 반격 데미지 증가
        if (mValue3 > 0.0f)
        {
            List<AniEvent.sEvent> listAtkEvt = m_owner.aniEvent.GetAllAttackEvent(eAnimation.CounterAttack);
            for (int j = 0; j < listAtkEvt.Count; j++)
            {
                listAtkEvt[j].atkRatio += (listAtkEvt[j].atkRatio * mValue3);
            }
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        // 받는 데미지 감소 증가
        if (mValue2 > 0.0f)
        {
            BOCharSkill.ChangeBattleOptionValue1(m_data.CharAddBOSetID2, TableId, mValue2);
        }

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

        if (mValue1 > 0.0f)
        {
            mCurAni = eAnimation.CounterAttack2;
            mCurEndTime = m_owner.PlayAniImmediate(eAnimation.CounterAttack2);
        }
        else
        {
            mCurAni = eAnimation.CounterAttack;
            mCurEndTime = m_owner.PlayAniImmediate(eAnimation.CounterAttack);
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();

        ForceQuitBuffDebuff = true;
        BOCharSkill.EndBattleOption(BattleOption.eBOTimingType.DuringSkill, TableId);
    }

	public override void OnCancel() {
		base.OnCancel();
        ForceQuitBuffDebuff = true;
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
