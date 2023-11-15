namespace Models.Shop
{
    public interface IPaymentFactory
    {
        IPayment Create(PaymentData data);
    }
}