using System;
using Shears;
using UnityEngine;

namespace BigHappyBurger.Interaction
{
    public class ItemSpawner : MonoBehaviour, IItemSpawner
    {
        [SerializeField, Required]
        private Item itemToSpawn;

        [SerializeField, Required]
        private Collider interactionCollider;

        [SerializeField, Required]
        private Transform spawnTransform;

        [SerializeField]
        private bool hasLimitedSupply = false;

        [SerializeField, ShowIf(nameof(hasLimitedSupply))]
        private int maxCount;

        [SerializeField, ShowIf(nameof(hasLimitedSupply))]
        private int currentCount;

        [SerializeField]
        private Vector3 dragOffset;

        [SerializeField]
        private float planeDistanceOffset;

        public int MaxCount => maxCount;
        public Item ItemToSpawn => itemToSpawn;

        public event Action<int> CountChanged;

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

            if (hasLimitedSupply)
            {
                currentCount--;

                if (currentCount == 0)
                    interactionCollider.enabled = false;

                CountChanged?.Invoke(currentCount);
            }

            return new(item, transform.rotation * dragOffset, planeDistanceOffset);
        }

        public void AddCount(int count)
        {
            if (!hasLimitedSupply)
                return;

            if (currentCount + count > maxCount)
                count = maxCount - currentCount;

            currentCount += count;
            if (currentCount > 0 && !interactionCollider.enabled)
                interactionCollider.enabled = true;

            CountChanged?.Invoke(currentCount);
        }
    }
}
