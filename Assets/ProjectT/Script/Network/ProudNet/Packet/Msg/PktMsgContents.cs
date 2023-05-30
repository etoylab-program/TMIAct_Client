using System.Collections.Generic;
using Nettention.Proud;


//////////////////////////////////////////////////////////////////////////
///
// 코스츔 부위 잠금 패킷 메시지
public class PktInfoCostumeDyeingLock : PktMsgType
{
    public System.UInt32 costumeID_;   // 잠금 대상 코스츔 아이디
    public System.Byte   lockFlag_;    // 잠금할 비트플래그 ( 클라이언트에서 채워줄 값 ) 
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoCostumeDyeingLock _v)
    {
        _v = new PktInfoCostumeDyeingLock();
        if(false == _s.Read(out _v.costumeID_)) return false;
        if (false == _s.Read(out _v.lockFlag_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCostumeDyeingLock _v)
    {
        _s.Write(_v.costumeID_);
        _s.Write(_v.lockFlag_);
    }
}

// 코스튬 염색 색상 결정 요청 패킷 메시지 
public class PktInfoSetCostumeDyeingReq : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.Byte partIndex_;   // 염색 부위 인덱스 클라이언트에서 채워줄 값
        public System.Byte colorIndex_;  // 선택한 컬러 인덱스 클라이언트에서 채워줄 값  

        public override bool Read(Message _s)
        {
            if (false == _s.Read(out partIndex_)) return false;
            if (false == _s.Read(out colorIndex_)) return false;
            return true;
        }
        public override void Write(Message _s)
        {
            _s.Write(partIndex_);
            _s.Write(colorIndex_);
        }
    };
    public List<Piece> infos_;

};

public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    
    public static bool Read(Message _s, out PktInfoSetCostumeDyeingReq _v)
    {
        _v = new PktInfoSetCostumeDyeingReq();
        if (false == ReadList(_s, out _v.infos_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoSetCostumeDyeingReq _v)
    {
        WriteList(_s, _v.infos_);
    }
}

// 컬러 패킷 메시지
public class PktInfoColor : PktMsgType
{
    public System.Byte red_;
    public System.Byte green_;
    public System.Byte blue_;
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoColor _v)
    {
        _v = new PktInfoColor();
        if (false == _s.Read(out _v.red_)) return false;
        if (false == _s.Read(out _v.green_)) return false;
        if (false == _s.Read(out _v.blue_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoColor _v)
    {
        _s.Write(_v.red_);
        _s.Write(_v.green_);
        _s.Write(_v.blue_);
    }
};


// 코스튬 컬러 패킷 메시지
public class PktInfoUserCostumeColor : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.Byte partIndex_;       // 부위
        public System.Byte colorIndex_;      // 컬러 인덱스 
        public PktInfoColor color_;          // 색상
        
        public override bool Read(Message _s)
        {
            if (false == _s.Read(out partIndex_)) return false;
            if (false == _s.Read(out colorIndex_)) return false;
            if (false == PN_MarshalerEx.Read(_s, out color_)) return false;
            return true;
        }

        public override void Write(Message _s)
        {
            _s.Write(partIndex_);
            _s.Write(colorIndex_);
            PN_MarshalerEx.Write(_s, color_);
        }
    };
    public List<Piece> infos_;

};

