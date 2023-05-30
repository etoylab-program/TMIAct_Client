
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIUsernamecolorPopup : FComponent {
	[SerializeField] private FList		_ColorList;
	[SerializeField] private UIPanel    _ColorListPanel;
	[SerializeField] private UILabel	_TitleLabel;
	[SerializeField] private UILabel	_DescLabel;
	[SerializeField] private UILabel    _NameLabel;

	private int				mSelectedColorIndex	= 0;
	private StringBuilder	mSb					= new StringBuilder();


	public override void Awake() {
		base.Awake();

		_ColorList.EventGetItemCount = GetColorCount;
		_ColorList.EventUpdate = UpdateColorSlot;
	}

	public override void OnEnable() {
		base.OnEnable();

		_TitleLabel.textlocalize = FLocalizeString.Instance.GetText( 3305 );
		_DescLabel.textlocalize = FLocalizeString.Instance.GetText( 3306 );

		SelectColor( GameInfo.Instance.UserData.NickNameColorId );
		Renewal( true );
	}

	public override void Renewal( bool bChildren = false ) {
		base.Renewal( bChildren );
		UpdateColorList();
	}

	public void SelectColor( int selectedIndex ) {
		mSelectedColorIndex = selectedIndex;
		_NameLabel.textlocalize = Utility.GetColoredNickName( GameInfo.Instance.UserData.GetRawNickName(), mSelectedColorIndex, mSb );
	}

	public void UpdateColorList() {
		_ColorList.UpdateList();
	}

	public void OnBtnOk() {
		if( mSelectedColorIndex == GameInfo.Instance.UserData.NickNameColorId ) {
			OnClickClose();
			return;
		}

		GameInfo.Instance.Send_ReqUserSetNameColor( mSelectedColorIndex, OnChangeUserNameColor );
	}

	public void OnBtnCancel() {
		OnClickClose();
	}

	private int GetColorCount() {
		return GameInfo.Instance.GameClientTable.NameColors.Count;
	}

	private void UpdateColorSlot( int index, GameObject slotObject ) {
		UINickNameColorSlot slot = slotObject.GetComponent<UINickNameColorSlot>();
		if( slot == null ) {
			return;
		}

		slot.ParentGO = gameObject;
		slot.UpdateSlot( index, mSelectedColorIndex );
	}

	private void OnChangeUserNameColor( int result, PktMsgType pktmsg ) {
		if( result != 0 ) {
			return;
		}

		UIUserInfoPopup popup = LobbyUIManager.Instance.GetActiveUI<UIUserInfoPopup>( "UserInfoPopup" );
		if( popup ) {
			popup.Renewal( true );
		}

		UITopPanel panel = LobbyUIManager.Instance.GetUI<UITopPanel>( "TopPanel" );
		if( panel ) {
			panel.Renewal( true );
		}

		OnClickClose();
	}

	private void Update() {
		_ColorListPanel.transform.position = Vector3.zero;
	}
}
