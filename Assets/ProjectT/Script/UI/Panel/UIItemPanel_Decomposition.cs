using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class UIItemPanel : FComponent
{
    private List<long> _decompoitemlist = new List<long>();
    public List<long> DecompoItemList {  get { return _decompoitemlist; } }

    private List<RewardData> _decompoRewards = new List<RewardData>();
    //public List<RewardData> DecompoRewards { get { return _decompoRewards; } }

    private void UpdateDecompoRandomGroupID()
    {
        _decompoRewards.Clear();

        Action<int> ActionAddGroupID = (id) =>
        {   
            var Randoms = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == id);
            if (Randoms == null || Randoms.Count == 0)
                return;

            for (int i = 0; i < Randoms.Count; i++)
            {
                var p1 = Randoms[i];
                var p2 = _decompoRewards.Find(x => x.Type == p1.ProductType &&
                        x.Index == p1.ProductIndex);

                if (p2 != null)
                {
                    p2.Value += p1.ProductValue;                    
                }
                else
                    _decompoRewards.Add(new RewardData(p1.ProductType, p1.ProductIndex, p1.ProductValue));
            }
        };

        for (int i = 0; i < _decompoitemlist.Count; i++)
        {
            if (TabType == eTabType.TabType_Weapon)
            {
                WeaponData data = GameInfo.Instance.WeaponList.Find(x => x != null && x.WeaponUID == _decompoitemlist[i]);
                if (data == null) continue;
                ActionAddGroupID(data.TableData.Decomposition);
            }
            else if (TabType == eTabType.TabType_Card)
            {   
                CardData data = GameInfo.Instance.CardList.Find(x => x != null && x.CardUID == _decompoitemlist[i]);                
                if (data == null) continue;
                ActionAddGroupID(data.TableData.Decomposition);
            }
        }

        _decompoRewards.Sort((RewardData l, RewardData r) =>
            {
                if (l.eGrade > r.eGrade) return -1;
                else if (l.eGrade < r.eGrade) return 1;
                return 0;
            });

        kDecompoList.UpdateList();
    }

    private void SelectItemGradeDecompo(int index)
    {
        switch (TabType)
        {
            case eTabType.TabType_Weapon:
                {
                    foreach (WeaponData weaponData in _WeaponList)
                    {
                        if (_decompoitemlist.Count >= GameInfo.Instance.GameConfig.SellCount)
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

                        if (weaponData.Lock || weaponData.TableData.Decomposable == 0)
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

                        if (_decompoitemlist.Exists(r => r == weaponData.WeaponUID) == true) {
                            continue;
                        }

                        _decompoitemlist.Add(weaponData.WeaponUID);
                    }
                }
                break;
            case eTabType.TabType_Card:
                {
                    foreach (CardData cardData in _CardList)
                    {
                        if (_decompoitemlist.Count >= GameInfo.Instance.GameConfig.SellCount)
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

                        if (_decompoitemlist.Exists(r => r == cardData.CardUID) == true) {
                            continue;
                        }

                        _decompoitemlist.Add(cardData.CardUID);
                    }
                }
                break;
        }
    }

    private void DeselectItemGradeDecompo(int index)
    {
        List<long> removeList = new List<long>();
        foreach (long uid in _decompoitemlist)
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
            }
        }

        foreach (long uid in removeList)
        {
            _decompoitemlist.Remove(uid);
        }

        removeList.Clear();
    }

    private bool OnEventTabDecompoSelect(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Click)
        {
            if (kDecompoGradeFTab.kBtnList.Count <= nSelect)
            {
                return false;
            }

            bool isActive = false;
            Transform selSpr = kDecompoGradeFTab.kBtnList[nSelect].transform.Find("SelSpr");
            if (selSpr != null)
            {
                isActive = selSpr.gameObject.activeSelf;
            }

            if (isActive == false && GameInfo.Instance.GameConfig.SellCount <= _decompoitemlist.Count)
            {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3056));

                return false;
            }
        }

        return true;
    }

    private bool OnEventTabDecompoSelectActive(int nSelect, bool isActive)
    {
        if (isActive)
        {
            SelectItemGradeDecompo(nSelect);
        }
        else
        {
            DeselectItemGradeDecompo(nSelect);
        }

        UpdateDecompoRandomGroupID();

        kDecompoPopupEmpty.SetActive(_decompoitemlist.Count <= 0);
        kDecompoPopupSelete.SetActive(0 < _decompoitemlist.Count);

        ReflashDeomposition_CountLabel();

        _listItemInstance.RefreshNotMoveAllItem();

        return true;
    }

    public void ShowDecompoPopup()
    {
        _bdecompopopup = true;

        ReflashDecompoPopup();

        PlayAnimtion(4);
    }
    private void HideDecompoPopup()
    {
        _bdecompopopup = false;

        _decompoitemlist.Clear();        
        UpdateDecompoRandomGroupID();
        ReflashDecompoPopup();

        PlayAnimtion(5);

        HideItemMenu((int)eMenuType.MenuType_Info);
    }

	public void ReflashDecompoPopup() {
		_decompoitemlist.Clear();
		UpdateDecompoRandomGroupID();

		kDecompoPopup.SetActive( true );
		kDecompoPopupEmpty.SetActive( true );
		kDecompoPopupSelete.SetActive( false );

		kDecompoGradeFTab.gameObject.SetActive( kItemTab.kSelectTab == (int)eTabType.TabType_Weapon || kItemTab.kSelectTab == (int)eTabType.TabType_Card );
		kDecompoGradeFTab.DisableTab();

		ReflashDeomposition_CountLabel();

		//장착 또는 잠금 상태 아이템 비활성 처리
		this._listItemInstance.RefreshNotMove();
	}

	private void ReflashDeomposition_CountLabel()
    {
        if (kDecompoPopupSelete.activeSelf)
            FLocalizeString.SetLabel(kDecompoCountLabel, 276, _decompoitemlist.Count, GameInfo.Instance.GameConfig.SellCount);
    }

    public void OnClick_DecompoOK()
    {
        if (_decompoitemlist.Count <= 0) return;
        UIDecompositionPopup popup =  LobbyUIManager.Instance.GetUI<UIDecompositionPopup>("DecompositionPopup");
        if(popup != null)
        {
            popup.SetData(_decompoitemlist, _decompoRewards, TabType);
            LobbyUIManager.Instance.ShowUI("DecompositionPopup", true);
        }
    }

    public void OnClick_DecompoCancel()
    {
        HideDecompoPopup();
        this._listItemInstance.RefreshNotMove();
    }

    public void OnClick_AutoMat_Decomposition()
    {
        for (int i = 0; i < _isGradeTabEnableArray.Length; i++)
        {
            _isGradeTabEnableArray[i] = false;
        }

        _decompoitemlist.Clear();
        if (TabType == eTabType.TabType_Weapon)
        {
            foreach (WeaponData weaponData in _WeaponList)
            {
                if (_decompoitemlist.Count >= GameInfo.Instance.GameConfig.SellCount)
                {
                    break;
                }

                if (weaponData == null)
                {
                    continue;
                }

                if (weaponData.Lock || weaponData.TableData.Decomposable == 0)
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

                if (_isGradeTabEnableArray[weaponData.TableData.Grade - 1] == false)
                {
                    _isGradeTabEnableArray[weaponData.TableData.Grade - 1] = true;
                }

                _decompoitemlist.Add(weaponData.WeaponUID);
            }
        }
        else if (TabType == eTabType.TabType_Card)
        {
            foreach (CardData cardData in _CardList)
            {
                if (_decompoitemlist.Count >= GameInfo.Instance.GameConfig.SellCount)
                {
                    break;
                }

                if (cardData == null)
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

                CharData equipData = GameInfo.Instance.GetEquiCardCharData(cardData.CardUID);
                if (equipData != null)
                {
                    continue;
                }
                if (GameInfo.Instance.GetEquipCharFacilityData(cardData.CardUID) != null)
                {
                    continue;
                }

                if (_isGradeTabEnableArray[cardData.TableData.Grade - 1] == false)
                {
                    _isGradeTabEnableArray[cardData.TableData.Grade - 1] = true;
                }

                _decompoitemlist.Add(cardData.CardUID);
            }
        }

        UpdateDecompoRandomGroupID();

        kDecompoPopupEmpty.SetActive(_decompoitemlist.Count <= 0);
        kDecompoPopupSelete.SetActive(0 < _decompoitemlist.Count);

        ReflashDeomposition_CountLabel();

        _listItemInstance.RefreshNotMoveAllItem();

        if (kDecompoGradeFTab.gameObject.activeSelf)
        {
            for (int i = 0; i < _isGradeTabEnableArray.Length; i++)
            {
                kDecompoGradeFTab.EnableTab(i, _isGradeTabEnableArray[i]);
            }
        }
    }


    private void _UpdateDecompListSlot(int index, GameObject slotObject)
    {
        UIItemListSlot slot = slotObject.GetComponent<UIItemListSlot>();
        if (slot == null) return;

        switch (TabType)
        {
            case eTabType.TabType_Weapon:
            case eTabType.TabType_Card:
                slot.UpdateSlot(UIItemListSlot.ePosType.RewardDataInfo, index, _decompoRewards[index]);
                break;                
        }

    }
}
