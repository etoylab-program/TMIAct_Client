using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameRewardPopup : FComponent
{
    public UILabel kTitleLabel;
    public UILabel kComentLabel;
    public UILabel kRewardCountLabel;

    
    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);
        kRewardCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.STACK_COUNT_TXT), GameInfo.Instance.GameConfig.StageMissionClearCash);
    }

    public void OnClick_ConfirmBtn()
    {
        World.Instance.OnEndResult();
        SetUIActive(false, true);
    }

}
