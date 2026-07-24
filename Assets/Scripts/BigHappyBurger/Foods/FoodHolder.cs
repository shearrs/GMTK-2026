using BigHappyBurger.Interaction;
using Shears;
using UnityEngine;

namespace BigHappyBurger.Foods
{
    [RequireComponent(typeof(ItemHolder))]
    public partial class FoodHolder : MonoBehaviour
    {
        [SerializeField]
        private bool holdsCookable = true;

        [Auto]
        private ItemHolder holder;

        public Item Item => holder.Item;

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
                else
                    return false;
            }
        }
    }
}
