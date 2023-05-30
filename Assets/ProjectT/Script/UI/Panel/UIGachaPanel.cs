using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class UIGachaPanel : FComponent
{
    public class GachaTabParam
    {
        public System.DateTime StartDate;  //시작일시
        public System.DateTime EndDate;    //종료일시
        public int ID;
        public string Type;
        public int Category;
        public string TabIcon;
        public string BGTaxture;
        public string AddTexture;
        public int StoreID1;
        public int StoreID2;
        public int StoreID3;
        public int StoreID4;
        public int Name;
        public int Desc;
        public bool[] Localize;
        public string Value1;
        public int SubCategory;
    }

    [Serializable]
    public class GachaGuerrillaReward {
        public GameObject RewardObj;
        public GameObject FrameType1Obj;
        public GameObject FrameType2Obj;
        public GameObject Notice;
        public UILabel GachaRewardText;
        public UILabel GachaRewardText_Shadow;
        public GameObject LeftArrow;
        public GameObject RightArrow;
        public UIGachaGuerrilaRewardSlot Slot;
        public UIGaugeUnit GachaGauge;

        public int selectIndex = 0;
    }

    //prf_fx_ui_item_level_up
    public GameObject kBasciGacha;
    public GameObject kTicket;
    public UITexture kBasicTex;
    public UISprite kBasicSpr;
    public GameObject kDate;
    public UILabel kDateLabel;
    public UISprite kDateSpr;

    public List<UIGachaBtnUnit> kGachaBtnUnitList = new List<UIGachaBtnUnit>();
    [SerializeField] private FList _BannerListInstance;
    [SerializeField] private FList _TicketGachaListInstance;

	public GameObject TexLoadingObj;

    public List<GameObject> kTicketTypeOffObjs;

    [Header("Gacha Guerrilla Reward")]
    [SerializeField] private GachaGuerrillaReward _GachaGuerrillaReward;

    [Header("Desire Objs")]
    public GameObject kDesireShopObj;
    public UISprite kDesireBGSpr;
    public UIGaugeUnit kDesireGaugeUnit;
    [SerializeField] private FList _DesireShopListInstance;
    public GameObject kDesireGaugeMaxEff;

    [Header("Basic Gacha")]
    [SerializeField] private UITexture kGachaTitleTex;
    [SerializeField] private UILabel kGachaTitleLabel;
    [SerializeField] private GameObject kGachaDescriptionObj;
    [SerializeField] private UILabel kGachaDescriptionLabel;
    [SerializeField] private GameObject kGachaFrameType1Spr;
    [SerializeField] private GameObject kGachaFrameType2Spr;
    [SerializeField] private UISprite kLimitedSpr;
    [SerializeField] private UISprite kRepeatSpr;
	[SerializeField] private GameObject BtnPercentage;

	[Header("Rotation Gacha")]
    [SerializeField] private GameObject kRotationGachaObj;
    [SerializeField] private FList _RotationGachaList;
    [SerializeField] private GameObject kRotationGachaSlotCircle;    
    [SerializeField] private GameObject kRotationGachaBottomObj;
    [SerializeField] private GameObject kRotationGachaResetTimeObj;
    [SerializeField] private UILabel kRotationRemainTime;
    [SerializeField] private GameObject kRotationBuyGachaResetTimeObj;
    [SerializeField] private UILabel kRotationBuyRemainTime;
    [SerializeField] private List<UIGachaBtnUnit> kRotationGachaBtnUnitList = new List<UIGachaBtnUnit>();
	[SerializeField] private GameObject kRotationGachaPercentObj;


	public UIGachaBtnUnit CurClickedGachaBtn { get; private set; } = null;

	private List<GameObject> RotationGachaCircleList = new List<GameObject>();

    private bool _bsendfree = false;
    private int _sendstoreid = -1;

    private int _selectTab = 0;
    
    private List<CardBookData> _cardbooklist = new List<CardBookData>();
    private List<GachaTabParam> _gachatablist = new List<GachaTabParam>();

    private List<GameClientTable.StoreDisplayGoods.Param> _oriticketgachalist = new List<GameClientTable.StoreDisplayGoods.Param>();
    private List<GameClientTable.StoreDisplayGoods.Param> _ticketgachalist = new List<GameClientTable.StoreDisplayGoods.Param>();
    private List<GameTable.Store.Param> _desireshoplist = new List<GameTable.Store.Param>();

    public int SeleteTab { get { return _selectTab; } }
    public List<CardBookData> CardBookList { get { return _cardbooklist; } }

    private bool m_isBackButton = true;
    private Coroutine _bgLoadCoroutine = null;

    private int CurRotaionGachaID = 1;
    private bool IsRotationGachaTimeUpdate = false;
    private bool IsBuyRotationGachaTimeUpdate = false;
    private System.DateTime CurRotationDateTime = System.DateTime.MinValue;

	private bool mIsSendGachaPacket = false;

    //자동 가챠 관련
    public bool mIsAutoGachaDiretor { get; private set; }
    private bool mIsAutoGacha = false;
    public void SetAutoGacha(bool _isOn) {
        mIsAutoGacha = _isOn;
    }

    public override void Awake()
	{
		base.Awake();

        if (this._BannerListInstance == null) return;

        this._BannerListInstance.EventUpdate = this._UpdateBannerListSlot;
        this._BannerListInstance.EventGetItemCount = this._GetBannerElementCount;
        this._BannerListInstance.InitBottomFixing();

        if (this._TicketGachaListInstance == null) return;

        this._TicketGachaListInstance.EventUpdate = this._UpdateTicketGachaListSlot;
        this._TicketGachaListInstance.EventGetItemCount = this._GetTicketGachaElementCount;
        this._TicketGachaListInstance.InitBottomFixing();

        if (this._DesireShopListInstance == null) return;

        this._DesireShopListInstance.EventUpdate = this._UpdateDesireShopListSlot;
        this._DesireShopListInstance.EventGetItemCount = this._GetDesireShopElementCount;
        this._DesireShopListInstance.InitBottomFixing();


        if (_RotationGachaList == null) return;
        _RotationGachaList.EventUpdate = _UpdateRotationGachaListSlot;
        _RotationGachaList.EventGetItemCount = () => { return GameInfo.Instance.GameTable.RotationGachas.Count; };
        _RotationGachaList.ScrollView.onDragFinished = OnDragFinished_RotationGacha;
        _RotationGachaList.ScrollView.onPressMoving = OnPressMoving_RotationGacha;
        _RotationGachaList.ScrollView.onStoppedMoving = OnStoppedMoving_RotationGacha;
        _RotationGachaList.ScrollView.scrollWheelFactor = 0f;
		_RotationGachaList.UpdateList();
	}
 
	public override void OnEnable()
	{
        InitComponent();

        _sendstoreid = -1;

        base.OnEnable();
    }

	public override void OnDisable() {
		base.OnDisable();

		mIsSendGachaPacket = false;
	}

    private void FixedUpdate()
    {
        if (IsRotationGachaTimeUpdate)
        {
            FixedUpate_RotationGachaTime();
        }

        if (IsBuyRotationGachaTimeUpdate)
        {
            FixedUpate_BuyRotationGachaTime();
        }
    }

    void FixedUpate_BuyRotationGachaTime()
    {
        System.TimeSpan t = GameSupport.GetRemainTime(CurRotationDateTime, GameInfo.Instance.GetNetworkTime());
        if (t.TotalSeconds <= 0)
        {
            IsBuyRotationGachaTimeUpdate = false;
            Renewal(false);
            return;
        }

        kRotationBuyRemainTime.textlocalize = FLocalizeString.Instance.GetText(1710, GameSupport.GetTimeString(t));
    }

    void FixedUpate_RotationGachaTime()
    {
        System.TimeSpan t = GameSupport.GetRemainTime(CurRotationDateTime, GameInfo.Instance.GetNetworkTime());
        if (t.TotalSeconds <= 0)
        {
            IsRotationGachaTimeUpdate = false;
            Renewal(false);
            return;
        }

        kRotationRemainTime.textlocalize = FLocalizeString.Instance.GetText(1059, GameSupport.GetTimeString(t));
    }

    public override void InitComponent()
    {
        ServerData serverdata = GameInfo.Instance.ServerData;
        //if (serverdata.GachaCategoryList.Count <= 0)
        //    return;

        _gachatablist.Clear();
        
        foreach (GameClientTable.GachaTab.Param param in GameInfo.Instance.GameClientTable.GachaTabs)
        {
            GachaTabParam data = new GachaTabParam();
            data.Type = param.Type;
            data.Category = param.Category;
            data.TabIcon = param.TabIcon;
            data.BGTaxture = param.BGTaxture;
            data.StoreID1 = param.StoreID1;
            data.StoreID2 = param.StoreID2;
            data.StoreID3 = (int)eCOUNT.NONE;
            data.StoreID4 = (int)eCOUNT.NONE;
            data.Name = param.Name;
            
            _gachatablist.Add(data);
        }
        
        System.DateTime nowTime = GameInfo.Instance.GetNetworkTime();
        
        foreach (GachaCategoryData gachaCategoryData in GameInfo.Instance.ServerData.GachaCategoryList)
        {
            if(string.IsNullOrEmpty(gachaCategoryData.Type))
            {
                Debug.LogWarning($"{gachaCategoryData.StoreID1} Gacha Type : {gachaCategoryData.Type}");
                continue;
            }

            System.DateTime starttime = gachaCategoryData.StartDate;
            System.DateTime endtime = gachaCategoryData.EndDate;

            if (starttime.Ticks <= nowTime.Ticks && endtime.Ticks >= nowTime.Ticks)
            {
                GachaTabParam data = new GachaTabParam();
                data.ID = -1;
                data.StartDate = gachaCategoryData.StartDate;
                data.EndDate = gachaCategoryData.EndDate;
                data.Type = gachaCategoryData.Type;
                data.TabIcon = gachaCategoryData.UrlBtnImage;
                data.BGTaxture = gachaCategoryData.UrlBGImage;
                data.AddTexture = gachaCategoryData.UrlAddImage;
                data.StoreID1 = -1;
                data.StoreID2 = -1;
                data.StoreID3 = -1;
                data.StoreID4 = -1;
                data.Name = gachaCategoryData.Name;
                data.Desc = gachaCategoryData.Desc;
                data.Localize = gachaCategoryData.Localize;
                data.Value1 = gachaCategoryData.Value1;
                data.SubCategory = -1;
                
                var s1data = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == gachaCategoryData.StoreID1);
                var s2data = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == gachaCategoryData.StoreID2);
                if (s1data != null && s2data != null)
                {
                    data.StoreID1 = s1data.ID;
                    data.StoreID2 = s2data.ID;
                    data.SubCategory = s1data.SubCategory;
                }
                
                var s3data = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == gachaCategoryData.StoreID3);
                var s4data = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == gachaCategoryData.StoreID4);
                if (s3data != null && s4data != null)
                {
                    data.StoreID3 = s3data.ID;
                    data.StoreID4 = s4data.ID;
                    data.SubCategory = s3data.SubCategory;
                }
                
                if (_gachatablist.Count > 0)
                {
                    _gachatablist.Insert(0, data);
                }
                else
                {
                    _gachatablist.Add(data);
                }
            }
        }

        _oriticketgachalist.Clear();
        _ticketgachalist.Clear();
        var list = GameInfo.Instance.GameClientTable.FindAllStoreDisplayGoods(x => x.PanelType == (int)eSD_PanelType.GACHA && x.Type == "TICKET");
        for (int i = 0; i < list.Count; i++)
        {
            _oriticketgachalist.Add(list[i]);
            _ticketgachalist.Add(list[i]);
        }

        _desireshoplist.Clear();
        var desirelist = GameInfo.Instance.GameClientTable.FindAllStoreDisplayGoods(x => x.Type == "DESIRE" && x.Category == 13 );
        for (int i = 0; i < desirelist.Count; i++)
        {
            GameTable.Store.Param desireStore = GameInfo.Instance.GameTable.FindStore(x => x.ID == desirelist[i].StoreID);
            if (desireStore == null)
                continue;

            if (desireStore.ConnectStoreID == (int)eCOUNT.NONE)
            {
                if (desireStore.SaleType == (int)eStoreSaleKind.LimitCnt)		//구매횟수 제한이지만, 염원의상점에서만 기간제로 기능하기
                {
                    GachaCategoryData dataflag = GameInfo.Instance.ServerData.GachaCategoryList.Find(x => x.StoreID1 == desireStore.ID || x.StoreID2 == desireStore.ID);
                    if (dataflag == null)
                        continue;

                    System.DateTime starttime = dataflag.StartDate;
                    System.DateTime endtime = dataflag.EndDate;

                    if (starttime.Ticks <= nowTime.Ticks && endtime.Ticks >= nowTime.Ticks)
                    {
                        _desireshoplist.Add(desireStore);
                    }
                }
                else
                {
                    if (GameSupport.IsShowStoreDisplay(desirelist[i]))
                    {
                        _desireshoplist.Add(desireStore);
                    }
                }
            }
            else
            {
                if (desireStore.SaleType == (int)eStoreSaleKind.LimitDate)
                {
                    GachaCategoryData dataflag = GameInfo.Instance.ServerData.GachaCategoryList.Find(x => x.StoreID1 == desireStore.ConnectStoreID || x.StoreID2 == desireStore.ConnectStoreID);
                    if (dataflag == null)
                        continue;

                    System.DateTime starttime = dataflag.StartDate;
                    System.DateTime endtime = dataflag.EndDate;

                    if (starttime.Ticks <= nowTime.Ticks && endtime.Ticks >= nowTime.Ticks)
                    {
                        if (GameSupport.IsShowStoreDisplay(desirelist[i]))
                        {
                            _desireshoplist.Add(desireStore);
                        }
                    }
                }
                else
                {
                    if (GameSupport.IsShowStoreDisplay(desirelist[i]))
                    {
                        _desireshoplist.Add(desireStore);
                    }
                }
            }
        }

        this._BannerListInstance.UpdateList();
        this._TicketGachaListInstance.UpdateList();
        this._DesireShopListInstance.UpdateList();
	}

	public override void TabRenewal()
    {
        base.TabRenewal();
        
        _selectTab = 0;
        
        string typeStr = (string)UIValue.Instance.TryGetValue(UIValue.EParamType.GachaTab, "", true);
        string subCategoryStr = (string)UIValue.Instance.TryGetValue(UIValue.EParamType.GachaTabValue03, "", true);
        int.TryParse(subCategoryStr, out int nSubCategory);
        for (int i = 0; i < _gachatablist.Count; i++)
        {
            if (_gachatablist[i].Type == typeStr && _gachatablist[i].SubCategory == nSubCategory)
            {
                _selectTab = i;
                break;
            }
        }

        _BannerListInstance.SpringSetFocus(_selectTab, isImmediate: true);
    }
    
    public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

		TexLoadingObj.SetActive(true);

		BtnPercentage.SetActive(true);
		BtnPercentage.transform.localPosition = new Vector3(-480.0f,
															BtnPercentage.transform.localPosition.y,
															BtnPercentage.transform.localPosition.z);

		ServerData serverdata = GameInfo.Instance.ServerData;

        int insertNum = 0;
        _ticketgachalist.Clear();
        for (int i = 0; i < _oriticketgachalist.Count; i += 2)
        {
            var storetable = GameInfo.Instance.GameTable.FindStore(_oriticketgachalist[i].StoreID);
            if (storetable != null)
            {
                int count = GameInfo.Instance.GetItemIDCount(storetable.PurchaseIndex);
                if (count > 0)
                {
                    _ticketgachalist.Insert(insertNum, _oriticketgachalist[i]);
                    _ticketgachalist.Insert(insertNum + 1, _oriticketgachalist[i + 1]);
                    insertNum += 2;
                }
                else
                {
                    _ticketgachalist.Add(_oriticketgachalist[i]);
                    _ticketgachalist.Add(_oriticketgachalist[i + 1]);
                }
            }
        }

        this._BannerListInstance.RefreshNotMove();
        this._TicketGachaListInstance.RefreshNotMove();
        this._DesireShopListInstance.RefreshNotMove();

        GachaTabParam gachatabdata = null;
        if (0 <= _selectTab && _gachatablist.Count > _selectTab)
            gachatabdata = _gachatablist[_selectTab];

        Renewal_SetState(false);

        if (_gachatablist[_selectTab].Type == "TICKET")
        {
            TexLoadingObj.SetActive(false);
            kTicket.SetActive(true);
        }
        else if (_gachatablist[_selectTab].Type == "DESIRE")
        {
			TexLoadingObj.SetActive(false);
			kDesireShopObj.SetActive(true);
            
            kDesireBGSpr.spriteName = _gachatablist[_selectTab].BGTaxture;

            kDesireGaugeUnit.InitGaugeUnit((float)GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.DESIREPOINT] / (float)GameInfo.Instance.GameConfig.LimitMaxDP);
            kDesireGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText(218), GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.DESIREPOINT], GameInfo.Instance.GameConfig.LimitMaxDP));

            if (GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.DESIREPOINT] >= GameInfo.Instance.GameConfig.LimitMaxDP)
                kDesireGaugeMaxEff.SetActive(true);
        }
        else if (_gachatablist[_selectTab].Type == "ROTATION")
        {
            kRotationGachaObj.SetActive(true);
            kRotationGachaBottomObj.SetActive(true);
            kRotationGachaPercentObj.SetActive(true);

            _RotationGachaList.SpringSetFocus(CurRotaionGachaID - 1, isImmediate: bChildren);

            var serverGachaInfo =  GameInfo.Instance.ServerRotationGachaData.Infos.Find(x => x.RemainCount == CurRotaionGachaID);
            if (serverGachaInfo != null)
            {
                // 오늘 로테이션 가챠, 구매 가능
                Renewal_RotationGacha_Enable(serverGachaInfo);
            }
            else
            {
                var userGachaInfo = GameInfo.Instance.UserRotationGachaData.Infos.Find(x => x.RemainCount == CurRotaionGachaID);
                if (userGachaInfo == null)
                {
                    // 로테이션 가챠 구매 불가능
                    // 남은 시간 표시
                    Renewal_RotationGacha_Disable(userGachaInfo);
                }
                else
                {
                    System.TimeSpan t = GameSupport.GetRemainTime(userGachaInfo.Time, GameInfo.Instance.GetNetworkTime());
                    if (t.TotalSeconds <= (int)eCOUNT.NONE)
                    {
                        // 로테이션 가챠 구매 불가능
                        // 남은 시간 표시
                        Renewal_RotationGacha_Disable(userGachaInfo);
                    }
                    else
                    {
                        // 로테이션 가챠 구매 가능
                        Renewal_RotationGacha_Enable(userGachaInfo);
                    }
                    
                }
            }

            Renewal_RotationGacha_Circle();
        }
        else
        {
            kBasciGacha.SetActive(true);

            
            eGachaPickupType pickUpType = eGachaPickupType.None;
            if (gachatabdata?.Value1 != null)
            {
                string[] splits = gachatabdata.Value1.Split(',');
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

            kLimitedSpr.SetActive((pickUpType & eGachaPickupType.Limit) == eGachaPickupType.Limit);

            bool bShowRepeat = true;
            if (gachatabdata != null && (gachatabdata.Type.Equals("PICKUP3") || gachatabdata.Type.Equals("PICKUP4")))
            {
                bShowRepeat = false;
            }
            kRepeatSpr.SetActive((pickUpType & eGachaPickupType.Retro) == eGachaPickupType.Retro && bShowRepeat);
            
            kGachaFrameType1Spr.SetActive(!kLimitedSpr.gameObject.activeSelf);
            kGachaFrameType2Spr.SetActive(kLimitedSpr.gameObject.activeSelf);

            string hexColor = kLimitedSpr.gameObject.activeSelf ? "#FFC50BFF" : "#A80000FF";
            ColorUtility.TryParseHtmlString(hexColor, out Color dateSprColor);
            kDateSpr.color = dateSprColor;

            kGachaTitleLabel.textlocalize = FLocalizeString.Instance.GetText(gachatabdata?.Name ?? 0);
            bool addUrlImageActive = (gachatabdata?.Type.Contains("PICKUP") ?? false) && string.IsNullOrEmpty(kGachaTitleLabel.textlocalize);
            kGachaTitleTex.SetActive(addUrlImageActive);
            if (kGachaTitleTex.gameObject.activeSelf)
            {
                kGachaFrameType1Spr.SetActive(false);
                kGachaFrameType2Spr.SetActive(false);

                GetBGTexture(kGachaTitleTex, gachatabdata.AddTexture, false, gachatabdata.Localize[(int)eGachaLocalizeType.AddImage]);
            }

            int descId = gachatabdata?.Desc ?? 0;
            kGachaDescriptionObj.SetActive(descId != 0);
            if (kGachaDescriptionObj.activeSelf)
            {
                kGachaDescriptionLabel.textlocalize = FLocalizeString.Instance.GetText(descId);
            }
            
            for (int i = 0; i < kTicketTypeOffObjs.Count; i++)
                kTicketTypeOffObjs[i].SetActive(true);

            if (gachatabdata.StoreID1 != -1 && gachatabdata.StoreID2 != -1)            
            {
                GetBGTexture(kBasicTex, gachatabdata.BGTaxture, true, gachatabdata.Localize[(int)eGachaLocalizeType.Background]);

                kBasicTex.gameObject.SetActive(true);
                kDate.gameObject.SetActive(true);
                kBasicSpr.gameObject.SetActive(false);
                kDateLabel.textlocalize = GameSupport.GetEndTime(GameSupport.GetMinusOneMinuteEndTime(gachatabdata.EndDate)); //GameSupport.GetPeriodTime(gachatabdata.StartDate, gachatabdata.EndDate);
            }            
            else
            {
                kBasicSpr.spriteName = gachatabdata.BGTaxture;
                kBasicSpr.gameObject.SetActive(true);
                kBasicTex.gameObject.SetActive(false);
                kDate.gameObject.SetActive(false);
            }
            
            // if(bChildren)
            //     PlayAnimtion(2);

            if (_gachatablist[_selectTab].StoreID1 == -1 && _gachatablist[_selectTab].StoreID2 == -1)           
            {
                var list = GameInfo.Instance.GameClientTable.FindAllStoreDisplayGoods(x => x.PanelType == (int)eSD_PanelType.GACHA && x.Type == _gachatablist[_selectTab].Type && x.Category == gachatabdata.Category);
                if (list.Count < 2)
                    return;

                if (list.Count > 2)
                {
                    List<GameClientTable.StoreDisplayGoods.Param> gachaStoreList = new List<GameClientTable.StoreDisplayGoods.Param>();
                    List<GameClientTable.StoreDisplayGoods.Param> gachaStoreItemList = new List<GameClientTable.StoreDisplayGoods.Param>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        GameTable.Store.Param gachaStoreData = GameInfo.Instance.GameTable.FindStore(x => x.ID == list[i].StoreID);
                        if (eGOODSTYPE.CASH == (eGOODSTYPE)gachaStoreData.PurchaseIndex)
                        {
                            gachaStoreList.Add(list[i]);
                        }
                        else
                        {
                            gachaStoreItemList.Add(list[i]);
                        }
                    }

                    for (int i = 0; i < gachaStoreList.Count; i++)
                    {
                        kGachaBtnUnitList[i].gameObject.SetActive(true);
                        kGachaBtnUnitList[i].InitGachaBtnUnit(gachaStoreList[i].ID, gachaStoreItemList[i].ID);
                    }
                }
                else
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        kGachaBtnUnitList[i].gameObject.SetActive(true);
                        kGachaBtnUnitList[i].InitGachaBtnUnit(list[i].ID);
                    }
                }
            }            
            else
            {
				// StoreID1
				GameClientTable.StoreDisplayGoods.Param find1 = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.ID == _gachatablist[_selectTab].StoreID1);
				kGachaBtnUnitList[0].gameObject.SetActive(find1 != null && find1.ShowButton == 1);
				kGachaBtnUnitList[0].InitGachaBtnUnit(_gachatablist[_selectTab].StoreID1, _gachatablist[_selectTab].StoreID3);

				if(!kGachaBtnUnitList[0].gameObject.activeSelf)
				{
					BtnPercentage.transform.localPosition = new Vector3(-270.0f,
																		BtnPercentage.transform.localPosition.y,
																		BtnPercentage.transform.localPosition.z);
				}

				// StoreID2
				GameClientTable.StoreDisplayGoods.Param find2 = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.ID == _gachatablist[_selectTab].StoreID2);
				kGachaBtnUnitList[1].gameObject.SetActive(find2 != null && find2.ShowButton == 1);
				kGachaBtnUnitList[1].InitGachaBtnUnit(_gachatablist[_selectTab].StoreID2, _gachatablist[_selectTab].StoreID4);

                SetGachaGuerrillaMission(true);

				if(kGachaBtnUnitList[1].DisablePurchase && kGachaBtnUnitList[1].DisablePurchase.activeSelf)
				{
					BtnPercentage.transform.localPosition = new Vector3(-342.0f,
																		BtnPercentage.transform.localPosition.y,
																		BtnPercentage.transform.localPosition.z);
				}
            }
        }
    }

    private void Renewal_SetState(bool state)
    {
        Log.Show("Renewal_SetState : " + state);

        for (int i = 0; i < kGachaBtnUnitList.Count; i++)
            kGachaBtnUnitList[i].gameObject.SetActive(state);

        _GachaGuerrillaReward.RewardObj.SetActive(state);

        kBasciGacha.SetActive(state);
        kDesireShopObj.SetActive(state);
        kDesireGaugeMaxEff.SetActive(state);
        kTicket.SetActive(state);

        kRotationGachaObj.SetActive(state);
        kRotationGachaBottomObj.SetActive(state);
        kRotationGachaResetTimeObj.SetActive(state);
        kRotationBuyGachaResetTimeObj.SetActive(state);
        for (int i = 0; i < kTicketTypeOffObjs.Count; i++)
            kTicketTypeOffObjs[i].SetActive(state);

        for (int i = 0; i < kRotationGachaBtnUnitList.Count; i++)
            kRotationGachaBtnUnitList[i].SetActive(state);

        kRotationGachaPercentObj.SetActive(state);

        IsRotationGachaTimeUpdate = state;
    }

    private void Renewal_RotationGacha_Enable(RotationGachaData.Piece piece)
    {
        kRotationBuyGachaResetTimeObj.SetActive(true);

        List<GameClientTable.StoreDisplayGoods.Param> gachaStoreList = new List<GameClientTable.StoreDisplayGoods.Param>();
        List<GameClientTable.StoreDisplayGoods.Param> gachaStoreItemList = new List<GameClientTable.StoreDisplayGoods.Param>();

        System.Action<int> ActionMakeList = (storeid) =>
        {
            if (storeid <= 0) return;

            var data = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x =>
                x.PanelType == (int)eSD_PanelType.GACHA &&
                x.Type == _gachatablist[_selectTab].Type &&
                x.Category == 15 &&
                x.StoreID == storeid);

            if (data == null) return;

            GameTable.Store.Param gachaStoreData = GameInfo.Instance.GameTable.FindStore(x => x.ID == data.StoreID);
            if (eGOODSTYPE.CASH == (eGOODSTYPE)gachaStoreData.PurchaseIndex)
            {
                gachaStoreList.Add(data);
            }
            else
            {
                gachaStoreItemList.Add(data);
            }
        };

        int tableid = piece.RemainCount;
        var rotGacha = GameInfo.Instance.GameTable.RotationGachas.Find(x => x.ID == tableid);
        if (rotGacha == null) return;

        ActionMakeList(rotGacha.StoreID1);
        ActionMakeList(rotGacha.StoreID2);
        ActionMakeList(rotGacha.StoreID3);
        ActionMakeList(rotGacha.StoreID4);

        for (int i = 0; i < gachaStoreList.Count; i++)
        {
            kRotationGachaBtnUnitList[i].SetActive(true);
            kRotationGachaBtnUnitList[i].InitGachaBtnUnit(gachaStoreList[i].ID, gachaStoreItemList[i].ID);
        }
        IsRotationGachaTimeUpdate = false;
        IsBuyRotationGachaTimeUpdate = true;

        CurRotationDateTime = piece.Time;
    }
    
    private void Renewal_RotationGacha_Disable(RotationGachaData.Piece piece)
    {
        kRotationGachaResetTimeObj.SetActive(true);

        int afterDay = 0;
        var gachas = GameInfo.Instance.GameTable.RotationGachas;
        int startIndex = GameInfo.Instance.ServerRotationGachaData.Infos[0].RemainCount;

        Debug.LogWarningFormat("startIndex : {0}, CurRotaionGachaID : {1}", startIndex, CurRotaionGachaID);
        for (int i = 0; i < gachas.Count; i++)
        {
            if (startIndex >= gachas.Count)
                startIndex = 0;

            if (gachas[startIndex].ID == CurRotaionGachaID)
            {   
                break;
            }
            startIndex++;
            afterDay++;
        }

        Debug.LogWarningFormat("afterDay : {0}", afterDay);
        CurRotationDateTime = GameInfo.Instance.ServerRotationGachaData.Infos[0].Time.AddDays(afterDay);        

        IsRotationGachaTimeUpdate = true;
        IsBuyRotationGachaTimeUpdate = false;

        //kRotationRemainTime.textlocalize = GameSupport.GetRemainTimeString(CurRotationDateTime, GameSupport.GetCurrentServerTime());
    }

    private void Renewal_RotationGacha_Circle()
    {
        if (RotationGachaCircleList.Count != GameInfo.Instance.GameTable.RotationGachas.Count)
        {
            for(int i = 0;  i < RotationGachaCircleList.Count; i++)
            {
                if (RotationGachaCircleList[i] == null) continue;
                DestroyImmediate(RotationGachaCircleList[i]);
            }
            RotationGachaCircleList.Clear();
        }

        // 생성
        if (RotationGachaCircleList.Count == 0)
        {
            kRotationGachaSlotCircle.SetActive(false);
            
            for (int i = 0; i < GameInfo.Instance.GameTable.RotationGachas.Count; i++)
            {
                GameObject go = Instantiate(kRotationGachaSlotCircle);

                go.transform.parent = kRotationGachaSlotCircle.transform.parent;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                go.layer = gameObject.layer;

                go.SetActive(true);
                RotationGachaCircleList.Add(go);
            }

            //포지션
            float w = (float)kRotationGachaSlotCircle.GetComponent<UISprite>().width + 1f;
            float totalwidth = w * RotationGachaCircleList.Count;
            float fstartx = -(totalwidth * 0.5f) + (w * 0.5f);

            for (int i = 0; i < RotationGachaCircleList.Count; i++)
            {
                RotationGachaCircleList[i].transform.localPosition = new Vector3(fstartx + ((float)w * i), kRotationGachaSlotCircle.transform.localPosition.y, 1.0f);
                //if (i == 0) SetPositionSelect(i);
            }
        }

        for (int i = 0; i < RotationGachaCircleList.Count; i++)
        {
            UISprite s = RotationGachaCircleList[i].GetComponent<UISprite>();
            if (s == null) continue;

            if (i == (CurRotaionGachaID - 1))
                s.spriteName = "lobbyEventPageSelect";
            else
                s.spriteName = "lobbyEventPageNum";
        }
    }

    private void SetGachaGuerrillaMission(bool isRenewal) {
        //가챠 탭 데이터 가져오기
        if (_selectTab < 0 && _selectTab >= _gachatablist.Count) {
            Debug.LogError("SetGachaGuerrillaMission : _selectTab ID Error");
            return;
        }
        GachaTabParam gachaTabData = _gachatablist[_selectTab];

        GameClientTable.StoreDisplayGoods.Param storedisplaygoods = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.ID == gachaTabData.StoreID1);
        if (storedisplaygoods == null)
            return;

        GameTable.Store.Param storeParam = GameInfo.Instance.GameTable.FindStore(x => x.ID == storedisplaygoods.StoreID);
        if (storeParam == null)
            return;

        //현재 진행중인 게릴라 미션 정보
        List<GuerrillaMissionData> missionDataList = GameInfo.Instance.ServerData.GuerrillaMissionList.FindAll(x => x.Type == "GM_BuyStoreGacha_Cnt" && x.Condition == storeParam.ProductIndex);
        if (null == missionDataList || missionDataList.Count <= 0)
            return;

        //missionData를 GroupOrder 순으로 정렬한다 
        missionDataList.Sort( new Comparison<GuerrillaMissionData>((n1, n2) => n1.GroupOrder.CompareTo(n2.GroupOrder)) );

        //게릴라 미션시간 체크 - 하나라도 조건이 안되면 보이지 않는다.
        for (int i = 0; i < missionDataList.Count; i++) {
            if (GameSupport.IsGuerrillaMissionTimeCheck(missionDataList[i]) == false) {
                return;
            }
        }

        //처음항목 갱신일때는 검증을 위해 0으로 시작한다.
        if (isRenewal == true) {
            _GachaGuerrillaReward.selectIndex = 0;
        }
        else {
            //현재 인덱스 유효성 검증
            if (_GachaGuerrillaReward.selectIndex < 0) {
                _GachaGuerrillaReward.selectIndex = 0;
            }
            else if (_GachaGuerrillaReward.selectIndex >= missionDataList.Count) {
                _GachaGuerrillaReward.selectIndex = missionDataList.Count - 1;
            }
        }

        //갱신 기준이 될 데이터
        GuerrillaMissionData selectData = missionDataList[_GachaGuerrillaReward.selectIndex];

        //현재 선택한 항목의 진행도 최대치
        int curCount = 0;
        bool isNotice = false;

        //서버에 기록된 진행도 가져오기
        GllaMissionData userData = GameInfo.Instance.GllaMissionList.Find(x => x.GroupID == selectData.GroupID);
        if (userData != null) {
            curCount = userData.Count;

            int lastIndex = 0;
            //노티 뜨는 조건 검사 = 받을 수 있는 가장 낮은 보상을 찾는다.
            for (int i = 0; i < missionDataList.Count; i++) {
                lastIndex = i;
                //나보다 작으면 이미 받은 보상이므로 무시한다.
                if (i + 1 < userData.Step)
                    continue;

                //필요량 보다 많거나 같다 -> 보상 받을 수 있는 항목
                if (missionDataList[i].Count <= userData.Count) {
                    isNotice = true;
                }

                break;
            }

            //보상 받을 수 있는 첫 항목이므로 해당 인덱스로 세팅 해 준다.
            if (isRenewal == true) {
                _GachaGuerrillaReward.selectIndex = lastIndex;
                selectData = missionDataList[_GachaGuerrillaReward.selectIndex];
            }
        }

        int maxCount = selectData.Count;
        //진행도가 최대치를 못넘게 처리
        curCount = Mathf.Min(curCount, maxCount);

        //오브젝트 활성화
        _GachaGuerrillaReward.RewardObj.SetActive(true);

        //보상 받기 알림 
        _GachaGuerrillaReward.Notice.SetActive(isNotice);

        //현재 선택 기준으로 슬롯 업데이트
        _GachaGuerrillaReward.Slot.UpdateSlot(selectData, userData);

        //프레임 세팅 - 한정판인지 체크
        Enum.TryParse(gachaTabData?.Value1, out eGachaPickupType value);
        bool bLimited = value == eGachaPickupType.Limit;
        _GachaGuerrillaReward.FrameType1Obj.SetActive(!bLimited);
        _GachaGuerrillaReward.FrameType2Obj.SetActive(bLimited);

        //게이지 세팅
        _GachaGuerrillaReward.GachaGauge.InitGaugeUnit(((float)curCount / (float)maxCount));
        _GachaGuerrillaReward.GachaGauge.SetText(string.Format(FLocalizeString.Instance.GetText(218), curCount, maxCount));

        //화살표 및 가챠게이지 문구 세팅
        if (missionDataList.Count == 1) {
            _GachaGuerrillaReward.GachaRewardText.textlocalize = FLocalizeString.Instance.GetText(1889);

            _GachaGuerrillaReward.LeftArrow.SetActive(false);
            _GachaGuerrillaReward.RightArrow.SetActive(false);
        }
        else {
            _GachaGuerrillaReward.GachaRewardText.textlocalize = string.Format(FLocalizeString.Instance.GetText(1890), selectData.GroupOrder);

            //첫 항목이다.
            if (_GachaGuerrillaReward.selectIndex == 0) {
                _GachaGuerrillaReward.LeftArrow.SetActive(false);
                _GachaGuerrillaReward.RightArrow.SetActive(true);
            }
            //맨 마지막 항목이다
            else if (_GachaGuerrillaReward.selectIndex == missionDataList.Count - 1) {
                _GachaGuerrillaReward.LeftArrow.SetActive(true);
                _GachaGuerrillaReward.RightArrow.SetActive(false);
            }
            //중간 항목이다 
            else {
                _GachaGuerrillaReward.LeftArrow.SetActive(true);
                _GachaGuerrillaReward.RightArrow.SetActive(true);
            }
        }

        //그림자 텍스트
        _GachaGuerrillaReward.GachaRewardText_Shadow.textlocalize = _GachaGuerrillaReward.GachaRewardText.textlocalize;
    }

    private void _UpdateBannerListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIBannerSlot card = slotObject.GetComponent<UIBannerSlot>();
            if (null == card) break;
            card.ParentGO = this.gameObject;

            GachaTabParam data = null;
            if (0 <= index && _gachatablist.Count > index)
                data = _gachatablist[index];
            card.ParentGO = this.gameObject;
            card.UpdateSlot(UIBannerSlot.ePosType.Gacha, index, data);
            
        } while (false);
    }

    private int _GetBannerElementCount()
    {
        return _gachatablist.Count;
    }


    private void _UpdateTicketGachaListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIGachaTicketListSlot card = slotObject.GetComponent<UIGachaTicketListSlot>();
            if (null == card) break;
            card.ParentGO = this.gameObject;

            GameClientTable.StoreDisplayGoods.Param data1 = null;
            GameClientTable.StoreDisplayGoods.Param data2 = null;
            int num1 = index * 2;
            int num2 = num1 + 1;
            if (0 <= num1 && _ticketgachalist.Count > num1)
                data1 = _ticketgachalist[num1];
            if (0 <= num2 && _ticketgachalist.Count > num2)
                data2 = _ticketgachalist[num2];
            card.ParentGO = this.gameObject;
            card.UpdateSlot(index, data1, data2);
        } while (false);
    }

    private int _GetTicketGachaElementCount()
    {
        return _ticketgachalist.Count / 2;
    }

    private void _UpdateDesireShopListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIDesireShopListSlot slot = slotObject.GetComponent<UIDesireShopListSlot>();
            if (null == slot) break;

            GameTable.Store.Param data = null;

            if (0 <= index && _desireshoplist.Count > index)
            {
                data = _desireshoplist[index];
            }

            slot.UpdateSlot(data);
        } while (false);
    }
    
    private int _GetDesireShopElementCount()
    {
        return _desireshoplist.Count;
    }

    private void _UpdateRotationGachaListSlot(int index, GameObject slotObject)
    {
        UIRotationGachaListSlot slot = slotObject.GetComponent<UIRotationGachaListSlot>();
        if (slot == null) return;

        GameTable.RotationGacha.Param p = GameInfo.Instance.GameTable.RotationGachas.Find(x => x.ID == index + 1);
        if (p == null) return;

        slot.UpdateSlot(p);
    }

    private void OnDragFinished_RotationGacha()
    {
        //_RotationGachaList.SetFocus(CurRotaionGachaID - 1, false);
        //Renewal_RotationGacha_Circle();

        ResetRotationGachaWeaponTexList();

        Renewal(false);
    }
    private float _bannerSlotSizeX = 0;
    private void OnPressMoving_RotationGacha()
    {   
        if (_bannerSlotSizeX == 0)
        {   
            UIWidget uIWidget = _RotationGachaList.TargetItem.GetComponent<UIWidget>();
            _bannerSlotSizeX = uIWidget.localSize.x;
        }
        
        //  현재 위치 값
        float xPos = _RotationGachaList.ScrollView.transform.localPosition.x;
        //  현재 위치해야하는 인덱스
        int index = Mathf.Abs(System.Convert.ToInt32(xPos / _bannerSlotSizeX));
                
        //  슬롯이 0이상의 경우(중심이 왼쪽으로 이동하기 때문에 -값)
        if (xPos < 0)
        {
            //  슬롯의 / 2 크기로 높은지 낮은지를 판단
            float value = xPos % _bannerSlotSizeX;
            //  근사치 인덱스 셋팅
            if (value > _bannerSlotSizeX * 0.5f)
                index += 1;
        }
        else
        {
            index = (int)eCOUNT.NONE;
        }        

        //  슬롯이 배너 갯수를 넘어간 경우        
        if (index > GameInfo.Instance.GameTable.RotationGachas.Count)
        {
            index = (int)eCOUNT.NONE;
        }
        

        ////  서클로 현재 가운데 선택된 배너가 몇번째인지를 표시해줍니다.
        //if (_bannerCircleList.Count != 0)
        //{
        //    //if (index >= _bannerCircleList.Count)
        //    //    index = _bannerCircleList.Count - 1;

        //    //if (index >= 0)
        //    //    kSelectCircle.transform.localPosition = _bannerCircleList[index].transform.localPosition;
        //}

        CurRotaionGachaID = index + 1;
        if (CurRotaionGachaID > GameInfo.Instance.GameTable.RotationGachas.Count)
            CurRotaionGachaID = GameInfo.Instance.GameTable.RotationGachas.Count;
    }

    private void OnStoppedMoving_RotationGacha()
    {
        SpringPanel springPanel = _RotationGachaList.Panel.GetComponent<SpringPanel>();
        if (springPanel == null)
        {
            return;
        }

        if (springPanel.enabled)
        {
            return;
        }

        if (Math.Abs(springPanel.target.x - _RotationGachaList.Panel.transform.localPosition.x) < 1)
        {
            return;
        }

        springPanel.enabled = true;
    }

    public void SetSeleteTab(int tab)
    {
        if (!_selectTab.Equals(tab))
        {
            _selectTab = tab;

            if (_gachatablist[_selectTab].Type == "ROTATION")
            {
                CurRotaionGachaID = GameInfo.Instance.ServerRotationGachaData.Infos[0].RemainCount;

                SpringPanel springPanel = _RotationGachaList.Panel.GetComponent<SpringPanel>();
                if (springPanel != null)
                {
                    springPanel.target.x = Mathf.Abs(_RotationGachaList.Panel.transform.localPosition.x) * -1;
                }
            }
            
            Renewal(true);
        }
    }
   
	public void OnClick_PercentageBtn()
	{   
        UIGachaBtnUnit unit = null;
        if (_gachatablist[_selectTab].Type == "ROTATION")
        {
            //unit = kRotationGachaBtnUnitList[0];
            GameTable.RotationGacha.Param data = GameInfo.Instance.GameTable.RotationGachas[CurRotaionGachaID - 1];
            UIValue.Instance.SetValue(UIValue.EParamType.GachaDetailStoreID, data.StoreID1);
        }
        else
        {
            unit = kGachaBtnUnitList[0];

            if (unit.StoreDisplayTable == null)
                return;

            UIValue.Instance.SetValue(UIValue.EParamType.GachaDetailStoreID, unit.StoreDisplayTable.StoreID);
        }
        
        LobbyUIManager.Instance.ShowUI("GachaDetailPopup", true);
	}

    public void OnClick_CommercialBtn()
    {

    }

    public void OnClick_GachaBtnUnit_00()
	{
        OnClick_GachaBtnUnit(0);
    }
	
	public void OnClick_GachaBtnUnit_01()
	{
        OnClick_GachaBtnUnit(1);
    }

	void OnClick_GachaBtnUnit( int index )
    {
        ServerData serverdata = GameInfo.Instance.ServerData;
		CurClickedGachaBtn = null;

		if (_gachatablist[_selectTab].Type == "ROTATION")
        {
			CurClickedGachaBtn = kRotationGachaBtnUnitList[index];
			if(CurClickedGachaBtn == null)
			{
				return;
			}

            if (CurClickedGachaBtn.StoreTicketDisplayTable != null)
                SendStoreDisplayID(CurClickedGachaBtn.StoreTicketDisplayTable.ID);
            else
                SendStoreDisplayID(CurClickedGachaBtn.StoreDisplayTable.ID);
        }
        else
        {
			CurClickedGachaBtn = kGachaBtnUnitList[index];
			if(CurClickedGachaBtn == null || (CurClickedGachaBtn.DisablePurchase && CurClickedGachaBtn.DisablePurchase.activeSelf))
			{
				return;
			}

            if (CurClickedGachaBtn.StoreTicketDisplayTable != null)
                SendStoreDisplayID(CurClickedGachaBtn.StoreTicketDisplayTable.ID);
            else
                SendStoreDisplayID(CurClickedGachaBtn.StoreDisplayTable.ID);
        }
    }

    public void OnClick_Purchase_RotationGachaTime()
    {
        var gachaTable = GameInfo.Instance.GameTable.RotationGachas.Find(x => x.ID == CurRotaionGachaID);
        if (gachaTable == null)
        {
            // 테이블 정보를 찾을 수 없음
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(200004));
            return;
        }

        bool bisSale = false;
        int openCash = gachaTable.OpenCash;
        GuerrillaCampData guerrillaCampData = GameSupport.GetOnGuerrillaCampaignType(eGuerrillaCampaignType.GC_Rotation_OpenCashSale, (int)eCOUNT.NONE);
        if (guerrillaCampData != null)
        {
            bisSale = true;
            int discount = (int)(openCash * (float)((float)guerrillaCampData.EffectValue / (float)eCOUNT.MAX_BO_FUNC_VALUE));
            openCash -= discount;
        }

        long cashCount = GameInfo.Instance.UserData.GetGoods(eGOODSTYPE.CASH);
        if (cashCount <= 0 || cashCount < openCash)
        {
            // 대마석 부족
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(101));
            return;
        }

        System.TimeSpan t = GameSupport.GetRemainTime(GameInfo.Instance.ServerRotationGachaData.Infos[0].Time, GameInfo.Instance.GetNetworkTime());
        if (t.TotalSeconds <= GameInfo.Instance.GameConfig.RotationGachaCheckTime)
        {
            MessagePopup.OKCANCEL(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3277), 
                () => {
                    BuyPopup.Show(FLocalizeString.Instance.GetText(1717), FLocalizeString.Instance.GetText(1719, t.Minutes), false, bisSale, eREWARDTYPE.GOODS, eGOODSTYPE.CASH, gachaTable.OpenCash, openCash, OnCallBack_BuyPopup_PurchaseRotationGachaTime, null);
                }, 
                () => { });
            return;
        }


        BuyPopup.Show(FLocalizeString.Instance.GetText(1717), FLocalizeString.Instance.GetText(1718, t.Hours, t.Minutes), false, bisSale, eREWARDTYPE.GOODS, eGOODSTYPE.CASH, gachaTable.OpenCash, openCash, OnCallBack_BuyPopup_PurchaseRotationGachaTime, null);
    }

    public void OnClick_RotationRightArrowBtn()
    {
        Log.Show("OnClick_RotationRightArrowBtn");
        CurRotaionGachaID++;
        if (CurRotaionGachaID > GameInfo.Instance.GameTable.RotationGachas.Count)
            CurRotaionGachaID = GameInfo.Instance.GameTable.RotationGachas.Count;

        ResetRotationGachaWeaponTexList();

        Renewal(false);
    }

    public void OnClick_RotationLeftArrowBtn()
    {
        Log.Show("OnClick_RotationLeftArrowBtn", Log.ColorType.Red);
        CurRotaionGachaID--;
        if (CurRotaionGachaID < (int)eCOUNT.NONE)
            CurRotaionGachaID = (int)eCOUNT.NONE + 1;

        ResetRotationGachaWeaponTexList();

        Renewal(false);
    }


    public void OnClick_GachaGuerrilaRightArrowBtn() {
        //우측 버튼 >
        _GachaGuerrillaReward.selectIndex++;

        SetGachaGuerrillaMission(false);
    }

    public void OnClick_GachaGuerrilaLeftArrowBtn() {
        //좌측 버튼 <
        _GachaGuerrillaReward.selectIndex--;

        SetGachaGuerrillaMission(false);
    }


    private void ResetRotationGachaWeaponTexList()
    {
        GameObject obj = _RotationGachaList.GetItem(CurRotaionGachaID - 1);
        if (obj == null)
        {
            return;
        }

        UIRotationGachaListSlot slot = obj.GetComponent<UIRotationGachaListSlot>();
        if (slot == null)
        {
            return;
        }

        slot.ResetWeaponFList();
    }


    public void SendStoreDisplayID(int displayid)
    {
        if (!GameSupport.IsCheckInven())
            return;
        var storeDisplayTable = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(displayid);
        if (storeDisplayTable == null)
            return;
        var storeTable = GameInfo.Instance.GameTable.FindStore(storeDisplayTable.StoreID);
        if (storeTable == null)
            return;

        bool bFree = false;

        //var storesaledata = GameInfo.Instance.ServerData.StoreSaleList.Find(x => x.TableID == storetable.ID);
        int discountRate = GameSupport.GetStoreDiscountRate(storeTable.ID);
        bool bSaleApply = GameSupport.IsStoreSaleApply(storeTable.ID);
        int saleValue = storeTable.PurchaseValue;

        if (bSaleApply)
        {
            if (discountRate == 100)
                bFree = true;
            else
                saleValue = storeTable.PurchaseValue - (int)(storeTable.PurchaseValue * (float)((float)discountRate / (float)eCOUNT.MAX_BO_FUNC_VALUE));
        }

        if (!GameSupport.IsTutorial())
        {
            if (!bFree)
            {
                if (storeTable.PurchaseType == (int)eREWARDTYPE.GOODS)
                {
                    if (!GameSupport.IsCheckGoods((eGOODSTYPE)storeTable.PurchaseIndex, saleValue))
                        return;
                }
                else if (storeTable.PurchaseType == (int)eREWARDTYPE.ITEM)
                {
                    int count = GameInfo.Instance.GetItemIDCount(storeTable.PurchaseIndex);
                    if (storeTable.PurchaseValue > count)
                    {
                        var tabledata = GameInfo.Instance.GameTable.FindItem(storeTable.PurchaseIndex);
                        if (tabledata != null)
                        {
                            string message = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR), FLocalizeString.Instance.GetText(tabledata.Name));
                            MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3071), message));
                        }
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        _sendstoreid = storeTable.ID;
        _bsendfree = bFree;

        //골드 10연차일 경우에만 AutoGacha 쪽을 띄워준다.
        if (displayid == 2202) {
            UIAutoGachaPopup autoGachaPopup = LobbyUIManager.Instance.GetUI<UIAutoGachaPopup>("AutoGachaPopup");
            if (autoGachaPopup == null) {
                Debug.LogError("오토 가챠 팝업 없음");
                return;
            }
            autoGachaPopup.SetData(storeDisplayTable, storeTable, saleValue, bSaleApply, bFree, OnMsg_Purchase);
            autoGachaPopup.SetUIActive(true);

            //오토가챠인지?
            mIsAutoGacha = true;

            //가챠 연출 보여줄지 여부
            mIsAutoGachaDiretor = true;
        }
        else {
            string strtext1 = string.Format(FLocalizeString.Instance.GetText(112), FLocalizeString.Instance.GetText(storeDisplayTable.Name), FLocalizeString.Instance.GetText(storeDisplayTable.Count));
            string strtext2;

            if (bFree)
                strtext2 = FLocalizeString.Instance.GetText(114);
            else
                strtext2 = FLocalizeString.Instance.GetText(113);

            BuyPopup.Show(strtext1, strtext2, bFree, bSaleApply, (eREWARDTYPE)storeTable.PurchaseType, (eGOODSTYPE)storeTable.PurchaseIndex, storeTable.PurchaseValue, saleValue, OnMsg_Purchase, OnMsg_Purchase_Cancel);
           
            mIsAutoGacha = false;
        }
    }

    public void ReGacha()
    {
        if (_sendstoreid == -1)
            return;

        var storedisplaytable = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x=>x.StoreID == _sendstoreid);
        if (storedisplaytable == null)
            return;

        SendStoreDisplayID(storedisplaytable.ID);
    }

  

    public void OnMsg_Purchase()
    {
		if ( mIsSendGachaPacket ) {
			return;
		}

		mIsSendGachaPacket = true;

        _cardbooklist.Clear();
        for (int i = 0; i < GameInfo.Instance.CardBookList.Count; i++)
        {
            CardBookData cardbookdata = GameInfo.Instance.CardBookList[i];
            _cardbooklist.Add(new CardBookData(cardbookdata.TableID, cardbookdata.StateFlag));
        }

        GameInfo.Instance.Send_ReqStorePurchase(_sendstoreid, _bsendfree, 1, OnNetPurchase);
    }

    public void OnMsg_Purchase_Cancel()
    {
        
    }

    public void OnNetPurchase(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        Log.Show("!@#!@#!@#!@#", Log.ColorType.Yellow);

		mIsSendGachaPacket = false;

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

            for ( int i = 0; i < pktInfoStorePurchase.products_.goodsInfos_.Count; i++ )
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

                int rand = Random.Range(0, i);
                GameInfo.Instance.RewardList.Insert(rand, data);
            }

            for (int i = 0; i < GameInfo.Instance.RewardList.Count; i++)
            {
                RewardData reward = GameInfo.Instance.RewardList[i];
                if (reward.Type == (int)eREWARDTYPE.CARD)
                {
                    var data = _cardbooklist.Find(x => x.TableID == reward.Index);
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
                int rand = Random.Range(0, i);
                GameInfo.Instance.RewardList.Insert(rand, tempRewardList[i]);
            }

            for (int i = 0; i < GameInfo.Instance.RewardList.Count; i++)
            {
                RewardData reward = GameInfo.Instance.RewardList[i];
                if (reward.Type == (int)eREWARDTYPE.CARD)
                {
                    var data = _cardbooklist.Find(x => x.TableID == reward.Index);
                    if (data == null)
                        GameInfo.Instance.RewardList[i].bNew = true;
                }
            }
        }

        GameClientTable.StoreDisplayGoods.Param sddata = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == _sendstoreid);
        if (sddata == null)
            return;

        //Debug.LogError(sddata.DrtType);

        //오토 가챠인지?
        if (mIsAutoGacha == true) {
            //오토 가챠 연출이 나와야 하는지?
            if (mIsAutoGachaDiretor == true) {
                DirectorUIManager.Instance.PlayGachaGold(true);
            }
            else {
                UIAutoGachaResultPopup autoGachaResultPopup = LobbyUIManager.Instance.GetUI<UIAutoGachaResultPopup>("AutoGachaResultPopup");
                if (autoGachaResultPopup == null) {
                    Debug.LogError("오토 가챠 결과 팝업 없음");
                }
                else {
                    //autoGachaResultPopup.SetUIActive(true);
                    autoGachaResultPopup.RefreshGachaResult();
                }
            }

            UIAutoGachaPopup autoGachaPopup = LobbyUIManager.Instance.GetUI<UIAutoGachaPopup>("AutoGachaPopup");
            if (autoGachaPopup == null) {
                Debug.LogError("오토 가챠 팝업 없음");
            }
            else {
                //UI에서 가져온 값 -> autoGachaPopup.IsSkipDirector가 true이면 연출을 하지않는다.
                mIsAutoGachaDiretor = !autoGachaPopup.IsSkipDirector;
            }
        }
        else {
            if (sddata.DrtType == 1) {
                DirectorUIManager.Instance.PlayGacha(1);
            }
            else if (sddata.DrtType == 2) {
                DirectorUIManager.Instance.PlayGacha(2);
            }
            else if (sddata.DrtType == 3) {
                DirectorUIManager.Instance.PlayGacha(3);
            }
            else if (sddata.DrtType == 4) {
                DirectorUIManager.Instance.PlayGachaGold(false);
            }
            else {
                DirectorUIManager.Instance.PlayGachaRestoration();
            }
        }


        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");

        Renewal(true);
    }


    private void OnCallBack_BuyPopup_PurchaseRotationGachaTime()
    {
        List<uint> ids = new List<uint>();
        ids.Add((uint)CurRotaionGachaID);
        GameInfo.Instance.Send_ReqUserRotationGachaOpen(ids, OnNetPurchase_RotationGachaTime);
    }

    private void OnNetPurchase_RotationGachaTime(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");

        Renewal(false);
    }
    public override bool IsBackButton()
    {
        return m_isBackButton;
    }

    private void GetBGTexture(UITexture target, string url, bool platform, bool localize)
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
            if(target.mainTexture == null)
            {
                _bgLoadCoroutine = StartCoroutine(GetBGTextureAsync(target, url));
            }
        }
        else
        {
            target.mainTexture = (Texture2D)ResourceMgr.Instance.LoadFromAssetBundle("temp", "Temp/" + url);
        }
    }

    IEnumerator GetBGTextureAsync(UITexture target, string url)
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
}
