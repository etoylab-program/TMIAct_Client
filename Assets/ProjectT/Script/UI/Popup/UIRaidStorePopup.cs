
using UnityEngine;
using System.Collections.Generic;


public class UIRaidStorePopup : FComponent {
	[Header( "[Raid store]" )]
	[SerializeField] private FTab		_MainTab;
	[SerializeField] private FList		_StoreList;
	[SerializeField] private GameObject	_VrStoreObj;
	[SerializeField] private GameObject _SecretStoreObj;
	[SerializeField] private UILabel	_RemainTimeLabel;
	[SerializeField] private UILabel    _NoItemLabel;
	[SerializeField] private GameObject _ChangeBtnObj;
	[SerializeField] private GameObject _DisableChangeBtnObj;
	[SerializeField] private UIButton	_HelpBtn;

	private int												mSelectTab						= 0;
	private List<GameClientTable.StoreDisplayGoods.Param>	mRaidAllStoreList				= new List<GameClientTable.StoreDisplayGoods.Param>();
	private List<GameClientTable.StoreDisplayGoods.Param>	mSelectedRaidStoreList			= new List<GameClientTable.StoreDisplayGoods.Param>();
	private List<GameClientTable.StoreDisplayGoods.Param>	mSelectedStoreItemForSort		= new List<GameClientTable.StoreDisplayGoods.Param>();
	private List<byte>										mSelectedRaidStoreNumberList	= new List<byte>();
	private bool											mSendingReqRaidStoreList		= false;
	private int												mChangeStoreItemId				= 0;
	private System.TimeSpan									mRemainTime;
	private float											mCheckTime						= 0.0f;
	private bool											mLoadListFocus					= false;
	private float											mCheckDisableTime				= 0.0f;
	private int												mForceSelectTabIndex			= -1;


	public override void Awake() {
		base.Awake();

		_MainTab.EventCallBack = OnTabMainSelect;

		_StoreList.EventUpdate = UpdateStoreListSlot;
		_StoreList.EventGetItemCount = GetStoreListCount;
		_StoreList.UpdateList();
	}

	public override void OnEnable() {
		InitComponent();
		base.OnEnable();
	}

	public override void OnDisable() {
		base.OnDisable();
		mForceSelectTabIndex = -1;
	}

	public override void InitComponent() {
		mRaidAllStoreList = GameInfo.Instance.GameClientTable.FindAllStoreDisplayGoods( x => x.PanelType == (int)eSD_PanelType.RAID_STORE );

		mRemainTime = GameSupport.GetRemainTime( GameInfo.Instance.RaidSecretStoreChangeRemainTime );//, GameSupport.GetCurRealServerTime() );
		if( mForceSelectTabIndex == 2 || ( mForceSelectTabIndex == -1 && mRemainTime.TotalSeconds <= 0 ) ) {
			_MainTab.SetTab( 1, SelectEvent.Code );
		}
		else if( mForceSelectTabIndex == 1 || ( mForceSelectTabIndex == -1 && mRemainTime.TotalSeconds > 0 ) ) {
			_MainTab.SetTab( 0, SelectEvent.Code );

			mSendingReqRaidStoreList = false;
			mChangeStoreItemId = 0;
			mCheckTime = 1.0f;
		}

		mLoadListFocus = false;
		_DisableChangeBtnObj.SetActive( false );
	}

	public override void Renewal( bool bChildren = false ) {
		base.Renewal( bChildren );
		SelectTab();

		if( mLoadListFocus ) {
			_StoreList.LoadSavedFocus();
			mLoadListFocus = false;
		}
	}

	public void ForceSelectTab( int index ) {
		mForceSelectTabIndex = index;
	}

	public void ChangeSecretRaidStoreItem( int storeId ) {
		mChangeStoreItemId = storeId;

		GameTable.RaidStore.Param param = GameInfo.Instance.GameTable.FindRaidStore( x => x.GroupID == GameInfo.Instance.ServerData.RaidCurrentSeason && x.StoreID == storeId );
		GameInfo.Instance.Send_ReqRaidStoreList( (byte)param.No, OnNetChangeRaidStoreItem );
	}

	public void SaveListPosition() {
		mLoadListFocus = true;
		_StoreList.SaveCurrentFocus();
	}

