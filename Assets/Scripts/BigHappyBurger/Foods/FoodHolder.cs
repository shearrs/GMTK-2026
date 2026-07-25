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

        [Auto]
        [AutoEvent(nameof(ItemHolder.ItemChanged), nameof(OnItemChanged))]
        private ItemHolder holder;

        public Item Item => holder.Item;

        public event Action<FoodHolder> ItemChanged;

        private void Awake()
        {
            __AutoAwake();

            holder.CanBeHeldCallback = CanHoldItem;
        }

        public bool CanBeHeld(Food food) => holder.CanBeHeld(food.Item);

        public bool Hold(Food food)
        {
            return holder.Hold(food.Item);
        }

        public bool Release()
        {
            return holder.Release();
        }

        public void Lock() => holder.Lock();

        public void Unlock() => holder.Unlock();

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
