// 타입 관련 정의 위키 링크 -> http://10.10.10.90/Mediawiki/index.php/Project-A/일반/ProjectDefine

// 제화 타입 위키 링크 -> http://10.10.10.90/Mediawiki/index.php/Project-A/일반/ProjectDefine/eGOODSTYPE
public enum eGOODSTYPE ////ProductIndex, PurchaseIndex
{
    NONE = -1,       //
    GOLD = 0,        //골드
    CASH,                           //캐쉬
    SUPPORTERPOINT,                 //서포트포인트
    ROOMPOINT,                      //룸포인트
    AP,                             //티겟
    BP,                             //배틀 티켓
    BATTLECOIN,
    FRIENDPOINT,
    DESIREPOINT,                    // 염원의 기운
    AWAKEPOINT,                     //각성 포인트
    RAIDPOINT,                      // 레이드 포인트
	CIRCLEPOINT,                    //서클 포인트
	COUNT,// = BATTLECOIN,  // 아직 서버작업이 완료 된 상태가 아니기 때문에 BP 전까지를 GOODS 최대 값으로 설정합니다.
    GOODS = 100,      //실재화
    KRW,
    JPN,
    USD,
}

public enum eCircleGoodsType
{
	CIRCLE_GOLD = 0,  //골드
	COUNT
};

// 제품 분류 위키 링크 -> http://10.10.10.90/Mediawiki/index.php/Project-A/일반/ProjectDefine/eREWARDTYPE
public enum eREWARDTYPE //ProductType, PurchaseType, RewardType
{
    GACHA               = 0,    //가챠
    GOODS,                      //재화
    CHAR,                       //캐릭터
    WEAPON,                     //무기
    GEM,                        //곡옥
    CARD,                       //카드
    COSTUME,                    //코스튬
    ITEM,                       //아이템
    ROOMTHEME,                  //룸테마    
    ROOMFUNC,                   //룸기능
    ROOMACTION,                 //룸액션
    ROOMFIGURE,                 //룸피규어
    USERMARK,                   //유저아이콘
    PACKAGE,                    //묶음상품
    BADGE,                      //문양
    PASS,                       //패스
    MONTHLYFEE,                 //월 정액 아이템
    BUFF,
    //ITEMINVEN,      //아이템인벤토리
    LOBBYTHEME,                 //로비 테마
    LOBBYANIMATION,              //로비 애니메이션
    DAILYPACKAGE,               //일자 보상 포함 패키지
	CHATSTAMP,                  //채팅 스탬프
	_MAX_,

	_CIRCLE_START_ = 200,
	CIRCLE_GOODS = _CIRCLE_START_,      // 서클 재화
	CIRCLE_MARK,						// 서클 마크
	_CIRCLE_END_
};

public enum eGEMCOLOR
{
    _NONE_              = 0,

    _START_COLOR_       = 1,
    GREEN_HP            = _START_COLOR_,
    RED_ATK,
    BLUE_DEF,
    YELLOW_CRI,
    RAINBOW,
    _END_COLOR_         = RAINBOW,

    _GEMCOLOR_MAX_,
};

public enum eRoomActionType
{
    _ACTION_TYPE_NONE_  = 0,

    FACE,
    BODY,

    _ACTION_TYPE_MAX_,
};

public enum eSTAGETYPE
{
    STAGE_NONE          = 0,
    STAGE_MAIN_STORY,
    STAGE_CHAR_STORY,
    STAGE_DAILY,
    STAGE_EVENT,
    STAGE_SPECIAL,
    STAGE_TIMEATTACK,
    STAGE_PVP,
    STAGE_TRAINING,
    STAGE_PVP_PROLOGUE,
    STAGE_EVENT_BONUS,
	STAGE_SECRET,
    STAGE_RAID,

    STAGE_TOWER = 99,
    STAGE_TEST,

    _MAX_,
}

// Addon아이템 타입 
enum eTRADE_ADDON_TYPE
{
    ADDON_NONE = 0,    // 조건 없음

    PROBABILITY_UP = 1,     // 확률 증가
    DECOMPOSITION = 2,      // 분해 아이템 추가 획득
    SUPPORT_TYPE_SET = 3,   // 서포터 속성 지정
    WEAPON_CHAR_SET = 4,    // 무기 대마인 지정

    _ADDON_MAX_,
}

public enum eSTAGE_SPECIAL_TYPE
{
    NONE = 0,
    BIKE = 1,
    THROW = 2,
    _MAX,
}

// 스테이지 조건 보상 타입
public enum eSTAGE_CONDI
{
    NONE                = 0,    // 조건 없음

    ASSIST_CARD_CNT,            // 지원 서포터 배치 n명
    PROTECT_CARD_CNT,           // 보호 서포터 배치 n명
    DOMINATE_CARD_CNT,          // 제압 서포터 배치 n명

    _STAGE_CONDI_MAX_,

    INFLUENCE_CONDI = 11,       // 서버 달성이벤트(세력참가)

    NOT_CHECK_CONDI = 99,        // 조건 없이 지급
};

public enum eITEMTYPE
{
    MATERIAL            = 0,
    USE,
    EVENT,
}

public enum eITEMSUBTYPE
{
    MATERIAL_WEAPON_LEVELUP = 0,
    MATERIAL_CARD_LEVELUP,
    MATERIAL_GOLD_BAR,
    MATERIAL_MIX_CARD_SLVUP,
    MATERIAL_MIX_WEAPON_SLVUP,
    MATERIAL_FACILITY_UPGRADE,
    MATERIAL_CHAR_WAKEUP_BASE,
    MATERIAL_WEAPON_WAKEUP_BASE,
    MATERIAL_CHRWPN_WAKEUP_TYPE,
    MATERIAL_CHRWPN_WAKEUP_SPECIAL,
    MATERIAL_FACILITY_TIME,
    MATERIAL_CARD_SLVUP,
    MATERIAL_WEAPON_SLVUP,
    MATERIAL_GEM_WAKEUP,
    MATERIAL_CARD_WAKEUP_BASE,
    MATERIAL_CARD_WAKEUP_TYPE,
    MATERIAL_CARD_WAKEUP_SPECIAL,
    MATERIAL_BADGE_LEVELUP,
    MATERIAL_BADGE_INIT_LV,
    MATERIAL_ARENA_ATKBUFF,
    MATERIAL_ARENA_DEFBUFF,
    MATERIAL_SPECIAL_CHANGE,
    MATERIAL_SPECIAL_RESET,
    MATERIAL_FRIEND_METAL,
    MATERIAL_FRIEND_FIBER,
    MATERIAL_CHAR_TOKEN,
    MATERIAL_FRIEND_CHAR_USE,

    MATERIAL_PREFERENCE_PRESENT = 32,

    MATERIAL_RAID_ATKBUFF = 37,
    MATERIAL_RAID_HPBUFF = 38,
    MATERIAL_RAID_POTION = 39,

    USE_AP_CHARGE = 0,
    USE_BP_CHARGE,
    USE_GACHA_TICKET,
    USE_STORE_PACKAGE,
    USE_SELECT_ITEM,
    USE_RANDOM_ITEM,
    USE_PROMOTION_ITEM,
    USE_PACKAGE_ITEM,
    USE_ADDITEMSLOT,
    USE_PUCHASE_ROOM,
	USE_EMPTY,
	USE_CHARACTER_GRADE_UP,
    USE_RESET_ALL_AWAKEN_Skill,
	USE_AP_CHARGE_NUM,
	USE_BP_CHARGE_NUM,

	EVENT_GACHA_TICKET = 0,
}

public enum eBuyType
{
    Single = 0,
    Multiple,
}

public enum eCHARSKILLPASSIVETYPE
{
    UPGRADE_NORMAL      = 0,
    UPGRADE_ULTIMATE,
    SELECT_NORMAL,
    SELECT_ULTIMATE,
    CONDITION_SKILL,
    _MAX_,
}

public enum eBookGroup
{
    None                = 0,

    Weapon,
    Supporter,
    Monster,
    Costume,
}

// 성장 상태 위키 링크 -> http://10.10.10.90/Mediawiki/index.php/Project-A/일반/ProjectDefine/eGrowState
public enum eGrowState : System.Byte
{
    SUCC_NOR            = 0,    // 일반 성공
    SUCC_GRATE,                 // 대성공
    SUCC_OVER_GRATE,            // 초 대성공

    FAIL,                       // 실패

    _MAX_
};

public enum eCostumeStateFlag
{
    CSF_WEAPON          = 0,            //무기 오리지널무기 사용여부
    CSF_HAIR,                           //헤어 오리지널헤어 사용여부
    CSF_ATTACH_1,                       //부착 아이템 사용여부 (안경 등)
    CSF_ATTACH_2,                       //부착 아이템 사용여부 (아스카 팔,다리)
    CSF_ATTACH_3,                       //부착 아이템 사용여부
    CSF_ATTACH_4,                       //부착 아이템 사용여부
    CSF_ATTACH_5,                       //부착 아이템 사용여부
    _MAX_
}

public enum eBookStateFlag
{
    NEW_CHK             = 0,        // 신규 획득 확인 여부
    MAX_WAKE_AND_LV,                // 최대 각성 및 레벨 달성 여부
    MAX_FAVOR_LV,                   // 최대 호감도 달성 여부
    FAVOR_RWD_GET_1 = 11,           // 호감도 보상1 획득 여부
    FAVOR_RWD_GET_2,                // 호감도 보상2 획득 여부
    FAVOR_RWD_GET_3,                // 호감도 보상3 획득 여부
    FAVOR_RWD_GET_4,                // 호감도 보상4 획득 여부
    FAVOR_RWD_GET_5,                // 호감도 보상5 획득 여부
    _MAX_
}

// 미션 타입 위키 링크 -> http://10.10.10.90/Mediawiki/index.php/Project-A/일반/ProjectDefine/eMISSIONTYPE
public enum eMISSIONTYPE
{
    _NONE_              = 0,
    WM_Con_WpnUpgrade,              //  무기 강화 {0}회
    WM_Con_SptUpgrade,              //  서포터 강화 {0}회
    WM_Con_GemUpgrade,              //  곡옥 강화 {0}회
    WM_Con_MainStageClear,          //  메인 스테이지 완료 {0}회
    WM_Con_DailyStageClear,         //  요일 스테이지 완료 {0}회
    WM_Con_DailyLogin,              //  일일 접속 {0}회
    WM_Con_ItemSell,                //  아이템 판매 {0}회
    WM_Con_StageGetItem,            //  스테이지 아이템 획득 {0}회
    WM_Con_GachaGetItem,            //  가챠 아이템 획득 {0}회
    WM_Con_SendFriendPoint,         //  친구 포인트 전송 {0}회
    WM_Con_FacilityGetItem,         //  아이템 조합 {0}회
    WM_Con_WpnSLvUP,                //  무기 스킬 업
    WM_Con_SptSLvUP,                //  서포터 스킬 업
    WM_ArenaWin,                    //  배틀 아레나 승리
    WM_ArenaPlay,                   //  배틀 아레나 플레이
    WM_FacilityCHAR_EXP,            //  강화 훈련 시설
    WM_FacilityCHAR_SP,             //  기술 훈련 시설
    WM_FacilityWEAPON_EXP,          //  무기 강화 시설
    WM_FacilityCARD_TRADE,          //  교환 시설
    WM_FacilityOPER_ROOM,           //  작전회의 시설

    _MAX_,
}

// 게릴라 미션 타입
public enum eGuerrillaMissionType
{
    NONE                = 0,
    GM_LoginBonus,              // 로그인 보너스
    GM_MailReward,              // 메일 전체 보상
    GM_Upgrade_Cnt,             // 서포터, 무기, 곡옥, 문양 강화
    GM_BuyStoreID_Cnt,          // 특정 상품 구매
    GM_BuyStoreGacha_Cnt,       // (가챠)인 상품 구매
    GM_BuyStoreItem_Cnt,        // (아이템)인 상품 구매
    GM_StageClear_Cnt,          // 특정 게임 모드 클리어
    GM_StageClearDiff_Cnt,      // 특정 난이도 클리어
    GM_CharStageClear_Cnt,      // 특정 캐릭터로 게임 모드 클리어
    GM_FacilityOperation_Cnt,   // 특정 시설 이용
    GM_FacilityStart_Cnt,       // 특정 시설 시작
    GM_UseGoods_Amount,         // 특정 재화 소모
    GM_ArenaWin_Cnt,            // 배틀 아레나 승리
    GM_ArenaPlay_Cnt,           // 배틀 아레나 플레이
	GM_StageClearID_Cnt,		// 특정 스테이지 클리어
    GM_SptTIDWake_Cnt,          // 특정 서포터 각성
    GM_SptTIDFavor_Cnt,         // 특정 서포터 호감도
    GM_GueCondi_Make,           // 특정 게릴라 미션의 시작 조건 생성
    GM_GueCondi_LoginBonusDate, // 게릴라 미션이 조건으로 동작되는 기간 한정 출석 체크 방식의 이벤트
	GM_Decomposition_Cnt,       // 무기 및 서포터 분해 미션

	_MAX_
};

// 상점 세일 분류
public enum eStoreSaleKind : System.Byte
{
    CycleMinute         = 0,        // 시간 주기(단위 = 분)
    LimitCnt,                       // 횟수 제한
    LimitDate = 2,
    LimitDate_Day = 2,                  // 일간 갱신
    LimitDate_Weekly,               // 주간 갱신
    LimitDate_Monthly,              // 월간 갱신
    _MAX_,
};

public enum eBannerType : System.Byte
{
    ROLLING             = 0,	// MainPanel 의 롤링배너
    PACKAGE_BANNER,             // 패키지팝업의 패키지 상품 배너(기존에는 아틀라스와 텍스쳐 같은이름사용)
    PACKAGE_BG,                 // 패키지팝업의 패키지 상품 BG
    EVENT_MAINBG,               // 이벤트 MainPanel BG
    EVENT_BANNER_NOW,           // 이벤트 EventModePopup 활성 배너
    EVENT_BANNER_PAST,          // 이벤트 EventModePopup 비활성 배너
    EVENT_STAGEBG,              // 이벤트 StagePanel BG
    EVENT_RULEBG,               // 이벤트 EventRulePopup BG
    GLLA_MISSION_LOGIN,         // 게릴라 미션 로그인 보너스용
    EVENT_LOGO,                 // 이벤트 로고
    LOGIN_PACKAGE_BG,           // 로그인 보너스 이후에 나오는 패키지 광고 이미지(글로벌 전용)
    SPECIAL_BUY,                // 글로벌에서 사용 - 1$짜리 특별 구매 팝업
    SOCIAL,
    UNEXPECTED_PACKAGE,             // 돌발 패키지
    UNEXPECTED_PACKAGE_BANNER,      // 돌발 패키지 배너
	LOGIN_EVENT_BG,				// 로그인 이벤트 배경
}

// 이벤트 보상 분류 위키 링크 -> http://10.10.10.90/Mediawiki/index.php/Project-A/일반/ProjectDefine/eEventRewardKind
public enum eEventRewardKind
{
    _NONE_              = 0,

    RESET_LOTTERY,      // 리셋 뽑기형
    MISSION,            // 미션 완료형
    EXCHANGE,           // 아이템 교환형
    _MAX_
}

// 콘텐츠 위치 분류 위키 링크 -> http://10.10.10.90/Mediawiki/index.php/Project-A/일반/ProjectDefine/eContentsPosKind
public enum eContentsPosKind : System.Byte
{
    _NONE_              = 0,    // 사용되지 않음.
    _START_             = 1,

    CHAR                = _START_,
    FACILITY,
    COSTUME,
    MONSTER,
    CARD,
    GEM,
    ITEM,
    WEAPON,
    BADGE,
    ARENA,
    TIME_ATK,
    DISPATCH,
    ARENA_TOWER,
    PRESET,
    RAID,
    RAID_FIRST,

    _END_,

    _MAX_               = _END_,
};


