using Shears;
using Shears.Detection;
using UnityEngine;

namespace BigHappyBurger.Interaction
{
    public class HoldInteractor : MonoBehaviour
    {
        [SerializeField, Required, Local]
        private SphereDetector detector;

        public void TryToPlaceIntoHolder(Item item, Vector3 offset)
        {
            detector.Offset = transform.InverseTransformPoint(item.Position + offset);

            if (!detector.Detect() || !detector.TryGetDetection(out ItemHolder holder, true))
                return;

            holder.Hold(item);
        }
    }
}
