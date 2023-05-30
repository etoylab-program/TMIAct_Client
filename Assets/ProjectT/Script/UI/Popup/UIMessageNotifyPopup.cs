using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageNotifyPopup
{
	public static UIMessageNotifyPopup GetMessageNotifyPopup()
	{
		UIMessageNotifyPopup popup = null;

		if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
			popup = LobbyUIManager.Instance.GetUI<UIMessageNotifyPopup>("MessageNotifyPopup");
		else if(AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
			popup = GameUIManager.Instance.GetUI<UIMessageNotifyPopup>("MessageNotifyPopup");

		return popup;
	}

	public static void Show(string str, float duration)
	{
		UIMessageNotifyPopup popup = GetMessageNotifyPopup();
		if (popup == null)
		{
			Log.Show(AppMgr.Instance.SceneType + " MessageNotifyPopup is NULL", Log.ColorType.Red);
			return;
		}

		popup.InitNotifyPopup(str, duration);
	}
}

public class UIMessageNotifyPopup : FComponent
{
	public UILabel kTextLabel;
 

	public void InitNotifyPopup(string str, float duration)
	{
		kTextLabel.textlocalize = str;
		if(!this.gameObject.activeSelf)
			SetUIActive(true);
		CancelInvoke("ClosePopup");
		Invoke("ClosePopup", duration);
	}
 
	private void ClosePopup()
	{
		base.OnClickClose();
	}
 
}
