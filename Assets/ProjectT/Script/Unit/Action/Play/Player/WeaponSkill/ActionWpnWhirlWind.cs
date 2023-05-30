
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionWpnWhirlWind : ActionWeaponSkillBase
{
    private ActionParamFromBO mParamFromBO = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.WpnWhirlWind;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        //superArmor = Unit.eSuperArmor.Invincible;
        mCurAni = eAnimation.Whirlwind;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        mParamFromBO = param as ActionParamFromBO;
        m_owner.TemporaryInvincible = true;

        ActionParamFromBO paramFromBO = param as ActionParamFromBO;

        List<AniEvent.sEvent> listAtkEvent = m_owner.aniEvent.GetAllAttackEvent(mCurAni);
        for (int i = 0; i < listAtkEvent.Count; i++)
        {
            listAtkEvent[i].atkRatio *= paramFromBO.battleOptionData.value;
        }

        UnitCollider targetCollider = m_owner.GetMainTargetCollider(false);
        if (targetCollider)
        {
            LookAtTarget(targetCollider.Owner);
        }

        m_aniLength = m_owner.PlayAniImmediate(mCurAni);
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime >= m_aniLength)
            {
                m_endUpdate = true;
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnCancel()
    {
        base.OnCancel();
        m_owner.TemporaryInvincible = false;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        m_owner.TemporaryInvincible = false;

        if (mParamFromBO.battleOptionData.addCallTiming == BattleOption.eBOAddCallTiming.OnEnd)
        {
            if (mParamFromBO.battleOptionData.timingType == BattleOption.eBOTimingType.Use &&
                mParamFromBO.battleOptionData.dataOnEndCall.conditionType == BattleOption.eBOConditionType.ComboCountAsValue)
            {
                mParamFromBO.battleOptionData.dataOnEndCall.evt.value = mParamFromBO.battleOptionData.evt.value;
            }

            EffectManager.Instance.Play(m_owner, mParamFromBO.battleOptionData.dataOnEndCall.startEffId, (EffectManager.eType)mParamFromBO.battleOptionData.dataOnEndCall.effType);

            mParamFromBO.battleOptionData.dataOnEndCall.useTime = System.DateTime.Now;
            EventMgr.Instance.SendEvent(mParamFromBO.battleOptionData.dataOnEndCall.evt);

            Log.Show(mParamFromBO.battleOptionData.dataOnEndCall.evt.battleOptionData.battleOptionSetId + "번 배틀옵션셋 사용 (애드콜)!!!", Log.ColorType.Green);
        }
    }
}
