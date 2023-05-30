using System.Collections.Generic;
using Nettention.Proud;


///////////////////////////////////////////////////////////////////////
///
// 서클 생성요청 패킷 메시지
public class PktInfoCircleOpenReq : PktMsgType
{
	public PktInfoStr name_;
	public PktInfoStr comment_;
	public eLANGUAGE lang_;

	public bool suggestAnotherLang_;

	public override bool Read(Message _s)
	{
		if (false == PN_MarshalerEx.Read(_s, out name_)) return false;
		if (false == PN_MarshalerEx.Read(_s, out comment_)) return false;
		if (false == PN_MarshalerEx.Read(_s, out lang_)) return false;
		if (false == _s.Read(out suggestAnotherLang_)) return false;
		return true;
	}
	public override void Write(Message _s)
	{
		PN_MarshalerEx.Write(_s, name_);
		PN_MarshalerEx.Write(_s, comment_);
		PN_MarshalerEx.Write(_s, lang_);
		_s.Write(suggestAnotherLang_);
	}
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoCircleOpenReq _v)
	{
		_v = new PktInfoCircleOpenReq();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoCircleOpenReq _v)
	{
		_v.Write(_s);
	}
}

public class PktInfoCircleMarkSet : PktMsgType
{
	public uint markTID_;
	public uint flagTID_;
	public uint colorTID_;

	public override bool Read(Message _s)
	{
		if (false == _s.Read(out markTID_)) return false;
		if (false == _s.Read(out flagTID_)) return false;
		if (false == _s.Read(out colorTID_)) return false;
		return true;
	}
	public override void Write(Message _s)
	{
		_s.Write(markTID_);
		_s.Write(flagTID_);
		_s.Write(colorTID_);
	}
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoCircleMarkSet _v)
	{
		_v = new PktInfoCircleMarkSet();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoCircleMarkSet _v)
	{
		_v.Write(_s);
	}
}

public class PktInfoCircleSimple : PktMsgType
{
	public class LeaderComunityInfo : PktMsgType
	{
		public PktInfoStr nickName_;       // 유저 닉네임
		public PktInfoTime lastConnTM_;    // 유저 마지막 접속 시간
		public uint mark_;     // 유저 마크 ID
		public byte rank_;      // 유저 랭크

		public override bool Read(Message _s)
		{
			if (false == PN_MarshalerEx.Read(_s, out nickName_)) return false;
			if (false == _s.Read(out rank_)) return false;
			if (false == _s.Read(out mark_)) return false;
			if (false == PN_MarshalerEx.Read(_s, out lastConnTM_)) return false;
			return true;
		}
		public override void Write(Message _s)
		{
			PN_MarshalerEx.Write(_s, nickName_);
			_s.Write(rank_);
			_s.Write(mark_);
			PN_MarshalerEx.Write(_s, lastConnTM_);
		}
	};

	public class Piece : PktMsgType
	{
		public ulong ID_;							// 서클 고유아이디
		public ushort rank_;						// 서클 레벨
		public PktInfoStr name_;					// 서클명
		public PktInfoStr comment_;					// 서클 안내문구
		public ulong leaderID_;						// 부장 UUID
		public PktInfoCircleMarkSet markSet_;		// 설정된 마크정보
		public byte memCount_;						// 현재 가입된 인원 수
		public byte maxMemCount_;					// 서클의 최대 수용가능 인원
		public eLANGUAGE lang_;						// 서클 주사용 언어
		public bool suggestAnotherLang_;			// 다른언어 유저 가입 허용여부

		public LeaderComunityInfo leaderInfo_;		// 부장의 서클용 커뮤니티 정보

		public override bool Read(Message _s)
		{
			if (false == _s.Read(out ID_)) return false;
			if (false == _s.Read(out rank_)) return false;
			if (false == PN_MarshalerEx.Read(_s, out name_)) return false;
			if (false == PN_MarshalerEx.Read(_s, out comment_)) return false;
			if (false == _s.Read(out leaderID_)) return false;
			if (false == PN_MarshalerEx.Read(_s, out markSet_)) return false;
			if (false == _s.Read(out memCount_)) return false;
			if (false == _s.Read(out maxMemCount_)) return false;
			if (false == PN_MarshalerEx.Read(_s, out lang_)) return false;
			if (false == _s.Read(out suggestAnotherLang_)) return false;
			if (false == PN_MarshalerEx.Read(_s, out leaderInfo_)) return false;
			return true;
		}
		public override void Write(Message _s)
		{
			_s.Write(ID_);
			_s.Write(rank_);
			PN_MarshalerEx.Write(_s, name_);
			PN_MarshalerEx.Write(_s, comment_);
			_s.Write(leaderID_);
			PN_MarshalerEx.Write(_s, markSet_);
			_s.Write(memCount_);
			_s.Write(maxMemCount_);
			PN_MarshalerEx.Write(_s, lang_);
			_s.Write(suggestAnotherLang_);
			PN_MarshalerEx.Write(_s, leaderInfo_);
		}
	};

	public List<Piece> infos_;
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoCircleSimple.LeaderComunityInfo _v)
	{
		_v = new PktInfoCircleSimple.LeaderComunityInfo();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoCircleSimple.LeaderComunityInfo _v)
	{
		_v.Write(_s);
	}

	public static bool Read(Message _s, out PktInfoCircleSimple.Piece _v)
	{
		_v = new PktInfoCircleSimple.Piece();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoCircleSimple.Piece _v)
	{
		_v.Write(_s);
	}

	public static bool Read(Message _s, out PktInfoCircleSimple _v)
	{
		_v = new PktInfoCircleSimple();
		return ReadList(_s, out _v.infos_);
	}
	public static void Write(Message _s, PktInfoCircleSimple _v)
	{
		WriteList(_s, _v.infos_);
	}
}

public class PktInfoCircleMark : PktMsgType
{
	public enum FLAG
	{
		_0 = 0,
		_1,

		_MAX_,
	}

	public ulong[] ownedMark_ = new ulong[(int)FLAG._MAX_];
	public ulong[] ownedFlag_ = new ulong[(int)FLAG._MAX_];
	public ulong[] ownedColor_ = new ulong[(int)FLAG._MAX_];

