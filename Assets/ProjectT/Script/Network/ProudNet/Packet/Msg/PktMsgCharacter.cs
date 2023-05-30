using System.Collections.Generic;
using Nettention.Proud;


///////////////////////////////////////////////////////////////////////
///
// 캐릭터 패킷 메시지
public class PktInfoChar : PktMsgType
{
    public enum ENUM
    {
        MAX_RAID_HP = 10000,
    };
    public enum SkillSlot
    {
        _1 = 0,
        _2,
        _3,
        _4,

        _S_SKILL = _4,

        _MAX_,
    };
    public class WpnSlot : PktMsgType
    {
        public enum Enum
        {
            _1 = 0,
            _2,

            _MAX_,

            _MAIN_      = _1
        };
        public System.UInt64 wpnUID_;           // 장착 무기 UID
        public System.UInt32 wpnSkin_;          // 무기 스킨 테이블 ID
        public override bool Read(Message _s) {
            if (false == _s.Read(out wpnUID_)) return false;
            if (false == _s.Read(out wpnSkin_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(wpnUID_);
            _s.Write(wpnSkin_);
        }
    };
    public WpnSlot[] wpns_              = new WpnSlot[(int)WpnSlot.Enum._MAX_];  // 캐릭터가 장착 중인 무기들의 정보
    public System.UInt64 cuid_;                 // 캐릭터 UID
    public System.UInt32[] skillID_ = new System.UInt32[(int)SkillSlot._MAX_];  // 장착 스킬 ID
    public System.UInt32 tableID_;              // 테이블 ID
    public System.UInt32 costumeID_;            // 코스츔 ID
    public System.UInt32 passivePT_;            // 페시브 포인트
    public System.UInt32 skinStateFlag_;        // 적용된 스킨 상태 플레그
    public System.UInt32 exp_;                  // 경험치
    public System.UInt32 aniFlag_;              // 에니 플레그
    public System.UInt32 friExp_;               // 친밀도 경험치
    public System.UInt32 operationRoomTID_;     // 작전 회의 중 Facility TID
    public System.UInt16 raidHP_;               // 레이드 HP, 100퍼센트의 소수점 둘째자리까지값, 10000 => 100.00 
    public System.Byte friLv_;                  // 친밀도 레벨
    public System.Byte lv_;                     // 레벨
    public System.Byte grade_;                  // 등급
    public System.Byte costumeClr_;             // 코스츔 색
    public System.Byte selNum_;                 // 선택 번호
    public System.Byte preCnt_;                 // 선물 하기 횟수
    public System.Byte secretCnt_;              // 시크릿 퀘스트 참여 가능 횟수
    public System.Byte preferenceNum_;          // 친밀도 버프 순서
    public override bool Read(Message _s) {
        for (int loop = 0; loop < (int)WpnSlot.Enum._MAX_; ++loop)
        {
            if (false == PN_MarshalerEx.Read(_s, out wpns_[loop]))
                return false;
        }
        for (int loop = 0; loop < (int)eCOUNT.SKILLSLOT; ++loop)
        {
            if (false == _s.Read(out skillID_[loop]))
                return false;
        }
        if (false == _s.Read(out cuid_)) return false;
        if (false == _s.Read(out tableID_)) return false;
        if (false == _s.Read(out costumeID_)) return false;
        if (false == _s.Read(out passivePT_)) return false;
        if (false == _s.Read(out skinStateFlag_)) return false;
        if (false == _s.Read(out exp_)) return false;
        if (false == _s.Read(out aniFlag_)) return false;
        if (false == _s.Read(out friExp_)) return false;
        if (false == _s.Read(out operationRoomTID_)) return false;
        if (false == _s.Read(out raidHP_)) return false;
        if (false == _s.Read(out friLv_)) return false;
        if (false == _s.Read(out lv_)) return false;
        if (false == _s.Read(out grade_)) return false;
        if (false == _s.Read(out costumeClr_)) return false;
        if (false == _s.Read(out selNum_)) return false;
        if (false == _s.Read(out preCnt_)) return false;
        if (false == _s.Read(out secretCnt_)) return false;
        if (false == _s.Read(out preferenceNum_)) return false;
        return true;
    }
    public override void Write(Message _s) {
        for (int loop = 0; loop < (int)WpnSlot.Enum._MAX_; ++loop)
            PN_MarshalerEx.Write(_s, wpns_[loop]);
        for (int loop = 0; loop < (int)eCOUNT.SKILLSLOT; ++loop)
            _s.Write(skillID_[loop]);
        _s.Write(cuid_);
        _s.Write(tableID_);
        _s.Write(costumeID_);
        _s.Write(passivePT_);
        _s.Write(skinStateFlag_);
        _s.Write(exp_);
        _s.Write(aniFlag_);
        _s.Write(friExp_);
        _s.Write(operationRoomTID_);
        _s.Write(raidHP_);
        _s.Write(friLv_);
        _s.Write(lv_);
        _s.Write(grade_);
        _s.Write(costumeClr_);
        _s.Write(selNum_);
        _s.Write(preCnt_);
        _s.Write(secretCnt_);
        _s.Write(preferenceNum_);
    }
}
// 캐릭터 등급업 패킷 메시지
public class PktInfoCharGradeUp : PktMsgType
{
    public PktInfoConsumeItemAndGoods retItemGoods_;    // 재료 아이템 정보 및 결과 재화(각성 비용이 차감된 현재 유저의 재화와 추가된 각성 포인트)
    public System.UInt64 cuid_;             // 캐릭터 고유 ID
    public System.Byte resultGrade_;        // 적용 최종 등급업 수치
};
// 캐릭터 등급 레벨 설정 패킷 메시지
public class PktInfoCharGradeExpLv : PktMsgType
{
    public PktInfoCharGradeUp gradeUp_;     // 등급업 정보
    public PktInfoExpLv expLv_;             // 경험치 레벨
    public PktInfoUnexpectedPackage pkg_;   // 패키지 활성화 정보
};
// 캐릭터 스킨 및 컬러 정보 변경
public class PktInfoCharSkinColor : PktMsgType
{
    public System.UInt64 cuid_;             // 캐릭터 고유 ID - (클라이언트에서 채워줘야 할 값)
    public System.UInt32 skinStateFlag_;    // 적용된 스킨 상태 플레그 - (클라이언트에서 채워줘야 할 값)
    public System.Byte costumeClr_;         // 코스츔 색 - (클라이언트에서 채워줘야 할 값)
};

// 캐릭터에 메인 코스츔 적용 요청 패킷
public class PktInfoCharSetMainCostumeReq : PktMsgType
{
    public System.UInt32 costumeTID_;       // 코스츔 테이블 ID - (클라이언트에서 채워줘야 할 값)
    public PktInfoCharSkinColor skinColor_; // 스킨 컬러 정보 - (클라이언트에서는 cuid_ 값만 채워주면됨)
};


// 캐릭터에 무기 장착 패킷
public class PktInfoCharEquipWeapon : PktMsgType
{
    public PktInfoChar.WpnSlot[] wpns_ = new PktInfoChar.WpnSlot[(int)PktInfoChar.WpnSlot.Enum._MAX_];  // 무기 정보 목록 - (클라이언트에서 채워줘야 할 값)
    public PktInfoCharSkinColor skinColor_; // 스킨 컬러 정보 - (클라이언트에서는 cuid_ 값만 채워주면됨)

