
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionYukikazeEmergencyAttack : ActionSelectSkillBase
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.EmergencyAttack;

        conditionActionCommand = new eActionCommand[1];
        conditionActionCommand[0] = eActionCommand.Defence;

        if (mValue1 >= 1.0f)
        {
            List<AniEvent.sEvent> list = m_owner.aniEvent.GetAllMeleeAttackEvent(eAnimation.EvadeCounter);
            for (int i = 0; i < list.Count; i++)
            {
                list[i].behaviour = eBehaviour.KnockBackAttack;
            }
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        ShowSkillNames(m_data);

        m_aniLength = m_owner.PlayAniImmediate(eAnimation.EvadeCounter);

        if (FSaveData.Instance.AutoTargetingSkill)
        {
            Unit target = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
            if (target)
            {
                m_owner.LookAtTarget(target.transform.position);
            }
        }
        else
        {
            m_owner.cmptRotate.UpdateRotation(m_owner.Input.GetDirection(), true);
        }
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while(!m_endUpdate)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if(m_checkTime >= m_aniLength)
            {
                m_endUpdate = true;
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(eAnimation.EvadeCounter);
        if (evt == null)
        {
            Debug.LogError("EvadeCounter 공격 이벤트가 없네??");
            return 0.0f;
        }
        else if (evt.visionRange <= 0.0f)
        {
            Debug.LogError("EvadeCounter Vistion Range가 0이네??");
        }

        return evt.visionRange;
    }
}
