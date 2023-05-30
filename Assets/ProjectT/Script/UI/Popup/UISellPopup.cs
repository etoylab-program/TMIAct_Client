using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UISellPopup : FComponent
{
	public UIGoodsUnit kGoodsUnit_00;
	public UIGoodsUnit kGoodsUnit_01;
	public UILabel kNameLabel;
	public UIButton kCancleBtn;
	public UIButton kSellBtn;
    [SerializeField] private FList _ItemListInstance;
    private UIItemPanel _itempanel;

    private int _receiveGold = 0;
    public override void Awake()
	{
		base.Awake();

        if (this._ItemListInstance == null) return;

        this._ItemListInstance.EventUpdate = this._UpdateItemListSlot;
        this._ItemListInstance.EventGetItemCount = this._GetItemElementCount;
        this._ItemListInstance.InitBottomFixing();
    }

	public override void OnEnable() {
		_itempanel = LobbyUIManager.Instance.GetUI<UIItemPanel>( "ItemPanel" );
		_ItemListInstance.UpdateList();

		base.OnEnable();
	}

	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        string strtext = "";
        int gold = 0;
        kGoodsUnit_00.gameObject.SetActive(false);

        if (_itempanel.kItemTab.kSelectTab == (int)UIItemPanel.eTabType.TabType_Weapon)
        {
            strtext = FLocalizeString.Instance.GetText(1089);

            int sp = 0;
            for (int i = 0; i < _itempanel.SelItemList.Count; i++)
            {
                var weapondata = GameInfo.Instance.GetWeaponData(_itempanel.SelItemList[i]);
                if (weapondata != null)
                {
                    gold += weapondata.TableData.SellPrice;
                    sp += weapondata.TableData.SellMPoint;
                }
            }

            kGoodsUnit_00.gameObject.SetActive(true);
            kGoodsUnit_00.InitGoodsUnit(eGOODSTYPE.SUPPORTERPOINT, sp);
        }
        else if (_itempanel.kItemTab.kSelectTab == (int)UIItemPanel.eTabType.TabType_Gem)
        {
            strtext = FLocalizeString.Instance.GetText(1090);

            for (int i = 0; i < _itempanel.SelItemList.Count; i++)
            {
                var gemdata = GameInfo.Instance.GetGemData(_itempanel.SelItemList[i]);
                if (gemdata != null)
                    gold += gemdata.TableData.SellPrice;
            }
        }
        else if (_itempanel.kItemTab.kSelectTab == (int)UIItemPanel.eTabType.TabType_Card)
        {
            strtext = FLocalizeString.Instance.GetText(1091);

            int sp = 0;
            for (int i = 0; i < _itempanel.SelItemList.Count; i++)
            {
                var carddata = GameInfo.Instance.GetCardData(_itempanel.SelItemList[i]);
                if (carddata != null)
                {
                    gold += carddata.TableData.SellPrice;
                    sp += carddata.TableData.SellMPoint;
                }
            }

            kGoodsUnit_00.gameObject.SetActive(true);
            kGoodsUnit_00.InitGoodsUnit(eGOODSTYPE.SUPPORTERPOINT, sp);
        }
        else if (_itempanel.kItemTab.kSelectTab == (int)UIItemPanel.eTabType.TabType_Badge)
        {
            strtext = FLocalizeString.Instance.GetText(1467);

            for(int i = 0; i < _itempanel.SelItemList.Count; i++)
            {
                var badgedata = GameInfo.Instance.GetBadgeData(_itempanel.SelItemList[i]);
                if(badgedata != null)
                {
                    gold += GameInfo.Instance.GameConfig.BadgeSellPrice;
                }
            }
        }
        else
        {
            ItemData data = GameInfo.Instance.GetItemData(_itempanel.SelItemList[0]);
            if(data != null)
                strtext = string.Format(FLocalizeString.Instance.GetText(1092), FLocalizeString.Instance.GetText(data.TableData.Name));


            var itemdata = GameInfo.Instance.GetItemData(_itempanel.SelItemList[0]);
            if (itemdata != null)
            {
                gold = _itempanel.SellCount * itemdata.TableData.SellPrice;
            }
        }

        _receiveGold = gold;

        kNameLabel.textlocalize = strtext;

        kGoodsUnit_01.InitGoodsUnit(eGOODSTYPE.GOLD, gold);
        if (_itempanel.kItemTab.kSelectTab == (int)UIItemPanel.eTabType.TabType_Badge)
        {
            kGoodsUnit_01.InitGoodsUnit(eGOODSTYPE.BATTLECOIN, gold);
        }
    }

    private void _UpdateItemListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIItemListSlot card = slotObject.GetComponent<UIItemListSlot>();
            if (null == card) break;

            if (0 <= index && _itempanel.SelItemList.Count > index)
            {
                card.ParentGO = this.gameObject;

                if (_itempanel.kItemTab.kSelectTab == (int)UIItemPanel.eTabType.TabType_Weapon)
                {
                    WeaponData data = null;
                    if (0 <= index && _itempanel.SelItemList.Count > index)
                        data = GameInfo.Instance.GetWeaponData(_itempanel.SelItemList[index]);
                    
                    
                    card.UpdateSlot(UIItemListSlot.ePosType.Info, index, data);
                }
                else if (_itempanel.kItemTab.kSelectTab == (int)UIItemPanel.eTabType.TabType_Gem)
                {
                    GemData data = null;
                    if (0 <= index && _itempanel.SelItemList.Count > index)
                        data = GameInfo.Instance.GetGemData(_itempanel.SelItemList[index]);

                    card.UpdateSlot(UIItemListSlot.ePosType.Info, index, data);
                }
                else if (_itempanel.kItemTab.kSelectTab == (int)UIItemPanel.eTabType.TabType_Card)
                {
                    CardData data = null;
                    if (0 <= index && _itempanel.SelItemList.Count > index)
                        data = GameInfo.Instance.GetCardData(_itempanel.SelItemList[index]);

                    card.UpdateSlot(UIItemListSlot.ePosType.Info, index, data);
                }
                else if (_itempanel.kItemTab.kSelectTab == (int)UIItemPanel.eTabType.TabType_Badge)
                {
                    BadgeData data = null;
                    if (0 <= index && _itempanel.SelItemList.Count > index)
                        data = GameInfo.Instance.GetBadgeData(_itempanel.SelItemList[index]);

                    card.UpdateSlot(UIItemListSlot.ePosType.Info, index, data);
                }
                else
                {
                    ItemData data = null;
                    if (0 <= index && _itempanel.SelItemList.Count > index)
                        data = GameInfo.Instance.GetItemData(_itempanel.SelItemList[index]);

                    card.UpdateSlot(UIItemListSlot.ePosType.Info, index, data);
                    card.SetCountLabel(_itempanel.SellCount.ToString());
                }

            }

        } while (false);
    }

    private int _GetItemElementCount()
    {
        return _itempanel.SelItemList.Count;
    }

    public void OnClick_CancleBtn()
	{
        OnClickClose();
    }
	
	public void OnClick_SellBtn()
	{
        bool sellFlag = false;

        if (_itempanel.kItemTab.kSelectTab == (int)UIItemPanel.eTabType.TabType_Badge)
        {
            MessagePopup.OKCANCEL(eTEXTID.TITLE_SELL, FLocalizeString.Instance.GetText(3044), SellItemList, null);
            return;
        }

        for (int i = 0; i < _itempanel.SelItemList.Count; i++)
        {
            if (_itempanel.kItemTab.kSelectTab == (int)UIItemPanel.eTabType.TabType_Weapon)
            {
                WeaponData weapondata = GameInfo.Instance.GetWeaponData(_itempanel.SelItemList[i]);
                if(weapondata.TableData.Grade >= (int)eGRADE.GRADE_SR)
                {
                    sellFlag = true;
                    break;
                }
            }
            else if (_itempanel.kItemTab.kSelectTab == (int)UIItemPanel.eTabType.TabType_Gem)
            {
                GemData gemdata = GameInfo.Instance.GetGemData(_itempanel.SelItemList[i]);
                if(gemdata.TableData.Grade >= (int)eGRADE.GRADE_SR)
                {
                    sellFlag = true;
                    break;
                }
            }
            else if (_itempanel.kItemTab.kSelectTab == (int)UIItemPanel.eTabType.TabType_Card)
            {
                CardData carddata = GameInfo.Instance.GetCardData(_itempanel.SelItemList[i]);
                if(carddata.TableData.Grade >= (int)eGRADE.GRADE_SR)
                {
                    sellFlag = true;
                    break;
                }
            }
        }

        if (sellFlag)
        {
            MessagePopup.OKCANCEL(eTEXTID.TITLE_SELL, FLocalizeString.Instance.GetText(3129), SellItemList, null);
            return;
        }


        SellItemList();
    }

    void SellItemList()
    {
        if (_itempanel.kItemTab.kSelectTab == (int)UIItemPanel.eTabType.TabType_Weapon)
        {
            GameInfo.Instance.Send_ReqSellWeaponList(_itempanel.SelItemList, OnNetWeaponSell);
        }
        else if (_itempanel.kItemTab.kSelectTab == (int)UIItemPanel.eTabType.TabType_Gem)
        {
            GameInfo.Instance.Send_ReqSellGemList(_itempanel.SelItemList, OnNetGemSell);
        }
        else if (_itempanel.kItemTab.kSelectTab == (int)UIItemPanel.eTabType.TabType_Card)
        {
            GameInfo.Instance.Send_ReqSellCardList(_itempanel.SelItemList, OnNetCardSell);
        }
        else if (_itempanel.kItemTab.kSelectTab == (int)UIItemPanel.eTabType.TabType_Badge)
        {
            GameInfo.Instance.Send_ReqSellBadge(_itempanel.SelItemList, OnNetBadgeSell);
        }
        else if (_itempanel.kItemTab.kSelectTab == (int)UIItemPanel.eTabType.TabType_ItemMat)
        {
            GameInfo.Instance.Send_ReqSellItemList(_itempanel.SelItemList[0], _itempanel.SellCount, OnNetItemSell);
        }
    }

    public void OnNetWeaponSell(int result, PktMsgType pktmsg)
    {
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        _itempanel.InitComponent();
        _itempanel.Renewal(true);

        _itempanel.ReflashSellPopup();

        ShowSellMessage(3048);
        OnClickClose();
    }

    public void OnNetGemSell(int result, PktMsgType pktmsg)
    {
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        _itempanel.InitComponent();
        _itempanel.Renewal(true);

        _itempanel.ReflashSellPopup();

        ShowSellMessage(3048);
        OnClickClose();
    }

    public void OnNetCardSell(int result, PktMsgType pktmsg)
    {
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        _itempanel.InitComponent();
        _itempanel.Renewal(true);

        _itempanel.ReflashSellPopup();

        ShowSellMessage(3048);
        OnClickClose();
    }

    public void OnNetItemSell(int result, PktMsgType pktmsg)
    {
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        _itempanel.InitComponent();
        _itempanel.Renewal(true);

        _itempanel.ReflashSellPopup();

        ShowSellMessage(3048);
        OnClickClose();
    }

    public void OnNetBadgeSell(int result, PktMsgType pktmsg)
    {
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        _itempanel.InitComponent();
        _itempanel.Renewal(true);

        _itempanel.ReflashSellPopup();

        ShowSellMessage(3169);
        OnClickClose();
    }

    /// <summary>
    ///  판매하여 획득한 금액을 메세지 팝업으로 표시합니다.
    /// </summary>
    private void ShowSellMessage(int textId)
    {
        string str = FLocalizeString.Instance.GetText(textId, _receiveGold);
        MessageToastPopup.Show(str);

        _receiveGold = 0;
    }
}
