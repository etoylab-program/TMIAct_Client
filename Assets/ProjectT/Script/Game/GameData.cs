
using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;


public partial class GachaCategoryData
{
    public DateTime StartDate;	//시작일시
    public DateTime EndDate; 	//종료일시
    public int StoreID1;
    public int StoreID2;
    public string Type;          	//StoreDisplayGoods,GachaTab/Type{string} 일치
    public int Text;           	
    public string UrlBtnImage;   	//탭버튼에 사용할 텍스쳐
    public string UrlBGImage;    	//BG에 사용될 텍스쳐 
    public string UrlAddImage;

    public int StoreID3;
    public int StoreID4;

    public int Name;
    public int Desc;
    public bool[] Localize = new bool[(int)eGachaLocalizeType.End];
    public string Value1;

    public GachaCategoryData()
    {

    }
    public GachaCategoryData( string type, int text, string tab, string bg, int id1, int id2, int id3, int id4, DateTime sd, DateTime se )
    {
        Type = type;
        Text = text;
        StoreID1 = id1;
        StoreID2 = id2;
        UrlBtnImage = tab;
        UrlBGImage = bg;
        StartDate = sd;
        EndDate = se;

        StoreID3 = id3;
        StoreID4 = id4;
    }
}

public partial class StoreSaleData
{
    public int TableID;               // 상점 테이블 ID
    public int DiscountRate;          // 할인률 (1=1%) (할인률이 0이 아닌 경우 제한에 해당해도 할인만 안될 뿐 구매 가능)
    public int LimitType;             // 구조체 바이트라인 (64비트 주소에 맞춥니다.)
    public int LimitValue;            // 제한 타입 값

    public StoreSaleData()
    {

    }
    public StoreSaleData(int tableid, int discountrate, int limittype, int limitvalue  )
    {
        TableID = tableid;
        DiscountRate = discountrate;
        LimitType = limittype;
        LimitValue = limitvalue;       
    }
}

public partial class BannerData
{
    public DateTime StartDate;	//시작일시
    public DateTime EndDate; 	//종료일시
    public int BannerType;      //배너 타입
    public int BannerTypeValue;     //배너 값
    public string UrlImage;
    public string UrlAddImage1;
    public string UrlAddImage2;
    public int Name;
    public int Desc;
    public bool[] Localizes = new bool[(int)eBannerLocalizeType.End];
    public string FunctionType; //eBANNERFUNCTIONTYPE 0 = 없음, 1 = UI패널, 2 = UI팝업, 3 = 웹뷰 링크
    public string FunctionValue1;   //MoveUI을 위한 변수
    public string FunctionValue2;
    public string FunctionValue3;
    public eBannerFuntionValue3Flag TagMark;
    
    public BannerData()
    {

    }
    public BannerData(string url, string functiontype, string functionvalue1, string functionvalue2, string functionvalue3, DateTime sd, DateTime se)
    {
        UrlImage = url;
        FunctionType = functiontype;
        FunctionValue1 = functionvalue1;
        FunctionValue2 = functionvalue2;
        StartDate = sd;
        EndDate = se;
    }
}

public partial class GuerrillaCampData
{
    public DateTime StartDate;	//시작일시
    public DateTime EndDate;    //종료일시
    public int EffectValue;     // 효과 값
    public string Type;         // 타입
    public int Name;
    public int Desc;
    public int Condition;       // 조건 값


    public GuerrillaCampData()
    {

    }

    public GuerrillaCampData(string type, int condition, int efectvalue, int name, int desc, DateTime sd, DateTime se)
    {
        StartDate = sd;
        EndDate = se;
        Condition = condition;
        EffectValue = efectvalue;
        Type = type;
        Name = name;
        Desc = desc;
    }
}
//9998
public partial class GuerrillaMissionData
{
    public DateTime StartDate;	//시작일시
    public DateTime EndDate;    //종료일시
    public string Type;         // 타입
    public int Name;
    public int Desc;
    public int Condition;       // 조건 값
    public int Count;           // 필요 횟수
    public int GroupID;         // 그룹ID
    public int GroupOrder;      // 그룹 내 순서
    public int RewardType;      // 보상 타입
    public int RewardIndex;     // 보상 인덱스
    public int RewardValue;     // 보상 수량


    public GuerrillaMissionData()
    {

    }

    public GuerrillaMissionData(string type, int name, int desc, int cond, int count, int groupid, int grouporder, int rtype, int rindex, int rvalue, DateTime sd, DateTime se)
    {
        StartDate = sd;
        EndDate = se;
        Type = type;
        Name = name;
        Desc = desc;
        Condition = cond;
        Count = count;
        GroupID = groupid;
        GroupOrder = grouporder;
        RewardType = rtype;
        RewardIndex = rindex;
        RewardValue = rvalue;
    }
}

public partial class SecretQuestOptionData
{
    public int GroupId;      // 그룹 ID
    public int LevelId;      // 레벨 번호
    public int BoSetId;      // 배틀 옵션 셋
}

public partial class ServerData
{
    public int Version;
    public string Text;
    public DateTime LoginTime;
    public DateTime DayRemainTime;
    public DateTime ArenaSeasonEndTime;        //아레나 시즌 종료 시간
    public DateTime ArenaNextSeasonStartTime;   //다음 시즌 시작 시간
    public int ArenaState;

    public DateTime RaidSeasonEndTime;
    public int      RaidCurrentSeason;
    
    public List<GuerrillaCampData> GuerrillaCampList = new List<GuerrillaCampData>();
    public List<GuerrillaMissionData> GuerrillaMissionList = new List<GuerrillaMissionData>();    //9998    //로그인 보너스 미션의 경우 이미지 다운로드
    public List<GachaCategoryData> GachaCategoryList = new List<GachaCategoryData>();                       //가챠 이미지 다운로드
    public List<StoreSaleData> StoreSaleList = new List<StoreSaleData>();
    public List<BannerData> BannerList = new List<BannerData>();                                            //배너 이미지 다운로드
    public int ServerTimeGap = 0;   //  utc와 서버시간의 차이값(초단위)
    public List<SecretQuestOptionData> SecretQuestOptionList = new List<SecretQuestOptionData>();
}


public partial class UserData
{
	public struct sLoginEventInfo
	{
		public int		TableId;
		public ulong	RewardInfoFlag;
		public int		LatestRewardDay;
		public int		EndDay;


		public sLoginEventInfo(int tId, ulong rewardInfoFlag, int latestRewardDay, int endDay)
		{
			TableId = tId;
			RewardInfoFlag = rewardInfoFlag;
			LatestRewardDay = latestRewardDay;
            EndDay = endDay;
		}
	}


	public int PlatformType;     //플레폼 연동 타입
	public string PlatformID;    //플레폼 연동 아이디
	public string PID;           //디바이스 고유아이디
	public long UUID;            //계정 고유 아이디
	public long MainCharUID;     //메인캐릭터 아이디    
	public int RoomThemeSlot;    //룸 ID   
	public int Level;            //레벨
	public int Exp;              //경험치
	public long[] Goods = new long[(int)eGOODSTYPE.COUNT];
	public long HardCash;                       //하드 캐쉬 - 실제 결제로 얻은 캐쉬
	public int UserMarkID = 1;                  //등록한 유저 마크 ID
	public int UserLobbyThemeID = 1;            //로비 테마 ID
	public string UserWord = string.Empty;      //
	public int ItemSlotCnt = 0;                 //아이템 슬롯 갯수
	public int LoginTotalCount = 0;             //총 로그인 일수
	public int LoginContinuityCount = 0;
	public int TutorialNum = 0;                 //아이템 인벤토리 레벨
	public long TutorialFlag = 0;               //아이템 인벤토리 레벨
	public DateTime APRemainTime;               //AP회복 시간
	public DateTime BPRemainTime;               //BP회복 시간
	public int LoginBonusGroupID;               //로그인 보너스 그룹 ID
	public int LoginBonusGroupCnt;              //로그인 그룹 카운트
	public DateTime PrevLoginTime;
	public DateTime LoginBonusRecentDate;       //
	public DateTime LastPlaySpecialModeTime;    // 특별모드 성공시 시간체크용
	public int NextPlaySpecialModeTableID;      //다음 미니게임 모드의 스테이지 테이블 ID
	public string AccountCode = string.Empty;   //이어하기 코드
	public bool PasswordSet = false;            //이어하기 비밀번호를 셋팅 했는지 여부
	public List<AccounLinkData> AccountLinkList = new List<AccounLinkData>();       //연동한 서드파티 리스트
	public int ArenaPrologueValue = 0;          //아레나 프롤로그 게이지 값
	public DateTime NextFrientPointGiveTime;    //친구 포인트 전달 가능 시간
    public List<PassSetData> PassSetDataList = new List<PassSetData>();

	public int CardFormationID = 0;
	public int ArenaCardFormationID = 0;
	public int ArenaTowerCardFormationID = 0;

	public bool AccountCodeReward = false;
	public bool AccountLinkReward = false;

	public List<AwakenSkillInfo> ListAwakenSkillData = new List<AwakenSkillInfo>();

	public bool ShowPkgPopup = false;

	public long[] ArrLobbyBgCharUid { get; private set; } = null;
    public long[] ArrFavorBuffCharUid { get; private set; } = null;
	public long DyeingCostumeId = 0;

	public List<sLoginEventInfo> ListLoginEventInfo { get; private set; } = new List<sLoginEventInfo>(); // 로그인 이벤트 정보 리스트

    public int      BlacklistLevel      = 0;
    public DateTime BlacklistEndTime;

    public int NickNameColorId = 1;

    public DateTime CirclePossibleJoinTime;
    public CircleAttendanceData CircleAttendance = new CircleAttendanceData();
    public long[] CircleChatStampBit { get; private set; } = new long[(int)PktInfoChatStamp.FLAG._MAX_];
    public CircleAuthLevelData CircleAuthLevel
    {
        get
        {
            if (_circleAuthLevelData == null)
            {
                _circleAuthLevelData = new CircleAuthLevelData();
            }

            return _circleAuthLevelData;
        }
    }

    public int LoginBonusMonthlyCount = 0;

    private string                      mNickName;
    private System.Text.StringBuilder   mSbNickName = new System.Text.StringBuilder();
    private CircleAuthLevelData _circleAuthLevelData;

    public bool IsGoods(eGOODSTYPE type, long num)
    {
        if (num > Goods[(int)type])
            return false;
        return true;
    }
    public void AddGoods(eGOODSTYPE type, long num)
    {
        Goods[(int)type] += num;
        //if (type == eGOODSTYPE.GOLD)
        //    AccGold += num;
    }
    public void SubGoods(eGOODSTYPE type, long num)
    {
        Goods[(int)type] -= num;
        if (type == eGOODSTYPE.CASH)
        {
            HardCash -= num;
            if (HardCash < 0)
                HardCash = 0;
        }
    }

    public long GetGoods(eGOODSTYPE type)
    {
        if (type >= eGOODSTYPE.COUNT)
            return -1;

        return Goods[(int)type];
    }

    public void SetGoods(eGOODSTYPE type, long num)
    {
        if (type >= eGOODSTYPE.COUNT)
            return;

        Goods[(int)type] = num;
    }

    public void UpdateAP(long ticketNum, DateTime remainTime)
    {
        Goods[(int)eGOODSTYPE.AP] = ticketNum;
        APRemainTime = GameSupport.GetLocalTimeByServerTime(remainTime);
    }

    public void UpdateBP(long ticketNum, DateTime remainTime)
    {
        Goods[(int)eGOODSTYPE.BP] = ticketNum;
        BPRemainTime = GameSupport.GetLocalTimeByServerTime(remainTime);
    }

    public bool HasAwakenSkill()
    {
        for(int i = 0; i < ListAwakenSkillData.Count; i++)
        {
            if(ListAwakenSkillData[i].Level >= 1)
            {
                return true;
            }
        }

        return false;
    }

    public byte[] GetTutorialByte()
    {
        return BitConverter.GetBytes(TutorialNum);
    }

    public int GetTutorialState()
    {
        byte[] outbytes = BitConverter.GetBytes(TutorialNum);
        return (int)outbytes[(int)eTutorial.State];
    }
    public int GetTutorialStep()
    {
        byte[] outbytes = BitConverter.GetBytes(TutorialNum);
        return (int)outbytes[(int)eTutorial.Step];
    }

    public void SetTutorial( int state, int step)
    {
        byte[] bytes = new byte[(int)eTutorial.Count];
        bytes[0] = (byte)state;
        bytes[1] = (byte)step;
        bytes[2] = 0;
        bytes[3] = 0;
        TutorialNum = BitConverter.ToInt32(bytes, 0);
    }

    public void DoOnTutorialFlag(eTutorialFlag flag)
    {
        uint stateflag = (uint)TutorialFlag;
        GameSupport._DoOnOffBitIdx(ref stateflag, (int)flag, true);
        TutorialFlag = (int)stateflag;
    }

    public void DoOffTutorialFlag(eTutorialFlag flag)
    {
        uint stateflag = (uint)TutorialFlag;
        GameSupport._DoOnOffBitIdx(ref stateflag, (int)flag, false);
        TutorialFlag = (int)stateflag;
    }

    public bool IsOnTutorialFlag(eTutorialFlag flag)
    {
        return GameSupport._IsOnBitIdx((uint)TutorialFlag, (int)flag);
    }

    public int GetLobbyBgCharSlotIndex(long uid)
    {
        for(int i = 0; i < ArrLobbyBgCharUid.Length; i++)
        {
            if(ArrLobbyBgCharUid[i] == uid)
            {
                return i;
            }
        }

        return -1;
    }

    public void SetNickName( string nickName ) {
        mNickName = nickName;
	}

