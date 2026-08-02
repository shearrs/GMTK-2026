using Shears;
using Shears.Signals;
using Shears.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace BigHappyBurger.GameManagement.Graphics
{
    public partial class PauseScreen : MonoBehaviour
    {
        [SerializeField, Required]
        private Players.PlayerInput input;

        [SerializeField, Required]
        private UIElement menuElement;

        [SerializeField, Required]
        private TutorialSequence tutorial;

        [SerializeField, Required]
        [AutoEvent(nameof(UIButton.Clicked), nameof(OnSkipTutorialButtonClicked))]
        private UIButton skipTutorialButton;

        [SerializeField, Required]
        [AutoEvent(nameof(UIButton.Clicked), nameof(OnRestartClicked))]
        private UIButton restartButton;

        private bool isPaused = false;

        private void OnEnable()
        {
            __AutoOnEnable();

            input.GameActions.Pause.performed += OnPauseInput;
        }

        private void OnDisable()
        {
            __AutoOnDisable();

            input.GameActions.Pause.performed -= OnPauseInput;
        }

        public void Unpause()
        {
            if (!isPaused)
                return;

            input.PlayerActions.Enable();
            menuElement.Disable();
            Time.timeScale = 1.0f;

            isPaused = false;

            SignalShuttle.Emit(new StringSignal("Unpaused"));
        }

        public void Pause()
        {
            input.PlayerActions.Disable();
            menuElement.Enable();
            Time.timeScale = 0.0f;

            isPaused = true;

            SignalShuttle.Emit(new StringSignal("Paused"));
        }

        private void OnSkipTutorialButtonClicked()
        {
            tutorial.ForceFinishTutorial();
            Unpause();
        }

        private void OnRestartClicked()
        {
            Unpause();
            SceneManager.LoadScene("Game Scene");
        }

        private void OnPauseInput(InputAction.CallbackContext _)
        {
            if (isPaused)
                Unpause();
            else
                Pause();
        }
    }
}
