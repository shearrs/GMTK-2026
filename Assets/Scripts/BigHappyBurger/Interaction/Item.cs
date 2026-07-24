using System;
using Shears;
using Shears.Logging;
using UnityEngine;

namespace BigHappyBurger.Interaction
{
    [SelectionBase]
    public class Item : MonoBehaviour, ISHLoggable
    {
        [field: Header("Logging")]
        [field: SerializeField]
        public SHLogLevels LogLevels { get; set; } = SHLogUtil.Default;

        [Header("Item")]
        [field: SerializeField, ReadOnly]
        public string ID { get; private set; }

        [SerializeField, Local]
        private Rigidbody rigidbody;

        [SerializeField, Local]
        private Draggable draggable;

        [SerializeField, Local]
        private Holdable holdable;

        public Vector3 Position => rigidbody != null ? rigidbody.position : transform.position;
        public Quaternion Rotation => rigidbody != null ? rigidbody.rotation : transform.rotation;
        public Transform Parent => transform.parent;
        public bool IsDraggable => draggable != null && (!IsBeingHeld || holdable.CanBeReleased());
        public bool IsBeingHeld => holdable != null && holdable.IsBeingHeld;

        private void Reset()
        {
            ID = Guid.NewGuid().ToString();

            TryGetComponent(out rigidbody);
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(ID))
                ID = Guid.NewGuid().ToString();
        }

        public void OnDragBegin()
        {
            if (IsBeingHeld)
            {
                if (!holdable.Release())
                {
                    this.Log($"Could not release from holdable!", SHLogLevels.Error);
                    return;
                }
            }

            draggable.OnDragBegin();
        }

        public void OnDragEnd() => draggable.OnDragEnd();

        public void SetDragPosition(Vector3 position) => draggable.SetTargetPosition(position);

        public void SetDragRotation(Quaternion rotation) => draggable.SetTargetRotation(rotation);

        public void OnHoldBegin(ItemHolder holder) => holdable.OnHoldBegin(holder);

        public void OnHoldEnd() => holdable.OnHoldEnd();

        public void SetParent(Transform parent)
        {
            if (rigidbody != null)
                rigidbody.transform.SetParent(parent);
            else
                transform.SetParent(parent);
        }

        public void SetLocalPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            if (rigidbody == null || rigidbody.isKinematic)
                transform.SetLocalPositionAndRotation(position, rotation);
            else
            {
                this.Log(
                    $"Should not be trying to set the transform of an active {nameof(Rigidbody)}.",
                    SHLogLevels.Warning
                );
                return;
            }
        }

        internal void SetRigidbody(Rigidbody rigidbody) => this.rigidbody = rigidbody;

        internal void SetDraggable(Draggable draggable) => this.draggable = draggable;

        internal void SetHoldable(Holdable holdable) => this.holdable = holdable;
    }
}
