using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial class GachaCategoryData
{
    public void SetPktData(PktInfoTblSvrGuerrillaStore.Piece gachaInfo_)
    {
        StartDate = GameSupport.GetLocalTimeByServerTime(gachaInfo_.StartDate.GetTime());
        EndDate = GameSupport.GetLocalTimeByServerTime(gachaInfo_.EndDate.GetTime());
        StoreID1 = (int)gachaInfo_.StoreID1;
        StoreID2 = (int)gachaInfo_.StoreID2;
        Type = gachaInfo_.Type.GetStr();
        Text = (int)gachaInfo_.Text;
        UrlBtnImage = gachaInfo_.UrlBtnImage.GetStr();
        UrlBGImage = gachaInfo_.UrlBGImage.GetStr();
        UrlAddImage = gachaInfo_.UrlAddImage.GetStr();

        StoreID3 = (int)gachaInfo_.StoreID3;
        StoreID4 = (int)gachaInfo_.StoreID4;

        Name = (int)gachaInfo_.Name;
        Desc = (int)gachaInfo_.Desc;
        string[] splits = gachaInfo_.Local.GetStr().Split(',');
        for (int i = 0; i < splits.Length; i++)
        {
            int.TryParse(splits[i], out int result);
            Localize[i] = result == 1;
        }
        Value1 = gachaInfo_.Val1.GetStr();
    }
}

public partial class StoreSaleData
{
    public void SetPktData(PktInfoTblSvrStoreSale.Piece storesaleInfo_)
    {
        TableID = (int)storesaleInfo_.TableID;
        LimitValue = (int)storesaleInfo_.LimitValue;
        DiscountRate = (int)storesaleInfo_.DiscountRate;
        LimitType = (int)storesaleInfo_.LimitType;
    }
}

public partial class BannerData
{
    public void SetPktData(PktInfoTblSvrBanner.Piece bannerInfo_)
    {
        StartDate = GameSupport.GetLocalTimeByServerTime(bannerInfo_.StartDate.GetTime());
        EndDate = GameSupport.GetLocalTimeByServerTime(bannerInfo_.EndDate.GetTime());
        UrlImage = bannerInfo_.URL.GetStr();

        UrlAddImage1 = bannerInfo_.URL_Add1?.GetStr();
        UrlAddImage2 = bannerInfo_.URL_Add2?.GetStr();
        Name = (int)bannerInfo_.Name;
        Desc = (int)bannerInfo_.Desc;
        
        string getStr = bannerInfo_.Local?.GetStr();
        if (!string.IsNullOrEmpty(getStr))
        {
            string[] splits = getStr.Split(',');
            for (int i = 0; i < splits.Length; i++)
            {
                if (Localizes.Length <= i)
                {
                    continue;
                }
                
                Localizes[i] = splits[i].Equals("1");
            }
        }

        FunctionType = bannerInfo_.FuncTP.GetStr();
        FunctionValue1 = bannerInfo_.FuncVal1.GetStr();
        FunctionValue2 = bannerInfo_.FuncVal2.GetStr();
        FunctionValue3 = bannerInfo_.FuncVal3.GetStr();
        TagMark = eBannerFuntionValue3Flag.None;
        getStr = bannerInfo_.TagMark.GetStr();
        if (!string.IsNullOrEmpty(getStr))
        {
            string[] splits = getStr.Split(',');
            foreach (string split in splits)
            {
                if (int.TryParse(split, out int result))
                {
                    TagMark |= (eBannerFuntionValue3Flag)(1 << result);
                }
            }
        }
        
        BannerType = (int)bannerInfo_.Type;
        BannerTypeValue = (int)bannerInfo_.TypeValue;
    }
}

public partial class GuerrillaCampData
{
    public void SetPktData(PktInfoTblSvrGuerrillaCamp.Piece guerrillacampInfo_)
    {
        StartDate = GameSupport.GetLocalTimeByServerTime(guerrillacampInfo_.StartDate.GetTime());
        EndDate = GameSupport.GetLocalTimeByServerTime(guerrillacampInfo_.EndDate.GetTime());
        Condition = (int)guerrillacampInfo_.Condition;
        EffectValue = (int)guerrillacampInfo_.EffectValue;
        Type = guerrillacampInfo_.Type.GetStr();
        Name = (int)guerrillacampInfo_.Title;
        Desc = (int)guerrillacampInfo_.Desc;
    }
}
//9998
public partial class GuerrillaMissionData
{
    public void SetPktData(PktInfoTblSvrGuerrillaMission.Piece guerrillacampInfo_)
    {
        StartDate = GameSupport.GetLocalTimeByServerTime(guerrillacampInfo_.StartDate.GetTime());
        EndDate = GameSupport.GetLocalTimeByServerTime(guerrillacampInfo_.EndDate.GetTime());
        
        Type = guerrillacampInfo_.Type.GetStr();
        Name = (int)guerrillacampInfo_.Name;
        Desc = (int)guerrillacampInfo_.Desc;
        Condition = (int)guerrillacampInfo_.Condition;
        Count = (int)guerrillacampInfo_.Count;
        GroupID = (int)guerrillacampInfo_.GroupID;
        GroupOrder = (int)guerrillacampInfo_.GroupOrder;
        RewardType = (int)guerrillacampInfo_.RewardType;
        RewardIndex = (int)guerrillacampInfo_.RewardIndex;
        RewardValue = (int)guerrillacampInfo_.RewardValue;
    }
}


public partial class ServerData
{
    public void SetPktData(PktInfoUserReflash _pktInfo)
    {
        GuerrillaCampList.Clear();
        GachaCategoryList.Clear();
        StoreSaleList.Clear();
        BannerList.Clear();
        GuerrillaMissionList.Clear();
        //게릴라 캠페인??

        DayRemainTime = GameSupport.GetCurrentServerTime().AddDays(1);
        DateTime tomorrow = GameInfo.Instance.UserData.LoginBonusRecentDate.AddDays(1);
        DayRemainTime = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day);

        //DayRemainTime = GameInfo.Instance.UserData.LoginBonusRecentDate.AddDays(1);

        if (null != _pktInfo.campaign_.infos_)
        {
            foreach (PktInfoTblSvrGuerrillaCamp.Piece svrGuerrillaCamp in _pktInfo.campaign_.infos_)
            {
                GuerrillaCampData guerrillacampdata = new GuerrillaCampData();
                guerrillacampdata.SetPktData(svrGuerrillaCamp);
                GuerrillaCampList.Add(guerrillacampdata);
            }
        }
        //게일라 상점(픽업 가챠 포함)
        if (null != _pktInfo.store_.infos_)
        {
            foreach (PktInfoTblSvrGuerrillaStore.Piece svrStore in _pktInfo.store_.infos_)
            {
                GachaCategoryData gachaCategoryData = new GachaCategoryData();
                gachaCategoryData.SetPktData(svrStore);
                GachaCategoryList.Add(gachaCategoryData);
            }
        }

        //상점 세일 
        if (null != _pktInfo.sale_.infos_)
        {
            foreach (PktInfoTblSvrStoreSale.Piece svrStoreSale in _pktInfo.sale_.infos_)
            {
                StoreSaleData storesaledata = new StoreSaleData();
                storesaledata.SetPktData(svrStoreSale);
                StoreSaleList.Add(storesaledata);
            }
        }

        //배너 정보
        if (null != _pktInfo.banner_.infos_)
        {
            foreach (PktInfoTblSvrBanner.Piece svrBanner in _pktInfo.banner_.infos_)
            {
                BannerData bannerData = new BannerData();
                bannerData.SetPktData(svrBanner);
                BannerList.Add(bannerData);
            }
        }

        //게릴라 미션
        if(null != _pktInfo.mission_.infos_)
        {
            foreach(PktInfoTblSvrGuerrillaMission.Piece svrGuerrillaMission in _pktInfo.mission_.infos_)
            {
                GuerrillaMissionData guerrillaMissionData = new GuerrillaMissionData();
                guerrillaMissionData.SetPktData(svrGuerrillaMission);
                GuerrillaMissionList.Add(guerrillaMissionData);
            }
        }
    }

    public void SetPktSecretQuestData(PktInfoSecretQuestOpt infos)
    {
        SecretQuestOptionList.Clear();
        foreach (var info in infos.infos_)
        {
            SecretQuestOptionList.Add(
                new SecretQuestOptionData
                {
                    GroupId = (int)info.groupID_,
                    LevelId = info.lvNo_,
                    BoSetId = info.boSet_,
                });
        }
    }
}

public partial class UserData
{
    public void SetPktData(PktInfoUser userInfo_)
    {
        UUID = (long)userInfo_.uuid_;
        mNickName = userInfo_.nickName_;
        NickNameColorId = (int)userInfo_.nickNameColorID_;
        UserWord = userInfo_.comment_;
        HardCash = (long)userInfo_.hardCash_;
        Level = (int)userInfo_.expLv_.lv_;
        Exp = (int)userInfo_.expLv_.exp_;
        RoomThemeSlot = (int)userInfo_.roomSlotNum_;
        APRemainTime = GameSupport.GetLocalTimeByServerTime(userInfo_.ticketRemainTM_.GetTime());
        BPRemainTime = GameSupport.GetLocalTimeByServerTime(userInfo_.BPNextTM_.GetTime());
        UserMarkID = (int)userInfo_.markID_;
        UserLobbyThemeID = (int)userInfo_.lobbyThemeID_;
        ItemSlotCnt = (int)userInfo_.itemSlotCnt_;
        TutorialNum = (int)userInfo_.tutoValue_;
        TutorialFlag = (long)userInfo_.tutoFlag_;
        LoginBonusGroupID = (int)userInfo_.loginGroupID_;
        LoginBonusGroupCnt = userInfo_.loginGroupCnt_;
        //LoginBonusRecentDate = GameSupport.GetLocalTimeByServerTime(userInfo_.lastLoginBonusTime_.GetTime());
        PrevLoginTime = userInfo_.lastLoginBonusTM_.GetTime();
        LoginBonusRecentDate = userInfo_.lastLoginBonusTM_.GetTime();     //주간미션에서 사용 하기 때문에 로컬 타임변환 안하고 그냥 받음.
        LoginTotalCount = (int)userInfo_.totalLoginCnt_;
        LoginContinuityCount = (int)userInfo_.continueLoginCnt_;
        NextFrientPointGiveTime = GameSupport.GetLocalTimeByServerTime(userInfo_.nextFriPTGiveTM_.GetTime());

        CardFormationID = (int)userInfo_.cardFrmtID_;

        AccountCodeReward = userInfo_.accountCodeReward_;
        AccountLinkReward = userInfo_.accountLinkReward_;

        ShowPkgPopup = userInfo_.pkgShow_ == 0 ? false : true;

        ArrLobbyBgCharUid = new long[GameInfo.Instance.GameConfig.MaxLobbyBgCharCount];
        ArrFavorBuffCharUid = new long[GameInfo.Instance.GameConfig.MaxLobbyBgCharCount];

        DyeingCostumeId = userInfo_.dyeingCostumeID_;

        BlacklistLevel = userInfo_.blackLisInfo_.level_;
        BlacklistEndTime = GameSupport.GetLocalTimeByServerTime( userInfo_.blackLisInfo_.blockEndTime_.GetTime() );

        CircleAuthLevel.AuthLevel = userInfo_.circleAuthInfo_.authLv_;
        CirclePossibleJoinTime = userInfo_.circleJoinPossibleDateNum_.GetTime();
        CircleAttendance.LastCheckDate = userInfo_.lastCircleAttendTM_.GetTime();
        CircleAttendance.RewardGroupId = (int)userInfo_.circleAttendGroupID_;
        CircleAttendance.RewardCount = userInfo_.circleAttendGroupCnt_;

		LoginBonusMonthlyCount = userInfo_.nowLgnBonusMonthlyCnt_;
	}

    public void SetPktData(PktInfoGoodsAll goodsInfo_)
    {
        for (int i = 0; i < (int)eGOODSTYPE.COUNT; i++)
            Goods[i] = (long)goodsInfo_.goodsValues_[i];
    }

