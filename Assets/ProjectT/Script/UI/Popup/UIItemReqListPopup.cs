using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemReqListPopup
{
	public static UIItemReqListPopup GetItemReqListPopupPopup()
	{
		UIItemReqListPopup popup = null;
		if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
			popup = LobbyUIManager.Instance.GetUI<UIItemReqListPopup>("ItemReqListPopup");

		return popup;
	}

	public static void ShowItemReqListPopup(int titleNum, int descNum, GameTable.ItemReqList.Param data, UIItemReqListPopup.OnClickOKCallBack okCallBack)
	{
		UIItemReqListPopup popup = null;
		popup = GetItemReqListPopupPopup();
		if (popup == null)
			return;

		popup.InitItemReqListPopup(titleNum, descNum, data, okCallBack);
	}

	public static void ShowItemReqListPopup(string title, string desc, GameTable.ItemReqList.Param data, UIItemReqListPopup.OnClickOKCallBack okCallBack)
	{
		UIItemReqListPopup popup = null;
		popup = GetItemReqListPopupPopup();
		if (popup == null)
			return;

		popup.InitItemReqListPopup(title, desc, data, okCallBack);
	}
}

public class UIItemReqListPopup : FComponent
{
	public UILabel kTitleLabel;
	public UILabel kDescLabel;
	[SerializeField] private FList _ItemListInstance;
	public UIButton kYesBtn;
	public UIButton kNoBtn;
	public UIButton kCloseBtn;

	public delegate void OnClickOKCallBack();
	public OnClickOKCallBack CallBackOK;

	private List<RewardData> _rewardDataList = new List<RewardData>();
	private GameTable.ItemReqList.Param _itemReqList = null;

	public override void Awake()
	{
		base.Awake();

		if(this._ItemListInstance == null) return;
		
		this._ItemListInstance.EventUpdate = this._UpdateItemListSlot;
		this._ItemListInstance.EventGetItemCount = this._GetItemElementCount;
	}
 
	public void InitItemReqListPopup(int titleNum, int descNum, GameTable.ItemReqList.Param data, OnClickOKCallBack okCallback)
	{
		SetUIActive(true);

		kTitleLabel.textlocalize = FLocalizeString.Instance.GetText(titleNum);
		kDescLabel.textlocalize = FLocalizeString.Instance.GetText(descNum);

		CallBackOK = okCallback;

		_itemReqList = data;

		SetItemReqList();
	}

	public void InitItemReqListPopup(string title, string desc, GameTable.ItemReqList.Param data, OnClickOKCallBack okCallback)
	{
		SetUIActive(true);

		kTitleLabel.textlocalize = title;
		kDescLabel.textlocalize = desc;

		CallBackOK = okCallback;

		_itemReqList = data;

		SetItemReqList();
	}

	private void SetItemReqList()
	{
		_rewardDataList.Clear();

		AddRewardItem(_itemReqList.ItemID1, _itemReqList.Count1);
		AddRewardItem(_itemReqList.ItemID2, _itemReqList.Count2);
		AddRewardItem(_itemReqList.ItemID3, _itemReqList.Count3);
		AddRewardItem(_itemReqList.ItemID4, _itemReqList.Count4);

		if (_itemReqList.Gold > (int)eCOUNT.NONE)
			_rewardDataList.Add(new RewardData((int)eREWARDTYPE.GOODS, (int)eGOODSTYPE.GOLD, _itemReqList.Gold));

		if(_itemReqList.GoodsValue > (int)eCOUNT.NONE)
			_rewardDataList.Add(new RewardData((int)eREWARDTYPE.GOODS, (int)eGOODSTYPE.CASH, _itemReqList.GoodsValue));

		_ItemListInstance.SetListInactive();
		_ItemListInstance.ScrollView.contentPivot = _rewardDataList.Count > 4 ? UIWidget.Pivot.Left : UIWidget.Pivot.Center;

		_ItemListInstance.UpdateList();
	}

	private void AddRewardItem(int itemID, int itemCnt)
	{
		if (itemID == -1)
			return;

		RewardData rewardData = new RewardData((int)eREWARDTYPE.ITEM, itemID, itemCnt);
		_rewardDataList.Add(rewardData);
	}

	private void _UpdateItemListSlot(int index, GameObject slotObject)
	{
		do
		{
			UIRewardListSlot slot = slotObject.GetComponent<UIRewardListSlot>();
			if (null == slot) break;
			slot.ParentGO = this.gameObject;

			if (0 <= index && _rewardDataList.Count > index)
			{
				slot.UpdateSlot(_rewardDataList[index], false);
				
				if (_rewardDataList[index].Type == (int)eREWARDTYPE.GOODS)
				{
					long goodscnt = GameInfo.Instance.UserData.Goods[_rewardDataList[index].Index];
					string strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR);
					if (goodscnt < _rewardDataList[index].Value)
						strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.RED_TEXT_COLOR);

					string strmatcount = string.Format(strHaveCntColor, _rewardDataList[index].Value);
					slot.kCountLabel.textlocalize = strmatcount;
				}
				else
				{
					long orgcut = 0;
					orgcut = GameInfo.Instance.GetItemIDCount(_rewardDataList[index].Index);

					int orgmax = _rewardDataList[index].Value;
					string strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR);
					if (orgcut < orgmax)
					{
						strHaveCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.RED_TEXT_COLOR);
					}
					string strmatcount = string.Format(strHaveCntColor, string.Format(FLocalizeString.Instance.GetText(236), orgcut, orgmax));
					slot.kCountLabel.textlocalize = strmatcount;
				}
				
			}

		} while(false);
	}
	
	private int _GetItemElementCount()
	{
		return _rewardDataList.Count;
	}
	
	public void OnClick_YesBtn()
	{
		if (CallBackOK != null)
			CallBackOK();

		OnClickClose();
	}
	
	public void OnClick_NoBtn()
	{
		OnClickClose();
	}
	
	public void OnClick_CloseBtn()
	{
		OnClickClose();
	}

	
	public void OnClick_BGBtn()
	{
		OnClickClose();
	}
}
