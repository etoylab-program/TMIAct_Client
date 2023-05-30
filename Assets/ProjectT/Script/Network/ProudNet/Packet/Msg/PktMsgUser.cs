using System.Collections.Generic;
using Nettention.Proud;


// 연결 정보 패킷 메시지
public class PktInfoAccountLinkInfo : PktMsgType
{
    public PktInfoStr linkID_;                  // 이어하기를 진행할 연결 ID - (클라이언트에서 채워줘야 할 값)
    public eAccountType type_;                  // 걔정 타입 - (클라이언트에서 채워줘야 할 값)
    public System.Boolean updateFlag_;          // 갱신 여부(false: 갱신 안되고 기존에 없을때만 적용 true: 기존에 정보가 있어도 갱신 가능함.) - (클라이언트에서 채워줘야 할 값)
    public override bool Read(Message _s) {
        if (false == PN_MarshalerEx.Read(_s, out linkID_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out type_)) return false;
        if (false == _s.Read(out updateFlag_)) return false;
        return true;
    }
    public override void Write(Message _s) {
        PN_MarshalerEx.Write(_s, linkID_);
        PN_MarshalerEx.Write(_s, type_);
        _s.Write(updateFlag_);
    }
}
// 연결 정보 목록 패킷 메시지
public class PktInfoAccountLinkList : PktMsgType
{
    public List<PktInfoAccountLinkInfo> infos_; // 연결 정보 목록
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 연결 정보 패킷 메시지
    public static bool Read(Message _s, out PktInfoAccountLinkInfo _v) {
        _v = new PktInfoAccountLinkInfo();
        if (false == _v.Read(_s)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoAccountLinkInfo _v) {
        _v.Write(_s);
    }
    // 연결 정보 목록 패킷 메시지
    public static bool Read(Message _s, out PktInfoAccountLinkList _v) {
        _v = new PktInfoAccountLinkList();
        if (false == ReadList(_s, out _v.infos_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoAccountLinkList _v) {
        WriteList(_s, _v.infos_);
    }
}

// 유저 국가 및 언어 코드 패킷 메시지
public class PktInfoCountryLangCode : PktMsgType
{
    public PktInfoStr country_;                 // 국가 코드 - (클라이언트에서 채워줘야 할 값)
    public PktInfoStr lang_;                    // 언어 코드 - (클라이언트에서 채워줘야 할 값)
    public override bool Read(Message _s) {
        if (false == PN_MarshalerEx.Read(_s, out country_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out lang_)) return false;
        return true;
    }
    public override void Write(Message _s) {
        PN_MarshalerEx.Write(_s, country_);
        PN_MarshalerEx.Write(_s, lang_);
    }
}
    public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 유저 국가 및 언어 코드 패킷 메시지
    public static bool Read(Message _s, out PktInfoCountryLangCode _v) {
        _v = new PktInfoCountryLangCode();
        if (false == _v.Read(_s)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCountryLangCode _v) {
        _v.Write(_s);
    }
}

//  유저 정보
public class PktInfoUser : PktMsgType
{
    public System.String nickName_;         // 이름
    public System.String comment_;          // 인사말
	public uint nickNameColorID_;			// 이름색상 아이디
    public PktInfoCountryLangCode coLangCode_;  // 국가 및 언어 코드
    public PktInfoTime ticketRemainTM_;     // 티켓 다시 리셋 시간
    public PktInfoTime BPNextTM_;           // BP 다시 리셋 시간
    public PktInfoTime lastLoginBonusTM_;   // 마지막으로 로그인 보너스 획득 시간
    public PktInfoTime nextFriPTGiveTM_;    // 친구 포인트 전달 가능 시간
	public PktInfoTime lastCircleAttendTM_;  // 마지막 서클 출석체크 시간
	public PktInfoExpLv expLv_;             // 경험치 레벨
	public System.UInt64 uuid_;             // 유니크 ID
    public System.UInt64 hardCash_;         // 서브 캐쉬(이벤트나 기타로 획득되는 캐쉬)
    public System.UInt64 tutoFlag_;         // 튜토리얼 플래그
    public System.UInt32 tutoValue_;        // 튜토리얼 값
    public System.UInt32 roomSlotNum_;      // 메인 룸 테마 슬롯 번호
    public System.UInt32 loginGroupID_;     // 로그인 보너스 그룹 ID
    public System.UInt32 markID_;           // 지휘관 마크 ID
    public System.UInt32 lobbyThemeID_;     // 로비 테마 ID
    public System.UInt32 cardFrmtID_;       // 카드(서포터) 메인 진형 ID
	public uint circleAttendGroupID_;		// 서클 출석체크 그룹 ID
	public System.UInt16 totalLoginCnt_;    // 총 로그인 일수
	public System.UInt16 continueLoginCnt_; // 연속 로그인 일수
    public System.UInt16 itemSlotCnt_;      // 인벤토리 공간 수
    public System.Byte badgeSlotCnt_;       // 획득 가능한 문양 슬롯 수
    public System.Byte loginGroupCnt_;      // 현재 로그인 보너스 획득 일시
    public System.Byte pkgShow_;            // 키지를 보여줘야 하는지 여부(0 : 안보여줌, 1 : 보여줌)
	public byte circleAttendGroupCnt_;				// 현재 서클 출석체크 보상 카운트
	public System.Boolean accountCodeReward_;// 이어하기코드 보상 상태
	public System.Boolean accountLinkReward_;// 계정연동 보상 상태
    public System.UInt32 dyeingCostumeID_;   // 염색중인 코스튬 아이디
	public PktInfoBlackList blackLisInfo_;  //	유저 블랙리스트 정보
	public PktInfoCircleAuthority circleAuthInfo_; // 소속 서클의 권한 정보
	public PktInfoTime circleJoinPossibleDateNum_; // 서클에 가입 가능한 날짜
	public ushort nowLgnBonusMonthlyCnt_;			// 300일 출석 로그인 일수

}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoUser _v) {
        _v = new PktInfoUser();
        if (false == _s.Read(out _v.nickName_)) return false;
		if (false == _s.Read(out _v.nickNameColorID_)) return false;
		if (false == _s.Read(out _v.comment_)) return false;
        if (false == Read(_s, out _v.coLangCode_)) return false;
        if (false == Read(_s, out _v.ticketRemainTM_)) return false;
        if (false == Read(_s, out _v.BPNextTM_)) return false;
        if (false == Read(_s, out _v.lastLoginBonusTM_)) return false;
        if (false == Read(_s, out _v.nextFriPTGiveTM_)) return false;
        if (false == Read(_s, out _v.expLv_)) return false;
        if (false == _s.Read(out _v.uuid_)) return false;
        if (false == _s.Read(out _v.hardCash_)) return false;
        if (false == _s.Read(out _v.tutoValue_)) return false;
        if (false == _s.Read(out _v.tutoFlag_)) return false;
        if (false == _s.Read(out _v.roomSlotNum_)) return false;
        if (false == _s.Read(out _v.loginGroupID_)) return false;
        if (false == _s.Read(out _v.markID_)) return false;
        if (false == _s.Read(out _v.lobbyThemeID_)) return false;
        if (false == _s.Read(out _v.cardFrmtID_)) return false;
        if (false == _s.Read(out _v.totalLoginCnt_)) return false;
        if (false == _s.Read(out _v.continueLoginCnt_)) return false;
        if (false == _s.Read(out _v.itemSlotCnt_)) return false;
        if (false == _s.Read(out _v.badgeSlotCnt_)) return false;
        if (false == _s.Read(out _v.loginGroupCnt_)) return false;
        if (false == _s.Read(out _v.pkgShow_)) return false;
        if (false == _s.Read(out _v.accountCodeReward_)) return false;
        if (false == _s.Read(out _v.accountLinkReward_)) return false;
        if (false == _s.Read(out _v.dyeingCostumeID_)) return false;
		if (false == Read(_s, out _v.blackLisInfo_)) return false;
		if (false == Read(_s, out _v.circleAuthInfo_)) return false;
		if (false == Read(_s, out _v.circleJoinPossibleDateNum_)) return false;
		if (false == Read(_s, out _v.lastCircleAttendTM_)) return false;
        if (false == _s.Read(out _v.circleAttendGroupID_)) return false;
        if (false == _s.Read(out _v.circleAttendGroupCnt_)) return false;
        if (false == _s.Read(out _v.nowLgnBonusMonthlyCnt_)) return false;
		return true;
    }
    public static void Write(Message _s, PktInfoUser _v) {
        _s.Write(_v.nickName_);
		_s.Write(_v.nickNameColorID_);
		_s.Write(_v.comment_);
        Write(_s, _v.coLangCode_);
        Write(_s, _v.ticketRemainTM_);
        Write(_s, _v.BPNextTM_);
        Write(_s, _v.lastLoginBonusTM_);
        Write(_s, _v.nextFriPTGiveTM_);
        Write(_s, _v.expLv_);
        _s.Write(_v.uuid_);
        _s.Write(_v.hardCash_);
        _s.Write(_v.tutoValue_);
        _s.Write(_v.tutoFlag_);
        _s.Write(_v.roomSlotNum_);
        _s.Write(_v.loginGroupID_);
        _s.Write(_v.markID_);
        _s.Write(_v.lobbyThemeID_);
        _s.Write(_v.cardFrmtID_);
        _s.Write(_v.totalLoginCnt_);
        _s.Write(_v.continueLoginCnt_);
        _s.Write(_v.itemSlotCnt_);
        _s.Write(_v.badgeSlotCnt_);
        _s.Write(_v.loginGroupCnt_);
        _s.Write(_v.pkgShow_);
        _s.Write(_v.accountCodeReward_);
        _s.Write(_v.accountLinkReward_);
        _s.Write(_v.dyeingCostumeID_);
		Write(_s, _v.blackLisInfo_);
		Write(_s, _v.circleAuthInfo_);
		Write(_s, _v.circleJoinPossibleDateNum_);
		Write(_s, _v.lastCircleAttendTM_);
        _s.Write(_v.circleAttendGroupID_);
        _s.Write(_v.circleAttendGroupCnt_);
        _s.Write(_v.nowLgnBonusMonthlyCnt_);
    }
}

// 유저의 서버 이전 정보
public class PktInfoRelocateUser : PktMsgType
{
    public PktInfoStr accountID_;           // 계정 ID - (클라이언트에서 채워줘야 할 값)
    public PktInfoStr id_;                  // 이전 ID - (클라이언트에서 채워줘야 할 값)
    public PktInfoStr pw_;                  // 이전 비밀번호 - (클라이언트에서 채워줘야 할 값)
    public System.UInt64 uuid_;             // 유저 고유 ID
    public System.UInt32 reserveCnt_;       // 현재 이전 신청한 유저의 수
    public System.Byte complete_;           // 이전 완료 값(0: 이전 미완료, 1: 이전 완료)
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoRelocateUser _v) {
        _v = new PktInfoRelocateUser();
        if (false == Read(_s, out _v.accountID_)) return false;
        if (false == Read(_s, out _v.id_)) return false;
        if (false == Read(_s, out _v.pw_)) return false;
        if (false == _s.Read(out _v.uuid_)) return false;
        if (false == _s.Read(out _v.reserveCnt_)) return false;
        if (false == _s.Read(out _v.complete_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoRelocateUser _v) {
        Write(_s, _v.accountID_);
        Write(_s, _v.id_);
        Write(_s, _v.pw_);
        _s.Write(_v.uuid_);
        _s.Write(_v.reserveCnt_);
        _s.Write(_v.complete_);
    }
}

// 유저의 특정 정보의 현재 값에 대한 패킷 메시지
public class PktInfoRefreahUserInfo : PktMsgType
{
    public PktInfoGoodsAll nowGoods_;       // 현재 유저 재화 정보
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoRefreahUserInfo _v) {
        _v = new PktInfoRefreahUserInfo();
        if (false == Read(_s, out _v.nowGoods_)) return false; 
        return true;
    }
    public static void Write(Message _s, PktInfoRefreahUserInfo _v) {
        Write(_s, _v.nowGoods_);
    }
}

// 재접속 유저 정보 패킷 메시지
public class PktInfoReconnectUserInfoReq : PktMsgType
{
    public PktInfoVersion ver_;             // 버전 정보 - (클라이언트에서 채워줘야 할 값)
    public PktInfoStr accountID_;           // 계정 ID
    public System.UInt64 uuid_;             // 유저 고유 ID
}
public class PktInfoReconnectUserInfoAck : PktMsgType
{
    public PktInfoGoodsAll nowGoods_;       // 현재 유저 재화 정보
    public System.UInt64 uuid_;             // 유저 고유 ID
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoReconnectUserInfoReq _v) {
        _v = new PktInfoReconnectUserInfoReq();
        if (false == Read(_s, out _v.ver_)) return false;
        if (false == Read(_s, out _v.accountID_)) return false;
        if (false == _s.Read(out _v.uuid_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoReconnectUserInfoReq _v) {
        Write(_s, _v.ver_);
        Write(_s, _v.accountID_);
        _s.Write(_v.uuid_);
    }
    public static bool Read(Message _s, out PktInfoReconnectUserInfoAck _v) {
        _v = new PktInfoReconnectUserInfoAck();
        if (false == Read(_s, out _v.nowGoods_)) return false; 
        if (false == _s.Read(out _v.uuid_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoReconnectUserInfoAck _v) {
        Write(_s, _v.nowGoods_);
        _s.Write(_v.uuid_);
    }
}

// 푸시 토큰 설정 패킷 메시지
public class PktInfoPushNotiSetToken : PktMsgType
{
    public PktInfoStr token_;               // 푸쉬 알람을 받을 토큰 정보
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoPushNotiSetToken _v) {
        _v = new PktInfoPushNotiSetToken();
        if (false == Read(_s, out _v.token_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoPushNotiSetToken _v) {
        Write(_s, _v.token_);
    }
}

// 유저 티켓 갱신시간 변경및 티켓 추가 알림
public class PktInfoUpdateTicketUserNoti : PktMsgType
{
    public PktInfoTime nextTime_;           // 다음번 티켓 갱신 시간
    public System.UInt64 resultTicket_;     // 티켓 갱신 결과가 적용된 현재 티켓의 수
    public eGOODSTYPE ticketType_;          // 티켓 종류 (일반 티켓 / 배틀 티켓)
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoUpdateTicketUserNoti _v) {
        _v = new PktInfoUpdateTicketUserNoti();
        if (false == Read(_s, out _v.nextTime_)) return false;
        if (false == _s.Read(out _v.resultTicket_)) return false;
        if (false == Read(_s, out _v.ticketType_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoUpdateTicketUserNoti _v) {
        Write(_s, _v.nextTime_);
        _s.Write(_v.resultTicket_);
        Write(_s, _v.ticketType_);
    }
}

// 유저 로그인 보너스 관련 정보
public class PktInfoLoginBonus : PktMsgType
{
    public PktInfoMission mission_;         // 미션 정보
    public PktInfoEvtLogin evtLgn_;         // 이벤트 로그인 정보
    public PktInfoTime lastBonusLgnTM_;     // 마지막 로그인 보너스 획득 시간
    public System.UInt32 lgnGID_;           // 로그인 보너스 그룹 ID
    public System.UInt16 totalLgnCnt_;      // 총 로그인 일수
    public System.UInt16 continueLgnCnt_;   // 연속 로그인 일수
    public System.UInt16 raidHP_;           // 초기화된 모든 캐릭터의 raidHP 비율 (0보다 클 경우 초기화 진행)
    public System.UInt16 dailyLimitPoint_;  // 초기화된 하루동안 누적된 레이드 포인트
    public System.Byte lgnGroupCnt_;        // 로그인 보너스 그룹내의 보상 카운트
    public System.Byte preCnt_;             // 초기화된 모든 캐릭터의 선물하기 횟수 (0보다 클 경우 초기화 진행)
    public System.Byte scrCnt_;             // 초기화된 모든 캐릭터의 시크릿 퀘스트 횟수 (0보다 클 경우 초기화 진행)
	public ushort nowLgnBonusMonthlyCnt_;   // 현재 300일 출석판 로그인 일수
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoLoginBonus _v) {
        _v = new PktInfoLoginBonus();
        if (false == Read(_s, out _v.mission_)) return false;
        if (false == Read(_s, out _v.evtLgn_)) return false;
        if (false == Read(_s, out _v.lastBonusLgnTM_)) return false;
        if (false == _s.Read(out _v.lgnGID_)) return false;
        if (false == _s.Read(out _v.totalLgnCnt_)) return false;
        if (false == _s.Read(out _v.continueLgnCnt_)) return false;
        if (false == _s.Read(out _v.raidHP_)) return false;
        if (false == _s.Read(out _v.dailyLimitPoint_)) return false;
        if (false == _s.Read(out _v.lgnGroupCnt_)) return false;
        if (false == _s.Read(out _v.preCnt_)) return false;
        if (false == _s.Read(out _v.scrCnt_)) return false;
        if (false == _s.Read(out _v.nowLgnBonusMonthlyCnt_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoLoginBonus _v) {
        Write(_s, _v.mission_);
        Write(_s, _v.evtLgn_);
        Write(_s, _v.lastBonusLgnTM_);
        _s.Write(_v.lgnGID_);
        _s.Write(_v.totalLgnCnt_);
        _s.Write(_v.continueLgnCnt_);
        _s.Write(_v.raidHP_);
        _s.Write(_v.dailyLimitPoint_);
        _s.Write(_v.lgnGroupCnt_);
        _s.Write(_v.preCnt_);
        _s.Write(_v.scrCnt_);
        _s.Write(_v.nowLgnBonusMonthlyCnt_);
    }
}

// 슬롯 확장
public class PktInfoAddSlot : PktMsgType
{
    public PktInfoConsumeItemAndGoods comsume_; // 비용 소모가 적용된 아이템 및 재화
    public System.UInt16 nowSlotCnt_;           // 확장된 결과 슬롯 공간 수
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 슬롯 확장
    public static bool Read(Message _s, out PktInfoAddSlot _v) {
        _v = new PktInfoAddSlot();
        if (false == Read(_s, out _v.comsume_)) return false;
        if (false == _s.Read(out _v.nowSlotCnt_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoAddSlot _v) {
        Write(_s, _v.comsume_);
        _s.Write(_v.nowSlotCnt_);
    }
}

// 유저 계정삭제 패킷 메시지
public class PktInfoAccountDelete : PktMsgType
{
	public List<System.UInt32> timeAtkStageTIDs_;
    public List<System.UInt16> raidLevels_;
	public System.UInt64 uuid_;
	public System.Byte arenaRank_;
	public System.Byte influRank_;
	public System.UInt64 delTime_;
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoAccountDelete _v)
	{
		_v = new PktInfoAccountDelete();
		if (false == Read(_s, out _v.timeAtkStageTIDs_)) return false;
        if (false == Read(_s, out _v.raidLevels_)) return false;
		if (false == _s.Read(out _v.uuid_)) return false;
		if (false == _s.Read(out _v.arenaRank_)) return false;
		if (false == _s.Read(out _v.influRank_)) return false;
		if (false == _s.Read(out _v.delTime_)) return false;
		return true;
	}
	public static void Write(Message _s, PktInfoAccountDelete _v)
	{
		Write(_s, _v.timeAtkStageTIDs_);
        Write(_s, _v.raidLevels_);
		_s.Write(_v.uuid_);
		_s.Write(_v.arenaRank_);
		_s.Write(_v.influRank_);
		_s.Write(_v.delTime_);
	}
}