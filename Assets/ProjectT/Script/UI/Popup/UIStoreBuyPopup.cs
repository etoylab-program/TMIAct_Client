using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoreBuyPopup
{
    public static UIStoreBuyPopup GetStoreBuyPopup()
    {
        UIStoreBuyPopup mpopup = null;
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
            mpopup = LobbyUIManager.Instance.GetUI<UIStoreBuyPopup>("StoreBuyPopup");
        else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
            mpopup = GameUIManager.Instance.GetUI<UIStoreBuyPopup>("StoreBuyPopup");
        return mpopup;
    }

    public static void Show(GameClientTable.StoreDisplayGoods.Param storedisplaytable, ePOPUPGOODSTYPE popupGoodsType, UIStoreBuyPopup.OnClickOKCallBack callbackok, UIStoreBuyPopup.OnClickOKCallBack callbackcancel)
    {
        UIStoreBuyPopup mpopup = GetStoreBuyPopup();
        mpopup.InitStoreBuyPopup(storedisplaytable, popupGoodsType, callbackok, callbackcancel);
    }

    public static int GetItemCount()
    {
        UIStoreBuyPopup popup = GetStoreBuyPopup();
        if(popup == null)
        {
            return 1;
        }

        return popup.ItemCount;
    }
}

public class UIStoreBuyPopup : FComponent
{
    public UIStoreListSlot      kStoreListSlot;
    public UIStoreListSlotSM    kStoreListSlotSM;
    public UIStoreMonthlySlot   kStoreMonthlySlot;
    public GameObject           RunCntObj;
    public UILabel              LbCount;

    public delegate void OnClickOKCallBack();
    public int ItemCount { get; set; } = 1;

    private OnClickOKCallBack CallBackOK;
    private OnClickOKCallBack CallBackCancel;

    private GameClientTable.StoreDisplayGoods.Param _storedisplaytable;
    private GameTable.Store.Param _storetable;
    private StoreSaleData _storesaledata;

    private int mMaxCount = 1;


    public void InitStoreBuyPopup(GameClientTable.StoreDisplayGoods.Param storedisplaytable, ePOPUPGOODSTYPE popupGoodsType, OnClickOKCallBack callbackok, OnClickOKCallBack callbackcancel)
    {
        this.kPopupGoodsType = popupGoodsType;

        _storedisplaytable = storedisplaytable;
        _storetable = GameInfo.Instance.GameTable.FindStore(_storedisplaytable.StoreID);
        _storesaledata = GameInfo.Instance.ServerData.StoreSaleList.Find(x => x.TableID == _storedisplaytable.ID);
        CallBackOK = callbackok;
        CallBackCancel = callbackcancel;

        kStoreListSlot.ParentGO = this.gameObject;
        kStoreListSlotSM.ParentGO = this.gameObject;
        //kStoreListSlot.UpdateSlot(0, _storedisplaytable);
        kStoreListSlot.gameObject.SetActive(false);
        kStoreListSlotSM.gameObject.SetActive(false);
        kStoreMonthlySlot.gameObject.SetActive(false);
        //kSubscription.SetActive(false);

        SetCountLabelAndUpdateSlot(1);

        mMaxCount = 1;
        RunCntObj.SetActive(false);

        if(_storetable.BuyType == (int)eBuyType.Multiple)
        {
            RunCntObj.SetActive(true);
        }

        SetUIActive(true, true);
    }

	public void OnClick_CancelBtn()
	{
        OnClickClose();
    }
	
