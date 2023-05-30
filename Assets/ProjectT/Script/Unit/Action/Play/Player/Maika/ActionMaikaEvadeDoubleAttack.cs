
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionMaikaEvadeDoubleAttack : ActionSelectSkillBase
{
    private Unit    mTarget		= null;
	private float	mDuration	= 3.0f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.Teleport;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        extraCancelCondition = new eActionCondition[3];
        extraCancelCondition[0] = eActionCondition.UseSkill;
        extraCancelCondition[1] = eActionCondition.UseQTE;
        extraCancelCondition[2] = eActionCondition.UseUSkill;

        superArmor = Unit.eSuperArmor.Lv2;

		mDuration = mValue1;
	}

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);
        ShowSkillNames(m_data);

        m_aniLength = m_owner.PlayAniImmediate(eAnimation.EvadeDouble);

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
    }

    public override IEnumerator UpdateAction()
    {
		bool loopAni = false;

        while (m_endUpdate == false)
        {
            m_checkTime += m_owner.fixedDeltaTime;
            if (m_checkTime >= mDuration)
            {
                m_endUpdate = true;
            }
			else if(!loopAni && m_checkTime >= m_aniLength)
			{
				loopAni = true;
				m_owner.PlayAniImmediate(eAnimation.EvadeDouble_1);
			}

			/*
			if (mTarget && mTarget.curHp <= 0.0f)
			{
				LookAtTarget();
			}
			else if(mTarget == null)
			{
				LookAtTarget();
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

		m_owner.PlayAniImmediate(eAnimation.EvadeDouble_2);
		yield return new WaitForSeconds(m_owner.aniEvent.GetCutFrameLength(eAnimation.EvadeDouble_2));
	}

    public override float GetAtkRange()
    {
        AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent(eAnimation.EvadeDouble_1);
        if (evt == null)
        {
            Debug.LogError(eAnimation.EvadeDouble_1 + "공격 이벤트가 없네??");
            return 0.0f;
        }
        else if (evt.visionRange <= 0.0f)
        {
            Debug.LogError(eAnimation.EvadeDouble_1 + "Vistion Range가 0이네??");
        }

        return evt.visionRange;
    }

	private void LookAtTarget()
	{
		mTarget = World.Instance.EnemyMgr.GetNearestTarget(m_owner, true);
		if (mTarget)
		{
			if (FSaveData.Instance.AutoTargetingSkill)
			{
				m_owner.LookAtTarget(mTarget.transform.position);
			}
			else
			{
				m_owner.cmptRotate.UpdateRotation(m_owner.Input.GetDirection(), true);
			}
		}
	}
}
