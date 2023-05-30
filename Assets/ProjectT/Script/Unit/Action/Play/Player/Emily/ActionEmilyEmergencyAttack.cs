
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionEmilyEmergencyAttack : ActionSelectSkillBase
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.EmergencyAttack;

        conditionActionCommand = new eActionCommand[1];
        conditionActionCommand[0] = eActionCommand.Defence;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        ShowSkillNames(m_data);

        if (mValue1 > 0.0f)
        {
            m_aniLength = m_owner.PlayAniImmediate(eAnimation.CounterAttack2);
        }
        else
        {
            m_aniLength = m_owner.PlayAniImmediate(eAnimation.CounterAttack);
        }
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while(!m_endUpdate)
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
