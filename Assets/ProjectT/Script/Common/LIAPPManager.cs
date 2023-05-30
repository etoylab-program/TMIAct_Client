using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_IOS && LIAPP_FOR_DISTRIBUTE
using System.Runtime.InteropServices;
#endif

public class LIAPPManager : MonoSingleton<LIAPPManager>
{
    public delegate void LIAPPDelegate();

    public static int LIAPP_EXCEPTION = -1;
    public static int LIAPP_SUCCESS = 0;
    public static int LIAPP_DETECTED = 1;
    public static int LIAPP_DETECTED_ANTI_DEBUGGER = 1;
    public static int LIAPP_DETECTED_INTEGRITY = 2;
    public static int LIAPP_DETECTED_VM = 3;
    public static int LIAPP_DETECTED_ROOTING = 7;
    public static int LIAPP_DETECTED_HACKING_TOOL = 8;

    private bool bIsStarted = false;
    private int nRet = LIAPP_EXCEPTION;

    private string strLiappMessage;
    private int _intLA1;
#if UNITY_ANDROID && LIAPP_FOR_DISTRIBUTE
    AndroidJavaClass _LiappAgent;
#elif UNITY_IOS && LIAPP_FOR_DISTRIBUTE
    [DllImport("__Internal")]
    private static extern int LiappLA1();

    [DllImport("__Internal")]
    private static extern int LiappLA2();

    [DllImport("__Internal")]
    private static extern string LiappGA(string pszUser_key_from_server);

    [DllImport("__Internal")]
    private static extern string LiappGetMessage();
#endif
    private LIAPPDelegate _callBack = null;

    // Use this for initialization
    public void CheckLIAppStart(LIAPPDelegate callBack)
    {
        DontDestroyOnLoad(this);
        _callBack = callBack;

#if !LIAPP_FOR_DISTRIBUTE
        if (_callBack != null)
            _callBack();
        return;
#endif

        if (bIsStarted)
        {
            if (_callBack != null)
                _callBack();
            return;
        }

        bool bconnect = false;

        if (NETStatic.Mgr != null)
        {
            if (NETStatic.Mgr.IsConnectingAboutActiveSvr())
            {
                bconnect = true;
            }
        }

        if (!bconnect)
        {
            GameInfo.Instance.DoInitGame(AppMgr.Instance.configData.m_Network);
            GameInfo.Instance.SvrConnect_Login(false, OnNetSvrConnect_Login);
            return;
        }

        LIAPP_Start();
    }

    public void OnNetSvrConnect_Login(int result, PktMsgType pktmsg)
    {
        if (Nettention.Proud.ErrorType.Ok == (Nettention.Proud.ErrorType)result)
        {
            LIAPP_Start();
        }
    }

    public void OnNet_AckClientSecurityInfo(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        PktInfoClientSecurityAck securityPkt = pktmsg as PktInfoClientSecurityAck;

        string userKey = securityPkt.secuKey_.str_;

        string authtoken = string.Empty;
#if UNITY_EDITOR
        authtoken = "TEST_DEV_KEY";
#else
    #if UNITY_ANDROID && LIAPP_FOR_DISTRIBUTE
                    authtoken = _LiappAgent.CallStatic<string>("GA", userKey);
    #elif UNITY_IOS && LIAPP_FOR_DISTRIBUTE
                    authtoken = this.GA(userKey);
    #endif
#endif


        if (string.IsNullOrEmpty(authtoken))
        {
            if (_callBack != null)
                _callBack();
        }
        else
        {
            GameInfo.Instance.Send_ReqClientSecurityVerify(authtoken, OnNet_AckClientSecurityVerify);
        }
    }

    public void OnNet_AckClientSecurityVerify(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        if (_callBack != null)
            _callBack();
    }


