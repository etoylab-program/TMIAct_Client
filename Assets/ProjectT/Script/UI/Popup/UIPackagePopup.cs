using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIPackagePopup : FComponent
{
    public enum ePackageCategory
    {
        New_Package = 1,
        Char_Package,
        Time_Package,
    }

    public enum ePackageBtnType
    {
        NONE = 0,
        CategoryBtn,
        PackageItem,
    }

    public enum ePackageUIType
    {
        NONE = 0,
        CharJump,
        Starter,
        Rank,
        Time,
        MonthlyFee,
        Gacha,
        Scenario,
        Wellcome,
        WakeUp,
    }

    public enum eTimePackageType
    {
        NONE = 0,
        Day,
        Weekly,
        Monthly,
    }

    public class PackageBtnData
    {
        public ePackageBtnType kBtnType = ePackageBtnType.NONE;
        public ePackageCategory kCategoryType;
        public GameClientTable.StoreDisplayGoods.Param kPackageItem = null;

        public PackageBtnData(ePackageBtnType btnType, ePackageCategory categoryType, GameClientTable.StoreDisplayGoods.Param itemdata)
        {
            kBtnType = btnType;
            kCategoryType = categoryType;
            kPackageItem = itemdata;
        }
    }
    
    [Serializable]
    public class CharPackageTypeParam
    {
        public int kType;
        public GameObject kObj;
        public UILabel kLabel;
    }
    
    [Header("Renewal")]
    public UITexture kPackageTex;
    public UITexture kCharaPackageTex;
    public GameObject kPackageTexLoadingObj;

    [SerializeField] private FList _btnList;
    private int _selBtnIndex = -1;
    private PackageBtnData _selCategoryData;
    private PackageBtnData _selBtnData;
    private List<PackageBtnData> _packageList = new List<PackageBtnData>();
    private ePackageCategory kSelectCategory = ePackageCategory.New_Package;

    [Header("NormalPackage")]
    public GameObject kNormalPackageRoot;
    public GameObject kPremiumEffectObj;
    public UIPackageSlot kPackageSlot01;
    public UIPackageSlot kPackageSlot02;
    public UIPackageSlot kPackageSlot03;

    [Header("StarterPackage")]
    public GameObject kStarterPackageRoot;
    [SerializeField] private FList _starterPackageList;
    private List<GameClientTable.StoreDisplayGoods.Param> _staterDataList = new List<GameClientTable.StoreDisplayGoods.Param>();

    [Header("RankPackage")]
    public GameObject kRankPackageRoot;

    [Header("TimePackage")]
    public GameObject kTimePackageRoot;
    [SerializeField] private FList _timePackageList;
    private List<GameClientTable.StoreDisplayGoods.Param> _timeDataList = new List<GameClientTable.StoreDisplayGoods.Param>();

    [Header("MonthlyFeePackage")]
    public GameObject kMonthlyFeePackageRoot;

    [Header("Notice Property")]
    public UISprite kNoticeType_DateSpr;
    public UILabel kNoticeType_DateLabel;
    public UISprite kLimitedTimeSpr;
    public UITexture kLogoTex;
    
    public int PackageStoreID { get; set; }

    [Header("GachaPackage Property")]
    public GameObject kPackageBtns;
    public GameObject kGachaBtns;
    //public UILabel kGachaDateLabel;
    //public UILabel kGachaDescLabel;
    public GameObject kGachaPriceOnObj;
    public GameObject kGachaPriceOffObj;

    public UIButton kGachaBuyBtn;
    public UILabel kGachaBuyLabel;
    public GameObject kGachaBuyDimdObj;

    public GameObject kGachaLimitedObj;
    public UILabel kGachaLimitLb;

    public UILabel kGachaCountLb;
    public UILabel kGachaCountDescLb;

    public GameObject kGachaRemainObj;
    public UILabel kGachaRemainLabel;

    [Header("RankPackage")]
    public GameObject kRankPackageInfoObjs;

    [Header("Char Package Name")]
    public GameObject kCharPackageNameFrame;
    public List<CharPackageTypeParam> kCharPackageTypeList;

    private int _seleteindex;
    private List<CardBookData> _precardbooklist = new List<CardBookData>();

    private Coroutine _bgLoadCoroutine = null;

    private bool _bIsGacha = false;
    private DateTime _nowTime;

    private int _buyStoreID = (int)eCOUNT.NONE;

    public override void Awake()
	{
		base.Awake();

        if (_btnList == null) return;
        _btnList.EventUpdate = _UpdatePackageListSlot;
        _btnList.EventGetItemCount = _GetPackageElementCount;
        _btnList.InitBottomFixing();

        if (_starterPackageList == null) return;
        _starterPackageList.EventUpdate = _UpdateStarterPackageListSlot;
        _starterPackageList.EventGetItemCount = _GetStarterPackageElementCount;
        _starterPackageList.InitBottomFixing();

        if (_timePackageList == null) return;
        _timePackageList.EventUpdate = _UpdateTimePackageListSlot;
        _timePackageList.EventGetItemCount = _GetTimePackageElementCount;
        _timePackageList.InitBottomFixing();
    }

    

    public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}

	public override void InitComponent()
	{
        Log.Show(PackageStoreID > (int)eCOUNT.NONE);
        _buyStoreID = (int)eCOUNT.NONE;
        _nowTime = GameInfo.Instance.GetNetworkTime();
        
        string charName = string.Empty;
        int characterID = -1;
        GameClientTable.StoreDisplayGoods.Param storeDisplayGoods = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == PackageStoreID);
        if (storeDisplayGoods != null)
        {
            GameTable.Character.Param characterParam = GameInfo.Instance.GameTable.FindCharacter(storeDisplayGoods.CharacterID);
            if (characterParam != null)
            {
                charName = characterParam.Icon.ToLower();
				characterID = characterParam.ID;
			}
        }

        SetPackageBtnList();

        if (!string.IsNullOrEmpty(charName) && 0 < characterID)
        {
            PickCharacter( charName, characterID );
        }

        if (!IAPManager.Instance.IAPNULL_CHECK())
        {
            MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3197), null);
            return;
        }

        kPackageSlot01.ParentGO = this.gameObject;
        kPackageSlot02.ParentGO = this.gameObject;
        kPackageSlot03.ParentGO = this.gameObject;

        IAPManager.Instance.IsBuying = false;
    }

	public override void Renewal(bool bChildren)
	{
        base.Renewal(bChildren);
        if (_selBtnData == null)
            return;
        _buyStoreID = (int)eCOUNT.NONE;
        _nowTime = GameInfo.Instance.GetNetworkTime();
        
        if (_selBtnData.kPackageItem == null)
        {
            return;
        }

        Log.Show((ePackageUIType)_selBtnData.kPackageItem.PackageUIType);
        BannerData packageBannerData = GameInfo.Instance.ServerData.BannerList.Find(x => x.BannerType == (int)eBannerType.PACKAGE_BG && x.BannerTypeValue == _selBtnData.kPackageItem.StoreID);

        if (packageBannerData == null)
            return;

        bool bLocalize = packageBannerData.Localizes[(int)eBannerLocalizeType.Url];
        if (_selBtnData.kPackageItem.PackageUIType == (int)ePackageUIType.Rank)
        {
            List<GameClientTable.StoreDisplayGoods.Param> rankpackageList = GameInfo.Instance.GameClientTable.FindAllStoreDisplayGoods(x => x.SubCategory == _selBtnData.kPackageItem.SubCategory + 1000);
            if (rankpackageList == null)
            {
                packageBannerData = GameInfo.Instance.ServerData.BannerList.Find(x => x.BannerType == (int)eBannerType.PACKAGE_BG && x.BannerTypeValue == _selBtnData.kPackageItem.StoreID);
                GetBGTexture(kPackageTex, packageBannerData.UrlImage, localize: bLocalize);
            }
            else
            {
                bool rankflag = false;

                if (!GameSupport.IsHaveStoreData(rankpackageList[(int)eCOUNT.NONE].StoreID))
                {
                    GameTable.Store.Param packStoreData = GameInfo.Instance.GameTable.FindStore(x => x.ID == rankpackageList[(int)eCOUNT.NONE].StoreID);
                    if (packStoreData != null)
                    {
                        packageBannerData = GameInfo.Instance.ServerData.BannerList.Find(x => x.BannerType == (int)eBannerType.PACKAGE_BG && x.BannerTypeValue == packStoreData.ID);
                        GetBGTexture(kPackageTex, packageBannerData.UrlImage, localize: bLocalize);
                    }
                }
                else
                {
                    for (int i = 1; i < rankpackageList.Count; i++)
                    {
                        Log.Show(rankpackageList[i].StoreID, Log.ColorType.Red);
                        if (GameSupport.IsHaveStoreData(rankpackageList[i].StoreID))
                            continue;

                        GameTable.Store.Param packStoreData = GameInfo.Instance.GameTable.FindStore(x => x.ID == rankpackageList[i].StoreID);
                        if (packStoreData != null)
                        {
                            if (GameSupport.IsHaveStoreData(packStoreData.NeedBuyStoreID))
                            {
                                rankflag = true;
                                packageBannerData = GameInfo.Instance.ServerData.BannerList.Find(x => x.BannerType == (int)eBannerType.PACKAGE_BG && x.BannerTypeValue == packStoreData.ID);
                                GetBGTexture(kPackageTex, packageBannerData.UrlImage, localize: bLocalize);
                                break;
                            }
                        }
                    }

                    if (!rankflag)
                    {
                        packageBannerData = GameInfo.Instance.ServerData.BannerList.Find(x => x.BannerType == (int)eBannerType.PACKAGE_BG && x.FunctionValue2.Equals("BUY") &&
                                                                                        x.BannerTypeValue == rankpackageList[rankpackageList.Count - 1].StoreID);
                        GetBGTexture(kPackageTex, packageBannerData.UrlImage, localize: bLocalize);
                    }
                }

                

            }
        }
        else
        {
            GetBGTexture(kPackageTex, packageBannerData.UrlImage, localize: bLocalize);
        }

        bool bLogoEnable = false;
        
        kCharPackageNameFrame.SetActive(false);

        Enum.TryParse(packageBannerData.FunctionValue2.ToUpper(), out eBannerFuncType subResult);
        kCharaPackageTex.SetActive(subResult == eBannerFuncType.CHAR);
        
        switch (subResult)
        {
            case eBannerFuncType.CHAR:
                GetBGTexture(kCharaPackageTex, packageBannerData.UrlAddImage1, localize: packageBannerData.Localizes[(int)eBannerLocalizeType.AddUrl1]);

                if (!string.IsNullOrEmpty(packageBannerData.UrlAddImage2))
                {
                    bLogoEnable = true;
                    GetBGTexture(kLogoTex, packageBannerData.UrlAddImage2, false, packageBannerData.Localizes[(int)eBannerLocalizeType.AddUrl2]);
                }
                else
                {
                    kCharPackageNameFrame.SetActive(true);

                    GameClientTable.StoreDisplayGoods.Param sdgParam =
                    GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == packageBannerData.BannerTypeValue);
                    if (sdgParam != null)
                    {
                        GameTable.Character.Param cParam = GameInfo.Instance.GameTable.FindCharacter(sdgParam.CharacterID);
                        if (cParam != null)
                        {
                            foreach (CharPackageTypeParam data in kCharPackageTypeList)
                            {
                                data.kObj.SetActive(data.kType == cParam.Type);
                                if (!data.kObj.activeSelf)
                                {
                                    continue;
                                }
                                data.kLabel.textlocalize = FLocalizeString.Instance.GetText(packageBannerData.Name);
                            }
                        }
                    }
                } break;
            case eBannerFuncType.LIMIT:
                if (!string.IsNullOrEmpty(packageBannerData.UrlAddImage2))
                {
                    bLogoEnable = true;
                    GetBGTexture(kLogoTex, packageBannerData.UrlAddImage2, false, packageBannerData.Localizes[(int)eBannerLocalizeType.AddUrl2]);
                } break;
        }
        
        kLogoTex.gameObject.SetActive(bLogoEnable);
        
        kGachaBtns.SetActive(false);
        kPremiumEffectObj.SetActive(false);
        kNormalPackageRoot.SetActive(false);
        kStarterPackageRoot.SetActive(false);
        kTimePackageRoot.SetActive(false);

        kPackageSlot01.gameObject.SetActive(false);
        kPackageSlot02.gameObject.SetActive(false);
        kPackageSlot03.gameObject.SetActive(false);

        kNoticeType_DateSpr.gameObject.SetActive(false);

        kRankPackageInfoObjs.SetActive(false);

        _bIsGacha = false;

        _btnList.RefreshNotMove();

        bool bLimitedTimeEnable = false;

        if (_selBtnData.kPackageItem.PackageUIType == (int)ePackageUIType.CharJump)
        {
            GameClientTable.StoreDisplayGoods.Param storeDisplayGoods = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x =>
                x.StoreID == _selBtnData.kPackageItem.StoreID && x.PackageUIType == 0);
            bLimitedTimeEnable = storeDisplayGoods.AdvancedPackage == (int) CharPackageType.Limit;

			kNoticeType_DateSpr.gameObject.SetActive( storeDisplayGoods.LimitType != (int)ePackageLimitType.NONE );
            if ( kNoticeType_DateSpr.gameObject.activeSelf ) {
				kNoticeType_DateLabel.textlocalize = GameSupport.GetEndTime( GameSupport.GetMinusOneMinuteEndTime( packageBannerData.EndDate ) );
			}

            kNormalPackageRoot.SetActive(true);
            SetNormalPackageBtn(_selBtnData);
        }
        else if (_selBtnData.kPackageItem.PackageUIType == (int)ePackageUIType.Rank)
        {
            kRankPackageInfoObjs.SetActive(true);
            kNormalPackageRoot.SetActive(true);
            SetNormalPackageBtn(_selBtnData);
        }
        else if (_selBtnData.kPackageItem.PackageUIType == (int)ePackageUIType.MonthlyFee)
        {
            kNormalPackageRoot.SetActive(true);
            SetNormalPackageBtn(_selBtnData);
        }
        else if (_selBtnData.kPackageItem.PackageUIType == (int)ePackageUIType.Starter)
        {
            kStarterPackageRoot.SetActive(true);
            int subCaegoryValue = _selBtnData.kPackageItem.SubCategory + 1000;
            List<GameClientTable.StoreDisplayGoods.Param> staterList = GameInfo.Instance.GameClientTable.FindAllStoreDisplayGoods(x => x.SubCategory == subCaegoryValue);
            if (staterList != null)
            {
                _staterDataList.Clear();
                staterList.Sort(CompareFuncStoreDisplayGoodsStoreID);
                for (int i = 0; i < staterList.Count; i++)
                {
                    _staterDataList.Add(staterList[i]);
                }
                _starterPackageList.UpdateList();
            }
        }
        else if (_selBtnData.kPackageItem.PackageUIType == (int)ePackageUIType.Time)
        {
            DateTime serverTime = GameSupport.GetCurrentRealServerTime();
            kTimePackageRoot.SetActive(true);

            int subCaegoryValue = _selBtnData.kPackageItem.SubCategory + 1000;
            List<GameClientTable.StoreDisplayGoods.Param> timeList = GameInfo.Instance.GameClientTable.FindAllStoreDisplayGoods(x => x.SubCategory == subCaegoryValue);
            if (timeList != null)
            {
                _timeDataList.Clear();
                timeList.Sort(CompareFuncStoreDisplayGoodsStoreID);
                for (int i = 0; i < timeList.Count; i++)
                {
                    _timeDataList.Add(timeList[i]);
                }
                _timePackageList.UpdateList();
            }

            kNoticeType_DateSpr.gameObject.SetActive(true);
            
            if (_selBtnData.kPackageItem.SubCategory == (int)eTimePackageType.Day)
            {
                DateTime temp = _nowTime.AddDays(1);
                DateTime endtime = new DateTime(temp.Year, temp.Month, temp.Day);
                kNoticeType_DateLabel.textlocalize = GameSupport.GetPackageEndTime(endtime);
            }
            else if (_selBtnData.kPackageItem.SubCategory == (int)eTimePackageType.Weekly)
            {
                
                DateTime endDayTime;

                if ((int)serverTime.DayOfWeek == GameInfo.Instance.GameConfig.WeeklyResetDay)
                    endDayTime = serverTime.AddDays((int)eDayOfWeek._END_DAY_);
                else
                    endDayTime = serverTime.AddDays((int)eDayOfWeek._END_DAY_ - (int)serverTime.DayOfWeek);

                DateTime endtime = new DateTime(endDayTime.Year, endDayTime.Month, endDayTime.Day, GameInfo.Instance.GameConfig.WeeklyResetTime, 0, 0);
                endtime.AddMinutes(-2);
                kNoticeType_DateLabel.textlocalize = GameSupport.GetPackageEndTime(GameSupport.GetLocalTimeByServerTime(endtime));
            }
            else if (_selBtnData.kPackageItem.SubCategory == (int)eTimePackageType.Monthly)
            {
                DateTime temp = _nowTime.AddMonths(1);
                DateTime endtime = new DateTime(temp.Year, temp.Month, 1);
                endtime.AddMinutes(-2);
                kNoticeType_DateLabel.textlocalize = GameSupport.GetPackageEndTime(GameSupport.GetLocalTimeByServerTime(endtime));
            }

        }
        else if (_selBtnData.kPackageItem.PackageUIType == (int)ePackageUIType.Gacha)
        {
            GameTable.Store.Param gachaStore = GameInfo.Instance.GameTable.FindStore(x => x.ID == _selBtnData.kPackageItem.StoreID);

            kGachaBtns.SetActive(true);

            kNoticeType_DateSpr.gameObject.SetActive(true);
            kNoticeType_DateLabel.textlocalize = GameSupport.GetEndTime(GameSupport.GetMinusOneMinuteEndTime(packageBannerData.EndDate));

            kGachaCountLb.textlocalize = gachaStore.ProductValue.ToString();
            kGachaCountDescLb.textlocalize = FLocalizeString.Instance.GetText(1087);

            //if (!string.IsNullOrEmpty(packageBannerData.FunctionValue1))
            //{
            //    kGachaDescLabel.SetActive(true);
            //    kGachaDescLabel.textlocalize = FLocalizeString.Instance.GetText(int.Parse(packageBannerData.FunctionValue1));
            //}
            _bIsGacha = true;
            SetGachaPriceBtn(gachaStore.ID);
        }
        
        kLimitedTimeSpr.gameObject.SetActive((packageBannerData.TagMark & eBannerFuntionValue3Flag.Limit) == eBannerFuntionValue3Flag.Limit);
        
        switch (kSelectCategory)
        {
            case ePackageCategory.New_Package:
                {
                    if (!string.IsNullOrEmpty(packageBannerData.FunctionValue1) && !string.IsNullOrEmpty(packageBannerData.FunctionValue2))
                    {
                        kNoticeType_DateSpr.gameObject.SetActive(subResult != eBannerFuncType.CHAR); // 추후 On/Off로 변경될 수도 있음
                        kNoticeType_DateLabel.textlocalize = GameSupport.GetEndTime(GameSupport.GetMinusOneMinuteEndTime(packageBannerData.EndDate));
                    }
                }
                break;
            case ePackageCategory.Char_Package:
                {

                }
                break;
            case ePackageCategory.Time_Package:
                {

                }
                break;                
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();

        kSelectCategory = ePackageCategory.New_Package;
    }

    private void SetNormalPackageBtn(PackageBtnData data)
    {
        if (data.kPackageItem.PackageUIType == (int)ePackageUIType.CharJump)
        {
            int subCategoryValue = data.kPackageItem.SubCategory + 1000;
            List<GameClientTable.StoreDisplayGoods.Param> subList = GameInfo.Instance.GameClientTable.FindAllStoreDisplayGoods(x => x.SubCategory == subCategoryValue);
            if (subList == null)
                return;

            if (subList.Count == 1)
            {
                kPackageSlot03.gameObject.SetActive(true);
                kPackageSlot03.UpdateSlot(_selCategoryData, ePackageUIType.CharJump, subList[0]);
            }
            else if (subList.Count == 2)
            {
                kPackageSlot02.gameObject.SetActive(true);
                kPackageSlot03.gameObject.SetActive(true);
                kPackageSlot02.UpdateSlot(_selCategoryData, ePackageUIType.CharJump, subList[0]);
                kPackageSlot03.UpdateSlot(_selCategoryData, ePackageUIType.CharJump, subList[1]);
            }
            else if (subList.Count == 3)
            {
                kPackageSlot01.gameObject.SetActive(true);
                kPackageSlot02.gameObject.SetActive(true);
                kPackageSlot03.gameObject.SetActive(true);

                kPackageSlot01.UpdateSlot(_selCategoryData, ePackageUIType.CharJump, subList[0]);
                kPackageSlot02.UpdateSlot(_selCategoryData, ePackageUIType.CharJump, subList[1]);
                kPackageSlot03.UpdateSlot(_selCategoryData, ePackageUIType.CharJump, subList[2]);
            }
            for (int i = 0; i < subList.Count; i++)
            {
                if (subList[i].AdvancedPackage > (int)eCOUNT.NONE)
                {
                    kPremiumEffectObj.SetActive(true);
                    break;
                }
            }
        }
        else if (data.kPackageItem.PackageUIType == (int)ePackageUIType.Rank)
        {
            kPackageSlot03.gameObject.SetActive(true);
            List<GameClientTable.StoreDisplayGoods.Param> rankpackageList = GameInfo.Instance.GameClientTable.FindAllStoreDisplayGoods(x => x.SubCategory == _selBtnData.kPackageItem.SubCategory + 1000);
            if (rankpackageList == null)
            {
                kPackageSlot03.UpdateSlot(_selCategoryData, ePackageUIType.MonthlyFee, data.kPackageItem);
            }
            else
            {
                int checkIndex = (int)eCOUNT.NONE;
                for (int i = 0; i < rankpackageList.Count; i++)
                {
                    Log.Show(rankpackageList[i].StoreID, Log.ColorType.Red);
                    if (GameSupport.IsHaveStoreData(rankpackageList[i].StoreID))
                    {
                        if (i >= rankpackageList.Count - 1)
                        {
                            GameTable.Store.Param packLastStoreData = GameInfo.Instance.GameTable.FindStore(x => x.ID == rankpackageList[i].StoreID);
                            if (packLastStoreData != null)
                            {
                                if (GameSupport.IsHaveStoreData(packLastStoreData.NeedBuyStoreID))
                                {
                                    checkIndex = i;
                                    kPackageSlot03.UpdateSlot(_selCategoryData, ePackageUIType.Rank, rankpackageList[checkIndex]);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                        

                    GameTable.Store.Param packStoreData = GameInfo.Instance.GameTable.FindStore(x => x.ID == rankpackageList[i].StoreID);
                    if (packStoreData != null)
                    {
                        if (GameSupport.IsHaveStoreData(packStoreData.NeedBuyStoreID))
                        {
                            checkIndex = i;
                            kPackageSlot03.UpdateSlot(_selCategoryData, ePackageUIType.Rank, rankpackageList[checkIndex]);
                            break;
                        }
                    }
                }

                if (checkIndex <= (int)eCOUNT.NONE)
                {
                    kPackageSlot03.UpdateSlot(_selCategoryData, ePackageUIType.Rank, rankpackageList[(int)eCOUNT.NONE]);
                }

            }
        }
        else if (data.kPackageItem.PackageUIType == (int)ePackageUIType.MonthlyFee)
        {
            kPackageSlot03.gameObject.SetActive(true);

            

            kPackageSlot03.UpdateSlot(_selCategoryData, ePackageUIType.MonthlyFee, data.kPackageItem);
        }
        else if (data.kPackageItem.PackageUIType == (int)ePackageUIType.Gacha)
        {

        }
    }

    private void _UpdatePackageListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIPackageBtnSlot card = slotObject.GetComponent<UIPackageBtnSlot>();
            if (null == card) break;
            card.ParentGO = this.gameObject;

            card.UpdateSlot(index, _selBtnIndex, kSelectCategory, _packageList[index]);

        } while (false);


    }

    private int _GetPackageElementCount()
    {
        if (_packageList == null)
            return (int)eCOUNT.NONE;

        return _packageList.Count;
    }

    private void _UpdateStarterPackageListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIPackageSlot slot = slotObject.GetComponent<UIPackageSlot>();
            if (null == slot) break;
            slot.ParentGO = this.gameObject;

            slot.UpdateSlot(_selCategoryData, ePackageUIType.Starter, _staterDataList[index]);
        } while (false);


    }

    private int _GetStarterPackageElementCount()
    {
        if (_staterDataList == null)
            return (int)eCOUNT.NONE;

        return _staterDataList.Count;
    }

    private void _UpdateTimePackageListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIPackageSlot slot = slotObject.GetComponent<UIPackageSlot>();
            if (null == slot) break;
            slot.ParentGO = this.gameObject;
            slot.UpdateSlot(_selBtnData, ePackageUIType.Time, _timeDataList[index]);
        } while (false);


    }

    private int _GetTimePackageElementCount()
    {
        if (_timeDataList == null)
            return (int)eCOUNT.NONE;

        return _timeDataList.Count;
    }

    private void AddBtnList()
    {
        List<GameClientTable.StoreDisplayGoods.Param> list = new List<GameClientTable.StoreDisplayGoods.Param>();
        if (kSelectCategory.Equals(ePackageCategory.New_Package) || kSelectCategory.Equals(ePackageCategory.Char_Package))
        {
            //신규, 캐릭터, 상시 패키지
            list = GameInfo.Instance.GameClientTable.FindAllStoreDisplayGoods(x => x.PanelType == (int)eSD_PanelType.PACKAGE && x.Category == (int)ePackageCategory.New_Package && x.PackageUIType > 0);
        }
        else if (kSelectCategory.Equals(ePackageCategory.Time_Package))
        {
            //강화 패키지
            list = GameInfo.Instance.GameClientTable.FindAllStoreDisplayGoods(x => x.PanelType == (int)eSD_PanelType.PACKAGE && x.Category == (int)ePackageCategory.Time_Package - 1 && x.PackageUIType > 0);
        }

        list.Sort(CompareFuncStoreDisplayGoods);

        for (int i = 0; i < list.Count; i++)
        {
            if (kSelectCategory == ePackageCategory.New_Package)
            {
                if (!GameSupport.IsShowStoreDisplay(list[i]) && !GameSupport.IsHaveStoreData(list[i].StoreID))
                    continue;
            }

            BannerData packageBannerData = GameInfo.Instance.ServerData.BannerList.Find(x => x.BannerType == (int)eBannerType.PACKAGE_BG && x.BannerTypeValue == list[i].StoreID);
            if (packageBannerData != null)
            {
                if (kSelectCategory == ePackageCategory.Char_Package)
                {
                    if (packageBannerData.FunctionValue2.ToLower().Equals("char"))
                    {
                        DateTime bannerEndDate = packageBannerData.EndDate;
                        if (bannerEndDate.Ticks < _nowTime.Ticks)
                        {
                            _packageList.Add(new PackageBtnData(ePackageBtnType.PackageItem, ePackageCategory.Char_Package, list[i]));
                        }
                    }
                }
                else if (kSelectCategory == ePackageCategory.New_Package || kSelectCategory == ePackageCategory.Time_Package)
                {
                    DateTime bannerStartDate = packageBannerData.StartDate;
                    DateTime bannerEndDate = packageBannerData.EndDate;
                    if (bannerStartDate.Ticks <= _nowTime.Ticks && bannerEndDate.Ticks >= _nowTime.Ticks)
                    {
                        GameTable.Store.Param storeTableData = GameInfo.Instance.GameTable.FindStore(x => x.ID == list[i].StoreID);
                        if (storeTableData == null)
                        {
                            _packageList.Add(new PackageBtnData(ePackageBtnType.PackageItem, ePackageCategory.New_Package, list[i]));
                        }
                        else
                        {
                            if (storeTableData.ProductType == (int)eREWARDTYPE.MONTHLYFEE)
                            {
                                AddMonthlyPackage(storeTableData, list[i]);
                            }
                            else
                            {
                                _packageList.Add(new PackageBtnData(ePackageBtnType.PackageItem, ePackageCategory.New_Package, list[i]));
                            }
                        }
                        
                    }
                        
                }
            }
            else
            {
                if (kSelectCategory == ePackageCategory.Char_Package)
                    continue;

                if (kSelectCategory == ePackageCategory.New_Package)
                {
                    GameTable.Store.Param storeTableData = GameInfo.Instance.GameTable.FindStore(x => x.ID == list[i].StoreID);
                    if (storeTableData == null)
                        continue;
                    if (storeTableData.ProductType == (int)eREWARDTYPE.MONTHLYFEE)
                    {
                        AddMonthlyPackage(storeTableData, list[i]);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(list[i].Icon))
                            _packageList.Add(new PackageBtnData(ePackageBtnType.PackageItem, ePackageCategory.New_Package, list[i]));
                    }
                }
                else
                {
                    _packageList.Add(new PackageBtnData(ePackageBtnType.PackageItem, ePackageCategory.Time_Package, list[i]));
                }
            }
        }
    }

    private void AddMonthlyPackage(GameTable.Store.Param monthlyStore, GameClientTable.StoreDisplayGoods.Param monthlyClient)
    {
        //우종훈 파트장님과 상의하에 StoreID 하드 코딩
        if (GameSupport.PremiumMonthlyDateFlag(monthlyStore.ProductIndex))
        {
            //프리미엄 월정액 구매 했으면 2023
            if (monthlyClient.StoreID == 2023)
            {
                PackageBtnData packagedata = new PackageBtnData(ePackageBtnType.PackageItem, ePackageCategory.New_Package, monthlyClient);
                if (!_packageList.Contains(packagedata))
                {
                    _packageList.Add(packagedata);
                }
            }
        }
        else
        {
            //프리미엄 월정액 구매 안했으면 2022
            if (monthlyClient.StoreID == 2022)
            {
                PackageBtnData packagedata = new PackageBtnData(ePackageBtnType.PackageItem, ePackageCategory.New_Package, monthlyClient);
                if (!_packageList.Contains(packagedata))
                    _packageList.Add(packagedata);
            }
        }
    }

    private void SetPackageBtnList(bool updateList = true)
    {
        _packageList.Clear();
        Log.Show("PackageStoreID : " + PackageStoreID, Log.ColorType.Red);
        if (PackageStoreID != (int)eCOUNT.NONE)
        {
            GameClientTable.StoreDisplayGoods.Param packageParam = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == PackageStoreID);
            if (packageParam.Category == ((int)ePackageCategory.Time_Package) - 1)
                kSelectCategory = ePackageCategory.Time_Package;
            else
            {
                BannerData packageBannerData = GameInfo.Instance.ServerData.BannerList.Find(x => x.BannerType == (int)eBannerType.PACKAGE_BG && x.BannerTypeValue == PackageStoreID);
                if (packageBannerData == null)
                {
                    kSelectCategory = (ePackageCategory)packageParam.Category;
                }
                else
                {
                    if (packageBannerData.FunctionValue2.ToLower().Equals("char"))
                    {
                        DateTime bannerEndDate = packageBannerData.EndDate;
                        if (bannerEndDate.Ticks < _nowTime.Ticks)
                        {
                            kSelectCategory = ePackageCategory.Char_Package;
                        }
                        else
                        {
                            kSelectCategory = (ePackageCategory)packageParam.Category;
                        }
                    }
                    else
                    {
                        kSelectCategory = (ePackageCategory)packageParam.Category;
                    }
                    
                }
                
            }
                
        }
        

        _packageList.Add(new PackageBtnData(ePackageBtnType.CategoryBtn, ePackageCategory.New_Package, null));
        if (kSelectCategory.Equals(ePackageCategory.New_Package))
        {
            _selCategoryData = _packageList[_packageList.Count - 1];
            _selBtnIndex = _packageList.Count;
            AddBtnList();
        }
        _packageList.Add(new PackageBtnData(ePackageBtnType.CategoryBtn, ePackageCategory.Char_Package, null));
        if (kSelectCategory.Equals(ePackageCategory.Char_Package))
        {
            _selCategoryData = _packageList[_packageList.Count - 1];
            _selBtnIndex = _packageList.Count;
            AddBtnList();
        }
        _packageList.Add(new PackageBtnData(ePackageBtnType.CategoryBtn, ePackageCategory.Time_Package, null));
        if (kSelectCategory.Equals(ePackageCategory.Time_Package))
        {
            _selCategoryData = _packageList[_packageList.Count - 1];
            _selBtnIndex = _packageList.Count;
            AddBtnList();
        }

        if (PackageStoreID != 0)
        {
            for (int i = 0; i < _packageList.Count; i++)
            {
                if (_packageList[i].kPackageItem != null)
                {
                    if (_packageList[i].kPackageItem.StoreID == PackageStoreID)
                    {
                        _selBtnIndex = i;
                        break;
                    }
                }
            }
            
        }

        PackageStoreID = (int)eCOUNT.NONE;
        _selBtnData = _packageList[_selBtnIndex];

        if(updateList)
            _btnList.UpdateList();
    }

    public void OnClick_PackbtnList(int index, PackageBtnData packageBtnData)
    {
        if (packageBtnData.kBtnType == ePackageBtnType.CategoryBtn)
        {
            PackageStoreID = (int)eCOUNT.NONE;
            kSelectCategory = packageBtnData.kCategoryType;
            SetPackageBtnList();
        }
        else if (packageBtnData.kBtnType == ePackageBtnType.PackageItem)
        {
            SetPackageBtnList(false);
            _selBtnIndex = index;
            _selBtnData = _packageList[_selBtnIndex];
            //_btnList.RefreshNotMove();
        }

        Renewal(true);
    }

	public void MoveCharPackage( string charName ) {
		if ( string.IsNullOrEmpty( charName ) ) {
			return;
		}

		GameTable.Character.Param characterParam = GameInfo.Instance.GameTable.FindCharacter( x => x.Icon.ToLower().Contains( charName ) );
		if ( characterParam == null ) {
			return;
		}

		int selectCategory = -1;
		for ( int i = 0; i < _packageList.Count; i++ ) {
			if ( _packageList[i].kPackageItem == null ) {
				continue;
			}

			if ( _packageList[i].kPackageItem.Etc.ToLower().Contains( charName ) && _packageList[i].kPackageItem.CharacterID.Equals( characterParam.ID ) ) {
				selectCategory = _packageList[i].kPackageItem.Category;
				break;
			}
		}

		if ( 0 <= selectCategory ) {
			ePackageCategory selectCategoryType = (ePackageCategory)selectCategory;
			if ( kSelectCategory != selectCategoryType ) {
				kSelectCategory = selectCategoryType;
				SetPackageBtnList();
			}
		}
		else {
			if ( kSelectCategory != ePackageCategory.Char_Package ) {
				kSelectCategory = ePackageCategory.Char_Package;
				SetPackageBtnList();
			}
		}

		PickCharacter( charName, characterParam.ID );

		Renewal( true );
	}

	private void PickCharacter( string charName, int characterID ) {
		for ( int i = 0; i < _packageList.Count; i++ ) {
			if ( _packageList[i].kPackageItem == null ) {
				continue;
			}

			if ( _packageList[i].kPackageItem.Etc.ToLower().Contains( charName ) && _packageList[i].kPackageItem.CharacterID.Equals( characterID ) ) {
				SetPackageBtnList( false );
				_selBtnIndex = i;
				_selBtnData = _packageList[i];
				_btnList.SpringSetFocus( i, 0.5f, true );
				break;
			}
		}
	}

	private void SetGachaPriceBtn(int storeID)
    {
        GameTable.Store.Param storeParam = GameInfo.Instance.GameTable.FindStore(x => x.ID == storeID);
        if (storeParam == null)
        {
            Debug.LogError("StoreParam is null");
            return;
        }

        if (IAPManager.Instance.IAPNULL_CHECK())
        {
            kGachaPriceOnObj.SetActive(true);
            kGachaPriceOffObj.SetActive(false);
            kGachaBuyBtn.gameObject.SetActive(true);
            kGachaLimitedObj.SetActive(false);
            kGachaRemainObj.SetActive(false);

            if (!GameSupport.IsStoreSaleApply(storeID))
            {
                kGachaBuyDimdObj.SetActive(true);
                kGachaBuyLabel.textlocalize = FLocalizeString.Instance.GetText(1580);

                kGachaPriceOnObj.SetActive(false);
                kGachaPriceOffObj.SetActive(true);
                kGachaBuyLabel.gameObject.SetActive(false);

                kGachaCountDescLb.SetActive(false);
                kGachaCountLb.SetActive(false);

                var storemydata = GameInfo.Instance.GetStoreData(storeID);
                if (storemydata != null)
                {
                    kGachaRemainObj.SetActive(true);
                    System.DateTime remaintime = storemydata.GetResetTime();
                    string strtime = GameSupport.GetRemainTimeString(GameSupport.GetLocalTimeByServerTime(remaintime), _nowTime);
                    kGachaRemainLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1059), strtime);
                }
            }
            else
            {
                kGachaCountDescLb.SetActive(true);
                kGachaCountLb.SetActive(true);

                //구입 가능
                kGachaLimitedObj.SetActive(true);
                kGachaLimitLb.textlocalize = FLocalizeString.Instance.GetText(1606);

#if UNITY_EDITOR
                kGachaBuyLabel.textlocalize = string.Format("${0:0.##}", (float)storeParam.PurchaseValue * 0.01f);
#else
#if UNITY_ANDROID
                    kGachaBuyLabel.textlocalize = IAPManager.Instance.GetPrice(storeParam.AOS_ID);
#elif UNITY_IOS
                    kGachaBuyLabel.textlocalize = IAPManager.Instance.GetPrice(storeParam.IOS_ID);
#elif !DISABLESTEAMWORKS
                    kGachaBuyLabel.textlocalize = string.Format("${0:0.##}", (float)storeParam.PurchaseValue * 0.01f);
#endif

#endif
            }
        }
        else
        {
            kGachaPriceOnObj.SetActive(false);
            kGachaPriceOffObj.SetActive(true);
            kGachaBuyLabel.gameObject.SetActive(false);
        }
    }

    public void SetSeleteIndex( int index )
    {
        Log.Show(index, Log.ColorType.Blue);
        _seleteindex = index;
        Renewal(true);
    }

    public void OnClick_PercentageBtn()
    {
        if (!_bIsGacha)
            return;
        UIValue.Instance.SetValue(UIValue.EParamType.GachaDetailStoreID, _selBtnData.kPackageItem.StoreID);

        LobbyUIManager.Instance.ShowUI("GachaDetailPopup", true);
    }

    public void OnClick_GachaBtn()
    {
        if (IAPManager.Instance.IsBuying)
            return;

        if (_selBtnData == null)
            return;

        if (!IAPManager.Instance.IAPNULL_CHECK())
        {
            MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3197), null);
            return;
        }

        if (kGachaBuyDimdObj.activeSelf)
            return;

        //MessagePopup.OKCANCEL(eTEXTID.BUY, string.Format(FLocalizeString.Instance.GetText(110), FLocalizeString.Instance.GetText(_selBtnData.kPackageItem.Name)), OnMessageOK, OnMessageCancel);
        MessagePopup.OKCANCEL(eTEXTID.BUY, string.Format(FLocalizeString.Instance.GetText(110), FLocalizeString.Instance.GetText(_selBtnData.kPackageItem.Name)), ()=> { OnMessageOK(_selBtnData.kPackageItem.StoreID); }, OnMessageCancel);
    }

    public void OnClick_BuyBtn()
	{
        if (IAPManager.Instance.IsBuying)
            return;

        if (_selBtnData == null)
            return;

        if(!IAPManager.Instance.IAPNULL_CHECK())
        {
            MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3197), null);
            return;
        }

        if (kGachaBuyDimdObj.activeSelf)
            return;

        BuyPackage(_selBtnData.kPackageItem.StoreID);
    }

    public void OnMessageOK(int storeid)
    {
        WaitPopup.Show();
        IAPManager.Instance.IsBuying = true;

        GameTable.Store.Param storeParam = GameInfo.Instance.GameTable.FindStore(x => x.ID == storeid);

#if UNITY_ANDROID
        if (string.IsNullOrEmpty(storeParam.AOS_ID))
        {
            WaitPopup.Hide();
            IAPManager.Instance.IsBuying = false;
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3028));
        }
        else
            IAPManager.Instance.BuyIAPProduct(storeParam.AOS_ID, ()=> { OnIAPBuySuccess(storeid); }, OnIAPBuyFailed);
