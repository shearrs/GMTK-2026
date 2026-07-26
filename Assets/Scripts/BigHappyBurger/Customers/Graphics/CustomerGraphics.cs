using Shears;
using Shears.Logging;
using UnityEngine;

namespace BigHappyBurger.Customers.Graphics
{
    [RequireComponent(typeof(Customer)), DisallowMultipleComponent]
    public partial class CustomerGraphics : MonoBehaviour
    {
        [SerializeField]
        private CustomerModel[] modelChoices;

        [Auto]
        [AutoEvent(nameof(Customer.Spawned), nameof(Randomize))]
        private Customer customer;

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

            var model = modelChoices.Random();
            var modelInstance = Instantiate(model, transform);
            modelInstance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            modelInstance.SetCustomer(customer);
            modelInstance.Randomize();
        }
    }
}
