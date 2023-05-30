using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFacilityTradeDetailPopup : FComponent
{
    public enum eTradeType
    {
        None = -1,
        Card,
        Weapon,
        LimitedCard,
        LimitedWeapon,
        Max,
    }
    
    [Header("FacilityTradeDetailPopup - Official")]
    [SerializeField] private FTab officialTab;
    [SerializeField] private FList exchangeFormulaList;
    [SerializeField] private FList exchangeAddFunctionList;
    
    [Header("FacilityTradeDetailPopup - List")]
    [SerializeField] private FList tabList;
    [SerializeField] private FList exchangeCardList;

    private FList _currentOfficialList;
    private eTradeType _eTradeType = eTradeType.None;
    private List<GameTable.Card.Param> _cardTableList;
    private List<GameTable.Card.Param> _cardLimitedTableList;
    private List<GameTable.Weapon.Param> _weaponTableList;
    private List<GameTable.Weapon.Param> _weaponLimitedTableList;
    private readonly List<string> _typeNameList = new List<string>();

    public override void Awake()
    {
        base.Awake();

        officialTab.EventCallBack = _OnTabOfficialSelect;

        tabList.EventUpdate = _UpdateTabList;
        tabList.EventGetItemCount = () => (int)eTradeType.Max;

        exchangeFormulaList.EventUpdate = _UpdateExchangeFormulaList;
        exchangeFormulaList.EventGetItemCount = () => GameInfo.Instance.GameTable.FacilityTrades.Count;
        exchangeFormulaList.InitBottomFixing();

        exchangeAddFunctionList.EventUpdate = _UpdateExchangeAddFunctionList;
        exchangeAddFunctionList.EventGetItemCount = () => GameInfo.Instance.GameClientTable.FacilityTradeHelps.Count;
        exchangeAddFunctionList.InitBottomFixing();

        exchangeCardList.EventUpdate = _UpdateExchangeCardList;
        exchangeCardList.EventGetItemCount = () =>
        {
            int result = 0;
            switch (_eTradeType)
            {
                case eTradeType.Card:
                    result = _cardTableList.Count;
                    break;
                case eTradeType.Weapon:
                    result = _weaponTableList.Count;
                    break;
                case eTradeType.LimitedCard:
                    result = _cardLimitedTableList.Count;
                    break;
                case eTradeType.LimitedWeapon:
                    result = _weaponLimitedTableList.Count;
                    break;
            }
            return result;
        };
        exchangeCardList.InitBottomFixing();
        
        _cardTableList = _GetCardList(1);
        _weaponTableList = _GetWeaponList(1);
        _cardLimitedTableList = _GetCardList(2);
        _weaponLimitedTableList = _GetWeaponList(2);
    }

    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
        _typeNameList.Clear();
        _typeNameList.Add(FLocalizeString.Instance.GetText(1022));
        _typeNameList.Add(FLocalizeString.Instance.GetText(1020));
        _typeNameList.Add(FLocalizeString.Instance.GetText(220, 
            FLocalizeString.Instance.GetText(1714), FLocalizeString.Instance.GetText(1022)));
        _typeNameList.Add(FLocalizeString.Instance.GetText(220, 
            FLocalizeString.Instance.GetText(1714), FLocalizeString.Instance.GetText(1020)));
        
        officialTab.SetTab(0, SelectEvent.Code);
        exchangeFormulaList.gameObject.SetActive(true);
        exchangeAddFunctionList.gameObject.SetActive(false);
        
        _eTradeType = eTradeType.Card;
        _currentOfficialList = exchangeFormulaList;

        tabList.UpdateList();
        _currentOfficialList.UpdateList();
        exchangeCardList.UpdateList();
    }

    private List<GameTable.Card.Param> _GetCardList(int findId)
    {
        List<GameTable.Card.Param> list = GameInfo.Instance.GameTable.Cards.FindAll(x => x.Tradeable == findId);
        list.Sort((left, right) =>
        {
            if (left.Grade > right.Grade)
            {
                return -1;
            }
            if (left.Grade < right.Grade)
            {
                return 1;
            }
            return 0;
        });

        return list;
    }

    private List<GameTable.Weapon.Param> _GetWeaponList(int findId)
    {
        List<GameTable.Weapon.Param> list = GameInfo.Instance.GameTable.Weapons.FindAll(x => x.Tradeable == findId);
        list.Sort((left, right) =>
        {
            if (left.Grade > right.Grade)
            {
                return -1;
            }
            if (left.Grade < right.Grade)
            {
                return 1;
            }
            return 0;
        });

        return list;
    }
    
    private bool _OnTabOfficialSelect(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Code)
        {
            return true;
        }
        
        if (type != SelectEvent.Click)
        {
            return false;
        }
        
        if (_currentOfficialList != null)
        {
            _currentOfficialList.gameObject.SetActive(false);
        }
        
        switch (nSelect)
        {
            case 0:
                _currentOfficialList = exchangeFormulaList;
                break;
            case 1:
                _currentOfficialList = exchangeAddFunctionList;
                break;
        }

        if (_currentOfficialList != null)
        {
            _currentOfficialList.gameObject.SetActive(true);
            _currentOfficialList.UpdateList();
        }

        return true;
    }

    private void _UpdateTabList(int index, GameObject slotObj)
    {
        UIHowToBeStrongListSlot slot = slotObj.GetComponent<UIHowToBeStrongListSlot>();
        if (slot == null)
        {
            return;
        }

        string titleStr = "";
        if (index < _typeNameList.Count)
        {
            titleStr = _typeNameList[index];
        }
        
        slot.ParentGO = this.gameObject;
        slot.UpdateSlot(index, (int)_eTradeType, titleStr);
        
        exchangeCardList.UpdateList();
    }

    private void _UpdateExchangeFormulaList(int index, GameObject slotObj)
    {
        UITradeFormulaListSlot slot = slotObj.GetComponent<UITradeFormulaListSlot>();
        if (slot == null)
        {
            return;
        }

        if (GameInfo.Instance.GameTable.FacilityTrades.Count <= index)
        {
            return;
        }

        slot.UpdateSlot(GameInfo.Instance.GameTable.FacilityTrades[index]);
    }

    private void _UpdateExchangeAddFunctionList(int index, GameObject slotObj)
    {
        UITradeFormulaListSlot slot = slotObj.GetComponent<UITradeFormulaListSlot>();
        if (slot == null)
        {
            return;
        }

        if (GameInfo.Instance.GameTable.FacilityTrades.Count <= index)
        {
            return;
        }
        
        slot.UpdateSlot(GameInfo.Instance.GameClientTable.FacilityTradeHelps[index]);
    }

    private void _UpdateExchangeCardList(int index, GameObject slotObj)
    {
        UITradableListSlot slot = slotObj.GetComponent<UITradableListSlot>();
        if (slot == null)
        {
            return;
        }
        
        switch (_eTradeType)
        {
            case eTradeType.Card:
            {
                if (_cardTableList.Count <= index)
                {
                    return;
                }
                slot.UpdateSlot(_cardTableList[index]);
            } break;
            case eTradeType.Weapon:
            {
                if (_weaponTableList.Count <= index)
                {
                    return;
                }
                slot.UpdateSlot(_weaponTableList[index]);
            } break;
            case eTradeType.LimitedCard:
            {
                if (_cardLimitedTableList.Count <= index)
                {
                    return;
                }
                slot.UpdateSlot(_cardLimitedTableList[index]);
            } break;
            case eTradeType.LimitedWeapon:
            {
                if (_weaponLimitedTableList.Count <= index)
                {
                    return;
                }
                slot.UpdateSlot(_weaponLimitedTableList[index]);
            } break;
        }
    }

    public void SelectTabList(int index)
    {
        _eTradeType = (eTradeType)index;
        
        tabList.RefreshNotMove();
    }
}
