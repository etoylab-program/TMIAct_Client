using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitterKit.Unity;
using System.IO;
#if !DISABLESTEAMWORKS
using Steamworks;
#endif
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

public class ScreenShotPopup
{
    public static UIScreenShotPopup GetScreenShotPopup()
    {
        UIScreenShotPopup mpopup = null;

        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
            mpopup = LobbyUIManager.Instance.GetUI<UIScreenShotPopup>("ScreenShotPopup");
        else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
            mpopup = GameUIManager.Instance.GetUI<UIScreenShotPopup>("ScreenShotPopup");
        else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
            mpopup = TitleUIManager.Instance.GetUI<UIScreenShotPopup>("ScreenShotPopup");

        return mpopup;
    }

    public static void ShowScreenShotPopup(Texture2D tex, string filePath)
    {
        UIScreenShotPopup mpopup = GetScreenShotPopup();

        if (mpopup == null)
            return;

        mpopup.InitScreenShotPopup(tex, filePath);
    }

    public static void ShowScreenShotPopup(UIFigureAlbumPopup figureAlbumPopup, UIFigureAlbumPopup.sImgInfo imgInfo)
    {
        UIScreenShotPopup popup = GetScreenShotPopup();
        if (popup == null)
        {
            return;
        }

        popup.InitScreenShotPopup(figureAlbumPopup, imgInfo);
    }
}

public class UIScreenShotPopup : FComponent
{
    //public UILabel kTitleLabel;
    public UITexture kSrcTexture;
    public GameObject kMainUI;
    public UISprite kFlashSprite;

    public System.Action onPopupClose = null;

    private const string twitterNameParam = "Check this game";
    private string twitterDescriptionParam = "";
    private const string TWITTER_ADDRESS = "http://twitter.com/intent/tweet";
    private string TWITTER_LANGUAGE = "en";
    //private bool isFlashSpriteOn = true;
    //public bool IsFlashSpriteOn { set { this.isFlashSpriteOn = value; } }
    private string _filePath;

    private UIFigureAlbumPopup          mFigureAlbumPopup   = null;
    private UIFigureAlbumPopup.sImgInfo mImgInfo            = null;


    public override void Awake()
    {
#if !UNITY_STANDALONE
        Twitter.AwakeInit();
        Twitter.Init();
#endif
        base.Awake();
    }

    public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		base.OnDisable();

        Object.DestroyImmediate(kSrcTexture.mainTexture);
        kSrcTexture.mainTexture = null;

