
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSoraAttackHoldAttack : ActionSelectSkillBase {
	private eAnimation					mCurAni			= eAnimation.AttackHold;
	private List<Projectile>			mListPjt		= new List<Projectile>();
	private AniEvent.sProjectileInfo    mPjtInfo		= null;
	private AniEvent.sEvent             mAniEvt			= null;
	private int							mPjtCount		= 3;
	private List<Unit>					mListTarget		= new List<Unit>();
	private Unit						mCurTarget		= null;
	private int							mTargetIndex	= 0;
	private WaitForSeconds              mWaitForSec		= new WaitForSeconds( 1.0f );


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		actionCommand = eActionCommand.AttackDuringAttack;

		extraCondition = new eActionCondition[4];
		extraCondition[0] = eActionCondition.Grounded;
		extraCondition[1] = eActionCondition.NoUsingSkill;
		extraCondition[2] = eActionCondition.NoUsingQTE;
		extraCondition[3] = eActionCondition.NoUsingUSkill;

		cancelActionCommand = new eActionCommand[1];
		cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

		superArmor = Unit.eSuperArmor.Lv1;
		
		string pjtName = "Projectile/pjt_character_foxsora_attackhold.prefab";
		mPjtCount = Mathf.RoundToInt( mValue1 );

		if( mValue2 > 0.0f ) {
			mCurAni = eAnimation.AttackHold_1;
			pjtName = "Projectile/pjt_character_foxsora_attackhold_skill.prefab";
		}

		for( int i = 0; i < 5; i++ ) {
			Projectile pjt = GameSupport.CreateProjectile( pjtName );
			mListPjt.Add( pjt );
		}

		if( mAniEvt == null ) {
			mAniEvt = mOwnerPlayer.aniEvent.CreateEvent( eBehaviour.Projectile, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 1.0f );
		}

		if( mPjtInfo == null ) {
			mPjtInfo = mOwnerPlayer.aniEvent.CreateProjectileInfo( mListPjt[0] );
		}
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		ShowSkillNames( m_data );

		mListTarget = World.Instance.EnemyMgr.GetNearestTargetList( m_owner, true );
		mCurTarget = GetNextTargetOrNull();

		if( FSaveData.Instance.AutoTargetingSkill ) {
			if( mCurTarget ) {
				m_owner.LookAtTarget( mCurTarget.transform.position );
			}
		}
		else {
			m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
		}

		m_aniLength = m_owner.PlayAniImmediate( mCurAni );
		m_aniCutFrameLength = m_owner.aniEvent.GetCurCutFrameLength();
	}

	public override IEnumerator UpdateAction() {
		bool fire = false;

		while( !m_endUpdate ) {
			m_checkTime += m_owner.fixedDeltaTime;
			if( m_checkTime >= m_aniLength ) {
				m_endUpdate = true;
			}
			else if( !fire && m_checkTime >= m_aniCutFrameLength ) {
				fire = true;

				StopCoroutine( "UpdatePjt" );
				StartCoroutine( "UpdatePjt" );
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override float GetAtkRange() {
		return 20.0f;
	}

	private Unit GetNextTargetOrNull() {
		if( mTargetIndex >= mListTarget.Count - 1 ) {
			mTargetIndex = 0;
		}

		for( int i = mTargetIndex; i < mListTarget.Count; i++ ) {
			Unit enemy = mListTarget[i];
			if( enemy == m_owner || enemy == null || enemy.IsActivate() == false || enemy.curHp <= 0.0f || enemy.ignoreHit ) {
				continue;
			}

			mTargetIndex = i + 1;
			return mListTarget[i];
		}

		return null;
	}

	private IEnumerator UpdatePjt() {
		int index = 0;

		while( index < mPjtCount ) {
			mPjtInfo.followParentRot = true;
			mPjtInfo.addedPosition = mOwnerPlayer.GetCenterPos() - transform.position;
			mPjtInfo.notAniEventAtk = false;

			mListPjt[index].Fire( mOwnerPlayer, BattleOption.eToExecuteType.Unit, mAniEvt, mPjtInfo, mCurTarget, TableId );

			mCurTarget = GetNextTargetOrNull();
			++index;

			yield return mWaitForSec;
		}
	}
}
