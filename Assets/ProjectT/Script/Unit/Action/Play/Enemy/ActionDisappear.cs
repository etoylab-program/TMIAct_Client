
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class ActionDisappear : ActionEnemyBase
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Disappear;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        Enemy enemy = m_owner as Enemy;
        enemy.StopBT();
    }

    public override IEnumerator UpdateAction()
    {
        m_owner.OnDie();
        EventMgr.Instance.SendEvent(eEventSubject.World, eEventType.EVENT_GAME_PASS_PORTAL, m_owner);

        yield return null;
    }

    public override void OnEnd()
    {
        isPlaying = false;
        m_curCancelDuringSameActionCount = 0;
    }
}
