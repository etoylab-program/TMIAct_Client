using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICashBuyPopup : FComponent
{
	public UIButton kCloseBtn;
	public UISprite kBgSpr;
	public UISprite kicoSpr;
	public UILabel khaveTotalLabel;
	public UILabel kPayTotalLabel;
	public UILabel kFreeTotalLabel;
	public UIButton klaw1Btn;
	public UIButton klaw2Btn;
	[SerializeField] private FList _cashlistinstance;

    private List<GameClientTable.StoreDisplayGoods.Param> _storedisplaylist = new List<GameClientTable.StoreDisplayGoods.Param>();
    private const int _category = 0;
    private GameClientTable.StoreDisplayGoods.Param _sendstoredisplaytable;

    //private bool m_iapBuying = false;

    public override void Awake()
	{
		base.Awake();

        if(this._cashlistinstance == null) return;
		
		this._cashlistinstance.EventUpdate = this._UpdateCashListSlot;
		this._cashlistinstance.EventGetItemCount = this._GetCashElementCount;
	}
 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        List<GameClientTable.StoreDisplayGoods.Param> list = new List<GameClientTable.StoreDisplayGoods.Param>();

        //월정액 상품 먼저 추가
        List<GameClientTable.StoreDisplayGoods.Param> monthList = GameInfo.Instance.GameClientTable.FindAllStoreDisplayGoods(x => x.PanelType == (int)eSD_PanelType.CASH && x.SubCategory == 0 && x.ShowHaveStoreID == (int)eCOUNT.NONE);
        for (int i = 0; i < monthList.Count; i++)
        {
            if (!GameSupport.IsShowStoreDisplay(monthList[i]))
            {
                //월정액 끝났는지 확인 미리하기?
                //GameTable.Store.Param storedata = GameInfo.Instance.GameTable.FindStore(x => x.ID == monthList[i].StoreID);
                //if (GameSupport.IsHaveMonthlyData(storedata.ProductIndex))
                //{
                    
                //}
                //else
                //{
                    
                //}

                var showData = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.PanelType == (int)eSD_PanelType.CASH && x.ShowHaveStoreID == monthList[i].StoreID);
                if (showData != null)
                    list.Add(showData);
            }
            else
            {
                list.Add(monthList[i]);
            }
        }

        //210812 - Review체크 X
        List<GameClientTable.StoreDisplayGoods.Param> vlist = GameInfo.Instance.GameClientTable.FindAllStoreDisplayGoods(x => x.PanelType == (int)eSD_PanelType.CASH && x.SubCategory == 1 && x.ShowHaveStoreID == 0);
        for (int i = 0; i < vlist.Count; i++)
        {
            list.Add(vlist[i]);
        }


        _storedisplaylist.Clear();

        khaveTotalLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_W), GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.CASH]);

        //유상 대마석 소지수
        kPayTotalLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_W), GameInfo.Instance.UserData.HardCash);

        //무상 대마석 소지수
        kFreeTotalLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_W), GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.CASH] - GameInfo.Instance.UserData.HardCash);

        for (int i = 0; i < GameInfo.Instance.StoreList.Count; i++)
        {
            Log.Show(GameInfo.Instance.StoreList[i].TableID);
        }

        //210812 - Review체크 X
        for (int i = 0; i < list.Count; i++)
        {
            if (!GameSupport.IsShowStoreDisplay(list[i]))
            {
                var showData = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.PanelType == (int)eSD_PanelType.CASH && x.ShowHaveStoreID == list[i].StoreID);
                if (showData != null)
                {
                    _storedisplaylist.Add(showData);
                }
                else
                {
                    _storedisplaylist.Add(list[i]);
                }
            }
            else
            {
                _storedisplaylist.Add(list[i]);
            }
        }
        this._cashlistinstance.UpdateList();
        IAPManager.Instance.IsBuying = false;

        // 자금 결제법 버튼은 일본버전에서만 보여야한다.
        if (AppMgr.Instance.configData.m_ServiceType != Config.eServiceType.Japan)
        {
            klaw1Btn?.gameObject.SetActive(false);
            klaw2Btn?.gameObject.SetActive(false);
        }
        else
        {
            klaw1Btn?.gameObject.SetActive(true);
            klaw2Btn?.gameObject.SetActive(true);
        }
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);
        if (!IAPManager.Instance.IAPNULL_CHECK())
        {
            MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3197), null);
            return;
        }
    }
	
	private void _UpdateCashListSlot(int index, GameObject slotObject)
	{
		do
		{
            UICashSlot slot = slotObject.GetComponent<UICashSlot>();
            GameClientTable.StoreDisplayGoods.Param data = null;
            if (0 <= index && _storedisplaylist.Count > index)
            {
                data = _storedisplaylist[index];
            }
            Debug.Log(data.StoreID);
            slot.ParentGO = this.gameObject;
            slot.UpdateSlot(index, data);
            //if(index >= 0 && )
            //Do UpdateListSlot
            
		}while(false);
	}
	
	private int _GetCashElementCount()
	{
		return _storedisplaylist.Count; //TempValue
	}
	
	public void OnClick_CloseBtn()
	{        
        if (IAPManager.Instance.IsBuying)
            return;
        OnClickClose();
	}

    public override void OnClickClose()
    {
        if (IAPManager.Instance.IsBuying)
            return;
        base.OnClickClose();
    }

    public void OnClick_law1Btn()           //현금 결제법
    {
        if (IAPManager.Instance.IsBuying)
            return;
        if (string.IsNullOrEmpty(GameInfo.Instance.GameConfig.WebCashpaymentmethod))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3001));
        }
        else
        {
            GameSupport.OpenWebView(FLocalizeString.Instance.GetText(504), GameInfo.Instance.GameConfig.WebCashpaymentmethod);
        }
    }
	
	public void OnClick_law2Btn()           //특정 상거래법
    {
        if (IAPManager.Instance.IsBuying)
            return;
        if (string.IsNullOrEmpty(GameInfo.Instance.GameConfig.WebSpecifiedCommercialTransactionsAct))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3001));
        }
        else
        {
            GameSupport.OpenWebView(FLocalizeString.Instance.GetText(505), GameInfo.Instance.GameConfig.WebSpecifiedCommercialTransactionsAct);
        }
    }

    public void OnClick_Slot(GameClientTable.StoreDisplayGoods.Param storedisplaytable, long purchasevalue)
    {
        if (IAPManager.Instance.IsBuying)
            return;
        _sendstoredisplaytable = storedisplaytable;
        StoreBuyPopup.Show(storedisplaytable, ePOPUPGOODSTYPE.MPOINT_NONE_BTN, OnMsg_Purchase, OnMsg_Purchase_Cancel);
    }

    public void OnIAPBuySuccess()
    {
        var storetabledata = GameInfo.Instance.GameTable.FindStore(x => x.ID == _sendstoredisplaytable.StoreID);
        if (storetabledata == null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3009));
            return;
        }

        if ((eGOODSTYPE)storetabledata.PurchaseIndex != eGOODSTYPE.NONE && (eGOODSTYPE)storetabledata.PurchaseIndex != eGOODSTYPE.GOODS)
        {
            if (!GameSupport.IsCheckGoods((eGOODSTYPE)storetabledata.PurchaseIndex, storetabledata.PurchaseValue))
                return;
        }

