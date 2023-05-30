using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eTEXTID
{
    OK = 1,
    CANCEL,
    YES,
    NO,
    BUY,
    CHANGE,
    APPLY,
    CLOSE,
    SELL,
    BACK,
    GET,
    BACK2,
    CHANGE2,
    RELESE,
    USE,
    SELETE,
    SERVERERROR,
    TITLE_NOTICE = 50,
    TITLE_BUY,
    TITLE_SELL,
    GOODSTYPE_LACK_START = 100,
    RANK_TXT_NOW_LV = 210,
    LEVEL_TXT_NOW_LV,
    LEVEL_TXT_NOW_AND_MAX_LV,
    STACK_COUNT_TXT,
    SLASH_TXT_MAX_CNT,
    SKILL_LEVEL_TXT_NOW_LV,
    NUMBER_TXT_NOW_NUM,
    LEVEL_TXT_NOW_LV_MAX = 219,
    SPACE_TYPE_TEXT = 220,  // 국가별 띄어쓰기 여부 관련 스트링 ({0}{1}, {0} {1})
    WHITE_TEXT_COLOR = 222,
    GRAY_TEXT_COLOR,
    GREEN_TEXT_COLOR,
    RED_TEXT_COLOR,
    LEVEL_NOW_TEXT_COLOR,
    LEVEL_MAX_TEXT_COLOR,

    GOODSTYPE_TEXT_START = 240,

    CURRENCYCODE = 250,//통화기호코드
    GOODSTEXT = 270,
    GOODSTEXT_W,
    GOODSTEXT_R,
    PERCENT_TEXT = 273,
    PERCENT_ONETWO_POINT_TEXT,
    PERCENT_ONE_POINT_TEXT,
    PLUS_TEXT = 282,
    TIME_D = 300,
    TIME_M,
    ACCOUNTTYPE = 310,
    GOOGLEPLAY = 310,
    GAMECENTER,
    TWITTER,
    FACEBOOK,
    YAHOO,
    LINE,
    STEAM,
    APPLE,
    STAT_TEXT_ID = 320,
    STAT_HP,
    STAT_ATK,
    STAT_DEF,
    STAT_CRI,
    STAT_LV,
    STAT_SLV,
    STAT_LUCK,
    REWARDTYPE = 400,
    LOCK = 410,
    LOCKMSG = 420,
    CARDTYPE = 450,
    MON_TYPE_TEXT_START = 10000,
    MAIL_TYPE_TEXT_START = 10100,
    WEEKLY_MISSION_TEXT_START = 10200,
    BLUE_TEXT_COLOR = 1494,
    YELLOW_TEXT_COLOR = 1495,
    PERCENT_ONETHREE_POINT_TEXT = 1496,

    REWARD_POPUP_TITLE = 1262,
    REWARD_POPUP_TEXT = 1263,       // 즉시 지급
    REWARD_POPUP_TEXT_MAIL = 1500,  // 우편 발송
}

public enum eCOUNT
{
    NONE = 0,
    WEAPONGEMSLOT = 4,
    GEMRANDOPT = 5,
    CARDSLOT = 3,
    SKILLSLOT = 4,
    WEAPONSLOT = 2,
    SKILLULTIMATEINDEX = 3,
    BADGEOPTCOUNT = 3,  //
    ROOMCOUNT = 3,
    STAGEMISSION = 3,
    STAGEREWARD_N = 0,
    STAGEREWARD_LUCK = 1,
    LOGINBONUSMAX = 7,
    WEEKMISSIONCOUNT = 7,
    ONE_MINUTE = 60,
    MAX_DIFFICULTY = 3,
    MAX_BO_FUNC_VALUE = 100,
    MAX_RATE_VALUE = 1000,
    MAX_PROBABILITY = 10000,
    IS_ARENA_ENEMY_UUID_FLAG = 10000,
    FAVOR_ITEM_VERY_GOOD = 3,
    FAVOR_ITEM_GOOD = 2,
    FAVOR_ITEM_NORMAL = 1,
    BADGESLOT = 3,
}

public enum eAxis
{
    X = 0,
    Y,
    Z
}

public enum ePlayerCharType
{
    None = 0,

    Asagi,
    Sakura,
    Yukikaze,
    Rinko,
    Murasaki,
    Jinglei,
    Shiranui,
    Emily,
    Kurenai,
    Oboro,
    Asuka,
    Kirara,
    Ingrid,
    Noah,
    Astaroth,
    Tokiko,
    Shizuru,
	Felicia,
	Maika,
	Rin,
    Sora,
    Annerose,
    Nagi,
    Aina,
    Saika,
    Shisui,

