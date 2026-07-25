using System;
using UnityEngine;

namespace BigHappyBurger.Interaction
{
    [RequireComponent(typeof(Item))]
    public class ManualDraggable : Draggable
    {
        [SerializeField]
        private Item item;

        public Func<Vector3, Vector3> PositionValidateCallback;

        private void Reset()
        {
            item = GetComponent<Item>();
            item.SetDraggable(this);
        }

        protected override void OnDragBeginImplementation() { }

        protected override void OnDragEndImplementation() { }

        private void Update()
        {
            if (IsBeingDragged)
                MoveTowardsTarget();
        }

        private void MoveTowardsTarget()
        {
            Vector3 targetPosition = TargetPosition;

            if (PositionValidateCallback != null)
                targetPosition = PositionValidateCallback(targetPosition);

            item.SetPosition(targetPosition);
        }
    }
}