#if !DISABLESTEAMWORKS
        GameInfo.Instance.Send_ReqSteamPurchase(_sendstoredisplaytable.StoreID, OnNetPurchase);
#else
        PlayerPrefs.SetInt("IAPBuyStoreID", _sendstoredisplaytable.StoreID);
        PlayerPrefs.Save();
#if UNITY_EDITOR
        GameInfo.Instance.Send_ReqStorePurchase(_sendstoredisplaytable.StoreID, false, 1, OnNetPurchase);
#else
        GameInfo.Instance.Send_ReqStorePurchaseInApp(IAPManager.Instance.Receipt, _sendstoredisplaytable.StoreID, OnNetPurchase);
#endif


#endif
    }

    public void OnIAPBuyFailed()
    {
        WaitPopup.Hide();
        IAPManager.Instance.IsBuying = false;

        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3212));
    }

    public void OnMsg_Purchase()
    {
        GameTable.Store.Param storeParam = GameInfo.Instance.GameTable.FindStore(x => x.ID == _sendstoredisplaytable.StoreID);
        if (storeParam == null)
        {
            Debug.LogError("StoreParam is null");
            return;
        }
        WaitPopup.Show();
        IAPManager.Instance.IsBuying = true;
#if UNITY_ANDROID
        Debug.Log(storeParam.ID + " / " + storeParam.AOS_ID);
        IAPManager.Instance.BuyIAPProduct(storeParam.AOS_ID, OnIAPBuySuccess, OnIAPBuyFailed);
#elif UNITY_IOS
        Debug.Log(storeParam.ID + " / " + storeParam.IOS_ID);
        IAPManager.Instance.BuyIAPProduct(storeParam.IOS_ID, OnIAPBuySuccess, OnIAPBuyFailed);
#elif !DISABLESTEAMWORKS
        Debug.Log(storeParam.ID + " / " + storeParam.ID);
        IAPManager.Instance.BuyIAPProduct(storeParam.ID.ToString(), OnIAPBuySuccess, OnIAPBuyFailed);
#endif
    }

    public void OnMsg_Purchase_Cancel()
    {
        WaitPopup.Hide();
        IAPManager.Instance.IsBuying = false;
    }

    public void OnNetPurchase(int result, PktMsgType pktmsg)
    {
        Log.Show("OnNetPurchase");
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.Renewal("ApBpRecoverySelectPopup");
        InitComponent();

        Renewal(true);

        //구매 결과 응답을 받으면 제거
        PlayerPrefs.DeleteKey("IAPBuyReceipt");
        PlayerPrefs.DeleteKey("IAPBuyStoreID");
        PlayerPrefs.Save();

        IAPManager.Instance.ResetReceipt();

        WaitPopup.Hide();
        IAPManager.Instance.IsBuying = false;

        if (GameSupport.IsRewardTypeWithStoreDisplayTable(_sendstoredisplaytable) == eREWARDTYPE.MONTHLYFEE)
        {
            MessageRewardListPopup.RewardListMessage(FLocalizeString.Instance.GetText(1600), FLocalizeString.Instance.GetText(1601), GameInfo.Instance.RewardList);
        }
        else
        {
            MessageRewardListPopup.RewardListMessage(FLocalizeString.Instance.GetText(3213), FLocalizeString.Instance.GetText(3214), GameInfo.Instance.RewardList);
        }
    }

    public override bool IsBackButton()
    {
        if (IAPManager.Instance.IsBuying)
            return false;

        return base.IsBackButton();
    }
}
