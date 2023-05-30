using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAccountGetCodePopup : FComponent
{
    public UILabel kCodeLabel;
    public UILabel kPasswordLabel;
    private string _password;
    private UIInput _passwordInputField;


    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
        _password = "";
        kCodeLabel.textlocalize = "";
        kPasswordLabel.textlocalize = "";

        if (kPasswordLabel.GetComponent<UIInput>() != null)
        {
            _passwordInputField = kPasswordLabel.GetComponent<UIInput>();
            
        }
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        kCodeLabel.textlocalize = GameInfo.Instance.UserData.AccountCode;

        if (_password != "" || _password != string.Empty)
            kPasswordLabel.textlocalize = _password;
        else if (GameInfo.Instance.UserData.PasswordSet)
        {
            kPasswordLabel.textlocalize = FLocalizeString.Instance.GetText(1049);
            if (_passwordInputField != null)
                _passwordInputField.defaultText = kPasswordLabel.textlocalize;
        }
        else
            kPasswordLabel.textlocalize = "";
    }

    public void OnClick_PasswordBtn()
    {
        //8889 패스워드 입력받기
        _password = kPasswordLabel.textlocalize;
    }
    public void OnClick_ApplyBtn()
    {
        //8889 자리수 체크 6~14
        if(_password.Length < 6 || _password.Length > 14)
        {
            MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3091), FLocalizeString.Instance.GetText(905), "6~14"));
            return;
        }
        
        //특수문자 검사
        if(GameSupport.CheckSpecialText(_password))
        {
            MessagePopup.OK(eTEXTID.OK, string.Format(FLocalizeString.Instance.GetText(3092), FLocalizeString.Instance.GetText(905)), null);
            return;
        }

        GameInfo.Instance.Send_ReqAccountSetPassword(_password, OnNetAccountSetPassword);
    }

    public void OnClick_CloseBtn()
    {
        OnClickClose();
    }

    public void OnNetAccountSetPassword(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;
        
        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3089));
        Renewal(true);
        LobbyUIManager.Instance.Renewal("AccountLinkingPopup");
    }


    //Copy To ClipBoard
    public void OnClick_CopyToUserAccountID()
    {
        UniClipboard.SetText(GameInfo.Instance.UserData.AccountCode);
        MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3175), FLocalizeString.Instance.GetText(901)));
    }

    public override void OnClickClose()
    {
        _password = string.Empty;
        kPasswordLabel.text = string.Empty;
        base.OnClickClose();
    }
}
