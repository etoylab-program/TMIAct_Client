
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionRinkoDropAttack : ActionSelectSkillBase
{
    protected Unit          mTarget = null;
    protected eAnimation    mCurAni = eAnimation.DownAttack;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.HoldingDefBtnAttack;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        extraCancelCondition = new eActionCondition[3];
        extraCancelCondition[0] = eActionCondition.UseSkill;
        extraCancelCondition[1] = eActionCondition.UseQTE;
        extraCancelCondition[2] = eActionCondition.UseUSkill;

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

        superArmor = Unit.eSuperArmor.Lv1;

        List<Projectile> listProjectile = m_owner.aniEvent.GetAllProjectile(eAnimation.DownAttack);
        for (int i = 0; i < listProjectile.Count; i++)
        {
            listProjectile[i].SetProjectileAtkAttr(Projectile.EProjectileAtkAttr.NORMAL);
        }

        listProjectile = m_owner.aniEvent.GetAllProjectile(eAnimation.DownAttack2);
        for (int i = 0; i < listProjectile.Count; i++)
        {
            listProjectile[i].SetProjectileAtkAttr(Projectile.EProjectileAtkAttr.NORMAL);
        }

        if (mValue1 >= 2.0f)
        {
            listProjectile = m_owner.aniEvent.GetAllProjectile(eAnimation.DownAttack);
            for (int i = 0; i < listProjectile.Count; i++)
            {
                listProjectile[i].SetProjectileAtkAttr(Projectile.EProjectileAtkAttr.DOWN);
            }

            listProjectile = m_owner.aniEvent.GetAllProjectile(eAnimation.DownAttack2);
            for (int i = 0; i < listProjectile.Count; i++)
            {
                listProjectile[i].SetProjectileAtkAttr(Projectile.EProjectileAtkAttr.DOWN);
            }
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        ShowSkillNames(m_data);

        if (mValue1 <= 0.0f)
        {
            mCurAni = eAnimation.DownAttack;
        }
        else
        {
            mCurAni = eAnimation.DownAttack2;
        }

        m_aniLength = m_owner.PlayAniImmediate(mCurAni);

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
