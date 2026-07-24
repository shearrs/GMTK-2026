using UnityEngine;

namespace BigHappyBurger.Interaction
{
    [RequireComponent(typeof(Item))]
    public abstract class Holdable : MonoBehaviour
    {
        protected bool IsBeingHeld { get; private set; }
        public ItemHolder Holder { get; internal set; }
        public bool IsLocked => Holder != null && Holder.IsLocked;

        public void OnHoldBegin()
        {
            IsBeingHeld = true;
            OnHoldBeginImplementation();
        }

        public void OnHoldEnd()
        {
            IsBeingHeld = false;
            OnHoldEndImplementation();
        }

        protected abstract void OnHoldBeginImplementation();

        protected abstract void OnHoldEndImplementation();
    }
}
