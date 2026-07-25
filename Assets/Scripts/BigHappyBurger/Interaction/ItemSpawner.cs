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
            public Vector3 PointerOffset { get; }
            public float PlaneDistanceOffset { get; }

            public ItemSpawnInfo(Item item, Vector3 offset, float planeDistanceOffset = 0)
            {
                Item = item;
                PointerOffset = offset;
                PlaneDistanceOffset = planeDistanceOffset;
            }
        }

        public ItemSpawnInfo SpawnItem()
        {
            var item = Instantiate(itemToSpawn, spawnTransform.position, spawnTransform.rotation);

            return new(item, transform.rotation * dragOffset);
        }
    }
}