// 돌발 패키지 타입
public enum eUnexpectedPackageType : System.Byte
{
    _NONE_ = 0,     // 사용되지 않음.
    STAGE,          // 스테이지 클리어 
    ACCOUNT_LEVEL,  // 계정 레벨 
    CHAR_LEVEL,     // 캐릭터 레벨 
    FIRST_STAGE,    // 최초 스테이지 클리어 
};

// 돌발 패키지 반복 타입 
public enum eUnexpectedPackageRepeatType : System.Byte
{
    ENDLESS = 0,        // 무한 반복
    NOT_REPEAT,         // 반복 없음
    REPEAT              // 반복 
};

// 아레나 부정행위 치트타입 
public enum eArenaCheat : System.Byte
{
    NONE = 0,
    TEAM_POWER,     // 팀 파워 이상
    TIME_CHECK,     // 전투시간 미달 
    MAX_DAMAGE      // 최대 데미지 체크 
};


// 콘텐츠 위치 맞춰서 적용 가능한 최대 카드(서포터) 슬롯 개수
public enum eCardSlotPosMax : System.Byte
{
    CHAR                = 3,
    FACILITY            = 1,
    COSTUME             = 1,
    MONSTER             = 1,
    CARD                = 1,
    GEM                 = 1,
    ITEM                = 1,
    WEAPON              = 1,
    ARENA               = 1,
    DISPATCH            = 5,
    ARENA_TOWER         = 1,

    _SLOT_MAX_          = DISPATCH,     // 가장 큰 슬롯 개수 기준으로 설정합니다.
};

// 콘텐츠 위치 맞춰서 적용 가능한 최대 문양 슬롯 개수
public enum eBadgeSlotPosMax : System.Byte
{
    CHAR                = 1,
    FACILITY            = 1,
    COSTUME             = 1,
    MONSTER             = 1,
    CARD                = 1,
    GEM                 = 1,
    ITEM                = 1,
    WEAPON              = 1,
    ARENA               = 3,
    DISPATCH            = 0,
    ARENA_TOWER         = 3,

    _SLOT_MAX_          = ARENA,    // 가장 큰 슬롯 개수 기준으로 설정합니다.
};

public enum eDayOfWeek : System.Byte
{
    _START_DAY_         = 0,
    
    _SUN                = _START_DAY_,  // 일요일
    _MON,                               // 월요일
    _TUES,                              // 화요일
    _WEDNES,                            // 수요일
    _THURS,                             // 목요일
    _FRI,                               // 금요일
    _SATUR,                             // 토요일
    
    _END_DAY_,

};

public enum eAccountType : System.Byte
{
    DEFAULT             = 0,    // 유니티나 다른 플렛폼 접속했을 때의 기본값 (에디터 상에 수동으로 입력한 ID)
    GUEST,                      // 게스트 계정

    GOOGLE_PLAY         = 10,   // 구글 플레이 계정
    GAME_CENTER,                // 애플 게임센터 계정
    TWITTER,                    // 트위터 계정
    FACEBOOK,                   // 페이스북 계정
    YAHOO,                      // 야후 계정
    LINE,                       // Line 계정
    STEAM,                      // Steam 계정
    APPLE,
    _MAX_,
};

public enum eInAppKind : System.Byte
{
    APP_STORE           = 0,
    PLAY_STORE,
    STEAM_STORE,

    _MAX_,
};

public enum ePlatformKind : System.Byte
{
    _NONE_              = 0,    // 알수 없음.

    IOS,                        // IOS 기기
    ANDROID,                    // 안드로이드 기기
    STEAM,                      // 스팀 기기

    _MAX_,
};

public enum eServerType : System.Byte
{
    NONE = 0,
    //FARM                = 1,      // 서버에서만 사용하는 서버입니다.
    LOGIN               = 2,
    LOBBY               = 3,
    BATTLE              = 4,
    COMMUNITY           = 5,
    CHAT                = 6,
    RANKING             = 7,
    BRIDGE              = 8,

    _MAX_,

    _START_USER_SERVER_ = LOGIN,
    _END_USER_SERVER_   = CHAT + 1,
};

public enum eConnectKind : System.Byte
{
    CONNECT_ERR			= 0,

	DISCONNECT_ERR		= 100,

	DISCONNECT_LOGOUT	= 110,
	DISCONNECT_MOVE_SVR	= 111,
};

public enum eSecurityKind : System.Byte
{
    _NONE_              = 0,

    LOCK_IN,                    // 락인 컴퍼니
    STEAM_VAC,                  // 스팀 VAC

    _MAX_,
};

public enum ePresetKind : byte
{
	_NONE_ = 0,

	CHAR = 1,				// 캐릭터 프리셋
	STAGE,					// 메인, 요일, 스토리 등 스테이지 컨텐츠 프리셋
	ARENA,					// PVP 프리셋
	ARENA_TOWER,			// 아레나 타워 프리셋
	RAID,					// 추후 추가될 레이드 컨텐츠

	_END_,
};

public enum eCircleAuthLevel : byte
{
	_NONE_ = 0,
	MEMBER,
	DEPUTY,
	MASTER,
};

public enum eCircleMarkType : byte
{
	FLAG = 1,
	MARK = 2,
	COLOR = 3,
};

public enum eCircleNotiType : byte
{
	_CIRCLE_NOTI_NONE_ = 0,

	ARENA_RANK_IN = 1,
	TIMEATK_RANK_IN,
	TAKE_UR_GRADE_CARD,
	TAKE_UR_GRADE_WPN,
	WPN_ENCHANT_SUCCESS,

	_CIRCLE_NOTI_END_
};

// 대마인 에러 번호 위키 링크 - http://10.10.10.90/Mediawiki/index.php/Project-A/일반/ProjectDefine/eTMIErrNum
public enum eTMIErrNum : System.UInt64
{
    SUCCESS_OK                                      = 0,        // 성공

    __SERVER_START__                                = 1,


    _START_DB_MAIN_                                 = __SERVER_START__,
    SVR_DB_ERR_SQL_TRY_CATCH                        = _START_DB_MAIN_,   // DB 작업중 SQL 예외에 걸리는 오류입니다.
    SVR_DB_ERR_PARAM_BAD                            = 2,            // DB 파라메터 값을 잘못 전달하였습니다.
    SVR_DB_ERR_LOGIC_BUG                            = 3,            // DB 로직 오류입니다.
    SVR_DB_ERR_ADO_OPEN_ERR                         = 4,            // DB ADO 객체 생성 실패
    SVR_DB_ERR_EXCUTE_FAIL                          = 10,           // Excute 시작 오류 입니다.
    SVR_DB_ERR_QUERY_FAIL                           = 11,           // Query 시작 오류 입니다.
    SVR_DB_ERR_DEFAULT                              = 12,           // DB 기본 오류 입니다.
    SVR_DB_ERR_GET_PARAM                            = 13,           // DB ouput 파라메터 값 얻어오는 도중 발생하는 오류
    _END_DB_MAIN_,

    _START_DB_SUB_                                  = 100,
    SVR_DB_ERR_USER_ADD_FAIL                        = _START_DB_SUB_,   // DB에 유저 추가 실패
    SVR_DB_ERR_USER_EVENT_END_FAIL                  = 101,          // DB에 유저 이벤트 제거 실패
    SVR_DB_ERR_USER_EVENT_ADD_FAIL                  = 102,          // DB에 유저 이벤트 추가 실패
    SVR_DB_ERR_USER_PASS_END_FAIL                   = 103,          // DB에 유저 패스 제거 실패
    SVR_DB_ERR_USER_PASS_ADD_FAIL                   = 104,          // DB에 유저 패스 추가 실패
    SVR_DB_ERR_USER_PASS_UPDATE_INFO_FAIL           = 105,          // DB에 유저 패스 정보 갱신 실패
    SVR_DB_ERR_USER_RELOCATE_FAIL                   = 106,          // DB에 유저 정보 이전 실패
    SVR_DB_ERR_USER_EVENT_LOGIN_ADD_FAIL            = 107,          // DB에 유저 이벤트 로그인 추가 실패

    SVR_DB_FUNC_ERR_COMMON                          = 1000,         // DB 공용 기능 에러
    SVR_DB_FUNC_ERR_APPLY_USE_SAME_ACCOUNT_CODE     = 1001,         // DB 이어하기 계정에 적용할 동일한 코드값이 사용되고 있습니다.
    SVR_DB_FUNC_ERR_ALREADY_EXIST_ACCOUNT_LINK      = 1002,         // DB 이어하기 연결 계정이 존재해서 처리하지 못했습니다.
    SVR_DB_FUNC_ERR_ALREADY_EXIST_INAPP_PAYMENT     = 1003,         // DB 이미 적용한 이력이 있는 인앱 지불 정보라서 처리하지 못했습니다.
    SVR_DB_FUNC_ERR_ALREADY_EXIST_RELOCATE_USER     = 1004,         // DB 이미 이전 정보를 등록한 유저라 처리하지 못했습니다.
    SVR_DB_FUNC_ERR_ALREADY_EXIST_ID                = 1005,         // DB 사용중인 ID라서 처리하지 못했습니다.
    SVR_DB_FUNC_ERR_NOT_FIND_ID_FOR_INFO            = 1006,         // DB 해당 ID를 통해서 정보를 찾지 못했습니다.
    SVR_DB_FUNC_ERR_ALREADY_COMPLATE_RELOCATE_USER  = 1007,         // DB 서버 이전 완료 처리가 끝난 정보입니다.
    SVR_DB_FUNC_ERR_NOT_SAME_PASSWORD               = 1008,         // DB 비밀 번호가 일치하지 않습니다.
    SVR_DB_FUNC_ERR_NOT_ADD_OVER_RELOCATE_RESERVATION   = 1009,     // DB 서버 이전 예약 가능 최대 수치라 처리하지 못했습니다.
    _END_DB_SUB_                                    = 10000 - 1,

    _START_SESSION_                                 = 10000,
    SVR_SESION_ERR_NOT_EXIST                        = _START_SESSION_,  // 존재하지 않는 세션입니다.
    SVR_SESION_ERR_NOW_SERVER_MOVING                = 10001,        // 서버 이동중인 세션입니다.
    SVR_SESION_ERR_NOW_OTHER_WORKING                = 10002,        // 다른 일을 진행중인 세션입니다.
    SVR_SESION_ERR_CLASH_TO_WORK                    = 10003,        // 다른 작업 진행중에 다른 작엄을 진행했습니다.(크리티컬 에러 유저 접속 종료 후 제접 유도해야함)
    SVR_SESION_ERR_NOW_OVER_CONNECTING              = 10004,        // 접속 유지 시간이 지난 세션입니다. 잠시후 다시 연결 시도해 주세요
    SVR_SESION_ERR_NOT_SIGNAL_LONG_TIME_KICK        = 10005,        // 오랜시간 동안 신호가 없어서 접속이 끊어졌습니다.
    SVR_SESION_ERR_ALREADY_SECURITY_INFO            = 10006,        // 이미 보안 정보가 존재해서 처리하지 못했습니다.
    SVR_SESION_ERR_ALREADY_SECURITY_VERIFY          = 10007,        // 이미 보안 검증이 완료된 유저라 처리하지 못했습니다.
    SVR_SESION_ERR_NOT_ALLOW_SECURITY_VERIFY        = 10008,        // 보안 검증이 허용된 상태가 아니라서 처리하지 못했습니다.
    SVR_SESION_ERR_BANNED_SECURITY_VERIFY           = 10009,        // 보안 검증 결과 차단된 유저입니다.
    _END_SESSION_                                   = 20000 - 1,

    _START_SVR_OTHER_SUB_                           = 20000,
    SVR_OTHER_ERR_NOT_NOTIFY_TO                     = 20001,         // 유저에게 알리지 않는 에러

    __START_SVR_RETURN__                            = 20100,
    SVR_RETURN_BREAK                                = 20101,         // 결과를 받으면 상황에 따라서 break 처리를 합니다.
    SVR_RETURN_CONTINUE                             = 20102,         // 결과를 받으면 상황에 따라서 continue 처리를 합니다.
    __END_SVR_RETURN__                              = 20199,

    _END_SVR_OTHER_SUB_                             = 30000 - 1,

    _START_SERVER_                                  = 30000,
    SVR_ERR_NOT_CALL_FUNCTION                       = _START_SERVER_,   // 호출 하면 안되는 오브젝트의 함수를 호출했습니다.
    _END_SERVER_                                    = 40000 - 1,

    _START_USER_                                    = 100000,
    // 서버 공용 관련
    SVR_COM_ERR_DEFAULT                             = _START_USER_, // 서버 에러 입니다.
    SVR_COM_ERR_PREPARATION_FUNCTION                = 100001,       // 준비중인 기능입니다.
    SVR_COM_ERR_PREPARATION_CONTENTS                = 100002,       // 준비중인 콘텐츠입니다.
    SVR_COM_ERR_NOW_VERY_BUSY_SERVER                = 100003,       // 서버가 다른일을 수행중이라 처리할 수 없습니다.
    SVR_COM_ERR_REMOVED_FUNCTION                    = 100004,       // 제거된 기능입니다.
    SVR_COM_ERR_FUNCTION_TO_BE_REMOVED              = 100005,       // 제거 예정인 기능입니다.
    SVR_COM_ERR_FUNCTION_TO_BE_EDITED               = 100006,       // 수정 예정인 기능입니다.
    SVR_COM_ERR_NOT_OPENED_NOW_READY                = 100007,       // 서버가 열려있지 않고 준비 상태입니다.
    SVR_COM_ERR_NOT_FIND_ACCOUNT_UUID               = 100008,       // 해당 계정 유저 정보를 찾을 수 없습니다.
    SVR_COM_ERR_NOT_SAME_VERSION                    = 100009,       // 버전이 같지 않습니다.
    SVR_COM_ERR_NOW_CLOSING                         = 100010,       // 현재 서버가 종료중입니다.
    SVR_COM_ERR_OVER_TIME_RELOCATE_SERVER           = 100011,       // 서버 이전 시기가 아니라서 처리하지 못했습니다.
    SVR_COM_ERR_NOT_PERIOD_INFLU_MISSION_PLAY       = 100012,       // 서버 달성(세력) 이벤트 진행 기간이 아니라 처리하지 못했습니다.
    SVR_COM_ERR_NOT_PERIOD_INFLU_ACTIVE             = 100013,       // 서버 달성(세력) 이벤트 활성화 기간이 아니라 처리하지 못했습니다.
    SVR_COM_ERR_NOT_WORKING_IN_INFLU_PLAYING        = 100014,       // 서버 달성(세력) 이벤트 진행 기간 동안에 할 수 없는 작업입니다.
    SVR_COM_ERR_INVALID_PACKET_MSG                  = 100015,       // 무효한 패킷 메시지입니다.

    SVR_COM_ERR_NOT_FIND_TARGET_SERVER              = 100100,       // 해당 서버를 찾을 수 없어서 처리하지 못했습니다.
    SVR_COM_ERR_NOT_FIND_LOBBY_SERVER               = 100101,       // 로비 서버를 찾을 수 없어서 처리하지 못했습니다.

    // 랭킹 서버 관련
    SVR_RAN_ERR_NOT_FIND_INFLUENCE_INFO             = 100700,       // 서버 달성(세력) 정보가 존재하지 않습니다.

