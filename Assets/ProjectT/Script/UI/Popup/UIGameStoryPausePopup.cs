using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIGameStoryPausePopup : FComponent
{
    public UILabel lbChapter;
    public UILabel lbTitle;

    public GameObject kChallengeObj;
    public List<UIChallengeUnit> kMissions;

    public FTab TabKeyMappingType;
    
    //Skills
    public List<UISkillUnit> kSkillObjs;
    private bool mbConnectGamePad = false;
    private int mKeyMappingType = 0;

    public UIButton kEnemyInfoBtn;

    public UIButton BtnTutorialSkip;
    public FToggle  ToggleRunInBackground;

    [Header("[Tower]")]
    public GameObject               AllyInfo;
    public List<UIUserCharListSlot> ListAllyCharInfo;
    public UILabel                  LbSupporterUnitName;

    [Header("[Raid]")]
    [SerializeField] private UILabel    _RaidTitleLabel;
    [SerializeField] private FList      _RaidRandomOptionList;

    public delegate void OnVoidCallBack();
    public OnVoidCallBack onVoidCallBack;

    private List<GameClientTable.StageBOSet.Param>mRaidRandomOptionList = new List<GameClientTable.StageBOSet.Param>();


    public override void Awake()
	{
		base.Awake();

        TabKeyMappingType.EventCallBack = OnTabKeyMappingType;
        ToggleRunInBackground.EventCallBack = OnToggleRunInBackground;

        _RaidRandomOptionList.EventUpdate = UpdateRandomOptionListSlot;
        _RaidRandomOptionList.EventGetItemCount = GetRandomOptionListCount;
    }
 
	public override void OnEnable()
	{
        //Invoke("WorldPause", GetOpenAniTime());
        World.Instance.Pause(true);
        base.OnEnable();

        if(!PlayerPrefs.HasKey("KeyMappingType"))
        {
            PlayerPrefs.SetInt("KeyMappingType", 0);
            mKeyMappingType = 0;
        }
        else
        {
            mKeyMappingType = PlayerPrefs.GetInt("KeyMappingType");
        }

        TabKeyMappingType.SetTab(mKeyMappingType, SelectEvent.Code);

        InitComponent();

        TabKeyMappingType.gameObject.SetActive(false);
        AppMgr.Instance.CustomInput.ShowCursor(true);
    }

    void WorldPause()
    {
        World.Instance.Pause(true);
    }
 
	public override void InitComponent()
	{
        for (int i = 0; i < kMissions.Count; i++)
            kMissions[i].gameObject.SetActive(false);

        TabKeyMappingType.gameObject.SetActive(false);
        if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam || AppMgr.Instance.IsAndroidPC())
        {
            TabKeyMappingType.gameObject.SetActive(true);

            mbConnectGamePad = false;
            string[] gamePadNames = Input.GetJoystickNames();
            for(int i = 0; i < gamePadNames.Length; i++)
            {
                if(!string.IsNullOrEmpty(gamePadNames[i]))
                {
                    mbConnectGamePad = true;
                    break;
                }
            }

            TabKeyMappingType.kBtnList[1].SetActive(mbConnectGamePad);
        }

#if !UNITY_EDITOR
        if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam)
#endif
        {
            ToggleRunInBackground.gameObject.SetActive(true);
            ToggleRunInBackground.SetToggle(FSaveData.Instance.RunInBackground ? 0 : 1, SelectEvent.Code);
        }
#if !UNITY_EDITOR
        else
        {
            ToggleRunInBackground.gameObject.SetActive(false);
        }
