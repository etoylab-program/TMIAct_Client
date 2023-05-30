using System;
//using System.Timers;
using System.Collections;
using Nettention.Proud;
using UnityEngine;


//public class NetMgrProudNet : NetMgr
public class NetMgrProudNet : MonoSingleton<NetMgrProudNet>, NetMgr
{
    System.Guid[] PN_PKT_VER_ARR = {
                                new System.Guid("{0x3ae33249,0xecc6,0x0000,{0xbc,0x5d,0x7b,0xa,0x99,0x9c,0x07,0x00}}")
                                , new System.Guid("{0x3ae33249,0xecc6,0x0000,{0xbc,0x5d,0x7b,0xa,0x99,0x9c,0x07,0x01}}")
                                , new System.Guid("{0x3ae33249,0xecc6,0x0000,{0xbc,0x5d,0x7b,0xa,0x99,0x9c,0x07,0x02}}")
                                , new System.Guid("{0x3ae33249,0xecc6,0x0000,{0xbc,0x5d,0x7b,0xa,0x99,0x9c,0x07,0x03}}")
                                , new System.Guid("{0x3ae33249,0xecc6,0x0000,{0xbc,0x5d,0x7b,0xa,0x99,0x9c,0x07,0x04}}")
                                , new System.Guid("{0x3ae33249,0xecc6,0x0000,{0xbc,0x5d,0x7b,0xa,0x99,0x9c,0x07,0x05}}")
                                , new System.Guid("{0x3ae33249,0xecc6,0x0000,{0xbc,0x5d,0x7b,0xa,0x99,0x9c,0x07,0x06}}")
                                , new System.Guid("{0x3ae33249,0xecc6,0x0000,{0xbc,0x5d,0x7b,0xa,0x99,0x9c,0x07,0x07}}")
                                , new System.Guid("{0x3ae33249,0xecc6,0x0000,{0xbc,0x5d,0x7b,0xa,0x99,0x9c,0x07,0x08}}")
                                , new System.Guid("{0x3ae33249,0xecc6,0x0000,{0xbc,0x5d,0x7b,0xa,0x99,0x9c,0x07,0x09}}")
                            };

    NetConnectInfo[] m_InfoArr = new NetConnectInfo[(int)eServerType._END_USER_SERVER_];
    PN_ClientEventBase[] m_EventArr = new PN_ClientEventBase[(int)eServerType._END_USER_SERVER_];
    //Timer[] m_NetTimerArr = new Timer[(int)eServerType._MAX_];
    IEnumerator[] m_TimeFuncArr = new IEnumerator[(int)eServerType._END_USER_SERVER_];
    NetClient[] m_NetClientArr = new NetClient[(int)eServerType._END_USER_SERVER_];
    PacketGlobal[] m_PktGlobal = new PacketGlobal[(int)eServerType._END_USER_SERVER_];
    NetClient m_ActiveNet = null;
    //NetClient m_CommunityNet = null;
    //NetClient m_ChatNet = null;
    DisconnectArgs m_DisconnectArgs = new DisconnectArgs();
    ByteArray m_Comment = new ByteArray();



    NetConnectionParam m_ConnectParam = new NetConnectionParam();

    Byte m_ActiveSvrTP  = (Byte)eServerType._END_USER_SERVER_;