    /// <summary>
    /// 컬러 코드가 안들어간 닉네임 얻는 함수
    /// </summary>
    public string GetRawNickName() {
        return mNickName;
	}

    /// <summary>
    /// 컬러 코드가 안들어간 닉네임의 char 배열을 얻는 함수
    /// </summary>
    public char[] GetRawNickNameCharArr() {
        return mNickName.ToCharArray();
    }

    /// <summary>
    /// 컬러 코드가 들어간 닉네임을 얻는 함수
    /// </summary>
    public string GetNickName() {
        return Utility.GetColoredNickName( mNickName, NickNameColorId, mSbNickName );
    }
}

public partial class StoreData
{
    public long ResetTime;
    public int TableID;
    public long TypeVal;

    public StoreData()
    {
        TableID = -1;
        TypeVal = 0;
        ResetTime = 0;
    }

    public StoreData(int tableid, long typeval )
    {
        TableID = tableid;
        TypeVal = typeval;
    }

    public DateTime GetTime()
    {
        /*
        PktInfoTime pkttime = new PktInfoTime();
        pkttime.time_ = (UInt64)TypeVal;
        //return pkttime.GetTime();
        return GameSupport.GetLocalTimeByServerTime(pkttime.GetTime());
        */

        return GameSupport.GetLocalTimeByServerTime( GetTime( (UInt64)TypeVal ) );
    }

	public DateTime GetTimeByResetTime()
	{
        /*
		PktInfoTime pkttime = new PktInfoTime();
		pkttime.time_ = (UInt64)ResetTime;
		return GameSupport.GetLocalTimeByServerTime(pkttime.GetTime());
        */

        return GameSupport.GetLocalTimeByServerTime( GetTime( (UInt64)ResetTime ) );
    }

	public DateTime GetResetTime()
    {
        /*
        PktInfoTime pkttime = new PktInfoTime();
        pkttime.time_ = (UInt64)ResetTime;
        //return pkttime.GetTime();
        return GameSupport.GetLocalTimeByServerTime(pkttime.GetTime());
        */

        return GameSupport.GetLocalTimeByServerTime( GetTime( (UInt64)ResetTime ) );
    }

    private System.Int32 Year( UInt64 time_ ) { return (System.Int32)( ( time_ / ( (System.UInt64)10000000000 * 1000 ) ) ); }
    private System.Int32 Month( UInt64 time_ ) { return (System.Int32)( ( time_ / ( (System.UInt64)100000000 * 1000 ) ) % 100 ); }
    private System.Int32 MDay( UInt64 time_ ) { return (System.Int32)( ( time_ / ( (System.UInt64)1000000 * 1000 ) ) % 100 ); }
    private System.Int32 Hour( UInt64 time_ ) { return (System.Int32)( ( time_ / ( (System.UInt64)10000 * 1000 ) ) % 100 ); }
    private System.Int32 Minute( UInt64 time_ ) { return (System.Int32)( ( time_ / ( (System.UInt64)100 * 1000 ) ) % 100 ); }
    private System.Int32 Second( UInt64 time_ ) { return (System.Int32)( ( time_ / ( (System.UInt64)1 * 1000 ) ) % 100 ); }
    private System.Int32 MilliSec( UInt64 time_ ) { return (System.Int32)( ( time_ / ( (System.UInt64)1 * 1 ) ) % 1000 ); }

    private System.DateTime GetTime( UInt64 time_ ) {
        if( time_ == 0 )
            return new System.DateTime();
        else {
            int year = Year( time_ );
            int month = Month( time_ );
            int day = MDay( time_ );

            int daysInMonth = System.DateTime.DaysInMonth(year, month);
            if( day > daysInMonth ) {
                day = daysInMonth;
            }

            return new System.DateTime( year, month, day, Hour( time_ ), Minute( time_ ), Second( time_ ), MilliSec( time_ ), System.DateTimeKind.Local );
        }
    }
}

public partial class AchieveData
{
    public int GroupID;
    public int GroupOrder;
    public int Value;
    public bool bTotalComplete;
    public GameTable.Achieve.Param TableData;

    public AchieveData()
    {
        GroupID = -1;
        GroupOrder = -1;
        Value = 0;
        bTotalComplete = false;
    }

    public AchieveData( int group, int order, int value)
    {
        GroupID = group;
        GroupOrder = order;
        Value = value;
        SetTable();
    }

    public void SetTable()
    {
        bTotalComplete = false;
        TableData = GameInfo.Instance.GameTable.FindAchieve(x => x.GroupID == GroupID && x.GroupOrder == GroupOrder);
        if (TableData == null)
        {
            TableData = GameInfo.Instance.GameTable.FindAchieve(x => x.GroupID == GroupID && x.GroupOrder == GroupOrder - 1);
            bTotalComplete = true;
        }
    }
}

public partial class AchieveEventData
{
    public int GroupId;
    public int AchieveGroupId;
    public int GroupOrder;
    public int Value;
    public GameTable.AchieveEventData.Param TableData;

    public AchieveEventData(PktInfoAchieveEvent.Piece achieveEvent)
    {
        SetInfo(achieveEvent);
    }

    public void SetInfo(PktInfoAchieveEvent.Piece achieveEvent)
    {
        GroupId = (int)achieveEvent.groupID_;
        AchieveGroupId = (int)achieveEvent.achieveGroup_;
        GroupOrder = (int)achieveEvent.groupOrder_;
        Value = (int)achieveEvent.condiVal_;

        CheckTotalComplete();
    }

    private void CheckTotalComplete()
    {
        TableData = GameInfo.Instance.GameTable.FindAchieveEventData(x => x.GroupID == GroupId && x.AchieveGroup == AchieveGroupId && x.GroupOrder == GroupOrder);
    }
}

public partial class CharData
{
    public long CUID;            //캐릭터 고유 아이디
    public int TableID;         //테이블 아이디
    public int Grade;           //등급 
    public int Level;           //레벨
    public int Exp;             //경험치
    public int PassviePoint;                                //패시브스킬 포인트
    public int EquipCostumeID;
    public long EquipWeaponUID;
    public long EquipWeapon2UID;            //보조무기 UID
    public int EquipWeaponSkinTID;         //주무기 스킨 TableID
    public int EquipWeapon2SkinTID;        //보조무기 스킨 TableID

    public int CostumeStateFlag;   //byte 가능   인덱스 0 = 헤어, 1 = 무기, 2~7 = 어테치 아이템   0 = 코스튬 원본으로 사용 1 = (무기,헤어=오리지널사용, 어태치아이템= 사용안함)
    public int CostumeColor;       //byte 가능   0~5
    public DyeingData DyeingData;


    public long[] EquipCard = new long[(int)eCOUNT.CARDSLOT];
    public int[] EquipSkill = new int[(int)eCOUNT.SKILLSLOT];
    

    public List<PassiveData> PassvieList = new List<PassiveData>();    //패시브스킬 리스트 실사용
    
    //public List<int> illustidlist
    //외형류 장착 아이템 리스트
    //무기류 장착 아이템 리스트 equi
    //스킬정보 리스트
    //패시브정보 리스트

    public GameTable.Character.Param TableData;

    //친밀도관련
    public int CharAniFlag;
    public int FavorExp = 0;
    public int FavorLevel = 0;
    public int FavorPreCnt = 5;
    
    // 작전 회의실
    public int OperationRoomTID = 0;
    
    // 시크릿 퀘스트
    public int SecretQuestCount;

    // 전투력
    public int CombatPower = 0;

    // 레이드
    public float RaidHpPercentage = 0.0f;

    // 무기 숨기기
    public bool IsHideWeapon = false;

    public CharData PresetDataClone(PresetCharData presetCharData)
    {
        CharData result = new CharData();

        result.CUID = this.CUID;
        result.TableID = this.TableID;
        result.Grade = this.Grade;
        result.Level = this.Level;
        result.Exp = this.Exp;
        result.PassviePoint = this.PassviePoint;
        result.EquipCostumeID = presetCharData.CostumeId;
        result.EquipWeaponUID = presetCharData.WeaponDatas[(int)eWeaponSlot.MAIN].Uid;
        result.EquipWeapon2UID = presetCharData.WeaponDatas[(int)eWeaponSlot.SUB].Uid;
        result.EquipWeaponSkinTID = presetCharData.WeaponDatas[(int)eWeaponSlot.MAIN].SkinTid;
        result.EquipWeapon2SkinTID = presetCharData.WeaponDatas[(int)eWeaponSlot.SUB].SkinTid;
        result.CostumeStateFlag = presetCharData.CostumeStateFlag;
        result.CostumeColor = presetCharData.CostumeColor;

        for (int i = 0; i < result.EquipCard.Length; i++)
        {
            result.EquipCard[i] = presetCharData.CardUids[i];
        }

        for (int i = 0; i < result.EquipSkill.Length; i++)
        {
            result.EquipSkill[i] = presetCharData.SkillIds[i];
        }

        result.PassvieList.Clear();
        result.PassvieList.AddRange(this.PassvieList.ToArray());

        result.TableData = this.TableData;

        result.CharAniFlag = this.CharAniFlag;
        result.FavorExp = this.FavorExp;
        result.FavorLevel = this.FavorLevel;
        result.FavorPreCnt = this.FavorPreCnt;

        result.OperationRoomTID = this.OperationRoomTID;

        result.SecretQuestCount = this.SecretQuestCount;

        result.CombatPower = CombatPower;
        result.RaidHpPercentage = RaidHpPercentage;

        return result;
    }

    public void PresetDataCopy(PresetCharData presetCharData)
    {
        this.EquipCostumeID = presetCharData.CostumeId;

        this.EquipWeaponUID = presetCharData.WeaponDatas[(int)eWeaponSlot.MAIN].Uid;
        this.EquipWeapon2UID = presetCharData.WeaponDatas[(int)eWeaponSlot.SUB].Uid;
        this.EquipWeaponSkinTID = presetCharData.WeaponDatas[(int)eWeaponSlot.MAIN].SkinTid;
        this.EquipWeapon2SkinTID = presetCharData.WeaponDatas[(int)eWeaponSlot.SUB].SkinTid;
        
        this.CostumeStateFlag = presetCharData.CostumeStateFlag;
        this.CostumeColor = presetCharData.CostumeColor;

        for (int i = 0; i < this.EquipCard.Length; i++)
        {
            this.EquipCard[i] = presetCharData.CardUids[i];
        }

        for (int i = 0; i < this.EquipSkill.Length; i++)
        {
            this.EquipSkill[i] = presetCharData.SkillIds[i];
        }
    }

    public void Init(long uid, int tableid)
    {
        CUID = uid;
        TableID = tableid;
        Grade = 1;
        Level = 1;
        Exp = 0;
        PassviePoint = 0;

        EquipCostumeID = (int)eCOUNT.NONE;
        EquipWeaponUID = (int)eCOUNT.NONE;

        CostumeStateFlag = 0;
        CostumeColor = 0;
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
            EquipCard[i] = (int)eCOUNT.NONE;
        for (int i = 0; i < (int)eCOUNT.SKILLSLOT; i++)
            EquipSkill[i] = (int)eCOUNT.NONE;

        TableData = GameInfo.Instance.GameTable.FindCharacter(x => x.ID == tableid);
    }

