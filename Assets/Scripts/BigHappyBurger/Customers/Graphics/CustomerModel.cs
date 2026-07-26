using Shears;
using UnityEngine;

namespace BigHappyBurger.Customers.Graphics
{
    public class CustomerModel : MonoBehaviour
    {
        [Header("Renderers")]
        [SerializeField]
        private Renderer body;

        [SerializeField]
        private int bodyMaterialIndex = 0;

        [SerializeField]
        private int eyeMaterialIndex = 1;

        [SerializeField]
        private int mouthMaterialIndex = 2;

        [Header("Eyebrows")]
        [SerializeField, Required]
        private Transform leftEyeBrow;

        [SerializeField, Required]
        private Transform rightEyebrow;

        [SerializeField]
        private float maxEyebrowRotation = 37.0f;

        [Header("Choices")]
        [SerializeField, Required(targetCollectionSize: 3)]
        private Texture2D[] colorChoices;

        [SerializeField, Required(targetCollectionSize: 3)]
        private Texture2D[] eyeChoices;

        [SerializeField, Required(targetCollectionSize: 3)]
        private Texture2D[] mouthChoices;

        private Customer customer;
        private Quaternion initialLeftEyebrowRotation;
        private Quaternion initialRightEyebrowRotation;

        private void Awake()
        {
            initialLeftEyebrowRotation = leftEyeBrow.localRotation;
            initialRightEyebrowRotation = rightEyebrow.localRotation;
        }

        private void Update()
        {
            if (customer == null)
                return;

            var finalLeftRotation =
                Quaternion.Inverse(Quaternion.Euler(0, 0, maxEyebrowRotation))
                * initialLeftEyebrowRotation;
            var finalRightRotation =
                Quaternion.Euler(0, 0, maxEyebrowRotation) * initialRightEyebrowRotation;

            var targetLeftRotation = Quaternion.Lerp(
                initialLeftEyebrowRotation,
                finalLeftRotation,
                customer.Disatisfaction
            );
            var targetRightRotation = Quaternion.Lerp(
                initialRightEyebrowRotation,
                finalRightRotation,
                customer.Disatisfaction
            );

            leftEyeBrow.transform.localRotation = targetLeftRotation;
            rightEyebrow.transform.localRotation = targetRightRotation;
        }

        public void SetCustomer(Customer customer)
        {
            this.customer = customer;
        }

        public void Randomize()
        {
            RandomizeColor();
            RandomizeEyes();
            RandomizeMouth();
        }

        private void RandomizeColor()
        {
            if (colorChoices == null || colorChoices.Length == 0)
                return;

            body.materials[bodyMaterialIndex].mainTexture = colorChoices.Random();
        }

        private void RandomizeEyes()
        {
            if (eyeChoices == null || eyeChoices.Length == 0)
                return;

            body.materials[eyeMaterialIndex].mainTexture = eyeChoices.Random();
        }

        private void RandomizeMouth()
        {
            if (mouthChoices == null || mouthChoices.Length == 0)
                return;

            body.materials[mouthMaterialIndex].mainTexture = mouthChoices.Random();
        }
    }
}
