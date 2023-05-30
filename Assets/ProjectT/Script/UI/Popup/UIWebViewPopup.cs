using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIWebViewPopup : FComponent
{
	public UIButton kCloseBtn;
	public UILabel kTitleLabel;
    public GameObject kWebViewGO;
    public UniWebView kWebView;
    //public WebViewObject kWebView;
    public Camera kUICamera;
    public GameObject kWebViewSObj;
    public GameObject kWebViewEObj;

    public UISprite SprYouTubeNotice;


	public override void OnEnable()
	{
        base.OnEnable();
        InitComponent();
    }
 
	public override void InitComponent()
	{
        string strtitle = string.Empty;
        string strurl = string.Empty;
        
        var objtitle = UIValue.Instance.GetValue(UIValue.EParamType.WebViewTitle);
        if (objtitle != null)
            strtitle = (string)objtitle;
        var objaddr = UIValue.Instance.GetValue(UIValue.EParamType.WebViewAddr);
        if (objaddr != null)
            strurl = (string)objaddr;

        kTitleLabel.textlocalize = strtitle;

        SprYouTubeNotice.gameObject.SetActive(GameInfo.Instance.IsNewYouTubeLink());

#if UNITY_EDITOR
        return;
#endif
        if (kWebView == null)
        {
            GameObject webTemp = new GameObject("UniWebViewObj");
            webTemp.transform.parent = kWebViewGO.transform;
            webTemp.transform.localPosition = Vector3.zero;
            webTemp.transform.localRotation = Quaternion.identity;
            webTemp.transform.localScale = Vector3.one;

            kWebView = webTemp.AddComponent<UniWebView>();

            //kWebView.Frame = new Rect(sx, sy, rw, rh);
            StartCoroutine(LoadURLWaitForSeconds(strurl, 0.5f));
        }
    }

    private IEnumerator LoadURLWaitForSeconds(string strURL, float waitTime = 0.5f)
    {
        yield return new WaitForSeconds(waitTime);
        LoadURL(strURL, new Rect(50, 150, AppMgr.Instance.DeviceWidth - 100, AppMgr.Instance.DeviceHeight - 200));
    }

    private void LoadURL(string strURL, Rect rc)
    {
        if (kWebView == null)
            return;

        kWebView.SetHorizontalScrollBarEnabled(false);
        kWebView.OnShouldClose += (view) => {

            OnClickClose();
            return true;
        };
        kWebView.BackgroundColor = Color.black;
        kWebView.SetHorizontalScrollBarEnabled(false);

        /*
        if ((AppMgr.Instance.DeviceWidth * 0.5f) > 1280)
        {
            kWebView.Frame = new Rect(50, 150, (NGUITools.screenSize.x * 2f) - 100, (NGUITools.screenSize.y * 2f) - 200);
        }
        else
        {
            kWebView.Frame = new Rect(50, 150, NGUITools.screenSize.x - 100, NGUITools.screenSize.y - 200);
        }
        */

        kWebView.Frame = rc;

        kWebView.CleanCache();
        kWebView.Load(strURL);
        kWebView.Show();


    }

    public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);
	}

#if UNITY_EDITOR
    private void OnGUI()
    {
        GUIStyle currentStyle = new GUIStyle(GUI.skin.box);
        currentStyle.normal.background = MakeTex(2, 2, new Color(0f, 1f, 0f, 0.5f));
        //GUI.Box(new Rect(50, 150, Screen.width - 100, Screen.height - 200), "WebView", currentStyle);
        GUI.Box(new Rect(50, 150, NGUITools.screenSize.x - 100, NGUITools.screenSize.y - 200), "WebView", currentStyle);
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
#endif

    public void OnClick_CloseBtn()
	{
        OnClickClose();
	}

    public void OnClick_BackBtn()
    {
        if (kWebView == null)
            return;

        if (kWebView.CanGoBack)
            kWebView.GoBack();
        else
            OnClickClose();
    }

    public void OnBtnYouTube()
    {
#if UNITY_EDITOR
        return;
#endif

        SprYouTubeNotice.gameObject.SetActive(false);

        PlayerPrefs.SetString("YOUTUBE_LINK", GameInfo.Instance.YouTubeLink);
        LoadURL(GameInfo.Instance.YouTubeLink, new Rect(50, 100, AppMgr.Instance.DeviceWidth - 100, AppMgr.Instance.DeviceHeight - 150));
    }
       
    void OnPageStarted(UniWebView webView, string url)
    {
        //m_isLoading = true;
    }

    void OnPageFinished(UniWebView webView, int statusCode, string url)
    {
        //m_isLoading = false;
        /*
        if (success)
            webView.Show();
        else
            Debug.Log("Something wrong in webview loading: " + errorMessage);
        */
        //m_backButton.SetActive(webView.CanGoBack());
    }

    bool OnShouldClose(UniWebView webView)
    {
        OnClickClose();

        return true;
    }

    public override bool IsBackButton()
    {
        bool isBack = false;

#if UNITY_EDITOR
        isBack = true;
#endif

        return isBack;
    }
    
    private void CloseWebView()
    {
        if(kWebView != null)
        {
            kWebView.CleanCache();
            UniWebView.ClearCookies();
            DestroyImmediate(kWebView.gameObject);
            kWebView = null;
        }
    }

    public override void OnClickClose()
    {
        CloseWebView();
        StopAllCoroutines();
        base.OnClickClose();

        GameSupport.OpenWebView_ServerRelocate(FLocalizeString.Instance.GetText(6002), FLocalizeString.Instance.GetText(3229));
    }
}
