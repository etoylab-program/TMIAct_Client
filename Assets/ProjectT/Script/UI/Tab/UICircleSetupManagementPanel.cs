using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICircleSetupManagementPanel : FComponent
{
    [Header("UICircleLobbyInfoPanel")]
    [SerializeField] private UITexture circleFlagTex = null;
    [SerializeField] private UITexture circleMarkTex = null;
    [SerializeField] private UITexture circleColorTex = null;
    [SerializeField] private UILabel circleRankLabel = null;
    [SerializeField] private UILabel circleNameLabel = null;
    [SerializeField] private UILabel circleUidLabel = null;
    [SerializeField] private UILabel circleMemberCountLabel = null;
    [SerializeField] private UILabel contentLabel = null;
    [SerializeField] private UILabel mainLangLabel = null;
    [SerializeField] private FToggle otherLangFToggle = null;
    [SerializeField] private UITexture leaderMarkTex = null;
    [SerializeField] private UILabel leaderRankLabel = null;
    [SerializeField] private UILabel leaderNameLabel = null;
    [SerializeField] private UILabel leaderUidLabel = null;

    public override void InitComponent()
    {
        base.InitComponent();
    }

    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void Renewal(bool bChildren = false)
    {
        base.Renewal(bChildren);

        CircleData circleData = GameInfo.Instance.CircleData;

        circleFlagTex.mainTexture = LobbyUIManager.Instance.GetCircleMarkTexture(circleData.FlagId);
        circleMarkTex.mainTexture = LobbyUIManager.Instance.GetCircleMarkTexture(circleData.MarkId);
        circleColorTex.mainTexture = LobbyUIManager.Instance.GetCircleMarkTexture(circleData.ColorId, true);
        circleColorTex.color = LobbyUIManager.Instance.GetCircleMarkColor(circleData.ColorId);

        circleUidLabel.textlocalize = circleData.GetStringUid();
        circleRankLabel.textlocalize = circleData.GetStringRank();
        circleNameLabel.textlocalize = circleData.Name;
        circleMemberCountLabel.textlocalize = circleData.GetStringMemberCount();

        contentLabel.textlocalize = circleData.Content;
        mainLangLabel.textlocalize = circleData.GetStringMainLang();
        otherLangFToggle.SetToggle(circleData.GetIntOtherLang(), SelectEvent.Code);

        LobbyUIManager.Instance.GetUserMarkIcon(this.gameObject, this.gameObject, circleData.Leader.MarkId, ref leaderMarkTex);
        leaderRankLabel.textlocalize = circleData.Leader.GetStringRank();
        leaderNameLabel.textlocalize = circleData.Leader.GetNickName();
        leaderUidLabel.textlocalize = circleData.Leader.GetStringUid();
    }

    public void OnClick_MarkChangeBtn()
    {
        if (GameInfo.Instance.UserData.CircleAuthLevel.IsLessThan(eCircleAuthLevel.MEMBER))
        {
            MessageToastPopup.Show("권한이 부족합니다."); // Test - LeeSeungJin - Change String
            return;
        }

        GameInfo.Instance.Send_ReqCircleGetMarkList(OnNet_CircleGetMarkList);
    }

    public void OnClick_InfoChangeBtn()
    {
        ShowCircleMessagePopup(eCircleSequenceType.Info);
    }

    public void OnClick_ContentChangeBtn()
    {
        ShowCircleMessagePopup(eCircleSequenceType.Content);
    }

    public void OnClick_MainLangChangeBtn()
    {
        ShowCircleMessagePopup(eCircleSequenceType.MainLang);
    }

    public void OnClick_OtherLangChangeBtn()
    {
        ShowCircleMessagePopup(eCircleSequenceType.OtherLang);
    }

    public void OnClick_DissolutionBtn()
    {
        ShowCircleMessagePopup(eCircleSequenceType.Dissolution);
    }

    public void OnClick_WithdrawalBtn()
    {
        ShowCircleMessagePopup(eCircleSequenceType.Withdrawal, isAuthCheck: false);
    }

    private void OnNet_CircleGetMarkList(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        UICircleLobbyPanel circleLobbyPanel = LobbyUIManager.Instance.GetUI<UICircleLobbyPanel>("CircleLobbyPanel");
        if (circleLobbyPanel == null)
        {
            return;
        }
        circleLobbyPanel.SetActivePanel(eCircleLobbyPanelType.MarkChange);
    }

    private void ShowCircleMessagePopup(eCircleSequenceType setType, bool isAuthCheck = true)
    {
        if (isAuthCheck && GameInfo.Instance.UserData.CircleAuthLevel.IsNotEqual(eCircleAuthLevel.MASTER))
        {
            MessageToastPopup.Show("권한이 부족합니다."); // Test - LeeSeungJin - Change String
            return;
        }

        UICircleMessagePopup circleMessagePopup = LobbyUIManager.Instance.GetUI<UICircleMessagePopup>("CircleMessagePopup");
        if (circleMessagePopup == null)
        {
            return;
        }
        circleMessagePopup.SetData(setType);
        circleMessagePopup.SetUIActive(true);
    }
}
