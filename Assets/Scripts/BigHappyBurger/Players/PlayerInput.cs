using Shears;
using UnityEngine;

namespace BigHappyBurger.Players
{
    public class PlayerInput : MonoBehaviour
    {
        private PlayerInputActions actions;
        private PlayerInputActions Actions
        {
            get
            {
                actions ??= new();

                return actions;
            }
        }

        public PlayerInputActions.PlayerActions PlayerActions => Actions.Player;
        public PlayerInputActions.GameActions GameActions => Actions.Game;

        private void Awake()
        {
            Enable();
        }

        public void Enable()
        {
            Actions.Enable();
        }

        public void Disable()
        {
            Actions.Disable();
        }
    }
}
