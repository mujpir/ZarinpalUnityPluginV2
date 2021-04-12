# ZarinpalUnityPluginV2

Setup ZarrinPal plugin in Unity

1. First, download the UnityZarinpal.unitypackage file from the mentioned path and enter it into your Unity project through the Assets / Import package menu.

2. After importing the Unity package if you are on Android, click the Menu button Assets/ExtrenalDependencymanager /Android Resolver/ForceResolve to download the required plugin files.
3. Then select the settings option from  Zarinpal/Setting menu and make the required settings for the plugin:

•	Enable: Determines whether the plugin is enabled. If enabled ,  the required files will be imported and required changes will be reflected in the AndroidManifest file.
•	MerchantID: The merchant code you received from ZarrinPal.
•	Auto Verify Purchase: Determines whether to verify payment immediately after successful purchase. If it is disabled, you need to call Zarinpal.VerifyPayment() method to check the validation of a payament.
•	Auto Start Purchase: Determines whether zarinpal start purchase flow after calling PaymentRequest, or not? If it is disabled, the Zarinpal.StartPay () method needs to be called to redirect the user to the payment gateway.
•	Scheme and Host: These two set the desired unique code that is needed to guide the purchase result to your app or game. Please note that the combination of the two must be unique, otherwise the result of the purchase may be redirected to another game or app on the user's device. It is recommended to specify the name of your app or game for the Scheme and use paymentresult for the Host (without the use of symbols) .
•	Use Scheme & Host as Callback: Determines whether to use a combination of Scheme and Host as Callback. CallbackUrl must be specified by you if it is disabled.
•	CallbackUrl: Specifies which Url the purchase result will be sent to after the purchase completed.
•	LogEnabled: Determines whether the purchase log is enabled.

After your desired changes, click the Update Files & manifest button to apply the required changes and activate the plugin.

API Usage :

As you may know, ZarrinPal offers an easy payment gateway, and currently does not offer the features available in other Android or iOS markets. As a result, a concept called Product has not yet been implemented for ZarrinPal.

How to make a purchase using ZarinPal is as follows:

1-	You send a Payment Request for ZarrinPal with parameters MerchantID, Amount ( In Iranian Rials), Description and CallbackUrl . Then  ZarrinPal will reply to you with a unique purchase ID (Authority).
2-	Then with the Authority in Hand , You call StartPay method and ZarrinPal in response will direct you to a payment gateway if the purchase ID is valid and after completing the purchase , Zarinpal send Purchase Result to CallbackUrl you specified in Step 1.
3-	After completing the purchase, you can send a VerifyPayment request if it is successful, and ZarrinPal will send you a RefID, which represents the transaction ID, in case of success.

ZarrinPal Unity Pplugin has implemented all these steps .  As it was said, there is no concept of product in ZarrinPal, but ZarrinPal Unity Plugin gives you an option to configure products right inside unity. Therfore , the only thing you need to star a purchase is calling Zarinpal. RequestPayment(string productID).
Also, instead of using CallbackUrl, the ZarrinPal plugin uses Scheme and Host to direct the purchase result to your game or app . As mentioned, the combination of the two must be unique.

Here is method you need to Initialize plugin in your code and do a purchase:

1-	The Zarinpal.Initialize() method must be called before using any method in ZarrinPal. It is recommended that this be called at the beginning of your game and only once. If the above method is successful, the StoreInitialized event will be called, otherwise the StoreInitializeFailed event will be called.
2-	After Zarinpal Initialized  , QuerySkuDetails method should be called to get the list of game products. If successful, the above method will call the OnQuerySkuDetailsSucceeded event, which has a list of active products in its parameter, otherwise the OnQuerySkuDetailsFailed event will be called.
3-	Once you have received the product list, you can make a purchase using the product , Call the Zarinpal.PaymentRequest ("product_id") method to begin your purchase. After calling the above method, two events are called : One is the OnPurchaseStarted event, which is called if the purchase initiation is successful, and the other is the OnPurchaseFailedToStart event, which is called if the purchase initiation is not successful.
4-	After calling the OnPurchaseStarted event, if the Auto Start Purchase option is enabled, the user will be automatically directed to the payment gateway, and if this option is not enabled, the Zarinpal.StartPay ("Authority") method must be called to direct use to payment gateway. Be guided. Disabling the Auto Start Purchase option is useful when, for example, you want to record the purchase status in your database for the user.
5-	After the purchase is completed by the user, if the purchase is successful, the OnPurchaseSucceed event will be called, otherwise the OnPurchaseFailed event will be called.
6-	If the Auto Verify purchase option is enabled in the plugin settings, the purchase will be checked automatically by the plugin, otherwise the Zarinpal.VerifyPayment("Authority", "Amount")  method must be called. If the purchase review is successful, the OnPaymentVerificationSucceed event will be called, otherwise the OnPaymentVerificationFailed event will be called.
For further reading please refer to ZarinpalExample.cs in your unity projects.

Advanced Setting:

As mentioned, ZarrinPal plugin for Unity allows you to configure your product list in Unity to make a purchase using  productID, but the disadvantage of this is that the product list and its details are on the client side. Therfore the possibility of updating products without uploading a new binary to market is impossible. The solution for this is that you can create a class from the IzarinpalQueryProvider interface and implement two methods, QuerySkuDetail and QueryPurchases . You can implement a new calss to retrieve products from your server. After implemention this class, you must call the Zarinpal.SetQueryProvider method before calling the Initialize method and send an instance of the IzarinpalQueryProvider class to it as a parameter.


You may also want the purchase result to be sent to your Web Service first, and after registering in the database, the purchase result will be directed to your game or app . To do this, you must disable the Use Scheme & Host As Callback Url option through the settings and Set your WebService Api address for CallbackUrl. Also, your WebService must send the result to your game through the same Scheme and Host after registering the purchase. For example, if your scheme is “awesomegame” and your host is “zarinpalpayment”, your WebService should be redirected user to this url after registering a purchase:
awesomegame://zarinpalpayment?Authority=<PAYMENT_ATHORITY?&Status=OK|NOK

