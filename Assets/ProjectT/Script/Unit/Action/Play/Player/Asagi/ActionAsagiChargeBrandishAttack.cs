
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionAsagiChargeBrandishAttack : ActionChargeAttack
{
    private float mDistance = 0.0f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        World.Instance.UIPlay.btnAtk.m_maxChargeCount = 2;

        m_psStart = GameSupport.CreateParticle("Effect/Character/prf_fx_asagi_charge_step01.prefab", m_owner.transform);
        m_listPSCharge.Add(GameSupport.CreateParticle("Effect/Character/prf_fx_asagi_charge_step02.prefab", m_owner.transform));
        m_listPSCharge.Add(GameSupport.CreateParticle("Effect/Character/prf_fx_asagi_charge_step02.prefab", m_owner.transform));
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        mTargetCollider = m_owner.GetMainTargetCollider(true);
        if (mTargetCollider)
        {
            m_owner.LookAtTarget(mTargetCollider.GetCenterPos());
            
            //m_owner.SetMainTarget(mTargetCollider.Owner);
            //World.Instance.InGameCamera.TurnToTarget(mTargetCollider.GetCenterPos(), 2.0f);
        }

        m_psStart.gameObject.SetActive(true);
    }

	public override IEnumerator UpdateAction() {
		float chargingTime = 0.0f;

		while ( m_endUpdate == false ) {
			if ( m_chargeCount < World.Instance.UIPlay.btnAtk.m_maxChargeCount ) {
				chargingTime += m_owner.fixedDeltaTime;

				m_chargeCount = Mathf.Clamp( (int)( chargingTime / World.Instance.UIPlay.btnAtk.m_chargeTime ), 1, World.Instance.UIPlay.btnAtk.m_maxChargeCount );
				if ( m_chargeCount != m_beforeChargeCount )
					PlayEffCharge( m_chargeCount - 1 );

				m_beforeChargeCount = m_chargeCount;
			}

			UpdateMove( m_owner.speed );
			yield return mWaitForFixedUpdate;
		}

		mTargetCollider = m_owner.GetMainTargetCollider( true );
		StopAllEffCharge();

		if ( m_chargeCount > 0 ) {
			m_chargeCount = World.Instance.UIPlay.btnAtk.m_maxChargeCount + (int)mValue1;

			StartChargeAttack();
			ShowSkillNames( m_data );

			for ( int i = 0; i < m_chargeCount; i++ ) {
				if ( PrepareAttack() ) {
					m_owner.OnStepForward( m_aniCutFrameLength, false, mDistance, true, 0.0f );
				}

				while ( m_checkTime < m_aniLength ) {
					m_checkTime += m_owner.fixedDeltaTime;
					yield return mWaitForFixedUpdate;
				}

				yield return mWaitForFixedUpdate;
			}
		}
		else {
			m_owner.actionSystem.CancelCurrentAction();
		}
	}

	public override void OnUpdating(IActionBaseParam param)
    {
        if(World.Instance.IsPause || mbStartAttack)
        {
            if (World.Instance.IsPause && !mbStartAttack)
            {
                OnCancel();
            }

            return;
        }

        if (m_chargeCount > 0)
        {
            m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(eAnimation.ChargeEnd);
            m_aniLength = m_owner.PlayAniImmediate(eAnimation.ChargeEnd);
        }

        m_endUpdate = true;
    }

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(eAnimation.ChargeBrandish);
        if (evt == null)
        {
            Debug.LogError("ChargeBrandish 공격 이벤트가 없네??");
            return 0.0f;
        }
        else if (evt.visionRange <= 0.0f)
        {
            Debug.LogError("ChargeBrandish Vistion Range가 0이네??");
        }

        return evt.visionRange;
    }

    private bool PrepareAttack()
    {
        if (mTargetCollider == null)
        {
            m_aniLength = m_owner.PlayAniImmediate(eAnimation.ChargeBrandish);
            return false;
        }

        //if (!mTargetCollider.Owner.IsActivate() || mTargetCollider.Owner.curHp <= 0.0f)
            mTargetCollider = m_owner.GetRandomTargetCollider();//m_owner.GetMainTargetCollider(true);

        if (mTargetCollider == null)
        {
            m_aniLength = m_owner.PlayAniImmediate(eAnimation.ChargeBrandish);
            return false;
        }

        mDir = (mTargetCollider.Owner.transform.position - m_owner.transform.position).normalized;
        Vector3 dest = mTargetCollider.Owner.transform.position + (mDir * 2.0f);
        mDistance = Vector3.Distance(m_owner.transform.position, dest);

        RaycastHit hitInfo;
        if (Physics.Raycast(m_owner.transform.position, mDir, out hitInfo, mDistance, (1 << (int)eLayer.EnvObject) | (1 << (int)eLayer.Wall)))
            mDistance = hitInfo.distance <= 1.0f ? 0.0f : hitInfo.distance;

        m_owner.LookAtTarget(dest);

        m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(eAnimation.ChargeBrandish);
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.ChargeBrandish);
        m_checkTime = 0.0f;

        return true;
    }
}
