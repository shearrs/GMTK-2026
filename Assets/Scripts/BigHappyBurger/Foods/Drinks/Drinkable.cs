using BigHappyBurger.Interaction;
using Shears;
using UnityEngine;

namespace BigHappyBurger.Foods
{
    [RequireComponent(typeof(Food))]
    public class Drinkable : MonoBehaviour
    {
        public enum Size
        {
            Small,
            Medium,
            Large,
        }

        [field: SerializeField]
        public DrinkType DrinkType { get; private set; }

        [field: SerializeField]
        public Size DrinkSize { get; private set; }

        [SerializeField, Required, Local]
        private SpecificItemHolder lidHolder;

        public bool HasLiquid { get; private set; }
        public bool IsFull { get; private set; }
        public bool HasLid => lidHolder.HasItem;

        private void Reset()
        {
            var food = GetComponent<Food>();

            food.SetDrinkable(this);
        }

        public void Fill(DrinkTypeSize drink)
        {
            if (HasLiquid)
                return;

            HasLiquid = true;
            DrinkType = drink.Type;
            IsFull = drink.Size == DrinkSize;
        }
    }
}
