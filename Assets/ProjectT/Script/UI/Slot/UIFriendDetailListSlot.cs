using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIFriendDetailListSlot : FSlot
{
    enum eFriendRoomVisit
    {
        FRIEND_ROOM_ON = 0,
        FRIEND_ROOM_OFF = 1,
        _MAX_,
    }

	public UISprite kbgSpr;
	public UITexture kUserTex;
	public UILabel kRankLabel;
	public UILabel kNameLabel;
	public UILabel kIDLabel;
	public UIButton kDeleteBtn;
	public UIButton kAddBtn;

    public FToggle kRoomToggle;
    public UIButton kRoomToggleOn;
    public UIButton kRoomToggleOff;
    public UIButton kUseFriendDelteBtn;
    public UIButton kUseFriendGotoRoom;
    public UIButton kUseFriendReciveBtn;
    public UISprite kUseFriendReciveIconSpr;

    [Header("[PVP]")]
    public UIButton BtnPVP;
    public UISprite SprDisablePVP;

    [Header("[Circle]")]
    [SerializeField] private UILabel circleNameLabel = null;
    [SerializeField] private UITexture circleFlagTex = null;
    [SerializeField] private UITexture circleMarkTex = null;

    [Header( "Friend Popup" )]
    [SerializeField] private UISprite _SelSpr;

    private FriendUserData _friendUserData;
    private int _index;

    private bool mIsEditMode;
    private bool mIsSelect;

    public void Awake()
    {
        kRoomToggle.EventCallBack = OnRoomToggle;
    }

	public void UpdateSlot( int index, FriendUserData data, bool useFriend, bool isEditMode, bool isSelect ) {
		if ( data == null ) {
			return;
		}

		_index = index;
		_friendUserData = data;

		mIsEditMode = isEditMode;

        //에디터 모드 일때는 버튼 비활성화
        kRoomToggleOn.isEnabled = !mIsEditMode;
        kRoomToggleOff.isEnabled = !mIsEditMode;
        kUseFriendDelteBtn.isEnabled = !mIsEditMode;
        kUseFriendGotoRoom.isEnabled = !mIsEditMode;
        kUseFriendReciveBtn.isEnabled = !mIsEditMode;
        BtnPVP.isEnabled = !mIsEditMode;

        if ( mIsEditMode ) {
            SetSelect( isSelect );
		}
        else {
			SetSelect( false );
		}

		LobbyUIManager.Instance.GetUserMarkIcon( ParentGO, this.gameObject, data.UserMark, ref kUserTex );

		kRankLabel.textlocalize = FLocalizeString.Instance.GetText( (int)eTEXTID.RANK_TXT_NOW_LV, _friendUserData.Rank );
		kNameLabel.textlocalize = _friendUserData.GetNickName();
		//kIDLabel.textlocalize = _friendUserData.UUID.ToString();
		kIDLabel.textlocalize = string.Format( "ID : {0} {1}", _friendUserData.UUID, GameSupport.GetFriendLastConnectTimeString( _friendUserData.LastConnectTime ) );

		kDeleteBtn.gameObject.SetActive( !useFriend );
		kAddBtn.gameObject.SetActive( !useFriend );

		kRoomToggle.gameObject.SetActive( useFriend );
		kUseFriendDelteBtn.gameObject.SetActive( useFriend );
		kUseFriendGotoRoom.gameObject.SetActive( useFriend );
		kUseFriendReciveBtn.gameObject.SetActive( useFriend );

		BtnPVP.SetActive( useFriend );
		SprDisablePVP.gameObject.SetActive( !data.HasArenaInfo || GameInfo.Instance.UserBattleData.Now_GradeId <= 0 );

		circleNameLabel.textlocalize = data.CircleInfo.Name;

		circleFlagTex.mainTexture = LobbyUIManager.Instance.GetCircleMarkTexture( data.CircleInfo.FlagId );
		circleMarkTex.mainTexture = LobbyUIManager.Instance.GetCircleMarkTexture( data.CircleInfo.MarkId );
		circleFlagTex.color = LobbyUIManager.Instance.GetCircleMarkColor( data.CircleInfo.ColorId );

		if ( useFriend ) {
			//Log.Show( data.GetNickName() + " / " + _friendUserData.FriendRoomFlagWithMyRoom, Log.ColorType.Black );
			kRoomToggle.SetToggle( !_friendUserData.FriendRoomFlagWithMyRoom ? (int)eFriendRoomVisit.FRIEND_ROOM_ON : (int)eFriendRoomVisit.FRIEND_ROOM_OFF, SelectEvent.Code );
			kUseFriendReciveBtn.enabled = _friendUserData.FriendPointTakeFlag;

			//친구 포인트 받기
			if ( _friendUserData.FriendPointTakeFlag ) {
				kUseFriendReciveBtn.enabled = true;
				kUseFriendReciveBtn.normalSprite = "btn_Base_circle_s_yellow";
				kUseFriendReciveIconSpr.spriteName = "ico_FPRecive";
			}
			else {
				kUseFriendReciveBtn.enabled = false;
				kUseFriendReciveBtn.normalSprite = "btn_Base_circle_s_off";
				kUseFriendReciveIconSpr.spriteName = "ico_FPRecive_dis";
			}

			//친구 프라이빗 룸 이동
			if ( !_friendUserData.FriendRoomFlagWithFriendRoom ) {
				kUseFriendGotoRoom.enabled = true;
				kUseFriendGotoRoom.normalSprite = "btn_Base_circle_s";
			}
			else {
				kUseFriendGotoRoom.enabled = false;
				kUseFriendGotoRoom.normalSprite = "btn_Base_circle_s_off";
			}
		}
	}

    public void SetSelect( bool isSelect ) {
        mIsSelect = isSelect;
		_SelSpr.SetActive( mIsSelect );
    }

    public void OnClick_Slot()
	{
        if ( ParentGO == null ) {
            return;
        }

        if ( mIsEditMode ) {
			UIFriendPopup friendPopup = ParentGO.GetComponent<UIFriendPopup>();
			if ( friendPopup != null ) {
				friendPopup.SelectEditSlot( _friendUserData);
			}
		}
	}

	public void OnClick_DeleteBtn()
	{
        GameInfo.Instance.Send_ReqFriendAnswer(_friendUserData.UUID, false, OnAckFriendAsk);
	}
	
	public void OnClick_AddBtn()
	{
        if(GameInfo.Instance.CommunityData.FriendList.Count >= GameInfo.Instance.GameConfig.FriendAddMaxNumber)
        {
            //더 이상 친구를 추가할수 없습니다.
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3181));
            return;
        }

        FriendUserData friendUser = GameInfo.Instance.CommunityData.FriendList.Find(x => x.UUID == _friendUserData.UUID);
        if(friendUser != null)
        {
            //이미 친구로 등록된 유저입니다.
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3182));
            LobbyUIManager.Instance.Renewal("FriendPopup");
            return;
        }

        GameInfo.Instance.Send_ReqFriendAnswer(_friendUserData.UUID, true, OnAckFriendAsk);
    }

    public void OnAckFriendAsk(int result, PktMsgType pktMsg)
    {
        if (result != 0)
            return;

        if (this.ParentGO == null)
            return;

        UIFriendPopup friendPopup = ParentGO.GetComponent<UIFriendPopup>();
        if (friendPopup != null)
        {
            friendPopup.SelectSlot(_index, true);
        }
    }

    public bool OnRoomToggle(int select, SelectEvent type)
    {
        if (type == SelectEvent.Enable)
            return false;

        if(type == SelectEvent.Click)
        {
            //eFriendRoomVisit roomvisit = (eFriendRoomVisit)select;

            List<long> uuidlist = new List<long>();
            uuidlist.Add(_friendUserData.UUID);
            Log.Show("Select : " + select);
            //GameInfo.Instance.Send_ReqFriendRoomVisitFlag(uuidlist, select != (int)eFriendRoomVisit.FRIEND_ROOM_ON ? true : false, OnAckFriendRoomVisitFlag);
            GameInfo.Instance.Send_ReqFriendRoomVisitFlag(uuidlist, _friendUserData.FriendRoomFlagWithMyRoom, OnAckFriendRoomVisitFlag);

            return true;
        }

        return true;
    }

    public void OnClick_UseFriendDeleteBtn()
    {
        FriendConfirmPopup.ShowFriendConfirmPopup(_friendUserData);
    }

    //친구 프라이빗룸으로 이동하기
    public void OnClick_UseFriendGotoRoom()
    {
        if (_friendUserData == null)
            return;

        if(GameInfo.Instance.FriendRoomSaveUserUUID == _friendUserData.UUID)
        {
            Log.Show("Not Send Room Msg", Log.ColorType.Red);
            OnAckFriendRoomInfoGet(0, null);
            return;
        }


        Log.Show("Send Room Msg", Log.ColorType.Red);
        GameInfo.Instance.Send_ReqFriendRoomInfoGet(_friendUserData, OnAckFriendRoomInfoGet);
    }

    //친구 포인트 받기
    public void OnClick_UseFriendReciveBtn()
    {
        List<long> uuidList = new List<long>();
        uuidList.Add(_friendUserData.UUID);

        GameInfo.Instance.Send_ReqFriendPointTake(uuidList, OnAckFriendPointTake);
    }

    public void OnAckFriendKick(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        if (this.ParentGO == null)
            return;

        LobbyUIManager.Instance.Renewal("FriendPopup");
    }

    public void OnAckFriendPointTake(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        if (this.ParentGO == null)
            return;

        FriendUserData friendUser = GameInfo.Instance.CommunityData.FriendList.Find(x => x.UUID == _friendUserData.UUID);
        if (friendUser != null)
        {
            friendUser.UpdateBitFlag(FriendUserData.eFriendFlag.TAKE_FP, false);
        }

        NotificationManager.Instance.CheckNotification(NotificationManager.eTYPE.FRIEND_ALL_CONFIRM);

        LobbyUIManager.Instance.Renewal("FriendPopup");
    }

    public void OnAckFriendRoomVisitFlag(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;
        PktInfoFriendRoomFlag pktInfoFriendRoomFlag = pktmsg as PktInfoFriendRoomFlag;
        FriendUserData frienduser = GameInfo.Instance.CommunityData.FriendList.Find(x => x.UUID == _friendUserData.UUID);
        if(frienduser != null)
        {
            frienduser.UpdateBitFlag(FriendUserData.eFriendFlag.MY_ROOM_VISIT, !pktInfoFriendRoomFlag.accept_);
            Log.Show(frienduser.GetNickName() + " / " + frienduser.FriendRoomFlagWithMyRoom + " / " + pktInfoFriendRoomFlag.accept_, Log.ColorType.Red);

            if(frienduser.FriendRoomFlagWithMyRoom)
            {
                //비허용
                MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3199), frienduser.GetNickName() ) );
            }
            else
            {
                //허용
                MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3198), frienduser.GetNickName() ) );
            }
        }

        LobbyUIManager.Instance.Renewal("FriendPopup");
    }

    public void OnAckFriendRoomInfoGet(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        GameInfo.Instance.FriendRoomUserUUID = (long)_friendUserData.UUID;
        GameInfo.Instance.FriendRoomSaveUserUUID = (long)_friendUserData.UUID;
        LobbyUIManager.Instance.HideUI("FriendPopup", true);
        
        Lobby.Instance.MoveToRoom(GameInfo.Instance.FriendRoomSlotData.TableID);

    }

    public void OnBtnPVP()
    {
        if (GameSupport.IsLockArena()) return;
        UIValue.Instance.SetValue(UIValue.EParamType.CardFormationType, eCharSelectFlag.ARENA);
        List<ulong> reqUUID = new List<ulong>();
        reqUUID.Add((ulong)_friendUserData.UUID);

        GameInfo.Instance.Send_ReqCommunityUserArenaInfoGet(GameInfo.eCommunityUserInfoGetType.ARENA, reqUUID, OnFriendPVP);
    }

    private void OnFriendPVP(int result, PktMsgType pktMsg)
    {
        if(result != 0)
        {
            return;
        }

        UIValue.Instance.SetValue(UIValue.EParamType.IsFriendPVP, true);
        LobbyUIManager.Instance.ShowUI("ArenaBattleConfirmPopup", true);
    }

