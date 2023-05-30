
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionGateDie : ActionBase
{
    private EnemyGate mOwnerGate = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Die;

        mOwnerGate = m_owner as EnemyGate;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        if(mOwnerGate.curHp > 0.0f)
        {
            mOwnerGate.SetDie();
        }

        m_aniLength = m_owner.PlayAni(eAnimation.Die);
    }

    public override IEnumerator UpdateAction()
    {
        m_checkTime = 0.0f;
        while(m_checkTime < m_aniLength)
        {
            m_checkTime += Time.deltaTime;
            yield return null;
        }

        m_endUpdate = true;
    }
}
