using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eBtnType = eFacilityFunctionSelectPopupButtonType;

public class UIFacilityFunctionSelectPopup : FComponent
{
    [Header("FacilityFunctionSelectPopup")]
    [SerializeField] private UIItemListSlot rewardItemListSlot;
    [SerializeField] private FList itemList;
    [SerializeField] private GameObject functionNameSprObj;
    [SerializeField] private UILabel functionNameSprLabel;
    [SerializeField] private UILabel functionInfoLabel;
    [SerializeField] private List<GameObject> clearBtnList;

    private int _selectType;
    private List<GameTable.FacilityTradeAddon.Param> _addOnParamList;
    private GameTable.Item.Param _itemParam;

    public override void Awake()
    {
        base.Awake();
        
        itemList.EventUpdate = _UpdateList;
        itemList.EventGetItemCount = _GetItemList;
        itemList.InitBottomFixing();
    }

    public override void OnEnable()
    {
		_addOnParamList = GameInfo.Instance.GameTable.FacilityTradeAddons.FindAll( x => x.AddType == 0 || x.AddType == _selectType );
		itemList.UpdateList();

		base.OnEnable();
    }

    public override void Renewal(bool bChildren = false)
    {
        base.Renewal(bChildren);
        
        itemList.RefreshNotMove();
        
        rewardItemListSlot.UpdateSlot(UIItemListSlot.ePosType.ItemInfo, 0, _itemParam);
        
        functionNameSprObj.SetActive(_itemParam != null);
        if (_itemParam != null)
        {
            string rewardItemCount = string.Empty;
            string functionName = string.Empty;
            string functionDesc = string.Empty;
            GameTable.FacilityTradeAddon.Param tParam = _addOnParamList.Find(x => x.AddItemID == _itemParam.ID);
            if (tParam != null)
            {
                ItemData itemData = GameInfo.Instance.ItemList.Find(x => x.TableID == _itemParam.ID);
                int itemCount = itemData?.Count ?? 0;
                int needCount = tParam.AddItemCount;
                eTEXTID textColorId = itemCount < needCount ? eTEXTID.RED_TEXT_COLOR : eTEXTID.WHITE_TEXT_COLOR;
                string countStr = FLocalizeString.Instance.GetText(276, itemCount, needCount);
                rewardItemCount = FLocalizeString.Instance.GetText((int)textColorId, countStr);

                functionName = FLocalizeString.Instance.GetText(tParam.Name);
                string addValue = string.Empty;
                switch (tParam.AddFuncType)
                {
                    case (int)eTRADE_ADDON_TYPE.PROBABILITY_UP:
                        addValue = tParam.AddFuncValue.ToString();
                        break;
                    case (int)eTRADE_ADDON_TYPE.DECOMPOSITION:
                        break;
                    case (int)eTRADE_ADDON_TYPE.SUPPORT_TYPE_SET:
                        addValue = FLocalizeString.Instance.GetText(450 + tParam.AddFuncValue);
                        break;
                    case (int)eTRADE_ADDON_TYPE.WEAPON_CHAR_SET:
                        GameTable.Character.Param cParam = GameInfo.Instance.GameTable.FindCharacter(tParam.AddFuncValue);
                        if (cParam != null)
                        {
                            addValue = FLocalizeString.Instance.GetText(cParam.Name);
                        }
                        break;
                }
                functionDesc = FLocalizeString.Instance.GetText(tParam.Desc, addValue);
            }
            
            rewardItemListSlot.SetCountText(rewardItemCount);
            
            functionNameSprLabel.textlocalize = functionName;
            functionInfoLabel.textlocalize = functionDesc;
        }
        else
        {
            functionInfoLabel.textlocalize = FLocalizeString.Instance.GetText(3016);
        }
        
        _ButtonsActive(_itemParam == null ? eBtnType.Off : eBtnType.On);
    }

    public override void OnClickClose()
    {
        base.OnClickClose();
    }

    public void SetSelectType(int selectType)
    {
        _selectType = selectType;
    }

    public void SetSelectItemData(GameTable.Item.Param itemParam)
    {
        _itemParam = itemParam;
    }

    public void OnClick_SelectItem(GameTable.Item.Param itemParam)
    {
        SetSelectItemData(itemParam);
        
        Renewal();
    }

    public void OnClick_ApplyBtn()
    {
        GameTable.FacilityTradeAddon.Param param = null;
        UIFacilityPanel facilityPanel = LobbyUIManager.Instance.GetUI<UIFacilityPanel>("FacilityPanel");
        if (_itemParam != null)
        {
            ItemData itemData = GameInfo.Instance.ItemList.Find(x => x.TableID == _itemParam.ID);
            int itemCount = itemData?.Count ?? 0;
            param = _addOnParamList.Find(x => x.AddItemID == _itemParam.ID);
            if (param == null)
            {
                return;
            }

            if (itemCount < param.AddItemCount)
            {
                ItemBuyMessagePopup.ShowItemBuyPopup(_itemParam.ID, itemCount, param.AddItemCount);
                return;
            }

            if (facilityPanel != null && facilityPanel.GradeExcessCheck(param, UIItemListSlot.ePosType.None))
            {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3274));
                return;
            }
        }

        if (facilityPanel != null)
        {
            facilityPanel.SetAddOnItem(param);
        }
        
        LobbyUIManager.Instance.Renewal("FacilityPanel");
        OnClickClose();
    }

    public void OnClick_ClearBtn()
    {
        _itemParam = null;
        
        Renewal();
    }

    public void OnClick_HelpBtn()
    {
        LobbyUIManager.Instance.ShowUI("FacilityTradeDetailPopup", true);
    }

    private void _ButtonsActive(eBtnType type)
    {
        int typeIndex = (int)type;
        for (int i = 0; i < clearBtnList.Count; i++)
        {
            clearBtnList[i].SetActive(i == typeIndex);
        }
    }

    private void _UpdateList(int index, GameObject obj)
    {
        if (_addOnParamList.Count <= index)
        {
            return;
        }

        UIItemListSlot comp = obj.GetComponent<UIItemListSlot>();
        if (comp == null)
        {
            return;
        }

        comp.ParentGO = this.gameObject;
        bool bActive = false;
        if (_itemParam != null && index < _addOnParamList.Count)
        {
            bActive = _addOnParamList[index].AddItemID == _itemParam.ID;
        }
        comp.UpdateSlot(UIItemListSlot.ePosType.Facility_Function_Select, index, GameInfo.Instance.GameTable.FindItem(_addOnParamList[index].AddItemID));
        comp.SetSelectActive(bActive);
    }

    private int _GetItemList()
    {
		if(_addOnParamList == null)
		{
			return 0;
		}

        return _addOnParamList.Count;
    }
}
