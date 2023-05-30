using System.Collections.Generic;
using Nettention.Proud;


// 미션 패킷 메시지
public class PktInfoMission : PktMsgType
{
    // 일일 미션 패킷 메시지
    public class Daily : PktMsgType
    {
        public class Piece : PktMsgType
        {
            public enum ENUM : System.Byte {

                _NO_START_  = 0,
                _0          = _NO_START_,
                _1,
                _2,
                _3,
                _4,
                _5,
                _6,
                _7,
                _8,
                _9,
                _NO_END_,

                _DAY_START_ = _NO_END_,
                DAY_1       = _DAY_START_,
                _DAY_END_,

                _COM_START_ = _DAY_END_,
                COM_1       = _COM_START_,
                _COM_END_,

                _MAX_   = _COM_END_,
            };
            public PktInfoTime startTM_;                // 시작 시간
            public PktInfoTime endTM_;                  // 종료 시간
            public System.UInt32 groupID_;              // 일일 미션 그룹 ID
            public System.UInt32 rwdFlag_;              // 각각의 보상 획득 여부
            public System.Byte[] noVal_ = new System.Byte[(int)ENUM._NO_END_];  // 미션별 수행 값
            public System.Byte day_;                    // 일일 미션 일차

            public override bool Read(Message _s) {
                if (false == PN_MarshalerEx.Read(_s, out startTM_)) return false;
                if (false == PN_MarshalerEx.Read(_s, out endTM_)) return false;
                if (false == _s.Read(out groupID_)) return false;
                if (false == _s.Read(out rwdFlag_)) return false;
                for (int loop = 0; loop < (int)ENUM._NO_END_; ++loop)
                {
                    if (false == _s.Read(out noVal_[loop]))
                        return false;
                }
                if (false == _s.Read(out day_)) return false;
                return true;
            }
            public override void Write(Message _s) {
                PN_MarshalerEx.Write(_s, startTM_);
                PN_MarshalerEx.Write(_s, endTM_);
                _s.Write(groupID_);
                _s.Write(rwdFlag_);
                for (int loop = 0; loop < (int)ENUM._NO_END_; ++loop)
                    _s.Write(noVal_[loop]);
                _s.Write(day_);
            }
        };

        public List<Piece> infos_;

