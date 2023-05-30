
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine.Timeline;


public class WorldStage : World
{
    public GameTable.ArenaTower.Param		        TowerStageData	    { get; private set; } = null;
    public GameTable.SecretQuestLevel.Param         SecretQuestData     { get; private set; } = null;
    public List<GameClientTable.StageBOSet.Param>   RaidStageBOSetList  { get; private set; } = null;

    private int mCurrentPlayerIndex = 0;


	public override void Init( int tableId, eSTAGETYPE stageType = eSTAGETYPE.STAGE_NONE ) {
		base.Init( tableId, stageType );

		if( !GameInfo.Instance.IsTowerStage ) {
			if( stageType == eSTAGETYPE.STAGE_SECRET ) {
				InitSecretStage();
			}
			else {
				InitStage();
			}
		}
		else {
			InitTowerStage( tableId );
		}

		PostInit();

		if( EnemyMgr && StageData != null ) {
			if( stageType != eSTAGETYPE.STAGE_SECRET && stageType != eSTAGETYPE.STAGE_RAID ) {
				EnemyMgr.AddStageBOSet( StageData.StageBOSet );
			}
			else if( stageType == eSTAGETYPE.STAGE_RAID ) {
				List<GameClientTable.RaidBOSet.Param> raidBOSetList = GameInfo.Instance.GameClientTable.FindAllRaidBOSet( x => x.RaidStep <= GameInfo.Instance.SelectedRaidLevel );
                GameClientTable.RaidBOSet.Param raidBOSetParam = raidBOSetList[raidBOSetList.Count - 1];

                RaidStageBOSetList = GameInfo.Instance.GameClientTable.FindAllStageBOSet( x => x.Group == raidBOSetParam.StageBOSet );

                while( RaidStageBOSetList.Count > raidBOSetParam.Count ) {
                    RaidStageBOSetList.RemoveAt( UnityEngine.Random.Range( 0, RaidStageBOSetList.Count ) );
                }

				for( int i = 0; i < RaidStageBOSetList.Count; i++ ) {
					EnemyMgr.AddStageBOSet( RaidStageBOSetList[i] );
				}
			}
			else {
				EnemyMgr.AddStageBOSet( SecretQuestStageBOSetId );
			}
		}
	}

	public override void ShowResultUI() {
        if ( mShowResult ) {
            return;
        }

        for( int i = 0; i < ListPlayer.Count; i++ ) {
            if( ListPlayer[i] != Player ) {
                ListPlayer[i].ShowMesh( false );
			}
		}

		mShowResult = true;
		base.ShowResultUI();

		if ( GameSupport.IsInGameTutorial() ) {
			eTutorialState tutorialState = (eTutorialState)GameInfo.Instance.UserData.GetTutorialState();
            if ( tutorialState == eTutorialState.TUTORIAL_STATE_Init ) {
                GameSupport.SendFireBaseLogEvent( eFireBaseLogType._1_1_Clear );
            }
            else if ( tutorialState == eTutorialState.TUTORIAL_STATE_Stage2Clear ) {
                GameSupport.SendFireBaseLogEvent( eFireBaseLogType._1_2_Clear );
            }
            else if ( tutorialState == eTutorialState.TUTORIAL_STATE_Stage3Clear ) {
                GameSupport.SendFireBaseLogEvent( eFireBaseLogType._1_3_Clear );
            }
		}

		Director clearDirector = Player.GetDirector("Win01");
		if ( clearDirector && clearDirector.CharLight ) {
			Director.IsPlaying = false;
			InGameCamera.SetPlayerLightPositionAndRotation( clearDirector.CharLight.transform.position, clearDirector.CharLight.transform.rotation.eulerAngles, false );
		}

		Player.PlayAniImmediate( eAnimation.Win, GetAnimationSync( clearDirector, Player.aniEvent.GetAniInfo( eAnimation.Win ) ) );
		Player.PlayFaceAni( eFaceAnimation.WinIdle );

        if ( Player.UseGuardian == true && Player.Guardian != null ) {
			Player.Guardian.StopBT();
			Player.Guardian.PlayAniImmediate( eAnimation.Win, GetAnimationSync( clearDirector, Player.Guardian.aniEvent.GetAniInfo( eAnimation.Win ) ) );
		}

		TimeSpan ts = TimeSpan.FromSeconds(ProgressTime);

		if ( !GameInfo.Instance.IsTowerStage ) {
            GameInfo.Instance.MaxNormalBoxCount = Mathf.Min( GameInfo.Instance.MaxNormalBoxCount, StageData.N_DropMaxCnt );

            if( StageData != null && StageData.StageType == (int)eSTAGETYPE.STAGE_RAID ) {
                GameInfo.Instance.Send_ReqRaidStageEnd( (int)ts.TotalMilliseconds, Player.goldCount, GameInfo.Instance.MaxNormalBoxCount, OnNetGameResult );
            }
            else {
                Mission01 = CheckStageMission( 0, StageData.Mission_00 );
                Mission02 = CheckStageMission( 1, StageData.Mission_01 );
                Mission03 = CheckStageMission( 2, StageData.Mission_02 );

                GameInfo.Instance.Send_ReqStageEnd( StageData.ID, GameInfo.Instance.SeleteCharUID, (int)ts.TotalMilliseconds, Player.goldCount,
                                                    GameInfo.Instance.MaxNormalBoxCount,
                                                    Mission01 != EMissionClearStatus.FAIL ? true : false,
                                                    Mission02 != EMissionClearStatus.FAIL ? true : false,
                                                    Mission03 != EMissionClearStatus.FAIL ? true : false,
                                                    OnNetGameResult );
            }
        }
		else {
            if ( !GameInfo.Instance.IsTowerStageTestPlay ) {
                GameInfo.Instance.Send_PktInfoArenaTowerGameEndReq( (uint)ts.TotalMilliseconds, true, OnNetGameResult );
            }
            else {
                OnNetGameResult( 0, null );
            }
		}
	}

    public override void OnEndResult() {
        GameUIManager.Instance.HideUI( "GameResultPanel", false );
        GameUIManager.Instance.HideUI( "GameRewardPopup", false );

        if( HasDirector( "EndMission" ) ) {
            PlayDirector( "EndMission", OnEndMissionDirector );
        }
        else {
            OnEndMissionDirector();
        }
    }

    public void ShowScenario( int scenarioGroupId, Action endCallback ) {
        if( scenarioGroupId <= 0 ) {
            return;
        }

        if( StageData.Difficulty > 1 ) {
            endCallback?.Invoke();
            return;
        }

        Pause( true, false );
        StopCoroutine( "ResumeAll" );

		//GameUIManager.Instance.HideUI( "GameOffPopup", false );
		GameUIManager.Instance.ShowUI( "GameOffPopup", false );
		GameUIManager.Instance.HideUI( "GamePlayPanel", false );

        ScenarioMgr.Instance.Open( scenarioGroupId, endCallback );

        // 씬 로드하고 한프레임정도 씬을 비출때가 있어서 시나리오 오픈할 때 카메라 Disable해주고 0.5초 후에 켜줌
        Invoke( "EnableMainCamera", 0.5f );
    }

