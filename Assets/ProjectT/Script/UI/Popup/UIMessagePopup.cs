using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MessagePopup
{
    public static bool IsActive = false;
    public static UIMessagePopup GetMessagePopup()
    {
        UIMessagePopup mpopup = null;
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
            mpopup = LobbyUIManager.Instance.GetUI<UIMessagePopup>("MessagePopup");
        else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage || AppMgr.Instance.SceneType == AppMgr.eSceneType.Training)
            mpopup = GameUIManager.Instance.GetUI<UIMessagePopup>("MessagePopup");
        else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
            mpopup = TitleUIManager.Instance.GetUI<UIMessagePopup>("MessagePopup");
        else
        {
            GameObject uiroot = GameObject.FindObjectOfType<UIRoot>().gameObject;
            mpopup = uiroot.transform.Find("MessagePopup").GetComponent<UIMessagePopup>();
        }
        return mpopup;
    }

    public static void ShowMessagePopup(bool bAni = true)
    {
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
            LobbyUIManager.Instance.ShowUI("MessagePopup", true);
        else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage || AppMgr.Instance.SceneType == AppMgr.eSceneType.Training)
            GameUIManager.Instance.ShowUI("MessagePopup", bAni);
        else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
            TitleUIManager.Instance.ShowUI("MessagePopup", true);
        else
        {
            GameObject uiroot = GameObject.FindObjectOfType<UIRoot>().gameObject;
            UIMessagePopup mpopup = uiroot.transform.Find("MessagePopup").GetComponent<UIMessagePopup>();
            mpopup.SetUIActive(true, bAni);
        }
    }

    public static void ClosePopup()
    {
        UIMessagePopup popup = GetMessagePopup();
        if(popup == null || !popup.gameObject.activeSelf)
        {
            return;
        }

        popup.OnClose();
    }

    public static void OK(eTEXTID title, int text, UIMessagePopup.OnClickOKCallBack callbackok, bool playAni = true, bool ignoreBGBtn = false)
    {
        UIMessagePopup mpopup = GetMessagePopup();
        if (mpopup == null)
            return;

        mpopup.IgnoreBGBtn = ignoreBGBtn;
        mpopup.InitMessagePopupOK(FLocalizeString.Instance.GetText((int)title), FLocalizeString.Instance.GetText(text), FLocalizeString.Instance.GetText((int)eTEXTID.OK), eGOODSTYPE.NONE, 0, callbackok, playAni);
    }

    public static void OK(eTEXTID title, string text, UIMessagePopup.OnClickOKCallBack callbackok, bool playAni = true, bool ignoreBGBtn = false)
    {
        UIMessagePopup mpopup = GetMessagePopup();
        if (mpopup == null)
            return;

        mpopup.IgnoreBGBtn = ignoreBGBtn;
        mpopup.InitMessagePopupOK(FLocalizeString.Instance.GetText((int)title), text, FLocalizeString.Instance.GetText((int)eTEXTID.OK), eGOODSTYPE.NONE, 0, callbackok, playAni);
    }

    public static void OK(int title, string text, UIMessagePopup.OnClickOKCallBack callbackok)
    {
        UIMessagePopup mpopup = GetMessagePopup();
        if (mpopup == null)
            return;
        mpopup.InitMessagePopupOK(FLocalizeString.Instance.GetText(title), text, FLocalizeString.Instance.GetText((int)eTEXTID.OK), eGOODSTYPE.NONE, 0, callbackok);
    }

    public static void OK(string title, string text, string btn, UIMessagePopup.OnClickOKCallBack callbackok)
    {
        UIMessagePopup mpopup = GetMessagePopup();
        if (mpopup == null)
            return;
        mpopup.InitMessagePopupOK(title, text, btn, eGOODSTYPE.NONE, 0, callbackok);
    }

    public static void OK(string title, int stageId, UIMessagePopup.OnClickOKCallBack callbackok)
    {
        UIMessagePopup mpopup = GetMessagePopup();
        if (mpopup == null)
            return;
        mpopup.InitBoSetMessagePopupOK(title, stageId, FLocalizeString.Instance.GetText(1), eGOODSTYPE.NONE, 0, callbackok);
    }

    public static void OK(string title, string text, string btn, UIMessagePopup.OnClickOKCallBack callbackok, UIMessagePopup.OnClickOKCallBack callbackno)
    {
        UIMessagePopup mpopup = GetMessagePopup();
        if (mpopup == null)
            return;
        mpopup.InitMessagePopupOK(title, text, btn, eGOODSTYPE.NONE, 0, callbackok, callbackno);
    }

    public static void OK( string title, string text, int wpValue, long itemUid, UIMessagePopup.OnClickOKCallBack callback ) {
        UIMessagePopup mpopup = GetMessagePopup();
        if ( mpopup == null ) {
            return;
        }

        mpopup.InitUseWpResetTicketPopup( title, text, wpValue, itemUid, callback );
    }

    public static void OKCANCEL(eTEXTID title, int text, UIMessagePopup.OnClickOKCallBack callbackyes, UIMessagePopup.OnClickOKCallBack callbackno = null, bool bAni = true, bool includeCloseBtn = false, bool ignoreBGBtn = false)
    {
        UIMessagePopup mpopup = GetMessagePopup();
        if (mpopup == null)
            return;

        mpopup.IgnoreBGBtn = ignoreBGBtn;
        mpopup.InitMessagePopupYN(FLocalizeString.Instance.GetText((int)title), FLocalizeString.Instance.GetText(text), FLocalizeString.Instance.GetText((int)eTEXTID.OK), FLocalizeString.Instance.GetText((int)eTEXTID.CANCEL), eGOODSTYPE.NONE, 0, false, callbackyes, callbackno, bAni, includeCloseBtn);
    }
    
    public static void OKCANCEL(eTEXTID title, string text, UIMessagePopup.OnClickOKCallBack callbackyes, UIMessagePopup.OnClickOKCallBack callbackno = null, UIMessagePopup.OnClickOKCallBack callbackCharBuyMove = null)
    {
        UIMessagePopup mpopup = GetMessagePopup();
        if (mpopup == null)
        {
            return;
        }
        
        mpopup.InitMessagePopupYN(FLocalizeString.Instance.GetText((int)title), text, FLocalizeString.Instance.GetText((int)eTEXTID.OK), FLocalizeString.Instance.GetText((int)eTEXTID.CANCEL), eGOODSTYPE.NONE, 0, false, callbackyes, callbackno, callbackCharBuyMove);
    }

    public static void YN(eTEXTID title, int text, UIMessagePopup.OnClickOKCallBack callbackyes, UIMessagePopup.OnClickOKCallBack callbackno = null)
    {
        UIMessagePopup mpopup = GetMessagePopup();
        if (mpopup == null)
            return;
        mpopup.InitMessagePopupYN(FLocalizeString.Instance.GetText((int)title), FLocalizeString.Instance.GetText(text), FLocalizeString.Instance.GetText((int)eTEXTID.YES), FLocalizeString.Instance.GetText((int)eTEXTID.NO), eGOODSTYPE.NONE, 0, false, callbackyes, callbackno);
    }

    public static void YN(eTEXTID title, string text, UIMessagePopup.OnClickOKCallBack callbackyes, UIMessagePopup.OnClickOKCallBack callbackno = null, int wpValue = -1)
    {
        UIMessagePopup mpopup = GetMessagePopup();
        if (mpopup == null)
            return;
        mpopup.InitMessagePopupYN(FLocalizeString.Instance.GetText((int)title), text, FLocalizeString.Instance.GetText((int)eTEXTID.YES), FLocalizeString.Instance.GetText((int)eTEXTID.NO), eGOODSTYPE.NONE, 0, false, callbackyes, callbackno, wpValue:wpValue);
    }

	public static void YNAuto( eTEXTID title, string text, string yesBtnStr, UIMessagePopup.OnClickOKCallBack yesCallback ) {
		UIMessagePopup mpopup = GetMessagePopup();
		if ( mpopup == null ) {
			return;
		}

		mpopup.InitMessagePopupYNAutoGemOption( FLocalizeString.Instance.GetText( (int)title ), text, yesBtnStr, FLocalizeString.Instance.GetText( (int)eTEXTID.CANCEL ), eGOODSTYPE.NONE, 0, false, yesCallback, null );
	}

    public static void YNEventGacha( eTEXTID title, string text, UIMessagePopup.OnClickOKCallBack yesCallback, OnEventTabSelect tabSelectCallback ) {
		UIMessagePopup mpopup = GetMessagePopup();
		if ( mpopup == null ) {
			return;
		}

		mpopup.InitMessagePopupYNEventGacha( FLocalizeString.Instance.GetText( (int)title ), text, FLocalizeString.Instance.GetText( (int)eTEXTID.YES ), FLocalizeString.Instance.GetText( (int)eTEXTID.CANCEL ), eGOODSTYPE.NONE, 0, false, yesCallback, null, tabSelectCallback );		
	}

	public static void CYN(eTEXTID title, int text, eTEXTID btnyes, eTEXTID btnno, UIMessagePopup.OnClickOKCallBack callbackyes, UIMessagePopup.OnClickOKCallBack callbackno = null)
    {
        UIMessagePopup mpopup = GetMessagePopup();
        if (mpopup == null)
            return;
        mpopup.InitMessagePopupYN(FLocalizeString.Instance.GetText((int)title), FLocalizeString.Instance.GetText(text), FLocalizeString.Instance.GetText((int)btnyes), FLocalizeString.Instance.GetText((int)btnno), eGOODSTYPE.NONE, 0, false, callbackyes, callbackno);
    }

    public static void CYN(eTEXTID title, int text, eTEXTID btnyes, eTEXTID btnno, eGOODSTYPE etype, long count, UIMessagePopup.OnClickOKCallBack callbackyes, UIMessagePopup.OnClickOKCallBack callbackno = null)
    {
        UIMessagePopup mpopup = GetMessagePopup();
        if (mpopup == null)
            return;
        mpopup.InitMessagePopupYN(FLocalizeString.Instance.GetText((int)title), FLocalizeString.Instance.GetText(text), FLocalizeString.Instance.GetText((int)btnyes), FLocalizeString.Instance.GetText((int)btnno), etype, count, false, callbackyes, callbackno);
    }

    public static void CYN(eTEXTID title, string text, eTEXTID btnyes, eTEXTID btnno, UIMessagePopup.OnClickOKCallBack callbackyes, UIMessagePopup.OnClickOKCallBack callbackno = null)
    {
        UIMessagePopup mpopup = GetMessagePopup();
        if (mpopup == null)
            return;
        mpopup.InitMessagePopupYN(FLocalizeString.Instance.GetText((int)title), text, FLocalizeString.Instance.GetText((int)btnyes), FLocalizeString.Instance.GetText((int)btnno), eGOODSTYPE.NONE, 0, false, callbackyes, callbackno);
    }

    public static void CYN(string title, string text, string btnyes, string btnno, UIMessagePopup.OnClickOKCallBack callbackyes, UIMessagePopup.OnClickOKCallBack callbackno = null)
    {
        UIMessagePopup mpopup = GetMessagePopup();
        if (mpopup == null)
            return;
        mpopup.InitMessagePopupYN(title, text, btnyes, btnno, eGOODSTYPE.NONE, 0, false, callbackyes, callbackno);
    }

    public static void CYN(eTEXTID title, string text, eTEXTID btnyes, eTEXTID btnno, eGOODSTYPE etype, long count, UIMessagePopup.OnClickOKCallBack callbackyes, UIMessagePopup.OnClickOKCallBack callbackno = null)
    {
        UIMessagePopup mpopup = GetMessagePopup();
        if (mpopup == null)
            return;
        mpopup.InitMessagePopupYN(FLocalizeString.Instance.GetText((int)title), text, FLocalizeString.Instance.GetText((int)btnyes), FLocalizeString.Instance.GetText((int)btnno), etype, count, false, callbackyes, callbackno);
    }

    public static void CYN( string title, string text, eTEXTID btnyes, eTEXTID btnno, eGOODSTYPE etype, long count, UIMessagePopup.OnClickOKCallBack callbackyes, UIMessagePopup.OnClickOKCallBack callbackno = null ) {
        UIMessagePopup mpopup = GetMessagePopup();
        if( mpopup == null )
            return;
        mpopup.InitMessagePopupYN( title, text, FLocalizeString.Instance.GetText( (int)btnyes ), FLocalizeString.Instance.GetText( (int)btnno ), etype, count, false, callbackyes, callbackno );
    }

    public static void CYNItem(eTEXTID title, string text, eTEXTID btnyes, eTEXTID btnno, long itemuid, long count, UIMessagePopup.OnClickOKCallBack callbackyes, UIMessagePopup.OnClickOKCallBack callbackno = null)
    {
        UIMessagePopup mpopup = GetMessagePopup();
        if (mpopup == null)
            return;

        mpopup.InitMessageItemPopupYN(FLocalizeString.Instance.GetText((int)title), text, FLocalizeString.Instance.GetText((int)btnyes), FLocalizeString.Instance.GetText((int)btnno), itemuid, count, false, callbackyes, callbackno);
    }

    public static void CYNItem(string title, string text, eTEXTID btnyes, eTEXTID btnno, long itemuid, long count, UIMessagePopup.OnClickOKCallBack callbackyes, UIMessagePopup.OnClickOKCallBack callbackno = null)
    {
        UIMessagePopup mpopup = GetMessagePopup();
        if (mpopup == null)
            return;

        mpopup.InitMessageItemPopupYN(title, text, FLocalizeString.Instance.GetText((int)btnyes), FLocalizeString.Instance.GetText((int)btnno), itemuid, count, false, callbackyes, callbackno);
    }

    public static void CYNCheck(string title, string text, string btnyes, string btnno, UIMessagePopup.OnClickOKCallBack callbackyes, UIMessagePopup.OnClickOKCallBack callbackno = null)
    {
        UIMessagePopup mpopup = GetMessagePopup();
        if (mpopup == null)
            return;
        mpopup.InitMessagePopupYN(title, text, btnyes, btnno, eGOODSTYPE.NONE, 0, true, callbackyes, callbackno);
    }

    public static void TextLong(string title, string text, bool ignoreBGBtn = false,  UIMessagePopup.OnClickOKCallBack callbackClose = null)
    {
        UIMessagePopup mpopup = GetMessagePopup();
        if (mpopup == null)
            return;
        mpopup.IgnoreBGBtn = ignoreBGBtn;
        mpopup.InitMessagePopupTextLong(title, text, callbackClose);
    }

    public static void IAPMessage(string title, string text, GameTable.Store.Param storeItem, UIMessagePopup.OnClickOKCallBack callbackIAPSuccess, UIMessagePopup.OnClickOKCallBack callbackIAPFailed = null)
    {
        UIMessagePopup mpopup = GetMessagePopup();
        if (mpopup == null)
            return;
        mpopup.IgnoreBGBtn = true;
        mpopup.InitIAPMessagePopup(title, text, storeItem, callbackIAPSuccess, callbackIAPFailed);
    }

    public static void ScreenShotPopup(string title, string text, UIMessagePopup.OnClickOKCallBack callbackyes)
    {
        UIMessagePopup mpopup = GetMessagePopup();
        if (mpopup == null)
            return;

        mpopup.IgnoreBGBtn = true;
        mpopup.InitScreenShotPopup(title, text, callbackyes);
    }

    public static void CYNItemCash(string title, string text, eTEXTID btnyes, eTEXTID btnno, long itemuid, long count, UIMessagePopup.OnClickOKCallBack callbackyes, UIMessagePopup.OnClickOKCallBack callbackno = null)
    {
        UIMessagePopup mpopup = GetMessagePopup();
        if (mpopup == null)
            return;

        mpopup.InitMessageItemBuyCashPopupYN(title, text, FLocalizeString.Instance.GetText((int)btnyes), FLocalizeString.Instance.GetText((int)btnno), itemuid, count, false, callbackyes, callbackno);
    }

    public static void CYNReward(string pTitle, string pDesc, RewardData pRewardData, eREWARDTYPE pConsumType, int pConsumIndex, long pConsumCount, eTEXTID pYesId, eTEXTID pNoId, UIMessagePopup.OnClickOKCallBack pYesCallback, UIMessagePopup.OnClickOKCallBack pNoCallback = null)
    {
        UIMessagePopup uiMessagePopup = GetMessagePopup();
        if (uiMessagePopup == null)
        {
            return;
        }

        uiMessagePopup.InitMessagePopupRewardYN(pTitle, pDesc, pRewardData, pConsumType, pConsumIndex, pConsumCount,
            FLocalizeString.Instance.GetText((int)pYesId), FLocalizeString.Instance.GetText((int)pNoId), pYesCallback, pNoCallback);
    }

    public static void YNGemAnalyzed(string title, string gemSpriteName = "", UIMessagePopup.OnClickOKCallBack yesCallback = null)
    {
        UIMessagePopup messagePopup = GetMessagePopup();
        if (messagePopup == null)
        {
            return;
        }

        messagePopup.InitMessagePopupGemAnalyzedYN(title, gemSpriteName, yesCallback);
    }


    public static void YNFriendDelete(List<FriendUserData> deleteFriendUserDataList, UIMessagePopup.OnClickOKCallBack yesCallback = null) {
        UIMessagePopup messagePopup = GetMessagePopup();
        if (messagePopup == null) {
            return;
        }

        messagePopup.InitMessagePopupFriendDeleteYN(deleteFriendUserDataList, yesCallback);
    }
}



