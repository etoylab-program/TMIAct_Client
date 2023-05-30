
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionRinkoRushAttack : ActionSelectSkillBase
{
    private eAnimation  mCurAni             = eAnimation.RushAttack;
    private float       mDist               = 15.0f;
    private float       mLookAtTargetAngle  = 180.0f;
    private Unit        mTarget             = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.RushAttack;

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

        superArmor = Unit.eSuperArmor.Lv1;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        ShowSkillNames(m_data);

        // 이거 ActionSelectSkillBase 쪽으로 넣어야 할 거 같은디...지금은 귀찮아서 그냥 감
        if (mOwnerPlayer.boWeapon != null)
        {
            mOwnerPlayer.boWeapon.Execute(BattleOption.eBOTimingType.StartSkill, TableId, null);
            mOwnerPlayer.boWeapon.Execute(BattleOption.eBOTimingType.DuringSkill, TableId, null);
        }

        mCurAni = eAnimation.RushAttack;

        if (mValue1 == 1.0f)
        {
            List<AniEvent.sEvent> list = m_owner.aniEvent.GetAllMeleeAttackEvent(mCurAni);
            for (int i = 0; i < list.Count; i++)
            {
                list[i].behaviour = eBehaviour.GroggyAttack;
            }
        }
        
        if(mValue1 > 1.0f)
        {
            mCurAni = eAnimation.RushAttack2;
        }

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
        float endTime = m_aniLength;
        if(SetAddAction)
        {
            endTime = m_aniCutFrameLength;
        }

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime > endTime)
            {
                m_endUpdate = true;
            }

            yield return mWaitForFixedUpdate;
        }

        if(SetAddAction)
        {
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

            m_aniLength = m_owner.PlayAniImmediate(mCurAni);
            m_endUpdate = false;
            m_checkTime = 0.0f;

            while (!m_endUpdate)
            {
                m_checkTime += m_owner.fixedDeltaTime;
                if (m_checkTime > m_aniLength)
                {
                    m_endUpdate = true;
                }

                yield return mWaitForFixedUpdate;
            }
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
