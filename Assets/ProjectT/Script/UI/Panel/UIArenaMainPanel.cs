using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIArenaMainPanel : FComponent
{
    

    public UIGoodsUnit kArenaGoldUnit;
    public UIGoodsUnit kSerachGoods;
    public UILabel kDate;                               //아레나 개최 기간
	public UILabel kBattleScoreCountLb;                 //최고 배틀 스코어
	public UILabel kWinningCountLb;                     //최고 연승
	public UILabel kTotalBattleCountLb;                 //총 전적
	public UILabel kFirstWinCountLb;                    //선봉 승리
	public UILabel kSecondWinCountLb;                   //중견 승리
	public UILabel kThirdWinCountLb;                    //대장 승리
	public UILabel kseasonLabel;                        //시즌 기록 - 제거 해도 될듯
	
	public UILabel kWinLabel;                           //연승, 연패 여부

    public UIArenaBattleListSlot kUserArenaSlot;

    private UserBattleData _battleData;
    private List<long> _teamCharList;
    public List<long> TeamCharList { get { return _teamCharList; } }

    private const int _charListCnt = 3;                 //캐릭터 배치 가능 수
    private const int _badgeListCnt = 3;                //뱃지 배치 가능 수
    private int _charSlotSelectIdx = 0;
    public int CharSlotSelectIdx { get { return _charSlotSelectIdx; } }

    private GameTable.ArenaGrade.Param _arenaGradeTableData;

 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}

    public override void InitComponent()
	{
        UIValue.Instance.SetValue(UIValue.EParamType.CardFormationType, eCharSelectFlag.ARENA);

        _battleData = GameInfo.Instance.UserBattleData;
        _arenaGradeTableData = GameInfo.Instance.GameTable.FindArenaGrade(x => x.GradeID == GameInfo.Instance.UserBattleData.Now_GradeId);

        GameInfo.Instance.ArenaTowerFriendContainer.Clear();
    }
    
    public override void OnUIOpen()
    {
        LobbyUIManager.Instance.kBlackScene.SetActive(false);
    }

    public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        GameSupport.SetArenaBG(kBGIndex, true);
        _teamCharList = GameInfo.Instance.TeamcharList;
        kArenaGoldUnit.InitGoodsUnit(eGOODSTYPE.BATTLECOIN, GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.BATTLECOIN]);
        kSerachGoods.InitGoodsUnit(eGOODSTYPE.GOLD, _arenaGradeTableData.MatchPrice, true);

        System.DateTime endDayTime = GameSupport.GetCurrentWeekArenaEndTime();
        string endDayStr = string.Format(FLocalizeString.Instance.GetText(1452), endDayTime.Year, endDayTime.Month, endDayTime.Day);
        endDayStr = string.Format("{0} ({1}) {2}:{3}", endDayStr, FLocalizeString.Instance.GetText(190 + (int)endDayTime.DayOfWeek), endDayTime.Hour.ToString("D2"), endDayTime.Minute.ToString("D2"));
        kDate.textlocalize = string.Format(FLocalizeString.Instance.GetText(1399), endDayStr);
        if (_battleData == null)
            return;

        if(_battleData.Now_WinLoseCnt >= 0)
        {
            kWinLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1445), _battleData.Now_WinLoseCnt);
        }
        else
        {
            kWinLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1446), _battleData.Now_WinLoseCnt * -1);
        }


        //kArenaGradeIcoSpr.spriteName = string.Format("ArenaGrade_{0}", )
        kUserArenaSlot.UpdateSlot(false, null, false);
        SetBattleData();
    }

    /// <summary>
    /// 배틀 정보 셋팅
    /// </summary>
    private void SetBattleData()
    {
        kBattleScoreCountLb.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_NOW_TEXT_COLOR), _battleData.SR_BestScore);
        kWinningCountLb.textlocalize = string.Format(FLocalizeString.Instance.GetText(1443), string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR), _battleData.SR_BestWinningStreak));

        kTotalBattleCountLb.textlocalize = GetTotalBattleCount();

        kFirstWinCountLb.textlocalize = string.Format(FLocalizeString.Instance.GetText(1443), string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR), _battleData.SR_FirstWinCnt));
        kSecondWinCountLb.textlocalize = string.Format(FLocalizeString.Instance.GetText(1443), string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR), _battleData.SR_SecondWinCnt));
        kThirdWinCountLb.textlocalize = string.Format(FLocalizeString.Instance.GetText(1443), string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR), _battleData.SR_ThirdWinCnt));
    }

    /// <summary>
    /// 총 전적 셋팅 - x전 x승 x패 (승률 x%)
    /// </summary>
    /// <returns></returns>
    private string GetTotalBattleCount()
    {
        string result = string.Empty;
        int totalBattleCnt = _battleData.SR_TotalCnt;
        int totalWinCnt = _battleData.SR_FirstWinCnt + _battleData.SR_SecondWinCnt + _battleData.SR_ThirdWinCnt;
        int totalLoseCnt = totalBattleCnt - totalWinCnt;

        float winPer = 0;
        if(totalBattleCnt > 0)
        {
            float w = (float)totalWinCnt / (float)totalBattleCnt;
            winPer = w * 100f;
        }

        result = string.Format(FLocalizeString.Instance.GetText(1455), 
            string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_NOW_TEXT_COLOR), totalBattleCnt),
            string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR), totalWinCnt),
            string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.RED_TEXT_COLOR), totalLoseCnt),
            string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_NOW_TEXT_COLOR), winPer.ToString("f1")));
        return result;
    }

    /// <summary>
    /// 아레나 상점
    /// </summary>
	public void OnClick_ArenaShopBtn()
	{
        if (!GameInfo.Instance.IsUserMark(GameInfo.Instance.UserData.UserMarkID))
            GameInfo.Instance.Send_ReqUserMarkList(OnNetUserMarkList);
        else
            LobbyUIManager.Instance.ShowUI("ArenaStorePopup", true);
	}

    public void OnNetUserMarkList(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;
        LobbyUIManager.Instance.ShowUI("ArenaStorePopup", true);
    }

    //캐릭터 선택 CharSeletePopup
    public void OnClick_CharChangeBtn(int slotIdx)
	{
        //선택한 슬롯 인덱스 - 교체할때를 대비하여 저장
        _charSlotSelectIdx = slotIdx;
        
        UIValue.Instance.SetValue(UIValue.EParamType.ArenaTeamCharSlot, (int)_charSlotSelectIdx);
        UIValue.Instance.SetValue(UIValue.EParamType.CharSeletePopupType, (int)eCharSelectFlag.ARENA);
        LobbyUIManager.Instance.ShowUI("CharSeletePopup", true);
    }
    
    /// <summary>
    /// 순위 보기
    /// </summary>
    public void OnClick_ArenaRankingBtn()
	{
        Log.Show("OnClick_ArenaRankingBtn");
        if (GameInfo.Instance.ArenaRankingList.RankingSimpleList.Count <= 0)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3159));
            return;
        }
        LobbyUIManager.Instance.ShowUI("ArenaRankingListPopup", true);
    }

    /// <summary>
    /// 아레나 보상 보기
    /// </summary>
    public void OnClick_ArenaRewardBtn()
	{
        Log.Show("OnClick_ArenaRewardBtn");

        UIArenaRewardListPopup arenaRewardListPopup = LobbyUIManager.Instance.GetUI<UIArenaRewardListPopup>("ArenaRewardListPopup");
        if (arenaRewardListPopup == null)
        {
            return;
        }

        arenaRewardListPopup.SetRewardType(eArenaRewardListPopupType.ArenaReward);
        arenaRewardListPopup.SetUIActive(true);
    }
	
    //
	public void OnClick_ArenaBattleListSlot()
	{
        Log.Show("OnClick_ArenaBattleListSlot");
	}
	
    //아레나 룰
	public void OnClick_explainBtn()
	{
        Log.Show("OnClick_ArenaRuleBtn");
        UIValue.Instance.SetValue(UIValue.EParamType.RulePopupType, (int)UIEventRulePopup.eRulePopupType.ARENA_RULE);
        LobbyUIManager.Instance.ShowUI("EventRulePopup", true);
    }

	public void OnClick_FindBtn() {
		if ( !GameSupport.IsCheckGoods( eGOODSTYPE.GOLD, _arenaGradeTableData.MatchPrice ) || GameSupport.IsEmptyInEquipMainWeapon( ePresetKind.ARENA ) ) {
			return;
		}

		if ( !GameSupport.IsCheckGoods( eGOODSTYPE.BP, GameInfo.Instance.GameConfig.ArenaUseBP, true ) ) {
			MessagePopup.CYN( eTEXTID.TITLE_NOTICE, 3389, eTEXTID.YES, eTEXTID.NO, eGOODSTYPE.GOLD, _arenaGradeTableData.MatchPrice, SendReqArenaEnemySearch, null );
			return;
		}

		SendReqArenaEnemySearch();
	}

	public void OnNet_AckArenaEnemySearch(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        if(GameInfo.Instance.MatchTeam != null)
        {
            LobbyUIManager.Instance.ShowUI("ArenaBattleConfirmPopup", true);
        }

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        Renewal(true);
    }

    public void OnClick_CharTypeChartBtn()
    {
        LobbyUIManager.Instance.ShowUI("CharTypeChartPopup", true);
    }

    public void OnClick_PresetBtn()
    {
        if (GameInfo.Instance.ArenaPresetDatas != null)
        {
            OnNet_PresetList(0, null);
        }
        else
        {
            GameInfo.Instance.Send_ReqGetUserPresetList(ePresetKind.ARENA, 0, OnNet_PresetList);
        }
    }

    public void OnBtnArenaPrologue() {
        int stageid = GameSupport.GetStorySuitableStageID(eSTAGETYPE.STAGE_PVP_PROLOGUE);
        UIValue.Instance.SetValue( UIValue.EParamType.ArenaPrologueStageID, stageid );

        UIValue.Instance.SetValue( UIValue.EParamType.EventStagePopupType, (int)eEventStageType.ARENA_PROLOGUE );
        UIValue.Instance.SetValue( UIValue.EParamType.CardFormationType, eCharSelectFlag.STAGE );
        LobbyUIManager.Instance.SetPanelType( ePANELTYPE.EVENT_STORY_STAGE );
    }

    private void OnNet_PresetList( int result, PktMsgType pktmsg ) {
        if ( result != 0 ) {
            return;
        }

        PktInfoUserPreset pktInfoUserPreset = pktmsg as PktInfoUserPreset;
        if ( pktInfoUserPreset != null && pktInfoUserPreset.infos_.Count <= 0 ) {
            GameInfo.Instance.SetPresetData( ePresetKind.ARENA, -1, GameInfo.Instance.GameConfig.ContPresetSlot );
        }

        UIPresetPopup presetPopup = LobbyUIManager.Instance.GetUI<UIPresetPopup>( "PresetPopup" );
        if ( presetPopup == null ) {
            return;
        }

        presetPopup.SetPresetData( eCharSelectFlag.ARENA, ePresetKind.ARENA );
        presetPopup.SetUIActive( true );
    }

    private void SendReqArenaEnemySearch() {
        if ( !GameSupport.ArenaTeamCheckFlag() ) {
            return;
        }
            
        GameInfo.Instance.Send_ReqArenaEnemySearch( OnNet_AckArenaEnemySearch );
    }
}
