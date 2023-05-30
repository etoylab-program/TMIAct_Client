using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class UICircleLobbyPanel : FComponent
{
    [Header("UICircleLobbyPanel")]
    [SerializeField] private List<FComponent> panelList = null;

    private UITopPanel _topPanel;
    private eCircleLobbyPanelType _currentPanelType;


    public override void OnEnable()
    {
		_currentPanelType = eCircleLobbyPanelType.Main;
		base.OnEnable();
    }

    public override void Renewal(bool bChildren = false)
    {
        base.Renewal(bChildren);

        int activeIndex = (int)_currentPanelType;
        for (int i = 0; i < panelList.Count; i++)
        {
            bool isActive = (i == activeIndex);
            if (isActive)
            {
                if (_topPanel == null)
                {
                    _topPanel = LobbyUIManager.Instance.GetUI<UITopPanel>("TopPanel");
                }
                
                if (_topPanel != null)
                {
                    if (panelList[i].kPopupGoodsType == ePOPUPGOODSTYPE.CIRCLE_GOLD)
                    {
                        _topPanel.SetTopStatePlay(UITopPanel.eTOPSTATE.CIRCLE_GOLD);
                    }
                    else if (panelList[i].kPopupGoodsType == ePOPUPGOODSTYPE.CIRCLE_POINT)
                    {
                        _topPanel.SetTopStatePlay(UITopPanel.eTOPSTATE.CIRCLE_POINT);
                    }
                    else
                    {
                        _topPanel.SetTopStatePlay(UITopPanel.eTOPSTATE.NORMAL);
                    }
                }
            }
            panelList[i].SetUIActive(isActive);
        }
    }

    public void BackAction()
    {
        switch(_currentPanelType)
        {
            case eCircleLobbyPanelType.Main:
                {
                    LobbyUIManager.Instance.SetPanelType(ePANELTYPE.MAIN);
                }
                break;
            case eCircleLobbyPanelType.MarkChange:
                {
                    _currentPanelType = eCircleLobbyPanelType.Setup;
                    Renewal();
                }
                break;
            default:
                {
                    _currentPanelType = eCircleLobbyPanelType.Main;
                    Renewal();
                }
                break;
        }
    }

    public void SetActivePanel(eCircleLobbyPanelType changePanel)
    {
        _currentPanelType = changePanel;

        Renewal();
    }
}
