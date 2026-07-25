using UnityEngine;

namespace BigHappyBurger.Interaction
{
    public class ItemSpawner : MonoBehaviour
    {
        [SerializeField]
        private Item itemToSpawn;

        [SerializeField]
        private Vector3 dragOffset;

        [SerializeField]
        private Transform spawnTransform;

        public readonly ref struct ItemSpawnInfo
        {
            public Item Item { get; }
            public Vector3 Offset { get; }

            public ItemSpawnInfo(Item item, Vector3 offset)
            {
                Item = item;
                Offset = offset;
            }
        }

        public ItemSpawnInfo SpawnItem()
        {
            var item = Instantiate(itemToSpawn, spawnTransform.position, spawnTransform.rotation);

            return new(item, transform.rotation * dragOffset);
        }
    }
}
