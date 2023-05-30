
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionAstarothAttackHoldAttack : ActionSelectSkillBase {
	private Projectile[]                mArrPjt			= null;
	private Projectile[]                mArrPjt2		= null;
	private AniEvent.sProjectileInfo[]  mArrPjtInfo		= null;
	private AniEvent.sEvent             mAniEvt			= null;
	private int                         mPjtCount		= 0;
	private bool						mAddBOSetOnce	= false;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );
		actionCommand = eActionCommand.AttackDuringAttack;

		conditionActionCommand = new eActionCommand[1];
		conditionActionCommand[0] = eActionCommand.Attack01;

		extraCondition = new eActionCondition[5];
		extraCondition[0] = eActionCondition.Grounded;
		extraCondition[1] = eActionCondition.NoUsingSkill;
		extraCondition[2] = eActionCondition.NoUsingQTE;
		extraCondition[3] = eActionCondition.NoUsingUSkill;

		cancelActionCommand = new eActionCommand[1];
		cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

		mPjtCount = (int)mValue1;

		mArrPjt = new Projectile[mPjtCount];
		for( int i = 0; i < mPjtCount; i++ ) {
			mArrPjt[i] = GameSupport.CreateProjectile( "Projectile/pjt_character_astaroth_firesnake.prefab" );
		}

		mArrPjt2 = new Projectile[mPjtCount];
		for( int i = 0; i < mPjtCount; i++ ) {
			mArrPjt2[i] = GameSupport.CreateProjectile( "Projectile/pjt_character_astaroth_firesnake_02.prefab" );
		}

		mAniEvt = m_owner.aniEvent.CreateEvent( eBehaviour.Projectile, 0, eHitDirection.None, eAttackDirection.Skip, 0.0f, 0.0f, 0.4f );
		mAniEvt.atkRatio = mValue2;

		mArrPjtInfo = new AniEvent.sProjectileInfo[mPjtCount];
		for( int i = 0; i < mArrPjtInfo.Length; i++ ) {
			mArrPjtInfo[i] = m_owner.aniEvent.CreateProjectileInfo( mArrPjt[i] );
			mArrPjtInfo[i].attach = false;
			mArrPjtInfo[i].boneName = Random.Range( 0, 2 ) == 0 ? "Bip001 R Hand" : "Bip001 L Hand";
			mArrPjtInfo[i].followParentRot = true;
		}
	}

	public override float GetAtkRange() {
		return 20.0f;
	}

	public void ManuallyStart() {
		UnitCollider targetCollider = m_owner.GetMainTargetCollider(true);
		if( targetCollider == null ) {
			return;
		}

		//==
		// 스킬 구조가 특이해서 하드코딩함
		SetAddAction = false;
		WeaponData wpnData = mOwnerPlayer.GetCurrentWeaponDataOrNull();
		if( wpnData != null && wpnData.TableData.ID == 15017 && mOwnerPlayer.boWeapon != null ) {
			if( !mAddBOSetOnce) {
				AddSkillInfoByBOSetId( 30000108 );
				BOCharSkill.ChangeBattleOptionSetRandomStart( 30000108, Mathf.RoundToInt( AddActionValue1 * (float)eCOUNT.MAX_BO_FUNC_VALUE ) );

				mAddBOSetOnce = true;
			}

			SetAddAction = true;
		}

		if( !SetAddAction ) {
			BOCharSkill.ChangeBattleOptionSetRandomStart( 30000108, 100 ); // SetAddAction이 아니면 발동 안하도록 (랜덤 체크는 0~99)
		}
		//==

		for( int i = 0; i < mArrPjt.Length; i++ ) {
			float x = Utility.GetRandom(0.0f, 0.3f, 10.0f);
			float y = Utility.GetRandom(0.0f, 0.5f, 10.0f);

			mArrPjtInfo[i].addedPosition = new Vector3( x, y, 0.0f );

			if( !SetAddAction ) {
				mArrPjt[i].Fire( m_owner, BattleOption.eToExecuteType.Unit, mAniEvt, mArrPjtInfo[i], targetCollider.Owner, TableId );
			}
			else {
				mArrPjtInfo[i].projectile = mArrPjt2[i];
				mArrPjt2[i].Fire( m_owner, BattleOption.eToExecuteType.Unit, mAniEvt, mArrPjtInfo[i], targetCollider.Owner, TableId );
			}
		}

		if( m_data != null && !SkipBOExecuteOnStart ) {
			ExecuteStartSkillBO();
		}

		mOwnerPlayer.OnStartSkill( this );
		StartCoolTime();
	}
}
