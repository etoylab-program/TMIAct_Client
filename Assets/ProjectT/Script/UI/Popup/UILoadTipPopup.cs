using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILoadTipPopup : FComponent
{
    public UILabel kNameLabel;
    public UILabel kCvLabel;
    public UILabel kIllustLabel;
    public UILabel kTalkLabel;
    public UITextList kTextList;
    public UIButton kCvPlayBtn;
    public UIButton kArrow_RBtn;
    public UIButton kArrow_LBtn;
	public UIButton BtnPV;

    private int _charid = 1;


	public override void OnEnable() {
		_charid = 1;
		BtnPV.SetActive( TitleUIManager.Instance.IsPVPlay );

		base.OnEnable();
	}

	public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        string strname = string.Empty;
        string strcv = string.Empty;
        string strillust = string.Empty;
        string strtalk = string.Empty;
        string strtext = string.Empty;

        strname = FLocalizeString.Instance.GetText(960 + ((_charid - 1) * 10) + 0);
        strcv = FLocalizeString.Instance.GetText(960 + ((_charid - 1) * 10) + 1);
        strillust = FLocalizeString.Instance.GetText(960 + ((_charid - 1) * 10) + 2);
        strtalk = FLocalizeString.Instance.GetText(960 + ((_charid - 1) * 10) + 3);
        strtext = FLocalizeString.Instance.GetText(960 + ((_charid - 1) * 10) + 4);

        kNameLabel.textlocalize = strname;
        kCvLabel.textlocalize = strcv;
        kIllustLabel.textlocalize = strillust;
        kTalkLabel.textlocalize = strtalk;
        kTextList.textLabel.textlocalize = "";
        kTextList.Clear();
        kTextList.Add(strtext);
        kTextList.textLabel.textlocalize = strtext;
    }

    public void OnClick_CvPlayBtn()
    {
        TitleUIManager.Instance.PlaySound(_charid);
    }

    public void OnClick_Arrow_LBtn()
    {
        TitleUIManager.Instance.StopSound(_charid);
        _charid -= 1;
        if (_charid <= 0)
            _charid = 3;
        
        TitleUIManager.Instance.PlaySound(0);
        Renewal(true);
    }

    public void OnClick_Arrow_RBtn()
    {
        TitleUIManager.Instance.StopSound(_charid);
        _charid += 1;
        if (_charid > 3)
            _charid = 1;

        
        TitleUIManager.Instance.PlaySound(0);
        Renewal(true);
    }

    public override bool IsBackButton()
    {
        return false;
    }
}
