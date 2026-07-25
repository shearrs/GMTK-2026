using System;
using Shears;
using UnityEngine;

namespace BigHappyBurger.Interaction
{
    public class ItemHolder : MonoBehaviour
    {
        [field: SerializeField]
        public bool IsLocked { get; private set; }

        [SerializeField]
        private bool disableCollision;

        [SerializeField]
        private Transform container;

        [SerializeField]
        private bool overridePlaneDistance;

        [field: SerializeField, ShowIf(nameof(overridePlaneDistance))]
        public float PlaneDistanceOverride { get; private set; }

        private Transform previousParent;

        public Item Item { get; private set; }
        public bool OverridePlaneDistance => overridePlaneDistance;
        public Func<Item, bool> CanBeHeldCallback { get; set; }
        public Func<Item, bool> CanBeReleasedCallback { get; set; }

        public event Action<Item> ItemChanged;

        public bool CanBeHeld(Item item)
        {
            bool callbackValue = CanBeHeldCallback == null || CanBeHeldCallback(item);

            return Item == null && !IsLocked && callbackValue;
        }

        public bool CanBeReleased()
        {
            if (Item == null)
                return true;

            bool callbackValue = CanBeReleasedCallback == null || CanBeReleasedCallback(Item);

            return !IsLocked && callbackValue;
        }

        public void Lock() => IsLocked = true;

        public void Unlock() => IsLocked = false;

        public bool Hold(Item item)
        {
            if (Item != null || !CanBeHeld(item))
                return false;

            Item = item;
            previousParent = item.Parent;

            if (disableCollision)
                Item.DisableCollision();

            item.OnHoldBegin(this);
            item.SetParent(container);
            item.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            ItemChanged?.Invoke(item);

            return true;
        }

        public bool Release()
        {
            if (Item == null)
                return true;

            if (!CanBeReleased())
                return false;

            if (disableCollision)
                Item.EnableCollision();

            Item.SetParent(previousParent);
            Item.OnHoldEnd();
            Item = null;

            ItemChanged?.Invoke(Item);

            return true;
        }

        private void OnDrawGizmosSelected()
        {
            if (!overridePlaneDistance)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(
                transform.position + PlaneDistanceOverride * -transform.forward,
                0.15f
            );
        }
    }
}