public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{    public static bool Read(Message _s, out PktInfoUserCostumeColor.Piece _v)
    {
        _v = new PktInfoUserCostumeColor.Piece();
        if (false == _s.Read(out _v.partIndex_)) return false;
        if (false == _s.Read(out _v.colorIndex_)) return false;
        if (false == Read(_s, out _v.color_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoUserCostumeColor.Piece _v)
    {
        _s.Write(_v.partIndex_);
        _s.Write(_v.colorIndex_);
        Write(_s, _v.color_);
    }
    public static bool Read(Message _s, out PktInfoUserCostumeColor _v)
    {
        _v = new PktInfoUserCostumeColor();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoUserCostumeColor _v)
    {
        WriteList(_s, _v.infos_);
    }
}

// 코스츔 패킷 메시지
public class PktInfoCostume : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt32 tableID_;   // 테이블아이디
        public PktInfoColor part1_;      // 염색 부위 1
        public PktInfoColor part2_;      // 염색 부위 2
        public PktInfoColor part3_;      // 염색 부위 3 
        public System.Byte lockFlag_;   // 염색 부위 잠금 비트플래그 1: 잠금, 0 해제 
        public bool isFirstDyeing_ = true; // 처음 염색 플래그 

        public override bool Read(Message _s)
        {
            if (false == _s.Read(out tableID_)) return false;
            if (false == PN_MarshalerEx.Read(_s, out part1_)) return false;
            if (false == PN_MarshalerEx.Read(_s, out part2_)) return false;
            if (false == PN_MarshalerEx.Read(_s, out part3_)) return false;
            if (false == _s.Read(out lockFlag_)) return false;
            if (false == _s.Read(out isFirstDyeing_)) return false;
            return true;
        }

        public override void Write(Message _s)
        {
            _s.Write(tableID_);
            PN_MarshalerEx.Write(_s, part1_);
            PN_MarshalerEx.Write(_s, part2_);
            PN_MarshalerEx.Write(_s, part3_);
            _s.Write(lockFlag_);
            _s.Write(isFirstDyeing_);

        }
    };
    public List<Piece> infos_;

};

public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoCostume.Piece _v)
    {
        _v = new PktInfoCostume.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoCostume.Piece _v)
    {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoCostume _v)
    {
        _v = new PktInfoCostume();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoCostume _v)
    {
        WriteList(_s, _v.infos_);
    }
}

// 코스츔 염색 랜덤색상 응답 패킷 메시지
public class PktInfoRandomCostumeDyeing : PktMsgType
{
    public PktInfoCostume.Piece costume_;               // 대상 코스튬과 현재 색상
    public PktInfoUserCostumeColor userCostumeColor_;   // 랜덤하게 나온 색상
    public PktInfoConsumeItem useItem_;                 // 사용한 아이템 정보 
};

public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
   public static bool Read(Message _s, out PktInfoRandomCostumeDyeing _v)
    {
        _v = new PktInfoRandomCostumeDyeing();
        if (false == Read(_s, out _v.costume_)) return false;
        if (false == Read(_s, out _v.userCostumeColor_)) return false;
        if (false == Read(_s, out _v.useItem_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoRandomCostumeDyeing _v)
    {
        Write(_s, _v.costume_);
        Write(_s, _v.userCostumeColor_);
        Write(_s, _v.useItem_);
    }
}



///
//////////////////////////////////////////////////////////////////////////


//////////////////////////////////////////////////////////////////////////
///
// 무료 상점 정보 패킷 메시지
public class PktInfoStoreSale : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt64 resetTM_;      // 리셋 시간
        public System.UInt64 typeVal_;      // 타입 값
        public System.UInt32 tableID_;      // 상점 TID
        public override bool Read(Message _s) {
            if (false == _s.Read(out resetTM_)) return false;
            if (false == _s.Read(out typeVal_)) return false;
            if (false == _s.Read(out tableID_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(resetTM_);
            _s.Write(typeVal_);
            _s.Write(tableID_);
        }
    };
    public List<Piece> infos_;
};

// 돌발 패키지 일자 보상 요청 메시지
public class PktInfoUnexpectedPackageDailyRewardReq : PktMsgType
{
    public System.UInt32 unexpectedPackageTID_;     // 돌발 패키지 테이블아이디
    public System.Byte dayNumber_;                  // 보상 받을 일자
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoUnexpectedPackageDailyRewardReq _v)
    {
        _v = new PktInfoUnexpectedPackageDailyRewardReq();
        if (false == _s.Read(out _v.unexpectedPackageTID_)) return false;
        if (false == _s.Read(out _v.dayNumber_)) return false;
        return true;
    }

    public static void Write(Message _s, PktInfoUnexpectedPackageDailyRewardReq _v)
    {
        _s.Write(_v.unexpectedPackageTID_);
        _s.Write(_v.dayNumber_);
    }
}


