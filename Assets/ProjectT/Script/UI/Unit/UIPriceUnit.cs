using UnityEngine;
using System.Collections;

public class UIPriceUnit : FUnit 
{
    public GameObject ParentGO;

    [Header("CommonBuy Property")]
    public GameObject kCommonBuyBtn;
    public GameObject kCommonLimitedObj;
    public UILabel kCommonLimitedLabel;

    public GameObject kCommonPriceOn;
    public UILabel kCommonPriceOnLabel;
    public GameObject kCommonPriceDimd;

    public GameObject kCommonPriceOff;
    public UILabel kCommonPriceOffLabel;

    public GameObject kCommonMonthlyExtendObj;
    public UILabel kCommonMonthlyExtendLabel;

    public GameObject kCommonSaleObj;
    public UILabel kCommonOriginPriceLabel;

    [Header("PremiumBuy Property")]
    public GameObject kPremiumBuyBtn;
    public GameObject kPremiumLimitedObj;
    public UILabel kPremiumLimitedLabel;

    public GameObject kPremiumPriceOn;
    public UILabel kPremiumPriceOnLabel;
    public GameObject kPremiumPriceOnDimd;

    public GameObject kPremiumPriceOff;
    public UILabel kPremiumPriceOffLabel;

    public GameObject kPremiumSaleObj;
    public UILabel kPremiumOriginPriceLabel;

    [SerializeField] private GameObject _TimeObj;
    [SerializeField] private GameObject _InfoSprObj;

    [SerializeField] private UILabel _TimeLabel;
    [SerializeField] private UILabel _InfoSprLabel;

    private UIPackagePopup.ePackageUIType _uiType;
    private System.DateTime _nowTime;
    private GameClientTable.StoreDisplayGoods.Param _clientDisplayData = null;

    public void UpdateSlot(int storeID, System.DateTime nowTime, UIPackagePopup.ePackageUIType uiType) 	//Fill parameter if you need
	{
        kCommonBuyBtn.SetActive(false);
        kPremiumBuyBtn.SetActive(false);
        _uiType = uiType;
        _nowTime = nowTime;
        SetPriceBtn(storeID);

        _clientDisplayData = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == storeID && x.PackageUIType == (int)eCOUNT.NONE);
        if (_clientDisplayData == null)
            return;

		ePackageLimitType limitType = (ePackageLimitType)_clientDisplayData.LimitType;
		if ( ePackageLimitType.MAX <= limitType ) {
			limitType = ePackageLimitType.NONE;
		}

		_TimeObj.SetActive( limitType != ePackageLimitType.NONE && kPremiumPriceOff.activeSelf );
		if ( _TimeObj.gameObject.activeSelf ) {
			string remainTimeStr = string.Empty;
			StoreData storeData = GameInfo.Instance.GetStoreData( storeID );
			if ( storeData != null ) {
				remainTimeStr = FLocalizeString.Instance.GetText( 1059, GameSupport.GetRemainTimeString( GameSupport.GetLocalTimeByServerTime( storeData.GetResetTime() ), GameSupport.GetCurrentServerTime() ) );
			}
			_TimeLabel.textlocalize = remainTimeStr;
		}

		_InfoSprObj.SetActive( limitType != ePackageLimitType.NONE && kPremiumPriceOn.activeSelf );
		if ( _InfoSprObj.gameObject.activeSelf ) {
			_InfoSprLabel.text = limitType.ToString();
		}

