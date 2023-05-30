using UnityEngine;
using System;
using System.Collections;

public class UIMenuListSlot : FSlot
{
    enum eMENUTYPE
    {
        NONE = 0,
        BOOK,
        USER,
        LOGIN,
        HOWTOBESTRONG,
        SETUP,
        NOTICE,
        ACCOUNTLINKING,
        COUPON,
        HELP,
        INFO,
        USE,
        LANGUAGE,
        TOTITLE,
        EXIT,
    }

	[SerializeField] private UISprite _Icon;
    [SerializeField] private UILabel _Text;
    [SerializeField] private UISprite _Notice;

    private GameClientTable.Menu.Param mTableData;
    public void UpdateSlot( int index, GameClientTable.Menu.Param tabledata) 	//Fill parameter if you need
	{
        mTableData = tabledata;

        _Notice.gameObject.SetActive(false);
        _Text.textlocalize = FLocalizeString.Instance.GetText(mTableData.Name);
        _Icon.spriteName = mTableData.Icon;
        _Icon.MakePixelPerfect();

        eMENUTYPE type = (eMENUTYPE)mTableData.ID;
        switch (type)
        {
            case eMENUTYPE.BOOK:
                {
                    if (NotificationManager.Instance.NewBookTotal != 0)
                        _Notice.gameObject.SetActive(true);
                }
                break;
            case eMENUTYPE.USER:
                {
                    if (NotificationManager.Instance.IsAchievementComplete || GameInfo.Instance.NewIconList.Count != 0)
                        _Notice.gameObject.SetActive(true);
                }
                break;
            case eMENUTYPE.NOTICE:
                _Notice.gameObject.SetActive(GameInfo.Instance.IsNewYouTubeLink());
                break;
        }
    }
 
