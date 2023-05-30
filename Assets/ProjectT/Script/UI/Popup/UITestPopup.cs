using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITestPopup : FComponent
{
    public UILabel kTextLabel;


    public override void OnEnable()
    {
        Invoke("OnClickClose", GameInfo.Instance.GameConfig.TestDuration);
        base.OnEnable();
    }
    
    public override void Renewal(bool bChildren)
    {
        var obj = UIValue.Instance.GetValue(UIValue.EParamType.TestString);
        if (obj == null)
            return;
        kTextLabel.textlocalize = (string)obj;
    }

    public override void OnClose()
    {
        base.OnClose();
    }

    public void OnClick_BG()
    {
        OnClickClose();
    }
}
