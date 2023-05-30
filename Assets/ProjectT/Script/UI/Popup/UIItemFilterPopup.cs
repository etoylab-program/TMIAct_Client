using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Flags]
public enum eFilterFlag
{
    NONE = 0,
    Pick_1 = 1 << 0,
    Pick_2 = 1 << 1,
    Pick_3 = 1 << 2,
    Pick_4 = 1 << 3,
    Pick_5 = 1 << 4,
    Pick_6 = 1 << 5,
    Pick_7 = 1 << 6,
    Pick_8 = 1 << 7,
    Pick_9 = 1 << 8,
    Pick_10 = 1 << 9,
    Pick_11 = 1 << 10,
    Pick_12 = 1 << 11,
    Pick_13 = 1 << 12,
    Pick_14 = 1 << 13,
    Pick_15 = 1 << 14,
    Pick_16 = 1 << 15,
    Pick_17 = 1 << 16,
    Pick_18 = 1 << 17,
    Pick_19 = 1 << 18,
    Pick_20 = 1 << 19,
    ALL = int.MaxValue,
}

public enum eLvFilter
{
    ALL,
    LV_1,
    _COUNT_,
}

public enum eSkillLvFilter
{
    ALL,
    SLV_1,
    _COUNT_,
}

public enum eWakeFilter
{
    ALL,
    NoneWake,
    _COUNT_,
}

public enum eEnchantFilter
{
    ALL,
    NoneEnchant,
    _COUNT_,
}

public enum eBadgeLvFilter
{
    ALL,
    LV_0,
    LV_1,
    _COUNT_,
}

public class UIItemFilterPopup : FComponent
{
    public enum eFilterOpenUI
    {
        None,
        ItemPanel,
        CharMainPanel,
        BookCharListPopup,
        BookCostumeListPopup,
        CardFormationPopup,
        _MAX_,
    }

    public GameObject kMainFilterObj;
    public GameObject kBadgeFilgerObj;
    public GameObject kCharFilterObj;
    public GameObject kCostumeFilterObj;

    [Header("Main Filter")]
	public FTab kRare;
	public FTab kBasicLv;
	public FTab kSkillLv;
	public FTab kWakeLv;
    public FTab kGemWake;
    public FTab kEnchantLv;
    public FTab kCardType;
	public FTab kWeaponType;
	[SerializeField] private FList _WeaponTypeListInstance;
	public FTab kGemType;
    public GameObject kWeaponAllBtn;

    [Header("Badge Filter")]
    public FTab kBadgeLv;
    public FTab kBadgeLvUpCnt;


    public UITexture kBadgeMainOptTex;
    public UITexture kBadgeSubOptTex;

    public UILabel kBadgeMainOptLb;
    public UILabel kBadgeSubOptLb;

    public UILabel kBadgeMainOptAllLb;
    public UILabel kBadgeSubOptAllLb;

    [Header("Char Filter")]
    public FTab kCharType;
    public FTab kCharMonType;

    [Header("Costume Filter")]
    public FTab kCostumeFilterTab;
    [SerializeField] private FList _CharCostumeListInstance;
    public GameObject kCostumeAllBtn;

    [Header("CardFormation Filter")]
    public GameObject kCardformationFilterRoot;
    public FTab kFormationFlagTab;
    public FTab kFormationOptTab;
    public FTab kFormationFavorTab;

    private eFilterFlag _rareFilter = eFilterFlag.NONE;
    private eLvFilter _lvFilter = eLvFilter.ALL;
    private eSkillLvFilter _skillLvFilter = eSkillLvFilter.ALL;
    private eWakeFilter _wakeFilter = eWakeFilter.ALL;
    private eWakeFilter _gemWakeFilter = eWakeFilter.ALL;
    private eEnchantFilter _enchantFilter = eEnchantFilter.ALL;
    private eFilterFlag _weaponCharFilter = eFilterFlag.NONE;
    private eFilterFlag _cardTypeFilter = eFilterFlag.NONE;
    private eFilterFlag _gemTypeFilter = eFilterFlag.NONE;

