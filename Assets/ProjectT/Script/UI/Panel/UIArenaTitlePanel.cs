using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIArenaTitlePanel : FComponent
{

	public UILabel kSeasonLabel;
	public UILabel kSeasonLabelshadow;
	public GameObject kDateObj;
	public UILabel kDateLb;
	public GameObject kStartObj;                    //아레나 입장
	public UIButton kArenaTitleBtn;
	public UILabel kStartBtnLb;
	public GameObject kCountingObj;                 //아레나 집계중
	public UIButton kArenaGradeViewBtn;
	public UILabel kArenaGradeLb;
	public UISprite kArenaGradeIcoSpr;
	public UIButton kRankingViewBtn;
	public UILabel kArenaRankingLb;
    [Header("Reward")]
	public GameObject kRewardObj;                   //아레나 집계 후 보상 받기.
	public UILabel kRewardArenaGradeLb;
	public UISprite kRewardArenaGradeIcoSpr;
	public UILabel kRewardArenaRankingLb;
    public List<UIItemListSlot> kRewardItemListSlot;

    public UIGoodsUnit kArenaGoldUnit;

    //보상 테이블
    private List<GameTable.Random.Param> _rewardRandomList = null;

 
	public override void OnEnable()
	{
		_rewardRandomList = null;
		base.OnEnable();
	}

    public override void OnUIOpen()
    {
        LobbyUIManager.Instance.kBlackScene.SetActive(false);
    }

    public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        //아레나입장 가능 체크
        eArenaState arenaState = GameSupport.ArenaPlayFlag();

        kStartObj.SetActive(false);
        kCountingObj.SetActive(false);
        kRewardObj.SetActive(false);
        kDateObj.SetActive(false);

        System.DateTime endDayTime = GameSupport.GetCurrentWeekArenaEndTime();

        kArenaGoldUnit.InitGoodsUnit(eGOODSTYPE.BATTLECOIN, GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.BATTLECOIN]);

        //년 월 일 
        string endDayStr = string.Format(FLocalizeString.Instance.GetText(1452), endDayTime.Year, endDayTime.Month, endDayTime.Day);
        endDayStr = string.Format("{0} ({1}) {2}:{3}", endDayStr, FLocalizeString.Instance.GetText(190 + (int)endDayTime.DayOfWeek), endDayTime.Hour.ToString("D2"), endDayTime.Minute.ToString("D2"));

        if (arenaState == eArenaState.PLAYING || arenaState == eArenaState.REWARD)         //아레나 입장가능 or 보상받기
        {
            bool bRewarded = false;
            
            if(GameInfo.Instance.UserBattleData.Now_RewardDate == default(System.DateTime) ||
                GameInfo.Instance.UserBattleData.Now_RewardDate == GameInfo.Instance.ServerData.ArenaSeasonEndTime)       
            {
                GameSupport.SetArenaBG(kBGIndex, true);
                //받을 보상이 없으면 아레나 입장.
                kSeasonLabel.textlocalize = kSeasonLabelshadow.textlocalize = FLocalizeString.Instance.GetText(1310);
                kStartObj.SetActive(true);
                kDateObj.SetActive(true);

                kDateLb.textlocalize = string.Format(FLocalizeString.Instance.GetText(1399), endDayStr);
            }
            else                        //받을 보상이 있을때
            {
                for (int i = 0; i < kRewardItemListSlot.Count; i++)
                {
                    kRewardItemListSlot[i].gameObject.SetActive(false);
                }

                GameTable.ArenaGrade.Param grade = GameInfo.Instance.GameTable.FindArenaGrade(x => x.GradeID == GameInfo.Instance.UserBattleData.Now_GradeId);

                

                if(GameInfo.Instance.UserBattleData.SR_Rank > 0)
                {
                    //랭커
                    kRewardArenaRankingLb.textlocalize = string.Format(FLocalizeString.Instance.GetText(1448), GameInfo.Instance.UserBattleData.SR_Rank.ToString("#,##0"));
                    List<GameTable.ArenaReward.Param> rewardList = GameInfo.Instance.GameTable.FindAllArenaReward(x => x.RewardType == (int)eArenaRewardType.RANK);
                    if(rewardList != null)
                    {
                        for(int i = 0; i < rewardList.Count; i++)
                        {
                            if(GameInfo.Instance.UserBattleData.SR_Rank <= rewardList[i].RewardValue)
                            {
                                _rewardRandomList = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == rewardList[i].RewardGroupID);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    kRewardArenaRankingLb.textlocalize = string.Format(FLocalizeString.Instance.GetText(1448), "-");
                    GameTable.ArenaReward.Param rewardData = GameInfo.Instance.GameTable.FindArenaReward(x => x.RewardType == (int)eArenaRewardType.GRADE && x.RewardValue == grade.GradeID);
                    if(rewardData != null)
                    {
                        _rewardRandomList = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == rewardData.RewardGroupID);
                    }
                }

                if(_rewardRandomList != null)
                {
                    for(int i = 0; i < _rewardRandomList.Count; i++)
                    {
                        kRewardItemListSlot[i].gameObject.SetActive(true);
                        kRewardItemListSlot[i].ParentGO = this.gameObject;
                        kRewardItemListSlot[i].UpdateSlot(UIItemListSlot.ePosType.ArenaReward, 0, _rewardRandomList[i]);
                    }
                }

                GameSupport.SetArenaBG(kBGIndex, false);

                kRewardArenaGradeIcoSpr.spriteName = grade.Icon;
                kRewardArenaGradeLb.textlocalize = string.Format(FLocalizeString.Instance.GetText(222), GameInfo.Instance.UserBattleData.SR_BestScore);

                kSeasonLabel.textlocalize = kSeasonLabelshadow.textlocalize = FLocalizeString.Instance.GetText(1430);
                kRewardObj.SetActive(true);
            }
        }
        else                    //아레나 집계중 - 입장 불가
        {
            GameSupport.SetArenaBG(kBGIndex, false);
            kSeasonLabel.textlocalize = kSeasonLabelshadow.textlocalize = FLocalizeString.Instance.GetText(1429);
            kCountingObj.SetActive(true);
            kDateObj.SetActive(true);

            GameTable.ArenaGrade.Param grade = GameInfo.Instance.GameTable.FindArenaGrade(x => x.GradeID == GameInfo.Instance.UserBattleData.Now_GradeId);

            kArenaGradeLb.textlocalize = string.Format(FLocalizeString.Instance.GetText(222), GameInfo.Instance.UserBattleData.SR_BestScore);
            kArenaGradeIcoSpr.spriteName = grade.Icon;
            if(GameInfo.Instance.UserBattleData.SR_Rank > 0)
            {
                //kArenaRankingLb.textlocalize = GameInfo.Instance.UserBattleData.SR_Rank.ToString();
                kArenaRankingLb.textlocalize = string.Format(FLocalizeString.Instance.GetText(1448), GameInfo.Instance.UserBattleData.SR_Rank.ToString("#,##0"));
            }
            else
            {
                kArenaRankingLb.textlocalize = string.Format(FLocalizeString.Instance.GetText(1448), "-");
            }
            


            endDayTime = endDayTime.AddHours(GameInfo.Instance.GameConfig.ArenaEndResultHour);
            endDayStr = string.Format(FLocalizeString.Instance.GetText(1452), endDayTime.Year, endDayTime.Month, endDayTime.Day);
            endDayStr = string.Format("{0} ({1}) {2}:{3}", endDayStr, FLocalizeString.Instance.GetText(190 + (int)endDayTime.DayOfWeek), endDayTime.Hour.ToString("D2"), endDayTime.Minute.ToString("D2"));
            kDateLb.textlocalize = string.Format(FLocalizeString.Instance.GetText(1451), endDayStr);
        }
	}
 
	//아레나 입장
	public void OnClick_ArenaTitleBtn()
	{
        Log.Show("OnClick_ArenaTitleBtn");

        GameInfo.Instance.Send_ReqArenaSeasonPlay(OnNet_AckArenaSeasonPlay);
        //LobbyUIManager.Instance.SetPanelType(ePANELTYPE.ARENA_MAIN);
    }
	
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
	
    //
	public void OnClick_ArenaGradeWithRewardListBtn()
	{
        Log.Show("OnClick_ArenaGradeWithRewardListBtn");

        UIArenaRewardListPopup arenaRewardListPopup = LobbyUIManager.Instance.GetUI<UIArenaRewardListPopup>("ArenaRewardListPopup");
        if (arenaRewardListPopup == null)
        {
            return;
        }

        arenaRewardListPopup.SetRewardType(eArenaRewardListPopupType.ArenaReward);
        arenaRewardListPopup.SetUIActive(true);
    }

    public void OnClick_ArenaGradeViewBtn()
	{
        Log.Show("OnClick_ArenaGradeViewBtn");
    }
	
    //
	public void OnClick_RankingViewBtn()
	{
        Log.Show("OnClick_RankingViewBtn");
        
    }
	
	public void OnClick_RewardArenaViewBtn()
	{
        Log.Show("OnClick_RewardArenaViewBtn");
    }
	
	public void OnClick_RewardArenaRankingViewBtn()
	{
        Log.Show("OnClick_RewardArenaRankingViewBtn");
    }
	
    /// <summary>
    /// 보상 목록
    /// </summary>
	public void OnClick_ArenaRewardBtn()
	{
        Log.Show("OnClick_ArenaRewardBtn");
        GameInfo.Instance.Send_ReqArenaSeasonPlay(OnNet_AckArenaSeasonPlay);
        //OnNet_AckArenaSeasonPlay(0, null);
    }
	
    /// <summary>
    /// 아레나 툴팁
    /// </summary>
	public void OnClick_ArenaRuleBtn()
	{
        Log.Show("OnClick_ArenaRuleBtn");
        UIValue.Instance.SetValue(UIValue.EParamType.RulePopupType, (int)UIEventRulePopup.eRulePopupType.ARENA_RULE);
        LobbyUIManager.Instance.ShowUI("EventRulePopup", true);
    }

    /// <summary>
    /// 아레나 상점
    /// </summary>
    public void OnClick_ArenaStoreBtn()
    {
        Log.Show("OnClick_ArenaStoreBtn");
        UITopPanel toppanel = LobbyUIManager.Instance.GetUI<UITopPanel>("TopPanel");
        if(toppanel != null)
        {
            toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.NORMAL);
            toppanel.Renewal(true);
        }

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

    /// <summary>
    /// 아레나 시즌 참가 & 보상 받기
    /// </summary>
    /// <param name="result"></param>
    /// <param name="pktMsg"></param>
    public void OnNet_AckArenaSeasonPlay(int result, PktMsgType pktMsg)
    {
        if (result != 0)
            return;

        if(_rewardRandomList != null)
        {
            List<RewardData> rewardList = new List<RewardData>();
            for(int i = 0; i < _rewardRandomList.Count; i++)
            {
                rewardList.Add(new RewardData(_rewardRandomList[i].ProductType, _rewardRandomList[i].ProductIndex, _rewardRandomList[i].ProductValue));
            }
            MessageRewardListPopup.RewardListMessage(FLocalizeString.Instance.GetText(1499), FLocalizeString.Instance.GetText((int)eTEXTID.REWARD_POPUP_TEXT_MAIL), rewardList, OnRewardCheck);
        }
        else
        {
            LobbyUIManager.Instance.SetPanelType(ePANELTYPE.ARENA_MAIN);
        }
    }

    public void OnRewardCheck()
    {
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.ARENA_MAIN);
    }
}
