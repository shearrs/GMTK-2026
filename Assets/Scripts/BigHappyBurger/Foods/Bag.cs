using System.Collections.Generic;
using BigHappyBurger.Interaction;
using Shears;
using Shears.Tweens;
using UnityEngine;

namespace BigHappyBurger.Foods
{
    [RequireComponent(typeof(Item))]
    public partial class Bag : MonoBehaviour
    {
        [SerializeField, Required(targetCollectionSize: 1), Local]
        private FoodHolder[] holders;

        [SerializeField]
        private Vector3 holdScale = new(0.5f, 0.5f, 0.5f);

        [SerializeField]
        private TweenData scaleTween = new(0.5f, easingFunction: TweenEase.InOutBack);

        [Auto]
        private Item item;

        [AutoEvent(nameof(Timer.Completed), nameof(OnEmptyTimerCompleted))]
        private readonly Timer emptyTimer = new(0.5f);

        [AutoEvent(nameof(Timer.Completed), nameof(Unlock))]
        private readonly Timer unlockTimer = new(1.0f);

        private bool isFlipped = false;

        private void Update()
        {
            if (holders.Length == 0)
                return;

            if (!isFlipped && item.IsFlipped)
            {
                foreach (var holder in holders)
                    holder.Lock();

                unlockTimer.Stop();
                emptyTimer.Restart();
            }
            else if (isFlipped && !item.IsFlipped)
            {
                emptyTimer.Stop();
                unlockTimer.Restart();
            }

            isFlipped = item.IsFlipped;
        }

        public void EmptyFirst()
        {
            foreach (var holder in holders)
            {
                var item = holder.Item;

                if (item != null)
                {
                    holder.Unlock();
                    holder.Release();
                    holder.Lock();
                    item.TweenScale(Vector3.one, scaleTween);
                    break;
                }
            }
        }

        public void GetFood(List<Food> food)
        {
            food.Clear();

            foreach (var holder in holders)
            {
                if (holder.Item != null && holder.Item.TryGetComponent(out Food bagFood))
                    food.Add(bagFood);
            }
        }

        private void Unlock()
        {
            foreach (var holder in holders)
                holder.Unlock();
        }

        private void OnEmptyTimerCompleted()
        {
            EmptyFirst();

            foreach (var holder in holders)
            {
                if (holder.Item != null)
                {
                    emptyTimer.Start();
                    break;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var food = other.GetComponentInParent<Food>(true);

            if (food == null || isFlipped)
                return;

            foreach (var holder in holders)
            {
                if (holder.Hold(food))
                {
                    food.Item.TweenScale(holdScale, scaleTween);
                    holder.Lock();
                    return;
                }
            }
        }
    }
}
