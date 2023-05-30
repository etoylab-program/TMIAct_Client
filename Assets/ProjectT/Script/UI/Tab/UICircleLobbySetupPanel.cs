using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICircleLobbySetupPanel : FComponent
{
    [Header("UICircleLobbySetupPanel")]
    [SerializeField] private FTab mainFTab = null;
    [SerializeField] private List<FComponent> panelList = null;

    private int _selectTabIndex = 0;

    public override void Awake()
    {
        base.Awake();

        mainFTab.EventCallBack = OnEventMainTabSelect;
    }

    public override void InitComponent()
    {
        base.InitComponent();

        mainFTab.SetTab(0, SelectEvent.Code);
    }

    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    private bool OnEventMainTabSelect(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Enable)
        {
            return false;
        }

        if (type == SelectEvent.Code)
        {
            for (int i = 0; i < panelList.Count; i++)
            {
                panelList[i].SetUIActive(i == nSelect);
            }

            return true;
        }

        _selectTabIndex = nSelect;

        switch ((eCircleInfoTabType)nSelect)
        {
            case eCircleInfoTabType.Info:
                {
                    OnNet_GetCircleUserList(0, null);
                }
                break;
            case eCircleInfoTabType.Member:
                {
                    GameInfo.Instance.Send_ReqGetCircleUserList(OnNet_GetCircleUserList);
                }
                break;
            case eCircleInfoTabType.Facility:
                {
                    OnNet_GetCircleUserList(0, null);
                }
                break;
            case eCircleInfoTabType.Buff:
                {
                    OnNet_GetCircleUserList(0, null);
                }
                break;
        }

        return false;
    }

    private void OnNet_GetCircleUserList(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        mainFTab.SetTab(_selectTabIndex, SelectEvent.Code);
    }
}
