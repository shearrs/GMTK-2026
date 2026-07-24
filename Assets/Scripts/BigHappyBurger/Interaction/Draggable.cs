using Shears;
using UnityEngine;

namespace BigHappyBurger.Interaction
{
    [RequireComponent(typeof(Item))]
    public class Draggable : MonoBehaviour
    {
        private const float INTERPOLATION_DURATION = 5.0f;

        [SerializeField, Required, Local]
        private PhysicsMaterial normalBounceMaterial;

        [SerializeField, Required, Local]
        private PhysicsMaterial noBounceMaterial;

        [SerializeField, Required, Local]
        private Rigidbody rigidbody;

        [SerializeField, Required(targetCollectionSize: 1), Local]
        private Collider[] colliders;

        [SerializeField, Min(0)]
        private float springiness = 500.0f;

        [SerializeField, Min(0)]
        private float damping = 25.0f;

        [SerializeField, Min(0)]
        private float angularSpringiness = 0.1f;

        [SerializeField, Min(0)]
        private float angularDamping = 10.0f;

        private bool isBeingDragged = false;
        private bool previouslyUsedGravity = false;
        private readonly Timer interpolateTimer = new(INTERPOLATION_DURATION);
        private Vector3 targetPosition;
        private Quaternion targetRotation;

        private void Reset()
        {
            var item = GetComponent<Item>();
            TryGetComponent(out rigidbody);

            colliders = GetComponentsInChildren<Collider>();

            item.SetDraggable(this);
        }

        private void Awake()
        {
            interpolateTimer.Completed += () =>
                rigidbody.interpolation = RigidbodyInterpolation.None;
        }

        public void OnDragBegin()
        {
            if (isBeingDragged)
                return;

            previouslyUsedGravity = rigidbody.useGravity;

            foreach (var collider in colliders)
                collider.material = noBounceMaterial;

            rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            rigidbody.useGravity = false;
            isBeingDragged = true;
        }

        public void OnDragEnd()
        {
            if (!isBeingDragged)
                return;

            foreach (var collider in colliders)
                collider.material = normalBounceMaterial;

            interpolateTimer.Restart();
            rigidbody.useGravity = previouslyUsedGravity;
            isBeingDragged = false;
        }

        public void SetTargetPosition(Vector3 position)
        {
            targetPosition = position;
        }

        public void SetTargetRotation(Quaternion rotation)
        {
            targetRotation = rotation;
        }

        private void FixedUpdate()
        {
            if (isBeingDragged)
            {
                MoveTowardsTarget();
                RotateTowardsTarget();
            }
        }

        private void MoveTowardsTarget()
        {
            var heading = targetPosition - rigidbody.position;
            float distance = heading.magnitude;

            Vector3 springForce = Vector3.zero;

            if (!Mathf.Approximately(distance, 0.0f))
            {
                var direction = heading / distance;
                springForce += springiness * distance * direction;
            }

            Vector3 dampForce = damping * -rigidbody.linearVelocity;

            rigidbody.AddForceAtPosition(springForce, targetPosition, ForceMode.Force);
            rigidbody.AddForceAtPosition(dampForce, targetPosition, ForceMode.Acceleration);
        }

        private void RotateTowardsTarget()
        {
            var heading = targetRotation * Quaternion.Inverse(rigidbody.rotation);

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
            if (!isBeingDragged)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(targetPosition, 0.25f);
        }
    }
}