    private eFilterFlag _charTypeFilter = eFilterFlag.NONE;
    private eFilterFlag _charMonTypeFilter = eFilterFlag.NONE;
    private eFilterFlag _costumeFilter = eFilterFlag.NONE;

    private eFilterFlag _formationFlagFilter = eFilterFlag.ALL;
    private eFilterFlag _formationOptFilter = eFilterFlag.ALL;
    private eFilterFlag _formationFavorFilter = eFilterFlag.ALL;

    public eFilterFlag RareFilter { get { return _rareFilter; } set { _rareFilter = value; } }
    public eLvFilter LvFilter { get { return _lvFilter; } set { _lvFilter = value; } }
    public eSkillLvFilter SkillLvFilter { get { return _skillLvFilter; } set { _skillLvFilter = value; } }
    public eWakeFilter WakeFilter { get { return _wakeFilter; } set { _wakeFilter = value; } }
    public eWakeFilter GemWakeFilter { get { return _gemWakeFilter; } set { _gemWakeFilter = value; } }
    public eEnchantFilter EnchantFilter { get { return _enchantFilter; } set { _enchantFilter = value; } }
    public eFilterFlag WeaponCharFilter { get { return _weaponCharFilter; } set { _weaponCharFilter = value; } }
    public eFilterFlag CardTypeFilter { get { return _cardTypeFilter; } set { _cardTypeFilter = value; } }
    public eFilterFlag GemTypeFilter { get { return _gemTypeFilter; } set { _gemTypeFilter = value; } }

    public eFilterFlag CharTypeFilter { get { return _charTypeFilter; } set { _charTypeFilter = value; } }
    public eFilterFlag CharMonTypeFilter { get { return _charMonTypeFilter; } set { _charMonTypeFilter = value; } }
    public eFilterFlag CostumeFilter { get { return _costumeFilter; } set { _costumeFilter = value; } }

    public eFilterFlag FormationFlagFilter { get { return _formationFlagFilter; } set { _formationFlagFilter = value; } }
    public eFilterFlag FormationOptFilter { get { return _formationOptFilter; } set { _formationOptFilter = value; } }
    public eFilterFlag FormationFavorFilter { get { return _formationFavorFilter; } set { _formationFavorFilter = value; } }

    private eBadgeLvFilter _badgeLvFiler = eBadgeLvFilter.ALL;
    private eBadgeLvFilter _badgeLvUpCntFilter = eBadgeLvFilter.ALL;
    private int _badgeMainOptID = -1; //0모두, 그외 option id
    private int _badgeSubOptID = -1;

    public eBadgeLvFilter BadgeLvFilter { get { return _badgeLvFiler; } set { _badgeLvFiler = value; } }
    public eBadgeLvFilter BadgeLvUpCntFilter { get { return _badgeLvUpCntFilter; } set { _badgeLvUpCntFilter = value; } }
    public int BadgeMainOptID { get { return _badgeMainOptID; } set { _badgeMainOptID = value; } }
    public int BadgeSubOptID { get { return _badgeSubOptID; } set { _badgeSubOptID = value; } }

    
    private bool _defaultTab = false;
    public bool DefailtTab { set { _defaultTab = value; } }

    private eFilterOpenUI _filterOpenUI = eFilterOpenUI.None;

    private UIItemPanel _itemPanel = null;
    private UIBookCharListPopup _bookCharListPopup = null;
    private UICharMainPanel _charMainPanel = null;
    private UIBookCostumeListPopup _bookCostumeListPopup = null;
    private UICardFormationPopup _cardFormationPopup = null;

    private List<GameTable.BadgeOpt.Param> _badgeOptList;

