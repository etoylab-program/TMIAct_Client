
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionIngridAttackHoldAttack : ActionSelectSkillBase
{
    private float   mDist               = 12.0f;
    private float   mLookAtTargetAngle  = 180.0f;
    private Unit    mTarget             = null;
    private int     mExtraHitNum        = 0;
    private int     mCurHitCount        = 0;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        actionCommand = eActionCommand.AttackDuringAttack;
        IsRepeatSkill = true;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

        superArmor = Unit.eSuperArmor.Lv1;

        mExtraHitNum += (int)mValue1;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        ShowSkillNames(m_data);

        mCurHitCount = 0;

        m_aniLength = m_owner.PlayAniImmediate(eAnimation.AttackHold);
        m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(eAnimation.AttackHold);

        if (FSaveData.Instance.AutoTargetingSkill)
        {
            GetNewTarget();
        }
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            if (mTarget && mTarget.curHp <= 0.0f)
            {
                GetNewTarget();
            }

            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime >= m_aniLength)
            {
                m_endUpdate = true;
            }

            yield return mWaitForFixedUpdate;
        }

        if (FSaveData.Instance.AutoTargetingSkill)
        {
            GetNewTarget();
        }

        m_aniLength = m_owner.PlayAniImmediate(eAnimation.AttackHold_2);
        yield return new WaitForSeconds(m_aniLength);
    }

    public override void OnUpdating(IActionBaseParam param)
    {
        if (mExtraHitNum <= 0 || m_endUpdate || mCurHitCount > mExtraHitNum)
        {
            return;
        }

        if (m_checkTime <= m_aniCutFrameLength)
        {
            return;
        }

        ++mCurHitCount;
        if (mCurHitCount > mExtraHitNum)
        {
            m_endUpdate = true;
            return;
        }

        m_aniLength = m_owner.PlayAniImmediate(eAnimation.AttackHold_1);
        m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(eAnimation.AttackHold_1);
        m_checkTime = 0.0f;

        if (FSaveData.Instance.AutoTargetingSkill)
        {
            GetNewTarget();
        }
        else
        {
            m_owner.cmptRotate.UpdateRotation(m_owner.Input.GetDirection(), true);
        }
    }

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(eAnimation.AttackHold_1);
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

    private void GetNewTarget()
    {
        mTarget = World.Instance.EnemyMgr.GetNearestTarget(m_owner, mDist, mLookAtTargetAngle);
        if (mTarget)
        {
            m_owner.LookAtTarget(mTarget.transform.position);
        }
    }
}
