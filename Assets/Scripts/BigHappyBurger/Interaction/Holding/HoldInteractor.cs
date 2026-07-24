using Shears;
using Shears.Detection;
using UnityEngine;

namespace BigHappyBurger.Interaction
{
    public class HoldInteractor : MonoBehaviour
    {
        [SerializeField, Required, Local]
        private SphereDetector detector;

        public void TryToPlaceIntoHolder(Item item)
        {
            detector.Offset = transform.InverseTransformPoint(item.Position);

            if (!detector.Detect() || !detector.TryGetDetection(out ItemHolder holder, true))
                return;

            holder.Hold(item);
        }
    }
}
