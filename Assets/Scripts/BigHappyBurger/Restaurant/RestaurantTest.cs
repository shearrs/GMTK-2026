using BigHappyBurger.Foods;
using UnityEngine;

namespace BigHappyBurger.Restaurants
{
    public class RestaurantTest : MonoBehaviour
    {
        [SerializeField]
        private Restaurant restaurant;

        [SerializeField]
        private Food[] foodToOrder;

        [ContextMenu("Add Order")]
        private void AddOrder()
        {
            var order = new Order(foodToOrder);

            restaurant.AddOrder(order);
        }
    }
}
