
using Platforms;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionAinaExtreamEvade : ActionExtreamEvade {
	private DroneUnit.sDroneData mDrone = null;
	private ParticleSystem mEffHealFloor = null;
	private BoxCollider mBoxHealFloor = null;


	public override void Init( int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam ) {
		base.Init( tableId, listAddCharSkillParam );

		string droneName = "Aina_Drone_1";
		if ( mValue2 > 1.0f ) {
			droneName = "Aina_Drone_2";
		}

		DroneUnit droneUnit = ResourceMgr.Instance.CreateFromAssetBundle<DroneUnit>( "unit", $"Unit/Weapon/{droneName}/{droneName}.prefab" );
		if ( droneUnit != null ) {
			mDrone = new DroneUnit.sDroneData();
			mDrone.Drone = droneUnit;
			mDrone.IsInit = true;
			mDrone.Index = 0;
			mDrone.CrAppear = null;

			mDrone.Drone.Init( 0, eCharacterType.Other, "" );
			mDrone.Drone.AddAIController( "Aina_Drone" );
			mDrone.Drone.SetDroneUnit( DroneUnit.eDroneType.ByCharacter, m_owner, mDrone.Index, mValue1, 0.0f );
			mDrone.Drone.Deactivate();
		}

		if ( mValue2 > 0.0f ) {
			mEffHealFloor = GameSupport.CreateParticle( "Effect/Character/prf_fx_aina_evade_skill_1.prefab", null );
			mBoxHealFloor = mEffHealFloor.GetComponent<BoxCollider>();
		}
	}

	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );
		StartEvadingBuff();
	}

	private void StartEvadingBuff() {
		mOwnerPlayer.ExtreamEvading = true;

		StopCoroutine( "EndEvading" );
		StartCoroutine( "EndEvading", mValue1 );
	}

	public override void OnEnd() {
		base.OnEnd();
		IsSkillEnd = false;
	}

	private IEnumerator EndEvading( float duration ) {
		if ( mDrone != null ) {
			Utility.StopCoroutine( this, ref mDrone.CrAppear );

			mDrone.Drone.SetDroneAttackPowerBySkill( m_owner.attackPower + ( m_owner.attackPower * mValue3 ) );
			mDrone.CrAppear = StartCoroutine( mDrone.Drone.AppearForCharacter( 1.0f ) );
		}

		StartCoroutine( HealingField() );

		yield return StartCoroutine( ContinueDash() );

		yield return new WaitForSeconds( duration );

		mOwnerPlayer.ExtreamEvading = false;
		IsSkillEnd = true;
	}

	private IEnumerator HealingField() {
		if ( mEffHealFloor == null ) {
			yield break;
		}

		float totalDurationSec = 4.0f;
		float tickIntervalSec = 0.7f;
		float tickCheckTime = 0.0f;
		float healPercentage = 0.04f;

		mEffHealFloor.transform.position = m_owner.posOnGround + ( m_owner.transform.forward * 1.25f );
		mEffHealFloor.gameObject.SetActive( true );
		EffectManager.Instance.RegisterStopEffByDuration( mEffHealFloor, null, totalDurationSec );

		WorldPVP worldPVP = World.Instance as WorldPVP;

		m_checkTime = 0.0f;

		while ( true ) {
			m_checkTime += m_owner.fixedDeltaTime;
			if ( m_checkTime >= totalDurationSec || ( worldPVP && ( !worldPVP.IsBattleStart || m_owner.curHp <= 0.0f ) ) ) {
				EffectManager.Instance.StopEffImmediate( mEffHealFloor, null );

				yield break;
			}

			tickCheckTime += m_owner.fixedDeltaTime;

			if ( tickCheckTime >= tickIntervalSec ) {
				if ( World.Instance.ListPlayer.Count > 0 ) {
					for ( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
						if ( World.Instance.ListPlayer[i].curHp <= 0.0f || !World.Instance.ListPlayer[i].IsActivate() ) {
							continue;
						}

						if ( mBoxHealFloor.bounds.Contains( World.Instance.ListPlayer[i].transform.position ) ) {
							World.Instance.ListPlayer[i].AddHpPercentage( BattleOption.eToExecuteType.Unit, healPercentage, false );
						}
					}
				}
				else {
					if ( m_owner.curHp > 0.0f && mBoxHealFloor.bounds.Contains( m_owner.transform.position ) ) {
						m_owner.AddHpPercentage( BattleOption.eToExecuteType.Unit, healPercentage, false );
					}
				}

				tickCheckTime = 0.0f;
			}

			yield return mWaitForFixedUpdate;
		}
	}
}
