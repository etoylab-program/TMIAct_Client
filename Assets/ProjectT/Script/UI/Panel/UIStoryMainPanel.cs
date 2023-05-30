using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIStoryMainPanel : FComponent
{
    public UIButtonLock kDairyBtn;
    public UIButtonLock kEventBtn;
    public UIButtonLock kSpecialBtn;
    public UIButtonLock kTimeAttackBtn;

    public GameObject kEventMark;

    public UILabel kSpecialTimeLabel;
    public GameObject kSpecialOff;
    public GameObject kSpecialMark;

    public GameObject kHandUnit;

    private double m_reOpenTotalSeconds;
    private int m_reOpenTimerID = 0;


    public override void OnEnable()
    {
		UIValue.Instance.SetValue( UIValue.EParamType.CardFormationType, eCharSelectFlag.STAGE );
		base.OnEnable();
    }

    public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		base.OnDisable();

        if (m_reOpenTimerID != 0)
        {
            FGlobalTimer.Instance.RemoveTimer(m_reOpenTimerID);
            m_reOpenTimerID = 0;
        }
    }

    public override void OnUIOpen()
    {
        LobbyUIManager.Instance.kBlackScene.SetActive(false);
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        if (kHandUnit != null)
        {
            kHandUnit.gameObject.SetActive(GameSupport.ShowLobbyStageHand());
        }

        if (kEventBtn.IsLock)
        {
            kEventMark.SetActive(false);
        }
        else
        {
            if (IsCheckDoingEvent())
                kEventMark.SetActive(true);
            else
                kEventMark.SetActive(false);
        }

        if (kSpecialBtn.IsLock)
        {
            kSpecialMark.SetActive(false);
            kSpecialOff.SetActive(false);
        }
        else
        {
            m_reOpenTotalSeconds = GameSupport.GetRemainTime(GameInfo.Instance.UserData.LastPlaySpecialModeTime).TotalSeconds;
            FLocalizeString.SetLabel(kSpecialTimeLabel, GetStringReOpenTime((int)m_reOpenTotalSeconds));
            
            // 시간에 따른 버튼 막기 상태(0이 된 시점 전에 비활성화 시킴)
            bool isActive = (m_reOpenTotalSeconds > 1);
            kSpecialOff.SetActive(isActive);
            kSpecialMark.SetActive(!isActive);
            if (true == isActive && 0 == m_reOpenTimerID)
            {
                m_reOpenTimerID = FGlobalTimer.Instance.AddTimer(1, OnCheckRemainTime, true);
            }
        }
    }

    /// <summary>
    ///  등록된 타이머에 반응하는 함수
    /// </summary>
    private void OnCheckRemainTime()
    {
        m_reOpenTotalSeconds -= 1;
        FLocalizeString.SetLabel(kSpecialTimeLabel, GetStringReOpenTime((int)m_reOpenTotalSeconds));

        //  시간이 남지 않았을시 막기 비활성화 및 시간체크 지우기
        if (m_reOpenTotalSeconds <= 1)
        {
            kSpecialOff.SetActive(false);
            kSpecialMark.SetActive(true);
            FGlobalTimer.Instance.RemoveTimer(m_reOpenTimerID);
        }
    }

    private bool IsCheckDoingEvent()
    {
        for( int i = 0; i < GameInfo.Instance.EventSetDataList.Count; i++ )
        {
            var eventdata = GameInfo.Instance.EventSetDataList[i];
            if (eventdata == null)
                continue;

            int state = GameSupport.GetJoinEventState(eventdata.TableID);
            if (state == (int)eEventState.EventPlaying)
                return true;
        }
        return false;
    }

    public void OnClick_MainStoryBtn()
    {
        int stageid = GameSupport.GetStorySuitableStageID(eSTAGETYPE.STAGE_MAIN_STORY);
        UIValue.Instance.SetValue(UIValue.EParamType.StoryStageID, stageid);
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.STORY);
    }

    public void OnClick_DairyBtn()
    {
        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.DAILY);
    }

    public void OnClick_EventBtn()
    {
        bool isEventPlay = false;
        for (int i = 0; i < GameInfo.Instance.EventSetDataList.Count; i++)
        {
            int state = GameSupport.GetJoinEventState(GameInfo.Instance.EventSetDataList[i].TableID);
            if (state > (int)eEventState.EventNone)
            {
                isEventPlay = true;
                break;
            }
        }

        if (isEventPlay == false)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3158));
            return;
        }

        LobbyUIManager.Instance.ShowUI("EventModePopup", true);
    }
    public void OnClick_SpecialBtn()
    {
        double reOpenTotalSeconds = GameSupport.GetRemainTime(GameInfo.Instance.UserData.LastPlaySpecialModeTime).TotalSeconds;
        bool isActive = (reOpenTotalSeconds > 1);

        if(isActive)
        {
            GameTable.Item.Param resetItem = GameInfo.Instance.GameTable.FindItem(x => x.Type == (int)eITEMTYPE.MATERIAL && x.SubType == (int)eITEMSUBTYPE.MATERIAL_SPECIAL_RESET);

            int resetItemCnt = GameInfo.Instance.GetItemIDCount(resetItem.ID);
            if(resetItemCnt >= GameInfo.Instance.GameConfig.SpecialModeResetItemCnt)
            {
                //시간 초기화 아이템 사용
                ItemData itemdata = GameInfo.Instance.GetItemData(resetItem.ID);

                if (itemdata != null)
                {
                    MessagePopup.CYNItem(
                    eTEXTID.OK,
                    FLocalizeString.Instance.GetText(3194),
                    eTEXTID.OK,
                    eTEXTID.CANCEL,
                    itemdata.TableID,
                    GameInfo.Instance.GameConfig.SpecialModeResetItemCnt,
                    () =>
                    {
                        GameInfo.Instance.Send_ReqUseItem(itemdata.ItemUID, GameInfo.Instance.GameConfig.SpecialModeChangeItemCnt, 0, OnAckUseItemStageSpecial);
                    },
                    null
                    );
                }
            }
            else
            {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3143));
            }
            return;
        }

        //미니게임 스테이지
        UIValue.Instance.SetValue(UIValue.EParamType.StageID, GameInfo.Instance.UserData.NextPlaySpecialModeTableID);

        LobbyUIManager.Instance.ShowUI("SpecialModePopup", true);
    }

    public void OnAckUseItemStageSpecial(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;
        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3195));
        m_reOpenTotalSeconds = 0;
        Renewal(true);
    }

    public void OnClick_TimeAttackBtn()
    {
        GameInfo.Instance.Send_ReqTimeAtkRankingList(OnNetTimeAtkRankingList);
    }

    public void OnNetTimeAtkRankingList(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        LobbyUIManager.Instance.SetPanelType(ePANELTYPE.TIMEATTACK);
    }

    /// <summary>
    ///  시 : 분 : 초 표시
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private string GetStringReOpenTime(int time)
    {
        int h = time / 3600;
        int m = (time % 3600) / 60;
        int s = time % 60;

        return string.Format("{0:00} : {1:00} : {2:00}", h, m, s);
    }

}
