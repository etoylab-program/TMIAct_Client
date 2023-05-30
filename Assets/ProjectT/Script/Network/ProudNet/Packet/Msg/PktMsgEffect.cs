using System.Collections.Generic;
using Nettention.Proud;


// 효과 정보 패킷 메시지
public class PktInfoEffect : PktMsgType
{
    // 버프 관련 문서 참고 -> http://10.10.10.90/Mediawiki/index.php/Project-A/일반/계정_버프_시스템
    public class Buff : PktMsgType
    {
        public PktInfoTime endTM_;          // 버프 종료 시간
        public System.UInt32 tableID_;      // 버프 TID

        public override bool Read(Message _s) {
            if (false == PN_MarshalerEx.Read(_s, out endTM_)) return false;
            if (false == _s.Read(out tableID_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            PN_MarshalerEx.Write(_s, endTM_);
            _s.Write(tableID_);
        }
    };
    public class BuffP : PktMsgType
    {
        public System.UInt32 tableID_;      // 버프 TID

        public override bool Read(Message _s) {
            if (false == _s.Read(out tableID_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(tableID_);
        }
    };
    public List<Buff> bufs_;
    public List<BuffP> bufSs_;
};

public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 효과 정보 패킷 메시지
    public static bool Read(Message _s, out PktInfoEffect.Buff _v) {
        _v = new PktInfoEffect.Buff();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoEffect.Buff _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoEffect.BuffP _v) {
        _v = new PktInfoEffect.BuffP();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoEffect.BuffP _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoEffect _v) {
        _v = new PktInfoEffect();
        if (false == ReadList(_s, out _v.bufs_)) return false;
        return ReadList(_s, out _v.bufSs_);
    }
    public static void Write(Message _s, PktInfoEffect _v) {
        WriteList(_s, _v.bufs_);
        WriteList(_s, _v.bufSs_);
    }
}