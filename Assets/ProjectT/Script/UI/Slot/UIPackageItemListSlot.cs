using UnityEngine;
using System.Collections;

public class UIPackageItemListSlot : FSlot
{
    public UILabel kNameLabel;
	public UILabel kTextLabel;
    public UISprite kItemBGSpr;
    public UISprite kItemGoodsSpr;
    public UITexture kItemIconTex;
    public UISprite kItemFrameSpr;
    public UISprite kItemGradeSpr;
    public UILabel kItemCountLabel;
    public UISprite kTypeSpr;

    private RewardData _rewardData = null;

    public void UpdateSlot(GameTable.Random.Param data) 	//Fill parameter if you need
	{
        _rewardData = new RewardData(0, data.ProductType, data.ProductIndex, data.ProductValue, false);
        GameSupport.UpdateSlotByRewardData(ParentGO, this.gameObject, _rewardData, kItemFrameSpr, kItemGradeSpr, kTypeSpr, kItemBGSpr, kItemCountLabel, kItemIconTex, kItemGoodsSpr, null);
        kItemCountLabel.gameObject.SetActive(false);
        kItemFrameSpr.spriteName = "itembgSlot_size";

        string typeStr = FLocalizeString.Instance.GetText(14000 + (int)data.ProductType);
        kNameLabel.textlocalize = typeStr;
        FLocalizeString.SetLabel(kTextLabel, GameSupport.GetProductName(_rewardData));
    }
 
	public void OnClick_Slot()
	{
        if (_rewardData == null)
            return;

        if (_rewardData.Type == (int)eREWARDTYPE.GOODS)
            return;

        GameSupport.OpenRewardTableDataInfoPopup(_rewardData);
    }
}
