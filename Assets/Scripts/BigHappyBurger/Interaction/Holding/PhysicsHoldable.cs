using Shears;
using UnityEngine;

namespace BigHappyBurger.Interaction
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsHoldable : Holdable
    {
        [SerializeField, Required, Local]
        private Rigidbody rigidbody;

        private bool wasKinematic = false;

        private void Reset()
        {
            TryGetComponent(out Item item);
            rigidbody = GetComponent<Rigidbody>();

            item.SetHoldable(this);
        }

        protected override void OnHoldEndImplementation()
        {
            wasKinematic = rigidbody.isKinematic;

            rigidbody.interpolation = RigidbodyInterpolation.None;
            rigidbody.isKinematic = true;
        }

        protected override void OnHoldBeginImplementation()
        {
            rigidbody.isKinematic = wasKinematic;
        }
    }
}
