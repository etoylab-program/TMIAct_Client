using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAchievementSlot : FSlot
{
    [SerializeField] private UIItemListSlot itemListSlot = null;
    [SerializeField] private UIGaugeUnit gaugeUnit = null;
    [SerializeField] private UIButton getBtn = null;
    [SerializeField] private UILabel titleLabel = null;
    [SerializeField] private UILabel infoLabel = null;
    [SerializeField] private UISprite completeSpr = null;

    private int _index;
    private AchieveEventData _achieveEventData;

    public void UpdateSlot(int index, AchieveEventData achieveEventData)
    {
        if (achieveEventData == null)
        {
            return;
        }

        _index = index;
        _achieveEventData = achieveEventData;

        GameTable.AchieveEventData.Param achieveEventTableParam = achieveEventData.TableData;
        completeSpr.SetActive(achieveEventTableParam == null);
        if (completeSpr.gameObject.activeSelf)
        {
            achieveEventTableParam = GameInfo.Instance.GameTable.FindAchieveEventData(x => x.GroupID == achieveEventData.GroupId && x.AchieveGroup == achieveEventData.AchieveGroupId && x.GroupOrder == achieveEventData.GroupOrder - 1);
        }

        itemListSlot.UpdateSlot(UIItemListSlot.ePosType.RewardDataInfo, 0, 
            new RewardData(achieveEventTableParam.RewardType, achieveEventTableParam.RewardIndex, achieveEventTableParam.RewardValue));

        titleLabel.textlocalize = FLocalizeString.Instance.GetText(achieveEventTableParam.Name, achieveEventTableParam.GroupOrder);
        infoLabel.textlocalize = FLocalizeString.Instance.GetText(achieveEventTableParam.Desc, achieveEventTableParam.AchieveValue);

        int nowValue = achieveEventData.Value;
        int maxValue = achieveEventTableParam.AchieveValue;
        GameSupport.GetAchieveEventNowMaxValue(achieveEventData, ref nowValue, ref maxValue);

        string gaugeText = FLocalizeString.Instance.GetText(278, nowValue, maxValue);
        float fillAmount = (float)nowValue / maxValue;
        gaugeUnit.InitGaugeUnit(fillAmount);
        gaugeUnit.SetText(gaugeText);

        getBtn.SetActive(!completeSpr.gameObject.activeSelf && maxValue <= nowValue);
    }

    public void OnClick_Slot()
    {
        if (_achieveEventData == null)
        {
            return;
        }

        if (GameSupport.IsAchieveEventComplete(_achieveEventData) != eAchieveEventType.Reward)
        {
            return;
        }

        UIAchieveEvent achieveEvent = ParentGO.GetComponent<UIAchieveEvent>();
        if (achieveEvent != null)
        {
            achieveEvent.GetAchieveReward(_index, _achieveEventData);
        }
    }
}
