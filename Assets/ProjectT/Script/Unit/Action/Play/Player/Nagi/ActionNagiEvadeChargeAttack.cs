
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionNagiEvadeChargeAttack : ActionSelectSkillBase {
	private Unit			mTarget				= null;
	private eAnimation		mCurAni				= eAnimation.EvadeCharge;
	private List<Vector3>	mClonePositionList	= new List<Vector3>();


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );
		actionCommand = eActionCommand.HoldingDefBtnAttack;

		extraCondition = new eActionCondition[4];
		extraCondition[0] = eActionCondition.Grounded;
		extraCondition[1] = eActionCondition.NoUsingSkill;
		extraCondition[2] = eActionCondition.NoUsingQTE;
		extraCondition[3] = eActionCondition.NoUsingUSkill;

		cancelActionCommand = new eActionCommand[1];
		cancelActionCommand[0] = eActionCommand.TimingHoldAttack;

		extraCancelCondition = new eActionCondition[3];
		extraCancelCondition[0] = eActionCondition.UseSkill;
		extraCancelCondition[1] = eActionCondition.UseQTE;
		extraCancelCondition[2] = eActionCondition.UseUSkill;

		superArmor = Unit.eSuperArmor.Lv1;

		if( mValue1 > 2.0f ) {
			mCurAni = eAnimation.EvadeCharge_1;

			mClonePositionList.Capacity = 4;
			mClonePositionList.Add( new Vector3( -2.4f, 1.2f, 15.0f ) );
			mClonePositionList.Add( new Vector3( -1.2f, 0.6f, 15.0f ) );
			mClonePositionList.Add( new Vector3( 1.2f, 0.6f, -15.0f ) );
			mClonePositionList.Add( new Vector3( 2.4f, 1.2f, -15.0f ) );
		}
		else {
			mClonePositionList.Capacity = 2;
			mClonePositionList.Add( new Vector3( -1.2f, 0.6f, 15.0f ) );
			mClonePositionList.Add( new Vector3( 1.2f, 0.6f, -15.0f ) );
		}
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		ShowSkillNames( m_data );

		if( FSaveData.Instance.AutoTargetingSkill ) {
			mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
			if( mTarget ) {
				m_owner.LookAtTarget( mTarget.transform.position );
			}
		}
		else {
			m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
		}

		m_aniLength = m_owner.PlayAniImmediate( mCurAni );
		m_aniCutFrameLength = m_owner.aniEvent.GetCurCutFrameLength();

		int cloneIndexOffset = 2;
		for( int i = 0; i < mValue1; i++ ) {
			m_owner.ShowClone( i + cloneIndexOffset, 
							   m_owner.transform.position + ( m_owner.transform.right * mClonePositionList[i].x ) + ( m_owner.transform.forward * mClonePositionList[i].y ), 
							   Quaternion.Euler( m_owner.transform.localEulerAngles + new Vector3( 0.0f, mClonePositionList[i].z, 0.0f ) ) );

			Unit clone = m_owner.GetClone( i + cloneIndexOffset );
			clone.SetAttackPower( m_owner.attackPower * 0.3f );
			clone.PlayAniImmediate( mCurAni );
		}
	}

	public override IEnumerator UpdateAction() {
		while( m_endUpdate == false ) {
			m_checkTime += m_owner.fixedDeltaTime;
			if( m_checkTime >= m_aniCutFrameLength ) {
				m_endUpdate = true;
			}
			/*
			else if( mTarget && mTarget.curHp <= 0.0f ) {
				if( FSaveData.Instance.AutoTargetingSkill ) {
					mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
					if( mTarget ) {
						m_owner.LookAtTarget( mTarget.transform.position );
					}
				}
				else {
					m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
				}
			}
			*/

			int cloneIndexOffset = 2;
			for( int i = 0; i < mValue1; i++ ) {
				Unit clone = m_owner.GetClone( i + cloneIndexOffset );
				clone.SetInitialPosition( clone.transform.position, 
										  Quaternion.Euler( m_owner.transform.localEulerAngles + new Vector3( 0.0f, mClonePositionList[i].z, 0.0f ) ) );
			}
			
			yield return mWaitForFixedUpdate;
		}
	}

	public override void OnEnd() {
		base.OnEnd();

		int cloneIndexOffset = 2;
		for( int i = 0; i < mValue1; i++ ) {
			m_owner.HideClone( i + cloneIndexOffset );
		}
	}

	public override void OnCancel() {
		base.OnCancel();

		int cloneIndexOffset = 2;
		for( int i = 0; i < mValue1; i++ ) {
			m_owner.HideClone( i + cloneIndexOffset );
		}
	}

	public override float GetAtkRange() {
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( mCurAni );
		if( evt == null ) {
			Debug.LogError( mCurAni.ToString() + "공격 이벤트가 없네??" );
			return 0.0f;
		}
		else if( evt.visionRange <= 0.0f ) {
			Debug.LogError( mCurAni.ToString() + "Vistion Range가 0이네??" );
		}

		return evt.visionRange;
	}
}
