
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionRinkoChargeHomingAttack : ActionChargeAttack
{
    private eAnimation mCurAni = eAnimation.ChargingAttack;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

        m_psStart = GameSupport.CreateParticle("Effect/Character/prf_fx_rinko_charge_step01.prefab", m_owner.transform);
        m_listPSCharge.Add(GameSupport.CreateParticle("Effect/Character/prf_fx_rinko_charge_step02.prefab", m_owner.transform));

        List<Projectile> listProjectile = m_owner.aniEvent.GetAllProjectile(eAnimation.ChargingAttack);
        for(int i = 0; i < listProjectile.Count; i++)
        {
            listProjectile[i].SetProjectileAtkAttr(Projectile.EProjectileAtkAttr.NORMAL);
        }

        listProjectile = m_owner.aniEvent.GetAllProjectile(eAnimation.ChargingAttack2);
        for (int i = 0; i < listProjectile.Count; i++)
        {
            listProjectile[i].SetProjectileAtkAttr(Projectile.EProjectileAtkAttr.NORMAL);
        }

        if(mValue1 >= 2.0f)
        {
            listProjectile = m_owner.aniEvent.GetAllProjectile(eAnimation.ChargingAttack);
            for (int i = 0; i < listProjectile.Count; i++)
            {
                listProjectile[i].SetProjectileAtkAttr(Projectile.EProjectileAtkAttr.KNOCKBACK);
            }

            listProjectile = m_owner.aniEvent.GetAllProjectile(eAnimation.ChargingAttack2);
            for (int i = 0; i < listProjectile.Count; i++)
            {
                listProjectile[i].SetProjectileAtkAttr(Projectile.EProjectileAtkAttr.KNOCKBACK);
            }
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        mDir = Vector3.zero;
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

        if (m_chargeCount <= 0)
        {
            m_owner.actionSystem.CancelCurrentAction();
        }
        else
        {
            StartChargeAttack();
            ShowSkillNames(m_data);

            mCurAni = eAnimation.ChargingAttack;
            if (mValue1 > 0)
            {
                mCurAni = eAnimation.ChargingAttack2;
                m_aniLength = m_owner.aniEvent.GetCutFrameLength(mCurAni);
            }

            m_endUpdate = false;
            m_checkTime = 0.0f;
            m_aniLength = m_owner.PlayAniImmediate(mCurAni);

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

    protected override void StartChargeAttack()
    {
        base.StartChargeAttack();
    }
}
