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

        public IReadOnlyList<Order> Orders => orders;

        public event Action<IReadOnlyCollection<Order>> OrdersChanged;
        public event Action<Order> OrderAdded;
        public event Action<Order> OrderRemoved;
        public event Action<int> MarksChanged;

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

            OrderAdded?.Invoke(order);
            OrdersChanged?.Invoke(orders);
        }

        public void RemoveOrder(Order order)
        {
            orders.Remove(order);

            OrderRemoved?.Invoke(order);
            OrdersChanged?.Invoke(orders);
        }

        public void AddBadMark()
        {
            badMarks++;

            MarksChanged?.Invoke(badMarks);
        }

        public void Clear()
        {
            while (orders.Count > 0)
                RemoveOrder(orders[0]);

            drinkMachine.Clear();

            chef.Clear();
        }

        public void AddTime(float time)
        {
            foreach (var order in orders)
            {
                float remainingTime = order.Timer.Time - order.Timer.CurrentTime;
                remainingTime += time;

                order.Timer.Stop();
                order.Timer.Start(remainingTime);
            }
        }
    }
}