	public override bool Read(Message _s)
	{
		for (int loop = 0; loop < (int)PktInfoChar.WpnSlot.Enum._MAX_; ++loop)
		{
			if (false == PN_MarshalerEx.Read(_s, out wpns_[loop]))
				return false;
		}
		if (false == PN_MarshalerEx.Read(_s, out skinColor_)) return false;

		return true;
	}

	public override void Write(Message _s)
	{
		for (int loop = 0; loop < (int)PktInfoChar.WpnSlot.Enum._MAX_; ++loop)
			PN_MarshalerEx.Write(_s, wpns_[loop]);
		PN_MarshalerEx.Write(_s, skinColor_);
	}
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 캐릭터 무기 슬롯 패킷 메시지
    public static bool Read(Message _s, out PktInfoChar.WpnSlot _v) {
        _v = new PktInfoChar.WpnSlot();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoChar.WpnSlot _v) {
        _v.Write(_s);
    }
    // 캐릭터 패킷 메시지
    public static bool Read(Message _s, out PktInfoChar _v) {
        _v = new PktInfoChar();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoChar _v) {
        _v.Write(_s);
    }
    // 캐릭터 등급업 패킷 메시지
    public static bool Read(Message _s, out PktInfoCharGradeUp _v) {
        _v = new PktInfoCharGradeUp();
        if (false == Read(_s, out _v.retItemGoods_)) return false;
        if (false == _s.Read(out _v.cuid_)) return false;
        if (false == _s.Read(out _v.resultGrade_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCharGradeUp _v) {
        Write(_s, _v.retItemGoods_);
        _s.Write(_v.cuid_);
        _s.Write(_v.resultGrade_);
    }
    // 캐릭터 등급 레벨 설정 패킷 메시지
    public static bool Read(Message _s, out PktInfoCharGradeExpLv _v) {
        _v = new PktInfoCharGradeExpLv();
        if (false == Read(_s, out _v.gradeUp_)) return false;
        if (false == Read(_s, out _v.expLv_)) return false;
        if (false == Read(_s, out _v.pkg_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCharGradeExpLv _v) {
        Write(_s, _v.gradeUp_);
        Write(_s, _v.expLv_);
        Write(_s, _v.pkg_);
    }
    // 캐릭터 스킨 및 컬러 정보 변경
    public static bool Read(Message _s, out PktInfoCharSkinColor _v) {
        _v = new PktInfoCharSkinColor();
        if (false == _s.Read(out _v.cuid_)) return false;
        if (false == _s.Read(out _v.skinStateFlag_)) return false;
        if (false == _s.Read(out _v.costumeClr_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCharSkinColor _v) {
        _s.Write(_v.cuid_);
        _s.Write(_v.skinStateFlag_);
        _s.Write(_v.costumeClr_);
    }
    // 캐릭터에 메인 코스츔 적용 패킷
    public static bool Read(Message _s, out PktInfoCharSetMainCostumeReq _v) {
        _v = new PktInfoCharSetMainCostumeReq();
        if (false == _s.Read(out _v.costumeTID_)) return false;
        if (false == Read(_s, out _v.skinColor_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCharSetMainCostumeReq _v) {
        _s.Write(_v.costumeTID_);
        Write(_s, _v.skinColor_);
    }
    // 캐릭터에 무기 장착 패킷
    public static bool Read(Message _s, out PktInfoCharEquipWeapon _v) {
        _v = new PktInfoCharEquipWeapon();
        for (int loop = 0; loop < (int)PktInfoChar.WpnSlot.Enum._MAX_; ++loop)
        {
            if (false == PN_MarshalerEx.Read(_s, out _v.wpns_[loop]))
                return false;
        }
        if (false == Read(_s, out _v.skinColor_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCharEquipWeapon _v) {
        for (int loop = 0; loop < (int)PktInfoChar.WpnSlot.Enum._MAX_; ++loop)
            PN_MarshalerEx.Write(_s, _v.wpns_[loop]);
        Write(_s, _v.skinColor_);
    }
}
///
///////////////////////////////////////////////////////////////////////


///////////////////////////////////////////////////////////////////////
///
// 스킬 패킷 메시지
public class PktInfoCharSkill : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt32 tableID_;
        public System.Byte lv_;
        public override bool Read(Message _s) {
            if (false == _s.Read(out tableID_)) return false;
            return _s.Read(out lv_);
        }
        public override void Write(Message _s) {
            _s.Write(tableID_);
            _s.Write(lv_);
        }
    };
    public List<Piece> infos_;
    public override bool Read(Message _s) {
        return PN_MarshalerEx.ReadList(_s, out infos_);
    }
    public override void Write(Message _s) {
        PN_MarshalerEx.WriteList(_s, infos_);
    }
};
// 캐릭터 스킬 슬롯 적용 패킷 메시지
public class PktInfoCharSlotSkill : PktMsgType
{
    public System.UInt64 cuid_;         // 캐릭터 고유 ID - (클라이언트에서 채워줘야 할 값)
    public System.UInt32 tutoVal_;      // 튜토리얼 값 - (클라이언트에서 채워줘야 할 값)
    public System.UInt32[] skilTIDs_ = new System.UInt32[(int)PktInfoChar.SkillSlot._MAX_]; // 슬롯에 위치할 스킬 테이블 ID들 - (클라이언트에서 채워줘야 할 값)
};
// 스킬 레벨업 요청 패킷 메시지
public class PktInfoSkillLvUpReq : PktMsgType
{
    public System.UInt64 cuid_;         // 캐릭터 고유 ID
    public System.UInt32 tutoVal_;      // 튜토리얼 값
    public System.UInt32 skillTID_;     // 스킬 테이블 ID
};
// 스킬 레벨업 패킷 메시지
public class PktInfoSkillLvUp : PktMsgType
{
    public PktInfoConsumeItem maters_;  // 재료 아이템 정보
    public System.UInt64 cuid_;         // 캐릭터 고유 ID
    public System.UInt64 userGold_;     // 레벨업 비용이 차감된 현재 유저의 금화
    public System.UInt32 tutoVal_;      // 튜토리얼 값
    public System.UInt32 skillPT_;      // 레벨업 비용이 차감된 현재 캐릭터의 스킬 포인트
    public System.UInt32 skillTID_;     // 스킬 테이블 ID
    public System.Byte resultLv_;       // 레벨업 적용 수치
};
// 캐릭터 선물 주기 요청 패킷 메시지
public class PktInfoGivePresentCharReq : PktMsgType
{
    public PktInfoItemCnt maters_;      // 소모 아이템 정보
    public System.UInt64 cuid_;         // 캐릭터 고유 ID
};
// 원하는 캐릭터 시크릿 퀘스트 횟수 초기화 응답 패킷 메시지
public class PktInfoCharSecretCntRst : PktMsgType
{
    public PktInfoConsumeItemAndGoods consume_; // 소모 아이템 및 재화 정보
    public PktInfoUIDList chars_;       // 소모 아이템 정보
};
// 캐릭터 레이드 hp 회복 아이템 사용 요청
public class PktInfoRaidRestoreHPReq : PktMsgType
{
    public PktInfoItemCnt items_;       // 아이템 정보
    public System.UInt64 cuid_;         // 캐릭터 고유ID
};
// 캐릭터 레이드 hp 회복 아이템 사용 응답
public class PktInfoRaidRestoreHPAck : PktMsgType
{
    public PktInfoConsumeItem consume_; // 소모 아이템 및 재화
    public System.UInt64 cuid_;         // 캐릭터 고유ID
    public System.UInt16 raidHP_;       // 레이드 HP
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 스킬 패킷 메시지
    public static bool Read(Message _s, out PktInfoCharSkill.Piece _v) {
        _v = new PktInfoCharSkill.Piece();
        if (false == _v.Read(_s)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCharSkill.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoCharSkill _v) {
        _v = new PktInfoCharSkill();
        if (false == _v.Read(_s)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCharSkill _v) {
        _v.Write(_s);
    }
    // 캐릭터 스킬 슬롯 적용 패킷 메시지
    public static bool Read(Message _s, out PktInfoCharSlotSkill _v) {
        _v = new PktInfoCharSlotSkill();
        if (false == _s.Read(out _v.cuid_)) return false;
        if (false == _s.Read(out _v.tutoVal_)) return false;
        for (int loop = 0; loop < (int)PktInfoChar.SkillSlot._MAX_; ++loop)
        {
            if (false == _s.Read(out _v.skilTIDs_[loop]))
                return false;
        }
        return true;
    }
    public static void Write(Message _s, PktInfoCharSlotSkill _v) {
        _s.Write(_v.cuid_);
        _s.Write(_v.tutoVal_);
        for (int loop = 0; loop < (int)PktInfoChar.SkillSlot._MAX_; ++loop)
            _s.Write(_v.skilTIDs_[loop]);
    }
    public static bool Read(Message _s, out PktInfoSkillLvUpReq _v) {
        _v = new PktInfoSkillLvUpReq();
        if (false == _s.Read(out _v.cuid_)) return false;
        if (false == _s.Read(out _v.tutoVal_)) return false;
        if (false == _s.Read(out _v.skillTID_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoSkillLvUpReq _v) {
        _s.Write(_v.cuid_);
        _s.Write(_v.tutoVal_);
        _s.Write(_v.skillTID_);
    }
    public static bool Read(Message _s, out PktInfoSkillLvUp _v) {
        _v = new PktInfoSkillLvUp();
        if (false == Read(_s, out _v.maters_)) return false;
        if (false == _s.Read(out _v.cuid_)) return false;
        if (false == _s.Read(out _v.userGold_)) return false;
        if (false == _s.Read(out _v.tutoVal_)) return false;
        if (false == _s.Read(out _v.skillPT_)) return false;
        if (false == _s.Read(out _v.skillTID_)) return false;
        if (false == _s.Read(out _v.resultLv_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoSkillLvUp _v) {
        Write(_s, _v.maters_);
        _s.Write(_v.cuid_);
        _s.Write(_v.userGold_);
        _s.Write(_v.tutoVal_);
        _s.Write(_v.skillPT_);
        _s.Write(_v.skillTID_);
        _s.Write(_v.resultLv_);
    }
    public static bool Read(Message _s, out PktInfoGivePresentCharReq _v) {
        _v = new PktInfoGivePresentCharReq();
        if (false == Read(_s, out _v.maters_)) return false;
        if (false == _s.Read(out _v.cuid_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoGivePresentCharReq _v) {
        Write(_s, _v.maters_);
        _s.Write(_v.cuid_);
    }
    public static bool Read(Message _s, out PktInfoCharSecretCntRst _v) {
        _v = new PktInfoCharSecretCntRst();
        if (false == Read(_s, out _v.consume_)) return false;
        if (false == Read(_s, out _v.chars_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCharSecretCntRst _v) {
        Write(_s, _v.consume_);
        Write(_s, _v.chars_);
    }
    // 캐릭터 레이드 hp 회복 아이템 사용 요청
    public static bool Read(Message _s, out PktInfoRaidRestoreHPReq _v) {
        _v = new PktInfoRaidRestoreHPReq();
        if (false == PN_MarshalerEx.Read(_s, out _v.items_)) return false;
        if (false == _s.Read(out _v.cuid_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoRaidRestoreHPReq _v) {
        PN_MarshalerEx.Write(_s, _v.items_);
        _s.Write(_v.cuid_);
    }
    // 캐릭터 레이드 hp 회복 아이템 사용 응답
    public static bool Read(Message _s, out PktInfoRaidRestoreHPAck _v) {
        _v = new PktInfoRaidRestoreHPAck();
        if (false == PN_MarshalerEx.Read(_s, out _v.consume_)) return false;
        if (false == _s.Read(out _v.cuid_)) return false;
        if (false == _s.Read(out _v.raidHP_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoRaidRestoreHPAck _v) {
        PN_MarshalerEx.Write(_s, _v.consume_);
        _s.Write(_v.cuid_);
        _s.Write(_v.raidHP_);
    }
}
///
///////////////////////////////////////////////////////////////////////


///////////////////////////////////////////////////////////////////////
///
// 유저 스킬 패킷 메시지
public class PktInfoUserSkill : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt32 tableID_;
        public System.Byte lv_;
        public override bool Read(Message _s) {
            if (false == _s.Read(out tableID_)) return false;
            return _s.Read(out lv_);
        }
        public override void Write(Message _s) {
            _s.Write(tableID_);
            _s.Write(lv_);
        }
    };
    public List<Piece> infos_;
    public override bool Read(Message _s) {
        return PN_MarshalerEx.ReadList(_s, out infos_);
    }
    public override void Write(Message _s) {
        PN_MarshalerEx.WriteList(_s, infos_);
    }
};
// 유저 스킬 레벨업 요청 패킷 메시지
public class PktInfoUserSklLvUpReq : PktMsgType
{
    public System.UInt32 tid_;          // 스킬 테이블 ID
};
// 유저 스킬 레벨업 응답 패킷 메시지
public class PktInfoUserSklLvUpAck : PktMsgType
{
    public PktInfoConsumeItem maters_;  // 재료 아이템 정보
    public PktInfoGoods goods_;         // 소모된 재화가 적용된 현재 재화
    public System.UInt32 tid_;          // 유저 각성 스킬 테이블 ID
    public System.Byte resultLv_;       // 레벨업 적용 수치
};
// 유저 스킬 초기화 패킷 메시지
public class PktInfoUserSklReset : PktMsgType
{
    public PktInfoConsumeItemAndGoods consume_; // 소모된 재화가 적용된 아이템 및 재화
    public PktInfoTIDList tids_;        // 초기화가 진행된 유저 스킬 테이블 ID
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 유저 스킬 패킷 메시지
    public static bool Read(Message _s, out PktInfoUserSkill.Piece _v) {
        _v = new PktInfoUserSkill.Piece();
        if (false == _v.Read(_s)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoUserSkill.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoUserSkill _v) {
        _v = new PktInfoUserSkill();
        if (false == _v.Read(_s)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoUserSkill _v) {
        _v.Write(_s);
    }
    // 유저 스킬 레벨업 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoUserSklLvUpReq _v) {
        _v = new PktInfoUserSklLvUpReq();
        if (false == _s.Read(out _v.tid_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoUserSklLvUpReq _v) {
        _s.Write(_v.tid_);
    }
    // 유저 스킬 레벨업 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoUserSklLvUpAck _v) {
        _v = new PktInfoUserSklLvUpAck();
        if (false == Read(_s, out _v.maters_)) return false;
        if (false == Read(_s, out _v.goods_)) return false;
        if (false == _s.Read(out _v.tid_)) return false;
        if (false == _s.Read(out _v.resultLv_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoUserSklLvUpAck _v) {
        Write(_s, _v.maters_);
        Write(_s, _v.goods_);
        _s.Write(_v.tid_);
        _s.Write(_v.resultLv_);
    }
    // 유저 스킬 레벨업 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoUserSklReset _v) {
        _v = new PktInfoUserSklReset();
        if (false == Read(_s, out _v.consume_)) return false;
        if (false == Read(_s, out _v.tids_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoUserSklReset _v) {
        Write(_s, _v.consume_);
        Write(_s, _v.tids_);
    }
}
///
///////////////////////////////////////////////////////////////////////