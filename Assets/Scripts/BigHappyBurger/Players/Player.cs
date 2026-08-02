using Shears;
using UnityEngine;

namespace BigHappyBurger.Players
{
    public class Player : MonoBehaviour
    {
        [SerializeField, Required, Local]
        private PlayerInput input;

        [SerializeField, Required, Local]
        private PlayerInteractor interactor;

        [SerializeField, Required, Local]
        private PlayerCamera camera;

        private bool isInteractionEnabled;

        public PlayerInput Input => input;
        public PlayerInteractor Interactor => interactor;
        public PlayerCamera Camera => camera;

        private void Start()
        {
            interactor.Initialize(input);
        }

        private void Update()
        {
            if (isInteractionEnabled)
                interactor.UpdateInteraction();
        }

        public void EnableInteraction() => isInteractionEnabled = true;

        public void DisableInteraction() => isInteractionEnabled = false;
    }
}
