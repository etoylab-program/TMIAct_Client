using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public enum eSpecialLocalData
{
	SPECIAL_BUY_POPUP_OPEN_CHECK_DATE,			//팝업 오픈 날짜 체크
	SPECIAL_BUY_POPUP_STAGE_PLAY_CNT,			//스테이지 플레이 횟수 체크
	SPECIAL_BUY_POPUP_CLOSE_CNT,				//팝업 닫은지 체크(구매하지 않고)
	SPECIAL_BUY_POPUP_NEXT_WEEK,				//팝업 2번 닫은 뒤 다음열릴시간
	SPECIAL_BUY_POPUP_REMAIN_TIME,				//팝업이 열리고 구매 하지 않고 닫으면 1시간 유지 체크용
}

public class UISpecialBuyPopup : FComponent
{
	public UIButton kBuyBtn;
	public GameObject kPriceOnObj;
	public GameObject kPriceOffObj;
	public UILabel kPriceLabel;
	public UILabel kLabel;
	public UISprite kSpr;
	public UITexture kSpecialBuyPopup_Tex;

	[Header("UISpecialBuyPopup")]
	public GameObject kDateObj;
	public UILabel kTitleLabel;
	public UILabel kDateLabel;
	public UILabel kNoticeLabel;
	
	[Header("Banner Texture")]
	public UITexture kSpecialBuyPopupTex;
	public Animation kTexLoadingAnim;
	
	[Header("Package")]
	public GameObject kPackageObj;
	public FList kPackageList;
	
	[Header("Limited")]
	public GameObject kLimitedObj;
	public UILabel kLimitedLabel;
    
	[Header("Button")]
	public GameObject kBuyBtnOnObj;
	public UILabel kBuyBtnOnLabel;
	public GameObject kBuyBtnOffObj;

	[Header("WakeUp")]
	public GameObject kWakeUpObj;
	public UISprite kWakeUpCmpstBnftsSpr;
	public UILabel kWakeUpExpLabel;
	public UITexture kWakeUpCharTex;
	public List<Transform> kWakeUpRewardList;
	
	private BannerData _bannerData;
	private List<CardBookData> _preCardBookList = new List<CardBookData>();

	private GameTable.Store.Param _storeTableData;
	private GameClientTable.StoreDisplayGoods.Param _storeDisplayGoodsTableData;

	private DateTime _popupOpenLimitDateTime;
	private DateTime _limitDateTime;
	private Coroutine _limitTimeCoroutine;

	private Action _closeAction;
	
	private bool _bPackage = false;
	private GameTable.UnexpectedPackage.Param _unexpectedTableData;

	private bool _bChmuki = false;
	private int _closeBtnCnt = 0;
	private GameTable.Random.Param _rewardData;

	public class SpecialBuyData
	{
		public GameTable.UnexpectedPackage.Param UnexpectedPackageTableData;
		public GameClientTable.StoreDisplayGoods.Param StoreDisplayGoodsTableData;
		public GameTable.Character.Param CharTableData;
		public BannerData Banner;
		public DateTime EndBuyTime;

		public SpecialBuyData(GameTable.UnexpectedPackage.Param unexpectedPackageTableData,
			GameClientTable.StoreDisplayGoods.Param storeDisplayGoodsTableData,
			GameTable.Character.Param charTableData, BannerData banner, DateTime endBuyTime)
		{
			UnexpectedPackageTableData = unexpectedPackageTableData;
			StoreDisplayGoodsTableData = storeDisplayGoodsTableData;
			CharTableData = charTableData;
			Banner = banner;
			EndBuyTime = endBuyTime;
		}
	}
	
	private readonly List<SpecialBuyData> _specialBuyDataList = new List<SpecialBuyData>();
	private readonly Dictionary<int, List<UITexture>> _dicRewardTexture = new Dictionary<int, List<UITexture>>();
	private Transform _currentRewardTrans;
	
	private Coroutine _getTextureAsyncCoroutine;
	
