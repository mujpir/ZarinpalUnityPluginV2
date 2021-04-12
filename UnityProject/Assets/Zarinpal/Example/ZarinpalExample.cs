using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZarinpalIAB;

public class ZarinpalExample : MonoBehaviour
{
    //this is needed for verification purchases
    private string _purchaseAuthority;
    [SerializeField]
    private Text _text;
    public void Initialize()
    {
        Zarinpal.StoreInitialized += Zarinpal_StoreInitialized;
        Zarinpal.StoreInitializeFailed += Zarinpal_StoreInitializeFailed;
        Zarinpal.PurchaseSucceed += Zarinpal_PurchaseSucceed;
        Zarinpal.PurchaseFailed += Zarinpal_PurchaseFailed;
        Zarinpal.PurchaseFailedToStart += Zarinpal_PurchaseFailedToStart;
        Zarinpal.PaymentVerificationSucceed += Zarinpal_PaymentVerificationSucceed;
        Zarinpal.PaymentVerificationFailed += Zarinpal_PaymentVerificationFailed;
        Zarinpal.QuerySkuDetailsSucceeded += Zarinpal_QuerySkuDetailsSucceeded;
        Zarinpal.QuerySkuDetailsFailed += Zarinpal_QuerySkuDetailsFailed;

        //call SetQueryProvider in order to use your own products query provider , The default value is ZarinpalLocalQueryProvider that use
        //ZarinpalProductSetting ScriptableObject located in Resources folder to retrieve products
        //Zarinpal.SetQueryProvider(new YourOwnProductQueryProvider());

        Zarinpal.Initialize();
    }

    private void Zarinpal_StoreInitialized()
    {
        LogMessage("Store initialized.");
    }

    private void Zarinpal_StoreInitializeFailed(string error)
    {
        LogMessage("Store failed to initialize."+error);
    }

    private void Zarinpal_PurchaseSucceed(Purchase obj)
    {
        _purchaseAuthority = obj.Authority;
        LogMessage("Purchase Successful" + obj);
    }

    private void Zarinpal_PurchaseFailed(string error)
    {
        LogMessage("Purchase failed." + error);
    }

    private void Zarinpal_PurchaseFailedToStart(string error)
    {
        LogMessage("Purchase failed to start ." + error);
    }

    private void Zarinpal_PaymentVerificationSucceed(VerifyPayment verifyPayment)
    {
        LogMessage("Payment Verification Succeed ." + verifyPayment);
    }

    private void Zarinpal_PaymentVerificationFailed(string error)
    {
        LogMessage("Payment Verification Failed ." + error);
    }

    private void Zarinpal_QuerySkuDetailsSucceeded(List<SkuInfo> products)
    {
        LogMessage("Query Sku Details Succeeded . number of products " + products.Count);
    }

    private void Zarinpal_QuerySkuDetailsFailed(string error)
    {
        LogMessage("Query Sku Details Failed ." + error);
    }

    public void Purchase()
    {
        Zarinpal.PaymentRequest("test_product");
    }

    public void Verify()
    {
        if (!string.IsNullOrEmpty(_purchaseAuthority))
        {
            LogMessage("verifying purchase with authority : "+ _purchaseAuthority);
            Zarinpal.VerifyPayment("test_product", _purchaseAuthority);
        }
        else
        {
            LogError("failed to verify purchase because _purchaseAuthority is null");
        }
    }

    public void QuerySkuDetails()
    {
        //We use ZarinpalLocalQueryProvider , se we just send null to retrieve all products
        Zarinpal.QuerySkuDetails(null);
    }



    private void LogMessage(string message)
    {
        _text.text = message;
        Debug.Log(message);
    }

    private void LogError(string message)
    {
        _text.text = message;
        Debug.LogError(message);
    }
}
