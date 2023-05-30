using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MessageEventData
{
    public string Text;

    public MessageEventData(string text)
    {
        Text = text;
    }
    /*
    public eNOTICE eType;
    public int TableID;
    public string Text;
    public bool AutoClose;
    public NoticePopupData(eNOTICE e, int tableid, string text, bool autoclose)
    {
        eType = e;
        TableID = tableid;
        Text = text;
        AutoClose = autoclose;
    }
    */
}

public class MessageEventPopup
{
    private static List<MessageEventData> _noticepopupdatalist = new List<MessageEventData>();

    public static UIMessageEventPopup GetMessageEventPopup()
    {
        UIMessageEventPopup mpopup = null;
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
            mpopup = LobbyUIManager.Instance.GetUI<UIMessageEventPopup>("MessageEventPopup");
        else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
            mpopup = GameUIManager.Instance.GetUI<UIMessageEventPopup>("MessageEventPopup");
        return mpopup;
    }

    public static bool IsShowMessageEventPopup()
    {
        UIMessageEventPopup mpopup = null;
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
        {
            if (LobbyUIManager.Instance.IsActiveUI("MessageEventPopup"))
                return true;
        }
        else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
        {
            if (GameUIManager.Instance.IsActiveUI("MessageEventPopup"))
                return true;

        }
        return false;
    }

    public static void AddMessageEventText(string text)
    {
        _noticepopupdatalist.Add(new MessageEventData(text));
        if (!IsShowMessageEventPopup())
            NextShowMessageEventPopup();
    }

    public static void NextShowMessageEventPopup()
    {
        if (_noticepopupdatalist == null)
            return;
        if (_noticepopupdatalist.Count == 0)
            return;

        MessageEventData data = _noticepopupdatalist[0];

        UIMessageEventPopup mpopup = GetMessageEventPopup();
        mpopup.InitMessageEventPopup(data.Text);
        _noticepopupdatalist.RemoveAt(0);
    }


    public static void Show(string str)
    {
        UIMessageEventPopup mpopup = GetMessageEventPopup();
        mpopup.InitMessageEventPopup(str);
    }
}

public class UIMessageEventPopup : FComponent
{
    public UISprite kIconSpr;
    public UILabel kCountLabel;
    public UILabel kTitleLabel;
    public UILabel kTextLabel;


    public override void OnEnable()
    {
        base.OnEnable();
        Invoke("OnClickClose", GameInfo.Instance.GameConfig.MessageEventDuration);
    }

    public bool InitMessageEventPopup(string text)
    {
        kIconSpr.gameObject.SetActive(false);
        kCountLabel.gameObject.SetActive(false);
        kTitleLabel.gameObject.SetActive(false);
        kTextLabel.gameObject.SetActive(true);
        kTextLabel.textlocalize = text;

        SetUIActive(true);

        return true;
    }

    public override void OnClose()
    {
        base.OnClose();
        MessageEventPopup.NextShowMessageEventPopup();
    }

    public override bool IsBackButton()
    {
        return false;
    }
}
