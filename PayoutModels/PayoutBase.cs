
using Shop.IAP;

namespace Models.Shop
{
    public abstract class PayoutBase
    {
        private string type;

        private string data;

        public string Type { get => type; }
        
        public string Data { get => data; }

        public PayoutBase(PayoutData payoutData)
        {
            this.type = payoutData.Type;
            this.data = payoutData.Data;
        }

        public abstract void Use(bool firstTimeUsed);
    }
}