    SVR_COM_UNKNOWN_REWARDTYPE_TYPE                 = 101001,       // 알 수 없는 eREWARDTYPE 입니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_CHARACTER          = 101002,       // 캐릭터 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_SKILL              = 101003,       // 스킬 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_COSTUME            = 101004,       // 코스츔 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_CARD               = 101005,       // 카드(서포터) 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_ITEM               = 101006,       // 아이템 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_GEM                = 101007,       // 곡옥 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_WEAPON             = 101008,       // 무기 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_STAGE              = 101009,       // 스테이지 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_STORE              = 101010,       // 상점 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_FACILITY           = 101011,       // 시설 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_ROOM_THEME         = 101012,       // 룸 테마 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_ROOM_FIGURE        = 101013,       // 룸 피규어 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_ROOM_ACTION        = 101014,       // 룸 액션 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_ROOM_FUNC          = 101015,       // 룸 기능 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_LOGIN_BONUS        = 101016,       // 로그인 보너스 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_RANKUP_REWARD      = 101017,       // 룸 카메라 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_WEEKLY_MISSION     = 101018,       // 주간 미션 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_EVENTSET           = 101019,       // 이벤트 셋 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_EVENT_RESET_REWARD = 101020,       // 이벤트 리셋 보상 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_LEVELUP            = 101021,       // 레벨업 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_GEM_RAND_OPT       = 101022,       // 곡옥 랜덤 옵션 테이블 데이터가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_ACHIEVE            = 101023,       // 공적 테이블 데이터가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_USERMARK           = 101024,       // 유저 마크 테이블 데이터가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_EVENT_EXCHANGE_REWARD  = 101025,   // 이벤트 교환 보상 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_RANDOM             = 101026,       // 랜덤 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_PASSSET            = 101027,       // 패스 셋 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_PASS_MISSION       = 101028,       // 패스 미션 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_MAIL               = 101029,       // 메일 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_CARD_DISPATCH_SLOT = 101030,       // 카드(서포터) 파견 슬롯 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_CARD_DISPATCH_MISSION  = 101031,   // 카드(서포터) 파견 임무 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_ITEMREQLIST        = 101032,       // 아이템 비용 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_ARENA_TOWER        = 101033,       // 아레나 타워 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_MONTHLY_FEE        = 101034,       // 정기 결제 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_BUFF               = 101035,       // 버프 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_LOBBYTHEME         = 101036,       // 로비 테마 테이블 데이터가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_AWAKE_SKILL        = 101037,       // 각성 스킬 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_ENCHANT            = 101038,       // 인챈트 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_INFLU_INFO         = 101039,       // 서버 달성(세력) 정보 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_INFLU_MISSION      = 101040,       // 서버 달성(세력) 미션 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_INFLU_MISSION_SET  = 101041,       // 서버 달성(세력) 미션 셋 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_INFLU_RANK         = 101042,       // 서버 달성(세력) 랭크 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_ROTATION_GACHA     = 101043,       // 로테이션 가챠 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_DAULY_MISSION      = 101044,       // 일일 미션 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_DAULY_MISSION_SET  = 101045,       // 일일 미션 셋 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_FACILITY_TRADE     = 101046,       // 시설 교환 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_LOBBY_ANI          = 101047,       // 로비 에니메이션 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_CARD_FORMATION     = 101048,       // 카드(서포터) 진형 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_FACILITY_OPER_ROOM = 101049,       // 작전회의실 테이블 데이타가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_FACILITY_ADDON     = 101050,       // AddOn테이블 데이터가 존재하지 않습니다.
    SVR_COM_NOT_EXIST_TABLE_DATA_ACHIEVE_EVT        = 101051,       // 공적 이벤트 테이블 데이터가 존재하지 않습니다.
	SVR_COM_NOT_EXIST_TABLE_DATA_CIRCLE_ATTENDANCE  = 101052,       // 서클 출석체크 테이블 데이터가 존재하지 않습니다.
	SVR_COM_NOT_EXIST_TABLE_DATA_LOGIN_BONUS_MONTHLY = 101053,       // 월단위(300일 출석) 로그인 보너스 테이블 데이타가 존재하지 않습니다


	// 유저 공용
	SVR_USER_ERR_ALREADY_EXIST_USER                 = 110000,       // 이미 로그온 된 유저입니다.
    SVR_USER_ERR_NOT_OVER_TIME_FRIEND_POINT_GIVE    = 110001,       // 친구 포인트 전달 가능 시간이 지나지 않아서 처리하지 못했습니다.
    SVR_USER_ERR_NOT_OVER_TIME_LOGIN_BONUS          = 110002,       // 아직 로그인 보너스 획득 가능 시간이 지나지 않아서 처리하지 못했습니다.
    SVR_USER_ERR_NOT_FIND_REWARD_DATA               = 110003,       // 보상 정보를 찾지 못했습니다.
    SVR_USER_ERR_BLOCK_USER                         = 110004,       // 차단된 유저입니다.
    SVR_USER_ERR_NOT_FIND_USERINFO_BY_UUID          = 110005,       // 유저 고유 ID를 통해서 해당 유저 정보를 찾을 수가 없습니다.(DB)
    SVR_USER_ERR_ALREADY_SET_SAME_USER_MARK         = 110006,       // 이미 동일한 지휘관 마크를 설정한 상태입니다.
    SVR_USER_ERR_NOT_EXIST_USER_MARK                = 110007,       // 가지고 있지 않은 지휘관 마크 입니다.
    SVR_USER_ERR_ALREADY_TAKE_MARK                  = 110008,       // 이미 획득한 유저 마크라 처리하지 못했습니다.
    SVR_USER_ERR_NOT_TAKE_MARK                      = 110009,       // 획득할 수 없는 마크입니다.
    SVR_USER_ERR_ERROR_CONDITION_MARK               = 110010,       // 조건을 만족하지 못해서 획득할 수 없는 마크입니다.
    SVR_USER_ERR_CONTINUE_SAME_MARK_ADD             = 110011,       // 동일한 지휘관 마크를 연속해서 추가하려고 했습니다.
    SVR_USER_ERR_CHANGE_SECURITY_ACCOUNT_PASSWORD   = 110012,       // 이어하기 계정에 비밀번호 암호화에 실패해서 처리하지 못했습니다.
    SVR_USER_ERR_EMPTY_ACCOUNT_CODE_NOT_SET_PW      = 110013,       // 이어하기 계정 코드가 없어서 비밀번호를 설정할 수 없습니다.
    SVR_USER_ERR_USER_SHIFT_SESSION_ABOUT_RECONNECT = 110014,       // 유저 정보가 세로운 접속으로 연결됐기 때문에 다시 로그인 해주시기 바랍니다.
    SVR_USER_ERR_EMPTY_USER_SHIFT_SESSION           = 110015,       // 유저 정보가 잘못되서 재접속 중인 유저 정보를 연결하지 못했습니다.
    SVR_USER_ERR_ALREADY_SHOW_TUTORIAL              = 110016,       // 유저가 이미 확인한 튜토리얼입니다.
    SVR_USER_ERR_ALREADY_DELETE_USER_INFO           = 110017,       // 삭제가 된 유저입니다.
    SVR_USER_ERR_ALREADY_SET_SAME_LOBBY_THEME       = 110018,       // 이미 동일한 로비 테마를 설정한 상태입니다.
    SVR_USER_ERR_NOT_EXIST_LOBBY_THEME              = 110019,       // 가지고 있지 않은 로비 테마 입니다.
    SVR_USER_ERR_ALREADY_TAKE_LOBBY_THEME           = 110020,       // 이미 획득한 로비 테마라 처리하지 못했습니다.
    SVR_USER_ERR_CONTINUE_SAME_LOBBY_THEME          = 110023,       // 동일한 로비 테마를 연속해서 추가하려고 했습니다.
    SVR_USER_ERR_ALREADY_RELOCATE_INFO              = 110024,       // 서버 이전 정보가 이미 존재해서 처리하지 못했습니다.


    // 유저 이동
    SVR_USER_MOVE_ERR_EXIST_SAME_CREDITKEY          = 110030,       // 유저 서버 이동 오류 - 이미 예약된 동일 키 존재함.
    SVR_USER_MOVE_ERR_NOT_FIND_WAIT_MOVE_INFO       = 110031,       // 유저 서버 이동 오류 - 예약된 인증 정보가 없습니다.
    SVR_USER_MOVE_ERR_NOT_FIND_WANT_MOVE_SVR        = 110032,       // 유저 서버 이동 오류 - 이동을 원하는 서버 정보를 찾을 수 없습니다.


    // 로그인 관련
    SVR_USER_LOGIN_ERR_INVALID_ACCOUNT_TYPE         = 110041,       // 무효한 계정 타입입니다.
    SVR_USER_LOGIN_ERR_EMPTY_ACCOUNT_ID             = 110042,       // 계정 아이디 값이 비었습니다.
    SVR_USER_LOGIN_ERR_NOT_INPUT_KEEP_ON_ID         = 110043,       // 이어하기 관련 연결을 인증할 정보를 입력하지 않습니다.
    SVR_USER_LOGIN_ERR_NOT_INPUT_KEEP_ON_PW         = 110044,       // 이어하기 관련 연결을 인증할 비밀번호를 입력하지 않았습니다.
    SVR_USER_LOGIN_ERR_NOT_INPUT_KEEP_ON_ACCOUNT_ID = 110045,       // 이어하기 관련 교체할 계정 정보를 입력하지 않았습니다.
    SVR_USER_LOGIN_ERR_INVALID_KEEP_ON_KIND         = 110046,       // 이어하기 관련 인증 분류 정보가 잘못돼서 처리하지 못했습니다.
    SVR_USER_LOGIN_ERR_NOT_FIND_UUID_IN_KEEP_ON     = 110047,       // 이어하기에 연결된 유저 정보를 찾지 못햇습니다.

    SVR_USER_RELOGIN_ERR_NOT_FIND_INFO              = 110050,       // 재접속 가능한 유저 정보를 찾지 못했습니다.
    SVR_USER_RELOGIN_ERR_ALREADY_CONNECT_USER       = 110051,       // 이미 접속된 상태라서 재접속 처리를 하지 못했습니다.


    // 캐릭터 관련
    SVR_CHAR_COM_ERR_NOT_EXIST_INST                 = 110100,       // 존재하지 않는 캐릭터 입니다.
    SVR_CHAR_ADD_ERR_ALREADY_EXIST_CHAR             = 110101,       // 추가하려는 캐릭터는 이미 존재하는 타입 입니다.
    SVR_CHAR_CNG_ERR_ALREADY_SET_SAME_MAIN_CHAR     = 110102,       // 이미 메인으로 같은 캐릭가 설정돼 있습니다.
    SVR_CHAR_CARD_ERR_ALREADY_EQUIPED_SAME_CARD     = 110103,       // 이미 같은 카드(서포터)를 장착한 상태입니다.
    SVR_CHAR_WPN_ERR_ALREADY_EQUIPED_SAME_WEAPON    = 110104,       // 이미 같은 무기를 장착한 상태입니다.
    SVR_CHAR_WPN_ERR_ALREADY_OHTER_CHARTER_EQUIP    = 110105,       // 다른 캐릭터가 이미 장착한 무기입니다.
    SVR_CHAR_CSTM_ERR_ALREADY_APPLY_SAME_COSTUME    = 110106,       // 이미 같은 코스츔을 적용한 상태입니다.
    SVR_CHAR_CSTM_ERR_IMPOSSIBLE_APPLY_SKILL        = 110107,       // 해당 캐릭터가 적용할 수 없는 타입의 코스츔입니다.
    SVR_CHAR_CSTM_ERR_OVER_AREA_COLOR_VALUE         = 110108,       // 적용 가능한 범위를 넘어선 코스츔 색입니다.
    SVR_CHAR_WPN_ERR_CANT_EMPTY_MAIN_SLOT           = 110109,       // 메인 무기 슬롯에 무기를 제거할 수 없습니다.
    SVR_CHAR_WPN_ERR_CANT_APPLY_SKIN_NOT_EXIST_BOOK = 110110,       // 해당 무기 스킨 적용 조건을 만족하지 못해서 처리 하지 못했습니다.
    SVR_CHAR_SEL_ERR_CANT_REMOVE_MAIN_CHARACTER     = 110111,       // 메인 캐릭터는 해제할 수 없습니다.
    SVR_CHAR_CSTM_ERR_ALREADY_DYEING_COSTUME        = 110112,       // 이미 염색중 상태입니다.
    SVR_CHAR_CSTM_ERR_NOT_DYEING_COSTUMESTATE       = 110113,       // 염색중 상태가 아닙니다. 
    SVR_CHAR_CSTM_ERR_CAN_NOT_DYEING_TO_LOCK        = 110114,       // 잠금 상태의 부위는 염색할 수 없습니다.
    SVR_CHAR_CSTM_ERR_TARGET_IS_NULL                = 110115,       // 코스츔 타겟이 null입니다. 
    SVR_CHAR_CSTM_ERR_SAME_LOCKFLAG                 = 110116,       // 동일한 잠금 상태입니다. 
    SVR_CHAR_CSTM_ERR_INVALID_DYEING_COUNT          = 110117,       // 한 부위에 두개이상의 색상이 선택됐습니다.

    SVR_CHAR_SKL_ERR_NOT_EXIST_INST                 = 110150,       // 아직 습득하지 않은 스킬 입니다.
    SVR_CHAR_SKL_ERR_DO_NOT_APPLY_TYPE_SKILL        = 110151,       // 현재 캐릭터가 장착할 수 없는 타입의 스킬입니다.
    SVR_CHAR_SKL_ERR_TRY_ATTACH_SAME_SKILL          = 110152,       // 동일한 스킬을 다른 슬롯에 장착할 수 없습니다.
    SVR_CHAR_SKL_ERR_SAME_ATTACH_GEM_SLOT_ALL       = 110153,       // 장착된 스킬과 장착하려는 스킬이 모두 같습니다.
    SVR_CHAR_SKL_ERR_LVUP_NOT_EXIST_ITEMREQLIST     = 110154,       // 스킬 레벨업에 필요한 재료 아이템 테이블 정보가 존재하지 않습니다.
    SVR_CHAR_SKL_ERR_LACK_SKILL_PT                  = 110155,       // 스킬 포인트가 부족합니다.
    SVR_CHAR_SKL_ERR_LVUP_LACK_CHAR_REQ_LEVEL       = 110156,       // 캐릭터 레벨이 부족하여 스킬 레벨업을 진행할 수 없습니다.
    SVR_CHAR_SKL_ERR_LVUP_NOT_EXIST_PARENT_SKILL    = 110157,       // 선행 스킬을 습득한 상태가 아니라서 스킬을 습득할 수 없습니다.
    SVR_CHAR_SKL_ERR_NOT_SAME_TYPE_SLOT_APPLY       = 110158,       // 스킬 타입이 다른 슬롯에는 스킬을 적용할 수 없습니다.
    SVR_CHAR_SKL_ERR_NOT_OPEN_SLOT                  = 110159,       // 열리지 않은 스킬 슬롯에 스킬 적용을 시도해서 처리하지 못했습니다.

    SVR_CHAR_ANI_ERR_ALREADY_EXIST_ANI              = 110160,       // 이미 존재하는 캐릭터 에니메이션이라 추가하지 못했습니다.
    SVR_CHAR_FRI_ERR_LACK_PRESENT_CNT               = 110161,       // 선물하기 횟수가 부족해서 처리하지 못했습니다.


    // 재화 관련
    SVR_GODS_COM_ERR_NOT_EXIST_INST                 = 110200,       // 존재하지 않는 재화를 참조하려고 했습니다.
    SVR_GODS_COM_ERR_NOT_TICKET_TYPE                = 110201,       // 티켓 타입이 아닌 재화입니다.


