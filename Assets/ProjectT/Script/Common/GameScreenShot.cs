//#define USE_UNITY_CAPTURE_API
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif

public class GameScreenShot : MonoBehaviour
{
    public delegate void OnScreenShot();
    public OnScreenShot onScreenShot;
    public List<GameObject> disableUIRoot;
    public List<Camera> camList;
    public float delay;

    private bool takingScreenShot;
    private int resWidth;
    private int resHeight;
    string path;
    string folderName;
    // Use this for initialization
    private string _filePath;
#if UNITY_IOS
    [DllImport("__Internal")]
    public static extern void iOSCaptureToCamera(String fileName);
#endif

    public string   SavePath    { get { return path; } }
    public int      Width       { get { return resWidth; } }
    public int      Height      { get { return resHeight; } }


    void Start()
    {
        takingScreenShot = false;

        resWidth = (int)((float)Screen.width * GameInfo.Instance.GameConfig.ScreenShotWidthRatio);
        resHeight = (int)((float)Screen.height * GameInfo.Instance.GameConfig.ScreenShotWidthRatio);
        folderName = "/ScreenShot";

#if UNITY_EDITOR
        path =  Application.dataPath + folderName;
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
            //path = Application.persistentDataPath + folderName;
    //#else
            path = Application.persistentDataPath;
    //#endif

#endif
        Debug.Log(path);
    }

    public void OnClick_ScreenShot()
    {
#if !UNITY_STANDALONE
        Handheld.Vibrate();
#endif
//#if !USE_UNITY_CAPTURE_API
        if (camList == null || camList.Count == 0 || takingScreenShot)
        {
            if (onScreenShot != null)
                onScreenShot();

            return;
        }
//#endif
        //#else
        takingScreenShot = true;
        StartCoroutine(coScreenShot());
//#endif
    }

    public void OnSaveScreenShot()
    {
#if !UNITY_STANDALONE
        Handheld.Vibrate();
#endif
        if (camList == null || camList.Count == 0 || takingScreenShot)
        {
            if (onScreenShot != null)
                onScreenShot();

            return;
        }

        takingScreenShot = true;
        StartCoroutine(SaveLobbyScreenShot());
    }

    private IEnumerator coScreenShot()
    {
        string fileName = this.SetDirAndFileName();
        _filePath = fileName;
        yield return new WaitForEndOfFrame();

        Texture2D screenShot = TakeScreenShot();

        byte[] bytes = screenShot.EncodeToPNG();

        string fullPath = path + "/" + fileName;

        File.WriteAllBytes(fullPath, bytes);
        Debug.LogError("Path : " + path);
        Debug.LogError("FileName : " + fileName);
        RenderTexture.active = null;
        yield return new WaitForEndOfFrame();


        if (onScreenShot != null)
        {
            onScreenShot();
        }

        yield return new WaitForSeconds(delay);
        takingScreenShot = false;
        yield return null;

        ScreenShotPopup.ShowScreenShotPopup(screenShot, fullPath);
    }

    private IEnumerator SaveLobbyScreenShot()
    {
        string fileName = GameInfo.Instance.GameConfig.LobbyBGFileName;

        _filePath = fileName;

        yield return new WaitForEndOfFrame();

        Texture2D screenShot = TakeScreenShot();

        byte[] bytes = screenShot.EncodeToPNG();

        string fullPath = path + "/" + fileName;

        File.WriteAllBytes(fullPath, bytes);
        Debug.LogError("Path : " + path);
        Debug.LogError("FileName : " + fileName);
        RenderTexture.active = null;
        yield return new WaitForEndOfFrame();


        if (onScreenShot != null)
        {
            onScreenShot();
        }

        yield return new WaitForSeconds(delay);
        takingScreenShot = false;
        yield return null;

    }

    public Texture2D TakeScreenShot()
    {
        camList.Sort(CompareCamera);
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        RenderTexture.active = rt;

        for (int i = 0; i < camList.Count; ++i)
        {
            if (camList[i].gameObject.activeSelf)
            {
                camList[i].targetTexture = rt;
                camList[i].Render();
                camList[i].targetTexture = null;
            }
        }

        //Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        //screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0, false);
        //screenShot.Apply();

        Texture2D screenShot = ScreenCapture(resWidth, resHeight);

        Destroy(rt);
        return screenShot;
    }

    public Texture2D ScreenCapture(int width, int height)
    {
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
        screenShot.Apply();

        return screenShot;
    }

    private int CompareCamera(Camera cam1, Camera cam2)
    {
        float depth1 = cam1.depth;
        float depth2 = cam2.depth;

        if (depth1 > depth2)
            return 1;
        else if (depth2 > depth1)
            return -1;
        else
            return 0;

    }

