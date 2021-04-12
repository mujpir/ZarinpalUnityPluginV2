# ZarinpalUnityPluginV2

<h4>Setup ZarinPal plugin in Unity</h4>
<ol type = "1">
<li>First,download the latest <b>Zarinpal unitypackage</b> file from the release section and import it into your Unity project through the Assets/Import package menu.</li>
<li>After importing the Unity package if you are on Android, click the Menu button Assets/ExtrenalDependencymanager/Android Resolver/ForceResolve to download the required plugin files.</li>
<li>Then select the settings option from  Zarinpal/Setting menu and make the required settings for the plugin:</li>
</ol>

<li><b>Enable:</b> Determines whether the plugin is enabled. If enabled ,  the required files will be imported and required changes will be reflected in the AndroidManifest file.</li>
<li><b>MerchantID:</b> The merchant code you received from ZarinPal.</li>
<li><b>Auto Verify Purchase:</b> Determines whether to verify payment immediately after successful purchase. If it is disabled, you need to call Zarinpal.VerifyPayment() method to check the validation of a payament.</li>
<li><b>Auto Start Purchase:</b> Determines whether zarinpal start purchase flow after calling PaymentRequest, or not? If it is disabled, the Zarinpal.StartPay () method needs to be called to redirect the user to the payment gateway.</li>
<li><b>Scheme and Host:</b> These two set the desired unique code that is needed to guide the purchase result to your app or game. Please note that the combination of the two must be unique, otherwise the result of the purchase may be redirected to another game or app on the user's device. It is recommended to specify the name of your app or game for the Scheme and use paymentresult for the Host (without the use of symbols) .</li>
<li><b>Use Scheme & Host as Callback:</b> Determines whether to use a combination of Scheme and Host as Callback. CallbackUrl must be specified by you if it is disabled.</li>
<li><b>CallbackUrl:</b> Specifies which Url the purchase result will be sent to after the purchase completed.</li>
<li><b>LogEnabled:</b> Determines whether the purchase log is enabled.</li>

After your made desired changes, click the <b>Update Files & manifest</b> button to apply the required changes and activate the plugin.

<h4>API Usage :</h4>

As you may know, ZarinPal offers an easy payment gateway, and currently does not offer the features available in other Android or iOS markets. As a result, a concept called <b>Product</b> has not yet been implemented for ZarinPal.

How to make a purchase using ZarinPal is as follows:

<ol>
  <li>You send a <code>PaymentRequest</code> for ZarinPal with parameters <b>MerchantID</b>, <b>Amount</b> ( In Iranian Rials), <b>Description</b> and <b>CallbackUrl</b> . Then ZarinPal will reply to you with a unique purchase ID <b>(Authority)</b>.</li>
  <li>Then with the Authority in Hand , You call <code>StartPay</code> method and ZarinPal in response will direct you to a payment gateway if the purchase ID is valid and after completing the purchase , Zarinpal send Purchase Result to <b>CallbackUrl</b> you specified in Step 1.</li>
  <li>After completing the purchase, you can send a <code>VerifyPayment</code> request if it is successful, and ZarinPal will send you a <b>RefID</b>, which represents the transaction ID, in case of success.</li>
</ol>

ZarinPal Unity Plugin has implemented all these steps .  As it was said, there is no concept of product in ZarinPal, but ZarinPal Unity Plugin gives you an option to configure products right inside unity. Therfore , the only thing you need to star a purchase is calling <code>Zarinpal.RequestPayment(string productID)</code>.
Also, instead of using CallbackUrl, the ZarinPal plugin uses Scheme and Host to direct the purchase result to your game or app . As mentioned, the combination of the two must be unique.

Here is method you need to Initialize plugin in your code and do a purchase:

<ol>
  <li>The <code>Zarinpal.Initialize()</code> method must be called before using any method in ZarinPal. It is recommended that this be called at the beginning of your game and only once. If the above method is successful, the <code>StoreInitialized</code> event will be called, otherwise the <code>StoreInitializeFailed</code> event will be called.</li>
  <li>After Zarinpal Initialized  , <code>QuerySkuDetails</code> method should be called to get the list of game products. If successful, the above method will call the <code>OnQuerySkuDetailsSucceeded</code> event, which has a list of active products in its parameter, otherwise the <code>OnQuerySkuDetailsFailed</code> event will be called.</li>
<li>Once you have received the product list, you can make a purchase using the product , Call the <code>Zarinpal.PaymentRequest("product_id")</code> method to begin your purchase. After calling the above method, two events are called : One is the <code>OnPurchaseStarted</code> event, which is called if the purchase initiation is successful, and the other is the <code>OnPurchaseFailedToStart</code> event, which is called if the purchase initiation is not successful.</li>
  <li>After calling the <code>OnPurchaseStarted</code> event, if the Auto Start Purchase option is enabled, the user will be automatically directed to the payment gateway, and if this option is not enabled, the <code>Zarinpal.StartPay("Authority")</code> method must be called to direct use to payment gateway. Be guided. Disabling the Auto Start Purchase option is useful when, for example, you want to record the purchase status in your database for the user.</li>
<li>After the purchase is completed by the user, if the purchase is successful, the <code>OnPurchaseSucceed</code> event will be called, otherwise the <code>OnPurchaseFailed</code> event will be called.</li>
<li>If the Auto Verify purchase option is enabled in the plugin settings, the purchase will be checked automatically by the plugin, otherwise the <code>Zarinpal.VerifyPayment("Authority", "Amount")</code> method must be called. If the purchase review is successful, the <code>OnPaymentVerificationSucceed</code> event will be called, otherwise the <code>OnPaymentVerificationFailed</code> event will be called.</li>
</ol>
For further reading please refer to <b>ZarinpalExample.cs</b> in your unity projects.

<h4>Advanced Setting:</h4>

As mentioned, ZarinPal plugin for Unity allows you to configure your product list in Unity to make a purchase using  productID, but the disadvantage of this is that the product list and its details are on the client side. Therfore the possibility of updating products without uploading a new binary to market is impossible. The solution for this is that you can create a class from the <code>IzarinpalQueryProvider</code> interface and implement two methods, <code>QuerySkuDetail</code> and <code>QueryPurchases</code> . You can implement a new calss to retrieve products from your server. After implemention this class, you must call the <code>Zarinpal.SetQueryProvider</code> method before calling the Initialize method and send an instance of the <code>IzarinpalQueryProvider</code> class to it as a parameter.


You may also want the purchase result to be sent to your Web Service first, and after registering it in the database, the purchase result will be directed to your game or app . To do this, you must disable the <b>Use Scheme & Host As Callback Url</b> option through the settings and Set your WebService Api address for CallbackUrl. Also, your WebService must send the result to your game through the same Scheme and Host after registering the purchase. For example, if your scheme is “awesomegame” and your host is “zarinpalpayment”, your WebService should be redirected user to this url after registering a purchase:
<code>awesomegame://zarinpalpayment?Authority=<PAYMENT_ATHORITY?&Status=OK|NOK</code>