    public void SetPktData(PktInfoGoods goodsInfo_)
    {
        for (int i = 0; i < goodsInfo_.infos_.Count; i++)
        {
            int idx = (int)goodsInfo_.infos_[i].type_;
            if (Goods.Length <= idx) continue;

            Goods[idx] = (long)goodsInfo_.infos_[i].value_;
        }
        if (true == goodsInfo_.hardCashFlag_)
            HardCash = (long)goodsInfo_.nowHardCash_;
    }

    public void SetPktData(PktInfoUserSklLvUpAck pktInfo)
    {
        AwakenSkillInfo find = ListAwakenSkillData.Find(x => x.TableId == pktInfo.tid_);
        if(find == null)
        {
            AwakenSkillInfo info = new AwakenSkillInfo();
            info.SetPktData((int)pktInfo.tid_, pktInfo.resultLv_);

            ListAwakenSkillData.Add(info);
        }
        else
        {
            find.SetLevel(pktInfo.resultLv_);
        }
    }

	public void SetPktData( List<PktInfoChar> listPktInfo ) {
		for ( int i = 0; i < ArrLobbyBgCharUid.Length; i++ ) {
			ArrLobbyBgCharUid[i] = 0;
		}

		for ( int i = 0; i < ArrFavorBuffCharUid.Length; i++ ) {
			ArrFavorBuffCharUid[i] = 0;
		}

		for ( int i = 0; i < listPktInfo.Count; i++ ) {
			if ( listPktInfo[i].selNum_ > 0 ) {
				ArrLobbyBgCharUid[listPktInfo[i].selNum_ - 1] = ( (long)listPktInfo[i].cuid_ );

				if ( listPktInfo[i].selNum_ == 1 ) {
					MainCharUID = (long)listPktInfo[i].cuid_;
				}
			}

			if ( listPktInfo[i].preferenceNum_ > 0 ) {
				ArrFavorBuffCharUid[listPktInfo[i].preferenceNum_ - 1] = (long)listPktInfo[i].cuid_;
			}
		}
	}

	public void SetPktData(PktInfoTIDList pktInfo)
    {
        for(int i = 0; i < pktInfo.tids_.Count; i++)
        {
            int tid = (int)pktInfo.tids_[i];

            AwakenSkillInfo find = ListAwakenSkillData.Find(x => x.TableId == tid);
            if (find == null)
            {
                continue;
            }

            ListAwakenSkillData.Remove(find);
        }
    }

    public bool SetPktData(PktInfoUIDValue pktInfo)
    {
        for(int i = 0; i < ArrLobbyBgCharUid.Length; i++)
        {
            ArrLobbyBgCharUid[i] = 0;
        }

        long uid = 0;
        for (int i = 0; i < pktInfo.infos_.Count; i++)
        {
            uid = (long)pktInfo.infos_[i].uid_;

            CharData charData = GameInfo.Instance.GetCharData(uid);
            if (charData == null)
            {
                return false;
            }

            if (pktInfo.infos_[i].val_ > 0)
            {
                ArrLobbyBgCharUid[pktInfo.infos_[i].val_ - 1] = uid;

                if (pktInfo.infos_[i].val_ == 1)
                {
                    MainCharUID = uid;
                }
            }
        }

        return true;
    }

    public bool SetPktData(PktInfoUserBingoEvent pktInfo)
    {
        Dictionary<int, List<EventData>> eventDataDict = GameInfo.Instance.EventDataDict;

        int eventType = (int)eLobbyEventType.Bingo;
        if (eventDataDict.ContainsKey(eventType))
        {
            eventDataDict[eventType].Clear();
        }
        else
        {
            eventDataDict.Add(eventType, new List<EventData>());
        }

        foreach (PktInfoUserBingoEvent.Piece info in pktInfo.infos_)
        {
            eventDataDict[eventType].Add(new EventData(info));
        }

        return true;
    }

    public void SetPktData(PktInfoChatStamp pktInfo)
    {
        if (pktInfo == null)
        {
            return;
        }

        for (int i = 0; i < pktInfo.flag_.Length; i++)
        {
            if (CircleChatStampBit.Length <= i)
            {
                break;
            }

            CircleChatStampBit[i] = (long)pktInfo.flag_[i];
        }
    }

    public bool SetPktEvtLoginData(PktInfoEvtLogin loginEventInfo)
	{
		ListLoginEventInfo.Clear();

        foreach (PktInfoEvtLogin.Piece info in loginEventInfo.infos_)
        {
            ListLoginEventInfo.Add(new sLoginEventInfo(
                (int)info.tid_, info.rwds_, info.day_, info.endDay_));
        }
        
        return true;
	}

    public void UpdatePktEvtLoginData(PktInfoEvtLogin pLoginEventInfo)
    {
        foreach (PktInfoEvtLogin.Piece info in pLoginEventInfo.infos_)
        {
            int nIndex = ListLoginEventInfo.FindIndex(x => x.TableId == info.tid_);
            if (nIndex < 0)
            {
                return;
            }
            
            ListLoginEventInfo.RemoveAt(nIndex);
            ListLoginEventInfo.Insert(nIndex, new sLoginEventInfo(
                (int)info.tid_, info.rwds_, info.day_, info.endDay_));
        }
    }

    public bool HasLoginEventInfo()
    {
        return ListLoginEventInfo.Count > 0;
    }
}

public partial class StoreData
{
    public void SetPktData(PktInfoStoreSale.Piece storesaleInfo_)
    {
        TableID = (int)storesaleInfo_.tableID_;
        TypeVal = (long)storesaleInfo_.typeVal_;
        ResetTime = (long)storesaleInfo_.resetTM_;
    }
}

public partial class AchieveData
{
    public void SetPktData(PktInfoAchieve.Piece achieveInfo_)
    {
        GroupID = (int)achieveInfo_.groupID_;
        GroupOrder = (int)achieveInfo_.nowStep_;
        Value = (int)achieveInfo_.condiVal_;
        SetTable();
    }
}

public partial class CharData
{
    public void SetPktData(PktInfoChar charInfo_)
    {
        //!@# 멀티 무기 개선 작업 확인 필요 #@!
        CUID = (long)charInfo_.cuid_;
        TableID = (int)charInfo_.tableID_;
        Grade = (int)charInfo_.grade_;
        Level = (int)charInfo_.lv_;
        Exp = (int)charInfo_.exp_;
        PassviePoint = (int)charInfo_.passivePT_;
        EquipCostumeID = (int)charInfo_.costumeID_;
        EquipWeaponUID = (long)charInfo_.wpns_[(int)eWeaponSlot.MAIN].wpnUID_;

        //0211 추가
        EquipWeaponSkinTID = (int)charInfo_.wpns_[(int)eWeaponSlot.MAIN].wpnSkin_;
        EquipWeapon2UID = (long)charInfo_.wpns_[(int)eWeaponSlot.SUB].wpnUID_;
        EquipWeapon2SkinTID = (int)charInfo_.wpns_[(int)eWeaponSlot.SUB].wpnSkin_;

        CostumeStateFlag = (int)charInfo_.skinStateFlag_;
        Log.Show("CostumeStateFlag : " + CostumeStateFlag, Log.ColorType.Red);
        CostumeColor = (int)charInfo_.costumeClr_;

        for (int i = 0; i < (int)eCOUNT.SKILLSLOT; i++)
            EquipSkill[i] = (int)charInfo_.skillID_[i];

        TableData = GameInfo.Instance.GameTable.FindCharacter(x => x.ID == TableID);
        
        CharAniFlag = (int)charInfo_.aniFlag_;
        FavorExp = (int)charInfo_.friExp_;
        FavorLevel = (int)charInfo_.friLv_;
        FavorPreCnt = (int)charInfo_.preCnt_;

        OperationRoomTID = (int) charInfo_.operationRoomTID_;
        SecretQuestCount = charInfo_.secretCnt_;
        
		GameTable.Costume.Param find = GameInfo.Instance.GameTable.FindCostume(x => x.ID == EquipCostumeID);
		if (find != null && find.SubHairChange == 1)
		{
			bool isOn = GameSupport._IsOnBitIdx((uint)CostumeStateFlag, (int)(eCostumeStateFlag.CSF_HAIR));

			uint flag = (uint)CostumeStateFlag;
			GameSupport._DoOnOffBitIdx(ref flag, (int)(eCostumeStateFlag.CSF_HAIR), !isOn);

			CostumeStateFlag = (int)flag;
		}

        RaidHpPercentage = (float)charInfo_.raidHP_ / 100.0f; // 소수점 둘째 자리까지

        IsHideWeapon = PlayerPrefs.GetInt($"{CUID}_IS_HIDE_WEAPON", 0) == 0 ? false : true;
	}

    public void SetPktSkinColorData(PktInfoCharSkinColor skinColorInfo_)
    {
        CostumeStateFlag = (int)skinColorInfo_.skinStateFlag_;

		GameTable.Costume.Param find = GameInfo.Instance.GameTable.FindCostume(x => x.ID == EquipCostumeID);
		if (find != null && find.SubHairChange == 1)
		{
			bool isOn = GameSupport._IsOnBitIdx((uint)CostumeStateFlag, (int)(eCostumeStateFlag.CSF_HAIR));

			uint flag = (uint)CostumeStateFlag;
			GameSupport._DoOnOffBitIdx(ref flag, (int)(eCostumeStateFlag.CSF_HAIR), !isOn);

			CostumeStateFlag = (int)flag;
		}

		Log.Show("CostumeStateFlag : " + CostumeStateFlag, Log.ColorType.Yellow);
        CostumeColor = (int)skinColorInfo_.costumeClr_;
    }
}

public partial class WeaponData
{
    public void SetPktData(PktInfoWeapon.Piece weaponInfopiece_)
    {
        WeaponUID = (long)weaponInfopiece_.weaponUID_;
        TableID = (int)weaponInfopiece_.tableID_;
        Level = (int)weaponInfopiece_.lv_;
        Exp = (int)weaponInfopiece_.exp_;
        Wake = (int)weaponInfopiece_.wake_;
        SkillLv = (int)weaponInfopiece_.skillLv_;
        Lock = (bool)weaponInfopiece_.lock_;
        EnchantLv = weaponInfopiece_.encLv_;
        
        for ( int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++ )
        {
            SlotGemUID[i] = (long)weaponInfopiece_.slot_[i].gemUID_;
        }

        TableData = GameInfo.Instance.GameTable.FindWeapon(x => x.ID == TableID);
        AddCharacterId();
    }
}

public partial class GemData
{
    public void SetPktData(PktInfoGem.Piece gemInfopiece_)
    {
        GemUID = (long)gemInfopiece_.gemUID_;
        TableID = (int)gemInfopiece_.tableID_;
        Level = (int)gemInfopiece_.lv_;
        Exp = (int)gemInfopiece_.exp_;
        Wake = (int)gemInfopiece_.wake_;
        Lock = (bool)gemInfopiece_.lock_;
        SetOptID = (int)gemInfopiece_.setOptID_;

        if (gemInfopiece_.rstOpt_.optID_ == 0)
            TempOptIndex = -1;
        else
            TempOptIndex = (int)gemInfopiece_.rstIdx_;
        TempOptID = (int)gemInfopiece_.rstOpt_.optID_;
        TempOptValue = (int)gemInfopiece_.rstOpt_.value_;

        for (int i = 0; i < (int)eCOUNT.GEMRANDOPT; i++)
        {
            RandOptID[i] = (int)gemInfopiece_.opt_[i].optID_;
            RandOptValue[i] = (int)gemInfopiece_.opt_[i].value_;
        }

        TableData = GameInfo.Instance.GameTable.FindGem(x => x.ID == TableID);
    }
}

public partial class CardData
{
    public void SetPktData(PktInfoCard.Piece cardInfopiece_)
    {
        CardUID = (long)cardInfopiece_.cardUID_;
        TableID = (int)cardInfopiece_.tableID_;
        Level = cardInfopiece_.lv_;
        Exp = (int)cardInfopiece_.exp_;
        SkillLv = (int)cardInfopiece_.skillLv_;
        EnchantLv = (int)cardInfopiece_.encLv_;
        Wake = (int)cardInfopiece_.wake_;
        PosValue = (long)cardInfopiece_.posValue_;
        PosKind = (int)cardInfopiece_.posKind_;
        PosSlot = (int)cardInfopiece_.posSlotNum_;
        Lock = (bool)cardInfopiece_.lock_;
        Type = cardInfopiece_.type_;

        TableData = GameInfo.Instance.GameTable.FindCard(x => x.ID == TableID);

        if(Type == 0) // 0이면 테이블에 있는 속성값 사용
        {
            Type = TableData.Type;
        }
    }
}



