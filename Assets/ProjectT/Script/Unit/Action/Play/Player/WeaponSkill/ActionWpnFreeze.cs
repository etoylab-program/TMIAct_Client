
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionWpnFreeze : ActionWeaponSkillBase
{
    private ActionParamFromBO   mParamFromBO    = null;
    private bool                mbCast          = false;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.WpnFreeze;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        superArmor = Unit.eSuperArmor.Invincible;
        mCurAni = eAnimation.CastingBuff;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        mParamFromBO = param as ActionParamFromBO;

        m_aniLength = m_owner.PlayAni(mCurAni);
        m_aniCutFrameLength = m_owner.aniEvent.GetCurCutFrameLength();

        mbCast = false;

        UnitCollider targetCollider = m_owner.GetMainTargetCollider(true);
        if (targetCollider)
        {
            m_owner.LookAtTarget(targetCollider.GetCenterPos());
            m_owner.SetMainTarget(targetCollider.Owner);
        }
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            m_checkTime += m_owner.fixedDeltaTime;

            if (m_checkTime >= m_aniLength)
            {
                m_endUpdate = true;
            }
            else if(!mbCast && m_checkTime >= m_aniCutFrameLength)
            {
                mbCast = true;
                EffectManager.Instance.Play(m_owner, mParamFromBO.battleOptionData.effId1, (EffectManager.eType)mParamFromBO.battleOptionData.effType);

                if (mParamFromBO.battleOptionData.addCallTiming == BattleOption.eBOAddCallTiming.OnSend)
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

                Invoke("Freeze", 0.2f);
            }

            yield return mWaitForFixedUpdate;
        }
    }

    private void Freeze()
    {
        mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
        mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.Common;
        mBuffEvt.Set(TableId, eEventSubject.ActiveEnemies, eEventType.EVENT_DEBUFF_FREEZE, m_owner, 0.0f, 0.0f, 0.0f,
                     mParamFromBO.battleOptionData.duration, 0.0f, 0, 0, eBuffIconType.Debuff_Freeze);

        EventMgr.Instance.SendEvent(mBuffEvt);
    }
}