    // 비밀기지 관련
    SVR_FACI_COM_ERR_NOT_EXIST_INST                 = 110300,       // 존재하지 않는 비밀기지 입니다.
    SVR_FACI_LV_ERR_LVUP_NOT_EXIST_ITEMREQLIST      = 110301,       // 시설 레벨업에 필요한 재료 아이템 테이블 정보가 존재하지 않습니다.
    SVR_FACI_LV_ERR_LVUP_LACK_USER_REQ_LEVEL        = 110302,       // 유저 레벨이 부족하여 시설 레벨업을 진행할 수 없습니다.
    SVR_FACI_OPER_ERR_NOT_EXIST_CHAR                = 110303,       // 존재하지 않는 캐릭터이므로 시설을 이용할 수 없습니다.
    SVR_FACI_OPER_ERR_INVALID_ITEM_TID              = 110304,       // 아이템 테이블에 등록되지 않는 아이템이라 조합을 진행할 수 없습니다.
    SVR_FACI_OPER_ERR_REGI_ALREADY_CHAR             = 110305,       // 이미 시설에 등록된 캐릭터이므로 다른 시설을 이용할 수 없습니다.
    SVR_FACI_OPER_ERR_ALREADY_MAX_LV_CHAR           = 110306,       // 이미 캐릭터의 레벨이 최대라서 시설을 이용할 수 없습니다.
    SVR_FACI_OPER_ERR_ALREADY_MAX_SP_CHAR           = 110307,       // 이미 캐릭터의 스킬 포인트가 최대라서 시설을 이용할 수 없습니다.
    SVR_FACI_OPER_ERR_NOT_EXIST_ITEM_COMBINE_TABLE  = 110308,       // 원하는 아이템의 조합 정보가 없어서 시설을 이용할 수 없습니다.
    SVR_FACI_OPER_ERR_CONBINE_NOT_EXIST_ITEMREQ     = 110309,       // 아이템 조합에 필요한 재료 아이템 테이블 정보가 존재하지 않아서 작돌할 수 없습니다.
    SVR_FACI_OPER_ERR_ALREADY_MAX_LV_WEAPON         = 110310,       // 이미 무기의 레벨이 최대라서 시설을 이용할 수 없습니다.
    SVR_FACI_OPER_ERR_REGI_ALREADY_WEAPON           = 110311,       // 이미 시설에 등록된 무기라 관련 작업을 처리할 수 없습니다.
    SVR_FACI_OPER_ERR_NOT_EXIST_WEAPON              = 110312,       // 존재하지 않는 무기이므로 시설을 이용할 수 없습니다.
    SVR_FACI_OPER_ERR_NOT_STARTED                   = 110313,       // 작동하지 않은 시설이라서 완료 처리를 할 수 없습니다.
    SVR_FACI_OPER_ERR_ALREADY_START_CANT_RESTART    = 110314,       // 이미 작동이 진행중인 시설이라 다시 시작할 수 없습니다.
    SVR_FACI_OPER_ERR_LACK_LV_FACILITY              = 110315,       // 시설의 레벨이 부족해서 작동할 수 업습니다.
    SVR_FACI_LV_ERR_NOT_EXIST_SAME_PARENT_DATA      = 110316,       // 원하는 ParentsID 가지고 있는 시설 정보가 존재하지 않습니다.
    SVR_FACI_OPER_ERR_SKILL_POINT_CALCULATION       = 110317,       // 스킬 포인트 계산 오류로 시설 완료 처리를 할 수 없습니다.
    SVR_FACI_LV_ERR_NOT_ACTIVE_PRE_FACILITY         = 110318,       // 이전 단계의 시설을 활성화 하지 않아서 원하는 시설을 활성화 하지 못했습니다.
    SVR_FACI_LV_ERR_NOT_NOT_OPEN_LACK_USER_LV       = 110319,       // 유저의 레벨이 낮아서 시설을 활성화 할 수 없습니다.
    SVR_FACI_LV_ERR_NOT_SAME_PARENTSID              = 110320,       // ParentsID와 시설ID가 같지 않아서 처리할 수 없습니다.
    SVR_FACI_LV_ERR_SAME_GROUP_ACTIVED              = 110321,       // 이미 같은 그룹의 시설중 활성화 된 것이 있어서 활성화를 진행할 수 없습니다.
    SVR_FACI_OPER_ERR_LACK_LV_NOT_OPER              = 110322,       // 시설의 레벨이 낮아서 시설을 작동할 수 없습니다.
    SVR_FACI_OPER_ERR_EQUIPED_CHARACTER             = 110323,       // 캐릭터가 장착중인 무기라 시설에서 이용할 수 없습니다.
    SVR_FACI_OPER_ERR_NOT_FACILITY_ITEM             = 110324,       // 해당 아이템은 시간단축 아이템이 아니라서 즉시완료를 할 수 없습니다.
    SVR_FACI_OPER_ERR_OVER_COUNT_MATERIAL_COUNT     = 110325,       // 시설에서 처리 가능한 재료의 수 이상을 전달해서 처리하지 못했습니다.
    SVR_FACI_OPER_ERR_NOT_SAME_NEED_MATERIAL_COUNT  = 110326,       // 필요 재료 수량과 같지 않아서 비밀 기지 작동을 처리하지 못했습니다.
    SVR_FACI_OPER_ERR_CAN_NOT_CALCEL_TYPE           = 110327,       // 작동 취소를 할 수 없는 타입의 시설입니다.


    // 룸 테마 관련
    SVR_RMTM_COM_ERR_NOT_EXIST_INST_THEME           = 110400,       // 존재하지 않는 룸 테마 입니다.
    SVR_RMTM_COM_ERR_NOT_EXIST_INST_THEME_SLOT      = 110401,       // 존재하지 않는 룸 테마 슬롯 입니다.
    SVR_RMTM_COM_ERR_NOT_EXIST_INST_ACTION          = 110402,       // 존재하지 않는 룸 액션 입니다.
    SVR_RMTM_COM_ERR_NOT_EXIST_INST_CAMERA          = 110403,       // 존재하지 않는 룸 카메라 입니다.
    SVR_RMTM_COM_ERR_ALREADY_EXIST_THEME            = 110404,       // 이미 존재하는 룸 테마 입니다.
    SVR_RMTM_COM_ERR_ALREADY_PURCHASE_INST          = 110405,       // 이미 구매된 룸 관련 구매 정보입니다.
    SVR_RMTM_ADD_ERR_NOT_EXIST_ROOM_PRODUCT         = 110406,       // 존재 하지 않는 룸 상품입니다.
    SVR_RMTM_ADD_ERR_ALREADY_ROOM_THEME_CANT_ADD    = 110407,       // 이미 존재하는 룸 테마라서 추가할 수 없습니다.
    SVR_RMTM_ADD_ERR_ALREADY_ROOM_ACTION_ADD        = 110408,       // 이미 존재하는 룸 액션이라서 추가할 수 없습니다.
    SVR_RMTM_ADD_ERR_LACK_ROOM_POINT                = 110409,       // 룸 포인트가 부족해 구매할 수 없습니다.
    SVR_RMTM_ADD_ERR_NOT_EXIST_FUNC_BELONG_THEME    = 110410,       // 해당 룸 기능과 관련된 룸 테마가 존재하지 않아서 구입할 수 업습니다.
    SVR_RMTM_ADD_ERR_NOT_EXIST_ACTION_BELONG_CHAR   = 110411,       // 해당 룸 액션과 관련된 캐락터를 가지고 있지 않아서 구입할 수 업습니다.
    SVR_RMTM_ADD_ERR_NOT_EXIST_FIGURE_BELONG_CSTM   = 110412,       // 해당 룸 피규어와 관련된 코스츔을 가지고 있지 않아서 구입할 수 업습니다.
    SVR_RMTM_ADD_ERR_NOT_EXIST_FIGURE_BELONG_MON    = 110413,       // 해당 룸 피규어와 관련된 몬스터 도감을 가지고 있지 않아서 구입할 수 업습니다.
    SVR_RMTM_ADD_ERR_NOT_EXIST_FIGURE_BELONG_WPN    = 110414,       // 해당 룸 피규어와 관련된 무기를 가지고 있지 않아서 구입할 수 업습니다.
    SVR_RMTM_SAVE_ERR_TIME_DELAY_WAITING            = 110415,       // 룸 테마 저장 딜레이 중입니다. 조금더 시간이 지난후에 다시 시도해 주시기 바랍니다.
    SVR_RMTM_SAVE_ERR_NOT_OVER_SLOT_ADD             = 110416,       // 슬롯 최대 개수를 넘어서 저장 할 수 없습니다.
    SVR_RMTM_SAVE_ERR_OTHER_THEME_SLOT_NUM_IN_FIGURE    = 110417,   // 다른 테마룸 슬롯에서 사용중인 피규어가 포함돼있습니다.
    SVR_RMTM_SAVE_ERR_EMPTY_DETAIL_INFO_FIGURE      = 110418,       // 세부정보가 없는 피규어가 포함돼 있습니다.
    SVR_RMTM_SAVE_ERR_OTHER_THEME_SLOT_USED         = 110419,       // 다른 룸 테마 슬롯에서 사용중이라서 처리할 수 없습니다.
    SVR_RMTM_SAVE_ERR_CANT_APPLY_ACTION             = 110420,       // 해당 피규어의 캐릭터가 적용할 수 없는 룸 액션입니다.
    SVR_RMTM_SAVE_ERR_CANT_APPLY_POSITION_ACTION    = 110421,       // 잘못된 위치에 룸 액션 타입을 적용하려고 했습니다.
    SVR_RMTM_SAVE_ERR_OVER_FIGURE_COUNT_IN_THEME    = 110422,       // 해당 테마에 저장 가능한 피규어 개수를 초과했습니다.
    SVR_RMTM_SAVE_ERR_SAME_FIGURE_SLOT_NUM          = 110423,       // 동일한 피규어 슬롯 번호를 사용하는 데이타가 존재합니다.


    // 코스츔 관련
    SVR_CSTM_COM_ERR_NOT_EXIST_INST                 = 110500,       // 존재하지 않는 코스츔 입니다.
    SVR_CSTM_ADD_ERR_NOT_EIXST_SAME_TYPE_CHARACTER  = 110501,       // 코스츔에 해당하는 캐릭터가 존재하지 않아서 처리하지 못했습니다.


    // 스테이지 관련
    SVR_STGE_COM_ERR_NOT_EXIST_INST                 = 110600,       // 존재하지 않는 스테이지 입니다.
    SVR_STGE_SRT_ERR_LACK_TACKET                    = 110601,       // 티켓이 부족해서 해당 스테이지를 진행할 수 없습니다.
    SVR_STGE_RET_ERR_NOT_SAME_START_INFO_STAGE_TID  = 110602,       // 게임 시작과 종료 결과의 정보가 올바르지 않습니다.(스테이지 ID)
    SVR_STGE_RET_ERR_NOT_SAME_START_INFO_CHAR_IDX   = 110603,       // 게임 시작과 종료 결과의 정보가 올바르지 않습니다.(캐릭터 ID)
    SVR_STGE_RET_ERR_NOT_SAME_START_INFO_CERT_KEY   = 110604,       // 게임 시작과 종료 결과의 정보가 올바르지 않습니다.(인증 키)
    SVR_STGE_RET_ERR_OVER_NBOX_MAX_COUNT            = 110605,       // 게임 종료 결과 획득 가능한 일반 상자의 최대 수 이상 회득해서 처리할 수 없습니다.
    SVR_STGE_NOT_NOW_SPECIAL_STAGE_TID              = 110606,       // 현재 진행예정인 스페셜 스테이지가 아닙니다.
    SVR_STGE_NOT_OVER_SPECIAL_STAGE_TIME            = 110607,       // 아직 스페셜 스테이지 오픈 가능 시간이 되지 않았습니다.
    SVR_STGE_ERR_LACK_MULTIPLE_IDX                  = 110608,       // AP 배수 인덱스가 올바르지 않습니다.
    SVR_STGE_ERR_NO_RANKING_STAGE_TID               = 110609,       // 랭킹을 저장하지 않는 스테이지 입니다.
    SVR_STGE_ERR_UUID_NOT_IN_RANKING                = 110610,       // 요청한 UUID는 랭킹에 존재하지 않습니다.
    SVR_STGE_ERR_NOT_RESET_WHEN_CAN_NOW_SPECIAL_PLAY    = 110611,   // 진행이 가능한 상태 특별 모드 리셋을 처리하지 못했습니다.
    SVR_STGE_COM_NOT_START_STAGE                    = 110612,       // 스테이지 시작을 진행하지 않아 처리하지 못했습니다.
    SVR_STGE_CON_NOT_CONTINUE_STAGE                 = 110613,       // 스테이지 이어하기가 불가능한 스테이지라 처리하지 못했습니다.
    SVR_STGE_ERR_NOT_USE_FASTQUESTTICKET_STAGE      = 110614,       // 토벌권을 사용할 수 없는 스테이지입니다.
    SVR_STGE_ERR_NOT_CLEAR_ALLMISSION               = 110615,       // 스테이지에 모든 미션이 클리어 되지 않았습니다.
    SVR_STGE_ERR_FAIL_START_IN_SECRET               = 110616,       // 시크릿 퀘스트 시작 가능 조건에 만족하지 않아 처리하지 못했습니다.
    SVR_STGE_ERR_NO_RAID_RANKING_STAGE_TID          = 110617,       // 레이드 스테이지와 단계가 존재하지않습니다.
    SVR_STGE_ERR_NO_RAID_RANKING_EXIST              = 110618,       // 해당 단계에 레이드 랭킹이 존재하지않습니다.
    SVR_STGE_ERR_DISABLE_SUBCHAR_NEED_CLEAR         = 110619,       // 서브캐릭터 입장은 클리어한 스테이지만 가능합니다.
    SVR_STGE_ERR_NOT_EXIST_SUBCHAR                  = 110620,       // 서브캐릭터가 존재하지 않습니다.
    SVR_STGE_ERR_FAIL_START_SECRET_BY_SUBCHAR_CNT   = 110621,       // 서브캐릭터의 비밀임무 참여 횟수가 부족합니다.
    SVR_STGE_ERR_DISABLE_SAMEUID_MAINCHAR           = 110622,       // 서브캐릭터의 UID 가 메인캐릭터와 중복됩니다. 
    SVR_STGE_ERR_DUPLICATE_IN_SUBCHARUID            = 110623,       // 서브캐릭터의 UID가 중복됩니다.

    // 몬스터 관련
    SVR_MON_COM_ERR_NOT_EXIST_INST                  = 110700,       // 존재하지 않는 몬스터 입니다.