    public void OnNetGameResult( int result, PktMsgType pktmsg ) {
		if ( result != 0 ) {
			Debug.LogError( "Result가 0이 아님!!!!!!!!" );
			return;
		}

		EndTutorial();
		FSaveData.Instance.RemoveStageData();

		Debug.Log( "[OnNetGameResult]" );
		if ( Player.HasDirector( "Win01" ) == false ) {
			ChangeToFinishBGM( true );
		}

		if ( GameInfo.Instance.IsTowerStage && !GameInfo.Instance.IsTowerStageTestPlay ) {
			PktInfoArenaTowerGameEndAck towerGameEnd = pktmsg as PktInfoArenaTowerGameEndAck;
			if ( towerGameEnd != null ) {
				GameInfo.Instance.ApplyProduct( towerGameEnd.products_, false, false );
				GameInfo.Instance.ClearArenaTower();
			}
		}

		GameUIManager.Instance.HideUI( "GamePlayPanel", false );
		GameUIManager.Instance.ShowUI( "GameResultPanel", true );
	}

    public void RestoreIngameCamera() {
		if ( EnemyMgr.UseCameraSetting ) {
			if ( EnemyMgr.CameraMode == InGameCamera.EMode.DEFAULT ) {
				InGameCamera.SetMode( EnemyMgr.CameraMode, EnemyMgr.CameraDefaultSetting );
			}
			else if ( EnemyMgr.CameraMode == InGameCamera.EMode.SIDE ) {
				InGameCamera.SetMode( EnemyMgr.CameraMode, EnemyMgr.CameraSideSetting );
			}
			else if ( EnemyMgr.CameraMode == InGameCamera.EMode.FIXED ) {
				InGameCamera.SetMode( EnemyMgr.CameraMode, EnemyMgr.CameraFixedSetting );
			}
			else if ( EnemyMgr.CameraMode == InGameCamera.EMode.FOLLOW_PLAYER ) {
				InGameCamera.SetMode( EnemyMgr.CameraMode, EnemyMgr.CameraFollowPlayerSetting );
			}
		}
		else {
			InGameCamera.SetDefaultMode();
		}
	}

    protected override void EndIntro()
    {
        base.EndIntro();

        if (!GameInfo.Instance.IsTowerStage && !GameInfo.Instance.ContinueStage && StageData.ScenarioID_AfterStart > 0)
        {
            //ShowScenario(StageData.ScenarioID_AfterStart, OnEndAfterStartScenario);
            if ( StageData.ShowPlayerStartDrt > 0 ) {
                Invoke( "DelayedShowAfterStartSecnario", 0.1f );
            }
            else {
                ShowScenario( StageData.ScenarioID_AfterStart, OnEndAfterStartScenario );
            }
        }
        else
        {
            OnEndAfterStartScenario();
        }
    }

    protected void DelayedShowAfterStartSecnario() {
        ShowScenario( StageData.ScenarioID_AfterStart, OnEndAfterStartScenario );
	}

	protected void InitPlayer( bool isTutorial ) {
        bool isAutoPlay = false;
#if UNITY_EDITOR
        isAutoPlay = GameInfo.Instance.BattleConfig.AutoPlay;
#endif

        Player = GameSupport.CreatePlayer( GameSupport.GetGameSeleteCharUID(), false, false, false, isAutoPlay );
		if( Player != null ) {
			Utility.SetPhysicsLayerCollision( eLayer.Player, eLayer.Enemy );
			Utility.SetPhysicsLayerCollision( eLayer.Player, eLayer.EnemyGate );

			InGameCamera.SetPlayer( Player );

			Vector3 startPos = Vector3.zero;
			Quaternion startRot = Quaternion.identity;
			EnemyMgr.GetStartPoint( out startPos, out startRot );

			Player.SetInitialRigidBodyPos( startPos, startRot );
			Player.AddDirector();

            if ( Player.UseGuardian == true && Player.Guardian != null ) {
				Player.Guardian.SetInitialPosition( startPos, startRot );
			}

			InGameCamera.SetInitialDefaultMode();

			if( GameInfo.Instance.SelectedCardFormationTID > 0 ) {
				GameTable.CardFormation.Param param = GameInfo.Instance.GameTable.FindCardFormation( GameInfo.Instance.SelectedCardFormationTID );
				if( param != null ) {
					Player.AddBOCharBattleOptionSet( param.FormationBOSetID1, 1 );
					Player.AddBOCharBattleOptionSet( param.FormationBOSetID2, 1 );
				}
			}

			for( int i = 0; i < GameInfo.Instance.UserData.ListAwakenSkillData.Count; i++ ) {
				AwakenSkillInfo info = GameInfo.Instance.UserData.ListAwakenSkillData[i];
				if( info.Level <= 0 ) {
					continue;
				}

				GameTable.AwakeSkill.Param param = GameInfo.Instance.GameTable.FindAwakeSkill( info.TableId );
				if( param == null ) {
					Debug.LogError( info.TableId + "번 각성 스킬 정보가 없습니다." );
					continue;
				}

				Player.AddBOCharBattleOptionSet( param.SptAddBOSetID1, info.Level );
				Player.AddBOCharBattleOptionSet( param.SptAddBOSetID2, info.Level );
			}

            Player.AutoSupporterSkill = FSaveData.Instance.RaidAutoSupporterSkill;
            Player.AutoWeaponSkill = FSaveData.Instance.RaidAutoWeaponSkill;
            Player.AutoUltimateSkill = FSaveData.Instance.RaidAutoUltimateSkill;

            ListPlayer.Add( Player );

            if( !isTutorial ) {
				if( !GameInfo.Instance.ContinueStage && StageData.ScenarioID_BeforeStart > 0 ) {
					Player.gameObject.SetActive( false );
					ShowScenario( StageData.ScenarioID_BeforeStart, OnEndBeforeStartScenario );
				}
				else {
					if( GameInfo.Instance.ContinueStage ) {
						int goldCount = PlayerPrefs.GetInt("SAVE_STAGE_PLAYER_GOLD_");
						Player.SetGoldCount( goldCount );
					}

					OnEndBeforeStartScenario();
				}
			}
            else {
                GameUIManager.Instance.ShowUI( "GameOffPopup", false );
            }
		}

        if( UseHelper || StageType == eSTAGETYPE.STAGE_RAID || GameInfo.Instance.IsRaidPrologue ) {
            SetHelper();
        }
    }

    protected void SetHelper() {
        /*
        if( TestHelperType == eTestHelperType.NONE ) {
            UIPlay.ShowSubCharBtns( false );
            return;
		}
        else if( TestHelperType == eTestHelperType.IN_MY_CHAR_LIST || TestHelperType == eTestHelperType.LOBBY_CHARS ) {
            UIPlay.ShowSubCharBtns( true );
            SetHelperDataInMyCharList();
		}
        */

        if( UseHelper ) {
            TestHelperType = eTestHelperType.MANUAL;
        }

        SetHelperDataInMyCharList();

        for( int i = 0; i < TestHelperDatas.Length; i++ ) {
            GameTable.Character.Param param = GameInfo.Instance.GameTable.FindCharacter( TestHelperDatas[i].TableId );
            if( param == null ) {
                Debug.LogError( TestHelperDatas[i].TableId + "번 캐릭터가 없습니다." );
                return;
            }

            CharData charData = GameInfo.Instance.GetCharDataByTableID( TestHelperDatas[i].TableId );
            if( charData == null ) {
                Debug.LogError( TestHelperDatas[i].TableId + "번 캐릭터 데이터가 없습니다." );
                return;
            }

            charData.Grade = Mathf.Max( 1, TestHelperDatas[i].Grade );
            charData.Level = Mathf.Max( TestHelperDatas[i].Level );

            Player helper = GameSupport.CreatePlayer( charData, false, false, true, false );
            if( helper == null ) {
                continue;
            }

            //helper.SetAction( true );
            helper.AddDirector();

            if( GameInfo.Instance.SelectedCardFormationTID > 0 ) {
                GameTable.CardFormation.Param cardFormationParam = GameInfo.Instance.GameTable.FindCardFormation( GameInfo.Instance.SelectedCardFormationTID );
                if( cardFormationParam != null ) {
                    helper.AddBOCharBattleOptionSet( cardFormationParam.FormationBOSetID1, 1 );
                    helper.AddBOCharBattleOptionSet( cardFormationParam.FormationBOSetID2, 1 );
                }
            }

            for( int j = 0; j < GameInfo.Instance.UserData.ListAwakenSkillData.Count; j++ ) {
                AwakenSkillInfo info = GameInfo.Instance.UserData.ListAwakenSkillData[j];
                if( info.Level <= 0 ) {
                    continue;
                }

                GameTable.AwakeSkill.Param awakeSkillParam = GameInfo.Instance.GameTable.FindAwakeSkill( info.TableId );
                if( awakeSkillParam == null ) {
                    Debug.LogError( info.TableId + "번 각성 스킬 정보가 없습니다." );
                    continue;
                }

                helper.AddBOCharBattleOptionSet( awakeSkillParam.SptAddBOSetID1, info.Level );
                helper.AddBOCharBattleOptionSet( awakeSkillParam.SptAddBOSetID2, info.Level );
            }

            helper.AutoSupporterSkill = FSaveData.Instance.RaidAutoSupporterSkill;
            helper.AutoWeaponSkill = FSaveData.Instance.RaidAutoWeaponSkill;
            helper.AutoUltimateSkill = FSaveData.Instance.RaidAutoUltimateSkill;

			if ( helper.UseGuardian == true && helper.Guardian != null ) {
				helper.Guardian.Deactivate();
			}

			helper.Deactivate();
            ListPlayer.Add( helper );

            UIPlay.SetSubCharUnit( i, helper );
        }
    }

