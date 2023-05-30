
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Lobby : FMonoSingleton<Lobby>
{
    public enum eSELETETYPE
    {
        NONE = 0,
        CHAR,
        FACILITY,
        ROOM,
        DRONE,
        
        LOBBY,
    }


    [System.Serializable]
    public class sBGCharInfo
    {
        [HideInInspector] public LobbyPlayer BGChar;

        public eLayer   Layer;
        public Vector3  Pos;
        public float    Scale;


        public void Init(LobbyPlayer bgChar)
        {
            BGChar = bgChar;

            bgChar.Init();
            bgChar.SetKinematicRigidBody();
            bgChar.SetInitialPosition(Pos, Quaternion.Euler(0.0f, -90.0f, 0.0f), Scale);

			bgChar.costumeUnit.ShowObject(bgChar.costumeUnit.Param.InGameOnly, false);
			bgChar.costumeUnit.ShowObject(bgChar.costumeUnit.Param.LobbyOnly, true);

			Utility.SetLayer(bgChar.gameObject, (int)Layer, true);
            bgChar.CharLight.cullingMask = 1 << (int)Layer;
        }

        public void Destroy()
        {
            if (BGChar)
            {
                DestroyImmediate(BGChar.gameObject);
                BGChar = null;
            }
        }

        public void Show(bool show)
        {
            if(show)
            {
                BGChar.Show();
            }
            else
            {
                BGChar.Hide();
            }
        }
    }


    [Header("[Property]")]
    public LobbyCamera          lobbyCamera;
    public Transform            TfLobbyCamera;
    public GameObject           LobbyBg;
    public List<sBGCharInfo>    ListBGCharInfo;
    public Transform            LobbyBGRoot;

    public LobbyPlayer LobbyPlayer { get; private set; } = null;

    private List<LobbyFacility> _lobbyfacilitylist  = new List<LobbyFacility>();
    private int                 mCurRoomTableId     = 0;
    private string              mCurRoomSceneName   = null;
    private eSELETETYPE         mSelectType         = eSELETETYPE.NONE;
    private long                mSelectIndex        = 0;
    private bool                mbLockShow          = false;
    private long[]              ArrBgCharUid        = null; // 0번은 대표 캐릭터 고정


    public void InitLobby()
    {
        GameInfo.Instance.ContinueStage = false;
        CreateLobbyAssets();
    }

    private void CreateLobbyBG()
    {
        string lobbyScreenShot = PlayerPrefs.GetString("LobbyBGWithScreenShot", "false");
        Log.Show("LobbyBGWithScreenShot : " + lobbyScreenShot);

        if (lobbyScreenShot.ToLower().Equals("true") && LobbyBGScreenShotFileCheck())
        {
            SetBackGroundWithScreenShot();
        }
        else
        {
            var listLobbyThemes = GameInfo.Instance.GameTable.LobbyThemes;
            if (listLobbyThemes == null | listLobbyThemes.Count < 0)
                return;

            var tabledata = listLobbyThemes.Find(x => x.ID == GameInfo.Instance.UserData.UserLobbyThemeID);
            if (tabledata == null)
                return;

            SetBackGround(tabledata);
        }
    }

    private void CreateLobbyAssets()
    {
        CreateLobbyBG();
        CreateLobbyPlayer();
    }

	private void CreateLobbyPlayer() {
		LobbyPlayer player = GameSupport.CreateLobbyPlayer( GameInfo.Instance.GetMainChar() );

		player.CharLight_On();
		player.SetInitialPosition( ListBGCharInfo[0].Pos, Quaternion.Euler( 0.0f, -90.0f, 0.0f ) );
		player.Init();
		player.ShowShadow( false );
		LobbyPlayer = player;

		lobbyCamera.SetCameraType( LobbyCamera.eType.FixedTarget, TfLobbyCamera );

		mCurRoomTableId = 0;
		mCurRoomSceneName = null;

		LobbyPlayer.costumeUnit.ShowObject( LobbyPlayer.costumeUnit.Param.InGameOnly, false );
		LobbyPlayer.costumeUnit.ShowObject( LobbyPlayer.costumeUnit.Param.LobbyOnly, true );

		ArrBgCharUid = new long[GameInfo.Instance.GameConfig.MaxLobbyBgCharCount];

		player.SetKinematicRigidBody();

		ChangeMainChar();
		ShowBgChar( false );
	}

	public void ShowBgChar(bool show, bool lockShow = false)
    {
        mbLockShow = lockShow;

        for(int i = 0; i < ListBGCharInfo.Count; ++i)
        {
            if (ListBGCharInfo[i].BGChar == null)
            {
                continue;
            }
            
            ListBGCharInfo[i].BGChar.gameObject.SetActive(show);
        }
    }

    public void ChangeBgCharUid(int slot, long uid, OnReceiveCallBack callBack)
    {
        if(slot < 0 || slot >= ArrBgCharUid.Length || ArrBgCharUid[slot] == uid)
        {
            return;
        }

        int changeSlot = -1;
        for(int i = 0; i < ArrBgCharUid.Length; i++)
        {
            if(ArrBgCharUid[i] == uid)
            {
                changeSlot = i;
                break;
            }
        }

        if(changeSlot >= 0)
        {
            ArrBgCharUid[changeSlot] = ArrBgCharUid[slot];
        }

        ArrBgCharUid[slot] = uid;
        GameInfo.Instance.Send_ReqChangeLobbyBgChar(ArrBgCharUid, callBack);
    }

    public void RemoveBgCharUid(int slot, OnReceiveCallBack callBack)
    {
        if(ArrBgCharUid[slot] <= 0)
        {
            return;
        }

        ArrBgCharUid[slot] = 0;
        GameInfo.Instance.Send_ReqChangeLobbyBgChar(ArrBgCharUid, callBack);
    }

    private void UpdateBGChar()
    {
        for(int i = 0; i < ArrBgCharUid.Length; i++)
        {
            ArrBgCharUid[i] = 0;
        }

        for (int i = 0; i < GameInfo.Instance.UserData.ArrLobbyBgCharUid.Length; i++)
        {
            if (i >= ArrBgCharUid.Length)
            {
                break;
            }

            ArrBgCharUid[i] = GameInfo.Instance.UserData.ArrLobbyBgCharUid[i];
        }

        for (int i = 1; i < ListBGCharInfo.Count; i++)
        {
            ListBGCharInfo[i].Destroy();
        }

        for(int i = 0; i < ArrBgCharUid.Length; i++)
        {
            LobbyPlayer bgChar = null;

            if (i == 0)
            {
                bgChar = LobbyPlayer;
            }
            else
            {
                if(ArrBgCharUid[i] == 0)
                {
                    continue;
                }

                long uid = ArrBgCharUid[i];

                CharData charData = GameInfo.Instance.GetCharData(uid);
                if (charData == null)
                {
                    Debug.LogError("UID : " + uid + " 캐릭터 데이터가 없습니다.");
                    return;
                }

                bgChar = GameSupport.CreateLobbyPlayer(charData);
                if (bgChar == null)
                {
                    Debug.LogError(charData.TableID + "번 로비 캐릭터를 생성할 수 없습니다.");
                    return;
                }
            }

            ListBGCharInfo[i].Init(bgChar);
        }
    }

#if UNITY_EDITOR
    bool mbSetPartsColorMask = false;
#endif
    private void Update()
    {
		if(AppMgr.Instance.CustomInput == null || lobbyCamera == null || lobbyCamera.camera == null)
		{
			return;
		}

        if (AppMgr.Instance.CustomInput.GetButtonDown(BaseCustomInput.eKeyKind.Select) && lobbyCamera.camera.enabled)
        {
            Ray ray = lobbyCamera.camera.ScreenPointToRay(AppMgr.Instance.CustomInput.GetTouchPos());

            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, 
                                (1 << (int)eLayer.Player) | (1 << (int)eLayer.Enemy) | (1 << (int)eLayer.EnemyGate)))
            {
                if(AppMgr.Instance.CustomInput.IsOverUI())
                {
                    return;
                }

                LobbyPlayer selectedPlayer = hitInfo.collider.GetComponent<LobbyPlayer>();
                if (selectedPlayer)
                {
                    selectedPlayer.Touch();
                }
            }
            else
            {
                if (LobbyUIManager.Instance.PanelType == ePANELTYPE.MAIN)
                {
                    UIMainPanel mainpanel = LobbyUIManager.Instance.GetActiveUI<UIMainPanel>("MainPanel");
                    if (mainpanel != null)
                    {
                        if (mainpanel.UIHide)
                        {
                            mainpanel.SetUIShow();
                        }
                    }
                }
            }
        }

