using UnityEngine;

namespace BigHappyBurger.Interaction
{
    [RequireComponent(typeof(Item))]
    public abstract class Holdable : MonoBehaviour
    {
        public bool IsBeingHeld { get; private set; }
        public ItemHolder Holder { get; private set; }
        public bool IsLocked => Holder != null && Holder.IsLocked;

        public void OnHoldBegin(ItemHolder holder)
        {
            IsBeingHeld = true;
            Holder = holder;
            OnHoldBeginImplementation();
        }

        public void OnHoldEnd()
        {
            IsBeingHeld = false;
            Holder = null;
            OnHoldEndImplementation();
        }

        public bool CanBeReleased() => Holder == null || Holder.CanBeReleased();

        public bool Release()
        {
            if (Holder == null)
                return true;

            return Holder.Release();
        }

        protected abstract void OnHoldBeginImplementation();

        protected abstract void OnHoldEndImplementation();
    }
}
