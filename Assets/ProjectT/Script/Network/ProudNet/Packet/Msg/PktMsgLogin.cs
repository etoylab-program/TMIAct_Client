using System.Collections.Generic;
using Nettention.Proud;


public class PktInfoCircleAuthority : PktMsgType
{
	public ulong ID_;                   // 서클 ID
	public eCircleAuthLevel authLv_;    // 나의 서클 권한

	public override bool Read(Message _s)
	{
		if (false == _s.Read(out ID_)) return false;
		if (false == PN_MarshalerEx.Read(_s, out authLv_)) return false;
		return true;
	}
	public override void Write(Message _s)
	{
		_s.Write(ID_);
		PN_MarshalerEx.Write(_s, authLv_);
	}
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoCircleAuthority _v)
	{
		_v = new PktInfoCircleAuthority();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoCircleAuthority _v)
	{
		_v.Write(_s);
	}
}

// 로그인 시점에 받는 공용 정보
public class PktInfoLoginCommon : PktMsgType
{
    public PktInfoUserReflash reflash_;         // 서버에서 갱신될 수 있는 공용 정보
    public PktInfoArenaSeasonTime arenaSeasonTime_; // PVP 모드 시즌 시간
    public PktInfoRaidSeasonTime raidSeasonTime_; // RAID 모드 시즌 시간
    public PktInfoComTimeAndTID rgacha_;        // 서버 로테이션 가챠 정보
    public PktInfoSecretQuestOpt sqOpt_;        // 서버 시크릿 퀘스트 옵션 정보
    public PktInfoTime svrTime_;                // 서버 기준 시스템 시간입니다.
    public PktInfoVersion ver_;                 // 서버 버전
    public System.Int32 gmTimeGap_;             // 서버 기준 시간과 세계 표준 기준 시간과의 차이입니다.(초단위)
    public System.UInt32 verNum_;               // 서버 버전 번호(svn 리비전)
    public System.UInt32 openRaidTypeValue_;    // 현재 오픈된 레이드 타입벨류
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoLoginCommon _v) {
        _v = new PktInfoLoginCommon();
        if (false == Read(_s, out _v.reflash_)) return false;
        if (false == Read(_s, out _v.arenaSeasonTime_)) return false;
        if (false == Read(_s, out _v.raidSeasonTime_)) return false;
        if (false == Read(_s, out _v.rgacha_)) return false;
        if (false == Read(_s, out _v.sqOpt_)) return false;
        if (false == Read(_s, out _v.svrTime_)) return false;
        if (false == Read(_s, out _v.ver_)) return false;
        if (false == _s.Read(out _v.gmTimeGap_)) return false;
        if (false == _s.Read(out _v.verNum_)) return false;
        if (false == _s.Read(out _v.openRaidTypeValue_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoLoginCommon _v) {
        Write(_s, _v.reflash_);
        Write(_s, _v.arenaSeasonTime_);
        Write(_s, _v.raidSeasonTime_);
        Write(_s, _v.rgacha_);
        Write(_s, _v.sqOpt_);
        Write(_s, _v.svrTime_);
        Write(_s, _v.ver_);
        _s.Write(_v.gmTimeGap_);
        _s.Write(_v.verNum_);
        _s.Write(_v.openRaidTypeValue_);
    }
}

// 클라이언트 보안 정보 요청 패킷 메시지
public class PktInfoClientSecurityReq : PktMsgType
{
    public eSecurityKind secuKind_;         // 보안 분류
}
// 클라이언트 보안 정보 응답 패킷 메시지
public class PktInfoClientSecurityAck : PktMsgType
{
    public PktInfoStr secuKey_;             // 보안 키
}
// 클라이언트 보안 검증 응답 패킷 메시지
public class PktInfoClientSecurityVerifyReq : PktMsgType
{
    public PktInfoStr token_;               // 보안 토큰
    public System.UInt64 secuUserID_;       // 보안을 요청한 유저의 해당 플렛폼 고유 ID
    public eSecurityKind secuKind_;         // 보안 분류
}
// 클라이언트 보안 검증 응답 패킷 메시지
public class PktInfoClientSecurityVerifyAck : PktMsgType
{
    public PktInfoLoginCommon svrInfo_;     // 서버 정보
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 클라이언트 보안 정보 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoClientSecurityReq _v) {
        _v = new PktInfoClientSecurityReq();
        if (false == Read(_s, out _v.secuKind_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoClientSecurityReq _v) {
        Write(_s, _v.secuKind_);
    }
    // 클라이언트 보안 정보 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoClientSecurityAck _v) {
        _v = new PktInfoClientSecurityAck();
        if (false == Read(_s, out _v.secuKey_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoClientSecurityAck _v) {
        Write(_s, _v.secuKey_);
    }
    // 클라이언트 보안 검증 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoClientSecurityVerifyReq _v) {
        _v = new PktInfoClientSecurityVerifyReq();
        if (false == Read(_s, out _v.token_)) return false;
        if (false == _s.Read(out _v.secuUserID_)) return false;
        if (false == Read(_s, out _v.secuKind_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoClientSecurityVerifyReq _v) {
        Write(_s, _v.token_);
        _s.Write(_v.secuUserID_);
        Write(_s, _v.secuKind_);
    }
    // 클라이언트 보안 검증 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoClientSecurityVerifyAck _v) {
        _v = new PktInfoClientSecurityVerifyAck();
        if (false == Read(_s, out _v.svrInfo_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoClientSecurityVerifyAck _v) {
        Write(_s, _v.svrInfo_);
    }
}

// 계정 인증 및 로그인 패킷
public class PktInfoAuthLogin : PktMsgType
{
    public PktInfoVersion ver_;                 // 버전 정보 - (클라이언트에서 채워줘야 할 값)
    public PktInfoStr accountID_;               // 계정 ID - (클라이언트에서 채워줘야 할 값)
    public PktInfoStr country_;                 // 국가 코드 - (클라이언트에서 채워줘야 할 값)
    public PktInfoStr lang_;                    // 언어 코드 - (클라이언트에서 채워줘야 할 값)
    public System.UInt64 uuid_;                 // 유저 고유 ID - (클라이언트에서 채워줘야 할 값)
    public ePlatformKind platform_;             // 플렛폼 분류 - (클라이언트에서 채워줘야 할 값)
    public eSecurityKind secuKind_;             // 보안 분류 - (클라이언트에서 채워줘야 할 값)
}
// 연동 된 정보로 유저 정보 획득 요청
public class PktInfoUserInfoFromLinkReq : PktMsgType
{
    public PktInfoStr linkID_;                  // 이어 하기 코드 또는 연결 계정 ID 값 - (클라이언트에서 채워줘야 할 값)
    public PktInfoStr password_;                // 이어 하기 코드를 검증할 PassWord - (클라이언트에서 채워줘야 할 값)
    public PktInfoStr accountID_;               // 계정 ID - (클라이언트에서 채워줘야 할 값)
    public eAccountType type_;                  // 걔정 타입 - (클라이언트에서 채워줘야 할 값)
}
// 연동 된 정보로 유저 정보 획득 응답
public class PktInfoUserInfoFromLinkAck : PktMsgType
{
    public System.UInt64 uuid_;                  // 유저 고유 ID
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 계정 인증 및 로그인 패킷
    public static bool Read(Message _s, out PktInfoAuthLogin _v) {
        _v = new PktInfoAuthLogin();
        if (false == Read(_s, out _v.ver_)) return false;
        if (false == Read(_s, out _v.accountID_)) return false;
        if (false == Read(_s, out _v.country_)) return false;
        if (false == Read(_s, out _v.lang_)) return false;
        if (false == _s.Read(out _v.uuid_)) return false;
        if (false == Read(_s, out _v.platform_)) return false;
        if (false == Read(_s, out _v.secuKind_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoAuthLogin _v) {
        Write(_s, _v.ver_);
        Write(_s, _v.accountID_);
        Write(_s, _v.country_);
        Write(_s, _v.lang_);
        _s.Write(_v.uuid_);
        Write(_s, _v.platform_);
        Write(_s, _v.secuKind_);
    }
    // 연동 된 정보로 유저 정보 획득 요청
    public static bool Read(Message _s, out PktInfoUserInfoFromLinkReq _v) {
        _v = new PktInfoUserInfoFromLinkReq();
        if (false == Read(_s, out _v.linkID_)) return false;
        if (false == Read(_s, out _v.password_)) return false;
        if (false == Read(_s, out _v.accountID_)) return false;
        if (false == Read(_s, out _v.type_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoUserInfoFromLinkReq _v) {
        Write(_s, _v.linkID_);
        Write(_s, _v.password_);
        Write(_s, _v.accountID_);
        Write(_s, _v.type_);
    }
    // 연동 된 정보로 유저 정보 획득 응답
    public static bool Read(Message _s, out PktInfoUserInfoFromLinkAck _v) {
        _v = new PktInfoUserInfoFromLinkAck();
        if (false == _s.Read(out _v.uuid_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoUserInfoFromLinkAck _v) {
        _s.Write(_v.uuid_);
    }
}