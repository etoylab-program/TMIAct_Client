using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIEventModePopup : FComponent
{
    public UILabel kTitleLabel;
	public UIButton kCloseBtn;
    [SerializeField] private FList _EventModeSetListInstance;
    private List<EventSetData> m_eventSetDatas = new List<EventSetData>();
    public override void Awake()
	{
		base.Awake();

        if (this._EventModeSetListInstance == null) return;

        this._EventModeSetListInstance.EventUpdate = this._UpdateEventSetListSlot;
        this._EventModeSetListInstance.EventGetItemCount = this._GetEventSetElementCount;
	}
 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        m_eventSetDatas.Clear();

        for (int i = 0; i < GameInfo.Instance.EventSetDataList.Count; i++)
        {
            int state = GameSupport.GetJoinEventState(GameInfo.Instance.EventSetDataList[i].TableID);
            if (state > (int)eEventState.EventNone)
            {
                bool useEventCheck = false;
                for(int j = 0; j < m_eventSetDatas.Count; j++)
                {
                    if(m_eventSetDatas[j].TableID == GameInfo.Instance.EventSetDataList[i].TableID)
                    {
                        useEventCheck = true;
                        break;
                    }
                }
                if(!useEventCheck)
                    m_eventSetDatas.Add(GameInfo.Instance.EventSetDataList[i]);
            }
        }

        m_eventSetDatas.Sort(CompareEventDate);
        this._EventModeSetListInstance.UpdateList();
	}
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);
	}

    private void _UpdateEventSetListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIEventModeList_SlotSlot slot = slotObject.GetComponent<UIEventModeList_SlotSlot>();
            if (slot == null) break;

            slot.ParentGO = gameObject;
            slot.UpdateSlot(m_eventSetDatas[index]);
        } while (false);
    }

    private int _GetEventSetElementCount()
    {
        return m_eventSetDatas.Count;
    }

    public void OnClick_CloseBtn()
	{
        OnClickClose();
        //LobbyUIManager.Instance.HideUI("EventModePopup");
    }

    public override void OnClickClose()
    {
        base.OnClickClose();
    }

    private int CompareEventDate(EventSetData lhs, EventSetData rhs)
    {
        if(lhs == null || lhs.TableData == null || rhs == null || rhs.TableData == null)
        {
            return -1;
        }

        System.DateTime dtLhs = GameSupport.GetTimeWithString(lhs.TableData.EndTime);
        System.DateTime dtRhs = GameSupport.GetTimeWithString(rhs.TableData.EndTime);

        // 날짜도 내림차순
        if (dtLhs < dtRhs)
        {
            return 1;
        }
        else if(dtLhs > dtRhs)
        {
            return -1;
        }

        // 아이디도 내림차순
        if (lhs.TableData.EventID < rhs.TableData.EventID)
        {
            return 1;
        }
        else if(lhs.TableData.EventID > rhs.TableData.EventID)
        {
            return -1;
        }

        return 0;
    }
}
