using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class FaceAuthWithStandalone {
    public static void ShowFacebookAuth() {
#if !DISABLESTEAMWORKS
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
            LobbyUIManager.Instance.ShowUI("FacebookLoginWebViewPopup", true);
        else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
            TitleUIManager.Instance.ShowUI("FacebookLoginWebViewPopup", true);
        else {
            GameObject uiRoot = GameObject.FindObjectOfType<UIRoot>().gameObject;
            UIFacebookLoginWebViewPopup popup = uiRoot.transform.Find("FacebookLoginWebViewPopup").GetComponent<UIFacebookLoginWebViewPopup>();
            popup.SetUIActive(true);
        }
#endif
    }
}

public class UIFacebookLoginWebViewPopup : FComponent {
    [SerializeField] private GameObject _WebViewObject;
    [SerializeField] private GameObject _WebKeyBoard;

    public override void OnEnable() {
        Type = TYPE.Popup;

        base.OnEnable();
        InitComponent();
    }

    public override void InitComponent() {
        AppMgr.Instance.CustomInput.UseCustomCursor(false);
        //Cursor.visible = true; EmbeddedBrowser 웹뷰는 자체 마우스를 보여주므로 안켜줘도 됨.

#if !DISABLESTEAMWORKS
        //브라우저를 찾아준다.
        ZenFulcrum.EmbeddedBrowser.Browser _WebViewBrowser = _WebViewObject.GetComponent<ZenFulcrum.EmbeddedBrowser.Browser>();

        //스팀 덱일때만 키보드를 노출한다.
        _WebKeyBoard.SetActive(Steamworks.SteamUtils.IsSteamRunningOnSteamDeck());

        //브라우저 주소가 변경되면 호출 될 콜백 설정
        _WebViewBrowser.onNavStateChange += () => {

            //Debug.LogWarning($"Changed URL : {webViewBrowser.Url}");
            string[] urlCheck = _WebViewBrowser.Url.Split('?');

            if (urlCheck != null && urlCheck.Length > 0) {
                if (urlCheck[0].Equals("https://www.facebook.com/connect/login_success.html")) {
                    for (int i = 0; i < urlCheck.Length; i++)
                        Debug.LogWarning(i + " / " + urlCheck[i]);

                    StartCoroutine(GetFBToken(urlCheck));
                    return;
                }
            }
        };

        string cliend_id = "client_id=1036111603509822";
        string redirect_uri = "&redirect_uri=https://www.facebook.com/connect/login_success.html";
        //string state = "&state=\"{st=state123abc,ds=123456789}\"";
        string url = "https://www.facebook.com/v8.0/dialog/oauth?" + cliend_id + redirect_uri + "&auth_type=rerequest";// &scope=profile";

        _WebViewBrowser.Url = url;
#endif
    }

#if !DISABLESTEAMWORKS

    IEnumerator GetFBToken(string[] parameter) {
        Debug.Log("GetFBToken");

        string fbAppID = "1036111603509822";

        string url = "https://graph.facebook.com/v8.0/oauth/access_token?" + "client_id=" + fbAppID + "&redirect_uri=" + parameter[0] + "&client_secret=" + AppMgr.Instance.FBSCode + "&" + parameter[1];
        Debug.Log(url);
        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.error != null) {
            Debug.LogError(www.error);
            yield break;
        }

        Debug.Log(www.downloadHandler.text);

        JSONNode json = JSON.Parse(www.downloadHandler.text);

        string access_token = json["access_token"];
        string token_type = json["token_type"];
        string expires_in = json["expires_in"];

        yield return new WaitForSeconds(0.1f);
        StartCoroutine(GetCheckFBToken(access_token));
    }

    IEnumerator GetCheckFBToken(string token) {
        Debug.Log("GetCheckFBToken");

        //string url = "https://graph.facebook.com/debug_token?" + "input_token=" + token + "&access_token=" + token;
        //string url = string.Format("https://graph.facebook.com/v8.0/debug_token?input_token={0}&access_token={1}", token, token);
        string url = string.Format("https://graph.facebook.com/me?access_token={0}", token);


        Debug.Log(url);
        UnityWebRequest www = UnityWebRequest.Get(new System.Uri(url));

        yield return www.SendWebRequest();

        if (www.error != null) {
            Debug.LogError(www.error);
            Debug.Log(www.downloadHandler.text);
            yield break;
        }

        Debug.Log(www.downloadHandler.text);

        JSONNode json = JSON.Parse(www.downloadHandler.text);
        //string linkId = json["data"]["user_id"];
        string linkId = json["id"];
        Debug.Log("Steam Facebook Account UserID : " + linkId);
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title) {
            GameInfo.Instance.StartTimeoutCheckSend(TimeOutCallback);
            GameInfo.Instance.Send_ReqGetUserInfoFromAccountLink(eAccountType.FACEBOOK, linkId, "", TitleUIManager.Instance.OnNetGetUserInfoFromAccountLink);
        }
        else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby) {
            GameInfo.Instance.Send_ReqAddLinkAccountAuth(eAccountType.FACEBOOK, linkId, OnNetAddLinkAccountAuth);
        }
        WaitPopup.Show();

        OnClickClose();
    }

    void TimeOutCallback() {
        //연결된 Login 서버 연결 끊기.
        NETStatic.Mgr.DoDisConnectByActiveNet();
        Renewal(true);
        OnClickClose();
        WaitPopup.Hide();
        MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(110050), null);
    }

    public void OnNetAddLinkAccountAuth(int result, PktMsgType pktmsg) {
        if (result != 0)
            return;

        //{0} 계정이 연동되었습니다.
        MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3095), FLocalizeString.Instance.GetText(313)));

        GameInfo.Instance.Send_ReqLinkAccountList((int r, PktMsgType p) => {
            var ui = LobbyUIManager.Instance.GetUI("AccountDateConnectPopup");
            ui?.Renewal();
        });

        OnClickClose();
    }
#endif


    public void OnClick_CloseBtn() {
        OnClickClose();
    }

    public void OnClick_BackBtn() {
        OnClickClose();
    }

    public override void OnClickClose() {
        AppMgr.Instance.CustomInput.UseCustomCursor(true);
        StopAllCoroutines();

        base.OnClickClose();
    }
}