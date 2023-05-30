using UnityEngine;
using System.Collections;

public class UIFacilityItemCombineListSlot : FSlot {
 

	public UISprite kBGSpr;
    public UISprite kSelSpr;
    public UILabel kNameLabel;
	public UILabel kDescLabel;
    public UILabel kItemCountLabel;
	public UIItemListSlot kItemListSlot;

    
    private GameTable.FacilityItemCombine.Param _data;
    private int _index;

    public void UpdateSlot( int index, GameTable.FacilityItemCombine.Param data) 	//Fill parameter if you need
	{
        _data = data;
        _index = index;

        kSelSpr.gameObject.SetActive(false);

        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_data.Name);
        kDescLabel.textlocalize = FLocalizeString.Instance.GetText(_data.Desc);

        var itemdata = GameInfo.Instance.GameTable.FindItem(data.ItemID);
        if (itemdata != null)
            kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Mat, -1, itemdata);

        kItemListSlot.SetCountLabel(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT), _data.ItemCnt));

        kItemCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(279), GameInfo.Instance.GetItemIDCount(data.ItemID));

        UIFacilityItemCombinePopup facilityitemcombinepopup = ParentGO.GetComponent<UIFacilityItemCombinePopup>();
        if (facilityitemcombinepopup == null)
            return;
        if (facilityitemcombinepopup.SeleteID == _data.ID)
        {
            kSelSpr.gameObject.SetActive(true);
            
        }
    }
 
	public void OnClick_Slot()
	{
        UIFacilityItemCombinePopup facilityitemcombinepopup = ParentGO.GetComponent<UIFacilityItemCombinePopup>();
        if (facilityitemcombinepopup == null)
            return;
        facilityitemcombinepopup.SetSeleteID(_data.ID);
    }
 
}
