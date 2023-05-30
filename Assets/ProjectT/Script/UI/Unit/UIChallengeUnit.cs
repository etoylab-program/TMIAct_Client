using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIChallengeUnit : FUnit {

    public UISprite kIconSpr;
    public UILabel kTitleLabel;

    public void InitChallengeUnit(GameClientTable.StageMission.Param missiondata, int clear)
    {
        if (missiondata == null) return;

        kTitleLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(missiondata.Desc), missiondata.ConditionValue);

        if (clear == (int)eCOUNT.NONE)
        {
            kIconSpr.gameObject.SetActive(false);
        }
        else
        {
            kIconSpr.gameObject.SetActive(true);
        }
        
    }

    public void InitChallengeUnit(GameClientTable.StageMission.Param missiondata, bool clear)
    {
        kTitleLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(missiondata.Desc), missiondata.ConditionValue);

        if (!clear)
        {
            kIconSpr.gameObject.SetActive(false);
        }
        else
        {
            kIconSpr.gameObject.SetActive(true);
        }

    }
}
