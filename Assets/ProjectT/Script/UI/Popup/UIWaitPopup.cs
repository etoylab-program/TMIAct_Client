using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitPopup
{
    static UIWaitPopup GetWaitPopup()
    {
        UIWaitPopup mpopup = null;
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
            mpopup = LobbyUIManager.Instance.GetUI<UIWaitPopup>("WaitPopup");
        else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
            mpopup = GameUIManager.Instance.GetUI<UIWaitPopup>("WaitPopup");
        else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
            mpopup = TitleUIManager.Instance.GetUI<UIWaitPopup>("WaitPopup");
        return mpopup;
    }

    public static void Show( bool bimmediately = false )
    {
        if (!GameInfo.Instance.netFlag)
            return;

        UIWaitPopup wpopup = GetWaitPopup();
        if (wpopup == null)
            return;
        wpopup.SetUIActive(true, bimmediately);
    }

    public static void Hide()
    {
        if (!GameInfo.Instance.netFlag)
            return;

        UIWaitPopup wpopup = GetWaitPopup();
        if (wpopup == null)
            return;
        wpopup.SetUIActive(false);
    }

}

public class UIWaitPopup : FComponent
{
    public UIWidget kRootWidget;
    public GameObject kLoading;
    public bool kImmediately;


	public override void SetUIActive( bool _bActive, bool _bAnimation = true ) {
		if ( this.gameObject.activeSelf == _bActive ) {
			return;
		}

		if ( _bActive == true ) {
			kImmediately = _bAnimation;
		}

		StopCoroutine( FadeIn() );
		StopCoroutine( FadeOut() );

		if ( kImmediately ) {
			if ( _bActive ) {
				this.gameObject.SetActive( true );
				kRootWidget.alpha = 1.0f;
				kLoading.SetActive( true );
			}
			else {
				kRootWidget.alpha = 0.0f;
				this.gameObject.SetActive( false );
			}
		}
		else {
			if ( _bActive ) {
				this.gameObject.SetActive( true );
				kRootWidget.alpha = 0.0f;
				kLoading.SetActive( false );

				StartCoroutine( FadeIn() );
			}
			else {
				StartCoroutine( FadeOut() );
			}
		}
	}

	private IEnumerator FadeIn()
    {
        float f = kRootWidget.alpha;
        while (f < 1.0f)
        {
            f += (Time.deltaTime * 0.5f);
            kRootWidget.alpha = f;
            yield return null;
        }
        kRootWidget.alpha = 1.0f;
        yield return new WaitForSeconds(3.0f);
        kLoading.SetActive(true);
    }

    private IEnumerator FadeOut()
    {
        float f = kRootWidget.alpha;
        while (f > 0.0f)
        {
           f -= (Time.deltaTime * 0.5f);
            kRootWidget.alpha = f;
            yield return null;
        }

        kRootWidget.alpha = 0.0f;
        this.gameObject.SetActive(false);
    }

    public override bool IsBackButton()
    {
        return false;
    }
}