    // 카드(서포터) 관련
    SVR_CARD_COM_ERR_NOT_EXIST_INST                 = 110800,       // 존재하지 않는 카드(서포터) 입니다.
    SVR_CARD_LV_ERR_CHANGE_GAQ_LV_ZERO              = 110801,       // 카드(서포터) 레벨 변경에 적용할 레벨이 0입니다.
    SVR_CARD_LV_ERR_OVER_SIZE_MATERIAL_CARD         = 110802,       // 재료로 사용될 카드(서포터)의 수가 너무 많습니다.
    SVR_CARD_LV_ERR_NOT_EXIST_MATERIAL              = 110803,       // 레벨업 재료에 존재하지 않는 카드(서포터)가 있습니다.
    SVR_CARD_LV_ERR_OTHER_TYPE_MATERIAL             = 110804,       // 레벨업 재료에 다른 타입의 카드(서포터)가 있습니다.
    //SVR_CARD_POS_ERR_APPLY_CHARACTER                = 110805,       // 캐릭터가 위치 적용중인 카드(서포터)라 처리할 수 없습니다.
    //SVR_CARD_POS_ERR_APPLY_FACILITY                 = 110806,       // 비밀기지에서 위치 적용중인 카드(서포터)라 처리할 수 없습니다.
    SVR_CARD_POS_ERR_ALREADY_SAME_CHARACTER         = 110807,       // 이미 같은 캐릭터가 이 카드(서포터)를 위치 적용중입니다.
    SVR_CARD_POS_ERR_ALREADY_SAME_FACILITY          = 110808,       // 이미 같은 비밀기지가 이 카드(서포터)를 위치 적용중입니다.
    SVR_CARD_POS_ERR_ALREADY_SAME_USED              = 110809,       // 이미 동일한 대상이 위치 적용중인 카드(서포터)라 처리할 수 없습니다.
    SVR_CARD_POS_ERR_NOT_OUT_POS                    = 110810,       // 위치 적용 해제로 동작할 수 없는 처리입니다.
    SVR_CARD_POS_ERR_ALREADY_OUT_NOT_RE_OUT         = 110811,       // 이미 위치 적용 해제되서 추가 해제할 수 없습니다.
    SVR_CARD_IMG_ERR_ALREADY_SAME_IMAGE             = 110812,       // 이미 같은 이미지가 적용중입니다.
    SVR_CARD_LV_ERR_ALREADY_LIMIT_VALUE             = 110813,       // 레벨업으로 올릴수 있는 최대 상태라서 진행할 수 없습니다.
    SVR_CARD_POS_ERR_IMPOSSIBLE_CHANGE_POS          = 110814,       // 교체가 불가능한 위치의 카드(서포터)입니다.
    //SVR_CARD_POS_ERR_APPLY_WHERE                    = 110815,       // 어딘가에서 위치 적용중인 카드(서포터)라 처리할 수 없습니다.
    SVR_CARD_POS_ERR_APPLY_UNKNOWN_POS_KIND         = 110816,       // 알 수 없는 위치라 적용할 수 없습니다.
    SVR_CARD_LV_ERR_NOT_SAME_TYPE_MATERIAL          = 110817,       // 다른 타입의 재료가 포함되서 레벨업 처리를 하지 못했습니다.
    SVR_CARD_RWD_ERR_ALREADY_FAVOR_GET_REWARD       = 110818,       // 이미 획득한 호감도 보상입니다.
    SVR_CARD_RWD_ERR_INVALID_REWARD_DATA            = 110819,       // 보상 테이블 정보가 잘못되서 보상을 획득하지 못했습니다.
    SVR_CARD_POS_ERR_OVER_MAX_POS_SLOT_NUM          = 110820,       // 적용 가능한 슬롯을 넘어서는 슬롯 번호라서 처리하지 못했습니다.
    SVR_CARD_POS_ERR_NOT_FIND_POS_SLOT_INST         = 110821,       // 해당 위치 슬롯 정보를 찾지 못해서 처리하지 못했습니다.
    SVR_CARD_POS_ERR_ALREADY_SAME_USED_IN_SLOT      = 110822,       // 해당 위치 적용중 다른 슬롯에 동일한 종류의 카드(서포터)가 사용중이라 처리하지 못했습니다.
    SVR_CARD_POS_ERR_NOW_FACILITY_OPERATING         = 110823,       // 해당 시설이 작동중이라 처리하지 못했습니다.
    SVR_CARD_POS_ERR_ALREADY_APPLY_USED             = 110824,       // 이미 위치 적용중인 카드(서포터)라서 처리하지 못했습니다.
    SVR_CARD_TYP_ERR_CHANGE_IMPOSSIBLE_INST         = 110825,       // 변경이 불가능한 카드(서포터)라서 처리하지 못했습니다.
    SVR_CARD_TYP_ERR_OUT_OF_VALID_RANGE_TO_TYPE     = 110826,       // 설정할 수 없는 속성으로 카드(서포터) 속성 변경을 요청했습니다.
    SVR_CARD_ERR_DISABLE_OPERATION_ROOM             = 110827,       // 작전회의실에 서포터를 적용할수 없습니다. 



    // 아이템 관련
    SVR_ITEM_COM_ERR_NOT_EXIST_INST                 = 110900,       // 존재하지 않는 아이템 입니다.
    SVR_ITEM_COM_ERR_LACK_COUNT_IN_ITEM             = 110901,       // 가지고 있는 아이템의 수량이 부족합니다.
    SVR_ITEM_USE_ERR_INVALID_TYPE                   = 110902,       // 사용할 수 없는 타입의 아이템입니다.
    SVR_ITEM_USE_ERR_OVER_TICKET                    = 110903,       // 소지 가능 최대 티켓 수 보다 많아져서 사용하지 못했습니다.
	SVR_ITEM_USE_ERR_NOT_EXIST_CODE                 = 110904,       // 아이템 소비하여 얻을 수 있는 코드가 존재하지 않습니다.
    SVR_ITEM_MULTI_USE_ERR_INVALID_BUYTYPE          = 110905,       // 다중 구매할 수 없는 buyType입니다. 
    SVR_ITEM_EXC_NOT_EXCHANGE_CASH                  = 110906,       // 캐시로 교환 가능한 아이템이 아닙니다.
	SVR_ITEM_USE_ERR_DIFF_SUBTYPE_ITEM_INCLUDE		= 110907,       // 동시에 사용할 수 없는 서브 타입의 아이템이 포함되어 처리할 수 없습니다.

	// 곡옥 관련
	SVR_GEM_COM_ERR_NOT_EXIST_INST                  = 111000,       // 존재하지 않는 곡옥 입니다.
    SVR_GEM_SELL_ERR_IN_WPN_SLOT                    = 111001,       // 무기에 장착중인 곡옥이라 판매할 수 없습니다.
    SVR_GEM_RSTOPT_ERR_NOT_ACTIVE_OPTION_SLOT       = 111002,       // 아직 활성화 되지 않은 옵션 슬롯 입니다.
    SVR_GEM_RSTOPT_ERR_DONT_SEL_PRE_OPTION          = 111003,       // 이전에 진행한 재설정 옵션을 아직 선택하지 않아서 처리할 수 없습니다.
    SVR_GEM_RSTOPTSEL_ERR_NOT_RESET_PRE             = 111004,       // 재설정을 진행하지 않아서 옵션을 선택할 수 없습니다.
    SVR_GEM_RSTOPTSEL_ERR_WRONG_RESET_OPT_INFO      = 111005,       // 재설정 정보가 잘못 되서 처리하지 못했습니다.
	SVR_GEM_COM_ERR_IS_ALREADY_ANALYZE				= 111006,        // 이미 세트 감정이 완료된 곡옥이므로 처리하지 못했습니다.


	// 무기 관련
	SVR_WPN_COM_ERR_NOT_EXIST_INST                  = 111100,       // 존재하지 않는 무기 입니다.
    SVR_WPN_COM_ERR_EQUIPED_CHARACTER               = 111101,       // 캐릭터가 장착중인 무기라 처리할 수 없습니다.
    SVR_WPN_LV_ERR_ADD_EXP_ZERO                     = 111102,       // 무기에 적용할 경험치가 0입니다.
    SVR_WPN_LV_ERR_OVER_SIZE_MATERIAL_WPN           = 111103,       // 재료로 사용될 무기의 수가 너무 많습니다.
    SVR_WPN_LV_ERR_NOT_EXIST_MATERIAL               = 111104,       // 레벨업 재료에 존재하지 않는 무기가 있습니다.
    SVR_WPN_WAK_ERR_NOT_EXIST_ITEMREQLIST           = 111105,       // 각성에 필요한 재료 아이템 테이블 정보가 존재하지 않습니다.
    SVR_WPN_SLOT_ERR_APPLY_GEM_COUNT                = 111106,       // 적용하려는 곡옥의 수량이 잘못됐습니다.
    SVR_WPN_SLOT_ERR_SAME_ATTACH_GEM_SLOT_ALL       = 111107,       // 부착된 곡옥과 부착하려는 곡옥이 모두 같습니다.
    SVR_WPN_SLOT_ERR_NOT_EXIST_ATTACH_GEM           = 111108,       // 곡옥이 장착되지 않은 슬롯입니다.
    SVR_WPN_SLOT_ERR_NOT_EXIST_ATTACH_GEM_ALL       = 111109,       // 장착된 곡옥이 하나도 없습니다.
    SVR_WPN_SLOT_ERR_ALREADY_ATTACH_GEM             = 111110,       // 이미 장착된 곡옥이라서 장착할 수 없습니다.
    SVR_WPN_SLOT_ERR_TRY_ATTACH_SAME_GEM            = 111111,       // 동일한 곡옥을 다른 슬롯에 장착할 수 없습니다.
    SVR_WPN_EQUIP_IMPOSSIBLE_CHAR_TYPE              = 111112,       // 원하는 캐릭터가 장착할 수 없는 타입의 무기입니다.
    SVR_WPN_DEPOT_LACK_OPEN_SLOT_NEED_BOOK          = 111113,       // 무기 창고 슬롯 확장에 필요한 도감의 개수가 부족합니다.
    SVR_WPN_COM_APPLY_IN_DEPOT_SLOT                 = 111114,       // 무기 창고 배치중인 무기라 처리할 수 없습니다.
    SVR_WPN_COM_BEING_SAME_TID_WPN                  = 111115,       // 같은 무기가 존재해서 처리하지 못했습니다.


    // 도감 관련
    SVR_BOOK_COM_ALREADY_CONFIRM                    = 111200,       // 이미 확인된 도감입니다.
    SVR_BOOK_CARD_ERR_NOT_EXIST_INST                = 111201,       // 존재하지 않는 카드(서포터) 도감 입니다.
    SVR_BOOK_CARD_ERR_NOT_EXIST_TABLE_DATA          = 111202,       // 카드(서포터) 도감 테이블 데이타가 존재하지 않습니다.
    SVR_BOOK_CARD_LV_ERR_ADD_EXP_ZERO               = 111203,       // 카드(서포터) 도감에 적용할 경험치가 0입니다.
    SVR_BOOK_CARD_LV_ERR_ALREADY_MAX_LEVEL          = 111204,       // 이미 최대 레벨 카드(서포터) 도감입니다.
    SVR_BOOK_WPN_ERR_NOT_EXIST_INST                 = 111205,       // 존재하지 않는 무기 도감 입니다.
    SVR_BOOK_MON_ERR_NOT_EXIST_INST                 = 111206,       // 존재하지 않는 몬스터 도감 입니다.


    // 아이템 제품군 관련
    SVR_PRDT_ERR_DEFAULT                            = 111300,       // 결과물 구성 기본 오류입니다.
    SVR_PRDT_ERR_ALREADY_COSTUME_NOT_ADD            = 111301,       // 이미 존재하는 코스츔이라 추가할 수 없습니다.
    SVR_PRDT_ERR_CONTINUE_SAME_COSTUME_ADD          = 111302,       // 동일한 코스츔을 연속해서 추가하려고 했습니다.


    // 상점 관련
    SVR_STOR_COM_ERR_DEFAULT                        = 111400,       // 상점 관련 기본 오류입니다.
    SVR_STOR_COM_ERR_WRONG_RANDOM_TABLE_IN_DATA     = 111401,       // 랜덤 테이블 안의 데이터가 잘못 됐습니다.
    SVR_STOR_PUC_ERR_NOT_DURING_SEASON_SALE         = 111402,       // 해당 상품의 구매 가능 시기가 아니라서 처리할 수 없습니다.
    SVR_STOR_PUC_ERR_NOT_OVER_CYCLEMINUTE           = 111403,       // 해당 상품의 무료 구매 시간이 아니여서 처리할 수 없습니다.
    SVR_STOR_PUC_ERR_ALREADY_OVER_LIMIT_COUNT       = 111404,       // 해당 상품의 구매 제한 개수를 초과해서 처리할 수 없습니다.
    SVR_STOR_RGC_ERR_NOT_EXIST_INST                 = 111405,       // 존재하지 않는 로테이션 가챠 정보 입니다.
    SVR_STOR_RGC_ERR_ALREADY_EXIST_ACTIVE_RGACHA    = 111406,       // 이미 활성화된 로테이션 가챠가 존재해서 처리하지 못했습니다.
    SVR_STOR_PUC_ERR_NOW_RESTRICT_CONDI_STATE       = 111407,       // 해당 상품은 구매 제한 조건에 의해 처리하지 못했습니다.
    SVR_STOR_PUC_ERR_NOT_BUY_NEED_PRODUCT           = 111408,       // 먼저 구매 해야할 상품을 구매하지 않아서 처리하지 못했습니다.
    SVR_STOR_NOT_EXIST_UNEXPECTEDPACKAGE            = 111409,       // 돌발 패키지가 존재하지 않습니다. 
    SVR_STOR_NOT_PURCHASE_UNEXPECTEDPACKAGE         = 111410,       // 구매하지 않은 돌발 패키지 입니다. 
    SVR_STOR_DISABLE_TIME_DAILY_REWARD              = 111411,       // 데일리보상을 획득할 수 없는 시간입니다.
    SVR_STOR_RAID_NOT_EXIST_RESET_ITEM_LIST         = 111412,       // 레이드 상점에 리셋 가능한 항목이 없습니다.
    SVR_STOR_RAID_NOT_SHOWING_ITEM                  = 111413,       // 레이드 상점에 전시중인 상태가 아닌 항목이라 처리하지 못했습니다.
    SVR_STOR_RAID_NOT_FIND_RESET_ITEM_IN_LIST       = 111414,       // 레이드 상점에 리셋을 원하는 항목을 목록안에서 찾을 수 없습니다.

    SVR_STOR_INAPP_ERR_INVALID                      = 111430,       // 무효한 인 앱 에러입니다.
    SVR_STOR_INAPP_ERR_UNKNOWN_RECEIPT              = 111431,       // 인 앱 영수증 검증 관련 알 수 없는 오류입니다.
    SVR_STOR_INAPP_ERR_NOT_EXIST_RECEIPT            = 111432,       // 존재하지 않는 인 앱 영수증 입니다.
    SVR_STOR_INAPP_ERR_UNKNOWN_HTTP                 = 111433,       // 결재 검증 도중 알 수 없는 HTTP 통신 에러가 발생했습니다.
    SVR_STOR_INAPP_ERR_WORK_HTTP                    = 111434,       // 결재 검증 도중 HTTP 처리에 오류가 발생했습니다.
    SVR_STOR_INAPP_ERR_INVALID_INAPP_KIND           = 111435,       // 무효한 인 앱 분류라 처리하지 못했습니다.
    SVR_STOR_INAPP_ERR_EMPTY_PRODUCT_ID             = 111436,       // Product ID 비어 있는 상품이라 처리하지 못했습니다.
    SVR_STOR_INAPP_ERR_ALREADY_USE_RECEIPT_INFO     = 111437,       // 이미 사용한 영수증 정보라서 처리하지 못했습니다.
    SVR_STOR_INAPP_ERR_NOT_SAME_PRODUCT_ID          = 111438,       // 결재 검증 진행중에 구매한 제품과 영수중에 포함된 제품 ID가 같지 않아서 처리하지 못했습니다.
    SVR_STOR_INAPP_ERR_NOT_READ_JSON_OBJ            = 111450,       // 결재 검증 진행중에 앱 스토어가 제공 한 JSON 객체를 읽을 수 없습니다.
    SVR_STOR_INAPP_ERR_UNKNOWN_SEND_FORMAT          = 111451,       // 결재 검증 진행중에 수신 데이터 속성의 데이터 형식이 잘못되었습니다.
    SVR_STOR_INAPP_ERR_UNABLE_RECEIPT_VERIFY        = 111452,       // 결재 검증 진행중에 영수증 인증 할 수 없습니다.
    SVR_STOR_INAPP_ERR_NOT_SAME_ACCOUNT_FILE_PW     = 111453,       // 결재 검증 진행중에 당신이 제공하는 공유 비밀은 귀하의 계정에 대한 파일의 공유 비밀과 일치하지 않습니다.
    SVR_STOR_INAPP_ERR_DONT_USE_VERIFY_SERVER       = 111454,       // 결재 검증 진행중에 수신 서버는 현재 사용할 수 없습니다.
    SVR_STOR_INAPP_ERR_FINISHED_SUBSCRIBE           = 111455,       // 결재 검증 진행중에 이 영수증은 유효하지만 구독이 만료되었습니다. 이 상태 코드는 서버에 반환 될 때, 수신 데이터는 디코딩과 응답의 일부로 반환됩니다.
    SVR_STOR_INAPP_ERR_RETRY_SANDBOX_VERIFY         = 111456,       // 결재 검증 진행중에 이 영수증은 샌드 박스 영수증이지만 확인을 위해 생산 서비스로 전송되었습니다.
    SVR_STOR_INAPP_ERR_RETRY_BUY_VERIFY             = 111457,       // 결재 검증 진행중에 이 영수증은 제품 수령이지만 확인을 위해 샌드 박스 서비스로 전송되었습니다.
    SVR_STOR_INAPP_ERR_FAIL_TAKE_ACCESS_TOKEN       = 111460,       // 결재 검증 진행중에 엑세스 토큰 획득에 실패했습니다.
    SVR_STOR_INAPP_ERR_NOT_SAME_OTHER_ID            = 111461,       // 결재 검증 진행중에 구매한 제품과 영수중에 포함된 주문 고유 ID가 같지 않아서 처리하지 못했습니다.
    SVR_STOR_INAPP_ERR_CANCELED_APPROVAL            = 111462,       // 결재 검증 진행중에 영수증이 승인 취소된 상품이라서 처리하지 못했습니다.
    SVR_STOR_INAPP_ERR_WAITING_FOR_APPROVAL         = 111463,       // 결재 검증 진행중에 해당 영수증이 승인 대기중인 상품이라 처리하지 못했습니다.
	SVR_STOR_INAPP_ERR_STEAM_INITTXN_FAILURE		= 111464,       // 스팀 결제 과정 중 InitTxn이 실패했습니다.
	SVR_STOR_INAPP_ERR_STEAM_FINALIZETXN_FAILURE	= 111465,       // 스팀 결제 과정 중 FinalizeTxn이 실패했습니다.
    SVR_STOR_INAPP_ERR_RESULT_EMPTY_IN_INAPP_INFO   = 111466,       // 결재 검증 진행중에 영수증 검증 결과 안에 INAPP 정보가 비어 있어서 처리하지 못했습니다.

