
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionCloneShiranuiSpecialAttack : ActionSelectSkillBase
{
    private Unit        mTarget                 = null;
    private eAnimation  mCurAni                 = eAnimation.ChargeBrandish;
    private float       mOriginalAttackPower    = 0.0f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Attack02;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        extraCancelCondition = new eActionCondition[3];
        extraCancelCondition[0] = eActionCondition.UseSkill;
        extraCancelCondition[1] = eActionCondition.UseQTE;
        extraCancelCondition[2] = eActionCondition.UseUSkill;

        superArmor = Unit.eSuperArmor.Lv1;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        if(mOriginalAttackPower == 0.0f)
        {
            mOriginalAttackPower = m_owner.attackPower;
        }

        m_owner.SetAttackPower(m_owner.cloneOwner.attackPower * m_owner.IncreaseSkillAtkValue);
        m_aniLength = m_owner.PlayAniImmediate(mCurAni);

        mTarget = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
        if (mTarget)
        {
            m_owner.LookAtTarget(mTarget.transform.position);
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

    public override void OnEnd()
    {
        base.OnEnd();

        m_owner.SetAttackPower(mOriginalAttackPower);
        m_owner.UseAttack02 = false;
    }

    public override void OnCancel()
    {
        base.OnCancel();

        m_owner.UseAttack02 = false;
        m_owner.SetAttackPower(mOriginalAttackPower);
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