    public bool DelEquipCard(long cuid)
    {
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            if (EquipCard[i] == cuid)
                EquipCard[i] = (int)eCOUNT.NONE;
        }
        return false;
    }

    public bool IsEquipCard( long cuid )
    {
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
            if (EquipCard[i] == cuid)
                return true;
        return false;
    }

    public CardData GetEquipCard(long cuid)
    {
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
            if (EquipCard[i] == cuid)
                return GameInfo.Instance.GetCardData(EquipCard[i]);
        return null;
    }

    public int GetEquipCardIndex(long cuid)
    {
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
            if (EquipCard[i] == cuid)
                return i;
        return -1;
    }

    public bool IsEquipCardTypeIndex(int index, int cardtype)
    {
        if (EquipCard[index] != (int)eCOUNT.NONE)
        {
            var data = GameInfo.Instance.GetCardData(EquipCard[index]);
            if (data != null)
            {
#if UNITY_EDITOR
                if(data.Type == 0)
                {
                    data.Type = data.TableData.Type;
                }
#endif

                if (data.Type == cardtype)
                    return true;
            }
        }
        return false;
    }

    public bool IsEquipCardTypeIndexWithTableData(CardData cardData, int cardtype)
    {
        if(cardData == null)
        {
            return false;
        }

        if(cardData.Type == cardtype)
        {
            return true;
        }

        /*
        if (EquipCard[index] != (int)eCOUNT.NONE)
        {
            var data = GameInfo.Instance.GameTable.FindCard(x => x.ID == EquipCard[index]);
            if (data != null)
            {
                if (data.Type == cardtype)
                    return true;
            }
        }
        */
        
        return false;
    }

    public bool IsEquipCardType(int cardtype )
    {
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            if (IsEquipCardTypeIndex(i, cardtype))
                return true;
        }
        return false;
    }

    public int GetEquipCardTypeCount(int cardtype)
    {
        int total = 0;
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            if (IsEquipCardTypeIndex(i, cardtype))
                total += 1;
        }
        return total;
    }

    public int GetEquipCardTypeCountWithTableData(List<CardData> listCardData, int cardtype)
    {
        int total = 0;

        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            if(i >= listCardData.Count)
            {
                break;
            }

            if (IsEquipCardTypeIndexWithTableData(listCardData[i], cardtype))
                total += 1;
        }
        return total;
    }

    public int GetEquipCardCount()
    {
        int total = 0;
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            var data = GameInfo.Instance.GetCardData(EquipCard[i]);
            if (data != null)
            {
                total += 1;
            }
        }
        return total;
    }

    public int GetEquipCardCountWithTableData()
    {
        int total = 0;
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            var data = GameInfo.Instance.GameTable.FindCard(x => x.ID == EquipCard[i]);
            if (data != null)
            {
                total += 1;
            }
        }
        return total;
    }

    public int GetCharHP()
    {
        return (int)((TableData.HP + (int)(TableData.IncHP * (Level - 1))) * GameInfo.Instance.GameConfig.CharGradeStatRate[Grade]);
    }
    public int GetCharATK()
    {
        return (int)((TableData.ATK + (int)(TableData.IncATK * (Level - 1))) * GameInfo.Instance.GameConfig.CharGradeStatRate[Grade]);
    }
    public int GetCharDEF()
    {
        return (int)((TableData.DEF + (int)(TableData.IncDEF * (Level - 1))) * GameInfo.Instance.GameConfig.CharGradeStatRate[Grade]);
    }
    public int GetCharCRI()
    {
        return (int)((TableData.CRI + (int)(TableData.IncCRI * (Level - 1))) * GameInfo.Instance.GameConfig.CharGradeStatRate[Grade]);
    }

    public bool IsEquipSkill(int skillId)
    {
        for (int i = 0; i < (int)eCOUNT.SKILLSLOT; i++)
        {
            if (EquipSkill[i] == skillId)
                return true;
        }

        return false;
    }

    public void DoOnCostumeStateFlag(eCostumeStateFlag flag)
    {
        int _flagIdx = (int)flag;
        if (32 <= _flagIdx)
            return;
        int checkFlag = 0x00000001 << _flagIdx;
        CostumeStateFlag |= (System.Int32)checkFlag;
    }

    public void DoOffCostumeStateFlag(eCostumeStateFlag flag)
    {
        int _flagIdx = (int)flag;
        if (32 <= _flagIdx)
            return;
        uint _targetVal = (uint)CostumeStateFlag;
        uint checkFlag = (uint)(0x00000001 << (int)_flagIdx);
        _targetVal &= (uint)0xFFFFFFFF ^ checkFlag;
        CostumeStateFlag = (int)_targetVal;
    }

    public bool IsOnCostumeStateFlag(eCostumeStateFlag flag)
    {
        int _flagIdx = (int)flag;
        if (32 <= _flagIdx)
            return false;
        int checkFlag = 0x00000001 << _flagIdx;
        return (checkFlag == (CostumeStateFlag & (System.Int32)checkFlag));
    }

    static public int CompareFuncTableID(CharData a, CharData b)
    {
        if (a.TableID < b.TableID) return -1;
        if (a.TableID > b.TableID) return 1;
        return 0;
    }
}

public partial class WeaponData
{
    public long                     WeaponUID;		                                    // 고유 식별 ID
    public int                      TableID;		                                    // 무기 테이블 ID
    public int                      Level;		                                        // 무기 레벨
    public int                      Exp;		                                        // 무기 경험치
    public int                      Wake;                                               // 무기 제련 단계
    public int                      SkillLv;                                            // 무기 스킬 레벨 
    public int                      EnchantLv;                                          // 무기 인챈트 레벨
    public long[]                   SlotGemUID  = new long[(int)eCOUNT.WEAPONGEMSLOT];  // 곡옥 슬롯 1에 장착한 곡옥 UID
    public bool                     Lock        = false;	                            // 잠금 상태
    public bool                     New         = false;                                // 신규 클라만
    public bool                     RedDot      = false;                                // 레드닷 클라만
    public GameTable.Weapon.Param   TableData;
    public List<int>                ListCharId  = new List<int>();


    static public bool SortUp = true;
    static public int CompareFuncGradeLevel( WeaponData a, WeaponData b ) {
        WeaponData weapon1 = a;
        WeaponData weapon2 = b;

        if( SortUp ) {
            weapon1 = b;
            weapon2 = a;
        }

        if( weapon1.TableData.Grade == weapon2.TableData.Grade ) {
            if( weapon1.TableData.ID == weapon2.TableData.ID ) {
                return weapon2.WeaponUID.CompareTo( weapon1.WeaponUID );
            }
            else {
                return weapon2.TableData.ID.CompareTo( weapon1.TableData.ID );
            }
        }
        else {
            return weapon2.TableData.Grade.CompareTo( weapon1.TableData.Grade );
        }
    }

    public WeaponData() {
		for( int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++ ) {
			SlotGemUID[i] = (int)eCOUNT.NONE;
		}
	}

	public WeaponData( long uid, int tableid ) {
		WeaponUID = uid;
		TableID = tableid;
		TableData = GameInfo.Instance.GameTable.FindWeapon( x => x.ID == tableid );
		Level = 1;
		Exp = 0;
		Wake = 0;
		SkillLv = 1;

		for( int i = 0; i < (int)eCOUNT.WEAPONGEMSLOT; i++ ) {
			SlotGemUID[i] = (int)eCOUNT.NONE;
		}

        AddCharacterId();
	}

	public WeaponData PresetDataClone( PresetWeaponData presetWeaponData ) {
		WeaponData result = new WeaponData();

		result.WeaponUID = this.WeaponUID;
		result.TableID = this.TableID;
		result.Level = this.Level;
		result.Exp = this.Exp;
		result.Wake = this.Wake;
		result.SkillLv = this.SkillLv;
		result.EnchantLv = this.EnchantLv;

		for( int i = 0; i < result.SlotGemUID.Length; i++ ) {
			long uid = 0;
			if( i < presetWeaponData.GemUids.Length ) {
				uid = presetWeaponData.GemUids[i];
			}
			result.SlotGemUID[i] = uid;
		}

		result.Lock = this.Lock;
		result.New = this.New;
		result.RedDot = this.RedDot;
		result.TableData = this.TableData;

		return result;
	}

	public int GetWeaponATK() {
		float atk = ( ( TableData.ATK + (int)( TableData.IncATK * ( Level - 1 ) ) ) * GameInfo.Instance.GameConfig.WeaponWakeStatRate[Wake] * 
                    GameInfo.Instance.GameConfig.WeaponSkillLvStatRate[SkillLv]);

		if( EnchantLv > 0 && TableData != null ) {
			GameTable.Enchant.Param param = GameInfo.Instance.GameTable.FindEnchant(x => x.GroupID == TableData.EnchantGroup && x.Level == EnchantLv);
			if( param != null ) {
				atk *= ( (float)( param.IncreaseValue + 100 ) / 100 );
			}
		}

		return (int)atk;
	}

	public int GetWeaponATK( int level ) {
		float atk = ( ( TableData.ATK + (int)( TableData.IncATK * ( level - 1 ) ) ) * GameInfo.Instance.GameConfig.WeaponWakeStatRate[Wake] * 
                    GameInfo.Instance.GameConfig.WeaponSkillLvStatRate[SkillLv]);

		if( EnchantLv > 0 && TableData != null ) {
			GameTable.Enchant.Param param = GameInfo.Instance.GameTable.FindEnchant( x => x.GroupID == TableData.EnchantGroup && x.Level == EnchantLv );
			if( param != null ) {
				atk *= ( (float)( param.IncreaseValue + 100 ) / 100 );
			}
		}

		return (int)atk;
	}

    public int GetWeaponCRI() {
        float cri = ( ( TableData.CRI + (int)( TableData.IncCRI * ( Level - 1 ) ) ) * GameInfo.Instance.GameConfig.WeaponWakeStatRate[Wake] * 
                    GameInfo.Instance.GameConfig.WeaponSkillLvStatRate[SkillLv]);

        if( EnchantLv > 0 && TableData != null ) {
            GameTable.Enchant.Param param = GameInfo.Instance.GameTable.FindEnchant(x => x.GroupID == TableData.EnchantGroup && x.Level == EnchantLv);
            if( param != null ) {
                cri *= ( (float)( param.IncreaseValue + 100 ) / 100 );
            }
        }

        return (int)cri;
    }

    public int GetWeaponCRI( int level ) {
		float cri = ( ( TableData.CRI + (int)( TableData.IncCRI * ( level - 1 ) ) ) * GameInfo.Instance.GameConfig.WeaponWakeStatRate[Wake] * 
                    GameInfo.Instance.GameConfig.WeaponSkillLvStatRate[SkillLv]);

		if( EnchantLv > 0 && TableData != null ) {
			GameTable.Enchant.Param param = GameInfo.Instance.GameTable.FindEnchant( x => x.GroupID == TableData.EnchantGroup && x.Level == EnchantLv );
			if( param != null ) {
				cri *= ( (float)( param.IncreaseValue + 100 ) / 100 );
			}
		}

		return (int)cri;
	}

	public bool CheckRedDot() {
		if( GameSupport.CheckWeaponData( this ) ) {
			RedDot = true;
		}
		else {
			RedDot = false;
		}

		return RedDot;
	}

	private void AddCharacterId() {
        string[] split = Utility.Split( TableData.CharacterID, ',' );
        ListCharId.Capacity = split.Length;

        for( int i = 0; i < split.Length; i++ ) {
            ListCharId.Add( Utility.SafeIntParse( split[i] ) );
        }
    }
}

public partial class GemData
{
    public long GemUID;            // 고유 식별 ID
    public int TableID;         // 곡옥 테이블 ID
    public int Level;           // 곡옥 레벨
    public int Exp;             //
    public int Wake;            // 무기 제련 단계
    public int[] RandOptID = new int[(int)eCOUNT.GEMRANDOPT];
    public int[] RandOptValue = new int[(int)eCOUNT.GEMRANDOPT];
    public int TempOptIndex;
    public int TempOptID;
    public int TempOptValue;
    public int SetOptID;
    public bool Lock = false;		// 잠금 상태
    public bool New = false;    //신규 클라만
    public GameTable.Gem.Param TableData;

    public GemData()
    {
        for (int i = 0; i < (int)eCOUNT.GEMRANDOPT; i++)
        {
            RandOptID[i] = -1;
            RandOptValue[i] = -1;
        }
        TempOptIndex = -1;
        TempOptID = -1;
        TempOptValue = 0;
    }

    public GemData(long uid, int tableid)
    {
        GemUID = uid;
        TableID = tableid;
        TableData = GameInfo.Instance.GameTable.FindGem(x => x.ID == tableid);
        Level = 1;
        Wake = 0;
        for (int i = 0; i < (int)eCOUNT.GEMRANDOPT; i++)
        {
            RandOptID[i] = -1;
            RandOptValue[i] = -1;
        }
        TempOptIndex = -1;
        TempOptID = -1;
        TempOptValue = 0;
    }
    public int GetGemHP()
    {
        return (int)((TableData.HP + (int)(TableData.IncHP * (Level - 1))) * GameInfo.Instance.GameConfig.GemWakeStatRate[Wake]);
    }
    public int GetGemATK()
    {
        return (int)((TableData.ATK + (int)(TableData.IncATK * (Level - 1))) * GameInfo.Instance.GameConfig.GemWakeStatRate[Wake]);
    }
    public int GetGemDEF()
    {
        return (int)((TableData.DEF + (int)(TableData.IncDEF * (Level - 1))) * GameInfo.Instance.GameConfig.GemWakeStatRate[Wake]);
    }
    public int GetGemCRI()
    {
        return (int)((TableData.CRI + (int)(TableData.IncCRI * (Level - 1))) * GameInfo.Instance.GameConfig.GemWakeStatRate[Wake]);
    }

    public int GetGemHP(int level)
    {
        return (int)((TableData.HP + (int)(TableData.IncHP * (level - 1))) * GameInfo.Instance.GameConfig.GemWakeStatRate[Wake]);
    }
    public int GetGemATK(int level)
    {
        return (int)((TableData.ATK + (int)(TableData.IncATK * (level - 1))) * GameInfo.Instance.GameConfig.GemWakeStatRate[Wake]);
    }
    public int GetGemDEF(int level)
    {
        return (int)((TableData.DEF + (int)(TableData.IncDEF * (level - 1))) * GameInfo.Instance.GameConfig.GemWakeStatRate[Wake]);
    }
    public int GetGemCRI(int level)
    {
        return (int)((TableData.CRI + (int)(TableData.IncCRI * (level - 1))) * GameInfo.Instance.GameConfig.GemWakeStatRate[Wake]);
    }
    public int GetOptCount()
    {
        return Wake;
    }
   
    public int GetTypeStatus( int i )
    {
        if (i == (int)eGEMCOLOR.GREEN_HP)
            return GetGemHP();
        else if (i == (int)eGEMCOLOR.RED_ATK)
            return GetGemATK();
        else if (i == (int)eGEMCOLOR.BLUE_DEF)
            return GetGemDEF();
        else if(i == (int)eGEMCOLOR.YELLOW_CRI)
            return GetGemCRI();
        return 0;
    }

    public int GetTypeStatus(int i, int level)
    {
        if (i == (int)eGEMCOLOR.GREEN_HP)
            return GetGemHP(level);
        else if (i == (int)eGEMCOLOR.RED_ATK)
            return GetGemATK(level);
        else if (i == (int)eGEMCOLOR.BLUE_DEF)
            return GetGemDEF(level);
        else if (i == (int)eGEMCOLOR.YELLOW_CRI)
            return GetGemCRI(level);
        return 0;
    }

    static public bool SortUp = true;
    static public int CompareFuncGradeLevel(GemData a, GemData b)
    {
        if(SortUp)
        {
            if(a.TableData.Grade == b.TableData.Grade)
            {
                if (a.TableData.ID == b.TableData.ID)
                    return b.Level.CompareTo(a.Level);
                else
                    return a.TableData.ID.CompareTo(b.TableData.ID);
            }
            else
            {
                if (a.TableData.Grade < b.TableData.Grade) return 1;
                if (a.TableData.Grade > b.TableData.Grade) return -1;
            }
        }
        else
        {
            if(a.TableData.Grade == b.TableData.Grade)
            {
                if (a.TableData.ID == b.TableData.ID)
                    return a.Level.CompareTo(b.Level);
                else
                    return b.TableData.ID.CompareTo(a.TableData.ID);
            }
            else
            {
                if (a.TableData.Grade < b.TableData.Grade) return -1;
                if (a.TableData.Grade > b.TableData.Grade) return 1;
            }
        }

        return 0;
    }
}

