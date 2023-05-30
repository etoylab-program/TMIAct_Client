
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionKiraraExtreamEvade : ActionExtreamEvade {
	private ParticleSystem				mPsStart	= null;
	private Projectile					mPjt        = null;
	private AniEvent.sEvent				mAniEvt     = null;
	private AniEvent.sProjectileInfo	mPjtInfo    = null;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		mPsStart = GameSupport.CreateParticle( "Effect/Character/prf_fx_kirara_evasion.prefab", null );

		mAniEvt = m_owner.aniEvent.CreateEvent( eBehaviour.Projectile, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 0.4f );
		mPjt = GameSupport.CreateProjectile( "Projectile/pjt_character_wskill_kirara_cake.prefab" );

		mPjtInfo = m_owner.aniEvent.CreateProjectileInfo( mPjt );
		mPjtInfo.attach = false;
		mPjtInfo.boneName = "";
		mPjtInfo.followParentRot = false;
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		StartEvadingBuff();
	}

	private void StartEvadingBuff() {
		mOwnerPlayer.ExtreamEvading = true;

		mPsStart.transform.position = m_owner.transform.position;
		mPsStart.gameObject.SetActive( true );

		float duration = GameInfo.Instance.BattleConfig.BuffDuration + mValue3;

		StopCoroutine( "EndEvadingBuff" );
		StartCoroutine( "EndEvadingBuff", duration );

		// 속도 감소
		mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
		mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.Each_Monster_Normal_Hit;
		mBuffEvt.Set( m_data.ID, eEventSubject.ActiveEnemies, eEventType.EVENT_DEBUFF_SPEED_DOWN, m_owner, mValue1, 0.0f, 0.0f,
					 duration, 0.0f, 0, mBuffEvt.effId2, eBuffIconType.Debuff_Speed );

		EventMgr.Instance.SendEvent( mBuffEvt );

		// 방어력 감소
		mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
		mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.Each_Monster_Normal_Hit;
		mBuffEvt.Set( GetInstanceID(), eEventSubject.ActiveEnemies, eEventType.EVENT_DEBUFF_DMG_RATE_UP, m_owner, mValue2, 0.0f, 0.0f,
					 duration, 0.0f, 0, mBuffEvt.effId2, eBuffIconType.Debuff_Def );

		EventMgr.Instance.SendEvent( mBuffEvt );

		if( SetAddAction ) {
			if( m_owner.curSp >= AddActionValue2 ) {
				Unit target = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
				mAniEvt.atkRatio = AddActionValue1;

				if( target ) {
					m_owner.UseSp( AddActionValue2 );
					mPjt.Fire( m_owner, BattleOption.eToExecuteType.Unit, mAniEvt, mPjtInfo, target, TableId );
				}
			}
		}
	}

	public override void OnEnd() {
		base.OnEnd();
		IsSkillEnd = false;
	}

	private IEnumerator EndEvadingBuff( float duration ) {
		yield return StartCoroutine( ContinueDash() );
		yield return new WaitForSeconds( duration );

		mOwnerPlayer.ExtreamEvading = false;
		IsSkillEnd = true;

		mPsStart.gameObject.SetActive( false );
	}
}
