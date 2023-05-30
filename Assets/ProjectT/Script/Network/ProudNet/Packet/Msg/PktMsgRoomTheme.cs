using System.Collections.Generic;
using Nettention.Proud;


// 룸 관련 구매 정보 패킷 메시지
public class PktInfoRoomPurchase : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt32 tableID_;          // 룸 관련 구매 타입과 관련된 테이블 ID
        public eREWARDTYPE type_;               // 룸 관련 구매 타입
        public override bool Read(Message _s) {
            if (false == _s.Read(out tableID_)) return false;
            if (false == PN_MarshalerEx.Read(_s, out type_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(tableID_);
            PN_MarshalerEx.Write(_s, type_);
        }
        public System.String GetStr() {
            return string.Format("TP:{0} TID:{1}", type_, tableID_);
        }
    };
    public List<Piece> infos_;

    public System.String GetStr() {
        System.String log = string.Format("RoomPurchase[");
        foreach (var info in infos_)
            log += info.GetStr() + "\n";
        log += string.Format("]");
        return log;
    }
};

// 룸 테마 슬롯 관련 패킷 메시지
public class PktInfoRoomThemeSlot : PktMsgType
{
    public class Piece : PktMsgType
    {
        public PktInfoStream data_;             // 룸 테마 관련 정보
        public System.UInt64 funcStateFlag_;    // 룸 기능 상태
        public System.UInt32 tableID_;          // 룸 테마 테이블 ID
        public System.UInt32 slotNum_;          // 룸 테마 슬롯 번호
        public override bool Read(Message _s) {
            if (false == PN_MarshalerEx.Read(_s, out data_)) return false;
            if (false == _s.Read(out funcStateFlag_)) return false;
            if (false == _s.Read(out tableID_)) return false;
            if (false == _s.Read(out slotNum_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            PN_MarshalerEx.Write(_s, data_);
            _s.Write(funcStateFlag_);
            _s.Write(tableID_);
            _s.Write(slotNum_);
        }
        public System.String GetStr() {
            return string.Format("Slot:{0} TID:{1} State:{2}", slotNum_, tableID_, funcStateFlag_);
        }

        public bool IsOnFunc(ulong _flagIdx) {
            return _IsOnBitIdx(funcStateFlag_, _flagIdx);
        }
        public void DoOnFunc(ulong _flagIdx) {
            _DoOnBitIdx(ref funcStateFlag_, _flagIdx);
        }
        public void DoOffFunc(ulong _flagIdx) {
            _DoOffBitIdx(ref funcStateFlag_, _flagIdx);
        }
    };
    public List<Piece> infos_;

    public System.String GetStr() {
        System.String log = string.Format("ThemeSlot[");
        foreach (var info in infos_)
            log += info.GetStr() + "\n";
        log += string.Format("]");
        return log;
    }
};

// 룸 피규어 슬롯 관련 패킷 메시지
public class PktInfoRoomFigureSlot : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt32 tableID_;          // 룸 피규어 테이블 ID
        public System.UInt32 actionID1_;        // 룸 액션 ID 1 (표정)
        public System.UInt32 actionID2_;        // 룸 액션 ID 2 (동작)
        public System.UInt32 skinStateFlag_;    // 적용된 스킨 상태 플레그
        public System.Byte costumeClr_;         // 코스츔 색
        public System.Byte figureSlotNum_;      // 룸 피규어 슬롯 번호
        public System.Byte themeSlotNum_;       // 룸 테마 슬롯 번호
        public PktInfoStream detail_;           // 룸 피규어 세부 정보 (필요 시 따로 요청)
        public override bool Read(Message _s) {
            if (false == _s.Read(out tableID_)) return false;
            if (false == _s.Read(out actionID1_)) return false;
            if (false == _s.Read(out actionID2_)) return false;
            if (false == _s.Read(out skinStateFlag_)) return false;
            if (false == _s.Read(out costumeClr_)) return false;
            if (false == _s.Read(out figureSlotNum_)) return false;
            if (false == _s.Read(out themeSlotNum_)) return false;
            if (false == PN_MarshalerEx.Read(_s, out detail_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(tableID_);
            _s.Write(actionID1_);
            _s.Write(actionID2_);
            _s.Write(skinStateFlag_);
            _s.Write(costumeClr_);
            _s.Write(figureSlotNum_);
            _s.Write(themeSlotNum_);
            PN_MarshalerEx.Write(_s, detail_);
        }
        public System.String GetStr() {
            return string.Format("Slot:{0} ThemeSlot:{1} TID:{2} ActionID1:{3} ActionID2:{4} Detail:{5}", figureSlotNum_, themeSlotNum_, tableID_, actionID1_, actionID2_, detail_.GetStr());
        }
        public bool IsEmptyDefail() { return detail_.IsEmpty(); }
};
    public List<Piece> infos_;

    public bool IsExistEmptyDefail() {
        foreach (var info in infos_) {
            if (true == info.IsEmptyDefail())
                return true;
        }
        return false;
    }

    public System.String GetStr() {
        System.String log = string.Format("FigureSlot[");
        foreach (var info in infos_)
            log += info.GetStr() + "\n";
        log += string.Format("]");
        return log;
    }
};

// 룸 테마 묶음 패킷 메시지
public class PktInfoRoomPackage : PktMsgType
{
    public PktInfoRoomPurchase purchase_;       // 룸 구매 정보
    public PktInfoRoomThemeSlot themeSlot_;     // 룸 테마 슬롯 정보
    public PktInfoRoomFigureSlot figureSlot_;   // 룸 피규어 슬롯 정보
};

// 룸 관련 구매 패킷 메시지
public class PktInfoRoomStorePurchase : PktMsgType
{
    public class Piece : PktMsgType
    {
        public PktInfoRoomPurchase.Piece purchase_ = new PktInfoRoomPurchase.Piece(); // 룸 구매정보
        public System.UInt32 storeTID_;             // 룸 관련 구매 타입
        public override bool Read(Message _s) {
            if (false == purchase_.Read(_s)) return false;
            if (false == _s.Read(out storeTID_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            purchase_.Write(_s);
            _s.Write(storeTID_);
        }
        public System.String GetStr() {
            return string.Format("StoreTID:{0} Purchase[{1}]", storeTID_, purchase_.GetStr());
        }
    };
    public List<Piece> infos_;                  // 구매된 상품 목록
    public PktInfoRoomThemeSlot addThemeSlot_;  // 추가된 룸 테마 슬롯 정보
    public PktInfoConsumeItemAndGoods retConsume_;  // 비용 소모가 적용된 아이템 및 재화
};

// 룸 테마 디테일 정보 패킷 메시지
public class PktInfoRoomThemeSlotDetail : PktMsgType
{
    public PktInfoRoomThemeSlot.Piece themeSlot_ = new PktInfoRoomThemeSlot.Piece();    // 룸 구매 정보
    public PktInfoRoomFigureSlot figureSlot_;       // 룸 테마 슬롯 정보

