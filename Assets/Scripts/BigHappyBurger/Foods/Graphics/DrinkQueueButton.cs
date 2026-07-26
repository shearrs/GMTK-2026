using Shears;
using Shears.UI;
using UnityEngine;

namespace BigHappyBurger.Foods.Graphics
{
    [RequireComponent(typeof(UIButton))]
    public partial class DrinkQueueButton : MonoBehaviour
    {
        [SerializeField]
        private DrinkMachine machine;

        [SerializeField]
        private DrinkType type;

        [SerializeField]
        private Drinkable.Size size;

        [Auto]
        [AutoEvent(nameof(UIButton.Clicked), nameof(OnClicked))]
        private UIButton button;

        private void OnClicked()
        {
            if (machine.DrinkQueue.Count >= 4)
                return;

            machine.EnqueueDrink(new(type, size));
        }
    }
}
