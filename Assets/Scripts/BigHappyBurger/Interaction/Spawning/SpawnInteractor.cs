using Shears;
using Shears.Detection;
using UnityEngine;

namespace BigHappyBurger.Interaction
{
    public class SpawnInteractor : MonoBehaviour
    {
        [SerializeField, Required, Local]
        private RayDetector3D detector;

        public bool TryToSpawnItem(out ItemSpawner.ItemSpawnInfo info)
        {
            info = default;

            if (!detector.Detect() || !detector.TryGetDetection(out ItemSpawner spawner, true))
                return false;

            info = spawner.SpawnItem();

            return info.Item != null;
        }
    }
}
