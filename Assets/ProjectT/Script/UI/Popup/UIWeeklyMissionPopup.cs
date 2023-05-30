using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIWeeklyMissionPopup : FComponent
{
    public enum ReceiveType
    {
        None,   //  받지 않은 상태
        Recive, //  받을 수 있는 상태
        Get,    //  받은 상태
    }

    [System.Serializable]
    public class MissionCountReward
    {
        public UIButton m_btn;
        public GameObject m_goOn;
        public GameObject m_goOff;
        public GameObject m_goDis;
        public UILabel m_rewardCountOnLabel;
        public UILabel m_rewardCountOffLabel;
        public UILabel m_rewardCountDisLabel;
    }

    public UILabel kWeeklyMissionDescLabel;

    public List<MissionCountReward> m_misstionCountRewards = new List<MissionCountReward>();

    public UISprite kgaugeSpr;
    public UILabel kcountLabel;
    public UIButton kAllreciveBtn;
    public UISprite kAllreciveDisSpr;
    [SerializeField] private FList _MissionListInstance;

    private GameTable.WeeklyMissionSet.Param m_weekQuestInfo = null;
    private int[] m_missionCount = { 2, 5, 7 };

    private List<eMISSIONTYPE> m_weekQuestType = new List<eMISSIONTYPE>();
    private List<uint> m_weekQuestCntRemain = new List<uint>();
    private List<uint> m_weekQuestCntMax = new List<uint>();
    private List<GameTable.Random.Param> m_weekQuestRewards = new List<GameTable.Random.Param>();

    private int m_missionComplateCount = 0;

    public override void Awake()
    {
        base.Awake();

        if (this._MissionListInstance == null) return;

        this._MissionListInstance.EventUpdate = this._UpdateMissionListSlot;
        this._MissionListInstance.EventGetItemCount = this._GetMissionElementCount;
        this._MissionListInstance.InitBottomFixing();
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        kWeeklyMissionDescLabel.textlocalize = GameSupport.GetEndTime(GameSupport.GetCuttentWeekEndTime());

        m_weekQuestType.Clear();
        m_weekQuestCntRemain.Clear();
        m_weekQuestCntMax.Clear();

        //  해당 주간 미션 테이블 정보 
        m_weekQuestInfo = GameInfo.Instance.GameTable.FindWeeklyMissionSet((int)GameInfo.Instance.WeekMissionData.fWeekMissionSetID);
        m_weekQuestRewards = GameInfo.Instance.GameTable.FindAllRandom(a => a.GroupID == m_weekQuestInfo.RewardGroupID);

        //  미션 설명
        {
            m_weekQuestType.Add((eMISSIONTYPE)GameSupport.CompareMissionString(m_weekQuestInfo.WMCon0));
            m_weekQuestType.Add((eMISSIONTYPE)GameSupport.CompareMissionString(m_weekQuestInfo.WMCon1));
            m_weekQuestType.Add((eMISSIONTYPE)GameSupport.CompareMissionString(m_weekQuestInfo.WMCon2));
            m_weekQuestType.Add((eMISSIONTYPE)GameSupport.CompareMissionString(m_weekQuestInfo.WMCon3));
            m_weekQuestType.Add((eMISSIONTYPE)GameSupport.CompareMissionString(m_weekQuestInfo.WMCon4));
            m_weekQuestType.Add((eMISSIONTYPE)GameSupport.CompareMissionString(m_weekQuestInfo.WMCon5));
            m_weekQuestType.Add((eMISSIONTYPE)GameSupport.CompareMissionString(m_weekQuestInfo.WMCon6));
        }

        //  남은 미션 횟수
        {
            m_weekQuestCntRemain.AddRange(GameInfo.Instance.WeekMissionData.fMissionRemainCntSlot);
        }

        //  맥스 횟수
        {
            m_weekQuestCntMax.Add((uint)m_weekQuestInfo.WMCnt0);
            m_weekQuestCntMax.Add((uint)m_weekQuestInfo.WMCnt1);
            m_weekQuestCntMax.Add((uint)m_weekQuestInfo.WMCnt2);
            m_weekQuestCntMax.Add((uint)m_weekQuestInfo.WMCnt3);
            m_weekQuestCntMax.Add((uint)m_weekQuestInfo.WMCnt4);
            m_weekQuestCntMax.Add((uint)m_weekQuestInfo.WMCnt5);
            m_weekQuestCntMax.Add((uint)m_weekQuestInfo.WMCnt6);
        }

        //  상단 진행도 게이지
        SetWeeklyMissionProgress();
        //  상단 보상 버튼
        SetMissionCountReward();

        this._MissionListInstance.UpdateList();

        bool brecv = false;
        for (int i = 0; i < m_weekQuestCntRemain.Count; i++)
        {
            if (m_weekQuestCntRemain[i] == 0)
            {
                if (GameSupport.IsComplateMissionRecive(GameInfo.Instance.WeekMissionData.fMissionRewardFlag, i) == false)
                    brecv = true;
            }
        }

        for (int i = 0; i < m_misstionCountRewards.Count; i++)
            if( m_misstionCountRewards[i].m_goOn.activeSelf  )
                brecv = true;

        if(brecv)
        {
            kAllreciveDisSpr.gameObject.SetActive(false);
        }
        else
        {
            kAllreciveDisSpr.gameObject.SetActive(true);
        }
        
    }



    private void _UpdateMissionListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIMissionListSlot wMissionSlot = slotObject.GetComponent<UIMissionListSlot>();
            if (wMissionSlot == null) break;
            wMissionSlot.ParentGO = gameObject;

            wMissionSlot.UpdateSlot(index,
                                    m_weekQuestRewards[index],
                                    m_weekQuestType[index],
                                    m_weekQuestCntRemain[index],
                                    m_weekQuestCntMax[index]);
        } while (false);
    }

    private int _GetMissionElementCount()
    {
        return (int)eCOUNT.WEEKMISSIONCOUNT; //TempValue
    }

    public void OnClick_BackBtn()
    {
        //NotificationManager.Instance.Init();
        OnClickClose();
    }
    /// <summary>
    ///  상단 버튼 이벤트
    /// </summary>
    public void OnClick_ComplateReward(int num)
    {
        int count = num - 1;
        if (m_misstionCountRewards[count].m_goOn.activeSelf == true)
        {
            //  마지막보상은 특별 보상 메세지 팝업을 사용
            ReciveWeekQuestReward((int)eCOUNT.WEEKMISSIONCOUNT + count);
        }
    }

    public void OnClick_ComplateAllreciveBtn()
    {
        if (kAllreciveDisSpr.gameObject.activeSelf)
            return;

        List<int> list = new List<int>();
        for (int i = 0; i < m_weekQuestCntRemain.Count; i++)
        {
            if (m_weekQuestCntRemain[i] == 0)
            {
                if (GameSupport.IsComplateMissionRecive(GameInfo.Instance.WeekMissionData.fMissionRewardFlag, i) == false)
                    list.Add(i);
            }
        }
        for (int i = 0; i < m_misstionCountRewards.Count; i++)
            if (m_misstionCountRewards[i].m_goOn.activeSelf)
                list.Add((int)eCOUNT.WEEKMISSIONCOUNT + i);

        
        if( list.Count != 0 )
            GameInfo.Instance.Send_ReqRewardWeekMission(list, OnRefreshPopup);
    }
    /// <summary>
    ///  상단 보상 아이콘 셋팅
    /// </summary>
    private void SetMissionCountReward()
    {
        var param = GameInfo.Instance.GameTable.FindAllRandom(a => a.GroupID == m_weekQuestInfo.RewardGroupID);
        for (int i = 0; i < m_misstionCountRewards.Count; i++)
        {
            //  미션 클리어 횟수 보상은 7~9까지
            int index = (int)eCOUNT.WEEKMISSIONCOUNT + i;
            RewardData rewardData = new RewardData(param[index].ProductType,
                                                   param[index].ProductIndex,
                                                   param[index].ProductValue);

            //  상단 보상 수령 여부
            bool isRecived = GameSupport.IsComplateMissionRecive(GameInfo.Instance.WeekMissionData.fMissionRewardFlag, index);

            //  미션 완료 갯수 체크
            //  해당 보상 수령 여부로 상태 변경
            if (m_missionComplateCount >= m_missionCount[i])
            {
                if (isRecived == true)
                {
                    SetMissionCountRreward(i, rewardData, ReceiveType.Get);
                }
                else
                {
                    SetMissionCountRreward(i, rewardData, ReceiveType.Recive);
                }
            }
            else
            {
                SetMissionCountRreward(i, rewardData, ReceiveType.None);
            }
        }
    }

    /// <summary>
    ///  미션 완료된 갯수를 게이지바에 표시
    /// </summary>
    private void SetWeeklyMissionProgress()
    {
        if (kgaugeSpr != null)
        {
            int complateCount = 0;
            for (int i = 0; i < (int)eCOUNT.WEEKMISSIONCOUNT; i++)
            {
                //  하단 스크롤 수령 체크
                if (GameSupport.IsComplateMissionRecive(GameInfo.Instance.WeekMissionData.fMissionRewardFlag, i))
                    complateCount++;
            }

            //  게이지 바
            kgaugeSpr.fillAmount = (float)complateCount / (int)eCOUNT.WEEKMISSIONCOUNT;
            //  게이지 갯수
            FLocalizeString.SetLabel(kcountLabel, 218, complateCount, (int)eCOUNT.WEEKMISSIONCOUNT);
            //  완료된 미션 갯수
            m_missionComplateCount = complateCount;
        }
    }

    /// <summary>
    ///  상단 게이지의 보상 아이콘 셋팅
    /// </summary>
    private void SetMissionCountRreward(int index, RewardData rewardData, ReceiveType type)
    {
        m_misstionCountRewards[index].m_rewardCountOnLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.STACK_COUNT_TXT), rewardData.Value);
        m_misstionCountRewards[index].m_rewardCountOffLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.STACK_COUNT_TXT), rewardData.Value);
        m_misstionCountRewards[index].m_rewardCountDisLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.STACK_COUNT_TXT), rewardData.Value);

        m_misstionCountRewards[index].m_goOn.SetActive(false);
        m_misstionCountRewards[index].m_goOff.SetActive(false);
        m_misstionCountRewards[index].m_goDis.SetActive(false);

        if (type == ReceiveType.Recive)
        {
            m_misstionCountRewards[index].m_goOn.SetActive(true);
            m_misstionCountRewards[index].m_btn.enabled = true;
        }
        else if (type == ReceiveType.Get)
        {
            m_misstionCountRewards[index].m_goOff.SetActive(true);
            m_misstionCountRewards[index].m_btn.enabled = false;
        }
        else
        {
            m_misstionCountRewards[index].m_goDis.SetActive(true);
            m_misstionCountRewards[index].m_btn.enabled = false;
        }
    }

    /// <summary>
    ///  이미지 셋팅,활성화
    /// </summary>
    private void SetActiveSprite(UISprite sprite, bool bActive, string spriteName)
    {
        if (sprite != null)
        {
            sprite.gameObject.SetActive(bActive);

            if (string.IsNullOrEmpty(spriteName) == false)
                sprite.spriteName = spriteName;
        }

    }

    /// <summary>
    ///  텍스쳐 셋팅,활성화
    /// </summary>
    private void SetActiveTexture(UITexture texture, bool bActive, string texturePath)
    {
        if (texture != null)
        {
            texture.gameObject.SetActive(bActive);

            if (string.IsNullOrEmpty(texturePath) == false)
                texture.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", texturePath);
        }
    }

    public void ReciveWeekQuestReward(int index)
    {
        List<int> list = new List<int>();
        list.Add(index);
        GameInfo.Instance.Send_ReqRewardWeekMission(list, OnRefreshPopup);
    }

    public void OnRefreshPopup(int result, PktMsgType pktmsg)
    {
        //  보상 획득 메세지
        string title = FLocalizeString.Instance.GetText(1272);
        string desc = FLocalizeString.Instance.GetText(1273);

        if( GameInfo.Instance.RewardList.Count == 1 )
            MessageRewardSpecialPopup.RewardSpecialMessage(title, desc, GameInfo.Instance.RewardList[0]);
        else
            MessageRewardListPopup.RewardListMessage(title, desc, GameInfo.Instance.RewardList);

        /*
        if (m_isSpecialReward == true)
        {
            MessageRewardSpecialPopup.RewardSpecialMessage(title, desc, GameInfo.Instance.RewardList[0]);
        }
        else
        {
            MessageRewardListPopup.RewardListMessage(title, desc, GameInfo.Instance.RewardList);
        }
        */
        Renewal(true);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            GameInfo.Instance.WeekMissionData.fMissionRemainCntSlot[1] = 0;
            Renewal(true);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            GameInfo.Instance.WeekMissionData.fMissionRemainCntSlot[2] = 0;
            Renewal(true);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            GameInfo.Instance.WeekMissionData.fMissionRemainCntSlot[3] = 0;
            Renewal(true);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            GameInfo.Instance.WeekMissionData.fMissionRemainCntSlot[4] = 0;
            Renewal(true);
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            GameInfo.Instance.WeekMissionData.fMissionRemainCntSlot[5] = 0;
            Renewal(true);
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            GameInfo.Instance.WeekMissionData.fMissionRemainCntSlot[6] = 0;
            Renewal(true);
        }
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            GameInfo.Instance.WeekMissionData.fMissionRemainCntSlot[0] = 0;
            Renewal(true);
        }
    }
   
}
