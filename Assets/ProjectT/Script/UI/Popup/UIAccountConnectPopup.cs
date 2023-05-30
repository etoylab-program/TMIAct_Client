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

public class UIAccountConnectPopup : FComponent
{
    [Header("Japan Only")]
    public GameObject kJapanOnlyObj;
    public UIButton kAOSBtn;
    public UIButton kIOSBtn;
    public UIButton kAppleBtn;

    [Header("Global")]
    public GameObject kGlobalObj;
    public UIButton kGlobalAOSBtn;
    public UIButton kGlobalIOSBtn;
    public UIButton kGlobalAppleBtn;
    public UIButton kGlobalLineBtn;
    [Header("Steam")]
    public GameObject SteamObj;
    public UIButton kSteamBtn;
    public string _linkid;

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
        SteamObj.SetActive(false);

        if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Japan)
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
            kGlobalIOSBtn.gameObject.SetActive(true);
            kGlobalAOSBtn.gameObject.SetActive(false);
            kGlobalAppleBtn.gameObject.SetActive(true);
#elif UNITY_ANDROID

            kGlobalLineBtn.gameObject.SetActive(!AppMgr.Instance.IsAndroidPC());

            kGlobalIOSBtn.gameObject.SetActive(false);
            kGlobalAOSBtn.gameObject.SetActive(true);
            kGlobalAppleBtn.gameObject.SetActive(false);
