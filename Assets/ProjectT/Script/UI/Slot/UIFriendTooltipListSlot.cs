using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIFriendTooltipListSlot : FSlot {
 

	public UISprite kbgSpr;
	public UITexture kUserTex;
	public UILabel kRankLabel;
	public UILabel kNameLabel;
	public UILabel kIDLabel;
	public UIButton kDetailBtn;
    public UISprite kDetailBtnSpr;


    private FriendUserData _friendUserData;
    private bool _friendAddFlag = false;

    public void UpdateSlot(int slotIndex, FriendUserData data)
    {
        _friendUserData = data;
        //_friendAddFlag = addFlag;

        LobbyUIManager.Instance.GetUserMarkIcon(ParentGO, this.gameObject, _friendUserData.UserMark, ref kUserTex);

        kRankLabel.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.RANK_TXT_NOW_LV, _friendUserData.Rank);
        kNameLabel.textlocalize = _friendUserData.GetNickName();
        //kIDLabel.textlocalize = _friendUserData.UUID.ToString();


        kIDLabel.textlocalize = string.Format("ID : {0}\n{1}", _friendUserData.UUID, GameSupport.GetFriendLastConnectTimeString(_friendUserData.LastConnectTime));
        kDetailBtn.gameObject.SetActive(false);
        if (_friendAddFlag)
            kDetailBtnSpr.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
        else
            kDetailBtnSpr.transform.localRotation = Quaternion.identity;

        //if(delFlag)
        //{
        //    kDetailBtn.gameObject.SetActive(false);
        //}
    }	
	public void OnClick_DetailBtn()
	{
        if (_friendUserData == null)
            return;

        FriendConfirmPopup.ShowFriendConfirmPopup(_friendUserData);
    }
}