    /// <summary>
    /// 내 캐릭터 리스트에서 헬퍼 선택
    /// </summary>
    protected void SetHelperDataInMyCharList() {
        if( GameInfo.Instance.IsRaidPrologue ) {
            TestHelperDatas = new sTestPlayerData[Mathf.Min( 2, GameInfo.Instance.RaidPrologueCharTidList.Count )];

            for( int i = 0; i < TestHelperDatas.Length; i++ ) {
                sTestPlayerData data = new sTestPlayerData();
                data.TableId = GameInfo.Instance.RaidPrologueCharTidList[i];
                TestHelperDatas[i] = data;
            }
        }
        else if( StageType == eSTAGETYPE.STAGE_RAID ) {
            TestHelperDatas = new sTestPlayerData[Mathf.Min( 2, GameInfo.Instance.RaidUserData.CharUidList.Count - 1 )];

            for( int i = 0; i < TestHelperDatas.Length; i++ ) {
                CharData charData = GameInfo.Instance.GetCharData( GameInfo.Instance.RaidUserData.CharUidList[i + 1] );

                sTestPlayerData data = new sTestPlayerData();
                data.TableId = charData.TableID;

                TestHelperDatas[i] = data;
            }
        }
        else {
            if( TestHelperType == eTestHelperType.IN_MY_CHAR_LIST ) {
                if( GameInfo.Instance.CharList.Count <= 1 ) {
                    return;
                }

                TestHelperDatas = new sTestPlayerData[Mathf.Min( 2, GameInfo.Instance.CharList.Count )];

                List<int> list = new List<int>();
                for( int i = 0; i < GameInfo.Instance.CharList.Count; i++ ) {
                    if( GameInfo.Instance.CharList[i].TableID == Player.tableId || GameInfo.Instance.CharList[i].EquipWeaponUID == 0 ) {
                        continue;
                    }

                    list.Add( i );
                }

                for( int i = 0; i < TestHelperDatas.Length; i++ ) {
                    int index = list[UnityEngine.Random.Range( 0, list.Count )];
                    list.Remove( index );

                    sTestPlayerData data = new sTestPlayerData();
                    data.TableId = GameInfo.Instance.CharList[index].TableID;
                    TestHelperDatas[i] = data;
                }
            }
            else if( TestHelperType == eTestHelperType.LOBBY_CHARS ) {
                List<int> list = new List<int>();

                if( Lobby.Instance.ListBGCharInfo != null ) {
                    for( int i = 1; i < Lobby.Instance.ListBGCharInfo.Count; i++ ) {
                        if( Lobby.Instance.ListBGCharInfo[i].BGChar != null ) {
                            list.Add( Lobby.Instance.ListBGCharInfo[i].BGChar.tableId );
                        }
                    }
                }

                TestHelperDatas = new sTestPlayerData[Mathf.Min( 2, list.Count )];

                for( int i = 0; i < TestHelperDatas.Length; i++ ) {
                    sTestPlayerData data = new sTestPlayerData();
                    data.TableId = list[i];
                    TestHelperDatas[i] = data;
                }
            }
        }

        for( int i = 0; i < TestHelperDatas.Length; i++ ) {
            CharData charData = GameInfo.Instance.CharList.Find( x => x.TableID == TestHelperDatas[i].TableId );
            if( charData == null ) {
                continue;
			}

            TestHelperDatas[i].Grade = charData.Grade;
            TestHelperDatas[i].Level = charData.Level;

            // 스킬 세팅
            TestHelperDatas[i].SkillTableIds = null;

            if( charData.EquipSkill.Length > 0 ) {
                TestHelperDatas[i].SkillTableIds = new int[charData.EquipSkill.Length];

                for( int j = 0; j < TestHelperDatas[i].SkillTableIds.Length; j++ ) {
                    TestHelperDatas[i].SkillTableIds[j] = charData.EquipSkill[j];
                }
            }

            // 서포터 세팅
            TestHelperDatas[i].SupporterDatas = null;

            if( charData.EquipCard.Length > 0 ) {
                TestHelperDatas[i].SupporterDatas = new sTestSupporterData[charData.EquipCard.Length];

                for( int j = 0; j < TestHelperDatas[i].SupporterDatas.Length; j++ ) {
                    CardData cardData = charData.GetEquipCard( (int)charData.EquipCard[j] );
                    if( cardData == null ) {
                        continue;
					}

                    TestHelperDatas[i].SupporterDatas[j] = new sTestSupporterData();
                    TestHelperDatas[i].SupporterDatas[j].Id = cardData.TableID;
                    TestHelperDatas[i].SupporterDatas[j].Level = cardData.Level;
                    TestHelperDatas[i].SupporterDatas[j].SkillLv = cardData.SkillLv;
                    TestHelperDatas[i].SupporterDatas[j].Wake = cardData.Wake;
                }
            }

            // 무기 세팅
            TestHelperDatas[i].WeaponDatas = null;

            WeaponData weaponData = GameInfo.Instance.GetWeaponData( charData.EquipWeaponUID );
            if( weaponData != null ) {
                TestHelperDatas[i].WeaponDatas = new sTestWeaponData[1];
                TestHelperDatas[i].WeaponDatas[0] = new sTestWeaponData();
                TestHelperDatas[i].WeaponDatas[0].Id = weaponData.TableID;
                TestHelperDatas[i].WeaponDatas[0].Level = weaponData.Level;
                TestHelperDatas[i].WeaponDatas[0].SkillLevel = weaponData.SkillLv;
                TestHelperDatas[i].WeaponDatas[0].Level = weaponData.Level;
                TestHelperDatas[i].WeaponDatas[0].Wake = weaponData.Wake;
            }

            // 코스튬 세팅
            TestHelperDatas[i].CostumeData = new sTestCostumeData();
            TestHelperDatas[i].CostumeData.Id = charData.EquipCostumeID;
            TestHelperDatas[i].CostumeData.Color = charData.CostumeColor;
        }
    }

