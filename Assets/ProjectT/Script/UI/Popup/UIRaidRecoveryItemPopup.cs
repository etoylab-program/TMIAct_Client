
using UnityEngine;
using System.Collections.Generic;


public class UIRaidRecoveryItemPopup : FComponent {
	[Header( "[Raid recovery item]" )]
	[SerializeField] private UILabel		_TitleLabel;
	[SerializeField] private UILabel		_RecoverLabel;
	[SerializeField] private UISprite		_DisableBtnSpr;
	[SerializeField] private FList			_ItemList;

	private List<ItemData>	mRecoverItemDataList		= new List<ItemData>();
	private List<ItemData>	mSendRecoverItemDataList	= new List<ItemData>();
	private CharData		mCharData					= null;
	private List<int>		mSelectedItemSlotIndexList	= new List<int>();
	private int				mCurHpPercentage			= 0;
	private int				mResultHpPercentage			= 0;
	private int				mSelectItemIndex			= 0;


	public override void Awake() {
		base.Awake();

		_ItemList.EventUpdate = UpdateItemListSlot;
		_ItemList.EventGetItemCount = GetItemListCount;
	}

	public override void OnEnable() {
		mSelectItemIndex = 0;
		base.OnEnable();
	}

	public override void Renewal( bool bChildren = false ) {
		base.Renewal( bChildren );

		_TitleLabel.textlocalize = FLocalizeString.Instance.GetText( 3329 );
		mSelectedItemSlotIndexList.Clear();

		List<ItemData> find = GameInfo.Instance.ItemList.FindAll( x => x.TableData.Type == 0 && x.TableData.SubType == (int)eITEMSUBTYPE.MATERIAL_RAID_POTION );

		if( find != null && find.Count > 0 ) {
			_ItemList.gameObject.SetActive( true );
			_DisableBtnSpr.SetActive( false );

			mRecoverItemDataList.Clear();
			mRecoverItemDataList.Capacity = find.Count;
			mRecoverItemDataList.AddRange( find );

			mRecoverItemDataList.Sort( delegate ( ItemData lhs, ItemData rhs ) {
				if( lhs.TableData.Value < rhs.TableData.Value ) {
					return 1;
				}
				else if( lhs.TableData.Value > rhs.TableData.Value ) {
					return -1;
				}

				return 0;
			} );

			_ItemList.UpdateList();

			mSelectedItemSlotIndexList.Capacity = find.Capacity;
			SelectItem( mSelectItemIndex );
		}
		else {
			_ItemList.gameObject.SetActive( false );
			_DisableBtnSpr.SetActive( true );

			_RecoverLabel.textlocalize = FLocalizeString.Instance.GetText( 1593 );
		}
	}

	public void SetCharData( CharData charData ) {
		mCharData = charData;

		mCurHpPercentage = (int)mCharData.RaidHpPercentage;
		mResultHpPercentage = mCurHpPercentage;
	}

	public bool HasItemSlotIndex( int index ) {
		for( int i = 0; i < mSelectedItemSlotIndexList.Count; i++ ) {
			if( mSelectedItemSlotIndexList[i] == index ) {
				return true;
			}
		}

		return false;
	}

	public void SelectItem( int index ) {
		if( index < 0 || index >= mRecoverItemDataList.Count ) {
			return;
		}

		mSelectItemIndex = index;

		if ( mSelectedItemSlotIndexList.Count > 0 ) {
			mSelectedItemSlotIndexList.RemoveAt( 0 );
			mResultHpPercentage = mCurHpPercentage;
		}

		mSelectedItemSlotIndexList.Add( index );
		mResultHpPercentage += mRecoverItemDataList[index].TableData.Value;

		_DisableBtnSpr.SetActive( mCurHpPercentage >= 100 );
		_ItemList.UpdateList();

		UpdateRecoverLabel();
	}

	public void OnBtnUse() {
		if( mSelectedItemSlotIndexList.Count <= 0 || mRecoverItemDataList.Count <= 0 ) {
			return;
		}

		mSendRecoverItemDataList.Clear();
		mSendRecoverItemDataList.Capacity = mSelectedItemSlotIndexList.Count;

		for( int i = 0; i < mSelectedItemSlotIndexList.Count; i++ ) {
			mSendRecoverItemDataList.Add( mRecoverItemDataList[mSelectedItemSlotIndexList[i]] );
		}

		GameInfo.Instance.Send_ReqRaidHpRestore( mCharData.CUID, mSendRecoverItemDataList, OnNetRaidHpRestore );
	}

	private void UpdateItemListSlot( int index, GameObject slotObject ) {
		UIItemListSlot slot = slotObject.GetComponent<UIItemListSlot>();
		slot.ParentGO = gameObject;

		slot.UpdateSlot( UIItemListSlot.ePosType.RaidRecoverHpItem, index, mRecoverItemDataList[index] );
	}

	private int GetItemListCount() {
		return mRecoverItemDataList.Count;
	}

	private void UpdateRecoverLabel() {
		_RecoverLabel.textlocalize = string.Format( FLocalizeString.Instance.GetText( 3330 ), mCurHpPercentage, Mathf.Min( 100, mResultHpPercentage ) );
	}

	private void OnNetRaidHpRestore( int result, PktMsgType pkt ) {
		if( result != 0 ) {
			return;
		}

		UIRaidDetailPopup popup = LobbyUIManager.Instance.GetUI<UIRaidDetailPopup>( "RaidDetailPopup" );
		popup.PlayUseHpItemEffAndRenewal();

		SetCharData( mCharData );

		if( mCurHpPercentage >= 100 ) {
			OnClickClose();
			return;
		}

		Renewal();
	}
}
