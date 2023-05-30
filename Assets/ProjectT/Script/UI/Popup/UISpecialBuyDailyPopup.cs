using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UISpecialBuyDailyPopup : FComponent
{
    [Header("UISpecialBuyDailyPopup")]
    public GameObject kDateObj;
    public UILabel kTitleLabel;
    public UILabel kDateLabel;
    public UILabel kNoticeLabel;
    
    [Header("Banner Texture")]
    public UITexture kBeginnerTex;
    public Animation kTexLoadingAnim;
    
    [Header("Daily")]
    public FList kDailyList;

    [Header("Limited")]
    public GameObject kLimitedObj;
    public UILabel kLimitedLabel;
    
    [Header("Button")]
    public GameObject kCommonBtnOnObj;
    public UILabel kCommonBtnOnLabel;
    public GameObject kCommonBtnOffObj;

    private DateTime _popupOpenLimitDateTime;
    private DateTime _limitDateTime;
    private Coroutine _limitTimeCoroutine;
    
    // Pre Setting
    private BannerData _bannerData;
    private int _unexpectedTableId = -1;
    private int _unexpectedType = -1;
    private bool _buyPackage;
    private byte _dailyFlag;
    private DateTime _endRewardDaysTime;
    private Action _closeAction;
    
    private GameTable.Store.Param _storeTableData;
    private readonly Dictionary<int, List<GameTable.Random.Param>> _dicRandomTableData = new Dictionary<int, List<GameTable.Random.Param>>();
    
    private Coroutine _getTextureAsyncCoroutine;
    
    public override void Awake()
    {
        base.Awake();
        
        kDailyList.EventUpdate = _UpdateDailyItem;
        kDailyList.EventGetItemCount = _GetDailyItemCount;
    }

    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public override void InitComponent()
    {
        base.InitComponent();
        
        kTexLoadingAnim.gameObject.SetActive(true);
        
        Utility.StopCoroutine(this, ref _getTextureAsyncCoroutine);
        
        if (kBeginnerTex.mainTexture != null)
        {
            DestroyImmediate(kBeginnerTex.mainTexture, false);
			kBeginnerTex.mainTexture = null;
		}
        
        if (_bannerData != null)
        {
            kBeginnerTex.mainTexture = BannerManager.Instance.LoadTextureWithFileURL(_bannerData.UrlImage, true);
        }

        if (kBeginnerTex.mainTexture != null)
        {
            kTexLoadingAnim.gameObject.SetActive(false);
        }
        else
        {
            if (_bannerData != null)
            {
                _getTextureAsyncCoroutine = StartCoroutine(GetTextureAsync(_bannerData.UrlImage));
            }
        }
        
        // Store Info Setting
        if (_storeTableData != null)
        {
            SetPrice();
            GameClientTable.StoreDisplayGoods.Param storeDisplayGoods =
                GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == _storeTableData.ID);

            if (storeDisplayGoods != null)
            {
                kTitleLabel.textlocalize = FLocalizeString.Instance.GetText(storeDisplayGoods.Name);
                kNoticeLabel.textlocalize = FLocalizeString.Instance.GetText(storeDisplayGoods.Description);
            }
        }

        kDailyList.UpdateList();
    }
    
    public override void Renewal(bool bChildren = false)
    {
        base.Renewal(bChildren);
        
        if (GameInfo.Instance.UnexpectedPackageDataDict.ContainsKey(_unexpectedType))
        {
            UnexpectedPackageData data = GameInfo.Instance.UnexpectedPackageDataDict[_unexpectedType].Find(x => x.TableId == _unexpectedTableId);
            if (data != null)
            {
                _buyPackage = data.IsPurchase;
                if (_buyPackage)
                {
                    _endRewardDaysTime = data.EndTime;
                }
                
                data.IsAdd = false;
                _dailyFlag = data.RewardBitFlag;
                
                kDailyList.RefreshNotMove();
            }
        }
        
        kLimitedObj.SetActive(!_buyPackage);
        kCommonBtnOnObj.SetActive(!_buyPackage);
        kCommonBtnOffObj.SetActive(_buyPackage);
        
        kDateObj.SetActive(!_buyPackage);
        Utility.StopCoroutine(this, ref _limitTimeCoroutine);
        if (kDateObj.activeSelf)
        {
            _limitTimeCoroutine = StartCoroutine(UpdateLimitTime());
        }
    }

    public override void OnClickClose()
    {
        _closeAction?.Invoke();
        _closeAction = null;
        
        base.OnClickClose();
    }
    
    private IEnumerator GetTextureAsync(string url)
    {
        while(gameObject.activeSelf)
        {
			if (kBeginnerTex.mainTexture != null)
			{
				DestroyImmediate(kBeginnerTex.mainTexture, false);
				kBeginnerTex.mainTexture = null;
			}

			kBeginnerTex.mainTexture = BannerManager.Instance.LoadTextureWithFileURL(url, true);
			
            if (kBeginnerTex.mainTexture != null)
            {
                kTexLoadingAnim.gameObject.SetActive(false);
                break;
            }
			
            yield return null;
        }
    }
    
    private BannerData GetBannerData(int productIndex, bool bBackground = true)
    {
        int type = (int)(bBackground ? eBannerType.UNEXPECTED_PACKAGE : eBannerType.UNEXPECTED_PACKAGE_BANNER);
        return GameInfo.Instance.ServerData.BannerList.Find(x => x.BannerType == type && x.BannerTypeValue == productIndex);
    }
    
    private IEnumerator UpdateLimitTime()
    {
        bool bContinue = true;
        DateTime limitTime = default;
        if (GameInfo.Instance.UnexpectedPackageDataDict.ContainsKey(_unexpectedType))
        {
            UnexpectedPackageData data = GameInfo.Instance.UnexpectedPackageDataDict[_unexpectedType].Find(x => x.TableId == _unexpectedTableId);
            if (data != null)
            {
                _limitDateTime = limitTime = data.EndTime;
            }
            else
            {
                bContinue = false;
            }
        }
        
        var waitForSeconds = new WaitForSeconds(1);
        while (bContinue)
        {
            TimeSpan diffTime = limitTime - GameInfo.Instance.GetNetworkTime();
            
            string result;
            if (0 < diffTime.Hours)
            {
                result = FLocalizeString.Instance.GetText(261, diffTime.Hours, diffTime.Minutes);
            }
            else
            {
                result = FLocalizeString.Instance.GetText(262, diffTime.Minutes);
            }
            
            kDateLabel.textlocalize = FLocalizeString.Instance.GetText(1768, result);

            if (diffTime.Ticks <= 0)
            {
                bContinue = false;
            }
            
            yield return waitForSeconds;
        }
    }
    
    private void _UpdateDailyItem(int index, GameObject slotObj)
    {
        UIDailySlot slot = slotObj.GetComponent<UIDailySlot>();
        if (slot == null)
        {
            return;
        }
        slot.ParentGO = gameObject;
        
        GameTable.Random.Param[] randomArray = null;
        int day = index + 1;
        if (_dicRandomTableData.ContainsKey(day))
        {
            randomArray = _dicRandomTableData[day].ToArray();
        }
        
        TimeSpan diffTime = GameInfo.Instance.GetNetworkTime() - _endRewardDaysTime;
        bool bBtnEnable = day <= diffTime.Days + 1;
        bool bRewardComplete = bBtnEnable;
        if (bBtnEnable)
        {
            bBtnEnable = IsRewardFlag(index) == false;
            bRewardComplete = IsRewardFlag(index);
        }
        
        slot.UpdateSlot(index, _unexpectedTableId, randomArray, _buyPackage, bBtnEnable, bRewardComplete);
    }
    
    private int _GetDailyItemCount()
    {
        return GetLastDay();
    }

    private int GetLastDay()
    {
        int lastDay = 0;
        int index = _dicRandomTableData.Count - 1;
        if (0 < index)
        {
            lastDay = _dicRandomTableData[index].LastOrDefault()?.Value ?? 0;
        }
        
        return lastDay;
    }

    public void SetGameTable(int id)
    {
        GameTable.UnexpectedPackage.Param packageParam = GameInfo.Instance.GameTable.FindUnexpectedPackage(id);
        if (packageParam == null)
        {
            return;
        }
        
        _unexpectedTableId = id;
        _unexpectedType = packageParam.UnexpectedType;
        
        if (0 < packageParam.RepeatValue)
        {
            kLimitedLabel.textlocalize = FLocalizeString.Instance.GetText(1584, packageParam.RepeatValue);
        }
        
        _storeTableData = GameInfo.Instance.GameTable.FindStore(packageParam.ConnectStoreID);
        if (_storeTableData == null)
        {
            return;
        }
        
        _bannerData = GetBannerData(_storeTableData.ProductIndex);
        
        List<GameTable.Random.Param> list =
            GameInfo.Instance.GameTable.FindAllRandom(x => x.GroupID == _storeTableData.ProductIndex);

        foreach (GameTable.Random.Param param in list)
        {
            if (_dicRandomTableData.ContainsKey(param.Value))
            {
                _dicRandomTableData[param.Value].Add(param);
            }
            else
            {
                _dicRandomTableData.Add(param.Value, new List<GameTable.Random.Param>{ param });
            }
        }
    }
    
    public void SetCloseAction(Action closeAction)
    {
        _closeAction = closeAction;
    }

    private bool IsRewardFlag(int index)
    {
        int flag = 1 << index;
        if ((_dailyFlag & flag) == flag)
        {
            return true;
        }

        return false;
    }

    private void OnNetReward(int result, PktMsgType pktmsg)
    {
        if (result < 0)
        {
            return;
        }
        
        kDailyList.RefreshNotMove();
        
        string title = FLocalizeString.Instance.GetText(1322);
        string desc = FLocalizeString.Instance.GetText(1323);
        MessageRewardListPopup.RewardListMessage(title, desc, GameInfo.Instance.RewardList, null);
        
        LobbyUIManager.Instance.Renewal("TopPanel");
    }
    public void OnClick_Reward(int index)
    {
        Renewal();
        
        if (!IsRewardFlag(index))
        {
            return;
        }
        
        OnNetReward(0, null);
    }

    public void OnClick_InfoBtn()
    {
        Log.Show("OnClick_HelpBtn");

        GameClientTable.StoreDisplayGoods.Param goods =
            GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == _storeTableData.ID);

        if (goods != null)
        {
            PackageInfoPopup.ShowPackageSubInfoPopup(_storeTableData.ID, (int)eCOUNT.NONE, UIPackagePopup.ePackageUIType.Time, goods, OnClick_CommonBuyBtn);
        }
    }
    
    public void OnClick_CommonBuyBtn()
    {
        if (_buyPackage)
        {
            return;
        }
        
        Log.Show("OnClick_CommonBuyBtn");
        
        GameClientTable.StoreDisplayGoods.Param sendStoreDisplayTableData =
            GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == _storeTableData.ID);
        if (sendStoreDisplayTableData == null)
        {
            return;
        }
        
        _popupOpenLimitDateTime = _limitDateTime;
        
        MessagePopup.OKCANCEL(eTEXTID.BUY,
            string.Format(FLocalizeString.Instance.GetText(110), FLocalizeString.Instance.GetText(sendStoreDisplayTableData.Name)),
            BuyItem,
            () => {
                IAPManager.Instance.FailToUnLock();
            });
    }
    
    public void OnClick_CloseBtn()
    {
        Log.Show("OnClick_CloseBtn");
        
        OnClickClose();
    }
    
    private void SetPrice()
    {
#if UNITY_EDITOR
        kCommonBtnOnLabel.textlocalize = $"${_storeTableData.PurchaseValue * 0.01f:0.##}";
#else
    #if UNITY_ANDROID
        kCommonBtnOnLabel.textlocalize = IAPManager.Instance.GetPrice(_storeTableData.AOS_ID);
    #elif UNITY_IOS
        kCommonBtnOnLabel.textlocalize = IAPManager.Instance.GetPrice(_storeTableData.IOS_ID);
    #elif !DISABLESTEAMWORKS
        kCommonBtnOnLabel.textlocalize = $"${_storeTableData.PurchaseValue * 0.01f:0.##}";
    #endif
#endif
    }

    private void BuyItem()
    {
        if (_popupOpenLimitDateTime < GameInfo.Instance.GetNetworkTime())
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(111402));
            return;
        }
        
        WaitPopup.Show();
        IAPManager.Instance.IsBuying = true;

        string storeId = "";
        bool bFail = false;
        
