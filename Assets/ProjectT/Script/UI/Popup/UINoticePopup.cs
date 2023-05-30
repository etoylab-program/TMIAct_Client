using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NoticePopup
{
    public static UINoticePopup GetNoticePopup()
    {
        UINoticePopup mpopup = LobbyUIManager.Instance.GetUI<UINoticePopup>("NoticePopup");
        return mpopup;
    }

    public static void ShowMessagePopup()
    {
        LobbyUIManager.Instance.ShowUI("NoticePopup", true);
    }

    public static void ShowText(UINoticePopup.eTYPE type, string text, string res, UINoticePopup.eRUNTYPE runtype, int value1, int value2)
    {
        UINoticePopup mpopup = GetNoticePopup();
        mpopup.InitNoticePopup(type, text, res, runtype, value1, value2);
    }
}

public class UINoticePopup : FComponent
{
    public enum eTYPE
    {
        TEXT = 0,
        SPR,
        TEX,
    }

    public enum eRUNTYPE
    {
        NONE = 0,
        CHARWEAPON,
        CHARSKILL,
        CHARCARD,
    }

    public class PopupData
    {
        public eTYPE Type;
        public string Text;
        public string Res;
        public eRUNTYPE RunType;
        public int Value1;
        public int Value2;

        public PopupData(eTYPE type, string text, string res, eRUNTYPE runtype, int value1, int value2 )
        {
            Type = type;
            Text = text;
            Res = res;
            RunType = runtype;
            Value1 = value1;
            Value2 = value2;
        }
    }
    private List<PopupData> PopupList = new List<PopupData>();

    public UITexture kIconTex;
    public UISprite kIconSpr;
    public UILabel kTextLabel;
    public UIButton kBGBtn;

    private eRUNTYPE _runyype;
    private int _value1;
    private int _value2;
     

    public void InitNoticePopup(eTYPE type, string text, string res, eRUNTYPE runtype, int value1, int value2)
    {
        if (this.gameObject.activeSelf)
        {
            PopupList.Add(new PopupData(type, text, res, runtype, value1, value2));
            return;
        }

        NoticePopup.ShowMessagePopup();

        _runyype = runtype;
        _value1 = value1;
        _value2 = value2;

        kTextLabel.textlocalize = text;
        
        if (UIAni != null)
            UIAni.Stop();
        CancelInvoke("OnClickClose");

        SetUIActive(true);
        kIconTex.gameObject.SetActive(false);
        kIconSpr.gameObject.SetActive(false);
        
        if( type == eTYPE.SPR)
        {
            kIconSpr.gameObject.SetActive(true);
            kIconSpr.spriteName = res;
            kIconSpr.MakePixelPerfect();
            //kIconSpr.transform.localPosition = new Vector3(-((kTextLabel.printedSize.x / 2) + 30), 0, 0);
        }
        else if (type == eTYPE.TEX)
        {
            kIconTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", res + ".png");
            kIconTex.gameObject.SetActive(true);
            //kIconTex.transform.localPosition = new Vector3(-((kTextLabel.printedSize.x / 2) + 60), 0, 0);
        }


        Invoke("OnClickClose", GameInfo.Instance.GameConfig.NoticePopupDuration );
        kBGBtn.gameObject.SetActive(true);
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
        ShowNextPopup();
    }

    public void OnClick_BG()
    {
        if (GameSupport.IsTutorial())
            return;

        if (_runyype == eRUNTYPE.CHARWEAPON)
        {
            UIValue.Instance.SetValue(UIValue.EParamType.CharSelUID, (long)_value1);
            UIValue.Instance.SetValue(UIValue.EParamType.CharSelTableID, _value2);
            UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.WEAPON);
            LobbyUIManager.Instance.SetPanelType(ePANELTYPE.CHARINFO);
        }
        else if (_runyype == eRUNTYPE.CHARSKILL)
        {
            UIValue.Instance.SetValue(UIValue.EParamType.CharSelUID, (long)_value1);
            UIValue.Instance.SetValue(UIValue.EParamType.CharSelTableID, _value2);
            UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.SKILL);
            LobbyUIManager.Instance.SetPanelType(ePANELTYPE.CHARINFO);
        }
        else if (_runyype == eRUNTYPE.CHARCARD)
        {
            UIValue.Instance.SetValue(UIValue.EParamType.CharSelUID, (long)_value1);
            UIValue.Instance.SetValue(UIValue.EParamType.CharSelTableID, _value2);
            UIValue.Instance.SetValue(UIValue.EParamType.CharInfoTab, (int)UICharInfoPanel.eCHARINFOTAB.SUPPORTER);
            LobbyUIManager.Instance.SetPanelType(ePANELTYPE.CHARINFO);
        }

        OnClickClose();
    }

    public void ShowNextPopup()
    {
        if (PopupList.Count == 0)
            return;

        PopupData data = PopupList[0];
        InitNoticePopup(data.Type, data.Text, data.Res, data.RunType, data.Value1, data.Value2);
        PopupList.Remove(data);
    }
}
