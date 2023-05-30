using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICircleJoinPanel : FComponent
{
    [Header("UICircleJoinPanel")]
    [SerializeField] private FTab mainFTab = null;
    [SerializeField] private List<GameObject> rootObjList = null;

    private enum eMainTabType
    {
        Recommend = 0,      // 서클 목록
        Join,               // 신청 현황
        Setup,              // 서클 개설
    }

    private eMainTabType _currentMainTab;

    public override void Awake()
    {
        base.Awake();

        mainFTab.EventCallBack = OnEventMainTabSelect;
    }

    public override void InitComponent()
    {
        base.InitComponent();

        mainFTab.SetTab((int)eMainTabType.Recommend, SelectEvent.Code);

        UITopPanel topPanel = LobbyUIManager.Instance.GetUI<UITopPanel>("TopPanel");
        if (topPanel != null)
        {
            topPanel.SetUIActive(false, false);
        }
    }

    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void OnClickClose()
    {
        base.OnClickClose();

        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.MAIN);
    }

    public override void OnDisable()
    {
        base.OnDisable();

        UITopPanel topPanel = LobbyUIManager.Instance.GetUI<UITopPanel>("TopPanel");
        if (topPanel != null)
        {
            topPanel.SetUIActive(true, false);
        }
    }

    private bool OnEventMainTabSelect(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Enable)
        {
            return false;
        }

        _currentMainTab = (eMainTabType)nSelect;

        for (int i = 0; i < rootObjList.Count; i++)
        {
            rootObjList[i].SetActive(i == (int)_currentMainTab);
        }

        return true;
    }
}
