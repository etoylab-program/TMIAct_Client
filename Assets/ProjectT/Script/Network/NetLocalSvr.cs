using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetLocalSvr : FSingleton<NetLocalSvr>
{
    private int _testcharuid = 100000;
    private int _testitemuid = 200000;

    public void Regist()
    {
        GameConfig _gameconfig = GameInfo.Instance.GameConfig;
        BattleConfig _battleconfig = GameInfo.Instance.BattleConfig;
        GameTable _gametable = GameInfo.Instance.GameTable;
        GameClientTable _gameclienttable = GameInfo.Instance.GameClientTable;

        ServerData _serverdata = GameInfo.Instance.ServerData;
        UserData _userdata = GameInfo.Instance.UserData;
        List<AchieveData> _achievelist = GameInfo.Instance.AchieveList;
        List<StoreData> _storelist = GameInfo.Instance.StoreList;
        List<CharData> _charlist = GameInfo.Instance.CharList;
        List<WeaponData> _weaponlist = GameInfo.Instance.WeaponList;
        List<GemData> _gemlist = GameInfo.Instance.GemList;
        List<CardData> _cardlist = GameInfo.Instance.CardList;
        List<ItemData> _itemlist = GameInfo.Instance.ItemList;
        List<FacilityData> _facilitylist = GameInfo.Instance.FacilityList;
        List<StageClearData> _stageclearlist = GameInfo.Instance.StageClearList;
        List<WeaponBookData> _weaponbooklist = GameInfo.Instance.WeaponBookList;
        List<CardBookData> _cardbooklist = GameInfo.Instance.CardBookList;
        List<MonsterBookData> _monsterbooklist = GameInfo.Instance.MonsterBookList;
        List<TimeAttackClearData> _timeattackclearlist = GameInfo.Instance.TimeAttackClearList;
        List<TimeAttackRankData> _timeattackranklist = GameInfo.Instance.TimeAttackRankList;
        List<int> _costumelist = GameInfo.Instance.CostumeList;
        List<int> _roomthemelist = GameInfo.Instance.RoomThemaList;
        List<RoomThemeSlotData> _roomthemeslotlist = GameInfo.Instance.RoomThemeSlotList;

        List<RewardData> RewardList = GameInfo.Instance.RewardList;
        GameResultData GameResultData = GameInfo.Instance.GameResultData;
        FacilityResultData FacilityResultData = GameInfo.Instance.FacilityResultData;

        WeekMissionData WeeklyMisstionData = GameInfo.Instance.WeekMissionData;

        List<EventSetData> _eventSetDatsList = GameInfo.Instance.EventSetDataList;

        string strUniqueIdentifier = "";
#if UNITY_IOS
          strUniqueIdentifier = UnityEngine.iOS.Device.vendorIdentifier;
#elif UNITY_ANDROID
        strUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
#elif UNITY_EDITOR
        strUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
#endif
        _userdata.SetNickName( _gameconfig.InitAccountNickName );
        _userdata.Level = 1;
        _userdata.Exp = 0;
        _userdata.Goods[(int)eGOODSTYPE.GOLD] = 999999999;//_gameconfig.InitGold;
        _userdata.Goods[(int)eGOODSTYPE.CASH] = 999999999;//_gameconfig.InitCash;
        _userdata.Goods[(int)eGOODSTYPE.SUPPORTERPOINT] = 99999;//_gameconfig.InitSupporterPoint;
        _userdata.Goods[(int)eGOODSTYPE.ROOMPOINT] = 99999;//_gameconfig.InitRoomPoint;
        _userdata.Goods[(int)eGOODSTYPE.AP] = GameSupport.GetMaxAP();
        _userdata.Goods[(int)eGOODSTYPE.BP] = _gameconfig.BPMaxCount;
        _userdata.APRemainTime = GameSupport.GetCurrentServerTime();
        _userdata.BPRemainTime = GameSupport.GetCurrentServerTime();
        _userdata.ItemSlotCnt = _gameconfig.BaseItemSlotCount;

        for (int i = 0; i < _gametable.Facilitys.Count; i++)
            _facilitylist.Add(new FacilityData(_gametable.Facilitys[i].ID, 0, GameSupport.GetCurrentServerTime()));

        for (int i = 0; i < _gametable.EventSets.Count; i++)
            _eventSetDatsList.Add(new EventSetData(_gametable.EventSets[i].EventID, 1, 0));

        //for (int i = 0; i < _gametable.EventResetRewards.Count; i++)
        //    _eventResetRewardDataList.Add(new EventResetRewardData(_gametable.EventResetRewards[i].EventID, _gametable.EventResetRewards[i].RewardStep));

        List<GameTable.EventResetReward.Param> tempeventresetrewarddata = new List<GameTable.EventResetReward.Param>();
        int maxstep = 0;
        for (int i = 0; i < _gametable.EventResetRewards.Count; i++)
        {
            if (maxstep < _gametable.EventResetRewards[i].RewardStep)
                maxstep = _gametable.EventResetRewards[i].RewardStep;
        }

        for (int i = 1; i <= maxstep; i++)
            tempeventresetrewarddata.Add(_gametable.EventResetRewards.Find(x => x.RewardStep == i));


        //룸테마 지급
        AddRoomTheme(GameInfo.Instance.GameConfig.InitRoomID);
        _roomthemeslotlist.Add(new RoomThemeSlotData(0, _roomthemelist[0]));
        //룸테마 기본셋팅
        _userdata.RoomThemeSlot = _roomthemeslotlist[0].SlotNum;

        //초기 캐릭터 지급
        //AddChar(1);
        //AddChar(2);
        //AddChar(3);

        //메인캐릭터 셋팅
        //_userdata.MainCharUID = _charlist[0].CUID;

        var list = _gametable.FindAllAchieve(x => x.GroupOrder == 1);
        for (int i = 0; i < list.Count; i++)
            _achievelist.Add(new AchieveData(list[i].GroupID, list[i].GroupOrder, 0));

        //무기 지급
        for (int i = 0; i < GameInfo.Instance.GameConfig.InitWeaponList.Count; i++)
            AddWeapon(GameInfo.Instance.GameConfig.InitWeaponList[i]);

        //곡옥 지급
        for (int i = 0; i < GameInfo.Instance.GameConfig.InitGemList.Count; i++)
            AddGem(GameInfo.Instance.GameConfig.InitGemList[i]);

        //서포터 지급
        for (int i = 0; i < GameInfo.Instance.GameConfig.InitCardList.Count; i++)
            AddCard(GameInfo.Instance.GameConfig.InitCardList[i]);

        //아이템 지급
        for (int i = 0; i < GameInfo.Instance.GameConfig.InitItemCountList.Count; i++)
            if (GameInfo.Instance.GameConfig.InitItemCountList[i] != 0)
                AddItem(GameInfo.Instance.GameConfig.InitItemList[i], GameInfo.Instance.GameConfig.InitItemCountList[i]);

        //코스튬 지급
        //for (int i = 0; i < GameInfo.Instance.GameConfig.InitCostumeList.Count; i++)
        for ( int i = 0; i < GameInfo.Instance.GameTable.Costumes.Count; i++ ) {
            AddCostume( GameInfo.Instance.GameTable.Costumes[i].ID );
        }

        //피규어 지급
        for (int i = 0; i < GameInfo.Instance.GameConfig.InitFigureList.Count; i++)
        {
            AddRoomFigure(GameInfo.Instance.GameConfig.InitFigureList[i]);
        }

        //피규어 지급
        for (int i = 0; i < GameInfo.Instance.GameConfig.InitUserMarkList.Count; i++)
        {
            AddUserMark(GameInfo.Instance.GameConfig.InitUserMarkList[i]);
        }

        //AddUserMarkFunc(1)
        //테스트 모드 초기셋팅
        //if (_gameconfig.TestMode) // 테스트모드 여부랑 상관없이 그냥 다 지급하는게 나을듯
        {
            _userdata.Level = _gameconfig.TestLevel;
            if (_userdata.Level != 1)
                _userdata.Exp = _gametable.FindLevelUp(x => x.Group == GameInfo.Instance.GameConfig.AccountLevelUpGroup && x.Level == _userdata.Level - 1).Exp;

            for (int i = 0; i < _charlist.Count; i++)
            {
                _charlist[i].Level = _gameconfig.TestCharLevel;
                if (_charlist[i].Level != 1)
                    _charlist[i].Exp = _gametable.FindLevelUp(x => x.Group == GameInfo.Instance.GameConfig.CharLevelUpGroup && x.Level == _charlist[0].Level - 1).Exp;
            }

            var tblData = _gameclienttable.FindAllBook(x => x.Group == (int)eBookGroup.Monster);

            for (int i = 0; i < tblData.Count; i++)
                _monsterbooklist.Add(new MonsterBookData(tblData[i].ItemID, 0));

            int count = 1;
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < _gametable.Cards.Count; j++)
                    AddCard(_gametable.Cards[j].ID);

                for (int j = 0; j < _gametable.Weapons.Count; j++)
                    AddWeapon(_gametable.Weapons[j].ID);

                for (int j = 0; j < _gametable.Items.Count; j++)
                    AddItem(_gametable.Items[j].ID, 1000);

                AddGem(111);
                AddGem(122);
                AddGem(133);
                AddGem(144);
                AddGem(211);
                AddGem(222);
                AddGem(233);
                AddGem(244);
                AddGem(311);
                AddGem(322);
                AddGem(333);
                AddGem(344);
            }
        }

        var rlist = _gametable.FindAllRandom(x => x.GroupID == _gameconfig.InitTutorialItemGroupID);
        for (int i = 0; i < rlist.Count; i++)
        {
            var rdata = rlist[i];
            AddMail(eMailType.InitReward, "aaaa", rdata.ProductType, rdata.ProductIndex, rdata.ProductValue);
        }

        //  로그인보너스 기본 셋팅
        _userdata.LoginBonusGroupID = 1;    //  신규 로그인 그룹 ID 1
        _userdata.LoginBonusGroupCnt = 0;   //  로그인 횟수 0
        _userdata.LoginBonusRecentDate = GameSupport.GetCurrentServerTime().AddDays(-1);  // 초기 시간(현재 시간을 기준으로 어제 시간)

        //서버 정보
        _serverdata.Version = 1;

        GameInfo.Instance.MailTotalCount = 0;

        var weeklyMisstionTable = GameInfo.Instance.GameTable.FindWeeklyMissionSet(1);
        if (WeeklyMisstionData != null)
        {
            WeeklyMisstionData.fWeekMissionSetID = 1;
            WeeklyMisstionData.fMissionRemainCntSlot[0] = (uint)weeklyMisstionTable.WMCnt0;
            WeeklyMisstionData.fMissionRemainCntSlot[1] = (uint)weeklyMisstionTable.WMCnt1;
            WeeklyMisstionData.fMissionRemainCntSlot[2] = (uint)weeklyMisstionTable.WMCnt2;
            WeeklyMisstionData.fMissionRemainCntSlot[3] = (uint)weeklyMisstionTable.WMCnt3;
            WeeklyMisstionData.fMissionRemainCntSlot[4] = (uint)weeklyMisstionTable.WMCnt4;
            WeeklyMisstionData.fMissionRemainCntSlot[5] = (uint)weeklyMisstionTable.WMCnt5;
            WeeklyMisstionData.fMissionRemainCntSlot[6] = (uint)weeklyMisstionTable.WMCnt6;
            WeeklyMisstionData.fWeekMissionResetDate = new DateTime();  // 초기 시간
        }

        //UIValue.Instance.SetValue(UIValue.EParamType.WeeklyMissionRefreshPossible, true);

        //_serverdata.FreeGachaList.Add(new FreeGachaData(5, 4*60));
        //_serverdata.GachaCategoryList.Add(new GachaCategoryData(0, true, "새해이벤트", "Gacha_0.png"));
        //_serverdata.GachaCategoryList.Add(new GachaCategoryData(1, true, "런칭이벤트", "Gacha_1.png"));

        //로컬은 그냥 폴더에서 읽는걸로...
        //가챠 정보
        _serverdata.GachaCategoryList.Add(new GachaCategoryData("PICKUP3", 0, "PickupGacha_Banner3.png", "PickupGacha_BigBanner3.png", 1000005, 1000006, 1000105, 1000106, GameSupport.GetCurrentServerTime(), GameSupport.GetCurrentServerTime().AddDays(2)));
        _serverdata.GachaCategoryList.Add(new GachaCategoryData("PICKUP2", 0, "PickupGacha_Banner2.png", "PickupGacha_BigBanner2.png", 1000003, 1000004, 1000103, 1000104, GameSupport.GetCurrentServerTime(), GameSupport.GetCurrentServerTime().AddDays(2)));
        _serverdata.GachaCategoryList.Add(new GachaCategoryData("PICKUP1", 0, "PickupGacha_Banner.png", "PickupGacha_BigBanner.png", 1000001, 1000002, 1000101, 1000102,GameSupport.GetCurrentServerTime(), GameSupport.GetCurrentServerTime().AddDays(2)));

        //배너 정보
        _serverdata.BannerList.Add(new BannerData("PickupGacha_Banner.png", "UIPANEL", "GACHA", "PICKUP1", "", GameSupport.GetCurrentServerTime(), GameSupport.GetCurrentServerTime().AddDays(1)));
        _serverdata.BannerList.Add(new BannerData("PickupGacha_Banner2.png", "UIPANEL", "GACHA", "PICKUP2", "", GameSupport.GetCurrentServerTime(), GameSupport.GetCurrentServerTime().AddDays(1)));
        _serverdata.BannerList.Add(new BannerData("PickupGacha_Banner3.png", "UIPANEL", "GACHA", "PICKUP3", "", GameSupport.GetCurrentServerTime(), GameSupport.GetCurrentServerTime().AddDays(1)));
        _serverdata.BannerList.Add(new BannerData("RollGachaBanner2.png", "UIPANEL", "GACHA", "CASH", "", GameSupport.GetCurrentServerTime(), GameSupport.GetCurrentServerTime().AddDays(1)));
        _serverdata.BannerList.Add(new BannerData("RollGachaBanner3.png", "UIPANEL", "GACHA", "GOLD", "", GameSupport.GetCurrentServerTime(), GameSupport.GetCurrentServerTime().AddDays(1)));

        DateTime StartTime = GameSupport.GetTimeWithString("2020021800");
        DateTime EndTime = GameSupport.GetTimeWithString("2021022423", true);

        _serverdata.GuerrillaMissionList.Add(new GuerrillaMissionData("GM_BuyStoreGacha_Cnt", 0, 0, 2000011, 100, 200218001, 1, 7, 20033, 1, StartTime, EndTime));
        _serverdata.GuerrillaMissionList.Add(new GuerrillaMissionData("GM_BuyStoreGacha_Cnt", 0, 0, 2000011, 150, 200218001, 2, 7, 20035, 1, StartTime, EndTime));
        _serverdata.GuerrillaMissionList.Add(new GuerrillaMissionData("GM_BuyStoreGacha_Cnt", 1, 1, 2000001, 100, 200218002, 1, 7, 20034, 1, StartTime, EndTime));
        _serverdata.GuerrillaMissionList.Add(new GuerrillaMissionData("GM_BuyStoreGacha_Cnt", 2, 2, 2000021, 100, 200218003, 1, 5, 59, 1, StartTime, EndTime));
        _serverdata.GuerrillaMissionList.Add(new GuerrillaMissionData("GM_MailReward", 3, 3, 0, 0, 200218005, 1, 1, 1, 10, StartTime, EndTime));

        //BannerManager.Instance.AddServerData(_serverdata);

        //세일 정보
        _serverdata.StoreSaleList.Add(new StoreSaleData(7, 100, 0, 1440));
        _serverdata.StoreSaleList.Add(new StoreSaleData(8, 25, 0, 1440));
        _serverdata.StoreSaleList.Add(new StoreSaleData(5, 50, 0, 1440));
        _serverdata.StoreSaleList.Add(new StoreSaleData(17, 0, 1, 1));
        _serverdata.StoreSaleList.Add(new StoreSaleData(18, 0, 1, 1));
        _serverdata.StoreSaleList.Add(new StoreSaleData(19, 0, 1, 1));
        _serverdata.StoreSaleList.Add(new StoreSaleData(20, 0, 1, 1));
        _serverdata.StoreSaleList.Add(new StoreSaleData(21, 0, 1, 1));
        _serverdata.StoreSaleList.Add(new StoreSaleData(22, 0, 1, 1));
        _serverdata.StoreSaleList.Add(new StoreSaleData(2001, 0, 1, 1));
        _serverdata.StoreSaleList.Add(new StoreSaleData(2002, 0, 1, 1));
        _serverdata.StoreSaleList.Add(new StoreSaleData(2003, 0, 1, 1));
        _serverdata.StoreSaleList.Add(new StoreSaleData(2004, 0, 1, 1));
        _serverdata.StoreSaleList.Add(new StoreSaleData(2005, 0, 1, 1));
        _serverdata.StoreSaleList.Add(new StoreSaleData(2006, 0, 1, 1));

        //무료가챠 정보

        /*
        AddTimeAttackClear(3001, 200999);
        AddTimeAttackClear(3002, 200998);
        AddTimeAttackClear(3003, 200997);
        AddTimeAttackClear(3101, 200996);
        */

        /*
        var timeattacklist = _gametable.FindAllStage(x => x.StageType == (int)eSTAGETYPE.STAGE_TIMEATTACK);
        for( int i = 0; i < timeattacklist.Count; i++)
        {
            TimeAttackRankData data = new TimeAttackRankData(timeattacklist[i].ID);
            _timeattackranklist.Add(data);
            for (int j = 0; j < 10; j++)
                AddTimeAttackRankUser(timeattacklist[i].ID, 2009968 - (j * 10), GameInfo.Instance.CharList[0].CUID );
        }
        */
        /*
        //타임어택
        TimeAttackClearData data = new TimeAttackClearData()
        _timeattackclearlist.Add();
        List<TimeAttackClearData> _timeattackclearlist = GameInfo.Instance.TimeAttackClearList;
        List<TimeAttackRankData> _timeattackranklist = GameInfo.Instance.TimeAttackRankList;
        */
        PlayerPrefs.SetInt("NetLocalSvr", 1);
    }


    //로그인
    public void Login()
    {
        ServerData _serverdata = GameInfo.Instance.ServerData;
        GameTable _gametable = GameInfo.Instance.GameTable;
        UserData _userdata = GameInfo.Instance.UserData;

        // 티켓 최대치
        _userdata.Goods[(int)eGOODSTYPE.AP] = GameSupport.GetMaxAP();

        _serverdata.LoginTime = GameSupport.GetCurrentServerTime();
        _serverdata.DayRemainTime = GameSupport.GetCurrentServerTime().AddDays(1);

        //_serverdata.GuerrillaCampList.Clear();
        //_serverdata.GuerrillaCampList.Add(new GuerrillaCampData("GC_StageClear_ExpRateUP", (int)eSTAGETYPE.STAGE_MAIN_STORY, 1000, "경험치 획득량 증가", "메인스토리 클리어시", DateTime.Now, DateTime.Now.AddHours(2)));
        //_serverdata.GuerrillaCampList.Add(new GuerrillaCampData("GC_StageClear_GoldRateUP", (int)eSTAGETYPE.STAGE_DAILY, 1000, "골드 획득량 증가", "요일던전 클리어시", DateTime.Now, DateTime.Now.AddHours(2)));
        //_serverdata.GuerrillaCampList.Add(new GuerrillaCampData("GC_StageClear_ItemCntUP", (int)eSTAGETYPE.STAGE_EVENT, 5, " 아이템 획득수 증가", "이벤트모드 클리어시", DateTime.Now, DateTime.Now.AddHours(2)));
        //_serverdata.GuerrillaCampList.Add(new GuerrillaCampData("GC_StageClear_APRateDown", (int)eSTAGETYPE.STAGE_PVP_PROLOGUE, 50, "AP 소모량 감소", "프롤로그 클리어시", DateTime.Now, DateTime.Now.AddHours(2)));
        //_serverdata.GuerrillaCampList.Add(new GuerrillaCampData("GC_StageClear_FavorRateUP", (int)eSTAGETYPE.STAGE_TIMEATTACK, 1000, "서포터 호감도 획득량 증가", "타임어택 클리어시", DateTime.Now, DateTime.Now.AddHours(2)));
        //_serverdata.GuerrillaCampList.Add(new GuerrillaCampData("GC_Upgrade_ExpRateUP", (int)eContentsPosKind.CARD, 100, "강화 시 경험치 획득량 증가", "서포터 강화시", DateTime.Now, DateTime.Now.AddHours(2)));
        //_serverdata.GuerrillaCampList.Add(new GuerrillaCampData("GC_Upgrade_PriceRateDown", (int)eContentsPosKind.GEM, 100, "강화 시 골드 소모량 감소", "곡옥 강화시", DateTime.Now, DateTime.Now.AddHours(2)));
        //_serverdata.GuerrillaCampList.Add(new GuerrillaCampData("GC_Upgrade_SucNorRateDown", 0, 90, "강화 성공 확률 감소", "강화 시", DateTime.Now, DateTime.Now.AddHours(2)));
        //_serverdata.GuerrillaCampList.Add(new GuerrillaCampData("GC_ItemSell_PriceRateUp", 0, 1000, "골드 획득량 증가", "아이템 판매 시", DateTime.Now, DateTime.Now.AddHours(2)));
        //_serverdata.GuerrillaCampList.Add(new GuerrillaCampData("GC_Arena_CoinRateUP", 0, 1000, "아레나 코인 획득량 증가", "배틀 아레나 시", DateTime.Now, DateTime.Now.AddHours(2)));
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  저장 데이타
    //
    //----------------------------------------------------------------------------------------------------------------------------------------------------
    public void SaveLocalData()
    {
        GameInfo.Instance.ClearProtocolData();

        ServerData _serverdata = GameInfo.Instance.ServerData;
        UserData _userdata = GameInfo.Instance.UserData;
        List<StoreData> _storelist = GameInfo.Instance.StoreList;
        List<AchieveData> _achievelist = GameInfo.Instance.AchieveList;
        List<CharData> _charlist = GameInfo.Instance.CharList;
        List<WeaponData> _weaponlist = GameInfo.Instance.WeaponList;
        List<GemData> _gemlist = GameInfo.Instance.GemList;
        List<CardData> _cardlist = GameInfo.Instance.CardList;
        List<ItemData> _itemlist = GameInfo.Instance.ItemList;
        List<FacilityData> _facilitylist = GameInfo.Instance.FacilityList;
        List<StageClearData> _stageclearlist = GameInfo.Instance.StageClearList;
        List<WeaponBookData> _weaponbooklist = GameInfo.Instance.WeaponBookList;
        List<CardBookData> _cardbooklist = GameInfo.Instance.CardBookList;
        List<MonsterBookData> _monsterbooklist = GameInfo.Instance.MonsterBookList;
        List<TimeAttackClearData> _timeattackclearlist = GameInfo.Instance.TimeAttackClearList;
        List<TimeAttackRankData> _timeattackranklist = GameInfo.Instance.TimeAttackRankList;
        List<int> _costumelist = GameInfo.Instance.CostumeList;
        List<int> _roomactionlist = GameInfo.Instance.RoomActionList;
        List<int> _roomthemalist = GameInfo.Instance.RoomThemaList;
        List<int> _roomfigurelist = GameInfo.Instance.RoomFigureList;
        List<int> _roomfunclist = GameInfo.Instance.RoomFuncList;
        List<int> _usermarklist = GameInfo.Instance.UserMarkList;
        List<RoomThemeSlotData> _roomthemeslotlist = GameInfo.Instance.RoomThemeSlotList;
        List<RoomThemeFigureSlotData> _roomthemefigureslotlist = GameInfo.Instance.RoomThemeFigureSlotList;
        List<MailData> _mailList = GameInfo.Instance.MailList;
        WeekMissionData _weeklyMisstionData = GameInfo.Instance.WeekMissionData;

        List<EventSetData> _eventSetDatList = GameInfo.Instance.EventSetDataList;


        PlayerPrefs.SetInt("TestCharUID", _testcharuid);
        PlayerPrefs.SetInt("TestItemUID", _testitemuid);

        _serverdata.SaveData();
        _userdata.SaveData();
        _weeklyMisstionData.SaveData();
        PlayerPrefs.SetInt("Store_Count", _storelist.Count);
        PlayerPrefs.SetInt("AchieveList_Count", _achievelist.Count);
        PlayerPrefs.SetInt("CharList_Count", _charlist.Count);
        PlayerPrefs.SetInt("WeaponList_Count", _weaponlist.Count);
        PlayerPrefs.SetInt("GemList_Count", _gemlist.Count);
        PlayerPrefs.SetInt("CardList_Count", _cardlist.Count);
        PlayerPrefs.SetInt("ItemList_Count", _itemlist.Count);
        PlayerPrefs.SetInt("CostumeList_Count", _costumelist.Count);
        PlayerPrefs.SetInt("FacilityList_Count", _facilitylist.Count);
        PlayerPrefs.SetInt("StageClearList_Count", _stageclearlist.Count);
        PlayerPrefs.SetInt("WeaponBookList_Count", _weaponbooklist.Count);
        PlayerPrefs.SetInt("CardBookList_Count", _cardbooklist.Count);
        PlayerPrefs.SetInt("MonsterBookList_Count", _monsterbooklist.Count);
        PlayerPrefs.SetInt("TimeAttackClearList_Count", _timeattackclearlist.Count);
        PlayerPrefs.SetInt("TimeAttackRankList_Count", _timeattackranklist.Count);
        PlayerPrefs.SetInt("RoomActionList_Count", _roomactionlist.Count);
        PlayerPrefs.SetInt("RoomThemaList_Count", _roomthemalist.Count);
        PlayerPrefs.SetInt("RoomFigureList_Count", _roomfigurelist.Count);
        PlayerPrefs.SetInt("RoomFuncList_Count", _roomfunclist.Count);
        PlayerPrefs.SetInt("RoomUserMark_Count", _usermarklist.Count);
        PlayerPrefs.SetInt("RoomThemeSlotList_Count", _roomthemeslotlist.Count);
        PlayerPrefs.SetInt("RoomThemeFigureSlotList_Count", _roomthemefigureslotlist.Count);
        PlayerPrefs.SetInt("MailList_Count", _mailList.Count);
        PlayerPrefs.SetInt("EventSetDataList_Count", _eventSetDatList.Count);

        for (int i = 0; i < _storelist.Count; i++)
            _storelist[i].SaveData(i);
        for (int i = 0; i < _achievelist.Count; i++)
            _achievelist[i].SaveData(i);
        for (int i = 0; i < _charlist.Count; i++)
            _charlist[i].SaveData(i);
        for (int i = 0; i < _weaponlist.Count; i++)
            _weaponlist[i].SaveData(i);
        for (int i = 0; i < _gemlist.Count; i++)
            _gemlist[i].SaveData(i);
        for (int i = 0; i < _cardlist.Count; i++)
            _cardlist[i].SaveData(i);
        for (int i = 0; i < _itemlist.Count; i++)
            _itemlist[i].SaveData(i);
        for (int i = 0; i < _facilitylist.Count; i++)
            _facilitylist[i].SaveData(i);
        for (int i = 0; i < _stageclearlist.Count; i++)
            _stageclearlist[i].SaveData(i);
        for (int i = 0; i < _costumelist.Count; i++)
            PlayerPrefs.SetInt("CostumeList_" + i.ToString(), _costumelist[i]);
        for (int i = 0; i < _weaponbooklist.Count; i++)
            _weaponbooklist[i].SaveData(i);
        for (int i = 0; i < _cardbooklist.Count; i++)
            _cardbooklist[i].SaveData(i);
        for (int i = 0; i < _monsterbooklist.Count; i++)
            _monsterbooklist[i].SaveData(i);
        for (int i = 0; i < _timeattackclearlist.Count; i++)
            _timeattackclearlist[i].SaveData(i);
        for (int i = 0; i < _timeattackranklist.Count; i++)
            _timeattackranklist[i].SaveData(i);
        for (int i = 0; i < _roomactionlist.Count; i++)
            PlayerPrefs.SetInt("RoomActionList_" + i.ToString(), _roomactionlist[i]);
        for (int i = 0; i < _roomthemalist.Count; i++)
            PlayerPrefs.SetInt("RoomThemaList_" + i.ToString(), _roomthemalist[i]);
        for (int i = 0; i < _roomfigurelist.Count; i++)
            PlayerPrefs.SetInt("RoomFigureList_" + i.ToString(), _roomfigurelist[i]);
        for (int i = 0; i < _roomfunclist.Count; i++)
            PlayerPrefs.SetInt("RoomFuncList_" + i.ToString(), _roomfunclist[i]);
        for (int i = 0; i < _usermarklist.Count; i++)
            PlayerPrefs.SetInt("UserMarkList_" + i.ToString(), _usermarklist[i]);
        for (int i = 0; i < _roomthemeslotlist.Count; i++)
            _roomthemeslotlist[i].SaveData(i);
        for (int i = 0; i < _roomthemefigureslotlist.Count; i++)
            _roomthemefigureslotlist[i].SaveData(i);
        for (int i = 0; i < _mailList.Count; i++)
            _mailList[i].SaveData(i);
        for (int i = 0; i < _eventSetDatList.Count; i++)
            _eventSetDatList[i].SaveData(i);
    }

    public void LoadLocalData()
    {
        ServerData _serverdata = GameInfo.Instance.ServerData;
        UserData _userdata = GameInfo.Instance.UserData;
        List<StoreData> _storelist = GameInfo.Instance.StoreList;
        List<AchieveData> _achievelist = GameInfo.Instance.AchieveList;
        List<CharData> _charlist = GameInfo.Instance.CharList;
        List<WeaponData> _weaponlist = GameInfo.Instance.WeaponList;
        List<GemData> _gemlist = GameInfo.Instance.GemList;
        List<CardData> _cardlist = GameInfo.Instance.CardList;
        List<ItemData> _itemlist = GameInfo.Instance.ItemList;
        List<FacilityData> _facilitylist = GameInfo.Instance.FacilityList;
        List<StageClearData> _stageclearlist = GameInfo.Instance.StageClearList;
        List<WeaponBookData> _weaponbooklist = GameInfo.Instance.WeaponBookList;
        List<CardBookData> _cardbooklist = GameInfo.Instance.CardBookList;
        List<MonsterBookData> _monsterbooklist = GameInfo.Instance.MonsterBookList;
        List<TimeAttackClearData> _timeattackclearlist = GameInfo.Instance.TimeAttackClearList;
        List<TimeAttackRankData> _timeattackranklist = GameInfo.Instance.TimeAttackRankList;
        List<int> _costumelist = GameInfo.Instance.CostumeList;
        List<int> _roomactionlist = GameInfo.Instance.RoomActionList;
        List<int> _roomthemalist = GameInfo.Instance.RoomThemaList;
        List<int> _roomfigurelist = GameInfo.Instance.RoomFigureList;
        List<int> _roomfunclist = GameInfo.Instance.RoomFuncList;
        List<int> _usermarklist = GameInfo.Instance.UserMarkList;
        List<RoomThemeSlotData> _roomthemeslotlist = GameInfo.Instance.RoomThemeSlotList;
        List<RoomThemeFigureSlotData> _roomthemefigureslotlist = GameInfo.Instance.RoomThemeFigureSlotList;

        List<MailData> _mailList = GameInfo.Instance.MailList;
        WeekMissionData _weeklyMisstionData = GameInfo.Instance.WeekMissionData;

        List<EventSetData> _eventSetDataList = GameInfo.Instance.EventSetDataList;

        int tempUID = PlayerPrefs.GetInt("TestCharUID");
        if (_testcharuid <= tempUID)
            _testcharuid = tempUID;
        tempUID = PlayerPrefs.GetInt("TestItemUID");
        if (_testitemuid <= tempUID)
            _testitemuid = tempUID;

        _storelist.Clear();
        _achievelist.Clear();
        _charlist.Clear();
        _weaponlist.Clear();
        _gemlist.Clear();
        _cardlist.Clear();
        _itemlist.Clear();
        _costumelist.Clear();
        _facilitylist.Clear();
        _stageclearlist.Clear();
        _weaponbooklist.Clear();
        _cardbooklist.Clear();
        _monsterbooklist.Clear();
        _timeattackclearlist.Clear();
        _timeattackranklist.Clear();
        _roomactionlist.Clear();
        _roomthemalist.Clear();
        _roomfigurelist.Clear();
        _roomfunclist.Clear();
        _usermarklist.Clear();
        _roomthemeslotlist.Clear();
        _roomthemefigureslotlist.Clear();
        _mailList.Clear();
        _eventSetDataList.Clear();

        _serverdata.LoadData();
        _userdata.LoadData();
        _weeklyMisstionData.LoadData();

        int Store_Count = PlayerPrefs.GetInt("Store_Count", _storelist.Count);
        int AchieveList_Count = PlayerPrefs.GetInt("AchieveList_Count", _achievelist.Count);
        int CharList_Count = PlayerPrefs.GetInt("CharList_Count", _charlist.Count);
        int WeaponList_Count = PlayerPrefs.GetInt("WeaponList_Count", _weaponlist.Count);
        int GemList_Count = PlayerPrefs.GetInt("GemList_Count", _gemlist.Count);
        int CardList_Count = PlayerPrefs.GetInt("CardList_Count", _cardlist.Count);
        int ItemList_Count = PlayerPrefs.GetInt("ItemList_Count", _itemlist.Count);
        int CostumeList_Count = PlayerPrefs.GetInt("CostumeList_Count", _costumelist.Count);
        int FacilityList_Count = PlayerPrefs.GetInt("FacilityList_Count", _facilitylist.Count);
        int StageClearList_Count = PlayerPrefs.GetInt("StageClearList_Count", _stageclearlist.Count);
        int WeaponBookList_Count = PlayerPrefs.GetInt("WeaponBookList_Count", _weaponbooklist.Count);
        int CardBookList_Count = PlayerPrefs.GetInt("CardBookList_Count", _cardbooklist.Count);
        int MonsterBookList_Count = PlayerPrefs.GetInt("MonsterBookList_Count", _monsterbooklist.Count);
        int TimeAttackClearList_Count = PlayerPrefs.GetInt("TimeAttackClearList_Count", _timeattackclearlist.Count);
        int TimeAttackRankList_Count = PlayerPrefs.GetInt("TimeAttackRankList_Count", _timeattackranklist.Count);
        int RoomActionList_Count = PlayerPrefs.GetInt("RoomActionList_Count", _roomactionlist.Count);
        int RoomThemaList_Count = PlayerPrefs.GetInt("RoomThemaList_Count", _roomthemalist.Count);
        int RoomFigureList_Count = PlayerPrefs.GetInt("RoomFigureList_Count", _roomfigurelist.Count);
        int RoomFuncList_Count = PlayerPrefs.GetInt("RoomFuncList_Count", _roomfunclist.Count);
        int RoomUserMark_Count = PlayerPrefs.GetInt("RoomUserMark_Count", _usermarklist.Count);
        int RoomThemeSlotList_Count = PlayerPrefs.GetInt("RoomThemeSlotList_Count", _roomthemeslotlist.Count);
        int RoomThemeFigureSlotList_Count = PlayerPrefs.GetInt("RoomThemeFigureSlotList_Count", _roomthemefigureslotlist.Count);
        int MailList_Count = PlayerPrefs.GetInt("MailList_Count", _mailList.Count);
        int EventSetDataList_Count = PlayerPrefs.GetInt("EventSetDataList_Count", _eventSetDataList.Count);

        for (int i = 0; i < Store_Count; i++)
            _storelist.Add(new StoreData());
        for (int i = 0; i < AchieveList_Count; i++)
            _achievelist.Add(new AchieveData());
        for (int i = 0; i < CharList_Count; i++)
            _charlist.Add(new CharData());
        for (int i = 0; i < WeaponList_Count; i++)
            _weaponlist.Add(new WeaponData());
        for (int i = 0; i < GemList_Count; i++)
            _gemlist.Add(new GemData());
        for (int i = 0; i < CardList_Count; i++)
            _cardlist.Add(new CardData());
        for (int i = 0; i < ItemList_Count; i++)
            _itemlist.Add(new ItemData());
        for (int i = 0; i < FacilityList_Count; i++)
            _facilitylist.Add(new FacilityData());
        for (int i = 0; i < StageClearList_Count; i++)
            _stageclearlist.Add(new StageClearData());
        for (int i = 0; i < WeaponBookList_Count; i++)
            _weaponbooklist.Add(new WeaponBookData());
        for (int i = 0; i < CardBookList_Count; i++)
            _cardbooklist.Add(new CardBookData());
        for (int i = 0; i < MonsterBookList_Count; i++)
            _monsterbooklist.Add(new MonsterBookData());
        for (int i = 0; i < TimeAttackClearList_Count; i++)
            _timeattackclearlist.Add(new TimeAttackClearData());
        for (int i = 0; i < TimeAttackRankList_Count; i++)
            _timeattackranklist.Add(new TimeAttackRankData());
        for (int i = 0; i < RoomThemeSlotList_Count; i++)
            _roomthemeslotlist.Add(new RoomThemeSlotData());
        for (int i = 0; i < RoomThemeFigureSlotList_Count; i++)
            _roomthemefigureslotlist.Add(new RoomThemeFigureSlotData());
        for (int i = 0; i < MailList_Count; i++)
            _mailList.Add(new MailData());
        for (int i = 0; i < EventSetDataList_Count; i++)
            _eventSetDataList.Add(new EventSetData());



        for (int i = 0; i < Store_Count; i++)
            _storelist[i].LoadData(i);
        for (int i = 0; i < AchieveList_Count; i++)
            _achievelist[i].LoadData(i);
        for (int i = 0; i < CharList_Count; i++)
            _charlist[i].LoadData(i);
        for (int i = 0; i < WeaponList_Count; i++)
            _weaponlist[i].LoadData(i);
        for (int i = 0; i < GemList_Count; i++)
            _gemlist[i].LoadData(i);
        for (int i = 0; i < CardList_Count; i++)
            _cardlist[i].LoadData(i);
        for (int i = 0; i < ItemList_Count; i++)
            _itemlist[i].LoadData(i);
        for (int i = 0; i < FacilityList_Count; i++)
            _facilitylist[i].LoadData(i);
        for (int i = 0; i < StageClearList_Count; i++)
            _stageclearlist[i].LoadData(i);
        for (int i = 0; i < CostumeList_Count; i++)
            _costumelist.Add(PlayerPrefs.GetInt("CostumeList_" + i.ToString()));
        for (int i = 0; i < WeaponBookList_Count; i++)
            _weaponbooklist[i].LoadData(i);
        for (int i = 0; i < CardBookList_Count; i++)
            _cardbooklist[i].LoadData(i);
        for (int i = 0; i < MonsterBookList_Count; i++)
            _monsterbooklist[i].LoadData(i);
        for (int i = 0; i < TimeAttackClearList_Count; i++)
            _timeattackclearlist[i].LoadData(i);
        for (int i = 0; i < TimeAttackRankList_Count; i++)
            _timeattackranklist[i].LoadData(i);
        for (int i = 0; i < RoomActionList_Count; i++)
            _roomactionlist.Add(PlayerPrefs.GetInt("RoomActionList_" + i.ToString()));
        for (int i = 0; i < RoomThemaList_Count; i++)
            _roomthemalist.Add(PlayerPrefs.GetInt("RoomThemaList_" + i.ToString()));
        for (int i = 0; i < RoomFigureList_Count; i++)
            _roomfigurelist.Add(PlayerPrefs.GetInt("RoomFigureList_" + i.ToString()));
        for (int i = 0; i < RoomFuncList_Count; i++)
            _roomfunclist.Add(PlayerPrefs.GetInt("RoomFuncList_" + i.ToString()));
        for (int i = 0; i < RoomUserMark_Count; i++)
            _usermarklist.Add(PlayerPrefs.GetInt("UserMarkList_" + i.ToString()));

        for (int i = 0; i < RoomThemeSlotList_Count; i++)
            _roomthemeslotlist[i].LoadData(i);
        for (int i = 0; i < RoomThemeFigureSlotList_Count; i++)
            _roomthemefigureslotlist[i].LoadData(i);


        for (int i = 0; i < MailList_Count; i++)
            _mailList[i].LoadData(i);
        for (int i = 0; i < _eventSetDataList.Count; i++)
            _eventSetDataList[i].LoadData(i);

        GameInfo.Instance.MailTotalCount = _mailList.Count;
        //mailsvrcount = GameInfo.Instance.MailList.Count;

    }

    //------------------------------------------------------------------------------------------------------------------------------------
    //
    //로그인
    //
    //------------------------------------------------------------------------------------------------------------------------------------

    public void Proc_Login(OnReceiveCallBack ReceiveCallBack)//, OnReceiveFailCallBack RecieveFileDelegate = null)
    {
        bool b = PlayerPrefs.HasKey("NetLocalSvr");
        if (!b)
        {
            Regist();

            SaveLocalData();
        }

        LoadLocalData();

        Login();

        ReceiveCallBack(0, null);
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //
    //  유저 정보 관련
    //
    //------------------------------------------------------------------------------------------------------------------------------------
    public void Proc_ReqRewardTakeAchieve(List<int> list, OnReceiveCallBack ReceiveCallBack)
    {
        int groupid = list[0];
        AchieveData achievedata = GameInfo.Instance.GetAchieveData(groupid);
        AchieveData achievecontribution = GameInfo.Instance.AchieveList.Find(x => x.GroupID == GameInfo.Instance.GameConfig.AchieveContributionGroupID);
        UserData _userdata = GameInfo.Instance.UserData;

        if (achievedata == null || achievecontribution == null)
        {
            ReceiveCallBack(2, null);
            return;
        }

        bool bcomplete = false;


        bcomplete = GameSupport.IsAchievementComplete(achievedata);

        if (!bcomplete)
        {
            ReceiveCallBack(2, null);
            return;
        }
        else
        {
            GameInfo.Instance.RewardList.Clear();
            GameInfo.Instance.RewardList.Add(new RewardData(achievedata.TableData.ProductType, achievedata.TableData.ProductIndex, achievedata.TableData.ProductValue));

            achievecontribution.Value += achievedata.TableData.RewardAchievePoint;
            _userdata.AddGoods((eGOODSTYPE)achievedata.TableData.ProductIndex, achievedata.TableData.ProductValue);
            achievedata.GroupOrder += 1;
            achievedata.SetTable();
        }

        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    public void Proc_ReqUserMarkList(OnReceiveCallBack ReceiveCallBack)
    {
        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    public void Proc_ReqUserSetMark(int mark, OnReceiveCallBack ReceiveCallBack)
    {
        UserData _userdata = GameInfo.Instance.UserData;

        if (!GameInfo.Instance.IsUserMark(mark))
        {
            ReceiveCallBack(2, null);
            return;
        }

        _userdata.UserMarkID = mark;

        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    public void Proc_ReqUserSetName(string nickname, OnReceiveCallBack ReceiveCallBack)
    {
        UserData _userdata = GameInfo.Instance.UserData;

        _userdata.SetNickName( nickname );

        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    public void Proc_ReqUserSetCommentMsg(string word, OnReceiveCallBack ReceiveCallBack)
    {
        UserData _userdata = GameInfo.Instance.UserData;

        _userdata.UserWord = word;

        SaveLocalData();
        ReceiveCallBack(0, null);
    }


    public void Proc_ReqAddItemSlot(OnReceiveCallBack ReceiveCallBack)
    {
        UserData _userdata = GameInfo.Instance.UserData;
        GameConfig _gameconfig = GameInfo.Instance.GameConfig;
        //레벨 제한 확인
        if (_userdata.ItemSlotCnt >= _gameconfig.MaxItemSlotCount)
        {
            ReceiveCallBack(1, null);
            return;
        }
        if (!_userdata.IsGoods(eGOODSTYPE.GOLD, _gameconfig.AddItemSlotGold))
        {
            ReceiveCallBack(1, null);
            return;
        }
        _userdata.ItemSlotCnt += _gameconfig.AddItemSlotCount;
        _userdata.SubGoods(eGOODSTYPE.GOLD, _gameconfig.AddItemSlotGold);
        SaveLocalData();
        ReceiveCallBack(0, null);
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //
    //  캐릭터관리 관련
    //
    //------------------------------------------------------------------------------------------------------------------------------------
    //메인 캐릭터 등록
    public void Proc_CharMain(long charuid, OnReceiveCallBack ReceiveCallBack)
    {
        UserData _userdata = GameInfo.Instance.UserData;

        //캐릭터 확인
        CharData chardata = GameInfo.Instance.GetCharData(charuid);
        if (chardata == null)
        {
            ReceiveCallBack(2, null);
            return;
        }

        //유저데이타에 메인캐릭터로 등록
        _userdata.MainCharUID = charuid;

        SaveLocalData();
        ReceiveCallBack(0, null);
    }
    //캐릭터 등급업
    public void Proc_CharGardeUpRequest(long charuid, OnReceiveCallBack ReceiveCallBack)
    {
        UserData _userdata = GameInfo.Instance.UserData;
        List<ItemData> _itemlist = GameInfo.Instance.ItemList;

        //캐릭터 보유 확인
        CharData chardata = GameInfo.Instance.GetCharData(charuid);
        if (chardata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }

        //최대 등급인지 확인
        if (GameSupport.IsMaxGrade(chardata.Grade))
        {
            ReceiveCallBack(1, null);
            return;
        }

        //필요 재료 확인
        var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == chardata.TableData.WakeReqGroup && x.Level == chardata.Grade);
        if (reqdata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }
        if (!_userdata.IsGoods(eGOODSTYPE.GOLD, reqdata.Gold)) //금액부족
        {
            ReceiveCallBack(1, null);
            return;
        }
        List<MatItemData> matitemlist = new List<MatItemData>();
        //재료 체크
        List<int> idlist = new List<int>();
        List<int> countlist = new List<int>();
        idlist.Add(reqdata.ItemID1);
        idlist.Add(reqdata.ItemID2);
        idlist.Add(reqdata.ItemID3);
        idlist.Add(reqdata.ItemID4);
        countlist.Add(reqdata.Count1);
        countlist.Add(reqdata.Count2);
        countlist.Add(reqdata.Count3);
        countlist.Add(reqdata.Count4);
        for (int i = 0; i < idlist.Count; i++)
        {
            if (idlist[i] != -1)
            {
                var matitem = _itemlist.Find(x => x.TableID == idlist[i]);
                if (matitem != null)
                {
                    if (countlist[i] > matitem.Count)
                    {
                        ReceiveCallBack(1, null);
                        return;
                    }
                    else
                    {
                        matitemlist.Add(new MatItemData(matitem, countlist[i]));
                    }
                }
                else
                {
                    ReceiveCallBack(1, null);
                    return;
                }
            }
        }

        //재료 제거
        for (int i = 0; i < matitemlist.Count; i++)
            DelItem(matitemlist[i].ItemData.ItemUID, matitemlist[i].Count);

        //캐릭터 등급업
        chardata.Grade += 1;

        //재화 삭감
        _userdata.SubGoods(eGOODSTYPE.GOLD, reqdata.Gold);

        SaveLocalData();
        ReceiveCallBack(0, null);

    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //
    //  인법 룬, 커맨드 콤보 패시브 관련
    //
    //------------------------------------------------------------------------------------------------------------------------------------
    public void Proc_CharSkillPassiveLevelUp(long charuid, int skillid, OnReceiveCallBack ReceiveCallBack)
    {
        UserData _userdata = GameInfo.Instance.UserData;

        CharData chardata = GameInfo.Instance.GetCharData(charuid);
        if (chardata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }
        //패시브 스킬 테이블 확인
        var charskilldata = GameInfo.Instance.GameTable.FindCharacterSkillPassive(x => x.CharacterID == chardata.TableID && x.ID == skillid);
        if (charskilldata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }

        int level = 0;
        //최대 레벨 체크
        var passiveskil = chardata.PassvieList.Find(x => x.SkillID == skillid);
        if (passiveskil != null)
        {
            level = passiveskil.SkillLevel;
            if (passiveskil.SkillLevel >= charskilldata.MaxLevel)
            {
                ReceiveCallBack(1, null);
                return;
            }
        }

        var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == charskilldata.ItemReqListID && x.Level == level);
        if (reqdata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }


        //레벨 제한 확인
        if (chardata.Level < reqdata.LimitLevel)
        {
            ReceiveCallBack(1, null);
            return;
        }
        //스킬 포인트 확인     
        if (reqdata.GoodsValue > chardata.PassviePoint)
        {
            ReceiveCallBack(1, null);
            return;
        }
        //재화 확인
        if (!_userdata.IsGoods(eGOODSTYPE.GOLD, reqdata.Gold)) //금액부족
        {
            ReceiveCallBack(1, null);
            return;
        }



        //기존에 습득한 스킬인지 확인
        if (passiveskil != null)
            passiveskil.SkillLevel += 1;
        else
            chardata.PassvieList.Add(new PassiveData(skillid, 1));


        chardata.PassviePoint -= reqdata.GoodsValue;
        _userdata.SubGoods(eGOODSTYPE.GOLD, reqdata.Gold);


        SaveLocalData();
        ReceiveCallBack(0, null);
    }
    public void Proc_CharSkillEquip(long charuid, int slot, int skillid, OnReceiveCallBack ReceiveCallBack)
    {
        CharData chardata = GameInfo.Instance.GetCharData(charuid);
        if (chardata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }

        if (0 > slot || (int)eCOUNT.SKILLSLOT <= slot)
        {
            ReceiveCallBack(1, null);
            return;
        }

        var passiveskil = chardata.PassvieList.Find(x => x.SkillID == skillid);
        if (passiveskil == null)
        {
            ReceiveCallBack(1, null);
            return;
        }

        if (slot == (int)eCOUNT.SKILLULTIMATEINDEX)
        {
            if (passiveskil.TableData.Type != (int)eCHARSKILLPASSIVETYPE.SELECT_ULTIMATE)
            {
                ReceiveCallBack(1, null);
                return;
            }
        }
        else
        {
            if (passiveskil.TableData.Type == (int)eCHARSKILLPASSIVETYPE.SELECT_ULTIMATE)
            {
                ReceiveCallBack(1, null);
                return;
            }
        }

        //슬롯 사용 레벨 제한
        if (chardata.Level < GameInfo.Instance.GameConfig.CharSkillSlotLimitLevel[slot])
        {
            ReceiveCallBack(1, null);
            return;
        }


        for (int i = 0; i < (int)eCOUNT.SKILLSLOT; i++)
        {
            if (chardata.EquipSkill[i] == skillid)
                chardata.EquipSkill[i] = 0;
        }




        chardata.EquipSkill[slot] = skillid;

        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    public void Proc_CharSkillRemove(long charuid, int slot, OnReceiveCallBack ReceiveCallBack)
    {
        CharData chardata = GameInfo.Instance.GetCharData(charuid);
        if (chardata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }

        if (0 > slot || (int)eCOUNT.SKILLSLOT <= slot)
        {
            ReceiveCallBack(1, null);
            return;
        }

        if (slot == (int)eCOUNT.SKILLULTIMATEINDEX)
        {
            ReceiveCallBack(1, null);
            return;
        }

        chardata.EquipSkill[slot] = 0;

        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    public void Proc_CharSkillUpdate(long charuid, int[] skillSlots, OnReceiveCallBack ReceiveCallBack)
    {
        CharData chardata = GameInfo.Instance.GetCharData(charuid);
        if (chardata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }

        if (skillSlots == null || (int)eCOUNT.SKILLSLOT < skillSlots.Length)
        {
            ReceiveCallBack(1, null);
            return;
        }

        //if (slot == (int)eCOUNT.SKILLULTIMATEINDEX)
        //{
        //    ReceiveCallBack(1, null);
        //    return;
        //}

        chardata.EquipSkill = skillSlots;

        SaveLocalData();
        ReceiveCallBack(0, null);
    }


    //------------------------------------------------------------------------------------------------------------------------------------
    //
    //  코스튬 관련
    //
    //------------------------------------------------------------------------------------------------------------------------------------
    //캐릭터 코스튬 장착
    public void Proc_CharCostumeEquip(long charuid, int costumeid, int costumecolor, int costumeflag, OnReceiveCallBack ReceiveCallBack)
    {
        GameTable _gametable = GameInfo.Instance.GameTable;

        //캐릭터 보유 확인
        CharData chardata = GameInfo.Instance.GetCharData(charuid);
        if (chardata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }

        //코스튬ID 체크
        var costumetabledata = _gametable.FindCostume(costumeid);
        if (costumetabledata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }

        //코스튬이 해당 캐릭터용인지 확인
        if (costumetabledata.CharacterID != chardata.TableID)
        {
            ReceiveCallBack(1, null);
            return;
        }

        //코스튬 보유 확인
        if (!GameInfo.Instance.HasCostume(costumeid))
        {
            ReceiveCallBack(1, null);
            return;
        }

        //캐릭터에 코스튬 아이디 등록
        chardata.EquipCostumeID = costumeid;
        chardata.CostumeColor = costumecolor;
        chardata.CostumeStateFlag = costumeflag;

        SaveLocalData();
        ReceiveCallBack(0, null);
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //
    //  아이템 관련
    //
    //------------------------------------------------------------------------------------------------------------------------------------
    //아이템 판매
    public void Proc_ReqSellItemList(long uid, int count, OnReceiveCallBack ReceiveCallBack)
    {
        UserData _userdata = GameInfo.Instance.UserData;

        //아이템 보유 확인
        var itemdata = GameInfo.Instance.GetItemData(uid);
        if (itemdata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }
        //판매량과 보유량 확인
        if (count > itemdata.Count)
        {
            ReceiveCallBack(2, null);
            return;
        }

        //판매 수량만큼 금액지급
        int gold = count * itemdata.TableData.SellPrice;
        _userdata.AddGoods(eGOODSTYPE.GOLD, gold);

        //아이템 수량 삭감

        DelItem(itemdata.ItemUID, count);

        //  주간 미션 아이템 판매(판매 갯수당)
        for (int i = 0; i < count; i++)
            SetClearMission(eMISSIONTYPE.WM_Con_ItemSell);

        SaveLocalData();
        ReceiveCallBack(0, null);
    }
    //아이템 사용
    public void Proc_ReqUseItem(long uid, int count, int value, OnReceiveCallBack ReceiveCallBack)
    {
        UserData _userdata = GameInfo.Instance.UserData;

        //아이템 보유 확인
        var itemdata = GameInfo.Instance.GetItemData(uid);
        if (itemdata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }
        //수량 확인
        if (count > itemdata.Count)
        {
            ReceiveCallBack(2, null);
            return;
        }
        if (itemdata.TableData.Type != (int)eITEMTYPE.USE)
        {
            ReceiveCallBack(3, null);
            return;
        }
        //타입별 효과 처리
        if (itemdata.TableData.SubType == (int)eITEMSUBTYPE.USE_AP_CHARGE)
        {
            int nValue = GameSupport.GetMaxAP();
            _userdata.Goods[(int)eGOODSTYPE.AP] += nValue * count;
            //아이템 수량 삭감
            DelItem(itemdata.ItemUID, count);
        }
        else if (itemdata.TableData.SubType == (int)eITEMSUBTYPE.USE_BP_CHARGE)
        {
            _userdata.Goods[(int)eGOODSTYPE.BP] += itemdata.TableData.Value * count;
            //아이템 수량 삭감
            DelItem(itemdata.ItemUID, count);
        }
        else if (itemdata.TableData.SubType == (int)eITEMSUBTYPE.USE_SELECT_ITEM)
        {
            GameInfo.Instance.RewardList.Clear();
            GameTable.Random.Param data = GameInfo.Instance.GameTable.Randoms.Find(x => x.GroupID == itemdata.TableData.Value && x.Value == value);
            if (data != null)
            {
                long addUid = -1;
                for (int i = 0; i < count; i++)
                {
                    if ((eREWARDTYPE)data.ProductType == eREWARDTYPE.CHAR)
                    {
                        addUid = AddChar(data.ProductIndex);
                    }
                    else if ((eREWARDTYPE)data.ProductType == eREWARDTYPE.WEAPON)
                    {
                        addUid = AddWeapon(data.ProductIndex);
                    }
                    else if ((eREWARDTYPE)data.ProductType == eREWARDTYPE.GEM)
                    {
                        addUid = AddGem(data.ProductIndex);
                    }
                    else if ((eREWARDTYPE)data.ProductType == eREWARDTYPE.CARD)
                    {
                        addUid = AddCard(data.ProductIndex);
                    }
                    else if ((eREWARDTYPE)data.ProductType == eREWARDTYPE.COSTUME)
                    {
                        addUid = AddCostume(data.ProductIndex);
                    }
                    else if ((eREWARDTYPE)data.ProductType == eREWARDTYPE.ITEM)
                    {
                        addUid = AddItem(data.ProductIndex, data.ProductValue);
                    }
                    else
                    {
                        addUid = 0;
                    }
                    RewardData reward = new RewardData(addUid, data.ProductType, data.ProductIndex, data.ProductValue, false);
                    GameInfo.Instance.RewardList.Add(reward);
                }
            }
            //아이템 수량 삭감
            DelItem(itemdata.ItemUID, count);
        }
        else if (itemdata.TableData.SubType == (int)eITEMSUBTYPE.USE_RANDOM_ITEM)
        {
            GameInfo.Instance.RewardList.Clear();
            Purchase_Random(itemdata.TableData.Value, count, ref GameInfo.Instance.RewardList, false);
            //아이템 수량 삭감
            DelItem(itemdata.ItemUID, count);
        }
        else if (itemdata.TableData.SubType == (int)eITEMSUBTYPE.USE_PACKAGE_ITEM)
        {
            GameInfo.Instance.RewardList.Clear();
            Purchase_Package(itemdata.TableData.Value, ref GameInfo.Instance.RewardList);
            //아이템 수량 삭감
            DelItem(itemdata.ItemUID, count);
        }
        else
        {
            ReceiveCallBack(4, null);
            return;
        }

        SaveLocalData();
        ReceiveCallBack(0, null);
    }
    //----------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  무기관련
    //
    //----------------------------------------------------------------------------------------------------------------------------------------------------
    public void Proc_WeaponCharEquip(long charuid, long mainWeaponUID, long subWeaponUID, int mainSkinTID, int subSkinTID, OnReceiveCallBack ReceiveCallBack)
    {
        //캐릭터 보유 확인
        CharData chardata = GameInfo.Instance.GetCharData(charuid);
        if (chardata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }

        //Main Weapon
        {
            //무기 보유 확인
            var weapon = GameInfo.Instance.GetWeaponData(mainWeaponUID);
            if (weapon == null)
            {
                ReceiveCallBack(2, null);//인덱스 이상
                return;
            }

            //내 직업 무기가 맞는지 체크
            for( int i = 0; i < weapon.ListCharId.Count; i++ ) {
                if( chardata.TableID != weapon.ListCharId[i] ) {
                    ReceiveCallBack( 3, null );
                    return;
                }
			}

            //캐릭터에 무기 등록
            chardata.EquipWeaponUID = weapon.WeaponUID;
            chardata.EquipWeaponSkinTID = mainSkinTID;
        }

        //Sub Weapon
        {
            if (subWeaponUID != (int)eCOUNT.NONE)
            {
                var weapon = GameInfo.Instance.GetWeaponData(subWeaponUID);
                if (weapon == null)
                {
                    ReceiveCallBack(2, null);//인덱스 이상
                    return;
                }

                //내 직업 무기가 맞는지 체크
                for( int i = 0; i < weapon.ListCharId.Count; i++ ) {
                    if( chardata.TableID != weapon.ListCharId[i] ) {
                        ReceiveCallBack( 3, null );
                        return;
                    }
                }

                chardata.EquipWeapon2UID = weapon.WeaponUID;
                chardata.EquipWeapon2SkinTID = subSkinTID;
            }
            else
            {
                //해제
                chardata.EquipWeapon2UID = 0;
                chardata.EquipWeapon2SkinTID = 0;
            }


            //캐릭터에 무기 등록

        }


        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    //무기 판매
    public void Proc_WeaponSell(List<long> uidlist, OnReceiveCallBack ReceiveCallBack)
    {
        UserData _userdata = GameInfo.Instance.UserData;

        int gold = 0;
        int sp = 0;
        for (int i = 0; i < uidlist.Count; i++)
        {
            //무기 보유 확인
            var weapondata = GameInfo.Instance.GetWeaponData(uidlist[i]);
            if (weapondata == null)
            {
                ReceiveCallBack(1, null);//인덱스 이상
                return;
            }
            //무기 잠금인경우 판매금지
            if (weapondata.Lock == true)
            {
                ReceiveCallBack(2, null);//잠금 상태
                return;
            }

            //무기 장착중일때 판매금지
            CharData chardata = GameInfo.Instance.GetEquipWeaponCharData(weapondata.WeaponUID);
            if (chardata != null)
            {
                ReceiveCallBack(3, null);//착용 중 아이템
                return;
            }

            gold += weapondata.TableData.SellPrice;
            sp += weapondata.TableData.SellMPoint;
        }

        //무기 삭제
        for (int i = 0; i < uidlist.Count; i++)
            DelWeapon(uidlist[i]);

        //판매금액 지급
        _userdata.AddGoods(eGOODSTYPE.GOLD, gold);
        _userdata.AddGoods(eGOODSTYPE.SUPPORTERPOINT, sp);

        SaveLocalData();
        ReceiveCallBack(0, null);
    }
    //무기 락, 언락
    public void Proc_WeaponLock(List<long> weaponuidlist, List<bool> locklist, OnReceiveCallBack ReceiveCallBack)
    {
        if (weaponuidlist.Count != locklist.Count)
        {
            ReceiveCallBack(1, null);//인덱스 이상
            return;
        }
        //무기 보유 확인
        for (int i = 0; i < weaponuidlist.Count; i++)
        {
            var weapondata = GameInfo.Instance.GetWeaponData(weaponuidlist[i]);
            if (weapondata == null)
            {
                ReceiveCallBack(1, null);//인덱스 이상
                return;
            }
        }

        //무기 Lock상태 변경
        for (int i = 0; i < weaponuidlist.Count; i++)
        {
            var weapondata = GameInfo.Instance.GetWeaponData(weaponuidlist[i]);
            if (weapondata != null)
                weapondata.Lock = locklist[i];
        }

        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    //무기 레벨업
    public void Proc_ReqLvUpWeapon(long weaponuid, bool bmatitem, List<long> matlist, List<MatItemData> matitemlist, OnReceiveCallBack ReceiveCallBack)
    {
        UserData _userdata = GameInfo.Instance.UserData;

        //무기 보유 확인
        WeaponData target = GameInfo.Instance.GetWeaponData(weaponuid);
        if (target == null)
        {
            ReceiveCallBack(1, null);
            return;
        }
        WeaponBookData targetbook = GameInfo.Instance.GetWeaponBookData(target.TableID);
        if (targetbook == null)
        {
            ReceiveCallBack(1, null);
            return;
        }
        //무기 최대레벨 체크
        if (GameSupport.IsMaxLevelWeapon(target, target.Level))
        {
            ReceiveCallBack(1, null);
            return;
        }
        int exp = 0;
        int gold = 0;

        if (bmatitem)
        {
            //재료 확인
            if (matitemlist.Count == 0)
            {
                ReceiveCallBack(1, null);
                return;
            }
            //무기 재료 유무 확인
            for (int i = 0; i < matitemlist.Count; i++)
            {
                ItemData matitem = GameInfo.Instance.GetItemData(matitemlist[i].ItemData.ItemUID);
                if (matitem == null)
                {
                    ReceiveCallBack(1, null);
                    return;
                }

                if (matitemlist[i].Count > matitem.Count)
                {
                    ReceiveCallBack(1, null);
                    return;
                }
            }

            //모든 재료의 Exp
            exp = GameSupport.GetItemMatExp(matitemlist);
            //모든 재료의 필요 골드
            gold = GameSupport.GetWeaponLevelUpItemCost(target, matitemlist);

            //필요 금액 확인
            if (!_userdata.IsGoods(eGOODSTYPE.GOLD, gold))
            {
                ReceiveCallBack(1, null);
                return;
            }

            //재료무기 삭제
            for (int i = 0; i < matitemlist.Count; i++)
            {
                DelItem(matitemlist[i].ItemData.ItemUID, matitemlist[i].Count);
                //  주간 미션 갱신(재료 갯수당)
                SetClearMission(eMISSIONTYPE.WM_Con_WpnUpgrade);
            }
        }
        else
        {
            //재료 확인
            if (matlist.Count == 0)
            {
                ReceiveCallBack(1, null);
                return;
            }

            //무기 재료 유무 확인
            List<WeaponData> matweaponlist = new List<WeaponData>();
            for (int i = 0; i < matlist.Count; i++)
            {
                WeaponData matitem = GameInfo.Instance.GetWeaponData(matlist[i]);
                if (matitem == null)
                {
                    ReceiveCallBack(1, null);
                    return;
                }
                else
                {
                    matweaponlist.Add(matitem);
                }
            }
            //모든 재료의 Exp
            exp = GameSupport.GetWeaponMatExp(matweaponlist);
            //모든 재료의 필요 골드
            gold = GameSupport.GetWeaponLevelUpCost(target, matweaponlist);

            //필요 금액 확인
            if (!_userdata.IsGoods(eGOODSTYPE.GOLD, gold))
            {
                ReceiveCallBack(1, null);
                return;
            }

            //재료무기 삭제
            for (int i = 0; i < matlist.Count; i++)
            {
                DelWeapon(matlist[i]);
                //  주간 미션 갱신(재료 갯수당)
                SetClearMission(eMISSIONTYPE.WM_Con_WpnUpgrade);
            }
        }


        //최대 경험치에 넘어가면 보정
        int nRemainExp = GameSupport.GetRemainWeaponExpToMaxLevel(target);
        if (exp >= nRemainExp)
            exp = nRemainExp;

        //경험치 추가
        target.Exp += exp;
        //경험치별 레벨
        target.Level = GameSupport.GetWeaponExpLevel(target, target.Exp);

        if (GameSupport.IsMaxWakeWeapon(target) && GameSupport.IsMaxLevelWeapon(target))
            targetbook.DoOnFlag(eBookStateFlag.MAX_WAKE_AND_LV);

        //골드 삭감
        _userdata.SubGoods(eGOODSTYPE.GOLD, gold);

        SaveLocalData();
        ReceiveCallBack(0, null);

    }

    //무기 각성
    public void Proc_ReqWakeWeapon(long weaponuid, OnReceiveCallBack ReceiveCallBack)
    {
        UserData _userdata = GameInfo.Instance.UserData;
        List<ItemData> _itemlist = GameInfo.Instance.ItemList;

        //무기 보유 확인
        WeaponData target = GameInfo.Instance.GetWeaponData(weaponuid);
        if (target == null)
        {
            ReceiveCallBack(1, null);
            return;
        }

        //최대 레벨일때만 각성 가능
        if (!GameSupport.IsMaxLevelWeapon(target))
        {
            ReceiveCallBack(1, null);
            return;
        }

        //최대 각성인지 체크
        if (GameSupport.IsMaxWakeWeapon(target))
        {
            ReceiveCallBack(1, null);
            return;
        }

        //각성의 필요 재료 얻어오기
        var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == target.TableData.WakeReqGroup && x.Level == target.Wake);
        if (reqdata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }
        //필요 골드 확인
        if (!_userdata.IsGoods(eGOODSTYPE.GOLD, reqdata.Gold))
        {
            ReceiveCallBack(1, null);
            return;
        }

        //재료 체크
        List<MatItemData> matitemlist = new List<MatItemData>();
        List<int> idlist = new List<int>();
        List<int> countlist = new List<int>();
        idlist.Add(reqdata.ItemID1);
        idlist.Add(reqdata.ItemID2);
        idlist.Add(reqdata.ItemID3);
        idlist.Add(reqdata.ItemID4);
        countlist.Add(reqdata.Count1);
        countlist.Add(reqdata.Count2);
        countlist.Add(reqdata.Count3);
        countlist.Add(reqdata.Count4);
        for (int i = 0; i < idlist.Count; i++)
        {
            if (idlist[i] != -1)
            {
                var matitem = _itemlist.Find(x => x.TableID == idlist[i]);
                if (matitem != null)
                {
                    if (countlist[i] > matitem.Count)
                    {
                        ReceiveCallBack(1, null);
                        return;
                    }
                    else
                    {
                        matitemlist.Add(new MatItemData(matitem, countlist[i]));
                    }
                }
                else
                {
                    ReceiveCallBack(1, null);
                    return;
                }
            }
        }

        //재료 아이템 제거
        for (int i = 0; i < matitemlist.Count; i++)
            DelItem(matitemlist[i].ItemData.ItemUID, matitemlist[i].Count);

        //각성 +1
        target.Level = 1;
        target.Exp = 0;
        target.Wake += 1;
        var bookWeapon = GameInfo.Instance.GetWeaponBookData(target.TableID);
        if (bookWeapon.StateFlag < 0x00000001 << 0 && target.Wake == 2)
        {
            bookWeapon.StateFlag = (int)(0x00000001 << 0);
        }

        //재료 골드 삭감
        _userdata.SubGoods(eGOODSTYPE.GOLD, reqdata.Gold);


        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    //무기 스킬 레벨업
    public void Proc_ReqSkillLvUpWeapon(long weaponuid, int mattype, long matuid, OnReceiveCallBack ReceiveCallBack)
    {
        //무기 보유 확인
        UserData _userdata = GameInfo.Instance.UserData;
        WeaponData target = GameInfo.Instance.GetWeaponData(weaponuid);
        if (target == null)
        {
            ReceiveCallBack(1, null);
            return;
        }

        if (GameSupport.IsMaxSkillLevelWeapon(target.SkillLv))
        {
            ReceiveCallBack(1, null);
            return;
        }

        int level = 1;
        int gold = 0;

        if (mattype == (int)MatSkillData.eTYPE.ITEM)
        {
            ItemData mat = GameInfo.Instance.GetItemData(matuid);
            if (mat == null)
            {
                ReceiveCallBack(1, null);
                return;
            }
            gold = GameSupport.GetWeaponSkillLevelUpCost(target, 1);
            if (!_userdata.IsGoods(eGOODSTYPE.GOLD, gold))
            {
                ReceiveCallBack(1, null);
                return;
            }

            level = mat.TableData.Value;
            DelItem(matuid, 1);
        }
        else
        {
            WeaponData mat = GameInfo.Instance.GetWeaponData(matuid);
            if (mat == null)
            {
                ReceiveCallBack(1, null);
                return;
            }
            gold = GameSupport.GetWeaponSkillLevelUpCost(target, 1);
            if (!_userdata.IsGoods(eGOODSTYPE.GOLD, gold))
            {
                ReceiveCallBack(1, null);
                return;
            }

            level = mat.SkillLv;
            DelWeapon(matuid);
        }

        target.SkillLv += level;
        if (target.SkillLv >= GameInfo.Instance.GameConfig.WeaponMaxSkillLevel)
            target.SkillLv = GameInfo.Instance.GameConfig.WeaponMaxSkillLevel;
        //재료 골드 삭감
        _userdata.SubGoods(eGOODSTYPE.GOLD, gold);

        SaveLocalData();
        ReceiveCallBack(0, null);
    }
    //----------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  곡옥관련
    //
    //----------------------------------------------------------------------------------------------------------------------------------------------------
    //곡옥 무기에 장착

    public void Proc_GemWeaponEquip(long weaponuid, long[] gemlist, int[] slotlist, OnReceiveCallBack ReceiveCallBack)
    {
        List<WeaponData> _weaponlist = GameInfo.Instance.WeaponList;
        //무기 보유 확인
        WeaponData weapondata = GameInfo.Instance.GetWeaponData(weaponuid);
        if (weapondata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }

        if (gemlist.Length != slotlist.Length)
        {
            ReceiveCallBack(1, null);
            return;
        }

        int slotcount = GameSupport.GetWeaponGradeSlotCount(weapondata.TableData.Grade, weapondata.Wake);

        for (int i = 0; i < slotcount; i++)
        {
            //곡옥 보유 확인
            GemData gemdata = GameInfo.Instance.GetGemData(gemlist[i]);
            if (gemlist[i] != (int)eCOUNT.NONE && gemdata == null)
            {
                ReceiveCallBack(1, null);
                return;
            }
            //슬롯 확인
            if (0 > slotlist[i] || (int)eCOUNT.WEAPONGEMSLOT <= slotlist[i])
            {
                ReceiveCallBack(1, null);
                return;
            }
        }

        for (int x = 0; x < slotcount; x++)
        {
            //이미 장착되어 있는 곡옥이면 일단 제거 (동일 무기 내에서 곡옥의 위치만 변경하는 경우도 이에 해당)
            for (int i = 0; i < _weaponlist.Count; i++)
            {
                for (int j = 0; j < (int)eCOUNT.WEAPONGEMSLOT; j++)
                {
                    if (gemlist[x] != (int)eCOUNT.NONE && _weaponlist[i].SlotGemUID[j] == gemlist[x])
                        _weaponlist[i].SlotGemUID[j] = (int)eCOUNT.NONE;
                }
            }

            //무기에 곡옥 등록
            weapondata.SlotGemUID[slotlist[x]] = gemlist[x];
        }

        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    //곡옥 락, 언락
    public void Proc_ReqSetLockGemList(List<long> gemuidlist, List<bool> locklist, OnReceiveCallBack ReceiveCallBack)
    {
        if (gemuidlist.Count != locklist.Count)
        {
            ReceiveCallBack(1, null);//인덱스 이상
            return;
        }
        //곡옥 보유 확인
        for (int i = 0; i < gemuidlist.Count; i++)
        {
            GemData gemdata = GameInfo.Instance.GetGemData(gemuidlist[i]);
            if (gemdata == null)
            {
                ReceiveCallBack(1, null);//인덱스 이상
                return;
            }
        }
        //곡옥 락,언락
        for (int i = 0; i < gemuidlist.Count; i++)
        {
            GemData gemdata = GameInfo.Instance.GetGemData(gemuidlist[i]);
            if (gemdata != null)
                gemdata.Lock = locklist[i];
        }

        SaveLocalData();
        ReceiveCallBack(0, null);
    }
    //곡옥 판매
    public void Proc_ReqSellGemList(List<long> uidlist, OnReceiveCallBack ReceiveCallBack)
    {
        UserData _userdata = GameInfo.Instance.UserData;

        int gold = 0;
        for (int i = 0; i < uidlist.Count; i++)
        {
            //곡옥 보유 확인
            GemData gemdata = GameInfo.Instance.GetGemData(uidlist[i]);
            if (gemdata == null)
            {
                ReceiveCallBack(1, null);
                return;
            }
            //곡옥 잠김 상태면 판매금지
            if (gemdata.Lock == true)
            {
                ReceiveCallBack(2, null);//잠금 상태
                return;
            }
            //곡옥 장착인경우 판매금지
            WeaponData chardata = GameInfo.Instance.GetEquipGemWeaponData(gemdata.GemUID);
            if (chardata != null)
            {
                ReceiveCallBack(3, null);//착용 중 아이템
                return;
            }

            gold += gemdata.TableData.SellPrice;
        }

        //곡옥 삭제
        for (int i = 0; i < uidlist.Count; i++)
            DelGem(uidlist[i]);
        //판매금액 지급
        _userdata.AddGoods(eGOODSTYPE.GOLD, gold);

        SaveLocalData();
        ReceiveCallBack(0, null);
    }
    //곡옥 레벨업
    public void Proc_ReqLvUpGem(long gemuid, List<long> matlist, OnReceiveCallBack ReceiveCallBack)
    {
        UserData _userdata = GameInfo.Instance.UserData;

        //무기 보유 확인
        GemData target = GameInfo.Instance.GetGemData(gemuid);
        if (target == null)
        {
            ReceiveCallBack(1, null);
            return;
        }

        //무기 최대레벨 체크
        if (GameSupport.IsMaxLevelGem(target))
        {
            ReceiveCallBack(1, null);
            return;
        }
        //재료 확인
        if (matlist.Count == 0)
        {
            ReceiveCallBack(1, null);
            return;
        }

        //무기 재료 유무 확인
        List<GemData> matitemlist = new List<GemData>();
        for (int i = 0; i < matlist.Count; i++)
        {
            GemData matitem = GameInfo.Instance.GetGemData(matlist[i]);
            if (matitem == null)
            {
                ReceiveCallBack(1, null);
                return;
            }
            else
            {
                matitemlist.Add(matitem);
            }
        }
        //모든 재료의 Exp
        int exp = GameSupport.GetGemMatExp(matitemlist);
        //모든 재료의 필요 골드
        int gold = GameSupport.GetGemLevelUpCost(target, matitemlist);

        //필요 금액 확인
        if (!_userdata.IsGoods(eGOODSTYPE.GOLD, gold))
        {
            ReceiveCallBack(1, null);
            return;
        }

        //최대 경험치에 넘어가면 보정
        int nRemainExp = GameSupport.GetRemainGemExpToMaxLevel(target);
        if (exp >= nRemainExp)
            exp = nRemainExp;

        //경험치 추가
        target.Exp += exp;
        //경험치별 레벨
        target.Level = GameSupport.GetGemExpLevel(target, target.Exp);

        //골드 삭감
        _userdata.SubGoods(eGOODSTYPE.GOLD, gold);

        for (int i = 0; i < matlist.Count; i++)
        {
            DelGem(matlist[i]);
        }

        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    public void Proc_ReqWakeGem(long gemuid, OnReceiveCallBack ReceiveCallBack)
    {
        UserData _userdata = GameInfo.Instance.UserData;
        List<ItemData> _itemlist = GameInfo.Instance.ItemList;

        //무기 보유 확인
        GemData target = GameInfo.Instance.GetGemData(gemuid);
        if (target == null)
        {
            ReceiveCallBack(1, null);
            return;
        }

        //최대 레벨일때만 각성 가능
        if (!GameSupport.IsMaxLevelGem(target))
        {
            ReceiveCallBack(1, null);
            return;
        }
        //최대 각성인지 체크
        if (GameSupport.IsMaxWakeGem(target))
        {
            ReceiveCallBack(1, null);
            return;
        }

        //각성의 필요 재료 얻어오기
        var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == target.TableData.WakeReqGroup && x.Level == target.Wake);
        if (reqdata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }
        //필요 골드 확인
        if (!_userdata.IsGoods(eGOODSTYPE.GOLD, reqdata.Gold))
        {
            ReceiveCallBack(1, null);
            return;
        }

        //재료 체크
        List<MatItemData> matitemlist = new List<MatItemData>();
        List<int> idlist = new List<int>();
        List<int> countlist = new List<int>();
        idlist.Add(reqdata.ItemID1);
        idlist.Add(reqdata.ItemID2);
        idlist.Add(reqdata.ItemID3);
        idlist.Add(reqdata.ItemID4);
        countlist.Add(reqdata.Count1);
        countlist.Add(reqdata.Count2);
        countlist.Add(reqdata.Count3);
        countlist.Add(reqdata.Count4);
        for (int i = 0; i < idlist.Count; i++)
        {
            if (idlist[i] != -1)
            {
                var matitem = _itemlist.Find(x => x.TableID == idlist[i]);
                if (matitem != null)
                {
                    if (countlist[i] > matitem.Count)
                    {
                        ReceiveCallBack(1, null);
                        return;
                    }
                    else
                    {
                        matitemlist.Add(new MatItemData(matitem, countlist[i]));
                    }
                }
                else
                {
                    ReceiveCallBack(1, null);
                    return;
                }
            }
        }

        //재료 아이템 제거
        for (int i = 0; i < matitemlist.Count; i++)
            DelItem(matitemlist[i].ItemData.ItemUID, matitemlist[i].Count);

        //각성 +1
        SetGemOpt(target.GemUID, target.Wake);
        target.Level = 1;
        target.Exp = 0;
        target.Wake += 1;

        //재료 골드 삭감
        _userdata.SubGoods(eGOODSTYPE.GOLD, reqdata.Gold);

        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    public void Proc_ReqResetOptGem(long gemuid, int slot, OnReceiveCallBack ReceiveCallBack)
    {
        UserData _userdata = GameInfo.Instance.UserData;
        List<ItemData> _itemlist = GameInfo.Instance.ItemList;

        //무기 보유 확인
        GemData target = GameInfo.Instance.GetGemData(gemuid);
        if (target == null)
        {
            ReceiveCallBack(1, null);
            return;
        }

        //옵션 변경의 필요 재료 얻어오기
        var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == target.TableData.OptResetReqGroup);
        if (reqdata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }
        //필요 골드 확인
        if (!_userdata.IsGoods(eGOODSTYPE.GOLD, reqdata.Gold))
        {
            ReceiveCallBack(1, null);
            return;
        }

        //재료 체크
        List<MatItemData> matitemlist = new List<MatItemData>();
        List<int> idlist = new List<int>();
        List<int> countlist = new List<int>();
        idlist.Add(reqdata.ItemID1);
        idlist.Add(reqdata.ItemID2);
        idlist.Add(reqdata.ItemID3);
        idlist.Add(reqdata.ItemID4);
        countlist.Add(reqdata.Count1);
        countlist.Add(reqdata.Count2);
        countlist.Add(reqdata.Count3);
        countlist.Add(reqdata.Count4);
        for (int i = 0; i < idlist.Count; i++)
        {
            if (idlist[i] != -1)
            {
                var matitem = _itemlist.Find(x => x.TableID == idlist[i]);
                if (matitem != null)
                {
                    if (countlist[i] > matitem.Count)
                    {
                        ReceiveCallBack(1, null);
                        return;
                    }
                    else
                    {
                        matitemlist.Add(new MatItemData(matitem, countlist[i]));
                    }
                }
                else
                {
                    ReceiveCallBack(1, null);
                    return;
                }
            }
        }

        //재료 아이템 제거
        for (int i = 0; i < matitemlist.Count; i++)
            DelItem(matitemlist[i].ItemData.ItemUID, matitemlist[i].Count);

        //옵션변경
        SetGemOpt(target.GemUID, -1);
        target.TempOptIndex = slot;
        //재료 골드 삭감
        _userdata.SubGoods(eGOODSTYPE.GOLD, reqdata.Gold);

        SaveLocalData();
        ReceiveCallBack(0, null);
    }
    public void Proc_ReqResetOptSelectGem(long gemuid, bool bnew, OnReceiveCallBack ReceiveCallBack)
    {
        //무기 보유 확인
        GemData target = GameInfo.Instance.GetGemData(gemuid);
        if (target == null)
        {
            ReceiveCallBack(1, null);
            return;
        }

        if (target.TempOptIndex == -1)
        {
            ReceiveCallBack(1, null);
            return;
        }

        if (0 > target.TempOptIndex || (int)target.Wake <= target.TempOptIndex)
        {
            ReceiveCallBack(1, null);
            return;
        }


        if (bnew)
        {
            target.RandOptID[target.TempOptIndex] = target.TempOptID;
            target.RandOptValue[target.TempOptIndex] = target.TempOptValue;
        }

        target.TempOptIndex = -1;
        target.TempOptID = -1;
        target.TempOptValue = 0;

        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    //------------------------------------------------------------------------------------------------------------------------------------
    //
    //  카드 관련
    //
    //------------------------------------------------------------------------------------------------------------------------------------
    //카드 장착
    public void Proc_CardCharEquip(long carduid, long charuid, int slot, OnReceiveCallBack ReceiveCallBack)
    {
        //카드 보유 확인
        CardData card = GameInfo.Instance.GetCardData(carduid);
        if (card == null)
        {
            ReceiveCallBack(1, null);//인덱스 이상
            return;
        }

        //캐릭터 보유 확인
        CharData chardata = GameInfo.Instance.GetCharData(charuid);
        if (chardata == null)
        {
            ReceiveCallBack(1, null);//인덱스 이상
            return;
        }

        FacilityData facilitydata = GameInfo.Instance.GetEquiCardFacilityData(carduid);
        if (facilitydata != null)
        {
            if (facilitydata.Stats != (int)eFACILITYSTATS.WAIT)
            {
                ReceiveCallBack(1, null);//인덱스 이상
                return;
            }
        }

        if (card.PosKind == (int)eContentsPosKind.CHAR)
        {
            CharData outchardata = GameInfo.Instance.GetCharData(card.PosValue);
            if (outchardata != null)
            {
                if (outchardata.CUID == chardata.CUID)
                {
                    int outpos = outchardata.GetEquipCardIndex(carduid);
                    outchardata.EquipCard[outpos] = outchardata.EquipCard[slot];
                    CardData outcard = outchardata.GetEquipCard(card.PosValue);
                    if (outcard != null)
                    {
                        outcard.PosValue = outpos;
                    }
                }
                else
                {
                    outchardata.DelEquipCard(carduid);
                }
            }
        }
        else if (card.PosKind == (int)eContentsPosKind.FACILITY)
        {
            for (int i = 0; i < GameInfo.Instance.FacilityList.Count; i++)
            {
                if (GameInfo.Instance.FacilityList[i].EquipCardUID == carduid)
                {
                    if (GameInfo.Instance.FacilityList[i].Stats == (int)eFACILITYSTATS.WAIT)
                    {
                        GameInfo.Instance.FacilityList[i].EquipCardUID = (int)eCOUNT.NONE;
                    }
                }
            }
        }

        card.PosKind = (int)eContentsPosKind.CHAR;
        card.PosValue = charuid;
        card.PosSlot = slot;
        chardata.EquipCard[slot] = carduid;

        SaveLocalData();
        ReceiveCallBack(0, null);
    }
    //카드 장착 해제
    public void Proc_CardCharRemove(long carduid, OnReceiveCallBack ReceiveCallBack)
    {
        //카드 보유 확인
        CardData card = GameInfo.Instance.GetCardData(carduid);
        if (card == null)
        {
            ReceiveCallBack(1, null);
            return;
        }


        CharData outchardata = GameInfo.Instance.GetCharData(card.PosValue);
        if (outchardata != null)
        {
            outchardata.DelEquipCard(carduid);
        }

        card.PosKind = (int)eContentsPosKind._NONE_;
        card.PosValue = 0;
        card.PosSlot = 0;


        SaveLocalData();
        ReceiveCallBack(0, null);
    }
    //카드 판매
    public void Proc_CardSell(List<long> uidlist, OnReceiveCallBack ReceiveCallBack)
    {
        UserData _userdata = GameInfo.Instance.UserData;

        int gold = 0;
        int sp = 0;
        for (int i = 0; i < uidlist.Count; i++)
        {
            //카드 보유 확인
            var carddata = GameInfo.Instance.GetCardData(uidlist[i]);
            if (carddata == null)
            {
                ReceiveCallBack(1, null);
                return;
            }
            //카드 잠김상태면 판재금지
            if (carddata.Lock == true)
            {
                ReceiveCallBack(2, null);
                return;
            }
            //카드 장착중이면 판매 금지
            CharData chardata = GameInfo.Instance.GetEquiCardCharData(carddata.CardUID);
            if (chardata != null)
            {
                ReceiveCallBack(3, null);//착용 중 아이템
                return;
            }

            gold += carddata.TableData.SellPrice;
            sp += carddata.TableData.SellMPoint;
        }

        //카드 삭제
        for (int i = 0; i < uidlist.Count; i++)
            DelCard(uidlist[i]);

        //판매금액 지급
        _userdata.AddGoods(eGOODSTYPE.GOLD, gold);
        _userdata.AddGoods(eGOODSTYPE.SUPPORTERPOINT, sp);

        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    //카드 락, 언락
    public void Proc_CardLock(List<long> carduidlist, List<bool> locklist, OnReceiveCallBack ReceiveCallBack)
    {

        if (carduidlist.Count != locklist.Count)
        {
            ReceiveCallBack(1, null);//인덱스 이상
            return;
        }
        //카드 보유 확인
        for (int i = 0; i < carduidlist.Count; i++)
        {
            var carddata = GameInfo.Instance.GetCardData(carduidlist[i]);
            if (carddata == null)
            {
                ReceiveCallBack(1, null);//인덱스 이상
                return;
            }
        }
        //카드 Lock 상태변경
        for (int i = 0; i < carduidlist.Count; i++)
        {
            var carddata = GameInfo.Instance.GetCardData(carduidlist[i]);
            if (carddata != null)
                carddata.Lock = locklist[i];
        }

        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    //카드 스킬 레벨업
    public void Proc_ReqLvUpCard(long carduid, bool bmatitem, List<long> matlist, List<MatItemData> matitemlist, OnReceiveCallBack ReceiveCallBack)
    {
        UserData _userdata = GameInfo.Instance.UserData;
        GameConfig _gameconfig = GameInfo.Instance.GameConfig;

        //카드 보유 확인
        CardData target = GameInfo.Instance.GetCardData(carduid);
        if (target == null)
        {
            ReceiveCallBack(1, null);
            return;
        }
        CardBookData targetbook = GameInfo.Instance.GetCardBookData(target.TableID);
        if (targetbook == null)
        {
            ReceiveCallBack(1, null);
            return;
        }


        //카드 스킬 최대레벨 확인
        if (GameSupport.IsMaxLevelCard(target))
        {
            ReceiveCallBack(1, null);
            return;
        }

        int exp = 0;
        int gold = 0;

        if (bmatitem)
        {
            //재료 확인
            if (matitemlist.Count == 0)
            {
                ReceiveCallBack(1, null);
                return;
            }
            //무기 재료 유무 확인
            for (int i = 0; i < matitemlist.Count; i++)
            {
                ItemData matitem = GameInfo.Instance.GetItemData(matitemlist[i].ItemData.ItemUID);
                if (matitem == null)
                {
                    ReceiveCallBack(1, null);
                    return;
                }

                if (matitemlist[i].Count > matitem.Count)
                {
                    ReceiveCallBack(1, null);
                    return;
                }
            }

            //모든 재료의 Exp
            exp = GameSupport.GetItemMatExp(matitemlist);
            //모든 재료의 필요 골드
            gold = GameSupport.GetCardLevelUpItemCost(target, matitemlist);

            //필요 금액 확인
            if (!_userdata.IsGoods(eGOODSTYPE.GOLD, gold))
            {
                ReceiveCallBack(1, null);
                return;
            }

            //재료무기 삭제
            for (int i = 0; i < matitemlist.Count; i++)
            {
                DelItem(matitemlist[i].ItemData.ItemUID, matitemlist[i].Count);
                //  주간 미션 갱신(재료 갯수당)
                SetClearMission(eMISSIONTYPE.WM_Con_WpnUpgrade);
            }
        }
        else
        {
            //재료 확인
            if (matlist.Count == 0)
            {
                ReceiveCallBack(1, null);
                return;
            }

            //무기 재료 유무 확인
            List<CardData> matcardlist = new List<CardData>();
            for (int i = 0; i < matlist.Count; i++)
            {
                CardData matitem = GameInfo.Instance.GetCardData(matlist[i]);
                if (matitem == null)
                {
                    ReceiveCallBack(1, null);
                    return;
                }
                else
                {
                    matcardlist.Add(matitem);
                }
            }
            //모든 재료의 Exp
            exp = GameSupport.GetCardMatExp(matcardlist);
            //모든 재료의 필요 골드
            gold = GameSupport.GetCardLevelUpCost(target, matcardlist);

            //필요 금액 확인
            if (!_userdata.IsGoods(eGOODSTYPE.GOLD, gold))
            {
                ReceiveCallBack(1, null);
                return;
            }

            //재료무기 삭제
            for (int i = 0; i < matlist.Count; i++)
            {
                DelCard(matlist[i]);
                //  주간 미션 갱신(재료 갯수당)
                SetClearMission(eMISSIONTYPE.WM_Con_SptUpgrade);
            }
        }

        //최대 경험치에 넘어가면 보정
        int nRemainExp = GameSupport.GetRemainCardExpToMaxLevel(target);
        if (exp >= nRemainExp)
            exp = nRemainExp;

        //경험치 추가
        target.Exp += exp;
        //경험치별 레벨
        target.Level = GameSupport.GetCardExpLevel(target, target.Exp);

        if (GameSupport.IsMaxSkillLevelCard(target))
        {
            target.SkillLv = GameSupport.GetMaxSkillLevelCard();

        }
        if (GameSupport.IsMaxLevelCard(target))
        {
            target.Level = GameSupport.GetMaxLevelCard(target);
        }
        if (GameSupport.IsMaxSkillLevelCard(target) && GameSupport.IsMaxLevelCard(target))
            targetbook.DoOnFlag(eBookStateFlag.MAX_WAKE_AND_LV);

        //골드 삭감
        _userdata.SubGoods(eGOODSTYPE.GOLD, gold);

        SaveLocalData();
        ReceiveCallBack(0, null);

    }

    public void Proc_ReqWakeCard(long carduid, OnReceiveCallBack ReceiveCallBack)
    {
        UserData _userdata = GameInfo.Instance.UserData;
        List<ItemData> _itemlist = GameInfo.Instance.ItemList;

        //무기 보유 확인
        CardData target = GameInfo.Instance.GetCardData(carduid);
        if (target == null)
        {
            ReceiveCallBack(1, null);
            return;
        }

        //최대 레벨일때만 각성 가능
        if (!GameSupport.IsMaxLevelCard(target))
        {
            ReceiveCallBack(1, null);
            return;
        }

        //최대 각성인지 체크
        if (GameSupport.IsMaxWakeCard(target))
        {
            ReceiveCallBack(1, null);
            return;
        }

        //각성의 필요 재료 얻어오기
        var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == target.TableData.WakeReqGroup && x.Level == target.Wake);
        if (reqdata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }
        //필요 골드 확인
        if (!_userdata.IsGoods(eGOODSTYPE.GOLD, reqdata.Gold))
        {
            ReceiveCallBack(1, null);
            return;
        }

        //재료 체크
        List<MatItemData> matitemlist = new List<MatItemData>();
        List<int> idlist = new List<int>();
        List<int> countlist = new List<int>();
        idlist.Add(reqdata.ItemID1);
        idlist.Add(reqdata.ItemID2);
        idlist.Add(reqdata.ItemID3);
        idlist.Add(reqdata.ItemID4);
        countlist.Add(reqdata.Count1);
        countlist.Add(reqdata.Count2);
        countlist.Add(reqdata.Count3);
        countlist.Add(reqdata.Count4);
        for (int i = 0; i < idlist.Count; i++)
        {
            if (idlist[i] != -1)
            {
                var matitem = _itemlist.Find(x => x.TableID == idlist[i]);
                if (matitem != null)
                {
                    if (countlist[i] > matitem.Count)
                    {
                        ReceiveCallBack(1, null);
                        return;
                    }
                    else
                    {
                        matitemlist.Add(new MatItemData(matitem, countlist[i]));
                    }
                }
                else
                {
                    ReceiveCallBack(1, null);
                    return;
                }
            }
        }

        //재료 아이템 제거
        for (int i = 0; i < matitemlist.Count; i++)
            DelItem(matitemlist[i].ItemData.ItemUID, matitemlist[i].Count);

        //각성 +1
        target.Level = 1;
        target.Exp = 0;
        target.Wake += 1;
        var bookCard = GameInfo.Instance.GetCardBookData(target.TableID);
        if (bookCard.StateFlag < 0x00000001 << 0 && target.Wake == 2)
        {
            bookCard.StateFlag = (int)(0x00000001 << 0);
        }

        //재료 골드 삭감
        _userdata.SubGoods(eGOODSTYPE.GOLD, reqdata.Gold);


        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    public void Proc_ReqSkillLvUpCard(long carduid, int mattype, long matuid, OnReceiveCallBack ReceiveCallBack)
    {
        UserData _userdata = GameInfo.Instance.UserData;
        //무기 보유 확인
        CardData target = GameInfo.Instance.GetCardData(carduid);
        if (target == null)
        {
            ReceiveCallBack(1, null);
            return;
        }

        if (GameSupport.IsMaxSkillLevelWeapon(target.SkillLv))
        {
            ReceiveCallBack(1, null);
            return;
        }

        int level = 1;
        int gold = 0;

        if (mattype == (int)MatSkillData.eTYPE.ITEM)
        {
            ItemData mat = GameInfo.Instance.GetItemData(matuid);
            if (mat == null)
            {
                ReceiveCallBack(1, null);
                return;
            }
            gold = GameSupport.GetCardSkillLevelUpCost(target, 1);
            if (!_userdata.IsGoods(eGOODSTYPE.GOLD, gold))
            {
                ReceiveCallBack(1, null);
                return;
            }

            level = mat.TableData.Value;
            DelItem(matuid, 1);
        }
        else
        {
            CardData mat = GameInfo.Instance.GetCardData(matuid);
            if (mat == null)
            {
                ReceiveCallBack(1, null);
                return;
            }
            gold = GameSupport.GetCardSkillLevelUpCost(target, 1);
            if (!_userdata.IsGoods(eGOODSTYPE.GOLD, gold))
            {
                ReceiveCallBack(1, null);
                return;
            }

            level = mat.SkillLv;
            DelCard(matuid);
        }

        target.SkillLv += level;
        if (target.SkillLv >= GameInfo.Instance.GameConfig.CardMaxSkillLevel)
            target.SkillLv = GameInfo.Instance.GameConfig.CardMaxSkillLevel;
        //재료 골드 삭감
        _userdata.SubGoods(eGOODSTYPE.GOLD, gold);

        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    public void Proc_ReqFavorLvRewardCard(int tableid, int level, OnReceiveCallBack ReceiveCallBack)
    {
        var bookCard = GameInfo.Instance.GetCardBookData(tableid);
        if (bookCard == null)
        {
            ReceiveCallBack(1, null);
            return;
        }
        var tableData = GameInfo.Instance.GameTable.FindCard(tableid);
        if (tableData == null)
        {
            ReceiveCallBack(1, null);
            return;
        }
        var levelup = GameInfo.Instance.GameTable.FindLevelUp(x => x.Group == tableData.FavorGroup && x.Level == level);
        if (levelup == null)
        {
            ReceiveCallBack(1, null);
            return;
        }
        if (level > bookCard.FavorLevel)
        {
            ReceiveCallBack(1, null);
            return;
        }

        GameInfo.Instance.RewardList.Clear();
        Purchase_Random(levelup.Value1, 1, ref GameInfo.Instance.RewardList, false);
        bookCard.DoOnFlag(eBookStateFlag.FAVOR_RWD_GET_1 + (level - 1));

        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    //------------------------------------------------------------------------------------------------------------------------------------
    //
    //  비밀 기지
    //
    //------------------------------------------------------------------------------------------------------------------------------------
    //비밀기지 시설 활성화(변경예정)
    public void Proc_FacilityActive(int tableid, OnReceiveCallBack ReceiveCallBack) //시설 활성화 
    {
        UserData _userdata = GameInfo.Instance.UserData;

        FacilityData facilitydata = GameInfo.Instance.GetFacilityData(tableid);
        if (facilitydata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }

        if (_userdata.Level < facilitydata.TableData.FacilityOpenUserRank)
        {
            ReceiveCallBack(1, null);
            return;
        }

        facilitydata.Level = 1;
        facilitydata.Stats = (int)eFACILITYSTATS.WAIT;

        SaveLocalData();
        ReceiveCallBack(0, null);
    }
    //비밀기지 시설 사용(변경예정)
    public void Proc_ReqFacilityOperation(int tableid, long selete, int itemcnt, OnReceiveCallBack ReceiveCallBack) //시설 이용
    {
        UserData _userdata = GameInfo.Instance.UserData;
        List<FacilityData> _facilitylist = GameInfo.Instance.FacilityList;
        List<ItemData> _itemlist = GameInfo.Instance.ItemList;

        FacilityData facilitydata = GameInfo.Instance.GetFacilityData(tableid);
        if (facilitydata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }

        if (facilitydata.Stats != (int)eFACILITYSTATS.WAIT)
        {
            ReceiveCallBack(1, null);
            return;
        }

        if (facilitydata.TableData.EffectType == "FAC_CHAR_EXP")
        {
            for (int i = 0; i < _facilitylist.Count; i++)
            {
                if (_facilitylist[i].TableData.EffectType == "FAC_CHAR_EXP" || _facilitylist[i].TableData.EffectType == "FAC_CHAR_SP")
                {
                    if (_facilitylist[i].Selete == selete)
                    {
                        ReceiveCallBack(1, null);
                        return;
                    }
                }
            }

            CharData chardata = GameInfo.Instance.GetCharData(selete);
            if (chardata == null)
            {
                ReceiveCallBack(1, null);
                return;
            }

            facilitydata.Selete = selete;
        }
        else if (facilitydata.TableData.EffectType == "FAC_CHAR_SP")
        {
            for (int i = 0; i < _facilitylist.Count; i++)
            {
                if (_facilitylist[i].TableData.EffectType == "FAC_CHAR_EXP" || _facilitylist[i].TableData.EffectType == "FAC_CHAR_SP")
                {
                    if (_facilitylist[i].Selete == selete)
                    {
                        ReceiveCallBack(1, null);
                        return;
                    }
                }
            }

            CharData chardata = GameInfo.Instance.GetCharData(selete);
            if (chardata == null)
            {
                ReceiveCallBack(1, null);
                return;
            }

            facilitydata.Selete = selete;
        }
        else if (facilitydata.TableData.EffectType == "FAC_WEAPON_EXP")
        {
            WeaponData weapondata = GameInfo.Instance.GetWeaponData(facilitydata.Selete);
            if (weapondata == null)
            {
                ReceiveCallBack(1, null);
                return;
            }
        }
        else if (facilitydata.TableData.EffectType == "FAC_ITEM_COMBINE")
        {
            var combinedata = GameInfo.Instance.GameTable.FindFacilityItemCombine((int)facilitydata.Selete);
            if (combinedata == null)
            {
                ReceiveCallBack(1, null);
                return;
            }

            var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == combinedata.ItemReqGroup);
            if (reqdata == null)
            {
                ReceiveCallBack(1, null);
                return;
            }
            if (!_userdata.IsGoods(eGOODSTYPE.GOLD, reqdata.Gold * itemcnt)) //금액부족
            {
                ReceiveCallBack(1, null);
                return;
            }
            List<MatItemData> matitemlist = new List<MatItemData>();
            List<int> idlist = new List<int>();
            List<int> countlist = new List<int>();
            idlist.Add(reqdata.ItemID1);
            idlist.Add(reqdata.ItemID2);
            idlist.Add(reqdata.ItemID3);
            idlist.Add(reqdata.ItemID4);
            countlist.Add(reqdata.Count1);
            countlist.Add(reqdata.Count2);
            countlist.Add(reqdata.Count3);
            countlist.Add(reqdata.Count4);

            for (int i = 0; i < idlist.Count; i++)
            {
                if (idlist[i] != -1)
                {
                    var matitem = _itemlist.Find(x => x.TableID == idlist[i]);
                    if (matitem != null)
                    {
                        if (countlist[i] * itemcnt > matitem.Count)
                        {
                            ReceiveCallBack(1, null);
                            return;
                        }
                        else
                        {
                            matitemlist.Add(new MatItemData(matitem, countlist[i] * itemcnt));
                        }
                    }
                    else
                    {
                        ReceiveCallBack(1, null);
                        return;
                    }
                }
            }


            for (int i = 0; i < matitemlist.Count; i++)
                DelItem(matitemlist[i].ItemData.ItemUID, matitemlist[i].Count);

            _userdata.SubGoods(eGOODSTYPE.GOLD, reqdata.Gold * itemcnt);
            facilitydata.RemainTime = GameSupport.GetCurrentServerTime().AddSeconds(GameSupport.GetFacilityItemCombineTime(combinedata.Time * 60 * itemcnt, facilitydata));
        }

        float min = GameSupport.GetFacilityTime(facilitydata);
        facilitydata.Stats = (int)eFACILITYSTATS.USE;

        if (facilitydata.TableData.EffectType != "FAC_ITEM_COMBINE")
        {
            facilitydata.RemainTime = GameSupport.GetCurrentServerTime().AddMinutes(min);
        }

        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    public void Proc_ReqFacilityComplete(int tableid, long speedItemid, OnReceiveCallBack ReceiveCallBack) //시설 사용 완료
    {
        UserData _userdata = GameInfo.Instance.UserData;
        FacilityResultData FacilityResultData = GameInfo.Instance.FacilityResultData;

        FacilityData facilitydata = GameInfo.Instance.GetFacilityData(tableid);
        if (facilitydata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }

        if (facilitydata.Stats != (int)eFACILITYSTATS.USE)
        {
            ReceiveCallBack(1, null);
            return;
        }

        if (speedItemid != 0)
        {
            int speedCnt = GameSupport.GetFacilityNeedSpeedItem(facilitydata);

            var itemData = GameInfo.Instance.GetItemData((int)speedItemid);
            if (itemData == null)
            {
                Log.Show(speedItemid + " / Item is null");
                ReceiveCallBack(1, null);
                return;
            }

            DelItem(itemData.TableID, speedCnt);
        }
        else
        {
            var diffTime = facilitydata.RemainTime - GameSupport.GetCurrentServerTime();

            if (diffTime.Ticks > 0)
            {
                ReceiveCallBack(1, null);
                return;
            }
        }

        GameInfo.Instance.FacilityResultData.Init();
        GameInfo.Instance.FacilityResultData.FacilityID = tableid;

        if (facilitydata.TableData.EffectType == "FAC_CHAR_EXP")
        {
            CharData chardata = GameInfo.Instance.GetCharData(facilitydata.Selete);
            if (chardata == null)
            {
                Log.Show("CharData is NULL", Log.ColorType.Red);
                ReceiveCallBack(1, null);
                return;
            }

            GameInfo.Instance.FacilityResultData.TargetUID = facilitydata.Selete;
            GameInfo.Instance.FacilityResultData.TargetBeforeLevel = chardata.Level;
            GameInfo.Instance.FacilityResultData.TargetBeforeExp = chardata.Exp;

            AddCharExp(ref chardata, facilitydata.GetEffectValue());

            GameInfo.Instance.FacilityResultData.TargetAfterLevel = chardata.Level;
            GameInfo.Instance.FacilityResultData.TargetAfterExp = chardata.Exp;

        }
        else if (facilitydata.TableData.EffectType == "FAC_CHAR_SP")
        {
            CharData chardata = GameInfo.Instance.GetCharData(facilitydata.Selete);
            if (chardata == null)
            {
                Log.Show("CharData is NULL", Log.ColorType.Red);
                ReceiveCallBack(1, null);
                return;
            }

            GameInfo.Instance.FacilityResultData.TargetUID = facilitydata.Selete;
            GameInfo.Instance.FacilityResultData.TargetBeforeLevel = chardata.PassviePoint;

            chardata.PassviePoint += facilitydata.GetEffectValue();

            GameInfo.Instance.FacilityResultData.TargetAfterLevel = facilitydata.GetEffectValue();
        }
        else if (facilitydata.TableData.EffectType == "FAC_WEAPON_EXP")
        {
            WeaponData weapondata = GameInfo.Instance.GetWeaponData(facilitydata.Selete);
            if (weapondata == null)
            {
                ReceiveCallBack(1, null);
                return;
            }

            GameInfo.Instance.FacilityResultData.TargetUID = facilitydata.Selete;
            GameInfo.Instance.FacilityResultData.TargetBeforeLevel = weapondata.Level;
            GameInfo.Instance.FacilityResultData.TargetBeforeExp = weapondata.Exp;

            AddWeaponExp(ref weapondata, facilitydata.GetEffectValue());

            GameInfo.Instance.FacilityResultData.TargetAfterLevel = weapondata.Level;
            GameInfo.Instance.FacilityResultData.TargetAfterExp = weapondata.Exp;
        }
        else if (facilitydata.TableData.EffectType == "FAC_ITEM_COMBINE")
        {
            var combinedata = GameInfo.Instance.GameTable.FindFacilityItemCombine((int)facilitydata.Selete);
            if (combinedata == null)
            {
                ReceiveCallBack(1, null);
                return;
            }

            long itemuid = AddItem(combinedata.ItemID, combinedata.ItemCnt * facilitydata.OperationCnt);

            GameInfo.Instance.FacilityResultData.TargetUID = itemuid;
            GameInfo.Instance.FacilityResultData.TargetAfterLevel = combinedata.ItemCnt * facilitydata.OperationCnt;

            //시설 아이템 조합은 아이템 갯수에 따라 호감도 셩험치가 오르기때문에 따로 적용.
            if (facilitydata.EquipCardUID != (int)eCOUNT.NONE)
            {
                CardData equipCard = GameInfo.Instance.GetCardData(facilitydata.EquipCardUID);
                if (equipCard == null)
                {
                    ReceiveCallBack(1, null);
                    return;
                }
                CardBookData cardBookData = GameInfo.Instance.GetCardBookData(equipCard.TableID);
                if (cardBookData != null)
                {
                    GameInfo.Instance.FacilityResultData.CardUID = equipCard.CardUID;
                    GameInfo.Instance.FacilityResultData.CardBeforeLevel = cardBookData.FavorLevel;
                    GameInfo.Instance.FacilityResultData.CardBeforeExp = cardBookData.FavorExp;

                    AddCardFavorExp(equipCard.TableID, combinedata.RewardCardFavor * facilitydata.OperationCnt);

                    GameInfo.Instance.FacilityResultData.CardAfterLevel = cardBookData.FavorLevel;
                    GameInfo.Instance.FacilityResultData.CardAfterExp = cardBookData.FavorExp;
                }
            }


            //  주간미션 시설 아이템 조합(조합 횟수당)
            SetClearMission(eMISSIONTYPE.WM_Con_FacilityGetItem);
        }

        //시설 아이템 조합 이외의 시설들은 서포터 호감도 경험치가 정해져있어서 한번에 처리.
        if (facilitydata.EquipCardUID != (int)eCOUNT.NONE && facilitydata.TableData.EffectType != "FAC_ITEM_COMBINE")
        {
            CardData equipCard = GameInfo.Instance.GetCardData(facilitydata.EquipCardUID);
            if (equipCard == null)
            {
                ReceiveCallBack(1, null);
                return;
            }
            CardBookData cardBookData = GameInfo.Instance.GetCardBookData(equipCard.TableID);
            if (cardBookData != null)
            {
                GameInfo.Instance.FacilityResultData.CardUID = equipCard.CardUID;
                GameInfo.Instance.FacilityResultData.CardBeforeLevel = cardBookData.FavorLevel;
                GameInfo.Instance.FacilityResultData.CardBeforeExp = cardBookData.FavorExp;

                AddCardFavorExp(equipCard.TableID, facilitydata.TableData.RewardCardFavor);

                GameInfo.Instance.FacilityResultData.CardAfterLevel = cardBookData.FavorLevel;
                GameInfo.Instance.FacilityResultData.CardAfterExp = cardBookData.FavorExp;
            }

        }

        facilitydata.Stats = (int)eFACILITYSTATS.WAIT;
        facilitydata.Selete = (int)eCOUNT.NONE;


        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    public void Proc_ReqFacilityCancel(int tableid, OnReceiveCallBack ReceiveCallBack) //시설 사용 취소
    {
        FacilityData facilitydata = GameInfo.Instance.GetFacilityData(tableid);
        if (facilitydata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }

        if (facilitydata.Stats != (int)eFACILITYSTATS.USE)
        {
            ReceiveCallBack(1, null);
            return;
        }

        var diffTime = facilitydata.RemainTime - GameSupport.GetCurrentServerTime();
        if (diffTime.Ticks < 0)
        {
            ReceiveCallBack(1, null);
            return;
        }

        //아이템 조합 - 취소 시 재화 및 아이템 복구
        if (facilitydata.TableData.EffectType == "FAC_ITEM_COMBINE")
        {
            UserData _userdata = GameInfo.Instance.UserData;
            List<ItemData> _itemlist = GameInfo.Instance.ItemList;
            var combinedata = GameInfo.Instance.GameTable.FindFacilityItemCombine((int)facilitydata.Selete);
            if (combinedata == null)
            {
                ReceiveCallBack(1, null);
                return;
            }

            var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == combinedata.ItemReqGroup);
            if (reqdata == null)
            {
                ReceiveCallBack(1, null);
                return;
            }

            List<MatItemData> matitemlist = new List<MatItemData>();
            List<int> idlist = new List<int>();
            List<int> countlist = new List<int>();
            idlist.Add(reqdata.ItemID1);
            idlist.Add(reqdata.ItemID2);
            idlist.Add(reqdata.ItemID3);
            idlist.Add(reqdata.ItemID4);
            countlist.Add(reqdata.Count1);
            countlist.Add(reqdata.Count2);
            countlist.Add(reqdata.Count3);
            countlist.Add(reqdata.Count4);

            for (int i = 0; i < idlist.Count; i++)
            {
                if (idlist[i] != -1)
                {
                    AddItem(idlist[i], countlist[i] * facilitydata.OperationCnt);
                }
            }

            _userdata.AddGoods(eGOODSTYPE.GOLD, reqdata.Gold * facilitydata.OperationCnt);
        }

        facilitydata.Stats = (int)eFACILITYSTATS.WAIT;
        facilitydata.Selete = 0;

        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    public void Proc_ReqFacilityCardEquip(long carduid, int tableid, eContentsPosKind poskind, OnReceiveCallBack ReceiveCallBack)
    {
        List<FacilityData> _facilitylist = GameInfo.Instance.FacilityList;

        FacilityData facilitydata = GameInfo.Instance.GetFacilityData(tableid);
        if (facilitydata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }
        if (facilitydata.Stats != (int)eFACILITYSTATS.WAIT)
        {
            ReceiveCallBack(1, null);
            return;
        }

        CharData chardata = GameInfo.Instance.GetEquiCardCharData(carduid);
        if (chardata != null)
        {
            ReceiveCallBack(1, null);
            return;
        }

        CardData carddata = GameInfo.Instance.GetCardData(carduid);
        if (carddata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }

        for (int i = 0; i < _facilitylist.Count; i++)
        {
            if (_facilitylist[i].EquipCardUID == carduid)
            {
                ReceiveCallBack(1, null);
                return;
            }
        }
        carddata.PosKind = (int)poskind;
        facilitydata.EquipCardUID = carduid;

        SaveLocalData();
        ReceiveCallBack(0, null);
    }
    public void Proc_ReqFacilityCardRemove(long carduid, OnReceiveCallBack ReceiveCallBack)
    {
        CardData carddata = GameInfo.Instance.GetCardData(carduid);
        if (carddata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }
        FacilityData facilitydata = GameInfo.Instance.GetEquiCardFacilityData(carduid);
        if (facilitydata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }
        if (facilitydata.Stats != (int)eFACILITYSTATS.WAIT)
        {
            ReceiveCallBack(1, null);
            return;
        }
        carddata.PosKind = (int)eContentsPosKind._NONE_;
        facilitydata.EquipCardUID = (int)eCOUNT.NONE;

        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    public void Proc_FacilityItemEquip(int tableid, long itemid, int itemcnt, OnReceiveCallBack ReceiveCallBack)
    {
        FacilityData facilitydata = GameInfo.Instance.GetFacilityData(tableid);
        if (facilitydata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }
        if (facilitydata.Stats != (int)eFACILITYSTATS.WAIT)
        {
            ReceiveCallBack(1, null);
            return;
        }
        if (facilitydata.TableData.EffectType == "FAC_ITEM_COMBINE")
        {
            GameTable.FacilityItemCombine.Param combindata = GameInfo.Instance.GameTable.FindFacilityItemCombine((int)itemid);
            if (combindata == null)
            {
                ReceiveCallBack(1, null);
                return;
            }
            facilitydata.OperationCnt = itemcnt;
            facilitydata.Selete = itemid;
        }
        else if (facilitydata.TableData.EffectType == "FAC_WEAPON_EXP")
        {
            //중복검사
            facilitydata.Selete = itemid;
        }
        else
        {
            ReceiveCallBack(1, null);
            return;
        }

        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    public void Proc_FacilityItemRemove(int tableid, OnReceiveCallBack ReceiveCallBack)
    {
        FacilityData facilitydata = GameInfo.Instance.GetFacilityData(tableid);
        if (facilitydata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }
        if (facilitydata.Stats != (int)eFACILITYSTATS.WAIT)
        {
            ReceiveCallBack(1, null);
            return;
        }
        if (facilitydata.TableData.EffectType != "FAC_ITEM_COMBINE")
        {
            ReceiveCallBack(1, null);
            return;
        }

        facilitydata.Selete = (int)eCOUNT.NONE;

        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    //비밀기지 시설 레벨업
    public void Proc_ReqFacilityUpgrade(int tableid, OnReceiveCallBack ReceiveCallBack) //시설 업그레이드
    {
        UserData userdata = GameInfo.Instance.UserData;
        List<ItemData> itemlist = GameInfo.Instance.ItemList;

        FacilityData facilitydata = GameInfo.Instance.GetFacilityData(tableid);
        if (facilitydata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }
        var reqdata = GameInfo.Instance.GameTable.FindItemReqList(x => x.Group == facilitydata.TableData.LevelUpItemReq && x.Level == facilitydata.Level);
        if (reqdata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }
        if (!userdata.IsGoods(eGOODSTYPE.GOLD, reqdata.Gold)) //금액부족
        {
            ReceiveCallBack(1, null);
            return;
        }
        List<MatItemData> matitemlist = new List<MatItemData>();
        List<int> idlist = new List<int>();
        List<int> countlist = new List<int>();
        idlist.Add(reqdata.ItemID1);
        idlist.Add(reqdata.ItemID2);
        idlist.Add(reqdata.ItemID3);
        idlist.Add(reqdata.ItemID4);
        countlist.Add(reqdata.Count1);
        countlist.Add(reqdata.Count2);
        countlist.Add(reqdata.Count3);
        countlist.Add(reqdata.Count4);

        //레벨업에 필요한 재료 아이템 셋팅
        for (int i = 0; i < idlist.Count; i++)
        {
            if (idlist[i] != -1)
            {
                var matitem = itemlist.Find(x => x.TableID == idlist[i]);
                if (matitem != null)
                {
                    if (countlist[i] > matitem.Count)
                    {
                        ReceiveCallBack(1, null);
                        return;
                    }
                    else
                    {
                        matitemlist.Add(new MatItemData(matitem, countlist[i]));
                    }
                }
                else
                {
                    ReceiveCallBack(1, null);
                    return;
                }
            }
        }
        //재료 아이템 감소
        for (int i = 0; i < matitemlist.Count; i++)
            DelItem(matitemlist[i].ItemData.ItemUID, matitemlist[i].Count);

        //재화 감소
        userdata.SubGoods(eGOODSTYPE.GOLD, reqdata.Gold);

        //레벨증가
        var list = GameInfo.Instance.FacilityList.FindAll(x => x.TableData.ParentsID == facilitydata.TableID);
        for (int i = 0; i < list.Count; i++)
            list[i].Level += 1;

        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    //=============================================================================================
    // 피규어 룸
    //=============================================================================================
    //787878
    public void Proc_SetMainRoomTheme(int roomthemeslotnum, OnReceiveCallBack ReceiveCallBack)
    {
        SaveLocalData();
        ReceiveCallBack(0, null);
    }
    public void Proc_RoomPurchase(int storeroomtid, OnReceiveCallBack ReceiveCallBack)
    {
        UserData _userdata = GameInfo.Instance.UserData;
        GameTable _gametable = GameInfo.Instance.GameTable;
        //스테이지 테이블 확인
        GameTable.StoreRoom.Param storeroomdata = _gametable.FindStoreRoom(x => x.ID == storeroomtid);
        if (storeroomdata == null)
        {
            ReceiveCallBack(1, null);//인덱스 이상
            return;
        }

        if (!_userdata.IsGoods((eGOODSTYPE)storeroomdata.PurchaseType, storeroomdata.PurchaseValue)) //재화부족
        {
            ReceiveCallBack(1, null);//인덱스 이상
            return;
        }

        if (!_userdata.IsGoods(eGOODSTYPE.ROOMPOINT, storeroomdata.NeedRoomPoint)) //재화부족
        {
            ReceiveCallBack(1, null);//인덱스 이상
            return;
        }

        int result = -1;
        if ((eREWARDTYPE)storeroomdata.ProductType == eREWARDTYPE.ROOMTHEME)           //가챠
        {
            result = AddRoomTheme(storeroomdata.ProductIndex);
        }
        else if ((eREWARDTYPE)storeroomdata.ProductType == eREWARDTYPE.ROOMFUNC)
        {
            result = AddRoomFunc(storeroomdata.ProductIndex);
        }
        else if ((eREWARDTYPE)storeroomdata.ProductType == eREWARDTYPE.ROOMACTION)
        {
            result = AddRoomAction(storeroomdata.ProductIndex);
        }
        else if ((eREWARDTYPE)storeroomdata.ProductType == eREWARDTYPE.ROOMFIGURE)
        {
            result = AddRoomFigure(storeroomdata.ProductIndex);
        }

        if (result == -1)
        {
            ReceiveCallBack(1, null);//인덱스 이상
            return;
        }

        _userdata.SubGoods((eGOODSTYPE)storeroomdata.PurchaseType, storeroomdata.PurchaseValue);
        _userdata.SubGoods(eGOODSTYPE.ROOMPOINT, storeroomdata.NeedRoomPoint);

        SaveLocalData();
        ReceiveCallBack(0, null);
    }
    public void Proc_RoomThemeSlotDetailInfo(int roomthemeslotnum, OnReceiveCallBack ReceiveCallBack)
    {
        SaveLocalData();
        ReceiveCallBack(0, null);
    }
    public void Proc_RoomThemeSlotSave(RoomThemeSlotData roomslotdata, List<RoomThemeFigureSlotData> roomfigureslotlist, OnReceiveCallBack ReceiveCallBack)
    {
        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    public void Proc_ReqBookNewConfirm(int type, int tableid, OnReceiveCallBack ReceiveCallBack)
    {
        List<WeaponBookData> _weaponbooklist = GameInfo.Instance.WeaponBookList;
        List<CardBookData> _cardbooklist = GameInfo.Instance.CardBookList;
        List<MonsterBookData> _monsterbooklist = GameInfo.Instance.MonsterBookList;

        if (type == 1)
        {
            var bookdata = _weaponbooklist.Find(x => x.TableID == tableid);
            if (bookdata == null)
            {
                ReceiveCallBack(1, null); //인덱스 이상
                return;
            }

            bookdata.DoOnFlag(eBookStateFlag.NEW_CHK);
        }
        else if (type == 2)
        {
            var bookdata = _cardbooklist.Find(x => x.TableID == tableid);
            if (bookdata == null)
            {
                ReceiveCallBack(1, null); //인덱스 이상
                return;
            }
            bookdata.DoOnFlag(eBookStateFlag.NEW_CHK);
        }
        else if (type == 3)
        {
            var bookdata = _monsterbooklist.Find(x => x.TableID == tableid);
            if (bookdata == null)
            {
                ReceiveCallBack(1, null); //인덱스 이상
                return;
            }
            bookdata.DoOnFlag(eBookStateFlag.NEW_CHK);
        }


        SaveLocalData();
        ReceiveCallBack(0, null);
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //
    //  상점
    //
    //------------------------------------------------------------------------------------------------------------------------------------
    //판매
    public void Proc_Purchase(int storeid, bool bfree, OnReceiveCallBack ReceiveCallBack)
    {
        UserData _userdata = GameInfo.Instance.UserData;
        GameTable _gametable = GameInfo.Instance.GameTable;
        //77887788  List<FreeGachaData> _freegachalist = GameInfo.Instance.FreeGachaList;
        //storeid, 상점아이디
        //bfree, 무료인지

        //상점 ID 체크
        GameTable.Store.Param storedata = _gametable.FindStore(storeid);
        if (storedata == null)
            return;

        //  * 확인 필수 *
        //storedata.ProductType
        //storedata.PurchaseType

        //상점 세일상품인지 체크후 판매재화및 수량 변경
        int PurchaseValue = storedata.PurchaseValue;   //재화 수량
        int ProductValue = storedata.ProductValue;      //상품 수량
        //77887788
        /*
        var _storesaledata = GameInfo.Instance.ServerData.StoreSaleList.Find(x => x.TableID == storedata.ID);
        if (_storesaledata != null)
        {
            if (_storesaledata.ProductValue != 0)
                ProductValue = _storesaledata.ProductValue;
            if (_storesaledata.PurchaseValue != 0)
                PurchaseValue = _storesaledata.PurchaseValue;
        }
        */
        if (storedata.ProductType == 0)  //  가챠타입인 경우
        {
            //  가챠의 결과 갯수 만큼 미션 갱신
            for (int i = 0; i < ProductValue; i++)
                SetClearMission(eMISSIONTYPE.WM_Con_GachaGetItem);
        }

        GameInfo.Instance.RewardList.Clear(); //클라용

        long uid = -1;
        //무료가챠인경우 무료중인지 확인
        if (bfree)
        {
            //77887788
            /*
            var freegacha = _freegachalist.Find(x => x.TableID == storedata.ID);
            if (freegacha == null)
                return;
               
            var diffTime = freegacha.RemainTime - DateTime.Now;
            if (diffTime.Ticks >= 0)
            {
                return;
            }
             */
        }
        else
        {
            if ((eREWARDTYPE)storedata.PurchaseType == eREWARDTYPE.GOODS)
            {
                //내부 재화사용시 재화 체크
                if ((eGOODSTYPE)storedata.PurchaseIndex != eGOODSTYPE.GOODS)
                {
                    if (!_userdata.IsGoods((eGOODSTYPE)storedata.PurchaseIndex, PurchaseValue)) //재화부족
                        return;
                }
            }
            else if ((eREWARDTYPE)storedata.PurchaseType == eREWARDTYPE.ITEM)
            {
                int count = GameInfo.Instance.GetItemIDCount(storedata.PurchaseIndex);
                if (PurchaseValue > count)
                {
                    return;
                }
            }

        }

        //상품별 처리
        if ((eREWARDTYPE)storedata.ProductType == eREWARDTYPE.GACHA)           //가챠
        {
            int normalCnt = storedata.ProductValue;

            // 확정 드랍 값이 설정돼있으면 관련 값을 처리합니다.
            if (0 < storedata.Value1)
            {
                normalCnt -= 1;
                Purchase_Random(storedata.Value1, 1, ref GameInfo.Instance.RewardList, false);
            }
            // 낮은 등급 하나를 높은 등급으로 올리는 확율이 설정돼있으면 관련 값을 처리합니다.
            if (0 < storedata.Value2)
            {
                if (UnityEngine.Random.Range(0, 100) < storedata.Value2)
                {
                    normalCnt -= 1;
                    Purchase_Random(storedata.Value3, 1, ref GameInfo.Instance.RewardList, true);
                }
            }

            Purchase_Random(storedata.ProductIndex, normalCnt, ref GameInfo.Instance.RewardList, false);

            if (storedata.BonusGoodsType == (int)eGOODSTYPE.SUPPORTERPOINT)
                GameInfo.Instance.RewardGachaSupporterPoint = storedata.BonusGoodsValue;
            else
                GameInfo.Instance.RewardGachaSupporterPoint = 0;
        }
        else if ((eREWARDTYPE)storedata.ProductType == eREWARDTYPE.PACKAGE)        //묶음상품
        {
            Purchase_Package(storedata.ProductIndex, ref GameInfo.Instance.RewardList);
        }
        else
        {
            if ((eREWARDTYPE)storedata.ProductType == eREWARDTYPE.GOODS)            //재화구매
            {
                _userdata.AddGoods((eGOODSTYPE)storedata.ProductIndex, ProductValue);
                if (storedata.ProductIndex == (int)eGOODSTYPE.CASH)
                    _userdata.HardCash += storedata.ProductValue;
            }
            else if ((eREWARDTYPE)storedata.ProductType == eREWARDTYPE.CHAR)        //캐릭터구매
            {
                uid = AddChar(storedata.ProductIndex);
                //GameInfo.Instance.RewardList.Add(new RewardData(uid, storedata.ProductType, storedata.ProductIndex, ProductValue, false)); 

            }
            else if ((eREWARDTYPE)storedata.ProductType == eREWARDTYPE.WEAPON)      //무기구매
            {
                uid = AddWeapon(storedata.ProductIndex);
                //GameInfo.Instance.RewardList.Add(new RewardData(uid, storedata.ProductType, storedata.ProductIndex, ProductValue, false)); 

            }
            else if ((eREWARDTYPE)storedata.ProductType == eREWARDTYPE.GEM)         //곡옥구매
            {
                uid = AddGem(storedata.ProductIndex);
                //GameInfo.Instance.RewardList.Add(new RewardData(uid, storedata.ProductType, storedata.ProductIndex, ProductValue, false)); 

            }
            else if ((eREWARDTYPE)storedata.ProductType == eREWARDTYPE.CARD)         //카드구매
            {
                uid = AddCard(storedata.ProductIndex);
                //GameInfo.Instance.RewardList.Add(new RewardData(uid, storedata.ProductType, storedata.ProductIndex, ProductValue, false));

            }
            else if ((eREWARDTYPE)storedata.ProductType == eREWARDTYPE.COSTUME)     //코스튬구매
            {
                uid = AddCostume(storedata.ProductIndex);
                //GameInfo.Instance.RewardList.Add(new RewardData(uid, storedata.ProductType, storedata.ProductIndex, ProductValue, false));

            }
            else if ((eREWARDTYPE)storedata.ProductType == eREWARDTYPE.ITEM)        //아이템구매
            {
                uid = AddItem(storedata.ProductIndex, ProductValue);
                //GameInfo.Instance.RewardList.Add(new RewardData(uid, storedata.ProductType, storedata.ProductIndex, ProductValue, false));
            }
            else
            {
                uid = 0;
                Debug.LogWarning("Not eRewardType");
            }

            RewardData reward = new RewardData(uid, storedata.ProductType, storedata.ProductIndex, ProductValue, false);
            GameInfo.Instance.RewardList.Add(reward);
        }

        //현금결재
        if ((eREWARDTYPE)storedata.PurchaseType == eREWARDTYPE.GOODS && (eGOODSTYPE)storedata.PurchaseIndex == eGOODSTYPE.GOODS)
        {
            //현금결재 로그
            //78787 Tapjoy.TrackPurchase(response.productIdentifier, "KRW", value );
        }
        else
        {
            //무료구매인경우 무료 시간 리셋
            if (bfree)
            {
                //77887788
                /*
                var freegacha = _freegachalist.Find(x => x.TableID == storedata.ID);
                if (freegacha == null)
                    return;
                   
                freegacha.ResetTime();
                 */
            }
            else
            {
                if ((eREWARDTYPE)storedata.PurchaseType == eREWARDTYPE.GOODS)
                {
                    _userdata.SubGoods((eGOODSTYPE)storedata.PurchaseIndex, PurchaseValue);
                }
                else if ((eREWARDTYPE)storedata.PurchaseType == eREWARDTYPE.ITEM)
                {
                    var itemdata = GameInfo.Instance.GetItemData(storedata.PurchaseIndex);
                    if (itemdata != null)
                    {
                        DelItem(itemdata.ItemUID, PurchaseValue);
                    }
                }

                if ((int)eGOODSTYPE.NONE < storedata.BonusGoodsType && storedata.BonusGoodsType < (int)eGOODSTYPE.COUNT)
                    _userdata.AddGoods((eGOODSTYPE)storedata.BonusGoodsType, storedata.BonusGoodsValue);
            }
        }

        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    //패키지 구매 Send_Purchase 사용
    private bool Purchase_Package(int packageid, ref List<RewardData> rewardlist)
    {
        UserData _userdata = GameInfo.Instance.UserData;
        GameTable _gametable = GameInfo.Instance.GameTable;

        var list = _gametable.FindAllRandom(x => x.GroupID == packageid);
        if (list.Count == 0)
            return false;


        for (int i = 0; i < list.Count; i++)
        {
            long uid = -1;
            if ((eREWARDTYPE)list[i].ProductType == eREWARDTYPE.GOODS)     //재화
            {
                _userdata.AddGoods((eGOODSTYPE)list[i].ProductIndex, list[i].ProductValue);
                rewardlist.Add(new RewardData(uid, list[i].ProductType, list[i].ProductIndex, list[i].ProductValue, false));
            }
            else if ((eREWARDTYPE)list[i].ProductType == eREWARDTYPE.CHAR)           //캐릭터
            {
                uid = AddChar(list[i].ProductIndex);
                if (uid != -1)
                    rewardlist.Add(new RewardData(uid, list[i].ProductType, list[i].ProductIndex, list[i].ProductValue, false));
            }
            else if ((eREWARDTYPE)list[i].ProductType == eREWARDTYPE.WEAPON)           //무기
            {
                uid = AddWeapon(list[i].ProductIndex);
                if (uid != -1)
                    rewardlist.Add(new RewardData(uid, list[i].ProductType, list[i].ProductIndex, list[i].ProductValue, false));
            }
            else if ((eREWARDTYPE)list[i].ProductType == eREWARDTYPE.GEM)           //곡옥
            {
                uid = AddGem(list[i].ProductIndex);
                if (uid != -1)
                    rewardlist.Add(new RewardData(uid, list[i].ProductType, list[i].ProductIndex, list[i].ProductValue, false));
            }
            else if ((eREWARDTYPE)list[i].ProductType == eREWARDTYPE.CARD)           //카드
            {
                uid = AddCard(list[i].ProductIndex);
                if (uid != -1)
                    rewardlist.Add(new RewardData(uid, list[i].ProductType, list[i].ProductIndex, list[i].ProductValue, false));
            }
            else if ((eREWARDTYPE)list[i].ProductType == eREWARDTYPE.COSTUME)        //코스튬
            {
                uid = AddCostume(list[i].ProductIndex);
                if (uid != -1)
                    rewardlist.Add(new RewardData(uid, list[i].ProductType, list[i].ProductIndex, list[i].ProductValue, false));
            }
            else if ((eREWARDTYPE)list[i].ProductType == eREWARDTYPE.ITEM)           //아이템
            {
                uid = AddItem(list[i].ProductIndex, list[i].ProductValue);
                if (uid != -1)
                    rewardlist.Add(new RewardData(uid, list[i].ProductType, list[i].ProductIndex, list[i].ProductValue, false));
            }

            else if ((eREWARDTYPE)list[i].ProductType == eREWARDTYPE.GACHA)         //가챠
            {
                Purchase_Random(list[i].ProductIndex, list[i].ProductValue, ref rewardlist, false);
            }
        }
        return true;
    }

    //랜덤 구매 Send_Purchase 사용 (가챠등)
    private void Purchase_Random(int groupid, int count, ref List<RewardData> rewardlist, bool changeGrade)
    {
        UserData _userdata = GameInfo.Instance.UserData;
        GameTable _gametable = GameInfo.Instance.GameTable;

        var list = _gametable.FindAllRandom(x => x.GroupID == groupid);

        for (int i = 0; i < count; i++)
        {
            var dropdata = GetStoreDropRandom(ref list);
            if( dropdata == null ) {
                continue;
			}

            long uid = -1;

            if ((eREWARDTYPE)dropdata.ProductType == eREWARDTYPE.GOODS)     //재화
            {
                _userdata.AddGoods((eGOODSTYPE)dropdata.ProductIndex, dropdata.ProductValue);
                rewardlist.Add(new RewardData(uid, dropdata.ProductType, dropdata.ProductIndex, dropdata.ProductValue, false));
            }
            else if ((eREWARDTYPE)dropdata.ProductType == eREWARDTYPE.CHAR)           //캐릭터
            {
                uid = AddChar(dropdata.ProductIndex);
                if (uid != -1)
                    rewardlist.Add(new RewardData(uid, dropdata.ProductType, dropdata.ProductIndex, dropdata.ProductValue, false));
            }
            else if ((eREWARDTYPE)dropdata.ProductType == eREWARDTYPE.WEAPON)           //무기
            {
                uid = AddWeapon(dropdata.ProductIndex);
                if (uid != -1)
                    rewardlist.Add(new RewardData(uid, dropdata.ProductType, dropdata.ProductIndex, dropdata.ProductValue, changeGrade));
            }
            else if ((eREWARDTYPE)dropdata.ProductType == eREWARDTYPE.GEM)           //곡옥
            {
                uid = AddGem(dropdata.ProductIndex);
                if (uid != -1)
                    rewardlist.Add(new RewardData(uid, dropdata.ProductType, dropdata.ProductIndex, dropdata.ProductValue, changeGrade));
            }
            else if ((eREWARDTYPE)dropdata.ProductType == eREWARDTYPE.CARD)           //카드
            {
                uid = AddCard(dropdata.ProductIndex);
                if (uid != -1)
                    rewardlist.Add(new RewardData(uid, dropdata.ProductType, dropdata.ProductIndex, dropdata.ProductValue, changeGrade));
            }
            else if ((eREWARDTYPE)dropdata.ProductType == eREWARDTYPE.ITEM)           //아이템
            {
                uid = AddItem(dropdata.ProductIndex, dropdata.ProductValue);
                if (uid != -1)
                    rewardlist.Add(new RewardData(uid, dropdata.ProductType, dropdata.ProductIndex, dropdata.ProductValue, changeGrade));
            }
            else if ((eREWARDTYPE)dropdata.ProductType == eREWARDTYPE.COSTUME)        //코스튬
            {
                uid = AddCostume(dropdata.ProductIndex);
                if (uid != -1)
                    rewardlist.Add(new RewardData(uid, dropdata.ProductType, dropdata.ProductIndex, dropdata.ProductValue, false));
            }
        }
    }

    private GameTable.Random.Param GetStoreDropRandom(ref List<GameTable.Random.Param> DropTableList)
    {
        int nTotal = 0;
        for (int i = 0; i < DropTableList.Count; i++)
        {
            nTotal += DropTableList[i].Prob;
        }

        int nRand = UnityEngine.Random.Range(0, nTotal + 1);

        int nMin = 0;
        int nMax = 0;
        int nResult = 0;

        for (int i = 0; i < DropTableList.Count; i++)
        {
            nMax = nMin + DropTableList[i].Prob;
            if (nRand <= nMax && nRand >= nMin)
            {
                nResult = i;
                break;
            }
            nMin += DropTableList[i].Prob;
        }

        if( nResult < 0 || nResult >= DropTableList.Count ) {
            return null;
		}

        return DropTableList[nResult];
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //
    //  게임결과
    //
    //------------------------------------------------------------------------------------------------------------------------------------
    //게임 시작
    public void Proc_ReqStageStart(int stageid, long charuid, OnReceiveCallBack ReceiveCallBack)
    {
        UserData _userdata = GameInfo.Instance.UserData;
        GameTable _gametable = GameInfo.Instance.GameTable;
        List<StageClearData> _stageclearlist = GameInfo.Instance.StageClearList;

        //클라용 저장
        GameInfo.Instance.SeleteCharUID = charuid;

        //캐릭터 보유 확인
        CharData chardata = GameInfo.Instance.GetCharData(charuid);
        if (chardata == null)
        {
            ReceiveCallBack(1, null);//인덱스 이상
            return;
        }

        //스테이지 테이블 확인
        GameTable.Stage.Param stagetabledata = _gametable.FindStage(x => x.ID == stageid);
        if (stagetabledata == null)
        {
            ReceiveCallBack(1, null);//인덱스 이상
            return;
        }

        //티켓 체크
        if (!_userdata.IsGoods(eGOODSTYPE.AP, stagetabledata.Ticket))
        {
            ReceiveCallBack(1, null);//인덱스 이상
            return;
        }
        else
        {
            //  티켓 차감
            _userdata.SubGoods(eGOODSTYPE.AP, stagetabledata.Ticket);
        }

        GameInfo.Instance.StageClearID = -1;
        GameInfo.Instance.bStageFailure = false;

        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    //게임 종료
    public void Proc_ReqStageEnd(int stageid, long charuid, int clearTime, int goldCnt, int box1, bool mission1, bool mission2, bool mission3, OnReceiveCallBack ReceiveCallBack)
    {
        GameInfo.Instance.StageClearID = stageid;
        GameInfo.Instance.bStageFailure = false;

        UserData _userdata = GameInfo.Instance.UserData;
        GameTable _gametable = GameInfo.Instance.GameTable;
        List<MonsterBookData> _monsterbooklist = GameInfo.Instance.MonsterBookList;
        List<StageClearData> _stageclearlist = GameInfo.Instance.StageClearList;
        List<TimeAttackClearData> _timeattackclearlist = GameInfo.Instance.TimeAttackClearList;

        //캐릭터 보유 확인
        CharData chardata = GameInfo.Instance.GetCharData(charuid);
        if (chardata == null)
        {
            ReceiveCallBack(1, null);//인덱스 이상
            return;
        }

        //스테이지 테이블 확인
        GameTable.Stage.Param stagetabledata = _gametable.FindStage(x => x.ID == stageid);
        if (stagetabledata == null)
        {
            ReceiveCallBack(1, null);//인덱스 이상
            return;
        }

        if (stagetabledata.StageType == (int)eSTAGETYPE.STAGE_MAIN_STORY)
        {
            //  메인 스토리 클리어시 1회
            SetClearMission(eMISSIONTYPE.WM_Con_MainStageClear);
        }
        else if (stagetabledata.StageType == (int)eSTAGETYPE.STAGE_DAILY)
        {
            //  요일 던전 클리어시 1회
            SetClearMission(eMISSIONTYPE.WM_Con_DailyStageClear);
        }
        else if (stagetabledata.StageType == (int)eSTAGETYPE.STAGE_SPECIAL)
        {
            GameInfo.Instance.UserData.LastPlaySpecialModeTime = GameSupport.GetCurrentServerTime().AddMinutes(GameInfo.Instance.GameConfig.SpecialModeRefreshTime);
        }

        List<CardData> cardlist = new List<CardData>();
        List<CardBookData> cardbooklist = new List<CardBookData>();

        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            cardlist.Add(GameInfo.Instance.GetCardData(chardata.EquipCard[i]));
            cardbooklist.Add(null);
        }


        //클라용 저장
        GameInfo.Instance.RewardList.Clear();

        GameInfo.Instance.GameResultData.Init();
        GameInfo.Instance.GameResultData.StageID = stageid;
        GameInfo.Instance.GameResultData.UserBeforeLevel = _userdata.Level;
        GameInfo.Instance.GameResultData.UserBeforeExp = _userdata.Exp;
        GameInfo.Instance.GameResultData.CharUID = charuid;
        GameInfo.Instance.GameResultData.CharBeforeGrade = chardata.Grade;
        GameInfo.Instance.GameResultData.CharBeforeLevel = chardata.Level;
        GameInfo.Instance.GameResultData.CharBeforeExp = chardata.Exp;

        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            if (cardlist[i] != null)
            {
                cardbooklist[i] = GameInfo.Instance.GetCardBookData(cardlist[i].TableID);
                if (cardbooklist[i] != null)
                {
                    GameInfo.Instance.GameResultData.CardUID[i] = cardlist[i].CardUID;
                    GameInfo.Instance.GameResultData.CardFavorBeforeLevel[i] = cardbooklist[i].FavorLevel;
                    GameInfo.Instance.GameResultData.CardFavorBeforeExp[i] = cardbooklist[i].FavorExp;
                }
            }
        }

        //골드 보상
        //if (stagetabledata.RewardGold != 0)
        //    GameInfo.Instance.GameResultData.Goods[(int)eGOODSTYPE.GOLD] += stagetabledata.RewardGold;
        if (goldCnt > 0)
        {
            GameInfo.Instance.GameResultData.Goods[(int)eGOODSTYPE.GOLD] += (long)(stagetabledata.RewardGold + (stagetabledata.RewardGold * Mathf.Min((float)(goldCnt * GameInfo.Instance.GameConfig.StageGoldDropAddRate), GameInfo.Instance.GameConfig.StageGoldDropAddMaxRate)));
        }

        GameInfo.Instance.GameResultData.Goods[(int)eGOODSTYPE.AP] -= stagetabledata.Ticket;
        GameInfo.Instance.GameResultData.Goods[(int)eGOODSTYPE.ROOMPOINT] += (long)(stagetabledata.Ticket * GameInfo.Instance.GameConfig.RPRatePerTicket);

        //지휘관 경험치 보상
        AddUserExp(stagetabledata.RewardEXP);

        //캐릭터 경험치 보상
        AddCharExp(ref chardata, stagetabledata.RewardCharEXP);

        //카드 경험치 보상
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            if (cardlist[i] != null && cardbooklist[i] != null)
                AddCardFavorExp(cardlist[i].TableID, stagetabledata.RewardCardFavor);
        }

        //캐릭터 스킬포인트 보상
        chardata.PassviePoint += stagetabledata.RewardSkillPoint;

        if (stagetabledata.StageType == (int)eSTAGETYPE.STAGE_TIMEATTACK)
        {
            var cleardata = _timeattackclearlist.Find(x => x.TableID == stageid);
            if (cleardata == null)
            {
                AddTimeAttackClear(stageid, charuid, clearTime);
            }
            else
            {
                if (cleardata.HighestScore > clearTime)
                {
                    cleardata.HighestScore = clearTime;
                    cleardata.CharCUID = charuid;
                    cleardata.HighestScoreRemainTime = GameSupport.GetCurrentServerTime().AddDays(15);
                }
            }
        }
        else
        {
            //스테이지 클리어 리스트에 클리어 미션등록
            var cleardata = _stageclearlist.Find(x => x.TableID == stageid);
            if (cleardata == null)
                _stageclearlist.Add(new StageClearData(stageid));

            cleardata = _stageclearlist.Find(x => x.TableID == stageid);
            if (cleardata != null)
            {
                if (cleardata.Mission[0] == (int)eCOUNT.NONE && mission1 && stagetabledata.Mission_00 != -1)
                {
                    cleardata.Mission[0] = stagetabledata.Mission_00;
                    GameInfo.Instance.GameResultData.ClearMissionIDList.Add(stagetabledata.Mission_00);
                }

                if (cleardata.Mission[1] == (int)eCOUNT.NONE && mission2 && stagetabledata.Mission_01 != -1)
                {
                    cleardata.Mission[1] = stagetabledata.Mission_01;
                    GameInfo.Instance.GameResultData.ClearMissionIDList.Add(stagetabledata.Mission_01);
                }

                if (cleardata.Mission[2] == (int)eCOUNT.NONE && mission3 && stagetabledata.Mission_02 != -1)
                {
                    cleardata.Mission[2] = stagetabledata.Mission_02;
                    GameInfo.Instance.GameResultData.ClearMissionIDList.Add(stagetabledata.Mission_02);
                }
            }

            //최초 클리어 미션 보상
            if (cleardata.IsClearAll())
            {
                if (GameInfo.Instance.GameResultData.ClearMissionIDList.Count != 0)
                    GameInfo.Instance.GameResultData.Goods[(int)eGOODSTYPE.CASH] += GameInfo.Instance.GameConfig.StageMissionClearCash;
            }
        }

        //아레나 프롤로그 스테이지 게이지 증가
        if (stagetabledata.StageType == (int)eSTAGETYPE.STAGE_PVP_PROLOGUE)
        {
            _userdata.ArenaPrologueValue += stagetabledata.TypeValue;
        }


        //일반 보상
        if (box1 > stagetabledata.N_DropMaxCnt)
            box1 = stagetabledata.N_DropMaxCnt;

        Purchase_Random(stagetabledata.N_DropID, box1, ref GameInfo.Instance.RewardList, false);
        for (int i = 0; i < box1; i++)
        {
            //GameInfo.Instance.GameResultData.BoxTypeList.Add((int)eCOUNT.STAGEREWARD_N);
            //  주간 미션 스테이지 당 획득한 아이템(갯수 당)
            SetClearMission(eMISSIONTYPE.WM_Con_StageGetItem);
        }

        //행운 보상
        if (cardlist[(int)eCARDSLOT.SLOT_MAIN] != null)
        {
            //77    행운도 확률 적용해야함
            Purchase_Random(stagetabledata.Luck_DropID, 1, ref GameInfo.Instance.RewardList, false);
            //for (int i = 0; i < 1; i++)
            //    GameInfo.Instance.GameResultData.BoxTypeList.Add((int)eCOUNT.STAGEREWARD_LUCK);
        }

        for (int i = 0; i < (int)eGOODSTYPE.COUNT; i++)
            _userdata.AddGoods((eGOODSTYPE)i, GameInfo.Instance.GameResultData.Goods[i]);

        List<int> monsterlist = new List<int>();
        monsterlist.Add(stagetabledata.Monster1);
        monsterlist.Add(stagetabledata.Monster2);
        monsterlist.Add(stagetabledata.Monster3);
        monsterlist.Add(stagetabledata.Monster4);
        monsterlist.Add(stagetabledata.Monster5);

        for (int i = 0; i < monsterlist.Count; i++)
        {
            if (monsterlist[i] != -1)
            {
                var bookdata = _monsterbooklist.Find(x => x.TableID == monsterlist[i]);
                if (bookdata == null)
                    _monsterbooklist.Add(new MonsterBookData(monsterlist[i], 0));
            }
        }

        GameInfo.Instance.GameResultData.UserAfterLevel = _userdata.Level;
        GameInfo.Instance.GameResultData.UserAfterExp = _userdata.Exp;
        GameInfo.Instance.GameResultData.CharAfterLevel = chardata.Level;
        GameInfo.Instance.GameResultData.CharAfterExp = chardata.Exp;

        //  랭크 업 만큼 메일 보상 추가
        int levelCha = GameInfo.Instance.GameResultData.UserAfterLevel - GameInfo.Instance.GameResultData.UserBeforeLevel;
        for (int i = GameInfo.Instance.GameResultData.UserBeforeLevel; i < GameInfo.Instance.GameResultData.UserBeforeLevel + levelCha; i++)
        {
            var rankUpReward = GameInfo.Instance.GameTable.FindRankUPReward(i + 1);
            var reward = GameInfo.Instance.GameTable.FindRandom(rankUpReward.RewardGroupID);

            AddMail((eMailType)1, (i + 1).ToString(), reward.ProductType, reward.ProductIndex, reward.ProductValue);
        }

        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            if (cardlist[i] != null && cardbooklist[i] != null)
            {
                GameInfo.Instance.GameResultData.CardFavorAfterLevel[i] = cardbooklist[i].FavorLevel;
                GameInfo.Instance.GameResultData.CardFavorAfterExp[i] = cardbooklist[i].FavorExp;
            }
        }



        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    public void Proc_ReqStageEndFail(int clearTime, OnReceiveCallBack ReceiveCallBack)
    {
        GameInfo.Instance.bStageFailure = true;

        SaveLocalData();
        //ReceiveCallBack(0, null);
    }

    //------------------------------------------------------------------------------------------------------------------------------------
    //
    //  우편함
    //
    //------------------------------------------------------------------------------------------------------------------------------------

    public void Proc_ReqMailTakeProductList(List<ulong> reciveMailUIDs, OnReceiveCallBack ReceiveCallBack)
    {
        if (reciveMailUIDs.Count == 0)
        {
            ReceiveCallBack(1, null);
            return;
        }

        //  삭제될 메일의 보상을 저장
        GameInfo.Instance.RewardList.Clear();
        for (int i = 0; i < reciveMailUIDs.Count; i++)
        {
            MailData mailData = GameInfo.Instance.GetMailData(reciveMailUIDs[i]);

            eREWARDTYPE productType = (eREWARDTYPE)mailData.ProductType;
            int productIndex = (int)mailData.ProductIndex;
            int productValue = (int)mailData.ProductValue;

            switch (productType)
            {
                case eREWARDTYPE.GOODS: //재화구매
                    {
                        GameInfo.Instance.UserData.AddGoods((eGOODSTYPE)productIndex, productValue);
                        if (mailData.ProductIndex == (int)eGOODSTYPE.CASH)
                            GameInfo.Instance.UserData.HardCash += productValue;

                        GameInfo.Instance.RewardList.Add(new RewardData((int)productType, productIndex, productValue));
                    }
                    break;
                case eREWARDTYPE.WEAPON:    //무기구매
                    {
                        AddWeapon((int)mailData.ProductIndex);
                        GameInfo.Instance.RewardList.Add(new RewardData(mailData.ProductType, (int)mailData.ProductIndex, (int)mailData.ProductValue));
                    }
                    break;
                case eREWARDTYPE.GEM:   //곡옥구매
                    {
                        AddGem((int)mailData.ProductIndex);
                        GameInfo.Instance.RewardList.Add(new RewardData(mailData.ProductType, (int)mailData.ProductIndex, (int)mailData.ProductValue));
                    }
                    break;
                case eREWARDTYPE.CARD:  //카드구매
                    {
                        AddCard((int)mailData.ProductIndex);
                        GameInfo.Instance.RewardList.Add(new RewardData(mailData.ProductType, (int)mailData.ProductIndex, (int)mailData.ProductValue));
                    }
                    break;
                case eREWARDTYPE.ITEM:     //아이템구매
                    {
                        AddItem((int)mailData.ProductIndex, (int)mailData.ProductValue);
                        GameInfo.Instance.RewardList.Add(new RewardData(mailData.ProductType, (int)mailData.ProductIndex, (int)mailData.ProductValue));
                    }
                    break;
                    //case eREWARDTYPE.CHAR:  //캐릭터구매
                    //    {
                    //        AddChar((int)mailData.ProductIndex);
                    //        GameInfo.Instance.RewardList.Add(new RewardData(mailData.ProductType, (int)mailData.ProductIndex, (int)mailData.ProductValue));
                    //    }
                    //    break;
                    //case eREWARDTYPE.COSTUME:     //코스튬구매
                    //    {
                    //        AddCostume((int)mailData.ProductIndex);
                    //        GameInfo.Instance.RewardList.Add(new RewardData(mailData.ProductType, (int)mailData.ProductIndex, (int)mailData.ProductValue));
                    //    }
                    //    break;
                    //case eREWARDTYPE.PACKAGE: //묶음상품
                    //    {
                    //        Purchase_Package(productIndex, ref GameInfo.Instance.RewardList);
                    //    }
                    //    break;
            }

            GameInfo.Instance.MailList.Remove(mailData);
            GameInfo.Instance.MailTotalCount--;
        }

        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    public void Proc_ReqMailList(ulong reciveStartIDX, uint viewCount, OnReceiveCallBack ReceiveCallBack)
    {
        if (GameInfo.Instance.MailTotalCount == 0)
        {
            ////  보상을 보여주기전 보상리스트를 초기화합니다.
            //GameInfo.Instance.MailList.Clear();

            //int randomCount = 20;
            //int[] randomProduct = { 100, 101, 200, 7002, 8013, 9001 };
            //for (int i = 0; i < randomCount; i++)
            //{
            //    int id = randomProduct[(int)UnityEngine.Random.Range(0, randomProduct.Length)];
            //    var reward = GameInfo.Instance.GameTable.FindRandom(a => a.GroupID == id);

            //    if(reward != null)
            //    {
            //        AddMail(1, 0, reward.ProductType, reward.ProductIndex, reward.ProductValue);
            //    }
            //}
        }
        else
        {
            //  해당 메일의 남은 시간이 -시간인 경우 메일 삭제
            GameInfo.Instance.MailList.RemoveAll(a => a.RemainTime.Subtract(GameSupport.GetCurrentServerTime()).Ticks < 0);
        }


        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    //------------------------------------------------------------------------------------------------------------------------------------
    //
    //  로그인 보너스
    //
    //------------------------------------------------------------------------------------------------------------------------------------

    public void Proc_ReqReflashLoginBonus(OnReceiveCallBack ReceiveCallBack)
    {
        //  로그인 보너스 그룹 ID 체크
        if (GameInfo.Instance.UserData.LoginBonusGroupID == 0)
        {
            ReceiveCallBack(1, null);
            return;
        }

        //  로그인 보너스는 일주일만 체크
        if (GameInfo.Instance.UserData.LoginBonusGroupCnt > 7)
        {
            ReceiveCallBack(1, null);
            return;
        }

        int LoginBonusGroupID = GameInfo.Instance.UserData.LoginBonusGroupID;
        int LoginBonusGroupCnt = GameInfo.Instance.UserData.LoginBonusGroupCnt;
        GameInfo.Instance.UserData.LoginBonusRecentDate = GameSupport.GetCurrentServerTime();

        //  갱신하지 않은 정보 확인
        var beforeParam = GameInfo.Instance.GameTable.FindLoginBonus(a => (a.LoginGroupID == LoginBonusGroupID && a.LoginCnt == LoginBonusGroupCnt));
        if (beforeParam != null && beforeParam.NextGroupID != 0)
        {
            GameInfo.Instance.UserData.LoginBonusGroupID = beforeParam.NextGroupID;
            GameInfo.Instance.UserData.LoginBonusGroupCnt = 1;
        }
        else
        {
            LoginBonusGroupCnt = GameInfo.Instance.UserData.LoginBonusGroupCnt += 1;
        }

        //  갱신된 정보 확인
        var currentParam = GameInfo.Instance.GameTable.FindLoginBonus(a => (a.LoginGroupID == LoginBonusGroupID && a.LoginCnt == LoginBonusGroupCnt));

        //  보상 메일로 전달
        var reward = GameInfo.Instance.GameTable.FindRandom(currentParam.RewardGroupID);
        AddMail((eMailType)currentParam.SendMailTypeID,
                currentParam.LoginCnt.ToString(),
                reward.ProductType,
                reward.ProductIndex,
                reward.ProductValue);

        //  주간 미션 로그인(일일 1회)
        SetClearMission(eMISSIONTYPE.WM_Con_DailyLogin);

        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    //------------------------------------------------------------------------------------------------------------------------------------
    //
    //  주간 미션
    //
    //------------------------------------------------------------------------------------------------------------------------------------

    public void Proc_ReqRewardWeekMission(List<int> indexlist, OnReceiveCallBack ReceiveCallBack)
    {
        //  보기용 보상리스트 추가
        GameInfo.Instance.RewardList.Clear();

        for (int x = 0; x < indexlist.Count; x++)
        {
            int index = indexlist[x];
            //  주간 미션 데이터가 없는 경우
            if (GameInfo.Instance.WeekMissionData == null)
            {
                ReceiveCallBack(1, null);
                return;
            }

            //  이미 수령한 경우
            if (GameSupport.IsComplateMissionRecive(GameInfo.Instance.WeekMissionData.fMissionRewardFlag, index) == true)
            {
                ReceiveCallBack(1, null);
                return;
            }

            if (index > 6)
            {
                uint reciveIndexFlag = (uint)(0x0000000001 << index);
                GameInfo.Instance.WeekMissionData.fMissionRewardFlag += reciveIndexFlag;
            }
            else
            {
                //  미션 카운트를 내리고 내린 값이 0인 경우 플래그 갱신
                uint remainCount = GameInfo.Instance.WeekMissionData.fMissionRemainCntSlot[index];
                if (remainCount == 0)
                {
                    uint reciveIndexFlag = (uint)(0x0000000001 << index);
                    GameInfo.Instance.WeekMissionData.fMissionRewardFlag += reciveIndexFlag;
                }
            }

            //  보상 데이터 확인
            var weeklyMission = GameInfo.Instance.GameTable.FindWeeklyMissionSet((int)GameInfo.Instance.WeekMissionData.fWeekMissionSetID);
            var rewardGroup = GameInfo.Instance.GameTable.FindAllRandom(a => a.GroupID == weeklyMission.RewardGroupID);
            RewardData rewardData = new RewardData(rewardGroup[index].ProductType,
                                                   rewardGroup[index].ProductIndex,
                                                   rewardGroup[index].ProductValue);

            //  보상을 메일로 전달
            AddMail((eMailType)GameInfo.Instance.GameConfig.WeeklyMailTypeID, "1", rewardData.Type, rewardData.Index, rewardData.Value);


            GameInfo.Instance.RewardList.Add(rewardData);
        }


        Debug.LogWarning(string.Format("###로그인###   로그인 그룹은 {0}", GameInfo.Instance.WeekMissionData.fWeekMissionSetID));

        SaveLocalData();
        ReceiveCallBack(0, null);
    }

    //------------------------------------------------------------------------------------------------------------------------------------
    //
    //  이벤트 모드
    //
    //------------------------------------------------------------------------------------------------------------------------------------
    public void Proc_ReqEventRewardReset(int eventID, OnReceiveCallBack ReceiveCallBack)
    {
        EventSetData eventdata = GameInfo.Instance.GetEventSetData(eventID);
        if (eventdata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }

        int curStep = eventdata.RewardStep + 1;
        int tableID = eventdata.TableID;
        eventdata.RewardStep = curStep;
        int eventStep = 0;

        foreach (GameTable.EventResetReward.Param er in GameInfo.Instance.GameTable.EventResetRewards)
        {
            if (eventStep < er.RewardStep)
                eventStep = er.RewardStep;
        }

        if (eventdata.RewardStep >= eventStep)
            eventdata.RewardStep = eventStep;

        eventdata.Count += 1;

        List<GameTable.EventResetReward.Param> tempRewardList = GameInfo.Instance.GameTable.EventResetRewards.FindAll(x => x.EventID == eventdata.TableID && x.RewardStep == eventdata.RewardStep);
        Log.Show(eventdata.RewardStep);
        eventdata.RewardItemCount.Clear();

        for (int i = 0; i < tempRewardList.Count; i++)
        {
            eventdata.RewardItemCount.Add(tempRewardList[i].RewardCnt);
        }

        for (int i = 0; i < GameInfo.Instance.EventSetDataList.Count; i++)
        {
            if (GameInfo.Instance.EventSetDataList[i].TableID.Equals(tableID))
            {
                GameInfo.Instance.EventSetDataList[i] = eventdata;
                break;
            }
        }

        //GameInfo.Instance.EventSetDataList.Clear();
        //GameInfo.Instance.EventSetDataList.Add(eventdata);

        SaveLocalData();
        LoadLocalData();
        ReceiveCallBack(0, null);
    }

    public void Proc_ReqEventRewardTake(int eventID, int cnt, int eventresetstep, int idx, OnReceiveCallBack ReceiveCallBack)
    {
        UserData _userdata = GameInfo.Instance.UserData;
        List<ItemData> _itemlist = GameInfo.Instance.ItemList;

        EventSetData eventsetdata = GameInfo.Instance.GetEventSetData(eventID);
        if (eventsetdata == null)
        {
            ReceiveCallBack(1, null);
            return;
        }



        GameTable.EventSet.Param tableData = GameInfo.Instance.GameTable.FindEventSet(x => x.EventID == eventID);
        if (tableData == null)
        {
            ReceiveCallBack(1, null);
            return;
        }

        int eventStep = 0;

        //보상 목록 작성
        List<RewardData> resultData = new List<RewardData>();

        if (tableData.EventType == (int)eEventRewardKind.RESET_LOTTERY)
        {
            foreach (GameTable.EventResetReward.Param er in GameInfo.Instance.GameTable.EventResetRewards)
            {
                if (eventStep < er.RewardStep)      //테이블에 있는 RewardStep보다 크면 마지막 RewardStep 반복
                    eventStep = er.RewardStep;
            }

            if (eventsetdata.RewardStep < eventStep)
                eventStep = eventsetdata.RewardStep;


            List<GameTable.EventResetReward.Param> rewardData = GameInfo.Instance.GameTable.EventResetRewards.FindAll(x => x.EventID == eventsetdata.TableID && x.RewardStep == eventsetdata.RewardStep);
            int index = cnt;
            while (index > 0)
            {
                int rnd = UnityEngine.Random.Range(0, eventsetdata.RewardItemCount.Count);

                if (eventsetdata.RewardItemCount[rnd] <= 0)
                    continue;

                eventsetdata.RewardItemCount[rnd] -= 1;
                index--;
                resultData.Add(new RewardData(rewardData[rnd].ProductType,
                    rewardData[rnd].ProductIndex, rewardData[rnd].ProductValue));
            }

            //소모한 이벤트 티켓 감소
            ItemData it = _itemlist.Find(x => x.TableID == eventsetdata.TableData.EventItemID1);
            if (it == null)
            {
                ReceiveCallBack(1, null);
                return;
            }
            it.Count -= GameInfo.Instance.GameConfig.EvtResetGachaReqCnt * cnt;
            if (it.Count <= 0)
                _itemlist.Remove(it);
        }
        else if (tableData.EventType == (int)eEventRewardKind.EXCHANGE)
        {
            eventStep = eventresetstep;

            List<GameTable.EventExchangeReward.Param> exchangeList = GameInfo.Instance.GameTable.EventExchangeRewards.FindAll(x => x.EventID == eventID && x.RewardStep == eventresetstep);
            for (int i = 0; i < exchangeList.Count; i++)
            {
                if (i == idx)
                {
                    resultData.Add(new RewardData(exchangeList[i].ProductType, exchangeList[i].ProductIndex, exchangeList[i].ProductValue));
                    break;
                }
            }
        }
        else if (tableData.EventType == (int)eEventRewardKind.MISSION)
        {

        }






        //보상 증가(현재 재화만 가능 / 190708 - 나머지는 추후에 작업 하기로함)
        //for (int i = 0; i < resultData.Count; i++)
        //{
        //    eREWARDTYPE rt = (eREWARDTYPE)resultData[i].Type;

        //    switch (rt)
        //    {
        //        case eREWARDTYPE.GACHA:
        //            break;
        //        case eREWARDTYPE.GOODS:
        //            {
        //                eGOODSTYPE et = (eGOODSTYPE)resultData[i].Index;
        //                switch (et)
        //                {
        //                    case eGOODSTYPE.CASH:
        //                        _userdata.AddGoods(eGOODSTYPE.CASH, (long)resultData[i].Value);
        //                        break;
        //                    case eGOODSTYPE.GOLD:
        //                        _userdata.AddGoods(eGOODSTYPE.GOLD, (long)resultData[i].Value);
        //                        break;
        //                    default:
        //                        AddItem((int)resultData[i].UID, resultData[i].Value);
        //                        break;
        //                }
        //            }
        //            break;
        //        case eREWARDTYPE.CHAR:
        //            break;
        //        case eREWARDTYPE.WEAPON:
        //            break;
        //        case eREWARDTYPE.GEM:
        //            break;
        //        case eREWARDTYPE.CARD:
        //            break;
        //        case eREWARDTYPE.COSTUME:
        //            break;
        //        case eREWARDTYPE.ITEM:
        //            break;
        //        case eREWARDTYPE.ROOMTHEME:
        //            break;
        //        case eREWARDTYPE.ROOMFUNC:
        //            break;
        //        case eREWARDTYPE.ROOMACTION:
        //            break;
        //        case eREWARDTYPE.ROOMFIGURE:
        //            break;
        //        case eREWARDTYPE.PACKAGE:
        //            break;
        //        default:
        //            break;
        //    }

        //}


        GameInfo.Instance.RewardList = resultData;


        SaveLocalData();
        LoadLocalData();
        ReceiveCallBack(0, null);
    }

    public void Proc_ReqSetTutorialVal(int state, int step, OnReceiveCallBack ReceiveCallBack)
    {
        GameInfo.Instance.UserData.SetTutorial(state, step);
        SaveLocalData();
        LoadLocalData();
        ReceiveCallBack(0, null);
    }

    public void Proc_ReqSetTutorialFlag(long flag, OnReceiveCallBack ReceiveCallBack)
    {
        GameInfo.Instance.UserData.TutorialFlag = flag;
        SaveLocalData();
        LoadLocalData();
        ReceiveCallBack(0, null);
    }

    public void Proc_ReqTimeAtkRankingList(OnReceiveCallBack ReceiveCallBack)
    {
        SaveLocalData();
        LoadLocalData();
        ReceiveCallBack(0, null);
    }

    public void Proc_ReqTimeAtkRankerDetail(OnReceiveCallBack ReceiveCallBack)
    {
        SaveLocalData();
        LoadLocalData();
        ReceiveCallBack(0, null);
    }

    //=============================================================================================
    // 아레나
    //=============================================================================================
    public void Proc_ReqArenaSeasonPlay(OnReceiveCallBack ReceiveCallBack)
    {
        ReceiveCallBack(0, null);
    }
    public void Proc_ReqArenaRankingList(OnReceiveCallBack ReceiveCallBack)
    {
        ReceiveCallBack(0, null);
    }
    public void Proc_ReqSetArenaTeam(List<long> teamCharList, OnReceiveCallBack ReceiveCallBack)
    {
        ReceiveCallBack(0, null);
    }
    //=============================================================================================
    // 문양
    //=============================================================================================

    public void Proc_ReqApplyPosBadge(long badgeUID, int slotNum, OnReceiveCallBack ReceiveCallBack)
    {
        ReceiveCallBack(0, null);
    }

    public void Proc_ReqApplyOutPosBadge(long badgeUID, OnReceiveCallBack ReceiveCallBack)
    {
        ReceiveCallBack(0, null);
    }

    public void Proc_ReqUpgradeBadge(long badgeUID, OnReceiveCallBack ReceiveCallBack)
    {
        ReceiveCallBack(0, null);
    }

    public void Proc_ReqResetUpgradeBadge(long badgeUID, OnReceiveCallBack ReceiveCallBack)
    {
        ReceiveCallBack(0, null);
    }

    public void Proc_ReqSetLockBadge(List<long> badgeuidlist, List<bool> locklist, OnReceiveCallBack ReceiveCallBack)
    {
        ReceiveCallBack(0, null);
    }

    public void Proc_ReqSellBadge(List<long> uidlist, OnReceiveCallBack ReceiveCallBack)
    {
        ReceiveCallBack(0, null);
    }

    //public void Send_ReqAddBadgeSlot(OnReceiveCallBack ReceiveCallBack)
    //{
    //    UserData _userdata = GameInfo.Instance.UserData;
    //    GameConfig _gameconfig = GameInfo.Instance.GameConfig;
    //    //레벨 제한 확인
    //    if (_userdata.BadgeSlotCnt >= _gameconfig.BadgeMaxSlotCount)
    //    {
    //        ReceiveCallBack(1, null);
    //        return;
    //    }
    //    if (!_userdata.IsGoods(eGOODSTYPE.GOLD, _gameconfig.BadgeAddSlotGold))
    //    {
    //        ReceiveCallBack(1, null);
    //        return;
    //    }
    //    _userdata.ItemSlotCnt += _gameconfig.BadgeAddSlotCount;
    //    _userdata.SubGoods(eGOODSTYPE.GOLD, _gameconfig.BadgeAddSlotGold);
    //    SaveLocalData();
    //    ReceiveCallBack(0, null);
    //}
    //------------------------------------------------------------------------------------------------------------------------------------
    //
    //  기본 추가 함수
    //
    //------------------------------------------------------------------------------------------------------------------------------------
    public void AddCharExp(ref CharData chardata, int Exp)
    {
        int oldlev = chardata.Level;

        int nRemainExp = GameSupport.GetRemainCharExpToMaxLevel(chardata.Exp, chardata.Grade);
        if (Exp >= nRemainExp)
            Exp = nRemainExp;

        chardata.Exp += Exp;
        chardata.Level = GameSupport.GetCharacterExpLevel(chardata.Exp, chardata.Grade);
        if (oldlev != chardata.Level)
        {

        }
    }

    public void AddWeaponExp(ref WeaponData weapondata, int Exp)
    {
        int oldlev = weapondata.Level;

        int nRemainExp = GameSupport.GetRemainWeaponExpToMaxLevel(weapondata);
        if (Exp >= nRemainExp)
            Exp = nRemainExp;

        weapondata.Exp += Exp;
        weapondata.Level = GameSupport.GetWeaponExpLevel(weapondata, weapondata.Exp);
        if (oldlev != weapondata.Level)
        {

        }
    }

    public void AddUserExp(int Exp)
    {

        GameTable _gametable = GameInfo.Instance.GameTable;
        UserData _userdata = GameInfo.Instance.UserData;

        int oldlev = _userdata.Level;

        int nRemainExp = GameSupport.GetRemainAccountExpToMaxLevel(_userdata.Exp);
        if (Exp >= nRemainExp)
            Exp = nRemainExp;

        _userdata.Exp += Exp;
        _userdata.Level = GameSupport.GetAccountExpLevel(_userdata.Exp);

        if (oldlev < _userdata.Level)
        {
            for (int uplv = oldlev + 1; uplv <= _userdata.Level; uplv++)
            {
                _userdata.Goods[(int)eGOODSTYPE.AP] += GameSupport.GetMaxAP();
            }
        }
    }

    public void AddCardExp(CardData carddata, int Exp)
    {
        int oldlev = carddata.Level;
        int nRemainExp = GameSupport.GetRemainCardExpToMaxLevel(carddata);
        if (Exp >= nRemainExp)
            Exp = nRemainExp;

        carddata.Exp += Exp;
        carddata.Level = GameSupport.GetCardExpLevel(carddata, carddata.Exp);
        if (oldlev != carddata.Level)
        {

        }
    }

    public void AddCardFavorExp(int tableid, int Exp)
    {
        CardBookData cardbookdata = GameInfo.Instance.GetCardBookData(tableid);
        if (cardbookdata == null)
            return;
        int oldlev = cardbookdata.FavorLevel;
        int nRemainExp = GameSupport.GetRemainCardFavorToMaxLevel(cardbookdata);
        if (Exp >= nRemainExp)
            Exp = nRemainExp;

        cardbookdata.FavorExp += Exp;
        cardbookdata.FavorLevel = GameSupport.GetCardFavorLevel(cardbookdata.TableID, cardbookdata.FavorExp);
        if (oldlev != cardbookdata.FavorLevel)
        {
            if (cardbookdata.FavorLevel == GameInfo.Instance.GameConfig.CardFavorMaxLevel)
            {
                cardbookdata.DoOnFlag(eBookStateFlag.MAX_FAVOR_LV);
            }
        }
    }



    public long AddChar(int tableid, long uid = -1)
    {
        GameConfig _gameconfig = GameInfo.Instance.GameConfig;
        GameTable _gametable = GameInfo.Instance.GameTable;
        List<CharData> _charlist = GameInfo.Instance.CharList;
        List<StageClearData> _stageclearlist = GameInfo.Instance.StageClearList;

        var chartabledata = _gametable.FindCharacter(tableid);
        if (chartabledata == null)
            return -1; //없는 데이타

        var checkdata = _charlist.Find(x => x.TableID == tableid);
        if (checkdata != null)
            return -1; //이미 보유한 캐릭터

        long charUid = uid == -1 ? _testcharuid : uid;

        CharData chardata = new CharData();
        chardata.Init(charUid, tableid);
        chardata.PassviePoint = _gameconfig.InitCharPassviePoint;

        _charlist.Add(chardata);

        if (uid == -1)
        {
            _testcharuid += 1;
        }

        //기본 무기 지급
        long weaponuid = AddWeapon(chartabledata.InitWeapon);
        chardata.EquipWeaponUID = weaponuid;
        //기본 코스튬 지급
        int costumetableid = AddCostume(chartabledata.InitCostume);
        chardata.EquipCostumeID = costumetableid;

        //캐릭터 스킬 슬롯 
        if (chartabledata.InitSkillSlot1 != -1)
            AddSkill(chardata, chartabledata.InitSkillSlot1, 1);
        if (chartabledata.InitSkillSlot2 != -1)
            AddSkill(chardata, chartabledata.InitSkillSlot2, 1);
        if (chartabledata.InitSkillSlot3 != -1)
            AddSkill(chardata, chartabledata.InitSkillSlot3, 1);
        if (chartabledata.InitSkillSlot4 != -1)
            AddSkill(chardata, chartabledata.InitSkillSlot4, 1);

        chardata.EquipSkill[0] = chartabledata.InitSkillSlot1;
        chardata.EquipSkill[1] = chartabledata.InitSkillSlot2;
        chardata.EquipSkill[2] = chartabledata.InitSkillSlot3;
        chardata.EquipSkill[3] = chartabledata.InitSkillSlot4;

        //캐릭터 스토리 모드 지급
        var stagedata = GameInfo.Instance.GameTable.FindStage(chartabledata.InitAddStageID);
        if (stagedata != null)
        {
            var data = _stageclearlist.Find(x => x.TableID == chartabledata.InitAddStageID);
            if (data == null)
                _stageclearlist.Add(new StageClearData(chartabledata.InitAddStageID));
        }
        return chardata.CUID;
    }

    public void RemoveChar(long uid)
    {
        object weaponBookTID = UIValue.Instance.GetValue(UIValue.EParamType.TemporaryWeaponBookTraining, true);
        if (weaponBookTID != null)
        {
            WeaponBookData findWpnBook = GameInfo.Instance.WeaponBookList.Find(x => x.TableID == (int)weaponBookTID);
            if (findWpnBook != null)
            {
                GameInfo.Instance.WeaponBookList.Remove(findWpnBook);
            }
        }

        object weaponUID = UIValue.Instance.GetValue(UIValue.EParamType.TemporaryWeaponTraining, true);
        if (weaponUID != null)
        {
            WeaponData findWpn = GameInfo.Instance.WeaponList.Find(x => x.WeaponUID == (long)weaponUID);
            if (findWpn != null)
            {
                GameInfo.Instance.WeaponList.Remove(findWpn);
            }
        }

        CharData find = GameInfo.Instance.CharList.Find(x => x.CUID == uid);
        if(find == null)
        {
            return;
        }

        GameInfo.Instance.CostumeList.Remove(find.EquipCostumeID);

        GameInfo.Instance.CharList.Remove(find);
    }

    public void AddSkill(CharData chardata, int skillid, int level)
    {
        GameTable _gametable = GameInfo.Instance.GameTable;
        var data = _gametable.FindCharacterSkillPassive(skillid);
        if (data != null)
            chardata.PassvieList.Add(new PassiveData(skillid, level));
    }

    public long AddWeapon(int tableid)
    {
        GameTable _gametable = GameInfo.Instance.GameTable;
        List<WeaponData> _weaponlist = GameInfo.Instance.WeaponList;
        List<WeaponBookData> _weaponbooklist = GameInfo.Instance.WeaponBookList;

        var weapontabledata = _gametable.FindWeapon(tableid);
        if (weapontabledata == null)
            return -1; //없는 데이타

        WeaponData weapondata = null;
        weapondata = new WeaponData(_testitemuid, tableid);
        _weaponlist.Add(weapondata);
        _testitemuid += 1;

        GameSupport.GetWeaponGradeSlotCount(weapondata.TableData.Grade, weapondata.Wake);

        //도감
        var bookdata = _weaponbooklist.Find(x => x.TableID == tableid);
        if (bookdata == null)
            _weaponbooklist.Add(new WeaponBookData(tableid, 0));
        return weapondata.WeaponUID;
    }

    public long AddGem(int tableid)
    {
        GameConfig _gameconfig = GameInfo.Instance.GameConfig;
        GameTable _gametable = GameInfo.Instance.GameTable;
        List<GemData> _gemlist = GameInfo.Instance.GemList;

        var gemtabledata = _gametable.FindGem(tableid);
        if (gemtabledata == null)
            return -1; //없는 데이타
        var list = _gametable.FindAllGemRandOpt(x => x.GroupID == gemtabledata.RandOptGroup);
        if (list == null)
            return -1; //없는 데이타
        if (list.Count == 0)
            return -1;

        GemData gemdata = null;
        gemdata = new GemData(_testitemuid, tableid);
        _gemlist.Add(gemdata);

        _testitemuid += 1;

        return gemdata.GemUID;
    }

    public bool SetGemOpt(long gemuid, int slot)
    {
        GameConfig _gameconfig = GameInfo.Instance.GameConfig;
        GameTable _gametable = GameInfo.Instance.GameTable;
        GemData target = GameInfo.Instance.GetGemData(gemuid);
        if (target == null)
            return false;
        var list = _gametable.FindAllGemRandOpt(x => x.GroupID == target.TableData.RandOptGroup);
        if (list == null)
            return false; //없는 데이타
        if (list.Count == 0)
            return false;

        int rand = UnityEngine.Random.Range(0, list.Count);
        var seldata = list[rand];
        int value = UnityEngine.Random.Range(0, seldata.RndStep + 1);
        //패시브 아이디, 값 등록
        if (slot == -1)
        {
            target.TempOptID = seldata.ID;
            target.TempOptValue = value;
        }
        else
        {
            target.RandOptID[slot] = seldata.ID;
            target.RandOptValue[slot] = value;
        }

        return true;
    }

    public long AddCard(int tableid)
    {
        GameTable _gametable = GameInfo.Instance.GameTable;
        List<CardData> _cardlist = GameInfo.Instance.CardList;
        List<CardBookData> _cardbooklist = GameInfo.Instance.CardBookList;

        var cardtabledata = _gametable.FindCard(tableid);
        if (cardtabledata == null)
            return -1; //없는 데이타
        CardData carddata = null;

        carddata = new CardData(_testitemuid, tableid);
        _cardlist.Add(carddata);
        _testitemuid += 1;

        //도감
        var bookdata = _cardbooklist.Find(x => x.TableID == tableid);
        if (bookdata == null)
            _cardbooklist.Add(new CardBookData(tableid, 0));

        //업적처리
        return carddata.CardUID;
    }


    public long AddItem(int tableid, int count)
    {
        GameTable _gametable = GameInfo.Instance.GameTable;
        List<ItemData> _itemlist = GameInfo.Instance.ItemList;

        var itemtabledata = _gametable.FindItem(tableid);
        if (itemtabledata == null)
            return -1; //없는 데이타
        ItemData itemdata = null;

        itemdata = _itemlist.Find(x => x.TableID == tableid);
        if (itemdata == null)
        {
            itemdata = new ItemData(_testitemuid, tableid);
            itemdata.Count = count;
            _itemlist.Add(itemdata);
            _testitemuid += 1;
        }
        else
        {
            itemdata.Count += count;
        }

        return itemdata.ItemUID;
    }

    public int AddCostume(int tableid)
    {
        if (GameInfo.Instance.HasCostume(tableid))
        {
            return tableid;
        }

        var costumetabledata = GameInfo.Instance.GameTable.FindCostume(tableid);
        if (costumetabledata == null)
            return -1;

        GameInfo.Instance.CostumeList.Add(tableid);

        return tableid;
    }

    public int AddRoomTheme(int tableid)
    {
        if (GameInfo.Instance.IsRoomThema(tableid))
            return -1;

        var paramRoomTheme = GameInfo.Instance.GameTable.FindRoomTheme(tableid);
        if (paramRoomTheme == null)
            return -1;

        GameInfo.Instance.RoomThemaList.Add(tableid);

        AddRoomFunc(paramRoomTheme.InitFunc);

        return 1;
    }

    public int AddRoomFunc(int tableid)
    {
        if (GameInfo.Instance.IsRoomFunc(tableid))
            return -1;

        var param = GameInfo.Instance.GameTable.FindRoomFunc(tableid);
        if (param == null)
            return -1;

        GameInfo.Instance.RoomFuncList.Add(tableid);

        return 1;
    }
    public int AddRoomAction(int tableid)
    {
        if (GameInfo.Instance.IsRoomAction(tableid))
            return -1;
        var param = GameInfo.Instance.GameTable.FindRoomAction(tableid);
        if (param == null)
            return -1;
        GameInfo.Instance.RoomActionList.Add(tableid);
        return 1;
    }

    public int AddRoomFigure(int tableid)
    {
        if (GameInfo.Instance.HasFigure(tableid))
            return -1;
        var param = GameInfo.Instance.GameTable.FindRoomFigure(tableid);
        if (param == null)
            return -1;
        GameInfo.Instance.RoomFigureList.Add(tableid);
        return 1;
    }

    public int AddUserMark(int tableid)
    {
        if (GameInfo.Instance.IsUserMark(tableid))
            return -1;

        var param = GameInfo.Instance.GameTable.FindUserMark(tableid);
        if (param == null)
            return -1;

        GameInfo.Instance.UserMarkList.Add(tableid);
        return 1;
    }

    public int AddTimeAttackClear(int tableid, long cuid, int score)
    {
        List<TimeAttackClearData> _timeattackclearlist = GameInfo.Instance.TimeAttackClearList;


        TimeAttackClearData data = new TimeAttackClearData(tableid);

        var chardata = GameInfo.Instance.CharList[0];


        data.HighestScore = score;
        data.CharCUID = cuid;
        data.HighestScoreRemainTime = GameSupport.GetCurrentServerTime().AddDays(15);

        _timeattackclearlist.Add(data);

        return 1;
    }

    public int AddTimeAttackRankUser(int tableid, int score, long charuid)
    {
        List<TimeAttackRankData> _timeattackranklist = GameInfo.Instance.TimeAttackRankList;
        var ranklist = _timeattackranklist.Find(x => x.TableID == tableid);
        if (ranklist == null)
            return -1;
        var chardata = GameInfo.Instance.GetCharData(charuid);
        if (chardata == null)
            return -1;

        TimeAttackRankUserData rankuserdata = new TimeAttackRankUserData();

        rankuserdata.Rank = ranklist.RankUserList.Count + 1;
        rankuserdata.HighestScore = score;
        rankuserdata.SetUserNickName( GameInfo.Instance.UserData.GetNickName() );
        rankuserdata.UserMark = GameInfo.Instance.UserData.UserMarkID;
        rankuserdata.UserRank = GameInfo.Instance.UserData.Level;
        rankuserdata.CharData = chardata;
        rankuserdata.WeaponData = GameInfo.Instance.GetWeaponData(chardata.EquipWeaponUID);
        rankuserdata.CardList.Clear();
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
            rankuserdata.CardList.Add(GameInfo.Instance.GetCardData(chardata.EquipCard[i]));

        ranklist.RankUserList.Add(rankuserdata);

        return 1;
    }

    public ulong AddMail(eMailType MailType, string MailValue, int ProductType, int ProductIndex, int ProductValue)
    {
        MailData mailData = new MailData();
        mailData.MailUID = (ulong)_testitemuid;
        mailData.MailType = MailType;
        mailData.MailTypeValue = MailValue;

        mailData.ProductType = ProductType;
        mailData.ProductIndex = (uint)ProductIndex;
        mailData.ProductValue = (uint)ProductValue;

        mailData.RemainTime = GameSupport.GetCurrentServerTime().AddDays(UnityEngine.Random.Range(1, 10));
        GameInfo.Instance.MailList.Add(mailData);

        _testitemuid += 1;
        GameInfo.Instance.MailTotalCount++;

        /*
        if (GameInfo.Instance.MailList.Count > 0)
            return -1;

        for (int i = 0; i < 20; i++)
        {
            //  메일 관련 변수
            MailData mailData = new MailData();
            mailData.MailUID = i;
            mailData.MailType = UnityEngine.Random.Range(1, 5);
            mailData.MailTypeValue = i;

            //  메일의 상품 random그룹ID들
            int[] arr = { 100, 101, 200, 7001, 7002, 8001, 8002, 9001, 9002 };
            GameTable.Random.Param param = GameInfo.Instance.GameTable.FindRandom(arr[UnityEngine.Random.Range(0, arr.Length)]);
            mailData.ProductType = param.ProductType;
            mailData.ProductIndex = param.ProductIndex;
            mailData.ProductValue = param.ProductValue;

            //  메일의 삭제예정 시간
            mailData.RemainTime = DateTime.Now.AddDays(UnityEngine.Random.Range(0, 10));

            GameInfo.Instance.MailList.Add(mailData);
        }
        */
        return mailData.MailUID;
    }

    public bool DelWeapon(long uid)
    {
        WeaponData target = GameInfo.Instance.GetWeaponData(uid);
        if (target == null)
            return false;

        GameInfo.Instance.WeaponList.Remove(target);
        return true;
    }

    public bool DelGem(long uid)
    {
        GemData target = GameInfo.Instance.GetGemData(uid);
        if (target == null)
            return false;

        GameInfo.Instance.GemList.Remove(target);
        return true;
    }

    public bool DelCard(long uid)
    {
        CardData target = GameInfo.Instance.GetCardData(uid);
        if (target == null)
            return false;

        GameInfo.Instance.CardList.Remove(target);
        return true;
    }

    public bool DelItem(long uid, int count)
    {
        ItemData target = GameInfo.Instance.GetItemData(uid);
        if (target == null)
            return false;

        target.Count -= count;
        if (target.Count <= 0)
        {
            GameInfo.Instance.ItemList.Remove(target);
        }

        return true;
    }

    /// <summary>
    ///  주간 미션 로컬 갱신
    /// </summary>
    public void SetClearMission(eMISSIONTYPE missionType)
    {
        if (GameInfo.Instance.WeekMissionData == null)
            return;

        uint weeklyMissionID = GameInfo.Instance.WeekMissionData.fWeekMissionSetID;
        if (weeklyMissionID == 0)
            return;

        GameTable.WeeklyMissionSet.Param param = GameInfo.Instance.GameTable.FindWeeklyMissionSet((int)GameInfo.Instance.WeekMissionData.fWeekMissionSetID);
        int[] missionList = new int[7];
        missionList[0] = GameSupport.CompareMissionString(param.WMCon0);
        missionList[1] = GameSupport.CompareMissionString(param.WMCon1);
        missionList[2] = GameSupport.CompareMissionString(param.WMCon2);
        missionList[3] = GameSupport.CompareMissionString(param.WMCon3);
        missionList[4] = GameSupport.CompareMissionString(param.WMCon4);
        missionList[5] = GameSupport.CompareMissionString(param.WMCon5);
        missionList[6] = GameSupport.CompareMissionString(param.WMCon6);

        //  미션 타입 확인
        int missionNum = 0;
        for (int i = 0; i < missionList.Length; i++)
        {
            if (missionList[i] == (int)missionType)
            {
                missionNum = i;
                break;
            }
        }

        //  해당 미션 카운트 차감
        uint remainClearCount = GameInfo.Instance.WeekMissionData.fMissionRemainCntSlot[missionNum];
        if (remainClearCount > 0)
        {
            GameInfo.Instance.WeekMissionData.fMissionRemainCntSlot[missionNum]--;
        }
    }
}

    
