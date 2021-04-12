
using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

namespace ZarinpalIAB
{
    [Serializable]
    public class SkuInfo
    {
        [SerializeField]
        private string _productId;
        [SerializeField]
        private string _title;
        [SerializeField]
        private string _price;
        private string _type;
        [SerializeField]
        private string _description;


        public string Title
        {
            get => _title;
            private set => _title = value;
        }

        public string Price
        {
            get => _price;
            private set => _price = value;
        }

        public string Type
        {
            get => _type;
            private set => _type = value;
        }

        public string Description
        {
            get => _description;
            private set => _description = value;
        }

        public string ProductId
        {
            get => _productId;
            private set => _productId = value;
        }

        public string PriceCurrencyCode { get; private set; }

        public static List<SkuInfo> fromJsonArray(JSONArray items)
        {
            var skuInfos = new List<SkuInfo>();

            foreach (JSONNode item in items.AsArray)
            {
                SkuInfo bSkuInfo = new SkuInfo();
                bSkuInfo.fromJson(item.AsObject);
                skuInfos.Add(bSkuInfo);
            }

            return skuInfos;
        }

        public SkuInfo() { }

        public void fromJson(JSONClass json)
        {
            Title = json["title"].Value;
            Price = json["price"].Value;
            Type = json["type"].Value;
            Description = json["description"].Value;
            ProductId = json["productId"].Value;
            PriceCurrencyCode = json["price_currency_code"].Value;
        }

        public override string ToString()
        {
            return string.Format("<BazaarSkuInfo> title: {0}, price: {1}, type: {2}, description: {3}, productId: {4}  , priceCurrencyCode : {5}",
                Title, Price, Type, Description, ProductId,PriceCurrencyCode);
        }

    }
}
