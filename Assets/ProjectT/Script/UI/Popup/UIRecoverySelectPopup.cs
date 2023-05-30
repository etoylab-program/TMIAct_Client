using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIRecoverySelectPopup : FComponent {
	[Header( "UIRecoverySelectPopup" )]
	[SerializeField] private UILabel _TitleLabel;
	[SerializeField] private UILabel _ExplainLabel;
	[SerializeField] private UILabel _ChangeCashLabel;
	[SerializeField] private UILabel _ChangeItemLabel;

	[SerializeField] private UIButton _ChangeCashBtn;
	[SerializeField] private UIButton _ChangeItemBtn;

	[Header( "Page 01" )]
	[SerializeField] private FTab _SelectFTab;

	[SerializeField] private UIRecoverySlot _CashSlot;
	[SerializeField] private UIRecoverySlot _ItemSlot;

	[Header( "Page 02" )]
	[SerializeField] private GameObject _RecoveryItemObj;

	[SerializeField] private FList _RecoveryFList;

	[SerializeField] private UILabel _RecoveryLabel;
	[SerializeField] private UILabel _ItemNumberLabel;

	[SerializeField] private UISprite _OrderUpSpr;
	[SerializeField] private UISprite _OrderDownSpr;

	private enum eSelectType {
		CASH = 0,
		ITEM,
	}

	private enum ePageType {
		MAIN = 0,
		ITEM,
	}

	private enum eSortType {
		ASCENDING_ORDER = 0,
		DESCENDING_ORDER,
	}

	private eGOODSTYPE mGoodsType;
	private eSelectType mSelectType;
	private ePageType mPageType;
	private eSortType mSortType;

	private int mRecoveryValue;
	private int mStoreTID;
	private int mItemTID => mStoreTID + 100;

	private List<ItemData> mItemList = new List<ItemData>();
	private List<RecoverySelectData> mSelectItemList = new List<RecoverySelectData>();

	public override void Awake() {
		base.Awake();

		_SelectFTab.EventCallBack = OnEventTabSelect;

		_RecoveryFList.EventUpdate = OnEventRecoverFListUpdate;
		_RecoveryFList.EventGetItemCount = OnEventRecoverFListCount;
		_RecoveryFList.UpdateList();
	}

	public override void InitComponent() {
		base.InitComponent();

		_TitleLabel.textlocalize = FLocalizeString.Instance.GetText( 3372, mGoodsType.ToString() );
		_ExplainLabel.textlocalize = FLocalizeString.Instance.GetText( 3373, mGoodsType.ToString() );
		_ChangeCashLabel.textlocalize = FLocalizeString.Instance.GetText( 3374, mGoodsType.ToString() );
		_ChangeItemLabel.textlocalize = FLocalizeString.Instance.GetText( 3375, mGoodsType.ToString() );

		SelectItemClear();

		mPageType = ePageType.MAIN;
		mSortType = eSortType.ASCENDING_ORDER;

		if ( mGoodsType == eGOODSTYPE.BP ) {
			mStoreTID = GameInfo.Instance.GameConfig.TicketBPStoreID;
		}
		else {
			mStoreTID = GameInfo.Instance.GameConfig.TicketAPStoreID;
		}

		mSelectType = eSelectType.CASH;
		GameTable.Store.Param storeParam = GameInfo.Instance.GameTable.FindStore( x => x.ID == mItemTID );
		if ( null != storeParam ) {
			if ( GameInfo.Instance.GetItemIDCount( storeParam.ProductIndex ) > (int)eCOUNT.NONE ) {
				mSelectType = eSelectType.ITEM;
			}
		}

		_CashSlot.SetSlot( mGoodsType, mStoreTID, mSelectType == eSelectType.CASH, bCash: true );
		_ItemSlot.SetSlot( mGoodsType, mItemTID, mSelectType == eSelectType.ITEM, bCash: false );

		_SelectFTab.SetTab( (int)mSelectType, SelectEvent.Code );
	}

	public override void OnEnable() {
		InitComponent();
		base.OnEnable();
	}

	public override void Renewal( bool bChildren = false ) {
		base.Renewal( bChildren );

		_ChangeCashBtn.SetActive( mPageType == ePageType.ITEM );
		_ChangeItemBtn.SetActive( mPageType == ePageType.MAIN );
		_RecoveryItemObj.SetActive( mPageType == ePageType.ITEM );

		_SelectFTab.gameObject.SetActive( mPageType == ePageType.MAIN );

		if ( mPageType == ePageType.MAIN ) {
			_CashSlot.UpdateSlot( mSelectType == eSelectType.CASH, bCash: true );
			_ItemSlot.UpdateSlot( mSelectType == eSelectType.ITEM, bCash: false );
		}
		else {
			int maxGoods;
			int subType;
			if ( mGoodsType == eGOODSTYPE.BP ) {
				subType = (int)eITEMSUBTYPE.USE_BP_CHARGE_NUM;
				maxGoods = GameInfo.Instance.GameConfig.BPMaxCount;
			}
			else {
				subType = (int)eITEMSUBTYPE.USE_AP_CHARGE_NUM;
				maxGoods = GameSupport.GetMaxAP();
			}

			_OrderUpSpr.SetActive( mSortType == eSortType.ASCENDING_ORDER );
			_OrderDownSpr.SetActive( mSortType == eSortType.DESCENDING_ORDER );

			long totalValue = GetSelectItemTotalValue();

			_RecoveryLabel.text = FLocalizeString.Instance.GetText( 3211,
				FLocalizeString.Instance.GetText( (int)eTEXTID.GOODSTYPE_TEXT_START + (int)mGoodsType ),
				GameInfo.Instance.UserData.GetGoods( mGoodsType ) + totalValue, maxGoods, totalValue ).Replace( "\n", "" );
			_ItemNumberLabel.text = FLocalizeString.Instance.GetText( 218, GetSelectItemTotalCount(), GameInfo.Instance.GameConfig.MatCount );

			mItemList = GameInfo.Instance.ItemList.FindAll( x => x.TableData.Type == (int)eITEMTYPE.USE && x.TableData.SubType == subType );

			SortItemList();

			_RecoveryFList.RefreshNotMoveAllItem();
			_RecoveryFList.ScrollViewCheck();
		}
	}

	public void SetGoodsType( eGOODSTYPE goodsType ) {
		mGoodsType = goodsType;
	}

	public void SetSelectItem( long uid, int value ) {
		if ( GameInfo.Instance.GameConfig.MatCount <= GetSelectItemTotalCount() ) {
			return;
		}

		int itemCount = 0;
		ItemData itemData = mItemList.Find(x => x.ItemUID == uid);
		if ( itemData != null ) {
			itemCount = itemData.Count;
		}

		RecoverySelectData selectData = mSelectItemList.Find( x => x.UID == uid );
		if ( selectData != null ) {
			itemCount -= selectData.Count;
		}

		if ( itemCount <= 0 ) {
			return;
		}

		AddSelectItem( uid, value, 1 );
		Renewal();
	}

	public void OnClick_ChangeCashBtn() {
		mPageType = ePageType.MAIN;
		Renewal();
	}

	public void OnClick_ChangeItemBtn() {
		mPageType = ePageType.ITEM;

		_RecoveryFList.SpringSetFocus( 0, isImmediate: true );

		SelectItemClear();
		Renewal();
	}

	public void OnClick_SelectItemClear() {
		SelectItemClear();
		Renewal();
	}

	public void OnClick_AutoSelectItem() {
		mSelectItemList.Clear();

		int remainCount = GameInfo.Instance.GameConfig.MatCount - GetSelectItemTotalCount();
		for ( int i = 0; i < mItemList.Count; i++ ) {
			if ( remainCount <= 0 ) {
				break;
			}

			int itemCount = mItemList[i].Count;
			RecoverySelectData selectData = mSelectItemList.Find(x => x.UID == mItemList[i].ItemUID );
			if ( selectData != null ) {
				itemCount -= selectData.Count;
			}

			if ( itemCount <= 0 ) {
				continue;
			}

			if ( itemCount <= remainCount ) {
				AddSelectItem( mItemList[i].ItemUID, mItemList[i].TableData.Value, mItemList[i].Count );
				remainCount -= mItemList[i].Count;
			}
			else {
				AddSelectItem( mItemList[i].ItemUID, mItemList[i].TableData.Value, remainCount );
				remainCount = 0;
			}
		}

		Renewal();
	}

	public void OnClick_SortItem() {
		if ( mSortType == eSortType.ASCENDING_ORDER ) {
			mSortType = eSortType.DESCENDING_ORDER;
		}
		else {
			mSortType = eSortType.ASCENDING_ORDER;
		}

		Renewal();
	}

	public void OnClick_RecoveryBtn() {
		if ( mPageType == ePageType.MAIN ) {
			if ( mSelectType == eSelectType.CASH ) {
				GameTable.Store.Param storeParam = _CashSlot.GetStoreTableDataOrNull();
				if ( storeParam == null ) {
					return;
				}

				int useCount = _CashSlot.GetUseItemCount();
				if ( !GameSupport.IsCheckGoods( eGOODSTYPE.CASH, useCount * storeParam.PurchaseValue ) ) {
					return;
				}

				mRecoveryValue = _CashSlot.GetRecoveryValue();

				if ( IsInnerLimitMaxGoods( mRecoveryValue ) ) {
					GameInfo.Instance.Send_ReqStorePurchase( storeParam.ID, false, useCount, OnNetPurchaseBuy );
				}
			}
			else {

				GameTable.Item.Param itemParam = _ItemSlot.GetItemData();
				if ( itemParam == null ) {
					return;
				}

				ItemData itemdata = GameInfo.Instance.GetItemData( itemParam.ID );
				if ( itemdata == null ) {
					MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3071, FLocalizeString.Instance.GetText( itemParam.Name ) ) );
					return;
				}

				int useCount = _ItemSlot.GetUseItemCount();
				int useItemCount = GameInfo.Instance.GetItemIDCount( itemParam.ID );
				if ( useItemCount <= (int)eCOUNT.NONE || useItemCount < useCount ) {
					MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3071, FLocalizeString.Instance.GetText( itemParam.Name ) ) );
					return;
				}

				mRecoveryValue = _ItemSlot.GetRecoveryValue();

				if ( IsInnerLimitMaxGoods( mRecoveryValue ) ) {
					GameInfo.Instance.Send_ReqUseItem( itemdata.ItemUID, useCount, 0, OnNetPurchaseBuy );
				}
			}
		}
		else {
			List<RecoverySelectData> selectDataList = mSelectItemList.FindAll( x => x.Count > 0);
			if ( selectDataList.Count <= 0 ) {
				MessageToastPopup.Show( FLocalizeString.Instance.GetText( 1109 ) );
				return;
			}

			for ( int i = 0; i < selectDataList.Count; i++ ) {
				ItemData itemData = GameInfo.Instance.GetItemData( selectDataList[i].UID );
				if ( itemData == null ) {
					return;
				}

				if ( itemData.Count < selectDataList[i].Count ) {
					MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3071, FLocalizeString.Instance.GetText( itemData.TableData.Name ) ) );
					return;
				}
			}

			mRecoveryValue = (int)GetSelectItemTotalValue();

			if ( IsInnerLimitMaxGoods( mRecoveryValue ) ) {
				GameInfo.Instance.Send_ReqUseItemForArray( selectDataList.ToArray(), OnNetPurchaseBuy );
			}
		}
	}

	private bool OnEventTabSelect( int nSelect, SelectEvent type ) {
		if ( type == SelectEvent.Enable ) {
			return false;
		}

		if ( type == SelectEvent.Click ) {
			mSelectType = (eSelectType)nSelect;
			Renewal();
		}

		return true;
	}

	private void OnEventRecoverFListUpdate( int index, GameObject obj ) {
		UIItemListSlot slot = obj.GetComponent<UIItemListSlot>();
		if ( slot == null ) {
			return;
		}

		int count = 0;
		bool isSelect = false;
		ItemData itemData = null;
		if ( 0 <= index && index < mItemList.Count ) {
			itemData = mItemList[index];
			isSelect = IsSelectItem( itemData.ItemUID );
			count = itemData.Count - GetSelectItemCount( itemData.ItemUID );
		}

		if ( slot.ParentGO == null ) {
			slot.ParentGO = this.gameObject;
		}

		slot.UpdateSlot( UIItemListSlot.ePosType.AP_BP_RECORVERY, index, itemData, isSelect, count );
	}

	private int OnEventRecoverFListCount() {
		return mItemList.Count;
	}

	private void OnNetPurchaseBuy( int result, PktMsgType pktmsg ) {
		if ( result != 0 ) {
			return;
		}

		LobbyUIManager.Instance.Renewal( "TopPanel" );
		LobbyUIManager.Instance.Renewal( "GoodsPopup" );
		LobbyUIManager.Instance.Renewal( "StageDetailPopup" );
		LobbyUIManager.Instance.Renewal( "BattleResultPopup" );
		LobbyUIManager.Instance.Renewal( "CharSeletePopup" );
		LobbyUIManager.Instance.Renewal( "RaidDetailPopup" );

		string descStr;
		if ( mGoodsType == eGOODSTYPE.AP ) {
			descStr = FLocalizeString.Instance.GetText( 3067, mRecoveryValue );
		}
		else {
			descStr = FLocalizeString.Instance.GetText( 3171, mRecoveryValue );
		}
		MessageToastPopup.Show( descStr );

		mSelectItemList.Clear();

		Renewal();
	}

	private void SelectItemClear() {
		for ( int i = 0; i < mSelectItemList.Count; i++ ) {
			mSelectItemList[i].Count = 0;
		}
	}

	private bool IsSelectItem( long uid ) {
		return mSelectItemList.Any( x => x.UID == uid && x.Count > 0 );
	}

	private void AddSelectItem( long uid, int value, int plusCount ) {
		RecoverySelectData recoverySelectData = mSelectItemList.Find( x => x.UID == uid );
		if ( recoverySelectData != null ) {
			recoverySelectData.Count += plusCount;
		}
		else {
			mSelectItemList.Add( new RecoverySelectData() { UID = uid, Count = plusCount, Value = value } );
		}
	}

	private int GetSelectItemCount( long uid ) {
		int result = 0;
		RecoverySelectData recoverySelectData = mSelectItemList.Find( x => x.UID == uid );
		if ( recoverySelectData != null ) {
			result = recoverySelectData.Count;
		}
		return result;
	}

	private int GetSelectItemTotalCount() {
		int result = 0;
		for ( int i = 0; i < mSelectItemList.Count; i++ ) {
			result += mSelectItemList[i].Count;
		}
		return result;
	}

	private long GetSelectItemTotalValue() {
		long result = 0;
		for ( int i = 0; i < mSelectItemList.Count; i++ ) {
			result += mSelectItemList[i].Count * mSelectItemList[i].Value;
		}
		return result;
	}

	private void SortItemList() {
		int value01;
		int value02;
		if ( mSortType == eSortType.ASCENDING_ORDER ) {
			value01 = -1;
			value02 = 1;
		}
		else {
			value01 = 1;
			value02 = -1;
		}

		mItemList.Sort( ( x, y ) => {
			if ( x.TableData.Value < y.TableData.Value ) {
				return value01;
			}

			if ( x.TableData.Value > y.TableData.Value ) {
				return value02;
			}

			if ( x.TableData.Value == y.TableData.Value ) {
				if ( x.TableID < y.TableID ) {
					return value01;
				}

				if ( x.TableID > y.TableID ) {
					return value02;
				}
			}

			return 0;
		} );
	}

	private bool IsInnerLimitMaxGoods( int plusValue ) {
		int limitValue = mGoodsType == eGOODSTYPE.AP ? GameInfo.Instance.GameConfig.LimitMaxAP : GameInfo.Instance.GameConfig.LimitMaxBP;
		if ( limitValue < GameInfo.Instance.UserData.GetGoods( mGoodsType ) + plusValue ) {
			MessageToastPopup.Show( FLocalizeString.Instance.GetText( 3395, mGoodsType.ToString() ) );
			return false;
		}

		return true;
	}
}
