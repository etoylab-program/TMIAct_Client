using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

/*
public enum eBannerType : System.Byte
{
    ROLLING              = 0,	        // MainPanel 의 롤링배너
    PACKAGE_BANNER       = 1,             // 패키지팝업의 패키지 상품 배너(기존에는 아틀라스와 텍스쳐 같은이름사용)        
    PACKAGE_BG           = 2,                 // 패키지팝업의 패키지 상품 BG                                        -aos구분
    EVENT_MAINBG         = 3,               // 이벤트 MainPanel BG                                                -aos구분
    EVENT_BANNER_NOW     = 4,           // 이벤트 EventModePopup 활성 배너
    EVENT_BANNER_PAST    = 5,          // 이벤트 EventModePopup 비활성 배너
    EVENT_STAGEBG        = 6,              // 이벤트 StagePanel BG
    EVENT_RULEBG         = 7,               // 이벤트 EventRulePopup BG                                            -aos구분
    GLLA_MISSION_LOGIN   = 8,         // 게릴라 미션 로그인 보너스용                                                -aos구분
    EVENT_LOGO         = 9            //이벤트 로고
}//
*/

public class BannerManager : MonoSingleton<BannerManager>
{
    private const string                m_imgExtension  = ".png";
    private const string                m_aosFlsg       = "_aos";
    private bool                        m_bIsInit       = false;
    private string                      m_BannerPath    = "banner";
    private string                      m_SavePath;
    private string                      m_FilePath;
    private string                      m_testURL       = "https://testcdn0000.blob.core.windows.net/patch/develop_outer/gm/banner_images/tk%20%283%29.jpg";
    private Dictionary<string, string>  m_files         = new Dictionary<string, string>(); //FileName, FilePath
    private Dictionary<string, string>  m_LoadFiles     = new Dictionary<string, string>(); //FileName, URL
    private List<Texture>               mDestroyTexList = new List<Texture>();


    private void Start() { 
        Init(); 
    }

    void Init()
    {
        if(Instance != this)
        {
            DestroyImmediate(this.gameObject);
            return;
        }

        if (m_bIsInit)
            return;
        DontDestroyOnLoad(gameObject);
        m_bIsInit = true;
        CheckDirectory();
    }

    //디렉토리 유무 확인, 존재한다면 디렉토리 안에 있는 파일리스트 확인 및 저장
    void CheckDirectory()
    {
        m_SavePath = Path.Combine(Application.persistentDataPath, m_BannerPath);
        m_FilePath = Path.Combine(m_SavePath, GetFileNameWithExtension(m_testURL));

        Debug.Log(m_SavePath);

        if (!Directory.Exists(m_SavePath))
        {
            Debug.Log(m_SavePath + " - 디렉토리 생성");
            Directory.CreateDirectory(m_SavePath);
            return;
        }

        m_files.Clear();

        string[] files = Directory.GetFiles(m_SavePath);
        for (int i = 0; i < files.Length; i++)
            m_files.Add(GetFileName(files[i]), files[i]);
    }

