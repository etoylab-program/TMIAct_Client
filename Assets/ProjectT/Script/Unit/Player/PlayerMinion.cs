
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMinion : Unit {
	public bool SummoningMinion	{ get; set; } = false;

	public GameClientTable.Monster.Param	Data	{ get; private set; } = null;
	public Unit								Owner	{ get; private set; } = null;

	private Renderer[]	mChangeMtrlMesh	= null;
	private bool		mbStopBT		= false;
	private BaseEvent	mBaseEvent		= new BaseEvent();


	public override void Init( int tableId, eCharacterType type, string faceAniControllerPath ) {
		Debug.Log( "Enemy Table Id (Player Minion) : " + tableId );
		base.Init( tableId, type, faceAniControllerPath );

		m_charType = eCharacterType.Summons;
		m_originalMass = m_rigidBody.mass;
		m_massOnFloating = m_rigidBody.mass * 100.0f;

		AggroValue = -1;
		SummoningMinion = false;

		if ( m_actionSystem.HasNoAction() == false ) {
			for ( int i = 0; i < m_actionSystem.ListAction.Count; i++ ) {
				m_actionSystem.ListAction[i].InitAfterOwnerInit();
			}

			AddAIController( Data.AI );
		}

		ChangeMainTexture();

		mBaseEvent.eventSubject = eEventSubject.World;
		mBaseEvent.sender = this;

		if ( World.Instance.StageType == eSTAGETYPE.STAGE_PVP ) {
			SetLockAxis( Unit.eAxisType.Z );
		}
	}

	public override void SetData( int tableId ) {
		Data = GameInfo.Instance.GetMonsterData( tableId );
		if ( Data == null ) {
			return;
		}

		tableName = FLocalizeString.Instance.GetText( Data.Name );

		m_tableId = tableId;
		m_monType = (eMonType)Data.MonType;
		m_grade = (eGrade)Data.Grade;
		m_groggy = false;

		SetStats();
		m_folder = Utility.GetFolderFromPath( Data.ModelPb );
	}

	public void SetAggroValue( int value ) {
		AggroValue = value;
	}

	public override void Activate() {
		base.Activate();
		TemporaryInvincible = true;

		mBaseEvent.eventType = eEventType.EVENT_SUMMON_PLAYER_MINION;
		EventMgr.Instance.SendEvent( mBaseEvent );
	}

	public override void Deactivate() {
		base.Deactivate();

		mBaseEvent.eventType = eEventType.EVENT_SUMMON_OFF_PLAYER_MINION;
		EventMgr.Instance.SendEvent( mBaseEvent );
	}

	public override UnitCollider GetMainTargetCollider( bool onlyEnemy, float checkDist = 0.0f, bool skipHasShieldTarget = false, bool onlyAir = false ) {
		UnitCollider nearestHitCollider = null;

		if ( World.Instance.StageType != eSTAGETYPE.STAGE_PVP && m_mainTarget ) {
			if ( FSaveData.Instance.AutoTargeting || ( mInput && mInput.GetDirection() != Vector3.zero ) || m_mainTarget.curHp <= 0.0f || !m_mainTarget.IsActivate() ||
			   ( checkDist > 0.0f && Vector3.Distance( transform.position, m_mainTarget.transform.position ) > checkDist ) ) {
				m_mainTarget = null;
			}
		}

		onlyEnemy = true;

		Unit target = World.Instance.EnemyMgr.GetNearestTarget( Owner, onlyEnemy, skipHasShieldTarget, onlyAir );
		if ( target ) {
			SetMainTarget( target );
			nearestHitCollider = target.GetNearestColliderFromPos( transform.position );
		}
		else {
			target = World.Instance.EnemyMgr.GetNearestTarget( Owner, false, skipHasShieldTarget, onlyAir );
			if ( target ) {
				SetMainTarget( target );
				nearestHitCollider = target.GetNearestColliderFromPos( transform.position );
			}
		}

		return nearestHitCollider;
	}

	public override UnitCollider GetRandomTargetCollider( bool onlyEnemy = false ) {
		return GetMainTargetCollider( onlyEnemy );
	}

	public override void ExecuteBattleOption( BattleOption.eBOTimingType timingType, int actionTableId, Projectile projectile, bool skipWeaponBO = false ) {
		if ( timingType == BattleOption.eBOTimingType.OnAttack ) {
			ActionEnemyBase action = null;

			if ( projectile && projectile.OwnerAction ) {
				action = projectile.OwnerAction as ActionEnemyBase;
			}
			else {
				action = m_actionSystem.GetCurrentAction<ActionEnemyBase>();
			}

			if ( action ) {
				action.ExecuteBattleOption( timingType, projectile );
			}
		}
	}

	public override void OnSuccessAttack( AniEvent.sEvent atkEvt, bool notAniEventAtk, int actionTableId, Projectile projectile, bool isCritical ) {
		base.OnSuccessAttack( atkEvt, notAniEventAtk, actionTableId, projectile, isCritical );
		Owner.OnSuccessAttack( atkEvt, notAniEventAtk, actionTableId, projectile, isCritical );
	}

	public override void OnOnlyDamageAttack( AniEvent.sEvent atkEvt, bool notAniEventAtk, int actionTableId, Projectile projectile, bool isCritical ) {
		base.OnOnlyDamageAttack( atkEvt, notAniEventAtk, actionTableId, projectile, isCritical );
		Owner.OnOnlyDamageAttack( atkEvt, notAniEventAtk, actionTableId, projectile, isCritical );
	}

	public void SetOwner( Unit owner ) {
		Owner = owner;
	}

	public void SetMinionAttackPower( int ownerLayer, float attackPower ) {
		if ( (eLayer)ownerLayer == eLayer.Player ) {
			Utility.SetLayer( gameObject, (int)eLayer.PlayerClone, true );
		}
		else {
			Utility.SetLayer( gameObject, (int)eLayer.EnemyClone, true );
		}

		m_attackPower = attackPower + ( attackPower * Owner.IncreaseSummonsAttackPowerRate );
	}

	protected override void FixedUpdate() {
		base.FixedUpdate();

		if ( !mbStopBT && Director.IsPlaying ) {
			mbStopBT = true;

			StopBT();
			ShowMesh( false );
		}
		else if ( mbStopBT && !Director.IsPlaying && !World.Instance.IsPause ) {
			mbStopBT = false;
			GetMainTargetCollider( true );

			ResetBT();
			ShowMesh( true );
		}
	}

	private void SetStats() {
		m_maxHp = Data.MaxHP;
		m_curHp = m_maxHp;

		m_maxShield = Mathf.Max( 0.0f, Data.Shield );
		m_curShield = m_maxShield;

		if ( m_curShield > 0.0f ) {
			mChangedSuperArmorId = SetSuperArmor( eSuperArmor.Lv2 );
		}
		else {
			mChangedSuperArmorId = SetSuperArmor( eSuperArmor.None );
		}

		m_originalSpeed = Data.MoveSpeed;
		m_curSpeed = m_originalSpeed;
		m_backwardSpeed = Data.BackwardSpeed;

		m_attackPower = Data.AttackPower;
		m_defenceRate = Data.DefenceRate;
		m_criticalRate = Data.CriticalRate;

		m_scale = Data.Scale;
		transform.localScale = new Vector3( m_scale, m_scale, m_scale );
	}

	private void ChangeMainTexture() {
		if ( string.IsNullOrEmpty( Data.Texture ) ) {
			return;
		}

		string[] fileName = Utility.Split( name, ' ' );
		fileName[0] = fileName[0].Replace( "(Clone)", "" );
		string path = string.Format( "{0}.png", Data.Texture );

		Texture2D tex = ResourceMgr.Instance.LoadFromAssetBundle( "model_monster", path ) as Texture2D;
		if ( tex == null ) {
			Debug.LogError( path + "에 텍스쳐를 읽어올 수 없습니다." );
			return;
		}
		else {
			if ( mChangeMtrlMesh != null && mChangeMtrlMesh.Length > 0 ) {
				for ( int i = 0; i < mChangeMtrlMesh.Length; i++ ) {
					mChangeMtrlMesh[i].material.mainTexture = tex;
				}
			}
			else {
				for ( int i = 0; i < m_aniEvent.ListMtrl.Count; i++ ) {
					m_aniEvent.ListMtrl[i].mainTexture = tex;
				}
			}
		}
	}
}
