using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAniEvent : MonoBehaviour {

	public void UIEventShowUI( string uiname )
    {
        if( AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby )
        {
            LobbyUIManager.Instance.ShowUI(uiname, true);
        }
        else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
        {
            GameUIManager.Instance.ShowUI(uiname, true);
        }
    }

    public void UIEventHideUI(string uiname)
    {
        
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
        {
            LobbyUIManager.Instance.HideUI(uiname, true);
        }
        else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
        {
            GameUIManager.Instance.HideUI(uiname, true);
        }
    }

    public void SelfHide()
    {
        gameObject.SetActive(false);
    }
}