    public void AddServerData(ServerData inServerData)
    {
        StopAllCoroutines();
        CheckDirectory();
        m_LoadFiles.Clear();
        if (null != inServerData.GuerrillaMissionList)
        {
            List<GuerrillaMissionData> loginMission = new List<GuerrillaMissionData>();
            loginMission.Clear();
            GameSupport.GetGuerrillaMissionListWithTimeByType(ref loginMission, "GM_LoginBonus");
            for (int i = 0; i < loginMission.Count; i++)
            {
                GuerrillaMissionData guerrillaMissionData = loginMission[i];
                if (null == guerrillaMissionData)
                {
                    Debug.LogError("GuerrillaMissionData 내용이 없습니다.");
                    continue;
                }
                //게릴라 로그인 미션 이미지 다운로드 방식? 변경예정
                //if (!string.IsNullOrEmpty(guerrillaMissionData.Desc))
                //    AddFileDownloadURL(guerrillaMissionData.Desc);
            }
        }

        if (null != inServerData.BannerList)
        {
            //System.DateTime curTime = GameInfo.Instance.GetNetworkTime();

            for (int i = 0; i < inServerData.BannerList.Count; i++)
            {
                BannerData bannerData = inServerData.BannerList[i];
                if (null == bannerData)
                {
                    Debug.LogError("BannerData 내용이 없습니다.");
                    continue;
                }

                /*
                if( curTime >= bannerData.EndDate ) {
                    continue;
				}
                */

                //PACKAGE_BG, EVENT_MAINBG, EVENT_RULEBG, GLLA_MISSION_LOGIN
                if (!string.IsNullOrEmpty(bannerData.UrlImage))
                {
                    bool bLocalize = bannerData.Localizes[(int)eBannerLocalizeType.Url];
                    if (bannerData.BannerType == (int)eBannerType.PACKAGE_BG ||
                        bannerData.BannerType == (int)eBannerType.EVENT_MAINBG ||
                        bannerData.BannerType == (int)eBannerType.EVENT_STAGEBG ||
                        bannerData.BannerType == (int)eBannerType.EVENT_RULEBG ||
                        bannerData.BannerType == (int)eBannerType.GLLA_MISSION_LOGIN ||
                        bannerData.BannerType == (int)eBannerType.LOGIN_PACKAGE_BG ||
                        bannerData.BannerType == (int)eBannerType.SPECIAL_BUY ||
                        bannerData.BannerType == (int)eBannerType.UNEXPECTED_PACKAGE ||
                        bannerData.BannerType == (int)eBannerType.LOGIN_EVENT_BG)
                    {
                        AddFileDownloadURL(bannerData.UrlImage, true, bLocalize);
                    }
                    else if (bannerData.BannerType == (int)eBannerType.SOCIAL && bannerData.BannerTypeValue == 1) // 유튜브 
                    {
                        GameInfo.Instance.YouTubeLink = bannerData.UrlImage;
                    }
                    else
                    {
                        AddFileDownloadURL(bannerData.UrlImage, false, bLocalize);
                    }
                }

                if (!string.IsNullOrEmpty(bannerData.UrlAddImage1))
                {
                    if (bannerData.FunctionValue2.ToLower().Equals("char"))
                    {
                        AddFileDownloadURL(bannerData.UrlAddImage1, true, bannerData.Localizes[(int)eBannerLocalizeType.AddUrl1]);
                    }
                    else
                    {
                        AddFileDownloadURL(bannerData.UrlAddImage1, false, bannerData.Localizes[(int)eBannerLocalizeType.AddUrl1]);
                    }
                }
                
                if (!string.IsNullOrEmpty(bannerData.UrlAddImage2))
                {
                    AddFileDownloadURL(bannerData.UrlAddImage2, false, bannerData.Localizes[(int)eBannerLocalizeType.AddUrl2]);
                }
            }
        }

        if (null != inServerData.GachaCategoryList)
        {
            for (int i = 0; i < inServerData.GachaCategoryList.Count; i++)
            {
                GachaCategoryData gachaCategoryData = inServerData.GachaCategoryList[i];
                if (null == gachaCategoryData)
                {
                    Debug.LogError("GachaCategoryData 내용이 없습니다.");
                    continue;
                }

                if (!string.IsNullOrEmpty(gachaCategoryData.UrlBtnImage))
                    AddFileDownloadURL(gachaCategoryData.UrlBtnImage, false, gachaCategoryData.Localize[(int)eGachaLocalizeType.Banner]);
                if (!string.IsNullOrEmpty(gachaCategoryData.UrlBGImage))
                    AddFileDownloadURL(gachaCategoryData.UrlBGImage, true, gachaCategoryData.Localize[(int)eGachaLocalizeType.Background]);
                if (!string.IsNullOrEmpty(gachaCategoryData.UrlAddImage))
                    AddFileDownloadURL(gachaCategoryData.UrlAddImage, false, gachaCategoryData.Localize[(int)eGachaLocalizeType.AddImage]);
            }
        }

        DownLoadImages();
    }

    public void RefreshBannerImages()
    {
        WaitPopup.Show();
        m_LoadFiles.Clear();
        AddServerData(GameInfo.Instance.ServerData);
    }

