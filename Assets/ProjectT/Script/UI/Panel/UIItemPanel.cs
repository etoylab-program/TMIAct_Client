using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnVoidCallBack();

public partial class UIItemPanel : FComponent
{
    public enum eTabType
    {
        TabType_None = -1,
        TabType_Weapon,
        TabType_Gem,
        TabType_Card,
        TabType_ItemMat,
        TabType_Badge,
    }
    public enum eMenuType
    {
        MenuType_None = -1,
        MenuType_Info,
        MenuType_Sell,
        MenuType_Lock,
        MenuType_Decomposition,
    }

    public enum eSortType
    {
        None,
        Grade,
        Level,
        Wake,
        SkillLevel,
        //        SubType,
        //        ID,
        Get,        
        Enchant,
        MainStat,
        SubStat,
        Max
    }

    public enum eSortOrder
    {
        SortOrder_Up,
        SortOrder_Down,
    }

    public GameObject m_rootObject = null;
    public FTab kItemTab;
    public GameObject kSellPopup;
    public GameObject kLockPopup;   
    public GameObject kSellPopupEmpty;
    public GameObject kSellPopupCount;
    public GameObject kSellPopupSelete;
    public GameObject kSellPopupCountSlide;
    public UISlider kSellSlider;
    public UILabel kSellSlideCountLabel;
    public GameObject kLockPopupEmpty;
    public GameObject kLockPopupSelete;
    public GameObject kHaveInfo;
    public UILabel kSortLabel;
    public UILabel kOrderLabel;
    public UISprite kOrderSpr;
    public UILabel kHaveLabel;
    public UILabel kHaveLimitedLabel;
    public UILabel kCountLabel;
    public UILabel kSelCountLabel;
    public UILabel kLockCountLabel;
    public UIGoodsUnit kGoldGoodsUnit;
    public UIGoodsUnit kSpGoodsUnit;

    public UIButton kBtnLock = null;
    public UIButton kBtnDecomposition = null;
    public UIButton kFilterBtn = null;
    public GameObject kAutoMatBtn = null;
    [SerializeField]
    private FList _listItemInstance = null;

    private List<WeaponData> _WeaponList = new List<WeaponData>();
    private List<GemData> _GemList = new List<GemData>();
    private List<CardData> _CardList = new List<CardData>();
    private List<ItemData> _ItemList = new List<ItemData>();
    private List<BadgeData> _BadgeList = new List<BadgeData>();

    private int _menutype = 0;
    private int _tabindex;
    private int _sellcount;
    private bool _bselpopup = false;
    private bool _blockpopup = false;
    private bool _bdecompopopup = false;
    private bool[] _btabopen = new bool[5];
    private int _itemcount = 0;

    private eSortType _sortTypeWeapon = eSortType.Grade;
    private eSortType _sortTypeGem = eSortType.Grade;
    private eSortType _sortTypeCard = eSortType.Grade;
    private eSortType _sortTypeBadge = eSortType.Level;    
    private eSortOrder _sortOrder = eSortOrder.SortOrder_Down;
    

    private List<long> _sellitemlist = new List<long>();
    public List<long> SelItemList { get { return _sellitemlist; } }
    public int SellCount { get { return _sellcount; } }
    public bool IsSellPopup { get { return _bselpopup; } }
    public bool IsLockPopup { get { return _blockpopup; } }
    public bool IsDecompopopup { get { return _bdecompopopup; } }
    public eTabType TabType { get { return (eTabType)kItemTab.kSelectTab; } }

    public List<WeaponData> WeaponList { get { return _WeaponList; } }
    public List<GemData> GemList { get { return _GemList; } }
    public List<CardData> CardList { get { return _CardList; } }
    public List<ItemData> ItemList { get { return _ItemList; } }
    public List<BadgeData> BadgeList { get { return _BadgeList; } }

    public GameObject kNoneListObj;
    public UILabel kNoneLabel;

    // January Renewal
    [SerializeField] private FTab kSellGradeFTab;

    [Header("Decomposition")]
    [SerializeField] private GameObject kDecompoPopup;
    [SerializeField] private GameObject kDecompoPopupEmpty;
    [SerializeField] private GameObject kDecompoPopupSelete;
    [SerializeField] private FList kDecompoList = null;
    [SerializeField] private UILabel kDecompoCountLabel;
    [SerializeField] private FTab kDecompoGradeFTab;

    public FToggle kEquipFilterToggle;
    private bool _equipFilter = true;

    private eFilterFlag _rareFilter = eFilterFlag.ALL;
    private eLvFilter _lvFilter = eLvFilter.ALL;
    private eSkillLvFilter _skillLvFilter = eSkillLvFilter.ALL;
    private eWakeFilter _wakeFilter = eWakeFilter.ALL;
    private eWakeFilter _gemWakeFilter = eWakeFilter.ALL;
    private eEnchantFilter _enchantFilter = eEnchantFilter.ALL;
    private eFilterFlag _weaponCharFilter = eFilterFlag.ALL;
    private eFilterFlag _cardTypeFilter = eFilterFlag.ALL;
    private eFilterFlag _gemTypeFilter = eFilterFlag.ALL;

    private eBadgeLvFilter _badgeLvFilter = eBadgeLvFilter.ALL;
    private eBadgeLvFilter _badgeLevelUpFilter = eBadgeLvFilter.ALL;
    private int _badgeMainOptID = -1;
    private int _badgeSubOptId = -1;
    
    private int _prevTab = -1;

    private bool[] _isGradeTabEnableArray = new bool[(int)eGRADE.GRADE_UR];

    public eSortType SortType
    {
        get
        {
            if (TabType == eTabType.TabType_Weapon)
                return _sortTypeWeapon;
            else if (TabType == eTabType.TabType_Gem)
                return _sortTypeGem;
            else if (TabType == eTabType.TabType_Card)
                return _sortTypeCard;
            else if (TabType == eTabType.TabType_Badge)
                return _sortTypeBadge;
            return _sortTypeWeapon;
        }
    }

    public override void Awake()
    {
        base.Awake();

        kItemTab.EventCallBack = OnTabItemSelect;

        if (null == this._listItemInstance) return;

        this._listItemInstance.EventUpdate = this._UpdateItemListSlot;
        this._listItemInstance.EventGetItemCount = this._GetItemElementCount;
        // InitComponent에서 처음 GetCheckSlotCount 호출 시 _listItemInstance.ColumnCount(한 줄의 슬롯 개수) 값을 얻기 위해 Awake에서 UpdateList() 실행
        this._listItemInstance.UpdateList();


        kDecompoList.EventUpdate = _UpdateDecompListSlot;
        kDecompoList.EventGetItemCount = () => { return _decompoRewards.Count; };
        kDecompoList.UpdateList();

        kSellSlider.onChange.Add(new EventDelegate(OnChange_SellSlide));

        kEquipFilterToggle.EventCallBack = EquipFilterToggleCallBack;
        kEquipFilterToggle.SetToggle((int)eCOUNT.NONE, SelectEvent.Code);

        kSellGradeFTab.EventCallBack = OnEventTabSellSelect;
        kSellGradeFTab.EventCallBackActive = OnEventTabSellSelectActive;

        kDecompoGradeFTab.EventCallBack = OnEventTabDecompoSelect;
        kDecompoGradeFTab.EventCallBackActive = OnEventTabDecompoSelectActive;
    }

    private void SelectItemGradeSell(int index)
    {
        switch (TabType)
        {
            case eTabType.TabType_Weapon:
                {
                    foreach (WeaponData weaponData in _WeaponList)
                    {
                        if (_sellitemlist.Count >= GameInfo.Instance.GameConfig.SellCount)
                        {
                            break;
                        }

                        if (weaponData == null)
                        {
                            continue;
                        }

                        if (index != weaponData.TableData.Grade - 1)
                        {
                            continue;
                        }

                        if (weaponData.Lock)
                        {
                            continue;
                        }

                        if (GameInfo.Instance.GetEquipWeaponFacilityData(weaponData.WeaponUID) != null)
                        {
                            continue;
                        }

                        if (GameSupport.GetEquipWeaponDepot(weaponData.WeaponUID))
                        {
                            continue;
                        }

                        if (GameInfo.Instance.GetEquipWeaponCharData(weaponData.WeaponUID) != null)
                        {
                            continue;
                        }

                        if (_sellitemlist.Exists(r => r == weaponData.WeaponUID) == true) {
                            continue;
                        }

                        _sellitemlist.Add(weaponData.WeaponUID);
                    }
                }
                break;
            case eTabType.TabType_Card:
                {
                    foreach (CardData cardData in _CardList)
                    {
                        if (_sellitemlist.Count >= GameInfo.Instance.GameConfig.SellCount)
                        {
                            break;
                        }

                        if (cardData == null)
                        {
                            continue;
                        }

                        if (index != cardData.TableData.Grade - 1)
                        {
                            continue;
                        }

                        if (cardData.Lock)
                        {
                            continue;
                        }

                        if (GameSupport.IsEquipAndUsingCardData(cardData.CardUID))
                        {
                            continue;
                        }

                        if (GameInfo.Instance.GetEquiCardCharData(cardData.CardUID) != null)
                        {
                            continue;
                        }

                        if (GameInfo.Instance.GetEquipCharFacilityData(cardData.CardUID) != null)
                        {
                            continue;
                        }

                        if (_sellitemlist.Exists(r => r == cardData.CardUID) == true) {
                            continue;
                        }

                        _sellitemlist.Add(cardData.CardUID);
                    }
                }
                break;
            case eTabType.TabType_Gem:
                {
                    foreach (GemData gemData in _GemList)
                    {
                        if (_sellitemlist.Count >= GameInfo.Instance.GameConfig.SellCount)
                        {
                            break;
                        }

                        if (gemData == null)
                        {
                            continue;
                        }

                        if (index != gemData.TableData.Grade - 1)
                        {
                            continue;
                        }

                        if (gemData.Lock)
                        {
                            continue;
                        }

                        if (GameInfo.Instance.GetEquipGemWeaponData(gemData.GemUID) != null)
                        {
                            continue;
                        }

                        if (_sellitemlist.Exists(r => r == gemData.GemUID) == true) {
                            continue;
                        }

                        _sellitemlist.Add(gemData.GemUID);
                    }
                }
                break;
        }
    }

    private void DeselectItemGradeSell(int index)
    {
        List<long> removeList = new List<long>();
        foreach(long uid in _sellitemlist)
        {
            switch (TabType)
            {
                case eTabType.TabType_Weapon:
                    {
                        WeaponData weaponData = GameInfo.Instance.GetWeaponData(uid);
                        if (weaponData == null)
                        {
                            continue;
                        }

                        if (index != weaponData.TableData.Grade - 1)
                        {
                            continue;
                        }

                        removeList.Add(weaponData.WeaponUID);
                    }
                    break;
                case eTabType.TabType_Card:
                    {
                        CardData cardData = GameInfo.Instance.GetCardData(uid);
                        if (cardData == null)
                        {
                            continue;
                        }

                        if (index != cardData.TableData.Grade - 1)
                        {
                            continue;
                        }

                        removeList.Add(cardData.CardUID);
                    }
                    break;
                case eTabType.TabType_Gem:
                    {
                        GemData gemData = GameInfo.Instance.GetGemData(uid);
                        if (gemData == null)
                        {
                            continue;
                        }

                        if (index != gemData.TableData.Grade - 1)
                        {
                            continue;
                        }

                        removeList.Add(gemData.GemUID);
                    }
                    break;
            }
        }

        foreach (long uid in removeList)
        {
            _sellitemlist.Remove(uid);
        }

        removeList.Clear();
    }

    private bool OnEventTabSellSelect(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Click)
        {
            if (kSellGradeFTab.kBtnList.Count <= nSelect)
            {
                return false;
            }

            bool isActive = false;
            Transform selSpr = kSellGradeFTab.kBtnList[nSelect].transform.Find("SelSpr");
            if (selSpr != null)
            {
                isActive = selSpr.gameObject.activeSelf;
            }

            if (isActive == false && GameInfo.Instance.GameConfig.SellCount <= _sellitemlist.Count)
            {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3056));

