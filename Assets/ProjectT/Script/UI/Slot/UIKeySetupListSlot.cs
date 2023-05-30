using UnityEngine;
using System.Collections;
using eKeyKind = BaseCustomInput.eKeyKind;
using sKeyMapping = BaseCustomInput.sKeyMapping;

public class UIKeySetupListSlot : FSlot {
 

	public FLocalizeText kOperationLabel;
	public FLocalizeText kKeyLabel;

	private eKeyKind _eKeyKind;

	public void UpdateSlot() {
		kOperationLabel.SetLabel( AppMgr.Instance.CustomInput.GetTextID( _eKeyKind ) );
		sKeyMapping _sKeyMapping =  AppMgr.Instance.CustomInput.GetKeyMapping(_eKeyKind);

		if( _sKeyMapping == null ) {
			kKeyLabel.SetLabel( string.Empty );
		}
		else {
			if( _sKeyMapping.GetPCKey() == KeyCode.None ) {
				kKeyLabel.SetLabel( string.Empty );
			}
			else {
				KeyCode keyCode = _sKeyMapping.GetPCKey();
				string keyname = keyCode.ToString();

				if( keyCode >= KeyCode.Alpha0 && keyCode <= KeyCode.Alpha9 ) {
					keyname = keyCode.ToString();
					keyname = keyname.Replace( "Alpha", "" );
				}
				else {
					switch( keyCode ) {
						case KeyCode.Mouse0:
							keyname = "Mouse Left Button";
							break;
						case KeyCode.Mouse1:
							keyname = "Mouse Right Button";
							break;
					}
				}

				kKeyLabel.SetLabel( keyname );
			}
		}
	}

	public void OnClick_Slot()
	{
	}
	
	public void OnClick_KeyBtn()
	{
		UIKeyConfirmPopup popup = null;
		if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
		{
			popup = LobbyUIManager.Instance.ShowUI("KeyConfirmPopup", true) as UIKeyConfirmPopup;
		}
		else
		{
			popup = GameUIManager.Instance.ShowUI("KeyConfirmPopup", false) as UIKeyConfirmPopup;
		}

		if(popup != null)
        {
			/*
			popup.SetData(_eKeyKind, () => {
				var _UIKeySetupPopup = ParentGO.GetComponent<UIKeySetupPopup>();
				if (_UIKeySetupPopup != null) _UIKeySetupPopup.Refresh();
			});
			*/

			popup.SetData(_eKeyKind, OnYes);
		}
	}

	public void SetData(eKeyKind key)
    {
		_eKeyKind = key;		
	}

	private void OnYes()
	{
		UIOptionPopup popup = ParentGO.GetComponent<UIOptionPopup>();
		if(popup)
		{
			popup.RefreshKeySetting();
		}
	}
}
