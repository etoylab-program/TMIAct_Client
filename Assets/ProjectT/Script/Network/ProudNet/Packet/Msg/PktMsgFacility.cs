using System.Collections.Generic;
using Nettention.Proud;


// 시설 패킷 메시지
public class PktInfoFacility : PktMsgType
{
    public class Piece : PktMsgType
    {
        public PktInfoTime operationEndTime_;       // 시설 작동 완료 시간
        public System.UInt64 operationValue_;       // 시설 작동 관련 값(캐릭터 UID, 아이템 TID 등) eFacilityEffTP 타입에 따라서 사용 용도가 다름
        public System.UInt32 tableID_;
        public System.UInt16 operationCnt_ = 1;     // 시설 작동 대상의 개수(현재는 아이템 조합 개수로만 사용됨) - 최소 1이상의 값이 필요함.
        public System.Byte lv_;

        public override bool Read(Message _s) {
            if (false == PN_MarshalerEx.Read(_s, out operationEndTime_)) return false;
            if (false == _s.Read(out operationValue_)) return false;
            if (false == _s.Read(out tableID_)) return false;
            if (false == _s.Read(out operationCnt_)) return false;
            if (false == _s.Read(out lv_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            PN_MarshalerEx.Write(_s, operationEndTime_);
            _s.Write(operationValue_);
            _s.Write(tableID_);
            _s.Write(operationCnt_);
            _s.Write(lv_);
        }
        public eFACILITYSTATS GetState()
        {
            if (0 == operationEndTime_.time_)
                return eFACILITYSTATS.WAIT;
            if (operationEndTime_.GetTime() < GameSupport.GetCurrentServerTime())
                return eFACILITYSTATS.COMPLETE;
            return eFACILITYSTATS.USE;
        }
    };
    public List<Piece> infos_;
};
// 시설 업그레이드 패킷 메시지
public class PktInfoFacilityUpgrade : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt32 facilityTID_;      // 시설 테이블 ID
        public System.Byte resultLv_;           // 레벨업 적용 수치
        public bool newFlag_;                   // 새롭게 추가된 시설인지 여부

        public override bool Read(Message _s) {
            if (false == _s.Read(out facilityTID_)) return false;
            if (false == _s.Read(out resultLv_)) return false;
            if (false == _s.Read(out newFlag_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(facilityTID_);
            _s.Write(resultLv_);
            _s.Write(newFlag_);
        }
    };
    public List<Piece> infos_;
    public PktInfoConsumeItem maters_;      // 재료 아이템 정보
    public System.UInt64 userGold_;         // 레벨업 비용이 차감된 현재 유저의 금화
};
// 시설 작동 요청 패킷 메시지
public class PktInfoFacilityOperationReq : PktMsgType
{
    public class Mater : PktMsgType
    {
        public System.UInt64 uid_;              // 재료로 사용되는 항목의 고유 ID
        public eContentsPosKind kind_;          // 재료로 사용되는 항목의 컨텐츠 위치

        public override bool Read(Message _s) {
            if (false == _s.Read(out uid_)) return false;
            if (false == PN_MarshalerEx.Read(_s, out kind_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(uid_);
            PN_MarshalerEx.Write(_s, kind_);
        }
    };
    public List<Mater> maters_;             // 제품 재료 목록(교환 등에서 사용)
    public System.UInt64 operationValue_;   // 시설 작동 관련 값(캐릭터 UID, 아이템 TID 등) eFacilityEffTP 타입에 따라서 사용 용도가 다름 - (클라이언트에서 채워줘야 할 값)
    public System.UInt32 facilityTID_;      // 시설 테이블 ID - (클라이언트에서 채워줘야 할 값)
    public System.UInt32 tradeAddonTID_;    // 교환 AddOnTID
    public PktInfoUIDList operationRoomChar_;     // 작전회의실 참여 캐릭터 목록
    public System.UInt16 operationCnt_ = 1; // 조합할 아이템의 수 (아이템 조합 시설 사용시 필요) - (클라이언트에서 채워줘야 할 값)
};
// 시설 작동 완료 요청 패킷 메시지
public class PktInfoFacilityOperConfirmReq : PktMsgType
{
    public enum TYPE : System.Byte {
        _NONE_          = 0,
        CASH,
        ITEM,

        _MAX_
    };
    public System.UInt32 facilityTID_;          // 시설 테이블 ID - (클라이언트에서 채워줘야 할 값)
    public System.UInt32 consumeVal_;           // 소모 타입에 따른 값 - (클라이언트에서 채워줘야 할 값)
    public System.Byte consumeTP_;              // 소모 타입(TYPE) - (클라이언트에서 채워줘야 할 값)
    public bool clearOperValFlag_;              // operationValue값을 초기화 시키는지 여부 - (클라이언트에서 채워줘야 할 값)
};


public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 시설 패킷 메시지
    public static bool Read(Message _s, out PktInfoFacility.Piece _v) {
        _v = new PktInfoFacility.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoFacility.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoFacility _v) {
        _v = new PktInfoFacility();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoFacility _v) {
        WriteList(_s, _v.infos_);
    }

    // 시설 업그레이드 패킷 메시지
    public static bool Read(Message _s, out PktInfoFacilityUpgrade.Piece _v) {
        _v = new PktInfoFacilityUpgrade.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoFacilityUpgrade.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoFacilityUpgrade _v) {
        _v = new PktInfoFacilityUpgrade();
        if (false == ReadList(_s, out _v.infos_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.maters_)) return false;
        if (false == _s.Read(out _v.userGold_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoFacilityUpgrade _v) {
        WriteList(_s, _v.infos_);
        PN_MarshalerEx.Write(_s, _v.maters_);
        _s.Write(_v.userGold_);
    }

    // 시설 작동 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoFacilityOperationReq _v) {
        _v = new PktInfoFacilityOperationReq();
        if (false == ReadList(_s, out _v.maters_)) return false;
        if (false == _s.Read(out _v.operationValue_)) return false;
        if (false == _s.Read(out _v.facilityTID_)) return false;
        if (false == _s.Read(out _v.tradeAddonTID_)) return false;
        if (false == Read(_s, out _v.operationRoomChar_)) return false;
        if (false == _s.Read(out _v.operationCnt_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoFacilityOperationReq _v) {
        WriteList(_s, _v.maters_);
        _s.Write(_v.operationValue_);
        _s.Write(_v.facilityTID_);
        _s.Write(_v.tradeAddonTID_);
        Write(_s, _v.operationRoomChar_);
        _s.Write(_v.operationCnt_);
    }
    // 시설 작동 완료 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoFacilityOperConfirmReq _v) {
        _v = new PktInfoFacilityOperConfirmReq();
        if (false == _s.Read(out _v.facilityTID_)) return false;
        if (false == _s.Read(out _v.consumeVal_)) return false;
        if (false == _s.Read(out _v.consumeTP_)) return false;
        if (false == _s.Read(out _v.clearOperValFlag_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoFacilityOperConfirmReq _v) {
        _s.Write(_v.facilityTID_);
        _s.Write(_v.consumeVal_);
        _s.Write(_v.consumeTP_);
        _s.Write(_v.clearOperValFlag_);
    }

   
}