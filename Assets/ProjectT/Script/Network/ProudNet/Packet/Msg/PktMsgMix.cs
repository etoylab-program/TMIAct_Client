using System.Collections.Generic;
using Nettention.Proud;



public class PktInfoCharSetMainCostumeAck : PktMsgType
{
    public System.UInt32 costumeTID_;       // 코스츔 테이블 ID - (클라이언트에서 채워줘야 할 값)
    public PktInfoCharSkinColor skinColor_; // 스킨 컬러 정보 - (클라이언트에서는 cuid_ 값만 채워주면됨)
    public PktInfoCostume.Piece costume_;   // 염색시 적용된 컬러 정보
};

public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoCharSetMainCostumeAck _v)
    {
        _v = new PktInfoCharSetMainCostumeAck();
        if (false == _s.Read(out _v.costumeTID_)) return false;
        if (false == Read(_s, out _v.skinColor_)) return false;
        if (false == Read(_s, out _v.costume_)) return false;

        return true;
    }
    public static void Write(Message _s, PktInfoCharSetMainCostumeAck _v)
    {
        _s.Write(_v.costumeTID_);
        Write(_s, _v.skinColor_);
        Write(_s, _v.costume_);
    }
}
// 캐릭터 추가 관련 정보 패킷 메시지
public class PktInfoAddChar : PktMsgType
{
    public class Piece : PktMsgType {
        public PktInfoChar char_;                   // 캐릭터 정보
        public PktInfoCharSkill charSkills_;        // 캐릭터 스킬 정보
        public PktInfoCostume costumes_;            // 코스츔 정보
        public PktInfoWeapon weapons_;              // 무기 정보
        public PktInfoWeaponBook weaponBooks_;      // 무기 도감 정보
        public override bool Read(Message _s) {
            if (false == PN_MarshalerEx.Read(_s, out char_)) return false;
            if (false == PN_MarshalerEx.Read(_s, out charSkills_)) return false;
            if (false == PN_MarshalerEx.Read(_s, out costumes_)) return false;
            if (false == PN_MarshalerEx.Read(_s, out weapons_)) return false;
            if (false == PN_MarshalerEx.Read(_s, out weaponBooks_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            PN_MarshalerEx.Write(_s, char_);
            PN_MarshalerEx.Write(_s, charSkills_);
            PN_MarshalerEx.Write(_s, costumes_);
            PN_MarshalerEx.Write(_s, weapons_);
            PN_MarshalerEx.Write(_s, weaponBooks_);
        }
    };
    public List<Piece> infos_;
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoAddChar _v) {
        _v = new PktInfoAddChar();
        if (false == ReadList(_s, out _v.infos_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoAddChar _v) {
        WriteList(_s, _v.infos_);
    }
}

// 아이템 제품군 정보 패킷 메시지
public class PktInfoProductPack : PktMsgType
{
    public class Goods : PktMsgType {
        public System.Int32 value_;                 // 획득한 재화
        public eGOODSTYPE type_;                    // 획득한 재화 타입
        public override bool Read(Message _s) {
            if (false == _s.Read(out value_)) return false;
            if (false == PN_MarshalerEx.Read(_s, out type_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(value_);
            PN_MarshalerEx.Write(_s, type_);
        }
    };
    // 가챠(뽑기) 획득 정보
    public class Lottery : PktMsgType {
        public enum TYPE : System.Byte {
            _NONE_,

            GRADE_UP,               // 등급업

            SPT_DROP,               // 서포터 드롭 일반
            SPT_DROP_WAKE,          // 서포터 드롭 각성

            BUFF_DROP,              // 버프 드롭 일반(헤비시카)
            BUFF_DROP_ACTIVE,       // 버프 드롭 액티브(친밀도)

            _MAX_,
        };
        public System.UInt64 uid_;                  // 뽑힌 제품의 UID
        public System.UInt32 idx_;                  // 뽑힌 제품의 Idx(보통은 테이블 ID로 사용됨)
        public System.UInt16 value_;                // 뽑힌 제품의 Value(보통은 개수로 사용됨)
        public eREWARDTYPE rwdTP_;                  // 뽑힌 제품의 타입
        public System.Byte dropTP_;                 // 획득한 방식 타입 Lottery::TYPE
        public override bool Read(Message _s) {
            if (false == _s.Read(out uid_)) return false;
            if (false == _s.Read(out idx_)) return false;
            if (false == _s.Read(out value_)) return false;
            if (false == PN_MarshalerEx.Read(_s, out rwdTP_)) return false;
            if (false == _s.Read(out dropTP_)) return false;
            return true;
        }
        public override void Write(Message _s) {
            _s.Write(uid_);
            _s.Write(idx_);
            _s.Write(value_);
            PN_MarshalerEx.Write(_s, rwdTP_);
            _s.Write(dropTP_);
        }
    };
    public PktInfoGoodsAll oldGoods;                // 기존 재화 정보 - (##S 서버용 참조 데이터라서 데이타 전송시 보내지 않습니다.)
    public List<Goods> goodsInfos_;                 // 변경된 재화 정보 로그용
    public List<PktInfoAddChar.Piece> charInfos_;
    public List<Lottery> lotteryInfos_;             // 랜덤으로 나온 결과물 정보입니다.
    public PktInfoAddItem addItemInfos_;            // 뷰어 및 로그용 입니다.
    public PktInfoCostume costumes_;
    public PktInfoCard cards_;
    public PktInfoCardBook cardBooks_;
    public PktInfoItem items_;                      // ItemPack 내용을 적용한 최종 획득 Item 정보(같은 TID가 하나로 합쳐진 데이터입니다.)
    public PktInfoGem gems_;
    public PktInfoWeapon weapons_;
    public PktInfoWeaponBook weaponBooks_;
    public PktInfoBadge badges_;                    // 유저 문양 정보
    public PktInfoMonthlyFee monFees_;              // 갱신된 월정액 정보
    public PktInfoEffect effs_;                     // 갱신된 효과 정보
    public PktInfoUserMarkTake marks_;              // 유저 마크 정보
    public PktInfoLobbyThemeTake lobbyTheme_;       // 로비 테마 정보
    public PktInfoUIDValue charAni_;                // 캐릭터 에니 정보
    public PktInfoPass pass_;                       // 패스 정보
    public PktInfoUnexpectedPackage unexpectedPackage_; // 추가된 돌발 패키지 정보
    public PktInfoMail mailes_;                     // 추가된 메일 목록
    public PktInfoGoods retGoods_;                  // GoodsPack 내용과 유저의 현재 금액이 누적된 최종 결과 재화
	public PktInfoChatStampTake chatStamp_;			// 획득한 채팅 스탬프 정보

	public bool IsNew(Lottery _lottery)
    {
        System.UInt32 tid = _lottery.idx_;
        switch (_lottery.rwdTP_)
        {
            case eREWARDTYPE.CARD: {
                    foreach (var info in cardBooks_.infos_) {
                        if (tid == info.tableID_)
                            return true;
                    }
                } break;
            case eREWARDTYPE.WEAPON: {
                    foreach (var info in weaponBooks_.infos_) {
                        if (tid == info.tableID_)
                            return true;
                    }
                } break;
        }
        return false;
    }
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoProductPack _v) {
        _v = new PktInfoProductPack();
        if (false == Read(_s, out _v.oldGoods)) return false;
        if (false == ReadList(_s, out _v.goodsInfos_)) return false;
        if (false == ReadList(_s, out _v.charInfos_)) return false;
        if (false == ReadList(_s, out _v.lotteryInfos_)) return false;
        if (false == Read(_s, out _v.addItemInfos_)) return false;
        if (false == Read(_s, out _v.costumes_)) return false;
        if (false == Read(_s, out _v.cards_)) return false;
        if (false == Read(_s, out _v.cardBooks_)) return false;
        if (false == Read(_s, out _v.items_)) return false;
        if (false == Read(_s, out _v.gems_)) return false;
        if (false == Read(_s, out _v.weapons_)) return false;
        if (false == Read(_s, out _v.weaponBooks_)) return false;
        if (false == Read(_s, out _v.badges_)) return false;
        if (false == Read(_s, out _v.monFees_)) return false;
        if (false == Read(_s, out _v.effs_)) return false;
        if (false == Read(_s, out _v.marks_)) return false;
        if (false == Read(_s, out _v.chatStamp_)) return false;
        if (false == Read(_s, out _v.lobbyTheme_)) return false;
        if (false == Read(_s, out _v.charAni_)) return false;
        if (false == Read(_s, out _v.pass_)) return false;
        if (false == Read(_s, out _v.unexpectedPackage_)) return false;
        if (false == Read(_s, out _v.mailes_)) return false;
        if (false == Read(_s, out _v.retGoods_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoProductPack _v) {
        Write(_s, _v.oldGoods);
        WriteList(_s, _v.goodsInfos_);
        WriteList(_s, _v.charInfos_);
        WriteList(_s, _v.lotteryInfos_);
        Write(_s, _v.addItemInfos_);
        Write(_s, _v.costumes_);
        Write(_s, _v.cards_);
        Write(_s, _v.cardBooks_);
        Write(_s, _v.items_);
        Write(_s, _v.gems_);
        Write(_s, _v.weapons_);
        Write(_s, _v.weaponBooks_);
        Write(_s, _v.badges_);
        Write(_s, _v.monFees_);
        Write(_s, _v.effs_);
        Write(_s, _v.marks_);
        Write(_s, _v.chatStamp_);
        Write(_s, _v.lobbyTheme_);
        Write(_s, _v.charAni_);
        Write(_s, _v.pass_);
        Write(_s, _v.unexpectedPackage_);
        Write(_s, _v.mailes_);
        Write(_s, _v.retGoods_);
    }
}

// 인앱 제품 구매 정보 요청 패킷 메시지
public class PktInfoStorePurchaseInAppReq : PktMsgType
{
    public PktInfoStr receipt_;                     // 구매한 영수증 내용 - 스팀의 경우 {"orderid":"","Authorized":} 형태로 넣어주세요
    public System.UInt32 storeID_;                  // 구매한 상점 ID
    public eInAppKind inappKind_;                   // 인앱 상점 타입
}
// 제품 구매 요청 패킷 메시지
public class PktInfoStorePurchaseReq : PktMsgType
{
    public System.UInt32 tutoVal_;                   // 튜토리얼 값
    public System.UInt32 storeID_;                   // 상점 ID
    public System.Byte purchaseCnt_;                 // 구매 상품 수
}
// 제품 구매 정보 패킷 메시지
public class PktInfoStorePurchase : PktMsgType
{
    public PktInfoProductPack products_;            // 구매한 재품 정보
    public PktInfoConsumeItem consumeItem_;         // 아이템 소비로 구매하는 경우 소비한 아이템
    public PktInfoStoreSale addSaleInfo_;           // 추가된 무료 상점 정보
    public PktInfoStoreSale updateSaleInfo_;        // 변경된 무료 상점 정보
    public System.UInt32 storeID_;                  // 구매를 요청한 해당 상품 ID
    public System.Byte tutoVal_;                    // 튜토리얼 값
    public System.Byte purchaseCnt_;                // 구매 상품 수
}
// 스팀 결제 요청 패킷 메시지
public class PktInfoSteamPurchaseReq : PktMsgType
{
    public System.String languageCode_;     // 언어코드 
    public System.String itemDesc_;         // 아이템 설명
    public System.UInt64 steamID_;          // 스팀 ID
    public System.UInt32 storeID_;          // 상점 ID
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoStorePurchaseInAppReq _v) {
        _v = new PktInfoStorePurchaseInAppReq();
        if (false == Read(_s, out _v.receipt_)) return false;
        if (false == _s.Read(out _v.storeID_)) return false;
        if (false == Read(_s, out _v.inappKind_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoStorePurchaseInAppReq _v) {
        Write(_s, _v.receipt_);
        _s.Write(_v.storeID_);
        Write(_s, _v.inappKind_);
    }
    public static bool Read(Message _s, out PktInfoStorePurchaseReq _v) {
        _v = new PktInfoStorePurchaseReq();
        if (false == _s.Read(out _v.tutoVal_)) return false;
        if (false == _s.Read(out _v.storeID_)) return false;
        if (false == _s.Read(out _v.purchaseCnt_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoStorePurchaseReq _v) {
        _s.Write(_v.tutoVal_);
        _s.Write(_v.storeID_);
        _s.Write(_v.purchaseCnt_);
    }
    public static bool Read(Message _s, out PktInfoStorePurchase _v) {
        _v = new PktInfoStorePurchase();
        if (false == Read(_s, out _v.products_)) return false;
        if (false == Read(_s, out _v.consumeItem_)) return false;
        if (false == Read(_s, out _v.addSaleInfo_)) return false;
        if (false == Read(_s, out _v.updateSaleInfo_)) return false;
        if (false == _s.Read(out _v.storeID_)) return false;
        if (false == _s.Read(out _v.tutoVal_)) return false;
        if (false == _s.Read(out _v.purchaseCnt_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoStorePurchase _v) {
        Write(_s, _v.products_);
        Write(_s, _v.consumeItem_);
        Write(_s, _v.addSaleInfo_);
        Write(_s, _v.updateSaleInfo_);
        _s.Write(_v.storeID_);
        _s.Write(_v.tutoVal_);
        _s.Write(_v.purchaseCnt_);
    }
    public static bool Read(Message _s, out PktInfoSteamPurchaseReq _v)
    {
        _v = new PktInfoSteamPurchaseReq();
        if (false == _s.Read(out _v.languageCode_)) return false;
        if (false == _s.Read(out _v.itemDesc_)) return false;
        if (false == _s.Read(out _v.steamID_)) return false;
        if (false == _s.Read(out _v.storeID_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoSteamPurchaseReq _v)
    {
        Write(_s, _v.languageCode_);
        Write(_s, _v.itemDesc_);
        _s.Write(_v.steamID_);
        _s.Write(_v.storeID_);
    }
}

// 분해 결과 응답 패킷 메시지
public class PktInfoDecompositionAck : PktMsgType
{
    public eContentsPosKind kind_;                  // 컨텐츠 종류
    public PktInfoUIDList decompositionList_;       // 분해할 UID 목록
    public PktInfoProductPack takeProduct_;         // 분해한 결과로 받은 product
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoDecompositionAck _v)
    {
        _v = new PktInfoDecompositionAck();
        if (false == Read(_s, out _v.kind_)) return false;
        if (false == Read(_s, out _v.decompositionList_)) return false;
        if (false == Read(_s, out _v.takeProduct_)) return false;
        return true;
    }

    public static void Write(Message _s, PktInfoDecompositionAck _v)
    {
        PN_MarshalerEx.Write(_s, _v.kind_);
        PN_MarshalerEx.Write(_s, _v.decompositionList_);
        PN_MarshalerEx.Write(_s, _v.takeProduct_);
    }
}

// 돌발 패키지 데일리 보상 응답 메시지
public class PktInfoUnexpectedPackageDailyRewardAck : PktMsgType
{
    public PktInfoUnexpectedPackage.Piece piece_;   // 업데이트된 돌발 패키지
    public PktInfoProductPack productReward_;       // 보상 정보
 };
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoUnexpectedPackageDailyRewardAck _v)
    {
        _v = new PktInfoUnexpectedPackageDailyRewardAck();
        if (false == Read(_s, out _v.piece_)) return false;
        if (false == Read(_s, out _v.productReward_)) return false;
        return true;
    }

    public static void Write(Message _s, PktInfoUnexpectedPackageDailyRewardAck _v)
    {
        PN_MarshalerEx.Write(_s, _v.piece_);
        PN_MarshalerEx.Write(_s, _v.productReward_);
    }
   
}

// 유저 프리셋 정보 패킷 메시지
public class PktInfoUserPreset : PktMsgType
{
	public class CharData : PktMsgType
	{
		public class WpnInfo : PktMsgType
		{
			public PktInfoChar.WpnSlot		wpn_;
			public PktInfoWeapon.Slot[]		gems_ = new PktInfoWeapon.Slot[(int)PktInfoWeapon.Slot.ENUM._MAX_];

			public override bool Read(Message _s)
			{
				if (false == PN_MarshalerEx.Read(_s, out wpn_)) return false;
				for (int loop = 0; loop < (int)PktInfoWeapon.Slot.ENUM._MAX_; ++loop)
				{
					if (false == PN_MarshalerEx.Read(_s, out gems_[loop]))
						return false;
				}
				return true;
			}
			public override void Write(Message _s)
			{
				PN_MarshalerEx.Write(_s, wpn_);
				for (int loop = 0; loop < (int)PktInfoWeapon.Slot.ENUM._MAX_; ++loop)
					PN_MarshalerEx.Write(_s, gems_[loop]);
			}
		}

		public byte				slotNum_;		// 프리셋 슬롯 번호
		public byte				posValue_;		// 여러 캐릭터를 설정하는 컨텐츠의 편성된 위치값 ex)아레나 선봉,중견,대장 
		public WpnInfo[]		wpn_ = new WpnInfo[(byte)PktInfoChar.WpnSlot.Enum._MAX_];	// 장착된 무기정보
		public uint[]			skillIds_ = new uint[(byte)PktInfoChar.SkillSlot._MAX_];	// 설정된 스킬정보

		public PktInfoUIDList	cards_;			// 장착한 서포터 정보

		public ulong			cuid_;			// 캐릭터 고유 아이디
		public uint				costumeID_;		// 장착된 코스튬 아이디
		public byte				costumeClr_;	// 설정된 코스튬 컬러
		public uint				skinStateFlag_;	// 코스튬 악세사리 정보

		public override bool Read(Message _s)
		{
			for (int loop = 0; loop < (byte)PktInfoChar.WpnSlot.Enum._MAX_; ++loop)
				if (false == PN_MarshalerEx.Read(_s, out wpn_[loop])) return false;
			for (int loop = 0; loop < (byte)PktInfoChar.SkillSlot._MAX_; ++loop)
				if (false == _s.Read(out skillIds_[loop])) return false;

			if (false == _s.Read(out slotNum_))	return false;
			if (false == _s.Read(out posValue_)) return false;
			if (false == PN_MarshalerEx.Read(_s, out cards_)) return false;
			if (false == _s.Read(out cuid_)) return false;
			if (false == _s.Read(out costumeID_)) return false;
			if (false == _s.Read(out costumeClr_)) return false;
			if (false == _s.Read(out skinStateFlag_)) return false;
			return true;
		}

		public override void Write(Message _s)
		{
			for (int loop = 0; loop < (byte)PktInfoChar.WpnSlot.Enum._MAX_; ++loop)
				PN_MarshalerEx.Write(_s, wpn_[loop]);
			for (int loop = 0; loop < (byte)PktInfoChar.SkillSlot._MAX_; ++loop)
				_s.Write(skillIds_[loop]);

			_s.Write(slotNum_);
			_s.Write(posValue_);
			PN_MarshalerEx.Write(_s, cards_);
			_s.Write(cuid_);
			_s.Write(costumeID_);
			_s.Write(costumeClr_);
			_s.Write(skinStateFlag_);
		}
	}

	public class Piece : PktMsgType
	{
		public ePresetKind		kind_;			// 프리셋 유형
		public byte				slotNum_;		// 프리셋 슬롯번호
		public ulong			cuid_;			// 캐릭터 프리셋만 사용 (캐릭터별 구분값)
		public uint				cardFrmtID_;	// 서포터 진형 아이디(컨텐츠 프리셋)

		public List<CharData>	characters_;	// 편성된 캐릭터 정보
		public PktInfoStr		name_;			// 프리셋 이름
		public PktInfoUIDList	badgeUids_;		// 등록된 문양 uid 리스트(아레나, 아레나 타워 프리셋)

		public override bool Read(Message _s)
		{
			if (false == PN_MarshalerEx.ReadList(_s, out characters_)) return false;
			if (false == PN_MarshalerEx.Read(_s, out kind_)) return false;
			if (false == _s.Read(out slotNum_)) return false;
			if (false == _s.Read(out cuid_)) return false;
			if (false == _s.Read(out cardFrmtID_)) return false;
			if (false == PN_MarshalerEx.Read(_s, out name_)) return false;
			if (false == PN_MarshalerEx.Read(_s, out badgeUids_)) return false;
			return true;
		}

		public override void Write(Message _s)
		{
			PN_MarshalerEx.WriteList(_s, characters_);
			PN_MarshalerEx.Write(_s, kind_);
			_s.Write(slotNum_);
			_s.Write(cuid_);
			_s.Write(cardFrmtID_);
			PN_MarshalerEx.Write(_s, name_);
			PN_MarshalerEx.Write(_s, badgeUids_);
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
	public static bool Read(Message _s, out PktInfoUserPreset.CharData.WpnInfo _v)
	{
		_v = new PktInfoUserPreset.CharData.WpnInfo();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoUserPreset.CharData.WpnInfo _v)
	{
		_v.Write(_s);
	}

	public static bool Read(Message _s, out PktInfoUserPreset.CharData _v)
	{
		_v = new PktInfoUserPreset.CharData();
		return _v.Read(_s);
	}

	public static void Write(Message _s, PktInfoUserPreset.CharData _v)
	{
		_v.Write(_s);
	}

	public static bool Read(Message _s, out PktInfoUserPreset.Piece _v)
	{
		_v = new PktInfoUserPreset.Piece();
		return _v.Read(_s);
	}

	public static void Write(Message _s, PktInfoUserPreset.Piece _v)
	{
		_v.Write(_s);
	}

	public static bool Read(Message _s, out PktInfoUserPreset _v)
	{
		_v = new PktInfoUserPreset();
		return _v.Read(_s);
	}

	public static void Write(Message _s, PktInfoUserPreset _v)
	{
		_v.Write(_s);
	}
}

// 유저 프리셋 데이터 적용(불러오기) 패킷 메시지
public class PktInfoUserPresetLoad : PktMsgType
{
	public class DetachGem : PktMsgType
	{
		public ulong wpnUID_;
		public PktInfoWeapon.Slot[] gemSlotInfo_ = new PktInfoWeapon.Slot[(int)PktInfoWeapon.Slot.ENUM._MAX_];

		public override bool Read(Message _s)
		{
			if (false == _s.Read(out wpnUID_)) return false;
			for (int loop = 0; loop < (int)PktInfoWeapon.Slot.ENUM._MAX_; ++loop)
			{
				if (false == PN_MarshalerEx.Read(_s, out gemSlotInfo_[loop]))
					return false;
			}
			return true;
		}

		public override void Write(Message _s)
		{
			_s.Write(wpnUID_);
			for (int loop = 0; loop < (int)PktInfoWeapon.Slot.ENUM._MAX_; ++loop)
				PN_MarshalerEx.Write(_s, gemSlotInfo_[loop]);
		}
	}

	public List<DetachGem>			detachGemInfo_;		// 해제된 곡옥 정보
	public PktInfoUIDList			detachCards_;		// 해제된 카드(서포터) 정보
	public PktInfoUIDList			detachBadge_;       // 해제된 문양 정보 (아레나, 아레나 타워만 해당)

	public List<PktInfoCharEquipWeapon> affectOtherChars_;	//	무기 공유 시스템으로 해제되어야 할 캐릭터의 장비 정보

	public PktInfoUserPreset.Piece	loadInfo_;			// 불러오기 요청한 프리셋 정보

	public override bool Read(Message _s)
	{
		if (false == PN_MarshalerEx.ReadList(_s, out detachGemInfo_)) return false;
		if (false == PN_MarshalerEx.Read(_s, out detachCards_)) return false;
		if (false == PN_MarshalerEx.Read(_s, out detachBadge_)) return false;
		if (false == PN_MarshalerEx.ReadList(_s, out affectOtherChars_)) return false;
		if (false == PN_MarshalerEx.Read(_s, out loadInfo_)) return false;
		return true;
	}

	public override void Write(Message _s)
	{
		PN_MarshalerEx.WriteList(_s, detachGemInfo_);
		PN_MarshalerEx.Write(_s, detachCards_);
		PN_MarshalerEx.Write(_s, detachBadge_);
		PN_MarshalerEx.WriteList(_s, affectOtherChars_);
		PN_MarshalerEx.Write(_s, loadInfo_);
	}
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoUserPresetLoad _v)
	{
		_v = new PktInfoUserPresetLoad();
		return _v.Read(_s);
	}
	public static void Write(Message _s, PktInfoUserPresetLoad _v)
	{
		_v.Write(_s);
	}
}

	// 로그인 유저 정보 패킷 메시지
	public class PktInfoDetailUser : PktMsgType
{
    public List<PktInfoChar> charInfo_;             // 캐릭터 정보
    public PktInfoCharSkill charSkls_;              // 스킬 정보
    public PktInfoUserSkill userSkls_;              // 유저 스킬 정보
    public PktInfoUser userInfo_;                   // 유저 정보
    public PktInfoUserMark marks_;                  // 마크 정보
    public PktInfoLobbyTheme lobbyTheme_;        	// 로비 테마 정보
    public PktInfoCardFormaFavi cardFormaFavi_;     // 서포터 진형 즐겨찾기 정보
    public PktInfoFacility facilityes_;             // 시설 정보
    public PktInfoDispatch dispatches_;             // 파견 정보
    public PktInfoRoomPackage roomPackage_;         // 룸 테마 관련 묶음 정보
    public PktInfoCostume costumes_;                // 코스츔 정보
    public PktInfoStageClear stageClears_;          // 스테이지 클리어 정보
    public PktInfoStage stageInfo_;                 // 스테이지 정보
    public PktInfoUserRaid raid_;                   // 레이드 전투 정보
    public PktInfoTimeAtkStageRec timeRecord_;      // 타임어택 스테이지 최고기록 정보
    public PktInfoStoreSale storeSales_;            // 상점 세일 정보
    public PktInfoMonthlyFee monFees_;              // 월정액 정보
    public PktInfoRaidStore raidStore_;             // 레이드 상점 정보
    public PktInfoMonsterBook monsterBooks_;        // 몬스터 도감 정보
    public PktInfoCardBook cardBooks_;              // 카드 도감 정보
    public PktInfoCard cards_;                      // 카드 정보
    public PktInfoItem items_;                      // 아이템 정보
    public PktInfoGem gems_;                        // 곡옥 정보
    public PktInfoWeaponBook weaponBooks_;          // 무기 도감 정보
    public PktInfoWeapon weapons_;                  // 무기 정보
    public PktInfoWpnDepotSet wnpDepot_;            // 무기 창고 정보
    public PktInfoBadge badges_;                    // 문양 정보
    public PktInfoMail mails_;                      // 메일 정보
    public PktInfoPass pass_;                       // 패스 정보
    public PktInfoMission mission_;                 // 미션 정보
    public PktInfoEvent events_;                    // 이벤트 정보
    public PktInfoEffect effs_;                     // 효과 정보
    public PktInfoAchieve achieves_;                // 공적 정보
    public PktInfoUserArena arena_;                 // PVP 정보
    public PktInfoUserArenaTower arenaTower_;       // 아레나 타워 정보
    public PktInfoRelocateUser relocate_;           // 서버 이전 정보
    public PktInfoComTimeAndTID rgacha_;            // 로테이션 가챠 정보
    public PktInfoUnexpectedPackage unexpectedPackage_; // 돌발 패키지 정보
	public PktInfoUserBingoEvent bingoEventList_ ;       //	빙고 이벤트 진행 정보
    public PktInfoAchieveEvent achieveEventList_;   // 공적 이벤트 진행 정보
	public PktInfoUserPreset preset_;				// 유저 프리셋 정보 (로그인시 전달하지 않지만 서버 패킷 메시지 구조 동기화를 위해 추가)
	public PktInfoChatStamp chatStamp_;                  // 유저 채팅 스탬프 정보
	public PktInfoGoodsAll goodsInfo_;              // 재화 정보    <- 클라 서버간 디버그 확인용으로 가장 마지막에 위치하도록 합니다.
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoDetailUser _v) {
        _v = new PktInfoDetailUser();
        if (false == ReadList(_s, out _v.charInfo_)) return false;
        if (false == Read(_s, out _v.charSkls_)) return false;
        if (false == Read(_s, out _v.userSkls_)) return false;
        if (false == Read(_s, out _v.userInfo_)) return false;
        if (false == Read(_s, out _v.marks_)) return false;
        if (false == Read(_s, out _v.lobbyTheme_)) return false;
        if (false == Read(_s, out _v.cardFormaFavi_)) return false;
        if (false == Read(_s, out _v.facilityes_)) return false;
        if (false == Read(_s, out _v.dispatches_)) return false;
        if (false == Read(_s, out _v.roomPackage_)) return false;
        if (false == Read(_s, out _v.costumes_)) return false;
        if (false == Read(_s, out _v.stageClears_)) return false;
        if (false == Read(_s, out _v.stageInfo_)) return false;
        if (false == Read(_s, out _v.raid_)) return false;
        if (false == Read(_s, out _v.timeRecord_)) return false;
        if (false == Read(_s, out _v.storeSales_)) return false;
        if (false == Read(_s, out _v.monFees_)) return false;
        if (false == Read(_s, out _v.raidStore_)) return false;
        if (false == Read(_s, out _v.monsterBooks_)) return false;
        if (false == Read(_s, out _v.cardBooks_)) return false;
        if (false == Read(_s, out _v.cards_)) return false;
        if (false == Read(_s, out _v.items_)) return false;
        if (false == Read(_s, out _v.gems_)) return false;
        if (false == Read(_s, out _v.weaponBooks_)) return false;
        if (false == Read(_s, out _v.weapons_)) return false;
        if (false == Read(_s, out _v.wnpDepot_)) return false;
        if (false == Read(_s, out _v.badges_)) return false;
        if (false == Read(_s, out _v.mails_)) return false;
        if (false == Read(_s, out _v.pass_)) return false;
        if (false == Read(_s, out _v.mission_)) return false;
        if (false == Read(_s, out _v.events_)) return false;
        if (false == Read(_s, out _v.effs_)) return false;
        if (false == Read(_s, out _v.achieves_)) return false;
        if (false == Read(_s, out _v.arena_)) return false;
        if (false == Read(_s, out _v.arenaTower_)) return false;
        if (false == Read(_s, out _v.relocate_)) return false;
        if (false == Read(_s, out _v.rgacha_)) return false;
        if (false == Read(_s, out _v.unexpectedPackage_)) return false;
		if (false == Read(_s, out _v.bingoEventList_)) return false;
        if (false == Read(_s, out _v.achieveEventList_)) return false;
		if (false == Read(_s, out _v.preset_)) return false;
		if (false == Read(_s, out _v.chatStamp_)) return false;
		if (false == Read(_s, out _v.goodsInfo_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoDetailUser _v) {
        WriteList(_s, _v.charInfo_);
        Write(_s, _v.charSkls_);
        Write(_s, _v.userSkls_);
        Write(_s, _v.userInfo_);
        Write(_s, _v.marks_);
        Write(_s, _v.lobbyTheme_);
        Write(_s, _v.cardFormaFavi_);
        Write(_s, _v.facilityes_);
        Write(_s, _v.dispatches_);
        Write(_s, _v.roomPackage_);
        Write(_s, _v.costumes_);
        Write(_s, _v.stageClears_);
        Write(_s, _v.stageInfo_);
        Write(_s, _v.raid_);
        Write(_s, _v.timeRecord_);
        Write(_s, _v.storeSales_);
        Write(_s, _v.monFees_);
        Write(_s, _v.raidStore_);
        Write(_s, _v.monsterBooks_);
        Write(_s, _v.cardBooks_);
        Write(_s, _v.cards_);
        Write(_s, _v.items_);
        Write(_s, _v.gems_);
        Write(_s, _v.weaponBooks_);
        Write(_s, _v.weapons_);
        Write(_s, _v.wnpDepot_);
        Write(_s, _v.badges_);
        Write(_s, _v.mails_);
        Write(_s, _v.pass_);
        Write(_s, _v.mission_);
        Write(_s, _v.events_);
        Write(_s, _v.effs_);
        Write(_s, _v.achieves_);
        Write(_s, _v.arena_);
        Write(_s, _v.arenaTower_);
        Write(_s, _v.relocate_);
        Write(_s, _v.rgacha_);
        Write(_s, _v.unexpectedPackage_);
		Write(_s, _v.bingoEventList_);
        Write(_s, _v.achieveEventList_);
		Write(_s, _v.preset_);
		Write(_s, _v.chatStamp_);
		Write(_s, _v.goodsInfo_);
    }
}

// 메일 아이템 획득 패킷 메시지
public class PktInfoUserDataChange : PktMsgType
{
    public PktInfoProductPack products_;        // 획득한 제품 정보
    public PktInfoAchieve achieves_;            // 공적 정보
    public PktInfoMission mission_;             // 미션 정보
    public PktInfoAchieveEvent achieveEvent_;   // 공적 이벤트 
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoUserDataChange _v) {
        _v = new PktInfoUserDataChange();
        if (false == Read(_s, out _v.products_)) return false;
        if (false == Read(_s, out _v.achieves_)) return false;
        if (false == Read(_s, out _v.mission_)) return false;
        if (false == Read(_s, out _v.achieveEvent_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoUserDataChange _v) {
        Write(_s, _v.products_);
        Write(_s, _v.achieves_);
        Write(_s, _v.mission_);
        Write(_s, _v.achieveEvent_);
    }
}

// 메일 아이템 획득 요청 패킷 메시지
public class PktInfoMailProductTakeReq : PktMsgType
{
    public PktInfoUIDList mailIDs_;                 // 아이템 획득을 요청할 메일 고유 ID 목록
}
// 메일 아이템 획득 패킷 메시지
public class PktInfoMailProductTake : PktMsgType
{
    public PktInfoProductPack takeProduct_;         // 메일에 포함되어 획득한 제품들
    public PktInfoUIDList delList_;                 // 제품 획득으로 제거된 메일 UID 목록
    public PktInfoMail mails_;                      // 남아 있는 메일 정보(모두 보내지는 않고 일정 수량까지만 보냅니다.)
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoMailProductTakeReq _v) {
        _v = new PktInfoMailProductTakeReq();
        if (false == Read(_s, out _v.mailIDs_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoMailProductTakeReq _v) {
        Write(_s, _v.mailIDs_);
    }
    public static bool Read(Message _s, out PktInfoMailProductTake _v) {
        _v = new PktInfoMailProductTake();
        if (false == Read(_s, out _v.takeProduct_)) return false;
        if (false == Read(_s, out _v.delList_)) return false;
        if (false == Read(_s, out _v.mails_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoMailProductTake _v) {
        Write(_s, _v.takeProduct_);
        Write(_s, _v.delList_);
        Write(_s, _v.mails_);
    }
}

// 도감 상태 보상 패킷 메시지
public class PktInfoBookStateReward : PktMsgType
{
    public PktInfoProductPack products_;            // 획득한 도감 상태 보상 정보
    public PktInfoBookChangeState bookState_;       // 도감 상태
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoBookStateReward _v) {
        _v = new PktInfoBookStateReward();
        if (false == Read(_s, out _v.products_)) return false;
        if (false == Read(_s, out _v.bookState_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoBookStateReward _v) {
        Write(_s, _v.products_);
        Write(_s, _v.bookState_);
    }
}

// 시설 작동 응답 패킷 메시지
public class PktInfoFacilityOperationAck : PktMsgType
{
    public List<PktInfoFacilityOperationReq.Mater> maters_; // 제품 재료 목록(교환 등에서 사용)
    public PktInfoConsumeItem itemMaters_;          // 재료 아이템 정보
    public PktInfoProductPack products_;            // 획득한 제품 정보(교환 등을 통해서)
    public PktInfoTime endTime_;                    // 시설 작동 완료 예정 시간
    public PktInfoUIDList operationRoomChar_;                // 작전회의중인 캐릭터 정보
    public System.UInt64 operationValue_;           // 시설 작동 관련 값(캐릭터 UID, 아이템 TID 등) Game_Facility.txt 테이블의 EffectType의 값에 따라서 사용 용도가 다름(예 : EffectType의 값이 FAC_CHAR_SP일 경우 - 시설 작동으로 증가된 캐릭터 최종 SkillPoint를 나타냅니다.)
    public System.UInt32 facilityTID_;              // 시설 테이블 ID
    public System.UInt32 cardTID_;                  // 효과를 적용한 카드(서포터) TID (로그 및 뷰어용)
    public System.UInt16 operationCnt_ = 1;         // 조합할 아이템의 수
};
// 시설 작동 완료 응답 패킷 메시지
public class PktInfoFacilityOperConfirmAck : PktMsgType
{
    public PktInfoAddItem logAddItem_;      // 추가된 아이템 정보(로그 및 뷰어용) - [취소시 재료로 사용된 아이템 복구 정보]
    public PktInfoItem items_;              // 결과 아이템(조합 등을 통해서 생성된) - [취소시 재료로 사용된 아이템 복구 정보]
    public PktInfoCardBook cardBook_;       // 카드(서포터) 도감 관련 갱신 정보
    public PktInfoWeaponBook weaponBook_;   // 무기 도감 관련 갱신 정보
    public PktInfoTime confiremTime_;       // 시설 작동 완료 시간
    public PktInfoExpLv operEffExpLv_;      // 시설 작동 효과로 적용된 경험치 레벨//무기, 캐릭터 경험치 및 레벨
    public PktInfoConsumeItemAndGoods consumeAndGoods_; // 사용한 즉시 완료 아이템 및 재화 적용
    public PktInfoUnexpectedPackage unexpectedPackage_; // 추가된 돌발 패키지 정보 및 목록
    public PktInfoProductPack products_;            // 작전회의 결과로 획득할 제품 목록
    public PktInfoUIDList operationRoomChar_;       // 작전회의실 작동완료된 캐릭터 목록
    public System.UInt64 operationValue_;   // 시설 작동 관련 값(캐릭터 UID, 아이템 TID 등) eFacilityEffTP 타입에 따라서 사용 용도가 다름
    public System.UInt64 retValue1_;        // 시설 작동을 통해서 얻어진 값//스킬포인트
    public System.UInt32 facilityTID_;      // 시설 테이블 ID (로그 및 뷰어용)
    public System.UInt32 cardTID_;          // 효과를 적용한 카드(서포터) TIDsadsadsad
    public System.Byte operEffType_;        // 작동 효과 타입(eFacilityEffTP)
    public bool operEndFlag_;               // 시간이 지나서 작동을 올바르게 끝냈다면 true 아직 시간이 안지나서 취소로 처리 됐으면 false
    public bool clearOperValFlag_;          // operationValue값을 초기화 시키는지 여부
};

public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 시설 작동 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoFacilityOperationAck _v) {
        _v = new PktInfoFacilityOperationAck();
        if (false == ReadList(_s, out _v.maters_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.itemMaters_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.products_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.endTime_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.operationRoomChar_)) return false;
        if (false == _s.Read(out _v.operationValue_)) return false;
        if (false == _s.Read(out _v.facilityTID_)) return false;
        if (false == _s.Read(out _v.cardTID_)) return false;
        if (false == _s.Read(out _v.operationCnt_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoFacilityOperationAck _v) {
        WriteList(_s, _v.maters_);
        PN_MarshalerEx.Write(_s, _v.itemMaters_);
        PN_MarshalerEx.Write(_s, _v.products_);
        PN_MarshalerEx.Write(_s, _v.endTime_);
        PN_MarshalerEx.Write(_s, _v.operationRoomChar_);
        _s.Write(_v.operationValue_);
        _s.Write(_v.facilityTID_);
        _s.Write(_v.cardTID_);
        _s.Write(_v.operationCnt_);
    }

    // 시설 작동 완료 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoFacilityOperConfirmAck _v)
    {
        _v = new PktInfoFacilityOperConfirmAck();
        if (false == PN_MarshalerEx.Read(_s, out _v.logAddItem_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.items_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.cardBook_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.weaponBook_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.confiremTime_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.operEffExpLv_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.consumeAndGoods_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.unexpectedPackage_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.operationRoomChar_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.products_)) return false;
        if (false == _s.Read(out _v.operationValue_)) return false;
        if (false == _s.Read(out _v.retValue1_)) return false;
        if (false == _s.Read(out _v.facilityTID_)) return false;
        if (false == _s.Read(out _v.cardTID_)) return false;
        if (false == _s.Read(out _v.operEffType_)) return false;
        if (false == _s.Read(out _v.operEndFlag_)) return false;
        if (false == _s.Read(out _v.clearOperValFlag_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoFacilityOperConfirmAck _v)
    {
        PN_MarshalerEx.Write(_s, _v.logAddItem_);
        PN_MarshalerEx.Write(_s, _v.items_);
        PN_MarshalerEx.Write(_s, _v.cardBook_);
        PN_MarshalerEx.Write(_s, _v.weaponBook_);
        PN_MarshalerEx.Write(_s, _v.confiremTime_);
        PN_MarshalerEx.Write(_s, _v.operEffExpLv_);
        PN_MarshalerEx.Write(_s, _v.consumeAndGoods_);
        PN_MarshalerEx.Write(_s, _v.unexpectedPackage_);
        PN_MarshalerEx.Write(_s, _v.operationRoomChar_);
        PN_MarshalerEx.Write(_s, _v.products_);
        _s.Write(_v.operationValue_);
        _s.Write(_v.retValue1_);
        _s.Write(_v.facilityTID_);
        _s.Write(_v.operEffType_);
        _s.Write(_v.operEndFlag_);
        _s.Write(_v.clearOperValFlag_);
    }
}


// 이벤트 보상 리셋형 패킷 메시지
public class PktInfoEventRewardTake : PktMsgType
{
    public PktInfoConsumeItem maters_;      // 재료 아이템 정보
    public PktInfoProductPack products_;    // 획득한 제품 정보
    public PktInfoEventReward reward_;      // 변경된 이벤트 보상 정보
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoEventRewardTake _v) {
        _v = new PktInfoEventRewardTake();
        if (false == Read(_s, out _v.maters_)) return false;
        if (false == Read(_s, out _v.products_)) return false;
        if (false == Read(_s, out _v.reward_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoEventRewardTake _v) {
        Write(_s, _v.maters_);
        Write(_s, _v.products_);
        Write(_s, _v.reward_);
    }
}

// 아이템 사용(제품 획득) 응답 패킷 메시지
public class PktInfoUseItemProductAck : PktMsgType
{
    public PktInfoConsumeItem useItem_;     // 사용한 아이템 정보
    public PktInfoProductPack products_;    // 아이템 사용한 결과 값(획득한 제품 정보)
}
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoUseItemProductAck _v) {
        _v = new PktInfoUseItemProductAck();
        if (false == Read(_s, out _v.useItem_)) return false;
        if (false == Read(_s, out _v.products_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoUseItemProductAck _v) {
        Write(_s, _v.useItem_);
        Write(_s, _v.products_);
    }
}

// 파견 임무 완료 응답 패킷 메시지
public class PktInfoDispatchOperConfirmAck : PktMsgType
{
    public PktInfoProductPack products_;    // 파견 임무 완료 보상
	public PktInfoConsumeItem consumeItem_;	// 소모된 아이템 정보(즉시완료 요청시)
	public PktInfoUIDList outCards_;        // 위치 적용 해제된 카드(서포터) 목록
    public PktInfoDispatch opers_;          // 파견 정보
    public System.Boolean operEndFlag_;     // 시간이 지나서 임무를 올바르게 끝냈다면 true 아직 시간이 안지나서 취소로 처리 됐으면 false
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 파견 임무 완료 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoDispatchOperConfirmAck _v) {
        _v = new PktInfoDispatchOperConfirmAck();
        if (false == Read(_s, out _v.products_)) return false;
		if (false == Read(_s, out _v.consumeItem_)) return false;
		if (false == Read(_s, out _v.outCards_)) return false;
        if (false == Read(_s, out _v.opers_)) return false;
        if (false == _s.Read(out _v.operEndFlag_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoDispatchOperConfirmAck _v) {
        Write(_s, _v.products_);
		Write(_s, _v.consumeItem_);
		Write(_s, _v.outCards_);
        Write(_s, _v.opers_);
        _s.Write(_v.operEndFlag_);
    }
}

// 아레나 타워 시작 응답 패킷 메시지
public class PktInfoArenaTowerGameStartAck : PktMsgType
{
    public List<PktInfoArenaDetail> commuInfo_; // 사용하려는 커뮤니티 정보
    public PktInfoConsumeItem useItem_;     // 아레나 타워 시작시 사용한 소모 아이템 정보
    public System.UInt64 useCommuUuid_;     // 사용한 커뮤니티 유저 고유 ID
    public System.UInt32 towerID_;          // 진행할 아레나 타워 ID 
};
// 아레나 타워 종료 응답 패킷 메시지
public class PktInfoArenaTowerGameEndAck : PktMsgType
{
    public PktInfoProductPack products_;    // 결과 보상 정보
    public PktInfoUserArenaTower info_;     // 아레나 타워 정보
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 아레나 타워 시작 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoArenaTowerGameStartAck _v) {
        _v = new PktInfoArenaTowerGameStartAck();
        if (false == ReadList(_s, out _v.commuInfo_)) return false;
        if (false == Read(_s, out _v.useItem_)) return false;
        if (false == _s.Read(out _v.useCommuUuid_)) return false;
        if (false == _s.Read(out _v.towerID_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoArenaTowerGameStartAck _v) {
        WriteList(_s, _v.commuInfo_);
        Write(_s, _v.useItem_);
        _s.Write(_v.useCommuUuid_);
        _s.Write(_v.towerID_);
    }
    // 아레나 타워 종료 응답 패킷 메시지
    public static bool Read(Message _s, out PktInfoArenaTowerGameEndAck _v) {
        _v = new PktInfoArenaTowerGameEndAck();
        if (false == Read(_s, out _v.products_)) return false;
        if (false == Read(_s, out _v.info_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoArenaTowerGameEndAck _v) {
        Write(_s, _v.products_);
        Write(_s, _v.info_);
    }
}

// 캐릭터 선물 주기 응답 패킷 메시지
public class PktInfoGivePresentCharAck : PktMsgType
{
    public PktInfoProductPack products_;    // 결과 보상 정보
    public PktInfoConsumeItem maters_;      // 소모 아이템 정보
    public PktInfoExpLv expLv_;             // 변경된 경험치 레벨
    public System.UInt64 cuid_;             // 캐릭터 고유 ID
    public System.Byte preCnt_;                // 남은 선물 하기 횟수
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoGivePresentCharAck _v) {
        _v = new PktInfoGivePresentCharAck();
        if (false == Read(_s, out _v.products_)) return false;
        if (false == Read(_s, out _v.maters_)) return false;
        if (false == Read(_s, out _v.expLv_)) return false;
        if (false == _s.Read(out _v.cuid_)) return false;
        return _s.Read(out _v.preCnt_);
    }
    public static void Write(Message _s, PktInfoGivePresentCharAck _v) {
        Write(_s, _v.products_);
        Write(_s, _v.maters_);
        Write(_s, _v.expLv_);
        _s.Write(_v.cuid_);
        _s.Write(_v.preCnt_);
    }
}

// 일일 미션 보상 획득 응답 패킷 메시지
public class PktInfoRwdDailyMissionAck : PktMsgType
{
    public PktInfoProductPack products_;    // 결과 보상 정보
    public System.UInt32 retRwdFlag_;       // 보상 결과가 적용된 보상 플레그 값
    public System.UInt32 groupID_;          // 요청할 일일 미션 그룹 ID
    public System.Byte day_;                // 요청할 일일 미션 날 수
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    public static bool Read(Message _s, out PktInfoRwdDailyMissionAck _v) {
        _v = new PktInfoRwdDailyMissionAck();
        if (false == Read(_s, out _v.products_)) return false;
        if (false == _s.Read(out _v.retRwdFlag_)) return false;
        if (false == _s.Read(out _v.groupID_)) return false;
        return _s.Read(out _v.day_);
    }
    public static void Write(Message _s, PktInfoRwdDailyMissionAck _v) {
        Write(_s, _v.products_);
        _s.Write(_v.retRwdFlag_);
        _s.Write(_v.groupID_);
        _s.Write(_v.day_);
    }
}

//////////////////////////////////////////////////////////////////////////
///
// 서버 달성(세력) 랭킹 정보
public class PktInfoRankInfluence : PktMsgType
{
    public class Piece : PktMsgType
    {
        public PktInfoStr name_;                // 유저 닉네임
		public uint nickNameColorID_;			// 유저 닉네임 색상 아이디
		public System.UInt64 uuid_;             // 유저 고유 ID
        public System.UInt32 mark_;             // 유저 마크 ID
        public System.UInt32 influPT_;          // 서버 달성(세력) 획득 점수
        public System.Byte rank_;               // 랭킹
        public System.Byte lv_;                 // 유저 레벨

        public override bool Read(Message _s) {
            if (false == PN_MarshalerEx.Read(_s, out name_)) return false;
			if (false == _s.Read(out nickNameColorID_)) return false;
			if (false == _s.Read(out uuid_)) return false;
            if (false == _s.Read(out mark_)) return false;
            if (false == _s.Read(out influPT_)) return false;
            if (false == _s.Read(out rank_)) return false;
            return _s.Read(out lv_);
        }
        public override void Write(Message _s) {
            PN_MarshalerEx.Write(_s, name_);
			_s.Write(nickNameColorID_);
			_s.Write(uuid_);
            _s.Write(mark_);
            _s.Write(influPT_);
            _s.Write(rank_);
            _s.Write(lv_);
        }
    };
    public List<Piece> infos_;
    public PktInfoTime lastUpTM_;           // 마지막으로 갱신된 시간
    public System.UInt32 idInRankTbl_;      // 자신이 속한 랭크 테이블 안의 ID
    public System.Byte nowRank_;            // 현재 자신의 랭크
};
// 서버 달성(세력) 보상 획득 응답 패킷 메시지
public class PktInfoRwdInfluenceTgtAck : PktMsgType
{
    public PktInfoProductPack products_;        // 보상 결과 제품 목록
    public System.UInt32 retTgtRwdFlag_;        // 보상 결과가 적용된 보상 플레그 값
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
    // 서버 달성(세력) 랭킹 정보
    public static bool Read(Message _s, out PktInfoRankInfluence.Piece _v) {
        _v = new PktInfoRankInfluence.Piece();
        return _v.Read(_s);
    }
    public static void Write(Message _s, PktInfoRankInfluence.Piece _v) {
        _v.Write(_s);
    }
    public static bool Read(Message _s, out PktInfoRankInfluence _v) {
        _v = new PktInfoRankInfluence();
        if (false == ReadList(_s, out _v.infos_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.lastUpTM_)) return false;
        if (false == _s.Read(out _v.idInRankTbl_)) return false;
        return _s.Read(out _v.nowRank_);
    }
    public static void Write(Message _s, PktInfoRankInfluence _v) {
        WriteList(_s, _v.infos_);
        PN_MarshalerEx.Write(_s, _v.lastUpTM_);
        _s.Write(_v.idInRankTbl_);
        _s.Write(_v.nowRank_);
    }
    // 서버 달성(세력) 랭킹 요청 패킷 메시지
    public static bool Read(Message _s, out PktInfoRwdInfluenceTgtAck _v) {
        _v = new PktInfoRwdInfluenceTgtAck();
        if (false == PN_MarshalerEx.Read(_s, out _v.products_)) return false;
        return _s.Read(out _v.retTgtRwdFlag_);
    }
    public static void Write(Message _s, PktInfoRwdInfluenceTgtAck _v) {
        PN_MarshalerEx.Write(_s, _v.products_);
        _s.Write(_v.retTgtRwdFlag_);
    }
}

public class PktInfoBingoEventRewardReq : PktMsgType
{
	public System.UInt32 groupID;            //	빙고 이벤트 그룹 아이디
	public System.Byte no_;             //	빙고이벤트 그룹 내 회차 정보
	public PktInfoUInt8List rwdLine_;    //	보상 받으려는 줄 수 (완성 된 줄의 개수)
}
public class PktInfoBingoEventRewardAck : PktMsgType
{
	public PktInfoProductPack products_;            // 결과 보상 정보
	public PktInfoUserBingoEvent.Piece bingoInfo_; // 변경된 유저의 빙고 이벤트 진행 정보
};

// 유저 공적 보상 패킷 메시지
public class PktInfoAchieveReward : PktMsgType
{
    public PktInfoProductPack products_;        // 보상으로 획득한 내용이 적용된 유저의 모든 재화
    public PktInfoAchieve achieve_;             // 보상 획득으로 변경된 공적 정보
};

// 공적 이벤트 보상 패킷 메시지
public class PktInfoAchieveEventReward : PktMsgType
{
    public PktInfoProductPack products_;                // 보상으로 획득한 내용이 적용된 유저의 모든 재화
    public PktInfoAchieveEvent achieveEvent_;           // 보상 획득으로 변경된 공적 이벤트 정보
};
public partial class PN_MarshalerEx : Nettention.Proud.Marshaler
{
	public static bool Read(Message _s, out PktInfoBingoEventRewardReq _v)
	{
		_v = new PktInfoBingoEventRewardReq();
		if (false == _s.Read(out _v.groupID)) return false;
		if (false == _s.Read(out _v.no_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.rwdLine_)) return false;
		return true;
	}
	public static void Write(Message _s, PktInfoBingoEventRewardReq _v)
	{
		_s.Write(_v.groupID);
		_s.Write(_v.no_);
		PN_MarshalerEx.Write(_s, _v.rwdLine_);
	}
	public static bool Read(Message _s, out PktInfoBingoEventRewardAck _v)
	{
		_v = new PktInfoBingoEventRewardAck();
		if (false == PN_MarshalerEx.Read(_s, out _v.products_)) return false;
		if (false == PN_MarshalerEx.Read(_s, out _v.bingoInfo_)) return false;
		return true;
	}
	public static void Write(Message _s, PktInfoBingoEventRewardAck _v)
	{
		PN_MarshalerEx.Write(_s, _v.products_);
		PN_MarshalerEx.Write(_s, _v.bingoInfo_);
	}

    // 유저 공적 보상 패킷 메시지
    public static bool Read(Message _s, out PktInfoAchieveReward _v)
    {
        _v = new PktInfoAchieveReward();
        if (false == Read(_s, out _v.products_)) return false;
        if (false == Read(_s, out _v.achieve_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoAchieveReward _v)
    {
        Write(_s, _v.products_);
        Write(_s, _v.achieve_);
    }

    // 공적 이벤트 보상 패킷 메시지
    public static bool Read(Message _s, out PktInfoAchieveEventReward _v)
    {
        _v = new PktInfoAchieveEventReward();
        if (false == PN_MarshalerEx.Read(_s, out _v.products_)) return false;
        if (false == PN_MarshalerEx.Read(_s, out _v.achieveEvent_)) return false;
        return true;
    }
    public static void Write(Message _s, PktInfoAchieveEventReward _v)
    {
        PN_MarshalerEx.Write(_s, _v.products_);
        PN_MarshalerEx.Write(_s, _v.achieveEvent_);
    }
}


///
//////////////////////////////////////////////////////////////////////////