    void DownLoadImages()
    {
        foreach(KeyValuePair<string, string> pair in m_files)
        {
            if (!m_LoadFiles.ContainsKey(pair.Key))
                RemoveTexture(pair.Value);
        }

        StartCoroutine(StartDownLoads());
    }

	private IEnumerator StartDownLoads() {
		mDestroyTexList.Clear();

		foreach( KeyValuePair<string, string> pair in m_LoadFiles ) {
			yield return StartCoroutine( GetTexture( pair.Value ) );
		}

        for( int i = 0; i < mDestroyTexList.Count; i++ ) {
            DestroyImmediate( mDestroyTexList[i], true );
            mDestroyTexList[i] = null;
        }

        Debug.Log( "배너 이미지 " + mDestroyTexList.Count + "개 " + "다운로드 완료" );

        System.GC.Collect();
		WaitPopup.Hide();
	}

	public void AddFileDownloadURL(string url, bool platformFlag = false, bool localizeFlag = true)
    {
        Log.Show("AddFileDownloadURL : " + url, Log.ColorType.Red);

        url = Path.Combine(AppMgr.Instance.configData.GetPatchServerAddr(), url);
        url = GetLocalizeURL(url, platformFlag, localizeFlag);

        string fileName = GetFileName(url);
        if (m_LoadFiles.ContainsKey(fileName))
        {
            return;
        }

        if (AppMgr.Instance.ServerType != AppMgr.eServerType.LIVE)
        {
            if (AppMgr.Instance.ServerType == AppMgr.eServerType.INTERNAL || AppMgr.Instance.ServerType == AppMgr.eServerType.QA)
            {
                url += AppMgr.Instance.CurrentRestriction;
            }
            else if (AppMgr.Instance.ServerType == AppMgr.eServerType.REVIEW)
            {
                url += AppMgr.Instance.CurrentRestriction;
            }
        }

        m_LoadFiles.Add(fileName, url);
        Log.Show("LocalizeURL AddFileDownloadURL : " + url  + " / " + fileName);
    }

    //파일 이름으로 파일 유무 체크
    public bool FileExistsCheckWithFileName(string fileName)
    {
        return m_files.ContainsKey(fileName);
    }

    //파일 경로로 파일 유무 체크
    public bool FileExistsCheckWithFilePath(string filePath)
    {
        return m_files.ContainsValue(filePath);
    }

    //해당 경로에 파일이 존재하는지 체크 후 없다면 다운로드 실행
    public void DownloadTexture(string url)
    {
        //Debug.Log("파일명 : " + GetFileName(url));
        //Debug.Log("파일 확장자 : " + GetExtension(url));

        string checkTexPath = Path.Combine(m_SavePath, GetFileNameWithExtension(url));

        if (!File.Exists(checkTexPath))
        {
            StartCoroutine(GetTexture(url));
        }
    }

    /// <summary>
    /// 실제 다운로드 하는 함수
    /// </summary>
    IEnumerator GetTexture( string url ) {
		string path2 = url;
        
        if( !string.IsNullOrEmpty( AppMgr.Instance.CurrentRestriction ) ) {
            path2 = path2.Replace( AppMgr.Instance.CurrentRestriction, "" );
        }

        path2 = path2.Replace( "?cache=no", "" );
        path2 = GetFileNameWithExtension( path2 );
        
		string checkTexPath = Path.Combine(m_SavePath, path2);
		if( File.Exists( checkTexPath ) ) {
			Log.Show( path2 + " 파일이 이미 존재합니다." );
			yield break;
		}

		UnityWebRequest www = UnityWebRequestTexture.GetTexture(url, true);
		yield return www.SendWebRequest();

		if( www.result != UnityWebRequest.Result.Success ) {
		}
		else {
			string filePath = "";

			Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
			byte[] bytes = ((DownloadHandlerTexture)www.downloadHandler).data;

			url = url.Replace( "?cache=no", "" );

			if( !string.IsNullOrEmpty( AppMgr.Instance.CurrentRestriction ) ) {
				url = url.Replace( AppMgr.Instance.CurrentRestriction, "" );
			}

			string extension = GetExtension(url);

			if( extension.ToLower().Equals( "jpg" ) || extension.ToLower().Equals( "jpeg" ) || extension.ToLower().Equals( "png" ) ) {
				filePath = Path.Combine( m_SavePath, GetFileNameWithExtension( url ) );
				File.WriteAllBytes( filePath, bytes );
			}

            //DestroyImmediate( tex, false );
            mDestroyTexList.Add( tex );
            Debug.Log( filePath + " 파일 저장" );
		}

        www.Dispose();
    }

