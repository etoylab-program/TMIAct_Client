using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIInfluenceMissionListSlot : FSlot 
{
	[SerializeField] private UIRewardListSlot RewardSlot;
	[SerializeField] private UILabel TitleLabel;
	[SerializeField] private UILabel DescLabel;
	[SerializeField] private UIButton ReceBtn;
	[SerializeField] private GameObject CompleteObj;
    [SerializeField] private UILabel CountLabel;

    private GameTable.InfluenceMission.Param param;
    private uint ServerCount = 0;
    private byte ChoiceBit = 0;

    private RewardData RewardInfo;
    public void UpdateSlot(GameTable.InfluenceMission.Param _param, uint _serverCount, byte _choieBit) 	//Fill parameter if you need
	{
		SetComponents(false);
        ServerCount = _serverCount;
        ChoiceBit = _choieBit;
        param = _param;
		if (param == null) return;

        bool state = true;
        RewardSlot.SetActive(state);
        TitleLabel.SetActive(state);
        DescLabel.SetActive(state);        

        var dayReward = GameInfo.Instance.GameTable.FindRandom(param.RewardGroupID);
        RewardInfo = new RewardData(dayReward.ProductType,
                                        dayReward.ProductIndex,
                                        dayReward.ProductValue);
        RewardSlot.UpdateSlot(RewardInfo, true);

        
        eMISSIONTYPE missionType = (eMISSIONTYPE)GameSupport.CompareMissionString(param.MissionType);        
        TitleLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(param.Desc), param.MissionValue);        
        DescLabel.textlocalize = GameSupport.GetProductName(RewardInfo);

                
        if (ServerCount <= 0)
        {
            // »πµÊ ∞°¥… or »πµÊ øœ∑·
            if(GameSupport.IsComplateMissionRecive(GameInfo.Instance.InfluenceMissionData.RwdFlag, ChoiceBit) == true)
            {
                //»πµÊ»Ø∑·
                CompleteObj.SetActive(state);
            }
            else
            {
                //»πµÊ∞°¥…
                ReceBtn.SetActive(state);
            }
        }
        else
        {
            // πÃº« ¡¯«‡¡ﬂ
            CountLabel.SetActive(state);
            //πÃº«»Ωºˆ        
            FLocalizeString.SetLabel(CountLabel, 218, param.MissionValue - _serverCount, param.MissionValue);
        }

    }

	private void SetComponents(bool state)
    {
        RewardSlot.SetActive(state);
        TitleLabel.SetActive(state);
        DescLabel.SetActive(state);
        ReceBtn.SetActive(state);
        CompleteObj.SetActive(state);
        CountLabel.SetActive(state);
    }

    public void OnClick_ReceiveReward()
    {
        if (ServerCount > 0) return;
        if (GameSupport.IsComplateMissionRecive(GameInfo.Instance.InfluenceMissionData.RwdFlag, ChoiceBit)) return;

        //≥Ø¬•¡ˆ≥µ¿∏∏È ºº∑¬¿¸ ∆Àæ˜ off «œ∞Ì ∑Œ±◊¿Œ ∫∏≥ Ω∫ πﬁ±‚!
        if (GameSupport.IsPossibleLoginBonus())
        {
            MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3247), () =>
            {
                LobbyUIManager.Instance.HideUI("InfluenceMainPopup");
            }
            );
            return;
        }
        List<byte> datas = new List<byte>();
        datas.Add(ChoiceBit);
        GameInfo.Instance.Send_ReqRewardInfluMission(datas, OnNetAckRewardInfluMission);
    }


    private void OnNetAckRewardInfluMission(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        //æ∆¿Ã≈€ »πµÊ ø¨√‚
        string title = FLocalizeString.Instance.GetText(1433);
        string desc = FLocalizeString.Instance.GetText(1262);

        List<RewardData> RewardList = new List<RewardData>();
        RewardList.Add(RewardInfo); 

        MessageRewardListPopup.RewardListMessage(title, desc, RewardList, null);

        LobbyUIManager.Instance.Renewal("InfluenceMainPopup");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.Renewal("TopPanel");

    }
}
