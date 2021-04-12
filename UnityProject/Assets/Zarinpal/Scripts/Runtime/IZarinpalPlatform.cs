using System;
using ZarinpalIAB;

namespace ZarinpalIAB
{
    public interface IZarinpalPlatform
    {
        string MerchantID { get; }
        bool AutoVerifyPurchase { get; }

        string Callback { get; }
        bool IsInitialized { get; }

        Purchase PurchaseStatus { get; }

        void Initialize(string merchantID, bool verifyPurchase, string callbackScheme,bool autoStartPurchase);

        void PaymentRequest(long amount, string desc, string productID);

        void StartPay(string authority);
        
        void VerifyPayment(string authority,int amount);


        event Action StoreInitialized;

        event Action<string,string> PurchaseStarted;

        event PurchaseFailedToStartDelegate PurchaseFailedToStart;

        event PurchaseSucceedDelegate PurchaseSucceed;

        event PurchaseFailedDelegate PurchaseFailed;

        event Action PurchaseCanceled;

        event Action<string> PaymentVerificationStarted;

        event Action<string> PaymentVerificationSucceed;

        event Action<string> PaymentVerificationFailed;
        void Dispose();
    }
}
