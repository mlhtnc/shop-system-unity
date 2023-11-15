
namespace Models.Shop
{
    public interface IPayment
    {
        int Amount { get; set;}

        bool CanAfford();
        bool Pay();

    }
}