    public override void Awake()
	{
		base.Awake();

        kRare.EventCallBack = OnRareSelect;
        kBasicLv.EventCallBack = OnBasicLvSelect;
        kSkillLv.EventCallBack = OnSkillLvSelect;
        kWakeLv.EventCallBack = OnWakeLvSelect;
        kGemWake.EventCallBack = OnGemWakeLvSelect;
        kEnchantLv.EventCallBack = OnEnchantLvSelect;
        kCardType.EventCallBack = OnCardTypeSelect;
        kWeaponType.EventCallBack = OnWeaponTypeSelect;
        kGemType.EventCallBack = OnGemTypeSelect;

        kBadgeLv.EventCallBack = OnBadgeLvSelect;
        kBadgeLvUpCnt.EventCallBack = OnBadgeLvUpCntSelect;

        kCharType.EventCallBack = OnCharTypeSelect;
        kCharMonType.EventCallBack = OnCharMonTypeSelect;

        kCostumeFilterTab.EventCallBack = OnCostumeSelect;

        kFormationFlagTab.EventCallBack = OnFormationFlabSelect;
        kFormationOptTab.EventCallBack = OnFormationOptSelect;
        kFormationFavorTab.EventCallBack = OnFormationFavorSelect;

        _defaultTab = true;

        if (this._WeaponTypeListInstance == null) return;

        _WeaponTypeListInstance.AddRowOrColumn = GameInfo.Instance.GameTable.Characters.Count - 5;
        this._WeaponTypeListInstance.EventUpdate = this._UpdateWeaponTypeListSlot;
		this._WeaponTypeListInstance.EventGetItemCount = this._GetWeaponTypeElementCount;

        if (this._CharCostumeListInstance == null) return;

        _CharCostumeListInstance.AddRowOrColumn = GameInfo.Instance.GameTable.Characters.Count - 5;
        this._CharCostumeListInstance.EventUpdate = this._UpdateCharCostumeListSlot;
        this._CharCostumeListInstance.EventGetItemCount = this._GetCharCostumeElementCount;
    }
 
