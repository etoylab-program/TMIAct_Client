
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionWpnRinkoBrandish : ActionWeaponSkillBase
{
    private UnitCollider    mTargetCollider = null;
    private int             mAttackCount    = 3;
    private float           mDistance       = 0.0f;
    private Vector3         mDir            = Vector3.zero;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.WpnRinkoBrandish;

        extraCondition = new eActionCondition[4];
        extraCondition[0] = eActionCondition.Grounded;
        extraCondition[1] = eActionCondition.NoUsingSkill;
        extraCondition[2] = eActionCondition.NoUsingQTE;
        extraCondition[3] = eActionCondition.NoUsingUSkill;

        superArmor = Unit.eSuperArmor.Invincible;
        mCurAni = eAnimation.Brandish;
    }

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

        ActionParamFromBO paramFromBO = param as ActionParamFromBO;

        List<AniEvent.sEvent> listAtkEvent = m_owner.aniEvent.GetAllAttackEvent(mCurAni);
        for (int i = 0; i < listAtkEvent.Count; i++)
            listAtkEvent[i].atkRatio *= paramFromBO.battleOptionData.value;

        mTargetCollider = m_owner.GetMainTargetCollider(true);
        if (mTargetCollider)
        {
            m_owner.LookAtTarget(mTargetCollider.GetCenterPos());
            m_owner.SetMainTarget(mTargetCollider.Owner);
        }
    }

	public override IEnumerator UpdateAction() {
		mTargetCollider = m_owner.GetMainTargetCollider( true );
		if ( mTargetCollider ) {
			Utility.IgnorePhysics( eLayer.Player, eLayer.Enemy );
			Utility.IgnorePhysics( eLayer.Player, eLayer.EnemyGate );

			for ( int i = 0; i < mAttackCount; i++ ) {
				if ( PrepareAttack() ) {
					m_owner.OnStepForward( m_aniCutFrameLength, false, mDistance, true, 0.0f );
				}

				while ( m_checkTime < m_aniLength ) {
					m_checkTime += m_owner.fixedDeltaTime;
					yield return mWaitForFixedUpdate;
				}
			}

			yield return new WaitForSeconds( m_owner.PlayAni( eAnimation.BrandishEnd ) );
			m_endUpdate = true;
		}
		else {
			m_endUpdate = true;
			m_owner.actionSystem.CancelCurrentAction();
		}
	}

	public override void OnEnd()
    {
        base.OnEnd();

        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.Enemy);
        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.EnemyGate);
    }

    public override void OnCancel()
    {
        base.OnCancel();
        m_owner.StopStepForward();

        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.Enemy);
        Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.EnemyGate);
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
