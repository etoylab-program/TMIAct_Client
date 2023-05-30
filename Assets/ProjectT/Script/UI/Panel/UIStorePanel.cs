using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum eStorePanelType
{
    Cash = 0,
    Package,
    Main,
    Character,
    CostumeMain,
    CostumeSub,
    Etc_Tab,
    Etc,
    Etc_Event,
}

[Serializable]
public class StorePanelActive
{
    public eStorePanelType ViewType;
    public List<GameObject> ObjList;
}

public class UIStorePanel : FComponent
{
    public enum eStoreTabType
    {
        _START_ = 0,
        STORE_CHAR,             //캐릭터
        STORE_COSTUME,          //코스튬
        STORE_POINT,            //마일리지
        STORE_ETC,              //물자조달
        STORE_FRIENDPOINT,      //친구포인트
        STORE_EVENT,            //이벤트스토어
        _MAX_,

        STORE_COSTUME_SUB = 9,  //코스튬 서브
    }

    public enum eCharInfoType
    {
        View,
        CashBuy,
        Date,
        NotPurchase_Info,
        NotPurchase_Buy,
    }
	
    [Header("StorePanel")]
    [SerializeField] private List<StorePanelActive> storePanelActiveList = null;

    [Header("Character")]
    [SerializeField] private FList characterList = null;
    [SerializeField] private UILabel characterNameLabel = null;
    [SerializeField] private UITexture characterIconTex = null;
    [SerializeField] private GameObject apObj = null;
    [SerializeField] private UILabel apLabel = null;
    [SerializeField] private UISprite supportTypeSpr = null;
    [SerializeField] private UILabel supportTypeLabel = null;
    [SerializeField] private UISprite tribeTypeSpr = null;
    [SerializeField] private UILabel tribeTypeLabel = null;
    [SerializeField] private UILabel speakingLabel = null;
    [SerializeField] private GameObject basicGoodsObj = null;
    [SerializeField] private UIGoodsUnit basicGoodsUnit = null;
    [SerializeField] private GameObject saleGoodsObj = null;
    [SerializeField] private UIGoodsUnit saleGoodsUnit = null;
    [SerializeField] private UILabel saleValueLabel = null;
    [SerializeField] private FLocalizeText costumeInfoText = null;
    [SerializeField] private FLocalizeText packageInfoText = null;
    [SerializeField] private UIButton buyBtn = null;
    [SerializeField] private UIButton charInfoBtn = null;
    [SerializeField] private UIButton packageBuyBtn = null;

    [Header("Costume")]
    [SerializeField] private FList costumeList = null;

    [Header("Etc")]
    [SerializeField] private FTab etcTab = null;
    [SerializeField] private FList etcList = null;

    [Header("Etc Event")]
    [SerializeField] private FList etcEventList = null;

    private eStorePanelType _storePanelType = eStorePanelType.Main;
    private eStoreTabType _storeTabType = eStoreTabType._START_;
    private GameTable.Character.Param _curCharParam = null;
    private GameTable.Store.Param _curStoreParam = null;
    private GameClientTable.StoreDisplayGoods.Param _curStoreDisplayGoodsParam = null;
    private StoreSaleData _curStoreSaleData = null;

    private List<GameTable.Character.Param> _characterParamList = new List<GameTable.Character.Param>();
    private List<GameClientTable.StoreDisplayGoods.Param> _costumeMainParamList = new List<GameClientTable.StoreDisplayGoods.Param>();
    private List<GameClientTable.StoreDisplayGoods.Param> _costumeSubParamList = new List<GameClientTable.StoreDisplayGoods.Param>();
    private List<int> _costumeSaleList = new List<int>();
    private List<GameClientTable.StoreDisplayGoods.Param> _etcParamList = new List<GameClientTable.StoreDisplayGoods.Param>();
    private List<GameClientTable.StoreDisplayGoods.Param> _etcEventParamList = new List<GameClientTable.StoreDisplayGoods.Param>();

