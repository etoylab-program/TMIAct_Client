using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGaugeUnit : FUnit
{

    public UISprite kGaugeBGSpr;
    public UISprite kGaugeSpr;
    public UILabel kTextLabel;

    public void InitGaugeUnit(float f, int texttype = 0)
    {
        kGaugeSpr.fillAmount = f;

        if (kTextLabel != null)
        {
            if (texttype == 0)
                kTextLabel.gameObject.SetActive(false);
            else
                kTextLabel.gameObject.SetActive(true);
        }
    }

    public void SetText(string text)
    {
        if (kTextLabel == null)
            return;

        kTextLabel.gameObject.SetActive(true);
        kTextLabel.textlocalize = text;
    }

    public void SetColor(int coloridx)
    {
        kGaugeSpr.color = GameInfo.Instance.GameConfig.GemGaugeColor[coloridx];
    }
}
