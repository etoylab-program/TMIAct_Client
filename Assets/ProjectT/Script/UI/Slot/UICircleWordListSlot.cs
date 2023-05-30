using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICircleWordListSlot : FSlot
{
    [Header("UICircleWordListSlot")]
    [SerializeField] private UILabel wordLabel = null;
    [SerializeField] private UISprite selSpr = null;

    private int _index;
    private GameClientTable.ChatWords.Param _chatWordsParam;

    public void UpdateSlot(int index, GameClientTable.ChatWords.Param chatWordsParam, int selectId)
    {
        _index = index;
        _chatWordsParam = chatWordsParam;

        if (_chatWordsParam == null)
        {
            return;
        }

        selSpr.SetActive(_chatWordsParam.ID == selectId);
        wordLabel.textlocalize = FLocalizeString.Instance.GetText(_chatWordsParam.Words);
    }

    public void OnClick_Slot()
    {
        if (ParentGO == null)
        {
            return;
        }

        UICircleLobbyMainPanel circleLobbyMainPanel = ParentGO.GetComponent<UICircleLobbyMainPanel>();
        if (circleLobbyMainPanel != null)
        {
            circleLobbyMainPanel.SelectSlotTableId(_chatWordsParam.ID);
        }
    }
}
