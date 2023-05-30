using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PackageInfoPopup
{
	public static UIPackageInfoPopup GetPackageInfoPopup()
	{
		UIPackageInfoPopup popup = null;

		if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
			popup = LobbyUIManager.Instance.GetUI<UIPackageInfoPopup>("PackageInfoPopup");

		return popup;
	}

	public static void ShowPackageInfoPopup(int storeID, UIPackagePopup.ePackageUIType uiType, GameClientTable.StoreDisplayGoods.Param clientstoredata, UIPackageInfoPopup.OnClickBuyBtn callBackBuy)
	{
		UIPackageInfoPopup popup = GetPackageInfoPopup();
		if (popup == null)
			return;

		popup.ShowPackageInfoPopup(storeID, uiType, clientstoredata, callBackBuy);
		popup.ShowPopup();
	}
	
	public static void ShowPackageSubInfoPopup(int storeID, int subIndex, UIPackagePopup.ePackageUIType uiType, GameClientTable.StoreDisplayGoods.Param clientstoredata, UIPackageInfoPopup.OnClickBuyBtn callBackBuy)
	{
		UIPackageInfoPopup popup = GetPackageInfoPopup();
		if (popup == null)
			return;

		popup.ShowPackageSubInfoPopup(storeID, subIndex, uiType, clientstoredata, callBackBuy);
		popup.ShowPopup();
	}

	public static void ShowNotBuyPackageInfoPopup(int storeID, UIPackagePopup.ePackageUIType uiType, GameClientTable.StoreDisplayGoods.Param clientstoredata, UIPackageInfoPopup.OnClickBuyBtn callBackBuy)
	{
		UIPackageInfoPopup popup = GetPackageInfoPopup();
		if (popup == null)
			return;

		popup.ShowNotBuyPackageInfoPopup(storeID, uiType, clientstoredata, callBackBuy);
		popup.ShowPopup();
	}
}

public class UIPackageInfoPopup : FComponent
{
	public delegate void OnClickBuyBtn();
	private OnClickBuyBtn CallBackBuy;

    public UILabel kPackageNameLabel;
    public GameObject kDescriptionRoot;
    public UILabel kDescriptionLabel;

    [SerializeField] private FList _PackageInfoListInstance;

    public UIPriceUnit kPriceUnit;

    private List<GameTable.Random.Param> _packageInfoRandomList = new List<GameTable.Random.Param>();
	private GameTable.Store.Param _storeData;
    private System.DateTime _nowTime;
    private UIPackagePopup.ePackageUIType _uiType;
    private GameClientTable.StoreDisplayGoods.Param _clientStoreData;


	public void ShowPopup()
	{
		SetUIActive(true);
	}

	public void ShowPackageInfoPopup(int storeID, UIPackagePopup.ePackageUIType uiType, GameClientTable.StoreDisplayGoods.Param clientstoredata, OnClickBuyBtn callBackBuy)
	{
		if (this._PackageInfoListInstance == null) return;

		if(this._PackageInfoListInstance.EventUpdate == null)
			this._PackageInfoListInstance.EventUpdate = this._UpdatePackageInfoListSlot;
		if(this._PackageInfoListInstance.EventGetItemCount == null)
			this._PackageInfoListInstance.EventGetItemCount = this._GetPackageInfoElementCount;

        kPriceUnit.ParentGO = this.gameObject;

        _storeData = GameInfo.Instance.GameTable.FindStore(x => x.ID == storeID);
		_packageInfoRandomList.Clear();
		if (_storeData == null)
			return;

        _uiType = uiType;
        _clientStoreData = clientstoredata;

        _packageInfoRandomList = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == _storeData.ProductIndex && x.ProductType != (int)eREWARDTYPE.BUFF);
        
        _PackageInfoListInstance.UpdateList();

		CallBackBuy = callBackBuy;

        _nowTime = GameInfo.Instance.GetNetworkTime();

        kPackageNameLabel.textlocalize = FLocalizeString.Instance.GetText(_clientStoreData.Name);

        bool bDescCheck = true;
        switch (_clientStoreData.PackageUIType)
        {
	        case (int) UIPackagePopup.ePackageUIType.Scenario:
	        case (int) UIPackagePopup.ePackageUIType.Wellcome:
	        case (int) UIPackagePopup.ePackageUIType.WakeUp:
	        {
		        bDescCheck = false;
		        break;
	        }
        }
        
        if (bDescCheck && _clientStoreData.Description > (int)eCOUNT.NONE)
        {
            kDescriptionRoot.SetActive(true);
            kDescriptionLabel.textlocalize = FLocalizeString.Instance.GetText(_clientStoreData.Description);
        }
        else
        {
            kDescriptionRoot.SetActive(false);
        }
        
        kPriceUnit.UpdateSlot(_storeData.ID, _nowTime, _uiType);
        kPriceUnit.SetVisiblePremiumBtn(false);
	}
	
	public void ShowPackageSubInfoPopup(int storeID, int subIndex, UIPackagePopup.ePackageUIType uiType, GameClientTable.StoreDisplayGoods.Param clientstoredata, OnClickBuyBtn callBackBuy)
	{
		ShowPackageInfoPopup(storeID, uiType, clientstoredata, callBackBuy);
		_packageInfoRandomList = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == _storeData.ProductIndex && x.ProductType != (int)eREWARDTYPE.BUFF && x.Value == subIndex);
		_PackageInfoListInstance.UpdateList(true, false);
	}

	public void ShowNotBuyPackageInfoPopup(int storeID, UIPackagePopup.ePackageUIType uiType, GameClientTable.StoreDisplayGoods.Param clientstoredata, OnClickBuyBtn callBackBuy)
	{
		ShowPackageInfoPopup(storeID, uiType, clientstoredata, callBackBuy);
		kPriceUnit.kCommonLimitedObj.SetActive(false);
		kPriceUnit.SetPriceOffBtn(1715);
	}

	private void _UpdatePackageInfoListSlot(int index, GameObject slotObject)
	{
		do
		{
			UIPackageItemListSlot slot = slotObject.GetComponent<UIPackageItemListSlot>();
			if (null == slot) break;
			slot.ParentGO = this.gameObject;

			GameTable.Random.Param data = null;
			if (0 <= index && _packageInfoRandomList.Count > index)
				data = _packageInfoRandomList[index];

			slot.ParentGO = this.gameObject;
			slot.UpdateSlot(data);
		} while(false);
	}
	
	private int _GetPackageInfoElementCount()
	{
		if (_packageInfoRandomList == null)
			return (int)eCOUNT.NONE;

		return _packageInfoRandomList.Count;
	}

    public override bool IsBackButton()
    {
        if (IAPManager.Instance.IsBuying)
            return false;

        return base.IsBackButton();
    }

    public override void OnClickClose()
    {
        if (IAPManager.Instance.IsBuying)
            return;

        base.OnClickClose();
    }

    public void OnClick_CloseBtn()
	{
		OnClickClose();
	}

	
	public void OnClick_BGBtn()
	{
	}
	
	public void OnClick_BuyBtn()
	{
        if (CallBackBuy != null)
            CallBackBuy();
    }
}
