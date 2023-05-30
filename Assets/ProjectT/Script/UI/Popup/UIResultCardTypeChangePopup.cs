
using UnityEngine;
using System;
using System.Text;


public class UIResultCardTypeChangePopup : FComponent
{
    [Header("[Property]")]
    public UITexture    TexCard;
    public UISprite     SprBeforeType;
    public UISprite     SprCurrentType;

    private CardData        mCardData           = null;
    private eSTAGE_CONDI    mBeforeCardType     = eSTAGE_CONDI.NONE;
    private StringBuilder   mStringBuilder      = new StringBuilder();
    private Action          mCallbackOnClose    = null;


    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
        int cardImageNum = GameSupport.GetCardImageNum(mCardData);
        string path = string.Format("Card/{0}_{1}.png", mCardData.TableData.Icon, cardImageNum);

        TexCard.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("card", path);

        mStringBuilder.Clear();
        mStringBuilder.AppendFormat("{0}{1}", UICardTypeChangePopup.TYPE_SPR_NAME, (int)mBeforeCardType);
        SprBeforeType.spriteName = mStringBuilder.ToString();

        mStringBuilder.Clear();
        mStringBuilder.AppendFormat("{0}{1}", UICardTypeChangePopup.TYPE_SPR_NAME, mCardData.Type);
        SprCurrentType.spriteName = mStringBuilder.ToString();
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);
    }

    public override void OnClickClose()
    {
        base.OnClickClose();
        mCallbackOnClose?.Invoke();
    }

    public void SetResultInfo(CardData cardData, eSTAGE_CONDI beforeCardType, Action callbackOnClose)
    { // OnEnable보다 먼저 호출돼야 함 (LobbyUIManager.Instance.ShowUI 이전)
        mCardData = cardData;
        mBeforeCardType = beforeCardType;
        mCallbackOnClose = callbackOnClose;
    }
}
