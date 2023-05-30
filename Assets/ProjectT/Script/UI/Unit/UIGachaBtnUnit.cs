using UnityEngine;
using System.Collections;

public class UIGachaBtnUnit : FUnit
{
	public GameObject Root;

	public GameObject kBasic;
    public GameObject kFree;
    public GameObject kSale;
    public UISprite kSaleSpr;
    public GameObject kFreeRemain;
    public UISprite kDisabledSpr;
    public UILabel kNoLabel;
    public UILabel kTextLabel;
    public UILabel kFreeRemainTextLabel;
    public UIGoodsUnit kBasicGoodsUnit;
	public UIGoodsUnit kSaleGoodsUnit;
	public UILabel kSaleRateLabel;
	public UILabel kSaleValueLabel;
    public UISprite kMarkSpr;

    public GameObject kMileageObj;
    public UISprite kMileageSpr;
    public GameObject kSRObj;

    public GameObject kTicketObj;
    public UITexture kTicketIconTex;
    public UILabel kTicketCntLabel;
    public GameObject kTicketInfoObj;
    public UITexture kTicketNeedIconTex;
    public UILabel kTicketNeedCntLabel;

	[Header("[Additional]")]
	public GameObject	DisablePurchase;
	public UILabel		LbResetRemainTime;

	public bool IsFree100Gacha { get; private set; } = false;

    private GameClientTable.StoreDisplayGoods.Param _storedisplaytable;
    private GameTable.Store.Param _storetable;
    private StoreSaleData _storesaledata;
    private bool _bsaleapply;
    private int _discountrate;

    //public bool IsFree { get { return _bfree;  } }

    public GameClientTable.StoreDisplayGoods.Param StoreDisplayTable { get { return _storedisplaytable; } }
    public GameTable.Store.Param StoreTable { get { return _storetable; } }
    public StoreSaleData StoreSaleData { get { return _storesaledata; } }

    private GameClientTable.StoreDisplayGoods.Param _ticketStoredisplaytable;
    private GameTable.Store.Param _ticketStoreTable;
    public GameClientTable.StoreDisplayGoods.Param StoreTicketDisplayTable { get { return _ticketStoredisplaytable; } }
    public GameTable.Store.Param StoreTicketTable { get { return _ticketStoreTable; } }