	public override void OnEnable()
	{
        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
	{
        var filterOpenUIvalue = UIValue.Instance.GetValue(UIValue.EParamType.FilterOpenUI);
        if (filterOpenUIvalue == null)
        {
            base.OnClickClose();
            return;
        }

        string filterOpenUIStr = (string)filterOpenUIvalue;

        _filterOpenUI = (eFilterOpenUI)System.Enum.Parse(typeof(eFilterOpenUI), filterOpenUIStr);

        kMainFilterObj.SetActive(false);
        kBadgeFilgerObj.SetActive(false);
        kCharFilterObj.SetActive(false);
        kCostumeFilterObj.SetActive(false);
        kCardformationFilterRoot.SetActive(false);
        switch (_filterOpenUI)
        {
            case eFilterOpenUI.ItemPanel:
                {
                    InitItemPanelFilter();
                }
                break;
            case eFilterOpenUI.CharMainPanel:
            case eFilterOpenUI.BookCharListPopup:
                {
                    if (_filterOpenUI == eFilterOpenUI.CharMainPanel)
                        _charMainPanel = LobbyUIManager.Instance.GetActiveUI<UICharMainPanel>("CharMainPanel");
                    else if (_filterOpenUI == eFilterOpenUI.BookCharListPopup)
                        _bookCharListPopup = LobbyUIManager.Instance.GetActiveUI<UIBookCharListPopup>("BookCharListPopup");

                    kCharFilterObj.SetActive(true);
                    kCharType.multiTab = true;
                    kCharMonType.multiTab = true;
                }
                break;
            case eFilterOpenUI.BookCostumeListPopup:
                {
                    _bookCostumeListPopup = LobbyUIManager.Instance.GetActiveUI<UIBookCostumeListPopup>("BookCostumeListPopup");
                    kCostumeFilterObj.SetActive(true);
                    
                    if (kCostumeFilterTab.kBtnList.Count <= 1)
                    {
                        kCostumeFilterTab.kBtnList.Clear();
                        kCostumeFilterTab.kBtnList.Add(kCostumeAllBtn);
                        this._CharCostumeListInstance.UpdateList();
                    }
                    else
                    {
                        this._CharCostumeListInstance.RefreshNotMove();
                    }

                    kCostumeFilterTab.multiTab = true;
                }
                break;
            case eFilterOpenUI.CardFormationPopup:
                {
                    _cardFormationPopup = LobbyUIManager.Instance.GetActiveUI<UICardFormationPopup>("CardFormationPopup");

                    kCardformationFilterRoot.SetActive(true);

                    kFormationFlagTab.multiTab = true;
                    kFormationOptTab.multiTab = true;
                    kFormationFavorTab.multiTab = true;
                }
                break;
        }

        if (_defaultTab)
        {
            kRare.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
            kBasicLv.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
            kSkillLv.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
            kWakeLv.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
            kEnchantLv.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
            switch (_filterOpenUI)
            {
                case eFilterOpenUI.ItemPanel:
                    {
                        if (_itemPanel.TabType == UIItemPanel.eTabType.TabType_Card)
                            kCardType.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
                        if (_itemPanel.TabType == UIItemPanel.eTabType.TabType_Weapon)
                            kWeaponType.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
                        if (_itemPanel.TabType == UIItemPanel.eTabType.TabType_Gem)
                        {
                            kGemType.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
                            kGemWake.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
                        }
                    }
                    break;
                case eFilterOpenUI.CharMainPanel:
                case eFilterOpenUI.BookCharListPopup:
                    {
                        kCharType.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
                        kCharMonType.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
                    }
                    break;
                case eFilterOpenUI.BookCostumeListPopup:
                    kCostumeFilterTab.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
                    break;
                case eFilterOpenUI.CardFormationPopup:
                    {
                        kFormationFlagTab.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
                        kFormationOptTab.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
                        kFormationFavorTab.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
                    }
                    break;
            }
            

            kBadgeLv.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
            kBadgeLvUpCnt.SetTab((int)eCOUNT.NONE, SelectEvent.Click);

            BadgeMainOptID = -1;
            BadgeSubOptID = -1;

            _defaultTab = false;
        }
    }

    private void InitItemPanelFilter()
    {
        if (_itemPanel == null)
        {
            _itemPanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
            if (_itemPanel == null)
                return;
        }

        kMainFilterObj.SetActive(true);

        _badgeOptList = GameInfo.Instance.GameTable.BadgeOpts;

        kMainFilterObj.SetActive(_itemPanel.TabType != UIItemPanel.eTabType.TabType_Badge);
        kBadgeFilgerObj.SetActive(_itemPanel.TabType == UIItemPanel.eTabType.TabType_Badge);

        kWakeLv.gameObject.SetActive(!(_itemPanel.TabType == UIItemPanel.eTabType.TabType_Gem));
        kSkillLv.gameObject.SetActive(!(_itemPanel.TabType == UIItemPanel.eTabType.TabType_Gem));
        kEnchantLv.gameObject.SetActive(!(_itemPanel.TabType == UIItemPanel.eTabType.TabType_Gem));

        kGemType.gameObject.SetActive((_itemPanel.TabType == UIItemPanel.eTabType.TabType_Gem));
        kGemWake.gameObject.SetActive((_itemPanel.TabType == UIItemPanel.eTabType.TabType_Gem));

        kCardType.gameObject.SetActive((_itemPanel.TabType == UIItemPanel.eTabType.TabType_Card));
        kWeaponType.gameObject.SetActive((_itemPanel.TabType == UIItemPanel.eTabType.TabType_Weapon));

        UIButton nBtn = kRare.kBtnList[(int)eGRADE.GRADE_N].GetComponent<UIButton>();
        if (nBtn != null)
        {
            nBtn.isEnabled = true;
            kRare.SetEnabled((int)eGRADE.GRADE_N, true);
        }
        UIButton urBtn = kRare.kBtnList[(int)eGRADE.GRADE_UR].GetComponent<UIButton>();
        if (urBtn != null)
        {
            urBtn.isEnabled = true;
            kRare.SetEnabled((int)eGRADE.GRADE_UR, true);
        }

        //맨 앞의 모두 버튼까지 확인
        if (_itemPanel.TabType == UIItemPanel.eTabType.TabType_Weapon)
        {
            if (kWeaponType.kBtnList.Count <= 1)
            {
                kWeaponType.kBtnList.Clear();
                kWeaponType.kBtnList.Add(kWeaponAllBtn);
                this._WeaponTypeListInstance.UpdateList();
            }
            else
            {
                this._WeaponTypeListInstance.RefreshNotMove();
            }

            kWakeLv.SetTabLabel(0, FLocalizeString.Instance.GetText(1409));
            kWakeLv.SetTabLabel(1, FLocalizeString.Instance.GetText(1410));
            //kWeaponType.SetTab(0, SelectEvent.Code);
        }
        else if (_itemPanel.TabType == UIItemPanel.eTabType.TabType_Card)
        {
            if (nBtn != null)
            {
                nBtn.isEnabled = false;
                kRare.SetEnabled((int)eGRADE.GRADE_N, false);
            }

            kWakeLv.SetTabLabel(0, FLocalizeString.Instance.GetText(1407));
            kWakeLv.SetTabLabel(1, FLocalizeString.Instance.GetText(1408));
        }

        kRare.multiTab = true;
        kWeaponType.multiTab = true;
        kCardType.multiTab = true;
        kGemType.multiTab = true;
    }

    public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        switch (_filterOpenUI)
        {
            case eFilterOpenUI.ItemPanel:
                RenewalItemPanel();
                break;
            case eFilterOpenUI.CharMainPanel:
                break;
            case eFilterOpenUI.BookCharListPopup:
                break;
            case eFilterOpenUI.BookCostumeListPopup:
                break;
        }
    }