    public eStoreTabType EtcTabSelect => (eStoreTabType)etcTab.kSelectTab + 1;
    public eStorePanelType StorePanelType => _storePanelType;
    public int CharacterSlotIndex { get; private set; }
    public int CostumeSlotIndex { get; private set; }

    public override void Awake()
	{
		base.Awake();

        characterList.EventUpdate = UpdateCharacterSlot;
        characterList.EventGetItemCount = GetCharacterCount;
        characterList.UpdateList();

        costumeList.EventUpdate = UpdateCostumeSlot;
        costumeList.EventGetItemCount = GetCostumeCount;
        costumeList.UpdateList();

        etcTab.EventCallBack = OnEventTabSelect;
        etcList.EventUpdate = UpdateEtcSlot;
        etcList.EventGetItemCount = GetEtcCount;
        etcList.InitBottomFixing();
        etcList.UpdateList();

        etcEventList.EventUpdate = UpdateEtcEventSlot;
        etcEventList.EventGetItemCount = GetEtcEventCount;
        etcEventList.InitBottomFixing();
        etcEventList.UpdateList();
    }

	public override void OnEnable()
	{
        InitComponent();
        base.OnEnable();
    }
    
    public override void InitComponent()
    {
        base.InitComponent();

        CharacterSlotIndex = 0;

        SetCharTableData();

        int eventTabIndex = (int)eStoreTabType.STORE_EVENT - 1;
        if (eventTabIndex < etcTab.kBtnList.Count)
        {
            _etcEventParamList.Clear();

            if (AppMgr.Instance.HasContentFlag(eContentType.STORE_EVENT))
            {
                List<GameClientTable.StoreDisplayGoods.Param> storeDisplayGoodsList =
                GameInfo.Instance.GameClientTable.FindAllStoreDisplayGoods(x => x.PanelType == (int)eSD_PanelType.STORE && x.Category == 14);

                foreach (GameClientTable.StoreDisplayGoods.Param storeDisplayGoods in storeDisplayGoodsList)
                {
                    if (!GameSupport.IsStoreGoodsTimeExpireByGachaCategoryData(storeDisplayGoods, GameSupport.GetCurrentServerTime()))
                    {
                        continue;
                    }

                    _etcEventParamList.Add(storeDisplayGoods);
                }
            }

            etcTab.kBtnList[eventTabIndex].SetActive(0 < _etcEventParamList.Count);
        }
    }
    
