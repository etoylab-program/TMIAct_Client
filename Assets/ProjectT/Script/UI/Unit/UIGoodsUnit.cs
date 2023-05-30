using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGoodsUnit : FUnit
{

    public UISprite kIconSpr;
    public UILabel kTextLabel;
    public UITexture kIconTexture;

    public long MyAbs(long num)
    {
        long result = num;
        if (num < 0)
            result = num * -1;
        return result;
    }

    public void InitGoodsUnit(eGOODSTYPE etype, long num, bool bcheck = false, bool bsale = false)
    {
        kIconSpr.gameObject.SetActive(true);
        kTextLabel.gameObject.SetActive(true);
        kIconTexture.SetActive(false);

        kIconSpr.spriteName = GameSupport.GetGoodsIconName(etype, true);

        if (kIconSpr.spriteName == string.Empty)
            kIconSpr.gameObject.SetActive(false);

        if (bcheck)
        {
            
            if (etype == eGOODSTYPE.GOODS)
            {
                kTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_W), num);
            }
            else
            {
                if (GameInfo.Instance.UserData.IsGoods(etype, MyAbs(num)))
                {
                    if (bsale)
                    {
                        if(MyAbs(num) <= (int)eCOUNT.NONE)
                            kTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_W), num);
                        else
                            kTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR), num);
                    }
                    else
                        kTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_W), num);
                }                    
                else
                    kTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_R), num);
            }
        }
        else
        {
            if (etype == eGOODSTYPE.GOODS)
            {
                kTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_W), num);
            }
            else
            {
                if (bsale == true)
                    kTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR), num);
                else
                    kTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT), num);
            }
        }
    }

    public void InitGoodsUnit(eGOODSTYPE etype, long num, long max)
    {
        kIconTexture.SetActive(false);

        if (etype == eGOODSTYPE.AP)
            kIconSpr.spriteName = "Goods_Ticket";
        else if (etype == eGOODSTYPE.BP)
            kIconSpr.spriteName = "Goods_TimeAttack";
        else if (etype == eGOODSTYPE.FRIENDPOINT)
            kIconSpr.spriteName = "Goods_FP";

        if (max <= num)
            kTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(277), num, max);
        else
            kTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(278), num, max);
    }

    public void InitGoodsUnit(eCircleGoodsType circleGoodsType, long num, bool isSmall = false)
    {
        kIconTexture.SetActive(false);

        string spriteName = string.Empty;
        switch (circleGoodsType)
        {
            case eCircleGoodsType.CIRCLE_GOLD:
                {
                    spriteName = isSmall ? "Goods_Circleactivity_s" : "Goods_Circleactivity";
                }
                break;
        }

        kIconSpr.gameObject.SetActive(!string.IsNullOrEmpty(spriteName));
        if (kIconSpr.gameObject.activeSelf)
        {
            kIconSpr.spriteName = spriteName;
        }

        kTextLabel.textlocalize = num.ToString();
    }

    
    public void InitGoodsUnit(GameTable.Store.Param storedata, StoreSaleData storesaledata = null, int itemCount = 1)
    {
        kIconTexture.SetActive(false);
        kIconSpr.SetActive(false);
        kTextLabel.SetActive(true);

        if (storesaledata == null)
        {
            storesaledata = GameInfo.Instance.ServerData.StoreSaleList.Find(x => x.TableID == storedata.ID);
        }

        switch ((eREWARDTYPE)storedata.PurchaseType)
        {
            case eREWARDTYPE.GOODS:
                kIconSpr.SetActive(true);
                kIconSpr.spriteName = GameSupport.GetGoodsIconName((eGOODSTYPE)storedata.PurchaseIndex, true);
                if (kIconSpr.spriteName == string.Empty)
                    kIconSpr.gameObject.SetActive(false);

                kTextLabel.textlocalize = FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_W, GameSupport.GetDiscountPrice(storedata.PurchaseValue, storesaledata) * itemCount);
                break;
            case eREWARDTYPE.ITEM:
                kIconTexture.SetActive(true);
                var itemdata = GameInfo.Instance.GameTable.FindItem(storedata.PurchaseIndex);
                if (itemdata != null)
                {
                    // 텍스쳐 Depth, 위치 보정
                    kIconTexture.depth = kIconSpr.depth;
                    kIconTexture.transform.localPosition = kIconSpr.transform.localPosition;

                    kIconTexture.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + itemdata.Icon);
                }

                if(storesaledata == null)
                    kTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_W), storedata.PurchaseValue * itemCount);
                else
                {
                    //재할인 대기 시간 확인
                    if ((eStoreSaleKind)storesaledata.LimitType == eStoreSaleKind.CycleMinute)
                    {
                        var storemydata = GameInfo.Instance.GetStoreData(storedata.ID);
                        if (storemydata != null)
                        {
                            if(GameSupport.GetRemainTime(storemydata.GetTime()).TotalSeconds > 0)
                            {
                                kTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_W), storedata.PurchaseValue * itemCount);
                                return;
                            }
                        }
                    }

                    kTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_W), GameSupport.GetDiscountPrice(storedata.PurchaseValue * itemCount, storesaledata));
                }
                break;
            default:
                return;
        }
    }

    public void InitGoodsUnitTexture(Texture iconTexture, long num, Color32 fontColor)
    {
        kIconSpr.gameObject.SetActive(false);
        kTextLabel.gameObject.SetActive(true);
        kIconTexture.SetActive(true);

        kTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT), num);
        kTextLabel.color = fontColor;
        kIconTexture.mainTexture = iconTexture;
    }

    public void InitGoodsUnitTexture(Texture iconTexture)
    {
        kIconSpr.gameObject.SetActive(false);
        kTextLabel.gameObject.SetActive(false);
        kIconTexture.SetActive(true);

        kIconTexture.mainTexture = iconTexture;
    }


    public void InitGoodsUnit_EventStore()
    {
        //이벤트 스토어 재화 표현
        kIconTexture.SetActive(true);
        kIconSpr.SetActive(false);
        kTextLabel.SetActive(true);
                
        var itemdata = GameInfo.Instance.GameTable.FindItem(GameInfo.Instance.GameConfig.EventStoreGoodsTableID);
        if (itemdata != null)
        {
            // 텍스쳐 Depth, 위치 보정
            kIconTexture.depth = kIconSpr.depth;
            kIconTexture.transform.localPosition = kIconSpr.transform.localPosition;

            kIconTexture.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + itemdata.Icon);


            int value = 0;
            var item = GameInfo.Instance.ItemList.Find(x => x.TableID == GameInfo.Instance.GameConfig.EventStoreGoodsTableID);
            if (item != null)            
                value = item.Count;

            kTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_W), value);
        }
    }

    public void SetText(string text)
    {
        kTextLabel.textlocalize = text;
    }

    public void SetTextNowMax(long num, long max)
    {
        if (max <= num)
            kTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(277), num, max);
        else
            kTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(278), num, max);
    }
}
