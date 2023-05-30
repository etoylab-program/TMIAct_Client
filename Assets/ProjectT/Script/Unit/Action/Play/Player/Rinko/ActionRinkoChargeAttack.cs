
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionRinkoChargeAttack : ActionChargeAttack
{
    private eAnimation		mStartAni		= eAnimation.ChargeBrandish;
    private eAnimation		mEndAni			= eAnimation.None;
	private WaitForSeconds	mWaitForSeconds	= new WaitForSeconds(0.2f);


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

        m_psStart = GameSupport.CreateParticle("Effect/Character/prf_fx_rinko_charge_step01.prefab", m_owner.transform);
        m_listPSCharge.Add(GameSupport.CreateParticle("Effect/Character/prf_fx_rinko_charge_step02.prefab", m_owner.transform));

        if(mValue1 >= 1)
        {
            mStartAni = eAnimation.ChargeBrandish2;
        }

        if (mValue1 >= 2)
        {
            mEndAni = eAnimation.ChargeEnd2;
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        //SetTarget();
        m_psStart.gameObject.SetActive(true);
    }

	public override IEnumerator UpdateAction() {
		float chargingTime = 0.0f;

		while( m_endUpdate == false ) {
			if( m_chargeCount < World.Instance.UIPlay.btnAtk.m_maxChargeCount ) {
				chargingTime += m_owner.fixedDeltaTime;

				m_chargeCount = Mathf.Clamp( (int)( chargingTime / World.Instance.UIPlay.btnAtk.m_chargeTime ), 1, World.Instance.UIPlay.btnAtk.m_maxChargeCount );
                if( m_chargeCount != m_beforeChargeCount ) {
                    PlayEffCharge( m_chargeCount - 1 );
                }

				m_beforeChargeCount = m_chargeCount;
			}

			UpdateMove( m_owner.speed );
			yield return mWaitForFixedUpdate;
		}

		StopAllEffCharge();

		if( m_chargeCount > 0 ) {
			StartChargeAttack();
			ShowSkillNames( m_data );

			SetTarget();

			m_aniLength = m_owner.PlayAniImmediate( mStartAni );

			m_checkTime = 0.0f;
			while( m_checkTime <= m_aniLength ) {
				m_checkTime += m_owner.fixedDeltaTime;

				if( mTargetCollider && m_checkTime <= m_owner.aniEvent.GetCutFrameLength( mStartAni ) ) {
					m_owner.LookAtTarget( mTargetCollider.GetCenterPos() );
					m_owner.SetStepForwardDir( m_owner.transform.forward );
				}

				yield return mWaitForFixedUpdate;
			}

			if( mEndAni != eAnimation.None ) {
				m_checkTime = 0.0f;
				m_aniLength = m_owner.PlayAni( mEndAni );

				while( m_checkTime <= m_aniLength ) {
                    m_checkTime += m_owner.fixedDeltaTime;

                    if( mTargetCollider && mTargetCollider.Owner && mTargetCollider.Owner.curHp <= 0.0f ) {
                        SetTarget();
					}

					yield return mWaitForFixedUpdate;
				}
				//yield return new WaitForSeconds(m_owner.PlayAni(mEndAni));
			}
			else {
				yield return mWaitForSeconds;
			}
		}
		else {
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
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(mStartAni);
        if (evt == null)
        {
            Debug.LogError(mStartAni.ToString() + "공격 이벤트가 없네??");
            return 0.0f;
        }
        else if (evt.visionRange <= 0.0f)
        {
            Debug.LogError(mStartAni.ToString() + "Vistion Range가 0이네??");
        }

        return evt.visionRange;
    }

    private void SetTarget()
    {
        mTargetCollider = m_owner.GetMainTargetCollider(true);
        if (mTargetCollider == null)
        {
            return;
        }

        m_owner.LookAtTarget(mTargetCollider.GetCenterPos());
        m_owner.SetMainTarget(mTargetCollider.Owner);
    }
}
