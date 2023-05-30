
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionEmilySummonDrone : ActionSelectSkillBase {
	protected Unit			mTarget	= null;
	protected eAnimation	mCurAni	= eAnimation.EvadeDouble;


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
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		ShowSkillNames( m_data );

		mCurAni = eAnimation.EvadeDouble;
		if ( mValue2 >= 1.0f ) {
			List<Projectile> list = m_owner.aniEvent.GetAllProjectile( mCurAni );
			for ( int i = 0; i < list.Count; i++ ) {
				list[i].SetProjectileAtkAttr( Projectile.EProjectileAtkAttr.KNOCKBACK );
			}
		}
		else if ( mValue1 >= 1.0f ) {
			mCurAni = eAnimation.EvadeDouble_1;
		}

		if ( SetAddAction ) {
			List<Projectile> list = m_owner.aniEvent.GetAllProjectile( mCurAni );
			for ( int i = 0; i < list.Count; i++ ) {
				list[i].SetAddAtkRatio( AddActionValue1 );
			}
		}

		m_aniLength = m_owner.PlayAniImmediate( mCurAni );

		if ( FSaveData.Instance.AutoTargetingSkill ) {
			mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
			if ( mTarget ) {
				m_owner.LookAtTarget( mTarget.transform.position );
			}
		}
		else {
			m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
		}
	}

	public override IEnumerator UpdateAction() {
		while ( m_endUpdate == false ) {
			m_checkTime += m_owner.fixedDeltaTime;
			if ( m_checkTime >= m_aniLength ) {
				m_endUpdate = true;
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override float GetAtkRange() {
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( mCurAni );
		if ( evt == null ) {
			Debug.LogError( mCurAni.ToString() + "공격 이벤트가 없네??" );
			return 0.0f;
		}
		else if ( evt.visionRange <= 0.0f ) {
			Debug.LogError( mCurAni.ToString() + "Vistion Range가 0이네??" );
		}

		return evt.visionRange;
	}
}
