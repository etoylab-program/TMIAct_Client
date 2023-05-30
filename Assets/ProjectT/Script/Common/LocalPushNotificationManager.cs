using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#elif UNITY_IOS
using Unity.Notifications.iOS;
#endif

using System.Threading;
using System.Threading.Tasks;
using System;

public class LocalPushNotificationManager : MonoSingleton<LocalPushNotificationManager>
{
    public enum eLocalPushMessageType
    {
        NONE = 0,

        AP,
        BP,
        CHAR_MSG,

        FACILITY = 100, // 100부터 Facility 테이블 데이터 개수만큼 사용
    }

    private bool m_bIsInit = false;
    private string m_androidDefaultChannelName;

    private string m_fcmToken;
    public string FcmToken { get { return m_fcmToken; } }
    //private const string m_getTokenURL = "http://iid.googleapis.com/iid/info/";
    //private const string m_setTopicURL = "https://iid.googleapis.com/iid/v1/";

    //private void Awake() { Init(); }
    //private void Start() { Init(); }

    public void Init()
    {
        if (Instance != this)
        {
            DestroyImmediate(this.gameObject);
            return;
        }

        if (m_bIsInit)
            return;

        m_bIsInit = true;

        DontDestroyOnLoad();
#if UNITY_EDITOR
        return;
#endif
        //시작시 모든 알람 해제
        RemoveAllNotification();
        //SetNightPush();
#if UNITY_ANDROID
        m_androidDefaultChannelName = "Taimanin";

        AddNotificationChannel(m_androidDefaultChannelName, "Taimanin_Local_Notification", Importance.High, "Defailt Taimanin Local Notification Channel");
#elif UNITY_IOS
        StartCoroutine(RequestAuthorization());
#endif

        InitNotificationCallBack();

#if UNITY_EDITOR
        SetFCMSubscribe();
#endif

#if UNITY_ANDROID
        Firebase.FirebaseApp.LogLevel = Firebase.LogLevel.Error;
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                //   app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                Firebase.FirebaseApp.Create();
                Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
                Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;

                Firebase.Messaging.FirebaseMessaging.TokenRegistrationOnInitEnabled = true;

                Firebase.Analytics.FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
#elif UNITY_IOS
        Firebase.Messaging.FirebaseMessaging.TokenRegistrationOnInitEnabled = true;

        Firebase.FirebaseApp.Create();
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;

        

        Firebase.Analytics.FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
#endif

        //RemoveAllNotification();
    }


    //2020.02.06
    //Android에서 OnApplicationQuit 콜백이 호출이 안되기 때문에 Pause로 변경
    public void OnApplicationPause(bool pause)
    {
        //앱 처음 열었을때는 무시
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title || AppMgr.Instance.SceneType == AppMgr.eSceneType.None)
        {
            return;
        }

        if (pause)
        {
            if (!GameInfo.Instance.IsConnect)
            {
                return;
            }

            RemoveAllNotification();

            if (!FSaveData.Instance.bPushAll)
            {
                UnSubscribeLanguage();
                UnSubscribeNightPush();
                UnSubscribeGlobalPush();

                return;
            }

            //위에서 해지할거 해지하고 다시 구독
            SetFCMSubscribe();

            /*/ 로컬 푸쉬는 심야 푸쉬 옵션에 적용 안받음
            if (!FSaveData.Instance.bPushNigth && GameSupport.IsNightTime(0, 6))
            {
                RemoveNotification((int)eLocalPushMessageType.CHAR_MSG);
                RemoveNotification((int)eLocalPushMessageType.AP);

                return;
            }*/

            // 일정 시간동안 접속 안할시 로컬 메시지 등록
            if (GameInfo.Instance.GameConfig.CharLoginMsgStringIdx.Count >= 0)
            {
                int rand = UnityEngine.Random.Range(0, GameInfo.Instance.GameConfig.CharLoginMsgStringIdx.Count);
                int index = GameInfo.Instance.GameConfig.CharLoginMsgStringIdx[rand];

                AddScheduleNotification((int)eLocalPushMessageType.CHAR_MSG, FLocalizeString.Instance.GetText(index - 2000), 
                                        FLocalizeString.Instance.GetText(index), GameInfo.Instance.GameConfig.CharLoginMsgAddHour, 0, 0);
            }

            // 시설 완료 로컬 푸쉬 등록
            List<FacilityData> listFind = GameInfo.Instance.FacilityList.FindAll(x => x.Stats == 1);
            for (int i = 0; i < listFind.Count; i++)
            {
                FacilityData data = listFind[i];
                int notificationId = (int)eLocalPushMessageType.FACILITY + data.TableID;

                if (data.Stats == 1) // 진행중
                {
                    TimeSpan tsRemain = listFind[i].RemainTime - GameSupport.GetCurrentServerTime();
                    if (0 < tsRemain.Ticks)
                    {
                        int txtIdx = 25006 + data.TableData.Type;
                        AddScheduleNotification(notificationId, FLocalizeString.Instance.GetText(txtIdx - 2000), FLocalizeString.Instance.GetText(txtIdx),
                            tsRemain.Hours, tsRemain.Minutes, tsRemain.Seconds);
                    }
                }
            }

            if (!FSaveData.Instance.bPushAP)
            {
                return;
            }

            AddLocalNotificationGoods((int)eLocalPushMessageType.AP, FLocalizeString.Instance.GetText(23101), 25012, 
                                      (int)GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.AP],
                                      GameSupport.GetMaxAP(), GameInfo.Instance.GameConfig.APUpdateTimeSec);