    public void InitGachaBtnUnit(int displayid, bool baseshow = true)     //Fill parameter if you need
    {
		Root.SetActive(true);

		if (DisablePurchase)
		{
			DisablePurchase.SetActive(false);
		}

		IsFree100Gacha = false;

		kDisabledSpr.gameObject.SetActive(false);
        _discountrate = 0;
        _bsaleapply = false;
        _storesaledata = null;
        _storedisplaytable = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(displayid);
        if (_storedisplaytable == null)
            return;
        _storetable = GameInfo.Instance.GameTable.FindStore(_storedisplaytable.StoreID);
        if (_storetable == null)
            return;

        _ticketStoredisplaytable = null;
        _ticketStoreTable = null;

        _storesaledata = GameInfo.Instance.ServerData.StoreSaleList.Find(x => x.TableID == _storetable.ID);
        kNoLabel.textlocalize = _storetable.ProductValue.ToString();
        kTextLabel.textlocalize = FLocalizeString.Instance.GetText(1087);//FLocalizeString.Instance.GetText(_storedisplaytable.Name);

        kMileageObj.SetActive(false);
        kSRObj.SetActive(false);

        kBasic.SetActive(true);
        kBasicGoodsUnit.gameObject.SetActive(false);
        kFree.SetActive(false);
        kSale.SetActive(false);
        kFreeRemain.SetActive(false);
        kTicketObj.SetActive(false);
        kTicketInfoObj.SetActive(false);
        if (!baseshow)
            return;

        if(_storetable.BonusGoodsType == (int)eGOODSTYPE.SUPPORTERPOINT)
        {
            kMileageObj.SetActive(true);
            kMileageSpr.spriteName = string.Format("ico_Gacha_MileageGet_{0}", _storetable.BonusGoodsValue);
            kSRObj.SetActive(true);
        }

        _discountrate = GameSupport.GetStoreDiscountRate(_storetable.ID);
        if(_discountrate == 0) //원가
        {
            kBasicGoodsUnit.gameObject.SetActive(true);
            kBasicGoodsUnit.InitGoodsUnit((eGOODSTYPE)_storetable.PurchaseIndex, _storetable.PurchaseValue, false);
        }
        else
        {
            System.DateTime networkTime = GameInfo.Instance.GetNetworkTime();

            _bsaleapply = GameSupport.IsStoreSaleApply(_storetable.ID);
            var storemydata = GameInfo.Instance.GetStoreData(_storetable.ID);
            if (_discountrate == 100) //무료
            {
                if (_bsaleapply)
                {
                    kFree.SetActive(true);
                }
                else
                {
					kFreeRemain.SetActive(true);

					System.DateTime remaintime;
					UILabel lb = null;

					if (_storesaledata.LimitType == (int)eStoreSaleKind.CycleMinute)
					{
						remaintime = storemydata.GetTime();
						lb = kFreeRemainTextLabel;
					}
					else
					{
						remaintime = storemydata.GetTimeByResetTime();

						if (DisablePurchase && _storesaledata.LimitType == (int)eStoreSaleKind.LimitDate_Day) // 무료 100연차
						{
							IsFree100Gacha = true;

							Root.SetActive(false);
							kFreeRemain.SetActive(false);

							DisablePurchase.SetActive(true);

							if (LbResetRemainTime)
							{
								lb = LbResetRemainTime;
							}
						}
					}

					string startTime = GameSupport.GetRemainTimeString(GameSupport.GetLocalTimeByServerTime(remaintime), networkTime);
					lb.textlocalize = string.Format(FLocalizeString.Instance.GetText(1059), startTime);

					kBasicGoodsUnit.gameObject.SetActive(true);
                    kBasicGoodsUnit.InitGoodsUnit((eGOODSTYPE)_storetable.PurchaseIndex, _storetable.PurchaseValue, false);
                }
            }
            else //세일중
            {
                if (_bsaleapply)
                {
                    int salevalue = _storetable.PurchaseValue - (int)(_storetable.PurchaseValue * (float)((float)_discountrate / (float)eCOUNT.MAX_BO_FUNC_VALUE));

                    kSale.SetActive(true);
                    kSaleGoodsUnit.InitGoodsUnit((eGOODSTYPE)_storetable.PurchaseIndex, salevalue, false); //할인된 가격

                    if (_storesaledata.LimitType == (int)eStoreSaleKind.LimitCnt)
                    {
                        if (_storesaledata.LimitValue > (int)eCOUNT.NONE + 1)
                        {
                            kSaleSpr.spriteName = "ico_EventShopBonus";
                        }
                        else
                        {
                            kSaleSpr.spriteName = "ico_GachaBonus";
                        }
                    }

                    kSaleValueLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.GOODSTEXT_W), _storetable.PurchaseValue); //기존 가격
                    kSaleRateLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.PERCENT_TEXT), _discountrate); //할인율
                }
                else
                {
                   if (_storesaledata.LimitType == (int)eStoreSaleKind.CycleMinute)
                    {
                        kFreeRemain.SetActive(true);
                        System.DateTime remaintime = storemydata.GetTime();
                        string strtime = GameSupport.GetRemainTimeString(GameSupport.GetLocalTimeByServerTime(remaintime), networkTime);
                        kFreeRemainTextLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(1059), strtime);
                    }

                    kBasicGoodsUnit.gameObject.SetActive(true);
                    kBasicGoodsUnit.InitGoodsUnit((eGOODSTYPE)_storetable.PurchaseIndex, _storetable.PurchaseValue, false);
                }
            }
        }
    }

    public void InitGachaBtnUnit(int displayid, int ticketid, bool baseshow = true)     //Fill parameter if you need
    {
		Root.SetActive(true);

		if (DisablePurchase)
		{
			DisablePurchase.SetActive(false);
		}

		IsFree100Gacha = false;

		kDisabledSpr.gameObject.SetActive(false);
        _discountrate = 0;
        _bsaleapply = false;
        _storesaledata = null;
        
        _storedisplaytable = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(displayid);
        if (_storedisplaytable == null)
            return;
        
        _storetable = GameInfo.Instance.GameTable.FindStore(_storedisplaytable.StoreID);
        if (_storetable == null)
            return;

        if (ticketid <= (int)eCOUNT.NONE)
        {
            InitGachaBtnUnit(displayid, baseshow);
            return;
        }

        _ticketStoredisplaytable = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(ticketid);
        if (_ticketStoredisplaytable == null)
            return;

        _ticketStoreTable = GameInfo.Instance.GameTable.FindStore(_ticketStoredisplaytable.StoreID);
        if (_ticketStoreTable == null)
            return;

        _storesaledata = GameInfo.Instance.ServerData.StoreSaleList.Find(x => x.TableID == _storetable.ID);
        kNoLabel.textlocalize = _storetable.ProductValue.ToString();
        kTextLabel.textlocalize = FLocalizeString.Instance.GetText(1087);//FLocalizeString.Instance.GetText(_storedisplaytable.Name);

        kMileageObj.SetActive(false);
        kSRObj.SetActive(false);

        kBasic.SetActive(true);
        kBasicGoodsUnit.gameObject.SetActive(false);
        kFree.SetActive(false);
        kSale.SetActive(false);
        kFreeRemain.SetActive(false);
        kTicketObj.SetActive(false);
        if (!baseshow)
            return;

        int ticketCnt = GameInfo.Instance.GetItemIDCount(_ticketStoreTable.PurchaseIndex);

        if (ticketCnt >= _ticketStoreTable.PurchaseValue)
        {
            ItemData ticketItemData = GameInfo.Instance.GetItemData(_ticketStoreTable.PurchaseIndex);
            
            kTicketObj.SetActive(true);
            kTicketIconTex.mainTexture = (Texture2D)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + ticketItemData.TableData.Icon);
            kTicketCntLabel.textlocalize = FLocalizeString.Instance.GetText(228, ticketCnt);

            kTicketInfoObj.SetActive(true);
            kTicketNeedIconTex.mainTexture = (Texture2D)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + ticketItemData.TableData.Icon);
            kTicketNeedCntLabel.textlocalize = _ticketStoreTable.PurchaseValue.ToString();

            //SR확정이 있는지 여부
            if (_ticketStoreTable.Value1 > 0 )
            {
                kSRObj.SetActive(true);
            }
        }
        else
        {
            InitGachaBtnUnit(displayid, baseshow);
        }
    }


    public void OnClick_Slot()
	{
	}
}
