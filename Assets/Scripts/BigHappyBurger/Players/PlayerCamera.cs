using Shears;
using Shears.Cameras;
using UnityEngine;

namespace BigHappyBurger.Players
{
    [RequireComponent(typeof(ManagedCamera))]
    public partial class PlayerCamera : MonoBehaviour
    {
        [SerializeField]
        private PlayerInput input;

        [Auto]
        private ManagedCamera cam;

        private void Awake()
        {
            __AutoAwake();

            cam.GlobalData.RotateRight = () =>
                input.PlayerActions.RotateRight.WasPressedThisFrame();
            cam.GlobalData.RotateLeft = () => input.PlayerActions.RotateLeft.WasPressedThisFrame();
            cam.GlobalData.RotateUp = () => input.PlayerActions.RotateUp.IsPressed();
            cam.GlobalData.RotateDown = () => input.PlayerActions.RotateDown.IsPressed();

            cam.Initialize();
        }
    }
}
