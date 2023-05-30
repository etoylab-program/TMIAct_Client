using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBingoRewardSlot : FSlot
{
    [SerializeField] private GameObject normalObj = null;
    [SerializeField] private GameObject rareObj = null;
    [SerializeField] private UIItemListSlot itemListSlot = null;
    [SerializeField] private UILabel nameLabel = null;
    [SerializeField] private UILabel descLabel = null;

    [SerializeField] private GameObject countObj = null;
    [SerializeField] private UILabel countLabel = null;
    [SerializeField] private UIButton recvBtn = null;
    [SerializeField] private GameObject completeObj = null;

    private GameTable.Random.Param _randomParam;
    public void UpdateSlot(int index, GameTable.Random.Param randomParam, bool isComplete, bool isReceive, int receiveCount)
    {
        if (randomParam == null)
        {
            return;
        }

        _randomParam = randomParam;

        normalObj.SetActive(randomParam.Value != 0);
        rareObj.SetActive(randomParam.Value == 0);

        RewardData rewardData = new RewardData(randomParam.ProductType, randomParam.ProductIndex, randomParam.ProductValue);

        itemListSlot.UpdateSlot(UIItemListSlot.ePosType.RewardDataInfo, 0, rewardData);

        if (randomParam.Value == 0)
        {
            nameLabel.textlocalize = FLocalizeString.Instance.GetText(3288);
        }
        else
        {
            nameLabel.textlocalize = FLocalizeString.Instance.GetText(3289, randomParam.Value);
        }

        descLabel.textlocalize = GameSupport.GetProductName(rewardData);

        completeObj.SetActive(isComplete);
        recvBtn.SetActive(!isComplete && isReceive);

        countObj.SetActive(!isComplete && !isReceive && randomParam.Value != 0);
        if (countObj.activeSelf)
        {
            countLabel.textlocalize = FLocalizeString.Instance.GetText(218, receiveCount, randomParam.Value);
        }
    }

    public void OnClick_RecvBtn()
    {
        if (ParentGO == null)
        {
            return;
        }

        UIBingoEvent bingoEventPanel = ParentGO.GetComponent<UIBingoEvent>();
        if (bingoEventPanel == null)
        {
            return;
        }

        bingoEventPanel.RewardRecvSlot(_randomParam.Value);
    }
}
