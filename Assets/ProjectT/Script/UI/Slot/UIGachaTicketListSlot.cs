using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIGachaTicketListSlot : FSlot
{
    [System.Serializable]
    public class GachaTicketButton
    {
        public UILabel kNoLabel;
        public UILabel kTextLabel;
        public UISprite kDisabledSpr;
    }

    public UITexture kTicketTex;
    public UISprite kBasicSpr;
    public UILabel kGachaTicketDescLabel;
    public UILabel kGoodsTextLabel;
    public List<GachaTicketButton> kGachaTicketBtnList;

    private int _index;
    private List<GameClientTable.StoreDisplayGoods.Param> _datalist = new List<GameClientTable.StoreDisplayGoods.Param>();
    
    public void UpdateSlot(int index, GameClientTable.StoreDisplayGoods.Param data1, GameClientTable.StoreDisplayGoods.Param data2) 	//Fill parameter if you need
	{
        _index = index;
        _datalist.Clear();
        _datalist.Add(data1);
        _datalist.Add(data2);

        kBasicSpr.spriteName = data1.Icon;
        kGachaTicketDescLabel.textlocalize = FLocalizeString.Instance.GetText(data1.Description);
        kGoodsTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(228), 0);

        for (int i = 0; i < kGachaTicketBtnList.Count; i++)
            InitBtn(i);
    }
 
    public void InitBtn(int index)
    {
        var storetable = GameInfo.Instance.GameTable.FindStore(_datalist[index].StoreID);
        if (storetable == null)
            return;

        kGachaTicketBtnList[index].kNoLabel.textlocalize = storetable.ProductValue.ToString();
        kGachaTicketBtnList[index].kTextLabel.textlocalize = FLocalizeString.Instance.GetText(1087);
        kGachaTicketBtnList[index].kDisabledSpr.gameObject.SetActive(true);

        if (storetable.PurchaseType == (int)eREWARDTYPE.ITEM)
        {
            int count = GameInfo.Instance.GetItemIDCount(storetable.PurchaseIndex);
            if( count >= storetable.PurchaseValue )
                kGachaTicketBtnList[index].kDisabledSpr.gameObject.SetActive(false);

            kGoodsTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(228), count);

            if(index == 0)
            {
                var tabledata = GameInfo.Instance.GameTable.FindItem(storetable.PurchaseIndex);
                if(tabledata != null)
                {
                    kTicketTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + tabledata.Icon);
                }
            }
        }
    }

	public void OnClick_Slot()
	{
	}

    public void OnClick_GachaBtnUnit( int index )
    {
        if (0 <= index && kGachaTicketBtnList.Count > index)
        {
            UIGachaPanel gachapanel = ParentGO.GetComponent<UIGachaPanel>();
            if (gachapanel != null)
            {
                gachapanel.SendStoreDisplayID(_datalist[index].ID);
            }
        }
    }

    public void OnClick_PercentageBtn()
    {
        if (kGachaTicketBtnList.Count == 0)
            return;
        
        UIValue.Instance.SetValue(UIValue.EParamType.GachaDetailStoreID, _datalist[0].StoreID);
        LobbyUIManager.Instance.ShowUI("GachaDetailPopup", true);
    }

}