    private void RenewalItemPanel()
    {
        if (_itemPanel == null)
            return;

        if (_itemPanel.TabType == UIItemPanel.eTabType.TabType_Badge)
        {
            if (BadgeMainOptID == -1)
            {
                //모든 옵션
                kBadgeMainOptAllLb.gameObject.SetActive(true);
                kBadgeMainOptTex.gameObject.SetActive(false);
                kBadgeMainOptLb.textlocalize = "-";
            }
            else
            {
                kBadgeMainOptAllLb.gameObject.SetActive(false);
                kBadgeMainOptTex.gameObject.SetActive(true);

                kBadgeMainOptTex.mainTexture = GameSupport.GetBadgeIcon(_badgeOptList[BadgeMainOptID]);
                kBadgeMainOptLb.textlocalize = FLocalizeString.Instance.GetText(_badgeOptList[BadgeMainOptID].Desc + 100000);
            }

            if (BadgeSubOptID == -1)
            {
                //모든 옵션
                kBadgeSubOptAllLb.gameObject.SetActive(true);
                kBadgeSubOptTex.gameObject.SetActive(false);
                kBadgeSubOptLb.textlocalize = "-";
            }
            else
            {
                kBadgeSubOptAllLb.gameObject.SetActive(false);
                kBadgeSubOptTex.gameObject.SetActive(true);

                kBadgeSubOptTex.mainTexture = GameSupport.GetBadgeIcon(_badgeOptList[BadgeSubOptID]);
                kBadgeSubOptLb.textlocalize = FLocalizeString.Instance.GetText(_badgeOptList[BadgeSubOptID].Desc + 100000);
            }
        }
    }
 	
	private bool OnRareSelect(int nSelect, SelectEvent type)
	{
        return MultiTabCheck(kRare, ref _rareFilter, nSelect, type);
    }
	
	private bool OnBasicLvSelect(int nSelect, SelectEvent type)
	{
        LvFilter = (eLvFilter)nSelect;
        return true;
	}
	
	private bool OnSkillLvSelect(int nSelect, SelectEvent type)
	{
        SkillLvFilter = (eSkillLvFilter)nSelect;
		return true;
	}
	
