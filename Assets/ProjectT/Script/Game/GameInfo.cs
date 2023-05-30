
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Nettention.Proud;
using System.Net;
using System.Net.Sockets;


public delegate void OnReceiveCallBack(int result, PktMsgType pktmsg);
public delegate void OnTimeOutCallBack();

public partial class GameInfo : FMonoSingleton<GameInfo>
{
    private GameConfig _gameconfig = null;
    private BattleConfig _battleconfig = null;
    private GameTable _gametable = null;
    private GameClientTable _gameclienttable = null;
    private ServerData _serverdata = new ServerData();
    private UserData _userdata = new UserData();
    private List<StoreData> _storelist = new List<StoreData>();
    private List<AchieveData> _achievelist = new List<AchieveData>();
    private List<CharData> _charlist = new List<CharData>();
    private List<WeaponData> _weaponlist = new List<WeaponData>();
    private List<GemData> _gemlist = new List<GemData>();
    private List<CardData> _cardlist = new List<CardData>();
    private List<ItemData> _itemlist = new List<ItemData>();
    private List<FacilityData> _facilitylist = new List<FacilityData>();
    private List<StageClearData> _stageclearlist = new List<StageClearData>();
    private List<WeaponBookData> _weaponbooklist = new List<WeaponBookData>();
    private List<CardBookData> _cardbooklist = new List<CardBookData>();
    private List<MonsterBookData> _monsterbooklist = new List<MonsterBookData>();
    private List<TimeAttackClearData> _timeattackclearlist = new List<TimeAttackClearData>();
    private List<TimeAttackRankData> _timeattackranklist = new List<TimeAttackRankData>();
    private List<GllaMissionData> _gllamissionlist = new List<GllaMissionData>();   //9998

    private List<int> _costumelist = new List<int>();
    private List<int> _roomactionlist = new List<int>();    //구매된 룸 액션 테이블의 아이디 리스트 ->RoomAction
    private List<int> _roomthemalist = new List<int>();     //구매된 룸테마의 테이블 아이디 리스트 -> RoomTheme
    private List<int> _roomfigurelist = new List<int>();    //구매된 룸 피규어의 테이블 아이디 리스트 - >RoomFigure
    private List<int> _roomfunclist = new List<int>();      //구매된 룸 기능의 테이블 아이디 리스트 ->RoomFunc
    private List<int> _usermarklist = new List<int>();      //구매된 룸 기능의 테이블 아이디 리스트 ->RoomFunc
    private List<RoomThemeSlotData> _roomthemeslotlist = new List<RoomThemeSlotData>();
    private List<RoomThemeFigureSlotData> _roomthemefigureslotlist = new List<RoomThemeFigureSlotData>();
    private List<MailData> _mailList = new List<MailData>();
    private List<EventSetData> _eventSetDataList = new List<EventSetData>();
    private bool _roomthemefiguredetailinfo = false;
    //private StageInfo m_stageInfo = new StageInfo();
    private List<long> _newcardlist = new List<long>();
    private List<long> _newweaponlist = new List<long>();
    private List<long> _newgemlist = new List<long>();
    private List<long> _newitemlist = new List<long>();
    private List<int> _newiconlist = new List<int>();
    private List<long> _newbadgelist = new List<long>();

    private UserBattleData _userbattledata = new UserBattleData();
    private List<BadgeData> _badgelist = new List<BadgeData>();
    private List<long> _teamcharlist = new List<long>();
    private TeamData _matchteam = new TeamData();
    private ArenaRankingListData _arenaRankingList = new ArenaRankingListData();
    private TeamData _arenaRankerDetialData = new TeamData();

    private List<PassMissionData> _passMissionData = new List<PassMissionData>();
    private CommunityData _communityData = new CommunityData();

    private RoomThemeSlotData _friendRoomSlotData = new RoomThemeSlotData();
    private List<RoomThemeFigureSlotData> _friendRoomFigureSlotList = new List<RoomThemeFigureSlotData>();

    private UserMonthlyData _userMonthlyData = new UserMonthlyData();
    private UserBuffEffectData _userBuffEffectData = new UserBuffEffectData();

    private WeaponArmoryData _weaponArmoryData = new WeaponArmoryData();
    private List<MyDyeingData> _dyeingDataList = new List<MyDyeingData>();
    private Dictionary<int, List<UnexpectedPackageData>> _unexpectedPackageDataDict = new Dictionary<int, List<UnexpectedPackageData>>();

    private Dictionary<int, List<EventData>> _eventDataDict = new Dictionary<int, List<EventData>>();
    private List<AchieveEventData> _achieveEventList = new List<AchieveEventData>();

    private Dictionary<int, PresetData[]> _charPresetDataDict = new Dictionary<int, PresetData[]>();
    private PresetData[] _questPresetDatas;
    private PresetData[] _arenaPresetDatas;
    private PresetData[] _arenaTowerPresetDatas;
    private PresetData[] _raidPresetDatas;
    private CircleData _circleData = new CircleData();
    private List<CircleData> _circleRecommendList;
    private List<CircleData> _circleJoinList;
    private List<CircleChatData> _circleChatList;
    private List<CircleNotiData> _circleNotiList;

    public GameConfig GameConfig { get { return _gameconfig; } }
    public BattleConfig BattleConfig { get { return _battleconfig; } }
    public GameTable GameTable { get { return _gametable; } }
    public GameClientTable GameClientTable { get { return _gameclienttable; } }
    public ServerData ServerData { get { return _serverdata; } }
    public UserData UserData { get { return _userdata; } }
    public List<StoreData> StoreList { get { return _storelist; } }
    public List<AchieveData> AchieveList { get { return _achievelist; } }
    public List<CharData> CharList { get { return _charlist; } }
    public List<WeaponData> WeaponList { get { return _weaponlist; } }
    public List<GemData> GemList { get { return _gemlist; } }
    public List<CardData> CardList { get { return _cardlist; } }
    public List<ItemData> ItemList { get { return _itemlist; } }
    public List<FacilityData> FacilityList { get { return _facilitylist; } }
    public List<StageClearData> StageClearList { get { return _stageclearlist; } }
    public List<WeaponBookData> WeaponBookList { get { return _weaponbooklist; } }
    public List<CardBookData> CardBookList { get { return _cardbooklist; } }
    public List<MonsterBookData> MonsterBookList { get { return _monsterbooklist; } }
    public List<TimeAttackClearData> TimeAttackClearList { get { return _timeattackclearlist; } }
    public List<TimeAttackRankData> TimeAttackRankList { get { return _timeattackranklist; } }
    public List<GllaMissionData> GllaMissionList { get { return _gllamissionlist; } }   //9998

    public List<int> CostumeList { get { return _costumelist; } }
    public List<int> RoomActionList { get { return _roomactionlist; } }
    public List<int> RoomThemaList { get { return _roomthemalist; } }
    public List<int> RoomFigureList { get { return _roomfigurelist; } }
    public List<int> RoomFuncList { get { return _roomfunclist; } }
    public List<int> UserMarkList { get { return _usermarklist; } }
    public List<RoomThemeSlotData> RoomThemeSlotList { get { return _roomthemeslotlist; } }
    public List<RoomThemeFigureSlotData> RoomThemeFigureSlotList { get { return _roomthemefigureslotlist; } }
    public List<MailData> MailList { get { return _mailList; } }
    public List<EventSetData> EventSetDataList { get { return _eventSetDataList; } }
    public bool RoomThemeFigureDetailInfo { get { return _roomthemefiguredetailinfo; } }    //787878
    //public StageInfo StageInfo { get { return m_stageInfo; } }
    public List<long> NewCardList { get { return _newcardlist; } }
    public List<long> NewWeaponList { get { return _newweaponlist; } }
    public List<long> NewGemList { get { return _newgemlist; } }
    public List<long> NewItemList { get { return _newitemlist; } }
    public List<int> NewIconList { get { return _newiconlist; } }
    public List<long> NewBadgeList { get { return _newbadgelist; } }

    public UserBattleData UserBattleData { get { return _userbattledata; } }        //배틀 데이터
    public List<BadgeData> BadgeList { get { return _badgelist; } }                 //보유 뱃지 목록
    public List<long> TeamcharList { get { return _teamcharlist; } }                //팀에 배치된 캐릭터목록
    public TeamData MatchTeam { get { return _matchteam; } }                        //매칭된 팀의 데이터
    public ArenaRankingListData ArenaRankingList { get { return _arenaRankingList; } }
    public TeamData ArenaRankerDetialData { get { return _arenaRankerDetialData; } }

    public List<PassMissionData> PassMissionData { get { return _passMissionData; } }
    public CommunityData CommunityData { get { return _communityData; } }

    public RoomThemeSlotData FriendRoomSlotData { get { return _friendRoomSlotData; } }
    public List<RoomThemeFigureSlotData> FriendRoomFigureSlotList { get { return _friendRoomFigureSlotList; } }

    public UserMonthlyData UserMonthlyData { get { return _userMonthlyData; } }
    public UserBuffEffectData UserBuffEffectData { get { return _userBuffEffectData; } }

    public WeaponArmoryData WeaponArmoryData { get { return _weaponArmoryData; } }
    
    public List<MyDyeingData> DyeingDataList { get { return _dyeingDataList; } }
    public Dictionary<int, List<UnexpectedPackageData>> UnexpectedPackageDataDict { get { return _unexpectedPackageDataDict; } }

    public Dictionary<int, List<EventData>> EventDataDict { get { return _eventDataDict; } }
    public List<AchieveEventData> AchieveEventList => _achieveEventList;

    public Dictionary<int, PresetData[]> CharPresetDataDict => _charPresetDataDict;
    public PresetData[] QuestPresetDatas => _questPresetDatas;
    public PresetData[] ArenaPresetDatas => _arenaPresetDatas;
    public PresetData[] ArenaTowerPresetDatas => _arenaTowerPresetDatas;
    public PresetData[] RaidPresetDatas => _raidPresetDatas;
    public CircleData CircleData => _circleData;
    public List<CircleData> CircleRecommendList
    {
        get
        {
            if (_circleRecommendList == null)
            {
                _circleRecommendList = new List<CircleData>();
                _circleRecommendList.Capacity = _gameconfig.CircleRecommendMaxNumber;
            }

            return _circleRecommendList;
        }
    }
    public List<CircleData> CircleJoinList
    {
        get
        {
            if (_circleJoinList == null)
            {
                _circleJoinList = new List<CircleData>();
                _circleJoinList.Capacity = _gameconfig.CircleWaitingJoinMaxNumber;
            }

            return _circleJoinList;
        }
    }

    public List<CircleChatData> CircleChatList
    {
        get
        {
            if (_circleChatList == null)
            {
                _circleChatList = new List<CircleChatData>();
                _circleChatList.Capacity = _gameconfig.ChatViewLineLimitNumber;
            }

            return _circleChatList;
        }
    }

    public List<CircleNotiData> CircleNotiList
    {
        get
        {
            if (_circleNotiList == null)
            {
                _circleNotiList = new List<CircleNotiData>();
            }

            return _circleNotiList;
        }
    }

    public bool     CircleReqChatList           { get; set; }   = false;
    public bool     CharSelete					{ get; set; }   = false;
    public bool     ContinueStage				{ get; set; }   = false;
    public bool     IsTowerStage				{ get; set; }   = false;
    public bool     IsTowerStageTestPlay        { get; set; }   = false;
    public int      SelecteStageTableId			{ get; set; }   = 0;
	public int		SelecteSecretQuestLevelId	{ get; set; }	= 0;
	public int		SelecteSecretQuestBOSetId	{ get; set; }	= 0;
	public int      SelectMultipleIndex			{ get; set; }   = 0;
    public int      MaxNormalBoxCount			{ get; set; }   = 0;
    public int      PVPGameSpeedType			{ get; set; }   = 1;
    public bool     PVPAutoSupporter			{ get; set; }   = true;
    public string   YouTubeLink					{ get; set; }   = "";
    public bool     MultibleGCFlag				{ get; set; }   = false;
    public bool     IsPrevSkillTrainingRoom		{ get; set; }   = false;

    public int                      ServerMigrationCountOnWeek  { get; private set; } = 0;
    public UnicodeCheckListTable    UnicodeCheckTable           { get; private set; } = null;

    // for Raid
    public int                  SelectedRaidLevel               { get; set; }           = 1;
    public DateTime             RaidSecretStoreChangeRemainTime { get; set; }
    public bool                 RaidAtkBuffRateFlag             { get; set; }           = false;
    public bool                 RaidHpBuffRateFlag              { get; set; }           = false;
    public bool                 IsNewRaidReocrd                 { get; set; }           = false;
    public RaidUserData         RaidUserData                    { get; private set; }   = new RaidUserData();
    public List<RaidRankData>   RaidRankDataList                { get; private set; }   = new List<RaidRankData>();
    

    // for Raid Prologue
    public bool                 IsRaidPrologue          { get; set; }           = false;
    public List<int>            RaidPrologueCharTidList { get; set; }           = new List<int>();

    public eAccountType UsedAccountTypeInGetLink { get; protected set; } = eAccountType.DEFAULT;
    
    public bool bFastStageClear { get; set; } = false;
    public long SeleteCharUID = -1;

    public int                              SelectedCardFormationTID    { get; private set; } = 0;
    public GameTable.CardFormation.Param    CardFormationTableData      { get; private set; } = null;

    public List<AllyPlayerData> ListAllyPlayerData = new List<AllyPlayerData>();

    public int StageClearID = -1;
    public bool bStageFailure = false;
    public RoomThemeSlotData UseRoomThemeData = new RoomThemeSlotData();                                //787878  현재 테마룸
    public List<RoomThemeFigureSlotData> UseRoomThemeFigureList = new List<RoomThemeFigureSlotData>();  //787878  테마룸에서 사용하는 피규어 리스트

    public PktInfoRoomThemeSlotDetail RoomThemeSlotDetail = new PktInfoRoomThemeSlotDetail();
    public List<RewardData> RewardList = new List<RewardData>();
    public int RewardGachaSupporterPoint = 0;
    public int RewardGachaDesirePoint = 0;
    public GameResultData GameResultData = new GameResultData();
    public FacilityResultData FacilityResultData = new FacilityResultData();
    public WeekMissionData WeekMissionData = new WeekMissionData();
    public int MailTotalCount = 0;// { set; get; }
    public bool TutorialSkipFlag = false;
    
    //ArenaBuffInfo
    public bool ArenaATK_Buff_Flag = false;
    public bool ArenaDEF_Buff_Flag = false;
    public bool ArenaGold_Buff_Flag = false;
    public bool ArenaGameEnd_Flag = false;      //아레나 플레이 후 로비로 왔을때 아레나 UI열어주는 플래그
    public bool bArenaLose = false;
    public bool IsFriendArenaGame = false;

    private NetConnectInfo _netconnetinfo;
    private eServerType _servertype = eServerType.NONE;
    private bool _bconnect = false;
    
    private bool _bautologin = false;
    private bool _netflag = false;

    private long _accountUUID = 0;

    private bool _reconnect = false;
    private int _timeping = -1;
    private bool _greetings = false;

    private PktInfoStageGameStartAck _pktstagegamestart = null;
    private PktInfoStageGameResultAck _pktstagegamestartresult = null;
    private PktInfoProductPack _pktproduct = null;                         //결과 보상, 가챠등

    private Socket      mSocket     = null;
    private IPEndPoint  mIPEndPoint = null;

    public bool IsReqStageEndReConnect = false;

    public bool IsConnect { get { return _bconnect; } }
    public bool netFlag { get { return _netflag; } }
    public bool Greetings { get { return _greetings; } set { _greetings = value; } }

    public PktInfoStageGameStartAck     PktStageGameStart       { get { return _pktstagegamestart; } }
    public PktInfoStageGameResultAck    PktStageGameStartResult { get { return _pktstagegamestartresult; } }
    public PktInfoProductPack           PktProduct              { get { return _pktproduct; } }
    public PktInfoArenaGameStartAck     PktArenaGameStart       { get; private set; } = null;

    private List<ProtocolData> _protocallist = new List<ProtocolData>();

    private Coroutine _timeoutcoroutine = null;

    //Friend Community
    private long _friendRoomUserUUID = 0;
    public long FriendRoomSaveUserUUID { get; set; }
    public long FriendRoomUserUUID { get { return _friendRoomUserUUID; } set { _friendRoomUserUUID = value; } }

    private List<int> _UserLobbyThemeList = new List<int>();
    public List<int> UserLobbyThemeList { get { return _UserLobbyThemeList; } }

    private List<int> _CardFormationFavorList = new List<int>();
    public List<int> CardFormationFavorList { get { return _CardFormationFavorList; } }

    private int _towerClearID = -1;
    public int TowerClearID { get { return _towerClearID; } }

    private List<long> _towercharlist = new List<long>();
    public List<long> TowercharList { get { return _towercharlist; } }      //아레나 타워 팀에 배치된 캐릭터목록

    private List<TeamData> _towerFriendTeamData = new List<TeamData>();
    public List<TeamData> TowerFriendTeamData { get { return _towerFriendTeamData; } }    //아레나 타워 - 친구들 팀의 데이터

    public ArenaTowerFriendContainer ArenaTowerFriendContainer = new ArenaTowerFriendContainer();

    public InfluenceMissionData InfluenceMissionData = new InfluenceMissionData();
    public InfluenceData InfluenceData = new InfluenceData();
    public InfluenceRankData InfluenceRankData = new InfluenceRankData();

    private RotationGachaData _ServerRotationGachaData = new RotationGachaData();
    public RotationGachaData ServerRotationGachaData { get { return _ServerRotationGachaData; } }

    private RotationGachaData _UserRotationGachaData = new RotationGachaData();
    public RotationGachaData UserRotationGachaData { get { return _UserRotationGachaData; } }

    public DailyMissionData DailyMissionData = new DailyMissionData();


    void Awake()
    {
        DontDestroyOnLoad();

        InitNetworkTime("time.google.com");
        LoadTable();            
    }

    protected override void OnApplicationQuit()
    {
        if(mSocket != null)
        {
            mSocket.Close();
        }

        if (_netflag)
        {
            if (NETStatic.Mgr != null)
                NETStatic.Mgr.DoRelease(eConnectKind.DISCONNECT_LOGOUT);
        }

        base.OnApplicationQuit();
    }

    public void LoadTable()
    {
#if UNITY_EDITOR
        PlayerPrefs.SetInt(SAVETYPE.LANGUAGE.ToString(), (int)eLANGUAGE.KOR);
#endif
        _gameconfig = (GameConfig)ResourceMgr.Instance.LoadFromAssetBundle("system", "System/GameConfig.asset");
        _battleconfig = (BattleConfig)ResourceMgr.Instance.LoadFromAssetBundle("system", "System/BattleConfig.asset");

#if UNITY_EDITOR
        if( AppMgr.Instance.configData.m_EditorTableLoadType == Config.eEditorTableLoadType.Editor )
        {
            _gametable = (GameTable)ResourceMgr.Instance.LoadFromAssetBundle("system", "System/Table/GameEditor.asset");
            _gameclienttable = (GameClientTable)ResourceMgr.Instance.LoadFromAssetBundle("system", "System/Table/GameClientEditor.asset");

        }
        else
        {
            _gametable = (GameTable)ResourceMgr.Instance.LoadFromAssetBundle("system", "System/Table/Game.asset");
            _gameclienttable = (GameClientTable)ResourceMgr.Instance.LoadFromAssetBundle("system", "System/Table/GameClient.asset");
        }
#else
        _gametable = (GameTable)ResourceMgr.Instance.LoadFromAssetBundle("system", "System/Table/Game.asset");
        _gameclienttable = (GameClientTable)ResourceMgr.Instance.LoadFromAssetBundle("system", "System/Table/GameClient.asset");
#endif

        UnicodeCheckTable = (UnicodeCheckListTable)ResourceMgr.Instance.LoadFromAssetBundle( "system", "System/Table/UnicodeCheckList.asset" );

        //테이블 로드가 끝난뒤 IAPManager초기화
        IAPManager.Instance.Init();
    }

    public void DoInitGame(bool netflag = false)
    {
        _netflag = netflag;
        if(_netflag)
            NETStatic.DoInitNetwork(_netflag);

        _reconnect = false;
        CharSelete = false;
    }

    public bool IsAccount()
    {
        bool b = PlayerPrefs.HasKey("User_AccountUUID");
        if (b == true)
            return true;
        return false;
    }

    public void MoveLobbyToTitle()
    {
        if (_netflag)
        {
            if (NETStatic.Mgr != null)
                NETStatic.Mgr.DoRelease(eConnectKind.DISCONNECT_LOGOUT);
        }

        UIValue.Instance.SetValue(UIValue.EParamType.LoadingType, (int)UILoadingPopup.eLoadingType.LobbyToTitle);
        UIValue.Instance.SetValue(UIValue.EParamType.LoadingStage, -1);

        if (AppMgr.Instance.SceneType != AppMgr.eSceneType.Lobby && AppMgr.Instance.SceneType != AppMgr.eSceneType.Title)
        {
            GameUIManager.Instance.ShowUI("LoadingPopup", false);
        }
        else
        {
            LobbyUIManager.Instance.ShowUI("LoadingPopup", false);
        }

        AppMgr.Instance.LoadScene(AppMgr.eSceneType.Title, "Title");
    }
    //----------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  Timer
    //
    //----------------------------------------------------------------------------------------------------------------------------------------------------

    private void OnTimePing()
    {
        Send_ReqPing();
    }
    
    //----------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  Get
    //
    //----------------------------------------------------------------------------------------------------------------------------------------------------
    public StoreData GetStoreData(int tableid )
    {
        return _storelist.Find(x => x.TableID == tableid);
    }


    public AchieveData GetAchieveData( int groupid )
    {
        return _achievelist.Find(x => x.GroupID == groupid);
    }

    public int GetCharDataIdx(long uid)
    {
        for (int i = 0; i < _charlist.Count; i++)
        {
            if (_charlist[i].CUID == uid)
                return i;
        }
        return -1;
    }

    public int GetCharDataTableID(long uid)
    {
        for (int i = 0; i < _charlist.Count; i++)
        {
            if (_charlist[i].CUID == uid)
                return _charlist[i].TableID;
        }
        return -1;
    }

    public CharData GetCharData(long uid)
    {
        return _charlist.Find(x => x.CUID == uid);
    }

    public CharData GetCharDataByTableID(int tableid)
    {
        return _charlist.Find(x => x.TableData.ID == tableid);
    }

    public CharData GetMainChar()
    {
        return GetCharData(_userdata.MainCharUID);
    }

    public long GetMainCharUID()
    {
        return _userdata.MainCharUID;
    }

    public void GetCharList(ref List<CharData> list)
    {
        list.Add(GetMainChar());
        for (int i = 0; i < _charlist.Count; i++)
            if (_charlist[i].CUID != _userdata.MainCharUID)
                list.Add(_charlist[i]);
    }

    public void GetCharListTableData(ref List<GameTable.Character.Param> list)
    {
        list.Add(GetMainChar().TableData);
        for (int i = 0; i < _charlist.Count; i++)
            if (_charlist[i].CUID != _userdata.MainCharUID)
                list.Add(_charlist[i].TableData);
    }

    public WeaponData GetWeaponData(long uid)
    {
        return _weaponlist.Find(x => x.WeaponUID == uid);
    }

    public GemData GetGemData(long uid)
    {
        return _gemlist.Find(x => x.GemUID == uid);
    }

    public CardData GetCardData(long uid)
    {
        return _cardlist.Find(x => x.CardUID == uid);
    }

    public CardData GetCardDataByTableId(int tid)
    {
        return _cardlist.Find(x => x.TableID == tid);
    }

    public ItemData GetItemData(long uid)
    {
        return _itemlist.Find(x => x.ItemUID == uid);
    }

    public ItemData GetItemData(int tableid)
    {
        return _itemlist.Find(x => x.TableID == tableid);
    }
    public BadgeData GetBadgeData(long uid)
    {
        return _badgelist.Find(x => x.BadgeUID == uid);
    }

    public WeaponData GetEquipGemWeaponData(long gemuid) //곡옥이 장착된 아이템
    {
        for (int i = 0; i < _weaponlist.Count; i++)
        {
            for (int j = 0; j < _weaponlist[i].SlotGemUID.Length; j++)
                if (_weaponlist[i].SlotGemUID[j] == gemuid)
                    return _weaponlist[i];
        }
        return null;
    }

    public CharData GetEquipWeaponCharData(long weaponuid) //아이템을 장착한 캐릭터
    {
        for (int i = 0; i < _charlist.Count; i++)
        {
            if (_charlist[i].EquipWeaponUID == weaponuid || _charlist[i].EquipWeapon2UID == weaponuid)
                return _charlist[i];
        }
        return null;
    }

