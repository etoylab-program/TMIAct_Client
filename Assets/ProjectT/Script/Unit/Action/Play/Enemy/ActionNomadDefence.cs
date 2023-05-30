
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionNomadDefence : ActionEnemyCounterAttackBase
{
    [Header("Property")]
    public eAnimation       aniDefence;
    public eAnimation       aniAttack;
    public eActionCommand   actionCmd;
    public float            defenceTime = 8.0f;

    private bool mbCounter = false;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = actionCmd;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        m_owner.PlayAni(aniDefence);
        m_checkTime = 0.0f;
        mbCounter = false;

        UnitCollider targetCollider = m_owner.GetMainTargetCollider(true);
        LookAtTarget(targetCollider.Owner);
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime >= defenceTime)
            {
                m_endUpdate = true;
            }

            yield return mWaitForFixedUpdate;
        }

        if (mbCounter)
        {
            m_aniLength = m_owner.PlayAni(aniAttack);
            yield return new WaitForSeconds(m_aniLength);
        }
    }

    public override void OnUpdating(IActionBaseParam param)
    {
        m_checkTime = defenceTime;
        mbCounter = true;
    }
}