	public override bool Read(Message _s)
	{
		for (int loop = 0; loop < (int)FLAG._MAX_; ++loop)
			if (false == _s.Read(out ownedMark_[loop]))
				return false;
		for (int loop = 0; loop < (int)FLAG._MAX_; ++loop)
			if (false == _s.Read(out ownedFlag_[loop]))
				return false;
		for (int loop = 0; loop < (int)FLAG._MAX_; ++loop)
			if (false == _s.Read(out ownedColor_[loop]))
				return false;
		return true;
	}
	public override void Write(Message _s)
	{
		for (int loop = 0; loop < (int)FLAG._MAX_; ++loop)
			_s.Write(ownedMark_[loop]);
		for (int loop = 0; loop < (int)FLAG._MAX_; ++loop)
			_s.Write(ownedFlag_[loop]);
		for (int loop = 0; loop < (int)FLAG._MAX_; ++loop)
			_s.Write(ownedColor_[loop]);
	}
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoCircleMark _v)
	{
		_v = new PktInfoCircleMark();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoCircleMark _v)
	{
		_v.Write(_s);
	}
}

public class PktInfoCircleFacility : PktMsgType
{
	public class Piece : PktMsgType
	{
		public uint facilityGroupID_;
		public ushort facilityLv_;

		public override bool Read(Message _s)
		{
			if (false == _s.Read(out facilityGroupID_)) return false;
			if (false == _s.Read(out facilityLv_)) return false;
			return true;
		}

		public override void Write(Message _s)
		{
			_s.Write(facilityGroupID_);
			_s.Write(facilityLv_);

		}
	}

	public List<Piece> infos_;
}

public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoCircleFacility.Piece _v)
	{
		_v = new PktInfoCircleFacility.Piece();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoCircleFacility.Piece _v)
	{
		_v.Write(_s);
	}

	public static bool Read(Message _s, out PktInfoCircleFacility _v)
	{
		_v = new PktInfoCircleFacility();
		return ReadList(_s, out _v.infos_);
	}
	public static void Write(Message _s, PktInfoCircleFacility _v)
	{
		WriteList(_s, _v.infos_);
	}
}

// 서클 기본정보 패킷 메시지
public class PktInfoCircle : PktMsgType
{
	public ulong ID_;
	public ulong[] goods_ = new ulong[(int)eCircleGoodsType.COUNT];
	public uint lobbySet_;
	public byte attendenceCnt_;
	public PktInfoTime recentAttendRwdDate_;
	public PktInfoTime nextUserKickPossibleTime_;
	public byte subLeaderCnt_;
	public byte maxSubLeaderCnt_;

	// 시설관련
	PktInfoCircleFacility facilityInfo_;
	// 소유한 마크정보
	PktInfoCircleMark markInfo_;

	public override bool Read(Message _s)
	{
		if (false == _s.Read(out ID_)) return false;
		if (false == _s.Read(out lobbySet_)) return false;
		if (false == _s.Read(out attendenceCnt_)) return false;
		if (false == PN_MarshalerEx.Read(_s, out facilityInfo_)) return false;
		if (false == PN_MarshalerEx.Read(_s, out markInfo_)) return false;
		if (false == PN_MarshalerEx.Read(_s, out recentAttendRwdDate_)) return false;
		if (false == PN_MarshalerEx.Read(_s, out nextUserKickPossibleTime_)) return false;
		if (false == _s.Read(out subLeaderCnt_)) return false;
		if (false == _s.Read(out maxSubLeaderCnt_)) return false;
		for(int loop = 0; loop < (int)eCircleGoodsType.COUNT; ++loop)
			if (false == _s.Read(out goods_[loop])) return false;
		return true;
	}
	public override void Write(Message _s)
	{
		_s.Write(ID_);
		_s.Write(lobbySet_);
		_s.Write(attendenceCnt_);
		PN_MarshalerEx.Write(_s, facilityInfo_);
		PN_MarshalerEx.Write(_s, markInfo_);
		PN_MarshalerEx.Write(_s, recentAttendRwdDate_);
		PN_MarshalerEx.Write(_s, nextUserKickPossibleTime_);
		_s.Write(subLeaderCnt_);
		_s.Write(maxSubLeaderCnt_);
		for(int loop = 0; loop < (int)eCircleGoodsType.COUNT; ++loop)
			_s.Write(goods_[loop]);
	}
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoCircle _v)
	{
		_v = new PktInfoCircle();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoCircle _v)
	{
		_v.Write(_s);
	}
}

// 서클 생성 요청에 대한 응답 메시지
public class PktInfoCircleOpenAck : PktMsgType
{
	public PktInfoCircleSimple.Piece simpleInfo_;
	public PktInfoCircle info_;
	public PktInfoConsumeItemAndGoods consume_;
	public PktInfoCircleAuthority authInfo_;

	public override bool Read(Message _s)
	{
		if (false == PN_MarshalerEx.Read(_s, out info_)) return false;
		if (false == PN_MarshalerEx.Read(_s, out simpleInfo_)) return false;
		if (false == PN_MarshalerEx.Read(_s, out consume_)) return false;
		if (false == PN_MarshalerEx.Read(_s, out authInfo_)) return false;
		return true;
	}
	public override void Write(Message _s)
	{
		PN_MarshalerEx.Write(_s, info_);
		PN_MarshalerEx.Write(_s, simpleInfo_);
		PN_MarshalerEx.Write(_s, consume_);
		PN_MarshalerEx.Write(_s, authInfo_);
	}
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoCircleOpenAck _v)
	{
		_v = new PktInfoCircleOpenAck();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoCircleOpenAck _v)
	{
		_v.Write(_s);
	}
}

// 서클 추천리스트 응답 메시지
public class PktInfoGetSuggestCircleAck : PktMsgType
{
	public PktInfoCircleSimple list_;
	public PktInfoCircleSimple joinReqList_;

	public override bool Read(Message _s)
	{
		if (false == PN_MarshalerEx.Read(_s, out list_)) return false;
		if (false == PN_MarshalerEx.Read(_s, out joinReqList_)) return false;
		return true;
	}
	public override void Write(Message _s)
	{
		PN_MarshalerEx.Write(_s, list_);
		PN_MarshalerEx.Write(_s, joinReqList_);
	}
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoGetSuggestCircleAck _v)
	{
		_v = new PktInfoGetSuggestCircleAck();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoGetSuggestCircleAck _v)
	{
		_v.Write(_s);
	}
}

// 서클 가입요청 메시지
public class PktInfoCircleJoinReq : PktMsgType
{
	public eLANGUAGE suggestLang_;
	public ulong joinReqCircleID_;

