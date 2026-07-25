using Shears;
using UnityEngine;

namespace BigHappyBurger.Foods.Graphics
{
    [RequireComponent(typeof(DrinkMachine))]
    public partial class DrinkMachineGraphics : MonoBehaviour
    {
        [SerializeField, Required(targetCollectionSize: 4), Local]
        private DrinkTypeSizeUI[] queueSprites;

        [SerializeField, Required, Local]
        private DrinkTypeSizeUI currentSprite;

        [Auto]
        [AutoEvent(nameof(DrinkMachine.DrinkQueueChanged), nameof(OnQueueChanged))]
        [AutoEvent(nameof(DrinkMachine.PourChanged), nameof(OnPourChanged))]
        private DrinkMachine machine;

        // also need to play particles here

        private void OnQueueChanged()
        {
            for (int i = 0; i < queueSprites.Length; i++)
            {
                if (i >= machine.DrinkQueue.Count)
                    queueSprites[i].SetDrink(DrinkTypeSize.Empty);
                else
                {
                    var drink = machine.DrinkQueue[i];

                    queueSprites[i].SetDrink(drink);
                }
            }
        }

        private void OnPourChanged(DrinkTypeSize drink)
        {
            currentSprite.SetDrink(drink);
        }
    }
}
