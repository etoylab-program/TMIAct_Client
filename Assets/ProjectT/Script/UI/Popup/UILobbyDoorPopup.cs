using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LobbyDoorPopup
{
    static UILobbyDoorPopup kPopup;

    public static UILobbyDoorPopup GetDoorPopup()
    {
        return LobbyUIManager.Instance.ShowUI("LobbyDoorPopup", true) as UILobbyDoorPopup;
    }

    public static void Show(UILobbyDoorPopup.OnDoorPopupCallBack callback = null, bool autoHide = true)
    {
        UILobbyDoorPopup mpopup = GetDoorPopup();
        if (mpopup == null)
            return;

        Lobby.Instance.ShowBgChar(false, true);
        mpopup.InitDoorPopup(callback, autoHide);
    }

    public static void Show(float delay, UILobbyDoorPopup.OnDoorPopupCallBack callback = null, bool autoHide = true)
    {
        UILobbyDoorPopup mpopup = GetDoorPopup();
        if (mpopup == null)
            return;

        Lobby.Instance.ShowBgChar(false, true);
        mpopup.InitDoorPopup(delay, callback, autoHide);
    }
}

public class UILobbyDoorPopup : FComponent
{
    public delegate void OnDoorPopupCallBack();
    private OnDoorPopupCallBack CallBackFunc;
    private float fWaitDelay = 0f;
    private bool m_autoHide = false;


	public override void OnEnable() {
		StartCoroutine( OpenDoor_Check() );
		base.OnEnable();
	}

	public void InitDoorPopup(OnDoorPopupCallBack callback, bool autoHide)
    {
        m_autoHide = autoHide;
        CallBackFunc = callback;
    }

    public void InitDoorPopup(float delay, OnDoorPopupCallBack callback, bool autoHide)
    {
        m_autoHide = autoHide;
        fWaitDelay = delay;

        CallBackFunc = callback;
    }

    IEnumerator OpenDoor_Check()
    {
        LobbyUIManager.Instance.HideUI("MainPanel");
        yield return new WaitForEndOfFrame();

        while (this.gameObject.activeSelf)
        {
            if (!IsPlayAnimtion(0))
                break;

            yield return null;
        }

        PlayAnimtion(2);

        SoundManager.Instance.PlayUISnd(32);
        yield return new WaitForSeconds(1f);

        if(CallBackFunc != null)
        {
            CallBackFunc();
            CallBackFunc = null;
        }

        if (m_autoHide)
        {
            LobbyUIManager.Instance.HideUI("LobbyDoorPopup", true);
        }
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);
	}

    public override bool IsBackButton()
    {
        return false;
    }
}