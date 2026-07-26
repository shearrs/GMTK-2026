using BigHappyBurger.Foods;
using BigHappyBurger.Interaction;
using BigHappyBurger.Restaurants;
using TMPro;
using UnityEngine;

namespace BigHappyBurger.Restaurant.Graphics
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
                countdownText.text = Mathf
                    .RoundToInt(slot.Timer.Time - slot.Timer.CurrentTime)
                    .ToString();
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
