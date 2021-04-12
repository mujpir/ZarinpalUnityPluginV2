using UnityEngine;
using UnityEngine.UI;

namespace ZarinpalIAB.Example
{
    public class TestZarinpal : MonoBehaviour
    {
        [SerializeField] private Text m_text;
        private AndroidJavaClass zarinpalJavaClass;
        private AndroidJavaObject mZarinpalJavaObject;

        void Start()
        {
        }

        public void InitializePurchase()
        {
            zarinpalJavaClass = new AndroidJavaClass("com.kingcodestudio.unityzarinpaliab.ZarinpalUnityFragment");
            zarinpalJavaClass.CallStatic("start", gameObject.name);
            mZarinpalJavaObject = zarinpalJavaClass.GetStatic<AndroidJavaObject>("instance");



            m_text.text = "plugin initialized ";
            KKLog.Log("plugin initialized");
        }

        public void RequestPayment()
        {
            mZarinpalJavaObject.Call("purchase", 100L, "pardakht");
        }

        public void VerifyPurchase()
        {
            mZarinpalJavaObject.Call("verifyPurchase");
        }

        public void OnErrorOnPaymentRequest(string error)
        {
            m_text.text = "an error occured in requesting for payment : " + error;
            KKLog.Log("an error occured in requesting for payment : " + error);
        }

        public void OnPaymentProcessStarted()
        {
            m_text.text = "payment process started ";
            KKLog.Log("payment process started");
        }

        public void OnPaymentVerificationSucceed(string refID)
        {
            m_text.text = "payment verification succeed refid :" + refID;
            KKLog.Log("payment verification succeed refid :" + refID);
        }

        public void OnPaymentVerificationFailed()
        {
            m_text.text = "payment verification failed ";
            KKLog.Log("payment verification failed");
        }
    }
}
