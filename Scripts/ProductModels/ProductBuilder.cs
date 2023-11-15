using Helpers;
using Shop.IAP;

namespace Models.Shop
{
    public class ProductBuilder : IProductBuilder,IService
    {
        public  IPayoutFactory PayoutFactory { set; private get; }
        public IPaymentFactory PaymentFactory { set; private get; }

        public ShopProduct BuildProduct(IProductData productData)
        {
            ShopProduct product;
            if(string.IsNullOrEmpty(productData.ExpireDate))
            {

                product = new ShopProduct(
                    productData.Id,
                    PaymentFactory.Create(productData.PaymentData),
                    productData.UpdateFrequency
                );
            }
            else
            {
                product = new ExpirableShopProduct(
                    productData.Id,
                    PaymentFactory.Create(productData.PaymentData),
                    productData.UpdateFrequency,
                    productData.ExpireDate
                );
            }

            for (int i = 0; i < productData.Payouts.Length; ++i)
            {
                var payoutData = productData.Payouts[i];
                var payout = PayoutFactory.CreatePayout(payoutData);
                product.AddPayout(payout);
            }

            return product;
        }
    }
}