    SVR_STOR_INAPP_ERR_STEAM_CODE_1                 = 111471,       // 결제 관련 내부 오류입니다.
    SVR_STOR_INAPP_ERR_STEAM_CODE_2                 = 111472,       // 결제 관련 사용자가 아직 거래를 승인하지 않았습니다.
    SVR_STOR_INAPP_ERR_STEAM_CODE_3                 = 111473,       // 결제 관련 거래가 이미 성사됐습니다.
    SVR_STOR_INAPP_ERR_STEAM_CODE_4                 = 111474,       // 결제 관련 사용자가 로그인하지 않았습니다.
    SVR_STOR_INAPP_ERR_STEAM_CODE_5                 = 111475,       // 결제 관련 통화가 사용자의 Steam 계정 통화와 일치하지 않습니다.
    SVR_STOR_INAPP_ERR_STEAM_CODE_6                 = 111476,       // 결제 관련 계정이 존재하지 않거나 일시적으로 이용할 수 없는 상태입니다.
    SVR_STOR_INAPP_ERR_STEAM_CODE_7                 = 111477,       // 결제 관련 사용자가 거래를 거부했습니다.
    SVR_STOR_INAPP_ERR_STEAM_CODE_8                 = 111478,       // 결제 관련 사용자가 제한된 국가에 거주 중이라 거래가 거부됐습니다.
    SVR_STOR_INAPP_ERR_STEAM_CODE_9                 = 111479,       // 결제 관련 사용자의 청구 동의서가 활성화되지 않아 거래가 거부됐습니다.
    SVR_STOR_INAPP_ERR_STEAM_CODE_10                = 111480,       // 결제 관련 GAME 유형이 아니므로 청구 동의서 처리 불가능합니다.
    SVR_STOR_INAPP_ERR_STEAM_CODE_11                = 111481,       // 결제 관련 청구 불능 또는 결제 거부로 인해 청구 동의서가 보류된 상태입니다.
    SVR_STOR_INAPP_ERR_STEAM_CODE_12                = 111482,       // 결제 관련 STEAM 유형이 아니므로 청구 동의서 처리 불가능합니다.
    SVR_STOR_INAPP_ERR_STEAM_CODE_13                = 111483,       // 결제 관련 사용자에게 이미 본 게임의 청구 동의서가 있습니다.
    SVR_STOR_INAPP_ERR_STEAM_CODE_14                = 111484,       // 결제 관련 사용자의 자금 부족으로 처리하지 못했습니다.
    SVR_STOR_INAPP_ERR_STEAM_CODE_15                = 111485,       // 결제 관련 거래 완료 시간 제한 초과로 처리하지 못했습니다.
    SVR_STOR_INAPP_ERR_STEAM_CODE_16                = 111486,       // 결제 관련 비활성화된 계정입니다.
    SVR_STOR_INAPP_ERR_STEAM_CODE_17                = 111487,       // 결제 관련 구매할 권한이 없는 계정입니다.
    SVR_STOR_INAPP_ERR_STEAM_CODE_18                = 111488,       // 결제 관련 사기 감지로 인해 거래가 거부됐습니다.
    SVR_STOR_INAPP_ERR_STEAM_CODE_19                = 111489,       // 결제 관련 캐시된 결제 메서드가 존재하지 않습니다.
    SVR_STOR_INAPP_ERR_STEAM_CODE_20                = 111490,       // 결제 관련 거래가 청구 동의서의 지출 한도를 초과했습니다.


    // 메일 관련
    SVR_MAIL_COM_ERR_NOT_EXIST_INST                 = 111500,       // 존재하지 않는 메일 입니다.
    SVR_MAIL_LIST_ERR_START_INDEX_OVER              = 111501,       // 메일 목록의 시작 위치를 가지고 있는 최대 개수보다 큰 값으로 했습니다.
    SVR_MAIL_TAKE_ERR_EXIST_OVER_TIME               = 111502,       // 유지 시간이 지난 메일이 포함되서 관련 처리를 진행할 수 없습니다. 메일 목록을 갱신해 주세요.
    SVR_MAIL_TAKE_ERR_EXIST_SAME_TAKE_UID           = 111503,       // 메일 받기 목록에 동일한 메일을 포함해서 처리할 수 없습니다.


    // 미션 관련
    SVR_MSIN_COM_ERR_NOT_EXIST_INST                 = 111600,       // 존재하지 않는 미션 입니다.
    SVR_MSIN_RWD_ERR_NOT_COMPLETE                   = 111601,       // 아직 완료하지 못해서 보상을 받을 수 없습니다.
    SVR_MSIN_RWD_ERR_ALREADY_TAKE                   = 111602,       // 이미 수령 받은 보상입니다.
    SVR_MSIN_RST_ERR_NOT_OVER_RESET_TIME            = 111603,       // 재설정 시간이 되지 않아서 진행할 수 없습니다.

    SVR_MSIN_COM_NOT_EXIST_CHANGE_INFLU             = 111630,       // 변경할 서버 달성(세력) 미션 존재하지 않습니다.
    SVR_MSIN_COM_ALREADY_CHOICE_INFLUENCE_ID        = 111631,       // 이미 세력을 선택해서 처리하지 못했습니다.

    SVR_MSIN_COM_NOT_EXIST_DAILY_INST               = 111640,       // 변경할 일일 미션이 존재하지 않습니다.


    // 이벤트 관련
    SVR_EVT_COM_ERR_NOT_EXIST_INST                  = 111700,       // 존재하지 않는 이벤트 입니다.
    SVR_EVT_COM_ERR_NOT_ACTIVE                      = 111701,       // 활성화 되지 않은 이벤트입니다.
    SVR_EVT_COM_NOT_EXIST_CHANGE_EVENT              = 111702,       // 변경할 이벤트가 존재 하지 않습니다.
    SVR_EVT_STGE_ERR_NOT_ON_PLAY_TIME               = 111703,       // 스테이지 진행 가능 시간이 아닌 이벤트입니다.
    SVR_EVT_COM_ERR_NOW_CHANGING_EVENT              = 111704,       // 이벤트가 변경중이라 처리할 수 없습니다.
    SVR_EVT_COM_ERR_NOT_EXIST_REWARD_INST           = 111705,       // 해당 이벤트의 보상 정보가 존재하지 않습니다.
    SVR_EVT_COM_ERR_LACK_REMAIN_REWARD_COUNT        = 111706,       // 요청한 수량 보다 남아있는 보상 수량이 부족합니다.
    SVR_EVT_COM_ERR_LACK_REMAIN_EVENT_ITEM_COUNT    = 111707,       // 요청한 수량 보다 남아있는 이벤트 아이템 수량이 부족합니다.
    SVR_EVT_COM_ERR_CANT_RESET_EVENT_TYPE           = 111708,       // 재설정 처리를 할 수 없는 이벤트 입니다.
    SVR_EVT_COM_ERR_NOT_ALL_TAKE_RESET_PRODUCT      = 111709,       // 재설정 가능 아이템을 모두 획득하지 않아서 재설정을 진행하지 못했습니다.
    SVR_EVT_LGN_ERR_NOT_EXIST_INST                  = 111720,       // 존재하지 않는 로그인 이벤트 입니다.
    SVR_EVT_LGN_ERR_NOT_TAKE_REQUEST                = 111721,       // 요청할 수 없는 로그인 이벤트 보상입니다.
	SVR_EVT_BINGO_ERR_NOT_EXIST_INST				= 111722,		// 존재하지 않는 빙고 이벤트 입니다.
	SVR_EVT_BINGO_ERR_NOT_ENOUGH_REWARD_LINE		= 111723,		// 빙고 이벤트 보상 요청의 완료 라인 수가 부족 합니다.
	SVR_EVT_BINGO_ERR_ALREADY_REWARD_LINE			= 111724,		// 이미 수령한 빙고 이벤트 보상 입니다.
	SVR_EVT_BINGO_ERR_EVENT_END						= 111725,		// 종료된 빙고 이벤트 입니다.
    SVR_EVT_ACHIEVE_ERR_EVENT_END                   = 111726,       // 해당 이벤트가 종료되어 더 이상 보상을 받을 수 없습니다.


    // 공적 관련
    SVR_ACHV_COM_ERR_NOT_EXIST_INST                 = 111800,       // 존재하지 않는 공적입니다.
    SVR_ACHV_RWD_ERR_ALREADY_MAX_REWARD_GET         = 111801,       // 이미 최대 보상을 받은 공적입니다.
    SVR_ACHV_RWD_ERR_LACK_GROUP_POINT               = 111802,       // 포인트가 부족하여 해당 공적 보상을 받을 수 없습니다.
    SVR_ACHV_RWD_ERR_NOT_CLEAR_STATE                = 111803,       // 스테이지를 클리어한 상태가 아니여서 해당 공적 보상을 받을 수 없습니다.
    SVR_ACHV_RWD_ERR_LACK_USER_LV                   = 111804,       // 지휘관의 목표 레벨을 달성한 상태가 아니여서 해당 공적 보상을 받을 수 없습니다.
    SVR_ACHV_RWD_ERR_LACK_CHAR_LV                   = 111805,       // 해당 캐릭터가 목표 레벨을 달성한 상태가 아니여서 해당 공적 보상을 받을 수 없습니다.
    SVR_ACHV_RWD_ERR_LACK_CHAR_GRADE                = 111806,       // 해당 캐릭터가 목표 진급에 도달한 상태가 아니여서 해당 공적 보상을 받을 수 없습니다.
    SVR_ACHV_RWD_ERR_LACK_CHAR_LV_COUNT             = 111807,       // 특정 레벨을 달성한 캐릭터 수가 부족해 공적 보상을 받을 수 없습니다.


    // 문양 관련
    SVR_BADG_COM_ERR_NOT_EXIST_INST                 = 111900,       // 존재하지 않는 문양 입니다.
    SVR_BADG_SELL_ERR_IN_WPN_SLOT                   = 111901,       // 무기에 장착중인 문양이라 판매할 수 없습니다.
    SVR_BADG_RSTOPT_ERR_NOT_ACTIVE_OPTION_SLOT      = 111902,       // 아직 활성화 되지 않은 옵션 슬롯 입니다.
    SVR_BADG_LV_ERR_LACK_LVUP_POINT                 = 111903,       // 남은 강화 횟수가 부족해서 처리할 수 없습니다.
    SVR_BADG_LV_RESET_ERR_LOW_TRY_LVUP_CNT          = 111904,       // 강회 진행 횟수 부족으로 초기화 처리를 할 수 없습니다.
    SVR_BADG_POS_ERR_ALREADY_SAME_ARENA             = 111905,       // 이미 같은 아레나에서 문양을 위치 적용중입니다.
    SVR_BADG_POS_ERR_ALREADY_SAME_USED              = 111906,       // 이미 동일한 대상이 위치 적용중인 문양이라 처리할 수 없습니다.
    SVR_BADG_POS_ERR_ALREADY_OUT_NOT_RE_OUT         = 111907,       // 이미 위치 적용 해제되서 추가 해제할 수 없습니다.
    SVR_BADG_POS_ERR_APPLY_UNKNOWN_POS_KIND         = 111908,       // 알수 없는 위치라 적용할 수 없습니다.
    SVR_BADG_POS_ERR_IMPOSSIBLE_CHANGE_POS          = 111909,       // 교체가 불가능한 위치의 문양입니다.
    SVR_BADG_POS_ERR_OVER_MAX_POS_SLOT_NUM          = 111910,       // 적용 가능한 슬롯을 넘어서는 슬롯 번호라서 처리하지 못했습니다.
    SVR_BADG_POS_ERR_NOT_FIND_POS_SLOT_INST         = 111911,       // 해당 위치 슬롯 정보를 찾지 못해서 처리하지 못했습니다.


    // PVP 관련
    SVR_ARN_COM_ERR_ALREADY_START_SEASON            = 112000,       // 유저가 이미 해당 시즌을 시작하였습니다.
    SVR_ARN_COM_ERR_NOT_FIND_REWARD_DATA            = 112001,       // 보상 데이터를 찾을 수 없습니다.
    SVR_ARN_COM_ERR_NOW_SEASON_READY_TIME           = 112002,       // 시즌 준비중인 기간입니다.
    SVR_ARN_COM_ERR_NOT_ARENA_ITEM                  = 112003,       // PVP에서 사용하는 아이템이 아닙니다.
    SVR_ARN_COM_ERR_LACK_TEAM_ORDER                 = 112004,       // 팀 편성 순서가 잘못되었습니다.
    SVR_ARN_COM_ERR_LACK_GAME_RESULT                = 112005,       // 게임 결과 수치가 잘못되었습니다.
    SVR_ARN_COM_ERR_USER_NOT_SEASON_START           = 112006,       // 해당 PVP 시즌 시작 처리가 되지 않았습니다.
    SVR_ARN_COM_ERR_NOT_GAME_START                  = 112007,       // PVP 대전이 정상적으로 시작되지 않았습니다.
    SVR_ARN_RANK_ERR_NOT_FIND_GRADE_DATA            = 112008,       // 해당 등급의 데이터를 찾을 수 없습니다.
    SVR_ARN_RANK_ERR_NOT_FIND_UUID                  = 112009,       // 요청한 UUID는 PVP 랭킹에 존재하지 않습니다.
    SVR_ARN_RANK_ERR_NOT_FIND_ENEMY_INFO            = 112010,       // 상대 유저 정보를 찾지 못했습니다.
    SVR_ARN_COM_ERR_LACK_ARENA_PROC                 = 112011,       // 아레나 진행 수치가 부족해서 처리 하지 못했습니다.


