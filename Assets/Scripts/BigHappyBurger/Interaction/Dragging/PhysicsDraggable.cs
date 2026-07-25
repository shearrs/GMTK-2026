using Shears;
using UnityEngine;

namespace BigHappyBurger.Interaction
{
    [RequireComponent(typeof(Rigidbody), typeof(Item))]
    public class PhysicsDraggable : Draggable
    {
        private const float INTERPOLATION_DURATION = 5.0f;

        [SerializeField, Required, Local]
        private PhysicsMaterial normalBounceMaterial;

        [SerializeField, Required, Local]
        private PhysicsMaterial noBounceMaterial;

        [SerializeField, Required, Local]
        private Item item;

        [SerializeField, Required, Local]
        private Rigidbody rigidbody;

        [SerializeField, Min(0)]
        private float springiness = 800.0f;

        [SerializeField, Min(0)]
        private float damping = 25.0f;

        [SerializeField, Min(0)]
        private float angularSpringiness = 0.1f;

        [SerializeField, Min(0)]
        private float angularDamping = 20.0f;

        private readonly Timer interpolateTimer = new(INTERPOLATION_DURATION);

        private void Reset()
        {
            item = GetComponent<Item>();
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            item.SetRigidbody(rigidbody);

            item.SetDraggable(this);
        }

        private void Awake()
        {
            interpolateTimer.Completed += () =>
                rigidbody.interpolation = RigidbodyInterpolation.None;
        }

        protected override void OnDragBeginImplementation()
        {
            foreach (var collider in item.Colliders)
                collider.material = noBounceMaterial;

            rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }

        protected override void OnDragEndImplementation()
        {
            foreach (var collider in item.Colliders)
                collider.material = normalBounceMaterial;

            interpolateTimer.Restart();
        }

        private void FixedUpdate()
        {
            if (IsBeingDragged)
            {
                MoveTowardsTarget();
                RotateTowardsTarget();
            }
        }

        private void MoveTowardsTarget()
        {
            var heading = TargetPosition - rigidbody.position;
            float distance = heading.magnitude;

            Vector3 springForce = Vector3.zero;

            if (!Mathf.Approximately(distance, 0.0f))
            {
                var direction = heading / distance;
                springForce += springiness * distance * direction;
            }

            Vector3 dampForce = damping * -rigidbody.linearVelocity;

            rigidbody.AddForce(springForce, ForceMode.Force);
            rigidbody.AddForce(dampForce, ForceMode.Acceleration);
        }

        private void RotateTowardsTarget()
        {
            var heading = TargetRotation * Quaternion.Inverse(rigidbody.rotation);

            heading.ToAngleAxis(out float angle, out var axis);

            if (angle > 180f)
                angle -= 360f;

            var springForce = Vector3.zero;

            if (!Mathf.Approximately(angle, 0.0f))
                springForce = angularSpringiness * angle * axis;

            var dampForce = angularDamping * -rigidbody.angularVelocity;

            rigidbody.AddTorque(springForce, ForceMode.Force);
            rigidbody.AddTorque(dampForce, ForceMode.Acceleration);
        }

        private void OnDrawGizmosSelected()
        {
            if (!IsBeingDragged)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(TargetPosition, 0.25f);
        }
    }
}
