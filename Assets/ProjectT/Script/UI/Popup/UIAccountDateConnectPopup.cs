//#define NOT_GPGS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Line.LineSDK;
using System.Text;
using System;
#if UNITY_IOS
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
#endif

public class UIAccountDateConnectPopup : FComponent
{
    [Header("Japan Only")]
    public GameObject kJapanOnlyObj;        //JapanOnly Object Root
    public UIButton kAOSBtn;
    public UIButton kIOSBtn;
    public UILabel kAOSLabel;
    public UILabel kIOSLabel;
    public UILabel kTwitterLabel;
    
    public UILabel kLineLabel;

    public UIButton kAppleBtn;
    public UILabel kAppleLabel;

    [Header("Global")]
    public GameObject kGlobalObj;
    public UIButton kGlobal_AOSBtn;
    public UIButton kGlobal_IOSBtn;
    public UIButton kGlobal_LineBtn;
    public UILabel kGlobal_AOSLabel;
    public UILabel kGlobal_IOSLabel;
    public UILabel kGlobal_TwitterLabel;
    public UILabel kGlobal_LineLabel;
    public UILabel kGlobal_FacebookLabel;
    public UIButton kGlobal_AppleBtn;
    public UILabel kGlobal_AppleLabel;

    [Header("Steam")]
    public GameObject kSteamObj;
    public UILabel kSteam_FacebookLabel;

    public string _linkid;
    public eAccountType _accounttype;

#if UNITY_IOS
    private const string AppleUserIdKey = "AppleUserId";
    private IAppleAuthManager _appleAuthManager;
#endif

    public override void Awake()
    {
        base.Awake();
        if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Japan || AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Global)
        {
            LineSDK.Instance.channelID = AppMgr.Instance.GetLINE_KEY;
            LineSDK.Instance.SetupSDK();
        }
        if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Global)
        {
            FBInitialize();
        }
        if(AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Japan)
        {
            TwitterKit.Unity.Twitter.AwakeInit();
        }

#if UNITY_ANDROID
        GooglePlayGames.BasicApi.PlayGamesClientConfiguration config = new GooglePlayGames.BasicApi.PlayGamesClientConfiguration.Builder().Build();
        GooglePlayGames.PlayGamesPlatform.InitializeInstance(config);
        GooglePlayGames.PlayGamesPlatform.DebugLogEnabled = true;
        GooglePlayGames.PlayGamesPlatform.Activate();
#elif UNITY_IOS
        if (AppleAuthManager.IsCurrentPlatformSupported)
        {
            // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
            var deserializer = new PayloadDeserializer();
            // Creates an Apple Authentication manager with the deserializer
            this._appleAuthManager = new AppleAuthManager(deserializer);
        }
#endif
    }

    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

#if UNITY_IOS
    private void Update() {
        if ( this._appleAuthManager != null ) {
            this._appleAuthManager.Update();
        }
	}
