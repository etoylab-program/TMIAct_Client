using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !DISABLESTEAMWORKS
using Steamworks;
#endif

public class GameUIManager : FManager
{
    //[Header("TestStageType")]
    //public eStageMode kTestStageType = eStageMode.Mission;
#if !DISABLESTEAMWORKS
    protected Callback<GameOverlayActivated_t> mGameOverlayActivated;
    private void OnGameOverlayActivated(GameOverlayActivated_t pCallback) {
        if (pCallback.m_bActive != 0) {
            OnFocus(false);
        }
        else {
            OnFocus(true);
        }
    }
#endif

    private static GameUIManager s_instance = null;
    public static GameUIManager Instance
    {
        get
        {
            return s_instance;
        }
    }

    private float mGcCheckTime = 0.0f;


    void Awake()
    {
        if (s_instance == null)
            s_instance = this;
        else
            Debug.LogError("GameUIManager Instance Error!");

        UIRoot uiroot = this.transform.parent.GetComponent<UIRoot>();
        if (uiroot != null)
        {
            if (AppMgr.Instance.WideScreen)
            {
                uiroot.fitWidth = true;
                uiroot.fitHeight = true;
            }
            else
            {
                uiroot.fitWidth = true;
                uiroot.fitHeight = false;
            }
        }

        List<UIGamePlayPanel> listPanel = new List<UIGamePlayPanel>();
        listPanel.AddRange(gameObject.GetComponentsInChildren<UIGamePlayPanel>(true));

        UIGamePlayPanel find = null;
        if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam || AppMgr.Instance.IsAndroidPC())
        {
            find = listPanel.Find(x => x.name.Contains("_Steam"));
        }
        else
        {
            find = listPanel.Find(x => !x.name.Contains("_Steam"));
        }

