using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIEventmodeExchangeStoryPopup : FComponent
{
    [System.Serializable]
    public class sEventStoryOpenCondition
    {
        public int ItemRewardStepIndex;
        public int ItemIndexID;
    }

    [System.Serializable]
    public class EventStoryValues
    {
        public int                              EventID;
        public List<sEventStoryOpenCondition>   ListOpenCondition;
        public int                              ItemString;
        public int                              ScenarioGroupID;
    }


	public UIButton kCloseBtn;
	[SerializeField] private FList _EventmodeExchangeStoryListInstance;

    [Header("Event Story Values")]
    public List<EventStoryValues> kEventStoryList = new List<EventStoryValues>();

    private List<EventStoryValues> _eventStoryListWithEventID = new List<EventStoryValues>();

    private int _eventId = 0;

	public override void Awake()
	{
		base.Awake();

		if(this._EventmodeExchangeStoryListInstance == null) return;
        this._EventmodeExchangeStoryListInstance.InitBottomFixing();
		this._EventmodeExchangeStoryListInstance.EventUpdate = this._UpdateEventmodeExchangeStoryListSlot;
		this._EventmodeExchangeStoryListInstance.EventGetItemCount = this._GetEventmodeExchangeStoryElementCount;
	}
 
	public override void OnEnable()
	{
		_eventId = (int)UIValue.Instance.GetValue( UIValue.EParamType.EventID );
		_eventStoryListWithEventID.Clear();
		_eventStoryListWithEventID = kEventStoryList.FindAll( x => x.EventID == _eventId );

		base.OnEnable();
	}
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        _EventmodeExchangeStoryListInstance.UpdateList();
    }

    public override void OnUIOpen() {
        LobbyUIManager.Instance.kBlackScene.SetActive( false );
    }

    private void _UpdateEventmodeExchangeStoryListSlot(int index, GameObject slotObject)
	{
		do
		{
            UIEventmodeExchangeStoryListSlot uIEventExchangeRewardListSlot = slotObject.GetComponent<UIEventmodeExchangeStoryListSlot>();
            if (null == uIEventExchangeRewardListSlot) break;

            uIEventExchangeRewardListSlot.ParentGO = this.gameObject;
            uIEventExchangeRewardListSlot.UpdateSlot(_eventId, _eventStoryListWithEventID[index]);
        } while(false);
	}
	
	private int _GetEventmodeExchangeStoryElementCount()
	{
        if (_eventStoryListWithEventID == null || _eventStoryListWithEventID.Count <= 0)
            return 0;
        return _eventStoryListWithEventID.Count;
    }
	
	public void OnClick_CloseBtn()
	{
        OnClickClose();
	}
}