    public override void Renewal(bool bChildren = false)
    {
        base.Renewal(bChildren);

        foreach (StorePanelActive storePanelActive in storePanelActiveList)
        {
            bool activeSelf = storePanelActive.ViewType == _storePanelType;

            switch(_storePanelType)
            {
                case eStorePanelType.CostumeSub:
                    {
                        activeSelf = storePanelActive.ViewType == eStorePanelType.CostumeMain;
                    } break;
                case eStorePanelType.Etc:
                case eStorePanelType.Etc_Event:
                    {
                        if (storePanelActive.ViewType == eStorePanelType.Etc_Tab)
                        {
                            activeSelf = true;
                        }
                    } break;
            }

            foreach (GameObject activeObj in storePanelActive.ObjList)
            {
                activeObj.SetActive(activeSelf);
            }
        }

        switch (_storePanelType)
        {
            case eStorePanelType.Character:
                {
                    characterList.RefreshNotMoveAllItem();
                    characterList.SpringSetFocus(CharacterSlotIndex, 0.5f);

                    if (_curCharParam != null)
                    {
                        FLocalizeString.SetLabel(speakingLabel, _curCharParam.Name + 500000);

                        characterNameLabel.textlocalize = FLocalizeString.Instance.GetText(_curCharParam.Name);
                        characterIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", $"Icon/Char/Full/Full_{_curCharParam.Icon}.png");
                        supportTypeSpr.spriteName = GameSupport.GetCardTypeSpriteName((eSTAGE_CONDI)_curCharParam.Type);
                        supportTypeLabel.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.CARDTYPE + _curCharParam.Type);
                        tribeTypeSpr.spriteName = $"TribeType_{_curCharParam.MonType}";
                        tribeTypeLabel.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.MON_TYPE_TEXT_START + _curCharParam.MonType);

                        eCharInfoType charInfoType = eCharInfoType.View;
                        CharData charData = GameInfo.Instance.GetCharDataByTableID(_curCharParam.ID);
                        if (charData == null)
                        {
                            charInfoType = eCharInfoType.CashBuy;

                            if (_curStoreDisplayGoodsParam != null)
                            {
                                if (_curStoreDisplayGoodsParam.StoreID == -1 && !string.IsNullOrEmpty(_curStoreDisplayGoodsParam.HideConType))
                                {
                                    if (_curStoreDisplayGoodsParam.Category == (int)UIStorePanel.eStoreTabType.STORE_CHAR)
                                    {
                                        if (_curStoreDisplayGoodsParam.HideConType.Equals("HCT_NFS"))
                                        {
                                            if (_curStoreDisplayGoodsParam.HideConValue <= 0)
                                            {
                                                charInfoType = eCharInfoType.NotPurchase_Info;
                                            }
                                            else
                                            {
                                                charInfoType = eCharInfoType.NotPurchase_Buy;
                                            }
                                        }
                                        else
                                        {
                                            charInfoType = eCharInfoType.Date;
                                        }
                                    }
                                }
                            }
                        }
                        
                        buyBtn.SetActive(charInfoType == eCharInfoType.CashBuy);
                        apObj.SetActive(buyBtn.gameObject.activeSelf);
                        basicGoodsObj.SetActive(_curStoreSaleData == null && buyBtn.gameObject.activeSelf);
                        saleGoodsObj.SetActive(_curStoreSaleData != null && buyBtn.gameObject.activeSelf);

                        charInfoBtn.SetActive(charInfoType == eCharInfoType.View);
                        packageBuyBtn.SetActive(charInfoType == eCharInfoType.NotPurchase_Buy);

                        apLabel.textlocalize = string.Empty;
                        costumeInfoText.SetLabel(0);
                        packageInfoText.SetLabel(0);
                        switch (charInfoType)
                        {
                            case eCharInfoType.CashBuy:
                                {
                                    if (_curStoreParam != null)
                                    {
                                        if (saleGoodsObj.activeSelf)
                                        {
                                            saleValueLabel.textlocalize = _curStoreParam.PurchaseValue.ToString();
                                            saleGoodsUnit.InitGoodsUnit((eGOODSTYPE)_curStoreParam.PurchaseIndex, GameSupport.GetDiscountPrice(_curStoreParam.PurchaseValue, _curStoreSaleData));
                                        }
                                        else
                                        {
                                            basicGoodsUnit.InitGoodsUnit(_curStoreParam);
                                        }
                                    }

                                    apLabel.textlocalize = FLocalizeString.Instance.GetText(1579, GameSupport.GetMaxAP() + GameInfo.Instance.GameConfig.CharOpenAddAP, "", GameInfo.Instance.GameConfig.CharOpenAddAP);
                                } break;
                            case eCharInfoType.NotPurchase_Info:
                                {
                                    costumeInfoText.SetLabel(3302);
                                } break;
                            case eCharInfoType.NotPurchase_Buy:
                                {
                                    packageInfoText.SetLabel(3302);
                                } break;
                            case eCharInfoType.Date:
                                {
                                    costumeInfoText.SetLabel(FLocalizeString.Instance.GetText(305, GameSupport.GetStartTime(GameSupport.GetTimeWithString(_curStoreDisplayGoodsParam.HideConType, true))));
                                } break;
                        }
                    }
                } break;
            case eStorePanelType.CostumeMain:
                {
                    costumeList.Reset();
                    costumeList.SpringSetFocus(CostumeSlotIndex, 0.5f, true);
                    costumeList.ScrollViewCheck();
                } break;
            case eStorePanelType.CostumeSub:
                {
                    costumeList.RefreshNotMoveAllItem();
                } break;
            case eStorePanelType.Etc:
                {
                    switch(_storeTabType)
                    {
                        case eStoreTabType.STORE_POINT:
                        case eStoreTabType.STORE_FRIENDPOINT:
                        case eStoreTabType.STORE_ETC:
                            {
                                etcTab.SetTab((int)_storeTabType - 1, SelectEvent.Code);
                            } break;
                    }
                    etcList.Reset();
                } break;
            case eStorePanelType.Etc_Event:
                {
                    switch (_storeTabType)
                    {
                        case eStoreTabType.STORE_EVENT:
                            {
                                etcTab.SetTab((int)_storeTabType - 1, SelectEvent.Code);
                            }
                            break;
                    }
                    etcEventList.Reset();
                } break;
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();

