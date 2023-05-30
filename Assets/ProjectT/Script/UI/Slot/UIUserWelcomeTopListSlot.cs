using UnityEngine;
using System.Collections;

public class UIUserWelcomeTopListSlot : FSlot {

    [SerializeField] private UIRewardListSlot RewardSlot;
    [SerializeField] private UISprite EffSpr;
    [SerializeField] private UILabel NameLabel;
    [SerializeField] private UISprite ReceiveSpr;
    [SerializeField] private UISprite StampSpr;
    [SerializeField] private UISprite AwardSpr;
    [SerializeField] private UIButton ReceiveBtn;
    
    private enum eState
    {
        Common,
        Recv_Enable,
        Recv_Complete,
    }
    private eState State = eState.Common;
    private DailyMissionData.Piece Piece;
    private bool IsCompleteReward = false;

    public void UpdateSlot(int index, DailyMissionData.Piece piece, bool isCompleteReward = false)  //Fill parameter if you need
    {
        Piece = piece;
        IsCompleteReward = isCompleteReward;

        SetComponentState(false);

        SetState();

        RewardSlot.SetActive(true);
        var dailyMissionSet = GameInfo.Instance.GameTable.DailyMissionSets.Find(x => x.ID == piece.GroupID);
        GameTable.Random.Param randomData = null;

        if (IsCompleteReward)
            randomData = GameInfo.Instance.GameTable.Randoms.Find(x => x.GroupID == dailyMissionSet.RewardGroupID && x.Value == 0);
        else
            randomData = GameInfo.Instance.GameTable.Randoms.Find(x => x.GroupID == dailyMissionSet.RewardGroupID && x.Value == piece.Day);

        // 완성보상
        RewardData data = new RewardData(randomData.ProductType, randomData.ProductIndex, randomData.ProductValue);
        RewardSlot.UpdateSlot(data, true);


        //NameLabel.SetActive(State == eState.Common);
        if (IsCompleteReward)
        {
            //NameLabel.textlocalize = FLocalizeString.Instance.GetText(1686);
            NameLabel.SetActive(false);
            AwardSpr.SetActive(true);
        }
        else
        {
            NameLabel.SetActive(true);
            NameLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1603), piece.Day);
        }

        if (State == eState.Recv_Complete)
        {   
            StampSpr.SetActive(true);
            //AwardSpr.SetActive(true);
        }
    }

    private void SetComponentState(bool state)
    {
        RewardSlot.SetActive(state);
        EffSpr.SetActive(state);
        NameLabel.SetActive(state);
        ReceiveSpr.SetActive(state);
        StampSpr.SetActive(state);
        AwardSpr.SetActive(state);
        ReceiveBtn.SetActive(state);
    }
    private void SetState()
    {
        State = eState.Common;

        // 1. 보상을 받았는지에 대한 확인
        byte ChoiceBit = 0;
        if (IsCompleteReward)
        {
            ChoiceBit = (byte)((int)PktInfoMission.Daily.Piece.ENUM.COM_1);            
        }
        else
        {
            ChoiceBit = (byte)((int)PktInfoMission.Daily.Piece.ENUM.DAY_1);            
        }

        if (GameSupport.IsComplateMissionRecive(Piece.RwdFlag, ChoiceBit))
        {
            State = eState.Recv_Complete;
        }
        
        if (State != eState.Common)
            return;

        if (IsCompleteReward)
        {
            //모든 DAY_1 보상을 받았는지 확인
            var list = GameInfo.Instance.DailyMissionData.Infos.FindAll(x => x.GroupID == Piece.GroupID);
            if (list == null | list.Count == 0)
                return;

            bool IsAllReceived = true;
            var iter = list.GetEnumerator();
            while (iter.MoveNext())
            {   
                if (!GameSupport.IsComplateMissionRecive(iter.Current.RwdFlag, (int)PktInfoMission.Daily.Piece.ENUM.DAY_1))
                {
                    IsAllReceived = false;
                    break;
                }
            }

            if (IsAllReceived)
            {
                State = eState.Recv_Enable;
                ReceiveBtn.SetActive(true);
                ReceiveSpr.SetActive(true);
            }
        }
        else
        {
            //보상을 받을 수 있는지 확인 체크
            bool IsAllZero = true;
            for (int i = 0; i < Piece.NoVal.Length; i++)
            {
                if (Piece.NoVal[i] > 0)
                {
                    IsAllZero = false;
                    break;
                }
            }

            if (IsAllZero)
            {
                State = eState.Recv_Enable;
                ReceiveBtn.SetActive(true);
                ReceiveSpr.SetActive(true);
            }
        }
    }

    public void OnClick_Slot()
    {
        UIUserWelcomeEventPopup ui = LobbyUIManager.Instance.GetUI<UIUserWelcomeEventPopup>("UserWelcomeEventPopup");
        if (ui == null) return;

        int index = 0;
        int groupid = 0;
        int day = 0;

        if (IsCompleteReward)
        {
            index = (int)PktInfoMission.Daily.Piece.ENUM.COM_1;
            day = 1;
        }
        else
        {
            index = (int)PktInfoMission.Daily.Piece.ENUM.DAY_1;
            day = Piece.Day;
        }
        groupid = (int)Piece.GroupID;

        ui.Send_ReqRewardDailyMission(index, groupid, day);
    }
}