        if (mFigureAlbumPopup)
        {
            mFigureAlbumPopup.ReleaseGIF();
            mFigureAlbumPopup = null;
        }
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);
    }

    public override void OnClickClose()
    {
        if (onPopupClose != null)
        {
            onPopupClose.Invoke();
        }
        base.OnClickClose();
    }

    public void SetSavePathInfoLabel(string text)
    {
        //kTitleLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1204), text);
    }

    public void InitScreenShotPopup(Texture2D tex, string filePath)
    {
        kSrcTexture.mainTexture = tex;
        _filePath = filePath;
        mImgInfo = null;

        SetUIActive(true);
    }

    public void InitScreenShotPopup(UIFigureAlbumPopup figureAlbumPopup, UIFigureAlbumPopup.sImgInfo imgInfo)
    {
        mFigureAlbumPopup = figureAlbumPopup;
        mImgInfo = imgInfo;

        mImgInfo.LoadImage(figureAlbumPopup.GIFDecoder);

        kSrcTexture.mainTexture = mImgInfo.MainTex;
        _filePath = mImgInfo.Path;

        SetUIActive(true);
    }

    public void OnClick_Twitter()
    {
        //Twitter.Init();
        string imageUri = "file://" + _filePath;
#if UNITY_ANDROID
        //ShareAndroid("アクション対魔忍", "アクション対魔忍", "", new string[] { _filePath }, "image/gif", true, "Select Sharing App");
        new NativeShare().AddFile(_filePath).SetSubject("アクション対魔忍").SetText("#アクション対魔忍").Share();
#elif UNITY_IOS
        new NativeShare().AddFile(_filePath).SetSubject("アクション対魔忍").SetText("#アクション対魔忍").Share();
#elif !DISABLESTEAMWORKS
        int resWidth = (int)((float)Screen.width * GameInfo.Instance.GameConfig.ScreenShotWidthRatio);
        int resHeight = (int)((float)Screen.height * GameInfo.Instance.GameConfig.ScreenShotWidthRatio);

        string extName = Path.GetExtension(_filePath).ToLower();
        if (extName == ".gif")
        {
#if UNITY_STANDALONE_WIN
            _filePath = _filePath.Replace("/", "\\");
            string arg = string.Format("/select,\"{0}\"", _filePath);
            System.Diagnostics.Process.Start("Explorer.exe", arg);
#elif UNITY_STANDALONE_OSX
            System.Diagnostics.Process.Start(Path.GetDirectoryName(_filePath));
#endif            
        }
        else
        {
            SteamScreenshots.AddScreenshotToLibrary(_filePath, null, resWidth, resHeight);
        }
#endif
    }

        IEnumerator coFlashLight()
    {
        kMainUI.SetActive(false);
        //if(isFlashSpriteOn)
        kFlashSprite.alpha = 1f;
        yield return new WaitForEndOfFrame();
        //if (isFlashSpriteOn)
        //{
            TweenAlpha tween = TweenAlpha.Begin(kFlashSprite.gameObject, 0.5f, 0f);
            yield return new WaitForSeconds(0.8f);
            kFlashSprite.alpha = 0f;
        //}
        //else
        //    yield return new WaitForSeconds(0.3f);

        kMainUI.SetActive(true);

        yield return null;
    }


    private string GetTwittLanguage()
    {
        string ret = "en";
        switch (FLocalizeString.Language)
        {
            case eLANGUAGE.JPN:
                ret = "jp";
                break;
            case eLANGUAGE.KOR:
                ret = "ko";
                break;

            default:
                ret = "jp";
                break;
        }
        return ret;
    }

    private void FixedUpdate()
    {
        if(mFigureAlbumPopup == null || mImgInfo == null)
        {
            return;
        }

        if(!mImgInfo.IsGIF)
        {
            return;
        }

        mFigureAlbumPopup.UpdateGIFFrame(kSrcTexture);
    }

//#if UNITY_ANDROID
//	public static void ShareAndroid(string body, string subject, string url, string[] filePaths, string mimeType, bool chooser, string chooserText)
//	{
//		using (AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent"))
//		using (AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent"))
//		{
//			using (intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND")))
//			{ }
//			using (intentObject.Call<AndroidJavaObject>("setType", mimeType))
//			{ }
//			using (intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), subject))
//			{ }
//			using (intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), body))
//			{ }

//			if (!string.IsNullOrEmpty(url))
//			{
//				// attach url
//				using (AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri"))
//				using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", url))
//				using (intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject))
//				{ }
//			}
//			else if (filePaths != null)
//			{
//				// attach extra files (pictures, pdf, etc.)
//				using (AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri"))
//				using (AndroidJavaObject uris = new AndroidJavaObject("java.util.ArrayList"))
//				{
//					for (int i = 0; i < filePaths.Length; i++)
//					{
//						//instantiate the object Uri with the parse of the url's file
//						using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", filePaths[i]))
//						{
//							uris.Call<bool>("add", uriObject);
//						}
//					}

//					using (intentObject.Call<AndroidJavaObject>("putParcelableArrayListExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uris))
//					{ }
//				}
//			}

//			// finally start application
//			using (AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
//			using (AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity"))
//			{
//				if (chooser)
//                {
//                    AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, chooserText);
//                    currentActivity.Call("startActivity", jChooser);
//                }
//                else
//                {
//                    currentActivity.Call("startActivity", intentObject);
//                }
//			}
//		}
//	}
//#endif
}