                return false;
            }
        }

        return true;
    }

	private bool OnEventTabSellSelectActive( int nSelect, bool isActive ) {
		if ( isActive ) {
			SelectItemGradeSell( nSelect );
		}
		else {
			DeselectItemGradeSell( nSelect );
		}

		kSellPopupEmpty.SetActive( _sellitemlist.Count <= 0 );
		kSellPopupSelete.SetActive( 0 < _sellitemlist.Count );
		kGoldGoodsUnit.gameObject.SetActive( 0 < _sellitemlist.Count );

		RenewalSellPopup();
		_listItemInstance.RefreshNotMoveAllItem();

		return true;
	}

	private bool EquipFilterToggleCallBack(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Code)
            return true;

        if (type != SelectEvent.Click)
        {
            return true;
        }

        _equipFilter = (nSelect == (int)eCOUNT.NONE);
        InitComponent();
        Log.Show("Equip Filter : " + _equipFilter);

        return true;
    }

    public override void OnEnable()
    {
        _prevTab = -1;
        _bselpopup = false;
        _blockpopup = false;
        _bdecompopopup = false;
       
        kSellPopup.SetActive(false);
        kLockPopup.SetActive(false);
        kDecompoPopup?.SetActive(false);

        for (int i = 0; i < _btabopen.Length; i++)
            _btabopen[i] = false;

        object v = UIValue.Instance.GetValue(UIValue.EParamType.ItemTab, true);
        eTabType tabType = eTabType.TabType_Weapon;
        if (v != null)
        {
            tabType = (eTabType)(int)v;
        }

        kItemTab.SetTab((int)tabType, SelectEvent.Code);

        HideItemMenu((int)eMenuType.MenuType_Info);

        base.OnEnable();
    }

    public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		if (_blockpopup)
            RollbackLockInfo();
        _sellitemlist.Clear();
        _bselpopup = false;
        _blockpopup = false;
        _bdecompopopup = false;

        if ( _btabopen[0] )
            GameInfo.Instance.DeleteNewWeapon();
        if (_btabopen[1])
            GameInfo.Instance.DeleteNewGem();
        if (_btabopen[2])
            GameInfo.Instance.DeleteNewCard();
        if (_btabopen[3])
            GameInfo.Instance.DeleteNewItem();
        if (_btabopen[4])
            GameInfo.Instance.DeleteNewBadge();

        NotificationManager.Instance.CheckNotification(NotificationManager.eTYPE.ITEM);
        
        base.OnDisable();
    }
    
    public override void InitComponent()
    {
        _itemcount = GameSupport.GetInvenCount();
        _WeaponList.Clear();
        _GemList.Clear();
        _CardList.Clear();
        _ItemList.Clear();
        _BadgeList.Clear();

        int lineMaxCount = GetCheckSlotCount(); // 한 줄의 슬롯 개수
        int ViewMaxCount = lineMaxCount * GetCheckLineCount(); // 보여지는 갯수(한줄 * 줄수)

        SetNoneLabel(false, 0);

        if (kItemTab.kSelectTab == (int)eTabType.TabType_Weapon)
        {
            SetFilterWeapon(_rareFilter, _lvFilter, _skillLvFilter, _wakeFilter, _weaponCharFilter, _enchantFilter, false);
        }
        else if (kItemTab.kSelectTab == (int)eTabType.TabType_Gem)
        {
            SetFilterGem(_rareFilter, _lvFilter, _gemWakeFilter, _gemTypeFilter, false);
        }
        else if (kItemTab.kSelectTab == (int)eTabType.TabType_Card)
        {
            SetFilterCard(_rareFilter, _lvFilter, _skillLvFilter, _wakeFilter, _cardTypeFilter, _enchantFilter, false);
        }
        else if (kItemTab.kSelectTab == (int)eTabType.TabType_Badge)
        {
            SetFilterBadge(_badgeLvFilter, _badgeLevelUpFilter, _badgeMainOptID, _badgeSubOptId, false);
        }
        else
        {
            if (kItemTab.kSelectTab == (int)eTabType.TabType_ItemMat)
            {
                for (int i = 0; i < GameInfo.Instance.ItemList.Count; i++)
                {
                    if (GameInfo.Instance.ItemList[i].TableData.Type != (int)eITEMTYPE.EVENT)
                        _ItemList.Add(GameInfo.Instance.ItemList[i]);
                }
            }
            if(_ItemList.Count <= 0)
                SetNoneLabel(true, 1593);

            SetSort_Item(false);

            int count = 0;
            if (_ItemList.Count < ViewMaxCount)
                count = ViewMaxCount - _ItemList.Count;
            else
                count = ( _ItemList.Count % lineMaxCount == 0 ) ? 0 : lineMaxCount - ( _ItemList.Count % lineMaxCount );
            for (int i = 0; i < count; i++)
                _ItemList.Add(null);
        }
        
        if (_prevTab == kItemTab.kSelectTab)
        {
            _listItemInstance.RefreshNotMoveAllItem();
        }
        else
        {
            _listItemInstance.UpdateList();
            _listItemInstance.ScrollPositionSet();
        }
        _prevTab = kItemTab.kSelectTab;
    }

    private readonly string[] m_sortIcon = { "ico_Filter1", "ico_Filter2" };
    
    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        kOrderLabel.textlocalize = FLocalizeString.Instance.GetText(1075);
        kOrderSpr.transform.localScale = Vector3.one;

        SetActiveOrderBtn(( kItemTab.kSelectTab != (int)eTabType.TabType_ItemMat ) == true);
        SetActiveSortBtn(( kItemTab.kSelectTab != (int)eTabType.TabType_ItemMat ) == true);
        SetActiveLockBtn(( kItemTab.kSelectTab != (int)eTabType.TabType_ItemMat ) == true);
        kEquipFilterToggle.gameObject.SetActive(kItemTab.kSelectTab != (int)eTabType.TabType_ItemMat);
        SetActiveDecompositionBtn();

        //  정렬 순서에 따른 이미지 변경
        kOrderSpr.spriteName = _sortOrder == eSortOrder.SortOrder_Up ? m_sortIcon[0] : m_sortIcon[1];

        int now = _itemcount;
        int max = GameInfo.Instance.UserData.ItemSlotCnt;

        switch ((eTabType)kItemTab.kSelectTab)
        {
            case eTabType.TabType_Weapon:
                {
                    SetTextSortBtn(_sortTypeWeapon);
                }
                break;
            case eTabType.TabType_Gem:
                {
                    SetTextSortBtn(_sortTypeGem);
                }
                break;
            case eTabType.TabType_Card:
                {
                    SetTextSortBtn(_sortTypeCard);
                }
                break;
            case eTabType.TabType_ItemMat:
                break;
            case eTabType.TabType_Badge:
                {
                    SetTextSortBtn(_sortTypeBadge);
                }
                break;
            case eTabType.TabType_None:
            default:
                {
                    Debug.Log("Not Selected TabType");
                }
                break;
        }

        if (now >= 0)
        {
            kHaveInfo.gameObject.SetActive(true);
            kHaveLabel.textlocalize = string.Format("{0:#,##0}", now);
            kHaveLimitedLabel.textlocalize = string.Format("/{0:#,##0}", max);
        }
        else
        {
            //kHaveInfo.gameObject.SetActive(false);
        }
    }

    public bool OnTabItemSelect(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Enable)
        {
            return false;
        }
        
        if (_bselpopup)
            HideSellPopup();
        if (_blockpopup)
            HideLockPopup();
        if (_bdecompopopup)
            HideDecompoPopup();

        Log.Show("OnTabItemSelect", Log.ColorType.Red);

        if (0 <= nSelect && _btabopen.Length > nSelect)
            _btabopen[nSelect] = true;

        _tabindex = nSelect;

        //kEquipFilterToggle.SetToggle((int)eCOUNT.NONE, SelectEvent.Code);

        DefaultFilter();
        InitComponent();
        Renewal(true);
        
        HideItemMenu((int)eMenuType.MenuType_Info);
        
        return true;
    }
    
    private void _UpdateItemListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIItemListSlot slot = slotObject.GetComponent<UIItemListSlot>();
            if (null == slot) break;
            slot.ParentGO = this.gameObject;

            if (kItemTab.kSelectTab == (int)eTabType.TabType_Weapon)
            {
                WeaponData data = null;
                if (0 <= index && _WeaponList.Count > index)
                    data = _WeaponList[index];
                slot.UpdateSlot(UIItemListSlot.ePosType.Item_Inven, index, data);
            }
            else if (kItemTab.kSelectTab == (int)eTabType.TabType_Gem)
            {
                GemData data = null;
                if (0 <= index && _GemList.Count > index)
                    data = _GemList[index];
                slot.UpdateSlot(UIItemListSlot.ePosType.Item_Inven, index, data);
            }
            else if (kItemTab.kSelectTab == (int)eTabType.TabType_Card)
            {
                CardData data = null;
                if (0 <= index && _CardList.Count > index)
                    data = _CardList[index];
                slot.UpdateSlot(UIItemListSlot.ePosType.Item_Inven, index, data);
            }
            else if(kItemTab.kSelectTab == (int)eTabType.TabType_Badge)
            {
                BadgeData data = null;
                if (0 <= index && _BadgeList.Count > index)
                    data = _BadgeList[index];
                slot.UpdateSlot(UIItemListSlot.ePosType.Item_Inven, index, data);
            }
            else
            {
                ItemData data = null;
                if (0 <= index && _ItemList.Count > index)
                    data = _ItemList[index];
                slot.UpdateSlot(UIItemListSlot.ePosType.Item_Inven, index, data);
            }


        } while (false);
    }

    private int _GetItemElementCount()
    {
        if (kItemTab.kSelectTab == (int)UIItemPanel.eTabType.TabType_Weapon)
            return _WeaponList.Count;
        else if (kItemTab.kSelectTab == (int)UIItemPanel.eTabType.TabType_Gem)
            return _GemList.Count;
        else if (kItemTab.kSelectTab == (int)UIItemPanel.eTabType.TabType_Card)
            return _CardList.Count;
        else if (kItemTab.kSelectTab == (int)UIItemPanel.eTabType.TabType_Badge)
            return _BadgeList.Count;
        else
            return _ItemList.Count;
    }

    public void OnClick_SortBtn()
    {
        //  추후 필터로 변경됩니다.
        //  내부 기능만 구현한 상태
        {
            eSortType sortType = eSortType.Grade;

            if (kItemTab.kSelectTab == (int)eTabType.TabType_Weapon)
            {
                _sortTypeWeapon++;
                if ((int)_sortTypeWeapon >= (int)eSortType.Max)
                    _sortTypeWeapon = sortType;

                SetSort_Weapon();
                sortType = _sortTypeWeapon;
            }
            else if (kItemTab.kSelectTab == (int)eTabType.TabType_Gem)
            {
                _sortTypeGem++;
                if ((int)_sortTypeGem >= (int)eSortType.Enchant)
                    _sortTypeGem = sortType;

                //  곡옥 정렬 시 스킬 레벨 패스
                if (_sortTypeGem == eSortType.SkillLevel) _sortTypeGem = eSortType.Get;

                SetSort_Gem();
                sortType = _sortTypeGem;
            }
            else if (kItemTab.kSelectTab == (int)eTabType.TabType_Card)
            {
                _sortTypeCard++;
                if ((int)_sortTypeCard >= (int)eSortType.Max)
                    _sortTypeCard = sortType;

                SetSort_Card();
                sortType = _sortTypeCard;
            }
            else if(kItemTab.kSelectTab == (int)eTabType.TabType_Badge)
            {
                //문양은 레벨순, 획득순만 필요해서 따로 구현
                sortType = eSortType.Level;
                _sortTypeBadge++;
                
                if ((int)_sortTypeBadge >= (int)eSortType.Enchant)
                    _sortTypeBadge = sortType;
                if ((int)_sortTypeBadge > (int)eSortType.Level)
                    _sortTypeBadge = eSortType.Get;

                SetSort_Badge();
                sortType = _sortTypeBadge;
            }

            SetTextSortBtn(sortType);
            
            //  정렬 결과 갱신
            RefreshList();
        }
    }

    public void OnClick_FilterBtn()
    {
        UIValue.Instance.SetValue(UIValue.EParamType.FilterOpenUI, UIItemFilterPopup.eFilterOpenUI.ItemPanel.ToString());
        //  추후 정렬 필터로 추가됩니다.
        LobbyUIManager.Instance.ShowUI("ItemFilterPopup", true);
    }

    public void OnClick_OrderBtn()
    {
        //  정렬 순서 변경
        _sortOrder = _sortOrder == eSortOrder.SortOrder_Up ? eSortOrder.SortOrder_Down : eSortOrder.SortOrder_Up;
        //  정렬 순서에 따른 이미지 변경
        kOrderSpr.spriteName = _sortOrder == eSortOrder.SortOrder_Up ? m_sortIcon[0] : m_sortIcon[1];

        //  정렬 진행
        switch (( eTabType )kItemTab.kSelectTab)
        {
            case eTabType.TabType_Weapon:
                {
                    SetSort_Weapon();
                }
                break;
            case eTabType.TabType_Gem:
                {
                    SetSort_Gem();
                }
                break;
            case eTabType.TabType_Card:
                {
                    SetSort_Card();
                }
                break;
            case eTabType.TabType_Badge:
                {
                    SetSort_Badge();
                }
                break;
            case eTabType.TabType_None:
            case eTabType.TabType_ItemMat:
            default:
                {
                    Debug.Log("Not Selected TabType");
                }
                break;
        }

        //  정렬 결과 갱신
        RefreshList();
    }

    public void OnClick_ItemMenu01Btn()
    {
        if (_blockpopup)
        {
            HideLockPopup();
        }
        if (_bdecompopopup)
            HideDecompoPopup();

        HideItemMenu((int)eMenuType.MenuType_Sell);
    }

    public void OnClick_ItemMenu02Btn()
    {
        if (_bselpopup) HideSellPopup();        
        if (_bdecompopopup) HideDecompoPopup();

        HideItemMenu((int)eMenuType.MenuType_Lock);
    }

    public void OnClick_ItemMenu03Btn()
    {
        if (_blockpopup) HideLockPopup();
        if (_bselpopup) HideSellPopup();

        HideItemMenu((int)eMenuType.MenuType_Decomposition);
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //
    //
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void ShowItemMenu(int menutype)
    {
        _menutype = menutype;
    
    }

    public void HideItemMenu(int menutype)
    {
        _menutype = menutype;
        kSellPopup.SetActive(false);
        kLockPopup.SetActive(false);
        kDecompoPopup.SetActive(false);
    
        switch ((eMenuType)_menutype)
        {
            case eMenuType.MenuType_Info:
                {
                   
                }
                break;
            case eMenuType.MenuType_Sell:
                {
                    ShowSellPopup();
                }
                break;
            case eMenuType.MenuType_Lock:
                {
                    ShowLockPopup();
                }
                break;
            case eMenuType.MenuType_Decomposition:
                ShowDecompoPopup();
                break;

            case eMenuType.MenuType_None:
            default:
                Debug.Log("_menutype : MenuType_None");
                break;
        }
    }
   
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //
    //
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void SeleteItem(long uid, OnVoidCallBack OnCallBack = null)
    {
        if (_menutype == (int)eMenuType.MenuType_Info)
        {
            HideItemMenu((int)eMenuType.MenuType_Info);
            if (kItemTab.kSelectTab == (int)eTabType.TabType_Weapon)
            {
                var weapondata = GameInfo.Instance.GetWeaponData(uid);
                if (weapondata == null)
                    return;

                UIValue.Instance.SetValue(UIValue.EParamType.WeaponUID, uid);
                UIWeaponInfoPopup p = LobbyUIManager.Instance.ShowUI("WeaponInfoPopup", true) as UIWeaponInfoPopup;
                p.OnCloseCallBack = OnCallBack;
            }
            else if (kItemTab.kSelectTab == (int)eTabType.TabType_Gem)
            {
                var gemdata = GameInfo.Instance.GetGemData(uid);
                if (gemdata == null)
                    return;

                UIValue.Instance.SetValue(UIValue.EParamType.GemUID, uid);
                LobbyUIManager.Instance.ShowUI("GemInfoPopup", true);
            }
            else if (kItemTab.kSelectTab == (int)eTabType.TabType_Card)
            {
                var carddata = GameInfo.Instance.GetCardData(uid);
                if (carddata == null)
                    return;

                UIValue.Instance.SetValue(UIValue.EParamType.CardUID, uid);
                UICardInfoPopup p = LobbyUIManager.Instance.ShowUI("CardInfoPopup", true) as UICardInfoPopup;
                p.OnCloseCallBack = OnCallBack;
            }
            else if (kItemTab.kSelectTab == (int)eTabType.TabType_Badge)
            {
                BadgeData badgedata = GameInfo.Instance.GetBadgeData(uid);
                if (badgedata == null)
                    return;

                UIValue.Instance.SetValue(UIValue.EParamType.BadgeUID, uid);
                LobbyUIManager.Instance.ShowUI("BadgeInfoPopup", true);
            }
            else
            {
                var itemdata = GameInfo.Instance.GetItemData(uid);
                if (itemdata == null)
                    return;

                if (itemdata.TableData.Type == (int)eITEMTYPE.EVENT)
                    return;

                UIValue.Instance.SetValue(UIValue.EParamType.ItemUID, uid);

                if (GameSupport.IsPackageInfoPopupItem(itemdata.TableData) == true)
                {
                    LobbyUIManager.Instance.ShowUI("ItemPackageInfoPopup", true);
                    return;
                }
                
                LobbyUIManager.Instance.ShowUI("ItemInfoPopup", true);
            }
        }
        else if (_menutype == (int)eMenuType.MenuType_Lock)
        {
            var check = _sellitemlist.Find(x => x == uid);
            if (check == 0)
            {
                if (_sellitemlist.Count < GameInfo.Instance.GameConfig.MatCount)
                    _sellitemlist.Add(uid);
                else
                    return;
            }
            else
                _sellitemlist.Remove(uid);

            if (kItemTab.kSelectTab == (int)eTabType.TabType_Weapon)
            {
                var weapondata = GameInfo.Instance.GetWeaponData(uid);
                if (weapondata == null)
                    return;
                weapondata.Lock = !weapondata.Lock;
            }
            else if (kItemTab.kSelectTab == (int)eTabType.TabType_Gem)
            {
                var gemdata = GameInfo.Instance.GetGemData(uid);
                if (gemdata == null)
                    return;
                gemdata.Lock = !gemdata.Lock;
            }
            else if (kItemTab.kSelectTab == (int)eTabType.TabType_Card)
            {
                var carddata = GameInfo.Instance.GetCardData(uid);
                if (carddata == null)
                    return;
                carddata.Lock = !carddata.Lock;
            }
            else if (kItemTab.kSelectTab == (int)eTabType.TabType_Badge)
            {
                var badgedata = GameInfo.Instance.GetBadgeData(uid);
                if (badgedata == null)
                    return;

                badgedata.Lock = !badgedata.Lock;
            }

            if (_sellitemlist.Count == 0)
            {
                kLockPopupEmpty.gameObject.SetActive(true);
                kLockPopupSelete.gameObject.SetActive(false);
            }
            else
            {
                kLockPopupEmpty.gameObject.SetActive(false);
                kLockPopupSelete.gameObject.SetActive(true);
            }

            this._listItemInstance.RefreshNotMove();
            //kLockCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(276), _sellitemlist.Count, GameInfo.Instance.GameConfig.MatCount);
            FLocalizeString.SetLabel(kLockCountLabel, 276, _sellitemlist.Count, GameInfo.Instance.GameConfig.MatCount);
        }
        else if (_menutype == (int)eMenuType.MenuType_Decomposition)
        {
            System.Action<long> ActionCommonDecompo = (ActionUID) =>
            {
                var check = _decompoitemlist.Find(x => x == ActionUID);
                if (check == 0)
                {
                    if (_decompoitemlist.Count < GameInfo.Instance.GameConfig.SellCount)
                    {
                        _decompoitemlist.Add(ActionUID);
                        UpdateDecompoRandomGroupID();
                    }

                    else
                    {
                        //  선택이 11번째 이상인 경우 메세지 팝업 출력
                        string str = FLocalizeString.Instance.GetText(3056);
                        MessageToastPopup.Show(str, true);
                    }
                }
                else
                {
                    _decompoitemlist.Remove(ActionUID);
                    UpdateDecompoRandomGroupID();
                }

                kDecompoPopupEmpty.SetActive(false);
                kDecompoPopupSelete.SetActive(true);
                ReflashDeomposition_CountLabel();

                if (_decompoitemlist.Count == 0)
                {
                    kDecompoPopupEmpty.SetActive(true);
                    kDecompoPopupSelete.SetActive(false);                    
                }               
                
                _listItemInstance.RefreshNotMove();
            };

            if (kItemTab.kSelectTab == (int)eTabType.TabType_Weapon)
            {
                var weapondata = GameInfo.Instance.GetWeaponData(uid);
                if (weapondata == null)
                    return;
                if (weapondata.Lock || weapondata.TableData.Decomposable == 0)
                    return;
                if (GameInfo.Instance.GetEquipWeaponFacilityData(weapondata.WeaponUID) != null)
                    return;
                if (GameSupport.GetEquipWeaponDepot(weapondata.WeaponUID))
                    return;

                CharData equipData = GameInfo.Instance.GetEquipWeaponCharData(weapondata.WeaponUID);
                if (equipData != null)
                    return;

                ActionCommonDecompo(uid);                
            }
            else if (kItemTab.kSelectTab == (int)eTabType.TabType_Card)
            {
                var carddata = GameInfo.Instance.GetCardData(uid);
                if (carddata == null)
                    return;

                if (carddata.Lock)
                    return;

                if (GameSupport.IsEquipAndUsingCardData(carddata.CardUID))
                    return;

                CharData equipData = GameInfo.Instance.GetEquiCardCharData(carddata.CardUID);
                if (equipData != null)
                    return;

                if (GameInfo.Instance.GetEquipCharFacilityData(carddata.CardUID) != null)
                    return;
                ActionCommonDecompo(uid);               
            }
        }
        else
        {
			if ( kItemTab.kSelectTab == (int)eTabType.TabType_Weapon ) {
				var weapondata = GameInfo.Instance.GetWeaponData( uid );
				if ( weapondata == null )
					return;
				if ( weapondata.Lock )
					return;
				if ( GameInfo.Instance.GetEquipWeaponFacilityData( weapondata.WeaponUID ) != null )
					return;
				if ( GameSupport.GetEquipWeaponDepot( weapondata.WeaponUID ) )
					return;

				CharData equipData = GameInfo.Instance.GetEquipWeaponCharData( weapondata.WeaponUID );
				if ( equipData != null )
					return;
				var check = _sellitemlist.Find( x => x == uid );
				if ( check == 0 ) {
					if ( _sellitemlist.Count < GameInfo.Instance.GameConfig.SellCount )
						_sellitemlist.Add( uid );
					else {
						//  선택이 11번째 이상인 경우 메세지 팝업 출력
						string str = FLocalizeString.Instance.GetText( 3056 );
						MessageToastPopup.Show( str, true );
					}
				}
				else
					_sellitemlist.Remove( uid );

				kSellPopupEmpty.gameObject.SetActive( false );
				kSellPopupSelete.gameObject.SetActive( true );
				kGoldGoodsUnit.gameObject.SetActive( true );
			}
			else if ( kItemTab.kSelectTab == (int)eTabType.TabType_Gem ) {
				var gemdata = GameInfo.Instance.GetGemData( uid );
				if ( gemdata == null )
					return;
				if ( gemdata.Lock )
					return;
				WeaponData equipData = GameInfo.Instance.GetEquipGemWeaponData( gemdata.GemUID );
				if ( equipData != null )
					return;
				var check = _sellitemlist.Find( x => x == uid );
				if ( check == 0 ) {
					if ( _sellitemlist.Count < GameInfo.Instance.GameConfig.SellCount )
						_sellitemlist.Add( uid );
					else {
						//  선택이 11번째 이상인 경우 메세지 팝업 출력
						string str = FLocalizeString.Instance.GetText( 3056 );
						MessageToastPopup.Show( str, true );
					}
				}
				else
					_sellitemlist.Remove( uid );

				kSellPopupEmpty.gameObject.SetActive( false );
				kSellPopupSelete.gameObject.SetActive( true );
				kGoldGoodsUnit.gameObject.SetActive( true );
			}
			else if ( kItemTab.kSelectTab == (int)eTabType.TabType_Card ) {
				var carddata = GameInfo.Instance.GetCardData( uid );
				if ( carddata == null )
					return;

				if ( carddata.Lock )
					return;

				if ( GameSupport.IsEquipAndUsingCardData( carddata.CardUID ) )
					return;

				CharData equipData = GameInfo.Instance.GetEquiCardCharData( carddata.CardUID );
				if ( equipData != null )
					return;

				if ( GameInfo.Instance.GetEquipCharFacilityData( carddata.CardUID ) != null )
					return;

				var check = _sellitemlist.Find( x => x == uid );
				if ( check == 0 ) {
					if ( _sellitemlist.Count < GameInfo.Instance.GameConfig.SellCount )
						_sellitemlist.Add( uid );
					else {
						//  선택이 11번째 이상인 경우 메세지 팝업 출력
						string str = FLocalizeString.Instance.GetText( 3056 );
						MessageToastPopup.Show( str, true );
					}
				}
				else
					_sellitemlist.Remove( uid );

				kSellPopupEmpty.gameObject.SetActive( false );
				kSellPopupSelete.gameObject.SetActive( true );
				kGoldGoodsUnit.gameObject.SetActive( true );
			}
			else if ( kItemTab.kSelectTab == (int)eTabType.TabType_Badge ) {
				var badgedata = GameInfo.Instance.GetBadgeData( uid );

				if ( badgedata == null )
					return;

				if ( badgedata.Lock )
					return;

				if ( badgedata.PosKind != (int)eContentsPosKind._NONE_ )
					return;

				var check = _sellitemlist.Find( x => x == uid );
				if ( check == 0 ) {
					if ( _sellitemlist.Count < GameInfo.Instance.GameConfig.SellCount )
						_sellitemlist.Add( uid );
					else {
						//  선택이 11번째 이상인 경우 메세지 팝업 출력
						string str = FLocalizeString.Instance.GetText( 3056 );
						MessageToastPopup.Show( str, true );
					}
				}
				else
					_sellitemlist.Remove( uid );

				kSellPopupEmpty.gameObject.SetActive( false );
				kSellPopupSelete.gameObject.SetActive( true );
				kGoldGoodsUnit.gameObject.SetActive( true );
			}
			else {
				var itemdata = GameInfo.Instance.GetItemData( uid );
				if ( itemdata == null )
					return;

				if ( !GameSupport.IsAbleSellItem( itemdata.TableData ) )
					return;

				_sellitemlist.Clear();
				_sellitemlist.Add( uid );
				_sellcount = 1;

				kSellPopupEmpty.gameObject.SetActive( false );
				//kSellPopupCount.gameObject.SetActive(true);
				kSellPopupCountSlide.gameObject.SetActive( true );
				kSellSlider.value = 0f;
				kSellPopupCount.gameObject.SetActive( false );
				kGoldGoodsUnit.gameObject.SetActive( true );
			}

			if (_sellitemlist.Count == 0)
            {
                kSellPopupEmpty.gameObject.SetActive(true);
                kSellPopupSelete.gameObject.SetActive(false);
                kGoldGoodsUnit.gameObject.SetActive(false);
                kSpGoodsUnit.gameObject.SetActive(false);
            }

            this._listItemInstance.RefreshNotMove();
            RenewalSellPopup();
        }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //
    //
    //
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void OnClick_SellMinus()
    {
        if (_sellitemlist.Count == 0)
            return;
        var itemdata = GameInfo.Instance.GetItemData(_sellitemlist[0]);
        if (itemdata == null)
            return;

        _sellcount -= 1;
        if (_sellcount <= 1)
            _sellcount = 1;
        RenewalSellPopup();
    }

    public void OnClick_SellPlus()
    {
        if (_sellitemlist.Count == 0)
            return;
        var itemdata = GameInfo.Instance.GetItemData(_sellitemlist[0]);
        if (itemdata == null)
            return;

        _sellcount += 1;
        if (_sellcount >= itemdata.Count)
            _sellcount = itemdata.Count;
        RenewalSellPopup();
    }

    public void OnClick_SellMax()
    {
        if (_sellitemlist.Count == 0)
            return;
        var itemdata = GameInfo.Instance.GetItemData(_sellitemlist[0]);
        if (itemdata == null)
            return;

        _sellcount = itemdata.Count;
        RenewalSellPopup();
    }

    public void OnChange_SellSlide()
    {
        if (_sellitemlist.Count == 0)
            return;

        var itemdata = GameInfo.Instance.GetItemData(_sellitemlist[0]);
        if (itemdata == null)
            return;

        _sellcount = (int)(kSellSlider.value * itemdata.Count);
        if(_sellcount <= 1)
            _sellcount = 1;

        Log.Show(kSellSlider.value);
        RenewalSellPopup();
    }

    public void OnClick_SellOK()
    {
        if (_sellitemlist.Count == 0)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3005));
            return;
        }

        if (kSellPopupCountSlide.gameObject.activeSelf && _sellcount <= (int)eCOUNT.NONE)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(200007));
            return;
        }

        LobbyUIManager.Instance.ShowUI("SellPopup", true);
    }

    public void OnClick_SellCancel()
    {
        HideSellPopup();
        this._listItemInstance.RefreshNotMove();
    }

    public void OnClick_LockOk()
    {
        if (_sellitemlist.Count == 0)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3005));
            return;
        }

        List<bool> locklist = new List<bool>();

        switch ((eTabType)kItemTab.kSelectTab)
        {
            case eTabType.TabType_Weapon:
                {
                    for (int i = 0; i < _sellitemlist.Count; i++)
                    {
                        var weapondata = GameInfo.Instance.GetWeaponData(_sellitemlist[i]);
                        if (weapondata == null)
                            return;
                        locklist.Add(weapondata.Lock);
                    }

                    GameInfo.Instance.Send_ReqSetLockWeaponList(_sellitemlist, locklist, OnNetWeaponLock);
                }
                break;
            case eTabType.TabType_Gem:
                {
                    for (int i = 0; i < _sellitemlist.Count; i++)
                    {
                        var gemdata = GameInfo.Instance.GetGemData(_sellitemlist[i]);
                        if (gemdata == null)
                            return;
                        locklist.Add(gemdata.Lock);
                    }

                    GameInfo.Instance.Send_ReqSetLockGemList(_sellitemlist, locklist, OnNetGemLock);
                }
                break;
            case eTabType.TabType_Card:
                {
                    for (int i = 0; i < _sellitemlist.Count; i++)
                    {
                        var carddata = GameInfo.Instance.GetCardData(_sellitemlist[i]);
                        if (carddata == null)
                            return;
                        locklist.Add(carddata.Lock);
                    }

                    GameInfo.Instance.Send_ReqSetLockCardList(_sellitemlist, locklist, OnNetCardLock);
                }
                break;
            case eTabType.TabType_Badge:
                {
                    for(int i = 0; i < _sellitemlist.Count; i++)
                    {
                        var badgedata = GameInfo.Instance.GetBadgeData(_sellitemlist[i]);
                        if (badgedata == null)
                            return;
                        locklist.Add(badgedata.Lock);
                    }

                    GameInfo.Instance.Send_ReqSetLockBadge(_sellitemlist, locklist, OnNetBadgeLock);
                }
                break;
            case eTabType.TabType_None:
            case eTabType.TabType_ItemMat:
            default:
                break;
        }
    }

    public void OnClick_LockCancel()
    {
        HideLockPopup();
    }

    public void OnClick_InvenBuyBtn()
    {
        GameSupport.OnMsg_InvenExpansion();
    }

	public void ReflashSellPopup( bool isLastPosCheck = true ) {
		_bselpopup = true;
		_sellitemlist.Clear();

		kSellPopup.SetActive( true );
		kSellPopupEmpty.SetActive( true );

		kSellGradeFTab.gameObject.SetActive( kItemTab.kSelectTab == (int)eTabType.TabType_Weapon || kItemTab.kSelectTab == (int)eTabType.TabType_Card || kItemTab.kSelectTab == (int)eTabType.TabType_Gem );
		kSellGradeFTab.DisableTab();

		kSellPopupCountSlide.SetActive( false );
		kSellPopupCount.SetActive( false );
		kSellPopupSelete.SetActive( false );
		kGoldGoodsUnit.gameObject.SetActive( false );
		kSpGoodsUnit.gameObject.SetActive( false );
		kAutoMatBtn.SetActive( TabType != eTabType.TabType_ItemMat );
		RenewalSellPopup();

		//장착 또는 잠금 상태 아이템 비활성 처리
		this._listItemInstance.RefreshNotMove();

		if ( isLastPosCheck && _listItemInstance.IsLastPosY() ) {
			_listItemInstance.SpringSetFocus( _listItemInstance.EventGetItemCount() - 1, ratio: 1 );
		}
	}

	private void RenewalSellPopup()
    {
		kSpGoodsUnit.gameObject.SetActive( ( TabType == eTabType.TabType_Weapon || TabType == eTabType.TabType_Card ) && 0 < _sellitemlist.Count );

        int gold = 0;

        if (kSellPopupSelete.gameObject.activeSelf == true)
        {
            FLocalizeString.SetLabel(kSelCountLabel, 276, _sellitemlist.Count, GameInfo.Instance.GameConfig.SellCount);

            if (kItemTab.kSelectTab == (int)eTabType.TabType_Weapon)
            {
                int sp = 0;
                for (int i = 0; i < _sellitemlist.Count; i++)
                {
                    var weapondata = GameInfo.Instance.GetWeaponData(_sellitemlist[i]);
                    if (weapondata != null)
                    {
                        gold += weapondata.TableData.SellPrice;
                        sp += weapondata.TableData.SellMPoint;
                    }
                }

                kSpGoodsUnit.InitGoodsUnit(eGOODSTYPE.SUPPORTERPOINT, sp);
            }
            else if (kItemTab.kSelectTab == (int)eTabType.TabType_Gem)
            {
                for (int i = 0; i < _sellitemlist.Count; i++)
                {
                    var gemdata = GameInfo.Instance.GetGemData(_sellitemlist[i]);
                    if (gemdata != null)
                        gold += gemdata.TableData.SellPrice;
                }
            }
            else if (kItemTab.kSelectTab == (int)eTabType.TabType_Card)
            {
                int sp = 0;
                for (int i = 0; i < _sellitemlist.Count; i++)
                {
                    var carddata = GameInfo.Instance.GetCardData(_sellitemlist[i]);
                    if (carddata != null)
                    {
                        gold += carddata.TableData.SellPrice;
                        sp += carddata.TableData.SellMPoint;
                    }
                }

                kSpGoodsUnit.InitGoodsUnit(eGOODSTYPE.SUPPORTERPOINT, sp);
            }
            else if (kItemTab.kSelectTab == (int)eTabType.TabType_Badge)
            {
                for (int i = 0; i < _sellitemlist.Count; i++)
                {
                    var badgedata = GameInfo.Instance.GetBadgeData(_sellitemlist[i]);
                    if (badgedata != null)
                    {
                        gold += GameInfo.Instance.GameConfig.BadgeSellPrice;
                    }
                }
            }
        }
        else if (kSellPopupCount.gameObject.activeSelf)
        {
            kCountLabel.textlocalize = _sellcount.ToString();

            var itemdata = GameInfo.Instance.GetItemData(_sellitemlist[0]);
            if (itemdata != null)
            {
                gold = _sellcount * itemdata.TableData.SellPrice;
            }
        }
        else if (kSellPopupCountSlide.gameObject.activeSelf)
        {
            kSellSlideCountLabel.textlocalize = _sellcount.ToString("#,##0");

            var itemdata = GameInfo.Instance.GetItemData(_sellitemlist[0]);
            if (itemdata != null)
            {
                float f = _sellcount / (float)itemdata.Count;
                if (_sellcount <= 1)
                    f = 0f;

                if (itemdata.Count <= 1)
                    f = 1f;

                kSellSlider.Set(f, false);
                //kSellSlider.value = _sellcount / (float)itemdata.Count;

                gold = _sellcount * itemdata.TableData.SellPrice;
            }
        }

        kGoldGoodsUnit.InitGoodsUnit(eGOODSTYPE.GOLD, gold);
        if(kItemTab.kSelectTab == (int)eTabType.TabType_Badge)
        {
            kGoldGoodsUnit.InitGoodsUnit(eGOODSTYPE.BATTLECOIN, gold);
        }
    }

    public void ShowSellPopup()
    {
        ReflashSellPopup(isLastPosCheck: false);

        PlayAnimtion(2);
    }

    public void HideSellPopup()
    {
        Log.Show("HideSellPopup", Log.ColorType.Red);
        _bselpopup = false;
        _sellitemlist.Clear();
        this._listItemInstance.RefreshNotMove();

        PlayAnimtion(3);

        HideItemMenu((int)eMenuType.MenuType_Info);
    }

    public void ShowLockPopup()
    {
        RollbackLockInfo();
        _listItemInstance.RefreshNotMove();
        
        _blockpopup = true;
        _sellitemlist.Clear();

        kLockPopup.gameObject.SetActive(true);
        kLockPopupEmpty.gameObject.SetActive(true);
        kLockPopupSelete.gameObject.SetActive(false);

        PlayAnimtion(4);
    }

    public void HideLockPopup(int result = -1)
    {
        if (result != 0)
            RollbackLockInfo();

        _blockpopup = false;
        _sellitemlist.Clear();
        this._listItemInstance.RefreshNotMove();

        PlayAnimtion(5);

        HideItemMenu((int)eMenuType.MenuType_Info);
    }

    private void RollbackLockInfo()
    {
        for (int i = 0; i < _sellitemlist.Count; i++)
        {
            if (_tabindex == (int)eTabType.TabType_Weapon)
            {
                WeaponData weapondata = GameInfo.Instance.GetWeaponData(_sellitemlist[i]);
                if (weapondata == null)
                    return;
                weapondata.Lock = !weapondata.Lock;
            }
            else if (_tabindex == (int)eTabType.TabType_Gem)
            {
                GemData gemdata = GameInfo.Instance.GetGemData(_sellitemlist[i]);
                if (gemdata == null)
                    return;
                gemdata.Lock = !gemdata.Lock;
            }
            else if (_tabindex == (int)eTabType.TabType_Card)
            {
                CardData carddata = GameInfo.Instance.GetCardData(_sellitemlist[i]);
                if (carddata == null)
                    return;
                carddata.Lock = !carddata.Lock;
            }
            else if (_tabindex == (int)eTabType.TabType_Badge)
            {
                BadgeData badgedata = GameInfo.Instance.GetBadgeData(_sellitemlist[i]);
                if (badgedata == null)
                    return;
                badgedata.Lock = !badgedata.Lock;
            }
        }
    }

    public void OnNetWeaponLock(int result, PktMsgType pktmsg)
    {
        InitComponent();
        Renewal(true);

        ShowLockMessage();
        HideLockPopup(result);
    }
    public void OnNetGemLock(int result, PktMsgType pktmsg)
    {
        InitComponent();
        Renewal(true);

        ShowLockMessage();
        HideLockPopup(result);
    }
    public void OnNetCardLock(int result, PktMsgType pktmsg)
    {
        InitComponent();
        Renewal(true);

        ShowLockMessage();
        HideLockPopup(result);
    }

    public void OnNetBadgeLock(int result, PktMsgType pktmsg)
    {
        InitComponent();
        Renewal(true);

        ShowLockMessage();
        HideLockPopup(result);
    }

    /// <summary>
    ///  잠금 및 잠금해제하여 메세지 팝업으로 표시합니다.
    /// </summary>
    private void ShowLockMessage()
    {
        string str = FLocalizeString.Instance.GetText(3050);
        MessageToastPopup.Show(str);
    }

    public void RefreshList()
    {
        this._listItemInstance.RefreshNotMove();
    }
    
    //  정렬버튼 활성화/비활성화
    private void SetActiveSortBtn(bool bActive)
    {
        UIButton sortBtn = kSortLabel.transform.parent.GetComponent<UIButton>();
        if (sortBtn != null)
        {
            sortBtn.gameObject.SetActive(bActive);
        }
    }

    //  정렬순서버튼 활성화/비활성화
    private void SetActiveOrderBtn(bool bActive)
    {
        UIButton orderBtn = kOrderLabel.transform.parent.GetComponent<UIButton>();
        if (orderBtn != null)
        {
            orderBtn.gameObject.SetActive(bActive);
        }
    }

    /// <summary>
    ///  잠금 버튼 활성화/비활성화
    /// </summary>
    private void SetActiveLockBtn(bool isActive)
    {
        if(kBtnLock != null)
        {
            kBtnLock.gameObject.SetActive(isActive);
        }
        if (kFilterBtn != null)
            kFilterBtn.gameObject.SetActive(isActive);
    }

    private void SetActiveDecompositionBtn()
    {
        if (kBtnDecomposition == null) return;

        switch((eTabType)kItemTab.kSelectTab)
        {
            case eTabType.TabType_Weapon:
            case eTabType.TabType_Card:
                kBtnDecomposition.SetActive(true);
                break;
            default:
                kBtnDecomposition.SetActive(false);
                break;
        }
    }

    //  정렬타입에 따른 정렬 텍스트 셋팅
    private void SetTextSortBtn(eSortType sortType)
    {
        int strID = 0;
        switch (sortType)
        {
            case eSortType.Grade:       strID = 1246; break;
            case eSortType.Level:       strID = 1247; break;
            case eSortType.Wake:
                {
                    if(kItemTab.kSelectTab == (int)eTabType.TabType_Weapon)
                    {
                        strID = 1248;
                    }
                    else if (kItemTab.kSelectTab == (int)eTabType.TabType_Gem)
                    {
                        strID = 1370;
                    }
                    else if (kItemTab.kSelectTab == (int)eTabType.TabType_Card)
                    {
                        strID = 1369;
                    }
                }
                break;
            case eSortType.SkillLevel:  strID = 1249; break;
            case eSortType.Get:         strID = 1250; break;
            case eSortType.Enchant:     strID = 1673; break;
            case eSortType.MainStat:
                {
                    if (kItemTab.kSelectTab == (int)eTabType.TabType_Weapon)
                    {
                        strID = 1748;
                    }
                    else if (kItemTab.kSelectTab == (int)eTabType.TabType_Card)
                    {
                        strID = 1750;
                    }
                }
                break;
            case eSortType.SubStat:
                {
                    if (kItemTab.kSelectTab == (int)eTabType.TabType_Weapon)
                    {
                        strID = 1749;
                    }
                    else if (kItemTab.kSelectTab == (int)eTabType.TabType_Card)
                    {
                        strID = 1751;
                    }
                }
                break;
        }
        FLocalizeString.SetLabel(kSortLabel, strID);
    }

    private int GetCheckSlotCount()
    {
        return _listItemInstance.ColumnCount;
    }

    private int GetCheckLineCount()
    {
        _listItemInstance.Panel.ResetAndUpdateAnchors();
        int minCount = 3;
        Vector2 itemSize = _listItemInstance.ItemSize;
        Vector2 paddingSize = _listItemInstance.Padding;
        Vector2 viewSize = _listItemInstance.Panel.GetViewSize();
        float oneLineSize = itemSize.y + paddingSize.y;
        minCount = (int)(viewSize.y / oneLineSize);
        return minCount;
    }

