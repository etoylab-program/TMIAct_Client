using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eChoicePopupType
{
    NONE = 0,
    SELECT_ITEM = 1,
    SELECT_WEAPON_SKIN = 2,
}

public class UIChoiceitemPopup : FComponent
{
    private long _itemUid;
    private ItemData _itemData = null;
    private GameTable.Item.Param _itemTableData;
    private List<GameTable.Random.Param> _packageInItems;

    public UIButton kSelectBtn;
    [SerializeField] private FList _ChoiceItemList;

    [SerializeField] private GameObject RunCntObj;
    [SerializeField] private UILabel    LbCount;

    private int _selectedIdx;
    private GameTable.Random.Param _selectedData = null;
    private List<CardBookData> _precardbooklist = new List<CardBookData>();

    private eChoicePopupType _choiceType = eChoicePopupType.NONE;

    //무기 스킨 선택용
    private CharData _charData;
    private WeaponBookData _weaponbookdata;
    private List<GameClientTable.Book.Param> _havebooklist = new List<GameClientTable.Book.Param>();
    private eWeaponSlot _weaponSlotType;
    private GameClientTable.Book.Param _bookdata;

    private int mCount      = 1;
    private int mMaxCount   = 1;


    public override void Awake()
	{
		base.Awake();

        if (this._ChoiceItemList == null) return;

        this._ChoiceItemList.EventUpdate = this._UpdateChoiceItemListSlot;
        
        this._ChoiceItemList.EventGetItemCount = this._GetChoiceItemElementCount;
        this._ChoiceItemList.InitBottomFixing();
    }
 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}

	public override void InitComponent() {
		_itemTableData = null;

		_selectedIdx = -1;
		_selectedData = null;

		object choiceType = UIValue.Instance.GetValue( UIValue.EParamType.ChoicePopupType );
        if( choiceType != null ) {
            _choiceType = (eChoicePopupType)choiceType;
        }

		mMaxCount = 1;
		SetCountLabel( 1 );
		RunCntObj.SetActive( false );

		switch( _choiceType ) {
			case eChoicePopupType.SELECT_ITEM: {
				_itemUid = (long)UIValue.Instance.GetValue( UIValue.EParamType.ItemUID );
				if( _itemUid == -1 ) {
					_itemData = GameInfo.Instance.GetItemData( (int)UIValue.Instance.GetValue( UIValue.EParamType.ItemTableID ) );
					_itemTableData = GameInfo.Instance.GameTable.FindItem( (int)UIValue.Instance.GetValue( UIValue.EParamType.ItemTableID ) );

				}
				else {
					//아이템을 보유 중일땐 사용 가능
					_itemData = GameInfo.Instance.GetItemData( _itemUid );
					_itemTableData = _itemData.TableData;

					if( _itemData.Count > 1 ) {
						RunCntObj.SetActive( true );
                        if( _itemData.Count < GameInfo.Instance.GameConfig.MatCount ) {
                            mMaxCount = _itemData.Count;
                        }
                        else {
                            mMaxCount = GameInfo.Instance.GameConfig.MatCount;
                        }
					}
				}

                if( _itemTableData == null ) {
                    return;
                }

				_packageInItems = GameInfo.Instance.GameTable.Randoms.FindAll( x => x.GroupID == _itemTableData.Value );
				this._ChoiceItemList.UpdateList();
			}
			break;

			case eChoicePopupType.SELECT_WEAPON_SKIN: {
				var slotType = UIValue.Instance.GetValue(UIValue.EParamType.CharEquipWeaponSlot);
				if( slotType == null ) {
					Log.Show( "#### Weapon Equip Failed ... ####", Log.ColorType.Red );
					return;
				}

				_weaponSlotType = (eWeaponSlot)slotType;

				long uid = (long)UIValue.Instance.GetValue(UIValue.EParamType.CharSelUID);
				_charData = GameInfo.Instance.GetCharData( uid );

                if( _charData == null ) {
                    return;
                }

				_havebooklist.Clear();
				List <GameClientTable.Book.Param> tempbooklist = GameInfo.Instance.GameClientTable.FindAllBook(x => x.Group == (int)eBookGroup.Weapon);

                string[] split = null;
				for( int i = 0; i < tempbooklist.Count; i++ ) {
					WeaponBookData bookdata = GameInfo.Instance.WeaponBookList.Find(x => x.TableID == tempbooklist[i].ItemID);
                    if( bookdata == null ) {
                        continue;
                    }

					GameTable.Weapon.Param tempWeapon = GameInfo.Instance.GameTable.FindWeapon(tempbooklist[i].ItemID);
                    split = Utility.Split( tempWeapon.CharacterID, ',' );

                    for( int j = 0; j < split.Length; j++ ) {
                        if( Utility.SafeIntParse( split[j] ) == _charData.TableID ) {
                            _havebooklist.Add( tempbooklist[i] );
                        }
                    }
				}

				this._ChoiceItemList.UpdateList();
			}
			break;
	
            default: {
				Log.Show( "정의 되지 않은 타입입니다.", Log.ColorType.Red );
			}
			break;
		}
	}

	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        if (_itemTableData == null)
            return;

        kSelectBtn.isEnabled = _selectedIdx != -1;
        _ChoiceItemList.RefreshNotMove();   
    }
 	
	public void OnClick_BackBtn()
	{
        OnClickClose();
	}
	
	public void OnClick_ChangeBtn()
	{
        if (_selectedIdx == -1)
            return;

        switch(_choiceType)
        {
            case eChoicePopupType.SELECT_ITEM:
                {
                    RewardData rewardData = new RewardData(_selectedData.ProductType, _selectedData.ProductIndex, _selectedData.ProductValue * mCount);
                    string itemName = GameSupport.GetProductName(rewardData);
                    MessagePopup.CYN(eTEXTID.TITLE_NOTICE, string.Format(FLocalizeString.Instance.GetText(116), itemName), eTEXTID.YES, eTEXTID.NO, SelectItemOpen, null);
                }
                break;
            case eChoicePopupType.SELECT_WEAPON_SKIN:
                {
                    string itemName = string.Empty;
                    GameTable.Weapon.Param tableData = GameInfo.Instance.GameTable.FindWeapon(_bookdata.ItemID);
                    if (tableData != null)
                    {
                        int prevSkinTID = 0;
                        if(_weaponSlotType == eWeaponSlot.MAIN)
                        {
                            prevSkinTID = _charData.EquipWeaponSkinTID;
                        }
                        else if(_weaponSlotType == eWeaponSlot.SUB)
                        {
                            prevSkinTID = _charData.EquipWeapon2SkinTID;
                        }

                        //같은 스킨 장착시 같은무기 장착할때 처럼 팝업창 off
                        if(prevSkinTID.Equals(_bookdata.ItemID))
                        {
                            OnClickClose();
                            return;
                        }
                        itemName = FLocalizeString.Instance.GetText(tableData.Name);
                    }
                    SelectWeaponSkin();
                    //MessagePopup.CYN(eTEXTID.TITLE_NOTICE, string.Format(FLocalizeString.Instance.GetText(116), itemName), eTEXTID.YES, eTEXTID.NO, SelectWeaponSkin, null);
                }
                break;
        }
        
    }

    public void SelectItemOpen()
    {
        _precardbooklist.Clear();
        _precardbooklist.AddRange(GameInfo.Instance.CardBookList);

        GameInfo.Instance.Send_ReqUseItem(_itemUid, mCount, _selectedData.Value, OnNetEventSelectItemOpen);
    }

    public void SelectWeaponSkin()
    {
        GameInfo.Instance.Send_ReqEquipWeaponChar(_charData.CUID, _charData.CostumeColor, _charData.CostumeStateFlag, _charData.EquipWeaponUID, _charData.EquipWeapon2UID,
            (_weaponSlotType == eWeaponSlot.MAIN) ? _bookdata.ItemID : _charData.EquipWeaponSkinTID, (_weaponSlotType == eWeaponSlot.SUB) ? _bookdata.ItemID : _charData.EquipWeapon2SkinTID, OnNetEventSelectSkin);
    }

    public void OnNetEventSelectItemOpen(int _result, PktMsgType pktMsg)
    {
        if (_result != 0)
            return;

        for (int i = 0; i < GameInfo.Instance.RewardList.Count; i++)
        {
            if (GameInfo.Instance.RewardList[i].Type == (int)eREWARDTYPE.CHAR)
            {
                DirectorUIManager.Instance.PlayCharBuy(GameInfo.Instance.RewardList[i].Index, OnDirectorUIPlayCharBuy);
                return;
            }
        }

        OnDirectorUIPlayCharBuy();
    }

    public void OnDirectorUIPlayCharBuy()
    {
        MessageRewardListPopup.RewardListMessage(FLocalizeString.Instance.GetText((int)eTEXTID.REWARD_POPUP_TITLE), FLocalizeString.Instance.GetText((int)eTEXTID.REWARD_POPUP_TEXT), GameInfo.Instance.RewardList, OnMessageRewardCallBack);

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.InitComponent("ItemPanel");

        LobbyUIManager.Instance.HideUI("ItemPackageInfoPopup", false);
        OnClickClose();
    }

    public void OnMessageRewardCallBack()
    {
        DirectorUIManager.Instance.PlayNewCardGreeings(_precardbooklist);
    }

    public void OnNetEventSelectSkin(int _result, PktMsgType pktMsg)
    {
        if (_result != 0)
            return;

		RenderTargetChar.Instance.SetCostumeWeapon( _charData, !_charData.IsHideWeapon );
		LobbyUIManager.Instance.Renewal("CharInfoPanel");
        OnClickClose();
    }

    private void _UpdateChoiceItemListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIItemListSlot slot = slotObject.GetComponent<UIItemListSlot>();
            if (slot == null) break;
            
            if(_choiceType == eChoicePopupType.SELECT_ITEM)
            {
                if (index > _packageInItems.Count) break;

                slot.UpdateSlot(UIItemListSlot.ePosType.Select_Item, index, _packageInItems[index], _selectedIdx == index, true);

            }
            else if(_choiceType == eChoicePopupType.SELECT_WEAPON_SKIN)
            {
                if (index > _havebooklist.Count) break;

                slot.UpdateSlot(UIItemListSlot.ePosType.EquipWeaponSkin, index, _havebooklist[index], _selectedIdx == index);
            }

            slot.ParentGO = this.gameObject;

        } while (false);
    }

    private int _GetChoiceItemElementCount()
    {
        if(_choiceType == eChoicePopupType.SELECT_ITEM)
        {
            if (_packageInItems == null)
                return 0;
            return _packageInItems.Count; //TempValue
        }
        else if(_choiceType == eChoicePopupType.SELECT_WEAPON_SKIN)
        {
            if (_havebooklist == null)
                return 0;

            return _havebooklist.Count;
        }
        else
        {
            return 0;
        }
    }

    public void SelectItem(int index, GameTable.Random.Param data)
    {
        if(_selectedIdx == index)
        {
            _selectedIdx = -1;
            _selectedData = null;
        }
        else
        {
            _selectedIdx = index;
            _selectedData = data;
        }

        Log.Show("SelectIdx : " + _selectedIdx, Log.ColorType.Red);

        this._ChoiceItemList.RefreshNotMove();
        kSelectBtn.isEnabled = _selectedIdx != -1;
    }

    public void SelectSkin(int index, GameClientTable.Book.Param data)
    {
        if (_selectedIdx == index)
        {
            _selectedIdx = -1;
            _bookdata = null;
        }
        else
        {
            _selectedIdx = index;
            _bookdata = data;
        }
        Log.Show("SelectIdx : " + _selectedIdx, Log.ColorType.Red);

        this._ChoiceItemList.RefreshNotMove();
        kSelectBtn.isEnabled = _selectedIdx != -1;
    }

    public void OnBtnMinus()
    {
        SetCountLabel(mCount - 1);
    }

    public void OnBtnPlus()
    {
        SetCountLabel(mCount + 1);
    }

    public void OnBtnMax()
    {
        SetCountLabel(mMaxCount);
    }

    private void SetCountLabel(int count)
    {
        mCount = Mathf.Clamp(count, 1, mMaxCount);
        LbCount.textlocalize = mCount.ToString();
    }
}
