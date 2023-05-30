using System.Collections.Generic;
using Nettention.Proud;


// PVP 관련 문서 참고 -> http://10.10.10.90/Mediawiki/index.php/Project-A/일반/PVP_모드
// PVP 대전 유저 팀 편성 패킷 메세지
public class PktInfoUserArenaTeam : PktMsgType
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
// PVP 대전 유저 기록 패킷 메세지
public class PktInfoUserArenaRec : PktMsgType
{
    public PktInfoTime lastRewardTime_;     // 보상을 받은 시즌이 끝나는 시간
    public System.UInt64 nowScore_;         // 현재 점수
    public System.UInt64 sr_BestScore_;     // 시즌 최고 점수
    public System.UInt32 sr_BestWinCnt_;    // 시즌 최고로 많이 연승한 수
    public System.UInt32 sr_TotalCnt_;      // 시즌 전체 게임 수
    public System.UInt32 sr_FirstWinCnt_;   // 시즌 선봉 승리 수
    public System.UInt32 sr_SecondWinCnt_;  // 시즌 중견 승리 수
    public System.UInt32 sr_ThirdWinCnt_;   // 시즌 대장 승리 수
    public System.UInt32 nowGradeID_;       // 현재 등급 ID
    public System.Int32 nowWinLoseCnt_;     // 현재 연승(연패) 수
    public System.UInt32 cheatValue_;       // 아레나 부정행위값
    public System.Byte promotionRemainCnt_; // 현재 승급전 남은 횟수(승급전 진행중이 아니라면 0)
    public System.Byte promotionWinCnt_;    // 승급전 이긴 횟수
    public System.Byte exSeasonRank_;       // 이전 시즌 유저 등수(등수안에 못 들었을 경우 0) - 시즌 종료시에만 사용합니다.
    public System.Byte cheatType_;          // 아레나 부정행위 타입
};

// PVP 대전 유저 정보 패킷 메세지
public class PktInfoUserArena : PktMsgType
{
    public PktInfoUserArenaTeam team_;      // 팀 데이터
    public PktInfoUserArenaRec record_;     // 기록 데이터
};

// PVP 대전 시작 요청 패킷 메세지
public class PktInfoArenaGameStartReq : PktMsgType
{
    public List<System.UInt64> useItemUIDs_;    // 유저가 사용한 버프 아이템 UID - (클라이언트에서 채워줘야 할 값)
    public System.Boolean useBattleCoinBuff_;   // 배틀 코인 버프 사용 여부 - (클라이언트에서 채워줘야 할 값)
    public System.Boolean upCharSKBuffFlag_;    // 캐릭터 스킬 확장 버프 플레그 - (클라이언트에서 채워줘야 할 값)
};
// PVP 대전 시작 응답 패킷 메세지
public class PktInfoArenaGameStartAck : PktMsgType
{
    public System.Guid certifyKey_;             // 스테이지 인증키
    public PktInfoConsumeItem useItemInfos_;    // 유저가 사용한 버프 아이템 UID
    public System.UInt64 userGold_;             // 배틀 코인 버프 사용한 결과 변경된 골드
};
// PVP 대전 종료 요청 패킷 메세지
public class PktInfoArenaGameEndReq : PktMsgType
{
    public enum eRESULT
    {
        LOSE = 0,

        FIRST_WIN = 1,
        SECOND_WIN = 2,
        THIRD_WIN = 3,

        _MAX_
    };

