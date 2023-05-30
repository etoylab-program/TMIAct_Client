using System.Collections.Generic;
using Nettention.Proud;


// 메일 패킷 메시지
public class PktInfoMail : PktMsgType
{
    public class Piece : PktMsgType
    {
        public PktInfoTime endTime_;                // 메일 삭제 예정 시간
        public PktInfoProductOne productInfo_;      // 상품 정보
        public System.UInt64 mailUID_;              // 메일 고유 ID
        public System.UInt32 typeID_;               // 메일 타입 ID(우편 타입)
        public System.String typeValue_;            // 메일 타입 값

        public override bool Read(Message _s) {
            if (false == PN_MarshalerEx.Read(_s, out endTime_)) return false;
            if (false == PN_MarshalerEx.Read(_s, out productInfo_)) return false;
            if (false == _s.Read(out mailUID_)) return false;
            if (false == _s.Read(out typeID_)) return false;
            if (false == _s.Read(out typeValue_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            PN_MarshalerEx.Write(_s, endTime_);
            PN_MarshalerEx.Write(_s, productInfo_);
            _s.Write(mailUID_);
            _s.Write(typeID_);
            _s.Write(typeValue_);
        }
    };
    public List<Piece> infos_;
    public System.UInt16 maxCnt_;           // 메일 최대 개수(삭제 되지 않은 유저의 메일 최대 개수를 나타내며 )
};
// 메일 목록 요청 패킷 메시지
public class PktInfoMailListReq : PktMsgType
{
    public System.UInt32 tutoVal_;          // 튜토리얼 값
    public System.Byte startIdx_;           // 요청한 메일 목록의 시작 위치
    public System.Byte cnt_;                // 요청한 메일의 개수
    public System.Boolean isRefresh_;       // 강제 새로고침 여부
};
// 메일 기간 종료 패킷 메시지
public class PktInfoMailEndTimeList : PktMsgType
{
    public PktInfoUIDList delList_;         // 제거된 메일 고요 ID 목록
    public System.UInt32 tutoVal_;          // 튜토리얼 값
    public System.Byte getStartIdx_;        // 요청한 메일 목록의 시작 위치
    public System.Byte getCnt_;             // 요청한 메일의 개수
};

public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 메일 패킷 메시지
    public static bool Read(Message _s, out PktInfoMail _v) {
        _v = new PktInfoMail();
        if (false == ReadList(_s, out _v.infos_)) return false;
        if (false == _s.Read(out _v.maxCnt_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoMail _v) {
        WriteList(_s, _v.infos_);
        _s.Write(_v.maxCnt_);
    }

    // 메일 목록 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoMailListReq _v) {
        _v = new PktInfoMailListReq();
        if (false == _s.Read(out _v.tutoVal_)) return false;
        if (false == _s.Read(out _v.startIdx_)) return false;
        if (false == _s.Read(out _v.cnt_)) return false;
        if (false == _s.Read(out _v.isRefresh_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoMailListReq _v) {
        _s.Write(_v.tutoVal_);
        _s.Write(_v.startIdx_);
        _s.Write(_v.cnt_);
        _s.Write(_v.isRefresh_);
    }

    // 메일 기간 종료 패킷 메시지
    public static bool Read(Message _s, out PktInfoMailEndTimeList _v) {
        _v = new PktInfoMailEndTimeList();
        if (false == Read(_s, out _v.delList_)) return false;
        if (false == _s.Read(out _v.tutoVal_)) return false;
        if (false == _s.Read(out _v.getStartIdx_)) return false;
        if (false == _s.Read(out _v.getCnt_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoMailEndTimeList _v) {
        Write(_s, _v.delList_);
        _s.Write(_v.tutoVal_);
        _s.Write(_v.getStartIdx_);
        _s.Write(_v.getCnt_);
    }
}