	public override bool Read(Message _s)
	{
		if (false == PN_MarshalerEx.Read(_s, out suggestLang_)) return false;
		if (false == _s.Read(out joinReqCircleID_)) return false;
		return true;
	}
	public override void Write(Message _s)
	{
		PN_MarshalerEx.Write(_s, suggestLang_);
		_s.Write(joinReqCircleID_);
	}
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoCircleJoinReq _v)
	{
		_v = new PktInfoCircleJoinReq();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoCircleJoinReq _v)
	{
		_v.Write(_s);
	}
}

// 서클 로비정보 요청 패킷메시지
public class PktInfoCircleLobby : PktMsgType
{
	public uint lobbySet_;
	public byte attendenceCnt_;

	public override bool Read(Message _s)
	{
		if (false == _s.Read(out lobbySet_)) return false;
		if (false == _s.Read(out attendenceCnt_)) return false;
		return true;
	}
	public override void Write(Message _s)
	{
		_s.Write(lobbySet_);
		_s.Write(attendenceCnt_);
	}
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoCircleLobby _v)
	{
		_v = new PktInfoCircleLobby();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoCircleLobby _v)
	{
		_v.Write(_s);
	}
}

// 서클 탈퇴 요청 응답 메시지
public class PktInfoCircleWithdrawalAck : PktMsgType
{
	public PktInfoTime possibleCircleJoinDate_;		// 다음 서클 가입 가능 날짜
	public PktInfoCircleAuthority authInfo_;		// 현재 유저의 서클 권한 정보

	public override bool Read(Message _s)
	{
		if (false == PN_MarshalerEx.Read(_s, out possibleCircleJoinDate_)) return false;
		if (false == PN_MarshalerEx.Read(_s, out authInfo_)) return false;
		return true;
	}
	public override void Write(Message _s)
	{
		PN_MarshalerEx.Write(_s, possibleCircleJoinDate_);
		PN_MarshalerEx.Write(_s, authInfo_);
	}
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoCircleWithdrawalAck _v)
	{
		_v = new PktInfoCircleWithdrawalAck();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoCircleWithdrawalAck _v)
	{
		_v.Write(_s);
	}
}

// 서클 유저리스트 메시지
public class PktInfoCircleUserList : PktMsgType
{
	public PktInfoComCommuUser userList_;      //	가입된 유저 리스트
	public PktInfoComCommuUser joinWaitList_;  //	가입대기 유저 리스트

	public override bool Read(Message _s)
	{
		if (false == PN_MarshalerEx.Read(_s, out userList_)) return false;
		if (false == PN_MarshalerEx.Read(_s, out joinWaitList_)) return false;
		return true;
	}
	public override void Write(Message _s)
	{
		PN_MarshalerEx.Write(_s, userList_);
		PN_MarshalerEx.Write(_s, joinWaitList_);
	}
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoCircleUserList _v)
	{
		_v = new PktInfoCircleUserList();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoCircleUserList _v)
	{
		_v.Write(_s);
	}
}

// 서클 유저 추방 패킷 메시지
public class PktInfoCircleUserKickAck : PktMsgType
{
	public PktInfoTime nextUserKickPossibleTime_;
	public PktInfoCircleUserList userList_;

	public override bool Read(Message _s)
	{
		if (false == PN_MarshalerEx.Read(_s, out nextUserKickPossibleTime_)) return false;
		if (false == PN_MarshalerEx.Read(_s, out userList_)) return false;
		return true;
	}
	public override void Write(Message _s)
	{
		PN_MarshalerEx.Write(_s, nextUserKickPossibleTime_);
		PN_MarshalerEx.Write(_s, userList_);
	}
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoCircleUserKickAck _v)
	{
		_v = new PktInfoCircleUserKickAck();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoCircleUserKickAck _v)
	{
		_v.Write(_s);
	}
}

// 서클 가입대기 유저 상태변경 메시지
public class PktInfoChangeStateJoinWaitUser : PktMsgType
{
	public ulong tagetUUID_;		// 변경대상 유저아이디 (가입대기 유저)
	public bool state_;            // 변경하고자하는 상태값 (0: 거절, 1: 수락)

	public override bool Read(Message _s)
	{
		if (false == _s.Read(out tagetUUID_)) return false;
		if (false == _s.Read(out state_)) return false;
		return true;
	}
	public override void Write(Message _s)
	{
		_s.Write(tagetUUID_);
		_s.Write(state_);
	}
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoChangeStateJoinWaitUser _v)
	{
		_v = new PktInfoChangeStateJoinWaitUser();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoChangeStateJoinWaitUser _v)
	{
		_v.Write(_s);
	}
}

// 서클 유저 권한변경 패킷 메시지
public class PktInfoCircleChangeAuthority : PktMsgType
{
	public class Piece : PktMsgType
	{
		public ulong uuid_;
		public eCircleAuthLevel authLevel_;

		public override bool Read(Message _s)
		{
			if (false == _s.Read(out uuid_)) return false;
			if (false == PN_MarshalerEx.Read(_s, out authLevel_)) return false;
			return true;
		}
		public override void Write(Message _s)
		{
			_s.Write(uuid_);
			PN_MarshalerEx.Write(_s, authLevel_);
		}
	}

	public Piece targetUser_;          // 클라이언트에서 채워줘야 할 값 (변경대상 유저)
	public Piece affectedUser_;            // 유저 권한 변경으로 영향받은(함께변경된) 유저

	public override bool Read(Message _s)
	{
		if (false == PN_MarshalerEx.Read(_s, out targetUser_)) return false;
		if (false == PN_MarshalerEx.Read(_s, out affectedUser_)) return false;
		return true;
	}
	public override void Write(Message _s)
	{
		PN_MarshalerEx.Write(_s, targetUser_);
		PN_MarshalerEx.Write(_s, affectedUser_);
	}
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoCircleChangeAuthority.Piece _v)
	{
		_v = new PktInfoCircleChangeAuthority.Piece();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoCircleChangeAuthority.Piece _v)
	{
		_v.Write(_s);
	}
	public static bool Read(Message _s, out PktInfoCircleChangeAuthority _v)
	{
		_v = new PktInfoCircleChangeAuthority();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoCircleChangeAuthority _v)
	{
		_v.Write(_s);
	}
}

// 서클명 변경 패킷메시지
public class PktInfoCircleChangeName : PktMsgType
{
	public PktInfoStr changeName_;
	public PktInfoGoods consume_;

	public override bool Read(Message _s)
	{
		if (false == PN_MarshalerEx.Read(_s, out changeName_)) return false;
		if (false == PN_MarshalerEx.Read(_s, out consume_)) return false;
		return true;
	}
	public override void Write(Message _s)
	{
		PN_MarshalerEx.Write(_s, changeName_);
		PN_MarshalerEx.Write(_s, consume_);
	}
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoCircleChangeName _v)
	{
		_v = new PktInfoCircleChangeName();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoCircleChangeName _v)
	{
		_v.Write(_s);
	}
}