    public System.Guid certifyKey_;             // 스테이지 인증키 - (클라이언트에서 채워줘야 할 값)
    public System.UInt32 playTime_Ms_;          // 플레이 시간 밀리세컨드 단위 - (클라이언트에서 채워줘야 할 값)
    public System.UInt32 teamPower_;            // 유저 전투력 - (클라이언트에서 채워줘야 할 값)
    public float teamHP_;                       // 유저 체력 - (클라이언트에서 채워줘야 할 값)
    public float teamAtk_;                      // 유저 공격력 - (클라이언트에서 채워줘야 할 값)
    public System.UInt32 enemyTeamPower_;       // 상대 유저 전투력 - (클라이언트에서 채워줘야 할 값)
    public System.UInt32 enemyScore_;           // 상대 유저 배틀 점수 - (클라이언트에서 채워줘야 할 값)
    public System.UInt32 maxDamage_;            // 최대 데미지 - (클라이언트에서 채워줘야 할 값)
    public System.Byte result_;                 // 게임 결과 - (클라이언트에서 채워줘야 할 값)
};
// PVP 대전 종료 응답 패킷 메세지
public class PktInfoArenaGameEndAck : PktMsgType
{
    public PktInfoUserArenaRec record_;         // 대전 종료 결과 변경된 유저 PVP 기록
    public PktInfoGoodsAll userGoods_;          // 대전 종료 졀과 변경된 유저 재화
    public PktInfoTime BPNextTime_;             // 다음 티켓 회복 시간(BP의 회복시간을 나타냅니다.)
};

// PVP 상대 유저 정보 패킷 메세지
public class PktinfoArenaEnemy : PktMsgType
{
    public class CharInfo : PktMsgType
    {
        public List<PktInfoConPosCharDetail.ASkill> askls_; // 각성 스킬 정보
        public PktInfoConPosCharDetail.ComInfo[] weapon_ = new PktInfoConPosCharDetail.ComInfo[(int)PktInfoChar.WpnSlot.Enum._MAX_];   // 캐릭터 무기 정보
        public PktInfoConPosCharDetail.CardInfo[] card_ = new PktInfoConPosCharDetail.CardInfo[(int)eCardSlotPosMax.CHAR];             // 캐릭터 서포터 정보
        public System.UInt32[] skillIDs_        = new System.UInt32[(int)PktInfoChar.SkillSlot._MAX_];    // 캐릭터 스킬 ID
        public System.UInt32 charID_;           // 캐릭터 TID
        public System.UInt32 costumeID_;        // 캐릭터 코스튬 TID
        public System.Byte charLv_;             // 캐릭터 레벨
        public System.Byte charGrade_;          // 캐릭터 등급
        public System.Byte costumeClr_;         // 캐릭터 코스튬 색
        public PktInfoConPosCharDetail.CostumeDyeing costumeDyeing_; // 염색 정보

        public override bool Read(Message _s) {
            if (false == PN_MarshalerEx.ReadList(_s, out askls_)) return false;
            for (int loop = 0; loop < (int)PktInfoChar.WpnSlot.Enum._MAX_; ++loop)
            {
                if (false == PN_MarshalerEx.Read(_s, out weapon_[loop]))
                    return false;
            }
            for (int loop = 0; loop < (int)eCardSlotPosMax.CHAR; ++loop)
            {
                if (false == PN_MarshalerEx.Read(_s, out card_[loop]))
                    return false;
            }
            for (int loop = 0; loop < (int)PktInfoChar.SkillSlot._MAX_; ++loop)
            {
                if (false == _s.Read(out skillIDs_[loop]))
                    return false;
            }
            if (false == _s.Read(out charID_)) return false;
            if (false == _s.Read(out costumeID_)) return false;
            if (false == _s.Read(out charLv_)) return false;
            if (false == _s.Read(out charGrade_)) return false;
            if (false == _s.Read(out costumeClr_)) return false;
            if (false == PN_MarshalerEx.Read(_s, out costumeDyeing_)) return false;

            return true;
        }
        public override void Write(Message _s) {
            PN_MarshalerEx.WriteList(_s, askls_);
            for (int loop = 0; loop < (int)PktInfoChar.WpnSlot.Enum._MAX_; ++loop)
                PN_MarshalerEx.Write(_s, weapon_[loop]);
            for (int loop = 0; loop < (int)eCardSlotPosMax.CHAR; ++loop)
                PN_MarshalerEx.Write(_s, card_[loop]);
            for (int loop = 0; loop < (int)PktInfoChar.SkillSlot._MAX_; ++loop)
                _s.Write(skillIDs_[loop]);
            _s.Write(charID_);
            _s.Write(costumeID_);
            _s.Write(charLv_);
            _s.Write(charGrade_);
            _s.Write(costumeClr_);
            PN_MarshalerEx.Write(_s, costumeDyeing_);
        }
    };
    public PktInfoArenaUserSimple userInfo_;        // 유저 관련 데이터
    public CharInfo[] charInfos_                = new CharInfo[(int)PktInfoArenaDetail.eCHAR._MAX_];        // 캐릭터 관련 데이터 
    public PktInfoArenaUserBadge[] badgeInfos_  = new PktInfoArenaUserBadge[(int)eBadgeSlotPosMax.ARENA];   // 문양 관련 데이터
};

