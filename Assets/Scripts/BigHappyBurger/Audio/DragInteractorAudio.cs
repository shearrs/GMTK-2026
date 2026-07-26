using BigHappyBurger.Interaction;
using Shears;
using UnityEngine;

namespace BigHappyBurger.Audio
{
    [RequireComponent(typeof(DragInteractor), typeof(AudioSource))]
    public partial class DragInteractorAudio : MonoBehaviour
    {
        [Auto]
        [AutoEvent(nameof(DragInteractor.DragBegan), nameof(OnDragBegan))]
        private DragInteractor interactor;

        [Auto]
        private AudioSource audioSource;

        private void OnDragBegan()
        {
            audioSource.PlayWithRange();
        }
    }
}
