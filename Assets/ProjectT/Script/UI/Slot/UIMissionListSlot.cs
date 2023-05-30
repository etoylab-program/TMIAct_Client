using UnityEngine;
using System.Collections;


public class UIMissionListSlot : FSlot 
{
	public UIRewardListSlot kRewardListSlot;

    public UILabel kTitleLabel;
	public UILabel kDescLabel;

    public UIButton kReciveBtn;

	public UILabel kcountLabel;

    public GameObject m_goComplate;
    
    private int m_slotIndex = 0;

    private GameTable.DailyMission.Param DailyMissionParam = null;

    public void UpdateSlot(int index, GameTable.Random.Param reward,eMISSIONTYPE missionType,uint serverCount,uint MaxCount)    //Fill parameter if you need
    {
        m_slotIndex = index;

        RewardData rewardData = new RewardData(reward.ProductType,
                                               reward.ProductIndex,
                                               reward.ProductValue);

        //  Ÿ��Ʋ
        FLocalizeString.SetLabel(kTitleLabel, (int)eTEXTID.WEEKLY_MISSION_TEXT_START + (int)missionType, MaxCount);
        //  ����
        FLocalizeString.SetLabel(kDescLabel, GameSupport.GetProductName(rewardData));
        //  �̼� Ƚ��
        FLocalizeString.SetLabel(kcountLabel, 218, MaxCount - serverCount, MaxCount);
        //  ���� ������
        kRewardListSlot.UpdateSlot(rewardData, true);

        //  ���� ���� ����
        m_goComplate.gameObject.SetActive(false);
        kReciveBtn.gameObject.SetActive(false);
        if (serverCount == 0)
        {
            if(GameSupport.IsComplateMissionRecive(GameInfo.Instance.WeekMissionData.fMissionRewardFlag, index) == true)
            {
                m_goComplate.gameObject.SetActive(true);
                kcountLabel.gameObject.SetActive(false);
            }
            else
                kReciveBtn.gameObject.SetActive(true);
        }
    }

    public void UpdateSlot(int index, GameTable.DailyMission.Param param)
    {
        m_slotIndex = index;
        DailyMissionParam = param;

        var randomParam = GameInfo.Instance.GameTable.Randoms.Find(x => x.GroupID == DailyMissionParam.RewardGroupID);
        if (randomParam == null)
            return;
        DailyMissionData.Piece p = GameInfo.Instance.DailyMissionData.Infos.Find(x => x.GroupID == DailyMissionParam.GroupID && x.Day == DailyMissionParam.Day);
        if (p == null)
            return;

        RewardData rewardData = new RewardData(randomParam.ProductType,
                                               randomParam.ProductIndex,
                                               randomParam.ProductValue);

        //  Ÿ��Ʋ        
        string strFormat = FLocalizeString.Instance.GetText(param.Desc);
        if(strFormat.Contains("{0}"))
        {
            FLocalizeString.SetLabel(kTitleLabel, string.Format(strFormat, DailyMissionParam.MissionValue));
        }
        else
        {
            FLocalizeString.SetLabel(kTitleLabel, strFormat);
        }   
        //  ����
        FLocalizeString.SetLabel(kDescLabel, GameSupport.GetProductName(rewardData));
        //  �̼� Ƚ��
        FLocalizeString.SetLabel(kcountLabel, 218, DailyMissionParam.MissionValue - p.NoVal[DailyMissionParam.No], DailyMissionParam.MissionValue);
        //  ���� ������
        

        if (GameSupport.GetCurrentDailyEventDay((eEventTarget)DailyMissionParam.GroupID) >= p.Day)
        {
            kRewardListSlot.UpdateSlot(rewardData, true);
        }
        else
        {
            kRewardListSlot.UpdateSlot(rewardData, true, true);
        }

        //  ���� ���� ����
        m_goComplate.gameObject.SetActive(false);
        kReciveBtn.gameObject.SetActive(false);
        if (p.NoVal[DailyMissionParam.No] == 0)
        {
            if (GameSupport.IsComplateMissionRecive(p.RwdFlag, index) == true)
            {
                m_goComplate.gameObject.SetActive(true);
                kcountLabel.gameObject.SetActive(false);
            }
            else
                kReciveBtn.gameObject.SetActive(true);
        }

        

    }
 
	public void OnClick_Slot()
	{
        //  ������� ����
	}
 

	/// <summary>
    ///  �ޱ� ��ư
    /// </summary>
	public void OnClick_ReciveBtn()
	{
        if (DailyMissionParam == null)
        {
            UIWeeklyMissionPopup ui = LobbyUIManager.Instance.GetActiveUI<UIWeeklyMissionPopup>("WeeklyMissionPopup");
            if (ui != null)
            {
                ui.ReciveWeekQuestReward(m_slotIndex);
            }
        }
        else
        {
            UIUserWelcomeEventPopup ui = LobbyUIManager.Instance.GetActiveUI<UIUserWelcomeEventPopup>("UserWelcomeEventPopup");
            if (ui != null)
            {
                ui.Send_ReqRewardDailyMission(DailyMissionParam);
            }
        }
	}
}
