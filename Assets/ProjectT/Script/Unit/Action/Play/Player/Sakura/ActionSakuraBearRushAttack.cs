
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSakuraBearRushAttack : ActionSelectSkillBase
{
    [Header("Property")]
    public float dist = 12.0f;
    public float lookAtTargetAngle = 180.0f;

    protected Unit m_target = null;
    protected eAnimation m_curAni = eAnimation.BearRushAttack;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.RushAttack;

        superArmor = Unit.eSuperArmor.Lv1;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        ShowSkillNames(m_data);

        if(mValue1 <= 0.0f)
            m_curAni = eAnimation.BearRushAttack;
        else
            m_curAni = eAnimation.BearRushAttack2;

        m_aniLength = m_owner.PlayAniImmediate(m_curAni);

        if (FSaveData.Instance.AutoTargetingSkill)
        {
            m_target = World.Instance.EnemyMgr.GetNearestTarget(m_owner, dist, lookAtTargetAngle);
            if (m_target)
                m_owner.LookAtTarget(m_target.transform.position);
        }
        else
        {
            m_owner.cmptRotate.UpdateRotation(m_owner.Input.GetDirection(), true);
        }
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if(m_checkTime >= m_aniLength)
                m_endUpdate = true;

            yield return mWaitForFixedUpdate;
        }
    }

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(m_curAni);
        if (evt == null)
        {
            Debug.LogError(m_curAni.ToString() + "공격 이벤트가 없네??");
            return 0.0f;
        }
        else if (evt.visionRange <= 0.0f)
        {
            Debug.LogError(m_curAni.ToString() + "Vistion Range가 0이네??");
        }

        return evt.visionRange;
    }
}