	public override void Awake()
	{
		base.Awake();
		
		kPackageList.EventUpdate = _UpdatePackageItem;
		kPackageList.EventGetItemCount = _GetPackageItemCount;
		
		foreach (var wakeUpReward in kWakeUpRewardList)
		{
			string number = wakeUpReward.name.ToLower().Replace("wakeup", "");
			if (int.TryParse(number, out int result))
			{
				List<UITexture> list = new List<UITexture>();
				list.AddRange(wakeUpReward.GetComponentsInChildren<UITexture>());
				_dicRewardTexture.Add(result, list);
			}
		}
	}

	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}

    public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		IAPManager.Instance.IsBuying = false;
        base.OnDisable();
    }

    public override void InitComponent()
    {
	    if (_storeDisplayGoodsTableData != null)
	    {
		    kTitleLabel.textlocalize = FLocalizeString.Instance.GetText(_storeDisplayGoodsTableData.Name);
	    }
	    
		kPackageObj.SetActive(_bPackage);
		if (_bPackage)
		{
			SetBannerDataList();
			kPackageList.UpdateList();
		}
	}
 
	public override void Renewal(bool bChildren = false)
	{
		base.Renewal(bChildren);
		
		kTexLoadingAnim.gameObject.SetActive(true);
		
		Utility.StopCoroutine(this, ref _getTextureAsyncCoroutine);
		
		if (_unexpectedTableData != null)
		{
			kWakeUpObj.SetActive(_unexpectedTableData.UnexpectedType == (int)eUnexpectedPackageType.CHAR_LEVEL);
		}
		else
		{
			kWakeUpObj.SetActive(false);
		}
		
		kSpecialBuyPopupTex.mainTexture = null;
		
		if (kWakeUpObj.activeSelf)
		{
			SetWakeUpPackage();
		}
		else
		{
			if (_bannerData != null)
			{
				kSpecialBuyPopupTex.mainTexture = BannerManager.Instance.LoadTextureWithFileURL(_bannerData.UrlImage, true);
			}
		}
		
		if (kSpecialBuyPopupTex.mainTexture != null)
		{
			kTexLoadingAnim.gameObject.SetActive(false);
		}
		else
		{
			if (!kWakeUpObj.activeSelf && _bannerData != null)
			{
				_getTextureAsyncCoroutine = StartCoroutine(GetTextureAsync(_bannerData.UrlImage));
			}
		}
		
		if (_storeTableData != null)
		{
			SetPrice();
		}

		kDateObj.SetActive(!_bChmuki);
		Utility.StopCoroutine(this, ref _limitTimeCoroutine);
		if (!_bChmuki)
		{
			_limitTimeCoroutine = StartCoroutine(UpdateLimitTime());
		}
	}
	
	public override void OnClickClose()
	{
		if (IAPManager.Instance.IsBuying)
		{
			return;
		}

		if (_bChmuki)
		{
			string remainTimeTickStr = PlayerPrefs.GetString(eSpecialLocalData.SPECIAL_BUY_POPUP_REMAIN_TIME.ToString(), "0");
			DateTime remainTime = new DateTime(long.Parse(remainTimeTickStr));

			//현재 등록된 남은시간이 없으면 등록
			if (remainTime.Ticks == 0)
			{
				remainTime = DateTime.Now.AddHours(1);
				PlayerPrefs.SetString(eSpecialLocalData.SPECIAL_BUY_POPUP_REMAIN_TIME.ToString(), remainTime.Ticks.ToString());
				LobbyUIManager.Instance.Renewal("MainPanel");
			}
		}
		
		_closeAction?.Invoke();
		_closeAction = null;
		
		base.OnClickClose();
	}
 
	private void _UpdatePackageItem(int index, GameObject slotObj)
	{
		UIBannerSlot slot = slotObj.GetComponent<UIBannerSlot>();
		if (slot == null)
		{
			return;
		}
		slot.ParentGO = gameObject;
		
		SpecialBuyData data = null;
		if (index < _specialBuyDataList.Count)
		{
			data = _specialBuyDataList[index];
		}
		
		slot.UpdateSlot(UIBannerSlot.ePosType.Package, index, _unexpectedTableData.ID, data);
	}
    
	private int _GetPackageItemCount()
	{
		return _specialBuyDataList.Count;
	}
	
	private void SetBannerDataList()
	{
		if (!GameInfo.Instance.UnexpectedPackageDataDict.ContainsKey(_unexpectedTableData.UnexpectedType))
		{
			return;
		}
		
		_specialBuyDataList.Clear();
		
		DateTime serverTime = GameInfo.Instance.GetNetworkTime();
		foreach(UnexpectedPackageData data in GameInfo.Instance.UnexpectedPackageDataDict[_unexpectedTableData.UnexpectedType])
		{
			if (data.EndTime < serverTime || data.IsPurchase)
			{
				continue;
			}
			
			data.IsAdd = false;
			
			GameTable.UnexpectedPackage.Param unexpectedPackage = GameInfo.Instance.GameTable.FindUnexpectedPackage(data.TableId);
			if (unexpectedPackage == null)
			{
				continue;
			}
			
			GameTable.Store.Param store = GameInfo.Instance.GameTable.FindStore(unexpectedPackage.ConnectStoreID);
			if (store == null)
			{
				continue;
			}

			BannerData banner = null;
			GameTable.Character.Param charTableData = null;
			GameClientTable.StoreDisplayGoods.Param storeDisplayGoodsTableData = null;
			if (_unexpectedTableData.UnexpectedType == (int)eUnexpectedPackageType.ACCOUNT_LEVEL)
			{
				banner = GetBannerData(store.ProductIndex, false);
			}
			else
			{
				charTableData = GameInfo.Instance.GameTable.FindCharacter(unexpectedPackage.Value1);
				storeDisplayGoodsTableData = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == store.ID);
			}
			
			_specialBuyDataList.Add(new SpecialBuyData(unexpectedPackage, storeDisplayGoodsTableData, charTableData, banner, data.EndTime));
		}
	}
	
	private IEnumerator GetTextureAsync(string url)
	{
		while(gameObject.activeSelf)
		{
			if (kSpecialBuyPopupTex.mainTexture != null)
			{
				DestroyImmediate(kSpecialBuyPopupTex.mainTexture, false);
				kSpecialBuyPopupTex.mainTexture = null;
			}

			kSpecialBuyPopupTex.mainTexture = BannerManager.Instance.LoadTextureWithFileURL(url, true);
			
			if (kSpecialBuyPopupTex.mainTexture != null)
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
		if (GameInfo.Instance.UnexpectedPackageDataDict.ContainsKey(_unexpectedTableData.UnexpectedType))
		{
			UnexpectedPackageData data = GameInfo.Instance.UnexpectedPackageDataDict[_unexpectedTableData.UnexpectedType].
				Find(x => x.TableId == _unexpectedTableData.ID);
			if (data != null)
			{
				data.IsAdd = false;
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
			TimeSpan diffTime = limitTime - GameSupport.GetCurrentServerTime();

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
	
	private void SetWakeUpPackage()
	{
		if (_unexpectedTableData == null || _storeTableData == null)
		{
			return;
		}
		
		if (_currentRewardTrans != null)
		{
			_currentRewardTrans.gameObject.SetActive(false);
		}
		
		_currentRewardTrans = kWakeUpRewardList.Find(x => x.name.Contains(_unexpectedTableData.Value2.ToString()));
		
		if (_currentRewardTrans != null)
		{
			_currentRewardTrans.gameObject.SetActive(true);
		}
		
		string path = "UI/UITexture/Package/WakeupPackageBG" + _unexpectedTableData.Value2 + ".png";
		kSpecialBuyPopupTex.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("ui", path) as Texture;
			
		GameTable.Character.Param charTableData = GameInfo.Instance.GameTable.FindCharacter(_unexpectedTableData.Value1);
		if (charTableData != null)
		{
			path = "Icon/Char/Full/Full_" + charTableData.Icon + ".png";
			kWakeUpCharTex.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("icon", path) as Texture;
		}
		
		if (_storeDisplayGoodsTableData == null)
		{
			return;
		}
		
		kWakeUpCmpstBnftsSpr.spriteName = _storeDisplayGoodsTableData.CmpstBnfts;

		var split = Utility.Split(_storeDisplayGoodsTableData.Icon.Replace(" ", ""), ','); //_storeDisplayGoodsTableData.Icon.Replace(" ", "").Split(',');
		if (_dicRewardTexture.ContainsKey(_unexpectedTableData.Value2))
		{
			var list = _dicRewardTexture[_unexpectedTableData.Value2];
			for(int i = 0; i < list.Count; i++)
			{
				if (split.Length <= i)
				{
					break;
				}

				if (int.TryParse(split[i], out int result))
				{
					GameClientTable.GoodsIcon.Param goodsIconTableData = GameInfo.Instance.GameClientTable.FindGoodsIcon(result);
					if (goodsIconTableData == null)
					{
						continue;
					}
						
					path = "Icon/" + goodsIconTableData.Icon;
					list[i].mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("icon", path) as Texture;
				}
			}
		}
		
		GameTable.Random.Param ramdomTableData =
			GameInfo.Instance.GameTable.FindRandom(x => x.GroupID == _storeTableData.ProductIndex && x.ProductType == (int)eREWARDTYPE.BUFF);
		kWakeUpExpLabel.gameObject.SetActive(ramdomTableData != null);
	}
	
	private void DeleteListSlot(int index)
	{
		_specialBuyDataList.RemoveAt(index);
		kPackageList.UpdateList(true, false);
	}
	
	public void DeleteSlot(int tableId, bool bCloseAction = true)
	{
		int findIndex = _specialBuyDataList.FindIndex(x => x.UnexpectedPackageTableData.ID == tableId);
		if (0 <= findIndex)
		{
			IAPManager.Instance.IsBuying = false;
			if (1 < _specialBuyDataList.Count)
			{
				SetGameTable(_specialBuyDataList[findIndex == 0 ? 1 : 0].UnexpectedPackageTableData.ID);
				DeleteListSlot(findIndex);
				Renewal();
			}
			else
			{
				DeleteListSlot(0);

				if (bCloseAction)
				{
					OnClickClose();
				}
			}
		}
	}

	public void SetGameTable(int id)
	{
		_unexpectedTableData = GameInfo.Instance.GameTable.FindUnexpectedPackage(id);
		if (_unexpectedTableData == null)
		{
			return;
		}

		_bChmuki = false;
		_bPackage = (int)eUnexpectedPackageType.ACCOUNT_LEVEL <= _unexpectedTableData.UnexpectedType &&
		            _unexpectedTableData.UnexpectedType <= (int)eUnexpectedPackageType.CHAR_LEVEL;
		
		kLimitedObj.SetActive(0 < _unexpectedTableData.RepeatValue);
		if (kLimitedObj.activeSelf)
		{
			kLimitedLabel.textlocalize = FLocalizeString.Instance.GetText(1584, _unexpectedTableData.RepeatValue);
		}
		
		_storeTableData = GameInfo.Instance.GameTable.FindStore(_unexpectedTableData.ConnectStoreID);
		if (_storeTableData == null)
		{
			return;
		}
		
		_bannerData = GetBannerData(_storeTableData.ProductIndex);
		
		_storeDisplayGoodsTableData = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == _storeTableData.ID);
	}
	
	public void SetCloseAction(Action closeAction)
	{
		_closeAction = closeAction;
	}

	public void SetChmuki()
	{
		_unexpectedTableData = null;
		_bChmuki = true;
		_bPackage = false;
		
		_closeBtnCnt = PlayerPrefs.GetInt("SPECIAL_BUY_POPUP_CLOSE_CNT", 0);

		_bannerData = GameInfo.Instance.ServerData.BannerList.Find(x => x.BannerType == (int)eBannerType.SPECIAL_BUY);
		if (_bannerData == null)
		{
			return;
		}
		
		kLimitedObj.gameObject.SetActive(false);
		
		_storeTableData = GameInfo.Instance.GameTable.FindStore(x => x.ID == _bannerData.BannerTypeValue);
		if (_storeTableData != null)
		{
			_rewardData = GameInfo.Instance.GameTable.FindRandom(x => x.GroupID == _storeTableData.ProductIndex);
		}
		
		_storeDisplayGoodsTableData = GameInfo.Instance.GameClientTable.FindStoreDisplayGoods(x => x.StoreID == _storeTableData.ID);
	}

	public void ChangePackageData(int index, int unexpectedPackageTableId)
	{
		if (_specialBuyDataList.Count <= index)
		{
			return;
		}
		
		SetGameTable(unexpectedPackageTableId);
		Renewal();
		
		kPackageList.RefreshNotMove();
	}
	
	public void OnClick_BuyBtn()
	{
		if (IAPManager.Instance.IsBuying)
		{
			return;
		}

		if(!IAPManager.Instance.IAPNULL_CHECK())
        {
			MessagePopup.OK(eTEXTID.TITLE_NOTICE, FLocalizeString.Instance.GetText(3197), null);
			return;
		}
		
		if (_storeTableData == null)
		{
			Debug.LogError("StoreParam is null");
			return;
		}
		
		if (_storeDisplayGoodsTableData == null)
		{
			return;
		}

		_popupOpenLimitDateTime = _limitDateTime;
		MessagePopup.OKCANCEL(eTEXTID.BUY,
			string.Format(FLocalizeString.Instance.GetText(110), FLocalizeString.Instance.GetText(_storeDisplayGoodsTableData.Name)),
			BuyItem,
			() => {
				IAPManager.Instance.FailToUnLock();
			});
	}
	
	public void OnClick_closeBtn()
	{
		Log.Show("OnClick_closeBtn");
		
		OnClickClose();
	}

	public void OnClick_SupporterInfoBtn()
	{
		Log.Show("OnClick_SupporterInfoBtn");
		
		if (_storeDisplayGoodsTableData != null)
		{
			PackageInfoPopup.ShowPackageInfoPopup(_storeTableData.ID, UIPackagePopup.ePackageUIType.Time, _storeDisplayGoodsTableData, OnClick_BuyBtn);
		}
	}
	
	public void OnClick_PackageInfoBtn()
	{
		Log.Show("OnClick_PackageInfoBtn");
		
		if (_storeDisplayGoodsTableData != null)
		{
			PackageInfoPopup.ShowPackageInfoPopup(_storeTableData.ID, UIPackagePopup.ePackageUIType.Time, _storeDisplayGoodsTableData, OnClick_BuyBtn);
		}
	}
	
	public void OnClick_ImgBtn()
	{
		Log.Show("######");
		if (!_bChmuki || _rewardData == null)
		{
			return;
		}
		
		GameSupport.OpenRewardTableDataInfoPopup(new RewardData(_rewardData.ProductType, _rewardData.ProductIndex, _rewardData.ProductValue));
	}
	
	private void SetPrice()
	{
		if (IAPManager.Instance.IAPNULL_CHECK())
		{
			kPriceOnObj.SetActive(true);
			kPriceOffObj.SetActive(false);
			kPriceLabel.gameObject.SetActive(true);
			
	#if UNITY_EDITOR
			kPriceLabel.textlocalize = $"${_storeTableData.PurchaseValue * 0.01f:0.##}";
	#else
		#if UNITY_ANDROID
			kPriceLabel.textlocalize = IAPManager.Instance.GetPrice(_storeTableData.AOS_ID);
		#elif UNITY_IOS
            kPriceLabel.textlocalize = IAPManager.Instance.GetPrice(_storeTableData.IOS_ID);
		#elif !DISABLESTEAMWORKS
            kPriceLabel.textlocalize = $"${_storeTableData.PurchaseValue * 0.01f:0.##}";
		#endif
	#endif
		}
		else
		{
			kPriceOnObj.SetActive(false);
			kPriceOffObj.SetActive(true);
			kPriceLabel.gameObject.SetActive(false);
		}
	}
	
	private void BuyItem()
	{
		if (!_bChmuki && _popupOpenLimitDateTime < GameInfo.Instance.GetNetworkTime())
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

		_preCardBookList.Clear();
		_preCardBookList.AddRange(GameInfo.Instance.CardBookList);
		
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
		
		WaitPopup.Hide();
		
		if (!_bChmuki)
		{
			DeleteSlot(_unexpectedTableData.ID, false);
		}
		
		string title = FLocalizeString.Instance.GetText(1322);
		string desc = FLocalizeString.Instance.GetText(1323);
		MessageRewardListPopup.RewardListMessage(title, desc, GameInfo.Instance.RewardList, OnMessageRewardCallBack);
		
		LobbyUIManager.Instance.HideUI("PackageInfoPopup");
		LobbyUIManager.Instance.Renewal("TopPanel");
	}

	private void OnMessageRewardCallBack()
	{
		if (_bChmuki)
		{
			DirectorUIManager.Instance.PlayNewCardGreeings(_preCardBookList);
			
			//구매하면 남은시간 초기화
			PlayerPrefs.SetString(eSpecialLocalData.SPECIAL_BUY_POPUP_REMAIN_TIME.ToString(), "0");

			base.OnClickClose();
		}
		else
		{
			if (_specialBuyDataList.Count <= 0)
			{
				OnClickClose();
			}
		}
	}
}