    protected void OnEndBeforeStartScenario() {
		GameUIManager.Instance.HideUI( "GameOffPopup", false );

		bool hasStartDirector = HasDirector( "Start" );
		if( GameInfo.Instance.ContinueStage || !hasStartDirector ) {
			StartPlayerIntro();
		}
		else {
			for( int i = 0; i < ListPlayer.Count; i++ ) {
				ListPlayer[i].gameObject.SetActive( false );
			}

			if( !PlayDirector( "Start", StartPlayerIntro ) ) {
				EndIntro();
			}
		}
	}

	protected void OnEndAfterStartScenario() {
		ScenarioMgr.Instance.InitOnEndAction();
		GameUIManager.Instance.HideUI( "GameOffPopup", false );

		//BGM플레이 중이 아니면 플레이
		AudioSource bgmAudio = SoundManager.Instance.GetAudioSource( SoundManager.eSoundType.Primary );
		if( bgmAudio != null && !bgmAudio.isPlaying ) {
			base.EndIntro();
		}

		Vector3 basePos = Player.transform.position - ( Player.transform.forward * 1.0f );
		int evenNumCount = 1;
		int oddNumCount = 1;

        if( World.Instance.StageType != eSTAGETYPE.STAGE_TOWER ) {
            for( int i = 0; i < ListPlayer.Count; i++ ) {
                if( !ListPlayer[i].gameObject.activeSelf ) {
                    ListPlayer[i].gameObject.SetActive( true );
                }

                ListPlayer[i].ShowMesh( true );
                ListPlayer[i].OnGameStart();

                if( !ListPlayer[i].IsHelper ) {
                    continue;
                }

                if( i % 2 == 0 ) {
                    ListPlayer[i].SetInitialPosition( basePos - ( Player.transform.right * ( 1.0f * evenNumCount++ ) ), Player.transform.rotation );
                }
                else {
                    ListPlayer[i].SetInitialPosition( basePos + ( Player.transform.right * ( 1.0f * oddNumCount++ ) ), Player.transform.rotation );
                }

                if ( ListPlayer[i].Guardian ) {
					ListPlayer[i].Guardian.SetInitialPosition( ListPlayer[i].transform.position - ListPlayer[i].transform.forward, Player.transform.rotation );
				}
            }
        }
        else {
            if( !Player.gameObject.activeSelf ) {
                Player.gameObject.SetActive( true );
            }

            Player.ShowMesh( true );
            Player.OnGameStart();
        }

		UIPlay.SetWidgetsAlpha( 1.0f );

		if( EnemyMgr.UseCameraSetting ) {
			if( EnemyMgr.CameraMode == InGameCamera.EMode.DEFAULT ) {
				InGameCamera.SetMode( EnemyMgr.CameraMode, EnemyMgr.CameraDefaultSetting );
			}
			else if( EnemyMgr.CameraMode == InGameCamera.EMode.SIDE ) {
				InGameCamera.SetMode( EnemyMgr.CameraMode, EnemyMgr.CameraSideSetting );
			}
			else if( EnemyMgr.CameraMode == InGameCamera.EMode.FIXED ) {
				InGameCamera.SetMode( EnemyMgr.CameraMode, EnemyMgr.CameraFixedSetting );
			}
			else if( EnemyMgr.CameraMode == InGameCamera.EMode.FOLLOW_PLAYER ) {
				InGameCamera.SetMode( EnemyMgr.CameraMode, EnemyMgr.CameraFollowPlayerSetting );
			}
		}
		else {
			InGameCamera.SetDefaultMode();
		}

		if( !GameSupport.IsInGameTutorial() ) {
			GameUIManager.Instance.ShowUI( "GameStartPanel", true );
		}
		else {
			OnPlayerMissionStart();
		}

		Invoke( "ActiveEnemyMgr", Mathf.Clamp( mShowGamePlayUILength - 1.0f, 0.0f, mShowGamePlayUILength ) );
	}

	protected override void SendStageFailed() {
		ChangeToFinishBGM( false );

		if( Player.curHp > 0.0f ) {
			Player.SetDie( StageType != eSTAGETYPE.STAGE_RAID );
		}

		if( EnemyMgr ) {
			EnemyMgr.ClearEnvObjects();
		}

		GameUIManager.Instance.ShowUI( "GameFailPanel", false );

		TimeSpan ts = TimeSpan.FromSeconds(ProgressTime);

        if( StageType == eSTAGETYPE.STAGE_RAID ) {
            GameInfo.Instance.Send_ReqRaidStageEndFail( (int)ts.Ticks, OnNetGameFail );
        }
        else {
            if( !GameInfo.Instance.IsTowerStage ) {
                GameInfo.Instance.Send_ReqStageEndFail( (int)ts.Ticks, null ); //, OnNetGameFail); 임무 실패는 서버 응답 안받는걸로
                OnNetGameFail( 0, null );
            }
            else {
                if( !GameInfo.Instance.IsTowerStageTestPlay ) {
                    GameInfo.Instance.Send_PktInfoArenaTowerGameEndReq( (uint)ts.Ticks, false, OnNetGameFail );
                }
                else {
                    OnNetGameFail( 0, null );
                }
            }
        }
	}

	protected override void EndGame( eEventType eventType, Unit sender ) {
		if( eventType == eEventType.EVENT_GAME_PLAYER_DEAD ) {
			++mCurrentPlayerIndex;

			if( IsAllPlayersDead() ) {
				base.EndGame( eventType, sender );
			}
			else {
				ProcessingEnd = false;

				if( StageType == eSTAGETYPE.STAGE_TOWER ) {
					ChangePlayer();
				}
				else {
                    Player aliveAIPlayer = ListPlayer.Find( x => x != Player && x.curHp > 0.0f );

					if( sender == Player ) {
                        ChangePlayableCharacter( aliveAIPlayer );

                        UISubCharacterUnit subCharUnit = UIPlay.GetSubCharUnitByPlayerOrNull( aliveAIPlayer );
                        subCharUnit.Set( sender as Player );
                    }

                    /*
                    if( aliveAIPlayer ) {
                        UIPlay.SetSubCharUnit( 0, aliveAIPlayer );
                        UIPlay.SetSubCharUnit( 1, sender as Player );
                    }
                    else {
                        UIPlay.SetSubCharUnit( 0, sender as Player );
                    }
                    */

                    UIPlay.DisableDiedSubCharUnit();

                    HUDResMgr.Instance.HideAllDamageText( sender );
                    sender.DeactivateAllPlayerMinion();
                    sender.Deactivate();
                }

				EnemyMgr.AllEnemyRetarget();
			}
		}
		else {
			base.EndGame( eventType, sender );
		}
	}

