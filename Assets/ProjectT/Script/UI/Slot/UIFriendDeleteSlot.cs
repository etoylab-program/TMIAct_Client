using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFriendDeleteSlot : FSlot {

    public UITexture kUserTex;
    public UILabel kRankLabel;
    public UILabel kNameLabel;
    public UILabel kIDLabel;


    [Header("[Circle]")]
    [SerializeField] private UILabel circleNameLabel = null;
    [SerializeField] private UITexture circleFlagTex = null;
    [SerializeField] private UITexture circleMarkTex = null;

	private FriendUserData _friendUserData;
	private int _index;


	public void UpdateSlot(int index, FriendUserData data) {
		if (data == null) {
			return;
		}

		_index = index;
		_friendUserData = data;

		LobbyUIManager.Instance.GetUserMarkIcon(ParentGO, this.gameObject, data.UserMark, ref kUserTex);

		kRankLabel.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.RANK_TXT_NOW_LV, _friendUserData.Rank);
		kNameLabel.textlocalize = _friendUserData.GetNickName();
		kIDLabel.textlocalize = string.Format("ID : {0} {1}", _friendUserData.UUID, GameSupport.GetFriendLastConnectTimeString(_friendUserData.LastConnectTime));

		//서클 정보
		circleNameLabel.textlocalize = data.CircleInfo.Name;

		circleFlagTex.mainTexture = LobbyUIManager.Instance.GetCircleMarkTexture(data.CircleInfo.FlagId);
		circleMarkTex.mainTexture = LobbyUIManager.Instance.GetCircleMarkTexture(data.CircleInfo.MarkId);
		circleFlagTex.color = LobbyUIManager.Instance.GetCircleMarkColor(data.CircleInfo.ColorId);
	}
}
