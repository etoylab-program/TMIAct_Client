
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;


public class DroneUnit : Unit {
	public enum eDroneType {
		None = 0,
		ByCharacter,
		BySupporter,
	}


	public class sDroneData {
		public DroneUnit Drone = null;
		public int Index = -1;
		public bool IsInit = false;
		public Coroutine CrAppear = null;
	}


	public eDroneType	DroneType	{ get; private set; } = eDroneType.None;
	public Unit			Owner		{ get; private set; } = null;
	public Player		OwnerPlayer { get; private set; } = null;
	public float		Tick		{ get; private set; } = 0.0f;
	public GameObject	LaserProp	{ get; private set; } = null;

	[SerializeField] private string _DronePrefabPath = "Effect/Supporter/prf_fx_supporter_skill_59_dron_appear.prefab";

	private MeshRenderer		mMesh			= null;
	private SkinnedMeshRenderer	mSkinnedMesh	= null;
	private int					mIndex			= -1;
	private float				mLifeTime		= 0.0f;
	private ParticleSystem		mEffAppear		= null;
	private TweenPosition		mTweenPos		= null;
	private WorldPVP			mWorldPVP		= null;


	public override void Init( int tableId, eCharacterType type, string faceAniControllerPath ) {
		base.Init( tableId, type, faceAniControllerPath );

		m_curHp = m_maxHp = 1.0f;

		Transform[] objs = gameObject.GetComponentsInChildren<Transform>();
		for ( int i = 0; i < objs.Length; i++ ) {
			if ( objs[i].name == "Prop" ) {
				LaserProp = objs[i].gameObject;
				break;
			}
		}

		mMesh = gameObject.GetComponentInChildren<MeshRenderer>();
		if ( mMesh == null ) {
			mSkinnedMesh = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
		}

		mTweenPos = gameObject.GetComponentInChildren<TweenPosition>();
		if ( mTweenPos ) {
			mTweenPos.enabled = false;
		}

		mEffAppear = ResourceMgr.Instance.CreateFromAssetBundle<ParticleSystem>( "effect", _DronePrefabPath );
		mEffAppear.gameObject.SetActive( false );

		SoundManager.Instance.AddAudioClip( "DroneAppear", 77, 1.0f );
		SoundManager.Instance.AddAudioClip( "DroneAttack", 78, 0.5f );
		SoundManager.Instance.AddAudioClip( "DroneHomingAttack", 79, 1.0f );

		ForceSetSuperArmor( eSuperArmor.Invincible );
		m_charType = eCharacterType.Summons;

		mWorldPVP = World.Instance as WorldPVP;
	}

	public override void SetData( int tableId ) {
		if ( !withoutAniEvent ) {
			m_folder = "Weapon";
		}
	}

	public override void Activate() {
		base.Activate();
		World.Instance.AddPlayerSummons( this );
	}

	public override void Deactivate() {
		StopBT();
		base.Deactivate();

		World.Instance.RemovePlayerSummons( this );
	}

	public override UnitCollider GetMainTargetCollider( bool onlyEnemy, float checkDist = 0.0f, bool skipHasShieldTarget = false, bool onlyAir = false ) {
		ActionBase action = m_actionSystem.currentAction;
		if ( action ) {
			checkDist = Mathf.Max( 20.0f, action.GetAtkRange() );
		}

		UnitCollider nearestHitCollider = null;

		if ( World.Instance.StageType != eSTAGETYPE.STAGE_PVP && m_mainTarget ) {
			if ( m_mainTarget.curHp <= 0.0f || !m_mainTarget.IsActivate() ||
			   ( checkDist > 0.0f && Vector3.Distance( transform.position, m_mainTarget.transform.position ) > checkDist ) ) {
				m_mainTarget = null;
			}
		}

		if ( m_mainTarget ) {
			nearestHitCollider = m_mainTarget.GetNearestColliderFromPos( transform.position );
		}
		else {
			Unit target = World.Instance.EnemyMgr.GetNearestTarget( Owner, onlyEnemy, skipHasShieldTarget, onlyAir, checkDist );
			if ( target ) {
				nearestHitCollider = target.GetNearestColliderFromPos( transform.position );
			}
			else {
				target = World.Instance.EnemyMgr.GetNearestTarget( Owner, false, skipHasShieldTarget, onlyAir, checkDist );
			}

			if ( target && !target.alwaysKinematic ) {
				Vector3 v1 = transform.position;

				UnitCollider unitCollider = target.GetNearestColliderFromPos( v1 );
				Vector3 v2 = unitCollider ? unitCollider.GetCenterPos() : target.GetCenterPos();

				int checkLayer = ( 1 << (int)eLayer.Enemy ) | ( 1 << (int)eLayer.EnemyGate ) | ( 1 << (int)eLayer.EnvObject ) |
								 ( 1 << (int)eLayer.Wall ) | ( 1 << (int)eLayer.Wall_Inside );

				if ( Physics.Raycast( v1, ( v2 - v1 ).normalized, out RaycastHit hitInfo, checkDist, checkLayer ) ) {
					if ( hitInfo.collider.gameObject.layer == (int)eLayer.Wall || hitInfo.collider.gameObject.layer == (int)eLayer.Wall_Inside ) {
						target = null;
						nearestHitCollider = null;
					}
				}
			}

			if ( target ) {
				float dist = Utility.GetDistanceWithoutY( transform.position, target.transform.position );
				ActionDroneGunFire actionAtk = m_actionSystem.GetAction<ActionDroneGunFire>( eActionCommand.Attack01 );
				if ( actionAtk && dist >= actionAtk.GetAtkRange() ) {
					target = null;
				}
			}

			SetMainTarget( target );
		}

		return nearestHitCollider;
	}