// 돌발 패키지 패킷 메시지
public class PktInfoUnexpectedPackage : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt32 tableID_;          // TID
        public PktInfoTime endTime_;            // 이벤트 종료 시간
        public System.Byte repeatCount_;        // 이벤트 반복 횟수
        public System.Byte rewardBitFlag_;      // 일자별 보상 획득 여부 비트플래그 (0: 미수령, 1:수령 )
        public System.Boolean isPurchase_;      // 구매 여부

        public override bool Read(Message _s)
        {
            if (false == _s.Read(out tableID_)) return false;
            if (false == PN_MarshalerEx.Read(_s, out endTime_)) return false;
            if (false == _s.Read(out repeatCount_)) return false;
            if (false == _s.Read(out rewardBitFlag_)) return false;
            if (false == _s.Read(out isPurchase_)) return false;
            return true;
        }

        public override void Write(Message _s)
        {
            _s.Write(tableID_);
            PN_MarshalerEx.Write(_s, endTime_);
            _s.Write(repeatCount_);
            _s.Write(rewardBitFlag_);
            _s.Write(isPurchase_);
        }
    };    
    public List<Piece> infos_;
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoUnexpectedPackage.Piece _v)
    {
        _v = new PktInfoUnexpectedPackage.Piece();
        return _v.Read(_s);
    }

    public static void Write(Message _s, PktInfoUnexpectedPackage.Piece _v)
    {
        _v.Write(_s);
    }

    public static bool Read(Message _s, out PktInfoUnexpectedPackage _v)
    {
        _v = new PktInfoUnexpectedPackage();
        return ReadList(_s, out _v.infos_);
    }

    public static void Write(Message _s, PktInfoUnexpectedPackage _v)
    {
        WriteList(_s, _v.infos_);
    }
}



