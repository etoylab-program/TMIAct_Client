using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInfluenceRankingPopup : FComponent
{   
    [SerializeField] private FList kRankingList;

    private List<InfluenceRankData.Piece> RankList
    {
        get { return GameInfo.Instance.InfluenceRankData.Infos; }
    }
    public override void Awake()
    {
        base.Awake();

        if (kRankingList)
        {
            kRankingList.EventUpdate = UpdateListSlot;
            kRankingList.EventGetItemCount = () =>
            {
                if (RankList == null) return 0;
                return RankList.Count;                
            };
            kRankingList.InitBottomFixing();
        }
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);
        InitComponent();

        kRankingList.UpdateList();
    }

    private void UpdateListSlot(int index, GameObject slotObj)
    {
        UIInfluenceRankingListSlot slot = slotObj.GetComponent<UIInfluenceRankingListSlot>();
        if (slot == null) return;

        slot.UpdateData(RankList[index]);
    }
}