#elif UNITY_IOS
        if (string.IsNullOrEmpty(storeParam.IOS_ID))
        {
            WaitPopup.Hide();
            IAPManager.Instance.IsBuying = false;
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3028));
        }
        else
            IAPManager.Instance.BuyIAPProduct(storeParam.IOS_ID, ()=> { OnIAPBuySuccess(storeid); }, OnIAPBuyFailed);
#elif !DISABLESTEAMWORKS
        if (storeParam.PurchaseValue <= 0)
        {
            WaitPopup.Hide();
            IAPManager.Instance.IsBuying = false;
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3028));
        }
        else
            IAPManager.Instance.BuyIAPProduct(storeParam.ID.ToString(), ()=> { OnIAPBuySuccess(storeid); }, OnIAPBuyFailed);
#endif
    }

    public void OnMessageCancel()
    {
        WaitPopup.Hide();
        IAPManager.Instance.IsBuying = false;
    }

    public void OnIAPBuySuccess(int storeid)
    {
        var storetabledata = GameInfo.Instance.GameTable.FindStore(x => x.ID == storeid);
        if (storetabledata == null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3009));
            return;
        }
        //PackageStoreID = storeid;
        _buyStoreID = storeid;
        PlayerPrefs.SetInt("IAPBuyStoreID", storeid);
        PlayerPrefs.Save();

        _precardbooklist.Clear();
        _precardbooklist.AddRange(GameInfo.Instance.CardBookList);