// 서클명 출석체크 응답 메시지 (보상은 우편으로 지급)
public class PktInfoCircleAttendanceRwd : PktMsgType
{
	public PktInfoTime lastCircleAttendTM_;        // 마지막 서클 출석체크 시간
	public uint circleAttendGroupID_;      // 서클 출석체크 그룹 ID
	public byte circleAttendGroupCnt_;  // 현재 서클 출석체크 보상 카운트

	public override bool Read(Message _s)
	{
		if (false == PN_MarshalerEx.Read(_s, out lastCircleAttendTM_)) return false;
		if (false == _s.Read(out circleAttendGroupID_)) return false;
		if (false == _s.Read(out circleAttendGroupCnt_)) return false;
		return true;
	}
	public override void Write(Message _s)
	{
		PN_MarshalerEx.Write(_s, lastCircleAttendTM_);
		_s.Write(circleAttendGroupID_);
		_s.Write(circleAttendGroupCnt_);
	}
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoCircleAttendanceRwd _v)
	{
		_v = new PktInfoCircleAttendanceRwd();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoCircleAttendanceRwd _v)
	{
		_v.Write(_s);
	}
}

// 서클 마크관련 아이템 구매 응답 패킷 메시지
public class PktInfoCircleBuyMarkItem : PktMsgType
{
	public ulong[] oldGoods_ = new ulong[(int)eCircleGoodsType.COUNT];			// 구매전 서클 재화정보
	public ulong[] circleGoods_ = new ulong[(int)eCircleGoodsType.COUNT];		// 구매후 서클 재화정보
	public PktInfoCircleMark ownedMark_;			// 구매 완료 후 소유한 마크아이템 플래그 정보

	public override bool Read(Message _s)
	{
		for (int loop = 0; loop < (int)eCircleGoodsType.COUNT; ++loop)
			if (false == _s.Read(out oldGoods_[loop])) return false;
		for (int loop = 0; loop < (int)eCircleGoodsType.COUNT; ++loop)
			if (false == _s.Read(out circleGoods_[loop])) return false;
		if (false == PN_MarshalerEx.Read(_s, out ownedMark_)) return false;
		return true;
	}
	public override void Write(Message _s)
	{
		for (int loop = 0; loop < (int)eCircleGoodsType.COUNT; ++loop)
			_s.Write(oldGoods_[loop]);
		for (int loop = 0; loop < (int)eCircleGoodsType.COUNT; ++loop)
			_s.Write(circleGoods_[loop]);
		PN_MarshalerEx.Write(_s, ownedMark_);
	}
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoCircleBuyMarkItem _v)
	{
		_v = new PktInfoCircleBuyMarkItem();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoCircleBuyMarkItem _v)
	{
		_v.Write(_s);
	}
}

// 서클 소유한 마크리스트 조회 패킷 메시지
public class PktInfoGetCircleMarkList : PktMsgType
{
	public PktInfoCircleMark list_;
	public ulong[] goods_ = new ulong[(int)eCircleGoodsType.COUNT];

	public override bool Read(Message _s)
	{
		if (false == PN_MarshalerEx.Read(_s, out list_)) return false;
		for (int loop = 0; loop < (int)eCircleGoodsType.COUNT; ++loop)
			if (false == _s.Read(out goods_[loop])) return false;
		return true;
	}
	public override void Write(Message _s)
	{
		PN_MarshalerEx.Write(_s, list_);
		for (int loop = 0; loop < (int)eCircleGoodsType.COUNT; ++loop)
			_s.Write(goods_[loop]);
	}
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoGetCircleMarkList _v)
	{
		_v = new PktInfoGetCircleMarkList();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoGetCircleMarkList _v)
	{
		_v.Write(_s);
	}
}

// 서클 검색요청 메시지
public class PktInfoCircleSearch : PktMsgType
{
	public ulong circleID_;				// 검색할 고유 서클 아이디 (둘중하나를 꼭 채워야함)
	public PktInfoStr circleName_;		// 검색할 서클명

	public override bool Read(Message _s)
	{
		if (false == _s.Read(out circleID_)) return false;
		if (false == PN_MarshalerEx.Read(_s, out circleName_)) return false;
		return true;
	}
	public override void Write(Message _s)
	{
		_s.Write(circleID_);
		PN_MarshalerEx.Write(_s, circleName_);
	}
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoCircleSearch _v)
	{
		_v = new PktInfoCircleSearch();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoCircleSearch _v)
	{
		_v.Write(_s);
	}
}

// 서클 전체알림(채팅) 패킷 메시지
public class PktInfoCircleNotification : PktMsgType
{
	public class Piece : PktMsgType
	{
		public List<ulong> values_;				// 로컬라이징 포맷에 사용될 값 (Ex: 스테이지 {val_0} 의 타임어택 랭킹 상위 {val_1} 진입)

		public eCircleNotiType notiTp_;			// 알림메시지 타입
		public PktInfoStr nickName_;			// 대상 유저 이름
		public PktInfoTime sendTm_;				// 전송시간

		public override bool Read(Message _s)
		{
			if (false == PN_MarshalerEx.Read(_s, out notiTp_)) return false;
			if (false == PN_MarshalerEx.Read(_s, out nickName_)) return false;
			if (false == PN_MarshalerEx.Read(_s, out values_)) return false;
			if (false == PN_MarshalerEx.Read(_s, out sendTm_)) return false;
			return true;
		}
		public override void Write(Message _s)
		{
			PN_MarshalerEx.Write(_s, notiTp_);
			PN_MarshalerEx.Write(_s, nickName_);
			PN_MarshalerEx.Write(_s, values_);
			PN_MarshalerEx.Write(_s, sendTm_);
		}
	}

	public List<Piece> infos_;

	public override bool Read(Message _s)
	{
		if (false == PN_MarshalerEx.ReadList(_s, out infos_)) return false;
		return true;
	}
	public override void Write(Message _s)
	{
		PN_MarshalerEx.WriteList(_s, infos_);
	}
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoCircleNotification.Piece _v)
	{
		_v = new PktInfoCircleNotification.Piece();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoCircleNotification.Piece _v)
	{
		_v.Write(_s);
	}
	public static bool Read(Message _s, out PktInfoCircleNotification _v)
	{
		_v = new PktInfoCircleNotification();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoCircleNotification _v)
	{
		_v.Write(_s);
	}
}

