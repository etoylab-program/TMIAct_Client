using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class FriendConfirmPopup
{
    public static UIFriendConfirmPopup GetFriendConfirmPopup()
    {
        UIFriendConfirmPopup mpopup = null;
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
            mpopup = LobbyUIManager.Instance.GetUI<UIFriendConfirmPopup>("FriendConfirmPopup");

        return mpopup;
    }

    public static void ShowFriendConfirmPopup(FriendUserData friendUser)
    {
        UIFriendConfirmPopup popup = GetFriendConfirmPopup();
        if (popup == null)
            return;

        popup.InitFriendConfirmPopup(friendUser);
        LobbyUIManager.Instance.ShowUI("FriendConfirmPopup", true);
    }
}

public class UIFriendConfirmPopup : FComponent
{

	public UIButton kYesBtn;
	public UIButton kNoBtn;
    public UIFriendListSlot kFriendListSlot;

    private FriendUserData _friendUserData;

    
	public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		_friendUserData = null;
        base.OnDisable();
        
    }

    public void InitFriendConfirmPopup(FriendUserData friendUser)
    {
        _friendUserData = friendUser;
        kFriendListSlot.UpdateSlot(-1, _friendUserData, false, true);
        
    }
	
	public void OnClick_YesBtn()
	{
        if (_friendUserData == null)
            return;

        List<long> userUUIDList = new List<long>();
        userUUIDList.Add(_friendUserData.UUID);

        GameInfo.Instance.Send_ReqFriendKick(userUUIDList, OnAckFriendKick);
    }
	
	public void OnClick_NoBtn()
	{
        if (_friendUserData == null)
            return;

        OnClickClose();
	}

	
	public void OnClick_BGBtn()
	{
	}

    public void OnAckFriendKick(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        LobbyUIManager.Instance.Renewal("FriendPopup");
        OnClickClose();
    }
}
