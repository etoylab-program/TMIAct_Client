using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameConfig : ScriptableObject
{
    [Header("Test")]
    [SerializeField] private bool _TestMode = true;                          //*클라만 사용
    [SerializeField] private int _TestLevel = 1;                             //*클라만 사용 테스트 모드인경우에만
    [SerializeField] private int _TestCharLevel = 1;                         //*클라만 사용 테스트 모드인경우에만
    [SerializeField] private int _TestInitCharTID = 1;                       //*클라만 사용, 초기 생성 캐릭터 TID
    public bool TestSkipDirector = false;

    [Header("Init")]
    [SerializeField] private int _InitGold = 10000;                             //초기 지급 골드
    [SerializeField] private int _InitCash = 50;                                //초기 지급 캐쉬
    [SerializeField] private int _InitSupporterPoint = 0;                       //초기 지급 서포터포인트    *추가   _InitCardCnt, _InitItemCnt 제거됨
    [SerializeField] private int _InitRoomPoint = 0;                            //초기 지급 룸포인트        *추가
    [SerializeField] private int _InitBattleCoin = 0;                           //초기 지급 배틀 코인
    [SerializeField] private int _InitFriendPoint = 0;                          //초기 지급 우정 포인트
    [SerializeField] private int _InitCharPassviePoint = 0;                     //캐릭터 생성시 스킬 포인트 *추가 
    [SerializeField] private int _InitRoomID = 1;                               //초기 지급 룸테마 ID
    [SerializeField] private int _InitUserMarkID = 1;                           //초기 지급 유저 마크 ID
    [SerializeField] private int _InitLoginBonusGroupID = 1;                    //초기 로그인 보너스 그룹 ID
    [SerializeField] private int _InitItemMailTypeID = 4;                       //초기 지급 아이템 발송 시 우편 타입 ID
    [SerializeField] private int _InitSpecialModeID = 20001;                    //초기 특별 모드 스테이지 ID
    [SerializeField] private int _InitTutorialItemGroupID = 11000;              //초기 지급 튜토리얼 보상 아이템 ID
    [SerializeField] private int _InitLobbyThemeID = 1;                         //초기 설정 로비테마 ID
    [SerializeField] private string _InitAccountNickName;                       //초기 지휘관 이름
    [SerializeField] private List<int> _InitGoldList;                           //초기 지급 리스트
    [SerializeField] private List<int> _InitCashList;                           //초기 지급 리스트
    [SerializeField] private List<int> _InitWeaponList;                         //초기 지급 리스트
    [SerializeField] private List<int> _InitGemList;                            //초기 지급 리스트
    [SerializeField] private List<int> _InitCardList;                           //초기 지급 리스트
    [SerializeField] private List<int> _InitItemList;                           //초기 지급 리스트
    [SerializeField] private List<int> _InitItemCountList;                      //초기 지급 리스트
    [SerializeField] private List<int> _InitCostumeList;                        //초기 지급 리스트
    [SerializeField] private List<int> _InitFigureList;                         //초기 지급 리스트
    [SerializeField] private List<int> _InitUserMarkList;                       //초기 지급 리스트


    [Header("Limit")]
    [SerializeField] private int _LimitMaxGold = 999999999;                     //소지 가능 최대 골드
    [SerializeField] private int _LimitMaxCash = 999999999;                     //소지 가능 최대 캐쉬
    [SerializeField] private int _LimitMaxSP = 999999999;                       //소지 가능 최대 서포터 포인트
    [SerializeField] private int _LimitMaxRP = 50000;                           //소지 가능 최대 룸 포인트
    [SerializeField] private int _LimitMaxAP = 9999;                            //소지 가능 최대 티켓
    [SerializeField] private int _LimitMaxBP = 9999;                            //소지 가능 최대 배틀 티켓
    [SerializeField] private int _LimitMaxBattleCoin = 999999999;               //소지 가능 최대 배틀 코인
    [SerializeField] private int _LimitMaxFriendPoint = 9999;                   //소지 가능 최대 우정포인트
    [SerializeField] private int _LimitMaxItemStack = 50000;                    //소지 가능 최대 아이템 수량
    [SerializeField] private int _LimitMaxSkillPT = 50000;                      //소지 가능 최대 스킬 포인트
    [SerializeField] private int _LimitMaxDP = 100;                             //소지 가능 최대 염원의 기운
    [SerializeField] private int _LimitMaxWP = 9999;                            //소지 가능 최대 각성 포인트
    [SerializeField] private int _LimitMaxRaidPoint = 999999999;                //소지 가능 최대 레이드 포인트
    [SerializeField] private int _LimitMaxCirclePoint = 999999999;              //소지 가능 최대 서클 포인트
    [SerializeField] private int _LimitMaxCircleGold = 999999999;               //소지 가능 최대 서클 활동비

    [Header("Account")]
    [SerializeField] private int _AccountMaxLevel = 100;                         //계정 최대 레벨
    [SerializeField] private int _AccountLevelUpGroup = 1;                       //계정 레벨업 그룹 아이디
    [SerializeField] private int _APUpdateTimeSec = 180;                         //티켓 갱신 시간 (단위: 초)
    [SerializeField] private int _BPUpdateTimeSec = 3600;                        //티켓 갱신 시간 (단위: 초)
    [SerializeField] private int _BPMaxCount = 5;
    [SerializeField] private int _BaseItemSlotCount = 200;                       //인벤토리 기본 지급 공간
    [SerializeField] private int _MaxItemSlotCount = 500;                        //인벤토리 최대 공간
    [SerializeField] private int _AddItemSlotCount = 5;                          //인벤토리 확장 시 추가 공간
    [SerializeField] private int _AddItemSlotGold = 5000;                        //인벤토리 확장에 소모되는 골드
    [SerializeField] private int _AddItemSlotCash = 10;                          //인벤토리 확장에 소모되는 캐쉬(대마석)
    [SerializeField] private int _AddItemSlotCashCount = 500;                    //캐쉬(대마석)로 변경되는 인벤토리 크기 기준
    [SerializeField] private int _MatCount = 10;                                 //재료 최대 갯수
    [SerializeField] private int _SellCount = 50;                                //판매 및 분해 최대 갯수
    [SerializeField] private float _RPRatePerTicket = 10.0f;                     //티켓 소모 대비 룸 포인트 획득 비율
    [SerializeField] private int _MaxMailCnt = 20;                               //패킷 한번에 보낼 최대 우편 개수
    [SerializeField] private int _DefaultDelMailDay = 30;                        //우편 기본 보관 일수
    [SerializeField] private int _DefaultMailTypeID = 99;                        //우편 기본 타입 ID
    [SerializeField] private int _WeeklyMailTypeID = 3;                          //주간 미션 보상 우편 타입 ID
    [SerializeField] private int _WeeklyResetDay = 1;                            //주간 미션 초기화 요일 (eDayOfWeek ENUM 참조, 0=일요일/1=월요일)
    [SerializeField] private int _WeeklyResetTime = 0;                           //주간 미션 초기화 시간
    [SerializeField] private int _EvtResetGachaReqCnt = 10;                      //리셋 뽑기를 위한 필요 이벤트 아이템 개수
    [SerializeField] private int _EvtResetGachaMaxNum = 10;                      //리셋 뽑기 최대 횟수
    [SerializeField] private List<int> _MultipleList;
    [SerializeField] private List<int> _MultipleRewardRateList;
    [SerializeField] private int _ReturnUserRule = 30;                           //복귀 유저 기준 (일)
    [SerializeField] private int _AccountInterlockReward = 0;                    //계정 연동 보상
    [SerializeField] private int _RepresentativeCount = 3;                       //대표 캐릭터의 수량
    [SerializeField] private int _NewUserTerm = 5;                               //신규 유저 기간
    [SerializeField] private int _ReturnUserTerm = 5;                            //복귀 유저 기간
    [SerializeField] private int _AbsentCostType = 1;                            //출석 보상판 결석 보상 비용 타입
    [SerializeField] private int _AbsentCostIndex = 8;                           //출석 보상판 결석 보상 비용 인덱스
    [SerializeField] private int _AbsentCostValue = 20;                          //출석 보상판 결석 보상 비용
    [SerializeField] private int _AbsentGracePeriod = 1;                         //출석 보상판 유예기간 (일)
    [SerializeField] private int _LoginBonusMonthlyDayMax = 300;                 //출석판 보상 최대 일자
    [SerializeField] private int _MailRefreshTimeSec = 5;                        //선물함 새로 고침 딜레이 시간 (초)

    [Header("Awake")]
    [SerializeField] private int _AwakeSkillClearCash = 400;                     //각성 스킬 초기화 비용 (캐쉬)
    [SerializeField] private int _AwakeOpenGrade = 7;                            //각성 스킬 시스템 활성화 등급
    [SerializeField] private int _AwakeSkillClearItemID = 20206;                 //각성 스킬 초기화 소모 아이템 ID

    [Header("Char")]
    [SerializeField] private int _CharMaxGrade = 5;                              //캐릭터 최고 등급
    [SerializeField] public int CharStartAwakenGrade = 6;                       // 각성 가능한 최소 등급
    [SerializeField] private List<float> _CharGradeStatRate;                     //캐릭터 등급별 능력치 가중치 *클라전용
    [SerializeField] private List<int> _CharMaxLevel;                            //캐릭터 등급별 최대레벨
    [SerializeField] private int _CharLevelUpGroup = 3;                          //캐릭터 레벨업 그룹 아이디
    [SerializeField] private int _CharFavorMaxLevel = 5;
    [SerializeField] private int _CharFavorLevelUpGroup = 4;
    [SerializeField] private int _CharOpenTermsLevel = 70;                       //캐릭터 Favor 오픈 조건 레벨
    [SerializeField] private int _CharOpenTermsPreference = 5;                   //캐릭터 Favor 오픈 조건 친밀도 레벨
    [SerializeField] private List<int> _CharCardSlotLimitLevel;                  //캐릭터 서포터 슬롯 제한 레벨
    [SerializeField] private List<int> _CharSkillSlotLimitLevel;                 //캐릭터 스킬 슬롯 제한 레벨
    [SerializeField] private List<int> _CharWeaponSlotLimitLevel;                //캐릭터 무기 슬롯 제한 레벨
    [SerializeField] private int _CharLoginMsgAddHour = 24;                      //접속 안한지 n시간 뒤 로컬 푸시 시간
    [SerializeField] private List<int> _CharLoginMsgStringIdx;                   //접속 안한지 n시간 뒤 로컬 푸시 메시지
    [SerializeField] private int _CharOpenAddAP = 5;                             //캐릭터 오픈 시 AP 증가량
    [SerializeField] private int _CharAwakeGetWP = 2;                            //등급 업 시 획득 포인트 (각성) 
    [SerializeField] private int _CharWPStartGrade = 7;                          //획득 가능 등급
    [SerializeField] private int _CharLevelGetWP = 1;                            //레벨업 시 획득 포인트
    [SerializeField] private int _CharWPStartLevel = 76;                         //획득 가능 레벨
    public int MaxLobbyBgCharCount = 3;                        // 로비에 배치될 최대 캐릭터 수
    [SerializeField] private int _RandomDyeingMat = 10086;                       //염색 재료 ID
    [SerializeField] private int _RandomDyeingCost = 5;                          //염색 재료 기본 비용
    [SerializeField] private int _RandomDyeingRockCost = 2;                      //염색 잠금 추가 비용
    [SerializeField] private int _DayPresentCount = 5;                           //일일 선물 가능 횟수 (대마인 별로)
    [SerializeField] private float _PreferenceStepRate1 = 2.5f;                  //매우 좋음 친밀도 배율
    [SerializeField] private float _PreferenceStepRate2 = 1.5f;                  //좋음 친밀도 배율
    [SerializeField] private int _CharPresetSlot = 5;                            //캐릭터 프리셋 슬롯 개수

    [Header("Weapon")]
    [SerializeField] private List<int> _WeaponGradeSlot;                         //등급별 곡옥 슬롯수
    [SerializeField] private List<int> _WeaponGradeSlotWakeInc;                  //등급별 제련당 증가 곡옥 슬롯수
    [SerializeField] private int _WeaponLevelupCostByLevel = 100;                //레벨당 무기 레벨업 비용
    [SerializeField] private float _WeaponExpMatWeigth = 0.5f;                   //레벨업된 무기 재료로 사용시 적용되는 EXP비중
    [SerializeField] private List<int> _WeaponMaxLevel;                          //등급별 무기 최대레벨
    [SerializeField] private List<int> _WeaponWakeIncLevel;                      //재련시 최대레벨 증가량
    [SerializeField] private List<int> _WeaponWakeMax;                           //등급별 각성 최대레벨
    [SerializeField] private List<float> _WeaponWakeStatRate;                    //무기 각성별 능력치 가중치 *클라전용
    [SerializeField] private int _WeaponMaxSkillLevel = 5;                       //카드 최대 스킬레벨
    [SerializeField] private List<float> _WeaponSkillLvStatRate;                 //무기 스킬레벨별 능력치 가중치 *클라전용
    [SerializeField] private List<float> _WeaponSubSlotStatBySLV;                //무기 스킬레벨별 서브 슬롯 카드 능력치 비율 *클라전용
    [SerializeField] private List<int> _WeaponSkillLevelupCostByGrade;           //등급별 스킬 레벨업 금액

    [Header("Gem")]
    [SerializeField] private List<int> _GemLevelupCostByGrade;                   //등급별 무기 레벨업 비용
    [SerializeField] private float _GemExpMatWeigth = 0.5f;                      //레벨업된 무기 재료로 사용시 적용되는 EXP비중
    [SerializeField] private int _GemMaxLevel;                                   //등급별 최대 레벨
    [SerializeField] private int _GemMaxWake;                                    //최대 각성수
    [SerializeField] private List<float> _GemWakeStatRate;                       //곡옥 각성별 능력치 가중치 *클라전용
    [SerializeField] private int _GemSetGrade = 4;                               //세트 효과 적용 등급
    [SerializeField] private int _GemAnalyzeCostType = 7;                        //세트 효과 감정 재료 타입
    [SerializeField] private int _GemAnalyzeCostIndex = 10107;                   //세트 효과 감정 재료 인덱스
    [SerializeField] private int _GemAnalyzeCostValue = 1;                       //세트 효과 감정 재료 수량

    [Header("Card")]
    [SerializeField] private int _CardLevelupCostByLevel = 100;                  //레벨당 카드 레벨업 비용
    [SerializeField] private float _CardExpMatWeigth = 0.5f;                      //레벨업된 카드 재료로 사용시 적용되는 EXP비중
    [SerializeField] private List<int> _CardMaxLevel;                             //등급별 카드 최대레벨
    [SerializeField] private List<int> _CardWakeIncLevel;                         //재련시 최대레벨 증가량
    [SerializeField] private List<int> _CardWakeMax;                              //등급별 각성 최대레벨
    [SerializeField] private List<float> _CardWakeStatRate;                       //카드 각성별 능력치 가중치 *클라전용
    [SerializeField] private int _CardMaxSkillLevel = 5;                          //카드 최대 스킬레벨
    [SerializeField] private List<float> _CardSkillLvStatRate;                    //카드 스킬레벨별 능력치 가중치 *클라전용
    [SerializeField] private List<float> _CardSubSlotStatBySLV;                   //카드 스킬레벨별 서브 슬롯 카드 능력치 비율 *클라전용
    [SerializeField] private List<int> _CardSkillLevelupCostByGrade;              //등급별 스킬 레벨업 금액
    [SerializeField] private int _CardFavorMaxLevel = 5;                          //카드 호감도 최대 레벨
    [SerializeField] private List<int> _CardFavorBonusDropRate;                   //카드 호감도 레벨별 보너스 아이템 드랍율
    [SerializeField] private int _CardChangeGold = 500000;                        //카드 속성 변경 금화 비용
    [SerializeField] private int _CardChangeCash = 200;                           //카드 속성 변경 대마석 비용
    [SerializeField] private int _CardFormationFavoritesCount = 10;               //서포터 진형 즐겨 찾기 등록 최대 개수
    [SerializeField] private int _CardDispatchFastItemID = 10097;                 //서포터 파견 즉시 완료 아이템 ID
    [SerializeField] private int _CardDispatchFastItemValue = 1;                  //서포터 파견 즉시 완료 아이템 수량

    [Header("LevelUpBonus")]
    [SerializeField] private List<int> _LevelUpBonusRate;                           //레벨업 보너스 확률
    [SerializeField] private List<float> _LevelUpBonusRatio;                        //레벨업 보너스 경험치 비율

    [Header("Facility")]
    [SerializeField] private List<int> _FacilityCardGradeBaseSubTimeRatio;
    [SerializeField] private List<float> _FacilityCardGradeSubTimeRatio;
    [SerializeField] private int _FacCombineCntMax = 50;                            //재료 최대 갯수
    [SerializeField] private int _ParticipantMaxCount = 10;                         //작전회의실 최대 참가 인원

    [Header("Figure Room")]
    [SerializeField] private int _MaxServerRoomSaveSlot = 1;                      // 최대 서버 저장 룸 슬롯 개수

    [Header("Stage")]
    [SerializeField] private int _StageMissionClearCash = 1;
    [SerializeField] private float _StageGoldDropAddRate = 0.001f;
    [SerializeField] private float _StageGoldDropAddMaxRate = 0.15f;
    [SerializeField] private float _StageDailyCostTicketRate = 0.5f;
    [SerializeField] private int _FastQuestTicketID = 35001;                         // 작전 계획서 ID
    [SerializeField] private int _FastQuestTicketCount = 5;                          // 토벌권 충전 수량 (사용안함)
    [SerializeField] private int _FastQuestUnlockRank = 30;                          // 무인 작전 언락 랭크
    [SerializeField] private List<int> _SecretQuestOpenKind;                         // 시크릿 퀘스트 입장 조건 리스트 (스테이지ID)
    [SerializeField] private int _SecretQuestEntranceCount = 1;                      // 시크릿 퀘스트 입장 횟수 (대마인 개별)
    [SerializeField] private int _SecretResetTicketID = 10090;                       // 시크릿 퀘스트 입장 초기화 아이템 ID
    [SerializeField] private int _SecretResetTicketCount = 1;                        // 시크릿 퀘스트 입장 아이템 수량
    [SerializeField] private int _StageHackSec = 25;                                 // 타임어택 최소 시간 체크
    [SerializeField] private int _ContPresetSlot = 5;                                // 콘텐츠 프리셋 슬롯 개수
    [SerializeField] private int _MultipleMaxCount = 10;                             // 주회배수 최대값

    [Header("Store")]
    [SerializeField] private int _GetDPValue = 1;                                 // 가챠 횟수 당 획득 DP
    [SerializeField] private int _WakeUPCheckLevel = 81;                          // 웨이크업 패키지 보완 체크 레벨

    [Header("SpecialMode")]
    [SerializeField] private int _SpecialModeRefreshTime = 360;                   // 특별 모드 갱신 시간 (단위: 분)
    [SerializeField] private int _SpecialModeRewardCash = 3;                      // 특별 모드 클리어 시 보상 보석 개수
    [SerializeField] private int _SpecialModeChangeItemCnt = 1;                   // 특별 모드 변경에 필요한 아이템 개수
    [SerializeField] private int _SpecialModeResetItemCnt = 1;                    // 특별 모드 시간 초기화에 필요한 아이템 개수

    [Header("TimeAttackMode")]
    [SerializeField] private int _TimeAttackModeRecordDay = 15;                   // 타임 어택 모드 기록 저장 기간 (단위: 일)

    [Header("Arena")]
    [SerializeField] private int _ArenaEndDay = 0;                             // 배틀 아레나 종료 요일 (eDayOfWeek ENUM 참조, 0=일요일/1=월요일)
    [SerializeField] private int _ArenaEndTime = 22;                           // 배틀 아레나 종료 시간 (22=오후 10시를 의미)
    [SerializeField] private int _ArenaEndResultHour = 2;                      // 배틀 아레나 종료 처리 시간 (2=2시간을 의미)
    [SerializeField] private int _ArenaInitScore = 6000;                       // 배틀 아레나 시즌 시작 시 일괄 초기화될 배틀 스코어 (해당 값 이상은 해당 값으로 초기화)
    [SerializeField] private int _ArenaSubScore = 1000;                        // 배틀 아레나 시즌 시작 시 차감할 배틀 스코어
    [SerializeField] private int _ArenaMailTypeID = 5;                         // 배틀 아레나 보상 우편 타입 ID
    [SerializeField] private int _ArenaPromotionCnt = 3;                       // 배틀 아레나 승급전 횟수 (3=3전 2선승을 의미)
    [SerializeField] private float _ArenaMinRateScore = 0.25f;                 // 배틀 아레나 진행 시 배틀 스코어 최소 비율
    [SerializeField] private float _ArenaMaxRateScore = 2.0f;                  // 배틀 아레나 진행 시 배틀 스코어 최대 비율
    [SerializeField] private float _ArenaScoreBuffRate = 0.15f;                // 배틀 아레나 물약 버프 배틀 스코어 추가 획득량
    [SerializeField] private float _ArenaUseItemCoinRate = 0.25f;              // 배틀 아레나 물약 버프 배틀 코인 추가 획득량
    [SerializeField] private float _ArenaCoinBuffRate = 1.0f;                  // 배틀 아레나 버프 배틀 코인 추가 획득량
    [SerializeField] private float _ArenaCoinWinStreakRate = 0.035f;           // 배틀 아레나 연승 시 배틀 코인 추가 획득량
    [SerializeField] private int _ArenaCoinBuffPrice = 5000;                   // 배틀 아레나 버프 배틀 코인 가격
    [SerializeField] private int _ArenaMaxRankCnt = 100;                       // 배틀 아레나 최대 랭크 인원
    [SerializeField] private int _ArenaMaxMatchCnt = 200;                      // 배틀 아레나 최대 매칭 인원 (최고 등급 기준 / 랭커 제외)
    [SerializeField] private int _ArenaMatchMinCnt = 10;                       // 배틀 아레나 랭크 데이터 매칭 시 필요 최소 인원
    [SerializeField] private int _ArenaMatchWinStreakCnt = 5;                  // 배틀 아레나 랭크 데이터 매칭 시 필요 연승 횟수 (비순위권 유저 기준)
    [SerializeField] private int _ArenaUseBP = 1;                              // 배틀 아레나 매칭 시 소모 BP
    [SerializeField] private List<float> _ArenaWinRate;                        // 배틀 아레나 진행 시 결과 가중치 (패배, 선봉 승리, 중견 승리, 대장 승리)
    [SerializeField] private List<int> _ArenaBaseCoin;                         // 배틀 아레나 진행 시 등급별 배틀 코인 기본 획득량
    [SerializeField] private List<int> _ArenaBaseScore;                        // 배틀 아레나 진행 시 등급별 배틀 스코어 기본 획득 점수
    [SerializeField] private List<float> _ArenaLoseScoreRate;                  // 배틀 아레나 패배 시 등급별 배틀 스코어 차감 비율
    [SerializeField] private int _ArenaWinStreakLimit = 100;                   // 배틀 아레나 연승 시 획득 스코어 제한 (연승 기준)
    [SerializeField] private List<int> _ArenaTimeCheckWinStreak;               // 배틀 아레나 시간 체크 기준 연승
    [SerializeField] private List<int> _ArenaTimeCheckValue;                   // 배틀 아레나 시간 체크 값
    [SerializeField] private int _ArenaMaxDamage = 1000000;                    // 배틀 아레나 최대 대미지 제한 체크
    [SerializeField] private int _ArenaMaxTeamPower = 45000;                   // 배틀 아레나 최대 팀 전투력 최대 체크
    [SerializeField] private int _ArenaMinTeamPower = 9999;                    // 배틀 아레나 최대 팀 전투력 최소 체크

    [Header("Raid")]
    [SerializeField] private int _TeamChangeCoolDown = 10;                     // 레이드 리더 변경 재사용 시간 (초)
    [SerializeField] private List<int> _RaidClearStep;                         // 레이드 클리어 상승 단계
    [SerializeField] private List<int> _RaidClearTime;                         // 레이드 클리어 상승 기준 시간 (초)
    [SerializeField] private int _RaidEndDay = 0;                              // 레이드 초기화 요일 (0=일요일/1=월요일 – eDayOfWe)
    [SerializeField] private int _RaidEndTime = 21;                            // 레이드 초기화 시간 (24시간제)
    [SerializeField] private int _RaidEndResultHour = 3;                       // 레이드 초기화 진행 시간 (시)
    [SerializeField] private int _RaidMaxTeamPower = 45000;                    // 레이드 부정행위 체크 팀 최대 파워
    [SerializeField] private int _RaidMinTeamPower = 9999;                     // 레이드 부정행위 체크 팀 최소 파워
    [SerializeField] private List<int> _RaidTimeCheckStep;                     // 레이드 부정행위 체크 단계 기준
    [SerializeField] private List<int> _RaidTimeCheckValue;                    // 레이드 부정행위 체크 클리어 타임 (초)
    [SerializeField] private int _RaidMaxDamage = 3000000;                     // 레이드 부정행위 체크 팀 최대 대미지
    [SerializeField] private int _RaidStoreCount = 3;                          // 레이드 상점 랜덤 상품 등장 개수
    [SerializeField] private int _RaidStoreResetCost = 5;                      // 레이드 상점 랜덤 상품 갱신 비용 (대마석)
    [SerializeField] private int _RaidPointDailyLimit = 1000;                  // 레이드 포인트 일 획득 제한 수량
    [SerializeField] private int _RaidStoreTimeInterval = 4;                   // 레이드 상점 갱신 시간 간격 (횟수)
    [SerializeField] private int _RewardRaidPoint = 200;                       // 레이드 기본 지급 포인트
    [SerializeField] private float _RaidPointStepRate = 0.05f;                 // 레이드 기본 지급 포인트 스텝별 증가 비율
    [SerializeField] private float _RaidPointFailRate = 0.5f;                  // 레이드 기분 지급 포인트 실패 시 보상 비율

    [Header("Circle")]
    [SerializeField] private int _CircleMakingRank = 30;                       // 서클 생성 제한 유저 랭크
    [SerializeField] private int _CircleMakingItemReqID = 30000;               // 서클 생성 재료 Game.ItemReqList.ID
    [SerializeField] private int _CircleNameChangeCost = 50;                   // 서클 이름 변경 비용 (대마석)
    [SerializeField] private int _CircleIntroWordsMaxNumber = 100;             // 서클 소개문 최대 글자 수 
    [SerializeField] private int _CircleIntroLineBreakMaxNumber = 4;           // 서클 소개문 최대 문장 줄 수
    [SerializeField] private int _CircleNameWordsMaxNumber = 16;               // 서클 이름 최대 글자 수
    [SerializeField] private int _CircleDefaultMemberNumber = 10;              // 서클 최초 생성 시 최대 인원 수
    [SerializeField] private int _CircleDefaultSubLeaderNumber = 1;            // 서클 최초 생성 시 최대 부부장 인원 수
    [SerializeField] private int _CircleAddMaxNumber = 20;                     // 서클 확장 가능한 최대 인원 수
    [SerializeField] private int _CircleAddSubLeaderMaxNumber = 2;             // 서클 확장 가능한 최대 부부장 인원 수
    [SerializeField] private int _CircleWaitingJoinMaxNumber = 10;             // 서클 동시 신청 가능 최대 수
    [SerializeField] private int _CircleRecommendMaxNumber = 10;               // 서클 추천 리스트에서 표시할 서클 수
    [SerializeField] private int _CircleRecommendMemberLoginTime = 48;         // 서클 추천 리스트의 최근 부원 로그인 기록 체크 기간 (단위 : 시간)
    [SerializeField] private int _CircleQuitDelayTime = 24;                    // 서클 탈퇴 후 유저가 서클 가입 가능 할 때까지 제한 시간 (단위 : 시간)
    [SerializeField] private int _CircleDeportDelayTime = 6;                   // 서클 추방 후 다른 유저 재 추방까지 대기 시간 (단위 : 시간)
    [SerializeField] private int _CircleCheckPresentCircleGold = 50;           // 서클 출석 체크 후 서클 에게 지급하는 서클 활동비 (서클 부원 1명마다)

    [Header("Chat")]
    [SerializeField] private int _ChatOverlapTime = 10;                       // 채팅 동일 문구 도배 횟수 체크 시간 (단위 : 초)
    [SerializeField] private int _ChatOverlapCount = 3;                        // 채팅 동일 문구 도배 횟수 체크 횟수
    [SerializeField] private int _ChatViewLineLimitNumber = 100;               // 채팅 표시 가능 최대 채팅 수
    [SerializeField] private int _ChatWordsLimitNumber = 40;                   // 채팅 1회에 전송 가능한 문자 수 (공백 포함)

    [Header("Badge")]
    [SerializeField] private int _BadgeMinOptVal = 1;                                   // 문양 랜덤 옵션 최소값
    [SerializeField] private int _BadgeMaxOptVal = 10;                                  // 문양 랜덤 옵션 최대값
    [SerializeField] private int _BadgeLvCnt = 10;                                      // 문양 최대 강화 횟수
    [SerializeField] private int _BadgeLvInitCost = 10000;                              // 문양 강화 횟수 초기화 비용
    [SerializeField] private int _BadgeLvInitMatCnt = 3;                                // 문양 강화 횟수 초기화 필요 재료 개수
    [SerializeField] private int _BadgeSellPrice = 30;                                  // 문양 판매 시 획득 배틀 코인
    [SerializeField] private List<float> _BadgeSetAddRate;                              // 문양 세트 효과 옵션 증가 비율
    [SerializeField] private List<int> _BadgeLvUpMatCntByLv;                            // 문양 레벨별 강화 재료 필요 개수
    [SerializeField] private List<int> _BadgeLvUpRateByLv;                              // 문양 레벨별 강화 확률
    [SerializeField] private List<int> _BadgeLvUpCostByLv;                              // 문양 레벨별 강화 비용

    [Header("Friend")]
    [SerializeField] private int _FriendAddMaxNumber = 50;                              // 친구 최대 인원
    [SerializeField] private int _FriendAskMaxNumber = 10;                              // 친구 요청 대기 최대 인원
    [SerializeField] private int _FriendReadyMaxNumber = 20;                            // 친구 승인 대기 최대 인원
    [SerializeField] private int _FriendSendFPCoolHour = 23;                            // 우정 포인트 쿨타임 시간
    [SerializeField] private int _FriendSendFPGetValue = 5;                             // 우정 포인트 보내기 시 획득 수치
    [SerializeField] private int _FriendReceiveFPGetValue = 3;                          // 우정 포인트 받기 시 획득 수치
    [SerializeField] private int _FriendRecommendMaxNumber = 200;                       // 친구 추천 검색 대상 인원
    [SerializeField] private int _FriendRecommendRndNumber = 10;                        // 친구 추천 랜덤 인원
    [SerializeField] private int _FriendRecommendTimeSec = 3;                           // 친구 추천 재검색 시간 (단위: 초)
    [SerializeField] private int _FriendCharUseItemCnt = 1;                             // 친구 캐릭터 사용 시 필요한 아이템 개수
    [SerializeField] private int _FriendCharUseFPGetValue = 2;                          // 친구 캐릭터 지원 시 획득 우정 포인트

    [Header("WpnDepot")]
    [SerializeField] private int _WpnDepotItemReqGroup = 9000;                          // 무기고 슬롯 확장 아이템 그룹 아이디
    [SerializeField] private List<float> _WpnDepotBaseRatio;                            // 무기고 장착된 무기 등급별 기본 증가량
    [SerializeField] private List<float> _WpnDepotLvRatio;                              // 무기고 장착된 무기 등급별 레벨에 따른 증가량

    [Header("WebAddr")]
    [SerializeField] private string _WebNotice;                              //공지사항
    [SerializeField] private string _WebHelp;                                //헬프
    [SerializeField] private string _WebQuestions;                           //문의사항
    [SerializeField] private string _WebTermsofservice;                      //이용약관
    [SerializeField] private string _WebCashpaymentmethod;                   //현금결제법
    [SerializeField] private string _WebSpecifiedCommercialTransactionsAct;  //특정상거래법 
    [SerializeField] private List<string> _WebLanguageAddrList;
    [SerializeField] private string _WebCoupon;                              //쿠폰입력 (라이브 서버)
    [SerializeField] private string _WebReCoupon;                              //쿠폰입력 (리뷰 서버)
    [SerializeField] private string _WebInCoupon;                              //쿠폰입력 (내부 서버)

    [Header("Network Time Out")]
    [SerializeField] private float _NetTimeOutSend = 15.0f;
    [SerializeField] private float _NetTimeOutConnect = 15.0f;
    [SerializeField] private float _NetTitleOutSend = 7.0f;

    [Header("UI")]
    [SerializeField] private int _AchieveContributionGroupID = 90001;
    [SerializeField] private int _TicketAPStoreID = 200;
    [SerializeField] private int _TicketBPStoreID = 201;
    [SerializeField] private int _PrivateRoomOpenRank = 5;
    [SerializeField] private int _TutorialGachaStoreID = 23;
    [SerializeField] private int _SalePremiumGachaStoreID = 5;
    [SerializeField] private int _SaleGoldGachaStoreID = 8;
    [SerializeField] private int _FreeGachaStoreID = 7;
    [SerializeField] private int _ArenaOpenRank = 20;
    [SerializeField] private int _ReviewOpenRank = 10;

    [SerializeField] private List<int> _StageTypeOpenIDList;
    [SerializeField] private List<string> _LoadingPopupBGList;
    [SerializeField] private float _MessageEventDuration = 2.0f;                  //*클라만 사용
    [SerializeField] private float _MessageToastDuration = 2.0f;                  //*클라만 사용
    [SerializeField] private float _MessageNotifyDuration = 10.0f;                //*클라만 사용
    [SerializeField] private float _NoticeLevelUpDuration = 4.0f;                 //*클라만 사용
    [SerializeField] private float _NoticeOpenDuration = 4.0f;                    //*클라만 사용
    [SerializeField] private float _NoticePopupDuration = 5.0f;                   //*클라만 사용
    [SerializeField] private int _ShowStageHand = 10;                         //해당 스테이지 클리어 전까지 로비에 손가락 표시 나옴
    [SerializeField] private string _LobbyBGFileName = "LobbyScreenShot.png";

    public float MissionStartNoticeDuration = 5.0f;
    public float DefenceModeWarningNoticeDuration = 2.0f;

    [SerializeField] private float _ResultGaugeDuration = 2.0f;                   //*클라만 사용
    [SerializeField] private float _ResultGoodsDuration = 2.0f;                   //*클라만 사용
    [SerializeField] private float _ResultItemDuration = 2.0f;                    //*클라만 사용
    [SerializeField] private float _TestDuration = 4.0f;                          //*클라만 사용
    [SerializeField] private List<Color> _TextColor;                              //*클라만 사용
    [SerializeField] private List<Color> _GemGaugeColor;                          //*클라만 사용
    [SerializeField] private List<Color> _MultipleRewardRateColor;                //*클라만 사용
    [SerializeField] private List<Color> _CostumeGradeColor;                      //*클라만 사용
    [SerializeField] private List<Color> _MonsterGradeColor;                      //*클라만 사용
    [SerializeField] private Color _GachaNameColor;                               //*클라만 사용
    [SerializeField] private Color _GachaRateColor;                               //*클라만 사용
    [SerializeField] private Color _GachaPickupFontColor;                         //*클라만 사용
    [SerializeField] private List<AnimationCurve> _AnimationCurve;                //*클라만 사용

    [SerializeField] private int _NameMaxLength = 16;                             //*클라용 - 닉네임 최대 글자수
    [SerializeField] private int _NameAddLength = 10;                             //*클라용 - 닉네임 최대 글자수에 더해줄 숫자(일본어 입력기 중에 영어에서 한자, 일본어로 변환하는 것 때문에 여유 글자수를 더줌
    [SerializeField] private int _WordMaxLength = 100;
    [SerializeField] private int _WordAddLength = 15;

    [SerializeField] private bool _NotchTake = false;                           //*클라만 사용 - 노치대응 여부(true여도 아이폰만 가능)

    [SerializeField] private int _SpecialBuyOpenStageCnt = 20;                  //스페셜 팝업 오픈 - 스테이지 플레이 횟수
    [SerializeField] private int _SpecialBuyCloseCnt = 2;                       //스페셜 팝업 닫기 버튼으로 닫은 카운트
    [SerializeField] private int _SpecialBuyLimitLv = 70;                       //스페셜 팝업 레벨제한 (해당 레빌 이상은 오픈되지 않음)
    [SerializeField] private int _SpecialCheckSection = 5;                      //스페셜 팝업 - 체크할 섹션
    [SerializeField] private int _SpecialCheckDifficulty = 1;                   //스페셜 팝업 - 체크할 난이도

    [SerializeField] private float _EvtResetGachaAutoTimeSec = 1.0f;              //*클라만 사용 - 이벤트 가챠 10연차 간 딜레이 시간
    [SerializeField] private float _GemOptAutoTimeSec = 1.5f;                     //*클라만 사용 - 자동 옵션 재설정 간 딜레이 시간
    [SerializeField] private float _SkillLevelUpDelayTimeSec = 1.0f;              //*클라만 사용 - 스킬 레벨업 버튼 딜레이 시간
    [SerializeField] private int _GoldGachaTenMaxCnt = 50;                      //*클라만 사용 - 골드 가챠 10연차 최대 횟수
    [SerializeField] private float _GoldGachaAutoTimeSec = 1.0f;                   //*클라만 사용 - 골드 가챠 자동 10연차 간 딜레이 시간


    [Header("PushNotification")]
    [SerializeField] private int _NightPushStartTime = 22;
    [SerializeField] private int _NightPushEndTime = 8;

    [Header("Screen Shot")]
    public int ScreenShotMaxSelectCount = 20;
    public int ScreenShotCountPerPage = 5;
    public int ScreenShotPageCount = 10;
    public float ScreenShotWidthRatio = 0.5f;
    public float ScreenShotHeightRatio = 0.5f;
    public float ScreenShotGIFRatio = 0.5f;

    [Header("MonthlyFee")]
    [SerializeField] private int _MonthlyFeeTotalDay = 30;                      //월 정액 일수(30일)
    [SerializeField] private int _MonthlyFeeLimitMin = 7200;                       //월 정액 연장 체크 분(남은 일자가 5일 이하일때 체크 용)


    [Header("Rotation Gacha")]
    [SerializeField] private int _RotationGachaCheckTime = 0;                   //로테이션 가챠 - 임시오픈 마감 체크

    [Header("Event Store")]
    [SerializeField] private int _EventStoreGoodsTableID = 0;                   //로테이션 가챠 - 임시오픈 마감 체크

    public bool TestMode { get { return _TestMode; } set { _TestMode = value; } }
    public int TestLevel { get { return _TestLevel; } }
    public int TestCharLevel { get { return _TestCharLevel; } }
    public int TestInitCharTID { get { return _TestInitCharTID; } }

    public int InitGold { get { return _InitGold; } }
    public int InitCash { get { return _InitCash; } }
    public int InitSupporterPoint { get { return _InitSupporterPoint; } }
    public int InitRoomPoint { get { return _InitRoomPoint; } }
    public int InitBattleCoin { get { return _InitBattleCoin; } }
    public int InitFriendPoint { get { return _InitFriendPoint; } }
    public int InitCharPassviePoint { get { return _InitCharPassviePoint; } }
    public int InitRoomID { get { return _InitRoomID; } }
    public int InitUserMarkID { get { return _InitUserMarkID; } }
    public int InitLoginBonusGroupID { get { return _InitLoginBonusGroupID; } }
    public int InitItemMailTypeID { get { return _InitItemMailTypeID; } }
    public int InitSpecialModeID { get { return _InitSpecialModeID; } }
    public int InitTutorialItemGroupID { get { return _InitTutorialItemGroupID; } }
    public int InitLobbyThemeID { get { return _InitLobbyThemeID; } }
    public string InitAccountNickName { get { return _InitAccountNickName; } }

    public List<int> InitGoldList { get { return _InitGoldList; } }
    public List<int> InitCashList { get { return _InitCashList; } }
    public List<int> InitWeaponList { get { return _InitWeaponList; } }
    public List<int> InitGemList { get { return _InitGemList; } }
    public List<int> InitCardList { get { return _InitCardList; } }
    public List<int> InitItemList { get { return _InitItemList; } }
    public List<int> InitItemCountList { get { return _InitItemCountList; } }
    public List<int> InitCostumeList { get { return _InitCostumeList; } }
    public List<int> InitFigureList { get { return _InitFigureList; } }
    public List<int> InitUserMarkList { get { return _InitUserMarkList; } }

    public int LimitMaxGold { get { return _LimitMaxGold; } }
    public int LimitMaxCash { get { return _LimitMaxCash; } }
    public int LimitMaxSP { get { return _LimitMaxSP; } }
    public int LimitMaxRP { get { return _LimitMaxRP; } }
    public int LimitMaxAP { get { return _LimitMaxAP; } }
    public int LimitMaxBP { get { return _LimitMaxBP; } }
    public int LimitMaxBattleCoin { get { return _LimitMaxBattleCoin; } }
    public int LimitMaxFriendPoint { get { return _LimitMaxFriendPoint; } }
    public int LimitMaxItemStack { get { return _LimitMaxItemStack; } }
    public int LimitMaxSkillPT { get { return _LimitMaxSkillPT; } }
    public int LimitMaxDP { get { return _LimitMaxDP; } }
    public int LimitMaxWP { get { return _LimitMaxWP; } }
    public int LimitMaxRaidPoint { get { return _LimitMaxRaidPoint; } }
    public int LimitMaxCirclePoint { get { return _LimitMaxRaidPoint; } }
    public int LimitMaxCircleGold { get { return _LimitMaxRaidPoint; } }

    public int AccountMaxLevel { get { return _AccountMaxLevel; } }
    public int AccountLevelUpGroup { get { return _AccountLevelUpGroup; } }
    public int APUpdateTimeSec { get { return _APUpdateTimeSec; } }
    public int BPUpdateTimeSec { get { return _BPUpdateTimeSec; } }
    public int BPMaxCount { get { return _BPMaxCount; } }

    public int BaseItemSlotCount { get { return _BaseItemSlotCount; } }
    public int MaxItemSlotCount { get { return _MaxItemSlotCount; } }
    public int AddItemSlotCount { get { return _AddItemSlotCount; } }
    public int AddItemSlotGold { get { return _AddItemSlotGold; } }
    public int AddItemSlotCash { get { return _AddItemSlotCash; } }
    public int AddItemSlotCashCount { get { return _AddItemSlotCashCount; } }
    public int MatCount { get { return _MatCount; } }
    public int SellCount { get { return _SellCount; } }
    public float RPRatePerTicket { get { return _RPRatePerTicket; } }
    public int MaxMailCnt { get { return _MaxMailCnt; } }
    public int DefaultDelMailDay { get { return _DefaultDelMailDay; } }
    public int DefaultMailTypeID { get { return _DefaultMailTypeID; } }
    public int WeeklyMailTypeID { get { return _WeeklyMailTypeID; } }
    public int WeeklyResetDay { get { return _WeeklyResetDay; } }
    public int WeeklyResetTime { get { return _WeeklyResetTime; } }
    public int EvtResetGachaReqCnt { get { return _EvtResetGachaReqCnt; } }
    public int EvtResetGachaMaxNum { get { return _EvtResetGachaMaxNum; } }
    public List<int> MultipleList { get { return _MultipleList; } }
    public List<int> MultipleRewardRateList { get { return _MultipleRewardRateList; } }
    public int ReturnUserRule { get { return _ReturnUserRule; } }
    public int AccountInterlockReward { get { return _AccountInterlockReward; } }
    public int RepresentativeCount { get { return _RepresentativeCount; } }
    public int NewUserTerm { get { return _NewUserTerm; } }
    public int ReturnUserTerm { get { return _ReturnUserTerm; } }
    public int AbsentCostType { get { return _AbsentCostType; } }
    public int AbsentCostIndex { get { return _AbsentCostIndex; } }
    public int AbsentCostValue { get { return _AbsentCostValue; } }
    public int AbsentGracePeriod { get { return _AbsentGracePeriod; } }
    public int LoginBonusMonthlyDayMax { get { return _LoginBonusMonthlyDayMax; } }
    public int MailRefreshTimeSec { get { return _MailRefreshTimeSec; } }
    public int AwakeSkillClearCash { get { return _AwakeSkillClearCash; } }
    public int AwakeOpenGrade { get { return _AwakeOpenGrade; } }
    public int AwakeSkillClearItemID { get { return _AwakeSkillClearItemID; } }
    public int CharMaxGrade { get { return _CharMaxGrade; } }
    public List<float> CharGradeStatRate { get { return _CharGradeStatRate; } }             //*클라만 사용
    public List<int> CharMaxLevel { get { return _CharMaxLevel; } }
    public int CharLevelUpGroup { get { return _CharLevelUpGroup; } }
    public int CharFavorMaxLevel { get { return _CharFavorMaxLevel; } }
    public int CharFavorLevelUpGroup { get { return _CharFavorLevelUpGroup; } }
    public int CharOpenTermsLevel { get { return _CharOpenTermsLevel; } }
    public int CharOpenTermsPreference { get { return _CharOpenTermsPreference; } }
    public List<int> CharCardSlotLimitLevel { get { return _CharCardSlotLimitLevel; } }     //*클라만 사용
    public List<int> CharSkillSlotLimitLevel { get { return _CharSkillSlotLimitLevel; } }   //*클라만 사용
    public List<int> CharWeaponSlotLimitLevel { get { return _CharWeaponSlotLimitLevel; } } //*클라만 사용
    public int CharLoginMsgAddHour { get { return _CharLoginMsgAddHour; } }                 //접속 안한지 n시간 뒤 로컬 푸시 시간
    public List<int> CharLoginMsgStringIdx { get { return _CharLoginMsgStringIdx; } }       //접속 안한지 n시간 뒤 로컬 푸시 메시지
    public int CharOpenAddAP { get { return _CharOpenAddAP; } }
    public int CharAwakeGetWP { get { return _CharAwakeGetWP; } }
    public int CharWPStartGrade { get { return _CharWPStartGrade; } }
    public int CharLevelGetWP { get { return _CharLevelGetWP; } }
    public int CharWPStartLevel { get { return _CharWPStartLevel; } }
    public int RandomDyeingMat { get { return _RandomDyeingMat; } }
    public int RandomDyeingCost { get { return _RandomDyeingCost; } }
    public int RandomDyeingRockCost { get { return _RandomDyeingRockCost; } }
    public int DayPresentCount { get { return _DayPresentCount; } }
    public float PreferenceStepRate1 { get { return _PreferenceStepRate1; } }
    public float PreferenceStepRate2 { get { return _PreferenceStepRate2; } }
    public int CharPresetSlot { get { return _CharPresetSlot; } }

    public List<int> WeaponGradeSlot { get { return _WeaponGradeSlot; } }
    public List<int> WeaponGradeSlotWakeInc { get { return _WeaponGradeSlotWakeInc; } }
    public int WeaponLevelupCostByLevel { get { return _WeaponLevelupCostByLevel; } }
    public float WeaponExpMatWeigth { get { return _WeaponExpMatWeigth; } }
    public List<int> WeaponMaxLevel { get { return _WeaponMaxLevel; } }
    public List<int> WeaponWakeIncLevel { get { return _WeaponWakeIncLevel; } }
    public List<int> WeaponWakeMax { get { return _WeaponWakeMax; } }
    public List<float> WeaponWakeStatRate { get { return _WeaponWakeStatRate; } }
    public int WeaponMaxSkillLevel { get { return _WeaponMaxSkillLevel; } }
    public List<float> WeaponSkillLvStatRate { get { return _WeaponSkillLvStatRate; } }
    public List<float> WeaponSubSlotStatBySLV { get { return _WeaponSubSlotStatBySLV; } }
    public List<int> WeaponSkillLevelupCostByGrade { get { return _WeaponSkillLevelupCostByGrade; } }

    public List<int> GemLevelupCostByGrade { get { return _GemLevelupCostByGrade; } }
    public float GemExpMatWeigth { get { return _GemExpMatWeigth; } }
    public int GemMaxLevel { get { return _GemMaxLevel; } }
    public int GemMaxWake { get { return _GemMaxWake; } }
    public List<float> GemWakeStatRate { get { return _GemWakeStatRate; } }
    public int GemSetGrade { get { return _GemSetGrade; } }
    public int GemAnalyzeCostType { get { return _GemAnalyzeCostType; } }
    public int GemAnalyzeCostIndex { get { return _GemAnalyzeCostIndex; } }
    public int GemAnalyzeCostValue { get { return _GemAnalyzeCostValue; } }

    public int CardLevelupCostByLevel { get { return _CardLevelupCostByLevel; } }
    public float CardExpMatWeigth { get { return _CardExpMatWeigth; } }
    public List<int> CardMaxLevel { get { return _CardMaxLevel; } }
    public List<int> CardWakeIncLevel { get { return _CardWakeIncLevel; } }
    public List<int> CardWakeMax { get { return _CardWakeMax; } }
    public List<float> CardWakeStatRate { get { return _CardWakeStatRate; } }
    public int CardMaxSkillLevel { get { return _CardMaxSkillLevel; } }
    public List<float> CardSkillLvStatRate { get { return _CardSkillLvStatRate; } }
    public List<float> CardSubSlotStatBySLV { get { return _CardSubSlotStatBySLV; } }
    public List<int> CardSkillLevelupCostByGrade { get { return _CardSkillLevelupCostByGrade; } }
    public int CardFavorMaxLevel { get { return _CardFavorMaxLevel; } }
    public List<int> CardFavorBonusDropRate { get { return _CardFavorBonusDropRate; } }
    public int CardChangeGold { get { return _CardChangeGold; } }
    public int CardChangeCash { get { return _CardChangeCash; } }
    public int CardFormationFavoritesCount { get { return _CardFormationFavoritesCount; } }
    public int CardDispatchFastItemID { get { return _CardDispatchFastItemID; } }
    public int CardDispatchFastItemValue { get { return _CardDispatchFastItemValue; } }
    public List<int> LevelUpBonusRate { get { return _LevelUpBonusRate; } }
    public List<float> LevelUpBonusRatio { get { return _LevelUpBonusRatio; } }

    public List<int> FacilityCardGradeBaseSubTimeRatio { get { return _FacilityCardGradeBaseSubTimeRatio; } }
    public List<float> FacilityCardGradeSubTimeRatio { get { return _FacilityCardGradeSubTimeRatio; } }
    public int FacCombineCntMax { get { return _FacCombineCntMax; } }
    public int ParticipantMaxCount { get { return _ParticipantMaxCount; } }

    public int MaxServerRoomSaveSlot { get { return _MaxServerRoomSaveSlot; } }

    public int StageMissionClearCash { get { return _StageMissionClearCash; } }
    public float StageGoldDropAddRate { get { return _StageGoldDropAddRate; } }
    public float StageGoldDropAddMaxRate { get { return _StageGoldDropAddMaxRate; } }
    public float StageDailyCostTicketRate { get { return _StageDailyCostTicketRate; } }
    public int FastQuestTicketID { get { return _FastQuestTicketID; } }
    public int FastQuestTicketCount { get { return _FastQuestTicketCount; } }
    public int FastQuestUnlockRank { get { return _FastQuestUnlockRank; } }
    public List<int> SecretQuestOpenKind { get { return _SecretQuestOpenKind; } }
    public int SecretQuestEntranceCount { get { return _SecretQuestEntranceCount; } }
    public int SecretResetTicketID { get { return _SecretResetTicketID; } }
    public int SecretResetTicketCount { get { return _SecretResetTicketCount; } }
    public int StageHackSec { get { return _StageHackSec; } }
    public int ContPresetSlot { get { return _ContPresetSlot; } }
    public int MultipleMaxCount { get { return _MultipleMaxCount; } }

    public int GetDPValue { get { return _GetDPValue; } }
    public int WakeUPCheckLevel { get { return _WakeUPCheckLevel; } }

    public int SpecialModeRefreshTime { get { return _SpecialModeRefreshTime; } }
    public int SpecialModeRewardCash { get { return _SpecialModeRewardCash; } }
    public int SpecialModeChangeItemCnt { get { return _SpecialModeChangeItemCnt; } }
    public int SpecialModeResetItemCnt { get { return _SpecialModeResetItemCnt; } }

    public int TimeAttackModeRecordDay { get { return _TimeAttackModeRecordDay; } }

    public int ArenaEndDay { get { return _ArenaEndDay; } }
    public int ArenaEndTime { get { return _ArenaEndTime; } }
    public int ArenaEndResultHour { get { return _ArenaEndResultHour; } }
    public int ArenaInitScore { get { return _ArenaInitScore; } }
    public int ArenaSubScore { get { return _ArenaSubScore; } }
    public int ArenaMailTypeID { get { return _ArenaMailTypeID; } }
    public int ArenaPromotionCnt { get { return _ArenaPromotionCnt; } }
    public float ArenaMinRateScore { get { return _ArenaMinRateScore; } }
    public float ArenaMaxRateScore { get { return _ArenaMaxRateScore; } }
    public float ArenaScoreBuffRate { get { return _ArenaScoreBuffRate; } }
    public float ArenaUseItemCoinRate { get { return _ArenaUseItemCoinRate; } }
    public float ArenaCoinBuffRate { get { return _ArenaCoinBuffRate; } }
    public float ArenaCoinWinStreakRate { get { return _ArenaCoinWinStreakRate; } }
    public int ArenaCoinBuffPrice { get { return _ArenaCoinBuffPrice; } }
    public int ArenaMaxRankCnt { get { return _ArenaMaxRankCnt; } }
    public int ArenaMaxMatchCnt { get { return _ArenaMaxMatchCnt; } }
    public int ArenaMatchMinCnt { get { return _ArenaMatchMinCnt; } }
    public int ArenaMatchWinStreakCnt { get { return _ArenaMatchWinStreakCnt; } }
    public int ArenaUseBP { get { return _ArenaUseBP; } }
    public List<float> ArenaWinRate { get { return _ArenaWinRate; } }
    public List<int> ArenaBaseCoin { get { return _ArenaBaseCoin; } }
    public List<int> ArenaBaseScore { get { return _ArenaBaseScore; } }
    public List<float> ArenaLoseScoreRate { get { return _ArenaLoseScoreRate; } }
    public int ArenaWinStreakLimit { get { return _ArenaWinStreakLimit; } }
    public List<int> ArenaTimeCheckWinStreak { get { return _ArenaTimeCheckWinStreak; } }
    public List<int> ArenaTimeCheckValue { get { return _ArenaTimeCheckValue; } }
    public int ArenaMaxDamage { get { return _ArenaMaxDamage; } }
    public int ArenaMaxTeamPower { get { return _ArenaMaxTeamPower; } }
    public int ArenaMinTeamPower { get { return _ArenaMinTeamPower; } }

    public int TeamChangeCoolDown { get { return _TeamChangeCoolDown; } }
    public List<int> RaidClearStep { get { return _RaidClearStep; } }
    public List<int> RaidClearTime { get { return _RaidClearTime; } }
    public int RaidEndDay { get { return _RaidEndDay; } }
    public int RaidEndTime { get { return _RaidEndTime; } }
    public int RaidEndResultHour { get { return _RaidEndResultHour; } }
    public int RaidMaxTeamPower { get { return _RaidMaxTeamPower; } }
    public int RaidMinTeamPower { get { return _RaidMinTeamPower; } }
    public List<int> RaidTimeCheckStep { get { return _RaidTimeCheckStep; } }
    public List<int> RaidTimeCheckValue { get { return _RaidTimeCheckValue; } }
    public int RaidMaxDamage { get { return _RaidMaxDamage; } }
    public int RaidStoreCount { get { return _RaidStoreCount; } }
    public int RaidStoreResetCost { get { return _RaidStoreResetCost; } }
    public int RaidPointDailyLimit { get { return _RaidPointDailyLimit; } }
    public int RaidStoreTimeInterval { get { return _RaidStoreTimeInterval; } }
    public int RewardRaidPoint { get { return _RewardRaidPoint; } }
    public float RaidPointStepRate { get { return _RaidPointStepRate; } }
    public float RaidPointFailRate { get { return _RaidPointFailRate; } }

    public int CircleMakingRank { get { return _CircleMakingRank; } }
    public int CircleMakingItemReqID { get { return _CircleMakingItemReqID; } }
    public int CircleNameChangeCost { get { return _CircleNameChangeCost; } }
    public int CircleIntroWordsMaxNumber { get { return _CircleIntroWordsMaxNumber; } }
    public int CircleIntroLineBreakMaxNumber { get { return _CircleIntroLineBreakMaxNumber; } }
    public int CircleNameWordsMaxNumber { get { return _CircleNameWordsMaxNumber; } }
    public int CircleDefaultMemberNumber { get { return _CircleDefaultMemberNumber; } }
    public int CircleDefaultSubLeaderNumber { get { return _CircleDefaultSubLeaderNumber; } }
    public int CircleAddMaxNumber { get { return _CircleAddMaxNumber; } }
    public int CircleAddSubLeaderMaxNumber { get { return _CircleAddSubLeaderMaxNumber; } }
    public int CircleWaitingJoinMaxNumber { get { return _CircleWaitingJoinMaxNumber; } }
    public int CircleRecommendMaxNumber { get { return _CircleRecommendMaxNumber; } }
    public int CircleRecommendMemberLoginTime { get { return _CircleRecommendMemberLoginTime; } }
    public int CircleQuitDelayTime { get { return _CircleQuitDelayTime; } }
    public int CircleDeportDelayTime { get { return _CircleDeportDelayTime; } }
    public int CircleCheckPresentCircleGold { get { return _CircleCheckPresentCircleGold; } }

    public int ChatOverlapTime { get { return _ChatOverlapTime; } }
    public int ChatOverlapCount { get { return _ChatOverlapCount; } }
    public int ChatViewLineLimitNumber { get { return _ChatViewLineLimitNumber; } }
    public int ChatWordsLimitNumber { get { return _ChatWordsLimitNumber; } }

    public int BadgeMinOptVal { get { return _BadgeMinOptVal; } }
    public int BadgeMaxOptVal { get { return _BadgeMaxOptVal; } }
    public int BadgeLvCnt { get { return _BadgeLvCnt; } }
    public int BadgeLvInitCost { get { return _BadgeLvInitCost; } }
    public int BadgeLvInitMatCnt { get { return _BadgeLvInitMatCnt; } }
    public int BadgeSellPrice { get { return _BadgeSellPrice; } }
    public List<float> BadgeSetAddRate { get { return _BadgeSetAddRate; } }
    public List<int> BadgeLvUpMatCntByLv { get { return _BadgeLvUpMatCntByLv; } }
    public List<int> BadgeLvUpRateByLv { get { return _BadgeLvUpRateByLv; } }
    public List<int> BadgeLvUpCostByLv { get { return _BadgeLvUpCostByLv; } }

    public int FriendAddMaxNumber { get { return _FriendAddMaxNumber; } }
    public int FriendAskMaxNumber { get { return _FriendAskMaxNumber; } }
    public int FriendReadyMaxNumber { get { return _FriendReadyMaxNumber; } }
    public int FriendSendFPCoolHour { get { return _FriendSendFPCoolHour; } }
    public int FriendSendFPGetValue { get { return _FriendSendFPGetValue; } }
    public int FriendReceiveFPGetValue { get { return _FriendReceiveFPGetValue; } }
    public int FriendRecommendMaxNumber { get { return _FriendRecommendMaxNumber; } }
    public int FriendRecommendRndNumber { get { return _FriendRecommendRndNumber; } }
    public int FriendRecommendTimeSec { get { return _FriendRecommendTimeSec; } }
    public int FriendCharUseItemCnt { get { return _FriendCharUseItemCnt; } }
    public int FriendCharUseFPGetValue { get { return _FriendCharUseFPGetValue; } }

    public int WpnDepotItemReqGroup { get { return _WpnDepotItemReqGroup; } }
    public List<float> WpnDepotBaseRatio { get { return _WpnDepotBaseRatio; } }
    public List<float> WpnDepotLvRatio { get { return _WpnDepotLvRatio; } }

    public string WebNotice { get { return _WebNotice; } }
    public string WebHelp { get { return _WebHelp; } }
    public string WebQuestions { get { return _WebQuestions; } }
    public string WebTermsofservice { get { return _WebTermsofservice; } }
    public string WebCashpaymentmethod { get { return _WebCashpaymentmethod; } }
    public string WebSpecifiedCommercialTransactionsAct { get { return _WebSpecifiedCommercialTransactionsAct; } }
    public List<string> WebLanguageAddrList { get { return _WebLanguageAddrList; } }
    public string WebCoupon { get { return _WebCoupon; } }
    public string WebReCoupon { get { return _WebReCoupon; } }
    public string WebInCoupon { get { return _WebInCoupon; } }

    public float NetTimeOutSend { get { return _NetTimeOutSend; } }
    public float NetTimeOutConnect { get { return _NetTimeOutConnect; } }
    public float NetTitleOutSend { get { return _NetTitleOutSend; } }

    public int AchieveContributionGroupID { get { return _AchieveContributionGroupID; } }
    public int TicketAPStoreID { get { return _TicketAPStoreID; } }
    public int TicketBPStoreID { get { return _TicketBPStoreID; } }
    public int PrivateRoomOpenRank { get { return _PrivateRoomOpenRank; } }
    public int TutorialGachaStoreID { get { return _TutorialGachaStoreID; } }
    public int SalePremiumGachaStoreID { get { return _SalePremiumGachaStoreID; } }
    public int SaleGoldGachaStoreID { get { return _SaleGoldGachaStoreID; } }
    public int FreeGachaStoreID { get { return _FreeGachaStoreID; } }
    public int ArenaOpenRank { get { return _ArenaOpenRank; } }
    public int ReviewOpenRank { get { return _ReviewOpenRank; } }

    public List<int> StageTypeOpenIDList { get { return _StageTypeOpenIDList; } }
    public List<string> LoadingPopupBGList { get { return _LoadingPopupBGList; } }
    public float MessageEventDuration { get { return _MessageEventDuration; } }
    public float MessageToastDuration { get { return _MessageToastDuration; } }
    public float MessageNotifyDuration { get { return _MessageNotifyDuration; } }
    public float NoticeLevelUpDuration { get { return _NoticeLevelUpDuration; } }
    public float NoticeOpenDuration { get { return _NoticeOpenDuration; } }
    public float NoticePopupDuration { get { return _NoticePopupDuration; } }
    public int ShowStageHand { get { return _ShowStageHand; } }
    public string LobbyBGFileName { get { return _LobbyBGFileName; } }
    public float ResultGaugeDuration { get { return _ResultGaugeDuration; } }
    public float ResultGoodsDuration { get { return _ResultGoodsDuration; } }
    public float ResultItemDuration { get { return _ResultItemDuration; } }
    public float TestDuration { get { return _TestDuration; } }

    public List<Color> TextColor { get { return _TextColor; } }
    public List<Color> GemGaugeColor { get { return _GemGaugeColor; } }
    public List<Color> MultipleRewardRateColor { get { return _MultipleRewardRateColor; } }
    public List<Color> CostumeGradeColor { get { return _CostumeGradeColor; } }
    public List<Color> MonsterGradeColor { get { return _MonsterGradeColor; } }
    public Color GachaNameColor { get { return _GachaNameColor; } }
    public Color GachaRateColor { get { return _GachaRateColor; } }
    public Color GachaPickupFontColor { get { return _GachaPickupFontColor; } }
    public List<AnimationCurve> AnimationCurve { get { return _AnimationCurve; } }

    public int NameMaxLength { get { return _NameMaxLength; } }
    public int NameAddLength { get { return _NameAddLength; } }
    public int WordMaxLength { get { return _WordMaxLength; } }
    public int WordAddLength { get { return _WordAddLength; } }

    public bool NotchTake { get { return _NotchTake; } }

    public int NightPushStartTime { get { return _NightPushStartTime; } }
    public int NightPushEndTime { get { return _NightPushEndTime; } }

    public int SpecialBuyOpenStageCnt { get { return _SpecialBuyOpenStageCnt; } }

    public int SpecialBuyCloseCnt { get { return _SpecialBuyCloseCnt; } }

    public int SpecialBuyLimitLv { get { return _SpecialBuyLimitLv; } }

    public int SpecialCheckSection { get { return _SpecialCheckSection; } }

    public int SpecialCheckDifficulty { get { return _SpecialCheckDifficulty; } }

    public float EvtResetGachaAutoTimeSec { get { return _EvtResetGachaAutoTimeSec; } }
    public float GemOptAutoTimeSec { get { return _GemOptAutoTimeSec; } }
    public float SkillLevelUpDelayTimeSec { get { return _SkillLevelUpDelayTimeSec; } }
    public int GoldGachaTenMaxCnt { get { return _GoldGachaTenMaxCnt; } }
    public float GoldGachaAutoTimeSec { get { return _GoldGachaAutoTimeSec; } }


    public int MonthlyFeeTotalDay { get { return _MonthlyFeeTotalDay; } }
    public int MonthlyFeeLimitMin { get { return _MonthlyFeeLimitMin; } }

    public int RotationGachaCheckTime { get { return _RotationGachaCheckTime; } }

    public int EventStoreGoodsTableID { get { return _EventStoreGoodsTableID; } }
}