	//텍스쳐 불러오기
	public Texture2D LoadTexture(string texPath)
    {
        if (!File.Exists(texPath))
        {
            //Debug.LogError(texPath + "경로에 " + GetFileName(texPath) + "파일이 존재하지 않습니다.");
            return null;
        }
        
        byte[] bytes = File.ReadAllBytes(texPath);
        Texture2D tex = new Texture2D(128, 128, TextureFormat.RGBA32, false);
        tex.LoadImage(bytes);
        
        return tex;
    }
    
    public Texture2D LoadTextureWithFileURL(string url, bool platformFlag = false, bool localizeFlag = true)
    {
        if (string.IsNullOrEmpty(url))
        {
            return null;
        }
        
        url = Path.Combine(AppMgr.Instance.configData.GetPatchServerAddr(), url);
        url = GetLocalizeURL(url, platformFlag, localizeFlag);

        string fileName = GetFileNameWithExtension(url);
        string texPath = Path.Combine(m_SavePath, fileName);

        return LoadTexture(texPath);
    }
    
    //파일 경로로 유무 확인 후 파일이 존재한다면 지우기
    public void RemoveTexture(string texPath)
    {
        if (File.Exists(texPath))
        {
            File.Delete(texPath);
            Debug.Log("파일 제거!!!");
        }
        else
        {
            Debug.Log(texPath + "경로에 " + GetFileName(texPath) + "파일이 존재하지 않습니다.");
        }
    }

    //파일이름 + 확장자
    string GetFileNameWithExtension(string url)
    {
        return Path.GetFileName(url);
    }

    //파일이름
    string GetFileName(string url, bool platform = false)
    {
        string fileName = Path.GetFileName(url);
		string[] fileNames = Utility.Split(fileName, '.'); //fileName.Split('.');

        if (platform)
        {
            if (AppMgr.Instance.configData.ResLoadPlatformType == AppMgr.eResPlatform.aos)
                return fileNames[0] + m_aosFlsg;
            else if (AppMgr.Instance.configData.ResLoadPlatformType == AppMgr.eResPlatform.ios)
                return fileNames[0];

        }
        return fileNames[0];
    }

    //파일 확장자
    string GetExtension(string url)
    {
        string extension = Path.GetExtension(url);
        extension = extension.Replace(".", "");
        return extension;
    }
    
    string GetLocalizeURL(string url, bool platformFlag = false, bool localizeFlag = true)
    {
        if(AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Japan)
        {
            if(platformFlag)
            {
                if (AppMgr.Instance.configData.ResLoadPlatformType == AppMgr.eResPlatform.aos)
                {
                    return url + m_aosFlsg + m_imgExtension;
                }
                else if (AppMgr.Instance.configData.ResLoadPlatformType == AppMgr.eResPlatform.ios)
                {
                    return url + m_imgExtension;
                }
            }
            else
            {
                return url + m_imgExtension;
            }
        }
        else if(AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Global || AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Steam)
        {
            string addStr = "_";
            if (localizeFlag)
            {
                addStr += FLocalizeString.Language;
            }
            else
            {
                addStr += eLANGUAGE.KOR;
            }
            
            if (platformFlag)
            {
                if (AppMgr.Instance.configData.ResLoadPlatformType == AppMgr.eResPlatform.aos)
                {
                    addStr += m_aosFlsg;
                }
            }
            
            return url + addStr + m_imgExtension;
        }
        
        return url + "_" + FLocalizeString.Language.ToString() + m_imgExtension;
    }
}
