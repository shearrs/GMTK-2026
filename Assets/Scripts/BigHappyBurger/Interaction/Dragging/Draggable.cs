using Shears;
using UnityEngine;

namespace BigHappyBurger.Interaction
{
    [RequireComponent(typeof(Item))]
    public abstract class Draggable : MonoBehaviour
    {
        public bool IsBeingDragged { get; private set; }
        protected Vector3 TargetPosition { get; private set; }
        protected Quaternion TargetRotation { get; private set; }

        public void OnDragBegin()
        {
            if (IsBeingDragged)
                return;

            IsBeingDragged = true;
            OnDragBeginImplementation();
        }

        public void OnDragEnd()
        {
            if (!IsBeingDragged)
                return;

            IsBeingDragged = false;
            OnDragEndImplementation();
        }

        protected abstract void OnDragBeginImplementation();

        protected abstract void OnDragEndImplementation();

        public void SetTargetPosition(Vector3 position)
        {
            TargetPosition = position;
        }

        public void SetTargetRotation(Quaternion rotation)
        {
            TargetRotation = rotation;
        }
    }
}
