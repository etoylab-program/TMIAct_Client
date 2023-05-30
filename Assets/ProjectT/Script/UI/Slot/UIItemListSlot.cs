using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIItemListSlot : FSlot
{
    public enum ePosType
    {
        //Equi = 0,
        None = -1,
        Item_Inven = 0,


        Info,
        Mat,
        Store,
        EquipCard,
        Weapon_SeleteList,
        Card_SeleteList,
        Gem_SeleteList,

        Weapon_LevelUpMatList,
        Weapon_LevelUpMatItemList,
        Weapon_SkillLevelUpMatList,
        Weapon_SkillLevelUpMatItemList,
        Card_LevelUpMatList,
        Card_LevelUpMatItemList,
        Card_SkillLevelUpMatList,
        Card_SkillLevelUpMatItemList,
        Gem_LevelUpMatList,
        Book,
        FacilityCard_SeleteList,
        FacilityWeapon_SeleteList,

        //Facility_Mat,
        //Facility_Booster,
        RewardTable,
        Result,
        Select_Item,
        EquipWeapon,
        EquipWeaponSkin,

        ArenaStore,
        ArenaReward,

        DispatchCard_SelectList,
        DispatchCard_RewardItem,

        ArenaTower_RewardItem,

        ArmoryWeapon_SeleteList,

        FacilityMaterial_Card,
        FacilityMaterial_Weapon,

        RewardDataInfo,
        
        Char_FavorLevelUpMatList,
        Char_FavorPanelSelectItem,
        
        SpecialBuyPopup_ItemInfo,
        
        Dye_MatItemSlot,
        
        Facility_Function_Select,
        ItemInfo,

        Preset_Card,
        Preset_Card_Info,
        Preset_Card_Warning,
        Preset_Weapon,
        Preset_Weapon_Info,
        Preset_Weapon_Warning,

        RaidStore,
        RaidRecoverHpItem,

        REWARD_DATA_INFO_NOT_SELL,
        AP_BP_RECORVERY,
    }

    public UISprite kSelSpr;
    public UISprite kBGSpr;
    public UITexture kIconSpr;
    public UISprite kItemGoodsSpr;
    public UISprite kFrmGradeSpr;
    public UISprite kLockSpr;
    public UISprite kNewSpr;
    public UISprite kGradeSpr;
    public UISprite kTypeSpr;
    public UISprite kWakeSpr;
    public UISprite kInactiveSpr;
    public UILabel kCountLabel;
    public UILabel kSortLabel;

    public UISprite kCardTypeSpr;
    public UISprite kShortFallSpr;

    public UILabel kEnchantLvLabel;
    public UISprite kHeartSpr;
    public UILabel kHeartLabel;

    public UISprite kCardTypeChangeSpr;
    public UILabel kPossessItemLabel;
    public UISprite kWarningSpr;

    [Header("Add Gem UR Grade")]
    [SerializeField] private UISprite gemTypeSpr = null;
    [SerializeField] private UISprite questionSpr = null;

    [Header( "Renewal" )]
    [SerializeField] private UILabel _NumberLabel;
    [SerializeField] private UILabel _NumberTweenLabel;

	private ePosType _postype;
    private int _index;
    private ItemData _itemdata;
    private WeaponData _weapondata;
    private GemData _gemdata;
    private CardData _carddata;
    private BadgeData _badgedata;
    private RewardData _rewarddata;
    private long _charUid;

    private GameTable.Item.Param _tabledata;
    private GameTable.Card.Param _tablecarddata;
    private GameTable.Weapon.Param _tableweapondata;
    private GameTable.Gem.Param _tablegemdata;

    private GameTable.UserMark.Param _tableUserMarkData;
    private GameTable.BadgeOpt.Param _tableBadgeOptData;
    private int _badgeOptionID;

    private UIItemStateUnit _itemstateunit = null;
    private GameObject _itemlistslotbatch = null;
    private GameObject _itemlistslotgradeeff = null;

    private GameTable.Random.Param _tablerandomdata;
    private GameClientTable.Book.Param _bookdata;
    private GameTable.CardDispatchMission.Param _dispatchMissionParam;
    private GameTable.ArenaTower.Param _arenaTowerParam;

	private TweenPosition mNumberTweenPos;
	private TweenAlpha mNumberTweenAlpha;

	public CardData CardData => _carddata;
    public WeaponData WeaponData => _weapondata;

    //---------------------------------------------------------------------------------------------------------------------------------
    //
    //  카드
    //
    //---------------------------------------------------------------------------------------------------------------------------------
    private void Init()
    {
        _postype = ePosType.None;
        _index = -1;

        _itemdata = null;
        _weapondata = null;
        _gemdata = null;
        _carddata = null;
        _badgedata = null;
        _tabledata = null;
        _tablecarddata = null;
        _tableweapondata = null;
        _tablegemdata = null;

        _tableUserMarkData = null;
        _tableBadgeOptData = null;
        _badgeOptionID = -1;

        if (_itemstateunit != null)
            _itemstateunit.gameObject.SetActive(false);
        if (_itemlistslotbatch != null)
            _itemlistslotbatch.SetActive(false);
        if (_itemlistslotgradeeff != null)
            _itemlistslotgradeeff.SetActive(false);

        kSelSpr.gameObject.SetActive(false);
        kNewSpr.gameObject.SetActive(false);
        kLockSpr.gameObject.SetActive(false);
        kTypeSpr.gameObject.SetActive(false);
        kWakeSpr.gameObject.SetActive(false);
        kInactiveSpr.gameObject.SetActive(false);
        kSortLabel.gameObject.SetActive(false);
        kCountLabel.textlocalize = "";

        kItemGoodsSpr.gameObject.SetActive(false);

        if (kCardTypeSpr != null)
            kCardTypeSpr.gameObject.SetActive(false);
        if (kShortFallSpr != null)
            kShortFallSpr.gameObject.SetActive(false);
        if (kEnchantLvLabel != null)
            kEnchantLvLabel.SetActive(false);
        if (kCardTypeChangeSpr != null)
            kCardTypeChangeSpr.SetActive(false);
        if (kPossessItemLabel != null)
            kPossessItemLabel.SetActive(false);
        if (kWarningSpr != null)
            kWarningSpr.SetActive(false);

        if (gemTypeSpr != null)
        {
            gemTypeSpr.SetActive(false);
        }

        if (questionSpr != null)
        {
            questionSpr.SetActive(false);
        }

		if ( _NumberLabel != null ) {
			_NumberLabel.SetActive( false );
		}

        if ( _NumberTweenLabel != null ) {
			_NumberTweenLabel.SetActive( false );
		}
	}

    private void AtlasInit()
    {
        Object obj = FLocalizeAtlas.Instance.GetLocalizeAtlas("Common");
        if (obj != null)
        {
            GameObject gameObj = obj as GameObject;
            if (gameObj != null)
            {
                UIAtlas commonAtlas = gameObj.GetComponent<UIAtlas>();
                if (commonAtlas != null)
                {
                    kItemGoodsSpr.atlas = commonAtlas;
                }
            }
        }
    }
    //---------------------------------------------------------------------------------------------------------------------------------
    //
    //  카드
    //
    //---------------------------------------------------------------------------------------------------------------------------------


    public void UpdateSlot(ePosType postype, int index, CardData itemdata)
    {
        Init();
        _postype = postype;
        _index = index;
        _carddata = itemdata;
        if (postype == ePosType.Preset_Card_Warning)
        {
            if (kWarningSpr)
            {
                kWarningSpr.SetActive(true);
            }
        }

        if (_carddata == null)
        {
            SetBinSlot(0);
            return;
        }
        kIconSpr.gameObject.SetActive(true);
        kFrmGradeSpr.gameObject.SetActive(true);
        kGradeSpr.gameObject.SetActive(true);
        kTypeSpr.gameObject.SetActive(true);

        kBGSpr.spriteName = "itembgSlot_weapon_" + _carddata.TableData.Grade.ToString();
        
        kIconSpr.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", string.Format("Icon/Item/{0}.png", _carddata.TableData.Icon) );
        kFrmGradeSpr.spriteName = "itemfrm_weapon_" + _carddata.TableData.Grade.ToString();
        kGradeSpr.spriteName = "itemgrade_S_" + _carddata.TableData.Grade.ToString();
        kGradeSpr.MakePixelPerfect();
                
        if (_carddata.EnchantLv > 0)
        {
            kEnchantLvLabel.SetActive(true);
            kEnchantLvLabel.textlocalize = string.Format("+{0}", _carddata.EnchantLv);
            RepositionEnchantLevel(_carddata.TableData.Grade);
        }

        int type = _carddata.Type;
        if(type == 0 && itemdata.TableData != null)
        {
            type = itemdata.TableData.Type;
        }

        kTypeSpr.spriteName = "SupporterType_" + type.ToString();
        //kTypeSpr.MakePixelPerfect();

        if (_carddata.Wake != 0 )
        {
            kWakeSpr.gameObject.SetActive(true);
            kWakeSpr.transform.localPosition = new Vector3(-52.0f, 25.0f, 0.0f);
            kWakeSpr.spriteName = "itemwake_0" + _carddata.Wake.ToString();
            kWakeSpr.MakePixelPerfect();
        }
        

        kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), _carddata.Level);
        if (_carddata.Lock == true)
            kLockSpr.gameObject.SetActive(true);
        else
            kLockSpr.gameObject.SetActive(false);

        if (_carddata.TableData.Changeable > (int)eCOUNT.NONE)
        {
            if (kCardTypeChangeSpr != null)
                kCardTypeChangeSpr.SetActive(true);
        }

        CharData chardata = GameInfo.Instance.GetEquiCardCharData(_carddata.CardUID);
        if (chardata != null && index != -1)
        {
            if (_itemstateunit == null)
                _itemstateunit = CreateItemStateUnit();
            if (_itemstateunit != null)
            {
                _itemstateunit.gameObject.SetActive(true);
                _itemstateunit.InitItemStateUnit(chardata, _carddata);
            }
        }

        if (_itemlistslotgradeeff == null)
            _itemlistslotgradeeff = CreateItemListSlotGradeEFF();
        if (_itemlistslotgradeeff != null)
        {
            if (_carddata.TableData.Grade == (int)eGRADE.GRADE_UR)
                _itemlistslotgradeeff.gameObject.SetActive(true);
            else
                _itemlistslotgradeeff.gameObject.SetActive(false);
        }
        if (_postype == ePosType.Item_Inven)
        {
            //if (_carddata.RedDot)
            //    SetActiveNotice(true);
            if (_carddata.New)
                SetActiveNew(true);
        }

        if (ParentGO == null)
            return;

        if (_postype == ePosType.Item_Inven)
        {
            SetItemInvenSelete(_carddata.CardUID);
            SetFaciliyCardEquipCheck();
            SetSupportDispatchCheck();
            // 잠금 상태 또는 장비 중인 아이템인 경우 판매 팝업 시 비활성 표시
            UIItemPanel itempanel = ParentGO.GetComponent<UIItemPanel>();
            if (itempanel != null && (itempanel.IsSellPopup || itempanel.IsDecompopopup))
            {
                if (_carddata.Lock || 
                    GameInfo.Instance.GetEquiCardCharData(_carddata.CardUID) != null ||
                    GameSupport.IsEquipAndUsingCardData(_carddata.CardUID))
                    kInactiveSpr.gameObject.SetActive(true);
                else
                    kInactiveSpr.gameObject.SetActive(false);
            }
            else
                kInactiveSpr.gameObject.SetActive(false);
            SetSortLabelItemPanel(itempanel, _carddata);
        }
        else if (_postype == ePosType.Card_SeleteList)
        {
            UICharCardSeletePopup cardseletepopup = ParentGO.GetComponent<UICharCardSeletePopup>();
            if (cardseletepopup == null)
                return;
            if (cardseletepopup.SeleteCardUID == _carddata.CardUID)
            {
                kSelSpr.gameObject.SetActive(true);
                kInactiveSpr.gameObject.SetActive(true);
            }
            SetFaciliyCardEquipCheck();
            SetSupportDispatchCheck();
        }
        else if (_postype == ePosType.Card_LevelUpMatList)
        {
            UICardLevelUpPopup cardleveluppopup = ParentGO.GetComponent<UICardLevelUpPopup>();
            if (cardleveluppopup == null)
                return;
            var matitem = cardleveluppopup.MatCardList.Find(x => x.CardUID == _carddata.CardUID);
            if (matitem != null)
            {
                kSelSpr.gameObject.SetActive(true);
                kInactiveSpr.gameObject.SetActive(true);
            }
            SetSortLabel(eContentsPosKind.CARD, _carddata, UIItemPanel.eSortType.SkillLevel);
        }
        else if (_postype == UIItemListSlot.ePosType.Card_SkillLevelUpMatList)
        {
            UICardSkillLevelUpPopup cardskillleveluppopup = ParentGO.GetComponent<UICardSkillLevelUpPopup>();
            if (cardskillleveluppopup == null)
                return;
            var matitem = cardskillleveluppopup.SelMatData;
            if (matitem != null)
            {
                if (matitem.Type == MatSkillData.eTYPE.CARD)
                {
                    if (matitem.CardData != null)
                    {
                        if (matitem.CardData.CardUID == _carddata.CardUID)
                        {
                            kSelSpr.gameObject.SetActive(true);
                            kInactiveSpr.gameObject.SetActive(true);
                        }
                    }
                }
            }
            SetSortLabel(eContentsPosKind.CARD, _carddata, UIItemPanel.eSortType.SkillLevel);
        }
        else if (_postype == ePosType.FacilityCard_SeleteList )
        {
            UIFacilityCardSeletePopup facilitycardseletepopup = ParentGO.GetComponent<UIFacilityCardSeletePopup>();
            if (facilitycardseletepopup == null)
                return;
            if (facilitycardseletepopup.SeleteCardUID == _carddata.CardUID)
            {
                kSelSpr.gameObject.SetActive(true);
                kInactiveSpr.gameObject.SetActive(true);
            }
            float rate = GameSupport.GetFacilitySptOptValRate(facilitycardseletepopup.Facilitydata, _carddata);
            if (rate != 0.0f)
                SetSortLabelString(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_ONETWO_POINT_TEXT, rate * (float)eCOUNT.MAX_BO_FUNC_VALUE)));
            SetFaciliyCardEquipCheck();
        }
        else if (_postype == ePosType.DispatchCard_SelectList)
        {
            UIDispatchCardSelectPopup popup = ParentGO.GetComponent<UIDispatchCardSelectPopup>();
            if (popup == null) return;

            if(popup.SeleteCardUID == _carddata.CardUID)
            {
                kSelSpr.gameObject.SetActive(true);
                kInactiveSpr.gameObject.SetActive(true);
            }
            SetSupportDispatchCheck();
        }
        else if (_postype == ePosType.FacilityMaterial_Card)
        {
            UIFacilityCardSeletePopup p = (UIFacilityCardSeletePopup)LobbyUIManager.Instance.GetUI("FacilityCardSeletePopup");
            if (p == null) return;
            int CurGrade = FacilitySupport.GetCurTradeMaterialGrade(eFacilityTradeType.CARD);

            if (CurGrade > 0 && CurGrade != _carddata.TableData.Grade)
            {
                kSelSpr.SetActive(false);
                kInactiveSpr.SetActive(true);
                return;
            }

            bool state = FacilitySupport.IsTradeSelectMaterial(_carddata.CardUID);
            if (gameObject == ParentGO)
                state = false;

            kSelSpr.gameObject.SetActive(state);
            kInactiveSpr.gameObject.SetActive(state);
        }
    }

    public void UpdateSlot(ePosType postype, int index, GameTable.Card.Param tabledata, bool binActive = false)
    {
        Init();
        _postype = postype;
        _index = index;
        _tablecarddata = tabledata;
        if (_tablecarddata == null)
        {
            SetBinSlot(0);
            return;
        }

        kIconSpr.gameObject.SetActive(true);
        kFrmGradeSpr.gameObject.SetActive(true);
        kGradeSpr.gameObject.SetActive(true);
        kTypeSpr.gameObject.SetActive(true);

        kBGSpr.spriteName = "itembgSlot_weapon_" + _tablecarddata.Grade.ToString();
        kIconSpr.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", string.Format("Icon/Item/{0}.png", _tablecarddata.Icon));
        kFrmGradeSpr.spriteName = "itemfrm_weapon_" + _tablecarddata.Grade.ToString();
        kGradeSpr.spriteName = "itemgrade_S_" + _tablecarddata.Grade.ToString();
        kGradeSpr.MakePixelPerfect();
        kTypeSpr.spriteName = "SupporterType_" + _tablecarddata.Type.ToString();
        //kTypeSpr.MakePixelPerfect();

        if (_tablecarddata.Changeable > (int)eCOUNT.NONE)
        {
            if (kCardTypeChangeSpr != null)
                kCardTypeChangeSpr.SetActive(true);
        }

        kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), 1);

        if (_itemlistslotgradeeff == null)
            _itemlistslotgradeeff = CreateItemListSlotGradeEFF();
        if (_itemlistslotgradeeff != null)
        {
            if (_tablecarddata.Grade == (int)eGRADE.GRADE_UR)
                _itemlistslotgradeeff.gameObject.SetActive(true);
            else
                _itemlistslotgradeeff.gameObject.SetActive(false);
        }

        kInactiveSpr.SetActive(binActive);
    }

    public void UpdateSlot(ePosType posType, int index, CharData charData)
    {
        if (charData == null || charData.EquipCard.Length < index)
        {
            SetBinSlot(0);
            return;
        }

        _charUid = charData.CUID;

        switch(posType)
        {
            case ePosType.Preset_Card:
            case ePosType.Preset_Card_Info:
            case ePosType.Preset_Card_Warning:
                {
                    _carddata = GameInfo.Instance.CardList.Find(x => x.CardUID == charData.EquipCard[index]);

                    UpdateSlot(posType, index, _carddata);
                } break;
            case ePosType.Preset_Weapon:
            case ePosType.Preset_Weapon_Info:
                {
                    long weaponUid = index == 0 ? charData.EquipWeaponUID : charData.EquipWeapon2UID;

                    _weapondata = GameInfo.Instance.WeaponList.Find(x => x.WeaponUID == weaponUid);
                    UpdateSlot(posType, index, _weapondata);
                } break;
        }
        
    }
    //---------------------------------------------------------------------------------------------------------------------------------
    //
    //  아이템
    //
    //---------------------------------------------------------------------------------------------------------------------------------

    public void UpdateSlot(ePosType postype, int index, ItemData itemdata, int count = -1)
    {
        Init();
        _postype = postype;
        _index = index;
        _itemdata = itemdata;
        if (_itemdata == null)
        {
            SetBinSlot(1);
            return;
        }

        kIconSpr.gameObject.SetActive(true);
        kFrmGradeSpr.gameObject.SetActive(true);
        kGradeSpr.gameObject.SetActive(false);
        kBGSpr.spriteName = "itembgSlot_" + _itemdata.TableData.Grade.ToString();
        kIconSpr.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + _itemdata.TableData.Icon);
        kFrmGradeSpr.spriteName = "itemfrm_" + _itemdata.TableData.Grade.ToString();

        if (count == -1)
            kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT), _itemdata.Count);
        else
            kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT), count);

        if (_itemlistslotbatch == null)
            _itemlistslotbatch = CreateItemListSlotBatch();
        if (_itemlistslotbatch != null)
        {
            if (itemdata.TableData.Type == (int)eITEMTYPE.USE)
                _itemlistslotbatch.gameObject.SetActive(true);
            else
                _itemlistslotbatch.gameObject.SetActive(false);
        }

        if (_postype == ePosType.Item_Inven)
        {
            if (_itemdata.New)
                SetActiveNew(true);
        }

        if (ParentGO == null)
            return;

        if (_postype == ePosType.Item_Inven)
        {
            SetItemInvenSelete(_itemdata.ItemUID);

            UIItemPanel itempanel = ParentGO.GetComponent<UIItemPanel>();
            if (itempanel != null && itempanel.IsSellPopup == true)
            {
                if (!GameSupport.IsAbleSellItem(_itemdata.TableData))
                    kInactiveSpr.gameObject.SetActive(true);
                else
                    kInactiveSpr.gameObject.SetActive(false);
            }

            SetSortLabelItemPanel(itempanel, _badgedata);
        }
        else if (_postype == ePosType.Weapon_LevelUpMatItemList)
        {
            UIWeaponLevelUpPopup weaponleveluppopup = ParentGO.GetComponent<UIWeaponLevelUpPopup>();
            if (weaponleveluppopup == null)
                return;

            var matitem = weaponleveluppopup.MatItemList.Find(x => x.ItemData.ItemUID == _itemdata.ItemUID);
            if (matitem != null)
            {
                kSelSpr.gameObject.SetActive(true);
                kInactiveSpr.gameObject.SetActive(true);
                SetSortLabelMatCount(matitem.Count);
            }
        }
        else if (_postype == UIItemListSlot.ePosType.Weapon_SkillLevelUpMatItemList)
        {
            UIWeaponSkillLevelUpPopup weaponskillleveluppopup = ParentGO.GetComponent<UIWeaponSkillLevelUpPopup>();
            if (weaponskillleveluppopup == null)
                return;

            var matitem = weaponskillleveluppopup.SelMatData;
            if (matitem != null)
            {
                if (matitem.Type == MatSkillData.eTYPE.ITEM)
                {
                    if (matitem.ItemData != null)
                    {
                        if (matitem.ItemData.ItemUID == _itemdata.ItemUID)
                        {
                            kSelSpr.gameObject.SetActive(true);
                            kInactiveSpr.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
        else if (_postype == ePosType.Card_LevelUpMatItemList)
        {
            UICardLevelUpPopup cardleveluppopup = ParentGO.GetComponent<UICardLevelUpPopup>();
            if (cardleveluppopup == null)
                return;

            var matitem = cardleveluppopup.MatItemList.Find(x => x.ItemData.ItemUID == _itemdata.ItemUID);
            if (matitem != null)
            {
                kSelSpr.gameObject.SetActive(true);
                kInactiveSpr.gameObject.SetActive(true);
                SetSortLabelMatCount(matitem.Count);
            }
        }
        else if (_postype == ePosType.Card_SkillLevelUpMatItemList)
        {
            UICardSkillLevelUpPopup cardskillleveluppopup = ParentGO.GetComponent<UICardSkillLevelUpPopup>();
            if (cardskillleveluppopup == null)
                return;

            var matitem = cardskillleveluppopup.SelMatData;
            if (matitem != null)
            {
                if (matitem.Type == MatSkillData.eTYPE.ITEM)
                {
                    if (matitem.ItemData != null)
                    {
                        if (matitem.ItemData.ItemUID == _itemdata.ItemUID)
                        {
                            kSelSpr.gameObject.SetActive(true);
                            kInactiveSpr.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
        else if (_postype == ePosType.Char_FavorLevelUpMatList)
        {
            UICharGiveGiftPopup charGiveGiftPopup = ParentGO.GetComponent<UICharGiveGiftPopup>();
            if (charGiveGiftPopup == null)
            {
                return;
            }
            
            ItemData itemData = charGiveGiftPopup.SelItemDataList.Find(x => x.ItemUID == _itemdata.ItemUID);
            if (itemData != null)
            {
                kSelSpr.gameObject.SetActive(true);
                kInactiveSpr.gameObject.SetActive(true);
                
                SetSortLabelMatCount(itemData.Count);
                
                kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT),
                    _itemdata.Count - itemData.Count);
            }
        }
        else if( _postype == ePosType.RaidRecoverHpItem ) {
            kSelSpr.SetActive( false );

            UIRaidRecoveryItemPopup popup = ParentGO.GetComponent<UIRaidRecoveryItemPopup>();
            if( popup ) {
                kSelSpr.SetActive( popup.HasItemSlotIndex( _index ) );
			}
		}
    }

    public void UpdateSlot(ePosType postype, int index, GameTable.Item.Param tabledata)
    {
        Init();
        _postype = postype;
        _index = index;
        _tabledata = tabledata;
        if (_tabledata == null)
        {
            SetBinSlot(1);
            return;
        }

        _rewarddata = new RewardData((int)eREWARDTYPE.ITEM, _tabledata.ID, _tabledata.Value);

        kIconSpr.gameObject.SetActive(true);
        kFrmGradeSpr.gameObject.SetActive(true);
        kGradeSpr.gameObject.SetActive(false);
        kBGSpr.spriteName = "itembgSlot_" + tabledata.Grade.ToString();
        kIconSpr.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + _tabledata.Icon);
        kFrmGradeSpr.spriteName = "itemfrm_" + tabledata.Grade.ToString();

        kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT), 1);

        if (_itemlistslotbatch == null)
            _itemlistslotbatch = CreateItemListSlotBatch();
        if (_itemlistslotbatch != null)
        {
            if (_tabledata.Type == (int)eITEMTYPE.USE)
                _itemlistslotbatch.gameObject.SetActive(true);
            else
                _itemlistslotbatch.gameObject.SetActive(false);
        }
        
        if (_postype == ePosType.Facility_Function_Select)
        {
            _itemdata = GameInfo.Instance.GetItemData(tabledata.ID);
            
            kCountLabel.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT, _itemdata?.Count ?? 0);
            kInactiveSpr.SetActive(_itemdata == null);
        }
    }

    public void UpdateSlot(GameTable.CardDispatchMission.Param dispatchMissionParam)
    {
        Init();

        _postype = ePosType.DispatchCard_RewardItem;
        _dispatchMissionParam = dispatchMissionParam;

        RewardData rewardData = new RewardData(_dispatchMissionParam.RewardType, _dispatchMissionParam.RewardIndex, _dispatchMissionParam.RewardValue);
        GameSupport.UpdateSlotByRewardData(ParentGO, this.gameObject, rewardData, kFrmGradeSpr, kGradeSpr, kTypeSpr, kBGSpr, kCountLabel, kIconSpr, kItemGoodsSpr, null);
    }

	public void UpdateSlot( ePosType posType, int index, ItemData itemData, bool isSelect, int count ) {
		UpdateSlot( posType, index, itemData, count );
		SetSelectActive( isSelect );

		int addCount = itemData == null ? 0 : itemData.Count - count;
		kSortLabel.SetActive( 0 < addCount );
		if ( kSortLabel.gameObject.activeSelf ) {
			kSortLabel.text = string.Format( "+{0}", addCount );
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------------
	//
	//  무기
	//
	//---------------------------------------------------------------------------------------------------------------------------------
	public void UpdateSlot(ePosType postype, int index, WeaponData weapondata)
    {
        Init();
        _postype = postype;
        _index = index;
        _weapondata = weapondata;
        
        if (postype == ePosType.Preset_Weapon_Warning)
        {
            if (kWarningSpr)
            {
                kWarningSpr.SetActive(true);
            }
        }

        if (_weapondata == null)
        {
            SetBinSlot(0);
            return;
        } 

        kIconSpr.gameObject.SetActive(true);
        kFrmGradeSpr.gameObject.SetActive(true);
        kGradeSpr.gameObject.SetActive(true);
        kBGSpr.spriteName = "itembgSlot_weapon_" + _weapondata.TableData.Grade.ToString();
        kIconSpr.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + _weapondata.TableData.Icon);
        kFrmGradeSpr.spriteName = "itemfrm_weapon_" + _weapondata.TableData.Grade.ToString();
        kGradeSpr.spriteName = "itemgrade_S_" + _weapondata.TableData.Grade.ToString();
        kGradeSpr.MakePixelPerfect();

        if (_weapondata.Wake != 0)
        {
            kWakeSpr.gameObject.SetActive(true);
            kWakeSpr.transform.localPosition = new Vector3(-52.0f, 25.0f, 0.0f);
            kWakeSpr.spriteName = "itemwake_0" + _weapondata.Wake.ToString();
            kWakeSpr.MakePixelPerfect();
        }

        kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), _weapondata.Level);
        if (_weapondata.Lock == true)
            kLockSpr.gameObject.SetActive(true);
        else
            kLockSpr.gameObject.SetActive(false);
                
        if (_weapondata.EnchantLv > 0)
        {
            kEnchantLvLabel.SetActive(true);
            kEnchantLvLabel.textlocalize = string.Format("+{0}", _weapondata.EnchantLv);
            RepositionEnchantLevel(_weapondata.TableData.Grade);
        }

        CharData chardata = GameInfo.Instance.GetEquipWeaponCharData(_weapondata.WeaponUID);
        if (chardata != null && index != -1)
        {
            if (_itemstateunit == null)
                _itemstateunit = CreateItemStateUnit();
            if (_itemstateunit != null)
            {
                _itemstateunit.gameObject.SetActive(true);
                _itemstateunit.InitItemStateUnit(chardata, _weapondata);
            }
        }

        if (_itemlistslotgradeeff == null)
            _itemlistslotgradeeff = CreateItemListSlotGradeEFF();
        if (_itemlistslotgradeeff != null)
        {
            if (weapondata.TableData.Grade == (int)eGRADE.GRADE_UR)
                _itemlistslotgradeeff.gameObject.SetActive(true);
            else
                _itemlistslotgradeeff.gameObject.SetActive(false);
        }
        if (_postype == ePosType.Item_Inven)
        {
            //if (_weapondata.RedDot)
            //  SetActiveNotice(true);
            if (_weapondata.New)
                SetActiveNew(true);
        }

        if (ParentGO == null)
            return;

        if (_postype == ePosType.Item_Inven)
        {
            SetItemInvenSelete(_weapondata.WeaponUID);
            SetFaciliyWeaponEquipCheck();
            SetArmoryWeaponEquipCheck();
            // 잠금 상태 또는 장비 중인 아이템인 경우 판매 팝업 시 비활성 표시
            UIItemPanel itempanel = ParentGO.GetComponent<UIItemPanel>();            
            if (itempanel != null && (itempanel.IsSellPopup || itempanel.IsDecompopopup))
            {
                if (itempanel.IsSellPopup)
                {
                    if (_weapondata.Lock ||
                    GameInfo.Instance.GetEquipWeaponCharData(_weapondata.WeaponUID) != null ||
                    GameInfo.Instance.GetEquipWeaponFacilityData(_weapondata.WeaponUID) != null ||
                    GameInfo.Instance.WeaponArmoryData.ArmoryWeaponUIDList.Contains(_weapondata.WeaponUID))
                    {
                        kInactiveSpr.gameObject.SetActive(true);
                    }
                }
                else if (itempanel.IsDecompopopup)
                {
                    if (_weapondata.Lock ||
                    _weapondata.TableData.Decomposable == 0 ||
                    GameInfo.Instance.GetEquipWeaponCharData(_weapondata.WeaponUID) != null ||
                    GameInfo.Instance.GetEquipWeaponFacilityData(_weapondata.WeaponUID) != null ||
                    GameInfo.Instance.WeaponArmoryData.ArmoryWeaponUIDList.Contains(_weapondata.WeaponUID))
                    {
                        kInactiveSpr.gameObject.SetActive(true);
                    }
                }
                else
                    kInactiveSpr.gameObject.SetActive(false);
            }
            else
                kInactiveSpr.gameObject.SetActive(false);

            SetSortLabelItemPanel(itempanel, _weapondata);
        }
        else if(_postype == ePosType.EquipWeapon)
        {
            if (_itemstateunit != null)
                _itemstateunit.gameObject.SetActive(false);
        }
        else if (_postype == ePosType.Weapon_SeleteList)
        {
            UICharWeaponSeletePopup weaponseletepopup = ParentGO.GetComponent<UICharWeaponSeletePopup>();
            if (weaponseletepopup == null)
                return;
            if (weaponseletepopup.SeleteWeaponUID == weapondata.WeaponUID)
            {
                kSelSpr.gameObject.SetActive(true);
                kInactiveSpr.gameObject.SetActive(true);
            }
            SetFaciliyWeaponEquipCheck();
        }
        else if (_postype == ePosType.Weapon_LevelUpMatList)
        {
            UIWeaponLevelUpPopup weaponleveluppopup = ParentGO.GetComponent<UIWeaponLevelUpPopup>();
            if (weaponleveluppopup == null)
                return;
            var matitem = weaponleveluppopup.MatWeaponList.Find(x => x.WeaponUID == _weapondata.WeaponUID);
            if (matitem != null)
            {
                kSelSpr.gameObject.SetActive(true);
                kInactiveSpr.gameObject.SetActive(true);
            }
            SetSortLabel(eContentsPosKind.WEAPON, _weapondata, UIItemPanel.eSortType.SkillLevel);
        }
        else if (_postype == UIItemListSlot.ePosType.Weapon_SkillLevelUpMatList)
        {
            UIWeaponSkillLevelUpPopup weaponskillleveluppopup = ParentGO.GetComponent<UIWeaponSkillLevelUpPopup>();
            if (weaponskillleveluppopup == null)
                return;
            var matitem = weaponskillleveluppopup.SelMatData;
            if (matitem != null)
            {
                if (matitem.Type == MatSkillData.eTYPE.WEAPON)
                {
                    if (matitem.WeaponData != null)
                    {
                        if (matitem.WeaponData.WeaponUID == _weapondata.WeaponUID)
                        {
                            kSelSpr.gameObject.SetActive(true);
                            kInactiveSpr.gameObject.SetActive(true);
                        }
                    }
                }
            }
            SetSortLabel(eContentsPosKind.WEAPON, _weapondata, UIItemPanel.eSortType.SkillLevel);
        }
        else if (_postype == ePosType.FacilityWeapon_SeleteList || _postype == ePosType.ArmoryWeapon_SeleteList)
        {
            UIFacilityWeaponSeletePopup facilityweaponseletepopup = ParentGO.GetComponent<UIFacilityWeaponSeletePopup>();
            if (facilityweaponseletepopup == null)
                return;
            if (facilityweaponseletepopup.SeleteWeaponUID == _weapondata.WeaponUID)
            {
                kSelSpr.gameObject.SetActive(true);
                kInactiveSpr.gameObject.SetActive(true);
            }
            SetFaciliyWeaponEquipCheck();
            SetArmoryWeaponEquipCheck();
        }
        
        else if (_postype == ePosType.FacilityMaterial_Weapon)
        {
            UIFacilityWeaponSeletePopup p = (UIFacilityWeaponSeletePopup)LobbyUIManager.Instance.GetUI("FacilityWeaponSeletePopup");
            if (p == null) return;
            
            int CurGrade = FacilitySupport.GetCurTradeMaterialGrade(eFacilityTradeType.WEAPON);

            if (CurGrade > 0 && CurGrade != _weapondata.TableData.Grade)
            {
                kSelSpr.SetActive(false);
                kInactiveSpr.SetActive(true);                
                return;
            }

            bool state = FacilitySupport.IsTradeSelectMaterial(_weapondata.WeaponUID);
            if (gameObject == ParentGO)
                state = false;
            kSelSpr.gameObject.SetActive(state);
            kInactiveSpr.gameObject.SetActive(state);
        }
    }

    public void UpdateSlot(ePosType postype, int index, GameTable.Weapon.Param tableweapondata)
    {
        Init();
        _postype = postype;
        _index = index;
        _tableweapondata = tableweapondata;
        if (_tableweapondata == null)
        {
            SetBinSlot(0);
            return;
        }

        kIconSpr.gameObject.SetActive(true);
        kFrmGradeSpr.gameObject.SetActive(true);
        kGradeSpr.gameObject.SetActive(true);
        kBGSpr.spriteName = "itembgSlot_weapon_" + tableweapondata.Grade.ToString();
        kIconSpr.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + tableweapondata.Icon);
        kFrmGradeSpr.spriteName = "itemfrm_weapon_" + tableweapondata.Grade.ToString();
        kGradeSpr.spriteName = "itemgrade_S_" + tableweapondata.Grade.ToString();
        kGradeSpr.MakePixelPerfect();

        kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), 1);

        if (_itemlistslotgradeeff == null)
            _itemlistslotgradeeff = CreateItemListSlotGradeEFF();
        if (_itemlistslotgradeeff != null)
        {
            if (_tableweapondata.Grade == (int)eGRADE.GRADE_UR)
                _itemlistslotgradeeff.gameObject.SetActive(true);
            else
                _itemlistslotgradeeff.gameObject.SetActive(false);
        }
    }

    public void UpdateSlot(ePosType postype, int index, GameClientTable.Book.Param weaponbookdata, bool selected = false)
    {
        Init();
        _postype = postype;
        _index = index;
        if (weaponbookdata == null)
        {
            SetBinSlot(1);
            return;
        }

        _bookdata = weaponbookdata;
        kSelSpr.gameObject.SetActive(selected);
        kIconSpr.gameObject.SetActive(true);
        kFrmGradeSpr.gameObject.SetActive(true);
        kGradeSpr.gameObject.SetActive(true);
        kInactiveSpr.gameObject.SetActive(false);

        GameTable.Weapon.Param weapondata = GameInfo.Instance.GameTable.FindWeapon(x => x.ID == weaponbookdata.ItemID);

        kBGSpr.spriteName = "itembgSlot_weapon_" + weapondata.Grade.ToString();
        kIconSpr.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + weapondata.Icon);
        kFrmGradeSpr.spriteName = "itemfrm_weapon_" + weapondata.Grade.ToString();
        kGradeSpr.spriteName = "itemgrade_S_" + weapondata.Grade.ToString();
        kGradeSpr.MakePixelPerfect();

        if (GameInfo.Instance.BattleConfig.TestWeaponMaxWakeView == true)
        {
            kInactiveSpr.gameObject.SetActive(false);
        }
        else
        {
            WeaponBookData bookdata = GameInfo.Instance.WeaponBookList.Find(x => x.TableID == weapondata.ID);
            if (bookdata == null)
                kInactiveSpr.gameObject.SetActive(true);
            else
            {
                kInactiveSpr.gameObject.SetActive(false);

                /* 최대 강화 조건 제거
                if (!bookdata.IsOnFlag(eBookStateFlag.MAX_WAKE_AND_LV))
                    kInactiveSpr.gameObject.SetActive(true);
                    */
            }
        }
        kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(216), weaponbookdata.Num);
    }

    //---------------------------------------------------------------------------------------------------------------------------------
    //
    //  곡옥
    //
    //---------------------------------------------------------------------------------------------------------------------------------
    public void UpdateSlot(ePosType postype, int index, GemData gemdata)
    {
        Init();
        _postype = postype;
        _index = index;
        _gemdata = gemdata;
        if (_gemdata == null)
        {
            SetBinSlot(2);
            return;
        }

        if (GameInfo.Instance.GameConfig.GemSetGrade <= _gemdata.TableData.Grade)
        {
            GameTable.GemSetType.Param gemSetTypeParam = GameInfo.Instance.GameTable.FindGemSetType(_gemdata.SetOptID);

            gemTypeSpr.SetActive(gemSetTypeParam != null);
            questionSpr.SetActive(gemSetTypeParam == null);

            if (gemSetTypeParam != null)
            {
                gemTypeSpr.spriteName = gemSetTypeParam.Icon;
            }
        }

        kIconSpr.gameObject.SetActive(true);
        kFrmGradeSpr.gameObject.SetActive(true);
        kGradeSpr.gameObject.SetActive(true);
        kBGSpr.spriteName = "itembgSlot_" + _gemdata.TableData.Grade.ToString();
        kIconSpr.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + _gemdata.TableData.Icon);
        kFrmGradeSpr.spriteName = "itemfrm_" + _gemdata.TableData.Grade.ToString();
        kGradeSpr.spriteName = "itemgrade_S_" + _gemdata.TableData.Grade.ToString();
        kGradeSpr.MakePixelPerfect();

        if (_gemdata.Wake != 0)
        {
            kWakeSpr.gameObject.SetActive(true);
            kWakeSpr.transform.localPosition = new Vector3(0.0f, -37.0f, 0.0f);
            kWakeSpr.spriteName = "itemGemwake_0" + _gemdata.Wake.ToString();
            kWakeSpr.MakePixelPerfect();
        }

        if (0 < _gemdata.SetOptID)
        {
            GameTable.GemSetType.Param gemSetTypeParam = GameInfo.Instance.GameTable.FindGemSetType(_gemdata.SetOptID);
            if (gemSetTypeParam != null)
            {
                kTypeSpr.SetActive(true);
                kTypeSpr.spriteName = gemSetTypeParam.Icon;
            }
        }

        kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), _gemdata.Level);
        if (_gemdata.Lock == true)
            kLockSpr.gameObject.SetActive(true);
        else
            kLockSpr.gameObject.SetActive(false);

        if (_postype == ePosType.Item_Inven)
        {
            if (_gemdata.New)
                SetActiveNew(true);
        }

        if (ParentGO == null)
            return;

        if (_postype == ePosType.Item_Inven)
        {
            SetGemWeaponEquipCheck();

            SetItemInvenSelete(_gemdata.GemUID);

            // 잠금 상태 또는 장비 중인 아이템인 경우 판매 팝업 시 비활성 표시
            UIItemPanel itempanel = ParentGO.GetComponent<UIItemPanel>();
            if (itempanel != null && itempanel.IsSellPopup == true)
            {
                if (_gemdata.Lock || GameInfo.Instance.GetEquipGemWeaponData(_gemdata.GemUID) != null)
                    kInactiveSpr.gameObject.SetActive(true);
                else
                    kInactiveSpr.gameObject.SetActive(false);
            }
            else
                kInactiveSpr.gameObject.SetActive(false);

            SetSortLabelItemPanel(itempanel, _gemdata);
        }
        else if (_postype == ePosType.Gem_SeleteList)
        {
            SetGemWeaponEquipCheck();

            UIWeaponGemSeletePopup weapongemseletepopup = ParentGO.GetComponent<UIWeaponGemSeletePopup>();
            if (weapongemseletepopup == null)
                return;
            if (weapongemseletepopup.SeleteGemUID == _gemdata.GemUID)
            {
                kSelSpr.gameObject.SetActive(true);
                kInactiveSpr.gameObject.SetActive(true);
            }
        }
        else if (_postype == ePosType.Gem_LevelUpMatList)
        {
            UIGemLevelUpPopup gemleveluppopup = ParentGO.GetComponent<UIGemLevelUpPopup>();
            if (gemleveluppopup == null)
                return;

            var matitem = gemleveluppopup.MatGemList.Find(x => x.GemUID == _gemdata.GemUID);
            if (matitem != null)
            {
                kSelSpr.gameObject.SetActive(true);
                kInactiveSpr.gameObject.SetActive(true);
            }
        }

    }

  
    public void UpdateSlot(ePosType postype, int index, GameTable.Gem.Param tablegemdata)
    {
        Init();
        _postype = postype;
        _index = index;
        _tablegemdata = tablegemdata;
        if (_tablegemdata == null)
        {
            SetBinSlot(2);
            return;
        }

        kIconSpr.gameObject.SetActive(true);
        kFrmGradeSpr.gameObject.SetActive(true);
        kGradeSpr.gameObject.SetActive(true);
        kBGSpr.spriteName = "itembgSlot_" + _tablegemdata.Grade.ToString();
        kIconSpr.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + _tablegemdata.Icon);
        kFrmGradeSpr.spriteName = "itemfrm_" + _tablegemdata.Grade.ToString();
        kGradeSpr.spriteName = "itemgrade_S_" + _tablegemdata.Grade.ToString();
        kGradeSpr.MakePixelPerfect();

        kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), 1);

    }

    //---------------------------------------------------------------------------------------------------------------------------------
    //
    //  유저 아이콘
    //
    //---------------------------------------------------------------------------------------------------------------------------------

    public void UpdateSlot(ePosType postype, int index, GameTable.UserMark.Param tableUsermarkData)
    {
        Init();
        _postype = postype;
        _index = index;
        _tableUserMarkData = tableUsermarkData;

        if(_tableUserMarkData == null)
        {
            SetBinSlot(0);
            return;
        }

        kIconSpr.gameObject.SetActive(true);
        kFrmGradeSpr.gameObject.SetActive(true);
        kBGSpr.gameObject.SetActive(true);
        kGradeSpr.gameObject.SetActive(false);

        kBGSpr.spriteName = "goodsbgSlot_1";
        kFrmGradeSpr.spriteName = "goodsfrm_1";

        LobbyUIManager.Instance.GetUserMarkIcon(ParentGO, this.gameObject, _tableUserMarkData.ID, ref kIconSpr);

        bool useMark = GameInfo.Instance.UserMarkList.Contains(_tableUserMarkData.ID);
        if (useMark)
        {
            kInactiveSpr.gameObject.SetActive(true);
        }

        kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), 1);
    }

    //---------------------------------------------------------------------------------------------------------------------------------
    //
    //  문양
    //
    //---------------------------------------------------------------------------------------------------------------------------------

    public void UpdateSlot(ePosType postype, int index, BadgeData badgedata)
    {
        Init();
        _postype = postype;
        _index = index;
        _badgedata = badgedata;

        if (_badgedata == null)
        {
            SetBinSlot(2);
            return;

        }

        kIconSpr.gameObject.SetActive(true);
        kFrmGradeSpr.gameObject.SetActive(true);
        kBGSpr.gameObject.SetActive(true);
        kGradeSpr.gameObject.SetActive(false);

        kBGSpr.spriteName = "goodsbgSlot_1";
        kFrmGradeSpr.spriteName = "goodsfrm_1";

        kIconSpr.mainTexture = GameSupport.GetBadgeIcon(badgedata);
        
        kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), _badgedata.Level);

        if (_badgedata.Lock == true)
            kLockSpr.gameObject.SetActive(true);
        else
            kLockSpr.gameObject.SetActive(false);

        if(_postype == ePosType.Item_Inven)
        {
            if (_badgedata.New)
                SetActiveNew(true);
        }

        if (ParentGO == null)
            return;

        if(_postype == ePosType.Item_Inven)
        {
            SetItemInvenSelete(_badgedata.BadgeUID);
            SetArenaBadgeEquipCheck();

            UIItemPanel itempanel = ParentGO.GetComponent<UIItemPanel>();
            if(itempanel != null && itempanel.IsSellPopup == true)
            {
                if (_badgedata.PosKind == (int)eContentsPosKind.ARENA || _badgedata.PosKind == (int)eContentsPosKind.ARENA_TOWER || _badgedata.Lock)
                    kInactiveSpr.gameObject.SetActive(true);
                else
                    kInactiveSpr.gameObject.SetActive(false);
            }

            SetSortLabelItemPanel(itempanel, _badgedata);
        }

    }

    public void UpdateSlot(ePosType postype, int index, int productIndex, GameTable.BadgeOpt.Param tableBadgeData)
    {
        Init();
        _postype = postype;
        _index = index;
        _tableBadgeOptData = tableBadgeData;

        _badgeOptionID = productIndex;

        if (_tableBadgeOptData == null)
        {
            if(productIndex == 0)
            {
                kIconSpr.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/Badge_0.png");
            }
            else
            {
                SetBinSlot(0);
                return;
            }
        }
        else
        {
            kIconSpr.mainTexture = GameSupport.GetBadgeIcon(_tableBadgeOptData);
        }

        kIconSpr.gameObject.SetActive(true);
        kFrmGradeSpr.gameObject.SetActive(true);
        kGradeSpr.gameObject.SetActive(false);
        kBGSpr.spriteName = "itembgSlot";
        
        kFrmGradeSpr.spriteName = "goodsfrm_1";

        kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), 1);
    }

    //---------------------------------------------------------------------------------------------------------------------------------
    //
    //  몬스터
    //
    //---------------------------------------------------------------------------------------------------------------------------------
    public void UpdateSlot(ePosType postype, int index, GameClientTable.BookMonster.Param tabledata)
    {
        Init();
        _postype = postype;
        _index = index;
        if (tabledata == null)
        {
            SetBinSlot(3);
            return;
        }

        kIconSpr.gameObject.SetActive(true);
        kFrmGradeSpr.gameObject.SetActive(true);
        kGradeSpr.gameObject.SetActive(false);
        kBGSpr.spriteName = "itembgSlot";
        kIconSpr.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Monster/" + tabledata.Icon);
        kFrmGradeSpr.spriteName = "itemfrm_" + tabledata.Grade.ToString();

        kCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GRAY_TEXT_COLOR), 1);
    }

    //---------------------------------------------------------------------------------------------------------------------------------
    //
    //  패키지 선택형
    //
    //---------------------------------------------------------------------------------------------------------------------------------
    public void UpdateSlot(ePosType postype, int index, GameTable.Random.Param tabledata, bool selected = false, bool isPossessItemShow = false)
    {
        Init();
        AtlasInit();
        
        _postype = postype;
        _index = index;

        _tablerandomdata = tabledata;

        if (kPossessItemLabel != null && isPossessItemShow)
        {
            kPossessItemLabel.SetActive(tabledata.ProductType == (int)eREWARDTYPE.ITEM);
            if (kPossessItemLabel.gameObject.activeSelf)
            {
                int myItemCount = GameInfo.Instance.GetItemIDCount(tabledata.ProductIndex);
                eTEXTID textId = myItemCount <= 0 ? eTEXTID.RED_TEXT_COLOR : eTEXTID.GREEN_TEXT_COLOR;
                kPossessItemLabel.textlocalize = FLocalizeString.Instance.GetText((int)textId, FLocalizeString.Instance.GetText(1833, myItemCount));
            }
        }

        RewardData rewardData = new RewardData(tabledata.ProductType, tabledata.ProductIndex, tabledata.ProductValue);
        GameSupport.UpdateSlotByRewardData(ParentGO, this.gameObject, rewardData, kFrmGradeSpr, kGradeSpr, kTypeSpr, kBGSpr, kCountLabel, kIconSpr, kItemGoodsSpr, null, kInactiveSpr);

        kSelSpr.gameObject.SetActive(selected);
    }

    public void UpdateSlot(ePosType postype, int index, RewardData rewarddata, bool selected = false)
    {
        Init();
        _postype = postype;
        _index = index;
        _rewarddata = rewarddata;

		if ( rewarddata == null ) {
			return;
		}

		GameSupport.UpdateSlotByRewardData(ParentGO, this.gameObject, rewarddata, kFrmGradeSpr, kGradeSpr, kTypeSpr, kBGSpr, kCountLabel, kIconSpr, kItemGoodsSpr, null, kInactiveSpr);
        kSelSpr.gameObject.SetActive(selected);
    }

	public void UpdateSlotRewardDataByCount( ePosType posType, int index, RewardData rewardData ) {
		Init();

		_postype = posType;
		_index = index;
		_rewarddata = rewardData;

		if ( rewardData == null ) {
			return;
		}

		GameSupport.UpdateSlotByRewardData( ParentGO, this.gameObject, rewardData, kFrmGradeSpr, kGradeSpr, kTypeSpr, kBGSpr, kCountLabel, kIconSpr, kItemGoodsSpr, null, kInactiveSpr );

		_NumberLabel.SetActive( true );
		_NumberLabel.text = FLocalizeString.Instance.GetText( 213, rewardData.Count );

		if ( 0 < rewardData.NewCount ) {
			_NumberTweenLabel.SetActive( true );
			_NumberTweenLabel.text = FLocalizeString.Instance.GetText( 213, rewardData.NewCount );

			if ( mNumberTweenPos == null ) {
				mNumberTweenPos = _NumberTweenLabel.GetComponent<TweenPosition>();
			}

			if ( mNumberTweenAlpha == null ) {
				mNumberTweenAlpha = _NumberTweenLabel.GetComponent<TweenAlpha>();
			}

			if ( mNumberTweenPos != null ) {
				mNumberTweenPos.ResetToBeginning();
				mNumberTweenPos.PlayForward();
			}

			if ( mNumberTweenAlpha != null ) {
				mNumberTweenAlpha.ResetToBeginning();
				mNumberTweenAlpha.PlayForward();
			}

			rewardData.NewCount = 0;
		}
	}

	public void UpdateSlot(GameTable.ArenaTower.Param param)
    {
        Init();
        _postype = ePosType.ArenaTower_RewardItem;
        _arenaTowerParam = param;
        if (_arenaTowerParam == null)
            return;

        RewardData rewardData = new RewardData(param.RewardType, param.RewardIndex, param.RewardValue);
        GameSupport.UpdateSlotByRewardData(ParentGO, this.gameObject, rewardData, kFrmGradeSpr, kGradeSpr, kTypeSpr, kBGSpr, kCountLabel, kIconSpr, kItemGoodsSpr, null);
    }



    //---------------------------------------------------------------------------------------------------------------------------------
    //
    //  생성
    //
    //---------------------------------------------------------------------------------------------------------------------------------
    private UIItemStateUnit CreateItemStateUnit()
    {
        GameObject res = ResourceMgr.Instance.LoadFromAssetBundle("ui", "UI/Prefab/ItemStateUnit.prefab") as GameObject;
        if (res == null)
            return null;
        GameObject instObj = Instantiate(res, this.gameObject.transform) as GameObject;
        if (instObj == null)
            return null;

        //instObj.transform.localPosition = new Vector3(40, -33, 0);

        return instObj.GetComponent<UIItemStateUnit>();
    }

    private GameObject CreateItemListSlotBatch()
    {
        GameObject res = ResourceMgr.Instance.LoadFromAssetBundle("ui", "UI/Prefab/ItemListSlotBatch.prefab") as GameObject;
        if (res == null)
            return null;
        GameObject instObj = Instantiate(res, this.gameObject.transform) as GameObject;
        if (instObj == null)
            return null;

        return instObj;
    }

    private GameObject CreateItemListSlotGradeEFF()
    {
        GameObject res = ResourceMgr.Instance.LoadFromAssetBundle("ui", "UI/Prefab/ItemListSlotGradeEFF.prefab") as GameObject;
        if (res == null)
            return null;
        GameObject instObj = Instantiate(res, this.gameObject.transform) as GameObject;
        if (instObj == null)
            return null;

        return instObj;
    }
    //---------------------------------------------------------------------------------------------------------------------------------



    //---------------------------------------------------------------------------------------------------------------------------------

    public void SetCountLabel( string text )
    {
        kCountLabel.transform.parent.gameObject.SetActive(true);
        kCountLabel.textlocalize = text;
    }

    public void SetActiveItemStateUnit( bool active )
    {
        if (_itemstateunit != null)
            _itemstateunit.gameObject.SetActive(active);
    }

    public void SetActiveNew(bool active)
    {
		if(kNewSpr == null)
		{
			return;
		}

        kNewSpr.gameObject.SetActive(active);
        if(active)
        {
            kNewSpr.spriteName = "item_new";
            kNewSpr.MakePixelPerfect();
        }        
    }

    public void SetActiveNotice(bool active)
    {
		if (kNewSpr == null)
		{
			return;
		}

		kNewSpr.gameObject.SetActive(active);
        if (active)
        {
            kNewSpr.spriteName = "frmNotice";
            kNewSpr.MakePixelPerfect();
        }
    }

    public void SetBinSlot(int type)
    {
        kBGSpr.spriteName = "itembgSlot_binitem";

        kIconSpr.gameObject.SetActive(false);
        kFrmGradeSpr.gameObject.SetActive(false);
        kGradeSpr.gameObject.SetActive(false);
        kCountLabel.textlocalize = "";

        kItemGoodsSpr.SetActive(false);
        kSelSpr.SetActive(false);
        kLockSpr.SetActive(false);
        kNewSpr.SetActive(false);
        kTypeSpr.SetActive(false);
        kWakeSpr.SetActive(false);
        kInactiveSpr.SetActive(false);
        kSortLabel.SetActive(false);
    }

    private void SetItemInvenSelete(long uid)
    {
        UIItemPanel itempanel = ParentGO.GetComponent<UIItemPanel>();
        if (itempanel == null)
            return;
        if (!itempanel.IsSellPopup && !itempanel.IsLockPopup && !itempanel.IsDecompopopup)
            return;
        long matitem = 0;
        if (itempanel.IsSellPopup)
            matitem = itempanel.SelItemList.Find(x => x == uid);
        else if(itempanel.IsDecompopopup)
            matitem = itempanel.DecompoItemList.Find(x => x == uid);

        if (matitem == 0)
            return;

        kSelSpr.gameObject.SetActive(true);
    }

    private void SetGemWeaponEquipCheck()
    {
        WeaponData _weapondata = GameInfo.Instance.GetEquipGemWeaponData(_gemdata.GemUID);
        if (_weapondata != null)
        {
            if (_itemstateunit == null)
                _itemstateunit = CreateItemStateUnit();
            if (_itemstateunit != null)
            {
                _itemstateunit.gameObject.SetActive(true);
                _itemstateunit.InitItemStateUnit(_weapondata, _gemdata);
            }
        }
    }

    //시설 배치중 체그
    private bool SetFaciliyWeaponEquipCheck()
    {
        FacilityData facilityWeaponData = GameInfo.Instance.GetEquipWeaponFacilityData(_weapondata.WeaponUID);
        if(facilityWeaponData != null)
        {
            if (_itemstateunit == null)
                _itemstateunit = CreateItemStateUnit();
            if (_itemstateunit != null)
            {
                _itemstateunit.gameObject.SetActive(true);
                _itemstateunit.InitItemStateUnit(facilityWeaponData);

                return false;
            }
        }

        return true;
    }

    private bool SetFaciliyCardEquipCheck()
    {
        FacilityData facilityCardData = GameInfo.Instance.GetEquiCardFacilityData(_carddata.CardUID);
        if (facilityCardData != null)
        {
            if (_itemstateunit == null)
                _itemstateunit = CreateItemStateUnit();
            if (_itemstateunit != null)
            {
                _itemstateunit.gameObject.SetActive(true);
                _itemstateunit.InitItemStateUnit(facilityCardData);

                return false;
            }
        }

        return true;
    }

    //문양 아레나 장착중 체크
    private bool SetArenaBadgeEquipCheck()
    {
        BadgeData badgedata = GameInfo.Instance.GetEquipBadgeData(_badgedata.BadgeUID);
        if(badgedata != null)
        {
            if (_itemstateunit == null)
                _itemstateunit = CreateItemStateUnit();
            if(_itemstateunit != null)
            {
                _itemstateunit.gameObject.SetActive(true);
                _itemstateunit.InitItemStateUnit(badgedata);
            }
        }

        return true;
    }

    //서포터 파견 장착중 체크
    private bool SetSupportDispatchCheck()
    {
        if (_carddata.PosKind != (int)eContentsPosKind.DISPATCH)
            return true;

        var dispatch = GameInfo.Instance.Dispatches.Find(x => x.TableID == _carddata.PosValue);
        if(dispatch != null)
        {
            if (_itemstateunit == null)
                _itemstateunit = CreateItemStateUnit();
            if (_itemstateunit != null)
            {
                _itemstateunit.gameObject.SetActive(true);
                _itemstateunit.InitItemStateUnit(dispatch);
            }
        }
        return true;
    }

    private bool SetArmoryWeaponEquipCheck()
    {
        if (GameInfo.Instance.WeaponArmoryData.ArmoryWeaponUIDList.Contains(_weapondata.WeaponUID))
        {
            if (_itemstateunit == null)
                _itemstateunit = CreateItemStateUnit();
            if (_itemstateunit != null)
            {
                _itemstateunit.gameObject.SetActive(true);
                _itemstateunit.InitArmoryStateUnit(_weapondata);

                return false;
            }
        }

        return true;
    }

    public void OnClick_Slot()
    {
        if (GameSupport.IsTutorial())
            return;

		UIGemOptChangePopup gemOptChangePopup = LobbyUIManager.Instance.GetActiveUI( "GemOptChangePopup" ) as UIGemOptChangePopup;
		if ( gemOptChangePopup != null ) {
			if ( gemOptChangePopup.IsAutoIng() ) {
				MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3367 ) );
				return;
			}
		}

		UIEventmodeStoryAutoGachaPopup eventmodeStoryAutoGachaPopup = LobbyUIManager.Instance.GetActiveUI( "EventmodeStoryAutoGachaPopup" ) as UIEventmodeStoryAutoGachaPopup;
		if ( eventmodeStoryAutoGachaPopup != null ) {
			if ( eventmodeStoryAutoGachaPopup.IsAutoIng() ) {
				MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3367 ) );
				return;
			}
		}

        if (LobbyUIManager.Instance.IsActiveUI("AutoGachaResultPopup")) {
            UIAutoGachaResultPopup autoGachaResultPopup = LobbyUIManager.Instance.GetActiveUI<UIAutoGachaResultPopup>("AutoGachaResultPopup");
            if (autoGachaResultPopup != null) {
                if (autoGachaResultPopup.mGachaType == UIAutoGachaResultPopup.eGachaType.ING)
                    return;
            }
        }

        if (LobbyUIManager.Instance.IsActiveUI("GachaResultPopup"))
        {
            UIGachaResultPopup gachaResultPopup = LobbyUIManager.Instance.GetActiveUI<UIGachaResultPopup>("GachaResultPopup");
            if (gachaResultPopup != null)
                gachaResultPopup.DesireEffectHide();
        }

        if (_postype == ePosType.Mat)
        {
            //GameSupport.OpenRewardDataInfoPopup(new RewardData((int)eREWARDTYPE.ITEM, _tabledata.ID, 0 ));
            GameSupport.OpenRewardTableDataInfoPopup(new RewardData((int)eREWARDTYPE.ITEM, _tabledata.ID, 0));
        }
        else if (_postype == ePosType.RewardDataInfo)
        {
            if (Director.IsPlaying)
                return;

            if (_rewarddata != null)
                GameSupport.OpenRewardDataInfoPopup(_rewarddata);
        }
        else if (_postype == ePosType.ItemInfo)
        {
            if (_tabledata == null)
            {
                return;
            }
            
            UIValue.Instance.SetValue(UIValue.EParamType.ItemUID, (long)-1);
            UIValue.Instance.SetValue(UIValue.EParamType.ItemTableID, _tabledata.ID);
            LobbyUIManager.Instance.ShowUI("ItemInfoPopup", true);
        }
		else if ( _postype == ePosType.REWARD_DATA_INFO_NOT_SELL ) {
            if ( _rewarddata == null ) {
                return;
            }

			GameSupport.OpenRewardTableDataInfoPopup( _rewarddata );
		}

		// 추후 요청이 있을 시 오픈될 기능
		//if (_postype == ePosType.Preset_Card)
		//{
		//    UIValue.Instance.SetValue(UIValue.EParamType.CharSelUID, _charUid);
		//    UIValue.Instance.SetValue(UIValue.EParamType.CharEquipCardSlot, _index);

		//    LobbyUIManager.Instance.ShowUI("CharCardSeletePopup", true);
		//}

		// 추후 요청이 있을 시 오픈될 기능
		//if (_postype == ePosType.Preset_Weapon)
		//{
		//    UIValue.Instance.SetValue(UIValue.EParamType.CharSelUID, _charUid);
		//    UIValue.Instance.SetValue(UIValue.EParamType.CharEquipWeaponSlot, (eWeaponSlot)_index);

		//    LobbyUIManager.Instance.ShowUI("CharWeaponSeletePopup", true);
		//}

		if (ParentGO == null)
            return;

        if (_postype == ePosType.Store || _postype == ePosType.ArenaStore )
        {
            UIStoreListSlotSM storelistslotsm = ParentGO.GetComponent<UIStoreListSlotSM>();
            if (storelistslotsm == null)
                return;
            if (storelistslotsm.StoreTable == null)
                return;

            GameSupport.OpenRewardDataInfoPopup(new RewardData(storelistslotsm.StoreTable.ProductType, storelistslotsm.StoreTable.ProductIndex, storelistslotsm.StoreTable.ProductValue));
        }
        if (_postype == ePosType.EquipCard)
        {
            UIValue.Instance.SetValue(UIValue.EParamType.CardUID, _carddata.CardUID);
            LobbyUIManager.Instance.ShowUI("CardInfoPopup", true);
        }
		else if( _postype == ePosType.RewardTable ) {
			UIStageDetailPopup stageDetailPopup = ParentGO.GetComponent<UIStageDetailPopup>();
			if( stageDetailPopup != null ) {
				if( _index >= 0 && _index < stageDetailPopup.DropItemList.Count ) {
					GameTable.Random.Param randomParam = stageDetailPopup.DropItemList[_index];
					GameSupport.OpenRewardDataInfoPopup( new RewardData( randomParam.ProductType, randomParam.ProductIndex, randomParam.ProductValue ) );
				}
			}
			else {
                UIRaidDetailPopup raidDetailPopup = ParentGO.GetComponent<UIRaidDetailPopup>();
                if( raidDetailPopup != null ) {
                    if( _index >= 0 && _index < raidDetailPopup.DropItemParamList.Count ) {
                        GameTable.Random.Param randomParam = raidDetailPopup.DropItemParamList[_index];
                        GameSupport.OpenRewardDataInfoPopup( new RewardData( randomParam.ProductType, randomParam.ProductIndex, randomParam.ProductValue ) );
                    }
                }
            }
		}

		else if (_postype == ePosType.Result)
        {
            if (0 <= _index && GameInfo.Instance.RewardList.Count > _index)
            {
                RewardData reward = GameInfo.Instance.RewardList[_index];
                GameSupport.OpenRewardTableDataInfoPopup(reward);
            }
        }
        else if (_postype == ePosType.Item_Inven)
        {
            UIItemPanel itempanel = ParentGO.GetComponent<UIItemPanel>();
            if (itempanel == null)
                return;
            if (_weapondata != null)
                itempanel.SeleteItem(_weapondata.WeaponUID, () =>
                {
                    if (this == null) return;
                    UpdateSlot(_postype, _index, _weapondata);
                });
            else if (_gemdata != null)
                itempanel.SeleteItem(_gemdata.GemUID);
            else if (_carddata != null)
                itempanel.SeleteItem(_carddata.CardUID, () =>
                {
                    if (this == null) return;
                    UpdateSlot(_postype, _index, _carddata);
                });
            else if (_itemdata != null)
                itempanel.SeleteItem(_itemdata.ItemUID);
            else if (_badgedata != null)
                itempanel.SeleteItem(_badgedata.BadgeUID);
        }
        else if (_postype == ePosType.Weapon_LevelUpMatList)
        {
            UIWeaponLevelUpPopup weaponleveluppopup = ParentGO.GetComponent<UIWeaponLevelUpPopup>();
            if (weaponleveluppopup == null)
                return;
            weaponleveluppopup.AddMatWeapon(_weapondata);
        }
        else if (_postype == ePosType.Weapon_LevelUpMatItemList)
        {
            UIWeaponLevelUpPopup weaponleveluppopup = ParentGO.GetComponent<UIWeaponLevelUpPopup>();
            if (weaponleveluppopup == null)
                return;
            weaponleveluppopup.AddMatItem(_itemdata);
        }
        else if (_postype == UIItemListSlot.ePosType.Weapon_SkillLevelUpMatList)
        {
            UIWeaponSkillLevelUpPopup weaponskillleveluppopup = ParentGO.GetComponent<UIWeaponSkillLevelUpPopup>();
            if (weaponskillleveluppopup == null)
                return;
            weaponskillleveluppopup.SeletMatWeapon(_weapondata);
        }
        else if (_postype == UIItemListSlot.ePosType.Weapon_SkillLevelUpMatItemList)
        {
            UIWeaponSkillLevelUpPopup weaponskillleveluppopup = ParentGO.GetComponent<UIWeaponSkillLevelUpPopup>();
            if (weaponskillleveluppopup == null)
                return;
            weaponskillleveluppopup.SeletMatItem(_itemdata);
        }
        else if (_postype == ePosType.Card_LevelUpMatList)
        {
            UICardLevelUpPopup cardleveluppopup = ParentGO.GetComponent<UICardLevelUpPopup>();
            if (cardleveluppopup == null)
                return;
            cardleveluppopup.AddMatCard(_carddata);
        }
        else if (_postype == ePosType.Card_LevelUpMatItemList)
        {
            UICardLevelUpPopup cardleveluppopup = ParentGO.GetComponent<UICardLevelUpPopup>();
            if (cardleveluppopup == null)
                return;
            cardleveluppopup.AddMatItem(_itemdata);
        }
        else if (_postype == UIItemListSlot.ePosType.Card_SkillLevelUpMatItemList)
        {
            UICardSkillLevelUpPopup cardskillleveluppopup = ParentGO.GetComponent<UICardSkillLevelUpPopup>();
            if (cardskillleveluppopup == null)
                return;
            cardskillleveluppopup.SeletMatItem(_itemdata);
        }
        else if (_postype == UIItemListSlot.ePosType.Card_SkillLevelUpMatList)
        {
            UICardSkillLevelUpPopup cardskillleveluppopup = ParentGO.GetComponent<UICardSkillLevelUpPopup>();
            if (cardskillleveluppopup == null)
                return;
            cardskillleveluppopup.SeletMatCard(_carddata);
        }
        else if (_postype == ePosType.Weapon_SeleteList)
        {
            UICharWeaponSeletePopup weaponseletepopup = ParentGO.GetComponent<UICharWeaponSeletePopup>();
            if (weaponseletepopup == null)
                return;
            weaponseletepopup.SetSeleteWeaponUID(_weapondata.WeaponUID);
        }
        else if (_postype == ePosType.Card_SeleteList)
        {
            UICharCardSeletePopup cardseletepopup = ParentGO.GetComponent<UICharCardSeletePopup>();
            if (cardseletepopup == null)
                return;
            cardseletepopup.SetSeleteCardUID(_carddata.CardUID);
        }
        else if (_postype == ePosType.Gem_SeleteList)
        {
            UIWeaponGemSeletePopup weapongemseletepopup = ParentGO.GetComponent<UIWeaponGemSeletePopup>();
            if (weapongemseletepopup == null)
                return;

            weapongemseletepopup.SetSeleteGemUID(_gemdata.GemUID);
        }
        else if (_postype == ePosType.Gem_LevelUpMatList)
        {
            UIGemLevelUpPopup gemleveluppopup = ParentGO.GetComponent<UIGemLevelUpPopup>();
            if (gemleveluppopup == null)
                return;
            gemleveluppopup.AddMatGem(_gemdata);
        }
        else if (_postype == ePosType.FacilityCard_SeleteList)
        {
            UIFacilityCardSeletePopup facilitycardseletepopup = ParentGO.GetComponent<UIFacilityCardSeletePopup>();
            if (facilitycardseletepopup == null)
                return;
            facilitycardseletepopup.SetSeleteCardUID(_carddata.CardUID);
        }
        else if (_postype == ePosType.FacilityWeapon_SeleteList)
        {
            UIFacilityWeaponSeletePopup facilityweaponseletepopup = ParentGO.GetComponent<UIFacilityWeaponSeletePopup>();
            if (facilityweaponseletepopup == null)
                return;
            facilityweaponseletepopup.SetSeleteWeaponUID(_weapondata.WeaponUID);
        }
        else if (_postype == ePosType.Select_Item)
        {
            UIChoiceitemPopup uIUIChoiceitemPopup = ParentGO.GetComponent<UIChoiceitemPopup>();
            if (uIUIChoiceitemPopup == null)
                return;

            if (kInactiveSpr.gameObject.activeSelf)
                return;

            uIUIChoiceitemPopup.SelectItem(_index, _tablerandomdata);
        }
        else if (_postype == ePosType.EquipWeaponSkin)
        {
            UIChoiceitemPopup uIUIChoiceitemPopup = ParentGO.GetComponent<UIChoiceitemPopup>();
            if (uIUIChoiceitemPopup == null)
                return;

            if (kInactiveSpr.gameObject.activeSelf)
            {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3155));
                return;
            }

            uIUIChoiceitemPopup.SelectSkin(_index, _bookdata);
        }
        else if (_postype == ePosType.EquipWeapon)
        {
            if( _weapondata == null ) {
                return;
			}

            Log.Show(_postype, Log.ColorType.Red);
            UIValue.Instance.SetValue(UIValue.EParamType.WeaponUID, _weapondata.WeaponUID);
            LobbyUIManager.Instance.ShowUI("WeaponInfoPopup", true);
        }
        else if (_postype == ePosType.ArenaReward)
        {
            //UIArenaGradeRewardSlot arenaGradeRewardSlot = ParentGO.GetComponent<UIArenaGradeRewardSlot>();
            //if (arenaGradeRewardSlot == null)
            //    return;
            if (_tablerandomdata == null)
                return;

            GameSupport.OpenRewardDataInfoPopup(new RewardData(_tablerandomdata.ProductType, _tablerandomdata.ProductIndex, _tablerandomdata.Value));
        }
        else if (_postype == ePosType.DispatchCard_SelectList)
        {
            UIDispatchCardSelectPopup popup = ParentGO.GetComponent<UIDispatchCardSelectPopup>();
            if (popup == null)
                return;
            popup.SetSelectCardUID(_carddata.CardUID);
        }
        else if (_postype == ePosType.DispatchCard_RewardItem)
        {
            // 파견 보상 UI
            if (_dispatchMissionParam == null)
                return;

            GameSupport.OpenRewardDataInfoPopup(new RewardData(_dispatchMissionParam.RewardType, _dispatchMissionParam.RewardIndex, _dispatchMissionParam.RewardValue));
        }
        else if (_postype == ePosType.ArmoryWeapon_SeleteList)
        {
            UIFacilityWeaponSeletePopup facilityweaponseletepopup = ParentGO.GetComponent<UIFacilityWeaponSeletePopup>();
            if (facilityweaponseletepopup == null)
                return;
            facilityweaponseletepopup.SetSeleteWeaponUID(_weapondata.WeaponUID);
        }
        else if (_postype == ePosType.Book)
        {
            if (LobbyUIManager.Instance.IsActiveUI("CardInfoPopup"))
                return;
            UIValue.Instance.SetValue(UIValue.EParamType.CardUID, (long)-1);
            UIValue.Instance.SetValue(UIValue.EParamType.CardTableID, _tablecarddata.ID);
            LobbyUIManager.Instance.ShowUI("CardInfoPopup", true);
        }
        else if (_postype == ePosType.ArenaTower_RewardItem)
        {
            // 아레나 타워 보상
            if (_arenaTowerParam == null)
                return;

            GameSupport.OpenRewardDataInfoPopup(new RewardData(_arenaTowerParam.RewardType, _arenaTowerParam.RewardIndex, _arenaTowerParam.RewardValue));
        }
        else if (_postype == ePosType.FacilityMaterial_Card)
        {
            UIFacilityCardSeletePopup p = (UIFacilityCardSeletePopup)LobbyUIManager.Instance.GetUI("FacilityCardSeletePopup");
            if (p == null) return;

            int CurGrade = FacilitySupport.GetCurTradeMaterialGrade(eFacilityTradeType.CARD);
            if (CurGrade > 0 && CurGrade != _carddata.TableData.Grade)
            {
                return;
            }

            UIFacilityPanel facilityPanel = LobbyUIManager.Instance.GetUI<UIFacilityPanel>("FacilityPanel");
            if (facilityPanel != null && facilityPanel.GradeExcessCheck(_carddata, ePosType.FacilityMaterial_Card))
            {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3275));
                return;
            }

            FacilitySupport.TardeToggleMaterial(_carddata.CardUID, p.Facilitydata);

            bool state = FacilitySupport.IsTradeSelectMaterial(_carddata.CardUID);
            kSelSpr.gameObject.SetActive(state);
            kInactiveSpr.gameObject.SetActive(state);
            p.Renewal(true);

        }
        else if (_postype == ePosType.FacilityMaterial_Weapon)
        {
            UIFacilityWeaponSeletePopup p = (UIFacilityWeaponSeletePopup)LobbyUIManager.Instance.GetUI("FacilityWeaponSeletePopup");
            if (p == null) return;

            int CurGrade = FacilitySupport.GetCurTradeMaterialGrade(eFacilityTradeType.WEAPON);
            if (CurGrade > 0 && CurGrade != _weapondata.TableData.Grade)
            {
                return;
            }

            UIFacilityPanel facilityPanel = LobbyUIManager.Instance.GetUI<UIFacilityPanel>("FacilityPanel");
            if (facilityPanel != null && facilityPanel.GradeExcessCheck(_weapondata, ePosType.FacilityMaterial_Weapon))
            {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3275));
                return;
            }

            FacilitySupport.TardeToggleMaterial(_weapondata.WeaponUID, p.Facilitydata);

            bool state = FacilitySupport.IsTradeSelectMaterial(_weapondata.WeaponUID);
            kSelSpr.gameObject.SetActive(state);
            kInactiveSpr.gameObject.SetActive(state);
            p.Renewal(true);
        }
        else if (_postype == ePosType.Char_FavorLevelUpMatList)
        {
            UICharGiveGiftPopup chargivegiftpopup = ParentGO.GetComponent<UICharGiveGiftPopup>();
            if (chargivegiftpopup == null) return;

            chargivegiftpopup.SetFavorMatItem(_itemdata);
        }
        else if (_postype == ePosType.Char_FavorPanelSelectItem)
        {
            UICharInfoTabCharFavorPanel charInfoTabCharFavorPanel = ParentGO.GetComponent<UICharInfoTabCharFavorPanel>();
            if (charInfoTabCharFavorPanel == null)
            {
                return;
            }
            
            UIValue.Instance.SetValue(UIValue.EParamType.ItemUID, (long)-1);
            UIValue.Instance.SetValue(UIValue.EParamType.ItemTableID, _rewarddata.Index);
            UIValue.Instance.SetValue(UIValue.EParamType.ItemTableCount, _rewarddata.Value);
            LobbyUIManager.Instance.ShowUI("ItemInfoPopup", true);
        }
        else if (_postype == ePosType.SpecialBuyPopup_ItemInfo)
        {
            if (_tablerandomdata.ProductType == (int)eREWARDTYPE.GOODS)
            {
                return;
            }
            
            RewardData rewardData = new RewardData(_tablerandomdata.ProductType, _tablerandomdata.ProductIndex,
                _tablerandomdata.Value);
            
            GameSupport.OpenRewardTableDataInfoPopup(rewardData);
        }
        else if (_postype == ePosType.Dye_MatItemSlot)
        {
            if (kInactiveSpr.gameObject.activeSelf)
            {
                return;
            }
            
            GameSupport.OpenRewardTableDataInfoPopup(new RewardData((int)eREWARDTYPE.ITEM, _tabledata.ID, 0));
        }
        else if (_postype == ePosType.Facility_Function_Select)
        {
            if (kSelSpr.gameObject.activeSelf)
            {
                return;
            }
            
            UIFacilityFunctionSelectPopup popup = ParentGO.GetComponent<UIFacilityFunctionSelectPopup>();
            if (popup == null)
            {
                return;
            }
            
            popup.OnClick_SelectItem(_tabledata);
        }
        else if( _postype == ePosType.RaidRecoverHpItem ) {
            UIRaidRecoveryItemPopup popup = ParentGO.GetComponent<UIRaidRecoveryItemPopup>();
            if( popup ) {
                popup.SelectItem( _index );
			}
		}
		else if ( _postype == ePosType.AP_BP_RECORVERY ) {
			UIRecoverySelectPopup uIRecoverySelectPopup = ParentGO.GetComponent<UIRecoverySelectPopup>();
			if ( uIRecoverySelectPopup == null ) {
				return;
			}

			uIRecoverySelectPopup.SetSelectItem( _itemdata.ItemUID, _itemdata.TableData.Value );
		}
	}

    public void OnClick_SubSlot(int index)
    {
        // 추후 요청이 있을 시 오픈될 기능
        //if (_postype == ePosType.Preset_Weapon)
        //{
        //    if (_weapondata == null)
        //    {
        //        return;
        //    }

        //    UIValue.Instance.SetValue(UIValue.EParamType.WeaponUID, _weapondata.WeaponUID);
        //    UIValue.Instance.SetValue(UIValue.EParamType.WeaponGemIndex, index);

        //    LobbyUIManager.Instance.ShowUI("WeaponGemSeletePopup", true);
        //}
    }

    public void SetSelectActive(bool active)
    {
        kSelSpr.SetActive(active);
    }

   
    private bool IsUsePressing()
    {
        
        if (_postype == ePosType.Weapon_SeleteList)
        {
            if (_weapondata == null)
                return false;
            return true;
        }
        else if (_postype == ePosType.Card_SeleteList)
        {
            if (_carddata == null)
                return false;

            return true;
        }
        else if (_postype == ePosType.Gem_SeleteList)
        {
            if (_gemdata == null)
                return false;
            return true;
        }

        return false;
    }
    public void OnPressStart()
    {
        if (GameSupport.IsTutorial())
            return;

        if (IsUsePressing())
            LobbyUIManager.Instance.kHoldGauge.Show(this.transform.position, 1.5f);
    }
    public void OnPressed()
    {
        LobbyUIManager.Instance.kHoldGauge.Hide();
    }
    public void OnPressing_Slot()
    {
        LobbyUIManager.Instance.kHoldGauge.Hide();

        if (ParentGO == null)
            return;

        Debug.Log("OnPressing_Slot");

        if (_postype == ePosType.Weapon_SeleteList)
        {
            if (_weapondata == null)
                return;
            UIValue.Instance.SetValue(UIValue.EParamType.WeaponUID, _weapondata.WeaponUID);
            LobbyUIManager.Instance.ShowUI("WeaponInfoPopup", true);
        }
        else if (_postype == ePosType.Card_SeleteList)
        {
            if (_carddata == null)
                return;

            UIValue.Instance.SetValue(UIValue.EParamType.CardUID, _carddata.CardUID);
            LobbyUIManager.Instance.ShowUI("CardInfoPopup", true);
        }
        else if (_postype == ePosType.Gem_SeleteList)
        {
            if (_gemdata == null)
                return;
            UIValue.Instance.SetValue(UIValue.EParamType.GemUID, _gemdata.GemUID);
            LobbyUIManager.Instance.ShowUI("GemInfoPopup", true);

            UIWeaponGemSeletePopup uIWeaponGemSeletePopup = LobbyUIManager.Instance.GetActiveUI<UIWeaponGemSeletePopup>("WeaponGemSeletePopup");
            if(uIWeaponGemSeletePopup != null)
            {
                uIWeaponGemSeletePopup.OnClick_BackBtn();
            }
        }
        /*
      else if (_postype == ePosType.RewardTable)
      {
          UIStageDetailPopup stagedetailpopup = ParentGO.GetComponent<UIStageDetailPopup>();
          if (stagedetailpopup == null)
              return;
          if (0 <= _index && stagedetailpopup.DropItemList.Count > _index)
          {
              GameTable.Random.Param dropdata = stagedetailpopup.DropItemList[_index];

              OpenRewardTableInfoPopup(dropdata.ProductType, dropdata.ProductIndex, dropdata.ProductValue);
          }
      }

      else if (_postype == ePosType.Result)
      {
          if (0 <= _index && GameInfo.Instance.RewardList.Count > _index)
          {
              RewardData reward = GameInfo.Instance.RewardList[_index];
              OpenRewardDataInfoPopup(reward);
          }
      }
      */
    }


   

   
    

    public void SetCountText( string text )
    {
        if(kCountLabel != null)
        {
            kCountLabel.textlocalize = text;
        }
    }

    private void SetLockSprite(bool isActive)
    {
        if(kLockSpr != null)
        {
            kLockSpr.gameObject.SetActive(isActive);
        }
    }

    /// <summary>
    /// 정렬시 특별하게 보여지는 텍스트
    /// </summary>
    public void SetSortLabelItemPanel(UIItemPanel itempanel, object obj)
    {
        kSortLabel.gameObject.SetActive(true);
        kSortLabel.pivot = UIWidget.Pivot.Center;
        kSortLabel.transform.localPosition = new Vector3(0.0f, -36.0f, 0.0f);
            
        switch (itempanel.TabType)
        {
            case UIItemPanel.eTabType.TabType_Weapon:
                {
                    WeaponData data = obj as WeaponData;
                    
                    if (itempanel.SortType == UIItemPanel.eSortType.Wake )
                        kSortLabel.textlocalize = string.Format("+{0}", data.Wake);
                    else if (itempanel.SortType == UIItemPanel.eSortType.SkillLevel )
                        kSortLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.SKILL_LEVEL_TXT_NOW_LV), data.SkillLv);
                    else if (itempanel.SortType == UIItemPanel.eSortType.MainStat)
                        kSortLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT),
                            GameSupport.GetWeaponATK(data.Level, data.Wake, data.SkillLv, data.EnchantLv, data.TableData));
                    else if (itempanel.SortType == UIItemPanel.eSortType.SubStat)
                        kSortLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT),
                            GameSupport.GetWeaponCRI(data.Level, data.Wake, data.SkillLv, data.EnchantLv, data.TableData));
                    else
                        kSortLabel.gameObject.SetActive(false);
                }
                break;
            case UIItemPanel.eTabType.TabType_Card:
                {
                    CardData data = obj as CardData;

                    if (itempanel.SortType == UIItemPanel.eSortType.Wake)
                        kSortLabel.textlocalize = string.Format("+{0}", data.Wake);
                    else if (itempanel.SortType == UIItemPanel.eSortType.SkillLevel)
                        kSortLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.SKILL_LEVEL_TXT_NOW_LV), data.SkillLv);
                    else if (itempanel.SortType == UIItemPanel.eSortType.MainStat)
                        kSortLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT),
                            GameSupport.GetCardHP(data.Level, data.Wake, data.SkillLv, data.EnchantLv, data.TableData));
                    else if (itempanel.SortType == UIItemPanel.eSortType.SubStat)
                        kSortLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT),
                            GameSupport.GetCardDEF(data.Level, data.Wake, data.SkillLv, data.EnchantLv, data.TableData));
                    else
                        kSortLabel.gameObject.SetActive(false);
                }
                break;
            case UIItemPanel.eTabType.TabType_Gem:
                {
                    GemData data = obj as GemData;

                    if (itempanel.SortType == UIItemPanel.eSortType.Wake)
                        kSortLabel.textlocalize = string.Format("+{0}", data.Wake);
                    else
                        kSortLabel.gameObject.SetActive(false);
                }
                break;
            case UIItemPanel.eTabType.TabType_Badge:
                {
                    BadgeData data = obj as BadgeData;
                    //if(itempanel.SortType == UIItemPanel.eSortType.Level)
                }
                break;
            default:
                kSortLabel.gameObject.SetActive(false);
                break;
        }
    }

    public void SetSortLabelString(string text)
    {
        kSortLabel.gameObject.SetActive(true);
        kSortLabel.pivot = UIWidget.Pivot.Center;
        kSortLabel.transform.localPosition = new Vector3(0.0f, -36.0f, 0.0f);
        kSortLabel.textlocalize = text;
    }

    public void SetSortLabelMatCount(int count)
    {
        kSortLabel.gameObject.SetActive(true);
        kSortLabel.pivot = UIWidget.Pivot.Center;
        kSortLabel.transform.localPosition = Vector3.zero;
        kSortLabel.textlocalize = string.Format("+{0}", count);
    }

    public void SetSortLabel(eContentsPosKind contentsPos, object obj, UIItemPanel.eSortType sort)
    {
        kSortLabel.gameObject.SetActive(true);
        kSortLabel.pivot = UIWidget.Pivot.Center;
        kSortLabel.transform.localPosition = new Vector3(0.0f, -36.0f, 0.0f);

        switch (contentsPos)
        {
            case eContentsPosKind.WEAPON:
                {
                    WeaponData data = obj as WeaponData;
                    if (sort == UIItemPanel.eSortType.Wake)
                        kSortLabel.textlocalize = string.Format("+{0}", data.Wake);
                    else if (sort == UIItemPanel.eSortType.SkillLevel)
                        kSortLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.SKILL_LEVEL_TXT_NOW_LV), data.SkillLv);
                    else if (sort == UIItemPanel.eSortType.MainStat)
                        kSortLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT),
                            GameSupport.GetWeaponATK(data.Level, data.Wake, data.SkillLv, data.EnchantLv, data.TableData));
                    else if (sort == UIItemPanel.eSortType.SubStat)
                        kSortLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT),
                            GameSupport.GetWeaponCRI(data.Level, data.Wake, data.SkillLv, data.EnchantLv, data.TableData));
                    else
                        kSortLabel.gameObject.SetActive(false);
                }
                break;
            case eContentsPosKind.CARD:
                {
                    CardData data = obj as CardData;
                    if (sort == UIItemPanel.eSortType.Wake)
                        kSortLabel.textlocalize = string.Format("+{0}", data.Wake);
                    else if (sort == UIItemPanel.eSortType.SkillLevel)
                        kSortLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.SKILL_LEVEL_TXT_NOW_LV), data.SkillLv);
                    else if (sort == UIItemPanel.eSortType.MainStat)
                        kSortLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT),
                            GameSupport.GetCardHP(data.Level, data.Wake, data.SkillLv, data.EnchantLv, data.TableData));
                    else if (sort == UIItemPanel.eSortType.SubStat)
                        kSortLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT),
                            GameSupport.GetCardDEF(data.Level, data.Wake, data.SkillLv, data.EnchantLv, data.TableData));
                    else
                        kSortLabel.gameObject.SetActive(false);
                }
                break;
            case eContentsPosKind.GEM:
                {
                    GemData data = obj as GemData;
                    if (sort == UIItemPanel.eSortType.Wake)
                        kSortLabel.textlocalize = string.Format("+{0}", data.Wake);
                    else
                        kSortLabel.gameObject.SetActive(false);
                }
                break;
            default:
                kSortLabel.gameObject.SetActive(false);
                break;
        }
    }

    //서포터 장착 조건이 걸린 경우 호출
    public void SetCardTypeFlag(eSTAGE_CONDI cardCondiType, bool cardContiFlag)
    {
        if (kCardTypeSpr != null)
        {
            kCardTypeSpr.gameObject.SetActive(true);
            kCardTypeSpr.spriteName = GameSupport.GetCardTypeSpriteName(cardCondiType);
        }

        if (kShortFallSpr != null)
        {
            kShortFallSpr.gameObject.SetActive(!cardContiFlag);
        }

        kInactiveSpr.gameObject.SetActive(!cardContiFlag);
    }

    public void SetHeart(int number, bool bEnable)
    {
        kHeartSpr.gameObject.SetActive(bEnable);
        kHeartLabel.textlocalize = number.ToString();
    }
    
    private void RepositionEnchantLevel(int grade)
    {
        if (grade <= 0) return;
        if ((int)eGRADE.COUNT <= grade) return;

        Vector3 pos = Vector3.zero;
        pos.y = 64;
        switch ((eGRADE)grade)
        {
            case eGRADE.GRADE_UR:
            case eGRADE.GRADE_SR:
                pos.x = 6;
                break;
            case eGRADE.GRADE_R:
            case eGRADE.GRADE_N:
                pos.x = -22;
                break;
        }
        kEnchantLvLabel.gameObject.transform.localPosition = pos;
    }
}
 