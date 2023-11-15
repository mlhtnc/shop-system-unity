using Shop.IAP;

namespace Models.Shop
{
    public interface IProductBuilder
    {
        IPayoutFactory PayoutFactory{ set; }
        IPaymentFactory PaymentFactory { set; }
        ShopProduct BuildProduct(IProductData productData);
    }
}