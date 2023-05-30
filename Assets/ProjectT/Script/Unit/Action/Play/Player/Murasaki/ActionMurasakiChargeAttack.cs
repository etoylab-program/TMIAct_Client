
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionMurasakiChargeAttack : ActionChargeAttack
{
    private int                 mRotationCount  = 0;
    private eActionCommand[]    mOriginalCancelCommand = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        superArmor = Unit.eSuperArmor.Lv2;

        m_psStart = GameSupport.CreateParticle("Effect/Character/prf_fx_murasaki_charge.prefab", m_owner.transform);
        m_listPSCharge.Add(GameSupport.CreateParticle("Effect/Character/prf_fx_murasaki_charge.prefab", m_owner.transform));

        mOriginalCancelCommand = cancelActionCommand;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        if(mValue2 >= 1.0f)
        {
            List<AniEvent.sEvent> list = m_owner.aniEvent.GetAllMeleeAttackEvent(eAnimation.ChargeEnd);
            for (int i = 0; i < list.Count; i++)
            {
                list[i].behaviour = eBehaviour.KnockBackAttack;
            }
        }

        mRotationCount += (int)mValue1;
        m_psStart.gameObject.SetActive(true);
    }

    public override IEnumerator UpdateAction()
    {
        float chargingTime = 0.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (m_endUpdate == false)
        {
            if (m_chargeCount < World.Instance.UIPlay.btnAtk.m_maxChargeCount)
            {
                chargingTime += m_owner.fixedDeltaTime;

                m_chargeCount = Mathf.Clamp((int)(chargingTime / World.Instance.UIPlay.btnAtk.m_chargeTime), 1, World.Instance.UIPlay.btnAtk.m_maxChargeCount);
                if (m_chargeCount != m_beforeChargeCount)
                    PlayEffCharge(m_chargeCount - 1);

                m_beforeChargeCount = m_chargeCount;
            }

            UpdateMove(m_owner.speed);
            yield return mWaitForFixedUpdate;
        }

        StopAllEffCharge();

        if (m_chargeCount > 0)
        {
            StartChargeAttack();
            ShowSkillNames(m_data);

            m_checkTime = 0.0f;

            m_aniLength = m_owner.PlayAniImmediate(eAnimation.ChargingStart);
            while (mRotationCount > 0)
            {
                m_checkTime += m_owner.fixedDeltaTime;
                if (m_checkTime >= m_aniLength)
                {
                    m_checkTime = 0.0f;

                    m_aniLength = m_owner.PlayAniImmediate(eAnimation.ChargingAttack);
                    --mRotationCount;
                }

                if (m_owner.aniEvent.curAniType == eAnimation.ChargingAttack)
                {
                    UpdateMoveOnAttack(m_owner.speed * mValue3);
                }

                yield return mWaitForFixedUpdate;
            }

            if (SetAddAction)
            {
                SkipConditionCheck = false;

                cancelActionCommand = new eActionCommand[1];
                cancelActionCommand[0] = eActionCommand.Defence;

                mOwnerPlayer.ExecuteBattleOption(BattleOption.eBOTimingType.OnStartAddAction, 0, null);

                cancelActionCommand = new eActionCommand[1];
                cancelActionCommand[0] = eActionCommand.Defence;

                float fChargePerSec = GameInfo.Instance.BattleConfig.USMaxSP / GameInfo.Instance.BattleConfig.USSPRegenSpeedTime;
                fChargePerSec += fChargePerSec * mOwnerPlayer.SPRegenIncRate;

                while (m_owner.curSp > fChargePerSec)
                {
                    m_checkTime += m_owner.fixedDeltaTime;
                    if (m_checkTime >= m_aniLength)
                    {
                        m_checkTime = 0.0f;
                        m_aniLength = m_owner.PlayAniImmediate(eAnimation.ChargingAttack);
                    }

                    if (m_owner.aniEvent.curAniType == eAnimation.ChargingAttack)
                    {
                        UpdateMoveOnAttack(m_owner.speed * mValue3);
                    }

                    yield return mWaitForFixedUpdate;
                }
            }

            m_aniLength = m_owner.PlayAniImmediate(eAnimation.ChargeEnd);
            yield return new WaitForSeconds(m_aniLength);            
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

    public override void OnEnd()
    {
        if (SetAddAction)
        {
            m_owner.cmptBuffDebuff.RemoveDebuff(eEventType.EVENT_DEBUFF_DECREASE_SP);
        }

        base.OnEnd();

        mRotationCount = 0;
        cancelActionCommand = mOriginalCancelCommand;
    }

    public override void OnCancel()
    {
        if (SetAddAction)
        {
            m_owner.cmptBuffDebuff.RemoveDebuff(eEventType.EVENT_DEBUFF_DECREASE_SP);
        }

        base.OnCancel();

        mRotationCount = 0;
        cancelActionCommand = mOriginalCancelCommand;
    }

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(eAnimation.ChargingStart);
        if (evt == null)
        {
            Debug.LogError("Charging Start 공격 이벤트가 없네??");
            return 0.0f;
        }
        else if (evt.visionRange <= 0.0f)
        {
            Debug.LogError("Charging Start Vistion Range가 0이네??");
        }

        return evt.visionRange;
    }

    private void UpdateMoveOnAttack(float speed)
    {
        if (m_owner.Input.GetRawDirection() == Vector3.zero)
        {
            return;
        }

        Vector3 inputDir = m_owner.Input.GetRawDirection();

        Vector3 cameraRight = World.Instance.InGameCamera.transform.right;
        Vector3 cameraForward = World.Instance.InGameCamera.transform.forward;

        mDir = Vector3.zero;
        mDir.x = (inputDir.x * cameraRight.x) + (inputDir.y * cameraForward.x);
        mDir.z = (inputDir.x * cameraRight.z) + (inputDir.y * cameraForward.z);
        mDir.y = 0.0f;

        m_owner.cmptRotate.UpdateRotation(mDir, true);
        m_owner.cmptMovement.UpdatePosition(mDir, speed, false);
    }
}
