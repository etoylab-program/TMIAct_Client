using Nettention.Proud;

internal class PN_ClientEventBase
{
    NetClient m_Client      = null;
    eServerType m_SvrType   = eServerType._END_USER_SERVER_;

    protected NetClient _GetNet() { return m_Client; }


    public void DoInit(eServerType _svrTp, NetClient _client)
    {
        m_SvrType       = _svrTp;
        m_Client        = _client;
        m_Client.JoinServerCompleteHandler = OnJoinServerComplete;
        m_Client.LeaveServerHandler = OnLeaveServer;
        m_Client.ServerOnlineHandler = OnServerOnline;
        m_Client.ServerOfflineHandler = OnServerOffline;
        m_Client.ReceivedUserMessageHandler = OnReceiveUserMessage;
        m_Client.P2PMemberJoinHandler = OnP2PMemberJoin;
        m_Client.P2PMemberLeaveHandler = OnP2PMemberLeave;

        m_Client.ErrorHandler = OnError;
        m_Client.WarningHandler = OnWarning;
        m_Client.ExceptionHandler = OnException;
        m_Client.InformationHandler = OnInformation;

        m_Client.NoRmiProcessedHandler = OnNoRmiProcessed;
        m_Client.ChangeServerUdpStateHandler = OnChangeServerUdp;
        m_Client.ReceivedUserMessageHandler = OnReceiveUserMessage;
    }

    public void OnJoinServerComplete(ErrorInfo info, ByteArray replyFromServer)//
    {
        GameInfo.Instance.DoOnJoinServerComplete_Base(info, replyFromServer);

        if (ErrorType.Ok != info.errorType)
        {
            Debug.LogError(string.Format("OnJoinServerComplete Err - Type[{0}-{1}] Comment:{2}", info.errorType.ToString(), info.detailType.ToString(), info.comment));
            return;
        }

        _DoOnJoinServerComplete(info, replyFromServer);
    }
    void OnLeaveServer(ErrorInfo errorInfo)//
    {
        GameInfo.Instance.DoOnLeaveServer_Base(errorInfo);

        if (ErrorType.Ok != errorInfo.errorType)
        {
            Debug.LogError(string.Format("OnLeaveServer Err - Type[{0}-{1}] Comment:{2}", errorInfo.errorType.ToString(), errorInfo.detailType.ToString(), errorInfo.comment));
            return;
        }
 
        _DoOnLeaveServer(errorInfo);
    }
    void OnServerOnline(RemoteOnlineEventArgs args)
    {
        Debug.Log("[NetEvent] OnServerOnline HostID");
        _DoOnServerOnline(args);
    }
    void OnServerOffline(RemoteOfflineEventArgs args)
    {
        GameInfo.Instance.DoOnServerOffline(args);
        Debug.Log("[NetEvent] OnServerOffline HostID");
        _DoOnServerOffline(args);
    }
    void OnP2PMemberJoin(HostID memberHostID, HostID groupHostID, int memberCount, ByteArray customField)
    {
        Debug.Log("[NetEvent] Join P2P member " + memberHostID + " joined  group " + groupHostID + ".");
        _DoOnP2PMemberJoin(memberHostID, groupHostID, memberCount, customField);
    }
    void OnP2PMemberLeave(HostID memberHostID, HostID groupHostID, int memberCount)
    {
        Debug.Log("[NetEvent] Leave P2P member " + memberHostID + " left group " + groupHostID + ".");
        _DoOnP2PMemberLeave(memberHostID, groupHostID, memberCount);
    }
    void OnError(ErrorInfo errorInfo)
    {
        Debug.LogError("[NetEvent]Error : " + errorInfo.ToString());
        GameInfo.Instance.OnError_Base(errorInfo);
        _DoOnError(errorInfo);
    }
    void OnWarning(ErrorInfo errorInfo)
    {
        Debug.LogWarning("[NetEvent] Warning :  " + errorInfo.ToString());
        GameInfo.Instance.OnWarning_Base(errorInfo);
        _DoOnWarning(errorInfo);
    }
    void OnException(HostID remoteID, System.Exception e)
    {
        Debug.LogError("[NetEvent] remeteID" + remoteID + "exception : " + e.ToString());
        GameInfo.Instance.OnException_Base(e);
        _DoOnException(remoteID, e);
    }
    void OnInformation(ErrorInfo errorInfo)
    {
        Debug.Log("[NetEvent] Information " + errorInfo.ToString());
        _DoOnInformation(errorInfo);
    }
    void OnNoRmiProcessed(RmiID rmiID)
    {
        Debug.LogError("[NetEvent] NoRmiProcessed : " + rmiID);
        _DoOnNoRmiProcessed(rmiID);
    }
    void OnChangeServerUdp(ErrorType reason)
    {
        Debug.Log("[NetEvent] ChangeServerUdp " + reason);
        _DoOnChangeServerUdp(reason);
    }
    void OnReceiveUserMessage(HostID sender, RmiContext rmiContext, ByteArray payload)
    {
        Debug.Log("[NetEvent] ReceiveUserMessage HostID : " + sender);
        _DoOnReceiveUserMessage(sender, rmiContext, payload);
    }

    protected virtual void _DoOnJoinServerComplete(ErrorInfo info, ByteArray replyFromServer)
    {
        return;
    }
    protected virtual void _DoOnLeaveServer(ErrorInfo errorInfo)
    {
        return;
    }
    protected void _DoOnServerOnline(RemoteOnlineEventArgs args)
    {
        return;
    }
    protected void _DoOnServerOffline(RemoteOfflineEventArgs args)
    {
        return;
    }
    protected virtual void _DoOnP2PMemberJoin(HostID memberHostID, HostID groupHostID, int memberCount, ByteArray customField)
    {
        return;
    }
    protected virtual void _DoOnP2PMemberLeave(HostID memberHostID, HostID groupHostID, int memberCount)
    {
        return;
    }
    protected virtual void _DoOnError(ErrorInfo errorInfo)
    {
        return;
    }
    protected virtual void _DoOnWarning(ErrorInfo errorInfo)
    {
        return;
    }
    protected virtual void _DoOnException(HostID remoteID, System.Exception e)
    {
        return;
    }
    protected virtual void _DoOnInformation(ErrorInfo errorInfo)
    {
        return;
    }
    protected virtual void _DoOnNoRmiProcessed(RmiID rmiID)
    {
        return;
    }
    protected virtual void _DoOnChangeServerUdp(ErrorType reason)
    {
        return;
    }
    protected virtual void _DoOnReceiveUserMessage(HostID sender, RmiContext rmiContext, ByteArray payload)
    {
        return;
    }
}