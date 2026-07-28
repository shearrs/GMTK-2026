using System.Collections.Generic;
using BigHappyBurger.Foods;
using BigHappyBurger.Interaction;
using BigHappyBurger.Restaurants;
using Shears;
using UnityEngine;

namespace BigHappyBurger.Restaurants.Graphics
{
    [RequireComponent(typeof(Chef))]
    public partial class ChefGraphics : MonoBehaviour
    {
        [SerializeField, Required, Local]
        [AutoEvent(nameof(FoodConveyor.QueueChanged), nameof(OnQueueChanged))]
        private FoodConveyor foodConveyor;

        [SerializeField, Required(targetCollectionSize: 4), Local]
        private List<FoodTimerGraphics> timerGraphics = new();

        [SerializeField, Required(targetCollectionSize: 4), Local]
        private List<SpriteRenderer> queueSprites = new();

        [Auto]
        private Chef chef;

        private void Awake()
        {
            __AutoAwake();

            for (int i = 0; i < chef.Slots.Count; i++)
                timerGraphics[i].SetSlot(chef.Slots[i]);

            foreach (var sprite in queueSprites)
                sprite.sprite = null;
        }

        private void OnQueueChanged(IReadOnlyList<Food> queue)
        {
            foreach (var sprite in queueSprites)
                sprite.sprite = null;

            for (int i = 0; i < queue.Count; i++)
            {
                if (i >= queueSprites.Count)
                    return;

                if (queue[i] == null)
                    continue;
                else if (queue[i].TryGetComponent(out Item item))
                    queueSprites[i].sprite = item.Sprite;
            }
        }
    }
}