public partial class CardData
{
    public long CardUID;        //캐릭터 고유 아이디
    public int TableID;         //테이블 아이디
    public int Level;
    public int Exp;
    public int Wake;		    //각성 단계
    public int SkillLv;
    public int EnchantLv;       //인챈트 레벨
    public bool Lock = false;
    public bool New = false;    //신규 클라만
    public bool RedDot = false; //레드닷 클라만
    public long PosValue;     // 위치 값
    public int  PosKind;      // 위치 분류 - eContentsPosKind
    public int  PosSlot = 0;
    public int  Type;           // 속성    

    public GameTable.Card.Param TableData;


    public CardData()
    {

    }

    public CardData(long uid, int tableid)
    {
        CardUID = uid;
        TableID = tableid;
        TableData = GameInfo.Instance.GameTable.FindCard(x => x.ID == tableid);
        SkillLv = 1;
        EnchantLv = 0;
        Level = 1;
        Exp = 0;
        Wake = 0;
        PosKind = (int)eContentsPosKind._NONE_;
        PosValue = 0;
    }

    public int GetCardHP()
    {
        float hp = ((TableData.HP + (int)(TableData.IncHP * (Level - 1))) * GameInfo.Instance.GameConfig.CardWakeStatRate[Wake] * GameInfo.Instance.GameConfig.CardSkillLvStatRate[SkillLv]);

        if (EnchantLv > 0 && TableData != null)
        {
            GameTable.Enchant.Param param = GameInfo.Instance.GameTable.FindEnchant(x => x.GroupID == TableData.EnchantGroup && x.Level == EnchantLv);
            if (param != null)
            {
                hp *= ((float)(param.IncreaseValue + 100) / 100);
            }
        }

        return (int)hp;
    }

    public int GetCardDEF()
    {
        float def = ((TableData.DEF + (int)(TableData.IncDEF * (Level - 1))) * GameInfo.Instance.GameConfig.CardWakeStatRate[Wake] * GameInfo.Instance.GameConfig.CardSkillLvStatRate[SkillLv]);

        if (EnchantLv > 0 && TableData != null)
        {
            GameTable.Enchant.Param param = GameInfo.Instance.GameTable.FindEnchant(x => x.GroupID == TableData.EnchantGroup && x.Level == EnchantLv);
            if (param != null)
            {
                def *= ((float)(param.IncreaseValue + 100) / 100);
            }
        }

        return (int)def;
    }

    public int GetCardHP(int level)
    {
        float hp = ((TableData.HP + (int)(TableData.IncHP * ( level - 1))) * GameInfo.Instance.GameConfig.CardWakeStatRate[Wake] * GameInfo.Instance.GameConfig.CardSkillLvStatRate[SkillLv]);

        if (EnchantLv > 0 && TableData != null)
        {
            GameTable.Enchant.Param param = GameInfo.Instance.GameTable.FindEnchant(x => x.GroupID == TableData.EnchantGroup && x.Level == EnchantLv);
            if (param != null)
            {
                hp *= ((float)(param.IncreaseValue + 100) / 100);
            }
        }

        return (int)hp;
    }

    public int GetCardDEF(int level)
    {
        float def = ((TableData.DEF + (int)(TableData.IncDEF * ( level - 1))) * GameInfo.Instance.GameConfig.CardWakeStatRate[Wake] * GameInfo.Instance.GameConfig.CardSkillLvStatRate[SkillLv]);

        if (EnchantLv > 0 && TableData != null)
        {
            GameTable.Enchant.Param param = GameInfo.Instance.GameTable.FindEnchant(x => x.GroupID == TableData.EnchantGroup && x.Level == EnchantLv);
            if (param != null)
            {
                def *= ((float)(param.IncreaseValue + 100) / 100);
            }
        }

        return (int)def;
    }

    public bool CheckRedDot()
    {
        if (GameSupport.CheckCardData(this))
            RedDot = true;
        else
            RedDot = false;
        return RedDot;
    }

    public void InitPos()
    {
        PosKind = 0;
        PosSlot = 0;
        PosValue = 0;
        
    }

    public void SetPos(long v, int k, int s)
    {
        PosValue = v;
        PosKind = k;
        PosSlot = s;
    }

    static public bool SortUp = true;
    static public int CompareFuncGradeLevel(CardData a, CardData b)
    {
        CardData card1 = a;
        CardData card2 = b;

        if (SortUp)
        {
            card1 = b;
            card2 = a;
        }

        if (card1.TableData.Grade == card2.TableData.Grade)
        {
            if (card1.TableData.ID == card2.TableData.ID)
            {
                return card2.CardUID.CompareTo(card1.CardUID);
            }
            else
            {
                return card2.TableData.ID.CompareTo(card1.TableData.ID);
            }
        }
        else
        {
            return card2.TableData.Grade.CompareTo(card1.TableData.Grade);
        }
    }
}

public partial class ItemData
{
    public long ItemUID;            //캐릭터 고유 아이디
    public int TableID;         //테이블 아이디
    public int Count;           //부적 = 레벨, 갯수
    public bool New = false;    //신규 클라만
    public GameTable.Item.Param TableData;


    public ItemData()
    {

    }

    public ItemData( long uid, int tableid )
    {
        ItemUID = uid;
        TableID = tableid;
        Count = 0;
        TableData = GameInfo.Instance.GameTable.FindItem(x => x.ID == tableid);
    }

    static public bool SortUp = true;
    static public int CompareFuncGrade(ItemData a, ItemData b)
    {
        if (SortUp)
        {
            if (a.TableData.Grade < b.TableData.Grade) return 1;
            if (a.TableData.Grade > b.TableData.Grade) return -1;
        }
        else
        {
            if (a.TableData.Grade < b.TableData.Grade) return -1;
            if (a.TableData.Grade > b.TableData.Grade) return 1;
        }

        return 0;
    }
}


public partial class FacilityData
{
    public int TableID;
    public int Level;
    public int Stats;    //시설 상태 0 = 대기, 1 = 진행중, 2 = 완료
    public long EquipCardUID; //서포터 uid
    public long Selete; //이용중인 캐릭터 또는 아이템조합 테이블 아이디
    public DateTime RemainTime; //남은시간 60초 1틱
    public GameTable.Facility.Param TableData;
    public int OperationCnt = 1;

    public FacilityData()
    {

    }
    public FacilityData(int tableid, int level, DateTime time , int operationCnt = 1)
    {
        TableID = tableid;
        Level = level;
        Stats = (int)eFACILITYSTATS.WAIT;
        EquipCardUID = (int)eCOUNT.NONE;
        Selete = (int)eCOUNT.NONE;
        RemainTime = time;
        OperationCnt = operationCnt;
        TableData = GameInfo.Instance.GameTable.FindFacility(x => x.ID == tableid);
    }
  
    public int GetEffectValue()
    {
        return TableData.EffectValue + (int)(TableData.IncEffectValue * (Level - 1));
    }
    public int GetEffectValue(int level)
    {
        if(level == 0)
            return 0;
        return TableData.EffectValue + (int)(TableData.IncEffectValue * (level - 1));
    }
    public int GetEffectTime() //분
    {
        return TableData.Time;
    }

}

//=================================================================================================
// 피규어 룸 데이터들
//=================================================================================================
public partial class RoomThemeFuncData
{
    public int TableID; //RoomFunc
    public bool On;

    
    public GameTable.RoomFunc.Param TableData;
    public RoomThemeFuncData()
    {

    }
    public RoomThemeFuncData(int tableid)
    {
        TableID = tableid;
        TableData = GameInfo.Instance.GameTable.FindRoomFunc(TableID);
    }

    public RoomThemeFuncData DeepCopy()
    {
        RoomThemeFuncData newCopy = new RoomThemeFuncData();
        newCopy.TableID = this.TableID;
        newCopy.On = this.On;
        return newCopy;
    }
}
public partial class RoomThemeSlotData
{
    public int SlotNum;
    public int TableID; //RoomTheme
    public List<RoomThemeFuncData> RoomThemeFuncList = new List<RoomThemeFuncData>();
    public GameTable.RoomTheme.Param TableData;
    public byte[] ArrLightInfo;

    public RoomThemeSlotData()
    {

    }
    public RoomThemeSlotData(int slot, int tableid)
    {
        SlotNum = slot;
        TableID = tableid;
        TableData = GameInfo.Instance.GameTable.FindRoomTheme(TableID);
    }

    public void Init()
    {
        SlotNum = -1;
        TableID = -1;

    }
    public RoomThemeSlotData DeepCopy()
    {
        RoomThemeSlotData newCopy = new RoomThemeSlotData();
        newCopy.SlotNum = this.SlotNum;
        newCopy.TableID = this.TableID;
        newCopy.TableData = GameInfo.Instance.GameTable.FindRoomTheme(TableID);
        newCopy.ArrLightInfo = ArrLightInfo;
        newCopy.RoomThemeFuncList = new List<RoomThemeFuncData>();
        for (int i = 0; i < RoomThemeFuncList.Count; i++)
            newCopy.RoomThemeFuncList.Add(RoomThemeFuncList[i].DeepCopy());

        return newCopy;
    }
}

public partial class RoomThemeFigureSlotData
{
    public int SlotNum;
    public int RoomThemeSlotNum;
    public int TableID; //RoomFigure
    public int Action1; //RoomAction
    //public int Action2; //RoomAction
    public byte[] detailarry;
    public GameTable.RoomFigure.Param TableData;

    public int CostumeStateFlag;   //byte 가능   인덱스 0 = 헤어, 1 = 무기, 2~7 = 어테치 아이템   0 = 코스튬 원본으로 사용 1 = (무기,헤어=오리지널사용, 어태치아이템= 사용안함)
    public int CostumeColor;       //byte 가능   0~5


    public RoomThemeFigureSlotData()
    {

    }
    public RoomThemeFigureSlotData(int tableid)
    {
        TableID = tableid;
        TableData = GameInfo.Instance.GameTable.FindRoomFigure(TableID);
    }

    public RoomThemeFigureSlotData DeepCopy()
    {
        RoomThemeFigureSlotData newCopy = new RoomThemeFigureSlotData();

        newCopy.SlotNum = this.SlotNum;
        newCopy.RoomThemeSlotNum = this.RoomThemeSlotNum;
        newCopy.TableID = this.TableID;
        newCopy.Action1 = this.Action1;
        //newCopy.Action2 = this.Action2;
        newCopy.detailarry = this.detailarry;
        newCopy.TableData = GameInfo.Instance.GameTable.FindRoomFigure(TableID);
        newCopy.CostumeStateFlag = CostumeStateFlag;
        newCopy.CostumeColor = CostumeColor;

        return newCopy;
    }
}

public partial class StageClearData
{
    public int TableID;
    public int[] Mission = new int[(int)eCOUNT.STAGEMISSION];
    public GameTable.Stage.Param TableData;
    public StageClearData()
    {
        TableID = -1;
        for (int i = 0; i < (int)eCOUNT.STAGEMISSION; i++)
            Mission[i] = (int)eCOUNT.NONE;
    }

    public StageClearData(int tableid)
    {
        TableID = tableid;
        for (int i = 0; i < (int)eCOUNT.STAGEMISSION; i++)
            Mission[i] = (int)eCOUNT.NONE;
        TableData = GameInfo.Instance.GameTable.FindStage(TableID);
    }

    public int GetMissionCount()
    {
        int count = 0;
        if (TableData.Mission_00 > 0)
            count += 1;
        if (TableData.Mission_01 > 0)
            count += 1;
        if (TableData.Mission_02 > 0)
            count += 1;
        return count;
    }

    public int GetClearCount()
    {
        int count = 0;
        for (int i = 0; i < (int)eCOUNT.STAGEMISSION; i++)
            if (Mission[i] != (int)eCOUNT.NONE)
                count += 1;
        return count;
    }

    public bool IsClearMission( int index )
    {
        if (Mission[index] != (int)eCOUNT.NONE)
            return false;
        return true;
    }
    public bool IsClearAll()
    {
        if (GetClearCount() == GetMissionCount())
            return true;
        return false;
    }

    static public bool SortUp = true;
    static public int CompareFuncStageID(StageClearData a, StageClearData b)
    {
        if (SortUp)
        {
            if (a.TableData.ID < b.TableData.ID) return 1;
            if (a.TableData.ID > b.TableData.ID) return -1;
        }
        else
        {
            if (a.TableData.ID < b.TableData.ID) return -1;
            if (a.TableData.ID > b.TableData.ID) return 1;
        }

        return 0;
    }
}

public partial class TimeAttackClearData
{
    public int TableID;
    public int HighestScore;
    public long CharCUID;
    public DateTime HighestScoreRemainTime;
    public GameTable.Stage.Param TableData;

    public TimeAttackClearData()
    {

    }
    public TimeAttackClearData(int tableid)
    {
        TableID = tableid;
        TableData = GameInfo.Instance.GameTable.FindStage(TableID);
    }
}

public partial class TimeAttackRankUserData {
	public long             UUID;
	public int              Rank;                                       //등수
	public int              HighestScore;                               //점수
	public int              UserMark;
	public int              UserRank;
	public bool             bDetail;
	public CharData         CharData;
	public List<CardData>   CardList            = new List<CardData>();
	public WeaponData       WeaponData;
	public List<GemData>    MainGemList         = new List<GemData>();
	public WeaponData       SubWeaponData;
	public List<GemData>    SubGemList          = new List<GemData>();
	public int              UserNickNameColorId = 1;
    public bool             IsRaidFirstRanker   = false;

