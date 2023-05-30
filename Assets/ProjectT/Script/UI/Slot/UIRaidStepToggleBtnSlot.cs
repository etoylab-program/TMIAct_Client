
using UnityEngine;
using System.Collections;


public class UIRaidStepToggleBtnSlot : FSlot {
	[SerializeField] private UISprite   _OnSelSpr;
	[SerializeField] private UILabel    _OnStepLabel;
	[SerializeField] private UILabel    _OnNumberLabel;
	[SerializeField] private UISprite	_OffSpr;
	[SerializeField] private UILabel    _OffStepLabel;
	[SerializeField] private UILabel    _OffNumberLabel;

	private int mStepNumber = 0;


	public void UpdateSlot( int index )
	{
		mStepNumber = index + 1;
		bool isSelect = mStepNumber == GameInfo.Instance.SelectedRaidLevel;

		_OnSelSpr.gameObject.SetActive( isSelect );
		_OnStepLabel.textlocalize = FLocalizeString.Instance.GetText( 3318 );
		_OnNumberLabel.textlocalize = mStepNumber.ToString( "D2" );

		_OffSpr.gameObject.SetActive( !isSelect );
		_OffNumberLabel.textlocalize = mStepNumber.ToString( "D2" );
		_OffStepLabel.textlocalize = FLocalizeString.Instance.GetText( 3318 );
	}

	public void OnBtn() {
		if( ParentGO == null ) {
			return;
		}

		UIRaidPanel panel = ParentGO.GetComponent<UIRaidPanel>();
		if( panel == null ) {
			return;
		}

		panel.SelectStep( mStepNumber );
	}
}
