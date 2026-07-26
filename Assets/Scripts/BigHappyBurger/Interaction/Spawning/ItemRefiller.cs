using System.Collections;
using Shears;
using UnityEngine;

namespace BigHappyBurger.Interaction
{
    [RequireComponent(typeof(ItemSpawnContainer))]
    public partial class ItemRefiller : MonoBehaviour
    {
        [SerializeField]
        private float delayTime = 3.0f;

        [Auto]
        [AutoEvent(nameof(ItemSpawnContainer.ItemReleased), nameof(OnItemReleased))]
        private ItemSpawnContainer spawner;

        private void OnItemReleased()
        {
            StartCoroutine(IEDelayAdd());
        }

        private IEnumerator IEDelayAdd()
        {
            yield return CoroutineUtil.WaitForSeconds(delayTime);

            if (spawner == null)
                yield break;

            spawner.AddCount(1);
        }
    }
}
