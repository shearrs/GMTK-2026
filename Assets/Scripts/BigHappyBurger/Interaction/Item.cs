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
        [SerializeField, Local]
        private Rigidbody rigidbody;

        [SerializeField, Local]
        private Draggable draggable;

        [SerializeField, Local]
        private Holdable holdable;

        public bool IsDraggable => draggable != null && (holdable == null || !holdable.IsLocked);

        private void Reset()
        {
            TryGetComponent(out rigidbody);
        }

        public void OnDragBegin() => draggable.OnDragBegin();

        public void OnDragEnd() => draggable.OnDragEnd();

        public void SetDragPosition(Vector3 position) => draggable.SetTargetPosition(position);

        public void SetDragRotation(Quaternion rotation) => draggable.SetTargetRotation(rotation);

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

        internal void SetDraggable(Draggable draggable) => this.draggable = draggable;

        internal void SetHoldable(Holdable holdable) => this.holdable = holdable;
    }
}
