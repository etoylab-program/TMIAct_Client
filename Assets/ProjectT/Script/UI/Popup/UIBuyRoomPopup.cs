
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UIBuyRoomPopup : FComponent
{
    public delegate void BuyRoomCallBack();
    private BuyRoomCallBack BuyCallBack;

    public enum eBuyRoomImgType
    {
        None = 0,
        Icon,
        Theme,
        Costume,
    }

    [Header("[Buy with Img]")] // 룸 테마만 사용
    public GameObject buyWithImg;
    public UILabel lbNameWithImg;
    public UITexture texRoom;
    public GameObject buyWithImgIcon;
    public UITexture imgIcon;
    public GameObject buyWithImgChar;
    public UITexture imgChar;

    [Header("[Buy with Text]")]
    public GameObject buyWithText;
    public UILabel lbNameWithText;

    [Header("[Common]")]
    public GameObject goRoomPoint;
    public UILabel lbRoomPoint;
    public GameObject goGoods;
    public UISprite sprGoods;
    public UILabel lbGoods;

    private FComponent m_parentComponent = null;
    private int m_curRoomTableId;
    private GameTable.StoreRoom.Param m_param;
    private eBuyRoomImgType m_BuyRoomImgType;

    public void Show(FComponent parentComponent, int curRoomTableId, GameTable.StoreRoom.Param param, BuyRoomCallBack callback)
    {
        Show(parentComponent, curRoomTableId, param);
        BuyCallBack = callback;
    }

    public void Show(FComponent parentComponent, int curRoomTableId, GameTable.StoreRoom.Param param)
    {
        BuyCallBack = null;

        m_parentComponent = parentComponent;
        m_curRoomTableId = curRoomTableId;
        m_param = param;
        m_BuyRoomImgType = eBuyRoomImgType.None;

        string iconPath = null;

        string name = null;
        switch ((eREWARDTYPE)m_param.ProductType)
        {
            case eREWARDTYPE.ROOMTHEME:
                GameTable.RoomTheme.Param paramRoom = GameInfo.Instance.GameTable.FindRoomTheme(m_param.ProductIndex);
                name = FLocalizeString.Instance.GetText(paramRoom.Name);
                m_BuyRoomImgType = eBuyRoomImgType.Theme;
                iconPath = paramRoom.Icon;
                break;

            case eREWARDTYPE.ROOMFUNC:
                GameTable.RoomFunc.Param paramRoomFunc = GameInfo.Instance.GameTable.FindRoomFunc(m_param.ProductIndex);
                name = FLocalizeString.Instance.GetText(paramRoomFunc.Name);
                m_BuyRoomImgType = eBuyRoomImgType.Icon;
                iconPath = paramRoomFunc.Icon;
                break;

            case eREWARDTYPE.ROOMACTION:
                GameTable.RoomAction.Param paramRoomAction = GameInfo.Instance.GameTable.FindRoomAction(m_param.ProductIndex);
                name = FLocalizeString.Instance.GetText(paramRoomAction.Name);
                break;

            case eREWARDTYPE.ROOMFIGURE:
                GameTable.RoomFigure.Param paramRoomFigure = GameInfo.Instance.GameTable.FindRoomFigure(m_param.ProductIndex);
                name = FLocalizeString.Instance.GetText(paramRoomFigure.Name);
                m_BuyRoomImgType = eBuyRoomImgType.Icon;
                if (paramRoomFigure.ContentsType == (int)eContentsPosKind.COSTUME)
                {
                    iconPath = string.Format("Char/ListSlot/{0}", paramRoomFigure.IconBuy);
                    m_BuyRoomImgType = eBuyRoomImgType.Costume;
                }
                else if (paramRoomFigure.ContentsType == (int)eContentsPosKind.MONSTER)
                    iconPath = string.Format("Monster/{0}", paramRoomFigure.IconBuy);
                else if (paramRoomFigure.ContentsType == (int)eContentsPosKind.WEAPON)
                    iconPath = string.Format("Item/{0}", paramRoomFigure.IconBuy);
                break;
        }

        if (m_BuyRoomImgType == eBuyRoomImgType.None)
        {
            buyWithImg.SetActive(false);
            buyWithText.SetActive(true);

            lbNameWithText.textlocalize = name;
        }
        else
        {
            buyWithImg.SetActive(true);
            buyWithText.SetActive(false);

            lbNameWithImg.textlocalize = name;

            if (m_BuyRoomImgType == eBuyRoomImgType.Theme)
            {
                buyWithImgIcon.SetActive(false);
                buyWithImgChar.SetActive(false);
                texRoom.gameObject.SetActive(true);

                texRoom.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/" + iconPath) as Texture2D;
            }
            else if (m_BuyRoomImgType == eBuyRoomImgType.Costume)
            {
                buyWithImgIcon.SetActive(false);
                buyWithImgChar.SetActive(true);
                texRoom.gameObject.SetActive(false);

                imgChar.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/" + iconPath) as Texture2D;
            }
            else
            {
                buyWithImgIcon.SetActive(true);
                buyWithImgChar.SetActive(false);
                texRoom.gameObject.SetActive(false);

                imgIcon.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/" + iconPath) as Texture2D;
            }
        }

        if (param.NeedRoomPoint > 0)
        {
            goRoomPoint.SetActive(true);
            lbRoomPoint.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT), param.NeedRoomPoint);
        }
        else
        {
            goRoomPoint.SetActive(false);
        }

        if (param.PurchaseValue > 0)
        {
            goGoods.SetActive(true);
            if (param.PurchaseType == (int)eGOODSTYPE.GOLD)
                sprGoods.spriteName = "Goods_Gold_s";
            else if (param.PurchaseType == (int)eGOODSTYPE.CASH)
                sprGoods.spriteName = "Goods_Cash_s";
            lbGoods.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT), param.PurchaseValue);
        }
        else
        {
            goGoods.SetActive(false);
        }

        SetUIActive(true, true);
    }

    public void OnBtnBuy()
    {
        if (!GameSupport.IsCheckGoods(eGOODSTYPE.ROOMPOINT, m_param.NeedRoomPoint))
            return;

        if (!GameSupport.IsCheckGoods((eGOODSTYPE)m_param.PurchaseType, m_param.PurchaseValue))
            return;


        GameInfo.Instance.Send_RoomPurchase(m_param.ID, OnBuy);
    }

    private void OnBuy(int result, PktMsgType ptkMsgType)
    {
        if (result != 0)
            return;

        OnClickClose();
        m_parentComponent.Renewal(true);

        if (BuyCallBack != null)
            BuyCallBack();
    }

    public void OnBtnCancel()
    {
        OnClickClose();
    }
}
