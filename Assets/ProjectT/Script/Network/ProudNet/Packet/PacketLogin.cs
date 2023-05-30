using Nettention.Proud;


public class PacketLoginBase : PacketBase
{
    // 클라이언트서 서버로 요청 보내는 오브젝트
    protected LoginC2S.Proxy m_C2SProxy = new LoginC2S.Proxy();
    // 서버에서 응답을 받는 오브젝트
    protected LoginS2C.Stub m_S2CStub = new LoginS2C.Stub();

    //public LoginC2S.Proxy Send() { return m_C2SProxy; }
    public LoginS2C.Stub Recv() { return m_S2CStub; }

    public override void DoAttach(NetClient _netClient) { return; }

    public virtual void ReqClientSecurityInfo(PktInfoClientSecurityReq pkt) { Debug.LogError("Not Make embody - ReqClientSecurityInfo"); }
    public virtual void ReqClientSecurityVerify(PktInfoClientSecurityVerifyReq pkt) { Debug.LogError("Not Make embody - ReqClientSecurityVerify"); }
    public virtual void ReqAccountAuthLogin(PktInfoAuthLogin pkt) { Debug.LogError("Not Make embody - ReqAccountAuthLogin"); }
    public virtual void ReqMoveToLobby() { Debug.LogError("Not Make embody - ReqMoveToLobby"); }
}
public class PacketLogin : PacketLoginBase
{
    
    public override void DoAttach(NetClient _netClient)
    {
        _netClient.AttachProxy(m_C2SProxy);
        _netClient.AttachStub(m_S2CStub);


        __DoRegistRecv();
        return;
    }

    void __DoRegistRecv()
    {
        m_S2CStub.AckErrLoginUserBlock = __AckErrLoginUserBlock;
    }
    public override void ReqClientSecurityInfo(PktInfoClientSecurityReq pkt) {
        m_C2SProxy.ReqClientSecurityInfo(HostID.HostID_Server, RmiContext.ReliableSend, pkt);
    }
    public override void ReqClientSecurityVerify(PktInfoClientSecurityVerifyReq pkt) {
        m_C2SProxy.ReqClientSecurityVerify(HostID.HostID_Server, RmiContext.SecureReliableSend, pkt);
    }
    public override void ReqAccountAuthLogin(PktInfoAuthLogin pkt) {
        m_C2SProxy.ReqAccountAuthLogin(HostID.HostID_Server, RmiContext.SecureReliableSend, pkt);
    }
    public override void ReqMoveToLobby() {
        m_C2SProxy.ReqMoveToLobby(HostID.HostID_Server, RmiContext.ReliableSend);
    }

    bool __AckErrLoginUserBlock(Nettention.Proud.HostID remote, Nettention.Proud.RmiContext rmiContext, PktInfoTime _blockTime) {
        Platforms.IBase.Inst.DoLogAckErrLoginUserBlock(_blockTime);
        return true;
    }
}