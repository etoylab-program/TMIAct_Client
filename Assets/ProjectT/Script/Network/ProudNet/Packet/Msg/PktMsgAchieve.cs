using System.Collections.Generic;
using Nettention.Proud;


// 공적 관련 문서 참고 -> http://10.10.10.90/Mediawiki/index.php/Project-A/일반/지휘관_정보_(공적)
// 공적 패킷 메시지
public class PktInfoAchieve : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt32 groupID_;          // 그룹 ID
        public System.UInt32 condiVal_;         // 진행 조건 현재 수치값
        public System.Byte nowStep_;            // 각각의 보상 획득 여부

        public override bool Read(Message _s) {
            if (false == _s.Read(out groupID_)) return false;
            if (false == _s.Read(out condiVal_)) return false;
            if (false == _s.Read(out nowStep_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(groupID_);
            _s.Write(condiVal_);
            _s.Write(nowStep_);
        }
    };
    public List<Piece> infos_;
};

public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 공적 패킷 메시지
    public static bool Read(Message _s, out PktInfoAchieve.Piece _v) {
        _v = new PktInfoAchieve.Piece();
        if (false == _v.Read(_s)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoAchieve.Piece _v) {
        _v.Write(_s);
    }
    // 공적 패킷 메시지
    public static bool Read(Message _s, out PktInfoAchieve _v) {
        _v = new PktInfoAchieve();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoAchieve _v) {
        WriteList(_s, _v.infos_);
    }
   
}