
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionIngridEvadeTimingAttack : ActionSelectSkillBase
{
    private eAnimation  mCurAni             = eAnimation.EvadeTiming;
    private ActionDash  mActionDash         = null;
    private float       mCurDashSpeedRatio  = 0.0f;
    private bool        mNoDir              = false;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.TimingHoldAttack;

        extraCondition = new eActionCondition[2];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingUSkill;

        superArmor = Unit.eSuperArmor.Lv1;

        if (mValue2 >= 1.0f)
        {
            mCurAni = eAnimation.EvadeTiming_1;
        }

        if (mValue1 >= 1.0f)
        {
            List<Projectile> list = m_owner.aniEvent.GetAllProjectile(mCurAni);
            for (int i = 0; i < list.Count; i++)
            {
                list[i].SetProjectileAtkAttr(Projectile.EProjectileAtkAttr.DOWN);
            }
        }

        mActionDash = m_owner.actionSystem.GetAction<ActionDash>(eActionCommand.Defence);
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        ShowSkillNames(m_data);

        Utility.IgnorePhysics(eLayer.Player, eLayer.Enemy);
        Utility.IgnorePhysics(eLayer.Player, eLayer.EnemyGate);

        mNoDir = false;

        mDir = m_owner.Input.GetDirection();
        if (mDir == Vector3.zero)
        {
            mDir = -m_owner.transform.forward;
            mNoDir = true;
        }

        mCurDashSpeedRatio = mActionDash.DashSpeedRatio;

        if (Physics.Raycast(m_owner.MainCollider.GetCenterPos(), mDir, out RaycastHit hitInfo, mCurDashSpeedRatio, 
                            (1 << (int)eLayer.EnvObject) | (1 << (int)eLayer.Wall)))
        {
            if (hitInfo.distance < mCurDashSpeedRatio)
            {
                mCurDashSpeedRatio = hitInfo.distance <= 1.0f ? 0.0f : hitInfo.distance;
            }
        }

        mDir = new Vector3(mDir.x, 0.0f, mDir.z);
        m_owner.StopStepForward();
        m_aniLength = m_owner.PlayAniImmediate(mCurAni);
    }

    public override IEnumerator UpdateAction()
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            if (!mNoDir)
            {
                m_owner.cmptRotate.UpdateRotation(mDir, true);
            }

            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime < mActionDash.DashTime)
            {
                m_owner.cmptMovement.UpdatePosition(mDir, m_owner.originalSpeed * mCurDashSpeedRatio, true);
            }
            else if(m_checkTime >= m_aniLength)
            {
                m_endUpdate = true;
            }

            yield return mWaitForFixedUpdate;
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();

        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.Enemy);
        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.EnemyGate);
    }

    public override void OnCancel()
    {
        base.OnCancel();
        m_owner.cmptMovement.UpdatePosition(Vector3.zero, 0.0f, true);

        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.Enemy);
        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.EnemyGate);
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
