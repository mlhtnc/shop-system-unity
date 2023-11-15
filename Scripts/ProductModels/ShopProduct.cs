using System;
using System.Collections.Generic;
using Helpers.Updatable;

namespace Models.Shop
{
    public class ShopProduct : IUpdatable
    {
        public Action<ShopProduct> Updated;

        private IPayment paymentMethod;

        private List<PayoutBase> payouts;

        private string id;

        private string priceString;

        protected ProductUpdateFrequency updateFrequency;

        public List<PayoutBase> Payouts
        {
            get => payouts;
        }

        public string Id { get => id; }

        public string PriceString
        {
            get
            {
                if(string.IsNullOrEmpty(priceString))
                    return "$ --";

                return priceString;
            }
            set => priceString = value;
        }

        public IPayment PaymentMethod
        {
            get
            {
                return paymentMethod;
            }
        }

        public virtual bool ShouldUpdate { get; set; }

        public string Title;

        public string Description;

        public ShopProduct(string id, IPayment paymentMethod, ProductUpdateFrequency updateFrequency)
        {
            this.paymentMethod = paymentMethod;
            this.id = id;
            this.updateFrequency = updateFrequency;
            payouts = new List<PayoutBase>();

            var shop = ShopManager.Instance;
            
            if(this.updateFrequency != ProductUpdateFrequency.None)
            {
                shop.UpdatableManager.Add(this);
            }
        }
        public bool CanUse()
        {
            return paymentMethod != null && paymentMethod.CanAfford();
        }
        public void Use(bool firstTimeUsed)
        {
            paymentMethod?.Pay();
            // We need to copy this list before iterating because
            // some of the elements might be removed and it will cause bug
            var payoutArray = payouts.ToArray();

            for (int i = 0; i < payoutArray.Length; ++i)
            {
                payoutArray[i].Use(firstTimeUsed);
            }
        }

        public void AddPayout(PayoutBase payout)
        {
            payouts.Add(payout);
        }

        public void RemovePayout(PayoutBase payout)
        {
            payouts.Remove(payout);
        }

        public virtual void Update(float deltaTime)
        {
            if (ShouldUpdate == false)
            {
                UnityEngine.Debug.LogWarning("this should not be updated?? something is wrong?");
                return;
            }
            if(updateFrequency==ProductUpdateFrequency.None)
            {
                ShouldUpdate = false;
                UnityEngine.Debug.LogWarning("this should not be updated?? something is wrong?");
                return;
            }
            if(updateFrequency==ProductUpdateFrequency.Notify)
            {
                Updated?.Invoke(this);
                ShouldUpdate = false;
                return;
            }

            Updated?.Invoke(this);
        }
    }
}
