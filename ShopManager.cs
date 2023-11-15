using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Shop.IAP;
using Helpers.Updatable;

namespace Models.Shop
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ShopManager
    {
        #region Fields
            
        private static ShopManager instance;

        private const string NonStoreProductCatalogPath = "NonStoreProductCatalog";

        private Dictionary<string, ShopProduct> storeProductDict;

        private Dictionary<string, ShopProduct> nonStoreProductDict;

        private IProductBuilder productBuilder;
        
        private IPayoutFactory payoutFactory;

        private UpdatableManager updatableManager;
        
        private bool isInitialized;

        #endregion

        #region Props
            
        public ShopProduct[] StoreProductCatalog { get; private set; }

        public ShopProduct[] NonStoreProductCatalog { get; private set; }

        public UpdatableManager UpdatableManager { get => updatableManager; }

        public static ShopManager Instance {
            get
            {
                if (instance == null)
                {
                    instance = new ShopManager();
                }
                return instance;
            }
        }

        public bool IsInitialized => isInitialized;

        #endregion

        #region Methods

        public void Initialize(IPayoutFactory payoutFactory, IPaymentFactory paymentFactory)
        {
            if(paymentFactory == null || payoutFactory == null)
            {
                UnityEngine.Debug.LogError("Initializing shop is not successfull. You did not specify factories.");
                return;
            }
            productBuilder = new ProductBuilder();

            productBuilder.PayoutFactory = payoutFactory;
            productBuilder.PaymentFactory = paymentFactory;

            storeProductDict = new Dictionary<string, ShopProduct>();
            updatableManager = new UpdatableManager();

            ReadNonStoreProductCatalog();
            ReadStoreProductCatalog();

            Purchaser.ProductPurchased += OnStoreProductPurchased;

            isInitialized = true;
        }

        public ShopProduct GetStoreProduct(string id)
        {
            CheckIfInitialized();

            if (storeProductDict.ContainsKey(id) == false)
            {
                Debug.LogError("Shop::GetStoreProduct(): Couldn't find product " + id);
                return null;
            }

            return storeProductDict[id];
        }

        public ShopProduct GetNonStoreProduct(string id)
        {
            CheckIfInitialized();

            if (nonStoreProductDict.ContainsKey(id) == false)
            {
                Debug.LogError("Shop::GetNonStoreProduct(): Couldn't find product " + id);
                return null;
            }

            return nonStoreProductDict[id];
        }

        private void CheckIfInitialized()
        {
            if(isInitialized == false)
                throw new System.Exception("Shop manager is not initialized. Either you forgot or something went wrong.");
        }

        private void OnStoreProductPurchased(ProductData productData)
        {
            var product = GetStoreProduct(productData.Id);

            if(product == null)
            {
                Debug.LogWarning("Shop::OnProductPurchased(): " + productData.Id + " is null.");
                return;
            }
            //First time used. Either we bought it or restored
            product.Use(true);
        }

        private void ReadStoreProductCatalog()
        {
            var productDatas = Purchaser.GetProductsFromCatalog();
            StoreProductCatalog = new ShopProduct[productDatas.Length];

            for (int i = 0; i < productDatas.Length; ++i)
            {
                string productId = productDatas[i].Id;
                ShopProduct product = productBuilder.BuildProduct(productDatas[i]);
                StoreProductCatalog[i] = product;
                storeProductDict.Add(product.Id, product);
            }
        }

        private void ReadNonStoreProductCatalog()
        {
            var deserializedObject = NonStoreProductDeserializer.Deserialize(NonStoreProductCatalogPath);

            NonStoreProductCatalog = new ShopProduct[deserializedObject.Length];
            nonStoreProductDict = new Dictionary<string, ShopProduct>();

            for (int i = 0; i < deserializedObject.Length; ++i)
            {
                var p = productBuilder.BuildProduct(deserializedObject[i]);
                nonStoreProductDict.Add(
                    deserializedObject[i].Id,
                    p
                );
                NonStoreProductCatalog[i] = p;
            }

        }

        #endregion
    }
}



