using System;
using System.Collections.Generic;
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

        [SerializeField, ReadOnly]
        private List<Order> orders = new();

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

            OrdersChanged?.Invoke(orders);
        }

        public void RemoveOrder(Order order)
        {
            orders.Remove(order);

            OrdersChanged?.Invoke(orders);
        }
    }
}
