using System;
using System.Collections.Generic;
using BigHappyBurger.Foods;
using Shears;
using UnityEngine;

namespace BigHappyBurger.Restaurants
{
    public class Restaurant : MonoBehaviour
    {
        [SerializeField, Required, Local]
        private Chef chef;

        [SerializeField, Required, Local]
        private FoodConveyor conveyor;

        [SerializeField, Required, Local]
        private DrinkMachine drinkMachine;

        [SerializeField, ReadOnly]
        private List<Order> orders = new();

        private int badMarks = 0;

        public event Action<IReadOnlyCollection<Order>> OrdersChanged;

        private void Update()
        {
            conveyor.UpdateConveyor();
            chef.UpdateCooking();
        }

        public void AddOrder(Order order)
        {
            orders.Add(order);
            chef.AddOrder(order);

            foreach (var drink in order.Drinks)
                drinkMachine.EnqueueDrink(drink);

            OrdersChanged?.Invoke(orders);
        }

        public void RemoveOrder(Order order)
        {
            orders.Remove(order);

            OrdersChanged?.Invoke(orders);
        }

        public void AddBadMark()
        {
            badMarks++;

            if (badMarks >= 3)
            {
                Debug.Log("3 STRIKES, YOU'RE OUT");
            }
        }
    }
}