            AddLocalNotificationGoods((int)eLocalPushMessageType.BP, FLocalizeString.Instance.GetText(23102), 25013, 
                                      (int)GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.BP],
                                      GameInfo.Instance.GameConfig.BPMaxCount, GameInfo.Instance.GameConfig.BPUpdateTimeSec);
        }
        else
        {
            RemoveAllNotification();
        }
    }

    public void OnNet_RefrashUserInfo(int result, PktMsgType _pktmsg)
    {
        if (result != 0)
            return;

        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
        {
            LobbyUIManager.Instance.Renewal("TopPanel");
            LobbyUIManager.Instance.Renewal("GoodsPopup");
        }
    }

    void InitNotificationCallBack()
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.NotificationReceivedCallback receivedCallback = delegate (AndroidNotificationIntentData data)
        {
            string msg = "Notification received : " + data.Id + "\n";
            msg += "\n Notification received: ";
            msg += "\n .Title: " + data.Notification.Title;
            msg += "\n .Body: " + data.Notification.Text;
            msg += "\n .Channel: " + data.Channel;
            Debug.Log(msg);
        };

        AndroidNotificationCenter.OnNotificationReceived += receivedCallback;
#endif
    }

#if UNITY_ANDROID
    //안드로이드 알람 채널 등록(최소 1개는 등록이 되어야함)
    public void AddNotificationChannel(string channelId, string channelName, Unity.Notifications.Android.Importance channelImportance, string channelDesc)
    {
        AndroidNotificationChannel channel = new AndroidNotificationChannel()
        {
            Id = channelId,
            Name = channelName,
            Importance = channelImportance,
            Description = channelDesc,
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }
#endif

#if UNITY_IOS
    IEnumerator RequestAuthorization()
    {
        using (AuthorizationRequest req = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true))
        {
            while (!req.IsFinished)
            {
                yield return null;
            }
            
        string res = "\n RequestAuthorization: \n";
        res += "\n finished: " + req.IsFinished;
        res += "\n granted :  " + req.Granted;
        res += "\n error:  " + req.Error;
        res += "\n deviceToken:  " + req.DeviceToken;
        Debug.Log(res);
        };
    }
#endif

    //알람 추가 - 해당 알람의 ID(취소할때 사용),  제목, 설명, 시, 분, 초
    public void AddScheduleNotification(int notificationId, string title, string desc, int addHour, int addMin, int addSec)
    {
#if UNITY_ANDROID
        AndroidNotification notification = new AndroidNotification();
        notification.Title = title;
        notification.Text = desc;
        notification.FireTime = System.DateTime.Now.Add(new System.TimeSpan(addHour, addMin, addSec));

        AndroidNotificationCenter.SendNotification(notification, m_androidDefaultChannelName);

#elif UNITY_IOS
        iOSNotificationTimeIntervalTrigger timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = new System.TimeSpan(addHour, addMin, addSec),
            Repeats = false
        };

        iOSNotification notification = new iOSNotification()
        {
            Identifier = notificationId.ToString(),
            Title = title,
            Body = desc,
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound | PresentationOption.Badge),
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger,
            Badge = 1,
        };
        iOSNotificationCenter.ScheduleNotification(notification);
