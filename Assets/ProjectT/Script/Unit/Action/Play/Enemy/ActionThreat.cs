
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionThreat : ActionEnemyBase
{
    private float m_duration = 0.0f;
    private ActionParamAI m_paramAI = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Threat;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        UnitCollider targetCollider = m_owner.GetMainTargetCollider(true);
        if(targetCollider)
        {
            LookAtTarget(targetCollider.Owner);
            m_owner.PlayAni(eAnimation.Threat);
        }
        else
        {
            m_endUpdate = true;
        }
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            if (m_owner.aniEvent.IsAniPlaying(eAnimation.Threat) == eAniPlayingState.End)
                m_endUpdate = true;

            yield return mWaitForFixedUpdate;
        }
    }
}
