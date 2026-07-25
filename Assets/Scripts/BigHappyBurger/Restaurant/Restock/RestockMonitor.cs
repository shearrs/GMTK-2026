using System.Collections;
using System.Collections.Generic;
using BigHappyBurger.Interaction;
using Shears;
using Shears.Logging;
using Shears.UI;
using TMPro;
using UnityEngine;

namespace BigHappyBurger.Restaurants
{
    public class RestockMonitor : MonoBehaviour
    {
        [Header("Screen")]
        [SerializeField, Required, Local]
        private SpriteRenderer icon;

        [SerializeField, Required, Local]
        private GameObject buyScreenContainer;

        [SerializeField, Required, Local]
        private GameObject deliveringScreenContainer;

        [SerializeField, Required, Local]
        private TextMeshPro countdownText;

        [SerializeField, Required, Local]
        private RestockCursor cursor;

        [SerializeField, Required, Local]
        private RestockButton leftButton;

        [SerializeField, Required, Local]
        private RestockButton rightButton;

        [SerializeField, Required, Local]
        private RestockButton buyButton;

        [Header("Items")]
        [SerializeField, Required]
        private ItemSpawnContainer bbqSpawner;

        [SerializeField, Required]
        private ItemSpawnContainer snsSpawner;

        [SerializeField, Required]
        private ItemSpawnContainer ketchupSpawner;

        [SerializeField, Required]
        private ItemSpawnContainer appleSpawner;

        [SerializeField, Required]
        private ItemSpawner napkinSpawner;

        [SerializeField, Required(targetCollectionSize: 5), Local]
        private Item[] restockableItems;

        [Header("Settings")]
        [SerializeField, Local]
        private float deliveryTime = 10.0f;

        [SerializeField, Local]
        private Range<float> countdownTextRange = new(1.5f, 2.25f);

        [SerializeField, Local]
        private float screenDepth;

        [SerializeField, Local]
        private Vector2 minExtents;

        [SerializeField, Local]
        private Vector2 maxExtents;

        private readonly Timer deliveryTimer = new();
        int currentSelection = 0;
        private RestockButton currentHover;

        private Bounds Bounds
        {
            get
            {
                var midPoint = (Vector3)(0.5f * (minExtents + maxExtents));

                float length = maxExtents.x - minExtents.x;
                float height = maxExtents.y - minExtents.y;

                return new(midPoint, new(length, height, 0.01f));
            }
        }

        [ContextMenu("Click")]
        public void Click()
        {
            if (!deliveryTimer.IsDone)
                return;

            if (currentHover == null)
                return;
            else if (currentHover == leftButton)
                MoveSelection(-1);
            else if (currentHover == rightButton)
                MoveSelection(1);
            else if (currentHover == buyButton)
                Buy();
        }

        public void MoveCursor(Vector2 movement)
        {
            if (!deliveryTimer.IsDone)
                return;

            cursor.Move(movement, Bounds);

            if (cursor.IsOverlapping(leftButton.Bounds))
                Hover(leftButton);
            else if (cursor.IsOverlapping(rightButton.Bounds))
                Hover(rightButton);
            else if (cursor.IsOverlapping(buyButton.Bounds))
                Hover(buyButton);
            else if (currentHover != null)
                Hover(null);
        }

        private void Hover(RestockButton button)
        {
            if (button == currentHover)
                return;

            if (currentHover != null)
                currentHover.Unhover();

            currentHover = button;

            if (currentHover != null)
                currentHover.Hover();
        }

        private void MoveSelection(int direction)
        {
            int newSelection = currentSelection + direction;

            if (newSelection < 0)
                newSelection = restockableItems.Length - 1;
            else if (newSelection == restockableItems.Length)
                newSelection = 0;

            currentSelection = newSelection;
            icon.sprite = restockableItems[currentSelection].Sprite;
        }

        private void Buy()
        {
            if (!deliveryTimer.IsDone)
                return;

            StopAllCoroutines();
            StartCoroutine(IEBuy());
        }

        private IEnumerator IEBuy()
        {
            buyScreenContainer.SetActive(false);
            deliveringScreenContainer.SetActive(true);
            deliveryTimer.Start(deliveryTime);

            while (!deliveryTimer.IsDone)
            {
                countdownText.text = Mathf
                    .RoundToInt(deliveryTimer.Time - deliveryTimer.CurrentTime)
                    .ToString();
                countdownText.fontSize = countdownTextRange.Lerp(deliveryTimer.Percentage);
                yield return null;
            }

            buyScreenContainer.SetActive(true);
            deliveringScreenContainer.SetActive(false);

            IItemSpawner spawner = null;
            var item = restockableItems[currentSelection];

            if (bbqSpawner.ItemToSpawn == item)
                spawner = bbqSpawner;
            else if (snsSpawner.ItemToSpawn == item)
                spawner = snsSpawner;
            else if (ketchupSpawner.ItemToSpawn == item)
                spawner = ketchupSpawner;
            else if (appleSpawner.ItemToSpawn == item)
                spawner = appleSpawner;
            else if (napkinSpawner.ItemToSpawn == item)
                spawner = napkinSpawner;

            if (spawner == null)
            {
                SHLogger.Log($"Failed to map item to spawner: {item}", SHLogLevels.Error);
                yield break;
            }

            int count = spawner.MaxCount;
            spawner.AddCount(count);
        }

        private void OnDrawGizmosSelected()
        {
            var matrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

            Gizmos.DrawWireCube(Bounds.center.With(z: screenDepth), Bounds.size);

            Gizmos.matrix = matrix;
        }
    }
}
