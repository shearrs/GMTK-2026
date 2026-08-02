using System.Collections;
using BigHappyBurger.Customers;
using BigHappyBurger.Players;
using BigHappyBurger.Restaurants;
using Shears;
using UnityEngine;

namespace BigHappyBurger.GameManagement
{
    public partial class GameManager : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField]
        private Player player;

        [SerializeField]
        private CustomerManager customerManager;

        [SerializeField]
        [AutoEvent(nameof(CustomerDialogue.TimeAddRequested), nameof(OnTimeAddRequested))]
        private CustomerDialogue dialogue;

        [SerializeField]
        private Restaurant restaurant;

        [SerializeField]
        [AutoEvent(nameof(TutorialSequence.TutorialFinished), nameof(StartLevel))]
        [AutoEvent(nameof(TutorialSequence.HappyBoxEnabled), nameof(OnHappyBoxEnabled))]
        private TutorialSequence tutorial;

        [Header("Levels")]
        [SerializeField]
        private LevelData levelData;

        private readonly Timer customerCreateTimer = new();
        private bool canSpawnHappyBox = false;

        private void StartLevel()
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
                        var order = data.GetRandomOrder(canSpawnHappyBox);
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

        private void OnTimeAddRequested(float time)
        {
            restaurant.AddTime(time);
        }

        private void OnHappyBoxEnabled()
        {
            canSpawnHappyBox = true;
        }
    }
}
