using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZarinpalIAB
{
    public class Zarinpal
    {
        private readonly object _lockObject = new object();

        private static IZarinpalQueryProvider m_queryProvider;
        
        private static IZarinpalPlatform _platform;

        private static List<SkuInfo> _products;

        private static string _cachedAuthority;
        
        public static IZarinpalPlatform BillingPlatform
        {
            get { return _platform; }
        }

        public event Action<List<Purchase>, List<SkuInfo>> QueryInventorySucceeded;
        public event Action<string> QueryInventoryFailed;
        public static event Action<List<SkuInfo>> QuerySkuDetailsSucceeded;
        public static event Action<string> QuerySkuDetailsFailed;
        public static event Action<List<Purchase>> QueryPurchasesSucceeded;
        public static event Action<string> QueryPurchasesFailed;
        public void Dispose()
        {
            if (_platform != null)
            {
                _platform.StoreInitialized -= OnStoreInitialized;
                _platform.PurchaseStarted -= OnPurchaseStarted;
                _platform.PurchaseFailedToStart -= OnPurchaseFailedToStart;
                _platform.PurchaseSucceed -= OnPurchaseSucceed;
                _platform.PurchaseFailed -= OnPurchaseFailed;
                _platform.PurchaseCanceled -= OnPurchaseCanceled;
                _platform.PaymentVerificationStarted -= OnPaymentVerificationStarted;
                _platform.PaymentVerificationSucceed -= OnPaymentVerificationSucceed;
                _platform.PaymentVerificationFailed -= OnPaymentVerificationFailed;

                _platform.Dispose();
                _platform = null;

            }
        }

        /// <summary>
        /// Occures when store is successfully initialized
        /// </summary>
        public static event Action StoreInitialized;

        /// <summary>
        /// Occures when store failed to initialized
        /// </summary>
        public static event Action<string> StoreInitializeFailed;

        /// <summary>
        /// Occures when zarinpal initiaate a purchase
        /// </summary>
        public static event Action<string,string> PurchaseStarted;

        /// <summary>
        /// Occures when zarinpal can not start a purchase flow.It may be caused by invalid merchant id or unavailabilty of zarinpal service.
        /// </summary>
        public static event Action<string> PurchaseFailedToStart;

        /// <summary>
        /// Occures when a purchase completed by user but still not verified.
        /// </summary>
        public static event Action<Purchase> PurchaseSucceed;

        /// <summary>
        /// Occures when a purchase failed.
        /// </summary>
        public static event Action<string> PurchaseFailed;

        /// <summary>
        /// Occures when zarinpal started to verify purchase
        /// </summary>
        public static event Action<string> PaymentVerificationStarted;

        /// <summary>
        /// Occures when payment verified by zarinpal and would be valid.You can award your user here
        /// </summary>
        public static event Action<VerifyPayment> PaymentVerificationSucceed;

        /// <summary>
        /// Occures when payment verified by zarinpal and would NOT be valid or verification failed at first.
        /// </summary>
        public static event Action<string> PaymentVerificationFailed;

        public static bool Initialized
        {
            get
            {
                if (_platform == null)
                {
                    return false;
                }

                return _platform.IsInitialized;
            }
        }


        public static Purchase PurchaseStatus
        {
            get
            {
                if (_platform == null)
                {
                    return null;
                }

                return _platform.PurchaseStatus;
            }
        }

        public static void SetQueryProvider(IZarinpalQueryProvider provider)
        {
            m_queryProvider = provider;
        }
        

        /// <summary>
        /// Initialize Zarinpal . Call this once is start up of your game.
        /// </summary>
        public static void Initialize()
        {
            if (m_queryProvider == null)
            {
                m_queryProvider = new ZarinpalLocalQueryProvider();
            }

            if (_platform != null)
            {
                if (Initialized)
                {
                    var message = "Zarinpal is already initialized.Please make sure you call 'Initialize' once.";
                    OnStoreInitializeFailed(message);
                    KKLog.LogWarning(message);
                }
                else
                {
                    var message =
                        "Platform has been created but not initialized . There may be an error. Please see logs for more details";
                    OnStoreInitializeFailed(message);
                    KKLog.LogError(message);
                }

                return;
            }

#if UNITY_EDITOR
            _platform = new ZarinpalEditor();
#elif UNITY_IOS
        _platform = ZarinpaliOS.CreateInstance();
#elif UNITY_ANDROID
        _platform = ZarinpalAndroid.CreateInstance();
#endif

            //Subscribing events
            _platform.StoreInitialized += OnStoreInitialized;
            _platform.PurchaseStarted += OnPurchaseStarted;
            _platform.PurchaseFailedToStart += OnPurchaseFailedToStart;
            _platform.PurchaseSucceed += OnPurchaseSucceed;
            _platform.PurchaseFailed += OnPurchaseFailed;
            _platform.PurchaseCanceled += OnPurchaseCanceled;
            _platform.PaymentVerificationStarted += OnPaymentVerificationStarted;
            _platform.PaymentVerificationSucceed += OnPaymentVerificationSucceed;
            _platform.PaymentVerificationFailed += OnPaymentVerificationFailed;


            if (Initialized)
            {
                var message = "Zarinpal is already initialized.Please make sure you call 'Initialize' once.";
                OnStoreInitializeFailed(message);
                KKLog.LogWarning(message);
                return;
            }

            var setting = Resources.Load<IABConfig>("IABSetting");

            if (setting == null)
            {
                var message =
                    "Could not find zarinpal config file.Make sure you have setup zarinpal setting in KingIAB/Setting";
                OnStoreInitializeFailed(message);
                KKLog.LogWarning(message);
                return;
            }

            if (string.IsNullOrEmpty(setting.MerchantID) || setting.MerchantID == "MY_ZARINPAL_MERCHANT_ID")
            {
                var message = "Invalid MerchantID.Please go to menu : KingIAB/Setting to set a valid merchant id";
                OnStoreInitializeFailed(message);
                KKLog.LogWarning(message);
                return;
            }

            var scheme = setting.Scheme;
            var host = setting.Host;

#if !UNITY_EDITOR
        if (string.IsNullOrEmpty(setting.Scheme) || string.IsNullOrEmpty(setting.Host)
            || setting.Scheme=="MY_SCHEME" || setting.Host=="MY_HOST")
        {
            var message =
 "Scheme or Host Can not be null or Empty.Please go to menu : Zarinpal/Setting to set a valid Scheme and Host";
            OnStoreInitializeFailed(message);
            KKLog.LogWarning(message);
            return;
        }
#else
            scheme = string.Empty;
            host = string.Empty;
#endif

            if (_platform == null)
            {
                var message = "Platform is not supported";
                OnStoreInitializeFailed(message);
                KKLog.LogError(message);
                return;
            }

            _platform.Initialize(setting.MerchantID, setting.AutoVerifyPurchase,setting.CallbackUrl,setting.AutoStartPurchase);
        }




        public bool IsInitialized
        {
            get
            {
                if (_platform == null)
                {
                    return false;
                }
                return _platform.IsInitialized;
            }
        }

        public IZarinpalPlatform Platform
        {
            get { return _platform; }
        }

        public static void QuerySkuDetails(string[] products)
        {
            if (m_queryProvider!= null)
            {
                m_queryProvider.QuerySkuDetail(products, OnQuerySkuDetailsSucceeded, OnQuerySkuDetailsFailed);
            }
            else
            {
                throw new NullReferenceException(
                    "QueryPurchaseProvider is null, Please set it using Zarinpal.SetQueryProvider before initializing");
            }
        }

        public static void QueryPurchases()
        {
            if (m_queryProvider != null)
            {
                m_queryProvider.QueryPurchases(OnQueryPurchasesSucceeded, OnQueryPurchasesFailed);
            }
            else
            {
                throw new NullReferenceException(
                    "QueryPurchaseProvider is null, Please set it using Zarinpal.SetQueryProvider before initializing");
            }
        }

        public static void PaymentRequest(string productID)
        {
            if (_products != null)
            {
                var product = _products.FirstOrDefault(p => p.ProductId == productID);
                if (product != null)
                {
                    int price;
                    if (int.TryParse(product.Price, out price))
                    {
                        PaymentRequest(price, product.Description, productID);
                    }
                    else
                    {
                        OnPurchaseFailed("unable to parse product price , purchase failed.");
                    }
                }
                else
                {
                    OnPurchaseFailed("Product '" + productID + "' not found in inventory . purchase failed");
                }
            }
            else
            {
                OnPurchaseFailed("There is no product available to purchase");
            }
        }


        /// <summary>
        /// Start a zarinpal purchase
        /// </summary>
        /// <param name="amount">your product/service price in toman</param>
        /// <param name="desc">payment description.please note it can not be null or empty</param>
        /// <param name="productID">the id of product you are purchasing</param>
        private static void PaymentRequest(long amount, string desc, string productID = "na")
        {
            if (amount < 1000)
            {
                var message = "Purchase is not valid.Amount can not be less than 1000 Rial.";
                OnPurchaseFailedToStart(message);
                KKLog.LogError(message);
                return;
            }

            if (string.IsNullOrEmpty(desc))
            {
                var message =
                    "Purchase is not valid.Description can not be null or empty .Please provide a valid description";
                OnPurchaseFailedToStart(message);
                KKLog.LogError(message);
                return;
            }

            if (_platform == null || !_platform.IsInitialized)
            {
                var message =
                    "Purchase is not valid.Platform is not supported or is not initialized yet.Please Call initialize first";
                OnPurchaseFailedToStart(message);
                KKLog.LogError(message);
                return;
            }

            if (string.IsNullOrEmpty(productID))
            {
                productID = "unknown product";
            }

            _platform.PaymentRequest(amount, desc, productID);
        }

        public static void StartPay(string authority)
        {
            if (string.IsNullOrEmpty(authority))
            {
                throw new ArgumentNullException(nameof(authority), "Authority can not be null or empty");
            }
            else
            {
                _platform.StartPay(authority);
            }
        }


        /// <summary>
        /// Provider authority to verify purchase
        /// </summary>
        /// <param name="authority"></param>
        public static void VerifyPayment(string productID,string authority)
        {
            if (_products != null)
            {
                var product = _products.FirstOrDefault(p => p.ProductId == productID);
                if (product != null)
                {
                    int price;
                    if (int.TryParse(product.Price, out price))
                    {
                        _platform.VerifyPayment(authority, price);
                    }
                    else
                    {
                        OnPaymentVerificationFailed("unable to parse product price , verification failed.");
                    }
                }
                else
                {
                    OnPaymentVerificationFailed("Product '" + productID + "' not found in inventory . verification failed");
                }
            }
            else
            {
                OnPaymentVerificationFailed("There is no product available to verify");
            }
        }


        #region Callbacks

        protected static void OnStoreInitialized()
        {
            var handler = StoreInitialized;
            if (handler != null) handler();
        }

        private static void OnStoreInitializeFailed(string error)
        {
            var handler = StoreInitializeFailed;
            if (handler != null) handler(error);
        }

        protected static void OnPurchaseStarted(string productCode,string authority)
        {
            _cachedAuthority = authority;
            var handler = PurchaseStarted;
            if (handler != null) handler(productCode,authority);
        }

        protected static void OnPurchaseFailedToStart(string message)
        {
            var handler = PurchaseFailedToStart;
            if (handler != null) handler(message);
        }

        protected static void OnPurchaseSucceed(Purchase purchase)
        {
            _cachedAuthority = null;
            var handler = PurchaseSucceed;
            if (handler != null) handler(purchase);
        }

        protected static void OnPurchaseFailed(string error)
        {
            _cachedAuthority = null;
            var handler = PurchaseFailed;
            if (handler != null) handler(error);
        }

        protected static void OnPurchaseCanceled()
        {
            var handler = PurchaseFailed;
            if (handler != null) handler("User canceled the purchase.");
        }

        protected static void OnPaymentVerificationStarted(string obj)
        {
            var handler = PaymentVerificationStarted;
            if (handler != null) handler(obj);
        }

        protected static void OnPaymentVerificationSucceed(string refID)
        {
            var purchase = new VerifyPayment(refID, PurchaseStatus.ProductID);
            var handler = PaymentVerificationSucceed;
            if (handler != null) handler(purchase);
        }

        protected static void OnPaymentVerificationFailed(string error)
        {
            var handler = PaymentVerificationFailed;
            if (handler != null) handler(error);
        }

        protected static void OnQuerySkuDetailsSucceeded(List<SkuInfo> skuinfos)
        {
            _products = skuinfos;
            var handler = QuerySkuDetailsSucceeded;
            if (handler != null) handler(skuinfos);
        }

        protected static void OnQuerySkuDetailsFailed(string error)
        {
            var handler = QuerySkuDetailsFailed;
            if (handler != null) handler(error);
        }

        protected static void OnQueryPurchasesSucceeded(List<Purchase> purchases)
        {
            var handler = QueryPurchasesSucceeded;
            if (handler != null) handler(purchases);
        }

        protected static void OnQueryPurchasesFailed(string error)
        {
            var handler = QueryPurchasesFailed;
            if (handler != null) handler(error);
        }

        #endregion
    }
}