        switch (_uiType)
        {
            case UIPackagePopup.ePackageUIType.CharJump:
                {
                    if (_clientDisplayData.AdvancedPackage == (int)CharPackageType.Normal)
                    {
                        kCommonLimitedLabel.textlocalize = FLocalizeString.Instance.GetText(1702);
                    }
                    else if (_clientDisplayData.AdvancedPackage == (int)CharPackageType.Premium)
                    {
                        kPremiumLimitedLabel.textlocalize = FLocalizeString.Instance.GetText(1702);
                    }
                    else if (_clientDisplayData.AdvancedPackage == (int)CharPackageType.Limit)
                    {
                        int limitedCnt = GameSupport.GetLimitedCnt(storeID);
                        kPremiumLimitedLabel.textlocalize = FLocalizeString.Instance.GetText(1584, limitedCnt);
                    }
                }
                break;
            case UIPackagePopup.ePackageUIType.MonthlyFee:
                break;
            case UIPackagePopup.ePackageUIType.Rank:
                break;
            case UIPackagePopup.ePackageUIType.Starter:
                break;
            case UIPackagePopup.ePackageUIType.Time:
                break;
            case UIPackagePopup.ePackageUIType.Gacha:
                break;
        }
    }

    public void OnClick_CommonBuyBtn()
    {
        if (kCommonPriceOff.activeSelf)
            return;

        if (this.ParentGO != null)
        {
            if (this.ParentGO.GetComponent<UIPackageSlot>() != null)
            {
                this.ParentGO.GetComponent<UIPackageSlot>().BuyPackage();
            }
            else if (this.ParentGO.GetComponent<UIPackageInfoPopup>() != null)
            {
                this.ParentGO.GetComponent<UIPackageInfoPopup>().OnClick_BuyBtn();
            }
        }
        Log.Show("OnClick_CommonBuyBtn");
    }

    public void OnClick_PremiumBuyBtn()
    {
        if (kPremiumPriceOff.activeSelf)
            return;

        if (this.ParentGO != null)
        {
            if (this.ParentGO.GetComponent<UIPackageSlot>() != null)
            {
                this.ParentGO.GetComponent<UIPackageSlot>().BuyPackage();
            }
            else if (this.ParentGO.GetComponent<UIPackageInfoPopup>() != null)
            {
                this.ParentGO.GetComponent<UIPackageInfoPopup>().OnClick_BuyBtn();
            }
        }
        Log.Show("OnClick_PremiumBuyBtn");
    }

    public void SetVisiblePremiumBtn(bool b)
    {
        kCommonBuyBtn.SetActive(!b);
        kPremiumBuyBtn.SetActive(b);
    }

    private void SetPriceBtn(int storeID)
    {
        GameTable.Store.Param storeParam = GameInfo.Instance.GameTable.FindStore(x => x.ID == storeID);
        if (storeParam == null)
        {
            Debug.LogError("StoreParam is null");
            return;
        }

        kCommonPriceDimd.SetActive(false);
        kPremiumPriceOnDimd.SetActive(false);
        kCommonLimitedObj.SetActive(false);
        kPremiumLimitedObj.SetActive(false);
        kCommonMonthlyExtendObj.SetActive(false);

        kCommonSaleObj.SetActive(false);
        kPremiumSaleObj.SetActive(false);

        switch (_uiType) {

            case UIPackagePopup.ePackageUIType.CharJump: {
                    if (GameSupport.GetLimitedCnt(storeID) <= 0) {
                        SetPriceOffBtn(1580);
                        return;
                    }
                }
                break;

            case UIPackagePopup.ePackageUIType.MonthlyFee: {
                    if (GameSupport.IsHaveMonthlyData((int)eMonthlyType.PREMIUM) && !GameSupport.PremiumMonthlyDateFlag((int)eMonthlyType.PREMIUM)) {
                        SetPriceOffBtn(1580);
                        return;
                    }
                }
                break;

            case UIPackagePopup.ePackageUIType.Time: {
                    if (GameSupport.IsHaveStoreData(storeID)) {
                        if (!GameSupport.IsStoreSaleApply(storeID)) {
                            SetPriceOffBtn(1580);
                            return;
                        }
                    }
                }
                break;
        }

        if (_uiType != UIPackagePopup.ePackageUIType.Time) {
            if (storeParam.BuyNotStoreID > (int)eCOUNT.NONE) {
                if (GameSupport.IsHaveStoreData(storeParam.BuyNotStoreID)) {
                    SetPriceOffBtn(1715);
                    return;
                }
            }
        }

        if (storeParam.ProductType == (int)eREWARDTYPE.MONTHLYFEE)
        {
            if (GameSupport.PremiumMonthlyDateFlag(storeParam.ProductIndex))
            {
                kCommonMonthlyExtendObj.SetActive(true);

                int addDay = (GameInfo.Instance.GameConfig.MonthlyFeeLimitMin / 24) / 60;

                kCommonMonthlyExtendLabel.textlocalize = FLocalizeString.Instance.GetText(1589, addDay.ToString());

                kCommonPriceOn.SetActive(true);
                kCommonPriceOff.SetActive(false);
                kCommonPriceOnLabel.SetActive(true);

                if (GameSupport.IsHaveStoreData(storeID) && GameSupport.GetLimitedCnt(storeID).Equals(0))
                {
                    SetPriceOffBtn(1580);
                }
                else
                {
                    int limitedCnt = GameSupport.GetLimitedCnt(storeID);

                    if (limitedCnt > 0)
                    {
                        kCommonLimitedObj.SetActive(true);
                        kCommonLimitedLabel.textlocalize = FLocalizeString.Instance.GetText(1584, limitedCnt);
                    }

                    SetPriceValue(storeParam);
                }
            }
            else
            {
                if (GameSupport.IsHaveMonthlyData(storeParam.ProductIndex))
                {
                    kCommonPriceDimd.SetActive(true);

                    MonthlyData monthlyData = GameInfo.Instance.UserMonthlyData.GetMonthlyDataWithStoreID(storeParam.ProductIndex);

                    string endTimeStr = GameSupport.GetRemainTimeString(monthlyData.MonthlyEndTime, _nowTime);

                    kCommonPriceOnLabel.textlocalize = FLocalizeString.Instance.GetText(1588, endTimeStr);
                }
                else
                {
                    kCommonPriceOn.SetActive(true);
                    kCommonPriceOff.SetActive(false);
                    kCommonPriceOnLabel.gameObject.SetActive(true);

                    if (GameSupport.IsHaveStoreData(storeID) && GameSupport.GetLimitedCnt(storeID).Equals(0))
                    {
                        kCommonPriceDimd.SetActive(true);
                        kCommonPriceOnLabel.textlocalize = FLocalizeString.Instance.GetText(1580);
                    }
                    else
                    {
                        int limitedCnt = GameSupport.GetLimitedCnt(storeID);

                        if (limitedCnt > 0)
                        {
                            kCommonLimitedObj.SetActive(true);
                            kCommonLimitedLabel.textlocalize = FLocalizeString.Instance.GetText(1584, limitedCnt);
                        }

                        SetPriceValue(storeParam);
                    }
                }
            }
        }
        else
        {
            if (IAPManager.Instance.IAPNULL_CHECK())
            {
                kCommonPriceOn.SetActive(true);
                kCommonPriceOff.SetActive(false);
                kCommonPriceOnLabel.gameObject.SetActive(true);

                kPremiumPriceOn.SetActive(true);
                kPremiumPriceOff.SetActive(false);
                kPremiumPriceOnLabel.gameObject.SetActive(true);



                //if (GameSupport.IsHaveStoreData(storeID) && GameSupport.GetLimitedCnt(storeID).Equals(0))
                if (!GameSupport.IsStoreSaleApply(storeID))
                {
                    SetPriceOffBtn(1580);
                }
                else
                {
                    int limitedCnt = GameSupport.GetLimitedCnt(storeID);
                    
                    if (storeID == 2019)
                    {
                        limitedCnt = 0;
                    }
                    
                    if (limitedCnt > 0)
                    {
                        kCommonLimitedObj.SetActive(true);
                        kCommonLimitedLabel.textlocalize = FLocalizeString.Instance.GetText(1584, limitedCnt);

                        kPremiumLimitedObj.SetActive(true);
                        kPremiumLimitedLabel.textlocalize = FLocalizeString.Instance.GetText(1584, limitedCnt);
                    }

                    SetPriceValue(storeParam);
                }
            }
            else
            {
                SetPriceOffBtn(1580);
            }
        }
    }

    public void SetPriceOffBtn(int textID)
    {
        kCommonPriceOn.SetActive(false);
        kCommonPriceOff.SetActive(true);
        kCommonPriceOnLabel.gameObject.SetActive(false);

        kPremiumPriceOn.SetActive(false);
        kPremiumPriceOff.SetActive(true);
        kPremiumPriceOnLabel.gameObject.SetActive(false);

        kCommonPriceOffLabel.textlocalize = FLocalizeString.Instance.GetText(textID);
        kPremiumPriceOffLabel.textlocalize = FLocalizeString.Instance.GetText(textID);
    }

    private void SetPriceValue(GameTable.Store.Param storeParam) {

        if (storeParam.PurchaseValue == 0) {
            kCommonPriceOnLabel.textlocalize = FLocalizeString.Instance.GetText(1048);
        }
        else {
#if UNITY_EDITOR
            kCommonPriceOnLabel.textlocalize = string.Format("${0:0.##}", (float)storeParam.PurchaseValue * 0.01f);
#elif UNITY_ANDROID
            kCommonPriceOnLabel.textlocalize = IAPManager.Instance.GetPrice(storeParam.AOS_ID);
#elif UNITY_IOS
            kCommonPriceOnLabel.textlocalize = IAPManager.Instance.GetPrice(storeParam.IOS_ID);
#elif !DISABLESTEAMWORKS
            kCommonPriceOnLabel.textlocalize = string.Format("${0:0.##}", (float)storeParam.PurchaseValue * 0.01f);
#endif
        }

        kPremiumPriceOnLabel.textlocalize = kCommonPriceOnLabel.textlocalize;

        //할인 오브젝트 체크
        GameClientTable.StoreDisplayGoods.Param clientTable = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == storeParam.ID);

        if (clientTable == null)
            return;

        if (clientTable.OriginalPriceID > (int)eCOUNT.NONE) {
            GameTable.Store.Param storedata = GameInfo.Instance.GameTable.FindStore(x => x.ID == clientTable.OriginalPriceID);
            if (storedata != null) {
                kCommonSaleObj.SetActive(true);
                kPremiumSaleObj.SetActive(true);

#if UNITY_EDITOR
                kCommonOriginPriceLabel.textlocalize = string.Format("${0:0.##}", (float)storedata.PurchaseValue * 0.01f);
#elif UNITY_ANDROID
                kCommonOriginPriceLabel.textlocalize = IAPManager.Instance.GetPrice(storedata.AOS_ID);
#elif UNITY_IOS
                kCommonOriginPriceLabel.textlocalize = IAPManager.Instance.GetPrice(storedata.IOS_ID);
#elif !DISABLESTEAMWORKS
                kCommonOriginPriceLabel.textlocalize = string.Format("${0:0.##}", (float)storedata.PurchaseValue * 0.01f);
#endif
                kPremiumOriginPriceLabel.textlocalize = kCommonOriginPriceLabel.textlocalize;
            }
        }
    }
}
