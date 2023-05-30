using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIArenaRewardListPopup : FComponent
{
	[Header("UIArenaRewardListPopup")]
	[SerializeField] private FList rewardFList = null;

    private List<GameTable.CircleCheck.Param> _circleCheckParamList = new List<GameTable.CircleCheck.Param>();
    private eArenaRewardListPopupType _rewardType = eArenaRewardListPopupType.ArenaReward;
    private int _focusIndex;
    private bool _isPlayAnimation;

    public override void Awake()
    {
        base.Awake();

        rewardFList.EventUpdate = OnEventRewardListUpdate;
        rewardFList.EventGetItemCount = OnEventRewardListCount;
        rewardFList.InitBottomFixing();
        rewardFList.UpdateList();
    }

    public override void InitComponent()
    {
        base.InitComponent();
        
        _focusIndex = 0;

        switch (_rewardType)
        {
            case eArenaRewardListPopupType.ArenaReward:
                {
                    int rewardOrderID = -1;

                    //1~100위의 랭커인지 검사
                    if (GameInfo.Instance.UserBattleData.Now_Rank > 0)
                    {
                        List<GameTable.ArenaReward.Param> arenaRewardParamList = GameInfo.Instance.GameTable.FindAllArenaReward(x => x.RewardType == (int)eArenaRewardType.RANK);
                        foreach (GameTable.ArenaReward.Param arenaRewardParam in arenaRewardParamList)
                        {
                            if (GameInfo.Instance.UserBattleData.Now_Rank <= arenaRewardParam.RewardValue)
                            {
                                rewardOrderID = arenaRewardParam.RewardOrder;
                                break;
                            }
                        }
                    }

                    //랭커가 아니면 Grade로 검사
                    if (rewardOrderID == -1)
                    {
                        List<GameTable.ArenaReward.Param> arenaRewardParamList = GameInfo.Instance.GameTable.FindAllArenaReward(x => x.RewardType == (int)eArenaRewardType.GRADE);
                        foreach (GameTable.ArenaReward.Param arenaRewardParam in arenaRewardParamList)
                        {
                            if (GameInfo.Instance.UserBattleData.Now_GradeId == arenaRewardParam.RewardValue)
                            {
                                rewardOrderID = arenaRewardParam.RewardOrder;
                                break;
                            }
                        }
                    }

                    if (rewardOrderID != -1)
                    {
                        _focusIndex = GameInfo.Instance.GameTable.ArenaRewards.FindIndex(x => x.RewardOrder == rewardOrderID);
                    }
                }
                break;
            case eArenaRewardListPopupType.CircleAttendance:
                {
                    _circleCheckParamList = GameInfo.Instance.GameTable.FindAllCircleCheck(x => x.GroupID == GameInfo.Instance.UserData.CircleAttendance.RewardGroupId);
                }
                break;
        }

        rewardFList.RefreshNotMoveAllItem();
        rewardFList.SpringSetFocus(_focusIndex);
    }

    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        rewardFList.SpringSetFocus(0, isImmediate: true);
    }

    private void OnEventRewardListUpdate(int index, GameObject obj)
    {
        UIArenaGradeRewardSlot slot = obj.GetComponent<UIArenaGradeRewardSlot>();
        if (slot == null)
        {
            return;
        }

        if (slot.ParentGO == null)
        {
            slot.ParentGO = this.gameObject;
        }

        switch(_rewardType)
        {
            case eArenaRewardListPopupType.ArenaReward:
                {
                    int prevRewardValue = 0;
                    GameTable.ArenaReward.Param data = null;
                    if (0 <= index && index < GameInfo.Instance.GameTable.ArenaRewards.Count)
                    {
                        data = GameInfo.Instance.GameTable.ArenaRewards[index];
                        if (0 < index)
                        {
                            prevRewardValue = GameInfo.Instance.GameTable.ArenaRewards[index - 1].RewardValue;
                        }
                    }
                    slot.UpdateSlot(data, prevRewardValue, _focusIndex == index);
                }
                break;
            case eArenaRewardListPopupType.CircleAttendance:
                {
                    GameTable.CircleCheck.Param circleCheckParam = null;
                    if (0 <= index && index < _circleCheckParamList.Count)
                    {
                        circleCheckParam = _circleCheckParamList[index];
                    }
                    slot.UpdateSlot(circleCheckParam, _isPlayAnimation);
                }
                break;
        }
    }

    private int OnEventRewardListCount()
    {
        int count = 0;
        switch(_rewardType)
        {
            case eArenaRewardListPopupType.ArenaReward:
                {
                    count = GameInfo.Instance.GameTable.ArenaRewards.Count;
                }
                break;
            case eArenaRewardListPopupType.CircleAttendance:
                {
                    count = _circleCheckParamList.Count;
                }
                break;
        }
        return count;
    }

    public void SetRewardType(eArenaRewardListPopupType rewardType, bool isPlayAnimation = false)
    {
        _rewardType = rewardType;
        _isPlayAnimation = isPlayAnimation;
    }
}