public class UIMessagePopup : FComponent
{
    public enum eMsgResultType
    {
        NONE,
        OK,
        YES,
        NO,
        CANCEL,
        CLOSE,
        CHAR_BUY_MOVE,
    }

    [Serializable]
    public class SecretQuestInfo
    {
        public UISprite optionSpr;
        public UILabel optionLabel;
    }

    public class PopupData
    {
        public string strTitle;
        public string strText;
        public string strOKLabel;
        public string strYesLabel;
        public string strNoLabel;
        public eGOODSTYPE eGoodsType;
        public long lGoodsCount;
        public bool bCheck;
        public OnClickOKCallBack CallBackOK;
        public OnClickOKCallBack CallBackYes;
        public OnClickOKCallBack CallBackNo;
        public int Type = 0;
        public long lItemUID;
        public long lItemCount;
        public PopupData(string title, string text, string okbtn, eGOODSTYPE etype, long count, OnClickOKCallBack callbackok)
        {
            Type = 0;
            strTitle = title;
            strText = text;
            strOKLabel = okbtn;
            strYesLabel = "";
            strNoLabel = "";
            eGoodsType = etype;
            lGoodsCount = count;
            bCheck = false;
            CallBackOK = callbackok;
            CallBackYes = null;
            CallBackNo = null;
        }
        public PopupData(string title, string text, string yesbtn, string nobtn, eGOODSTYPE etype, long count, bool check, OnClickOKCallBack callbackyes, OnClickOKCallBack callbackno)
        {
            Type = 1;
            strTitle = title;
            strText = text;
            strOKLabel = "";
            strYesLabel = yesbtn;
            strNoLabel = nobtn;
            eGoodsType = etype;
            lGoodsCount = count;
            bCheck = check;
            CallBackOK = null;
            CallBackYes = callbackyes;
            CallBackNo = callbackno;
        }

