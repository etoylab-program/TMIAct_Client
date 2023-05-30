using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PackageBuyPopup
{
	public static UIPackageBuyPopup GetPackageBuyPopup()
	{
		UIPackageBuyPopup popup = null;

		if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
			popup = LobbyUIManager.Instance.ShowUI("PackageBuyPopup", true) as UIPackageBuyPopup;

		return popup;
	}

	public static void ShowPackageBuyPopup(int storeID, List<GameTable.Random.Param> overlabList, string titleStr, string okStr, string cancelStr, UIPackageBuyPopup.OnClickOKCancelCallBack okcallback, UIPackageBuyPopup.OnClickOKCancelCallBack cancelCallBack)
	{
		UIPackageBuyPopup popup = GetPackageBuyPopup();
		if (popup == null)
			return;

		popup.InitPackageBuyPopup(storeID, overlabList, titleStr, okStr, cancelStr, okcallback, cancelCallBack);
	}
}

public class UIPackageBuyPopup : FComponent
{
	[Serializable]
	public class RewardItemSlot
	{
		public GameObject itemObj;
		public List<UIRewardListSlot> beforeSlot;
		public List<UIRewardListSlot> afterSlot;
	}
	
	public UILabel kTitleLb;
	public UILabel kDescLb;

	public UIButton kYesBtn;
	public UIButton kNoBtn;
	public UILabel kYesLb;
	public UILabel kNoLb;

	public UIButton kCloseBtn;
	public List<RewardItemSlot> kRewardItemSlotList;

	public delegate void OnClickOKCancelCallBack();
	private OnClickOKCancelCallBack CallBackOK;
	private OnClickOKCancelCallBack CallBackCancel;

	private int _storeID;


	public void InitPackageBuyPopup(int storeID, List<GameTable.Random.Param> overlabList, string titleStr, string okStr, string cancelStr, OnClickOKCancelCallBack okCallBack, OnClickOKCancelCallBack cancelCallBack)
	{
		_storeID = storeID;

		CallBackOK = okCallBack;
		CallBackCancel = cancelCallBack;

		GameTable.Store.Param storeTableData = GameInfo.Instance.GameTable.FindStore(x => x.ID == _storeID);
		if (storeTableData == null)
		{
			CallBackOK = null;
			CallBackCancel = null;
			base.OnClickClose();
			return;
		}

		GameClientTable.StoreDisplayGoods.Param storeDisplayGoods = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == _storeID && x.PackageUIType == (int)eCOUNT.NONE);

		kTitleLb.textlocalize = titleStr;
		kYesLb.textlocalize = okStr;
		kNoLb.textlocalize = cancelStr;
		kDescLb.textlocalize = FLocalizeString.Instance.GetText(3215, FLocalizeString.Instance.GetText(storeDisplayGoods.Name));

		int index = overlabList.Count - 1;
		for (int i = 0; i < kRewardItemSlotList.Count; i++)
		{
			RewardItemSlot slot = kRewardItemSlotList[i];
			slot.itemObj.SetActive(index == i);
			if (!slot.itemObj.activeSelf)
			{
				continue;
			}

			bool bSumAfterValue = slot.beforeSlot.Count != slot.afterSlot.Count;
			for (int o = 0; o < overlabList.Count; o++)
			{
				GameTable.Random.Param param = overlabList[o];
				RewardData rData = new RewardData(param.ProductType, param.ProductIndex, param.Value);
				if (o < slot.beforeSlot.Count)
				{
					slot.beforeSlot[o].UpdateSlot(rData, false);
				}

				if (bSumAfterValue)
				{
					SetRewardSlotData(rData, slot.afterSlot[0], storeTableData, o + 1);
				}
				else
				{
					if (o < slot.afterSlot.Count)
					{
						SetRewardSlotData(rData, slot.afterSlot[o], storeTableData);
					}
				}
			}
		}
	}

	private void SetRewardSlotData(RewardData rData, UIRewardListSlot slot, GameTable.Store.Param storeTableData, int multi = 1)
	{
		if (rData.GetRewardType == eREWARDTYPE.CHAR)
		{
			slot.UpdateSlot(new RewardData((int)eREWARDTYPE.GOODS, (int)eGOODSTYPE.CASH, storeTableData.Value1 * multi), false, false);
		}
		else if (rData.GetRewardType == eREWARDTYPE.COSTUME)
		{
			slot.UpdateSlot(new RewardData((int)eREWARDTYPE.GOODS, (int)eGOODSTYPE.CASH, storeTableData.Value2 * multi), false, false);
		}
	}

	public void OnClick_YesBtn()
	{
		SetUIActive(false);
		if (CallBackOK != null)
			CallBackOK();
	}
	
	public void OnClick_NoBtn()
	{
		SetUIActive(false);
		if (CallBackCancel != null)
			CallBackCancel();
	}
	
	public void OnClick_CloseBtn()
	{
		SetUIActive(false);
		if (CallBackCancel != null)
			CallBackCancel();
	}

	
	public void OnClick_BGBtn()
	{
		SetUIActive(false);
		if (CallBackCancel != null)
			CallBackCancel();
	}
}
