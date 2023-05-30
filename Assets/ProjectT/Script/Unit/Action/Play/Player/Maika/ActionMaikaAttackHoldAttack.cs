
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionMaikaAttackHoldAttack : ActionSelectSkillBase
{
    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.AttackDuringAttack;

        conditionActionCommand = new eActionCommand[1];
        conditionActionCommand[0] = eActionCommand.Attack01;

        extraCondition = new eActionCondition[5];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

		extraCancelCondition = new eActionCondition[3];
		extraCancelCondition[0] = eActionCondition.UseSkill;
		extraCancelCondition[1] = eActionCondition.UseQTE;
		extraCancelCondition[2] = eActionCondition.UseUSkill;
	}

	public override void OnStart(IActionBaseParam param)
	{
		if(mValue2 >= 1.0f)//
		{
			superArmor = Unit.eSuperArmor.Lv2;
		}
		else
		{
			superArmor = Unit.eSuperArmor.Lv1;
		}

		base.OnStart(param);
		ShowSkillNames(m_data);

		eAnimation curAni = eAnimation.AttackHold;

		ActionParamMaikaAttackDuringAttack paramAtkDuringAtk = param as ActionParamMaikaAttackDuringAttack;
		if(paramAtkDuringAtk != null)
		{
			if(paramAtkDuringAtk.AttackIndex == 3)
			{
				curAni = eAnimation.AttackHold_2;
			}
			else if(paramAtkDuringAtk.AttackIndex > 0)
			{
				curAni = eAnimation.AttackHold_1;
			}
		}

		if (mValue1 > 0.0f)
		{
			List<Projectile> list = m_owner.aniEvent.GetAllProjectile(curAni);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					list[i].SetProjectileAtkAttr(Projectile.EProjectileAtkAttr.STUN);
				}
			}
		}

		m_aniLength = m_owner.PlayAniImmediate(curAni);

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
	}

	public override IEnumerator UpdateAction()
	{
		Unit target = null;
		while (!m_endUpdate)
		{
			m_checkTime += m_owner.fixedDeltaTime;
			if (m_checkTime >= m_aniLength)
			{
				m_endUpdate = true;
			}
			else
			{
				if (World.Instance.StageType == eSTAGETYPE.STAGE_PVP)
				{
					target = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
					if (target)
					{
						m_owner.LookAtTarget(target.transform.position);
					}
				}
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override float GetAtkRange()
    {
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(eAnimation.AttackHold);
		if (evt == null)
		{
			Debug.LogError(eAnimation.AttackHold + "공격 이벤트가 없네??");
			return 0.0f;
		}
		else if (evt.visionRange <= 0.0f)
		{
			Debug.LogError(eAnimation.AttackHold + "Vistion Range가 0이네??");
		}

		return evt.visionRange;
	}
}
