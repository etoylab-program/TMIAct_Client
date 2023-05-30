using System.Collections;

public class NetMgrDefault : NetMgr
{
    public void DoInit()
    {; }
    public IEnumerator DoProcess()
    {
        yield return null;
    }
    public void DoRelease(eConnectKind _conKind)
    {; }

    public bool DoSetActiveNet(eServerType _svrType, bool _force = false)
    {
        return true;
    }
    public void DoSetConnectInfo(eServerType _svrType, ref NetConnectInfo _info)
    {; }
    public bool DoDisConnectByActiveNet(bool _canReconnect)
    {
        return false;
    }
    public bool DoConnectByActiveNet()
    {
        return false;
    }

    public void DoStartTimer(eServerType _svrType)
    {; }
    public void DoStopTimer(eServerType _svrType)
    {; }

    public bool DoAllInOneConnect(eServerType _svrType, bool _force = false)
    {
        return false;
    }
    public bool DoAllInOneReConnect(eServerType _svrType, bool _force = false)
    {
        return false;
    }
    public bool DoAllInOneConnect(eServerType _svrType, ref NetConnectInfo _info, bool _force = false)
    {
        return false;
    }
    public eServerType GetActiveSvrType() { return eServerType._END_USER_SERVER_; }
    public bool IsConnectingAboutActiveSvr() { return false; }
    public void DoRegiRecvInPktGbl(PacketGlobal.Event _event) {; }
}
