using BigHappyBurger.Interaction;
using UnityEngine;

namespace BigHappyBurger.Foods
{
    [RequireComponent(typeof(Item))]
    public class Food : MonoBehaviour
    {
        private Item item;

        private Item Item
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
