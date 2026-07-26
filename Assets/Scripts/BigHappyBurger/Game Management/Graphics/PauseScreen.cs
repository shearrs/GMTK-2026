using BigHappyBurger.Players;
using Shears;
using Shears.UI;
using UnityEngine;

namespace BigHappyBurger.GameManagement.Graphics
{
    public class PauseScreen : MonoBehaviour
    {
        [SerializeField, Required]
        private PlayerInput input;

        [SerializeField, Required]
        private UIElement menuElement;

        private bool isPaused = false;

        private void Update()
        {
            if (input.PlayerActions.Pause.WasPressedThisFrame())
                Pause();
        }

        public void Unpause()
        {
            if (!isPaused)
                return;

            input.Enable();
            menuElement.Disable();
            Time.timeScale = 1.0f;
        }

        public void Pause()
        {
            input.Disable();
            menuElement.Enable();
            Time.timeScale = 0.0f;

            isPaused = true;
        }
    }
}
