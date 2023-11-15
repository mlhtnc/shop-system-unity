using Shop.IAP;

namespace Models.Shop
{
    public class NonStoreProductData : IProductData
    {
        public string Id { get; set; }
        public PayoutData[] Payouts { get; set; }
        public ProductUpdateFrequency UpdateFrequency { get; set; }
        public string ExpireDate { get; set; }
        public PaymentData PaymentData { get; set; }
    }
}