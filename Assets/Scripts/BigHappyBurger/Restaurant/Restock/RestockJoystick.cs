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
        private float joystickDistance = 0.28f;

        [SerializeField]
        private Range<float> zHeightRange = new(0.1f, 0.25f);

        [SerializeField]
        private float minHeight = 0.15f;

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
            var heading = (position - ballPivot.position);
            var distance = heading.magnitude;
            var targetDirection = heading / distance;
            float yOffset = heading.y;

            if (yOffset < zHeightRange.Min)
            {
                targetDirection += 3.0f * (zHeightRange.Min - yOffset) * transform.forward;
                targetDirection.Normalize();
            }
            else if (yOffset > zHeightRange.Max)
            {
                targetDirection += 3.0f * (yOffset - zHeightRange.Max) * -transform.forward;
                targetDirection.Normalize();
            }

            position = ballPivot.position + (joystickDistance * targetDirection);

            if (yOffset < minHeight)
                position.y = ballPivot.position.y + minHeight;

            return position;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.TransformPoint(defaultPosition), 0.15f);
        }
    }
}
