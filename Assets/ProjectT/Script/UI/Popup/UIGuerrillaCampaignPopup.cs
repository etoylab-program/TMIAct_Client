using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIGuerrillaCampaignPopup : FComponent
{
    public enum eGuerrillaType
    {
        Campaign = 0,
        Mission = 1,
    }

    public class GuerrillaSlotItem
    {
        public int GuerrillaType;   // 0 ķ����, 1 �̼�
        public DateTime StartDate;  //�����Ͻ�
        public DateTime EndDate;    //�����Ͻ�
        public string Type;         // Ÿ��
        public int Name;
        public int Desc;
        public int Condition;       // ���� ��
        public int Count;           // �ʿ� Ƚ��
        public int GroupID;         // �׷�ID
        public int GroupOrder;      // �׷� �� ����
        public int RewardType;      // ���� Ÿ��
        public int RewardIndex;     // ���� �ε���
        public int RewardValue;     // ���� ����

        public void SetGuerrillaCampaign(GuerrillaCampData data)
        {
            GuerrillaType = (int)eGuerrillaType.Campaign;
            StartDate = data.StartDate;
            EndDate = data.EndDate;
            Type = data.Type;
            Name = data.Name;
            Desc = data.Desc;
            Condition = data.Condition;
        }

        public void SetGuerrillaMission(GuerrillaMissionData data)
        {
            GuerrillaType = (int)eGuerrillaType.Mission;
            StartDate = data.StartDate;
            EndDate = data.EndDate;
            Type = data.Type;
            Name = data.Name;
            Desc = data.Desc;
            Condition = data.Condition;
            Count = data.Count;
            GroupID = data.GroupID;
            GroupOrder = data.GroupOrder;
            RewardType = data.RewardType;
            RewardIndex = data.RewardIndex;
            RewardValue = data.RewardValue;
        }
    }


	[SerializeField] private FList _GuerrillaCampaignListInstance;
    private List<GuerrillaSlotItem> _guerrillacamplist = new List<GuerrillaSlotItem>();

    public override void Awake()
	{
		base.Awake();

		if(this._GuerrillaCampaignListInstance == null) return;
		
		this._GuerrillaCampaignListInstance.EventUpdate = this._UpdateGuerrillaCampaignListSlot;
		this._GuerrillaCampaignListInstance.EventGetItemCount = this._GetGuerrillaCampaignElementCount;
		_GuerrillaCampaignListInstance.UpdateList();
	}
 
	public override void OnEnable()
	{
		_GuerrillaCampaignListInstance.SpringSetFocus( 0, isImmediate: true );

		InitComponent();
		base.OnEnable();
	}

    public override void InitComponent()
    {
        _guerrillacamplist.Clear();
        for (int i = 0; i < GameInfo.Instance.ServerData.GuerrillaCampList.Count; i++)
        {
            if (GameSupport.IsGuerrillaCampaign(GameInfo.Instance.ServerData.GuerrillaCampList[i]))
            {
                GuerrillaSlotItem data = new GuerrillaSlotItem();
                data.SetGuerrillaCampaign(GameInfo.Instance.ServerData.GuerrillaCampList[i]);
                _guerrillacamplist.Add(data);
            }
        }

        for (int i = GameInfo.Instance.ServerData.GuerrillaMissionList.Count - 1; i >= 0; i--)
        {
            GuerrillaMissionData missionData = GameInfo.Instance.ServerData.GuerrillaMissionList[i];

            //�Ը��� �̼� �ð� üũ �� �α��� ����
            if (!GameSupport.IsGuerrillaMission(missionData))
                continue;

            GllaMissionData userMissionData = GameInfo.Instance.GllaMissionList.Find(x => x.GroupID == missionData.GroupID);
            if(userMissionData == null)     //���̺��� ���� ���� �ƹ� �ൿ�� ���� �ʾƼ� �������� ���� �����Ͱ� ���� ��
            {
                GuerrillaSlotItem guerrillaSlotItem = _guerrillacamplist.Find(x => x.GroupID == missionData.GroupID);
                if(guerrillaSlotItem == null)
                {
                    GuerrillaSlotItem data = new GuerrillaSlotItem();
                    data.SetGuerrillaMission(missionData);
                    _guerrillacamplist.Add(data);
                }
                else
                {
                    if(guerrillaSlotItem.GroupOrder > missionData.GroupOrder)
                    {
                        guerrillaSlotItem.SetGuerrillaMission(missionData);
                    }
                }

                Log.Show(missionData.GroupID + " / " + missionData.GroupOrder + " / " + missionData.Count, Log.ColorType.Red);
            }
            else
            {
                GuerrillaMissionData guerrillaFlagData = GameInfo.Instance.ServerData.GuerrillaMissionList.Find(x => x.GroupID == userMissionData.GroupID && x.GroupOrder == userMissionData.Step);
                if(guerrillaFlagData != null)
                {
                    GuerrillaSlotItem guerrillaSlotItem = _guerrillacamplist.Find(x => x.GroupID == guerrillaFlagData.GroupID);
                    if (guerrillaSlotItem == null)
                    {
                        GuerrillaSlotItem data = new GuerrillaSlotItem();
                        data.SetGuerrillaMission(guerrillaFlagData);
                        _guerrillacamplist.Add(data);
                    }
                    else
                    {
                        if (guerrillaSlotItem.GroupOrder > guerrillaFlagData.GroupOrder)
                        {
                            guerrillaSlotItem.SetGuerrillaMission(guerrillaFlagData);
                        }
                    }

                    Log.Show(guerrillaFlagData.GroupID + " / " + guerrillaFlagData.GroupOrder + " / " + guerrillaFlagData.Count, Log.ColorType.Blue);
                }
            }
        }

        //if(_guerrillacamplist.Count == 0)
        //{
        //    LobbyUIManager.Instance.GetUI("MainPanel").Renewal(true);
        //    OnClickClose();
        //    return;
        //}

        Log.Show("_guerrillacamplist " + _guerrillacamplist.Count);
        Log.Show("GameInfo.Instance.GllaMissionList.Count " + GameInfo.Instance.GllaMissionList.Count, Log.ColorType.Red);
		_GuerrillaCampaignListInstance.RefreshNotMoveAllItem();
		if ( _GuerrillaCampaignListInstance.IsLastPosY() ) {
			_GuerrillaCampaignListInstance.SpringSetFocus( _GuerrillaCampaignListInstance.EventGetItemCount() - 1, ratio: 1 );
		}
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);
	}
 
	private void _UpdateGuerrillaCampaignListSlot(int index, GameObject slotObject)
	{
		do
		{
            UIGuerrillaCampaignListSlot slot = slotObject.GetComponent<UIGuerrillaCampaignListSlot>();
            GuerrillaSlotItem data = null;
            if (0 <= index && _guerrillacamplist.Count > index)
            {
                data = _guerrillacamplist[index];
            }
            slot.ParentGO = this.gameObject;
            slot.UpdateSlot(index, data);
        } while(false);
	}
	
	private int _GetGuerrillaCampaignElementCount()
	{
		return _guerrillacamplist.Count; //TempValue
	}

	public void OnClick_CloseBtn()
	{
        OnClickClose();
	}

    public override void OnClickClose()
    {
        //�˾� ������ Noti üũ
        NotificationManager.Instance.CheckNotification(NotificationManager.eTYPE.CAMPAIGN);
        base.OnClickClose();
    }
}