	public void OnClick_Slot()
	{
        eMENUTYPE type = (eMENUTYPE)mTableData.ID;
        switch(type)
        {
            case eMENUTYPE.BOOK:
                {
                    LobbyUIManager.Instance.ShowUI("BookMainPopup", true);
                }
                break;
            case eMENUTYPE.USER:
                {
                    UIValue.Instance.SetValue(UIValue.EParamType.UserInfoPopup, 0);
                    LobbyUIManager.Instance.ShowUI("UserInfoPopup", true);
                }
                break;
            case eMENUTYPE.LOGIN:
                {
                    //LobbyUIManager.Instance.ShowUI("DailyLoginBonusPopup", true);
                    //DailyLoginBonusPopup.OpenDailyLoginPopup();
                    LobbyUIManager.Instance.LoginBonusStep = eLoginBonusStep.Step01;
                    LobbyUIManager.Instance.ShowDailyLoginPopup(false);
                }
                break;
            case eMENUTYPE.HOWTOBESTRONG:
                {
                    //UIValue.Instance.SetValue(UIValue.EParamType.HowToBeStrongCUID, (long)-1);
                    UIValue.Instance.SetValue(UIValue.EParamType.HowToBeStrongCUID, GameInfo.Instance.GetMainCharUID());
                    //LobbyUIManager.Instance.ShowUI("HowToBeStrongPopup", true);
                    LobbyUIManager.Instance.ShowUI("HowToBeStrongV2Popup", true);
                }
                break;
            case eMENUTYPE.SETUP:
                {
                    LobbyUIManager.Instance.ShowUI("OptionPopup", true);
                }
                break;
            case eMENUTYPE.NOTICE:
                {
                    if( string.IsNullOrEmpty(GameInfo.Instance.GameConfig.WebNotice) )
                    {
                        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3001));
                    }
                    else
                    {
                        GameSupport.OpenWebView(FLocalizeString.Instance.GetText(500), GameSupport.GetServiceTypeWebAddr(GameInfo.Instance.GameConfig.WebNotice));
                    }

                    _Notice.gameObject.SetActive(false);
                }
                break;
            case eMENUTYPE.ACCOUNTLINKING:
                {
                    LobbyUIManager.Instance.ShowUI("AccountLinkingPopup", true);
                }
                break;
            case eMENUTYPE.COUPON: 
                {
                    string couponUrl = "";
                    switch (AppMgr.Instance.ServerType) {
                        case AppMgr.eServerType.INTERNAL:
                            couponUrl = GameInfo.Instance.GameConfig.WebInCoupon;
                            break;
                        case AppMgr.eServerType.QA:
                            couponUrl = "";//작업 당시 QA서버는 미사용
                            break;
                        case AppMgr.eServerType.REVIEW:
                            couponUrl = GameInfo.Instance.GameConfig.WebReCoupon;
                            break;
                        case AppMgr.eServerType.LIVE:
                            couponUrl = GameInfo.Instance.GameConfig.WebCoupon;
                            break;
                        default:
                            break;
                    }

                    if (string.IsNullOrEmpty(couponUrl)) 
                    {
                        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3001));
                    }
                    else 
                    {
                        //7월 URL 개편 작업 전 까지 사용 할 임시 코드 
                        string lang = "ko";
                        switch (FLocalizeString.Language) {
                            case eLANGUAGE.KOR:
                                lang = "ko";
                                break;
                            case eLANGUAGE.JPN:
                                lang = "ja";
                                break;
                            case eLANGUAGE.ENG:
                                lang = "en";
                                break;
                            case eLANGUAGE.CHS:
                                lang = "zh_cn";
                                break;
                            case eLANGUAGE.CHT:
                                lang = "zh_tw";
                                break;
                            case eLANGUAGE.ESP:
                                lang = "es";
                                break;
                            default:
                                break;
                        }

                        string uuid = GameInfo.Instance.UserData.UUID.ToString();
                        string url = $"{couponUrl}?uuid={uuid}&lng={lang}";

                        GameSupport.OpenWebView(FLocalizeString.Instance.GetText(mTableData.Name), url);
                    }
                }
                break;
            case eMENUTYPE.HELP:
                {
                    if (string.IsNullOrEmpty(GameInfo.Instance.GameConfig.WebHelp))
                    {
                        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3001));
                    }
                    else
                    {
                        GameSupport.OpenWebView(FLocalizeString.Instance.GetText(501), GameSupport.GetServiceTypeWebAddr(GameInfo.Instance.GameConfig.WebHelp));
                    }
                }
                break;
            case eMENUTYPE.INFO:
                {
                    if (string.IsNullOrEmpty(GameInfo.Instance.GameConfig.WebQuestions))
                    {
                        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3001));
                    }
                    else
                    {
                        GameSupport.OpenWebView(FLocalizeString.Instance.GetText(502), GameSupport.GetServiceTypeWebAddr(GameInfo.Instance.GameConfig.WebQuestions));
                    }
                }
                break;
            case eMENUTYPE.USE:
                {
                    if (string.IsNullOrEmpty(GameInfo.Instance.GameConfig.WebTermsofservice))
                    {
                        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3001));
                    }
                    else
                    {
                        GameSupport.OpenWebView(FLocalizeString.Instance.GetText(503), GameSupport.GetServiceTypeWebAddr(GameInfo.Instance.GameConfig.WebTermsofservice));
                    }
                }
                break;
            case eMENUTYPE.TOTITLE:
                {
                    MessagePopup.CYN(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3030), eTEXTID.YES, eTEXTID.NO, OnMsg_ToTitle);
                }
                break;
            case eMENUTYPE.LANGUAGE:
                {
                    LobbyUIManager.Instance.ShowUI("LanguageSeletePopup", true);                    
                }
                break;
            case eMENUTYPE.EXIT:
                {
                    MessagePopup.CYN(eTEXTID.TITLE_NOTICE, 3046, eTEXTID.YES, eTEXTID.NO, OnMsg_Exit);
                }
                break;
        }

    }

    private void OnMsg_ToTitle()
    {
        //#if UNITY_STANDALONE
        //        GameInfo.Instance.CurAccountType = eAccountType._MAX_;
        //#endif
        FGlobalTimer.Instance.RemoveAllTimer();
        GameInfo.Instance.SetPktInitData();
        GameInfo.Instance.MoveLobbyToTitle();
    }

    public void OnMsg_Exit()
    {
        Application.Quit();
    }
}
