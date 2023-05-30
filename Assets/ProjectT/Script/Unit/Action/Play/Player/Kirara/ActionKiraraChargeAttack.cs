
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionKiraraChargeAttack : ActionChargeAttack
{
    private int                 mAttackCount            = 0;
    private eActionCommand[]    mOriginalCancelCommand  = null;
	private Unit				mTarget					= null;


	public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        superArmor = Unit.eSuperArmor.Lv1;

        m_psStart = GameSupport.CreateParticle("Effect/Character/prf_fx_kirara_charge_step01.prefab", m_owner.transform);
        m_listPSCharge.Add(GameSupport.CreateParticle("Effect/Character/prf_fx_kirara_charge_step01.prefab", m_owner.transform));

        mOriginalCancelCommand = cancelActionCommand;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        mAttackCount += (int)mValue1;
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

			m_aniLength = m_owner.PlayAniImmediate(eAnimation.AttackCharge);
            yield return new WaitForSeconds(m_aniLength);

            //Unit beforeNearestTarget = null;

            m_aniLength = m_owner.PlayAniImmediate(eAnimation.AttackCharge_1);
            while (mAttackCount > 0)
            {
                m_checkTime += m_owner.fixedDeltaTime;
                if (m_checkTime >= m_aniLength)
                {
                    m_checkTime = 0.0f;

                    m_aniLength = m_owner.PlayAniImmediate(eAnimation.AttackCharge_1);
                    --mAttackCount;
                }

				/*
                Unit nearestTarget = World.Instance.EnemyMgr.GetNearestTarget(m_owner, m_owner.listHitTarget);
                if(nearestTarget == null)
                {
                    nearestTarget = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
                    beforeNearestTarget = null;
                }

                if (m_owner.aniEvent.curAniType == eAnimation.AttackCharge_1 && nearestTarget && nearestTarget != beforeNearestTarget)
                {
                    m_owner.cmptRotate.UpdateRotation(nearestTarget.transform.position - m_owner.transform.position, false);
                    beforeNearestTarget = nearestTarget;
                }
				*/

				if (World.Instance.StageType != eSTAGETYPE.STAGE_PVP)
				{
					m_owner.cmptRotate.UpdateRotation(m_owner.Input.GetDirection(), true);
				}
				else
				{
					mTarget = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
					if (mTarget)
					{
						m_owner.LookAtTarget(mTarget.transform.position);
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

            m_aniLength = m_owner.PlayAniImmediate(eAnimation.AttackCharge_2);
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

        mAttackCount = 0;
        cancelActionCommand = mOriginalCancelCommand;

        m_owner.cmptBuffDebuff.RemoveDebuff(eEventType.EVENT_DEBUFF_DECREASE_SP);
    }

    public override void OnCancel()
    {
        base.OnCancel();

        mAttackCount = 0;
        cancelActionCommand = mOriginalCancelCommand;

        m_owner.cmptBuffDebuff.RemoveDebuff(eEventType.EVENT_DEBUFF_DECREASE_SP);
    }

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(eAnimation.AttackCharge_1);
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
}
