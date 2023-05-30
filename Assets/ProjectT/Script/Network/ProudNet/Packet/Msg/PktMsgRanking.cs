using System.Collections.Generic;
using Nettention.Proud;

// 타임어택 랭킹 리스트 헤더 (랭킹 업데이트 시간, 스테이지 ID) 패킷 메시지
public class PktInfoTimeAtkRankingHeader : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt64 updateTM_;       // 랭킹을 마지막으로 업데이트 한 시간 - (클라이언트에서 채워줘야 할 값)
        public System.UInt32 stageID_;        // 랭킹을 가져 올 스테이지 ID - (클라이언트에서 채워줘야 할 값)

        public override bool Read(Message _s) {
            if (false == _s.Read(out updateTM_)) return false;
            if (false == _s.Read(out stageID_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(updateTM_);
            _s.Write(stageID_);
        }
    };

    public List<Piece> infos_;
}

// 레이드 랭킹 리스트 헤더 (랭킹 업데이트 시간, 스테이지 ID) 패킷 메시지
public class PktInfoRaidRankingHeader : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt64 updateTM_;       // 랭킹을 마지막으로 업데이트 한 시간 - (클라이언트에서 채워줘야 할 값)
        public System.UInt32 stageID_;        // 랭킹을 가져 올 스테이지 ID - (클라이언트에서 채워줘야 할 값)
        public System.UInt16 raidLevel_;      // 랭킹을 가져 올 스테이지 단계(level) - (클라이언트에서 채워줘야 할 값)

        public override bool Read( Message _s ) {
            if( false == _s.Read( out updateTM_ ) )
                return false;
            if( false == _s.Read( out stageID_ ) )
                return false;
            if( false == _s.Read( out raidLevel_ ) )
                return false;
            return true;
        }
        public override void Write( Message _s ) {
            _s.Write( updateTM_ );
            _s.Write( stageID_ );
            _s.Write( raidLevel_ );
        }
    };

    public List<Piece> infos_;
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 타임어택 랭킹 리스트 헤더 (랭킹 업데이트 시간, 스테이지 ID) 패킷 메시지
    public static bool Read(Message _s, out PktInfoRaidRankingHeader.Piece _v)
    {
        _v = new PktInfoRaidRankingHeader.Piece();
        if (false == _s.Read(out _v.updateTM_)) return false;
        if (false == _s.Read(out _v.stageID_)) return false;
        if (false == _s.Read(out _v.raidLevel_)) return false;
        return true;
    }

    public static void Write(Message _s, PktInfoRaidRankingHeader.Piece _v)
    {
        _s.Write(_v.updateTM_);
        _s.Write(_v.stageID_);
        _s.Write(_v.raidLevel_);
    }


    public static bool Read(Message _s, out PktInfoRaidRankingHeader _v)
    {
        _v = new PktInfoRaidRankingHeader();
        if (false == PN_MarshalerEx.ReadList(_s, out _v.infos_)) return false;
        return true;
    }

    public static void Write(Message _s, PktInfoRaidRankingHeader _v)
    {
        PN_MarshalerEx.WriteList(_s, _v.infos_);
    }
}
    // 타임어택 랭킹 리스트(하나의 스테이지) 패킷 메시지
    public class PktInfoTimeAtkRankStage : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.String nickName_;         // 랭킹 유저 닉네임
		public uint nickNameColorID_;			// 랭킹 유저 닉네임 색상 아이디
        public PktInfoTime delTime_;            // 랭킹 유저 기록 유지 시간
        public System.UInt64 uuid_;             // 랭킹 유저 UUID
        public System.UInt32 timeRecord_Ms_;    // 랭킹 유저 기록
        public System.UInt32 markID_;           // 랭킹 유저 마크 ID
        public System.UInt32 costumeID_;        // 캐릭터 코스튬ID
        public System.UInt32 charID_;           // 랭킹 유저가 기록에 사용한 캐릭터 TID
        public System.Byte userLv_;             // 랭킹 유저 레벨
        public System.Byte charLv_;             // 랭킹 유저가 기록에 사용한 캐릭터 Lv
        public System.Byte charGrade_;          // 랭킹 유저가 기록에 사용한 캐릭터 등급
		public ulong circleID_;					// 랭킹 유저의 서클 아이디

		public override bool Read(Message _s) {
            if (false == _s.Read(out nickName_)) return false;
			if (false == _s.Read(out nickNameColorID_)) return false;
			if (false == PN_MarshalerEx.Read(_s, out delTime_)) return false;
            if (false == _s.Read(out uuid_)) return false;
            if (false == _s.Read(out timeRecord_Ms_)) return false;
            if (false == _s.Read(out markID_)) return false;
            if (false == _s.Read(out costumeID_)) return false;
            if (false == _s.Read(out charID_)) return false;
            if (false == _s.Read(out userLv_)) return false;
            if (false == _s.Read(out charLv_)) return false;
            if (false == _s.Read(out charGrade_)) return false;
            if (false == _s.Read(out circleID_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(nickName_);
			_s.Write(nickNameColorID_);
			PN_MarshalerEx.Write(_s, delTime_);
            _s.Write(uuid_);
            _s.Write(timeRecord_Ms_);
            _s.Write(markID_);
            _s.Write(costumeID_);
            _s.Write(charID_);
            _s.Write(userLv_);
            _s.Write(charLv_);
            _s.Write(charGrade_);
            _s.Write(circleID_);
        }
    };

    public List<Piece> infos_;
    public PktInfoTimeAtkRankingHeader.Piece header_;       // 랭킹 업데이트 시간, 스테이지 ID 정보

    public override bool Read(Message _s) {
        if (false == PN_MarshalerEx.ReadList(_s, out infos_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out header_)) return false;
        return true;
    }
    public override void Write(Message _s) {
        PN_MarshalerEx.WriteList(_s, infos_);
        PN_MarshalerEx.Write(_s, header_);
    }
};

