
using UnityEngine;
using System.Text;
using System.Collections.Generic;


public class UICardTypeChangePopup : FComponent
{
    [System.Serializable]
    public struct sUICardAttr
    {
        public UIButton BtnSelect;
        public UISprite SprAttr;
        public UILabel  LbAttr;
    }


    public static string   TYPE_SPR_NAME                       = "SupporterType_";
    public static int      CARD_TYPE_STRING_TABLE_ID_OFFSET    = 451;

    [Header("[Property]")]
    public UISprite         SprSelectText;
    public UISprite         SprSelectLine;
    public sUICardAttr[]    ArrCardAttr;
    public UIGoodsUnit      GUGold;
    public UIGoodsUnit      GUCash;

    private CardData        mCardData           = null;
    private List<int>       mListTypeId         = new List<int>();
    private StringBuilder   mStringBuilder      = new StringBuilder();
    private int             mSelectedIndex      = 0;
    private eGOODSTYPE      mConfirmGoodsType   = eGOODSTYPE.GOLD;
    private eSTAGE_CONDI    mOriginalCardType   = eSTAGE_CONDI.NONE;


    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
        mListTypeId.Clear();
        for (int i = 1; i < (int)eSTAGE_CONDI._STAGE_CONDI_MAX_; i++)
        {
            mListTypeId.Add(i);
        }

        mListTypeId.Remove(mCardData.Type);

        SetBeChangedCardType(0);
        SetBeChangedCardType(1);

        mSelectedIndex = 0;

        GUGold.InitGoodsUnit(eGOODSTYPE.GOLD, GameInfo.Instance.GameConfig.CardChangeGold);
        GUCash.InitGoodsUnit(eGOODSTYPE.CASH, GameInfo.Instance.GameConfig.CardChangeCash);
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        if(mCardData == null)
        {
            return;
        }

        SelectLineAndTextPosition(ArrCardAttr[mSelectedIndex].BtnSelect);
    }

    public override void OnClickClose()
    {
        base.OnClickClose();
    }

    public void SetCardData(CardData cardData) // OnEnable보다 먼저 호출돼야 함 (LobbyUIManager.Instance.ShowUI 이전)
    {
        mCardData = cardData;
        mOriginalCardType = (eSTAGE_CONDI)mCardData.Type;
    }

    public void OnBtnTypeSelect1()
    {
        mSelectedIndex = 0;
        Renewal(true);
    }

    public void OnBtnTypeSelect2()
    {
        mSelectedIndex = 1;
        Renewal(true);
    }

    public void OnBtnGoldConfirm()
    {
        ShowConfirmMessagePopup(eGOODSTYPE.GOLD);
    }

    public void OnBtnCashConfirm()
    {
        ShowConfirmMessagePopup(eGOODSTYPE.CASH);
    }

    private void SetBeChangedCardType(int index)
    {
        mStringBuilder.Clear();
        mStringBuilder.AppendFormat("{0}{1}", TYPE_SPR_NAME, mListTypeId[index]);

        ArrCardAttr[index].SprAttr.spriteName = mStringBuilder.ToString();
        ArrCardAttr[index].LbAttr.textlocalize = FLocalizeString.Instance.GetText(CARD_TYPE_STRING_TABLE_ID_OFFSET + (mListTypeId[index] - 1));
    }

    private void SelectLineAndTextPosition(UIButton btnTypeSelect)
    {
        SprSelectLine.transform.SetParent(btnTypeSelect.transform);
        Utility.InitTransform(SprSelectLine.gameObject);

        SprSelectText.transform.SetParent(btnTypeSelect.transform);
        Utility.InitTransform(SprSelectText.gameObject, new Vector3(0.0f, 100.0f, 0.0f), Quaternion.identity, Vector3.one);
    }

    private void ShowConfirmMessagePopup(eGOODSTYPE confirmGoodsType)
    {
        mConfirmGoodsType = confirmGoodsType;

        string strType = FLocalizeString.Instance.GetText(CARD_TYPE_STRING_TABLE_ID_OFFSET + (mListTypeId[mSelectedIndex] - 1));

        mStringBuilder.Clear();
        mStringBuilder.Append(string.Format(FLocalizeString.Instance.GetText(3228), strType));

        MessagePopup.OKCANCEL(eTEXTID.OK, mStringBuilder.ToString(), OnConfirm, null);
    }

    private void OnConfirm()
    {
        GameInfo.Instance.Send_ReqChangeCardType(mCardData.CardUID, mListTypeId[mSelectedIndex], mConfirmGoodsType, OnCardTypeChanged);
    }

    private void OnCardTypeChanged(int result, PktMsgType pktMsg)
    {
        if(result != 0)
        {
            return;
        }

        PktInfoCardTypeChangeAck pktCardTypeChange = pktMsg as PktInfoCardTypeChangeAck;
        if(pktCardTypeChange == null)
        {
            return;
        }

        UITopPanel panel = LobbyUIManager.Instance.GetUI<UITopPanel>("TopPanel");
        panel.Renewal(true);

        UIResultCardTypeChangePopup popup = LobbyUIManager.Instance.GetUI<UIResultCardTypeChangePopup>("ResultCardTypeChangePopup");
        popup.SetResultInfo(mCardData, mOriginalCardType, OnChangeCardResultClose);

        LobbyUIManager.Instance.ShowUI("ResultCardTypeChangePopup", true);
        OnClickClose();
    }

    private void OnChangeCardResultClose()
    {
        UICardInfoPopup popup = LobbyUIManager.Instance.GetActiveUI<UICardInfoPopup>("CardInfoPopup");
        popup.OnCloseCallBack = null;
        popup.Renewal(true);

        UIItemPanel panel = LobbyUIManager.Instance.GetActiveUI<UIItemPanel>("ItemPanel");
        if (panel)
        {
            panel.OnTabItemSelect((int)UIItemPanel.eTabType.TabType_Card, SelectEvent.Click);
        }
    }
}