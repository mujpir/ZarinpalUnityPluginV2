using System;
namespace ZarinpalIAB.Editor
{
		public interface IManifestTools
    {
#if UNITY_EDITOR
			void UpdateManifest();
			void ClearManifest();
#endif
		}
}

