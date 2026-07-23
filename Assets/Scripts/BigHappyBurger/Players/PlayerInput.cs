using UnityEngine;

namespace BigHappyBurger.Players
{
    public class PlayerInput : MonoBehaviour
    {
        private PlayerInputActions actions;

        public PlayerInputActions.PlayerActions PlayerActions => actions.Player;

        private void Awake()
        {
            actions = new();

            Enable();
        }

        public void Enable()
        {
            actions.Enable();
        }

        public void Disable()
        {
            actions.Disable();
        }
    }
}
