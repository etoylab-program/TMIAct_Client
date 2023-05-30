using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIPassMissionPopup : FComponent
{
    public class PassRewardData
    {
        public int PassPoint;
        public GameTable.Random.Param NormalRewardTableData;
        public GameTable.Random.Param SpecialRewardTableData;

        public PassRewardData(int passPoint, GameTable.Random.Param normalData, GameTable.Random.Param specialData)
        {
            PassPoint = passPoint;
            NormalRewardTableData = normalData;
            SpecialRewardTableData = specialData;
        }
    }

    [Serializable]
    public class RewardInfoData
    {
        public bool specialBuy;
        public int passValue;
        public int normalComplete;
        public int specialComplete;
        public int normalUse;
        public int specialUse;
    }

    [Serializable]
    public class PassTypeData
    {
        public UIBannerSlot slot;
        public UIButton allReceiveBtn;
        public UIButton buyBtn;
        public GameObject header;
        public GameObject body;
    }

    enum ePassMissionTabType
    {
        REWARD = 0,
        MISSION,
    }

    [Header("PassMissionPopup")]
    public GameObject kRewardObj;
    public GameObject kMissionObj;
    public FList kPassRewardList;
	public FList kPassMissionList;
	public FTab kPassMissionTab;
    public List<PassTypeData> kPassTypeDataList;
    
    [Header("Golden Pass")]
    public UILabel kActiveDateLabel;
    public UIGaugeUnit kPassGaugeUnit;
    public UILabel kPassBtnTypeLabel;

    private readonly Dictionary<ePassSystemType, List<PassRewardData>> _dicRewardData = new Dictionary<ePassSystemType, List<PassRewardData>>();
    private readonly Dictionary<ePassSystemType, RewardInfoData> _dicRewardInfoData = new Dictionary<ePassSystemType, RewardInfoData>();
    private readonly List<RewardData> _passAllRewards = new List<RewardData>();

    private ePassMissionTabType _selectTab = ePassMissionTabType.REWARD;
    private ePassSystemType _selectPassTab = ePassSystemType.Gold;

    private List<PassMissionData> _passMissionDataList;
    private List<int> _passAllMissionIdList;

    private int _lastPassNormalValue;
    private int _lastPassSpecialValue;
    private int _passAllMissionPoint = 0;
    private int _forceOpenPopupIndex = -1;

	public override void Awake()
	{
		base.Awake();

		kPassMissionTab.EventCallBack = _OnPassMissionTabSelect;

		kPassRewardList.EventUpdate = _UpdatePassRewardListSlot;
		kPassRewardList.EventGetItemCount = _GetPassRewardElementCount;

		kPassMissionList.EventUpdate = _UpdatePassMissionListSlot;
		kPassMissionList.EventGetItemCount = _GetPassMissionElementCount;
	}
 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        _selectTab = ePassMissionTabType.REWARD;
        _passMissionDataList = GameInfo.Instance.PassMissionData.FindAll(x => x.PassID == (int)ePassSystemType.Gold);
        
        _SetRewardList(ePassSystemType.Gold);
        _SetRewardList(ePassSystemType.Story);
        _SetRewardList(ePassSystemType.Rank);
        
        int startIndex = _forceOpenPopupIndex;
        for (int i = 0; i < kPassTypeDataList.Count; i++)
        {
            if (kPassTypeDataList[i].slot.gameObject.activeSelf)
            {
                if (startIndex < 0)
                {
                    startIndex = i;
                    _selectPassTab = (ePassSystemType)startIndex + 1;
                }
            }
            
            kPassTypeDataList[i].slot.UpdateSlot(UIBannerSlot.ePosType.Mission, i, startIndex);
            kPassTypeDataList[i].header.SetActive(i == startIndex);
            kPassTypeDataList[i].body.SetActive(i == startIndex);
        }
        _forceOpenPopupIndex = -1;
        
        kRewardObj.SetActive(_selectPassTab == ePassSystemType.Gold);
        kMissionObj.SetActive(false);
        
        kPassMissionTab.gameObject.SetActive(_selectPassTab == ePassSystemType.Gold);
        kPassMissionTab.SetTab((int)_selectTab, SelectEvent.Code);

        kPassRewardList.UpdateList();
        kPassMissionList.UpdateList();
    }
 
	public override void Renewal(bool bChildren = false)
	{
		base.Renewal(bChildren);

        //레드닷 갱신
        NotificationManager.Instance.CheckNotification(NotificationManager.eTYPE.PASS);
        NotificationManager.Instance.CheckNotification(NotificationManager.eTYPE.PASS_TAB_REWARD);
        NotificationManager.Instance.CheckNotification(NotificationManager.eTYPE.PASS_TAB_MISSION);
        NotificationManager.Instance.CheckNotification(NotificationManager.eTYPE.PASS_TAB_GOLD);
        NotificationManager.Instance.CheckNotification(NotificationManager.eTYPE.PASS_TAB_STORY);
        NotificationManager.Instance.CheckNotification(NotificationManager.eTYPE.PASS_TAB_RANK);

        _passAllMissionPoint = 0;
        _passAllMissionIdList = _AllCompletePassMissionList();
        foreach (int missionId in _passAllMissionIdList)
        {
            PassMissionData data = _passMissionDataList.Find(x => x.PassMissionID == missionId);
            if (data != null)
            {
                _passAllMissionPoint += data.PassMissionTableData.RewardPoint;
            }
        }

        foreach (KeyValuePair<ePassSystemType,RewardInfoData> rewardInfoData in _dicRewardInfoData)
        {
            RewardInfoData data = rewardInfoData.Value;
            switch (rewardInfoData.Key)
            {
                case ePassSystemType.Gold:
                    data.passValue = _GetCurrentMissionPoint();
                    break;
                case ePassSystemType.Story:
                    data.passValue = _GetCurrentStagePoint();
                    break;
                case ePassSystemType.Rank:
                    data.passValue = GameInfo.Instance.UserData.Level;
                    break;
            }

            int passSetId = (int)rewardInfoData.Key;
            data.specialComplete = _GetCompletePoint(passSetId, true);
            data.normalComplete = _GetCompletePoint(passSetId);
            data.specialUse = _GetSPRewardPoint(rewardInfoData.Key);
            data.normalUse = _GetNormalRewardPoint(rewardInfoData.Key);
            data.specialBuy = GameSupport.GetUseSPItem(passSetId);
        }

        _GetAllRewardItems();

        int totalPassPoint = _GetTotalPassPoint();
        int passCurPoint = _GetPassValue(ePassSystemType.Gold);
        kPassGaugeUnit.InitGaugeUnit((float)passCurPoint / totalPassPoint);
        kPassGaugeUnit.SetText(FLocalizeString.Instance.GetText(218, passCurPoint, totalPassPoint));
        PassSetData passSetData = GameInfo.Instance.UserData.PassSetDataList.Find(x => x.PassID == (int)ePassSystemType.Gold);
        if (passSetData != null && passSetData.PassTableData != null)
        {
            kActiveDateLabel.textlocalize = GameSupport.GetEndTime(GameSupport.GetTimeWithString(passSetData.PassTableData.EndTime, true));
        }

        _SetPassMissionTab();

        int passBtnTypeId = 1509;
        switch (_selectTab)
        {
            case ePassMissionTabType.REWARD:
                {
                    int selectIndex = (int)(_selectPassTab - 1);
                    if (selectIndex < kPassTypeDataList.Count)
                    {
                        kPassTypeDataList[selectIndex].buyBtn.SetActive(!_GetPassSpecialBuy(_selectPassTab));
                    }
                    passBtnTypeId = 1257;
                }
                break;
        }

        kPassBtnTypeLabel.textlocalize = FLocalizeString.Instance.GetText(passBtnTypeId);
        kPassMissionList.RefreshNotMoveAllItem();
        kPassRewardList.RefreshNotMoveAllItem();
        
        _SetFocusRewardItem();
    }

    private int _GetPassValue(ePassSystemType type)
    {
        int result = 0;
        if (_dicRewardInfoData.ContainsKey(type))
        {
            result = _dicRewardInfoData[type].passValue;
        }
        return result;
    }

    private int _GetPassNormalComplete(ePassSystemType type)
    {
        int result = 0;
        if (_dicRewardInfoData.ContainsKey(type))
        {
            result = _dicRewardInfoData[type].normalComplete;
        }
        return result;
    }
    
    private int _GetPassSpecialComplete(ePassSystemType type)
    {
        int result = 0;
        if (_dicRewardInfoData.ContainsKey(type))
        {
            result = _dicRewardInfoData[type].specialComplete;
        }
        return result;
    }

    private bool _GetPassSpecialBuy(ePassSystemType type)
    {
        bool result = false;
        if (_dicRewardInfoData.ContainsKey(type))
        {
            result = _dicRewardInfoData[type].specialBuy;
        }
        return result;
    }

    private void _SetFocusRewardItem()
    {
        _dicRewardData.TryGetValue(_selectPassTab, out List<PassRewardData> list);
        if (list != null)
        {
            int index = 0;
            
            _dicRewardInfoData.TryGetValue(_selectPassTab, out RewardInfoData info);
            int sComplete = info?.specialComplete ?? 0;
            int nComplete = info?.normalComplete ?? 0;
            int sUse = info?.specialUse ?? 0;
            int nUse = info?.normalUse ?? 0;
            
            for (int i = 0; i < list.Count; i++)
            {
                PassRewardData data = list[i];
                if (data == null)
                {
                    continue;
                }
                
                UIPassRewardListSlot.ePassRewardState nState = _GetRewardState(data.PassPoint, nComplete, nUse);
                UIPassRewardListSlot.ePassRewardState sState = _GetRewardState(data.PassPoint, sComplete, sUse);
                
                // First
                if (nState == UIPassRewardListSlot.ePassRewardState.Reward_Use)
                {
                    index = i;
                    break;
                }
                
                // Second
                if (sState == UIPassRewardListSlot.ePassRewardState.Reward_Use)
                {
                    index = i;
                    break;
                }
                
                // Third
                if (nState == UIPassRewardListSlot.ePassRewardState.Reward_Complete)
                {
                    index = i;
                }
                else if (sState == UIPassRewardListSlot.ePassRewardState.Reward_Complete)
                {
                    index = i;
                }
            }

            kPassRewardList.SpringSetFocus(index);
        }
    }

    public void SetForcePassTab(int passSetId)
    {
        _forceOpenPopupIndex = passSetId - 1;
        _selectPassTab = (ePassSystemType)passSetId;
    }
    public void SetPassTab(int index)
    {
        ePassSystemType tempPassTab = (ePassSystemType)(index + 1);
        if (_selectPassTab == tempPassTab)
        {
            return;
        }

        _selectPassTab = tempPassTab;

        for (int i = 0; i < kPassTypeDataList.Count; i++)
        {
            kPassTypeDataList[i].slot.UpdateSlot(UIBannerSlot.ePosType.Mission, i, index);
            kPassTypeDataList[i].header.SetActive(i == index);
            kPassTypeDataList[i].body.SetActive(i == index);
        }

        kPassMissionTab.SetTab(0, SelectEvent.Code);
        kPassMissionTab.gameObject.SetActive(_selectPassTab == ePassSystemType.Gold);

        Renewal();
    }

    private void _SetAllReceiveBtn(bool state, ePassSystemType passType)
    {
        int index = (int)passType - 1;
        if (kPassTypeDataList.Count <= index)
        {
            return;
        }

        kPassTypeDataList[index].allReceiveBtn.isEnabled = state;
    }

    private void _SetPassMissionTab()
    {
        kRewardObj.SetActive(_selectTab == ePassMissionTabType.REWARD);
        kMissionObj.SetActive(_selectTab == ePassMissionTabType.MISSION);
        
        bool state = false;
        switch(_selectTab)
        {
            case ePassMissionTabType.REWARD:
            {
                state = _lastPassNormalValue > 0 || _lastPassSpecialValue > 0;
            } break;
            case ePassMissionTabType.MISSION:
            {
                state = 0 < _passAllMissionIdList.Count;
            } break;
        }
        
        _SetAllReceiveBtn(state, _selectPassTab);
    }

    private List<int> _AllCompletePassMissionList()
    {
        List<int> result = new List<int>();
        foreach (PassMissionData data in _passMissionDataList)
        {
            if(data.PassMissionState == (int)eCOUNT.NONE)
            {
                if (data.PassMissionValue == (int) eCOUNT.NONE)
                {
                    result.Add(data.PassMissionID);
                }
            }
        }
        return result;
    }

    private void _SetRewardList(ePassSystemType type)
    {
        if (!_dicRewardInfoData.ContainsKey(type))
        {
            _dicRewardInfoData.Add(type, new RewardInfoData());
        }
        
        if (!_dicRewardData.ContainsKey(type))
        {
            _dicRewardData.Add(type, new List<PassRewardData>());
        }

        List<PassRewardData> list = _dicRewardData[type];
        list.Clear();
        
        GameTable.PassSet.Param param = GameInfo.Instance.GameTable.FindPassSet(x => x.Type == (int)type);
        if (param == null)
        {
            return;
        }

        if (GameSupport.GetTimeWithString(param.EndTime, true) < GameSupport.GetCurrentServerTime())
        {
            kPassTypeDataList[(int)type - 1].slot.SetActive(false);
            return;
        }
        
        List<GameTable.Random.Param> nReward = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == param.N_RewardID);
        List<GameTable.Random.Param> sReward = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == param.S_RewardID);
        foreach (GameTable.Random.Param nData in nReward)
        {
            list.Add(new PassRewardData(nData.Value, nData, null));
        }
        foreach (GameTable.Random.Param sData in sReward)
        {
            PassRewardData data = list.Find(x => x.PassPoint == sData.Value);
            if (data != null)
            {
                data.SpecialRewardTableData = sData;
            }
            else
            {
                list.Add(new PassRewardData(sData.Value, null, sData));
            }
        }

        list.Sort(_SortList);
    }

    private int _SortList(PassRewardData a, PassRewardData b)
    {
        if (a.PassPoint > b.PassPoint)
        {
            return 1;
        }

        if (a.PassPoint < b.PassPoint)
        {
            return -1;
        }

        return 0;
    }

    private int _GetCompletePoint(int passSetId, bool special = false)
    {
        int result = 0;
        PassSetData data = GameInfo.Instance.UserData.PassSetDataList.Find(x => x.PassID == passSetId);
        if (data != null)
        {
            result = special ? data.Pass_SPReward : data.Pass_NormalReward;
        }
        return result;
    }
    //총 미션 포인트
    private int _GetTotalPassPoint()
    {
        int result = 0;
        for (int i = 0; i < _passMissionDataList.Count; i++)
        {
            result += _passMissionDataList[i].PassMissionTableData.RewardPoint;
        }
        return result;
    }

    //현재 미션 완료 포인트
    private int _GetCurrentMissionPoint()
    {
        int result = 0;
        for (int i = 0; i < _passMissionDataList.Count; i++)
        {
            if (_passMissionDataList[i].PassMissionState != 0)
            {
                result += _passMissionDataList[i].PassMissionTableData.RewardPoint;
            }
        }
        return result;
    }

    private int _GetCurrentStagePoint()
    {
        int result = 0;
        GameTable.PassSet.Param passSetParam = GameInfo.Instance.GameTable.FindPassSet(x => x.Type == (int)ePassSystemType.Story);
        if (passSetParam != null)
        {
            List<GameTable.Random.Param> randomParam = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == passSetParam.N_RewardID || x.GroupID == passSetParam.S_RewardID);
            if (randomParam != null)
            {
                foreach (GameTable.Random.Param random in randomParam)
                {
                    if (GameInfo.Instance.StageClearList.Any(x => x.TableID == random.Value))
                    {
                        if (result < random.Value)
                        {
                            result = random.Value;
                        }
                    }
                }
            }
        }
        return result;
    }

    //일반 보상 받을 수 있는 것(앞에서 하나씩만 받을수 있음)
    private int _GetNormalRewardPoint(ePassSystemType type)
    {
        int result = 0;
        GameTable.PassSet.Param passSetParam = GameInfo.Instance.GameTable.FindPassSet(x => x.Type == (int)type);
        if (passSetParam == null)
        {
            return result;
        }
        
        int passCurPoint = _GetPassValue(type);
        int normalComplete = _GetPassNormalComplete(type);
        if (normalComplete < passCurPoint)
        {
            List<GameTable.Random.Param> normalReward = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == passSetParam.N_RewardID);
            foreach (GameTable.Random.Param param in normalReward)
            {
                if (param.Value <= passCurPoint && param.Value > normalComplete)
                {
                    if (result == 0)
                    {
                        result = param.Value;
                        continue;
                    }
                    result = Mathf.Min(result, param.Value);
                }
            }
        }
        return result;
    }

    private int _GetSPRewardPoint(ePassSystemType type)
    {
        int result = 0;
        GameTable.PassSet.Param passSetParam = GameInfo.Instance.GameTable.FindPassSet(x => x.Type == (int)type);
        if (passSetParam == null)
        {
            return result;
        }
        
        int passCurPoint = _GetPassValue(type);
        if (GameSupport.GetUseSPItem((int)type))
        {
            //SP티켓 구매 체크
            int specialComplete = _GetPassSpecialComplete(type);
            if (specialComplete < passCurPoint)
            {
                List<GameTable.Random.Param> spReward = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == passSetParam.S_RewardID);
                foreach (GameTable.Random.Param param in spReward)
                {
                    if(param.Value <= passCurPoint && param.Value > specialComplete)
                    {
                        if (result == 0)
                        {
                            result = param.Value;
                            continue;
                        }
                        result = Mathf.Min(result, param.Value);
                    }
                }
            }   
        }
        return result;
    }

    private void _GetAllRewardItems()
    {
        _lastPassNormalValue = 0;
        _lastPassSpecialValue = 0;
        _passAllRewards.Clear();

        GameTable.PassSet.Param passSetParam = GameInfo.Instance.GameTable.FindPassSet(x => x.Type == (int)_selectPassTab);
        if (passSetParam == null)
        {
            return;
        }
        
        int passCurPoint = _GetPassValue(_selectPassTab);
        int normalComplete = _GetPassNormalComplete(_selectPassTab);
        if (normalComplete < passCurPoint)
        {
            List<GameTable.Random.Param> normalReward = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == passSetParam.N_RewardID);
            foreach (GameTable.Random.Param param in normalReward)
            {
                if (param.Value <= normalComplete || passCurPoint < param.Value)
                {
                    continue;
                }
                _lastPassNormalValue = Mathf.Max(_lastPassNormalValue, param.Value);
                _passAllRewards.Add(new RewardData(param.ProductType, param.ProductIndex, param.ProductValue));
            }
        }

        if (_GetPassSpecialBuy(_selectPassTab))
        {
            int specialComplete = _GetPassSpecialComplete(_selectPassTab);
            if (specialComplete < passCurPoint)
            {
                List<GameTable.Random.Param> spReward = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == passSetParam.S_RewardID);
                foreach (GameTable.Random.Param param in spReward)
                {
                    if (param.Value <= specialComplete || passCurPoint < param.Value)
                    {
                        continue;
                    }
                    _lastPassSpecialValue = Mathf.Max(_lastPassSpecialValue, param.Value);
                    _passAllRewards.Add(new RewardData(param.ProductType, param.ProductIndex, param.ProductValue));
                }
            }
        }
    }

    private UIPassRewardListSlot.ePassRewardState _GetRewardState(int point, int complete, int use)
    {
        UIPassRewardListSlot.ePassRewardState result = UIPassRewardListSlot.ePassRewardState.Reward_After;
        if (point <= complete)
        {
            result = UIPassRewardListSlot.ePassRewardState.Reward_Complete;
        }
        else if (point == use)
        {
            result = UIPassRewardListSlot.ePassRewardState.Reward_Use;
        }
        return result;
    }

    //일괄 완료 - 미션, 일광 받기 - 보상(일반, 패스 보상)
    public void OnClick_AllReceiveBtn()
	{
        switch (_selectTab)
        {
            case ePassMissionTabType.MISSION:
                {
                    if (_passAllMissionIdList.Count > 0)
                    {
                        GameInfo.Instance.Send_ReqRewardPassMission(_passAllMissionIdList, OnNet_AckRewardPassMission);
                    }
                }
                break;
            case ePassMissionTabType.REWARD:
                {
                    if (_passAllRewards.Count > 0)
                    {
                        GameInfo.Instance.Send_ReqRewardPass((int)_selectPassTab, _lastPassNormalValue, _lastPassSpecialValue, OnNet_AckRewardPass);
                    }
                }
                break;
        }
    }
	
    //패스 아이템 인앱구매
	public void OnClick_BuyBtn()
	{
        if (!IAPManager.Instance.IAPNULL_CHECK())
        {
            MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3197), null);
            return;
        }

        GameTable.PassSet.Param passSetParam = GameInfo.Instance.GameTable.FindPassSet(x => x.PassID == (int) _selectPassTab);
        if (passSetParam == null)
        {
            return;
        }

        GameTable.Store.Param passStoreItem = GameInfo.Instance.GameTable.FindStore(x => x.ID == passSetParam.PassStoreID);
        if (passStoreItem == null)
        {
            return;
        }

        //테이블에 현금으로 들어가있을시...(테스트 편하게)
        if(passStoreItem.PurchaseType == (int)eREWARDTYPE.GOODS && passStoreItem.PurchaseIndex == (int)eGOODSTYPE.GOODS)
        {
            MessagePopup.IAPMessage(
            FLocalizeString.Instance.GetText(3177),
            FLocalizeString.Instance.GetText(110, FLocalizeString.Instance.GetText(30001)),
            passStoreItem,
            OnIAPBuySuccess,
            OnIAPBuyFailed
            );
        }
        else
        {
            //TestCode
            MessagePopup.OK(eTEXTID.OK, FLocalizeString.Instance.GetText(110, FLocalizeString.Instance.GetText(30001)),
                () =>
                {
                    GameInfo.Instance.Send_ReqStorePurchase(passStoreItem.ID, false, 1, OnNet_AckRewardPassMission);
                });
        }
	}

    private void OnIAPBuySuccess()
    {
        IAPManager.Instance.IsBuying = false;
        
        //구입성공 메세지 보여주기!!
        GameTable.PassSet.Param passSetParam = GameInfo.Instance.GameTable.FindPassSet(x => x.PassID == (int) _selectPassTab);
        if (passSetParam == null)
        {
            return;
        }

        GameTable.Store.Param passStoreItem = GameInfo.Instance.GameTable.FindStore(x => x.ID == passSetParam.PassStoreID);
        if (passStoreItem == null)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3049, FLocalizeString.Instance.GetText(30001)));
        }
        else
        {
            if (passStoreItem.ProductType == (int)eREWARDTYPE.PASS)
            {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3276));
            }
        }

        Renewal();
    }

    private void OnIAPBuyFailed()
    {
        WaitPopup.Hide();
        IAPManager.Instance.IsBuying = false;
        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3212));
    }
    
    //미션 일괄 완료
    private void OnNet_AckRewardPassMission(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }
        MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3176), _passAllMissionPoint));
        Renewal();
    }

    //일반, 특별보상 일괄 받기완료
    private void OnNet_AckRewardPass(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }
        MessageRewardListPopup.RewardListMessage(FLocalizeString.Instance.GetText((int)eTEXTID.REWARD_POPUP_TITLE), FLocalizeString.Instance.GetText((int)eTEXTID.REWARD_POPUP_TEXT_MAIL), _passAllRewards);
        Renewal();
    }
    
    private bool _OnPassMissionTabSelect(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Enable)
        {
            return false;
        }

        _selectTab = (ePassMissionTabType)nSelect;
        Renewal();
        return true;
    }
    
    private void _UpdatePassRewardListSlot(int index, GameObject slotObject)
	{
        if (slotObject == null)
        {
            return;
        }
        
        UIPassRewardListSlot slot = slotObject.GetComponent<UIPassRewardListSlot>();
        if (slot == null)
        {
            return;
        }
        
        _dicRewardData.TryGetValue(_selectPassTab, out List<PassRewardData> list);
        if (list == null || index < 0 || list.Count <= index)
        {
            return;
        }
        
        PassRewardData data = list[index];
        if (data == null)
        {
            return;
        }
        
        _dicRewardInfoData.TryGetValue(_selectPassTab, out RewardInfoData info);
        int sComplete = info?.specialComplete ?? 0;
        int nComplete = info?.normalComplete ?? 0;
        int sUse = info?.specialUse ?? 0;
        int nUse = info?.normalUse ?? 0;
        int passValue = info?.passValue ?? 0;
        
        UIPassRewardListSlot.ePassRewardState nState = _GetRewardState(data.PassPoint, nComplete, nUse);
        UIPassRewardListSlot.ePassRewardState sState = _GetRewardState(data.PassPoint, sComplete, sUse);
        
        slot.ParentGO = this.gameObject;
        slot.UpdateSlot((int)_selectPassTab, passValue, data, nState, sState, _GetPassSpecialBuy(_selectPassTab));
    }

    private int _GetPassRewardElementCount()
    {
        _dicRewardData.TryGetValue(_selectPassTab, out List<PassRewardData> result);
        return result?.Count ?? 0;
    }
    
    private void _UpdatePassMissionListSlot(int index, GameObject slotObject)
    {
        if (slotObject == null)
        {
            return;
        }
        
        UIPassMissionListSlot slot = slotObject.GetComponent<UIPassMissionListSlot>();
        if (slot == null)
        {
            return;
        }

        if (_passMissionDataList.Count <= index)
        {
            return;
        }

        slot.ParentGO = this.gameObject;
        slot.UpdateSlot(_passMissionDataList[index]);
    }
	
    private int _GetPassMissionElementCount()
    {
        return _passMissionDataList.Count;
    }
}
