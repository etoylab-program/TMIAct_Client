using UnityEngine;
using System.Collections;

public class UIStatusUnit : FUnit
{
    public UILabel kTextLabel;
    public UILabel kValueLabel;
    public UILabel kAddLabel;

    public void InitStatusUnit( string text, int value ) 	//Fill parameter if you need
	{
        kTextLabel.textlocalize = text;
        kValueLabel.textlocalize = string.Format("{0:#,##0}", value );

        if (kAddLabel)
            kAddLabel.textlocalize = "";
    }

    public void InitStatusUnit(int textid, int value)  //Fill parameter if you need
    {
        kTextLabel.textlocalize = FLocalizeString.Instance.GetText(textid);
        kValueLabel.textlocalize = string.Format("{0:#,##0}", value);

        if(kAddLabel)
            kAddLabel.textlocalize = "";
    }

    public void InitStatusUnit(string text, string value)  //Fill parameter if you need
    {
        kTextLabel.textlocalize = text;
        kValueLabel.textlocalize = value;

        if (kAddLabel)
            kAddLabel.textlocalize = "";
    }

    public void InitStatusUnit(int textid, int value, int add, float percent = 0.0f)  //Fill parameter if you need
    {
        kTextLabel.textlocalize = FLocalizeString.Instance.GetText(textid);
        kValueLabel.textlocalize = string.Format("{0:#,##0}", value);

        if(value == add)
        {
            if (kAddLabel)
                kAddLabel.textlocalize = string.Format("{0:#,##0}", add);
        }
        else
        {
            if (kAddLabel)
            {
                if (percent == 0.0f)
                {
                    if (value < add)
                        kAddLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR), string.Format("{0:#,##0} (+{1:#,##0})", add, add - value));
                    else
                        kAddLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.RED_TEXT_COLOR), string.Format("{0:#,##0} ({1:#,##0})", add, add - value));
                }
                else
                {
                    if (value < add)
                        kAddLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR), string.Format("{0:#,##0} (+{1:#,##0.#}%)", add, percent));
                    else
                        kAddLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.RED_TEXT_COLOR), string.Format("{0:#,##0} ({1:#,##0.#}%)", add, percent));
                }
            }
        }
    }
       
    public void SetValueText( string text)
    {
        kValueLabel.textlocalize = text;// string.Format("{0:#,##0}", text);
    }

    public void SetAddText(string text)
    {
        if (kAddLabel)
            kAddLabel.textlocalize = text;// string.Format("{0:#,##0}", text);
    }

    public void OnClick_Slot()
	{
	}
 
}