    public CharData GetEquiCardCharData(long carduid) //아이템을 장착한 캐릭터
    {
        for (int i = 0; i < _charlist.Count; i++)
        {
            if (_charlist[i].IsEquipCard(carduid))
                return _charlist[i];
        }
        return null;
    }

    public FacilityData GetEquipCharFacilityData(long charuid)
    {
        for(int i = 0; i < _facilitylist.Count; i++)
        {
            if (_facilitylist[i].TableData.EffectType == "FAC_CHAR_EXP" || _facilitylist[i].TableData.EffectType == "FAC_CHAR_SP")
            {
                if (_facilitylist[i].Selete == charuid)
                    return _facilitylist[i];
            }
        }
        return null;
    }

    public FacilityData GetEquiCardFacilityData(long carduid) //아이템을 장착한 캐릭터
    {
        for (int i = 0; i < _facilitylist.Count; i++)
        {
            if (_facilitylist[i].EquipCardUID == carduid)
                return _facilitylist[i];
        }
        return null;
    }

    public FacilityData GetEquipWeaponFacilityData(long weaponuid)
    {
        for(int i = 0; i < _facilitylist.Count; i++)
        {
            //if (_facilitylist[i].TableData.EffectType == "FAC_WEAPON_EXP" ||
            //    _facilitylist[i].TableData.EffectType != "FAC_ITEM_COMBINE")
            if (_facilitylist[i].TableData.EffectType == "FAC_WEAPON_EXP")
            {
                if (_facilitylist[i].Selete == weaponuid)
                    return _facilitylist[i];
            }
                
        }

        return null;
    }
    public BadgeData GetEquipBadgeData(long badgeuid)
    {
        BadgeData badgedata = _badgelist.Find(x => x.BadgeUID == badgeuid);
        if(badgedata != null)
        {
            if (badgedata.PosKind != (int)eContentsPosKind._NONE_)
                return badgedata;
        }
        //for(int i = 0; i < _badgelist.Count; i++)
        //{
        //    if (_badgelist[i].PosKind != (int)eContentsPosKind._NONE_)
        //        return _badgelist[i];
        //}

        return null;
    }

    public bool HasCostume(int tableid)
    {
        int id = _costumelist.Find(x => x == tableid);
        if (id == 0)
            return false;
        return true;
    }

    public FacilityData GetFacilityData(int tableid)
    {
        return _facilitylist.Find(x => x.TableID == tableid);
    }


    public RoomThemeSlotData GetRoomThemeSlotData( int slotnum )
    {
        return _roomthemeslotlist.Find(x => x.SlotNum == slotnum);
    }

    public AchieveEventData GetAchieveEventData(int groupId, int achieveGroupId)
    {
        return _achieveEventList.Find(x => x.GroupId == groupId && x.AchieveGroupId == achieveGroupId);
    }

    /// <summary>
    /// 룸테마 슬롯에 있는 피규어 리스트 정보를 얻어온다.
    /// </summary>
    /// <param name="slotnum"></param>
    /// <returns></returns>
    public List<RoomThemeFigureSlotData> GetFigureListRoomThemeInSlotData(int slotnum)
    {
        return _roomthemefigureslotlist.FindAll(x => x.RoomThemeSlotNum == slotnum);
    }

    /// <summary>
    /// 룸테마 슬롯에 피규어가 있는지.
    /// </summary>
    /// <param name="slotNum"></param>
    /// <param name="tablieId"></param>
    /// <returns></returns>
    public bool IsPlacementFigureInRoomTheme(int slotNum, int tablieId)
    {
        var _figureList = GetFigureListRoomThemeInSlotData(slotNum);
        if (_figureList == null)
            return false;

        var _figure = _figureList.Find(x => x.TableID == tablieId);

        return (_figure != null);
    }

    public bool IsRoomThema(int tableid)
    {
        int id = _roomthemalist.Find(x => x == tableid);
        if (id <= 0)
            return false;
        return true;
    }

    public bool IsRoomAction(int tableid)
    {
        int id = _roomactionlist.Find(x => x == tableid);
        if (id <= 0)
            return false;
        return true;
    }

    public bool IsRoomFunc(int tableid)
    {
        int id = _roomfunclist.Find(x => x == tableid);
        if (id <= 0)
            return false;
        return true;
    }

    public bool IsUserMark(int tableid)
    {
        int id = _usermarklist.Find(x => x == tableid);
        if (id <= 0)
            return false;
        return true;
    }

    public bool IsCircleMark(eCircleMarkType circleFlagType, int tableId)
    {
        int index = tableId % 100 - 1;
        byte longBitMaxCount = 64;
        int arrayIndex = index / longBitMaxCount;
        
        long bitFlag = 0;
        switch (circleFlagType)
        {
            case eCircleMarkType.FLAG:
                {
                    bitFlag = _circleData.FlagBit[arrayIndex];
                }
                break;
            case eCircleMarkType.MARK:
                {
                    bitFlag = _circleData.MarkBit[arrayIndex];
                }
                break;
            case eCircleMarkType.COLOR:
                {
                    bitFlag = _circleData.ColorBit[arrayIndex];
                }
                break;
        }

        int bitIndex = index - longBitMaxCount * arrayIndex;
        return (bitFlag & (1 << bitIndex)) != 0;
    }

    public bool IsCircleChatStamp(int index)
    {
        byte longBitMaxCount = 64;
        int arrayIndex = index / longBitMaxCount;

        long bitFlag = _userdata.CircleChatStampBit[arrayIndex];

        int bitIndex = index - longBitMaxCount * arrayIndex;
        return (bitFlag & (1 << bitIndex)) != 0;
    }

    public bool HasFigure(int tableId)
    {
        int find = _roomfigurelist.Find(x => x == tableId);
        if (find <= 0)
            return false;
        return true;
    }

    public void UseRoomDataClear()
    {
        UseRoomThemeData.Init();
        UseRoomThemeFigureList.Clear();
    }

    public void UseRoomDataMoveRoom( int roomtableid )
    {
        // 현재는 무조건 0번 슬롯만 사용하기로 함 -dc-
        //var _slotTheme = RoomThemeSlotList.Find(x => x.TableID == roomtableid);
        //int _index = RoomThemeSlotList.Count;
        //if (_slotTheme != null)
        //    _index = _slotTheme.SlotNum;
        UseRoomThemeData = new RoomThemeSlotData(0, roomtableid);
        UseRoomThemeFigureList.Clear();
    }

    public void UseRoomDataCopySelRoomSlot()
    {
        RoomThemeSlotData roomslotdata = GetRoomThemeSlotData(_userdata.RoomThemeSlot);
        if (roomslotdata == null)
        {
            Debug.Log("UseRoomDataCopySelRoomSlot Init Error");
            return;
        }
        UseRoomThemeData = roomslotdata.DeepCopy();
        UseRoomThemeFigureList.Clear();

        var list = GameInfo.Instance.GetFigureListRoomThemeInSlotData(_userdata.RoomThemeSlot);
        for (int i = 0; i < list.Count; i++)
            UseRoomThemeFigureList.Add(list[i].DeepCopy());
    }

    public GameTable.Stage.Param GetStageData(int tableId)
    {
        GameTable.Stage.Param param = _gametable.FindStage(tableId);
        if (param == null)
        {
            Debug.LogError(tableId + "번 스테이지 데이터가 없습니다.");
            return null;
        }
        return param;
    }

    public GameTable.Character.Param GetCharacterData(int tableId)
    {
        GameTable.Character.Param param = _gametable.FindCharacter(tableId);
        if (param == null)
        {
            Debug.LogError(tableId + "번 캐릭터 데이터가 없습니다.");
            return null;
        }

        return param;
    }

    public GameClientTable.Monster.Param GetMonsterData(int tableId)
    {
        GameClientTable.Monster.Param param = _gameclienttable.FindMonster(tableId);
        if (param == null)
        {
            Debug.LogError(tableId + "번 몬스터 데이터가 없습니다.");
            return null;
        }

        return param;
    }

    public int GetItemIDCount(int tableid)
    {
        int total = 0;
        for (int i = 0; i < _itemlist.Count; i++)
        {
            if (_itemlist[i].TableID == tableid)
            {
                total += _itemlist[i].Count;
            }
        }
        return total;
    }

   
    public CardBookData GetCardBookData(int tableid)
    {
        return _cardbooklist.Find(x => x.TableID == tableid);
    }

    public WeaponBookData GetWeaponBookData(int tableid)
    {
        return _weaponbooklist.Find(x => x.TableID == tableid);
    }

    public MonsterBookData GetMonsterBookData(int tableId)
    {
        return _monsterbooklist.Find(x => x.TableID == tableId);
    }

    public MailData GetMailData(ulong uid)
    {
        return MailList.Find(x => x.MailUID == uid);
    }

    public EventSetData GetEventSetData(int tableid)
    {
        return EventSetDataList.Find(x => x.TableID == tableid);
    }
 
    public TimeAttackClearData GetTimeAttackClearData( int tableid )
    {
        return _timeattackclearlist.Find(x => x.TableID == tableid);
    }

    public TimeAttackRankData GetTimeAttackRankData(int tableid)
    {
        return _timeattackranklist.Find(x => x.TableID == tableid);
    }

    public TimeAttackRankUserData GetTimeAttackRankUserData(int tableid, long uuid )
    {
        var data = GetTimeAttackRankData(tableid);
        if (data == null)
            return null;
        
        return data.RankUserList.Find(x => x.UUID == uuid);
    }
    
    public MyDyeingData GetDyeingData(int tableId)
    {
        return _dyeingDataList.Find(x => x.TableId == tableId);
    }

    public List<UnexpectedPackageData> GetUnexpectedPackageFirstByType()
    {
        List<UnexpectedPackageData> result = new List<UnexpectedPackageData>();

        DateTime serverTime = GameSupport.GetCurrentServerTime();
        foreach(KeyValuePair<int, List<UnexpectedPackageData>> pair in _unexpectedPackageDataDict)
        {
            foreach (UnexpectedPackageData data in pair.Value)
            {
                if (data.IsPurchase)
                {
                    continue;
                }
                
                if (serverTime < data.EndTime)
                {
                    result.Add(data);
                    break;
                }
            }
        }
        
        // Sort
        result.Sort((x, y) => x.EndTime < y.EndTime ? -1 : 1);
        
        return result;
    }

    public EventData GetEventData(int eventType, int eventGroupId)
    {
        if (_eventDataDict.ContainsKey(eventType) == false)
        {
            _eventDataDict.Add(eventType, new List<EventData>());
        }

        EventData eventData = _eventDataDict[eventType].Find(x => x.GroupID == eventGroupId);
        if (eventData == null)
        {
            switch ((eLobbyEventType)eventType)
            {
                case eLobbyEventType.Bingo:
                    {
                        GameTable.BingoEvent.Param bingoEventParam = GameTable.FindBingoEvent(x => x.ID == eventGroupId);
                        GameTable.BingoEventData.Param bingoEventDataParam = GameTable.FindAllBingoEventData(x => x.GroupID == eventGroupId).FirstOrDefault();
                        if (bingoEventParam != null && bingoEventDataParam != null)
                        {
                            eventData = new EventData()
                            {
                                GroupID = eventGroupId,
                                No = bingoEventDataParam.No,
                                RwdFlag = 0,
                            };

                            _eventDataDict[eventType].Add(eventData);
                        }
                    } break;
                case eLobbyEventType.Achieve:
                    {
                        GameTable.AchieveEvent.Param achieveEventParam = GameTable.FindAchieveEvent(x => x.ID == eventGroupId);
                        GameTable.AchieveEventData.Param achieveEventDataParam = GameTable.FindAllAchieveEventData(x => x.GroupID == eventGroupId).FirstOrDefault();
                        if (achieveEventParam != null && achieveEventDataParam != null)
                        {
                            eventData = new EventData()
                            {
                                GroupID = eventGroupId,
                                No = achieveEventDataParam.AchieveGroup,
                                RwdFlag = 0,
                            };

                            _eventDataDict[eventType].Add(eventData);
                        }
                    }
                    break;
            }
        }

        return eventData;
    }
    
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  New 관련 
    //
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void CheckAllRedDotCard()
    {
        for (int i = 0; i < GameInfo.Instance.CardList.Count; i++)
            GameInfo.Instance.CardList[i].CheckRedDot();
    }
    public void CheckAllRedDotWeapon()
    {
        for (int i = 0; i < GameInfo.Instance.WeaponList.Count; i++)
            GameInfo.Instance.WeaponList[i].CheckRedDot();
    }        
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  New 관련 
    //
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  서포터 New 관련 
    //
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void AddNewCard(long udid)
    {
        if (!IsNewCard(udid))
            _newcardlist.Add(udid);
    }

    public bool IsNewCard(long udid)
    {
        long id = _newcardlist.Find(x => x == udid);
        if (id == 0)
            return false;
        return true;
    }

    public void LoadNewCard()
    {
        _newcardlist.Clear();
        int Count = FSaveData.Instance.GetInt("NCardList_Count");
        for (int i = 0; i < Count; i++)
        {
            long udID = long.Parse(FSaveData.Instance.GetString("NCardList_CardUID_" + i.ToString()));
            _newcardlist.Add(udID);
        }

        for (int i = 0; i < _newcardlist.Count; i++)
        {
            var data = GetCardData(_newcardlist[i]);
            if (data != null)
                data.New = true;
        }
    }
    public void SaveNewCard()
    {
        int nCount = _newcardlist.Count;

        FSaveData.Instance.SetInt("NCardList_Count", nCount);
        for (int i = 0; i < nCount; i++)
        {
            FSaveData.Instance.SetString("NCardList_CardUID_" + i.ToString(), _newcardlist[i].ToString());
        }

        for (int i = 0; i < _newcardlist.Count; i++)
        {
            var data = GetCardData(_newcardlist[i]);
            if (data != null)
                data.New = true;
        }
    }

    public void DeleteNewCard()
    {
        for (int i = 0; i < _newcardlist.Count; i++)
        {
            var data = GetCardData(_newcardlist[i]);
            if (data != null)
                data.New = false;
        }
        int nCount = _newcardlist.Count;
        if (nCount != 0)
        {
            FSaveData.Instance.DeleteKey("NCardList_Count");
            for (int i = 0; i < nCount; i++)
                FSaveData.Instance.DeleteKey("NCardList_CardUID_" + i.ToString());
        }
        _newcardlist.Clear();
    }
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  무기 New 관련 
    //
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void AddNewWeapon(long udid)
    {
        if (!IsNewWeapon(udid))
            _newweaponlist.Add(udid);
    }

    public bool IsNewWeapon(long udid)
    {
        long id = _newweaponlist.Find(x => x == udid);
        if (id == 0)
            return false;
        return true;
    }

    public void LoadNewWeapon()
    {
        _newweaponlist.Clear();
        int Count = FSaveData.Instance.GetInt("NWeaponList_Count");
        for (int i = 0; i < Count; i++)
        {
            long udID = long.Parse(FSaveData.Instance.GetString("NWeaponList_WeaponUID_" + i.ToString()));
            _newweaponlist.Add(udID);
        }

        for (int i = 0; i < _newweaponlist.Count; i++)
        {
            var data = GetWeaponData(_newweaponlist[i]);
            if (data != null)
                data.New = true;
        }
    }
    public void SaveNewWeapon()
    {
        int nCount = _newweaponlist.Count;
        FSaveData.Instance.SetInt("NWeaponList_Count", nCount);
        for (int i = 0; i < nCount; i++)
        {
            FSaveData.Instance.SetString("NWeaponList_WeaponUID_" + i.ToString(), _newweaponlist[i].ToString());
        }

        for (int i = 0; i < _newweaponlist.Count; i++)
        {
            var data = GetWeaponData(_newweaponlist[i]);
            if (data != null)
                data.New = true;
        }
    }

    public void DeleteNewWeapon()
    {
        for (int i = 0; i < _newweaponlist.Count; i++)
        {
            var data = GetWeaponData(_newweaponlist[i]);
            if (data != null)
                data.New = false;
        }
        int nCount = _newweaponlist.Count;
        if (nCount != 0)
        {
            FSaveData.Instance.DeleteKey("NWeaponList_Count");
            for (int i = 0; i < nCount; i++)
                FSaveData.Instance.DeleteKey("NWeaponList_WeaponUID_" + i.ToString());
        }
        _newweaponlist.Clear();
    }
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  곡옥 New 관련 
    //
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void AddNewGem(long udid)
    {
        if (!IsNewGem(udid))
            _newgemlist.Add(udid);
    }

    public bool IsNewGem(long udid)
    {
        long id = _newgemlist.Find(x => x == udid);
        if (id == 0)
            return false;
        return true;
    }

    public void LoadNewGem()
    {
        _newgemlist.Clear();
        int Count = FSaveData.Instance.GetInt("NGemList_Count");
        for (int i = 0; i < Count; i++)
        {
            long udID = long.Parse(FSaveData.Instance.GetString("NGemList_GemUID_" + i.ToString()));
            _newgemlist.Add(udID);
        }

        for (int i = 0; i < _newgemlist.Count; i++)
        {
            var data = GetGemData(_newgemlist[i]);
            if (data != null)
                data.New = true;
        }
    }
    public void SaveNewGem()
    {
        int nCount = _newgemlist.Count;

        FSaveData.Instance.SetInt("NGemList_Count", nCount);
        for (int i = 0; i < nCount; i++)
        {
            FSaveData.Instance.SetString("NGemList_GemUID_" + i.ToString(), _newgemlist[i].ToString());
        }

        for (int i = 0; i < _newgemlist.Count; i++)
        {
            var data = GetGemData(_newgemlist[i]);
            if (data != null)
                data.New = true;
        }
    }

    public void DeleteNewGem()
    {
        for (int i = 0; i < _newgemlist.Count; i++)
        {
            var data = GetGemData(_newgemlist[i]);
            if (data != null)
                data.New = false;
        }
        int nCount = _newgemlist.Count;
        if (nCount != 0)
        {
            FSaveData.Instance.DeleteKey("NGemList_Count");
            for (int i = 0; i < nCount; i++)
                FSaveData.Instance.DeleteKey("NGemList_GemUID_" + i.ToString());
        }
        _newgemlist.Clear();
    }
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  아이템 New 관련 
    //
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void AddNewItem(long udid)
    {
        if (!IsNewItem(udid))
            _newitemlist.Add(udid);
    }

    public bool IsNewItem(long udid)
    {
        long id = _newitemlist.Find(x => x == udid);
        if (id == 0)
            return false;
        return true;
    }

    public void LoadNewItem()
    {
        _newitemlist.Clear();
        int Count = FSaveData.Instance.GetInt("NItemList_Count");
        for (int i = 0; i < Count; i++)
        {
            long udID = long.Parse(FSaveData.Instance.GetString("NItemList_ItemUID_" + i.ToString()));
            _newitemlist.Add(udID);
        }

        for (int i = 0; i < _newitemlist.Count; i++)
        {
            var data = GetItemData(_newitemlist[i]);
            if (data != null)
                data.New = true;
        }
    }
    public void SaveNewItem()
    {
        int nCount = _newitemlist.Count;

        FSaveData.Instance.SetInt("NItemList_Count", nCount);
        for (int i = 0; i < nCount; i++)
        {
            FSaveData.Instance.SetString("NItemList_ItemUID_" + i.ToString(), _newitemlist[i].ToString());
        }

        for (int i = 0; i < _newitemlist.Count; i++)
        {
            var data = GetItemData(_newitemlist[i]);
            if (data != null)
                data.New = true;
        }
    }

