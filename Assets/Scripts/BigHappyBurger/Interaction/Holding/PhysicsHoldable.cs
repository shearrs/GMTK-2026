using Shears;
using UnityEngine;

namespace BigHappyBurger.Interaction
{
    [RequireComponent(typeof(Rigidbody)), DisallowMultipleComponent]
    public class PhysicsHoldable : Holdable
    {
        [SerializeField, Required, Local]
        private Rigidbody rigidbody;

        private bool wasKinematic = false;

        private void Reset()
        {
            TryGetComponent(out Item item);
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            item.SetRigidbody(rigidbody);

            item.SetHoldable(this);
        }

        protected override void OnHoldBeginImplementation()
        {
            wasKinematic = rigidbody.isKinematic;

            rigidbody.interpolation = RigidbodyInterpolation.None;
            rigidbody.isKinematic = true;
        }

        protected override void OnHoldEndImplementation()
        {
            rigidbody.isKinematic = wasKinematic;
        }
    }
}
