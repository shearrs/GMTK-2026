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

        private void Awake()
        {
            __AutoAwake();

            holder.CanBeHeldCallback = CanHoldItem;
        }

        private bool CanHoldItem(Item item)
        {
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