#endif
    }

    public void GetLastNotification()
    {
#if UNITY_ANDROID
        AndroidNotificationIntentData notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent();

        if (notificationIntentData != null)
        {
            int id = notificationIntentData.Id;
            string channel = notificationIntentData.Channel;
            AndroidNotification notification = notificationIntentData.Notification;
            Debug.Log("id : " + id + " / Channel : " + channel + " / Notification : " + notification);
        }
#elif UNITY_IOS
        iOSNotification iosNotification = iOSNotificationCenter.GetLastRespondedNotification();
        if(iosNotification != null)
        {
            string msg = "Last Received Notification : " + iosNotification.Identifier + "\n";
            msg += "\n - Notification received: ";
            msg += "\n - .Title: " + iosNotification.Title;
            msg += "\n - .Badge: " + iosNotification.Badge;
            msg += "\n - .Body: " + iosNotification.Body;
            msg += "\n - .CategoryIdentifier: " + iosNotification.CategoryIdentifier;
            msg += "\n - .Subtitle: " + iosNotification.Subtitle;
            msg += "\n - .Data: " + iosNotification.Data;
            Debug.Log(msg);
        }
        else
        {
            Debug.Log("No Notifications received.");
        }
#endif
    }

    //모든 알람 해제
    public void RemoveAllNotification()
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllNotifications();
#elif UNITY_IOS
        iOSNotificationCenter.RemoveAllScheduledNotifications();
        iOSNotificationCenter.ApplicationBadge = 0;
#endif
    }

    //해당 ID의 알람 해제
    public void RemoveNotification(int notificationID)
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelNotification(notificationID);
#elif UNITY_IOS
        iOSNotificationCenter.RemoveScheduledNotification(notificationID.ToString());
#endif
    }

    public void RemoveFacilityNotification(int facilityTableId)
    {
        int notificationId = (int)eLocalPushMessageType.FACILITY + facilityTableId;
        RemoveNotification(notificationId);
    }

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        Log.Show("OnTokenReceived : " + token.Token);
        if (token == null)
            return;
        if (token.Token == string.Empty)
            return;

        string strtoken = token.Token;

#if UNITY_ANDROID
        RemoveAllNotification();
        SetFCMSubscribe();