    Num,
}

public enum eGRADE
{
    GRADE_NONE = 0,
    GRADE_N,
    GRADE_R,
    GRADE_SR,
    GRADE_UR,
    COUNT,
}

public enum eCARDTYPE
{
    NONE = 0,
    ASSIST,
    PROTECT,
    DOMINATE,
    COUNT,
    ALL = 99,
}

public enum eCARDSLOT
{
    SLOT_MAIN = 0,
    SLOT_SUB1,
    SLOT_SUB2,
    _MAX_,
}

public enum eWeaponSlot
{
    MAIN = 0,
    SUB = 1,

    Count,
}

public enum eFACILITYSTATS
{
    WAIT = 0,
    USE,
    COMPLETE,
}

public enum eCHARABILITY
{
    HP = 0,
    ATK,
    DEF,
    CRI,

    _MAX_,
}

public enum eCHARSKILLCONDITION
{
    NONE = 0,               // 조건 없음
    ASSIST_CARD_CNT,        // 지원 서포터 배치 n명
    PROTECT_CARD_CNT,       // 보호 서포터 배치 n명
    DOMINATE_CARD_CNT,      // 제압 서포터 배치 n명
    MAIN_ASSIST_CARD,       // 메인으로 지원 서포터 
    MAIN_PROTECT_CARD,      // 메인으로 보호 서포터
    MAIN_DOMINATE_CARD,     // 메인으로 제압 서포터
    ANY_CARD_CNT,           // 아무 서포터 배치 n명
    _MAX_,
}

public enum eUIMOVEFUNCTIONTYPE
{
    NONE = 0,
    UIPANEL,
    UIPOPUP,
    WEBVIEW
}

public enum eBuffDebuffType
{
    None = 0,
    Buff,
    Debuff,
}

public enum eBuffIconType
{
	Debuff_Temp6 = -21,
	Debuff_Temp5 = -20,
	Debuff_Temp4 = -19, // 경화
	Debuff_Temp3 = -18,
    Debuff_Freeze = -17,
    Debuff_Confusion = -16,
    Debuff_DebuffTimePromote = -15,
    Debuff_CriDmg = -14,
    Debuff_WeaponSkill = -13,
    Debuff_SupporterSkill = -12,
    Debuff_Recovery = -11,
    Debuff_Nomove = -10,
    Debuff_Noattack = -9,
    Debuff_HPMinus = -8,
    Debuff_Flame = -7,
    Debuff_Def = -6,
    Debuff_Cri = -5,
    Debuff_Bleeding = -4,
    Debuff_Speed = -3,
    Debuff_Atk = -2,
    Debuff_Addiction = -1,

    None = 0,

    Buff_Atk = 1,
    Buff_Speed = 2,
    Buff_Cri = 3,
    Buff_Def = 4,
    Buff_HPPlus = 5,
    Buff_Recovery = 6,
    Buff_SupporterSkill = 7,
    Buff_WeaponSkill = 8,
    Buff_Superarmor = 9,
    Buff_Invin = 10,
    Buff_CriDmg = 11,
    Buff_DebuffTimeReduce = 12,
    Buff_Temp1 = 13,
    Buff_Temp2 = 14,
    Buff_Temp3 = 15,
}

public enum eStageClearCondition
{
    NONE = 0,
    STAGE_CLEAR,
    MAX_COMBO_COUNT,
    REMAIN_HP_PERCENTAGE,
    CLEAR_TIME,
    HIT_COUNT,
    NPC_REMAIN_HP_PERCENTAGE,
    NPC_HIT_COUNT,
}

public enum EAttackAttr
{
    NONE        = 0,
    NORMAL      = 1,
    FIRE        = 2,
    ICE         = 3,
    ELECTRIC    = 4,
    POISON      = 5,
    BLEEDING    = 6,
	Freeze		= 7,
}

[System.FlagsAttribute]
public enum eDebuffImmuneType_f {
    NONE                = 0,
    ATTACK_RATE_DOWN    = 1 << 1,
    DMG_RATE_UP         = 1 << 2,
    SPEED_DOWN          = 1 << 3,
    CRITICAL_RATE_DOWN  = 1 << 4,
    CRITICAL_DMG_DOWN   = 1 << 5,
}

public enum eSD_PanelType
{
    GACHA = 0,
    STORE,
    CASH,
    PACKAGE,
    ARENASTORE,
    RAID_STORE,
}

