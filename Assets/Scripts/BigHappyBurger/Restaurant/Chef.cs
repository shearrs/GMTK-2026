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
        private bool hasTimers; // this is not a good way to do this, but we are just making things work

        public IReadOnlyList<ChefSlot> Slots => slots;

        public event Action<IReadOnlyList<ChefSlot>> ChefSlotsChanged;

        [Serializable]
        public class ChefSlot
        {
            [SerializeField]
            private Cookable cookable;

            private readonly Timer timer = new();

            internal bool IsForceCompleted { get; set; } = false;
            public Cookable Cookable
            {
                get => cookable;
                set
                {
                    IsForceCompleted = false;
                    cookable = value;
                    FoodChanged?.Invoke(cookable);
                }
            }
            public bool IsCooking =>
                Cookable != null && !IsForceCompleted && (!timer.IsDone || !HasTimer);
            public bool HasTimer { get; internal set; }
            public Timer Timer => timer;

            public event Action<Cookable> FoodChanged;

            public void BeginCooking()
            {
                if (Cookable == null)
                {
                    SHLogger.Log($"Tried to begin cooking an empty slot!", SHLogLevels.Error);
                    return;
                }

                Debug.Log("begin cooking: " + Cookable.Food.gameObject.name);
                timer.Start(Cookable.CookTime);
            }

            public void PauseCooking()
            {
                timer.Pause();
            }

            public void UnpauseCooking()
            {
                if (Cookable != null && !timer.IsDone)
                    timer.Unpause();
            }
        }

        public void AddOrder(Order order)
        {
            foreach (var food in order.Foods)
            {
                if (food.IsCookable)
                {
                    Debug.Log("add food: " + food.gameObject.name);

                    cookQueue.Add(food.GetComponent<Cookable>());
                    hasTimers = order.HasTimer;
                }
            }
        }

        public void UpdateCooking()
        {
            if (foodConveyor.IsFull())
            {
                PauseCooking();
                return;
            }
            else if (isPaused)
                UnpauseCooking();

            ProcessSlots(out var openSlot);

            if (openSlot == null || cookQueue.Count == 0)
                return;

            for (int i = 0; i < cookQueue.Count; i++)
            {
                var food = cookQueue[i];

                if (!FoodAlreadyCooking(food))
                {
                    openSlot.Cookable = food;

                    cookQueue.RemoveAt(i);
                    i--;
                    openSlot.HasTimer = hasTimers;

                    if (hasTimers)
                        openSlot.BeginCooking();
                }
            }

            ChefSlotsChanged?.Invoke(slots);
        }

        public void Clear()
        {
            foreach (var slot in slots)
            {
                slot.Timer.Stop();
                slot.Cookable = null;
            }
        }

        public void ForceComplete(ChefSlot slot)
        {
            if (!slot.IsCooking)
                return;

            slot.IsForceCompleted = true;
        }

        private void PauseCooking()
        {
            if (isPaused)
                return;

            foreach (var slot in slots)
                slot.PauseCooking();

            isPaused = true;
        }

        private void UnpauseCooking()
        {
            if (!isPaused)
                return;

            foreach (var slot in slots)
                slot.UnpauseCooking();

            isPaused = false;
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
