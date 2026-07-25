using BigHappyBurger.Interaction;
using UnityEngine;

namespace BigHappyBurger.Interaction
{
    public class ItemSpawner : MonoBehaviour
    {
        [SerializeField]
        private Item itemToSpawn;

        [SerializeField]
        private Transform spawnTransform;

        public void SpawnItem()
        {
            var spawnedItem = Instantiate(itemToSpawn, spawnTransform.position, spawnTransform.rotation);
        }
    }
}
