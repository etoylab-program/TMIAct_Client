
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionShiranuiEvadeSummon : ActionExtreamEvade
{
    private float			mElectrostaticInterval		= 0.8f;
    private float			mElectrostaticDamageRatio	= 0.7f;
    private float			mElectrostaticRange			= 8.0f;
	private AniEvent.sEvent	mAniEvt						= null;
    private float           mSkillAttackRatio           = 0.0f;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        mElectrostaticDamageRatio += mValue1;
        mElectrostaticRange += mValue2;
        mElectrostaticInterval += mValue3;
	}

    public override void OnStart(IActionBaseParam param)
    {
        base.OnStart(param);

		if(mAniEvt == null)
		{
			mAniEvt = m_owner.aniEvent.CreateEvent(eBehaviour.Attack, 205, eHitDirection.None, "", Vector3.zero, Vector3.zero, Vector3.zero);
		}

        PrepareElectrostaticAttack();

		mSkillAttackRatio = 0.0f;

		if ( SetAddAction ) {
			mSkillAttackRatio = AddActionValue1;
		}
	}

    private void PrepareElectrostaticAttack()
    {
        float duration = GameInfo.Instance.BattleConfig.BuffDuration;
        EffectManager.Instance.Play(m_owner, 50012, EffectManager.eType.Common, duration);

        StopCoroutine("StartElectrostaticAttack");
        StartCoroutine("StartElectrostaticAttack", duration);
    }

	private IEnumerator StartElectrostaticAttack( float duration ) {
		yield return StartCoroutine( ContinueDash() );

		float attackCheckTime = mElectrostaticInterval;
		m_checkTime = 0.0f;
		mAniEvt.isRangeAttack = true;

		while ( true ) {
			m_checkTime += m_owner.fixedDeltaTime;
			if ( m_checkTime >= duration ) {
				mOwnerPlayer.ExtreamEvading = false;
				EffectManager.Instance.StopEffImmediate( 50012, EffectManager.eType.Common, null );

				break;
			}
			else {
				attackCheckTime += m_owner.fixedDeltaTime;
				if ( attackCheckTime >= mElectrostaticInterval ) {
					attackCheckTime = 0.0f;

					List<UnitCollider> listTargetCollider = m_owner.GetTargetColliderListByAround( m_owner.transform.position, mElectrostaticRange );
					if ( listTargetCollider != null && listTargetCollider.Count > 0 ) {
						World.Instance.EnemyMgr.SortListTargetByNearDistance( m_owner, ref listTargetCollider );

						if ( listTargetCollider.Count > 0 ) {
							mAtkEvt.Set( eEventType.EVENT_BATTLE_ON_DIRECT_HIT, m_owner, BattleOption.eToExecuteType.Unit, mAniEvt,
										 mOwnerPlayer.attackPower * mElectrostaticDamageRatio + mOwnerPlayer.attackPower * mSkillAttackRatio, 
										 eAttackDirection.Skip, false, 0, EffectManager.eType.None, listTargetCollider, 0.0f, false, false, false, 
										 TableId, null );

							EventMgr.Instance.SendEvent( mAtkEvt );
						}
					}
				}
			}

			yield return mWaitForFixedUpdate;
		}
	}
}