        if (_storePanelType == eStorePanelType.Character || _storePanelType == eStorePanelType.CostumeSub)
        {
            return;
        }

        ResetValue();
    }

#region FList & FTab
    private void UpdateCharacterSlot(int index, GameObject slotObject)
    {
        UICharListSlot slot = slotObject.GetComponent<UICharListSlot>();
        if (slot == null)
        {
            return;
        }

        if (index < 0 || _characterParamList.Count <= index)
        {
            return;
        }

        if (slot.ParentGO == null)
        {
            slot.ParentGO = this.gameObject;
        }

        slot.UpdateSlot(UICharListSlot.ePos.Store, index, _characterParamList[index]);
    }

    private int GetCharacterCount()
    {
        return _characterParamList.Count;
    }

	private void UpdateCostumeSlot( int index, GameObject slotObject ) {
		UIStoreListSlot slot = slotObject.GetComponent<UIStoreListSlot>();
		if ( slot == null ) {
			return;
		}

		List<GameClientTable.StoreDisplayGoods.Param> costumeList;
		if ( _storePanelType == eStorePanelType.CostumeSub ) {
			costumeList = _costumeSubParamList;
		}
		else {
			costumeList = _costumeMainParamList;
		}

		if ( index < 0 || costumeList.Count <= index ) {
			return;
		}

		if ( slot.ParentGO == null ) {
			slot.ParentGO = this.gameObject;
		}

		GameClientTable.StoreDisplayGoods.Param storeDisplayGoodsParam = costumeList[index];
		slot.UpdateSlot( index, storeDisplayGoodsParam );
		slot.ShowSaleMark( _costumeSaleList.Any( x => x == storeDisplayGoodsParam.ID ) );
	}

	private int GetCostumeCount()
    {
        int count;
        if (_storePanelType == eStorePanelType.CostumeSub)
        {
            count = _costumeSubParamList.Count;
        }
        else
        {
            count = _costumeMainParamList.Count;
        }

        return count;
    }

    private bool OnEventTabSelect(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Enable)
        {
            return false;
        }

        _storeTabType = (eStoreTabType)nSelect + 1;

        if (_storeTabType == eStoreTabType.STORE_EVENT)
        {
            _storePanelType = eStorePanelType.Etc_Event;
        }
        else
        {
            _etcParamList = GameInfo.Instance.GameClientTable.FindAllStoreDisplayGoods(x => x.PanelType == (int)eSD_PanelType.STORE && x.Category == etcTab.kSelectTab + 1);
            _storePanelType = eStorePanelType.Etc;
        }

        if (type == SelectEvent.Click)
        {
            Renewal();
        }

        LobbyUIManager.Instance.Renewal("TopPanel");

        return true;
    }

    private void UpdateEtcSlot(int index, GameObject slotObject)
    {
        UIEventStoreListSlot slot = slotObject.GetComponent<UIEventStoreListSlot>();
        if (slot == null)
        {
            return;
        }

        if (index < 0 || _etcParamList.Count <= index)
        {
            return;
        }

        if (slot.ParentGO == null)
        {
            slot.ParentGO = this.gameObject;
        }

        slot.UpdateSlot(_etcParamList[index]);
    }

    private int GetEtcCount()
    {
        return _etcParamList.Count;
    }

    private void UpdateEtcEventSlot(int index, GameObject slotObject)
    {
        UIEventStoreListSlot slot = slotObject.GetComponent<UIEventStoreListSlot>();
        if (slot == null)
        {
            return;
        }

        if (index < 0 || _etcEventParamList.Count <= index)
        {
            return;
        }

        if (slot.ParentGO == null)
        {
            slot.ParentGO = this.gameObject;
        }

        slot.UpdateSlot(_etcEventParamList[index]);
    }

    private int GetEtcEventCount()
    {
        return _etcEventParamList.Count;
    }
	#endregion

	#region OnClick Function
	public void OnClick_MainBtn( int index ) {
		bool isRenewal = false;

		eStorePanelType storePanelType = (eStorePanelType)index;
		switch ( storePanelType ) {
			case eStorePanelType.Cash: {
				GameSupport.PaymentAgreement_Cash();
			}
			break;
			case eStorePanelType.Package: {
				GameSupport.PaymentAgreement_Package();
			}
			break;
			case eStorePanelType.Character: {
				isRenewal = true;

				if ( _curStoreDisplayGoodsParam != null && _curStoreParam != null ) {
					if ( _curStoreDisplayGoodsParam.StoreID != _curStoreParam.ID ) {
						_curStoreParam = GameInfo.Instance.GameTable.FindStore( x => x.ID == _curStoreDisplayGoodsParam.StoreID );
					}
				}

				SoundManager.Instance.PlayUISnd( 41 );
			}
			break;
			case eStorePanelType.CostumeMain: {
				isRenewal = true;
				SoundManager.Instance.PlayUISnd( 41 );
			}
			break;
			case eStorePanelType.Etc: {
				isRenewal = true;
				_storeTabType = eStoreTabType.STORE_POINT;

				SoundManager.Instance.PlayUISnd( 41 );
			}
			break;
		}

		if ( isRenewal ) {
			_storePanelType = storePanelType;
			Renewal();
		}
	}

	public void OnClick_CharInfoBtn() {
		if ( _curCharParam == null ) { // || _curStoreParam == null ) {
			return;
		}

		CharData charData = GameInfo.Instance.GetCharDataByTableID( _curCharParam.ID );
		if ( charData == null ) {
			return;
		}

		UIValue.Instance.SetValue( UIValue.EParamType.CharSelUID, charData.CUID );
		UIValue.Instance.SetValue( UIValue.EParamType.CharSelTableID, charData.TableID ); // _curStoreParam.ProductIndex );
		UIValue.Instance.SetValue( UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.STATUS );
		LobbyUIManager.Instance.SetPanelType( ePANELTYPE.CHARINFO );
	}

	public void OnClick_CharBuyBtn()
    {
        if (_curStoreParam == null)
        {
            return;
        }

        long price = GameSupport.GetDiscountPrice(_curStoreParam.PurchaseValue, _curStoreSaleData);
        if (!GameSupport.IsCheckGoods((eGOODSTYPE)_curStoreParam.PurchaseType, price))
        {
            return;
        }

        if (_curStoreDisplayGoodsParam == null)
        {
            return;
        }

        StoreBuyPopup.Show(_curStoreDisplayGoodsParam, ePOPUPGOODSTYPE.MPOINT, OnMsg_Purchase, null);
    }

    public void OnClick_PackageBuyBtn()
    {
        if (_curStoreDisplayGoodsParam == null)
        {
            return;
        }

        GameClientTable.Acquisition.Param acquisitionParam = GameInfo.Instance.GameClientTable.FindAcquisition(_curStoreDisplayGoodsParam.HideConValue);
        if (acquisitionParam == null)
        {
            return;
        }

        GameSupport.MoveUI(acquisitionParam.Type, acquisitionParam.Value1, acquisitionParam.Value2, acquisitionParam.Value3);
    }

    public void OnClick_CostumeBuyBtn()
    {
        CharData charData = GameInfo.Instance.GetCharDataByTableID(_curStoreParam.Value1);
        if (charData == null)
        {
            MessagePopup.OKCANCEL(eTEXTID.OK, 3051, () =>
            {
                DirectShow(eStoreTabType.STORE_CHAR, _curStoreParam.Value1);
                Renewal();
            });
            return;
        }

        UIValue.Instance.SetValue(UIValue.EParamType.CharSelUID, charData.CUID);
        UIValue.Instance.SetValue(UIValue.EParamType.CharSelTableID, _curStoreParam.Value1);
        UIValue.Instance.SetValue(UIValue.EParamType.CharCostumeID, _curStoreParam.ProductIndex);
        UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.COSTUME);
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.CHARINFO);
        LobbyUIManager.Instance.StorePanelUpdateSlotList = false;
    }

    public void OnClick_BookBtn()
    {
        if (_curCharParam == null)
        {
            return;
        }

        UIValue.Instance.SetValue(UIValue.EParamType.BookItemID, _curCharParam.ID);

        LobbyUIManager.Instance.ShowUI("BookCharInfoPopup", true);
    }
