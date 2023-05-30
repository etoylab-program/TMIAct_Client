using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIPvpAllRewardSlot : FSlot
{
    [Header("Center Objs")]
    public GameObject kCenterObj;
    public UISprite kDekoSpr;
    public UISprite kicoSpr;
    public UILabel kPvpGradeLabel;
    public UILabel kPvpRankingLabel;
    public List<UIUserCharListSlot> kCharSlotList;
    public List<UIBatchListSlotSlot> kBadgeSlotList;
    public UITexture kUserMark;
    public UILabel kUserName;
    public UILabel kUserLv;
    public UILabel kUserTeamPower;

    [Header("Top Objs")]
    public GameObject kTopObj;
    public UISprite kTopDekoSpr;
    public UISprite kTopicoSpr;
    public UILabel kPvpTopGradeLabel;
    public UILabel kPvpTopRankingLabel;
    public List<UIUserCharListSlot> kTopCharSlotList;
    public List<UIBatchListSlotSlot> kTopBadgeSlotList;
    public UITexture kTopUserMark;
    public UILabel kTopUserName;
    public UILabel kTopUserLv;
    public UILabel kTopUserTeamPower;

    public GameObject kMe;

    private TeamData _teamData;
    private GameTable.ArenaGrade.Param _gradeData;

    public void UpdateSlot(int index, TeamData data) 	//Fill parameter if you need
	{
        _teamData = data;

        int gradeID = GameSupport.GetArenaGradeWithNowPoint((int)_teamData.Score, eArenaGradeFlag.GRADE_ID);
        _gradeData = GameInfo.Instance.GameTable.FindArenaGrade(x => x.GradeID == gradeID);

        kMe.SetActive(false);
        kCenterObj.SetActive(false);
        kTopObj.SetActive(false);

        if (data.Rank == 1)
        {
            kTopObj.SetActive(true);
            SerRankingUser(kTopUserMark, kTopUserName, kTopUserLv, kTopUserTeamPower, kTopicoSpr, kPvpTopGradeLabel, kPvpTopRankingLabel, kTopCharSlotList, kTopBadgeSlotList);
        }
        else
        {
            kCenterObj.SetActive(true);
            SerRankingUser(kUserMark, kUserName, kUserLv, kUserTeamPower, kicoSpr, kPvpGradeLabel, kPvpRankingLabel, kCharSlotList, kBadgeSlotList);
        }

        if (_teamData.UUID == GameInfo.Instance.UserData.UUID)
            kMe.SetActive(true);
	}

    private void SerRankingUser(UITexture userMark, UILabel userName, UILabel userLv, UILabel userTeamPower, UISprite gradeSpr, UILabel gradeLabel, UILabel rankingLabel, List<UIUserCharListSlot> charListSlot, List<UIBatchListSlotSlot> badgeListSlot)
    {
        LobbyUIManager.Instance.GetUserMarkIcon(ParentGO, this.gameObject, _teamData.UserMark, ref userMark);

        userName.textlocalize = _teamData.GetUserNickName();
        userLv.textlocalize = _teamData.UserLv.ToString();
        userTeamPower.textlocalize = _teamData.TeamPower.ToString("#,##0");
        gradeSpr.spriteName = _gradeData.Icon;
        //gradeLabel.textlocalize = FLocalizeString.Instance.GetText(_gradeData.Name);
        gradeLabel.textlocalize = _teamData.Score.ToString("#,##0");
        //rankingLabel.textlocalize = string.Format("{0}{1}", _teamData.Rank, FLocalizeString.Instance.GetText(1448));
        rankingLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1448), _teamData.Rank);

        //전부 다 비활성화 후 적용
        for (int i = (int)eArenaTeamSlotPos.START_POS; i < (int)eArenaTeamSlotPos._MAX_; i++)
        {
            charListSlot[i].UpdateArenaTeamSlot(i, null);
        }

        for (int i = (int)eArenaTeamSlotPos.START_POS; i < (int)eArenaTeamSlotPos._MAX_; i++)
        {
            charListSlot[i].ParentGO = this.gameObject;
            charListSlot[i].UpdateArenaTeamSlot(i, _teamData.charlist[i].CharData, false, true);
            //charListSlot[i].UpdateSlot(_teamData.charlist[i].CharData);
        }

        int mainOptID = 0;

        //BadgeData mainBadgeData = _matchTeam.badgelist.Find(x => x.PosSlotNum == ((int)eBadgeSlot.FIRST - 1) && x.PosKind == (int)eContentsPosKind.ARENA);
        //if (mainBadgeData != null)
        //{
        //    mainOptID = mainBadgeData.OptID[(int)eBadgeOptSlot.FIRST];
        //}

        for (int i = 0; i < badgeListSlot.Count; i++)
        {
            int slotIdx = i;
            badgeListSlot[i].ParentGO = this.gameObject;
            //BadgeData badgedata = _teamData.badgelist.Find(x => x.PosSlotNum == slotIdx && x.PosKind == (int)eContentsPosKind.ARENA);
            BadgeData badgedata = _teamData.badgelist[i];
            if (badgedata != null && badgedata.OptID[(int)eBadgeOptSlot.FIRST] != (int)eCOUNT.NONE)
            {
                badgedata.PosSlotNum = i;
                badgedata.PosKind = (int)eContentsPosKind.ARENA;
                badgeListSlot[i].UpdateSlot(slotIdx + 1, mainOptID, badgedata, UIBatchListSlotSlot.eBadgeSlotType.Info, eContentsPosKind.ARENA);
            }
            else
            {
                badgeListSlot[i].UpdateSlot(slotIdx + 1, mainOptID, null, UIBatchListSlotSlot.eBadgeSlotType.Info, eContentsPosKind.ARENA);
            }
        }
    }

    public void OnShowBadgeToolTip()
    {
        BadgeToolTipPopup.Show(_teamData.badgelist, this.gameObject, BadgeToolTipPopup.eToolTipDir.RIGHT, eCharSelectFlag.ARENA, eContentsPosKind.ARENA);
    }

    public void OnClick_Slot()
	{
        //Log.Show(_teamData.UserNickName + " / " + _teamData.Rank);
	}
 

	
	public void OnClick_UserCharListSlot00()
	{
	}
	
	public void OnClick_UserCharListSlot01()
	{
	}
	
	public void OnClick_UserCharListSlot02()
	{
	}
	
	public void OnClick_BatchListSlotSlot00()
	{
        //
        //
    }
	
	public void OnClick_BatchListSlotSlot01()
	{
	}
	
	public void OnClick_BatchListSlotSlot02()
	{
	}
	
	public void OnClick_TopBatchListSlotSlot00()
	{
	}
	
	public void OnClick_TopBatchListSlotSlot01()
	{
	}
	
	public void OnClick_TopBatchListSlotSlot02()
	{
	}

    public void RankerDetialUUID_Check(int slotIdx)
    {
        if (_teamData == null)
            return;

        UIValue.Instance.SetValue(UIValue.EParamType.RankUserType, (int)eRankUserType.ARENA);
        UIValue.Instance.SetValue(UIValue.EParamType.ArenaTeamCharSlot, slotIdx);
        if (GameInfo.Instance.ArenaRankerDetialData == null)
        {
            GameInfo.Instance.Send_ReqArenaRankerDetail(_teamData.UUID, OnNetAckArenaRankerDetail);
        }
        else
        {
            if(GameInfo.Instance.ArenaRankerDetialData.UUID.Equals(_teamData.UUID))
                LobbyUIManager.Instance.ShowUI("UserCharDetailPopup", true);
            else
                GameInfo.Instance.Send_ReqArenaRankerDetail(_teamData.UUID, OnNetAckArenaRankerDetail);
        }
    }

    public void OnNetAckArenaRankerDetail(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        LobbyUIManager.Instance.ShowUI("UserCharDetailPopup", true);
    }
}
