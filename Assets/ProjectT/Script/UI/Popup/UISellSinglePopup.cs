using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UISellSinglePopup : FComponent
{
    public enum eSELLTYPE
    {
        NONE = 0,
        WEAPON,
        GEM,
        CARD,
        ITEM,
        BADGE,
    };
	public UILabel kNameLabel;
	public GameObject kSellPopupCount;
    public GameObject kSellPopupCountSlide;
    public UISlider kSellSlider;
    public UILabel kSellSlideCountLabel;
    public UILabel kCountLabel;
    public UIItemListSlot kItemListSlot;
    public UIGoodsUnit kGoodsUnit_00;
	public UIGoodsUnit kGoodsUnit_01;
	public UIButton kCancleBtn;
	public UIButton kSellBtn;
    private eSELLTYPE _type;
    private long _uid;
    private int _sellcount;

    private int _receiveGold = 0;

    public override void Awake()
	{
		base.Awake();

        kSellSlider.onChange.Add(new EventDelegate(OnChange_SellSlide));
    }
 
	public override void OnEnable()
	{
		_type = (eSELLTYPE)UIValue.Instance.GetValue( UIValue.EParamType.SellSingleType );
		_uid = (long)UIValue.Instance.GetValue( UIValue.EParamType.SellSingleUID );
		_sellcount = 1;

		base.OnEnable();

        if(_type == eSELLTYPE.WEAPON)
        {
            RenderTargetWeapon.Instance.gameObject.SetActive(false);
        }
            
        //처음 열리고 나서 창 애니메이션으로 틀어진 스프라이트 위치를 잡아준다.
        kSellSlider.thumb.localPosition = new Vector3(-105, 0, 0);
    }

    public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		base.OnDisable();

        if (_type == eSELLTYPE.WEAPON)
        {
            RenderTargetWeapon.Instance.gameObject.SetActive(true);
        }
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        string strtext = "";
        int gold = 0;
        kGoodsUnit_00.gameObject.SetActive(false);
        kSellPopupCount.SetActive(false);

        kSellPopupCountSlide.SetActive(false);

        if (_type == eSELLTYPE.WEAPON)
        {
            strtext = FLocalizeString.Instance.GetText(1089);

            int sp = 0;
            var weapondata = GameInfo.Instance.GetWeaponData(_uid);
            if (weapondata != null)
            {
                gold += weapondata.TableData.SellPrice;
                sp += weapondata.TableData.SellMPoint;
                kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Info, 0, weapondata);
            }

            kGoodsUnit_00.gameObject.SetActive(true);
            kGoodsUnit_00.InitGoodsUnit(eGOODSTYPE.SUPPORTERPOINT, sp);
        }
        else if (_type == eSELLTYPE.GEM)
        {
            strtext = FLocalizeString.Instance.GetText(1090);
            var gemdata = GameInfo.Instance.GetGemData(_uid);
            if (gemdata != null)
            {
                gold += gemdata.TableData.SellPrice;
                kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Info, 0, gemdata);
            }
        }
        else if (_type == eSELLTYPE.CARD)
        {
            strtext = FLocalizeString.Instance.GetText(1091);

            int sp = 0;
            var carddata = GameInfo.Instance.GetCardData(_uid);
            if (carddata != null)
            {
                gold += carddata.TableData.SellPrice;
                sp += carddata.TableData.SellMPoint;
                kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Info, 0, carddata);
            }

            kGoodsUnit_00.gameObject.SetActive(true);
            kGoodsUnit_00.InitGoodsUnit(eGOODSTYPE.SUPPORTERPOINT, sp);
        }
        else if (_type == eSELLTYPE.BADGE)
        {
            strtext = FLocalizeString.Instance.GetText(1467);       //문양판매

            var badgedata = GameInfo.Instance.GetBadgeData(_uid);
            if(badgedata != null)
            {
                gold += GameInfo.Instance.GameConfig.BadgeSellPrice;
                kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Info, 0, badgedata);
            }
        }
        else
        {
            var itemdata = GameInfo.Instance.GetItemData(_uid);
            if (itemdata != null)
            {
                strtext = string.Format(FLocalizeString.Instance.GetText(1092), FLocalizeString.Instance.GetText(itemdata.TableData.Name));
                gold = _sellcount * itemdata.TableData.SellPrice;
                kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Info, 0, itemdata);

                //kSellPopupCount.SetActive(true);
                //kCountLabel.textlocalize = _sellcount.ToString();

                kSellPopupCountSlide.SetActive(true);
                kSellSlideCountLabel.textlocalize = _sellcount.ToString("#,##0");

                float f = _sellcount / (float)itemdata.Count;
                if (_sellcount <= 1)
                    f = 0f;

                if (itemdata.Count <= 1)
                    f = 1f;

                kSellSlider.Set(f, false);
            }
            
        }

        _receiveGold = gold;

        kNameLabel.textlocalize = strtext;
        kGoodsUnit_01.InitGoodsUnit(eGOODSTYPE.GOLD, gold);

        if(_type == eSELLTYPE.BADGE)
        {
            kGoodsUnit_01.InitGoodsUnit(eGOODSTYPE.BATTLECOIN, gold);
        }
    }
 

	
	public void OnClick_CancleBtn()
	{
        OnClickClose();
    }

    public void OnClick_SellBtn()
    {
        bool sellFlag = false;

        if (_type == eSELLTYPE.WEAPON)
        {
            var weapondata = GameInfo.Instance.GetWeaponData(_uid);
            if (weapondata != null)
            {
                //  잠금 상태, 장착 상태, 시설 이용 상태의 경우
                if (weapondata.Lock == true ||
                   GameInfo.Instance.GetEquipWeaponCharData(_uid) != null ||
                   GameInfo.Instance.GetEquipWeaponFacilityData(_uid) != null)
                {
                    return;
                }

                if (weapondata.TableData.Grade >= (int)eGRADE.GRADE_SR)
                    sellFlag = true;
            }
        }
        else if (_type == eSELLTYPE.GEM)
        {
            var gemdata = GameInfo.Instance.GetGemData(_uid);
            if (gemdata != null)
            {
                //  잠금 상태거나 장착 상태의 경우
                if (gemdata.Lock == true ||
                    GameInfo.Instance.GetEquipGemWeaponData(_uid) != null)
                {
                    return;
                }

                if (gemdata.TableData.Grade >= (int)eGRADE.GRADE_SR)
                    sellFlag = true;
            }
        }
        else if (_type == eSELLTYPE.CARD)
        {
            var carddata = GameInfo.Instance.GetCardData(_uid);
            if (carddata != null)
            {
                //  잠금 상태, 장착 상태, 시설 이용 상태의 경우
                if (carddata.Lock == true ||
                    GameInfo.Instance.GetEquiCardCharData(_uid) != null ||
                    GameSupport.IsEquipAndUsingCardData(_uid))
                {
                    return;
                }

                if (carddata.TableData.Grade >= (int)eGRADE.GRADE_SR)
                    sellFlag = true;
            }
        }
        else if (_type == eSELLTYPE.BADGE)
        {
            MessagePopup.OKCANCEL(eTEXTID.TITLE_SELL, FLocalizeString.Instance.GetText(3044), SellItemList, null);
            return;
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
        List<long> uidlist = new List<long>();
        uidlist.Add(_uid);
        if (_type == eSELLTYPE.WEAPON)
        {
            GameInfo.Instance.Send_ReqSellWeaponList(uidlist, OnNetWeaponSell);
        }
        else if (_type == eSELLTYPE.GEM)
        {
            GameInfo.Instance.Send_ReqSellGemList(uidlist, OnNetGemSell);
        }
        else if (_type == eSELLTYPE.CARD)
        {
            GameInfo.Instance.Send_ReqSellCardList(uidlist, OnNetCardSell);
        }
        else if (_type == eSELLTYPE.ITEM)
        {
            if (_sellcount <= (int)eCOUNT.NONE)
            {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(200007));
                return;
            }
            GameInfo.Instance.Send_ReqSellItemList(_uid, _sellcount, OnNetItemSell);
        }
        else if (_type == eSELLTYPE.BADGE)
        {
            GameInfo.Instance.Send_ReqSellBadge(uidlist, OnNetBadgeSell);
        }
    }

	public void OnClick_MinusBtn()
	{
        var itemdata = GameInfo.Instance.GetItemData(_uid);
        if (itemdata == null)
            return;

        _sellcount -= 1;
        if (_sellcount <= 1)
            _sellcount = 1;

        Renewal(true);
    }
	
	public void OnClick_PlusBtn()
	{
        var itemdata = GameInfo.Instance.GetItemData(_uid);
        if (itemdata == null)
            return;

        _sellcount += 1;
        if (_sellcount >= itemdata.Count)
            _sellcount = itemdata.Count;
        Renewal(true);
    }
	
	public void OnClick_MaxBtn()
	{
        var itemdata = GameInfo.Instance.GetItemData(_uid);
        if (itemdata == null)
            return;

        _sellcount = itemdata.Count;
        Renewal(true);
    }

    public void OnChange_SellSlide()
    {
        var itemdata = GameInfo.Instance.GetItemData(_uid);
        if (itemdata == null)
            return;

        _sellcount = (int)(kSellSlider.value * itemdata.Count);

        if (_sellcount <= 1)
            _sellcount = 1;

        Log.Show(kSellSlider.value);
        Renewal(true);
    }



    public void OnNetWeaponSell(int result, PktMsgType pktmsg)
    {
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.HideUI("WeaponInfoPopup");

        UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
        if (itempanel != null)
        {
            itempanel.InitComponent();
            itempanel.Renewal(true);
            //itempanel.ReflashSellPopup();
        }

        LobbyUIManager.Instance.InitComponent("CharWeaponSeletePopup");
        LobbyUIManager.Instance.Renewal("CharWeaponSeletePopup");
        

        ShowSellMessage(3048);
        OnClickClose();
    }

    public void OnNetGemSell(int result, PktMsgType pktmsg)
    {
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.HideUI("GemInfoPopup");

        UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
        if(itempanel != null)
        {
            itempanel.InitComponent();
            itempanel.Renewal(true);
            //itempanel.ReflashSellPopup();
        }

        LobbyUIManager.Instance.InitComponent("WeaponGemSeletePopup");
        LobbyUIManager.Instance.Renewal("WeaponGemSeletePopup");



        ShowSellMessage(3048);
        OnClickClose();
    }

    public void OnNetCardSell(int result, PktMsgType pktmsg)
    {
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.HideUI("CardInfoPopup");

        UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
        if (itempanel != null)
        {
            itempanel.InitComponent();
            itempanel.Renewal(true);
            //itempanel.ReflashSellPopup();
        }

        LobbyUIManager.Instance.InitComponent("CharCardSeletePopup");
        LobbyUIManager.Instance.Renewal("CharCardSeletePopup");

        LobbyUIManager.Instance.InitComponent("FacilityCardSeletePopup");
        LobbyUIManager.Instance.Renewal("FacilityCardSeletePopup");
        

        ShowSellMessage(3048);
        OnClickClose();
    }

    public void OnNetItemSell(int result, PktMsgType pktmsg)
    {
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.HideUI("ItemInfoPopup");

        UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
        if (itempanel != null)
        {
            itempanel.InitComponent();
            itempanel.Renewal(true);
            //itempanel.ReflashSellPopup();
        }

        ShowSellMessage(3048);
        OnClickClose();
    }

    public void OnNetBadgeSell(int result, PktMsgType pktmsg)
    {
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.HideUI("BadgeInfoPopup");

        UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
        if (itempanel != null)
        {
            itempanel.InitComponent();
            itempanel.Renewal(true);
            //itempanel.ReflashSellPopup();
        }

        UIBadgeSelectPopup badgeSelectPopup = LobbyUIManager.Instance.GetActiveUI<UIBadgeSelectPopup>("BadgeSelectPopup");
        if (badgeSelectPopup != null)
        {
            badgeSelectPopup.InitComponent();
            badgeSelectPopup.Renewal(true);
        }

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
