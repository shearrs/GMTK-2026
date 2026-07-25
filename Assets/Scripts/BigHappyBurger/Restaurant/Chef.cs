using System;
using System.Collections.Generic;
using BigHappyBurger.Foods;
using Shears;
using Shears.Logging;
using UnityEngine;

namespace BigHappyBurger.Restaurants
{
    public class Chef : MonoBehaviour
    {
        [SerializeField, Required, Local]
        private FoodConveyor foodConveyor;

        [SerializeField, Required(targetCollectionSize: 4)]
        private ChefSlot[] slots;

        [SerializeField]
        private List<Cookable> cookQueue = new();

        private bool isPaused;

        public event Action<IReadOnlyList<ChefSlot>> ChefSlotsChanged;

        [Serializable]
        public class ChefSlot
        {
            [field: SerializeField]
            public Cookable Cookable { get; internal set; }

            private readonly Timer timer = new();

            public bool IsCooking => Cookable != null && !timer.IsDone;
            public bool IsPaused => timer.IsPaused;
            public Timer Timer { get; internal set; }

            public void BeginCooking()
            {
                if (Cookable == null)
                {
                    SHLogger.Log($"Tried to begin cooking an empty slot!", SHLogLevels.Error);
                    return;
                }

                timer.Start(Cookable.CookTime);
            }

            public void PauseCooking()
            {
                timer.Pause();
            }
        }

        public void AddOrder(Order order)
        {
            foreach (var food in order.Foods)
            {
                if (food.IsCookable)
                    cookQueue.Add(food.GetComponent<Cookable>());
            }
        }

        public void UpdateCooking()
        {
            if (foodConveyor.IsFull())
            {
                PauseCooking();
                return;
            }
            else
                isPaused = false;

            ProcessSlots(out var openSlot);

            if (openSlot == null || cookQueue.Count == 0)
                return;

            for (int i = 0; i < cookQueue.Count; i++)
            {
                var food = cookQueue[i];

                if (!FoodAlreadyCooking(food))
                {
                    cookQueue.RemoveAt(i);
                    openSlot.Cookable = food;
                    openSlot.BeginCooking();
                }
            }

            ChefSlotsChanged?.Invoke(slots);
        }

        private void PauseCooking()
        {
            if (isPaused)
                return;

            foreach (var slot in slots)
                slot.PauseCooking();

            isPaused = true;
        }

        private void ProcessSlots(out ChefSlot openSlot)
        {
            openSlot = null;

            foreach (var slot in slots)
            {
                if (!slot.IsCooking)
                {
                    if (slot.Cookable != null && !foodConveyor.IsFull())
                    {
                        foodConveyor.AddFood(slot.Cookable.Food);
                        slot.Cookable = null;
                    }

                    openSlot ??= slot;
                }
            }
        }

        private bool FoodAlreadyCooking(Cookable cookable)
        {
            foreach (var slot in slots)
            {
                if (slot.IsCooking && slot.Cookable.Food.ID == cookable.Food.ID)
                    return true;
            }

            return false;
        }
    }
}
