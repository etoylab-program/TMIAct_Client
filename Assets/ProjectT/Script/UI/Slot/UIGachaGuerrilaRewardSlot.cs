using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIGachaGuerrilaRewardSlot : FSlot {
    [SerializeField] private GameObject _ReceiveObj;
    [SerializeField] private GameObject _ReceiveEffObj;
    [SerializeField] private UILabel _GoalLabel;
    [SerializeField] private UIRewardListSlot _RewardListSlot;

    [SerializeField] private TweenAlpha _ReceiveAlphaTween;
    [SerializeField] private TweenScale _ReceiveScaleTween;

    private GuerrillaMissionData data;
    private GllaMissionData userData;

    public void UpdateSlot(GuerrillaMissionData guerrillaMissionData, GllaMissionData gllaMissionData) {
        data = guerrillaMissionData;
        userData = gllaMissionData;

        _ReceiveObj.SetActive(false);
        _ReceiveEffObj.SetActive(false);

        _RewardListSlot.ParentGO = this.gameObject;
        _RewardListSlot.UpdateSlot(new RewardData(data.RewardType, data.RewardIndex, data.RewardValue), true);

        //kGoalSpr.spriteName = string.Format("ico_GachaGauge_{0}", _data.Count);
        _GoalLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1891), data.Count);

        if (userData != null) {

            if (userData.Count >= data.Count) {
                if (userData.Step > data.GroupOrder) {
                    //완료
                    _ReceiveObj.SetActive(true);
                }
                else if (userData.Step == data.GroupOrder) {
                    //미완
                    _ReceiveEffObj.SetActive(true);
                    //트윈 재시작
                    if (_ReceiveAlphaTween != null) 
                        _ReceiveAlphaTween.ResetToBeginning();

                    if (_ReceiveScaleTween != null)
                        _ReceiveScaleTween.ResetToBeginning();
                }
            }
        }
        else {
            Log.Show(data.GroupID + " / " + data.Count, Log.ColorType.Red);
        }
    }

    public void OnClick_Slot(RewardData rewardData) {
        //보상받기 가능한 상태인지
        if (_ReceiveEffObj.activeSelf) {
            GameInfo.Instance.Send_ReqRewardGllaMission(userData.GroupID, OnRewardGuerrillMission);
        }
        else {
            GameSupport.OpenRewardTableDataInfoPopup(rewardData);
        }
    }

    public void OnRewardGuerrillMission(int result, PktMsgType pktmsg) {
        if (result != 0)
            return;

        PktInfoMission.Guerrilla guerrilla = pktmsg as PktInfoMission.Guerrilla;

        for (int i = 0; i < guerrilla.infos_.Count; i++) {
            Log.Show("GroupID : " + guerrilla.infos_[i].groupID_);
            Log.Show("Step : " + guerrilla.infos_[i].step_);
            Log.Show("Count : " + guerrilla.infos_[i].count_);
            Log.Show("=========================================", Log.ColorType.Red);
        }

        //3122 게릴라 미션을 달성했습니다. 선물함을 확인해주세요.
        List<RewardData> rewardList = new List<RewardData>();
        rewardList.Add(new RewardData(data.RewardType, data.RewardIndex, data.RewardValue));
        MessageRewardListPopup.RewardListMessage(FLocalizeString.Instance.GetText((int)eTEXTID.REWARD_POPUP_TITLE), FLocalizeString.Instance.GetText(3122), rewardList,
            () => {
                LobbyUIManager.Instance.Renewal("GachaPanel");
            });


    }

}
