using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using eKeyKind = BaseCustomInput.eKeyKind;
using sKeyMapping = BaseCustomInput.sKeyMapping;

public class UIKeyConfirmPopup : FComponent
{


    public FLocalizeText kTitleLabel;
    public FLocalizeText kTextLabel;
    public FLocalizeText kKeyUnitTextLabel;

    public GameObject kKeyUnitObject;
    public GameObject kConfirmObject;

    public FLocalizeText kBeforeKeyLabel;
    public FLocalizeText kAfterKeyLabel;
    public UIButton kYesBtn;
    public UIButton kNoBtn;

    public delegate void CallBack();

    private CallBack CompleteCallBack;

    private enum eKeyMappinMode
    {
        None = 0,
        Input,
        Conflict,
    }
    private eKeyMappinMode _eKeyMappinMode = eKeyMappinMode.None;
    private eKeyKind _eKeyKind;
    private sKeyMapping _sKeyMapping, _leftKeyMapping;
    private KeyCode _InputkeyCode;


	public override void OnEnable() {
		kTitleLabel.SetLabel( 1542 );
		kTextLabel.SetLabel( 1543 );

		base.OnEnable();
	}

	private void OnGUI()
    {
        switch (_eKeyMappinMode)
        {
            case eKeyMappinMode.Input: OnGUI_Input(); break;
        }
    }

    private void OnGUI_Input()
    {
        if (Event.current == null) return;

        if (Event.current.isKey)
        {
            _InputkeyCode = Event.current.keyCode;

            switch (_InputkeyCode)
            {
                case KeyCode.PageUp:
                case KeyCode.PageDown:
                case KeyCode.Escape:
                    _InputkeyCode = KeyCode.None;
                    MessageToastPopup.Show(FLocalizeString.Instance.GetText(3200));                    
                    return;
            }
        }
        else if (Event.current.isMouse)
        {
            if (Event.current.type == EventType.MouseDown)
            {
                switch (Event.current.button)
                {
                    case 0: _InputkeyCode = KeyCode.Mouse0; break;
                    case 1: _InputkeyCode = KeyCode.Mouse1; break;
                    case 2: _InputkeyCode = KeyCode.Mouse2; break;
                    case 3: _InputkeyCode = KeyCode.Mouse3; break;
                    case 4: _InputkeyCode = KeyCode.Mouse4; break;
                    case 5: _InputkeyCode = KeyCode.Mouse5; break;
                    case 6: _InputkeyCode = KeyCode.Mouse6; break;
                }
            }
            else
                return;
        }
        else
            return;


        if (AppMgr.Instance.CustomInput.IsConflictPCKeyCode(_eKeyKind, _InputkeyCode))
        {
            // 다른키와 충돌
            _eKeyMappinMode = eKeyMappinMode.None;

            kTextLabel.SetLabel(583);

            kKeyUnitObject.SetActive(false);
            kConfirmObject.SetActive(true);

            _leftKeyMapping = AppMgr.Instance.CustomInput.GetKeyMapping(_InputkeyCode);
            kBeforeKeyLabel.SetLabel(string.Format("{0} [FFE44C]{1}[-]",
                FLocalizeString.Instance.GetText(AppMgr.Instance.CustomInput.GetTextID(_leftKeyMapping.GetKeyKind())),
                _leftKeyMapping.GetPCKey().ToString()));

            kAfterKeyLabel.SetLabel(string.Format("{0} [FFE44C]{1}[-]",
                FLocalizeString.Instance.GetText(AppMgr.Instance.CustomInput.GetTextID(_eKeyKind)),
                _sKeyMapping.GetPCKey().ToString()));

        }
        else
        {
            AppMgr.Instance.CustomInput.ChangePCKey(_eKeyKind, _InputkeyCode);
            // 키 설정 완료
            _eKeyMappinMode = eKeyMappinMode.None;
            if (CompleteCallBack != null)
                CompleteCallBack();

            OnClickClose();
        }
    }

	public override void OnClickClose()
	{
		SetUIActive(false, AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby);
	}

	public void OnClick_BGBtn()
    {
        //OnClickClose();
    }

    public void OnClick_YesBtn()
    {
        KeyCode leftoldKecode = _leftKeyMapping.GetPCKey();
        AppMgr.Instance.CustomInput.ChangePCKey(_leftKeyMapping.GetKeyKind(), _sKeyMapping.GetPCKey());
        AppMgr.Instance.CustomInput.ChangePCKey(_eKeyKind, leftoldKecode);

        if (CompleteCallBack != null)
            CompleteCallBack();

        OnClickClose();
    }

    public void OnClick_NoBtn()
    {
        OnClickClose();
    }

	public void SetData( eKeyKind key, CallBack yescb = null ) {
		_eKeyKind = key;
		CompleteCallBack = yescb;

		kKeyUnitObject.SetActive( true );
		kConfirmObject.SetActive( false );

		_sKeyMapping = AppMgr.Instance.CustomInput.GetKeyMapping( _eKeyKind );
        KeyCode keyCode = _sKeyMapping.GetPCKey();

        if( _sKeyMapping == null || keyCode == KeyCode.None ) {
			kKeyUnitTextLabel.SetLabel( string.Empty );
		}
		else {
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

			kKeyUnitTextLabel.SetLabel( string.Format( "{0} [FFE44C]{1}[-]", 
                                                       FLocalizeString.Instance.GetText( AppMgr.Instance.CustomInput.GetTextID( _eKeyKind ) ), keyname ) );
		}

		_eKeyMappinMode = eKeyMappinMode.Input;
	}
}