    public void ScreenShotSaveToAlbum(OnScreenShot saveEndedCallback)
    {
        StopAllCoroutines();

#if UNITY_ANDROID && !UNITY_EDITOR
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
#endif

        StartCoroutine(ScreenShotSave());
    }

    IEnumerator ScreenShotSave(OnScreenShot saveEndedCallback = null)
    {
        string fileName = this.SetDirAndFileName();
        _filePath = fileName;
        yield return new WaitForEndOfFrame();

        Texture2D screenShot = ScreenCapture(Screen.width, Screen.height);

        byte[] bytes = screenShot.EncodeToPNG();

        string fullPath = path + "/" + fileName;

        File.WriteAllBytes(fullPath, bytes);
        Debug.LogError("Path : " + path);
        Debug.LogError("FileName : " + fileName);
        RenderTexture.active = null;
        yield return new WaitForEndOfFrame();


#if UNITY_IOS
        iOSCaptureToCamera(string.Format("/{0}", fileName));
#endif

#if UNITY_ANDROID// && !UNITY_EDITOR
        // 안드로이드 갤러리, 사진첩 업데이트 부분
        // 요거 안하면 "내파일" 에서는 보이지만 갤러리 및 사진첩 어플에서는 보이지 않는 문제가 생김!!!  

        //string myFolderLocation = "/storage/emulated/0/DCIM/ProjectT/";
        //string myScreenshotLocation = myFolderLocation + fileName;

        //if (!System.IO.Directory.Exists(myFolderLocation))
        //{
        //    System.IO.Directory.CreateDirectory(myFolderLocation);
        //}

        //string oriPath = path + "/" + fileName;
        //System.IO.File.Move(oriPath, myScreenshotLocation);

        AndroidJavaClass classPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject objActivity = classPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        using (AndroidJavaClass mediaClass = new AndroidJavaClass("android.provider.MediaStore$Images$Media"))
        {
            using (AndroidJavaObject contentResolver = objActivity.Call<AndroidJavaObject>("getContentResolver"))
            {
                AndroidJavaObject image = Texture2DToAndroidBitmap(screenShot);
                mediaClass.CallStatic<string>("insertImage", contentResolver, image, fileName, fileName);
            }
        }
        //android.permission.WRITE_EXTERNAL_STORAGE
        /*AndroidJavaClass classUri = new AndroidJavaClass("android.net.Uri");
        AndroidJavaObject objIntent = new AndroidJavaObject("android.content.Intent", new object[2] { "android.intent.action.MEDIA_SCANNER_SCAN_FILE", classUri.CallStatic<AndroidJavaObject>("parse", "file://" + myScreenshotLocation) });
        objActivity.Call("sendBroadcast", objIntent);

        Application.OpenURL(myScreenshotLocation);*/
#endif

        if (saveEndedCallback != null)
            saveEndedCallback();
    }

#if UNITY_ANDROID
    private AndroidJavaObject Texture2DToAndroidBitmap(Texture2D a_Texture)
    {
        byte[] encodedTexture = a_Texture.EncodeToPNG();
        using (AndroidJavaClass bitmapFactory = new AndroidJavaClass("android.graphics.BitmapFactory"))
        {
            return bitmapFactory.CallStatic<AndroidJavaObject>("decodeByteArray", encodedTexture, 0, encodedTexture.Length);
        }
    }
#endif

    private string SetDirAndFileName()
    {
        string fileName = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
#if USE_UNITY_CAPTURE_API && UNITY_ANDROID

        DirectoryInfo tempDir = new DirectoryInfo(Application.persistentDataPath + folderName);
        if (!tempDir.Exists)
        {
            Directory.CreateDirectory(Application.persistentDataPath + folderName);
        }
#endif
#if UNITY_ANDROID
        DirectoryInfo dir = new DirectoryInfo(path);
        if (!dir.Exists)
        {
            Directory.CreateDirectory(path);
        }
        fileName = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
#elif UNITY_IOS
        fileName = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
#endif
        return fileName;

    }
#if USE_UNITY_CAPTURE_API
    private void MoveFileToCustomPath(string fileName)
    {

#if UNITY_EDITOR
        string Origin_Path = System.IO.Path.Combine(path + "/", fileName);
#else
        string Origin_Path = System.IO.Path.Combine(Application.persistentDataPath + folderName + "/", fileName);
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
        // This is the path of my folder.
        string Path = path + "/" + fileName;
        if (System.IO.File.Exists(Origin_Path))
        {
            System.IO.File.Move(Origin_Path, Path);
        }
#endif
#if UNITY_IOS
        iOSCaptureToCamera(string.Format("/{0}", fileName));
        //iOSCaptureToCamera(string.Format("{0}/{1}", path,fileName));
#endif
    }
#endif
}
