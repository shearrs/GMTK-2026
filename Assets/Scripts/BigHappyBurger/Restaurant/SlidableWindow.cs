using BigHappyBurger.Interaction;
using Shears;
using Shears.Tweens;
using UnityEngine;

namespace BigHappyBurger.Restaurants
{
    [RequireComponent(typeof(ManualDraggable))]
    public partial class SlidableWindow : MonoBehaviour
    {
        private const float RETURN_TIME = 20.0f;

        [SerializeField]
        private Range<float> xMovementRange = new();

        [SerializeField]
        private Transform windowTransform;

        [Auto]
        [AutoEvent(nameof(Draggable.DragBegan), nameof(OnDragBegan))]
        [AutoEvent(nameof(Draggable.DragEnded), nameof(OnDragEnded))]
        private ManualDraggable draggable;

        [AutoEvent(nameof(Timer.Completed), nameof(ReturnToPosition))]
        private readonly Timer returnTimer = new(RETURN_TIME);

        private readonly TweenData returnTweenData = new(1.0f, easingFunction: TweenEase.OutBounce);

        private Tween returnTween;
        private Vector3 initialPosition;
        private Vector3 initialLocalPosition;

        private void Awake()
        {
            __AutoAwake();

            draggable.PositionValidateCallback = ValidatePosition;
            initialPosition = transform.position;
            initialLocalPosition = transform.localPosition;
        }

        private void Update()
        {
            float xMovement = transform.localPosition.x - initialLocalPosition.x;
            windowTransform.localPosition = windowTransform.localPosition.With(x: xMovement);
        }

        private Vector3 ValidatePosition(Vector3 targetPosition)
        {
            float x = xMovementRange.Clamp(targetPosition.x - initialPosition.x);

            targetPosition.x = x + initialPosition.x;
            targetPosition.y = transform.position.y;
            targetPosition.z = transform.position.z;

            return targetPosition;
        }

        private void OnDragBegan()
        {
            returnTimer.Stop();
            returnTween.Dispose();
        }

        private void OnDragEnded()
        {
            returnTimer.Restart();
        }

        private void ReturnToPosition()
        {
            returnTween = transform.DoMoveLocalTween(initialLocalPosition, returnTweenData);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            var pos = initialPosition;
            var min = pos + (xMovementRange.Min * Vector3.left);
            var max = pos + (xMovementRange.Max * Vector3.right);

            Gizmos.DrawLine(min, max);
        }
    }
}