// 월정액 정보 패킷 메시지
public class PktInfoMonthlyFee : PktMsgType
{
    public class Piece : PktMsgType
    {
        public PktInfoTime endTM_;          // 월정액 종료 시간
        public System.UInt32 tableID_;      // 월정액 TID
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
    public List<Piece> infos_;
};
// 로테이션 가챠 열기 패킷 메시지
public class PktInfoUserRGachaOpenAck : PktMsgType
{
    public PktInfoGoods goods_;         // 소모된 비용이 적용된 현재 재화
    public PktInfoComTimeAndTID infos_; // 로비 테마 ID 목록
};
// 레이드 상점 정보 패킷 메시지
public class PktInfoRaidStore : PktMsgType
{
    public PktInfoTime resetTM_;        // 리셋 시간
    public System.UInt32 showFlag_;     // 출력 정보
};
// 레이드 상점 목록 요청 패킷 메시지
public class PktInfoRaidStoreListReq : PktMsgType
{
    public PktInfoUInt8List resetNums_; // 갱신하고 싶은 레이드 상품 그룹안의 번호 목록(빈 값으로 보내면 전체 목록 요청임)
};
// 레이드 상점 목록 응답 패킷 메시지
public class PktInfoRaidStoreListAck : PktMsgType
{
    public PktInfoConsumeItemAndGoods consume_; // 소모 아이템 및 재화 정보
    public PktInfoRaidStore info_;      // 레이드 상점 정보
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 무료 상점 정보 패킷 메시지
    public static bool Read(Message _s, out PktInfoStoreSale.Piece _v) {
        _v = new PktInfoStoreSale.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoStoreSale.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoStoreSale _v) {
        _v = new PktInfoStoreSale();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoStoreSale _v) {
        WriteList(_s, _v.infos_);
    }
    // 월정액 정보 패킷 메시지
    public static bool Read(Message _s, out PktInfoMonthlyFee.Piece _v) {
        _v = new PktInfoMonthlyFee.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoMonthlyFee.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoMonthlyFee _v) {
        _v = new PktInfoMonthlyFee();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoMonthlyFee _v) {
        WriteList(_s, _v.infos_);
    }
    // 로테이션 가챠 열기 패킷 메시지
    public static bool Read(Message _s, out PktInfoUserRGachaOpenAck _v) {
        _v = new PktInfoUserRGachaOpenAck();
        if (false == Read(_s, out _v.goods_)) return false;
        if (false == Read(_s, out _v.infos_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoUserRGachaOpenAck _v) {
        Write(_s, _v.goods_);
        Write(_s, _v.infos_);
    }
    // 레이드 상점 정보 패킷 메시지
    public static bool Read(Message _s, out PktInfoRaidStore _v) {
        _v = new PktInfoRaidStore();
        if (false == Read(_s, out _v.resetTM_)) return false;
        if (false == _s.Read(out _v.showFlag_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoRaidStore _v) {
        Write(_s, _v.resetTM_);
        _s.Write(_v.showFlag_);
    }
    // 로테이션 가챠 열기 패킷 메시지
    public static bool Read(Message _s, out PktInfoRaidStoreListReq _v) {
        _v = new PktInfoRaidStoreListReq();
        if (false == Read(_s, out _v.resetNums_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoRaidStoreListReq _v) {
        Write(_s, _v.resetNums_);
    }
    // 로테이션 가챠 열기 패킷 메시지
    public static bool Read(Message _s, out PktInfoRaidStoreListAck _v) {
        _v = new PktInfoRaidStoreListAck();
        if (false == Read(_s, out _v.consume_)) return false;
        if (false == Read(_s, out _v.info_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoRaidStoreListAck _v) {
        Write(_s, _v.consume_);
        Write(_s, _v.info_);
    }
}
///
//////////////////////////////////////////////////////////////////////////


//////////////////////////////////////////////////////////////////////////
///
// 몬스터 도감 패킷 메시지
public class PktInfoMonsterBook : PktMsgType
{
    public class Piece : PktMsgType
    {
        public enum STATE : System.UInt32 {
            NEW_CHK     = 0,            // 신규 획득 확인 여부
            
            _MAX_
        };

        public System.UInt32 tableID_;
        public System.UInt32 stateFlag_;

        // 원하는 부분을 활성화 했는지 확인합니다.
        public bool IsOnFlag(int _flagIdx /*PktInfoMonsterBook.Piece.STATE*/) {
            if (32 <= _flagIdx)
                return false;
            return _IsOnBitIdx(stateFlag_, (uint)_flagIdx);
        }
        // 원하는 부분을 활성화 했는지 확인합니다.
        public void DoOnFlag(int _flagIdx /*PktInfoMonsterBook.Piece.STATE*/) {
            if (32 <= _flagIdx) return;
            _DoOnBitIdx(ref stateFlag_, (uint)_flagIdx);
        }
        public override bool Read(Message _s) {
            if (false == _s.Read(out tableID_)) return false;
            return _s.Read(out stateFlag_);
        }
        public override void Write(Message _s) {
            _s.Write(tableID_);
            _s.Write(stateFlag_);
        }
    };
    public List<Piece> infos_;
};

// 도감 확인 패킷 메시지
public class PktInfoBookNewConfirm : PktMsgType
{
    public System.UInt32 retStateFlag_;     // 확인 상태가 활성화된 도감 상태 플레그
    public System.UInt32 bookTID_;          // 도감 테이블 ID - (클라이언트에서 채워줘야 할 값)
    public eBookGroup bookGroup_;           // 도감 그룹 - (클라이언트에서 채워줘야 할 값)
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 몬스터 도감 패킷 메시지
    public static bool Read(Message _s, out PktInfoMonsterBook.Piece _v) {
        _v = new PktInfoMonsterBook.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoMonsterBook.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoMonsterBook _v) {
        _v = new PktInfoMonsterBook();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoMonsterBook _v) {
        WriteList(_s, _v.infos_);
    }
    // 도감 확인 패킷 메시지
    public static bool Read(Message _s, out PktInfoBookNewConfirm _v) {
        _v = new PktInfoBookNewConfirm();
        if (false == _s.Read(out _v.retStateFlag_)) return false;
        if (false == _s.Read(out _v.bookTID_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.bookGroup_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoBookNewConfirm _v) {
        _s.Write(_v.retStateFlag_);
        _s.Write(_v.bookTID_);
        PN_MarshalerEx.Write(_s, _v.bookGroup_);
    }
}
///
//////////////////////////////////////////////////////////////////////////


//////////////////////////////////////////////////////////////////////////
///
// 유저 마크 패킷 메시지
public class PktInfoUserMark : PktMsgType
{
    public enum FLAG : System.Byte {
        _0              = 0,
        _1,

        _MAX_,
    };
    public System.UInt64 sysMarkFlag_;      // 시스템 적으로 컨트롤할 마크 플레그(현재 사용하지 않고 있음)
    public System.UInt64[] flags_ = new System.UInt64[(int)FLAG._MAX_]; // 유저가 컨트롤할 마크 플레그
};
// 유저 마크 획득 패킷 메시지
public class PktInfoUserMarkTake : PktMsgType
{
    public PktInfoUserMark info_;           // 마크 정보
    public PktInfoTIDList tids_;            // 마크ID 목록
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 유저 마크 패킷 메시지
    public static bool Read(Message _s, out PktInfoUserMark _v) {
        _v = new PktInfoUserMark();
        if (false == _s.Read(out _v.sysMarkFlag_)) return false;
        for (int loop = 0; loop < (int)PktInfoUserMark.FLAG._MAX_; ++loop)
            if (false == _s.Read(out _v.flags_[loop]))
                return false;
        return true;
    }
    public static void Write(Message _s, PktInfoUserMark _v) {
        _s.Write(_v.sysMarkFlag_);
        for (int loop = 0; loop < (int)PktInfoUserMark.FLAG._MAX_; ++loop)
            _s.Write(_v.flags_[loop]);
    }
    // 유저 마크 획득 패킷 메시지
    public static bool Read(Message _s, out PktInfoUserMarkTake _v) {
        _v = new PktInfoUserMarkTake();
        if (false == PN_MarshalerEx.Read(_s, out _v.info_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.tids_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoUserMarkTake _v) {
        PN_MarshalerEx.Write(_s, _v.info_);
        PN_MarshalerEx.Write(_s, _v.tids_);
    }
}

// 로비 테마 패킷 메시지
public class PktInfoLobbyTheme : PktMsgType
{
    public enum FLAG : System.Byte {
        _0              = 0,
        _1,

        _MAX_,
    };
    public System.UInt64 sysFlag_;          // 시스템 적으로 컨트롤할 로비 테마 플레그(현재 사용하지 않고 있음)
    public System.UInt64[] flags_ = new System.UInt64[(int)FLAG._MAX_]; // 유저가 컨트롤할 로비 테마 플레그
};
// 로비 테마 획득 패킷 메시지
public class PktInfoLobbyThemeTake : PktMsgType
{
    public PktInfoLobbyTheme info_;         // 로비 테마 정보
    public PktInfoTIDList tids_;            // 로비 테마 ID 목록
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 로비 테마 패킷 메시지
    public static bool Read(Message _s, out PktInfoLobbyTheme _v) {
        _v = new PktInfoLobbyTheme();
        if (false == _s.Read(out _v.sysFlag_)) return false;
        for (int loop = 0; loop < (int)PktInfoLobbyTheme.FLAG._MAX_; ++loop)
            if (false == _s.Read(out _v.flags_[loop]))
                return false;
        return true;
    }
    public static void Write(Message _s, PktInfoLobbyTheme _v) {
        _s.Write(_v.sysFlag_);
        for (int loop = 0; loop < (int)PktInfoLobbyTheme.FLAG._MAX_; ++loop)
            _s.Write(_v.flags_[loop]);
    }
    // 로비 테마 획득 패킷 메시지
    public static bool Read(Message _s, out PktInfoLobbyThemeTake _v) {
        _v = new PktInfoLobbyThemeTake();
        if (false == Read(_s, out _v.info_)) return false;
        if (false == Read(_s, out _v.tids_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoLobbyThemeTake _v) {
        Write(_s, _v.info_);
        Write(_s, _v.tids_);
    }
}

// 서클 유저 개인의 채팅 스탬프 정보
public class PktInfoChatStamp : PktMsgType
{
	public enum FLAG : System.Byte
	{
		_0 = 0,
		_1,

		_MAX_,
	};

	public ulong[] flag_ = new ulong[(int)FLAG._MAX_];		// 유저가 소유한 채팅 스탬프 플래그
};

public class PktInfoChatStampTake : PktMsgType
{
	public PktInfoChatStamp info_;         // 변경된 채팅 스탬프 소유 정보
	public PktInfoTIDList tids_;           // 채팅 스탬프 테이블 아이디 목록
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	// 유저 채팅 스탬프 플래그
	public static bool Read(Message _s, out PktInfoChatStamp _v)
	{
		_v = new PktInfoChatStamp();
		for (int loop = 0; loop < (int)PktInfoChatStamp.FLAG._MAX_; ++loop)
			if (false == _s.Read(out _v.flag_[loop]))
				return false;
		return true;
	}
	public static void Write(Message _s, PktInfoChatStamp _v)
	{
		for (int loop = 0; loop < (int)PktInfoChatStamp.FLAG._MAX_; ++loop)
			_s.Write(_v.flag_[loop]);
	}

	// 유저 채팅스탬프 획득 메시지
	public static bool Read(Message _s, out PktInfoChatStampTake _v)
	{
		_v = new PktInfoChatStampTake();
		if (false == Read(_s, out _v.info_)) return false;
		if (false == Read(_s, out _v.tids_)) return false;
		return true;
	}
	public static void Write(Message _s, PktInfoChatStampTake _v)
	{
		Write(_s, _v.info_);
		Write(_s, _v.tids_);
	}
}

// 서포터 진형 즐겨찾기 패킷 메시지
public class PktInfoCardFormaFavi : PktMsgType
{
    public enum FLAG : System.Byte {
        _0              = 0,
        _1,
        _2,
        _3,

        _MAX_,
    };
    public System.UInt64[] flags_ = new System.UInt64[(int)FLAG._MAX_]; // 유저가 컨트롤할 로비 테마 플레그

    // 원하는 보상을 획득했는지 확인합니다.
    public bool IsOnFlag(int _flagIdx)
    {
        int setIdx = _flagIdx / 64;
        if ((int)FLAG._MAX_ <= setIdx)          return false;

        long checkIdx = _flagIdx % 64;
        bool result   = _IsOnBitIdx(flags_[setIdx], (ulong)checkIdx);
        return result;
}
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 서포터 진형 즐겨찾기 패킷 메시지
    public static bool Read(Message _s, out PktInfoCardFormaFavi _v) {
        _v = new PktInfoCardFormaFavi();
        for (int loop = 0; loop < (int)PktInfoCardFormaFavi.FLAG._MAX_; ++loop)
            if (false == _s.Read(out _v.flags_[loop]))
                return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCardFormaFavi _v) {
        for (int loop = 0; loop < (int)PktInfoCardFormaFavi.FLAG._MAX_; ++loop)
            _s.Write(_v.flags_[loop]);
    }
}
///
//////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////
///
// 서버 달성(세력) 정보 패킷 메시지
public class PktInfoInfluence : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt64 point_;            // 세력이 획득한 점수
        public System.UInt32 influID_;          // 세력 ID
        public System.UInt32 userCnt_;          // 세력에 참여한 유저수

        public override bool Read(Message _s) {
            if (false == _s.Read(out point_)) return false;
            if (false == _s.Read(out influID_)) return false;
            return _s.Read(out userCnt_);
        }
        public override void Write(Message _s) {
            _s.Write(point_);
            _s.Write(influID_);
            _s.Write(userCnt_);
        }
    };
    public List<Piece> infos_;
    public System.UInt64 idInRankTbl_;      // 모든 세력의 포인트를 합산한 값
};
// 서버 달성(세력) 선택 패킷 메시지
public class PktInfoInfluenceChoice : PktMsgType
{
    public System.UInt32 tid_;              // 선택한 세력 ID
};
// 서버 달성(세력) 랭킹 요청 패킷 메시지
public class PktInfoInfluRankListReq : PktMsgType
{
    public PktInfoTime lastUpTM_;           // 마지막으로 업데이트 된 시간 - (클라이언트에서 채워줘야 할 값)
    public System.UInt64 reqUuid_;          // 요청을 보낸 유저 고유 ID
};
// 서버 달성(세력) 보상 획득 요청 패킷 메시지
public class PktInfoRwdInfluenceTgtReq : PktMsgType
{
    public PktInfoUInt8List idxs_;          // 획득을 요청할 보상 인덱스 (PktInfoMission::Influ::TARGET)
    public System.Byte rwdTP_;              // 요청한 보상 타입 (RWD_TP)
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 서버 달성(세력) 정보 패킷 메시지
    public static bool Read(Message _s, out PktInfoInfluence.Piece _v) {
        _v = new PktInfoInfluence.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoInfluence.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoInfluence _v) {
        _v = new PktInfoInfluence();
        if (false == ReadList(_s, out _v.infos_)) return false;
        return _s.Read(out _v.idInRankTbl_);
    }
    public static void Write(Message _s, PktInfoInfluence _v) {
        WriteList(_s, _v.infos_);
        _s.Write(_v.idInRankTbl_);
    }
    // 서버 달성(세력) 선택 패킷 메시지
    public static bool Read(Message _s, out PktInfoInfluenceChoice _v) {
        _v = new PktInfoInfluenceChoice();
        return _s.Read(out _v.tid_);
    }
    public static void Write(Message _s, PktInfoInfluenceChoice _v) {
        _s.Write(_v.tid_);
    }
    // 서버 달성(세력) 랭킹 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoInfluRankListReq _v) {
        _v = new PktInfoInfluRankListReq();
        if (false == Read(_s, out _v.lastUpTM_)) return false;
        return _s.Read(out _v.reqUuid_);
    }
    public static void Write(Message _s, PktInfoInfluRankListReq _v) {
        Write(_s, _v.lastUpTM_);
        _s.Write(_v.reqUuid_);
    }
    // 서버 달성(세력) 보상 획득 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoRwdInfluenceTgtReq _v) {
        _v = new PktInfoRwdInfluenceTgtReq();
        if (false == Read(_s, out _v.idxs_)) return false;
        return _s.Read(out _v.rwdTP_);
    }
    public static void Write(Message _s, PktInfoRwdInfluenceTgtReq _v) {
        Write(_s, _v.idxs_);
        _s.Write(_v.rwdTP_);
    }
}
// 유저 프리셋 설정/갱신 요청 패킷 메시지
public class PktAddOrUpdatePreset : PktMsgType
{
	public ePresetKind	kind_;
	public ulong		cuid_;
	public byte			slotNum_;

	public override bool Read(Message _s)
	{
		if (false == PN_MarshalerEx.Read(_s, out kind_)) return false;
		if (false == _s.Read(out cuid_)) return false;
		if (false == _s.Read(out slotNum_)) return false;
		return true;
	}

	public override void Write(Message _s)
	{
		PN_MarshalerEx.Write(_s, kind_);
		_s.Write(cuid_);
		_s.Write(slotNum_);
	}
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktAddOrUpdatePreset _v)
	{
		_v = new PktAddOrUpdatePreset();
		return _v.Read(_s);
	}

	public static void Write(Message _s, PktAddOrUpdatePreset _v)
	{
		_v.Write(_s);
	}
}

public class PktInfoPresetCommon : PktMsgType
{
	public ePresetKind kind_;
	public ulong cuid_;
	public byte slotNum_;
	public PktInfoStr name_;

	public override bool Read(Message _s)
	{
		if (false == PN_MarshalerEx.Read(_s, out kind_)) return false;
		if (false == _s.Read(out cuid_)) return false;
		if (false == _s.Read(out slotNum_)) return false;
		if (false == PN_MarshalerEx.Read(_s, out name_)) return false;
		return true;
	}

	public override void Write(Message _s)
	{
		PN_MarshalerEx.Write(_s, kind_);
		_s.Write(cuid_);
		_s.Write(slotNum_);
		PN_MarshalerEx.Write(_s, name_);
	}
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoPresetCommon _v)
	{
		_v = new PktInfoPresetCommon();
		return _v.Read(_s);
	}

	public static void Write(Message _s, PktInfoPresetCommon _v)
	{
		_v.Write(_s);
	}
}
///
//////////////////////////////////////////////////////////////////////////