	public override bool OnHit( Unit attacker, BattleOption.eToExecuteType toExecuteType, AniEvent.sEvent attackerAniEvt, ObscuredFloat damage,
								ref bool isCritical, ref eHitState hitState, Projectile projectile, bool isUltimateSkill, bool skipMaxDamageRecord ) {
		return false;
	}

	public override void OnSuccessAttack( AniEvent.sEvent atkEvt, bool notAniEventAtk, int actionTableId, Projectile projectile, bool isCritical ) {
		OwnerPlayer.OnSuccessAttack( atkEvt, notAniEventAtk, actionTableId, projectile, isCritical );
	}

	public override void OnOnlyDamageAttack( AniEvent.sEvent atkEvt, bool notAniEventAtk, int actionTableId, Projectile projectile, bool isCritical ) {
		OwnerPlayer.OnOnlyDamageAttack( atkEvt, notAniEventAtk, actionTableId, projectile, isCritical );
	}

	public override bool OnAttack( AniEvent.sEvent evt, float atkRatio, bool notAniEventAtk ) {
		if ( !notAniEventAtk ) {
			m_onAttackAniEvent = evt;

			curSuccessAttack = false;
			curHitState = eHitState.Fail;
		}

		bool isCritical = false;
		float finalAttackPower = GetAttackPower( evt, ref isCritical ) * atkRatio;
		if ( !isCritical ) {
			isCritical = decideCritical;
		}

		m_atkDir = evt.atkDirection;
		if ( m_actionSystem && m_actionSystem.IsCurrentUSkillAction() ) {
			m_atkDir = eAttackDirection.Skip;
		}

		AttackEvent atkEvent = new AttackEvent();
		atkEvent.Set( eEventType.EVENT_BATTLE_ON_MELEE_ATTACK, Owner, BattleOption.eToExecuteType.Unit, evt, finalAttackPower, m_atkDir, isCritical, 0,
					  EffectManager.eType.None, null, 0.0f, notAniEventAtk, droneUnit: this );

		EventMgr.Instance.SendEvent( atkEvent );
		return true;
	}

	public void SetDroneUnit( eDroneType type, Unit owner, int index, float lifeTime, float tick ) {
		DroneType = type;
		Owner = owner;
		mIndex = index;
		mLifeTime = lifeTime;
		Tick = tick;

		m_originalSpeed = owner.speed;
		m_curSpeed = m_originalSpeed;

		OwnerPlayer = Owner as Player;
	}

	public void SetDroneAttackPower( float attackPower ) {
		m_attackPower = attackPower + ( attackPower * Owner.IncreaseSummonsAttackPowerRate );
	}

	public void SetDroneAttackPowerBySkill( float attackPower ) {
		SetDroneAttackPower( attackPower );

		attackPower = m_attackPower;
		m_attackPower = attackPower + ( attackPower * Owner.IncreaseSkillAtkValue ) + ( attackPower * GameInfo.Instance.BattleConfig.CharSkillDmgRatio );
	}

	public void SetTweenToPos( Vector3 toPos ) {
		if ( mTweenPos == null ) {
			return;
		}

		mTweenPos.to = toPos;
		mTweenPos.enabled = true;
	}

	public void UpdateMovement( Unit target ) {
		if ( Director.IsPlaying ) {
			return;
		}

		Vector3 v = Owner.posOnGround;
		v.y += Owner.MainCollider.height * 1.2f;
		Vector3 refPos = Vector3.zero;

		if ( mIndex == -1 ) {
			refPos = v + ( Owner.transform.forward * 0.5f ) + ( Owner.transform.right * 1.0f );
		}
		else if ( mIndex == 0 ) {
			refPos = v + ( Owner.transform.forward * 0.5f ) + ( Owner.transform.right * 0.5f );
		}
		else if ( mIndex == 1 ) {
			refPos = v + ( Owner.transform.forward * 0.5f ) - ( Owner.transform.right * 0.5f );
		}
		else if ( mIndex == 2 ) {
			refPos = v - ( Owner.transform.forward * 0.5f ) + ( Owner.transform.right * 0.5f );
		}
		else if ( mIndex == 3 ) {
			refPos = v - ( Owner.transform.forward * 0.5f ) - ( Owner.transform.right * 0.5f );
		}

		Vector3 dir = ( refPos - transform.position ).normalized;

		float dist = Vector3.Distance( transform.position, refPos );
		if ( dist > 1.0f ) {
			cmptMovement.UpdatePosition( dir, speed, false );
		}

		if ( target == null ) {
			cmptRotate.UpdateRotation( Owner.transform.forward, false, 0.5f );
		}
		else {
			if ( target.gameObject.layer == (int)eLayer.EnvObject ) {
				dir = ( target.GetCenterPos() - transform.position ).normalized;
			}
			else {
				UnitCollider targetCollider = target.GetNearestColliderFromPos( transform.position );
				if ( targetCollider ) {
					dir = ( targetCollider.GetCenterPos() - transform.position ).normalized;
				}
				else {
					dir = ( target.GetCenterPos() - transform.position ).normalized;
				}
			}

			cmptRotate.UpdateRotation( dir, false );
		}
	}

