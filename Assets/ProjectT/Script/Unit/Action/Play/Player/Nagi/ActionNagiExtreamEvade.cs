
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionNagiExtreamEvade : ActionExtreamEvade {
	private List<Projectile>				mPjtList		= new List<Projectile>( 4 );
	private AniEvent.sEvent					mAniEvt         = null;
	private List<AniEvent.sProjectileInfo>	mPjtInfoList	= new List<AniEvent.sProjectileInfo>( 4 );


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		mAniEvt = m_owner.aniEvent.CreateEvent( eBehaviour.Projectile, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 0.0f );

		for( int i = 0; i < 4; i++ ) {
			Projectile pjt = null;

			if( mValue2 > 0.0f ) {
				pjt = GameSupport.CreateProjectile( "Projectile/pjt_character_nagi_extream_evade_1.prefab" );
				mAniEvt.atkRatio = 0.9f;
			}
			else {
				pjt = GameSupport.CreateProjectile( "Projectile/pjt_character_nagi_extream_evade.prefab" );
				mAniEvt.atkRatio = 1.5f;
			}

			pjt.child.OnHitFunc = OnProjectileHit;
			mPjtList.Add( pjt );

			AniEvent.sProjectileInfo info = m_owner.aniEvent.CreateProjectileInfo( pjt );
			info.attach = false;
			info.boneName = "";
			info.followParentRot = false;

			mPjtInfoList.Add( info );
		}
	}

	public override void OnStart( IActionBaseParam param ) {
		BOCharSkill.ChangeBattleOptionDuration( BattleOption.eBOTimingType.DuringSkill, TableId, mValue1 );

		base.OnStart( param );
		StartEvadingBuff();
	}

	private void StartEvadingBuff() {
		mOwnerPlayer.ExtreamEvading = true;

		StopCoroutine( "EndEvadingBuff" );
		StartCoroutine( "EndEvadingBuff" );
	}

	public override void OnEnd() {
		base.OnEnd();
		IsSkillEnd = false;
	}

	private IEnumerator EndEvadingBuff() {
		UnitCollider targetCollider = m_owner.GetMainTargetCollider( true );
		if( targetCollider != null ) {
			for( int i = 0; i < 4; i++ ) {
				Vector3 pos = Vector3.zero;
				float interval = 2.5f;

				if( i == 0 ) {
					pos = -( m_owner.transform.right * interval ) + ( m_owner.transform.forward * interval );
				}
				else if( i == 1) {
					pos = ( m_owner.transform.right * interval ) + ( m_owner.transform.forward * interval );
				}
				else if( i == 2) {
					pos = ( m_owner.transform.right * interval ) - ( m_owner.transform.forward * interval );
				}
				else {
					pos = -( m_owner.transform.right * interval ) - ( m_owner.transform.forward * interval );
				}

				mPjtInfoList[i].addedPosition = pos;
				mPjtList[i].Fire( m_owner, BattleOption.eToExecuteType.Unit, mAniEvt, mPjtInfoList[i], targetCollider.Owner, TableId );
			}
		}

		yield return StartCoroutine( ContinueDash() );

		mOwnerPlayer.ExtreamEvading = false;
		IsSkillEnd = true;
	}

	private bool OnProjectileHit( Unit target ) {
		if( mValue1 <= 0.0f || target == null ) {
			return false;
		}

		mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
		mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.None;

		mBuffEvt.Set( 0, eEventSubject.HitTargetList, eEventType.EVENT_DEBUFF_DOT_DMG, mOwnerPlayer, 0.6f, (float)EAttackAttr.FIRE, 
					  0.0f, 4.0f, 1.0f, 0, 0, eBuffIconType.Debuff_Flame );

		EventMgr.Instance.SendEvent( mBuffEvt );

		return true;
	}
}