// 서클 채팅리스트 패킷 메시지
public class PktInfoCircleChat : PktMsgType
{
	public class Piece : PktMsgType
	{
		public ulong uuid_;           // 유저 고유아이디
		public PktInfoStr nickName_;           // 유저 닉네임
		public uint mark_;             // 유저 마크 아이디
		public uint stampID_;          // 채팅 스탬프 아이디
		public PktInfoStr msg_;                // 채팅 메시지
		public PktInfoTime sendTm_;            // 채팅 전송시간

		public override bool Read(Message _s)
		{
			if (false == _s.Read(out uuid_)) return false;
			if (false == PN_MarshalerEx.Read(_s, out nickName_)) return false;
			if (false == _s.Read(out mark_)) return false;
			if (false == _s.Read(out stampID_)) return false;
			if (false == PN_MarshalerEx.Read(_s, out msg_)) return false;
			if (false == PN_MarshalerEx.Read(_s, out sendTm_)) return false;
			return true;
		}
		public override void Write(Message _s)
		{
			_s.Write(uuid_);
			PN_MarshalerEx.Write(_s, nickName_);
			_s.Write(mark_);
			_s.Write(stampID_);
			PN_MarshalerEx.Write(_s, msg_);
			PN_MarshalerEx.Write(_s, sendTm_);
		}
	}

	public List<Piece> infos_;
	public PktInfoCircleNotification notiMessage_;     // 서클(전체) 알림 메시지

	public override bool Read(Message _s)
	{
		if (false == PN_MarshalerEx.Read(_s, out notiMessage_)) return false;
		if (false == PN_MarshalerEx.ReadList(_s, out infos_)) return false;
		return true;
	}
	public override void Write(Message _s)
	{
		PN_MarshalerEx.Write(_s, notiMessage_);
		PN_MarshalerEx.WriteList(_s, infos_);
	}
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoCircleChat.Piece _v)
	{
		_v = new PktInfoCircleChat.Piece();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoCircleChat.Piece _v)
	{
		_v.Write(_s);
	}

	public static bool Read(Message _s, out PktInfoCircleChat _v)
	{
		_v = new PktInfoCircleChat();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoCircleChat _v)
	{
		_v.Write(_s);
	}
}

// 공용 커뮤니티 유저 정보 패킷 메시지
public class PktInfoComCommuUser : PktMsgType
{
    public class Piece : PktMsgType
    {
		public PktInfoStr nickName_;			// 유저 닉네임
		public uint nickNameColorID_;			// 유저 닉네임 색상 아이디
        public PktInfoTime lastConnTM_;         // 유저 마지막 접속 시간
        public System.UInt64 uuid_;             // 유저 유저 고유 ID
        public System.UInt32 mark_;             // 유저 최신 마크 ID
        public System.UInt32 arenaTWID_;        // 유저 최신 마크 ID
        public System.UInt16 dbID_;             // 유저 정보가 존재하는 DB ID
        public System.Byte rank_;               // 유저 랭크
        public System.Byte roomSlotNum_;        // 유저 메인 룸 슬롯 번호
		public PktInfoCircleSimple.Piece circleInfo_; // 소속된 서클의 간소화된 정보
		public PktInfoCircleAuthority circleAuthInfo_; // 소속된 서클의 권한정보

        public override bool Read(Message _s) {
            if (false == PN_MarshalerEx.Read(_s, out nickName_)) return false;
			if (false == _s.Read(out nickNameColorID_)) return false;
			if (false == PN_MarshalerEx.Read(_s, out lastConnTM_)) return false;
            if (false == _s.Read(out uuid_)) return false;
            if (false == _s.Read(out mark_)) return false;
            if (false == _s.Read(out arenaTWID_)) return false;
            if (false == _s.Read(out dbID_)) return false;
            if (false == _s.Read(out rank_)) return false;
            if (false == _s.Read(out roomSlotNum_)) return false;
			if (false == PN_MarshalerEx.Read(_s, out circleAuthInfo_)) return false;
			if (false == PN_MarshalerEx.Read(_s, out circleInfo_)) return false;
			return true;
        }
        public override void Write(Message _s) {
			PN_MarshalerEx.Write(_s, nickName_);
			_s.Write(nickNameColorID_);
			PN_MarshalerEx.Write(_s, lastConnTM_);
            _s.Write(uuid_);
            _s.Write(mark_);
            _s.Write(arenaTWID_);
            _s.Write(dbID_);
            _s.Write(rank_);
            _s.Write(roomSlotNum_);
			PN_MarshalerEx.Write(_s, circleAuthInfo_);
			PN_MarshalerEx.Write(_s, circleInfo_);
		}
    };
    public List<Piece> infos_;
};
// 친구 정보 패킷 메시지
public class PktInfoFriend : PktMsgType
{
    enum FLAG : System.Byte {
        _CAN_TAKE_PT    = 0,        // 우정 포인트 받기 가능(0:불가능, 1:받기 가능)
        _CHK_CTRL_IN_ROOM_TO_TGT,   // 내 친구 목록에서 대상 룸 입장 여부 조작 확인용(0:룸 입장 가능, 1:불가능)
        _CAN_ME_IN_ROOM,            // 내가 해당 친구 룸 입장 가능 한지 여부(0:룸 입장 가능, 1:불가능)

        _MAX_,
    };
    public class Piece : PktMsgType
    {
        public PktInfoComCommuUser.Piece info_; // 공용 커뮤니티 유저 정보
        public System.UInt32 flag_;             // 친구 기능 상태 프레그 값
        public System.UInt16 callCnt_;          // 호출 횟수
        public System.Boolean arena_;           // 아레나 정보가 있는지 여부