#if UNITY_EDITOR
        storeId = _storeTableData.ID.ToString();
        if (_storeTableData.PurchaseValue <= 0)
        {
            bFail = true;
        }
#else
    #if UNITY_ANDROID
        storeId = _storeTableData.AOS_ID;
        if (string.IsNullOrEmpty(storeId))
        {
            bFail = true;
        }
    #elif UNITY_IOS
        storeId = _storeTableData.IOS_ID;
        if (string.IsNullOrEmpty(storeId))
        {
            bFail = true;
        }
    #elif !DISABLESTEAMWORKS
		storeId = _storeTableData.ID.ToString();
        if (_storeTableData.PurchaseValue <= 0)
        {
            bFail = true;				
        }
    #endif
#endif
        if (bFail)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3028));
            IAPManager.Instance.FailToUnLock();
        }
        else
        {
        #if UNITY_EDITOR
            OnIAPBuySuccess();
        #else
            IAPManager.Instance.BuyIAPProduct(storeId, OnIAPBuySuccess, OnIAPBuyFailed);
        #endif
        }
    }
    
    private void OnIAPBuySuccess()
    {
        PlayerPrefs.SetInt("IAPBuyStoreID", _storeTableData.ID);
        PlayerPrefs.Save();
        
#if UNITY_EDITOR
        GameInfo.Instance.Send_ReqStorePurchase(_storeTableData.ID, false, 1, OnNetPurchase);
#else
	#if !DISABLESTEAMWORKS
		GameInfo.Instance.Send_ReqSteamPurchase(_storeTableData.ID, OnNetPurchase);
    #else
        GameInfo.Instance.Send_ReqStorePurchaseInApp(IAPManager.Instance.Receipt, _storeTableData.ID, OnNetPurchase);
	#endif
#endif
    }

    private void OnIAPBuyFailed()
    {
        WaitPopup.Hide();
        IAPManager.Instance.IsBuying = false;

        MessageToastPopup.Show(FLocalizeString.Instance.GetText(3212));
    }
    
    private void OnNetPurchase(int result, PktMsgType pktmsg)
    {
        PlayerPrefs.DeleteKey("IAPBuyReceipt");
        PlayerPrefs.DeleteKey("IAPBuyStoreID");
        PlayerPrefs.Save();

        IAPManager.Instance.ResetReceipt();
        IAPManager.Instance.IsBuying = false;
        WaitPopup.Hide();
        
        kDailyList.UpdateList(preInit: false);
        
        Renewal();
        
        string title = FLocalizeString.Instance.GetText(1322);
        string desc = FLocalizeString.Instance.GetText(1323);
        MessageRewardListPopup.RewardListMessage(title, desc, GameInfo.Instance.RewardList, null);
        
        LobbyUIManager.Instance.HideUI("PackageInfoPopup");
        LobbyUIManager.Instance.Renewal("TopPanel");
    }
}
