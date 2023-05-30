using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MessageTimerPopup
{
    public static UIMessageTimerPopup GetMessagePopup()
    {
        UIMessageTimerPopup mpopup = null;
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
            mpopup = LobbyUIManager.Instance.GetUI<UIMessageTimerPopup>("MessageTimerPopup");
        else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage || AppMgr.Instance.SceneType == AppMgr.eSceneType.Training)
            mpopup = GameUIManager.Instance.GetUI<UIMessageTimerPopup>("MessageTimerPopup");
        else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
            mpopup = TitleUIManager.Instance.GetUI<UIMessageTimerPopup>("MessageTimerPopup");
        else
        {
            GameObject uiroot = GameObject.FindObjectOfType<UIRoot>().gameObject;
            mpopup = uiroot.transform.Find("MessageTimerPopup").GetComponent<UIMessageTimerPopup>();
        }
        return mpopup;
    }

    public static void ShowMessagePopup(bool bAni = true)
    {
		if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
			LobbyUIManager.Instance.ShowUI("MessageTimerPopup", true);
		else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage || AppMgr.Instance.SceneType == AppMgr.eSceneType.Training)
		{
			GameUIManager.Instance.ShowUI("MessageTimerPopup", bAni);
		}
		else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Title)
			TitleUIManager.Instance.ShowUI("MessageTimerPopup", true);
		else
		{
			GameObject uiroot = GameObject.FindObjectOfType<UIRoot>().gameObject;
			UIMessageTimerPopup mpopup = uiroot.transform.Find("MessageTimerPopup").GetComponent<UIMessageTimerPopup>();
			mpopup.SetUIActive(true, bAni);
		}
    }

    public static void ClosePopup()
    {
        UIMessageTimerPopup popup = GetMessagePopup();
        if(popup == null || !popup.gameObject.activeSelf)
        {
            return;
        }

        popup.OnClose();
    }

    public static void YN(eTEXTID title, int text, UIMessageTimerPopup.OnClickOKCallBack callbackyes, UIMessageTimerPopup.OnClickOKCallBack callbackno = null, float timer = 0f)
    {
        UIMessageTimerPopup mpopup = GetMessagePopup();
        if (mpopup == null)
            return;
        mpopup.InitMessagePopupYN(FLocalizeString.Instance.GetText((int)title), FLocalizeString.Instance.GetText(text), FLocalizeString.Instance.GetText((int)eTEXTID.YES), FLocalizeString.Instance.GetText((int)eTEXTID.NO), false, callbackyes, callbackno, true, timer);
    }

    public static void YN(eTEXTID title, string text, UIMessageTimerPopup.OnClickOKCallBack callbackyes, UIMessageTimerPopup.OnClickOKCallBack callbackno = null, float timer = 0f)
    {
        UIMessageTimerPopup mpopup = GetMessagePopup();
        if (mpopup == null)
            return;
        mpopup.InitMessagePopupYN(FLocalizeString.Instance.GetText((int)title), text, FLocalizeString.Instance.GetText((int)eTEXTID.YES), FLocalizeString.Instance.GetText((int)eTEXTID.NO), false, callbackyes, callbackno, true, timer);
    }

    public static void CYN(eTEXTID title, int textid, eTEXTID btnyes, eTEXTID btnno, UIMessageTimerPopup.OnClickOKCallBack callbackyes, UIMessageTimerPopup.OnClickOKCallBack callbackno = null, float timer = 0f)
    {
        UIMessageTimerPopup mpopup = GetMessagePopup();
        if (mpopup == null)
            return;
        mpopup.InitMessagePopupYN(FLocalizeString.Instance.GetText((int)title), FLocalizeString.Instance.GetText(textid), FLocalizeString.Instance.GetText((int)btnyes), FLocalizeString.Instance.GetText((int)btnno), false, callbackyes, callbackno, true, timer);
    }

    public static void CYN(eTEXTID title, string text, eTEXTID btnyes, eTEXTID btnno, UIMessageTimerPopup.OnClickOKCallBack callbackyes, UIMessageTimerPopup.OnClickOKCallBack callbackno = null, float timer = 0f)
    {
        UIMessageTimerPopup mpopup = GetMessagePopup();
        if (mpopup == null)
            return;
        mpopup.InitMessagePopupYN(FLocalizeString.Instance.GetText((int)title), text, FLocalizeString.Instance.GetText((int)btnyes), FLocalizeString.Instance.GetText((int)btnno), false, callbackyes, callbackno, true, timer);
    }

    public static void CYN(string title, string text, string btnyes, string btnno, UIMessageTimerPopup.OnClickOKCallBack callbackyes, UIMessageTimerPopup.OnClickOKCallBack callbackno = null, float timer = 0f)
    {
        UIMessageTimerPopup mpopup = GetMessagePopup();
        if (mpopup == null)
            return;
        mpopup.InitMessagePopupYN(title, text, btnyes, btnno, false, callbackyes, callbackno, true, timer);
    }

    
}



