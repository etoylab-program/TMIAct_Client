using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAutoGachaRewardListPopup : FComponent {

	[Header("- UIAutoGachaRewardListPopup")]
	[SerializeField] private FList _RewardFList;

	private List<RewardData> mRewardList = new List<RewardData>();


	public override void Awake() {
		base.Awake();

		_RewardFList.EventUpdate = OnEventRewardFListUpdate;
		_RewardFList.EventGetItemCount = OnEventRewardFListGetItemCount;
		_RewardFList.UpdateList();
	}

	public override void Renewal(bool bChildren = false) {
		base.Renewal(bChildren);

		_RewardFList.SpringSetFocus(0, isImmediate: true);
		_RewardFList.RefreshNotMoveAllItem();
	}

	public void SetData(List<RewardData> rewardList) {
		mRewardList = rewardList;
	}

    private void OnEventRewardFListUpdate(int index, GameObject obj) {
		UIItemListSlot slot = obj.GetComponent<UIItemListSlot>();
		if (slot == null) {
			return;
		}

		if (slot.ParentGO == null) {
			slot.ParentGO = this.gameObject;
		}

		RewardData rewardData = null;
		if (0 <= index && index < mRewardList.Count) {
			rewardData = mRewardList[index];
		}

		slot.UpdateSlotRewardDataByCount(UIItemListSlot.ePosType.REWARD_DATA_INFO_NOT_SELL, index, rewardData);
	}

	private int OnEventRewardFListGetItemCount() {
		return mRewardList.Count;
	}

}
