using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICircleLobbyMainPanel : FComponent
{
    [System.Serializable]
    private class Info
    {
        public GameObject Obj;
        public UILabel Name;
        public UILabel Content;
    }

    [Header("UICircleLobbyMainPanel")]
    [SerializeField] private UITexture circleFlagTex = null;
    [SerializeField] private UITexture circleMarkTex = null;
    [SerializeField] private UITexture circleColorTex = null;
    [SerializeField] private UILabel circleRankLabel = null;
    [SerializeField] private UILabel circleNameLabel = null;
    [SerializeField] private UILabel circleMemberCountLabel = null;
    [SerializeField] private UILabel leaderRankLabel = null;
    [SerializeField] private UILabel leaderNameLabel = null;
    [SerializeField] private UILabel circleBuffInfoLabel = null;
    [SerializeField] private UILabel personalBuffInfoLabel = null;
    [SerializeField] private UILabel attendanceInfoLabel = null;

    [Header("Chat")]
    [SerializeField] private GameObject chatObj = null;
    [SerializeField] private GameObject macroObj = null;
    [SerializeField] private FList chatFList = null;
    [SerializeField] private FList chatWordFList = null;
    [SerializeField] private FList chatStampFList = null;
    [SerializeField] private UIInput chatInput = null;
    [SerializeField] private UILabel chatAlramLabel = null;
    [SerializeField] private UILabel chatMiniAlramLabel = null;
    [SerializeField] private FList chatMiniFList = null;
        
    private System.DateTime _lastChatSendTime;
    private int _lastChatOverlapCount;

    private int _selectSlotTableId;

    private float _notiViewSec;
    private float _notiWaitSec;
    private int _notiViewIndex;
    private bool _isShowNoti;

    public override void Awake()
    {
        base.Awake();

        chatFList.EventUpdate = OnEventChatUpdate;
        chatFList.EventGetItemCount = OnEventChatCount;
        chatFList.UpdateList();

        chatMiniFList.EventUpdate = OnEventChatMiniUpdate;
        chatMiniFList.EventGetItemCount = OnEventChatMiniCount;
        chatMiniFList.UpdateList();

        chatWordFList.EventUpdate = OnEventWordUpdate;
        chatWordFList.EventGetItemCount = OnEventWordCount;
        chatWordFList.UpdateList();

        chatStampFList.EventUpdate = OnEventStampUpdate;
        chatStampFList.EventGetItemCount = OnEventStampCount;
        chatStampFList.UpdateList();

        chatInput.defaultText = "채팅 입력"; // Test - LeeSeungJin - Change String

        _notiViewSec = 10;
        _notiViewIndex = 0;
    }

    public override void InitComponent()
    {
        base.InitComponent();

        chatObj.SetActive(false);
        macroObj.SetActive(false);

        _selectSlotTableId = 0;
        _notiWaitSec = 0;

        _isShowNoti = FSaveData.Instance.CircleChatAlramAll;
        if (_isShowNoti)
        {
            GetNextNotiIndex(isInit: true);
            GetNotiMessage();
        }
        else
        {
            chatAlramLabel.textlocalize = chatMiniAlramLabel.textlocalize = string.Empty;
        }
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

        circleRankLabel.textlocalize = circleData.GetStringRank();
        circleNameLabel.textlocalize = circleData.Name;
        circleMemberCountLabel.textlocalize = circleData.GetStringMemberCount();

        leaderRankLabel.textlocalize = circleData.Leader.GetStringRank();
        leaderNameLabel.textlocalize = circleData.Leader.Name;

        attendanceInfoLabel.textlocalize = FLocalizeString.Instance.GetText(218, circleData.AttendanceCount, circleData.MemberCount);

        if (chatObj.activeSelf)
        {
            chatFList.RefreshNotMoveAllItem();
            chatFList.SpringSetFocus(GameInfo.Instance.CircleChatList.Count - 1, ratio: 1, isImmediate: true);
        }

        if (chatStampFList.gameObject.activeSelf)
        {
            chatStampFList.Reset();
            chatStampFList.RefreshNotMoveAllItem();
        }

        if (chatWordFList.gameObject.activeSelf)
        {
            chatWordFList.Reset();
            chatWordFList.RefreshNotMoveAllItem();
        }

        chatMiniFList.RefreshNotMoveAllItem();
    }

    private void FixedUpdate()
    {
        CircleNotification();
    }

    private void CircleNotification()
    {
        if (GameInfo.Instance.CircleNotiList.Count <= 0)
        {
            return;
        }

        if (FSaveData.Instance.CircleChatAlramAll == false)
        {
            if (_isShowNoti != FSaveData.Instance.CircleChatAlramAll)
            {
                _isShowNoti = FSaveData.Instance.CircleChatAlramAll;

                chatAlramLabel.textlocalize = chatMiniAlramLabel.textlocalize = string.Empty;
            }
            return;
        }

        if (_isShowNoti != FSaveData.Instance.CircleChatAlramAll)
        {
            _isShowNoti = FSaveData.Instance.CircleChatAlramAll;

            GetNextNotiIndex(isInit: true);
            GetNotiMessage();
        }

        _notiWaitSec += Time.fixedDeltaTime;
        if (_notiViewSec < _notiWaitSec)
        {
            GetNextNotiIndex();
            GetNotiMessage();
            _notiWaitSec = 0;
        }
    }

    private bool NotiAlramView(int index)
    {
        if (GameInfo.Instance.CircleNotiList.Count <= index)
        {
            return false;
        }

        CircleNotiData notiData = GameInfo.Instance.CircleNotiList[index];
        if (notiData == null)
        {
            return false;
        }

        if (notiData.SendTime.AddHours(1) < GameSupport.GetCurrentServerTime())
        {
            return false;
        }

        bool result = false;

        switch (notiData.NotiType)
        {
            case eCircleNotiType.ARENA_RANK_IN:
                {
                    result = FSaveData.Instance.CircleChatAlramArena;
                }
                break;
            case eCircleNotiType.TIMEATK_RANK_IN:
                {
                    result = FSaveData.Instance.CircleChatAlramTimeAttack;
                }
                break;
            case eCircleNotiType.TAKE_UR_GRADE_CARD:
            case eCircleNotiType.TAKE_UR_GRADE_WPN:
                {
                    result = FSaveData.Instance.CircleChatAlramGacha;
                }
                break;
            case eCircleNotiType.WPN_ENCHANT_SUCCESS:
                {
                    result = FSaveData.Instance.CircleChatAlramWeaponEnhance;
                }
                break;
        }

        return result;
    }

    private void GetNextNotiIndex(bool isInit = false)
    {
        int tempIndex = _notiViewIndex < 0 ? 0 : _notiViewIndex;
        int result = -1;
        int index = isInit ? tempIndex : ++tempIndex;
        for (; index < GameInfo.Instance.CircleNotiList.Count; index++)
        {
            if (NotiAlramView(index))
            {
                result = index;
                break;
            }
        }

        if (result < 0)
        {
            index = isInit ? _notiViewIndex : ++_notiViewIndex;
            for (int i = 0; i < index; i++)
            {
                if (NotiAlramView(i))
                {
                    result = i;
                    break;
                }
            }
        }

        _notiViewIndex = result;
    }

    private void GetNotiMessage()
    {
        if (_notiViewIndex < 0)
        {
            chatAlramLabel.textlocalize = chatMiniAlramLabel.textlocalize = string.Empty;
            return;
        }

        CircleNotiData circleNotiData = GameInfo.Instance.CircleNotiList[_notiViewIndex];
        if (circleNotiData == null)
        {
            chatAlramLabel.textlocalize = chatMiniAlramLabel.textlocalize = string.Empty;
            return;
        }

        string result = string.Empty;
        long value01 = circleNotiData.Values.Count < 1 ? 0 : circleNotiData.Values[0];
        long value02 = circleNotiData.Values.Count < 2 ? 0 : circleNotiData.Values[1];
        // Test - LeeSeungJin - Change String Start
        string format;
        switch (circleNotiData.NotiType)
        {
            case eCircleNotiType.ARENA_RANK_IN:
                {
                    format = "[{0}]이/가 아레나 [{1}등]에 입상하였습니다.";
                    result = string.Format(format, circleNotiData.NickName, value01);
                }
                break;
	        case eCircleNotiType.TIMEATK_RANK_IN:
                {
                    format = "[{0}]이/가 [{1}]의 [{2}등]에 입상하였습니다.";
                    result = string.Format(format, circleNotiData.NickName, value01, value02);
                }
                break;
            case eCircleNotiType.TAKE_UR_GRADE_CARD:
                {
                    format = "[{0}]이/가 서포터 [{1}]을 획득 하셨습니다.";
                    string cardName = string.Empty;
                    GameTable.Card.Param cardParam = GameInfo.Instance.GameTable.FindCard((int)value01);
                    if (cardParam != null)
                    {
                        cardName = FLocalizeString.Instance.GetText(cardParam.Name);
                    }
                    result = string.Format(format, circleNotiData.NickName, cardName);
                }
                break;
            case eCircleNotiType.TAKE_UR_GRADE_WPN:
                {
                    format = "[{0}]이/가 무기 [{1}]을 획득 하셨습니다.";
                    string weaponName = string.Empty;
                    GameTable.Weapon.Param weaponParam = GameInfo.Instance.GameTable.FindWeapon((int)value01);
                    if (weaponParam != null)
                    {
                        weaponName = FLocalizeString.Instance.GetText(weaponParam.Name);
                    }
                    result = string.Format(format, circleNotiData.NickName, weaponName);
                }
                break;
            case eCircleNotiType.WPN_ENCHANT_SUCCESS:
                {
                    format = "[{0}]이/가 무기 [{1}]의 [{2}단계]에 성공하셨습니다.";
                    string weaponName = string.Empty;
                    GameTable.Weapon.Param weaponParam = GameInfo.Instance.GameTable.FindWeapon((int)value01);
                    if (weaponParam != null)
                    {
                        weaponName = FLocalizeString.Instance.GetText(weaponParam.Name);
                    }
                    result = string.Format(format, circleNotiData.NickName, weaponName, value02);
                }
                break;
        }
        // Test - LeeSeungJin - Change String End

        chatAlramLabel.textlocalize = chatMiniAlramLabel.textlocalize = result;
    }

    private void MoveFListItem(bool isPlus, ref FList fList)
    {
        Vector2 itemSize = fList.ItemSize * fList.ItemScale + fList.Padding;

        float panelAbsLocalPosX = Mathf.Abs(fList.Panel.transform.localPosition.x);
        float indexPoint = panelAbsLocalPosX / itemSize.x;

        int index = Mathf.FloorToInt(indexPoint);
        int screenViewItemX = Mathf.FloorToInt(fList.Panel.width / itemSize.x);
        int screenViewItemY = Mathf.FloorToInt(fList.Panel.height / itemSize.y);

        if (isPlus)
        {
            index += screenViewItemX;
        }
        else
        {
            index -= screenViewItemX;
        }
        index *= screenViewItemY;

        fList.SpringSetFocus(index, isImmediate: true);
    }

    public void SelectSlotTableId(int selectSlotTableId)
    {
        _selectSlotTableId = selectSlotTableId;

        Renewal();
    }

    public void OnClick_UpsizeChatBtn()
    {
        chatObj.SetActive(true);
        macroObj.SetActive(false);
        chatStampFList.gameObject.SetActive(false);
        chatWordFList.gameObject.SetActive(false);

        UIAni.Play(_aninamelist[2]);

        Renewal();

        chatInput.value = string.Empty;
    }

    public void OnClick_DownsizeChatBtn()
    {
        UIAni.Play(_aninamelist[3]);
        chatObj.SetActive(false);
    }

    public void OnClick_ShopBtn()
    {

    }

    public void OnClick_AttendanceBtn()
    {
        System.DateTime now = GameSupport.GetCurrentServerTime().Date;
        System.DateTime last = GameInfo.Instance.UserData.CircleAttendance.LastCheckDate.Date;
        System.TimeSpan diff = now - last;

        if (1 <= diff.Days)
        {
            GameInfo.Instance.Send_ReqCircleAttendance(OnNet_CircleAttendance);
        }
        else
        {
            UIArenaRewardListPopup arenaRewardListPopup = LobbyUIManager.Instance.GetUI<UIArenaRewardListPopup>("ArenaRewardListPopup");
            if (arenaRewardListPopup == null)
            {
                return;
            }
            arenaRewardListPopup.SetRewardType(eArenaRewardListPopupType.CircleAttendance);
            arenaRewardListPopup.SetUIActive(true);
        }
    }

    public void OnClick_PersonalBuffBtn()
    {

    }

    public void OnClick_LeftArrowBtn()
    {
        if (chatStampFList.gameObject.activeSelf)
        {
            MoveFListItem(false, ref chatStampFList);
        }

        if (chatWordFList.gameObject.activeSelf)
        {
            MoveFListItem(false, ref chatWordFList);
        }
    }

    public void OnClick_RightArrowBtn()
    {
        if (chatStampFList.gameObject.activeSelf)
        {
            MoveFListItem(true, ref chatStampFList);
        }

        if (chatWordFList.gameObject.activeSelf)
        {
            MoveFListItem(true, ref chatWordFList);
        }
    }

    public void OnClick_ChatSettingBtn()
    {
        UICircleMessagePopup circleMessagePopup = LobbyUIManager.Instance.GetUI<UICircleMessagePopup>();
        if (circleMessagePopup == null)
        {
            return;
        }

        circleMessagePopup.SetData(eCircleSequenceType.ChatAlramSet);
        circleMessagePopup.SetUIActive(true);
    }

    public void OnClick_ChatWordsBtn()
    {
        _selectSlotTableId = 0;

        chatStampFList.gameObject.SetActive(false);
        chatWordFList.gameObject.SetActive(!chatWordFList.gameObject.activeSelf);

        macroObj.SetActive(chatWordFList.gameObject.activeSelf);

        Renewal();
    }

    public void OnClick_ChatStampBtn()
    {
        _selectSlotTableId = 0;

        chatStampFList.gameObject.SetActive(!chatStampFList.gameObject.activeSelf);
        chatWordFList.gameObject.SetActive(false);

        macroObj.SetActive(chatStampFList.gameObject.activeSelf);

        Renewal();
    }

    public void OnClick_ChatSendBtn()
    {
        if (string.IsNullOrEmpty(chatInput.value))
        {
            MessagePopup.OK(eTEXTID.OK, "채팅이 비어 있습니다.", null); // Test - LeeSeungJin - Change String
            return;
        }

        System.DateTime nowTime = GameSupport.GetCurrentServerTime();
        System.TimeSpan timeSpan = nowTime - _lastChatSendTime;
        if (timeSpan.TotalSeconds < GameInfo.Instance.GameConfig.ChatOverlapTime)
        {
            if (GameInfo.Instance.GameConfig.ChatOverlapCount <= _lastChatOverlapCount)
            {
                MessagePopup.OK(eTEXTID.OK, "바른 채팅 문화를 위해서 도배는 삼가해 주시기 바랍니다.", null); // Test - LeeSeungJin - Change String
                return;
            }

            ++_lastChatOverlapCount;
        }
        else
        {
            _lastChatSendTime = nowTime;
            _lastChatOverlapCount = 1;
        }

        string sendChatStr = Utility.CommandToStar(chatInput.value);
        if (0 < _selectSlotTableId)
        {
            sendChatStr = string.Empty;
        }

        GameInfo.Instance.Send_ReqCircleChatSend(sendChatStr, _selectSlotTableId, null);

        _selectSlotTableId = 0;
        macroObj.SetActive(false);
    }

    public void OnClick_InfoBtn()
    {
        UICircleLobbyPanel circleLobbyPanel = LobbyUIManager.Instance.GetUI<UICircleLobbyPanel>("CircleLobbyPanel");
        if (circleLobbyPanel == null)
        {
            return;
        }
        circleLobbyPanel.SetActivePanel(eCircleLobbyPanelType.Setup);
    }

    private void OnNet_CircleAttendance(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        UIArenaRewardListPopup arenaRewardListPopup = LobbyUIManager.Instance.GetUI<UIArenaRewardListPopup>("ArenaRewardListPopup");
        if (arenaRewardListPopup == null)
        {
            return;
        }
        arenaRewardListPopup.SetRewardType(eArenaRewardListPopupType.CircleAttendance, true);
        arenaRewardListPopup.SetUIActive(true);
    }

    private void OnEventChatUpdate(int index, GameObject obj)
    {
        UICircleChatListSlot slot = obj.GetComponent<UICircleChatListSlot>();
        if (slot == null)
        {
            return;
        }

        if (slot.ParentGO == null)
        {
            slot.ParentGO = this.gameObject;
        }

        bool isPlayer = false;
        CircleChatData circleChatData = null;
        if (0 <= index && index < GameInfo.Instance.CircleChatList.Count)
        {
            circleChatData = GameInfo.Instance.CircleChatList[index];
            isPlayer = circleChatData.Uid.Equals(GameInfo.Instance.UserData.UUID);
        }

        slot.UpdateSlot(index, circleChatData, isPlayer);
    }

    private int OnEventChatCount()
    {
        return GameInfo.Instance.CircleChatList.Count;
    }

    private void OnEventChatMiniUpdate(int index, GameObject obj)
    {
        UIMiniChatSlot slot = obj.GetComponent<UIMiniChatSlot>();
        if (slot == null)
        {
            return;
        }

        if (slot.ParentGO == null)
        {
            slot.ParentGO = this.gameObject;
        }

        int chatIndex = -1;
        int circleChatCount = GameInfo.Instance.CircleChatList.Count;
        CircleChatData circleChatData = null;
        switch (index)
        {
            case 0:
                {
                    if (2 <= circleChatCount)
                    {
                        chatIndex = circleChatCount - 2;
                    }
                    else if (1 <= circleChatCount)
                    {
                        chatIndex = circleChatCount - 1;
                    }
                }
                break;
            case 1:
                {
                    if (2 <= circleChatCount)
                    {
                        chatIndex = circleChatCount - 1;
                    }
                }
                break;
        }

        if (0 <= chatIndex)
        {
            circleChatData = GameInfo.Instance.CircleChatList[chatIndex];
        }

        slot.UpdateSlot(circleChatData);
    }

    private int OnEventChatMiniCount()
    {
        return 2; // Test - LeeSeungJin - Temp
    }

    private void OnEventWordUpdate(int index, GameObject obj)
    {
        UICircleWordListSlot slot = obj.GetComponent<UICircleWordListSlot>();
        if (slot == null)
        {
            return;
        }

        if (slot.ParentGO == null)
        {
            slot.ParentGO = this.gameObject;
        }

        GameClientTable.ChatWords.Param chatWordsParam = null;
        if (0 <= index && index < GameInfo.Instance.GameClientTable.ChatWordss.Count)
        {
            chatWordsParam = GameInfo.Instance.GameClientTable.ChatWordss[index];
        }

        slot.UpdateSlot(index, chatWordsParam, _selectSlotTableId);
    }

    private int OnEventWordCount()
    {
        return GameInfo.Instance.GameClientTable.ChatWordss.Count;
    }

    private void OnEventStampUpdate(int index, GameObject obj)
    {
        UICircleStampListSlot slot = obj.GetComponent<UICircleStampListSlot>();
        if (slot == null)
        {
            return;
        }

        if (slot.ParentGO == null)
        {
            slot.ParentGO = this.gameObject;
        }

        GameTable.ChatStamp.Param chatStampParam = null;
        if (0 <= index && index < GameInfo.Instance.GameTable.ChatStamps.Count)
        {
            chatStampParam = GameInfo.Instance.GameTable.ChatStamps[index];
        }

        slot.UpdateSlot(index, chatStampParam, _selectSlotTableId);
    }

    private int OnEventStampCount()
    {
        return GameInfo.Instance.GameTable.ChatStamps.Count;
    }
}
