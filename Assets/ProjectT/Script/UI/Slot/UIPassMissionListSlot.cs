using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIPassMissionListSlot : FSlot {
 

	public UIGaugeUnit kGaugeUnit;
	public UISprite kDefaultbgSpr;
	public UISprite kGetbgSpr;
	public UISprite kDisbgSpr;
	public UILabel kDescLabel;

    public GameObject kPointObj;
    public UISprite kPointicoSpr;
	public UILabel kPointLabel;

    public UIButton kGetBtn;

    public GameObject kCompleteObj;

    [Header("Lock")]
    public GameObject kLockObj;
	public UISprite kLockIcoSpr;
	public UILabel kLockLabel;

    private PassMissionData _passMissionData;

    public void UpdateSlot(PassMissionData data) 	//Fill parameter if you need
	{
        _passMissionData = data;

        kDefaultbgSpr.gameObject.SetActive(false);
        kDescLabel.gameObject.SetActive(false);
        kPointObj.SetActive(false);
        kLockObj.SetActive(false);
        kCompleteObj.SetActive(false);
        kGetbgSpr.gameObject.SetActive(false);
        kDisbgSpr.gameObject.SetActive(false);
        kGetBtn.gameObject.SetActive(false);
        kGaugeUnit.gameObject.SetActive(false);

        if (GetPassMissionStartTimeCheck())
        {
            kDescLabel.gameObject.SetActive(true);
            kPointObj.SetActive(true);
            kDescLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(_passMissionData.PassMissionTableData.Desc), _passMissionData.PassMissionTableData.MissionValue);
            kPointLabel.textlocalize = _passMissionData.PassMissionTableData.RewardPoint.ToString();

            if(_passMissionData.PassMissionState == (int)eCOUNT.NONE)
            {
                //_passMissionData.PassMissionValue이 0이면 보상을 받을수 있는 상태(미션 완료)

                //보상을 받을수 있을때
                if (_passMissionData.PassMissionValue <= (int)eCOUNT.NONE)
                {
                    kGetbgSpr.gameObject.SetActive(true);
                    kGetBtn.gameObject.SetActive(true);
                }
                else
                {
                    //아직 미션 진행중
                    kDefaultbgSpr.gameObject.SetActive(true);
                    kGaugeUnit.gameObject.SetActive(true);

                    kGaugeUnit.InitGaugeUnit((float)1f - ((float)_passMissionData.PassMissionValue / (float)_passMissionData.PassMissionTableData.MissionValue));
                    kGaugeUnit.SetText(string.Format(FLocalizeString.Instance.GetText(218), _passMissionData.PassMissionTableData.MissionValue - _passMissionData.PassMissionValue, _passMissionData.PassMissionTableData.MissionValue));
                }
            }
            else
            {
                kCompleteObj.SetActive(true);
            }
            
        }
        else
        {
            kDisbgSpr.gameObject.SetActive(true);
            kLockObj.SetActive(true);
            
            kLockLabel.textlocalize = GameSupport.GetStartTime(GameSupport.GetTimeWithString(_passMissionData.PassMissionTableData.ActiveTime));
        }

    }
 
    private bool GetPassMissionStartTimeCheck()
    {
        System.DateTime nowTime = GameSupport.GetCurrentServerTime();

        System.DateTime activeTime = GameSupport.GetTimeWithString(_passMissionData.PassMissionTableData.ActiveTime);

        if (activeTime > nowTime)
            return false;


        return true;
    }

	public void OnClick_Slot()
	{

	}
 
	public void OnClick_GetBtn()
	{
        if (ParentGO == null)
            return;

        List<int> missionidList = new List<int>();
        missionidList.Add(_passMissionData.PassMissionID);

        GameInfo.Instance.Send_ReqRewardPassMission(missionidList, OnNet_AckRewardPassMission);
    }

    public void OnNet_AckRewardPassMission(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3176), _passMissionData.PassMissionTableData.RewardPoint));

        LobbyUIManager.Instance.Renewal("PassMissionPopup");
    }
}
