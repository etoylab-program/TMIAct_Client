using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIDesireShopListSlot : FSlot 
{
	public UIRewardListSlot kRewardListSlot;
	public UILabel kNameLabel;
	public UILabel kCountLabel;
	public UISprite kIconSpr;
	public UISprite kLimitedSpr;

	public UIGoodsUnit kGoodsUnit;

	public UISprite kDimdSpr;
	public UILabel kDimdLabel;

	public UILabel kDateLabel;

	public GameObject kLimitedObj;
	public UILabel kLimitedLabel;

	private GameTable.Store.Param _storeData;
	private GameClientTable.StoreDisplayGoods.Param _displayData;

	private bool _cashFlag = false;

	private List<CardBookData> _precardbooklist = new List<CardBookData>();

	public void UpdateSlot(GameTable.Store.Param data) 	//Fill parameter if you need
	{
		_storeData = data;

		kLimitedSpr.SetActive(false);
		kDateLabel.SetActive(false);
		kDimdSpr.SetActive(false);
		kLimitedObj.SetActive(false);
		if (_storeData == null)
			return;

		_displayData = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == _storeData.ID);
		if (_displayData == null)
			return;

		RewardData rewardData = new RewardData(_storeData.ProductType, _storeData.ProductIndex, _storeData.ProductValue);

		kNameLabel.textlocalize = GameSupport.GetProductName(rewardData);
		
		kIconSpr.SetActive(_storeData.NeedDesirePoint != 0);
		kCountLabel.SetActive(kIconSpr.gameObject.activeSelf);

		if (kIconSpr.gameObject.activeSelf)
		{
			eTEXTID eTextColor = eTEXTID.RED_TEXT_COLOR;
			if (_storeData.NeedDesirePoint <= GameInfo.Instance.UserData.Goods[(int) eGOODSTYPE.DESIREPOINT])
			{
				eTextColor = eTEXTID.WHITE_TEXT_COLOR;
			}
			kCountLabel.textlocalize = FLocalizeString.Instance.GetText((int)eTextColor, _storeData.NeedDesirePoint.ToString("#,##0"));
		}
		
		kRewardListSlot.UpdateSlot(rewardData, true);
		kGoodsUnit.SetActive(true);
		kGoodsUnit.InitGoodsUnit((eGOODSTYPE)_storeData.PurchaseIndex, _storeData.PurchaseValue, true);

		//날짜 체크
		if (_storeData.SaleType == (int)eStoreSaleKind.LimitDate)
		{
			GachaCategoryData gachaCategoryData = GameInfo.Instance.ServerData.GachaCategoryList.Find(x => x.StoreID1 == _storeData.ConnectStoreID || x.StoreID2 == _storeData.ConnectStoreID);
			if (gachaCategoryData != null)
			{
				kDateLabel.SetActive(true);
				kLimitedSpr.SetActive(true);
				kDateLabel.textlocalize = GameSupport.GetEndTime(GameSupport.GetMinusOneMinuteEndTime(gachaCategoryData.EndDate));
			}
		}
		else if (_storeData.SaleType == (int)eStoreSaleKind.LimitCnt)		//구매횟수 제한이지만, 염원의상점에서만 기간제로 기능하기
		{
			GachaCategoryData gachaCategoryData = GameInfo.Instance.ServerData.GachaCategoryList.Find(x => x.StoreID1 == _storeData.ID || x.StoreID2 == _storeData.ID);
			if (gachaCategoryData != null)
			{
				kDateLabel.SetActive(true);
				kLimitedSpr.SetActive(true);
				kDateLabel.textlocalize = GameSupport.GetEndTime(GameSupport.GetMinusOneMinuteEndTime(gachaCategoryData.EndDate));
			}
		}

		//구매 횟수 체크
		StoreSaleData saleData = GameInfo.Instance.ServerData.StoreSaleList.Find(x => x.TableID == _storeData.ID);
		if (saleData != null)
		{
			if (saleData.LimitType == (int)eStoreSaleKind.LimitCnt)
			{
				if (GameSupport.GetLimitedCnt(_storeData.ID) > 0)
				{
					kLimitedObj.SetActive(true);
					kLimitedLabel.textlocalize = FLocalizeString.Instance.GetText(1584, GameSupport.GetLimitedCnt(_storeData.ID));
				}
				else
				{
					kGoodsUnit.SetActive(false);
					kDimdSpr.SetActive(true);
					kDimdLabel.textlocalize = FLocalizeString.Instance.GetText(1580);
				}
			}
		}
	}
 
	public void OnClick_Slot()
	{
		if (kDimdSpr.gameObject.activeSelf)
			return;

        if (!GameSupport.IsCheckGoods((eGOODSTYPE)_storeData.PurchaseIndex, _storeData.PurchaseValue))
        {
            return;
        }

        if (!GameSupport.IsCheckGoods(eGOODSTYPE.DESIREPOINT, _storeData.NeedDesirePoint))
        {
            return;
        }

        StoreBuyPopup.Show(_displayData, ePOPUPGOODSTYPE.DESIREPOINT_NONE_BTN, OnMsg_Purchase, OnMsg_Purchase_Cancel);
	}

	public void OnMsg_Purchase()
	{
		var storetabledata = GameInfo.Instance.GameTable.FindStore(x => x.ID == _displayData.StoreID);
		if (storetabledata == null)
		{
			MessageToastPopup.Show(FLocalizeString.Instance.GetText(3009));
			return;
		}

		if (GameSupport.HasStoreProduct((eREWARDTYPE)storetabledata.ProductType, storetabledata.ProductIndex))
		{
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
        if (!GameSupport.IsCheckGoods(eGOODSTYPE.DESIREPOINT, storetabledata.NeedDesirePoint * itemCount))
            return;

        _precardbooklist.Clear();
		_precardbooklist.AddRange(GameInfo.Instance.CardBookList);

		GameInfo.Instance.Send_ReqStorePurchase(_displayData.StoreID, false, itemCount, OnNetPurchase);
	}

	public void OnMsg_Purchase_Cancel()
	{

	}

	public void OnNetPurchase(int result, PktMsgType pktmsg)
	{
		LobbyUIManager.Instance.Renewal("TopPanel");
		LobbyUIManager.Instance.Renewal("GoodsPopup");
		LobbyUIManager.Instance.Renewal("GachaPanel");
		//InitComponent();
		//Renewal(true);

		var storetabledata = GameInfo.Instance.GameTable.FindStore(x => x.ID == _displayData.StoreID);
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