// 레이드 랭킹 리스트(하나의 스테이지) 패킷 메시지
public class PktInfoRaidRankStage : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.String nickName_;         // 랭킹 유저 닉네임
        public uint nickNameColorID_;			// 랭킹 유저 닉네임 색상 아이디
        public System.UInt64 uuid_;             // 랭킹 유저 UUID
        public System.UInt32 timeRecord_Ms_;    // 랭킹 유저 기록
        public System.UInt32 markID_;           // 랭킹 유저 마크 ID
        public System.UInt32 costumeID_;        // 캐릭터 코스튬ID
        public System.UInt32 charID_;           // 랭킹 유저가 기록에 사용한 캐릭터 TID
        public System.Byte userLv_;             // 랭킹 유저 레벨
        public System.Byte charLv_;             // 랭킹 유저가 기록에 사용한 캐릭터 Lv
        public System.Byte charGrade_;          // 랭킹 유저가 기록에 사용한 캐릭터 등급

        public override bool Read(Message _s)
        {
            if (false == _s.Read(out nickName_)) return false;
            if (false == _s.Read(out nickNameColorID_)) return false;
            if (false == _s.Read(out uuid_)) return false;
            if (false == _s.Read(out timeRecord_Ms_)) return false;
            if (false == _s.Read(out markID_)) return false;
            if (false == _s.Read(out costumeID_)) return false;
            if (false == _s.Read(out charID_)) return false;
            if (false == _s.Read(out userLv_)) return false;
            if (false == _s.Read(out charLv_)) return false;
            if (false == _s.Read(out charGrade_)) return false;
            return true;
        }
        public override void Write(Message _s)
        {
            _s.Write(nickName_);
            _s.Write(nickNameColorID_);
            _s.Write(uuid_);
            _s.Write(timeRecord_Ms_);
            _s.Write(markID_);
            _s.Write(costumeID_);
            _s.Write(charID_);
            _s.Write(userLv_);
            _s.Write(charLv_);
            _s.Write(charGrade_);
        }

    };

    public List<Piece> infos_;
    public PktInfoRaidRankingHeader.Piece header_;       // 랭킹 업데이트 시간, 스테이지 ID 정보

    public override bool Read(Message _s)
    {
        if (false == PN_MarshalerEx.ReadList(_s, out infos_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out header_)) return false;
        return true;
    }
    public override void Write(Message _s)
    {
        PN_MarshalerEx.WriteList(_s, infos_);
        PN_MarshalerEx.Write(_s, header_);
    }
};