	public void ShowNoItemText() {
		_StoreList.gameObject.SetActive( false );
		_NoItemLabel.SetActive( true );
	}

	public void OnBtnChangeSecretItems() {
		MessagePopup.CYN( FLocalizeString.Instance.GetText( 3327 ), FLocalizeString.Instance.GetText( 3328 ), eTEXTID.YES, eTEXTID.NO,
						  eGOODSTYPE.CASH, GameInfo.Instance.GameConfig.RaidStoreResetCost * mSelectedRaidStoreNumberList.Count, OnMsgChangeSecretItems );
	}

	public void OnBtnHelp() {
		LobbyUIManager.Instance.ShowUI( "RaidStoreHelpPopup", true );
	}

	private void SortItems() {
		mSelectedStoreItemForSort.Clear();

		for ( int i = 0; i < mSelectedRaidStoreList.Count; i++ ) {
			StoreSaleData storeSaleData = GameInfo.Instance.ServerData.StoreSaleList.Find( x => x.TableID == mSelectedRaidStoreList[i].StoreID );
			if( storeSaleData == null ) {
				continue;
			}

			if( storeSaleData.LimitType >= (int)eStoreSaleKind.LimitDate && GameSupport.GetLimitedCnt( mSelectedRaidStoreList[i].StoreID ) <= 0 ) {
				mSelectedStoreItemForSort.Add( mSelectedRaidStoreList[i] );
			}
		}

		for( int i = 0; i < mSelectedStoreItemForSort.Count; i++ ) {
			mSelectedRaidStoreList.Remove( mSelectedStoreItemForSort[i] );
			mSelectedRaidStoreList.Add( mSelectedStoreItemForSort[i] );
		}
	}

	private void SelectTab() {
		_NoItemLabel.SetActive( false );
		_StoreList.gameObject.SetActive( true );
		_HelpBtn.SetActive(mSelectTab != 0);
		_DisableChangeBtnObj.SetActive( false );

		if ( mSelectTab == 0 ) {
			_VrStoreObj.SetActive( true );
			_SecretStoreObj.SetActive( false );
			_ChangeBtnObj.SetActive( false );

			mSelectedRaidStoreList.Clear();
			List<GameClientTable.StoreDisplayGoods.Param> find = mRaidAllStoreList.FindAll( x => x.Category == mSelectTab + 1 );

			for( int i = 0; i < find.Count; i++ ) {
				//if( GameSupport.IsShowStoreDisplay( find[i] ) ) {
					mSelectedRaidStoreList.Add( find[i] );
				//}
			}

			SortItems();
			_StoreList.Reset();
			_StoreList.RefreshNotMoveAllItem();
		}
		else {
			_VrStoreObj.SetActive( false );
			_SecretStoreObj.SetActive( true );
			_ChangeBtnObj.SetActive( true );

			SendReqRaidStoreList();
		}
	}

	private void UpdateStoreListSlot( int index, GameObject slotObject ) {
		if( mSelectedRaidStoreList.Count <= 0 || index < 0 || index >= mSelectedRaidStoreList.Count ) {
			return;
		}

		UIEventStoreListSlot slot = slotObject.GetComponent<UIEventStoreListSlot>();
		
		slot.ParentGO = gameObject;
		slot.UpdateSlot( mSelectedRaidStoreList[index] );
	}

	private int GetStoreListCount() {
		return mSelectedRaidStoreList.Count;
	}

	private void SendReqRaidStoreList() {
		mSendingReqRaidStoreList = true;
		_DisableChangeBtnObj.SetActive( true );

		GameInfo.Instance.Send_ReqRaidStoreList( 0, OnNetRaidStoreList );
	}

	private bool OnTabMainSelect( int nSelect, SelectEvent type ) {
		if ( type == SelectEvent.Enable || ( type != SelectEvent.Code && mSelectTab == nSelect ) ) {
			return false;
		}

		mSelectTab = nSelect;
		SelectTab();

		return true;
	}

	private void OnMsgChangeSecretItems() {
		mSendingReqRaidStoreList = true;
		_DisableChangeBtnObj.SetActive( true );

		GameInfo.Instance.Send_ReqChangeRaidAllStoreList( mSelectedRaidStoreNumberList, OnNetRaidStoreList );
	}

