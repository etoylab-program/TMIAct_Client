using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIInfluenceRewardListSlot : FSlot
{
	[SerializeField] private UILabel kLeftLabel;
	[SerializeField] private UIItemListSlot[] kItemListSlots;

	[SerializeField] private GameObject kMeObj;
	[SerializeField] private GameObject kReceiveObjObj;
	[SerializeField] private UISprite kReceiveBGSpr;

	GameTable.InfluenceRank.Param Param;
	int StartRank = 0;
	int EndRank = 0;
	
	List<GameTable.Random.Param> RandomTable = new List<GameTable.Random.Param>();
	public void UpdateData(GameTable.InfluenceRank.Param _param, int _startRank, bool _isReceivePeriod)
	{
		SetState(false);

		Param = _param;		

		if (Param == null) return;

		kLeftLabel.SetActive(true);

		string strRank = string.Empty;
        
		StartRank = _startRank;
        EndRank = Param.RewardValue;

        if (Param.RewardType == 1)
		{
            // 절대 순위
            if (StartRank + 1 == EndRank)
            {
                strRank = string.Format(FLocalizeString.Instance.GetText(1053), EndRank);
            }
            else
            {
                strRank = string.Format(FLocalizeString.Instance.GetText(1053), StartRank+1) + "~" + string.Format(FLocalizeString.Instance.GetText(1053), EndRank);
            }

			
		}
		else if (Param.RewardType == 2)
		{
			// 백분율
			strRank = string.Format("{0}%", Param.RewardValue);
		}
		kLeftLabel.textlocalize = strRank;


		RandomTable = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == Param.RewardGroupID);
		if (RandomTable == null || RandomTable.Count == 0) return;

		// 위치 초기화
		Vector3 localPos = new Vector3(20, 4, 0);
        for (int i = 0; i < kItemListSlots.Length; i++)
        {
			kItemListSlots[i].transform.localPosition = localPos;
			localPos.x += 80;
		}

        if (RandomTable.Count == 1)
        {
            kItemListSlots[1].SetActive(true);
            kItemListSlots[1].UpdateSlot(UIItemListSlot.ePosType.ArenaReward, 0, RandomTable[0]);
        }
		else if (RandomTable.Count == 2)
        {
            kItemListSlots[0].SetActive(true);
            kItemListSlots[0].UpdateSlot(UIItemListSlot.ePosType.ArenaReward, 0, RandomTable[0]);

            kItemListSlots[1].SetActive(true);
            kItemListSlots[1].UpdateSlot(UIItemListSlot.ePosType.ArenaReward, 1, RandomTable[1]);

			kItemListSlots[0].transform.localPosition = new Vector3(60, 4, 0);
			kItemListSlots[1].transform.localPosition = new Vector3(140, 4, 0);
		}
		else if (RandomTable.Count == 3)
        {
            for (int i = 0; i < RandomTable.Count; i++)
            {
                kItemListSlots[i].SetActive(true);
                kItemListSlots[i].UpdateSlot(UIItemListSlot.ePosType.ArenaReward, i, RandomTable[i]);
            }
        }

		//MeObj	
		kMeObj.SetActive(GameInfo.Instance.InfluenceRankData.IdInRankTable == Param.RewardOrder);

		//Receive Button
		if (_isReceivePeriod && GameInfo.Instance.InfluenceRankData.IdInRankTable == Param.RewardOrder)
		{
			kReceiveObjObj.SetActive(true);
			if (!GameSupport.IsComplateMissionRecive(GameInfo.Instance.InfluenceMissionData.TgtRwdFlag, (byte)PktInfoMission.Influ.TARGET._RANK_START_))
            {
				kReceiveBGSpr.spriteName = "btn_Base_Yellow_50px";
			}
			else
            {
				kReceiveBGSpr.spriteName = "btn_Base_dis";
			}
		}
	}

	public void OnClick_Receive()
    {
		if (GameSupport.IsComplateMissionRecive(GameInfo.Instance.InfluenceMissionData.TgtRwdFlag, (byte)PktInfoMission.Influ.TARGET._RANK_START_))
			return;

        List<byte> list = new List<byte>();
        list.Add((byte)PktInfoMission.Influ.TARGET._RANK_START_);
        GameInfo.Instance.Send_ReqInfluenceTgtRwd(list, (byte)eRWD_TP.RANK, OnNetAckInfluenceTgtRwd);
	}

	private void OnNetAckInfluenceTgtRwd(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

		if (RandomTable.Count > 0)
		{
			List<RewardData> RewardList = new List<RewardData>();
			for (int i = 0; i < RandomTable.Count; i++)
			{
				RewardData data = new RewardData(RandomTable[i].ProductType, RandomTable[i].ProductIndex, RandomTable[i].ProductValue);
				RewardList.Add(data);
			}

			string title = FLocalizeString.Instance.GetText(1433);
            string desc = FLocalizeString.Instance.GetText(1262);

            MessageRewardListPopup.RewardListMessage(title, desc, RewardList, null);
        }

		LobbyUIManager.Instance.Renewal("InfluenceRewardPopup");
		LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.Renewal("TopPanel");
    }
	private void SetState(bool state)
    {
		kLeftLabel.SetActive(state);
		if (kItemListSlots != null)
		{
			for (int i = 0; i < kItemListSlots.Length; i++)
			{
				kItemListSlots[i].SetActive(state);
			}
		}
		kMeObj.SetActive(state);
		kReceiveObjObj.SetActive(state);
    }
}
