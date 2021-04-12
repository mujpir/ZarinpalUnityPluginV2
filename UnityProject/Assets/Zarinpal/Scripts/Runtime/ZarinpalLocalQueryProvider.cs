using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZarinpalIAB
{
    public class ZarinpalLocalQueryProvider : IZarinpalQueryProvider
    {
        public void QueryPurchases(Action<List<Purchase>> succeedAction, Action<string> failedAction)
        {
            if (succeedAction != null)
            {
                succeedAction.Invoke(new List<Purchase>());
            }
        }

        public void QuerySkuDetail(string[] skus, Action<List<SkuInfo>> succeedAction, Action<string> failedAction)
        {
            var productsSetting = Resources.Load<ZarinpalProductSetting>("ZarinpalProductSetting");
            if (succeedAction != null)
            {
                succeedAction.Invoke(productsSetting.ProductsDescriptions);
            }
        }
    }
}

