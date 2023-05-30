using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAccountCodePopup : FComponent
{

    public UILabel kCodeLabel;
    public UIInput kCodeInputField;
    public UILabel kPasswordLabel;
    public UIInput kPasswordInputField;
    private string _code;
    private string _password; 


    public override void OnEnable()
    {
		_password = "";
		kCodeLabel.textlocalize = "";
		kPasswordLabel.textlocalize = "";

		base.OnEnable();
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        _code = kCodeInputField.value;
        _password = kPasswordInputField.value;
    }
    public void OnClick_CodeBtn()
    {
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
            TitleUIManager.Instance.PlaySound(0);

        if (kCodeInputField == null)
        {
            return;
        }
        //8889 코드 입력받기
        _code = kCodeInputField.value;
    }
    public void OnClick_PasswordBtn()
    {
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
            TitleUIManager.Instance.PlaySound(0);

        //8889 패스워드 입력받기
        if (kPasswordInputField == null)
            return;
        _password = kPasswordInputField.value;
    }
    public void OnClick_LoadBtn()
    {
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
            TitleUIManager.Instance.PlaySound(0);

        //예외상항 체크
        if (string.IsNullOrEmpty(_code) || string.IsNullOrEmpty(_password))
        {
            //3093 입력 항목중에 빈 항목이 있습니다.
            MessagePopup.OK(eTEXTID.OK, FLocalizeString.Instance.GetText(3093), null);
            return;
        }

        if(_code.Length != 9)
        {
            //3091    {0} 입력범위는 {1}자 입니다.
            MessagePopup.OK(eTEXTID.OK, string.Format(FLocalizeString.Instance.GetText(3091), FLocalizeString.Instance.GetText(904), 9), null);
            return;
        }

        if(_password.Length < 6 || _password.Length > 14)
        {
            //3091    {0} 입력범위는 {1}자 입니다.
            MessagePopup.OK(eTEXTID.OK, string.Format(FLocalizeString.Instance.GetText(3091), FLocalizeString.Instance.GetText(905), "6~14"), null);
            return;
        }

        if(GameSupport.CheckSpecialText(_code) || GameSupport.CheckSpecialText(_password))
        {
            //3094    입력 항목중에 특수문자가 포함되어 있습니다.
            MessagePopup.OK(eTEXTID.OK, FLocalizeString.Instance.GetText(3094), null);
            return;
        }
        
        GameInfo.Instance.StartTimeoutCheckSend(TimeOutCallback);
        GameInfo.Instance.Send_ReqGetUserInfoFromAccountLink( eAccountType.DEFAULT, _code, _password, OnNetGetUserInfoFromAccountLink);
        WaitPopup.Show();
    }
    public void OnClick_CloseBtn()
    {
        OnClickClose();
    }

    public void OnNetGetUserInfoFromAccountLink(int result, PktMsgType pktmsg)
    {
        GameInfo.Instance.StopTimeOut();
        Renewal(true);
        
        CloseAccountConnectPopup();
        OnClickClose();
        WaitPopup.Hide();

        if (Nettention.Proud.ErrorType.Ok != (Nettention.Proud.ErrorType)result)
        {
            Nettention.Proud.ErrorType errorType = (Nettention.Proud.ErrorType)result;
            Log.Show("ErrorType : " + errorType, Log.ColorType.Red);
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(110050));
            return;
        }
        PlayerPrefs.DeleteKey("LobbyBGWithScreenShot");
        GameSupport.SendFireBaseLogEvent(eFireBaseLogType._Prev_Device);
        TitleUIManager.Instance.OnClick_Start();
    }

    //이어하기 연동이 성공하면 뒤에 깔려있는 연동 팝업도 꺼준다.
    void CloseAccountConnectPopup()
    {
        UIAccountConnectPopup popup = null;
        if(AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
        {
            popup = TitleUIManager.Instance.GetUI<UIAccountConnectPopup>("AccountConnectPopup");
            if(popup != null)
            {
                popup.SetUIActive(false, false);
            }
        }
    }

    void TimeOutCallback()
    {
        //연결된 Login 서버 연결 끊기.
        TitleUIManager.Instance.OnMsg_Exit();
        CloseAccountConnectPopup();
        OnClickClose();
        WaitPopup.Hide();
        MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(110050), null);
    }
}
