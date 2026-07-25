using BigHappyBurger.Interaction;
using Shears;
using UnityEngine;

namespace BigHappyBurger.Restaurants
{
    public class RestockJoystick : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField, Required]
        private RestockMonitor monitor;

        [SerializeField, Required, Local]
        private Transform ballPivot;

        [SerializeField, Required, Local]
        private Transform ballTarget;

        [SerializeField, Required]
        private ManualDraggable draggable;

        [Header("Settings")]
        [SerializeField]
        private Vector3 defaultPosition;

        [SerializeField]
        private Range<float> distanceRange = new(0.05f, 0.25f);

        [SerializeField]
        private float minHeightDifference = 0.15f;

        [SerializeField]
        private float cursorSensitivity = 0.01f;

        [SerializeField]
        private float returnSpeed = 1.0f;

        private void Awake()
        {
            draggable.PositionValidateCallback = ValidatePosition;
        }

        private void Update()
        {
            if (!draggable.IsBeingDragged)
                return;

            var heading = ballTarget.localPosition - ballPivot.localPosition;
            var distance = heading.magnitude;
            var direction = heading / distance;
            var monitorDirection = new Vector2(direction.x, -direction.z);

            monitor.MoveCursor(cursorSensitivity * distance * monitorDirection);
        }

        private void FixedUpdate()
        {
            if (!draggable.IsBeingDragged)
            {
                var defaultPos = transform.TransformPoint(defaultPosition);

                if (ballTarget.position != defaultPos)
                    ballTarget.position = Vector3.MoveTowards(
                        ballTarget.position,
                        defaultPos,
                        Time.deltaTime * returnSpeed
                    );
            }

            var direction = (ballTarget.position - ballPivot.position).normalized;
            ballPivot.transform.up = direction;
        }

        private Vector3 ValidatePosition(Vector3 position)
        {
            float heightDifference = position.y - ballPivot.position.y;

            if (heightDifference < minHeightDifference)
                position.y = ballPivot.position.y + minHeightDifference;

            var heading = (position - ballPivot.position);
            var distance = heading.magnitude;
            var targetDirection = heading / distance;

            distance = distanceRange.Clamp(distance);

            position = ballPivot.position + (distance * targetDirection);

            return position;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.TransformPoint(defaultPosition), 0.15f);
        }
    }
}
