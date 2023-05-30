
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionShizuruDash2Attack : ActionSelectSkillBase
{
    public bool SkipFirstAni { get; set; } = false;

    private float   mDist               = 12.0f;
    private float   mLookAtTargetAngle  = 180.0f;
    private Unit    mTarget             = null;
    private int     mExtraHitNum        = 0;
    private int     mCurHitCount        = 0;


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
        mExtraHitNum += (int)mValue1;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        ShowSkillNames(m_data);

        mCurHitCount = 0;

        if (!SkipFirstAni)
        {
            m_aniLength = m_owner.PlayAniImmediate(eAnimation.DashAttack2);
            m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(eAnimation.DashAttack2);
        }
        else
        {
            m_aniLength = m_owner.aniEvent.GetAniLength(eAnimation.DashAttack2);
            m_aniCutFrameLength = 0.0f;
        }

        if (FSaveData.Instance.AutoTargetingSkill)
        {
            mTarget = World.Instance.EnemyMgr.GetNearestTarget(m_owner, mDist, mLookAtTargetAngle);
            if (mTarget)
            {
                m_owner.LookAtTarget(mTarget.transform.position);
            }
        }
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime >= m_aniLength)
            {
                m_endUpdate = true;
            }

            yield return mWaitForFixedUpdate;
        }

        if (FSaveData.Instance.AutoTargetingSkill)
        {
            mTarget = World.Instance.EnemyMgr.GetNearestTarget(m_owner, mDist, mLookAtTargetAngle);
            if (mTarget)
            {
                m_owner.LookAtTarget(mTarget.transform.position);
            }
        }

        m_aniLength = m_owner.PlayAniImmediate(eAnimation.DashAttack2_2);
        yield return new WaitForSeconds(m_aniLength);
    }

    public override void OnUpdating(IActionBaseParam param)
    {
        if (mExtraHitNum <= 0 || m_endUpdate || m_checkTime <= m_aniCutFrameLength)
        {
            return;
        }

        m_aniLength = m_owner.PlayAniImmediate(eAnimation.DashAttack2_1);
        m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(eAnimation.DashAttack2_1);
        m_checkTime = 0.0f;

        if (FSaveData.Instance.AutoTargetingSkill)
        {
            mTarget = World.Instance.EnemyMgr.GetNearestTarget(m_owner, mDist, mLookAtTargetAngle);
            if (mTarget)
            {
                m_owner.LookAtTarget(mTarget.transform.position);
            }
        }
        else
        {
            m_owner.cmptRotate.UpdateRotation(m_owner.Input.GetDirection(), true);
        }

        ++mCurHitCount;
        if (mCurHitCount > mExtraHitNum)
        {
            m_endUpdate = true;
        }
    }

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(eAnimation.DashAttack2);
        if (evt == null)
        {
            Debug.LogError("DashAttack2 공격 이벤트가 없네??");
            return 0.0f;
        }
        else if (evt.visionRange <= 0.0f)
        {
            Debug.LogError("DashAttack2 Vistion Range가 0이네??");
        }

        return evt.visionRange;
    }

    public void AddExtraHit(int add)
    {
        mExtraHitNum += add;
    }
}
