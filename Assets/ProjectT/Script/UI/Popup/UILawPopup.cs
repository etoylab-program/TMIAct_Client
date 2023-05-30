using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILawPopup : FComponent
{
    public UILabel kTitleLabel;
    public UILabel kAgreeLabel;
    public UITextList kTermsofUseLabel;
    public UITextList kPrivacyPolicyLabel;


    public override void OnEnable()
    {
        InitComponent();

        base.OnEnable();
    }

    public override void InitComponent()
    {
        kTitleLabel.textlocalize = FLocalizeString.Instance.GetText(909);
        kAgreeLabel.textlocalize = FLocalizeString.Instance.GetText(910);

        string strterms = string.Empty;
        string strprivacy = string.Empty;

        if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Japan)
        {
            TextAsset txterms = ResourceMgr.Instance.LoadResourceFromCache("Text/TermsofUse_JPN", typeof(TextAsset)) as TextAsset;
            if (txterms != null)
            {
                strterms = txterms.text;
            }

            TextAsset txprivacy = ResourceMgr.Instance.LoadResourceFromCache("Text/PrivacyPolicy_JPN", typeof(TextAsset)) as TextAsset;
            if (txprivacy != null)
            {
                strprivacy = txprivacy.text;
            }
        }
        else
        {
            strterms = FLocalizeString.Instance.GetText(200000003);
            strprivacy = FLocalizeString.Instance.GetText(200000002);
        }

        kTermsofUseLabel.Add(strterms);
        kTermsofUseLabel.textLabel.textlocalize = strterms;

        kPrivacyPolicyLabel.Add(strprivacy);
        kPrivacyPolicyLabel.textLabel.textlocalize = strprivacy;
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);
    }


    public void OnClick_CloseBtn()
    {
        TitleUIManager.Instance.PlaySound(0);
        TitleUIManager.Instance.OnMsg_Exit();
        OnClickClose();
    }

    public void OnClick_OKBtn()
    {
        GameSupport.SendFireBaseLogEvent(eFireBaseLogType._Agreement);
        TitleUIManager.Instance.PlaySound(0);
        OnClickClose();
        PlayerPrefs.SetInt(SAVETYPE.TERMSOFUSE.ToString(), 1);

        TitleUIManager.Instance.OnMsg_Exit();
        TitleUIManager.Instance.OnClick_Progress();
    }
}
