
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;


public class UITitlePanel : FComponent
{
	public GameObject[] HideObjectsWithoutLoadingBar;

    public UILabel kCopyrightLabel;
    public UISprite kTouchtoStartBGSpr;
    public UISprite kLoadingBarBGSpr;
    public UISprite kLoadingBarSpr;
    public UILabel kLoadingLabel;
    public UILabel kVerLabel;
    public UILabel lbUID;
    public UIButton kBtnProgress;
    public UIButton kBtnStart;
    public UIButton kAccountConnectBtn;
    public UIButton kRePatchBtn;
    public UILabel kAccountConnectLabel;
    public GameObject kLoading;

    [Header("[Title]")]
    public GameObject       BIJapan;
    public GameObject       BIGlobal;
    public List<GameObject> ListCharAOS;
    public List<GameObject> ListCharIOS;

	[Header("[Logo]")]
	public UITexture	Logo;
	public Texture2D	TexLogo;
	public Texture2D	TexLogoJpn;

	//public List<GameObject> kIOSList;
	//public List<GameObject> kAOSList;

	[Header("[RePatch]")]
    public GameObject DisableOnClearCache;
    public UIButton BtnRePatch;

    [Header("[ServerRelocate]")]
    public GameObject kServerRelocateObj;

	[Header("[PV]")]
	public UISprite		SprBgPV;
	public UITexture	TexPV;
	public UIButton		BtnSkipPV;

	private int[]       mHiddenCommand      = {0, 0, 1, 0, 1, 1, 1};
    private List<int>   mListHiddenCommand  = new List<int>();
    private bool        mbRightCommand      = true;

    private eLANGUAGE elanguage = eLANGUAGE.JPN;

    private string str921 = "";
    private string str922 = "";
    private string str923 = "";
    private string str924 = "";

	private VideoClip		mPV					= null;
	private VideoPlayer		mVideoPlayer		= null;
	private RenderTexture	mRTVideo			= null;
	private System.Action	mOnEndPVCallback	= null;
    private UIAnchor[]      mChildAnchors       = null;
    private float           mFixAnchorMaxTime   = 5.0f;


