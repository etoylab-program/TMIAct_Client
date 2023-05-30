using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIServerIDPopup : FComponent
{
    public UILabel kIDLabel;
    public UILabel kPW1stLabel;
    public UILabel kPW2ndLabel;

    public UIInput InputId;
    public UIInput kPW1stInput;
    public UIInput kPW2ndInput;

    public UILabel LbCount;

    private string ID;
    private string PW1st, PW2nd;


    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
        ID = string.Empty;
        PW1st = string.Empty;
        PW2nd = string.Empty;

        kIDLabel.text = string.Empty;
        kPW1stLabel.text = string.Empty;
        kPW2ndLabel.text = string.Empty;

        InputId.value = string.Empty;
        kPW1stInput.value = string.Empty;
        kPW2ndInput.value = string.Empty;
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        ID = kIDLabel.text;
        PW1st = kPW1stLabel.text;
        PW2nd = kPW2ndLabel.text;

        string c = "[FFFFFFFF]";
        if (GameInfo.Instance.ServerMigrationCountOnWeek >= GameInfo.Instance.GameTable.ServerMergeSets[0].MaxCount)
        {
            c = "[FF0000FF]";
        }

        LbCount.textlocalize = string.Format(FLocalizeString.Instance.GetText(6021), 
                                             c, GameInfo.Instance.ServerMigrationCountOnWeek, GameInfo.Instance.GameTable.ServerMergeSets[0].MaxCount);
    }

    public void OnClick_ID()
    {
        ID = kIDLabel.text;
    }

    public void OnClick_PW_1()
    {
        PW1st = kPW1stInput.value;
    }

    public void OnClick_PW_2()
    {
        PW2nd = kPW2ndInput.value;
    }

    public void OnClick_Apply()
    {
        if (string.IsNullOrEmpty(ID) || string.IsNullOrEmpty(PW1st) || string.IsNullOrEmpty(PW2nd))
        {
            //3093 입력 항목중에 빈 항목이 있습니다.
            MessagePopup.OK(eTEXTID.OK, FLocalizeString.Instance.GetText(3093), null);
            return;
        }

        if (GameInfo.Instance.ServerMigrationCountOnWeek >= GameInfo.Instance.GameTable.ServerMergeSets[0].MaxCount)
        {
            // 이번주 신청 끝
            MessagePopup.OK(eTEXTID.OK, FLocalizeString.Instance.GetText(6022), null);
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

        if (CheckLength(PW1st) || CheckLength(PW2nd))
        {
            MessagePopup.OK(eTEXTID.OK, string.Format(FLocalizeString.Instance.GetText(3091), FLocalizeString.Instance.GetText(905), "6~14"), null);
            return;
        }

        if (GameSupport.CheckSpecialText(ID) || GameSupport.CheckSpecialText(PW1st) || GameSupport.CheckSpecialText(PW2nd))
        {
            //3094    입력 항목중에 특수문자가 포함되어 있습니다.
            MessagePopup.OK(eTEXTID.OK, FLocalizeString.Instance.GetText(3094), null);
            return;
        }

        if (!PW1st.Equals(PW2nd))
        {   
            MessagePopup.OK(eTEXTID.OK, FLocalizeString.Instance.GetText(6014), null);
            return;
        }
        Debug.Log("Send_ReqRelocateUserInfoSet");

        MessagePopup.OKCANCEL(eTEXTID.TITLE_NOTICE, string.Format(FLocalizeString.Instance.GetText(6015), ID, PW1st), () => { GameInfo.Instance.Send_ReqRelocateUserInfoSet(ID, PW1st, OnNetAckRelocateUserInofSet); }, null);
    }

    private void OnNetAckRelocateUserInofSet(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        MessagePopup.OK(eTEXTID.OK, FLocalizeString.Instance.GetText(6005), () =>
        {   
            LobbyUIManager.Instance.Renewal("TopPanel");
            OnClickClose();            
        });
    }
}
