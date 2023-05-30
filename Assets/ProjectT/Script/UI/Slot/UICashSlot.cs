using UnityEngine;
using System.Collections;

public class UICashSlot : FSlot
{
	public UISprite kBGSpr;
	public UITexture kIconSpr;
    public UISprite kFirstBonusSpr;
    public UILabel kNameLabel;
	public UILabel kDescLabel;
	public UIButton kBuyBtn;
    public UILabel kBuyLabel;
    public UISprite kMonthlyBGSpr;

    [Space(10)]
    public GameObject kPriceOnObj;
    public GameObject kPriceOnSprObj;
    public GameObject kMonthlyDateTimeObj;
    public UILabel kMonthlyDateTimeLabel;

    public GameObject kPriceOffObj;

    [Header("Discount Price")]
    public GameObject kSaleObj;
    public UILabel kSalePriceLabel;

    private GameClientTable.StoreDisplayGoods.Param _storedisplaytable;
    private GameTable.Store.Param _storetable;
    //private StoreSaleData _storesaledata;

    private MonthlyData _monthlyData;
    private System.DateTime _nowTime;

    public void UpdateSlot(int index, GameClientTable.StoreDisplayGoods.Param tabledata)
	{
        kMonthlyBGSpr.SetActive(false);

        _storedisplaytable = tabledata;

        _storetable = GameInfo.Instance.GameTable.FindStore(_storedisplaytable.StoreID);
        //_storesaledata = GameInfo.Instance.ServerData.StoreSaleList.Find(x => x.TableID == _storedisplaytable.ID);

        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_storedisplaytable.Name);
        kDescLabel.textlocalize = FLocalizeString.Instance.GetText(_storedisplaytable.Description);

        GameSupport.LoadLocalizeTexture(kIconSpr, "icon", "Icon/" + _storedisplaytable.Icon, _storedisplaytable.IconLocalize);

        kFirstBonusSpr.gameObject.SetActive(false);
        kBuyLabel.transform.parent.gameObject.SetActive(true);

        _monthlyData = null;
        _nowTime = GameInfo.Instance.GetNetworkTime();
        //월 정액 상품 배경 활성화
        if (_storetable.ProductType == (int)eREWARDTYPE.MONTHLYFEE)
        {
            if(GameSupport.IsHaveMonthlyData(_storetable.ProductIndex))
                _monthlyData = GameInfo.Instance.UserMonthlyData.GetMonthlyDataWithStoreID(_storetable.ProductIndex);
            kMonthlyBGSpr.SetActive(true);
        }

        //kBuyLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_W), _storetable.PurchaseValue);

