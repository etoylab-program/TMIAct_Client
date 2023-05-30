
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionBackDash : ActionEnemyBase
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.BackDash;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.BackDash);
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

            yield return mWaitForFixedUpdate;
        }
    }
}
