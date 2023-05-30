using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIValue : FSingleton<UIValue>
{
    public enum EParamType
    {
        None = 0,


        CharInfoTab,
        CharSelUID,
        CharSelTableID,
        CharEquipCardSlot,
        CharEquipSkillSlot,
        CharSkillPassiveID,
        CharSeletePopupType,    //0 = 
        CharCostumeID,
        GachaRewardIndex,
        GachaGreetings,
        //GachaResultType,  
        StageID,
        

        DeckSel,
        StoreTab,
        ArenaStoreTab,
        GachaTab,
        GachaDetailStoreID,
        
        BookItemListType,
        BookItemID,
        FacilityID,

        WebViewTitle,
        WebViewAddr,

        CharUID,        //선택된
        WeaponUID,
        GemUID,
        CardUID,
        ItemUID,
        StageCharCUID,

        WeaponTableID,
        GemTableID,
        CardTableID,
        ItemTableID,
        ItemTableCount,

        ScenarioGroupID,
        ScenarioFavorBGStr,
        ScenarioFavorBGSprite,

        SellSingleType,
        SellSingleUID,

        WeaponGemIndex,

        SelectRewardType,

        StroyID,
        
        StoryStageID,
        EventStageID,
        TimeAttackRankStageID,
        TimeAttackRankUUID,
        UserLevel,

        NoticeLevelUpType,
        TestString,

        
        LastPlayStageID,
        
        
        LoadingType,
        LoadingStage,

        HowToBeStrongCUID,

        EventID,          //이벤트 모드 ID

        TutorialState,
        TutorialStep,

        UserInfoPopup, 
        ShowHelpPopupInPausePopup,  // 도움말 창을 일시정지 창에서 열었는지 여부
        CharEquipWeaponSlot,        // 무기 슬롯 위치 (0 = 주무기, 1 = 보조무기)
        ChoicePopupType,            // 선택창 타입 (UIChoiceItemPpup 상단에 정의)
        EventStagePopupType,        // 가챠형 이벤트 스테이지, 아레나 프로롤그 같은 프리팹을 쓰기 때문에 구분용으로 정의
        ArenaPrologueStageID,       // 프롤로그 스테이지ID
        RulePopupType,              // 룰 안내 팝업 페이지 타입

        BadgeSlotIndex,             // Info 슬롯 선택
        BadgeUID,                   // 문양 UID
        ArenaTeamCharSlot,          // 아레나 팀 슬롯 위치
        ArenaCharInfoFlag,          // 아레나에서 CHARINFO 로 이동시 뒤로가기 눌렀을때 아레나로 돌아와야하기때문에 추가
        RankUserType,

        TimeAttackClearScore,

        SkipCinemaPictureMode,

        FirstClearStageID,          //해당 스테이지 처음 클리어

        IsFriendPVP,                // 친구와 PVP인지 여부


        ArenaTowerTeamCharSlot,     // 아레타 타워 팀 슬롯 위치
        ArenaTowerChoiceStage,      // 아레나 타워 선택 스테이지
        ArenaTowerFriendCUID,       // 아레나 타워 친구 CUID

        BadgeType,

        ArmoryWeaponUID,

        CardFormationType,

        TradeMaterial,
        StageType,

        EnchantUID,

        ItemTab,

        TemporaryCharTraining,
        DailyEventType,
        TemporaryWeaponTraining,
        TemporaryWeaponBookTraining,
        
        LobbyAnimTableID,
        RandomTableID,

        LobbyToTrainingRoom,
        TrainingCharTID,
        FilterOpenUI,

		GradeUpValue,
        GachaTabValue03,

        SelectedRaidCharIndex,
        SelectedRaidCuid,

        SelectFavorBuffCharIndex,
        SelectFavorBuffCuid,
    };

    private Dictionary<EParamType, object> DicParameter = new Dictionary<EParamType, object>();

    public void ClearParameter()
    {
        DicParameter.Clear();
    }

    public void SetValue(EParamType key, object value)
    {
        if (DicParameter.ContainsKey(key))
        {
            DicParameter[key] = value;
        }
        else
        {
            DicParameter.Add(key, value);
        }
    }

    public object GetValue(EParamType key, bool isRemove = false)
    {
        object outValue;
        if (DicParameter.TryGetValue(key, out outValue))
        {
            if (isRemove)
                DicParameter.Remove(key);
            return outValue;
        }
        return null;
    }

    public object TryGetValue(EParamType key, object defaultObj, bool isRemove = false)
    {
        object outValue = GetValue(key, isRemove);
        return outValue ?? defaultObj;
    }

    public bool ContainsKey(EParamType key)
    {
        return DicParameter.ContainsKey(key);
    }

    public bool RemoveValue(EParamType key)
    {
        object outValue;
        if (DicParameter.TryGetValue(key, out outValue))
        {
            DicParameter.Remove(key);
            return true;
        }
        return false;
    }

}
