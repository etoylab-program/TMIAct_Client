using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !UNITY_STANDALONE
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using UnityEngine.Accessibility;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
#endif

#if !UNITY_STANDALONE
public class IAPManager :  MonoBehaviour, IStoreListener
{
    protected static IAPManager instance;

    public static IAPManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType(typeof(IAPManager)) as IAPManager;
            }
            if(instance == null)
            {
                GameObject obj = new GameObject(typeof(IAPManager).Name);
                instance = obj.AddComponent(typeof(IAPManager)) as IAPManager;
            }

            return instance;
        }
    }


    public delegate void OnIAPCallBack();

    private IStoreController _storeController = null;
    private List<string> _ProductIds;

    private string _logStr;
    
    private OnIAPCallBack kOnIAPSuccess;
    private OnIAPCallBack kOnIAPFailed;

    private string m_Receipt;
    public string Receipt { get { return m_Receipt; } }
    private string m_buyStoreID;
    public string BuyStoreID {  get { return m_buyStoreID; } }


    public bool IsBuying = false;
    void Awake()
    {
        if (instance == null)
        {
            instance = gameObject.GetComponent<IAPManager>();
        }
        else if (instance != this)
        {
            Debug.Log("A Singleton already exists. Destroy new one");
            Destroy(this.gameObject);
        }

        IsBuying = false;
    }

    public virtual void OnDestroy()
    {
        instance = null;
    }

    public void Init()
    {
        Log.Show("Init Unity Gaming Services");
        var options = new InitializationOptions().SetEnvironmentName("production");
        UnityServices.InitializeAsync(options);

        Log.Show("Init IAP Manager");
        DontDestroyOnLoad(this);

        _storeController = null;

        if (_storeController == null)
        {
            _ProductIds = new List<string>();
            //List<GameTable.Store.Param> storeParam = GameInfo.Instance.GameTable.FindAllStore(x => x.ProductType == (int)eREWARDTYPE.GOODS || x.ProductType == (int)eREWARDTYPE.PACKAGE || x.ProductType == (int)eREWARDTYPE.PASS || x.ProductType == (int)eREWARDTYPE.MONTHLYFEE);
#if UNITY_EDITOR
            List<GameTable.Store.Param> storeParam = GameInfo.Instance.GameTable.Stores;
#else
            List<GameTable.Store.Param> storeParam = GameInfo.Instance.GameTable.FindAllStore(x => x.PurchaseType == (int)eREWARDTYPE.GOODS && x.PurchaseIndex == (int)eGOODSTYPE.GOODS);
#endif
            if (storeParam == null)
            {
                Debug.LogError("StoreParam is null");
                return;
            }
            for (int i = 0; i < storeParam.Count; i++)
            {
#if UNITY_ANDROID
               
                if (string.IsNullOrEmpty(storeParam[i].AOS_ID))
                    continue;
                if (_ProductIds.Contains(storeParam[i].AOS_ID))
                    continue;
                Debug.Log("AOS_ID : " + storeParam[i].AOS_ID);
                _ProductIds.Add(storeParam[i].AOS_ID.Replace(" ",""));
#elif UNITY_IOS
                if (string.IsNullOrEmpty(storeParam[i].IOS_ID))
                    continue;
                if (_ProductIds.Contains(storeParam[i].IOS_ID))
                    continue;
                _ProductIds.Add(storeParam[i].IOS_ID.Replace(" ",""));

#elif !DISABLESTEAMWORKS
                if (_ProductIds.Contains(storeParam[i].ID.ToString()))
                    continue;
                _ProductIds.Add(storeParam[i].ID.ToString());
#endif
            }

            InitStore();
        }
    }

    void InitStore()
    {
        Debug.Log("IAPManager InitStore()");
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        for(int i = 0; i < _ProductIds.Count; i++)
        {
            builder.AddProduct(_ProductIds[i], ProductType.Consumable, new IDs { { _ProductIds[i], AppleAppStore.Name }, {_ProductIds[i], GooglePlay.Name } });
        }
        UnityPurchasing.Initialize(this, builder);
        
    }

    public void BuyIAPProduct(string buyID, OnIAPCallBack successCallback, OnIAPCallBack failedCallback)
    {
        
        kOnIAPSuccess = successCallback;
        kOnIAPFailed = failedCallback;
        m_buyStoreID = buyID;
        if (!_ProductIds.Contains(buyID))
        {
            Debug.LogError("등록된 상품이 아닙니다.");
            return;
        }
#if !DISABLESTEAMWORKS
        successCallback?.Invoke();
#else
        _storeController.InitiatePurchase(buyID);
#endif
    }

    /// <summary>
    /// 결제모듈 초기화 성공
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="extensions"></param>
    void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {

        Debug.Log("OnInitialized : PASS");
        Debug.Log("Available Items : ");
        
        foreach(Product item in controller.products.all)
        {
            if(item.availableToPurchase)
            {
                Debug.Log(string.Join(" - ", new[]
                {
                    "TransactionID : " + item.transactionID,
                    "Metadata.LocalizedTitle : " + item.metadata.localizedTitle,
                    "Metadata.LocalizedDescription" + item.metadata.localizedDescription,
                    "Metadata.isoCurrencyCode" + item.metadata.isoCurrencyCode,
                    "Metadata.LocalizedPrice" + item.metadata.localizedPrice.ToString(),
                    "Metadata.LocalizedPriceString" + item.metadata.localizedPriceString,
                    "Receipt : " + item.receipt
                }));
            }
        }

        _storeController = controller;
        
        Debug.Log("IStoreController 초기화");
       
    }

    /// <summary>
    /// 결제모듈 초기화 실패
    /// </summary>
    /// <param name="error"></param>
    void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("IStoreController 초기화 실패");
        switch(error)
        {
            case InitializationFailureReason.AppNotKnown:
                Debug.Log("Is Your App Correctly uploaded on the relevane publisher console?");
                break;
            case InitializationFailureReason.PurchasingUnavailable:
                Debug.Log("Billing disabled");
                break;
            case InitializationFailureReason.NoProductsAvailable:
                Debug.Log("No products available for purchase!");
                break;
        }
    }

    /// <summary>
    /// 인앱 구매 성공
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs e)
    {
        bool isSuccess = false;

        //if (e.purchasedProduct.definition.id.Equals(m_buyStoreID))
        //{
            
        //}
        
        Debug.Log("구매성공");
        Debug.Log("Receipt : " + e.purchasedProduct.receipt);
        Debug.Log(e.purchasedProduct);
        m_Receipt = e.purchasedProduct.receipt;

        PlayerPrefs.SetString("IAPBuyReceipt", m_Receipt);
        PlayerPrefs.Save();


        if (kOnIAPSuccess != null)
            kOnIAPSuccess();
        //m_Receipt = string.Empty;
        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// 인앱 구매 실패
    /// </summary>
    /// <param name="i"></param>
    /// <param name="error"></param>
    void IStoreListener.OnPurchaseFailed(Product i, PurchaseFailureReason error)
    {
        if(!error.Equals(PurchaseFailureReason.UserCancelled))
        {
            Debug.Log("구매실패");
        }

        if (kOnIAPFailed != null)
            kOnIAPFailed();
    }

    /// <summary>
    /// 결제 영수증 저장 정보 초기화
    /// </summary>
    public void ResetReceipt()
    {
        m_Receipt = string.Empty;
    }

	/// <summary>
	/// 현지화 가격
	/// </summary>
	/// <param name="productID"></param>
	/// <returns></returns>
	public string GetPrice( string productID ) {
        if ( _storeController == null || _storeController.products == null ) {
            Init();
        }

        if ( _storeController.products == null ) {
            return string.Empty;
		}

        Product product = _storeController.products.WithID( productID );
        if( product == null || product.metadata == null ) {
            return string.Empty;
		}

        return _storeController.products.WithID( productID ).metadata.localizedPriceString;
	}

	/// <summary>
	/// AppStore, Google Play Store 계정이 꼬여서 결제모듈이 초기화 되지 않았을때
	/// </summary>
	/// <returns></returns>
	public bool IAPNULL_CHECK()
    {
#if !DISABLESTEAMWORKS
        return true;
#endif

        if (_storeController == null)
        {
            Debug.LogError("_storeController is null");
            return false;
        }

        if(_storeController.products == null)
        {
            Debug.LogError("_storeController.products is null");
            return false;
        }

		if(_ProductIds == null || _ProductIds.Count <= 0)
		{
			Debug.LogError("_ProductIds.Count is 0");
			return false;
		}

        if (_storeController.products.WithID(_ProductIds[0]) == null)
        {
            Debug.LogError("_storeController.products.WithID is null");
            return false;
        }

        if (_storeController.products.WithID(_ProductIds[0]).metadata == null)
        {
            Debug.LogError("_storeController.products.WithID(_ProductIds[0]).metadata is null");
            return false;
        }

        //if (_storeController.products.WithID(_ProductIds[0]).metadata.localizedPriceString == null)
        //{
        //    Debug.LogError("_storeController.products.WithID(_ProductIds[0]).metadata.localizedPriceString is null");
        //    return false;
        //}

        return true;
    }

    /// <summary>
    /// 결제 시도 실패 후 게임 Lock 해제
    /// </summary>
    public void FailToUnLock()
    {
        WaitPopup.Hide();
        IsBuying = false; 
    }
}
#else
public class IAPManager : MonoBehaviour
{
    protected static IAPManager instance;

