using System;
using UnityEngine;

namespace ZarinpalIAB
{
    public class ZarinpalAndroid : MonoBehaviour, IZarinpalPlatform
    {
        private AndroidJavaClass _zarinpalJavaClass;
        private AndroidJavaObject _zarinpalJavaObject;
        private static ZarinpalAndroid _instance;

        public static ZarinpalAndroid CreateInstance()
        {
            if (_instance == null)
            {
                _instance = new GameObject("ZarinpalAndroid").AddComponent<ZarinpalAndroid>();
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }

        public string MerchantID { get; private set; }
        public bool AutoVerifyPurchase { get; private set; }
        public string Callback { get; private set; }
        public bool IsInitialized { get; private set; }

        private bool m_purchaseOpen;

        private string m_productID;

        private string m_authority;

        private string m_refID;

        public Purchase PurchaseStatus
        {
            get
            {
                return new Purchase(m_authority, m_productID);
            }
        }

        public void Initialize(string merchantID, bool verifyPurchase, string schemeCallback,bool autoStartPurchase)
        {
            MerchantID = merchantID;
            AutoVerifyPurchase = verifyPurchase;
            Callback = schemeCallback;
            _zarinpalJavaClass = new AndroidJavaClass("com.kingcodestudio.unityzarinpaliab.ZarinpalActivity");
            _zarinpalJavaClass.CallStatic("initialize", merchantID, verifyPurchase, schemeCallback,autoStartPurchase);
        }

        public void PaymentRequest(long amount, string desc, string productID)
        {
            m_purchaseOpen = true;
            m_authority = null;
            m_productID = productID;
            m_refID = null;
            _zarinpalJavaClass.CallStatic("startPurchaseFlow", amount, productID, desc);
        }

        public void StartPay(string authority)
        {
            _zarinpalJavaClass.CallStatic("startPurchaseActivity");
        }

        public void VerifyPayment(string authority,int amount)
        {
            m_authority = authority;
            _zarinpalJavaClass.CallStatic("verifyPurchase", authority,amount);

        }

        public event Action StoreInitialized;
        public event Action<string,string> PurchaseStarted;
        public event PurchaseFailedToStartDelegate PurchaseFailedToStart;
        public event PurchaseSucceedDelegate PurchaseSucceed;
        public event PurchaseFailedDelegate PurchaseFailed;
        public event Action PurchaseCanceled;
        public event Action<string> PaymentVerificationStarted;
        public event Action<string> PaymentVerificationSucceed;
        public event Action<string> PaymentVerificationFailed;
        public void Dispose()
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);
                _instance = null;
            }
        }


        #region Callbacks

        private void OnStoreInitialized(string nullMessage)
        {
            IsInitialized = true;
            var handler = StoreInitialized;
            if (handler != null) handler();
        }

        private void OnPurchaseStarted(string authority)
        {
            m_authority = authority;
            var handler = PurchaseStarted;
            if (handler != null) handler(m_productID,authority);
        }

        private void OnPurchaseFailedToStart(string error)
        {
            var handler = PurchaseFailedToStart;
            if (handler != null) handler(error);
        }

        private void OnPurchaseSucceed(string authority)
        {
            m_purchaseOpen = false;
            m_authority = authority;
            var handler = PurchaseSucceed;
            var purchase = new Purchase(authority, m_productID);
            if (handler != null) handler(purchase);
        }

        private void OnPurchaseFailed(string error)
        {
            if (m_purchaseOpen)
            {
                m_purchaseOpen = false;
                var handler = PurchaseFailed;
                if (handler != null) handler(error);
            }
        }

        protected virtual void OnPurchaseCanceled()
        {
            if (m_purchaseOpen)
            {
                m_purchaseOpen = false;
                var handler = PurchaseCanceled;
                if (handler != null) handler();
            }
        }

        private void OnPaymentVerificationStarted(string url)
        {
            var handler = PaymentVerificationStarted;
            if (handler != null) handler(url);
        }

        private void OnPaymentVerificationSucceed(string refID)
        {
            m_refID = refID;
            m_purchaseOpen = false;
            var handler = PaymentVerificationSucceed;
            if (handler != null) handler(refID);
        }

        private void OnPaymentVerificationFailed(string error)
        {
            if (m_purchaseOpen)
            {
                m_purchaseOpen = false;
                var handler = PaymentVerificationFailed;
                if (handler != null) handler(error);
            }
        }

        #endregion
    }
}
