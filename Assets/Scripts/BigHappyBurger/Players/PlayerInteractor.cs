using BigHappyBurger.Interaction;
using Shears;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BigHappyBurger.Players
{
    public class PlayerInteractor : MonoBehaviour
    {
        [Header("Interactors")]
        [SerializeField, Required, Local]
        private DragInteractor dragInteractor;

        [SerializeField, Required, Local]
        private HoldInteractor holdInteractor;

        [Header("Settings")]
        [SerializeField]
        private float itemScrollSensitivity = 0.1f;

        private PlayerInput input;
        private InputAction scrollItemInput;

        public void Initialize(PlayerInput input)
        {
            this.input = input;
            scrollItemInput = input.PlayerActions.ScrollItem;
        }

        public void UpdateInteraction()
        {
            if (scrollItemInput.WasPressedThisFrame() && dragInteractor.Item != null)
                dragInteractor.ChangePlaneDistance(
                    scrollItemInput.ReadValue<Vector2>().y * itemScrollSensitivity
                );

            var dragInfo = dragInteractor.UpdateDragging(input.PlayerActions.Interact.IsPressed());

            if (dragInfo.ReleasedItem)
                holdInteractor.TryToPlaceIntoHolder(dragInfo.Item);
        }
    }
}