    public NetMgrProudNet()
    {
        NETStatic.SetNetMgr(this);

        m_DisconnectArgs.gracefulDisconnectTimeoutMs = 0;
        m_DisconnectArgs.disconnectSleepIntervalMs = 0;

        var pktLogin = new PacketLogin();
        var pktLobby = new PacketLobby();
        var pktBattle = new PacketBattle();
        var pktProd = new PacketProduct();
        NETStatic.DoSetPktLogin(pktLogin);
        NETStatic.DoSetPktLobby(pktLobby);
        NETStatic.DoSetPktBattle(pktBattle);
        NETStatic.DoSetPktProduct(pktProd);


        var evtObj      = __DoAddNetObjects(eServerType.LOGIN, new PN_ClientEventLogin());
        pktLogin.DoAttach(evtObj);
        //pktItem.DoAttach(evtObj);
        evtObj          = __DoAddNetObjects(eServerType.LOBBY, new PN_ClientEventLobby());
        pktLobby.DoAttach(evtObj);
        pktProd.DoAttach(evtObj);
        evtObj          = __DoAddNetObjects(eServerType.BATTLE, new PN_ClientEventBattle());
        pktBattle.DoAttach(evtObj);
        //m_CommunityNet  = __DoAddNetObjects(eServerType.COMMUNITY, new PN_ClientEventBase());
        //m_ChatNet       = __DoAddNetObjects(eServerType.CHAT, new PN_ClientEventBase());


        m_ConnectParam.protocolVersion = new Nettention.Proud.Guid();
        /** 클라이언트-서버간에 TCP 핑퐁을 주고 받으며, 이 핑퐁이 너무 오랫동안 되지 못하면 연결을 끊어버리는 기능입니다.
		기본값은 true입니다. 하위호환성을 위해 true일 뿐이며, 여러분은 가급적이면 이 값을 false로 바꾸는 것을 권장합니다.

		일반적인 경우 이것을 켜지 않으셔도, NetClient와 NetServer는 TCP 연결 해제를 즉시 혹은 20초 정도 지나서 감지합니다.

		이보다 더 빠른 시간 안에 TCP 연결 해제를 감지하고 싶다면 이 값을 true로 만들고 SetDefaultTimeoutTime이나 SetTimeoutTime을 추가적으로 사용하십시오.

		그러나 주의가 필요합니다. 이것을 true로 설정하는 경우 평소 인터넷 품질이 나쁜 나라나 무선 신호가 약한 네트워킹에서 의도치 않은 연결해제가 일어날 수 있습니다.
		인터넷 환경이 나쁘지만 그래도 조금이나마 통신이 되는 것을, 통신 불가능으로 오판하기 때문입니다. 
		
		이 값이 true이면, 모바일 기기에서 NetClient가 사용중일 때 주의사항이 있습니다. 프로그램이 오랫동안 백그라운드에 있을 때 즉 일시정지 상태에서 수십초 정도의
		오랜 시간이 지나면, 서버에서는 연결해제 즉 OnClientLeave나 OnClientOffline이 발생할 수 있습니다. 
		*/
        m_ConnectParam.m_closeNoPingPongTcpConnections = false;
        m_ConnectParam.enableAutoConnectionRecovery = true;    // "http://guide.nettention.com/cpp_ko#acr" >연결 유지 기능 사용하기</a> 를 켜거나 끕니다. 기본적으로 꺼져 있습니다.


        __DoAddTimer(eServerType.LOGIN, __DoCoroutineToActiveNet());
        __DoAddTimer(eServerType.LOBBY, __DoCoroutineToActiveNet());
        __DoAddTimer(eServerType.BATTLE, __DoCoroutineToActiveNet());
        //__DoAddTimer(eServerType.COMMUNITY, __DoCoroutineToCommunityNet());
        //__DoAddTimer(eServerType.CHAT, __DoCoroutineToChatNet());
    }
    NetClient __DoAddNetObjects(eServerType _svrTP, PN_ClientEventBase _event)
    {
        NetClient netClient         = new NetClient();
        PacketGlobal packetGbl      = new PacketGlobal();

        m_NetClientArr[(int)_svrTP] = netClient;
        m_EventArr[(int)_svrTP]     = _event;
        m_PktGlobal[(int)_svrTP]    = packetGbl;

        _event.DoInit(_svrTP, netClient);
        packetGbl.DoAttach(netClient);

        return netClient;
    }
    //Timer __DoAddTimer(eServerType _svrTP, ElapsedEventHandler _elapsed, double _interVal, bool _enabled = false)
    //{
    //    var timer = new Timer(_interVal);
    //    m_NetTimerArr[(int)_svrTP]  = timer;

    //    //timer.Interval  = _interVal;
    //    timer.Elapsed   += _elapsed;
    //    //timer.Elapsed   += new System.Timers.ElapsedEventHandler(_elapsed);
    //    timer.Enabled   = _enabled;

    //    return timer;
    //}
    void __DoAddTimer(eServerType _svrTP, IEnumerator _routine)
    {
        m_TimeFuncArr[(int)_svrTP] = _routine;
        return ;
    }

    void Awake()
    {
        DontDestroyOnLoad();
    }
    //protected override void OnApplicationQuit()
    //{
    //    Debug.Log("NetMgrProudNet.OnApplicationQuit", this);
    //    this.DoRelease();

    //    base.OnApplicationQuit();
    //}

    public void DoInit()
    {
        return;
    }

