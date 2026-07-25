using System.Collections.Generic;
using Shears;
using UnityEngine;

namespace BigHappyBurger.Interaction
{
    public class ItemSpawnContainer : MonoBehaviour, IItemSpawner
    {
        [SerializeField, Required, Local]
        private Item itemPrefab;

        [SerializeField, Required, Local]
        private Transform container;

        [SerializeField]
        private float planeOffset;

        [SerializeField, Min(0)]
        private int maxCount;

        [SerializeField]
        private List<Item> spawnedItems = new();

        public int MaxCount => maxCount;
        public Item ItemToSpawn => itemPrefab;

        private void Awake()
        {
            foreach (var item in spawnedItems)
                LockItem(item);
        }

        public void AddCount(int count)
        {
            int countToAdd = count;

            if (spawnedItems.Count + countToAdd > maxCount)
                countToAdd = maxCount - spawnedItems.Count;

            for (int i = 0; i < countToAdd; i++)
            {
                var instance = Instantiate(itemPrefab, container);
                spawnedItems.Add(instance);

                LockItem(instance);
            }
        }

        private void LockItem(Item item)
        {
            if (item.TryGetComponent(out Rigidbody rigidbody))
                rigidbody.isKinematic = true;

            item.LockDragging();
        }

        public ItemSpawner.ItemSpawnInfo Release()
        {
            if (spawnedItems.Count == 0)
                return new();
            else
            {
                var item = spawnedItems[^1];
                spawnedItems.RemoveAt(spawnedItems.Count - 1);

                item.SetParent(null);

                if (item.TryGetComponent(out Rigidbody rigidbody))
                    rigidbody.isKinematic = false;

                item.UnlockDragging();

                return new(item, Vector3.zero, -planeOffset);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + planeOffset * transform.forward, 0.1f);
        }
    }
}
