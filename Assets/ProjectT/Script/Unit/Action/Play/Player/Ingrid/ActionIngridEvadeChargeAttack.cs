
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionIngridEvadeChargeAttack : ActionSelectSkillBase
{
    protected Unit          mTarget = null;
    protected eAnimation    mCurAni = eAnimation.EvadeCharge;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.HoldingDefBtnAttack;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

        extraCancelCondition = new eActionCondition[3];
        extraCancelCondition[0] = eActionCondition.UseSkill;
        extraCancelCondition[1] = eActionCondition.UseQTE;
        extraCancelCondition[2] = eActionCondition.UseUSkill;

        superArmor = Unit.eSuperArmor.Lv1;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        ShowSkillNames(m_data);

        float useSp = ( AddActionValue1 * ( float )eCOUNT.MAX_BO_FUNC_VALUE );

        if ( SetAddAction && m_owner.curSp >= useSp ) {
            mCurAni = eAnimation.EvadeCharge2;
            m_owner.UseSp( useSp );
        }
        else {
            mCurAni = eAnimation.EvadeCharge;
        }

        if (mValue1 >= 1.0f)
        {
            List<AniEvent.sEvent> list = m_owner.aniEvent.GetAllAttackEvent(mCurAni);
            for (int i = 0; i < list.Count; i++)
            {
                list[i].behaviour = eBehaviour.GroggyAttack;
            }
        }

        m_aniLength = m_owner.PlayAniImmediate( mCurAni );
    }

    public override IEnumerator UpdateAction()
    {
        float firstAtkEventLength = m_owner.aniEvent.GetFirstAttackEventLength(mCurAni);

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if(m_checkTime <= firstAtkEventLength)
            {
                if (FSaveData.Instance.AutoTargetingSkill)
                {
                    mTarget = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
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
            else if (m_checkTime >= m_aniLength)
            {
                m_endUpdate = true;
            }

            yield return mWaitForFixedUpdate;
        }
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