        public PopupData(string title, string text, string yesbtn, string nobtn, long itemuid, long count, bool check, OnClickOKCallBack callbackyes, OnClickOKCallBack callbackno)
        {
            Type = 1;
            strTitle = title;
            strText = text;
            strOKLabel = "";
            strYesLabel = yesbtn;
            strNoLabel = nobtn;
            eGoodsType = eGOODSTYPE.NONE;
            lGoodsCount = 0;
            bCheck = check;
            CallBackOK = null;
            CallBackYes = callbackyes;
            CallBackNo = callbackno;

            lItemUID = itemuid;
            lItemCount = count;
        }
    }

    public delegate void OnClickOKCallBack();
    private OnClickOKCallBack CallBackOK;
    private OnClickOKCallBack CallBackYes;
    private OnClickOKCallBack CallBackNo;
    private OnClickOKCallBack CallBackClose;
    private OnClickOKCallBack CallBackCharBuyMove;
    private List<PopupData> PopupList = new List<PopupData>();

    public UILabel kTitle;
    public UILabel kText;
    public UILabel kText02;
    public UIGoodsUnit kGoodsUnit;
    public UIButton kOKBtn;
    public UIButton kYesBtn;
    public UIButton kNoBtn;
    public UILabel kOKLabel;
    public UILabel kYesLabel;
    public UILabel kNoLabel;
    public FToggle kCheckToggle;
    public GameObject kCloseBtn;
    public GameObject kScreenShotBtn;
    //아이템
    public GameObject kItemInfoObj;
    public UITexture kItemInfoIconTex;
    public UILabel kItemInfoCntLabel;
    public UIRewardListSlot kRewardSlot;