#region Sort_Weapon

    //무기
    //  (고정)장착 중 최우선
    //  정렬 선택 규칙
    //    등급[디폴트]
    //    레벨
    //    제련
    //    획득순

    public void SetSort_Weapon(bool isNullCheck = true)
    {
        int nullCount = 0;
        if (isNullCheck == true)
        {
            for (int i = 0; i < _WeaponList.Count; i++)
            {
                if (_WeaponList[i] == null)
                    nullCount++;
            }

            _WeaponList.RemoveRange(_WeaponList.Count - nullCount, nullCount);
        }

        switch (_sortTypeWeapon)
        {
            case eSortType.Grade: _WeaponList.Sort(SortGrade_Weapon); break;
            case eSortType.Level: _WeaponList.Sort(SortLevel_Weapon); break;
            case eSortType.Wake: _WeaponList.Sort(SortWake_Weapon); break;
            case eSortType.SkillLevel: _WeaponList.Sort(SortSkillLv_Weapon); break;
            case eSortType.Get: _WeaponList.Sort(SortGet_Weapon); break;
            case eSortType.Enchant: _WeaponList.Sort(SortEnchant_Weapon); break;
            case eSortType.MainStat: _WeaponList.Sort(SortMainStat_Weapon); break;
            case eSortType.SubStat: _WeaponList.Sort(SortSubStat_Weapon); break;
        }

        SortEquip_Weapon();

        if (isNullCheck == true)
        {
            for (int i = 0; i < nullCount; i++)
            {
                _WeaponList.Add(null);
            }
        }
    }

    private int SortGrade_Weapon(WeaponData data1, WeaponData data2)
    {
        WeaponData weapon1 = data1;
        WeaponData weapon2 = data2;

        if (_sortOrder == eSortOrder.SortOrder_Up && _sortTypeWeapon == eSortType.Grade)
        {
            weapon1 = data2;
            weapon2 = data1;
        }

        if (weapon1.TableData.Grade == weapon2.TableData.Grade)
            return SortTID_Weapon(data1, data2);
        else
            return weapon2.TableData.Grade.CompareTo(weapon1.TableData.Grade);
    }

    private int SortLevel_Weapon(WeaponData data1, WeaponData data2)
    {
        WeaponData weapon1 = data1;
        WeaponData weapon2 = data2;
        if (_sortOrder == eSortOrder.SortOrder_Up && _sortTypeWeapon == eSortType.Level)
        {
            weapon1 = data2;
            weapon2 = data1;
        }

        if (weapon1.Level == weapon2.Level)
            return SortTID_Weapon(data1, data2);
        else
            return weapon2.Level.CompareTo(weapon1.Level);
    }

    private int SortWake_Weapon(WeaponData data1, WeaponData data2)
    {
        WeaponData weapon1 = data1;
        WeaponData weapon2 = data2;
        if (_sortOrder == eSortOrder.SortOrder_Up && _sortTypeWeapon == eSortType.Wake)
        {
            weapon1 = data2;
            weapon2 = data1;
        }

        if (weapon1.Wake == weapon2.Wake)
            return SortTID_Weapon(data1, data2);
        else
            return weapon2.Wake.CompareTo(weapon1.Wake);
    }

    private int SortSkillLv_Weapon(WeaponData data1, WeaponData data2)
    {
        WeaponData weapon1 = data1;
        WeaponData weapon2 = data2;

        if (_sortOrder == eSortOrder.SortOrder_Up && _sortTypeWeapon == eSortType.SkillLevel)
        {
            weapon1 = data2;
            weapon2 = data1;
        }

        if (weapon1.SkillLv == weapon2.SkillLv)
            return SortTID_Weapon(data1, data2);
        else
            return weapon2.SkillLv.CompareTo(weapon1.SkillLv);
    }

    private int SortTID_Weapon(WeaponData data1, WeaponData data2)
    {
        WeaponData weapon1 = data1;
        WeaponData weapon2 = data2;

        if (_sortOrder == eSortOrder.SortOrder_Up && _sortTypeWeapon == eSortType.Grade)
        {
            weapon1 = data2;
            weapon2 = data1;
        }

        if (weapon1.TableData.ID == weapon2.TableData.ID)
            return SortGet_Weapon(data1, data2);
        else
            return weapon2.TableData.ID.CompareTo(weapon1.TableData.ID);
    }

    private int SortGet_Weapon(WeaponData data1, WeaponData data2)
    {
        WeaponData weapon1 = data1;
        WeaponData weapon2 = data2;
        if (_sortOrder == eSortOrder.SortOrder_Up && _sortTypeWeapon == eSortType.Get)
        {
            weapon1 = data2;
            weapon2 = data1;
        }

        return weapon2.WeaponUID.CompareTo(weapon1.WeaponUID);
    }

    private int SortEnchant_Weapon(WeaponData data1, WeaponData data2)
    {
        WeaponData weapon1 = data1;
        WeaponData weapon2 = data2;

        if (_sortOrder == eSortOrder.SortOrder_Up && _sortTypeWeapon == eSortType.Enchant)
        {
            weapon1 = data2;
            weapon2 = data1;
        }

        if (weapon1.EnchantLv == weapon2.EnchantLv)
            return SortGrade_Weapon(data1, data2);
        else
            return weapon2.EnchantLv.CompareTo(weapon1.EnchantLv);
    }

    private int SortMainStat_Weapon(WeaponData data1, WeaponData data2)
    {
        WeaponData weapon1 = data1;
        WeaponData weapon2 = data2;

        if (_sortOrder == eSortOrder.SortOrder_Up && _sortTypeWeapon == eSortType.MainStat)
        {
            weapon1 = data2;
            weapon2 = data1;
        }

        int wpn1Stat = GameSupport.GetWeaponATK(weapon1.Level, weapon1.Wake, weapon1.SkillLv, weapon1.EnchantLv, weapon1.TableData);
        int wpn2Stat = GameSupport.GetWeaponATK(weapon2.Level, weapon2.Wake, weapon2.SkillLv, weapon2.EnchantLv, weapon2.TableData);

        if (wpn1Stat == wpn2Stat)
            return SortGrade_Weapon(data1, data2);
        else
        {
            if (wpn1Stat < wpn2Stat)
                return 1;
            else
                return -1;
        }
    }

    private int SortSubStat_Weapon(WeaponData data1, WeaponData data2)
    {
        WeaponData weapon1 = data1;
        WeaponData weapon2 = data2;

        if (_sortOrder == eSortOrder.SortOrder_Up && _sortTypeWeapon == eSortType.SubStat)
        {
            weapon1 = data2;
            weapon2 = data1;
        }

        int wpn1Stat = GameSupport.GetWeaponCRI(weapon1.Level, weapon1.Wake, weapon1.SkillLv, weapon1.EnchantLv, weapon1.TableData);
        int wpn2Stat = GameSupport.GetWeaponCRI(weapon2.Level, weapon2.Wake, weapon2.SkillLv, weapon2.EnchantLv, weapon2.TableData);

        if (wpn1Stat == wpn2Stat)
            return SortGrade_Weapon(data1, data2);
        else
        {
            if (wpn1Stat < wpn2Stat)
                return 1;
            else
                return -1;
        }
    }

    private void SortEquip_Weapon()
    {
        if (_equipFilter)
        {
            List<WeaponData> equipList = new List<WeaponData>();
            List<int> removeList = new List<int>();
            // 장착 위치와 지울 위치 기억
            for (int i = 0; i < _WeaponList.Count; i++)
            {
                if (GameInfo.Instance.GetEquipWeaponCharData(_WeaponList[i].WeaponUID) != null)
                {
                    equipList.Add(_WeaponList[i]);
                    removeList.Add(i);
                }
            }

            // 지울 시 뒤에서 부터 삭제
            for (int i = removeList.Count - 1; i >= 0; i--)
                _WeaponList.RemoveAt(removeList[i]);

            // 장착 아이템 정렬 처리
            _WeaponList.InsertRange(0, equipList);
        }
        
    }
