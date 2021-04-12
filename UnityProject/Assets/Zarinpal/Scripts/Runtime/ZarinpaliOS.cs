using System;
using System.Runtime.InteropServices;
using UnityEngine;
using ZarinpalIAB;

namespace ZarinpalIAB
{
#if UNITY_IOS
    public class ZarinpaliOS : MonoBehaviour, IZarinpalPlatform
	{

		private static ZarinpaliOS _instance;
		
		private bool m_purchaseOpen;

		private string m_productID;

		private string m_authority;

		private string m_refID;

		public static ZarinpaliOS CreateInstance()
		{
			if (_instance == null)
			{
				if (_instance == null)
				{
					_instance = new GameObject("ZarinpaliOS").AddComponent<ZarinpaliOS>();
					DontDestroyOnLoad(_instance.gameObject);
				}
			}

			return _instance;
		}

		public string MerchantID { get; private set; }

		/// <summary>
		/// AutoVerifyPurchase is always true on iOS
		/// </summary>

		public bool AutoVerifyPurchase { get; private set; }

		/// <summary>
		/// Callback is not necessary for iOS
		/// </summary>
		public string Callback
		{
			get { return null; }
		}

		public bool IsInitialized { get; private set; }

		public Purchase PurchaseStatus
		{
			get { return new Purchase( m_authority, m_productID); }
		}

		public void Initialize(string merchantID, bool verifyPurchase, string callbackScheme,bool autoStartPurchase)
		{
			MerchantID = merchantID;
			AutoVerifyPurchase = verifyPurchase;
			_zu_initialize(merchantID,callbackScheme,verifyPurchase,autoStartPurchase);
		}

		public void PaymentRequest(long amount, string desc,string productID)
		{
			m_productID = productID;
			Debug.Log("ZarinpalIOS : Purchasing productID : " + m_productID);
			m_authority = null;
			m_refID = null;
			m_purchaseOpen = true;
			_zu_startPurchase((int) amount, productID, desc);
		}

		public void StartPay(string authority)
		{
			_zu_openPaymentGateway(authority);
		}

		public void VerifyPayment(string authority,int amount)
		{
			Debug.Log("consuming product inside ZarinpaliOS : "+amount);
			_zu_verifyPurchase(authority, amount);
		}

		public event Action StoreInitialized;

		public event Action<string,string> PurchaseStarted;
		public event PurchaseFailedToStartDelegate PurchaseFailedToStart;

#pragma warning disable 0067
		/// <summary>
		/// Not supported in iOS . use PaymentVerificationSucceed event instead.
		/// </summary>
		public event PurchaseSucceedDelegate PurchaseSucceed;

		public event PurchaseFailedDelegate PurchaseFailed;

		/// <summary>
		/// Not supported in iOS . use PurchaseFailed event instead.
		/// </summary>
		public event Action PurchaseCanceled;

		/// <summary>
		/// Not supported in iOS .
		/// </summary>
		public event Action<string> PaymentVerificationStarted;

		public event Action<string> PaymentVerificationSucceed;

		/// <summary>
		/// Not supported in iOS .
		/// </summary>
		public event Action<string> PaymentVerificationFailed;

#pragma warning restore 0067


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

		private void OnPurchaseSucceeded(string authority)
		{
			m_purchaseOpen = false;
			m_authority = authority;
			var handler = PurchaseSucceed;
			Debug.Log("ZarinpalIOS : OnPurchaseSucceeded : " + m_productID);
            Purchase purchase = new Purchase( authority, m_productID);
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
			/*
		    if (_transactionID.HasValue)
		    {
		        _transactionID = null;
		        var handler = PurchaseCanceled;
		        if (handler != null) handler();
		    }
		    */
		}

		private void OnPaymentVerificationStarted(string url)
		{
			var handler = PaymentVerificationStarted;
			if (handler != null) handler(url);
		}

		private void OnPaymentVerificationSucceed(string jsonValue)
		{
			try
			{
				var verifyObj = JsonUtility.FromJson<VerifyObject>(jsonValue);
				//var verifyObj = JsonUtility.FromJson<VerifyObject>(jsonValue);

				m_authority = verifyObj.authority;
				m_refID = verifyObj.refid;

				Debug.Log("authority : " + m_authority + "   ,   refid  :  " + m_refID);
				m_purchaseOpen = false;
				var handler = PaymentVerificationSucceed;
				if (handler != null) handler(m_refID);
			}
			catch (Exception e)
			{
				OnPaymentVerificationFailed("malfunctioned json");
			}


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
		
		public class VerifyObject
		{
			public string refid;
			public string authority;
		}
		
		public void Dispose(){
		}

    #endregion



    #region C-Extern

		[DllImport("__Internal")]
		private static extern void _zu_initialize(string merchantID,string callbackUrl , bool autoVerifyPurchase,bool autoStartPurchase);

		[DllImport("__Internal")]
		private static extern void _zu_startPurchase(int amount, string productID, string desc);

		[DllImport("__Internal")]
		private static extern void _zu_openPaymentGateway(string authority);
		
		[DllImport("__Internal")]
		private static extern void _zu_verifyPurchase(string authority,int amount);

    #endregion
	}
#endif
}
