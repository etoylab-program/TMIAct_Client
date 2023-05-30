
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionNoahEvadeCharge2Attack : ActionSelectSkillBase
{
    private eAnimation  mCurAni = eAnimation.EvadeCharge2;


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

        superArmor = Unit.eSuperArmor.Lv1;

        if(mValue1 > 0.0f)
        {
            mCurAni = eAnimation.EvadeCharge2_1;
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        ShowSkillNames(m_data);
        LookAtTarget();

        m_aniLength = m_owner.PlayAniImmediate(mCurAni);
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (!m_endUpdate)
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
