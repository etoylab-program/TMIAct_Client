
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionWpnSwimSuitSword : ActionWeaponSkillBase
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.WpnSwimSuitSword;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        superArmor = Unit.eSuperArmor.Invincible;
        mCurAni = eAnimation.SwimSuitSword;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        ActionParamFromBO paramFromBO = param as ActionParamFromBO;

        List<AniEvent.sEvent> listAtkEvent = m_owner.aniEvent.GetAllAttackEvent(mCurAni);
        for (int i = 0; i < listAtkEvent.Count; i++)
            listAtkEvent[i].atkRatio *= paramFromBO.battleOptionData.value;

        UnitCollider targetCollider = m_owner.GetMainTargetCollider(true);
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
}
