using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;


public class AcceptAllCertificates : CertificateHandler {
    protected override bool ValidateCertificate( byte[] certificateData ) { return true; }
}

public class TitleUIManager : FManager
{
    private static TitleUIManager s_instance = null;
    public static TitleUIManager Instance
    {
        get
        {
            return s_instance;
        }
    }


    public enum TITLEPROGRESS
    {
        NONE = 0,
        TERMSOFUSE,
        RESLOADTYPE,
        VERSIONCHECK,
        SERVERCHECK,
        PATCH,
        LOADASSETBUNDLES,
        WAIT
    }

	public enum eMovieState
	{
		None = 0,
		PV,
		Synopsis,
		PVOnly,
		SynopsisOnly,
	}


    private static int CDN_RETRY_COUNT = 3;

    public List<AudioClip>  kAudioClipList;
    public int              MaxBundleCount  = 0;
    public int              NowBundleCount  = 0;

    public string			StrAskRepatch	                { get; private set; } = string.Empty;
	public TITLEPROGRESS	TitleProgress	                { get; private set; } = TITLEPROGRESS.NONE;
	public eMovieState		MovieState		                { get; private set; } = eMovieState.None;
	public bool				IsPVPlay		                { get; private set; } = false;
    public bool             IsUpdateSystemAndUpdateBundle   { get; private set; } = false;
    public bool             IsUpdate                        { get { return _bupdate; } }

    private eLANGUAGE               eLanguage;
    private string                  strUpdateAOS                = string.Empty;
    private string                  strUpdateiOS                = string.Empty;
    private string                  strUpdateSteam              = string.Empty;
    private string                  strServerRelocateUpdateAOS  = string.Empty;
    private string                  strServerRelocateUpdateiOS  = string.Empty;
    private bool                    bGoGlobalStore              = false;
    private string                  strVersionJsonUrl           = string.Empty;
    private string                  strServerJsonUrl            = string.Empty;
    private string                  strConnectionText           = string.Empty;
    private string                  strConnectionTitle          = string.Empty;
    private string                  strDataCheckText            = string.Empty;
    private string                  strRestartText              = string.Empty;
    private string                  strDownLoadText             = string.Empty;
    private string                  strPopupBtn                 = string.Empty;
    private string                  strPopupConfirmBtn          = string.Empty;
    private string                  strPopupCancelBtn           = string.Empty;
    private FPatchSaveData          LocalPatchData              = new FPatchSaveData();
    private FPatchSaveData          ServerPatchData             = new FPatchSaveData();
    private List<string>            BundleNameList              = new List<string>();
    private List<int>               BundleVersionList           = new List<int>();
    private List<long>              BundleSizeList              = new List<long>();
    private List<string>            BundleNewNameList           = new List<string>();
    private bool                    bloadingscene               = false;
    private UITitlePanel            _titlepanel;
    private UICinematicPopup        _cinematicpopup;
    private bool                    _bupdate                    = false;
    private string                  mCultureName                = "";
    private List<AudioSource>       _audiosourcelist            = new List<AudioSource>();
    private Director                _DirectorSynopsis           = null;
    private UnityWebRequest         mWebReq                     = null;
    private AcceptAllCertificates   mAcceptAllCertificates      = null;
    private WaitForSeconds          mWaitForHalfSec             = new WaitForSeconds( 0.5f );
    private int                     mCheckRetryCount            = 0;


    void Awake()
    {
        if (s_instance == null)
            s_instance = this;
        else
            Debug.LogError("LobbyUIManager Instance Error!");
    }

    public override void Start()
    {
        base.Start();

        etlEncrypt.LoadKey();

        AssetBundleMgr.Instance.Release();
        FLocalizeAtlas.Instance.ClearAtlas();

        if (AppMgr.Instance.configData.m_resLoadType == Config.eResLoadType.AssetBundle_Mix)
        {
            AssetBundleMgr.Instance.FindLocalBundlesAndClearCache();
        }
        
        AppMgr.Instance.SetSceneType(AppMgr.eSceneType.Title);
        //화면꺼짐 방지
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //아이폰에서는
        //iPhoneSettings.screenCanDarken = false;
        //화면회전 고정
        //Android playersetting->Resolution Android Presentation->Default Orientation 옵션
        //Auto  자동회전
        //Portrait  세로고정
        //Landscape 가로고정
        //여기서 수정하면 다른곳도 적용


        /*
#if UNITY_EDITOR
        PlayerPrefs.SetInt(SAVETYPE.LANGUAGE.ToString(), (int)eLANGUAGE.KOR); //나중에는 지워야함
#endif
        */

        bool bHasLANGUAGE = PlayerPrefs.HasKey(SAVETYPE.LANGUAGE.ToString());
        if (bHasLANGUAGE)
        {
            eLanguage = (eLANGUAGE)PlayerPrefs.GetInt(SAVETYPE.LANGUAGE.ToString());
            Log.Show(eLanguage, Log.ColorType.Red);
        }
#if DISABLESTEAMWORKS // 스팀일 경우엔 밑에서 스팀 초기화 하고 스팀 클라이언트에 설정된 언어를 가져옴
        else
        {
            if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Japan)
            {
                eLanguage = eLANGUAGE.JPN;
            }
            else
            {
                if (AppMgr.Instance.configData.m_InitLanguage == Config.eInitLanguage.System)
                {
                    SystemLanguage slanguage = Application.systemLanguage;
                    if (slanguage == SystemLanguage.Korean)
                        eLanguage = eLANGUAGE.KOR;
                    else if (slanguage == SystemLanguage.Japanese)
                        eLanguage = eLANGUAGE.JPN;
                    else if (slanguage == SystemLanguage.English)
                        eLanguage = eLANGUAGE.ENG;
                    else if (slanguage == SystemLanguage.Chinese)
                        eLanguage = eLANGUAGE.CHT;
                    else if (slanguage == SystemLanguage.ChineseSimplified)
                        eLanguage = eLANGUAGE.CHS;
                    else if (slanguage == SystemLanguage.ChineseTraditional)
                        eLanguage = eLANGUAGE.CHT;
                    else if (slanguage == SystemLanguage.Spanish)
                        eLanguage = eLANGUAGE.ESP;
                    else
                    {
                        if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Japan)
                            eLanguage = eLANGUAGE.JPN;
                        else
                            eLanguage = eLANGUAGE.ENG;
                    }
                }
                else
                    eLanguage = (eLANGUAGE)AppMgr.Instance.configData.m_InitLanguage - 1;
            }

            PlayerPrefs.SetInt(SAVETYPE.LANGUAGE.ToString(), (int)eLanguage);
            PlayerPrefs.Save();
        }
#endif

        if ( System.Globalization.CultureInfo.CurrentCulture != null && !string.IsNullOrEmpty( System.Globalization.CultureInfo.CurrentCulture.Name ) ) {
            mCultureName = System.Globalization.CultureInfo.CurrentCulture.Name;
        }

        strVersionJsonUrl = AppMgr.Instance.configData.GetLivePatchServer() + "/version.json?cache=no";
        mAcceptAllCertificates = new AcceptAllCertificates();
        mCheckRetryCount = 0;

        for (int i = 0; i < AppMgr.Instance.configData.m_listLocalizeFont.Count; i++)
        {
            if (AppMgr.Instance.configData.m_listLocalizeFont[i])
            {
                Debug.Log("Resources폴더로 폰트가 묶였네!!!!!" + AppMgr.Instance.configData.m_listLocalizeFont[i].name);
            }
        }

        //LINE SDK설정
        if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Japan || AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Global)
        {
            Line.LineSDK.LineSDK.Instance.channelID = AppMgr.Instance.GetLINE_KEY;
            Line.LineSDK.LineSDK.Instance.SetupSDK();
        }

        Log.Show("#### / " + eLanguage, Log.ColorType.Red);

        //스팀매니저 초기화
