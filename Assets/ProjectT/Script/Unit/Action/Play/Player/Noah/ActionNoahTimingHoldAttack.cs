
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionNoahTimingHoldAttack : ActionSelectSkillBase
{
    private UnitCollider mTargetCollider = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.TimingHoldAttack;

        extraCondition = new eActionCondition[1];
        extraCondition[0] = eActionCondition.Grounded;

        superArmor = Unit.eSuperArmor.Lv1;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        ShowSkillNames(m_data);

        LookAtTarget();
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.EvadeTiming);
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
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(eAnimation.EvadeTiming);
        if (evt == null)
        {
            Debug.LogError(eAnimation.EvadeTiming.ToString() + "공격 이벤트가 없네??");
            return 0.0f;
        }
        else if (evt.visionRange <= 0.0f)
        {
            Debug.LogError(eAnimation.EvadeTiming.ToString() + "Vistion Range가 0이네??");
        }

        return evt.visionRange;
    }

    private void LookAtTarget()
    {
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
}
