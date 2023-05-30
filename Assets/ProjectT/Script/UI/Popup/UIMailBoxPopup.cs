using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIMailBoxPopup : FComponent
{
    [SerializeField] private FList _MailListInstance;
    [SerializeField] private UILabel _DescLabel;
    [SerializeField] private UILabel _NoneLabel;

    private List<CardBookData> mPrecardbookList = new List<CardBookData>();
    private bool mbRewardVisible = false;
    private bool mbTutorial = false;
    private DateTime mLastRefreshTime;

    public override void Awake()
    {
        base.Awake();

        if (this._MailListInstance == null) return;

        this._MailListInstance.EventUpdate = this._UpdateMailListSlot;
        this._MailListInstance.EventGetItemCount = this._GetMailElementCount;
    }

    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();

        mLastRefreshTime = GameSupport.GetCurrentServerTime();
    }

    public override void InitComponent()
    {
        mbTutorial = false;
        if ( GameSupport.IsTutorial() )
            mbTutorial = true;

        this._MailListInstance.UpdateList();
        mbRewardVisible = false;
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        if(_NoneLabel != null)
                _NoneLabel.gameObject.SetActive(false);

        this._MailListInstance.UpdateList();
        this._MailListInstance.ScrollPositionSet();

        if(GameInfo.Instance.MailList == null || GameInfo.Instance.MailList.Count <= 0)
        {
            if(_NoneLabel != null)
                _NoneLabel.gameObject.SetActive(true);
        }

        //  메일 보유 갯수
        FLocalizeString.SetLabel(_DescLabel, 276, GameInfo.Instance.MailList.Count, GameInfo.Instance.GameConfig.MaxMailCnt);
    }



    private void _UpdateMailListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIMailListSlot mailSlot = slotObject.GetComponent<UIMailListSlot>();
            if (mailSlot == null) break;
            mailSlot.ParentGO = gameObject;

            if (0 <= index && _GetMailElementCount() > index)
                mailSlot.UpdateSlot(GameInfo.Instance.MailList[index]);
            //Do UpdateListSlot
        } while (false);
    }

    private int _GetMailElementCount()
    {
        return GameInfo.Instance.MailList.Count; //TempValue
    }

    //  팝업 닫기 버튼
    public void OnClick_closeBtn()
    {
        if(mbTutorial)
        {
            LobbyUIManager.Instance.InitComponent("MainPanel");
            LobbyUIManager.Instance.Renewal("MainPanel");
        }
        
        OnClickClose();
    }

    //  전체 수령 버튼
    public void OnClick_AllConfirmBtn()
    {
        ReciveAllStart();
    }

    /// <summary>
    ///  단일 우편 받기
    /// </summary>
    public void ReciveMailStart(ulong mailUID)
    {
        if (!GameSupport.IsCheckInven())
            return;

        mbRewardVisible = true;

        List<ulong> list = new List<ulong>();
        list.Add(mailUID);

        mPrecardbookList.Clear();
        mPrecardbookList.AddRange(GameInfo.Instance.CardBookList);
        
        GameInfo.Instance.Send_ReqMailTakeProductList(list, OnNetMailTakeProductList);
    }

    /// <summary>
    ///  우편 전체 받기
    /// </summary>
    public void ReciveAllStart()
    {
        if(GameInfo.Instance.MailList.Count == 0)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3043), true);
            return;
        }

        if (!GameSupport.IsCheckInven())
            return;

        mbRewardVisible = true;

        List<ulong> list = new List<ulong>();
        for (int i = 0; i< GameInfo.Instance.MailList.Count; i++)
            list.Add(GameInfo.Instance.MailList[i].MailUID);

        GameInfo.Instance.Send_ReqMailTakeProductList(list, OnNetMailTakeProductList);
    }

    /// <summary>
    ///  패킷 받은후 처리
    /// </summary>
    public void OnNetMailTakeProductList(int result, PktMsgType pktmsg)
    {
        //  0이 성공, 다른수 실패 
        if (result != 0)
            return;

        string title = FLocalizeString.Instance.GetText(1144);
        string desc = FLocalizeString.Instance.GetText(1147);

        MessageRewardListPopup.RewardListMessage(title, desc, GameInfo.Instance.RewardList, OnMessageRewardCallBack);

        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("MainPanel");

        Renewal(true);

        
    }

    public void OnMessageRewardCallBack()
    {
        DirectorUIManager.Instance.PlayNewCardGreeings(mPrecardbookList);
        mbRewardVisible = false;
    }

    public override bool IsBackButton()
    {
        return !mbRewardVisible;
    }

    public override void OnClickClose()
    {
        NotificationManager.Instance.CheckNotification(NotificationManager.eTYPE.MAIL);
        base.OnClickClose();
    }


    public void OnClick_Refresh() {

        TimeSpan ts = GameSupport.GetCurrentServerTime() - mLastRefreshTime;
        if (ts.Seconds < GameInfo.Instance.GameConfig.MailRefreshTimeSec) {
            //잠시 뒤에 다시 시도해 주세요
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3393));
        }
        else {
            GameInfo.Instance.Send_ReqMailList(0, (uint)GameInfo.Instance.GameConfig.MaxMailCnt, true, OnRefreshPopup);
        }
    }

    private void OnRefreshPopup(int result, PktMsgType pktmsg) {
        if (result != 0)
            return;

        //선물함 목록을 새로고침 하였습니다.
        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3394));

        mLastRefreshTime = GameSupport.GetCurrentServerTime();
        Renewal(true);
    }
}