    //시나리오 모드
    [Header("Scenario")]
    public UITextList kScenarioTextList;

    //Pause 에서 사용될 Root
    [Header("Pause Root")]
    public GameObject kRoot;

    //InApp관련
    [Header("InApp")]
    public GameObject               kBuyBtn;
    public GameObject               kWebCashPaymentBtn;
    public GameObject               kWebSpecifiedBtn;
    public UILabel                  kPriceLabel;
    public GameObject               kSaleObj;
    public UILabel                  kOriginPriceLabel;
    private OnClickOKCallBack       CallBack_IAPSuccess;
    private OnClickOKCallBack       CallBack_IAPFailed;
    private GameTable.Store.Param   _storeItem;

    //마찰 최소화 관련
    [Header("ItemBuyCash")]
    public GameObject kItemBuyCashBtn;
    public UIGoodsUnit kItemBuyCashGoodsUnit;
    public UILabel kItemBuyCashLabel;

    public UIButton kCharBuyBtn;

    [Header("Awaken Skill Point")]
    public GameObject   kWpGetObj;
    public UILabel      kWpGetValueLabel;
    public UIButton     WpResetTicketBtn;
    public UILabel      WpResetTicketLabel;

    [Header("Secret Quest")]
    public GameObject kSecretQuestObj;
    public List<SecretQuestInfo> kSecretQuestInfoList;

    [Header("Add Gem UR Grade")]
    [SerializeField] private GameObject gemObj = null;
    [SerializeField] private UIButton gemUpConfirmBtn = null;
    [SerializeField] private UISprite questionSpr = null;
    [SerializeField] private UISprite gemTypeSpr = null;
    [SerializeField] private UIRewardListSlot rewardListSlot = null;
    [SerializeField] private UILabel gemLabel = null;

    [Header( "Event Gacha" )]
    [SerializeField] private FToggle _EventGachaToggle;

	[Header( "Gem Option Auto" )]
	[SerializeField] private UISprite _AutoSpr;

    [Header("Friend Delete Info")]
    [SerializeField] private GameObject _FriendDeleteInfoObj;
    [SerializeField] private FList _FriendFList;
    private List<FriendUserData> _DeleteFriendUserDataList;

    //ScreenShot관련
    private GameScreenShot _gameScreenShot = null;

    public bool IgnoreBGBtn { get; set; } = false;

    private eMsgResultType _msgResultType = eMsgResultType.NONE;
    public eMsgResultType MsgResultType { get { return _msgResultType; } }

