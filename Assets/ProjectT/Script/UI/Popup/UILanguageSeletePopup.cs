using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILanguageSeletePopup : FComponent
{
    public FTab kLanguageTab;
    private int _initlanguage = (int)eLANGUAGE.KOR;

    private int _prevLanguage = (int)eLANGUAGE.KOR;

    public override void Awake()
    {
        kLanguageTab.EventCallBack = OnLanguageTabSelect;
     
        base.Awake();
    }

	public override void OnEnable() {
		_initlanguage = (int)FLocalizeString.Language;
		_prevLanguage = (int)FLocalizeString.Language;
		kLanguageTab.SetTab( _initlanguage, SelectEvent.Code );

		base.OnEnable();
	}

    public void OnClick_BackBtn()
    {
        OnClickClose();
    }

    public void OnNetAckUserSetCountryAndLangCode(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;
        BannerManager.Instance.RefreshBannerImages();
#if !UNITY_EDITOR
        LocalPushNotificationManager.Instance.UnSubscribeLanguage();
        LocalPushNotificationManager.Instance.UnSubscribeNightPush();
        LocalPushNotificationManager.Instance.UnSubscribeGlobalPush();
        LocalPushNotificationManager.Instance.SetFCMSubscribe();
#endif
        base.OnClickClose();
    }

    public override void OnClickClose()
    {
        if (_prevLanguage != _initlanguage && GameInfo.Instance.netFlag == true)
        {
            GameInfo.Instance.Send_ReqUserSetCountryAndLangCode(OnNetAckUserSetCountryAndLangCode);
            return;
        }
        base.OnClickClose();
    }


    private bool OnLanguageTabSelect(int nSelect, SelectEvent type)
    {
        if (type == SelectEvent.Code)
            return true;

        if( _initlanguage == nSelect )
        {
            return true;
        }

        _initlanguage = nSelect;
        ChangeLanguage((eLANGUAGE)_initlanguage);
        return true;
    }

    public void ChangeLanguage(eLANGUAGE e)
    {
        FLocalizeString.Instance.InitLocalize(e);
        FLocalizeString.Instance.SaveLanguage();
        //ScenarioMgr.Instance.InitScenarioMgr(e);
        var root = GameObject.Find("UIRoot");
        if (root == null)
            return;

        GameSupport.ReLoadTimeString();

        var labellist = root.GetComponentsInChildren<FLocalizeLabel>(true);
        for (int i = 0; i < labellist.Length; i++)
            labellist[i].OnLocalize();

        var spritelist = root.GetComponentsInChildren<FLocalizeSprite>(true);
        for (int i = 0; i < spritelist.Length; i++)
            spritelist[i].OnLocalize();

        LobbyUIManager.Instance.Renewal();
    }
}