        for (int i = 0; i < listPanel.Count; i++)
        {
            if (find == listPanel[i])
            {
                listPanel[i].name = "GamePlayPanel";
                listPanel[i].m_screenEffect = gameObject.GetComponentInChildren<ScreenEffect>(true);
                listPanel[i].SetUIActive(true, false);
            }
            else
            {
                listPanel[i].name = "GamePlayPanel_NotUsed";
                listPanel[i].transform.SetParent(null);
            }
        }

#if !DISABLESTEAMWORKS
        //스팀 콜백 등록
        mGameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
#endif
    }

	public override void Start() {
		base.Start();

		if ( AppMgr.Instance.SceneType == AppMgr.eSceneType.None ) {
			OnPrepare();
		}
	}

	public override void OnPrepare()
    {
        base.OnPrepare();
        ShowUI( "GameOffPopup", false );
        
#if UNITY_EDITOR
        if( World.Instance as WorldTestScene ) {
            World.Instance.Init( 0, eSTAGETYPE.STAGE_TEST );
            return;
		}

        AppMgr.Instance.TutorialSkipFlag = true;
        AppMgr.Instance.TutorialBattleHelpFlag = false;
#else
        World.Instance.EnableDamage = true;
        World.Instance.InvinciblePlayer = false;
        World.Instance.UseHelper = false;
#endif

        GameInfo.Instance.SetCardFormationData(0);

        // 게임씬 바로 실행시킴
        if( AppMgr.Instance.SceneType == AppMgr.eSceneType.None ) { 
            FSaveData.Instance.AutoTargeting = true;
            FSaveData.Instance.AutoTargetingSkill = true;
            FSaveData.Instance.TurnCameraToTarget = World.Instance.TurnCameraToTarget;

            FSaveData.Instance.SaveLocalData();
            FSaveData.Instance.LoadLocalData();

            NetLocalSvr.Instance.Regist();

			if( GameInfo.Instance.CharList.Count <= 0 ) {
				for( int i = 0; i < GameInfo.Instance.GameTable.Characters.Count; i++ ) {
					NetLocalSvr.Instance.AddChar( GameInfo.Instance.GameTable.Characters[i].ID );
				}
			}

			CharData charData = SetTestPlayerOrNull( World.Instance.TestPlayerData );
            if( charData != null ) {
                GameInfo.Instance.SeleteCharUID = charData.CUID;
            }

			WorldTraining worldTraining = World.Instance as WorldTraining;
			if( worldTraining ) {
				AppMgr.Instance.SetSceneType( AppMgr.eSceneType.Training );
				World.Instance.Init( 0, eSTAGETYPE.STAGE_TRAINING );
			}
			else
            {
				WorldPVP worldPVP = World.Instance as WorldPVP;
				if( worldPVP ) {
					AppMgr.Instance.SetSceneType( AppMgr.eSceneType.Stage );

					worldPVP.AsTestScene = true;
					World.Instance.Init( 0, eSTAGETYPE.STAGE_PVP );
				}
				else
                {
					if( World.Instance.SpecialStageType == World.eSpecialStageType.Tower ) {
						GameInfo.Instance.ListAllyPlayerData.Clear();
						GameInfo.Instance.ListAllyPlayerData.Add( new AllyPlayerData( charData, null ) );

						if( World.Instance.TestPlayer2Data.TableId > 0 ) {
							charData = SetTestPlayerOrNull( World.Instance.TestPlayer2Data );
							GameInfo.Instance.ListAllyPlayerData.Add( new AllyPlayerData( charData, null ) );
						}

						if( World.Instance.TestPlayer3Data.TableId > 0 ) {
							charData = SetTestPlayerOrNull( World.Instance.TestPlayer3Data );
							GameInfo.Instance.ListAllyPlayerData.Add( new AllyPlayerData( charData, null ) );
						}

						GameInfo.Instance.SelecteStageTableId = World.Instance.TestSelectStageTableId;
						GameInfo.Instance.SetCardFormationData( World.Instance.CardFormationTableId );
						GameInfo.Instance.IsTowerStage = true;

						AppMgr.Instance.SetSceneType( AppMgr.eSceneType.Stage );
						World.Instance.Init( World.Instance.TestSelectStageTableId, eSTAGETYPE.STAGE_TOWER );
					}
					else if( World.Instance.SpecialStageType == World.eSpecialStageType.Secret ) {
						GameInfo.Instance.SelecteStageTableId = World.Instance.TestSelectStageTableId;
						GameInfo.Instance.IsTowerStage = false;

						AppMgr.Instance.SetSceneType( AppMgr.eSceneType.Stage );
						World.Instance.Init( World.Instance.TestSelectStageTableId, eSTAGETYPE.STAGE_SECRET );
					}
					else {
                        GameInfo.Instance.SelecteStageTableId = World.Instance.TestSelectStageTableId;
						GameInfo.Instance.IsTowerStage = false;

                        GameTable.Stage.Param stageParam = GameInfo.Instance.GameTable.FindStage( GameInfo.Instance.SelecteStageTableId );
                        if( stageParam != null && stageParam.StageType == (int)eSTAGETYPE.STAGE_RAID ) {
                            charData.RaidHpPercentage = 100.0f;

                            GameInfo.Instance.RaidUserData.CharUidList.Clear();
                            GameInfo.Instance.RaidUserData.CharUidList.Add( GameInfo.Instance.SeleteCharUID );

                            for( int i = 0; i < World.Instance.TestHelperDatas.Length; i++ ) {
                                CharData helperData = SetTestPlayerOrNull( World.Instance.TestHelperDatas[i] );
                                if( helperData != null ) {
                                    helperData.RaidHpPercentage = 100.0f;
                                    GameInfo.Instance.RaidUserData.CharUidList.Add( helperData.CUID );
                                }
                            }

                            GameInfo.Instance.SelectedRaidLevel = Mathf.Max( 1, World.Instance.TestRaidLevel );
                            GameInfo.Instance.ServerData.RaidCurrentSeason = World.Instance.TestRaidSeason;

                            GameInfo.Instance.SetCardFormationData( GameInfo.Instance.RaidUserData.CardFormationId );

                            FSaveData.Instance.RaidAutoUltimateSkill = true;
                        }

						AppMgr.Instance.SetSceneType( AppMgr.eSceneType.Stage );
						World.Instance.Init( World.Instance.TestSelectStageTableId, (eSTAGETYPE)stageParam.StageType );
					}
				}
			}
		}
		else if ( ( !AppMgr.Instance.configData._Tutorial && GameInfo.Instance.CharList.Count <= 0 ) ||  // 튜토리얼은 꺼놨는데 캐릭터가 없는 경우
                  GameSupport.IsInGameTutorial() ||                                                      // 인게임 튜토리얼일 때
                  AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby) {                                // 로비에서 진입 시

            World.Instance.EnableDamage = true;
            //World.Instance.InvinciblePlayer = false;
            World.Instance.UseHelper = false;

            AppMgr.Instance.SetSceneType( AppMgr.eSceneType.Stage );

			WorldTraining worldTraining = World.Instance as WorldTraining;
			if( worldTraining ) {
				AppMgr.Instance.SetSceneType( AppMgr.eSceneType.Training );
				World.Instance.Init( 0, eSTAGETYPE.STAGE_TRAINING );
			}
			else {
				WorldPVP worldPVP = World.Instance as WorldPVP;
				if( worldPVP ) {
					AppMgr.Instance.SetSceneType( AppMgr.eSceneType.Stage );

					worldPVP.AsTestScene = false;
					World.Instance.Init( 0, eSTAGETYPE.STAGE_PVP );
				}
				else {
					if( GameInfo.Instance.IsTowerStage ) {
						GameInfo.Instance.SetCardFormationData( GameInfo.Instance.UserData.ArenaTowerCardFormationID );
						GameInfo.Instance.ListAllyPlayerData.Clear();

						List<AllyPlayerData> list = GameSupport.GetAllyPlayerDataList();
						for( int i = 0; i < list.Count; i++ ) {
							if( list[i].IsEmpty() ) {
								GameInfo.Instance.ListAllyPlayerData.Add( null );
								continue;
							}

							GameInfo.Instance.ListAllyPlayerData.Add( list[i] );
						}

						World.Instance.Init( GameInfo.Instance.SelecteStageTableId, eSTAGETYPE.STAGE_TOWER );
					}
					else {
                        GameTable.Stage.Param param = GameInfo.Instance.GameTable.FindStage( GameInfo.Instance.SelecteStageTableId );

                        if( param.StageType == (int)eSTAGETYPE.STAGE_RAID ) {
                            GameInfo.Instance.SetCardFormationData( GameInfo.Instance.RaidUserData.CardFormationId );
                            World.Instance.Init( GameInfo.Instance.SelecteStageTableId, eSTAGETYPE.STAGE_RAID );
                        }
                        else {
                            GameInfo.Instance.SetCardFormationData( GameInfo.Instance.UserData.CardFormationID );

                            if( param.StageType == (int)eSTAGETYPE.STAGE_SECRET ) {
                                World.Instance.SecretQuestLevelId = GameInfo.Instance.SelecteSecretQuestLevelId;
                                World.Instance.SecretQuestStageBOSetId = GameInfo.Instance.SelecteSecretQuestBOSetId;

                                World.Instance.Init( GameInfo.Instance.SelecteStageTableId, eSTAGETYPE.STAGE_SECRET );
                            }
                            else {
                                World.Instance.Init( GameInfo.Instance.SelecteStageTableId );
                            }
                        }
					}
				}
			}
        }

#if UNITY_EDITOR
        FSaveData.Instance.SkipDirector = GameInfo.Instance.GameConfig.TestSkipDirector;
#else
        FSaveData.Instance.SkipDirector = false;
#endif
    }

    public override void Update() {
        base.Update();

        if ( AppMgr.Instance.GcCollectInGame && World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
			if ( AppMgr.Instance.IsAppPause ) {
				mGcCheckTime = 0.0f;
			}
			else {
				mGcCheckTime += Time.deltaTime;
				if ( mGcCheckTime >= 60.0f ) {
					Resources.UnloadUnusedAssets();
					System.GC.Collect();

					mGcCheckTime = 0.0f;
				}
			}
        }

#if UNITY_EDITOR
        if ( Input.GetKeyDown( KeyCode.G ) ) {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
#endif
    }

    private List<FComponent> mListTempUI = new List<FComponent>();
    public override void OnEscape()
    {
        if(GetActiveUI("WaitPopup") != null || GetActiveUI("LoadingPopup") != null)
        {
            return;
        }

        UIFirstCharSelectPopup popup = GetActiveUI<UIFirstCharSelectPopup>("FirstCharSelectPopup");
        if (popup != null)
        {
            UICharViewerPopup viewer = GetActiveUI<UICharViewerPopup>("CharViewerPopup");
            if (viewer != null)
            {
                viewer.OnClickClose();
                return;
            }

            FComponent popupMsg = GetActiveUI("MessagePopup");
            if (popupMsg)
            {
                popupMsg.OnClickClose();
            }
            else if(!popup.IsSelect)
            {
                popup.OnClick_CancleBtn();
            }
            
            return;
        }
        else
        {
			if ( GetActiveUI( "TutorialPopup" ) != null || GetActiveUI( "BikeModeResultPopup" ) != null || Director.IsPlaying ) {
				return;
			}

			mListTempUI.Clear();

            FComponent[] comps = GetComponentsInChildren<FComponent>();
            for(int i = 0; i < comps.Length; i++)
            {
                FComponent comp = comps[i];
                if (comp == null || comp.Type != FComponent.TYPE.Popup || comp.name.Equals("GoodsPopup") || comp.name.Equals("CinematicPopup"))
                {
                    continue;
                }

                mListTempUI.Add(comp);
            }

            if(mListTempUI.Count <= 0)
            {
                if(World.Instance.StageType == eSTAGETYPE.STAGE_SPECIAL)
                {
                    UISpecialModePanel panelSpecial = GetUI<UISpecialModePanel>();
                    if(panelSpecial)
                    {
                        if (!World.Instance.IsPause)
                        {
                            panelSpecial.OnBtnPause();
                        }
                        else
                        {
                            panelSpecial.OnClickClose();
                        }
                    }
                }
                else if (World.Instance.StageType != eSTAGETYPE.STAGE_PVP)
                {
                    World.Instance.UIPlay.OnBtnPause();
                }

                return;
            }

            mListTempUI.Sort(delegate (FComponent lhs, FComponent rhs)
            {
                int lhsDepth = lhs.GetPanelDepth();
                int rhsDepth = rhs.GetPanelDepth();

                if (lhsDepth < rhsDepth)
                {
                    return 1;
                }
                else if (lhsDepth > rhsDepth)
                {
                    return -1;
                }

                return 0;
            });

            UIGameStoryPausePopup popupPause = mListTempUI[0].GetComponent<UIGameStoryPausePopup>();
            if (popupPause)
            {
                popupPause.OnClick_RestartBtn();
            }
            else
            {
                if (mListTempUI[0].IsPlayAnimtion())
                {
                    return;
                }

                UIStroyPopup popupStory = mListTempUI[0].GetComponent<UIStroyPopup>();
                if (popupStory)
                {
                    UIGameOffPopup popupOff = GetActiveUI<UIGameOffPopup>("GameOffPopup");
                    if (!popupOff)
                    {
                        popupStory.OnClick_ExitBtn();
                    }
                }
                else
                {
                    mListTempUI[0].OnClickClose();
                }
            }
        }
    }

    private CharData SetTestPlayerOrNull(long testPlayerUid, int testPlayerEquipCardTableId, int testPlayerEquipCardLevel, int testPlayerEquipCardWake,
                                         int testPlayerEquipWeapon1TableId, int testPlayerEquipWeapon1Wake, 
                                         int testPlayerEquipWeapon2TableId, int testPlayerEquipWeapon2Wake,
                                         int[] testPlayerSkill, int testPlayerCostumeTableId)
    {
        if(testPlayerUid <= 0)
        {
            return null;
        }

        CharData charData = GameInfo.Instance.GetCharData(testPlayerUid);
        if(charData == null)
        {
            return null;
        }

        if (testPlayerEquipCardTableId > 0)
        {
            long itemUID = NetLocalSvr.Instance.AddCard(testPlayerEquipCardTableId);
            if (itemUID != -1)
            {
                charData.EquipCard[(int)eCARDSLOT.SLOT_MAIN] = itemUID;

                CardData cardData = GameInfo.Instance.GetCardData(itemUID);
                cardData.Level = testPlayerEquipCardLevel;
                cardData.Wake = testPlayerEquipCardWake;
            }
        }

        if (testPlayerEquipWeapon1TableId > 0)
        {
            long itemUID = NetLocalSvr.Instance.AddWeapon(testPlayerEquipWeapon1TableId + (charData.TableID * 1000));
            if (itemUID != -1)
            {
                charData.EquipWeaponUID = itemUID;
                WeaponData weaponData = GameInfo.Instance.GetWeaponData(itemUID);
                weaponData.Wake = testPlayerEquipWeapon1Wake;
            }
        }

        if (testPlayerEquipWeapon2TableId > 0)
        {
            long itemUID = NetLocalSvr.Instance.AddWeapon(testPlayerEquipWeapon2TableId + (charData.TableID * 1000));
            if (itemUID != -1)
            {
                charData.EquipWeapon2UID = itemUID;
                WeaponData weaponData = GameInfo.Instance.GetWeaponData(itemUID);
                weaponData.Wake = testPlayerEquipWeapon2Wake;
            }
        }

        if (testPlayerSkill != null && testPlayerSkill.Length > 0)
        {
            charData.PassvieList.Clear();

            for (int i = 0; i < testPlayerSkill.Length; i++)
            {
                charData.PassvieList.Add(new PassiveData(testPlayerSkill[i], 1));
            }
        }

        if (testPlayerCostumeTableId > 0)
        {
            if (GameInfo.Instance.HasCostume(testPlayerCostumeTableId + (charData.TableID * 1000)))
            {
                charData.EquipCostumeID = testPlayerCostumeTableId + (charData.TableID * 1000);
            }
            else
            {
                int itemTableID = NetLocalSvr.Instance.AddCostume(testPlayerCostumeTableId + (charData.TableID * 1000));
                if (itemTableID != -1)
                {
                    charData.EquipCostumeID = itemTableID;
                }
            }
        }

        return charData;
    }

    private CharData SetTestPlayerOrNull( sTestPlayerData testPlayerData ) {
        if( testPlayerData.TableId == 0 ) {
            return null;
		}

        CharData charData = GameInfo.Instance.GetCharDataByTableID( testPlayerData.TableId );
        if( charData == null ) {
            NetLocalSvr.Instance.AddChar( testPlayerData.TableId );
            charData = GameInfo.Instance.GetCharDataByTableID( testPlayerData.TableId );
        }

        charData.Grade = Mathf.Max( 1, testPlayerData.Grade );
        charData.Level = Mathf.Max( testPlayerData.Level );

        Utility.AddTestPlayerSkill( charData, testPlayerData.SkillTableIds, testPlayerData.Skill2ndStatsLevel );
        Utility.AddTestPlayerSupporter( charData, testPlayerData.SupporterDatas );
        Utility.AddTestPlayerWeapon( charData, testPlayerData.WeaponDatas );
        Utility.AddTestPlayerCostume( charData, testPlayerData.CostumeData );

        return charData;
    }

#if !UNITY_EDITOR
    public void OnApplicationFocus(bool hasFocus)
    {
        OnFocus(hasFocus);
    }
#endif

    private void OnFocus(bool hasFocus) {
        //Debug.LogError($"<color=#FF9900>OnFocus {hasFocus}</color>");
#if !UNITY_EDITOR
        if (hasFocus || FSaveData.Instance.RunInBackground)// || GameInfo.Instance.CharList.Count <= 0)
        {
            return;
        }

        if (World.Instance.UIPlay)
        {
            if (World.Instance.IsEndGame)
            {
                return;
            }

            Debug.Log("OnApplicationFocus " + hasFocus);

            if (Director.CurrentPlaying)
            {
                if (!Director.CurrentPlaying.isPause)
                {
                    Director.CurrentPlaying.Pause();

                    UICinematicPopup cinematicPopup = GetUI<UICinematicPopup>("CinematicPopup");
                    if (cinematicPopup)
                    {
                        if (!cinematicPopup.gameObject.activeSelf)
                        {
                            ShowUI("CinematicPopup", false);
                        }

                        cinematicPopup.ShowPausePopup();
                    }
                }
            }
            else
            {
                if (World.Instance.StageType == eSTAGETYPE.STAGE_SPECIAL)
                {
                    UISpecialModePanel panel = GetUI<UISpecialModePanel>();
                    if(panel)
                    {
                        panel.OnBtnPause();
                    }
                }
                else if (World.Instance.StageType != eSTAGETYPE.STAGE_PVP)
                {
                    World.Instance.UIPlay.OnBtnPause();
                }
            }
        }
#endif
    }
}
