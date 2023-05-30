
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionAinaAttackHoldAttack : ActionSelectSkillBase {
	private eAnimation				mCurAni	= eAnimation.AttackHold;
	private Unit					mTarget	= null;
	private DroneUnit.sDroneData	mDrone	= null;


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

		if ( mValue1 > 0.0f ) {
			mCurAni = eAnimation.AttackHold_1;
		}
		else if ( mValue2 > 0.0f ) {
			mCurAni = eAnimation.AttackHold_2;

			DroneUnit droneUnit = ResourceMgr.Instance.CreateFromAssetBundle<DroneUnit>( "unit", "Unit/Weapon/Aina_Drone_1/Aina_Drone_1.prefab" );
			if ( droneUnit != null ) {
				mDrone = new DroneUnit.sDroneData();
				mDrone.Drone = droneUnit;
				mDrone.IsInit = true;
				mDrone.Index = 1;
				mDrone.CrAppear = null;

				mDrone.Drone.Init( 0, eCharacterType.Other, "" );
				mDrone.Drone.AddAIController( "Aina_Drone" );
				mDrone.Drone.SetDroneUnit( DroneUnit.eDroneType.ByCharacter, m_owner, mDrone.Index, mValue2, 0.0f );
				mDrone.Drone.Deactivate();
			}
		}
		else if ( mValue3 > 0.0f ) {
			mCurAni = eAnimation.AttackHold_3;
		}
	}

	public override void OnStart( IActionBaseParam param ) {
		if ( mValue1 >= 1.0f ) {
			superArmor = Unit.eSuperArmor.Lv2;
		}
		else {
			superArmor = Unit.eSuperArmor.Lv1;
		}

		base.OnStart( param );
		ShowSkillNames( m_data );

		m_aniLength = m_owner.PlayAniImmediate( mCurAni );
		m_owner.afterImg.Play( m_owner.aniEvent.listSkinnedMesh.ToArray(), m_aniLength, 0.3f, m_aniLength, Color.white );

		if ( FSaveData.Instance.AutoTargetingSkill ) {
			mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
			if ( mTarget ) {
				m_owner.LookAtTarget( mTarget.transform.position );
			}
		}
		else {
			m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
		}

		if ( mDrone != null ) {
			Utility.StopCoroutine( this, ref mDrone.CrAppear );

			mDrone.Drone.SetDroneAttackPowerBySkill( m_owner.attackPower );
			mDrone.CrAppear = StartCoroutine( mDrone.Drone.AppearForCharacter( 1.0f ) );
		}
	}

	public override IEnumerator UpdateAction() {
		while ( m_endUpdate == false ) {
			m_checkTime += m_owner.fixedDeltaTime;
			if ( m_checkTime >= m_aniLength ) {
				m_endUpdate = true;
			}

			if ( FSaveData.Instance.AutoTargetingSkill ) {
				if ( mTarget && mTarget.curHp > 0.0f ) {
					m_owner.LookAtTarget( mTarget.transform.position );
				}
				else {
					mTarget = World.Instance.EnemyMgr.GetNearestTarget( m_owner, true );
				}
			}
			else {
				m_owner.cmptRotate.UpdateRotation( m_owner.Input.GetDirection(), true );
			}

			yield return mWaitForFixedUpdate;
		}
	}

	public override void OnCancel() {
		base.OnCancel();
		m_owner.StopStepForward();
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