	public IEnumerator AppearForSupporter() {
		Vector3 startPos = Vector3.zero;
		if ( mIndex == 0 ) {
			startPos = Owner.GetHeadPos( 1.2f ) + Owner.transform.forward + Owner.transform.right;
		}
		else if ( mIndex == 1 ) {
			startPos = Owner.GetHeadPos( 1.2f ) + Owner.transform.forward - Owner.transform.right;
		}
		else if ( mIndex == 2 ) {
			startPos = Owner.GetHeadPos( 1.2f ) - Owner.transform.forward + Owner.transform.right;
		}
		else if ( mIndex == 3 ) {
			startPos = Owner.GetHeadPos( 1.2f ) - Owner.transform.forward - Owner.transform.right;
		}

		mEffAppear.gameObject.SetActive( true );
		mEffAppear.transform.position = startPos;
		mEffAppear.Stop();
		mEffAppear.Play();

		SoundManager.Instance.PlaySnd( SoundManager.eSoundType.FX, "DroneAppear" );

		yield return new WaitForSeconds( mEffAppear.main.duration );

		SetInitialPosition( startPos, Owner.transform.rotation );
		SetActiveMesh( true );
		base.Activate();

		StartBT();
		CommandAction( eActionCommand.DroneFollowOwner, null );

		if ( mLifeTime > 0.0f ) {
			StopCoroutine( "UpdateLife" );
			StartCoroutine( "UpdateLife" );
		}
	}

	public IEnumerator AppearForCharacter( float heightRatio ) {
		Vector3 startPos = Vector3.zero;
		if ( mIndex == 0 ) {
			startPos = Owner.GetHeadPos( heightRatio ) + Owner.transform.forward + Owner.transform.right;
		}
		else if ( mIndex == 1 ) {
			startPos = Owner.GetHeadPos( heightRatio ) + Owner.transform.forward - Owner.transform.right;
		}
		else if ( mIndex == 2 ) {
			startPos = Owner.GetHeadPos( heightRatio ) - Owner.transform.forward + Owner.transform.right;
		}
		else if ( mIndex == 3 ) {
			startPos = Owner.GetHeadPos( heightRatio ) - Owner.transform.forward - Owner.transform.right;
		}

		mEffAppear.gameObject.SetActive( true );
		mEffAppear.transform.position = startPos;
		mEffAppear.Stop();
		mEffAppear.Play();

		SoundManager.Instance.PlaySnd( SoundManager.eSoundType.FX, "DroneAppear" );

		yield return new WaitForSeconds( mEffAppear.main.duration );

		SetInitialPosition( startPos, Owner.transform.rotation );
		SetActiveMesh( true );
		base.Activate();

		StartBT();
		CommandAction( eActionCommand.DroneFollowOwner, null );

		if ( mLifeTime > 0.0f ) {
			StopCoroutine( "UpdateLife" );
			StartCoroutine( "UpdateLife" );
		}
	}

	public bool IsActivateMesh() {
		if ( mMesh ) {
			return mMesh.gameObject.activeSelf;
		}
		else if ( mSkinnedMesh ) {
			return mSkinnedMesh.gameObject.activeSelf;
		}

		return false;
	}

	private IEnumerator UpdateLife() {
		float checkTime = 0.0f;
		while ( checkTime < mLifeTime && !World.Instance.IsEndGame && !World.Instance.ProcessingEnd && ( mWorldPVP == null || ( mWorldPVP && mWorldPVP.IsBattleStart ))) {
			if ( !Director.IsPlaying ) {
				checkTime += Time.deltaTime;
			}

			yield return null;
		}

		SoundManager.Instance.PlaySnd( SoundManager.eSoundType.FX, "DroneAppear" );

		StopBT();
		SetActiveMesh( false );
		m_actionSystem.CancelCurrentAction();

		if ( aniEvent != null ) {
			aniEvent.Clear( true );
		}

		mEffAppear.gameObject.SetActive( true );
		mEffAppear.transform.position = transform.position;
		mEffAppear.Stop();
		mEffAppear.Play();

		yield return new WaitForSeconds( mEffAppear.main.duration );

		mEffAppear.gameObject.SetActive( false );

		SetActiveMesh( true );
		Deactivate();
	}

	private void SetActiveMesh( bool active ) {
		if ( mMesh ) {
			mMesh.gameObject.SetActive( active );
		}
		else if ( mSkinnedMesh ) {
			mSkinnedMesh.gameObject.SetActive( active );
		}
	}
}
