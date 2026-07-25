using System.Collections.Generic;
using BigHappyBurger.Foods;
using UnityEngine;

namespace BigHappyBurger.Restaurants
{
    [System.Serializable]
    public class Order
    {
        [SerializeField]
        private Food[] foods;

        public IReadOnlyList<Food> Foods => foods;

        public Order(Food[] foods)
        {
            this.foods = foods;
        }
    }
}
