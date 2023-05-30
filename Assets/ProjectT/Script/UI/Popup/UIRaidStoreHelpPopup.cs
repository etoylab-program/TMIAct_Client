
using UnityEngine;
using System.Collections.Generic;


public class UIRaidStoreHelpPopup : FComponent {
	[Header( "[Raid Store Help]" )]
	[SerializeField] private FTab _MainFTab;
	[SerializeField] private FList _ItemFList;
	[SerializeField] private UILabel _GroupLabel;
	[SerializeField] private UILabel _InfoLabel;

	private eRaidStoreHelpType mTabType;
	private List<GameClientTable.StoreDisplayGoods.Param> mAlwaysParamList = new List<GameClientTable.StoreDisplayGoods.Param>();
	private List<GameClientTable.StoreDisplayGoods.Param> mDailyParamList = new List<GameClientTable.StoreDisplayGoods.Param>();
	private List<GameClientTable.StoreDisplayGoods.Param> mWeeklyParamList = new List<GameClientTable.StoreDisplayGoods.Param>();
	private List<GameClientTable.StoreDisplayGoods.Param> mMonthlyParamList = new List<GameClientTable.StoreDisplayGoods.Param>();

	public override void Awake() {
		base.Awake();

		mAlwaysParamList.Clear();
		mDailyParamList.Clear();
		mWeeklyParamList.Clear();
		mMonthlyParamList.Clear();

		List<GameTable.RaidStore.Param> raidStoreParamList = GameInfo.Instance.GameTable.FindAllRaidStore( x => x.GroupID == GameInfo.Instance.ServerData.RaidCurrentSeason );

		GameClientTable.StoreDisplayGoods.Param param = null;
		for ( int i = 0; i < raidStoreParamList.Count; i++ ) {
			param = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods( x => x.StoreID == raidStoreParamList[i].StoreID );
			if ( param != null ) {
				StoreSaleData storeSaleData = GameInfo.Instance.ServerData.StoreSaleList.Find( x => x.TableID == param.StoreID );
				if ( storeSaleData == null ) {
					mAlwaysParamList.Add( param );
				}
				else {
					switch ( (eStoreSaleKind)storeSaleData.LimitType ) {
						case eStoreSaleKind.LimitDate_Day: {
							mDailyParamList.Add( param );
						}
						break;

						case eStoreSaleKind.LimitDate_Weekly: {
							mWeeklyParamList.Add( param );
						}
						break;

						case eStoreSaleKind.LimitDate_Monthly: {
							mMonthlyParamList.Add( param );
						}
						break;

						default: {
							Debug.LogError( "eStoreSaleKind.LimitDate???" );
						}
						break;
					}
				}
			}
		}

		_MainFTab.EventCallBack = OnEventTabSelect;
		_ItemFList.EventGetItemCount = OnEventGetItemCount;
		_ItemFList.EventUpdate = OnEventUpdateItem;
		_ItemFList.UpdateList();
	}

	public override void InitComponent() {
		base.InitComponent();

		mTabType = eRaidStoreHelpType.NONE;

		int startMainTabIndex = -1;

		for ( int i = 0; i < _MainFTab.kBtnList.Count; i++ ) {
			bool isEnable = 0 < GetItemCount( (eRaidStoreHelpType)i );
			_MainFTab.SetEnabled( i, isEnable );

			if ( startMainTabIndex < 0 && isEnable ) {
				startMainTabIndex = i;
			}
		}

		if ( startMainTabIndex < 0 ) {
			startMainTabIndex = 0;
		}

		_MainFTab.SetTab( startMainTabIndex, SelectEvent.Code );
	}

	public override void OnEnable() {
		InitComponent();
		base.OnEnable();
	}

