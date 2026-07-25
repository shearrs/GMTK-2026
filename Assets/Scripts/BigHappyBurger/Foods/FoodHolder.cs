using System;
using BigHappyBurger.Interaction;
using Shears;
using UnityEngine;

namespace BigHappyBurger.Foods
{
    [RequireComponent(typeof(ItemHolder))]
    public partial class FoodHolder : MonoBehaviour
    {
        [SerializeField]
        private bool allowAnythingElse = true;

        [SerializeField]
        private bool holdsCookable = true;

        [SerializeField]
        private bool holdsDrinkable = false;

        private ItemHolder holder;

        [AutoEvent(nameof(ItemHolder.ItemChanged), nameof(OnItemChanged))]
        private ItemHolder Holder
        {
            get
            {
                if (holder == null)
                    holder = GetComponent<ItemHolder>();

                return holder;
            }
        }
        public Item Item => Holder.Item;

        public event Action<FoodHolder> ItemChanged;

        private void Awake()
        {
            Holder.CanBeHeldCallback = CanHoldItem;
        }

        public bool CanHold(Food food) => Holder.CanBeHeld(food.Item);

        public bool Hold(Food food)
        {
            return Holder.Hold(food.Item);
        }

        public bool Release()
        {
            return Holder.Release();
        }

        public void Lock() => Holder.Lock();

        public void Unlock() => Holder.Unlock();

        private bool CanHoldItem(Item item)
        {
            if (item.IsBeingDragged)
                return false;

            if (!item.TryGetComponent(out Food food))
                return false;
            else
            {
                if (food.IsCookable)
                    return holdsCookable;
                else if (food.IsDrinkable)
                    return holdsDrinkable;
                else
                    return allowAnythingElse;
            }
        }

        private void OnItemChanged(Item item)
        {
            ItemChanged?.Invoke(this);
        }
    }
}
