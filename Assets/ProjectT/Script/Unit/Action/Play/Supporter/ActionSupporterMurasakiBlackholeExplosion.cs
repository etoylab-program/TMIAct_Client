
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSupporterMurasakiBlackholeExplosion : ActionSupporterSkillBase {

	private List<Unit>      mListTarget     = null;
	private float           mHitInterval    = 0.25f;
	private float           mHitDistance    = 2.0f;
	private bool            mExplosion      = false;
	private Vector3         mBlackholePos   = Vector3.zero;
	private float           mAttackPower    = 0.0f;
	private float           mExplosionPower = 0.0f;
	private AniEvent.sEvent mAniEvt         = null;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );
		actionCommand = eActionCommand.SupporterMurasakiBlackholeExplosion;
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		mParamFromBO = param as ActionParamFromBO;

		if( mAniEvt == null ) {
			mAniEvt = m_owner.aniEvent.CreateEvent( eBehaviour.Attack, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 1.0f );
		}

		if( mParamFromBO.battleOptionData.effId1 > 0 ) {
			mBlackholePos = m_owner.transform.position + ( m_owner.transform.forward * 1.25f );
			mBlackholePos.y = m_owner.transform.position.y;

			EffectManager.Instance.Play( mBlackholePos, mParamFromBO.battleOptionData.effId1, (EffectManager.eType)mParamFromBO.battleOptionData.effType );
		}

		mExplosion = false;
		mListTarget = m_owner.GetEnemyList( true );

		if( mListTarget != null && mListTarget.Count > 0 ) {
			for( int i = 0; i < mListTarget.Count; i++ ) {
				Unit target = mListTarget[i];

				if( target.CurrentSuperArmor >= Unit.eSuperArmor.Lv2 || target.IsImmuneFloat() || target.IsImmuneKnockback() ) {
					continue;
				}

				target.actionSystem.CancelCurrentAction();
				target.StopStepForward();
			}

			mAttackPower = m_owner.attackPower * mParamFromBO.battleOptionData.value;
			mExplosionPower = mAttackPower + ( mAttackPower * mParamFromBO.battleOptionData.value2 );
		}
	}

	public override IEnumerator UpdateAction() {
		if( mListTarget == null || mListTarget.Count <= 0 ) {
			yield break;
		}

		float checkHitTime = 0.0f;
		bool hit = false;

		while( !mExplosion ) {
			if( m_checkTime >= 3.0f || Director.IsPlaying || World.Instance.IsEndGame || World.Instance.ProcessingEnd ) {
				mExplosion = true;
			}
			else {
				checkHitTime += m_owner.fixedDeltaTime;

				if( checkHitTime >= mHitInterval ) {
					m_checkTime += checkHitTime;

					checkHitTime = 0.0f;
					hit = true;
				}

				for( int i = 0; i < mListTarget.Count; i++ ) {
					Unit target = mListTarget[i];

					if( target == null || target.MainCollider == null || target.curHp <= 0.0f || target.cmptMovement == null ) {
						continue;
					}

					if( hit && Vector3.Distance( m_owner.GetTargetCapsuleEdgePos( target ), mBlackholePos ) <= mHitDistance ) {
						mAtkEvt.SetWithSingleTarget( eEventType.EVENT_BATTLE_ON_DIRECT_HIT, m_owner, BattleOption.eToExecuteType.Supporter, mAniEvt,
													 mAttackPower, eAttackDirection.Skip, false, 0, EffectManager.eType.None, target.MainCollider, 0.0f, true );

						EventMgr.Instance.SendEvent( mAtkEvt );
					}

					if( target.CurrentSuperArmor >= Unit.eSuperArmor.Lv2 || target.IsImmuneFloat() || target.IsImmuneKnockback() ) {
						continue;
					}

					target.StopStepForward();

					Vector3 v = ( mBlackholePos - target.transform.position ).normalized;
					v.y = 0.0f;

					target.cmptMovement.UpdatePosition( v, Mathf.Max( GameInfo.Instance.BattleConfig.BlackholeMinSpeed, target.speed * 1.2f ), false );
				}

				hit = false;
			}

			yield return mWaitForFixedUpdate;
		}

		if( World.Instance.IsEndGame || World.Instance.ProcessingEnd ) {
			EffectManager.Instance.StopEffImmediate( mParamFromBO.battleOptionData.effId1, (EffectManager.eType)mParamFromBO.battleOptionData.effType, null );
		}
		else if( mExplosion ) {
			if( mParamFromBO.battleOptionData.effId2 > 0 ) {
				EffectManager.Instance.Play( mBlackholePos, mParamFromBO.battleOptionData.effId2, (EffectManager.eType)mParamFromBO.battleOptionData.effType );
			}

			mAtkEvt.Set( eEventType.EVENT_BATTLE_ON_DIRECT_HIT, m_owner, BattleOption.eToExecuteType.Supporter, mAniEvt, mExplosionPower, eAttackDirection.Skip,
						false, 0, EffectManager.eType.None, m_owner.GetEnemyColliderList(), 0.0f, true );

			EventMgr.Instance.SendEvent( mAtkEvt );
		}
	}

	public override void OnCancel() {
		base.OnCancel();

		m_endUpdate = true;
		m_checkTime = 3.0f;
	}
}
