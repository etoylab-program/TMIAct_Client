using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UISpecialModePopup : FComponent
{
    public UITexture kMinigameModeTex;

    public UILabel kModeNameLabel;
    public UILabel kModeInfoLabel;

    public GameObject m_rewardObject = null;
    public GameObject kTicketInfo;

    public UILabel kTicketLabel;
    public UILabel kInfoLabel;
    public UIRewardListSlot m_reward = null;
    private GameTable.Stage.Param m_stageInfo = null;
    private long _seletecuid = -1;


    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
        //var obj = UIValue.Instance.GetValue(UIValue.EParamType.StageCharCUID);
        //if (obj == null)
        //  애니메이션 부족으로 운전수는 아사기로 고정
        CharData chardata = GameInfo.Instance.GetMainChar();
        if (chardata == null)
            return;

        _seletecuid = chardata.CUID;//GameInfo.Instance.UserData.MainCharUID;


        //int stageID = System.Convert.ToInt32(UIValue.Instance.GetValue(UIValue.EParamType.StageID));
        int stageID = GameInfo.Instance.UserData.NextPlaySpecialModeTableID;
        m_stageInfo = GameInfo.Instance.GameTable.FindStage(stageID);

        //  특별 모드
        RewardData reward = new RewardData((int)eREWARDTYPE.GOODS, (int)eGOODSTYPE.CASH, GameInfo.Instance.GameConfig.SpecialModeRewardCash);
        m_reward.UpdateSlot(reward, false);
        m_rewardObject.gameObject.SetActive(true);


        FLocalizeString.SetLabel(kModeNameLabel, m_stageInfo.Name);     //  바이크질주
        FLocalizeString.SetLabel(kModeInfoLabel, m_stageInfo.Name + 100);     //  특별모드 관련 진행 방법 설명
        FLocalizeString.SetLabel(kInfoLabel, m_stageInfo.Name + 200);         //  특별모드 관련 2줄 설명

        string miniGameTexName = "MiniGame_" + m_stageInfo.Chapter;

        if (kMinigameModeTex.mainTexture != null)
        {
            if(kMinigameModeTex.mainTexture.name != miniGameTexName)
            {
                string miniGameTexFileName = miniGameTexName + ".png";
                Texture2D minigameTex = (Texture2D)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Minigame/" + miniGameTexFileName);
                if(minigameTex != null)
                {
                    kMinigameModeTex.mainTexture = minigameTex;
                    //kMinigameModeTex.MakePixelPerfect();
                }
            }
        }

        if (m_stageInfo.Chapter == (int)eSTAGE_SPECIAL_TYPE.BIKE)
        {
            
        }
        else
        {

        }

        //  티켓 사용 갯수 표시
        FLocalizeString.SetLabel(kTicketLabel, m_stageInfo.Ticket.ToString());

        if (m_stageInfo.Ticket == 0)
            kTicketInfo.gameObject.SetActive(false);
        else
            kTicketInfo.gameObject.SetActive(true);

        GameSupport.ShowTutorialFlag(eTutorialFlag.SPECIAL);
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);
    }

    public void OnClick_BackBtn()
    {
        OnClickClose();
    }

    public void OnClick_CancleBtn()
    {
        OnClickClose();
    }


    public void OnClick_StartBtn()
    {
        if (!GameSupport.IsCheckTicketAP(m_stageInfo.Ticket))
            return;
        if (!GameSupport.IsCheckInven())
            return;

        CharData charData = null;
        if (m_stageInfo.Chapter == (int)eSTAGE_SPECIAL_TYPE.BIKE)
        {
            charData = GameInfo.Instance.GetCharDataByTableID((int)ePlayerCharType.Asagi);
        }
        else
        {
            charData = GameInfo.Instance.GetCharDataByTableID((int)ePlayerCharType.Yukikaze);
        }

        if (charData != null)
        {
            if (GameSupport.IsEmptyInEquipMainWeapon(ePresetKind.STAGE, charData.CUID))
            {
                return;
            }
        }

        Log.Show(m_stageInfo.ID);
        GameInfo.Instance.Send_ReqStageStart(m_stageInfo.ID, _seletecuid, 0, false, false, null, OnStartSpecialMode);
    }

    /// <summary>
    ///  특별 모드씬으로 전환 시작
    /// </summary>
    private void OnStartSpecialMode(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        int stageid = (int)UIValue.Instance.GetValue(UIValue.EParamType.StageID);
        var stagedata = GameInfo.Instance.GameTable.FindStage(stageid);

        if (!GameInfo.Instance.netFlag)
            GameInfo.Instance.MaxNormalBoxCount = UnityEngine.Random.Range(stagedata.N_DropMinCnt, stagedata.N_DropMaxCnt + 1);

        //  스테이지 패킷에서 셋팅된 값을 변경합니다.
        GameInfo.Instance.SeleteCharUID = _seletecuid;
        GameInfo.Instance.SelecteStageTableId = stageid;

        UIValue.Instance.SetValue(UIValue.EParamType.LoadingType, (int)UILoadingPopup.eLoadingType.LobbyToStage);
        UIValue.Instance.SetValue(UIValue.EParamType.LoadingStage, stageid);
        LobbyUIManager.Instance.ShowUI("LoadingPopup", false);
        AppMgr.Instance.LoadScene(AppMgr.eSceneType.Stage, m_stageInfo.Scene);
    }

    public void OnClick_ChangeBtn()
    {
        LobbyUIManager.Instance.ShowUI("SpecialModeChangePopup", true);
    }
}