#endregion

#region Sort_Gem

    //곡옥
    //  (고정)장착 중 최우선
    //  정렬 선택 규칙
    //    등급[디폴트]
    //    레벨
    //    획득순

    public void SetSort_Gem(bool isNullCheck = true)
    {
        int nullCount = 0;
        if (isNullCheck == true)
        {
            for (int i = 0; i < _GemList.Count; i++)
            {
                if (_GemList[i] == null)
                    nullCount++;
            }

            _GemList.RemoveRange(_GemList.Count - nullCount, nullCount);
        }

        switch (_sortTypeGem)
        {
            case eSortType.Grade: _GemList.Sort(SortGrade_Gem); break;
            case eSortType.Level: _GemList.Sort(SortLevel_Gem); break;
            case eSortType.Wake: _GemList.Sort(SortWake_Gem); break;
            case eSortType.Get:  _GemList.Sort(SortGet_Gem); break;
        }

        SortEquip_Gem();

        if (isNullCheck == true)
        {
            for (int i = 0; i < nullCount; i++)
            {
                _GemList.Add(null);
            }
        }
    }

    private int SortGrade_Gem(GemData data1, GemData data2)
    {
        GemData gem1 = data1;
        GemData gem2 = data2;

        if (_sortOrder == eSortOrder.SortOrder_Up && _sortTypeGem == eSortType.Grade)
        {
            gem1 = data2;
            gem2 = data1;
        }

        if (gem1.TableData.Grade == gem2.TableData.Grade)
            return SortTID_Gem(data1, data2);
        else
            return gem2.TableData.Grade.CompareTo(gem1.TableData.Grade);
    }

    private int SortLevel_Gem(GemData data1, GemData data2)
    {
        GemData gem1 = data1;
        GemData gem2 = data2;

        if (_sortOrder == eSortOrder.SortOrder_Up && _sortTypeGem == eSortType.Level)
        {
            gem1 = data2;
            gem2 = data1;
        }

        if (gem1.Level == gem2.Level)
            return SortTID_Gem(data1, data2);
        else
            return gem2.Level.CompareTo(gem1.Level);
    }

    private int SortWake_Gem(GemData data1, GemData data2)
    {
        GemData gem1 = data1;
        GemData gem2 = data2;
        if (_sortOrder == eSortOrder.SortOrder_Up && _sortTypeGem == eSortType.Wake)
        {
            gem1 = data2;
            gem2 = data1;
        }

        if (gem1.Wake == gem2.Wake)
            return SortTID_Gem(data1, data2);
        else
            return gem2.Wake.CompareTo(gem1.Wake);
    }

    private int SortTID_Gem(GemData data1, GemData data2)
    {
        GemData gem1 = data1;
        GemData gem2 = data2;

        if (_sortOrder == eSortOrder.SortOrder_Up && _sortTypeGem == eSortType.Grade)
        {
            gem1 = data2;
            gem2 = data1;
        }

        if (gem1.TableData.ID == gem2.TableData.ID)
            return SortGet_Gem(data1, data2);
        else
            return gem2.TableData.ID.CompareTo(gem1.TableData.ID);
    }

    private int SortGet_Gem(GemData data1, GemData data2)
    {
        GemData gem1 = data1;
        GemData gem2 = data2;

        if (_sortOrder == eSortOrder.SortOrder_Up && _sortTypeGem == eSortType.Get)
        {
            gem1 = data2;
            gem2 = data1;
        }

        return gem2.GemUID.CompareTo(gem1.GemUID);
    }

    private void SortEquip_Gem()
    {
        if (_equipFilter)
        {
            List<GemData> equipList = new List<GemData>();
            List<int> removeList = new List<int>();

            // 장착 위치와 지울 위치 기억
            for (int i = 0; i < _GemList.Count; i++)
            {
                if (GameInfo.Instance.GetEquipGemWeaponData(_GemList[i].GemUID) != null)
                {
                    equipList.Add(_GemList[i]);
                    removeList.Add(i);
                }
            }

            // 지울 시 뒤에서 부터 삭제
            for (int i = removeList.Count - 1; i >= 0; i--)
                _GemList.RemoveAt(removeList[i]);

            // 장착 아이템 정렬 처리
            _GemList.InsertRange(0, equipList);
        }
    }

