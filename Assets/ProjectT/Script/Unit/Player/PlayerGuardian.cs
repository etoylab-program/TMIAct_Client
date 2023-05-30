using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGuardian : Unit {
	public enum eArrowType {
		UP = 0,
		BACK,
		UP_FORWARD,
		UP_BACK,
	}

	[Header( "[Guardian Property]" )]
	[SerializeField] private float _Alpha = 0.3f;

	public Player	OwnerPlayer		{ get; private set; } = null;
	public bool		IsCommandAction { get; private set; } = false;
	public float	MinDistance		{ get; private set; } = 1.5f;

	private AttachObject	mAttachObject			= null;
	private Unit			mActionTarget			= null;
	private bool			mStopBT					= false;
	private BoxCollider		mBoxCol					= null;
	private WorldPVP		mWorldPVP				= null;
	private ParticleSystem	mTeleportStartEffect	= null;
	private ParticleSystem	mTeleportEndEffect		= null;
	private Coroutine		mTeleportStartCoroutine = null;
	private Coroutine		mTeleportEndCoroutine	= null;


	public override void Init( int tableId, eCharacterType type, string faceAniControllerPath ) {
		base.Init( tableId, type, faceAniControllerPath );
		mWorldPVP = World.Instance as WorldPVP;

		m_originalMass = m_rigidBody.mass;
		m_massOnFloating = m_rigidBody.mass * 1000.0f;

		ShowMaxEffect( false );

		mBoxCol = aniEvent.gameObject.AddComponent<BoxCollider>();
		mBoxCol.center = new Vector3( 0.0f, 1.0f, 0.0f );
		mBoxCol.size = new Vector3( 0.5f, 2.0f, 1.0f );
		mBoxCol.isTrigger = true;

		m_aniEvent.SetShaderFloat( "_Cutoff", 0.0f );

		for ( int i = 0; i < m_aniEvent.ListMtrl.Count; i++ ) {
			m_aniEvent.ListMtrl[i].renderQueue += 2;
		}

		mTeleportStartEffect = GameSupport.CreateParticle( "Effect/Character/prf_fx_Shisui_Guardian_Teleport_Start.prefab", transform.parent );
		mTeleportEndEffect = GameSupport.CreateParticle( "Effect/Character/prf_fx_Shisui_Guardian_Teleport_End.prefab", transform.parent );
	}

	public override bool ShowMesh( bool show, bool isLock = false ) {
		bool r =  base.ShowMesh( show, isLock );

		if ( OwnerPlayer ) {
			WeaponData data = OwnerPlayer.GetCurrentWeaponDataOrNull();
			if ( data != null ) {
				bool isMaxLvWeapon = GameSupport.IsMaxLevelWeapon( data );
				ShowMaxEffect( isMaxLvWeapon );
			}
		}

		return r;
	}

	public void ShowMaxEffect( bool show ) {
		if ( mAttachObject == null ) {
			mAttachObject = GetComponent<AttachObject>();
		} 
	
		mAttachObject.ActiveEffect( show );
	}

	protected override void FixedUpdate() {
		base.FixedUpdate();

		if ( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
			return;
		}

		if ( ( mWorldPVP && !mWorldPVP.IsBattleStart ) || World.Instance.IsEndGame ) {
			return;
		}

		if ( ( OwnerPlayer && !OwnerPlayer.IsMissionStart ) || ( OwnerPlayer && OwnerPlayer.Input.isPause ) || ( OwnerPlayer && OwnerPlayer.curHp <= 0.0f ) || Director.IsPlaying || World.Instance.IsPause ) {
			if ( mStopBT == false ) {
				mStopBT = true;
				StopBT();
			}
			return;
		}

		if ( mStopBT ) {
			mStopBT = false;
			StartBT();
		}
	}

	protected override void LateUpdate() {
		base.LateUpdate();

		if ( AppMgr.Instance.SceneType == AppMgr.eSceneType.Title || AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby ) {
			return;
		}

		if ( OwnerPlayer == null ) {
			return;
		}

		if ( Director.IsPlaying || World.Instance.IsEndGame ) {
			aniEvent.SetShaderAlpha( "_Color", 1.0f );
			return;
		}

		Vector3 dir = OwnerPlayer.transform.position - World.Instance.InGameCamera.transform.position;
		
		RaycastHit hit;
		if ( Physics.Raycast( World.Instance.InGameCamera.transform.position, dir,  out hit, 10.0f, 
							  ( 1 << (int)eLayer.Player ) | ( 1 << (int)eLayer.PlayerClone ) ) ) {
			if ( aniEvent.gameObject == hit.collider.gameObject ) {
				aniEvent.SetShaderAlpha( "_Color", _Alpha );
			}
			else if ( OwnerPlayer.gameObject == hit.collider.gameObject ) {
				aniEvent.SetShaderAlpha( "_Color", 1.0f );
			}
		}
	}

	public override void ExecuteBattleOption( BattleOption.eBOTimingType timingType, int actionTableId, Projectile projectile, bool skipWeaponBO = false ) {
		base.ExecuteBattleOption( timingType, actionTableId, projectile, skipWeaponBO );

		if ( actionTableId > 0 ) {
			ActionGuardianBase actionGuardianBase = m_actionSystem.GetActionOrNullByTableId<ActionGuardianBase>( actionTableId );
			if ( actionGuardianBase != null ) {
				actionGuardianBase.ExecuteBattleOption( timingType, actionTableId, projectile );
			}
		}

		if ( OwnerPlayer != null ) {
			OwnerPlayer.ExecuteBattleOption( timingType, actionTableId, projectile, skipWeaponBO );
		}
	}

	public override void SetData( int tableId ) {
		if ( !withoutAniEvent ) {
			m_folder = "Weapon";
		}

		m_curHp = 1.0f;
	}

	public override UnitCollider GetMainTargetCollider( bool onlyEnemy, float checkDist = 0.0f, bool skipHasShieldTarget = false, bool onlyAir = false ) {
		UnitCollider nearestHitCollider = null;

		if ( World.Instance.StageType != eSTAGETYPE.STAGE_PVP && m_mainTarget ) {
			if ( FSaveData.Instance.AutoTargeting || ( mInput && mInput.GetDirection() != Vector3.zero ) || m_mainTarget.curHp <= 0.0f || !m_mainTarget.IsActivate() ||
			   ( checkDist > 0.0f && Vector3.Distance( transform.position, m_mainTarget.transform.position ) > checkDist ) ) {
				m_mainTarget = null;
			}
		}

		if ( m_mainTarget ) {
			nearestHitCollider = m_mainTarget.GetNearestColliderFromPos( transform.position );
		}
		else {
			Unit target = World.Instance.EnemyMgr.GetNearestTarget( this, onlyEnemy, skipHasShieldTarget, onlyAir );
			if ( target ) {
				SetMainTarget( target );
				nearestHitCollider = target.GetNearestColliderFromPos( transform.position );
			}
			else if ( ( AI && World.Instance.EnemyMgr.HasAliveMonster() ) || !onlyEnemy ) {
				target = World.Instance.EnemyMgr.GetNearestTarget( this, World.Instance.EnemyMgr.GetActiveEnvObjects() );
				if ( target ) {
					SetMainTarget( target );
					nearestHitCollider = target.GetNearestColliderFromPos( transform.position );
				}
			}
		}

		return nearestHitCollider;
	}

	public override void OnSuccessAttack( AniEvent.sEvent atkEvt, bool notAniEventAtk, int actionTableId, Projectile projectile, bool isCritical ) {
		if ( OwnerPlayer ) {
			OwnerPlayer.OnSuccessAttack( atkEvt, notAniEventAtk, actionTableId, projectile, isCritical );
		}
	}

	public override void OnOnlyDamageAttack( AniEvent.sEvent atkEvt, bool notAniEventAtk, int actionTableId, Projectile projectile, bool isCritical ) {
		if ( OwnerPlayer ) {
			OwnerPlayer.OnOnlyDamageAttack( atkEvt, notAniEventAtk, actionTableId, projectile, isCritical );
		}
	}

	public override void Retarget() {
		base.Retarget();

		if ( mWorldPVP ) {
			Player enemy = mWorldPVP.GetCurrentPlayerTeamCharOrNull();
			if ( enemy == OwnerPlayer ) {
				enemy = mWorldPVP.GetCurrentOpponentTeamCharOrNull();
			}

			if ( enemy ) {
				SetMainTarget( enemy.GetHighestAggroUnit( this ) );
			}
		}
	}

	public void PlayAction( eActionCommand actionCommand, Unit target ) {
		actionSystem.CancelCurrentAction();

		mActionTarget = target;
		IsCommandAction = true;

		CommandAction( actionCommand, null );
	}

	public Unit GetActionTarget() {
		if ( IsCommandAction == false ) {
			if ( mActionTarget == null || mActionTarget.curHp <= 0.0f ) {
				mActionTarget = World.Instance.EnemyMgr.GetNearestTarget( this, true );
			}
		}

		if ( mActionTarget == null || mActionTarget.curHp <= 0.0f ) {
			return null;
		}

		return mActionTarget;
	}

	public void SetOwnerPlayer( Player ownerPlayer ) {
		OwnerPlayer = ownerPlayer;

		if ( OwnerPlayer == null ) {
			return;
		}

		if ( OwnerPlayer.Guardian == null ) {
			OwnerPlayer.SetGuardian( this );
		}

		m_originalSpeed = OwnerPlayer.originalSpeed;
		m_curSpeed = m_originalSpeed;

		ActionSelectSkillBase actionSelectSkillBase;
		for ( int i = 0; i < actionSystem.ListAction.Count; i++ ) {
			actionSelectSkillBase = actionSystem.ListAction[i] as ActionSelectSkillBase;
			if ( actionSelectSkillBase == null ) {
				continue;
			}

			actionSelectSkillBase.SetOwnerPlayer( OwnerPlayer );
		}

		for ( int i = 0; i < OwnerPlayer.AfterActionTypeList.Count; i++ ) {
			ActionBase actionBase = gameObject.AddComponent( OwnerPlayer.AfterActionTypeList[i] ) as ActionBase;
			actionSystem.AddAction( actionBase, 0, null );
		}

		AddAIController( "Guardian" );
	}

	public void SetActionTarget( Unit target ) {
		mActionTarget = target;
	}

	public void SetGuardianAttackPower( float attackPower ) {
		m_attackPower = attackPower + ( attackPower * OwnerPlayer.IncreaseSummonsAttackPowerRate );
	}

	public void EndAction() {
		mActionTarget = null;
		IsCommandAction = false;
	}

	public IEnumerator TeleportGuardian( eArrowType arrowType, Vector2 ratio, bool isZReverse ) {
		if ( OwnerPlayer == null || OwnerPlayer.UseGuardian == false ) {
			yield break;
		}

		Utility.StopCoroutine( this, ref mTeleportStartCoroutine );
		mTeleportStartCoroutine = StartCoroutine( PlayTeleportStartParticle( GetHeadPos() ) );

		MainCollider.Enable( false );

		switch ( arrowType ) {
			case eArrowType.UP: {
				transform.position = OwnerPlayer.transform.position + Vector3.up * ratio.y;
			}
			break;

			case eArrowType.UP_FORWARD: {
				transform.position = OwnerPlayer.transform.position + Vector3.up * ratio.y + OwnerPlayer.transform.forward * ratio.x;
			}
			break;

			case eArrowType.BACK: {
				transform.position = OwnerPlayer.transform.position - OwnerPlayer.transform.forward * ratio.x;
			}
			break;

			case eArrowType.UP_BACK: {
				transform.position = OwnerPlayer.transform.position + Vector3.up * ratio.y - OwnerPlayer.transform.forward * ratio.x;
			}
			break;
		}

		Quaternion rotation = OwnerPlayer.transform.rotation;
		if ( isZReverse ) {
			rotation = Quaternion.LookRotation( OwnerPlayer.transform.forward * -1.0f );
		}

		transform.rotation = rotation;

		Utility.StopCoroutine( this, ref mTeleportEndCoroutine );
		mTeleportEndCoroutine = StartCoroutine( PlayTeleportEndParticle( GetHeadPos() ) );

		yield return mWaitForFixedUpdate;
	}

	private IEnumerator PlayTeleportStartParticle( Vector3 position ) {
		if ( mTeleportStartEffect.gameObject.activeSelf ) {
			mTeleportStartEffect.gameObject.SetActive( false );
		}

		mTeleportStartEffect.transform.position = position;
		mTeleportStartEffect.gameObject.SetActive( true );

		yield return new WaitForSeconds( mTeleportStartEffect.main.duration );

		mTeleportStartEffect.gameObject.SetActive( false );
		mTeleportStartCoroutine = null;
	}

	private IEnumerator PlayTeleportEndParticle( Vector3 position ) {
		if ( mTeleportEndEffect.gameObject.activeSelf ) {
			mTeleportEndEffect.gameObject.SetActive( false );
		}

		mTeleportEndEffect.transform.position = position;
		mTeleportEndEffect.gameObject.SetActive( true );

		yield return new WaitForSeconds( mTeleportEndEffect.main.duration );

		mTeleportEndEffect.gameObject.SetActive( false );
		mTeleportEndCoroutine = null;
	}
}
