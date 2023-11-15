using Shop.IAP;

namespace Models.Shop
{
    public interface IPayoutFactory 
    {
        PayoutBase CreatePayout(PayoutData payoutData);
    }
}