    // 패스 관련
    SVR_PAS_COM_ERR_NOT_EXIST_INST                  = 112100,       // 존재하지 않는 패스 입니다.
    SVR_PAS_COM_ERR_NOT_ACTIVE                      = 112101,       // 활성화 되지 않은 패스입니다.
    SVR_PAS_COM_NOT_EXIST_CHANGE_PASS               = 112102,       // 변경할 패스가 존재 하지 않습니다.
    SVR_PAS_COM_ERR_NOW_CHANGING_PASS               = 112103,       // 패스가 변경중이라 처리할 수 없습니다.
    SVR_PAS_COM_ERR_NOT_EXIST_MISSION_INST          = 112104,       // 해당 패스의 미션 정보가 존재하지 않습니다.
    SVR_PAS_RWD_ERR_ALREADY_PASS_MISSION            = 112105,       // 이미 해당 패스 미션 완료 보상을 받아서 처리하지 못했습니다.
    SVR_PAS_RWD_ERR_LACK_PASS_POINT                 = 112106,       // 획득한 패스 포인트 부족으로 처리하지 못했습니다.
    SVR_PAS_RWD_ERR_NOT_BUY_SPECIAL_PASS            = 112107,       // 추가 패스 보상을 구매하지 않아서 처리하지 못했습니다.
    SVR_PAS_RWD_ERR_NOT_COMPLETE_MISSION            = 112108,       // 아직 완료되지 않은 미션이라서 처리하지 못했습니다.
    SVR_PAS_BUY_ERR_NOT_ACTIVE_PASS                 = 112109,       // 해당 패스가 활성화되지 않아서 구매하지 못했습니다.
    SVR_PAS_BUY_ERR_ALREADY_BUY_SAME_PASS           = 112110,       // 이미 해당 패스 구매 효과가 적용중인 상태라 구매하지 못했습니다.


    // 커뮤니티 관련
    SVR_CMM_COM_ERR_NOT_EXIST_INST                  = 112200,       // 존재하지 않는 서버 커뮤니티 유저 정보 입니다.
    SVR_CMM_COM_ERR_NOT_EXIST_TARGET_INST           = 112201,       // 대상의 서버 커뮤니티 유저 정보가 존재하지 않습니다.
    SVR_CMM_SUG_ERR_NOT_EXIST_INST                  = 112202,       // 요청한 추천 커뮤니티 유저 정보를 찾지 못했습니다.
    SVR_CMM_COM_ERR_NOT_EXIST_ARENA_INST            = 112203,       // 요청한 커뮤니티 유저의 아레나 정보를 찾을 수 없습니다.


    // 친구 관련
    SVR_FRI_COM_ERR_NOT_EXIST_INST                  = 112300,       // 존재하지 않는 친구 정보 입니다.
    SVR_FRI_COM_ERR_NOT_EXIST_TARGET_FRIENDLIST     = 112301,       // 대상의 친구 목록 정보가 존재하지 않아 처리하지 못했습니다.
    SVR_FRI_COM_ERR_NOT_EXIST_TARGET_IN_MY_FRIENDLIST   = 112302,   // 대상 유저가 내 친구 목록에 존재하지 않아 처리하지 못했습니다.
    SVR_FRI_COM_ERR_NOT_EXIST_ME_IN_TARGET_FRIENDLIST   = 112303,   // 대상 유저의 친구 목록에 내 정보가 존재하지 않아 처리하지 못했습니다.
    SVR_FRI_COM_ERR_CANT_ADD_OVER_FRIENDLIST        = 112304,       // 친구 목록 최대 개수를 넘어서 처리하지 못했습니다.
    SVR_FRI_COM_ERR_ZERO_USER_IN_FRIENDLIST         = 112305,       // 친구 목록에 친구가 하나도 없어서 처리하지 못했습니다.
    SVR_FRI_ASK_ERR_CANT_ADD_OVER_ASKLIST           = 112306,       // 친구 신청 목록 최대 개수를 넘어서 처리하지 못했습니다.
    SVR_FRI_ASK_ERR_ALREADY_TARGET_IN_MY_FRIENDLIST = 112307,       // 신청하려는 유저가 이미 등록된 친구라 처리하지 못햇습니다.
    SVR_FRI_ASK_ERR_ALREADY_TARGET_IN_MY_FROMLIST   = 112308,       // 친성하려는 유저가 이미 나에게 신청을 보낸 상태라 처리하지 못했습니다.
    SVR_FRI_ASK_ERR_ALREADY_TARGET_IN_MY_ASKLIST    = 112309,       // 신청하려는 유저가 이미 신청 목록에 포함된 상태라 처리하지 못했습니다.
    SVR_FRI_ASK_ERR_OVER_COUNT_TARGET_FROMLIST      = 112310,       // 신청하려는 유저의 승인 대기 인원이 너무 많아 처리하지 못했습니다.
    SVR_FRI_ASK_ERR_NOT_EXIST_INST_TO_TARGET        = 112311,       // 친구 신청할 유저 정보를 찾지 못해서 처리하지 못했습니다.
    SVR_FRI_ASW_ERR_NOT_EXIST_TARGET_IN_MY_FROMLIST = 112312,       // 승인 목록에서 찾을 수 없는 대상이 있어서 처리하지 못했습니다.
    SVR_FRI_ASW_ERR_NOT_EXIST_ME_IN_TARGET_ASKLIST  = 112313,       // 대상이 나를 신청한적이 없어서 처리하지 못했습니다.
    SVR_FRI_ASW_ERR_ACCEPT_CANT_ADD_OVER_TARGET_FRIEND  = 112314,   // 승인 대상의 친구 목록이 최대 개수를 넘어서 처리하지 못했습니다.
    SVR_FRI_TAKE_ERR_EXIST_CANT_TAKE_STATE_USER     = 112315,       // 요청 대상 중에 획득할 수 없는 상태의 유저가 포함되서 처리하지 못했습니다.
    SVR_FRI_ROOM_ERR_CANT_VISIT_BECAUSE_CLOSE       = 112316,       // 해당 유저는 프라이빗룸 입장에 막혀있어서 처리하지 못했습니다.


    // 파견 관련
    SVR_DSP_COM_ERR_NOT_EXIST_INST                  = 112400,       // 존재하지 않는 파견 정보입니다.
    SVR_DSP_COM_ERR_ALREADY_WORKING_MISSION         = 112401,       // 이미 파견 임무가 진행중인 상태라서 처리하지 못했습니다.
    SVR_DSP_OPEN_ERR_ALREADY_EXIST_INST             = 112402,       // 이미 존재하는 파견 슬롯이라 개방하지 못했습니다.
    SVR_DSP_OPR_ERR_NOT_SAME_CARDTYPE_IN_SLOT       = 112403,       // 슬롯에 알맞지 않은 카드(서포터)타입을 적용했습니다.
    SVR_DSP_OPR_ERR_LACK_UR_GRADE_CARD              = 112404,       // UR 등급의 카드(서포터)가 부족하서 파견 임무를 진행하지 못했습니다.
    SVR_DSP_OPR_ERR_NOT_ENOUGH_CARD_TPYE            = 112405,       // 파견 임부 조건중 카드(서포터) 타입이 만족하지 못해서 처리하지 못했습니다.
    SVR_DSP_OPR_ERR_OVER_COUNT_MISSION_CARD         = 112406,       // 해당 파견 임무를 진행할 카드(서포터)의 최대 개수를 넘어서 처리할 수 없습니다.
    SVR_DSP_CONF_ERR_NOT_STARTED                    = 112407,       // 해당 파견 임무가 시작되지 않은 상태라서 요청을 처리하지 못했습니다.
    SVR_DSP_OPR_ERR_LACK_COUNT_CARD                 = 112408,       // 해당 파견 임무를 진행할 카드(서포터)의 개수가 부족해서 처리하지 못했습니다.


    // 아레나 타워 관련
    SVR_ATWR_STR_ERR_NOT_ALREADY_START              = 112500,       // 이미 아레나 타워를 진행중이라 처리하지 못했습니다.
    SVR_ATWR_STR_ERR_NOT_CLEAR_BEFORE_TOWER_STEP    = 112501,       // 이전 단계 아레나 타워를 클리어하지 않아서 처리하지 못했습니다.
    SVR_ATWR_STR_ERR_NOW_LAST_STEP_CLEAR            = 112502,       // 최종 단계 아레나 타워를 클리어해서 처리하지 못했습니다.
    SVR_ATWR_STR_ERR_LACK_USE_FRI_COUNT             = 112503,       // 친구 지원 가능 횟수가 부족해서 처리하지 못했습니다.
    SVR_ATWR_END_ERR_NOT_START                      = 112504,       // 아레나 타워를 시작하지 않아서 종료 처리를 하지 못했습니다.


    // 효과 관련
    SVR_EFF_ADD_ERR_ALREADY_EXIST_BUFF              = 112600,       // 이미 존재하는 버프라 추가하지 못했습니다.


    // 작전회의실 관련
    SVR_OPERROOM_ERR_ALREADY_EXIST_TID              = 112700,       // 이미 존재하는 작전회의입니다.
    SVR_OPERROOM_ERR_PARTICIPANT_MAX                = 112701,       // 요청한 작전회의 캐릭터수가 최대치를 초과합니다.
    SVR_OPERROOM_ERR_DISABLE_ACCEl                  = 112702,       // 가속을 사용할 수 없는 시설입니다. 
    SVR_OPERROOM_ERR_NOT_EXIST_TID                  = 112703,       // 작전회의를 완료할 캐릭터가 존재하지 않습니다. 

	// 유저 프리셋 관련
	SVR_USER_PRESET_ERR_INVALID_PRESET_KIND			= 112750,		// 잘못된 프리셋 분류 요청 입니다.
	SVR_USER_PRESET_ERR_IS_NOT_EXSITS_PRESET		= 112751,       // 존재 하지않는 프리셋 입니다.
	SVR_USER_PRESET_ERR_IS_NEED_CUID				= 112752,       // 캐릭터 UID가 필요한 요청 입니다.
	SVR_USER_PRESET_ERR_IS_EMPTY_MAIN_WPN_PRESET	= 112753,       // 메인 무기가 없는 캐릭터가 편성된 프리셋을 불러오기 요청 하였습니다.
	SVR_USER_PRESET_ERR_IS_OTHER_CHAR_MAIN_WPN		= 112754,       // 현재 다른 캐릭터의 메인무기로 사용 중이므로 프리셋을 불러올 수 없습니다.

	// 공적 이벤트 관련 
	SVR_ACHV_EVT_COM_ERR_NOT_EXIST_INST             = 112800,       // 존재하지 않는 공적이벤트입니다.
    SVR_ACHV_EVT_RWD_ERR_ALREADY_MAX_REWARD_GET     = 112801,       // 이미 최대 보상을 받은 공적이벤트입니다.
    SVR_ACHV_EVT_RWD_ERR_LACK_GROUP_POINT           = 112802,       // 포인트가 부족하여 해당 공적이벤트 보상을 받을 수 없습니다.
    SVR_ACHV_EVT_RWD_ERR_NOT_CLEAR_STATE            = 112803,       // 스테이지를 클리어한 상태가 아니여서 해당 공적이벤트 보상을 받을 수 없습니다.
    SVR_ACHV_EVT_RWD_ERR_LACK_USER_LV               = 112804,       // 지휘관의 목표 레벨을 달성한 상태가 아니여서 해당 공적이벤트 보상을 받을 수 없습니다.
    SVR_ACHV_EVT_RWD_ERR_LACK_CHAR_LV               = 112805,       // 해당 캐릭터가 목표 레벨을 달성한 상태가 아니여서 해당 공적이벤트 보상을 받을 수 없습니다.
    SVR_ACHV_EVT_RWD_ERR_LACK_CHAR_GRADE            = 112806,       // 해당 캐릭터가 목표 진급에 도달한 상태가 아니여서 해당 공적이벤트 보상을 받을 수 없습니다.
    SVR_ACHV_EVT_RWD_ERR_LACK_CHAR_LV_COUNT         = 112807,       // 특정 레벨을 달성한 캐릭터 수가 부족해 공적이벤트 보상을 받을 수 없습니다.


    // 레이드 관련
    SVR_RAID_CHAR_COM_ERR_NOT_EXIST_INST            = 112900,      // 존재하지 않는 레이드 캐릭터 입니다.
    SVR_RAID_COM_ERR_NOT_RAID_ITEM                  = 112901,      // 레이드에서 사용하는 아이템이 아닙니다.
    SVR_RAID_COM_ERR_NOW_SEASON_READY_TIME          = 112902,      // 시즌 준비중인 기간입니다.
    SVR_RAID_COM_ERR_USER_NOT_SEASON_START          = 112903,      // 해당 레이드 시즌 시작 처리가 되지 않았습니다.
    SVR_RAID_STAGE_FAIL_CHECK_CLEARTIME             = 112904,      // 해당 스테이지 클리어타임 조건을 충족하지 못했습니다.
    SVR_RAID_STAGE_LEVEL_NOT_OPEN                   = 112905,      // 해당 스테이지는 아직 오픈되지 않았습니다.
    SVR_RAID_SEASON_DATA_NEED_INIT                  = 112906,      // 레이드 시즌 데이터 초기화가 필요합니다.
    SVR_RAID_SEASON_DATA_ALREADY_INIT               = 112907,      // 레이드 시즌 초기화가 이미 적용되었습니다.
    SVR_RAID_NOT_MATCH_CONDITION_STAGE_ENDFAIL      = 112908,      // 레이드 종료는 시간제한과 hp가 0일 경우에 가능합니다. 
    SVR_RAID_INVALID_HP                             = 112909,      // 레이드에 잘못된 hp 값이 들어왔습니다. 
    SVR_RAID_INVALID_TYPE_VALUE                     = 112910,      // 현재 오픈된 TypeValue 값이 아닙니다.     


    _END_USER_ = 200000 - 1,


    __SERVER_END__,


