using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum ePANELTYPE
{
    NULL = -1,
    MAIN = 0,
    CHARMAIN,
    CHARINFO,
    GACHA,
    ITEM,
    STORE,
    FACILITY,
    FACILITYITEM,
    FIGUREROOM,
    FIGUREROOM_FREELOOK,
    FIGUREROOM_EDITMODE,
    FIGUREROOM_SAVELOAD,
    STORYMAIN,
    STORY,
    DAILY,
    TIMEATTACK,
    EVENT_STORY_MAIN,            //가챠형 이벤트 메인
    EVENT_STORY_GACHA,
    EVENT_STORY_STAGE,
    EVENT_CHANGE_MAIN,           //아이템 교환형 이벤트 메인
    EVENT_CHANGE_STAGE,           //아이템 교환형 이벤트 메인
    EVENT_CHANGE_REWARD,           //아이템 교환형 이벤트 메인
    EVENT_MISSION,          //미션형 이벤트 메인
    ARENA,                  //아레나 프롤로그 스테이지 or 아레나 입장
    ARENA_TITLE,            //아레나 배틀 입장, 집계중, 정산
    ARENA_MAIN,             //아레나 입장 후 배치 및 배틀 시작

    CARD_DISPATCH,          //파견-서포터

    ARENATOWER,            //아레나 - 타워
    ARENATOWER_STAGE,      //아레나 - 타워 - 스테이지

    FACILITYTRADE,
    EVENT_BOARD_MAIN,

    RAID_MAIN,
    RAID,
    //    MAIN_IN,
    CircleJoin,
    CircleLobby,
}

public enum ePOPUPGOODSTYPE
{   // 중간에 삽입하면 문제 발생, 각 팝업 프리팹에 설정한 배열 순서 값이 밀릴 수 있음
    EXCEPT = -1,
    NONE = 0,
    BASE,
    SKILLPOINT,
    ROOMPOINT,
    MPOINT,
    BASEBP,
    BASE_NONE_BTN,
    SKILLPOINT_NONE_BTN,
    ROOMPOINT_NONE_BTN,
    MPOINT_NONE_BTN,
    BASEBP_NONE_BTN,
    ARENAPOINT,
    ARENAPOINT_NONE_BTN,
    FRIENDPOINT,
    FRIENDPOINT_NONE_BTN,
    DESIREPOINT,
    DESIREPOINT_NONE_BTN,
    AWAKEN_SKILL_POINT,
    EVENTSTORE_POINT,
    NONE_BTN,
    RAID_POINT,
    CIRCLE_POINT,
    CIRCLE_GOLD,
};


public enum eCharPanelTab
{
    CHARINFO_TAB = 0,
    CHARITEM_TAB,
    CHARSKILL_TAB,
    CHARCUSTOM_TAB,
}

public class GifAnimationData
{
    public GameObject ParentObject;
    public GameObject MySelfObject;
    public UITexture TargetUITexture;
}

public class LobbyUIManager : FManager
{
    private static LobbyUIManager s_instance = null;
    public static LobbyUIManager Instance
    {
        get
        {
            return s_instance;
        }
    }
    public UIHoldGauge kHoldGauge;
    public GameObject block = null;

    private ePANELTYPE _prepaneltype = ePANELTYPE.NULL;
    private ePANELTYPE _paneltype = ePANELTYPE.NULL;
    private ePANELTYPE _panelTypeRemamberStart = ePANELTYPE.NULL;   //  back키로 되돌아 가기 위한 패널 기억
    private int _panelbgindex = -1;
    
    public GameObject kBlackScene = null;
    //public List<GameObject> kTouchObj;
    private UITopPanel _toppanel;
    private UIGoodsPopup _goodspopup;
    private FComponent _showCurrentPanel = null;    //  현재 열려있는 패널
    private List<FComponent> _activepopuplist = new List<FComponent>();

    private UICamera _uiCamera = null;              //로비에서 MultiTouch 막기 위해 필요
    private Coroutine _loginBonusCoroutine = null;

    private UniGif _uniGifValue = new UniGif();
    private Dictionary<string, List<UniGif.GifTexture>> _dicGifTexture = new Dictionary<string, List<UniGif.GifTexture>>();
    private Dictionary<string, List<GifAnimationData>> _dicGifAnimation = new Dictionary<string, List<GifAnimationData>>();
    private Dictionary<string, int> _dicCurrentGifTextureIndex = new Dictionary<string, int>();
    private List<GifAnimationData> _deleteGifAnimationList = new List<GifAnimationData>();

    private bool mbLockEscape = false;

    public UICamera uiCamera { get { return _uiCamera; } }

    public int JoinStageID = -1;
    public int JoinCharSeleteIndex = -1;
    
    public bool StorePanelUpdateSlotList = true;
    public float StorePanelLocalPosY = 0;

    public List<FComponent> ActivePopupList { get { return _activepopuplist; } }
    public ePANELTYPE PanelType { get { return _paneltype; } }
    public ePANELTYPE PrepanelType { get { return _prepaneltype; } }
    public eLoginBonusStep LoginBonusStep { get; set; } = eLoginBonusStep.End;
    public bool IsOnceLoginBonusPopup { get; set; }

    private float mGcCheckTime = 0.0f;


    void Awake()
    {
        if (s_instance == null)
            s_instance = this;
        else
            Debug.LogError("LobbyUIManager Instance Error!");

        kBlackScene.SetActive(true);

        BoxCollider boxCol = block.GetComponent<BoxCollider>();
        boxCol.size = new Vector3(Screen.width, Screen.height, 1.0f);
        block.GetComponentInChildren<UISprite>().SetDimensions(Screen.width, Screen.height);
        block.SetActive(false);

        UIRoot uiroot = this.transform.parent.GetComponent<UIRoot>();
        if(uiroot != null)
        {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            uiroot.scalingStyle = UIRoot.Scaling.Constrained;
#endif
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
	}

    public override void Start()
    {
        base.Start();

        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.None)
        {
            OnPrepare();
        }

        if (!IAPManager.Instance.IAPNULL_CHECK())
            IAPManager.Instance.Init();

        StartCoroutine(nameof(LoadGifTextures));
    }