#if UNITY_EDITOR
        int r, g, b;
		if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if(LobbyPlayer.costumeUnit == null || LobbyPlayer.costumeUnit.CostumeBody == null)
            {
                return;
            }

            mbSetPartsColorMask = !mbSetPartsColorMask;
            if(mbSetPartsColorMask)
            {
                for (int i = 0; i < ListBGCharInfo.Count; i++)
                {
                    if(ListBGCharInfo[i].BGChar == null)
                    {
                        continue;
                    }

                    ListBGCharInfo[i].BGChar.aniEvent.SetMaskTexture(ListBGCharInfo[i].BGChar.costumeUnit.CostumeBody.TexPartsColor);
                }
            }
            else
            {
                for (int i = 0; i < ListBGCharInfo.Count; i++)
                {
                    if (ListBGCharInfo[i].BGChar == null)
                    {
                        continue;
                    }

                    ListBGCharInfo[i].BGChar.aniEvent.SetMaskTexture(null);
                }
            }
        }
#endif
    }

    public void ChangeMainChar()
    {
        CharData mainCharData = GameInfo.Instance.GetMainChar();

        if (LobbyPlayer != null)
        {
            Destroy(LobbyPlayer.gameObject);
        }

        LobbyPlayer = GameSupport.CreateLobbyPlayer(mainCharData);
        LobbyPlayer.CharLight_On();
        LobbyPlayer.SetInitialPosition(LobbyPlayer.LobbyPos, Quaternion.Euler(0.0f, -90.0f, 0.0f));
        LobbyPlayer.Init();
        this.SetCamera(mSelectType, mSelectIndex, true);

        LobbyPlayer.SetKinematicRigidBody();

        UpdateBGChar();
    }
   
    public void SetCamera(eSELETETYPE type, long id, bool ignoreSameType = false)
    {
        if (ignoreSameType == false && type == mSelectType && id == mSelectIndex)
            return;

        //_prevselecttype = mSelectType;
        //_prevselectindex = mSelectIndex;
        mSelectType = type;
        mSelectIndex = id;


        switch (type)
        {
            case eSELETETYPE.LOBBY:
                {
                    //kLobbyPlayerCamera.SetCameraTranform(TfLobbyCamera);
                    //kLobbyPlayerCamera.SetTarget(kMainCharTarget, true);
                    lobbyCamera.SetCameraType(LobbyCamera.eType.FixedTarget, TfLobbyCamera);
                }
                break;
                /*
            case eSELETETYPE.ROOM:
                {
                    //int i = (int)id;
                    //Transform tgt = i == -1 ? kTarget : kLobbyRoomList[i].kTarget;
                    //_lobbyPlayerlist[]
                    kLobbyPlayerCamera.SetTarget(kMainCharTarget, true);
                }
                break;
          
            case eSELETETYPE.CHAR:
                {
                    UIMainPanel mainpanel = LobbyUIManager.Instance.GetUI<UIMainPanel>("MainPanel");
                    int i = (int)id;
                    if (_lobbyPlayerlist[(int)id].Pos == mainpanel.roomNo)
                        kLobbyPlayerCamera.SetSmoothMoveCamera(_lobbyPlayerlist[i].transform, eSELETETYPE.CHAR, false);
                    else
                        kLobbyPlayerCamera.SetTarget(_lobbyPlayerlist[i].transform, true, eSELETETYPE.CHAR);
                }
                break;
            case eSELETETYPE.DRONE:
                {
                   
                    UIMainPanel mainpanel = LobbyUIManager.Instance.GetUI<UIMainPanel>("MainPanel");
                    int i = mainpanel.roomNo;
                    kDronePlayer.Pos = i;
                    kLobbyPlayerCamera.SetDroneCamera(kDronePlayer.transform, mSelectType);\
                   
                }
                break;
            case eSELETETYPE.FACILITY:
                {
                    //kLobbyPlayerCamera.SetFixedCamera(kLobbyFacilityList[(int)id].kCamera.transform, 1.0f);
                }
                break;
            
            */
            default:
                { }
                break;
        }
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //
    //
    //------------------------------------------------------------------------------------------------------------------------------------------------------
    public bool FacilityUpgreadFlag()
    {
        for(int i = 0; i < _lobbyfacilitylist.Count; i++)
        {
            if (_lobbyfacilitylist[i] == null)
                continue;
            
            if (!_lobbyfacilitylist[i].gameObject.activeSelf)
                continue;

            if (_lobbyfacilitylist[i].kUpgreadEffectFlag != null)
                return false;
        }

        return true;
    }

    public void MoveToLobby()
    {
        mbLockShow = false;
        //kLobbyPlayerCamera.SetCameraTranform(TfLobbyCamera);

        // 로비의 캐릭터의 capsule Collider 를 다시 켜준다. 피규어룸 작업시에 충돌로 인하여 로비 캐릭터가 이동되어버리는 현상 발생
        if (LobbyPlayer != null && LobbyPlayer.MainCollider != null)
            LobbyPlayer.MainCollider.Enable(true);

        lobbyCamera.SetCameraType(LobbyCamera.eType.FixedTarget, TfLobbyCamera);

        for (int i = 0; i < _lobbyfacilitylist.Count; i++)
            _lobbyfacilitylist[i].gameObject.SetActive(false);

        if (!string.IsNullOrEmpty(mCurRoomSceneName))
        {
            SceneManager.UnloadSceneAsync(mCurRoomSceneName);

            mCurRoomTableId = 0;
            mCurRoomSceneName = null;
        }
    }

    public void MoveToFacility( int tableid)
    {
        for (int i = 0; i < _lobbyfacilitylist.Count; i++)
            _lobbyfacilitylist[i].gameObject.SetActive(false);

        FacilityData facilitydata = GameInfo.Instance.GetFacilityData(tableid);
        if (facilitydata == null)
            return;

        LobbyFacility lf = null;
        //lf = GetLobbyFacility(tableid);
        lf = GetLobbyFacility(facilitydata.TableID);
        
        if (lf == null)
        {
            //생성
            string strModel = Utility.AppendString("Room/Prefab/", facilitydata.TableData.Model, ".prefab");
            lf = ResourceMgr.Instance.CreateFromAssetBundle<LobbyFacility>("room", strModel);
            _lobbyfacilitylist.Add(lf);
            
        }
        if (lf == null)
            return;

        if (!lf.gameObject.activeSelf)
            lf.gameObject.SetActive(true);
        lf.InitLobbyFacility(facilitydata);
        

        //kLobbyPlayerCamera.SetCameraTranform(lf.kFacilityCamera);
        lobbyCamera.SetCameraType(LobbyCamera.eType.FixedTarget, lf.kFacilityCamera);
    }

    public bool IsSameRoom(int tableId)
    {
        return mCurRoomTableId == tableId;
    }

    public void MoveToRoom( int tableid )
    {
        if (IsSameRoom(tableid))
            return;

        StartCoroutine(UnloadScene(tableid, mCurRoomSceneName));
    }

    /// <summary>
    /// MoveToRoom 사용시에 같은 테이블 아이디일 경우 이동이 불가하여, 강제로 이동하는 함수.
    /// </summary>
    /// <param name="tableId"></param>
    public void ForceToRoom(int tableId)
    {
        StartCoroutine(UnloadScene(tableId, mCurRoomSceneName));
    }

    private IEnumerator UnloadScene(int tableid, string sceneName)
    {
        if (!string.IsNullOrEmpty(mCurRoomSceneName))
        {
            LobbyUIManager.Instance.SetPanelType(ePANELTYPE.NULL);

            AsyncOperation async = SceneManager.UnloadSceneAsync(mCurRoomSceneName);
            while (!async.isDone)
                yield return null;
            
            //while (UIValue.Instance.GetValue(UIValue.EParamType.SelectedRoomData) != null)
            //    yield return null;
        }

        LoadFigureRoomScene(tableid);
    }

    private void LoadFigureRoomScene(int tableId)
    {
        GameTable.RoomTheme.Param data = GameInfo.Instance.GameTable.FindRoomTheme(x => x.ID == tableId);
        if (data == null)
            return;

        mCurRoomTableId = tableId;
        mCurRoomSceneName = data.Scene;

        LobbyDoorPopup.Show(StarCloseDoor, false);
    }

    private void StarCloseDoor()
    {
        StartCoroutine("LodingRoomScene", mCurRoomSceneName);
    }

    private IEnumerator LodingRoomScene(string sceneName)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            if (async.progress >= 0.9f)
                async.allowSceneActivation = true;
            
            yield return null;
        }
        // 로비의 캐릭터의 capsule Collider 를 꺼준다. 피규어룸 작업시에 충돌로 인하여 로비 캐릭터가 이동되어버리는 현상 발생
        if (LobbyPlayer != null && LobbyPlayer.MainCollider != null)
            LobbyPlayer.MainCollider.Enable(false);

        FigureRoomScene.Instance.Init();
        yield return FigureRoomScene.Instance.LoadFigures();
        
        System.GC.Collect();

        SoundManager.Instance.PlayUISnd(31);
        LobbyUIManager.Instance.HideUI("LobbyDoorPopup", true);

        if (GameSupport.IsFriendRoom())
            LobbyUIManager.Instance.SetPanelType(ePANELTYPE.FIGUREROOM_FREELOOK);
        else
            LobbyUIManager.Instance.SetPanelType(ePANELTYPE.FIGUREROOM);
    }

    public LobbyFacility GetLobbyFacility( int id )
    {
        return _lobbyfacilitylist.Find(x => x.kTableID == id);
    }

    public void ActivationRoomEffect(int tableid, int idx = 0)
    {
        LobbyFacility lobbyFacility = GetLobbyFacility(tableid);
        if (lobbyFacility != null)
            lobbyFacility.ActiveFacilityEffect(idx);
    }
    public void DisabledRoomEffect(int tableid, int idx = 0)
    {
        LobbyFacility lobbyFacility = GetLobbyFacility(tableid);
        if (lobbyFacility != null)
            lobbyFacility.DisableFacilityEffect(idx);
    }

    public void CompleteFacility(int tableId, int realTableId = -1, bool operation = false)
    {
        LobbyFacility lobbyFacility = GetLobbyFacility(tableId);
        if (lobbyFacility != null)
        {
            if (operation)
            {
                lobbyFacility.CompleteFacility(realTableId);
            }
            else
            {
                lobbyFacility.CompleteFacility();
            }
        }
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  
    //
    //------------------------------------------------------------------------------------------------------------------------------------------------------
    public void SetShowLobbyPlayer()
    {
        if(mbLockShow)
        {
            return;
        }

        bool bshow = false;

        //UIMainPanel mainpanel = LobbyUIManager.Instance.GetActiveUI<UIMainPanel>("MainPanel");
        //if(mainpanel != null)
        if (LobbyUIManager.Instance.PanelType == ePANELTYPE.MAIN)
        {
            if (LobbyUIManager.Instance.ActivePopupList.Count == 0)
            {
                bshow = true;
            }
            else
            {
                if (LobbyUIManager.Instance.ActivePopupList[LobbyUIManager.Instance.ActivePopupList.Count - 1].name == "MenuPopup" ||
                    LobbyUIManager.Instance.ActivePopupList[LobbyUIManager.Instance.ActivePopupList.Count - 1].name == "MailBoxPopup")
                {
                    bshow = true;
                }
            }

            if (LobbyPlayer)
            {
                LobbyPlayer.costumeUnit.ShowObject(LobbyPlayer.costumeUnit.Param.InGameOnly, false);
                LobbyPlayer.costumeUnit.ShowObject(LobbyPlayer.costumeUnit.Param.LobbyOnly, true);
            }
        }
        
        foreach (sBGCharInfo info in ListBGCharInfo)
        {
            if (info.BGChar == null)
            {
                continue;
            }
            
            bool nowactive = info.BGChar.gameObject.activeSelf;
            info.BGChar.gameObject.SetActive(bshow);
            
            if (bshow && !nowactive)
            {
                info.BGChar.Init();
            }
        }
    }

    GameTable.LobbyTheme.Param _CurrentLobbyTheme = null;
    public void SetBackGround(GameTable.LobbyTheme.Param param)
    {
        if (param == null)
            return;
        _CurrentLobbyTheme = param;

        if (LobbyBg != null)
            DestroyImmediate(LobbyBg);
        
        GameObject lobbyBG = ResourceMgr.Instance.CreateFromAssetBundle("room", param.Prefab + ".prefab");
        if (lobbyBG == null)
            return;

        lobbyBG.transform.parent = LobbyBGRoot;
        lobbyBG.transform.localPosition = Vector3.zero;
        lobbyBG.transform.localRotation = Quaternion.identity;
        lobbyBG.transform.localScale = Vector3.one;

        LobbyBg = lobbyBG;

        //BGM 교체
        SoundManager.Instance.PlayBgm(param.Bgm, FSaveData.Instance.GetMasterVolume(), true);

        PlayerPrefs.SetString("LobbyBGWithScreenShot", "false");
    }

    public void SetBackGroundWithScreenShot()
    {
        if (LobbyBg != null)
            DestroyImmediate(LobbyBg);

        GameObject lobbyBG = ResourceMgr.Instance.CreateFromAssetBundle("room", "Room/Art/Lobby/lobby_screen.prefab");
        if (lobbyBG == null)
            return;

        MeshRenderer renderer = lobbyBG.GetComponent<MeshRenderer>();

        Texture2D mainTex = LoadLobbyBGScreenShot();

        Material renderMat = renderer.material;
        renderMat.SetTexture("_MainTex", mainTex);
        renderMat.SetTexture("_Emission", mainTex);

        lobbyBG.transform.parent = LobbyBGRoot;
        lobbyBG.transform.localPosition = Vector3.zero;
        lobbyBG.transform.localRotation = Quaternion.identity;
        lobbyBG.transform.localScale = Vector3.one;

        lobbyBG.transform.parent = this.gameObject.transform;

        // ======================================================================================
        // Prev Code
        // float height = (float)(Camera.main.orthographicSize * 2.0);
        // float width = height * Screen.width / Screen.height;
        // lobbyBG.transform.localScale = new Vector3(width / 10, 1, height / 10);

        //lobbyBG.transform.localScale = new Vector3(Camera.main.orthographicSize * 2.0f * Screen.width / Screen.height, 0.1f, Camera.main.orthographicSize * 2.0f);
        
        // Next Code
        lobbyBG.transform.localPosition = new Vector3(
            lobbyBG.transform.localPosition.x, TfLobbyCamera.transform.localPosition.y, TfLobbyCamera.transform.localPosition.z);
        float height = (float)(Camera.main.orthographicSize * 0.2f);
        float width = height * Screen.width / Screen.height;
        lobbyBG.transform.localScale = new Vector3(width, 1, height);
        // ======================================================================================
        

        LobbyBg = lobbyBG;

        //BGM 교체
        if (LobbyUIManager.Instance.GetActiveUI("FigureRoomPanel") == null && LobbyUIManager.Instance.GetActiveUI("FigureRoomFreeLookPanel") == null)
        {
            List<GameTable.LobbyTheme.Param> listLobbyThemes = GameInfo.Instance.GameTable.LobbyThemes;
            if (listLobbyThemes == null | listLobbyThemes.Count < 0)
                return;

            GameTable.LobbyTheme.Param tabledata = listLobbyThemes.Find(x => x.ID == GameInfo.Instance.UserData.UserLobbyThemeID);
            if (tabledata == null)
                return;

            _CurrentLobbyTheme = tabledata;

            SoundManager.Instance.PlayBgm(tabledata.Bgm, 1f, true);
        }
        
    }

    private Texture2D LoadLobbyBGScreenShot()
    {
        string path = string.Empty;
        string folderName = "/ScreenShot/";
#if UNITY_EDITOR
        path = Application.dataPath + folderName;
#elif !DISABLESTEAMWORKS
        path =  Application.dataPath + folderName;        
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
#elif UNITY_ANDROID
        //path =  "/storage/emulated/0/DCIM" + folderName;  //"mnt/sdcard/Pictures"
        //path = Application.persistentDataPath + folderName;
        path = Application.persistentDataPath;
#elif UNITY_IOS
    //#if USE_UNITY_CAPTURE_API
            path = Application.persistentDataPath + folderName;
    //#else
            path = Application.persistentDataPath;
    //#endif

#endif
        Log.Show(path);

        string filePath = path + "/" + GameInfo.Instance.GameConfig.LobbyBGFileName;
        Log.Show(filePath);
        if (File.Exists(filePath))
        {
            Texture2D mainTex = new Texture2D(2, 3);

            byte[] bytes = File.ReadAllBytes(filePath);
            mainTex.LoadImage(bytes);

            return mainTex;
        }
           
        
        return null;
    }

    private bool LobbyBGScreenShotFileCheck()
    {
        string path = string.Empty;
        string folderName = "/ScreenShot/";
#if UNITY_EDITOR
        path = Application.dataPath + folderName;
#elif !DISABLESTEAMWORKS
        path =  Application.dataPath + folderName;        
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
#elif UNITY_ANDROID
        //path =  "/storage/emulated/0/DCIM" + folderName;  //"mnt/sdcard/Pictures"
        path = Application.persistentDataPath;
#elif UNITY_IOS
    //#if USE_UNITY_CAPTURE_API
            path = Application.persistentDataPath + folderName;
    //#else
            path = Application.persistentDataPath;
    //#endif

#endif
        string filePath = path + "/" + GameInfo.Instance.GameConfig.LobbyBGFileName;
        return File.Exists(filePath);
    }

    public int GetCurrentBGMID()
    {
        if (_CurrentLobbyTheme == null)
            return 1000;

        return _CurrentLobbyTheme.Bgm;
    }

    public void SetLobbyBG(bool state)
    {
        if (LobbyBg == null) return;
        LobbyBg.SetActive(state);
    }
}

