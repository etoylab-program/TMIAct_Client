using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIEventmodeStoryResetGachaPopup : FComponent
{
    public UILabel kTitleLabel;
	public UIButton kLeftBtn;
	public UIButton kRightBtn;
	[SerializeField] private FList _EventGachaRewardListInstance;
	public UIButton kCloseBtn;
	public UILabel kDiscLabel;

    private List<GameTable.EventResetReward.Param> _eventResetReward = new List<GameTable.EventResetReward.Param>();
    private List<GameTable.Random.Param> _randomList = new List<GameTable.Random.Param>();

    private int _maxstep = 0;
    private int _curstep = 0;
    private int _eventID = 0;

    private bool _isBingoEvent = false;
    private int _bingoGroupId = -1;
    private int _bingoReceiveCount = -1;

	public override void Awake()
	{
		base.Awake();

		if(this._EventGachaRewardListInstance == null) return;
		
		this._EventGachaRewardListInstance.EventUpdate = this._UpdateEventGachaRewardListSlot;
		this._EventGachaRewardListInstance.EventGetItemCount = this._GetEventGachaRewardElementCount;
	}
 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}

    public override void OnDisable()
    {
        base.OnDisable();

        _isBingoEvent = false;
    }

    public override void InitComponent()
	{
        if (_isBingoEvent)
        {
            // Empty
        }
        else
        {
            _eventID = (int)UIValue.Instance.GetValue(UIValue.EParamType.EventID);
            EventSetData eventSetData = GameInfo.Instance.GetEventSetData(_eventID);

            _maxstep = 0;

            foreach (GameTable.EventResetReward.Param er in GameInfo.Instance.GameTable.EventResetRewards)
            {
                if (_eventID != er.EventID)
                    continue;

                if (_maxstep < er.RewardStep)
                    _maxstep = er.RewardStep;
            }

            _curstep = eventSetData.RewardStep;
            if (_curstep >= _maxstep)
                _curstep = _maxstep;

            kLeftBtn.gameObject.SetActive(true);
            kRightBtn.gameObject.SetActive(true);

            kDiscLabel.textlocalize = FLocalizeString.Instance.GetText(1297, _maxstep.ToString());

            GetEventRewardTable();
        }
        
        this._EventGachaRewardListInstance.UpdateList();
        Renewal(true);
    }

    private void GetEventRewardTable()
    {
        _eventResetReward.Clear();

        _eventResetReward = GameInfo.Instance.GameTable.EventResetRewards.FindAll(x => x.EventID == _eventID && x.RewardStep == _curstep);
    }

	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);
        if (_isBingoEvent)
        {
            kTitleLabel.textlocalize = FLocalizeString.Instance.GetText(1830, _curstep);
        }
        else
        {
            kTitleLabel.textlocalize = FLocalizeString.Instance.GetText(1296, _curstep.ToString());
        }
        
        kRightBtn.gameObject.SetActive(true);
        kLeftBtn.gameObject.SetActive(true);
        kDiscLabel.gameObject.SetActive(false);
        if (_curstep >= _maxstep)
        {
            _curstep = _maxstep;
            kRightBtn.gameObject.SetActive(false);
            kDiscLabel.gameObject.SetActive(true);

        }
        if (_curstep <= 1)
        {
            _curstep = 1;
            kLeftBtn.gameObject.SetActive(false);
            kDiscLabel.gameObject.SetActive(false);
        }
    }
	
	private void _UpdateEventGachaRewardListSlot(int index, GameObject slotObject)
	{
		do
		{
            UIEventGachaRewardListSlot slot = slotObject.GetComponent<UIEventGachaRewardListSlot>();
            if (slot == null)
            {
                break;
            }
            
            if (slot.ParentGO == null)
            {
                slot.ParentGO = gameObject;
            }

            if (_isBingoEvent)
            {
                GameTable.Random.Param randomParam = null;
                if (0 <= index && index < _randomList.Count)
                {
                    randomParam = _randomList[index];
                }
                slot.UpdateSlot(randomParam, _bingoReceiveCount);
            }
            else
            {
                slot.UpdateSlot(_eventResetReward[index], index, true);
            }

        } while(false);
	}
	
	private int _GetEventGachaRewardElementCount()
	{
        if (_isBingoEvent)
        {
            return _randomList.Count;
        }
        else
        {
            return _eventResetReward.Count; //TempValue
        }
	}
	
	public void OnClick_LeftBtn()
	{
        _curstep--;
        if (_curstep <= 0)
            _curstep = 1;

        if (_isBingoEvent)
        {
            GameTable.BingoEventData.Param bingoEventData = GameInfo.Instance.GameTable.FindBingoEventData(x => x.GroupID == _bingoGroupId && x.No == _curstep);
            if (bingoEventData == null)
            {
                return;
            }

            _randomList = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == bingoEventData.RewardGroupID);
            _EventGachaRewardListInstance.RefreshAllItem();
        }
        else
        {
            GetEventRewardTable();
            this._EventGachaRewardListInstance.UpdateList();
        }

        Renewal(true);
    }
	
	public void OnClick_RightBtn()
	{
        _curstep++;
        if (_curstep >= _maxstep)
            _curstep = _maxstep;

        if (_isBingoEvent)
        {
            GameTable.BingoEventData.Param bingoEventData = GameInfo.Instance.GameTable.FindBingoEventData(x => x.GroupID == _bingoGroupId && x.No == _curstep);
            if (bingoEventData == null)
            {
                return;
            }

            _randomList = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == bingoEventData.RewardGroupID);
            _EventGachaRewardListInstance.RefreshAllItem();
        }
        else
        {
            GetEventRewardTable();
            this._EventGachaRewardListInstance.UpdateList();
        }

        Renewal(true);
    }
	
	public void OnClick_CloseBtn()
	{
        OnClickClose();
        //LobbyUIManager.Instance.HideUI("EventmodeStoryResetGachaPopup");
    }

    public override void OnClickClose()
    {
        base.OnClickClose();
    }

    public void SetBingoData(int groupId, int roundNo, int bingoReceiveCount)
    {
        GameTable.BingoEventData.Param bingoEventData = GameInfo.Instance.GameTable.FindBingoEventData(x => x.GroupID == groupId && x.No == roundNo);
        if (bingoEventData == null)
        {
            bingoEventData = GameInfo.Instance.GameTable.FindAllBingoEventData(x => x.GroupID == groupId).LastOrDefault();
            if (bingoEventData == null) {
                return;
            }
        }

        _isBingoEvent = true;
        _bingoGroupId = groupId;
        _curstep = roundNo;
        _bingoReceiveCount = bingoReceiveCount;
        _maxstep = GameInfo.Instance.GameTable.FindAllBingoEventData(x => x.GroupID == groupId).Count;

        if (_curstep > _maxstep) {
            _curstep = _maxstep;
        }

        _randomList = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == bingoEventData.RewardGroupID);
    }
}