	private bool InitStage() {
		if( StageData == null ) {
			return false;
		}

		if( !string.IsNullOrEmpty( StageData.ScenarioDrt_Start ) ) {
			string drtStartName = StageData.ScenarioDrt_Start;

			if( GameInfo.Instance.CharSelete ) {
				drtStartName += "_char_select";
			}

			AddScenarioDirector( null, "Start", drtStartName );
		}

		if( !string.IsNullOrEmpty( StageData.ScenarioDrt_BossAppear ) ) {
			AddScenarioDirector( null, "BossAppear", StageData.ScenarioDrt_BossAppear );
		}

		if( !string.IsNullOrEmpty( StageData.ScenarioDrt_BossDie ) ) {
			AddScenarioDirector( null, "BossDie", StageData.ScenarioDrt_BossDie );
		}

		if( !string.IsNullOrEmpty( StageData.ScenarioDrt_EndMission ) ) {
			AddScenarioDirector( null, "EndMission", StageData.ScenarioDrt_EndMission );
		}

		if( !string.IsNullOrEmpty( StageData.ScenarioDrt_AfterEndMission ) ) {
			AddScenarioDirector( null, "AfterEndMission", StageData.ScenarioDrt_AfterEndMission );
		}

		mbLastStorySequence = true;

		if( !string.IsNullOrEmpty( StageData.BGM ) ) {
			mBgm = LoadAudioClip( StageData.BGM );
		}

		if( !string.IsNullOrEmpty( StageData.AmbienceSound ) ) {
			mAmbienceSnd = LoadAudioClip( StageData.AmbienceSound );
		}

		string levelPath = Utility.AppendString("BackGround/StageLevel/", StageData.LevelPB, ".prefab");
		if( TestScene == null ) {
            if( StageType == eSTAGETYPE.STAGE_RAID ) {
                int selectedPBNumber = 0;

                GameClientTable.RaidInfo.Param raidInfoParam = GameInfo.Instance.GameClientTable.FindRaidInfo( GameInfo.Instance.ServerData.RaidCurrentSeason );
                if( raidInfoParam != null ) {
                    string[] split = Utility.Split( raidInfoParam.LevelPBList, ',' );

                    for( int i = 0; i < split.Length; i++ ) {
                        int n = Utility.SafeIntParse( split[i] );

                        if( n <= GameInfo.Instance.SelectedRaidLevel ) {
                            selectedPBNumber = n;
                        }
                    }
                }

                if( selectedPBNumber > 0 ) {
                    levelPath = Utility.AppendString( "BackGround/StageLevel/", StageData.LevelPB, "_", selectedPBNumber.ToString(), ".prefab" );
                }
			}

			EnemyMgr = ResourceMgr.Instance.CreateFromAssetBundle<BattleAreaManager>( "background", levelPath );
			if( EnemyMgr == null ) {
				return false;
			}

            //EnemyMgr.gameObject.SetActive( false );
            //EnemyMgr.Deactive();
        }

		ClearConditionValue = StageData.ConditionValue;
		mCheckConditionValue = 0;

		GameUIManager.Instance.HideUI( "BikeRaceModePanel", false );

		if( GameInfo.Instance.CharSelete ) {
            GameUIManager.Instance.HideUI( "GameOffPopup", false );

            GameSupport.SendFireBaseLogEvent( eFireBaseLogType._1_1_Production );
			PlayDirector( "Start", OnEndTutorialCharSelect );
		}
		else {
			InitPlayer( false );
			StartTimer();
		}

		return true;
	}

	private void AddScenarioDirector( Unit owner, string key, string drtName ) {
		Director director = null;

		string[] split = Utility.Split(drtName, ',');
		for( int i = 0; i < split.Length; ++i ) {
			director = GameSupport.CreateDirector( split[i] );
			AddDirector( owner, key, director );
		}
	}

	private bool InitTowerStage(int tableId)
    {
        TowerStageData = GameInfo.Instance.GameTable.FindArenaTower(tableId);
        if (TowerStageData == null)
        {
            Debug.LogError(tableId + "번 타워 스테이지는 없습니다.");
            return false;
        }

        mbLastStorySequence = true;

        if (!string.IsNullOrEmpty(TowerStageData.BGM))
        {
            mBgm = LoadAudioClip(TowerStageData.BGM);
        }

        string strModel = Utility.AppendString("BackGround/StageLevel/", TowerStageData.LevelPB, ".prefab");
        if (TestScene == null)
        {
            EnemyMgr = ResourceMgr.Instance.CreateFromAssetBundle<BattleAreaManager>("background", strModel);
            //EnemyMgr.gameObject.SetActive(false);
            EnemyMgr.Deactive();
        }

        ClearMode = (eClearMode)TowerStageData.ClearCondition;
        ClearConditionValue = TowerStageData.ConditionValue;
        mCheckConditionValue = 0;

        if (ClearMode != eClearMode.MonsterCount && TowerStageData.ConditionValue > 0)
        {
            IsTimeLimit = true;
            GameTime = TowerStageData.ConditionValue;
        }

        GameUIManager.Instance.HideUI("BikeRaceModePanel", false);

        InitPlayersInTower(false);
        StartTimer();

        return true;
    }

	private bool InitSecretStage()
	{
		if (StageData == null)
		{
			return false;
		}

		List<GameTable.SecretQuestLevel.Param> list = GameInfo.Instance.GameTable.FindAllSecretQuestLevel(x => x.GroupID == StageData.ID);
		for(int i = 0; i < list.Count; ++i)
		{
			if(list[i].No == SecretQuestLevelId)
			{
				SecretQuestData = list[i];
				break;
			}
		}

		if (!string.IsNullOrEmpty(SecretQuestData.BGM))
		{
			mBgm = LoadAudioClip(SecretQuestData.BGM);
		}

		if (!string.IsNullOrEmpty(SecretQuestData.AmbienceSound))
		{
			mAmbienceSnd = LoadAudioClip(SecretQuestData.AmbienceSound);
		}

		string strModel = Utility.AppendString("BackGround/StageLevel/", SecretQuestData.LevelPB, ".prefab");
		if (TestScene == null)
		{
			EnemyMgr = ResourceMgr.Instance.CreateFromAssetBundle<BattleAreaManager>("background", strModel);
			if (EnemyMgr == null)
			{
				return false;
			}

			//EnemyMgr.gameObject.SetActive(false);
		}

		ClearConditionValue = SecretQuestData.ConditionValue;
		mCheckConditionValue = 0;

		GameUIManager.Instance.HideUI("BikeRaceModePanel", false);

		if (GameInfo.Instance.CharSelete)
		{
			GameSupport.SendFireBaseLogEvent(eFireBaseLogType._1_1_Production);
			PlayDirector("Start", OnEndTutorialCharSelect);
		}
		else
		{
			InitPlayer(false);
			StartTimer();
		}

		//GameUIManager.Instance.HideUI("GameOffPopup", false);
		IsTimeLimit = false;

		ClearMode = (eClearMode)SecretQuestData.ClearCondition;
		if (ClearMode != eClearMode.MonsterCount && SecretQuestData.ConditionValue > 0)
		{
			IsTimeLimit = true;
		}

		if (SecretQuestData != null && IsTimeLimit)
		{
			GameTime = SecretQuestData.ConditionValue;
		}
		else
		{
			GameTime = 0.0f;
		}

		ProgressTime = 0.0f;
		return true;
	}

