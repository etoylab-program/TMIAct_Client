using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStoreListSlotSM : FSlot
{

    public UITexture kIconTex;
    public UIItemListSlot kItemListSlot;
    public UILabel kTitleLabel;
    public UILabel kCountLabel;
    public UILabel kDescLabel;
    public UILabel kSaleValueLabel;
    public UIGoodsUnit kGoodsUnit;
    public GameObject kNew;
    public GameObject kLimitedObj;
    public UILabel kLimitedLabel;


    private GameClientTable.StoreDisplayGoods.Param _storedisplaytable;
    private GameTable.Store.Param _storetable;
    private StoreSaleData _storesaledata;
    private long _purchasevalue;
    private int _index;

    public GameTable.Store.Param StoreTable { get { return _storetable; } }

    public void UpdateSlot(int index, GameClientTable.StoreDisplayGoods.Param tabledata, UIItemListSlot.ePosType type, int itemCount, ref int maxItemCount)    //Fill parameter if you need
    {
        _index = index;

        maxItemCount = 1;
        kLimitedObj.SetActive(false);

        _storedisplaytable = tabledata;

        _storetable = GameInfo.Instance.GameTable.FindStore(_storedisplaytable.StoreID);
        _storesaledata = GameInfo.Instance.ServerData.StoreSaleList.Find(x => x.TableID == _storedisplaytable.ID);

        kSaleValueLabel.transform.parent.gameObject.SetActive(false);
        kItemListSlot.gameObject.SetActive(false);
        kIconTex.gameObject.SetActive(true);
        kNew.gameObject.SetActive(false);
        kCountLabel.gameObject.SetActive(false);

        kTitleLabel.textlocalize = "";
        kDescLabel.textlocalize = "";
        kCountLabel.textlocalize = "";

        //  icon이 빈스트링일 경우 메인 텍스쳐 셋팅을 막습니다.
        if (string.IsNullOrEmpty(_storedisplaytable.Icon) != true)
            GameSupport.LoadLocalizeTexture(kIconTex, "icon", "Icon/" + _storedisplaytable.Icon, tabledata.IconLocalize);

        _storetable = GameInfo.Instance.GameTable.FindStore(_storedisplaytable.StoreID);

        if (_storetable == null)
        {
            kGoodsUnit.gameObject.SetActive(false);

        }
        else
        {
            //kTitleLabel.pivot = UIWidget.Pivot.Center;
            if (_storetable.ProductType == (int)eREWARDTYPE.GOODS)
            {
                //kCountLabel.gameObject.SetActive(true);
                //kTitleLabel.pivot = UIWidget.Pivot.Left;
                if(!string.IsNullOrEmpty(_storetable.AOS_ID) || !string.IsNullOrEmpty(_storetable.IOS_ID))
                {
                    RewardData rewardiap = new RewardData(0, _storetable.ProductType, _storetable.ProductIndex, _storetable.ProductValue, false);
                    kTitleLabel.textlocalize = GameSupport.GetProductName(rewardiap);

                    kGoodsUnit.gameObject.SetActive(true);
                    _purchasevalue = 0;

                    kGoodsUnit.InitGoodsUnit((eGOODSTYPE)_storetable.PurchaseIndex, _purchasevalue * itemCount);

#if UNITY_ANDROID
                    kGoodsUnit.SetText(IAPManager.Instance.GetPrice(_storetable.AOS_ID));
#elif UNITY_IOS
                    kGoodsUnit.SetText(IAPManager.Instance.GetPrice(_storetable.IOS_ID));
#elif !DISABLESTEAMWORKS
                    kGoodsUnit.SetText(string.Format("${0:0.##}", (float)_storetable.PurchaseValue * 0.01f));
#endif
                    SetTitleDesc();
                    return;
                }
            }
            else if (_storetable.ProductType == (int)eREWARDTYPE.WEAPON)
            {
                var data = GameInfo.Instance.GameTable.FindWeapon(_storetable.ProductIndex);
                if (data != null)
                {
                    kIconTex.gameObject.SetActive(false);
                    kItemListSlot.gameObject.SetActive(true);
                    kItemListSlot.ParentGO = this.gameObject;
                    kItemListSlot.UpdateSlot(type, -1, data);
                }
            }
            else if (_storetable.ProductType == (int)eREWARDTYPE.GEM)
            {
                var data = GameInfo.Instance.GameTable.FindGem(_storetable.ProductIndex);
                if (data != null)
                {
                    kIconTex.gameObject.SetActive(false);
                    kItemListSlot.gameObject.SetActive(true);
                    kItemListSlot.ParentGO = this.gameObject;
                    kItemListSlot.UpdateSlot(type, -1, data);
                }
            }
            else if (_storetable.ProductType == (int)eREWARDTYPE.CARD)
            {
                var data = GameInfo.Instance.GameTable.FindCard(_storetable.ProductIndex);
                if (data != null)
                {
                    kIconTex.gameObject.SetActive(false);
                    kItemListSlot.gameObject.SetActive(true);
                    kItemListSlot.ParentGO = this.gameObject;
                    kItemListSlot.UpdateSlot(type, -1, data);
                }
            }
            else if (_storetable.ProductType == (int)eREWARDTYPE.ITEM)
            {
                var data = GameInfo.Instance.GameTable.FindItem(_storetable.ProductIndex);
                if (data != null)
                {
                    kIconTex.gameObject.SetActive(false);
                    kItemListSlot.gameObject.SetActive(true);
                    kItemListSlot.ParentGO = this.gameObject;
                    kItemListSlot.UpdateSlot(type, -1, data);
                    
                    kItemListSlot.kCountLabel.textlocalize = (_storetable.ProductValue * itemCount).ToString();
                }
            }
            else if (_storetable.ProductType == (int)eREWARDTYPE.USERMARK)
            {
                GameTable.UserMark.Param data = GameInfo.Instance.GameTable.FindUserMark(_storetable.ProductIndex);
                if(data != null)
                {
                    kIconTex.gameObject.SetActive(false);
                    kItemListSlot.gameObject.SetActive(true);
                    kItemListSlot.ParentGO = this.gameObject;
                    kItemListSlot.UpdateSlot(type, -1, data);
                    kItemListSlot.kCountLabel.textlocalize = (_storetable.ProductValue * itemCount).ToString();
                }
            }
            else if (_storetable.ProductType == (int)eREWARDTYPE.BADGE)
            {
                GameTable.BadgeOpt.Param data = GameInfo.Instance.GameTable.FindBadgeOpt(_storetable.ProductIndex);
                if (data != null)
                {
                    kIconTex.gameObject.SetActive(false);
                    kItemListSlot.gameObject.SetActive(true);
                    kItemListSlot.ParentGO = this.gameObject;
                    kItemListSlot.UpdateSlot(type, -1, _storetable.ProductIndex, data);
                    kItemListSlot.kCountLabel.textlocalize = (_storetable.ProductValue * itemCount).ToString();
                }
                else
                {
                    //ProductIndex가 0이면 랜덤
                    if (_storetable.ProductIndex == 0)
                    {
                        kIconTex.gameObject.SetActive(false);
                        kItemListSlot.gameObject.SetActive(true);
                        kItemListSlot.ParentGO = this.gameObject;
                        kItemListSlot.UpdateSlot(type, -1, _storetable.ProductIndex, null);
                        kItemListSlot.kCountLabel.textlocalize = (_storetable.ProductValue * itemCount).ToString();
                    }
                }
            }
            RewardData reward = new RewardData(0, _storetable.ProductType, _storetable.ProductIndex, _storetable.ProductValue * itemCount, false);
            kTitleLabel.textlocalize = GameSupport.GetProductName(reward);

            kGoodsUnit.gameObject.SetActive(true);
            _purchasevalue = _storetable.PurchaseValue;

            kGoodsUnit.InitGoodsUnit(_storetable, null, itemCount);

            long count = 0;
            if ((eREWARDTYPE)_storetable.PurchaseType == eREWARDTYPE.GOODS)
            {
                count = GameInfo.Instance.UserData.GetGoods((eGOODSTYPE)_storetable.PurchaseIndex);
            }
            else if((eREWARDTYPE)_storetable.PurchaseType == eREWARDTYPE.ITEM)
            {
                count = GameInfo.Instance.GetItemIDCount(_storetable.PurchaseIndex);
            }

            maxItemCount = (int)(count / _purchasevalue);
        }

        bool isEvent = true;
        if( tabledata.PanelType != (int)eSD_PanelType.RAID_STORE ) {
            switch( (eGOODSTYPE)_storetable.PurchaseIndex ) {
                case eGOODSTYPE.CASH:
                case eGOODSTYPE.SUPPORTERPOINT:
                case eGOODSTYPE.FRIENDPOINT: {
                    isEvent = false;
                }
                break;
            }
        }

        //구매 횟수 체크
        StoreSaleData saleData = GameInfo.Instance.ServerData.StoreSaleList.Find(x => x.TableID == _storetable.ID);
        if (saleData != null)
        {
            if (saleData.LimitType == (int)eStoreSaleKind.LimitCnt || 
                ( tabledata.PanelType == (int)eSD_PanelType.RAID_STORE && 
                  ( saleData.LimitType == (int)eStoreSaleKind.LimitDate || 
                    saleData.LimitType == (int)eStoreSaleKind.LimitDate_Weekly || 
                    saleData.LimitType == (int)eStoreSaleKind.LimitDate_Monthly ) ) )
            {
                int limitCount = GameSupport.GetLimitedCnt(_storetable.ID);
                if (isEvent)
                {
                    if (0 < limitCount)
                    {
                        kLimitedObj.SetActive(true);
                        kLimitedLabel.textlocalize = FLocalizeString.Instance.GetText(1584, limitCount);
                    }
                    else
                    {
                        kGoodsUnit.SetActive(false);
                        //kDimdSpr.SetActive(true);
                        //kDimdLabel.textlocalize = FLocalizeString.Instance.GetText(1580);
                    }
                }
                else
                {
                    bool isSale = 0 < saleData.DiscountRate;
                    if (isSale)
                    {
                        int remainCount = GameSupport.GetLimitedCnt(_storetable.ID);
                        isSale = 0 < remainCount;
                        if (isSale)
                        {
                            kLimitedObj.SetActive(true);
                            kLimitedLabel.textlocalize = FLocalizeString.Instance.GetText(1584, limitCount);
                        }
                    }
                    else
                    {
                        if (0 < limitCount)
                        {
                            kLimitedObj.SetActive(true);
                            kLimitedLabel.textlocalize = FLocalizeString.Instance.GetText(1584, limitCount);
                        }
                    }

                    if (!isSale)
                    {
                        kGoodsUnit.InitGoodsUnit((eGOODSTYPE)_storetable.PurchaseIndex, _storetable.PurchaseValue, true);
                    }
                }

                if (0 < limitCount)
                {
                    maxItemCount = Mathf.Min(limitCount, maxItemCount);
                }
            }
        }

        SetTitleDesc();
    }

    private void SetTitleDesc()
    {
        if (_storedisplaytable.Name != 0)
            kTitleLabel.textlocalize = FLocalizeString.Instance.GetText(_storedisplaytable.Name);
        if (_storedisplaytable.Description != 0)
            kDescLabel.textlocalize = FLocalizeString.Instance.GetText(_storedisplaytable.Description);
        if (_storedisplaytable.Count != 0)
        {
            kCountLabel.gameObject.SetActive(true);
            kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.STACK_COUNT_TXT), _storedisplaytable.Count);
        }
    }

    public void OnClick_Slot()
    {
        if (ParentGO == null)
            return;
        
        UIStorePanel storepanel = ParentGO.GetComponent<UIStorePanel>();
        if (storepanel != null)
        {
            Log.Show(_storedisplaytable.StoreID);

            storepanel.ClickSlot(_index, _storedisplaytable);
        }

        //아레나 스토어
        UIArenaStorePopup arenaStorePopup = ParentGO.GetComponent<UIArenaStorePopup>();
        if(arenaStorePopup != null)
        {
            arenaStorePopup.OnClickArenaStoreSlot(_storedisplaytable, _purchasevalue);
        }
    }
}
