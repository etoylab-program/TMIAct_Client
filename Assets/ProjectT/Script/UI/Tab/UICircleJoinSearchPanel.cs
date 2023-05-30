using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class UICircleJoinSearchPanel : FComponent
{
    [Header("UICircleJoinSearchPanel")]
    [SerializeField] private FList circleFList = null;
    [SerializeField] private UIInput searchNameInput = null;

    private int _slotFocus;

    public override void Awake()
    {
        base.Awake();

        circleFList.EventUpdate = OnEventRecommendUpdate;
        circleFList.EventGetItemCount = OnEventRecommendCount;
        circleFList.InitBottomFixing();
        circleFList.UpdateList();

        searchNameInput.defaultText = "서클ID, 서클 명으로 검색"; // Test - LeeSeungJin - Change String
    }

    public override void InitComponent()
    {
        base.InitComponent();

        _slotFocus = 0;
        searchNameInput.value = string.Empty;

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

        _slotFocus = 0;
    }

    private void OnEventRecommendUpdate(int index, GameObject obj)
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
        if (0 <= index && index < GameInfo.Instance.CircleRecommendList.Count)
        {
            circleData = GameInfo.Instance.CircleRecommendList[index];
        }

        slot.UpdateSlot(index, circleData, eCircleInfoSlotType.Recommend);
    }

    private int OnEventRecommendCount()
    {
        return GameInfo.Instance.CircleRecommendList.Count;
    }

    public void SetFocus(int index)
    {
        _slotFocus = index;
    }

    public void OnClick_SearchBtn()
    {
        if (string.IsNullOrEmpty(searchNameInput.value))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(200050));
            return;
        }

        long ccuid = 0;
        string searchName = string.Empty;
        if (Regex.IsMatch(searchNameInput.value, @"[0-9]"))
        {
            long.TryParse(searchNameInput.value, out ccuid);
        }
        else
        {
            searchName = searchNameInput.value;
        }

        GameInfo.Instance.Send_ReqCircleSearch(ccuid, searchName, OnNet_CircleSearch);
    }

    private void OnNet_CircleSearch(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        circleFList.Reset();

        Renewal();
    }
}
