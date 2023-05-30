using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGamePlayHelpPopup : FComponent
{
    public GameObject PC;
    public GameObject Mobile;


	public override void OnEnable() {
		PC?.SetActive( false );
		Mobile?.SetActive( false );
#if !DISABLESTEAMWORKS
        PC?.SetActive(true);
        AppMgr.Instance.CustomInput.ShowCursor(true);
#else
		Mobile?.SetActive( true );
#endif

		base.OnEnable();
	}

	private void LateUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            World.Instance.OnCloseHelpPopup();
            OnClickClose();
        }
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);
    }

    public override void OnClickClose()
    {
        World.Instance.OnCloseHelpPopup();
        base.OnClickClose();
    }

    public void OnClick_BGBtn()
    {
        //AppMgr.Instance.CustomInput.ShowCursor(false);
        OnClickClose();
    }
}