public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoRaidRankStage.Piece _v)
    {
        _v = new PktInfoRaidRankStage.Piece();

        if (false == _s.Read(out _v.nickName_)) return false;
        if (false == _s.Read(out _v.nickNameColorID_)) return false;
        if (false == _s.Read(out _v.uuid_)) return false;
        if (false == _s.Read(out _v.timeRecord_Ms_)) return false;
        if (false == _s.Read(out _v.markID_)) return false;
        if (false == _s.Read(out _v.costumeID_)) return false;
        if (false == _s.Read(out _v.charID_)) return false;
        if (false == _s.Read(out _v.userLv_)) return false;
        if (false == _s.Read(out _v.charLv_)) return false;
        if (false == _s.Read(out _v.charGrade_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoRaidRankStage.Piece _v)
    {
        _s.Write(_v.nickName_);
        _s.Write(_v.nickNameColorID_);
        _s.Write(_v.uuid_);
        _s.Write(_v.timeRecord_Ms_);
        _s.Write(_v.markID_);
        _s.Write(_v.costumeID_);
        _s.Write(_v.charID_);
        _s.Write(_v.userLv_);
        _s.Write(_v.charLv_);
        _s.Write(_v.charGrade_);
    }


    public static bool Read(Message _s, out PktInfoRaidRankStage _v)
    {
        _v = new PktInfoRaidRankStage();
        if (false == PN_MarshalerEx.ReadList(_s, out _v.infos_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.header_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoRaidRankStage _v)
    {
        PN_MarshalerEx.WriteList(_s, _v.infos_);
        PN_MarshalerEx.Write(_s, _v.header_);
    }

}

    // 타임어택 랭킹 리스트 패킷 메시지
    public class PktInfoTimeAtkRankStageList : PktMsgType
{
    public List<PktInfoTimeAtkRankStage> infos_;    // 스테이지 별 랭킹 유저 리스트
};

// 레이드 랭킹 리스트 패킷 메시지
public class PktInfoRaidRankStageList : PktMsgType
{
    public List<PktInfoRaidRankStage> infos_;    // 스테이지 별 랭킹 유저 리스트
};

// 타임어택 랭킹 등록된 유저의 자세한 정보 요청 패킷 메시지
public class PktInfoTimeAtkRankerDetailReq : PktMsgType
{
    public System.UInt64 uuid_;             // 요청할 기록의 UUID - (클라이언트에서 채워줘야 할 값)
    public System.UInt32 stageID_;          // 요청할 기록의 stageTID - (클라이언트에서 채워줘야 할 값)
};

// 레이드 랭킹 등록된 유저의 자세한 정보 요청 패킷 메시지
public class PktInfoRaidRankerDetailReq : PktMsgType
{
    public System.UInt64 uuid_;             // 요청할 기록의 UUID - (클라이언트에서 채워줘야 할 값)
    public System.UInt32 stageID_;          // 요청할 기록의 stageTID - (클라이언트에서 채워줘야 할 값)
    public System.UInt16 raidLevel_;            // 요청할 기록의 level - (클라이언트에서 채워줘야 할 값)
};

// 타임어택 랭킹 등록된 유저의 자세한 정보 응답 패킷 메시지
public class PktInfoTimeAtkRankerDetailAck : PktMsgType
{
    public PktInfoConPosCharDetail charInfo_;           // 캐릭터 세부 정보
    public PktInfoTimeAtkRankStage.Piece simpleInfo_;   // 기본 정보(닉네임, UUID, 기록, 마크, 유저레벨, 캐릭터ID)
    public System.UInt32 stageID_;                      // 스테이지 ID
};

// 레이드 랭킹 등록된 유저의 자세한 정보 응답 패킷 메시지
public class PktInfoRaidRankerDetailAck : PktMsgType
{
    public PktInfoConPosCharDetail[] charInfo_ = new PktInfoConPosCharDetail[PktInfoStageGameStartReq.RaidCharMax_];           // 캐릭터 세부 정보
    public PktInfoRaidRankStage.Piece simpleInfo_;      // 기본 정보(닉네임, UUID, 기록, 마크, 유저레벨, 캐릭터ID)
    public System.UInt32 stageID_;                      // 스테이지 ID
    public System.UInt16 raidLevel_;                    // 스테이지 ID
    public bool isFirstClearRank_;                      // 레이드 최초 클리어 랭킹 여부
};


// PVP 대전 유저 간단한 정보 패킷 메시지
public class PktInfoArenaUserSimple : PktMsgType
{
    public System.String nickName_;         // 랭킹 유저 닉네임
	public uint nickNameColorID_;           // 랭킹 유저 닉네임 색상 아이디
	public System.UInt64 uuid_;             // 랭킹 유저 UUID
    public System.UInt64 score_;            // 랭킹 유저 점수
    public System.UInt32 markID_;           // 랭킹 유저 마크 ID
    public System.UInt32 cardFrmtID_;       // 카드(서포터) 진형 ID
    public System.UInt32 teamPower_;        // 유저 부대 전투력
    public float teamHP_;                   // 유저 부대 체력
    public float teamAtk_;                  // 유저 부대 공격력
    public System.Byte grade_;              // 랭킹 유저 등급
    public System.Byte tier_;               // 랭킹 유저 티어
    public System.Byte userLv_;             // 랭킹 유저 레벨
    public System.Byte rank_;               // 랭킹 유저 순위
	public ulong circleID_;					// 유저 서클아이디
};

// PVP 대전 유저 문양 정보 패킷 메시지
public class PktInfoArenaUserBadge : PktMsgType
{
    public PktInfoBadge.Option[] opt_ = new PktInfoBadge.Option[(int)PktInfoBadge.Option.ENUM._MAX_];     // 문양 옵션 정보
    public System.Byte lv_;                 // 문양 강화 레벨
};

// PVP 대전 유저 정보 패킷 메시지
public class PktInfoArenaDetail : PktMsgType
{
    public enum eCHAR
    {
        FIRST = 0,
        SECOND,
        THIRD,

        _MAX_
    };

    public PktInfoConPosCharDetail[] charInfos_ = new PktInfoConPosCharDetail[(int)eCHAR._MAX_];            // 캐릭터 정보
    public PktInfoArenaUserBadge[] badgeInfos_ = new PktInfoArenaUserBadge[(int)eBadgeSlotPosMax.ARENA];    // 문양 정보
    public PktInfoArenaUserSimple userInfo_;                                                                // 유저 정보

    // PVP 대전 유저 정보 패킷 메시지
    public override bool Read(Message _s)
    {
        for (int loop = 0; loop < (int)eCHAR._MAX_; ++loop)
        {
            if (false == PN_MarshalerEx.Read(_s, out charInfos_[loop]))
                return false;
        }
        for (int loop = 0; loop < (int)eBadgeSlotPosMax.ARENA; ++loop)
        {
            if (false == PN_MarshalerEx.Read(_s, out badgeInfos_[loop]))
                return false;
        }
        if (false == PN_MarshalerEx.Read(_s, out userInfo_)) return false;
        return true;
    }
    public override void Write(Message _s)
    {
        for (int loop = 0; loop < (int)eCHAR._MAX_; ++loop)
            PN_MarshalerEx.Write(_s, charInfos_[loop]);
        for (int loop = 0; loop < (int)eBadgeSlotPosMax.ARENA; ++loop)
            PN_MarshalerEx.Write(_s, badgeInfos_[loop]);
        PN_MarshalerEx.Write(_s, userInfo_);
    }
};

// PVP 대전 유저 간단한 정보 패킷 메시지
public class PktInfoArenaSimple : PktMsgType
{
    public class CharInfo : PktMsgType
    {
        public System.UInt32 charID_;          // 랭킹 유저가 기록에 사용한 캐릭터 TID
        public System.UInt32 costumeID_;       // 캐릭터 코스튬ID
        public System.Byte charLv_;            // 랭킹 유저가 기록에 사용한 캐릭터 레벨
        public System.Byte charGrade_;         // 랭킹 유저가 기록에 사용한 캐릭터 등급
        public override bool Read(Message _s)
        {
            if (false == _s.Read(out charID_)) return false;
            if (false == _s.Read(out costumeID_)) return false;
            if (false == _s.Read(out charLv_)) return false;
            if (false == _s.Read(out charGrade_)) return false;
            return true;
        }
        public override void Write(Message _s)
        {
            _s.Write(charID_);
            _s.Write(costumeID_);
            _s.Write(charLv_);
            _s.Write(charGrade_);
        }
    };

    public PktInfoArenaUserSimple userInfo_;
    public CharInfo[] charInfos_ = new CharInfo[(int)PktInfoArenaDetail.eCHAR._MAX_];     
    public PktInfoArenaUserBadge[] badgeInfos_ = new PktInfoArenaUserBadge[(int)eBadgeSlotPosMax.ARENA];

    public override bool Read(Message _s)
    {
        if (false == PN_MarshalerEx.Read(_s, out userInfo_)) return false;
        for (int loop = 0; loop < (int)PktInfoArenaDetail.eCHAR._MAX_; ++loop)
        {
            if (false == PN_MarshalerEx.Read(_s, out charInfos_[loop]))
                return false;
        }
        for (int loop = 0; loop < (int)eBadgeSlotPosMax.ARENA; ++loop)
        {
            if (false == PN_MarshalerEx.Read(_s, out badgeInfos_[loop]))
                return false;
        }
        return true;
    }
    public override void Write(Message _s)
    {
        PN_MarshalerEx.Write(_s, userInfo_);
        for (int loop = 0; loop < (int)PktInfoArenaDetail.eCHAR._MAX_; ++loop)
            PN_MarshalerEx.Write(_s, charInfos_[loop]);
        for (int loop = 0; loop < (int)eBadgeSlotPosMax.ARENA; ++loop)
            PN_MarshalerEx.Write(_s, badgeInfos_[loop]);
    }
};

// PVP 랭킹 리스트 패킷 메시지
public class PktInfoArenaRankList : PktMsgType
{
    public List<PktInfoArenaSimple> infos_;     // 랭킹 유저 정보
    public System.UInt64 updateTM_;             // 랭킹을 마지막으로 업데이트 한 시간

    public System.Byte userExSeasonRank_;       // 유저의 지난 시즌 랭킹 - (전 시즌이 끝나고 정리 기간이거나 유저 랭킹이 동기화가 되어있지 않을 때만 가져옵니다.)
};

public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 타임어택 랭킹 리스트 헤더 (랭킹 업데이트 시간, 스테이지 ID) 패킷 메시지
    public static bool Read(Message _s, out PktInfoTimeAtkRankingHeader.Piece _v) {
        _v = new PktInfoTimeAtkRankingHeader.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoTimeAtkRankingHeader.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoTimeAtkRankingHeader _v) {
        _v = new PktInfoTimeAtkRankingHeader();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoTimeAtkRankingHeader _v) {
        WriteList(_s, _v.infos_);
    }
    // 타임어택 랭킹 리스트(하나의 스테이지) 패킷 메시지
    public static bool Read(Message _s, out PktInfoTimeAtkRankStage.Piece _v) {
        _v = new PktInfoTimeAtkRankStage.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoTimeAtkRankStage.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoTimeAtkRankStage _v) {
        _v = new PktInfoTimeAtkRankStage();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoTimeAtkRankStage _v) {
        _v.Write(_s);
    }
    // 타임어택 랭킹 리스트 패킷 메시지
    public static bool Read(Message _s, out PktInfoTimeAtkRankStageList _v) {
        _v = new PktInfoTimeAtkRankStageList();
        if (false == ReadList(_s, out _v.infos_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoTimeAtkRankStageList _v) {
        WriteList(_s, _v.infos_);
    }
    // 타임어택 랭킹 등록된 유저의 자세한 정보 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoTimeAtkRankerDetailReq _v) {
        _v = new PktInfoTimeAtkRankerDetailReq();
        if (false == _s.Read(out _v.uuid_)) return false;
        if (false == _s.Read(out _v.stageID_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoTimeAtkRankerDetailReq _v) {
        Write(_s, _v.uuid_);
        Write(_s, _v.stageID_);
    }
    // 레이드 랭킹 리스트 패킷 메시지
    public static bool Read(Message _s, out PktInfoRaidRankStageList _v)
    {
        _v = new PktInfoRaidRankStageList();
        if (false == ReadList(_s, out _v.infos_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoRaidRankStageList _v)
    {
        WriteList(_s, _v.infos_);
    }
    // 레이드 랭킹 등록된 유저의 자세한 정보 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoRaidRankerDetailReq _v)
    {
        _v = new PktInfoRaidRankerDetailReq();
        if (false == _s.Read(out _v.uuid_)) return false;
        if (false == _s.Read(out _v.stageID_)) return false;
        if (false == _s.Read(out _v.raidLevel_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoRaidRankerDetailReq _v)
    {
        Write(_s, _v.uuid_);
        Write(_s, _v.stageID_);
        Write(_s, _v.raidLevel_);
    }

    // 타임어택 랭킹 등록된 유저의 자세한 정보 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoTimeAtkRankerDetailAck _v) {
        _v = new PktInfoTimeAtkRankerDetailAck();
        if (false == Read(_s, out _v.charInfo_)) return false;
        if (false == Read(_s, out _v.simpleInfo_)) return false;
        if (false == _s.Read(out _v.stageID_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoTimeAtkRankerDetailAck _v) {
        Write(_s, _v.charInfo_);
        Write(_s, _v.simpleInfo_);
        _s.Write(_v.stageID_);
    }

    // 레이드 랭킹 등록된 유저의 자세한 정보 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoRaidRankerDetailAck _v)
    {
        _v = new PktInfoRaidRankerDetailAck();
        for (int loop = 0; loop < (int)PktInfoStageGameStartReq.RaidCharMax_; ++loop)
        {
            if (false == PN_MarshalerEx.Read(_s, out _v.charInfo_[loop])) return false;
        }
        if (false == PN_MarshalerEx.Read(_s, out _v.simpleInfo_)) return false;
        if (false == _s.Read(out _v.stageID_)) return false;
        if (false == _s.Read(out _v.raidLevel_)) return false;
        if (false == _s.Read(out _v.isFirstClearRank_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoRaidRankerDetailAck _v)
    {
        for (int loop = 0; loop < (int)PktInfoStageGameStartReq.RaidCharMax_; ++loop)
        {
            PN_MarshalerEx.Write(_s, _v.charInfo_[loop]);
        }
        PN_MarshalerEx.Write(_s, _v.simpleInfo_);
        _s.Write(_v.stageID_);
        _s.Write(_v.raidLevel_);
        _s.Write(_v.isFirstClearRank_);
    }
    // PVP 대전 유저 간단한 정보 패킷 메시지
    public static bool Read(Message _s, out PktInfoArenaUserSimple _v) {
        _v = new PktInfoArenaUserSimple();
        if (false == _s.Read(out _v.nickName_)) return false;
		if (false == _s.Read(out _v.nickNameColorID_)) return false;
		if (false == _s.Read(out _v.uuid_)) return false;
        if (false == _s.Read(out _v.score_)) return false;
        if (false == _s.Read(out _v.markID_)) return false;
        if (false == _s.Read(out _v.cardFrmtID_)) return false;
        if (false == _s.Read(out _v.teamPower_)) return false;
        if (false == _s.Read(out _v.teamHP_)) return false;
        if (false == _s.Read(out _v.teamAtk_)) return false;
        if (false == _s.Read(out _v.grade_)) return false;
        if (false == _s.Read(out _v.tier_)) return false;
        if (false == _s.Read(out _v.userLv_)) return false;
        if (false == _s.Read(out _v.rank_)) return false;
        if (false == _s.Read(out _v.circleID_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoArenaUserSimple _v) {
        _s.Write(_v.nickName_);
		_s.Write(_v.nickNameColorID_);
		_s.Write(_v.uuid_);
        _s.Write(_v.score_);
        _s.Write(_v.markID_);
        _s.Write(_v.cardFrmtID_);
        _s.Write(_v.teamPower_);
        _s.Write(_v.teamHP_);
        _s.Write(_v.teamAtk_);
        _s.Write(_v.grade_);
        _s.Write(_v.tier_);
        _s.Write(_v.userLv_);
        _s.Write(_v.rank_);
        _s.Write(_v.circleID_);
    }
    // PVP 대전 유저 문양 정보 패킷 메시지
    public static bool Read(Message _s, out PktInfoArenaUserBadge _v) {
        _v = new PktInfoArenaUserBadge();
        for (int loop = 0; loop < (int)PktInfoBadge.Option.ENUM._MAX_; ++loop)
        {
            if (false == Read(_s, out _v.opt_[loop]))
                return false;
        }
        if (false == _s.Read(out _v.lv_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoArenaUserBadge _v) {
        for (int loop = 0; loop < (int)PktInfoBadge.Option.ENUM._MAX_; ++loop)
            PN_MarshalerEx.Write(_s, _v.opt_[loop]);
        _s.Write(_v.lv_);
    }
    // PVP 대전 유저 정보 패킷 메시지
    public static bool Read(Message _s, out PktInfoArenaDetail _v) {
        _v = new PktInfoArenaDetail();
        if (false == _v.Read(_s)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoArenaDetail _v) {
        _v.Write(_s);
    }
    // PVP 대전 유저 정보 패킷 메시지
    public static bool Read(Message _s, out PktInfoArenaSimple.CharInfo _v) {
        _v = new PktInfoArenaSimple.CharInfo();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoArenaSimple.CharInfo _v) {
        _v.Write(_s);
    }
    // PVP 대전 유저 간단한 정보 패킷 메시지
    public static bool Read(Message _s, out PktInfoArenaSimple _v) {
        _v = new PktInfoArenaSimple();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoArenaSimple _v) {
        _v.Write(_s);
    }
    // PVP 랭킹 리스트 패킷 메시지
    public static bool Read(Message _s, out PktInfoArenaRankList _v) {
        _v = new PktInfoArenaRankList();
        if (false == ReadList(_s, out _v.infos_)) return false;
        if (false == _s.Read(out _v.updateTM_)) return false;
        if (false == _s.Read(out _v.userExSeasonRank_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoArenaRankList _v) {
        WriteList(_s, _v.infos_);
        _s.Write(_v.updateTM_);
        _s.Write(_v.userExSeasonRank_);
    }
}