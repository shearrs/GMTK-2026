using BigHappyBurger.Foods;
using BigHappyBurger.Interaction;
using BigHappyBurger.Restaurants;
using TMPro;
using UnityEngine;

namespace BigHappyBurger.Restaurants.Graphics
{
    public class FoodTimerGraphics : MonoBehaviour
    {
        [SerializeField]
        private TextMeshPro countdownText;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        private Chef.ChefSlot slot;

        private void Awake()
        {
            countdownText.text = string.Empty;
            spriteRenderer.sprite = null;
        }

        private void OnDestroy()
        {
            if (slot != null)
                slot.FoodChanged -= OnFoodChanged;
        }

        private void Update()
        {
            if (slot.IsCooking)
            {
                if (slot.HasTimer)
                    countdownText.text = Mathf
                        .RoundToInt(slot.Timer.Time - slot.Timer.CurrentTime)
                        .ToString();
                else
                    countdownText.text = "...";
            }
        }

        public void SetSlot(Chef.ChefSlot slot)
        {
            this.slot = slot;
            slot.FoodChanged += OnFoodChanged;
        }

        private void OnFoodChanged(Cookable food)
        {
            if (food == null)
            {
                spriteRenderer.sprite = null;
                countdownText.text = string.Empty;
            }
            else if (food.TryGetComponent(out Item item))
                spriteRenderer.sprite = item.Sprite;
        }
    }
}
