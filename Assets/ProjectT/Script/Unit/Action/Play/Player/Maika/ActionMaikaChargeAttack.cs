
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionMaikaChargeAttack : ActionChargeAttack
{
	private List<eAnimation>					mListChargeAtkAni			= new List<eAnimation>(3);
	private ActionParamMaikaAttackDuringAttack	mActionParamAtkDuringAtk	= new ActionParamMaikaAttackDuringAttack();
	private int									mChargeCount				= 0;
	private float								mCheckAniTime				= 0.0f;


	public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
	{
		base.Init(tableId, listAddCharSkillParam);

		superArmor = Unit.eSuperArmor.Lv2;

		cancelActionCommand = new eActionCommand[1];
		cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

		mListChargeAtkAni.Add(eAnimation.AttackCharge_2);
		mListChargeAtkAni.Add(eAnimation.AttackCharge_3);
		mListChargeAtkAni.Add(eAnimation.AttackCharge_4);

		m_listPSCharge.Add(GameSupport.CreateParticle("Effect/Character/prf_fx_maika_charge_step_01.prefab", m_owner.transform));
		m_listPSCharge.Add(GameSupport.CreateParticle("Effect/Character/prf_fx_maika_charge_step_02.prefab", m_owner.transform));
		m_listPSCharge.Add(GameSupport.CreateParticle("Effect/Character/prf_fx_maika_charge_step_03.prefab", m_owner.transform));
		m_listPSCharge.Add(GameSupport.CreateParticle("Effect/Character/prf_fx_maika_charge_step_end.prefab", m_owner.transform));

		for (int i = 0; i < mListChargeAtkAni.Count; i++)
		{
			List<AniEvent.sEvent> listAtkEvt = m_owner.aniEvent.GetAllAttackEvent(mListChargeAtkAni[i]);
			for (int j = 0; j < listAtkEvt.Count; j++)
			{
				listAtkEvt[j].atkRatio += listAtkEvt[j].atkRatio * (mValue1 / (float)eCOUNT.MAX_BO_FUNC_VALUE);
			}
		}

		IsNormalAttack = true;
	}

	public override void OnStart(IActionBaseParam param)
	{
		base.OnStart(param);

		mbStartAttack = false;
		mDir = Vector3.zero;

		if (m_data != null && SkipBOExecuteOnStart)
		{
			ExecuteStartSkillBO();
		}
		
		StopAllEffCharge();
		m_owner.PlayAniImmediate(eAnimation.AttackCharge);
	}

	public override IEnumerator UpdateAction()
	{
		UnitCollider targetCollider = m_owner.GetMainTargetCollider(true);

		float chargingTime = 0.0f;
		float chargeTime = 0.5f;
		mChargeCount = 0;

		while (!m_endUpdate)
		{
			if (mChargeCount < 3)
			{
				chargingTime += m_owner.fixedDeltaTime;
				if (chargingTime >= chargeTime)
				{
					PlayEffCharge(mChargeCount);

					chargingTime = 0.0f;

					++mChargeCount;
					if (mChargeCount == 1)
					{
						m_owner.PlayAniImmediate(eAnimation.AttackCharge_1);
					}
				}
			}

			if (FSaveData.Instance.AutoTargetingSkill)
			{
				if (targetCollider)
				{
					if(targetCollider.Owner.curHp <= 0.0f)
					{
						targetCollider = m_owner.GetMainTargetCollider(true);
					}

					if (targetCollider)
					{
						m_owner.LookAtTarget(targetCollider.GetCenterPos());
					}
				}
			}
			else
			{
				m_owner.cmptRotate.UpdateRotation(m_owner.Input.GetDirection(), true);
			}

			yield return mWaitForFixedUpdate;
		}

		StopAllEffCharge();
		mbStartAttack = true;

		if (mChargeCount <= 0)
		{
			SetNextAction(eActionCommand.Attack01, null);
		}
		else
		{
			ShowSkillNames(m_data);
			PlayEffCharge(3);
			
			m_aniLength = m_owner.PlayAniImmediate(mListChargeAtkAni[mChargeCount - 1]);
			
			m_endUpdate = false;
			m_checkTime = 0.0f;
			mCheckAniTime = m_aniLength;

			while (!m_endUpdate)
			{
				m_checkTime += m_owner.fixedDeltaTime;
				if (m_checkTime >= mCheckAniTime)
				{
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
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(mListChargeAtkAni[0]);
		if (evt == null)
		{
			Debug.LogError(mListChargeAtkAni[0] + " 공격 이벤트가 없네??");
			return 0.0f;
		}
		else if (evt.visionRange <= 0.0f)
		{
			Debug.LogError(mListChargeAtkAni[0] + " Vistion Range가 0이네??");
		}

		return evt.visionRange;
	}

	public void SetNextAttackDuringAttackAction()
	{
		mCheckAniTime = m_owner.aniEvent.GetCurCutFrameLength();

		mActionParamAtkDuringAtk.SetAttackIndex(mChargeCount);
		SetNextAction(eActionCommand.AttackDuringAttack, mActionParamAtkDuringAtk);
	}
}