#endif

        Renewal(true);
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        BtnTutorialSkip.gameObject.SetActive(false);

        if (GameSupport.IsTutorial())
        { 
            if(AppMgr.Instance.TutorialSkipFlag)
                BtnTutorialSkip.gameObject.SetActive(true);
        }
        

        kEnemyInfoBtn.gameObject.SetActive(false);
		/*
        if(World.Instance.StageData != null)
        {
            var enemyInfoData = GameInfo.Instance.GameClientTable.FindHelpEnemyInfo(x => x.StageID == World.Instance.StageData.ID && x.StageType == World.Instance.StageData.StageType);
            if (enemyInfoData != null)
                kEnemyInfoBtn.gameObject.SetActive(true);
        }
		*/
        kChallengeObj.SetActive(true);

        SetText();
        SetMission();
        SetSkillSlot();

        AllyInfo.SetActive(false);
        _RaidTitleLabel.SetActive(false);

        _RaidRandomOptionList.gameObject.SetActive( false );

        if (World.Instance.StageType == eSTAGETYPE.STAGE_TOWER)
        {
            SetAllyCharInfo();
        }

        if (World.Instance.StageType == eSTAGETYPE.STAGE_RAID || World.Instance.StageType == eSTAGETYPE.STAGE_TEST)
        {
            SetRaidInfo();
        }
    }

	private void SetText() {
		if( World.Instance.StageData != null ) {
			lbChapter.textlocalize = FLocalizeString.Instance.GetText( World.Instance.StageData.Desc );
			lbTitle.textlocalize = FLocalizeString.Instance.GetText( World.Instance.StageData.Name );

            if( World.Instance.StageType == eSTAGETYPE.STAGE_RAID ) {
                lbChapter.textlocalize = FLocalizeString.Instance.GetText( World.Instance.StageData.Name );
                lbTitle.textlocalize = string.Format( FLocalizeString.Instance.GetText( 1821 ), GameInfo.Instance.SelectedRaidLevel );

                if( GameInfo.Instance.CardFormationTableData != null ) {
                    LbSupporterUnitName.textlocalize = FLocalizeString.Instance.GetText( GameInfo.Instance.CardFormationTableData.Name );
                }
                else {
                    LbSupporterUnitName.textlocalize = FLocalizeString.Instance.GetText( 1617 );
                }
            }
        }
		else {
			if( World.Instance.StageType == eSTAGETYPE.STAGE_TRAINING ) {
				lbChapter.textlocalize = FLocalizeString.Instance.GetText( 1396 );
				lbTitle.textlocalize = FLocalizeString.Instance.GetText( 1395 );
			}
			else if( World.Instance.StageType == eSTAGETYPE.STAGE_TOWER ) {
				WorldStage worldStage  = World.Instance as WorldStage;
				lbChapter.textlocalize = FLocalizeString.Instance.GetText( worldStage.TowerStageData.Desc );
				lbTitle.textlocalize = FLocalizeString.Instance.GetText( worldStage.TowerStageData.Name );

				if( GameInfo.Instance.CardFormationTableData != null ) {
					LbSupporterUnitName.textlocalize = FLocalizeString.Instance.GetText( GameInfo.Instance.CardFormationTableData.Name );
				}
				else {
					LbSupporterUnitName.textlocalize = FLocalizeString.Instance.GetText( 1617 );
				}
			}
			else {
				lbChapter.textlocalize = "";
				lbTitle.textlocalize = "Test Scene";
			}
		}
	}

	private void SetMission() {
		if( World.Instance.StageData != null && World.Instance.StageType != eSTAGETYPE.STAGE_RAID ) {
			RenewalChallengeUnit( 0, World.Instance.StageData.Mission_00, World.Instance.CheckStageMission( 0, World.Instance.StageData.Mission_00 ) );
			RenewalChallengeUnit( 1, World.Instance.StageData.Mission_01, World.Instance.CheckStageMission( 1, World.Instance.StageData.Mission_01 ) );
			RenewalChallengeUnit( 2, World.Instance.StageData.Mission_02, World.Instance.CheckStageMission( 2, World.Instance.StageData.Mission_02 ) );
		}
	}

	private void SetSkillSlot()
    {
        for (int i = 0; i < kSkillObjs.Count; i++)
        {
            kSkillObjs[i].gameObject.SetActive(false);
        }

        if (World.Instance.Player == null || World.Instance.Player.charData == null)
        {
            return;
        }

        if( World.Instance.StageType == eSTAGETYPE.STAGE_RAID || ( World.Instance.StageType == eSTAGETYPE.STAGE_TEST && World.Instance.ListPlayer.Count > 1 ) ) {
            return;
        }

        Log.Show(World.Instance.Player.charData.EquipSkill.Length);
        for(int i = 0; i < World.Instance.Player.charData.EquipSkill.Length; i++)
        {
            if (World.Instance.Player.charData.EquipSkill[i] == 0)
            {
                continue;
            }

            GameTable.CharacterSkillPassive.Param skillInfo = GameInfo.Instance.GameTable.FindCharacterSkillPassive(x => x.CharacterID == World.Instance.Player.charData.TableID && x.ID == World.Instance.Player.charData.EquipSkill[i]);
            if (skillInfo == null) continue;

            if (skillInfo.Slot == -1)
            {
                continue;
            }

            Log.Show(World.Instance.Player.charData.EquipSkill[i], Log.ColorType.Red);

            kSkillObjs[i].gameObject.SetActive(true);
            kSkillObjs[i].UpdateSlot(skillInfo);
        }
    }

    private void RenewalChallengeUnit(int index, int missionid, World.EMissionClearStatus clearStatus)
    {
        GameClientTable.StageMission.Param missiondata = GameInfo.Instance.GameClientTable.StageMissions.Find(x => x.ID == missionid);
        if (missiondata == null)
        {
            kMissions[index].gameObject.SetActive(false);
            return;
        }

        kMissions[index].gameObject.SetActive(true);
        kMissions[index].InitChallengeUnit(missiondata, clearStatus != World.EMissionClearStatus.FAIL ? 1 : 0);
    }

    private void SetAllyCharInfo()
    {
        kChallengeObj.SetActive(false);
        AllyInfo.SetActive(true);

        List<AllyPlayerData> list = GameSupport.GetAllyPlayerDataList();
        for (int i = 0; i < ListAllyCharInfo.Count; i++)
        {
            if (i >= list.Count)
            {
                break;
            }

            if (ListAllyCharInfo[i] == null) continue;
            if (list[i] == null) continue;

            ListAllyCharInfo[i].CharSelectFlag = eCharSelectFlag.ARENATOWER_STAGE;
            ListAllyCharInfo[i].UpdateSlot(list[i].GetCharDataOrNull());
        }
    }

    private void SetRaidInfo()
    {
        kChallengeObj.SetActive( false );
        AllyInfo.SetActive( true );

        for( int i = 0; i < ListAllyCharInfo.Count; i++ ) {
            if( i >= World.Instance.ListPlayer.Count ) {
                ListAllyCharInfo[i].SetActive( false );
                break;
            }

            if( ListAllyCharInfo[i] == null || World.Instance.ListPlayer[i] == null ) {
                ListAllyCharInfo[i].SetActive( false );
                continue;
            }

            ListAllyCharInfo[i].SetActive( true );
            ListAllyCharInfo[i].CharSelectFlag = eCharSelectFlag.RAID;
            ListAllyCharInfo[i].UpdateThreePlayerSlot( World.Instance.ListPlayer[i] );
        }

        WorldStage worldStage = World.Instance as WorldStage;
        if( worldStage ) {
            _RaidRandomOptionList.gameObject.SetActive( true );

            mRaidRandomOptionList.Clear();
            mRaidRandomOptionList.AddRange( worldStage.RaidStageBOSetList );
            _RaidRandomOptionList.UpdateList();
        }
    }

    public override void OnClickClose()
    {
        SetUIActive(false, false);
        AppMgr.Instance.CustomInput.ShowCursor(false);
    }

    public void OnBtnHelp()
    {
        UIValue.Instance.SetValue(UIValue.EParamType.ShowHelpPopupInPausePopup, true);
        GameUIManager.Instance.ShowUI("GamePlayHelpPopup", true);
    }

    public void OnClick_RestartBtn()
	{
        World.Instance.Pause(false);
        OnClickClose();
    }

	public void OnClick_GiveupBtn() {
		if( GameSupport.IsInGameTutorial() ) {
			MessagePopup.OK( eTEXTID.OK, 3127, null, false, false );
		}
		else {
			if( World.Instance.StageType == eSTAGETYPE.STAGE_TOWER ) {
				MessagePopup.OKCANCEL( eTEXTID.OK, 3123, SendTowerEnd, null, false );
			}
			else if( World.Instance.StageType == eSTAGETYPE.STAGE_RAID ) {
				MessagePopup.OKCANCEL( eTEXTID.OK, 3123, SendGiveUpRaid, null, false );
			}
			else {
				MessagePopup.OKCANCEL( eTEXTID.OK, 3123, GoToLobby, null, false );
			}
		}
	}

	public void OnBtnSettings()
	{
		GameUIManager.Instance.ShowUI("OptionPopup", false);
	}

    void GoToLobby()
    {
        // 미구입 캐릭터로 트레이닝 모드 돌렸을 때 임시로 추가한 CharData 제거
        if (UIValue.Instance.ContainsKey(UIValue.EParamType.TemporaryCharTraining))
        {
            object value = UIValue.Instance.GetValue(UIValue.EParamType.TemporaryCharTraining, true);

            if (value != null)
            {
                long temporaryUid = (long)value;
                CharData charData = GameInfo.Instance.GetCharData(temporaryUid);

                NetLocalSvr.Instance.RemoveChar(temporaryUid);
            }
        }

        FSaveData.Instance.RemoveStageData();
        World.Instance.Pause(true, false);

        GameUIManager.Instance.HideUI("GameFailPanel", false);
        GameUIManager.Instance.HideUI("GamePlayPanel", false);

        OnClickClose();

        UIValue.Instance.SetValue(UIValue.EParamType.LoadingType, (int)UILoadingPopup.eLoadingType.StageToLobby);
        UIValue.Instance.SetValue(UIValue.EParamType.LoadingStage, -1);
        GameUIManager.Instance.ShowUI("LoadingPopup", false);
        AppMgr.Instance.LoadScene(AppMgr.eSceneType.Lobby, "Lobby");
    }

    private void SendTowerEnd()
    {
        System.TimeSpan ts = System.TimeSpan.FromSeconds(World.Instance.ProgressTime);
        GameInfo.Instance.Send_PktInfoArenaTowerGameEndReq((uint)ts.TotalMilliseconds, false, null);

        GoToLobby();
    }

    private void SendGiveUpRaid() {
        World.Instance.Pause( true, false );
        GameInfo.Instance.Send_ReqRaidStageGiveUp( OnNetRaidGiveUpStage );
	}

    private void OnNetRaidGiveUpStage( int result, PktMsgType pkt ) {
        if( result != 0 ) {
            return;
		}

        GoToLobby();
	}

    public void OnClick_HelpBtn()
    {
        UIValue.Instance.SetValue(UIValue.EParamType.ShowHelpPopupInPausePopup, true);
        GameUIManager.Instance.ShowUI("GamePlayHelpPopup", false);
    }

    public void OnClick_EnemyInfoBtn()
    {
        GameUIManager.Instance.ShowUI("EnemyInfoPopup", false);
    }

    public void OnClick_BuffInfoBtn()
    {
        GameUIManager.Instance.ShowUI("GameBuffDebuffinfoPopup", false);
    }

    private bool OnTabKeyMappingType(int nSelect, SelectEvent type)
    {
        if(nSelect == 0)
        {
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.Attack, true);
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.Dash, true);
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.WeaponSkill, true);
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.SupporterSkill, true);
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.USkill, true);
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.ChangeWeapon, true);
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.Pause, true);
        }
        else
        {
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.Attack, false);
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.Dash, false);
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.WeaponSkill, false);
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.SupporterSkill, false);
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.USkill, false);
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.ChangeWeapon, false);
            AppMgr.Instance.CustomInput.ChangeShowKeyMapping(BaseCustomInput.eKeyKind.Pause, false);
        }

        PlayerPrefs.SetInt("KeyMappingType", nSelect);
        mKeyMappingType = nSelect;

        onVoidCallBack?.Invoke();

        return true;
    }

    public void OnBtnTutorialSkip()
    {
        MessagePopup.OKCANCEL(eTEXTID.OK, 3220, StartTutorialSkip, null, false);
    }

    private void StartTutorialSkip()
    {
        OnClick_RestartBtn();
        GameInfo.Instance.Send_ReqSetTutorialVal((int)eTutorialState.TUTORIAL_STATE_EndTutorial, 1, OnSkipTutorial);
    }

    public void OnSkipTutorial(int result, PktMsgType pktmsg)
    {
        if(result != 0)
        {
            return;
        }
        GameInfo.Instance.TutorialSkipFlag = true;
        GoToLobby();
    }

    private bool OnToggleRunInBackground(int nSelect, SelectEvent type)
    {
        if(type == SelectEvent.Enable)
        {
            return false;
        }

        FSaveData.Instance.SaveRunInBackground(nSelect == 0 ? true : false);
        return true;
    }

    private void UpdateRandomOptionListSlot( int index, GameObject slotObject ) {
        UIRaidRandomOptionSlot slot = slotObject.GetComponent<UIRaidRandomOptionSlot>();
        slot.ParentGO = gameObject;

        slot.UpdateSlot( index, mRaidRandomOptionList[index] );
    }

    private int GetRandomOptionListCount() {
        return mRaidRandomOptionList.Count;
    }
}
