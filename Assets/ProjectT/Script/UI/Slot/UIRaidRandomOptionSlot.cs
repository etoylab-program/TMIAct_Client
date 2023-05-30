
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIRaidRandomOptionSlot : FSlot {
	[SerializeField] private UISprite	_Spr;
	[SerializeField] private UILabel	_Label;


	public void UpdateSlot( int index, GameClientTable.StageBOSet.Param param ) {
		_Spr.spriteName = param.Icon;

		if( _Label ) {
			_Label.textlocalize = FLocalizeString.Instance.GetText( param.Desc );
		}
	}
}