	private void OnNetRaidStoreList( int result, PktMsgType pktMsg ) {
		mSendingReqRaidStoreList = false;

		if( result != 0 ) {
			return;
		}

		LobbyUIManager.Instance.Renewal( "TopPanel" );
		LobbyUIManager.Instance.Renewal( "GoodsPopup" );

		mRemainTime = GameSupport.GetRemainTime( GameInfo.Instance.RaidSecretStoreChangeRemainTime );//, GameSupport.GetCurRealServerTime() );
		UpdateRemainTime();

		PktInfoRaidStoreListAck pkt = pktMsg as PktInfoRaidStoreListAck;
		List<GameTable.RaidStore.Param> find = GameInfo.Instance.GameTable.FindAllRaidStore( x => x.GroupID == GameInfo.Instance.ServerData.RaidCurrentSeason );

		mSelectedRaidStoreNumberList.Clear();
		mSelectedRaidStoreList.Clear();

		for( int i = 0; i < find.Count; i++ ) {
			long n = pkt.info_.showFlag_ & ( 1 << ( find[i].No ) );
			if( n > 0 ) {
				GameClientTable.StoreDisplayGoods.Param param = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods( x => x.StoreID == find[i].StoreID );
				mSelectedRaidStoreNumberList.Add( (byte)find[i].No );

				//if( GameSupport.IsShowStoreDisplay( param ) ) {
				mSelectedRaidStoreList.Add( param );
				//}
			}
		}

		if( mSelectedRaidStoreList.Count > 0 ) {
			SortItems();
			_StoreList.Reset();
			_StoreList.RefreshNotMoveAllItem();
		}
		else {
			ShowNoItemText();
		}
	}

	private void OnNetChangeRaidStoreItem( int result, PktMsgType pktMsg ) {
		if( result != 0 ) {
			return;
		}

		LobbyUIManager.Instance.Renewal( "TopPanel" );
		LobbyUIManager.Instance.Renewal( "GoodsPopup" );

		PktInfoRaidStoreListAck pkt = pktMsg as PktInfoRaidStoreListAck;
		List<GameTable.RaidStore.Param> find = GameInfo.Instance.GameTable.FindAllRaidStore( x => x.GroupID == GameInfo.Instance.ServerData.RaidCurrentSeason );

		for( int i = 0; i < find.Count; i++ ) {
			long n = pkt.info_.showFlag_ & ( 1 << find[i].No );
			if( n > 0 ) {
				GameClientTable.StoreDisplayGoods.Param param = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods( x => x.StoreID == find[i].StoreID );
				
				GameClientTable.StoreDisplayGoods.Param findItem = mSelectedRaidStoreList.Find( x => x.ID == param.ID );
				if( findItem == null ) {
					for( int j = 0; j < mSelectedRaidStoreList.Count; j++ ) {
						if( mSelectedRaidStoreList[j].ID == mChangeStoreItemId ) {
							mSelectedRaidStoreList[j] = param;
							_StoreList.Reset();
							_StoreList.RefreshNotMoveAllItem();

							return;
						}
					}
				}
			}
		}
	}

	private bool UpdateRemainTime() {
		mRemainTime += System.TimeSpan.FromSeconds( -mCheckTime );

		string str = string.Format( "{0} {1}", FLocalizeString.Instance.GetText( 261 ), FLocalizeString.Instance.GetText( 263 ).Replace( "{0}", "{2}" ) );
		str = string.Format( str, mRemainTime.Hours, mRemainTime.Minutes, mRemainTime.Seconds );

		_RemainTimeLabel.textlocalize = string.Format( FLocalizeString.Instance.GetText( 3326 ), str );

		return mRemainTime.TotalSeconds > 0;
	}

	private void FixedUpdate() {
		if( _DisableChangeBtnObj.activeSelf ) {
			mCheckDisableTime += Time.fixedDeltaTime;
			if( mCheckDisableTime >= 2.0f ) {
				_DisableChangeBtnObj.SetActive( false );
				mCheckDisableTime = 0.0f;
			}
		}

		if( mSelectTab != 1 || mSendingReqRaidStoreList ) {
			return;
		}

		mCheckTime += Time.fixedDeltaTime;

		if( mCheckTime >= 1.0f ) {
			if( UpdateRemainTime() ) {
				mCheckTime = 0.0f;
			}
			else {
				SendReqRaidStoreList();
			}
		}
	}
}
