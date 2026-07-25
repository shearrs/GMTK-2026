using BigHappyBurger.Interaction;
using UnityEngine;

namespace BigHappyBurger.Foods
{
    [RequireComponent(typeof(Item))]
    public class Food : MonoBehaviour
    {
        [SerializeField]
        private Cookable cookable;

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

        internal void SetCookable(Cookable cookable)
        {
            this.cookable = cookable;
        }
    }
}
