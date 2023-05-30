using UnityEngine;
using System.Collections;

public class UIBatchListSlotSlot : FSlot
{
    public enum eBadgeSlotType
    {
        Info,           //랭킹 정보에서 사용
        Equip,          //문양 장착, 변경 가능(ArenaMainPanel 등에서 자신이 장착한 문양 보여줄때 사용)
        EquipSlot,      //문양 장착할 슬롯
        Slot,           //문양 인벤토리
    }

    public UISprite kPlusSpr;
	public UISprite kSocketSpr;
	public UITexture kBatchTex;
	public UILabel kBatchLevelLabel;
	public UISprite kSlotbgSpr;
	public UILabel kSlotLabel;
    public UISprite kRefreshSpr;
    public UISprite kSelSpr;
    public TweenScale kSelTweenScale;
    public TweenAlpha kSelTweenAlpha;

    public UISprite kFrmSpr;
    public UISprite kSelFrmSpr;

    public GameObject kSlotBGSpr;
    public UILabel kSlotNumLabel;

    public GameObject kLockObj;
    public UISprite kWarningSpr;

    private bool _selected = false;

    private BadgeData _badgeData;
    private eBadgeSlotType _slotType;
    private int _index;
    private GameTable.BadgeOpt.Param _baseBadgeTableData;
    private int _mainOptID = 0;

    private eContentsPosKind _contentsPosKind = eContentsPosKind._NONE_;

    private void Awake()
    {
        UIButton thisBtn = this.GetComponent<UIButton>();
        if (thisBtn != null)
            thisBtn.enabled = true;
    }

    public void UpdateSlot(int index, int mainOptID, BadgeData inBadgeData, eBadgeSlotType type, eContentsPosKind posKind, bool selected = false, long badgeUid = 0) 	//Fill parameter if you need
	{
        _selected = selected;
        _slotType = type;
        _badgeData = inBadgeData;
        _index = index;
        _mainOptID = mainOptID;
        _contentsPosKind = posKind;

        DefailtSettings();
        SetBadgeImg();
        switch (_slotType)
        {
            case eBadgeSlotType.Equip:
                {
                    SetBadgeSlotTypeWithEquip();
                }
                break;
            case eBadgeSlotType.EquipSlot:
                {
                    SetBadgeSlotTypeWithEquipSlot();
                    kSelFrmSpr.gameObject.SetActive(_selected);
                }
                break;
            case eBadgeSlotType.Info:
                {
                    SetBadgeSlotTypeWithInfo();
                    if (posKind == eContentsPosKind.PRESET)
                    {
                        kWarningSpr.SetActive(badgeUid != 0 && _badgeData == null);
                    }
                }
                break;
            case eBadgeSlotType.Slot:
                {
                    SetBadgeSlotTypeWithSlot();
                    if(_selected)
                    {
                        kSelFrmSpr.gameObject.SetActive(true);
                    }
                }
                break;
        }
	}

    private void DefailtSettings()
    {
        kPlusSpr.gameObject.SetActive(false);
        kFrmSpr.gameObject.SetActive(false);
        kSelFrmSpr.gameObject.SetActive(false);
        kRefreshSpr.gameObject.SetActive(false);
        kSlotBGSpr.gameObject.SetActive(false);
        kBatchTex.gameObject.SetActive(false);
        kBatchLevelLabel.gameObject.SetActive(false);
        kSelSpr.gameObject.SetActive(false);
        if (_badgeData != null)
        {
            _baseBadgeTableData = GameInfo.Instance.GameTable.FindBadgeOpt(x => x.OptionID == _badgeData.OptID[0]);
        }
        if (kWarningSpr != null)
        {
            kWarningSpr.SetActive(false);
        }
    }

    private void SetBadgeSlotTypeWithEquip()
    {
        kPlusSpr.gameObject.SetActive(true);
        kSlotbgSpr.gameObject.SetActive(true);
        kSlotNumLabel.textlocalize = _index.ToString();

        if (_badgeData != null)
            kRefreshSpr.gameObject.SetActive(true);
    }

    private void SetBadgeSlotTypeWithEquipSlot()
    {
        kSlotbgSpr.gameObject.SetActive(true);
        kSlotNumLabel.textlocalize = _index.ToString();

    }

    private void SetBadgeSlotTypeWithInfo()
    {
        kSlotbgSpr.gameObject.SetActive(true);
        kSlotNumLabel.textlocalize = _index.ToString();
    }

    private void SetBadgeSlotTypeWithSlot()
    {
        if(_badgeData.PosKind == (int)eContentsPosKind.ARENA)
        {
            kSlotbgSpr.gameObject.SetActive(true);
            kSlotNumLabel.textlocalize = (_badgeData.PosSlotNum + 1).ToString();
        }
        kFrmSpr.gameObject.SetActive(true);
    }

