
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionJingleiChargeAttack : ActionChargeAttack
{
    private eAnimation		mFireAni		= eAnimation.AttackCharge2;
	private WaitForSeconds	mWaitForSeconds	= new WaitForSeconds(0.2f);


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

        m_psStart = GameSupport.CreateParticle("Effect/Character/prf_fx_jinglei_charge_step01.prefab", m_owner.transform);
        m_listPSCharge.Add(GameSupport.CreateParticle("Effect/Character/prf_fx_jinglei_charge_step02.prefab", m_owner.transform));

        mFireAni = eAnimation.AttackCharge2;
        if (mValue1 >= 1.0f)
        {
            mFireAni = eAnimation.AttackCharge2_1;
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        m_psStart.gameObject.SetActive(true);

        if (FSaveData.Instance.AutoTargetingSkill)
        {
            UnitCollider targetCollider = m_owner.GetMainTargetCollider(true);
            if (targetCollider)
            {
                m_owner.LookAtTarget(targetCollider.GetCenterPos());
            }
        }
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
                {
                    PlayEffCharge(m_chargeCount - 1);
                }

                m_beforeChargeCount = m_chargeCount;
            }

            UpdateMove(m_owner.speed);
            yield return mWaitForFixedUpdate;
        }
        
        StopAllEffCharge();

        if (m_chargeCount <= 0)
        {
            m_owner.actionSystem.CancelCurrentAction();
        }
        else
        {
            StartChargeAttack();

            if (FSaveData.Instance.AutoTargetingSkill)
            {
                UnitCollider targetCollider = m_owner.GetMainTargetCollider(true);
                if (targetCollider)
                {
                    m_owner.LookAtTarget(targetCollider.GetCenterPos());
                }
            }
            else
            {
                m_owner.cmptRotate.UpdateRotation(m_owner.Input.GetDirection(), true);
            }

            ShowSkillNames(m_data);

            m_endUpdate = false;
            m_checkTime = 0.0f;
            m_aniLength = m_owner.PlayAniImmediate(mFireAni);

            while (!m_endUpdate)
            {
                m_checkTime += m_owner.fixedDeltaTime;
                if (m_checkTime >= m_aniLength)
                {
                    --m_chargeCount;
                    m_endUpdate = true;
                }

                yield return mWaitForFixedUpdate;
            }

            yield return mWaitForSeconds;
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
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(mFireAni);
        if (evt == null)
        {
            Debug.LogError(mFireAni.ToString() + "공격 이벤트가 없네??");
            return 0.0f;
        }
        else if (evt.visionRange <= 0.0f)
        {
            Debug.LogError(mFireAni.ToString() + "Vistion Range가 0이네??");
        }

        return evt.visionRange;
    }
}
