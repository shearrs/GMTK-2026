using TMPro;
using UnityEngine;

namespace BigHappyBurger.Foods.Graphics
{
    public class DrinkTypeSizeUI : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer drinkSprite;

        [SerializeField]
        private TextMeshPro sizeText;

        private void Awake()
        {
            drinkSprite.sprite = null;
            sizeText.text = string.Empty;
        }

        public void SetDrink(DrinkTypeSize drink)
        {
            if (drink.Type == null)
            {
                drinkSprite.sprite = null;
                sizeText.text = string.Empty;
            }
            else
            {
                drinkSprite.sprite = drink.Type.Sprite;

                switch (drink.Size)
                {
                    case Drinkable.Size.Small:
                        sizeText.text = "S";
                        break;
                    case Drinkable.Size.Medium:
                        sizeText.text = "M";
                        break;
                    case Drinkable.Size.Large:
                        sizeText.text = "L";
                        break;
                }
            }
        }
    }
}