        public override bool Read(Message _s) {
            if (false == PN_MarshalerEx.Read(_s, out info_)) return false;
            if (false == _s.Read(out flag_)) return false;
            if (false == _s.Read(out callCnt_)) return false;
            if (false == _s.Read(out arena_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            PN_MarshalerEx.Write(_s, info_);
            _s.Write(flag_);
            _s.Write(callCnt_);
            _s.Write(arena_);
        }
        // 해당 미션을 클리어 했는지 확인합니다.
        public bool IsOnFlag(int _flagIdx /*PktInfoFriend.Piece.FLAG*/)
        {
            if (32 <= _flagIdx) return false;
            return _IsOnBitIdx(flag_, (uint)_flagIdx);
        }
        // 해당 미션을 클리어했다는 값을 활성화합니다.
        public void DoOnFlag(int _flagIdx /*PktInfoFriend.Piece.FLAG*/)
        {
            if (32 <= _flagIdx) return;
            _DoOnBitIdx(ref flag_, (uint)_flagIdx);
        }
    };
    public List<Piece> infos_;
	public PktInfoTime lastUpdateTM_;      // 마지막으로 갱신된 시간

	public override bool Read(Message _s)
	{
		if (false == PN_MarshalerEx.ReadList(_s, out infos_)) return false;
		if (false == PN_MarshalerEx.Read(_s, out lastUpdateTM_)) return false;
		return true;
	}

	public override void Write(Message _s)
	{
		PN_MarshalerEx.WriteList(_s, infos_);
		PN_MarshalerEx.Write(_s, lastUpdateTM_);
	}
}// 커뮤니티 관련 정보 패킷 메시지
public class PktInfoCommunity : PktMsgType
{
    public PktInfoFriend friends_;          // 친구 목록
    public PktInfoFriend friToAsk_;         // 내가 유저에게 보낸 친구 신청 목록
    public PktInfoFriend friAskFromUser_;   // 유저로 부터 나에게 온 친구 신청 목록
    public System.UInt64 uuid_;             // 유저 고유 ID

	public PktInfoCircleSimple.Piece circleSimple_;   // 소속된 길드의 기본정보
};
// 커뮤니티 유저 아레나 정보 요청 패킷 메시지
public class PktInfoCommuUserArenaInfoReq : PktMsgType
{
    public PktInfoUIDList uids_;            // 정보 획득을 요청할 유저 고유 ID 목록 - (클라이언트에서 채워줘야 할 값)
    public System.UInt64 reqUuid_;          // 요청 유저 고유 ID
};
// 커뮤니티 유저 아레나 정보 응답 패킷 메시지
public class PktInfoCommuUserArenaInfoAck : PktMsgType
{
    public List<PktInfoArenaDetail> infos_;  // 요정한 유저 아레나 정보 목록
};
// 커뮤니티 유저 호출 횟수 사용 요청 패킷 메시지
public class PktInfoCommuUseCallCntReq : PktMsgType
{
    public PktInfoUIDList uids_;            // 호출 횟수 사용을 요청할 유저 고유 ID 목록 - (클라이언트에서 채워줘야 할 값)
    public System.UInt64 reqUuid_;          // 요청 유저 고유 ID
};
// 커뮤니티 유저 호출 횟수 사용 응답 패킷 메시지
public class PktInfoCommuUseCallCntAck : PktMsgType
{
    public System.UInt64 userFriPoint_;     // 호출 횟수 사용을 통해 획득한 수치를 포함한 최종 유저 친구 포인트
};
// 커뮤니티 관련 공용 회출 횟수 변경 알림 패킷 메시지
public class PktInfoCommuCallCntNoti : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt64 uuid_;         // 변경 대상 유저 고유 ID
        public System.UInt16 callCnt_;      // 변경된 내용이 적용된 현재 호출 횟수

