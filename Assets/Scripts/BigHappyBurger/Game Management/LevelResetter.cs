using BigHappyBurger.Customers;
using BigHappyBurger.Interaction;
using BigHappyBurger.Restaurants;
using Shears;
using UnityEngine;

namespace BigHappyBurger.GameManagement
{
    public class LevelResetter : MonoBehaviour
    {
        [SerializeField, Required(targetCollectionSize: 1)]
        private ItemSpawnContainer[] itemSpawnContainers;

        [SerializeField, Required(targetCollectionSize: 1)]
        private ItemSpawner[] itemSpawners;

        [SerializeField, Required]
        private CustomerManager customerManager;

        [SerializeField, Required]
        private Restaurant restaurant;

        [ContextMenu("Reset Level")]
        public void ResetLevel()
        {
            var items = FindObjectsByType<Item>();

            foreach (var item in items)
            {
                if (item.Spawned)
                    Destroy(item.gameObject);
            }

            foreach (var container in itemSpawnContainers)
                container.AddCount(container.MaxCount);

            foreach (var spawner in itemSpawners)
                spawner.AddCount(spawner.MaxCount);

            customerManager.ClearCustomers();
            restaurant.Clear();
        }
    }
}
