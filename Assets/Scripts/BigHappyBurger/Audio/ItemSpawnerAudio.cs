using BigHappyBurger.Interaction;
using Shears;
using UnityEngine;

namespace BigHappyBurger.Audio
{
    [RequireComponent(typeof(ItemSpawner), typeof(AudioSource))]
    public partial class ItemSpawnerAudio : MonoBehaviour
    {
        [SerializeField]
        private Range<float> pitchRange = new(0.85f, 1.15f);

        [Auto]
        [AutoEvent(nameof(ItemSpawner.ItemSpawned), nameof(OnItemSpawned))]
        private ItemSpawner spawner;

        [Auto]
        private AudioSource audioSource;

        private void OnItemSpawned(Item _)
        {
            audioSource.PlayWithRange(pitchRange.Min, pitchRange.Max);
        }
    }
}
