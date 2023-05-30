using UnityEngine;
using System.Collections;

public class UIRewardBoxListSlot : FSlot {

    public UISprite kBoxSpr;
    public UISprite kHeartSpr;
    public UISprite kMonthSpr;
    public UISprite kFavorSpr;
    public UIItemListSlot kItemListSlot;
    public UIRewardBoxOpen kOpen;
    private int _index;
    private RewardData _rewarddata;
    private bool _bopen;
    public bool IsOpen { get { return _bopen;  } }

    public void UpdateSlot(int index, int boxtype, RewardData rewarddata) 	//Fill parameter if you need
	{
        _index = index;
        _rewarddata = rewarddata;
        _bopen = rewarddata.bNew;

        kHeartSpr.gameObject.SetActive(false);
        kMonthSpr.gameObject.SetActive(false);
        kFavorSpr.gameObject.SetActive(false);
        if ( rewarddata.bNew )
        {
            kItemListSlot.gameObject.SetActive(true);
            kItemListSlot.transform.localScale = Vector3.one;
            kBoxSpr.gameObject.SetActive(false);

        }
        else
        {
            kBoxSpr.gameObject.SetActive(true);
            kItemListSlot.gameObject.SetActive(false);
        }


        if (boxtype == 0)
            kBoxSpr.spriteName = "icon_box";
        else
            kBoxSpr.spriteName = "icon_box2";

        

        if (rewarddata.Type == (int)eREWARDTYPE.WEAPON)         //무기
        {
            WeaponData data = GameInfo.Instance.GetWeaponData(rewarddata.UID);
            if (data != null)
            {
                kItemListSlot.ParentGO = ParentGO;
                kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Result, index, data);
                //kItemListSlot.kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GRAY_TEXT_COLOR), rewarddata.Value);
            }
        }
        else if (rewarddata.Type == (int)eREWARDTYPE.GEM)            //곡옥
        {
            GemData data = GameInfo.Instance.GetGemData(rewarddata.UID);
            if (data != null)
            {
                kItemListSlot.ParentGO = ParentGO;
                kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Result, index, data);
                //kItemListSlot.kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GRAY_TEXT_COLOR), rewarddata.Value);
            }
        }
        else if (rewarddata.Type == (int)eREWARDTYPE.ITEM)
        {
            ItemData data = GameInfo.Instance.GetItemData(rewarddata.UID);
            if (data != null)
            {
                kItemListSlot.ParentGO = ParentGO;
                kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Result, index, data, rewarddata.Value);
                //kItemListSlot.kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GRAY_TEXT_COLOR), rewarddata.Value);
            }
        }
        else if (rewarddata.Type == (int)eREWARDTYPE.CARD)
        {
            CardData data = GameInfo.Instance.GetCardData(rewarddata.UID);
            if (data != null)
            {
                kItemListSlot.ParentGO = ParentGO;
                kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Result, index, data);
                //kItemListSlot.kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GRAY_TEXT_COLOR), rewarddata.Value);
            }
        }
        else if (rewarddata.Type == (int)eREWARDTYPE.GOODS)
        {
            kItemListSlot.ParentGO = ParentGO;
            kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Result, index, rewarddata);
        }

        if (_bopen)
        {
            if (_rewarddata.ChangeGrade)
                kHeartSpr.gameObject.SetActive(true);
            if (_rewarddata.MonthGrade)
                kMonthSpr.gameObject.SetActive(true);
            if (_rewarddata.FavorGrade)
                kFavorSpr.gameObject.SetActive(true);
        }
    }

    public void Open()
    {
        kItemListSlot.gameObject.SetActive(true);
        kOpen.OpenBox();
        _bopen = true;
    }
    public void OnClick_Slot()
	{

	}
 
}
