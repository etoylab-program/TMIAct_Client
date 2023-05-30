
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionJingleiRushAttack : ActionSelectSkillBase
{
    private Unit        mTarget             = null;
    private eAnimation  mCurAni             = eAnimation.None;
    private float       mDist               = 12.0f;
    private float       mLookAtTargetAngle  = 180.0f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.RushAttack;

        conditionActionCommand = new eActionCommand[1];
        conditionActionCommand[0] = eActionCommand.Defence;

        extraCondition = new eActionCondition[1];
        extraCondition[0] = eActionCondition.Grounded;

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

        extraCancelCondition = new eActionCondition[3];
        extraCancelCondition[0] = eActionCondition.UseSkill;
        extraCancelCondition[1] = eActionCondition.UseQTE;
        extraCancelCondition[2] = eActionCondition.UseUSkill;

        superArmor = Unit.eSuperArmor.Lv1;

        mCurAni = eAnimation.RushAttack;
        if (mValue1 > 0)
        {
            mCurAni = eAnimation.RushAttack2;
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        ShowSkillNames(m_data);

        m_aniLength = m_owner.PlayAniImmediate(mCurAni);
        m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(mCurAni);

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
    }

    public override void OnCancel()
    {
        base.OnCancel();
        m_owner.StopStepForward();
    }

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( mCurAni );
        if (evt == null)
        {
            Debug.LogError(mCurAni.ToString() + "공격 이벤트가 없네??");
            return 0.0f;
        }
        else if (evt.visionRange <= 0.0f)
        {
            Debug.LogError(mCurAni.ToString() + "Vistion Range가 0이네??");
        }

        return evt.visionRange;
    }
}
