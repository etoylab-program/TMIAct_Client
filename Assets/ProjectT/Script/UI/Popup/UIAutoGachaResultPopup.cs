using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAutoGachaResultPopup : FComponent {

    public enum eGachaType {
        ING,
        END,
    }

    [Header("- UIAutoGachaResultPopup")]
    [SerializeField] private UILabel _GachaResultLabel;
    [SerializeField] private UIButton _BackBtn;
    [SerializeField] private UIButton _StopBtn;
    [SerializeField] private UIButton _CloseBtn;
    [SerializeField] private UIButton _RewardListBtn;

    [SerializeField] private UILabel _GachaOnceCountLabel;
    [SerializeField] private UILabel _GachaRemainCountLabel;
    [SerializeField] private UIGaugeUnit _GaugeUnit;

    [Header("- ItemList")]
    [SerializeField] private TweenScale _ItemListTweenScale;
    [SerializeField] private List<UIItemListSlot> _ItemList;

    public eGachaType mGachaType { get; private set; }
    private int mMultiCount;
    private WaitForFixedUpdate mWaitForFixedUpdate = new WaitForFixedUpdate();
    private int mGachaOnceValue;

    //가챠 결과
    private List<RewardData> mRewardList = new List<RewardData>();

    //가챠패널
    private UIGachaPanel gachaPanel;

    #region Override
    public override void Awake() {
        base.Awake();

        _GaugeUnit.InitGaugeUnit(0.0f);
    }

    public override bool IsBackButton() {
        if (mGachaType == eGachaType.ING)
            return false;

        return true;
    }
    #endregion


    public void RefreshGachaResult(bool _isSkip = false) {
        SetItemList();

        if (gachaPanel == null) {
            gachaPanel = LobbyUIManager.Instance.GetActiveUI<UIGachaPanel>("GachaPanel");
        }

        if (gachaPanel.mIsAutoGachaDiretor == false) {
            _ItemListTweenScale.enabled = true;
            _ItemListTweenScale.ResetToBeginning();
            _ItemListTweenScale.PlayForward();
        }
        else {
            _ItemListTweenScale.enabled = false;
        }

        //가챠를 진행 하였다 -> 한번에 가챠되는 만큼 빼준다. 스킵시는 안빼줌
        if (_isSkip == false) {
            mMultiCount -= mGachaOnceValue;
        }

        //남은 가차 횟수가 없으면 END
        if (mMultiCount <= 0)
            mGachaType = eGachaType.END;

        //인벤토리가 꽉찼을땐 보내지 않는다.
        int now = GameSupport.GetInvenCount();
        int max = GameInfo.Instance.UserData.ItemSlotCnt;
        if (now >= max) {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3399));
            mGachaType = eGachaType.END;
        }

        _GachaRemainCountLabel.textlocalize = FLocalizeString.Instance.GetText(1443, mMultiCount);


        if (mGachaType == eGachaType.ING) {
            _GachaResultLabel.textlocalize = FLocalizeString.Instance.GetText(3396);
            _GachaOnceCountLabel.textlocalize = FLocalizeString.Instance.GetText(3359, mGachaOnceValue);
            _StopBtn.gameObject.SetActive(true);
            _CloseBtn.gameObject.SetActive(false);
            _BackBtn.gameObject.SetActive(false);
            _RewardListBtn.gameObject.SetActive(false);

            StartCoroutine( nameof(WaitGachaCooldown) );
        }
        else {
            _GachaResultLabel.textlocalize = FLocalizeString.Instance.GetText(1267);
            _GachaOnceCountLabel.textlocalize = FLocalizeString.Instance.GetText(3359, 0);
            _StopBtn.gameObject.SetActive(false);
            _CloseBtn.gameObject.SetActive(true);
            _BackBtn.gameObject.SetActive(true);
            _RewardListBtn.gameObject.SetActive(true);

            //총 획득 보상 일람 Show
            ShowGachaRewardListPopup();
        }
    }

    public void SetData(int multiCount, int gachaOnceValue) {
        mGachaType = eGachaType.ING;
        mMultiCount = multiCount;
        mGachaOnceValue = gachaOnceValue;
        mRewardList.Clear();
    }

    #region Button Function
    public void OnClick_BackBtn() {
        if (mGachaType == eGachaType.ING) {
            return;
        }

        OnClickClose();
    }

    public void OnClick_AutoStopBtn() {
        _StopBtn.isEnabled = false;

        mGachaType = eGachaType.END;
    }

    public void OnClick_GachaRewardList() {
        /*
        Debug.Log("<color=#FF9900>리워드창 Show</color>");
        for (int i = 0; i < mRewardList.Count; i++) {
            Debug.Log($"<color=#4499FF>Index = {mRewardList[i].Index}  Count = {mRewardList[i].Count}</color>");
        }
        */
        //총 획득 보상 일람 Show
        ShowGachaRewardListPopup();
    }
    #endregion

    private IEnumerator WaitGachaCooldown() {
        _StopBtn.isEnabled = true;

        float waitTimeSec = 0.0f;
        float totalTimeSec = GameInfo.Instance.GameConfig.GoldGachaAutoTimeSec;
        while (waitTimeSec < totalTimeSec) {
            waitTimeSec += Time.fixedDeltaTime;
            _GaugeUnit.InitGaugeUnit(waitTimeSec / totalTimeSec);
            yield return mWaitForFixedUpdate;
        }

        _StopBtn.isEnabled = false;

        //시간 중도에 버튼을 눌렀으면 보내지 않는다.
        if (mGachaType == eGachaType.ING) {
            //GachaPanel의 다시 가챠 하게끔 보낸다.
            UIGachaPanel panel = LobbyUIManager.Instance.GetActiveUI<UIGachaPanel>("GachaPanel");
            if (panel != null) {
                panel.OnMsg_Purchase();
            }
        }
        //END 라면 창을 한번 갱신 해준다.
        else {
            RefreshGachaResult(true);
        }
    }

    private void SetItemList() {
        //ItemList
        for (int i = 0; i < _ItemList.Count; i++)
            _ItemList[i].gameObject.SetActive(false);

        for (int i = 0; i < GameInfo.Instance.RewardList.Count; i++) {
            //획득한 보상 리스트 셋팅
            RewardData originData = GameInfo.Instance.RewardList[i];
            RewardData rewardData = mRewardList.Find(x => x.Index == originData.Index);
            if (rewardData != null) {
                ++rewardData.Count;
            }
            else {
                rewardData = new RewardData(originData.Type, originData.Index, originData.Value, 1, 0);
                mRewardList.Add(rewardData);
            }

            ++rewardData.NewCount;

            //현재 슬롯 세팅
            SetItemSlot(originData, i);
        }
    }

    private void SetItemSlot(RewardData reward, int index) {
        if (reward.Type == (int)eREWARDTYPE.WEAPON) {
            WeaponData data = GameInfo.Instance.GetWeaponData(reward.UID);
            if (data != null) {
                _ItemList[index].ParentGO = this.gameObject;
                _ItemList[index].UpdateSlot(UIItemListSlot.ePosType.Result, index, data);
            }
        }
        else if (reward.Type == (int)eREWARDTYPE.GEM) {
            GemData data = GameInfo.Instance.GetGemData(reward.UID);
            if (data != null) {
                _ItemList[index].ParentGO = this.gameObject;
                _ItemList[index].UpdateSlot(UIItemListSlot.ePosType.Result, index, data);
            }
        }
        else if (reward.Type == (int)eREWARDTYPE.CARD) {
            CardData data = GameInfo.Instance.GetCardData(reward.UID);
            if (data != null) {
                _ItemList[index].ParentGO = this.gameObject;
                _ItemList[index].UpdateSlot(UIItemListSlot.ePosType.Result, index, data);
            }
        }
        else if (reward.Type == (int)eREWARDTYPE.ITEM) {
            ItemData data = GameInfo.Instance.GetItemData(reward.UID);
            if (data != null) {
                _ItemList[index].ParentGO = this.gameObject;
                _ItemList[index].UpdateSlot(UIItemListSlot.ePosType.Result, index, data, reward.Value);
            }
        }
        
        if (reward.bNew == true) {
            _ItemList[index].kNewSpr.gameObject.SetActive(true);
            _ItemList[index].kNewSpr.spriteName = "item_new";
        }
        else {
            _ItemList[index].kNewSpr.gameObject.SetActive(false);
        }

        _ItemList[index].gameObject.SetActive(true);
    }

    private void ShowGachaRewardListPopup() {
        UIAutoGachaRewardListPopup popup = LobbyUIManager.Instance.GetUI<UIAutoGachaRewardListPopup>("AutoGachaRewardListPopup");
        if (popup == null) {
            Debug.LogError("UIAutoGachaRewardListPopup 팝업 없음");
        }
        else {
            //LobbyUIManager.Instance.ShowUI("AutoGachaResultPopup", true);
            popup.SetData(mRewardList);
            popup.SetUIActive(true);
        }
    }

}
