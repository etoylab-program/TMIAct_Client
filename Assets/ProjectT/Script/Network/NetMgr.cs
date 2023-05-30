using System;
using System.Collections;


static public class NETStatic
{
    public delegate void NetMgrEvent(NetMgr _netMgr);

    static NetMgr m_Mgr                     = null;
    static System.Guid m_CreditKey          = System.Guid.Empty;
    static PacketGlobalBase m_PktGlobal     = new PacketGlobalBase();
    static PacketLoginBase m_PktLogin       = new PacketLoginBase();
    static PacketLobbyBase m_PktLobby       = new PacketLobbyBase();
    static PacketBattleBase m_PktBattle     = new PacketBattleBase();
    static PacketProductBase m_PktProd      = new PacketProductBase();

    static public void SetNetMgr(NetMgr _mgr) { m_Mgr = _mgr; }
    static public NetMgr Mgr                    { get { return m_Mgr; } }
    static public PacketGlobalBase PktGbl       { get { return m_PktGlobal; } }
    static public PacketLoginBase PktLgn        { get { return m_PktLogin; } }
    static public PacketLobbyBase PktLby        { get { return m_PktLobby; } }
    static public PacketBattleBase PktBtl       { get { return m_PktBattle; } }
    static public PacketProductBase PktProd     { get { return m_PktProd; } }
    static public System.Guid creditKey         { get { return m_CreditKey; } }
    static public void DoSetPktGlobal(PacketGlobal _pkt) { m_PktGlobal = _pkt; }
    static public void DoSetPktLogin(PacketLogin _pkt) { m_PktLogin = _pkt; }
    static public void DoSetPktLobby(PacketLobby _pkt) { m_PktLobby = _pkt; }
    static public void DoSetPktBattle(PacketBattle _pkt) { m_PktBattle = _pkt; }
    static public void DoSetPktProduct(PacketProduct _pkt) { m_PktProd = _pkt; }

    static public System.UInt32 GetNowTime_ms()
    {
        System.TimeSpan span = new System.TimeSpan(System.DateTime.Now.Ticks);
        return (System.UInt32)span.TotalMilliseconds;
    }
    static public System.UInt32 GetRand(int _min, int _max)
    {
        return (System.UInt32)UnityEngine.Random.Range(_min, _max + 1);
    }


    static public void DoInitNetwork(bool activeFlag)
    {
        if (null != m_Mgr)
            return;

        if (false == activeFlag)
            m_Mgr       = new NetMgrDefault();
        else
            m_Mgr       = NetMgrProudNet.Instance;
        m_Mgr.DoInit();
    }
    static public void DoSetCreditKey(System.Guid _guid) { m_CreditKey = _guid; }
    static public void DoClearCreditKey() { m_CreditKey = System.Guid.Empty; }
};

public struct NetConnectInfo
{
    public String ipAddr_;
    public UInt16 port_;

    public NetConnectInfo(String _addr = "", UInt16 _port = 0)
    {
        ipAddr_ = _addr;
        port_ = _port;
    }
}

public interface NetMgr
{
    void DoInit();
    IEnumerator DoProcess();
    void DoRelease(eConnectKind _conKind /*= eConnectKind.DISCONNECT_ERR*/);
    bool DoSetActiveNet(eServerType _svrType, bool _force = false);
    void DoSetConnectInfo(eServerType _svrType, ref NetConnectInfo _info);
    bool DoDisConnectByActiveNet(bool _canReconnect = false);
    bool DoConnectByActiveNet();
    void DoStartTimer(eServerType _svrType);
    void DoStopTimer(eServerType _svrType);
    bool DoAllInOneConnect(eServerType _svrType, bool _force = false);
    bool DoAllInOneReConnect(eServerType _svrType, bool _force = false);
    bool DoAllInOneConnect(eServerType _svrType, ref NetConnectInfo _info, bool _force = false);

    eServerType GetActiveSvrType();
    bool IsConnectingAboutActiveSvr();
    void DoRegiRecvInPktGbl(PacketGlobal.Event _event);
}