#endregion

    private void SetCharTableData()
    {
        if (0 <= CharacterSlotIndex && CharacterSlotIndex < _characterParamList.Count)
        {
            _curCharParam = _characterParamList[CharacterSlotIndex];

            if (_curCharParam != null)
            {
                _curStoreParam = GameInfo.Instance.GameTable.FindStore(x => x.ProductType == (int)eREWARDTYPE.CHAR && x.ProductIndex == _curCharParam.ID);
                if (_curStoreParam != null)
                {
                    _curStoreSaleData = GameInfo.Instance.ServerData.StoreSaleList.Find(x => x.TableID == _curStoreParam.ID);
                    _curStoreDisplayGoodsParam = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.CharacterID == _curCharParam.ID);
                }
            }
        }
        else
        {
            _curCharParam = null;
            _curStoreParam = null;
            _curStoreDisplayGoodsParam = null;
            _curStoreSaleData = null;
        }
    }

    private void OnMsg_Purchase()
    {
        if (_curStoreParam == null)
        {
            return;
        }

        int itemCount = StoreBuyPopup.GetItemCount();
        if (_curStoreParam.PurchaseIndex != (int)eGOODSTYPE.NONE && _curStoreParam.PurchaseIndex != (int)eGOODSTYPE.GOODS)
        {
            if (!GameSupport.IsCheckGoods((eGOODSTYPE)_curStoreParam.PurchaseIndex, _curStoreParam.PurchaseValue * itemCount))
            {
                return;
            }
        }

        GameInfo.Instance.Send_ReqStorePurchase(_curStoreParam.ID, false, itemCount, OnNet_Purchase);
    }

    private void OnNet_Purchase(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        if (_curStoreParam != null)
        {
            int ProductValue = _curStoreParam.ProductValue * StoreBuyPopup.GetItemCount();
            string productName;

            if (_curStoreParam.ProductType == (int)eREWARDTYPE.PACKAGE)
            {
                productName = FLocalizeString.Instance.GetText(1307);
            }
            else
            {
                RewardData reward = new RewardData(0, _curStoreParam.ProductType, _curStoreParam.ProductIndex, ProductValue, false);
                productName = GameSupport.GetProductName(reward);
            }

            if (_curStoreParam.ProductType == (int)eREWARDTYPE.CHAR)
            {
                SoundManager.Instance.StopVoice();
                DirectorUIManager.Instance.PlayCharBuy(_curStoreParam.ProductIndex, null);
            }
            else
            {
                string str = FLocalizeString.Instance.GetText(3049, productName);
                MessageToastPopup.Show(str, true);
            }
        }

        LobbyUIManager.Instance.Renewal("TopPanel");
        Renewal();
    }

    public bool TopBackBtnClick()
    {
        bool isSubBack = false;
        if (_storePanelType == eStorePanelType.CostumeSub)
        {
            _storePanelType = eStorePanelType.CostumeMain;
            isSubBack = true;
        }
        else if (_storePanelType != eStorePanelType.Main)
        {
            ResetValue();
            isSubBack = true;
        }

        if (isSubBack)
        {
            SoundManager.Instance.PlayUISnd(41);

            Renewal();

            LobbyUIManager.Instance.Renewal("TopPanel");

            return false;
        }

        return true;
    }

    public void ResetValue()
    {
        SoundManager.Instance.StopVoice();

        _storePanelType = eStorePanelType.Main;
        _storeTabType = eStoreTabType._START_;
        CostumeSlotIndex = 0;
    }

    public void ResetData()
    {
        ResetValue();

        if (_characterParamList.Count <= 0)
        {
            List<GameTable.Character.Param> haveCharList = new List<GameTable.Character.Param>();
            List<GameClientTable.StoreDisplayGoods.Param> storeDisplayGoodsList =
                GameInfo.Instance.GameClientTable.FindAllStoreDisplayGoods(x => x.PanelType == (int)eSD_PanelType.STORE && x.CharacterID > 0);
            foreach (GameClientTable.StoreDisplayGoods.Param storeDisplayGoods in storeDisplayGoodsList)
            {
                if (GameInfo.Instance.GetCharDataByTableID(storeDisplayGoods.CharacterID) != null)
                {
                    haveCharList.Add(GameInfo.Instance.GameTable.FindCharacter(storeDisplayGoods.CharacterID));
                    continue;
                }

                _characterParamList.Add(GameInfo.Instance.GameTable.FindCharacter(storeDisplayGoods.CharacterID));
            }

            _characterParamList.AddRange(haveCharList);
        }

        _costumeMainParamList.Clear();
        List<GameClientTable.StoreDisplayGoods.Param> mainStoreDisplayGoodsList = GameInfo.Instance.GameClientTable.FindAllStoreDisplayGoods(x => x.PanelType == (int)eSD_PanelType.STORE && x.Category == (int)eStoreTabType.STORE_COSTUME);
        foreach (GameClientTable.StoreDisplayGoods.Param mainStoreDisplayGoods in mainStoreDisplayGoodsList)
        {
            List<GameClientTable.StoreDisplayGoods.Param> subStoreDisplayGoodsList =
                        GameInfo.Instance.GameClientTable.FindAllStoreDisplayGoods(x =>
                        x.PanelType == (int)eSD_PanelType.STORE &&
                        x.Category == (int)UIStorePanel.eStoreTabType.STORE_COSTUME_SUB &&
                        x.SubCategory == mainStoreDisplayGoods.SubCategory);

            bool isShow = false;
            foreach (GameClientTable.StoreDisplayGoods.Param subStoreDisplayGoods in subStoreDisplayGoodsList)
            {
                GameTable.Store.Param storeParam = GameInfo.Instance.GameTable.FindStore(subStoreDisplayGoods.StoreID);
                if (storeParam == null)
                {
                    continue;
                }

                GameTable.Costume.Param costumeParam = GameInfo.Instance.GameTable.FindCostume(storeParam.ProductIndex);
                if (LobbyUIManager.Instance.IsShowCostumeSlot(costumeParam))
                {
                    isShow = true;
                    break;
                }
            }

            if (!isShow)
            {
                continue;
            }

            _costumeMainParamList.Add(mainStoreDisplayGoods);
        }

        _costumeSaleList.Clear();
		foreach ( GameClientTable.StoreDisplayGoods.Param costume in _costumeMainParamList ) {
			List<GameClientTable.StoreDisplayGoods.Param> costumeSubList =
				GameInfo.Instance.GameClientTable.FindAllStoreDisplayGoods( x =>
					x.PanelType == (int)eSD_PanelType.STORE && x.Category == (int)eStoreTabType.STORE_COSTUME_SUB && x.SubCategory == costume.SubCategory );
			foreach ( GameClientTable.StoreDisplayGoods.Param costumeSub in costumeSubList ) {
				if ( !GameInfo.Instance.ServerData.StoreSaleList.Any( x => x.TableID == costumeSub.StoreID ) ) {
					continue;
				}

				if ( _costumeSaleList.Any( x => x != costume.ID ) ) {
					_costumeSaleList.Add( costume.ID );
				}

				_costumeSaleList.Add( costumeSub.ID );
			}
		}
	}

    public void CharacterSelectSlot(int index)
    {
        CharacterSlotIndex = index;

        SetCharTableData();

        VoiceMgr.Instance.PlayChar(eVOICECHAR.FirstGreetings, _curCharParam.ID, 1);

        Renewal();
    }

    public void ClickSlot(int index, GameClientTable.StoreDisplayGoods.Param storeDisplayGoods)
    {
        switch((eStoreTabType)storeDisplayGoods.Category)
        {
            case eStoreTabType.STORE_COSTUME:
                {
                    if (_storePanelType == eStorePanelType.CostumeSub)
                    {
                        _storePanelType = eStorePanelType.CostumeMain;
                        Renewal();
                    }
                    else
                    {
                        CostumeSlotIndex = index;

                        _storePanelType = eStorePanelType.CostumeSub;

                        _costumeSubParamList.Clear();
                        _costumeSubParamList.Add(storeDisplayGoods);

                        DateTime curTime = GameSupport.GetCurrentServerTime();
                        List<GameClientTable.StoreDisplayGoods.Param> storeDisplayGoodsList = GameInfo.Instance.GameClientTable.FindAllStoreDisplayGoods(x =>
                                x.PanelType == (int)eSD_PanelType.STORE && x.Category == (int)eStoreTabType.STORE_COSTUME_SUB && x.SubCategory == storeDisplayGoods.SubCategory);
                        foreach (GameClientTable.StoreDisplayGoods.Param storeDisplayGoodsParam in storeDisplayGoodsList)
                        {
                            if (!string.IsNullOrEmpty(storeDisplayGoodsParam.HideConType))
                            {
                                DateTime endTime = GameSupport.GetTimeWithString(storeDisplayGoods.HideConType, true);
                                if (endTime < curTime)
                                {
                                    continue;
                                }
                            }

                            _costumeSubParamList.Add(storeDisplayGoodsParam);
                        }

                        costumeList.Reset();
                        costumeList.ScrollViewCheck();
                    }
                } break;
            case eStoreTabType.STORE_COSTUME_SUB:
                {
                    _curStoreParam = GameInfo.Instance.GameTable.FindStore(x => x.ID == storeDisplayGoods.StoreID);

                    OnClick_CostumeBuyBtn();
                } break;
            case eStoreTabType.STORE_FRIENDPOINT:
                {
                    _curStoreParam = GameInfo.Instance.GameTable.FindStore(x => x.ID == storeDisplayGoods.StoreID);

                    StoreBuyPopup.Show(storeDisplayGoods, ePOPUPGOODSTYPE.FRIENDPOINT_NONE_BTN, OnMsg_Purchase, null);
                } break;
            default:
                {
                    _curStoreParam = GameInfo.Instance.GameTable.FindStore(x => x.ID == storeDisplayGoods.StoreID);

                    StoreBuyPopup.Show(storeDisplayGoods, ePOPUPGOODSTYPE.MPOINT_NONE_BTN, OnMsg_Purchase, null);
                } break;
        }
    }

    public void DirectShow(eStoreTabType tabType, int characterIndex = -1)
    {
        ResetData();

        switch (tabType)
        {
            case eStoreTabType._START_:
                {
                    _storePanelType = eStorePanelType.Main;
                } break;
            case eStoreTabType.STORE_CHAR:
                {
                    _storePanelType = eStorePanelType.Character;
                    if (0 <= characterIndex)
                    {
                        CharacterSlotIndex = _characterParamList.FindIndex(x => x.ID == characterIndex);
                        SetCharTableData();
                    }
                } break;
            case eStoreTabType.STORE_COSTUME:
                {
                    _storePanelType = eStorePanelType.CostumeMain;
                } break;
            case eStoreTabType.STORE_POINT:
            case eStoreTabType.STORE_ETC:
            case eStoreTabType.STORE_FRIENDPOINT:
                {
                    _storePanelType = eStorePanelType.Etc;
                    _storeTabType = tabType;
                } break;
            case eStoreTabType.STORE_EVENT:
                {
                    _storePanelType = eStorePanelType.Etc_Event;
                    _storeTabType = tabType;
                } break;
        }
    }
}
