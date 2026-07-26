using System.Collections.Generic;
using BigHappyBurger.Foods;
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

        private readonly List<DrinkTypeSize> drinks = new();
        private readonly float timeMultiplier;

        public IReadOnlyList<Food> Foods => foods;
        public IReadOnlyList<DrinkTypeSize> Drinks => drinks;

        public Order(
            IReadOnlyList<Food> foods,
            IReadOnlyList<DrinkTypeSize> drinks,
            float timeMultiplier = 1.0f
        )
        {
            this.foods.AddRange(foods);
            this.drinks.AddRange(drinks);
            this.timeMultiplier = timeMultiplier;
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

                if (drink.IsSameDrink(desiredDrink))
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
