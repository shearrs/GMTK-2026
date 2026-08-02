using System.Collections;
using Shears;
using Shears.Logging;
using UnityEngine;

namespace BigHappyBurger.Customers.Graphics
{
    [RequireComponent(typeof(Customer), typeof(AudioSource)), DisallowMultipleComponent]
    public partial class CustomerGraphics : MonoBehaviour
    {
        [SerializeField]
        private CustomerModel[] modelChoices;

        [SerializeField]
        private AudioClip driveInClip;

        [SerializeField]
        private AudioClip driveOutClip;

        [Auto]
        private AudioSource audioSource;

        [Auto]
        [AutoEvent(nameof(Customer.Spawned), nameof(Randomize))]
        [AutoEvent(nameof(Customer.BeganExiting), nameof(OnExit))]
        [AutoEvent(nameof(Customer.BeganTalking), nameof(OnBeganTalking))]
        private Customer customer;

        private CustomerModel model;

        public void Randomize()
        {
            if (modelChoices == null || modelChoices.Length == 0)
            {
                SHLogger.Log(
                    $"{nameof(CustomerGraphics)} has no models to choose from!",
                    SHLogLevels.Error
                );
                return;
            }

            model = modelChoices.Random();
            var modelInstance = Instantiate(model, transform);
            modelInstance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            modelInstance.SetCustomer(customer);
            modelInstance.Randomize();

            CoroutineUtil.DoAfter(
                () =>
                {
                    audioSource.volume = 0.25f;
                    audioSource.PlayOneShot(driveInClip);
                },
                1.0f,
                this
            );
        }

        private void OnExit()
        {
            CoroutineUtil.DoAfter(
                () =>
                {
                    audioSource.volume = 0.15f;
                    audioSource.PlayOneShot(driveOutClip);
                },
                1.0f,
                this
            );
        }

        private void OnBeganTalking()
        {
            audioSource.volume = 0.65f;
            audioSource.clip = model.GetRandomTalkingSound();
            audioSource.PlayWithRange();
        }
    }
}
