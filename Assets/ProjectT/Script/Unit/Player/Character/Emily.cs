
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Emily : Player
{
    [Header("[Emily Property]")]
    public eAnimation[] DefaultAttackAnis;

    public int          CurAtkAniIndex          { get; set; }           = 0;
    public float        CheckTime               { get; set; }           = 0.0f;
    public bool         IsDroneDefaultAttack    { get; private set; }   = false;
    public DroneUnit    Drone                   { get; private set; }   = null;

    private Vector3                     mDir                    = Vector3.zero;
    private float                       mAniLength              = 0.0f;
    private bool                        mbDroneAutoAttack       = false;
    private float                       mDroneAddedAttackRatio  = 0.0f;
    private float                       mDroneAttackRatio       = 1.0f;
    private bool                        mbHasAutoMove           = false;
    private bool                        mbHasAutoEvade          = false;
    private ActionEmilyMoveByDirection  mActionMove             = null;
    private ActionEmilyHomingAttack     mActionHomingAtk        = null;
    private WorldPVP                    mWorldPVP               = null;
    private float                       mCheckTime              = 0.0f;
    private ActionParamAI               mParamAI                = new ActionParamAI( global::eDirection.RandomByTargetDirection, 0.0f, 0.0f, 
                                                                                     eActionCommand.None, false, false );


    public override void Init(int tableId, eCharacterType type, string faceAniControllerPath)
    {
        base.Init(tableId, type, faceAniControllerPath);
        mWorldPVP = World.Instance as WorldPVP;

        ActionDash actionDash = m_actionSystem.GetAction<ActionDash>(eActionCommand.Defence);
        if(actionDash)
        {
            actionDash.cancelDuringSameActionCount = 0; // 이걸 왜 코드에 박아놨지????
        }
    }

    public override void OnAfterCreateInPVP()
    {
        base.OnAfterCreateInPVP();

        if(World.Instance.StageType != eSTAGETYPE.STAGE_PVP)
        {
            return;
        }

        if(Drone == null)
        {
            OnAfterChangeWeapon();
        }

        Drone.Deactivate();
    }

    public override void OnGameStart()
    {
        base.OnGameStart();

        CheckEmilyAttack();
        OnAfterChangeWeapon();

        // 드론 생성 (연출용 클론도 생성)
        ListUnitWeaponClone.Clear();
        for (int i = 0; i < (int)eCOUNT.WEAPONSLOT; i++)
        {
            /*
            if(mWeaponIds[i] <= 0)
            {
                continue;
            }
            */

            CostumeUnit.sWeaponItem weaponItem = m_costumeUnit.GetWeaponItemOrNull(i);
            if (weaponItem == null)
            {
                continue;
            }

            List<Unit> list = new List<Unit>();
            for (int j = 0; j < UnitWeaponCloneCount; j++)
            {
                WeaponData data = GameInfo.Instance.GetWeaponData(weaponItem.UID);
                if (data == null || data.TableData == null)
                {
                    data = weaponItem.Data;
                    if (data == null || data.TableData == null)
                    {
                        continue;
                    }
                }

                string dronePath = string.Empty;

                if(this.charData.EquipWeaponSkinTID != (int)eCOUNT.NONE)
                {
                    GameTable.Weapon.Param skinWeapon = GameInfo.Instance.GameTable.FindWeapon(x => x.ID == this.charData.EquipWeaponSkinTID);
                    if (skinWeapon != null)
                        dronePath = skinWeapon.AddedUnitWeapon;
                    else
                        dronePath = data.TableData.AddedUnitWeapon;
                }
                else
                {
                    dronePath = data.TableData.AddedUnitWeapon;
                }
                Unit drone = GameSupport.CreateUnitWeapon(dronePath);
                drone.SetClone();

                list.Add(drone);
            }

            ListUnitWeaponClone.Add(list);
        }

        mActionMove = m_actionSystem.GetAction<ActionEmilyMoveByDirection>(eActionCommand.MoveByDirection);
        if(mActionMove)
        {
            mActionMove.SkipStopAni = mbHasAutoMove;
        }
    }

    public override void OnMissionStart()
    {
        base.OnMissionStart();

        if (!Drone.IsActivate())
        {
            Drone.Activate();
        }

        SetDroneInitialPos();
        Drone.CommandAction(eActionCommand.DroneFollowOwner, null);

        if (mbDroneAutoAttack)
        {
            OnEventAtkPressStart();
        }
    }

	public override bool OnEvent( BaseEvent evt, IActionBaseParam param = null ) {
		if( base.OnEvent( evt, param ) ) {
            return false;
		}

        switch( evt.eventType ) {
            case eEventType.EVENT_SET_EXTRA_PLAYER_FUNC:
                mbHasAutoEvade = true;
                return true;
        }

        return true;
	}

	public override void OnEndGame()
    {
        base.OnEndGame();

        if (Drone)
        {
            Drone.Deactivate();
        }
    }

    public override void OnAfterPVPBattle()
    {
        base.OnAfterPVPBattle();

        if (Drone)
        {
            Drone.Deactivate();
        }
    }

	public override void ChangeWeapon( bool init ) {
        mbHasAutoEvade = false;
		base.ChangeWeapon( init );
	}

	public override void SetAutoMove( bool on ) {
		if( !IsActivate() || ( !AI && !mbHasAutoMove ) || World.Instance.StageType == eSTAGETYPE.STAGE_PVP ) {
			return;
		}

		if( on && ( Input.isPause || Drone.mainTarget || ( m_actionSystem.currentAction && m_actionSystem.currentAction != mActionMove ) ) ) {
			Utility.StopCoroutine( this, ref mCrUpdateFindPath );
			IsAutoMove = false;

			return;
		}

		IsAutoMove = on;

		if( on ) {
			StopBT();

			Utility.StopCoroutine( this, ref mCrUpdateFindPath );
			mCrUpdateFindPath = StartCoroutine( UpdateFindPath() );
		}
		else {
			Utility.StopCoroutine( this, ref mCrUpdateFindPath );

			mInputDir = Vector3.zero;
			SendInputAutoEvent();

			StartBT();
		}
	}

	public override void Deactivate() {
		base.Deactivate();

        if( Drone ) {
            Drone.Deactivate();
        }
	}

	public void CheckEmilyAttack() {
		mDroneAttackRatio = 1.0f;
		mbHasAutoMove = false;
		mbDroneAutoAttack = false;

		ActionEmilyAttack actionAttack = m_actionSystem.GetAction<ActionEmilyAttack>( eActionCommand.Attack01 );
		if( actionAttack ) {
			mDroneAddedAttackRatio = actionAttack.GetValue1() / (float)eCOUNT.MAX_BO_FUNC_VALUE;
			mDroneAttackRatio = 0.5f;

			if( actionAttack.GetValue2() > 0.0f ) {
				mbHasAutoMove = true;
			}

			mbDroneAutoAttack = true;
		}

		if( World.Instance.StageType == eSTAGETYPE.STAGE_PVP ) {
			mbDroneAutoAttack = true;
			mbHasAutoMove = false;
		}
		else {
            if( mAI ) {
                mbDroneAutoAttack = true;
                mbHasAutoMove = true;
            }
			else if( !mbDroneAutoAttack )
				EndDroneAttack();
		}
	}

	public override void OnAfterChangeWeapon()
    {
        CostumeUnit.sWeaponItem weaponItem = m_costumeUnit.GetWeaponItemOrNull(mCurWeaponIndex);
        if (weaponItem != null)
        {
            Drone = weaponItem.AddedUnitWeapon as DroneUnit;
            if (Drone == null)
            {
                Debug.LogError(weaponItem.AddedUnitWeapon.name + "이 드론이 아닙니다.");
                return;
            }

            SetDroneInitialPos();
            Drone.SetDroneUnit(DroneUnit.eDroneType.ByCharacter, this, -1, 0.0f, 0.3f);
            Drone.SetDroneAttackPower((m_attackPower * (GameInfo.Instance.BattleConfig.EmilyDroneAttackRatio + mDroneAddedAttackRatio)) * mDroneAttackRatio);

            if (!Drone.actionSystem.IsCurrentAction(eActionCommand.DroneFollowOwner) && Drone.IsActivate())
            {
                Drone.CommandAction(eActionCommand.DroneFollowOwner, null);
            }

            if (gameObject.layer == (int)eLayer.Enemy)
            {
                Utility.SetLayer(Drone.gameObject, (int)eLayer.EnemyClone, false);
            }
        }

        IsDroneDefaultAttack = false;
        mActionHomingAtk = m_actionSystem.GetAction<ActionEmilyHomingAttack>(eActionCommand.AttackDuringAttack);
    }

    public override bool ShowMesh(bool show, bool isLock = false)
    {
        bool isShow = base.ShowMesh(show, isLock);

        if (Drone)
        {
            Drone.ShowMesh(show, isLock);
        }

        return isShow;
    }

    public override void ReleaseLockShowMesh()
    {
        base.ReleaseLockShowMesh();

        if (Drone)
        {
            Drone.ReleaseLockShowMesh();
        }
    }

	public override void Retarget() {
		base.Retarget();

		if (Drone) {
			Drone.Retarget();
		}
	}

    public override void OnAfterChangePlayableChar() {
        CheckEmilyAttack();
    }

    protected override void OnEventAtk()
    {
        ActionDash actionDash = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionDash;
        if (actionDash && actionDash.IsPossibleToDashAttack())
        {
            CommandAction(eActionCommand.RushAttack, null);
        }
    }

    protected override void OnEventDefence( IActionBaseParam param = null )
    {
        if (m_actionSystem.IsCurrentUSkillAction() == true)
        {
            return;
        }

        ActionDash actionDash = m_actionSystem.currentAction == null ? null : m_actionSystem.currentAction as ActionDash;
        ActionSelectSkillBase actionTeleport = m_actionSystem.GetAction<ActionSelectSkillBase>(eActionCommand.Teleport);
        if (actionTeleport && actionTeleport.PossibleToUse && actionDash && actionDash.IsPossibleToDashAttack() && GetMainTargetCollider(true))
        {
            float checkTime = actionDash.GetEvadeCutFrameLength();
            if (World.Instance.UIPlay.btnDash.deltaTime < checkTime)
            {
                CommandAction(eActionCommand.Teleport, null);
                return;
            }
        }

        CommandAction( eActionCommand.Defence, param );
    }

    protected override void OnEventAtkPressStart()
    {
        if(IsDroneDefaultAttack || Drone.mainTarget == null || skipAttack)// || (m_actionSystem.currentAction && m_actionSystem.currentAction.actionCommand != eActionCommand.MoveByDirection))
        {
            return;
        }

        IsDroneDefaultAttack = true;

        if (!Drone.actionSystem.IsCurrentAction(eActionCommand.Attack01))
        {
            Drone.CommandAction(eActionCommand.Attack01, null);
        }
    }

    /*
    protected override void OnEventAtkPressEnd()
    {
        if(mbDroneAutoAttack)
        {
            return;
        }

        EndDroneAttack();
    }
    */

    protected override void OnEventAtkTouchEnd()
    {
        if (mbDroneAutoAttack)
        {
            return;
        }

        EndDroneAttack();
    }

	protected override void FixedUpdate() {
		base.FixedUpdate();

		if( !Director.IsPlaying && Drone && Vector3.Distance( transform.position, Drone.transform.position ) > 5.0f ) {
			SetDroneInitialPos();
		}

		if( ( mWorldPVP && !mWorldPVP.IsBattleStart ) || World.Instance.IsEndGame || m_curHp <= 0.0f || ( Drone && !Drone.IsShowMesh ) ) {
			EndDroneAttack();
			return;
		}

		if( !IsMissionStart || Director.IsPlaying || ( Input && Input.isPause ) ) {
			if( Director.IsPlaying ) {
				OnEventAtkTouchEnd();
			}

			CheckTime = 0.0f;
			CurAtkAniIndex = 0;

			return;
		}

		if( mbHasAutoMove ) {
			if( !IsAutoMove && m_actionSystem.currentAction == null && !World.Instance.EnemyMgr.HasAliveMonster() ) {
				SetAutoMove( true );
			}
			else if( IsAutoMove && Drone.mainTarget ) {
				SetAutoMove( false );
			}
		}

		if( mbDroneAutoAttack && mbHasAutoEvade && Drone.mainTarget ) {
			mCheckTime += Time.fixedDeltaTime;
			if( mCheckTime >= 1.5f && m_aniEvent.aniSpeed >= 1.0f && !ExtreamEvading ) {
				if( UnityEngine.Random.Range( 0, 100 ) < 30 ) {
					OnEventDefence( mParamAI );
				}

				mCheckTime = 0.0f;
			}
		}

		if( IsDroneDefaultAttack && Drone.mainTarget == null ) {
			EndDroneAttack();
			SetAutoMove( true );
		}
		else if( skipAttack ) {
			EndDroneAttack();
			Drone.SetMainTarget( null );
		}
		else if( mbDroneAutoAttack && !IsDroneDefaultAttack ) {
			if( Drone.mainTarget ) {
				OnEventAtkPressStart();
			}
			else {
				if( Drone.GetMainTargetCollider( true ) == null && World.Instance.EnemyMgr.HasAliveMonster() && mbHasAutoMove ) {
                    SetAutoMove( true );
				}
			}
		}

		if( IsDroneDefaultAttack && Drone.mainTarget && !World.Instance.EnemyMgr.IsAllMonsterDeactive() ) {
			if( m_actionSystem.BeforeActionCommand == eActionCommand.Hit && m_actionSystem.currentAction == null && !IsCurrentAniAttack() ) {
				CheckTime = 0.0f;
				CurAtkAniIndex = 0;

				mAniLength = PlayAni( DefaultAttackAnis[CurAtkAniIndex] );
			}
			else if( m_actionSystem.currentAction == null ) {
				if( m_aniEvent.curAniType == eAnimation.Idle01 ) {
					CheckTime = 0.0f;
					CurAtkAniIndex = 0;

					mAniLength = PlayAni( DefaultAttackAnis[CurAtkAniIndex] );
				}

				CheckTime += fixedDeltaTime;
				if( CheckTime >= mAniLength ) {
					++CurAtkAniIndex;
					if( CurAtkAniIndex >= DefaultAttackAnis.Length ) {
						CurAtkAniIndex = DefaultAttackAnis.Length - 1;
					}

					mAniLength = PlayAni( DefaultAttackAnis[CurAtkAniIndex] );
					CheckTime = 0.0f;
				}

				Vector3 lookAt = Utility.GetDirWithoutY(Drone.mainTarget.transform.position, transform.position);
				m_cmptRotate.UpdateRotation( lookAt, true );
			}

			if( mActionHomingAtk && mActionHomingAtk.PossibleToUse ) {
				CommandAction( eActionCommand.AttackDuringAttack, null );
			}
		}
	}

	private void EndDroneAttack()
    {
        if(!IsDroneDefaultAttack)
        {
            return;
        }

        IsDroneDefaultAttack = false;

        mAniLength = 0.0f;
        CheckTime = 0.0f;
        CurAtkAniIndex = 0;

        IgnoreAction = eActionCommand.None;
        SetSpeedRateBySprint(0.0f);

        if (!Drone.actionSystem.IsCurrentAction(eActionCommand.DroneFollowOwner) && Drone.IsActivate())
        {
            Drone.CommandAction(eActionCommand.DroneFollowOwner, null);
        }

        if ((m_actionSystem.currentAction == null || m_actionSystem.currentAction.actionCommand == eActionCommand.AttackDuringAttack) && m_curHp > 0.0f)
        {
            PlayAni(eAnimation.Idle01);

            if (Input.GetRawDirection() != Vector3.zero)
            {
                CommandAction(eActionCommand.MoveByDirection, new ActionParamMoveByDirection(Input.GetRawDirection(), true));
            }
        }
    }

    private void SetDroneInitialPos()
    {
        if(Drone == null)
        {
            return;
        }

        Vector3 v = transform.position;
        v.y += MainCollider.height * 1.2f;
        Vector3 refPos = v + (transform.forward * 0.5f) + (transform.right * 1.0f);

        Drone.SetInitialPosition(refPos, transform.rotation);
    }

    private bool IsCurrentAniAttack()
    {
        for(int i = 0; i < DefaultAttackAnis.Length; i++)
        {
            if(DefaultAttackAnis[i] == m_aniEvent.curAniType)
            {
                return true;
            }
        }

        return false;
    }
}
