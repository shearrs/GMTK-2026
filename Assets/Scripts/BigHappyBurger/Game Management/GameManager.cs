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

        private readonly Timer customerSpawnTimer = new();

        private void Start()
        {
            StartCoroutine(IEPlayLevel(levelData));
        }

        private IEnumerator IEPlayLevel(LevelData data)
        {
            int remainingCustomersToCreate = data.CustomerCount;

            while (remainingCustomersToCreate > 0)
            {
                customerSpawnTimer.Start(data.CustomerArrivalRange.Random());

                if (customerSpawnTimer.IsDone)
                {
                    customerManager.CreateCustomer();
                    remainingCustomersToCreate--;

                    if (remainingCustomersToCreate > 0)
                        customerSpawnTimer.Start(data.CustomerArrivalRange.Random());
                }

                yield return null;
            }
        }

        private void TrySpawnCustomer() { }
    }
}
