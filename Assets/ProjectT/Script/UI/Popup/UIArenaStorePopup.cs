using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIArenaStorePopup : FComponent
{

	public FTab kMainTab;
	[SerializeField] private FList _StoreListSMInstance;

    private int _selectTab = 0;
    private int _arenaGrade = 1;
    private List<GameClientTable.StoreDisplayGoods.Param> _arenaStoreList = new List<GameClientTable.StoreDisplayGoods.Param>();
    private List<GameClientTable.StoreDisplayGoods.Param> _selectTabArenaStoreList = new List<GameClientTable.StoreDisplayGoods.Param>();
    private GameClientTable.StoreDisplayGoods.Param _sendstoredisplaytable;
    private List<CardBookData> _precardbooklist = new List<CardBookData>();

    public override void Awake()
	{
		base.Awake();

		kMainTab.EventCallBack = OnMainTabSelect;
		if(this._StoreListSMInstance == null) return;

        this._StoreListSMInstance.InitBottomFixing();
		this._StoreListSMInstance.EventUpdate = this._UpdateStoreListSMSlot;
		this._StoreListSMInstance.EventGetItemCount = this._GetStoreElementCount;
	}
 
	public override void OnEnable()
	{
        _selectTab = 0;
        object obj = UIValue.Instance.GetValue(UIValue.EParamType.ArenaStoreTab);
        if (obj != null)
            _selectTab = (int)obj;

        InitComponent();
		base.OnEnable();
	}

    public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		UIValue.Instance.RemoveValue(UIValue.EParamType.ArenaStoreTab);
        base.OnDisable();
    }

    public override void InitComponent()
	{
        _arenaStoreList = GameInfo.Instance.GameClientTable.FindAllStoreDisplayGoods(x => x.PanelType == (int)eSD_PanelType.ARENASTORE);
        _arenaGrade = 1;

        int arenaUserGradeId = GameInfo.Instance.UserBattleData.Now_GradeId;
        if (arenaUserGradeId == (int)eCOUNT.NONE)
            arenaUserGradeId = 1;

        GameTable.ArenaGrade.Param param = GameInfo.Instance.GameTable.ArenaGrades.Find(x => x.GradeID == arenaUserGradeId);
        if (param == null)
            return;

        _arenaGrade = param.Grade;

        if (GameInfo.Instance.GameConfig.TestMode || AppMgr.Instance.Review)
            _arenaGrade = 4;

        for (int i = 0; i < kMainTab.kBtnList.Count; i++)
        {
            GameClientTable.StoreDisplayGoods.Param arenaTabData = _arenaStoreList.Find(x => x.Category == (i + 1));
            if (arenaTabData == null)
            {
                kMainTab.DisableTab(i);
                continue;
            }

            GameObject lockObj = kMainTab.kBtnList[i].transform.Find("Lock").gameObject;
            if (lockObj != null)
                lockObj.SetActive(false);

            if (_arenaGrade >= arenaTabData.SubCategory)
            {
                kMainTab.SetEnabled(i, true);
                kMainTab.SetTabLabelAlpha(i, 1.0f);
            }
            else
            {
                kMainTab.SetEnabled(i, true);
                kMainTab.SetTabLabelAlpha(i, 0.25f);
            }
        }

        _selectTabArenaStoreList = GetStoreDisplayItems(_selectTab + 1);

        kMainTab.SetTab(_selectTab, SelectEvent.Code);
    }

    private bool OnMainTabSelect(int nSelect, SelectEvent type)
	{
        if (type == SelectEvent.Enable)
            return false;

        _selectTab = nSelect;
        _selectTabArenaStoreList = GetStoreDisplayItems(_selectTab + 1);
        
        _StoreListSMInstance.UpdateList();
        
		return true;
	}

    private List<GameClientTable.StoreDisplayGoods.Param> GetStoreDisplayItems(int selectSubCategory)
    {
        List<GameClientTable.StoreDisplayGoods.Param> resultList = new List<GameClientTable.StoreDisplayGoods.Param>();
        List<GameClientTable.StoreDisplayGoods.Param> tempList = _arenaStoreList.FindAll(x => x.SubCategory == selectSubCategory);
        for(int i = 0; i < tempList.Count; i++)
        {
            if (GameSupport.IsShowStoreDisplay(tempList[i]))
                resultList.Add(tempList[i]);
        }

        return resultList;
    }

	
	private void _UpdateStoreListSMSlot(int index, GameObject slotObject)
	{
        UIStoreListSlotSM card = slotObject.GetComponent<UIStoreListSlotSM>();
        if (card == null)
        {
            return;
        }
        
        if (index < 0 || _selectTabArenaStoreList.Count <= index)
        {
            return;
        }
        
        GameClientTable.StoreDisplayGoods.Param data = _selectTabArenaStoreList[index];
        card.ParentGO = this.gameObject;
        int temp = 1;
        card.UpdateSlot(index, data, UIItemListSlot.ePosType.ArenaStore, 1, ref temp);
    }
	
	private int _GetStoreElementCount()
	{
		return _selectTabArenaStoreList.Count; //TempValue
	}

    public void OnClick_BackBtn()
    {
        OnClickClose();
    }

    public void OnClickArenaStoreSlot(GameClientTable.StoreDisplayGoods.Param storedisplaytable, long purchasevalue)
    {
        var storetabledata = GameInfo.Instance.GameTable.FindStore(x => x.ID == storedisplaytable.StoreID);
        if(_arenaGrade < storedisplaytable.SubCategory)
        {
            GameTable.ArenaGrade.Param param = GameInfo.Instance.GameTable.ArenaGrades.Find(x => x.Grade == storedisplaytable.SubCategory);
            MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3173), FLocalizeString.Instance.GetText(param.Name)));
            return;
        }

        _precardbooklist.Clear();
        _precardbooklist.AddRange(GameInfo.Instance.CardBookList);

        _sendstoredisplaytable = storedisplaytable;

        StoreBuyPopup.Show(_sendstoredisplaytable, ePOPUPGOODSTYPE.ARENAPOINT_NONE_BTN, OnMsg_Purchase, OnMsg_Purchase_Cancel);
    }

    public void OnMsg_Purchase()
    {
        var storetabledata = GameInfo.Instance.GameTable.FindStore(x => x.ID == _sendstoredisplaytable.StoreID);
        if (storetabledata == null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3009));
            return;
        }
        if (!GameSupport.IsCheckInven())
            return;

        int itemCount = StoreBuyPopup.GetItemCount();

        if ((eGOODSTYPE)storetabledata.PurchaseIndex != eGOODSTYPE.NONE && (eGOODSTYPE)storetabledata.PurchaseIndex != eGOODSTYPE.GOODS)
        {
            if (!GameSupport.IsCheckGoods((eGOODSTYPE)storetabledata.PurchaseIndex, storetabledata.PurchaseValue * itemCount))
                return;
        }

        GameInfo.Instance.Send_ReqStorePurchase(_sendstoredisplaytable.StoreID, false, itemCount, OnNetPurchase);
    }

    public void OnMsg_Purchase_Cancel()
    {

    }

    public void OnNetPurchase(int result, PktMsgType pktmsg)
    {
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.Renewal("ArenaMainPanel");

        _selectTabArenaStoreList = GetStoreDisplayItems(_selectTab + 1);
        _StoreListSMInstance.RefreshNotMoveAllItem();
        
        if (_sendstoredisplaytable.DrtType != 0)
        {
            DirectorUIManager.Instance.PlayRewardOpen(FLocalizeString.Instance.GetText((int)eTEXTID.REWARD_POPUP_TITLE), FLocalizeString.Instance.GetText((int)eTEXTID.REWARD_POPUP_TEXT), GameInfo.Instance.RewardList);
        }
        else
        {
            var storetabledata = GameInfo.Instance.GameTable.FindStore(x => x.ID == _sendstoredisplaytable.StoreID);
            if (storetabledata != null)
            {
                long PurchaseValue = storetabledata.PurchaseValue;   //재화 수량
                int ProductValue = storetabledata.ProductValue * StoreBuyPopup.GetItemCount();      //상품 수량

                string productName = string.Empty;

                if ((eREWARDTYPE)storetabledata.ProductType == eREWARDTYPE.PACKAGE)        //묶음상품
                {
                    productName = FLocalizeString.Instance.GetText(1307);
                }
                else
                {
                    RewardData reward = new RewardData(0, storetabledata.ProductType, storetabledata.ProductIndex, ProductValue, false);
                    productName = GameSupport.GetProductName(reward);
                }

                if ((eREWARDTYPE)storetabledata.ProductType == eREWARDTYPE.CHAR)           //캐릭터
                {
                    DirectorUIManager.Instance.PlayCharBuy(storetabledata.ProductIndex, null);
                }
                else if ((eREWARDTYPE)storetabledata.ProductType == eREWARDTYPE.CARD)           //캐릭터
                {
                    var cardbookdata = _precardbooklist.Find(x => x.TableID == storetabledata.ProductIndex);
                    if (cardbookdata == null)
                    {
                        DirectorUIManager.Instance.PlayNewCardGreeings(_precardbooklist);
                    }
                    else
                    {
                        string str = FLocalizeString.Instance.GetText(3049, productName);
                        MessageToastPopup.Show(str, true);
                    }
                }
                else
                {
                    //  상품 구매 메세지
                    string str = FLocalizeString.Instance.GetText(3049, productName);
                    MessageToastPopup.Show(str, true);
                }
            }
        }
    }
}
