
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionWpnCasting : ActionWeaponSkillBase
{
    private ActionParamFromBO   mParamFromBO    = null;
    private bool                mbCast          = false;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.WpnCasting;

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

        List<AniEvent.sEvent> listAtkEvent = m_owner.aniEvent.GetAllAttackEvent(mCurAni);
        if ( listAtkEvent != null ) {
			for ( int i = 0; i < listAtkEvent.Count; i++ ) {
				listAtkEvent[i].atkRatio *= mParamFromBO.battleOptionData.value;
			}
		}

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
            }

            yield return mWaitForFixedUpdate;
        }
    }
}
