using Nettention.Proud;

internal class PN_ClientEventLogin : PN_ClientEventBase
{
    protected override void _DoOnJoinServerComplete(ErrorInfo info, ByteArray replyFromServer)
    {
        GameInfo.Instance.DoOnJoinServerComplete_Login(info, replyFromServer);
        return;
    }
    protected override void _DoOnLeaveServer(ErrorInfo errorInfo)
    {
        Debug.Log("PN_ClientEventLogin::_DoOnLeaveServer()");
        GameInfo.Instance.DoOnLeaveServer_Login(errorInfo);
        return;
    }
    protected override void _DoOnP2PMemberJoin(HostID memberHostID, HostID groupHostID, int memberCount, ByteArray customField)
    {
        return;
    }
    protected override void _DoOnP2PMemberLeave(HostID memberHostID, HostID groupHostID, int memberCount)
    {
        return;
    }
    protected override void _DoOnError(ErrorInfo errorInfo)
    {
        return;
    }
    protected override void _DoOnWarning(ErrorInfo errorInfo)
    {
        return;
    }
    protected override void _DoOnException(HostID remoteID, System.Exception e)
    {
        return;
    }
    protected override void _DoOnInformation(ErrorInfo errorInfo)
    {
        return;
    }
    protected override void _DoOnNoRmiProcessed(RmiID rmiID)
    {
        return;
    }
    protected override void _DoOnChangeServerUdp(ErrorType reason)
    {
        return;
    }
    protected override void _DoOnReceiveUserMessage(HostID sender, RmiContext rmiContext, ByteArray payload)
    {
        return;
    }
}