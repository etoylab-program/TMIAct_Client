using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIFriendPopup : FComponent {
    [Header("UIFriendPopup")]
    [SerializeField] private FTab _MainFTab;

    [SerializeField] private List<GameObject> _RootObjList;
    [SerializeField] private List<GameObject> _BottomObjList; // LeeHyungKoo - 하단 UI도 탭 변경에 반응하게 통합

    [Header("Friend List")]
    [SerializeField] private FList _FriendFList;

    [SerializeField] private UILabel _FriendEmptyLabel;
    [SerializeField] private List<UISprite> _SortSprList;
    [SerializeField] private UILabel _SortLabel;
    [SerializeField] private UILabel _FriendGiveTimeLabel;
    [SerializeField] private UISprite _OrderSpr;

    [SerializeField] private UILabel _FriendCountLabel;

    [SerializeField] private UISprite _FriendAllGivePointIconSpr;

    [SerializeField] private UIButton _FriendAllGivePointBtn;
    [SerializeField] private UIButton _FriendAllTakePointBtn;

    [SerializeField] private UIGoodsUnit _FriendPointUnit;

    [Header("Friend Suggest")]
    [SerializeField] private FList _SuggestFList;
    [SerializeField] private FList _WaitFList;

    [SerializeField] private UILabel _WaitCountLabel;

    [SerializeField] private UILabel _MyUUIDLabel;

    [SerializeField] private UILabel _SuggestEmptyLabel;
    [SerializeField] private UILabel _WaitEmptyLabel;

    [SerializeField] private UIInput _SearchUuidInput;

    [Header("Friend Ask From User")]
    [SerializeField] private FList _AskFromUserFList;

    [SerializeField] private UILabel _AskFromUserEmptyLabel;
    [SerializeField] private UILabel _AskFromUserCountLabel;


    [Header("Edit")]
    [SerializeField] private GameObject _EditObj;

    [SerializeField] private UILabel _EditTitleLabel;
    [SerializeField] private UILabel _EditSelectNumberLabel;
    [SerializeField] private UILabel _EditPrivateRoomInfoLabel;

    [SerializeField] private UIButton _EditDeleteBtn;
    [SerializeField] private UIButton _EditPublicBtn;
    [SerializeField] private UIButton _EditPrivateBtn;

    //스크롤 뷰 최상단으로 갱신 여부
    private bool _isImmediate = false;
    private int _mainSelectIndex = 0;
    private int _subSelectIndex = 0;

    private enum eMainTabType {
        FRIEND = 0,
        FRIEND_SUGGEST,
        ASK_FROM_USER,
    }

    private enum eSortType {
        CONNECT_TIME,
        RANK,
        FRIEND_PVP,
        PRIVATE_ROOM,
        MAX,
    }

    private enum eOrderType {
        ASCENDING,
        DESCENDING,
        MAX,
    }

    private enum eEditType {
        DELETE,
        PRIVATE_ROOM,
    }

    private List<long> mFriendPointTakeUuidList = new List<long>();
    private List<FriendUserData> mFriendSelectList = new List<FriendUserData>();

    private eMainTabType mCurTabType;
    private eSortType mSortType;
    private eOrderType mOrderType;
    private eEditType mEditType;

    private bool mIsAllFriendGiveTime;

    private float mWaitTime;

    public override void Awake() {
        base.Awake();

        _MainFTab.EventCallBack = OnEventMainFTabSelect;

        _FriendFList.EventUpdate = OnEventFriendFListUpdate;
        _FriendFList.EventGetItemCount = OnEventFriendFListGetItemCount;
        _FriendFList.InitBottomFixing();
        _FriendFList.UpdateList();

        _SuggestFList.EventUpdate = OnEventSuggestFListUpdate;
        _SuggestFList.EventGetItemCount = OnEventSuggestFListGetItemCount;
        _SuggestFList.InitBottomFixing();
        _SuggestFList.UpdateList();

        _WaitFList.EventUpdate = OnEventWaitFListUpdate;
        _WaitFList.EventGetItemCount = OnEventWaitFListGetItemCount;
        _WaitFList.InitBottomFixing();
        _WaitFList.UpdateList();

        _AskFromUserFList.EventUpdate = OnEventAskFromUserFListUpdate;
        _AskFromUserFList.EventGetItemCount = OnEventAskFromUserFListGetItemCount;
        _AskFromUserFList.InitBottomFixing();
        _AskFromUserFList.UpdateList();
    }

    public override void InitComponent() {
        base.InitComponent();

        _EditObj.SetActive(false);
        _MainFTab.SetTab((int)eMainTabType.FRIEND, SelectEvent.Code);

        _SearchUuidInput.defaultText = FLocalizeString.Instance.GetText(1531);

        mSortType = eSortType.CONNECT_TIME;
        mOrderType = eOrderType.ASCENDING;

        SetAllGivePointRemainTime();
        SetOrderSprite();
        SortList();

        mWaitTime = 0.0f;

        _isImmediate = false;
        _mainSelectIndex = 0;
        _subSelectIndex = 0;

        mFriendSelectList.Clear();
    }

    public override void OnEnable() {
        InitComponent();
        base.OnEnable();
    }

    public override void Renewal(bool bChildren = false) {
        base.Renewal(bChildren);

        switch (mCurTabType) {
            case eMainTabType.FRIEND: {
                    _FriendEmptyLabel.SetActive(GameInfo.Instance.CommunityData.FriendList.Count <= 0);

                    if ( 0 <= _mainSelectIndex ) {
                        _FriendFList.SpringSetFocus( _mainSelectIndex, 0.5f, _isImmediate );
                    }

                    _FriendFList.RefreshNotMoveAllItem();

                    string currentCount = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_NOW_TEXT_COLOR), GameInfo.Instance.CommunityData.FriendList.Count);
                    _FriendCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(218), currentCount, GameInfo.Instance.GameConfig.FriendAddMaxNumber);

                    _FriendPointUnit.InitGoodsUnit(eGOODSTYPE.FRIENDPOINT, GameInfo.Instance.UserData.Goods[(int)eGOODSTYPE.FRIENDPOINT], GameInfo.Instance.GameConfig.LimitMaxFriendPoint);

                    SetAllTakePointRemainTime();

                    if (_EditObj.gameObject.activeSelf) {
                        //상단 탭 비활성화
                        for (int i = 0; i < _MainFTab.kBtnList.Count; i++) {
                            _MainFTab.SetEnabled(i, false);
                        }

                        _EditDeleteBtn.SetActive(mEditType == eEditType.DELETE);
                        _EditPublicBtn.SetActive(mEditType == eEditType.PRIVATE_ROOM);
                        _EditPrivateBtn.SetActive(mEditType == eEditType.PRIVATE_ROOM);

                        _EditSelectNumberLabel.gameObject.SetActive(mEditType == eEditType.DELETE);
                        _EditPrivateRoomInfoLabel.gameObject.SetActive(mEditType == eEditType.PRIVATE_ROOM);

                        string titleStr = string.Empty;
                        switch (mEditType) {
                            case eEditType.DELETE: {
                                    titleStr = FLocalizeString.Instance.GetText(1532);

                                    string colorNumber = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_NOW_TEXT_COLOR), mFriendSelectList.Count);
                                    _EditSelectNumberLabel.textlocalize = $"{colorNumber} / {GameInfo.Instance.CommunityData.FriendList.Count}";
                                }
                                break;

                            case eEditType.PRIVATE_ROOM: {
                                    titleStr = FLocalizeString.Instance.GetText(1545);
                                    _EditPrivateRoomInfoLabel.textlocalize = FLocalizeString.Instance.GetText(3382);
                                }
                                break;

                            default: {

                                }
                                break;
                        }

                        _EditTitleLabel.textlocalize = titleStr;
                    }
                    else {
                        //상단 탭 활성화
                        for (int i = 0; i < _MainFTab.kBtnList.Count; i++) {
                            _MainFTab.SetEnabled(i, true);
                        }
                    }
                }
                break;

            case eMainTabType.FRIEND_SUGGEST: {
                    _SuggestEmptyLabel.SetActive(GameInfo.Instance.CommunityData.FriendSuggestList.Count <= 0);
                    _WaitEmptyLabel.SetActive(GameInfo.Instance.CommunityData.FriendToAskList.Count <= 0);

                    _MyUUIDLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1540), GameInfo.Instance.UserData.UUID);

                    string waitCount = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_NOW_TEXT_COLOR), GameInfo.Instance.CommunityData.FriendToAskList.Count);
                    _WaitCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(218), waitCount, GameInfo.Instance.GameConfig.FriendAskMaxNumber);

                    _SuggestFList.RefreshNotMoveAllItem();
                    if (0 <= _mainSelectIndex) {
                        _SuggestFList.SpringSetFocus(_mainSelectIndex, 0.5f, _isImmediate);
                    }

                    _WaitFList.RefreshNotMoveAllItem();
                    if (0 <= _subSelectIndex) {
                        _WaitFList.SpringSetFocus(_subSelectIndex, 0.5f, _isImmediate);
                    }
                }
                break;

            case eMainTabType.ASK_FROM_USER: {
                    _AskFromUserEmptyLabel.SetActive(GameInfo.Instance.CommunityData.FriendAskFromUserList.Count <= 0);

                    string currentCount = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_NOW_TEXT_COLOR), GameInfo.Instance.CommunityData.FriendAskFromUserList.Count);
                    _AskFromUserCountLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(218), currentCount, GameInfo.Instance.GameConfig.FriendReadyMaxNumber);

                    _AskFromUserFList.RefreshNotMoveAllItem();
                    _AskFromUserFList.SpringSetFocus(_mainSelectIndex, 0.5f, _isImmediate);
                }
                break;

            default: {

                }
                break;
        }

        _isImmediate = false;
        _mainSelectIndex = -1;
        _subSelectIndex = -1;
    }

    public void FixedUpdate() {
        if (mCurTabType == eMainTabType.FRIEND && !mIsAllFriendGiveTime) {
            mWaitTime += Time.fixedDeltaTime;
            if (mWaitTime >= 1.0f) {
                SetAllGivePointRemainTime();
                mWaitTime = 0.0f;
            }
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            _FriendFList.InitBottomFixing();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            _SuggestFList.InitBottomFixing();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            _WaitFList.InitBottomFixing();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            _AskFromUserFList.InitBottomFixing();
        }

#endif
    }

    public void SelectSlot(int index, bool isMain) {
        if (isMain) {
            _mainSelectIndex = index;
            _subSelectIndex = GameInfo.Instance.CommunityData.FriendToAskList.Count - 1;
        }
        else {
            _mainSelectIndex = -1;
            _subSelectIndex = index;
        }

        Renewal();
    }

    public void SelectEditSlot(FriendUserData data) {
        if (!_EditObj.activeSelf) {
            return;
        }

        //같은 친구를 찾아서 있으면 빼고 없으면 넣어준다.
        FriendUserData find = mFriendSelectList.Find(x => x.UUID == data.UUID);
        if (find == null)
            mFriendSelectList.Add(data);
        else 
            mFriendSelectList.Remove(find);

        Renewal();
    }

    public void OnClick_SortBtn() {
        ++mSortType;
        if (eSortType.MAX <= mSortType) {
            mSortType = eSortType.CONNECT_TIME;
        }

        _isImmediate = false;
        _mainSelectIndex = 0;
        _subSelectIndex = 0;

        SortList();
        Renewal();
    }

    public void OnClick_OrderBtn() {
        ++mOrderType;
        if (eOrderType.MAX <= mOrderType) {
            mOrderType = eOrderType.ASCENDING;
        }

        _isImmediate = false;
        _mainSelectIndex = 0;
        _subSelectIndex = 0;

        SetOrderSprite();
        SortList();
        Renewal();
    }

    public void OnClick_RoomEditBtn() {
        _EditObj.SetActive(true);

        mFriendSelectList.Clear();

        mEditType = eEditType.PRIVATE_ROOM;

        Renewal();
    }

    public void OnClick_FriendEditBtn() {
        _EditObj.SetActive(true);

        mFriendSelectList.Clear();

        mEditType = eEditType.DELETE;

        Renewal();
    }

    public void OnClick_SearchBtn() {
        if (_SearchUuidInput.value.Length <= 0) {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3179));
            return;
        }

        if (long.TryParse(_SearchUuidInput.value, out long serachUUID)) {
            GameSupport.ShowFriendPopupWithSerachFriendList(serachUUID.ToString());

            //맨 아래 항목을 보고 있다가 서치를 하면 서치 결과가 너무 위쪽에 있어 안보이는 버그가 있다.
            //해결책으로 우선 리스트를 최상단으로 이동 시켜줬다.
            _SuggestFList.SpringSetFocus(0, 0.5f, true);
            
            _isImmediate = true;
            _mainSelectIndex = 0;
            _subSelectIndex = 0;
        }
        else {
            Log.Show("Faild UUID");
        }
    }

    public void OnClick_AllGiveBtn() {
        if (GameInfo.Instance.CommunityData.FriendList.Count <= 0) {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(1536));
            return;
        }

        MessagePopup.OKCANCEL(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3191, GameInfo.Instance.CommunityData.FriendList.Count, GameInfo.Instance.GameConfig.FriendSendFPCoolHour),
            () => GameInfo.Instance.Send_ReqFriendPointGive(OnNetFriendPointGive));
    }

    public void OnClick_AllConfirmBtn() {
        List<FriendUserData> friendUserDataList = GameInfo.Instance.CommunityData.FriendList.FindAll(x => x.FriendPointTakeFlag);
        if (friendUserDataList.Count <= 0) {
            SetAllTakePointRemainTime();
            return;
        }

        mFriendPointTakeUuidList.Clear();
        for (int i = 0; i < friendUserDataList.Count; i++) {
            mFriendPointTakeUuidList.Add(friendUserDataList[i].UUID);
        }

        GameInfo.Instance.Send_ReqFriendPointTake(mFriendPointTakeUuidList, OnNetFriendPointTake);
    }

    public void OnClick_AllEditSelectBtn() {
        mFriendSelectList.Clear();
        for (int i = 0; i < GameInfo.Instance.CommunityData.FriendList.Count; i++) {
            mFriendSelectList.Add(GameInfo.Instance.CommunityData.FriendList[i]);
        }

        Renewal();
    }

    public void OnClick_AllEditDeSelectBtn() {
        mFriendSelectList.Clear();

        Renewal();
    }

    public void OnClick_EditPublicBtn() {
        Send_RoomVisitFlagList(true);
    }

    public void OnClick_EditPrivateBtn() {
        Send_RoomVisitFlagList(false);
    }

    void Send_RoomVisitFlagList(bool isOpen) {

        if (mFriendSelectList.Count <= 0) {
            //선택한 대상이 없습니다.
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3390));
            return;
        }

        List<long> filterList = new List<long>();
        for (int i = 0; i < mFriendSelectList.Count; i++) {
            if (mFriendSelectList[i].FriendRoomFlagWithMyRoom == isOpen) {
                filterList.Add(mFriendSelectList[i].UUID);
            }
        }

        //선택 가능한 항목이 없다?
        if (filterList.Count == 0) {

            if (isOpen == true) {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3384));
            }
            else {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3385));
            }
            
            //뭔가 경고 메세지 띄워 줘야 할거같음
            return;
        }

        GameInfo.Instance.Send_ReqFriendRoomVisitFlag(filterList, isOpen, delegate (int result, PktMsgType pktMsg) {
            //콜백 처리
            if (result != 0)
                return;

            PktInfoFriendRoomFlag pktInfoFriendRoomFlag = pktMsg as PktInfoFriendRoomFlag;
            FriendUserData firstUserData = null;

            for (int i = 0; i < filterList.Count; i++) {
                //친구 리스트에서 선택했던 유저를 하나씩 찾아옴
                FriendUserData frienduser = GameInfo.Instance.CommunityData.FriendList.Find(x => x.UUID == filterList[i]);
                if (frienduser != null) {
                    //첫번째 유저 저장
                    if (i == 0)
                        firstUserData = frienduser;

                    //해당유저의 비트 플레그를 바꿔줌
                    //FriendRoomFlagWithMyRoom 가 true이면 비활성(친구가 내 방에 못들어옴), false면 활성(내 방에 들어올수 있음)
                    //서버는 반대의 값으로 사용중
                    frienduser.UpdateBitFlag(FriendUserData.eFriendFlag.MY_ROOM_VISIT, !pktInfoFriendRoomFlag.accept_);

                    Log.Show(frienduser.GetNickName() + " / " + frienduser.FriendRoomFlagWithMyRoom + " / " + pktInfoFriendRoomFlag.accept_, Log.ColorType.Red);
                }
            }

            //한명과 여러명 변경의 분기 처리
            if (filterList.Count == 1) {
                if (firstUserData.FriendRoomFlagWithMyRoom)
                    MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3199), firstUserData.GetNickName()));
                else
                    MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3198), firstUserData.GetNickName()));
            }
            else {
                MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3376), firstUserData.GetNickName(), filterList.Count - 1));
            }

            //리스트 정리 및 갱신
            mFriendSelectList.Clear();
            Renewal();

        });//콜백 종료
    }


    //유저 단체 삭제 버튼
    public void OnClick_EditDeleteBtn() {

        if (mFriendSelectList.Count <= 0) {
            //선택한 대상이 없습니다.
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3390));
            return;
        }

        MessagePopup.YNFriendDelete(mFriendSelectList, Send_DeleteFriend);
    }

    //테스트 시 Send_DeleteFriend 대신 넣어준다.
    void Test_DeleteFriend() {
        for (int i = 0; i < mFriendSelectList.Count; i++) {
            GameInfo.Instance.CommunityData.FriendList.Remove(mFriendSelectList[i]);
        }

        OnAckDeleteFriend(0, null);
    }

    void Send_DeleteFriend() {
        List<long> deleteUUIDList = new List<long>();
        for (int i = 0; i < mFriendSelectList.Count; i++) {
            deleteUUIDList.Add(mFriendSelectList[i].UUID);
        }

        //서버에 삭제 요청
        GameInfo.Instance.Send_ReqFriendKick(deleteUUIDList, OnAckDeleteFriend);
    }

    void OnAckDeleteFriend(int result, PktMsgType pktMsg) {
        if (result != 0)
            return;

        if (mFriendSelectList.Count == 1) {
            
            MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3383), mFriendSelectList[0].GetNickName()));
        }
        else {
            MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3377), mFriendSelectList[0].GetNickName(), mFriendSelectList.Count - 1));
        }

        _isImmediate = true;
        _mainSelectIndex = 0;
        _subSelectIndex = 0;

        mFriendSelectList.Clear();
        Renewal();
    }

    public void OnClick_EditCloseBtn() {
        _EditObj.SetActive(false);

        mFriendSelectList.Clear();

        Renewal();
    }

    public void OnClick_ResetBtn() {
        
        GameSupport.ShowFriendPopupWithSerachFriendList(string.Empty, true);

        _isImmediate = false;
        _mainSelectIndex = 0;
        _subSelectIndex = 0;
    }

    public void OnClick_FriendShopBtn() {
        Log.Show("OnClick_FriendShopBtn");
        OnClickClose();

        UIStorePanel storePanel = LobbyUIManager.Instance.GetUI<UIStorePanel>("StorePanel");
        if (storePanel != null) {
            storePanel.DirectShow(UIStorePanel.eStoreTabType.STORE_FRIENDPOINT);
        }
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.STORE);
    }


    private void SetAllTakePointRemainTime() {
        bool mIsAllFriendTakeTime = GameSupport.GetFriendPointTakeCheck();
        _FriendAllTakePointBtn.enabled = mIsAllFriendTakeTime;
        _FriendAllTakePointBtn.normalSprite = mIsAllFriendTakeTime ? "btn_Base_Yellow_50px" : "btn_Base_dis";
    }

    private void SetAllGivePointRemainTime() {
        mIsAllFriendGiveTime = GameInfo.Instance.UserData.NextFrientPointGiveTime < GameSupport.GetCurrentServerTime();

        _FriendAllGivePointBtn.enabled = mIsAllFriendGiveTime;
        _FriendAllGivePointBtn.normalSprite = mIsAllFriendGiveTime ? "btn_Base_Red_50px" : "btn_Base_dis";
        _FriendAllGivePointIconSpr.spriteName = mIsAllFriendGiveTime ? "ico_FPGive" : "ico_FPGive_dis";

        if (mIsAllFriendGiveTime) {
            _FriendGiveTimeLabel.textlocalize = FLocalizeString.Instance.GetText(1525);
        }
        else {
            string remainTimeStr = GameSupport.GetRemainTimeString(GameInfo.Instance.UserData.NextFrientPointGiveTime, GameSupport.GetCurrentServerTime());
            _FriendGiveTimeLabel.textlocalize = string.Format("{0}\n{1}", FLocalizeString.Instance.GetText(1525), remainTimeStr);
        }
    }

    private void SetOrderSprite() {
        string spriteName = string.Empty;
        switch (mOrderType) {
            case eOrderType.ASCENDING: {
                    spriteName = "ico_Filter1";
                }
                break;

            case eOrderType.DESCENDING: {
                    spriteName = "ico_Filter2";
                }
                break;

            default: {

                }
                break;
        }

        _OrderSpr.spriteName = spriteName;
    }

    private void SortList() {
        string sortName = string.Empty;

        for (int i = 0; i < _SortSprList.Count; i++) {
            if (i == (int)mSortType) {
                _SortSprList[i].gameObject.SetActive(true);
            }
            else {
                _SortSprList[i].gameObject.SetActive(false);
            }
        }

        switch (mSortType) {
            case eSortType.CONNECT_TIME: {
                    GameInfo.Instance.CommunityData.FriendList.Sort(SortConnectTime);
                    sortName = FLocalizeString.Instance.GetText(3386);
                }
                break;

            case eSortType.RANK: {
                    GameInfo.Instance.CommunityData.FriendList.Sort(SortRank);
                    sortName = FLocalizeString.Instance.GetText(3387);
                }
                break;

            case eSortType.FRIEND_PVP: {
                    GameInfo.Instance.CommunityData.FriendList.Sort(SortFriendPVP);
                    sortName = FLocalizeString.Instance.GetText(3388);
                }
                break;

            case eSortType.PRIVATE_ROOM: {
                    GameInfo.Instance.CommunityData.FriendList.Sort(SortPrivateRoom);
                    sortName = FLocalizeString.Instance.GetText(1017);
                }
                break;

            default: {

                }
                break;
        }

        _SortLabel.textlocalize = sortName;
    }

    #region Sorting

    private int SortConnectTime(FriendUserData front, FriendUserData back) {
        int type01 = -1;
        int type02 = 1;
        if (mOrderType == eOrderType.DESCENDING) {
            type01 = 1;
            type02 = -1;
        }

        //접속시간
        if (front.LastConnectTime > back.LastConnectTime) {
            return type01;
        }
        else if (front.LastConnectTime < back.LastConnectTime) {
            return type02;
        }
        else {
            //친구 대전
            if (front.HasArenaInfo && !back.HasArenaInfo) {
                return type01;
            }
            else if (!front.HasArenaInfo && back.HasArenaInfo) {
                return type02;
            }
            else {
                //프라이빗 룸
                if (front.FriendRoomFlagWithMyRoom && !back.FriendRoomFlagWithMyRoom) {
                    return type01;
                }
                else if (!front.FriendRoomFlagWithMyRoom && back.FriendRoomFlagWithMyRoom) {
                    return type02;
                }
                else {
                    //Rank
                    if (front.Rank < back.Rank) {
                        return type01;
                    }
                    else if (front.Rank > back.Rank) {
                        return type02;
                    }
                    else {
                        //UUID
                        if (front.UUID < back.UUID) {
                            return type01;
                        }
                        else if (front.UUID > back.UUID) {
                            return type02;
                        }
                    }
                }
            }
        }

        return 0;
    }

    private int SortRank(FriendUserData front, FriendUserData back) {
        int type01 = -1;
        int type02 = 1;
        if (mOrderType == eOrderType.DESCENDING) {
            type01 = 1;
            type02 = -1;
        }

        //Rank
        if (front.Rank < back.Rank) {
            return type01;
        }
        else if (front.Rank > back.Rank) {
            return type02;
        }
        else {
            //친구 대전
            if (front.HasArenaInfo && !back.HasArenaInfo) {
                return type01;
            }
            else if (!front.HasArenaInfo && back.HasArenaInfo) {
                return type02;
            }
            else {
                //프라이빗 룸
                if (front.FriendRoomFlagWithMyRoom && !back.FriendRoomFlagWithMyRoom) {
                    return type01;
                }
                else if (!front.FriendRoomFlagWithMyRoom && back.FriendRoomFlagWithMyRoom) {
                    return type02;
                }
                else {
                    //접속 시간
                    if (front.LastConnectTime > back.LastConnectTime) {
                        return type01;
                    }
                    else if (front.LastConnectTime < back.LastConnectTime) {
                        return type02;
                    }
                    else {
                        //UUID
                        if (front.UUID < back.UUID) {
                            return type01;
                        }
                        else if (front.UUID > back.UUID) {
                            return type02;
                        }
                    }
                }
            }
        }

        return 0;
    }

    private int SortFriendPVP(FriendUserData front, FriendUserData back) {
        int type01 = -1;
        int type02 = 1;
        if (mOrderType == eOrderType.DESCENDING) {
            type01 = 1;
            type02 = -1;
        }


        //친구 대전
        if (front.HasArenaInfo && !back.HasArenaInfo) {
            return type01;
        }
        else if (!front.HasArenaInfo && back.HasArenaInfo) {
            return type02;
        }
        else {
            //프라이빗 룸
            if (front.FriendRoomFlagWithMyRoom && !back.FriendRoomFlagWithMyRoom) {
                return type01;
            }
            else if (!front.FriendRoomFlagWithMyRoom && back.FriendRoomFlagWithMyRoom) {
                return type02;
            }
            else {
                //Rank
                if (front.Rank < back.Rank) {
                    return type01;
                }
                else if (front.Rank > back.Rank) {
                    return type02;
                }
                else {
                    //접속 시간
                    if (front.LastConnectTime > back.LastConnectTime) {
                        return type01;
                    }
                    else if (front.LastConnectTime < back.LastConnectTime) {
                        return type02;
                    }
                    else {
                        //UUID
                        if (front.UUID < back.UUID) {
                            return type01;
                        }
                        else if (front.UUID > back.UUID) {
                            return type02;
                        }
                    }
                }
            }
        }

        return 0;
    }

    private int SortPrivateRoom(FriendUserData front, FriendUserData back) {
        int type01 = -1;
        int type02 = 1;
        if (mOrderType == eOrderType.DESCENDING) {
            type01 = 1;
            type02 = -1;
        }

        //프라이빗 룸
        if (front.FriendRoomFlagWithMyRoom && !back.FriendRoomFlagWithMyRoom) {
            return type01;
        }
        else if (!front.FriendRoomFlagWithMyRoom && back.FriendRoomFlagWithMyRoom) {
            return type02;
        }
        else {
            //친구 대전
            if (front.HasArenaInfo && !back.HasArenaInfo) {
                return type01;
            }
            else if (!front.HasArenaInfo && back.HasArenaInfo) {
                return type02;
            }
            else {
                //Rank
                if (front.Rank < back.Rank) {
                    return type01;
                }
                else if (front.Rank > back.Rank) {
                    return type02;
                }
                else {
                    //접속 시간
                    if (front.LastConnectTime > back.LastConnectTime) {
                        return type01;
                    }
                    else if (front.LastConnectTime < back.LastConnectTime) {
                        return type02;
                    }
                    else {
                        //UUID
                        if (front.UUID < back.UUID) {
                            return type01;
                        }
                        else if (front.UUID > back.UUID) {
                            return type02;
                        }
                    }
                }
            }
        }

        return 0;
    }

    #endregion

    private void OnNetFriendPointGive(int result, PktMsgType pktmsg) {
        if (result != 0) {
            return;
        }

        MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3188), GameInfo.Instance.CommunityData.FriendList.Count));

        SetAllGivePointRemainTime();

        Renewal();
    }

    private void OnNetFriendPointTake(int result, PktMsgType pktMsg) {
        if (result != 0) {
            return;
        }

        for (int i = 0; i < mFriendPointTakeUuidList.Count; i++) {
            FriendUserData friendUserData = GameInfo.Instance.CommunityData.FriendList.Find(x => x.UUID == mFriendPointTakeUuidList[i]);
            if (friendUserData != null) {
                friendUserData.UpdateBitFlag(FriendUserData.eFriendFlag.TAKE_FP, false);
            }
        }

        NotificationManager.Instance.CheckNotification(NotificationManager.eTYPE.FRIEND);
        NotificationManager.Instance.CheckNotification(NotificationManager.eTYPE.FRIEND_LIST);
        NotificationManager.Instance.CheckNotification(NotificationManager.eTYPE.FRIEND_APPLY);

        Renewal();
    }

    private bool OnEventMainFTabSelect(int nSelect, SelectEvent type) {
        if (type == SelectEvent.Enable) {
            return false;
        }

        for (int i = 0; i < _RootObjList.Count; i++) {
            _RootObjList[i].SetActive(i == nSelect);
            _BottomObjList[i].SetActive(i == nSelect);
        }

        mCurTabType = (eMainTabType)nSelect;

        //탭 이동간 기존 리스트 초기화
        _isImmediate = true;
        _mainSelectIndex = 0;
        _subSelectIndex = 0;

        if (type == SelectEvent.Click) {
            Renewal();
        }

        return true;
    }

    private void OnEventFriendFListUpdate(int index, GameObject obj) {
        UIFriendDetailListSlot slot = obj.GetComponent<UIFriendDetailListSlot>();
        if (slot == null) {
            return;
        }

        if (slot.ParentGO == null) {
            slot.ParentGO = this.gameObject;
        }

        FriendUserData data = null;
        if (0 <= index && index < GameInfo.Instance.CommunityData.FriendList.Count) {
            data = GameInfo.Instance.CommunityData.FriendList[index];
        }

        bool isSelect = false;
        if (data != null) {
            isSelect = mFriendSelectList.Exists(x => x.UUID == data.UUID);
        }

        slot.UpdateSlot(index, data, true, _EditObj.gameObject.activeSelf, isSelect);
    }

    private int OnEventFriendFListGetItemCount() {
        return GameInfo.Instance.CommunityData.FriendList.Count;
    }

    private void OnEventSuggestFListUpdate(int index, GameObject obj) {
        UIFriendListSlot slot = obj.GetComponent<UIFriendListSlot>();
        if (slot == null) {
            return;
        }

        if (slot.ParentGO == null) {
            slot.ParentGO = this.gameObject;
        }

        FriendUserData data = null;
        if (0 <= index && index < GameInfo.Instance.CommunityData.FriendSuggestList.Count) {
            data = GameInfo.Instance.CommunityData.FriendSuggestList[index];
        }

        slot.UpdateSlot(index, data, true);
    }

    private int OnEventSuggestFListGetItemCount() {
        return GameInfo.Instance.CommunityData.FriendSuggestList.Count;
    }

    private void OnEventWaitFListUpdate(int index, GameObject obj) {
        UIFriendListSlot slot = obj.GetComponent<UIFriendListSlot>();
        if (slot == null) {
            return;
        }

        if (slot.ParentGO == null) {
            slot.ParentGO = this.gameObject;
        }

        FriendUserData data = null;
        if (0 <= index && index < GameInfo.Instance.CommunityData.FriendToAskList.Count) {
            data = GameInfo.Instance.CommunityData.FriendToAskList[index];
        }

        slot.UpdateSlot(index, data, false);
    }

    private int OnEventWaitFListGetItemCount() {
        return GameInfo.Instance.CommunityData.FriendToAskList.Count;
    }

    private void OnEventAskFromUserFListUpdate(int index, GameObject obj) {
        UIFriendDetailListSlot slot = obj.GetComponent<UIFriendDetailListSlot>();
        if (slot == null) {
            return;
        }

        if (slot.ParentGO == null) {
            slot.ParentGO = this.gameObject;
        }

        FriendUserData data = null;
        if (0 <= index && index < GameInfo.Instance.CommunityData.FriendAskFromUserList.Count) {
            data = GameInfo.Instance.CommunityData.FriendAskFromUserList[index];
        }

        slot.UpdateSlot(index, data, false, false, false);
    }

    private int OnEventAskFromUserFListGetItemCount() {
        return GameInfo.Instance.CommunityData.FriendAskFromUserList.Count;
    }
}