	private string                      mUserNickName;
    private System.Text.StringBuilder   mSbUserNickName = new System.Text.StringBuilder();


    public void SetUserNickName( string nickName ) {
        mUserNickName = nickName;
    }

	/// <summary>
	/// 컬러 코드가 들어간 유저 닉네임을 얻는 함수
	/// </summary>
	public string GetUserNickName() {
        return Utility.GetColoredNickName( mUserNickName, UserNickNameColorId, mSbUserNickName );
    }
}

public partial class TimeAttackRankData
{
    public int TableID;
    public long UpdateTM;
    public List<TimeAttackRankUserData> RankUserList = new List<TimeAttackRankUserData>();
    public GameTable.Stage.Param TableData;

    public TimeAttackRankData()
    {

    }
    public TimeAttackRankData(int tableid)
    {
        TableID = tableid;
        TableData = GameInfo.Instance.GameTable.FindStage(TableID);
    }
}

public partial class WeaponBookData
{
    public int TableID;         //테이블 아이디
    public int StateFlag;           //상태    

    public WeaponBookData()
    {
        StateFlag = 0;
    }

    public WeaponBookData(int tableid, int state)
    {
        TableID = tableid;
        StateFlag = state;
    }

    public void DoOnFlag(eBookStateFlag flag)
    {
        uint stateflag = (uint)StateFlag;
        GameSupport._DoOnOffBitIdx(ref stateflag, (int)flag, true);
        StateFlag = (int)stateflag;
    }

    public void DoOffFlag(eBookStateFlag flag)
    {
        uint stateflag = (uint)StateFlag;
        GameSupport._DoOnOffBitIdx(ref stateflag, (int)flag, false);
        StateFlag = (int)stateflag;
    }


    public bool IsOnFlag(eBookStateFlag flag)
    {
        return GameSupport._IsOnBitIdx((uint)StateFlag, (int)flag);
    }

    static public int CompareFuncTableID(WeaponBookData a, WeaponBookData b)
    {
        if (a.TableID < b.TableID) return -1;
        if (a.TableID > b.TableID) return 1;
        return 0;
    }
}

public partial class CardBookData
{
    public int TableID;         //테이블 아이디
    public int FavorLevel;      //호감도 레벨
    public int FavorExp;        //호감도 경험치 
    public int StateFlag;           //상태    

    public CardBookData()
    {
        StateFlag = 0;
    }

    public CardBookData(int tableid, int state)
    {
        TableID = tableid;
        StateFlag = 0;
    }

    public void DoOnFlag(eBookStateFlag flag)
    {
        uint stateflag = (uint)StateFlag;
        GameSupport._DoOnOffBitIdx(ref stateflag, (int)flag, true);
        StateFlag = (int)stateflag;
    }

    public void DoOffFlag(eBookStateFlag flag)
    {
        uint stateflag = (uint)StateFlag;
        GameSupport._DoOnOffBitIdx(ref stateflag, (int)flag, false);
        StateFlag = (int)stateflag;
    }

    public bool IsOnFlag(eBookStateFlag flag)
    {
        return GameSupport._IsOnBitIdx((uint)StateFlag, (int)flag);
    }

  

    static public int CompareFuncTableID(CardBookData a, CardBookData b)
    {
        if (a.TableID < b.TableID) return -1;
        if (a.TableID > b.TableID) return 1;
        return 0;
    }
}


public partial class MonsterBookData
{
    public int TableID;         //테이블 아이디
    public int StateFlag;           //상태    

    public MonsterBookData()
    {
        StateFlag = 0;
    }

    public MonsterBookData(int tableid, int state)
    {
        TableID = tableid;
        StateFlag = state;
    }

    public void DoOnFlag(eBookStateFlag flag)
    {
        uint stateflag = (uint)StateFlag;
        GameSupport._DoOnOffBitIdx(ref stateflag, (int)flag, true);
        StateFlag = (int)stateflag;
    }

    public void DoOffFlag(eBookStateFlag flag)
    {
        uint stateflag = (uint)StateFlag;
        GameSupport._DoOnOffBitIdx(ref stateflag, (int)flag, false);
        StateFlag = (int)stateflag;
    }

    public bool IsOnFlag(eBookStateFlag flag)
    {
        return GameSupport._IsOnBitIdx((uint)StateFlag, (int)flag);
    }

    static public int CompareFuncTableID(MonsterBookData a, MonsterBookData b)
    {
        if (a.TableID < b.TableID) return -1;
        if (a.TableID > b.TableID) return 1;
        return 0;
    }
}

public partial class MailData
{
    //  유저식별ID
    public ulong MailUID;       //  우편식별ID
    public int ProductType;     //  상품타입
    public uint ProductIndex;   //  상품인덱스
    public uint ProductValue;   //  상품수량
    public DateTime RemainTime; //  삭제예정시간(남은시간 60초 1틱)
    public eMailType MailType;       //  우편타입ID          
    public string MailTypeValue;   //  우편타입값
}

public partial class WeekMissionData
{
    public uint fWeekMissionSetID;                    //  주간 미션 Set ID
    public uint[] fMissionRemainCntSlot = new uint[7];  //  주간 미션 슬롯 0 ~ 6 남은 목표 횟수
    public uint fMissionRewardFlag;                     //  주간 미션 보상 획득 비트 플래그
    public DateTime fWeekMissionResetDate;            //  주간 미션 Set ID 초기화 예정 시간

    public int GetComplateCount()
    {
        int count = 0;
        for(int i = 0; i< fMissionRemainCntSlot.Length; i++)
        {
            if (fMissionRemainCntSlot[i] == 0) count++;
        }
        return count;
    }
}

//9998
public partial class GllaMissionData
{
    public int GroupID;
    public int Count;
    public int Step;
    public bool LoginBonusDisplayFlag = false;  // 로그인 보너스 표시 여부 플래그

    public GllaMissionData()
    {
     
    }

    public GllaMissionData(int group, int count, int step, bool loginbonusdisplayflag = false)
    {
        GroupID = group;
        Count = count;
        Step = step;
        LoginBonusDisplayFlag = loginbonusdisplayflag;
    }
}

public partial class MyDyeingData
{
    public long TableId;
    public byte LockFlag;
    public DyeingData DyeingData;

    public MyDyeingData(PktInfoCostume.Piece info)
    {
        DyeingData = new DyeingData();
        ChangeData(info);
    }

    public void ChangeData(PktInfoCostume.Piece info)
    {
        TableId = info.tableID_;
        LockFlag = info.lockFlag_;
        DyeingData.ChangeColorData(info);
    }
}

public partial class DyeingData
{
    public bool IsFirstDyeing;
    public List<Color> PartsColorList = new List<Color>() { Color.white, Color.white, Color.white };

    public DyeingData()
    {
        
    }

    public DyeingData(PktInfoConPosCharDetail.CostumeDyeing info)
    {
        ChangeColorData(info);
    }

    public void ChangeColorData(PktInfoCostume.Piece info)
    {
        IsFirstDyeing = info.isFirstDyeing_;
        
        if (info.part1_ != null)
            PartsColorList[0] = new Color32(info.part1_.red_, info.part1_.green_, info.part1_.blue_, byte.MaxValue);
        
        if (info.part2_ != null)
            PartsColorList[1] = new Color32(info.part2_.red_, info.part2_.green_, info.part2_.blue_, byte.MaxValue);
        
        if (info.part3_ != null)
            PartsColorList[2] = new Color32(info.part3_.red_, info.part3_.green_, info.part3_.blue_, byte.MaxValue);
    }
    
    public void ChangeColorData(PktInfoConPosCharDetail.CostumeDyeing info)
    {
        IsFirstDyeing = info.isFirstDyeing_;
        
        if (info.part1_ != null)
            PartsColorList[0] = new Color32(info.part1_.red_, info.part1_.green_, info.part1_.blue_, byte.MaxValue);
        
        if (info.part2_ != null)
            PartsColorList[1] = new Color32(info.part2_.red_, info.part2_.green_, info.part2_.blue_, byte.MaxValue);
        
        if (info.part3_ != null)
            PartsColorList[2] = new Color32(info.part3_.red_, info.part3_.green_, info.part3_.blue_, byte.MaxValue);
    }
}

public partial class RewardData//클라전용
{
    public long UID;
    public int Type;
    public int Index; // Table Id
    public int Value;
    public int Count;
    public int NewCount;
    public bool bNew;
    public bool ChangeGrade;    //결과시 럭키
    public bool MonthGrade;     //월정액 보너스
    public bool FavorGrade;     //친밀도 보너스
    public eGRADE eGrade = eGRADE.GRADE_NONE;
    
    public RewardData()
    {

    }
    
    public RewardData(long uid, int type, int index, int value, bool changeGrade, bool monthGrade = false, bool favorGrade = false)
    {
        UID = uid;
        Type = type;
        Index = index;
        Value = value;
        bNew = false;
        ChangeGrade = changeGrade;
        MonthGrade = monthGrade;
        FavorGrade = favorGrade;
        SetGrade();
    }

    /// <summary>
    ///   RewardUI용
    /// </summary>
    public RewardData(int type, int index, int value)
    {
        Type = type;
        Index = index;
        Value = value;
        SetGrade();
    }

    public RewardData( int type, int index, int value, int count, int newCount ) {
		Type = type;
		Index = index;
		Value = value;
        Count = count;
        NewCount = newCount;
		SetGrade();
	}

    public eREWARDTYPE GetRewardType { get { return (eREWARDTYPE)Type; } }

    private void SetGrade()
    {
        switch ((eREWARDTYPE)Type)
        {          
            case eREWARDTYPE.WEAPON:
                {
                    var tabledata = GameInfo.Instance.GameTable.FindWeapon(Index);
                    if (tabledata != null) eGrade = (eGRADE)tabledata.Grade;
                }
                break;
            case eREWARDTYPE.GEM:
                {
                    var tabledata = GameInfo.Instance.GameTable.FindGem(Index);
                    if (tabledata != null) eGrade = (eGRADE)tabledata.Grade;
                }
                break;
            case eREWARDTYPE.CARD:
                {
                    var tabledata = GameInfo.Instance.GameTable.FindCard(Index);
                    if (tabledata != null) eGrade = (eGRADE)tabledata.Grade;
                }
                break;
            case eREWARDTYPE.COSTUME:
                {
                    var tabledata = GameInfo.Instance.GameTable.FindCostume(Index);
                    if (tabledata != null) eGrade = (eGRADE)tabledata.Grade;
                }
                break;
            case eREWARDTYPE.ITEM:
                {
                    var tabledata = GameInfo.Instance.GameTable.FindItem(Index);
                    if (tabledata != null) eGrade = (eGRADE)tabledata.Grade;
                }
                break;
            case eREWARDTYPE.LOBBYTHEME:
                {
                    //211207 경배님과 의논함 강제로 노말
                    eGrade = eGRADE.GRADE_N;
                }
                break;
        }
    }
}




public class PassiveData    //클라전용
{
    public int SkillID = 0;
    public int SkillLevel = 0;
    
    public GameTable.CharacterSkillPassive.Param TableData;

    public PassiveData()
    {

    }
    public PassiveData(int tableid, int level)
    {
        SkillID = tableid;
        SkillLevel = level;

        TableData = GameInfo.Instance.GameTable.FindCharacterSkillPassive(SkillID);
    }

    public void Init()
    {
        TableData = GameInfo.Instance.GameTable.FindCharacterSkillPassive(SkillID);
    }

    public float GetValue1()
    {
        return TableData.Value1 + (TableData.IncValue1 * (float)(SkillLevel - 1));
    }

    public float GetValue2()
    {
        return TableData.Value2 + (TableData.IncValue2 * (float)(SkillLevel - 1));
    }

    public float GetValue1(int level)
    {
        return TableData.Value1 + (TableData.IncValue1 * (float)(level - 1));
    }

    public float GetValue2(int level)
    {
        return TableData.Value2 + (TableData.IncValue2 * (float)(level - 1));
    }
}

public class MatItemData //클라전용
{
    public ItemData ItemData;
    public int Count;
    public MatItemData(ItemData itemdata, int count)
    {
        ItemData = itemdata;
        Count = count;
    }

    static public bool SortUp = true;
    static public int CompareFuncGrade(MatItemData a, MatItemData b)
    {
        if (SortUp)
            return a.ItemData.TableData.Grade.CompareTo(b.ItemData.TableData.Grade);
        else
            return b.ItemData.TableData.Grade.CompareTo(a.ItemData.TableData.Grade);
    }
}

public class MatSkillData
{
    public enum eTYPE
    {
        ITEM = 0,
        WEAPON,
        CARD,
    }
    public eTYPE Type;
    public ItemData ItemData;
    public WeaponData WeaponData;
    public CardData CardData;
    public MatSkillData(ItemData itemdata)
    {
        Type = eTYPE.ITEM;
        ItemData = itemdata;
    }
    public MatSkillData(WeaponData itemdata)
    {
        Type = eTYPE.WEAPON;
        WeaponData = itemdata;
    }
    public MatSkillData(CardData itemdata)
    {
        Type = eTYPE.CARD;
        CardData = itemdata;
    }
}

public class GameResultData    //클라전용
{
    public struct sCharExpInfo {
        public long Uid;
        public int  BeforeGrade;
        public int  BeforeLevel;
        public int  BeforeExp;
        public int  AfterLevel;
        public int  AfterExp;


