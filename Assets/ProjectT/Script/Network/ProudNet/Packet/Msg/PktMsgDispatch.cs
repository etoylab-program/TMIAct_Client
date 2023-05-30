using System.Collections.Generic;
using Nettention.Proud;


// 파견 패킷 메시지
public class PktInfoDispatch : PktMsgType
{
    public class Piece : PktMsgType
    {
        public PktInfoTime operEndTime_;        // 파견 완료 시간
        public System.UInt32 tableID_;          // 파견 슬롯 ID
        public System.UInt32 missionID_;        // 파견 임무 ID

        public override bool Read(Message _s) {
            if (false == PN_MarshalerEx.Read(_s, out operEndTime_)) return false;
            if (false == _s.Read(out tableID_)) return false;
            if (false == _s.Read(out missionID_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            PN_MarshalerEx.Write(_s, operEndTime_);
            _s.Write(tableID_);
            _s.Write(missionID_);
        }
    };
    public List<Piece> infos_;
};
// 파견 슬롯 열기 패킷 메시지
public class PktInfoDispatchOpen : PktMsgType
{
    public PktInfoDispatch opens_;          // 활성화된 파견 임무 목록
    public PktInfoGoods goods_;             // 비용 소모가 적용된 현재 금화
};
// 파견 슬롯 열기 패킷 메시지
public class PktInfoDispatchChange : PktMsgType
{
    public PktInfoDispatch changes_;        // 교체된 파견 임무 목록
    public PktInfoGoods goods_;             // 비용 소모가 적용된 현재 금화
};
// 파견 임무 진행 요청 패킷 메시지
public class PktInfoDispatchOperReq : PktMsgType
{
    public PktInfoUIDList cardUIDs_;        // 파견 보낼 카드(서포터) ID - (클라이언트에서 채워줘야 할 값)
    public System.UInt32 dispatchTID_;      // 파견 슬롯 테이블 ID - (클라이언트에서 채워줘야 할 값)
};
// 파견 임무 진행 응답 패킷 메시지
public class PktInfoDispatchOperAck : PktMsgType
{
    public PktInfoContentsSlotPos cards_;   // 파견 임무를 진행할 카드(서포터)의 위지 적용 목록
    public PktInfoDispatch opers_;          // 임무를 시작한 파견 목록
};
// 파견 임무 완료 요청 패킷 메시지
public class PktInfoDispatchOperConfirmReq : PktMsgType
{
    public System.UInt32 dispatchTID_;      // 파견 슬롯 테이블 ID - (클라이언트에서 채워줘야 할 값)
	public bool isImmediateComplete_;       // 파견 즉시완료 여부 - (클라이언트에서 채워줘야 할 값)
};

public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 시설 패킷 메시지
    public static bool Read(Message _s, out PktInfoDispatch.Piece _v) {
        _v = new PktInfoDispatch.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoDispatch.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoDispatch _v) {
        _v = new PktInfoDispatch();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoDispatch _v) {
        WriteList(_s, _v.infos_);
    }
    // 파견 슬롯 열기 패킷 메시지
    public static bool Read(Message _s, out PktInfoDispatchOpen _v) {
        _v = new PktInfoDispatchOpen();
        if (false == PN_MarshalerEx.Read(_s, out _v.opens_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.goods_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoDispatchOpen _v) {
        PN_MarshalerEx.Write(_s, _v.opens_);
        PN_MarshalerEx.Write(_s, _v.goods_);
    }
    // 파견 슬롯 열기 패킷 메시지
    public static bool Read(Message _s, out PktInfoDispatchChange _v) {
        _v = new PktInfoDispatchChange();
        if (false == PN_MarshalerEx.Read(_s, out _v.changes_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.goods_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoDispatchChange _v) {
        PN_MarshalerEx.Write(_s, _v.changes_);
        PN_MarshalerEx.Write(_s, _v.goods_);
    }
    // 파견 임무 진행 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoDispatchOperReq _v) {
        _v = new PktInfoDispatchOperReq();
        if (false == PN_MarshalerEx.Read(_s, out _v.cardUIDs_)) return false;
        if (false == _s.Read(out _v.dispatchTID_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoDispatchOperReq _v) {
        PN_MarshalerEx.Write(_s, _v.cardUIDs_);
        _s.Write(_v.dispatchTID_);
    }
    // 파견 임무 진행 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoDispatchOperAck _v) {
        _v = new PktInfoDispatchOperAck();
        if (false == PN_MarshalerEx.Read(_s, out _v.cards_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.opers_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoDispatchOperAck _v) {
        PN_MarshalerEx.Write(_s, _v.cards_);
        PN_MarshalerEx.Write(_s, _v.opers_);
    }
    // 파견 임무 완료 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoDispatchOperConfirmReq _v) {
        _v = new PktInfoDispatchOperConfirmReq();
        if (false == _s.Read(out _v.dispatchTID_)) return false;
		if (false == _s.Read(out _v.isImmediateComplete_)) return false;
		return true;
    }
    public static void Write(Message _s, PktInfoDispatchOperConfirmReq _v) {
        _s.Write(_v.dispatchTID_);
		_s.Write(_v.isImmediateComplete_);
	}
}