using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAchieveEvent : FComponent
{
    [Header("UIAchieveEvent")]
    [SerializeField] private FList rewardList = null;
    [SerializeField] private UIButton allReceiveBtn = null;
    [SerializeField] private GameObject disAllReceiveObj = null;

    [Header("Title")]
    [SerializeField] private GameObject titleObj = null;
    [SerializeField] private UITexture titleTex = null;
    [SerializeField] private UILabel titleLabel = null;
    [SerializeField] private UILabel dateLabel = null;

    private bool _isRewardListZeroFocus;
    private EventData _eventData;
    private GameTable.AchieveEvent.Param _achieveEvent;
    private List<AchieveEventData> _achieveEventList = new List<AchieveEventData>();
    private List<RewardData> _rewardList = new List<RewardData>();

    public override void Awake()
    {
        base.Awake();

        rewardList.EventUpdate = RewardUpdate;
        rewardList.EventGetItemCount = RewardCount;
        rewardList.UpdateList();
    }

    public override void OnEnable()
    {
        if (_eventData == null)
        {
            return;
        }

        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
        base.InitComponent();

        DataSetting();
        AchieveEventDataSetting();
    }

    public override void Renewal(bool bChildren = false)
    {
        base.Renewal(bChildren);

        if (_achieveEvent != null)
        {
            titleLabel.textlocalize = FLocalizeString.Instance.GetText(_achieveEvent.Name);
            dateLabel.textlocalize = FLocalizeString.Instance.GetText(303, GameSupport.GetTimeWithString(_achieveEvent.EndTime, true).ToString("yyyy.MM.dd. HH:mm"));
        }

        rewardList.RefreshNotMoveAllItem();
        if (_isRewardListZeroFocus)
        {
            _isRewardListZeroFocus = false;
            rewardList.SpringSetFocus(0);
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();

        StopAllCoroutines();
        titleObj.SetActive(false);
    }

    public void SetData(int groupId)
    {
        _eventData = GameInfo.Instance.GetEventData((int)eLobbyEventType.Achieve, groupId);
    }

    public void OnClick_AllAchieveReward()
    {
        _rewardList.Clear();

        List<int> achieveCompleteList = new List<int>();
        foreach (AchieveEventData achieveEventData in _achieveEventList)
        {
            if (achieveEventData.TableData == null)
            {
                continue;
            }

            if (GameSupport.IsAchieveEventComplete(achieveEventData) != eAchieveEventType.Reward)
            {
                continue;
            }

            achieveCompleteList.Add(achieveEventData.AchieveGroupId);
            _rewardList.Add(new RewardData(achieveEventData.TableData.RewardType, achieveEventData.TableData.RewardIndex, achieveEventData.TableData.RewardValue));
        }

        if (achieveCompleteList.Count <= 0)
        {
            return;
        }

        _isRewardListZeroFocus = true;

        GameInfo.Instance.Send_ReqRewardTakeAchieveEvent(achieveCompleteList, OnNet_AchieveReward);
    }

    public void GetAchieveReward(int index, AchieveEventData achieveEventData)
    {
        _rewardList.Clear();
        _rewardList.Add(new RewardData(achieveEventData.TableData.RewardType, achieveEventData.TableData.RewardIndex, achieveEventData.TableData.RewardValue));

        _isRewardListZeroFocus = false;

        GameInfo.Instance.Send_ReqRewardTakeAchieveEvent(new List<int>() { achieveEventData.AchieveGroupId }, OnNet_AchieveReward);
    }

    private void OnNet_AchieveReward(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        GameInfo.Instance.RewardList.Clear();
        GameInfo.Instance.RewardList.AddRange(_rewardList);

        MessageRewardListPopup.RewardListMessage(FLocalizeString.Instance.GetText(1142), FLocalizeString.Instance.GetText(1336), GameInfo.Instance.RewardList);

        AchieveEventDataSetting();
        Renewal();
    }

    private void DataSetting()
    {
        _isRewardListZeroFocus = true;
        _achieveEvent = GameInfo.Instance.GameTable.FindAchieveEvent(_eventData.GroupID);
        if (_achieveEvent == null)
        {
            return;
        }

        titleObj.SetActive(true);
        titleTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("ui", $"UI/UITexture/Event/{_achieveEvent.Image}.png");
    }

    private void AchieveEventDataSetting()
    {
        _achieveEventList.Clear();
        bool isAllReceiveBtn = false;
        eAchieveEventType achieveEventType = eAchieveEventType.Ing;
        List<AchieveEventData> achieveEventList = GameInfo.Instance.AchieveEventList.FindAll(x => x.GroupId == _achieveEvent.ID);
        achieveEventList.Reverse();
        List<AchieveEventData> achieveEventCompleteList = new List<AchieveEventData>();

        foreach (AchieveEventData achieveEvent in achieveEventList)
        {
            achieveEventType = GameSupport.IsAchieveEventComplete(achieveEvent);
            if (!isAllReceiveBtn)
            {
                isAllReceiveBtn = achieveEventType == eAchieveEventType.Reward;
            }

            if (achieveEventType == eAchieveEventType.Complete)
            {
                achieveEventCompleteList.Add(achieveEvent);
            }
            else if (achieveEventType == eAchieveEventType.Ing)
            {
                _achieveEventList.Add(achieveEvent);
            }
            else
            {
                _achieveEventList.Insert(0, achieveEvent);
            }
        }

        _achieveEventList.AddRange(achieveEventCompleteList);

        allReceiveBtn.SetActive(isAllReceiveBtn);
        disAllReceiveObj.SetActive(!isAllReceiveBtn);
    }

    private void RewardUpdate(int index, GameObject obj)
    {
        UIAchievementSlot achievementListSlot = obj.GetComponent<UIAchievementSlot>();
        if (achievementListSlot == null)
        {
            return;
        }

        if (achievementListSlot.ParentGO == null)
        {
            achievementListSlot.ParentGO = this.gameObject;
        }

        AchieveEventData achieveEventData = null;
        if (0 <= index && index < _achieveEventList.Count)
        {
            achieveEventData = _achieveEventList[index];
        }

        achievementListSlot.UpdateSlot(index, achieveEventData);
    }

    private int RewardCount()
    {
        return _achieveEventList.Count;
    }
}
