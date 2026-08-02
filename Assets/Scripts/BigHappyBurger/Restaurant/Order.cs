using System.Collections.Generic;
using BigHappyBurger.Foods;
using Shears;
using UnityEngine;

namespace BigHappyBurger.Restaurants
{
    [System.Serializable]
    public class Order
    {
        private const float BASE_TIME = 10.0f;
        private const float DRINK_TIME = 15.0f;

        [SerializeField]
        private List<Food> foods = new();

        [SerializeField]
        private List<DrinkTypeSize> drinks = new();

        private readonly Timer orderTimer = new();
        private readonly float timeMultiplier;

        public IReadOnlyList<Food> Foods => foods;
        public IReadOnlyList<DrinkTypeSize> Drinks => drinks;
        public bool NeedsHappyBox { get; }
        public bool HasTimer { get; }
        public Timer Timer => orderTimer;

        public Order(
            IReadOnlyList<Food> foods,
            float timeMultiplier = 1.0f,
            bool happyBox = false,
            bool hasTimer = true
        )
        {
            this.foods.AddRange(foods);
            this.timeMultiplier = timeMultiplier;
            NeedsHappyBox = happyBox;
            HasTimer = hasTimer;
        }

        public Order(
            IReadOnlyList<DrinkTypeSize> drinks,
            float timeMultiplier = 1.0f,
            bool happyBox = false,
            bool hasTimer = true
        )
        {
            this.drinks.AddRange(drinks);
            this.timeMultiplier = timeMultiplier;
            NeedsHappyBox = happyBox;
            HasTimer = hasTimer;
        }

        public Order(
            IReadOnlyList<Food> foods,
            IReadOnlyList<DrinkTypeSize> drinks,
            float timeMultiplier = 1.0f,
            bool happyBox = false,
            bool hasTimer = true
        )
        {
            this.foods.AddRange(foods);
            this.drinks.AddRange(drinks);
            this.timeMultiplier = timeMultiplier;
            NeedsHappyBox = happyBox;
            HasTimer = hasTimer;
        }

        public void StartTimer()
        {
            if (HasTimer)
                orderTimer.Start(GetExpectedWaitTime());
        }

        public float GetExpectedWaitTime()
        {
            float foodTime = 0;

            foreach (var food in foods)
                foodTime += timeMultiplier * food.CookTime;

            return BASE_TIME + foodTime + (timeMultiplier * DRINK_TIME * drinks.Count);
        }

        public bool TakeFood(Food food)
        {
            int index = -1;

            for (int i = 0; i < foods.Count; i++)
            {
                if (food.ID == foods[i].ID)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
                return false;

            foods.RemoveAt(index);
            return true;
        }

        public bool TakeDrink(Drinkable drink)
        {
            for (int i = 0; i < drinks.Count; i++)
            {
                var desiredDrink = drinks[i];

                if (drink.IsSameDrink(desiredDrink) && drink.IsFull && drink.HasLid)
                {
                    drinks.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public bool IsEmpty()
        {
            return Foods.Count == 0 && Drinks.Count == 0;
        }
    }
}
