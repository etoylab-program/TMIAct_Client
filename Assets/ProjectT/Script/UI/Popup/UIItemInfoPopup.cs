using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIItemInfoPopup : FComponent
{
    public enum eItemInfoType
    {
        Info,
        Get,
        _MAX_,
    }

    public GameObject kInfo;
    public GameObject kAcquisition;
    public UIButton kAcquisitionTabBtn;
    public UIAcquisitionInfoUnit kAcquisitionInfoUnit;
    public UILabel kNameLabel;
    public UIItemListSlot kItemListSlot;
    public UILabel kDescLabel;
    public UIButton kSelBtn;
    public UIButton kRunBtn;

    public GameObject kRunCntBtnObj;
    public UILabel kRunCntLabel;

    public GameObject kItemInfoTypeTabRoot;
    public FTab kItemInfoTypeTab;
    private eItemInfoType _itemInfoType;

    private ItemData _itemdata = null;
    private long _useitemuid = -1;
    private int _useitemtableid = -1;
    private List<CardBookData> _precardbooklist = new List<CardBookData>();

    private int _selectItemUseCnt = 1;

	private CharData mCharData = null;

    private UIPanel _uiPanel = null;
    private bool _isOriginSave = false;
    private int _originDepth = 0;
    private string _originSortingLayerName = "";
    private int _originSortingOrder = 0;
    private bool _originUseSortingOrder = false;
    private bool mbUseResetSkillPoint = false;

    private UIPanel uiPanel
    {
        get
        {
            if (_uiPanel == null)
            {
                _uiPanel = this.GetComponent<UIPanel>();
            }

            if (_uiPanel != null && _isOriginSave == false)
            {
                _isOriginSave = true;
                _originDepth = uiPanel.depth;
                _originSortingOrder = uiPanel.sortingOrder;
                _originUseSortingOrder = uiPanel.useSortingOrder;
                _originSortingLayerName = uiPanel.sortingLayerName;
            }

            return _uiPanel;
        }
    }

    public override void Awake()
    {
        base.Awake();

        kItemInfoTypeTab.EventCallBack = OnItemInfoTypeTabSelect;
    }

    public override void OnEnable()
    {
        _itemInfoType = eItemInfoType.Info;
        kItemInfoTypeTab.SetTab((int)_itemInfoType, SelectEvent.Code);
        base.OnEnable();
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        kSelBtn.gameObject.SetActive(false);
        kRunBtn.gameObject.SetActive(false);
        kRunCntBtnObj.SetActive(false);
        GameTable.Item.Param itemtabledata = null;
        GameTable.LobbyAnimation.Param lobbyTableData = null;

        long itemuid = (long)UIValue.Instance.GetValue(UIValue.EParamType.ItemUID);
        if (itemuid != -1)
        {
            _itemdata = GameInfo.Instance.GetItemData(itemuid);
			if (_itemdata != null && _itemdata.TableData != null)
			{
				itemtabledata = _itemdata.TableData;
                if (itemtabledata.SubType == (int)eITEMSUBTYPE.USE_RESET_ALL_AWAKEN_Skill)
                {
                    kSelBtn.SetActive(true);
                }
                else
                {
                    kSelBtn.gameObject.SetActive(GameSupport.IsAbleSellItem(itemtabledata));
                }
			}
        }
        else
        {
            int itemId = (int) UIValue.Instance.GetValue(UIValue.EParamType.ItemTableID);
            int lobbyAnimId = -1;
            object lobbyAnimObj = UIValue.Instance.GetValue(UIValue.EParamType.LobbyAnimTableID);
            if (!ReferenceEquals(lobbyAnimObj, null))
            {
                lobbyAnimId = (int)lobbyAnimObj;
            }
            
            if (itemId != -1)
            {
                _itemdata = GameInfo.Instance.GetItemData(itemId);
                itemtabledata = GameInfo.Instance.GameTable.FindItem(itemId);
            }
            else if (lobbyAnimId != -1)
            {
                lobbyTableData = GameInfo.Instance.GameTable.FindLobbyAnimation(lobbyAnimId);
            }
        }

        if (itemtabledata != null)
        {
            SetItemData(itemuid, itemtabledata);
        }
        else if (lobbyTableData != null)
        {
            kItemInfoTypeTabRoot.SetActive(false);

            kNameLabel.textlocalize = FLocalizeString.Instance.GetText(lobbyTableData.Name);
            kDescLabel.textlocalize = FLocalizeString.Instance.GetText(lobbyTableData.Desc);

            int randomId = (int) UIValue.Instance.GetValue(UIValue.EParamType.RandomTableID);
            if (randomId != -1)
            {
                GameTable.Random.Param randomTableData = GameInfo.Instance.GameTable.FindRandom(randomId);
                if (randomTableData != null)
                {
                    kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Info, 0, randomTableData);
                }
            }
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();

        if (uiPanel == null)
        {
            return;
        }

        uiPanel.depth = _originDepth;
        uiPanel.sortingOrder = _originSortingOrder;
        uiPanel.useSortingOrder = _originUseSortingOrder;
        uiPanel.sortingLayerName = _originSortingLayerName;
    }

    private void SetItemData(long itemuid, GameTable.Item.Param itemtabledata)
    {
        if (itemuid != -1 && itemtabledata.Type == (int)eITEMTYPE.USE)
        {
            if (itemtabledata.SubType == (int)eITEMSUBTYPE.USE_AP_CHARGE || itemtabledata.SubType == (int)eITEMSUBTYPE.USE_BP_CHARGE || 
				itemtabledata.SubType == (int)eITEMSUBTYPE.USE_AP_CHARGE_NUM || itemtabledata.SubType == (int)eITEMSUBTYPE.USE_BP_CHARGE_NUM ||
                itemtabledata.SubType == (int)eITEMSUBTYPE.USE_STORE_PACKAGE || itemtabledata.SubType == (int)eITEMSUBTYPE.USE_PACKAGE_ITEM ||
                itemtabledata.SubType == (int)eITEMSUBTYPE.USE_ADDITEMSLOT)
            {
                kRunCntBtnObj.SetActive(true);
                _selectItemUseCnt = 1;
                kRunCntLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(222), _selectItemUseCnt);

                kSelBtn.gameObject.SetActive(itemtabledata.SubType == (int)eITEMSUBTYPE.USE_ADDITEMSLOT && itemtabledata.SellPrice > 0);
            }
            else
            {
                kRunBtn.gameObject.SetActive(true);
            }
        }

        if (_itemdata != null)
        {
            kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Info, -1, _itemdata);
        }
        else
        {
            kItemListSlot.UpdateSlot(UIItemListSlot.ePosType.Info, -1, itemtabledata);
            kItemListSlot.SetCountText(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.RED_TEXT_COLOR), string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT), 0)));
        }

        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(itemtabledata.Name);
        kDescLabel.textlocalize = FLocalizeString.Instance.GetText(itemtabledata.Desc);

        kAcquisitionInfoUnit.UpdateSlot(itemtabledata.AcquisitionID);
        kAcquisitionTabBtn.gameObject.SetActive(true);
        kItemInfoTypeTabRoot.SetActive(false);
        if (itemtabledata.AcquisitionID > 0)
        {
            kItemInfoTypeTabRoot.SetActive(true);
        }

    }

    public void OnClick_SelBtn()
    {
        if (_itemdata == null)
            return;
        UIValue.Instance.SetValue(UIValue.EParamType.SellSingleType, UISellSinglePopup.eSELLTYPE.ITEM);
        UIValue.Instance.SetValue(UIValue.EParamType.SellSingleUID, _itemdata.ItemUID);
        LobbyUIManager.Instance.ShowUI("SellSinglePopup", true);
        OnClickClose();
    }

    public void OnClick_BackBtn()
    {
        OnClickClose();
    }

    public void OnClick_RunBtn()
    {
        if (_itemdata == null)
            return;

        if (_itemdata.TableData.Type != (int)eITEMTYPE.USE)
            return;

        mbUseResetSkillPoint = false;

        if (_itemdata.TableData.SubType == (int)eITEMSUBTYPE.USE_GACHA_TICKET)
        {
            UIValue.Instance.SetValue(UIValue.EParamType.GachaTab, "TICKET");
            LobbyUIManager.Instance.SetPanelType(ePANELTYPE.GACHA);
            OnClickClose();
        }
        else if (_itemdata.TableData.SubType == (int)eITEMSUBTYPE.USE_RESET_ALL_AWAKEN_Skill)
        {
            int totalPoint = 0;
            foreach(AwakenSkillInfo info in GameInfo.Instance.UserData.ListAwakenSkillData)
            {
                GameTable.AwakeSkill.Param awakeSkillParam = GameInfo.Instance.GameTable.FindAwakeSkill(info.TableId);
                if(awakeSkillParam == null)
                {
                    continue;
                }
                List<GameTable.ItemReqList.Param> itemReqListParamList =
                    GameInfo.Instance.GameTable.FindAllItemReqList(x => x.Group == awakeSkillParam.ItemReqListID && x.Level < info.Level);
                foreach(GameTable.ItemReqList.Param param in itemReqListParamList)
                {
                    totalPoint += param.GoodsValue;
                }
            }

            mbUseResetSkillPoint = true;

            if (totalPoint <= 0)
            {
                MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(1797), null);
            }
            else
            {
                MessagePopup.YN(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(1632), OnMsg_UseItem, wpValue: totalPoint);
            }
        }
        else
        {
            _precardbooklist.Clear();
            _precardbooklist.AddRange(GameInfo.Instance.CardBookList);

            if (_itemdata.TableData.SubType == (int)eITEMSUBTYPE.USE_PROMOTION_ITEM)
            {
                //프로모션 아이템 - 서버에서 코드 받으면 스크린샷 저장할지 팝업보여주기 - 콜백따로 받기
                string text = string.Format(FLocalizeString.Instance.GetText(115), FLocalizeString.Instance.GetText(_itemdata.TableData.Name));
                MessagePopup.CYN(eTEXTID.TITLE_NOTICE, text, eTEXTID.YES, eTEXTID.NO, OnMsg_UseItemCode);
                return;
            }
            else if (_itemdata.TableData.SubType == (int)eITEMSUBTYPE.USE_STORE_PACKAGE)
            {
                MessagePopup.CYN(eTEXTID.TITLE_NOTICE, string.Format(FLocalizeString.Instance.GetText(115), FLocalizeString.Instance.GetText(_itemdata.TableData.Name)), eTEXTID.YES, eTEXTID.NO, StorePackageItemOpen, null);
                return;
            }
			else if(_itemdata.TableData.SubType == (int)eITEMSUBTYPE.USE_CHARACTER_GRADE_UP)
			{
				int count = 0;
				for (int i = 0; i < GameInfo.Instance.CharList.Count; i++)
				{
					if (GameInfo.Instance.CharList[i].Grade >= _itemdata.TableData.Value)
					{
						continue;
					}

					++count;
				}

				if (count > 0)
				{
					MessagePopup.CYN(eTEXTID.TITLE_NOTICE, string.Format(FLocalizeString.Instance.GetText(115),
									 FLocalizeString.Instance.GetText(_itemdata.TableData.Name)), eTEXTID.YES, eTEXTID.NO, OnUseCharacterGradeUpItem, null);
				}
				else
				{
					MessageToastPopup.Show(FLocalizeString.Instance.GetText(3272));
				}

				return;
			}
            else
            {
                string text = string.Format(FLocalizeString.Instance.GetText(115), FLocalizeString.Instance.GetText(_itemdata.TableData.Name));
                MessagePopup.CYN(eTEXTID.TITLE_NOTICE, text, eTEXTID.YES, eTEXTID.NO, OnMsg_UseItem);
                return;
            }
        }
    }

    public void StorePackageItemOpen()
    {
        GameTable.Store.Param storeTableData = GameInfo.Instance.GameTable.FindStore(_itemdata.TableData.Value);
        if (storeTableData == null)
            return;

        _useitemuid = _itemdata.ItemUID;
        _useitemtableid = _itemdata.TableID;
        GameInfo.Instance.Send_ReqStorePurchase(storeTableData.ID, false, _selectItemUseCnt, OnNetUseItem);
    }

    public void OnMsg_UseItem()
    {
        _useitemuid = _itemdata.ItemUID;
        _useitemtableid = _itemdata.TableID;
        GameInfo.Instance.Send_ReqUseItem(_itemdata.ItemUID, 1, 0, OnNetUseItem);
    }

	public void OnNetUseItem( int result, PktMsgType pktmsg ) {
        if ( result != 0 ) {
            return;
        }

        if ( !mbUseResetSkillPoint ) {
            for ( int i = 0; i < GameInfo.Instance.RewardList.Count; i++ ) {
                if ( GameInfo.Instance.RewardList[i].Type == (int)eREWARDTYPE.CHAR ) {
                    DirectorUIManager.Instance.PlayCharBuy( GameInfo.Instance.RewardList[i].Index, OnNetResult_Renewal );
                    return;
                }
            }
        }

		OnNetResult_Renewal();
	}

	public void OnMsg_UseItemCode()
    {
        _useitemuid = _itemdata.ItemUID;
        _useitemtableid = _itemdata.TableID;
        GameInfo.Instance.Send_ReqUseItem(_itemdata.ItemUID, 1, 0, OnMsg_UseItemCode);
    }

    //프로모션 아이템 코드
    public void OnMsg_UseItemCode(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        PktInfoUseItemCodeAck pktInfoUseItemCodeAck = pktmsg as PktInfoUseItemCodeAck;
        if (pktInfoUseItemCodeAck != null)
            MessagePopup.ScreenShotPopup(FLocalizeString.Instance.GetText(10120022), pktInfoUseItemCodeAck.code_, null);

        OnNetResult_Renewal();
    }

    private void OnNetResult_Renewal()
    {
        var tabledata = GameInfo.Instance.GameTable.FindItem(_useitemtableid);
        if (tabledata != null)
            MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3076), FLocalizeString.Instance.GetText(tabledata.Name)));

        if ( !mbUseResetSkillPoint ) {
            DirectorUIManager.Instance.PlayNewCardGreeings( _precardbooklist );
        }

        var itemdata = GameInfo.Instance.GetItemData(_useitemuid);
        if (itemdata == null)
        {
            //191104
            //티켓 회복물약을 사용시 아이템이 제거 됬기 때문에 null로 떨어짐.
            //상단바 티켓 최신화

            LobbyUIManager.Instance.Renewal("TopPanel");
            LobbyUIManager.Instance.Renewal("GoodsPopup");
            UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
            if (itempanel != null)
            {
                itempanel.InitComponent();
                itempanel.Renewal(true);
            }

            OnClickClose();
        }
        else
        {
            LobbyUIManager.Instance.Renewal("TopPanel");
            LobbyUIManager.Instance.Renewal("GoodsPopup");
            
            if(itemdata.TableData.SubType == (int)eITEMSUBTYPE.USE_ADDITEMSLOT)
            {
                LobbyUIManager.Instance.InitComponent("ItemPanel");
                LobbyUIManager.Instance.Renewal("ItemPanel");
            }
            else
            {
                UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
                if (itempanel != null)
                    itempanel.RefreshList();
            }

            Renewal(true);
        }
    }

    public void OnClick_ItemUseCntPlus()
    {
        _selectItemUseCnt++;
        if (_selectItemUseCnt >= _itemdata.Count)
            _selectItemUseCnt = _itemdata.Count;

        if (_selectItemUseCnt >= GameInfo.Instance.GameConfig.MatCount)
            _selectItemUseCnt = GameInfo.Instance.GameConfig.MatCount;

        kRunCntLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(222), _selectItemUseCnt);
    }

    public void OnClick_ItemUseCntMinus()
    {
        _selectItemUseCnt--;

        if (_selectItemUseCnt <= 0)
            _selectItemUseCnt = 1;

        kRunCntLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(222), _selectItemUseCnt);
    }

    public void OnClick_ItemUseCntMax()
    {
        _selectItemUseCnt = _itemdata.Count;

        if (_selectItemUseCnt >= GameInfo.Instance.GameConfig.MatCount)
            _selectItemUseCnt = GameInfo.Instance.GameConfig.MatCount;

        kRunCntLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(222), _selectItemUseCnt);
    }

    public void OnClick_ItemUseCnt()
    {
        if (_itemdata.TableData.SubType == (int)eITEMSUBTYPE.USE_AP_CHARGE || _itemdata.TableData.SubType == (int)eITEMSUBTYPE.USE_AP_CHARGE_NUM)
        {
            UserData userdata = GameInfo.Instance.UserData;
            int now = (int)userdata.Goods[(int)eGOODSTYPE.AP];
            int add = GameSupport.GetMaxAP() * _selectItemUseCnt;
            if ( _itemdata.TableData.SubType == (int)eITEMSUBTYPE.USE_AP_CHARGE_NUM ) {
				add = _itemdata.TableData.Value * _selectItemUseCnt;
			}

            string textName = string.Format(FLocalizeString.Instance.GetText(115), FLocalizeString.Instance.GetText(_itemdata.TableData.Name));
            string textDesc = string.Format(FLocalizeString.Instance.GetText(3211), FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTYPE_TEXT_START + (int)eGOODSTYPE.AP),
                now + add, GameSupport.GetMaxAP(), add);
            string text = string.Format("{0}\n{1}", textName, textDesc);

            MessagePopup.CYN(eTEXTID.TITLE_NOTICE, text, eTEXTID.YES, eTEXTID.NO, OnMsg_UseItemCnt);
            return;
        }
        else if (_itemdata.TableData.SubType == (int)eITEMSUBTYPE.USE_BP_CHARGE || _itemdata.TableData.SubType == (int)eITEMSUBTYPE.USE_BP_CHARGE_NUM)
        {
            UserData userdata = GameInfo.Instance.UserData;
            int now = (int)userdata.Goods[(int)eGOODSTYPE.BP];
            int add = _itemdata.TableData.Value * _selectItemUseCnt;
			if ( _itemdata.TableData.SubType == (int)eITEMSUBTYPE.USE_BP_CHARGE_NUM ) {
				add = _itemdata.TableData.Value * _selectItemUseCnt;
			}

            string textName = string.Format(FLocalizeString.Instance.GetText(115), FLocalizeString.Instance.GetText(_itemdata.TableData.Name));
            string textDesc = string.Format(FLocalizeString.Instance.GetText(3211), FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTYPE_TEXT_START + (int)eGOODSTYPE.BP),
                now + add, GameInfo.Instance.GameConfig.BPMaxCount, add);
            string text = string.Format("{0}\n{1}", textName, textDesc);

            MessagePopup.CYN(eTEXTID.TITLE_NOTICE, text, eTEXTID.YES, eTEXTID.NO, OnMsg_UseItemCnt);
            return;
        }
        else if (_itemdata.TableData.SubType == (int)eITEMSUBTYPE.USE_STORE_PACKAGE)
        {
            MessagePopup.CYN(eTEXTID.TITLE_NOTICE, string.Format(FLocalizeString.Instance.GetText(115), FLocalizeString.Instance.GetText(_itemdata.TableData.Name)), eTEXTID.YES, eTEXTID.NO, StorePackageItemOpen, null);
            return;
        }
        else if(_itemdata.TableData.SubType == (int)eITEMSUBTYPE.USE_ADDITEMSLOT)
        {
            InvenExpansionByItem();
            return;
        }

        OnMsg_UseItemCnt();
    }

    public void OnMsg_UseItemCnt()
    {
        _useitemuid = _itemdata.ItemUID;
        _useitemtableid = _itemdata.TableID;

        _precardbooklist.Clear();
        _precardbooklist.AddRange(GameInfo.Instance.CardBookList);

        GameInfo.Instance.Send_ReqUseItem(_itemdata.ItemUID, _selectItemUseCnt, 0, OnNetUseItem);
    }

    private void InvenExpansionByItem()
    {
        int now = GameInfo.Instance.UserData.ItemSlotCnt;
        int add = GameInfo.Instance.GameConfig.AddItemSlotCount * _selectItemUseCnt;

        if (now >= GameInfo.Instance.GameConfig.MaxItemSlotCount)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3079));
            return;
        }

        string textName = string.Format(FLocalizeString.Instance.GetText(118), FLocalizeString.Instance.GetText(1002));
        string textDesc = string.Format(FLocalizeString.Instance.GetText(3211), FLocalizeString.Instance.GetText(1002),
                                        now + add, GameInfo.Instance.GameConfig.MaxItemSlotCount, add);
        string text = string.Format("{0}\n{1}", textName, textDesc);

        MessagePopup.CYN(eTEXTID.TITLE_BUY, text, eTEXTID.YES, eTEXTID.NO, eGOODSTYPE.NONE, _selectItemUseCnt, CheckInventoryCount);
    }

    private void CheckInventoryCount()
    {
        int count = GameInfo.Instance.UserData.ItemSlotCnt + (GameInfo.Instance.GameConfig.AddItemSlotCount * _selectItemUseCnt);
        if(count > GameInfo.Instance.GameConfig.MaxItemSlotCount)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3079));
            return;
        }

        OnMsg_UseItemCnt();
    }

    private bool OnItemInfoTypeTabSelect(int nSelect, SelectEvent type)
    {
        _itemInfoType = (eItemInfoType)nSelect;

        kInfo.SetActive(_itemInfoType == eItemInfoType.Info);
        kAcquisition.SetActive(_itemInfoType == eItemInfoType.Get);

        return true;
    }

	public void SetGradeUpCharData(CharData charData)
	{
		mCharData = charData;
		GameInfo.Instance.Send_ReqUseItem(_itemdata.ItemUID, 1, mCharData.CUID, OnCharGradeUp);
	}

	private void OnUseCharacterGradeUpItem()
	{
		UIValue.Instance.SetValue(UIValue.EParamType.CharSeletePopupType, (int)eCharSelectFlag.USE_CHAR_GRADE_UP_ITEM);
		UIValue.Instance.SetValue(UIValue.EParamType.GradeUpValue, _itemdata.TableData.Value);

		LobbyUIManager.Instance.ShowUI("CharSeletePopup", true);
	}

	public void OnCharGradeUp(int result, PktMsgType pktmsg)
	{
		if (result != 0)
		{
			return;
		}

		PktInfoCharGradeExpLv pkt = pktmsg as PktInfoCharGradeExpLv;
		if(pkt == null)
		{
			return;
		}

		UIValue.Instance.SetValue(UIValue.EParamType.CharSelUID, (long)pkt.gradeUp_.cuid_);
		DirectorUIManager.Instance.PlayCharGradeUp(mCharData.TableID);

		UIResultCharGradeUpPopup gradeUpResultPopup = LobbyUIManager.Instance.GetUI<UIResultCharGradeUpPopup>();
		gradeUpResultPopup.SetPanelDepth(110);
        gradeUpResultPopup.SetConfirmAction(() =>
        {
            LobbyUIManager.Instance.CheckAddSpecialPopup();
            LobbyUIManager.Instance.ShowAddSpecialPopup();
        });

		LobbyUIManager.Instance.Renewal("TopPanel");
		LobbyUIManager.Instance.Renewal("GoodsPopup");

		UIItemPanel itempanel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
		if (itempanel != null)
		{
            itempanel.InitComponent();
        }

		Renewal(true);

		if(_itemdata != null && _itemdata.Count <= 0)
		{
			OnClickClose();
		}
	}

    public void SetGemEvolutionItemInfo(int targetDepth, string sortingLayerName, int sotringOrder, bool useSortingOrder)
    {
        if (uiPanel == null)
        {
            return;
        }

        uiPanel.depth = targetDepth + 10;
        uiPanel.sortingOrder = sotringOrder + 1;
        uiPanel.useSortingOrder = useSortingOrder;
        uiPanel.sortingLayerName = sortingLayerName;
    }
}