    private void SetBadgeImg()
    {
        if(_badgeData == null)
        {
            kBatchTex.gameObject.SetActive(false);
            kBatchLevelLabel.gameObject.SetActive(false);
        }
        else
        {
            kBatchTex.gameObject.SetActive(true);
            kBatchTex.mainTexture = GameSupport.GetBadgeIcon(_baseBadgeTableData);

            if(_badgeData.Level > (int)eCOUNT.NONE)
            {
                kBatchLevelLabel.gameObject.SetActive(true);
                kBatchLevelLabel.textlocalize = string.Format("+{0}", _badgeData.Level);
            }

            if (_mainOptID == _badgeData.OptID[(int)eBadgeOptSlot.FIRST])
            {
                kSelSpr.gameObject.SetActive(true);
                kSelTweenScale.ResetToBeginning();
                kSelTweenAlpha.ResetToBeginning();
            }
        }
    }

    public void OnClick_Slot()
	{
        switch (_slotType)
        {
            case eBadgeSlotType.Equip:
                {
                    if(GameInfo.Instance.BadgeList.Count <= 0)
                    {
                        //보유한 문양이 없습니다
                        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3160));
                        return;
                    }

                    //슬롯에 장착할 수 있는 문양이 없습니다.
                    if (_badgeData == null)
                    {
                        var EmptyBadge = GameInfo.Instance.BadgeList.FindAll(x => x.PosKind == 0);
                        if (EmptyBadge == null || EmptyBadge.Count == 0)
                        {   
                            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3227));
                            return;
                        }
                    }

                    UIValue.Instance.SetValue(UIValue.EParamType.BadgeSlotIndex, _index);
                    if(_badgeData == null)
                        UIValue.Instance.SetValue(UIValue.EParamType.BadgeUID, (long)0);
                    else
                        UIValue.Instance.SetValue(UIValue.EParamType.BadgeUID, (long)_badgeData.BadgeUID);

                    UIValue.Instance.SetValue(UIValue.EParamType.BadgeType, _contentsPosKind);
                    LobbyUIManager.Instance.ShowUI("BadgeSelectPopup", true);
                }
                break;
            case eBadgeSlotType.EquipSlot:
                {
                    UIValue.Instance.SetValue(UIValue.EParamType.BadgeType, _contentsPosKind);
                    UIBadgeSelectPopup badgeSelectPopup = LobbyUIManager.Instance.GetActiveUI<UIBadgeSelectPopup>("BadgeSelectPopup");
                    if(badgeSelectPopup != null)
                    {
                        badgeSelectPopup.OnClick_EquipBadgeSlot(_index);
                    }
                }
                break;
            case eBadgeSlotType.Info:
                {
                    if (_badgeData == null)
                        return;
                    if(ParentGO != null)
                    {
                        UIPvpAllRewardSlot uIPvpAllRewardSlot = ParentGO.GetComponent<UIPvpAllRewardSlot>();
                        if(uIPvpAllRewardSlot != null)
                        {
                            uIPvpAllRewardSlot.OnShowBadgeToolTip();
                        }
                    }
                }
                break;
            case eBadgeSlotType.Slot:
                {
                    UIValue.Instance.SetValue(UIValue.EParamType.BadgeType, _contentsPosKind);
                    UIBadgeSelectPopup badgeSelectPopup = LobbyUIManager.Instance.GetActiveUI<UIBadgeSelectPopup>("BadgeSelectPopup");
                    if(badgeSelectPopup != null)
                    {
                        badgeSelectPopup.OnClick_BadgeSlot(_badgeData.BadgeUID);
                    }
                }
                break;
        }
	}
 
    public void OnPressStart()
    {
        if (_slotType != eBadgeSlotType.Slot)
            return;
        LobbyUIManager.Instance.kHoldGauge.Show(this.transform.position, 1.5f);
    }

    public void OnPressed()
    {
        if (_slotType != eBadgeSlotType.Slot)
            return;
        LobbyUIManager.Instance.kHoldGauge.Hide();
    }

    public void OnPressing_Slot()
    {
        if (_slotType != eBadgeSlotType.Slot)
            return;
        LobbyUIManager.Instance.kHoldGauge.Hide();

        if (ParentGO == null)
            return;
        UIValue.Instance.SetValue(UIValue.EParamType.BadgeUID, _badgeData.BadgeUID);
        LobbyUIManager.Instance.ShowUI("BadgeInfoPopup", true);
    }
}
