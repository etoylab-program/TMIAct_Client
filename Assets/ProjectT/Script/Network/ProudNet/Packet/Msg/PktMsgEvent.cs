using System.Collections.Generic;
using Nettention.Proud;


// 이벤트 보상 패킷 메시지
public class PktInfoEventReward : PktMsgType
{
    public enum ENUM : System.Byte {
        _START_         = 0,

        _1              = _START_,
        _2,
        _3,
        _4,
        _5,
        _6,
        _7,
        _8,
        _9,
        _10,
        _11,
        _12,
        _13,
        _14,
        _15,
        _16,
        _17,
        _18,
        _19,
        _20,

        _MAX_,
    };
    public class Piece : PktMsgType
    {
        public System.UInt32 tableID_;      // 이벤트 테이블 ID
        public System.UInt32 value_;        // 이벤트 보상 관련 값
        public System.Byte step_;           // 이벤트 보상 단계
        public System.Byte[] cnts_ = new System.Byte[(int)ENUM._MAX_];  // 보상 개수 관련

        public override bool Read(Message _s) {
            if (false == _s.Read(out tableID_)) return false;
            if (false == _s.Read(out value_)) return false;
            if (false == _s.Read(out step_)) return false;
            for (int loop = 0; loop < (int)ENUM._MAX_; ++loop)
            {
                if (false == _s.Read(out cnts_[loop]))
                    return false;
            }
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(value_);
            _s.Write(tableID_);
            _s.Write(step_);
            for (int loop = 0; loop < (int)ENUM._MAX_; ++loop)
                _s.Write(cnts_[loop]);
        }

        public System.String GetStr() {
            System.String str = string.Format("[TID:{0} Step:{1} Val:{2}]", tableID_, step_, value_);
            for (int loop = 0; loop < (int)ENUM._MAX_;  ++loop)
                str += string.Format(" {0}_:[{1}]", loop, cnts_[loop]);
            str += string.Format("\n");
            return str;
        }
    };
    public List<Piece> infos_;
    public System.String GetStr() {
        System.String str = string.Format("PktEvtRwd");
        foreach (var info in infos_)
            str += info.GetStr();
        return str;
    }
};
// 이벤트 로그인 정보 패킷 메시지
public class PktInfoEvtLogin : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt64 rwds_;         // 보상 받은 정보
        public System.UInt32 tid_;          // 이벤트 테이블 ID
        public System.Byte day_;            // 시작 기준으로 마지막으로 보상을 받은 날
        public System.Byte endDay_;         // 마지막 보상을 받을 날짜

        public override bool Read(Message _s) {
            if (false == _s.Read(out rwds_)) return false;
            if (false == _s.Read(out tid_)) return false;
            if (false == _s.Read(out day_)) return false;
            if (false == _s.Read(out endDay_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(rwds_);
            _s.Write(tid_);
            _s.Write(day_);
            _s.Write(endDay_);
        }
    };
    public List<Piece> infos_;
};
// 이벤트 패킷 메시지
public class PktInfoEvent : PktMsgType
{
    public PktInfoEventReward reward_;  // 이벤트 스테이지 모드 보상 정보
    public PktInfoEvtLogin login_;      // 이벤트 로그인 정보
};
// 유저 이벤트 보상 요청 패킷 메시지
public class PktInfoEventRewardReq : PktMsgType
{
    public System.UInt32 eventID_;      // 보상을 원하는 이벤트 ID - (클라이언트에서 채워줘야 할 값)
    public System.Byte step_;           // 보상을 원하는 스텝 번호 - (클라이언트에서 채워줘야 할 값)
    public System.Byte idx_;            // 보상을 원하는 인덱스(필요한 타입에서만 사용 - 기본 값은 0) - (클라이언트에서 채워줘야 할 값)
    public System.Byte cnt_;            // 보상 수량 - (클라이언트에서 채워줘야 할 값)
};
// 이벤트 보상 재설정 패킷 메시지
public class PktInfoEventRewardReset : PktMsgType
{
    public List<System.Byte> del_;      // 제거된 스텝 정보 들
    public PktInfoEventReward add_;     // 추가된 이벤트 보상 정보
    public PktInfoEventReward update_;  // 갱신된 이벤트 보상 정보
    public System.UInt32 eventID_;      // 기존 이벤트 ID
};
// 유저 이벤트 변경 목록 패킷 메시지
public class PktInfoEventChangeList : PktMsgType
{
    public class Piece : PktMsgType
    {
        public PktInfoTIDList endList_;         // 종료된 이벤트 ID 목록
        public PktInfoEventReward addEvtRwds_;  // 활성화된 이벤트 보상 정보
        public System.UInt64 uuid_;             // 유저 고유 ID
        public eTMIErrNum err_;                 // 에러 번호