#endif
        }
        else
        {
            SteamObj.SetActive(true);
            kSteamBtn.gameObject.SetActive(true);


            /*
#if DISABLESTEAMWORKS
            kGlobalObj.SetActive(true);
            kGlobalAppleBtn.gameObject.SetActive(false);
            kGlobalIOSBtn.gameObject.SetActive(false);
            kGlobalAOSBtn.gameObject.SetActive(true);
#endif
*/
        }
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);
    }
    public void OnClick_AOSBtn()
    {
        //8889 계정확인 진행
        SocialLogin();
    }

    public void OnClick_IOSBtn()
    {
        //8889 계정확인 진행
        SocialLogin();
    }
    
    public void OnClick_TwitterBtn()
    {
        if (AppMgr.Instance.configData.m_ServiceType != Config.eServiceType.Japan)
            return;

        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
            TitleUIManager.Instance.PlaySound(0);
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
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
            TitleUIManager.Instance.PlaySound(0);
        string[] scopes = new string[] { "profile", "openid" };
        LineSDK.Instance.Login(null, result =>
        {
            result.Match(
                value => {
                    GameInfo.Instance.StartTimeoutCheckSend(TimeOutCallback);
                    Debug.Log("Login OK. User display name : " + value.UserProfile.DisplayName);
                    Debug.Log("Login OK. User id : " + value.UserProfile.UserId);
                    _linkid = value.UserProfile.UserId;
                    GameInfo.Instance.Send_ReqGetUserInfoFromAccountLink(eAccountType.LINE, _linkid, "", OnNetGetUserInfoFromAccountLink);
                    WaitPopup.Show();
                },
                error => {
                    Debug.Log("Login failed, reason: " + error.Message);
                    MessagePopup.OK(eTEXTID.OK, string.Format(FLocalizeString.Instance.GetText(3090), "LINE"), null);
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
            OnClickClose();
            return;
        }
#endif
        //GlobalOnly
        if (AppMgr.Instance.configData.m_ServiceType != Config.eServiceType.Global)
            return;

        if (!Facebook.Unity.FB.IsInitialized)
        {
            FBInitialize();
            return;
        }

        var parm = new List<string>() { "public_profile" };
        Facebook.Unity.FB.LogInWithReadPermissions(parm, FaceAuthCallback);
    }

    public void OnClick_SteamBtn()
    {
#if !DISABLESTEAMWORKS
        GameInfo.Instance.Send_ReqGetUserInfoFromAccountLink(eAccountType.STEAM, AppMgr.Instance.SteamId.ToString(), "", 
                                                             TitleUIManager.Instance.OnSteamNetGetUserInfoFromAccountLink);
        OnClickClose();
#endif
    }

    private void FaceAuthCallback(Facebook.Unity.ILoginResult result)
    {
        if (result.Error != null)
        {
            Debug.LogError("Facebook LoginError : " + result.Error);
            MessagePopup.OK(eTEXTID.OK, string.Format(FLocalizeString.Instance.GetText(3090), "Facebook"), null);
        }
        else
        {
            if(result.Cancelled)
            {
                MessagePopup.OK(eTEXTID.OK, string.Format(FLocalizeString.Instance.GetText(3090), "Facebook"), null);
                return;
            }

            if (Facebook.Unity.FB.IsLoggedIn)
            {
                var facebookToken = Facebook.Unity.AccessToken.CurrentAccessToken;
                if (facebookToken != null)
                {
                    Debug.Log("###Facebook Token UserId : " + facebookToken.UserId);
                    _linkid = facebookToken.UserId;
                    GameInfo.Instance.StartTimeoutCheckSend(TimeOutCallback);
                    GameInfo.Instance.Send_ReqGetUserInfoFromAccountLink(eAccountType.FACEBOOK, _linkid, "", OnNetGetUserInfoFromAccountLink);
                    WaitPopup.Show();
                }
                else
                    MessagePopup.OK(eTEXTID.OK, string.Format(FLocalizeString.Instance.GetText(3090), "Facebook"), null);
            }
            else
            {
                MessagePopup.OK(eTEXTID.OK, string.Format(FLocalizeString.Instance.GetText(3090), "Facebook"), null);
            }
        }
    }

    public void OnClick_CodeBtn()
    {
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
            TitleUIManager.Instance.PlaySound(0);

        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
            TitleUIManager.Instance.ShowUI("AccountCodePopup", true);
        //else
        //    LobbyUIManager.Instance.ShowUI("AccountCodePopup", true);
    }
    public void OnClick_CancelBtn()
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
        if(Social.localUser.authenticated)
        {
            SocialLoginSuccess();
        }
        else
        {
#if UNITY_ANDROID
#if !NOT_GPGS
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
#endif
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
        GameInfo.Instance.StartTimeoutCheckSend(TimeOutCallback);
#if UNITY_EDITOR
        _linkid = "1";
#else
        _linkid = Social.localUser.id;
#endif
#if UNITY_ANDROID
        GameInfo.Instance.Send_ReqGetUserInfoFromAccountLink(eAccountType.GOOGLE_PLAY, _linkid, "", OnNetGetUserInfoFromAccountLink);
#elif UNITY_IOS
        GameInfo.Instance.Send_ReqGetUserInfoFromAccountLink(eAccountType.GAME_CENTER, _linkid, "", OnNetGetUserInfoFromAccountLink);
#endif
        WaitPopup.Show();
    }
    void SocialLoginFailed()
    {
#if UNITY_ANDROID
        MessagePopup.OK(eTEXTID.OK, string.Format(FLocalizeString.Instance.GetText(3090), "GooglePlay"), null);
#elif UNITY_IOS
        MessagePopup.OK(eTEXTID.OK, string.Format(FLocalizeString.Instance.GetText(3090), "GameCenter"), null);
#endif
    }
#endregion

#region TwitterLogin
    void TwitterLoginSuccess(TwitterKit.Unity.TwitterSession session)
    {
        Debug.Log(" [Info] : Login success. " + session.authToken);
        Debug.Log(" [LoginID] : " + session.id);

        _linkid = session.id.ToString();

        GameInfo.Instance.StartTimeoutCheckSend(TimeOutCallback);
        //8889 계정확인 진행
        GameInfo.Instance.Send_ReqGetUserInfoFromAccountLink(eAccountType.TWITTER, _linkid, "", OnNetGetUserInfoFromAccountLink);
        WaitPopup.Show();
    }

    void TwitterLoginFailed(TwitterKit.Unity.ApiError error)
    {
        Debug.LogError(" [Error] : Login faild code = " + error.code + " msg = " + error.message);
        MessagePopup.OK(eTEXTID.OK, string.Format(FLocalizeString.Instance.GetText(3090), "Twitter"), null);
    }
#endregion

#region LineLogin

#endregion

    public void OnNetGetUserInfoFromAccountLink(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;
        Log.Show("OnNetGetUserInfoFromAccountLink");
        GameSupport.SendFireBaseLogEvent(eFireBaseLogType._Prev_Device);
        Renewal(true);
        OnClickClose();
        WaitPopup.Hide();
        TitleUIManager.Instance.OnClick_Start();
    }

    void TimeOutCallback()
    {
        //연결된 Login 서버 연결 끊기.
        NETStatic.Mgr.DoDisConnectByActiveNet();
        Renewal(true);
        OnClickClose();
        WaitPopup.Hide();
        MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(110050), null);
    }

    public void OnClick_AppleSignIn()
    {
#if UNITY_IOS
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

                    GameInfo.Instance.StartTimeoutCheckSend(TimeOutCallback);
                    //8889 계정확인 진행
                    GameInfo.Instance.Send_ReqGetUserInfoFromAccountLink(eAccountType.APPLE, _linkid, "", OnNetGetUserInfoFromAccountLink);
                    WaitPopup.Show();
                }
            },
            error =>
            {
        // Something went wrong
        var authorizationErrorCode = error.GetAuthorizationErrorCode();
                Debug.LogError("AppleSign Erroe : " + authorizationErrorCode);
        MessagePopup.OK(eTEXTID.OK, string.Format(FLocalizeString.Instance.GetText(3090), "Apple"), null);
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
