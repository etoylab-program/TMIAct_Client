using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemBuyMessagePopup
{
	public static UIItemBuyMessagePopup GetItemBuyMessagePopup()
    {
		UIItemBuyMessagePopup popup = null;

		if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
			popup = LobbyUIManager.Instance.GetUI<UIItemBuyMessagePopup>("ItemBuyMessagePopup");

		return popup;
    }

	public static void ShowItemBuyPopup(int itemuid, int nowCount, int matCount)
    {
		UIItemBuyMessagePopup popup = GetItemBuyMessagePopup();
		if (popup == null)
			return;

		popup.InitItemBuyPopup(itemuid, nowCount, matCount);
    }
}
public class UIItemBuyMessagePopup : FComponent
{
	[SerializeField] FList _ItemSendList;
	public UIItemListSlot kItemSlot;
	public UIGoodsUnit kGoodsUnit;
	public GameObject kGoodsBuyBtn;
	private int _roundNeedCash = 0;
	private int _needCnt;
	private GameTable.Item.Param _itemdata;
	private List<GameClientTable.Acquisition.Param> _infolist = new List<GameClientTable.Acquisition.Param>();
	public void InitItemBuyPopup(int itemuid, int nowCount, int matCount)
    {
		if(_ItemSendList.EventUpdate == null)
			this._ItemSendList.EventUpdate = this._UpdateItemSendListSlot;
		if(_ItemSendList.EventGetItemCount == null)
			this._ItemSendList.EventGetItemCount = this._GetItemSendElementCount;
		this._ItemSendList.InitBottomFixing();

		_itemdata = GameInfo.Instance.GameTable.FindItem(itemuid);

		kItemSlot.UpdateSlot(UIItemListSlot.ePosType.Mat, (int)eCOUNT.NONE, _itemdata);
		string strmatcount = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.RED_TEXT_COLOR, string.Format(FLocalizeString.Instance.GetText(236), nowCount, matCount)));
		kItemSlot.SetCountLabel(strmatcount);

		_infolist.Clear();
		_infolist = GameInfo.Instance.GameClientTable.FindAllAcquisition(x => x.GroupID == _itemdata.AcquisitionID);

		if(_itemdata.CashExchange > (int)eCOUNT.NONE)
        {
			kGoodsUnit.gameObject.SetActive(true);
			kGoodsBuyBtn.gameObject.SetActive(true);
			_needCnt = matCount - nowCount;
			float needCash = _needCnt * _itemdata.CashExchange;
			//_roundNeedCash = Mathf.RoundToInt(needCash);
			_roundNeedCash = Mathf.CeilToInt(needCash);

			if (kGoodsUnit != null)
				kGoodsUnit.InitGoodsUnit(eGOODSTYPE.CASH, _roundNeedCash, true);
		}
		else
        {
			kGoodsUnit.gameObject.SetActive(false);
			kGoodsBuyBtn.gameObject.SetActive(false);
        }

		

		_ItemSendList.UpdateList();
		this.SetUIActive(true);
    }
	
	private void _UpdateItemSendListSlot(int index, GameObject slotObject)
	{
		do
		{
			UIItmeSendListSlot slot = slotObject.GetComponent<UIItmeSendListSlot>();
			if (null == slot) break;
			slot.ParentGO = this.gameObject;

			GameClientTable.Acquisition.Param data = null;
			if (0 <= index && _infolist.Count > index)
				data = _infolist[index];
			slot.ParentGO = this.gameObject;
			slot.UpdateSlot(index, data);

		} while(false);
	}
	
	private int _GetItemSendElementCount()
	{
		return _infolist.Count; //TempValue
	}
	
	public void OnClick_CloseBtn()
	{
		OnClickClose();
	}
	
	public void OnClick_BuyBtn()
	{
		if (kGoodsUnit == null)
			return;

		if(GameSupport.IsCheckGoods(eGOODSTYPE.CASH, _roundNeedCash))
        {
			Log.Show("Item Buy!!!");
			GameInfo.Instance.Send_ReqItemExchangeCash(_itemdata.ID, _needCnt, OnNet_AckItemExchangeCash);

		}
	}
	public void OnClick_BGBtn()
	{
	}

	public void OnNet_AckItemExchangeCash(int result, PktMsgType pktmsg)
	{
		if (result != 0)
			return;

		MessageRewardListPopup.RewardListMessage(FLocalizeString.Instance.GetText((int)eTEXTID.REWARD_POPUP_TITLE), 
			FLocalizeString.Instance.GetText((int)eTEXTID.REWARD_POPUP_TEXT), 
			GameInfo.Instance.RewardList, 
			() => 
				{
					LobbyUIManager.Instance.Renewal("CardGradeUpPopup");
					LobbyUIManager.Instance.Renewal("CharGradeUpPopup");
					LobbyUIManager.Instance.Renewal("WeaponGradeUpPopup");
					LobbyUIManager.Instance.Renewal("FacilityItemCombinePopup");
					LobbyUIManager.Instance.Renewal("TopPanel");
					LobbyUIManager.Instance.Renewal("GoodsPopup");
					LobbyUIManager.Instance.Renewal("StageDetailPopup");
					LobbyUIManager.Instance.Renewal("BattleResultPopup");
					LobbyUIManager.Instance.Renewal("CostumeDyePopup");
					LobbyUIManager.Instance.Renewal("FacilityFunctionSelectPopup");
				}
			);

		OnClickClose();
	}
}
