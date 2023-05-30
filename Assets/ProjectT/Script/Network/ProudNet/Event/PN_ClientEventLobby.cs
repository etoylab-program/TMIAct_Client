using Nettention.Proud;

internal class PN_ClientEventLobby : PN_ClientEventBase
{
    protected override void _DoOnJoinServerComplete(ErrorInfo info, ByteArray replyFromServer)
    {
        GameInfo.Instance.DoOnJoinServerComplete_Lobby(info, replyFromServer);
        return;
    }
    protected override void _DoOnLeaveServer(ErrorInfo errorInfo)
    {
        Debug.Log("PN_ClientEventLobby::_DoOnLeaveServer()");

        GameInfo.Instance.DoOnLeaveServer_Lobby(errorInfo);
        return;
    }
}