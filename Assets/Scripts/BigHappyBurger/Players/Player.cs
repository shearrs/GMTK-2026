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

        private void Start()
        {
            interactor.Initialize(input);
        }

        private void Update()
        {
            interactor.UpdateInteraction();
        }
    }
}
