
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionOboroChargeThrowAttack : ActionChargeAttack
{
    protected eAnimation mFireAni = eAnimation.ChargeEnd2;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        m_psStart = GameSupport.CreateParticle("Effect/Character/prf_fx_oboro_charge_step01.prefab", m_owner.transform);

        mFireAni = eAnimation.ChargeEnd2;
        if (mValue1 >= 1)
        {
            mFireAni = eAnimation.ChargeEnd3;
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        mDir = Vector3.zero;
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
    }

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(mFireAni);
        if (evt == null)
        {
            return 0.0f;
        }

        return evt.visionRange;
    }
}
