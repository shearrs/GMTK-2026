using Shears;
using UnityEngine;

namespace BigHappyBurger.Interaction
{
    [RequireComponent(typeof(ItemHolder))]
    public partial class SpecificItemHolder : MonoBehaviour
    {
        [SerializeField, Required(targetCollectionSize: 1), Local]
        private Item[] holdableItems;

        [SerializeField]
        private bool lockOnHold = false;

        [Auto]
        [AutoEvent(nameof(ItemHolder.ItemChanged), nameof(OnItemChanged))]
        private ItemHolder holder;

        public bool HasItem => holder.Item != null;

        private void Awake()
        {
            __AutoAwake();

            holder.CanBeHeldCallback = CanBeHeld;
        }

        private bool CanBeHeld(Item item)
        {
            foreach (var possibleItem in holdableItems)
            {
                if (item.ID == possibleItem.ID)
                    return true;
            }

            return false;
        }

        private void OnItemChanged(Item item)
        {
            if (item != null && lockOnHold)
            {
                holder.Lock();
                foreach (var collider in GetComponents<Collider>())
                    collider.enabled = false;
            }
        }
    }
}