#if UNITY_EDITOR
    private void CreateTestOpponentTeamData()
    {
        GameInfo.Instance.MatchTeam.SetUserNickName( "김혁" );
        GameInfo.Instance.MatchTeam.UserLv = 99;
        GameInfo.Instance.MatchTeam.charlist.Clear();

        GameClientTable.NPCCharRnd.Param npcRnd = GameInfo.Instance.GameClientTable.FindNPCCharRnd(x => x.ID == 31);

        List<int> listCharId = new List<int>();

        int randID = UnityEngine.Random.Range(1, GameInfo.Instance.GameClientTable.NPCCharRnds.Count + 1);
        npcRnd = GameInfo.Instance.GameClientTable.FindNPCCharRnd(x => x.ID == randID);

        List<GameTable.Character.Param> listAllCharacter = new List<GameTable.Character.Param>();
        listAllCharacter.AddRange(GameInfo.Instance.GameTable.Characters);

        for (int i = 0; i < (int)eArenaTeamSlotPos._MAX_; i++)
            listCharId.Add(0);

        for (int i = 0; i < npcRnd.CharCnt; i++)
        {
            int randIndex = Random.Range(0, listAllCharacter.Count);
            listCharId[(int)eArenaTeamSlotPos.LAST_POS - i] = listAllCharacter[randIndex].ID;

            listAllCharacter.RemoveAt(randIndex);
        }

        for (int i = 0; i < listCharId.Count; i++)
        {
            if (listCharId[i] <= 0)
            {
                GameInfo.Instance.MatchTeam.charlist.Add(null);
            }
            else
            {
                TeamCharData teamCharData = CreateTestTeamCharData(listCharId[i], null, npcRnd);
                GameInfo.Instance.MatchTeam.charlist.Add(teamCharData);
            }
        }
    }

    private TeamCharData CreateTestTeamCharData(int charTableId, WorldPVP.sTestPVPCharData testPVPCharData, GameClientTable.NPCCharRnd.Param npcRnd)
    {
        TeamCharData teamCharData = GameSupport.CreateArenaEnemyTeamCharData(charTableId, npcRnd);

        int charId = charTableId * 1000;

        // 무기
        int weaponId = teamCharData.MainWeaponData.TableID;

        //무기 추가 PVP씬에서 실행했을때만..
        long itemUID = NetLocalSvr.Instance.AddWeapon(weaponId);
        if (itemUID != -1)
        {
            teamCharData.CharData.EquipWeaponUID = itemUID;
            WeaponData weapondata = GameInfo.Instance.GetWeaponData(itemUID);
            weapondata.Level = teamCharData.MainWeaponData.Level;
            weapondata.SkillLv = teamCharData.MainWeaponData.SkillLv;
            weapondata.Wake = teamCharData.MainWeaponData.Wake;

            teamCharData.MainWeaponData = weapondata;
        }

        // 장착 스킬
        for (int i = 0; i < teamCharData.CharData.EquipSkill.Length; i++)
        {
            teamCharData.CharData.EquipSkill[i] = 0;
        }

        List<int> listSkillId = new List<int>();
        if (testPVPCharData != null && testPVPCharData.SkillIds.Length > 0)
        {
            listSkillId.AddRange(testPVPCharData.SkillIds);
        }

        teamCharData.CharData.PassvieList.Clear();
        teamCharData.CharData.PassvieList.Add(new PassiveData(charId + 301, 1)); // 오의
        teamCharData.CharData.EquipSkill[0] = charId + 301;

        if (listSkillId.Count > 0)
        {
            for (int i = 0; i < listSkillId.Count; i++)
            {
                teamCharData.CharData.PassvieList.Add(new PassiveData(listSkillId[i], 1));

                if (i < (int)eCOUNT.SKILLSLOT)
                {
                    teamCharData.CharData.EquipSkill[i + 1] = listSkillId[i];
                }
            }
        }

        // 장착 서포터
        for (int i = 0; i < teamCharData.CharData.EquipCard.Length; i++)
        {
            teamCharData.CharData.EquipCard[i] = 0;
        }

        List<int> listSupporterId = new List<int>();

        if (testPVPCharData != null && testPVPCharData.SupporterIds.Length > 0)
        {
            listSupporterId.AddRange(testPVPCharData.SupporterIds);
        }

        teamCharData.CardList.Clear();
        for (int i = 0; i < listSupporterId.Count; i++)
        {
            if (i >= (int)eCOUNT.CARDSLOT)
            {
                continue;
            }

            itemUID = NetLocalSvr.Instance.AddCard(listSupporterId[i]);
            if (itemUID <= 0)
            {
                continue;
            }

            teamCharData.CharData.EquipCard[i] = itemUID;

            CardData cardData = GameInfo.Instance.GetCardData(itemUID);
            cardData.Level = 1;
            cardData.SkillLv = 1;
            cardData.Wake = 1;

            teamCharData.CardList.Add(cardData);
        }

        return teamCharData;
    }
#endif
}