        public override bool Read(Message _s) {
            if (false == PN_MarshalerEx.ReadList(_s, out infos_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            PN_MarshalerEx.WriteList(_s, infos_);
        }
    };

    // 주간 미션 패킷 메시지
    public class Weekly : PktMsgType
    {
        public enum ENUM : System.Byte {

            _START_MSION_   = 0,
            MSION_1         = _START_MSION_,
            MSION_2,
            MSION_3,
            MSION_4,
            MSION_5,
            MSION_6,
            MSION_7,
            _END_MSION_,

            _START_EX_      = _END_MSION_,
            EX_1            = _START_EX_,
            EX_2,
            EX_3,
            _END_EX_,

            _MAX_           = _END_EX_,
        };
        public PktInfoComWeekMission comInfo_;      // 주간 미션 공용 정보
        public System.UInt32 rewardFlag_;           // 각각의 보상 획득 여부
        public System.UInt32[] condiVal_ = new System.UInt32[(int)ENUM._END_MSION_];    // 미션별 수행 값

        // 원하는 보상을 획득했는지 확인합니다.
        public bool IsOnReward(int _flagIdx /*PktInfoMission.Weekly.ENUM*/) {
            if (32 <= _flagIdx) return false;
            return _IsOnBitIdx(rewardFlag_, (uint)_flagIdx);
        }
        // 원하는 보상 획득했다는 값을 활성화 합니다.
        public void DoOnReward(int _flagIdx /*PktInfoMission.Weekly.ENUM*/) {
            if (32 <= _flagIdx) return;
            _DoOnBitIdx(ref rewardFlag_, (uint)_flagIdx);
        }

        public System.String GetStr() {
            System.String str = string.Format("{0} {1} {2}", comInfo_.GetStr(), this.GetRewardFlagStr(), this.GetCondiValStr());
            return str;
        }
        public System.String GetRewardFlagStr() {
            System.String log = string.Format("Reward - ");
            for (int loop = 0; loop < (int)PktInfoMission.Weekly.ENUM._MAX_; ++loop) {
                if (true == this.IsOnReward(loop))
                    log += string.Format("Get_{0} ", loop);
            }
            return log;
        }
        public System.String GetCondiValStr() {
            System.String log = string.Format("Condi - ");
            for (int loop = (int)PktInfoMission.Weekly.ENUM._START_MSION_; loop < (int)PktInfoMission.Weekly.ENUM._END_MSION_; ++loop) {
                log += string.Format("Val_{0}:{1} ", loop, condiVal_[loop]);
            }
            return log;
        }

        public override bool Read(Message _s) {
            if (false == PN_MarshalerEx.Read(_s, out comInfo_)) return false;
            if (false == _s.Read(out rewardFlag_)) return false;
            for (int loop = 0; loop < (int)ENUM._END_MSION_; ++loop) {
                if (false == _s.Read(out condiVal_[loop]))
                    return false;
            }
            return true;
        }
        public override void Write(Message _s) {
            PN_MarshalerEx.Write(_s, comInfo_);
            _s.Write(rewardFlag_);
            for (int loop = 0; loop < (int)ENUM._END_MSION_; ++loop)
                _s.Write(condiVal_[loop]);
        }
    };
    // 서버 달성(세력) 미션 패킷 메시지
    public class Influ : PktMsgType
    {
        public enum ENUM : System.Byte {

            _NO_START_  = 0,
            _0          = _NO_START_,
            _1,
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
            _NO_END_,
        };
        public enum TARGET : System.Byte {

            _SINGLE_START_  = 0,
            _S_0          = _SINGLE_START_,
            _S_1,
            _S_2,
            _S_3,
            _S_4,
            _S_5,
            _S_6,
            _S_7,
            _S_8,
            _S_9,
            _S_10,
            _S_11,
            _S_12,
            _S_13,
            _S_14,
            _SINGLE_END_,

            _ALL_START_ = _SINGLE_END_,
            _A_0        = _ALL_START_,
            _A_1,
            _A_2,
            _A_3,
            _A_4,
            _A_5,
            _A_6,
            _A_7,
            _A_8,
            _A_9,
            _A_10,
            _A_11,
            _A_12,
            _A_13,
            _A_14,
            _ALL_END_,

            _RANK_START_    = 30,
            _R_0_       = _RANK_START_,
            _RANK_END_
        };
        public System.UInt32 groupID_;          // 서버 달성 미션 테이블 ID
        public System.UInt32 influID_;          // 세력 ID
        public System.UInt32 rwdFlag_;          // 각각의 미션별 보상 획득 여부
        public System.UInt32 tgtRwdFlag_;       // 각각의 달성 및 랭킹 보상 획득 여부
        public System.Byte[] val_ = new System.Byte[(int)ENUM._NO_END_];    // 미션별 수행 값

        public override bool Read(Message _s) {
            if (false == _s.Read(out groupID_)) return false;
            if (false == _s.Read(out influID_)) return false;
            if (false == _s.Read(out rwdFlag_)) return false;
            if (false == _s.Read(out tgtRwdFlag_)) return false;
            for (int loop = 0; loop < (int)ENUM._NO_END_; ++loop) {
                if (false == _s.Read(out val_[loop]))
                    return false;
            }
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(groupID_);
            _s.Write(influID_);
            _s.Write(rwdFlag_);
            _s.Write(tgtRwdFlag_);
            for (int loop = 0; loop < (int)ENUM._NO_END_; ++loop)
                _s.Write(val_[loop]);
        }
    };

    // 게릴라 미션 패킷 메시지
    public class Guerrilla : PktMsgType
    {
        public class Piece : PktMsgType
        {
            public System.UInt32 groupID_;
            public System.UInt32 count_;
            public System.Byte step_;

            public override bool Read(Message _s) {
                if (false == _s.Read(out groupID_)) return false;
                if (false == _s.Read(out count_)) return false;
                if (false == _s.Read(out step_)) return false;
                return true;
            }
            public override void Write(Message _s) {
                _s.Write(groupID_);
                _s.Write(count_);
                _s.Write(step_);
            }
        };

        public List<Piece> infos_;

        public override bool Read(Message _s) {
            if (false == PN_MarshalerEx.ReadList(_s, out infos_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            PN_MarshalerEx.WriteList(_s, infos_);
        }
    };

    // 패스 미션 패킷 메시지
    public class Pass : PktMsgType
    {
        public class Piece : PktMsgType
        {
            public System.UInt32 missionID_;
            public System.UInt32 value_;
            public System.Byte state_;

            public override bool Read(Message _s) {
                if (false == _s.Read(out missionID_)) return false;
                if (false == _s.Read(out value_)) return false;
                if (false == _s.Read(out state_)) return false;
                return true;
            }
            public override void Write(Message _s) {
                _s.Write(missionID_);
                _s.Write(value_);
                _s.Write(state_);
            }
        };

        public List<Piece> infos_;

        public override bool Read(Message _s) {
            if (false == PN_MarshalerEx.ReadList(_s, out infos_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            PN_MarshalerEx.WriteList(_s, infos_);
        }
    };
    public Daily daily_ = new Daily();
    public Weekly weekly_ = new Weekly();
    public Influ influ_ = new Influ();
    public Guerrilla guerrilla_ = new Guerrilla();
    public Pass pass_ = new Pass();
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 일일 미션 패킷 메시지
    public static bool Read(Message _s, out PktInfoMission.Daily _v) {
        _v = new PktInfoMission.Daily();
        if (false == _v.Read(_s)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoMission.Daily _v) {
        _v.Write(_s);
    }
    // 주간 미션 패킷 메시지
    public static bool Read(Message _s, out PktInfoMission.Weekly _v) {
        _v = new PktInfoMission.Weekly();
        if (false == _v.Read(_s)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoMission.Weekly _v) {
        _v.Write(_s);
    }
    // 서버 달성(세력) 미션 패킷 메시지
    public static bool Read(Message _s, out PktInfoMission.Influ _v) {
        _v = new PktInfoMission.Influ();
        if (false == _v.Read(_s)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoMission.Influ _v) {
        _v.Write(_s);
    }
    // 게릴라 미션 패킷 메시지
    public static bool Read(Message _s, out PktInfoMission.Guerrilla _v) {
        _v = new PktInfoMission.Guerrilla();
        if (false == _v.Read(_s)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoMission.Guerrilla _v) {
        _v.Write(_s);
    }
    // 게릴라 미션 패킷 메시지
    public static bool Read(Message _s, out PktInfoMission.Pass _v) {
        _v = new PktInfoMission.Pass();
        if (false == _v.Read(_s)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoMission.Pass _v) {
        _v.Write(_s);
    }
    // 미션 패킷 메시지
    public static bool Read(Message _s, out PktInfoMission _v) {
        _v = new PktInfoMission();
        if (false == _v.daily_.Read(_s)) return false;
        if (false == _v.weekly_.Read(_s)) return false;
        if (false == _v.influ_.Read(_s)) return false;
        if (false == _v.guerrilla_.Read(_s)) return false;
        if (false == _v.pass_.Read(_s)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoMission _v) {
        _v.daily_.Write(_s);
        _v.weekly_.Write(_s);
        _v.influ_.Write(_s);
        _v.guerrilla_.Write(_s);
        _v.pass_.Write(_s);
    }
}

//////////////////////////////////////////////////////////////////////////
///
// 일일 미션 보상 획득 요청 패킷 메시지
public class PktInfoRwdDailyMissionReq : PktMsgType
{
    public PktInfoComRwd idxs_;             // 보상을 수령할 인덱스 목록
    public System.UInt32 groupID_;          // 보상을 요청할 일일 미션 그룹 ID
    public System.Byte day_;                // 보상을 요청할 일일 미션 날 수(이벤트 달성 보상은 1 설정합니다.)
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 일일 미션 보상 획득 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoRwdDailyMissionReq _v) {
        _v = new PktInfoRwdDailyMissionReq();
        if (false == Read(_s, out _v.idxs_)) return false;
        if (false == _s.Read(out _v.groupID_)) return false;
        return _s.Read(out _v.day_);
    }
    public static void Write(Message _s, PktInfoRwdDailyMissionReq _v) {
        Write(_s, _v.idxs_);
        _s.Write(_v.groupID_);
        _s.Write(_v.day_);
    }
}
///
//////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////
///
// 유저 서버 달성(세력) 이벤트 변경 목록 패킷 메시지
public class PktInfoInfluChangeList : PktMsgType
{
    public class Piece : PktMsgType
    {
        public PktInfoMission.Influ addInfo_;   // 활성화된 패스 미션 정보
        public System.UInt64 uuid_;             // 유저 고유 ID
        public eTMIErrNum err_;                 // 에러 번호

        public override bool Read(Message _s) {
            if (false == _s.Read(out uuid_)) return false;
			if (false == PN_MarshalerEx.Read(_s, out addInfo_)) return false;
			if (false == PN_MarshalerEx.Read(_s, out err_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(uuid_);
			PN_MarshalerEx.Write(_s, addInfo_);
			PN_MarshalerEx.Write(_s, err_);
        }
    };
    public List<Piece> infos_;
};
public class PktInfoRwdInfluMissionAck : PktMsgType
{
    public PktInfoRwdItem item_;            // 달성 포인트 아이템
    public System.UInt32 retRwdFlag_;       // 보상 결과가 적용된 보상 플레그 값
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 유저 서버 달성(세력) 이벤트 변경 목록 패킷 메시지
    public static bool Read(Message _s, out PktInfoInfluChangeList.Piece _v) {
        _v = new PktInfoInfluChangeList.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoInfluChangeList.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoInfluChangeList _v) {
        _v = new PktInfoInfluChangeList();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoInfluChangeList _v) {
        WriteList(_s, _v.infos_);
    }
    // 서버 달성(세력) 미션 보상 획득 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoRwdInfluMissionAck _v) {
        _v = new PktInfoRwdInfluMissionAck();
        if (false == Read(_s, out _v.item_)) return false;
        if (false == _s.Read(out _v.retRwdFlag_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoRwdInfluMissionAck _v) {
        Write(_s, _v.item_);
        _s.Write(_v.retRwdFlag_);
    }
}
///
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
///
// 게릴라 미션 갱신 요청 패킷 메시지
public class PktInfoUpdateGllaMission : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt32 count_;        // 추가 갱신될 미션 카운팅 값
        public System.Byte type_;           // 게릴라 미션 타입 - eGuerrillaMissionType

        public override bool Read(Message _s) {
            if (false == _s.Read(out count_)) return false;
            if (false == _s.Read(out type_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(count_);
            _s.Write(type_);
        }
    };
    public List<Piece> infos_;
};
// 게릴라 미션 보상 획득 요청 패킷 메시지
public class PktInfoRwdGllaMission : PktMsgType
{
    public PktInfoTIDList groupIDs_;        // 획득을 요청할 그룹 ID
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 게릴라 미션 갱신 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoUpdateGllaMission.Piece _v) {
        _v = new PktInfoUpdateGllaMission.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoUpdateGllaMission.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoUpdateGllaMission _v) {
        _v = new PktInfoUpdateGllaMission();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoUpdateGllaMission _v) {
        WriteList(_s, _v.infos_);
    }
    // 게릴라 미션 보상 획득 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoRwdGllaMission _v) {
        _v = new PktInfoRwdGllaMission();
        return Read(_s, out _v.groupIDs_);
    }
    public static void Write(Message _s, PktInfoRwdGllaMission _v) {
        Write(_s, _v.groupIDs_);
    }
}
///
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
///
// 유저 패스 변경 목록 패킷 메시지
public class PktInfoPassChangeList : PktMsgType
{
    public class Piece : PktMsgType
    {
        public PktInfoTIDList endMsionList_;    // 종료된 패스 미션 ID 목록
        public PktInfoPass pass_;               // 추가된 및 변경된 패스 정보
        public PktInfoMission.Pass addMsion_;   // 활성화된 패스 미션 정보
        public System.UInt64 uuid_;             // 유저 고유 ID
        public eTMIErrNum err_;                 // 에러 번호

        public override bool Read(Message _s) {
            if (false == PN_MarshalerEx.Read(_s, out endMsionList_)) return false;
            if (false == PN_MarshalerEx.Read(_s, out pass_)) return false;
            if (false == PN_MarshalerEx.Read(_s, out addMsion_)) return false;
            if (false == _s.Read(out uuid_)) return false;
            if (false == PN_MarshalerEx.Read(_s, out err_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            PN_MarshalerEx.Write(_s, endMsionList_);
            PN_MarshalerEx.Write(_s, pass_);
            PN_MarshalerEx.Write(_s, addMsion_);
            _s.Write(uuid_);
            PN_MarshalerEx.Write(_s, err_);
        }
    };
    public List<Piece> infos_;
};
// 패스 미션 보상 획득 요청 패킷 메시지
public class PktInfoRwdPassMission : PktMsgType
{
    public PktInfoTIDList ids_;             // 획득을 요청할 그룹 ID - (클라이언트에서 채워줘야 할 값)
};
// 패스 보상 획득 요청 패킷 메시지
public class PktInfoRwdPassReq : PktMsgType
{
    public System.UInt32 passID_;           // 보상 획득을 요청할 패스 ID - (클라이언트에서 채워줘야 할 값)
    public System.Byte rwdEndPT_N_;         // 획득할 일반 보상 포인트 목표 수치값  - (클라이언트에서 채워줘야 할 값)
    public System.Byte rwdEndPT_S_;         // 획득할 특별 보상 포인트 목표 수치값 - (클라이언트에서 채워줘야 할 값)
};
// 패스 보상 획득 요청 패킷 메시지
public class PktInfoRwdPassAck : PktMsgType
{
    public PktInfoPass pass_;               // 보상 획득으로 변경된 유저 패스 정보
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 유저 이벤트 변경 목록 패킷 메시지
    public static bool Read(Message _s, out PktInfoPassChangeList.Piece _v) {
        _v = new PktInfoPassChangeList.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoPassChangeList.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoPassChangeList _v) {
        _v = new PktInfoPassChangeList();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoPassChangeList _v) {
        WriteList(_s, _v.infos_);
    }
    // 패스 미션 보상 획득 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoRwdPassMission _v) {
        _v = new PktInfoRwdPassMission();
        return Read(_s, out _v.ids_);
    }
    public static void Write(Message _s, PktInfoRwdPassMission _v) {
        Write(_s, _v.ids_);
    }
    // 패스 보상 획득 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoRwdPassReq _v) {
        _v = new PktInfoRwdPassReq();
        if (false == _s.Read(out _v.passID_)) return false;
        if (false == _s.Read(out _v.rwdEndPT_N_)) return false;
        if (false == _s.Read(out _v.rwdEndPT_S_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoRwdPassReq _v) {
        _s.Write(_v.passID_);
        _s.Write(_v.rwdEndPT_N_);
        _s.Write(_v.rwdEndPT_S_);
    }
    // 패스 보상 획득 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoRwdPassAck _v) {
        _v = new PktInfoRwdPassAck();
        if (false == Read(_s, out _v.pass_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoRwdPassAck _v) {
        Write(_s, _v.pass_);
    }
}
///
/////////////////////////////////////////////////////////////////////////////