	private bool OnWakeLvSelect(int nSelect, SelectEvent type)
	{
        WakeFilter = (eWakeFilter)nSelect;
		return true;
	}

    private bool OnGemWakeLvSelect(int nSelect, SelectEvent type)
    {
        GemWakeFilter = (eWakeFilter)nSelect;
        return true;
    }
	
    private bool OnEnchantLvSelect(int nSelect, SelectEvent type)
    {
        EnchantFilter = (eEnchantFilter)nSelect;
        return true;
    }

    private bool OnCardTypeSelect(int nSelect, SelectEvent type)
	{
        return MultiTabCheck(kCardType, ref _cardTypeFilter, nSelect, type);
    }
	
	private bool OnWeaponTypeSelect(int nSelect, SelectEvent type)
	{
        return MultiTabCheck(kWeaponType, ref _weaponCharFilter, nSelect, type);
    }
	
	private bool OnGemTypeSelect(int nSelect, SelectEvent type)
	{
        return MultiTabCheck(kGemType, ref _gemTypeFilter, nSelect, type);
    }

    public bool OnBadgeLvSelect(int nSelect, SelectEvent type)
    {
        BadgeLvFilter = (eBadgeLvFilter)nSelect;
        return true;
    }

    private bool OnBadgeLvUpCntSelect(int nSelect, SelectEvent type)
    {
        BadgeLvUpCntFilter = (eBadgeLvFilter)nSelect;
        return true;
    }

    private bool OnCharTypeSelect(int nSelect, SelectEvent type)
    {
        return MultiTabCheck(kCharType, ref _charTypeFilter, nSelect, type);
    }

    private bool OnCharMonTypeSelect(int nSelect, SelectEvent type)
    {
        return MultiTabCheck(kCharMonType, ref _charMonTypeFilter, nSelect, type);
    }

    private bool OnCostumeSelect(int nSelect, SelectEvent type)
    {
        return MultiTabCheck(kCostumeFilterTab, ref _costumeFilter, nSelect, type);
    }

    private bool OnFormationFlabSelect(int nSelect, SelectEvent type)
    {
        return MultiTabCheck(kFormationFlagTab, ref _formationFlagFilter, nSelect, type);
    }

    private bool OnFormationOptSelect(int nSelect, SelectEvent type)
    {
        return MultiTabCheck(kFormationOptTab, ref _formationOptFilter, nSelect, type);
    }

    private bool OnFormationFavorSelect(int nSelect, SelectEvent type)
    {
        return MultiTabCheck(kFormationFavorTab, ref _formationFavorFilter, nSelect, type);
    }

    private void _UpdateWeaponTypeListSlot(int index, GameObject slotObject)
	{
		do
		{
            UIWeaponTypeSlot weaponSlot = slotObject.GetComponent<UIWeaponTypeSlot>();
            if (null == weaponSlot) break;

            weaponSlot.ParentGO = this.gameObject;
            weaponSlot.UpdateSlot(index + 1, GameInfo.Instance.GameTable.Characters[index].ID, GameInfo.Instance.GameTable.Characters[index].Icon);

            if (!kWeaponType.kBtnList.Contains(weaponSlot.gameObject))
                kWeaponType.kBtnList.Add(weaponSlot.gameObject);
		}while(false);
	}
	
	private int _GetWeaponTypeElementCount()
	{
		return GameInfo.Instance.GameTable.Characters.Count;
	}

    private void _UpdateCharCostumeListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIWeaponTypeSlot weaponSlot = slotObject.GetComponent<UIWeaponTypeSlot>();
            if (null == weaponSlot) break;

            weaponSlot.ParentGO = this.gameObject;
            weaponSlot.UpdateSlot(index + 1, GameInfo.Instance.GameTable.Characters[index].ID, GameInfo.Instance.GameTable.Characters[index].Icon);

