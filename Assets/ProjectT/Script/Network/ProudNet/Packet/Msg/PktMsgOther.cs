using Nettention.Proud;


public class PktInfoSimpleSvr : PktMsgType
{
    public System.UInt64 svrUid_;
    public System.UInt16 startPort_;
    public System.UInt16 endPort_;
    public System.Byte svrTP_;
    public System.String addr_;
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoSimpleSvr _v)
    {
        _v = new PktInfoSimpleSvr();
        if (false == _s.Read(out _v.svrUid_)) return false;
        if (false == _s.Read(out _v.startPort_)) return false;
        if (false == _s.Read(out _v.endPort_)) return false;
        if (false == _s.Read(out _v.svrTP_)) return false;
        if (false == _s.Read(out _v.addr_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoSimpleSvr value)
    {
        _s.Write(value.svrUid_);
        _s.Write(value.startPort_);
        _s.Write(value.endPort_);
        _s.Write(value.svrTP_);
        _s.Write(value.addr_);
    }
}