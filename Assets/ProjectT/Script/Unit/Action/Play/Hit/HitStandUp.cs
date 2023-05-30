
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class HitStandUp : ActionHitBase
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.StandUp;
    }

    public override void OnStart(IActionBaseParam param)
    {
        ActionParamHit hitParam = param as ActionParamHit;
        if(hitParam != null)
            hitParam.skip = true;

        base.OnStart(param);
    }

    public override IEnumerator UpdateAction()
    {
        //yield return new WaitForSeconds(2.0f);
        m_aniLength = m_owner.PlayAni(eAnimation.StandUp);

        while (m_endUpdate == false)
        {
            m_checkTime += m_owner.deltaTime;
            if (m_checkTime >= m_aniLength)
                m_endUpdate = true;

            yield return null;
        }
    }

    public override void OnUpdating(IActionBaseParam param)
    {
        OnCancel();
        m_endUpdate = true;
    }

    public override void OnEnd()
    {
        base.OnEnd();
    }
}
