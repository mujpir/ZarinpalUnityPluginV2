using UnityEngine;

namespace ZarinpalIAB
{
    [CreateAssetMenu]
    public class IABConfig : ScriptableObject
    {
        [SerializeField] private bool m_enable;
        
        //ZarinpalSetting
        [SerializeField] private string _merchantID;
        [SerializeField] private bool _autoStartPurchase = true;
        [SerializeField] private bool _autoVerifyPurchase = true;
        [SerializeField] private string _scheme = "return";
        [SerializeField] private string _host = "zarinpalpayment";
        [SerializeField] private string _calbackUrl;
        [SerializeField] private bool _useSchemeAndHostAsCallbackUrl;
        [SerializeField] private bool _logEnabled = true;

        public bool Enable
        {
            get { return m_enable; }
        }

        public string MerchantID
        {
            get { return _merchantID; }
        }

        public bool AutoVerifyPurchase
        {
            get { return _autoVerifyPurchase; }
        }

        public string Scheme
        {
            get { return _scheme; }
        }

        public string Host
        {
            get { return _host; }
        }

        public bool LogEnabled
        {
            get { return _logEnabled; }
        }

        public string CallbackUrl
        {
            get { return _calbackUrl; }
        }

        public bool AutoStartPurchase
        {
            get { return _autoStartPurchase; }
        }


    }
}