    //TimeScale 0일떄
    private bool m_Anim = true;
    public override void Awake()
    {
        if(kCheckToggle != null)
            kCheckToggle.EventCallBack = OnCheckToggleSelect;

        base.Awake();
    }
    public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		base.OnDisable();
        m_Anim = true;
        IgnoreBGBtn = false;
        MessagePopup.IsActive = false;
    }

    public override void SetUIActive(bool bActive, bool bAnimation = true)
    {
        base.SetUIActive(bActive, bAnimation);
        if(!bAnimation)
        {
            if (kRoot != null)
                kRoot.transform.localScale = Vector3.one;
        }

        MessagePopup.IsActive = bActive;
    }

    private void Init()
    {
        _msgResultType = eMsgResultType.NONE;
        _storeItem = null;
        kOKBtn.gameObject.SetActive(false);
        kYesBtn.gameObject.SetActive(false);
        kNoBtn.gameObject.SetActive(false);

        if (kGoodsUnit != null)
            kGoodsUnit.gameObject.SetActive(false);
        if (kCheckToggle != null)
            kCheckToggle.transform.parent.gameObject.SetActive(false);
        if (kCloseBtn != null)
            kCloseBtn.SetActive(false);
        if (kScenarioTextList != null)
            kScenarioTextList.gameObject.SetActive(false);

        if (kItemInfoObj != null)
            kItemInfoObj.SetActive(false);

        if (kBuyBtn != null)
            kBuyBtn.SetActive(false);
        if (kWebCashPaymentBtn != null)
            kWebCashPaymentBtn.SetActive(false);
        if (kWebSpecifiedBtn != null)
            kWebSpecifiedBtn.SetActive(false);
        if (kSaleObj != null)
            kSaleObj.SetActive(false);

        if (kScreenShotBtn != null)
            kScreenShotBtn.SetActive(false);

        if (kItemBuyCashBtn != null)
            kItemBuyCashBtn.SetActive(false);
        
        if (kCharBuyBtn != null)
            kCharBuyBtn.gameObject.SetActive(false);

        if (kWpGetObj != null)
            kWpGetObj.SetActive(false);

        if ( WpResetTicketBtn != null ) {
            WpResetTicketBtn.SetActive( false );
		}

        if (kSecretQuestObj != null)
            kSecretQuestObj.SetActive(false);

        if (kRewardSlot != null)
            kRewardSlot.SetActive(false);

        if (kText02 != null)
            kText02.SetActive(false);

        if (gemObj != null)
        {
            gemObj.SetActive(false);
        }

        if ( _EventGachaToggle != null ) {
			_EventGachaToggle.gameObject.SetActive( false );
		}

		if ( _AutoSpr != null ) {
			_AutoSpr.SetActive( false );
		}

        if (_FriendDeleteInfoObj != null) {
            _FriendDeleteInfoObj.SetActive(false);
        }

        CallBackCharBuyMove = null;
    }

    public void InitMessagePopupOK(string title, string text, string okbtn, eGOODSTYPE etype, long count, OnClickOKCallBack callbackok, bool bAni = true)
    {
        if (this.gameObject.activeSelf)
        {
            PopupList.Add(new PopupData(title, text, okbtn, etype, count, callbackok));
            return;
        }

        m_Anim = bAni;

        //  메세지 스택을 위한 쇼팝업
        MessagePopup.ShowMessagePopup(m_Anim);

        //SetUIActive(true);
        kTitle.textlocalize = title;
        kText.textlocalize = text;
        kOKLabel.textlocalize = okbtn;

        Init();
        kOKBtn.gameObject.SetActive(true);
        kText.gameObject.SetActive(true);
        CallBackOK = callbackok;
        CallBackYes = null;
        CallBackNo = null;
        CallBackClose = null;
        if (etype != eGOODSTYPE.NONE)
            SetGoodsUnit(etype, count);
    }
    public void InitMessagePopupOK(string title, string text, string okbtn, eGOODSTYPE etype, long count, OnClickOKCallBack callbackok, OnClickOKCallBack callbackno, bool bAni = true)
    {
        InitMessagePopupOK(title, text, okbtn, etype, count, callbackok);
        CallBackNo = callbackno;
    }

    public void InitBoSetMessagePopupOK(string title, int stageId, string okbtn, eGOODSTYPE etype, long count, OnClickOKCallBack callbackok)
    {
        InitMessagePopupOK(title, string.Empty, okbtn, etype, count, callbackok);
        kSecretQuestObj.SetActive(true);
        
        SecretQuestOptionData optionData = GameInfo.Instance.ServerData.SecretQuestOptionList.Find(x => x.GroupId == stageId);
        List<GameClientTable.StageBOSet.Param> stageBoSetList =
            GameInfo.Instance.GameClientTable.FindAllStageBOSet(x => x.Group == optionData.BoSetId);
        for (int i = 0; i < kSecretQuestInfoList.Count; i++)
        {
            int descId = 0;
            string spriteName = string.Empty;
            if (i < stageBoSetList.Count)
            {
                descId = stageBoSetList[i].Desc;
                spriteName = stageBoSetList[i].Icon;
            }

            kSecretQuestInfoList[i].optionSpr.spriteName = spriteName;
            kSecretQuestInfoList[i].optionSpr.SetActive(!string.IsNullOrEmpty(spriteName));
            
            kSecretQuestInfoList[i].optionLabel.textlocalize = FLocalizeString.Instance.GetText(descId);
            kSecretQuestInfoList[i].optionLabel.SetActive(0 < descId);
        }

        CallBackNo = null;
    }

    public void InitUseWpResetTicketPopup( string title, string text, int wpValue, long itemUid, OnClickOKCallBack callback ) {
        if ( gameObject.activeSelf ) {
            PopupList.Add( new PopupData( title, text, "", "", itemUid, 1, false, callback, null ) );
            return;
        }

        m_Anim = true;

        //  메세지 스택을 위한 쇼팝업
        MessagePopup.ShowMessagePopup( m_Anim );

        kTitle.textlocalize = title;
        kText.textlocalize = text;

        Init();
        
        kText.gameObject.SetActive( true );
        kWpGetObj.SetActive( true );
        WpResetTicketBtn.SetActive( true );
        kCloseBtn.SetActive( true );

        kWpGetValueLabel.textlocalize = wpValue.ToString();
        WpResetTicketLabel.textlocalize = "x1";

        CallBackOK = callback;
        CallBackYes = null;
        CallBackNo = null;
        CallBackClose = null;
    }

    public void InitMessagePopupYN(string title, string text, string yesbtn, string nobtn, eGOODSTYPE etype, long count, bool check, OnClickOKCallBack callbackyes, OnClickOKCallBack callbackno, bool bAni = true, bool includeCloseBtn = false, int wpValue = -1)
    {
        if (this.gameObject.activeSelf)
        {
            PopupList.Add(new PopupData(title, text, yesbtn, nobtn, etype, count, check, callbackyes, callbackno));
            return;
        }
    
        m_Anim = bAni;
    
        //  메세지 스택을 위한 쇼팝업
        MessagePopup.ShowMessagePopup(bAni);
        //SetUIActive(true);
        kTitle.textlocalize = title;
        kText.textlocalize = text;
        kYesLabel.textlocalize = yesbtn;
        kNoLabel.textlocalize = nobtn;
    
        Init();
    
        kYesBtn.gameObject.SetActive(true);
        kNoBtn.gameObject.SetActive(true);
        kText.gameObject.SetActive(true);
    
        if (kCloseBtn != null)
        {
            if (includeCloseBtn)
            {
                kCloseBtn.SetActive(true);
            }
            else
            {
                kCloseBtn.SetActive(false);
            }
        }
    
        CallBackClose = null;
    
        CallBackOK = null;
        CallBackYes = callbackyes;
        CallBackNo = callbackno;
        if (etype != eGOODSTYPE.NONE)
            SetGoodsUnit(etype, count);
    
        if (kCheckToggle != null)
        {
            if(check)
            {
                kCheckToggle.transform.parent.gameObject.SetActive(true);
                kCheckToggle.SetToggle(1, SelectEvent.Enable);
            }
        }

        if (0 <= wpValue)
        {
            kWpGetObj.SetActive(true);
            kWpGetValueLabel.textlocalize = wpValue.ToString();
        }
    }
    
    public void InitMessagePopupYN(string title, string text, string yesbtn, string nobtn, eGOODSTYPE etype, long count, bool check, OnClickOKCallBack callbackyes, OnClickOKCallBack callbackno, OnClickOKCallBack callbackCharBuyMove, bool bAni = true, bool includeCloseBtn = false)
    {
        InitMessagePopupYN(title, text, yesbtn, nobtn, etype, count, check, callbackyes, callbackno, bAni, includeCloseBtn);

        CallBackCharBuyMove = callbackCharBuyMove;

        if (kCharBuyBtn != null)
        {
            kCharBuyBtn.SetActive(callbackCharBuyMove != null);
        }
    }

	public void InitMessagePopupYNAutoGemOption( string title, string text, string yesbtn, string nobtn, eGOODSTYPE etype, long count, bool check, OnClickOKCallBack callbackyes, OnClickOKCallBack callbackno, bool bAni = true, bool includeCloseBtn = false ) {
		InitMessagePopupYN( title, text, yesbtn, nobtn, etype, count, check, callbackyes, callbackno, bAni, includeCloseBtn );

		_AutoSpr.SetActive( true );
	}

	public void InitMessagePopupYNEventGacha( string title, string text, string yesbtn, string nobtn, eGOODSTYPE etype, long count, bool check, OnClickOKCallBack callbackyes, OnClickOKCallBack callbackno, OnEventTabSelect callbackTabSelect, bool bAni = true, bool includeCloseBtn = false ) {
		InitMessagePopupYN( title, text, yesbtn, nobtn, etype, count, check, callbackyes, callbackno, bAni, includeCloseBtn );

		_EventGachaToggle.gameObject.SetActive( true );
		_EventGachaToggle.EventCallBack = callbackTabSelect;
		_EventGachaToggle.SetToggle( (int)eToggleType.On, SelectEvent.Code );
	}

	public void InitMessageItemPopupYN(string title, string text, string yesbtn, string nobtn, long itemuid, long count, bool check, OnClickOKCallBack callbackyes, OnClickOKCallBack callbackno)
    {
        if(this.gameObject.activeSelf)
        {
            PopupList.Add(new PopupData(title, text, yesbtn, nobtn, itemuid, count, check, callbackyes, callbackno));
            return;
        }

        //  메세지 스택을 위한 쇼팝업
        MessagePopup.ShowMessagePopup();

        kTitle.textlocalize = title;
        kText.textlocalize = text;
        kYesLabel.textlocalize = yesbtn;
        kNoLabel.textlocalize = nobtn;

        Init();

        kYesBtn.gameObject.SetActive(true);
        kNoBtn.gameObject.SetActive(true);
        kText.gameObject.SetActive(true);
        CallBackClose = null;

        CallBackOK = null;
        CallBackYes = callbackyes;
        CallBackNo = callbackno;

        if (kItemInfoObj != null)
        {
            kItemInfoObj.SetActive(true);
            var itemdata = GameInfo.Instance.GameTable.FindItem((int)itemuid);
            if (itemdata != null)
            {
                kItemInfoIconTex.mainTexture = (Texture2D)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + itemdata.Icon);
                int userItemCnt = GameInfo.Instance.GetItemIDCount((int)itemuid);

                string itemCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.WHITE_TEXT_COLOR);

                if (userItemCnt < count)
                {
                    itemCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.RED_TEXT_COLOR);
                }

                kItemInfoCntLabel.textlocalize = string.Format(itemCntColor, string.Format("{0}/{1}", userItemCnt, count));
            }
        }

        if (kCheckToggle != null)
        {
            if (check)
            {
                kCheckToggle.transform.parent.gameObject.SetActive(true);
                kCheckToggle.SetToggle(1, SelectEvent.Enable);
            }
        }
    }

    public void InitMessagePopupTextLong(string title, string text, OnClickOKCallBack callbackclose = null)
    {
        MessagePopup.ShowMessagePopup();

        CallBackClose = callbackclose;

        Init();

        kText.gameObject.SetActive(false);

        kCloseBtn.SetActive(true);
        kScenarioTextList.scrollValue = 0f;
        kScenarioTextList.gameObject.SetActive(true);
        

        kTitle.textlocalize = title;
        kScenarioTextList.textLabel.textlocalize = "";
        kScenarioTextList.Clear();
        kScenarioTextList.Add(text);
    }

    public void InitIAPMessagePopup(string title, string text, GameTable.Store.Param storeItem, OnClickOKCallBack callbackIAPSuccess, OnClickOKCallBack callbackIAPFailed)
    {
        MessagePopup.ShowMessagePopup();

        Init();

        _storeItem = storeItem;

        CallBack_IAPSuccess = callbackIAPSuccess;
        CallBack_IAPFailed = callbackIAPFailed;

        kText.gameObject.SetActive(true);
        if (kCloseBtn != null)
            kCloseBtn.SetActive(true);
        if (kBuyBtn != null)
            kBuyBtn.SetActive(true);
        if (kWebSpecifiedBtn != null)
        {
            if(AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Japan)
                kWebSpecifiedBtn.SetActive(true);
        }
        if (kWebCashPaymentBtn != null)
        {
            if (AppMgr.Instance.configData.m_ServiceType == Config.eServiceType.Japan)
                kWebCashPaymentBtn.SetActive(true);
        }
        if (kPriceLabel != null)
        {
            kPriceLabel.gameObject.SetActive(true);
#if UNITY_ANDROID
            kPriceLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(270), IAPManager.Instance.GetPrice(_storeItem.AOS_ID));
#elif UNITY_IOS
            kPriceLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(270), IAPManager.Instance.GetPrice(_storeItem.IOS_ID));
