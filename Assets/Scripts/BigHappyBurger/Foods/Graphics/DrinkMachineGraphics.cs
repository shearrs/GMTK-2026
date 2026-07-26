using Shears;
using UnityEngine;

namespace BigHappyBurger.Foods.Graphics
{
    [RequireComponent(typeof(DrinkMachine))]
    public partial class DrinkMachineGraphics : MonoBehaviour
    {
        [SerializeField, Required(targetCollectionSize: 4), Local]
        private DrinkTypeSizeUI[] queueSprites;

        [SerializeField]
        private DrinkType grapeType;

        [SerializeField]
        private DrinkType orangeType;

        [SerializeField]
        private DrinkType waterType;

        [SerializeField]
        private DrinkType limeType;

        [SerializeField, Required, Local]
        private DrinkTypeSizeUI currentSprite;

        [SerializeField]
        private ParticleSystem orangeParticle;

        [SerializeField]
        private ParticleSystem grapeParticle;

        [SerializeField]
        private ParticleSystem waterParticle;

        [SerializeField]
        private ParticleSystem limeParticle;

        private ParticleSystem currentParticle;

        [Auto]
        [AutoEvent(nameof(DrinkMachine.DrinkQueueChanged), nameof(OnQueueChanged))]
        [AutoEvent(nameof(DrinkMachine.PourChanged), nameof(OnPourChanged))]
        private DrinkMachine machine;

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

            if(drink.Type == null)
            {
                var emission = currentParticle.emission;
                emission.rateOverTime = 0;
            } else
            {
                if(drink.Type == grapeType)
                {
                    currentParticle = grapeParticle;
                } 
                else if(drink.Type == waterType)
                {
                    currentParticle = waterParticle;
                } 
                else if(drink.Type == limeType)
                {
                    currentParticle = limeParticle;
                } 
                else if(drink.Type == orangeType)
                {
                    currentParticle = orangeParticle;
                }

                var emission = currentParticle.emission;
                emission.rateOverTime = 30;
            }
        }
    }
}
