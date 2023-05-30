
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionEnemyAsagiBrandish : ActionEnemyTaimaninBase
{
    private Vector3 mDir        = Vector3.zero;
    private float   mDistance   = 0.0f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.ChargingAttack2;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        mTargetCollider = m_owner.GetMainTargetCollider(true);
        if (mTargetCollider)
        {
            m_owner.LookAtTarget(mTargetCollider.GetCenterPos());
        }
    }

	public override IEnumerator UpdateAction() {
		if ( PrepareAttack() ) {
			m_owner.OnStepForward( m_aniCutFrameLength, false, mDistance, true, 0.0f );
		}

		while ( m_checkTime < m_aniLength ) {
			m_checkTime += m_owner.fixedDeltaTime;
			yield return mWaitForFixedUpdate;
		}
	}

	private bool PrepareAttack()
    {
        if (mTargetCollider == null)
        {
            m_aniLength = m_owner.PlayAniImmediate(eAnimation.ChargeBrandish);
            return false;
        }

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
        {
            mDistance = hitInfo.distance <= 1.0f ? 0.0f : hitInfo.distance;
        }

        m_owner.LookAtTarget(dest);

        m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength(eAnimation.ChargeBrandish);
        m_aniLength = m_owner.PlayAniImmediate(eAnimation.ChargeBrandish);
        m_checkTime = 0.0f;

        return true;
    }
}