#endif

	public override void InitComponent()
    {
        kJapanOnlyObj.SetActive(false);
        kGlobalObj.SetActive(false);
        kSteamObj.SetActive(false);
        if(AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Japan)
        {
            kJapanOnlyObj.SetActive(true);
#if UNITY_IOS
            kAOSBtn.gameObject.SetActive(false);
            kIOSBtn.gameObject.SetActive(true);
            kAppleBtn.gameObject.SetActive(true);
#elif UNITY_ANDROID
            kAOSBtn.gameObject.SetActive(true);
            kIOSBtn.gameObject.SetActive(false);
            kAppleBtn.gameObject.SetActive(false);
#endif
        }
        else if(AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Global)
        {
            kGlobalObj.SetActive(true);
#if UNITY_IOS
            kGlobal_IOSBtn.gameObject.SetActive(true);
            kGlobal_AOSBtn.gameObject.SetActive(false);
            kGlobal_AppleBtn.gameObject.SetActive(true);
#elif UNITY_ANDROID
            kGlobal_LineBtn.gameObject.SetActive(!AppMgr.Instance.IsAndroidPC());
            kGlobal_IOSBtn.gameObject.SetActive(false);
            kGlobal_AOSBtn.gameObject.SetActive(true);
            kGlobal_AppleBtn.gameObject.SetActive(false);
#endif
        }
        else if(AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam)
        {
            kSteamObj.SetActive(true);
        }

        Renewal(true);
        //1035    연동되지 않음
        //1036    연동 완료
    }


    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        for (int i = 0; i < GameInfo.Instance.UserData.AccountLinkList.Count; i++)
        {
            Debug.Log("### " + GameInfo.Instance.UserData.AccountLinkList[i].Type + " / " + GameInfo.Instance.UserData.AccountLinkList[i].LinkID);
        }

        if(AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Japan)
        {
#if UNITY_IOS
            SetConnectState(eAccountType.GAME_CENTER, kIOSLabel);
            SetConnectState(eAccountType.APPLE, kAppleLabel);
#elif UNITY_ANDROID
            SetConnectState(eAccountType.GOOGLE_PLAY, kAOSLabel);
#endif
            SetConnectState(eAccountType.TWITTER, kTwitterLabel);
            SetConnectState(eAccountType.LINE, kLineLabel);
        }
        else if(AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Global)
        {
#if UNITY_IOS
            SetConnectState(eAccountType.GAME_CENTER, kGlobal_IOSLabel);
            SetConnectState(eAccountType.APPLE, kGlobal_AppleLabel);
#elif UNITY_ANDROID
            SetConnectState(eAccountType.GOOGLE_PLAY, kGlobal_AOSLabel);
#endif
            SetConnectState(eAccountType.TWITTER, kGlobal_TwitterLabel);
            SetConnectState(eAccountType.LINE, kGlobal_LineLabel);
            SetConnectState(eAccountType.FACEBOOK, kGlobal_FacebookLabel);
        }
        else if(AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam)
        {
            SetConnectState(eAccountType.FACEBOOK, kSteam_FacebookLabel);
        }
    }

    private void SetConnectState(eAccountType type, UILabel label)
    {
        var data = GameInfo.Instance.UserData.AccountLinkList.Find(x => x.Type == type);
        
        if(data == null)
        {
            label.textlocalize = FLocalizeString.Instance.GetText(1035);
        }
        else
        {
            label.textlocalize = FLocalizeString.Instance.GetText(1036);

        }
    }

    //이미 연동되어있다면 버튼을 눌러도 동작을 하지 않기위함..
    AccounLinkData GetConnectState(eAccountType type)
    {
        AccounLinkData data = GameInfo.Instance.UserData.AccountLinkList.Find(x => x.Type == type);

        return data;
    }

    public void OnClick_AOSBtn()
    {
        if (GetConnectState(eAccountType.GOOGLE_PLAY) != null)
            return;

        //8889 구글플레이 연동
        SocialLogin();
    }
    public void OnClick_IOSBtn()
    {
        if (GetConnectState(eAccountType.GAME_CENTER) != null)
            return;

        //8889 게임센타 연동
        SocialLogin();
    }
    public void OnClick_TwitterBtn()
    {
        if (AppMgr.Instance.configData.m_ServiceType != Config.eServiceType.Japan)
            return;

        if (GetConnectState(eAccountType.TWITTER) != null)
            return;
        //8889 트위터 연동
        TwitterKit.Unity.Twitter.Init();
        TwitterKit.Unity.TwitterSession session = TwitterKit.Unity.Twitter.Session;
        if (session == null)
        {
            //로그인이 되어있지 않다면 로그인 로직 실행
            TwitterKit.Unity.Twitter.LogIn(TwitterLoginSuccess, TwitterLoginFailed);
        }
        else
        {
            //이미 로그인이 되어있을 경우..
            TwitterLoginSuccess(session);
        }
    }

    public void OnClick_LineBtn()
    {
        if (GetConnectState(eAccountType.LINE) != null)
            return;

        string[] scopes = new string[] { "profile" };
        LineSDK.Instance.Login(null, result =>
        {
            result.Match(
                value => {
                    Debug.Log("Login OK. User display name : " + value.UserProfile.DisplayName);
                    Debug.Log("Login OK. User id : " + value.UserProfile.UserId);
                    _linkid = value.UserProfile.UserId;
                    _accounttype = eAccountType.LINE;
                    GameInfo.Instance.Send_ReqAddLinkAccountAuth(eAccountType.LINE, _linkid, OnNetAddLinkAccountAuth);
                    WaitPopup.Show();
                },
                error => {
                    Debug.Log("Login failed, reason: " + error.Message);
                    MessagePopup.OK(eTEXTID.OK, string.Format(FLocalizeString.Instance.GetText(3090), FLocalizeString.Instance.GetText((int)eTEXTID.LINE)), null);
                }
                );
        });
    }

    public void OnClick_FacebookBtn()
    {
#if !DISABLESTEAMWORKS
        if(AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam)
        {
            FaceAuthWithStandalone.ShowFacebookAuth();
            return;
        }
#endif
        //GlobalOnly
        if (AppMgr.Instance.configData.m_ServiceType != Config.eServiceType.Global)
            return;

        if (GetConnectState(eAccountType.FACEBOOK) != null)
            return;

        if (!Facebook.Unity.FB.IsInitialized)
        {
            FBInitialize();
            return;
        }

        var parm = new List<string>() { "public_profile" };
        Facebook.Unity.FB.LogInWithReadPermissions(parm, FaceAuthCallback);
    }

    private void FaceAuthCallback(Facebook.Unity.ILoginResult result)
    {
        if (result.Error != null)
        {
            MessagePopup.OK(eTEXTID.OK, string.Format(FLocalizeString.Instance.GetText(3090), FLocalizeString.Instance.GetText((int)eTEXTID.FACEBOOK)), null);
            Debug.LogError("Facebook LoginError : " + result.Error);
        }
        else
        {
            if (result.Cancelled)
            {
                MessagePopup.OK(eTEXTID.OK, string.Format(FLocalizeString.Instance.GetText(3090), FLocalizeString.Instance.GetText((int)eTEXTID.FACEBOOK)), null);
                return;
            }
            if (Facebook.Unity.FB.IsLoggedIn)
            {
                
                var facebookToken = Facebook.Unity.AccessToken.CurrentAccessToken;
                if (facebookToken != null)
                {
                    Debug.Log("###Facebook Token UserId : " + facebookToken.UserId);
                    _linkid = facebookToken.UserId;
                    _accounttype = eAccountType.FACEBOOK;
                    GameInfo.Instance.Send_ReqAddLinkAccountAuth(eAccountType.FACEBOOK, _linkid, OnNetAddLinkAccountAuth);
                    WaitPopup.Show();
                }
            }
            else
            {
                MessagePopup.OK(eTEXTID.OK, string.Format(FLocalizeString.Instance.GetText(3090), FLocalizeString.Instance.GetText((int)eTEXTID.FACEBOOK)), null);
            }
        }
    }

    public void OnClick_CloseBtn()
    {
        OnClickClose();
    }

