using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIServerIDTitlePopup : FComponent
{
    public UILabel kIDLabel;
    public UILabel kPW1stLabel;
    public UIInput kPW1stInput;    

    private string ID;
    private string PW1st;


    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
        ID = string.Empty;
        PW1st = string.Empty;
        kIDLabel.text = string.Empty;
        kPW1stLabel.text = string.Empty;
        kPW1stInput.value = string.Empty;
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        ID = kIDLabel.text;
        PW1st = kPW1stInput.value;
    }

    public void OnClick_ID()
    {
        ID = kIDLabel.text;
    }

    public void OnClick_PW_1()
    {
        PW1st = kPW1stInput.value;
    }

    public void OnClick_Apply()
    {
        if (string.IsNullOrEmpty(ID) || string.IsNullOrEmpty(PW1st))
        {
            //3093 입력 항목중에 빈 항목이 있습니다.
            MessagePopup.OK(eTEXTID.OK, FLocalizeString.Instance.GetText(3093), null);
            return;
        }

        System.Func<string, bool> CheckLength = (str) =>
        {
            if (str.Length < 6 || str.Length > 14)
                return true;

            return false;
        };

        if (CheckLength(ID))
        {
            MessagePopup.OK(eTEXTID.OK, string.Format(FLocalizeString.Instance.GetText(3091), "ID", "6~14"), null);
            return;
        }

        if (CheckLength(PW1st))
        {
            MessagePopup.OK(eTEXTID.OK, string.Format(FLocalizeString.Instance.GetText(3091), FLocalizeString.Instance.GetText(905), "6~14"), null);
            return;
        }

        if (GameSupport.CheckSpecialText(ID) || GameSupport.CheckSpecialText(PW1st))
        {
            //3094    입력 항목중에 특수문자가 포함되어 있습니다.
            MessagePopup.OK(eTEXTID.OK, FLocalizeString.Instance.GetText(3094), null);
            return;
        }

        Debug.Log("Send_ReqRelocateUserInfoSet");
        GameInfo.Instance.Send_ReqRelocateUserComplate(ID, PW1st, OnNetAckRelocateUserComplate);
    }

    private void OnNetAckRelocateUserComplate(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        GameInfo.Instance.StopTimeOut();
        Renewal(true);
        
        OnClickClose();
        WaitPopup.Hide();

        if (Nettention.Proud.ErrorType.Ok != (Nettention.Proud.ErrorType)result)
        {
            Nettention.Proud.ErrorType errorType = (Nettention.Proud.ErrorType)result;
            Log.Show("ErrorType : " + errorType, Log.ColorType.Red);
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(110050));
            return;
        }

        MessagePopup.OK(FLocalizeString.Instance.GetText(6010), FLocalizeString.Instance.GetText(6011), FLocalizeString.Instance.GetText(1),
            () => 
            {
                GameSupport.SendFireBaseLogEvent(eFireBaseLogType._Prev_Server);
                TitleUIManager.Instance.OnClick_Start(); 
            });
        
    }
}
