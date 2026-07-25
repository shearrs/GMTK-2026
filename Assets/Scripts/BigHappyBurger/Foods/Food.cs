using BigHappyBurger.Interaction;
using Shears;
using UnityEngine;

namespace BigHappyBurger.Foods
{
    [RequireComponent(typeof(Item))]
    public class Food : MonoBehaviour
    {
        [SerializeField, RuntimeReadOnly]
        private Cookable cookable;

        [SerializeField, RuntimeReadOnly]
        private Drinkable drinkable;

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
        public bool IsCookable => cookable != null;
        public bool IsDrinkable => drinkable != null;

        internal void SetCookable(Cookable cookable)
        {
            this.cookable = cookable;
        }

        internal void SetDrinkable(Drinkable drinkable)
        {
            this.drinkable = drinkable;
        }
    }
}