public partial class ItemData
{
    public void SetPktData(PktInfoItem.Piece itemInfopiece_)
    {
        ItemUID = (long)itemInfopiece_.itemUID_;
        TableID = (int)itemInfopiece_.tableID_;
        Count = (int)itemInfopiece_.cnt_;
        TableData = GameInfo.Instance.GameTable.FindItem(x => x.ID == TableID);
    }
}


public partial class FacilityData
{
    public void SetPktData(PktInfoFacility.Piece facilityinfopiece_)
    {
        TableID = (int)facilityinfopiece_.tableID_;
        Level = (int)facilityinfopiece_.lv_;
        Stats = (int)facilityinfopiece_.GetState();
        Selete = (long)facilityinfopiece_.operationValue_;
        RemainTime = GameSupport.GetLocalTimeByServerTime(facilityinfopiece_.operationEndTime_.GetTime());
        OperationCnt = (int)facilityinfopiece_.operationCnt_;
        TableData = GameInfo.Instance.GameTable.FindFacility(x => x.ID == TableID);
        if (GameInfo.Instance.CardList.Count > 0)
        {
            for (int i = 0; i < GameInfo.Instance.CardList.Count; i++)
            {
                CardData carddata = GameInfo.Instance.CardList[i];
                if (carddata.PosKind.Equals((int)eContentsPosKind.FACILITY) && carddata.PosValue.Equals(TableID))
                {
                    EquipCardUID = carddata.CardUID;
                    break;
                }
            }
        }
        
        // !@#~서버 시설 작업으로 클라에서 수정할 부분~#@!
        //EquipCardUID = (long)facilityinfopiece_.cardUID_; <- Card 정보에서 얻어와서 설정해야 합니다.
        //7778  Selete = (int)facilityinfopiece_.tableID_;
        //Stats = (int)facilityinfopiece_.state_;   <- 시간(operationEndTime_) 값이 0이면 대기 있으면 진행중 또는 완료이며 완료 조건은 지금 클라의 시간보다 이전 시간이면 완료입니다.


        
    }
}

public partial class RoomThemeFuncData
{
    public void SetPktData(bool on )
    {
        On = on;
    }
}


public partial class RoomThemeSlotData
{
    public void SetPktData(PktInfoRoomThemeSlot.Piece piece_)
    {
        SlotNum = (int)piece_.slotNum_;
        TableID = (int)piece_.tableID_;
        RoomThemeFuncList.Clear();

        ArrLightInfo = piece_.data_.stm_.ToArray();

        var list = GameInfo.Instance.GameTable.FindAllRoomFunc(x => x.RoomTheme == TableID);

        for( int i = 0; i < list.Count; i++ )
            RoomThemeFuncList.Add( new RoomThemeFuncData(list[i].ID));

        for (int i = 0; i < list.Count; i++)
        {
            bool on = piece_.IsOnFunc((ulong)i);
            RoomThemeFuncList[i].SetPktData(on);
        }
        TableData = GameInfo.Instance.GameTable.FindRoomTheme(TableID);
    }
}

public partial class RoomThemeFigureSlotData
{
    public void SetPktData(PktInfoRoomFigureSlot.Piece piece_)
    {
        SlotNum = piece_.figureSlotNum_;
        RoomThemeSlotNum = piece_.themeSlotNum_;
        TableID = (int)piece_.tableID_;
        Action1 = (int)piece_.actionID1_;
        //Action2 = (int)piece_.actionID2_;
        /*
        if (piece_.detail_.stm_.Count == 0)
            detail = false;
        else
            detail = true;
        */
        TableData = GameInfo.Instance.GameTable.FindRoomFigure(TableID);
        detailarry = piece_.detail_.stm_.ToArray();

        CostumeStateFlag = (int)piece_.skinStateFlag_;

		if (TableData != null)
		{
			GameTable.Costume.Param find = GameInfo.Instance.GameTable.FindCostume(x => x.ID == TableData.ContentsIndex);
			if (find != null && find.SubHairChange == 1)
			{
				bool isOn = GameSupport._IsOnBitIdx((uint)CostumeStateFlag, (int)(eCostumeStateFlag.CSF_HAIR));

				uint flag = (uint)CostumeStateFlag;
				GameSupport._DoOnOffBitIdx(ref flag, (int)(eCostumeStateFlag.CSF_HAIR), !isOn);

				CostumeStateFlag = (int)flag;
			}
		}

		CostumeColor = piece_.costumeClr_;
    }
}

public partial class StageClearData
{
    public void SetPktData(PktInfoStageClear.Piece stagerInfopiece_)
    {
        TableID = (int)stagerInfopiece_.tableID_;
        TableData = GameInfo.Instance.GameTable.FindStage(TableID);

        if( TableData.Mission_00 != -1 )
        {
            if (stagerInfopiece_.IsOnClear(0))
                Mission[0] = TableData.Mission_00;
        }
        if (TableData.Mission_01 != -1)
        {
            if (stagerInfopiece_.IsOnClear(1))
                Mission[1] = TableData.Mission_01;
        }
        if (TableData.Mission_02 != -1)
        {
            if (stagerInfopiece_.IsOnClear(2))
                Mission[2] = TableData.Mission_02;
        }
    }
}

public partial class TimeAttackClearData
{
    public void SetPktData(PktInfoTimeAtkStageRec.Piece stagerInfopiece_)
    {
        HighestScoreRemainTime = GameSupport.GetLocalTimeByServerTime(stagerInfopiece_.delTime_.GetTime());         // 유저의 최고기록유지 시간
        CharCUID = (long)stagerInfopiece_.charUID_;              // 최고기록을 낸 캐릭터 ID
        TableID = (int)stagerInfopiece_.stageTID_;              // 스테이지 테이블 ID
        HighestScore = (int)stagerInfopiece_.timeRecord_Ms_;      // 최고기록
    }
}

public partial class TimeAttackRankUserData
{
    public void SetPktData(PktInfoTimeAtkRankStage.Piece rankuserInfopiece_, int rank)
    {
        Rank = rank;
        UUID = (long)rankuserInfopiece_.uuid_;

        mUserNickName = rankuserInfopiece_.nickName_;
        Utility.RemoveBBCode( ref mUserNickName );
        
        UserNickNameColorId = (int)rankuserInfopiece_.nickNameColorID_;
        UserRank = (int)rankuserInfopiece_.userLv_;
        UserMark = (int)rankuserInfopiece_.markID_;
        HighestScore = (int)rankuserInfopiece_.timeRecord_Ms_;
        bDetail = false;
        CharData = new CharData();
        CharData.TableID = (int)rankuserInfopiece_.charID_;
        CharData.EquipCostumeID = (int)rankuserInfopiece_.costumeID_;
        CharData.Level = (int)rankuserInfopiece_.charLv_;
        CharData.Grade = (int)rankuserInfopiece_.charGrade_;
        CharData.TableData = GameInfo.Instance.GameTable.FindCharacter(CharData.TableID);
    }