    void LIAPP_Start()
    {
        Log.Show("LIAPP_Start", Log.ColorType.Red);
#if UNITY_EDITOR
        nRet = LIAPP_SUCCESS;
#else
#if UNITY_ANDROID
    #if LIAPP_FOR_DISTRIBUTE
                            _LiappAgent = new AndroidJavaClass("com.lockincomp.liappagent.LiappAgentforUnity");

                            if (bIsStarted == false)
                            {
                                nRet = _LiappAgent.CallStatic<int>("LA1");
                                Debug.Log("LiAPP LA1 : " + nRet);
                            }
                            else
                            {
                                nRet = _LiappAgent.CallStatic<int>("LA2");
                                Debug.Log("LiAPP LA2 : " + nRet);
                            }
    #else
                            nRet = LIAPP_SUCCESS;
    #endif
#elif UNITY_IOS
    #if LIAPP_FOR_DISTRIBUTE
                            if (bIsStarted == false)
                            {
                                nRet = this.LA1();
                            }
                            else
                            {
                                nRet = this.LA2();
                            }
                            Debug.Log("[StartLiapp] nRet : " + nRet);
    #else
                            nRet = LIAPP_SUCCESS;
    #endif
#endif
#endif

        Log.Show("nRet : " + nRet);

        if (LIAPP_SUCCESS == nRet)
        {
            //Success

            // you don’t create to ‘user_key_from_server’.
            // ‘user_key_from_server’is from the app server.
            if (!bIsStarted)
            {
                bIsStarted = true;
                GameInfo.Instance.Send_ReqClientSecurityInfo(OnNet_AckClientSecurityInfo);
            }
            
            //string user_key = string.Empty;         //우리서버에서 난수 키 받음
            
            return;
            //string authtoken = _LiappAgent.CallStatic<string>("GA", user_key);
            // send ‘authtoken’ to app server for token verification.
        }
        else if (LIAPP_EXCEPTION == nRet)
        {
            //Exception. Bypass or Network Connected Error.
            Application.Quit();
            return;
        }
        else
        {
#if UNITY_ANDROID && LIAPP_FOR_DISTRIBUTE
            strLiappMessage = _LiappAgent.CallStatic<string>("GetMessage");
#elif UNITY_IOS && LIAPP_FOR_DISTRIBUTE
            strLiappMessage = this.GetMessage();
#endif

            Debug.Log("[StartLiapp] strLiappMessage : " + strLiappMessage);

            if (LIAPP_DETECTED == nRet)
            {

                //DETECTED USER BLOCK or ANTI DEBUGGING or ANTI TAMPER
            }
            else if (LIAPP_DETECTED_ROOTING == nRet)
            {
                //Rooting Detection!
            }
            else if (LIAPP_DETECTED_VM == nRet)
            {
                //Virtual Machine Detection!
            }
            else if (LIAPP_DETECTED_HACKING_TOOL == nRet)
            {
                //Hacking Tool Detection!
            }
            else
            {
                //unknown Error
            }

            //Process Terminate
            MessagePopup.OK((int)eTEXTID.TITLE_NOTICE, strLiappMessage, () => { Application.Quit(); });
            return;
        }

        if (_callBack != null)
            _callBack();
    }
    void ReStart()
    {
#if UNITY_ANDROID
    #if LIAPP_FOR_DISTRIBUTE
            if (bIsStarted == false)
            {
                nRet = _LiappAgent.CallStatic<int>("LA1");
                Debug.Log("LiAPP LA1 : " + nRet);
            }
            else
            {
                nRet = _LiappAgent.CallStatic<int>("LA2");
                Debug.Log("LiAPP LA2 : " + nRet);
            }
    #else
            nRet = LIAPP_SUCCESS;
    #endif
#elif UNITY_IOS
    #if LIAPP_FOR_DISTRIBUTE
            if (bIsStarted == false)
            {
                nRet = this.LA1();
            }
            else
            {
                nRet = this.LA2();
            }
            Debug.Log("[StartLiapp] nRet : " + nRet);
    #else
            nRet = LIAPP_SUCCESS;
    #endif
#endif
        if (LIAPP_SUCCESS == nRet)
        {
            //Success
            bIsStarted = true;
        }
        else if (LIAPP_EXCEPTION == nRet)
        {
            //Exception. Bypass or Network Connected Error.
        }
        else
        {
#if UNITY_ANDROID && LIAPP_FOR_DISTRIBUTE
            strLiappMessage = _LiappAgent.CallStatic<string>("GetMessage");
#elif UNITY_IOS && LIAPP_FOR_DISTRIBUTE
            strLiappMessage = this.GetMessage();
#endif
            if (LIAPP_DETECTED == nRet)
            {
                //DETECTED USER BLOCK or ANTI DEBUGGING or ANTI TAMPER
            }
            else if (LIAPP_DETECTED_ROOTING == nRet)
            {
                //Rooting Detection!
            }
            else if (LIAPP_DETECTED_VM == nRet)
            {
                //Virtual Machine Detection!
            }
            else if (LIAPP_DETECTED_HACKING_TOOL == nRet)
            {
                //Hacking Tool Detection!
            }
            else
            {
                //unknown Error
            }
            MessagePopup.OK((int)eTEXTID.TITLE_NOTICE, strLiappMessage, () => { Application.Quit(); });
            return;
            //Process Terminate
        }

    }

    public int LA1()
    {
#if UNITY_IOS
#if LIAPP_FOR_DISTRIBUTE
            return LiappLA1();
#else
            return LIAPP_EXCEPTION;
#endif
#endif
        return LIAPP_SUCCESS;
    }

    public int LA2()
    {
#if UNITY_IOS
#if LIAPP_FOR_DISTRIBUTE
                return LiappLA2();
#else
                return LIAPP_EXCEPTION;
#endif
#endif
        return LIAPP_SUCCESS;
    }

    public string GA(string pszUser_key_from_server)
    {
#if UNITY_IOS
#if LIAPP_FOR_DISTRIBUTE
                return LiappGA(pszUser_key_from_server);
#else
                return "DEV_TEST_KEY";
#endif
#endif
        return "DEV_TEST_KEY";
    }

    public string GetMessage()
    {
#if UNITY_IOS
#if LIAPP_FOR_DISTRIBUTE
                return LiappGetMessage();
#else
                return "DEV_TEST_MESSAGE";
#endif
#endif
        return "DEV_TEST_MESSAGE";
    }
}
