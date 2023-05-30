
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionWpnBlackDragonSword : ActionWeaponSkillBase
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.WpnBlackDragonSword;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        superArmor = Unit.eSuperArmor.Invincible;
        mCurAni = eAnimation.BlackDragonSword;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        ActionParamFromBO paramFromBO = param as ActionParamFromBO;
        mBuffStatsInfo = mOwnerPlayer.unitBuffStats.GetBuffStat(paramFromBO.battleOptionData.referenceBattleOptionSetId);

        List<AniEvent.sEvent> listAtkEvent = m_owner.aniEvent.GetAllAttackEvent(mCurAni);
        for (int i = 0; i < listAtkEvent.Count; i++)
        {
            listAtkEvent[i].atkRatio *= paramFromBO.battleOptionData.value;
            if (mBuffStatsInfo != null)
            {
                listAtkEvent[i].atkRatio += (listAtkEvent[i].atkRatio * mBuffStatsInfo.stackCount * paramFromBO.battleOptionData.value2);

                mOwnerPlayer.unitBuffStats.RemoveBuffStat(mBuffStatsInfo.id);
                mOwnerPlayer.cmptBuffDebuff.RemoveExecute(mBuffStatsInfo.id);
            }
        }

        m_aniLength = m_owner.PlayAniImmediate(mCurAni);

        UnitCollider targetCollider = m_owner.GetMainTargetCollider(true);
        if (targetCollider)
        {
            LookAtTarget(targetCollider.Owner);
        }
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime >= m_aniLength)
                m_endUpdate = true;

            yield return mWaitForFixedUpdate;
        }
    }
}
