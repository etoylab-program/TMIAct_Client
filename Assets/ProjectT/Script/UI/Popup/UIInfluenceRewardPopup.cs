using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInfluenceRewardPopup : FComponent
{   
    [SerializeField] private FList kRewardList;

    private List<GameTable.InfluenceRank.Param> RankList
    {
        get { return GameInfo.Instance.GameTable.InfluenceRanks; }
    }
    
    private bool IsReceivePeriod = false;
    public override void Awake()
    {
        base.Awake();

        if (kRewardList)
        {
            kRewardList.EventUpdate = UpdateListSlot;
            kRewardList.EventGetItemCount = () =>
            {
                if (RankList == null) return 0;
                return RankList.Count;                
            };
            kRewardList.InitBottomFixing();
        }
    }

    public override void InitComponent()
    {
        RankList.Sort((l, r) =>
        {
            if (l.RewardOrder > r.RewardOrder) return 1;
            else if (l.RewardOrder < r.RewardOrder) return -1;
            return 0;
        });

        int SelectGroupID = (int)GameInfo.Instance.InfluenceMissionData.GroupID;
        GameTable.InfluenceMissionSet.Param InfMissionSetTable = GameInfo.Instance.GameTable.FindInfluenceMissionSet(SelectGroupID);

        System.DateTime EndTime = GameSupport.GetTimeWithString(InfMissionSetTable.EndTime, true);
        System.DateTime RewardTime = GameSupport.GetTimeWithString(InfMissionSetTable.RewardTime, true);

        System.TimeSpan EndTimeSpan =  GameSupport.GetRemainTime(EndTime);
        System.TimeSpan RewardTimeSpan = GameSupport.GetRemainTime(RewardTime);

        IsReceivePeriod = EndTimeSpan.TotalSeconds <= 0 && RewardTimeSpan.TotalSeconds >= 0;
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);
        InitComponent();

        kRewardList.UpdateList();
    }

    private void UpdateListSlot(int index, GameObject slotObj)
    {
        UIInfluenceRewardListSlot slot = slotObj.GetComponent<UIInfluenceRewardListSlot>();
        if (slot == null) return;

        int startRank = 0;
        if (index != 0)
        {
            startRank = RankList[index - 1].RewardValue;
        }

        slot.UpdateData(RankList[index], startRank, IsReceivePeriod);

    }
}
