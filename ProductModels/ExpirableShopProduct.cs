using System;
using Helpers;

namespace Models.Shop
{
    public class ExpirableShopProduct : ShopProduct
    {
        private ExpireState expireState;

        private DateTime expireDate;
        
        private float remainingTime;

        public double CurrentTime { get; private set; }

        public ExpireState ExpireState => expireState;

        private bool shouldUpdate;

        //Eğer should update değiştiyse ve True olmuşsa kalan zamanı hesapla
        public override bool ShouldUpdate
        {
            get
            {
                return shouldUpdate;
            }
            set
            {
                shouldUpdate = value;
                if (shouldUpdate == true)
                {
                    CalculateRemainingTime();
                }
            }
        }

        public ExpirableShopProduct(string id, IPayment paymentMethod, ProductUpdateFrequency updateFrequency, string expireDateString) :
            base(id, paymentMethod, updateFrequency)
        {
            SetExpireDate(expireDateString);
        }

        public override void Update(float deltaTime)
        {
            CurrentTime -= deltaTime;

            if(CurrentTime < 0f)
            {
                // We want to notify UICard one more time to destroy it.
                this.updateFrequency = ProductUpdateFrequency.Notify;
                this.expireState = ExpireState.Expired;

                
                ShopManager.Instance.UpdatableManager.Remove(this);
            }

            base.Update(deltaTime);
        }

        public void CalculateRemainingTime()
        {
            var diffDateTime = expireDate.Subtract(DateTime.Now);

            if (diffDateTime.TotalDays > 1000)
            {
                expireState = ExpireState.Infinite;
                shouldUpdate = false;
            }
            else if (diffDateTime.TotalMilliseconds < 0)
            {
                expireState = ExpireState.Expired;
                shouldUpdate = false;
            }
            else
            {
                expireState = ExpireState.NonExpired;
                CurrentTime = diffDateTime.TotalSeconds;
                shouldUpdate = true;
            }
        }

        private void SetExpireDate(string expireDateString)
        {
            expireDate = Extensions.ParseDateWithCultureInfo(expireDateString);
            CalculateRemainingTime();
        }
    }
}