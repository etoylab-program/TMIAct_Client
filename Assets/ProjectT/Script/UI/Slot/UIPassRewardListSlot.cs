using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIPassRewardListSlot : FSlot
{
    public enum ePassRewardState
    {
        NONE,
        Reward_Complete,        //보상 이미 받음
        Reward_Use,             //보상 받을수 있음
        Reward_After            //아직 보상 받기전
    }


	public UILabel kPointLabel;
	public UISprite kReward1GetStampSpr;
	public GameObject kReward1EFF;
	public UISprite kSPRewardBGSpr;
	public UISprite kSPGetStampSpr;
	public GameObject kSPRewardEFF;
	public GameObject kSPLocked;

    public UIRewardListSlot kRewardListSlot;
    public UIRewardListSlot kSPRewardListSlot;

    public List<GameObject> kPointObjList;
    public UISprite kPointBgDisSpr;

    private UIPassMissionPopup.PassRewardData _rewardData;
    private ePassRewardState _passSP_RewardState = ePassRewardState.NONE;
    private ePassRewardState _passN_RewardState = ePassRewardState.NONE;

    private RewardData _resultRewardData = null;

    private int _passId;

    public void UpdateSlot(int passId, int passValue, UIPassMissionPopup.PassRewardData data, ePassRewardState Nstate, ePassRewardState SPstate, bool useSPItem) 	//Fill parameter if you need
	{
        _rewardData = data;
        _passId = passId;

        int index = _passId - 1;
        for (int i = 0; i < kPointObjList.Count; i++)
        {
            kPointObjList[i].SetActive(i == index);
        }

        DefaultSetting();
        
        string labelStr = _rewardData.PassPoint.ToString();
        if (passId == (int)ePassSystemType.Story)
        {
            GameTable.Stage.Param param = GameInfo.Instance.GameTable.FindStage(x => x.ID == _rewardData.PassPoint && x.StageType == (int)eSTAGETYPE.STAGE_MAIN_STORY);
            if (param != null)
            {
                labelStr = param.Chapter.ToString();
            }
        }

        kPointLabel.textlocalize = labelStr;
        kRewardListSlot.ParentGO = kSPRewardListSlot.ParentGO = this.gameObject;
        kPointBgDisSpr.SetActive(passValue < data.PassPoint);

        _passN_RewardState = Nstate;
        _passSP_RewardState = SPstate;
        
        GameTable.Random.Param nParam = _rewardData.NormalRewardTableData;
        if (nParam == null)
        {
            kRewardListSlot.gameObject.SetActive(false);
            _passN_RewardState = ePassRewardState.NONE;
        }
        else
        {

            kRewardListSlot.gameObject.SetActive(true);
            kRewardListSlot.UpdateSlot(new RewardData(nParam.ProductType, nParam.ProductIndex, nParam.ProductValue), true);

            switch (_passN_RewardState)
            {
                case ePassRewardState.Reward_Complete:
                    kReward1GetStampSpr.gameObject.SetActive(true);
                    break;
                case ePassRewardState.Reward_Use:
                    kReward1EFF.gameObject.SetActive(true);
                    break;
                case ePassRewardState.Reward_After:
                    kRewardListSlot.UpdateSlot(new RewardData(nParam.ProductType, nParam.ProductIndex, nParam.ProductValue), true, true);
                    break;
            }
            
        }

        GameTable.Random.Param sParam = _rewardData.SpecialRewardTableData;
        if(sParam == null)
        {
            kSPRewardListSlot.gameObject.SetActive(false);
            _passSP_RewardState = ePassRewardState.NONE;
        }
        else
        {
            kSPRewardListSlot.gameObject.SetActive(true);
            kSPRewardListSlot.UpdateSlot(new RewardData(sParam.ProductType, sParam.ProductIndex, sParam.ProductValue), true);

            if (!useSPItem)
                kSPLocked.SetActive(true);
            else
            {
                switch (_passSP_RewardState)
                {
                    case ePassRewardState.Reward_Complete:
                        kSPGetStampSpr.gameObject.SetActive(true);
                        break;
                    case ePassRewardState.Reward_Use:
                        kSPRewardEFF.gameObject.SetActive(true);
                        break;
                    case ePassRewardState.Reward_After:
                        kSPRewardListSlot.UpdateSlot(new RewardData(sParam.ProductType, sParam.ProductIndex, sParam.ProductValue), true, true);
                        break;
                }
            }
        }
	}

    private void DefaultSetting()
    {
        kReward1GetStampSpr.gameObject.SetActive(false);
        kSPGetStampSpr.gameObject.SetActive(false);

        kReward1EFF.SetActive(false);
        kSPRewardEFF.SetActive(false);

        kSPLocked.SetActive(false);
    }

	public void OnClick_Slot(RewardData rewarddata, UIRewardListSlot slot)
	{
        if (slot.Equals(kRewardListSlot))
        {
            Log.Show("kRewardListSlot", Log.ColorType.Red);
            if (_passN_RewardState == ePassRewardState.Reward_Use)
            {
                _resultRewardData = rewarddata;
                GameInfo.Instance.Send_ReqRewardPass(_passId, _rewardData.PassPoint, 0, OnNet_AckRewardPass);
            }
            else
            {
                RewardInfoPopup(rewarddata);
            }
        }
        else
        {
            Log.Show("kSPRewardListSlot");
            if(_passSP_RewardState == ePassRewardState.Reward_Use)
            {
                _resultRewardData = rewarddata;
                GameInfo.Instance.Send_ReqRewardPass(_passId, 0, _rewardData.PassPoint, OnNet_AckRewardPass);
            }
            else
            {
                RewardInfoPopup(rewarddata);
            }
        }
    }

    public void OnNet_AckRewardPass(int result, PktMsgType pktMsg)
    {
        if (result != 0)
            return;

        List<RewardData> rewardlist = new List<RewardData>();
        rewardlist.Add(_resultRewardData);
        MessageRewardListPopup.RewardListMessage(FLocalizeString.Instance.GetText((int)eTEXTID.REWARD_POPUP_TITLE), FLocalizeString.Instance.GetText((int)eTEXTID.REWARD_POPUP_TEXT_MAIL), rewardlist);
        _resultRewardData = null;
        LobbyUIManager.Instance.Renewal("PassMissionPopup");
    }

    private void RewardInfoPopup(RewardData rewardData)
    {
        if (rewardData.Type == (int)eREWARDTYPE.GOODS)
            return;

        GameSupport.OpenRewardTableDataInfoPopup(rewardData);
    }
}
