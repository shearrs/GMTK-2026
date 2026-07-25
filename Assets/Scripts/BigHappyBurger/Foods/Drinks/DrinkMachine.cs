using System;
using System.Collections;
using System.Collections.Generic;
using Shears;
using Shears.Beziers;
using Shears.Logging;
using Shears.Tweens;
using UnityEngine;

namespace BigHappyBurger.Foods
{
    public class DrinkMachine : MonoBehaviour
    {
        private const float SMALL_POUR_TIME = 5.0f;
        private const float MEDIUM_POUR_TIME = 8.0f;
        private const float LARGE_POUR_TIME = 10.0f;

        [SerializeField, Required, Local]
        private Bezier bezier;

        [SerializeField, Required(targetCollectionSize: 4), Local]
        private List<FoodHolder> holderQueue = new();

        private readonly List<DrinkTypeSize> drinkQueue = new();
        private readonly Timer pourTimer = new();
        private bool isPouring = false;

        public IReadOnlyList<DrinkTypeSize> DrinkQueue => drinkQueue;

        public event Action DrinkQueueChanged;
        public event Action<DrinkTypeSize> PourChanged;

        private void Awake()
        {
            foreach (var holder in holderQueue)
                holder.ItemChanged += OnFoodHolderItemChanged;
        }

        private void OnDestroy()
        {
            foreach (var holder in holderQueue)
                holder.ItemChanged -= OnFoodHolderItemChanged;
        }

        public void EnqueueDrink(DrinkTypeSize drink)
        {
            drinkQueue.Add(drink);

            DrinkQueueChanged?.Invoke();

            TryPour();
        }

        private void OnFoodHolderItemChanged(FoodHolder holder)
        {
            TryPour();
        }

        private void TryPour()
        {
            if (drinkQueue.Count == 0 || isPouring)
                return;

            var firstItem = holderQueue[0].Item;

            if (firstItem == null)
                return;

            if (firstItem.TryGetComponent(out Drinkable drinkable))
            {
                if (drinkable.HasLiquid || drinkable.HasLid)
                    return;
            }

            StartCoroutine(IEPour());
        }

        private IEnumerator IEPour()
        {
            var holder = holderQueue[0];

            if (!holder.Item.TryGetComponent(out Drinkable drinkable))
            {
                SHLogger.Log($"Drink Machine received non drinkable item!", SHLogLevels.Error);
                yield break;
            }

            isPouring = true;
            drinkable.DisableLidHolder();
            pourTimer.Start(GetPourTime(drinkable.DrinkSize));

            var drinkOrder = drinkQueue[0];
            drinkQueue.RemoveAt(0);
            holder.Lock();

            DrinkQueueChanged?.Invoke();
            PourChanged?.Invoke(drinkOrder);

            while (!pourTimer.IsDone)
                yield return null;

            PourChanged?.Invoke(DrinkTypeSize.Empty);
            drinkable.Fill(drinkOrder);
            drinkable.EnableLidHolder();

            holder.Unlock();

            while (holder.Item != null)
                yield return null;

            foreach (var h in holderQueue)
                h.Lock();

            var tween = TweenManager.DoTween(
                t =>
                {
                    const int positionCount = 3;

                    for (int i = 0; i < holderQueue.Count; i++)
                    {
                        var holder = holderQueue[i];
                        int pointIndex = positionCount - i;
                        float initialT = bezier.GetPercentageForPoint(pointIndex);
                        float nextT;

                        if (pointIndex == positionCount)
                            nextT = 1;
                        else
                            nextT = bezier.GetPercentageForPoint(pointIndex + 1);

                        float remaining = nextT - initialT;
                        holder.transform.position = bezier.Sample(initialT + t * remaining);
                    }
                },
                new StructTweenData(2.0f)
            );

            while (tween.IsPlaying)
                yield return null;

            holderQueue.RemoveAt(0);
            holderQueue.Add(holder);

            foreach (var h in holderQueue)
                h.Unlock();

            isPouring = false;

            TryPour();
        }

        private float GetPourTime(Drinkable.Size size)
        {
            return size switch
            {
                Drinkable.Size.Small => SMALL_POUR_TIME,
                Drinkable.Size.Medium => MEDIUM_POUR_TIME,
                Drinkable.Size.Large => LARGE_POUR_TIME,
                _ => 0,
            };
        }
    }
}