public enum eEventState
{
    EventEnd = -2,      // 이벤트 종료
    EventNotStart = -1, // 이벤트 시작 안함
    EventNone = 0,
    EventOnlyReward,    // 이벤트 보상 획득까지만 가능
    EventPlaying,       // 이벤트 플레이 가능
}

public enum eGuerrillaCampaignType
{
    None = 0,
    GC_StageClear_ExpRateUP,
    GC_StageClear_GoldRateUP,
    GC_StageClear_ItemCntUP,
    GC_StageClear_APRateDown,
    GC_StageClear_FavorRateUP,
    GC_Upgrade_ExpRateUP,
    GC_Upgrade_PriceRateDown,
    GC_Upgrade_SucNorRateDown,
    GC_ItemSell_PriceRateUp,
    GC_Arena_CoinRateUP,
    GC_Rotation_OpenCashSale,
    GC_Stage_Multiple,
    GC_WeeklyMissionSet_Assign,
	GC_Gacha_DPUP,                      // 염원의 기운 획득량 증가
	GC_RAID_HPRESTORE,
}

public enum eTutorial
{
    State = 0,
    Step,
    Temp1,
    Temp2,
    Count
}

public enum eTutorialState
{
    TUTORIAL_STATE_Init = 0,
    TUTORIAL_STATE_CardEquip,   
    TUTORIAL_STATE_CardLevelUp, //캐릭터 서포터 장착창으로 연결
    TUTORIAL_STATE_Stage2Join,  //메인부터 시작
    TUTORIAL_STATE_Stage2Clear,
    TUTORIAL_STATE_SkillSpt,    //메인부터 시작
    TUTORIAL_STATE_SkillEquip,  //캐릭터 서포터 장착창, 스킬선택 팝업 연결 0번슬롯 선택
    TUTORIAL_STATE_Stage3Join,  //메인부터 시작
    TUTORIAL_STATE_Stage3Clear,
    TUTORIAL_STATE_Gacha,       //메인부터 시작
    TUTORIAL_STATE_Mail,        //메인부터 시작
    TUTORIAL_STATE_EndTutorial,
    TUTORIAL_FLAG = 1000
}

public enum eTutorialFlag
{
    DAILY = 0,
    EVENT,
    SPECIAL,
    TIMEATTACK,
    FAC_CHAR_SP,
    FAC_CHAR_EXP,
    FAC_ITEM_COMBINE,
    FAC_WEAPON_EXP,
    PRIVATEROOM,
    WEAPON,

    
}

public enum eCharSelectFlag
{
    FACILITY = 0,
    STAGE,
    USER_INFO,
    ARENA,
    ARENATOWER,
    ARENATOWER_STAGE,
	USE_CHAR_GRADE_UP_ITEM,
    SECRET_QUEST,
    Preset,
    RAID,
    RAID_PROLOGUE,
    FAVOR_BUFF_CHAR,
}

public enum eArenaGradeFlag
{
    GRADE,
    GRADE_ID,
    NONE,
}

public enum eBadgeSlot
{
    NONE = 0,
    FIRST,
    SECOND,
    THIRD,
    _MAX_,
}

public enum eBadgeOptSlot
{
    FIRST = 0,
    SECOND,
    THIRD,
    _MAX_,
}

public enum eArenaTeamSlotPos
{
    START_POS = 0,
    MID_POS = 1,
    LAST_POS = 2,   
    _MAX_,
    GUARDIAN = 3,
}

public enum eArenaGradeUpFlag
{
    NONE = -1,
    LOSE = 0,
    WIN = 1,
}

public enum eArenaToCharInfoFlag
{
    NONE = 0,
    ARENA_MAIN,
    ARENA_ENEMY_SEARCH,
    FRIEND_BATTLE,
    ARENATOWER,
    ARENATOWER_STAGE,
}

public enum eRankUserType
{
    NONE,
    TIMEATTACK,
    ARENA,
    ARENA_ENEMY,
    ARENATOWER_FRIEND,
    RAID,
}

public enum eArenaRewardType
{
    NONE,
    RANK,
    GRADE,
}

public enum eArenaState
{
    NONE,
    PLAYING,        //시즌 플레이 가능
    COMBINE,        //집계중
    REWARD,         //보상 받기
}

public enum eMainNoticeType     //로비 하단 알림창 타입
{
    BASE = 0,
    BUFF,
    FACILITY,
}

public enum eMonthlyType
{
    NONE = 0,       
    NORMAL,             //일반 월정액 상품
    PREMIUM             //프리미엄 월정액 상품
}

