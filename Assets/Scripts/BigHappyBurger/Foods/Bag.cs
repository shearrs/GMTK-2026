using System;
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
        [field: SerializeField]
        public bool IsHappyBox { get; private set; }

        [SerializeField, Required(targetCollectionSize: 1), Local]
        private FoodHolder[] holders;

        [SerializeField]
        private Vector3 holdScale = new(0.5f, 0.5f, 0.5f);

        [SerializeField]
        private TweenData scaleTween = new(0.5f, easingFunction: TweenEase.InOutBack);

        [AutoEvent(nameof(Timer.Completed), nameof(OnEmptyTimerCompleted))]
        private readonly Timer emptyTimer = new(0.5f);

        [AutoEvent(nameof(Timer.Completed), nameof(Unlock))]
        private readonly Timer unlockTimer = new(1.0f);

        private Item item;
        private bool isFlipped = false;

        public Item Item => this.LazyGet(ref item);
        public string ID => Item.ID;

        public event Action<Food> FoodHeld;

        private void Update()
        {
            if (holders.Length == 0)
                return;

            if (!isFlipped && Item.IsFlipped)
            {
                foreach (var holder in holders)
                    holder.Lock();

                unlockTimer.Stop();
                emptyTimer.Restart();
            }
            else if (isFlipped && !Item.IsFlipped)
            {
                emptyTimer.Stop();
                unlockTimer.Restart();
            }

            isFlipped = Item.IsFlipped;
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

        private void HoldFood(Food food)
        {
            foreach (var holder in holders)
            {
                if (holder.Hold(food))
                {
                    food.Item.TweenScale(holdScale, scaleTween);
                    holder.Lock();

                    FoodHeld?.Invoke(food);
                    return;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var food = other.GetComponentInParent<Food>(true);

            if (food == null || isFlipped)
                return;

            HoldFood(food);
        }
    }
}
