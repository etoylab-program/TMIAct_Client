using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIBingoEvent : FComponent
{
    [Header("UIBingoEvent")]
    [SerializeField] private FList rewardList = null;
    [SerializeField] private GameObject bingoRootObj = null;
    [SerializeField] private GameObject clearLineRootObj = null;
    [SerializeField] private UILabel currentBingoNoLabel = null;
    [SerializeField] private GameObject allGetButton_On = null;
    [SerializeField] private GameObject allGetButton_Off = null;

    [Header("Title")]
    [SerializeField] private GameObject titleObj = null;
    [SerializeField] private UITexture titleTex = null;
    [SerializeField] private UILabel titleLabel = null;
    [SerializeField] private UILabel dateLabel = null;

    private int _clearFlag = -1;
    private int _receiveCount = -1;
    private int _listFocus = -1;
    private EventData _eventData;
    private GameTable.BingoEvent.Param _bingoEvent;

    private List<GameTable.Random.Param> _randomList = new List<GameTable.Random.Param>();
    private List<UIBingoListSlot> _bingoList = new List<UIBingoListSlot>();
    private List<UISprite> _clearLineList = new List<UISprite>();
    private Dictionary<int, List<int>> _dicClearLineCheck = new Dictionary<int, List<int>>();

    private bool isAllGet;

    public override void Awake()
    {
        base.Awake();

        rewardList.EventUpdate = RewardUpdate;
        rewardList.EventGetItemCount = RewardCount;
        rewardList.InitBottomFixing();
        rewardList.UpdateList();

        _bingoList.AddRange(bingoRootObj.GetComponentsInChildren<UIBingoListSlot>());
        _clearLineList.AddRange(clearLineRootObj.GetComponentsInChildren<UISprite>());
    }

    public override void OnEnable()
    {
        if (_eventData == null)
        {
            return;
        }

        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
        base.InitComponent();

        _clearFlag = PlayerPrefs.GetInt($"event_bingo_clear_flag_{_eventData.GroupID}_{GameInfo.Instance.UserData.UUID}", 0);

        DataSetting();
        CheckClearLine();
        CheckReceiveCount();
    }

    public override void Renewal(bool bChildren = false)
    {
        base.Renewal(bChildren);

        if (_eventData != null)
        {
            currentBingoNoLabel.textlocalize = FLocalizeString.Instance.GetText(1830, _eventData.No);
        }

        if (_bingoEvent != null)
        {
            titleLabel.textlocalize = FLocalizeString.Instance.GetText(_bingoEvent.Name);
            dateLabel.textlocalize = FLocalizeString.Instance.GetText(303, GameSupport.GetTimeWithString(_bingoEvent.EndTime, true).ToString("yyyy.MM.dd. HH:mm"));
        }

        isAllGet = GetRewardableList().Count != 0;
        if (allGetButton_On != null) {
            allGetButton_On.SetActive(isAllGet);
            allGetButton_Off.SetActive(!isAllGet);
        }

        rewardList.RefreshNotMoveAllItem();
        rewardList.SpringSetFocus(_listFocus);
    }

    public override void OnDisable()
    {
        base.OnDisable();

        StopAllCoroutines();
        titleObj.SetActive(false);
    }

    public void SetData(int groupId)
    {
        _eventData = GameInfo.Instance.GetEventData((int)eLobbyEventType.Bingo, groupId);
    }

    public void OnClick_RewardInfoBtn()
    {
        UIEventmodeStoryResetGachaPopup eventmodeStoryResetGachaPopup = LobbyUIManager.Instance.GetUI<UIEventmodeStoryResetGachaPopup>();
        if (eventmodeStoryResetGachaPopup == null)
        {
            return;
        }

        eventmodeStoryResetGachaPopup.SetBingoData(_eventData.GroupID, _eventData.No, _receiveCount);
        eventmodeStoryResetGachaPopup.SetUIActive(true);
    }

    public void OnClick_ResetBtn()
    {
        for (int i = 0; i < _randomList.Count; i++)
        {
            if ((_eventData.RwdFlag & (1 << i)) == 0)
            {
                MessagePopup.OK(eTEXTID.OK, FLocalizeString.Instance.GetText(3291), null);
                return;
            }
        }

        UIBingoResetPopup bingoResetPopup = LobbyUIManager.Instance.GetUI<UIBingoResetPopup>();
        if (bingoResetPopup == null)
        {
            return;
        }

        GameTable.BingoEventData.Param bingoEventData = GameInfo.Instance.GameTable.FindBingoEventData(x => x.GroupID == _eventData.GroupID && x.No == _eventData.No + 1);
        if (bingoEventData == null)
        {
            bingoEventData = GameInfo.Instance.GameTable.FindAllBingoEventData(x => x.GroupID == _eventData.GroupID).LastOrDefault();
            if (bingoEventData == null)
            {
                return;
            }
        }

        bingoResetPopup.SetData(bingoEventData.OpenCost, () => { GameInfo.Instance.Send_ReqBingoNextOpen(_eventData.GroupID, OnNet_BingoReset); });
        bingoResetPopup.SetUIActive(true);
    }

    public void OnNet_BingoReset(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        _clearFlag = 0;

        SetData(_eventData.GroupID);

        DataSetting();
        CheckClearLine();
        CheckReceiveCount();
        Renewal();

        LobbyUIManager.Instance.Renewal("TopPanel");
    }

    public void RewardRecvSlot(int randomParamValue) {
        if (GameSupport.GetTimeWithString(_bingoEvent.EndTime, true) < GameSupport.GetCurrentServerTime()) {
            MessagePopup.OK(eTEXTID.OK, FLocalizeString.Instance.GetText(3290), () => LobbyUIManager.Instance.SetPanelType(ePANELTYPE.MAIN));
            return;
        }

        List<int> temp = new List<int>() { randomParamValue };
        GameInfo.Instance.Send_ReqBingoEventReward(_eventData.GroupID, _eventData.No, temp, OnNet_BingoRecvReward);
    }

    public void OnClick_AllGet()
    {
        if (isAllGet == false)
            return;

        if (GameSupport.GetTimeWithString(_bingoEvent.EndTime, true) < GameSupport.GetCurrentServerTime())
        {
            MessagePopup.OK(eTEXTID.OK, FLocalizeString.Instance.GetText(3290), () => LobbyUIManager.Instance.SetPanelType(ePANELTYPE.MAIN));
            return;
        }

        //받을 수 있는 보상 리스트
        List<int> rewardList = GetRewardableList();
        GameInfo.Instance.Send_ReqBingoEventReward(_eventData.GroupID, _eventData.No, rewardList, OnNet_BingoRecvReward);
    }

    public void OnNet_BingoRecvReward(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        if (0 < GameInfo.Instance.RewardList.Count)
        {
            MessageRewardListPopup.RewardListMessage(FLocalizeString.Instance.GetText(1262), FLocalizeString.Instance.GetText(1263), GameInfo.Instance.RewardList);
        }

        CheckReceiveCount();
        Renewal();

        LobbyUIManager.Instance.Renewal("TopPanel");
    }

    private void RewardUpdate(int index, GameObject obj)
    {
        UIBingoRewardSlot bingoRewardSlot = obj.GetComponent<UIBingoRewardSlot>();
        if (bingoRewardSlot == null)
        {
            return;
        }

        if (bingoRewardSlot.ParentGO == null)
        {
            bingoRewardSlot.ParentGO = this.gameObject;
        }

        GameTable.Random.Param randomParam = null;
        bool isComplete = false;
        bool isReceive = false;
        if (0 <= index && index < _randomList.Count)
        {
            randomParam = _randomList[index];
            isComplete = (_eventData.RwdFlag & (1 << index)) != 0;

            if (index <= 0)
            {
                isReceive = true;

                for (int i = 1; i < _randomList.Count; i++)
                {
                    if ((_eventData.RwdFlag & (1 << i)) == 0)
                    {
                        isReceive = false;
                        break;
                    }
                }
            }
            else
            {
                isReceive = index <= _receiveCount;
            }
        }

        bingoRewardSlot.UpdateSlot(index, randomParam, isComplete, isReceive, _receiveCount);
    }

    private int RewardCount()
    {
        return _randomList.Count;
    }

    private void DataSetting()
    {
        _randomList.Clear();

        _bingoEvent = GameInfo.Instance.GameTable.FindBingoEvent(_eventData.GroupID);
        if (_bingoEvent == null)
        {
            return;
        }

        titleObj.SetActive(true);
        titleTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("ui", $"UI/UITexture/Event/{_bingoEvent.Image}.png");

        GameTable.BingoEventData.Param bingoEventData = GameInfo.Instance.GameTable.FindBingoEventData(x => x.GroupID == _eventData.GroupID && x.No == _eventData.No);
        if (bingoEventData == null)
        {
            bingoEventData = GameInfo.Instance.GameTable.FindAllBingoEventData(x => x.GroupID == _eventData.GroupID).LastOrDefault();
            if (bingoEventData == null) {
                return;
            }
        }

        _randomList = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == bingoEventData.RewardGroupID);

        UpdateBingoList(bingoEventData);

        for (int i = 0; i < _clearLineList.Count; i++)
        {
            bool isClear = (_clearFlag & (1 << i)) != 0;
            _clearLineList[i].color = Color.white;
            _clearLineList[i].fillAmount = isClear ? 1 : 0;

            int clearNumber = i + 1;
            string clearStr = (string)bingoEventData.GetType().GetField($"Clear{clearNumber}").GetValue(bingoEventData);
            string[] splits = clearStr.Split(',');
            foreach (string split in splits)
            {
                int.TryParse(split, out int result);
                if (_dicClearLineCheck.ContainsKey(clearNumber))
                {
                    _dicClearLineCheck[clearNumber].Add(result);
                }
                else
                {
                    _dicClearLineCheck.Add(clearNumber, new List<int>() { result });
                }
            }
        }
    }

    private void UpdateBingoList(GameTable.BingoEventData.Param bingoEventData = null)
    {
        if (bingoEventData == null)
        {
            bingoEventData = GameInfo.Instance.GameTable.FindBingoEventData(x => x.GroupID == _eventData.GroupID && x.No == _eventData.No);
            if (bingoEventData == null)
            {
                bingoEventData = GameInfo.Instance.GameTable.FindAllBingoEventData(x => x.GroupID == _eventData.GroupID).LastOrDefault();
                if (bingoEventData == null) {
                    return;
                }
            }
        }

        for (int i = 0; i < _bingoList.Count; i++)
        {
            int itemId = (int)bingoEventData.GetType().GetField($"ItemID{i + 1}").GetValue(bingoEventData);

            GameTable.Item.Param itemParam = GameInfo.Instance.GameTable.FindItem(itemId);
            if (itemParam == null)
            {
                continue;
            }

            ItemData itemData = GameInfo.Instance.ItemList.Find(x => x.TableID == itemId);
            int invenItemCount = itemData == null ? 0 : itemData.Count;
            int itemCount = (int)bingoEventData.GetType().GetField($"ItemCount{i + 1}").GetValue(bingoEventData);
            bool bingoClear = invenItemCount >= itemCount;

            _bingoList[i].UpdateSlot(itemParam, bingoClear);
            _bingoList[i].SetCountLabel(
                FLocalizeString.Instance.GetText(
                    bingoClear ? (int)eTEXTID.GREEN_TEXT_COLOR : (int)eTEXTID.RED_TEXT_COLOR, $"{invenItemCount} / {itemCount}"));
        }
    }

    private void CheckReceiveCount()
    {
        _listFocus = 0;
        _receiveCount = 0;

        bool isSetFocus = true;
        for (int i = 0; i < _clearLineList.Count; i++)
        {
            if ((_clearFlag & (1 << i)) != 0)
            {
                ++_receiveCount;
            }

            if (isSetFocus)
            {
                int rewardIndex = i + 1;
                if ((_eventData.RwdFlag & (1 << rewardIndex)) != 0)
                {
                    _listFocus = rewardIndex + 1;
                }
                else
                {
                    isSetFocus = false;
                }
            }
        }

        if (_clearLineList.Count < _listFocus)
        {
            _listFocus = 0;
        }
    }

    private void CheckClearLine()
    {
        for (int i = 0; i < _clearLineList.Count; i++)
        {
            if ((_clearFlag & (1 << i)) != 0)
            {
                continue;
            }

            int clearNumber = i + 1;
            if (_dicClearLineCheck.ContainsKey(clearNumber) == false)
            {
                continue;
            }

            bool isClearLine = true;
            foreach (int bingoNumber in _dicClearLineCheck[clearNumber])
            {
                int bingoIndex = bingoNumber - 1;
                if (_bingoList.Count <= bingoIndex)
                {
                    continue;
                }

                if (_bingoList[bingoIndex].IsClear == false)
                {
                    isClearLine = false;
                    break;
                }
            }

            _clearLineList[i].fillAmount = 0;

            if (isClearLine)
            {
                _clearFlag |= (1 << i);

                StartCoroutine(nameof(DirectorClearLine), i);
            }
        }

        PlayerPrefs.SetInt($"event_bingo_clear_flag_{_eventData.GroupID}_{GameInfo.Instance.UserData.UUID}", _clearFlag);
    }

    private IEnumerator DirectorClearLine(int clearLineIndex)
    {
        if (_clearLineList.Count <= clearLineIndex)
        {
            yield break;
        }

        UISprite fillSprite = _clearLineList[clearLineIndex];
        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (fillSprite.fillAmount < 1)
        {
            fillSprite.fillAmount += UnityEngine.Time.fixedDeltaTime * 2;

            yield return waitForFixedUpdate;
        }
    }

    private List<int> GetRewardableList() {
        //받을 수 있는 보상 리스트- 첫 번째(올클리어) 보상 제외
        bool isGetAllReward = true;
        List<int> rewardList = new List<int>();
        for (int i = 1; i < _randomList.Count; i++) {
            //이미 받은 보상인지
            if ( _eventData != null && ( _eventData.RwdFlag & (1 << i)) == 0) {
                //하나라도 보상을 받은 적이 없다면 false
                isGetAllReward = false;

                //빙고 줄을 완료 해서 받을 수 있는 건지?
                if (i <= _receiveCount) {
                    rewardList.Add(_randomList[i].Value);
                }
            }
        }

        if (isGetAllReward == true) {
            if ( _eventData != null && ( _eventData.RwdFlag & (1 << 0)) == 0) {
                rewardList.Add(_randomList[0].Value);
            }
        }

        Debug.LogWarning($"보상 ID 리스트");
        for (int i = 0; i < rewardList.Count; i++) {
            Debug.LogWarning($"index : {i}  ID : {rewardList[i]}");
        }
        Debug.LogWarning($"======================");

        return rewardList;
    }
}