// PVP 대전 상대 검색 응답 패킷 메세지
public class PktInfoArenaSearchEnemyAck : PktMsgType
{
    public PktinfoArenaEnemy enemyInfo_;        // 대전 상대 정보
    public System.UInt64 userGold_;             // 대전 상대 검색 결과 변경된 골드
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // PVP 대전 유저 팀 편성 패킷 메세지
    public static bool Read(Message _s, out PktInfoUserArenaTeam _v) {
        _v = new PktInfoUserArenaTeam();
        for (int loop = 0; loop < (int)PktInfoUserArenaTeam.eCHAR._MAX_; ++loop)
        {
            if (false == _s.Read(out _v.CUIDs_[loop]))  return false;
        }
        if (false == _s.Read(out _v.cardFrmtID_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoUserArenaTeam _v) {
        for (int loop = 0; loop < (int)PktInfoUserArenaTeam.eCHAR._MAX_; ++loop)
            _s.Write(_v.CUIDs_[loop]);
        _s.Write(_v.cardFrmtID_);
    }
    // PVP 대전 유저 기록 패킷 메세지
    public static bool Read(Message _s, out PktInfoUserArenaRec _v)
    {
        _v = new PktInfoUserArenaRec();
        if (false == Read(_s, out _v.lastRewardTime_)) return false;
        if (false == _s.Read(out _v.nowScore_)) return false;
        if (false == _s.Read(out _v.sr_BestScore_)) return false;
        if (false == _s.Read(out _v.sr_BestWinCnt_)) return false;
        if (false == _s.Read(out _v.sr_TotalCnt_)) return false;
        if (false == _s.Read(out _v.sr_FirstWinCnt_)) return false;
        if (false == _s.Read(out _v.sr_SecondWinCnt_)) return false;
        if (false == _s.Read(out _v.sr_ThirdWinCnt_)) return false;
        if (false == _s.Read(out _v.nowGradeID_)) return false;
        if (false == _s.Read(out _v.nowWinLoseCnt_)) return false;
        if (false == _s.Read(out _v.cheatValue_)) return false;
        if (false == _s.Read(out _v.promotionRemainCnt_)) return false;
        if (false == _s.Read(out _v.promotionWinCnt_)) return false;
        if (false == _s.Read(out _v.exSeasonRank_)) return false;
        if (false == _s.Read(out _v.cheatType_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoUserArenaRec _v)
    {
        Write(_s, _v.lastRewardTime_);
        _s.Write(_v.nowScore_);
        _s.Write(_v.sr_BestScore_);
        _s.Write(_v.sr_BestWinCnt_);
        _s.Write(_v.sr_TotalCnt_);
        _s.Write(_v.sr_FirstWinCnt_);
        _s.Write(_v.sr_SecondWinCnt_);
        _s.Write(_v.sr_ThirdWinCnt_);
        _s.Write(_v.nowGradeID_);
        _s.Write(_v.nowWinLoseCnt_);
        _s.Write(_v.cheatValue_);
        _s.Write(_v.promotionRemainCnt_);
        _s.Write(_v.promotionWinCnt_);
        _s.Write(_v.exSeasonRank_);
		_s.Write(_v.cheatType_);
}
    // PVP 대전 유저 정보 패킷 메세지
    public static bool Read(Message _s, out PktInfoUserArena _v) {
        _v = new PktInfoUserArena();
        if (false == Read(_s, out _v.team_)) return false;
        if (false == Read(_s, out _v.record_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoUserArena _v) {
        Write(_s, _v.team_);
        Write(_s, _v.record_);
    }
    // PVP 대전 시작 요청 패킷 메세지
    public static bool Read(Message _s, out PktInfoArenaGameStartReq _v) {
        _v = new PktInfoArenaGameStartReq();
        if (false == Read(_s, out _v.useItemUIDs_)) return false;
        if (false == _s.Read(out _v.useBattleCoinBuff_)) return false;
        if (false == _s.Read(out _v.upCharSKBuffFlag_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoArenaGameStartReq _v) {
        Write(_s, _v.useItemUIDs_);
        Write(_s, _v.useBattleCoinBuff_);
        Write(_s, _v.upCharSKBuffFlag_);
    }
    // PVP 대전 시작 응답 패킷 메세지
    public static bool Read(Message _s, out PktInfoArenaGameStartAck _v) {
        _v = new PktInfoArenaGameStartAck();
        if (false == _s.Read(out _v.certifyKey_)) return false;
        if (false == Read(_s, out _v.useItemInfos_)) return false;
        if (false == _s.Read(out _v.userGold_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoArenaGameStartAck _v) {
        _s.Write(_v.certifyKey_);
        Write(_s, _v.useItemInfos_);
        _s.Write(_v.userGold_);
    }
    // PVP 대전 종료 요청 패킷 메세지
    public static bool Read(Message _s, out PktInfoArenaGameEndReq _v) {
        _v = new PktInfoArenaGameEndReq();
        if (false == _s.Read(out _v.certifyKey_)) return false;
        if (false == _s.Read(out _v.playTime_Ms_)) return false;
        if (false == _s.Read(out _v.teamPower_)) return false;
        if (false == _s.Read(out _v.teamHP_)) return false;
        if (false == _s.Read(out _v.teamAtk_)) return false;
        if (false == _s.Read(out _v.enemyTeamPower_)) return false;
        if (false == _s.Read(out _v.enemyScore_)) return false;
        if (false == _s.Read(out _v.maxDamage_)) return false;
        if (false == _s.Read(out _v.result_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoArenaGameEndReq _v) {
        _s.Write(_v.certifyKey_);
        _s.Write(_v.playTime_Ms_);
        _s.Write(_v.teamPower_);
        _s.Write(_v.teamHP_);
        _s.Write(_v.teamAtk_);
        _s.Write(_v.enemyTeamPower_);
        _s.Write(_v.enemyScore_);
        _s.Write(_v.maxDamage_);
        _s.Write(_v.result_);
    }
    // PVP 대전 종료 응답 패킷 메세지
    public static bool Read(Message _s, out PktInfoArenaGameEndAck _v) {
        _v = new PktInfoArenaGameEndAck();
        if (false == Read(_s, out _v.record_)) return false;
        if (false == Read(_s, out _v.userGoods_)) return false;
        if (false == Read(_s, out _v.BPNextTime_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoArenaGameEndAck _v) {
        Write(_s, _v.record_);
        Write(_s, _v.userGoods_);
        Write(_s, _v.BPNextTime_);
    }
    // PVP 대전 상대 검색 응답 패킷 메세지
    public static bool Read(Message _s, out PktInfoArenaSearchEnemyAck _v) {
        _v = new PktInfoArenaSearchEnemyAck();
        if (false == Read(_s, out _v.enemyInfo_)) return false;
        if (false == _s.Read(out _v.userGold_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoArenaSearchEnemyAck _v) {
        Write(_s, _v.enemyInfo_);
        Write(_s, _v.userGold_);
    }
    // PVP 상대 유저 정보 패킷 메세지
    public static bool Read(Message _s, out PktinfoArenaEnemy.CharInfo _v) {
        _v = new PktinfoArenaEnemy.CharInfo();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktinfoArenaEnemy.CharInfo _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktinfoArenaEnemy _v) {
        _v = new PktinfoArenaEnemy();
        if (false == Read(_s, out _v.userInfo_)) return false;
        for (int loop = 0; loop < (int)PktInfoArenaDetail.eCHAR._MAX_; ++loop)
        {
            if (false == Read(_s, out _v.charInfos_[loop]))
                return false;
        }
        for (int loop = 0; loop < (int)eBadgeSlotPosMax.ARENA; ++loop)
        {
            if (false == Read(_s, out _v.badgeInfos_[loop]))
                return false;
        }
        return true;
    }
    public static void Write(Message _s, PktinfoArenaEnemy _v) {
        Write(_s, _v.userInfo_);
        for (int loop = 0; loop < (int)PktInfoArenaDetail.eCHAR._MAX_; ++loop)
            Write(_s, _v.charInfos_[loop]);
        for (int loop = 0; loop < (int)eBadgeSlotPosMax.ARENA; ++loop)
            Write(_s, _v.badgeInfos_[loop]);
    }
}

// 유저 아레나 타워 정보 패킷 메시지
public class PktInfoUserArenaTower : PktMsgType
{
    public class Info : PktMsgType
    {
        public System.UInt32 claerID_;          // 클리어한 아레나 타워 ID

        public override bool Read(Message _s) {
            if (false == _s.Read(out claerID_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(claerID_);
        }
    };
    public PktInfoUserArenaTeam team_;      // 팀 데이터
    public Info info_;                      // 기록 데이터
};
// 아레나 타워 시작 요청 패킷 메시지
public class PktInfoArenaTowerGameStartReq : PktMsgType
{
    public System.UInt64 useCommuUuid_;     // 사용한 커뮤니티 유저 고유 ID - (클라이언트에서 채워줘야 할 값)
    public System.UInt32 towerID_;          // 진행할 아레나 타워 ID - (클라이언트에서 채워줘야 할 값)
    public System.Boolean upCharSKBuffFlag_;    // 캐릭터 스킬 확장 버프 플레그 - (클라이언트에서 채워줘야 할 값)
};
// 아레나 타워 종료 요청 패킷 메시지
public class PktInfoArenaTowerGameEndReq : PktMsgType
{
    public System.UInt32 playTime_Ms_;      // 플레이 시간 밀리세컨드 단위 - (클라이언트에서 채워줘야 할 값)
    public System.Boolean successFlag_;     // 아레나 타워 클리어 성공 여부 결과 - (클라이언트에서 채워줘야 할 값)
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 유저 아레나 타워 정보 패킷 메시지
    public static bool Read(Message _s, out PktInfoUserArenaTower _v) {
        _v = new PktInfoUserArenaTower();
        if (false == Read(_s, out _v.team_)) return false;
        if (false == Read(_s, out _v.info_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoUserArenaTower _v) {
        Write(_s, _v.team_);
        Write(_s, _v.info_);
    }
    public static bool Read(Message _s, out PktInfoUserArenaTower.Info _v) {
        _v = new PktInfoUserArenaTower.Info();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoUserArenaTower.Info _v) {
        _v.Write(_s);
    }
    // 아레나 타워 시작 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoArenaTowerGameStartReq _v) {
        _v = new PktInfoArenaTowerGameStartReq();
        if (false == _s.Read(out _v.useCommuUuid_)) return false;
        if (false == _s.Read(out _v.towerID_)) return false;
        if (false == _s.Read(out _v.upCharSKBuffFlag_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoArenaTowerGameStartReq _v) {
        _s.Write(_v.useCommuUuid_);
        _s.Write(_v.towerID_);
        _s.Write(_v.upCharSKBuffFlag_);
    }
    // 아레나 타워 종료 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoArenaTowerGameEndReq _v) {
        _v = new PktInfoArenaTowerGameEndReq();
        if (false == _s.Read(out _v.playTime_Ms_)) return false;
        if (false == _s.Read(out _v.successFlag_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoArenaTowerGameEndReq _v) {
        _s.Write(_v.playTime_Ms_);
        _s.Write(_v.successFlag_);
    }
}