        SetPriceBtn();

    }

    private void Update()
    {
        if (_monthlyData == null)
            return;

        if (!GameSupport.IsHaveMonthlyData(_storetable.ProductIndex))
        {
            _monthlyData = null;
            SetPriceBtn();
        }
        else
        {
            SetPriceBtn();
        }
    }


    public void OnClick_Slot()
	{
	}
 
	public void OnClick_BuyBtn()
	{
        if (ParentGO == null)
            return;

        if (AppMgr.Instance.DisablePurchase)
        {
            if (GameInfo.Instance.IsServerRelocate) // 이전 신청 완료한 유저
            {
                MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(6016), null);
            }
            else
            {
                UITopPanel panel = LobbyUIManager.Instance.GetUI<UITopPanel>();
                MessagePopup.OK(FLocalizeString.Instance.GetText((int)eTEXTID.OK), FLocalizeString.Instance.GetText(6018), 
                                FLocalizeString.Instance.GetText(6017), panel.OnClick_ServerRelocate);
            }

            FComponent component = ParentGO.GetComponent<FComponent>();
            component.OnClickClose();

            return;
        }

        UICashBuyPopup cashPopup = ParentGO.GetComponent<UICashBuyPopup>();
        if (cashPopup == null)
            return;

        if (!IAPManager.Instance.IAPNULL_CHECK())
        {
            MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3197), null);
            return;
        }
        Debug.Log("Inapp Purchase");

        if (_storetable.ProductType == (int)eREWARDTYPE.MONTHLYFEE)
        {
            if(GameSupport.IsHaveMonthlyData(_storetable.ProductIndex))
                return;
        }
        
        cashPopup.OnClick_Slot(_storedisplaytable, _storetable.PurchaseValue);
    }

    private void SetDiscount(int storeid)
    {
        kSaleObj.SetActive(false);

        GameClientTable.StoreDisplayGoods.Param clientTable = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == storeid);

        if (clientTable == null)
            return;

        if (clientTable.OriginalPriceID > (int)eCOUNT.NONE)
        {
            GameTable.Store.Param storedata = GameInfo.Instance.GameTable.FindStore(x => x.ID == clientTable.OriginalPriceID);
            if (storedata != null)
            {
                kSaleObj.SetActive(true);

#if UNITY_EDITOR
                kSalePriceLabel.textlocalize = string.Format("${0:0.##}", (float)storedata.PurchaseValue * 0.01f);
#else
#if UNITY_ANDROID
                    kSalePriceLabel.textlocalize = IAPManager.Instance.GetPrice(storedata.AOS_ID);
#elif UNITY_IOS
                    kSalePriceLabel.textlocalize = IAPManager.Instance.GetPrice(storedata.IOS_ID);
#elif !DISABLESTEAMWORKS
                    kSalePriceLabel.textlocalize = string.Format("${0:0.##}", (float)storedata.PurchaseValue * 0.01f);
#endif

#endif
            }
        }
    }

    private void SetPriceBtn()
    {
        //월 정액 상품 배경 활성화
        if (_storetable.ProductType == (int)eREWARDTYPE.MONTHLYFEE)
        {
            if (IAPManager.Instance.IAPNULL_CHECK())
            {
                if (_monthlyData != null && GameSupport.IsHaveMonthlyData(_storetable.ProductIndex))
                {
                    kPriceOnObj.SetActive(true);
                    kPriceOnSprObj.SetActive(false);
                    kMonthlyDateTimeObj.SetActive(true);

                    _nowTime = GameInfo.Instance.GetNetworkTime();
                    string endTimeStr = GameSupport.GetRemainTimeString_DayAndHours(_monthlyData.MonthlyEndTime, _nowTime);

                    kMonthlyDateTimeLabel.textlocalize = FLocalizeString.Instance.GetText(1588, endTimeStr);
                }
                else
                {
                    kPriceOnObj.SetActive(true);
                    kPriceOnSprObj.SetActive(true);
                    kMonthlyDateTimeObj.SetActive(false);
                    kPriceOffObj.SetActive(false);

                    kBuyLabel.gameObject.SetActive(true);

#if UNITY_EDITOR
                    kBuyLabel.textlocalize = string.Format("${0:0.##}", (float)_storetable.PurchaseValue * 0.01f);
#else
#if UNITY_ANDROID
                    kBuyLabel.textlocalize = IAPManager.Instance.GetPrice(_storetable.AOS_ID);
#elif UNITY_IOS
                    kBuyLabel.textlocalize = IAPManager.Instance.GetPrice(_storetable.IOS_ID);
#elif !DISABLESTEAMWORKS
                    kBuyLabel.textlocalize = string.Format("${0:0.##}", (float)_storetable.PurchaseValue * 0.01f);
#endif

#endif
                    SetDiscount(_storetable.ID);
                }
            }
            else
            {
                kSaleObj.SetActive(false);
                kPriceOnObj.SetActive(false);
                kPriceOffObj.SetActive(true);
                kBuyLabel.gameObject.SetActive(false);
            }
        }
        else
        {
            if (IAPManager.Instance.IAPNULL_CHECK())
            {
                kPriceOnObj.SetActive(true);
                kPriceOnSprObj.SetActive(true);
                kMonthlyDateTimeObj.SetActive(false);
                kPriceOffObj.SetActive(false);

                kBuyLabel.gameObject.SetActive(true);
#if UNITY_EDITOR
                kBuyLabel.textlocalize = string.Format("${0:0.##}", (float)_storetable.PurchaseValue * 0.01f);
#else
#if UNITY_ANDROID
                kBuyLabel.textlocalize = IAPManager.Instance.GetPrice(_storetable.AOS_ID);
#elif UNITY_IOS
                kBuyLabel.textlocalize = IAPManager.Instance.GetPrice(_storetable.IOS_ID);
#elif !DISABLESTEAMWORKS
                kBuyLabel.textlocalize = string.Format("${0:0.##}", (float)_storetable.PurchaseValue * 0.01f);
#endif

#endif
                SetDiscount(_storetable.ID);
            }
            else
            {
                kSaleObj.SetActive(false);
                kPriceOnObj.SetActive(false);
                kPriceOffObj.SetActive(true);
                kBuyLabel.gameObject.SetActive(false);
            }
        }
    }
}