#endif

        PlayerPrefs.SetString("FCM_PUSH_TOKEN_ORIGIN", strtoken);
        m_fcmToken = token.Token;
    }

    public void FirebaseSetFCMSubscribe()
    {
        SubscribePlatform();
        //if (FSaveData.Instance.bPushAll)
        SetFCMSubscribe();
    }

    public void SubscribePlatform()
    {
        string topicName = "ios";
#if UNITY_ANDROID
        topicName = "aos";
        Firebase.Messaging.FirebaseMessaging.Subscribe("/topics/" + topicName);
        Firebase.Messaging.FirebaseMessaging.Subscribe(topicName);
#endif

#if UNITY_IOS
        List<string> fcmTopics = new List<string>();

        fcmTopics.Add(topicName);

        OnSetSubscribe(fcmTopics);
#endif
    }

    //191004
    //주제 구독 일일히 등록하거나 서버에서 리스트를 받아서 등록하기.
    //FCM주제 구독
    public void SetFCMSubscribe()
    {
        if (!FSaveData.Instance.bPushAll)
            return;

        Debug.Log("SetFCMSubscribe");

        List<string> fcmTopics = new List<string>();

        switch (FLocalizeString.Language)
        {
            case eLANGUAGE.CHT:
                SetFCMSubscribeGlobal(ref fcmTopics, "zht");           //중국어 - 번체
                break;
            case eLANGUAGE.CHS:
                SetFCMSubscribeGlobal(ref fcmTopics, "zhs");           //중국어 - 간체
                break;
            case eLANGUAGE.ENG:
                SetFCMSubscribeGlobal(ref fcmTopics, "en");            //영어
                break;
            case eLANGUAGE.JPN:
                SetFCMSubscribeGlobal(ref fcmTopics, "ja");            //일본어
                break;
            case eLANGUAGE.KOR:
                SetFCMSubscribeGlobal(ref fcmTopics, "ko");            //한국어
                break;
            case eLANGUAGE.ESP:
                SetFCMSubscribeGlobal(ref fcmTopics, "es");            //스페인어
                break;
        }

        if (FSaveData.Instance.bPushNigth)
        {
            UnSubscribeNightPush();
            //야간만 안받는 토픽 구독
            Log.Show("bPushNigth");
            if (FSaveData.Instance.bPushEvent)
            {
                string gmtAllTopic = "gmt_all";
                fcmTopics.Add(gmtAllTopic);
#if UNITY_ANDROID
                Firebase.Messaging.FirebaseMessaging.Subscribe("/topics/" + gmtAllTopic);
                Firebase.Messaging.FirebaseMessaging.Subscribe(gmtAllTopic);
#endif
            }
            else
                UnSubscribeGlobalPush();
        }
        else
        {
            UnSubscribeGlobalPush();
            UnSubscribeNightPush();
            SetFCMSubscribeNightPush(ref fcmTopics);
        }


        if (!AppMgr.Instance.configData.m_LivePush)
        {
            string devTopic = "dev";
            fcmTopics.Add(devTopic);
#if UNITY_ANDROID
            Firebase.Messaging.FirebaseMessaging.Subscribe("/topics/" + devTopic);
            Firebase.Messaging.FirebaseMessaging.Subscribe(devTopic);
#endif
        }

#if UNITY_IOS
        OnSetSubscribe(fcmTopics);
#endif
    }

    private void SetFCMSubscribeGlobal(ref List<string> topicList, string topicLanguage)
    {
        string languageTopic = "lang_" + topicLanguage;
#if UNITY_ANDROID
        Firebase.Messaging.FirebaseMessaging.Subscribe("/topics/" + languageTopic);
        Firebase.Messaging.FirebaseMessaging.Subscribe(languageTopic);
        topicList.Add(languageTopic);
#elif UNITY_IOS
        topicList.Add(languageTopic);
#endif
    }

    /// <summary>
    /// 글로벌 야간 푸시 토픽 구독
    /// </summary>
    /// <param name="timeOffSet">시간 차이</param>
    /// <param name="topicList">토픽 리스트</param>
    private void SetFCMSubscribeGlobalWithNight(int timeOffSet, ref List<string> topicList)
    {
        string allTopic = "gmt_" + timeOffSet;

        Firebase.Messaging.FirebaseMessaging.Subscribe(allTopic);
        topicList.Add(allTopic);
    }

    //FCM주제 구독 해제
    public void UnSubscribeLanguage()
    {

        if (FLocalizeString.Language != eLANGUAGE.CHT)
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("lang_zht");    //중국어 - 간체
        if (FLocalizeString.Language != eLANGUAGE.CHS)
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("lang_zhs");    //중국어 - 번체
        if (FLocalizeString.Language != eLANGUAGE.ENG)
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("lang_en");    //영어
        if (FLocalizeString.Language != eLANGUAGE.JPN)
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("lang_ja");    //일본어
        if (FLocalizeString.Language != eLANGUAGE.KOR)
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("lang_ko");    //한국어
        if (FLocalizeString.Language != eLANGUAGE.ESP)
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("lang_es");    //스페인어

        Firebase.Messaging.FirebaseMessaging.Unsubscribe("all_zht");    //중국어 - 간체
        Firebase.Messaging.FirebaseMessaging.Unsubscribe("all_zhs");    //중국어 - 번체
        Firebase.Messaging.FirebaseMessaging.Unsubscribe("all_en");    //영어
        Firebase.Messaging.FirebaseMessaging.Unsubscribe("all_ja");    //일본어
        Firebase.Messaging.FirebaseMessaging.Unsubscribe("all_ko");    //한국어
        Firebase.Messaging.FirebaseMessaging.Unsubscribe("all_es");    //스페인어

        Firebase.Messaging.FirebaseMessaging.Unsubscribe("all_zht_dev");    //중국어 - 간체
        Firebase.Messaging.FirebaseMessaging.Unsubscribe("all_zhs_dev");    //중국어 - 번체
        Firebase.Messaging.FirebaseMessaging.Unsubscribe("all_en_dev");    //영어
        Firebase.Messaging.FirebaseMessaging.Unsubscribe("all_ja_dev");    //일본어
        Firebase.Messaging.FirebaseMessaging.Unsubscribe("all_ko_dev");    //한국어
        Firebase.Messaging.FirebaseMessaging.Unsubscribe("all_es_dev");    //스페인어
#if UNITY_ANDROID
        Firebase.Messaging.FirebaseMessaging.Unsubscribe("aos_zht");    //중국어 - 간체
        Firebase.Messaging.FirebaseMessaging.Unsubscribe("aos_zhs");    //중국어 - 번체
        Firebase.Messaging.FirebaseMessaging.Unsubscribe("aos_en");    //영어
        Firebase.Messaging.FirebaseMessaging.Unsubscribe("aos_ja");    //일본어
        Firebase.Messaging.FirebaseMessaging.Unsubscribe("aos_ko");    //한국어
        Firebase.Messaging.FirebaseMessaging.Unsubscribe("aos_es");    //스페인어

        Firebase.Messaging.FirebaseMessaging.Unsubscribe("aos_zht_dev");    //중국어 - 간체
        Firebase.Messaging.FirebaseMessaging.Unsubscribe("aos_zhs_dev");    //중국어 - 번체
        Firebase.Messaging.FirebaseMessaging.Unsubscribe("aos_en_dev");    //영어
        Firebase.Messaging.FirebaseMessaging.Unsubscribe("aos_ja_dev");    //일본어
        Firebase.Messaging.FirebaseMessaging.Unsubscribe("aos_ko_dev");    //한국어
        Firebase.Messaging.FirebaseMessaging.Unsubscribe("aos_es_dev");    //스페인어
#elif UNITY_IOS
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("ios_zht");    //중국어 - 간체
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("ios_zhs");    //중국어 - 번체
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("ios_en");    //영어
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("ios_ja");    //일본어
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("ios_ko");    //한국어
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("ios_es");    //스페인어
                                                           
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("ios_zht_dev");    //중국어 - 간체
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("ios_zhs_dev");    //중국어 - 번체
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("ios_en_dev");    //영어
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("ios_ja_dev");    //일본어
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("ios_ko_dev");    //한국어
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("ios_es_dev");    //스페인어
#endif

        //개발모드 푸시는 항상 해제
        Firebase.Messaging.FirebaseMessaging.Unsubscribe("dev");
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        Debug.Log("Received a new message from: " + e.Message.From);
        Debug.Log("Received a new message from: " + e.Message.Notification.Body);
    }

	public void SendPushToken() {
		string strorg = m_fcmToken;
        if( string.IsNullOrEmpty( strorg ) ) {
            return;
		}

        //string strorg = PlayerPrefs.GetString("FCM_PUSH_TOKEN_ORIGIN", "");
        string strser = PlayerPrefs.GetString( "FCM_PUSH_TOKEN_SERVER", string.Empty );

        //둘다 없으면 보낼필요 없다.
        if ( string.IsNullOrEmpty( strorg ) && string.IsNullOrEmpty( strser ) ) {
            return;
        }

		//같으면 보낼필요 없다.
		if ( strorg.Equals( strser ) ) {
			return;
		}

		//클리면 보내라
		GameInfo.Instance.Send_ReqPushNotifiTokenSet( strorg, OnPushNotifiTokenSet );
	}

	public void OnPushNotifiTokenSet(int _result, PktMsgType pktmsg)
    {
        if (_result != 0)
            return;
        //GameInfoRecv -> RecvAckPushNotifiTokenSet에서 서버에서 받은 토큰 저장
    }

    public void OnSetSubscribe(List<string> topics)
    {
        if (string.IsNullOrEmpty(m_fcmToken))
        {
            Debug.Log("FCM Token is NULL");
            return;
        }

		if (topics == null || topics.Count <= 0)
		{
			return;
		}

        StartCoroutine(SendPostRequest(topics));
    }

    IEnumerator SendPostRequest(List<string> topics)
    {
        string defaultURL = "https://iid.googleapis.com/iid/v1/" + m_fcmToken;
        Debug.Log("Add Topic default URL : " + defaultURL);

        for (int i = 0; i < topics.Count; i++)
        {
            string url = defaultURL + "/rel/topics/" + topics[i];
            Debug.Log("Add Topic Real URL : " + url);

            UnityWebRequest www = UnityWebRequest.Post(url, "");
			if(www == null || AppMgr.Instance.configData == null)
			{
				break;
			}

            www.SetRequestHeader("authorization", "key=" + AppMgr.Instance.configData.GetPushServerKey());
            //www.SetRequestHeader("authorization", "key=AAAACsNuQZ4:APA91bHfCyik1SKHKIY_fy8xbkbChgzXUKkq_jr7tcixDxwzLMsmMx3FOj57DxqZzHAwXfU0miOvN9Dlg8lDwHXPN-jJf5_5DNF6ozTFpgRStV-Yz1YhVFOTFL9YvBCv13iXivT-zKKn");// + AppMgr.Instance.configData.GetPushServerKey());;

            yield return www.SendWebRequest();

			if (www == null || AppMgr.Instance.configData == null)
			{
				break;
			}

			if (www.isNetworkError || www.isHttpError)
                Debug.LogError("Error : " + www.error);
            else
                Debug.Log(topics[i] + "Subscribe Success");

            www.Dispose();
        }


        yield return null;
    }

    /// <summary>
    /// 야간만 안받는 토픽 구독
    /// </summary>
    /// <param name="fcmTopics">토픽 리스트</param>
    public void SetFCMSubscribeNightPush(ref List<string> fcmTopics)
    {
        TimeSpan timeOffSet = TimeZoneInfo.Local.BaseUtcOffset;

        Debug.LogError("TimeOffSet : " + timeOffSet + " / " + timeOffSet.Hours);

        int offSetHours = timeOffSet.Hours;
        SetFCMSubscribeGlobalWithNight(offSetHours, ref fcmTopics);

        if (!AppMgr.Instance.configData.m_LivePush)
        {
            string devTopic = "dev";
            fcmTopics.Add(devTopic);
#if UNITY_ANDROID
            Firebase.Messaging.FirebaseMessaging.Subscribe("/topics/" + devTopic);
            Firebase.Messaging.FirebaseMessaging.Subscribe(devTopic);
#endif
        }
    }

    /// <summary>
    /// 글로벌 야간 푸시 토픽 해제
    /// </summary>
    public void UnSubscribeNightPush()
    {
        for (int i = -12; i <= 14; i++)
        {
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("all_night_" + i + "_zht");    //중국어 - 간체
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("all_night_" + i + "_zhs");    //중국어 - 번체
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("all_night_" + i + "_en");    //영어
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("all_night_" + i + "_ja");    //일본어
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("all_night_" + i + "_ko");    //한국어
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("all_night_" + i + "_es");    //스페인어

            Firebase.Messaging.FirebaseMessaging.Unsubscribe("all_night_" + i + "_zht_dev");    //중국어 - 간체
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("all_night_" + i + "_zhs_dev");    //중국어 - 번체
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("all_night_" + i + "_en_dev");    //영어
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("all_night_" + i + "_ja_dev");    //일본어
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("all_night_" + i + "_ko_dev");    //한국어
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("all_night_" + i + "_es_dev");    //스페인어

            //210622
            Firebase.Messaging.FirebaseMessaging.Unsubscribe("gmt_" + i);
        }
    }

    public void UnSubscribeGlobalPush()
    {
        Firebase.Messaging.FirebaseMessaging.Unsubscribe("gmt_all");
    }

    private void AddLocalNotificationGoods(int notificationId, string title, int txtIndex, int ticketNow, int ticketMax, int updateTimeSec)
    {
        if(ticketNow >= ticketMax)
        {
            return;
        }

        int fullUpdateTime = (ticketMax - ticketNow) * updateTimeSec;

        DateTime nowTime = DateTime.Now;
        DateTime time = nowTime.AddSeconds((double)fullUpdateTime);

        AddScheduleNotification(notificationId, title, FLocalizeString.Instance.GetText(txtIndex), 0, 0, (ticketMax - ticketNow) * updateTimeSec);
    }
}

