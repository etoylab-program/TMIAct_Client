using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class UIBannerSlot : FSlot {

    public enum ePosType
    {
        None = -1,
        Event,
        Banner,
        Gacha,
        Package,
        Mission,
        RenewalEvent,
    }

    public UITexture kBannerTex;
    public UISprite kBannerSpr;
    public UISprite kLoadingSpr;
    public UISprite kSelSpr;
    public UISprite kNoticeSpr;
    public GameObject kCompleteObj;
    public UISprite kDesireIconSpr;

    public GameObject kDesireGaugeObj;
    public UIGaugeUnit kDesireGaugeUnit;
    public GameObject kDesireGaugeMaxEff;

    public GameObject kInfoTimeObj;
    public UILabel kInfoTimeLabel;

    public UILabel kNameLabel;
    public UILabel kDescLabel;
    public UITexture kCharTex;

    public GameObject kCharPackageObj;
    public UITexture kCharPackageTex;
    public UILabel kCharPackageLabel;

    public UISprite kRepeatSpr;
    public UISprite kSaleSpr;
    public UISprite kLimitedSpr;
    public List<UILabel> kGachaLabelTypeList;
    public UILabel kDescriptionLabel;
    public UILabel kEventBoardLabel;

    private int _index;
    private ePosType _pos;
    private BannerData _bannerdata;
    private EventSetData _eventsetdata;
    private UIGachaPanel.GachaTabParam _gachatab;
    private GameClientTable.StoreDisplayGoods.Param _storedisplaygoods;
    
    private DateTime _endDateTime;
    private int _unexpectedPackageTableId;

    public void UpdateSlot(ePosType pos, int index, int selectIndex)
    {
        _pos = pos;
        _index = index;
        
        kBannerSpr.SetActive(true);
        kBannerTex.SetActive(false);
        kNoticeSpr.SetActive(false);
        kLoadingSpr.SetActive(false);
        
        kSelSpr.SetActive(index == selectIndex);
    }
    
    public void UpdateSlot(ePosType pos, int index, BannerData data ) 	//Fill parameter if you need
	{
        _pos = pos;
        _index = index;
        _bannerdata = data;
        kLoadingSpr.gameObject.SetActive(false);
        kSelSpr.gameObject.SetActive(false);
        kBannerSpr.gameObject.SetActive(false);
        kBannerTex.gameObject.SetActive(true);
        kNoticeSpr.gameObject.SetActive(false);
        if(kCompleteObj != null)
            kCompleteObj.SetActive(false);


        //kBannerTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("banner", _bannerdata.UrlImage);
        GetTextuer(_bannerdata.UrlImage);
        SetTagMarkUI();
        //Log.Show(this.gameObject.name + " / " + _bannerdata.UrlImage);

        BannerSetup();
    }
    
    public void UpdateSlot(ePosType pos, int index, int selectId, UISpecialBuyPopup.SpecialBuyData data) 	//Fill parameter if you need
    {
        if (data == null)
        {
            return;
        }
        
        _pos = pos;
        _index = index;
        _bannerdata = data.Banner;
        kLoadingSpr.gameObject.SetActive(false);
        kSelSpr.gameObject.SetActive(false);

        if (data.UnexpectedPackageTableData.ID == selectId)
        {
            kSelSpr.gameObject.SetActive(true);
        }
        
        kBannerTex.gameObject.SetActive(_bannerdata != null);
        kBannerSpr.gameObject.SetActive(_bannerdata == null);
        
        kNoticeSpr.gameObject.SetActive(false);
        if (kCompleteObj != null)
        {
            kCompleteObj.SetActive(false);
        }

        kCharTex.SetActive(_bannerdata == null);
        kNameLabel.SetActive(_bannerdata == null);
        kDescLabel.SetActive(_bannerdata == null);
        
        if (_bannerdata != null)
        {
            GetTextuer(_bannerdata.UrlImage);
            SetTagMarkUI();
        }
        else
        {
            kBannerSpr.spriteName = $"Store_Package_Wakeup{data.UnexpectedPackageTableData.Value2}";
            kCharTex.mainTexture = GetMainSlotTexture(data.CharTableData);

            if (data.StoreDisplayGoodsTableData != null)
            {
                kNameLabel.textlocalize = FLocalizeString.Instance.GetText(data.StoreDisplayGoodsTableData.Name);
                kDescLabel.textlocalize = FLocalizeString.Instance.GetText(data.StoreDisplayGoodsTableData.Description);
            }
            
        }

        kInfoTimeObj.SetActive(true);
        _endDateTime = data.EndBuyTime;
        _unexpectedPackageTableId = data.UnexpectedPackageTableData.ID;
    }
    
    public void UpdateSlot(ePosType pos, int index, UIGachaPanel.GachaTabParam data)  //Fill parameter if you need
    {
        
        _pos = pos;
        _index = index;
        _gachatab = data;
        kLoadingSpr.gameObject.SetActive(false);
        kSelSpr.gameObject.SetActive(false);
        kNoticeSpr.gameObject.SetActive(false);
        kDesireGaugeObj.gameObject.SetActive(false);
        if (kCompleteObj != null)
            kCompleteObj.SetActive(false);

        if (kDesireIconSpr != null)
            kDesireIconSpr.SetActive(false);

        eGachaPickupType pickUpType = eGachaPickupType.None;
        if (data?.Value1 != null)
        {
            string[] splits = data.Value1.Split(',');
            foreach (string split in splits)
            {
                int.TryParse(split, out int result);
                if (result < 0)
                {
                    continue;
                }

                pickUpType |= (eGachaPickupType) (1 << result);
            }
        }

        bool isSale = (pickUpType & eGachaPickupType.Sale) == eGachaPickupType.Sale;
        kSaleSpr.SetActive(isSale);
        if (isSale)
            kLimitedSpr.SetActive(false);
        else
            kLimitedSpr.SetActive((pickUpType & eGachaPickupType.Limit) == eGachaPickupType.Limit);

        kRepeatSpr.SetActive((pickUpType & eGachaPickupType.Retro) == eGachaPickupType.Retro);

        string labelTypeStr = string.Empty;
        int labelTypeIndex = -1;
        if (_gachatab.Type.Contains("PICKUP"))
        {
            labelTypeStr = FLocalizeString.Instance.GetText(_gachatab.Name);
            if (kRepeatSpr.gameObject.activeSelf || kLimitedSpr.gameObject.activeSelf)
            {
                labelTypeIndex = 0;
            }
            else
            {
                labelTypeIndex = 1;
            }
        }
        else
        {
            labelTypeStr = FLocalizeString.Instance.GetText(_gachatab.Name);
            switch (_gachatab.Category)
            {
                case 10: case 11: case 12: case 15:
                    labelTypeIndex = 2;
                    break;
                case 13:
                    labelTypeIndex = 3;
                    break;
            }
        }

        for (int i = 0; i < kGachaLabelTypeList.Count; i++)
        {
            if (labelTypeIndex < 0)
            {
                break;
            }

            kGachaLabelTypeList[i].SetActive(i == labelTypeIndex);
            if (kGachaLabelTypeList[i].gameObject.activeSelf)
            {
                kGachaLabelTypeList[i].textlocalize = labelTypeStr;
            }
        }

        kDescriptionLabel.SetActive(_gachatab.Desc != 0);
        if (kDescriptionLabel.gameObject.activeSelf)
        {
            string descStr = FLocalizeString.Instance.GetText(_gachatab.Desc);
            if (_gachatab.Type.Equals("PICKUP2"))
            {
                GameClientTable.StoreDisplayGoods.Param storeDisplayGoodsParam = 
                    GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(_gachatab.StoreID1);
                if (storeDisplayGoodsParam != null)
                {
                    GameTable.Character.Param charParam = GameInfo.Instance.GameTable.FindCharacter(storeDisplayGoodsParam.CharacterID);
                    if (charParam != null)
                    {
                        descStr = FLocalizeString.Instance.GetText(_gachatab.Desc, FLocalizeString.Instance.GetText(charParam.Name));
                    }
                }
            }
            kDescriptionLabel.textlocalize = descStr;
        }

        if (_gachatab.Type == "GOLD")
        {
            if (GameSupport.IsStoreSaleApply(GameInfo.Instance.GameConfig.FreeGachaStoreID) || GameSupport.IsStoreSaleApply(GameInfo.Instance.GameConfig.SaleGoldGachaStoreID))
                kNoticeSpr.gameObject.SetActive(true);
        }
        else if (_gachatab.Type == "CASH")
        {
            if (GameSupport.IsStoreSaleApply(GameInfo.Instance.GameConfig.SalePremiumGachaStoreID))
                kNoticeSpr.gameObject.SetActive(true);

            if (kDesireIconSpr != null)
                kDesireIconSpr.SetActive(true);
        }
        else if (_gachatab.Type == "DESIRE")
        {
            kDesireGaugeObj.SetActive(true);
            kDesireGaugeMaxEff.SetActive(false);
            kDesireGaugeUnit.InitGaugeUnit((float)GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.DESIREPOINT] / (float)GameInfo.Instance.GameConfig.LimitMaxDP);
            if (GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.DESIREPOINT] >= GameInfo.Instance.GameConfig.LimitMaxDP)
                kDesireGaugeMaxEff.SetActive(true);
        }
        else if(_gachatab.Type == "ROTATION")
        {
            kDesireIconSpr.SetActive(true);
        }

        if (_gachatab.StoreID1 != -1 && _gachatab.StoreID2 != -1)
        {
            kBannerSpr.gameObject.SetActive(false);
            kBannerTex.gameObject.SetActive(true);
            GetTextuer(_gachatab.TabIcon);
        }
        else
        {
            kBannerSpr.gameObject.SetActive(true);
            kBannerTex.gameObject.SetActive(false);
            kBannerSpr.spriteName = data.TabIcon;
        }
        
        this.gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 1.0f);
        UIGachaPanel gachapanel = ParentGO.GetComponent<UIGachaPanel>();
        if (gachapanel != null)
        {
            if (gachapanel.SeleteTab == _index)
            {
                kSelSpr.gameObject.SetActive(true);

                this.gameObject.transform.localScale = Vector3.one;
            }
        }
        
        GachaSetup();
        
        GameClientTable.StoreDisplayGoods.Param disdata1 = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.ID == _gachatab.StoreID1);
        GameClientTable.StoreDisplayGoods.Param disdata2 = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.ID == _gachatab.StoreID2);
        if (disdata1 == null || disdata2 == null)
            return;

        GameTable.Store.Param storedata1 = GameInfo.Instance.GameTable.FindStore(x => x.ID == disdata1.StoreID);
        GameTable.Store.Param storedata2 = GameInfo.Instance.GameTable.FindStore(x => x.ID == disdata2.StoreID);
        if (storedata1 == null || storedata2 == null)
            return;

        int productIndex = storedata1 != null ? storedata1.ProductIndex : storedata2.ProductIndex;
        
        if (IsRemainGuerrilaReward(productIndex) == true)
            kNoticeSpr.gameObject.SetActive(true);

        if (storedata1.GetableDP != (int)eCOUNT.NONE || storedata2.GetableDP != (int)eCOUNT.NONE)
        {
            if(kDesireIconSpr != null)
                kDesireIconSpr.SetActive(true);
        }
    }

    private bool IsRemainGuerrilaReward(int productIndex) {
        List<GuerrillaMissionData> missionDataList = GameInfo.Instance.ServerData.GuerrillaMissionList.FindAll(x => x.Type == "GM_BuyStoreGacha_Cnt" && x.Condition == productIndex);
        if (null == missionDataList || missionDataList.Count <= 0)
            return false;

        missionDataList.Sort(new Comparison<GuerrillaMissionData>((n1, n2) => n1.GroupOrder.CompareTo(n2.GroupOrder)));

        for (int i = 0; i < missionDataList.Count; i++) {
            if (GameSupport.IsGuerrillaMissionTimeCheck(missionDataList[i]) == false)
                return false;
        }

        GllaMissionData userData = GameInfo.Instance.GllaMissionList.Find(x => x.GroupID == missionDataList[0].GroupID);
        if (userData == null)
            return false;

        for (int i = 0; i < missionDataList.Count; i++) {
            if (i + 1 < userData.Step)
                continue;

            if (missionDataList[i].Count <= userData.Count) {
                return true;
            }
        }

        return false;
    }

    public void UpdateSlot(ePosType pos, int index, int sel, GameClientTable.StoreDisplayGoods.Param data)    //Fill parameter if you need
    {
        _pos = pos;
        _index = index;
        _storedisplaygoods = data;
        kLoadingSpr.gameObject.SetActive(false);
        kSelSpr.gameObject.SetActive(false);
        kNoticeSpr.gameObject.SetActive(false);
        kBannerSpr.gameObject.SetActive(false);
        kBannerTex.gameObject.SetActive(true);

        if (kCompleteObj != null)
            kCompleteObj.SetActive(false);

        if (index == sel )
            kSelSpr.gameObject.SetActive(true);

        _bannerdata = GameInfo.Instance.ServerData.BannerList.Find(x => x.BannerTypeValue == _storedisplaygoods.StoreID && x.BannerType == (int)eBannerType.PACKAGE_BANNER);

        if (_bannerdata != null)
        {
            GetTextuer(_bannerdata.UrlImage);
            SetTagMarkUI();
        }
        else
        {
            kBannerTex.gameObject.SetActive(false);
            kBannerSpr.gameObject.SetActive(true);
            kBannerSpr.spriteName = _storedisplaygoods.Icon;
        }

        //구매완료 체크
        if (_storedisplaygoods.PackageUIType == (int)UIPackagePopup.ePackageUIType.CharJump)
        {
            GameTable.Store.Param storedata = GameInfo.Instance.GameTable.FindStore(x => x.ID == _storedisplaygoods.StoreID);
            if (storedata != null)
            {
                if (storedata.BuyNotStoreID > (int)eCOUNT.NONE)
                {
                    if (GameSupport.IsHaveStoreData(storedata.ID) || GameSupport.IsHaveStoreData(storedata.BuyNotStoreID))
                    {
                        if (kCompleteObj != null)
                            kCompleteObj.SetActive(true);
                    }
                }
                else
                {
					if ( kCompleteObj != null ) {
						List<GameClientTable.StoreDisplayGoods.Param> storeDisplayGoodsParamList =
							GameInfo.Instance.GameClientTable.FindAllStoreDisplayGoods( x => x.SubCategory == (_storedisplaygoods.SubCategory + 1000) );

						int buyCount = 0;
						for ( int i = 0; i < storeDisplayGoodsParamList.Count; i++ ) {
							if ( PackageBuyCheck( storeDisplayGoodsParamList[i].StoreID ) ) {
								++buyCount;
							}
						}
						kCompleteObj.SetActive( buyCount == storeDisplayGoodsParamList.Count );
					}
				}
            }
        }
        else if (_storedisplaygoods.PackageUIType == (int)UIPackagePopup.ePackageUIType.MonthlyFee)
        {
            if (GameSupport.IsRewardTypeWithStoreDisplayTable(data) == eREWARDTYPE.MONTHLYFEE)
            {
                GameTable.Store.Param storeData = GameInfo.Instance.GameTable.FindStore(x => x.ID == data.StoreID);
                if (storeData != null && GameSupport.IsHaveMonthlyData(storeData.ProductIndex) && !GameSupport.PremiumMonthlyDateFlag(storeData.ProductIndex))
                {
                    if (kCompleteObj != null)
                        kCompleteObj.SetActive(true);
                }
            }
            else
            {
                if (GameSupport.IsHaveStoreData(data.StoreID) && GameSupport.GetLimitedCnt(data.StoreID).Equals(0))
                {
                    if (kCompleteObj != null)
                        kCompleteObj.SetActive(true);
                }
            }
        }
        else if (_storedisplaygoods.PackageUIType == (int)UIPackagePopup.ePackageUIType.Rank || _storedisplaygoods.PackageUIType == (int)UIPackagePopup.ePackageUIType.Starter)
        {
            List<GameClientTable.StoreDisplayGoods.Param> displayAll = GameInfo.Instance.GameClientTable.FindAllStoreDisplayGoods(x => x.SubCategory == _storedisplaygoods.SubCategory + 1000);
            bool flag = true;
            if (displayAll != null)
            {
                for (int i = 0; i < displayAll.Count; i++)
                {
                    if (!GameSupport.IsHaveStoreData(displayAll[i].StoreID))
                    {
                        flag = false;
                        break;
                    }
                }

                if (flag)
                {
                    if (kCompleteObj != null)
                        kCompleteObj.SetActive(true);
                }
            }
        }
        else if (_storedisplaygoods.PackageUIType == (int)UIPackagePopup.ePackageUIType.Time)
        {
        }
        
        BannerSetup();
    }

    public void UpdateSlot(ePosType posType, int index, int selectIndex, GameClientTable.EventPage.Param eventPage)
    {
        if (eventPage == null)
        {
            return;
        }

        UpdateSlot(posType, index, selectIndex);

        kBannerSpr.SetActive(false);
        kBannerTex.SetActive(true);
        kBannerTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("ui", $"UI/UITexture/Event/{eventPage.TabIcon}.png");
        kEventBoardLabel.textlocalize = FLocalizeString.Instance.GetText(eventPage.Name);
    }

    private void GachaSetup()
    {
        if (_gachatab == null)
        {
            return;
        }

        if (!Enum.IsDefined(typeof(eGachaTabType), _gachatab.Type))
        {
            return;
        }
        
        Enum.TryParse(_gachatab.Type.ToUpper(), out eGachaTabType result);
        switch (result)
        {
            case eGachaTabType.PICKUP1: case eGachaTabType.PICKUP2:
            case eGachaTabType.PICKUP3: case eGachaTabType.PICKUP4:
                _SetGacha01();
                break;
            case eGachaTabType.ROTATION:
                _SetGacha02();
                break;
            case eGachaTabType.CASH: case eGachaTabType.GOLD:
                _SetGacha03();
                break;
        }
    }

    private void _SetGacha01()
    {
        // Value1 (유기적)
        // Name = 이름
        // Desc = 설명 & 혜택
    }
    
    private void _SetGacha02()
    {
        // Name = 이름
    }
    
    private void _SetGacha03()
    {
        // GameClient.GachaTab.Name = 이름
    }
    
    private void BannerSetup()
    {
        if (_bannerdata == null)
        {
            return;
        }
        
        for (int i = 0; i < kGachaLabelTypeList.Count; i++)
        {
            kGachaLabelTypeList[i].SetActive(false);
        }
        
        kDescriptionLabel.SetActive(false);

        Enum.TryParse(_bannerdata.FunctionValue1.ToUpper(), out eBannerFuncType result);

        bool bCharPackageActive = false;
        switch (result)
        {
            case eBannerFuncType.PACKAGEPOPUP:
            {
                BannerData packageBannerData = GameInfo.Instance.ServerData.BannerList.Find(x => 
                    x.BannerType == (int)eBannerType.PACKAGE_BG && x.BannerTypeValue == _bannerdata.BannerTypeValue);
                if (packageBannerData != null)
                {
                    if (Enum.IsDefined(typeof(eBannerFuncType), packageBannerData.FunctionValue2))
                    {
                        Enum.TryParse(packageBannerData.FunctionValue2.ToUpper(), out eBannerFuncType subResult);
                        switch (subResult)
                        {
                            case eBannerFuncType.CHAR:
                                bCharPackageActive = true;
                                _SetBanner01();
                                break;
                            case eBannerFuncType.LIMIT:
                                _SetBanner02();
                                break;
                        }
                    }
                }
                
                break;
            }
            case eBannerFuncType.EVENT_STORY_MAIN: case eBannerFuncType.EVENT_CHANGE_MAIN:
            {
                _SetBanner02();
                break;
            }
            case eBannerFuncType.GACHA: case eBannerFuncType.STORE:
            {
                if (Enum.IsDefined(typeof(eBannerFuncType), _bannerdata.FunctionValue2))
                {
                    Enum.TryParse(_bannerdata.FunctionValue2.ToUpper(), out eBannerFuncType subResult);
                    switch (subResult)
                    {
                        case eBannerFuncType.CASH: case eBannerFuncType.GOLD:
                            _SetBanner04(2);
                            break;
                        default:
                            bCharPackageActive = true;
                            _SetBanner03();
                            break;
                    }
                }
                else
                {
                    if (_bannerdata.FunctionValue2.Contains("PICKUP"))
                    {
                        var match = Regex.Match(_bannerdata.FunctionValue2, "[0-9]");
                        int.TryParse(match.Value, out int pickUpIndex);
                        if (0 < pickUpIndex)
                        {
                            if (pickUpIndex == 3)
                            {
                                pickUpIndex = 1;
                            }
                            
                            _SetBanner04(pickUpIndex - 1);
                        }
                    }
                }
                break;
            }
        }

        kCharPackageObj.SetActive(bCharPackageActive);
    }

    private void _SetBanner01()
    {
        StartCoroutine(GetTextureAsync(kCharPackageTex, _bannerdata.UrlAddImage1, false, _bannerdata.Localizes[(int)eBannerLocalizeType.Url]));
        kCharPackageLabel.textlocalize = FLocalizeString.Instance.GetText(_bannerdata.Name);
    }
    
    private void _SetBanner02()
    {
        
    }
    
    private void _SetBanner03()
    {
        kCharPackageLabel.textlocalize = FLocalizeString.Instance.GetText(_bannerdata.Name);
    }
    
    private void _SetBanner04(int index)
    {
        if (index < kGachaLabelTypeList.Count)
        {
            kGachaLabelTypeList[index].SetActive(true);
            kGachaLabelTypeList[index].textlocalize = FLocalizeString.Instance.GetText(_bannerdata.Name);
        }
        
        kDescriptionLabel.SetActive(true);
        kDescriptionLabel.textlocalize = FLocalizeString.Instance.GetText(_bannerdata.Desc);
    }
    
    private IEnumerator GetTextureAsync(UITexture texture, string url, bool platform, bool localize)
    {
        texture.mainTexture = null;
        
        while(true)
        {
            texture.mainTexture = BannerManager.Instance.LoadTextureWithFileURL(url, platform, localize);
            yield return null;
            
            if (texture.mainTexture != null)
            {
                break;
            }
        }
    }

    private bool PackageBuyCheck(int storeid)
    {
        return GameSupport.IsHaveStoreData(storeid) && GameSupport.GetLimitedCnt(storeid).Equals(0);
    }
    
    private void FixedUpdate()
    {
        if (!kInfoTimeObj.activeSelf)
        {
            return;
        }
        
        TimeSpan timeSpan = _endDateTime - GameSupport.GetCurrentServerTime();
        if (timeSpan.TotalSeconds <= 0)
        {
            UISpecialBuyPopup popup = ParentGO.GetComponent<UISpecialBuyPopup>();
            if (popup != null)
            {
                popup.DeleteSlot(_unexpectedPackageTableId);
            }
            
            return;
        }
        
        kInfoTimeLabel.textlocalize = FLocalizeString.Instance.GetText(232, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }
    
    private Texture GetMainSlotTexture(GameTable.Character.Param tableData)
    {
        if (tableData == null)
        {
            return null;
        }

        string path = "Icon/Char/MainSlot/MainSlot_" + tableData.Icon + "_" + tableData.InitCostume + ".png";
        return ResourceMgr.Instance.LoadFromAssetBundle("icon", path) as Texture;
    }

    void GetTextuer(string url)
    {
        if(GameInfo.Instance.netFlag)
        {
			if (kBannerTex.mainTexture != null)
			{
				DestroyImmediate(kBannerTex.mainTexture, false);
				kBannerTex.mainTexture = null;
			}

            bool localize = true;
            bool platform = false;
            if (_bannerdata != null)
            {
                localize = _bannerdata.Localizes[(int)eBannerLocalizeType.Url];
                platform = _bannerdata.BannerType == (int)eBannerType.PACKAGE_BG ||
                    _bannerdata.BannerType == (int)eBannerType.EVENT_MAINBG ||
                    _bannerdata.BannerType == (int)eBannerType.EVENT_STAGEBG ||
                    _bannerdata.BannerType == (int)eBannerType.EVENT_RULEBG ||
                    _bannerdata.BannerType == (int)eBannerType.GLLA_MISSION_LOGIN ||
                    _bannerdata.BannerType == (int)eBannerType.LOGIN_PACKAGE_BG ||
                    _bannerdata.BannerType == (int)eBannerType.SPECIAL_BUY ||
                    _bannerdata.BannerType == (int)eBannerType.UNEXPECTED_PACKAGE;
            }
            else if (_gachatab != null)
            {
                localize = _gachatab.Localize[(int)eGachaLocalizeType.Banner];
            }
            
            kBannerTex.mainTexture = BannerManager.Instance.LoadTextureWithFileURL(url, platform, localize);
            if (kBannerTex.mainTexture == null)
            {
                StartCoroutine(GetTextureAsync(url, localize));
            }
        }
        else
        {
            //Local Image Load
            Log.Show(url);
            kBannerTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("temp", "Temp/" + url);
        }
    }

    IEnumerator GetTextureAsync(string url, bool localizeFlag = true)
    {
        while(this.gameObject.activeSelf)
        {
            if (kBannerTex.mainTexture != null)
            {
                DestroyImmediate(kBannerTex.mainTexture, false);
                kBannerTex.mainTexture = null;
            }

            kBannerTex.mainTexture = BannerManager.Instance.LoadTextureWithFileURL(url, localizeFlag: localizeFlag);
            if (kBannerTex.mainTexture != null)
            {
                break;
            }
            yield return null;
        }
    }

    public void OnClick_Slot()
	{
        if (_pos == ePosType.Gacha)
        {
            UIGachaPanel gachapanel = ParentGO.GetComponent<UIGachaPanel>();
            if (gachapanel != null)
            {
                gachapanel.SetSeleteTab(_index);
            }
        }
        else if(_pos == ePosType.Event)
        {
            //GameSupport.MoveUI("UIPANEL", "EVENT_STORY_MAIN", _bannerdata.FunctionValue2, "");
            Log.Show("EEEE");
            GameSupport.MoveUI(_bannerdata.FunctionType, _bannerdata.FunctionValue1, _bannerdata.FunctionValue2, "");
        }
        else if (_pos == ePosType.Package)
        {
            UIPackagePopup packagepopup = ParentGO.GetComponent<UIPackagePopup>();
            if (packagepopup != null)
            {
                packagepopup.SetSeleteIndex(_index);
            }
            
            UISpecialBuyPopup specialBuyPopup = ParentGO.GetComponent<UISpecialBuyPopup>();
            if (specialBuyPopup != null)
            {
                specialBuyPopup.ChangePackageData(_index, _unexpectedPackageTableId);
            }
        }
        else if (_pos == ePosType.Mission)
        {
            UIPassMissionPopup popup = LobbyUIManager.Instance.GetActiveUI<UIPassMissionPopup>("PassMissionPopup");
            if (popup != null)
            {
                popup.SetPassTab(_index);
            }
        }
        else if (_pos == ePosType.RenewalEvent) 
        {
            UIEventBoardMainPanel eventPanel = ParentGO.GetComponent<UIEventBoardMainPanel>();
            if (eventPanel != null)
            {
                eventPanel.SelectBanner(_index);
            }
        }
        else
        {
            //패키지 검사
            if(_bannerdata.FunctionValue1 == "PackagePopup")
            {
                GameSupport.PaymentAgreement_Package(int.Parse(_bannerdata.FunctionValue2));
            }
            else
            {
                GameSupport.MoveUI(_bannerdata.FunctionType, _bannerdata.FunctionValue1, _bannerdata.FunctionValue2, _bannerdata.FunctionValue3);
            }
        }
    }
 
    private void SetTagMarkUI() {
        bool isSale = (_bannerdata.TagMark & eBannerFuntionValue3Flag.Sale) == eBannerFuntionValue3Flag.Sale;
        kSaleSpr.SetActive(isSale);
        if (isSale == true)
            kRepeatSpr.SetActive(false);
        else
            kRepeatSpr.SetActive((_bannerdata.TagMark & eBannerFuntionValue3Flag.Retro) == eBannerFuntionValue3Flag.Retro);

        kLimitedSpr.SetActive((_bannerdata.TagMark & eBannerFuntionValue3Flag.Limit) == eBannerFuntionValue3Flag.Limit);
    }
}
