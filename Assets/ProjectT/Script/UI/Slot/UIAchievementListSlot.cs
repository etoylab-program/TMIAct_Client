using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIAchievementListSlot : FSlot {
    public UIRewardListSlot _RewardSlot;
    public UIGaugeUnit kGaugeUnit;
    public UISprite kbgSpr;
    public UISprite kicoSpr;
    public UILabel kAcievementLabel;
    public UILabel kAcievementTitleLabel;
    public UILabel kAcievementDescLabel;
    public UISprite kCompleteSpr;
    public UIButton kGetBtn;
    public UISprite kGaugeEFFSpr;
    private int _index;
    private AchieveData _achievedata;
    private GameTable.Achieve.Param _tabledata;


    public void UpdateSlot(int index, AchieveData achievedata)  //Fill parameter if you need
    {
        _index = index;
        _achievedata = achievedata;
        bool brewardcomplete = false;
        kGetBtn.gameObject.SetActive(false);
        kCompleteSpr.gameObject.SetActive(false);
        if (kGaugeEFFSpr != null)
            kGaugeEFFSpr.gameObject.SetActive(false);

        _tabledata = achievedata.TableData;
        brewardcomplete = achievedata.bTotalComplete;
        
        RewardData rewarddata = new RewardData(_tabledata.ProductType, _tabledata.ProductIndex, _tabledata.ProductValue);
        _RewardSlot.UpdateSlot(rewarddata, true);

        kAcievementLabel.textlocalize = _tabledata.RewardAchievePoint.ToString();
        kAcievementTitleLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(_tabledata.Name), _tabledata.GroupOrder);


        int NowValue = _achievedata.Value;
        int MaxValue = _tabledata.AchieveValue;
        float f = 0.0f;
        string text = string.Empty;

        GameSupport.GetAchievementNowMaxValue(achievedata, ref NowValue, ref MaxValue);

        if (_tabledata.AchieveType == "AM_Adv_StoryClear") {
            string str = "";
            var stagedata = GameInfo.Instance.GameTable.FindStage(_tabledata.AchieveIndex);
            if (stagedata != null)
                str = string.Format(FLocalizeString.Instance.GetText(1145), stagedata.Chapter);
            kAcievementDescLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(_tabledata.Desc), str);
        }
        else {
            kAcievementDescLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(_tabledata.Desc), _tabledata.AchieveValue);
        }

        text = string.Format(FLocalizeString.Instance.GetText(278), NowValue, MaxValue);
        f = (float)NowValue / (float)MaxValue;
        kGaugeUnit.gameObject.SetActive(true);
        kGaugeUnit.InitGaugeUnit(f);
        kGaugeUnit.SetText(text);

        bool bcomplete = GameSupport.IsAchievementComplete(achievedata);

        if (brewardcomplete) {
            kCompleteSpr.gameObject.SetActive(true);
        }
        else {
            if (bcomplete) {
                kGetBtn.gameObject.SetActive(true);
                if (kGaugeEFFSpr != null) {
                    kGaugeEFFSpr.gameObject.SetActive(true);
                }
                else {
                    kGaugeUnit.gameObject.SetActive(false);
                }
            }
        }
    }

    public void OnClick_Slot() {

    }

    public void OnClick_GetBtn() {
        if (_achievedata == null)
            return;

        UIUserInfoPopup userinfopopup = ParentGO.GetComponent<UIUserInfoPopup>();
        if (userinfopopup == null)
            return;

        // Inven Check
        if (!GameSupport.IsCheckInven()) {
            return;
        }

        userinfopopup.OnClick_GetBtn(_achievedata.GroupID);
    }
}