#endregion

#region Sort_Card

    //서포터
    //  (고정)장착 중 최우선
    //  정렬 선택 규칙
    //    등급[디폴트]
    //    레벨
    //    스킬 레벨
    //    행운
    //    획득순

    public void SetSort_Card(bool isNullCheck = true)
    {
        //  빈 데이터 갯수 확인및 갯수만큼 리스트에서 삭제
        int nullCount = 0;
        if (isNullCheck == true)
        {
            for (int i = 0; i < _CardList.Count; i++)
            {
                if (_CardList[i] == null)
                    nullCount++;
            }

            _CardList.RemoveRange(_CardList.Count - nullCount, nullCount);
        }

        //  남은 데이터로 정렬
        switch (_sortTypeCard)
        {
            case eSortType.Grade: _CardList.Sort(SortGrade_Card); break;
            case eSortType.Level: _CardList.Sort(SortLavel_Card); break;
            case eSortType.Wake:  _CardList.Sort(SortWake_Card); break;
            case eSortType.SkillLevel: _CardList.Sort(SortSkillLv_Card); break;
            case eSortType.Get: _CardList.Sort(SortGet_Card);  break;
            case eSortType.Enchant: _CardList.Sort(SortEnchant_Card); break;
            case eSortType.MainStat: _CardList.Sort(SortMainStat_Card); break;
            case eSortType.SubStat: _CardList.Sort(SortSubStat_Card); break;
        }
        //  정렬이 끝난후 마지막으로 장착 정렬
        SortEquip_Card();

        //  빈 데이터 추가
        if (isNullCheck == true)
        {
            for (int i = 0; i < nullCount; i++)
            {
                _CardList.Add(null);
            }
        }
    }

    private int SortGrade_Card(CardData data1, CardData data2)
    {
        CardData card1 = data1;
        CardData card2 = data2;

        if (_sortOrder == eSortOrder.SortOrder_Up && _sortTypeCard == eSortType.Grade)
        {
            card1 = data2;
            card2 = data1;
        }

        if (card1.TableData.Grade == card2.TableData.Grade)
            return SortTID_Card(data1, data2);
        else
            return card2.TableData.Grade.CompareTo(card1.TableData.Grade);
    }

    private int SortLavel_Card(CardData data1, CardData data2)
    {
        CardData card1 = data1;
        CardData card2 = data2;

        if (_sortOrder == eSortOrder.SortOrder_Up && _sortTypeCard == eSortType.Level)
        {
            card1 = data2;
            card2 = data1;
        }

        if (card1.Level == card2.Level)
            return SortTID_Card(data1, data2);
        else
            return card2.Level.CompareTo(card1.Level);
    }

    private int SortWake_Card(CardData data1, CardData data2)
    {
        CardData card1 = data1;
        CardData card2 = data2;
        if (_sortOrder == eSortOrder.SortOrder_Up && _sortTypeCard == eSortType.Wake)
        {
            card1 = data2;
            card2 = data1;
        }

        if (card1.Wake == card2.Wake)
            return SortTID_Card(data1, data2);
        else
            return card2.Wake.CompareTo(card1.Wake);
    }

    private int SortSkillLv_Card(CardData data1, CardData data2)
    {
        CardData card1 = data1;
        CardData card2 = data2;

        if (_sortOrder == eSortOrder.SortOrder_Up && _sortTypeCard == eSortType.SkillLevel)
        {
            card1 = data2;
            card2 = data1;
        }

        if (card1.SkillLv == card2.SkillLv)
            return SortTID_Card(data1, data2);
        else
            return card2.SkillLv.CompareTo(card1.SkillLv);
    }

    private int SortTID_Card(CardData data1, CardData data2)
    {
        CardData card1 = data1;
        CardData card2 = data2;

        if (_sortOrder == eSortOrder.SortOrder_Up && _sortTypeCard == eSortType.Grade)
        {
            card1 = data2;
            card2 = data1;
        }

        if (card1.TableData.ID == card2.TableData.ID)
            return SortGet_Card(data1, data2);
        else
            return card2.TableData.ID.CompareTo(card1.TableData.ID);
    }

    private int SortGet_Card(CardData data1, CardData data2)
    {
        CardData card1 = data1;
        CardData card2 = data2;

        if (_sortOrder == eSortOrder.SortOrder_Up && _sortTypeCard == eSortType.Get)
        {
            card1 = data2;
            card2 = data1;
        }

        return card2.CardUID.CompareTo(card1.CardUID);
    }

    private int SortEnchant_Card(CardData data1, CardData data2)
    {
        CardData card1 = data1;
        CardData card2 = data2;

        if (_sortOrder == eSortOrder.SortOrder_Up && _sortTypeCard == eSortType.Enchant)
        {
            card1 = data2;
            card2 = data1;
        }

        if (card1.EnchantLv == card2.EnchantLv)
            return SortGrade_Card(data1, data2);
        else
            return card2.EnchantLv.CompareTo(card1.EnchantLv);
    }

    private int SortMainStat_Card(CardData data1, CardData data2)
    {
        CardData card1 = data1;
        CardData card2 = data2;

        if (_sortOrder == eSortOrder.SortOrder_Up && _sortTypeCard == eSortType.MainStat)
        {
            card1 = data2;
            card2 = data1;
        }

        int card1Stat = GameSupport.GetCardHP(card1.Level, card1.Wake, card1.SkillLv, card1.EnchantLv, card1.TableData);
        int card2Stat = GameSupport.GetCardHP(card2.Level, card2.Wake, card2.SkillLv, card2.EnchantLv, card2.TableData);

        if (card1Stat == card2Stat)
            return SortGrade_Card(data1, data2);
        else
        {
            if (card1Stat < card2Stat)
                return 1;
            else
                return -1;
        }
    }

    private int SortSubStat_Card(CardData data1, CardData data2)
    {
        CardData card1 = data1;
        CardData card2 = data2;

        if (_sortOrder == eSortOrder.SortOrder_Up && _sortTypeCard == eSortType.SubStat)
        {
            card1 = data2;
            card2 = data1;
        }

        int card1Stat = GameSupport.GetCardDEF(card1.Level, card1.Wake, card1.SkillLv, card1.EnchantLv, card1.TableData);
        int card2Stat = GameSupport.GetCardDEF(card2.Level, card2.Wake, card2.SkillLv, card2.EnchantLv, card2.TableData);

        if (card1Stat == card2Stat)
            return SortGrade_Card(data1, data2);
        else
        {
            if (card1Stat < card2Stat)
                return 1;
            else
                return -1;
        }
    }

    private void SortEquip_Card()
    {
        if (_equipFilter)
        {
            List<CardData> equipList = new List<CardData>();
            List<int> removeList = new List<int>();

            // 장착 위치와 지울 위치 기억
            for (int i = 0; i < _CardList.Count; i++)
            {
                if (GameInfo.Instance.GetEquiCardCharData(_CardList[i].CardUID) != null)
                {
                    equipList.Add(_CardList[i]);
                    removeList.Add(i);
                }
            }

            // 지울 시 뒤에서 부터 삭제
            for (int i = removeList.Count - 1; i >= 0; i--)
                _CardList.RemoveAt(removeList[i]);

            // 장착 아이템 정렬 처리
            _CardList.InsertRange(0, equipList);
        }
    }

