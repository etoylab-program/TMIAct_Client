
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionMurasakiChainRushAttack : ActionSelectSkillBase {

	private Unit				mTarget					= null;
	private float				mDistance				= 6.0f;
	private float				mLookAtTargetAngle		= 180.0f;
	private List<eAnimation>	mListRepeatAni			= new List<eAnimation>();
	private int					mCurRepeatAniIndex		= 0;
	private int					mExtraHitNum			= 0;
	private int					mCurHitCount			= 0;
	private float				mAccumAddActionValue1	= 0.0f;
	private bool				mbExecuteAddAction		= false;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		actionCommand = eActionCommand.RushAttack;
		IsRepeatSkill = true;

		conditionActionCommand = new eActionCommand[1];
		conditionActionCommand[0] = eActionCommand.Defence;

		extraCondition = new eActionCondition[1];
		extraCondition[0] = eActionCondition.Grounded;

		extraCancelCondition = new eActionCondition[3];
		extraCancelCondition[0] = eActionCondition.UseSkill;
		extraCancelCondition[1] = eActionCondition.UseQTE;
		extraCancelCondition[2] = eActionCondition.UseUSkill;

		superArmor = Unit.eSuperArmor.Lv2;

		mListRepeatAni.Add( eAnimation.RushAttackRepeat );
		mListRepeatAni.Add( eAnimation.RushAttackRepeat2 );
		mCurRepeatAniIndex = 0;

		mExtraHitNum += (int)mValue1;
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		ShowSkillNames( m_data );

		mCurHitCount = 0;
		mAccumAddActionValue1 = AddActionValue1;

		m_aniLength = m_owner.PlayAniImmediate( eAnimation.RushAttack );
		m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength( eAnimation.RushAttack );

		if ( FSaveData.Instance.AutoTargetingSkill ) {
			mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, mDistance, mLookAtTargetAngle );
			if ( mTarget ) {
				m_owner.LookAtTarget( mTarget.transform.position );
			}
		}

		mbExecuteAddAction = false;
	}

	public override IEnumerator UpdateAction() {
		while ( m_endUpdate == false ) {
			m_checkTime += m_owner.fixedDeltaTime;
			if ( m_checkTime >= m_aniLength ) {
				m_endUpdate = true;
			}

			yield return mWaitForFixedUpdate;
		}

		if ( FSaveData.Instance.AutoTargetingSkill ) {
			mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, mDistance, mLookAtTargetAngle );
			if ( mTarget ) {
				m_owner.LookAtTarget( mTarget.transform.position );
			}
		}

		if ( SetAddAction ) {
			m_owner.cmptBuffDebuff.RemoveDebuff( eEventType.EVENT_DEBUFF_DECREASE_SP );
		}

		m_aniLength = m_owner.PlayAniImmediate( eAnimation.RushAttackRepeatFinish );
		yield return new WaitForSeconds( m_aniLength );
	}

	public override void OnUpdating( IActionBaseParam param ) {
		if ( mExtraHitNum <= 0 || m_endUpdate ) {
			return;
		}

		if ( m_checkTime <= m_aniCutFrameLength ) {
			return;
		}

		/*
		if ( mCurHitCount == 0 && SetAddAction ) {
			mOwnerPlayer.ExecuteBattleOption( BattleOption.eBOTimingType.OnStartAddAction, 0, false );
		}
		*/

		eAnimation curAni = mListRepeatAni[mCurRepeatAniIndex];

		if ( SetAddAction && mbExecuteAddAction ) {
			mOwnerPlayer.ExecuteBattleOption( BattleOption.eBOTimingType.OnStartAddAction, 0, null );

			List<AniEvent.sEvent> atkEventList = m_owner.aniEvent.GetAllAttackEvent( curAni );
			for ( int i = 0; i < atkEventList.Count; i++ ) {
				atkEventList[i].atkRatio = atkEventList[i].originalAtkRatio + mAccumAddActionValue1;
			}

			mAccumAddActionValue1 += AddActionValue1;
		}

		m_aniLength = m_owner.PlayAniImmediate( curAni );
		m_aniCutFrameLength = m_owner.aniEvent.GetCutFrameLength( curAni );
		m_checkTime = 0.0f;

		++mCurRepeatAniIndex;
		if ( mCurRepeatAniIndex >= mListRepeatAni.Count ) {
			mCurRepeatAniIndex = 0;
		}

		if ( FSaveData.Instance.AutoTargetingSkill || m_owner.AI ) {
			mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, mDistance, mLookAtTargetAngle );
			if ( mTarget ) {
				m_owner.LookAtTarget( mTarget.transform.position );
			}
			else if ( m_owner.AI ) {
				m_endUpdate = true;
				return;
			}
		}
		else {
			m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
		}

		++mCurHitCount;
		if ( mCurHitCount >= mExtraHitNum ) {
			if ( SetAddAction ) {
				if ( m_owner.curSp <= 0.0f ) {
					m_endUpdate = true;
				}
				else {
					mbExecuteAddAction = true;
				}
			}
			else {
				m_endUpdate = true;
			}
		}
	}

	public override void OnCancel() {
		if ( SetAddAction ) {
			m_owner.cmptBuffDebuff.RemoveDebuff( eEventType.EVENT_DEBUFF_DECREASE_SP );
		}

		base.OnCancel();
	}

	public override float GetAtkRange() {
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( eAnimation.RushAttack );
		if ( evt == null ) {
			Debug.LogError( "RushAtack 공격 이벤트가 없네??" );
			return 0.0f;
		}
		else if ( evt.visionRange <= 0.0f ) {
			Debug.LogError( "RushAttack Vistion Range가 0이네??" );
		}

		return evt.visionRange;
	}
}