    public System.String GetStr() {
        System.String log = string.Format("RoomThemeSlotDetail[");
        log += themeSlot_.GetStr() + "\n";
        log += figureSlot_.GetStr();
        log += string.Format("]");
        return log;
    }

    public void DoClear()
    {
        if (null != figureSlot_)
            figureSlot_.infos_.Clear();
    }
};

public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 룸 관련 구매 정보 패킷 메시지
    public static bool Read(Message _s, out PktInfoRoomPurchase _v) {
        _v = new PktInfoRoomPurchase();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoRoomPurchase _v) {
        WriteList(_s, _v.infos_);
    }
    // 룸 테마 슬롯 관련 패킷 메시지
    public static bool Read(Message _s, out PktInfoRoomThemeSlot _v) {
        _v = new PktInfoRoomThemeSlot();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoRoomThemeSlot _v) {
        WriteList(_s, _v.infos_);
    }
    // 룸 피규어 슬롯 관련 패킷 메시지
    public static bool Read(Message _s, out PktInfoRoomFigureSlot _v) {
        _v = new PktInfoRoomFigureSlot();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoRoomFigureSlot _v) {
        WriteList(_s, _v.infos_);
    }

    // 룸 테마 묶음 패킷 메시지
    public static bool Read(Message _s, out PktInfoRoomPackage _v) {
        _v = new PktInfoRoomPackage();
        if (false == Read(_s, out _v.purchase_)) return false;
        if (false == Read(_s, out _v.themeSlot_)) return false;
        if (false == Read(_s, out _v.figureSlot_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoRoomPackage _v) {
        Write(_s, _v.purchase_);
        Write(_s, _v.themeSlot_);
        Write(_s, _v.figureSlot_);
    }

    // 룸 관련 구매 패킷 메시지
    public static bool Read(Message _s, out PktInfoRoomStorePurchase _v) {
        _v = new PktInfoRoomStorePurchase();
        if (false == ReadList(_s, out _v.infos_)) return false;
        if (false == Read(_s, out _v.addThemeSlot_)) return false;
        if (false == Read(_s, out _v.retConsume_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoRoomStorePurchase _v) {
        WriteList(_s, _v.infos_);
        Write(_s, _v.addThemeSlot_);
        Write(_s, _v.retConsume_);
    }

    // 룸 테마 디테일 정보 패킷 메시지
    public static bool Read(Message _s, out PktInfoRoomThemeSlotDetail _v) {
        _v = new PktInfoRoomThemeSlotDetail();
        if (false == _v.themeSlot_.Read(_s)) return false;
        if (false == Read(_s, out _v.figureSlot_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoRoomThemeSlotDetail _v) {
        _v.themeSlot_.Write(_s);
        Write(_s, _v.figureSlot_);
    }
}