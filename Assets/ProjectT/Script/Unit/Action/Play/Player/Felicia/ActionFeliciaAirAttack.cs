
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionFeliciaAirAttack : ActionAirDownAttack
{
	public override IEnumerator UpdateAction()
	{
		//WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
		while (m_endUpdate == false)
		{
			switch ((eState)m_state.current)
			{
				case eState.Start:
					if (m_owner.isGrounded)
					{
						m_state.ChangeState(eState.End, true);
					}
					break;

				case eState.End:
					m_checkTime += m_owner.fixedDeltaTime;
					if (m_checkTime >= m_aniLength)
					{
						m_endUpdate = true;
					}
					break;
			}

			m_owner.cmptJump.UpdateJump();
			yield return mWaitForFixedUpdate;
		}
	}

	protected override bool ChangeStartState(bool changeAni)
	{
		m_curAni = eAnimation.PrepareGestureSkill03;
		m_aniLength = m_owner.PlayAniImmediate(m_curAni);

		return true;
	}

	protected override bool ChangeEndState(bool changeAni)
	{
		m_aniLength = 0.0f;
		m_checkTime = 0.0f;

		if (mValue1 > 0.0f)
		{
			m_curAni = GetEndStateAni();

			AniEvent.sEvent evt = m_owner.aniEvent.GetEventOnEndAction(m_curAni);
			if (evt != null)
			{
				m_owner.OnAttackOnEndAction(evt);
			}

			m_aniLength = m_owner.PlayAniImmediate(m_curAni);
			m_checkTime = 0.0f;
		}

		return true;
	}
}
