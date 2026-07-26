using System;
using Shears;
using Shears.Detection;
using Shears.Input;
using Shears.Logging;
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

        public event Action DragBegan;

        public readonly struct UpdateInfo
        {
            public static readonly UpdateInfo Empty = new();

            public Item Item { get; }
            public Vector3 DragOffset { get; }
            public bool ReleasedItem { get; }

            public UpdateInfo(Item item, Vector3 dragOffset, bool releasedItem = false)
            {
                Item = item;
                DragOffset = dragOffset;
                ReleasedItem = releasedItem;
            }
        }

        public UpdateInfo UpdateDragging(bool dragInput, bool flipInput)
        {
            var cam = Camera.main;
            var flatCamRotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);

            if (Item != null)
            {
                var offset = -(flatCamRotation * dragOffset);
                Item.IsFlipped = flipInput;

                if (dragInput)
                {
                    UpdateDrag(flipInput);
                    return new(Item, offset);
                }
                else
                {
                    var releasedItem = Item;
                    Release();
                    return new(releasedItem, offset, true);
                }
            }

            if (!dragInput || !detector.Detect())
                return UpdateInfo.Empty;

            if (!detector.TryGetDetection(out Item item, true) || !item.IsDraggable)
                return UpdateInfo.Empty;

            var hit = detector.GetHit(0);

            dragOffset = Quaternion.Inverse(item.Rotation) * (item.Position - hit.point);
            dragOffset.z = 0;

            var localItemPosition = cam.transform.InverseTransformPoint(item.Position);
            float distance = localItemPosition.z;

            Item = item;

            planeDistance = PlaneDistanceRange.Clamp(distance + item.ExtraPlaneDistance);

            Item.OnDragBegin();
            UpdateDrag(flipInput);
            //CursorManager.SetCursorVisibility(false);

            DragBegan?.Invoke();

            return new(Item, -(flatCamRotation * dragOffset));
        }

        public void BeginDragging(Item item, Vector3 offset, float planeOffset)
        {
            if (Item != null)
            {
                SHLogger.Log(
                    $"Tried to begin dragging, but already had an item!",
                    SHLogLevels.Error
                );
                return;
            }

            var cam = Camera.main;
            var localItemPosition = cam.transform.InverseTransformPoint(item.Position);
            float distance = localItemPosition.z;
            planeDistance = PlaneDistanceRange.Clamp(distance + planeOffset);

            dragOffset = Quaternion.Inverse(item.Rotation) * offset;
            Item = item;
            Item.OnDragBegin();

            DragBegan?.Invoke();
        }

        public void Release()
        {
            if (Item == null)
                return;

            //CursorManager.SetCursorVisibility(true);
            Item.OnDragEnd();
            Item = null;
        }

        public void ChangePlaneDistance(float change)
        {
            planeDistance = PlaneDistanceRange.Clamp(planeDistance + change);
        }

        private void UpdateDrag(bool flipInput)
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

            var dragRotation = Quaternion.LookRotation(-cam.transform.forward);

            Debug.DrawRay(planePosition, planeNormal, Color.blue);
            Debug.DrawLine(planePosition, dragPosition, Color.yellow);

            if (flipInput)
                dragRotation = Quaternion.Euler(180.0f, 0, 0) * dragRotation;

            dragPosition += dragRotation * dragOffset;

            Item.SetDragPosition(dragPosition);
            Item.SetDragRotation(dragRotation);
        }
    }
}
