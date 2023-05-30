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

        //  ���� ���� ����
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

    //  �˾� �ݱ� ��ư
    public void OnClick_closeBtn()
    {
        if(mbTutorial)
        {
            LobbyUIManager.Instance.InitComponent("MainPanel");
            LobbyUIManager.Instance.Renewal("MainPanel");
        }
        
        OnClickClose();
    }

    //  ��ü ���� ��ư
    public void OnClick_AllConfirmBtn()
    {
        ReciveAllStart();
    }

    /// <summary>
    ///  ���� ���� �ޱ�
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
    ///  ���� ��ü �ޱ�
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
    ///  ��Ŷ ������ ó��
    /// </summary>
    public void OnNetMailTakeProductList(int result, PktMsgType pktmsg)
    {
        //  0�� ����, �ٸ��� ���� 
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
            //��� �ڿ� �ٽ� �õ��� �ּ���
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3393));
        }
        else {
            GameInfo.Instance.Send_ReqMailList(0, (uint)GameInfo.Instance.GameConfig.MaxMailCnt, true, OnRefreshPopup);
        }
    }

    private void OnRefreshPopup(int result, PktMsgType pktmsg) {
        if (result != 0)
            return;

        //������ ����� ���ΰ�ħ �Ͽ����ϴ�.
        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3394));

        mLastRefreshTime = GameSupport.GetCurrentServerTime();
        Renewal(true);
    }
}