	public List<Player> ListTempPlayerInTower { get; protected set; }  = new List<Player>();
    private void InitPlayersInTower(bool isTutorial)
    {
        // 인풋 바인딩은 하나만 가능하니까 0번 캐릭터를 마지막에 초기화
        for( int i = GameInfo.Instance.ListAllyPlayerData.Count - 1; i >= 0; i--)
        {
            AllyPlayerData allyPlayerData = GameInfo.Instance.ListAllyPlayerData[i];
            if (allyPlayerData == null)
            {
                ListPlayer.Insert(0, null);
                continue;
            }

            CharData charData = null;
            bool isFriendData = false;

            if (allyPlayerData.MyCharData != null)
            {
                charData = allyPlayerData.MyCharData;
            }
            else if (allyPlayerData.FriendCharData != null)
            {
                charData = allyPlayerData.FriendCharData.CharData;
                isFriendData = true;
            }

            Player player = GameSupport.CreatePlayer( charData, isFriendData );
            player.CharIndexInTower = i;

            List<AwakenSkillInfo> listAwakenSkillData = null;

            if (isFriendData)
            {
                float addCardFormationHpRate = 0.0f;
                float addWeaponDepotAtkRate = 0.0f;

                addCardFormationHpRate = GameSupport.GetTotalCardFormationEffectValue() / 100.0f;
                addWeaponDepotAtkRate = GameSupport.GetTotalWeaponDepotEffectValue() / 100.0f;

                bool usePassiveList = false;
				if( TestScene || AppMgr.Instance.SceneType == AppMgr.eSceneType.None ) {
					usePassiveList = true;
				}

				player.FriendCharData = allyPlayerData.FriendCharData;
                player.SetAction(usePassiveList, allyPlayerData.FriendCharData.CardList);

                player.SetStatsForPVPOpponent(allyPlayerData.FriendCharData.CardList, allyPlayerData.FriendCharData.MainWeaponData, 
                                              allyPlayerData.FriendCharData.SubWeaponData, allyPlayerData.FriendCharData.MainGemList, 
                                              allyPlayerData.FriendCharData.SubGemList, i, addCardFormationHpRate, addWeaponDepotAtkRate);
                //player.Set2ndStatsForPVPOpponent(allyPlayerData.FriendCharData.MainGemList);
                player.costumeUnit.SetCostume(allyPlayerData.FriendCharData.CharData, allyPlayerData.FriendCharData.MainWeaponData, allyPlayerData.FriendCharData.SubWeaponData);
                player.GetBones();

                listAwakenSkillData = allyPlayerData.FriendCharData.ListAwakenSkillInfo;
            }
            else
            {
                listAwakenSkillData = GameInfo.Instance.UserData.ListAwakenSkillData;
            }

            UIPlay.SetPlayer(player, player.boSupporter == null ? null : player.boSupporter.data);

            if (GameInfo.Instance.SelectedCardFormationTID > 0)
            {
                GameTable.CardFormation.Param param = GameInfo.Instance.GameTable.FindCardFormation(GameInfo.Instance.SelectedCardFormationTID);
                if (param != null)
                {
                    player.AddBOCharBattleOptionSet(param.FormationBOSetID1, 1);
                    player.AddBOCharBattleOptionSet(param.FormationBOSetID2, 1);
                }
            }

            for (int j = 0; j < listAwakenSkillData.Count; j++)
            {
                AwakenSkillInfo info = listAwakenSkillData[j];
                if (info.Level <= 0)
                {
                    continue;
                }

                GameTable.AwakeSkill.Param param = GameInfo.Instance.GameTable.FindAwakeSkill(info.TableId);
                if (param == null)
                {
                    Debug.LogError(info.TableId + "번 배틀옵션셋이 없습니다.");
                    continue;
                }

                player.AddBOCharBattleOptionSet(param.SptAddBOSetID1, info.Level);
                player.AddBOCharBattleOptionSet(param.SptAddBOSetID2, info.Level);
            }

            ListPlayer.Insert(0, player);
        }

        ListTempPlayerInTower.Clear();
        ListTempPlayerInTower.AddRange(ListPlayer);

        SetBadgeStats(ListPlayer.ToArray(), GameSupport.GetEquipTowerBadgeList());

        for (int i = 0; i < ListPlayer.Count; i++)
        {
            if (ListPlayer[i] == null)
            {
                ListPlayer.RemoveAt(i);
                --i;
            }
        }

        mCurrentPlayerIndex = 0;
        Player = ListPlayer[0];

        if (Player != null)
        {
            Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.Enemy);
            Utility.SetPhysicsLayerCollision(eLayer.Player, eLayer.EnemyGate);

            InGameCamera.SetPlayer(Player);

            Vector3 startPos = Vector3.zero;
            Quaternion startRot = Quaternion.identity;
            EnemyMgr.GetStartPoint(out startPos, out startRot);

            for (int i = 0; i < ListPlayer.Count; i++)
            {
                ListPlayer[i].SetInitialRigidBodyPos(startPos, startRot);
                ListPlayer[i].AddDirector();
                ListPlayer[i].gameObject.SetActive(false);

                if (Player != ListPlayer[i])
                {
                    ListPlayer[i].ShowWeapon(false);
                }
            }

            InGameCamera.SetInitialDefaultMode();

            if (!isTutorial)
            {
                if (GameInfo.Instance.ContinueStage)
                {
                    int goldCount = PlayerPrefs.GetInt("SAVE_STAGE_PLAYER_GOLD_");
                    Player.SetGoldCount(goldCount);
                }

                GameUIManager.Instance.HideUI("GameOffPopup", false);

                if (GameInfo.Instance.ContinueStage)
                {
                    EndIntro();
                }
                else
                {
                    if (!Player.gameObject.activeSelf)
                    {
                        Player.gameObject.SetActive(true);
                    }

                    if (!Player.PlayDirector("Start", EndIntro))
                    {
                        EndIntro();
                    }
                }
            }
        }
    }

    public void SetBadgeStats(Player[] players, List<BadgeData> listBadge)
    {
        if (listBadge == null || listBadge.Count <= 0)
        {
            return;
        }

        List<ObscuredFloat> listPlayerMaxHp = new List<ObscuredFloat>();
        List<ObscuredFloat> listPlayerAttackPower = new List<ObscuredFloat>();
        List<ObscuredFloat> listPlayerDefenceRate = new List<ObscuredFloat>();
        List<ObscuredFloat> listPlayerCriticalRate = new List<ObscuredFloat>();

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                listPlayerMaxHp.Add(players[i].maxHp);
                listPlayerAttackPower.Add(players[i].attackPower);
                listPlayerDefenceRate.Add(players[i].defenceRate);
                listPlayerCriticalRate.Add(players[i].criticalRate);
            }
            else
            {
                listPlayerMaxHp.Add(0.0f);
                listPlayerAttackPower.Add(0.0f);
                listPlayerDefenceRate.Add(0.0f);
                listPlayerCriticalRate.Add(0.0f);
            }
        }

        // 메인 옵션 체크
        int mainOptID = 0;
        int mainOptEquipCnt = 0;

        if (listBadge[(int)eBadgeSlot.FIRST - 1] != null)
        {
            mainOptID = listBadge[(int)eBadgeSlot.FIRST - 1].OptID[(int)eBadgeOptSlot.FIRST];
            mainOptEquipCnt = GameSupport.GetMainOptEquipCnt(listBadge, mainOptID);
        }

        for (int i = 0; i < listBadge.Count; i++)
        {
            BadgeData data = listBadge[i];
            if (data == null)
            {
                continue;
            }

            for (int j = 0; j < data.OptID.Length; j++)
            {
                if (data.OptID[j] <= 0)
                {
                    continue;
                }

                GameTable.BadgeOpt.Param param = GameInfo.Instance.GameTable.FindBadgeOpt(x => x.OptionID == data.OptID[j]);
                if (param == null)
                {
                    continue;
                }

                float value = ((data.OptVal[j] + data.Level) * param.IncEffectValue) / (float)eCOUNT.MAX_RATE_VALUE;
                if (mainOptID == data.OptID[j])
                {
                    value += (value * GameInfo.Instance.GameConfig.BadgeSetAddRate[mainOptEquipCnt]);
                }

                string str = param.EffectType;
				string[] splits = Utility.Split(str, '_'); //str.Split('_');
                Unit2ndStatsTable.eBadgeOptType optType = Utility.GetEnumByString<Unit2ndStatsTable.eBadgeOptType>(splits[1]);
                if (optType == Unit2ndStatsTable.eBadgeOptType.CriResistUp) // 치명 저항 확률 증가
                {

                }
                else if (optType == Unit2ndStatsTable.eBadgeOptType.CriDefUp) // 치명 피해량 감소
                {

                }
                else if (optType == Unit2ndStatsTable.eBadgeOptType.StartCharUp) // 선봉 캐릭터 능력(HP, 공격력) 강화
                {
                    if (players[(int)eArenaTeamSlotPos.START_POS])
                    {
                        players[(int)eArenaTeamSlotPos.START_POS].AddMaxHp(listPlayerMaxHp[(int)eArenaTeamSlotPos.START_POS], value);
                        players[(int)eArenaTeamSlotPos.START_POS].AddAttackPower(listPlayerAttackPower[(int)eArenaTeamSlotPos.START_POS], value);
                    }
                }
                else if (optType == Unit2ndStatsTable.eBadgeOptType.MidCharUp) // 중견 캐릭터 능력(HP, 공격력) 강화
                {
                    if (players[(int)eArenaTeamSlotPos.MID_POS])
                    {
                        players[(int)eArenaTeamSlotPos.MID_POS].AddMaxHp(listPlayerMaxHp[(int)eArenaTeamSlotPos.MID_POS], value);
                        players[(int)eArenaTeamSlotPos.MID_POS].AddAttackPower(listPlayerAttackPower[(int)eArenaTeamSlotPos.MID_POS], value);
                    }
                }
                else if (optType == Unit2ndStatsTable.eBadgeOptType.LastCharUp) // 대장 캐릭터 능력(HP, 공격력) 강화
                {
                    if (players[(int)eArenaTeamSlotPos.LAST_POS])
                    {
                        players[(int)eArenaTeamSlotPos.LAST_POS].AddMaxHp(listPlayerMaxHp[(int)eArenaTeamSlotPos.LAST_POS], value);
                        players[(int)eArenaTeamSlotPos.LAST_POS].AddAttackPower(listPlayerAttackPower[(int)eArenaTeamSlotPos.LAST_POS], value);
                    }
                }
                else if (optType == Unit2ndStatsTable.eBadgeOptType.CharHPUp) // 전 캐릭터 체력 강화
                {
                    for (int k = 0; k < players.Length; k++)
                    {
                        if (players[k] == null)
                        {
                            continue;
                        }

                        players[k].AddMaxHp(listPlayerMaxHp[k], value);
                    }
                }
                else if (optType == Unit2ndStatsTable.eBadgeOptType.CharATKUp) // 전 캐릭터 공격력 강화
                {
                    for (int k = 0; k < players.Length; k++)
                    {
                        if (players[k] == null)
                        {
                            continue;
                        }

                        players[k].AddAttackPower(listPlayerAttackPower[k], value);
                    }
                }
                else if (optType == Unit2ndStatsTable.eBadgeOptType.CharDEFUp) // 전 캐릭터 방어력 강화
                {
                    for (int k = 0; k < players.Length; k++)
                    {
                        if (players[k] == null)
                        {
                            continue;
                        }

                        players[k].AddDefenceRate(listPlayerDefenceRate[k], value);
                    }
                }
                else if (optType == Unit2ndStatsTable.eBadgeOptType.CharCRIUp) // 전 캐릭터 치명확률 강화 
                {
                    for (int k = 0; k < players.Length; k++)
                    {
                        if (players[k] == null)
                        {
                            continue;
                        }

                        players[k].AddCriticalRate(listPlayerCriticalRate[k], value);
                    }
                }
            }
        }
    }

    private void OnEndTutorialCharSelect()
    {
        GameSupport.SendFireBaseLogEvent(eFireBaseLogType._1_1_Production_Complete);
        InitPlayer(true);
        Player.InitAfterEnemyMgrInit();

        StartTimer();
        StartPlayerIntro();
    }

    private void ActiveEnemyMgr()
    {
        if (GameSupport.IsInGameTutorial())
        {
            eTutorialState tutorialState = (eTutorialState)GameInfo.Instance.UserData.GetTutorialState();
            if (tutorialState == eTutorialState.TUTORIAL_STATE_Init)
            {
                GameSupport.SendFireBaseLogEvent(eFireBaseLogType._1_1_Start);
                if (AppMgr.Instance.TutorialBattleHelpFlag)
                {
                    UIValue.Instance.SetValue(UIValue.EParamType.ShowHelpPopupInPausePopup, false);
                    GameUIManager.Instance.ShowUI("GamePlayHelpPopup", true);
                    Pause(true);
                }
                else
                {
                    ShowTutorialHUD(1);
                }
                EnemyMgr.Active();
            }
            else if (tutorialState == eTutorialState.TUTORIAL_STATE_Stage2Clear)
            {
                GameSupport.SendFireBaseLogEvent(eFireBaseLogType._1_2_Start);

                //Player.SetHp(Player.maxHp * GameInfo.Instance.BattleConfig.DangerHpRatio);
                //UIPlay.SubPlayerHp(Player.curHp, Player.maxHp, 0.0f, 0.0f);

                //Player.Input.LockDirection(true);
                //Player.Input.LockBtnFlag(InputController.ELockBtnFlag.ATTACK | InputController.ELockBtnFlag.DASH | 
                //                         InputController.ELockBtnFlag.USKILL | InputController.ELockBtnFlag.WEAPON);

                //// 회복 아이템 하나 생성
                //GameClientTable.DropItem.Param param = GameInfo.Instance.GameClientTable.FindDropItem(11);
                //DropItem dropItem = ResourceMgr.Instance.CreateFromAssetBundle<DropItem>("item", param.ModelPb + ".prefab");
                //dropItem.Init(param);
                //dropItem.Drop(Player.transform.position + (transform.forward * 3.0f) + (Vector3.up * 0.5f));
                //dropItem.gameObject.SetActive(true);

                GameInfo.Instance.UserData.SetTutorial(GameInfo.Instance.UserData.GetTutorialState(), 2);
                ShowTutorialHUD(1);
                EnemyMgr.Active();
            }
            else if (tutorialState == eTutorialState.TUTORIAL_STATE_Stage3Clear)
            {
                GameSupport.SendFireBaseLogEvent(eFireBaseLogType._1_3_Start);
                GameUIManager.Instance.ShowUI("SkillTrainingPanel", false);
                ShowTutorialHUD(1);
            }
        }
        else
        {
            if(StageData != null)
            {
				if (StageType == eSTAGETYPE.STAGE_SECRET && SecretQuestData != null)
				{
					UIPlay.m_screenEffect.ShowHUD(FLocalizeString.Instance.GetText(SecretQuestData.MissionDesc), null, false,
												  GameInfo.Instance.GameConfig.MissionStartNoticeDuration);
				}
				else if (StageData.MissionDesc > 0)
				{
					UIPlay.m_screenEffect.ShowHUD(FLocalizeString.Instance.GetText(StageData.MissionDesc), null, false,
												  GameInfo.Instance.GameConfig.MissionStartNoticeDuration);
				}
            }
            else if(TowerStageData != null && TowerStageData.MissionDesc > 0)
            {
                UIPlay.m_screenEffect.ShowHUD(FLocalizeString.Instance.GetText(TowerStageData.MissionDesc), null, false,
                                              GameInfo.Instance.GameConfig.MissionStartNoticeDuration);
            }

            Pause(false, true, null, true);
            EnemyMgr.Active();
        }
    }

    private void OnEndMissionDirector()
    {
		//InGameCamera.EnableCamera(false);
		GameUIManager.Instance.ShowUI( "GameOffPopup", false );

		GameUIManager.Instance.HideUI("GameRewardPopup", false);

        if (!GameInfo.Instance.IsTowerStage && StageData.ScenarioID_EndMission > 0)
        {
            //GameUIManager.Instance.ShowUI("GameOffPopup", false);
            ShowScenario(StageData.ScenarioID_EndMission, OnEndScenario);
        }
        else
        {
            OnEndScenario();
        }
    }

    private void OnEndScenario()
    {
		GameUIManager.Instance.HideUI( "GameOffPopup", false );

		if (HasDirector("AfterEndMission"))
        {
			//InGameCamera.EnableCamera(true);
			//GameUIManager.Instance.HideUI("GameOffPopup", false);
            PlayDirector("AfterEndMission", OnAfterEndMissionDirector);
        }
        else
        {
			UIValue.Instance.SetValue(UIValue.EParamType.LoadingType, (int)UILoadingPopup.eLoadingType.StageToLobby);
            UIValue.Instance.SetValue(UIValue.EParamType.LoadingStage, -1);
            GameUIManager.Instance.ShowUI("LoadingPopup", false);
            AppMgr.Instance.LoadScene(AppMgr.eSceneType.Lobby, "Lobby");
        }
    }

    private void OnAfterEndMissionDirector()
    {
		UIValue.Instance.SetValue(UIValue.EParamType.LoadingType, (int)UILoadingPopup.eLoadingType.StageToLobby);
        UIValue.Instance.SetValue(UIValue.EParamType.LoadingStage, -1);
        GameUIManager.Instance.ShowUI("LoadingPopup", false);
        AppMgr.Instance.LoadScene(AppMgr.eSceneType.Lobby, "Lobby");
    }

    private void OnNetGameFail(int result, PktMsgType pktmsg)
    {
        EndTutorial();
        FSaveData.Instance.RemoveStageData();

        GameUIManager.Instance.HideUI("GamePlayPanel", false);
        Invoke("ToLobby", 5.0f);
    }

    private void EndTutorial()
    {
        GameInfo.Instance.CharSelete = false;
    }

	private void StartPlayerIntro() {
		if( GameInfo.Instance.ContinueStage || StageData.ShowPlayerStartDrt == 0 ) {
			EndIntro();
			return;
		}

		if( !Player.gameObject.activeSelf )
			Player.gameObject.SetActive( true );

        if ( GameInfo.Instance.CharSelete ) {
            GameUIManager.Instance.HideUI( "GameOffPopup", false );
        }

		if( !Player.PlayDirector( "Start", EndIntro ) ) {
			EndIntro();
		}
	}

	private void ChangePlayer()
    {
        Player beforePlayer = ListPlayer[mCurrentPlayerIndex - 1];
		//beforePlayer.HideAllDamageFont(); 220203
		HUDResMgr.Instance.HideAllDamageText(beforePlayer);
        beforePlayer.DeactivateAllPlayerMinion();
        beforePlayer.Deactivate();
        beforePlayer.OnEndGame();

        Player = ListPlayer[mCurrentPlayerIndex];
        Player.AddInputController();

        Player.Activate();
        Player.SetInitialPosition(beforePlayer.posOnGround, beforePlayer.transform.rotation);
        Player.ReInitPlayerUI(Player.curHp, beforePlayer.curSp);

        if ( Player.Guardian ) {
			Player.Guardian.SetInitialPosition( Player.transform.position - Player.transform.forward, Player.transform.rotation );
		}

        Player.ShowMesh(true);
        Player.OnGameStart();
        Player.OnMissionStart();

        UIPlay.SetWidgetsAlpha(1.0f);

        Player.CopyEnemyNavigators(beforePlayer);
        Player.CopyNextBattleAreaNavigator(beforePlayer);
        Player.CopyOtherObjectNavigator(beforePlayer);

        Player.ChangeInputController(beforePlayer);

        UIPlay.m_screenEffect.ClearSkillNameQueue();
        UIPlay.SetUIActive(true, false);
        UIPlay.OnEnable();
        
        InGameCamera.SetPlayer(Player);

        // 교체된 캐릭터가 나올 때 이펙트 하나 틀어줌
        EffectManager.Instance.Play(Player, 10006, EffectManager.eType.Common);

        // 교체된 캐릭터가 나올 때 전체 적에게 넉백 공격
        ActionEvent evtAction = new ActionEvent();
        evtAction.Set(eEventSubject.ActiveEnemies, eEventType.EVENT_ACTION_HIT_KNOCKBACK_ATTACK, Player, eActionCommand.None, 0, 0);
        EventMgr.Instance.SendEvent(evtAction);

        Player.TemporaryInvincible = true;
        Invoke("ReleaseInvinciblePlayer", GameInfo.Instance.BattleConfig.ChangePlayerInvincibleTime);
    }

    private void ReleaseInvinciblePlayer()
    {
        Player.TemporaryInvincible = false;
    }

	private float GetAnimationSync( Director clearDirector, AniEvent.sAniInfo aniInfo ) {
		float result = 0.0f;

		if ( clearDirector && aniInfo != null ) {
			foreach ( var output in clearDirector.playableDirector.playableAsset.outputs ) {
				AnimationTrack animationTrack = output.sourceObject as AnimationTrack;
				if ( animationTrack == null ) {
					continue;
				}

				TimelineClip timelineClip = animationTrack.GetClips().LastOrDefault();
				if ( timelineClip == null ) {
					continue;
				}

				if ( !aniInfo.clipName.Equals( timelineClip.animationClip.name ) ) {
					continue;
				}

				double clipIn = 0;
				if ( timelineClip.clipIn > 0 ) {
					clipIn = timelineClip.animationClip.length - timelineClip.clipIn;
				}

				result = (float)( ( timelineClip.duration - clipIn ) / timelineClip.animationClip.length * timelineClip.timeScale );
				while ( result > 1 ) {
					result -= 1;
				}

				break;
			}
		}

		return result;
	}

