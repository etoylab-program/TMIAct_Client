using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICircleMemberListSlot : FSlot
{
    [Header("UICircleMemberListSlot")]
    [Header("Member")]
    [SerializeField] private UISprite presidentSpr = null;
    [SerializeField] private UISprite vicePresidentSpr = null;
    [SerializeField] private UITexture markTex = null;
    [SerializeField] private UILabel rankLabel = null;
    [SerializeField] private UILabel nameLabel = null;
    [SerializeField] private UILabel uidLabel = null;
    [SerializeField] private UIButton managementBtn = null;
    [SerializeField] private GameObject joinWaitObj = null;

    private int _index;
    private FriendUserData _circleUserData;
    
    public void UpdateSlot(int index, FriendUserData circleUserData, eCircleUserSlotType circleUserSlotType)
    {
        _index = index;
        _circleUserData = circleUserData;

        if (_circleUserData == null)
        {
            return;
        }

        presidentSpr.SetActive(circleUserSlotType != eCircleUserSlotType.JoinWait && _circleUserData.CircleAuthLevel.IsEqual(eCircleAuthLevel.MASTER));
        vicePresidentSpr.SetActive(circleUserSlotType != eCircleUserSlotType.JoinWait && _circleUserData.CircleAuthLevel.IsEqual(eCircleAuthLevel.DEPUTY));

        LobbyUIManager.Instance.GetUserMarkIcon(this.gameObject, this.gameObject, _circleUserData.UserMark, ref markTex);
        rankLabel.textlocalize = _circleUserData.GetStringRank();
        nameLabel.textlocalize = _circleUserData.GetNickName();

        uidLabel.textlocalize = Utility.AppendString(_circleUserData.GetStringUid(), _circleUserData.GetStringLastLogin());

        managementBtn.SetActive(circleUserSlotType == eCircleUserSlotType.Member && GameInfo.Instance.UserData.CircleAuthLevel.IsExcess(_circleUserData.CircleAuthLevel.AuthLevel));
        joinWaitObj.SetActive(circleUserSlotType == eCircleUserSlotType.JoinWait);
    }

    public void OnClick_ManagementBtn()
    {
        if (GameInfo.Instance.UserData.CircleAuthLevel.IsLessThan(_circleUserData.CircleAuthLevel.AuthLevel))
        {
            MessageToastPopup.Show("권한이 부족합니다."); // Test - LeeSeungJin - Change String
            return;
        }

        if (_circleUserData.CircleAuthLevel.IsEqual(eCircleAuthLevel.MASTER))
        {
            return;
        }

        UICircleMessagePopup circleMessagePopup = LobbyUIManager.Instance.GetUI<UICircleMessagePopup>("CircleMessagePopup");
        if (circleMessagePopup == null)
        {
            return;
        }

        if (_circleUserData.CircleAuthLevel.IsEqual(eCircleAuthLevel.MEMBER))
        {
            circleMessagePopup.SetData(eCircleSequenceType.MemberManagement, circleUserData: _circleUserData);
        }

        if (_circleUserData.CircleAuthLevel.IsEqual(eCircleAuthLevel.DEPUTY))
        {
            circleMessagePopup.SetData(eCircleSequenceType.ViceManagement, circleUserData: _circleUserData);
        }

        circleMessagePopup.SetUIActive(true);
    }

    public void OnClick_AcceptedBtn()
    {
        if (GameInfo.Instance.UserData.CircleAuthLevel.IsNotEqual(eCircleAuthLevel.MASTER))
        {
            MessageToastPopup.Show("권한이 부족합니다."); // Test - LeeSeungJin - Change String
            return;
        }

        GameInfo.Instance.Send_ReqCircleChangeStateJoinWaitUser(_circleUserData.UUID, true, OnNet_CircleChangeStateJoinWaitUser);
    }

    public void OnClick_RefusalBtn()
    {
        if (GameInfo.Instance.UserData.CircleAuthLevel.IsNotEqual(eCircleAuthLevel.MASTER))
        {
            MessageToastPopup.Show("권한이 부족합니다."); // Test - LeeSeungJin - Change String
            return;
        }

        GameInfo.Instance.Send_ReqCircleChangeStateJoinWaitUser(_circleUserData.UUID, false, OnNet_CircleChangeStateJoinWaitUser);
    }

    private void OnNet_CircleChangeStateJoinWaitUser(int result, PktMsgType pktmsg)
    {
        LobbyUIManager.Instance.Renewal("CircleLobbyPanel");
    }
}
