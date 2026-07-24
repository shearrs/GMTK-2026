using BigHappyBurger.Interaction;
using UnityEngine;

namespace BigHappyBurger.Foods
{
    [RequireComponent(typeof(Item))]
    public class Food : MonoBehaviour
    {
        private Item item;

        internal Item Item
        {
            get
            {
                if (item == null)
                    item = GetComponent<Item>();

                return item;
            }
        }

        public string ID => Item.ID;
        public bool IsCookable => true;
    }
}
