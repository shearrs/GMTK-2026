using System;
using System.Collections;
using System.Collections.Generic;
using BigHappyBurger.Restaurants;
using Shears;
using Shears.Beziers;
using Shears.Logging;
using UnityEngine;

namespace BigHappyBurger.Customers
{
    public class CustomerManager : MonoBehaviour
    {
        private const float CUSTOMER_TRAVEL_TIME = 3.0f;

        [Header("Components")]
        [SerializeField, Required]
        private Restaurant restaurant;

        [Header("Spawning")]
        [SerializeField, Required]
        private Transform spawnTransform;

        [SerializeField, Required]
        private Bezier entryBezier;

        [SerializeField, Required]
        private Bezier exitBezier;

        [SerializeField, Required, Local]
        private Customer customerPrefab;

        [SerializeField, Required(targetCollectionSize: 1), Local]
        private Car[] carOptions;

        private readonly Timer travelTimer = new(CUSTOMER_TRAVEL_TIME);
        private readonly List<Customer> unspawnedCustomers = new();
        private Customer activeCustomer;

        public int UnspawnedCustomerCount => unspawnedCustomers.Count;

        public event Action CustomerArrivedAtWindow;
        public event Action CustomerCorrectlyServed;

        public Customer CreateCustomer()
        {
            var customer = Instantiate(customerPrefab);

            customer.SetCar(MakeRandomCar());
            customer.gameObject.SetActive(false);

            unspawnedCustomers.Add(customer);

            customer.Exited += OnCustomerExited;
            customer.CorrectlyServed += OnCustomerServed;

            return customer;
        }

        public bool CanSpawnCustomer() => unspawnedCustomers.Count > 0 && activeCustomer == null;

        public void SpawnCustomer()
        {
            if (activeCustomer != null)
            {
                SHLogger.Log("Customer Manager still has an active customer!", SHLogLevels.Error);
                return;
            }
            else if (unspawnedCustomers.Count == 0)
            {
                SHLogger.Log("Customer Manager has no customers to spawn!", SHLogLevels.Error);
                return;
            }

            activeCustomer = unspawnedCustomers[0];
            unspawnedCustomers.RemoveAt(0);
            activeCustomer.Spawn(restaurant, exitBezier);
            StartCoroutine(IEMoveCustomerIn(activeCustomer));
        }

        public void ClearCustomers()
        {
            foreach (var customer in unspawnedCustomers)
                customer.Dispose();

            activeCustomer = null;
            unspawnedCustomers.Clear();
        }

        private Car MakeRandomCar()
        {
            var prefab = carOptions.Random();

            var car = Instantiate(prefab);
            car.transform.SetPositionAndRotation(spawnTransform.position, spawnTransform.rotation);
            car.gameObject.SetActive(false);

            return car;
        }

        private IEnumerator IEMoveCustomerIn(Customer customer)
        {
            travelTimer.Start();

            while (!travelTimer.IsDone)
            {
                float t = Mathf.Min(travelTimer.Percentage, 1.0f);
                entryBezier.SampleWithRotation(t, out var position, out var rotation);

                customer.SetPosition(position);
                customer.SetRotation(rotation);

                yield return null;
            }

            customer.SetPosition(entryBezier.Points[^1].Position);
            customer.OnReachedWindow();
            CustomerArrivedAtWindow?.Invoke();
        }

        private void OnCustomerServed()
        {
            CustomerCorrectlyServed?.Invoke();
        }

        private void OnCustomerExited(Customer customer)
        {
            customer.Exited -= OnCustomerExited;

            if (customer == activeCustomer)
                activeCustomer = null;

            restaurant.RemoveOrder(customer.Order);
            customer.Dispose();
        }
    }
}