    public void SetPktData(PktInfoTimeAtkRankerDetailAck rankuserInfopiece_)
    {
        // !@# 패킷 메시지 캐릭터 세부 정보 변경 !@#

        bDetail = true;
        CharData.TableID = (int)rankuserInfopiece_.simpleInfo_.charID_;
        CharData.EquipCostumeID = (int)rankuserInfopiece_.simpleInfo_.costumeID_;
        CharData.Level = (int)rankuserInfopiece_.simpleInfo_.charLv_;
        CharData.Grade = (int)rankuserInfopiece_.simpleInfo_.charGrade_;
        CharData.CostumeStateFlag = (int)rankuserInfopiece_.charInfo_.skinStateFlag_;
        CharData.CostumeColor = (int)rankuserInfopiece_.charInfo_.costumeClr_;
        CharData.DyeingData = new DyeingData(rankuserInfopiece_.charInfo_.costumeDyeing_);
        for (int i = 0; i < rankuserInfopiece_.charInfo_.sklIDs_.Length; i++)
        {
            CharData.EquipSkill[i] = (int)rankuserInfopiece_.charInfo_.sklIDs_[i];
            if (CharData.EquipSkill[i] != (int)eCOUNT.NONE)
                CharData.PassvieList.Add(new PassiveData(CharData.EquipSkill[i], 1));
        }
        CharData.TableData = GameInfo.Instance.GameTable.FindCharacter(CharData.TableID);

        if(rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.MAIN].info_.ID_ != (int)eCOUNT.NONE)
        {
            WeaponData = new WeaponData((int)eCOUNT.NONE, (int)rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.MAIN].info_.ID_);
            WeaponData.Level = (int)rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.MAIN].info_.lv_;
            WeaponData.Wake = (int)rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.MAIN].info_.wake_;
            WeaponData.SkillLv = (int)rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.MAIN].info_.skilLv_;
            WeaponData.EnchantLv = rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.MAIN].info_.enc_;

            MainGemList.Clear();
            for(int i = 0; i < rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.MAIN].gems_.Length; i++)
            {
                GemData gemdata = new GemData((int)eCOUNT.NONE, (int)rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.MAIN].gems_[i].info_.ID_);
                if (rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.MAIN].gems_[i].info_.ID_ != (int)eCOUNT.NONE)
                {
                    gemdata.Level = (int)rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.MAIN].gems_[i].info_.lv_;
                    gemdata.Wake = (int)rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.MAIN].gems_[i].info_.wake_;
                    gemdata.SetOptID = (int)rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.MAIN].gems_[i].info_.setID_;

                    for (int j = 0; j < (int)eCOUNT.GEMRANDOPT; j++)
                    {
                        gemdata.RandOptID[j] = (int)rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.MAIN].gems_[i].opt_[j].optID_;
                        gemdata.RandOptValue[j] = (int)rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.MAIN].gems_[i].opt_[j].value_;
                    }
                }
                MainGemList.Add(gemdata);
            }
        }

        SubWeaponData = null;
        if (rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.SUB].info_.ID_ != (int)eCOUNT.NONE)
        {
            SubWeaponData = new WeaponData((int)eCOUNT.NONE, (int)rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.SUB].info_.ID_);
            SubWeaponData.Level = (int)rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.SUB].info_.lv_;
            SubWeaponData.Wake = (int)rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.SUB].info_.wake_;
            SubWeaponData.SkillLv = (int)rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.SUB].info_.skilLv_;
            SubWeaponData.EnchantLv = (int)rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.SUB].info_.enc_;

            SubGemList.Clear();
            for (int i = 0; i < rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.SUB].gems_.Length; i++)
            {
                GemData gemdata = new GemData((int)eCOUNT.NONE, (int)rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.SUB].gems_[i].info_.ID_);
                if (rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.SUB].gems_[i].info_.ID_ != (int)eCOUNT.NONE)
                {
                    gemdata.Level = (int)rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.SUB].gems_[i].info_.lv_;
                    gemdata.Wake = (int)rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.SUB].gems_[i].info_.wake_;
                    gemdata.SetOptID = (int)rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.SUB].gems_[i].info_.setID_;

                    for (int j = 0; j < (int)eCOUNT.GEMRANDOPT; j++)
                    {
                        gemdata.RandOptID[j] = (int)rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.SUB].gems_[i].opt_[j].optID_;
                        gemdata.RandOptValue[j] = (int)rankuserInfopiece_.charInfo_.wpns_[(int)eWeaponSlot.SUB].gems_[i].opt_[j].value_;
                    }
                }
                SubGemList.Add(gemdata);
            }
        }

        CardList.Clear();
        for (int i = 0; i < (int)eCardSlotPosMax.CHAR; i++)
        {
            if (rankuserInfopiece_.charInfo_.cards_[i].info_.ID_ != (int)eCOUNT.NONE)
            {
                CardData carddata = new CardData(i, (int)rankuserInfopiece_.charInfo_.cards_[i].info_.ID_);
                carddata.Level = (int)rankuserInfopiece_.charInfo_.cards_[i].info_.lv_;
                carddata.Wake = (int)rankuserInfopiece_.charInfo_.cards_[i].info_.wake_;
                carddata.SkillLv = (int)rankuserInfopiece_.charInfo_.cards_[i].info_.skilLv_;
                carddata.EnchantLv = (int)rankuserInfopiece_.charInfo_.cards_[i].info_.enc_;
                carddata.Type = (int)rankuserInfopiece_.charInfo_.cards_[i].type_;
                if ((int)carddata.Type == 0 && carddata.TableData != null)
                {
                    carddata.Type = carddata.TableData.Type;
                }

                CardList.Add(carddata);
                CharData.EquipCard[i] = carddata.TableID;
            }
            else
            {
                CardList.Add(null);
            }
        }
    }

    public void SetPktData( PktInfoRaidRankStage.Piece piece, int rank, bool isRaidFirstRanker = false ) {
        Rank = rank;
        UUID = (long)piece.uuid_;

        mUserNickName = piece.nickName_;
        Utility.RemoveBBCode( ref mUserNickName );

        UserNickNameColorId = (int)piece.nickNameColorID_;
        UserRank = (int)piece.userLv_;
        UserMark = (int)piece.markID_;
        HighestScore = (int)piece.timeRecord_Ms_;
        bDetail = false;

        CharData = new CharData();
        CharData.TableID = (int)piece.charID_;
        CharData.EquipCostumeID = (int)piece.costumeID_;
        CharData.Level = (int)piece.charLv_;
        CharData.Grade = (int)piece.charGrade_;
        CharData.TableData = GameInfo.Instance.GameTable.FindCharacter( CharData.TableID );

        IsRaidFirstRanker = isRaidFirstRanker;
    }

    public void SetPktData( PktInfoRaidRankerDetailAck pkt ) {
        bDetail = true;

        PktInfoConPosCharDetail charInfo = pkt.charInfo_[0];

        CharData.TableID = (int)pkt.simpleInfo_.charID_;
        CharData.EquipCostumeID = (int)pkt.simpleInfo_.costumeID_;
        CharData.Level = pkt.simpleInfo_.charLv_;
        CharData.Grade = pkt.simpleInfo_.charGrade_;
        CharData.CostumeStateFlag = (int)charInfo.skinStateFlag_;
        CharData.CostumeColor = charInfo.costumeClr_;
        CharData.DyeingData = new DyeingData( charInfo.costumeDyeing_ );
        
        for( int i = 0; i < charInfo.sklIDs_.Length; i++ ) {
            CharData.EquipSkill[i] = (int)charInfo.sklIDs_[i];

            if( CharData.EquipSkill[i] != (int)eCOUNT.NONE ) {
                CharData.PassvieList.Add( new PassiveData( CharData.EquipSkill[i], 1 ) );
            }
        }

        CharData.TableData = GameInfo.Instance.GameTable.FindCharacter( CharData.TableID );

        if( charInfo.wpns_[(int)eWeaponSlot.MAIN].info_.ID_ != (int)eCOUNT.NONE ) {
            WeaponData = new WeaponData( (int)eCOUNT.NONE, (int)charInfo.wpns_[(int)eWeaponSlot.MAIN].info_.ID_ );
            WeaponData.Level = charInfo.wpns_[(int)eWeaponSlot.MAIN].info_.lv_;
            WeaponData.Wake = charInfo.wpns_[(int)eWeaponSlot.MAIN].info_.wake_;
            WeaponData.SkillLv = charInfo.wpns_[(int)eWeaponSlot.MAIN].info_.skilLv_;
            WeaponData.EnchantLv = charInfo.wpns_[(int)eWeaponSlot.MAIN].info_.enc_;

            MainGemList.Clear();

            for( int i = 0; i < charInfo.wpns_[(int)eWeaponSlot.MAIN].gems_.Length; i++ ) {
                GemData gemdata = new GemData((int)eCOUNT.NONE, (int)charInfo.wpns_[(int)eWeaponSlot.MAIN].gems_[i].info_.ID_);

                if( charInfo.wpns_[(int)eWeaponSlot.MAIN].gems_[i].info_.ID_ != (int)eCOUNT.NONE ) {
                    gemdata.Level = charInfo.wpns_[(int)eWeaponSlot.MAIN].gems_[i].info_.lv_;
                    gemdata.Wake = charInfo.wpns_[(int)eWeaponSlot.MAIN].gems_[i].info_.wake_;
                    gemdata.SetOptID = charInfo.wpns_[(int)eWeaponSlot.MAIN].gems_[i].info_.setID_;

                    for( int j = 0; j < (int)eCOUNT.GEMRANDOPT; j++ ) {
                        gemdata.RandOptID[j] = charInfo.wpns_[(int)eWeaponSlot.MAIN].gems_[i].opt_[j].optID_;
                        gemdata.RandOptValue[j] = charInfo.wpns_[(int)eWeaponSlot.MAIN].gems_[i].opt_[j].value_;
                    }
                }

                MainGemList.Add( gemdata );
            }
        }

        SubWeaponData = null;
        if( charInfo.wpns_[(int)eWeaponSlot.SUB].info_.ID_ != (int)eCOUNT.NONE ) {
            SubWeaponData = new WeaponData( (int)eCOUNT.NONE, (int)charInfo.wpns_[(int)eWeaponSlot.SUB].info_.ID_ );
            SubWeaponData.Level = charInfo.wpns_[(int)eWeaponSlot.SUB].info_.lv_;
            SubWeaponData.Wake = charInfo.wpns_[(int)eWeaponSlot.SUB].info_.wake_;
            SubWeaponData.SkillLv = charInfo.wpns_[(int)eWeaponSlot.SUB].info_.skilLv_;
            SubWeaponData.EnchantLv = charInfo.wpns_[(int)eWeaponSlot.SUB].info_.enc_;

            SubGemList.Clear();

            for( int i = 0; i < charInfo.wpns_[(int)eWeaponSlot.SUB].gems_.Length; i++ ) {
                GemData gemdata = new GemData((int)eCOUNT.NONE, (int)charInfo.wpns_[(int)eWeaponSlot.SUB].gems_[i].info_.ID_);
            
                if( charInfo.wpns_[(int)eWeaponSlot.SUB].gems_[i].info_.ID_ != (int)eCOUNT.NONE ) {
                    gemdata.Level = charInfo.wpns_[(int)eWeaponSlot.SUB].gems_[i].info_.lv_;
                    gemdata.Wake = charInfo.wpns_[(int)eWeaponSlot.SUB].gems_[i].info_.wake_;
                    gemdata.SetOptID = charInfo.wpns_[(int)eWeaponSlot.SUB].gems_[i].info_.setID_;

                    for( int j = 0; j < (int)eCOUNT.GEMRANDOPT; j++ ) {
                        gemdata.RandOptID[j] = charInfo.wpns_[(int)eWeaponSlot.SUB].gems_[i].opt_[j].optID_;
                        gemdata.RandOptValue[j] = charInfo.wpns_[(int)eWeaponSlot.SUB].gems_[i].opt_[j].value_;
                    }
                }

                SubGemList.Add( gemdata );
            }
        }

        CardList.Clear();

        for( int i = 0; i < (int)eCardSlotPosMax.CHAR; i++ ) {
            if( charInfo.cards_[i].info_.ID_ != (int)eCOUNT.NONE ) {
                CardData carddata = new CardData(i, (int)charInfo.cards_[i].info_.ID_);
        
                carddata.Level = charInfo.cards_[i].info_.lv_;
                carddata.Wake = charInfo.cards_[i].info_.wake_;
                carddata.SkillLv = charInfo.cards_[i].info_.skilLv_;
                carddata.EnchantLv = charInfo.cards_[i].info_.enc_;
                carddata.Type = charInfo.cards_[i].type_;

                if( carddata.Type == 0 && carddata.TableData != null ) {
                    carddata.Type = carddata.TableData.Type;
                }

                CardList.Add( carddata );
                CharData.EquipCard[i] = carddata.TableID;
            }
            else {
                CardList.Add( null );
            }
        }
    }
}

public partial class TimeAttackRankData
{
    public void SetPktData(PktInfoTimeAtkRankingHeader.Piece rankInfopiece_)
    {
        TableID = (int)rankInfopiece_.stageID_;
        UpdateTM = (long)rankInfopiece_.updateTM_;
        TableData = GameInfo.Instance.GameTable.FindStage(TableID);
    }
}


public partial class WeaponBookData
{
    public void SetPktData(PktInfoWeaponBook.Piece weaponbookInfopiece_)
    {
        TableID = (int)weaponbookInfopiece_.tableID_;
        StateFlag = (int)weaponbookInfopiece_.stateFlag_;
    }
}


public partial class CardBookData
{
    public void SetPktData(PktInfoCardBook.Piece cardbookInfopiece_)
    {
        TableID = (int)cardbookInfopiece_.tableID_;
        FavorExp = (int)cardbookInfopiece_.favorExp_;
        FavorLevel = (int)cardbookInfopiece_.favorLv_;
        StateFlag = (int)cardbookInfopiece_.stateFlag_;
    }
}

public partial class MonsterBookData
{
    public void SetPktData(PktInfoMonsterBook.Piece monsterbookInfopiece_)
    {
        TableID = (int)monsterbookInfopiece_.tableID_;
        StateFlag = (int)monsterbookInfopiece_.stateFlag_;
    }
}

public partial class MailData
{
    public void SetPktData(PktInfoMail.Piece pktInfomailInfopiece_)
    {
        MailUID = pktInfomailInfopiece_.mailUID_;
        MailType = (eMailType)pktInfomailInfopiece_.typeID_;

        //MailTypeValue = string.IsNullOrEmpty(pktInfomailInfopiece_.typeValue_) ? 0 : Convert.ToInt32(pktInfomailInfopiece_.typeValue_);
        MailTypeValue = string.IsNullOrEmpty(pktInfomailInfopiece_.typeValue_) ? "0" : pktInfomailInfopiece_.typeValue_;
        Log.Show(MailType + " / " + MailTypeValue, Log.ColorType.Red);
        var pktTime = pktInfomailInfopiece_.endTime_;
        RemainTime = new System.DateTime(pktTime.Year(), pktTime.Month(), pktTime.MDay(), pktTime.Hour(), pktTime.Minute(), pktTime.Second(), pktTime.MilliSec(), System.DateTimeKind.Local);

        ProductIndex = pktInfomailInfopiece_.productInfo_.index_;
        ProductType = (int)pktInfomailInfopiece_.productInfo_.type_;
        ProductValue = pktInfomailInfopiece_.productInfo_.value_;
    }
}

public partial class WeekMissionData
{
    //  서버 데이터 셋팅

    public void SetPktData(PktInfoMission.Weekly pktInfoWeeklyMission_)
    {
        fWeekMissionSetID = pktInfoWeeklyMission_.comInfo_.tableID_;                  //  주간 미션 Set ID
        fWeekMissionResetDate = GameSupport.GetLocalTimeByServerTime(pktInfoWeeklyMission_.comInfo_.resetTime_.GetTime());  //  주간 미션 Set ID 초기화 예정 시간
        fMissionRemainCntSlot = pktInfoWeeklyMission_.condiVal_;                        //  주간 미션 슬롯 0 ~ 6 남은 목표 횟수
        fMissionRewardFlag = pktInfoWeeklyMission_.rewardFlag_;                         //  주간 미션 보상 획득 비트 플래그
    }
}

public partial class GllaMissionData
{
    public void SetPktData(PktInfoMission.Guerrilla.Piece _pktInfo)
    {
        GroupID = (int)_pktInfo.groupID_;
        Count = (int)_pktInfo.count_;
        Step = (int)_pktInfo.step_;
    }
}

public partial class EventSetData
{
    public void SetPktData(PktInfoEventReward.Piece pktEvent)
    {
        RewardItemCount.Clear();

        TableID = (int)pktEvent.tableID_;
        RewardStep = (int)pktEvent.step_;
        Count = (int)pktEvent.value_;
        TableData = GameInfo.Instance.GameTable.FindEventSet(x => x.EventID == TableID);

        for (int i = 0; i < pktEvent.cnts_.Length; i++)
            RewardItemCount.Add((int)pktEvent.cnts_[i]);
    }

