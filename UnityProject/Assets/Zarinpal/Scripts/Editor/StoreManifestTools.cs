/// Copyright (C) 2012-2014 KingKodeStudio Inc.
///
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///
///      http://www.apache.org/licenses/LICENSE-2.0
///
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Xml;
using System.Collections.Generic;

namespace ZarinpalIAB.Editor
{
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public class StoreManifestTools : IManifestTools
    {
#if UNITY_EDITOR
		static StoreManifestTools instance = new StoreManifestTools();
        private static IABConfig setting;

        static StoreManifestTools()
		{
			IABManifestTools.ManTools.Add(instance);
		}

		public void ClearManifest(){
			RemoveZarinpalBPFromManifest();
        }
		public void UpdateManifest() {
			HandleZarinpalBPManifest ();
        }

		public void HandleZarinpalBPManifest()
		{
			if (StoreSettings.Enable) {
				AddZarinpalBPToManifest();
			} else {
				RemoveZarinpalBPFromManifest();
			}
		}

		private void AddZarinpalBPToManifest()
		{
			//This is a tag that required to prevent error on android when targeting android 9.0 or above
			//if(PlayerSettings.Android.targetSdkVersion >= AndroidSdkVersions.AndroidApiLevel27)
			IABManifestTools.AppendApplicationElement("uses-library", "org.apache.http.legacy",
				new Dictionary<string, string>()
				{
					{"required", "false"}
				});
			IABManifestTools.SetPermission("android.permission.INTERNET");
			IABManifestTools.AddActivity("com.kingcodestudio.unityzarinpaliab.ZarinpalActivity",new Dictionary<string, string>()
			{
			    {"theme","@android:style/Theme.DeviceDefault.Light.Dialog.NoActionBar.MinWidth" }
			});
		    XmlElement activityElement = IABManifestTools.FindElementWithTagAndName("activity",
		        "com.kingcodestudio.unityzarinpaliab.ZarinpalActivity");
		    XmlElement intentElement = IABManifestTools.AppendElementIfMissing("intent-filter", null, null,false, activityElement);
		    IABManifestTools.AppendElementIfMissing("action", "android.intent.action.VIEW",
		        new Dictionary<string, string>(), false, intentElement);
		    IABManifestTools.AppendElementIfMissing("category", "android.intent.category.DEFAULT",
		        new Dictionary<string, string>(), false, intentElement);


		    IABManifestTools.AddActivity("com.kingcodestudio.unityzarinpaliab.ZarinpalResultActivity", new Dictionary<string, string>()
		    {
		        {"theme","@android:style/Theme.DeviceDefault.Light.Dialog.NoActionBar.MinWidth" }
		    });
            XmlElement activityResultElement = IABManifestTools.FindElementWithTagAndName("activity",
		        "com.kingcodestudio.unityzarinpaliab.ZarinpalResultActivity");
		    XmlElement intentResultElement = IABManifestTools.AppendElementIfMissing("intent-filter", null, null,false, activityResultElement);
		    IABManifestTools.AppendElementIfMissing("action", "android.intent.action.VIEW",
		        new Dictionary<string, string>(), false, intentResultElement);
		    IABManifestTools.AppendElementIfMissing("category", "android.intent.category.DEFAULT",
		        new Dictionary<string, string>(), false, intentResultElement);
		    IABManifestTools.AppendElementIfMissing("category", "android.intent.category.BROWSABLE",
		        new Dictionary<string, string>(), false, intentResultElement);
		    var scheme = StoreSettings.Scheme;
		    var host = StoreSettings.Host;
		    IABManifestTools.RemoveElement("data", null, intentResultElement);
		    IABManifestTools.AppendElementIfMissing("data", null,
		        new Dictionary<string, string>()
		        {
		            {"scheme",scheme },
		            {"host",host },
		        },false, intentResultElement);
        }
		
		
		private void RemoveZarinpalBPFromManifest(){
			// removing Iab Activity
			if (!StoreSettings.Enable)
			{
				IABManifestTools.RemoveApplicationElement("uses-library", "org.apache.http.legacy");
				IABManifestTools.RemoveActivity("com.kingcodestudio.unityzarinpaliab.ZarinpalActivity");
				IABManifestTools.RemoveActivity("com.kingcodestudio.unityzarinpaliab.ZarinpalResultActivity");
			}
		}

        public IABConfig StoreSettings
        {
            get
            {
                if(setting==null)
                    setting = AssetDatabase.LoadAssetAtPath<IABConfig>("Assets/Zarinpal/Resources/IABSetting.asset");
                return setting;
            }
        }
#endif
    }
}