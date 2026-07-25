using UnityEngine;

namespace BigHappyBurger.Restaurants
{
    public class RestockCursor : MonoBehaviour
    {
        [SerializeField]
        private Vector3 pickOffset;

        [SerializeField]
        private Vector3 pickSize = Vector3.one;

        private Bounds Bounds => new(transform.localPosition + pickOffset, pickSize);

        public bool IsOverlapping(Bounds bounds)
        {
            return bounds.Intersects(Bounds);
        }

        public void Move(Vector2 movement, Bounds bounds)
        {
            var newPosition = transform.localPosition + pickOffset + (Vector3)movement;

            if (!bounds.Contains(newPosition))
                newPosition = bounds.ClosestPoint(newPosition);

            transform.localPosition = newPosition - pickOffset;
        }

        private void OnDrawGizmosSelected()
        {
            var matrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(
                transform.parent.position,
                transform.parent.rotation,
                Vector3.one
            );

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(Bounds.center, Bounds.size);

            Gizmos.matrix = matrix;
        }
    }
}
