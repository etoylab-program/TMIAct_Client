using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIArenaGradeRewardSlot : FSlot
{
    public enum ArenaRewardType
    {
        Ranking = 1,
        Grade = 2,
    }

    public List<UIItemListSlot> kItemSlot;

    [Header("Grade")]
    public GameObject                       kArenaGrade;
    public UILabel                          kGradeLabel;
    public UISprite                         kGradeSpr;
    public UILabel                          kGradeScoreLabel;
    [Header("Ranking")]
    public GameObject                       kArenaRanking;
    public UILabel                          kArenaRankingLabel;

    public GameObject                       kMe;

    [Header("Circle")]
    public Animation stampAni;
    public UISprite stampSpr;
    public GameObject completeObj;
    
    private GameTable.ArenaReward.Param     _arenaRewardData;
    public GameTable.ArenaReward.Param ArenaRewardData { get { return _arenaRewardData; } }

    public void UpdateSlot(GameTable.ArenaReward.Param data, int prevRewardValue, bool bUser = false) 	//Fill parameter if you need
	{
        _arenaRewardData = data;

        for(int i = 0; i < kItemSlot.Count; i++)
        {
            kItemSlot[i].ParentGO = this.gameObject;
            kItemSlot[i].gameObject.SetActive(false);
        }

        kMe.SetActive(bUser);

        kArenaGrade.SetActive(false);
        kArenaRanking.SetActive(false);

        stampSpr.SetActive(false);
        completeObj.SetActive(false);
        stampAni.enabled = false;

        if (_arenaRewardData.RewardType == (int)ArenaRewardType.Ranking)
        {
            kArenaRanking.SetActive(true);
            if(_arenaRewardData.RewardValue == 1)       //1위
            {
                //kArenaRankingLabel.textlocalize = string.Format("{0}{1}", _arenaRewardData.RewardValue, FLocalizeString.Instance.GetText(1448));
                kArenaRankingLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1448), _arenaRewardData.RewardValue);
            }
            else
            {
                int startRanking = prevRewardValue + 1;     //전단계 +1 해줘야 정상적인 값
                //~ 코드 String에서 가지고 오는 방시긍로 변경할것 0323
                //kArenaRankingLabel.textlocalize = string.Format("{0}{1} ~ {2}{3}", startRanking, FLocalizeString.Instance.GetText(1448), _arenaRewardData.RewardValue, FLocalizeString.Instance.GetText(1448));
                kArenaRankingLabel.textlocalize = string.Format("{0} ~ {1}", string.Format(FLocalizeString.Instance.GetText(1448), startRanking), string.Format(FLocalizeString.Instance.GetText(1448), _arenaRewardData.RewardValue));
            }
        }
        else if(_arenaRewardData.RewardType == (int)ArenaRewardType.Grade)
        {
            kArenaGrade.SetActive(true);

            GameTable.ArenaGrade.Param gradeData = GameInfo.Instance.GameTable.FindArenaGrade(x => x.GradeID == _arenaRewardData.RewardValue);
            if(gradeData == null)
            {
                return;
            }

            kGradeLabel.textlocalize = FLocalizeString.Instance.GetText(gradeData.Name);
            kGradeSpr.spriteName = gradeData.Icon;
            kGradeScoreLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1493), gradeData.ReqScore);
        }

        List<GameTable.Random.Param> rewardData = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == _arenaRewardData.RewardGroupID);

        //최대 3개 혹시 몰라서 강제
        int cnt = rewardData.Count;
        if (cnt > 3)
            cnt = 3;

        for(int i = 0; i < cnt; i++)
        {
            kItemSlot[i].gameObject.SetActive(true);
            kItemSlot[i].UpdateSlot(UIItemListSlot.ePosType.ArenaReward, 0, rewardData[i], false);
        }
	}

    public void UpdateSlot(GameTable.CircleCheck.Param circleCheckParam, bool isPlayAnimation)
    {
        if (circleCheckParam == null)
        {
            return;
        }

        kArenaGrade.SetActive(false);
        kArenaRankingLabel.textlocalize = $"{circleCheckParam.CircleCheckCnt} 일차"; // Test - LeeSeungJin - Change String

        List<GameTable.Random.Param> randomParamList = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == circleCheckParam.RewardGroupID);
        for (int i = 0; i < kItemSlot.Count; i++)
        {
            if (kItemSlot[i].ParentGO == null)
            {
                kItemSlot[i].ParentGO = this.gameObject;
            }

            bool isActive = 0 <= i && i < randomParamList.Count;
            if (isActive)
            {
                RewardData rewardData = new RewardData(randomParamList[i].ProductType, randomParamList[i].ProductIndex, randomParamList[i].ProductValue);
                kItemSlot[i].UpdateSlot(UIItemListSlot.ePosType.RewardDataInfo, i, rewardData);
            }            
            kItemSlot[i].gameObject.SetActive(isActive);
        }

        bool isComplete = circleCheckParam.CircleCheckCnt <= GameInfo.Instance.UserData.CircleAttendance.RewardCount;
        stampSpr.SetActive(isComplete);
        completeObj.SetActive(isComplete);
        stampAni.enabled = isPlayAnimation && circleCheckParam.CircleCheckCnt == GameInfo.Instance.UserData.CircleAttendance.RewardCount;
    }
 
	public void OnClick_Slot()
	{
	}
 
	public void OnClick_ItemListSlot00()
	{
	}
	
	public void OnClick_ItemListSlot01()
	{
	}
	
	public void OnClick_ItemListSlot02()
	{
	}
}
