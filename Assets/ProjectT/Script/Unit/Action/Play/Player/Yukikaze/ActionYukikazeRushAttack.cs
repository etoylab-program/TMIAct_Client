
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionYukikazeRushAttack : ActionSelectSkillBase
{
    [Header("Property")]
    public float dist = 15.0f;
    public float lookAtTargetAngle = 180.0f;

    private eAnimation  mCurAni = eAnimation.RushAttack;
    private Unit        mTarget = null;


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

        if (mValue1 < 2.0f)
        {
            mCurAni = eAnimation.RushAttack;

            if (mValue1 >= 1.0f)
            {
                List<AniEvent.sEvent> list = m_owner.aniEvent.GetAllMeleeAttackEvent(mCurAni);
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].behaviour = eBehaviour.GroggyAttack;
                }
            }
        }
        else
        {
            mCurAni = eAnimation.RushAttack2;
        }

        m_aniLength = m_owner.PlayAniImmediate(mCurAni);
        m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(mCurAni);

        if (FSaveData.Instance.AutoTargetingSkill)
        {
            mTarget = World.Instance.EnemyMgr.GetNearestTarget(m_owner, dist, lookAtTargetAngle);
            if (mTarget)
                m_owner.LookAtTarget(mTarget.transform.position);
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
            if (m_checkTime > m_aniLength)
                m_endUpdate = true;

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
            return 0.0f;
        }

        return evt.visionRange;
    }
}
