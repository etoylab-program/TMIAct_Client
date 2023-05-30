using Nettention.Proud;


public class PacketBattleBase : PacketBase
{
    // 클라이언트서 서버로 요청 보내는 오브젝트
    protected BattleC2S.Proxy m_C2SProxy = new BattleC2S.Proxy();
    // 서버에서 응답을 받는 오브젝트
    protected BattleS2C.Stub m_S2CStub = new BattleS2C.Stub();

    public BattleS2C.Stub Recv() { return m_S2CStub; }

    public override void DoAttach(NetClient _netClient) { return; }

    public virtual void ReqMoveToLobby() { Debug.LogError("Not Make embody - ReqMoveToLobby"); }
}
public class PacketBattle : PacketBattleBase
{
    public override void DoAttach(NetClient _netClient)
    {
        _netClient.AttachProxy(m_C2SProxy);
        _netClient.AttachStub(m_S2CStub);
        return;
    }

    public override void ReqMoveToLobby() {
        m_C2SProxy.ReqMoveToLobby(HostID.HostID_Server, RmiContext.ReliableSend);
    }
}