#region GooglePlay_GameCenter_Login
    void SocialLogin()
    {
#if UNITY_EDITOR
        SocialLoginSuccess();
        return;
#endif
        //이미 로그인이 되어있는 경우
        if (Social.localUser.authenticated)
        {
            SocialLoginSuccess();
        }
        else
        {
#if UNITY_ANDROID
            ((GooglePlayGames.PlayGamesPlatform)Social.Active).localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    Debug.Log("!!! Login Success !!!");
                    SocialLoginSuccess();
                }
                else
                {
                    Debug.Log("!!! login failed !!! ");
                    SocialLoginFailed();
                }
            });
#elif UNITY_IOS
            Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                SocialLoginSuccess();
            }
            else
            {
                
                SocialLoginFailed();
            }
        });
#endif
        }
    }
    void SocialLoginSuccess()
    {
#if UNITY_EDITOR
        _linkid = "1";
#else
        _linkid = Social.localUser.id;
#endif

#if UNITY_ANDROID
        _accounttype = eAccountType.GOOGLE_PLAY;
        GameInfo.Instance.Send_ReqAddLinkAccountAuth(eAccountType.GOOGLE_PLAY, _linkid, OnNetAddLinkAccountAuth);
#elif UNITY_IOS
        _accounttype = eAccountType.GAME_CENTER;
        GameInfo.Instance.Send_ReqAddLinkAccountAuth(eAccountType.GAME_CENTER, _linkid, OnNetAddLinkAccountAuth);
#endif

        LobbyUIManager.Instance.Renewal("AccountLinkingPopup");
    }
    void SocialLoginFailed()
    {
#if UNITY_ANDROID
        MessagePopup.OK(eTEXTID.OK, string.Format(FLocalizeString.Instance.GetText(3090), FLocalizeString.Instance.GetText((int)eTEXTID.GOOGLEPLAY)), null);
#elif UNITY_IOS
        MessagePopup.OK(eTEXTID.OK, string.Format(FLocalizeString.Instance.GetText(3090), FLocalizeString.Instance.GetText((int)eTEXTID.GAMECENTER)), null);
#endif
    }
