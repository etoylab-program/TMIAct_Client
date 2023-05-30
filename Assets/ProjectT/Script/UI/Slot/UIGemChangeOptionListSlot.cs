using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGemChangeOptionListSlot : FSlot {
	[Header( "UIGemChangeOptionListSlot" )]
	[SerializeField] private UILabel _OptionLabel;
	[SerializeField] private GameObject _OptionLock;

	private int mIndex;

	public void UpdateSlot( int index, GameTable.GemRandOpt.Param param, ref GemData gemData, ref int selectOptionIndex ) {
		if ( param == null ) {
			return;
		}

		mIndex = index;

		int value = param.Min + (int)( (float)param.RndStep * param.Value );
		float v = value / (float)eCOUNT.MAX_RATE_VALUE;

		if ( param.EffectType.Contains( "Penetrate" ) || param.EffectType.Contains( "Sufferance" ) ) {
			v /= 10.0f;
		}

		_OptionLabel.textlocalize = FLocalizeString.Instance.GetText( param.Desc, v * 100.0f );

		bool isLock = false;
		if ( gemData != null && 0 <= selectOptionIndex ) {
			if ( param.ID == gemData.RandOptID[selectOptionIndex] ) {
				isLock = param.RndStep <= gemData.RandOptValue[selectOptionIndex];
			}
		}

		_OptionLock.SetActive( isLock );
	}

	public void OnClick_Slot() {
		if ( _OptionLock.activeSelf ) {
			return;
		}

		if ( ParentGO == null ) {
			return;
		}

		UIGemOptChangeAutoPopup gemOptChangeAutoPopup = ParentGO.GetComponent<UIGemOptChangeAutoPopup>();
		if ( gemOptChangeAutoPopup != null ) {
			gemOptChangeAutoPopup.SelectOption( mIndex );
		}
	}
}
