
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionIngridCharge2Attack : ActionChargeAttack
{
    private eAnimation  mCurAni     = eAnimation.AttackCharge2;
    private float       mDistance   = 0.0f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

        m_psStart = GameSupport.CreateParticle("Effect/Character/prf_fx_ingrid_charge_step01.prefab", m_owner.transform);
        m_listPSCharge.Add(GameSupport.CreateParticle("Effect/Character/prf_fx_ingrid_charge_step01.prefab", m_owner.transform));

        if (mValue2 >= 1.0f)
        {
            List<AniEvent.sEvent> list = m_owner.aniEvent.GetAllAttackEvent(mCurAni);
            for (int i = 0; i < list.Count; i++)
            {
                list[i].behaviour = eBehaviour.StunAttack;
            }
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        mTargetCollider = m_owner.GetMainTargetCollider(true);
        if (mTargetCollider)
        {
            m_owner.LookAtTarget(mTargetCollider.GetCenterPos());
        }

        m_psStart.gameObject.SetActive(true);
    }

	public override IEnumerator UpdateAction() {
		float chargingTime = 0.0f;

		while ( m_endUpdate == false ) {
			if ( m_chargeCount < World.Instance.UIPlay.btnAtk.m_maxChargeCount ) {
				chargingTime += m_owner.fixedDeltaTime;

				m_chargeCount = Mathf.Clamp( (int)( chargingTime / World.Instance.UIPlay.btnAtk.m_chargeTime ), 1, World.Instance.UIPlay.btnAtk.m_maxChargeCount );
				if ( m_chargeCount != m_beforeChargeCount ) {
					PlayEffCharge( m_chargeCount - 1 );
				}

				m_beforeChargeCount = m_chargeCount;
			}

			UpdateMove( m_owner.speed );
			yield return mWaitForFixedUpdate;
		}

		mTargetCollider = m_owner.GetMainTargetCollider( true );
		StopAllEffCharge();

		if ( m_chargeCount > 0 ) {
			m_chargeCount = 2;
			if ( mValue1 >= 1.0f ) {
				m_chargeCount += 2;
			}

			StartChargeAttack();
			ShowSkillNames( m_data );

			float aniLength = 0.0f;
			for ( int i = 0; i < m_chargeCount; i++ ) {
				if ( PrepareAttack() ) {
					m_owner.OnStepForward( m_aniCutFrameLength, false, mDistance, true, 0.0f, true );
				}

				if ( i >= m_chargeCount - 1 ) {
					aniLength = m_aniLength;
				}
				else {
					aniLength = m_aniCutFrameLength;
				}

				while ( m_checkTime < aniLength ) {
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

        m_endUpdate = true;
    }

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( mCurAni );
        if (evt == null)
        {
            Debug.LogError(mCurAni + " 공격 이벤트가 없네??");
            return 0.0f;
        }
        else if (evt.visionRange <= 0.0f)
        {
            Debug.LogError(mCurAni + " Vistion Range가 0이네??");
        }

        return evt.visionRange;
    }

    private bool PrepareAttack()
    {
        if (mTargetCollider == null)
        {
            m_aniLength = m_owner.PlayAniImmediate(mCurAni);
            return false;
        }

        mTargetCollider = m_owner.GetRandomTargetCollider();
        if (mTargetCollider == null)
        {
            m_aniLength = m_owner.PlayAniImmediate(mCurAni);
            return false;
        }

        mDir = (mTargetCollider.Owner.transform.position - m_owner.transform.position).normalized;
        Vector3 dest = mTargetCollider.Owner.transform.position + (mDir * 2.0f);
        mDistance = Vector3.Distance(m_owner.transform.position, dest);

        RaycastHit hitInfo;
        if (Physics.Raycast(m_owner.transform.position, mDir, out hitInfo, mDistance, (1 << (int)eLayer.EnvObject) | (1 << (int)eLayer.Wall)))
        {
            mDistance = hitInfo.distance <= 1.0f ? 0.0f : hitInfo.distance;
        }

        m_owner.LookAtTarget(dest);

        m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(mCurAni);
        m_aniLength = m_owner.PlayAniImmediate(mCurAni);
        m_checkTime = 0.0f;

        return true;
    }
}