    public IEnumerator DoProcess()
    {
        StartCoroutine("");

        if (null == m_ActiveNet)
            yield return null;
        //while (true)
        //{
        //    // 코루틴을 이용해 m_WaitTime 동안 대기.
        //    yield return new WaitForSeconds(m_WaitTimeSec);
        //    _DoNetProcess();
        //}
        yield return null;
    }
    public void DoRelease(eConnectKind _conKind)
    {
        for (UInt64 loop = (UInt64)eServerType._START_USER_SERVER_; loop < (UInt64)eServerType._END_USER_SERVER_; ++loop)
        {
            __DoDisconnectNetClient(m_NetClientArr[loop], _conKind);
        }
        return;
    }
    public bool DoSetActiveNet(eServerType _svrType, bool _force = false)
    {
        if (false == _force && (eServerType)m_ActiveSvrTP == _svrType)
            return false;

        if (eServerType._END_USER_SERVER_ == _svrType)
        {
            if (null != m_ActiveNet)
            {
                Debug.Log(string.Format("->> [DoSetActiveNet] LogOut Disconnect type:[{0}] addr_Port[{1}:{2}] ->>", m_ActiveSvrTP, m_ConnectParam.serverIP, m_ConnectParam.serverPort));
                this.DoStopTimer((eServerType)m_ActiveSvrTP);
                __DoDisconnectNetClient(m_ActiveNet, eConnectKind.DISCONNECT_LOGOUT);
            }

            m_ActiveSvrTP   = (Byte)_svrType;
            m_ActiveNet     = null;

            Debug.Log(string.Format("[DoSetActiveNet] fail : eServerType[{0}] ", _svrType));
            return false;
        }


        // 채팅 서버와 커뮤니티 서버는 계속 연결되는 서버이기 때문에 설정하지 못하게 합니다.
        if (eServerType.COMMUNITY == _svrType
            || eServerType.CHAT == _svrType)
        {
            Debug.Log(string.Format("[DoSetActiveNet] fail : eServerType[{0}] ", _svrType));
            return false;
        }

        if (null != m_ActiveNet)
        {
            Debug.Log(string.Format("->> [DoSetActiveNet] Move Disconnect type:[{0}] addr_Port[{1}:{2}] ->>", m_ActiveSvrTP, m_ConnectParam.serverIP, m_ConnectParam.serverPort));
            __DoDisconnectNetClient(m_ActiveNet, eConnectKind.DISCONNECT_MOVE_SVR);
            this.DoStopTimer((eServerType)m_ActiveSvrTP);
        }

        m_ActiveSvrTP   = (Byte)_svrType;
        m_ActiveNet     = m_NetClientArr[m_ActiveSvrTP];
        return true;
    }
    public void DoSetConnectInfo(eServerType _svrType, ref NetConnectInfo _info)
    {
        if (eServerType._END_USER_SERVER_ <= (eServerType)_svrType)
            return ;

        m_InfoArr[(int)_svrType]    = _info;
    }
    public bool DoDisConnectByActiveNet(bool _canReconnect)
    {
        if (null == m_ActiveNet)
            return false;

        eConnectKind kind = eConnectKind.DISCONNECT_LOGOUT;
        if (true == _canReconnect)
            kind = eConnectKind.DISCONNECT_ERR;

        Debug.Log(string.Format("->> [DoDisConnectByActiveNet] Disconnect type:[{0}] addr_Port[{1}:{2}] Kind[{3}] ->>", m_ActiveSvrTP, m_ConnectParam.serverIP, m_ConnectParam.serverPort, kind));
        __DoDisconnectNetClient(m_ActiveNet, kind);
        this.DoStopTimer((eServerType)m_ActiveSvrTP);

        //m_ActiveSvrTP = (Byte)eServerType._END_USER_SERVER_;
        //m_ActiveNet = null;
        return true;
    }
    public bool DoConnectByActiveNet()
    {
        if (eServerType._END_USER_SERVER_ <= (eServerType)m_ActiveSvrTP)
            return false;

        var info        = m_InfoArr[m_ActiveSvrTP];
        m_ConnectParam.protocolVersion.Set(PN_PKT_VER_ARR[m_ActiveSvrTP]);
        m_ConnectParam.serverIP      = info.ipAddr_;
        m_ConnectParam.serverPort    = info.port_;

        Debug.Log(string.Format("->> Connect Client To Server type:{0} : addr_Port[{1}:{2}] ->>", (eServerType)m_ActiveSvrTP, info.ipAddr_, info.port_));
        bool ret        = m_ActiveNet.Connect(m_ConnectParam);
        this.DoStartTimer((eServerType)m_ActiveSvrTP);
        NETStatic.DoSetPktGlobal(m_PktGlobal[m_ActiveSvrTP]);
        return ret;
    }
    public bool DoAllInOneConnect(eServerType _svrType, bool _force)
    {
        if (eServerType._END_USER_SERVER_ <= (eServerType)_svrType)
        {
            Debug.Log("DoAllInOneConnect failure _svrType");
            return false;
        }

        return this.DoAllInOneConnect(_svrType, ref m_InfoArr[(int)_svrType], _force);
    }
    public bool DoAllInOneConnect(eServerType _svrType, ref NetConnectInfo _info, bool _force)
    {
        if (false == this.DoSetActiveNet(_svrType, _force))
        {
            Debug.Log("DoAllInOneConnect failure DoSetActiveNet");
            return false;
        }

        this.DoSetConnectInfo(_svrType, ref _info);
        return this.DoConnectByActiveNet();
    }