    public static IAPManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(IAPManager)) as IAPManager;
            }
            if (instance == null)
            {
                GameObject obj = new GameObject(typeof(IAPManager).Name);
                instance = obj.AddComponent(typeof(IAPManager)) as IAPManager;
            }

            return instance;
        }
    }


    public delegate void OnIAPCallBack();
    
    private List<string> _ProductIds;

    private string _logStr;

    private OnIAPCallBack kOnIAPSuccess;
    private OnIAPCallBack kOnIAPFailed;

    private string m_Receipt;
    public string Receipt { get { return m_Receipt; } }
    private string m_buyStoreID;
    public string BuyStoreID { get { return m_buyStoreID; } }


    public bool IsBuying = false;
    void Awake()
    {
        if (instance == null)
        {
            instance = gameObject.GetComponent<IAPManager>();
        }
        else if (instance != this)
        {
            Debug.Log("A Singleton already exists. Destroy new one");
            Destroy(this.gameObject);
        }

        IsBuying = false;
    }

    public virtual void OnDestroy()
    {
        instance = null;
    }

    public void Init()
    {
        Log.Show("Init IAP Manager");
        DontDestroyOnLoad(this);

        _ProductIds = new List<string>();
        List<GameTable.Store.Param> storeParam = GameInfo.Instance.GameTable.FindAllStore(x => x.PurchaseType == (int)eREWARDTYPE.GOODS && x.PurchaseIndex == (int)eGOODSTYPE.GOODS);
        if (storeParam == null)
        {
            Debug.LogError("StoreParam is null");
            return;
        }
        for (int i = 0; i < storeParam.Count; i++)
        {
            if (_ProductIds.Contains(storeParam[i].ID.ToString()))
                continue;
            _ProductIds.Add(storeParam[i].ID.ToString());
        }

        InitStore();

    }

    void InitStore()
    {
        Debug.Log("IAPManager InitStore()");
    }

    public void BuyIAPProduct(string buyID, OnIAPCallBack successCallback, OnIAPCallBack failedCallback)
    {

        kOnIAPSuccess = successCallback;
        kOnIAPFailed = failedCallback;
        m_buyStoreID = buyID;
        if (!_ProductIds.Contains(buyID))
        {
            Debug.LogError("등록된 상품이 아닙니다.");
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3130));
            FailToUnLock();
            return;
        }

        successCallback?.Invoke();
    }

    /// <summary>
    /// 결제 영수증 저장 정보 초기화
    /// </summary>
    public void ResetReceipt()
    {
        m_Receipt = string.Empty;
    }

    /// <summary>
    /// AppStore, Google Play Store 계정이 꼬여서 결제모듈이 초기화 되지 않았을때
    /// </summary>
    /// <returns></returns>
    public bool IAPNULL_CHECK()
    {
        return true;
    }

    /// <summary>
    /// 결제 시도 실패 후 게임 Lock 해제
    /// </summary>
    public void FailToUnLock()
    {
        WaitPopup.Hide();
        IsBuying = false; 
    }
}
#endif