    public override void OnPrepare()
    {
        base.OnPrepare();

        if (_uiCamera == null)
            _uiCamera = UICamera.FindCameraForLayer((int)eLayer.UI);

        if (FSaveData.Instance.Graphic <= 1)
        {
            Lobby.Instance.lobbyCamera.EnablePostProcess(false);
        }
        else
        {
            Lobby.Instance.lobbyCamera.EnablePostProcess(true);
        }

        AppMgr.Instance.CustomInput.ShowCursor(true);
        SetUICameraMultiTouch(false);
        ePANELTYPE epanel = ePANELTYPE.NULL;
        
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.None)//로비로 바로 실행시킴
        {
        #if UNITY_EDITOR
            AppMgr.Instance.GetServerJsonUrl(false);
#endif

			if ( GameInfo.Instance.CharList.Count <= 0 ) {
				NetLocalSvr.Instance.LoadLocalData();

                for ( int i = 0; i < GameInfo.Instance.GameTable.Characters.Count; i++ ) {
                    NetLocalSvr.Instance.AddChar( GameInfo.Instance.GameTable.Characters[i].ID );
                }

				GameInfo.Instance.UserData.MainCharUID = GameInfo.Instance.CharList[GameInfo.Instance.CharList.Count - 1].CUID;
				NetLocalSvr.Instance.SaveLocalData();
			}

			AppMgr.Instance.GetConfigFontLoad();
            AppMgr.Instance.SetSceneType(AppMgr.eSceneType.Lobby);
            GameInfo.Instance.DoInitGame(AppMgr.Instance.configData.m_Network);
            GameInfo.Instance.SvrConnect_Login(true, OnNetLogin);
        }
        else
        {
            //다른씬에서 로비로 돌아오면 디렉터 off
            Director.IsPlaying = false;

            AppMgr.Instance.SetSceneType(AppMgr.eSceneType.Lobby);
            UIValue.Instance.SetValue(UIValue.EParamType.DeckSel, 0);
            Lobby.Instance.InitLobby();
            ShowUI("TopPanel", true);
            epanel = ePANELTYPE.MAIN;
            NotificationManager.Instance.Init();
            
            bool bGameClear = false;
            bool bGameResult = false;
            GameTable.Stage.Param stageParam = GameInfo.Instance.GameTable.FindStage(GameInfo.Instance.StageClearID);
            if (stageParam != null)
            {
                bGameClear = true;
                if (stageParam.StageType != (int)eSTAGETYPE.STAGE_SPECIAL)
                {
                    UIValue.Instance.SetValue(UIValue.EParamType.StageID, stageParam.ID);

                    bGameResult = true;
                    ShowUI("BattleResultPopup", true);
                }
                else
                {
                    GameInfo.Instance.StageClearID = -1;
                }
            }
            else
            {
                int nStageId = (int)UIValue.Instance.TryGetValue(UIValue.EParamType.LastPlayStageID, -1);
                if (0 <= nStageId)
                {
                    if (UIValue.Instance.GetValue(UIValue.EParamType.LobbyToTrainingRoom) == null)
                    {
                        stageParam = GameInfo.Instance.GameTable.FindStage(nStageId);
                    }
                }
                
                if (GameInfo.Instance.bStageFailure)
                {
                    if (stageParam != null && stageParam.StageType != (int)eSTAGETYPE.STAGE_SPECIAL)
                    {
                        ShowHowToStrongPopup();
                    }
                }
            }
            
            _StageEndCheck(stageParam, bGameClear, ref epanel);
            
            if (epanel != ePANELTYPE.NULL && !GameInfo.Instance.ArenaGameEnd_Flag)
            {
                if (GameSupport.IsInLobbyTutorial() || GameInfo.Instance.TutorialSkipFlag)
                {
                    GameInfo.Instance.TutorialSkipFlag = false;
                    epanel = ePANELTYPE.MAIN;
                }

                SetPanelType(epanel);
                if (epanel == ePANELTYPE.MAIN)
                {
                    object fromTrainingRoom = UIValue.Instance.GetValue(UIValue.EParamType.LobbyToTrainingRoom);
                    if (fromTrainingRoom == null)
                    {
                        kBlackScene.SetActive(false);
                    }
                }

                if (!bGameResult && epanel == ePANELTYPE.MAIN && !GameInfo.Instance.IsTowerStage)
                {
                    SetPanelType(ePANELTYPE.MAIN);
                    if (GameSupport.IsInLobbyTutorial())
                    {
                        GameSupport.ShowTutorial();
                    }
                }
            }

            if (GameInfo.Instance.ArenaGameEnd_Flag)
            {
                GameInfo.Instance.ArenaGameEnd_Flag = false;
                GameInfo.Instance.Send_ReqArenaRankingList(GameInfo.Instance.ArenaRankingList.UpdateTM, OnNetArenaRankingList);
            }

            for (int i = 0; i < PanelBGList.Count; i++)
                PanelBGList[i].SetActive(false);

            GameInfo.Instance.IsTowerStage = false;
        }
    }
    
    private void _StageEndCheck(GameTable.Stage.Param pStageParam, bool pGameClear, ref ePANELTYPE pPanelType)
    {
        if (GameInfo.Instance.ArenaGameEnd_Flag)
        {
            return;
        }
        
        if (GameInfo.Instance.IsFriendArenaGame)
        {
            GameInfo.Instance.IsFriendArenaGame = false;
            return;
        }
        
        if (GameInfo.Instance.IsPrevSkillTrainingRoom)
        {
            GameInfo.Instance.IsPrevSkillTrainingRoom = false;
            return;
        }
        
        if (GameInfo.Instance.IsTowerStage)
        {
            pPanelType = ePANELTYPE.ARENATOWER;
            return;
        }
        
        if (pStageParam == null)
        {
            return;
        }

        Enum.TryParse(pStageParam.StageType.ToString(), out eSTAGETYPE stageType);

        switch (stageType)
        {
            case eSTAGETYPE.STAGE_MAIN_STORY:
            {
                int nStageId = GameSupport.GetStorySuitableStageID(eSTAGETYPE.STAGE_MAIN_STORY);
                UIValue.Instance.SetValue(UIValue.EParamType.StoryStageID, nStageId);
                pPanelType = ePANELTYPE.STORY;
                break;
            }
            case eSTAGETYPE.STAGE_DAILY:
            {
                pPanelType = ePANELTYPE.DAILY;
                break;
            }
            case eSTAGETYPE.STAGE_SECRET:
            {
                pPanelType = ePANELTYPE.DAILY;
                UIStageDetailPopup stageDetailPopup = GetUI<UIStageDetailPopup>("StageDetailPopup");
                if (stageDetailPopup != null)
                {
                    stageDetailPopup.SetSecretStage();
                    stageDetailPopup.SetUIActive(true);
                }
                break;
            }
            case eSTAGETYPE.STAGE_EVENT: case eSTAGETYPE.STAGE_EVENT_BONUS:
            {
                int nEventId = pStageParam.TypeValue;
                UIValue.Instance.SetValue(UIValue.EParamType.EventID, nEventId);
                int nStageId = GameSupport.GetEventSuitableStageID(nEventId);
                UIValue.Instance.SetValue(UIValue.EParamType.EventStageID, nStageId);

                GameTable.EventSet.Param eventSetParam = GameInfo.Instance.GameTable.FindEventSet(x => x.EventID == nEventId);
                if (eventSetParam != null)
                {
                    if (eventSetParam.EventType == (int)eEventRewardKind.RESET_LOTTERY)
                    {
                        UIValue.Instance.SetValue(UIValue.EParamType.EventStagePopupType, (int)eEventStageType.EVENT);
                        pPanelType = ePANELTYPE.EVENT_STORY_STAGE;
                    }
                    else if (eventSetParam.EventType == (int) eEventRewardKind.EXCHANGE)
                    {
                        pPanelType = ePANELTYPE.EVENT_CHANGE_STAGE;
                    }
                    else if (eventSetParam.EventType == (int)eEventRewardKind.MISSION)
                    {
                        //추후 구현
                    }
                }
                break;
            }
            case eSTAGETYPE.STAGE_PVP_PROLOGUE:
            {
                int nStageId = GameSupport.GetStorySuitableStageID(eSTAGETYPE.STAGE_PVP_PROLOGUE);
                UIValue.Instance.SetValue(UIValue.EParamType.ArenaPrologueStageID, nStageId);
                pPanelType = ePANELTYPE.EVENT_STORY_STAGE;
                break;
            }
            case eSTAGETYPE.STAGE_TIMEATTACK:
            {
                pPanelType = ePANELTYPE.NULL;
                GameInfo.Instance.Send_ReqTimeAtkRankingList(OnNetTimeAtkRankingList);
                break;
            }
            case eSTAGETYPE.STAGE_SPECIAL:
            {
                pPanelType = ePANELTYPE.STORYMAIN;
                if (!pGameClear)
                {
                    ShowUI("SpecialModePopup", true);
                }
                break;
            }

            case eSTAGETYPE.STAGE_RAID:
                pPanelType = ePANELTYPE.NULL;
                GameInfo.Instance.Send_ReqRaidRankingList( OnNetRaidRankingList );
                break;
        }
    }

    public static int teststageid = 1;

    private void ShowHowToStrongPopup()
    {
        UIValue.Instance.SetValue(UIValue.EParamType.HowToBeStrongCUID, GameInfo.Instance.SeleteCharUID);
        ShowUI("HowToBeStrongPopupV2", true);
    }

	public override void Update() {
		base.Update();

		if ( AppMgr.Instance.GcCollectInLobby ) {
			if ( AppMgr.Instance.IsAppPause ) {
				mGcCheckTime = 0.0f;
			}
			else {
				mGcCheckTime += Time.deltaTime;
				if ( mGcCheckTime >= 300.0f ) {
					Resources.UnloadUnusedAssets();
					GC.Collect();

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

	public void GetUserMarkIcon(GameObject parentObj, GameObject mySelfObj, int userMarkTID, ref UITexture uITexture)
    {
        GameTable.UserMark.Param userMark = GameInfo.Instance.GameTable.FindUserMark(x => x.ID == userMarkTID);
        if (userMark == null)
        {
            return;
        }

        if (parentObj == null)
        {
            FComponent fComponent = mySelfObj.GetComponentInParent<FComponent>();
            if (fComponent != null)
            {
                parentObj = fComponent.gameObject;
            }
        }

        foreach (KeyValuePair<string, List<GifAnimationData>> keyValuePair in _dicGifAnimation)
        {
            GifAnimationData gifAnimationData = keyValuePair.Value.Find(x => x.ParentObject == parentObj && x.MySelfObject == mySelfObj);
            if (gifAnimationData != null)
            {
                gifAnimationData.ParentObject = null;
                gifAnimationData.MySelfObject = null;
                gifAnimationData.TargetUITexture = null;
            }
        }

        switch (userMark.AniEnable)
        {
            case 0:
                {
                    Texture texture = ResourceMgr.Instance.LoadFromAssetBundle("icon", Utility.AppendString("Icon/User/", userMark.Icon, ".png")) as Texture;
                    if (texture == null)
                    {
                        return;
                    }

                    uITexture.mainTexture = texture;
                }
                break;
            case 1:
                {
                    if (_dicGifAnimation.ContainsKey(userMark.Icon))
                    {
                        if (_dicGifAnimation[userMark.Icon].Any(x => x.ParentObject == parentObj && x.MySelfObject == mySelfObj))
                        {
                            return;
                        }

                        _dicGifAnimation[userMark.Icon].Add(new GifAnimationData() { ParentObject = parentObj, MySelfObject = mySelfObj, TargetUITexture = uITexture });

                        if (_dicGifTexture.ContainsKey(userMark.Icon) && _dicCurrentGifTextureIndex.ContainsKey(userMark.Icon))
                        {
                            int index = _dicCurrentGifTextureIndex[userMark.Icon];
                            if (0 <= index && index < _dicGifTexture[userMark.Icon].Count)
                            {
                                uITexture.mainTexture = _dicGifTexture[userMark.Icon][index].m_texture2d;
                            }
                        }
                    }
                }
                break;
        }
    }

    public Texture GetCircleMarkTexture(int tableId, bool isSubIcon = false)
    {
        Texture result = null;
        GameTable.CircleMark.Param param = GameInfo.Instance.GameTable.FindCircleMark(tableId);
        if (param != null)
        {
            if (isSubIcon)
            {
                result = ResourceMgr.Instance.LoadFromAssetBundle("icon", Utility.AppendString("Icon/Circle/", param.SubIcon, ".png")) as Texture;
            }
            else
            {
                result = ResourceMgr.Instance.LoadFromAssetBundle("icon", Utility.AppendString("Icon/Circle/", param.Icon, ".png")) as Texture;
            }
        }
        return result;
    }

    public Color32 GetCircleMarkColor(int tableId)
    {
        Color result = Color.white;
        GameTable.CircleMark.Param param = GameInfo.Instance.GameTable.FindCircleMark(tableId);
        if (param != null)
        {
            string[] splits = param.Icon.Split(',');
            if (3 <= splits.Length)
            {
                int.TryParse(splits[0], out int colorR);
                int.TryParse(splits[1], out int colorB);
                int.TryParse(splits[2], out int colorG);

                result = new Color32((byte)colorR, (byte)colorG, (byte)colorB, byte.MaxValue);
            }
        }
        return result;
    }

    private IEnumerator LoadGifTextures()
    {
        List<GameTable.UserMark.Param> userMarkList = GameInfo.Instance.GameTable.FindAllUserMark(x => x.AniEnable == 1);
        
        int index = 0;
        string iconName = string.Empty;
        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

        while(index < userMarkList.Count)
        {
            iconName = userMarkList[index].Icon;

            if (!_dicGifTexture.ContainsKey(iconName))
            {
                _dicGifTexture.Add(iconName, new List<UniGif.GifTexture>());
            }
            else
            {
                _dicGifTexture[iconName].Clear();
            }

            if (!_dicCurrentGifTextureIndex.ContainsKey(iconName))
            {
                _dicCurrentGifTextureIndex.Add(iconName, 0);
            }

            TextAsset gifTextAsset = ResourceMgr.Instance.LoadFromAssetBundle("icon", Utility.AppendString("Icon/User/Gif/", iconName, ".bytes")) as TextAsset;
            if (gifTextAsset == null)
            {
                ++index;
                continue;
            }

            bool isWait = true;

            _uniGifValue.GetTextureListCoroutine(gifTextAsset.bytes, (textureList, loopCount, width, height) =>
            {
                _dicGifTexture[iconName].AddRange(textureList);
                isWait = false;
                StartCoroutine(nameof(PlayGifAnimation), iconName);
            });

            while (isWait)
            {
                yield return waitForFixedUpdate;
            }

            ++index;
        }
    }

    private IEnumerator PlayGifAnimation(string iconName)
    {
        if (!_dicGifTexture.ContainsKey(iconName))
        {
            yield break;
        }

        if (!_dicGifAnimation.ContainsKey(iconName))
        {
            _dicGifAnimation.Add(iconName, new List<GifAnimationData>());
        }

        List<GifAnimationData> gifAnimationList = _dicGifAnimation[iconName];
        List<UniGif.GifTexture> gifTextureList = _dicGifTexture[iconName];

        int index = 0;
        float waitTime = 0;
        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

        while (true)
        {
            yield return waitForFixedUpdate;

            waitTime += Time.fixedDeltaTime;
            if (waitTime < gifTextureList[index].m_delaySec)
            {
                continue;
            }

            waitTime = 0;

            for (int i = 0; i < gifAnimationList.Count; i++)
            {
                if (gifAnimationList[i].ParentObject == null || !gifAnimationList[i].ParentObject.activeSelf)
                {
                    _deleteGifAnimationList.Add(gifAnimationList[i]);
                    continue;
                }

                if (gifAnimationList[i].TargetUITexture == null)
                {
                    _deleteGifAnimationList.Add(gifAnimationList[i]);
                    continue;
                }

                gifAnimationList[i].TargetUITexture.mainTexture = gifTextureList[index].m_texture2d;
            }

            for(int i = 0; i < _deleteGifAnimationList.Count; i++)
            {
                gifAnimationList.Remove(_deleteGifAnimationList[i]);
            }

            if (0 < _deleteGifAnimationList.Count)
            {
                _deleteGifAnimationList.Clear();
            }

            _dicCurrentGifTextureIndex[iconName] = index;

            ++index;
            if (gifTextureList.Count <= index)
            {
                index = 0;
            }
        }
    }

    public void EnableBlockUI(bool enable, float autoDisableTime = 0.0f)
    {
        block.SetActive(enable);

        if (enable && autoDisableTime > 0.0f)
        {
            StopCoroutine("DelayedDisableBlockUI");
            StartCoroutine("DelayedDisableBlockUI", autoDisableTime);
        }
    }

    private IEnumerator DelayedDisableBlockUI(float delay)
    {
        yield return new WaitForSeconds(delay);
        EnableBlockUI(false);
    }

    public bool IsBlockingUI()
    {
        return block.activeSelf;
    }

    public bool IsShowCostumeSlot(GameTable.Costume.Param costumeParam)
    {
        if (costumeParam == null)
        {
            return false;
        }
		
        if (costumeParam.PreVisible <= 0 || costumeParam.StoreID < 0)
        {
            return false;
        }
        
        GameTable.Store.Param storeParam = GameInfo.Instance.GameTable.FindStore(costumeParam.StoreID);
        if (storeParam == null)
        {
            return false;
        }
        
        if (storeParam.SaleType == 0)
        {
            return true;
        }
        
        GachaCategoryData gachaCategoryData = GameSupport.GetGachaCategoryData(storeParam.ID);
        if (gachaCategoryData == null)
        {
            return false;
        }
        
        return gachaCategoryData.StartDate <= GameSupport.GetCurrentServerTime() && GameSupport.GetCurrentServerTime() <= gachaCategoryData.EndDate;
    }

    public bool IsShowCostumeSlot(GameClientTable.StoreDisplayGoods.Param param)
    {
        if (param == null)
        {
            return false;
        }
        
        if (param.StoreID < 0)
        {
            return false;
        }
        
        GameTable.Store.Param storeParam = GameInfo.Instance.GameTable.FindStore(param.StoreID);
        if (storeParam == null)
        {
            return false;
        }
        
        if (storeParam.SaleType == 0)
        {
            return true;
        }
        
        GachaCategoryData gachaCategoryData = GameSupport.GetGachaCategoryData(storeParam.ID);
        if (gachaCategoryData == null)
        {
            return false;
        }
        
        return gachaCategoryData.StartDate <= GameSupport.GetCurrentServerTime() && GameSupport.GetCurrentServerTime() <= gachaCategoryData.EndDate;
    }

    public static ePANELTYPE GetPanelType(string subject)
    {
		string[] strs = Enum.GetNames(typeof(ePANELTYPE));
		for(int i = 0; i < strs.Length; ++i)
		{
			if(strs[i].CompareTo(subject) == 0)
			{
				return (ePANELTYPE)i;
			}
		}
		/*
        foreach (ePANELTYPE type in (ePANELTYPE[])System.Enum.GetValues(typeof(ePANELTYPE)))
        {
            if (type.ToString() == subject)
                return type;
        }
		*/
        return ePANELTYPE.NULL;
    }

    public void SetPanelType(ePANELTYPE type, bool bImmediately = false, bool forceChange = false )
    {
        FComponent panel = null;
        
        if (!forceChange && _paneltype == type)
        {
            switch (_paneltype)
            {
                case ePANELTYPE.GACHA:
                    panel = GetUI("GachaPanel");
                    break;
                case ePANELTYPE.STORE:
                    panel = GetUI("StorePanel");
                    break;
            }
            
            if (panel != null)
            {
                panel.TabRenewal();
                panel.Renewal();
            }
            
            return;
        }
        
        switch (_paneltype)
        {
            case ePANELTYPE.MAIN:
                panel = GetUI("MainPanel");
                break;
            case ePANELTYPE.GACHA:
                panel = GetUI("GachaPanel");
                break;
            case ePANELTYPE.CHARMAIN:
                panel = GetUI("CharMainPanel");
                break;
            case ePANELTYPE.CHARINFO:
                panel = GetUI("CharInfoPanel");
                break;
            case ePANELTYPE.ITEM:
                panel = GetUI("ItemPanel");
                break;
            case ePANELTYPE.STORE:
                panel = GetUI("StorePanel");
                break;
            case ePANELTYPE.FACILITY:
                panel = GetUI("FacilityPanel");
                break;
            case ePANELTYPE.FACILITYITEM:
                panel = GetUI("FacilityItemPanel");
                break;
            case ePANELTYPE.FIGUREROOM:
                panel = GetUI("FigureRoomPanel");
                break;
            case ePANELTYPE.FIGUREROOM_FREELOOK:
                panel = GetUI("FigureRoomFreeLookPanel");
                break;
            case ePANELTYPE.FIGUREROOM_EDITMODE:
                panel = GetUI("FigureRoomEditModePanel");
                break;
            case ePANELTYPE.FIGUREROOM_SAVELOAD:
                panel = GetUI("FigureRoomSaveLoadPanel");
                break;
            case ePANELTYPE.STORYMAIN:
                panel = GetUI("StoryMainPanel");
                break;
            case ePANELTYPE.STORY:
                panel = GetUI("StoryPanel");
                break;
            case ePANELTYPE.DAILY:
                panel = GetUI("DailyPanel");
                break;
            case ePANELTYPE.TIMEATTACK:
                panel = GetUI("TimeAttackPanel");
                break;
            case ePANELTYPE.EVENT_STORY_MAIN:
                panel = GetUI("EventmodeStoryMainPanel");
                break;
            case ePANELTYPE.EVENT_STORY_GACHA:
                panel = GetUI("EventmodeStoryResetGachaPanel");
                break;
            case ePANELTYPE.EVENT_STORY_STAGE:
                panel = GetUI("EventmodeStoryStagePanel");
                break;
            case ePANELTYPE.EVENT_CHANGE_MAIN:
                panel = GetUI("EventmodeExchangeMainPanel");
                break;
            case ePANELTYPE.EVENT_CHANGE_STAGE:
                panel = GetUI("EventmodeExchangeStagePanel");
                break;
               case ePANELTYPE.EVENT_CHANGE_REWARD:
                panel = GetUI("EventmodeExchangePanel");
                break;
            case ePANELTYPE.EVENT_MISSION:
                break;
            case ePANELTYPE.ARENA:
                panel = GetUI("ArenaPanel");
                break;
            case ePANELTYPE.ARENA_TITLE:
                panel = GetUI("ArenaTitlePanel");
                break;
            case ePANELTYPE.ARENA_MAIN:
                panel = GetUI("ArenaMainPanel");
                break;
            case ePANELTYPE.CARD_DISPATCH:
                panel = GetUI("CardDispatchPanel");
                break;
            case ePANELTYPE.ARENATOWER:
                panel = GetUI("ArenaTowerMainPanel");
                break;
            case ePANELTYPE.ARENATOWER_STAGE:
                panel = GetUI("ArenaTowerStagePanel");
                break;
            case ePANELTYPE.EVENT_BOARD_MAIN:
                panel = GetUI("EventBoardMainPanel");
                break;

            case ePANELTYPE.RAID_MAIN:
                panel = GetUI( "RaidMainPanel" );
                break;

            case ePANELTYPE.RAID:
                panel = GetUI( "RaidPanel" );
                break;
            case ePANELTYPE.CircleJoin:
                panel = GetUI("CircleJoinPanel");
                break;
            case ePANELTYPE.CircleLobby:
                panel = GetUI("CircleLobbyPanel");
                break;
        }


        if (type == ePANELTYPE.EVENT_STORY_MAIN || type == ePANELTYPE.EVENT_CHANGE_MAIN || type == ePANELTYPE.EVENT_MISSION)
        {
            if (_paneltype == ePANELTYPE.MAIN)
            {
                _panelTypeRemamberStart = ePANELTYPE.MAIN;
            }
        }

        float fwait = 0.0f;
        if (panel != null)
        {
            _panelbgindex = panel.kBGIndex;
            fwait = panel.GetCloseAniTime();
            HideUI(panel.name);
        }

        CancelInvoke("NextShowPanel");

        _prepaneltype = _paneltype;
        _paneltype = type;


        
        if (bImmediately)
        {
            NextShowPanel();
        }
        else
        {
            Invoke("NextShowPanel", fwait);
        }
    }

    private void NextShowPanel()
    {
        if (_toppanel == null)
            _toppanel = GetUI<UITopPanel>("TopPanel");

        switch (_paneltype)
        {
            case ePANELTYPE.MAIN:
                {
                    NotificationManager.Instance.Init(); //77889 
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.MAIN);
                    _showCurrentPanel = ShowUI("MainPanel", true);

                    //로비 들어올때 RenderTargetChar을 MainPanel에서 꺼줘서 여기에 적용
                    var fromTrainingRoom = UIValue.Instance.GetValue(UIValue.EParamType.LobbyToTrainingRoom, true);
                    if (fromTrainingRoom != null)
                    {
                        string fromTrainingRoomUI = (string)fromTrainingRoom;

                        var trainingRoomCharTID = UIValue.Instance.GetValue(UIValue.EParamType.TrainingCharTID, true);
                        if (trainingRoomCharTID != null)
                        {
                            int charTID = (int)trainingRoomCharTID;
                            Debug.Log("TrainingCharTID : " + charTID);

                            if (fromTrainingRoomUI.Equals("CharInfoTabSkillPanel"))
                            {
                                UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, 2);
                                SetPanelType(ePANELTYPE.CHARINFO);
                            }
                            else if (fromTrainingRoomUI.Equals("BookCharInfoPopup"))
                            {
                                LobbyUIManager.Instance.ShowUI("BookMainPopup", true);
                                RenderTargetChar.Instance.gameObject.SetActive(true);
                                LobbyUIManager.Instance.ShowUI("BookCharListPopup", true);
                                LobbyUIManager.Instance.ShowUI("BookCharInfoPopup", true);
                            }
                        }
                    }

                }
                break;
            case ePANELTYPE.CHARMAIN:
                {
                    _showCurrentPanel = ShowUI("CharMainPanel", true);
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.NORMAL);
                }
                break;
            case ePANELTYPE.CHARINFO:
                {
                    _showCurrentPanel = ShowUI("CharInfoPanel", true);
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.CHAR);
                }
                break;
            case ePANELTYPE.GACHA:
                {
                    _showCurrentPanel = ShowUI("GachaPanel", true);
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.GACHA);
                }
                break;
            case ePANELTYPE.ITEM:
                {
                    _showCurrentPanel = ShowUI("ItemPanel", true);
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.ITEM);
                }
                break;
            case ePANELTYPE.STORE:
                {
                    _showCurrentPanel = ShowUI("StorePanel", true);
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.STORE);
                }
                break;
            case ePANELTYPE.FACILITY:
                {
                    _showCurrentPanel = ShowUI("FacilityPanel", true);
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.FACILITY_IN);
                }
                break;
            case ePANELTYPE.FACILITYITEM:
                {
                    _showCurrentPanel = ShowUI("FacilityItemPanel", true);
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.FACILITY_IN);
                }
                break;
            case ePANELTYPE.FIGUREROOM:
                {
                    _showCurrentPanel = ShowUI("FigureRoomPanel", true);
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.ROOM);
                }
                break;
            case ePANELTYPE.FIGUREROOM_FREELOOK:
                {
                    _toppanel.OnClose();
                    _showCurrentPanel = ShowUI("FigureRoomFreeLookPanel", true);
                }
                break;
            case ePANELTYPE.FIGUREROOM_EDITMODE:
                {
                    _toppanel.OnClose();
                    _showCurrentPanel = ShowUI("FigureRoomEditModePanel", true);
                }
                break;
            case ePANELTYPE.FIGUREROOM_SAVELOAD:
                {
                    _toppanel.OnClose();
                    _showCurrentPanel = ShowUI("FigureRoomSaveLoadPanel", true);
                }
                break;
            case ePANELTYPE.STORYMAIN:
                {
                    _showCurrentPanel = ShowUI("StoryMainPanel", true);
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.STAGE);
                }
                break;
            case ePANELTYPE.STORY:
                {
                    _showCurrentPanel = ShowUI("StoryPanel", true);
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.STAGE);
                }
                break;
            case ePANELTYPE.DAILY:
                {
                    _showCurrentPanel = ShowUI("DailyPanel", true);
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.STAGE);
                }
                break;
            case ePANELTYPE.TIMEATTACK:
                {
                    _showCurrentPanel = ShowUI("TimeAttackPanel", true);
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.STAGE);
                }
                break;
            case ePANELTYPE.EVENT_STORY_MAIN:
                {
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.STAGE);
                    _showCurrentPanel = ShowUI("EventmodeStoryMainPanel", true);
                }
                break;
            case ePANELTYPE.EVENT_STORY_GACHA:
                {
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.STAGE);
                    _showCurrentPanel = ShowUI("EventmodeStoryResetGachaPanel", true);
                }
                break;
            case ePANELTYPE.EVENT_STORY_STAGE:
                {
                    _showCurrentPanel = ShowUI("EventmodeStoryStagePanel", true);
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.STAGE);
                }
                break;
            case ePANELTYPE.EVENT_CHANGE_MAIN:
                {
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.STAGE);
                    _showCurrentPanel = ShowUI("EventmodeExchangeMainPanel", true);
                }
                break;
            case ePANELTYPE.EVENT_CHANGE_STAGE:
                {
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.STAGE);
                    _showCurrentPanel = ShowUI("EventmodeExchangeStagePanel", true);
                }
                break;
            case ePANELTYPE.EVENT_CHANGE_REWARD:
                {
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.STAGE);
                    _showCurrentPanel = ShowUI("EventmodeExchangePanel", true);
                }
                break;
            case ePANELTYPE.EVENT_MISSION:
                {

                }
                break;
            case ePANELTYPE.ARENA:
                {
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.STAGE);
                    _showCurrentPanel = ShowUI("ArenaPanel", true);
                }
                break;
            case ePANELTYPE.ARENA_TITLE:
                {
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.STAGE);
                    _showCurrentPanel = ShowUI("ArenaTitlePanel", true);
                }
                break;
            case ePANELTYPE.ARENA_MAIN:
                {
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.STAGE);
                    _showCurrentPanel = ShowUI("ArenaMainPanel", true);
                }
                break;
            case ePANELTYPE.CARD_DISPATCH:
                {
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.NORMAL);
                    _showCurrentPanel = ShowUI("CardDispatchPanel", true);
                }
                break;
            case ePANELTYPE.ARENATOWER:
                {
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.ARENA_TOWER);
                    _showCurrentPanel = ShowUI("ArenaTowerMainPanel", true);                    
                }
                break;
            case ePANELTYPE.ARENATOWER_STAGE:
                {
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.ARENA_TOWER);
                    _showCurrentPanel = ShowUI("ArenaTowerStagePanel", true);
                }
                break;
            case ePANELTYPE.EVENT_BOARD_MAIN:
                {
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.STAGE);
                    _showCurrentPanel = ShowUI("EventBoardMainPanel", true);
                }
                break;

            case ePANELTYPE.RAID_MAIN: {
				_toppanel.SetTopStatePlay( UITopPanel.eTOPSTATE.STAGE );
				_showCurrentPanel = ShowUI( "RaidMainPanel", true );
            }
            break;
                
            case ePANELTYPE.RAID: {
                _showCurrentPanel = ShowUI( "RaidPanel", true );
                _toppanel.SetTopStatePlay( UITopPanel.eTOPSTATE.STAGE );
            }
            break;
            case ePANELTYPE.CircleJoin:
                {
                    _showCurrentPanel = ShowUI("CircleJoinPanel", true);
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.NORMAL);
                } break;
            case ePANELTYPE.CircleLobby:
                {
                    _showCurrentPanel = ShowUI("CircleLobbyPanel", true);
                    _toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.NORMAL);
                } break;
        }

        if (_showCurrentPanel != null)
        {
            if (_showCurrentPanel.kBGIndex == -1)
            {
                PanelBGAllHide();
            }
            else
            {
                if (_panelbgindex != -1 && _panelbgindex != _showCurrentPanel.kBGIndex)
                {
                    var panelbgclose = GetPanelBG(_panelbgindex);
                    if (panelbgclose != null)
                        panelbgclose.GetComponent<UIBGUnit>().SetUIActive(false);
                }

                PanelBGShow(_showCurrentPanel.kBGIndex);
            }
            
        }
    }

    public void PanelBGAllHide()
    {
        for (int i = 0; i < PanelBGList.Count; i++)
        {
            if (PanelBGList[i].activeSelf == true)
                PanelBGList[i].GetComponent<UIBGUnit>().SetUIActive(false);
        }
    }

    public void PanelBGShowAll()
    {
        for (int i = 0; i < PanelBGList.Count; i++)
        {
            if (PanelBGList[i].activeSelf == true)
                PanelBGList[i].GetComponent<UIBGUnit>().SetUIActive(true);
        }
    }

	private void PanelBGShow( int index ) {
		GameObject panelbg = GetPanelBG( index );
		if ( panelbg != null && panelbg.activeSelf == false ) {
			panelbg.GetComponent<UIBGUnit>().SetUIActive( true );
		}
	}

	public override void ShowComponent(FComponent comp)
    {
        if (comp == null)
            return;
        if (comp.Type != FComponent.TYPE.Popup)
            return;
        if (comp.kPopupGoodsType == ePOPUPGOODSTYPE.EXCEPT)
            return;

        if (_goodspopup == null)
            _goodspopup = GetUI<UIGoodsPopup>("GoodsPopup");

        var check = _activepopuplist.Find(x => x == comp);
        if(check == null)
            _activepopuplist.Add(comp);

        if (comp.kPopupGoodsType != ePOPUPGOODSTYPE.NONE)
        {
            Log.Show(comp.GetPanelDepth(), Log.ColorType.Red);
            _goodspopup.ShowGoodsStatePlay(comp.kPopupGoodsType, comp.GetPanelDepth() + 5);
        }
        else
        {
            _goodspopup.HideGoodsStatePlay();
        }

        Lobby.Instance.SetShowLobbyPlayer();
    }

    public override void HideComponent(FComponent comp)
    {
        if (comp == null)
            return;
        if (comp.Type != FComponent.TYPE.Popup)
            return;
        if (comp.kPopupGoodsType == ePOPUPGOODSTYPE.EXCEPT)
            return;

        if (_goodspopup == null)
            _goodspopup = GetUI<UIGoodsPopup>("GoodsPopup");

        _activepopuplist.Remove(comp);
        //if( _activepopuplist.Count != 0 )
        //    _activepopuplist.Sort(FComponent.CompareFuncPanelDepth);
    
        if (_activepopuplist.Count != 0)
        {
            if (_activepopuplist[_activepopuplist.Count - 1].kPopupGoodsType != ePOPUPGOODSTYPE.NONE)
            {
                FComponent last = _activepopuplist.LastOrDefault();
                if (last != null)
                {
                    _goodspopup.ShowGoodsStatePlay(last.kPopupGoodsType, last.GetPanelDepth() + 10);
                }
            }
            else
            {
                _goodspopup.HideGoodsStatePlay();
            }
        }
        else
        {
            _goodspopup.HideGoodsStatePlay();
            
        }
   
       
        if ( _activepopuplist.Count == 0 )
        {
            if( _paneltype == ePANELTYPE.MAIN )
            {
                NotificationManager.Instance.Init();
                Renewal( "MainPanel" );

                // Hide 하는데 초기화는 왜 해주는거지???? (결함 #2709관련)
                //InitComponent("MainPanel");
            }
        }

        //Lobby.Instance.SetShowLobbyPlayer();
    }

    public override void OnUIOpen(FComponent comp)
    {
        Lobby.Instance.SetShowLobbyPlayer();
    }
    public override void OnUIClose(FComponent comp)
    {
        Lobby.Instance.SetShowLobbyPlayer();
    }

    /// <summary>
    ///  lateUpdate에서 escape 입력시 진행
    /// </summary>
    public override void OnEscape()
    {
        if ( mbLockEscape ) {
            return;
		}

        mbLockEscape = true;
        Invoke( "ReleaseLockEscape", 0.4f );

        base.OnEscape();

        if (GameSupport.IsInLobbyTutorial())
            return;

        if (GetActiveUI("TutorialPopup") != null)// || GetActiveUI("BookCardCinemaPopup") != null)
            return;

        if(Director.IsPlaying)
        {
            Log.Show(Director.IsPlaying, Log.ColorType.Red);
            return;
        }

        if(GetActiveUI("WaitPopup") != null || GetActiveUI("LoadingPopup") != null)
        {
            return;
        }

        UICharFavorLevelUpPopup favorLevelUpPopup = GetActiveUI("CharFavorLevelUpPopup") as UICharFavorLevelUpPopup;
        if (favorLevelUpPopup != null)
        {
            favorLevelUpPopup.ExitLevelUpPopup();
            return;
        }
        
        UICharGiveGiftPopup charGiveGiftPopup = GetActiveUI("CharGiveGiftPopup") as UICharGiveGiftPopup;
        if (charGiveGiftPopup != null && GetActiveUI("MessageToastPopup") == null)
        {
            charGiveGiftPopup.FavorExit();
            return;
        }

        UIItemInfoPopup itemInfoPopup = GetActiveUI<UIItemInfoPopup>("ItemInfoPopup");
        if (itemInfoPopup != null)
        {
            itemInfoPopup.OnClickClose();
            return;
        }

		UIEventmodeStoryAutoGachaPopup eventmodeStoryAutoGachaPopup = GetActiveUI( "EventmodeStoryAutoGachaPopup" ) as UIEventmodeStoryAutoGachaPopup;
		if ( eventmodeStoryAutoGachaPopup != null ) {
			eventmodeStoryAutoGachaPopup.OnClickClose();
			return;
        }

        //  해당 활성화된 컴포넌트 확인(팝업만)
        var arr = GetComponentsInChildren<FComponent>();
        List<FComponent> list = new List<FComponent>();
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i].Type == FComponent.TYPE.Popup && !arr[i].name.Equals("GoodsPopup"))
                list.Add(arr[i]);
        }

        //  앱종료 메세지 팝업이 활성화 또는 UI스택이 하나이상인경우
        if (list.Count > 0)
        {
            FComponent messagepopup = GetActiveUI("MessagePopup");
            if(messagepopup != null)
            {
                if (FSaveData.Instance.IsContinueStage() || 0 < GameInfo.Instance.UserData.DyeingCostumeId)
                {
                    UIMessagePopup popupMsg = messagepopup.GetComponent<UIMessagePopup>();
                    if(popupMsg)
                    {
                        popupMsg.OnClickNo();
                    }
                }
                else
                {
                    //메세지 팝업이 가장 최상단에 뜨기때문에 열려있으면 우선적으로 닫아주기.
                    messagepopup.OnClickClose();
                }
            }
            else
            {
                //int finalPopup = list.Count - 1;
                ////  해당 팝업을 닫기가 가능한지 확인
                //FComponent ui = list[finalPopup];
                //if (ui && !ui.UIAni.isPlaying && ui.IsBackButton() == true)
                //{
                //    //HideUI(ui);
                //    Log.Show("@@@Close UI : " + ui.name, Log.ColorType.Red);
                //    ui.OnClickClose();
                //}

                //UI Depth가 가장 높은거 우선으로 끄기
                int popupDepth = -1;
                int popupIdx = -1;
                for(int i = 0; i < list.Count; i++)
                {
                    if(popupDepth < list[i].GetPanelDepth())
                    {
                        popupDepth = list[i].GetPanelDepth();
                        popupIdx = i;
                    }
                }

				FComponent ui = list[popupIdx];
				if( ui && !ui.UIAni.isPlaying ) {
					UIStroyPopup popupStory = ui.GetComponent<UIStroyPopup>();
					if( popupStory ) {
						UIGameOffPopup popupOff = GetActiveUI<UIGameOffPopup>("GameOffPopup");
						if( !popupOff ) {
							popupStory.OnClick_ExitBtn();
						}
					}
					else if( ui.IsBackButton() ) {
						Log.Show( "@@@Close UI : " + ui.name, Log.ColorType.Red );
						ui.OnClickClose();
					}
				}
			}
        }
        else
        {
            if(_showCurrentPanel && _showCurrentPanel.UIAni && _showCurrentPanel.UIAni.isPlaying)
            {
                return;
            }

            BackBtnEvent();
            return;
        }
    }

    private void ReleaseLockEscape() {
        mbLockEscape = false;
	}

    public void HomeBtnEvent()
    {
        if (Director.IsPlaying && !GameSupport.IsTutorial())
        {
            Log.Show(Director.IsPlaying, Log.ColorType.Red);
            return;
        }

        if (GetActiveUI("WaitPopup") != null || GetActiveUI("LoadingPopup") != null)
        {
            return;
        }

        UICostumeDyePopup costumeDyePopup = GetActiveUI("CostumeDyePopup") as UICostumeDyePopup;
        if (costumeDyePopup != null && GetActiveUI("MessagePopup") == null && GetActiveUI("ItemBuyMessagePopup") == null)
        {
            if(costumeDyePopup.kCurtainObj.activeSelf)
                return;
        }

        UICharFavorLevelUpPopup favorLevelUpPopup = GetActiveUI("CharFavorLevelUpPopup") as UICharFavorLevelUpPopup;
        if (favorLevelUpPopup != null)
        {
            favorLevelUpPopup.ExitLevelUpPopup();
            return;
        }

        UICharGiveGiftPopup charGiveGiftPopup = GetActiveUI("CharGiveGiftPopup") as UICharGiveGiftPopup;
        if (charGiveGiftPopup != null)
        {
            if (charGiveGiftPopup.FavorExit() == false)
                return;
        }
             
        if (_showCurrentPanel && _showCurrentPanel.UIAni && _showCurrentPanel.UIAni.isPlaying)
        {
            return;
        }

        UIGemOptChangePopup gemOptChangePopup = GetActiveUI( "GemOptChangePopup" ) as UIGemOptChangePopup;
		if ( gemOptChangePopup != null ) {
			if ( gemOptChangePopup.IsAutoIng() ) {
				MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3367 ) );
				return;
			}
		}


        //각종 강화 시 홈 버튼 동작 블럭 처리
        UICardLevelUpPopup cardLevelUpPopup = GetActiveUI("CardLevelUpPopup") as UICardLevelUpPopup;
        if (cardLevelUpPopup != null) {
            if (cardLevelUpPopup.IsSendLevelUp) {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3367));
                return;
            }
        }

        UIGemLevelUpPopup gemLevelUpPopup = GetActiveUI("GemLevelUpPopup") as UIGemLevelUpPopup;
        if (gemLevelUpPopup != null) {
            if (gemLevelUpPopup.IsSendLevelUp) {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3367));
                return;
            }
        }

        UIWeaponLevelUpPopup weaponLevelUpPopup = GetActiveUI("WeaponLevelUpPopup") as UIWeaponLevelUpPopup;
        if (weaponLevelUpPopup != null) {
            if (weaponLevelUpPopup.IsSendLevelUp) {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3367));
                return;
            }
        }

        UIBadgeLevelUpPopup badgeLevelUpPopup = GetActiveUI("BadgeLevelUpPopup") as UIBadgeLevelUpPopup;
        if (badgeLevelUpPopup != null) {
            if (badgeLevelUpPopup.IsSendLevelUp) {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3367));
                return;
            }
        }

        //UICharInfoPanel이 비밀 임무 에서 눌렀다면 선택된 리스트를 초기화 해준다.
        UICharInfoPanel charInfoPanel = GetActiveUI("CharInfoPanel") as UICharInfoPanel;
        if (charInfoPanel != null) {

            UICharSeletePopup charSelectPopup = GetUI("CharSeletePopup") as UICharSeletePopup;
            if (charSelectPopup != null) {
                var value = UIValue.Instance.GetValue(UIValue.EParamType.CharSeletePopupType);
                if (value != null) {
                    int _type = (int)value;
                    if (_type == (int)eCharSelectFlag.SECRET_QUEST) {
                        charSelectPopup.ClearAllSelect();
                    }
                }
            }
        }

        StartCoroutine(HomeBtnPanelEvent());
    }

    private IEnumerator HomeBtnPanelEvent()
    {
        FComponent[] arr = GetComponentsInChildren<FComponent>();
        List<FComponent> list = new List<FComponent>();
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i].Type == FComponent.TYPE.Popup && !arr[i].name.Equals("GoodsPopup"))
                list.Add(arr[i]);
        }


        //  앱종료 메세지 팝업이 활성화 또는 UI스택이 하나이상인경우
        if (list.Count > 0)
        {
            list.Sort(HomeBtnEventSort);

            for (int i = 0; i < list.Count; i++)
            {
                FComponent ui = list[i];

                if (ui && ui.IsBackButton() == true && !ui.UIAni.isPlaying)
                {
                    Log.Show("@@@Close UI : " + ui.name, Log.ColorType.Red);
                    ui.OnClickClose();
                    while (ui.UIAni.isPlaying)
                        yield return null;
                }
                else
                {
                    yield return null;
                    continue;
                }


                UIMessagePopup messagepopup = GetActiveUI<UIMessagePopup>("MessagePopup");
                if (messagepopup != null)
                {
                    while (messagepopup.MsgResultType == UIMessagePopup.eMsgResultType.NONE)
                        yield return null;

                    if (_paneltype != ePANELTYPE.FIGUREROOM)
                    {
                        if (messagepopup.MsgResultType == UIMessagePopup.eMsgResultType.OK || messagepopup.MsgResultType == UIMessagePopup.eMsgResultType.NO || messagepopup.MsgResultType == UIMessagePopup.eMsgResultType.CANCEL || messagepopup.MsgResultType == UIMessagePopup.eMsgResultType.CLOSE)
                            yield break;
                    }
                }

                yield return null;
            }
        }

        StorePanelUpdateSlotList = true;
        StorePanelLocalPosY = 0;
        
        switch (_paneltype)
        {
            case ePANELTYPE.MAIN:
                break;
            case ePANELTYPE.CHARINFO:
                {
                    UIValue.Instance.RemoveValue(UIValue.EParamType.ArenaCharInfoFlag);
                    UIValue.Instance.RemoveValue(UIValue.EParamType.CharCostumeID);
                    SetPanelType(ePANELTYPE.MAIN);
                }
                break;
            case ePANELTYPE.FACILITY:
            case ePANELTYPE.FACILITYITEM:
                {
                    if (!Lobby.Instance.FacilityUpgreadFlag())
                        yield break;

                    if (PanelType == ePANELTYPE.FACILITY) HideUI("FacilityPanel");
                    else HideUI("FacilityItemPanel");

                    LobbyDoorPopup.Show(_toppanel.DoorToLobby);
                }
                break;
            case ePANELTYPE.FIGUREROOM:
                {
                    GetActiveUI<UIFigureRoomPanel>("FigureRoomPanel").OnBtnBackToLobby();
                }
                break;
            case ePANELTYPE.FIGUREROOM_EDITMODE:
                {
                    GetActiveUI<UIFigureRoomEditModePanel>("FigureRoomEditModePanel").OnBtnBack(true);
                }
                break;
            case ePANELTYPE.FIGUREROOM_FREELOOK:
                {
                    GetActiveUI<UIFigureRoomFreeLookPanel>("FigureRoomFreeLookPanel").OnBtnBack(true);
                }
                break;
            case ePANELTYPE.GACHA:
                {
                    UIGachaPanel ui = GetActiveUI<UIGachaPanel>("GachaPanel");
                    if (ui.IsBackButton())
                        SetPanelType(ePANELTYPE.MAIN);
                }
                break;
            case ePANELTYPE.STORE:
            case ePANELTYPE.STORY:
            case ePANELTYPE.DAILY:
            case ePANELTYPE.TIMEATTACK:
            case ePANELTYPE.EVENT_STORY_MAIN:
            case ePANELTYPE.EVENT_STORY_GACHA:
            case ePANELTYPE.EVENT_STORY_STAGE:
            case ePANELTYPE.EVENT_CHANGE_MAIN:
            case ePANELTYPE.EVENT_CHANGE_REWARD:
            case ePANELTYPE.EVENT_CHANGE_STAGE:
            case ePANELTYPE.FIGUREROOM_SAVELOAD:
            case ePANELTYPE.ARENA:
            case ePANELTYPE.ARENA_TITLE:
            case ePANELTYPE.ARENA_MAIN:
            case ePANELTYPE.ARENATOWER:
            case ePANELTYPE.ARENATOWER_STAGE:
                {
                    SetPanelType(ePANELTYPE.MAIN);
                }
                break;
            default:
                SetPanelType(ePANELTYPE.MAIN);
                break;
        }

        //while (_paneltype != ePANELTYPE.MAIN || list.Count > (int)eCOUNT.NONE)
        //{
        //    //  앱종료 메세지 팝업이 활성화 또는 UI스택이 하나이상인경우
        //    if (list.Count > 0)
        //    {
        //        list.Sort(HomeBtnEventSort);

        //        for (int i = 0; i < list.Count; i++)
        //        {
        //            FComponent ui = list[i];

        //            if (ui && ui.IsBackButton() == true && !ui.UIAni.isPlaying)
        //            {
        //                Log.Show("@@@Close UI : " + ui.name, Log.ColorType.Red);
        //                ui.OnClickClose();
        //                while(ui.UIAni.isPlaying)
        //                    yield return null;
        //            }
        //            else
        //            {
        //                yield return null;
        //                continue;
        //            }


        //            UIMessagePopup messagepopup = GetActiveUI<UIMessagePopup>("MessagePopup");
        //            if (messagepopup != null)
        //            {
        //                while (messagepopup.MsgResultType == UIMessagePopup.eMsgResultType.NONE)
        //                    yield return null;

        //                if (_paneltype != ePANELTYPE.FIGUREROOM)
        //                {
        //                    if (messagepopup.MsgResultType == UIMessagePopup.eMsgResultType.NO || messagepopup.MsgResultType == UIMessagePopup.eMsgResultType.CANCEL || messagepopup.MsgResultType == UIMessagePopup.eMsgResultType.CLOSE)
        //                        yield break;
        //                }
        //            }

        //            yield return null;
        //        }
        //    }

        //    arr = GetComponentsInChildren<FComponent>();
        //    list.Clear();
        //    for (int i = 0; i < arr.Length; i++)
        //    {
        //        if (arr[i].Type == FComponent.TYPE.Popup && !arr[i].name.Equals("GoodsPopup"))
        //            list.Add(arr[i]);
        //    }

        //    if (list.Count > (int)eCOUNT.NONE)
        //    {
        //        yield return null;
        //        continue;
        //    }


        //    yield return null;

        //    UIMessagePopup mpopup = GetActiveUI<UIMessagePopup>("MessagePopup");
        //    if (mpopup != null)
        //    {
        //        while (mpopup.MsgResultType == UIMessagePopup.eMsgResultType.NONE)
        //            yield return null;

        //        if (_paneltype != ePANELTYPE.FIGUREROOM)
        //        {
        //            if (mpopup.MsgResultType == UIMessagePopup.eMsgResultType.NO || mpopup.MsgResultType == UIMessagePopup.eMsgResultType.CANCEL || mpopup.MsgResultType == UIMessagePopup.eMsgResultType.CLOSE)
        //                yield break;
        //        }

        //        yield return null;
        //        continue;
        //    }

        //    FComponent lobbydoorpopup = GetActiveUI("LobbyDoorPopup");
        //    if (lobbydoorpopup != null)
        //        break;

        //    BackBtnEvent(false);
        //    if (_showCurrentPanel && _showCurrentPanel.UIAni && _showCurrentPanel.UIAni.isPlaying)
        //    {
        //        yield return null;
        //    }
        //    //yield return null;

        //    arr = GetComponentsInChildren<FComponent>();
        //    list.Clear();
        //    for (int i = 0; i < arr.Length; i++)
        //    {
        //        if (arr[i].Type == FComponent.TYPE.Popup && !arr[i].name.Equals("GoodsPopup"))
        //            list.Add(arr[i]);
        //    }
        //}

        //StartCoroutine(HomeBtnEventHidePopup(true));
    }

    IEnumerator HomeBtnEventHidePopup()
    {
        FComponent[] arr = GetComponentsInChildren<FComponent>();
        List<FComponent> list = new List<FComponent>();
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i].Type == FComponent.TYPE.Popup && !arr[i].name.Equals("GoodsPopup"))
                list.Add(arr[i]);
        }

        //  앱종료 메세지 팝업이 활성화 또는 UI스택이 하나이상인경우
        if (list.Count > 0)
        {
            list.Sort(HomeBtnEventSort);

            for (int i = 0; i < list.Count; i++)
            {
                FComponent ui = list[i];

                if (ui && ui.IsBackButton() == true && !ui.UIAni.isPlaying)
                {
                    Log.Show("@@@Close UI : " + ui.name, Log.ColorType.Red);
                    ui.OnClickClose();
                }
                else
                {
                    yield return null;
                    continue;
                }


                UIMessagePopup messagepopup = GetActiveUI<UIMessagePopup>("MessagePopup");
                if (messagepopup != null)
                {
                    while (messagepopup.MsgResultType == UIMessagePopup.eMsgResultType.NONE)
                        yield return null;

                    if (_paneltype != ePANELTYPE.FIGUREROOM)
                    {
                        if (messagepopup.MsgResultType == UIMessagePopup.eMsgResultType.NO || messagepopup.MsgResultType == UIMessagePopup.eMsgResultType.CANCEL || messagepopup.MsgResultType == UIMessagePopup.eMsgResultType.CLOSE)
                            yield break;
                    }
                }

                yield return null;
            }
        }

        yield return null;
    }


    private int HomeBtnEventSort(FComponent data1, FComponent data2)
    {
        return data1.GetPanelDepth().CompareTo(data2.GetPanelDepth());
    }

    /// <summary>
    ///  로비 백버튼 처리
    /// </summary>
    public void BackBtnEvent(bool isEndCheck = true)
    {
        switch (_paneltype)
        {
            case ePANELTYPE.MAIN:
                {
                    UIMainPanel ui = GetActiveUI<UIMainPanel>("MainPanel");
                    if (ui != null)
                    {
                        if (ui.IsBackButton() == true)
                        {
                            if (isEndCheck)
                            {
                                MessagePopup.CYN(eTEXTID.TITLE_NOTICE,
                                             3046,
                                             eTEXTID.OK,
                                             eTEXTID.CANCEL,
                                             () => { Application.Quit(); },
                                             () => { HideUI("MessagePopup"); });
                            }
                        }
                    }
                    
                }
                break;
            case ePANELTYPE.CHARINFO:
                {
                    UIValue.Instance.RemoveValue(UIValue.EParamType.CharCostumeID);
                    if (JoinStageID == -1)
                    {
                        var arenaFlag = UIValue.Instance.GetValue(UIValue.EParamType.ArenaCharInfoFlag, true);
                        if(arenaFlag == null)
                        {
                            if (PrepanelType == ePANELTYPE.STORE)
                                SetPanelType(ePANELTYPE.STORE);
                            else if( PrepanelType == ePANELTYPE.RAID ) {
                                SetPanelType( ePANELTYPE.RAID );
                                ShowUI( "RaidDetailPopup", true );
                            }
                            else
                                SetPanelType(ePANELTYPE.CHARMAIN);
                        }
                        else
                        {
                            int flag = (int)arenaFlag;
                            if(flag == (int)eArenaToCharInfoFlag.NONE)
                                SetPanelType(ePANELTYPE.CHARMAIN);
                            else if(flag == (int)eArenaToCharInfoFlag.ARENA_MAIN)
                            {
                                if (GameSupport.ArenaPlayFlag() == eArenaState.PLAYING)
                                    SetPanelType(ePANELTYPE.ARENA_MAIN);
                                else
                                    SetPanelType(ePANELTYPE.ARENA_TITLE);
                            }
                            else if(flag == (int)eArenaToCharInfoFlag.ARENA_ENEMY_SEARCH)
                            {
                                if(GameSupport.ArenaPlayFlag() == eArenaState.PLAYING)
                                {
                                    SetPanelType(ePANELTYPE.ARENA_MAIN);
                                    ShowUI("ArenaBattleConfirmPopup", true);
                                }
                                else
                                    SetPanelType(ePANELTYPE.ARENA_TITLE);
                            }
                            else if(flag == (int)eArenaToCharInfoFlag.FRIEND_BATTLE)
                            {
                                SetPanelType(ePANELTYPE.MAIN);
                                ShowUI("ArenaBattleConfirmPopup", true);
                            }
                            else if (flag == (int)eArenaToCharInfoFlag.ARENATOWER)
                            {
                                SetPanelType(ePANELTYPE.ARENATOWER);
                            }
                            else if (flag == (int)eArenaToCharInfoFlag.ARENATOWER_STAGE)
                            {
                                SetPanelType(ePANELTYPE.ARENATOWER_STAGE);
                            }
                        }                       
                    }
                    else
                    {
                        var stagedata = GameInfo.Instance.GameTable.FindStage(JoinStageID);
                        bool isRaid = false;

                        if (stagedata != null)
                        {
                            if (stagedata.StageType == (int)eSTAGETYPE.STAGE_MAIN_STORY)
                            {
                                SetPanelType(ePANELTYPE.STORY);
                            }
                            else if (stagedata.StageType == (int)eSTAGETYPE.STAGE_DAILY || stagedata.StageType == (int)eSTAGETYPE.STAGE_SECRET)
                            {
                                SetPanelType(ePANELTYPE.DAILY);
                            }
                            else if (stagedata.StageType == (int)eSTAGETYPE.STAGE_TIMEATTACK)
                            {
                                SetPanelType(ePANELTYPE.TIMEATTACK);
                            }
						    else if( stagedata.StageType == (int)eSTAGETYPE.STAGE_EVENT || stagedata.StageType == (int)eSTAGETYPE.STAGE_EVENT_BONUS ) {
							    //스테이지 TypeValue로 이벤트 테이블 참조, 리셋가챠형/교환형/미션형 을 판단

							    GameTable.EventSet.Param eventTableData = GameInfo.Instance.GameTable.FindEventSet(x => x.EventID == stagedata.TypeValue);

							    if( eventTableData != null ) {
								    if( eventTableData.EventType == (int)eEventRewardKind.RESET_LOTTERY )
									    SetPanelType( ePANELTYPE.EVENT_STORY_STAGE );
								    else if( eventTableData.EventType == (int)eEventRewardKind.EXCHANGE )
									    SetPanelType( ePANELTYPE.EVENT_CHANGE_STAGE );
								    else if( eventTableData.EventType == (int)eEventRewardKind.MISSION ) {
									    //추후 구현 예정
								    }
							    }

							    //SetPanelType(ePANELTYPE.EVENT_STORY_STAGE);
						    }
						    else if(stagedata.StageType == (int)eSTAGETYPE.STAGE_PVP_PROLOGUE)
                            {
                                SetPanelType(ePANELTYPE.EVENT_STORY_STAGE);
                            }
                            else if( stagedata.StageType == (int)eSTAGETYPE.STAGE_RAID ) {
                                SetPanelType( ePANELTYPE.RAID, true, false );
                                
                                isRaid = true;
                                ShowUI( "RaidDetailPopup", true );
                            }

                            UIValue.Instance.SetValue( UIValue.EParamType.StageID, JoinStageID );

                            if( !isRaid ) {
                                ShowUI( "StageDetailPopup", true );
                            }

                            if (JoinCharSeleteIndex != -1)
                            {
                                if( isRaid ) {
                                    UIValue.Instance.SetValue( UIValue.EParamType.CharSeletePopupType, (int)eCharSelectFlag.RAID );
                                }
                                else {
                                    if( stagedata.PlayerMode == 1 ) {
                                        UIValue.Instance.SetValue( UIValue.EParamType.CharSeletePopupType, (int)eCharSelectFlag.RAID_PROLOGUE );
                                    }
                                    else if( stagedata.StageType != (int)eSTAGETYPE.STAGE_SECRET ) {
                                        UIValue.Instance.SetValue( UIValue.EParamType.CharSeletePopupType, (int)eCharSelectFlag.STAGE );
                                    }
                                }

                                ShowUI("CharSeletePopup", true);
                            }                            
                        }
                        else
                        {
                            SetPanelType(ePANELTYPE.CHARMAIN);
                        }

                        JoinStageID = -1;
                        JoinCharSeleteIndex = -1;
                    }
                }
                break;
            case ePANELTYPE.STORE:
                {
                    if (GetActiveUI<UIStorePanel>("StorePanel").TopBackBtnClick())
                        SetPanelType(ePANELTYPE.MAIN);
                }
                break;
            case ePANELTYPE.FACILITY:
            case ePANELTYPE.FACILITYITEM:
                {
                    if (!Lobby.Instance.FacilityUpgreadFlag())
                        return;

                    if (PanelType == ePANELTYPE.FACILITY) HideUI("FacilityPanel");
                    else HideUI("FacilityItemPanel");

                    LobbyDoorPopup.Show(_toppanel.DoorToLobby);
                }
                break;
            case ePANELTYPE.STORY:
            case ePANELTYPE.DAILY:
            case ePANELTYPE.TIMEATTACK:
                {
                    SetPanelType(ePANELTYPE.STORYMAIN);
                }
                break;
            case ePANELTYPE.EVENT_STORY_MAIN:
                {
                    if (_panelTypeRemamberStart == ePANELTYPE.NULL)
                        SetPanelType(ePANELTYPE.STORYMAIN);
                    else
                    {
                        SetPanelType(_panelTypeRemamberStart);
                        _panelTypeRemamberStart = ePANELTYPE.NULL;
                    }
                }
                break;
            case ePANELTYPE.EVENT_STORY_GACHA:
                {
            #if GACHA_MACRO_ADD
				    if ( GetActiveUI<UIEventmodeStoryResetGachaPanel>( "EventmodeStoryResetGachaPanel" ).TopBackBtnClick() ) {
					    SetPanelType( ePANELTYPE.EVENT_STORY_MAIN );
				    }
            #else
				    SetPanelType( ePANELTYPE.EVENT_STORY_MAIN );
            #endif
			    }
                break;
            case ePANELTYPE.EVENT_STORY_STAGE:
                {
                    var popupType = UIValue.Instance.GetValue(UIValue.EParamType.EventStagePopupType);
                    if (popupType == null)
                        return;

                    if((int)popupType == (int)eEventStageType.EVENT)
                        SetPanelType(ePANELTYPE.EVENT_STORY_MAIN);
                    else if((int)popupType == (int)eEventStageType.ARENA_PROLOGUE)
                        SetPanelType(ePANELTYPE.ARENA);
                }
                break;
            case ePANELTYPE.EVENT_CHANGE_MAIN:
                {
                    if (_panelTypeRemamberStart == ePANELTYPE.NULL)
                        SetPanelType(ePANELTYPE.STORYMAIN);
                    else
                    {
                        SetPanelType(_panelTypeRemamberStart);
                        _panelTypeRemamberStart = ePANELTYPE.NULL;
                    }
                }
                break;
            case ePANELTYPE.EVENT_CHANGE_REWARD:
            case ePANELTYPE.EVENT_CHANGE_STAGE:
                {
                    SetPanelType(ePANELTYPE.EVENT_CHANGE_MAIN);
                }
                break;
            case ePANELTYPE.FIGUREROOM:
                {
                    GetActiveUI<UIFigureRoomPanel>("FigureRoomPanel").OnBtnBackToLobby();
                }
                break;
            case ePANELTYPE.FIGUREROOM_EDITMODE:
                {
                    GetActiveUI<UIFigureRoomEditModePanel>("FigureRoomEditModePanel").OnBtnBack();
                }
                break;
            case ePANELTYPE.FIGUREROOM_FREELOOK:
                {
                    GetActiveUI<UIFigureRoomFreeLookPanel>("FigureRoomFreeLookPanel").OnBtnBack();
                }
                break;
            case ePANELTYPE.FIGUREROOM_SAVELOAD:
                {

                }
                break;
            case ePANELTYPE.ARENA:
                {
                    SetPanelType(ePANELTYPE.MAIN);
                }
                break;
            case ePANELTYPE.ARENA_TITLE:
                {
                    SetPanelType(ePANELTYPE.ARENA);
                }
                break;
            case ePANELTYPE.ARENA_MAIN:
                {
                    SetPanelType(ePANELTYPE.ARENA);
                }
                break;
            case ePANELTYPE.GACHA:
                {
                    UIGachaPanel ui = GetActiveUI<UIGachaPanel>("GachaPanel");
                    if(ui.IsBackButton())
                        SetPanelType(ePANELTYPE.MAIN);
                }
                break;
            case ePANELTYPE.ARENATOWER:
                {
                    SetPanelType(ePANELTYPE.ARENA);
                }
                break;
            case ePANELTYPE.ARENATOWER_STAGE:
                {
                    SetPanelType(ePANELTYPE.ARENATOWER);
                }
                break;
            case ePANELTYPE.RAID_MAIN:
                SetPanelType( ePANELTYPE.ARENA );
                break;

            case ePANELTYPE.RAID:
                SetPanelType( ePANELTYPE.RAID_MAIN );
                break;
            case ePANELTYPE.CircleLobby:
                {
                    UICircleLobbyPanel circleLobbyPanel = GetActiveUI<UICircleLobbyPanel>("CircleLobbyPanel");
                    if (circleLobbyPanel)
                    {
                        circleLobbyPanel.BackAction();
                    }
                }
                break;
            default:
                SetPanelType(ePANELTYPE.MAIN);
                break;
        }
    }
  
    public void BG_Stage(int _bgIndex, int _chapter, int _difficulty = 1)
    {
        GameObject panelBg = GetPanelBG(_bgIndex);
        if (panelBg == null)
            return;
        UIBGUnit BgUnit = panelBg.GetComponent<UIBGUnit>();

        GameClientTable.Chapter.Param paramChapter = GameInfo.Instance.GameClientTable.FindChapter(_chapter);
        Texture2D texBg = ResourceMgr.Instance.LoadFromAssetBundle("story", paramChapter.Bg) as Texture2D;
        BgUnit.SetBgTexture(texBg, _difficulty);
    }

    public void BG_Event(int _bgIndex, int eventid, int type, int _difficulty = 1)
    {
        GameObject panelBg = GetPanelBG(_bgIndex);
        if (panelBg == null)
            return;
        UIBGUnit BgUnit = panelBg.GetComponent<UIBGUnit>();

        EventSetData eventSetData = GameInfo.Instance.GetEventSetData(eventid);
        if (eventSetData == null)
            return;


        
        if (type == 0)
        {
            /*
            Texture2D texBg = ResourceMgr.Instance.LoadFromAssetBundle("ui", eventSetData.TableData.MainBG) as Texture2D;
            BgUnit.SetBgTexture(texBg, 1);
            */
        }
        else
        {
            BannerData bannerdataBG = GameInfo.Instance.ServerData.BannerList.Find(x => x.BannerType == (int)eBannerType.EVENT_STAGEBG && x.BannerTypeValue == eventSetData.TableID);
            if (bannerdataBG != null)
            {
                //Texture2D texBg = ResourceMgr.Instance.LoadFromAssetBundle("story", eventSetData.TableData.StageBG) as Texture2D;
                BgUnit.SetBgTexture(BannerManager.Instance.LoadTextureWithFileURL(bannerdataBG.UrlImage, true, bannerdataBG.Localizes[(int)eBannerLocalizeType.Url]), _difficulty);
            }
        }
    }

    public void BG_Arena(int _bgIndex, string bgFilePath, int _difficulty = 1)
    {
        GameObject panelBG = GetPanelBG(_bgIndex);
        if (panelBG == null)
            return;

        UIBGUnit BgUnit = panelBG.GetComponent<UIBGUnit>();

        Texture2D texBG = ResourceMgr.Instance.LoadFromAssetBundle("story", bgFilePath) as Texture2D;
        BgUnit.SetBgTexture(texBG, _difficulty);
    }

    public void SetUICameraMultiTouch(bool flag)
    {
        if (_uiCamera == null)
            return;

        _uiCamera.allowMultiTouch = flag;
    }

    public void ChangeFacility(string facilityEffectType)
    {
        FacilityData tempFacilityData = GameInfo.Instance.FacilityList.Find(x => x.TableData.EffectType == facilityEffectType);
        if (tempFacilityData == null)
        {
            Debug.LogError(facilityEffectType + " 타입의 시설이 존재하지 않습니다.");
            return;
        }



        HideUI("FacilityPanel");
        HideUI("FacilityItemPanel");

        UIValue.Instance.SetValue(UIValue.EParamType.FacilityID, tempFacilityData.TableData.ParentsID);

        LobbyDoorPopup.Show(
            () => {
                Lobby.Instance.MoveToFacility(tempFacilityData.TableData.ParentsID);
                if(tempFacilityData.TableData.EffectType.Equals("FAC_CHAR_EXP") || 
                   tempFacilityData.TableData.EffectType.Equals("FAC_CHAR_SP") ||
                   tempFacilityData.TableData.EffectType.Equals("FAC_CARD_TRADE"))
                {
                    SetPanelType(ePANELTYPE.FACILITY);
                    //_paneltype = ePANELTYPE.FACILITY;
                    //같은 패널일 경우 showui가 호출이 안되서 강제로 호출해준다..
                    ShowUI("FacilityPanel", true);
                }
                else if (tempFacilityData.TableData.EffectType.Equals("FAC_WEAPON_EXP") ||
                         tempFacilityData.TableData.EffectType.Equals("FAC_ITEM_COMBINE") ||
                         tempFacilityData.TableData.EffectType.Equals("FAC_OPERATION_ROOM"))
                {
                    SetPanelType(ePANELTYPE.FACILITYITEM);
                    //_paneltype = ePANELTYPE.FACILITYITEM;
                    ShowUI("FacilityItemPanel", true);
                }
            });
    }

    /// <summary>
    ///  로비 씬에서 바로 로그인 접속
    /// </summary>
    public void OnNetLogin(int result, PktMsgType pktmsg)
    {
        if(GameInfo.Instance.CharList.Count <= 0 && GameInfo.Instance.netFlag)
        {
            GameInfo.Instance.Send_ReqAddCharacter(1, OnSuccessLogin);
        }
        else
        {
            OnSuccessLogin(0, null);
        }
    }

    private void OnSuccessLogin(int result, PktMsgType pktmsg)
    {
        //if (GameInfo.Instance.GameConfig.Network)
        //NETStatic.PktGbl.ReqLogOnCreditKey();
        kBlackScene.SetActive(false);
        AppMgr.Instance.SetSceneType(AppMgr.eSceneType.Lobby);
        UIValue.Instance.SetValue(UIValue.EParamType.DeckSel, 0);
        Lobby.Instance.InitLobby();
        ShowUI("TopPanel", true); //LobbyUIManager.Instance.Renewal("TopPanel");
        SetPanelType(ePANELTYPE.MAIN);

        if (GameSupport.IsInLobbyTutorial())
            GameSupport.ShowTutorial();

        if (GameInfo.Instance.StageClearList.Count != 0)
            teststageid = GameInfo.Instance.StageClearList[GameInfo.Instance.StageClearList.Count - 1].TableID;

        NotificationManager.Instance.Init();
    }

    public void OnNetGameStart(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        int stageid = teststageid;
        //if (GameInfo.Instance.StageClearList.Count != 0)
        //    stageid = GameInfo.Instance.StageClearList[GameInfo.Instance.StageClearList.Count - 1].TableData.NextStage;
        //eSTAGETYPE
        var stagedata = GameInfo.Instance.GameTable.FindStage(stageid);

        if (!GameInfo.Instance.netFlag)
            GameInfo.Instance.MaxNormalBoxCount = UnityEngine.Random.Range(stagedata.N_DropMinCnt, stagedata.N_DropMaxCnt + 1);

        int box1 = GameInfo.Instance.MaxNormalBoxCount;
        GameInfo.Instance.Send_ReqStageEnd(stageid, GameInfo.Instance.CharList[0].CUID, 100, stagedata.RewardGold, box1, true, true, true, OnNetGameResult);
    }

    public void OnNetGameResult(int result, PktMsgType pktmsg)
    {
        teststageid += 1;

        OnPrepare();

        //ShowUI("BattleResultPopup", true);
        //Renewal("TopPanel");

        //  CheckRedDot(eRedDot_Contents.MailBox);
    }


    //로그인 팝업
    public void ShowDailyLoginPopup(bool bnotice = true)
    {
        if (_loginBonusCoroutine != null)
        {
            return;
        }
        
        _loginBonusCoroutine = StartCoroutine("LoginPopupFlag", bnotice);
    }

    public void StopDailyLoginPopup()
    {
        StopCoroutine(_loginBonusCoroutine);
        _loginBonusCoroutine = null;
    }

    IEnumerator LoginPopupFlag( bool bnotice )
    {
        UIDailyLoginBonusPopup dailyPopup = GetUI<UIDailyLoginBonusPopup>("DailyLoginBonusPopup");
        if (LoginBonusStep == eLoginBonusStep.Step01)
        {
            DailyLoginBonusPopup.OpenDailyLoginPopup();
            while (dailyPopup.gameObject.activeSelf)
            {
                yield return null;
            }
            
            ++LoginBonusStep;
        }

        if (LoginBonusStep == eLoginBonusStep.Step02)
        {
            List<GuerrillaMissionData> guerrillaMissionList = new List<GuerrillaMissionData>();
            guerrillaMissionList.Clear();

            GameSupport.GetGuerrillaMissionListWithTimeByType(ref guerrillaMissionList, "GM_LoginBonus");
            if (guerrillaMissionList.Count > 0)
            {
                foreach (GllaMissionData gmData in GameInfo.Instance.GllaMissionList)
                {
                    List<GuerrillaMissionData> loginDataList = guerrillaMissionList.FindAll(x => x.GroupID == gmData.GroupID);
                    if (loginDataList.Count <= 0 || (loginDataList.Count < gmData.Step && gmData.LoginBonusDisplayFlag == false))
                    {
                        continue;
                    }

                    DailyLoginBonusPopup.OpenGuerrillaLoginPopup(gmData.GroupID);
                    while (dailyPopup.gameObject.activeSelf)
                    {
                        yield return null;
                    }
                }
            }
            
            ++LoginBonusStep;
        }

		if ( LoginBonusStep == eLoginBonusStep.Step03 ) {
			UISpecialLoginBonusPopup specialLoginBonusPopup = ShowUI( "SpecialLoginBonusPopup", true ) as UISpecialLoginBonusPopup;
			if ( specialLoginBonusPopup != null ) {
				while ( specialLoginBonusPopup.gameObject.activeSelf ) {
					yield return null;
				}
			}

			++LoginBonusStep;
		}

		if (LoginBonusStep == eLoginBonusStep.Step04)
        {
            // 로그인 이벤트
            if (GameInfo.Instance.UserData.HasLoginEventInfo())
            {
                for (int i = GameInfo.Instance.UserData.ListLoginEventInfo.Count - 1; i >= 0; --i)
                {
                    DailyLoginBonusPopup.OpenLoginEventPopup(i);
                    while (dailyPopup.gameObject.activeSelf)
                    {
                        yield return null;
                    }
                }
            }
            
            ++LoginBonusStep;
        }

        if (LoginBonusStep == eLoginBonusStep.Step05)
        {
            bool isShowBannerPopup = false;
            bool isNotice = false;
            if (bnotice)
            {
                //스팀버전은 공지사항 자동으로 뜨는기능 제거
                if(AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Japan)
                {
                    if (!string.IsNullOrEmpty(GameInfo.Instance.GameConfig.WebNotice))
                    {
                        yield return new WaitForSeconds(0.2f);
                        GameSupport.OpenWebView(FLocalizeString.Instance.GetText(500), GameSupport.GetServiceTypeWebAddr(GameInfo.Instance.GameConfig.WebNotice));
                    }
                }
                else if(AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Global || AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam)
                {
                    isShowBannerPopup = true;
                    isNotice = true;
                }
            }
            else
            {
                if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Global || AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam)
                {
                    isShowBannerPopup = true;
                    isNotice = false;
                }
            }

            if (isShowBannerPopup)
            {
                if (GameInfo.Instance.UserData.ShowPkgPopup)
                {
                    UIMainPanel mainPanel = GetUI<UIMainPanel>("MainPanel");
                    if (mainPanel != null)
                    {
                        GameInfo.Instance.Send_ReqUserPkgShowOff(mainPanel.OnNetUserPkgShowOff);
                    }
                }
                else
                {
                    GameSupport.IsShowBannerPopup(isNotice);
                }
            }
            
            ++LoginBonusStep;
        }

        if (LoginBonusStep == eLoginBonusStep.Step06)
        {
            //인앱구매 내역이 남아있는지 확인.
            UIMainPanel mainPanel = GetUI<UIMainPanel>("MainPanel");
            mainPanel.ReceiptChackFlag();
            
            LoginBonusStep = eLoginBonusStep.End;
            IsOnceLoginBonusPopup = false;
            _loginBonusCoroutine = null;
        }
    }

    public override void HideAll(FComponent.TYPE e, bool bani = true)
    {
        if (e == FComponent.TYPE.Popup)
            _activepopuplist.Clear();

        base.HideAll(e, bani);
    }

    public void OnlyOneEnable(string componentName)
    {
        _activepopuplist.Clear();
        _showCurrentPanel = null;
        
        foreach(var keyValuePair in UIComponentDic)
        {
            var comp = keyValuePair.Value.GetComponent<FComponent>();
            if (comp != null)
            {
                if (keyValuePair.Key.Equals(componentName))
                {
                    if (comp != _toppanel)
                    {
                        _showCurrentPanel = comp;
                    }
                }
                else
                {
                    comp.SetUIActive(false);
                }
            }
        }
    }

    //타임어택
    public void OnNetTimeAtkRankingList(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        SetPanelType(ePANELTYPE.TIMEATTACK);
    }

    //아레나
    public void OnNetArenaRankingList(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        if (GameSupport.ArenaPlayFlag() == eArenaState.PLAYING)
        {
            SetPanelType(ePANELTYPE.ARENA_MAIN);
        }
        else
        {
            SetPanelType(ePANELTYPE.ARENA_TITLE);
        }

        if(GameInfo.Instance.bArenaLose && GameInfo.Instance.MatchTeam != null)
        {
            GameInfo.Instance.bArenaLose = false;
            ShowUI("ArenaLosePopup", true);
        }
    }
    
    private readonly Queue<UnexpectedPackageData> _showSpecialPopupQueue = new Queue<UnexpectedPackageData>();
    private readonly List<int> _checkInputQueueList = new List<int>();

    public void CheckAddSpecialPopup()
    {
        _checkInputQueueList.Clear();
        foreach(KeyValuePair<int, List<UnexpectedPackageData>> data in GameInfo.Instance.UnexpectedPackageDataDict)
        {
            UnexpectedPackageData packageData = data.Value.Find(x => x.IsAdd);
            if (packageData == null)
            {
                continue;
            }
            
            GameTable.UnexpectedPackage.Param tableData = GameInfo.Instance.GameTable.FindUnexpectedPackage(packageData.TableId);
            if (tableData == null)
            {
                continue;
            }
            
            if (_checkInputQueueList.Any(x => x == tableData.UnexpectedType))
            {
                continue;
            }
            
            _checkInputQueueList.Add(tableData.UnexpectedType);
            _showSpecialPopupQueue.Enqueue(packageData);
        }
    }
    
    public void ShowAddSpecialPopup()
    {
        if (_showSpecialPopupQueue.Count <= 0)
        {
            if (GetActiveUI("FacilityPanel") == null && GameSupport.IsShowSpecialBuyPopup())
            {
                UISpecialBuyPopup specialBuyPopup = LobbyUIManager.Instance.GetUI("SpecialBuyPopup") as UISpecialBuyPopup;
                if (specialBuyPopup != null)
                {
                    specialBuyPopup.SetChmuki();
                    specialBuyPopup.SetUIActive(true);
                }
            }
            
            return;
        }
        
        UnexpectedPackageData data = _showSpecialPopupQueue.Dequeue();
        GameTable.UnexpectedPackage.Param tableData = GameInfo.Instance.GameTable.FindUnexpectedPackage(data.TableId);
        if (tableData == null)
        {
            ShowAddSpecialPopup();
            return;
        }
        
        float waitTime = 0.233f;
        if (tableData.UnexpectedType == (int)eUnexpectedPackageType.FIRST_STAGE)
        {
            UISpecialBuyDailyPopup specialBuyDailyPopup = LobbyUIManager.Instance.GetUI("SpecialBuyDailyPopup") as UISpecialBuyDailyPopup;
            if (specialBuyDailyPopup != null)
            {
                if (1 < specialBuyDailyPopup.AniNameList.Count)
                {
                    waitTime = specialBuyDailyPopup.UIAni[specialBuyDailyPopup.AniNameList[1]].length + 0.1f;
                }
                
                specialBuyDailyPopup.SetGameTable(data.TableId);
                specialBuyDailyPopup.SetCloseAction(() => Invoke("ShowAddSpecialPopup", waitTime));
                specialBuyDailyPopup.SetUIActive(true);
            }
        }
        else
        {
            UISpecialBuyPopup specialBuyPopup = LobbyUIManager.Instance.GetUI("SpecialBuyPopup") as UISpecialBuyPopup;
            if (specialBuyPopup != null)
            {
                if (1 < specialBuyPopup.AniNameList.Count)
                {
                    waitTime = specialBuyPopup.UIAni[specialBuyPopup.AniNameList[1]].length + 0.1f;
                }

                UIItemInfoPopup popup = LobbyUIManager.Instance.GetActiveUI<UIItemInfoPopup>("ItemInfoPopup");
                if (popup != null)
                {
                    popup.SetUIActive(false);
                }
                
                specialBuyPopup.SetGameTable(data.TableId);
                specialBuyPopup.SetCloseAction(() => Invoke("ShowAddSpecialPopup", waitTime));
                specialBuyPopup.SetUIActive(true);
            }
        }
    }

    public long GetInvenCount(eREWARDTYPE pRewardType, int pIndex)
    {
        long lResult = 0;
        switch (pRewardType)
        {
            case eREWARDTYPE.GOODS:
                {
                    lResult = GameInfo.Instance.UserData.GetGoods((eGOODSTYPE)pIndex);
                    break;
                }
            case eREWARDTYPE.WEAPON:
                {
                    lResult = GameInfo.Instance.WeaponList.FindAll(x => x.TableID == pIndex).Count;
                    break;
                }
            case eREWARDTYPE.GEM:
                {
                    lResult = GameInfo.Instance.GemList.FindAll(x => x.TableID == pIndex).Count;
                    break;
                }
            case eREWARDTYPE.CARD:
                {
                    lResult = GameInfo.Instance.CardList.FindAll(x => x.TableID == pIndex).Count;
                    break;
                }
            case eREWARDTYPE.ITEM:
                {
                    ItemData itemData = GameInfo.Instance.ItemList.Find(x => x.TableID == pIndex);
                    if (itemData != null)
                    {
                        lResult = itemData.Count;
                    }
                    break;
                }
        }

        return lResult;
    }

    public GameTable.BingoEvent.Param GetBingoEvent(GameClientTable.EventPage.Param eventPage)
    {
        GameTable.BingoEvent.Param bingoEvent = GameInfo.Instance.GameTable.FindBingoEvent(x => x.ID == eventPage.TypeValue);
        if (bingoEvent == null)
        {
            return null;
        }

        DateTime startTime = GameSupport.GetTimeWithString(bingoEvent.StartTime);
        DateTime endTime = GameSupport.GetTimeWithString(bingoEvent.EndTime, true);
        DateTime currentTime = GameSupport.GetCurrentServerTime();

        if (currentTime < startTime || endTime < currentTime)
        {
            return null;
        }

        return bingoEvent;
    }

    public GameTable.AchieveEvent.Param GetAchieveEvent(GameClientTable.EventPage.Param eventPage)
    {
        GameTable.AchieveEvent.Param achieveEvent = GameInfo.Instance.GameTable.FindAchieveEvent(x => x.ID == eventPage.TypeValue);
        if (achieveEvent == null)
        {
            return null;
        }

        DateTime startTime = GameSupport.GetTimeWithString(achieveEvent.StartTime);
        DateTime endTime = GameSupport.GetTimeWithString(achieveEvent.EndTime, true);
        DateTime currentTime = GameSupport.GetCurrentServerTime();

        if (currentTime < startTime || endTime < currentTime)
        {
            return null;
        }

        return achieveEvent;
    }

    public string GetGemSetOpt(long[] gemUids)
    {
        Dictionary<int, int> dictGemSetOpt = new Dictionary<int, int>();

        foreach (long gemUid in gemUids)
        {
            GemData gemData = GameInfo.Instance.GemList.Find(x => x.GemUID == gemUid);
            if (gemData == null)
            {
                continue;
            }

            if (gemData.SetOptID <= 0)
            {
                continue;
            }

            if (dictGemSetOpt.ContainsKey(gemData.SetOptID))
            {
                ++dictGemSetOpt[gemData.SetOptID];
            }
            else
            {
                dictGemSetOpt.Add(gemData.SetOptID, 1);
            }
        }


        int titleWriteGroupId = -1;
        string titleStr = string.Empty;
        System.Text.StringBuilder resultStr = new System.Text.StringBuilder();
        foreach (KeyValuePair<int, int> keyValue in dictGemSetOpt)
        {
            List<GameClientTable.GemSetOpt.Param> gemSetOptParamList = GameInfo.Instance.GameClientTable.FindAllGemSetOpt(x => x.GroupID == keyValue.Key);
            foreach (GameClientTable.GemSetOpt.Param gemSetOptParam in gemSetOptParamList)
            {
                if (keyValue.Value < gemSetOptParam.SetCount)
                {
                    continue;
                }

                if (titleWriteGroupId != gemSetOptParam.GroupID)
                {
                    titleWriteGroupId = gemSetOptParam.GroupID;
                    resultStr.Append(FLocalizeString.Instance.GetText(3312, FLocalizeString.Instance.GetText(gemSetOptParam.Name))).Append("\n\n");
                }

                resultStr.Append(FLocalizeString.Instance.GetText(3313, gemSetOptParam.SetCount)).Append("\n");

                GameClientTable.BattleOptionSet.Param battleOptionSetParam = GameInfo.Instance.GameClientTable.FindBattleOptionSet(gemSetOptParam.GemBOSetID1);
                if (battleOptionSetParam != null)
                {
                    resultStr.Append(FLocalizeString.Instance.GetText(gemSetOptParam.Desc, battleOptionSetParam.BOFuncValue)).Append("\n\n");
                }
            }
        }

        return resultStr.ToString();
    }

    private void OnNetRaidRankingList( int result, PktMsgType pkt ) {
        if( result != 0 ) {
            return;
        }

        GameInfo.Instance.Send_ReqRaidFirstRankingList( OnNetRaidFirstRankingList );
    }

    private void OnNetRaidFirstRankingList( int result, PktMsgType pkt ) {
        if( result != 0 ) {
            return;
        }

        SetPanelType( ePANELTYPE.RAID );
    }
}