#if !DISABLESTEAMWORKS
#if UNITY_EDITOR
        GameInfo.Instance.Send_ReqStorePurchase(storeid, false, 1, OnNetPurchase);    
#else
        GameInfo.Instance.Send_ReqSteamPurchase(storeid, OnNetPurchase);
#endif
#else

#if UNITY_EDITOR
        GameInfo.Instance.Send_ReqStorePurchase(storeid, false, 1, OnNetPurchase);
#else
        GameInfo.Instance.Send_ReqStorePurchaseInApp(IAPManager.Instance.Receipt, storeid, OnNetPurchase);
#endif


#endif
    }

    public void OnIAPBuyFailed()
    {
        WaitPopup.Hide();
        IAPManager.Instance.IsBuying = false;

        LobbyUIManager.Instance.HideUI("PackageInfoPopup");

        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3212));
    }
	
	public void OnClick_CloseBtn()
	{
        OnClickClose();
	}

    public override void OnClickClose()
    {
        if (IAPManager.Instance.IsBuying)
            return;

        PackageStoreID = (int)eCOUNT.NONE;
        _buyStoreID = (int)eCOUNT.NONE;
        UICharInfoPanel charinfopanel = LobbyUIManager.Instance.GetActiveUI<UICharInfoPanel>("CharInfoPanel");

        if(charinfopanel != null)
        {
            //Debug.LogError(charinfopanel.CharinfoTab);
            //UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, charinfopanel.CharinfoTab);
            LobbyUIManager.Instance.InitComponent("CharInfoPanel");
            LobbyUIManager.Instance.Renewal("CharInfoPanel");
        }

        base.OnClickClose();
    }

    public void OnClick_law1Btn()
	{
        if (IAPManager.Instance.IsBuying)
            return;

        if (string.IsNullOrEmpty(GameInfo.Instance.GameConfig.WebCashpaymentmethod))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3001));
        }
        else
        {
            GameSupport.OpenWebView(FLocalizeString.Instance.GetText(504), GameInfo.Instance.GameConfig.WebCashpaymentmethod);
        }
    }
	
	public void OnClick_law2Btn()
	{
        if (IAPManager.Instance.IsBuying)
            return;

        if (string.IsNullOrEmpty(GameInfo.Instance.GameConfig.WebSpecifiedCommercialTransactionsAct))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3001));
        }
        else
        {
            GameSupport.OpenWebView(FLocalizeString.Instance.GetText(505), GameInfo.Instance.GameConfig.WebSpecifiedCommercialTransactionsAct);
        }
    }

    public void OnNetPurchase(int result, PktMsgType pktmsg)
    {
        if (kPremiumEffectObj.activeSelf)
            kPremiumEffectObj.SetActive(false);

        LobbyUIManager.Instance.HideUI("PackageInfoPopup");

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");

        //추가 스킬 슬롯 오픈 관련
        if(LobbyUIManager.Instance.GetActiveUI("CharInfoPanel") != null)
            LobbyUIManager.Instance.Renewal("CharInfoPanel");

        //구매 결과 응답을 받으면 제거
        PlayerPrefs.DeleteKey("IAPBuyReceipt");
        PlayerPrefs.DeleteKey("IAPBuyStoreID");
        PlayerPrefs.Save();

        IAPManager.Instance.ResetReceipt();

        //200128 구글 이중결제 이슈 관련 수정
        //구매 영수증 로컬데이터 제거 후 버튼 동작 먹히도록 수정
        WaitPopup.Hide();

        if (_bIsGacha)
        {
            if (GameInfo.Instance.netFlag)
            {
                PktInfoStorePurchase pktInfoStorePurchase = pktmsg as PktInfoStorePurchase;
                if (pktInfoStorePurchase == null)
                {
                    Debug.LogError("OnNetPurchase::pktmsg가 PktInfoStorePurchase 타입이 아닙니다.");
                    return;
                }

                GameInfo.Instance.RewardGachaSupporterPoint = 0;
                GameInfo.Instance.RewardGachaDesirePoint = 0;
                GameInfo.Instance.RewardList.Clear();

                for (int i = 0; i < pktInfoStorePurchase.products_.goodsInfos_.Count; i++)
                {
                    if (pktInfoStorePurchase.products_.goodsInfos_[i].value_ <= (int)eCOUNT.NONE)
                        continue;

                    if (pktInfoStorePurchase.products_.goodsInfos_[i].type_ == eGOODSTYPE.SUPPORTERPOINT)
                        GameInfo.Instance.RewardGachaSupporterPoint = (int)pktInfoStorePurchase.products_.goodsInfos_[i].value_;
                    else if (pktInfoStorePurchase.products_.goodsInfos_[i].type_ == eGOODSTYPE.DESIREPOINT)
                        GameInfo.Instance.RewardGachaDesirePoint = (int)pktInfoStorePurchase.products_.goodsInfos_[i].value_;
                }

                for (int i = 0; i < pktInfoStorePurchase.products_.lotteryInfos_.Count; i++)
                {
                    PktInfoProductPack.Lottery lottery = pktInfoStorePurchase.products_.lotteryInfos_[i];
                    bool changeGrade = PktInfoProductPack.Lottery.TYPE.GRADE_UP == (PktInfoProductPack.Lottery.TYPE)lottery.dropTP_;

                    RewardData data = new RewardData((long)lottery.uid_, (int)lottery.rwdTP_, (int)lottery.idx_, lottery.value_, changeGrade);
                    //data.bNew = pktInfoStorePurchase.products_.IsNew(lottery);

                    int rand = UnityEngine.Random.Range(0, i);
                    GameInfo.Instance.RewardList.Insert(rand, data);
                }

                for (int i = 0; i < GameInfo.Instance.RewardList.Count; i++)
                {
                    RewardData reward = GameInfo.Instance.RewardList[i];
                    if (reward.Type == (int)eREWARDTYPE.CARD)
                    {
                        var data = _precardbooklist.Find(x => x.TableID == reward.Index);
                        if (data == null)
                            GameInfo.Instance.RewardList[i].bNew = true;
                    }
                }
            }
            else
            {
                List<RewardData> tempRewardList = new List<RewardData>();
                for (int i = 0; i < GameInfo.Instance.RewardList.Count; i++)
                    tempRewardList.Add(GameInfo.Instance.RewardList[i]);

                GameInfo.Instance.RewardList.Clear();
                for (int i = 0; i < tempRewardList.Count; i++)
                {
                    int rand = UnityEngine.Random.Range(0, i);
                    GameInfo.Instance.RewardList.Insert(rand, tempRewardList[i]);
                }

                for (int i = 0; i < GameInfo.Instance.RewardList.Count; i++)
                {
                    RewardData reward = GameInfo.Instance.RewardList[i];
                    if (reward.Type == (int)eREWARDTYPE.CARD)
                    {
                        var data = _precardbooklist.Find(x => x.TableID == reward.Index);
                        if (data == null)
                            GameInfo.Instance.RewardList[i].bNew = true;
                    }
                }
            }


            if (_selBtnData == null)
                return;

            IAPManager.Instance.IsBuying = false;
            DirectorUIManager.Instance.PlayGacha(3);

            //InitComponent();
            Renewal(true);
        }
        else
        {
            

            PktInfoStorePurchase _pktInfo = pktmsg as PktInfoStorePurchase;
            if (_pktInfo.products_.charInfos_.Count != 0)
            {
                var data = _pktInfo.products_.charInfos_[0];
                DirectorUIManager.Instance.PlayCharBuy((int)data.char_.tableID_, OnDirectorUIPlayCharBuy);
            }
            else
            {
                OnDirectorUIPlayCharBuy();
            }
        }

        
    }

    public void OnDirectorUIPlayCharBuy()
    {
        IAPManager.Instance.IsBuying = false;

        var storetabledata = GameInfo.Instance.GameTable.FindStore(x => x.ID == _buyStoreID);
        if (storetabledata == null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3009));
            return;
        }

        string title = FLocalizeString.Instance.GetText(1322);
        string desc = FLocalizeString.Instance.GetText(1323);

        MessageRewardListPopup.RewardListMessage(title, desc, GameInfo.Instance.RewardList, OnMessageRewardCallBack);
    }

    public void OnMessageRewardCallBack()
    {
        DirectorUIManager.Instance.PlayNewCardGreeings(_precardbooklist);
        List<GameClientTable.StoreDisplayGoods.Param> list = new List<GameClientTable.StoreDisplayGoods.Param>();
        list = GameInfo.Instance.GameClientTable.FindAllStoreDisplayGoods(x => x.PanelType == (int)eSD_PanelType.PACKAGE);

        bool packageflag = false;
        for (int i = 0; i < list.Count; i++)
        {
            if (!GameSupport.IsShowStoreDisplay(list[i]))
                continue;

            packageflag = true;
            break;
        }

        if (!packageflag)
        {
            OnClickClose();
            return;
        }
        else
        {
            //InitComponent();
            Renewal(true);
        }
    }

    public int CompareFuncStoreDisplayGoodsStoreID(GameClientTable.StoreDisplayGoods.Param a, GameClientTable.StoreDisplayGoods.Param b)
    {

        if (a.StoreID < b.StoreID) return -1;
        if (a.StoreID > b.StoreID) return 1;
        return 0;
    }

    public int CompareFuncStoreDisplayGoods(GameClientTable.StoreDisplayGoods.Param a, GameClientTable.StoreDisplayGoods.Param b)
    {
        
        if (a.SubCategory < b.SubCategory) return 1; 
        if (a.SubCategory > b.SubCategory) return -1;
        return 0;
    }

    public int CompareFuncStoreData(GameClientTable.StoreDisplayGoods.Param a, GameClientTable.StoreDisplayGoods.Param b)
    {
        StoreData aData = GameInfo.Instance.GetStoreData(a.StoreID);
        StoreData bData = GameInfo.Instance.GetStoreData(b.StoreID);

        if (aData != null && GameSupport.GetLimitedCnt(a.StoreID).Equals(0)) return 1;
        if (bData != null && GameSupport.GetLimitedCnt(b.StoreID).Equals(0)) return -1;

        return 0;
    }

    public override bool IsBackButton()
    {
        if (IAPManager.Instance.IsBuying)
            return false;

        return base.IsBackButton();
    }

    private void GetBGTexture(UITexture target, string url, bool platform = true, bool localize = true)
    {
        if(_bgLoadCoroutine != null)
        {
            StopCoroutine(_bgLoadCoroutine);
            _bgLoadCoroutine = null;
        }

        if(GameInfo.Instance.netFlag)
        {
			if (target.mainTexture != null)
			{
				DestroyImmediate(target.mainTexture, false);
				target.mainTexture = null;
			}

            target.mainTexture = BannerManager.Instance.LoadTextureWithFileURL(url, platform, localize);
            if( target.mainTexture == null)
            {
                _bgLoadCoroutine = StartCoroutine(GetBGTexureAsync(target, url));
            }
        }
        else
        {
            target.mainTexture = (Texture2D)ResourceMgr.Instance.LoadFromAssetBundle("temp", "Temp/" + url);
        }
    }

    IEnumerator GetBGTexureAsync(UITexture target, string url)
    {
        while(this.gameObject.activeSelf)
        {
			if (target.mainTexture != null)
			{
				DestroyImmediate(target.mainTexture, false);
				target.mainTexture = null;
			}

            target.mainTexture = BannerManager.Instance.LoadTextureWithFileURL(url, true);
            if(target.mainTexture != null)
                break;
            yield return null;
        }

        if(_bgLoadCoroutine != null)
            _bgLoadCoroutine = null;
    }

    public void OnClick_RankPackageInfo(int storeID)
    {
        if (GameSupport.IsHaveStoreData(storeID))
            return;

        GameTable.Store.Param storedata = GameInfo.Instance.GameTable.FindStore(x => x.ID == storeID);
        if (storedata == null)
            return;

        GameClientTable.StoreDisplayGoods.Param disstoredata = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == storeID && x.PackageUIType == (int)eCOUNT.NONE);
        if (disstoredata == null)
            return;

        if (storedata.NeedBuyStoreID > (int)eCOUNT.NONE)
        {
            if (GameSupport.IsHaveStoreData(storedata.NeedBuyStoreID))
            {
                //구매가능
                Log.Show(storeID + " 구매가능");
                PackageInfoPopup.ShowPackageInfoPopup(storedata.ID, ePackageUIType.Rank, disstoredata, () =>
                {
                    BuyPackage(storedata.ID);
                });
            }
            else
            {
                //구매불가
                Log.Show(storeID + " 구매불가");
                PackageInfoPopup.ShowNotBuyPackageInfoPopup(storedata.ID, ePackageUIType.Rank, disstoredata, null);
            }
        }
        else
        {
            //구매가능
            Log.Show(storeID + " 구매가능");
            PackageInfoPopup.ShowPackageInfoPopup(storedata.ID, ePackageUIType.Rank, disstoredata, () =>
            {
                BuyPackage(storedata.ID);
            });
        }
    }

    public void BuyPackage(int storeid)
    {
        if (IAPManager.Instance.IsBuying)
            return;

        GameTable.Store.Param storedata = GameInfo.Instance.GameTable.FindStore(x => x.ID == storeid);

        if (storedata == null)
            return;

        if (!IAPManager.Instance.IAPNULL_CHECK())
        {
            MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3197), null);
            return;
        }

        List<GameTable.Random.Param> overlabList = new List<GameTable.Random.Param>();
        List<GameTable.Random.Param> rewardList = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == storedata.ProductIndex);
        for (int i = 0; i < rewardList.Count; i++)
        {
            if (rewardList[i].ProductType == (int)eREWARDTYPE.CHAR)
            {
                CharData chardata = GameInfo.Instance.GetCharDataByTableID(rewardList[i].ProductIndex);
                if (chardata != null)
                {
                    overlabList.Add(rewardList[i]);
                }
            }
            else if (rewardList[i].ProductType == (int)eREWARDTYPE.COSTUME)
            {
                for (int j = 0; j < GameInfo.Instance.CostumeList.Count; j++)
                {
                    if (GameInfo.Instance.CostumeList[j] == rewardList[i].ProductIndex)
                    {
                        overlabList.Add(rewardList[i]);
                    }
                }
            }
        }

        //7766 인앱 연결
        //3113	{0} 상품을 구매 하시겠습니까?
        if (overlabList.Count > 0)
        {
            PackageBuyPopup.ShowPackageBuyPopup(storedata.ID, overlabList, FLocalizeString.Instance.GetText((int)eTEXTID.TITLE_BUY), FLocalizeString.Instance.GetText((int)eTEXTID.OK), FLocalizeString.Instance.GetText((int)eTEXTID.CANCEL), () => { OnMessageOK(storeid); }, OnMessageCancel);
        }
        else
        {
            GameClientTable.StoreDisplayGoods.Param disclientdata = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == storedata.ID && x.PackageUIType == (int)eCOUNT.NONE);
            if (disclientdata != null)
            {
                //MessagePopup.OKCANCEL(eTEXTID.BUY, string.Format(FLocalizeString.Instance.GetText(110), FLocalizeString.Instance.GetText(disclientdata.Name)), OnMessageOK, OnMessageCancel);
                MessagePopup.OKCANCEL(eTEXTID.BUY, string.Format(FLocalizeString.Instance.GetText(110), FLocalizeString.Instance.GetText(disclientdata.Name)), ()=> { OnMessageOK(storeid); }, OnMessageCancel);
            }
            
        }
    }
}
