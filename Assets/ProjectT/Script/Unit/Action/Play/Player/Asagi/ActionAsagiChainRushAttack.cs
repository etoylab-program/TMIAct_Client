
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionAsagiChainRushAttack : ActionSelectSkillBase
{
    [Header("Property")]
    public float dist = 12.0f;
    public float lookAtTargetAngle = 180.0f;

    public bool SkipFirstAni { get; set; } = false;

    protected Unit m_target = null;

    private int m_extraHitNum = 0;
    private int m_curHitCount = 0;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        
        actionCommand = eActionCommand.RushAttack;
        IsRepeatSkill = true;

        conditionActionCommand = new eActionCommand[1];
        conditionActionCommand[0] = eActionCommand.Defence;

        extraCondition = new eActionCondition[1];
        extraCondition[0] = eActionCondition.Grounded;

        extraCancelCondition = new eActionCondition[3];
        extraCancelCondition[0] = eActionCondition.UseSkill;
        extraCancelCondition[1] = eActionCondition.UseQTE;
        extraCancelCondition[2] = eActionCondition.UseUSkill;

        superArmor = Unit.eSuperArmor.Lv1;

        List<AniEvent.sEvent> listAtkEvt = m_owner.aniEvent.GetAllAttackEvent(eAnimation.RushAttack);
        for (int j = 0; j < listAtkEvt.Count; j++)
            listAtkEvt[j].atkRange += listAtkEvt[j].atkRange * (mValue1 / (float)eCOUNT.MAX_BO_FUNC_VALUE);

        m_extraHitNum += (int)mValue1;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        ShowSkillNames(m_data);
        
        m_curHitCount = 0;

        if (!SkipFirstAni)
        {
            m_aniLength = m_owner.PlayAniImmediate(eAnimation.RushAttack);
            m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(eAnimation.RushAttack);
        }
        else
        {
            m_aniLength = m_owner.aniEvent.GetAniLength(eAnimation.RushAttack);
            m_aniCutFrameLength = 0.0f;

            List<AniEvent.sEvent> list = m_owner.aniEvent.GetAllAttackEvent(eAnimation.RushAttackRepeatFinish);
            for (int i = 0; i < list.Count; i++)
            {
                list[i].behaviour = eBehaviour.KnockBackAttack;
            }
        }

        if (FSaveData.Instance.AutoTargetingSkill)
        {
            m_target = World.Instance.EnemyMgr.GetNearestTarget(m_owner, dist, lookAtTargetAngle);
            if (m_target)
                m_owner.LookAtTarget(m_target.transform.position);
        }
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime >= m_aniLength)
                m_endUpdate = true;

            yield return mWaitForFixedUpdate;
        }

        if (FSaveData.Instance.AutoTargetingSkill)
        {
            m_target = World.Instance.EnemyMgr.GetNearestTarget(m_owner, dist, lookAtTargetAngle);
            if (m_target)
                m_owner.LookAtTarget(m_target.transform.position);
        }

        m_aniLength = m_owner.PlayAniImmediate(eAnimation.RushAttackRepeatFinish);
        yield return new WaitForSeconds(m_aniLength);
    }

    public override void OnUpdating(IActionBaseParam param)
    {
        if (m_extraHitNum <= 0 || m_endUpdate)
            return;

        if (m_checkTime <= m_aniCutFrameLength)
            return;

        m_aniLength = m_owner.PlayAniImmediate(eAnimation.RushAttackRepeat);
        Debug.Log("eAnimation.RushAttackRepeat!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(eAnimation.RushAttackRepeat);
        m_checkTime = 0.0f;

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

        ++m_curHitCount;
        if (m_curHitCount >= m_extraHitNum)
            m_endUpdate = true;
    }

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(eAnimation.RushAttack);
        if (evt == null)
        {
            Debug.LogError("RushAttack 공격 이벤트가 없네??");
            return 0.0f;
        }
        else if (evt.visionRange <= 0.0f)
        {
            Debug.LogError("RushAttack Vistion Range가 0이네??");
        }

        return evt.visionRange;
    }

    public void AddExtraHit(int add)
    {
        m_extraHitNum += add;
    }
}
