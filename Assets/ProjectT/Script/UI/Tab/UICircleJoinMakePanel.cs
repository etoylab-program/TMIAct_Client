using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICircleJoinMakePanel : FComponent
{
    [Header("UICircleJoinMakePanel")]
    [SerializeField] private UIInput        nameInput = null;
    [SerializeField] private UIInput        contentInput = null;
    [SerializeField] private UILabel        mainLangLabel = null;
    [SerializeField] private FToggle        otherLangFToggle = null;
    [SerializeField] private UIItemListSlot itemListSlot = null;
    [SerializeField] private UIGoodsUnit    goodsUnit = null;

    private eLANGUAGE _currentLang;
    private bool _isAvailable;

    public override void Awake()
    {
        base.Awake();

        nameInput.defaultText = "서클명 입력"; // Test - LeeSeungJin - Change String
        contentInput.defaultText = "서클 소개문 입력"; // Test - LeeSeungJin - Change String
    }

    public override void InitComponent()
    {
        base.InitComponent();

        nameInput.value = string.Empty;
        contentInput.value = string.Empty;

        otherLangFToggle.SetToggle((int)eToggleType.Off, SelectEvent.Code);

        // Test - LeeSeungJin - Game.ItemReqList & Change String
        GameTable.Item.Param itemParam = GameInfo.Instance.GameTable.FindItem(10014);
        itemListSlot.UpdateSlot(UIItemListSlot.ePosType.Mat, 0, itemParam);

        int needItemCount = 50;
        int itemCount = 0;
        ItemData itemData = GameInfo.Instance.GetItemData(10014);
        if (itemData != null)
        {
            itemCount = itemData.Count;
        }

        _isAvailable = needItemCount <= itemCount;
        eTEXTID colorTextId = _isAvailable ? eTEXTID.WHITE_TEXT_COLOR : eTEXTID.RED_TEXT_COLOR;
        itemListSlot.SetCountText(FLocalizeString.Instance.GetText((int)colorTextId, $"50/{itemCount}"));

        goodsUnit.InitGoodsUnit(eGOODSTYPE.GOLD, 200000, bcheck: true);
        // Test - LeeSeungJin - End
    }

    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void Renewal(bool bChildren = false)
    {
        base.Renewal(bChildren);

        _currentLang = FLocalizeString.Language;
        mainLangLabel.textlocalize = GetStringCurrentLang();
    }

    private string GetStringCurrentLang()
    {
        return FLocalizeString.Instance.GetText((int)_currentLang + 601); // Test - LeeSeungJin - Temp
    }

    public void OnClick_CreateBtn()
    {
        if (_isAvailable == false)
        {
            MessagePopup.OK(eTEXTID.OK, FLocalizeString.Instance.GetText(3003), null);
            return;
        }

        // Test - LeeSeungJin - Change String Start
        if (string.IsNullOrEmpty(nameInput.value))
        {
            MessageToastPopup.Show("서클명이 비어 있습니다.");
            return;
        }

        if (string.IsNullOrEmpty(contentInput.value))
        {
            MessageToastPopup.Show("서클 소개문이 비어 있습니다.");
            return;
        }

        if (Utility.IsCommandCheck(nameInput.value))
        {
            MessagePopup.OK(eTEXTID.OK, "서클명에 비속어가 포함되어 있습니다.", null);
            return;
        }

        if (Utility.IsCommandCheck(contentInput.value))
        {
            MessagePopup.OK(eTEXTID.OK, "서클 소개문에 비속어가 포함되어 있습니다.", null);
            return;
        }
        // Test - LeeSeungJin - Change String End

        GameInfo.Instance.Send_ReqCircleOpen(nameInput.value, contentInput.value, _currentLang, otherLangFToggle.kSelect == (int)eToggleType.On, OnNet_CircleOpen);
    }

    public void OnClick_LeftBtn()
    {
        if (_currentLang == eLANGUAGE.KOR)
        {
            return;
        }

        --_currentLang;
        mainLangLabel.textlocalize = GetStringCurrentLang();
    }

    public void OnClick_RightBtn()
    {
        if (_currentLang == eLANGUAGE.ESP)
        {
            return;
        }

        ++_currentLang;
        mainLangLabel.textlocalize = GetStringCurrentLang();
    }

    private void OnNet_CircleOpen(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.MAIN);
    }
}