    public void DeleteNewItem()
    {
        for (int i = 0; i < _newitemlist.Count; i++)
        {
            var data = GetItemData(_newitemlist[i]);
            if (data != null)
                data.New = false;
        }
        int nCount = _newitemlist.Count;
        if (nCount != 0)
        {
            FSaveData.Instance.DeleteKey("NItemList_Count");
            for (int i = 0; i < nCount; i++)
                FSaveData.Instance.DeleteKey("NItemList_ItemUID_" + i.ToString());
        }
        _newitemlist.Clear();
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  유저마크 New 관련 
    //
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void AddNewIcon(int tableid)
    {
        if (!IsNewIcon(tableid))
            _newiconlist.Add(tableid);
    }

    public bool IsNewIcon(int tableid)
    {
        
        long id = _newiconlist.Find(x => x == tableid);
        if (id == 0)
            return false;
        return true;
    }

    public void LoadNewIcon()
    {
        _newiconlist.Clear();
        int Count = PlayerPrefs.GetInt("NIconList_Count");
        for (int i = 0; i < Count; i++)
        {
            int tableid = PlayerPrefs.GetInt("NIconList_TableID_" + i.ToString());
            _newiconlist.Add(tableid);
        }
    }
    public void SaveNewIcon()
    {
        int nCount = _newiconlist.Count;

        PlayerPrefs.SetInt("NIconList_Count", nCount);
        for (int i = 0; i < nCount; i++)
        {
            PlayerPrefs.SetInt("NIconList_TableID_" + i.ToString(), _newiconlist[i]);
        }
    }

    public void DeleteNewIcon()
    {
        for (int i = 0; i < _newiconlist.Count; i++)
        {
            var data = GetItemData(_newiconlist[i]);
            if (data != null)
                data.New = false;
        }
        int nCount = _newiconlist.Count;
        if (nCount != 0)
        {
            PlayerPrefs.DeleteKey("NIconList_Count");
            for (int i = 0; i < nCount; i++)
                PlayerPrefs.DeleteKey("NIconList_TableID_" + i.ToString());
        }
        _newiconlist.Clear();
    }
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  문양 New 관련 
    //
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void AddNewBadge(long uuid)
    {
        if (!IsNewBadge(uuid))
            _newbadgelist.Add(uuid);
    }

    public bool IsNewBadge(long uuid)
    {
        long id = _newbadgelist.Find(x => x == uuid);
        if (id == 0)
            return false;

        return true;
    }
    public void LoadNewBadge()
    {
        _newbadgelist.Clear();
        int Count = PlayerPrefs.GetInt("NBadgeList_Count");
        for(int i = 0; i < Count; i++)
        {
            long uuid = long.Parse(PlayerPrefs.GetString("NBadgeList_BadgeUID_" + i.ToString()));
            _newbadgelist.Add(uuid);
        }

        for(int i = 0; i < _newbadgelist.Count; i++)
        {
            var data = GetBadgeData(_newbadgelist[i]);
            if (data != null)
                data.New = true;
        }
    }

    public void SaveNewBadge()
    {
        int nCount = _newbadgelist.Count;
        PlayerPrefs.SetInt("NBadgeList_Count", nCount);
        for (int i = 0; i < nCount; i++)
        {
            PlayerPrefs.SetString("NBadgeList_BadgeUID_" + i.ToString(), _newbadgelist[i].ToString());
        }

        for (int i = 0; i < _newbadgelist.Count; i++)
        {
            var data = GetBadgeData(_newbadgelist[i]);
            if (data != null)
                data.New = true;
        }
    }

    public void DeleteNewBadge()
    {
        for (int i = 0; i < _newbadgelist.Count; i++)
        {
            var data = GetBadgeData(_newbadgelist[i]);
            if (data != null)
                data.New = false;
        }

        int nCount = _newbadgelist.Count;
        if (nCount != 0)
        {
            PlayerPrefs.DeleteKey("NBadgeList_Count");
            for (int i = 0; i < nCount; i++)
                PlayerPrefs.DeleteKey("NBadgeList_BadgeUID_" + i.ToString());
        }

        _newbadgelist.Clear();
    }
    //----------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  net pkt Set
    //
    //----------------------------------------------------------------------------------------------------------------------------------------------------

    public void SetPktInitData()
    {
        _userdata.AccountCode = string.Empty;

        if (_facilitylist != null)
            _facilitylist.Clear();
        if (_eventSetDataList != null)
            _eventSetDataList.Clear();
        if (_costumelist != null)
            _costumelist.Clear();
        if (_weaponlist != null)
            _weaponlist.Clear();
        if (_itemlist != null)
            _itemlist.Clear();
        if (_gemlist != null)
            _gemlist.Clear();
        if (_stageclearlist != null)
            _stageclearlist.Clear();
        if (_weaponbooklist != null)
            _weaponbooklist.Clear();
        if (_cardbooklist != null)
            _cardbooklist.Clear();
        if (_monsterbooklist != null)
            _monsterbooklist.Clear();
        if (_charlist.Count > 0)
        {
            for (int i = 0; i < _charlist.Count; i++)
            {
                if (_charlist[i] != null && _charlist[i].PassvieList != null)
                    _charlist[i].PassvieList.Clear();
            }
        }
        if (_charlist != null)
            _charlist.Clear();
        if (_cardlist != null)
            _cardlist.Clear();
        if (_storelist != null)
            _storelist.Clear();
        if (_achievelist != null)
            _achievelist.Clear();
        if (_roomactionlist != null)
            _roomactionlist.Clear();
        if (_roomthemalist != null)
            _roomthemalist.Clear();
        if (_roomfigurelist != null)
            _roomfigurelist.Clear();
        if (_roomfunclist != null)
            _roomfunclist.Clear();
        if (_roomthemeslotlist != null)
            _roomthemeslotlist.Clear();
        if (_roomthemefigureslotlist != null)
            _roomthemefigureslotlist.Clear();
        if (_DispatchList != null)
            _DispatchList.Clear();

        GameResultData.Init();
        FacilityResultData.Init();

        /*if (GameResultData != null)
            GameResultData = null;
        if (FacilityResultData != null)
            FacilityResultData = null;*/

        if (RewardList != null)
            RewardList.Clear();

        StageClearID = -1;
        bStageFailure = false;

        UIValue.Instance.ClearParameter();

        if (DailyMissionData != null)
            DailyMissionData.Infos.Clear();
    }

    public void SetPktCharList(List<PktInfoChar> charInfo_)
    {
        _charlist.Clear();
        for (int i = 0; i < charInfo_.Count; i++)
            _charlist.Add(new CharData());

        for (int i = 0; i < charInfo_.Count; i++)
        {
            _charlist[i].SetPktData(charInfo_[i]);
        }

        _userdata.SetPktData(charInfo_);
    }

    public void SetPktCharSkillList(PktInfoCharSkill skillInfo_)
    {
        for (int i = 0; i < _charlist.Count; i++)
            _charlist[i].PassvieList.Clear();

        for (int i = 0; i < skillInfo_.infos_.Count; i++)
        {
            var skill = skillInfo_.infos_[i];
            var tabledata = GameInfo.Instance.GameTable.FindCharacterSkillPassive((int)skill.tableID_);
            if (tabledata != null)
            {
                var chardata = _charlist.Find(x => x.TableID == tabledata.CharacterID);
                if (chardata != null)
                {
                    chardata.PassvieList.Add(new PassiveData((int)skill.tableID_, (int)skill.lv_));
                }
            }
        }
    }
    public void SetPktFacilityList(PktInfoFacility facilityInfo_)
    {
        _facilitylist.Clear();

        for (int i = 0; i < GameTable.Facilitys.Count; i++)
            _facilitylist.Add(new FacilityData(GameTable.Facilitys[i].ID, 0, GameSupport.GetCurrentServerTime()));

        if (facilityInfo_.infos_.Count > 0)
        {
            for (int i = 0; i < facilityInfo_.infos_.Count; i++)
            {
                FacilityData fd = GetFacilityData((int)facilityInfo_.infos_[i].tableID_);
                if (fd != null)
                {
                    fd.SetPktData(facilityInfo_.infos_[i]);
                }
            }
        }
    }

    public void SetPktEventSetDataList(PktInfoEventReward eventInfo_)
    {
        _eventSetDataList.Clear();

        for (int i = 0; i < eventInfo_.infos_.Count; i++)
            _eventSetDataList.Add(new EventSetData());

        for (int i = 0; i < eventInfo_.infos_.Count; i++)
        {
            _eventSetDataList[i].SetPktData(eventInfo_.infos_[i]);
        }
    }

    public void UpdatePktEventSetDataList(PktInfoEventReward eventInfo_)
    {
        for (int i = 0; i < eventInfo_.infos_.Count; i++)
        {
            EventSetData tempData = _eventSetDataList.Find(x => x.TableID == eventInfo_.infos_[i].tableID_);
            if (tempData != null)
            {
                GameTable.EventSet.Param eventTableData = GameTable.FindEventSet(x => x.EventID == eventInfo_.infos_[i].tableID_);
                if (eventTableData.EventType == (int)eEventRewardKind.RESET_LOTTERY)
                {
                    tempData.UpdatePktData(eventInfo_.infos_[i]);
                }
                else if (eventTableData.EventType == (int)eEventRewardKind.EXCHANGE)
                {
                    EventSetData eventRewardData = _eventSetDataList.Find(x => x.TableID == eventInfo_.infos_[i].tableID_ && x.RewardStep == (int)eventInfo_.infos_[i].step_);
                    if (eventRewardData != null)
                    {
                        eventRewardData.UpdatePktData(eventInfo_.infos_[i]);
                    }
                    else
                    {
                        EventSetData newEvent = new EventSetData();
                        newEvent.SetPktData(eventInfo_.infos_[i]);
                        _eventSetDataList.Add(newEvent);
                    }
                }
                else if (eventTableData.EventType == (int)eEventRewardKind.MISSION)
                {

                }

            }
            else
            {
                EventSetData newEvent = new EventSetData();
                newEvent.SetPktData(eventInfo_.infos_[i]);
                _eventSetDataList.Add(newEvent);
            }
        }
    }

    public void SetPktCostumeList(PktInfoCostume costumeInfo_)
    {
        _costumelist.Clear();
        _dyeingDataList.Clear();
        for (int i = 0; i < costumeInfo_.infos_.Count; i++)
        {
            var info = costumeInfo_.infos_[i];
            
            _costumelist.Add((int)info.tableID_);
            _dyeingDataList.Add(new MyDyeingData(info));
        }
    }
    
    public void SetPktCardList(PktInfoCard cardInfo_)
    {
        _cardlist.Clear();
        for (int i = 0; i < cardInfo_.infos_.Count; i++)
            _cardlist.Add(new CardData());

        for (int i = 0; i < cardInfo_.infos_.Count; i++)
            _cardlist[i].SetPktData(cardInfo_.infos_[i]);

        ApplySetCardPos();
    }

    public void SetPktSaleStore(PktInfoStoreSale storeStoreSaleInfo_)
    {
        _storelist.Clear();
        for (int i = 0; i < storeStoreSaleInfo_.infos_.Count; i++)
        {
            var data = new StoreData();
            data.SetPktData(storeStoreSaleInfo_.infos_[i]);
            _storelist.Add(data);
        }
    }

    public void SetPktAchieves(PktInfoAchieve achieves_)
    {
        _achievelist.Clear();
        Log.Show("achieves_ " + achieves_.infos_.Count, Log.ColorType.Red);
        for (int i = 0; i < achieves_.infos_.Count; i++)
        {
            var data = new AchieveData();
            data.SetPktData(achieves_.infos_[i]);
            _achievelist.Add(data);
        }
    }

    public void SetPktAchieveEvents(PktInfoAchieveEvent pktInfoAchieve)
    {
        _achieveEventList.Clear();
        foreach (PktInfoAchieveEvent.Piece info in pktInfoAchieve.infos_)
        {
            _achieveEventList.Add(new AchieveEventData(info));
        }
    }

    public void SetPktWeaponList(PktInfoWeapon weaponInfo_)
    {
        _weaponlist.Clear();
        for (int i = 0; i < weaponInfo_.infos_.Count; i++)
            _weaponlist.Add(new WeaponData());

        for (int i = 0; i < weaponInfo_.infos_.Count; i++)
            _weaponlist[i].SetPktData(weaponInfo_.infos_[i]);
    }
    public void SetPktItemList(PktInfoItem itemInfo_)
    {
        _itemlist.Clear();
        for (int i = 0; i < itemInfo_.infos_.Count; i++)
            _itemlist.Add(new ItemData());

        for (int i = 0; i < itemInfo_.infos_.Count; i++)
            _itemlist[i].SetPktData(itemInfo_.infos_[i]);
    }
    public void SetPktGemList(PktInfoGem gemInfo_)
    {
        _gemlist.Clear();
        for (int i = 0; i < gemInfo_.infos_.Count; i++)
            _gemlist.Add(new GemData());

        for (int i = 0; i < gemInfo_.infos_.Count; i++)
            _gemlist[i].SetPktData(gemInfo_.infos_[i]);
    }
    public void SetPktStageClearList(PktInfoStageClear stageClearInfo_)
    {
        _stageclearlist.Clear();
        for (int i = 0; i < stageClearInfo_.infos_.Count; i++)
            _stageclearlist.Add(new StageClearData());
        for (int i = 0; i < stageClearInfo_.infos_.Count; i++)
            _stageclearlist[i].SetPktData(stageClearInfo_.infos_[i]);
    }

    public void SetPktWeaponBookList(PktInfoWeaponBook weaponBookInfo_)
    {
        _weaponbooklist.Clear();

        for (int i = 0; i < weaponBookInfo_.infos_.Count; i++)
            _weaponbooklist.Add(new WeaponBookData());
        for (int i = 0; i < weaponBookInfo_.infos_.Count; i++)
            _weaponbooklist[i].SetPktData(weaponBookInfo_.infos_[i]);
    }
    public void SetPktCardBookList(PktInfoCardBook cardBookInfo_)
    {
        _cardbooklist.Clear();

        for (int i = 0; i < cardBookInfo_.infos_.Count; i++)
            _cardbooklist.Add(new CardBookData());
        for (int i = 0; i < cardBookInfo_.infos_.Count; i++)
            _cardbooklist[i].SetPktData(cardBookInfo_.infos_[i]);
    }
    public void SetPktMonsterBookList(PktInfoMonsterBook monsterBookInfo_)
    {
        _monsterbooklist.Clear();

        for (int i = 0; i < monsterBookInfo_.infos_.Count; i++)
            _monsterbooklist.Add(new MonsterBookData());
        for (int i = 0; i < monsterBookInfo_.infos_.Count; i++)
            _monsterbooklist[i].SetPktData(monsterBookInfo_.infos_[i]);
    }

    public void SetPktTimeAttackClearList(PktInfoTimeAtkStageRec timeatkInfo_)
    {
        _timeattackclearlist.Clear();
        for (int i = 0; i < timeatkInfo_.infos_.Count; i++)
            _timeattackclearlist.Add(new TimeAttackClearData());
        for (int i = 0; i < timeatkInfo_.infos_.Count; i++)
            _timeattackclearlist[i].SetPktData(timeatkInfo_.infos_[i]);
    }

    public void ApplyPktTimeAttackClearList(PktInfoTimeAtkStageRec timeatkInfo_)
    {
        for (int i = 0; i < timeatkInfo_.infos_.Count; i++)
        {
            var data = _timeattackclearlist.Find(x => x.TableID == (int)timeatkInfo_.infos_[i].stageTID_);
            if (data == null)
            {
                var addata = new TimeAttackClearData();
                addata.SetPktData(timeatkInfo_.infos_[i]);
                _timeattackclearlist.Add(addata);
            }
            else
            {
                data.SetPktData(timeatkInfo_.infos_[i]);
            }

        }
    }

    public void SetPktMailCount(ushort mailCount)
    {
        MailTotalCount = mailCount;
    }

    public void SetPktInfoRoomPurchase(PktInfoRoomPurchase roompurchase)
    {
        _roomactionlist.Clear();
        _roomthemalist.Clear();
        _roomfigurelist.Clear();
        _roomfunclist.Clear();

        for (int i = 0; i < roompurchase.infos_.Count; i++)
        {
            SetPktInfoRoomPurchaseOne(roompurchase.infos_[i]);
        }
    }
    public void SetPktInfoRoomPurchaseOne(PktInfoRoomPurchase.Piece piece)
    {
        eREWARDTYPE rtype = piece.type_;
        int tableid = (int)piece.tableID_;
        if (rtype == eREWARDTYPE.ROOMTHEME)                 //룸테마    
            _roomthemalist.Add(tableid);
        else if (rtype == eREWARDTYPE.ROOMFUNC)                 //룸기능
            _roomfunclist.Add(tableid);
        else if (rtype == eREWARDTYPE.ROOMACTION)                 //룸액션
            _roomactionlist.Add(tableid);
        else if (rtype == eREWARDTYPE.ROOMFIGURE)                 //룸피규어
            _roomfigurelist.Add(tableid);
    }

    public void SetPktInfoRoomThemeSlot(PktInfoRoomThemeSlot roomthemeslot)
    {
        _roomthemeslotlist.Clear();

        for (int i = 0; i < roomthemeslot.infos_.Count; i++)
            SetPktInfoRoomThemeSlotOne(roomthemeslot.infos_[i]);
    }
    public void SetPktInfoRoomThemeSlotOne(PktInfoRoomThemeSlot.Piece piece)
    {
        var inst = new RoomThemeSlotData();
        inst.SetPktData(piece);
        _roomthemeslotlist.Add(inst);
    }

    public void SetPktInfoRoomFigureSlotAndClear(PktInfoRoomFigureSlot roomfigureslot)
    {
        _roomthemefigureslotlist.Clear();

        SetPktInfoRoomFigureSlot(roomfigureslot);
    }
    public void SetPktInfoRoomFigureSlot(PktInfoRoomFigureSlot roomfigureslot)
    {
        for (int i = 0; i < roomfigureslot.infos_.Count; i++)
            _roomthemefigureslotlist.Add(new RoomThemeFigureSlotData());
        for (int i = 0; i < roomfigureslot.infos_.Count; i++)
            _roomthemefigureslotlist[i].SetPktData(roomfigureslot.infos_[i]);
    }

    public void SetPktInfoRoomThemeSlotDetail(PktInfoRoomThemeSlotDetail roomslotdetail)
    {
        var themeSlot = roomslotdetail.themeSlot_;
        int themeStloNum = (int)themeSlot.slotNum_;
        // 기존 슬롯 정리
        for (int i = 0; i < _roomthemeslotlist.Count; i++)
        {
            if (_roomthemeslotlist[i].SlotNum == themeStloNum)
            {
                _roomthemeslotlist.RemoveAt(i);
                break;
            }
        }
        for (int i = _roomthemefigureslotlist.Count - 1; 0 <= i; --i)
        {
            if (_roomthemefigureslotlist[i].RoomThemeSlotNum == themeStloNum)
                _roomthemefigureslotlist.RemoveAt(i);
        }

        _roomthemefiguredetailinfo = true;
        // 신규 슬롯 정보 설정
        SetPktInfoRoomThemeSlotOne(themeSlot);
        SetPktInfoRoomFigureSlot(roomslotdetail.figureSlot_);
    }
    public void SetPktUserDataChange(PktInfoUserDataChange userDataChange)
    {
        // 유저 변경된 내용중 추가된 내용 적용
        ApplyProduct(userDataChange.products_, false);
        ApplyPktInfoAchieve(userDataChange.achieves_);
        ApplyPktMission(userDataChange.mission_);
        ApplyPktAchieveEvents(userDataChange.achieveEvent_);
    }

    public void SetPktGllaMissionData(PktInfoMission.Guerrilla guerrilla)
    {
        GllaMissionList.Clear();

        if (null == guerrilla || null == guerrilla.infos_)
        {
            Log.Show("PktInfoMission.Guerrilla 데이터가 없습니다.", Log.ColorType.Red);
            return;
        }

        for (int i = 0; i < guerrilla.infos_.Count; i++)
        {
			Debug.Log("GllaMissionList Group Id : " + guerrilla.infos_[i].groupID_);
			GllaMissionData guerrillaMissionData = new GllaMissionData();
            guerrillaMissionData.SetPktData(guerrilla.infos_[i]);
            GllaMissionList.Add(guerrillaMissionData);
        }

        //Send_ReqUpdateGllaMission();
        Log.Show("SetPktGllaMissionData : " + GllaMissionList.Count, Log.ColorType.Blue);
    }

    public void SetPktArenaData(PktInfoUserArena arena)
    {
        //초기화
        _teamcharlist.Clear();

        for (int i = (int)eArenaTeamSlotPos.START_POS; i < (int)eArenaTeamSlotPos._MAX_; i++)
        {
            _teamcharlist.Add((long)arena.team_.CUIDs_[i]);
        }

        _userdata.ArenaCardFormationID = (int)arena.team_.cardFrmtID_;
        _userbattledata.SetPktData(arena.record_);
    }

    public void SetPktBadgeList(PktInfoBadge badgeInfo_)
    {
        _badgelist.Clear();
        for (int i = 0; i < badgeInfo_.infos_.Count; i++)
            _badgelist.Add(new BadgeData());

        for (int i = 0; i < badgeInfo_.infos_.Count; i++)
            _badgelist[i].SetPktData(badgeInfo_.infos_[i]);
    }

    public void SetPktPassSetData(PktInfoPass pkt)
    {
        foreach (PktInfoPass.Piece info in pkt.infos_)
        {
            PassSetData data = _userdata.PassSetDataList.Find(x => x.PassID == info.tid_);
            if (data != null)
            {
                data.SetPassSetData(info);
            }
            else
            {
                PassSetData passSetData = new PassSetData();
                passSetData.SetPassSetData(info);
                _userdata.PassSetDataList.Add(passSetData);
            }
        }
    }

    public void SetPktPassMissionData(PktInfoMission.Pass passMissionDats)
    {
        if (passMissionDats == null)
        {
            return;
        }

        PassSetData data = UserData.PassSetDataList.Find(x => x.PassID == (int)ePassSystemType.Gold);
        if (data == null)
        {
            return;
        }
        
        _passMissionData.Clear();
        List<GameTable.PassMission.Param> passMissionList = GameTable.FindAllPassMission(x => x.PassID == data.PassID);
        foreach (GameTable.PassMission.Param param in passMissionList)
        {
            PassMissionData pdata = new PassMissionData
            {
                PassID = param.PassID,
                PassMissionTableData = param,
                PassMissionID = param.ID,
            };
            _passMissionData.Add(pdata);
        }

        foreach (PktInfoMission.Pass.Piece info in passMissionDats.infos_)
        {
            PassMissionData pdata = _passMissionData.Find(x => x.PassMissionID == info.missionID_);
            if (pdata != null)
            {
                pdata.PassMissionState = (int)info.state_;
                pdata.PassMissionValue = (int)info.value_;
            }
        }
    }

    public void SetPktInfluenceData(PktInfoMission.Influ influ)
    {
        InfluenceMissionData.SetPktData(influ);
    }

    public void ApplyPktMissionDailyData(PktInfoMission.Daily daily)
    {
        DailyMissionData.ApplyPktData(daily);
    }

    public void UpdatePktPassMissionData(PktInfoMission.Pass passMissionData)
    {
        if (_passMissionData == null || _passMissionData.Count <= 0)
        {
            SetPktPassMissionData(passMissionData);
            return;
        }

        for (int i = 0; i < passMissionData.infos_.Count; i++)
        {
            PassMissionData pdata = _passMissionData.Find(x => x.PassMissionID == passMissionData.infos_[i].missionID_);
            if (pdata != null)
            {
                pdata.PassMissionState = (int)passMissionData.infos_[i].state_;
                pdata.PassMissionValue = (int)passMissionData.infos_[i].value_;
            }
        }
    }

    public void EndPktPassMissions(PktInfoTIDList endMissionList)
    {
        if (endMissionList == null || endMissionList.tids_ == null || endMissionList.tids_.Count <= 0)
            return;

        for (int i = 0; i < endMissionList.tids_.Count; i++)
        {
            PassMissionData pm = _passMissionData.Find(x => x.PassMissionID == endMissionList.tids_[i]);
            if (pm != null)
                _passMissionData.Remove(pm);
        }
    }

    public void SetPktInfoWpnDepotSet(PktInfoWpnDepotSet pktInfoWpnDepotSet)
    {
        _weaponArmoryData.SetWeaponArmoryData(pktInfoWpnDepotSet);
    }

    public void SetPktLobbyTheme(PktInfoLobbyTheme pktLobbyTheme)
    {
        List<GameTable.LobbyTheme.Param> lobbyThemeList = GameInfo.Instance.GameTable.LobbyThemes;
        if (lobbyThemeList == null)
            return;

        if (_UserLobbyThemeList == null)
        {
            _UserLobbyThemeList = new List<int>();
            _UserLobbyThemeList.Clear();
        }

        for (int i = 0; i < lobbyThemeList.Count; i++)
        {
            if (lobbyThemeList[i].ID - 1 >= 64)
            {
                if (GameSupport._IsOnBitIdx((uint)pktLobbyTheme.flags_[(int)PktInfoLobbyTheme.FLAG._1], lobbyThemeList[i].ID - 1))
                    _UserLobbyThemeList.Add(lobbyThemeList[i].ID);
            }
            else
            {
                if (GameSupport._IsOnBitIdx((uint)pktLobbyTheme.flags_[(int)PktInfoLobbyTheme.FLAG._0], lobbyThemeList[i].ID - 1))
                    _UserLobbyThemeList.Add(lobbyThemeList[i].ID);
            }
        }
    }

    public void SetCardFormationFavor(PktInfoCardFormaFavi pktInfoCardFormaFavi)
    {
        if (_CardFormationFavorList == null)
            _CardFormationFavorList = new List<int>();

        _CardFormationFavorList.Clear();

        List<GameTable.CardFormation.Param> formationList = GameInfo.Instance.GameTable.CardFormations;
        int size    = formationList.Count();
        for (int i = 0; i < size; ++i)
        {
            var data = formationList[i];
            if (true == pktInfoCardFormaFavi.IsOnFlag(data.ID - 1))
                _CardFormationFavorList.Add(data.ID);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  net pkt add
    //
    //----------------------------------------------------------------------------------------------------------------------------------------------------
    public void AddPktCharList(PktInfoChar charInfo_)
    {
        var data = new CharData();
        data.SetPktData(charInfo_);
        _charlist.Add(data);
        FSaveData.Instance.SetInt("NCharBook_" + data.TableID.ToString(), 1);
        PlayerPrefs.SetInt("NCharBook_Favor_" + data.TableID.ToString(), 1);
    }

    public void ApplyItemWithStoreID(int storeId)
    {
        GameTable.Store.Param storeData = GameInfo.Instance.GameTable.FindStore(x => x.ID == storeId);
        if (storeData == null)
        {
            return;
        }

        //즉각 반응형 아이템
        if (storeData.ProductType == (int)eREWARDTYPE.PASS)
        {
            PassSetData data = UserData.PassSetDataList.Find(x => x.PassTableData.PassStoreID == storeId);
            if (data != null)
            {
                data.PassBuyEndTime = GameSupport.GetTimeWithString(data.PassTableData.EndTime, true);
            }
        }
    }

    public void ApplyPktSaleStore(PktInfoStoreSale storesale_)
    {
        for (int i = 0; i < storesale_.infos_.Count; i++)
        {
            var data = GetStoreData((int)storesale_.infos_[i].tableID_);
            if (data == null)
            {
                var newdata = new StoreData();
                newdata.SetPktData(storesale_.infos_[i]);
                _storelist.Add(newdata);
            }
            else
            {
                data.SetPktData(storesale_.infos_[i]);
            }
        }
    }

    public void AddPktCharSkillList(PktInfoCharSkill skillInfo_)
    {
        for (int i = 0; i < skillInfo_.infos_.Count; i++)
        {
            var skill = skillInfo_.infos_[i];
            var tabledata = GameInfo.Instance.GameTable.FindCharacterSkillPassive((int)skill.tableID_);
            if (tabledata != null)
            {
                var chardata = _charlist.Find(x => x.TableID == tabledata.CharacterID);
                if (chardata != null)
                {
                    chardata.PassvieList.Add(new PassiveData((int)skill.tableID_, (int)skill.lv_));
                }
            }
        }
    }

    public void AddPktCostumeList(PktInfoCostume costumeInfo_)
    {
        for (int i = 0; i < costumeInfo_.infos_.Count; i++)
        {
            var info = costumeInfo_.infos_[i];
            
            _costumelist.Add((int)info.tableID_);
            _dyeingDataList.Add(new MyDyeingData(info));

            FSaveData.Instance.SetInt("NCostumeBook_" + info.tableID_.ToString(), 1);
        }
    }
    public void AddPktCardList(PktInfoCard cardInfo_)
    {
        for (int i = 0; i < cardInfo_.infos_.Count; i++)
        {
            var data = new CardData();
            data.SetPktData(cardInfo_.infos_[i]);
            _cardlist.Add(data);
            AddNewCard(data.CardUID);
        }

        ApplySetCardPos();
        SaveNewCard();
    }
    public void AddPktWeaponList(PktInfoWeapon weaponInfo_)
    {
        for (int i = 0; i < weaponInfo_.infos_.Count; i++)
        {
            var data = new WeaponData();
            data.SetPktData(weaponInfo_.infos_[i]);
            _weaponlist.Add(data);
            AddNewWeapon(data.WeaponUID);
        }
        SaveNewWeapon();
    }
    public void AddPktItemList(PktInfoItem itemInfo_)
    {
        for (int i = 0; i < itemInfo_.infos_.Count; i++)
        {
            var findata = _itemlist.Find(x => x.ItemUID == (long)itemInfo_.infos_[i].itemUID_);
            if (findata != null)
            {
                findata.Count = (int)itemInfo_.infos_[i].cnt_;
            }
            else
            {
                var data = new ItemData();
                data.SetPktData(itemInfo_.infos_[i]);
                _itemlist.Add(data);
                AddNewItem(data.ItemUID);
            }
        }
        SaveNewItem();
    }
    public void AddPktGemList(PktInfoGem gemInfo_)
    {
        for (int i = 0; i < gemInfo_.infos_.Count; i++)
        {
            var data = new GemData();
            data.SetPktData(gemInfo_.infos_[i]);
            _gemlist.Add(data);
            AddNewGem(data.GemUID);
        }
        SaveNewGem();
    }
    public void AddPktStageClearList(PktInfoStageClear stageClearInfo_)
    {
        for (int i = 0; i < stageClearInfo_.infos_.Count; i++)
        {
            var data = new StageClearData();
            data.SetPktData(stageClearInfo_.infos_[i]);
            _stageclearlist.Add(data);
        }
    }

    public void AddPktWeaponBookList(PktInfoWeaponBook weaponBookInfo_)
    {
        for (int i = 0; i < weaponBookInfo_.infos_.Count; i++)
        {
            var data = new WeaponBookData();
            data.SetPktData(weaponBookInfo_.infos_[i]);
            _weaponbooklist.Add(data);
        }
    }
    public void UpdatePktWeaponBookList(PktInfoWeaponBook weaponBookInfo_)
    {
        for (int i = 0; i < weaponBookInfo_.infos_.Count; i++)
        {
            var weaponbookdata = GetWeaponBookData((int)weaponBookInfo_.infos_[i].tableID_);
            if (weaponbookdata != null)
            {
                weaponbookdata.SetPktData(weaponBookInfo_.infos_[i]);
            }
        }
    }
    public void AddPktCardBookList(PktInfoCardBook cardBookInfo_)
    {
        for (int i = 0; i < cardBookInfo_.infos_.Count; i++)
        {
            var data = new CardBookData();
            data.SetPktData(cardBookInfo_.infos_[i]);
            _cardbooklist.Add(data);
            PlayerPrefs.SetInt("NCardBook_Favor_" + data.TableID.ToString(), 1);
        }
    }
    public void UpdatePktCardBookList(PktInfoCardBook cardBookInfo_)
    {
        for (int i = 0; i < cardBookInfo_.infos_.Count; i++)
        {
            var cardbookdata = GetCardBookData((int)cardBookInfo_.infos_[i].tableID_);
            if (cardbookdata != null)
            {
                cardbookdata.SetPktData(cardBookInfo_.infos_[i]);
            }
        }
    }
    public void AddPktMonsterBookList(PktInfoMonsterBook monsterBookInfo_)
    {
        for (int i = 0; i < monsterBookInfo_.infos_.Count; i++)
        {
            var data = new MonsterBookData();
            data.SetPktData(monsterBookInfo_.infos_[i]);
            _monsterbooklist.Add(data);
        }
    }
    public void AddPktUserMarkList(PktInfoTIDList info_)
    {
        for (int i = 0; i < info_.tids_.Count; i++)
            _usermarklist.Add((int)info_.tids_[i]);
    }

    public void AddUserLobbyThemeList(PktInfoTIDList info_)
    {
        for (int i = 0; i < info_.tids_.Count; i++)
            _UserLobbyThemeList.Add((int)info_.tids_[i]);
    }

    public void AddPktTimeAttackClearList(PktInfoTimeAtkStageRec timeatkInfo_)
    {
        for (int i = 0; i < timeatkInfo_.infos_.Count; i++)
        {
            var data = new TimeAttackClearData();
            data.SetPktData(timeatkInfo_.infos_[i]);
            _timeattackclearlist.Add(data);
        }
    }

    public void AddPktMissionGuerrilla(PktInfoMission.Guerrilla _pktInfo)
    {
        for (int i = 0; i < _pktInfo.infos_.Count; i++)
        {
            GllaMissionData userMissionData = GllaMissionList.Find(x => x.GroupID == _pktInfo.infos_[i].groupID_);
            if (userMissionData != null)
            {
                userMissionData.SetPktData(_pktInfo.infos_[i]);
            }
            else
            {
                GllaMissionData guerrillaMissionData = new GllaMissionData();
                guerrillaMissionData.SetPktData(_pktInfo.infos_[i]);
                GllaMissionList.Add(guerrillaMissionData);
            }

        }


    }
    public void AddPktBadgeList(PktInfoBadge badgeInfo_)
    {
        for (int i = 0; i < badgeInfo_.infos_.Count; i++)
        {
            var data = new BadgeData();
            data.SetPktData(badgeInfo_.infos_[i]);
            _badgelist.Add(data);
            Log.Show("Badge UID : " + data.BadgeUID, Log.ColorType.Red);
            AddNewBadge(data.BadgeUID);
        }

        SaveNewBadge();
    }

    public void AddPktCharAniList(PktInfoUIDValue charAni_)
    {
        if (charAni_ == null)
            return;

        for (int i = 0; i < charAni_.infos_.Count; i++)
        {
            CharData chardata = GameInfo.Instance.CharList.Find(x => x.CUID == (long)charAni_.infos_[i].uid_);
            if (chardata != null)
            {
                chardata.CharAniFlag |= (int)charAni_.infos_[i].val_;
            }
            
        }
    }

    public void AddPktUnexpectedPackage(PktInfoUnexpectedPackage pktInfoUnexpectedPackage)
    {
        if (pktInfoUnexpectedPackage == null)
        {
            return;
        }

        bool bSort = false;
        foreach (PktInfoUnexpectedPackage.Piece info in pktInfoUnexpectedPackage.infos_)
        {
            bool bTempSort = AddPktUnexpectedPackagePiece(info);
            if (!bSort && bTempSort)
            {
                bSort = true;
            }
        }

        if (bSort)
        {
            // Sort
            foreach (KeyValuePair<int,List<UnexpectedPackageData>> pair in _unexpectedPackageDataDict)
            {
                pair.Value.Sort((x, y) => x.EndTime < y.EndTime ? -1 : 1);
            }
        }
    }

    public bool AddPktUnexpectedPackagePiece(PktInfoUnexpectedPackage.Piece piece)
    {
        GameTable.UnexpectedPackage.Param tableData = GameTable.FindUnexpectedPackage((int) piece.tableID_);
        if (tableData == null)
        {
            return false;
        }

        bool result = false;
        if (_unexpectedPackageDataDict.ContainsKey(tableData.UnexpectedType))
        {
            UnexpectedPackageData data = _unexpectedPackageDataDict[tableData.UnexpectedType].Find(x => x.TableId == piece.tableID_);
            if (data == null)
            {
                _unexpectedPackageDataDict[tableData.UnexpectedType].Add(new UnexpectedPackageData(piece, true));
            }
            else
            {
                data.UpdateData(piece);
                result = true;
            }
        }
        else
        {
            _unexpectedPackageDataDict.Add(tableData.UnexpectedType, new List<UnexpectedPackageData>(){ new UnexpectedPackageData(piece, true) });
        }

        return result;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  net pkt Apply
    //
    //----------------------------------------------------------------------------------------------------------------------------------------------------
    public void ApplyPktInfoMaterCard(PktInfoUIDList maters_)
    {
        for (int i = 0; i < maters_.uids_.Count; i++)
        {
            var data = GetCardData((long)maters_.uids_[i]);
            if (data != null)
            {
                _cardlist.Remove(data);
            }
        }
    }

    public void ApplyPktInfoMaterWeapon(PktInfoUIDList maters_)
    {
        for (int i = 0; i < maters_.uids_.Count; i++)
        {
            var data = GetWeaponData((long)maters_.uids_[i]);
            if (data != null)
            {
                _weaponlist.Remove(data);
            }
        }
    }

    public void ApplyPktInfoMaterGem(PktInfoUIDList maters_)
    {
        for (int i = 0; i < maters_.uids_.Count; i++)
        {
            var data = GetGemData((long)maters_.uids_[i]);
            if (data != null)
            {
                _gemlist.Remove(data);
            }
        }
    }

    public void ApplyPktInfoMaterItem(PktInfoConsumeItem maters_)
    {
        for (int i = 0; i < maters_.infos_.Count; i++)
        {
            var info = maters_.infos_[i];
            var itemdata = GetItemData((long)info.uid_);
            if (itemdata != null)
            {
                if (info.delFlag_ != 0)
                {
                    _itemlist.Remove(itemdata);
                }
                else
                {
                    itemdata.Count = (int)info.remainCnt_;
                }
            }

        }
    }

    public void ApplyPktInfoFacilityOperationAckMater(List<PktInfoFacilityOperationReq.Mater> maters_)
    {
        for (int i = 0; i < maters_.Count; i++)
        {
            if (maters_[i] == null) continue;
            switch (maters_[i].kind_)
            {
                case eContentsPosKind.CARD:
                    CardList.RemoveAll(x => x.CardUID == (long)maters_[i].uid_);
                    break;
                case eContentsPosKind.WEAPON:
                    WeaponList.RemoveAll(x => x.WeaponUID == (long)maters_[i].uid_);
                    break;
            }
        }
    }

    public void ApplyPktInfoMaterItemGoods(PktInfoConsumeItemAndGoods maters_)
    {
        if (maters_.items_ != null)
        {
            for (int i = 0; i < maters_.items_.infos_.Count; i++)
            {
                var info = maters_.items_.infos_[i];
                var itemdata = GetItemData((long)info.uid_);
                if (itemdata != null)
                {
                    if (info.delFlag_ != 0)
                    {
                        _itemlist.Remove(itemdata);
                    }
                    else
                    {
                        itemdata.Count = (int)info.remainCnt_;
                    }
                }
            }
        }

        _userdata.SetPktData(maters_.goods_);
    }

    public void ApplyPktInfoAchieve(PktInfoAchieve achieve_)
    {
        for (int i = 0; i < achieve_.infos_.Count; i++)
        {
            var data = GetAchieveData((int)achieve_.infos_[i].groupID_);
            if (data != null)
            {
                data.GroupOrder = (int)achieve_.infos_[i].nowStep_;
                if (data.Value < (int)achieve_.infos_[i].condiVal_)
                    data.Value = (int)achieve_.infos_[i].condiVal_;
                data.SetTable();
            }
            else
            {
                var adddata = new AchieveData();
                adddata.SetPktData(achieve_.infos_[i]);
                _achievelist.Add(adddata);
            }
        }
    }

    public void ApplyPktAchieveEvents(PktInfoAchieveEvent pktInfoAchieve)
    {
        foreach (PktInfoAchieveEvent.Piece info in pktInfoAchieve.infos_)
        {
            AchieveEventData achieveEventData = GetAchieveEventData((int)info.groupID_, (int)info.achieveGroup_);
            if (achieveEventData == null)
            {
                _achieveEventList.Add(new AchieveEventData(info));
            }
            else
            {
                achieveEventData.SetInfo(info);
            }
        }
    }

    public void ApplyPktMission(PktInfoMission mission_)
    {   
        if (mission_.influ_.groupID_ > 0)
            SetPktInfluenceData(mission_.influ_);
        
        ApplyPktMissionDailyData(mission_.daily_);        
    }

    public void ApplySetCardPos()
    {
        for (int i = 0; i < _charlist.Count; i++)
            for (int j = 0; j < (int)eCOUNT.CARDSLOT; j++)
                _charlist[i].EquipCard[j] = (int)eCOUNT.NONE;

        for (int i = 0; i < _facilitylist.Count; i++)
            _facilitylist[i].EquipCardUID = (int)eCOUNT.NONE;

        for (int i = 0; i < _cardlist.Count; i++)
        {
            if (_cardlist[i].PosKind == (int)eContentsPosKind.CHAR)
            {
                var chardata = GetCharData(_cardlist[i].PosValue);
                if (chardata != null)
                {
                    chardata.EquipCard[_cardlist[i].PosSlot] = _cardlist[i].CardUID;
                }
            }
            else if (_cardlist[i].PosKind == (int)eContentsPosKind.FACILITY)
            {
                var facilitydata = GetFacilityData((int)_cardlist[i].PosValue);
                if (facilitydata != null)
                {
                    facilitydata.EquipCardUID = _cardlist[i].CardUID;
                }
            }
        }
    }

    public void ApplyStageGameResult( PktInfoStageGameResultAck _pktInfo, bool isRaid )
    {
        AddPktMonsterBookList(_pktInfo.monsterBooks_);
        AddPktTimeAttackClearList(_pktInfo.timeRecord_);

        var chardata = GetCharData((long)_pktInfo.charUID_);

        GameResultData.Init();
        GameResultData.StageID = (int)_pktInfo.clearStageTID_;
        GameResultData.UserBeforeLevel = _userdata.Level;
        GameResultData.UserBeforeExp = _userdata.Exp;
        GameResultData.CharUID = (long)_pktInfo.charUID_;
        GameResultData.CharBeforeGrade = chardata.Grade;
        GameResultData.CharBeforeLevel = chardata.Level;
        GameResultData.CharBeforeExp = chardata.Exp;

        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
            GameResultData.CardUID[i] = chardata.EquipCard[i];

        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            var carddata = GameInfo.Instance.GetCardData(GameResultData.CardUID[i]);
            if (carddata != null)
            {
                var cardbookdata = GetCardBookData(carddata.TableID);
                if (cardbookdata != null)
                {
                    GameResultData.CardFavorBeforeLevel[i] = cardbookdata.FavorLevel;
                    GameResultData.CardFavorBeforeExp[i] = cardbookdata.FavorExp;
                }
            }
        }

        //  서버에서 받은 유저 갱신
        if (_pktInfo.isChangeUser_ == true)
        {
            _userdata.Level = (int)_pktInfo.userExpLv_.lv_;
            _userdata.Exp = (int)_pktInfo.userExpLv_.exp_;
        }

        //  서버에서 받은 캐릭터 갱신
        if (_pktInfo.isChangeChar_ == true)
        {
            chardata.Level = (int)_pktInfo.charExpLv_.lv_;
            chardata.Exp = (int)_pktInfo.charExpLv_.exp_;
            chardata.PassviePoint = (int)_pktInfo.charSkillPT_;
        }
        
        if (0 <= _pktInfo.charSecretCnt_)
        {
            chardata.SecretQuestCount = _pktInfo.charSecretCnt_;
        }

        // 비밀 임무 다중 캐릭터 입장 관련 처리
        CharData subChar = null;
        for (int i = 0; i < _pktInfo.subCharSecretCnt_.infos_.Count; i++) {
            subChar = GetCharData((long)_pktInfo.subCharSecretCnt_.infos_[i].uid_);
            subChar.SecretQuestCount = (int)_pktInfo.subCharSecretCnt_.infos_[i].val_;
        }

        UpdatePktCardBookList(_pktInfo.cardBook_);

        GameResultData.UserAfterLevel = _userdata.Level;
        GameResultData.UserAfterExp = _userdata.Exp;
        GameResultData.CharAfterLevel = chardata.Level;
        GameResultData.CharAfterExp = chardata.Exp;

        // for Raid
        if( isRaid ) {
            for( int i = 0; i < GameResultData.RaidCharExpInfoArr.Length; i++ ) {
                CharData raidCharData = GetCharData( (long)_pktInfo.raidCUIDs_[i] );

                GameResultData.RaidCharExpInfoArr[i].Uid = (long)_pktInfo.raidCUIDs_[i];
                GameResultData.RaidCharExpInfoArr[i].BeforeGrade = raidCharData.Grade;
                GameResultData.RaidCharExpInfoArr[i].BeforeLevel = raidCharData.Level;
                GameResultData.RaidCharExpInfoArr[i].BeforeExp = raidCharData.Exp;

                if( _pktInfo.isChangeRaidChar_[i] ) {
                    raidCharData.Level = (int)_pktInfo.raidCharExpLvs_[i].lv_;
                    raidCharData.Exp = (int)_pktInfo.raidCharExpLvs_[i].exp_;
                    raidCharData.PassviePoint = (int)_pktInfo.raidCharSkillPTs_[i];
                }

                GameResultData.RaidCharExpInfoArr[i].AfterLevel = raidCharData.Level;
                GameResultData.RaidCharExpInfoArr[i].AfterExp = raidCharData.Exp;
            }
        }

        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            var carddata = GameInfo.Instance.GetCardData(GameResultData.CardUID[i]);
            if (carddata != null)
            {
                var cardbookdata = GetCardBookData(carddata.TableID);
                if (cardbookdata != null)
                {
                    GameResultData.CardFavorAfterLevel[i] = cardbookdata.FavorLevel;
                    GameResultData.CardFavorAfterExp[i] = cardbookdata.FavorExp;
                }
            }
        }


        PktInfoStageClear.Piece stageclear = new PktInfoStageClear.Piece();
        stageclear.tableID_ = _pktInfo.clearStageTID_;
        stageclear.rewardFlag_ = _pktInfo.missionRewardFlag_;   //  미션 플래그 갱신

        var cleardata = _stageclearlist.Find(x => x.TableID == (int)stageclear.tableID_);
        if (cleardata == null)
        {
            StageClearData data = new StageClearData();
            data.SetPktData(stageclear);
            _stageclearlist.Add(data);

            //처음으로 클리어한 스테이지 값 임시저장 - 스페셜 구매 팝업 오픈 조건에서 사용
            UIValue.Instance.SetValue(UIValue.EParamType.FirstClearStageID, (int)stageclear.tableID_);
        }
        else
        {
            cleardata.SetPktData(stageclear);
        }

        if (_pktInfo.ticketType == eGOODSTYPE.AP)
            _userdata.APRemainTime = GameSupport.GetLocalTimeByServerTime(_pktInfo.ticketNextTime_.GetTime());
        else
            _userdata.BPRemainTime = GameSupport.GetLocalTimeByServerTime(_pktInfo.ticketNextTime_.GetTime());

        if (_pktInfo.info_.special_ != null)
        {
            var stagetabledata = GameInfo.Instance.GameTable.FindStage(x => x.ID == (int)stageclear.tableID_);
            if (stagetabledata != null)
            {
                if (stagetabledata.StageType == (int)eSTAGETYPE.STAGE_SPECIAL)
                {
                    _userdata.LastPlaySpecialModeTime = GameSupport.GetLocalTimeByServerTime(_pktInfo.info_.special_.nextTime_.GetTime());
                    _userdata.NextPlaySpecialModeTableID = (int)_pktInfo.info_.special_.tableID_;
                }
            }
        }

        if (_pktInfo.info_.arenaProc_ > 0)
            _userdata.ArenaPrologueValue = (int)_pktInfo.info_.arenaProc_;
        
        ApplyPktInfoMaterItem(_pktInfo.useItem_);
    }

    public void ApplyRaidFailResult( PktInfoStageGameEndFail pkt  ) {
		GameTable.Stage.Param stageParam = GameTable.FindStage( x => x.ID == pkt.stageTID_ );
		if ( stageParam != null ) {
			if ( stageParam.StageType != (int)eSTAGETYPE.STAGE_RAID ) {
				return;
			}
		}

		RewardList.Clear();

		StageClearID = (int)pkt.stageTID_;

        for ( int i = 0; i < pkt.raidCUIDs_.Length; i++ ) {
            long cuid = (long)pkt.raidCUIDs_[i];

            CharData charData = GetCharData( cuid );
			if ( charData == null ) {
				continue;
			}

            if ( i == 0 ) {
                GameResultData.Init();
                GameResultData.StageID = StageClearID;
                GameResultData.UserBeforeLevel = _userdata.Level;
                GameResultData.UserBeforeExp = _userdata.Exp;
                GameResultData.CharUID = cuid;
                GameResultData.CharBeforeGrade = charData.Grade;
                GameResultData.CharBeforeLevel = charData.Level;
                GameResultData.CharBeforeExp = charData.Exp;

                GameResultData.UserAfterLevel = _userdata.Level;
                GameResultData.UserAfterExp = _userdata.Exp;
                GameResultData.CharAfterLevel = charData.Level;
                GameResultData.CharAfterExp = charData.Exp;

                for ( int j = 0; j < (int)eCOUNT.CARDSLOT; j++ ) {
                    GameResultData.CardUID[j] = charData.EquipCard[j];
                }

                for ( int j = 0; j < (int)eCOUNT.CARDSLOT; j++ ) {
                    CardData cardData = GameInfo.Instance.GetCardData( GameResultData.CardUID[j] );
                    if ( cardData != null ) {
                        CardBookData cardBookData = GetCardBookData( cardData.TableID );
                        if ( cardBookData != null ) {
                            GameResultData.CardFavorBeforeLevel[i] = cardBookData.FavorLevel;
                            GameResultData.CardFavorBeforeExp[i] = cardBookData.FavorExp;

                            GameResultData.CardFavorAfterLevel[i] = cardBookData.FavorLevel;
                            GameResultData.CardFavorAfterExp[i] = cardBookData.FavorExp;
                        }
                    }
                }
            }

            GameResultData.RaidCharExpInfoArr[i].Uid = cuid;
            GameResultData.RaidCharExpInfoArr[i].BeforeGrade = charData.Grade;
            GameResultData.RaidCharExpInfoArr[i].BeforeLevel = charData.Level;
            GameResultData.RaidCharExpInfoArr[i].BeforeExp = charData.Exp;

            GameResultData.RaidCharExpInfoArr[i].AfterLevel = charData.Level;
            GameResultData.RaidCharExpInfoArr[i].AfterExp = charData.Exp;

            charData.RaidHpPercentage = (float)pkt.raidCharHPs_[i] / 100.0f;
		}

        long raidPoint = (long)pkt.resultRaidPoint_;

        long rewardPoint = raidPoint - _userdata.Goods[(int)eGOODSTYPE.RAIDPOINT];
        if ( rewardPoint < 0 ) {
            rewardPoint = 0;
		}

        GameResultData.Goods[(int)eGOODSTYPE.RAIDPOINT] = rewardPoint;
        _userdata.SetGoods( eGOODSTYPE.RAIDPOINT, raidPoint );

        if ( pkt.dailyLimitPoint_ > 0 ) {
            RaidUserData.DailyRaidPoint = pkt.dailyLimitPoint_;
        }
    }

    public void ApplyProduct(PktInfoProductPack _pktProduct, bool isGameResult, bool stackView = true, bool isRaid = false )
    {
        if (isGameResult)
        {
            RewardList.Clear();

            for (int i = 0; i < _pktProduct.goodsInfos_.Count; i++)
            {
                GameResultData.Goods[(int)_pktProduct.goodsInfos_[i].type_] = (long)_pktProduct.goodsInfos_[i].value_;
            }

            for (int i = 0; i < _pktProduct.lotteryInfos_.Count; i++)
            {
                PktInfoProductPack.Lottery lottery = _pktProduct.lotteryInfos_[i];
                
                bool changeGrade = false;
                bool monthGrade = false;
                bool favorGrade = false;

                switch ((PktInfoProductPack.Lottery.TYPE)lottery.dropTP_)
                {
                    case PktInfoProductPack.Lottery.TYPE.SPT_DROP: // 서포터 드롭 일반
                        changeGrade = true;
                        break;
                    case PktInfoProductPack.Lottery.TYPE.BUFF_DROP: // 버프 드롭 일반(헤비시카)
                        monthGrade = true;
                        break;
                    case PktInfoProductPack.Lottery.TYPE.BUFF_DROP_ACTIVE: // 버프 드롭 액티브(친밀도)
                        favorGrade = true;
                        break;
                }
                
                RewardData data = new RewardData((long)lottery.uid_, (int)lottery.rwdTP_, (int)lottery.idx_, lottery.value_, changeGrade, monthGrade, favorGrade);
                data.bNew = _pktProduct.IsNew(lottery);

                if ( data.Type == (int)eREWARDTYPE.GOODS && data.Index == (int)eGOODSTYPE.RAIDPOINT ) {
                    GameResultData.Goods[ data.Index ] -= data.Value;
                    if ( GameResultData.Goods[data.Index] < 0) {
						GameResultData.Goods[data.Index] = 0;
					}
				}

                RewardList.Add(data);
            }
        }
        else
        {
            SetRewardListPktProduct(_pktProduct, stackView);
        }

        for (int i = 0; i < _pktProduct.charInfos_.Count; i++)
        {
            var charinfo_ = _pktProduct.charInfos_[i];
            AddPktCharList(charinfo_.char_);
            AddPktCharSkillList(charinfo_.charSkills_);
            AddPktCostumeList(charinfo_.costumes_);
            AddPktWeaponList(charinfo_.weapons_);
            AddPktWeaponBookList(charinfo_.weaponBooks_);
        }

        AddPktCostumeList(_pktProduct.costumes_);
        AddPktCardList(_pktProduct.cards_);
        AddPktCardBookList(_pktProduct.cardBooks_);
        AddPktItemList(_pktProduct.items_);
        AddPktGemList(_pktProduct.gems_);
        AddPktWeaponList(_pktProduct.weapons_);
        AddPktWeaponBookList(_pktProduct.weaponBooks_);
        AddPktUserMarkList(_pktProduct.marks_.tids_);
        AddPktBadgeList(_pktProduct.badges_);
        UpdatePktInfoMonthlyFee(_pktProduct.monFees_);
        UpdatePktInfoBuffEffect(_pktProduct.effs_);
        AddPktCharAniList(_pktProduct.charAni_);
        SetPktLobbyTheme(_pktProduct.lobbyTheme_.info_);
        //MailList.Clear();
        //SetPktMailCount(MailTotalCount + _pktProduct.mailes_.infos_.Count);
        for (int i = 0; i < _pktProduct.mailes_.infos_.Count; i++)
        {
            MailData mailData = new MailData();
            mailData.SetPktData(_pktProduct.mailes_.infos_[i]);

            MailList.Add(mailData);
            MailTotalCount += 1;
            //MailCnt Add
        }

        SetPktPassSetData(_pktProduct.pass_);

        AddPktUnexpectedPackage(_pktProduct.unexpectedPackage_);

        _userdata.SetPktData(_pktProduct.retGoods_);
        _userdata.SetPktData(_pktProduct.chatStamp_.info_);

        if (!isGameResult && stackView)
        {
            for (int i = 0; i < _pktProduct.items_.infos_.Count; i++)
            {
                var data = RewardList.Find(x => x.UID == (long)_pktProduct.items_.infos_[i].itemUID_ && x.Type == (int)eREWARDTYPE.ITEM && x.Index == (int)_pktProduct.items_.infos_[i].tableID_);
                if (data != null)
                    data.Value = (int)_pktProduct.items_.infos_[i].cnt_ - data.Value;
            }
        }
    }

    public void ApplyPktInfoTimeAtkRankStageList(PktInfoTimeAtkRankStageList _pktInfo)
    {
        for (int i = 0; i < _pktInfo.infos_.Count; i++)
        {
            PktInfoTimeAtkRankStage stagedata = _pktInfo.infos_[i];

            TimeAttackRankData rankdata = GetTimeAttackRankData((int)stagedata.header_.stageID_);
            if (rankdata == null)
            {
                rankdata = new TimeAttackRankData((int)stagedata.header_.stageID_);
                _timeattackranklist.Add(rankdata);
            }
            rankdata.SetPktData(stagedata.header_);
            rankdata.RankUserList.Clear();

            for (int j = 0; j < stagedata.infos_.Count; j++)
            {
                PktInfoTimeAtkRankStage.Piece data = stagedata.infos_[j];

                if (stagedata.infos_[j].uuid_ != (System.UInt64)eCOUNT.NONE)
                {
                    var rankuserdata = new TimeAttackRankUserData();
                    rankuserdata.SetPktData(data, j + 1);
                    rankdata.RankUserList.Add(rankuserdata);
                }

            }
        }
    }

    public void ApplyPktInfoTimeAtkRankerDetailAck(PktInfoTimeAtkRankerDetailAck _pktInfo)
    {
        TimeAttackRankUserData rankuserdata = GetTimeAttackRankUserData((int)_pktInfo.stageID_, (long)_pktInfo.simpleInfo_.uuid_);
        if (rankuserdata == null)
            return;
        rankuserdata.SetPktData(_pktInfo);
    }

    public void ApplyPktInfoUserBingoEvent(PktInfoUserBingoEvent.Piece pktInfo)
    {
        if (_eventDataDict.ContainsKey((int)eLobbyEventType.Bingo) == false)
        {
            _eventDataDict.Add((int)eLobbyEventType.Bingo, new List<EventData>());
        }

        EventData eventData = _eventDataDict[(int)eLobbyEventType.Bingo].Find(x => x.GroupID == pktInfo.GroupID);
        if (eventData == null)
        {
            eventData = new EventData(pktInfo);
            _eventDataDict[(int)eLobbyEventType.Bingo].Add(eventData);
        }
        else
        {
            eventData.Copy(pktInfo);
        }
    }

    public void SetRewardListPktProduct(PktInfoProductPack _pktProduct, bool stackView = false)
    {
        RewardList.Clear();

        for (int i = 0; i < _pktProduct.goodsInfos_.Count; i++)
        {
            if ((int)_pktProduct.goodsInfos_[i].value_ <= (int)eCOUNT.NONE)
                continue;

            if (stackView)
            {
                RewardData data = RewardList.Find(x => x.Type == (int)eREWARDTYPE.GOODS && x.Index == (int)_pktProduct.goodsInfos_[i].type_);
                if (data == null)
                {
                    data = new RewardData((int)eREWARDTYPE.GOODS, (int)_pktProduct.goodsInfos_[i].type_, (int)_pktProduct.goodsInfos_[i].value_);
                    RewardList.Add(data);
                }
                else
                {
                    data.Value += (int)_pktProduct.goodsInfos_[i].value_;
                }
            }
            else
            {
                RewardData data = new RewardData((int)eREWARDTYPE.GOODS, (int)_pktProduct.goodsInfos_[i].type_, (int)_pktProduct.goodsInfos_[i].value_);
                RewardList.Add(data);
            }
        }

        for (int i = 0; i < _pktProduct.charInfos_.Count; i++)
        {
            RewardData data = new RewardData((long)_pktProduct.charInfos_[i].char_.cuid_, (int)eREWARDTYPE.CHAR, (int)_pktProduct.charInfos_[i].char_.tableID_, 1, false);
            RewardList.Add(data);
        }

        for (int i = 0; i < _pktProduct.costumes_.infos_.Count; i++)
        {
            RewardData data = new RewardData((int)eREWARDTYPE.COSTUME, (int)_pktProduct.costumes_.infos_[i].tableID_, 1);
            RewardList.Add(data);
        }

        for (int i = 0; i < _pktProduct.cards_.infos_.Count; i++)
        {
            RewardData data = new RewardData((long)_pktProduct.cards_.infos_[i].cardUID_, (int)eREWARDTYPE.CARD, (int)_pktProduct.cards_.infos_[i].tableID_, 1, false);
            RewardList.Add(data);
        }

        for (int i = 0; i < _pktProduct.weapons_.infos_.Count; i++)
        {
            RewardData data = new RewardData((long)_pktProduct.weapons_.infos_[i].weaponUID_, (int)eREWARDTYPE.WEAPON, (int)_pktProduct.weapons_.infos_[i].tableID_, 1, false);
            RewardList.Add(data);
        }

        if (stackView)
        {
            for (int i = 0; i < _pktProduct.items_.infos_.Count; i++)
            {
                int count = 0;
                var itemdata = GetItemData((long)_pktProduct.items_.infos_[i].itemUID_);
                if (itemdata != null)
                    count = itemdata.Count;

                RewardData data = new RewardData((long)_pktProduct.items_.infos_[i].itemUID_, (int)eREWARDTYPE.ITEM, (int)_pktProduct.items_.infos_[i].tableID_, count, false);
                RewardList.Add(data);
            }
        }
        else
        {
            for (int i = 0; i < _pktProduct.addItemInfos_.infos_.Count; i++)
            {
                long itemUID = 0;
                ItemData itemdata = GetItemData((int)_pktProduct.addItemInfos_.infos_[i].tid_);
                if (itemdata != null)
                {
                    itemUID = itemdata.ItemUID;
                }
                else
                {
                    for (int j = 0; j < _pktProduct.items_.infos_.Count; j++)
                    {
                        if (_pktProduct.items_.infos_[j].tableID_.Equals(_pktProduct.addItemInfos_.infos_[i].tid_))
                        {
                            itemUID = (long)_pktProduct.items_.infos_[j].itemUID_;
                            break;
                        }
                    }
                }

                RewardData data = new RewardData(itemUID, (int)eREWARDTYPE.ITEM, (int)_pktProduct.addItemInfos_.infos_[i].tid_, (int)_pktProduct.addItemInfos_.infos_[i].addCnt_, false);
                RewardList.Add(data);
            }
        }

        for (int i = 0; i < _pktProduct.gems_.infos_.Count; i++)
        {
            RewardData data = new RewardData((long)_pktProduct.gems_.infos_[i].gemUID_, (int)eREWARDTYPE.GEM, (int)_pktProduct.gems_.infos_[i].tableID_, 1, false);
            RewardList.Add(data);
        }

        for (int i = 0; i < _pktProduct.marks_.tids_.tids_.Count; i++)
        {
            RewardData data = new RewardData((int)eREWARDTYPE.USERMARK, (int)_pktProduct.marks_.tids_.tids_[i], 1);
            RewardList.Add(data);
        }

        for (int i = 0; i < _pktProduct.badges_.infos_.Count; i++)
        {
            RewardData data = new RewardData((long)_pktProduct.badges_.infos_[i].badgeUID_, (int)eREWARDTYPE.BADGE, (int)_pktProduct.badges_.infos_[i].opt_[0].optID_, 1, false);
            RewardList.Add(data);
        }

        for (int i = 0; i < _pktProduct.lobbyTheme_.tids_.tids_.Count; i++)
        {
            RewardData data = new RewardData((int)eREWARDTYPE.LOBBYTHEME, (int)_pktProduct.lobbyTheme_.tids_.tids_[i], 1);
            RewardList.Add(data);
        }
    }
    //----------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  아레나 통신
    //
    //----------------------------------------------------------------------------------------------------------------------------------------------------

    public void SetPktInfoUserArenaTeam(PktInfoUserArenaTeam _pktInfo)
    {
        _teamcharlist.Clear();
        /*for (int i = 0; i < _pktInfo.CUIDs_.Length; i++)
        {
            _teamcharlist.Add((long)_pktInfo.CUIDs_[i]);
        }*/
        for (int i = (int)eArenaTeamSlotPos.START_POS; i < (int)eArenaTeamSlotPos._MAX_; i++)
        {
            _teamcharlist.Add((long)_pktInfo.CUIDs_[i]);
        }

        _userdata.ArenaCardFormationID = (int)_pktInfo.cardFrmtID_;
    }


    //----------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //  통신관련 테스트 코드
    //
    //----------------------------------------------------------------------------------------------------------------------------------------------------
    public void SvrConnect_Login(bool bautologin, OnReceiveCallBack ReceiveCallBack)
    {
        _bautologin = bautologin;
        _netconnetinfo.ipAddr_ = AppMgr.Instance.GetServerAddr();
        _netconnetinfo.port_ = (UInt16)AppMgr.Instance.GetServerPort();

        Debug.Log(AppMgr.Instance.configData.m_ServerSelectType + " / " + _netconnetinfo.ipAddr_ + ":" + _netconnetinfo.port_);
        Debug.Log("_netFlag : " + _netflag);
        SetCallBackList("SvrConnect_Login", ReceiveCallBack);
        if (_netflag)
        {
            StartTimeOutConnect();
            NETStatic.Mgr.DoAllInOneConnect(eServerType.LOGIN, ref _netconnetinfo, true);
        }
        else
        {
            Send_ReqLogin(GetCallBackList("SvrConnect_Login"));
        }
    }

    private bool ReSvrConnect()
    {
        Debug.Log("ReSvrConnect");
        if (_bconnect == true)
        {
            Debug.Log("ReSvrConnect return _bconnect");
            return false;
        }

        Debug.Log("ReSvrConnect DoAllInOneConnect");

        //if( !NETStatic.Mgr.DoAllInOneConnect(_servertype, true) )
        if (!NETStatic.Mgr.DoAllInOneReConnect(_servertype, true))
        {
            Debug.Log("DoAllInOneConnect failure");
            return false;
        }

        WaitPopup.Show();
        _reconnect = true;
        StartTimeOutConnect();

        /*
        if (_servertype == eServerType.LOGIN)
        {
            Debug.Log("ReSvrConnect eServerType.LOGIN");
        }
        else
        {
            WaitPopup.Show();
            _reconnect = true;            

            Debug.Log("ReSvrConnect DoAllInOneConnect");
            
            NETStatic.Mgr.DoAllInOneConnect(_servertype, true);
        }
        */

        InitNetworkTime("time.google.com", true);
        return true;
    }

    public void OnNetReLogin(int result, PktMsgType pktmsg)
    {
        WaitPopup.Hide();

        SendProtocolData(true);
    }

    private void OnMsg_ShowResult()
    {
        World.Instance.ShowResultUI();
    }


    private bool CheckConnect()
    {
        Debug.Log("CheckConnect");

        if (!_netflag)
        {
            Debug.Log("CheckConnect return _netflag");
            return true;
        }

        /*
        if (NETStatic.Mgr.IsConnectingAboutActiveSvr())
        {
            Debug.Log("CheckConnect return IsConnectingAboutActiveSvr");
            return true;
        }
        */
        if (_bconnect == true)
        {
            Debug.Log("CheckConnect return _bconnect");
            return true;
        }

        ReSvrConnect();
        return false;
    }



    public void SvrConnect_LoginToLobby()
    {
        StartTimeOutConnect();
        NETStatic.PktLgn.ReqMoveToLobby();
    }

    public void SvrConnect_TitleToLobby(OnReceiveCallBack ReceiveCallBack)
    {
        SetCallBackList("Send_TitleToLobby", ReceiveCallBack);
        SvrConnect_LoginToLobby();
    }

    public void SvrConnect_BattleToLobby()
    {
        NETStatic.PktBtl.ReqMoveToLobby();
    }

    public void SvrConnect_LobbyToBattle()
    {
        NETStatic.PktLby.ReqMoveToBattle();
    }

    public void DoOnJoinServerComplete_Base(ErrorInfo info, ByteArray replyFromServer)
    {
        WaitPopup.Hide();
        StopTimeOut();

        if (info.errorType != ErrorType.Ok)
        {
            if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
            {
                MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3068), OnMsg_TitleReset);
            }
            else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby || AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
            {
                MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3069), OnMsg_ToTitle);//타이틀로
            }
            else
            {
                Debug.Log("DoOnJoinServerComplete_Base");
            }
            return;
        }


        NETStatic.PktGbl.Recv().AckPing = RecvAckPing;
        NETStatic.PktGbl.Recv().AckGetTotlaRelocateCntToNotComplete = RecvAckGetTotlaRelocateCntToNotComplete;
        NETStatic.PktGbl.Recv().AckRelocateUserInfoSet = RecvAckRelocateUserInfoSet;
        NETStatic.PktGbl.Recv().AckRelocateUserComplate = RecvAckRelocateUserComplate;
        NETStatic.PktGbl.Recv().AckRefrashUserInfo = RecvAckRefrashUserInfo;
        NETStatic.PktGbl.Recv().AckReConnectUserInfo = RecvAckReConnectUserInfo;
        NETStatic.PktGbl.Recv().AckAccountCode = RecvAckAccountCode;
        NETStatic.PktGbl.Recv().AckAccountSetPassword = RecvAckAccountSetPassword;
        NETStatic.PktGbl.Recv().AckAccountCodeReward = RecvAckAccountCodeReward;
        NETStatic.PktGbl.Recv().AckAccountLinkReward = RecvAckAccountLinkReward;
        NETStatic.PktGbl.Recv().AckLinkAccountList = RecvAckLinkAccountList;
        NETStatic.PktGbl.Recv().AckAddLinkAccountAuth = RecvAckAddLinkAccountAuth;
        NETStatic.PktGbl.Recv().AckGetUserInfoFromAccountLink = RecvAckGetUserInfoFromAccountLink;
        NETStatic.PktGbl.Recv().AckPushNotifiTokenSet = RecvAckPushNotifiTokenSet;

        // 캐릭터 관련
        NETStatic.PktGbl.Recv().AckAddCharacter = RecvAckAddCharacter;
        NETStatic.PktGbl.Recv().AckChangeMainChar = RecvAckChangeMainChar;
        NETStatic.PktGbl.Recv().AckChangePreferenceNum = RecvAckChangePreferenceNum;
        NETStatic.PktGbl.Recv().AckGradeUpChar = RecvAckGradeUpChar;
        NETStatic.PktGbl.Recv().AckSetGradeLvChar = RecvAckSetGradeLvChar;
        NETStatic.PktGbl.Recv().AckSetMainCostumeChar = RecvAckSetMainCostumeChar;
        NETStatic.PktGbl.Recv().AckRandomCostumeDyeing = RecvAckRandomCostumeDyeing;
        NETStatic.PktGbl.Recv().AckSetCostumeDyeing = RecvAckSetCostumeDyeing;
        NETStatic.PktGbl.Recv().AckCostumeDyeingLock = RecvAckCostumeDyeingLock;
        NETStatic.PktGbl.Recv().AckUserCostumeColor = RecvAckUserCostumeColor;

        NETStatic.PktGbl.Recv().AckEquipWeaponChar = RecvAckEquipWeaponChar;

        NETStatic.PktGbl.Recv().AckLvUpSkill = RecvAckLvUpSkill;
        NETStatic.PktGbl.Recv().AckLvUpUserSkill = RecvAckLvUpUserSkill;
        NETStatic.PktGbl.Recv().AckResetUserSkill = RecvAckResetUserSkill;
        NETStatic.PktGbl.Recv().AckGivePresentChar = RecvAckGivePresentChar;
        NETStatic.PktGbl.Recv().AckResetSecretCntChar = RecvAckResetSecretCntChar;
        NETStatic.PktGbl.Recv().AckApplySkillInChar = RecvAckApplySkillInChar;
        NETStatic.PktGbl.Recv().AckStageStart = RecvAckStageStart;
        NETStatic.PktGbl.Recv().AckStageEnd = RecvAckStageEnd;
        NETStatic.PktGbl.Recv().AckStageEndFail = RecvAckStageEndFail;
        NETStatic.PktGbl.Recv().AckStageContinue = RecvAckStageContinue;

        NETStatic.PktGbl.Recv().AckBookNewConfirm = RecvAckBookNewConfirm;

        NETStatic.PktGbl.Recv().AckTimeAtkRankingList = RecvAckTimeAtkRankingList;
        NETStatic.PktGbl.Recv().AckTimeAtkRankerDetail = RecvAckTimeAtkRankerDetail;

        NETStatic.PktGbl.Recv().AckArenaSeasonPlay = RecvAckArenaSeasonPlay;
        NETStatic.PktGbl.Recv().AckSetArenaTeam = RecvAckSetArenaTeam;
        NETStatic.PktGbl.Recv().AckArenaGameStart = RecvAckArenaGameStart;
        NETStatic.PktGbl.Recv().AckArenaGameEnd = RecvAckArenaGameEnd;
        NETStatic.PktGbl.Recv().AckArenaEnemySearch = RecvAckArenaEnemySearch;
        NETStatic.PktGbl.Recv().AckArenaRankingList = RecvAckArenaRankingList;
        NETStatic.PktGbl.Recv().AckArenaRankerDetail = RecvAckArenaRankerDetail;
        NETStatic.PktGbl.Recv().AckSetArenaTowerTeam = RecvAckSetArenaTowerTeam;
        NETStatic.PktGbl.Recv().AckArenaTowerGameStart = RecvAckArenaTowerGameStart;
        NETStatic.PktGbl.Recv().AckArenaTowerGameEnd = RecvAckArenaTowerGameEnd;
        NETStatic.PktGbl.Recv().AckUnexpectedPackageDailyReward = RecvAckUnexpectedPackageDailyReward;

        NETStatic.PktGbl.Recv().NotiSvrReloadTableInfo = RecvNotiSvrReloadTableInfo;
        NETStatic.PktGbl.Recv().NotiCloseServTime = RecvNotiCloseServTime;
        NETStatic.PktGbl.Recv().NotiCommonErr = RecvNotiCommonErr;
        NETStatic.PktGbl.Recv().NotiEmbargoWordErr = RecvNotiEmbargoWordErr;
        NETStatic.PktGbl.Recv().NotiCheckVersionErr = RecvNotiCheckVersionErr;
        NETStatic.PktGbl.Recv().NotiUpdateTicket = RecvNotiUpdateTicket;
        NETStatic.PktGbl.Recv().NotiUserMarkTake = RecvNotiUserMarkTake;
        NETStatic.PktGbl.Recv().NotiAddMail = RecvNotiAddMail;
        NETStatic.PktGbl.Recv().NotiUpdateAchieve = RecvNotiUpdateAchieve;
        NETStatic.PktGbl.Recv().NotiUpdateAchieveEvent = RecvNotiUpdateAchieveEvent;
        NETStatic.PktGbl.Recv().NotiSetSvrRotationGacha = RecvNotiSetSvrRotationGacha;
        NETStatic.PktGbl.Recv().NotiSetSvrSecretQuestOpt = RecvNotiSetSvrSecretQuestOpt;
        NETStatic.PktGbl.Recv().NotiUpdateDailyMission = RecvNotiUpdateDailyMission;
        NETStatic.PktGbl.Recv().NotiResetWeekMission = RecvNotiResetWeekMission;
        NETStatic.PktGbl.Recv().NotiUpdateWeekMission = RecvNotiUpdateWeekMission;
        NETStatic.PktGbl.Recv().NotiUpdateInfluMission = RecvNotiUpdateInfluMission;
        NETStatic.PktGbl.Recv().NotiUserInfluMissionChange = RecvNotiUserInfluMissionChange;
        NETStatic.PktGbl.Recv().NotiUpdateGllaMission = RecvNotiUpdateGllaMission;
        NETStatic.PktGbl.Recv().NotiUpdatePassMission = RecvNotiUpdatePassMission;
        NETStatic.PktGbl.Recv().NotiUserPassChange = RecvNotiUserPassChange;
        NETStatic.PktGbl.Recv().NotiUserEventChange = RecvNotiUserEventChange;
        NETStatic.PktGbl.Recv().NotiUpdateArenaTime = RecvNotiUpdateArenaTime;
        NETStatic.PktGbl.Recv().NotiCommunityUserArenaOn = RecvNotiCommunityUserArenaOn;
        NETStatic.PktGbl.Recv().NotiCommunityUserCallCnt = RecvNotiCommunityUserCallCnt;
        NETStatic.PktGbl.Recv().NotiCommunitySetArenaTowerID = RecvNotiCommunitySetArenaTowerID;
        NETStatic.PktGbl.Recv().NotiFriendFromAdd = RecvNotiFriendFromAdd;
        NETStatic.PktGbl.Recv().NotiFriendFromDel = RecvNotiFriendFromDel;
        NETStatic.PktGbl.Recv().NotiFriendAnswer = RecvNotiFriendAnswer;
        NETStatic.PktGbl.Recv().NotiFriendKick = RecvNotiFriendKick;
        NETStatic.PktGbl.Recv().NotiFriendFlagUpdate = RecvNotiFriendFlagUpdate;
        //NETStatic.PktGbl.Recv().NotiMoveUserInSvrToSvr 
        NETStatic.PktGbl.Recv().AckAccountDelete = RecvAccountDelete;
        NETStatic.PktGbl.Recv().AckInitRaidSeasonData = RecvInitRaidSeasonData;
        NETStatic.PktGbl.Recv().AckRaidRankingList = RecvAckRaidRankingList;
        NETStatic.PktGbl.Recv().AckRaidFirstRankingList = RecvAckRaidFirstRankingList;
        NETStatic.PktGbl.Recv().AckRaidHPRestore = RecvRaidHPRestore;
        NETStatic.PktGbl.Recv().AckRaidRankerDetail = RecvAckRaidRankerDetail;
        NETStatic.PktGbl.Recv().AckRaidFirstRankerDetail = RecvAckFirstRaidRankerDetail;
        NETStatic.PktGbl.Recv().AckSetRaidTeam = RecvAckSetRaidTeam;
        NETStatic.PktGbl.Recv().AckRaidStageDrop = RecvRaidStageDrop;
        NETStatic.PktGbl.Recv().NotiUpdateRaidTime = RecvNotiUpdateRaidTimeEnd;
        NETStatic.PktGbl.Recv().NotiUpdateRaidInitStart = RecvNotiUpdateRaidInitStart;
        NETStatic.PktGbl.Recv().AckCharLvUnexpectedPackageHardOpen = RecvAckCharLvUnexpectedPackageHardOpen;

        // 서클 관련
        NETStatic.PktGbl.Recv().AckCircleOpen = RecvAckCircleOpen;
        NETStatic.PktGbl.Recv().AckSuggestCircleList = RecvAckSuggestCircleList;
        NETStatic.PktGbl.Recv().AckCircleJoin = RecvAckCircleJoin;
        NETStatic.PktGbl.Recv().AckCircleJoinCancel = RecvAckCircleJoinCancel;
        NETStatic.PktGbl.Recv().AckCircleLobbyInfo = RecvAckCircleLobbyInfo;
        NETStatic.PktGbl.Recv().AckCircleWithdrawal = RecvAckCircleWithdrawal;
        NETStatic.PktGbl.Recv().AckCircleDisperse = RecvAckCircleDisperse;
        NETStatic.PktGbl.Recv().AckGetCircleUserList = RecvAckGetCircleUserList;
        NETStatic.PktGbl.Recv().AckCircleChangeStateJoinWaitUser = RecvAckCircleChangeStateJoinWaitUser;
        NETStatic.PktGbl.Recv().AckCircleUserKick = RecvAckCircleUserKick;
        NETStatic.PktGbl.Recv().AckCircleChangeAuthLevel = RecvAckCircleChangeAuthLevel;
        NETStatic.PktGbl.Recv().AckCircleChangeMark = RecvAckCircleChangeMark;
        NETStatic.PktGbl.Recv().AckCircleChangeName = RecvAckCircleChangeName;
        NETStatic.PktGbl.Recv().AckCircleChangeComment = RecvAckCircleChangeComment;
        NETStatic.PktGbl.Recv().AckCircleChangeMainLanguage = RecvAckCircleChangeMainLanguage;
        NETStatic.PktGbl.Recv().AckCircleChangeSuggestAnotherLangOpt = RecvAckCircleChangeSuggestAnotherLangOpt;
        NETStatic.PktGbl.Recv().AckCircleAttendance = RecvAckCircleAttendance;
        NETStatic.PktGbl.Recv().AckCircleBuyMarkItem = RecvAckCircleBuyMarkItem;
        NETStatic.PktGbl.Recv().AckCircleGetMarkList = RecvAckCircleGetMarkList;
        NETStatic.PktGbl.Recv().AckCircleSearch = RecvAckCircleSearch;
        NETStatic.PktGbl.Recv().AckCircleChatList = RecvAckCircleChatList;
        NETStatic.PktGbl.Recv().NotiCircleAcceptJoin = RecvNotiCircleAcceptJoin;
        NETStatic.PktGbl.Recv().NotiCircleUserKick = RecvNotiCircleUserKick;
        NETStatic.PktGbl.Recv().NotiCircleUserChangeAuth = RecvNotiCircleUserChangeAuth;
        NETStatic.PktGbl.Recv().NotiCircleChat = RecvNotiCircleChat;
        NETStatic.PktGbl.Recv().NotiCircleChatNotimessage = RecvNotiCircleChatNotimessage;

        // 튜토리얼 관련
        NETStatic.PktGbl.Recv().AckSetTutorialVal = RecvAckSetTutorialVal;
        NETStatic.PktGbl.Recv().AckSetTutorialFlag = RecvAckSetTutorialFlag;
    }

	public void DoOnJoinServerComplete_Login(ErrorInfo info, ByteArray replyFromServer)
    {
        if (ErrorType.Ok == info.errorType)
        {
            _servertype = eServerType.LOGIN;
            SetConnectValue(true);

            NETStatic.PktLgn.Recv().AckClientSecurityInfo = RecvAckClientSecurityInfo;
            NETStatic.PktLgn.Recv().AckClientSecurityVerify = RecvAckClientSecurityVerify;
            NETStatic.PktLgn.Recv().NotiFirstLogin = RecvNotiFirstLogin;
            NETStatic.PktLgn.Recv().NotiUserLoginChangeData = RecvNotiUserLoginChangeData;
            NETStatic.PktLgn.Recv().AckErrLoginUserBlock = RecvAckErrLoginUserBlock;
            NETStatic.PktLgn.Recv().AckLoginAndUserInfo = RecvAckLoginAndUserInfo;

            if (_bautologin)
            {
#if !DISABLESTEAMWORKS
                if(!IsAccount()) // 로그인 서버로 들어왔는데 로컬에 UUID가 없을때
                {
                    // 데이터 초기화로 들어온거면 UUID를 0으로 해서 로그인 시도.
                    if (PlayerPrefs.GetInt("InitData") > 0) 
                    {
                        Send_ReqLogin(TitleUIManager.Instance.OnNetLogin);
                        PlayerPrefs.DeleteKey("InitData");
                    }
                    else
                    {
                        // 데이터 초기화가 아니면 GetLink로 연동된 UUID를 찾아서 그 UUID로 로그인.
                        // 연동된 UUID가 없으면 SVR_USER_LOGIN_ERR_NOT_FIND_UUID_IN_KEEP_ON를 받은 곳에서 UUID 0으로 로그인 시도
                        Send_ReqGetUserInfoFromAccountLink(eAccountType.STEAM, AppMgr.Instance.SteamId.ToString(), "",
                                                           TitleUIManager.Instance.OnSteamNetGetUserInfoFromAccountLink);
                    }

                    return;
                }
                else if ( PlayerPrefs.HasKey( "User_SteamID" ) ) {
					ulong.TryParse( PlayerPrefs.GetString( "User_SteamID", "0" ), out ulong steamID );
                    if ( steamID != AppMgr.Instance.SteamId ) {
						Send_ReqGetUserInfoFromAccountLink( eAccountType.STEAM, AppMgr.Instance.SteamId.ToString(), "", TitleUIManager.Instance.OnSteamNetGetUserInfoFromAccountLink );
                        return;
					}
				}
#endif
                Send_ReqLogin(GetCallBackList("SvrConnect_Login"));
            }
            else
            {
                OnReceiveCallBack callback = GetCallBackList("SvrConnect_Login");
                if (callback != null)
                    callback((int)info.errorType, null);
            }
        }
    }

    // Global , Product , Lobby  패킷의 프라우드넷 함수와 Recv 연결
    public void DoOnJoinServerComplete_Lobby(ErrorInfo info, ByteArray replyFromServer)
    {
        
        WaitPopup.Hide();

        _servertype = eServerType.LOBBY;
        SetConnectValue(true);

        // product 패킷 Recv 시작 

        // 인벤토리 관련
        NETStatic.PktProd.Recv().AckAddItemSlot = RecvAckAddItemSlot;
        // 카드(서포터) 관련
        NETStatic.PktProd.Recv().AckApplyPosCard = RecvAckApplyPosCard;
        NETStatic.PktProd.Recv().AckApplyOutPosCard = RecvAckApplyOutPosCard;
        NETStatic.PktProd.Recv().AckSellCard = RecvAckSellCard;
        NETStatic.PktProd.Recv().AckSetLockCard = RecvAckSetLockCard;
        NETStatic.PktProd.Recv().AckChangeTypeCard = RecvAckChangeTypeCard;
        NETStatic.PktProd.Recv().AckLvUpCard = RecvAckLvUpCard;
        NETStatic.PktProd.Recv().AckWakeCard = RecvAckWakeCard;
        NETStatic.PktProd.Recv().AckEnchantCard = RecvAckEnchantCard;
        NETStatic.PktProd.Recv().AckSkillLvUpCard = RecvAckSkillLvUpCard;
        NETStatic.PktProd.Recv().AckFavorLvRewardCard = RecvAckFavorLvRewardCard;

        // 아이템 관련
        NETStatic.PktProd.Recv().AckSellItem = RecvAckSellItem;
        NETStatic.PktProd.Recv().AckItemExchangeCash = RecvAckItemExchangeCash;
        NETStatic.PktProd.Recv().AckUseItemGoods = RecvAckUseItemGoods;
        NETStatic.PktProd.Recv().AckUseItemProduct = RecvAckUseItemProduct;
        NETStatic.PktProd.Recv().AckUseItemCode = RecvAckUseItemCode;
        NETStatic.PktProd.Recv().AckUseItemStageSpecial = RecvAckUseItemStageSpecial;

        // 곡옥 관련
        NETStatic.PktProd.Recv().AckSellGem = RecvAckSellGem;
        NETStatic.PktProd.Recv().AckSetLockGem = RecvAckSetLockGem;
        NETStatic.PktProd.Recv().AckLvUpGem = RecvAckLvUpGem;
        NETStatic.PktProd.Recv().AckWakeGem = RecvAckWakeGem;
        NETStatic.PktProd.Recv().AckResetOptGem = RecvAckResetOptGem;
        NETStatic.PktProd.Recv().AckResetOptSelectGem = RecvAckResetOptSelectGem;
		NETStatic.PktProd.Recv().AckEvolutionGem = RecvAckEvolutionGem;
		NETStatic.PktProd.Recv().AckAnalyzeGem = RecvAckAnalyzeGem;

		// 무기 관련
		NETStatic.PktProd.Recv().AckSellWeapon = RecvAckSellWeapon;
        NETStatic.PktProd.Recv().AckSetLockWeapon = RecvAckSetLockWeapon;
        NETStatic.PktProd.Recv().AckLvUpWeapon = RecvAckLvUpWeapon;
        NETStatic.PktProd.Recv().AckWakeWeapon = RecvAckWakeWeapon;
        NETStatic.PktProd.Recv().AckEnchantWeapon = RecvAckEnchantWeapon;
        NETStatic.PktProd.Recv().AckApplyGemInWeapon = RecvAckApplyGemInWeapon;
        NETStatic.PktProd.Recv().AckAddSlotInWpnDepot = RecvAckAddSlotInWpnDepot;
        NETStatic.PktProd.Recv().AckApplySlotInWpnDepot = RecvAckApplySlotInWpnDepot;
        NETStatic.PktProd.Recv().AckSkillLvUpWeapon = RecvAckSkillLvUpWeapon;

        // 분해 관련
        NETStatic.PktProd.Recv().AckDecomposition = RecvAckDecomposition;

        // 문양 관련
        NETStatic.PktProd.Recv().AckApplyPosBadge = RecvAckApplyPosBadge;
        NETStatic.PktProd.Recv().AckApplyOutPosBadge = RecvAckApplyOutPosBadge;
        NETStatic.PktProd.Recv().AckSellBadge = RecvAckSellBadge;
        NETStatic.PktProd.Recv().AckSetLockBadge = RecvAckSetLockBadge;
        NETStatic.PktProd.Recv().AckUpgradeBadge = RecvAckUpgradeBadge;
        NETStatic.PktProd.Recv().AckResetUpgradeBadge = RecvAckResetUpgradeBadge;
        
        // Lobby  패킷 Recv  시작

        // 스토어 관련
        NETStatic.PktLby.Recv().AckStorePurchase = RecvAckStorePurchase;
        NETStatic.PktLby.Recv().AckStorePurchaseInApp = RecvAckStorePurchaseInApp;
        NETStatic.PktLby.Recv().AckUserRotationGachaOpen = RecvAckUserRotationGachaOpen;
        NETStatic.PktLby.Recv().AckRaidStoreList = RecvAckRaidStoreList;

        NETStatic.PktLby.Recv().AckSetMainRoomTheme = RecvAckSetMainRoomTheme;
        NETStatic.PktLby.Recv().AckRoomPurchase = RecvAckRoomPurchase;
        NETStatic.PktLby.Recv().AckRoomThemeSlotDetailInfo = RecvAckRoomThemeSlotDetailInfo;
        NETStatic.PktLby.Recv().AckRoomThemeSlotSave = RecvAckRoomThemeSlotSave;

        //로비 유저 관련
        NETStatic.PktLby.Recv().AckUserMarkList = RecvAckUserMarkList;
        NETStatic.PktLby.Recv().AckUserSetMark = RecvAckUserSetMark;
        NETStatic.PktLby.Recv().AckUserLobbyThemeList = RecvAckUserLobbyThemeList;
        NETStatic.PktLby.Recv().AckUserSetLobbyTheme = RecvAckUserSetLobbyTheme;
        NETStatic.PktLby.Recv().AckUserSetMainCardFormation = RecvAckUserSetMainCardFormation;
        NETStatic.PktLby.Recv().AckUserCardFormationFavi = RecvAckUserCardFormationFavi;
        NETStatic.PktLby.Recv().AckUserSetName = RecvAckUserSetName;
        NETStatic.PktLby.Recv().AckUserSetNameColor = RecvAckUserSetNameColor;
        NETStatic.PktLby.Recv().AckUserSetCommentMsg = RecvAckUserSetCommentMsg;
        NETStatic.PktLby.Recv().AckUserSetCountryAndLangCode = RecvAckUserSetCountryAndLangCode;
        NETStatic.PktLby.Recv().AckUserPkgShowOff = RecvAckUserPkgShowOff;

        //시설관련
        NETStatic.PktLby.Recv().AckFacilityUpgrade = RecvAckFacilityUpgrade;
        NETStatic.PktLby.Recv().AckFacilityOperation = RecvAckFacilityOperation;
        NETStatic.PktLby.Recv().AckFacilityOperationConfirm = RecvAckFacilityOperationConfirm;

        //파견관련
        NETStatic.PktLby.Recv().AckDispatchOpen = RecvAckDispatchOpen;
        NETStatic.PktLby.Recv().AckDispatchChange = RecvAckDispatchChange;
        NETStatic.PktLby.Recv().AckDispatchOperation = RecvAckDispatchOperation;
        NETStatic.PktLby.Recv().AckDispatchOperationConfirm = RecvAckDispatchOperationConfirm;

        //우편함 관련
        NETStatic.PktLby.Recv().AckMailList = RecvAckMailList;
        NETStatic.PktLby.Recv().AckMailTakeProductList = RecvAckMailTakeProductList;

        //커뮤니티
        NETStatic.PktLby.Recv().AckCommunityInfoGet = RecvAckCommunityInfoGet;
        NETStatic.PktLby.Recv().AckCommunityUserArenaInfoGet = RecvAckCommunityUserArenaInfoGet;
        NETStatic.PktLby.Recv().AckCommunityUserUseCallCnt = RecvAckCommunityUserUseCallCnt;
        //친구
        NETStatic.PktLby.Recv().AckFriendSuggestList = RecvAckFriendSuggestList;
        NETStatic.PktLby.Recv().AckFriendAsk = RecvAckFriendAsk;
        NETStatic.PktLby.Recv().AckFriendAskDel = RecvAckFriendAskDel;
        NETStatic.PktLby.Recv().AckFriendAnswer = RecvAckFriendAnswer;
        NETStatic.PktLby.Recv().AckFriendKick = RecvAckFriendKick;
        NETStatic.PktLby.Recv().AckFriendPointGive = RecvAckFriendPointGive;
        NETStatic.PktLby.Recv().AckFriendPointTake = RecvAckFriendPointTake;
        NETStatic.PktLby.Recv().AckFriendRoomVisitFlag = RecvAckFriendRoomVisitFlag;
        NETStatic.PktLby.Recv().AckFriendRoomInfoGet = RecvAckFriendRoomInfoGet;

        // 서버 달성(세력)
        NETStatic.PktLby.Recv().AckInfluenceChoice = RecvAckInfluenceChoice;
        NETStatic.PktLby.Recv().AckGetInfluenceInfo = RecvAckGetInfluenceInfo;
        NETStatic.PktLby.Recv().AckGetInfluenceRankInfo = RecvAckGetInfluenceRankInfo;
        NETStatic.PktLby.Recv().AckInfluenceTgtRwd = RecvAckInfluenceTgtRwd;

        // 이벤트 게시판
        NETStatic.PktLby.Recv().AckBingoEventReward = RecvAckBingoEventReward;
        NETStatic.PktLby.Recv().AckBingoNextOpen = RecvAckBingoNextOpen;
        NETStatic.PktGbl.Recv().AckRewardTakeAchieveEvent = RecvAckRewardTakeAchieveEvent;

        // 로그인 보너스 관련
        NETStatic.PktGbl.Recv().AckReflashLoginBonus = RecvAckReflashLoginBonus;

        // 공적 관련
        NETStatic.PktGbl.Recv().AckRewardTakeAchieve = RecvAckRewardTakeAchieve;

        // 미션 관련
        NETStatic.PktGbl.Recv().AckRewardDailyMission = RecvAckRewardDailyMission;
        NETStatic.PktGbl.Recv().AckRewardWeekMission = RecvAckRewardWeekMission;
        NETStatic.PktGbl.Recv().AckRewardInfluMission = RecvAckRewardInfluMission;
        NETStatic.PktGbl.Recv().AckUpdateGllaMission = RecvAckUpdateGllaMission;
        NETStatic.PktGbl.Recv().AckRewardGllaMission = RecvAckRewardGllaMission;
        NETStatic.PktGbl.Recv().AckRewardPassMission = RecvAckRewardPassMission;
        NETStatic.PktGbl.Recv().AckRewardPass = RecvAckRewardPass;

        //이벤트모드 관련
        NETStatic.PktGbl.Recv().AckEventRewardReset = RecvAckEventRewardReset;
        NETStatic.PktGbl.Recv().AckEventRewardTake = RecvAckEventRewardTake;
        NETStatic.PktGbl.Recv().AckEventLgnRewardTake = RecvAckEventLgnRewardTake;

        // 프리셋 관련
        NETStatic.PktGbl.Recv().AckGetUserPresetList = RecvAckGetUserPresetList;
        NETStatic.PktGbl.Recv().AckAddOrUpdateUserPreset = RecvAckAddOrUpdateUserPreset;
        NETStatic.PktGbl.Recv().AckUserPresetLoad = RecvAckUserPresetLoad;
        NETStatic.PktGbl.Recv().AckUserPresetChangeName = RecvAckUserPresetChangeName;


        if (_timeping != -1)
            FGlobalTimer.Instance.RemoveTimer(_timeping);
        _timeping = FGlobalTimer.Instance.AddTimer(60.0f, OnTimePing);

        if (_reconnect == true)
        {
            _reconnect = false;
            Send_ReqReConnectUserInfo(OnNetReLogin);
        }
        else
        {
            NETStatic.PktGbl.ReqLogOnCreditKey();

            if (GameInfo.Instance.CharSelete)
            {
                //RecvProtocolData(null);

                OnReceiveCallBack callback = GetCallBackList("Send_ReqAddCharacter");
                if (callback != null)
                    callback(0, null);

                callback = null;
                callback = GetCallBackList("Send_TitleToLobby");
                if (callback != null)
                    callback(0, null);
            }
            else
            {
                OnReceiveCallBack callback = GetCallBackList("Send_ReqLogin");
                if (callback != null)
                    callback(0, null);
            }
        }
    }

    public void DoOnJoinServerComplete_Battle(ErrorInfo info, ByteArray replyFromServer)
    {
        WaitPopup.Hide();

        _servertype = eServerType.BATTLE;
        SetConnectValue(true);

        NETStatic.PktGbl.ReqLogOnCreditKey();
    }

    public void DoOnLeaveServer_Base(ErrorInfo errorInfo)
    {
        SetConnectValue(false);
    }

    public void DoOnServerOffline(RemoteOfflineEventArgs args)
    {
        SetConnectValue(false);
    }

    public void DoOnLeaveServer_Login(ErrorInfo errorInfo)
    {
        SetConnectValue(false);
    }

    public void DoOnLeaveServer_Lobby(ErrorInfo errorInfo)
    {
        SetConnectValue(false);
    }

    public void DoOnLeaveServer_Battle(ErrorInfo errorInfo)
    {
        SetConnectValue(false);
    }

    public void OnError_Base(ErrorInfo errorInfo)
    {

    }
    public void OnWarning_Base(ErrorInfo errorInfo)
    {

    }
    public void OnException_Base(Exception e)
    {
        Debug.LogException(e);
    }
    
    public void NotiCommonErr(HostID remote, RmiContext rmiContext, System.UInt64 _errNum)
    {
        WaitPopup.Hide();
        ClearProtocolData();
        StopTimeOut();

        if (_reconnect == true)
        {
            _reconnect = false;
            //WaitPopup.Show();
            //SvrConnect_Login(OnNetReLogin); //재로그인 처리 SVR_USER_RELOGIN_ERR_NOT_FIND_INFO 일때
            int errMsgIdx = 3031;
            if (_errNum == (int)eTMIErrNum.SVR_USER_RELOGIN_ERR_NOT_FIND_INFO)
            {
                errMsgIdx = (int)eTMIErrNum.SVR_USER_RELOGIN_ERR_NOT_FIND_INFO;
            }
            MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(errMsgIdx), OnMsg_ToTitle);//타이틀로
        }
        else
        {
            bool btitlereset = false;
            if (_errNum == (int)eTMIErrNum.SVR_DB_FUNC_ERR_ALREADY_EXIST_ACCOUNT_LINK)
            {
                MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3142), null);    //연동할 계정이 이미 연동이 되어있음, 알림표시(추후 수정)
                btitlereset = true;
            }
            else if (_errNum == (int)eTMIErrNum.COM_ERR_NOT_FIND_USER) //유저를 찾을 수 없습니다.  //재접속 및 밀어내기 타이틀로 이동하면 됨
            {
                MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3031), OnMsg_ToTitle);//타이틀로
            }
            else if (_errNum == (int)eTMIErrNum.SVR_COM_ERR_NOT_FIND_ACCOUNT_UUID)//해당 계정 유저 정보를 찾을 수 없습니다. 새계정을 만드시겠습니까? 예, 아니오
            {
                if (AppMgr.Instance.DisableCreateNewUser)
                {
                    TitleUIManager.Instance.ShowDisableCreateNewUserPopup(true);
                }
                else
                {
                    MessagePopup.YN(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3097), OnMsg_NewAccountYes, OnMsg_TitleReset);//타이틀로
                }
            }
            else if (_errNum == (int)eTMIErrNum.SVR_USER_ERR_USER_SHIFT_SESSION_ABOUT_RECONNECT ||  // 유저 정보가 세로운 접속으로 연결됐기 때문에 다시 로그인 해주시기 바랍니다.
                     _errNum == (int)eTMIErrNum.SVR_USER_ERR_EMPTY_USER_SHIFT_SESSION)              // 유저 정보가 잘못되서 재접속 중인 유저 정보를 연결하지 못했습니다.
            {
                if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
                    MessagePopup.OK(eTEXTID.SERVERERROR, FLocalizeString.Instance.GetText((int)_errNum), OnMsg_TitleReset);
                else
                    MessagePopup.OK(eTEXTID.SERVERERROR, FLocalizeString.Instance.GetText((int)_errNum) + "\n" + FLocalizeString.Instance.GetText(3098), OnMsg_ToTitle);//타이틀로
            }
            else if (_errNum == (int)eTMIErrNum.SVR_STOR_INAPP_ERR_CANCELED_APPROVAL || // 결재 검증 진행중에 영수증이 승인 취소된 상품이라서 처리하지 못했습니다.
                    _errNum == (int)eTMIErrNum.SVR_STOR_PUC_ERR_NOT_DURING_SEASON_SALE ||       // 해당 상품의 구매 가능 시기가 아니라서 처리할 수 없습니다.
                    _errNum == (int)eTMIErrNum.SVR_STOR_PUC_ERR_NOT_OVER_CYCLEMINUTE ||         // 해당 상품의 무료 구매 시간이 아니여서 처리할 수 없습니다.
                    _errNum == (int)eTMIErrNum.SVR_STOR_PUC_ERR_ALREADY_OVER_LIMIT_COUNT ||    // 해당 상품의 구매 제한 개수를 초과해서 처리할 수 없습니다.
                    _errNum == (int)eTMIErrNum.SVR_STOR_INAPP_ERR_ALREADY_USE_RECEIPT_INFO || // 이미 사용한 영수증 정보라서 처리하지 못했습니다.
                    _errNum == (int)eTMIErrNum.SVR_STOR_INAPP_ERR_UNABLE_RECEIPT_VERIFY ||    // 결재 검증 진행중에 영수증 인증 할 수 없습니다.
                    _errNum == (int)eTMIErrNum.SVR_STOR_INAPP_ERR_NOT_EXIST_RECEIPT
                )
            {
                MessagePopup.OK(eTEXTID.SERVERERROR, FLocalizeString.Instance.GetText((int)_errNum) + "\n" + _errNum.ToString(), () =>
                {
                    //저장되어있던 영수증 정보 제거
                    PlayerPrefs.DeleteKey("IAPBuyReceipt");
                    PlayerPrefs.DeleteKey("IAPBuyStoreID");
                    PlayerPrefs.Save();

                    IAPManager.Instance.IsBuying = false;
                    WaitPopup.Hide();
                });
            }
            else if (_errNum == (int)eTMIErrNum.COM_ERR_NOT_FIND_CONDITION_ABOUT_TARGET)
            {
                MessagePopup.OK(eTEXTID.SERVERERROR, FLocalizeString.Instance.GetText((int)_errNum) + "\n" + _errNum.ToString(), null);
                if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
                {
                    LobbyUIManager.Instance.Renewal("FriendPopup");
                }
            }
            else if (_errNum == (int)eTMIErrNum.SVR_USER_RELOGIN_ERR_NOT_FIND_INFO)
            {
                MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText((int)eTMIErrNum.SVR_USER_RELOGIN_ERR_NOT_FIND_INFO), OnMsg_ToTitle);//타이틀로
            }
            else if (_errNum == (int)eTMIErrNum.COM_ERR_FORCE_USER_KICK) // 강제 추방
            {
                MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3152), () => Application.Quit());
            }
            else if (_errNum == (int)eTMIErrNum.SVR_USER_LOGIN_ERR_NOT_FIND_UUID_IN_KEEP_ON ||
                    _errNum == (int)eTMIErrNum.SVR_USER_LOGIN_ERR_NOT_INPUT_KEEP_ON_ID ||
                    _errNum == (int)eTMIErrNum.SVR_USER_LOGIN_ERR_NOT_INPUT_KEEP_ON_ACCOUNT_ID ||
                    _errNum == (int)eTMIErrNum.SVR_USER_LOGIN_ERR_INVALID_KEEP_ON_KIND)
            {
#if !DISABLESTEAMWORKS
                // 스팀에서 GetLink로 연동된 UUID를 찾지 못했을 땐 UUID를 0으로 해서 로그인 시도
                if (UsedAccountTypeInGetLink == eAccountType.STEAM && _errNum == (int)eTMIErrNum.SVR_USER_LOGIN_ERR_NOT_FIND_UUID_IN_KEEP_ON)
                {
                    Send_ReqLogin(TitleUIManager.Instance.OnNetLogin);
                }
                else
#endif
                {
                    MessagePopup.OK(eTEXTID.SERVERERROR, FLocalizeString.Instance.GetText((int)_errNum) + "\n" + _errNum.ToString(), null);
                }

                UsedAccountTypeInGetLink = eAccountType.DEFAULT;
            }
            else if (_errNum == (int)eTMIErrNum.SVR_STOR_INAPP_ERR_STEAM_INITTXN_FAILURE ||
                    _errNum == (int)eTMIErrNum.SVR_STOR_INAPP_ERR_STEAM_FINALIZETXN_FAILURE)
            {
                IAPManager.Instance.IsBuying = false;
                MessagePopup.OK(eTEXTID.SERVERERROR, ((eTMIErrNum)_errNum).ToString() + "\n" + _errNum.ToString(), null);
                btitlereset = true;
            }
            else if (_errNum == (int)eTMIErrNum.SVR_STOR_INAPP_ERR_NOT_SAME_PRODUCT_ID)
            {
                IAPManager.Instance.IsBuying = false;
                WaitPopup.Hide();
                MessagePopup.OK(eTEXTID.SERVERERROR, FLocalizeString.Instance.GetText((int)_errNum) + "\n" + _errNum.ToString(), null);
            }
            else if (_errNum == (int)eTMIErrNum.SVR_DB_FUNC_ERR_ALREADY_EXIST_RELOCATE_USER ||
                    _errNum == (int)eTMIErrNum.SVR_DB_FUNC_ERR_ALREADY_EXIST_ID ||
                    _errNum == (int)eTMIErrNum.SVR_DB_FUNC_ERR_NOT_FIND_ID_FOR_INFO ||
                    _errNum == (int)eTMIErrNum.SVR_DB_FUNC_ERR_ALREADY_COMPLATE_RELOCATE_USER ||
                    _errNum == (int)eTMIErrNum.SVR_DB_FUNC_ERR_NOT_SAME_PASSWORD)
            {
                GameInfo.Instance.StopTimeOut();
                MessagePopup.OK(eTEXTID.SERVERERROR, FLocalizeString.Instance.GetText(300000 + (int)_errNum), null);
            }
            else if (_errNum >= (int)eTMIErrNum.SVR_COM_ERR_DEFAULT && _errNum <= (int)eTMIErrNum.SVR_COM_ERR_NOT_FIND_LOBBY_SERVER)
            {
                MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText((int)_errNum) + "\n" + _errNum.ToString(), null);
            }
            else if (_errNum == (int)eTMIErrNum.SVR_DB_FUNC_ERR_NOT_ADD_OVER_RELOCATE_RESERVATION)
            {
                UIServerIDPopup popup = LobbyUIManager.Instance.GetUI<UIServerIDPopup>();
                if (popup)
                {
                    popup.OnClickClose();
                }

                MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(6022), null);
            }
            else if (_errNum == (int)eTMIErrNum.SVR_USER_ERR_ALREADY_DELETE_USER_INFO)
            {
                MessagePopup.OK(eTEXTID.SERVERERROR, FLocalizeString.Instance.GetText((int)_errNum) + "\n" + _errNum.ToString(), () =>
                {
                    TitleUIManager.Instance.HideAll(FComponent.TYPE.Popup);
                });

                btitlereset = true;
            }
            else if (_errNum == (int)eTMIErrNum.SVR_SESION_ERR_BANNED_SECURITY_VERIFY)
            {
                //스팀아이디가 다르거나 인증이 제대로 안됬을경우.
                MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3293),
                () => {
                    Application.Quit();
                },
                true);
            }
            else if( _errNum == (int)eTMIErrNum.SVR_STOR_RAID_NOT_EXIST_RESET_ITEM_LIST ) {
                UIRaidStorePopup raidStorePopup = LobbyUIManager.Instance.GetActiveUI<UIRaidStorePopup>( "RaidStorePopup" );
                if( raidStorePopup ) {
                    raidStorePopup.ShowNoItemText();
				}
			}
            else
            {
                if ((int)eTMIErrNum._START_USER_ <= _errNum && (int)eTMIErrNum.__COMMON_END__ > _errNum)
                {
                    string strErr = FLocalizeString.Instance.GetText((int)_errNum);
                    if (string.IsNullOrEmpty(strErr))
                    {
                        strErr = ((eTMIErrNum)(int)_errNum).ToString();
                    }

                    if (_errNum == (int)eTMIErrNum.COM_ERR_INVALID_TYPE)
                    {
#if !UNITY_EDITOR
                        MessagePopup.OK(eTEXTID.SERVERERROR, strErr + "\n" + _errNum.ToString(), ()=> { Application.Quit(); });
                        return;
#else
                        MessagePopup.OK(eTEXTID.SERVERERROR, strErr + "\n" + _errNum.ToString(), null);
#endif
                    }
                    else
                    {
                        MessagePopup.OK(eTEXTID.SERVERERROR, strErr + "\n" + _errNum.ToString(), null);
                    }


                }
                else
                {
                    MessagePopup.OK(eTEXTID.SERVERERROR, ((eTMIErrNum)_errNum).ToString() + "\n" + _errNum.ToString(), null);
                }
                btitlereset = true;
            }

            if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title && btitlereset)
            {
                OnMsg_TitleReset();
            }
        }
    }

    public void NotiCheckVersionErr(HostID remote, RmiContext rmiContext, PktInfoVersion _pktInfo)
    {
        string str = string.Empty;
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
        {
            str = string.Format(FLocalizeString.Instance.GetText(3104), AppMgr.Instance.configData.m_serverversionmain, AppMgr.Instance.configData.m_serverversionsub, _pktInfo.verMain_, _pktInfo.verSub_);
            MessagePopup.OK(eTEXTID.SERVERERROR, str, OnMsg_TitleReset);
        }
        else
        {
            str = string.Format(FLocalizeString.Instance.GetText(3103), AppMgr.Instance.configData.m_serverversionmain, AppMgr.Instance.configData.m_serverversionsub, _pktInfo.verMain_, _pktInfo.verSub_);
            MessagePopup.OK(eTEXTID.SERVERERROR, str, OnMsg_ToTitle);//타이틀로
        }
    }

    public void OnMsg_TitleReset()
    {
        ClearProtocolData();
        NETStatic.Mgr.DoDisConnectByActiveNet();

        TitleUIManager.Instance.OnMsg_Exit();
    }
    public void OnMsg_ToTitle()
    {
        ClearProtocolData();
        NETStatic.Mgr.DoDisConnectByActiveNet();

        GameInfo.Instance.MoveLobbyToTitle();
    }

    private void OnMsg_NewAccountYes()
    {
        PlayerPrefs.DeleteKey("User_AccountUUID");
        GameInfo.Instance.Send_ReqLogin(GetCallBackList("Send_ReqLogin"));
    }

    private void SetConnectValue(bool b)
    {
        bool btemp = _bconnect;
        _bconnect = b;

        WaitPopup.Hide();

        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
            LobbyUIManager.Instance.Renewal("TopPanel");
    }

    public void SetPktInfoUserReflashData(PktInfoUserReflash _pktInfo)
    {
        GameInfo.Instance.ServerData.SetPktData(_pktInfo);
        BannerManager.Instance.AddServerData(GameInfo.Instance.ServerData);
    }

    //대전상대 랜덤 생성
    public void GetArenaEnemyRandomTeam(PktInfoArenaSearchEnemyAck _pktInfo)
    {
        //10000미만이면 데이터를 랜덤으로 생성해주기.
        if (_pktInfo.enemyInfo_.userInfo_.uuid_ >= (int)eCOUNT.IS_ARENA_ENEMY_UUID_FLAG)
        {
            for (int i = 0; i < _matchteam.charlist.Count; i++)
            {
                if (_matchteam.charlist[i] == null)
                    continue;
                //패시브 스킬 캐릭터에 적용
                List<int> skillIds = GameSupport.CreateArenaOpponentCharPassiveSkillList(_matchteam.charlist[i].CharData.TableID);
                if (skillIds.Count > 0)
                {
                    for (int idx = 0; idx < skillIds.Count; idx++)
                    {
                        _matchteam.charlist[i].CharData.PassvieList.Add(new PassiveData(skillIds[idx], 1 + (int)((float)_matchteam.charlist[i].CharData.Level * GameInfo.Instance.BattleConfig.ArenaEnemyCharPassvieRate)));
                    }
                }
            }
            return;
        }

        TeamData matchTeamData = new TeamData();

        //서버에서 받은 데이터로 셋팅
        matchTeamData.UUID = (long)_pktInfo.enemyInfo_.userInfo_.uuid_;
        matchTeamData.Rank = _pktInfo.enemyInfo_.userInfo_.rank_;

        int userGradeID = GameInfo.Instance.UserBattleData.Now_GradeId;

        GameTable.ArenaGrade.Param gradeParam = GameInfo.Instance.GameTable.FindArenaGrade(x => x.GradeID == userGradeID);
        if (gradeParam == null)
        {
            Log.Show("Arena Grade Data is NULL", Log.ColorType.Red);
            return;
        }

        GameClientTable.NPCCharRnd.Param npcRnd = GameInfo.Instance.GameClientTable.FindNPCCharRnd(x => x.Grade == gradeParam.Grade && x.Tier == gradeParam.Tier);
        if (npcRnd == null)
        {
            Log.Show("Arena Enemy NPCCharRnd Data is NULL");
            return;
        }

        //NPC테이블 데이터로 셋팅
        matchTeamData.Grade = npcRnd.Grade;
        matchTeamData.Tier = npcRnd.Tier;
        matchTeamData.UserLv = GameSupport.GetNPCCharCndSplitRandomValue(npcRnd.UserRank);
        matchTeamData.SetUserNickName( string.Format("{0}{1}", FLocalizeString.Instance.GetText(1498), GameSupport.GetNPCCharCndSplitRandomValue(npcRnd.NickName)) );
        matchTeamData.Score = GameSupport.GetNPCCharCndSplitRandomValue(npcRnd.Score);

        string userMarks = npcRnd.MarkID.Replace(" ", "");
		string[] userMarkArray = Utility.Split(userMarks, ','); //userMarks.Split(',');

		if (userMarkArray.Length == 1)
            matchTeamData.UserMark = int.Parse(userMarks);
        else
        {
            matchTeamData.UserMark = int.Parse(userMarkArray[UnityEngine.Random.Range(0, userMarkArray.Length)]);
        }

        List<int> listCharId = new List<int>();
        List<GameTable.Character.Param> listAllCharacter = new List<GameTable.Character.Param>();
        listAllCharacter.AddRange(GameInfo.Instance.GameTable.Characters);

        //CharTableID 처음에 0으로 셋팅
        for (int i = 0; i < (int)eArenaTeamSlotPos._MAX_; i++)
            listCharId.Add(0);

        for (int i = 0; i < npcRnd.CharCnt; i++)
        {
            int randIndex = UnityEngine.Random.Range(0, listAllCharacter.Count);
            //listCharId.Add(listAllCharacter[randIndex].ID);
            listCharId[(int)eArenaTeamSlotPos.LAST_POS - i] = listAllCharacter[randIndex].ID;

            listAllCharacter.RemoveAt(randIndex);
        }

        for (int i = 0; i < (int)eArenaTeamSlotPos._MAX_; i++)
        {
            if (listCharId[i] <= 0)
                matchTeamData.charlist.Add(null);
            else
            {
                matchTeamData.charlist.Add(GameSupport.CreateArenaEnemyTeamCharData(listCharId[i], npcRnd));
            }
        }

        _matchteam = matchTeamData;

        //문양
        _matchteam.badgelist = GameSupport.CreateArenaDummyBadgeList(npcRnd);

        _matchteam.TeamPower = GameSupport.GetArenaEnemyTeamPower();
    }

    public void SetPktInfoMonthlyFee(PktInfoMonthlyFee pktInfo)
    {
        _userMonthlyData.SetUserMonthlyData(pktInfo);
    }

    public void UpdatePktInfoMonthlyFee(PktInfoMonthlyFee pktInfo)
    {
        _userMonthlyData.UpdateUserMonthlyData(pktInfo);
    }

    public void SetPktInfoBuffEffect(PktInfoEffect pktInfo)
    {
        _userBuffEffectData.SetUserBuffEffectData(pktInfo);
    }

    public void UpdatePktInfoBuffEffect(PktInfoEffect pktInfo)
    {
        _userBuffEffectData.UpdateUserBuffEffectData(pktInfo);
    }

    private List<Dispatch> _DispatchList = new List<Dispatch>();
    public List<Dispatch> Dispatches { get { return _DispatchList; } }

    public void SetPktDispatch(PktInfoDispatch dispatches)
    {
        for (int i = 0; i < dispatches.infos_.Count; i++)
        {
            var findData = _DispatchList.Find(x => x.TableID == (int)dispatches.infos_[i].tableID_);
            if (findData == null)
            {
                // 신규 데이터
                _DispatchList.Add(new Dispatch(dispatches.infos_[i]));
            }
            else
            {
                // 교체 데이터
                findData.SetDispatchData(dispatches.infos_[i]);
            }
        }

    }
    public void SetPktDispatch(PktInfoDispatchOperConfirmAck _pktInfo)
    {
        if (_pktInfo == null) return;

        SetPktDispatch(_pktInfo.products_);
        SetPktDispatch(_pktInfo.outCards_);
        SetPktDispatch(_pktInfo.opers_);

        ApplyPktInfoMaterItem( _pktInfo.consumeItem_ );
    }

    private void SetPktDispatch(PktInfoContentsSlotPos cards_)
    {
        if (cards_ == null || cards_.infos_ == null || cards_.infos_.Count == 0)
            return;

        for (int i = 0; i < cards_.infos_.Count; i++)
        {
            var card = _cardlist.Find(x => x.CardUID == (long)cards_.infos_[i].uid_);
            if (card == null) continue;

            card.SetPos((long)cards_.infos_[i].value_, (int)cards_.infos_[i].kind_, cards_.infos_[i].slotNum_);
        }
    }

    private void SetPktDispatch(PktInfoUIDList cards_)
    {
        if (cards_ == null || cards_.uids_ == null || cards_.uids_.Count == 0)
            return;

        for (int i = 0; i < cards_.uids_.Count; i++)
        {
            var card = _cardlist.Find(x => x.CardUID == (long)cards_.uids_[i]);
            if (card == null) continue;

            card.InitPos();
        }
    }

    private void SetPktDispatch(PktInfoProductPack products_)
    {
        if (products_ == null) return;
        _pktproduct = null;
        _pktproduct = products_;
        if (_pktproduct != null)
        {
            ApplyProduct(_pktproduct, false);
        }
    }

    public bool IsUsingDispatch(CardData data)
    {
        if (data == null) return false;
        return data.PosKind == (int)eContentsPosKind.DISPATCH;
    }

    public Dispatch GetUsingDispatchData(long carduid)
    {
        if (_DispatchList == null || _DispatchList.Count == 0)
            return null;

        for (int i = 0; i < _DispatchList.Count; i++)
        {
            for (int j = 0; j < _DispatchList[i].UsingCardData.Count; j++)
            {
                if (_DispatchList[i].UsingCardData[j].CardUID == carduid)
                    return _DispatchList[i];
            }
        }
        return null;
    }

    public enum eCommunityUserInfoGetType
    {
        NONE, ARENA, ARENATOWER,
    };
    public eCommunityUserInfoGetType CommunityUserInfoGetType = eCommunityUserInfoGetType.NONE;
    public void SetPktArenaTowerData(PktInfoUserArenaTower arena)
    {
        //초기화
        TowerFriendTeamData.Clear();
        _towercharlist.Clear();
        for (int i = (int)eArenaTeamSlotPos.START_POS; i < (int)eArenaTeamSlotPos._MAX_; i++)
        {
            _towercharlist.Add((long)arena.team_.CUIDs_[i]);
        }
        _towerClearID = (int)arena.info_.claerID_;
        _userdata.ArenaTowerCardFormationID = (int)arena.team_.cardFrmtID_;
    }

    public void SetPktInfoUserArenaTowerTeam(PktInfoUserArenaTeam _pktInfo)
    {
        _towercharlist.Clear();        
        for (int i = (int)eArenaTeamSlotPos.START_POS; i < (int)eArenaTeamSlotPos._MAX_; i++)
        {
            _towercharlist.Add((long)_pktInfo.CUIDs_[i]);
        }
        _userdata.ArenaTowerCardFormationID = (int)_pktInfo.cardFrmtID_;
    }

    public void GetArenaTowerCharList(ref List<CharData> list, bool withoutFriendChar = false, bool onlyFriendChar = false)
    {
        if (list == null) list = new List<CharData>();
        list.Clear();

        if (!onlyFriendChar)
        {
            list.Add(GetMainChar());
            for (int i = 0; i < _charlist.Count; i++)
                if (_charlist[i].CUID != _userdata.MainCharUID)
                    list.Add(_charlist[i]);
        }

        if (!withoutFriendChar)
        {
            var iterA = TowerFriendTeamData.GetEnumerator();
            while (iterA.MoveNext())
            {
                var iterB = iterA.Current.charlist.GetEnumerator();
                while (iterB.MoveNext())
                {
                    list.Add(iterB.Current.CharData);
                }
            }
        }
    }

    public CharData GetArenaTowerCharData(long uid)
    {
        if (uid <= 0) 
            return null;

        CharData result = GetCharData(uid);
        if (result != null)
            return result;


        var iterA = TowerFriendTeamData.GetEnumerator();
        while (iterA.MoveNext())
        {
            var iterB = iterA.Current.charlist.GetEnumerator();
            while (iterB.MoveNext())
            {
                if (iterB.Current.CharData.CUID == uid)
                    return iterB.Current.CharData;
            }
        }

        return null;
    }

    public WeaponData GetArenaTowerWeaponData(CharData charData)
    {
        WeaponData result = GetWeaponData(charData.EquipWeaponUID);
        if (result != null)
            return result;

        var iterA = TowerFriendTeamData.GetEnumerator();
        while (iterA.MoveNext())
        {
            var iterB = iterA.Current.charlist.GetEnumerator();
            while (iterB.MoveNext())
            {
                if (iterB.Current.CharData.CUID == charData.CUID)
                    return iterB.Current.MainWeaponData;
            }
        }

        return result;
    }

    public GemData GetArenaTowerGemData(CharData charData, int slotIndex)
    {
        WeaponData weapon = GetArenaTowerWeaponData(charData);
        if (weapon == null)
            return null;

        GemData result = GetGemData(weapon.SlotGemUID[slotIndex]);
        if (result != null)
            return result;

        var iterA = TowerFriendTeamData.GetEnumerator();
        while (iterA.MoveNext())
        {
            var iterB = iterA.Current.charlist.GetEnumerator();
            while (iterB.MoveNext())
            {
                if (iterB.Current.CharData.CUID == charData.CUID)
                {
                    var iterC = iterB.Current.MainGemList.GetEnumerator();
                    while(iterC.MoveNext())
                    {
                        if (iterC.Current.GemUID == weapon.SlotGemUID[slotIndex])
                            return iterC.Current;
                    }
                }
            }
        }
        return result;
    }

    public void ClearArenaTower()
    {
        ++_towerClearID;
    }

    public void SetCardFormationData(int tid)
    {
        SelectedCardFormationTID = tid;
        CardFormationTableData = GameTable.FindCardFormation(tid);
    }

    public void SetPktAwakenSkillData(PktInfoUserSkill pktAwakenSkill)
    {
        if(pktAwakenSkill == null || pktAwakenSkill.infos_ == null)
        {
            return;
        }

        _userdata.ListAwakenSkillData.Clear();

        for (int i = 0; i < pktAwakenSkill.infos_.Count; i++)
        {
            if(pktAwakenSkill.infos_[i].lv_ <= 0) // 레벨이 0일땐 없는 것과 같음
            {
                continue;
            }

            AwakenSkillInfo info = new AwakenSkillInfo();
            info.SetPktData((int)pktAwakenSkill.infos_[i].tableID_, (int)pktAwakenSkill.infos_[i].lv_);

            _userdata.ListAwakenSkillData.Add(info);
        }
    }

    public bool IsShowServerRelocateNotice = false;
    public bool IsServerRelocate = false;
    public string ServerRelocateID = string.Empty;
    public bool IsServerRelocateComplete = false;
    public void SetPktInfoRelocateUserInfo(PktInfoRelocateUser relocate_)
    {
        IsServerRelocate = false;
        IsServerRelocateComplete = false;
        ServerRelocateID = string.Empty;

        if (relocate_ == null) return;

        if (relocate_.accountID_ == null || relocate_.id_ == null || relocate_.pw_ == null)
            return;

        if (string.IsNullOrEmpty(relocate_.id_.str_) || string.IsNullOrEmpty(relocate_.pw_.str_))
            return;

        if (relocate_.uuid_ == 0)
            return;

        IsServerRelocate = true;
        IsServerRelocateComplete = relocate_.complete_ == 0 ? false : true;
        ServerRelocateID = relocate_.id_.str_;
    }

    public long[] TradeMaterialUIDList = new long[4];
    public void SetTradeMaterialUID(long[] uids)
    {
        if (uids == null) return;

        for (int i = 0; i < uids.Length; i++)
        {
            if (i >= TradeMaterialUIDList.Length)
                break;
            TradeMaterialUIDList[i] = uids[i];
        }
    }
    public void ApplyTradeMaterialUID(int selectIndex, long uid)
    {
        if (selectIndex >= TradeMaterialUIDList.Length) return;        

        if (uid <= 0 || TradeMaterialUIDList[selectIndex] == uid)
        {
            //remove
            TradeMaterialUIDList[selectIndex] = 0;
            return;
        }

        //uid와 같은 데이터가 있는지 조사
        int equalIndex = -1;
        for (int i = 0; i < TradeMaterialUIDList.Length; i++)
        {
            if(TradeMaterialUIDList[i] == uid)
            {
                equalIndex = i;
                break;
            }
        }

        if (equalIndex >= 0)
        {
            //swap
            long tmp = TradeMaterialUIDList[selectIndex];
            TradeMaterialUIDList[selectIndex] = uid;
            TradeMaterialUIDList[equalIndex] = tmp;
        }
        else
        {
            //replace
            TradeMaterialUIDList[selectIndex] = uid;
        }

    }
    public void ClearTradeMaterialUID()
    {
        for (int i = 0; i < TradeMaterialUIDList.Length; i++)
        {
            TradeMaterialUIDList[i] = 0;
        }
    }

    public bool IsEmptyTradeMaterialUID()
    {
        for (int i = 0; i < TradeMaterialUIDList.Length; i++)
        {
            if (TradeMaterialUIDList[i] > 0) return false;
        }
        return true;
    }

    public List<ulong> GetListTradeMaterialUID()
    {
        List<ulong> result = new List<ulong>();

        for (int i = 0; i < TradeMaterialUIDList.Length; i++)
        {
            if (TradeMaterialUIDList[i] <= 0) continue;
            result.Add((ulong)TradeMaterialUIDList[i]);
        }

        return result;
    }

    private void InitNetworkTime(string ntpServer, bool reconnect = false)
    {
        AddressFamily addressFamily = AddressFamily.InterNetwork;
        SocketType socketType = SocketType.Dgram;
        ProtocolType protocolType = ProtocolType.Udp;

        if (!reconnect)
        {
			try
			{
				IPAddress[] address = Dns.GetHostEntry(ntpServer).AddressList;
				if (address == null || address.Length == 0)
				{
					return;
				}

				IPAddress ip = null;
				for (int i = 0; i < address.Length; i++)
				{
					if (address[i].AddressFamily == AddressFamily.InterNetwork)
					{
						ip = address[i];
						break;
					}
				}

				if (ip == null)
				{
					ip = address[0];

					addressFamily = AddressFamily.InterNetworkV6;
					socketType = SocketType.Stream;
					protocolType = ProtocolType.Tcp;
				}

				mIPEndPoint = new IPEndPoint(ip, 123);
				if (mIPEndPoint == null)
				{
					return;
				}
			}
			catch(SocketException excep)
			{
				mSocket = null;
				Debug.LogError(excep.ErrorCode);

				return;
			}
        }

        mSocket = new Socket(addressFamily, socketType, protocolType);
        try
        {
            mSocket.Connect(mIPEndPoint);
        }
        catch(SocketException excep)
        {
            mSocket = null;
            Debug.LogError(excep.ErrorCode);
        }
    }

    /// <summary>
    /// Gets the current DateTime form <paramref name="ep"/> IPEndPoint.
    /// </summary>
    /// <param name="ep">The IPEndPoint to connect to.</param>
    /// <returns>A DateTime containing the current time.</returns>
    private byte[] mNtpData = new byte[48];
    public DateTime GetNetworkTime()
    {
        if (mSocket == null || mIPEndPoint == null)
        {
            return GameSupport.GetCurrentServerTime();
        }

        if(!mSocket.Connected)
        {
            try
            {
                mSocket.Connect(mIPEndPoint);
            }
            catch (SocketException e)
            {
                return GameSupport.GetCurrentServerTime();
            }
        }

        //byte[] ntpData = new byte[48]; // RFC 2030 
        Array.Clear(mNtpData, 0, mNtpData.Length);
        mNtpData[0] = 0x1B;
        for (int i = 1; i < 48; i++)
            mNtpData[i] = 0;

        try
        {
            mSocket.Send(mNtpData);

			if (!mSocket.Poll(0, SelectMode.SelectRead))
			{
				return GameSupport.GetCurrentServerTime();
			}

			int receiveSize = mSocket.Receive(mNtpData);
			if (receiveSize <= 0)
			{
				return GameSupport.GetCurrentServerTime();
			}
		}
        catch(SocketException e)
        {
            return GameSupport.GetCurrentServerTime();
        }

        byte offsetTransmitTime = 40;
        ulong intpart = 0;
        ulong fractpart = 0;

        for (int i = 0; i <= 3; i++)
            intpart = 256 * intpart + mNtpData[offsetTransmitTime + i];

        for (int i = 4; i <= 7; i++)
            fractpart = 256 * fractpart + mNtpData[offsetTransmitTime + i];

        ulong milliseconds = (intpart * 1000 + (fractpart * 1000) / 0x100000000L);

        TimeSpan timeSpan = TimeSpan.FromTicks((long)milliseconds * TimeSpan.TicksPerMillisecond);

        DateTime dateTime = new DateTime(1900, 1, 1);
        dateTime += timeSpan;

        return dateTime.Add(TimeZoneInfo.Local.BaseUtcOffset); //(dateTime + new TimeSpan(0, 0, _serverdata.ServerTimeGap));
    }

    public void SetPktServerRotaionGacha (PktInfoComTimeAndTID _pkt)
    {
        _ServerRotationGachaData.SetPktData(_pkt);
    }

    public void SetPktUserRotaionGacha(PktInfoComTimeAndTID _pkt)
    {
        _UserRotationGachaData.SetPktData(_pkt);
    }

    public void SetPktInfoUnexpectedPackage(PktInfoUnexpectedPackage _pkt)
    {
        if (_pkt == null)
        {
            return;
        }
        
        _unexpectedPackageDataDict.Clear();
        
        foreach (PktInfoUnexpectedPackage.Piece info in _pkt.infos_)
        {
            GameTable.UnexpectedPackage.Param tableData = GameTable.FindUnexpectedPackage((int) info.tableID_);
            if (tableData == null)
            {
                continue;
            }
            
            if (_unexpectedPackageDataDict.ContainsKey(tableData.UnexpectedType))
            {
                UnexpectedPackageData data = _unexpectedPackageDataDict[tableData.UnexpectedType].Find(x => x.TableId == info.tableID_);
                if (data == null)
                {
                    _unexpectedPackageDataDict[tableData.UnexpectedType].Add(new UnexpectedPackageData(info));
                }
                else
                {
                    data.UpdateData(info);
                }
            }
            else
            {
                _unexpectedPackageDataDict.Add(tableData.UnexpectedType, new List<UnexpectedPackageData>(){ new UnexpectedPackageData(info) });
            }
        }
        
        // Sort
        foreach (KeyValuePair<int,List<UnexpectedPackageData>> pair in _unexpectedPackageDataDict)
        {
            pair.Value.Sort((x, y) => x.EndTime < y.EndTime ? -1 : 1);
        }
    }

    public bool IsNewYouTubeLink()
    {
        if(AppMgr.Instance.configData.m_ServiceType != Config.eServiceType.Global)
        {
            return false;
        }

        if (!PlayerPrefs.HasKey("YOUTUBE_LINK"))
        {
            return true;
        }
        else
        {
            string link = PlayerPrefs.GetString("YOUTUBE_LINK");
            if(string.IsNullOrEmpty(link))
            {
                return false;
            }
            else if(link != YouTubeLink)
            {
                return true;
            }
        }

        return false;
    }

    public void SetPktLoginCommonInfo(PktInfoLoginCommon _pkt)
    {
        if (null == _pkt)
            return;

        if (_pkt.verNum_ <= (int)eCOUNT.NONE)
            return;

        _serverdata.ServerTimeGap = -(_pkt.gmTimeGap_);
        _serverdata.LoginTime = _pkt.svrTime_.GetTime();
        _serverdata.ArenaSeasonEndTime = GameSupport.GetLocalTimeByServerTime(_pkt.arenaSeasonTime_.endTime_.GetTime());
        _serverdata.ArenaNextSeasonStartTime = GameSupport.GetLocalTimeByServerTime(_pkt.arenaSeasonTime_.nextStartTime_.GetTime());

        _serverdata.RaidSeasonEndTime = GameSupport.GetLocalTimeByServerTime( _pkt.raidSeasonTime_.endTime_.GetTime() );
        _serverdata.RaidCurrentSeason = (int)_pkt.openRaidTypeValue_;

        _serverdata.SetPktSecretQuestData(_pkt.sqOpt_);

        SetPktInfoUserReflashData(_pkt.reflash_);
        SetPktServerRotaionGacha(_pkt.rgacha_);
    }

	public bool IsNewUser()
	{
        if( DailyMissionData == null || DailyMissionData.Infos == null ) {
            return false;
		}

		bool findNewUser = false;
		DateTime startTime = DateTime.Now;

		int latestDailyMissionSetIdForNewUser = 0;
		for (int i = DailyMissionData.Infos.Count - 1; i >= 0; --i)
		{
			GameTable.DailyMissionSet.Param param = GameTable.FindDailyMissionSet((int)DailyMissionData.Infos[i].GroupID);
			if(param.EventTarget == 1)
			{
				latestDailyMissionSetIdForNewUser = (int)DailyMissionData.Infos[i].GroupID;
				break;
			}
		}

		/*
		for(int i = GameTable.DailyMissionSets.Count - 1; i >= 0; --i)
		{
			if(GameTable.DailyMissionSets[i].EventTarget == 1)
			{
				latestDailyMissionSetIdForNewUser = GameTable.DailyMissionSets[i].ID;
				break;
			}
		}
		*/

		//for(int i = DailyMissionData.Infos.Count - 1; i >= 0; --i)
		for (int i = 0; i < DailyMissionData.Infos.Count; ++i)
		{
			GameTable.DailyMissionSet.Param param = GameTable.FindDailyMissionSet((int)DailyMissionData.Infos[i].GroupID);
			if(param == null || param.ID != latestDailyMissionSetIdForNewUser)
			{
				continue;
			}

			startTime = DailyMissionData.Infos[i].StartTime;
			findNewUser = true;

			break;
		}

		if(!findNewUser)
		{
			return false;
		}

		TimeSpan remainTime = GameSupport.GetRemainTime(startTime.AddDays(GameConfig.NewUserTerm), DateTime.UtcNow.AddSeconds(ServerData.ServerTimeGap));
		if(remainTime.Ticks < 0)
		{
			return false;
		}

		return true;
	}

	public bool IsComebackUser()
	{
        if( DailyMissionData == null || DailyMissionData.Infos == null ) {
            return false;
        }

        bool findComebackUser = false;
		DateTime startTime = DateTime.Now;

		int latestDailyMissionSetIdForCombackUser = 0;
		for (int i = DailyMissionData.Infos.Count - 1; i >= 0; --i)
		{
			GameTable.DailyMissionSet.Param param = GameTable.FindDailyMissionSet((int)DailyMissionData.Infos[i].GroupID);
			if (param.EventTarget == 2)
			{
				latestDailyMissionSetIdForCombackUser = (int)DailyMissionData.Infos[i].GroupID;
				break;
			}
		}

		/*
		for (int i = GameTable.DailyMissionSets.Count - 1; i >= 0; --i)
		{
			if (GameTable.DailyMissionSets[i].EventTarget == 2)
			{
				latestDailyMissionSetIdForCombackUser = GameTable.DailyMissionSets[i].ID;
				break;
			}
		}
		*/

		//for (int i = DailyMissionData.Infos.Count - 1; i >= 0; --i)
		for (int i = 0; i < DailyMissionData.Infos.Count; ++i)
		{
			GameTable.DailyMissionSet.Param param = GameTable.FindDailyMissionSet((int)DailyMissionData.Infos[i].GroupID);
			if (param == null || param.ID != latestDailyMissionSetIdForCombackUser)
			{
				continue;
			}

			startTime = DailyMissionData.Infos[i].StartTime;
			findComebackUser = true;

			break;
		}

		if (!findComebackUser)
		{
			return false;
		}

		TimeSpan remainTime = GameSupport.GetRemainTime(startTime.AddDays(GameConfig.ReturnUserTerm), DateTime.UtcNow.AddSeconds(ServerData.ServerTimeGap));
		if (remainTime.Ticks < 0)
		{
			return false;
		}

		return true;
	}

    public PresetData SetPresetData(ePresetKind presetKind, int slotIndex, int maxCount, int charId = 0)
    {
        PresetData[] presetDatas = new PresetData[maxCount];
        for (int i = 0; i < presetDatas.Length; i++)
        {
            presetDatas[i] = new PresetData();
        }

        switch (presetKind)
        {
            case ePresetKind.CHAR:
                {
                    _charPresetDataDict.Add(charId, presetDatas);
                } break;
            case ePresetKind.STAGE:
                {
                    _questPresetDatas = presetDatas;
                } break;
            case ePresetKind.ARENA:
                {
                    _arenaPresetDatas = presetDatas;
                } break;
            case ePresetKind.ARENA_TOWER:
                {
                    _arenaTowerPresetDatas = presetDatas;
                } break;
            case ePresetKind.RAID:
                {
                    _raidPresetDatas = presetDatas;
                } break;
        }

        if (slotIndex < 0 || presetDatas.Length <= slotIndex)
        {
            return null;
        }

        return presetDatas[slotIndex];
    }

    public void ApplyPresetDataInCharData(PktInfoUserPreset.Piece piece)
    {
        switch (piece.kind_)
        {
            case ePresetKind.ARENA:
                {
                    _userdata.ArenaCardFormationID = (int)piece.cardFrmtID_;
                } break;
            case ePresetKind.ARENA_TOWER:
                {
                    _userdata.ArenaTowerCardFormationID = (int)piece.cardFrmtID_;
                } break;
            case ePresetKind.STAGE:
                {
                    _userdata.CardFormationID = (int)piece.cardFrmtID_;
                } break;
            case ePresetKind.RAID:
                {
                    RaidUserData.CardFormationId = (int)piece.cardFrmtID_;
                } break;
        }

        foreach (PktInfoUserPreset.CharData charData in piece.characters_)
        {
            CharData originCharData = GetCharData((long)charData.cuid_);
            if (originCharData == null)
            {
                continue;
            }

            if (piece.kind_ == ePresetKind.ARENA_TOWER)
            {
                if (ArenaTowerFriendContainer.ContainsKey(charData.posValue_))
                {
                    ArenaTowerFriendContainer.Remove(charData.posValue_);
                }
            }

            originCharData.EquipCostumeID = (int)charData.costumeID_;
            originCharData.CostumeColor = (int)charData.costumeClr_;

            uint flag = charData.skinStateFlag_;
            GameTable.Costume.Param costumeParam = GameTable.FindCostume(originCharData.EquipCostumeID);
            if (costumeParam != null && costumeParam.SubHairChange == 1)
            {
                bool isOn = GameSupport._IsOnBitIdx(flag, (int)eCostumeStateFlag.CSF_HAIR);
                GameSupport._DoOnOffBitIdx(ref flag, (int)eCostumeStateFlag.CSF_HAIR, !isOn);
            }
            originCharData.CostumeStateFlag = (int)flag;

            for (int i = 0; i < charData.wpn_.Length; i++)
            {
                PktInfoUserPreset.CharData.WpnInfo weaponInfo = charData.wpn_[i];
                if (weaponInfo == null)
                {
                    continue;
                }

                if (i == (int)eWeaponSlot.MAIN)
                {
                    originCharData.EquipWeaponUID = (long)weaponInfo.wpn_.wpnUID_;
                    originCharData.EquipWeaponSkinTID = (int)weaponInfo.wpn_.wpnSkin_;
                }
                else
                {
                    originCharData.EquipWeapon2UID = (long)weaponInfo.wpn_.wpnUID_;
                    originCharData.EquipWeapon2SkinTID = (int)weaponInfo.wpn_.wpnSkin_;
                }

                WeaponData weaponData = GetWeaponData((long)weaponInfo.wpn_.wpnUID_);
                if (weaponData == null)
                {
                    continue;
                }

                for (int g = 0; g < weaponData.SlotGemUID.Length; g++)
                {
                    long gemUid = 0;
                    if (g < weaponInfo.gems_.Length)
                    {
                        gemUid = (long)weaponInfo.gems_[g].gemUID_;
                    }
                    weaponData.SlotGemUID[g] = gemUid;
                }
            }

            for (int i = 0; i < charData.cards_.uids_.Count; i++)
            {
                long cardUid = (long)charData.cards_.uids_[i];
                CardData cardData = GetCardData(cardUid);
                if (cardData == null)
                {
                    continue;
                }

                CharData clearData = GetCharData(cardData.PosValue);
                if (clearData != null)
                {
                    for (int c = 0; c < clearData.EquipCard.Length; c++)
                    {
                        if (clearData.EquipCard[c] == cardUid)
                        {
                            clearData.EquipCard[c] = 0;
                            break;
                        }
                    }
                }

                cardData.PosKind = (int)eContentsPosKind.CHAR;
                cardData.PosValue = originCharData.CUID;
                cardData.PosSlot = i;
            }

            for (int i = 0; i < originCharData.EquipCard.Length; i++)
            {
                long uid = 0;
                if (i < charData.cards_.uids_.Count)
                {
                    uid = (long)charData.cards_.uids_[i];
                }
                originCharData.EquipCard[i] = uid;
            }

            for (int i = 0; i < originCharData.EquipSkill.Length; i++)
            {
                int skillId = 0;
                if (i < charData.skillIds_.Length)
                {
                    skillId = (int)charData.skillIds_[i];
                }
                originCharData.EquipSkill[i] = skillId;
            }
        }

        for (int i = 0; i < piece.badgeUids_.uids_.Count; i++)
        {
            BadgeData badgeData = GetBadgeData((long)piece.badgeUids_.uids_[i]);
            if (badgeData == null)
            {
                continue;
            }

            badgeData.PosSlotNum = i;

            switch (piece.kind_)
            {
                case ePresetKind.ARENA:
                    {
                        badgeData.PosKind = (int)eContentsPosKind.ARENA;
                    }
                    break;
                case ePresetKind.ARENA_TOWER:
                    {
                        badgeData.PosKind = (int)eContentsPosKind.ARENA_TOWER;
                    }
                    break;
            }
        }
    }

    public void ApplyPresetData(PktInfoUserPreset.Piece piece)
    {
        PresetData presetData = null;
        int slotIndex = piece.slotNum_ - 1;

        switch(piece.kind_)
        {
            case ePresetKind.CHAR:
                {
                    CharData charData = _charlist.Find(x => x.CUID == (long)piece.cuid_);
                    if (charData != null)
                    {
                        if (_charPresetDataDict.ContainsKey(charData.TableID))
                        {
                            if (slotIndex < _charPresetDataDict[charData.TableID].Length)
                            {
                                presetData = _charPresetDataDict[charData.TableID][slotIndex];
                            }
                        }
                        else
                        {
                            presetData = SetPresetData(piece.kind_, slotIndex, GameInfo.Instance.GameConfig.CharPresetSlot, charData.TableID);
                        }
                    }
                } break;
            case ePresetKind.STAGE:
                {
                    if (_questPresetDatas != null)
                    {
                        if (slotIndex < _questPresetDatas.Length)
                        {
                            presetData = _questPresetDatas[slotIndex];
                        }
                    }
                    else
                    {
                        presetData = SetPresetData(piece.kind_, slotIndex, GameInfo.Instance.GameConfig.ContPresetSlot);
                    }
                } break;
            case ePresetKind.ARENA:
                {
                    if (_arenaPresetDatas != null)
                    {
                        if (slotIndex < _arenaPresetDatas.Length)
                        {
                            presetData = _arenaPresetDatas[slotIndex];
                        }
                    }
                    else
                    {
                        presetData = SetPresetData(piece.kind_, slotIndex, GameInfo.Instance.GameConfig.ContPresetSlot);
                    }
                } break;
            case ePresetKind.ARENA_TOWER:
                {
                    if (_arenaTowerPresetDatas != null)
                    {
                        if (slotIndex < _arenaTowerPresetDatas.Length)
                        {
                            presetData = _arenaTowerPresetDatas[slotIndex];
                        }
                    }
                    else
                    {
                        presetData = SetPresetData(piece.kind_, slotIndex, GameInfo.Instance.GameConfig.ContPresetSlot);
                    }
                } break;
            case ePresetKind.RAID:
                {
                    if (_raidPresetDatas != null)
                    {
                        if (slotIndex < _raidPresetDatas.Length)
                        {
                            presetData = _raidPresetDatas[slotIndex];
                        }
                    }
                    else
                    {
                        presetData = SetPresetData(piece.kind_, slotIndex, _gameconfig.ContPresetSlot);
                    }
                } break;
        }

        if (presetData == null)
        {
            return;
        }

        ApplyPresetDetailData(ref presetData, piece);
    }

    private void ApplyPresetDetailData(ref PresetData presetData, PktInfoUserPreset.Piece piece)
    {
        presetData.PresetName = piece.name_.str_;
        presetData.CardTeamId = (int)piece.cardFrmtID_;

        foreach (PktInfoUserPreset.CharData charData in piece.characters_)
        {
            PresetCharData presetCharData = presetData.CharDatas[charData.posValue_];
            presetCharData.CharUid = (long)charData.cuid_;
            presetCharData.CostumeId = (int)charData.costumeID_;
            presetCharData.CostumeColor = charData.costumeClr_;
            presetCharData.CostumeStateFlag = (int)charData.skinStateFlag_;

            for (int i = 0; i < charData.wpn_.Length; i++)
            {
                PktInfoUserPreset.CharData.WpnInfo wpnInfo = charData.wpn_[i];

                presetCharData.WeaponDatas[i].Uid = (long)wpnInfo.wpn_.wpnUID_;
                presetCharData.WeaponDatas[i].SkinTid = (int)wpnInfo.wpn_.wpnSkin_;
                for (int g = 0; g < wpnInfo.gems_.Length; g++)
                {
                    presetCharData.WeaponDatas[i].GemUids[g] = (long)wpnInfo.gems_[g].gemUID_;
                }
            }

            for (int i = 0; i < charData.cards_.uids_.Count; i++)
            {
                presetCharData.CardUids[i] = (long)charData.cards_.uids_[i];
            }

            for (int i = 0; i < charData.skillIds_.Length; i++)
            {
                presetCharData.SkillIds[i] = (int)charData.skillIds_[i];
            }
        }

        for (int i = 0; i < piece.badgeUids_.uids_.Count; i++)
        {
            presetData.BadgeUdis[i] = (long)piece.badgeUids_.uids_[i];
        }
    }

    public void ApplyPktInitRaidSeasonData( PktInfoInitRaidSeasonData pkt ) {
        RaidUserData.LastPlayedSeasonEndTime = GameSupport.GetLocalTimeByServerTime( pkt.lastPlaySeasonEndTime_.GetTime() );
        RaidUserData.SetCurStep( pkt.bestOpenRaidLevel_ );
	}

    public void ApplyPktInfoRaidRankStageList( PktInfoRaidRankStageList _pktInfo ) {
        for( int i = 0; i < _pktInfo.infos_.Count; i++ ) {
            PktInfoRaidRankStage stage = _pktInfo.infos_[i];

            RaidRankData data = RaidRankDataList.Find( x => x.StageTableID == (int)stage.header_.stageID_ && x.Step == (int)stage.header_.raidLevel_ );
            if( data == null ) {
                data = new RaidRankData( stage.header_ );
                RaidRankDataList.Add( data );
            }
            else {
                data.SetPktData( stage.header_ );
            }

            data.RankUserList.Clear();

            for( int j = 0; j < stage.infos_.Count; j++ ) {
                if( stage.infos_[j].uuid_ > 0 ) {
                    TimeAttackRankUserData userData = new TimeAttackRankUserData();
                    userData.SetPktData( stage.infos_[j], j + 1, false );

                    data.RankUserList.Add( userData );
                }
			}
		}
    }

    public void ApplyPktInfoRaidFirstRankStageList( PktInfoRaidRankStageList _pktInfo ) {
        for( int i = 0; i < _pktInfo.infos_.Count; i++ ) {
            PktInfoRaidRankStage stage = _pktInfo.infos_[i];
            if( stage.infos_.Count <= 0 || stage.infos_[0].uuid_ <= 0 ) {
                continue;
			}

            RaidRankData data = RaidRankDataList.Find( x => x.StageTableID == (int)stage.header_.stageID_ && x.Step == (int)stage.header_.raidLevel_ );
            if( data == null ) {
                continue;
			}

            bool skip = false;

            for( int j = 0; j < stage.infos_.Count; j++ ) {
                TimeAttackRankUserData find = data.RankUserList.Find( x => x.UUID == (long)stage.infos_[j].uuid_ );
                if( find != null && find.IsRaidFirstRanker ) {
                    skip = true;
                    break;
				}
            }

            if( skip ) {
                continue;
			}

            TimeAttackRankUserData userData = new TimeAttackRankUserData();
			userData.SetPktData( stage.infos_[0], 0, true );

			data.RankUserList.Insert( 0, userData );
		}
    }

    public void ApplyPktInfoRaidRankerDetailAck( PktInfoRaidRankerDetailAck pkt, bool isFirstRaidRankUser ) {
        TimeAttackRankUserData data = GetRaidRankUserDataOrNull( (int)pkt.raidLevel_, (long)pkt.simpleInfo_.uuid_, isFirstRaidRankUser );
        if( data == null ) {
            return;
        }

        data.SetPktData( pkt );
    }

    public void ApplyPktRaidClearList( PktInfoRaidStageRec pkt ) {
        foreach (PktInfoRaidStageRec.Piece info in pkt.infos_)
        {
            RaidClearData raidClearData = RaidUserData.StageClearList.Find(x => x.Step == info.level_ && x.StageTableID == info.stageTID_);
            if (raidClearData == null)
            {
                RaidUserData.StageClearList.Add(new RaidClearData(info));
            }
            else
            {
                raidClearData.SetPktData(info);
            }
        }
    }

    public RaidClearData GetRaidClearDataOrNull( int step ) {
        GameTable.Stage.Param stageParam = _gametable.FindStage(x => x.StageType == (int)eSTAGETYPE.STAGE_RAID && x.TypeValue == _serverdata.RaidCurrentSeason);
        if (stageParam == null)
        {
            return null;
        }

        List<RaidClearData> clearList = RaidUserData.StageClearList.FindAll(x => x.Step == step && x.StageTableID == stageParam.ID);
        clearList.Sort(delegate (RaidClearData lhs, RaidClearData rhs) {
            if (lhs.HighestScore > rhs.HighestScore)
            {
                return 1;
            }
            else if (lhs.HighestScore < rhs.HighestScore)
            {
                return -1;
            }

            return 0;
        });

        return clearList.FirstOrDefault();
    }

    public RaidRankData GetRaidRankDataOrNull( int step ) {
        GameTable.Stage.Param stageParam = _gametable.FindStage(x => x.StageType == (int)eSTAGETYPE.STAGE_RAID && x.TypeValue == _serverdata.RaidCurrentSeason);
        if (stageParam == null)
        {
            return null;
        }

        return RaidRankDataList.Find(x => x.Step == step && x.StageTableID == stageParam.ID);
    }

    public TimeAttackRankUserData GetRaidRankUserDataOrNull( int step, long userUid, bool findFirstClearUser = false ) {
        RaidRankData rankData = GetRaidRankDataOrNull( step );
        if( rankData != null ) {
            return rankData.GetRankUserDataOrNull( userUid, findFirstClearUser );
		}

        return null;
    }

    public void ResetRaidCharHp() {
        for( int i = 0; i < _charlist.Count; i++ ) {
            _charlist[i].RaidHpPercentage = 100.0f;
		}
	}
}
