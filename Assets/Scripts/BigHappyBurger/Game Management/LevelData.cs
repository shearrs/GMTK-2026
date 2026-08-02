using System;
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
        [field: Header("Customers")]
        [field: SerializeField]
        public int CustomerCount { get; private set; }

        [field: SerializeField, Min(0.0f)]
        public float TimePerOrderMultiplier { get; private set; } = 1.0f;

        [field: SerializeField]
        public float FirstCustomerArrivalTime { get; private set; } = 10.0f;

        [field: SerializeField]
        public Range<float> CustomerArrivalRange { get; private set; } = new(10.0f, 20.0f);

        [field: Header("Food Count")]
        [SerializeField]
        private Range<int> foodCountRange = new(1, 2);

        [SerializeField]
        private Range<int> drinkCountRange = new(0, 1);

        [field: Header("Food Choices")]
        [SerializeField, Range(0, 1)]
        private float happyBoxChance = 0.15f;

        [SerializeField, Required]
        private Food[] happyBoxItems = Array.Empty<Food>();

        [SerializeField, Required]
        private Food fries;

        [SerializeField]
        private Food[] possibleFoods = Array.Empty<Food>();

        [SerializeField]
        private Food[] possibleCondiments = Array.Empty<Food>();

        [SerializeField]
        private DrinkType[] possibleDrinks = Array.Empty<DrinkType>();

        [SerializeField]
        private Drinkable.Size[] possibleDrinkSizes = Array.Empty<Drinkable.Size>();

        private readonly List<Food> chosenFoods = new();
        private readonly List<DrinkTypeSize> chosenDrinks = new();

        public Order GetRandomOrder(bool canBeHappyBox)
        {
            GetRandomFoods(canBeHappyBox, out bool isHappyBox);
            GetRandomDrinks();

            return new(chosenFoods, chosenDrinks, TimePerOrderMultiplier, isHappyBox);
        }

        private void GetRandomFoods(bool canBeHappyBox, out bool isHappyBox)
        {
            isHappyBox = false;
            chosenFoods.Clear();

            if (canBeHappyBox)
            {
                float happyBox = UnityEngine.Random.Range(0.0f, 1.0f);
                if (happyBoxChance >= happyBox)
                {
                    for (int i = 0; i < happyBoxItems.Length; i++)
                        chosenFoods.Add(happyBoxItems[i]);

                    isHappyBox = true;

                    return;
                }
            }

            int foodCount = foodCountRange.Random();

            for (int i = 0; i < foodCount; i++)
                chosenFoods.Add(possibleFoods.Random());

            if (chosenFoods.Contains(fries) && possibleCondiments.Length > 0)
            {
                bool hasCondiment = UnityEngine.Random.Range(0, 2) > 0;

                if (!hasCondiment)
                    return;

                chosenFoods.Add(possibleCondiments.Random());
            }
        }

        private void GetRandomDrinks()
        {
            chosenDrinks.Clear();
            int drinkCount = drinkCountRange.Random();

            if (chosenFoods.Count == 0 && drinkCount == 0 && possibleDrinks.Length > 0)
                drinkCount = 1;

            for (int i = 0; i < drinkCount; i++)
            {
                var type = possibleDrinks.Random();
                var size = possibleDrinkSizes.Random();

                chosenDrinks.Add(new(type, size));
            }
        }
    }
}