        public void Init() {
            Uid = 0;
            BeforeGrade = 0;
            BeforeLevel = 0;
            BeforeExp = 0;
            AfterLevel = 0;
            AfterExp = 0;
        }
    }


    public int StageID;
    public int UserBeforeLevel;
    public int UserAfterLevel;
    public int UserBeforeExp;
    public int UserAfterExp;

    public long CharUID;
    public int CharBeforeGrade;
    public int CharBeforeLevel;
    public int CharAfterLevel;
    public int CharBeforeExp;
    public int CharAfterExp;

    public long[] CardUID = new long[(int)eCOUNT.CARDSLOT];
    public int[] CardFavorBeforeLevel = new int[(int)eCOUNT.CARDSLOT];
    public int[] CardFavorAfterLevel = new int[(int)eCOUNT.CARDSLOT];
    public int[] CardFavorBeforeExp = new int[(int)eCOUNT.CARDSLOT];
    public int[] CardFavorAfterExp = new int[(int)eCOUNT.CARDSLOT];

    public long[] Goods = new long[(int)eGOODSTYPE.COUNT];
    
    public List<int> ClearMissionIDList = new List<int>();

    // for Raid
    public sCharExpInfo[] RaidCharExpInfoArr = new sCharExpInfo[3];


    public void Init()
    {
        StageID = 0;
        UserBeforeLevel = 0;
        UserAfterLevel = 0;
        UserBeforeExp = 0;
        UserAfterExp = 0;

        CharUID = 0;
        CharBeforeGrade = 0;
        CharBeforeLevel = 0;
        CharAfterLevel = 0;
        CharBeforeExp = 0;
        CharAfterExp = 0;

        for (int i = 0; i < (int)eGOODSTYPE.COUNT; i++)
            Goods[i] = 0;

        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            CardUID[i] = 0;
            CardFavorBeforeLevel[i] = 0;
            CardFavorAfterLevel[i] = 0;
            CardFavorBeforeExp[i] = 0;
            CardFavorAfterExp[i] = 0;
        }

        ClearMissionIDList.Clear();
        //BoxTypeList.Clear();

        for( int i = 0; i < RaidCharExpInfoArr.Length; i++ ) {
            RaidCharExpInfoArr[i].Init();
        }
    }
}

public class FacilityResultData
{
    public int FacilityID;
    public int UserBeforeLevel;
    public int UserAfterLevel;
    public int UserBeforeExp;
    public int UserAfterExp;

    public long TargetUID;
    public int TargetBeforeLevel;
    public int TargetAfterLevel;
    public int TargetBeforeExp;
    public int TargetAfterExp;

    public long ItemUID;
    public int ItemCount;

    //서포터 호감도 관련 값
    public long CardUID;
    public int CardBeforeLevel;
    public int CardAfterLevel;
    public int CardBeforeExp;
    public int CardAfterExp;

    public void Init()
    {
        FacilityID = -1;
        UserBeforeLevel = 0;
        UserAfterLevel = 0;
        UserBeforeExp = 0;
        UserAfterExp = 0;

        TargetUID = (long)eCOUNT.NONE;
        TargetBeforeLevel = 0;         
        TargetAfterLevel = 0;
        TargetBeforeExp = 0;
        TargetAfterExp = 0;

        ItemUID = (long)eCOUNT.NONE;
        ItemCount = 0;

        CardUID = 0;
        CardBeforeLevel = 0;
        CardAfterLevel = 0;
        CardBeforeExp = 0;
        CardAfterExp = 0;
    }
}



public partial class EventSetData
{
    public int TableID;
    public GameTable.EventSet.Param TableData;
    public int RewardStep;
    public int Count;
    public List<int> RewardItemCount = new List<int>();
    public EventSetData() { }
    public EventSetData(int tableid)
    {
        TableID = tableid;
        TableData = GameInfo.Instance.GameTable.FindEventSet(x => x.EventID == tableid);
    }
    public EventSetData(int tableid, int step, int count)
    {
        TableID = tableid;
        TableData = GameInfo.Instance.GameTable.FindEventSet(x => x.EventID == tableid);
        RewardStep = step;
        Count = count;
    }
}

public partial class UserBattleData
{
    public int Now_Score;                           //현재 배틀 스코어
    public int Now_Rank;                            //현재 순위
    public ObscuredInt Now_WinLoseCnt;              //현재 승리 또는 패배 횟수(양수:연속 승리 횟수, 음수:연속 패배 횟수) • 연속 승리 또는 패배 도중 반대 결과가 나오는 경우 -1 또는 +1이 됨
    public int Now_RewardFalg;                      //보상 획득 상태 플래그
    public int Now_GradeId;                         //현재 등급 아이디 
    public DateTime Now_RewardDate;                 //최근 보상 획득 일시
    //public int Now_PromotionFalg;                   //승급전 여부 플래그
    public int Now_PromotionRemainCnt;              //남은 승급전 횟수
    public int Now_PromotionWinCnt;                 //승급전 승리 횟수
    public int SR_BestScore;                        //시즌 기록(최고 배틀 스코어)
    public int SR_BestWinningStreak;                //시즌 기록(최고 연승)
    public int SR_TotalCnt;                         //시즌 기록(총 경기 횟수)
    public int SR_FirstWinCnt;                      //시즌 기록(선봉 승리 횟수)
    public int SR_SecondWinCnt;                     //시즌 기록(중견 승리 횟수)
    public int SR_ThirdWinCnt;                      //시즌 기록(대장 승리 횟수)
    public int SR_Rank;

    public UInt32       CheatValue;
    public eArenaCheat  CheatType;
}

public partial class BadgeData
{
    public long BadgeUID;//                                     //문양 아이템 UID
    public int[] OptID = new int[(int)eCOUNT.BADGEOPTCOUNT];    //옵션 ID
    public int[] OptVal = new int[(int)eCOUNT.BADGEOPTCOUNT];   //옵션 값
    public int Level;                                           //레벨
    public int RemainLvCnt;	                                    //남은 강화 횟수(레벨업)
    public bool Lock;                                           //잠금 여부

    public long PosValue;
    public int PosKind;
    public int PosSlotNum;

    public bool New = false;    //신규 클라만
    public bool RedDot = false; //레드닷 클라만

    static public bool SortUp = true;
    static public int CompareFuncOptID(BadgeData a, BadgeData b)
    {
        if(SortUp)
        {
            if(a.OptID[(int)eBadgeOptSlot.FIRST] == b.OptID[(int)eBadgeOptSlot.FIRST])
            {
                return b.Level.CompareTo(a.Level);
            }
            else
            {
                return a.OptID[(int)eBadgeOptSlot.FIRST].CompareTo(b.OptID[(int)eBadgeOptSlot.FIRST]);
            }
        }
        else
        {
            if(a.OptID[(int)eBadgeOptSlot.FIRST] == b.OptID[(int)eBadgeOptSlot.FIRST])
            {
                return a.Level.CompareTo(b.Level);
            }
            else
            {
                return b.OptID[(int)eBadgeOptSlot.FIRST].CompareTo(b.OptID[(int)eBadgeOptSlot.FIRST]);
            }
        }
        return 0;
    }
}

public partial class TeamCharData
{
    public CharData CharData;                                   //팀에 배치될 캐릭터 데이터
    public WeaponData MainWeaponData;                               //배치된 캐릭터가 장착한 무기
    public WeaponData SubWeaponData;
    public List<CardData> CardList = new List<CardData>();      //배치된 캐릭터가 장착한 서포터 리스트
    public List<GemData> MainGemList = new List<GemData>();         //무기에 장착된곡옥 리스트
    public List<GemData> SubGemList = new List<GemData>();
    public List<AwakenSkillInfo> ListAwakenSkillInfo = new List<AwakenSkillInfo>();

    
    //ArenaTower에서 사용하기 위한 CUID (클라이언트 전용)
    public void CreateCUID(long uuid)
    {
        if (CharData == null) return;
        if (CharData.CUID != 0) return;
        CharData.CUID = uuid + CharData.TableID;
    }

    public CardData GetCardData(int index)
    {
        if (CardList == null || CardList.Count == 0 || CardList.Count <= index)
            return null;
        
        return CardList[index];
    }

    public GemData GetMainGemData(int index)
    {
        if (MainGemList == null || MainGemList.Count == 0 || MainGemList.Count <= index)
            return null;

        return MainGemList[index];
    }
}

public partial class TeamData {
	public long                 UUID;
	public long                 Score;                                              //현재 배틀 스코어
	public int                  UserMark;                                           //유저가 등록해논 지휘관 마크
	public int                  Grade;
	public int                  Tier;
	public int                  UserLv;
	public int                  Rank;                                               //현재 순위
	public int                  TeamPower;                                          //해당 팀 전투력
	public bool                 bDetail;                                            // ???
	public List<TeamCharData>   charlist            = new List<TeamCharData>();     //팀에 배치된 팀캐릭터 목록
	public List<BadgeData>      badgelist           = new List<BadgeData>();        //장착한 문양 리스트
	public float                TeamHP;
	public float                TeamATK;
	public int                  CardFormtaionID;
	public int                  UserNickNameColorId = 1;

	private string                      mUserNickName;
	private System.Text.StringBuilder   mSbUserNickName = new System.Text.StringBuilder();


	public void SetUserNickName( string nickName ) {
		mUserNickName = nickName;
        Utility.RemoveBBCode( ref mUserNickName );
	}

	/// <summary>
	/// 컬러 코드가 들어간 유저 닉네임을 얻는 함수
	/// </summary>
	public string GetUserNickName() {
        return Utility.GetColoredNickName( mUserNickName, UserNickNameColorId, mSbUserNickName );
	}
}

//아레나 랭킹 관련
public partial class ArenaRankingListData
{
    public List<TeamData> RankingSimpleList = new List<TeamData>();      // 간단한 랭커 유저 정보
    public long UpdateTM = 0;                                   // 랭킹을 마지막으로 업데이트 한 시간
}

//클라전용
public class AccounLinkData //클라전용
{
    public eAccountType Type;
    public string LinkID;
    public AccounLinkData()
    {

    }
    public AccounLinkData(eAccountType type, string linkid)
    {
        Type = type;
        LinkID = linkid;
    }
}

public class NoticeBaseData //클라전용
{
    public enum eTYPE
    {
        NONE = 0,
        USER_ACHIEVEMENT,
        CHAR_FAVOR,
        CHAR_GRADEUP,
        CHAR_SKILLOPEN,
        CHAR_SkLLSLOTOPEN,
        CHAR_CARDSLOTOPEN,
        FACILITY_OPEN,
        BOOK_OPEN,
        WMISSION_COMPLATE,
        CARD_FAVOR,
        CARD_WAKE,
        WEAPON_WAKE,
        CARD_SKILLLEVELUP,        
        WEAPON_SKILLLEVELUP,
        MAIL,
        BOOK_OPEN_COUNT_CHAR,
        BOOK_OPEN_COUNT_CARD,
        BOOK_OPEN_COUNT_WEAPON,
        BOOK_OPEN_COUNT_MONSTER,
        DISPATCH_OPEN,
        CHAR_AWAKEN,
        BOOK_OPEN_COUNT_COSTUME,
    }

    
    public eTYPE Type;
    public string Text;
    public long uuid;
    public int Value1;
    public int Value2;

    public NoticeBaseData()
    {

    }

    public NoticeBaseData(eTYPE type, string text1, int value1, int value2 )
    {
        Type = type;
        Value1 = value1;
        Value2 = value2;

        if (type != eTYPE.CHAR_AWAKEN)
        {
            Text = string.Format(FLocalizeString.Instance.GetText(4000 + (int)Type), text1);
        }
        else
        {
            Text = string.Format(FLocalizeString.Instance.GetText(4011), text1);
        }
    }

    public NoticeBaseData(eTYPE type, string text1, string text2, int value1, int value2)
    {
        Type = type;
        Value1 = value1;
        Value2 = value2;

        if (type != eTYPE.CHAR_AWAKEN)
        {
            Text = string.Format(FLocalizeString.Instance.GetText(4000 + (int)Type), text1, text2);
        }
        else
        {
            Text = string.Format(FLocalizeString.Instance.GetText(4011), text1, text2);
        }
    }
}

public class NoticeFacilityData //클라전용
{
    public int FacilityID;
    public long RemainTimeTick;

    public NoticeFacilityData()
    {

    }
    public NoticeFacilityData( int tableid, long remaintimetick )
    {
        FacilityID = tableid;
        RemainTimeTick = remaintimetick;
    }

    static public int CompareFuncTick(NoticeFacilityData a, NoticeFacilityData b)
    {
        if (a.RemainTimeTick < b.RemainTimeTick) return -1;
        if (a.RemainTimeTick > b.RemainTimeTick) return 1;
        return 0;
    }
}

public class NoticeDispatchData
{
    public int TableID;
    public int MissionID;
    public long RemainTimeTick;

    public NoticeDispatchData(int tableid, int missionid, long remaintimetick)
    {
        TableID = tableid;
        MissionID = missionid;
        RemainTimeTick = remaintimetick;
    }

    static public int CompareFuncTick(NoticeDispatchData a, NoticeDispatchData b)
    {
        if (a.RemainTimeTick < b.RemainTimeTick) return -1;
        if (a.RemainTimeTick > b.RemainTimeTick) return 1;
        return 0;
    }
}


//PassMission
public partial class PassSetData
{
    public int PassID = 0;
    public bool PassBuyFlag = false;
    public int PassPoint = 0;

    public DateTime PassBuyEndTime;
    public int Pass_NormalReward;
    public int Pass_SPReward;

    public GameTable.PassSet.Param PassTableData;

    public PassSetData()
    {

    }
}

public class PassMissionData
{
    public int PassID = 0;
    public GameTable.PassMission.Param PassMissionTableData;

