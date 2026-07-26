using BigHappyBurger.Interaction;
using BigHappyBurger.Customers;
using Shears;
using Shears.Detection;
using UnityEngine;

namespace BigHappyBurger.Players
{
    public class PlayerCustomerInteractor : MonoBehaviour
    {
        [SerializeField, Required, Local]
        private RayDetector3D detector;

        public bool TryToStartDialogue()
        {

            if (!detector.Detect())
                return false;


            if (detector.TryGetDetection(out CustomerDialogue customer, true))
            {
                customer.StartDialogue();
                return true;
            }

            return false;
        }
    }
}
