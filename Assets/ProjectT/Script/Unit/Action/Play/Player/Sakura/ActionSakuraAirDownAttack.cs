
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSakuraAirDownAttack : ActionAirDownAttack
{
    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        m_curAni = eAnimation.GestureSkill03;

        bool skipStartState = false;
        if (Physics.Raycast(m_owner.transform.position, -Vector3.up, out RaycastHit hitInfo, 0.5f, (1 << (int)eLayer.Floor) |
                                                                                                   (1 << (int)eLayer.Wall) |
                                                                                                   /*(1 << (int)eLayer.Wall_Inside) |*/
                                                                                                   (1 << (int)eLayer.EnvObject)))
        {
            skipStartState = true;
        }

        if(!skipStartState)
            m_state.ChangeState(eState.Start, true);
        else
            m_state.ChangeState(eState.Doing, true);
    }

    protected override eAnimation GetEndStateAni()
    {
        return m_curAni = eAnimation.GestureSkill03;
    }
}
