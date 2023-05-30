using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIArenaPanel : FComponent
{

	public UISprite kProlobueSpr;
    public UIGaugeUnit kPrologueGaugeUnit;
	public UILabel kPrologueGaugeLabel;
    public GameObject kArenaLockObj;
    public UISprite kArenaSpr;
    
    public UILabel kArenaOpenToolTipLb;

    public UIButton kArenaJoinBtn;

    public GameObject kArenaTowerRoot;
    public GameObject ArenaTowerLockObj;

    private float _fillAmountGauge = 0f;

 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        //#if !PVPMODE
        //        kArenaJoinBtn.isEnabled = false;
        //#endif
        kArenaTowerRoot.SetActive(false);
        kArenaTowerRoot.SetActive(true);
    }

    public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        //프롤로그 게이지 - 100보다 크면 데이터 수집중UI off
        //if(GameInfo.Instance.GameConfig.TestMode || AppMgr.Instance.Review) 아레나 진입 조건 없어짐
            GameInfo.Instance.UserData.ArenaPrologueValue = 100;

        ArenaTowerLockObj.SetActive(GameSupport.IsLockArenaTower());

        SetPrologueGauge();

        if(GameInfo.Instance.UserData.ArenaPrologueValue >= 100)
        {
            kPrologueGaugeLabel.gameObject.SetActive(false);
            kPrologueGaugeUnit.gameObject.SetActive(false);
            kArenaOpenToolTipLb.gameObject.SetActive(false);
            kArenaLockObj.SetActive(false);
        }
        else
        {
            kPrologueGaugeLabel.gameObject.SetActive(true);
            kPrologueGaugeUnit.gameObject.SetActive(true);
            kArenaOpenToolTipLb.gameObject.SetActive(true);
            kArenaLockObj.SetActive(true);
        }
    }

    private void SetPrologueGauge()
    {
        kPrologueGaugeUnit.InitGaugeUnit(GameInfo.Instance.UserData.ArenaPrologueValue * 0.01f);
        kPrologueGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT), (GameInfo.Instance.UserData.ArenaPrologueValue)));
    }
	
	public void OnClick_PrologueStageBtn()
	{   
        int stageid = GameSupport.GetStorySuitableStageID(eSTAGETYPE.STAGE_PVP_PROLOGUE);
        UIValue.Instance.SetValue(UIValue.EParamType.ArenaPrologueStageID, stageid);

        UIValue.Instance.SetValue(UIValue.EParamType.EventStagePopupType, (int)eEventStageType.ARENA_PROLOGUE);
        UIValue.Instance.SetValue(UIValue.EParamType.CardFormationType, eCharSelectFlag.STAGE);
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.EVENT_STORY_STAGE);
    }
	
	public void OnClick_BattleArenaBtn()
	{
        if(GameInfo.Instance.UserData.ArenaPrologueValue < 100)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(1456));
            return;
        }
        UIValue.Instance.SetValue(UIValue.EParamType.CardFormationType, eCharSelectFlag.ARENA);
        GameInfo.Instance.Send_ReqArenaRankingList(GameInfo.Instance.ArenaRankingList.UpdateTM, OnNetArenaRankingList);
    }
    
    public void OnClick_BattleTower()
    {
        if (GameInfo.Instance.UserData.ArenaPrologueValue < 100)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(1456));
            return;
        }

        if (GameSupport.IsLockArenaTower())
            return;

        UIValue.Instance.SetValue(UIValue.EParamType.CardFormationType, eCharSelectFlag.ARENATOWER);

        //친구 리스트 요청 후 아레나 타워 열기
        List<ulong> reqUUID = new List<ulong>();
        var iter = GameInfo.Instance.CommunityData.FriendList.GetEnumerator();
        while (iter.MoveNext())
        {
            if (iter.Current.HasArenaInfo)
            {
                TeamData teamdata = GameInfo.Instance.TowerFriendTeamData.Find(x => x.UUID == iter.Current.UUID);
                if (teamdata == null)
                    reqUUID.Add((ulong)iter.Current.UUID);
            }
        }

        //친구 캐릭터 설정 초기화
        GameInfo.Instance.ArenaTowerFriendContainer.Clear();

        if (reqUUID.Count == 0)
        {   
            LobbyUIManager.Instance.SetPanelType(ePANELTYPE.ARENATOWER);
            return;
        }

        GameInfo.Instance.Send_ReqCommunityUserArenaInfoGet(GameInfo.eCommunityUserInfoGetType.ARENATOWER, reqUUID,
            (int result, PktMsgType pktMsg) =>
            {
                if (result != 0) { return; }
                LobbyUIManager.Instance.SetPanelType(ePANELTYPE.ARENATOWER);
            });
    }

    public void OnNetArenaRankingList(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        if(GameSupport.ArenaPlayFlag() == eArenaState.PLAYING)
        {
            LobbyUIManager.Instance.SetPanelType(ePANELTYPE.ARENA_MAIN);
        }
        else
        {
            LobbyUIManager.Instance.SetPanelType(ePANELTYPE.ARENA_TITLE);
        }
    }

    public void OnBtnRaid() {
        if( GameInfo.Instance.CharList.Count < 3 ) {
            MessagePopup.OK( eTEXTID.OK, FLocalizeString.Instance.GetText( 3336 ), null );
            return;
		}

        if( GameSupport.IsRaidEnd() ) {
            MessagePopup.OK( eTEXTID.OK, 3332, null );
            return;
		}

        if( GameInfo.Instance.RaidUserData.NeedInitSeasonData() ) {
            GameInfo.Instance.Send_ReqInitRaidSeasonData( OnNetInitRaidSeasonData );
        }
        else {
            LobbyUIManager.Instance.SetPanelType( ePANELTYPE.RAID_MAIN );
        }
    }

    private void OnNetInitRaidSeasonData( int result, PktMsgType pkt ) {
        if( result != 0 ) {
            return;
		}

        LobbyUIManager.Instance.SetPanelType( ePANELTYPE.RAID_MAIN );
    }
}
