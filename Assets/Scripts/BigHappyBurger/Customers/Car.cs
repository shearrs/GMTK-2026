using Shears;
using UnityEngine;

namespace BigHappyBurger.Customers
{
    public class Car : MonoBehaviour
    {
        [SerializeField, Required(targetCollectionSize: 1)]
        private Transform[] seatTransforms;

        public Transform DriversSeat => seatTransforms[0];
    }
}