public enum eBuffEffectType
{
    NONE = 0,
    Buff_SkillSlot,         //추가 스킬슬롯
    Buff_MaxAP,             //추가 AP
    Buff_BonusDrop,         //보너스 드랍
    Buff_CharExpAll,        //캐릭터 경험치
}

public enum eFireBaseLogType
{
    NONE = 0,

    _1_1_Result,
    _1_2_Result,
    _1_3_Result,
    _1_4_Result,
    _1_5_Result,

    _2_1_Result,
    _2_2_Result,
    _2_3_Result,
    _2_4_Result,
    _2_5_Result,

    _3_1_Result,
    _3_2_Result,
    _3_3_Result,
    _3_4_Result,
    _3_5_Result = 15,
    _STAGE_END_ = 15,
    _RUN,                       //첫 실행
    _Agreement,                 //약관 동의
    _DownloadBundle,            //번들 다운로드
    _Login,                     //신규 생성 로그인
    _Prev_Device,               //계정 이어하기
    _Prev_Server,               //서버 이전
    _1_1_Production,            //1-1연출
    _Char_Selection,            //캐릭터 선택창 열렸을때
    _Char_Selection_Asagi,           //캐릭터 선택
    _Char_Selection_Sakura,           //캐릭터 선택
    _Char_Selection_Yukikaze,           //캐릭터 선택
    _Char_Selection_Complete,   //캐릭터 선택 완료
    _1_1_Production_Complete,   //1-1연출 끝
    _1_1_Start,
    _1_1_Tutorial_Skip,
    _1_1_Clear,
    

    _1_1_Tutorial_Card_Equip,
    _1_1_Tutorial_Card_Equip_Skip,

    _1_1_Tutorial_Card_LevelUp,
    _1_1_Tutorial_Card_LevelUp_Skip,

    _1_1_Tutorial_1_2_Join,
    _1_1_Tutorial_1_2_Join_Skip,

    _1_2_Start,
    _1_2_Tutorial_Skip,
    _1_2_Clear,
    

    _1_2_Tutorial_Char_Skill_Leam,
    _1_2_Tutorial_Char_Skill_Leam_Skip,

    _1_2_Tutorial_Char_Skill_Equip,
    _1_2_Tutorial_Char_Skill_Equip_Skip,

    _1_2_Tutorial_1_3_Join,
    _1_2_Tutorial_1_3_Join_Skip,

    _1_3_Start,
    _1_3_Tutorial_Skip,
    _1_3_Clear,
    

    _1_3_Tutorial_Gacha,
    _1_3_Tutorial_Gacha_Skip,

    _1_3_Tutorial_Mail,
    _1_3_Tutorial_Mail_Skip,

    _STOP_,
}

public enum eEventRuleTexName
{
    None,
    CardTex,
    StageBgTex,
    EventBgTex,
    EventLogoTex,
    End,
}

public enum eEventRuleTexType
{
    None,
    MainUrl,
    SubUrl,
    AssetBundle,
    End,
}

public enum eGachaTabType
{
    None,
    PICKUP1,
    PICKUP2,
    PICKUP3,
    PICKUP4,
    ROTATION,
    CASH,
    GOLD,
}

public enum eBannerFuncType
{
    None,
    PACKAGEPOPUP,
    CHAR,
    LIMIT,
    GACHA,
    STORE,
    CASH,
    GOLD,
    EVENT_STORY_MAIN,
    EVENT_CHANGE_MAIN,
    PASS,
}

public enum eBannerLocalizeType
{
    Url = 0,
    AddUrl1,
    AddUrl2,
    End,
}

[System.Flags]
public enum eBannerFuntionValue3Flag
{
    None = 0,
    Limit = 1 << 1, // 기간 한정
    Retro = 1 << 2, // 복각
    Event = 1 << 3, // 이벤트
    Sale  = 1 << 4, // 할인
}

public enum eGachaLocalizeType
{
    Banner = 0,
    Background,
    AddImage,
    End,
}

[System.Flags]
public enum eGachaPickupType
{
    None = 0,
    Limit = 1 << 1, // 기간 한정
    Retro = 1 << 2, // 복각
    Event = 1 << 3, // 이벤트
    Sale  = 1 << 4, // 할인
}

public enum eGachaRotationType
{
    None = 0,
    Weapon,
    Card,
}

public enum eFacilityEffect
{
    Appear = 0,
    ActivateLoop,
}

public enum eLoginBonusStep
{
    Step01,
    Step02,
    Step03,
    Step04,
    Step05,
	Step06,
	End,
}

public enum eHideWeaponCharType {
    NONE,
    SAIKA,
}