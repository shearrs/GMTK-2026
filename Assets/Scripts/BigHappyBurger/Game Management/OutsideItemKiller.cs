using BigHappyBurger.Interaction;
using Shears;
using UnityEngine;

namespace BigHappyBurger.GameManagement
{
    public class OutsideItemKiller : MonoBehaviour
    {
        private const float KILL_TIME = 30.0f;

        private void OnTriggerEnter(Collider other)
        {
            var item = other.GetComponentInParent<Item>();

            if (item == null)
                return;

            var timer = TimerPool.Get();
            timer.Start(KILL_TIME);

            timer.Completed += () =>
            {
                if (item != null)
                    Destroy(item.gameObject);

                TimerPool.Release(timer);
            };
        }
    }
}