#endregion

#region Sort_Item

    //소재
    //  기본 정렬
    //      (고정)타입이 최우선(소비 -> 소재)
    //      정렬 기본 규칙
    //          SubType(오름차순)
    //          등급(내림차순)
    //          Table.ID(오름차순)

    public void SetSort_Item(bool isNullCheck = true)
    {
        //  빈 데이터 갯수 확인및 갯수만큼 리스트에서 삭제
        int nullCount = 0;
        if (isNullCheck == true)
        {
            for (int i = 0; i < _ItemList.Count; i++)
            {
                if (_ItemList[i] == null)
                    nullCount++;
            }

            _ItemList.RemoveRange(_ItemList.Count - nullCount, nullCount);
        }

        _ItemList.Sort(SortType_Item);

        //  빈 데이터 추가
        if (isNullCheck == true)
        {
            for (int i = 0; i < nullCount; i++)
            {
                _ItemList.Add(null);
            }
        }
    }

    private int SortType_Item(ItemData data1,ItemData data2)
    {
        if (data1.TableData.Type == data2.TableData.Type)
            return SortSub_Item(data1, data2);
        else
            return data2.TableData.Type.CompareTo(data1.TableData.Type);
    }

    private int SortSub_Item(ItemData data1, ItemData data2)
    {
        if (data1.TableData.SubType == data2.TableData.SubType)
            return SortGrade_Item(data1, data2);
        else
            return data1.TableData.SubType.CompareTo(data2.TableData.SubType);
    }

    private int SortGrade_Item(ItemData data1, ItemData data2)
    {
        if (data1.TableData.Grade == data2.TableData.Grade)
            return SortTableID_Item(data1, data2);
        else
            return data2.TableData.Grade.CompareTo(data1.TableData.Grade);
    }

    private int SortTableID_Item(ItemData data1, ItemData data2)
    {
        return data1.TableID.CompareTo(data2.TableID);
    }

