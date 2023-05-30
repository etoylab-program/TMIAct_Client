
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UINickNameColorSlot : FSlot
{
	[SerializeField] private UISprite	_SelectSpr;
	[SerializeField] private UISprite	_ColorSpr;

	private int mIndex = 0;


	public void UpdateSlot( int index, int selectedIndex ) {
		mIndex = index + 1;
		_SelectSpr.SetActive( mIndex == selectedIndex );

		GameClientTable.NameColor.Param param = GameInfo.Instance.GameClientTable.NameColors[mIndex - 1];
		if( param == null ) {
			_ColorSpr.color = Color.white;
		}
		else {
			_ColorSpr.color = Utility.GetColorFromHex( param.RGBColor );
		}
	}

	public void OnBtnSelect() {
		UIUsernamecolorPopup popup = ParentGO.GetComponent<UIUsernamecolorPopup>();
		if( popup && popup.isActiveAndEnabled ) {
			popup.SelectColor( mIndex );
			popup.UpdateColorList();
		}
	}
}
