
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIOptionPopup : FComponent
{
    enum eToggleType
    {
        ON = 0,
        OFF = 1,
    }


	[System.Serializable]
	public struct sMainTabItem
	{
		public FTab			Tab;
		public GameObject[]	Pages;
	}

	[System.Serializable]
	public struct sShowPadSettingInfo
	{
		public GameObject	Img;
		public UILabel		LbName;


		public void Show(bool show)
		{
			Img.SetActive(show);
			LbName.gameObject.SetActive(show);
		}
	}


	[Header("[Common]")]
	public sMainTabItem						MainTab_Mobile;
	public sMainTabItem						MainTab_PC;
	public List<GameObject>					kMobileList;
	public List<GameObject>					kPCList;
	[Space(10)]
	public GameObject						kTestObj;

	[Header("[Graphics & Sounds]			")]
	public UIGaugeBtnUnit					kSoundMasterUnit;
	public UIGaugeBtnUnit					kSoundBGMUnit;
	public UIGaugeBtnUnit					kSoundSEUnit;
	public UIGaugeBtnUnit					kSoundVoiceUnit;
	public FTab								kMobileGraphicTab;
	public FTab								kMobileFrameTab;
	public FTab								kPCGraphicTab;
	public FTab								kPCFrameTab;
	public FLocalizeText					kPCResolutionLabel;
	public FToggle							kPCFullScreenToggle;
	public GameObject						PCFullScreenToggleDimmed;
	public FDropDown						kPCResolutionPopupList;
	public GameObject						kPCResolutionPopupDimmed;
	public GameObject						kPCScreenDropDown;

	[Header("[Game & Camera]")]
	public FToggle							kAutoTargetingToggle;
	public FToggle							ToggleAutoTurnCamera;
	public UIGaugeBtnUnit					GaugeBtnCameraSensitivity;
	public FToggle							kPCCameraUpDownToggle;
	public FToggle							kPCCameraRightLeftToggle;
	public UIGaugeBtnUnit					kPCCameraSenseUnit;

	[Header("[Scenario]")]
	public FTab								kVLTextTab;
	public FTab								kVLAutoTab;
	public FLabelTextShow					kVLTestLabel;
	public GameObject						VLTextDimmed;
	[Space(5)]

	[Header("[Notice]")]
    public FToggle							kPushAllToggle;
    public FToggle							kPushEventToggle;
    public FToggle							kPushAPToggle;
    public FToggle							kPushNightToggle;

	[Header("[Control]")]
	public GameObject						KeySettings;
	public UIButton							BtnShowGamePadSet;
	public GameObject						GamePadSet;
	public UIButton							BtnShowKeySettings;
	public FList							FListKeySetup;
	public sShowPadSettingInfo[]			ShowPadSettingInfos;

	private sMainTabItem					mCurrentTabITem;
	private List<Resolution>				mListPCScreenResolution			= null;
    private EventDelegate					mDeleOnSetPCScreenResolution	= null;
    private float							mMesssageTimerPopupTime			= 10f;
	private List<BaseCustomInput.eKeyKind>	mListKeys						= new List<BaseCustomInput.eKeyKind>();
	private int								mCurShowPadSettingIndex			= 0;


	public override void Awake()
    {
		MainTab_Mobile.Tab.EventCallBack = OnMainTabSelect;
		MainTab_PC.Tab.EventCallBack = OnMainTabSelect;

		// Graphics & Sounds
		kMobileGraphicTab.EventCallBack = OnGraphicTabSelect;
		kMobileFrameTab.EventCallBack = OnFrameTabSelect;
		kPCGraphicTab.EventCallBack = OnGraphicTabSelect;
        kPCFrameTab.EventCallBack = OnFrameTabSelect;
		kPCFullScreenToggle.EventCallBack = OnPCFullScreenToggleSelect;
		mDeleOnSetPCScreenResolution = new EventDelegate(this, "OnSetPCScreenResolution");

		// Game & Camera
		kAutoTargetingToggle.EventCallBack = OnAutoTargetingToggleSelect;
		ToggleAutoTurnCamera.EventCallBack = OnAutoTurnCameraToggleSelect;
		kPCCameraUpDownToggle.EventCallBack = OnPCCameraUpDownToggleSelect;
		kPCCameraRightLeftToggle.EventCallBack = OnPCCameraRightLeftToggleSelect;

		// Scenario
		kVLTextTab.EventCallBack = OnVLTextTabSelect;
		kVLAutoTab.EventCallBack = OnVLAutoTabSelect;

		// Notice
		kPushAllToggle.EventCallBack = OnPushAllToggleSelect;
        kPushEventToggle.EventCallBack = OnPushEventToggleSelect;
        kPushAPToggle.EventCallBack = OnPushAPToggleSelect;
        kPushNightToggle.EventCallBack = OnPushNightToggleSelect;

		// Control
		FListKeySetup.EventUpdate = UpdateKeySetupListSlot;
		FListKeySetup.EventGetItemCount = GetKeySetupElementCount;

		base.Awake();
    }

    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		base.OnDisable();
        EventDelegate.Remove(kPCResolutionPopupList.onChange, mDeleOnSetPCScreenResolution);
    }

    public override void InitComponent()
    {
        if( AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam || AppMgr.Instance.IsAndroidPC())
        {
			mCurrentTabITem = MainTab_PC;

			for (int i = 0; i < kPCList.Count; i++)
			{
				kPCList[i].SetActive(true);
			}

			for (int i = 0; i < kMobileList.Count; i++)
			{
				kMobileList[i].SetActive(false);
			}
        }
        else
        {
			mCurrentTabITem = MainTab_Mobile;

			for (int i = 0; i < kPCList.Count; i++)
			{
				kPCList[i].SetActive(false);
			}

			for (int i = 0; i < kMobileList.Count; i++)
			{
				kMobileList[i].SetActive(true);
			}
        }

		kTestObj.gameObject.SetActive(AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby && !AppMgr.Instance.configData.m_Network);

		// Graphics & Sounds
        FSaveData.Instance.Graphic = !PlayerPrefs.HasKey("FSaveData_Graphic") ? 2 : PlayerPrefs.GetInt("FSaveData_Graphic");

		kMobileGraphicTab.SetTab(FSaveData.Instance.Graphic, SelectEvent.Code);
		kMobileFrameTab.SetTab(FSaveData.Instance.Frame, SelectEvent.Code);
		kPCGraphicTab.SetTab(FSaveData.Instance.Graphic, SelectEvent.Code);
        kPCFrameTab.SetTab(FSaveData.Instance.Frame, SelectEvent.Code);
        
        kSoundMasterUnit.InitUIGaugeBtnUnit(FLocalizeString.Instance.GetText(1187), FSaveData.Instance.SoundMaster.ToString(), FSaveData.Instance.SoundMaster, 1, true);
        kSoundBGMUnit.InitUIGaugeBtnUnit(FLocalizeString.Instance.GetText(1188), FSaveData.Instance.SoundBG.ToString(), FSaveData.Instance.SoundBG, 2, true);
        kSoundSEUnit.InitUIGaugeBtnUnit(FLocalizeString.Instance.GetText(1189), FSaveData.Instance.SoundSE.ToString(), FSaveData.Instance.SoundSE, 3, true);
        kSoundVoiceUnit.InitUIGaugeBtnUnit(FLocalizeString.Instance.GetText(1190), FSaveData.Instance.SoundVoice.ToString(), FSaveData.Instance.SoundVoice, 4, true);

		if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam)
		{
			// 풀 스크린 상태
			kPCFullScreenToggle.SetToggle(PlayerPrefs.GetInt("Screen.fullScreen"), SelectEvent.Code);

			bool pcResolutionPopupDimmed = false;
			if((PlayerPrefs.GetInt("Screen.fullScreen") == 1) ||
			   ((AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage || AppMgr.Instance.SceneType == AppMgr.eSceneType.Training) && World.Instance.IsPause))
			{
				pcResolutionPopupDimmed = true;
			}

			kPCResolutionPopupDimmed.SetActive(pcResolutionPopupDimmed);

			if (PCFullScreenToggleDimmed)
			{
				PCFullScreenToggleDimmed.SetActive(false);
				if ((AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage || AppMgr.Instance.SceneType == AppMgr.eSceneType.Training) && 
					World.Instance.IsPause)
				{
					PCFullScreenToggleDimmed.SetActive(true);
				}
			}

			// 해상도 지원
			if (mListPCScreenResolution == null)
			{
				mListPCScreenResolution = new List<Resolution>();
			}

			mListPCScreenResolution.Clear();
			kPCResolutionPopupList.Clear();

			// 카메라 반전 상태
			kPCCameraUpDownToggle.SetToggle(PlayerPrefs.GetInt("Camera.UpDownToggle"), SelectEvent.Click);
			kPCCameraRightLeftToggle.SetToggle(PlayerPrefs.GetInt("Camera.RightLeftToggle"), SelectEvent.Click);

			Action<int, int> AddResolution = (x, y) =>
			{
			#if UNITY_STANDALONE_WIN
                bool bAdd = false;
                for (int i = 0; i < Screen.resolutions.Length; i++)
                {
                    Resolution tempR = Screen.resolutions[i];
                    if (tempR.width == x && tempR.height == y)
                    {
                        bAdd = true;
                        break;
                    }
                }
                if (!bAdd)
                    return;
			#endif

				Resolution r = new Resolution();
				r.width = x;
				r.height = y;
				r.refreshRate = 30;
				mListPCScreenResolution.Add(r);
				kPCResolutionPopupList.AddItem(string.Format("{0}x{1}", r.width, r.height), r);
			};

			AddResolution(800, 450);
			AddResolution(1024, 576);
			AddResolution(1200, 675);
			AddResolution(1280, 720);
			AddResolution(1600, 900);
			AddResolution(1920, 1080);
			AddResolution(2048, 1152);
			AddResolution(2560, 1440);
			AddResolution(3440, 1935);
			AddResolution(3840, 2160);
			AddResolution(4096, 2340);

			if (!PlayerPrefs.HasKey("Screen.width") || !PlayerPrefs.HasKey("Screen.height"))
			{
				//해상도 설정이 처음이다.
				if (mListPCScreenResolution.Count == 0)
				{
					kPCResolutionPopupList.Set("Not Supported");
				}
				else
				{
					var r = mListPCScreenResolution[mListPCScreenResolution.Count - 1];
					kPCResolutionPopupList.Set(string.Format("{0}x{1}", r.width, r.height));
				}
			}
			else
			{
				kPCResolutionPopupList.Set(string.Format("{0}x{1}", PlayerPrefs.GetInt("Screen.width"), PlayerPrefs.GetInt("Screen.height")));
			}

			EventDelegate.Add(kPCResolutionPopupList.onChange, mDeleOnSetPCScreenResolution);
		}

		// Game & Camera
		kAutoTargetingToggle.SetToggle(FSaveData.Instance.AutoTargeting ? (int)eToggleType.ON : (int)eToggleType.OFF, SelectEvent.Code);
		ToggleAutoTurnCamera.SetToggle(FSaveData.Instance.TurnCameraToTarget ? (int)eToggleType.ON : (int)eToggleType.OFF, SelectEvent.Code);
		kPCCameraSenseUnit.InitUIGaugeBtnUnit(FLocalizeString.Instance.GetText(1172), FSaveData.Instance.Sensitivity.ToString(), FSaveData.Instance.Sensitivity, 5, true);
		GaugeBtnCameraSensitivity.InitUIGaugeBtnUnit(FLocalizeString.Instance.GetText(1780), FSaveData.Instance.CameraSensitivity.ToString(), FSaveData.Instance.CameraSensitivity, 6, true);

		if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam)
		{
			ToggleAutoTurnCamera.transform.localPosition = new Vector3(ToggleAutoTurnCamera.transform.localPosition.x, -74, ToggleAutoTurnCamera.transform.localPosition.z);
		}
		else
		{
			ToggleAutoTurnCamera.transform.localPosition = new Vector3(ToggleAutoTurnCamera.transform.localPosition.x, 27, ToggleAutoTurnCamera.transform.localPosition.z);
		}

		// Scenario
		kVLTextTab.SetTab(FSaveData.Instance.VLText, SelectEvent.Code);
		kVLAutoTab.SetTab(FSaveData.Instance.VLAuto, SelectEvent.Code);

		if (VLTextDimmed)
		{
			VLTextDimmed.SetActive(false);
			if ((AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage || AppMgr.Instance.SceneType == AppMgr.eSceneType.Training) &&
				World.Instance.IsPause)
			{
				VLTextDimmed.SetActive(true);
			}
		}

		// Notice
		kPushAllToggle.SetToggle(FSaveData.Instance.bPushAll ? (int)eToggleType.ON : (int)eToggleType.OFF, SelectEvent.Code);
		kPushEventToggle.SetToggle(FSaveData.Instance.bPushEvent ? (int)eToggleType.ON : (int)eToggleType.OFF, SelectEvent.Code);
		kPushAPToggle.SetToggle(FSaveData.Instance.bPushAP ? (int)eToggleType.ON : (int)eToggleType.OFF, SelectEvent.Code);
		kPushNightToggle.SetToggle(FSaveData.Instance.bPushNigth ? (int)eToggleType.ON : (int)eToggleType.OFF, SelectEvent.Code);

		// Control
		mListKeys.Clear();
		for (BaseCustomInput.eKeyKind k = BaseCustomInput.eKeyKind.None + 1; k < BaseCustomInput.eKeyKind.PCCount; k++)
		{
			mListKeys.Add(k);
		}

		FListKeySetup.UpdateList();

		InitShowPadSetting();
		ShowCurrentPadSetting(true);
	}

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);
    }

	public override void OnClickClose()
	{
		// Graphics & Sounds
		if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam)
		{
			FSaveData.Instance.Graphic = kPCGraphicTab.kSelectTab;
			FSaveData.Instance.Frame = kPCFrameTab.kSelectTab;
		}
		else
		{
			FSaveData.Instance.Graphic = kMobileGraphicTab.kSelectTab;
			FSaveData.Instance.Frame = kMobileFrameTab.kSelectTab;
		}

		FSaveData.Instance.SoundMaster = kSoundMasterUnit.Value;
		FSaveData.Instance.SoundBG = kSoundBGMUnit.Value;
		FSaveData.Instance.SoundSE = kSoundSEUnit.Value;
		FSaveData.Instance.SoundVoice = kSoundVoiceUnit.Value;

		// Game & Camera
		FSaveData.Instance.AutoTargeting = kAutoTargetingToggle.kSelect == 0;
		FSaveData.Instance.AutoTargetingSkill = kAutoTargetingToggle.kSelect == 0;
		FSaveData.Instance.TurnCameraToTarget = ToggleAutoTurnCamera.kSelect == 0;
		FSaveData.Instance.Sensitivity = kPCCameraSenseUnit.Value;

		// Scenario
		FSaveData.Instance.VLText = kVLTextTab.kSelectTab;
		FSaveData.Instance.VLAuto = kVLAutoTab.kSelectTab;

		// Notice
		FSaveData.Instance.bPushAll = kPushAllToggle.kSelect == 0;
		FSaveData.Instance.bPushEvent = kPushEventToggle.kSelect == 0;
		FSaveData.Instance.bPushAP = kPushAPToggle.kSelect == 0;
		FSaveData.Instance.bPushNigth = kPushNightToggle.kSelect == 0;

	#if !UNITY_EDITOR
        //FCM Push설정 - AP회복은 앱 끌때 설정.
        if (FSaveData.Instance.bPushAll)
        {
            LocalPushNotificationManager.Instance.SetFCMSubscribe();
        }
        else
        {
            LocalPushNotificationManager.Instance.UnSubscribeLanguage();
            LocalPushNotificationManager.Instance.UnSubscribeNightPush();
            LocalPushNotificationManager.Instance.UnSubscribeGlobalPush();
        }
	#endif

		// Apply
		FSaveData.Instance.SaveLocalData();
		AppMgr.Instance.SetQualityLevel();

		if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby && Lobby.Instance.LobbyPlayer)
		{
			if (FSaveData.Instance.Graphic >= 2)
			{
				Lobby.Instance.LobbyPlayer.ShowShadow(true);
			}
			else
			{
				Lobby.Instance.LobbyPlayer.ShowShadow(false);
			}
		}

		SoundManager.Instance.SetBgmVolme(FSaveData.Instance.GetBGVolume());
		SoundManager.Instance.SetFxVolume(FSaveData.Instance.GetSEVolume());
		SoundManager.Instance.SetVoiceVolume(FSaveData.Instance.GetVoiceVolume());

		AppMgr.Instance.CustomInput.SetSensitivity(FSaveData.Instance.GetSensitivity());

		SetUIActive(false, AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby);
	}

	public void RefreshKeySetting()
	{
		AppMgr.Instance.CustomInput.SavePCKeySettings();
		FListKeySetup.RefreshNotMove();
	}

	public void OnClickGauge(int type)
	{
		int SoundMaster = kSoundMasterUnit.Value;
		int SoundBG = kSoundBGMUnit.Value;
		int SoundSE = kSoundSEUnit.Value;
		int SoundVoice = kSoundVoiceUnit.Value;
		int Sensitivity = kPCCameraSenseUnit.Value;

		if (type == 1 || type == 2)
		{
			float volume = ((float)SoundMaster * (float)SoundBG) * 0.01f;
			SoundManager.Instance.SetBgmVolme(volume);
		}
		else if (type == 3)
		{
			float volume = ((float)SoundMaster * (float)SoundSE) * 0.01f;
			SoundManager.Instance.PlaySnd(SoundManager.eSoundType.FX, 25, volume);
		}
		else if (type == 4)
		{
			float volume = ((float)SoundMaster * (float)SoundVoice) * 0.01f;
			SoundManager.Instance.PlayVoice(26, volume);
		}
		else if (type == 5)
		{
			float s = EasingFunction.Linear(1f, 15f, Sensitivity * 0.1f);
			AppMgr.Instance.CustomInput.SetSensitivity(s);
		}
		else if(type == 6)
		{
			FSaveData.Instance.CameraSensitivity = GaugeBtnCameraSensitivity.Value;
		}

		if (type == 1 || type == 3)
		{
			float volume = ((float)SoundMaster * (float)SoundSE) * 0.01f;
			SoundManager.Instance.SetSeVolumeChange(volume);
		}
	}

	public void OnClick_BackBtn()
	{
		OnClickClose();
	}

	public void OnClick_InitSet()
    {
        FSaveData.Instance.Init();
        FSaveData.Instance.SaveLocalData();

        PlayerPrefs.SetInt("Camera.UpDownToggle", 1);       // 0 : On , 1 : Off
        PlayerPrefs.SetInt("Camera.RightLeftToggle", 1);    // 0 : On , 1 : Off

		OnBtnResetKeySettings();

		InitComponent();
        Renewal(true);
    }

	private bool OnMainTabSelect(int nSelect, SelectEvent type)
	{
		if (type == SelectEvent.Enable)
		{
			return true;
		}

		for (int i = 0; i < mCurrentTabITem.Pages.Length; i++)
		{
			mCurrentTabITem.Pages[i].SetActive(i == nSelect);
		}

		return true;
	}

	private bool OnGraphicTabSelect(int nSelect, SelectEvent type)
	{
		if (type == SelectEvent.Enable)
		{
			return true;
		}

		FSaveData.Instance.Graphic = nSelect;
		return true;
	}

	private bool OnFrameTabSelect(int nSelect, SelectEvent type)
	{
		if (type == SelectEvent.Enable)
		{
			return true;
		}

		FSaveData.Instance.Frame = nSelect;
		return true;
	}

	public void OnClick_PCResolutionBtn()
    {
    }

	private bool OnPCFullScreenToggleSelect(int nSelect, SelectEvent type)
	{
		if (type != SelectEvent.Click)
			return true;

	#if !DISABLESTEAMWORKS
        // 1 : On , 0 : Off
        if (nSelect == 1)
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
            kPCResolutionPopupDimmed.SetActive(true);
        }
        else if (nSelect == 0)
        {
            Screen.SetResolution(PlayerPrefs.GetInt("Screen.width"), PlayerPrefs.GetInt("Screen.height"), FullScreenMode.Windowed);
            kPCResolutionPopupDimmed.SetActive(false);

        }
        PlayerPrefs.SetInt("Screen.fullScreen", nSelect);

        MessageTimerPopup.YN(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3196), null, () =>
        {
            //CANCEL
            if (PlayerPrefs.GetInt("Screen.fullScreen") == 0)
            {
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
                kPCFullScreenToggle.SetToggle(1, SelectEvent.Code);
                PlayerPrefs.SetInt("Screen.fullScreen", 1);
            }
            else
            {
                Screen.SetResolution(PlayerPrefs.GetInt("Screen.width"), PlayerPrefs.GetInt("Screen.height"), FullScreenMode.Windowed);
                kPCFullScreenToggle.SetToggle(0, SelectEvent.Code);
                PlayerPrefs.SetInt("Screen.fullScreen", 0);
            }
            Utility.RefreshUIAnchor();
        }, mMesssageTimerPopupTime);
	#endif

	#if UNITY_STANDALONE
        Utility.RefreshUIAnchor();
	#endif
		return true;
	}

	private bool OnAutoTargetingToggleSelect(int nSelect, SelectEvent type)
	{
		return true;
	}

	private bool OnAutoTurnCameraToggleSelect(int nSelect, SelectEvent type)
	{
		return true;
	}

	private bool OnPCCameraUpDownToggleSelect(int nSelect, SelectEvent type)
	{
		if (type != SelectEvent.Click)
		{
			return true;
		}

		// 0 : On , 1 : Off
		AppMgr.Instance.CustomInput.InverseCameraYAxis = nSelect == 0;

		PlayerPrefs.SetInt("Camera.UpDownToggle", nSelect);
		return true;
	}

	private bool OnPCCameraRightLeftToggleSelect(int nSelect, SelectEvent type)
	{
		if (type != SelectEvent.Click)
		{
			return true;
		}

		// 0 : On , 1 : Off
		AppMgr.Instance.CustomInput.InverseCameraXAxis = nSelect == 0;

		PlayerPrefs.SetInt("Camera.RightLeftToggle", nSelect);
		return true;
	}

	public void OnBtnShowGamePadSet()
	{
		GamePadSet.SetActive(true);
		KeySettings.SetActive(false);
	}

	public void OnBtnShowKeySettings()
	{
		KeySettings.SetActive(true);
		GamePadSet.SetActive(false);
	}

	public void OnBtnPadSettingLeft()
	{
		if(mCurShowPadSettingIndex > 0)
		{
			ShowCurrentPadSetting(false);
			--mCurShowPadSettingIndex;
		}
		else
		{
			ShowCurrentPadSetting(false);
			++mCurShowPadSettingIndex;
		}

		ShowCurrentPadSetting(true);
	}

	public void OnBtnPadSettingRight()
	{
		if (mCurShowPadSettingIndex < ShowPadSettingInfos.Length - 1)
		{
			ShowCurrentPadSetting(false);
			++mCurShowPadSettingIndex;
		}
		else
		{
			ShowCurrentPadSetting(false);
			--mCurShowPadSettingIndex;
		}

		ShowCurrentPadSetting(true);
	}

	public void OnBtnResetKeySettings()
	{
		AppMgr.Instance.CustomInput.InitPCKeyMapping();
		RefreshKeySetting();
	}

	/*
	public void OnClick_PCKeyMouseBtn()
	{
		LobbyUIManager.Instance.ShowUI("KeySetupPopup", true);
	}

	public void OnClick_PCGamePadBtn()
	{
		LobbyUIManager.Instance.ShowUI("GamePadPopup", true);
	}
	*/

	private void OnSetPCScreenResolution()
	{
		if (PlayerPrefs.GetInt("Screen.fullScreen") == 1 || MessageTimerPopup.GetMessagePopup().gameObject.activeSelf || UIPopupList.current.data == null)
		{
			return;
		}

		if(AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage || AppMgr.Instance.SceneType == AppMgr.eSceneType.Training)
		{
			return;
		}

		Resolution r = (Resolution)UIPopupList.current.data;
		//Debug.Log("Selection: " + string.Format("{0}x{1}", r.width, r.height, r.refreshRate));
		Screen.SetResolution(r.width, r.height, FullScreenMode.Windowed);
		Utility.RefreshUIAnchor();

		MessageTimerPopup.YN(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3196),
			() =>
			{
				//Yes
				PlayerPrefs.SetInt("Screen.width", r.width);
				PlayerPrefs.SetInt("Screen.height", r.height);
			},
			() =>
			{
				//No
				Screen.SetResolution(PlayerPrefs.GetInt("Screen.width"), PlayerPrefs.GetInt("Screen.height"), FullScreenMode.Windowed);
				kPCResolutionPopupList.Set(string.Format("{0}x{1}", PlayerPrefs.GetInt("Screen.width"), PlayerPrefs.GetInt("Screen.height")));
				Utility.RefreshUIAnchor();
			},
			mMesssageTimerPopupTime);
	}

	private bool OnVLTextTabSelect(int nSelect, SelectEvent type)
	{
		if (type == SelectEvent.Code)
		{
			return true;
		}

		FSaveData.Instance.VLText = nSelect;
		kVLTestLabel.EndScroll();

		if (nSelect.Equals(0))
		{
			kVLTestLabel.kCharSpeed = 0.15f;
			kVLTestLabel.SetText(FLocalizeString.Instance.GetText(1367), true);
		}
		else if (nSelect.Equals(1))
		{
			kVLTestLabel.kCharSpeed = 0.05f;
			kVLTestLabel.SetText(FLocalizeString.Instance.GetText(1367), true);
		}
		else if (nSelect.Equals(2))
		{
			kVLTestLabel.kCharSpeed = 0.01f;
			kVLTestLabel.SetText(FLocalizeString.Instance.GetText(1367), true);
		}
		else if (nSelect.Equals(3))
		{
			kVLTestLabel.SetText(FLocalizeString.Instance.GetText(1367), false);
		}

		return true;
	}

	private bool OnVLAutoTabSelect(int nSelect, SelectEvent type)
	{
		if (type == SelectEvent.Code)
		{
			return true;
		}

		FSaveData.Instance.VLAuto = nSelect;
		return true;
	}

	public bool OnPushAllToggleSelect(int nSelect, SelectEvent type)
	{
		if (type == SelectEvent.Click)
		{
			if (nSelect == (int)eToggleType.ON)
			{
				kPushAPToggle.SetToggle((int)eToggleType.ON, SelectEvent.Code);
				kPushEventToggle.SetToggle((int)eToggleType.ON, SelectEvent.Code);
			}
		}

		if (nSelect == (int)eToggleType.OFF)
		{
			kPushEventToggle.SetToggle((int)eToggleType.OFF, SelectEvent.Code);
			kPushAPToggle.SetToggle((int)eToggleType.OFF, SelectEvent.Code);
			kPushNightToggle.SetToggle((int)eToggleType.OFF, SelectEvent.Code);
		}

		return true;
	}

	public bool OnPushEventToggleSelect(int nSelect, SelectEvent type)
	{
		if (nSelect == (int)eToggleType.ON)
		{
			kPushAllToggle.SetToggle((int)eToggleType.ON, SelectEvent.Code);
		}
		else
		{
			if (type == SelectEvent.Click)
			{
				if (kPushAPToggle.kSelect == (int)eToggleType.OFF && kPushNightToggle.kSelect == (int)eToggleType.OFF)
				{
					kPushAllToggle.SetToggle((int)eToggleType.OFF, SelectEvent.Code);
				}
			}
		}

		return true;
	}

	public bool OnPushAPToggleSelect(int nSelect, SelectEvent type)
	{
		if (nSelect == (int)eToggleType.ON)
		{
			kPushAllToggle.SetToggle((int)eToggleType.ON, SelectEvent.Code);
		}
		else
		{
			if (type == SelectEvent.Click)
			{
				if (kPushEventToggle.kSelect == (int)eToggleType.OFF && kPushNightToggle.kSelect == (int)eToggleType.OFF)
				{
					kPushAllToggle.SetToggle((int)eToggleType.OFF, SelectEvent.Code);
				}
			}
		}

		return true;
	}

	public bool OnPushNightToggleSelect(int nSelect, SelectEvent type)
	{
		if (nSelect == (int)eToggleType.ON)
		{
			kPushAllToggle.SetToggle((int)eToggleType.ON, SelectEvent.Code);
		}
		else
		{
			if (type == SelectEvent.Click)
			{
				if (kPushEventToggle.kSelect == (int)eToggleType.OFF && kPushAPToggle.kSelect == (int)eToggleType.OFF)
					kPushAllToggle.SetToggle((int)eToggleType.OFF, SelectEvent.Code);
			}
		}
		return true;
	}

	private void UpdateKeySetupListSlot(int index, GameObject slotObject)
	{
		UIKeySetupListSlot slot = slotObject.GetComponent<UIKeySetupListSlot>();
		slot.ParentGO = gameObject;
		slot.SetData(mListKeys[index]);

		slot.UpdateSlot();
	}

	private int GetKeySetupElementCount()
	{
		return mListKeys.Count;
	}

	private void InitShowPadSetting()
	{
		for(int i = 0; i < ShowPadSettingInfos.Length; i++)
		{
			ShowPadSettingInfos[i].Show(false);
		}
	}

	private void ShowCurrentPadSetting(bool show)
	{
		ShowPadSettingInfos[mCurShowPadSettingIndex].Show(show);
	}

	//------------------------------------------------------------------------------------------------------------------------------------
	//
	// 테스트모드 기능
	//
	//------------------------------------------------------------------------------------------------------------------------------------
	public void OnClick_Kor()
    {
        ChangeLanguage(eLANGUAGE.KOR);
    }

    public void OnClick_Jpn()
    {
        ChangeLanguage(eLANGUAGE.JPN);
    }

    public void OnClick_UserLevelUp()
    {
        UserData userdata = GameInfo.Instance.UserData;
        if (userdata == null)
            return;

        if (GameSupport.IsMaxAccountLevel(userdata.Level))
            return;

        userdata.Level = userdata.Level + 1;
        userdata.Exp = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == GameInfo.Instance.GameConfig.AccountLevelUpGroup && x.Level == userdata.Level - 1).Exp;

        NetLocalSvr.Instance.SaveLocalData();
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
    }

    public void OnClick_CharLevelUp()
    {

        CharData chardata = GameInfo.Instance.GetMainChar();
        if (chardata == null)
            return;

        if (GameSupport.IsMaxCharLevel(chardata.Level, chardata.Grade))
            return;

        chardata.Level = chardata.Level + 1;
        chardata.Exp = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == GameInfo.Instance.GameConfig.CharLevelUpGroup && x.Level == chardata.Level - 1).Exp;

        NetLocalSvr.Instance.SaveLocalData();
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");

    }

	public void OnBtnGradeLevelUp() {
		CharData charData = GameInfo.Instance.GetMainChar();
		if ( charData == null ) {
			return;
		}

		int maxLevelInCurGrade = GameSupport.GetCharMaxLevel( charData.Grade );
		if ( GameSupport.IsMaxGrade( charData.Grade ) || charData.Level >= maxLevelInCurGrade ) {
			return;
		}

		charData.Level = maxLevelInCurGrade;
		charData.Exp = GameInfo.Instance.GameTable.FindLevelUp( x => x.Group == GameInfo.Instance.GameConfig.CharLevelUpGroup && 
																	 x.Level == charData.Level - 1 ).Exp;

		NetLocalSvr.Instance.SaveLocalData();
		LobbyUIManager.Instance.Renewal( "TopPanel" );
		LobbyUIManager.Instance.Renewal( "GoodsPopup" );
	}

    public void OnClick_StoryClear()
    {
        UserData userdata = GameInfo.Instance.UserData;

        var stagelist = GameInfo.Instance.GameTable.FindAllStage(x => x.StageType == (int)eSTAGETYPE.STAGE_MAIN_STORY);
        for (int i = 0; i < stagelist.Count; i++)
        {
            var cleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == stagelist[i].ID);
            if (cleardata == null)
            {
                if (stagelist[i].Chapter <= GameInfo.Instance.GameClientTable.Chapters.Count)
                    GameInfo.Instance.StageClearList.Add(new StageClearData(stagelist[i].ID));
            }
        }

        stagelist = GameInfo.Instance.GameTable.FindAllStage(x => x.StageType == (int)eSTAGETYPE.STAGE_DAILY);
        //stagelist = GameInfo.Instance.GameTable.FindAllStage(x => x.StageType == (int)eSTAGETYPE.STAGE_DAILY && x.Chapter == 1);
        for (int i = 0; i < stagelist.Count; i++)
        {
            var cleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == stagelist[i].ID);
            if (cleardata == null)
                GameInfo.Instance.StageClearList.Add(new StageClearData(stagelist[i].ID));
        }

        stagelist = GameInfo.Instance.GameTable.FindAllStage(x => x.StageType == (int)eSTAGETYPE.STAGE_PVP_PROLOGUE);
        for (int i = 0; i < stagelist.Count; i++)
        {
            var cleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == stagelist[i].ID);
            if (cleardata == null)
                GameInfo.Instance.StageClearList.Add(new StageClearData(stagelist[i].ID));
        }

        stagelist = GameInfo.Instance.GameTable.FindAllStage(x => x.StageType == (int)eSTAGETYPE.STAGE_EVENT);
        for (int i = 0; i < stagelist.Count; i++)
        {
            var cleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == stagelist[i].ID);
            if (cleardata == null)
                GameInfo.Instance.StageClearList.Add(new StageClearData(stagelist[i].ID));
        }
        NetLocalSvr.Instance.SaveLocalData();
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
    }
    
    public void OnClick_MissionClear()
    {
        if( GameInfo.Instance.StageClearList.Count == 0 )
        {
            GameInfo.Instance.StageClearList.Add(new StageClearData(1));
        }
        else
        {
            int tableid = GameInfo.Instance.StageClearList[GameInfo.Instance.StageClearList.Count - 1].TableData.NextStage;
            if (tableid != -1 )
            {
                var cleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == tableid);
                if (cleardata == null)
                    GameInfo.Instance.StageClearList.Add(new StageClearData(tableid));
            }
        }
       
        NetLocalSvr.Instance.SaveLocalData();
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
    }

    public void OnClick_AddGoods()
    {
        UserData userdata = GameInfo.Instance.UserData;

        userdata.Goods[(int)eGOODSTYPE.GOLD] += 10000000;
        userdata.Goods[(int)eGOODSTYPE.CASH] += 10000;
        userdata.Goods[(int)eGOODSTYPE.ROOMPOINT] += 10000;

        for(int i = 0; i < GameInfo.Instance.CharList.Count; i++)
        {
            GameInfo.Instance.CharList[i].PassviePoint += 10000;
        }

        NetLocalSvr.Instance.SaveLocalData();
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
    }

    public void OnClick_TimeInit()
    {
        for (int i = 0; i < GameInfo.Instance.CardBookList.Count; i++)
        {
            var card = GameInfo.Instance.CardBookList[i];
            card.DoOnFlag(eBookStateFlag.MAX_FAVOR_LV);
        }

        for (int i = 0; i < GameInfo.Instance.AchieveList.Count; i++)
        {
            var data = GameInfo.Instance.AchieveList[i];
            if (data.TableData.AchieveType != "AM_Adv_StoryClear")
                data.Value = data.TableData.AchieveValue;
        }
        /*
        for( int i = 0; i < GameInfo.Instance.CardList.Count; i++ )
        {
            var card = GameInfo.Instance.CardList[i];
            card.SkillLv = GameInfo.Instance.GameConfig.CardMaxSkillLevel;
        }

        for (int i = 0; i < GameInfo.Instance.CardBookList.Count; i++ )
        {
            var book = GameInfo.Instance.CardBookList[i];
            //77778 book.FavorLevel = GameInfo.Instance.GameConfig.CardMaxFavorLevel;
        }
        */
        /*
        UserData userdata = GameInfo.Instance.UserData;
        //        userdata.ExpMissionRemainTime = System.DateTime.Now;
        //        userdata.GoodsMissionRemainTime = System.DateTime.Now;
        userdata.Goods[(int)eGOODSTYPE.TICKET] = GameSupport.GetMaxAP();
        GameInfo.Instance.SaveLocalData();
        LobbyUIManager.Instance.Renewal("TopPanel");
        */
    }

    public void OnClick_AddItem()
    {
        for (int i = 0; i < GameInfo.Instance.GameConfig.InitItemList.Count; i++)
        {
            NetLocalSvr.Instance.AddItem(GameInfo.Instance.GameConfig.InitItemList[i], 1000);
        }
        NetLocalSvr.Instance.SaveLocalData();
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
    }

    public void OnClick_Init()
    {
        MessagePopup.CYN(eTEXTID.TITLE_NOTICE, 1214, eTEXTID.OK, eTEXTID.CANCEL, OnMsg_GameInit);
    }

    public void OnMsg_GameInit()
    {
        PlayerPrefs.DeleteAll();
        GameInfo.Instance.MoveLobbyToTitle();
    }

    public void ChangeLanguage(eLANGUAGE e)
    {
        FLocalizeString.Instance.InitLocalize(e);
        FLocalizeString.Instance.SaveLanguage();

        var root = GameObject.Find("UIRoot");
        if (root == null)
            return;
        var labellist = root.GetComponentsInChildren<FLocalizeLabel>(true);
        for (int i = 0; i < labellist.Length; i++)
            labellist[i].OnLocalize();
        
        LobbyUIManager.Instance.Renewal();
    }
}