#if !DISABLESTEAMWORKS
        if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam)
        {
            if (!SteamManager.Initialized)
            {
                System.Action OnApplicationQuit = () =>
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                };

                UIMessagePopup mpopup = MessagePopup.GetMessagePopup();
                if (mpopup == null)
                {
                    OnApplicationQuit();
                    return;
                }

                mpopup.InitMessagePopupOK("Notice", "Steam client not found.", "OK", eGOODSTYPE.NONE, 0,
                    () => { OnApplicationQuit(); }, true);

                Debug.LogWarning("Fail SteamManager.Initialized");
            }
            AppMgr.Instance.InitScreenResolution();

            // 스팀용 언어 초기화
            if (!bHasLANGUAGE)
            {
                eLANGUAGE eLanguage = eLANGUAGE.ENG;
                switch (Steamworks.SteamUtils.GetSteamUILanguage())
                {
                    case "koreana": eLanguage = eLANGUAGE.KOR; break;
                    case "japanese": eLanguage = eLANGUAGE.JPN; break;
                    case "schinese": eLanguage = eLANGUAGE.CHS; break;
                    case "tchinese": eLanguage = eLANGUAGE.CHT; break;
                    case "spanish": eLanguage = eLANGUAGE.ESP; break;
                }
                PlayerPrefs.SetInt(SAVETYPE.LANGUAGE.ToString(), (int)eLanguage);
                PlayerPrefs.Save();
            }
        }
#endif
        Debug.Log("PushKey : " + AppMgr.Instance.configData.GetPushServerKey());

#if !DISABLESTEAMWORKS
        StartCoroutine(SecurityInfoCheck());
#else
#if UNITY_ANDROID
        AssetBundleMgr.Instance.PreLoad(() => StartCoroutine(BeforehandAssetBundles()));
    #else
        //7788  system 번들 로드 되어 있어야함
        StartCoroutine(BeforehandAssetBundles());
    #endif
#endif
    }

#if !DISABLESTEAMWORKS
    IEnumerator SecurityInfoCheck()
    {
        string url = AppMgr.Instance.GetSecurityJsonUrl( false );

        WWW www = new WWW(url);
        yield return www;

        if (www.error == null)
        {
            var json = JSON.Parse(www.text);
            AppMgr.Instance.FBSCode = json["facebook"];
        }

        StartCoroutine(BeforehandAssetBundles());
    }
#endif
    public override void Update()
    {
        base.Update();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F1))
        {
			//PlaySynopsis();
			StartCoroutine(LoadSynopsis());

			MovieState = eMovieState.PV;
			_titlepanel.PlayPV(PlaySynopsis);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
			StartCoroutine(LoadSynopsis());
			TitleUIManager.Instance.ShowUI("LoadTipPopup", true);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            TitleUIManager.Instance.HideUI("LoadTipPopup", true);
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            StartCoroutine(LoadSynopsis());
        }
