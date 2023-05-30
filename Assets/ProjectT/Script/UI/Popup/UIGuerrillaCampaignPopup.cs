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
        public int GuerrillaType;   // 0 캠페인, 1 미션
        public DateTime StartDate;  //시작일시
        public DateTime EndDate;    //종료일시
        public string Type;         // 타입
        public int Name;
        public int Desc;
        public int Condition;       // 조건 값
        public int Count;           // 필요 횟수
        public int GroupID;         // 그룹ID
        public int GroupOrder;      // 그룹 내 순서
        public int RewardType;      // 보상 타입
        public int RewardIndex;     // 보상 인덱스
        public int RewardValue;     // 보상 수량

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

            //게릴라 미션 시간 체크 및 로그인 제외
            if (!GameSupport.IsGuerrillaMission(missionData))
                continue;

            GllaMissionData userMissionData = GameInfo.Instance.GllaMissionList.Find(x => x.GroupID == missionData.GroupID);
            if(userMissionData == null)     //테이블에는 존재 아직 아무 행동을 하지 않아서 서버에서 받은 데이터가 없을 때
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
        //팝업 닫을때 Noti 체크
        NotificationManager.Instance.CheckNotification(NotificationManager.eTYPE.CAMPAIGN);
        base.OnClickClose();
    }
}
