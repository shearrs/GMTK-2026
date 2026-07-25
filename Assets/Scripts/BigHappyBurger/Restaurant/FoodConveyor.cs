using System;
using System.Collections;
using System.Collections.Generic;
using BigHappyBurger.Foods;
using Shears;
using Shears.Beziers;
using Shears.Logging;
using Shears.Tweens;
using UnityEngine;

namespace BigHappyBurger.Restaurants
{
    public class FoodConveyor : MonoBehaviour
    {
        private const int LAST_BEZIER_INDEX = 4;
        private const int MAX_ACTIVE_SIZE = 4;
        private const int MAX_QUEUE_SIZE = 4;
        private const float RELEASE_DELAY = 2.0f;

        [SerializeField, Required, Local]
        private Bezier bezier;

        [SerializeField]
        private TweenData foodTweenData = new(1.0f, easingFunction: TweenEase.OutBack);

        [SerializeField]
        private List<FoodHolder> activeHolders = new();

        [SerializeField, Required(targetCollectionSize: 4), Local]
        private List<FoodHolder> inactiveHolders = new();

        [SerializeField]
        private List<Food> queue = new();

        private readonly Timer releaseDelayTimer = new(RELEASE_DELAY);
        private readonly Dictionary<FoodHolder, Tween> holderTweens = new();

        public event Action QueueChanged;

        private void Awake()
        {
            foreach (var holder in inactiveHolders)
            {
                holder.gameObject.SetActive(false);
                holder.ItemChanged += OnHolderItemChanged;
            }
        }

        private void OnDestroy()
        {
            foreach (var holder in inactiveHolders)
                holder.ItemChanged -= OnHolderItemChanged;

            foreach (var holder in activeHolders)
                holder.ItemChanged -= OnHolderItemChanged;
        }

        public void UpdateConveyor()
        {
            if (queue.Count == 0 || !releaseDelayTimer.IsDone)
                return;

            foreach (var holder in inactiveHolders)
            {
                if (holder.CanHold(queue[0]))
                {
                    var instance = Instantiate(queue[0]);
                    queue.RemoveAt(0);
                    holder.Hold(instance);
                    SetHolderActive(holder);
                    releaseDelayTimer.Start();

                    return;
                }
            }
        }

        public bool IsFull()
        {
            return activeHolders.Count == MAX_ACTIVE_SIZE && queue.Count >= MAX_QUEUE_SIZE;
        }

        public void AddFood(Food food)
        {
            foreach (var holder in inactiveHolders)
            {
                if (holder.CanHold(food))
                {
                    var instance = Instantiate(food);
                    holder.Hold(instance);
                    SetHolderActive(holder);
                    return;
                }
            }

            if (queue.Count >= MAX_QUEUE_SIZE)
            {
                SHLogger.Log($"Food conveyor tried to add to a full queue!", SHLogLevels.Error);
                return;
            }

            queue.Add(food);
            QueueChanged?.Invoke();
        }

        private void SetHolderActive(FoodHolder holder)
        {
            holder.transform.position = bezier.Points[0].Position;
            holder.gameObject.SetActive(true);
            inactiveHolders.Remove(holder);
            activeHolders.Add(holder);

            MoveHolder(holder, activeHolders.Count - 1);
        }

        private void MoveHolder(FoodHolder holder, int index)
        {
            holder.Lock();
            var targetPosition = bezier.Points[LAST_BEZIER_INDEX - index].Position;

            if (holderTweens.TryGetValue(holder, out var oldTween))
                oldTween.Dispose();

            var tween = holder.transform.DoMoveTween(targetPosition, foodTweenData);

            holderTweens[holder] = tween;

            tween.Completed += () =>
            {
                holderTweens.Remove(holder);
                holder.Unlock();
            };
        }

        private void OnHolderItemChanged(FoodHolder holder)
        {
            if (holder.Item != null)
                return;

            int index = activeHolders.IndexOf(holder);

            if (index == -1)
            {
                SHLogger.Log(
                    "Could not find conveyor holder in active holders!",
                    SHLogLevels.Error
                );
                return;
            }

            for (int i = index; i < activeHolders.Count; i++)
            {
                var current = activeHolders[i];

                if (current == holder)
                    continue;

                MoveHolder(current, i - 1);
            }

            activeHolders.Remove(holder);
            holder.gameObject.SetActive(false);
            inactiveHolders.Add(holder);
        }
    }
}