public class UIMessageTimerPopup : FComponent
{
    public class PopupData
    {
        public string strTitle;
        public string strText;
        
        public string strYesLabel;
        public string strNoLabel;                
        public bool bCheck;
        
        public OnClickOKCallBack CallBackYes;
        public OnClickOKCallBack CallBackNo;
        public int Type = 0;

        public float fTimer = 0f;
                
        public PopupData(string title, string text, string yesbtn, string nobtn, bool check, OnClickOKCallBack callbackyes, OnClickOKCallBack callbackno, float timer)
        {
            Type = 1;
            strTitle = title;
            strText = text;
            
            strYesLabel = yesbtn;
            strNoLabel = nobtn;            
            bCheck = check;
            
            CallBackYes = callbackyes;
            CallBackNo = callbackno;

            fTimer = timer;
        }
    }

    public delegate void OnClickOKCallBack();
    
    private OnClickOKCallBack CallBackYes;
    private OnClickOKCallBack CallBackNo;
    private OnClickOKCallBack CallBackClose;
    private List<PopupData> PopupList = new List<PopupData>();

    public UILabel kTitle;
    public UILabel kText;
    public UIButton kYesBtn;
    public UIButton kNoBtn;    
    public UILabel kYesLabel;
    public UILabel kNoLabel;
    
    public bool IgnoreBGBtn { get; set; } = false;

    //TimeScale 0일떄
    private bool m_Anim = true;

    private float fTimer = 0f;
    private string strText = "";

    public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		base.OnDisable();
        m_Anim = true;
        IgnoreBGBtn = false;
    }

    private void Update()
    {
        if(fTimer > 0f)
        {
            fTimer -= Time.deltaTime;
            Update_TextLabel();
            if(fTimer <= 0f)
            {
                OnClickNo();
                fTimer = 0f;                
            }
        }
    }

    private void Update_TextLabel()
    {
        kText.textlocalize = strText + "\n" + string.Format(FLocalizeString.Instance.GetText(263), (int)fTimer);        
    }
    
    private void Init()
    {   
        kYesBtn.gameObject.SetActive(false);
        kNoBtn.gameObject.SetActive(false);
    }

    public void InitMessagePopupYN(string title, string text, string yesbtn, string nobtn, bool check, OnClickOKCallBack callbackyes, OnClickOKCallBack callbackno, bool bAni = true, float timer = 0f)
    {
        if (this.gameObject.activeSelf)
        {
            PopupList.Add(new PopupData(title, text, yesbtn, nobtn, check, callbackyes, callbackno, timer));
            return;
        }

		if((AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage || AppMgr.Instance.SceneType == AppMgr.eSceneType.Training) && World.Instance.IsPause)
		{
			bAni = false;
		}

		m_Anim = bAni;

		//  메세지 스택을 위한 쇼팝업
		MessageTimerPopup.ShowMessagePopup(bAni);
        //SetUIActive(true);
        kTitle.textlocalize = title;        
        kText.textlocalize = text;
        strText = text;
        Update_TextLabel();
        kYesLabel.textlocalize = yesbtn;
        kNoLabel.textlocalize = nobtn;

        Init();

        kYesBtn.gameObject.SetActive(true);
        kNoBtn.gameObject.SetActive(true);
        kText.gameObject.SetActive(true);
                
        CallBackClose = null;

        CallBackYes = callbackyes;
        CallBackNo = callbackno;

        fTimer = timer;        
    }   
    
    public void OnClickYes()
    {
        fTimer = 0f;        

        SetUIActive(false, m_Anim);
        if (CallBackYes != null)
            CallBackYes();
    }
    public void OnClickNo()
    {
        SetUIActive(false, m_Anim);
        if (CallBackNo != null)
            CallBackNo();
    }
    
    public override void OnClickClose()
    {
        SetUIActive(false, m_Anim);
        if (CallBackClose != null)
            CallBackClose();
    }
    public override void OnClose()
    {
        base.OnClose();
        ShowNextPopup();
    }

    public void ShowNextPopup()
    {
        if (PopupList.Count == 0)
            return;

        PopupData data = PopupList[0];
        InitMessagePopupYN(data.strTitle, data.strText, data.strYesLabel, data.strNoLabel, data.bCheck, data.CallBackYes, data.CallBackNo);
        PopupList.Remove(data);
    }
}

