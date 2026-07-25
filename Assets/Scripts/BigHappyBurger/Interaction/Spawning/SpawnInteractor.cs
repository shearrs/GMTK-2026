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

            if (!detector.Detect())
                return false;

            if (detector.TryGetDetection(out ItemSpawner spawner, true))
                info = spawner.SpawnItem();
            else if (detector.TryGetDetection(out ItemSpawnContainer container, true))
                info = container.Release();

            return info.Item != null;
        }
    }
}
