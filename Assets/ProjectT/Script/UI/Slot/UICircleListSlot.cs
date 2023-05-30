using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICircleListSlot : FSlot
{
    [Header("UICircleInfoListSlot")]
    [SerializeField] private UITexture circleFlagTex = null;
    [SerializeField] private UITexture circleMarkTex = null;
    [SerializeField] private UILabel circleUidLabel = null;
    [SerializeField] private UILabel circleRankLabel = null;
    [SerializeField] private UILabel circleNameLabel = null;
    [SerializeField] private UILabel circleMemberCountLabel = null;
    [SerializeField] private UILabel leaderNameLabel = null;
    [SerializeField] private UIButton circleInfoBtn = null;
    [SerializeField] private UIButton joinCancelBtn = null;

    private int _index;
    private CircleData _circleData;

    public void UpdateSlot(int index, CircleData circleData, eCircleInfoSlotType circleInfoSlotType)
    {
        _index = index;
        _circleData = circleData;

        if (_circleData == null)
        {
            return;
        }

        circleFlagTex.mainTexture = LobbyUIManager.Instance.GetCircleMarkTexture(_circleData.FlagId);
        circleMarkTex.mainTexture = LobbyUIManager.Instance.GetCircleMarkTexture(_circleData.MarkId);
        circleFlagTex.color = LobbyUIManager.Instance.GetCircleMarkColor(_circleData.ColorId);

        circleUidLabel.textlocalize = _circleData.GetStringUid();
        circleRankLabel.textlocalize = _circleData.GetStringRank();
        circleNameLabel.textlocalize = _circleData.Name;
        circleMemberCountLabel.textlocalize = _circleData.GetStringMemberCount();

        leaderNameLabel.textlocalize = $"∫Œ¿Â : {_circleData.Leader.Name}"; // Test - LeeSeungJin - Change String

        circleInfoBtn.SetActive(circleInfoSlotType == eCircleInfoSlotType.Recommend);
        joinCancelBtn.SetActive(circleInfoSlotType == eCircleInfoSlotType.Join);
    }

    public void OnClick_CircleInfoBtn()
    {
        UICircleInfoPopup circleInfoPopup = LobbyUIManager.Instance.GetUI<UICircleInfoPopup>("CircleInfoPopup");
        if (circleInfoPopup == null)
        {
            return;
        }

        circleInfoPopup.SetData(_index, _circleData);
        circleInfoPopup.SetUIActive(true);
    }

    public void OnClick_JoinCancelBtn()
    {
        GameInfo.Instance.Send_ReqCircleJoinCancel(_circleData.Uid, OnNet_CircleJoinCancel);
    }

    private void OnNet_CircleJoinCancel(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        if (ParentGO == null)
        {
            return;
        }

        UICircleJoinWaitListPanel circleJoinWaitListPanel = ParentGO.GetComponent<UICircleJoinWaitListPanel>();
        if (circleJoinWaitListPanel != null)
        {
            circleJoinWaitListPanel.SetFocus(_index);
            circleJoinWaitListPanel.Renewal();
        }
    }
}
