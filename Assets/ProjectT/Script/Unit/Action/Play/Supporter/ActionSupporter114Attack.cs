
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSupporter114Attack : ActionSupporterSkillBase {

	private Projectile					mPjt		= null;
	private AniEvent.sProjectileInfo	mPjtInfo	= null;
	private AniEvent.sEvent				mAniEvt		= null;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );
		actionCommand = eActionCommand.Supporter114Attack;

		mPjt = GameSupport.CreateProjectile( "Projectile/pjt_supporter_114.prefab" );
		mPjt.OnHitFunc = OnProjectileHit;
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		mParamFromBO = param as ActionParamFromBO;

		if ( mAniEvt == null ) {
			mAniEvt = mOwnerPlayer.aniEvent.CreateEvent( eBehaviour.Projectile, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 1.0f );
		}

		if ( mPjtInfo == null ) {
			mPjtInfo = mOwnerPlayer.aniEvent.CreateProjectileInfo( mPjt );
		}

		UnitCollider targetCollider = m_owner.GetMainTargetCollider( true );
		if ( targetCollider ) {
			mPjtInfo.followParentRot = true;
			mPjtInfo.addedPosition = mOwnerPlayer.GetCenterPos() - transform.position;
			mPjtInfo.notAniEventAtk = true;

			mAniEvt.atkRatio = mParamFromBO.battleOptionData.value;

			mPjt.Fire( mOwnerPlayer, BattleOption.eToExecuteType.Supporter, mAniEvt, mPjtInfo, targetCollider.Owner, -1 );
		}
	}

	private bool OnProjectileHit( Unit hitTarget ) {
		if ( mParamFromBO.battleOptionData.addCallTiming == BattleOption.eBOAddCallTiming.OnSend ) {
			if ( mParamFromBO.battleOptionData.timingType == BattleOption.eBOTimingType.Use &&
				mParamFromBO.battleOptionData.dataOnEndCall.conditionType == BattleOption.eBOConditionType.ComboCountAsValue ) {
				mParamFromBO.battleOptionData.dataOnEndCall.evt.value = mParamFromBO.battleOptionData.evt.value;
			}

			EffectManager.Instance.Play( m_owner, mParamFromBO.battleOptionData.dataOnEndCall.startEffId, (EffectManager.eType)mParamFromBO.battleOptionData.dataOnEndCall.effType );

			mParamFromBO.battleOptionData.dataOnEndCall.useTime = System.DateTime.Now;
			EventMgr.Instance.SendEvent( mParamFromBO.battleOptionData.dataOnEndCall.evt );

			Log.Show( mParamFromBO.battleOptionData.dataOnEndCall.evt.battleOptionData.battleOptionSetId + "번 배틀옵션셋 사용 (애드콜)!!!", Log.ColorType.Green );
		}

		return true;
	}
}
