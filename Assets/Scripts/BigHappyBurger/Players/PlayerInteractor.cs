using BigHappyBurger.Interaction;
using Shears;
using UnityEngine;

namespace BigHappyBurger.Players
{
    public class PlayerInteractor : MonoBehaviour
    {
        [Header("Interactors")]
        [SerializeField, Required, Local]
        private DragInteractor dragInteractor;

        [Header("Settings")]
        [SerializeField]
        private float itemScrollSensitivity = 0.1f;

        private PlayerInput input;

        public void Initialize(PlayerInput input)
        {
            this.input = input;
        }

        public void UpdateInteraction()
        {
            if (input.PlayerActions.ScrollItem.WasPressedThisFrame() && dragInteractor.Item != null)
                dragInteractor.ChangePlaneDistance(
                    input.PlayerActions.ScrollItem.ReadValue<Vector2>().y * itemScrollSensitivity
                );

            dragInteractor.UpdateInteraction(input.PlayerActions.Interact.IsPressed());
        }
    }
}
