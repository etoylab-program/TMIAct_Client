using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shisui : Player {
	[SerializeField] private float _MinDistance = 5.0f;
	
	private bool			mIsAuto						= false;
	private ParticleSystem	mLeftParticle				= null;
	private ParticleSystem	mRightParticle				= null;
	private WorldPVP		mWorldPVP					= null;


	public override void Init( int tableId, eCharacterType type, string faceAniControllerPath ) {
		base.Init( tableId, type, faceAniControllerPath );
		mWorldPVP = World.Instance as WorldPVP;

		mLeftParticle = GameSupport.CreateParticle( "Effect/Character/prf_fx_Shisui_Normal_Dust.prefab", costumeUnit.FindChildByName( "effect_socket_1", costumeUnit.transform ) );
		mRightParticle = GameSupport.CreateParticle( "Effect/Character/prf_fx_Shisui_Normal_Dust.prefab", costumeUnit.FindChildByName( "effect_socket_2", costumeUnit.transform ) );

		if ( mLeftParticle != null ) {
			mLeftParticle.transform.rotation = Quaternion.Euler( new Vector3( 90.0f, 0.0f, 0.0f ) );
		}

		if ( mRightParticle != null ) {
			mRightParticle.transform.rotation = Quaternion.Euler( new Vector3( 90.0f, 0.0f, 0.0f ) );
		}
	}

	protected override void FixedUpdate() {
		base.FixedUpdate();

		if ( ( mWorldPVP && !mWorldPVP.IsBattleStart ) || World.Instance.IsEndGame || m_curHp <= 0.0f ) {
			return;
		}

		if ( !IsMissionStart || Director.IsPlaying || ( Input && ( Input.isPause || Input.GetRawDirection() != Vector3.zero ) ) ) {
			return;
		}

		if ( mIsAuto ) {
			bool hasAliveMonster = World.Instance.EnemyMgr.HasAliveMonster();
			if ( IsAutoMove && ( m_mainTarget || hasAliveMonster ) ) {
				SetAutoMove( false );
			}
			else if ( !IsAutoMove && m_mainTarget == null && !hasAliveMonster ) {
				SetAutoMove( true );
			}

			if ( IsAutoMove == false && m_actionSystem.currentAction == null ) {
				UnitCollider unitCollider;
				if ( hasAliveMonster ) {
					if ( m_mainTarget is not Enemy ) {
						unitCollider = GetMainTargetCollider( hasAliveMonster, _MinDistance );
					}
					else {
						unitCollider = m_mainTarget.MainCollider;
					}
				}
				else {
					unitCollider = GetDirAngleToEnvObject();
				}

				if ( unitCollider != null ) {
					Vector3 dir = Utility.GetDirWithoutY( unitCollider.transform.position, transform.position );
					cmptRotate.UpdateRotation( dir, false );

					float dist = Utility.GetDistanceWithoutY( unitCollider.transform.position, transform.position );
					if ( _MinDistance <= dist ) {
						cmptMovement.UpdatePosition( dir, speed, false );
						PlayAni( eAnimation.Run );
					}
					else {
						PlayAni( eAnimation.Idle01 );
					}
				}
			}
		}
	}

	protected override void LateUpdate() {
		base.LateUpdate();

		if ( m_actionSystem.IsCurrentAction( eActionCommand.Die ) ) {
			mLeftParticle.gameObject.SetActive( false );
			mRightParticle.gameObject.SetActive( false );
		}
		else {
			if ( !mLeftParticle.gameObject.activeSelf ) {
				mLeftParticle.gameObject.SetActive( true );
			}

			if ( !mRightParticle.gameObject.activeSelf ) {
				mRightParticle.gameObject.SetActive( true );
			}
		}
	}

	public override void Retarget() {
		base.Retarget();

		if ( Guardian ) {
			Guardian.Retarget();
		}
	}

	private UnitCollider GetDirAngleToEnvObject() {
		UnitCollider unitCollider = null;

		Vector3 dir = Utility.GetDirWithoutY( mAutoMoveDest, transform.position );
		cmptRotate.UpdateRotation( dir, false );

		float dist = Utility.GetDistanceWithoutY( mAutoMoveDest, transform.position );
		Unit target = World.Instance.EnemyMgr.GetNearestTargetForEnvObject( this, dist, 45.0f );
		if ( target ) {
			unitCollider = target.MainCollider;
			SetMainTarget( target );
		}
		else {
			m_mainTarget = null;
		}

		return unitCollider;
	}

	public override void ChangeWeapon( bool init ) {
		base.ChangeWeapon( init );

		Vector3 prevGuardianPosition = transform.position - transform.forward;
		Quaternion prevGuardianRotation = transform.rotation;

		if ( Guardian ) {
			prevGuardianPosition = Guardian.transform.position;
			prevGuardianRotation = Guardian.transform.rotation;
		}

		costumeUnit.SetGuardianOwnerPlayer( mCurWeaponIndex, this );

		if ( Guardian != null ) {
			if ( Guardian.gameObject.layer != (int)eLayer.PlayerClone ) {
				Utility.SetLayer( Guardian.gameObject, (int)eLayer.PlayerClone, true );
			}

			Guardian.SetInitialPosition( prevGuardianPosition, prevGuardianRotation );
			Guardian.ResetBT();
		}

		OnAfterAttackPower();
	}

	public override void SetAutoMove( bool on ) {
		if ( !IsActivate() || ( AI == null && mIsAuto == false ) || World.Instance.StageType == eSTAGETYPE.STAGE_PVP || m_curHp <= 0.0f ) {
			return;
		}

		if ( on && Input && Input.isPause ) {
			Utility.StopCoroutine( this, ref mCrUpdateFindPath );
			IsAutoMove = false;

			return;
		}

		IsAutoMove = on;

		if ( on ) {
			StopBT();

			Utility.StopCoroutine( this, ref mCrUpdateFindPath );
			mCrUpdateFindPath = StartCoroutine( UpdateFindPath() );
		}
		else {
			Utility.StopCoroutine( this, ref mCrUpdateFindPath );

			SendStopInputAutoEvent();
			StartBT();
		}
	}

	protected override IEnumerator UpdateFindPath() {
		BattleAreaManager battleAreaMgr = World.Instance.EnemyMgr as BattleAreaManager;
		if ( battleAreaMgr == null ) {
			SetAutoMove( false );
			yield break;
		}

		BattleArea battleArea = null;

		// 스테이지 시작 전
		if ( battleAreaMgr.CurBattleAreaIndex == -1 ) {
			battleArea = battleAreaMgr.GetBattleArea( 0 );
			mAutoMoveDest = battleArea.GetCenterPos();

			battleArea = null;
		}
		// 스테이지 시작 후
		else {
			battleArea = battleAreaMgr.GetCurrentBattleArea();

			// 현재 배틀 에어리어가 꺼져 있으면 PortalEntry 사용 안하고 적 다 잡은 상태
			if ( !battleArea.gameObject.activeSelf ) {
				BattleArea nextBattleArea = battleAreaMgr.GetNextBattleAreaOrNull();
				if ( nextBattleArea ) {
					if ( nextBattleArea.portalEntry ) {
						mAutoMoveDest = nextBattleArea.portalEntry.transform.position;
					}
					else if ( nextBattleArea.kPathEffect && nextBattleArea.kPathEffect.activeSelf ) {
						mAutoMoveDest = nextBattleArea.kPathEffect.transform.position;
					}
					else { // 최종 목적지는 다음 배틀 에어리어의 센터
						mAutoMoveDest = nextBattleArea.GetCenterPos();
					}
				}
			}
			// 현재 배틀 에어리어가 켜져 있고 포털 엔트리도 켜져 있으면 포털 엔트리를 최종 목적지로 함.
			else if ( battleArea.portalEntry && battleArea.portalEntry.gameObject.activeSelf ) {
				mAutoMoveDest = battleArea.portalEntry.transform.position;
			}
			else {
				mAutoMoveDest = transform.position;

				SetAutoMove( false );
				SendStopInputAutoEvent();

				// 적도 없으면 그냥 나가자
				yield break;
			}
		}

		List<Vector3> listPathPoint = null;
		if ( battleArea ) {
			listPathPoint = battleArea.GetPathPointListOrNull( transform.position );
			if ( listPathPoint == null ) { // 배틀 에어리어에 패쓰가 없으면 맵에 기본으로 깔린걸 가져옴
				listPathPoint = PathMgr.Instance.GetPathPointListOrNull( transform.position );
			}
		}

		if ( listPathPoint == null ) { // 패쓰가 맵에도 없고 배틀 에어리어에도 없으면 목적지만 설정
			listPathPoint = new List<Vector3>();
		}

		listPathPoint.Add( mAutoMoveDest );

		if ( listPathPoint.Count <= 0 ) {
			SetAutoMove( false );
			yield break;
		}

		while ( !isGrounded ) {
			yield return null;
		}

		int curIndex = 0;
		Vector3 curDest = listPathPoint[curIndex];
		curDest.y = transform.position.y;

		mBeforeInputDir = Vector3.zero;

		while ( true ) {
			if ( isGrounded ) {
				if ( AI && World.Instance.EnemyMgr.HasAliveMonster() ) {
					SetAutoMove( false );
					SendStopInputAutoEvent();

					break;
				}
				else {
					Vector3 finalDest = listPathPoint[listPathPoint.Count - 1];
					finalDest.y = curDest.y = transform.position.y;

					if ( Vector3.Distance( transform.position, curDest ) <= 0.2f ) {
						++curIndex;
						if ( curIndex >= listPathPoint.Count ) {
							SetAutoMove( false );
							SendStopInputAutoEvent();

							break;
						}

						curDest = listPathPoint[curIndex];
						curDest.y = transform.position.y;
					}
					else if ( Vector3.Distance( transform.position, mAutoMoveDest ) <= 0.2f ) {
						SetAutoMove( false );
						SendStopInputAutoEvent();

						break;
					}
				}

				mInputDir = ( curDest - transform.position ).normalized * 1000.0f;
				SendInputAutoEvent();
			}

			yield return null;
		}
	}

	public override void OnGameStart() {
		base.OnGameStart();

		if ( Guardian ) {
			if ( mWorldPVP ) {
				Guardian.aniEvent.Reset();
			}
			else {
				Guardian.StopBT();
			}
		}
	}

	public override void OnMissionStart() {
		base.OnMissionStart();

		if ( mWorldPVP == null && Guardian ) {
			Utility.SetLayer( Guardian.gameObject, (int)eLayer.PlayerClone, true );
			Guardian.StartBT();
		}
	}

	public override void OnGameEnd() {
		base.OnGameEnd();

		Utility.SetLayer( Guardian.gameObject, (int)eLayer.Player, true );
	}

	protected override void OnEventDefence( IActionBaseParam param = null ) {
		if ( m_actionSystem.IsCurrentUSkillAction() == true ) {
			return;
		}

		if ( CheckTimingHoldAttack() ) {
			return;
		}

		ActionDash actionDash = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionDash;
		ActionSelectSkillBase actionSkill = m_actionSystem.GetAction<ActionSelectSkillBase>( eActionCommand.Teleport );
		if ( actionSkill && actionSkill.PossibleToUse && actionDash && actionDash.IsPossibleToDashAttack() && GetMainTargetCollider( true ) ) {
			float checkTime = actionDash.GetEvadeCutFrameLength();
			if ( World.Instance.UIPlay.btnDash.deltaTime < checkTime ) {
				CommandAction( eActionCommand.Teleport, null );
				return;
			}
		}

		CommandAction( eActionCommand.Defence, param );
	}

	protected override void OnEventSpecialAtk() {
		if ( m_actionSystem.IsCurrentSkillAction() || m_actionSystem.IsCurrentUSkillAction() ) {
			return;
		}

		ActionBase action = m_actionSystem.GetAction( eActionCommand.AttackDuringAttack );
		if ( action == null ) {
			return;
		}

		if ( m_actionSystem.IsCurrentAction( eActionCommand.Attack01 ) ) {
			if ( !IsHelper ) {
				World.Instance.UIPlay.btnAtk.lockCharge = true;
			}

			CommandAction( eActionCommand.AttackDuringAttack, null );
		}
	}

	public override bool OnHit( Unit attacker, BattleOption.eToExecuteType toExecuteType, AniEvent.sEvent attackerAniEvt, ObscuredFloat damage, ref bool isCritical, ref eHitState hitState, Projectile projectile, bool isUltimateSkill, bool skipMaxDamageRecord ) {
		bool isResult = base.OnHit( attacker, toExecuteType, attackerAniEvt, damage, ref isCritical, ref hitState, projectile, isUltimateSkill, skipMaxDamageRecord );

		if ( AutoGuardianSkill ) {
			ActionShisuiEmergencyAttack actionShisuiEmergencyAttack = actionSystem.GetAction( eActionCommand.EmergencyAttack ) as ActionShisuiEmergencyAttack;
			if ( actionShisuiEmergencyAttack && actionShisuiEmergencyAttack.PossibleToUse && Guardian ) {
				if ( mIsAuto ) {
					if ( !Guardian.actionSystem.IsCurrentSkillAction() ) {
						Guardian.actionSystem.CancelCurrentAction();
						Guardian.ResetBT();
						Guardian.CommandAction( eActionCommand.EmergencyAttack, null );
					}
				}
				else {
					Guardian.actionSystem.CancelCurrentAction();
					Guardian.ResetBT();
					Guardian.CommandAction( eActionCommand.EmergencyAttack, null );
				}
			}
		}

		return isResult;
	}

	public void SetAuto( bool isAuto ) {
		mIsAuto = isAuto;

		if ( AppMgr.Instance.SceneType == AppMgr.eSceneType.Training ) {
			AutoGuardianSkill = false;
			PlayAniImmediate( eAnimation.Idle01, eFaceAnimation.Idle01, 0 );
		}
	}
}