#elif !DISABLESTEAMWORKS
            kPriceLabel.textlocalize = string.Format("${0:0.##}", (float)_storeItem.PurchaseValue * 0.01f);
#endif
        }

        //할인율
        if (kSaleObj != null) {
            GameClientTable.StoreDisplayGoods.Param clientTable = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == _storeItem.ID);
            if (clientTable == null)
                return;

            if (clientTable.OriginalPriceID > (int)eCOUNT.NONE) {
                GameTable.Store.Param storedata = GameInfo.Instance.GameTable.FindStore(x => x.ID == clientTable.OriginalPriceID);
                if (storedata != null) {
                    kSaleObj.SetActive(true);
#if UNITY_ANDROID
                    kOriginPriceLabel.textlocalize = IAPManager.Instance.GetPrice(storedata.AOS_ID);
#elif UNITY_IOS
                    kOriginPriceLabel.textlocalize = IAPManager.Instance.GetPrice(storedata.IOS_ID);
#elif !DISABLESTEAMWORKS
                    kOriginPriceLabel.textlocalize = string.Format("${0:0.##}", (float)storedata.PurchaseValue * 0.01f);
#endif
                }
            }

        }

        kTitle.textlocalize = title;
        kText.textlocalize = text;
    }

    public void InitScreenShotPopup(string title, string text, OnClickOKCallBack callbackIAPSuccess)
    {
        MessagePopup.ShowMessagePopup();

        Init();

        kText.gameObject.SetActive(true);

        kTitle.textlocalize = title;
        kText.textlocalize = text;
        if(kCloseBtn != null)
            kCloseBtn.gameObject.SetActive(true);
        if (kScreenShotBtn != null)
            kScreenShotBtn.SetActive(true);

        if (_gameScreenShot == null)
            _gameScreenShot = this.gameObject.AddComponent<GameScreenShot>();

    }

    public void InitMessageItemBuyCashPopupYN(string title, string text, string yesbtn, string nobtn, long itemuid, long count, bool check, OnClickOKCallBack callbackyes, OnClickOKCallBack callbackno)
    {
        if (this.gameObject.activeSelf)
        {
            PopupList.Add(new PopupData(title, text, yesbtn, nobtn, itemuid, count, check, callbackyes, callbackno));
            return;
        }

        //  메세지 스택을 위한 쇼팝업
        MessagePopup.ShowMessagePopup();

        kTitle.textlocalize = title;
        kText.textlocalize = text;
        kItemBuyCashLabel.textlocalize = yesbtn;
        kNoLabel.textlocalize = nobtn;

        Init();

        kItemBuyCashBtn.SetActive(true);
        kNoBtn.gameObject.SetActive(true);
        kText.gameObject.SetActive(true);
        CallBackClose = null;

        CallBackOK = null;
        CallBackYes = callbackyes;
        CallBackNo = callbackno;

        if (kItemInfoObj != null)
        {
            kItemInfoObj.SetActive(true);
            var itemdata = GameInfo.Instance.GameTable.FindItem((int)itemuid);
            if (itemdata != null)
            {
                kItemInfoIconTex.mainTexture = (Texture2D)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + itemdata.Icon);
                int userItemCnt = GameInfo.Instance.GetItemIDCount((int)itemuid);

                string itemCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.WHITE_TEXT_COLOR);

                if (userItemCnt < count)
                {
                    itemCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.RED_TEXT_COLOR);
                }

                kItemInfoCntLabel.textlocalize = string.Format(itemCntColor, string.Format("{0}/{1}", userItemCnt, count));

                float needCash = count * itemdata.CashExchange;
                Log.Show(needCash);
                int roundNeedCash = Mathf.CeilToInt(needCash);
                Log.Show(roundNeedCash, Log.ColorType.Red);

                if (kItemBuyCashGoodsUnit != null)
                    kItemBuyCashGoodsUnit.InitGoodsUnit(eGOODSTYPE.CASH, roundNeedCash, true);
            }
        }

        if (kCheckToggle != null)
        {
            if (check)
            {
                kCheckToggle.transform.parent.gameObject.SetActive(true);
                kCheckToggle.SetToggle(1, SelectEvent.Enable);
            }
        }
    }

    public void InitMessagePopupRewardYN(string pTitle, string pDesc, RewardData pRewardData, eREWARDTYPE pConsumType, int pConsumIndex, long pConsumCount, string pYesTitle, string pNoTitle, UIMessagePopup.OnClickOKCallBack pYesCallback, UIMessagePopup.OnClickOKCallBack pNoCallback)
    {
        if (pConsumType == eREWARDTYPE.GOODS)
        {
            InitMessagePopupYN(pTitle, pDesc, pYesTitle, pNoTitle, (eGOODSTYPE)pConsumIndex, pConsumCount, false, pYesCallback, pNoCallback);

            kGoodsUnit.InitGoodsUnit((eGOODSTYPE)pConsumIndex, pConsumCount, true);
        }
        else
        {
            InitMessagePopupYN(pTitle, pDesc, pYesTitle, pNoTitle, eGOODSTYPE.NONE, pConsumCount, false, pYesCallback, pNoCallback);

            if (kItemInfoObj != null)
            {
                kItemInfoObj.SetActive(true);

                string iconNameStr = string.Empty;
                switch (pConsumType)
                {
                    case eREWARDTYPE.WEAPON:
                        {
                            GameTable.Weapon.Param param = GameInfo.Instance.GameTable.FindWeapon(pConsumIndex);
                            if (param != null)
                            {
                                iconNameStr = param.Icon;
                            }
                            break;
                        }
                    case eREWARDTYPE.GEM:
                        {
                            GameTable.Gem.Param param = GameInfo.Instance.GameTable.FindGem(pConsumIndex);
                            if (param != null)
                            {
                                iconNameStr = param.Icon;
                            }
                            break;
                        }
                    case eREWARDTYPE.CARD:
                        {
                            GameTable.Card.Param param = GameInfo.Instance.GameTable.FindCard(pConsumIndex);
                            if (param != null)
                            {
                                iconNameStr = param.Icon;
                            }
                            break;
                        }
                    case eREWARDTYPE.ITEM:
                        {
                            GameTable.Item.Param param = GameInfo.Instance.GameTable.FindItem(pConsumIndex);
                            if (param != null)
                            {
                                iconNameStr = param.Icon;
                            }
                            break;
                        }
                }

                if (System.Text.RegularExpressions.Regex.Match(iconNameStr, ".png$").Success == false)
                {
                    // ".png" not Found in iconNameStr
                    System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
                    stringBuilder.Append(iconNameStr).Append(".png");
                    iconNameStr = stringBuilder.ToString();
                }
                
                kItemInfoIconTex.mainTexture = (Texture2D)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + iconNameStr);

                long lInvenCount = LobbyUIManager.Instance.GetInvenCount(pConsumType, pConsumIndex);
                string itemCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.WHITE_TEXT_COLOR);
                if (lInvenCount < pConsumCount)
                {
                    itemCntColor = FLocalizeString.Instance.GetText((int)eTEXTID.RED_TEXT_COLOR);
                }

                kItemInfoCntLabel.textlocalize = string.Format(itemCntColor, string.Format("{0}/{1}", lInvenCount, pConsumCount));
            }
        }

        kText.textlocalize = string.Empty;
        kText02.SetActive(true);
        kText02.textlocalize = pDesc;
        
        kRewardSlot.SetActive(true);
        kRewardSlot.UpdateSlot(pRewardData, false);
    }

    public void InitMessagePopupGemAnalyzedYN(string title, string gemSpriteName, UIMessagePopup.OnClickOKCallBack yesCallback)
    {
        Init();
        kText.SetActive(false);

        kYesLabel.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.YES);
        kNoLabel.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.NO);

        gemObj.SetActive(true);

        kTitle.textlocalize = title;
        CallBackYes = yesCallback;
        CallBackNo = null;

        bool isQuestion = string.IsNullOrEmpty(gemSpriteName);
        questionSpr.SetActive(isQuestion);
        rewardListSlot.SetActive(isQuestion);
        kYesBtn.SetActive(isQuestion);
        kNoBtn.SetActive(isQuestion);

        gemTypeSpr.SetActive(!isQuestion);
        gemUpConfirmBtn.SetActive(!isQuestion);

        if (isQuestion)
        {
            gemLabel.textlocalize = FLocalizeString.Instance.GetText(3310);
            if (rewardListSlot.ParentGO == null)
            {
                rewardListSlot.ParentGO = this.gameObject;
            }
            rewardListSlot.UpdateSlot(new RewardData(GameInfo.Instance.GameConfig.GemAnalyzeCostType, GameInfo.Instance.GameConfig.GemAnalyzeCostIndex, GameInfo.Instance.GameConfig.GemAnalyzeCostValue), true);

            int costIndex = GameInfo.Instance.GameConfig.GemAnalyzeCostIndex;
            GameTable.Item.Param itemTableParam = GameInfo.Instance.GameTable.FindItem(costIndex);

            int haveItemCount = 0;
            ItemData itemData = GameInfo.Instance.ItemList.Find(x => x.TableID == costIndex);
            if (itemData != null)
            {
                haveItemCount = itemData.Count;
            }

            int costValue = GameInfo.Instance.GameConfig.GemAnalyzeCostValue;
            bool isBuy = costValue <= haveItemCount;
            eTEXTID textId = isBuy ? eTEXTID.WHITE_TEXT_COLOR : eTEXTID.RED_TEXT_COLOR;
            rewardListSlot.SetCountLabel(FLocalizeString.Instance.GetText((int)textId, $"{haveItemCount} / {costValue}"));
        }
        else
        {
            gemLabel.textlocalize = FLocalizeString.Instance.GetText(3311);
            gemTypeSpr.spriteName = gemSpriteName;
        }
    }


    public void InitMessagePopupFriendDeleteYN(List<FriendUserData> deleteFriendUserDataList, UIMessagePopup.OnClickOKCallBack yesCallback) {
        MessagePopup.ShowMessagePopup();

        Init();

        kText.SetActive(false);

        kYesLabel.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.OK);
        kNoLabel.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.CANCEL);

        _FriendDeleteInfoObj.SetActive(true);

        kTitle.textlocalize = FLocalizeString.Instance.GetText(1532);
        CallBackYes = yesCallback;
        CallBackNo = null;

        kYesBtn.gameObject.SetActive(true);
        kNoBtn.gameObject.SetActive(true);

        //스크롤뷰 생성 및 자식 오브젝트 셋팅
        _DeleteFriendUserDataList = deleteFriendUserDataList;

        if (_FriendFList != null) {
            _FriendFList.EventUpdate = OnEventFriendFListUpdate;
            _FriendFList.EventGetItemCount = OnEventFriendFListGetItemCount;
            _FriendFList.UpdateList();
        }
    }

    private void OnEventFriendFListUpdate(int index, GameObject obj) {
        UIFriendDeleteSlot slot = obj.GetComponent<UIFriendDeleteSlot>();
        if (slot == null) {
            return;
        }

        if (slot.ParentGO == null) {
            slot.ParentGO = this.gameObject;
        }

        FriendUserData data = null;
        if (0 <= index && index < _DeleteFriendUserDataList.Count) {
            data = _DeleteFriendUserDataList[index];
        }

        slot.UpdateSlot(index, data);
    }
    private int OnEventFriendFListGetItemCount() {
        return _DeleteFriendUserDataList.Count;
    }



    private bool OnCheckToggleSelect(int nSelect, SelectEvent type)
    {
        return true;
    }

    public void SetGoodsUnit(eGOODSTYPE etype, long count)
    {
        if (kGoodsUnit != null)
        {
            kGoodsUnit.gameObject.SetActive(true);
            kGoodsUnit.InitGoodsUnit(etype, count);
        }
    }

    public void OnClickOK()
    {
        _msgResultType = eMsgResultType.OK;
        SetUIActive(false, m_Anim);
        if (CallBackOK != null)
            CallBackOK();
    }
    public void OnClickYes()
    {
        _msgResultType = eMsgResultType.YES;
        SetUIActive(false, m_Anim);
        if (CallBackYes != null)
            CallBackYes();
    }
    public void OnClickNo()
    {
        _msgResultType = eMsgResultType.NO;
        SetUIActive(false, m_Anim);
        if (CallBackNo != null)
            CallBackNo();
    }
    public void OnClickBG()
    {
        if(IgnoreBGBtn)
        {
            return;
        }

        if (kOKBtn.gameObject.activeSelf)
        {
            _msgResultType = eMsgResultType.OK;
            SetUIActive(false, m_Anim);
            if (CallBackOK != null)
                CallBackOK();
        }
        else
        {
            _msgResultType = eMsgResultType.NO;
            SetUIActive(false, m_Anim);
            if (CallBackNo != null)
                CallBackNo();
        }
    }
    public override void OnClickClose()
    {
        _msgResultType = eMsgResultType.CANCEL;
        SetUIActive(false, m_Anim);
        if (CallBackClose != null)
            CallBackClose();
    }
    
    public void OnClickCharBuyMove()
    {
        _msgResultType = eMsgResultType.CHAR_BUY_MOVE;
        SetUIActive(false, m_Anim);
        if (CallBackCharBuyMove != null)
        {
            CallBackCharBuyMove();
        }
    }
    
    public override void OnClose()
    {
        _msgResultType = eMsgResultType.CLOSE;
        base.OnClose();
        ShowNextPopup();
    }

    public void ShowNextPopup()
    {
        if (PopupList.Count == 0)
            return;

        PopupData data = PopupList[0];

        if (data.Type == 0)
        {
            InitMessagePopupOK(data.strTitle, data.strText, data.strOKLabel, data.eGoodsType, data.lGoodsCount, data.CallBackOK);
        }
        else
        {
            InitMessagePopupYN(data.strTitle, data.strText, data.strYesLabel, data.strNoLabel, data.eGoodsType, data.lGoodsCount, data.bCheck, data.CallBackYes, data.CallBackNo);
        }

        PopupList.Remove(data);
    }

    public void OnClick_WebCashPaymentBtn()
    {
        if (string.IsNullOrEmpty(GameInfo.Instance.GameConfig.WebCashpaymentmethod))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3001));
        }
        else
        {
            GameSupport.OpenWebView(FLocalizeString.Instance.GetText(504), GameInfo.Instance.GameConfig.WebCashpaymentmethod);
        }
    }

    public void OnClick_WebSpecifiedBtn()
    {
        if (string.IsNullOrEmpty(GameInfo.Instance.GameConfig.WebSpecifiedCommercialTransactionsAct))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3001));
        }
        else
        {
            GameSupport.OpenWebView(FLocalizeString.Instance.GetText(505), GameInfo.Instance.GameConfig.WebSpecifiedCommercialTransactionsAct);
        }
    }

    public void OnClick_IAPBtn()
    {
        if(_storeItem == null)
        {
            SetUIActive(false, m_Anim);
            return;
        }

        string buyStoreID = string.Empty;
#if UNITY_ANDROID
        buyStoreID = _storeItem.AOS_ID;
#elif UNITY_IOS
        buyStoreID = _storeItem.IOS_ID;
#elif !DISABLESTEAMWORKS
        buyStoreID = _storeItem.ID.ToString();
#endif
        Log.Show(buyStoreID);
        if (string.IsNullOrEmpty(buyStoreID))
            return;
        WaitPopup.Show();
        IAPManager.Instance.IsBuying = true;
        IAPManager.Instance.BuyIAPProduct(buyStoreID, 
            () => {
#if !DISABLESTEAMWORKS
                GameInfo.Instance.Send_ReqSteamPurchase(_storeItem.ID, OnNet_StorePurchaseInApp);
#else
                GameInfo.Instance.Send_ReqStorePurchaseInApp(IAPManager.Instance.Receipt, _storeItem.ID, OnNet_StorePurchaseInApp);
#endif
            }, 
            () => {
                WaitPopup.Hide();
                IAPManager.Instance.IsBuying = false;
                if (CallBack_IAPFailed != null)
                    CallBack_IAPFailed();
                SetUIActive(false, m_Anim);
            });
    }

    public void OnNet_StorePurchaseInApp(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        if (CallBack_IAPSuccess != null)
            CallBack_IAPSuccess();
        SetUIActive(false, m_Anim);
    }

    public void OnNet_SteamPurchaseInApp(int result, PktMsgType pktmsg)
    {

    }

    public void OnClick_ScreenShotBtn()
    {
        if(_gameScreenShot == null)
        {
            SetUIActive(false, m_Anim);
            return;
        }

        _gameScreenShot.ScreenShotSaveToAlbum(
            () => {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(1422));
            }
            );
    }
}