    __COMMON_START__                                = 200000,
    COM_ERR_NOT_WORK_REALIZE_EMBODY                 = 200000,       // 아직 구현이 안된 기능입니다.
    COM_ERR_NOW_WORK_PREPARE                        = 200001,       // 현재 준비중인 기능입니다.
    COM_ERR_FAIL                                    = 200002,       // 에러입니다.
    COM_ERR_INVALID_TABLE_ID                        = 200003,       // 무효한 테이블 ID입니다.
    COM_ERR_NOT_EXIST_TABLE_DATA                    = 200004,       // 테이블 데이타가 존재하지 않습니다.
    COM_ERR_INVALID_SCOPE_INDEX                     = 200005,       // 무효한 범위의 인덱스입니다.
    COM_ERR_NOT_FIND_USER                           = 200006,       // 유저를 찾을 수 없습니다.
    COM_ERR_SELL_IMPOSSIBLE_ZERO_COUNT              = 200007,       // 0개의 수량을 판매할 수 없습니다.
    COM_ERR_ALREADY_LOCK_STATE                      = 200008,       // 이미 락 상태입니다.
    COM_ERR_ALREADY_UNLOCK_STATE                    = 200009,       // 이미 언락 상태입니다.
    COM_ERR_ALREADY_MAX_LEVEL                       = 200010,       // 이미 최대 레벨입니다.
    COM_ERR_CAN_NOT_OVER_MAX_LEVEL                  = 200011,       // 최대 레벨를 넘어서는 레벨로 설정할 수 없습니다.
    COM_ERR_CAN_NOT_ZERO_BELOW_LEVEL                = 200012,       // 0 이하로 레벨을 설정할 수 없습니다.
    COM_ERR_LACK_PRODUCT                            = 200013,       // 재화가 부족합니다.
    COM_ERR_LACK_GOLD                               = 200014,       // 골드가 부족합니다.
    COM_ERR_DONT_WORK_TO_LOCKED                     = 200015,       // 잠긴 상태의 데이터가 포함돼 진행할 수 없습니다.
    COM_ERR_ALREADY_MAX_WAKE_VALUE                  = 200016,       // 이미 최대 각성 수치입니다.
    COM_ERR_INVALID_SLOT_INDEX                      = 200017,       // 무효한 슬롯 인덱스 입니다.
    COM_ERR_INVALID_SLOT_DATA                       = 200018,       // 슬롯 데이터를 잘못 보냈습니다.
    COM_ERR_ERR_NOT_SAME_TYPE_MATERIAL              = 200019,       // 다른 타입의 재료가 포함되서 처리를 하지 못했습니다.
    COM_ERR_LV_OTHER_TYPE_MATERIAL                  = 200020,       // 레벨업 재료에 사용할 수 없는 타입의 재료가 있습니다.
    COM_ERR_LV_LOCKED_MATERIAL                      = 200021,       // 레벨업 재료에 락 상태의 재료가 있습니다.
    COM_ERR_LV_USING_CHARACTER_MATERIAL             = 200022,       // 레벨업 재료에 캐릭터가 사용중인 재료가 있습니다.
    COM_ERR_LV_NOT_EXIST_TABLE_DATA_MATERIAL        = 200023,       // 레벨업 재료에 테이블 데이타가 존재하지 않는 재료가 있습니다.
    COM_ERR_ALREADY_MAX_GRADE                       = 200024,       // 이미 최대 등급입니다.
    COM_ERR_GRD_ERR_NOT_EXIST_ITEMREQLIST           = 200025,       // 등급업 필요한 재료 아이템 테이블 정보가 존재하지 않습니다.
    COM_ERR_LACK_POINT                              = 200026,       // 포인트가 부족합니다.
    COM_ERR_LACK_LEVEL                              = 200027,       // 레벨이 부족합니다.
    COM_ERR_INVALID_GOODS_TYPE                      = 200028,       // 알 수 없는 재화 타입입니다.
    COM_ERR_EIXST_TARGET_IN_MATERIAL                = 200029,       // 레벨업 대상이 재료에 포함되어 있어 진행할 수 없습니다.
    COM_ERR_INVALID_CONTENTS_KIND                   = 200030,       // 무효한 컨텐츠 분류(eContentsPosKind) 입니다.
    COM_ERR_ZERO_REQUEST_COUNT                      = 200031,       // 요청한 수량이 0이라 처리할 수 없습니다.
    COM_ERR_OVER_REQUEST_COUNT                      = 200032,       // 요청한 수량이 한번에 처리할 수 있는 개수를 초과해서 처리할 수 없습니다.
    COM_ERR_DUPLICATE_MATERIAL                      = 200033,       // 재료에 동일한 항목이 존재합니다.
    COM_ERR_DUPLICATE_IN_LIST                       = 200034,       // 목록에 동일한 항목이 존재합니다.
    COM_ERR_PLEASE_RECONTINUE                       = 200035,       // 에러로 인해서 재접속이 필요합니다.
    COM_ERR_NOT_APPLY_SAME_VALUE                    = 200036,       // 현재 설정된 값과 같은 값으로 설정할 수 없습니다.
    COM_ERR_INVALID_REWARD_TYPE                     = 200037,       // 무효한 보상 타입입니다.
    COM_ERR_CANT_ZERO_GAP_VALUE                     = 200038,       // 변동 값이 0인 값으로는 처리할 수 없는 작업입니다.
    COM_ERR_FORCE_ALL_USER_KICK                     = 200039,       // 서버에서 모든 유저를 강제로 내보냈습니다.
    COM_ERR_NOT_EXIST_ITEMREQLIST_NEED_WAKE         = 200040,       // 각성에 필요한 재료 아이템 테이블 정보가 존재하지 않습니다.
    COM_ERR_NOT_EXIST_MATERIAL_INST                 = 200041,       // 재료 중 정보가 존재하지 않는 항목이 있어서 처리하지 못했습니다.
    COM_ERR_TO_MATERIAL_IN_WPN_SLOT                 = 200042,       // 무기에 장착중인 곡옥이 재료에 포함되서 처리하지 못했습니다.
    COM_ERR_NOT_MAX_LV                              = 200043,       // 최대 레벨이 아니라서 처리하지 못했습니다.
    COM_ERR_NOT_NEED_MATERIAL_SETTING               = 200044,       // 필요 없는 재료가 설정되서 처리하지 못했습니다.
    COM_ERR_EMPTY_NEED_MATERIAL                     = 200045,       // 필요한 재료가 없어서 처리하지 못했습니다.
    COM_ERR_NOT_APPLY_SAME_INFO                     = 200046,       // 적용하려는 정보가 적용된 정보와 같아서 처리하지 못했습니다.
    COM_ERR_LACK_ITEM_SLOT                          = 200047,       // 인벤토리 공간이 부족하여 아이템을 추가할 수 없습니다.
    COM_ERR_ALREADY_MAX_SLOT                        = 200048,       // 슬롯 공간이 이미 최대입니다.
    COM_ERR_OVER_SIZE_STRING                        = 200049,       // 입력한 문자열의 내용이 너무 길에서 처리할 수 없습니다.
    COM_ERR_EMPTY_INPUT_SIZE                        = 200050,       // 입력한 내용이 하나도 없어서 처리할 수 없습니다.
    COM_ERR_LACK_INPUT_SIZE                         = 200051,       // 입력한 내용이 너무 짧아서 처리할 수 없습니다.
    COM_ERR_OVER_INPUT_SIZE                         = 200052,       // 입력한 내용이 너무 길어서 처리할 수 없습니다.
    COM_ERR_SAME_INPUT_TO_TARGET_INFO               = 200053,       // 입력한 내용이 같은 내용이라 처리할 수 없습니다.
    COM_ERR_IMPOSSIBLE_INPUT_INFO                   = 200054,       // 입력한 내용에 입력이 불가능한 내용 포함되 처리할 수 없습니다.
    COM_ERR_NOT_ADD                                 = 200055,       // 더 이상 추가할 수 없습니다.
    COM_ERR_FORCE_USER_KICK                         = 200056,       // 운영 정책에 의해서 강제로 내보내졌습니다.
    COM_ERR_NOT_EXIST_SEL_ITEM_DATA_IN_TABLE        = 200057,       // 선택한 아이템이 테이블에 존재하지 않습니다.
    COM_ERR_POS_ERR_APPLY_WHERE                     = 200058,       // 어딘가에서 위치 적용중인 상태라 처리할 수 없습니다.
    COM_ERR_POS_ERR_APPLY_CHARACTER                 = 200059,       // 캐릭터에서 위치 적용중인 상태라 처리할 수 없습니다.
    COM_ERR_POS_ERR_APPLY_FACILITY                  = 200060,       // 비밀기지에서 위치 적용중인 상태라 처리할 수 없습니다.
    COM_ERR_POS_ERR_APPLY_ARENA                     = 200061,       // 아레나에서 위치 적용중인 상태라 처리할 수 없습니다.
    COM_ERR_IS_ONLY_ADMIN_FUNCTION                  = 200062,       // 관리자 계정만 이용 가능한 기능이라 처리할 수 없습니다.
    COM_ERR_ALREADY_ALL_TAKE_REWARD                 = 200063,       // 이미 보상을 모두 획득해서 처리할 수 없습니다.
    COM_ERR_NOT_FIND_CONDITION_ABOUT_TARGET         = 200064,       // 조건에 맞는 대상이 없어서 처리하지 못했습니다.
    COM_ERR_NOT_FIND_CONDITION_ABOUT_ITEM           = 200065,       // 조건에 맞는 항목이 없어서 처리하지 못했습니다.
    COM_ERR_ERR_EMPTY_COUNTRY_CODE                  = 200066,       // 국가 코드 값이 없어서 처리하지 못했습니다.
    COM_ERR_ERR_EMPTY_LANGUAGE_CODE                 = 200067,       // 언어 코드 값이 없어서 처리하지 못했습니다.
    COM_ERR_ERR_OVER_SIZE_COUNTRY_CODE              = 200068,       // 국가 코드 값이 너무 길어서 처리하지 못했습니다.
    COM_ERR_ERR_OVER_SIZE_LANGUAGE_CODE             = 200069,       // 언어 코드 값이 너무 길어서 처리하지 못했습니다.
    COM_ERR_ERR_BE_WRONG_COUNT                      = 200070,       // 사용 수량이 맞지 않아서 처리 하지 못했습니다.
    COM_ERR_ERR_CANT_WORK                           = 200071,       // 처리할 수 없는 작업입니다.
    COM_ERR_ERR_OVER_VALUE_CANT_WORK                = 200072,       // 요청된 값이 처리할 수 있는 범위를 넘어서 처리하지 못했습니다.
    COM_ERR_TRY_USED_NOT_ACTIVE_SKILLSLOT_BUFF      = 200073,       // 활성화 되지 않은 스킬 확장 버프 사용을 시도해서 처리하지 못했습니다.
    COM_ERR_POS_ERR_APPLY_DISPATCH                  = 200074,       // 파견에 위치 적용중인 상태라 처리할 수 없습니다.
    COM_ERR_NOT_EXIST_RESET_DATA                    = 200075,       // 초기화할 내용이 없습니다.
    COM_ERR_NOW_NOT_AWAKE_STATE                     = 200076,       // 현재 각성 상태가 아니라서 처리하지 못했습니다.
    COM_ERR_NOT_SELECT_INFLU_TARGET                 = 200077,       // 세력을 선택하지 않아서 처리하지 못했습니다.
    COM_ERR_NOT_ITEM_TYPE_WANT_RWD                  = 200078,       // 아이템 보상이 아닌데 아이템으로 획득을 원해서 처리하지 못했습니다.
    COM_ERR_NOT_ACHIEVE_CONDI_TAKE_RWD              = 200079,       // 보상 조건에 만족하지 안아서 처리하지 못했습니다.
    COM_ERR_NOT_RETENTION_PERIOD_LEFT               = 200080,       // 유지 기간이 남아서 처리하지 못했습니다.
    COM_ERR_NOT_USE_TYPE_MATERIAL                   = 200081,       // 재료에 사용할 수 없는 타입의 재료가 있어서 처리하지 못했습니다.
    COM_ERR_NOT_SAME_GRADE_IN_MATERIAL              = 200082,       // 재료에 다른 등급이 포함돼서 처리하지 못했습니다.
    COM_ERR_CAN_NOT_USE_MATERIAL_TARGET             = 200083,       // 재료에 사용할 수 없는 대상이 포함돼서 처리하지 못했습니다.
    COM_ERR_CAN_NOT_DECOMPOSABLE_TARGET             = 200084,       // 분해 할 수 없는 대상입니다.
    COM_ERR_CAN_NOT_CONSUME_GOODS                   = 200085,       // 재화를 소모할 수 없는 상황입니다.
    COM_ERR_SETTING_VALUE                           = 200086,       // 설정값이 올바르지 않습니다.
    COM_ERR_SETTING_COUNT                           = 200087,       // 설정하려는 수량이 올바르지 않습니다.
    COM_ERR_INVALID_INFO_DATA_IN_TABLE              = 200088,       // 올바르지 않은 정보가 테이블 데이터 안에 있어 처리하지 못했습니다.
    COM_ERR_INVALID_TYPE                            = 200089,       // 무효한 타입이라 처리하지 못했습니다.
    COM_ERR_INVALID_SET_BELOW_GRADE                 = 200090,       // 더 낮거나 같은 등급으로 설정할 수 없습니다.
    COM_ERR_NOT_TARGET_DATA_IN_LIST                 = 200091,       // 처리할 대상이 아닌 정보가 포함돼서 처리하지 못했습니다.
    COM_ERR_NOT_ZERO_VALUE_WORK                     = 200092,       // 0 값으로 처리할 수 없는 작업입니다.
    COM_ERR_IS_NOT_MAX_WAKE                         = 200093,       // 최대 각성 상태가 아니므로 처리할 수 없습니다.
    COM_ERR_CAN_NOT_EVOLUTION_ITEM                  = 200094,		// 해당 아이템은 진화 할 수 없습니다.
    COM_ERR_NOT_EXIST_ITEMREQLIST_NEED              = 200095,       // 요청에 필요한 재료 아이템 테이블 정보가 존재하지 않습니다.
    COM_ERR_CAN_NOT_GRADE_ITEM                      = 200096,		// 해당 요청을 처리할 수 없는 아이템 등급 입니다.
    COM_ERR_ALREADY_MAX_STATE                       = 200097,		// 이미 최대 상태라서 처리하지 못했습니다.


	// HTTP 관련 공용
	COM_HTTP_ERR_WORK_HTTP                          = 210000,       // HTTP 처리중 오류 있어서 처리하지 못했습니다.
    COM_HTTP_ERR_UNKNOWN_HTTP                       = 210001,       // HTTP 처리중 알 수 없는 오류가 있어서 처리하지 못햇습니다.
    COM_HTTP_ERR_NOT_OPENED                         = 210002,       // HTTP 통신이 아직 열리지 않않아서 처리하지 못했습니다.
    COM_HTTP_ERR_ALREADY_OPENED                     = 210003,       // HTTP 통신이 이미 열린 상태라 처리하지 못했습니다.
    COM_HTTP_ERR_ALREADY_SAME_KEY_OBJ               = 210004,       // HTTP에 이미 같은 키의 오브젝트가 있어서  처리하지 못했습니다.
    COM_HTTP_ERR_READ_BUFF_SIZE_LACK                = 210005,       // HTTP 처리중 버퍼 크기가 부족해 처리하지 못했습니다.
    COM_HTTP_ERR_INVALID_HANDLE                     = 210006,       // HTTP 처리중 무효한 핸들로 인해 처리하지 못했습니다.
    COM_HTTP_ERR_HTTP_STATUS_ERR                    = 210007,       // HTTP 상태가 올바르지 않아 처리하지 못했습니다.
    COM_HTTP_ERR_HTTP_SERVER_ERR                    = 210008,       // HTTP 처리 서버에서 에러가 발생해 처리하지 못했습니다.
    COM_HTTP_ERR_HTTP_REQUEST_ERR                   = 210009,       // HTTP 요청중 에러가 발생해 처리하지 못했습니다.


    __COMMON_END__                                  = 300000 - 1,


    _MAX_,
};

//[System.Flags]
public enum eContentType
{
    NONE = 0,    
    
    SERVER_RELOCATE = 1 << 0,
    STORE_EVENT = 1 << 1,    

    MAX = int.MaxValue
}

enum eRWD_TP
{
    _NONE_,

    SINGLE,
    ALL,
    RANK,

    _MAX_
};

public enum eFacilityTradeType
{
    NONE =0,

    CARD,
    WEAPON,

    _MAX_
}


public enum eEventTarget
{
    NONE = 0,

    WELCOME = 1,
    COMEBACK,
    PUBLIC,

    MAX,
}

public enum ePackageSlotItemNameType
{
    Common = 0,
    Premium,
}

public enum eFacilityFunctionSelectPopupButtonType
{
    On = 0,
    Off,
}

public enum ePassSystemType
{
    Gold = 1,
    Rank,
    Story,
}

public enum eLobbyEventType
{
    Bingo = 1,
    Achieve = 2,
}

public enum eAchieveEventType
{
    Ing = 1,
    Reward,
    Complete,
}

public enum eCircleLobbyPanelType
{
    Main = 0,
    Setup,
    MarkChange,
}

public enum eCircleSequenceType
{
    Info = 0,           // 이름
    Content,            // 소개문
    MainLang,           // 주 언어
    OtherLang,          // 주 언어 이외
    BuyStamp,           // 스탬프 구매
    ChatAlramSet,       // 채팅 알림 세팅
    ViceManagement,     // 부부장 관리
    MemberManagement,   // 부원 관리
    Expulsion,          // 추방
    Dissolution,        // 해산
    Withdrawal,         // 탈퇴
}

public enum eCircleInfoSlotType
{
    Recommend = 0,
    Join,
}

public enum eCircleUserSlotType
{
    Member = 0,
    JoinWait,
    Other,
}

public enum eCircleChatAlramType
{
    All = 0,
    Arena,
    Gacha,
    TimeAttack,
    WeaponEnhance,
}

public enum eToggleType
{
    On = 0,
    Off = 1,
    ServerTypeOn = 1,
    ServerTypeOff = 0,
}

public enum eArenaRewardListPopupType
{
    ArenaReward = 0,
    CircleAttendance,
}

public enum eCircleInfoTabType
{
    Info = 0,
    Member,
    Facility,
    Buff,
}

public enum eRaidStoreHelpType {
    NONE = -1,
	ALWAYS = 0,
	DAILY,
	WEEKLY,
	MONTHLY,
}

public enum ePackageLimitType {
	NONE = 0,
	D,
	W,
	M,
	MAX,
}