    public void UpdatePktData(PktInfoEventReward.Piece pktEvent)
    {
        TableID = (int)pktEvent.tableID_;
        RewardStep = (int)pktEvent.step_;
        Count = (int)pktEvent.value_;
        TableData = GameInfo.Instance.GameTable.FindEventSet(x => x.EventID == TableID);

        for (int i = 0; i < pktEvent.cnts_.Length; i++)
            RewardItemCount[i] = (int)pktEvent.cnts_[i];
    }
}



public partial class UserBattleData
{
    public void SetPktData(PktInfoUserArenaRec pktArenaRec)
    {
        Now_Score = (int)pktArenaRec.nowScore_;
        //Now_Rank = (int)pktArenaRec.nowRank_;         //보상 받을때만 사용
        Now_WinLoseCnt = (int)pktArenaRec.nowWinLoseCnt_;
        //Now_RewardFalg                                //계산해서 적용
        Now_RewardDate = GameSupport.GetLocalTimeByServerTime(pktArenaRec.lastRewardTime_.GetTime());     //보상 받을 시즌 시간
        Now_GradeId = (int)pktArenaRec.nowGradeID_;
        
        //Now_RewardDate = new DateTime(2020, 3, 8, 22, 0, 0);

        Now_PromotionWinCnt = (int)pktArenaRec.promotionWinCnt_;
        Now_PromotionRemainCnt = (int)pktArenaRec.promotionRemainCnt_;

        SR_BestScore = (int)pktArenaRec.sr_BestScore_;
        SR_BestWinningStreak = (int)pktArenaRec.sr_BestWinCnt_;
        SR_TotalCnt = (int)pktArenaRec.sr_TotalCnt_;
        SR_FirstWinCnt = (int)pktArenaRec.sr_FirstWinCnt_;
        SR_SecondWinCnt = (int)pktArenaRec.sr_SecondWinCnt_;
        SR_ThirdWinCnt = (int)pktArenaRec.sr_ThirdWinCnt_;
        SR_Rank = (int)pktArenaRec.exSeasonRank_;

        CheatValue = pktArenaRec.cheatValue_;
        CheatType = (eArenaCheat)pktArenaRec.cheatType_;
    }
    public void UpdatePktData(PktInfoUserArenaRec pktArenaRec)
    {
        Now_Score = (int)pktArenaRec.nowScore_;
        //Now_Rank = (int)pktArenaRec.nowRank_;         //보상 받을때만 사용
        Now_WinLoseCnt = (int)pktArenaRec.nowWinLoseCnt_;
        //Now_RewardFalg                                //계산해서 적용
        Now_RewardDate = GameSupport.GetLocalTimeByServerTime(pktArenaRec.lastRewardTime_.GetTime());
        Now_GradeId = (int)pktArenaRec.nowGradeID_;
        Now_PromotionWinCnt = (int)pktArenaRec.promotionWinCnt_;
        Now_PromotionRemainCnt = (int)pktArenaRec.promotionRemainCnt_;

        SR_BestScore = (int)pktArenaRec.sr_BestScore_;
        SR_BestWinningStreak = (int)pktArenaRec.sr_BestWinCnt_;
        SR_TotalCnt = (int)pktArenaRec.sr_TotalCnt_;
        SR_FirstWinCnt = (int)pktArenaRec.sr_FirstWinCnt_;
        SR_SecondWinCnt = (int)pktArenaRec.sr_SecondWinCnt_;
        SR_ThirdWinCnt = (int)pktArenaRec.sr_ThirdWinCnt_;
        SR_Rank = (int)pktArenaRec.exSeasonRank_;
    }
}

public partial class BadgeData
{
    public void SetPktData(PktInfoBadge.Piece pktInfoBadge)
    {
        BadgeUID = (long)pktInfoBadge.badgeUID_;
        for(int i = 0; i < (int)eCOUNT.BADGEOPTCOUNT; i++)
        {
            OptID[i] = (int)pktInfoBadge.opt_[i].optID_;
            OptVal[i] = (int)pktInfoBadge.opt_[i].value_;
        }

        Level = (int)pktInfoBadge.lv_;
        RemainLvCnt = (int)pktInfoBadge.remainLvCnt_;
        Lock = pktInfoBadge.lock_;

        PosValue = (long)pktInfoBadge.posValue_;
        PosKind = (int)pktInfoBadge.posKind_;
        
        PosSlotNum = (int)pktInfoBadge.posSlotNum_;
    }
    public void SetPktData(PktInfoArenaUserBadge pktInfoArenaUserBadge)
    {
        for(int i = 0; i < (int)eCOUNT.BADGEOPTCOUNT; i++)
        {
            OptID[i] = (int)pktInfoArenaUserBadge.opt_[i].optID_;
            OptVal[i] = (int)pktInfoArenaUserBadge.opt_[i].value_;
        }

        Level = (int)pktInfoArenaUserBadge.lv_;
    }

    public void UpdatePktData()
    {

    }
}

public partial class TeamCharData
{
    public void SetPktData(PktInfoConPosCharDetail pktInfoConPosCharDetail)
    {
        if (CharData != null)
            CharData = null;
        CharData = new CharData();

        CharData.TableID = (int)pktInfoConPosCharDetail.charID_;
        CharData.Level = (int)pktInfoConPosCharDetail.lv_;
        CharData.Grade = (int)pktInfoConPosCharDetail.grade_;
        CharData.EquipCostumeID = (int)pktInfoConPosCharDetail.costumeID_;
        CharData.CostumeStateFlag = (int)pktInfoConPosCharDetail.skinStateFlag_;
        CharData.CostumeColor = (int)pktInfoConPosCharDetail.costumeClr_;
        CharData.DyeingData = new DyeingData(pktInfoConPosCharDetail.costumeDyeing_);
        CharData.TableData = GameInfo.Instance.GameTable.FindCharacter(CharData.TableID);        

        //Skill
        for (int i = 0; i < pktInfoConPosCharDetail.sklIDs_.Length; i++)
        {
            CharData.EquipSkill[i] = (int)pktInfoConPosCharDetail.sklIDs_[i];
            if (CharData.EquipSkill[i] != (int)eCOUNT.NONE)
                CharData.PassvieList.Add(new PassiveData(CharData.EquipSkill[i], 1));
        }

        // 스킬 단련
        for (int i = 0; i < pktInfoConPosCharDetail.cskls_.Count; i++)
        {
            CharData.PassvieList.Add(new PassiveData((int)pktInfoConPosCharDetail.cskls_[i].id_, pktInfoConPosCharDetail.cskls_[i].lv_));
        }

        //Weapon
        if (pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.MAIN].info_.ID_ != (int)eCOUNT.NONE)
        {
            MainWeaponData = new WeaponData((int)eCOUNT.NONE, (int)pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.MAIN].info_.ID_);
            MainWeaponData.Level = (int)pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.MAIN].info_.lv_;
            MainWeaponData.Wake = (int)pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.MAIN].info_.wake_;
            MainWeaponData.SkillLv = (int)pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.MAIN].info_.skilLv_;
            MainWeaponData.EnchantLv = (int)pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.MAIN].info_.enc_;

            MainGemList.Clear();
            for(int i = 0; i < pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.MAIN].gems_.Length; i++)
            {
                GemData gemdata = new GemData((int)eCOUNT.NONE, (int)pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.MAIN].gems_[i].info_.ID_);
                if(pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.MAIN].gems_[i].info_.ID_ != (int)eCOUNT.NONE)
                {
                    gemdata.Level = (int)pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.MAIN].gems_[i].info_.lv_;
                    gemdata.Wake = (int)pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.MAIN].gems_[i].info_.wake_;
                    gemdata.SetOptID = (int)pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.MAIN].gems_[i].info_.setID_;

                    for (int j = 0; j < (int)eCOUNT.GEMRANDOPT; j++)
                    {
                        gemdata.RandOptID[j] = (int)pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.MAIN].gems_[i].opt_[j].optID_;
                        gemdata.RandOptValue[j] = (int)pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.MAIN].gems_[i].opt_[j].value_;
                    }
                }
                MainGemList.Add(gemdata);
            }
        }

        SubWeaponData = null;
        if(pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.SUB].info_.ID_ != (int)eCOUNT.NONE)
        {
            SubWeaponData = new WeaponData((int)eCOUNT.NONE, (int)pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.SUB].info_.ID_);
            SubWeaponData.Level = (int)pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.SUB].info_.lv_;
            SubWeaponData.Wake = (int)pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.SUB].info_.wake_;
            SubWeaponData.SkillLv = (int)pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.SUB].info_.skilLv_;
            SubWeaponData.EnchantLv = (int)pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.SUB].info_.enc_;

            SubGemList.Clear();
            for(int i = 0; i < pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.SUB].gems_.Length; i++)
            {
                GemData gemdata = new GemData((int)eCOUNT.NONE, (int)pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.SUB].gems_[i].info_.ID_);
                if(pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.SUB].gems_[i].info_.ID_ != (int)eCOUNT.NONE)
                {
                    gemdata.Level = (int)pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.SUB].gems_[i].info_.lv_;
                    gemdata.Wake = (int)pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.SUB].gems_[i].info_.wake_;
                    gemdata.SetOptID = (int)pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.SUB].gems_[i].info_.setID_;

                    for (int j = 0; j < (int)eCOUNT.GEMRANDOPT; j++)
                    {
                        gemdata.RandOptID[j] = (int)pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.SUB].gems_[i].opt_[j].optID_;
                        gemdata.RandOptValue[j] = (int)pktInfoConPosCharDetail.wpns_[(int)eWeaponSlot.SUB].gems_[i].opt_[j].value_;
                    }
                }
                SubGemList.Add(gemdata);
            }
        }

        CardList.Clear();
        for(int i = 0; i < (int)eCardSlotPosMax.CHAR; i++)
        {
            if(pktInfoConPosCharDetail.cards_[i].info_.ID_ != (int)eCOUNT.NONE)
            {
                CardData carddata = new CardData(i, (int)pktInfoConPosCharDetail.cards_[i].info_.ID_);
                carddata.Level = (int)pktInfoConPosCharDetail.cards_[i].info_.lv_;
                carddata.Wake = (int)pktInfoConPosCharDetail.cards_[i].info_.wake_;
                carddata.SkillLv = (int)pktInfoConPosCharDetail.cards_[i].info_.skilLv_;
                carddata.EnchantLv = pktInfoConPosCharDetail.cards_[i].info_.enc_;

                carddata.Type = (int)pktInfoConPosCharDetail.cards_[i].type_;
                if((int)carddata.Type == 0 && carddata.TableData != null)
                {
                    carddata.Type = carddata.TableData.Type;
                }

                CardList.Add(carddata);

                CharData.EquipCard[i] = carddata.TableID;
            }
            else
            {
                CardList.Add(null);
            }
        }

        ListAwakenSkillInfo.Clear();
        for (int i = 0; i < pktInfoConPosCharDetail.askls_.Count; i++)
        {
            AwakenSkillInfo info = new AwakenSkillInfo();
            info.SetPktData(pktInfoConPosCharDetail.askls_[i].id_, pktInfoConPosCharDetail.askls_[i].lv_);

            ListAwakenSkillInfo.Add(info);
        }
    }

    //상대 유저 정보
    public void SetPktData(PktinfoArenaEnemy.CharInfo enemyCharInfo)
    {
        if (CharData != null)
            CharData = null;
        CharData = new CharData();

        CharData.TableID = (int)enemyCharInfo.charID_;
        CharData.Level = (int)enemyCharInfo.charLv_;
        CharData.Grade = (int)enemyCharInfo.charGrade_;
        CharData.EquipCostumeID = (int)enemyCharInfo.costumeID_;
        //CharData.CostumeStateFlag = (int)enemyCharInfo.skinStateFlag_;
        CharData.CostumeColor = (int)enemyCharInfo.costumeClr_;
        CharData.DyeingData = new DyeingData(enemyCharInfo.costumeDyeing_);
        CharData.TableData = GameInfo.Instance.GameTable.FindCharacter(CharData.TableID);
        //Skill
        for (int i = 0; i < enemyCharInfo.skillIDs_.Length; i++)
        {
            CharData.EquipSkill[i] = (int)enemyCharInfo.skillIDs_[i];
            if (CharData.EquipSkill[i] != (int)eCOUNT.NONE)
                CharData.PassvieList.Add(new PassiveData(CharData.EquipSkill[i], 1));
        }

        //Weapon
        MainWeaponData = null;
        MainGemList.Clear();
        if (enemyCharInfo.weapon_[(int)eWeaponSlot.MAIN].ID_ != (int)eCOUNT.NONE)
        {
            MainWeaponData = new WeaponData((int)eCOUNT.NONE, (int)enemyCharInfo.weapon_[(int)eWeaponSlot.MAIN].ID_);
            MainWeaponData.Level = (int)enemyCharInfo.weapon_[(int)eWeaponSlot.MAIN].lv_;
            MainWeaponData.Wake = (int)enemyCharInfo.weapon_[(int)eWeaponSlot.MAIN].wake_;
            MainWeaponData.SkillLv = (int)enemyCharInfo.weapon_[(int)eWeaponSlot.MAIN].skilLv_;
            MainWeaponData.EnchantLv = (int)enemyCharInfo.weapon_[(int)eWeaponSlot.MAIN].enc_;
        }
           
        SubWeaponData = null;
        SubGemList.Clear();
        if (enemyCharInfo.weapon_[(int)eWeaponSlot.SUB].ID_ != (int)eCOUNT.NONE)
        {
            SubWeaponData = new WeaponData((int)eCOUNT.NONE, (int)enemyCharInfo.weapon_[(int)eWeaponSlot.SUB].ID_);
            SubWeaponData.Level = (int)enemyCharInfo.weapon_[(int)eWeaponSlot.SUB].lv_;
            SubWeaponData.Wake = (int)enemyCharInfo.weapon_[(int)eWeaponSlot.SUB].wake_;
            SubWeaponData.SkillLv = (int)enemyCharInfo.weapon_[(int)eWeaponSlot.SUB].skilLv_;
            SubWeaponData.EnchantLv = (int)enemyCharInfo.weapon_[(int)eWeaponSlot.SUB].enc_;
        }

        //Card
        CardList.Clear();
        for (int i = 0; i < (int)eCardSlotPosMax.CHAR; i++)
        {
            if (enemyCharInfo.card_[i].info_.ID_ != (int)eCOUNT.NONE)
            {
                CardData carddata = new CardData(i, (int)enemyCharInfo.card_[i].info_.ID_);
                carddata.Level = (int)enemyCharInfo.card_[i].info_.lv_;
                carddata.Wake = (int)enemyCharInfo.card_[i].info_.wake_;
                carddata.SkillLv = (int)enemyCharInfo.card_[i].info_.skilLv_;
                carddata.EnchantLv = (int)enemyCharInfo.card_[i].info_.enc_;

                carddata.Type = (int)enemyCharInfo.card_[i].type_;
                if(carddata.Type == 0 && carddata.TableData != null)
                {
                    carddata.Type = carddata.TableData.Type;
                }

                CardList.Add(carddata);
                CharData.EquipCard[i] = carddata.TableID;
            }
            else
            {
                CardList.Add(null);
            }
        }

        ListAwakenSkillInfo.Clear();
        for (int i = 0; i < enemyCharInfo.askls_.Count; i++)
        {
            AwakenSkillInfo info = new AwakenSkillInfo();
            info.SetPktData(enemyCharInfo.askls_[i].id_, enemyCharInfo.askls_[i].lv_);

            ListAwakenSkillInfo.Add(info);
        }
    }

    public void SetPktData(PktInfoArenaSimple.CharInfo pktCharInfo)
    {
        if (CharData != null)
            CharData = null;

        if(pktCharInfo.charID_ != (int)eCOUNT.NONE)
        {
            CharData = new CharData();

            CharData.TableID = (int)pktCharInfo.charID_;
            CharData.EquipCostumeID = (int)pktCharInfo.costumeID_;
            CharData.Level = (int)pktCharInfo.charLv_;
            CharData.Grade = (int)pktCharInfo.charGrade_;
            CharData.TableData = GameInfo.Instance.GameTable.FindCharacter(CharData.TableID);
        }
        
        MainWeaponData = null;
        SubWeaponData = null;

        

        MainGemList.Clear();
        SubGemList.Clear();
        CardList.Clear();
    }
    public void UpdatePktData()
    {

    }
}

