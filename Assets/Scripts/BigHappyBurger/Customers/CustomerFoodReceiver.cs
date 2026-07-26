using System;
using BigHappyBurger.Foods;
using BigHappyBurger.Interaction;
using Shears;
using Shears.Logging;
using UnityEngine;

namespace BigHappyBurger.Customers
{
    [RequireComponent(typeof(ItemHolder))]
    public partial class CustomerFoodReceiver : MonoBehaviour
    {
        [SerializeField]
        private Collider collider;

        [Auto]
        [AutoEvent(nameof(ItemHolder.ItemChanged), nameof(OnItemChanged))]
        private ItemHolder holder;

        public event Action<Drinkable> DrinkReceived;
        public event Action<Bag> BagReceived;

        private void Awake()
        {
            __AutoAwake();

            holder.CanBeHeldCallback = CanBeHeld;
        }

        public void Enable()
        {
            holder.Unlock();
            collider.enabled = true;
        }

        public void Disable()
        {
            holder.Lock();
            collider.enabled = false;
        }

        public void Clear()
        {
            if (holder.Item == null)
                return;

            var item = holder.Item;
            holder.Unlock();
            holder.Release();

            Destroy(item.gameObject);
        }

        private bool CanBeHeld(Item item)
        {
            if (item.TryGetComponent(out Bag _) || item.TryGetComponent(out Drinkable drink))
                return true;
            else
                return false;
        }

        private void OnItemChanged(Item item)
        {
            if (item.TryGetComponent(out Bag bag))
                BagReceived?.Invoke(bag);
            else if (item.TryGetComponent(out Drinkable drink))
                DrinkReceived?.Invoke(drink);
            else
                SHLogger.Log($"{nameof(CustomerFoodReceiver)} received an unaccepted item!");
        }
    }
}
