using System.Collections;
using BigHappyBurger.Customers;
using BigHappyBurger.Players;
using BigHappyBurger.Restaurants;
using Shears;
using UnityEngine;

namespace BigHappyBurger.GameManagement
{
    public class GameManager : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField]
        private Player player;

        [SerializeField]
        private CustomerManager customerManager;

        [SerializeField]
        private Restaurant restaurant;

        [Header("Levels")]
        [SerializeField]
        private LevelData levelData;

        private readonly Timer customerCreateTimer = new();

        private void Start()
        {
            StartCoroutine(IEPlayLevel(levelData));
        }

        private IEnumerator IEPlayLevel(LevelData data)
        {
            int remainingCustomersToServe = data.CustomerCount;
            int remainingCustomersToCreate = data.CustomerCount;

            customerCreateTimer.Start(data.FirstCustomerArrivalTime);

            while (remainingCustomersToServe > 0)
            {
                if (remainingCustomersToCreate > 0)
                {
                    if (TryCreateCustomer(out var customer))
                    {
                        var order = data.GetRandomOrder();
                        customer.SetOrder(order);
                        restaurant.AddOrder(order);

                        remainingCustomersToCreate--;
                        customerCreateTimer.Start(data.CustomerArrivalRange.Random());
                    }
                }

                if (customerManager.CanSpawnCustomer())
                    customerManager.SpawnCustomer();

                yield return null;
            }
        }

        private bool TryCreateCustomer(out Customer customer)
        {
            customer = null;

            if (customerCreateTimer.IsDone)
            {
                customer = customerManager.CreateCustomer();
                return true;
            }

            return false;
        }
    }
}
