using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuyPopup
{
    public static UIBuyPopup GetBuyPopup()
    {
        UIBuyPopup mpopup = null;
        //if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
        //    mpopup = LobbyUIManager.Instance.GetUI<UIBuyPopup>("BuyPopup");

        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
            mpopup = LobbyUIManager.Instance.ShowUI("BuyPopup", true) as UIBuyPopup;
        else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
            mpopup = GameUIManager.Instance.ShowUI("BuyPopup", true) as UIBuyPopup;
        return mpopup;
    }

    public static void Show(string text1, string text2, bool isFree, bool isSale, eREWARDTYPE rewardtype, eGOODSTYPE etype, long count, long saleCount, UIBuyPopup.OnClickOKCallBack callbackok, UIBuyPopup.OnClickOKCallBack callbackcancel)
    {
        UIBuyPopup mpopup = GetBuyPopup();
        if (mpopup == null)
            return;
        mpopup.InitBuyPopup(text1, text2, isFree, isSale, rewardtype, etype, count, saleCount, callbackok, callbackcancel);
    }

    /*
   public static void Hide()
   {
       UIBuyPopup mpopup = GetBuyPopup();
       if (mpopup == null)
           return;
       mpopup.SetUIActive(false);
   }

   public static void SetActive(bool b)
   {
       UIBuyPopup mpopup = GetBuyPopup();
       if (mpopup == null)
           return;
       mpopup.gameObject.SetActive(b);
   }
   */
}

public class UIBuyPopup : FComponent
{
    public delegate void OnClickOKCallBack();
    private OnClickOKCallBack CallBackOK;
    private OnClickOKCallBack CallBackCancel;

    public UILabel kGoodsLabel;
    public UILabel kTextLabel;
    public UITexture kItemTex;
    public UIGoodsUnit kGoodsUnit;
    public UISprite kGoodsBGSpr;

    [Header("Sale Objs")]
    public GameObject kSaleObj;
    public UIGoodsUnit kSaleGoodsUnit;
    public UILabel kSaleDiscountValueLabel;

    public void InitBuyPopup(string text1, string text2, bool isFree, bool isSale, eREWARDTYPE rewardtype, eGOODSTYPE etype, long count, long saleCount, OnClickOKCallBack callbackok, OnClickOKCallBack callbackcancel)
    {
        kGoodsLabel.textlocalize = text1;
        kTextLabel.textlocalize = text2;
        kItemTex.gameObject.SetActive(false);
        kGoodsUnit.gameObject.SetActive(false);
        kGoodsBGSpr.gameObject.SetActive(false);
        kSaleObj.SetActive(false);
        if (isFree == false)
        {
            kGoodsBGSpr.gameObject.SetActive(true);
            if (isSale)
            {
                kSaleObj.SetActive(true);
                if (rewardtype == eREWARDTYPE.ITEM)
                {
                    kSaleGoodsUnit.kIconSpr.gameObject.SetActive(false);
                    kItemTex.gameObject.SetActive(true);
                    var data = GameInfo.Instance.GameTable.FindItem((int)etype);
                    if (data != null)
                        kItemTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + data.Icon);
                }
                else
                {
                    kSaleGoodsUnit.kIconSpr.gameObject.SetActive(true);
                }

                kSaleDiscountValueLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_W), count);
                kSaleGoodsUnit.InitGoodsUnit(etype, saleCount, false, isSale);
            }
            else
            {
                kGoodsUnit.gameObject.SetActive(true);
                
                if (rewardtype == eREWARDTYPE.ITEM)
                {
                    kGoodsUnit.kIconSpr.gameObject.SetActive(false);
                    kItemTex.gameObject.SetActive(true);
                    var data = GameInfo.Instance.GameTable.FindItem((int)etype);
                    if (data != null)
                        kItemTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + data.Icon);
                }
                else
                {
                    kGoodsUnit.kIconSpr.gameObject.SetActive(true);
                }

                kGoodsUnit.InitGoodsUnit(etype, count, false, isSale);
            }
        }

        CallBackOK = callbackok;
        CallBackCancel = callbackcancel;
    }

    public void OnClick_CancelBtn()
    {
        SetUIActive(false);
        if (CallBackCancel != null)
            CallBackCancel();
    }

    public void OnClick_BuyBtn()
    {
        SetUIActive(false);
        if (CallBackOK != null)
            CallBackOK();
    }

    public override void OnClickClose()
    {
        SetUIActive(false);
        if (CallBackCancel != null)
            CallBackCancel();
    }
}