#endif
    }

    public override void OnEscape()
    {
        base.OnEscape();

        if (bloadingscene || GetActiveUI("WaitPopup") != null || GetActiveUI("LoadingPopup") != null)
        {
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
            if (messagepopup != null)
            {
                //메세지 팝업이 가장 최상단에 뜨기때문에 열려있으면 우선적으로 닫아주기.
                messagepopup.OnClickClose();
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
                for (int i = 0; i < list.Count; i++)
                {
                    if (popupDepth < list[i].GetPanelDepth())
                    {
                        popupDepth = list[i].GetPanelDepth();
                        popupIdx = i;
                    }
                }

                FComponent ui = list[popupIdx];
                if (ui && !ui.UIAni.isPlaying && ui.IsBackButton() == true)
                {
                    //HideUI(ui);
                    Log.Show("@@@Close UI : " + ui.name, Log.ColorType.Red);
                    ui.OnClickClose();
                }
            }
        }
        else
        {
            MessagePopup.CYN(eTEXTID.TITLE_NOTICE, 3046, eTEXTID.YES, eTEXTID.NO, Application.Quit);
        }
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  LIAPP 확인
    //
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void OnLIAPPSuccess()
    {
        ShowTouchToStart();
    }


    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  이용 약관 동의 절차
    //
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    IEnumerator NextTermsofUse(bool bstart)
    {
        TitleProgress = TITLEPROGRESS.TERMSOFUSE;
        if (bstart)
        {
            float fAniTime = _titlepanel.GetOpenAniTime();
            yield return new WaitForSeconds(fAniTime * 0.5f);
        }

        bool b = PlayerPrefs.HasKey(SAVETYPE.TERMSOFUSE.ToString());
        if (b)
        {
            StartCoroutine(NextScene());
        }
        else
        {
            GameSupport.SendFireBaseLogEvent(eFireBaseLogType._RUN);
            TitleUIManager.Instance.ShowUI("LawPopup", true);
        }
        yield return null;
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  리소스 타입에 따른 분기 처리
    //
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    IEnumerator NextScene()
    {
        TitleProgress = TITLEPROGRESS.RESLOADTYPE;

        if( AppMgr.Instance.configData.m_ServerSelectType == Config.eServerSelectType.CloudServer ) {
            AppMgr.Instance.ServerType = AppMgr.eServerType.REVIEW;
        }

        Config.eResLoadType resLoadType = AppMgr.Instance.GetResLoadType();
        if (resLoadType == Config.eResLoadType.Folder)
        {
        #if UNITY_EDITOR
            AppMgr.Instance.GetServerJsonUrl(false);
        #endif

            ShowTouchToStart();
        }
        else if (resLoadType == Config.eResLoadType.Resources)
        {
            ShowTouchToStart();
        }
        else if (resLoadType == Config.eResLoadType.AssetBundle_Local)
        {
            //eResLoadType.AssetBundle_Local StandAlone 에서는 저장된 버전과 apk버전이 다르면 캐쉬클린한다.
            string ApkVersion = AppMgr.Instance.configData.m_version;
            string NowVersion = "";
            bool b = PlayerPrefs.HasKey(SAVETYPE.VERSION.ToString());
            if (b)
                NowVersion = PlayerPrefs.GetString(SAVETYPE.VERSION.ToString());

            if (NowVersion != ApkVersion)
                Caching.ClearCache();

            PlayerPrefs.SetString(SAVETYPE.VERSION.ToString(), ApkVersion);

            for (int i = 0; i < AppMgr.Instance.GetAssetBundleCount(); i++)
            {
                BundleNameList.Add(AppMgr.Instance.GetAssetBundleName(i));
                BundleVersionList.Add(1);
                BundleSizeList.Add(0);
            }

            AppMgr.Instance.Nocheckdid = true;


            StartCoroutine(LoadAssetBundles());
        }
        else
        {
            StartCoroutine(VersionInfoCheck());
        }

        yield return null;
    }

    /// <summary>
    /// 버전 정보을 얻어와서 확인
    /// </summary>
    IEnumerator VersionInfoCheck( bool patchDownload = true ) {
		TitleProgress = TITLEPROGRESS.VERSIONCHECK;

        /*
        if ( mCheckRetryCount >= CDN_RETRY_COUNT - 1 ) {
            strVersionJsonUrl = AppMgr.Instance.configData.GetLivePatchServer() + "/version.json?cache=no";
        }
        else {
            strVersionJsonUrl = AppMgr.Instance.configData.GetLivePatchServer() + "/33version.json?cache=no";
        }
        */

		mWebReq = UnityWebRequest.Get( strVersionJsonUrl );
        mWebReq.certificateHandler = mAcceptAllCertificates;

        yield return mWebReq.SendWebRequest();

        string strPopupText = strConnectionText;
        string strNoticeText = strConnectionTitle;
        string strBtn = strPopupBtn;
        bool failed = false;
        bool retry = false;

        if ( mWebReq != null && mWebReq.result == UnityWebRequest.Result.Success ) {
			JSONNode json = JSON.Parse( mWebReq.downloadHandler.text );

			string version_live = string.Empty;
			string version_review = string.Empty;
			string version_qa = string.Empty;
			string version_internal = string.Empty;

			version_live = json["version-live"];
			version_review = json["version-review"];
			version_qa = json["version-qa"];
			version_internal = json["version-internal"];

			if ( AppMgr.Instance.configData.m_version == version_internal ) {
				AppMgr.Instance.PatchFolder = "/internal";
				AppMgr.Instance.ServerType = AppMgr.eServerType.INTERNAL;
			}
			else if ( AppMgr.Instance.configData.m_version == version_qa )
			{
				AppMgr.Instance.PatchFolder = "/qa";
				AppMgr.Instance.ServerType = AppMgr.eServerType.QA;
			}
			else if ( AppMgr.Instance.configData.m_version == version_review )
			{
				AppMgr.Instance.PatchFolder = "/review";
				AppMgr.Instance.ServerType = AppMgr.eServerType.REVIEW;
			}
			else {
				AppMgr.Instance.PatchFolder = "/live";
				AppMgr.Instance.ServerType = AppMgr.eServerType.LIVE;
			}
		}
		else if ( mWebReq == null || mWebReq.result != UnityWebRequest.Result.InProgress ) {
            failed = true;
		}

		if ( !patchDownload ) {
            if ( mWebReq != null ) {
                mWebReq.Dispose();
                mWebReq = null;
            }

            yield break;
		}
		else {
			if ( failed ) {
                string webReqError = mWebReq != null ? mWebReq.error : "WebReq is null";

                if ( mCheckRetryCount >= CDN_RETRY_COUNT ) {
                    Firebase.Analytics.FirebaseAnalytics.LogEvent( "CDNConnectionFailed", "VersionInfoCheck", strVersionJsonUrl + "_" + mCultureName + "_" + webReqError );

                    if ( AppMgr.Instance.ServerType != AppMgr.eServerType.LIVE ) {
                        MessagePopup.OK( strNoticeText, strPopupText + "\n" + "VIC : " + strVersionJsonUrl + "\n" + webReqError, strBtn, OnMsg_Exit );
                    }
                    else {
                        MessagePopup.OK( strNoticeText, strPopupText, strBtn, OnMsg_Exit );
                    }
                }
                else {
                    if ( AppMgr.Instance.ServerType == AppMgr.eServerType.INTERNAL ) {
                        MessagePopup.OK( strNoticeText, strVersionJsonUrl + "\n" + webReqError, strBtn, () => {
                            ++mCheckRetryCount;
                            retry = true;

                            if ( mWebReq != null ) {
                                mWebReq.Dispose();
                                mWebReq = null;
                            }

                            StartCoroutine( VersionInfoCheck() );
                        } );
                    }
                    else {
                        Firebase.Analytics.FirebaseAnalytics.LogEvent( "CDNConnectionRetry", "VersionInfoCheck", strVersionJsonUrl + "_" + mCultureName + "_" + webReqError );

                        ++mCheckRetryCount;
                        retry = true;
                    }
                }
            }
			else {
				strServerJsonUrl = AppMgr.Instance.GetServerJsonUrl( false );
                mCheckRetryCount = 0;

                StartCoroutine( ServerInfoCheck( true ) );
			}

            if ( mWebReq != null ) {
                mWebReq.Dispose();
                mWebReq = null;
            }
		}

        if ( retry ) {
            yield return mWaitForHalfSec;
            StartCoroutine( VersionInfoCheck() );
        }
    }

    /// <summary>
    /// 서버정보을 얻어와서 확인
    /// </summary>
    IEnumerator ServerInfoCheck( bool retryOverWifi ) {
        TitleProgress = TITLEPROGRESS.LOADASSETBUNDLES;

        mWebReq = UnityWebRequest.Get( strServerJsonUrl );
        mWebReq.certificateHandler = mAcceptAllCertificates;

        yield return mWebReq.SendWebRequest();

        string strPopupText = strConnectionText;
        string strNoticeText = strConnectionTitle;
        string strBtn = strPopupBtn;
        bool server = false;
        bool needUpdateClient = false;
        bool failed = false;
        bool retryWifi = false;
        bool tryReconnect = false;
        bool retryConnect = false;

        if ( mWebReq != null && mWebReq.result == UnityWebRequest.Result.Success ) {
            JSONNode json = JSON.Parse( mWebReq.downloadHandler.text );

            string clientversion = string.Empty;
            string clientversion_message = string.Empty;
            bool clientupdate = false;
            string clientupdate_url = string.Empty;
            string servermessage = string.Empty;

            clientversion = json["client-version"];
            clientversion_message = GetClientVersionMessage( json );
            clientupdate = json["client-update"].AsBool;
            strUpdateAOS = json["client-update-url-aos"];
            strUpdateiOS = json["client-update-url-ios"];
            strUpdateSteam = json["client-update-url-steam"];

            strServerRelocateUpdateAOS = json["client-serverrelocate-update-url-aos"];
            strServerRelocateUpdateiOS = json["client-serverrelocate-update-url-ios"];
            bGoGlobalStore = json["go-global-store"].AsBool;

            IsPVPlay = json["client-pv-play"].AsBool;
            AppMgr.Instance.GcCollectInLobby = json["client-gc-lobby"].AsBool;
            AppMgr.Instance.GcCollectInGame = json["client-gc-game"].AsBool;

            server = json["server"].AsBool;
            servermessage = GetMaintenanceText( json );

            AppMgr.Instance.Review = json["client-review"].AsBool;
            AppMgr.Instance.DisablePurchase = json["client-disable-purchase"].AsBool;
            AppMgr.Instance.DisableCreateNewUser = json["client-disable-create-newuser"].AsBool;
            AppMgr.Instance.DebugInfo = json["client-debuginfo"].AsBool;
            AppMgr.Instance.ServerAddr = json["serveraddr"];
            AppMgr.Instance.ServerPort = json["serverport"].AsInt;

            AppMgr.Instance.TutorialSkipFlag = json["client-tutorial-skip"].AsBool;
            AppMgr.Instance.TutorialBattleHelpFlag = json["client-tutorial-battle-help"].AsBool;

            AppMgr.Instance.Nocheckdid = IsNocheckdid( json );

            if ( !AppMgr.Instance.Nocheckdid ) {
                if ( !server ) {
                    strPopupText = servermessage;
                    failed = true;
                }

                if ( AppMgr.Instance.configData.m_version != clientversion ) //클라 버전이 다른경우 
                {
                    needUpdateClient = clientupdate;    //true 면 앱스토어로 이동 false면 종료
                    failed = true;

                    if ( clientupdate ) {
                        strPopupText = clientversion_message;
                    }
                }
            }
        }
        else {
            failed = true;

            if ( retryOverWifi && ( AppMgr.Instance.ServerType == AppMgr.eServerType.INTERNAL || AppMgr.Instance.ServerType == AppMgr.eServerType.REVIEW ) &&
                mWebReq != null && mWebReq.error.Contains( "403 Forbidden" ) ) {
                retryWifi = true;
            }
            else {
                tryReconnect = true;
            }
        }

        if ( failed && !retryWifi ) {
            if ( !tryReconnect ) {
                if ( needUpdateClient ) {
                    MessagePopup.OK( strNoticeText, strPopupText, strBtn, OnMsg_OpenAppStore );
                }
                else if ( !server ) {
                    MessagePopup.OK( strNoticeText, strPopupText, strBtn, OnMsg_Exit );
                }
            }
            else {
                string webReqError = mWebReq != null ? mWebReq.error : "WebReq is null";

                if ( mCheckRetryCount >= CDN_RETRY_COUNT ) {
                    Firebase.Analytics.FirebaseAnalytics.LogEvent( "CDNConnectionFailed", "ServerInfoCheck", strServerJsonUrl + "_" + mCultureName + "_" + webReqError );

                    if ( AppMgr.Instance.ServerType != AppMgr.eServerType.LIVE ) {
                        MessagePopup.OK( strNoticeText, strPopupText + "\n" + "SIC : " + strServerJsonUrl + "\n" + webReqError, strBtn, OnMsg_Exit );
                    }
                    else {
                        MessagePopup.OK( strNoticeText, strPopupText, strBtn, OnMsg_Exit );
                    }
                }
                else {
                    Firebase.Analytics.FirebaseAnalytics.LogEvent( "CDNConnectionRetry", "ServerInfoCheck", strServerJsonUrl + "_" + mCultureName + "_" + webReqError );
                    if ( AppMgr.Instance.ServerType == AppMgr.eServerType.INTERNAL ) {
                        MessagePopup.OK( strNoticeText, strServerJsonUrl + "\n" + webReqError, strBtn, () => {
                            ++mCheckRetryCount;
                            retryConnect = true;

                            if ( mWebReq != null ) {
                                mWebReq.Dispose();
                                mWebReq = null;
                            }

                            StartCoroutine( ServerInfoCheck( false ) );
                        } );
                    }
                    else {
                        ++mCheckRetryCount;
                        retryConnect = true;
                    }
                }
            }
        }
        else if ( !retryWifi ){
            StartCoroutine( Update_Patch( AppMgr.Instance.GetPatchJsonUrl( false ), true ) );
        }

        if ( mWebReq != null ) {
            mWebReq.Dispose();
            mWebReq = null;
        }

        if ( retryWifi ) {
            strServerJsonUrl = AppMgr.Instance.GetServerJsonUrl( true );
            StartCoroutine( ServerInfoCheck( false ) );
        }
        else if ( retryConnect ) {
            yield return mWaitForHalfSec;
            StartCoroutine( ServerInfoCheck( false ) );
        }
    }

    string GetClientVersionMessage(JSONNode json)
    {
        string str = string.Empty;
        int index = (int)eLanguage;

        JSONArray jsonarray = json["client-version-message"].AsArray;
        if (jsonarray != null)
        {
            if (jsonarray.Count != 0)
            {
                str = jsonarray[index];
            }
        }

        if (str == string.Empty || str == "")
        {
            str = FLocalizeString.Instance.GetText(800);
        }
        return str;
    }
    string GetMaintenanceText(JSONNode json)
    {
#if UNITY_STANDALONE
        if (System.Globalization.CultureInfo.CurrentCulture.Name.Equals("th-TH"))
        {
            //태국어
            new System.Globalization.ThaiBuddhistCalendar();
        }
        else if (System.Globalization.CultureInfo.CurrentCulture.Name.Equals("ps-AF"))
        {
            //파슈토어(아프가니스탄)
            new System.Globalization.HijriCalendar();
        }
#endif

        string str = string.Empty;
        int index = (int)eLanguage;
        int mtype = json["maintenance-type"].AsInt;
        var DateTimest = json["maintenance-starttime"].AsDateTime;  //77788
        var DateTimeed = json["maintenance-endtime"].AsDateTime;    //77788

        JSONArray jsonarray = json["maintenance-text"].AsArray;
        if (jsonarray != null)
        {
            if (jsonarray.Count != 0)
            {
                str = jsonarray[index];
            }
        }

        if (str == string.Empty || str == "")
        {
            str = FLocalizeString.Instance.GetText(800 + mtype);
        }

        str = string.Format(str, GameSupport.GetLocalTimeByUTC0(DateTimest), GameSupport.GetLocalTimeByUTC0(DateTimeed));
        return str;
    }
    bool IsNocheckdid(JSONNode json)
    {
        string strUniqueIdentifier = Platforms.IBase.Inst.GetDeviceUniqueID();

        JSONArray jsonarray = json["nocheckdid"].AsArray;
        if (jsonarray == null)
            return false;
        if (jsonarray.Count == 0)
            return false;

        for (int i = 0; i < jsonarray.Count; i++)
        {
            string str = jsonarray[i];
            if (strUniqueIdentifier == str)
                return true;
        }

        return false;
    }

	//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	//
	//  패치 정보 확인
	//
	//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	IEnumerator Update_Patch( string url, bool retryOverWifi ) {
		//로컬 system, font로드 후, 약관동의 화면 넘어가면서 로컬 번들 먼저 로드
		TitleProgress = TITLEPROGRESS.LOADASSETBUNDLES;
		MaxBundleCount = AssetBundleMgr.Instance.GetLocalBundleCount();
		NowBundleCount = 0;

		_titlepanel.ShowLoadingBar();
		
        // 번들 로드 타입이 믹스일 경우엔 로컬 번들 먼저 읽어옴
		if( AppMgr.Instance.configData.m_resLoadType == Config.eResLoadType.AssetBundle_Mix ) {
			//프로그래스바 카운트 콜백 적용
			yield return StartCoroutine( AssetBundleMgr.Instance.LoadAssetBundleFromFileAsync( () => { NowBundleCount += 1; } ) );
		}
		
        _titlepanel.HideLoadingBar();

		TitleProgress = TITLEPROGRESS.PATCH;

		bool bPatchData = LocalPatchData.IsPatchData();
		LocalPatchData.Init();
		LocalPatchData.LoadPatchData();

		BundleNameList.Clear();
		BundleVersionList.Clear();
		BundleSizeList.Clear();
		BundleNewNameList.Clear();

        while( !Caching.ready ) {
            yield return null;
        }

		WWW www = new WWW(url);
		yield return www;

        bool retry = false;

        if( www.error == null ) {
			var json = JSON.Parse(www.text);
			ServerPatchData.Init();
			ServerPatchData.ParseJson( json );

			_bupdate = false;
			IsUpdateSystemAndUpdateBundle = false;

			if( !bPatchData ) {
				_bupdate = true;
				IsUpdateSystemAndUpdateBundle = true;
			}
			else {
				if( LocalPatchData.mPackageList.Count != ServerPatchData.mPackageList.Count ) {
					_bupdate = true;
					IsUpdateSystemAndUpdateBundle = true;
				}
				else {
					for( int i = 0; i < LocalPatchData.mPackageList.Count; i++ ) {
						if( LocalPatchData.mPackageList[i].mVersion != ServerPatchData.mPackageList[i].mVersion ) {
							_bupdate = true;

							if( LocalPatchData.mPackageList[i].mName.ToLower() == "system" ||
								LocalPatchData.mPackageList[i].mName.ToLower() == "update" ) {
								IsUpdateSystemAndUpdateBundle = true;
							}

							BundleNewNameList.Add( LocalPatchData.mPackageList[i].mName );
						}
					}
				}
			}

			AppMgr.Instance.configData.ClearAssetBundleName();
			for( int i = 0; i < ServerPatchData.mPackageList.Count; i++ ) {
				AppMgr.Instance.configData.AddAssetBundleName( ServerPatchData.mPackageList[i].mName );
				Debug.Log( ServerPatchData.mPackageList[i].mName );
			}

			//버전 체크하여 와이파이확인창 띄움
			//BundleNameList.Add("a");
			//BundleVersionList.Add(1);
			for( int i = 0; i < ServerPatchData.mPackageList.Count; i++ ) {
				BundleNameList.Add( ServerPatchData.mPackageList[i].mName );
				BundleVersionList.Add( ServerPatchData.mPackageList[i].mVersion );
				BundleSizeList.Add( ServerPatchData.mPackageList[i].mSize );
			}

			if( _bupdate ) {
				long totalbyte = 0;
				for( int i = 0; i < ServerPatchData.mPackageList.Count; i++ ) {
					FPatchSaveData.PackageSaveData data = LocalPatchData.mPackageList.Find(x => x.mName == ServerPatchData.mPackageList[i].mName);

                    if( data != null ) {
                        if( data.mVersion != ServerPatchData.mPackageList[i].mVersion ) {
                            totalbyte += ServerPatchData.mPackageList[i].mSize;
                        }
                    }
                    else {
                        totalbyte += ServerPatchData.mPackageList[i].mSize;
                    }
				}

				string str = string.Format(strDataCheckText, GetSize(totalbyte, 1));
				MessagePopup.CYN( strConnectionTitle, str, strPopupConfirmBtn, strPopupCancelBtn, OnMsg_Update, OnMsg_UpdateCancel );
			}
			else {
				StartCoroutine( LoadAssetBundles() );
			}
		}
		else {
            if( retryOverWifi && ( AppMgr.Instance.ServerType == AppMgr.eServerType.INTERNAL || AppMgr.Instance.ServerType == AppMgr.eServerType.REVIEW ) &&
                www.error.Equals( "403 Forbidden" ) ) {
                retry = true;
            }
            else {
                Firebase.Analytics.FirebaseAnalytics.LogEvent( "CDNConnectionFailed_" + mCultureName + "_" + url + "_" + www.error );

                strConnectionText += "\n" + www.error;
                MessagePopup.OK( strConnectionTitle, strConnectionText, strPopupBtn, OnMsg_Exit );
            }
		}

        if( retry  ) {
            StartCoroutine( Update_Patch( AppMgr.Instance.GetPatchJsonUrl( true ), false ) );
		}

		if( www != null ) {
			www.Dispose();
			www = null;
		}
	}

	IEnumerator BeforehandAssetBundles()
    {
        yield return StartCoroutine( InitLocalPushNotification() );

        // 번들 로드 타입이 믹스일 경우엔 로컬 번들 먼저 읽어옴 - system, font만 우선 로드
        if (AppMgr.Instance.configData.m_resLoadType == Config.eResLoadType.AssetBundle_Mix)
        {
            yield return StartCoroutine(AssetBundleMgr.Instance.LoadLocalAssetBundleFromFileAsync(new string[] { "system", "font" }));
        }

        if (PlayerPrefs.HasKey(SAVETYPE.LANGUAGE.ToString()))
        {
            eLANGUAGE currentLanguage = (eLANGUAGE)PlayerPrefs.GetInt(SAVETYPE.LANGUAGE.ToString());
            Log.Show("CurrentLangeage : " + currentLanguage);
            FLocalizeString.Instance.InitLocalize(currentLanguage);
        }

        //Font 셋팅
        AppMgr.Instance.GetConfigFontLoad();

        AssetBundleMgr.Instance.RemoveAssetBundle("system");


        while (!Caching.ready)
            yield return null;

        strConnectionTitle = FLocalizeString.Instance.GetText(911);
        strConnectionText = FLocalizeString.Instance.GetText(912);
        strPopupBtn = FLocalizeString.Instance.GetText(913);
        strPopupConfirmBtn = FLocalizeString.Instance.GetText(914);
        strPopupCancelBtn = FLocalizeString.Instance.GetText(915);
        strDataCheckText = FLocalizeString.Instance.GetText(916);
        strRestartText = FLocalizeString.Instance.GetText(917);
        StrAskRepatch = FLocalizeString.Instance.GetText(918);

        for (int i = 0; i < kAudioClipList.Count; i++)
            _audiosourcelist.Add(null);

        _cinematicpopup = GetUI<UICinematicPopup>("CinematicPopup");
        _titlepanel = GetUI<UITitlePanel>("TitlePanel");
        _titlepanel.Init();

        TitleProgress = TITLEPROGRESS.NONE;
        _titlepanel.SetUIActive(true, true);
        _titlepanel.ShowVersionWithCopyrightLabel();

        //Android에서는 Firebase 초기화 되는데 시간이 걸려서 여기서 하면 에러남.
#if UNITY_IOS
        LocalPushNotificationManager.Instance.RemoveAllNotification();
        LocalPushNotificationManager.Instance.FirebaseSetFCMSubscribe();
#endif
#if UNITY_ANDROID
        if (AppMgr.Instance.IsAndroidPC())
        {
            GPGSInputMgr.Instance.Init();
        }
#endif

        StartCoroutine(NextTermsofUse(true));
    }

    private IEnumerator InitLocalPushNotification() {
        LocalPushNotificationManager.Instance.Init();
        yield return new WaitForSeconds( 0.5f );
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  번들 다운로드 & 로딩
    //
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    IEnumerator LoadAssetBundles()
    {
        TitleProgress = TITLEPROGRESS.LOADASSETBUNDLES;

        if (_bupdate)
        {
            yield return StartCoroutine(LoadSynopsis());

			if (!IsPVPlay)
			{
				MovieState = eMovieState.Synopsis;
				PlaySynopsis();
			}
			else
			{
				MovieState = eMovieState.PV;
				_titlepanel.PlayPV(PlaySynopsis);
			}

            _cinematicpopup.BtnSkip.gameObject.SetActive(true);
        }

        if (!AssetBundleMgr.Instance.IsLoaded())
        {
            _titlepanel.ShowLoadingBar();

            int bundleCount = BundleNameList.Count;

            MaxBundleCount = bundleCount;
            NowBundleCount = 0;

            //yield return new WaitForSeconds(70.0f);
            PlayerPrefs.SetInt(FPatchSaveData.PATCHSAVETYPE.PACKAGECOUNT.ToString(), bundleCount);

            for (int i = 0; i < BundleNewNameList.Count; i++)
            {
                if (Caching.ClearAllCachedVersions(BundleNewNameList[i]))
                {
                    Debug.Log("Caching.ClearAllCachedVersions : " + BundleNewNameList[i]);
                }
                else
                {
                    Debug.Log("Caching.ClearAllCachedVersions Failed : " + BundleNewNameList[i]);
                }
            }

            System.TimeSpan ts = System.TimeSpan.FromTicks(System.DateTime.Now.Ticks);

            for (int i = 0; i < bundleCount; i++)
            {
                yield return StartCoroutine(AssetBundleMgr.Instance.DownloadAssetBundle(BundleNameList[i], BundleVersionList[i]));

                NowBundleCount += 1;

                if (AssetBundleMgr.Instance.Error)
                {
                    TitleUIManager.Instance.HideUI("LoadTipPopup", false);
                    MessagePopup.OK(strConnectionTitle, strConnectionText + "\nError Bundle " + BundleNameList[i], strPopupBtn, OnMsg_Exit);
                    break;
                }

                PlayerPrefs.SetString(FPatchSaveData.PATCHSAVETYPE.PACKAGE_NAME.ToString() + i.ToString(), BundleNameList[i]);
                PlayerPrefs.SetInt(FPatchSaveData.PATCHSAVETYPE.PACKAGE_VERSION.ToString() + i.ToString(), BundleVersionList[i]);
            }

            Log.Show("총 번들 로드 시간 : " + (System.TimeSpan.FromTicks(System.DateTime.Now.Ticks) - ts).TotalSeconds.ToString(), Log.ColorType.Green);
        }

        TitleUIManager.Instance.HideUI("LoadTipPopup", false);
        if (!AssetBundleMgr.Instance.Error)
        {
            GameSupport.SendFireBaseLogEvent(eFireBaseLogType._DownloadBundle);

            ServerPatchData.SavePatchData();

            GameInfo.Instance.LoadTable();
            SoundManager.Instance.LoadTable();

            if (PlayerPrefs.HasKey(SAVETYPE.LANGUAGE.ToString()))
            {
                eLANGUAGE currentLanguage = (eLANGUAGE)PlayerPrefs.GetInt(SAVETYPE.LANGUAGE.ToString());

                FLocalizeString.Instance.InitLocalize(currentLanguage);
                //ScenarioMgr.Instance.InitScenarioMgr(currentLanguage);
            }

            TitleProgress = TITLEPROGRESS.WAIT;
            if (_bupdate)
            {
                //if (!_bsynopsis)
				if(MovieState == eMovieState.None)
                {
                    _titlepanel.HideLoadingBar();
#if UNITY_EDITOR
                    LIAPPManager.Instance.CheckLIAppStart(OnLIAPPSuccess);
#else
#if UNITY_ANDROID
                LIAPPManager.Instance.CheckLIAppStart(OnLIAPPSuccess); 
#elif UNITY_IOS
                LIAPPManager.Instance.CheckLIAppStart(OnLIAPPSuccess); 
#else
                    SteamManager.Instance.OnNet_ReqClientSecurityVerify(OnLIAPPSuccess);
                    //ShowTouchToStart(); 
#endif
#endif
                }
            }
            else
            {
                _titlepanel.HideLoadingBar();
#if UNITY_EDITOR
                LIAPPManager.Instance.CheckLIAppStart(OnLIAPPSuccess);
#else
#if UNITY_ANDROID
                LIAPPManager.Instance.CheckLIAppStart(OnLIAPPSuccess); 
#elif UNITY_IOS
                LIAPPManager.Instance.CheckLIAppStart(OnLIAPPSuccess); 
#else
                SteamManager.Instance.OnNet_ReqClientSecurityVerify(OnLIAPPSuccess);
                //ShowTouchToStart(); 
#endif
#endif
            }
        }
    }


    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  메세지 처리
    //
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void OnMsg_OpenAppStore()
    {
        OnMsg_Exit();

#if UNITY_ANDROID
        if (string.IsNullOrEmpty(strUpdateAOS) == false)
            Application.OpenURL(strUpdateAOS);
#elif UNITY_IOS
        if (string.IsNullOrEmpty(strUpdateiOS) == false)
            Application.OpenURL(strUpdateiOS);
#elif UNITY_STANDALONE
        if (string.IsNullOrEmpty(strUpdateSteam) == false)
        {
            Application.OpenURL(strUpdateSteam);
            Application.Quit();
        }
#endif
    }

    public void OnMsg_Exit()
    {
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
            TitleUIManager.Instance.PlaySound(0);

        //if (_bsynopsis)
		if(MovieState != eMovieState.None)
        {
			OnBtnSkipPV();
			StopSynopsis();

            TitleUIManager.Instance.HideUI("LoadTipPopup", false);
        }

        bloadingscene = false;
        TitleProgress = TITLEPROGRESS.NONE;
        _titlepanel.ShowTouchToStartProgress();
        //Application.Quit();
    }

    void OnMsg_Update()
    {
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
            TitleUIManager.Instance.PlaySound(0);

        for (int i = 0; i < ServerPatchData.mPackageList.Count; i++)
        {
            AssetBundleMgr.Instance.CachingClearWithAssetBundleName(ServerPatchData.mPackageList[i].mName);
        }

        StartCoroutine(LoadAssetBundles());
    }

    void OnMsg_UpdateCancel()
    {
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
            TitleUIManager.Instance.PlaySound(0);

        MessagePopup.OK(strConnectionTitle, strRestartText, strPopupBtn, OnMsg_Exit);
    }

    void OpenAppStore()
    {
        if (Application.isMobilePlatform == false)
            return;

#if UNITY_ANDROID
        AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = activityClass.GetStatic<AndroidJavaObject>("currentActivity");

        if (activity != null)
        {
            activity.Call("openGoogleAppStore");
        }
#endif
    }

    private void ShowTouchToStart()
    {
        //AppMgr.Instance.GetConfigFontLoad();

        TitleProgress = TITLEPROGRESS.WAIT;
        _titlepanel.ShowTouchToStart();
    }

    /*
    public void SetLoadingSceneState(bool state)
    {
        bloadingscene = state;
    }
    */

    private IEnumerator CheckServerAndStart() {
        mWebReq = UnityWebRequest.Get( strServerJsonUrl );
        mWebReq.certificateHandler = mAcceptAllCertificates;

        yield return mWebReq.SendWebRequest();

        string strPopupText = strConnectionText;
        string strNoticeText = strConnectionTitle;
        string strBtn = strPopupBtn;
        bool server = false;
        bool needUpdateClient = false;
        bool failed = false;
        bool tryReconnect = false;
        bool retryConnect = false;

        if ( mWebReq != null && mWebReq.result == UnityWebRequest.Result.Success ) {
            JSONNode json = JSON.Parse( mWebReq.downloadHandler.text );

            string clientversion = string.Empty;
            string clientversion_message = string.Empty;
            bool clientupdate = false;
            string clientupdate_url = string.Empty;
            string servermessage = string.Empty;

            clientversion = json["client-version"];
            clientversion_message = GetClientVersionMessage( json );
            clientupdate = json["client-update"].AsBool;
            strUpdateAOS = json["client-update-url-aos"];
            strUpdateiOS = json["client-update-url-ios"];
            strUpdateSteam = json["client-update-url-steam"];

            strServerRelocateUpdateAOS = json["client-serverrelocate-update-url-aos"];
            strServerRelocateUpdateiOS = json["client-serverrelocate-update-url-ios"];
            bGoGlobalStore = json["go-global-store"].AsBool;

            IsPVPlay = json["client-pv-play"].AsBool;
            AppMgr.Instance.GcCollectInLobby = json["client-gc-lobby"].AsBool;
            AppMgr.Instance.GcCollectInGame = json["client-gc-game"].AsBool;

            server = json["server"].AsBool;
            servermessage = GetMaintenanceText( json );

            AppMgr.Instance.Review = json["client-review"].AsBool;
            AppMgr.Instance.DisablePurchase = json["client-disable-purchase"].AsBool;
            AppMgr.Instance.DisableCreateNewUser = json["client-disable-create-newuser"].AsBool;
            AppMgr.Instance.DebugInfo = json["client-debuginfo"].AsBool;
            AppMgr.Instance.ServerAddr = json["serveraddr"];
            AppMgr.Instance.ServerPort = json["serverport"].AsInt;

            AppMgr.Instance.TutorialBattleHelpFlag = json["client-tutorial-battle-help"].AsBool;

            AppMgr.Instance.Nocheckdid = IsNocheckdid( json );

            if ( !AppMgr.Instance.Nocheckdid ) {
                if ( !server ) {
                    strPopupText = servermessage;
                    failed = true;
                }

                if ( AppMgr.Instance.configData.m_version != clientversion ) //클라 버전이 다른경우 
                {
                    needUpdateClient = clientupdate;    //true 면 앱스토어로 이동 false면 종료
                    failed = true;

                    if ( clientupdate ) {
                        strPopupText = clientversion_message;
                    }
                }
            }
        }
        else {
            failed = true;
            tryReconnect = true;
        }

        if ( failed ) {
            if ( !tryReconnect ) {
                if ( needUpdateClient ) {
                    MessagePopup.OK( strNoticeText, strPopupText, strBtn, OnMsg_OpenAppStore );
                }
                else if ( !server ) {
                    MessagePopup.OK( strNoticeText, strPopupText, strBtn, OnMsg_Exit );
                }
            }
            else {
                string webReqError = mWebReq != null ? mWebReq.error : "WebReq is null";

                if ( mCheckRetryCount >= CDN_RETRY_COUNT ) {
                    Firebase.Analytics.FirebaseAnalytics.LogEvent( "CDNConnectionFailed", "ServerInfoCheck", strServerJsonUrl + "_" + mCultureName + "_" + webReqError );

                    if ( AppMgr.Instance.ServerType != AppMgr.eServerType.LIVE ) {
                        MessagePopup.OK( strNoticeText, strPopupText + "\n" + "SIC : " + strServerJsonUrl + "\n" + webReqError, strBtn, OnMsg_Exit );
                    }
                    else {
                        MessagePopup.OK( strNoticeText, strPopupText, strBtn, OnMsg_Exit );
                    }
                }
                else {
                    Firebase.Analytics.FirebaseAnalytics.LogEvent( "CDNConnectionRetry", "ServerInfoCheck", strServerJsonUrl + "_" + mCultureName + "_" + webReqError );
                    if ( AppMgr.Instance.ServerType == AppMgr.eServerType.INTERNAL ) {
                        MessagePopup.OK( strNoticeText, strServerJsonUrl + "\n" + webReqError, strBtn, () => {
                            ++mCheckRetryCount;
                            retryConnect = true;

                            if ( mWebReq != null ) {
                                mWebReq.Dispose();
                                mWebReq = null;
                            }

                            StartCoroutine( CheckServerAndStart() );
                        } );
                    }
                    else {
                        ++mCheckRetryCount;
                        retryConnect = true;
                    }
                }
            }
        }
        else {
            StartLogin();
        }

        if ( mWebReq != null ) {
            mWebReq.Dispose();
            mWebReq = null;
        }

        if ( retryConnect ) {
            yield return mWaitForHalfSec;
            StartCoroutine( CheckServerAndStart() );
        }
    }

    private void StartLogin() {
        if ( bloadingscene == true )
            return;

        bloadingscene = true;

        FLocalizeString.Instance.InitLocalize( FLocalizeString.Language );

        PlaySound( 0 );

        // 신규 유저 생성 멈춰!
        if ( AppMgr.Instance.DisableCreateNewUser && !GameInfo.Instance.IsAccount() ) {
            ShowDisableCreateNewUserPopup( false );
            return;
        }

        bool bconnect = false;

        if ( NETStatic.Mgr != null ) {
            if ( NETStatic.Mgr.IsConnectingAboutActiveSvr() ) {
                bconnect = true;
            }
        }

        LocalPushNotificationManager.Instance.RemoveAllNotification();
        LocalPushNotificationManager.Instance.FirebaseSetFCMSubscribe();

        _titlepanel.HideTouchToStart();
        _titlepanel.kLoading.SetActive( true );

        if ( bconnect ) {
            GameInfo.Instance.Send_ReqLogin( OnNetLogin );
        }
        else {
            GameInfo.Instance.DoInitGame( AppMgr.Instance.configData.m_Network );
            GameInfo.Instance.SvrConnect_Login( true, OnNetLogin );
        }
    }

    public void OnClick_Start()
    {
#if UNITY_EDITOR
        StartLogin();
#else
        StartCoroutine( CheckServerAndStart() );
#endif
    }

    public void ShowDisableCreateNewUserPopup(bool isConnect)
    {
        UIMessagePopup.OnClickOKCallBack callbackOnNo = null;
        if (isConnect)
        {
            callbackOnNo = GameInfo.Instance.OnMsg_TitleReset;
        }
        else
        {
            callbackOnNo = OnMsg_Exit;
        }

        string title = FLocalizeString.Instance.GetText(1);
        string text = FLocalizeString.Instance.GetText(6020);
        string yes = FLocalizeString.Instance.GetText(6008);
        string no = FLocalizeString.Instance.GetText(1);

        MessagePopup.CYN(title, text, yes, no,
                    () =>
                    {
                        //스토어 안내
                        strUpdateAOS = strServerRelocateUpdateAOS;
                        strUpdateiOS = strServerRelocateUpdateiOS;
                        OnMsg_OpenAppStore();

                        GameInfo.Instance.SetPktInitData();
                        if (GameInfo.Instance.netFlag)
                        {
                            if (NETStatic.Mgr != null)
                                NETStatic.Mgr.DoRelease(eConnectKind.DISCONNECT_LOGOUT);
                        }
                    },
                    callbackOnNo);
    }

    public void OnClick_Progress()
    {
        if (TitleProgress != TITLEPROGRESS.NONE)
            return;
        _titlepanel.HideTouchToStartProgress();
        StartCoroutine(NextTermsofUse(false));
    }


    public void OnNetLogin(int result, PktMsgType pktmsg)
    {
        //서버이전이 등록된 계정 처리        
        if (AppMgr.Instance.configData != null &&
            AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Japan &&
            AppMgr.Instance.HasContentFlag(eContentType.SERVER_RELOCATE))
        {
            // 서버이전이 완료된 계정만 팝업 처리
            if (GameInfo.Instance.IsServerRelocateComplete)
            {
                string title = FLocalizeString.Instance.GetText(1);
                string text = FLocalizeString.Instance.GetText(6007);
                string yes = FLocalizeString.Instance.GetText(6008);
                string no = FLocalizeString.Instance.GetText(1);

                if (bGoGlobalStore)
                {
                    //서버이전 대상자
                    MessagePopup.CYN(title, text, yes, no,
                        () =>
                        {
                            //스토어 안내
                            strUpdateAOS = strServerRelocateUpdateAOS;
                            strUpdateiOS = strServerRelocateUpdateiOS;
                            OnMsg_OpenAppStore();

                            GameInfo.Instance.SetPktInitData();
                            if (GameInfo.Instance.netFlag)
                            {
                                if (NETStatic.Mgr != null)
                                    NETStatic.Mgr.DoRelease(eConnectKind.DISCONNECT_LOGOUT);
                            }
                        },
                        () =>
                        {
                            GameInfo.Instance.SetPktInitData();
                            if (GameInfo.Instance.netFlag)
                            {
                                if (NETStatic.Mgr != null)
                                    NETStatic.Mgr.DoRelease(eConnectKind.DISCONNECT_LOGOUT);
                            }
                            OnMsg_Exit();
                        });
                }
                return;
            }
        }

        OnNetLogin_SERVER_RELOCATE();
    }

    private void OnNetLogin_SERVER_RELOCATE()
    {
        _titlepanel.kLoading.SetActive(false);
        _titlepanel.ShowLoadingBar();
        //_titlepanel.HideTouchToStart();

        bool bstage = false;

        if (GameInfo.Instance.CharList.Count <= 0) //튜토리얼 번호가 0경우
        {
            bstage = true;
            GameInfo.Instance.CharSelete = true;
        }
        else
        {
            if (GameSupport.IsInGameTutorial())
            {
                int state = GameInfo.Instance.UserData.GetTutorialState();
                int stageid = 1;
                if (state == (int)eTutorialState.TUTORIAL_STATE_Init)
                    stageid = 1;
                else if (state == (int)eTutorialState.TUTORIAL_STATE_Stage2Clear)
                    stageid = 2;
                else if (state == (int)eTutorialState.TUTORIAL_STATE_Stage3Clear)
                    stageid = 3;
                GameInfo.Instance.Send_ReqStageStart(stageid, GameInfo.Instance.GetMainCharUID(), 0, false, false, null, OnNetStageStart);
                return;
            }
        }

        if (bstage)
        {
            GameInfo.Instance.SvrConnect_TitleToLobby((int result, PktMsgType pktmsg) =>
            {
                StartTutorialStage(1);
            });
        }
        else
        {
			_titlepanel.Destroy();
            AppMgr.Instance.LoadScene(AppMgr.eSceneType.Lobby, "Lobby");
        }
    }

    public void OnSteamNetGetUserInfoFromAccountLink(int result, PktMsgType pktmsg)
    {
        if (result != (int)Nettention.Proud.ErrorType.Ok)
        {
            return;
        }

        Log.Show("OnSteamNetGetUserInfoFromAccountLink");
        WaitPopup.Hide();

        bloadingscene = false;
        OnClick_Start();
    }

    public void OnNetStageStart(int result, PktMsgType pktmsg)
    {
        int state = GameInfo.Instance.UserData.GetTutorialState();
        int stageid = 1;
        if (state == (int)eTutorialState.TUTORIAL_STATE_Init)
            stageid = 1;
        else if (state == (int)eTutorialState.TUTORIAL_STATE_Stage2Clear)
            stageid = 2;
        else if (state == (int)eTutorialState.TUTORIAL_STATE_Stage3Clear)
            stageid = 3;

        StartTutorialStage(stageid);
    }

    public void StartTutorialStage(int stageId)
    {
        UIValue.Instance.SetValue(UIValue.EParamType.StageID, stageId);

        GameTable.Stage.Param param = GameInfo.Instance.GameTable.FindStage(stageId);

        GameInfo.Instance.SelecteStageTableId = stageId;

        UIValue.Instance.SetValue(UIValue.EParamType.LoadingType, (int)UILoadingPopup.eLoadingType.LobbyToStage);
        UIValue.Instance.SetValue(UIValue.EParamType.LoadingStage, stageId);
        ShowUI("LoadingPopup", false);

		_titlepanel.Destroy();
		AppMgr.Instance.LoadScene(AppMgr.eSceneType.Stage, param.Scene);
    }

    //------------------------------------------------------------------------------------------------------------------------
    //
    //  기기이전 관련
    //
    //------------------------------------------------------------------------------------------------------------------------
    public void PlaySound(int i, bool loop = false)
    {
        if (i < 0)
            return;

        if (i >= kAudioClipList.Count)
        {
            return;
        }

        if (_audiosourcelist[i] == null)
        {
            _audiosourcelist[i] = this.gameObject.AddComponent<AudioSource>();
        }

        _audiosourcelist[i].clip = kAudioClipList[i]; //오디오에 bgm이라는 파일 연결
        //_audiosourcelist[i].volume = 1.0f; //0.0f ~ 1.0f사이의 숫자로 볼륨을 조절
        _audiosourcelist[i].volume = FSaveData.Instance.GetBGVolume();
        _audiosourcelist[i].loop = loop; //반복 여부
        _audiosourcelist[i].mute = false; //오디오 음소거
        _audiosourcelist[i].priority = 0;
        _audiosourcelist[i].Play(); //오디오 재생
    }

    public void StopSound(int i)
    {
        if (i < 0)
            return;

        if (i >= kAudioClipList.Count)
        {
            return;
        }

        if (_audiosourcelist[i] == null)
            return;

        _audiosourcelist[i].Stop();
    }

    public void StopSoundAll()
    {
        for (int i = 0; i < _audiosourcelist.Count; i++)
        {
            if (_audiosourcelist[i] != null)
                _audiosourcelist[i].Stop();
        }
    }

    string GetSize(long inSize, int pi)
    {
        int idx = 0;
        double dSize = inSize;
        string Space = "Byte";
        string returnStr = "";
        //while (dSize > 1024.0)
        //{
        //    dSize /= 1024.0;
        //    mok++;
        //}
        do
        {

            dSize /= 1024.0;
            idx++;
        } while (dSize > 1024.0);
        if (idx == 1)
            Space = "KB";
        else if (idx == 2)
            Space = "MB";
        else if (idx == 3)
            Space = "GB";
        else if (idx == 4)
            Space = "TB";
        if (idx != 0)
            if (pi == 1)
                returnStr = string.Format("{0:F1}{1}", dSize, Space);
            else if (pi == 2)
                returnStr = string.Format("{0:F2}{1}", dSize, Space);
            else if (pi == 3)
                returnStr = string.Format("{0:F3}{1}", dSize, Space);
            else
                returnStr = string.Format("{0}{1}", System.Convert.ToInt32(dSize), Space);
        else
        {
            returnStr = string.Format("{0:F1}{1}", dSize, Space);
        }
        return returnStr;
    }

    IEnumerator LoadSynopsis()
    {
        Object obj = Resources.Load("Title_Global/Title_director/prf_drt_synopsis");
        GameObject gObj = Object.Instantiate(obj, Vector3.zero, Quaternion.identity) as GameObject;
        if (gObj == null)
        {
            Debug.LogError("drt_synopsis를 불러올 수 없습니다.");
            yield return null;
        }

        _DirectorSynopsis = gObj.GetComponent<Director>();
        _DirectorSynopsis.SetCallbackOnEnd2(OnEndSynopsis);

        _DirectorSynopsis.gameObject.SetActive(false);
        yield return null;
    }

    public void PlaySynopsis()
    {
        StopSoundAll();
		//_bsynopsis = true;
		//_bstopsynopsis = false;
        _titlepanel.HideWithoutLoadingBar();
        _DirectorSynopsis.Play();
    }

    private void StopSynopsis()
    {
		//_bstopsynopsis = true;
		MovieState = eMovieState.None;
		_DirectorSynopsis.ForceQuit();
    }

    public void OnEndSynopsis()
    {
		//_bsynopsis = false;
		_titlepanel.ShowHideObjects();
        _cinematicpopup.SetUIActive(false, false);

        //if (_bstopsynopsis)
		if(MovieState == eMovieState.None)
        {
			//_bstopsynopsis = false;
			_titlepanel.HideLoadingBar();
        }
		else if((MovieState == eMovieState.SynopsisOnly || MovieState == eMovieState.PVOnly) && TitleProgress == TITLEPROGRESS.LOADASSETBUNDLES)
		{
			ShowUI("LoadTipPopup", false);
		}
		else
        {
			MovieState = eMovieState.None;

			if (TitleProgress == TITLEPROGRESS.LOADASSETBUNDLES)
            {
                ShowUI("LoadTipPopup", false);
            }
            else
            {
                _titlepanel.HideLoadingBar();
#if UNITY_EDITOR
                LIAPPManager.Instance.CheckLIAppStart(OnLIAPPSuccess);
#else
#if UNITY_ANDROID
                LIAPPManager.Instance.CheckLIAppStart(OnLIAPPSuccess); 
#elif UNITY_IOS
                LIAPPManager.Instance.CheckLIAppStart(OnLIAPPSuccess); 
#else
                //ShowTouchToStart(); 
                SteamManager.Instance.OnNet_ReqClientSecurityVerify(OnLIAPPSuccess);
#endif
#endif
            }
        }

        AppMgr.Instance.CustomInput.ShowCursor(true);
    }

    IEnumerator NextPanelOpen()
    {
        float fAniTime = _titlepanel.GetOpenAniTime();
        yield return new WaitForSeconds(fAniTime * 0.5f);

#if UNITY_EDITOR
        LIAPPManager.Instance.CheckLIAppStart(OnLIAPPSuccess);
#else
#if UNITY_ANDROID
                LIAPPManager.Instance.CheckLIAppStart(OnLIAPPSuccess); 
#elif UNITY_IOS
                LIAPPManager.Instance.CheckLIAppStart(OnLIAPPSuccess); 
#else
                //ShowTouchToStart(); 
                SteamManager.Instance.OnNet_ReqClientSecurityVerify(OnLIAPPSuccess);
#endif
#endif
    }

    public void DeleteLocalPatchData()
    {
        if (LocalPatchData == null)
        {
            return;
        }

        LocalPatchData.DeletePatchData();
    }

    /*
    private string GetPatchRootPath()
    {
        string rootURL = AppMgr.Instance.configData.GetPatchServerAddr() + AppMgr.Instance.PatchFolder;
        //#if UNITY_EDITOR || UNITY_STANDALONE
        //rootURL += "/pc/patch.json";
#if UNITY_ANDROID
        rootURL += "/aos/patch.json?cache=no";
#elif UNITY_IPHONE
        rootURL += "/ios/patch.json?cache=no";
#elif UNITY_STANDALONE_OSX
        rootURL += "/mac/patch.json?cache=no";
#elif UNITY_STANDALONE
        rootURL += "/pc/patch.json?cache=no";
#endif

		return rootURL;
    }
    */

    public void OnNetGetUserInfoFromAccountLink(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        Log.Show("Steam Facebook Account Success");
        WaitPopup.Hide();
        OnClick_Start();
    }

    public void HideLoadingBar()
    {
        if(_titlepanel == null)
        {
            return;
        }

        _titlepanel.HideLoadingBar();
    }

	public void OnBtnPlayPVOnly()
	{
		HideUI("LoadTipPopup", false);

		MovieState = eMovieState.PVOnly;
		_titlepanel.PlayPV(OnEndSynopsis);
	}

	public void OnBtnPlaySynopsisOnly()
	{
		HideUI("LoadTipPopup", false);

		MovieState = eMovieState.SynopsisOnly;
		PlaySynopsis();
	}

	public void OnBtnSkipPV()
	{
		_titlepanel.SkipPV();
	}
}
