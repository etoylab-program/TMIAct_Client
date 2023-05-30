
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSpidermistressWeb : ActionEnemyBase {
	private eAnimation			mCurAni						= eAnimation.Attack04;
	private List<Projectile>	mPjtList					= null;
	private List<BoxCollider>	mBoxFloorList				= null;
	private List<Unit>			mListStayOnBoxColliderEnemy	= new List<Unit>();
	private float				mDuration					= 0.0f;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );
		actionCommand = eActionCommand.Attack04;
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		if ( mPjtList == null ) {
			mPjtList = m_owner.aniEvent.GetAllProjectile( mCurAni );
			mBoxFloorList = new List<BoxCollider>( mPjtList.Count );

			for ( int i = 0; i < mPjtList.Count; i++ ) {
				mBoxFloorList.Add( mPjtList[i].BoxCol );
				mDuration = mPjtList[i].duration;
			}
		}

		m_aniLength = m_owner.PlayAniImmediate( mCurAni );
	}

	public override IEnumerator UpdateAction() {
		while ( !m_endUpdate ) {
			m_checkTime += Time.fixedDeltaTime;
			if ( m_checkTime >= m_aniLength ) {
				StopCoroutine( "UpdateFloor" );
				StartCoroutine( "UpdateFloor" );

				m_endUpdate = true;
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override float GetAtkRange() {
		return 7.0f;
	}

	private IEnumerator UpdateFloor() {
		BuffEvent buffEvt2 = new BuffEvent();
		WorldPVP worldPVP = World.Instance as WorldPVP;
		List<Unit> listEnemy = null;

		m_checkTime = 0.0f;
		mListStayOnBoxColliderEnemy.Clear();

		while ( true ) {
			m_checkTime += m_owner.fixedDeltaTime;
			if ( m_checkTime >= mDuration || ( worldPVP && ( !worldPVP.IsBattleStart || m_owner.curHp <= 0.0f ) ) ) {
				for ( int i = 0; i < mListStayOnBoxColliderEnemy.Count; i++ ) {
					mListStayOnBoxColliderEnemy[i].StayOnBoxCollider = null;
				}

				yield break;
			}

			listEnemy = m_owner.GetEnemyList( true );
			for ( int i = 0; i < listEnemy.Count; i++ ) {
				Unit enemy = listEnemy[i];
				if ( enemy.curHp <= 0.0f ) {
					continue;
				}

				for ( int j = 0; j < mBoxFloorList.Count; j++ ) {
					bool isEnemyInBox = mBoxFloorList[j].bounds.Contains( enemy.transform.position );
					if ( isEnemyInBox && enemy.StayOnBoxCollider == null ) {
						enemy.StayOnBoxCollider = mBoxFloorList[j];
						mListStayOnBoxColliderEnemy.Add( enemy );

						// 속도 감소
						mBuffEvt.battleOptionData.buffDebuffType = eBuffDebuffType.Debuff;
						mBuffEvt.battleOptionData.effType = (int)EffectManager.eType.Each_Monster_Normal_Hit;
						mBuffEvt.battleOptionData.conditionType = BattleOption.eBOConditionType.StayOnBoxCollider;
						mBuffEvt.battleOptionData.buffIconFlash = false;

						mBuffEvt.Set( 0, eEventSubject.Self, eEventType.EVENT_DEBUFF_SPEED_DOWN, enemy, 0.2f, 0.0f, 0.0f,
									 mDuration, 0.0f, mBuffEvt.effId, mBuffEvt.effId2, eBuffIconType.Debuff_Speed );

						EventMgr.Instance.SendEvent( mBuffEvt );
					}
					else if ( !isEnemyInBox && enemy.StayOnBoxCollider ) {
						enemy.StayOnBoxCollider = null;
						mListStayOnBoxColliderEnemy.Remove( enemy );
					}
				}
			}

			yield return mWaitForFixedUpdate;
		}
	}
}
