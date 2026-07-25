using Shears.Tweens;
using UnityEngine;

namespace BigHappyBurger.Restaurants
{
    public class RestockButton : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer sprite;

        [SerializeField]
        private Color hoverColor = new(0.85f, 0.85f, 0.85f, 1.0f);

        [SerializeField]
        private Vector2 pickOffset;

        [SerializeField]
        private Vector2 pickSize;

        private readonly TweenData tweenData = new(0.1f);
        private Color originalColor;
        private Tween tween;

        public Bounds Bounds => new(transform.localPosition + (Vector3)pickOffset, pickSize);

        private void Awake()
        {
            originalColor = sprite.color;
        }

        public bool Intersects(Bounds bounds)
        {
            return bounds.Intersects(Bounds);
        }

        public void Hover()
        {
            tween.Dispose();
            tween = sprite.DoColorTween(hoverColor * originalColor, tweenData);
        }

        public void Unhover()
        {
            tween.Dispose();
            tween = sprite.DoColorTween(originalColor, tweenData);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            var matrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(
                transform.parent.position,
                transform.parent.rotation,
                Vector3.one
            );

            Gizmos.DrawWireCube(Bounds.center, Bounds.size);

            Gizmos.matrix = matrix;
        }
    }
}
