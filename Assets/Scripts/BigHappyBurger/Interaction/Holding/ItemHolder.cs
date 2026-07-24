using System;
using UnityEngine;

namespace BigHappyBurger.Interaction
{
    public class ItemHolder : MonoBehaviour
    {
        [field: SerializeField]
        public bool IsLocked { get; set; }

        public Item Item { get; private set; }
        public Func<Item, bool> CanBeHeldCallback { get; set; }

        public event Action<Item> ItemChanged;

        public bool CanBeHeld(Item item)
        {
            return CanBeHeldCallback(item);
        }

        public bool Hold(Item item)
        {
            if (Item != null)
                return false;

            if (CanBeHeld(item))
            {
                return true;
            }
            else
                return false;
        }

        public void Release() { }
    }
}
