using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISpecialLoginBonusPopup : FComponent {
	[Header( "UISpecialLoginBonusPopup" )]
	[SerializeField] private FList _BannerFList = null;
	[SerializeField] private FList _RewardFList = null;

	private List<GameTable.LoginBonusMonthly.Param> mRewardList = new List<GameTable.LoginBonusMonthly.Param>();
	private List<GameClientTable.LoginBonusMonthlyDisplay.Param> mBannerList;

	public override void Awake() {
		base.Awake();

		mBannerList = GameInfo.Instance.GameClientTable.LoginBonusMonthlyDisplays;
		mBannerList.Sort( ( f, b ) => {
			if ( f.ID < b.ID ) {
				return -1;
			}

			if ( f.ID > b.ID ) {
				return 1;
			}

			return 0;
		} );

		_BannerFList.EventUpdate = OnEventBannerFListUpdate;
		_BannerFList.EventGetItemCount = OnEventBannerFListCount;
		_BannerFList.InitBottomFixing();
		_BannerFList.UpdateList();

		_RewardFList.EventUpdate = OnEventRewardFListUpdate;
		_RewardFList.EventGetItemCount = OnEventRewardFListCount;
		_RewardFList.InitBottomFixing();
		_RewardFList.UpdateList();
	}

	public override void InitComponent() {
		base.InitComponent();

		mRewardList = GameInfo.Instance.GameTable.FindAllLoginBonusMonthly( x => x.GroupID == GetBannerTID() );

		_BannerFList.SpringSetFocus( GetBannerSelectIndex() );
		_BannerFList.RefreshNotMoveAllItem();

		_RewardFList.SpringSetFocus( 0, isImmediate: true );
		_RewardFList.RefreshNotMoveAllItem();
	}

	public override void OnEnable() {
		InitComponent();
		base.OnEnable();
	}

	private void OnEventBannerFListUpdate( int index, GameObject gObj ) {
		UISpecialLoginBonusBannerSlot slot = gObj.GetComponent<UISpecialLoginBonusBannerSlot>();
		if ( slot == null ) {
			return;
		}

		if ( slot.ParentGO == null ) {
			slot.ParentGO = this.gameObject;
		}

		GameClientTable.LoginBonusMonthlyDisplay.Param param = null;
		if ( 0 <= index && index < mBannerList.Count ) {
			param = mBannerList[index];
		}

		slot.UpdateSlot( index, param, GetBannerSelectIndex() );
	}

	private int OnEventBannerFListCount() {
		return mBannerList.Count;
	}

	private void OnEventRewardFListUpdate( int index, GameObject gObj ) {
		UISpecialLoginBonusRewardSlot slot = gObj.GetComponent<UISpecialLoginBonusRewardSlot>();
		if ( slot == null ) {
			return;
		}

		if ( slot.ParentGO == null ) {
			slot.ParentGO = this.gameObject;
		}

		GameTable.LoginBonusMonthly.Param param = null;
		if ( 0 <= index && index < mRewardList.Count ) {
			param = mRewardList[index];
		}

		slot.UpdateSlot( index, param, GetRewardIndex() );
	}

	private int OnEventRewardFListCount() {
		return mRewardList.Count;
	}

	private int GetBannerSelectIndex() {
		return ( GameInfo.Instance.UserData.LoginBonusMonthlyCount - 1 ) / 30;
	}

	private int GetBannerTID() {
		return GetBannerSelectIndex() + 1;
	}

	private int GetRewardIndex() {
		return ( GameInfo.Instance.UserData.LoginBonusMonthlyCount - 1 ) % 30;
	}
}