        public override bool Read(Message _s) {
            if (false == _s.Read(out uuid_)) return false;
            if (false == _s.Read(out callCnt_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(uuid_);
            _s.Write(callCnt_);
        }
    };
    public List<Piece> infos_;
};
// 커뮤니티 유저 아레나 타워 클리어 ID 설정 알림 패킷 메시지
public class PktInfoCommuArenaTowerIDNoti : PktMsgType
{
    public System.UInt64 tgtUuid_;          // 대상 유저 고유 ID
    public System.UInt32 arenaTWID_;        // 아레나 타워 ID
};
// 커뮤니티 추천 유저 요청 패킷 메시지
public class PktInfoCommuSuggestReq : PktMsgType
{
    //안채우면 추천 리스트 - 채워보내면 검색한 유저만
    public PktInfoUIDList sugUids_;         // 검색 대상 유저 고유 ID 목록 - (클라이언트에서 채워줘야 할 값)
    public System.UInt64 reqUuid_;          // 요청 유저 고유 ID
};
// 커뮤니티 추천 유저 요청 패킷 메시지
public class PktInfoCommuSuggestAck : PktMsgType
{
    public List<PktInfoCommunity> commuInfos_;  // 커뮤니티 유저 정보 - (!!서버 전용 클라이언트 신경쓰지 않아도 됨~~)
    public PktInfoComCommuUser suggest_;    // 검색 대상 유저 정보 목록
};
// 커뮤니티 관련 공용 신청 요청 패킷 메시지
public class PktInfoCommuAskReq : PktMsgType
{
    public PktInfoUIDList tgtUids_;         // 커뮤니티 관련 신청 대상 유저 고유 ID 목록 - (클라이언트에서 채워줘야 할 값)
    public System.UInt64 reqUuid_;          // 검색 대상 유저 정보 목록
};
// 커뮤니티 관련 공용 신청 취소 패킷 메시지
public class PktInfoCommuAskDel : PktMsgType
{
    public PktInfoUIDList tgtUids_;         // 커뮤니티 관련 신청 취소 대상 유저 고유 ID 목록 - (클라이언트에서 채워줘야 할 값)
    public System.UInt64 reqUuid_;          // 요청 유저 고유 ID
};
// 커뮤니티 관련 공용 승인 대한 답변 패킷 메시지
public class PktInfoCommuAnswer : PktMsgType
{
    public PktInfoUIDList tgtUids_;         // 응답할 신청 유저 ID 목록 - (클라이언트에서 채워줘야 할 값)
    public System.UInt64 reqUuid_;          // 요청 유저 고유 ID
    public System.Boolean accept_;          // 승락 여부 (true:승락, false:거절) - (클라이언트에서 채워줘야 할 값)
};
// 커뮤니티 관련 공용 승인 답변 알림 패킷 메시지
public class PktInfoCommuAnswerNoti : PktMsgType
{
    public System.UInt64 uuidInAsk_;        // 신청 목록에서 변동이 생긴 유저 고유 ID
    public System.Boolean accept_;          // 승락 여부 (true:승락, false:거절)
};
// 커뮤니티 관련 공용 유저 제거 패킷 메시지
public class PktInfoCommuKick : PktMsgType
{
    public PktInfoUIDList tgtUids_;         // 제거 대상 유저 ID 목록 - (클라이언트에서 채워줘야 할 값)
    public System.UInt64 reqUuid_;          // 요청 유저 고유 ID
};
// 커뮤니티 관련 공용 유저 제거 알림 패킷 메시지
public class PktInfoCommuKickNoti : PktMsgType
{
    public System.UInt64 reqUuid_;          // 제거 대상 유저 고유 ID
};
// 커뮤니티 프라이빗룸 정보 획득 패킷 메시지
public class PktInfoCommuRoomInfoGet : PktMsgType
{
    public System.UInt64 tgtUuid_;          // 룸 정보를 얻어올 대상 유저 고유 ID - (클라이언트에서 채워줘야 할 값)
    public System.UInt64 reqUuid_;          // 요청을 보낸 유저 고유 ID
    public System.UInt64 dbID_;             // 롬정보가 있는 유저의 DB ID
    public System.Byte roomSlotNum_;        // 해당 룸 슬롯 번호
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 공용 커뮤니티 유저 정보 패킷 메시지
    public static bool Read(Message _s, out PktInfoComCommuUser.Piece _v) {
        _v = new PktInfoComCommuUser.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoComCommuUser.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoComCommuUser _v) {
        _v = new PktInfoComCommuUser();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoComCommuUser _v) {
        WriteList(_s, _v.infos_);
    }
    // 친구 정보 패킷 메시지
    public static bool Read(Message _s, out PktInfoFriend.Piece _v) {
        _v = new PktInfoFriend.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoFriend.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoFriend _v) {
        _v = new PktInfoFriend();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoFriend _v) {
        _v.Write(_s);
    }
    // 커뮤니티 관련 정보 패킷 메시지
    public static bool Read(Message _s, out PktInfoCommunity _v) {
        _v = new PktInfoCommunity();
        if (false == Read(_s, out _v.friends_)) return false;
        if (false == Read(_s, out _v.friToAsk_)) return false;
        if (false == Read(_s, out _v.friAskFromUser_)) return false;
        if (false == _s.Read(out _v.uuid_)) return false;
        if (false == Read(_s, out _v.circleSimple_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCommunity _v) {
        Write(_s, _v.friends_);
        Write(_s, _v.friToAsk_);
        Write(_s, _v.friAskFromUser_);
        _s.Write(_v.uuid_);
        Write(_s, _v.circleSimple_);
    }
    // 커뮤니티 유저 아레나 정보 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoCommuUserArenaInfoReq _v) {
        _v = new PktInfoCommuUserArenaInfoReq();
        if (false == Read(_s, out _v.uids_)) return false;
        if (false == _s.Read(out _v.reqUuid_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCommuUserArenaInfoReq _v) {
        Write(_s, _v.uids_);
        _s.Write(_v.reqUuid_);
    }
    // 커뮤니티 유저 아레나 정보 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoCommuUserArenaInfoAck _v) {
        _v = new PktInfoCommuUserArenaInfoAck();
        if (false == ReadList(_s, out _v.infos_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCommuUserArenaInfoAck _v) {
        WriteList(_s, _v.infos_);
    }
    // 커뮤니티 유저 호출 횟수 사용 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoCommuUseCallCntReq _v) {
        _v = new PktInfoCommuUseCallCntReq();
        if (false == Read(_s, out _v.uids_)) return false;
        if (false == _s.Read(out _v.reqUuid_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCommuUseCallCntReq _v) {
        Write(_s, _v.uids_);
        _s.Write(_v.reqUuid_);
    }
    // 커뮤니티 유저 호출 횟수 사용 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoCommuUseCallCntAck _v) {
        _v = new PktInfoCommuUseCallCntAck();
        if (false == _s.Read(out _v.userFriPoint_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCommuUseCallCntAck _v) {
        _s.Write(_v.userFriPoint_);
    }
    // 커뮤니티 관련 공용 회출 횟수 변경 알림 패킷 메시지
    public static bool Read(Message _s, out PktInfoCommuCallCntNoti _v) {
        _v = new PktInfoCommuCallCntNoti();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoCommuCallCntNoti _v) {
        WriteList(_s, _v.infos_);
    }
    // 커뮤니티 유저 호출 횟수 사용 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoCommuArenaTowerIDNoti _v) {
        _v = new PktInfoCommuArenaTowerIDNoti();
        if (false == _s.Read(out _v.tgtUuid_)) return false;
        if (false == _s.Read(out _v.arenaTWID_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCommuArenaTowerIDNoti _v) {
        _s.Write(_v.tgtUuid_);
        _s.Write(_v.arenaTWID_);
    }
    // 커뮤니티 추천 유저 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoCommuSuggestReq _v) {
        _v = new PktInfoCommuSuggestReq();
        if (false == Read(_s, out _v.sugUids_)) return false;
        if (false == _s.Read(out _v.reqUuid_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCommuSuggestReq _v) {
        Write(_s, _v.sugUids_);
        _s.Write(_v.reqUuid_);
    }
    // 커뮤니티 추천 유저 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoCommuSuggestAck _v) {
        _v = new PktInfoCommuSuggestAck();
        if (false == ReadList(_s, out _v.commuInfos_)) return false;
        if (false == Read(_s, out _v.suggest_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCommuSuggestAck _v) {
        WriteList(_s, _v.commuInfos_);
        Write(_s, _v.suggest_);
    }
    // 커뮤니티 관련 공용 신청 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoCommuAskReq _v) {
        _v = new PktInfoCommuAskReq();
        if (false == Read(_s, out _v.tgtUids_)) return false;
        if (false == _s.Read(out _v.reqUuid_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCommuAskReq _v) {
        Write(_s, _v.tgtUids_);
        _s.Write(_v.reqUuid_);
    }
    // 커뮤니티 관련 공용 신청 취소 패킷 메시지
    public static bool Read(Message _s, out PktInfoCommuAskDel _v) {
        _v = new PktInfoCommuAskDel();
        if (false == Read(_s, out _v.tgtUids_)) return false;
        if (false == _s.Read(out _v.reqUuid_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCommuAskDel _v) {
        Write(_s, _v.tgtUids_);
        _s.Write(_v.reqUuid_);
    }
    // 커뮤니티 관련 공용 승인 대한 답변 패킷 메시지
    public static bool Read(Message _s, out PktInfoCommuAnswer _v) {
        _v = new PktInfoCommuAnswer();
        if (false == Read(_s, out _v.tgtUids_)) return false;
        if (false == _s.Read(out _v.reqUuid_)) return false;
        if (false == _s.Read(out _v.accept_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCommuAnswer _v) {
        Write(_s, _v.tgtUids_);
        _s.Write(_v.reqUuid_);
        _s.Write(_v.accept_);
    }
    // 커뮤니티 관련 공용 승인 답변 알림 패킷 메 시지
    public static bool Read(Message _s, out PktInfoCommuAnswerNoti _v) {
        _v = new PktInfoCommuAnswerNoti();
        if (false == _s.Read(out _v.uuidInAsk_)) return false;
        if (false == _s.Read(out _v.accept_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCommuAnswerNoti _v) {
        _s.Write(_v.uuidInAsk_);
        _s.Write(_v.accept_);
    }
    // 커뮤니티 관련 공용 유저 제거 패킷 메시지
    public static bool Read(Message _s, out PktInfoCommuKick _v) {
        _v = new PktInfoCommuKick();
        if (false == Read(_s, out _v.tgtUids_)) return false;
        if (false == _s.Read(out _v.reqUuid_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCommuKick _v) {
        Write(_s, _v.tgtUids_);
        _s.Write(_v.reqUuid_);
    }
    // 커뮤니티 관련 공용 유저 제거 알림 패킷 메시지
    public static bool Read(Message _s, out PktInfoCommuKickNoti _v) {
        _v = new PktInfoCommuKickNoti();
        if (false == _s.Read(out _v.reqUuid_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCommuKickNoti _v) {
        _s.Write(_v.reqUuid_);
    }
    // 커뮤니티 프라이빗룸 정보 획득 패킷 메시지
    public static bool Read(Message _s, out PktInfoCommuRoomInfoGet _v) {
        _v = new PktInfoCommuRoomInfoGet();
        if (false == _s.Read(out _v.tgtUuid_)) return false;
        if (false == _s.Read(out _v.reqUuid_)) return false;
        if (false == _s.Read(out _v.dbID_)) return false;
        if (false == _s.Read(out _v.roomSlotNum_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCommuRoomInfoGet _v) {
        _s.Write(_v.tgtUuid_);
        _s.Write(_v.reqUuid_);
        _s.Write(_v.dbID_);
        _s.Write(_v.roomSlotNum_);
    }
}
///
///////////////////////////////////////////////////////////////////////





///////////////////////////////////////////////////////////////////////
///
// 친구 신청 정보 패킷 메시지
public class PktInfoFriendAsk : PktMsgType
{
    public PktInfoFriend addAsk_;           // 친구 신청 대상 유저 정보 목록
}
// 친구 포인트 전달 패킷 메시지
public class PktInfoFriendPointGive : PktMsgType
{
    public PktInfoTime nextFriPTGiveTM_;    // 다음 포인트 전달 가능 시간
    public System.UInt64 userFriPoint_;     // 포인트 전달을 통해 얻어진 친구 포인트를 포함한 최종 유저 친구 포인트
    public System.Byte nowFriCnt_;          // 현재 친구 수
}
// 친구 포인트 받기 요청 패킷 메시지
public class PktInfoFriendPointTakeReq : PktMsgType
{
    public PktInfoUIDList tgtUids_;         // 포인트 받을 유저 고유 ID 목록 - (클라이언트에서 채워줘야 할 값)
    public System.UInt64 reqUuid_;          // 요청을 보낸 유저 고유 ID
}
// 친구 포인트 받기 응답 패킷 메시지
public class PktInfoFriendPointTakeAck : PktMsgType
{
    public System.UInt64 userFriPoint_;     // 포인트 받기를 통해 획득한 수치를 포함한 최종 유저 친구 포인트
}
// 친구 프라이빗룸 입장 가능 상태 변경 요청 패킷 메시지
public class PktInfoFriendRoomFlag : PktMsgType
{
    public PktInfoUIDList tgtUids_;         // 입장 가능 상태 변경 대상 유저 고유 ID - (클라이언트에서 채워줘야 할 값)
    public System.UInt64 reqUuid_;          // 요청을 보낸 유저 고유 ID
    public System.Boolean accept_;          // 입장 허용 여부 (true:허용, false:불가) - (클라이언트에서 채워줘야 할 값)
}
// 친구 플레그 변경 알림 패킷 메시지
public class PktInfoFriendFlagUpdateNoti : PktMsgType
{
    public PktInfoFlagUpdate flag_;         // 플레그 변경 정보
    public System.UInt64 reqUuid_;          // 요청을 보낸 유저 고유 ID
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 친구 신청 정보 패킷 메시지
    public static bool Read(Message _s, out PktInfoFriendAsk _v) {
        _v = new PktInfoFriendAsk();
        if (false == Read(_s, out _v.addAsk_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoFriendAsk _v) {
        Write(_s, _v.addAsk_);
    }
    // 친구 포인트 전달 패킷 메시지
    public static bool Read(Message _s, out PktInfoFriendPointGive _v) {
        _v = new PktInfoFriendPointGive();
        if (false == Read(_s, out _v.nextFriPTGiveTM_)) return false;
        if (false == _s.Read(out _v.userFriPoint_)) return false;
        if (false == _s.Read(out _v.nowFriCnt_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoFriendPointGive _v) {
        Write(_s, _v.nextFriPTGiveTM_);
        _s.Write(_v.userFriPoint_);
        _s.Write(_v.nowFriCnt_);
    }
    // 친구 포인트 받기 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoFriendPointTakeReq _v) {
        _v = new PktInfoFriendPointTakeReq();
        if (false == Read(_s, out _v.tgtUids_)) return false;
        if (false == _s.Read(out _v.reqUuid_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoFriendPointTakeReq _v) {
        Write(_s, _v.tgtUids_);
        _s.Write(_v.reqUuid_);
    }
    // 친구 포인트 받기 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoFriendPointTakeAck _v) {
        _v = new PktInfoFriendPointTakeAck();
        if (false == _s.Read(out _v.userFriPoint_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoFriendPointTakeAck _v) {
        _s.Write(_v.userFriPoint_);
    }
    // 친구 프라이빗룸 입장 가능 상태 변경 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoFriendRoomFlag _v) {
        _v = new PktInfoFriendRoomFlag();
        if (false == Read(_s, out _v.tgtUids_)) return false;
        if (false == _s.Read(out _v.reqUuid_)) return false;
        if (false == _s.Read(out _v.accept_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoFriendRoomFlag _v) {
        Write(_s, _v.tgtUids_);
        _s.Write(_v.reqUuid_);
        _s.Write(_v.accept_);
    }
    // 친구 플레그 변경 알림 패킷 메시지
    public static bool Read(Message _s, out PktInfoFriendFlagUpdateNoti _v) {
        _v = new PktInfoFriendFlagUpdateNoti();
        if (false == Read(_s, out _v.flag_)) return false;
        if (false == _s.Read(out _v.reqUuid_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoFriendFlagUpdateNoti _v) {
        Write(_s, _v.flag_);
        _s.Write(_v.reqUuid_);
    }
}
///
///////////////////////////////////////////////////////////////////////