	public override void Awake() {
		base.Awake();

		mChildAnchors = this.gameObject.GetComponentsInChildren<UIAnchor>();
	}

	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}

	public void Start() {
		InitPV();

		StartCoroutine( nameof( FixChildAnchors ) );
	}

	public void Destroy()
	{
        if( mPV ) {
            mVideoPlayer.clip = null;
            mRTVideo = null;
            TexPV.mainTexture = null;
            mVideoPlayer.targetTexture = null;

            Resources.UnloadAsset( mPV );
            mPV = null;
        }

		Resources.UnloadUnusedAssets();
	}

	public override void InitComponent() {
		DisableOnClearCache.SetActive( false );

		elanguage = (eLANGUAGE)PlayerPrefs.GetInt( SAVETYPE.LANGUAGE.ToString() );
		kLoadingLabel.textlocalize = "";

		BIJapan.SetActive( false );
		BIGlobal.SetActive( false );

		if ( AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Japan ) {
			BIJapan.SetActive( true );
		}
		else {
			BIGlobal.SetActive( true );
		}

		for ( int i = 0; i < ListCharAOS.Count; i++ ) {
			ListCharAOS[i].SetActive( false );
		}

		for ( int i = 0; i < ListCharIOS.Count; i++ ) {
			ListCharIOS[i].SetActive( false );
		}

		if ( AppMgr.Instance.ResPlatform == AppMgr.eResPlatform.aos ) {
			for ( int i = 0; i < ListCharAOS.Count; i++ ) {
				ListCharAOS[i].SetActive( true );
			}
		}
		else {
			for ( int i = 0; i < ListCharIOS.Count; i++ ) {
				ListCharIOS[i].SetActive( true );
			}
		}

		lbUID.gameObject.SetActive( false );
		mListHiddenCommand.Clear();
		mListHiddenCommand.AddRange( mHiddenCommand );
		mbRightCommand = true;
	}

	public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        str921 = FLocalizeString.Instance.GetText(921);
        str922 = FLocalizeString.Instance.GetText(922);
        str923 = FLocalizeString.Instance.GetText(923);
        str924 = FLocalizeString.Instance.GetText(924);
    }

    //로컬 번들 로드 후 - 버전, 화면 우하단에 만든회사 라벨 보이게하기
    public void ShowVersionWithCopyrightLabel()
    {
        kVerLabel.textlocalize = "ver " + AppMgr.Instance.configData.m_version;

#if !UNITY_EDITOR
        if(AppMgr.Instance.configData.m_debugBuild)
            kVerLabel.textlocalize = "ver " + AppMgr.Instance.configData.m_version + " / " + AppMgr.Instance.configData.m_dateVersion;
#endif

        kCopyrightLabel.gameObject.SetActive(true);

		eLANGUAGE currentLanguage = (eLANGUAGE)PlayerPrefs.GetInt(SAVETYPE.LANGUAGE.ToString());
		if (currentLanguage == eLANGUAGE.JPN)
		{
			Logo.mainTexture = TexLogoJpn;
		}
		else
		{
			Logo.mainTexture = TexLogo;
		}
	}
    
    void Update()
    {
		if (kLoadingBarBGSpr.gameObject.activeSelf)
        {

            if (AssetBundleMgr.Instance.LoadStatus != AssetBundleMgr.eLoadStatus.None)//AssetBundleMgr.Instance.wwwProgress != null)
            {
                int displayNowBundleCount = TitleUIManager.Instance.NowBundleCount + 1;

                if (TitleUIManager.Instance.NowBundleCount == TitleUIManager.Instance.MaxBundleCount)
                {
                    kLoadingBarSpr.fillAmount = 1.0f;
                }
                else
                {
                    float f = (float)TitleUIManager.Instance.NowBundleCount / (float)TitleUIManager.Instance.MaxBundleCount;
                    float fc = 1.0f / (float)TitleUIManager.Instance.MaxBundleCount;
                    kLoadingBarSpr.fillAmount = f + (fc * AssetBundleMgr.Instance.GetProgress()); 
                }

                if (TitleUIManager.Instance.IsUpdate)
                {
                    if (TitleUIManager.Instance.NowBundleCount == TitleUIManager.Instance.MaxBundleCount)
                    {
                        kLoadingLabel.textlocalize = str921;
                    }
                    else
                    {
                        kLoadingLabel.textlocalize = string.Format(str922, displayNowBundleCount, TitleUIManager.Instance.MaxBundleCount);
                    }
                }
                else
                {
                    kLoadingLabel.textlocalize = string.Format(str923, displayNowBundleCount, TitleUIManager.Instance.MaxBundleCount);
                }
            }

            if (AppMgr.Instance.Async != null)
            {
                kLoadingBarSpr.fillAmount = AppMgr.Instance.Async.progress;
                kLoadingLabel.textlocalize = str924;
            }
        }
    }

    public void Init()
    {
        kTouchtoStartBGSpr.gameObject.SetActive(false);
        kBtnStart.gameObject.SetActive(false);
        kBtnProgress.gameObject.SetActive(false);
        kLoadingBarBGSpr.gameObject.SetActive(false);
        kLoading.SetActive(false);
        kLoadingBarSpr.fillAmount = 0.0f;
        kServerRelocateObj.SetActive(false);
    }

    public void ShowTouchToStart()
    {
        kTouchtoStartBGSpr.gameObject.SetActive(true);
        kBtnStart.gameObject.SetActive(true);
        kBtnProgress.gameObject.SetActive(false);
        
        BtnRePatch.gameObject.SetActive(true);
        kLoading.SetActive(false);

        /*/STEAM
#if !DISABLESTEAMWORKS
        if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam)
        {
            kAccountConnectBtn.gameObject.SetActive(false);
            kRePatchBtn.transform.localPosition = kAccountConnectBtn.transform.localPosition;
        }
        else
#endif
*/
        {
            kAccountConnectBtn.gameObject.SetActive(true);
            kRePatchBtn.gameObject.SetActive(true);
        }


        SetServerRelocate();
    }

    public void HideTouchToStart()
    {
        kTouchtoStartBGSpr.gameObject.SetActive(false);
        kAccountConnectBtn.gameObject.SetActive(false);
        BtnRePatch.gameObject.SetActive(false);
        kRePatchBtn.gameObject.SetActive(false);
        kBtnStart.gameObject.SetActive(false);
        kBtnProgress.gameObject.SetActive(false);
        kLoading.SetActive(false);

        if (kServerRelocateObj)
        {
            kServerRelocateObj.SetActive(false);
        }
    }

    public void ShowTouchToStartProgress()
    {
        kTouchtoStartBGSpr.gameObject.SetActive(true);
        kBtnStart.gameObject.SetActive(false);
        kBtnProgress.gameObject.SetActive(true);
        kLoading.SetActive(false);
    }

    public void HideTouchToStartProgress()
    {
        kTouchtoStartBGSpr.gameObject.SetActive(false);
        kBtnStart.gameObject.SetActive(false);
        kBtnProgress.gameObject.SetActive(false);
        kLoading.SetActive(false);
    }


    public void ShowLoadingBar()
    {
        kLoadingBarBGSpr.gameObject.SetActive(true);
        kLoading.SetActive(false);
    }

    public void HideLoadingBar()
    {
        kLoadingBarBGSpr.gameObject.SetActive(false);
        kLoading.SetActive(false);
    }

    public void OnClick_Progress()
    {
        TitleUIManager.Instance.PlaySound(0);
        TitleUIManager.Instance.OnClick_Progress();
    }

    public void OnClick_StartBtn()
    {
        TitleUIManager.Instance.PlaySound(0);
        TitleUIManager.Instance.OnClick_Start();        
    }

    public void OnClick_AccountConnectBtn()
    {
        TitleUIManager.Instance.PlaySound(0);
        bool bconnect = false;

        if (NETStatic.Mgr != null)
        {
            if (NETStatic.Mgr.IsConnectingAboutActiveSvr())
            {
                bconnect = true;
            }
        }

        if (bconnect)
        {
            TitleUIManager.Instance.ShowUI("AccountConnectPopup", true);
        }
        else
        {
            GameInfo.Instance.DoInitGame(AppMgr.Instance.configData.m_Network);
            GameInfo.Instance.SvrConnect_Login(false, OnNetSvrConnect_Login);

        }
    }

    public void OnClick_RePatchBtn()
    {
        MessagePopup.OKCANCEL(eTEXTID.OK, TitleUIManager.Instance.StrAskRepatch, StartRePatch);
    }

	private void StartRePatch() {
		DisableOnClearCache.SetActive( true );

		TitleUIManager.Instance.DeleteLocalPatchData();
		SoundManager.Instance.Release();
		AssetBundleMgr.Instance.Release();

		if ( Caching.ClearCache() ) {
			Debug.Log( "Clear Cache" );
		}
		else {
			Debug.Log( "Clear Cache Failed" );
		}

		AppMgr.Instance.LoadScene( AppMgr.eSceneType.Title, "Title", true );
	}

	public void OnNetSvrConnect_Login(int result, PktMsgType pktmsg)
    {
        if (Nettention.Proud.ErrorType.Ok == (Nettention.Proud.ErrorType)result)
        {
            TitleUIManager.Instance.ShowUI("AccountConnectPopup", true);
        }
    }

    public void OnNetSvrConnect_Login_ServerRelocate(int result, PktMsgType pktmsg)
    {
        if (Nettention.Proud.ErrorType.Ok == (Nettention.Proud.ErrorType)result)
        {
            TitleUIManager.Instance.ShowUI("ServerIDTitlePopup", true);
        }
    }

    public void HideWithoutLoadingBar()
    {
        for(int i = 0; i < HideObjectsWithoutLoadingBar.Length; i++)
        {
            HideObjectsWithoutLoadingBar[i].SetActive(false);
        }
    }

    public void ShowHideObjects()
    {
        for (int i = 0; i < HideObjectsWithoutLoadingBar.Length; i++)
        {
            HideObjectsWithoutLoadingBar[i].SetActive(true);
        }
    }

    public void OnAniEvent_Sound( int index )
    {
        TitleUIManager.Instance.PlaySound(4);
        TitleUIManager.Instance.PlaySound(5, true);
        TitleUIManager.Instance.PlaySound(6);
    }

    public void OnBtnHidden01()
    {
        if(!mbRightCommand)
        {
            return;
        }

        if(mListHiddenCommand[0] == 0)
        {
            mListHiddenCommand.RemoveAt(0);
            if(mListHiddenCommand.Count <= 0)
            {
                lbUID.gameObject.SetActive(true);
                lbUID.textlocalize = Platforms.IBase.Inst.GetDeviceUniqueID();
            }
        }
        else
        {
            mbRightCommand = false;
        }
    }

    public void OnBtnHidden02()
    {
        if (!mbRightCommand)
        {
            return;
        }

        if (mListHiddenCommand[0] == 1)
        {
            mListHiddenCommand.RemoveAt(0);
            if (mListHiddenCommand.Count <= 0)
            {
                lbUID.gameObject.SetActive(true);
                lbUID.textlocalize = Platforms.IBase.Inst.GetDeviceUniqueID();
            }
        }
        else
        {
            mbRightCommand = false;
        }
    }

    public void SetServerRelocate()
    {
        kServerRelocateObj.SetActive(false);

        if (AppMgr.Instance.configData == null ||
            AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Japan)
            return;

        if (!AppMgr.Instance.HasContentFlag(eContentType.SERVER_RELOCATE))
            return;

        //kServerRelocateObj.SetActive(true);
    }

    public void OnClick_ServerRelocate()
    {
        Debug.Log("OnClick_ServerRelocate");

        bool bconnect = false;

        if (NETStatic.Mgr != null)
        {
            if (NETStatic.Mgr.IsConnectingAboutActiveSvr())
            {
                bconnect = true;
            }
        }

        if (bconnect)
        {
            TitleUIManager.Instance.ShowUI("ServerIDTitlePopup", true);
        }
        else
        {
            GameInfo.Instance.DoInitGame(AppMgr.Instance.configData.m_Network);
            GameInfo.Instance.SvrConnect_Login(false, OnNetSvrConnect_Login_ServerRelocate);

        }
    }

	public void PlayPV(System.Action onEndCallback)
	{
		SprBgPV.gameObject.SetActive(true);
		BtnSkipPV.gameObject.SetActive(true);

		mVideoPlayer.Play();
		mOnEndPVCallback = onEndCallback;

		StopCoroutine("CheckPV");
		StartCoroutine("CheckPV");
	}

	public void SkipPV()
	{
		StopCoroutine("CheckPV");

		if(TitleUIManager.Instance.TitleProgress != TitleUIManager.TITLEPROGRESS.LOADASSETBUNDLES)
		{
			mOnEndPVCallback = TitleUIManager.Instance.OnEndSynopsis;
		}

		StopPV();
	}

	private void InitPV()
	{
        mPV = Resources.Load( "Title_Global/Title_director/intro_movie" ) as VideoClip;
        if( !mPV ) {
            return;
		}//

        UIRoot uiroot = FindObjectOfType<UIRoot>();
		if (uiroot != null)
		{
			uiroot.fitHeight = ((float)Screen.width / (float)Screen.height) >= 2.26f;
		}

		mVideoPlayer = TexPV.GetComponent<VideoPlayer>();
		mRTVideo = new RenderTexture((int)mPV.width, (int)mPV.height, TexPV.depth);

		int width = (int)((float)Screen.width * 1.5f);
		int height = (int)((float)Screen.height * 1.5f);

		SprBgPV.width = width;
		SprBgPV.height = height;

		BoxCollider boxCol = SprBgPV.GetComponent<BoxCollider>();
		boxCol.size = new Vector3(width, height, 1.0f);

		TexPV.mainTexture = mRTVideo;

		mVideoPlayer.clip = mPV;
		mVideoPlayer.targetTexture = mRTVideo;

		SprBgPV.gameObject.SetActive(false);
		BtnSkipPV.gameObject.SetActive(false);
	}

	private IEnumerator CheckPV()
	{
        if( mPV == null ) {
            yield break;
		}

		bool log = false;

		float checkTime = 0.0f;
		while (checkTime < mPV.length)
		{
			checkTime += Time.deltaTime;
			if (!log && checkTime >= mPV.length * 0.5)
			{
				Firebase.Analytics.FirebaseAnalytics.LogEvent("Wathcing PV");
				log = true;
			}

			yield return null;
		}

		StopPV();
	}

	private void StopPV()
	{
        if( mPV == null ) {
            return;
        }

        mVideoPlayer.Stop();

		SprBgPV.gameObject.SetActive(false);
		BtnSkipPV.gameObject.SetActive(false);

		if (TitleUIManager.Instance.TitleProgress != TitleUIManager.TITLEPROGRESS.LOADASSETBUNDLES)
		{
			mOnEndPVCallback = TitleUIManager.Instance.OnEndSynopsis;
		}

		mOnEndPVCallback?.Invoke();
	}

	private IEnumerator FixChildAnchors() {
		if ( mChildAnchors == null ) {
			yield break;
		}

		WaitForFixedUpdate waitforFixedUpdate = new WaitForFixedUpdate();
		float waitTime = 0.0f;
		while ( waitTime < mFixAnchorMaxTime ) {
			waitTime += Time.fixedDeltaTime;

			for ( int i = 0; i < mChildAnchors.Length; i++ ) {
				mChildAnchors[i].enabled = true;
			}

			yield return waitforFixedUpdate;
		}
	}
}
