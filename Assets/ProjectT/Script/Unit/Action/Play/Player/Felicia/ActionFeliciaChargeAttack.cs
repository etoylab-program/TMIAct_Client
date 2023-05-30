
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionFeliciaChargeAttack : ActionChargeAttack
{
	public enum eState
	{
		Jump = 0,
		AttackStart,
		Falling,
		AttackEnd,
		End,
	}


	private State		mState	= new State();
	private eAnimation	mCurAni	= eAnimation.AttackCharge;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

        m_psStart = GameSupport.CreateParticle("Effect/Character/prf_fx_felicia_charge_step_start.prefab", m_owner.transform);
        m_listPSCharge.Add(GameSupport.CreateParticle("Effect/Character/prf_fx_felicia_charge_step_start.prefab", m_owner.transform));

		mState.Init(5);
		mState.Bind(eState.Jump, ChangeJumpState);
		mState.Bind(eState.AttackStart, ChangeAttackStartState);
		mState.Bind(eState.Falling, ChangeFallingState);
		mState.Bind(eState.AttackEnd, ChangeAttackEndState);
		mState.Bind(eState.End, ChangeEndState);

		if(mValue1 > 0.0f)
		{
			mCurAni = eAnimation.AttackCharge_1;
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
            ShowSkillNames(m_data);

			mState.ChangeState(eState.Jump, true);

			m_endUpdate = false;
            m_checkTime = 0.0f;
            //m_aniLength = m_owner.PlayAniImmediate(mCurAni);

            while (!m_endUpdate)
            {
				switch ((eState)mState.current)
				{
					case eState.Jump:
						if (m_owner.isFalling || m_owner.isGrounded)
						{
							mState.ChangeState(eState.AttackStart, true);
						}
						break;

					case eState.AttackStart:
						m_checkTime += m_owner.fixedDeltaTime;
						if (m_checkTime >= m_aniLength)
						{
							mState.ChangeState(eState.Falling, false);
						}
						break;

					case eState.Falling:
						if (m_owner.isGrounded == true)
						{
							mState.ChangeState(eState.AttackEnd, true);
						}
						break;

					case eState.AttackEnd:
						if (m_owner.aniEvent.IsAniPlaying(eAnimation.Jump03) == eAniPlayingState.End)
						{
							mState.ChangeState(eState.End, true);
						}
						break;

					case eState.End:
						m_endUpdate = true;
						break;
				}

				if ((eState)mState.current != eState.AttackStart)
				{
					m_owner.cmptJump.UpdateJump();
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
            Debug.LogError(mCurAni + " 공격 이벤트가 없네??");
            return 0.0f;
        }
        else if (evt.visionRange <= 0.0f)
        {
            Debug.LogError(mCurAni + " Vistion Range가 0이네??");
        }

        return evt.visionRange;
    }

	private bool ChangeJumpState(bool changeAni)
	{
		Utility.IgnorePhysics(eLayer.Player, eLayer.Enemy);
		Utility.IgnorePhysics(eLayer.Player, eLayer.EnemyGate);

		if (mTargetCollider)
		{
			Vector3 v = m_owner.transform.position;

			v = Vector3.Lerp(m_owner.transform.position, v, 0.4f);
			v.y = m_owner.transform.position.y;

			m_owner.rigidBody.position = v;
		}

		m_owner.PlayAniImmediate(eAnimation.Jump01);
		m_owner.cmptJump.StartJump(m_owner.cmptJump.m_jumpPower);

		return true;
	}

	private bool ChangeAttackStartState(bool changeAni)
	{
		Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.Enemy);
		Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.EnemyGate);

		m_aniLength = m_owner.PlayAniImmediate(mCurAni);

		if (FSaveData.Instance.AutoTargetingSkill)
		{
			Unit target = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
			if (target)
			{
				m_owner.LookAtTarget(target.transform.position);
			}
		}
		else
		{
			m_owner.cmptRotate.UpdateRotation(m_owner.Input.GetDirection(), true);
		}

		return true;
	}

	private bool ChangeFallingState(bool changeAni)
	{
		m_owner.PlayAniImmediate(eAnimation.Jump02);
		return true;
	}

	private bool ChangeAttackEndState(bool changeAni)
	{
		m_owner.PlayAniImmediate(eAnimation.Jump03);
		return true;
	}

	private bool ChangeEndState(bool changeAni)
	{
		return true;
	}
}