#if UNITY_EDITOR
	private void ChangePlayer( int index ) {
        if ( index == mCurrentPlayerIndex ) {
            return;
		}

        int beforePlayerIndex = mCurrentPlayerIndex;
        mCurrentPlayerIndex = index;

        // 이전 플레이어 정리
        Player beforePlayer = ListPlayer[beforePlayerIndex];
        HUDResMgr.Instance.HideAllDamageText( beforePlayer );
        beforePlayer.DeactivateAllPlayerMinion();
        beforePlayer.Deactivate();
        beforePlayer.OnEndGame();

        // 새 플레이어 세팅
        Player = ListPlayer[mCurrentPlayerIndex];
        Player.AddInputController();

        Player.Activate();
        Player.SetInitialPosition( beforePlayer.posOnGround, beforePlayer.transform.rotation );
        Player.ReInitPlayerUI( Player.curHp, beforePlayer.curSp );

        Player.ShowMesh( true );
        Player.OnGameStart();
        Player.OnMissionStart();

        UIPlay.SetWidgetsAlpha( 1.0f );

        Player.CopyEnemyNavigators( beforePlayer );
        Player.CopyNextBattleAreaNavigator( beforePlayer );
        Player.CopyOtherObjectNavigator( beforePlayer );

        Player.ChangeInputController( beforePlayer );

        UIPlay.m_screenEffect.ClearSkillNameQueue();
        UIPlay.SetUIActive( true, false );
        UIPlay.OnEnable();

        InGameCamera.SetPlayer( Player );

        // 교체된 캐릭터가 나올 때 이펙트 하나 틀어줌
        EffectManager.Instance.Play( Player, 10006, EffectManager.eType.Common );

        // 교체된 캐릭터가 나올 때 전체 적에게 넉백 공격
        ActionEvent evtAction = new ActionEvent();
        evtAction.Set( eEventSubject.ActiveEnemies, eEventType.EVENT_ACTION_HIT_KNOCKBACK_ATTACK, Player, eActionCommand.None, 0, 0 );
        EventMgr.Instance.SendEvent( evtAction );

        Player.TemporaryInvincible = true;
        Invoke( "ReleaseInvinciblePlayer", GameInfo.Instance.BattleConfig.ChangePlayerInvincibleTime );
    }
#endif
}
