
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionNoahChargeAttack : ActionChargeAttack
{
    private float mRunTime          = 2.0f;
    private float mRunSpeedRatio    = 1.2f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        if (World.Instance.StageType == eSTAGETYPE.STAGE_PVP)
        {
            conditionActionCommand = new eActionCommand[3];
            conditionActionCommand[0] = eActionCommand.Idle;
            conditionActionCommand[1] = eActionCommand.MoveByDirection;
            conditionActionCommand[2] = eActionCommand.Attack01;
        }

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

        superArmor = Unit.eSuperArmor.Lv2;
        World.Instance.UIPlay.btnAtk.m_maxChargeCount = 1;

        m_psStart = GameSupport.CreateParticle("Effect/Character/prf_fx_noah_charge_step01.prefab", m_owner.transform);
        m_listPSCharge.Add(GameSupport.CreateParticle("Effect/Character/prf_fx_noah_charge_step02.prefab", m_owner.transform));
        m_listPSCharge.Add(GameSupport.CreateParticle("Effect/Character/prf_fx_noah_charge_step02.prefab", m_owner.transform));

        mRunTime += mValue1;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        ShowSkillNames(m_data);

        if (m_data != null && SkipBOExecuteOnStart)
        {
            ExecuteStartSkillBO();
        }

        m_psStart.gameObject.SetActive(true);
        Debug.LogError("노아 차지 스킬 발동!!!!!!!!!!!!!!!!!!!!!!");
    }

    public override IEnumerator UpdateAction()
    {
        float chargingTime = 0.0f;
        float runningTime = 0.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            runningTime += m_owner.fixedDeltaTime;

            if (runningTime >= mRunTime)
            {
                m_endUpdate = true;
            }
            else if (m_chargeCount < World.Instance.UIPlay.btnAtk.m_maxChargeCount)
            {
                chargingTime += m_owner.fixedDeltaTime;

                m_chargeCount = Mathf.Clamp((int)(chargingTime / World.Instance.UIPlay.btnAtk.m_chargeTime), 1, World.Instance.UIPlay.btnAtk.m_maxChargeCount);
                if (m_chargeCount != m_beforeChargeCount)
                {
                    PlayEffCharge(m_chargeCount - 1);
                }

                m_beforeChargeCount = m_chargeCount;
            }

            UpdateRun();
            yield return mWaitForFixedUpdate;
        }

        mTargetCollider = m_owner.GetMainTargetCollider(true);
        StopAllEffCharge();

        if (m_chargeCount > 0)
        {
            mbStartAttack = true;
            StartCoolTime();

            mTargetCollider = m_owner.GetMainTargetCollider(true);
            if (mTargetCollider)
            {
                m_owner.LookAtTarget(mTargetCollider.GetCenterPos());
            }

            yield return new WaitForSeconds(m_owner.PlayAniImmediate(eAnimation.AttackCharge_1));
        }
        else
        {
            m_owner.actionSystem.CancelCurrentAction();
        }
    }

    public override void OnUpdating(IActionBaseParam param)
    {
        if (World.Instance.IsPause || mbStartAttack)
        {
            if (World.Instance.IsPause && !mbStartAttack)
            {
                OnCancel();
            }

            return;
        }

        m_endUpdate = true;
    }

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(eAnimation.AttackCharge_1);
        if (evt == null)
        {
            Debug.LogError("AttackCharge_1 공격 이벤트가 없네??");
            return 0.0f;
        }
        else if (evt.visionRange <= 0.0f)
        {
            Debug.LogError("AttackCharge_1 Vistion Range가 0이네??");
        }

        return evt.visionRange;
    }

    private void UpdateRun()
    {
        if (m_owner.aniEvent.IsAniPlaying(eAnimation.AttackCharge) != eAniPlayingState.Playing)
        {
            m_owner.PlayAniImmediate(eAnimation.AttackCharge);
        }

        /*
        Vector3 inputDir = m_owner.Input.GetRawDirection();

        Vector3 cameraRight = World.Instance.InGameCamera.transform.right;
        Vector3 cameraForward = World.Instance.InGameCamera.transform.forward;

        mDir = Vector3.zero;
        mDir.x = (inputDir.x * cameraRight.x) + (m_owner.transform.forward.x);
        mDir.z = (inputDir.x * cameraRight.z) + (m_owner.transform.forward.z);
        mDir.y = 0.0f;

        m_owner.cmptRotate.UpdateRotation(mDir, false, mValue2);
        m_owner.cmptMovement.UpdatePosition(m_owner.transform.forward, m_owner.speed * mRunSpeedRatio, false);
        */

        if(World.Instance.StageType == eSTAGETYPE.STAGE_PVP)
        {
            if (mTargetCollider == null)
            {
                mTargetCollider = m_owner.GetMainTargetCollider(true);
            }

            if(mTargetCollider)
            {
                m_owner.LookAtTarget(mTargetCollider.GetCenterPos());
            }
        }
        else
        {
            Vector3 inputDir = m_owner.Input.GetRawDirection();

            Vector3 cameraRight = World.Instance.InGameCamera.transform.right;
            Vector3 cameraForward = World.Instance.InGameCamera.transform.forward;

            mDir = Vector3.zero;
            mDir.x = (inputDir.x * cameraRight.x) + (inputDir.y * cameraForward.x);
            mDir.z = (inputDir.x * cameraRight.z) + (inputDir.y * cameraForward.z);
            mDir.y = 0.0f;

            m_owner.cmptRotate.UpdateRotation(mDir, true);
        }

        m_owner.cmptMovement.UpdatePosition(m_owner.transform.forward, m_owner.speed * mRunSpeedRatio, false);
    }
}
