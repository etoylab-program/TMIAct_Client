using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIEventStoreListSlot : FSlot 
{
	public UIRewardListSlot kRewardListSlot;
	public UILabel kNameLabel;
	public UILabel kCountLabel;
	public UISprite kIconSpr;
	public UISprite kLimitedSpr;

	public UIGoodsUnit kBasicGoodsUnit;

	public GameObject kDiscountObj;
	public UIGoodsUnit kDiscountGoodsUnit;
	public UILabel kDiscountRateLabel;
	public UILabel kDiscountOriginValueLabel;
	public GameObject kDiscountRedLineObj;

	public UISprite kDimdSpr;
	public UILabel kDimdLabel;

	public UILabel kDateLabel;

	public GameObject kRemainTimeObj;
	public UILabel kRemainTimeLabel;

	public GameObject kLimitedObj;
	public UILabel kLimitedLabel;	

	[Header( "[For raid sotre]" )]
	[SerializeField] private GameObject _ChangeBtnObj;
	[SerializeField] private UISprite[] _DateLimitSpr;

	private GameTable.Store.Param _storeData;
	private GameClientTable.StoreDisplayGoods.Param _displayData;
	private bool _cashFlag = false;
	private List<CardBookData> _precardbooklist = new List<CardBookData>();
	private DateTime RemainTime = DateTime.MinValue;
	private StoreSaleData storeSaleData = null;
	private bool mDisable = false;


	public void UpdateSlot(GameTable.Store.Param data) 	//Fill parameter if you need
	{
		_storeData = data;

		kLimitedSpr.SetActive(true);

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

		if (_storeData.NeedDesirePoint <= GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.DESIREPOINT])
			kCountLabel.textlocalize = _storeData.NeedDesirePoint.ToString("#,##0");
		else
			kCountLabel.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.RED_TEXT_COLOR, _storeData.NeedDesirePoint.ToString("#,##0"));

		kRewardListSlot.UpdateSlot(rewardData, true);
		kBasicGoodsUnit.SetActive(true);
		kBasicGoodsUnit.InitGoodsUnit((eGOODSTYPE)_storeData.PurchaseIndex, _storeData.PurchaseValue, true);

		//날짜 체크
		if (_storeData.SaleType == (int)eStoreSaleKind.LimitDate)
		{
			GachaCategoryData gachaCategoryData = GameInfo.Instance.ServerData.GachaCategoryList.Find(x => x.StoreID1 == _storeData.ConnectStoreID || x.StoreID2 == _storeData.ConnectStoreID);
			if (gachaCategoryData != null)
			{
				kDateLabel.SetActive(true);				
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
					kBasicGoodsUnit.SetActive(false);
					kDimdSpr.SetActive(true);
					kDimdLabel.textlocalize = FLocalizeString.Instance.GetText(1580);
				}
			}
		}
	}

	public void UpdateSlot(GameClientTable.StoreDisplayGoods.Param data, bool isDisable = false)
	{
		mDisable = isDisable;

		kCountLabel.SetActive(false);
        kIconSpr.SetActive(false);
		kLimitedObj.SetActive(false);
		kDimdSpr.SetActive(false);
		kLimitedLabel.textlocalize = string.Empty;
		kDimdLabel.textlocalize = string.Empty;
		kDiscountObj.SetActive( false );

		if( _ChangeBtnObj ) {
			_ChangeBtnObj.SetActive( false );//data.PanelType == 5 && data.Category == 2 );
		}

		if ( _DateLimitSpr != null && _DateLimitSpr.Length > 0 ) {
			for ( int i = 0; i < _DateLimitSpr.Length; i++ ) {
				_DateLimitSpr[i].SetActive( false );
			}
		}

		if ( data == null ) {
			return;
		}

		_displayData = data;
		RemainTime = DateTime.MinValue;

		// Date End Time Check
		string dateEndTimeStr = string.Empty;
		GachaCategoryData gachaCategoryData = GameSupport.GetGachaCategoryData(data.StoreID);
		if (gachaCategoryData != null)
        {
			dateEndTimeStr = GameSupport.GetEndTime(GameSupport.GetMinusOneMinuteEndTime(gachaCategoryData.EndDate));
		}

		kDateLabel.text = dateEndTimeStr;
		kLimitedSpr.SetActive(!string.IsNullOrEmpty(dateEndTimeStr));
		
		_storeData = GameInfo.Instance.GameTable.FindStore(data.StoreID);
		if (_storeData == null)
        {
			return;
        }

        RewardData rewardData = new RewardData(_storeData.ProductType, _storeData.ProductIndex, _storeData.ProductValue);
        kNameLabel.textlocalize = GameSupport.GetProductName(rewardData);
        kRewardListSlot.UpdateSlot(rewardData, true);

		bool isEvent = true;
		if( _displayData.PanelType != (int)eSD_PanelType.RAID_STORE ) {
			switch( (eGOODSTYPE)_storeData.PurchaseIndex ) {
				case eGOODSTYPE.CASH:
				case eGOODSTYPE.SUPPORTERPOINT:
				case eGOODSTYPE.FRIENDPOINT:
					isEvent = false;
					break;
			}
		}
		else
		{
            kDimdLabel.textlocalize = FLocalizeString.Instance.GetText(1313);
        }

		// Sale Check
		storeSaleData = GameInfo.Instance.ServerData.StoreSaleList.Find(x => x.TableID == data.StoreID);

		if( isEvent && storeSaleData != null ) {
			StoreData storeData = null;

			switch( (eStoreSaleKind)storeSaleData.LimitType ) {
				case eStoreSaleKind.CycleMinute:
					bool isActiveSaleGoods = 0 < storeSaleData.DiscountRate;
					if( isActiveSaleGoods ) {
						storeData = GameInfo.Instance.GetStoreData(_storeData.ID);
						if( storeData != null ) {
							RemainTime = storeData.GetTime();
						}

						int remainTimeSecond = (int)GameSupport.GetRemainTime(RemainTime).TotalSeconds;
						isActiveSaleGoods = remainTimeSecond <= 0;
						if( isActiveSaleGoods ) {
							RemainTime = DateTime.MinValue;
						}
					}

					kBasicGoodsUnit.SetActive( !isActiveSaleGoods );
					kDiscountObj.SetActive( isActiveSaleGoods );
					break;

				case eStoreSaleKind.LimitCnt:
					int remainCount = GameSupport.GetLimitedCnt(_storeData.ID);
					if( 0 < remainCount ) {
						kLimitedObj.SetActive( true );
						kLimitedLabel.textlocalize = FLocalizeString.Instance.GetText( 1584, remainCount );
					}
					else {
						kDimdSpr.SetActive( true );
						kDimdLabel.textlocalize = FLocalizeString.Instance.GetText( 1580 );
					}

					kBasicGoodsUnit.SetActive( kLimitedObj.activeSelf && storeSaleData.DiscountRate <= 0 );
					kDiscountObj.SetActive( kLimitedObj.activeSelf && 0 < storeSaleData.DiscountRate );
					break;

				case eStoreSaleKind.LimitDate:
				case eStoreSaleKind.LimitDate_Weekly:
				case eStoreSaleKind.LimitDate_Monthly:
					kDateLabel.SetActive( false );

					if( GameSupport.GetLimitedCnt( _storeData.ID ) > 0 ) {
						if ( _DateLimitSpr != null && _DateLimitSpr.Length > 0 ) {
							if ( storeSaleData.LimitType - 2 >= 0 ) {
								_DateLimitSpr[storeSaleData.LimitType - 2].SetActive( true );
							}
						}

						kDimdSpr.SetActive( false );
						kLimitedObj.SetActive( true );

						kLimitedLabel.textlocalize = FLocalizeString.Instance.GetText( 1584, GameSupport.GetLimitedCnt( _storeData.ID ) );
					}
					else {
						kDimdSpr.SetActive( true );

						storeData = GameInfo.Instance.GetStoreData( _storeData.ID );
						if( storeData != null ) {
							RemainTime = storeData.GetResetTime();
						}
					}
					break;
			}
		}
		else
        {
			bool isShow = true;
			bool isSale = false;
			if (storeSaleData != null && eStoreSaleKind.LimitCnt == (eStoreSaleKind)storeSaleData.LimitType)
            {
				isSale = 0 < storeSaleData.DiscountRate;
				if (isSale)
				{
					int remainCount = GameSupport.GetLimitedCnt(_storeData.ID);
					isSale = 0 < remainCount;
					if (isSale)
					{
						kLimitedObj.SetActive(true);
						kLimitedLabel.textlocalize = FLocalizeString.Instance.GetText(1584, remainCount);
					}
				}
				else
                {
					int remainCount = GameSupport.GetLimitedCnt(_storeData.ID);
					if (0 < remainCount)
					{
						kLimitedObj.SetActive(true);
						kLimitedLabel.textlocalize = FLocalizeString.Instance.GetText(1584, remainCount);
					}
					else
					{
						isShow = false;
						kDimdSpr.SetActive(true);
						kDimdLabel.textlocalize = FLocalizeString.Instance.GetText(1580);
					}
				}
			}

			kBasicGoodsUnit.SetActive(isShow && !isSale);
			kDiscountObj.SetActive(isShow && isSale);
		}

		if (kBasicGoodsUnit.gameObject.activeSelf)
		{
			if (isEvent)
            {
				kBasicGoodsUnit.InitGoodsUnit(_storeData);
			}
			else
            {
				kBasicGoodsUnit.InitGoodsUnit((eGOODSTYPE)_storeData.PurchaseIndex, _storeData.PurchaseValue, true);
			}
		}

		if( kDiscountObj.activeSelf ) {
			kLimitedSpr.SetActive( false );

			kDiscountGoodsUnit.InitGoodsUnit( _storeData, storeSaleData );
			kDiscountOriginValueLabel.textlocalize = FLocalizeString.Instance.GetText( (int)eTEXTID.GOODSTEXT_W, _storeData.PurchaseValue );
			kDiscountRateLabel.textlocalize = string.Format( "{0}%", storeSaleData.DiscountRate );
		}
	}

	public void OnClick_Slot() {
		if ( kDimdSpr.gameObject.activeSelf || mDisable ) {
			return;
		}

		if ( !GameSupport.IsCheckGoods( _storeData, storeSaleData ) ) {
			return;
		}

		if ( ParentGO == null ) {
			return;
		}

		ePOPUPGOODSTYPE popupGoodsType = ePOPUPGOODSTYPE.NONE;

		UIStorePanel storePanel = ParentGO.GetComponent<UIStorePanel>();
		if ( storePanel != null ) {
			switch ( storePanel.EtcTabSelect ) {
				case UIStorePanel.eStoreTabType.STORE_FRIENDPOINT: {
					popupGoodsType = ePOPUPGOODSTYPE.FRIENDPOINT_NONE_BTN;
				}
				break;

				case UIStorePanel.eStoreTabType.STORE_EVENT: {
					popupGoodsType = ePOPUPGOODSTYPE.EVENTSTORE_POINT;
				}
				break;

				default: {
					popupGoodsType = ePOPUPGOODSTYPE.MPOINT_NONE_BTN;
				}
				break;
			}
		}

		UIRaidStorePopup raidStorePopup = ParentGO.GetComponent<UIRaidStorePopup>();
		if ( raidStorePopup != null ) {
			popupGoodsType = ePOPUPGOODSTYPE.RAID_POINT;
		}

		StoreBuyPopup.Show( _displayData, popupGoodsType, OnMsg_Purchase, OnMsg_Purchase_Cancel );
	}

	public void OnBtnChange() {
		MessagePopup.CYN( eTEXTID.TITLE_BUY, FLocalizeString.Instance.GetText( 3328 ), eTEXTID.YES, eTEXTID.NO, 
						  eGOODSTYPE.CASH, GameInfo.Instance.GameConfig.RaidStoreResetCost, OnMsgChangeSecretStoreItem );
	}

	public void OnMsg_Purchase()
	{
		var storetabledata = GameInfo.Instance.GameTable.FindStore(x => x.ID == _displayData.StoreID);
		if (storetabledata == null)
		{
			MessageToastPopup.Show(FLocalizeString.Instance.GetText(3009));
			return;
		}
		if (!GameSupport.IsCheckInven())
			return;

        int itemCount = StoreBuyPopup.GetItemCount();

        if (!GameSupport.IsCheckGoods(_storeData, storeSaleData, itemCount))
        {
            return;
        }

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

		UpdateSlot(_displayData);

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

		if( _storeData.PurchaseIndex == (int)eGOODSTYPE.RAIDPOINT ) {
			UIRaidStorePopup raidStorePopup = LobbyUIManager.Instance.GetActiveUI<UIRaidStorePopup>( "RaidStorePopup" );
			if( raidStorePopup ) {
				raidStorePopup.SaveListPosition();
				raidStorePopup.Renewal( true );
			}
			
			LobbyUIManager.Instance.Renewal( "RaidMainPanel" );
		}
	}

	private void OnMsgChangeSecretStoreItem() {
		UIRaidStorePopup popup = LobbyUIManager.Instance.GetActiveUI<UIRaidStorePopup>( "RaidStorePopup" );
		if( popup ) {
			popup.ChangeSecretRaidStoreItem( _displayData.StoreID );
		}
	}

    private void FixedUpdate()
    {
		if (RemainTime != DateTime.MinValue)
		{
			if (GameSupport.GetRemainTime(RemainTime).TotalSeconds > 0)
			{
				if (!kRemainTimeObj.activeSelf)
					kRemainTimeObj.SetActive(true);
                
                string strtime = GameSupport.GetRemainTimeString(GameSupport.GetLocalTimeByServerTime(RemainTime), GameSupport.GetCurrentServerTime());
				kRemainTimeLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1059), strtime);
			}
			else
            {
				RemainTime = DateTime.MinValue;
				UpdateSlot(_displayData);
			}
		}
		else
		{
			//남은시간 표현 오브젝트 Off
			if (kRemainTimeObj.activeSelf)
				kRemainTimeObj.SetActive(false);
		}

	}
}