    public int PassMissionID = 0;
    public int PassMissionValue = 0;
    public int PassMissionState = 0;
}

//Friend
public partial class FriendUserData {
	public enum eFriendFlag {
		TAKE_FP = 0,            //우정포인트 받기
		MY_ROOM_VISIT,          //내 친구목록에서 상대방이 내 프라이빗룸에 들어올수있는지 여부
		FRIEND_ROOM_VISIT,      //내가 상대방 프라이빗룸에 들어갈수있는지 없는지 여부
		_MAX,
	}

	public DateTime LastConnectTime;                //마지막 접속 시간
	public long     UUID;                           //UUID
	public int      UserMark;                       //유저 마커
	public ulong    DBID;                           //DB ID
	public int      Rank;                           //유저 랭크
	public int      RoomSlotNum;                    //프라이빗 룸 슬롯 번호
	public int      ClearArenaTowerID;              //클리어 한 아레타 타워 ID
	public uint     FriendTotalFlag;                //친구 관련 비트 플래그 관리용
	public bool     FriendPointTakeFlag;            //친구 포인드 줄수 있는 지 여부
	public bool     FriendRoomFlagWithMyRoom;       //내 프라이빗룸 접근 가능 여부
	public bool     FriendRoomFlagWithFriendRoom;   //친구 프라이빗룸 접근 가능 여부
	public bool     HasArenaInfo;                   // 아레나 정보 유무
	public int      NickNameColorId;

    private CircleData                  mCircleInfo;        // 서클 데이터
    private CircleAuthLevelData         mCircleAuthLevel;   // 서클 권한

    private string                      mNickName;                                      //닉네임
	private System.Text.StringBuilder   mSbNickName = new System.Text.StringBuilder();

    /// <summary>
    /// 컬러 코드가 안들어간 닉네임 얻는 함수
    /// </summary>
    public string GetRawNickName() {
		return mNickName;
	}

	/// <summary>
	/// 컬러 코드가 안들어간 닉네임의 char 배열을 얻는 함수
	/// </summary>
	public char[] GetRawNickNameCharArr() {
		return mNickName.ToCharArray();
	}

	/// <summary>
	/// 컬러 코드가 들어간 닉네임을 얻는 함수
	/// </summary>
	public string GetNickName() {
		return Utility.GetColoredNickName( mNickName, NickNameColorId, mSbNickName );
	}

    public string GetStringUid()
    {
        return $"ID : {UUID}"; // Test - LeeSeungJin - Change String
    }

    public string GetStringRank()
    {
        return FLocalizeString.Instance.GetText(210, Rank);
    }

    public string GetStringLastLogin()
    {
        // Test - LeeSeungJin - Change String Start
        string lastLoginTime;
        TimeSpan timeSpaned = GameSupport.GetCurrentServerTime() - LastConnectTime;
        if (0 < timeSpaned.Hours)
        {
            lastLoginTime = $"{timeSpaned.Hours} 시간";
        }
        else if (0 < timeSpaned.Minutes)
        {
            lastLoginTime = $"{timeSpaned.Minutes} 분";
        }
        else
        {
            lastLoginTime = $"{timeSpaned.Seconds} 초";
        }

        return $"마지막 로그인 시간 : {lastLoginTime}";
        // Test - LeeSeungJin - Change String End
    }

    public CircleData CircleInfo { get { return mCircleInfo ??= new CircleData(); } }

    public CircleAuthLevelData CircleAuthLevel
    {
        set
        {
            if (mCircleAuthLevel == null)
            {
                mCircleAuthLevel = new CircleAuthLevelData();
            }

            mCircleAuthLevel = value;
        }

        get
        {
            if (mCircleAuthLevel == null)
            {
                mCircleAuthLevel = new CircleAuthLevelData();
            }

            return mCircleAuthLevel;
        }
    }
}

public partial class CommunityData
{
    public List<FriendUserData> FriendList = new List<FriendUserData>();                 //내 친구리스트
    public List<FriendUserData> FriendToAskList = new List<FriendUserData>();            //내가 친구신청 보낸 리스트
    public List<FriendUserData> FriendAskFromUserList = new List<FriendUserData>();      //나한테 온 친구신청 리스트

    public List<FriendUserData> FriendSuggestList = new List<FriendUserData>();          //추천, 검색 리스트

    public System.DateTime FriendSerachTime;                                             //추천 검색 가능 시간 체크용(클라용)
}

public partial class MonthlyData                //월정액 데이터
{
    public DateTime MonthlyEndTime;             //월정액 종료 시간
    public long     MonthlyTableID;             //월정액 테이블 ID
}

public partial class UserMonthlyData
{
    public List<MonthlyData> MonthlyDataList = new List<MonthlyData>();
}

public partial class BuffEffectData
{
    public DateTime BuffEndTime;                //버프 종료 시간
    public long     BuffTableID;                //버프 테이블 ID

    public eBuffEffectType BuffEffectType;
    public GameTable.Buff.Param TableData;
}

public partial class UserBuffEffectData
{
    public List<BuffEffectData> BuffEffectDataList = new List<BuffEffectData>();
}

//Dispatch
public class Dispatch
{
    public DateTime EndTime;
    public int TableID;
    public int MissionID;

    public GameTable.CardDispatchMission.Param TableData;
    public List<CardData> UsingCardData;

    public Dispatch() { }
    public Dispatch(PktInfoDispatch.Piece piece)
    {
        SetDispatchData(piece);
    }
    public Dispatch(int mid, int tid, DateTime etime)
    {
        TableID = tid;
        MissionID = mid;
        EndTime = etime;

        RefreshData();
    }

    public void SetDispatchData(PktInfoDispatch.Piece piece)
    {
        EndTime = GameSupport.GetLocalTimeByServerTime(piece.operEndTime_.GetTime());
        TableID = (int)piece.tableID_;
        MissionID = (int)piece.missionID_;

        RefreshData();
    }

    public void RefreshData()
    {
        TableData = GameInfo.Instance.GameTable.FindCardDispatchMission(x => x.ID == MissionID);
        UsingCardData = GameInfo.Instance.CardList.FindAll(x =>
            x.PosKind == (int)eContentsPosKind.DISPATCH &&
            x.PosValue == TableID);
    }
}

//LobbyTheme
public class LobbyThemeData
{
    public ulong SysFlag;
    public ulong Flag0;
    public ulong Flag1;

    public LobbyThemeData() { }
    public LobbyThemeData(PktInfoLobbyTheme pkt) 
    {
        SysFlag = pkt.sysFlag_;
        Flag0 = pkt.flags_[(int)PktInfoLobbyTheme.FLAG._0];
        Flag1 = pkt.flags_[(int)PktInfoLobbyTheme.FLAG._1];
    }
}

public partial class WeaponArmoryData
{
    public int ArmorySlotCnt;
    public List<long> ArmoryWeaponUIDList = new List<long>();
}

public class ArenaTowerFriendContainer
{
    private Dictionary<int, long> _Container = new Dictionary<int, long>();
    public int Count
    {
        get
        {
            if (_Container == null) return 0;
            return _Container.Count;
        }
    }

    public void Clear() { _Container.Clear(); }
    public void Add(int key, long value)
    {
        if (_Container.ContainsKey(key))
            _Container[key] = value;
        else
            _Container.Add(key, value);
    }

    public  bool TryGetValue(int key, out long value)
    {
        return _Container.TryGetValue(key, out value);
    }
    public void Modify(int key, long value)
    {
        if (_Container.ContainsKey(key))
            _Container[key] = value;
    }

    public bool ContainsKey(int key)
    {
        return _Container.ContainsKey(key);
    }

    public void Remove(int key)
    {
        _Container.Remove(key);
    }

    public void SwapReplace(int k1, long v1)
    {
        if (!_Container.ContainsKey(k1))
        {
            AllRemoveByValue(v1);
            _Container.Add(k1, v1);
            return;
        }

        int k2 = GetKeyByValue(v1);
        long v2 = _Container[k1];

        if (k2 < 0)
        {
            _Container.Add(k1, v1);
            return;
        }

        _Container[k1] = v1;
        _Container[k2] = v2;
    }

    /// <summary>
    /// 배치
    ///   - key,value가 같은 데이터는 Remove
    ///   - 중복된 Value는 모두 삭제 후, 신규 key에 Add
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Assign(int key, long value)
    {
        //1.key에 저장된 value 얻기
        long oldValue = 0;
        TryGetValue(key, out oldValue);
        if (value == oldValue)
            Remove(key);
        else
        {
            //value와 같은 데이터는 모두 삭제
            List<int> removeKeys = new List<int>();
            var iterA = _Container.GetEnumerator();
            while(iterA.MoveNext())
            {
                if (iterA.Current.Value == value)
                    removeKeys.Add(iterA.Current.Key);
            }

            var iterB = removeKeys.GetEnumerator();
            while (iterB.MoveNext())            
                Remove(iterB.Current);

            //신규 데이터 추가
            Add(key, value);
        }
    }

    /// <summary>
    /// [아레나 타워] 친구 캐릭터 카운트
    /// </summary>
    /// <returns></returns>
    public int ValidCount()
    {
        int result = 0;
        var iter = _Container.GetEnumerator();
        while(iter.MoveNext())
        {
            if (iter.Current.Value != 0)
                result++;
        }
        return result;
    }

    public bool HasValue(long v)
    {
        var iter = _Container.GetEnumerator();
        while(iter.MoveNext())
        {
            if (iter.Current.Value == v)
                return true;
        }

        return false;
    }

    public void AllRemoveByValue(long value)
    {
        //value와 같은 데이터는 모두 삭제
        List<int> removeKeys = new List<int>();
        var iterA = _Container.GetEnumerator();
        while (iterA.MoveNext())
        {
            if (iterA.Current.Value == value)
                removeKeys.Add(iterA.Current.Key);
        }

        var iterB = removeKeys.GetEnumerator();
        while (iterB.MoveNext())
            Remove(iterB.Current);
    }

    public long GetValidValue()
    {
        var iterA = _Container.GetEnumerator();
        while (iterA.MoveNext())
        {
            if (iterA.Current.Value > 0)
                return iterA.Current.Value;
        }

        return 0;
    }

    public int GetKeyByValue(long value)
    {
        var iter = _Container.GetEnumerator();
        while (iter.MoveNext())
        {
            if (iter.Current.Value == value)
                return iter.Current.Key;
        }

        return -1;
    }
}

public class AllyPlayerData
{
    public CharData     MyCharData      = null;
    public TeamCharData FriendCharData  = null;


    public AllyPlayerData(CharData myCharData, TeamCharData friendCharData)
    {
        MyCharData = myCharData;
        FriendCharData = friendCharData;
    }

    public bool IsEmpty()
    {
        return MyCharData == null && FriendCharData == null;
    }

    public CharData GetCharDataOrNull()
    {
        if(MyCharData != null)
        {
            return MyCharData;
        }
        else if(FriendCharData != null)
        {
            return FriendCharData.CharData;
        }

        return null;
    }
}

public partial class AwakenSkillInfo
{
    public int TableId  { get; private set; } = 0;
    public int Level    { get; private set; } = 0;


    public void SetLevel(int level)
    {
        Level = level;
    }
}
public class DailyMissionData
{
    public class Piece
    {
        public DateTime StartTime;
        public DateTime EndTime;
        public uint GroupID;
        public uint RwdFlag;
        public byte[] NoVal = new byte[(int)PktInfoMission.Daily.Piece.ENUM._NO_END_];
        public byte Day;

        public Piece() {}
        public Piece(PktInfoMission.Daily.Piece p)
        {
            StartTime = GameSupport.GetLocalTimeByServerTime(p.startTM_.GetTime());
            EndTime = GameSupport.GetLocalTimeByServerTime(p.endTM_.GetTime());
            GroupID = p.groupID_;
            RwdFlag = p.rwdFlag_;
            p.noVal_.CopyTo(NoVal, 0);
            Day = p.day_;
        }
    }

    public List<Piece> Infos;

    public DailyMissionData() { }

    public void ApplyPktData(PktInfoMission.Daily pktDaily)
    {
        if (Infos == null) Infos = new List<Piece>();

        if (pktDaily.infos_ == null || pktDaily.infos_.Count <= 0)
            return;

        for (int i = 0; i < pktDaily.infos_.Count; i++)
        {
            Piece newPiece = new Piece(pktDaily.infos_[i]);

            if (Infos.Find(x => x.GroupID == newPiece.GroupID && x.Day == newPiece.Day) == null)
            {
                Infos.Add(newPiece);
            }
            else
            {
                for (int j = 0; j < Infos.Count; j++)
                {
                    if (Infos[j].GroupID == newPiece.GroupID && Infos[j].Day == newPiece.Day)
                    {
                        Infos[j] = newPiece;
                        continue;
                    }
                }
            }
        }
    }
}


public partial class InfluenceMissionData
{   
    public uint GroupID;          // 서버 달성 미션 테이블 ID
    public uint InfluID;          // 세력 ID
    public uint RwdFlag;          // 각각의 미션별 보상 획득 여부
    public uint TgtRwdFlag;       // 각각의 달성 및 랭킹 보상 획득 여부
    public byte[] Val = new byte[(int)PktInfoMission.Influ.ENUM._NO_END_];    // 미션별 수행 값
}

public partial class InfluenceData
{
    public class Piece
    {
        public ulong Point;
        public uint InfluID;
        public uint UserCount;

        public Piece() { }
        public Piece(PktInfoInfluence.Piece p)
        {
            Point = p.point_;
            InfluID = p.influID_;
            UserCount = p.userCnt_;
        }
    }
    public ulong TotalPoint;
    public List<Piece> Infos = new List<Piece>();
}

