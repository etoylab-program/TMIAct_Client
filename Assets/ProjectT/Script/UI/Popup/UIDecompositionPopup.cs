using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIDecompositionPopup : FComponent
{
    [SerializeField] private UILabel TitleLabel;
    [SerializeField] private FList MaterialList = null;
    [SerializeField] private FList ResultList = null;

    private UIItemPanel.eTabType CurTabType = UIItemPanel.eTabType.TabType_None;
    private List<long> MaterialUIDs = new List<long>();
    private List<RewardData> ResultRewards = new List<RewardData>();

    //private UIItemPanel ItemPanel = null;

    public void SetData(List<long> DecompoItemList, List<RewardData> DecompoRewardList, UIItemPanel.eTabType TabType)
    {
        MaterialUIDs.Clear();
        ResultRewards.Clear();

        for (int i = 0; i < DecompoItemList.Count; i++)
            MaterialUIDs.Add(DecompoItemList[i]);

        for (int i = 0; i < DecompoRewardList.Count; i++)
            ResultRewards.Add(DecompoRewardList[i]);

        CurTabType = TabType;
    }

    public override void Awake()
    {
        base.Awake();

        MaterialList.EventUpdate = UpdateMaterialListSlot;
        MaterialList.EventGetItemCount = () => { return MaterialUIDs.Count; };
        MaterialList.UpdateList();
        MaterialList.InitBottomFixing();

        ResultList.EventUpdate = UpdateResultListSlot;
        ResultList.EventGetItemCount = () => { return ResultRewards.Count; };
        ResultList.UpdateList();
    }

    public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		MaterialUIDs.Clear();
        ResultRewards.Clear();
        base.OnDisable();
    }

    public override void Renewal(bool bChildren = false)
    {
        base.Renewal(bChildren);

        switch (CurTabType)
        {
            case UIItemPanel.eTabType.TabType_Weapon:
                TitleLabel.textlocalize = FLocalizeString.Instance.GetText(1671);
                break;
            case UIItemPanel.eTabType.TabType_Card:
                TitleLabel.textlocalize = FLocalizeString.Instance.GetText(1670);
                break;
            case UIItemPanel.eTabType.TabType_None:
            case UIItemPanel.eTabType.TabType_Gem:
            case UIItemPanel.eTabType.TabType_ItemMat:
            case UIItemPanel.eTabType.TabType_Badge:
                {
                    TitleLabel.textlocalize = FLocalizeString.Instance.GetText(1669);
                    UpdateList();
                    return;
                }            
        }

        //재료리스트 표현
        UpdateList();
    }

    private void UpdateList()
    {
        MaterialList.UpdateList();
        ResultList.UpdateList();
        
    }

    public void OnClick_Decomposition()
    {
        eContentsPosKind kind = eContentsPosKind._NONE_;

        if (CurTabType == UIItemPanel.eTabType.TabType_Weapon)
            kind = eContentsPosKind.WEAPON;
        else if (CurTabType == UIItemPanel.eTabType.TabType_Card)
            kind = eContentsPosKind.CARD;

        if (kind == eContentsPosKind._NONE_)
            return;

        List<ulong> SendUIDs = new List<ulong>();
        for (int i = 0; i < MaterialUIDs.Count; i++)
        {
            SendUIDs.Add((ulong)MaterialUIDs[i]);
        }

        if (SendUIDs.Count == 0)
            return;

        // 분해시작
        GameInfo.Instance.Send_ReqDecomposition(kind, SendUIDs, OnNetAckDecomposition);
    }

    private void UpdateMaterialListSlot(int index, GameObject slotObj)
    {
        UIItemListSlot slot = slotObj.GetComponent<UIItemListSlot>();
        if (slot == null) return;

        if (CurTabType== UIItemPanel.eTabType.TabType_Weapon)
        {
            WeaponData data = null;
            if (0 <= index && MaterialUIDs.Count > index)
            {   
                data = GameInfo.Instance.WeaponList.Find(x => x.WeaponUID == MaterialUIDs[index]);
            }
            slot.UpdateSlot(UIItemListSlot.ePosType.None, index, data);
        }
        else if (CurTabType == UIItemPanel.eTabType.TabType_Card)
        {
            CardData data = null;
            if (0 <= index && MaterialUIDs.Count > index)
            {
                data = GameInfo.Instance.CardList.Find(x => x.CardUID == MaterialUIDs[index]);
            }
            slot.UpdateSlot(UIItemListSlot.ePosType.None, index, data);
        }
    }

    private void UpdateResultListSlot(int index, GameObject slotObj)
    {
        UIItemListSlot slot = slotObj.GetComponent<UIItemListSlot>();
        if (slot == null) return;

        slot.UpdateSlot(UIItemListSlot.ePosType.RewardDataInfo, index, ResultRewards[index]);
    }


    private void OnNetAckDecomposition(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        MessageRewardListPopup.RewardListMessage(FLocalizeString.Instance.GetText(1672), 
            FLocalizeString.Instance.GetText((int)eTEXTID.REWARD_POPUP_TEXT), 
            GameInfo.Instance.RewardList, () => {

                LobbyUIManager.Instance.Renewal("TopPanel");
                LobbyUIManager.Instance.Renewal("GoodsPopup");

                bool IsActiveWeaponUI = LobbyUIManager.Instance.GetActiveUI("WeaponInfoPopup") != null;
                bool IsActiveCardUI = LobbyUIManager.Instance.GetActiveUI("CardInfoPopup") != null;

                if (IsActiveWeaponUI) LobbyUIManager.Instance.HideUI("WeaponInfoPopup");
                if (IsActiveCardUI) LobbyUIManager.Instance.HideUI("CardInfoPopup");

                UIItemPanel ItemPanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
                if (ItemPanel != null)
                {
                    ItemPanel.InitComponent();
                    ItemPanel.Renewal(true);

                    if(!IsActiveWeaponUI && !IsActiveCardUI)
                        ItemPanel.ReflashDecompoPopup();
                }

                UICharWeaponSeletePopup charWeaponSeletePopup = LobbyUIManager.Instance.GetActiveUI<UICharWeaponSeletePopup>("CharWeaponSeletePopup");
                if (charWeaponSeletePopup != null)
                {
                    charWeaponSeletePopup.InitComponent();
                    charWeaponSeletePopup.Renewal(true);
                }

                UICharCardSeletePopup charCardSeletePopup = LobbyUIManager.Instance.GetActiveUI<UICharCardSeletePopup>("CharCardSeletePopup");
                if (charCardSeletePopup != null)
                {
                    charCardSeletePopup.InitComponent();
                    charCardSeletePopup.Renewal(true);
                }

                OnClickClose(); 
                });
    }

}