    public bool DoAllInOneReConnect(eServerType _svrType, bool _force)
    {
        //this.DoStopTimer((eServerType)m_ActiveSvrTP);
        this.DoDisConnectByActiveNet(true);

        this.DoSetConnectInfo(_svrType, ref m_InfoArr[(int)_svrType]);
        return this.DoConnectByActiveNet();
    }

    public eServerType GetActiveSvrType() { return (eServerType)m_ActiveSvrTP;  }
    public bool IsConnectingAboutActiveSvr()
    {
        if (null == m_ActiveNet)
            return false;

        return m_ActiveNet.HasServerConnection();
    }
    public void DoRegiRecvInPktGbl(PacketGlobal.Event _event)
    {
        for (UInt64 loop = (UInt64)eServerType.LOGIN; loop < (UInt64)eServerType._END_USER_SERVER_; ++loop)
            _event(m_PktGlobal[loop]);
    }

    public void DoStartTimer(eServerType _svrType)
    {
        if (eServerType._END_USER_SERVER_ <= (eServerType)m_ActiveSvrTP)
            return ;

        StartCoroutine(m_TimeFuncArr[(int)_svrType]);
        //StartCoroutine(m_TimeObjArr[(int)_svrType].DoTimeRun());
        //m_TimeObjArr[(int)_svrType].DoStart();
        //m_NetTimerArr[(int)_svrType].Start();
    }
    public void DoStopTimer(eServerType _svrType)
    {
        if (eServerType._END_USER_SERVER_ <= (eServerType)m_ActiveSvrTP)
            return;

        StopCoroutine(m_TimeFuncArr[(int)_svrType]);
        //m_NetTimerArr[(int)_svrType].Stop();
    }

    //void __DoActiveNetProc(object _sender, EventArgs _eArgs)
    //{
    //    if (null == m_ActiveNet)    return ;

    //    m_ActiveNet.FrameMove();
    //}
    //void __DoCommunityNetProc(object _sender, EventArgs _eArgs)
    //{
    //    m_CommunityNet.FrameMove();
    //}
    //void __DoChatNetProc(object _sender, EventArgs _eArgs)
    //{
    //    m_ChatNet.FrameMove();
    //}
    IEnumerator __DoCoroutineToActiveNet()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.05f);
        while (true)
        {
            if (null == m_ActiveNet)
                continue;

            yield return waitForSeconds;
            m_ActiveNet.FrameMove();
        }
    }
    //IEnumerator __DoCoroutineToCommunityNet()
    //{
    //    WaitForSeconds waitForSeconds = new WaitForSeconds(0.05f);
    //    while (true)
    //    {
    //        yield return waitForSeconds;
    //        m_CommunityNet.FrameMove();
    //    }
    //}
    //IEnumerator __DoCoroutineToChatNet()
    //{
    //    WaitForSeconds waitForSeconds = new WaitForSeconds(0.05f);
    //    while (true)
    //    {
    //        yield return waitForSeconds;
    //        m_ChatNet.FrameMove();
    //    }
    //}
    void __DoDisconnectNetClient(NetClient _netClient, eConnectKind _conKind = eConnectKind.DISCONNECT_ERR)
    {
        if (null == _netClient)
            return;

        m_Comment.Clear();
        m_Comment.Add((byte)_conKind);
        m_DisconnectArgs.SetComment(m_Comment);
        _netClient.Disconnect(m_DisconnectArgs);
    }
}