	public override void Renewal( bool bChildren = false ) {
		base.Renewal( bChildren );

		_ItemFList.SpringSetFocus( 0, isImmediate: true );
		_ItemFList.RefreshNotMoveAllItem();
		_ItemFList.ScrollViewCheck();

		switch ( mTabType ) {
			case eRaidStoreHelpType.ALWAYS: {
				_GroupLabel.textlocalize = FLocalizeString.Instance.GetText( 3340 );
				_InfoLabel.textlocalize = FLocalizeString.Instance.GetText( 3344 );
			}
			break;

			case eRaidStoreHelpType.DAILY: {
				_GroupLabel.textlocalize = FLocalizeString.Instance.GetText( 3341 );
				_InfoLabel.textlocalize = FLocalizeString.Instance.GetText( 3345 );
			}
			break;

			case eRaidStoreHelpType.WEEKLY: {
				_GroupLabel.textlocalize = FLocalizeString.Instance.GetText( 3342 );
				_InfoLabel.textlocalize = FLocalizeString.Instance.GetText( 3346 );
			}
			break;

			case eRaidStoreHelpType.MONTHLY: {
				_GroupLabel.textlocalize = FLocalizeString.Instance.GetText( 3343 );
				_InfoLabel.textlocalize = FLocalizeString.Instance.GetText( 3347 );
			}
			break;

			default: {
				_GroupLabel.textlocalize = string.Empty;
				_InfoLabel.textlocalize = string.Empty;
			}
			break;
		}
	}

	private bool OnEventTabSelect( int nSelect, SelectEvent type ) {
		if ( type == SelectEvent.Enable ) {
			return false;
		}

		eRaidStoreHelpType selectTabType = (eRaidStoreHelpType)nSelect;
		if ( mTabType == selectTabType ) {
			return false;
		}

		mTabType = selectTabType;

		if ( type == SelectEvent.Click ) {
			Renewal();
		}

		return true;
	}

	private int OnEventGetItemCount() {
		return GetItemCount( mTabType );
	}

	private void OnEventUpdateItem( int index, GameObject gObj ) {
		UIEventStoreListSlot slot = gObj.GetComponent<UIEventStoreListSlot>();
		if ( slot == null ) {
			Debug.LogError( "NullException - UIRaidStoreHelpPopup > OnEventUpdateItem > Slot" );
			return;
		}

		GameClientTable.StoreDisplayGoods.Param param = null;

		switch ( mTabType ) {
			case eRaidStoreHelpType.ALWAYS: {
				if ( 0 <= index && index < mAlwaysParamList.Count ) {
					param = mAlwaysParamList[index];
				}
			}
			break;

			case eRaidStoreHelpType.DAILY: {
				if ( 0 <= index && index < mDailyParamList.Count ) {
					param = mDailyParamList[index];
				}
			}
			break;

			case eRaidStoreHelpType.WEEKLY: {
				if ( 0 <= index && index < mWeeklyParamList.Count ) {
					param = mWeeklyParamList[index];
				}
			}
			break;

			case eRaidStoreHelpType.MONTHLY: {
				if ( 0 <= index && index < mMonthlyParamList.Count ) {
					param = mMonthlyParamList[index];
				}
			}
			break;

			default: {
				Debug.LogError( "Item Type Not Found - UIRaidStoreHelpPopup > GetItemCount" );
			}
			break;
		}

		slot.UpdateSlot( param );
	}

	private int GetItemCount( eRaidStoreHelpType itemType ) {
		int result = 0;

		switch ( itemType ) {
			case eRaidStoreHelpType.ALWAYS: {
				result = mAlwaysParamList.Count;
			}
			break;

			case eRaidStoreHelpType.DAILY: {
				result = mDailyParamList.Count;
			}
			break;

			case eRaidStoreHelpType.WEEKLY: {
				result = mWeeklyParamList.Count;
			}
			break;

			case eRaidStoreHelpType.MONTHLY: {
				result = mMonthlyParamList.Count;
			}
			break;

			default: {
				Debug.LogError( "Item Type Not Found - UIRaidStoreHelpPopup > GetItemCount" );
			}
			break;
		}

		return result;
	}
}