#endregion

#region Sort_Badge
    public void SetSort_Badge(bool isNullCheck = true)
    {
        int nullCount = 0;
        if (isNullCheck == true)
        {
            for (int i = 0; i < _BadgeList.Count; i++)
            {
                if (_BadgeList[i] == null)
                    nullCount++;
            }

            _BadgeList.RemoveRange(_BadgeList.Count - nullCount, nullCount);
        }

        switch (_sortTypeBadge)
        {
            case eSortType.Level: _BadgeList.Sort(SortLevel_Badge); break;
            case eSortType.Get: _BadgeList.Sort(SortGet_Badge); break;
        }

        SortEquip_Badge();

        if (isNullCheck == true)
        {
            for (int i = 0; i < nullCount; i++)
            {
                _BadgeList.Add(null);
            }
        }
    }

    private int SortLevel_Badge(BadgeData data1, BadgeData data2)
    {
        BadgeData badge1 = data1;
        BadgeData badge2 = data2;
        if (_sortOrder == eSortOrder.SortOrder_Up && _sortTypeBadge == eSortType.Level)
        {
            badge1 = data2;
            badge2 = data1;
        }

        if (badge1.Level == badge2.Level)
            return SortTID_Badge(data1, data2);
        else
            return badge2.Level.CompareTo(badge1.Level);
    }

    private int SortTID_Badge(BadgeData data1, BadgeData data2)
    {
        BadgeData badge1 = data1;
        BadgeData badge2 = data2;

        if (_sortOrder == eSortOrder.SortOrder_Up && _sortTypeBadge == eSortType.Grade)
        {
            badge1 = data2;
            badge2 = data1;
        }

        GameTable.BadgeOpt.Param dataopt1 = GameInfo.Instance.GameTable.FindBadgeOpt(x => x.OptionID == data1.OptID[0]);
        GameTable.BadgeOpt.Param dataopt2 = GameInfo.Instance.GameTable.FindBadgeOpt(x => x.OptionID == data2.OptID[0]);

        if (dataopt1.OptionID == dataopt2.OptionID)
            return SortGet_Badge(data1, data2);
        else
            return dataopt2.OptionID.CompareTo(dataopt1.OptionID);
    }

    private int SortGet_Badge(BadgeData data1, BadgeData data2)
    {
        BadgeData badge1 = data1;
        BadgeData badge2 = data2;
        if (_sortOrder == eSortOrder.SortOrder_Up && _sortTypeBadge == eSortType.Get)
        {
            badge1 = data2;
            badge2 = data1;
        }

        return badge2.BadgeUID.CompareTo(badge1.BadgeUID);
    }
    private void SortEquip_Badge()
    {
        if (_equipFilter)
        {
            List<BadgeData> equipList = new List<BadgeData>();
            List<int> removeList = new List<int>();

            // 장착 위치와 지울 위치 기억
            for (int i = 0; i < _BadgeList.Count; i++)
            {
                if (GameInfo.Instance.GetEquipBadgeData(_BadgeList[i].BadgeUID) != null)
                {
                    equipList.Add(_BadgeList[i]);
                    removeList.Add(i);
                }
            }

            // 지울 시 뒤에서 부터 삭제
            for (int i = removeList.Count - 1; i >= 0; i--)
                _BadgeList.RemoveAt(removeList[i]);

            // 장착 아이템 정렬 처리
            _BadgeList.InsertRange(0, equipList);
        }
    }
    #endregion

    #region Filter_Weapon
    List<WeaponData> weaponDummy = null;
    public void SetFilterWeapon(eFilterFlag rareFilter, eLvFilter lvFilter, eSkillLvFilter skillFilter, eWakeFilter wakeFilter, eFilterFlag charFilter, eEnchantFilter enchantFilter, bool pUpdate = true)
    {
        SetNoneLabel(false, 0);

        _rareFilter = rareFilter;
        _lvFilter = lvFilter;
        _skillLvFilter = skillFilter;
        _wakeFilter = wakeFilter;
        _enchantFilter = enchantFilter;
        _weaponCharFilter = charFilter;

        int lineMaxCount = GetCheckSlotCount(); // 한 줄의 슬롯 개수
        int ViewMaxCount = lineMaxCount * GetCheckLineCount(); // 보여지는 갯수(한줄 * 줄수)

        _WeaponList.Clear();

        /*/글로벌은 캐릭터 출시가 달라서 별도로 characterId 설정
        List<int> charID = new List<int>();
        for(int i = 0; i < GameInfo.Instance.GameTable.Characters.Count; i++)
        {
            charID.Add(GameInfo.Instance.GameTable.Characters[i].ID);
        }
        */

        if( weaponDummy == null ) {
            weaponDummy = new List<WeaponData>();
        }
        else {
            weaponDummy.Clear();
        }

        weaponDummy.Capacity = GameInfo.Instance.WeaponList.Count;

        //아래 조건을 모두 만족하면 리스트에 추가
        WeaponData data = null;
        for( int i = 0; i < GameInfo.Instance.WeaponList.Count; i++ ) {
            data = GameInfo.Instance.WeaponList[i];
            if( data == null ) {
                continue;
			}

			bool grade = ( rareFilter == eFilterFlag.ALL ) ? true : ( rareFilter & (eFilterFlag)(1 << data.TableData.Grade - 1 ) ) == (eFilterFlag)( 1 << data.TableData.Grade - 1 );
			bool level = ( lvFilter == eLvFilter.ALL ) ? true : ( data.Level == 1 );
			bool skill = ( skillFilter == eSkillLvFilter.ALL ) ? true : ( data.SkillLv == 1 );
			bool wake = ( wakeFilter == eWakeFilter.ALL ) ? true : ( data.Wake == 0 );
			bool enchant = ( enchantFilter == eEnchantFilter.ALL ) ? true : ( data.EnchantLv == 0 );
			bool character = charFilter == eFilterFlag.ALL;

			if( charFilter != eFilterFlag.ALL ) {
                for( int j = 0; j < data.ListCharId.Count; j++ ) {
                    int index = GameInfo.Instance.GameTable.Characters.FindIndex( x => x.ID == data.ListCharId[j] );
                    if( ( charFilter & (eFilterFlag)( 1 << index ) ) == (eFilterFlag)( 1 << index ) ) {
                        character = true;
                        break;
					}
                }
            }

            if( grade && level && skill && wake && enchant && character ) {
                weaponDummy.Add( data );
            }
        }

        /*
        List<WeaponData> weaponDummy = GameInfo.Instance.WeaponList.FindAll(x =>
                ((rareFilter == eFilterFlag.ALL) ? true : (rareFilter & (eFilterFlag)(1 << x.TableData.Grade - 1)) == (eFilterFlag)(1 << x.TableData.Grade - 1)) &&     //등급 체크
                (lvFilter == eLvFilter.ALL ? true : (x.Level == 1) )&&                                  //레벨 체크
                (skillFilter == eSkillLvFilter.ALL ? true : (x.SkillLv == 1)) &&                        //스킬레벨 체크
                (wakeFilter == eWakeFilter.ALL ? true : (x.Wake == 0)) &&                               //각성 체크
                (enchantFilter == eEnchantFilter.ALL ? true : (x.EnchantLv == 0)) &&                    //인챈트 체크
                (charFilter == eFilterFlag.ALL ? true : (charFilter & (eFilterFlag)(1 << charID.FindIndex(y => y == x.TableData.CharacterID))) == (eFilterFlag)(1 << charID.FindIndex(y => y == x.TableData.CharacterID)))                   //캐릭터 체크
                );
        */

        for (int i = 0; i < weaponDummy.Count; i++)
            _WeaponList.Add(weaponDummy[i]);
            
        //보유 아이템 체크
        if(_WeaponList.Count <= 0)
        {
            if(rareFilter == eFilterFlag.ALL && lvFilter == eLvFilter.ALL && skillFilter == eSkillLvFilter.ALL && wakeFilter == eWakeFilter.ALL && charFilter == eFilterFlag.ALL)
                SetNoneLabel(true, 1593);
            else
                SetNoneLabel(true, 1594);
        }

        //  정렬
        SetSort_Weapon(false);

        int count = 0;
        if (_WeaponList.Count < ViewMaxCount)
            count = ViewMaxCount - _WeaponList.Count;
        else
            count = (_WeaponList.Count % lineMaxCount == 0) ? 0 : lineMaxCount - (_WeaponList.Count % lineMaxCount);
        for (int i = 0; i < count; i++)
            _WeaponList.Add(null);

        if (pUpdate)
        {
            _listItemInstance.UpdateList();
        }

        weaponDummy.Clear();
        weaponDummy = null;
    }
#endregion

