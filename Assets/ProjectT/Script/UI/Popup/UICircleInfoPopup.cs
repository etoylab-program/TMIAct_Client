using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICircleInfoPopup : FComponent
{
    [Header("UICircleInfoPopup")]
    [SerializeField] private UITexture circleFlagTex = null;
    [SerializeField] private UITexture circleMarkTex = null;
    [SerializeField] private UILabel circleUidLabel = null;
    [SerializeField] private UILabel circleRankLabel = null;
    [SerializeField] private UILabel circleNameLabel = null;
    [SerializeField] private UILabel circleMemberCountLabel = null;
    [SerializeField] private UILabel circleContentLabel = null;
    [SerializeField] private UILabel circleMainLangLabel = null;
    [SerializeField] private FToggle circleOtherLangFToggle = null;
    [SerializeField] private UITexture leaderMarkTex = null;
    [SerializeField] private UILabel leaderUidLabel = null;
    [SerializeField] private UILabel leaderRankLabel = null;
    [SerializeField] private UILabel leaderNameLabel = null;

    private int _slotIndex;
    private CircleData _circleData;

    public void SetData(int slotIndex, CircleData circleData)
    {
        _slotIndex = slotIndex;
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

        circleContentLabel.textlocalize = _circleData.Content;
        circleMainLangLabel.textlocalize = _circleData.GetStringMainLang();
        circleOtherLangFToggle.SetToggle(_circleData.GetIntOtherLang(), SelectEvent.Code);

        LobbyUIManager.Instance.GetUserMarkIcon(this.gameObject, this.gameObject, _circleData.Leader.MarkId, ref leaderMarkTex);

        leaderUidLabel.textlocalize = _circleData.Leader.Uid.ToString();
        leaderRankLabel.textlocalize = _circleData.Leader.GetStringRank();
        leaderNameLabel.textlocalize = _circleData.Leader.Name;
    }

    public void OnClick_JoinBtn()
    {
        if (GameInfo.Instance.CircleJoinList.Capacity <= GameInfo.Instance.CircleJoinList.Count)
        {
            MessageToastPopup.Show("가입 신청할 수 있는 최대 한도입니다."); // Test - LeeSeungJin - Change String
            return;
        }

        GameInfo.Instance.Send_ReqCircleJoin(_circleData.Uid, _circleData.MainLanguage, OnNet_CircleJoin);
    }

    private void OnNet_CircleJoin(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        UICircleJoinPanel circleJoinPanel = LobbyUIManager.Instance.GetUI<UICircleJoinPanel>("CircleJoinPanel");
        if (circleJoinPanel != null)
        {
            UICircleJoinSearchPanel circleJoinSearchPanel = circleJoinPanel.GetComponentInChildren<UICircleJoinSearchPanel>();
            if (circleJoinSearchPanel != null)
            {
                circleJoinSearchPanel.SetFocus(_slotIndex);
                circleJoinSearchPanel.Renewal();
            }
        }

        OnClickClose();
    }
}
