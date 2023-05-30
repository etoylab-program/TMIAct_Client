using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICircleStampListSlot : FSlot
{
    [Header("UICircleStampListSlot")]
    [SerializeField] private UITexture iconTex = null;
    [SerializeField] private UISprite selSpr = null;
    [SerializeField] private GameObject lockObj = null;

    private int _index;
    private GameTable.ChatStamp.Param _chatStampParam;

    public void UpdateSlot(int index, GameTable.ChatStamp.Param chatStampParam, int selectId)
    {
        _index = index;
        _chatStampParam = chatStampParam;

        if (_chatStampParam == null)
        {
            return;
        }

        iconTex.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("icon", Utility.AppendString("Icon/Circle/", chatStampParam.Icon, ".png")) as Texture;
        selSpr.SetActive(_chatStampParam.ID == selectId);
        lockObj.SetActive(GameInfo.Instance.IsCircleChatStamp(_index) == false);
    }

    public void OnClick_Slot()
    {
        if (ParentGO == null)
        {
            return;
        }

        if (lockObj.activeSelf)
        {
            UICircleMessagePopup circleMessagePopup = LobbyUIManager.Instance.GetUI<UICircleMessagePopup>();
            if (circleMessagePopup != null)
            {
                circleMessagePopup.SetData(eCircleSequenceType.BuyStamp, FLocalizeString.Instance.GetText(51), stampTableId: _chatStampParam.ID);
                circleMessagePopup.SetUIActive(true);
            }
            return;
        }

        UICircleLobbyMainPanel circleLobbyMainPanel = ParentGO.GetComponent<UICircleLobbyMainPanel>();
        if (circleLobbyMainPanel != null)
        {
            circleLobbyMainPanel.SelectSlotTableId(_chatStampParam.ID);
        }
    }
}