#region Filter_Card
    public void SetFilterCard(eFilterFlag rareFilter, eLvFilter lvFilter, eSkillLvFilter skillFilter, eWakeFilter wakeFilter, eFilterFlag cardFilter, eEnchantFilter enchantFilter, bool pUpdate = true)
    {
        SetNoneLabel(false, 0);

        _rareFilter = rareFilter;
        _lvFilter = lvFilter;
        _skillLvFilter = skillFilter;
        _wakeFilter = wakeFilter;
        _enchantFilter = enchantFilter;
        _cardTypeFilter = cardFilter;

        int lineMaxCount = GetCheckSlotCount(); // 한 줄의 슬롯 개수
        int ViewMaxCount = lineMaxCount * GetCheckLineCount(); // 보여지는 갯수(한줄 * 줄수)

        _CardList.Clear();

        List<CardData> cardDummy = GameInfo.Instance.CardList.FindAll(x =>
            ((rareFilter == eFilterFlag.ALL) ? true : (rareFilter & (eFilterFlag)(1 << x.TableData.Grade - 1)) == (eFilterFlag)(1 << x.TableData.Grade - 1)) &&
            (lvFilter == eLvFilter.ALL ? true : (x.Level == 1)) &&
            (skillFilter == eSkillLvFilter.ALL ? true : (x.SkillLv == 1)) &&
            (wakeFilter == eWakeFilter.ALL ? true : (x.Wake == 0)) &&
            (enchantFilter == eEnchantFilter.ALL ? true : (x.EnchantLv == 0)) &&                    //인챈트 체크
            (cardFilter == eFilterFlag.ALL ? true : (cardFilter & (eFilterFlag)(1 << x.Type - 1)) == (eFilterFlag)(1 << x.Type - 1))
            );

        for (int i = 0; i < cardDummy.Count; i++)
            _CardList.Add(cardDummy[i]);

        //보유 아이템 체크
        if(_CardList.Count <= 0)
        {
            if(rareFilter == eFilterFlag.ALL && lvFilter == eLvFilter.ALL && skillFilter == eSkillLvFilter.ALL && wakeFilter == eWakeFilter.ALL && cardFilter == eFilterFlag.ALL)
                SetNoneLabel(true, 1593);
            else
                SetNoneLabel(true, 1594);
        }

        SetSort_Card(false);

        int count = 0;
        if (_CardList.Count < ViewMaxCount)
            count = ViewMaxCount - _CardList.Count;
        else
            count = (_CardList.Count % lineMaxCount == 0) ? 0 : lineMaxCount - (_CardList.Count % lineMaxCount);
        for (int i = 0; i < count; i++)
            _CardList.Add(null);

        if (pUpdate)
        {
            _listItemInstance.UpdateList();
        }

        cardDummy.Clear();
        cardDummy = null;
    }
#endregion

#region Filter_Gem
    public void SetFilterGem(eFilterFlag rareFilter, eLvFilter lvFilter, eWakeFilter gemWakeFilter, eFilterFlag gemFilter, bool pUpdate = true)
    {
        SetNoneLabel(false, 0);

        _rareFilter = rareFilter;
        _lvFilter = lvFilter;
        _gemTypeFilter = gemFilter;
        _gemWakeFilter = gemWakeFilter;
        int lineMaxCount = GetCheckSlotCount(); // 한 줄의 슬롯 개수
        int ViewMaxCount = lineMaxCount * GetCheckLineCount(); // 보여지는 갯수(한줄 * 줄수)

        _GemList.Clear();

        List<GemData> gemDummy = GameInfo.Instance.GemList.FindAll(x =>
            ((rareFilter == eFilterFlag.ALL) ? true : (rareFilter & (eFilterFlag)(1 << x.TableData.Grade - 1)) == (eFilterFlag)(1 << x.TableData.Grade - 1)) &&
            (lvFilter == eLvFilter.ALL ? true : (x.Level == 1)) &&
            (gemWakeFilter == eWakeFilter.ALL ? true : (x.Wake == 0)) &&
            (gemFilter == eFilterFlag.ALL ? true : (gemFilter & (eFilterFlag)(1 << x.TableData.MainType - 1)) == (eFilterFlag)(1 << x.TableData.MainType - 1))
            );

        for (int i = 0; i < gemDummy.Count; i++)
            _GemList.Add(gemDummy[i]);

        //보유 아이템 체크
        if(_GemList.Count <= 0)
        {
            if(rareFilter == eFilterFlag.ALL && lvFilter == eLvFilter.ALL && gemWakeFilter == eWakeFilter.ALL && gemFilter == eFilterFlag.ALL)
                SetNoneLabel(true, 1593);
            else
                SetNoneLabel(true, 1594);
        }

        SetSort_Gem(false);

        int count = 0;
        if (_GemList.Count < ViewMaxCount)
            count = ViewMaxCount - _GemList.Count;
        else
            count = (_GemList.Count % lineMaxCount == 0) ? 0 : lineMaxCount - (_GemList.Count % lineMaxCount);
        for (int i = 0; i < count; i++)
            _GemList.Add(null);

        if (pUpdate)
        {
            _listItemInstance.UpdateList();
        }

        gemDummy.Clear();
        gemDummy = null;
    }
#endregion

#region Filter_Badge
    public void SetFilterBadge(eBadgeLvFilter lvFilter, eBadgeLvFilter levelUpFilter, int mainOptID, int subOptID, bool pUpdate = true)
    {
        SetNoneLabel(false, 0);
        
        _badgeLvFilter = lvFilter;
        _badgeLevelUpFilter = levelUpFilter;
        _badgeMainOptID = mainOptID;
        _badgeSubOptId = subOptID;

        int lineMaxCount = GetCheckSlotCount(); // 한 줄의 슬롯 개수
        int ViewMaxCount = lineMaxCount * GetCheckLineCount(); // 보여지는 갯수(한줄 * 줄수)

        _BadgeList.Clear();

        List<BadgeData> badgeDummy = GameInfo.Instance.BadgeList.FindAll( x => 
            ((lvFilter == eBadgeLvFilter.ALL) ? true : (lvFilter == eBadgeLvFilter.LV_0) ? x.Level == 0 : x.Level != 0) &&
            ((levelUpFilter == eBadgeLvFilter.ALL) ? true : (levelUpFilter == eBadgeLvFilter.LV_0) ? x.RemainLvCnt >= GameInfo.Instance.GameConfig.BadgeLvCnt : x.RemainLvCnt < GameInfo.Instance.GameConfig.BadgeLvCnt) &&
            ((mainOptID == -1) ? true : x.OptID[(int)eBadgeOptSlot.FIRST] == mainOptID) &&
            ((subOptID == -1) ? true : (x.OptID[(int)eBadgeOptSlot.SECOND] == subOptID || x.OptID[(int)eBadgeOptSlot.THIRD] == subOptID))
            );

        for (int i = 0; i < badgeDummy.Count; i++)
            _BadgeList.Add(badgeDummy[i]);

        //보유 아이템 체크
        if(_BadgeList.Count <= 0)
        {
            if(lvFilter == eBadgeLvFilter.ALL && levelUpFilter == eBadgeLvFilter.ALL && mainOptID == -1 && subOptID == -1)
                SetNoneLabel(true, 1593);
            else
                SetNoneLabel(true, 1594);
        }

        SetSort_Badge(false);

        int count = 0;
        if (_BadgeList.Count < ViewMaxCount)
            count = ViewMaxCount - _BadgeList.Count;
        else
            count = (_BadgeList.Count % lineMaxCount == 0) ? 0 : lineMaxCount - (_BadgeList.Count % lineMaxCount);
        for (int i = 0; i < count; i++)
            _BadgeList.Add(null);

        if (pUpdate)
        {
            _listItemInstance.UpdateList();
        }

        badgeDummy.Clear();
        badgeDummy = null;
    }
#endregion

    private void DefaultFilter()
    {
        _rareFilter = eFilterFlag.ALL;
        _lvFilter = eLvFilter.ALL;
        _skillLvFilter = eSkillLvFilter.ALL;
        _wakeFilter = eWakeFilter.ALL;
        _gemWakeFilter = eWakeFilter.ALL;
        _weaponCharFilter = eFilterFlag.ALL;
        _cardTypeFilter = eFilterFlag.ALL;
        _gemTypeFilter = eFilterFlag.ALL;

        _badgeLvFilter = eBadgeLvFilter.ALL;
        _badgeLevelUpFilter = eBadgeLvFilter.ALL;
        _badgeMainOptID = -1;
        _badgeSubOptId = -1;

        UIItemFilterPopup filterPopup = LobbyUIManager.Instance.GetUI<UIItemFilterPopup>("ItemFilterPopup");
        if(filterPopup != null)
        {
            filterPopup.DefailtTab = true;
        }
    }

	public void OnClick_AutoMatBtn() {
		for ( int i = 0; i < _isGradeTabEnableArray.Length; i++ ) {
			_isGradeTabEnableArray[i] = false;
		}

		_sellitemlist.Clear();
		switch ( TabType ) {
			case eTabType.TabType_Weapon: {
				foreach ( WeaponData weaponData in _WeaponList ) {
					if ( _sellitemlist.Count >= GameInfo.Instance.GameConfig.SellCount ) {
						break;
					}

					if ( weaponData == null ) {
						continue;
					}

					if ( weaponData.Lock ) {
						continue;
					}

					if ( GameInfo.Instance.GetEquipWeaponFacilityData( weaponData.WeaponUID ) != null ) {
						continue;
					}

					if ( GameSupport.GetEquipWeaponDepot( weaponData.WeaponUID ) ) {
						continue;
					}

					if ( GameInfo.Instance.GetEquipWeaponCharData( weaponData.WeaponUID ) != null ) {
						continue;
					}

					if ( _isGradeTabEnableArray[weaponData.TableData.Grade - 1] == false ) {
						_isGradeTabEnableArray[weaponData.TableData.Grade - 1] = true;
					}

					_sellitemlist.Add( weaponData.WeaponUID );
				}
			}
			break;
			case eTabType.TabType_Card: {
				foreach ( CardData cardData in _CardList ) {
					if ( _sellitemlist.Count >= GameInfo.Instance.GameConfig.SellCount ) {
						break;
					}

					if ( cardData == null ) {
						continue;
					}

					if ( cardData.Lock ) {
						continue;
					}

					if ( GameSupport.IsEquipAndUsingCardData( cardData.CardUID ) ) {
						continue;
					}

					if ( GameInfo.Instance.GetEquiCardCharData( cardData.CardUID ) != null ) {
						continue;
					}

					if ( GameInfo.Instance.GetEquipCharFacilityData( cardData.CardUID ) != null ) {
						continue;
					}

					if ( _isGradeTabEnableArray[cardData.TableData.Grade - 1] == false ) {
						_isGradeTabEnableArray[cardData.TableData.Grade - 1] = true;
					}

					_sellitemlist.Add( cardData.CardUID );
				}
			}
			break;
			case eTabType.TabType_Gem: {
				foreach ( GemData gemData in _GemList ) {
					if ( _sellitemlist.Count >= GameInfo.Instance.GameConfig.SellCount ) {
						break;
					}

					if ( gemData == null ) {
						continue;
					}

					if ( gemData.Lock ) {
						continue;
					}

					if ( GameInfo.Instance.GetEquipGemWeaponData( gemData.GemUID ) != null ) {
						continue;
					}

					if ( _isGradeTabEnableArray[gemData.TableData.Grade - 1] == false ) {
						_isGradeTabEnableArray[gemData.TableData.Grade - 1] = true;
					}

					_sellitemlist.Add( gemData.GemUID );
				}
			}
			break;
			case eTabType.TabType_Badge: {
				foreach ( BadgeData badgeData in _BadgeList ) {
					if ( _sellitemlist.Count >= GameInfo.Instance.GameConfig.SellCount ) {
						break;
					}

					if ( badgeData == null ) {
						continue;
					}

					if ( badgeData.Lock ) {
						continue;
					}

					if ( badgeData.PosKind == (int)eContentsPosKind.ARENA || badgeData.PosKind == (int)eContentsPosKind.ARENA_TOWER ) {
						continue;
					}

					_sellitemlist.Add( badgeData.BadgeUID );
				}
			}
			break;
		}

		kSellPopupEmpty.SetActive( _sellitemlist.Count <= 0 );
		kSellPopupSelete.SetActive( 0 < _sellitemlist.Count );
		kGoldGoodsUnit.gameObject.SetActive( 0 < _sellitemlist.Count );

		RenewalSellPopup();
		_listItemInstance.RefreshNotMoveAllItem();

		if ( kSellGradeFTab.gameObject.activeSelf ) {
			for ( int i = 0; i < _isGradeTabEnableArray.Length; i++ ) {
				kSellGradeFTab.EnableTab( i, _isGradeTabEnableArray[i] );
			}
		}
	}

	private void SetNoneLabel(bool active, int stringNum)
    {
        if(kNoneListObj == null)
            return;

        kNoneListObj.SetActive(active);
        if(stringNum != (int)eCOUNT.NONE)
        {
            kNoneLabel.textlocalize = FLocalizeString.Instance.GetText(stringNum);
        }
    }
}
