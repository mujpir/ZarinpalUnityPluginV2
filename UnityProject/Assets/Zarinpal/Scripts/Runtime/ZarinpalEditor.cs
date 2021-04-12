using System;

namespace ZarinpalIAB
{
    public class ZarinpalEditor : IZarinpalPlatform
    {
        public string MerchantID { get; private set; }
        public bool AutoVerifyPurchase { get; private set; }
        public string Callback { get; private set; }
        public bool IsInitialized { get; private set; }
        
        private string m_productID;

        private string m_authority;

        private string m_refID;

        public Purchase PurchaseStatus
        {
            get
            {
                return new Purchase(m_authority,m_productID);
            }
        }

        public void Initialize(string merchantID, bool verifyPurchase, string schemeCallback,bool autoStartPurchase)
        {
            Log("initializing zarinpal with merchant-id : {0} , autoVerify : {1} , callback : {2}", merchantID,
                verifyPurchase, schemeCallback);
            MerchantID = merchantID;
            AutoVerifyPurchase = verifyPurchase;
            Callback = schemeCallback;
            IsInitialized = true;
            OnStoreInitialized();
        }

        public void PaymentRequest(long amount, string desc, string productID)
        {
            m_authority = "fake_authority_00000000000000000" + Guid.NewGuid();
            OnPurchaseStarted(productID,m_authority);
            Log("purchasing amount of : {0} toman , desc : {1} , productID : {2}", amount, desc, productID);
            m_productID = productID;
            OnPurchaseSucceed(productID, m_authority);
            if (AutoVerifyPurchase)
            {
                VerifyPayment(productID,(int) amount);
            }
        }

        public void StartPay(string authority)
        {

        }

        public void VerifyPayment(string authority,int amount)
        {
            m_authority = authority;
            OnPaymentVerificationStarted(m_authority);
            var refID = "fake_ref_id_00000000000000000" + Guid.NewGuid();
            OnPaymentVerificationSucceed(refID);
        }

        private void Log(string log)
        {
            KKLog.Log(log);
        }

        private void Log(string log, params object[] args)
        {
            KKLog.Log(string.Format(log, args));
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
        }

        protected virtual void OnStoreInitialized()
        {
            var handler = StoreInitialized;
            if (handler != null) handler();
        }

        protected virtual void OnPurchaseStarted(string productCode,string authority)
        {
            m_authority = authority;
            var handler = PurchaseStarted;
            if (handler != null) handler(productCode,authority);
        }

        protected virtual void OnPurchaseFailedToStart(string error)
        {
            var handler = PurchaseFailedToStart;
            if (handler != null) handler(error);
        }

        protected virtual void OnPurchaseSucceed(string productID, string authority)
        {
            var handler = PurchaseSucceed;
            var purchase =
                new Purchase( authority, productID);
            if (handler != null) handler(purchase);
        }

        protected virtual void OnPurchaseFailed(string error)
        {
            var handler = PurchaseFailed;
            if (handler != null) handler(error);
        }

        protected virtual void OnPurchaseCanceled()
        {
            var handler = PurchaseCanceled;
            if (handler != null) handler();
        }

        protected void OnPaymentVerificationStarted(string obj)
        {
            var handler = PaymentVerificationStarted;
            if (handler != null) handler(obj);
        }

        protected void OnPaymentVerificationSucceed(string refID)
        {
            var handler = PaymentVerificationSucceed;
            if (handler != null) handler(refID);
        }

        protected void OnPaymentVerificationFailed(string error)
        {
            var handler = PaymentVerificationFailed;
            if (handler != null) handler(error);
        }
    }
}