            if (!kCostumeFilterTab.kBtnList.Contains(weaponSlot.gameObject))
                kCostumeFilterTab.kBtnList.Add(weaponSlot.gameObject);
        } while (false);
    }

    private int _GetCharCostumeElementCount()
    {
        return GameInfo.Instance.GameTable.Characters.Count;
    }

    public void OnClick_ConfirmBtn()
	{
        OnClickClose();
	}

    public override void OnClickClose()
    {
        switch (_filterOpenUI)
        {
            case eFilterOpenUI.ItemPanel:
                {
                    switch (_itemPanel.TabType)
                    {
                        case UIItemPanel.eTabType.TabType_Weapon:
                            {
                                _itemPanel.SetFilterWeapon(RareFilter, LvFilter, SkillLvFilter, WakeFilter, WeaponCharFilter, EnchantFilter);
                            }
                            break;
                        case UIItemPanel.eTabType.TabType_Card:
                            {
                                _itemPanel.SetFilterCard(RareFilter, LvFilter, SkillLvFilter, WakeFilter, CardTypeFilter, EnchantFilter);
                            }
                            break;
                        case UIItemPanel.eTabType.TabType_Gem:
                            {
                                _itemPanel.SetFilterGem(RareFilter, LvFilter, GemWakeFilter, GemTypeFilter);
                            }
                            break;
                        case UIItemPanel.eTabType.TabType_Badge:
                            {
                                _itemPanel.SetFilterBadge(BadgeLvFilter, BadgeLvUpCntFilter, (BadgeMainOptID == -1) ? -1 : _badgeOptList[BadgeMainOptID].OptionID, (BadgeSubOptID == -1) ? -1 : _badgeOptList[BadgeSubOptID].OptionID);
                            }
                            break;
                        default:
                            base.OnClickClose();
                            break;
                    }
                }
                break;
            case eFilterOpenUI.CharMainPanel:
                {
                    if (_charMainPanel != null)
                    {
                        _charMainPanel.SetFilterChar(CharTypeFilter, CharMonTypeFilter , true);
                    }
                }
                break;
            case eFilterOpenUI.BookCharListPopup:
                {
                    if (_bookCharListPopup != null)
                    {
                        _bookCharListPopup.SetFilterChar(CharTypeFilter, CharMonTypeFilter, true);
                    }
                }
                break;
            case eFilterOpenUI.BookCostumeListPopup:
                {
                    if (_bookCostumeListPopup != null)
                    {
                        _bookCostumeListPopup.SetFilterWeapon(CostumeFilter);
                    }
                }
                break;
            case eFilterOpenUI.CardFormationPopup:
                {
                    if(_cardFormationPopup != null)
                    {
                        _cardFormationPopup.SetFilter(FormationFlagFilter, FormationOptFilter, FormationFavorFilter, true);
                    }
                }
                break;
        }

        _itemPanel = null;
        _charMainPanel = null;
        _bookCharListPopup = null;
        _bookCostumeListPopup = null;
        _cardFormationPopup = null;

        base.OnClickClose();
    }

    public void OnClick_CancelBtn()
	{
        kRare.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
        kBasicLv.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
        kSkillLv.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
        kWakeLv.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
        kEnchantLv.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
        switch (_filterOpenUI)
        {
            case eFilterOpenUI.ItemPanel:
                {
                    if (_itemPanel.TabType == UIItemPanel.eTabType.TabType_Card)
                        kCardType.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
                    if (_itemPanel.TabType == UIItemPanel.eTabType.TabType_Weapon)
                        kWeaponType.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
                    if (_itemPanel.TabType == UIItemPanel.eTabType.TabType_Gem)
                    {
                        kGemType.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
                        kGemWake.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
                    }
                }
                break;
            case eFilterOpenUI.CharMainPanel:
            case eFilterOpenUI.BookCharListPopup:
                {
                    kCharType.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
                    kCharMonType.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
                }
                break;
            case eFilterOpenUI.BookCostumeListPopup:
                {
                    kCostumeFilterTab.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
                }
                break;
            case eFilterOpenUI.CardFormationPopup:
                {
                    kFormationFlagTab.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
                    kFormationOptTab.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
                    kFormationFavorTab.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
                }
                break;
        }

        kBadgeLv.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
        kBadgeLvUpCnt.SetTab((int)eCOUNT.NONE, SelectEvent.Click);
        BadgeMainOptID = -1;
        BadgeSubOptID = -1;
        OnClick_ConfirmBtn();
    }

    private bool MultiTabCheck(FTab tab, ref eFilterFlag filter, int nSelect, SelectEvent type)
    {
        //Enable될때 들어오는 이벤트 무시
        if (type == SelectEvent.Enable)
            return false;

        //nSelect가 0 ~ @로 들어온다. 0은 ALL 이기 때문에 무시, 그이상은 -1을 해줘야 비트연산이 제대로 작동
        eFilterFlag tempFlag = (eFilterFlag)(1 << nSelect - 1);

        //InitComponent에서 불러줄때 들어옴, 위에서 실행한 비트연산으로 하면 1개밖에 체크가 안되기때문에
        //기존에 저장한 값으로 셋팅해준다.
        if (type == SelectEvent.Code)
        {
            tempFlag = (eFilterFlag)nSelect;
        }

        //nSelect가 0이면 UI에서 제일 앞의 버튼이기때문에 ALL로 셋팅
        if (nSelect == 0)
            tempFlag = eFilterFlag.ALL;

        //ALL이 켜져있는데 다시 ALL을 눌렀을 경우 무시
        if (filter == eFilterFlag.ALL && tempFlag == filter)
        {
            //tab.DisableTab(0);
            return false;

        }

        //처음에 실행할때 RareFilter가 NONE로 셋팅외어있기 때문에 강제로 셋팅(ALL로 셋팅됨)
        if (filter == eFilterFlag.NONE)
            filter = tempFlag;

        if (tempFlag == eFilterFlag.ALL)
        {
            //ALL버튼을 눌렀을 경우 나머지 버튼을 꺼주고 ALL만 셋팅
            filter = tempFlag;
            tab.DisableTabAnotherIndex(0);
        }
        else
        {
            if (filter == eFilterFlag.ALL)
            {
                //이전에 선택된 버튼이 ALL이면 ALL을 끄고 선택한 버튼 Enable
                filter = tempFlag;
                tab.DisableTab(0);
            }
            else
            {
                if (type == SelectEvent.Code)
                    return false;
                //이미 활성화된 버튼인지 확인
                if (tab.GetTabButtonSelectCheck(nSelect))
                {
                    //활성화된 버튼이라면 비트빼기 연산을 해서 0x0000이라면 다시 그 버튼을 활성화시켜준다.
                    //ALL버튼이 아닌 다른버튼은 1개이상 활성화가 되어야 함.
                    filter = filter &= ~tempFlag;
                    if (filter == eFilterFlag.NONE)
                    {
                        filter = tempFlag;
                        return false;
                    }

                }
                else
                    filter = tempFlag | filter;
            }

            tab.DisableTab(0);
        }
        return true;
    }

    public void OnClickBadgeMainOpt_RightBtn()
    {
        BadgeMainOptID++;
        if (BadgeMainOptID >= _badgeOptList.Count)
            BadgeMainOptID = -1;

        Renewal(true);
    }
    public void OnClickBadgeMainOpt_LeftBtn()
    {
        BadgeMainOptID--;
        if(BadgeMainOptID < -1)
        {
            BadgeMainOptID = _badgeOptList.Count - 1;
        }

        Renewal(true);
    }

    public void OnClickBadgeSubOpt_RightBtn()
    {
        BadgeSubOptID++;
        if (BadgeSubOptID >= _badgeOptList.Count)
            BadgeSubOptID = -1;

        Renewal(true);
    }
    public void OnClickBadgeSubOpt_LeftBtn()
    {
        BadgeSubOptID--;
        if (BadgeSubOptID < -1)
        {
            BadgeSubOptID = _badgeOptList.Count - 1;
        }

        Renewal(true);
    }

}
