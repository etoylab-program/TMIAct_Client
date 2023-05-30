﻿using System.Collections;
using System.Collections.Generic;


public class ActionShisuiEvadeDoubleAttack : ActionSelectSkillBase {
	private eAnimation	mCurAni = eAnimation.EvadeDouble;
	private Unit		mTarget = null;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		actionCommand = eActionCommand.Teleport;

		extraCondition = new eActionCondition[4];
		extraCondition[0] = eActionCondition.Grounded;
		extraCondition[1] = eActionCondition.NoUsingSkill;
		extraCondition[2] = eActionCondition.NoUsingQTE;
		extraCondition[3] = eActionCondition.NoUsingUSkill;

		extraCancelCondition = new eActionCondition[3];
		extraCancelCondition[0] = eActionCondition.UseSkill;
		extraCancelCondition[1] = eActionCondition.UseQTE;
		extraCancelCondition[2] = eActionCondition.UseUSkill;

		superArmor = Unit.eSuperArmor.Lv1;

		if ( mValue2 > 0.0f ) {
			mCurAni = eAnimation.EvadeDouble_1;
		}
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		if ( FSaveData.Instance.AutoTargetingSkill ) {
			mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, GetAtkRange(), 180.0f );
			if ( mTarget ) {
				m_owner.LookAtTarget( mTarget.transform.position );
			}
			else {
				mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
				if ( mTarget ) {
					m_owner.LookAtTarget( mTarget.transform.position );
				}
			}
		}
		else {
			m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
		}

		mOwnerPlayer.Guardian.PlayAction( actionCommand, mTarget );

		m_aniLength = m_owner.PlayAniImmediate( mCurAni );
		m_aniCutFrameLength = m_owner.aniEvent.GetCurCutFrameLength();

		ShowSkillNames( m_data );
	}

	public override IEnumerator UpdateAction() {
		while ( m_endUpdate == false ) {
			m_checkTime += m_owner.fixedDeltaTime;
			if ( m_checkTime >= m_aniCutFrameLength ) {
				m_endUpdate = true;
			}
			else if ( mTarget && mTarget.curHp <= 0.0f ) {
				if ( FSaveData.Instance.AutoTargetingSkill ) {
					mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
					if ( mTarget ) {
						m_owner.LookAtTarget( mTarget.transform.position );
						mOwnerPlayer.Guardian.SetActionTarget( mTarget );
					}
				}
				else {
					m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
				}
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override float GetAtkRange() {
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( mCurAni );
		if ( evt == null ) {
			Debug.LogError( mCurAni + "공격 이벤트가 없네??" );
			return 0.0f;
		}
		else if ( evt.visionRange <= 0.0f ) {
			Debug.LogError( mCurAni + "Vistion Range가 0이네??" );
		}

		return evt.visionRange;
	}
}
