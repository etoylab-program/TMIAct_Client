
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionWait : ActionEnemyBase
{
    private float m_duration = 0.0f;
    private ActionParamAI m_paramAI = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Wait;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        m_paramAI = param as ActionParamAI;
        m_duration = Utility.GetRandom(m_paramAI.minValue, m_paramAI.maxValue, 10.0f);

        m_owner.PlayAni(eAnimation.Idle01);
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime >= m_duration)
                m_endUpdate = true;

            yield return mWaitForFixedUpdate;
        }
    }
}
