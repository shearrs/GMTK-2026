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

        [SerializeField, Required, Local]
        private SpawnInteractor spawnInteractor;

        [SerializeField, Required, Local]
        private PlayerCustomerInteractor customerInteractor;

        [Header("Settings")]
        [SerializeField]
        private float itemScrollSensitivity = 0.1f;

        private InputAction interactInput;
        private InputAction flipInput;
        private InputAction scrollItemInput;

        public void Initialize(PlayerInput input)
        {
            interactInput = input.PlayerActions.Interact;
            flipInput = input.PlayerActions.FlipItem;
            scrollItemInput = input.PlayerActions.ScrollItem;
        }

        public void UpdateInteraction()
        {
            if (scrollItemInput.WasPressedThisFrame() && dragInteractor.Item != null)
                dragInteractor.ChangePlaneDistance(
                    scrollItemInput.ReadValue<Vector2>().y * itemScrollSensitivity
                );

            var dragInfo = dragInteractor.UpdateDragging(
                interactInput.IsPressed(),
                flipInput.IsPressed()
            );

            if (dragInfo.ReleasedItem && dragInfo.Item.IsHoldable)
                holdInteractor.TryToPlaceIntoHolder(dragInfo.Item, dragInfo.DragOffset);

            if (dragInteractor.Item == null && interactInput.WasPressedThisFrame())
            {
                if (spawnInteractor.TryToSpawnItem(out var info))
                {
                    dragInteractor.BeginDragging(
                        info.Item,
                        info.PointerOffset,
                        info.PlaneDistanceOffset
                    );

                    dragInteractor.UpdateDragging(interactInput.IsPressed(), flipInput.IsPressed());
                } else if (customerInteractor.TryToStartDialogue())
                {
                    //turn off turning of player
                }
            }
        }
    }
}