public partial class TeamData
{
    public void SetPktData(PktInfoArenaDetail pktInfoArenaDetail)
    {
        bDetail = true;

        SetUserNickName( pktInfoArenaDetail.userInfo_.nickName_ );
        UserNickNameColorId = (int)pktInfoArenaDetail.userInfo_.nickNameColorID_;
        UUID = (long)pktInfoArenaDetail.userInfo_.uuid_;
        Score = (long)pktInfoArenaDetail.userInfo_.score_;
        Grade = (int)pktInfoArenaDetail.userInfo_.grade_;
        Tier = (int)pktInfoArenaDetail.userInfo_.tier_;
        UserLv = (int)pktInfoArenaDetail.userInfo_.userLv_;
        Rank = (int)pktInfoArenaDetail.userInfo_.rank_;
        UserMark = (int)pktInfoArenaDetail.userInfo_.markID_;
        TeamPower = (int)pktInfoArenaDetail.userInfo_.teamPower_;

        CardFormtaionID = (int)pktInfoArenaDetail.userInfo_.cardFrmtID_;
        TeamHP = (int)pktInfoArenaDetail.userInfo_.teamHP_;
        TeamATK = (int)pktInfoArenaDetail.userInfo_.teamAtk_;

        charlist.Clear();
        for(int i = 0; i < pktInfoArenaDetail.charInfos_.Length; i++)
        {
            TeamCharData teamCharData = new TeamCharData();
            teamCharData.SetPktData(pktInfoArenaDetail.charInfos_[i]);
            if (teamCharData.CharData != null && teamCharData.CharData.TableData != null)
            {
                teamCharData.CreateCUID(UUID);
            }
            charlist.Add(teamCharData);
        }

        badgelist.Clear();
        for (int i = 0; i < pktInfoArenaDetail.badgeInfos_.Length; i++)
        {
            BadgeData badgedata = new BadgeData();
            badgedata.SetPktData(pktInfoArenaDetail.badgeInfos_[i]);
            badgelist.Add(badgedata);
        }
    }

    //행킹 정보
    public void SetPktData(PktInfoArenaSimple pktInfoArenaSimple)
    {
        bDetail = false;
        SetUserNickName( pktInfoArenaSimple.userInfo_.nickName_ );
        UserNickNameColorId = (int)pktInfoArenaSimple.userInfo_.nickNameColorID_;
        UUID = (long)pktInfoArenaSimple.userInfo_.uuid_;
        Score = (long)pktInfoArenaSimple.userInfo_.score_;
        Grade = (int)pktInfoArenaSimple.userInfo_.grade_;
        Tier = (int)pktInfoArenaSimple.userInfo_.tier_;
        UserLv = (int)pktInfoArenaSimple.userInfo_.userLv_;
        Rank = (int)pktInfoArenaSimple.userInfo_.rank_;
        UserMark = (int)pktInfoArenaSimple.userInfo_.markID_;
        TeamPower = (int)pktInfoArenaSimple.userInfo_.teamPower_;
        charlist.Clear();
        for(int i = 0; i < pktInfoArenaSimple.charInfos_.Length; i++)
        {
            TeamCharData teamCharData = new TeamCharData();
            teamCharData.SetPktData(pktInfoArenaSimple.charInfos_[i]);
            charlist.Add(teamCharData);
        }

        badgelist.Clear();
        for(int i = 0; i < pktInfoArenaSimple.badgeInfos_.Length; i++)
        {
            BadgeData badgedata = new BadgeData();
            badgedata.SetPktData(pktInfoArenaSimple.badgeInfos_[i]);
            badgelist.Add(badgedata);
        }
    }

    public void SetPktData(PktinfoArenaEnemy pktinfoArenaEnemy)
    {
        bDetail = false;
        SetUserNickName( pktinfoArenaEnemy.userInfo_.nickName_ );
        UserNickNameColorId = (int)pktinfoArenaEnemy.userInfo_.nickNameColorID_;
        UUID = (long)pktinfoArenaEnemy.userInfo_.uuid_;
        Score = (long)pktinfoArenaEnemy.userInfo_.score_;
        Grade = (int)pktinfoArenaEnemy.userInfo_.grade_;
        Tier = (int)pktinfoArenaEnemy.userInfo_.tier_;
        UserLv = (int)pktinfoArenaEnemy.userInfo_.userLv_;
        Rank = (int)pktinfoArenaEnemy.userInfo_.rank_;
        UserMark = (int)pktinfoArenaEnemy.userInfo_.markID_;
        TeamPower = (int)pktinfoArenaEnemy.userInfo_.teamPower_;

        charlist.Clear();
        for(int i = 0; i < pktinfoArenaEnemy.charInfos_.Length; i++)
        {
            TeamCharData teamCharData = new TeamCharData();
            teamCharData.SetPktData(pktinfoArenaEnemy.charInfos_[i]);
            charlist.Add(teamCharData);
        }

        badgelist.Clear();
        for (int i = 0; i < pktinfoArenaEnemy.badgeInfos_.Length; i++)
        {
            BadgeData badgedata = new BadgeData();
            badgedata.SetPktData(pktinfoArenaEnemy.badgeInfos_[i]);
            badgedata.PosSlotNum = i;
            if(badgedata.OptID[(int)eBadgeOptSlot.FIRST] != (int)eCOUNT.NONE)
            {
                badgedata.PosKind = (int)eContentsPosKind.ARENA;
            }
            badgelist.Add(badgedata);
        }

        CardFormtaionID = (int)pktinfoArenaEnemy.userInfo_.cardFrmtID_;
        TeamHP = pktinfoArenaEnemy.userInfo_.teamHP_;
        TeamATK = pktinfoArenaEnemy.userInfo_.teamAtk_;
    }

    public void UpdatePktData()
    {

    }
}
//아레나 랭킹 관련
public partial class ArenaRankingListData
{
    public void SetPktData(PktInfoArenaRankList pktInfoArenaRankList)
    {
        if(UpdateTM != (long)pktInfoArenaRankList.updateTM_)
        {
            RankingSimpleList.Clear();
            for (int i = 0; i < pktInfoArenaRankList.infos_.Count; i++)
            {
                TeamData teamdata = new TeamData();
                teamdata.SetPktData(pktInfoArenaRankList.infos_[i]);
                if (teamdata.Score > 0)      //서버에서 받은 리스트중 점수가 있는 유저만 랭킹에 추가
                    RankingSimpleList.Add(teamdata);
            }
        }
        
        UpdateTM = (long)pktInfoArenaRankList.updateTM_;
        Log.Show(pktInfoArenaRankList.infos_.Count + " / " + UpdateTM, Log.ColorType.Red);
    }
    public void UpdatePktData()
    {

    }
}

//PassMission
public partial class PassSetData
{

    public void SetPassSetData(PktInfoPass.Piece pkt)
    {
        PassBuyEndTime = GameSupport.GetLocalTimeByServerTime(pkt.rwdSEndTM_.GetTime());
        Pass_NormalReward = pkt.stepN_;
        Pass_SPReward = pkt.stepS_;
        PassID = (int)pkt.tid_;
        PassTableData = GameInfo.Instance.GameTable.FindPassSet(x => x.PassID == PassID);
    }
}

//Friend
public partial class FriendUserData
{
    public void SetFriendUserData(PktInfoFriend.Piece pktInfoFriendPiece)
    {
        mNickName = pktInfoFriendPiece.info_.nickName_.str_;
        Utility.RemoveBBCode( ref mNickName );

        NickNameColorId = (int)pktInfoFriendPiece.info_.nickNameColorID_;
        LastConnectTime = GameSupport.GetLocalTimeByServerTime(pktInfoFriendPiece.info_.lastConnTM_.GetTime());
        UUID = (long)pktInfoFriendPiece.info_.uuid_;
        UserMark = (int)pktInfoFriendPiece.info_.mark_;
        DBID = (ulong)pktInfoFriendPiece.info_.dbID_;
        Rank = (int)pktInfoFriendPiece.info_.rank_;
        RoomSlotNum = (int)pktInfoFriendPiece.info_.roomSlotNum_;
        ClearArenaTowerID = (int)pktInfoFriendPiece.info_.arenaTWID_;

        FriendTotalFlag = pktInfoFriendPiece.flag_;

        HasArenaInfo = pktInfoFriendPiece.arena_;

        UpdateBitFlag();
    }

