
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class UIUserWelcomeEventPopup : FComponent
{
   
    [Header("Top Controls")]    
    public FList kUserWelcomeTopList;
    public UITexture kBannerTex;
    public UIUserWelcomeTopListSlot CompleteRewardSlot;

    [Header("Left Controls")]
    public FList    kUserWelcomeLeftList;
    public UILabel  LbDate;

    [Header("Center Controls")]
    public FList kUserWelcomeMissionList;

    public FTab kEventSelectTab;
    
    private DailyMissionData _dailyMissionData = null;
    private List<GameTable.DailyMissionSet.Param> _dailyMissionSetList = new List<GameTable.DailyMissionSet.Param>();
    private List<List<DailyMissionData.Piece>> _dailyMissionDataPieces = new List<List<DailyMissionData.Piece>>();
    private List<GameTable.DailyMission.Param> _dailyMissionParams = new List<GameTable.DailyMission.Param>();
    private eEventTarget _eventTarget = eEventTarget.NONE;
    [SerializeField]private int _SelectDay = 0;
    public int SelectDay {  get { return _SelectDay; } }

    private int _selMissionDataPiecesIndex;
    private List<DailyMissionData.Piece> _selMissionDataList;

    private int             mCurDay     = 0;
    private bool            mbEnd       = false;
    private float           mCheckTime  = 0.0f;
    private StringBuilder   mSb         = new StringBuilder();


    public override void Awake()
    {
        base.Awake();
        
        kUserWelcomeTopList.EventUpdate = _UpdateWelcomeTopList;
        kUserWelcomeTopList.EventGetItemCount = _GetWelcomeTopListElementCount;

        kUserWelcomeLeftList.EventUpdate = _UpdateWelcomeLeftList;
        kUserWelcomeLeftList.EventGetItemCount = _GetWelcomeLeftListElementCount;

        kUserWelcomeMissionList.EventUpdate = _UpdateWelcomeMissionList;
        kUserWelcomeMissionList.EventGetItemCount = _GetWelcomeMissionListElementCount;

        kUserWelcomeLeftList.InitBottomFixing();
        kUserWelcomeMissionList.InitBottomFixing();

        kEventSelectTab.EventCallBack = OnTabEventSelect;
    }
    public override void OnEnable()
    {
        Lobby.Instance.SetLobbyBG(false);
        InitComponent();
        base.OnEnable();
    }
    public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		base.OnDisable();
        Lobby.Instance.SetLobbyBG(true);
    }
    public override void InitComponent()
    {
        var eventtype = UIValue.Instance.GetValue(UIValue.EParamType.DailyEventType);
        if (eventtype == null)
            return;

        _eventTarget = (eEventTarget)eventtype;

        _dailyMissionData = GameInfo.Instance.DailyMissionData;
        _dailyMissionSetList = GameInfo.Instance.GameTable.FindAllDailyMissionSet(x => x.EventTarget == (int)_eventTarget);

        if (null == _dailyMissionSetList || _dailyMissionSetList.Count <= (int)eCOUNT.NONE)
            return;

        _dailyMissionDataPieces.Clear();
        for (int i = 0; i < _dailyMissionSetList.Count; i++)
        {
            if (GameSupport.HasDailyEventPlayPossibleFlag(_dailyMissionSetList[i]) && GameSupport.HasDailyEventState(_dailyMissionSetList[i]))
            {
                List<DailyMissionData.Piece> missionDataPieces = _dailyMissionData.Infos.FindAll(x => x.GroupID == _dailyMissionSetList[i].ID);
                if (null == missionDataPieces || missionDataPieces.Count <= (int)eCOUNT.NONE)
                    continue;

                //_dailyMissionDataPieces.Add(missionDataPieces);
                _dailyMissionDataPieces.Insert(0, missionDataPieces);
            }
        }

        
        if (_dailyMissionDataPieces.Count >= 2)
        {
            kEventSelectTab.gameObject.SetActive(true);
            kEventSelectTab.SetTab((int)eCOUNT.NONE, SelectEvent.Code);
        }
        else
        {
            _selMissionDataPiecesIndex = (int)eCOUNT.NONE;
            _selMissionDataList = _dailyMissionDataPieces[_selMissionDataPiecesIndex];
            kEventSelectTab.gameObject.SetActive(false);
        }

        //_dailyMissionDataPieces = _dailyMissionData.Infos.FindAll(x => x.GroupID == (int)_eventTarget);
            Calc_SelectDay();
        Renewal_Data();
        kUserWelcomeTopList.UpdateList();
        kUserWelcomeLeftList.UpdateList();
        kUserWelcomeMissionList.UpdateList();

        mCheckTime = 0.0f;
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);
        if (!Renewal_Data())
        {
            OnClickClose();
            return;
        }

        //상단 배너, 텍스트 설정
        SetTopBannerData();

        kUserWelcomeTopList.RefreshNotMove();
        kUserWelcomeMissionList.UpdateList();
        kUserWelcomeLeftList.RefreshNotMove();
    }

    private bool Renewal_Data()
    {
        _dailyMissionData = GameInfo.Instance.DailyMissionData;
        if (_dailyMissionData == null || _dailyMissionData.Infos == null || _dailyMissionData.Infos.Count <= 0)
        {
            //신규,복귀 유저 아님            
            return false;
        }

        //n일 미션 테이블 데이터 확인
        _dailyMissionParams = GameInfo.Instance.GameTable.DailyMissions.FindAll(x => x.GroupID == _selMissionDataList[(int)eCOUNT.NONE].GroupID && x.Day == _SelectDay +1);
        return true;
    }

    private void Calc_SelectDay()
    {
        // 현재 n일 정하기        
        DateTime curTime = GameInfo.Instance.GetNetworkTime();
        for (int i = 0; i < _selMissionDataList.Count; i++)
        {
            _SelectDay = i;
            mCurDay = i;

            int result_0 = DateTime.Compare(_selMissionDataList[i].StartTime, curTime);
            int result_1 = DateTime.Compare(_selMissionDataList[i].EndTime, curTime);

            if (result_0 <= 0 && result_1 >= 0)
            {
                break;
            }
        }

        UpdateRemainTime();
    }

    private void SetTopBannerData()
    {
        kBannerTex.SetActive(true);
        string texName = "";
        if (_eventTarget == eEventTarget.WELCOME)
        {
            texName = string.Format("UI/UITexture/Event/Event_Welcome-{0}.png", FLocalizeString.Language.ToString());
        }
        else if (_eventTarget == eEventTarget.COMEBACK)
        {
            texName = string.Format("UI/UITexture/Event/Event_Comeback-{0}.png", FLocalizeString.Language.ToString());
        }
        else if (_eventTarget == eEventTarget.PUBLIC)
        {
            texName = string.Format("UI/UITexture/Event/Event_Public-{0}.png", FLocalizeString.Language.ToString());
        }
        kBannerTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("ui", texName);


        //모든 보상 완료 설정
        CompleteRewardSlot.SetActive(true);

        CompleteRewardSlot.UpdateSlot(0, _selMissionDataList[(int)eCOUNT.NONE], true);
    }

    private void _UpdateWelcomeTopList(int index, GameObject slotObject)
    {
        do
        {
            UIUserWelcomeTopListSlot slot = slotObject.GetComponent<UIUserWelcomeTopListSlot>();
            if (slot == null) return;

            DailyMissionData.Piece data = null;
            if (0 <= index && _selMissionDataList.Count > index)
                data = _selMissionDataList[index];

            slot.ParentGO = this.gameObject;
            slot.UpdateSlot(index, data);
        } while (false);
        
    }
   
    private void _UpdateWelcomeLeftList(int index, GameObject slotObject)
    {
        do
        {
            UIUserWelcomeLeftListSlot slot = slotObject.GetComponent<UIUserWelcomeLeftListSlot>();
            if (slot == null) return;

            DailyMissionData.Piece data = null;
            if (0 <= index && _selMissionDataList.Count > index)
                data = _selMissionDataList[index];

            slot.ParentGO = this.gameObject;
            slot.UpdateSlot(index, data, this);
        } while (false);
        
    }
    
    private void _UpdateWelcomeMissionList(int index, GameObject slotObject)
    {
        do
        {
            UIMissionListSlot slot = slotObject.GetComponent<UIMissionListSlot>();
            if (slot == null) return;

            GameTable.DailyMission.Param data = null;
            if (0 <= index && _dailyMissionParams.Count > index)
                data = _dailyMissionParams[index];

            slot.ParentGO = this.gameObject;
            slot.UpdateSlot(index, data);
        } while (false);
        
    }

    private int _GetWelcomeTopListElementCount()
    {
        if (_selMissionDataList == null) return 0;
        return _selMissionDataList.Count;
    }

    private int _GetWelcomeLeftListElementCount()
    {
        if (_selMissionDataList == null) return 0;
        return _selMissionDataList.Count;
    }

    private int _GetWelcomeMissionListElementCount()
    {
        if (_dailyMissionParams == null) return 0;
        return _dailyMissionParams.Count;
    }

    bool OnTabEventSelect(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Enable)
            return false;

        _selMissionDataPiecesIndex = nSelect;
        _selMissionDataList = _dailyMissionDataPieces[_selMissionDataPiecesIndex];
        Calc_SelectDay();
        Renewal_Data();

        kUserWelcomeTopList.UpdateList();
        kUserWelcomeLeftList.UpdateList();
        kUserWelcomeMissionList.UpdateList();

        Renewal(true);

        return true;
    }


    public void SetSelectDay(int index)
    {
        _SelectDay = index;
        Renewal(false);
    }


    public void Send_ReqRewardDailyMission(GameTable.DailyMission.Param param)
    {
        if (param == null) return;

        Send_ReqRewardDailyMission(param.No, param.GroupID, param.Day);
    }

    public void Send_ReqRewardDailyMission(int index, int groupid, int day)
    {
        List<int> list = new List<int>();
        list.Add(index);

        GameInfo.Instance.Send_ReqRewardDailyMission(list, groupid, day, OnNetAckRewardDailyMission);
    }

    private void OnNetAckRewardDailyMission(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        string title = FLocalizeString.Instance.GetText((int)eTEXTID.REWARD_POPUP_TITLE);
        string desc = FLocalizeString.Instance.GetText((int)eTEXTID.REWARD_POPUP_TEXT);
        MessageRewardListPopup.RewardListMessage(title, desc, GameInfo.Instance.RewardList);

        //_dailyMissionDataPieces = _dailyMissionData.Infos.FindAll(x => x.GroupID == (int)_eventTarget);
        Renewal(false);

        LobbyUIManager.Instance.Renewal("MainPanel");
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("GoodsPopup");
    }

    private void UpdateRemainTime()
    {
        if(_selMissionDataList.Count <= 0 || mCurDay < 0 || mCurDay >= _selMissionDataList.Count)
        {
            mbEnd = true;
            return;
        }

        DateTime curTime = GameInfo.Instance.GetNetworkTime();
        mbEnd = DateTime.Compare(curTime, _selMissionDataList[_selMissionDataList.Count - 1].EndTime) >= 0;

        if (mbEnd)
        {
            mbEnd = true;
            LbDate.textlocalize = FLocalizeString.Instance.GetText(1759);
        }
        else
        {
            mbEnd = false;
            string remainTime = GameSupport.GetRemainTimeString(_selMissionDataList[mCurDay].EndTime, curTime);

            mSb.Clear();
            mSb.Append(string.Format(FLocalizeString.Instance.GetText(1758), mCurDay + 1));
            mSb.Append(". ");
            mSb.Append(string.Format(FLocalizeString.Instance.GetText(1059), remainTime));

            LbDate.textlocalize = mSb.ToString();
        }
    }

    private void FixedUpdate()
    {
        if(mbEnd)
        {
            return;
        }

        mCheckTime += Time.fixedDeltaTime;
        if(mCheckTime >= 60.0f)
        {
            UpdateRemainTime();
            mCheckTime = 0.0f;           
        }
    }
}
