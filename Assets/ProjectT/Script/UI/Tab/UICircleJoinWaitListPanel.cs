using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICircleJoinWaitListPanel : FComponent
{
    [Header("UICircleJoinWaitListPanel")]
    [SerializeField] private FList circleFList = null;
    [SerializeField] private UILabel countLabel = null;
    [SerializeField] private UILabel maxLabel = null;

    private int _slotFocus;

    public override void Awake()
    {
        base.Awake();

        circleFList.EventUpdate = OnEventJoinUpdate;
        circleFList.EventGetItemCount = OnEventJoinCount;
        circleFList.InitBottomFixing();
        circleFList.UpdateList();

        maxLabel.textlocalize = FLocalizeString.Instance.GetText(214, GameInfo.Instance.GameConfig.CircleWaitingJoinMaxNumber);
    }

    public override void InitComponent()
    {
        base.InitComponent();

        _slotFocus = 0;
        countLabel.textlocalize = GameInfo.Instance.CircleJoinList.Count.ToString();

        circleFList.Reset();
    }

    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void Renewal(bool bChildren = false)
    {
        base.Renewal(bChildren);

        circleFList.SpringSetFocus(_slotFocus, ratio: 0.5f);
        circleFList.RefreshNotMoveAllItem();
    }

    public override void OnClickClose()
    {
        base.OnClickClose();

        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.MAIN);
    }

    private void OnEventJoinUpdate(int index, GameObject obj)
    {
        UICircleListSlot slot = obj.GetComponent<UICircleListSlot>();
        if (slot == null)
        {
            return;
        }

        if (slot.ParentGO == null)
        {
            slot.ParentGO = this.gameObject;
        }

        CircleData circleData = null;
        if (0 <= index && index < GameInfo.Instance.CircleJoinList.Count)
        {
            circleData = GameInfo.Instance.CircleJoinList[index];
        }

        slot.UpdateSlot(index, circleData, eCircleInfoSlotType.Join);
    }

    private int OnEventJoinCount()
    {
        return GameInfo.Instance.CircleJoinList.Count;
    }

    public void SetFocus(int index)
    {
        _slotFocus = index;
    }
}
