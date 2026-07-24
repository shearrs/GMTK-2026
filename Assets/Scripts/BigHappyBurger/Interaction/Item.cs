using System;
using System.Collections.Generic;
using Shears;
using Shears.Logging;
using Shears.Tweens;
using UnityEngine;

namespace BigHappyBurger.Interaction
{
    [SelectionBase]
    public class Item : MonoBehaviour, ISHLoggable
    {
        [field: Header("Logging")]
        [field: SerializeField]
        public SHLogLevels LogLevels { get; set; } = SHLogUtil.Default;

        [Header("Item")]
        [field: SerializeField, ReadOnly]
        public string ID { get; private set; }

        [SerializeField, Local]
        private Rigidbody rigidbody;

        [SerializeField, Local]
        private Collider[] colliders;

        [SerializeField, Local]
        private Draggable draggable;

        [SerializeField, Local]
        private Holdable holdable;

        private Tween scaleTween;

        public IReadOnlyList<Collider> Colliders => colliders;
        public Vector3 Position => rigidbody != null ? rigidbody.position : transform.position;
        public Quaternion Rotation => rigidbody != null ? rigidbody.rotation : transform.rotation;
        public Transform Parent => transform.parent;
        public bool IsDraggable => draggable != null && (!IsBeingHeld || holdable.CanBeReleased());
        public bool IsBeingHeld => holdable != null && holdable.IsBeingHeld;
        public bool IsBeingDragged => IsDraggable && draggable.IsBeingDragged;
        public bool IsFlipped { get; internal set; }

        private void Reset()
        {
            ID = Guid.NewGuid().ToString();
            colliders = GetComponentsInChildren<Collider>();

            TryGetComponent(out rigidbody);
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(ID))
                ID = Guid.NewGuid().ToString();
        }

        public void EnableCollision()
        {
            foreach (var collider in colliders)
                collider.enabled = true;
        }

        public void DisableCollision()
        {
            foreach (var collider in colliders)
                collider.enabled = false;
        }

        public void SetScale(Vector3 scale)
        {
            scaleTween.Dispose();
            transform.localScale = scale;
        }

        public void TweenScale(Vector3 scale, ITweenData data = null)
        {
            scaleTween.Dispose();
            scaleTween = transform.DoScaleLocalTween(scale, data);
        }

        internal void OnDragBegin()
        {
            if (IsBeingHeld)
            {
                if (!holdable.Release())
                {
                    this.Log($"Could not release from holdable!", SHLogLevels.Error);
                    return;
                }
            }

            draggable.OnDragBegin();
        }

        internal void OnDragEnd() => draggable.OnDragEnd();

        internal void SetDragPosition(Vector3 position) => draggable.SetTargetPosition(position);

        internal void SetDragRotation(Quaternion rotation) => draggable.SetTargetRotation(rotation);

        internal void OnHoldBegin(ItemHolder holder) => holdable.OnHoldBegin(holder);

        internal void OnHoldEnd() => holdable.OnHoldEnd();

        internal void SetParent(Transform parent)
        {
            if (rigidbody != null)
                rigidbody.transform.SetParent(parent);
            else
                transform.SetParent(parent);
        }

        internal void SetLocalPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            if (rigidbody == null || rigidbody.isKinematic)
                transform.SetLocalPositionAndRotation(position, rotation);
            else
            {
                this.Log(
                    $"Should not be trying to set the transform of an active {nameof(Rigidbody)}.",
                    SHLogLevels.Warning
                );
                return;
            }
        }

        internal void SetRigidbody(Rigidbody rigidbody) => this.rigidbody = rigidbody;

        internal void SetDraggable(Draggable draggable) => this.draggable = draggable;

        internal void SetHoldable(Holdable holdable) => this.holdable = holdable;
    }
}