        public override bool Read(Message _s) {
            if (false == PN_MarshalerEx.Read(_s, out endList_)) return false;
            if (false == PN_MarshalerEx.Read(_s, out addEvtRwds_)) return false;
            if (false == _s.Read(out uuid_)) return false;
            if (false == PN_MarshalerEx.Read(_s, out err_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            PN_MarshalerEx.Write(_s, endList_);
            PN_MarshalerEx.Write(_s, addEvtRwds_);
            _s.Write(uuid_);
            PN_MarshalerEx.Write(_s, err_);
        }
        public System.String GetStr() {
            System.String str = string.Format("PktInfoEventChangeList - EndEvent {0}\n", endList_.GetStr());
            str += string.Format("AddEvent {0} ", addEvtRwds_.GetStr());
            return str;
        }
    };
    public List<Piece> infos_;
};
// 이벤트 로그인 보상 요청 패킷 메시지
public class PktInfoEvtLgnRwdReq : PktMsgType
{
    public PktInfoUInt8List rwdDays_;           // 원하는 보상 날짜 목록
    public System.UInt32 evtLgnTID_;            // 이벤트 로그인 TID
};
// 이벤트 로그인 보상 응답 패킷 메시지
public class PktInfoEvtLgnRwdAck : PktMsgType
{
    public PktInfoConsumeItemAndGoods consume_; // 비용 소모가 적용된 아이템 및 재화
    public PktInfoEvtLogin evtLgn_;             // 이벤트 로그인 정보
    public PktInfoUInt8List rwdDays_;           // 원하는 보상 날짜 목록
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 이벤트 보상 패킷 메시지
    public static bool Read(Message _s, out PktInfoEventReward.Piece _v) {
        _v = new PktInfoEventReward.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoEventReward.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoEventReward _v) {
        _v = new PktInfoEventReward();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoEventReward _v) {
        WriteList(_s, _v.infos_);
    }
    // 이벤트 로그인 정보 패킷 메시지
    public static bool Read(Message _s, out PktInfoEvtLogin.Piece _v) {
        _v = new PktInfoEvtLogin.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoEvtLogin.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoEvtLogin _v) {
        _v = new PktInfoEvtLogin();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoEvtLogin _v) {
        WriteList(_s, _v.infos_);
    }

