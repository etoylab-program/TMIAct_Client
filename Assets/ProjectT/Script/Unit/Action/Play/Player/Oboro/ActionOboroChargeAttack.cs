
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionOboroChargeAttack : ActionChargeAttack
{
    private int                 mRotationCount  = 0;
    private eActionCommand[]    mOriginalCancelCommand = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        m_psStart = GameSupport.CreateParticle("Effect/Character/prf_fx_oboro_charge_step01.prefab", m_owner.transform);
        mOriginalCancelCommand = cancelActionCommand;

		if(mValue3 > 0.0f)
		{
			superArmor = Unit.eSuperArmor.Lv2;
		}
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

            if (SetAddAction)
            {
                float f = AddActionValue1 * (float)eCOUNT.MAX_BO_FUNC_VALUE;
                mRotationCount += (int)f;

                /*
                mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Buff;
                mBuffEvt.Set(m_data.ID, eEventSubject.Self, eEventType.EVENT_BLACKHOLE, m_owner, AddActionValue2, 0.0f, 0.0f, AddActionValue3, 0.0f, 
                             mBuffEvt.effId, mBuffEvt.effId2, eBuffIconType.Debuff_Speed);

                EventMgr.Instance.SendEvent(mBuffEvt);
                */
            }

            m_checkTime = 0.0f;

            m_aniLength = m_owner.PlayAniImmediate(eAnimation.ChargingStart);
            while (m_owner.curHp > 0.0f)
            {
                m_checkTime += m_owner.fixedDeltaTime;
                if (m_checkTime >= m_aniLength)
                {
                    if (mRotationCount <= 0)
                    {
                        break;
                    }
                    else
                    {
                        m_checkTime = 0.0f;

                        m_aniLength = m_owner.PlayAniImmediate(eAnimation.ChargingAttack);
                        --mRotationCount;
                    }
                }

                if (m_owner.aniEvent.curAniType == eAnimation.ChargingAttack)
                {
                    UpdateMoveOnAttack(m_owner.speed * 2.0f);
                }

                List<UnitCollider> list = m_owner.GetTargetColliderListByAround(m_owner.transform.position, AddActionValue2);
                if(list != null && list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        Unit target = list[i].Owner;
						if(target == null || target.cmptMovement == null)
						{
							continue;
						}

                        if (target.CurrentSuperArmor >= Unit.eSuperArmor.Lv2 || target.IsImmuneFloat() || target.IsImmuneKnockback())
                        {
                            continue;
                        }

                        if (target.actionSystem.IsCurrentAction(eActionCommand.Appear) || target.curHp <= 0.0f)
                        {
                            continue;
                        }

                        target.actionSystem.CancelCurrentAction();
                        target.StopStepForward();

                        Vector3 v = (m_owner.transform.position - target.transform.position).normalized;
                        v.y = 0.0f;

                        float radius = (target.MainCollider != null ? target.MainCollider.radius : 0.0f) + m_owner.MainCollider.radius;
                        if (Vector3.Distance(target.transform.position, m_owner.transform.position) <= (radius * 2.0f))
                        {
                            v = Vector3.zero;
                        }

                        target.cmptMovement.UpdatePosition(v, Mathf.Max(GameInfo.Instance.BattleConfig.BlackholeMinSpeed, target.speed * 1.2f), false);
                    }
                }

                yield return mWaitForFixedUpdate;
            }

            /*
            if (SetAddAction)
            {
                cancelActionCommand = new eActionCommand[1];
                cancelActionCommand[0] = eActionCommand.Defence;

                mOwnerPlayer.ExecuteBattleOption(BattleOption.eBOTimingType.OnStartAddAction, 0, false);

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

                    yield return waitForFixedUpdate;
                }
            }
            */

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
        base.OnEnd();

        mRotationCount = 0;
        cancelActionCommand = mOriginalCancelCommand;

        m_owner.cmptBuffDebuff.RemoveDebuff(eEventType.EVENT_DEBUFF_DECREASE_SP);
    }

    public override void OnCancel()
    {
        base.OnCancel();

        mRotationCount = 0;
        cancelActionCommand = mOriginalCancelCommand;

        m_owner.cmptBuffDebuff.RemoveDebuff(eEventType.EVENT_DEBUFF_DECREASE_SP);
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
