using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInfluenceMainPopup : FComponent
{
    [SerializeField] private UILabel TitleLabel;
    [SerializeField] private UILabel DescLabel;

    [SerializeField] private UILabel EndDateLabel;
    [SerializeField] private UILabel RewardEndDateLabel;


    [SerializeField] private UIGaugeUnit MissionSetGuage;
    [SerializeField] private Transform MissionSetRewardUnitPrefab;
    [SerializeField] private UISprite MissionSetAllRecvEffectSpr;
    [SerializeField] private UISprite MissionSetAllRecvDiableSpr;


    [SerializeField] private UIGaugeUnit InfluenceInfoGuage;
    [SerializeField] private Transform InfluenceInfoRewardUnitPrefab;
    [SerializeField] private UISprite InfluenceInfoAllRecvEffectSpr;
    [SerializeField] private UISprite InfluenceInfoAllRecvDisableSpr;

    [SerializeField] private UIInfluenceGuageUnit[] InfluenceGuages;
    [SerializeField] private GameObject NoneInfluenceObj;
    [SerializeField] private GameObject NoneInfluenceTotalInfoObj;

    [SerializeField] private UIButton PersonalRankingBtn;
    [SerializeField] private UIButton PersonalRewardBtn;

    [SerializeField] private FList InfluenceMissionList;
    [SerializeField] private UISprite InfluenceMissionFinishSpr;
    [SerializeField] private UILabel InfluenceIcoLabel;

    private List<Transform> listMissionSetRewardUnits = new List<Transform>();
    private List<Transform> listInfluenceInfoRewardUnits = new List<Transform>();

    private int SelectGroupID = 1;
    private GameTable.InfluenceMissionSet.Param InfMissionSetTable;

    private int InfluenceInfoNo = 1;
    private GameTable.InfluenceInfo.Param InfluenceInfoTable;
    private int InfluenceInfoItemCount = 0;

    private List<GameTable.InfluenceMission.Param> InfluenceMissionTables;
    private List<int> InfluenceMissionNoOrder = new List<int>();

    public override void Awake()
    {        
        base.Awake();
        MissionSetRewardUnitPrefab.gameObject.SetActive(false);
        InfluenceInfoRewardUnitPrefab.gameObject.SetActive(false);

        if (InfluenceMissionList)
        {
            InfluenceMissionList.EventUpdate = UpdateListSlot;
            InfluenceMissionList.EventGetItemCount = GetInfluenceMissionCount;
            InfluenceMissionList.InitBottomFixing();
        }
    }

    public override void OnEnable()
    {
        Lobby.Instance.SetLobbyBG(false);
        base.OnEnable();
        
        GameTable.InfluenceMissionSet.Param influenceMissionSet = GameSupport.GetCurrentInfluenceMissionSet();

        int defaultIndex = 1643;
        if (influenceMissionSet != null)
        {
            List<GameTable.InfluenceInfo.Param> influenceInfoList = GameInfo.Instance.GameTable.FindAllInfluenceInfo(x => x.GroupID == influenceMissionSet.ID);
            if (influenceInfoList != null)
                defaultIndex = influenceInfoList.Count <= 1 ? 1724 : defaultIndex;
        }
        
        InfluenceIcoLabel.textlocalize = FLocalizeString.Instance.GetText(defaultIndex);
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

    public override void OnClose()
    {
        
        base.OnClose();
    }
    public override void InitComponent()
    { 
        ItemData item = GameInfo.Instance.ItemList.Find(x => x.TableID == GameSupport.GetCurrentInfluenceEventItemID());
        InfluenceInfoItemCount = 0;
        if (item != null) InfluenceInfoItemCount = item.Count;

        SelectGroupID = (int)GameInfo.Instance.InfluenceMissionData.GroupID;
        InfluenceInfoNo = (int)GameInfo.Instance.InfluenceMissionData.InfluID;

        Debug.Log(string.Format("Influence GroupID : {0}", GameInfo.Instance.InfluenceMissionData.GroupID));
        Debug.Log(string.Format("Influence InfluenceID : {0}", GameInfo.Instance.InfluenceMissionData.InfluID));
        Debug.Log(string.Format("Influence RwdFlag : {0}", GameInfo.Instance.InfluenceMissionData.RwdFlag));
        Debug.Log(string.Format("Influence TgtRwdFlag : {0}", GameInfo.Instance.InfluenceMissionData.TgtRwdFlag));

        InfMissionSetTable = GameInfo.Instance.GameTable.FindInfluenceMissionSet(SelectGroupID);
        InfluenceInfoTable = GameInfo.Instance.GameTable.FindInfluenceInfo(x => x.GroupID == SelectGroupID && x.No == InfluenceInfoNo);

        InfluenceMissionTables = GameInfo.Instance.GameTable.FindAllInfluenceMission(x => x.GroupID == SelectGroupID);
        if (InfluenceMissionTables != null)
        {
            InfluenceMissionTables.Sort((l, r) =>
            {
                if (l.No > r.No) return 1;
                else if (l.No < r.No) return -1;
                return 0;
            });
        }

        //InfluenceMission 데이터 순서 만들기
        InfluenceMissionNoOrder.Clear();
        {
            // bit == 0, count == 0;
            for (int i = 0; i < InfluenceMissionTables.Count; i++)
            {
                byte ChoiceBit = (byte)((int)PktInfoMission.Influ.ENUM._NO_START_ + i);
                if (!GameSupport.IsComplateMissionRecive(GameInfo.Instance.InfluenceMissionData.RwdFlag, ChoiceBit) &&
                    GameInfo.Instance.InfluenceMissionData.Val[i] == 0)
                    InfluenceMissionNoOrder.Add(InfluenceMissionTables[i].No);
            }
            // bit == 0, count > 0
            for (int i = 0; i < InfluenceMissionTables.Count; i++)
            {
                byte ChoiceBit = (byte)((int)PktInfoMission.Influ.ENUM._NO_START_ + i);
                if (!GameSupport.IsComplateMissionRecive(GameInfo.Instance.InfluenceMissionData.RwdFlag, ChoiceBit) &&
                    GameInfo.Instance.InfluenceMissionData.Val[i] > 0)
                    InfluenceMissionNoOrder.Add(InfluenceMissionTables[i].No);
            }

            // bit == 1, count == 0
            for (int i = 0; i < InfluenceMissionTables.Count; i++)
            {
                byte ChoiceBit = (byte)((int)PktInfoMission.Influ.ENUM._NO_START_ + i);
                if (GameSupport.IsComplateMissionRecive(GameInfo.Instance.InfluenceMissionData.RwdFlag, ChoiceBit) &&
                    GameInfo.Instance.InfluenceMissionData.Val[i] == 0)
                    InfluenceMissionNoOrder.Add(InfluenceMissionTables[i].No);
            }
        }

        NoneInfluenceObj.SetActive(false);
        NoneInfluenceTotalInfoObj.SetActive(false);

        if (!InfluenceMissionList.gameObject.activeSelf)
            InfluenceMissionList.gameObject.SetActive(true);

        InfluenceMissionList.UpdateList();


        MissionSetAllRecvEffectSpr.SetActive(false);
        MissionSetAllRecvDiableSpr.SetActive(false);

        InfluenceInfoAllRecvEffectSpr.SetActive(false);
        InfluenceInfoAllRecvDisableSpr.SetActive(false);

        InfluenceMissionFinishSpr.SetActive(false);
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);
        InitComponent();
        SetMissionSetGuage();
        SetInfluenceInfoGuage();
        SetInfluenceGuage();

        TitleLabel.textlocalize = FLocalizeString.Instance.GetText(InfMissionSetTable.Name);
        DescLabel.textlocalize = FLocalizeString.Instance.GetText(InfMissionSetTable.Desc);

        System.DateTime eventEndTime = GameSupport.GetTimeWithString(InfMissionSetTable.EndTime, true);
        System.DateTime rewardEndtime = GameSupport.GetTimeWithString(InfMissionSetTable.RewardTime, true);

        // InfMissionSetTable.EndTime
        EndDateLabel.textlocalize = GameSupport.GetEndTime(eventEndTime);        
        RewardEndDateLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1642), GameSupport.GetEndTime(rewardEndtime, false));

        System.TimeSpan remainTimeSpan = GameSupport.GetRemainTime(eventEndTime);
        if (remainTimeSpan.TotalSeconds <= 0)
        {
            InfluenceMissionFinishSpr.SetActive(true);
            InfluenceMissionList.gameObject.SetActive(false);
        }
    }

    public override void OnClickClose()
    {
        base.OnClickClose();

        DestroyMissionSetRewardUnit();
        DestroyInfluenceInfoRewardUnit();
    }

    private void SetMissionSetGuage()
    {
        MissionSetGuage.InitGaugeUnit(0f);
        MissionSetGuage.SetText(string.Empty);        

        List<GameTable.Random.Param> reward = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == InfMissionSetTable.RewardGroupID);
        if (reward == null || reward.Count == 0)
            return;

        reward.Sort((l, r) =>
        {
            if (l.Value > r.Value) return 1;
            else if (l.Value < r.Value) return -1;
            return 0;
        });

        float gaugeRatio = Mathf.Clamp01((float)GameInfo.Instance.InfluenceData.TotalPoint / (float)reward[reward.Count - 1].Value);
        MissionSetGuage.InitGaugeUnit(gaugeRatio);
        MissionSetGuage.SetText(string.Format("{0:#,0}({1}%)", GameInfo.Instance.InfluenceData.TotalPoint, Mathf.Floor(gaugeRatio * 100f)));

        DestroyMissionSetRewardUnit();

        //게이지의 전체 크기 얻기        
        float guageSize = MissionSetGuage.kGaugeSpr.localSize.x;
        float max = reward[reward.Count - 1].Value;
        float startX = guageSize * -0.5f;

        bool bCanReceived = false;
        for (int i = 0; i < reward.Count; i++)
        {
            Transform t = Instantiate(MissionSetRewardUnitPrefab);
            t.parent = MissionSetGuage.kGaugeSpr.transform;
            t.localScale = Vector3.one;
            t.gameObject.SetActive(true);
            t.name = i.ToString();
            float v = reward[i].Value;
            float r = v / max;
            if (r < 0f) r = 0f;
            else if (r > 1f) r = 1f;
            t.localPosition = new Vector3(startX + (guageSize * r), 0f, 0f);

            //Data설정
            UIGaugeRewardUnit unit = t.GetComponent<UIGaugeRewardUnit>();

            bool EnableBG = GameInfo.Instance.InfluenceData.TotalPoint >= (ulong)reward[i].Value;
            byte choicebit = (byte)((int)PktInfoMission.Influ.TARGET._ALL_START_ + i);
            unit?.SetData(reward[i], GameInfo.Instance.InfluenceMissionData.TgtRwdFlag, choicebit, EnableBG);
            unit?.SetCallBack(Send_InfluenceMisiionSet_Reward);

            listMissionSetRewardUnits.Add(t);

            if (!bCanReceived && EnableBG && !GameSupport.IsComplateMissionRecive(GameInfo.Instance.InfluenceMissionData.TgtRwdFlag, choicebit))
            {
                bCanReceived = true;
            }
        }

        if (bCanReceived)
        {
            MissionSetAllRecvEffectSpr.SetActive(true);
            MissionSetAllRecvDiableSpr.SetActive(false);
        }
        else
        {
            MissionSetAllRecvEffectSpr.SetActive(false);
            MissionSetAllRecvDiableSpr.SetActive(true);
        }
    }

    private void SetInfluenceInfoGuage()
    {
        InfluenceInfoGuage.InitGaugeUnit(0f);
        InfluenceInfoGuage.SetText(string.Empty);

        List<GameTable.Random.Param> reward = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == InfluenceInfoTable.RewardGroupID);
        if (reward == null || reward.Count == 0)
            return;

        reward.Sort((l, r) =>
        {
            if (l.Value > r.Value) return 1;
            else if (l.Value < r.Value) return -1;
            return 0;
        });
        
        float gaugeRatio = Mathf.Clamp01((float)InfluenceInfoItemCount / (float)reward[reward.Count - 1].Value);
        InfluenceInfoGuage.InitGaugeUnit(gaugeRatio);
        InfluenceInfoGuage.SetText(string.Format("{0:#,0}({1}%)", InfluenceInfoItemCount, Mathf.Floor(gaugeRatio * 100f)));

        DestroyInfluenceInfoRewardUnit();

        //게이지의 전체 크기 얻기        
        float guageSize = InfluenceInfoGuage.kGaugeSpr.localSize.x;
        float max = reward[reward.Count - 1].Value;
        float startX = guageSize * -0.5f;

        bool bCanReceived = false;
        for (int i = 0; i < reward.Count; i++)
        {
            Transform t = Instantiate(MissionSetRewardUnitPrefab);
            t.parent = InfluenceInfoGuage.kGaugeSpr.transform;
            t.localScale = Vector3.one;
            t.gameObject.SetActive(true);
            t.name = i.ToString();
            float v = reward[i].Value;
            float r = v / max;
            if (r < 0f) r = 0f;
            else if (r > 1f) r = 1f;
            t.localPosition = new Vector3(startX + (guageSize * r), 0f, 0f);

            //Data설정
            UIGaugeRewardUnit unit = t.GetComponent<UIGaugeRewardUnit>();

            bool EnableBG = InfluenceInfoItemCount >= reward[i].Value;
            byte choicebit = (byte)((int)PktInfoMission.Influ.TARGET._SINGLE_START_ + i);
            unit?.SetData(reward[i], GameInfo.Instance.InfluenceMissionData.TgtRwdFlag, choicebit, EnableBG);
            unit?.SetCallBack(Send_InfluenceInfo_Reward);

            listInfluenceInfoRewardUnits.Add(t);

            if (!bCanReceived && EnableBG && !GameSupport.IsComplateMissionRecive(GameInfo.Instance.InfluenceMissionData.TgtRwdFlag, choicebit))
            {
                bCanReceived = true;
            }
        }

        if (bCanReceived)
        {
            InfluenceInfoAllRecvEffectSpr.SetActive(true);
            InfluenceInfoAllRecvDisableSpr.SetActive(false);
        }
        else
        {
            InfluenceInfoAllRecvEffectSpr.SetActive(false);
            InfluenceInfoAllRecvDisableSpr.SetActive(true);
        }
    }

    private void SetInfluenceGuage()
    {
        for (int i = 0; i < InfluenceGuages.Length; i++)
        {
            InfluenceGuages[i].SetActive(false);
        }
        if (GameInfo.Instance.InfluenceData.Infos.Count == 0)
        {
            NoneInfluenceTotalInfoObj.SetActive(true);
            return;
        }

        List<InfluenceData.Piece> infos = GameInfo.Instance.InfluenceData.Infos;
        infos.Sort((l, r) =>
        {
            if (l.Point > r.Point) return -1;
            else if (l.Point < r.Point) return 1;
            return 0;
        });

        for (int i = 0; i < infos.Count; i++)
        {
            var influInfo = GameInfo.Instance.GameTable.FindInfluenceInfo(x => x.GroupID == SelectGroupID && x.No == infos[i].InfluID);
            if (influInfo == null) continue;

            string title = FLocalizeString.Instance.GetText(influInfo.Name);
            //if (string.IsNullOrEmpty(title))
            //{
            //    title = FLocalizeString.Instance.GetText(Random.Range(60110001, 60110011));
            //}
            bool IsMe = GameInfo.Instance.InfluenceMissionData.InfluID == infos[i].InfluID;
            float ratio = (float)infos[i].Point / (float)GameInfo.Instance.InfluenceData.TotalPoint;

            InfluenceGuages[i].SetActive(true);
            InfluenceGuages[i].SetData(title, ratio, IsMe);
        }
    }

    private void UpdateListSlot(int index, GameObject slotObj)
    {
        UIInfluenceMissionListSlot slot = slotObj.GetComponent<UIInfluenceMissionListSlot>();
        if (slot == null) return;

        index = InfluenceMissionNoOrder[index];
        GameTable.InfluenceMission.Param param = InfluenceMissionTables[index];
        uint serverCount = 0;
        byte choiceBit = (byte)((int)PktInfoMission.Influ.ENUM._NO_START_ + index);
        int dataIndex = (int)PktInfoMission.Influ.ENUM._NO_START_ + index;

        if (dataIndex < GameInfo.Instance.InfluenceMissionData.Val.Length)
        {
            serverCount = GameInfo.Instance.InfluenceMissionData.Val[dataIndex];
        }

        slot.UpdateSlot(param, serverCount, choiceBit);
    }
    
    private int GetInfluenceMissionCount()
    {
        if (InfluenceMissionTables == null) return 0;
        return InfluenceMissionTables.Count;
    }

    public void OnClick_RankingUI()
    {
        Debug.Log("OnClick_RankingUI");
        
        GameInfo.Instance.Send_ReqGetInfluenceRankInfo((int result, PktMsgType pktmsg) =>
        {
            if (GameInfo.Instance.InfluenceRankData.Infos.Count <= 0)
            {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3159));
                return;
            }

            LobbyUIManager.Instance.ShowUI("InfluenceRankingPopup", true);
        });
    }

    public void OnClick_RewardUI()
    {
        Debug.Log("OnClick_RewardUI");
        GameInfo.Instance.Send_ReqGetInfluenceRankInfo((int result, PktMsgType pktmsg) =>
        {
            LobbyUIManager.Instance.ShowUI("InfluenceRewardPopup", true);
        });
    }

    private List<RewardData> RewardList = new List<RewardData>();
    public void OnClick_InfluenceMissionSetAllReceive()
    {   
        List<GameTable.Random.Param> reward = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == InfMissionSetTable.RewardGroupID);
        if (reward == null || reward.Count == 0)
            return;

        reward.Sort((l, r) =>
        {
            if (l.Value > r.Value) return 1;
            else if (l.Value < r.Value) return -1;
            return 0;
        });
        List<byte> ChoiceBits = new List<byte>();
        for (int i = 0; i < reward.Count; i++)
        {
            //점수 체크
            if (GameInfo.Instance.InfluenceData.TotalPoint < (ulong)reward[i].Value)
                continue;

            byte choicebit = (byte)((int)PktInfoMission.Influ.TARGET._ALL_START_ + i);

            if(!GameSupport.IsComplateMissionRecive(GameInfo.Instance.InfluenceMissionData.TgtRwdFlag, choicebit))
            {
                ChoiceBits.Add(choicebit);

                RewardData data = new RewardData(reward[i].ProductType, reward[i].ProductIndex, reward[i].ProductValue);
                RewardList.Add(data);
            }
        }
                
        if (ChoiceBits.Count <= 0) return;

        Send_InfluenceMisiionSet_Reward(ChoiceBits);
        Debug.Log("OnClick_InfluenceMissionSetAllReceive");
    }

    
    public void OnClick_InfluenceMissionInfoAllReceive()
    {
        RewardList.Clear();

        List<GameTable.Random.Param> reward = GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == InfluenceInfoTable.RewardGroupID);
        if (reward == null || reward.Count == 0)
            return;

        reward.Sort((l, r) =>
        {
            if (l.Value > r.Value) return 1;
            else if (l.Value < r.Value) return -1;
            return 0;
        });

        List<byte> ChoiceBits = new List<byte>();
        for (int i = 0; i < reward.Count; i++)
        {
            //점수 체크
            if (InfluenceInfoItemCount < reward[i].Value)
                continue;

            byte choicebit = (byte)((int)PktInfoMission.Influ.TARGET._SINGLE_START_ + i);

            if (!GameSupport.IsComplateMissionRecive(GameInfo.Instance.InfluenceMissionData.TgtRwdFlag, choicebit))
            {
                ChoiceBits.Add(choicebit);

                RewardData data = new RewardData(reward[i].ProductType, reward[i].ProductIndex, reward[i].ProductValue);
                RewardList.Add(data);
            }
        }
        
        if (ChoiceBits.Count <= 0) return;

        Send_InfluenceInfo_Reward(ChoiceBits);
        Debug.Log("OnClick_InfluenceMissionInfoAllReceive");
    }

    #region Network_Send

    private List<byte> GetChoiceBitListAndMakeRewardLIst(GameTable.Random.Param param, byte _choiceBit)
    {
        RewardList.Clear();
        RewardList.Add(new RewardData(param.ProductType, param.ProductIndex, param.ProductValue));

        List<byte> list = new List<byte>();
        list.Add(_choiceBit);
        return list;
    }

    private void Send_InfluenceMisiionSet_Reward(GameTable.Random.Param param, byte _choiceBit)
    {        
        Send_InfluenceMisiionSet_Reward(GetChoiceBitListAndMakeRewardLIst(param,_choiceBit));
    }

    private void Send_InfluenceMisiionSet_Reward(List<byte> list)
    {
        GameInfo.Instance.Send_ReqInfluenceTgtRwd(list, (byte)eRWD_TP.ALL, OnNetAckInfluenceTgtRwd);
    }

    private void Send_InfluenceInfo_Reward(GameTable.Random.Param param, byte _choiceBit)
    {   
        Send_InfluenceInfo_Reward(GetChoiceBitListAndMakeRewardLIst(param, _choiceBit));
    }

    private void Send_InfluenceInfo_Reward(List<byte> list)
    {
        GameInfo.Instance.Send_ReqInfluenceTgtRwd(list, (byte)eRWD_TP.SINGLE, OnNetAckInfluenceTgtRwd);
    }
    #endregion

    #region Network_Recv
    private void OnNetAckInfluenceTgtRwd(int result, PktMsgType pktmsg)
    {
        Debug.Log("OnNetAckInfluenceTgtRwd");

        if (result != 0)
            return;

        if (RewardList.Count >= 0)
        {
            string title = FLocalizeString.Instance.GetText(1433);
            string desc = FLocalizeString.Instance.GetText(1262);

            MessageRewardListPopup.RewardListMessage(title, desc, RewardList, null);
            RewardList.Clear();
        }

        LobbyUIManager.Instance.Renewal("GoodsPopup");
        LobbyUIManager.Instance.Renewal("TopPanel");
        Renewal(false);
    }
    #endregion

    private void DestroyMissionSetRewardUnit()
    {
        for (int i = 0; i < listMissionSetRewardUnits.Count; i++)
        {
            DestroyImmediate(listMissionSetRewardUnits[i].gameObject);
            listMissionSetRewardUnits[i] = null;
        }
        listMissionSetRewardUnits.Clear();
    }

    private void DestroyInfluenceInfoRewardUnit()
    {
        for (int i = 0; i < listInfluenceInfoRewardUnits.Count; i++)
        {
            DestroyImmediate(listInfluenceInfoRewardUnits[i].gameObject);
            listInfluenceInfoRewardUnits[i] = null;
        }

        listInfluenceInfoRewardUnits.Clear();
    }
}
