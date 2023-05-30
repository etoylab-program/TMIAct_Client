using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIServerAgreePopup : FComponent
{
    public GameObject kWebViewGO;
    public Vector4 WebviewPos = Vector4.zero;

    public UILabel LbCount;

    private UniWebView kWebView;


    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
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
        }

        StartCoroutine(LoadURLWaitForSeconds(FLocalizeString.Instance.GetText(3230), 0.5f));
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        string c = "[FFFFFFFF]";
        if (GameInfo.Instance.ServerMigrationCountOnWeek >= GameInfo.Instance.GameTable.ServerMergeSets[0].MaxCount)
        {
            c = "[FF0000FF]";
        }

        LbCount.textlocalize = string.Format(FLocalizeString.Instance.GetText(6021),
                                             c, GameInfo.Instance.ServerMigrationCountOnWeek, GameInfo.Instance.GameTable.ServerMergeSets[0].MaxCount);
    }

    public override void OnClickClose()
    {
        CloseWebView();
        StopAllCoroutines();
        base.OnClickClose();
    }

    public void OnClick_Agree()
    {
        GameInfo.Instance.Send_ReqGetTotalRelocateCntToNotComplete(ShowServerIdPopup);
    }

    private void ShowServerIdPopup(int result, PktMsgType pktMsg)
    {
        if(result != 0)
        {
            return;
        }

        OnClickClose();
        LobbyUIManager.Instance.ShowUI("ServerIDPopup", true);
    }

    private IEnumerator LoadURLWaitForSeconds(string strURL, float waitTime = 0.5f)
    {
        yield return new WaitForSeconds(waitTime);
        LoadURL(strURL);
    }

    private void LoadURL(string strURL)
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
       
        kWebView.Frame = new Rect(WebviewPos.x, WebviewPos.y, AppMgr.Instance.DeviceWidth - WebviewPos.w, AppMgr.Instance.DeviceHeight - WebviewPos.z);

        kWebView.CleanCache();
        kWebView.Load(strURL);
        kWebView.Show();
    }

    private void CloseWebView()
    {
        if (kWebView != null)
        {
            kWebView.CleanCache();
            UniWebView.ClearCookies();
            DestroyImmediate(kWebView.gameObject);
            kWebView = null;
        }
    }

#if UNITY_EDITOR
    
    private void OnGUI()
    {
        GUIStyle currentStyle = new GUIStyle(GUI.skin.box);
        currentStyle.normal.background = MakeTex(2, 2, new Color(0f, 1f, 0f, 0.5f));
        //GUI.Box(new Rect(50, 150, Screen.width - 100, Screen.height - 200), "WebView", currentStyle);        
        //GUI.Box(new Rect(50, 150, NGUITools.screenSize.x - 100, NGUITools.screenSize.y - 200), "WebView", currentStyle);
        GUI.Box(new Rect(WebviewPos.x, WebviewPos.y, NGUITools.screenSize.x - WebviewPos.w, NGUITools.screenSize.y - WebviewPos.z), "WebView", currentStyle);
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
}