    // 이벤트 패킷 메시지
    public static bool Read(Message _s, out PktInfoEvent _v) {
        _v = new PktInfoEvent();
        if (false == Read(_s, out _v.reward_)) return false;
        if (false == Read(_s, out _v.login_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoEvent _v) {
        Write(_s, _v.reward_);
        Write(_s, _v.login_);
    }

    // 유저 이벤트 보상 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoEventRewardReq _v) {
        _v = new PktInfoEventRewardReq();
        if (false == _s.Read(out _v.eventID_)) return false;
        if (false == _s.Read(out _v.step_)) return false;
        if (false == _s.Read(out _v.idx_)) return false;
        if (false == _s.Read(out _v.cnt_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoEventRewardReq _v) {
        _s.Write(_v.eventID_);
        _s.Write(_v.step_);
        _s.Write(_v.idx_);
        _s.Write(_v.cnt_);
    }

    // 이벤트 보상 재설정 패킷 메시지
    public static bool Read(Message _s, out PktInfoEventRewardReset _v) {
        _v = new PktInfoEventRewardReset();
        if (false == Read(_s, out _v.del_)) return false;
        if (false == Read(_s, out _v.add_)) return false;
        if (false == Read(_s, out _v.update_)) return false;
        if (false == _s.Read(out _v.eventID_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoEventRewardReset _v) {
        Write(_s, _v.del_);
        Write(_s, _v.add_);
        Write(_s, _v.update_);
        _s.Write(_v.eventID_);
    }

    // 유저 이벤트 변경 목록 패킷 메시지
    public static bool Read(Message _s, out PktInfoEventChangeList.Piece _v) {
        _v = new PktInfoEventChangeList.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoEventChangeList.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoEventChangeList _v) {
        _v = new PktInfoEventChangeList();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoEventChangeList _v) {
        WriteList(_s, _v.infos_);
    }

    // 이벤트 로그인 보상 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoEvtLgnRwdReq _v) {
        _v = new PktInfoEvtLgnRwdReq();
        if (false == Read(_s, out _v.rwdDays_)) return false;
        if (false == _s.Read(out _v.evtLgnTID_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoEvtLgnRwdReq _v) {
        Write(_s, _v.rwdDays_);
        _s.Write(_v.evtLgnTID_);
    }
    // 이벤트 로그인 보상 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoEvtLgnRwdAck _v) {
        _v = new PktInfoEvtLgnRwdAck();
        if (false == Read(_s, out _v.consume_)) return false;
        if (false == Read(_s, out _v.evtLgn_)) return false;
        if (false == Read(_s, out _v.rwdDays_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoEvtLgnRwdAck _v) {
        Write(_s, _v.consume_);
        Write(_s, _v.evtLgn_);
        Write(_s, _v.rwdDays_);
    }
}

public class PktInfoUserBingoEvent : PktMsgType
{
	public class Piece : PktMsgType
	{
		public System.UInt32 GroupID;
		public System.Byte No;
		public System.UInt16 RwdFlag;

		public override bool Read(Message _s)
		{
			if (false == _s.Read(out GroupID)) return false;
			if (false == _s.Read(out No)) return false;
			if (false == _s.Read(out RwdFlag)) return false;
			return true;
		}
		public override void Write(Message _s)
		{
			_s.Write(GroupID);
			_s.Write(No);
			_s.Write(RwdFlag);
		}
	};

	public List<Piece> infos_;
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoUserBingoEvent.Piece _v)
	{
		_v = new PktInfoUserBingoEvent.Piece();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoUserBingoEvent.Piece _v)
	{
		_v.Write(_s);
	}
	public static bool Read(Message _s, out PktInfoUserBingoEvent _v)
	{
		_v = new PktInfoUserBingoEvent();
		return ReadList(_s, out _v.infos_);
	}
	public static void Write(Message _s, PktInfoUserBingoEvent _v)
	{
		WriteList(_s, _v.infos_);
	}
}

public class PktInfoBingoEvtNextRoundOpen : PktMsgType
{
    public PktInfoConsumeItemAndGoods consume_;    // 비용 소모가 적용된 아이템 및 재화
    public PktInfoUserBingoEvent.Piece evtBingo_;  // 새로운 빙고 이벤트 정보
}

public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoBingoEvtNextRoundOpen _v)
	{
		_v = new PktInfoBingoEvtNextRoundOpen();
        if (false == Read(_s, out _v.consume_)) return false;
        if (false == Read(_s, out _v.evtBingo_)) return false;
        return true;
	}
	public static void Write(Message _s, PktInfoBingoEvtNextRoundOpen _v)
	{
        Write(_s, _v.consume_);
        Write(_s, _v.evtBingo_);
	}
}

public class PktInfoAchieveEvent : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt32 groupID_;          // 이벤트 ID
        public System.UInt32 achieveGroup_;     // 공적 그룹 ID
        public System.UInt32 condiVal_;         // 진행 조건 현재 수치값
        public System.Byte groupOrder_;         // 각각의 보상 획득 여부

        public override bool Read(Message _s)
        {
            if (false == _s.Read(out groupID_)) return false;
            if (false == _s.Read(out achieveGroup_)) return false;
            if (false == _s.Read(out condiVal_)) return false;
            if (false == _s.Read(out groupOrder_)) return false;
            return true;
        }
        public override void Write(Message _s)
        {
            _s.Write(groupID_);
            _s.Write(achieveGroup_);
            _s.Write(condiVal_);
            _s.Write(groupOrder_);
        }
    };
    public List<Piece> infos_;
};


public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 공적이벤트 패킷 메시지
    public static bool Read(Message _s, out PktInfoAchieveEvent.Piece _v)
    {
        _v = new PktInfoAchieveEvent.Piece();
        if (false == _v.Read(_s)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoAchieveEvent.Piece _v)
    {
        _v.Write(_s);
    }
    // 공적이벤트 패킷 메시지
    public static bool Read(Message _s, out PktInfoAchieveEvent _v)
    {
        _v = new PktInfoAchieveEvent();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoAchieveEvent _v)
    {
        WriteList(_s, _v.infos_);
    }
    // 무기 성장 패킷 메시지
   
}