    public void SetFriendUserData(PktInfoComCommuUser.Piece pktInfoComCommuUserPiece)
    {
        mNickName = pktInfoComCommuUserPiece.nickName_.str_;
        Utility.RemoveBBCode( ref mNickName );

        NickNameColorId = (int)pktInfoComCommuUserPiece.nickNameColorID_;
        LastConnectTime = GameSupport.GetLocalTimeByServerTime(pktInfoComCommuUserPiece.lastConnTM_.GetTime());
        UUID = (long)pktInfoComCommuUserPiece.uuid_;
        UserMark = (int)pktInfoComCommuUserPiece.mark_;
        DBID = (ulong)pktInfoComCommuUserPiece.dbID_;
        Rank = (int)pktInfoComCommuUserPiece.rank_;
        RoomSlotNum = (int)pktInfoComCommuUserPiece.roomSlotNum_;
        ClearArenaTowerID = (int)pktInfoComCommuUserPiece.arenaTWID_;

        CircleInfo.SetPktInfoCircleSimple(pktInfoComCommuUserPiece.circleInfo_);
        CircleAuthLevel.AuthLevel = pktInfoComCommuUserPiece.circleAuthInfo_.authLv_;

        FriendTotalFlag = 0;
        FriendPointTakeFlag = false;
        FriendRoomFlagWithMyRoom = false;
        FriendRoomFlagWithFriendRoom = false;
    }

    public void UpdateBitFlag()
    {
        FriendPointTakeFlag = GameSupport._IsOnBitIdx(FriendTotalFlag, (int)eFriendFlag.TAKE_FP);
        FriendRoomFlagWithMyRoom = GameSupport._IsOnBitIdx(FriendTotalFlag, (int)eFriendFlag.MY_ROOM_VISIT);
        FriendRoomFlagWithFriendRoom = GameSupport._IsOnBitIdx(FriendTotalFlag, (int)eFriendFlag.FRIEND_ROOM_VISIT);
    }

    public void UpdateBitFlag(eFriendFlag flag, bool flagValue)
    {
        GameSupport._DoOnOffBitIdx(ref FriendTotalFlag, (int)flag, flagValue);
        UpdateBitFlag();
    }
}

public partial class CommunityData
{
    public void SetCommunityData(PktInfoCommunity pktInfoCommunity)
    {
        FriendList.Clear();
        FriendToAskList.Clear();
        FriendAskFromUserList.Clear();

        SetFriendList(pktInfoCommunity.friends_);

        SetFriendToAskList(pktInfoCommunity.friToAsk_);

        SetFriendAskFromUserList(pktInfoCommunity.friAskFromUser_);
    }

    public void SetFriendList(PktInfoFriend pktInfoFriend)
    {
        FriendList.Clear();

        if(null != pktInfoFriend)
        {
            for(int i = 0;i < pktInfoFriend.infos_.Count; i++)
            {
                if (null == pktInfoFriend.infos_[i])
                    continue;

                FriendUserData friendUserData = new FriendUserData();
                friendUserData.SetFriendUserData(pktInfoFriend.infos_[i]);

                FriendList.Add(friendUserData);
            }
        }
    }

    public void SetFriendToAskList(PktInfoFriend pktInfoFriend)
    {
        FriendToAskList.Clear();

        if (null != pktInfoFriend)
        {
            for (int i = 0; i < pktInfoFriend.infos_.Count; i++)
            {
                if (null == pktInfoFriend.infos_[i])
                    continue;

                FriendUserData friendUserData = new FriendUserData();
                friendUserData.SetFriendUserData(pktInfoFriend.infos_[i]);

                FriendToAskList.Add(friendUserData);
            }
        }
    }

    public void UpdateFriendToAskList(PktInfoFriend pktInfoFriend)
    {
        if(null != pktInfoFriend)
        {
            for(int i= 0; i < pktInfoFriend.infos_.Count; i++)
            {
                if (null == pktInfoFriend.infos_[i])
                    continue;

                FriendUserData friendUserData = FriendToAskList.Find(x => x.UUID == (long)pktInfoFriend.infos_[i].info_.uuid_);
                if(friendUserData == null)
                {
                    FriendUserData friendUser = new FriendUserData();
                    friendUser.SetFriendUserData(pktInfoFriend.infos_[i]);

                    FriendToAskList.Add(friendUser);

                    FriendUserData delFriendUserData = FriendSuggestList.Find(x => x.UUID == (long)pktInfoFriend.infos_[i].info_.uuid_);
                    if (delFriendUserData != null)
                        FriendSuggestList.Remove(delFriendUserData);
                }
            }
        }
    }

    public void RemoveFriendToAskList(PktInfoUIDList pktInfoUIDList)
    {
        if(null != pktInfoUIDList)
        {
            for(int i = 0; i < pktInfoUIDList.uids_.Count; i++)
            {
                FriendUserData friendUserData = FriendToAskList.Find(x => x.UUID == (long)pktInfoUIDList.uids_[i]);
                if (friendUserData != null)
                    FriendToAskList.Remove(friendUserData);
            }
        }
    }

    public void SetFriendAskFromUserList(PktInfoFriend pktInfoFriend)
    {
        FriendAskFromUserList.Clear();

        if(null != pktInfoFriend)
        {
            for(int i = 0; i < pktInfoFriend.infos_.Count; i++)
            {
                if (null == pktInfoFriend.infos_[i])
                    continue;

                FriendUserData friendUserData = new FriendUserData();
                friendUserData.SetFriendUserData(pktInfoFriend.infos_[i]);

                FriendAskFromUserList.Add(friendUserData);
            }
        }
    }

    public void UpdateFriendAskFromUserList(PktInfoFriend pktInfoFriend)
    {
        if (null == pktInfoFriend)
            return;

        for(int i = 0; i < pktInfoFriend.infos_.Count; i++)
        {
            if (null == pktInfoFriend.infos_[i])
                continue;

            FriendUserData friendCheck = FriendAskFromUserList.Find(x => x.UUID == (long)pktInfoFriend.infos_[i].info_.uuid_);
            if(friendCheck == null)
            {
                FriendUserData friendUser = new FriendUserData();
                friendUser.SetFriendUserData(pktInfoFriend.infos_[i]);

                FriendAskFromUserList.Add(friendUser);
            }
        }
    }

    

    public void SetFriendSuggestList(PktInfoCommuSuggestAck pktInfoCommuSuggestAck)
    {
        FriendSuggestList.Clear();
        if (null == pktInfoCommuSuggestAck.suggest_)
            return;

        for(int i = 0; i < pktInfoCommuSuggestAck.suggest_.infos_.Count; i++)
        {
            if (null == pktInfoCommuSuggestAck.suggest_.infos_[i])
                continue;

            if (!AddFriendSuggestListCheck((long)pktInfoCommuSuggestAck.suggest_.infos_[i].uuid_))
                continue;

            FriendUserData friendUserData = new FriendUserData();
            friendUserData.SetFriendUserData(pktInfoCommuSuggestAck.suggest_.infos_[i]);

            FriendSuggestList.Add(friendUserData);
        }
    }

    //추천 목록리스트에 추가된 UUID가 다른 리스트에 있으면 스킵
    private bool AddFriendSuggestListCheck(long addFriendUUID)
    {
        bool result = true;

        FriendUserData friendUser = FriendList.Find(x => x.UUID == addFriendUUID);
        if (friendUser != null)
            return false;

        friendUser = FriendToAskList.Find(x => x.UUID == addFriendUUID);
        if (friendUser != null)
            return false;

        friendUser = FriendAskFromUserList.Find(x => x.UUID == addFriendUUID);
        if (friendUser != null)
            return false;

        return result;
    }
}

//월정액
public partial class MonthlyData
{
    public void SetMonthlyData(PktInfoMonthlyFee.Piece pktInfoMonthlyFeePiece)
    {
        MonthlyEndTime = GameSupport.GetLocalTimeByServerTime(pktInfoMonthlyFeePiece.endTM_.GetTime());
        MonthlyTableID = pktInfoMonthlyFeePiece.tableID_;
    }
}

public partial class UserMonthlyData
{
    public void SetUserMonthlyData(PktInfoMonthlyFee pktInfoMonthlyFee)
    {
        if (MonthlyDataList == null)
            MonthlyDataList = new List<MonthlyData>();

        MonthlyDataList.Clear();

        if (pktInfoMonthlyFee == null)
        {
            Debug.LogError("PktInfoMonthlyFee is NULL");
            return;
        }

        if (pktInfoMonthlyFee.infos_ == null)
        {
            Debug.LogError("PktInfoMonthlyFee.infos_ is NULL");
            return;
        }


        for (int i = 0; i < pktInfoMonthlyFee.infos_.Count; i++)
        {
            MonthlyData monthlyData = new MonthlyData();
            monthlyData.SetMonthlyData(pktInfoMonthlyFee.infos_[i]);

            MonthlyDataList.Add(monthlyData);
        }
    }

    public void UpdateUserMonthlyData(PktInfoMonthlyFee pktInfoMonthlyFee)
    {
        if (MonthlyDataList == null)
            MonthlyDataList = new List<MonthlyData>();

        for (int i = 0; i < pktInfoMonthlyFee.infos_.Count; i++)
        {
            MonthlyData monthlyData = MonthlyDataList.Find(x => x.MonthlyTableID == pktInfoMonthlyFee.infos_[i].tableID_);
            if (monthlyData == null)
            {
                monthlyData = new MonthlyData();
                monthlyData.SetMonthlyData(pktInfoMonthlyFee.infos_[i]);
                MonthlyDataList.Add(monthlyData);
            }
            else
            {
                monthlyData.SetMonthlyData(pktInfoMonthlyFee.infos_[i]);
            }
        }
    }

    public MonthlyData GetMonthlyDataWithStoreID(int tableID)
    {
        return MonthlyDataList.Find(x => x.MonthlyTableID == tableID);
    }
}

//유저버프
public partial class BuffEffectData
{
    public void SetBuffEffectData(PktInfoEffect.Buff pktInfoEffectBuff)
    {
        BuffEndTime = GameSupport.GetLocalTimeByServerTime(pktInfoEffectBuff.endTM_.GetTime());
        BuffTableID = pktInfoEffectBuff.tableID_;

        TableData = GameInfo.Instance.GameTable.FindBuff(x => x.ID == BuffTableID);

        BuffEffectType = (eBuffEffectType)Enum.Parse(typeof(eBuffEffectType), TableData.Type);
    }
    
    public void SetBuffEffectData(PktInfoEffect.BuffP pktInfoEffectBuff)
    {
        BuffEndTime = DateTime.MaxValue;
        BuffTableID = pktInfoEffectBuff.tableID_;

        TableData = GameInfo.Instance.GameTable.FindBuff(x => x.ID == BuffTableID);

        BuffEffectType = (eBuffEffectType)Enum.Parse(typeof(eBuffEffectType), TableData.Type);
    }
}

public partial class UserBuffEffectData
{
    public void SetUserBuffEffectData(PktInfoEffect pktInfoEffect)
    {
        if (BuffEffectDataList == null)
            BuffEffectDataList = new List<BuffEffectData>();

        BuffEffectDataList.Clear();

        if (pktInfoEffect == null)
        {
            Debug.LogError("PktInfoEffect is NULL");
            return;
        }

        if (pktInfoEffect.bufs_ == null)
        {
            Debug.LogError("PktInfoEffect.infos_ is NULL");
            return;
        }

        for (int i = 0; i < pktInfoEffect.bufs_.Count; i++)
        {
            BuffEffectData buffEffectData = new BuffEffectData();
            buffEffectData.SetBuffEffectData(pktInfoEffect.bufs_[i]);

            BuffEffectDataList.Add(buffEffectData);
        }

        if (pktInfoEffect.bufSs_ == null)
        {
            Debug.LogError("PktInfoEffect.bufSs_ is NULL");
            return;
        }

        for (int i = 0; i < pktInfoEffect.bufSs_.Count; i++)
        {
            BuffEffectData buffEffectData = new BuffEffectData();
            buffEffectData.SetBuffEffectData(pktInfoEffect.bufSs_[i]);

            BuffEffectDataList.Add(buffEffectData);
        }
    }

