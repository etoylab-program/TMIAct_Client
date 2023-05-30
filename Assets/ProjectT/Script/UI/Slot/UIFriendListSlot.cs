using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIFriendListSlot : FSlot {
 

	public UISprite kbgSpr;
	public UITexture kUserTex;
	public UILabel kRankLabel;
	public UILabel kNameLabel;
	public UILabel kIDLabel;
	public UIButton kAddBtn;
    public UISprite kAddBtnSpr;


    private FriendUserData _friendUserData;
    private bool _friendAddFlag = false;
    private int _index;

    public void UpdateSlot(int index, FriendUserData data, bool addFlag = false, bool delFlag = false)
	{
        if (data == null)
        {
            return;
        }

        if (0 <= index)
        {
            _index = index;
        }
        
        _friendUserData = data;
        _friendAddFlag = addFlag;

        LobbyUIManager.Instance.GetUserMarkIcon(ParentGO, this.gameObject, _friendUserData.UserMark, ref kUserTex);

        kRankLabel.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.RANK_TXT_NOW_LV, _friendUserData.Rank);
        kNameLabel.textlocalize = _friendUserData.GetNickName();
        //kIDLabel.textlocalize = _friendUserData.UUID.ToString();

        
        kIDLabel.textlocalize = string.Format("ID : {0}\n{1}", _friendUserData.UUID, GameSupport.GetFriendLastConnectTimeString(_friendUserData.LastConnectTime));
        kAddBtn.gameObject.SetActive(true);
        if (_friendAddFlag)
            kAddBtnSpr.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
        else
            kAddBtnSpr.transform.localRotation = Quaternion.identity;

        if(delFlag)
        {
            kAddBtn.gameObject.SetActive(false);
        }
	}
 
	public void OnClick_Slot()
	{
	}
 

	
	public void OnClick_AddBtn()
	{
        if (_friendUserData == null)
            return;

        if (_friendAddFlag)
            AddFriendAsk();
        else
            DelFriendAsk();
	}

    private void AddFriendAsk()
    {
        if (GameInfo.Instance.CommunityData.FriendToAskList.Count >= GameInfo.Instance.GameConfig.FriendAskMaxNumber)
        {
            //친구 요청 대기열이 가득 찼습니다, 더이상 친구 요청을 할 수 없습니다.
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3180));
            return;
        }

        if (GameInfo.Instance.CommunityData.FriendList.Count >= GameInfo.Instance.GameConfig.FriendAddMaxNumber)
        {
            //친구 목록이 가득찼습니다, 더이상 친구요청을 할 수 없습니다.
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3181));
            return;
        }

        FriendUserData friendUser = GameInfo.Instance.CommunityData.FriendList.Find(x => x.UUID == _friendUserData.UUID);
        if (friendUser != null)
        {
            //이미 친구인 유저입니다.
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3182));
            return;
        }

        friendUser = GameInfo.Instance.CommunityData.FriendToAskList.Find(x => x.UUID == _friendUserData.UUID);
        if (friendUser != null)
        {
            //이미 친구요청을 한 유저입니다.
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3183));
            return;
        }

        friendUser = GameInfo.Instance.CommunityData.FriendAskFromUserList.Find(x => x.UUID == _friendUserData.UUID);
        if (friendUser != null)
        {
            //친구요청 수락을 하지 않은 유저입니다.미승인 탭을 확인해주세요.
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3184));
            return;
        }

        GameInfo.Instance.Send_ReqFriendAsk(_friendUserData.UUID, OnAckFriendAsk);
    }

    private void DelFriendAsk()
    {
        List<long> uuidList = new List<long>();
        uuidList.Add(_friendUserData.UUID);

        GameInfo.Instance.Send_ReqFriendAskDel(uuidList, OnAckFriendAskDel);
    }

    public void OnAckFriendAsk(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        if (this.ParentGO == null)
            return;

        UIFriendPopup friendPopup = ParentGO.GetComponent<UIFriendPopup>();
        if (friendPopup == null)
        {
            return;
        }
        friendPopup.SelectSlot(_index, true);
    }

    public void OnAckFriendAskDel(int result, PktMsgType pktMsg)
    {
        if (result != 0)
            return;

        if (this.ParentGO == null)
            return;

        for(int i = 0; i < GameInfo.Instance.CommunityData.FriendToAskList.Count; i++)
        {
            FriendUserData friendUser = GameInfo.Instance.CommunityData.FriendToAskList[i];
            if (friendUser == null)
                continue;

            if(friendUser.UUID == _friendUserData.UUID)
            {
                GameInfo.Instance.CommunityData.FriendToAskList.Remove(friendUser);
                break;
            }
        }

        UIFriendPopup friendPopup = ParentGO.GetComponent<UIFriendPopup>();
        if (friendPopup == null)
        {
            return;
        }
        friendPopup.SelectSlot(_index, false);
    }
}
