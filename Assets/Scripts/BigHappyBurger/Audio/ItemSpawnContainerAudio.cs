using BigHappyBurger.Interaction;
using Shears;
using UnityEngine;

namespace BigHappyBurger.Audio
{
    [RequireComponent(typeof(ItemSpawnContainer), typeof(AudioSource))]
    public partial class ItemSpawnContainerAudio : MonoBehaviour
    {
        [Auto]
        [AutoEvent(nameof(ItemSpawnContainer.ItemReleased), nameof(OnItemSpawned))]
        private ItemSpawnContainer spawner;

        [Auto]
        private AudioSource audioSource;

        private void OnItemSpawned()
        {
            audioSource.PlayWithRange();
        }
    }
}