    public void UpdateUserBuffEffectData(PktInfoEffect pktInfoEffect)
    {
        if (BuffEffectDataList == null)
            BuffEffectDataList = new List<BuffEffectData>();

        for (int i = 0; i < pktInfoEffect.bufs_.Count; i++)
        {
            BuffEffectData buffEffectData = BuffEffectDataList.Find(x => x.BuffTableID == pktInfoEffect.bufs_[i].tableID_);
            if (buffEffectData == null)
            {
                buffEffectData = new BuffEffectData();
                buffEffectData.SetBuffEffectData(pktInfoEffect.bufs_[i]);

                BuffEffectDataList.Add(buffEffectData);
            }
            else
            {
                buffEffectData.SetBuffEffectData(pktInfoEffect.bufs_[i]);
            }
        }
        
        for (int i = 0; i < pktInfoEffect.bufSs_.Count; i++)
        {
            BuffEffectData buffEffectData = BuffEffectDataList.Find(x => x.BuffTableID == pktInfoEffect.bufSs_[i].tableID_);
            if (buffEffectData == null)
            {
                buffEffectData = new BuffEffectData();
                buffEffectData.SetBuffEffectData(pktInfoEffect.bufSs_[i]);

                BuffEffectDataList.Add(buffEffectData);
            }
            else
            {
                buffEffectData.SetBuffEffectData(pktInfoEffect.bufSs_[i]);
            }
        }
    }
}

//무기고
public partial class WeaponArmoryData
{
    public void SetWeaponArmoryData(PktInfoWpnDepotSet pktInfo)
    {
        ArmorySlotCnt = pktInfo.value_.maxCnt_;
        ArmoryWeaponUIDList.Clear();

        for (int i = 0; i < pktInfo.infos_.Count; i++)
        {
            if(pktInfo.infos_[i].wnpUID_ > (int)eCOUNT.NONE)
                ArmoryWeaponUIDList.Add((long)pktInfo.infos_[i].wnpUID_);
        }
    }

    public void SetWeaponArmorySlot(PktInfoWpnDepotApply pktInfo)
    {
        ArmoryWeaponUIDList.Clear();
        for (int i = 0; i < pktInfo.slots_.Count; i++)
        {
            if (pktInfo.slots_[i].wnpUID_ > (int)eCOUNT.NONE)
                ArmoryWeaponUIDList.Add((long)pktInfo.slots_[i].wnpUID_);
        }
    }
}

public partial class AwakenSkillInfo
{
    public void SetPktData(int id, int level)
    {
        TableId = id;
        Level = level;
    }
}

public enum ePacketType
{
    None = 0,
    Global,
    Login,
    Lobby,
    Battle,
    Product,
}
public class ProtocolData
{
    public ePacketType PacketType;
    public Nettention.Proud.RmiID PacketID;
    public PktMsgType PktMsgData = null;
    public long PktNumData = 0;
    public OnReceiveCallBack ReceiveCallBack;
    public ProtocolData()
    {

    }

    public ProtocolData(ePacketType type, Nettention.Proud.RmiID id, PktMsgType pktdata, OnReceiveCallBack callback)
    {
        PacketType = type;
        PacketID = id;
        PktMsgData = pktdata;
        PktNumData = -1;
        ReceiveCallBack = callback;
    }
    public ProtocolData(ePacketType type, Nettention.Proud.RmiID id, long pktnum, OnReceiveCallBack callback)
    {
        PacketType = type;
        PacketID = id;
        PktMsgData = null;
        PktNumData = pktnum;
        ReceiveCallBack = callback;
    }
}


public partial class InfluenceMissionData
{
    //  서버 데이터 셋팅
    public void SetPktData(PktInfoMission.Influ pktInflu)
    {
        GroupID = pktInflu.groupID_;
        InfluID = pktInflu.influID_;
        RwdFlag = pktInflu.rwdFlag_;
        TgtRwdFlag = pktInflu.tgtRwdFlag_;
        pktInflu.val_.CopyTo(Val, 0);
    }
}

public partial class InfluenceData
{
    public void SetPktData(PktInfoInfluence pktInfo)
    {
        TotalPoint = pktInfo.idInRankTbl_;
        Infos.Clear();
        for (int i = 0; i < pktInfo.infos_.Count; i++)
        {
            Infos.Add(new Piece(pktInfo.infos_[i]));
        }
    }
}

public partial class InfluenceRankData
{
    public void SetPktData(PktInfoRankInfluence pktInfo)
    {
        if (LaseUpdateTime == null) new PktInfoTime();
        if (LaseUpdateTime.time_ == pktInfo.lastUpTM_.time_) return;

        LaseUpdateTime.time_ = pktInfo.lastUpTM_.time_;

        IdInRankTable = pktInfo.idInRankTbl_;
        NowRank = pktInfo.nowRank_;

        Infos.Clear();
        for (int i = 0; i < pktInfo.infos_.Count; i++)
        {
            Infos.Add(new Piece(pktInfo.infos_[i]));
        }

        Debug.Log(string.Format("LaseUpdateTime.time_ {0}, IdInRankTable {1}, NowRank {2}, Count {3}", LaseUpdateTime.time_, IdInRankTable, NowRank, Infos.Count));
    }
}

public partial class RotationGachaData
{
    public void SetPktData(PktInfoComTimeAndTID pktInfo)
    {
        Infos.Clear();
        for (int i = 0; i < pktInfo.infos_.Count; i++)
        {
            Infos.Add(new Piece(pktInfo.infos_[i]));
        }

        ResetPktData();
    }
    public void ResetPktData()
    {
        for (int i = 0; i < Infos.Count; i++)
        {
            System.TimeSpan t = GameSupport.GetRemainTime(Infos[i].Time, GameInfo.Instance.GetNetworkTime());
            if (t.TotalSeconds <= (int)eCOUNT.NONE)
            {
                Infos.RemoveAt(i);
                i--;
            }
        }
    }

    public void UpdatePktData(PktInfoComTimeAndTID pktInfo)
    {
        

        for (int i = 0; i < pktInfo.infos_.Count; i++)
        {
            bool update = false;
            for (int j = 0; j < Infos.Count; j++)
            {
                if (Infos[j].RemainCount == (int)pktInfo.infos_[i].tid_)
                {
                    Infos[j].UpdatePiece(pktInfo.infos_[i]);
                    update = true;
                    break;
                }
            }

            if (!update)
            {
                Infos.Add(new Piece(pktInfo.infos_[i]));
            }
        }
    }
}

public partial class RaidUserData {
    public void SetPktData( PktInfoUserRaid pkt ) {
        // 팀 정보
        SetPktTeamInfo( pkt.team_ );

        // 모든 시즌 기록 데이터
        StageClearList.Clear();
        for( int i = 0; i < pkt.record_.infos_.Count; i++ ) {
            StageClearList.Add( new RaidClearData( pkt.record_.infos_[i] ) );
        }

        // 현재 시즌 정보
        LastPlayedSeasonEndTime = GameSupport.GetLocalTimeByServerTime( pkt.seasonData_.lastPlaySeasonEndTime_.GetTime() );
        SetCurStep( pkt.seasonData_.OpenLevel_ );

        DailyRaidPoint = pkt.dailyLimitPoint_;
    }

    public void SetPktTeamInfo( PktInfoUserRaidTeam pkt ) {
        CharUidList.Clear();

        for( int i = 0; i < pkt.CUIDs_.Length; i++ ) {
            CharUidList.Add( (long)pkt.CUIDs_[i] );
        }

        CardFormationId = (int)pkt.cardFrmtID_;
    }

    public void SetCurStep( int step ) {
        if( step <= 0 ) {
            return;
		}

        CurStep = step;
        CurStageParam = GameInfo.Instance.GameTable.FindStage( x => x.StageType == (int)eSTAGETYPE.STAGE_RAID && 
                                                                    x.TypeValue == GameInfo.Instance.ServerData.RaidCurrentSeason );
	}
}

public partial class RaidClearData {
    public void SetPktData( PktInfoRaidStageRec.Piece stagerInfopiece_ ) {
        //TODO 황소현
        StageTableID = (int)stagerInfopiece_.stageTID_;
        Step = (int)stagerInfopiece_.level_;
        HighestScore = stagerInfopiece_.timeRecord_Ms_;
        Cuid = (long)stagerInfopiece_.charUID_;
        TableData = GameInfo.Instance.GameTable.FindStage( StageTableID );
    }
}

public partial class RaidRankData {
    public void SetPktData( PktInfoRaidRankingHeader.Piece raidRankingHeaderPiece ) {
        StageTableID = (int)raidRankingHeaderPiece.stageID_;
        UpdateTM = (long)raidRankingHeaderPiece.updateTM_;
        Step = (int)raidRankingHeaderPiece.raidLevel_;
        StageTableData = GameInfo.Instance.GameTable.FindStage( StageTableID );
    }
}

public partial class CircleData
{
    public void SetPktInfoCircleSimple(PktInfoCircleSimple.Piece piece)
    {
        Uid = (long)piece.ID_;
        Rank = piece.rank_;
        Name = piece.name_.str_;
        Content = piece.comment_.str_;
        FlagId = (int)piece.markSet_.flagTID_;
        MarkId = (int)piece.markSet_.markTID_;
        ColorId = (int)piece.markSet_.colorTID_;
        MemberCount = piece.memCount_;
        MemberMaxCount = piece.maxMemCount_;
        MainLanguage = piece.lang_;
        IsOtherLanguage = piece.suggestAnotherLang_;

        Leader.Uid = (long)piece.leaderID_;
        Leader.Name = piece.leaderInfo_.nickName_.str_;
        Leader.LastLoginTime = piece.leaderInfo_.lastConnTM_.GetTime();
        Leader.MarkId = (int)piece.leaderInfo_.mark_;
        Leader.Rank = piece.leaderInfo_.rank_;
    }

    public void SetPktInfoCircle(PktInfoCircle info)
    {
        SetGoods(info.goods_);

        LobbySetId = (int)info.lobbySet_;
        AttendanceCount = info.attendenceCnt_;
        RecentAttendRewardTime = info.recentAttendRwdDate_.GetTime();
        NextUserKickPossibleTime = info.nextUserKickPossibleTime_.GetTime();
        SubLeaderCount = info.subLeaderCnt_;
        SubLeaderMaxCount = info.maxSubLeaderCnt_;
    }

    public void SetPktInfoCircleMark(PktInfoCircleMark pkt)
    {
        for (int i = 0; i < pkt.ownedFlag_.Length; i++)
        {
            if (FlagBit.Length <= i)
            {
                break;
            }
            FlagBit[i] = (long)pkt.ownedFlag_[i];
        }

        for (int i = 0; i < pkt.ownedMark_.Length; i++)
        {
            if (MarkBit.Length <= i)
            {
                break;
            }
            MarkBit[i] = (long)pkt.ownedMark_[i];
        }

        for (int i = 0; i < pkt.ownedColor_.Length; i++)
        {
            if (ColorBit.Length <= i)
            {
                break;
            }
            ColorBit[i] = (long)pkt.ownedColor_[i];
        }
    }
}

public partial class CircleChatData
{
    public void SetPktInfoCircleChat(PktInfoCircleChat.Piece piece)
    {
        Uid = (long)piece.uuid_;
        UserName = piece.nickName_.str_;
        Content = piece.msg_.str_;
        UserMarkId = (int)piece.mark_;
        StampId = (int)piece.stampID_;
        ChatTime = piece.sendTm_.GetTime();
    }
}

public partial class CircleNotiData
{
    public void SetPktInfoCircleNotification(PktInfoCircleNotification.Piece piece)
    {
        Values.Clear();
        foreach (ulong value in piece.values_)
        {
            _valueList.Add((long)value);
        }

        NotiType = piece.notiTp_;
        NickName = piece.nickName_.str_;
        SendTime = piece.sendTm_.GetTime();
    }
}