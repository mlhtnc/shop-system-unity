using Newtonsoft.Json.Linq;
using UnityEngine;
using Shop.IAP;

namespace Models.Shop
{
    public static class NonStoreProductDeserializer
    {
        public static NonStoreProductData[] Deserialize(string nonStoreProductCatalogPath)
        {
            TextAsset serializedJson = Resources.Load<TextAsset>(nonStoreProductCatalogPath);

            JArray ja = JArray.Parse(serializedJson.text);

            NonStoreProductData[] productDatas = new NonStoreProductData[ja.Count];

            int idxProduct = 0;
            for (int i = 0; i < ja.Count; i++)
            {
                var prodObj = ja[i];
                JArray payoutsArr = (JArray)prodObj["Payouts"];
                JObject paymentMethodObj = (JObject)prodObj["PaymentMethod"];


                string id = (string)prodObj["Id"];
                PayoutData[] payouts = new PayoutData[payoutsArr.Count];

                var paymentType = (string)paymentMethodObj["Type"];
                var paymentAmount = (int)paymentMethodObj["Amount"];

                int idxPayout = 0;
                foreach (var payoutObj in payoutsArr)
                {
                    PayoutData pData = new PayoutData();
                    pData.Type = (string)payoutObj["Type"];
                    pData.Data = (string)payoutObj["Data"];

                    payouts[idxPayout++] = pData;
                }

                var productData = new NonStoreProductData();
                productData.Id = id;
                productData.Payouts = payouts;
                productData.PaymentData = new PaymentData { Type = paymentType, Amount = paymentAmount };

                if (prodObj["UpdateFrequency"] != null)
                    productData.UpdateFrequency = ((ProductUpdateFrequency)((int)prodObj["UpdateFrequency"]));

                productDatas[idxProduct++] = productData;
            }

            return productDatas;
        }
    }
}