#endregion



#region TwitterLogin
    void TwitterLoginSuccess(TwitterKit.Unity.TwitterSession session)
    {
        Debug.Log(" [Info] : Login success. " + session.authToken);
        Debug.Log(" [LoginID] : " + session.id);

        _linkid = session.id.ToString();
        _accounttype = eAccountType.TWITTER;
        //8889 계정확인 진행
        GameInfo.Instance.Send_ReqAddLinkAccountAuth(eAccountType.TWITTER, _linkid, OnNetAddLinkAccountAuth);
    }

    void TwitterLoginFailed(TwitterKit.Unity.ApiError error)
    {
        Debug.LogError(" [Error] : Login faild code = " + error.code + " msg = " + error.message);
        //{0} 연동에 실패했습니다.
        MessagePopup.OK(eTEXTID.OK, string.Format(FLocalizeString.Instance.GetText(3090), FLocalizeString.Instance.GetText((int)eTEXTID.TWITTER)), null);
    }
#endregion

    public void OnNetAddLinkAccountAuth(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        //{0} 계정이 연동되었습니다.
        MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3095), FLocalizeString.Instance.GetText(300 + (int)_accounttype)));
        OnClick_CloseBtn();
        //Renewal(true);

        LobbyUIManager.Instance.Renewal("AccountLinkingPopup");
    }

    public void OnClick_AppleSignIn()
    {
#if UNITY_IOS
        if (GetConnectState(eAccountType.APPLE) != null)
            return;

        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

        this._appleAuthManager.LoginWithAppleId(
            loginArgs,
            credential =>
            {
                // Obtained credential, cast it to IAppleIDCredential
                var appleIdCredential = credential as IAppleIDCredential;
                if (appleIdCredential != null)
                {
                    // Apple User ID
                    // You should save the user ID somewhere in the device
                    var userId = appleIdCredential.User;
                    PlayerPrefs.SetString(AppleUserIdKey, userId);

                    // Email (Received ONLY in the first login)
                    var email = appleIdCredential.Email;

                    // Full name (Received ONLY in the first login)
                    var fullName = appleIdCredential.FullName;

                    // Identity token
                    var identityToken = Encoding.UTF8.GetString(
                                appleIdCredential.IdentityToken,
                                0,
                                appleIdCredential.IdentityToken.Length);

                    // Authorization code
                    var authorizationCode = Encoding.UTF8.GetString(
                                appleIdCredential.AuthorizationCode,
                                0,
                                appleIdCredential.AuthorizationCode.Length);

                    // And now you have all the information to create/login a user in your system
                    Debug.Log("Apple UserId : " + userId);
                    Debug.Log("Apple Email : " + email);
                    Debug.Log("Apple fullName : " + fullName);
                    Debug.Log("Apple identityToken : " + identityToken);
                    Debug.Log("Apple authorizationCode : " + authorizationCode);

                    _linkid = userId;
                    _accounttype = eAccountType.APPLE;
                    GameInfo.Instance.Send_ReqAddLinkAccountAuth(eAccountType.APPLE, _linkid, OnNetAddLinkAccountAuth);
                    WaitPopup.Show();
                }
            },
            error =>
            {
                // Something went wrong
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                Debug.LogError("AppleSign Erroe : " + authorizationErrorCode);
                MessagePopup.OK(eTEXTID.OK, string.Format(FLocalizeString.Instance.GetText(3090), FLocalizeString.Instance.GetText((int)eTEXTID.APPLE)), null);
            });
#endif
    }

    private void FBInitialize()
    {
        if (!Facebook.Unity.FB.IsInitialized)
        {
            Facebook.Unity.FB.Init(() =>
            {
                if (Facebook.Unity.FB.IsInitialized)
                {
                    Facebook.Unity.FB.ActivateApp();
                }
                else
                {
                    Debug.LogError("Failed Initialize Facebook SDK!!");
                }
            });
        }
        else
        {
            Facebook.Unity.FB.ActivateApp();
        }
    }
}