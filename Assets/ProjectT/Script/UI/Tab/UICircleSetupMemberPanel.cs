using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICircleSetupMemberPanel : FComponent
{
    [Header("UICircleLobbyMemberPanel")]
    [SerializeField] private FList memberFList = null;

    private eCircleUserSlotType _currentMainTabType;

    public override void Awake()
    {
        base.Awake();

        memberFList.EventUpdate = OnEventMemberUpdate;
        memberFList.EventGetItemCount = OnEventMemberCount;
        memberFList.InitBottomFixing();
        memberFList.UpdateList();
    }

    public override void InitComponent()
    {
        base.InitComponent();

        SetChangeSlotType(eCircleUserSlotType.Member);
    }

    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void Renewal(bool bChildren = false)
    {
        base.Renewal(bChildren);

        memberFList.RefreshNotMoveAllItem();
        if (memberFList.IsLastPosY())
        {
            memberFList.SpringSetFocus(memberFList.EventGetItemCount() - 1, ratio: 1);
        }
    }

    private void OnEventMemberUpdate(int index, GameObject obj)
    {
        UICircleMemberListSlot slot = obj.GetComponent<UICircleMemberListSlot>();
        if (slot == null)
        {
            return;
        }

        if (slot.ParentGO == null)
        {
            slot.ParentGO = this.gameObject;
        }

        FriendUserData circleUserData = null;
        eCircleUserSlotType circleUserSlotType;
        if (_currentMainTabType == eCircleUserSlotType.Member)
        {
            circleUserSlotType = eCircleUserSlotType.Member;
            if (0 <= index && index < GameInfo.Instance.CircleData.MemberList.Count)
            {
                circleUserData = GameInfo.Instance.CircleData.MemberList[index];
            }
        }
        else
        {
            circleUserSlotType = eCircleUserSlotType.JoinWait;
            if (0 <= index && index < GameInfo.Instance.CircleData.JoinWaitList.Count)
            {
                circleUserData = GameInfo.Instance.CircleData.JoinWaitList[index];
            }
        }

        slot.UpdateSlot(index, circleUserData, circleUserSlotType);
    }

    private int OnEventMemberCount()
    {
        int count = GameInfo.Instance.CircleData.JoinWaitList.Count;
        if (_currentMainTabType == eCircleUserSlotType.Member)
        {
            count = GameInfo.Instance.CircleData.MemberList.Count;
        }
        return count;
    }

    private void SetChangeSlotType(eCircleUserSlotType slotType)
    {
        _currentMainTabType = slotType;

        memberFList.Reset();
        memberFList.RefreshNotMoveAllItem();
    }

    public void OnClick_MemberBtn()
    {
        SetChangeSlotType(eCircleUserSlotType.Member);
    }

    public void OnClick_WaitListBtn()
    {
        SetChangeSlotType(eCircleUserSlotType.JoinWait);
    }
}
