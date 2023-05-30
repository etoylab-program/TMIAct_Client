
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionRandomAttack : ActionComboAttack
{
	private float mCheckEndUpdateTime = 0.0f;


	public override IEnumerator UpdateAction()
	{
		mCheckEndUpdateTime = m_owner.aniEvent.GetAniLength(m_currentAni);

		while (m_endUpdate == false)
		{
			m_checkTime += m_owner.fixedDeltaTime;

			if ((m_owner.AI && !IsLastAttack()) || m_nextAttackIndex > CurrentAttackIndex)
			{
				if (m_checkTime >= m_owner.aniEvent.GetCutFrameLength(m_currentAni))
				{
					if (m_owner.AI)
					{
						bool skip = false;
						if (m_owner.mainTarget)
						{
							float dist = Utility.GetDistanceWithoutY(m_owner.transform.position, m_owner.mainTarget.transform.position);
							if (dist >= GetAtkRange() * 1.5f)
							{
								dist = Utility.GetDistanceWithoutY(m_owner.transform.position, m_owner.GetTargetCapsuleEdgePos(m_owner.mainTarget));
								if (dist >= GetAtkRange() * 1.5f)
								{
									skip = true;
								}
							}
						}

						if (!skip)
						{
							OnUpdating(null);
						}
					}

					m_endUpdate = true;
				}
			}
			else
			{
				if (m_checkTime >= mCheckEndUpdateTime)
				{
					m_endUpdate = true;
				}
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override bool IsLastAttack()
	{
		return true;
	}

	public override float GetAtkRange()
	{
		return 20.0f;
	}

	public void ChangeEndUpdateTime()
	{
		mCheckEndUpdateTime = m_owner.aniEvent.GetCutFrameLength(m_currentAni);

		if(m_checkTime < mCheckEndUpdateTime)
		{
			m_checkTime = 0.0f;
		}
	}

	protected override eAnimation GetCurAni()
    {
		CurrentAttackIndex = 0;
		return attackAnimations[Random.Range(0, attackAnimations.Length)];
    }

	protected override void StartAttack()
	{
		m_currentAni = GetCurAni();
		m_aniLength = m_owner.PlayAni(m_currentAni);

		m_atkRange = m_owner.aniEvent.GetFirstAttackEventRange(m_currentAni);
		m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(m_currentAni);

		if (mLastAtkSuperArmor > Unit.eSuperArmor.None && IsLastAttack())
		{
			mChangedSuperArmorId = m_owner.SetSuperArmor(mLastAtkSuperArmor);
		}
	}
}
