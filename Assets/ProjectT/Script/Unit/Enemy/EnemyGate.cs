
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;


public class EnemyGate : Enemy {
	[Header("[Gate Property]")]
	public AudioClip SndHit;

	public BattleSpawnPoint SpawnPoint { get; private set; }


	public override void Init( int tableId, eCharacterType type, string faceAniControllerPath ) {
		base.Init( tableId, type, faceAniControllerPath );

		m_cmptBuffDebuff.Unused = true;
		ForceSetSuperArmor( eSuperArmor.Lv2 );
	}

	public void SetSpawnPoint( BattleSpawnPoint spawnPoint ) {
		SpawnPoint = spawnPoint;
	}

	public override bool OnHit( Unit attacker, BattleOption.eToExecuteType toExecuteType, AniEvent.sEvent attackerAniEvt, ObscuredFloat damage, 
								ref bool isCritical, ref eHitState hitState, Projectile projectile, bool isUltimateSkill, bool skipMaxDamageRecord ) {
		bool breakShield = base.OnHit( attacker, toExecuteType, attackerAniEvt, damage, ref isCritical, ref hitState, 
									   projectile, isUltimateSkill, skipMaxDamageRecord );

		SoundManager.Instance.PlayFxSnd( SndHit, 0.8f );
		return breakShield;
	}
}
