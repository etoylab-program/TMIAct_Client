using System.Collections.Generic;
using Nettention.Proud;


// 스테이지 클리어 패킷 메시지
public class PktInfoStageClear : PktMsgType
{
    public class Piece : PktMsgType {
        public enum REWARD : int {
            _MISSION_1  = 0,
            _MISSION_2,
            _MISSION_3,

            _MAX_,
        };

        public System.UInt32 tableID_;
        public System.UInt32 rewardFlag_;   // 보상 획득 여부

        // 해당 미션을 클리어 했는지 확인합니다.
        public bool IsOnClear(int _flagIdx /*PktInfoStageClear.Piece.REWARD*/) {
            if (32 <= _flagIdx) return false;
            return _IsOnBitIdx(rewardFlag_, (uint)_flagIdx);
        }
        // 해당 미션을 클리어했다는 값을 활성화합니다.
        public void DoOnClear(int _flagIdx /*PktInfoStageClear.Piece.REWARD*/) {
            if (32 <= _flagIdx) return;
            _DoOnBitIdx(ref rewardFlag_, (uint)_flagIdx);
        }

        public override bool Read(Message _s) {
            if (false == _s.Read(out tableID_)) return false;
            return _s.Read(out rewardFlag_);
        }
        public override void Write(Message _s) {
            _s.Write(tableID_);
            _s.Write(rewardFlag_);
        }
    };
    public List<Piece> infos_;
};

// 타임어택 스테이지 기록 저장 패킷 메시지
public class PktInfoTimeAtkStageRec : PktMsgType
{
    public class Piece : PktMsgType
    {
        public PktInfoTime delTime_;            // 유저의 최고기록유지 시간
        public System.UInt64 charUID_;          // 최고기록을 낸 캐릭터 ID
        public System.UInt32 stageTID_;         // 스테이지 테이블 ID
        public System.UInt32 timeRecord_Ms_;    // 최고기록

        public override bool Read(Message _s)
        {
            if (false == PN_MarshalerEx.Read(_s, out delTime_)) return false;
            if (false == _s.Read(out charUID_)) return false;
            if (false == _s.Read(out stageTID_)) return false;
            return _s.Read(out timeRecord_Ms_);
        }
        public override void Write(Message _s)
        {
            PN_MarshalerEx.Write(_s, delTime_);
            _s.Write(charUID_);
            _s.Write(stageTID_);
            _s.Write(timeRecord_Ms_);
        }
    }

    public List<Piece> infos_;
};

// 레이드 시즌 초기화 관련 패킷 메시지
public class PktInfoInitRaidSeasonData : PktMsgType
{

        public PktInfoTime lastPlaySeasonEndTime_;    // 마지막으로 플레이한 레이드 시즌의 시즌종료시간
        public System.UInt16 bestOpenRaidLevel_ = 1;  // 플레이 가능한 레이드 최고 스테이지 단계   

};

public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 레이드 시즌 초기화 관련 패킷 메시지
    public static bool Read(Message _s, out PktInfoInitRaidSeasonData _v)
    {
        _v = new PktInfoInitRaidSeasonData();
        if (false == PN_MarshalerEx.Read(_s, out _v.lastPlaySeasonEndTime_)) return false;

        if (false == _s.Read(out _v.bestOpenRaidLevel_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoInitRaidSeasonData _v)
    {
        PN_MarshalerEx.Write(_s, _v.lastPlaySeasonEndTime_);
        _s.Write(_v.bestOpenRaidLevel_);
    }
}

// 레이드 스테이지 기록 저장 패킷 메시지
public class PktInfoRaidStageRec : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt64 charUID_;          // 최고기록을 낸 캐릭터 ID
        public System.UInt32 stageTID_;         // 스테이지 테이블 ID
        public System.UInt32 timeRecord_Ms_;    // 최고기록
        public System.UInt16 level_;         // 레이드 스테이지 단계

        public override bool Read(Message _s)
        {
            if (false == _s.Read(out charUID_)) return false;
            if (false == _s.Read(out stageTID_)) return false;
            if (false == _s.Read(out timeRecord_Ms_)) return false;
            if (false == _s.Read(out level_)) return false;
            
            return true;
        }
        public override void Write(Message _s)
        {
            _s.Write(charUID_);
            _s.Write(stageTID_);
            _s.Write(timeRecord_Ms_);
            _s.Write(level_);
        }
    }

    public List<Piece> infos_;
};

