using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIStoreListSlot : FSlot
{
    public UISprite kBGSpr;
    public UISprite kBGDisSpr;
    public UILabel kTitleLabel;
    public UITexture kIconTex;
    public UILabel kSaleValueLabel;
    public GameObject kNew;
    public UIGoodsUnit kGoodsUnit;
    public UISprite kGoodsUnitBGSpr;
    public UISprite kSaleMark;
    public GameObject kGetGO;
    public GameObject kOpen;
    public UILabel kOpenDateLabel;
    public GameObject kLimitedDate;
    public UILabel kDateLabel;
    public GameObject kMaxAP;
    public UILabel kMaxLabel;
    public UISprite kMarkIconSpr;
    public GameObject kBookBtn;
    public UISprite kComplateSpr;
    public FLocalizeText kQuantityLabel;

    private GameClientTable.StoreDisplayGoods.Param _storedisplaytable;
    private GameTable.Store.Param _storetable;
    private StoreSaleData _storesaledata;
    private long _purchasevalue;
    private int _index;

    public void UpdateSlot(int index, GameClientTable.StoreDisplayGoods.Param tabledata, bool isBuyPopup = false)    //Fill parameter if you need
    {
        _index = index;
        _storedisplaytable = tabledata;

        _storetable = GameInfo.Instance.GameTable.FindStore(_storedisplaytable.StoreID);
        _storesaledata = GameInfo.Instance.ServerData.StoreSaleList.Find(x => x.TableID == _storedisplaytable.StoreID);

        kSaleValueLabel.transform.parent.gameObject.SetActive(false);
        kNew.gameObject.SetActive(false);
        kIconTex.gameObject.SetActive(true);
        kBGDisSpr.gameObject.SetActive(false);
        kGetGO.gameObject.SetActive(false);
        kSaleMark.gameObject.SetActive(false);
        kOpen.SetActive(false);
        kLimitedDate.SetActive(false);
        kMaxAP.SetActive(false);
        kMarkIconSpr.SetActive(false);
        kTitleLabel.textlocalize = FLocalizeString.Instance.GetText(_storedisplaytable.Name);
        kBookBtn.SetActive(false);
        kBGSpr.SetActive(true);

        if (tabledata.Category == (int)UIStorePanel.eStoreTabType.STORE_COSTUME)
        {
            int haveCostumeCount = 0;
            List<GameClientTable.StoreDisplayGoods.Param> storeDisplayGoodsParamList = GameInfo.Instance.GameClientTable.FindAllStoreDisplayGoods(x =>
                        x.PanelType == (int)eSD_PanelType.STORE && x.Category == (int)UIStorePanel.eStoreTabType.STORE_COSTUME_SUB && x.SubCategory == tabledata.SubCategory);

            GameTable.Store.Param storeParam = null;
            foreach (GameClientTable.StoreDisplayGoods.Param storeDisplayGoodsParam in storeDisplayGoodsParamList)
            {
                storeParam = GameInfo.Instance.GameTable.FindStore(storeDisplayGoodsParam.StoreID);
                if (storeParam == null)
                {
                    continue;
                }

                if (GameInfo.Instance.HasCostume(storeParam.ProductIndex))
                {
                    ++haveCostumeCount;
                }
            }

            kComplateSpr.SetActive(storeDisplayGoodsParamList.Count <= haveCostumeCount);

            string quantityStr = string.Empty;
            if (!kComplateSpr.gameObject.activeSelf)
            {
                quantityStr = FLocalizeString.Instance.GetText(218, haveCostumeCount, storeDisplayGoodsParamList.Count);
            }
            kQuantityLabel.gameObject.SetActive(!string.IsNullOrEmpty(quantityStr));
            kQuantityLabel.SetLabel(quantityStr);
        }
        else
        {
            kComplateSpr.SetActive(false);
            kQuantityLabel.gameObject.SetActive(false);
        }

        GameSupport.LoadLocalizeTexture(kIconTex, "icon", "Icon/" + _storedisplaytable.Icon, _storedisplaytable.IconLocalize);

        kIconTex.color = Color.white;

        _storetable = GameInfo.Instance.GameTable.FindStore(_storedisplaytable.StoreID);

        if (_storetable == null)
        {
            kBookBtn.SetActive(_storedisplaytable.Category == (int)UIStorePanel.eStoreTabType.STORE_CHAR);
            kGoodsUnit.gameObject.SetActive(false);
            kGoodsUnitBGSpr.gameObject.SetActive(false);
        }
        else
        {
            kGoodsUnitBGSpr.gameObject.SetActive(true);
            kGoodsUnit.gameObject.SetActive(true);
            _purchasevalue = _storetable.PurchaseValue;

            int purchaseValue = (int)_purchasevalue;
            if (_storesaledata != null)
            {
                bool isSale = true;
                switch ((eGOODSTYPE)_storetable.PurchaseIndex)
                {
                    case eGOODSTYPE.CASH:
                    case eGOODSTYPE.SUPPORTERPOINT:
                    case eGOODSTYPE.FRIENDPOINT:
                        {
                            isSale = 0 < _storesaledata.DiscountRate;
                            if (isSale)
                            {
                                int remainCount = GameSupport.GetLimitedCnt(_storetable.ID);
                                isSale = 0 < remainCount;
                            }
                        }
                        break;
                }

                if (isSale)
                {
                    int discountrate = GameSupport.GetStoreDiscountRate(_storetable.ID);
                    int salevalue = _storetable.PurchaseValue - (int)(_storetable.PurchaseValue * (float)((float)discountrate / (float)eCOUNT.MAX_BO_FUNC_VALUE));

                    kSaleValueLabel.transform.parent.gameObject.SetActive(true);
                    kSaleValueLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_W), _storetable.PurchaseValue);

                    purchaseValue = salevalue;
                }
            }
            kGoodsUnit.InitGoodsUnit((eGOODSTYPE)_storetable.PurchaseIndex, purchaseValue);

            // 캐릭터와 코스튬은 단일 판매
            // 1회 구매시 구매 완료 표시
            if (_storetable.ProductType == (int)eREWARDTYPE.CHAR || (_storetable.ProductType == (int)eREWARDTYPE.COSTUME && _storedisplaytable.Category == (int)UIStorePanel.eStoreTabType.STORE_COSTUME_SUB))
            {
                bool isHaveFlag = false;
                switch ((eREWARDTYPE)_storetable.ProductType)
                {
                    case eREWARDTYPE.CHAR:      //  캐릭터 보유 체크
                        {
                            isHaveFlag = (GameInfo.Instance.GetCharDataByTableID(_storetable.ProductIndex) != null);
                            if (isBuyPopup == true)
                            {
                                kMaxAP.SetActive(true);
                                kMaxLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1579), GameSupport.GetMaxAP() + GameInfo.Instance.GameConfig.CharOpenAddAP, "", GameInfo.Instance.GameConfig.CharOpenAddAP);
                                
                            }

                            if (ParentGO != null)
                            {
                                if (ParentGO.GetComponent<UIStorePanel>() != null)
                                {
                                    if(_storedisplaytable.CharacterID > (int)eCOUNT.NONE)
                                        kBookBtn.SetActive(!isHaveFlag);
                                }
                                    
                            }
                            
                        }
                        break;
                    case eREWARDTYPE.COSTUME:   //  코스튬 보유 체크
                        {
                            isHaveFlag = (GameInfo.Instance.HasCostume(_storetable.ProductIndex));
                            if (!string.IsNullOrEmpty(_storedisplaytable.MarkIcon))
                            {
                                kMarkIconSpr.SetActive(true);
                                kMarkIconSpr.spriteName = _storedisplaytable.MarkIcon;
                            }
                        }
                        break;
                }

                // 보유 여부에 따른 표시 처리
                kBGSpr.gameObject.SetActive(!isHaveFlag);

                if (isHaveFlag)
                {
                    kTitleLabel.textlocalize = "";
                }

                kGoodsUnitBGSpr.gameObject.SetActive(!isHaveFlag);
                kGoodsUnit.gameObject.SetActive(!isHaveFlag);
                kBGDisSpr.gameObject.SetActive(isHaveFlag);
                kGetGO.gameObject.SetActive(isHaveFlag);
                kIconTex.color = isHaveFlag == false ? Color.white : new Color(0.45f, 0.45f, 0.45f, 1);

                // 세일 정보는 보유 중인 경우만 별도로 체크해서 SetActive(false) 처리
                if (isHaveFlag == true)
                    kSaleValueLabel.transform.parent.gameObject.SetActive(false);
            }
        }

        if (_storedisplaytable.StoreID == -1 && (_storedisplaytable.HideConType != string.Empty || _storedisplaytable.HideConType != ""))
        {
            System.DateTime endtime = GameSupport.GetTimeWithString(_storedisplaytable.HideConType, true);
            
            if (_storedisplaytable.Category == (int)UIStorePanel.eStoreTabType.STORE_CHAR)
            {
                kOpen.SetActive(true);
                kGoodsUnitBGSpr.gameObject.SetActive(true);
                kOpenDateLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(305), GameSupport.GetStartTime(endtime));
            }
            else if (_storedisplaytable.Category == (int)UIStorePanel.eStoreTabType.STORE_COSTUME)
            {
                List<GameClientTable.StoreDisplayGoods.Param> storeDisplayGoodsList =
                    GameInfo.Instance.GameClientTable.FindAllStoreDisplayGoods(x =>
                    x.PanelType == (int) eSD_PanelType.STORE &&
                    x.Category == (int) UIStorePanel.eStoreTabType.STORE_COSTUME_SUB &&
                    x.SubCategory == _storedisplaytable.SubCategory);

                foreach (GameClientTable.StoreDisplayGoods.Param storeDisplayGoods in storeDisplayGoodsList)
                {
                    if (string.IsNullOrEmpty(storeDisplayGoods.HideConType))
                    {
                        continue;
                    }
                    
                    System.DateTime subEndTime = GameSupport.GetTimeWithString(storeDisplayGoods.HideConType, true);
                    if (endtime < subEndTime)
                    {
                        endtime = subEndTime;
                    }
                }
                
                kLimitedDate.SetActive(true);
                kDateLabel.textlocalize = GameSupport.GetEndTime(endtime);
            }
        }
    }

    public void OnClick_Slot()
    {
        if (ParentGO == null)
            return;
        UIStorePanel storepanel = ParentGO.GetComponent<UIStorePanel>();
        if (storepanel == null)
            return;

        storepanel.ClickSlot(_index, _storedisplaytable);
    }

    public void ShowSaleMark(bool isShow)
    {
        kSaleMark.gameObject.SetActive(isShow);
    }

    public void OnClick_BookBtn()
    {
        if (_storedisplaytable.CharacterID > (int)eCOUNT.NONE)
        {
            Log.Show("OnClick_BookBtn");

            UIValue.Instance.SetValue(UIValue.EParamType.BookItemID, _storedisplaytable.CharacterID);

            LobbyUIManager.Instance.ShowUI("BookMainPopup", true);
            LobbyUIManager.Instance.ShowUI("BookCharListPopup", true);
            LobbyUIManager.Instance.ShowUI("BookCharInfoPopup", true);
        }
    }
}