	public void OnClick_BuyBtn()
	{
        //아이템 구매 제한 체크 하기!!!
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

    public void OnBtnMinus()
    {
        UpdateItemSlot();
        SetCountLabelAndUpdateSlot(ItemCount - 1);
    }

    public void OnBtnPlus()
    {
        UpdateItemSlot();
        SetCountLabelAndUpdateSlot(ItemCount + 1);
    }

    public void OnBtnMax()
    {
        UpdateItemSlot();
        SetCountLabelAndUpdateSlot(mMaxCount);
    }

    private void UpdateItemSlot()
    {
        if ((eREWARDTYPE)_storetable.ProductType == eREWARDTYPE.GOODS)     //재화
        {
            kStoreListSlotSM.UpdateSlot(0, _storedisplaytable, UIItemListSlot.ePosType.Info, ItemCount, ref mMaxCount);
            kStoreListSlotSM.gameObject.SetActive(true);
            //kSubscription.SetActive(true);
        }
        else if ((eREWARDTYPE)_storetable.ProductType == eREWARDTYPE.CHAR)           //캐릭터
        {
            var data = GameInfo.Instance.GameTable.FindCharacter(x => x.ID == _storetable.ProductIndex);
            if (data != null)
            {
                kStoreListSlot.UpdateSlot(0, _storedisplaytable, true);
                kStoreListSlot.gameObject.SetActive(true);
            }
        }
        else if ((eREWARDTYPE)_storetable.ProductType == eREWARDTYPE.WEAPON)           //무기
        {
            var data = GameInfo.Instance.GameTable.FindWeapon(x => x.ID == _storetable.ProductIndex);
            if (data != null)
            {
                kStoreListSlotSM.UpdateSlot(0, _storedisplaytable, UIItemListSlot.ePosType.Info, ItemCount, ref mMaxCount);
                kStoreListSlotSM.gameObject.SetActive(true);
            }
        }
        else if ((eREWARDTYPE)_storetable.ProductType == eREWARDTYPE.GEM)           //곡옥
        {
            var data = GameInfo.Instance.GameTable.FindGem(x => x.ID == _storetable.ProductIndex);
            if (data != null)
            {
                kStoreListSlotSM.UpdateSlot(0, _storedisplaytable, UIItemListSlot.ePosType.Info, ItemCount, ref mMaxCount);
                kStoreListSlotSM.gameObject.SetActive(true);
            }
        }

        else if ((eREWARDTYPE)_storetable.ProductType == eREWARDTYPE.CARD)           //카드
        {
            var data = GameInfo.Instance.GameTable.FindCard(x => x.ID == _storetable.ProductIndex);
            if (data != null)
            {
                kStoreListSlotSM.UpdateSlot(0, _storedisplaytable, UIItemListSlot.ePosType.Info, ItemCount, ref mMaxCount);
                kStoreListSlotSM.gameObject.SetActive(true);
            }
        }
        else if ((eREWARDTYPE)_storetable.ProductType == eREWARDTYPE.ITEM)           //아이템
        {
            var data = GameInfo.Instance.GameTable.FindItem(x => x.ID == _storetable.ProductIndex);
            if (data != null)
            {
                kStoreListSlotSM.UpdateSlot(0, _storedisplaytable, UIItemListSlot.ePosType.Info, ItemCount, ref mMaxCount);
                kStoreListSlotSM.gameObject.SetActive(true);
            }
        }
        else if ((eREWARDTYPE)_storetable.ProductType == eREWARDTYPE.COSTUME)        //코스튬
        {
            var data = GameInfo.Instance.GameTable.FindCostume(x => x.ID == _storetable.ProductIndex);
            if (data != null)
            {
                kStoreListSlot.UpdateSlot(0, _storedisplaytable);
                kStoreListSlot.gameObject.SetActive(true);
            }
        }
        else if ((eREWARDTYPE)_storetable.ProductType == eREWARDTYPE.USERMARK)
        {
            var data = GameInfo.Instance.GameTable.FindUserMark(x => x.ID == _storetable.ProductIndex);
            if (data != null)
            {
                kStoreListSlotSM.UpdateSlot(0, _storedisplaytable, UIItemListSlot.ePosType.Info, ItemCount, ref mMaxCount);
                kStoreListSlotSM.gameObject.SetActive(true);
            }
        }
        else if ((eREWARDTYPE)_storetable.ProductType == eREWARDTYPE.BADGE)          //문양
        {
            var data = GameInfo.Instance.GameTable.FindBadgeOpt(x => x.OptionID == _storetable.ProductIndex);
            if (data != null)
            {
                kStoreListSlotSM.UpdateSlot(0, _storedisplaytable, UIItemListSlot.ePosType.Info, ItemCount, ref mMaxCount);
                kStoreListSlotSM.gameObject.SetActive(true);
            }
            else
            {
                if (_storetable.ProductIndex == 0)
                {
                    kStoreListSlotSM.UpdateSlot(0, _storedisplaytable, UIItemListSlot.ePosType.Info, ItemCount, ref mMaxCount);
                    kStoreListSlotSM.gameObject.SetActive(true);
                }
            }
        }
        else if ((eREWARDTYPE)_storetable.ProductType == eREWARDTYPE.MONTHLYFEE)        //월정액 상품
        {
            kStoreMonthlySlot.UpdateSlot(_storedisplaytable);
            kStoreMonthlySlot.gameObject.SetActive(true);
        }
        else
        {
            kStoreListSlotSM.UpdateSlot(0, _storedisplaytable, UIItemListSlot.ePosType.Info, ItemCount, ref mMaxCount);
            kStoreListSlotSM.gameObject.SetActive(true);
        }

        if (_storetable.BuyType != (int)eBuyType.Multiple)
        {
            mMaxCount = 1;
        }
    }

    private void SetCountLabelAndUpdateSlot(int count)
    {
        ItemCount = Mathf.Clamp(count, 1, Mathf.Min(Mathf.Max(mMaxCount, 1), GameInfo.Instance.GameConfig.MatCount));
        LbCount.textlocalize = ItemCount.ToString();

        UpdateItemSlot();
    }
}