public partial class InfluenceRankData {
	public class Piece {
		public ulong    UUID;                   // 유저 고유 ID
		public uint     MarkID;                 // 유저 마크 ID
		public uint     InfluencePoint;         // 서버 달성(세력) 획득 점수
		public byte     Rank;                   // 랭킹
		public byte     Level;                  // 유저 레벨
		public int      NickNameColorId = 1;    // 유저 닉네임 컬러 인덱스

		private string                      mNickName;
		private System.Text.StringBuilder   mSbNickName = new System.Text.StringBuilder();


		public Piece() { 
        }

		public Piece( PktInfoRankInfluence.Piece p ) {
            mNickName = p.name_.str_;
			UUID = p.uuid_;
			MarkID = p.mark_;
			InfluencePoint = p.influPT_;
			Rank = p.rank_;
			Level = p.lv_;
            NickNameColorId = (int)p.nickNameColorID_;
		}

        /// <summary>
        /// 컬러 코드가 들어간 닉네임을 얻는 함수
        /// </summary>
        public string GetNickName() {
            return Utility.GetColoredNickName( mNickName, NickNameColorId, mSbNickName );
        }
    }

	public List<Piece>  Infos           = new List<Piece>();
	public PktInfoTime  LaseUpdateTime  = new PktInfoTime();    // 마지막으로 갱신된 시간
	public uint         IdInRankTable;                          // 자신이 속한 랭크 테이블 안의 ID
	public byte         NowRank;                                // 현재 자신의 랭크    
}

public partial class RotationGachaData
{
    public class Piece
    {
        public DateTime Time;
        public int RemainCount;

        public Piece() { }
        public Piece(PktInfoComTimeAndTID.Piece p)
        {
            //Time = p.tm_.GetTime();
            Time = GameSupport.GetLocalTimeByServerTime(p.tm_.GetTime());
            RemainCount = (int)p.tid_;
        }

        public void UpdatePiece(PktInfoComTimeAndTID.Piece p)
        {
            //Time = p.tm_.GetTime();
            Time = GameSupport.GetLocalTimeByServerTime(p.tm_.GetTime());
            RemainCount = (int)p.tid_;
        }
    }

    public List<Piece> Infos = new List<Piece>();
}

public partial class UnexpectedPackageData
{
    public int TableId;         // TID
    public DateTime EndTime;    // 이벤트 종료 시간
    public byte RepeatCount;    // 이벤트 반복 횟수
    public byte RewardBitFlag;  // 일자별 보상 획득 여부 비트플래그 (0: 미수령, 1:수령 )
    public bool IsPurchase;     // 구매 여부
    public bool IsAdd;          // 추가 여부
    
    public UnexpectedPackageData(PktInfoUnexpectedPackage.Piece piece, bool isAdd = false)
    {
        TableId = (int)piece.tableID_;
        UpdateData(piece, isAdd);
    }

    public void UpdateData(PktInfoUnexpectedPackage.Piece piece, bool isAdd = false)
    {
        EndTime = GameSupport.GetLocalTimeByServerTime(piece.endTime_.GetTime());
        RepeatCount = piece.repeatCount_;
        RewardBitFlag = piece.rewardBitFlag_;
        IsPurchase = piece.isPurchase_;
        IsAdd = isAdd;
    }
}

public partial class EventData
{
    public int GroupID;
    public int No;
    public int RwdFlag;

    public EventData() { }
    public EventData(PktInfoUserBingoEvent.Piece piece)
    {
        Copy(piece);
    }

    public void Copy(PktInfoUserBingoEvent.Piece piece)
    {
        GroupID = (int)piece.GroupID;
        No = (int)piece.No;
        RwdFlag = (int)piece.RwdFlag;
	}
}

public partial class PresetWeaponData
{
    public long Uid;
    public int SkinTid;
    public long[] GemUids = new long[(int)eCOUNT.WEAPONGEMSLOT];

    public void Init()
    {
        Uid = 0;
        SkinTid = 0;
        for (int i = 0; i < GemUids.Length; i++)
        {
            GemUids[i] = 0;
        }
    }
}

public partial class PresetCharData
{
    public long CharUid;
    public int CostumeId;
    public int CostumeStateFlag;
    public int CostumeColor;
    public PresetWeaponData[] WeaponDatas = new PresetWeaponData[(int)eWeaponSlot.Count];
    public long[] CardUids = new long[(int)eCARDSLOT._MAX_];
    public int[] SkillIds = new int[(int)eCOUNT.SKILLSLOT];

    public PresetCharData()
    {
        for (int i = 0; i < WeaponDatas.Length; i++)
        {
            WeaponDatas[i] = new PresetWeaponData();
        }
    }

    public void Init()
    {
        CharUid = 0;
        CostumeId = 0;
        CostumeStateFlag = 0;
        CostumeColor = 0;

        for (int i = 0; i < CardUids.Length; i++)
        {
            CardUids[i] = 0;
        }

        for (int i = 0; i < WeaponDatas.Length; i++)
        {
            WeaponDatas[i].Init();
        }

        for (int i = 0; i < SkillIds.Length; i++)
        {
            SkillIds[i] = 0;
        }
    }
}

public partial class PresetData
{
    public string PresetName;
    public PresetCharData[] CharDatas = new PresetCharData[(int)eArenaTeamSlotPos._MAX_];
    public long[] BadgeUdis = new long[(int)eCOUNT.BADGESLOT];
    public int CardTeamId;

    public PresetData()
    {
        for (int i = 0; i < CharDatas.Length; i++)
        {
            CharDatas[i] = new PresetCharData();
        }

        Init();
    }

    public void Init()
    {
        PresetName = "";

        for (int i = 0; i < CharDatas.Length; i++)
        {
            CharDatas[i].Init();
        }

        for (int i = 0; i < BadgeUdis.Length; i++)
        {
            BadgeUdis[i] = 0;
        }

        CardTeamId = 0;
    }
}

public partial class RaidUserData {
    public List<long>               CharUidList                 = new List<long>( 3 );
    public int                      CardFormationId             = 0;    
    public List<RaidClearData>      StageClearList              = new List<RaidClearData>();
    public DateTime                 LastPlayedSeasonEndTime;                                    // 마지막에 플레이한 시즌 종료 시간
    public int                      DailyRaidPoint              = 0;
    public int                      CurStep                     = 0;
    public GameTable.Stage.Param    CurStageParam               = null;


    public bool NeedInitSeasonData() {
        return ( LastPlayedSeasonEndTime < GameInfo.Instance.ServerData.RaidSeasonEndTime );
	}
}

public partial class RaidClearData {
    public int                      StageTableID;
    public int                      Step;
    public uint                     HighestScore;
    public long                     Cuid;
    public GameTable.Stage.Param    TableData;


    public RaidClearData() {
    }

    public RaidClearData( PktInfoRaidStageRec.Piece pkt ) {
        SetPktData( pkt );
    }
}

public partial class RaidRankData {
    public int                          StageTableID;
    public GameTable.Stage.Param        StageTableData;
    public long                         UpdateTM;
    public int                          Step;
    public List<TimeAttackRankUserData> RankUserList = new List<TimeAttackRankUserData>();
    

    public RaidRankData() {
    }

    public RaidRankData( PktInfoRaidRankingHeader.Piece raidRankingHeaderPiece ) {
        SetPktData( raidRankingHeaderPiece );
    }

    public TimeAttackRankUserData GetRankUserDataOrNull( long userUid, bool findFirstClearUser = false ) {
        TimeAttackRankUserData data = RankUserList.Find( x => x.UUID == userUid && x.IsRaidFirstRanker == findFirstClearUser );
        return data;
	}
}

public partial class CircleData
{
    public long Uid;
    public int FlagId;
    public int MarkId;
    public int ColorId;
    public int Rank;
    public int LobbySetId;
    public int AttendanceCount;
    public int MemberCount;
    public int MemberMaxCount;
    public int SubLeaderCount;
    public int SubLeaderMaxCount;
    public string Name;
    public string Content;
    public eLANGUAGE MainLanguage;
    public bool IsOtherLanguage;
    public DateTime RecentAttendRewardTime;
    public DateTime NextUserKickPossibleTime;

    private long[] _goods;
    private long[] _ownerFlagBit;
    private long[] _ownerMarkBit;
    private long[] _ownerColorBit;

    private CircleLeaderData _leaderData;
    private List<FriendUserData> _memberList;
    private List<FriendUserData> _joinWaitList;

    public long[] Goods
    {
        set
        {
            if (_goods == null)
            {
                _goods = new long[(int)eCircleGoodsType.COUNT];
            }

            _goods = value;
        }

        get
        {
            if (_goods == null)
            {
                _goods = new long[(int)eCircleGoodsType.COUNT];
            }

            return _goods;
        }
    }

    public long[] FlagBit
    {
        set { _ownerFlagBit ??= new long[(int)PktInfoCircleMark.FLAG._MAX_];  _ownerFlagBit = value; }
        get { return _ownerFlagBit ??= new long[(int)PktInfoCircleMark.FLAG._MAX_];  }
    }
    public long[] MarkBit
    {
        set { _ownerMarkBit ??= new long[(int)PktInfoCircleMark.FLAG._MAX_]; _ownerMarkBit = value; }
        get { return _ownerMarkBit ??= new long[(int)PktInfoCircleMark.FLAG._MAX_]; }
    }
    public long[] ColorBit
    {
        set { _ownerColorBit ??= new long[(int)PktInfoCircleMark.FLAG._MAX_]; _ownerColorBit = value; }
        get { return _ownerColorBit ??= new long[(int)PktInfoCircleMark.FLAG._MAX_]; }
    }

    public CircleLeaderData Leader
    {
        get
        {
            if (_leaderData == null)
            {
                _leaderData = new CircleLeaderData();
            }

            return _leaderData;
        }
    }

    public List<FriendUserData> MemberList
    {
        get
        {
            if (_memberList == null)
            {
                _memberList = new List<FriendUserData>();
            }

            return _memberList;
        }
    }

    public List<FriendUserData> JoinWaitList
    {
        get
        {
            if (_joinWaitList == null)
            {
                _joinWaitList = new List<FriendUserData>();
            }

            return _joinWaitList;
        }
    }

    public void SetGoods(ulong[] goods)
    {
        for (int i = 0; i < goods.Length; i++)
        {
            if (Goods.Length <= i)
            {
                break;
            }

            Goods[i] = (long)goods[i];
        }
    }

    public string GetStringUid()
    {
        return $"ID : {Uid}"; // Test - LeeSeungJin - Change String
    }

    public string GetStringRank()
    {
        return FLocalizeString.Instance.GetText(210, Rank);
    }

    public string GetStringMemberCount()
    {
        return FLocalizeString.Instance.GetText(278, MemberCount, 50); // Test - LeeSeungJin - Temp
    }

    public string GetStringMainLang()
    {
        return FLocalizeString.Instance.GetText((int)MainLanguage + 601); // Test - LeeSeungJin - Change String
    }

    public int GetIntOtherLang()
    {
        return IsOtherLanguage ? (int)eToggleType.On : (int)eToggleType.Off;
    }
}

public partial class CircleLeaderData
{
    public long Uid;
    public int MarkId;
    public int Rank;
    public string Name;
    public DateTime LastLoginTime;

    private CircleAuthLevelData _authLevel;

    public CircleAuthLevelData AuthLevel
    {
        get
        {
            if (_authLevel == null)
            {
                _authLevel = new CircleAuthLevelData();
            }

            return _authLevel;
        }
    }

    public string GetStringUid()
    {
        return $"ID : {Uid}"; // Test - LeeSeungJin - Change String
    }

    public string GetNickName()
    {
        return Name;
    }

    public string GetStringRank()
    {
        return FLocalizeString.Instance.GetText(210, Rank);
    }
}

public partial class CircleAuthLevelData
{
    public eCircleAuthLevel AuthLevel;

    /// <summary>
    /// 대상보다 권한이 초과임
    /// </summary>
    /// <param name="targetAuthLevel"></param>
    /// <returns></returns>
    public bool IsExcess(eCircleAuthLevel targetAuthLevel)
    {
        return AuthLevel > targetAuthLevel;
    }

    /// <summary>
    /// 대상보다 권한이 이하임
    /// </summary>
    /// <param name="targetAuthLevel"></param>
    /// <returns></returns>
    public bool IsLessThan(eCircleAuthLevel targetAuthLevel)
    {
        return AuthLevel <= targetAuthLevel;
    }

    /// <summary>
    /// 대상과 권한이 같지 않음
    /// </summary>
    /// <param name="targetAuthLevel"></param>
    /// <returns></returns>
    public bool IsNotEqual(eCircleAuthLevel targetAuthLevel)
    {
        return AuthLevel != targetAuthLevel;
    }

    /// <summary>
    /// 대상과 권한이 같음
    /// </summary>
    /// <param name="targetAuthLevel"></param>
    /// <returns></returns>
    public bool IsEqual(eCircleAuthLevel targetAuthLevel)
    {
        return AuthLevel == targetAuthLevel;
    }
}

public partial class CircleAttendanceData
{
    public int RewardGroupId;
    public int RewardCount;
    public DateTime LastCheckDate;
}

public partial class CircleChatData
{
    public long Uid;
    public string UserName;
    public string Content;
    public int UserMarkId;
    public int StampId;
    public DateTime ChatTime;
}

public partial class CircleNotiData
{
    public eCircleNotiType NotiType;
    public string NickName;
    public DateTime SendTime;

    private List<long> _valueList;

    public List<long> Values
    {
        get
        {
            if (_valueList == null)
            {
                _valueList = new List<long>();
            }

            return _valueList;
        }
    }
}

public partial class RecoverySelectData {
	public long UID;
	public int Count;
	public int Value;
}