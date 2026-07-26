using System.Collections.Generic;
using BigHappyBurger.Foods;
using BigHappyBurger.Restaurants;
using Shears;
using UnityEngine;

namespace BigHappyBurger.GameManagement
{
    [CreateAssetMenu(menuName = "Big Happy Burger/Level Data")]
    public class LevelData : ScriptableObject
    {
        [field: SerializeField]
        public int CustomerCount { get; private set; }

        [field: SerializeField, Min(0.0f)]
        public float TimePerOrderMultiplier { get; private set; } = 1.0f;

        [field: SerializeField]
        public Range<float> CustomerArrivalRange { get; private set; } = new(10.0f, 20.0f);

        [SerializeField]
        private Range<int> foodCountRange = new(1, 2);

        [SerializeField]
        private Range<int> drinkCountRange = new(0, 1);

        [SerializeField]
        private Food[] possibleFoods;

        [SerializeField]
        private DrinkType[] possibleDrinks;

        [SerializeField]
        private Drinkable.Size[] possibleDrinkSizes;

        private readonly List<Food> chosenFoods = new();
        private readonly List<DrinkTypeSize> chosenDrinks = new();

        public IReadOnlyList<Food> PossibleFoods => possibleFoods;

        public Order GetRandomOrder()
        {
            chosenFoods.Clear();
            chosenDrinks.Clear();

            int foodCount = foodCountRange.Random();
            int drinkCount = drinkCountRange.Random();

            if (foodCount == 0 && drinkCount == 0)
                foodCount = 1;

            for (int i = 0; i < foodCount; i++)
            {
                chosenFoods.Add(possibleFoods.Random());
            }

            for (int i = 0; i < drinkCount; i++)
            {
                var type = possibleDrinks.Random();
                var size = possibleDrinkSizes.Random();

                chosenDrinks.Add(new(type, size));
            }

            return new(chosenFoods, chosenDrinks, TimePerOrderMultiplier);
        }
    }
}
