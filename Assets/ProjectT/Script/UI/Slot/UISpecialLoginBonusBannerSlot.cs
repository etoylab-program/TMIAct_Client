using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISpecialLoginBonusBannerSlot : FSlot {
	[Serializable]
	private class Detail {
		public GameObject _RootObj;
		public GameObject _CompleteObj;

		public UILabel _DayLabel;
		public UILabel _TitleLabel;
	}

	[Header( "UISpecialLoginBonusBannerSlot" )]
	[SerializeField] private UISprite _NoticeSpr;
	[SerializeField] private UISprite _SelSpr;

	[SerializeField] private UITexture _RewardIconTex;

	[SerializeField] private List<Detail> _DetailList;

	private enum eSlotType {
		BASIC = 0,
		NOW,
	}

	public void UpdateSlot( int index, GameClientTable.LoginBonusMonthlyDisplay.Param param, int selectIndex ) {
		if ( param == null ) {
			return;
		}

		_NoticeSpr.SetActive( false );

		eSlotType slotType = index != selectIndex ? eSlotType.BASIC : eSlotType.NOW;
		_SelSpr.SetActive( slotType == eSlotType.NOW );
		_RewardIconTex.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle( "icon", Utility.AppendString( "icon/loginbonus/", param.Banner, ".png" ) ) as Texture;

		for ( int i = 0; i < _DetailList.Count; i++ ) {
			_DetailList[i]._RootObj.SetActive( i == (int)slotType );
			if ( !_DetailList[i]._RootObj.activeSelf ) {
				continue;
			}

			_DetailList[i]._DayLabel.textlocalize = FLocalizeString.Instance.GetText( param.Day );
			_DetailList[i]._TitleLabel.textlocalize = FLocalizeString.Instance.GetText( param.Name );

			if ( _DetailList[i]._CompleteObj != null ) {
				_DetailList[i]._CompleteObj.SetActive( slotType == eSlotType.BASIC && index < selectIndex );
			}
		}
	}

	public void OnClick_Slot() {
		if ( ParentGO == null ) {
			return;
		}
	}
}
