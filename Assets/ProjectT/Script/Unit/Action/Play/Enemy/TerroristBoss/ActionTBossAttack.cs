
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionTBossAttack : ActionEnemyAttackBase
{
    [Header("Property")]
    public eAnimation ani;
    public eActionCommand actionCmd;

    private Transform m_ownerParentTransform = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = actionCmd;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        m_aniLength = m_owner.PlayAni(ani);
        m_checkTime = 0.0f;

        ((EnemyParts)m_owner).ActiveConstraints(false);

        if (m_owner.parent)
        {
            m_ownerParentTransform = m_owner.transform.parent;
            m_owner.transform.parent = null;
        }
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime >= m_aniLength)
                m_endUpdate = true;

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();

        ((EnemyParts)m_owner).ActiveConstraints(true);

        if(m_owner.parent)
            m_owner.transform.parent = m_ownerParentTransform;
    }

    public override void OnCancel()
    {
        if(m_owner.aniEvent)
        {
            m_owner.aniEvent.StopEffects();
        }

        base.OnCancel();
    }
}
