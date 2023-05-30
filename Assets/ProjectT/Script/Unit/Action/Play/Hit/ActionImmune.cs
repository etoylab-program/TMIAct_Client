
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionImmune : ActionBase
{
    private eAnimation m_curAni = eAnimation.None;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Immune;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        ActionParamHit paramHit = param as ActionParamHit;
        if(paramHit.attackerBehaviour == eBehaviour.UpperAttack)
            m_curAni = eAnimation.ImmuneFloat;
        else if (paramHit.attackerBehaviour == eBehaviour.DownAttack)
            m_curAni = eAnimation.ImmuneDown;
        else if (paramHit.attackerBehaviour == eBehaviour.KnockBackAttack)
            m_curAni = eAnimation.ImmuneKnockBack;

        if (!m_owner.aniEvent.HasAni(m_curAni))
            m_endUpdate = true;
        else
            m_owner.PlayAniImmediate(m_curAni);
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            if (m_owner.aniEvent.IsAniPlaying(m_curAni) == eAniPlayingState.End)
                m_endUpdate = true;

            yield return mWaitForFixedUpdate;
        }
    }
}
