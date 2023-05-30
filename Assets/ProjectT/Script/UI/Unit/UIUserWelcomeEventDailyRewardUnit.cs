using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIUserWelcomeEventDailyRewardUnit : FUnit
{
    public UIRewardListSlot ReweardListSlot;
    public UISprite kEnableEffectSpr;
    public UILabel kDayNumberLbl;
    public UISprite kReceiveCompleteSpr;
    public UISprite kReceiveStampSpr;
    public UISprite kAwardCompleteSpr;

    private void DisableComponents()
    {
        ReweardListSlot.SetActive(false);
        kEnableEffectSpr.SetActive(false);
        kDayNumberLbl.SetActive(false);
        kReceiveCompleteSpr.SetActive(false);
        kReceiveStampSpr.SetActive(false);
        kAwardCompleteSpr.SetActive(false);
    }

    public void Init()
    {
        DisableComponents();
    }


}
