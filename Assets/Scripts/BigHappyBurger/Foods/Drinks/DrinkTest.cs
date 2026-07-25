using UnityEngine;

namespace BigHappyBurger.Foods
{
    public class DrinkTest : MonoBehaviour
    {
        [SerializeField]
        private DrinkMachine machine;

        [SerializeField]
        private DrinkType drink;

        [ContextMenu("Queue Drink")]
        private void QueueDrink()
        {
            machine.EnqueueDrink(new(drink, Drinkable.Size.Medium));
        }
    }
}
