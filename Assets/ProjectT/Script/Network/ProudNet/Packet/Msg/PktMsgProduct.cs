using System.Collections.Generic;
using Nettention.Proud;


///////////////////////////////////////////////////////////////////////
///
// 카드(서포터) 패킷 메시지
public class PktInfoCard : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt64 cardUID_;
        public System.UInt64 posValue_;         // 카드(서포터) 사용 값
        public System.UInt32 tableID_;
        public System.UInt32 exp_;
        public System.Byte lv_;                 // 레벨
        public eContentsPosKind posKind_;       // 카드(서포터) 사용 위치 타입
        public System.Byte posSlotNum_;         // 카드(서포터) 사용 위치의 술롯 번호
        public System.Byte skillLv_;            // 스킬 레벨
        public System.Byte wake_;               // 각성 수치
        public System.Byte encLv_;              // 인챈 수치
        public System.Byte type_;               // 속성(설정되면 eSTAGE_CONDI 값을 사용하고 0 일 경우에는 기본 값을 사용합니다.)
        public bool lock_;

        public override bool Read(Message _s) {
            if (false == _s.Read(out cardUID_)) return false;
            if (false == _s.Read(out posValue_)) return false;
            if (false == _s.Read(out tableID_)) return false;
            if (false == _s.Read(out exp_)) return false;
            if (false == _s.Read(out lv_)) return false;
            if (false == PN_MarshalerEx.Read(_s, out posKind_)) return false;
            if (false == _s.Read(out posSlotNum_)) return false;
            if (false == _s.Read(out skillLv_)) return false;
            if (false == _s.Read(out wake_)) return false;
            if (false == _s.Read(out encLv_)) return false;
            if (false == _s.Read(out type_)) return false;
            if (false == _s.Read(out lock_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(cardUID_);
            _s.Write(posValue_);
            _s.Write(tableID_);
            _s.Write(exp_);
            _s.Write(lv_);
            PN_MarshalerEx.Write(_s, posKind_);
            _s.Write(posSlotNum_);
            _s.Write(skillLv_);
            _s.Write(wake_);
            _s.Write(encLv_);
            _s.Write(type_);
            _s.Write(lock_);
        }
    };
    public List<Piece> infos_;
};
// 카드(서포터) 위치 적용 패킷 메시지
public class PktInfoCardApplyPos : PktMsgType
{
    public System.UInt64 oldCardUID_;   // 기존에 적용중이였던 카드(서포터) 고유 ID
    public System.UInt64 cardUID_;      // 적용하려는 카드(서포터) 고유 ID - (클라이언트에서 채워줘야 할 값)
    public System.UInt64 posValue_;     // 위치 값 - (클라이언트에서 채워줘야 할 값)
    public System.UInt32 tutoVal_;      // 튜토리얼 값 - (클라이언트에서 채워줘야 할 값)
    public eContentsPosKind posKind_;   // 위치 분류 - (클라이언트에서 채워줘야 할 값)
    public System.Byte slotNum_;        // 적용 슬롯 번호 - (클라이언트에서 채워줘야 할 값)
    public System.Byte oldCardChangeSlotNum_;    // 기존에 적용중이였던 카드(서포터)가 교체 됐을 경우 슬롯 번호(교체가 아니면 eCardSlotPosMax::_SLOT_MAX_값입니다.)
};
// 카드(서포터) 위치 적용 해제 패킷 메시지
public class PktInfoCardApplyOutPos : PktMsgType
{
    public System.UInt64 cardUID_;      // 해제 하려는 카드(서포터) 고유 ID
};
// 카드(서포터) 판매 패킷 메시지
public class PktInfoCardSell : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt64 cardUID_;  // 카드(서포터) 고유 ID - (클라이언트에서 채워줘야 할 값)
        public System.UInt32 addGold_;  // 판매해서 추가된 금액
        public System.UInt32 addSP_;    // 판매해서 추가된 SP
        public override bool Read(Message _s) {
            if (false == _s.Read(out cardUID_)) return false;
            _s.Read(out addGold_);
            return _s.Read(out addSP_);
        }
        public override void Write(Message _s) {
            _s.Write(cardUID_);
            _s.Write(addGold_);
            _s.Write(addSP_);
        }
    };
    public List<Piece> infos_;
    public System.UInt64 userGold_;     // 판매 금액이 적용된 현재 유저의 금화
    public System.UInt64 userSP_;       // 판매 SP가 적용된 현재 유저의 SP
};
// 카드(서포터) 락 패킷 메시지
public class PktInfoCardLock : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt64 cardUID_;  // 카드(서포터) 고유 ID - (클라이언트에서 채워줘야 할 값)
        public System.Boolean lock_;    // 락 여부 - (클라이언트에서 채워줘야 할 값)
        public override bool Read(Message _s) {
            if (false == _s.Read(out cardUID_)) return false;
            return _s.Read(out lock_);
        }
        public override void Write(Message _s) {
            _s.Write(cardUID_);
            _s.Write(lock_);
        }
    };
    public List<Piece> infos_;
};
// 카드(서포터) 속성 변경 요청 패킷 메시지
public class PktInfoCardTypeChangeReq : PktMsgType
{
    public System.UInt64 cardUID_;      // 카드(서포터) 고유 ID - (클라이언트에서 채워줘야 할 값)
    public System.Byte type_;           // 변경하고 싶은 타입 값 - (클라이언트에서 채워줘야 할 값)
    public eGOODSTYPE goodsTP_;         // 소모할 비용 타입 - (클라이언트에서 채워줘야 할 값)
};
// 카드(서포터) 속성 변경 응답 패킷 메시지
public class PktInfoCardTypeChangeAck : PktMsgType
{
    public PktInfoGoods goods_;         // 소모된 비용이 적용된 현재 재화
    public System.UInt64 cardUID_;      // 카드(서포터) 고유 ID
    public System.Byte type_;           // 변경된 타입 값
};
// 카드(서포터) 성장 패킷 메시지
public class PktInfoCardGrow : PktMsgType
{
    public PktInfoProductComGrowAck comGrow_; // 공용 제품 레벨업 응답 정보
    public PktInfoCardBook bookStates_; // 도감의 변경된 상태 알려주는 정보
    public System.UInt32 tutoVal_;      // 튜토리얼 값
    public System.Byte retSkillLv_;     // 결과 스킬 레벨
    public System.Byte retWake_;        // 결과 각성 단계
    public System.Byte retEnc_;         // 결과 인챈 수치
    public eGrowState growState_;       // 성장 시도 결과
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 카드(서포터) 패킷 메시지
    public static bool Read(Message _s, out PktInfoCard.Piece _v) {
        _v = new PktInfoCard.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoCard.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoCard _v) {
        _v = new PktInfoCard();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoCard _v) {
        WriteList(_s, _v.infos_);
    }
    // 카드(서포터) 사용 패킷 메시지
    public static bool Read(Message _s, out PktInfoCardApplyPos _v) {
        _v = new PktInfoCardApplyPos();
        if (false == _s.Read(out _v.oldCardUID_)) return false;
        if (false == _s.Read(out _v.cardUID_)) return false;
        if (false == _s.Read(out _v.posValue_)) return false;
        if (false == _s.Read(out _v.tutoVal_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.posKind_)) return false;
        if (false == _s.Read(out _v.slotNum_)) return false;
        if (false == _s.Read(out _v.oldCardChangeSlotNum_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCardApplyPos _v) {
        _s.Write(_v.oldCardUID_);
        _s.Write(_v.cardUID_);
        _s.Write(_v.posValue_);
        _s.Write(_v.tutoVal_);
        PN_MarshalerEx.Write(_s, _v.posKind_);
        _s.Write(_v.slotNum_);
        _s.Write(_v.oldCardChangeSlotNum_);
    }
    // 카드(서포터) 위치 적용 해제 패킷 메시지
    public static bool Read(Message _s, out PktInfoCardApplyOutPos _v) {
        _v = new PktInfoCardApplyOutPos();
        return _s.Read(out _v.cardUID_);
    }
    public static void Write(Message _s, PktInfoCardApplyOutPos _v) {
        _s.Write(_v.cardUID_);
    }
    // 카드(서포터) 판매 패킷 메시지
    public static bool Read(Message _s, out PktInfoCardSell.Piece _v) {
        _v = new PktInfoCardSell.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoCardSell.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoCardSell _v) {
        _v = new PktInfoCardSell();
        ReadList(_s, out _v.infos_);
        _s.Read(out _v.userGold_);
        return _s.Read(out _v.userSP_);
    }
    public static void Write(Message _s, PktInfoCardSell _v) {
        WriteList(_s, _v.infos_);
        _s.Write(_v.userGold_);
        _s.Write(_v.userSP_);
    }
    // 카드(서포터) 락 패킷 메시지
    public static bool Read(Message _s, out PktInfoCardLock.Piece _v) {
        _v = new PktInfoCardLock.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoCardLock.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoCardLock _v) {
        _v = new PktInfoCardLock();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoCardLock _v) {
        WriteList(_s, _v.infos_);
    }
    // 카드(서포터) 속성 변경 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoCardTypeChangeReq _v) {
        _v = new PktInfoCardTypeChangeReq();
        if (false == _s.Read(out _v.cardUID_)) return false;
        if (false == _s.Read(out _v.type_)) return false;
        if (false == Read(_s, out _v.goodsTP_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCardTypeChangeReq _v) {
        _s.Write(_v.cardUID_);
        _s.Write(_v.type_);
        Write(_s, _v.goodsTP_);
    }
    // 카드(서포터) 속성 변경 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoCardTypeChangeAck _v) {
        _v = new PktInfoCardTypeChangeAck();
        if (false == Read(_s, out _v.goods_)) return false;
        if (false == _s.Read(out _v.cardUID_)) return false;
        if (false == _s.Read(out _v.type_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCardTypeChangeAck _v) {
        Write(_s, _v.goods_);
        _s.Write(_v.cardUID_);
        _s.Write(_v.type_);
    }
    // 카드(서포터) 성장 패킷 메시지
    public static bool Read(Message _s, out PktInfoCardGrow _v) {
        _v = new PktInfoCardGrow();
        if (false == Read(_s, out _v.comGrow_)) return false;
        if (false == Read(_s, out _v.bookStates_)) return false;
        if (false == _s.Read(out _v.tutoVal_)) return false;
        if (false == _s.Read(out _v.retSkillLv_)) return false;
        if (false == _s.Read(out _v.retWake_)) return false;
        if (false == _s.Read(out _v.retEnc_)) return false;
        if (false == Read(_s, out _v.growState_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoCardGrow _v) {
        Write(_s, _v.comGrow_);
        Write(_s, _v.bookStates_);
        _s.Write(_v.tutoVal_);
        _s.Write(_v.retSkillLv_);
        _s.Write(_v.retWake_);
        _s.Write(_v.retEnc_);
        Write(_s, _v.growState_);
    }
}
///
///////////////////////////////////////////////////////////////////////


///////////////////////////////////////////////////////////////////////
///
// 아이템 판매 패킷 메시지
public class PktInfoItemSell : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt64 itemUID_;  // 아이템 고유 ID - (클라이언트에서 채워줘야 할 값)
        public System.UInt32 addGold_;  // 판매해서 추가된 금액
        public System.UInt32 cnt_;      // 현재 남은 아이탬의 총 수량
        public System.UInt32 sellCnt_;  // 판매한 아이템의 수량 - (클라이언트에서 채워줘야 할 값)
        public System.Byte delFlag_;    // 아이템을 제거할지 여부
        public override bool Read(Message _s) {
            if (false == _s.Read(out itemUID_)) return false;
            if (false == _s.Read(out addGold_)) return false;
            if (false == _s.Read(out cnt_)) return false;
            if (false == _s.Read(out sellCnt_)) return false;
            return _s.Read(out delFlag_);
        }
        public override void Write(Message _s) {
            _s.Write(itemUID_);
            _s.Write(addGold_);
            _s.Write(cnt_);
            _s.Write(sellCnt_);
            _s.Write(delFlag_);
        }
    };
    public List<Piece> infos_;
    public System.UInt64 userGold_;         // 판매 금액이 적용된 현재 유저의 금화
};
// 아이템 사용 요청 패킷 메시지
public class PktInfoUseItemReq : PktMsgType
{
    public PktInfoItemCnt.Piece item_;      // 사용한 아이템
    public System.UInt64 value1_;           // 아이템 사용시 타입에 따라서 사용되는 값 eITEMSUBTYPE
};
// 아이템 사용(재화 획득) 응답 패킷 메시지
public class PktInfoUseItemGoodsAck : PktMsgType
{
    public PktInfoConsumeItem useItem_;      // 사용한 아이템
    public System.UInt64 userVal_;           // 아이템 사용한 결과 값(변경된 현재 재화 값)
    public eGOODSTYPE type_;                 // 사용한 아이템 결과로 얻은 재화 타입
};
// 아이템 사용(코드 획득) 응답 패킷 메시지
public class PktInfoUseItemCodeAck : PktMsgType
{
    public PktInfoConsumeItem useItem_;     // 사용한 아이템
    public System.String code_;				// 아이템 사용한 결과 값(코드)
};
// 아이템 사용(스테이지 특별 모드) 응답 패킷 메시지
public class PktInfoUseItemStageSpecialAck : PktMsgType
{
    public PktInfoConsumeItem useItem_;     // 사용한 아이템
    public PktInfoStage.Special special_;   // 아이템 사용한 결과 값(변경된 특별 모드 정보)
    public System.Byte subItemType_;		// 사용한 아이템 서브 타입(eITEMSUBTYPE)
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 아이템 판매 패킷 메시지
    public static bool Read(Message _s, out PktInfoItemSell.Piece _v) {
        _v = new PktInfoItemSell.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoItemSell.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoItemSell _v) {
        _v = new PktInfoItemSell();
        ReadList(_s, out _v.infos_);
        return _s.Read(out _v.userGold_);
    }
    public static void Write(Message _s, PktInfoItemSell _v) {
        WriteList(_s, _v.infos_);
        _s.Write(_v.userGold_);
    }
    // 아이템 사용 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoUseItemReq _v) {
        _v = new PktInfoUseItemReq();
        if (false == Read(_s, out _v.item_)) return false;
        return _s.Read(out _v.value1_);
    }
    public static void Write(Message _s, PktInfoUseItemReq _v) {
        Write(_s, _v.item_);
        _s.Write(_v.value1_);
    }
    // 아이템 사용(재화 획득) 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoUseItemGoodsAck _v) {
        _v = new PktInfoUseItemGoodsAck();
        if (false == Read(_s, out _v.useItem_)) return false;
        if (false == _s.Read(out _v.userVal_)) return false;
        return Read(_s, out _v.type_); 
    }
    public static void Write(Message _s, PktInfoUseItemGoodsAck _v) {
        Write(_s, _v.useItem_);
        _s.Write(_v.userVal_);
        Write(_s, _v.type_);
    }
    // 아이템 사용(코드 획득) 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoUseItemCodeAck _v) {
        _v = new PktInfoUseItemCodeAck();
        if (false == Read(_s, out _v.useItem_)) return false;
        if (false == _s.Read(out _v.code_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoUseItemCodeAck _v) {
        Write(_s, _v.useItem_);
        _s.Write(_v.code_);
    }
    // 아이템 사용(스테이지 특별 모드) 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoUseItemStageSpecialAck _v) {
        _v = new PktInfoUseItemStageSpecialAck();
        if (false == Read(_s, out _v.useItem_)) return false;
        if (false == Read(_s, out _v.special_)) return false;
        if (false == _s.Read(out _v.subItemType_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoUseItemStageSpecialAck _v) {
        Write(_s, _v.useItem_);
        Write(_s, _v.special_);
        _s.Write(_v.subItemType_);
    }
}
///
///////////////////////////////////////////////////////////////////////


///////////////////////////////////////////////////////////////////////
///
// 곡옥 패킷 메시지
public class PktInfoGem : PktMsgType
{
    public class Option : PktMsgType
    {
        public enum ENUM
        {
            _1 = 0,
            _2,
            _3,
            _4,
            _5,

            _MAX_
        };
        public System.UInt16	optID_;
        public System.Byte		value_;
        public override bool Read(Message _s) {
            if (false == _s.Read(out optID_)) return false;
            return _s.Read(out value_);
        }
        public override void Write(Message _s) {
            _s.Write(optID_);
            _s.Write(value_);
        }
    };
    public class Piece : PktMsgType
    {
        public Option[] opt_ = new Option[(int)Option.ENUM._MAX_];
        public Option rstOpt_;
        public System.UInt64 gemUID_;
        public System.UInt32 tableID_;
        public System.UInt32 exp_;
        public System.Byte lv_;
        public System.Byte wake_;
        public System.Byte rstIdx_;         // 리셋된 옵션 정보 슬롯 인덱스
		public byte setOptID_;				// 세트 속성 테이블 아이디
		public bool lock_;
        public override bool Read(Message _s) {
            for (int loop = 0; loop < (int)Option.ENUM._MAX_; ++loop)
            {
                if (false == PN_MarshalerEx.Read(_s, out opt_[loop]))
                    return false;
            }
            if (false == PN_MarshalerEx.Read(_s, out rstOpt_)) return false;
            if (false == _s.Read(out gemUID_)) return false;
            if (false == _s.Read(out tableID_)) return false;
            if (false == _s.Read(out exp_)) return false;
            if (false == _s.Read(out lv_)) return false;
            if (false == _s.Read(out wake_)) return false;
            if (false == _s.Read(out rstIdx_)) return false;
			if (false == _s.Read(out setOptID_)) return false;
			return _s.Read(out lock_);
        }
        public override void Write(Message _s) {
            for (int loop = 0; loop < (int)Option.ENUM._MAX_; ++loop)
                PN_MarshalerEx.Write(_s, opt_[loop]);
            PN_MarshalerEx.Write(_s, rstOpt_);
            _s.Write(gemUID_);
            _s.Write(tableID_);
            _s.Write(exp_);
            _s.Write(lv_);
            _s.Write(wake_);
            _s.Write(rstIdx_);
			_s.Write(setOptID_);
			_s.Write(lock_);
        }
    };
    public List<Piece> infos_;
};
// 곡옥 판매 패킷 메시지
public class PktInfoGemSell : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt64 gemUID_;   // 곡옥 고유 ID - (클라이언트에서 채워줘야 할 값)
        public System.UInt32 addGold_;  // 판매해서 추가된 금액
        public override bool Read(Message _s) {
            if (false == _s.Read(out gemUID_)) return false;
            return _s.Read(out addGold_);
        }
        public override void Write(Message _s) {
            _s.Write(gemUID_);
            _s.Write(addGold_);
        }
    };
    public List<Piece> infos_;
    public System.UInt64 userGold_;     // 판매 금액이 적용된 현재 유저의 금화
};
// 곡옥 락 패킷 메시지
public class PktInfoGemLock : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt64 gemUID_;   // 곡옥 고유 ID - (클라이언트에서 채워줘야 할 값)
        public System.Boolean lock_;    // 락 여부 - (클라이언트에서 채워줘야 할 값)
        public override bool Read(Message _s) {
            if (false == _s.Read(out gemUID_)) return false;
            return _s.Read(out lock_);
        }
        public override void Write(Message _s) {
            _s.Write(gemUID_);
            _s.Write(lock_);
        }
    };
    public List<Piece> infos_;
};
// 곡옥 성장 패킷 메시지
public class PktInfoGemGrow : PktMsgType
{
    public PktInfoProductComGrowAck comGrow_;   // 공용 제품 레벨업 응답 정보
    public PktInfoGem.Piece gem_;               // 성장을 통해 변경된 곡옥 정보
};
// 곡옥 옵션 변경 요청 패킷 메시지
public class PktInfoGemResetOptReq : PktMsgType
{
    public System.UInt64 gemUID_;           // 재설정 할 곡옥 UID - (클라이언트에서 채워줘야 할 값)
    public System.Byte slotIdx_;            // 재설정 할 옵션 인덱스 - (클라이언트에서 채워줘야 할 값)
};
// 곡옥 옵션 변경 응답 패킷 메시지
public class PktInfoGemResetOptAck : PktMsgType
{
    public PktInfoGem.Option opt_;					// 곡옥 옵션 정보
    public PktInfoGemResetOptReq req_;				// 재설정 요청 정보
    public PktInfoConsumeItemAndGoods consume_;		// 소모된 아이템, 재화정보
};
// 곡옥 옵션 변경 선택 패킷 메시지
public class PktInfoGemResetOptSelect : PktMsgType
{
    public System.UInt64 gemUID_;           // 재설정할 곡옥 UID - (클라이언트에서 채워줘야 할 값)
    public System.Boolean newFlag_;         // 새로운 값으로 선택할지 여부(true:신규, false:기존) - (클라이언트에서 채워줘야 할 값)
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 곡옥 패킷 메시지
    public static bool Read(Message _s, out PktInfoGem.Option _v) {
        _v = new PktInfoGem.Option();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoGem.Option _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoGem.Piece _v) {
        _v = new PktInfoGem.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoGem.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoGem _v) {
        _v = new PktInfoGem();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoGem _v) {
        WriteList(_s, _v.infos_);
    }
    // 곡옥 판매 패킷 메시지
    public static bool Read(Message _s, out PktInfoGemSell.Piece _v) {
        _v = new PktInfoGemSell.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoGemSell.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoGemSell _v) {
        _v = new PktInfoGemSell();
        ReadList(_s, out _v.infos_);
        return _s.Read(out _v.userGold_);
    }
    public static void Write(Message _s, PktInfoGemSell _v) {
        WriteList(_s, _v.infos_);
        _s.Write(_v.userGold_);
    }
    // 곡옥 락 패킷 메시지
    public static bool Read(Message _s, out PktInfoGemLock.Piece _v) {
        _v = new PktInfoGemLock.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoGemLock.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoGemLock _v) {
        _v = new PktInfoGemLock();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoGemLock _v) {
        WriteList(_s, _v.infos_);
    }
    // 곡옥 성장 패킷 메시지
    public static bool Read(Message _s, out PktInfoGemGrow _v) {
        _v = new PktInfoGemGrow();
        if (false == Read(_s, out _v.comGrow_)) return false;
        if (false == Read(_s, out _v.gem_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoGemGrow _v) {
        Write(_s, _v.comGrow_);
        Write(_s, _v.gem_);
    }
    // 곡옥 옵션 변경 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoGemResetOptReq _v) {
        _v = new PktInfoGemResetOptReq();
        if (false == _s.Read(out _v.gemUID_)) return false;
        if (false == _s.Read(out _v.slotIdx_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoGemResetOptReq _v) {
        _s.Write(_v.gemUID_);
        _s.Write(_v.slotIdx_);
    }
    // 곡옥 옵션 변경 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoGemResetOptAck _v) {
        _v = new PktInfoGemResetOptAck();
        if (false == Read(_s, out _v.opt_)) return false;
        if (false == Read(_s, out _v.req_)) return false;
        if (false == Read(_s, out _v.consume_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoGemResetOptAck _v) {
        Write(_s, _v.opt_);
        Write(_s, _v.req_);
        Write(_s, _v.consume_);
    }
    // 곡옥 옵션 변경 선택 패킷 메시지
    public static bool Read(Message _s, out PktInfoGemResetOptSelect _v) {
        _v = new PktInfoGemResetOptSelect();
        if (false == _s.Read(out _v.gemUID_)) return false;
        if (false == _s.Read(out _v.newFlag_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoGemResetOptSelect _v) {
        _s.Write(_v.gemUID_);
        _s.Write(_v.newFlag_);
    }
}
///
///////////////////////////////////////////////////////////////////////


///////////////////////////////////////////////////////////////////////
///
// 무기 패킷 메시지
public class PktInfoWeapon : PktMsgType
{
    public class Slot : PktMsgType
    {
        public enum ENUM
        {
            _1 = 0,
            _2,
            _3,
            _4,

            _MAX_
        };
        public System.UInt64 gemUID_;
        public override bool Read(Message _s) {
            if (false == _s.Read(out gemUID_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(gemUID_);
        }
    };
    public class Piece : PktMsgType
    {
        public Slot[] slot_ = new Slot[(int)Slot.ENUM._MAX_];
        public System.UInt64 weaponUID_;
        public System.UInt32 tableID_;
        public System.UInt32 exp_;
        public System.Byte lv_;
        public System.Byte skillLv_;
        public System.Byte wake_;
        public System.Byte encLv_;
        public bool lock_;
        public override bool Read(Message _s)
        {
            for (int loop = 0; loop < (int)Slot.ENUM._MAX_; ++loop)
            {
                if (false == PN_MarshalerEx.Read(_s, out slot_[loop]))
                    return false;
            }
            if (false == _s.Read(out weaponUID_)) return false;
            if (false == _s.Read(out tableID_)) return false;
            if (false == _s.Read(out exp_)) return false;
            if (false == _s.Read(out lv_)) return false;
            if (false == _s.Read(out skillLv_)) return false;
            if (false == _s.Read(out wake_)) return false;
            if (false == _s.Read(out encLv_)) return false;
            return _s.Read(out lock_);
        }
        public override void Write(Message _s)
        {
            for (int loop = 0; loop < (int)PktInfoWeapon.Slot.ENUM._MAX_; ++loop)
                PN_MarshalerEx.Write(_s, slot_[loop]);
            _s.Write(weaponUID_);
            _s.Write(tableID_);
            _s.Write(exp_);
            _s.Write(lv_);
            _s.Write(skillLv_);
            _s.Write(wake_);
            _s.Write(encLv_);
            _s.Write(lock_);
        }
    };
    public List<Piece> infos_;
};
// 무기 판매 패킷 메시지
public class PktInfoWeaponSell : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt64 weaponUID_;    // 무기 고유 ID - (클라이언트에서 채워줘야 할 값)
        public System.UInt32 addGold_;      // 판매해서 추가된 금액
        public System.UInt32 addSP_;        // 판매해서 추가된 카드(서포터) 포인트
        public override bool Read(Message _s)
        {
            if (false == _s.Read(out weaponUID_)) return false;
            if (false == _s.Read(out addGold_)) return false;
            if (false == _s.Read(out addSP_)) return false;
            return true;
        }
        public override void Write(Message _s)
        {
            _s.Write(weaponUID_);
            _s.Write(addGold_);
            _s.Write(addSP_);
        }
    };
    public List<Piece> infos_;
    public System.UInt64 userGold_;         // 판매 금액이 적용된 현재 유저의 금화
    public System.UInt64 userSP_;           // 판매 금액이 적용된 현재 유저의 카드(서포터) 포인트
};
// 무기 락 패킷 메시지
public class PktInfoWeaponLock : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt64 weaponUID_;    // 무기 고유 ID - (클라이언트에서 채워줘야 할 값)
        public System.Boolean lock_;        // 락 여부 - (클라이언트에서 채워줘야 할 값)
        public override bool Read(Message _s)
        {
            if (false == _s.Read(out weaponUID_)) return false;
            return _s.Read(out lock_);
        }
        public override void Write(Message _s)
        {
            _s.Write(weaponUID_);
            _s.Write(lock_);
        }
    };
    public List<Piece> infos_;
};
// 무기 성장 패킷 메시지
public class PktInfoWeaponGrow : PktMsgType
{
    public PktInfoProductComGrowAck comGrow_;   // 공용 제품 레벨업 응답 정보
    public PktInfoWeaponBook bookStates_;   // 도감의 변경된 상태 알려주는 정보
    public System.Byte retSkillLv_;         // 결과 스킬 레벨
    public System.Byte retWake_;            // 결과 각성 단계
    public System.Byte retEnc_;             // 결과 인챈 수치
    public eGrowState growState_;           // 성장 시도 결과
};
// 무기 슬롯 곡옥 적용 패킷 메시지
public class PktInfoWeaponSlotGem : PktMsgType
{
    public System.UInt64[] gemUIDs_ = new System.UInt64[(int)PktInfoWeapon.Slot.ENUM._MAX_];   // 슬롯에 위치할 곡옥 고유 ID들 - (클라이언트에서 채워줘야 할 값)
    public System.UInt64 weaponUID_;    // 무기 고유 ID - (클라이언트에서 채워줘야 할 값)
};
// 무기 창고 패킷 메시지
public class PktInfoWpnDepotSet : PktMsgType
{
    public class Value : PktMsgType
    {
        public System.Byte maxCnt_;        // 슬롯 최대 크기
        public override bool Read(Message _s) {
            return _s.Read(out maxCnt_);
        }
        public override void Write(Message _s) {
            _s.Write(maxCnt_);
        }
    };
    public class Piece : PktMsgType
    {
        public System.UInt64 wnpUID_;       // 무기 고유 ID
        public override bool Read(Message _s) {
            return _s.Read(out wnpUID_);
        }
        public override void Write(Message _s) {
            _s.Write(wnpUID_);
        }
    };
    public List<Piece> infos_;
    public Value value_;
};
// 무기 창고 슬롯 확장 패킷 메시지
public class PktInfoWpnDepotSlotAdd : PktMsgType
{
    public PktInfoConsumeItemAndGoods comsume_; // 비용 소모가 적용된 아이템 및 재화
    public PktInfoWpnDepotSet.Value value_;     // 무기 창고 정보 값
};
// 무기 창고 슬롯에 무기 적용 패킷 메시지
public class PktInfoWpnDepotApply : PktMsgType
{
    public List<PktInfoWpnDepotSet.Piece> slots_;   // 무기 창고에 적용된 모든 슬롯 정보 - (클라이언트에서 채워줘야 할 값)
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 무기 패킷 메시지
    public static bool Read(Message _s, out PktInfoWeapon.Slot _v) {
        _v = new PktInfoWeapon.Slot();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoWeapon.Slot _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoWeapon.Piece _v) {
        _v = new PktInfoWeapon.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoWeapon.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoWeapon _v) {
        _v = new PktInfoWeapon();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoWeapon _v) {
        WriteList(_s, _v.infos_);
    }
    // 무기 판매 패킷 메시지
    public static bool Read(Message _s, out PktInfoWeaponSell.Piece _v) {
        _v = new PktInfoWeaponSell.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoWeaponSell.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoWeaponSell _v) {
        _v = new PktInfoWeaponSell();
        ReadList(_s, out _v.infos_);
        _s.Read(out _v.userGold_);
        return _s.Read(out _v.userSP_);
    }
    public static void Write(Message _s, PktInfoWeaponSell _v) {
        WriteList(_s, _v.infos_);
        _s.Write(_v.userGold_);
        _s.Write(_v.userSP_);
    }
    // 무기 락 패킷 메시지
    public static bool Read(Message _s, out PktInfoWeaponLock.Piece _v) {
        _v = new PktInfoWeaponLock.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoWeaponLock.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoWeaponLock _v) {
        _v = new PktInfoWeaponLock();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoWeaponLock _v) {
        WriteList(_s, _v.infos_);
    }
    // 무기 성장 패킷 메시지
    public static bool Read(Message _s, out PktInfoWeaponGrow _v) {
        _v = new PktInfoWeaponGrow();
        if (false == Read(_s, out _v.comGrow_)) return false;
        if (false == Read(_s, out _v.bookStates_)) return false;
        if (false == _s.Read(out _v.retSkillLv_)) return false;
        if (false == _s.Read(out _v.retWake_)) return false;
        if (false == _s.Read(out _v.retEnc_)) return false;
        if (false == Read(_s, out _v.growState_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoWeaponGrow _v) {
        Write(_s, _v.comGrow_);
        Write(_s, _v.bookStates_);
        _s.Write(_v.retSkillLv_);
        _s.Write(_v.retWake_);
        _s.Write(_v.retEnc_);
        Write(_s, _v.growState_);
    }
    // 무기 슬롯 곡옥 적용 패킷 메시지
    public static bool Read(Message _s, out PktInfoWeaponSlotGem _v) {
        _v = new PktInfoWeaponSlotGem();
        for (int loop = 0; loop < (int)PktInfoWeapon.Slot.ENUM._MAX_; ++loop)
        {
            if (false == _s.Read(out _v.gemUIDs_[loop]))
                return false;
        }
        return _s.Read(out _v.weaponUID_);
    }
    public static void Write(Message _s, PktInfoWeaponSlotGem _v) {
        for (int loop = 0; loop < (int)PktInfoWeapon.Slot.ENUM._MAX_; ++loop)
            _s.Write(_v.gemUIDs_[loop]);
        _s.Write(_v.weaponUID_);
    }
    // 무기 창고 패킷 메시지
    public static bool Read(Message _s, out PktInfoWpnDepotSet.Value _v) {
        _v = new PktInfoWpnDepotSet.Value();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoWpnDepotSet.Value _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoWpnDepotSet.Piece _v) {
        _v = new PktInfoWpnDepotSet.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoWpnDepotSet.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoWpnDepotSet _v) {
        _v = new PktInfoWpnDepotSet();
        if (false == ReadList(_s, out _v.infos_)) return false;
        return Read(_s, out _v.value_);
    }
    public static void Write(Message _s, PktInfoWpnDepotSet _v) {
        WriteList(_s, _v.infos_);
        Write(_s, _v);
    }
    // 무기 창고 슬롯 확장 패킷 메시지
    public static bool Read(Message _s, out PktInfoWpnDepotSlotAdd _v) {
        _v = new PktInfoWpnDepotSlotAdd();
        if (false == Read(_s, out _v.comsume_)) return false;
        if (false == Read(_s, out _v.value_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoWpnDepotSlotAdd _v) {
        Write(_s, _v.comsume_);
        Write(_s, _v.value_);
    }
    // 무기 창고 슬롯에 무기 적용 패킷 메시지
    public static bool Read(Message _s, out PktInfoWpnDepotApply _v) {
        _v = new PktInfoWpnDepotApply();
        return ReadList(_s, out _v.slots_);
    }
    public static void Write(Message _s, PktInfoWpnDepotApply _v) {
        WriteList(_s, _v.slots_);
    }
}
///
///////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////
///
// 분해 패킷 메시지 
public class PktInfoDecompositionReq : PktMsgType
{
    public eContentsPosKind kind_;              // 컨텐츠 종류
    public PktInfoUIDList decompositionList_;   // 분해한 UID 목록
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoDecompositionReq _v)
    {
        _v = new PktInfoDecompositionReq();
        if (false == Read(_s, out _v.kind_)) return false;
        if (false == Read(_s, out _v.decompositionList_)) return false;
        return true;
    }

    public static void Write(Message _s, PktInfoDecompositionReq _v)
    {
        PN_MarshalerEx.Write(_s, _v.kind_);
        PN_MarshalerEx.Write(_s, _v.decompositionList_);
    }
}
///
///////////////////////////////////////////////////////////////////////


///////////////////////////////////////////////////////////////////////
///
// 문양 패킷 메시지
public class PktInfoBadge : PktMsgType
{
    public class Option : PktMsgType
    {
        public enum ENUM
        {
            _1 = 0,
            _2,
            _3,

            _MAX_
        };
        public System.Byte optID_;
        public System.Byte value_;
        public override bool Read(Message _s) {
            if (false == _s.Read(out optID_)) return false;
            return _s.Read(out value_);
        }
        public override void Write(Message _s) {
            _s.Write(optID_);
            _s.Write(value_);
        }
    };
    public class Piece : PktMsgType
    {
        public Option[] opt_ = new Option[(int)Option.ENUM._MAX_];
        public System.UInt64 badgeUID_;
        public System.UInt64 posValue_;         // 문양 위치 적용 값
        public eContentsPosKind posKind_;       // 문양 위치 적용 타입
        public System.Byte posSlotNum_;         // 문양 사용 위치의 술롯 번호
        public System.Byte lv_;
        public System.Byte remainLvCnt_;
        public bool lock_;
        public override bool Read(Message _s) {
            for (int loop = 0; loop < (int)PktInfoBadge.Option.ENUM._MAX_; ++loop)
            {
                if (false == PN_MarshalerEx.Read(_s, out opt_[loop]))
                    return false;
            }
            if (false == _s.Read(out badgeUID_)) return false;
            if (false == _s.Read(out posValue_)) return false;
            if (false == PN_MarshalerEx.Read(_s, out posKind_)) return false;
            if (false == _s.Read(out posSlotNum_)) return false;
            if (false == _s.Read(out lv_)) return false;
            if (false == _s.Read(out remainLvCnt_)) return false;
            return _s.Read(out lock_);
        }
        public override void Write(Message _s) {
            for (int loop = 0; loop < (int)Option.ENUM._MAX_; ++loop)
                PN_MarshalerEx.Write(_s, opt_[loop]);
            _s.Write(badgeUID_);
            _s.Write(posValue_);
            PN_MarshalerEx.Write(_s, posKind_);
            _s.Write(posSlotNum_);
            _s.Write(lv_);
            _s.Write(remainLvCnt_);
            _s.Write(lock_);
        }
    };
    public List<Piece> infos_;
};
// 문양 위치 적용 패킷 메시지
public class PktInfoBadgeApplyPos : PktMsgType
{
    public System.UInt64 oldBadgeUID_;      // 기존에 적용중이였던 문양 고유 ID
    public System.UInt64 badgeUID_;         // 적용하려는 문양 고유 ID - (클라이언트에서 채워줘야 할 값)
    public System.UInt64 posValue_;         // 위치 값 - (클라이언트에서 채워줘야 할 값)
    public eContentsPosKind posKind_;       // 위치 분류 - (클라이언트에서 채워줘야 할 값)
    public System.Byte slotNum_;            // 적용 슬롯 번호 - (클라이언트에서 채워줘야 할 값)
    public System.Byte oldBadgeChangeSlotNum_;  // 기존에 적용중이였던 문양이 교체 됐을 경우 슬롯 번호(교체가 아니면 eBadgeSlotPosMax::_SLOT_MAX_값입니다.)
};
// 문양 위치 적용 해제 패킷 메시지
public class PktInfoBadgeApplyOutPos : PktMsgType
{
    public System.UInt64 badgeUID_;         // 해제 하려는 문양 고유 ID
};
// 문양 판매 패킷 메시지
public class PktInfoBadgeSell : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt64 badgeUID_;     // 문양 고유 ID - (클라이언트에서 채워줘야 할 값)
        public System.UInt32 addBTCoin_;    // 판매해서 추가된 배클 코인
        public override bool Read(Message _s) {
            if (false == _s.Read(out badgeUID_)) return false;
            return _s.Read(out addBTCoin_);
        }
        public override void Write(Message _s) {
            _s.Write(badgeUID_);
            _s.Write(addBTCoin_);
        }
    };
    public List<Piece> infos_;
    public System.UInt64 userBTCoin_;       // 판매 금액이 적용된 현재 유저의 배틀 코인
};
// 문양 락 패킷 메시지
public class PktInfoBadgeLock : PktMsgType
{
    public class Piece : PktMsgType
    {
        public System.UInt64 badgeUID_;     // 문양 고유 ID - (클라이언트에서 채워줘야 할 값)
        public System.Boolean lock_;        // 락 여부 - (클라이언트에서 채워줘야 할 값)
        public override bool Read(Message _s) {
            if (false == _s.Read(out badgeUID_)) return false;
            return _s.Read(out lock_);
        }
        public override void Write(Message _s) {
            _s.Write(badgeUID_);
            _s.Write(lock_);
        }
    };
    public List<Piece> infos_;
};
// 원하는 문양에 대한 공용 요청 패킷 메시지
public class PktInfoBadgeComReq : PktMsgType
{
    public System.UInt64 badgeUID_;         // 요청할 문양 고유 ID
};
// 문양 강화 관련 패킷 메시지
public class PktInfoBadgeUpgrade : PktMsgType
{
    public PktInfoConsumeItem maters_;      // 강화관련해서 소모된 재료 아이템 정보
    public System.UInt64 badgeUID_;         // 강화관련 변경이 이루어진 문양 고유 ID
    public System.UInt64 userGold_;         // 강화관련 작업 진행후 남은 현재 금화
    public System.Byte retLv_;              // 강화관련 변경이 이루어진 현재 레벨
    public System.Byte retRemainLvUpCnt_;   // 강화관련 변경이 이루어진 남은 강화 시도 횟수
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 문양 패킷 메시지
    public static bool Read(Message _s, out PktInfoBadge.Option _v) {
        _v = new PktInfoBadge.Option();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoBadge.Option _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoBadge.Piece _v) {
        _v = new PktInfoBadge.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoBadge.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoBadge _v) {
        _v = new PktInfoBadge();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoBadge _v) {
        WriteList(_s, _v.infos_);
    }
    // 문양 위치 적용 패킷 메시지
    public static bool Read(Message _s, out PktInfoBadgeApplyPos _v) {
        _v = new PktInfoBadgeApplyPos();
        if (false == _s.Read(out _v.oldBadgeUID_)) return false;
        if (false == _s.Read(out _v.badgeUID_)) return false;
        if (false == _s.Read(out _v.posValue_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.posKind_)) return false;
        if (false == _s.Read(out _v.slotNum_)) return false;
        if (false == _s.Read(out _v.oldBadgeChangeSlotNum_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoBadgeApplyPos _v) {
        _s.Write(_v.oldBadgeUID_);
        _s.Write(_v.badgeUID_);
        _s.Write(_v.posValue_);
        PN_MarshalerEx.Write(_s, _v.posKind_);
        _s.Write(_v.slotNum_);
        _s.Write(_v.oldBadgeChangeSlotNum_);
    }
    // 문양 위치 적용 해제 패킷 메시지
    public static bool Read(Message _s, out PktInfoBadgeApplyOutPos _v) {
        _v = new PktInfoBadgeApplyOutPos();
        return _s.Read(out _v.badgeUID_);
    }
    public static void Write(Message _s, PktInfoBadgeApplyOutPos _v) {
        _s.Write(_v.badgeUID_);
    }
    // 문양 판매 패킷 메시지
    public static bool Read(Message _s, out PktInfoBadgeSell.Piece _v) {
        _v = new PktInfoBadgeSell.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoBadgeSell.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoBadgeSell _v) {
        _v = new PktInfoBadgeSell();
        ReadList(_s, out _v.infos_);
        return _s.Read(out _v.userBTCoin_);
    }
    public static void Write(Message _s, PktInfoBadgeSell _v) {
        WriteList(_s, _v.infos_);
        _s.Write(_v.userBTCoin_);
    }
    // 문양 락 패킷 메시지
    public static bool Read(Message _s, out PktInfoBadgeLock.Piece _v) {
        _v = new PktInfoBadgeLock.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoBadgeLock.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoBadgeLock _v) {
        _v = new PktInfoBadgeLock();
        return ReadList(_s, out _v.infos_);
    }
    public static void Write(Message _s, PktInfoBadgeLock _v) {
        WriteList(_s, _v.infos_);
    }
    // 원하는 문양에 대한 공용 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoBadgeComReq _v) {
        _v = new PktInfoBadgeComReq();
        if (false == _s.Read(out _v.badgeUID_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoBadgeComReq _v) {
        _s.Write(_v.badgeUID_);
    }
    // 문양 강화 관련 패킷 메시지
    public static bool Read(Message _s, out PktInfoBadgeUpgrade _v) {
        _v = new PktInfoBadgeUpgrade();
        if (false == Read(_s, out _v.maters_)) return false;
        if (false == _s.Read(out _v.badgeUID_)) return false;
        if (false == _s.Read(out _v.userGold_)) return false;
        if (false == _s.Read(out _v.retLv_)) return false;
        if (false == _s.Read(out _v.retRemainLvUpCnt_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoBadgeUpgrade _v) {
        Write(_s, _v.maters_);
        _s.Write(_v.badgeUID_);
        _s.Write(_v.userGold_);
        _s.Write(_v.retLv_);
        _s.Write(_v.retRemainLvUpCnt_);
    }
}
///
///////////////////////////////////////////////////////////////////////


///////////////////////////////////////////////////////////////////////
///
// 컨텐츠 포지션 캐릭터 세부정보
public class PktInfoConPosCharDetail : PktMsgType
{
    public class ComInfo : PktMsgType      // 무기, 서포터 정보
    {
        public System.UInt32 ID_;       // TID
        public System.Byte lv_;         // 레벨
        public System.Byte skilLv_;     // 스킬레벨
        public System.Byte wake_;       // 각성(서포터), 재련(무기) 단계
        public System.Byte enc_;        // 인챈 수치
		public byte setID_;				// 세트속성 아이디


		public override bool Read(Message _s) {
            if (false == _s.Read(out ID_)) return false;
            if (false == _s.Read(out lv_)) return false;
            if (false == _s.Read(out skilLv_)) return false;
            if (false == _s.Read(out wake_)) return false;
            if (false == _s.Read(out enc_)) return false;
			if (false == _s.Read(out setID_)) return false;
			return true;
        }
        public override void Write(Message _s) {
            _s.Write(ID_);
            _s.Write(lv_);
            _s.Write(skilLv_);
            _s.Write(wake_);
            _s.Write(enc_);
			_s.Write(setID_);
		}
    };

    public class GemInfo : PktMsgType
    {
        public PktInfoGem.Option[] opt_ = new PktInfoGem.Option[(int)PktInfoGem.Option.ENUM._MAX_]; // 랜덤 옵션
        public ComInfo info_;                   // 곡옥 성장 정보

        public override bool Read(Message _s) {
            for (int loop = 0; loop < (int)PktInfoGem.Option.ENUM._MAX_; ++loop)
            {
                if (false == PN_MarshalerEx.Read(_s, out opt_[loop]))
                    return false;
            }
            if (false == PN_MarshalerEx.Read(_s, out info_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            for (int loop = 0; loop < (int)PktInfoGem.Option.ENUM._MAX_; ++loop)
                PN_MarshalerEx.Write(_s, opt_[loop]);
            PN_MarshalerEx.Write(_s, info_);
        }
    };

    public class WpnInfo : PktMsgType
    {
        public GemInfo[] gems_ = new GemInfo[(int)PktInfoWeapon.Slot.ENUM._MAX_]; // 곡옥 정보
        public ComInfo info_;                   // 무기 정보

        public override bool Read(Message _s) {
            for (int loop = 0; loop < (int)PktInfoWeapon.Slot.ENUM._MAX_; ++loop)
            {
                if (false == PN_MarshalerEx.Read(_s, out gems_[loop]))
                    return false;
            }
            if (false == PN_MarshalerEx.Read(_s, out info_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            for (int loop = 0; loop < (int)PktInfoWeapon.Slot.ENUM._MAX_; ++loop)
                PN_MarshalerEx.Write(_s, gems_[loop]);
            PN_MarshalerEx.Write(_s, info_);
        }
    };

    public class CardInfo : PktMsgType
    {
        public ComInfo info_;                   // 서포터 성장 정보
        public System.Byte type_;               // 속성 정보

        public override bool Read(Message _s) {
            if (false == PN_MarshalerEx.Read(_s, out info_))  return false;
            if (false == _s.Read(out type_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            PN_MarshalerEx.Write(_s, info_);
            _s.Write(type_);
        }
    };
    public class CSkill : PktMsgType
    {
        public System.UInt32 id_;               // 캐릭터 패시브 스킬 ID
        public System.Byte lv_;                 // 캐릭터 패시브 스킬 레벨

        public override bool Read(Message _s) {
            if (false == _s.Read(out id_)) return false;
            return _s.Read(out lv_);
        }
        public override void Write(Message _s) {
            _s.Write(id_);
            _s.Write(lv_);
        }
    };
    public class ASkill : PktMsgType
    {
        public System.Byte id_;                 // 각성 스킬 ID
        public System.Byte lv_;                 // 각성 스킬 레벨

        public override bool Read(Message _s) {
            if (false == _s.Read(out id_)) return false;
            return _s.Read(out lv_);
        }
        public override void Write(Message _s) {
            _s.Write(id_);
            _s.Write(lv_);
        }
    };

    public class CostumeDyeing : PktMsgType
    {
        public bool isFirstDyeing_;                 // 각성 스킬 ID
        public PktInfoColor part1_;                 // 각성 스킬 레벨
        public PktInfoColor part2_;
        public PktInfoColor part3_;


    };

    public List<CSkill> cskls_;             // 캐릭터 스킬 정보
    public List<ASkill> askls_;             // 각성 스킬 정보
    public WpnInfo[] wpns_ = new WpnInfo[(int)PktInfoChar.WpnSlot.Enum._MAX_];  // 무기 정보
    public CardInfo[] cards_ = new CardInfo[(int)eCardSlotPosMax.CHAR];         // 서포터 정보
    public System.UInt32[] sklIDs_ = new System.UInt32[(int)PktInfoChar.SkillSlot._MAX_]; // 장착 스킬 ID
    public System.UInt32 charID_;           // 캐릭터 TID
    public System.UInt32 skinStateFlag_;    // 적용된 스킨 상태 플레그
    public System.UInt32 costumeID_;        // 캐릭터 코스튬ID
    public System.Byte costumeClr_;         // 코스츔 색
    public System.Byte lv_;                 // 레벨
    public System.Byte grade_;              // 등급
    public CostumeDyeing costumeDyeing_;    // 염색정보
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoConPosCharDetail.CostumeDyeing _v)
    {
        _v = new PktInfoConPosCharDetail.CostumeDyeing();
        if (false == _s.Read(out _v.isFirstDyeing_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.part1_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.part2_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.part3_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoConPosCharDetail.CostumeDyeing _v)
    {
        _s.Write(_v.isFirstDyeing_);
        PN_MarshalerEx.Write(_s, _v.part1_);
        PN_MarshalerEx.Write(_s, _v.part2_);
        PN_MarshalerEx.Write(_s, _v.part3_);
    }

    // 랭킹 등록된 유저의 자세한 정보 응답
    public static bool Read(Message _s, out PktInfoConPosCharDetail.ComInfo _v) {
        _v = new PktInfoConPosCharDetail.ComInfo();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoConPosCharDetail.ComInfo _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoConPosCharDetail.GemInfo _v) {
        _v = new PktInfoConPosCharDetail.GemInfo();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoConPosCharDetail.GemInfo _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoConPosCharDetail.WpnInfo _v) {
        _v = new PktInfoConPosCharDetail.WpnInfo();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoConPosCharDetail.WpnInfo _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoConPosCharDetail.CardInfo _v) {
        _v = new PktInfoConPosCharDetail.CardInfo();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoConPosCharDetail.CardInfo _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoConPosCharDetail.ASkill _v) {
        _v = new PktInfoConPosCharDetail.ASkill();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoConPosCharDetail.ASkill _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoConPosCharDetail _v) {
        _v = new PktInfoConPosCharDetail();
        if (false == ReadList(_s, out _v.cskls_)) return false;
        if (false == ReadList(_s, out _v.askls_)) return false;
        for (int loop = 0; loop < (int)PktInfoChar.WpnSlot.Enum._MAX_; ++loop)
        {
            if (false == Read(_s, out _v.wpns_[loop]))
                return false;
        }
        for (int loop = 0; loop < (int)eCardSlotPosMax.CHAR; ++loop)
        {
            if (false == Read(_s, out _v.cards_[loop]))
                return false;
        }
        for (int loop = 0; loop < (int)PktInfoChar.SkillSlot._MAX_; ++loop)
        {
            if (false == _s.Read(out _v.sklIDs_[loop]))
                return false;
        }
        if (false == _s.Read(out _v.charID_)) return false;
        if (false == _s.Read(out _v.skinStateFlag_)) return false;
        if (false == _s.Read(out _v.costumeID_)) return false;
        if (false == _s.Read(out _v.costumeClr_)) return false;
        if (false == _s.Read(out _v.lv_)) return false;
        if (false == _s.Read(out _v.grade_)) return false;
        if (false == Read(_s, out _v.costumeDyeing_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoConPosCharDetail _v) {
        WriteList(_s, _v.cskls_);
        WriteList(_s, _v.askls_);
        for (int loop = 0; loop < (int)PktInfoChar.WpnSlot.Enum._MAX_; ++loop)
            Write(_s, _v.wpns_[loop]);
        for (int loop = 0; loop < (int)eCardSlotPosMax.CHAR; ++loop)
            Write(_s, _v.cards_[loop]);
        for (int loop = 0; loop < (int)PktInfoChar.SkillSlot._MAX_; ++loop)
            _s.Write(_v.sklIDs_[loop]);
        _s.Write(_v.charID_);
        _s.Write(_v.skinStateFlag_);
        _s.Write(_v.costumeID_);
        _s.Write(_v.costumeClr_);
        _s.Write(_v.lv_);
        _s.Write(_v.grade_);
        Write(_s, _v.costumeDyeing_);
    }
}
///
///////////////////////////////////////////////////////////////////////