using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageToastPopup
{
    public static UIMessageToastPopup GetMessageToastPopup()
    {
        UIMessageToastPopup mpopup = null;
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
            mpopup = LobbyUIManager.Instance.GetUI<UIMessageToastPopup>("MessageToastPopup");
        else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
            mpopup = GameUIManager.Instance.GetUI<UIMessageToastPopup>("MessageToastPopup");

        return mpopup;
    }

    public static void Show( string str, bool bautoclose = true )
    {
        UIMessageToastPopup mpopup = GetMessageToastPopup();
        mpopup.InitMessageToastPopup(str, bautoclose);
    }
}

public class UIMessageToastPopup : FComponent
{
    public UILabel kTextLabel;
    public UIButton kBGBtn;


    public bool InitMessageToastPopup(string text, bool bautoclose)
    {
        kTextLabel.textlocalize = text;
        if( UIAni != null )
            UIAni.Stop();
        CancelInvoke("OnClickClose");

        SetUIActive(true);

        if (bautoclose)
        {
            Invoke("OnClickClose", GameInfo.Instance.GameConfig.MessageToastDuration);
            //kBGBtn.gameObject.SetActive(false);
        }
        else
        {
            kBGBtn.gameObject.SetActive(true);
        }

        return true;
    }

    public override void Renewal(bool bChildren)
    {

    }

    public override void OnClickClose()
    {
        CancelInvoke("OnClickClose");
        base.OnClickClose();
        
    }

    public override void OnClose()
    {
        base.OnClose();
    }

    public void OnClick_BG()
    {
        OnClickClose();
    }
}
