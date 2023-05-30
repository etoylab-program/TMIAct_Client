using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageRewardSpecialPopup
{
    public static void RewardSpecialMessage(string Title, string Msg, RewardData reward)
    {
        UIRewardSpecialPopup rewardListPopup = LobbyUIManager.Instance.ShowUI("RewardSpecialPopup", true) as UIRewardSpecialPopup;
        if (rewardListPopup != null)
        {
            rewardListPopup.InitPopup(Title, Msg, reward);
        }
    }
}

public class UIRewardSpecialPopup : FComponent
{

	public UILabel kTitleLabel;
	public UIButton kCloseBtn;
	public UILabel kTextLabel;
	public UIRewardListSlot kRewardListSlot;
 
 
    public void InitPopup(string Title, string Msg, RewardData reward)
    {
        //m_rewardData.Clear();
        //m_rewardData = rewards;

        kTitleLabel.textlocalize = Title;
        kTextLabel.textlocalize = Msg;

        kRewardListSlot.UpdateSlot(reward, false);
    }

    public void OnClick_CloseBtn()
    {
        OnClickClose();
    }

    public override void OnClickClose()
    {
        if (IsPlayAnimtion())
            return;

        GameInfo.Instance.RewardList.Clear();

        LobbyUIManager.Instance.HideUI("RewardSpecialPopup", false);

        //base.OnClickClose();
    }
}
