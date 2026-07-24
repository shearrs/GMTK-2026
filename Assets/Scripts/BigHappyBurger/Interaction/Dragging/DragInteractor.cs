using Shears;
using Shears.Detection;
using Shears.Input;
using UnityEngine;

namespace BigHappyBurger.Interaction
{
    public class DragInteractor : MonoBehaviour
    {
        [SerializeField, Required, Local]
        private RayDetector3D detector;

        [field: SerializeField]
        public Range<float> PlaneDistanceRange = new(1, 3);

        [SerializeField]
        private float planeDistance = 2;

        private Vector3 dragOffset;

        public Item Item { get; private set; }

        public void UpdateInteraction(bool dragInput)
        {
            if (Item != null)
            {
                if (dragInput)
                {
                    UpdateDrag();
                    return;
                }
                else
                {
                    Release();
                    return;
                }
            }

            if (!dragInput || !detector.Detect())
                return;

            if (!detector.TryGetDetection(out Item item, true) || !item.IsDraggable)
                return;

            var hit = detector.GetHit(0);
            var cam = Camera.main;
            var flatCamRotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);

            dragOffset =
                Quaternion.Inverse(flatCamRotation) * (item.transform.position - hit.point);
            dragOffset.z = 0;

            Item = item;
            Item.OnDragBegin();
            UpdateDrag();
        }

        public void Release()
        {
            if (Item == null)
                return;

            Item.OnDragEnd();
            Item = null;
        }

        public void ChangePlaneDistance(float change)
        {
            planeDistance = PlaneDistanceRange.Clamp(planeDistance + change);
        }

        private void UpdateDrag()
        {
            var pointerPos = ManagedPointer.Current.Position;

            if (float.IsNaN(pointerPos.x) || float.IsNaN(pointerPos.y))
                return;

            var cam = Camera.main;
            var planeNormal = -cam.transform.forward.With(y: 0).normalized;
            var planePosition = cam.transform.position + -planeNormal * planeDistance;
            var dragPosition = cam.ScreenPointToPlanePosition(
                pointerPos,
                planeNormal,
                planePosition
            );

            var flatCamRotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);

            Debug.DrawRay(planePosition, planeNormal, Color.blue);
            Debug.DrawLine(planePosition, dragPosition, Color.yellow);
            Debug.DrawLine(
                dragPosition,
                dragPosition + flatCamRotation * dragOffset,
                Color.magenta
            );

            // in case we decide that an offset is better
            dragPosition += flatCamRotation * dragOffset;

            Item.SetDragPosition(dragPosition);
            Item.SetDragRotation(Quaternion.LookRotation(cam.transform.forward)); // probably to be changed later)
        }
    }
}
