using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISpecialLoginBonusRewardSlot : FSlot {
	[Serializable]
	private class Detail {
		public GameObject _RootObj;
		public UILabel _CountLabel;
	}

	[Header( "UISpecialLoginBonusRewardSlot" )]
	[SerializeField] private UIRewardListSlot _RewardListSlot;

	[SerializeField] private Animation _StampAnim;

	[SerializeField] private GameObject _TodayEffectObj;

	[SerializeField] private List<Detail> _DetailList;

	private enum eGradeType {
		None = 0,
		BASIC,
		SPECIAL,
		MAIN_SPECIAL,
	}

	private RewardData mRewardData;

	public void UpdateSlot( int index, GameTable.LoginBonusMonthly.Param param, int rewardIndex ) {
		if ( param == null ) {
			mRewardData = null;
			return;
		}

		mRewardData = new RewardData( param.ProductType, param.ProductIndex, param.ProductValue );

		_RewardListSlot.UpdateSlot( mRewardData, false );
		_TodayEffectObj.SetActive( index == rewardIndex );

		_StampAnim.gameObject.SetActive( index <= rewardIndex );
		if ( _StampAnim.gameObject.activeSelf ) {
			_StampAnim.enabled = index == rewardIndex;
		}

		for ( int i = 0; i < _DetailList.Count; i++ ) {
			_DetailList[i]._RootObj.SetActive( i == (param.Grade - 1) );
			if ( !_DetailList[i]._RootObj.activeSelf ) {
				continue;
			}

			_DetailList[i]._CountLabel.text = param.DayCnt.ToString();
		}
	}

	public void OnClick_Slot() {
		if ( ParentGO == null ) {
			return;
		}

		if ( mRewardData == null ) {
			return;
		}

		GameSupport.OpenRewardTableDataInfoPopup( mRewardData );
	}
}
