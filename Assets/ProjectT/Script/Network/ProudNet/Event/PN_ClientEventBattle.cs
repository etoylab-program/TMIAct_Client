using Nettention.Proud;

internal class PN_ClientEventBattle : PN_ClientEventBase
{
    protected override void _DoOnJoinServerComplete(ErrorInfo info, ByteArray replyFromServer)
    {
        GameInfo.Instance.DoOnJoinServerComplete_Battle(info, replyFromServer);
        return;
    }
    protected override void _DoOnLeaveServer(ErrorInfo errorInfo)
    {
        Debug.Log("PN_ClientEventBattle::_DoOnLeaveServer()");
        GameInfo.Instance.DoOnLeaveServer_Battle(errorInfo);

        return;
    }
}