// RAID 유저 팀 편성 패킷 메세지
public class PktInfoUserRaidTeam : PktMsgType
{
    public enum eCHAR
    {
        FIRST = 0,
        SECOND,
        THIRD,

        _MAX_
    };
    public System.UInt64[] CUIDs_ = new System.UInt64[(int)eCHAR._MAX_];    //편성된 캐릭터 UIDs - (클라이언트에서 채워줘야 할 값)
    public System.UInt32 cardFrmtID_;       // 카드(서포터) 진형 ID
};

public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // RAID 유저 팀 편성 패킷 메세지
    public static bool Read(Message _s, out PktInfoUserRaidTeam _v)
    {
        _v = new PktInfoUserRaidTeam();
        for (int loop = 0; loop < (int)PktInfoUserRaidTeam.eCHAR._MAX_; ++loop)
        {
            if (false == _s.Read(out _v.CUIDs_[loop])) return false;
        }
        if (false == _s.Read(out _v.cardFrmtID_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoUserRaidTeam _v)
    {
        for (int loop = 0; loop < (int)PktInfoUserRaidTeam.eCHAR._MAX_; ++loop)
            _s.Write(_v.CUIDs_[loop]);
        _s.Write(_v.cardFrmtID_);
    }
}

// 레이드 유저 정보 패킷 메시지
public class PktInfoUserRaid : PktMsgType
{
    public PktInfoUserRaidTeam team_;           // 팀 데이터
    public PktInfoRaidStageRec record_;         // 모든 시즌 누적 기록 데이터 
    public PktInfoRaidData seasonData_;         // 레이드 현재 시즌 정보
    public PktInfoRaidStageRec seasonRecord_;   // 현재 시즌 누적 기록 데이터
    public System.UInt16 dailyLimitPoint_;      // 하루 동안 누적된 레이드 포인트
};

public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 레이드 유저 정보 패킷 메시지
    public static bool Read(Message _s, out PktInfoUserRaid _v)
    {
        _v = new PktInfoUserRaid();
        if (false == PN_MarshalerEx.Read(_s, out _v.team_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.record_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.seasonData_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.seasonRecord_)) return false;
        if (false == _s.Read(out _v.dailyLimitPoint_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoUserRaid _v)
    {
        PN_MarshalerEx.Write(_s, _v.team_);
        PN_MarshalerEx.Write(_s, _v.record_);
        PN_MarshalerEx.Write(_s, _v.seasonData_);
        PN_MarshalerEx.Write(_s, _v.seasonRecord_);
        _s.Write(_v.dailyLimitPoint_);

    }
}

// 게임 스테이지 시작 요청 패킷 메시지
public class PktInfoStageGameStartReq : PktMsgType
{
    public enum OPT {
        SK_BUF,         // 캐릭터 스킬 확장 버프 적용
        FAST_QUEST,     // 토벌권 사용
        MULTI_AP,       // stage AP 소모 비율 사용

        _MAX_,
    };
    public const System.Byte RaidCharMax_ = 3;          // 레이드 참여 캐릭터 개수
    public const System.Byte RaidLeaderIdx_ = 0;        // 레이드 리더 인덱스

    public PktInfoUIDList secretSubCuids_;      // 비밀임무 참여한 서브캐릭터 uid들
    public System.UInt64 cuid_;                 // 캐릭터 고유 ID - (클라이언트에서 채워줘야 할 값)
    public System.UInt64[] raidCUIDs_ = new System.UInt64[(int)RaidCharMax_];          // 레이드 참여 캐릭터 UIDs -(클라이언트에서 채워줘야 할 값)
    public List<System.UInt64> useItemUIDs_;    // 유저가 사용한 버프 아이템 UID - (클라이언트에서 채워줘야 할 값)
    public System.UInt32 stageTID_;             // 스테이지 테이블 ID - (클라이언트에서 채워줘야 할 값)
    public System.UInt16 raidLevel_;                  // 레이드 단계 - (클라이언트에서 채워줘야 할 값)
    public System.Byte ticketMultipleIdx_;      // 티켓 배수 배열 인덱스 - (클라이언트에서 채워줘야 할 값)
    public System.Byte optFlag_;                // 스테이지 관련 옵션 플레그 - (클라이언트에서 채워줘야 할 값 )

    // 해당 옵션 활성화 처리
    public void DoOnOpt(PktInfoStageGameStartReq.OPT _idx)
    {
        if (PktInfoStageGameStartReq.OPT._MAX_ <= _idx) return;
        _DoOnBitIdx(ref optFlag_, (System.Byte)_idx);
    }
}

// 게임 스테이지 시작 응답 패킷 메시지
public class PktInfoStageGameStartAck : PktMsgType
{
    public PktInfoTime ticketNextTime_;             // 다음 티켓 회복 시간( BP의 회복시간을 나타냅니다.)
    public PktInfoConsumeItemAndGoods consume_;     // 유저가 사용한 버프 아이템 UID
    public PktInfoUIDList secretSubCuids_;          // 비밀임무 참여한 서브캐릭터 uid들
    public System.UInt64 cuid_;                     // 캐릭터 고유 ID
    public System.UInt64[] raidCUIDs_= new System.UInt64[PktInfoStageGameStartReq.RaidCharMax_];    // 레이드 캐릭터 고유 IDs
    public System.UInt32 stageTID_;                     // 스테이지 테이블 ID
    public System.UInt16[] raidCharHP_ = new System.UInt16[PktInfoStageGameStartReq.RaidCharMax_];  // 레이드 캐릭터 HP
    public System.UInt16 raidLevel_;                // 레이드 스테이지 단계
    public System.Byte nBoxMaxCnt_;                 // 일반 상자 최대 개수(게임 플레이중 획득 가능한)
}

// 게임 플레이 정보 패킷 메시지 (부정행위 체크)
public class PktInfoIngamePlayData : PktMsgType
{
	public uint clearTime_Ms;			// 클리어 시간 밀리세컨드 단위
	public uint maxDamage_;				// 최대 데미지
	public uint teamPower_;				// 부대 전투력
	public uint avgDPS_;				// 평균 DPS(초당 데미지량)
	public uint maxDPS_;				// 최대 DPS(초당 데미지량)
	public uint sufferDamage_;			// 받은 총 피해량
	public uint totalRecovery_;			// 회복 총량
	public uint validRecovery_;			// 유효한 회복량(실제로 HP를 회복시킨 회복량)
	public float armoryRatio_;			// 무기고 증가배율
}

// 게임 스테이지 결과 요청 패킷 메시지
public class PktInfoStageGameResultReq : PktMsgType
{
    public System.UInt32 tutoVal_;          // 튜토리얼 값 - (클라이언트에서 채워줘야 할 값)
    public System.UInt16 dropGoldItemCnt_;  // 골드 아이템 획득 수 
    public System.UInt16[] raidCharHP_ = new System.UInt16[PktInfoStageGameStartReq.RaidCharMax_];   // 레이드 캐릭터 hp 
    public System.Byte nTakeBoxCnt_;        // 일반 상자 획득 개수 - (클라이언트에서 채워줘야 할 값)
    public bool mission0_;                  // 미션1 통과 여부 - (클라이언트에서 채워줘야 할 값)
    public bool mission1_;                  // 미션2 통과 여부 - (클라이언트에서 채워줘야 할 값)
    public bool mission2_;                  // 미션3 통과 여부 - (클라이언트에서 채워줘야 할 값)
	public PktInfoIngamePlayData playData_;	// 인게임 플레이(부정행위 체크) 정보(클라이언트에서 채워줘야 할 값)
}

// 게임 스테이지 결과 응답 패킷 메시지
public class PktInfoStageGameResultAck : PktMsgType
{
    public enum STATE : System.Byte {
        _NONE_          = 0,        // 일반 클리어
        FIRST,                      // 최초 클리어
        MISSION,                    // 미션 보상 클리어
        RAID_FIRST,                 // 레이드 스테이지 최초 클리어
        RAID_SEASON_FIRST,          // 레이드 스테이지 현재 시즌 최초 클리어
        RAID_AND_RAIDSEASON_FIRST,  // 레이드 스테이지 최초클리어 && 현재시즌 최초클리어 

        _MAX_,
    };
    public PktInfoMonsterBook monsterBooks_;    // 몬스터 도감에 추가되는 몬스터 TID들
    public PktInfoCardBook cardBook_;           // 카드(서포터) 도감 관련 갱신 정보
    public PktInfoStage info_;                  // 스테이지 정보
    public PktInfoTimeAtkStageRec timeRecord_;  // 타임어택 기록 정보
    public PktInfoRaidStageRec raidRecord_;     // 레이드 기록 정보
    public PktInfoRaidStageRec raidSeasonRecord_;// 레이드 현재 시즌 기록 정보 
    public PktInfoTime ticketNextTime_;         // 다음 티켓 회복 시간
    public PktInfoExpLv userExpLv_;             // 보상이 추가된 유저의 경험치 레벨
    public PktInfoExpLv charExpLv_;             // 보상이 추가된 캐릭터의 경험치 레벨
    public PktInfoExpLv[] raidCharExpLvs_ = new PktInfoExpLv[(int)PktInfoStageGameStartReq.RaidCharMax_];    // 보상이 추가된 캐릭터의 경험치 레벨 : 레이드 전용
    public PktInfoConsumeItem useItem_;         // 사용한 토벌권 아이템 정보
    public PktInfoUIDValue subCharSecretCnt_;   // 비밀임무 참여한 서브캐릭터 uid와 참여가능 cnt 값
    public System.UInt64 charUID_;              // 플레이한 캐릭터의 UID
    public System.UInt64[] raidCUIDs_ = new System.UInt64[(int)PktInfoStageGameStartReq.RaidCharMax_];    // 플레이한 캐릭터의 UID : 레이드 전용
    public System.UInt32 tutoVal_;              // 튜토리얼 값
    public System.UInt32 clearStageTID_;        // 클리어된 스테이지 TID
    public System.UInt32 missionRewardFlag_;    // 미션 보상 수령 여부
    public System.UInt32 charSkillPT_;          // 보상이 추가된 캐릭터의 현재 스킬 포인트
    public System.UInt32[]  raidCharSkillPTs_ = new System.UInt32[(int)PktInfoStageGameStartReq.RaidCharMax_]; // 보상이 추가된 캐릭터의 현재 스킬 포인트 : 레이드 전용
    public System.UInt16 raidLevel_;            // 클리어된 레이드 단계 
    public System.UInt16 raidBestOpenLevel_;    // 오픈된 레이드 레벨 최고값, 오픈된 값이 없을경우는 0 
    public System.UInt16 dailyLimitPoint_;      // 하루 동안 누적된 레이드 포인트, 획득 값이 없을경우는 0 
    public System.Byte stageClearState_;        // 처음 클리어 상태 PktInfoStageGameResultAck::STATE
    public System.SByte charSecretCnt_;         // 시크릿 퀘스트 현재 횟수(-1은 변경내용 없음 값임)
    public eGOODSTYPE ticketType;               // 사용한 티켓 타입
    public bool isChangeUser_;                  // 유저 경험치가 변경 됐는지 알아보는 플레그
    public bool isChangeChar_;                  // 캐릭터 경험치가 변경 됐는지 알아보는 플레그
    public bool[] isChangeRaidChar_ = new bool[(int)PktInfoStageGameStartReq.RaidCharMax_]{true,true,true}; // 캐릭터 정보가 변경 됐는지 알아보는 플레그 : 레이드 전용
    public System.UInt16[] raidCharHPs_ = new System.UInt16[(int)PktInfoStageGameStartReq.RaidCharMax_];    // 캐릭터 정보가 변경 됐는지 알아보는 플레그 : 레이드 전용
}

// 게임 스테이지 실패 패킷 메시지
public class PktInfoStageGameEndFail : PktMsgType
{
    public System.UInt64    resultRaidPoint_;                                                               // 기본 레이드포인트 적용한 최종값 : 레이드 전용
    public System.UInt64[]  raidCUIDs_ = new System.UInt64[(int)PktInfoStageGameStartReq.RaidCharMax_];     // 레이드: 캐릭터 고유 IDs - (클라이언트에서 채워줘야 할 값)
    public System.UInt32    playTime_Ms_;                                                                   // 플레이 시간 밀리세컨드 단위 - (클라이언트에서 채워줘야 할 값)
    public System.UInt32    stageTID_;                                                                      // 스테이지 테이블 ID
    public System.UInt16[]  raidCharHPs_ = new System.UInt16[(int)PktInfoStageGameStartReq.RaidCharMax_];   // 캐릭터 정보가 변경 됐는지 알아보는 플레그 : 레이드 전용
    public System.UInt16    dailyLimitPoint_;                                                               // 하루 누적 레이드 포인트 : 레이드 전용
};

// 레이드 스테이지 포기 패킷 메시지
public class PktInfoRaidStageDrop : PktMsgType
{    
    public System.UInt64[] raidCUIDs_ = new System.UInt64[(int)PktInfoStageGameStartReq.RaidCharMax_];    // 레이드: 캐릭터 고유 IDs - (클라이언트에서 채워줘야 할 값)
    public System.UInt16[] raidCharHPs_ = new System.UInt16[(int)PktInfoStageGameStartReq.RaidCharMax_];    // 레이드 캐릭터 hp  - (클라이언트에서 채워줘야 할 값)
};

public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 스테이지 클리어 패킷 메시지
    public static bool Read(Message _s, out PktInfoStageClear _v) {
        _v = new PktInfoStageClear();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoStageClear _v) {
        WriteList(_s, _v.infos_);
    }

    // 타임어택 스테이지 기록 저장 패킷 메시지
    public static bool Read(Message _s, out PktInfoTimeAtkStageRec _v) {
        _v = new PktInfoTimeAtkStageRec();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoTimeAtkStageRec _v) {
        WriteList(_s, _v.infos_);
    }
    
    // 레이드 스테이지 기록 저장 패킷 메시지
    public static bool Read(Message _s, out PktInfoRaidStageRec _v)
    {
        _v = new PktInfoRaidStageRec();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoRaidStageRec _v)
    {
        WriteList(_s, _v.infos_);
    }

    // 게임 스테이지 시작 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoStageGameStartReq _v) {
        _v = new PktInfoStageGameStartReq();
        if (false == Read(_s, out _v.secretSubCuids_)) return false;
        if (false == _s.Read(out _v.cuid_)) return false;

        for (int loop = 0; loop < (int)PktInfoStageGameStartReq.RaidCharMax_; ++loop)
            if (false == _s.Read(out _v.raidCUIDs_[loop]))  return false;
        if (false == Read(_s, out _v.useItemUIDs_)) return false;
        if (false == _s.Read(out _v.stageTID_)) return false;
        if (false == _s.Read(out _v.raidLevel_)) return false;
        if (false == _s.Read(out _v.ticketMultipleIdx_)) return false;
        if (false == _s.Read(out _v.optFlag_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoStageGameStartReq _v) {
        PN_MarshalerEx.Write(_s, _v.secretSubCuids_);
        _s.Write(_v.cuid_);
        for (int loop = 0; loop < (int)PktInfoStageGameStartReq.RaidCharMax_; ++loop)
            _s.Write(_v.raidCUIDs_[loop]);
        PN_MarshalerEx.Write(_s, _v.useItemUIDs_);
        _s.Write(_v.stageTID_);
        _s.Write(_v.raidLevel_);
        _s.Write(_v.ticketMultipleIdx_);
        _s.Write(_v.optFlag_);
    }


    // 게임 스테이지 시작 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoStageGameStartAck _v) {
        _v = new PktInfoStageGameStartAck();
        if (false == PN_MarshalerEx.Read(_s, out _v.ticketNextTime_)) return false;
        if (false == Read(_s, out _v.consume_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.secretSubCuids_)) return false;
        if (false == _s.Read(out _v.cuid_)) return false;

        for (int loop = 0; loop < (int)PktInfoStageGameStartReq.RaidCharMax_; ++loop)
            if (false == _s.Read(out _v.raidCUIDs_[loop])) return false;

        if (false == _s.Read(out _v.stageTID_)) return false;
        
        for (int loop = 0; loop < (int)PktInfoStageGameStartReq.RaidCharMax_; ++loop)
            if (false == _s.Read(out _v.raidCharHP_[loop])) return false;

        if (false == _s.Read(out _v.raidLevel_)) return false;
        if (false == _s.Read(out _v.nBoxMaxCnt_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoStageGameStartAck _v) {
        PN_MarshalerEx.Write(_s, _v.ticketNextTime_);
        PN_MarshalerEx.Write(_s, _v.consume_);
        PN_MarshalerEx.Write(_s, _v.secretSubCuids_);
        _s.Write(_v.cuid_);
        for (int loop = 0; loop < (int)PktInfoStageGameStartReq.RaidCharMax_; ++loop)
            PN_MarshalerEx.Write(_s, _v.raidCUIDs_[loop]);
        _s.Write(_v.stageTID_);
        for (int loop = 0; loop < (int)PktInfoStageGameStartReq.RaidCharMax_; ++loop)
            PN_MarshalerEx.Write(_s, _v.raidCharHP_[loop]);
        _s.Write(_v.raidLevel_);
        _s.Write(_v.nBoxMaxCnt_);
    }

	public static bool Read(Message _s, out PktInfoIngamePlayData _v) {
		_v = new PktInfoIngamePlayData();
        if (false == _s.Read(out _v.clearTime_Ms)) return false;
        if (false == _s.Read(out _v.maxDamage_)) return false;
        if (false == _s.Read(out _v.teamPower_)) return false;
        if (false == _s.Read(out _v.avgDPS_)) return false;
        if (false == _s.Read(out _v.maxDPS_)) return false;
        if (false == _s.Read(out _v.sufferDamage_)) return false;
        if (false == _s.Read(out _v.totalRecovery_)) return false;
        if (false == _s.Read(out _v.validRecovery_)) return false;
        if (false == _s.Read(out _v.armoryRatio_)) return false;
		return true;
	}

	public static void Write(Message _s, PktInfoIngamePlayData _v)
	{
		_s.Write(_v.clearTime_Ms);
		_s.Write(_v.maxDamage_);
		_s.Write(_v.teamPower_);
		_s.Write(_v.avgDPS_);
		_s.Write(_v.maxDPS_);
		_s.Write(_v.sufferDamage_);
		_s.Write(_v.totalRecovery_);
		_s.Write(_v.validRecovery_);
		_s.Write(_v.armoryRatio_);
	}

	// 게임 스테이지 결과 요청 패킷 메시지
	public static bool Read(Message _s, out PktInfoStageGameResultReq _v) {
        _v = new PktInfoStageGameResultReq();
        if (false == _s.Read(out _v.tutoVal_)) return false;
        if (false == _s.Read(out _v.dropGoldItemCnt_)) return false;
        for (int loop = 0; loop < (int)PktInfoStageGameStartReq.RaidCharMax_; ++loop)
            if (false == _s.Read(out _v.raidCharHP_[loop])) return false;
        if (false == _s.Read(out _v.nTakeBoxCnt_)) return false;
        if (false == _s.Read(out _v.mission0_)) return false;
        if (false == _s.Read(out _v.mission1_)) return false;
        if (false == _s.Read(out _v.mission2_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.playData_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoStageGameResultReq _v) {
        _s.Write(_v.tutoVal_);
        _s.Write(_v.dropGoldItemCnt_);
        for (int loop = 0; loop < (int)PktInfoStageGameStartReq.RaidCharMax_; ++loop)
            PN_MarshalerEx.Write(_s, _v.raidCharHP_[loop]);
        _s.Write(_v.nTakeBoxCnt_);
        _s.Write(_v.mission0_);
        _s.Write(_v.mission1_);
        _s.Write(_v.mission2_);
		PN_MarshalerEx.Write(_s, _v.playData_);

	}

    // 게임 스테이지 결과 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoStageGameResultAck _v) {
        _v = new PktInfoStageGameResultAck();
        if (false == Read(_s, out _v.monsterBooks_)) return false;
        if (false == Read(_s, out _v.cardBook_)) return false;
        if (false == Read(_s, out _v.info_)) return false;
        if (false == Read(_s, out _v.timeRecord_)) return false;
        if (false == Read(_s, out _v.raidRecord_)) return false;
        if (false == Read(_s, out _v.raidSeasonRecord_)) return false;
        if (false == Read(_s, out _v.ticketNextTime_)) return false;
        if (false == Read(_s, out _v.userExpLv_)) return false;
        if (false == Read(_s, out _v.charExpLv_)) return false;
        for (int loop = 0; loop < (int)PktInfoStageGameStartReq.RaidCharMax_; ++loop)
            if (false == PN_MarshalerEx.Read(_s, out _v.raidCharExpLvs_[loop])) return false;
        if (false == Read(_s, out _v.useItem_)) return false;
        if (false == Read(_s, out _v.subCharSecretCnt_)) return false;
        if (false == _s.Read(out _v.charUID_)) return false;
        for (int loop = 0; loop < (int)PktInfoStageGameStartReq.RaidCharMax_; ++loop)
            if (false == _s.Read(out _v.raidCUIDs_[loop])) return false;
        if (false == _s.Read(out _v.tutoVal_)) return false;
        if (false == _s.Read(out _v.clearStageTID_)) return false;
        if (false == _s.Read(out _v.missionRewardFlag_)) return false;
        if (false == _s.Read(out _v.charSkillPT_)) return false;
        for (int loop = 0; loop < (int)PktInfoStageGameStartReq.RaidCharMax_; ++loop)
            if (false == _s.Read(out _v.raidCharSkillPTs_[loop])) return false;
        if (false == _s.Read(out _v.raidLevel_)) return false;
        if (false == _s.Read(out _v.raidBestOpenLevel_)) return false;
        if (false == _s.Read(out _v.dailyLimitPoint_)) return false;
        if (false == _s.Read(out _v.stageClearState_)) return false;
        if (false == _s.Read(out _v.charSecretCnt_)) return false;
        if (false == Read(_s, out _v.ticketType)) return false;
        if (false == _s.Read(out _v.isChangeUser_)) return false;
        if (false == _s.Read(out _v.isChangeChar_)) return false;
        for (int loop = 0; loop < (int)PktInfoStageGameStartReq.RaidCharMax_; ++loop)
            if (false == _s.Read(out _v.isChangeRaidChar_[loop])) return false;
        for (int loop = 0; loop < (int)PktInfoStageGameStartReq.RaidCharMax_; ++loop)
            if (false == _s.Read(out _v.raidCharHPs_[loop])) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoStageGameResultAck _v) {
        Write(_s, _v.monsterBooks_);
        Write(_s, _v.cardBook_);
        Write(_s, _v.info_);
        Write(_s, _v.timeRecord_);
        Write(_s, _v.raidRecord_);
        Write(_s, _v.raidSeasonRecord_);
        Write(_s, _v.ticketNextTime_);
        Write(_s, _v.userExpLv_);
        Write(_s, _v.charExpLv_);
        for (int loop = 0; loop < (int)PktInfoStageGameStartReq.RaidCharMax_; ++loop)
            PN_MarshalerEx.Write(_s, _v.raidCharExpLvs_[loop]);
        Write(_s, _v.useItem_);
        Write(_s, _v.subCharSecretCnt_);
        _s.Write(_v.charUID_);
        for (int loop = 0; loop < (int)PktInfoStageGameStartReq.RaidCharMax_; ++loop)
            _s.Write(_v.raidCUIDs_[loop]);
        _s.Write(_v.tutoVal_);
        _s.Write(_v.clearStageTID_);
        _s.Write(_v.missionRewardFlag_);
        _s.Write(_v.charSkillPT_);
        for (int loop = 0; loop < (int)PktInfoStageGameStartReq.RaidCharMax_; ++loop)
            _s.Write(_v.raidCharSkillPTs_[loop]);
        _s.Write(_v.raidLevel_);
        _s.Write(_v.raidBestOpenLevel_);
        _s.Write(_v.dailyLimitPoint_);
        _s.Write(_v.stageClearState_);
        _s.Write(_v.charSecretCnt_);
        Write(_s, _v.ticketType);
        _s.Write(_v.isChangeUser_);
        _s.Write(_v.isChangeChar_);
        for (int loop = 0; loop < (int)PktInfoStageGameStartReq.RaidCharMax_; ++loop)
            _s.Write(_v.isChangeRaidChar_[loop]);
        for (int loop = 0; loop < (int)PktInfoStageGameStartReq.RaidCharMax_; ++loop)
            _s.Write(_v.raidCharHPs_[loop]);
    }

    // 게임 스테이지 실패 패킷 메시지
    public static bool Read(Message _s, out PktInfoStageGameEndFail _v) {
        _v = new PktInfoStageGameEndFail();
        if (false == _s.Read(out _v.resultRaidPoint_)) return false;
        
        for (System.Byte loop = 0; loop < PktInfoStageGameStartReq.RaidCharMax_; ++loop)
            if (false == _s.Read(out _v.raidCUIDs_[loop])) return false;
        
        if (false == _s.Read(out _v.playTime_Ms_)) return false;
        if (false == _s.Read(out _v.stageTID_)) return false;
        
        for (System.Byte loop = 0; loop < PktInfoStageGameStartReq.RaidCharMax_; ++loop)
            if (false == _s.Read(out _v.raidCharHPs_[loop])) return false;
        
        if (false == _s.Read(out _v.dailyLimitPoint_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoStageGameEndFail _v) {
        _s.Write(_v.resultRaidPoint_);

        for (System.Byte loop = 0; loop < PktInfoStageGameStartReq.RaidCharMax_; ++loop)
                _s.Write(_v.raidCUIDs_[loop]);

        _s.Write(_v.playTime_Ms_);
        _s.Write(_v.stageTID_);
        
        for (System.Byte loop = 0; loop < PktInfoStageGameStartReq.RaidCharMax_; ++loop)
            _s.Write(_v.raidCharHPs_[loop]);

        _s.Write(_v.dailyLimitPoint_);
    }
    
    // 레이드 스테이지 포기 패킷 메시지
    public static bool Read(Message _s, out PktInfoRaidStageDrop _v) {
        _v = new PktInfoRaidStageDrop();
        for (System.Byte loop = 0; loop < PktInfoStageGameStartReq.RaidCharMax_; ++loop)
            if (false == _s.Read(out _v.raidCUIDs_[loop])) return false;
        for (System.Byte loop = 0; loop < PktInfoStageGameStartReq.RaidCharMax_; ++loop)
            if (false == _s.Read(out _v.raidCharHPs_[loop])) return false;
        
        return true;
    }
    public static void Write(Message _s, PktInfoRaidStageDrop _v) {
        for (System.Byte loop = 0; loop < PktInfoStageGameStartReq.RaidCharMax_; ++loop)
                _s.Write(_v.raidCUIDs_[loop]);
        for (System.Byte loop = 0; loop < PktInfoStageGameStartReq.RaidCharMax_; ++loop)
            _s.Write(_v.raidCharHPs_[loop]);
    }
}