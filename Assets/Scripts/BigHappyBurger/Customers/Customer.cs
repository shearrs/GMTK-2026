using System;
using System.Collections;
using System.Collections.Generic;
using BigHappyBurger.Customers;
using BigHappyBurger.Foods;
using BigHappyBurger.Restaurants;
using Shears;
using Shears.Beziers;
using Shears.Logging;
using UnityEngine;

[RequireComponent(typeof(CustomerFoodReceiver)), DisallowMultipleComponent, SelectionBase]
public partial class Customer : MonoBehaviour, ISHLoggable
{
    private const float EXIT_DELAY = 1.0f;
    private const float EXIT_TIME = 1.0f;

    [field: Header("Logging")]
    public SHLogLevels LogLevels { get; set; } = SHLogUtil.Default;

    [Header("Customer")]
    [SerializeField, ReadOnly]
    private Car car;

    [SerializeField]
    private Order order;

    [Auto]
    [AutoEvent(nameof(CustomerFoodReceiver.DrinkReceived), nameof(OnDrinkReceived))]
    [AutoEvent(nameof(CustomerFoodReceiver.BagReceived), nameof(OnBagReceived))]
    private CustomerFoodReceiver foodReceiver;

    private readonly Timer exitTimer = new();
    private readonly List<Food> bagFoods = new();
    private bool isSpawned;
    private Bezier exitRoute;
    private Restaurant restaurant;

    internal Car Car => car;
    public Order Order => order;
    public float Disatisfaction => order.Timer.Percentage;
    public bool IsExiting { get; private set; }

    public event Action Spawned;
    public event Action ReachedWindow;
    public event Action ReceivedRightItem;
    public event Action ReceivedWrongItem;
    public event Action CorrectlyServed;
    public event Action WaitedTooLong;
    public event Action BeganExiting;
    public event Action<Customer> Exited;
    public event Action BeganTalking;

    private void OnDestroy()
    {
        if (order != null)
        {
            order.Timer.Stop();
            order.Timer.Completed -= OnDisatisfactionTimerCompleted;
        }
    }

    public void SetOrder(Order order)
    {
        this.order = order;

        order.StartTimer();
        order.Timer.Completed += OnDisatisfactionTimerCompleted;
    }

    public void SetRestaurant(Restaurant restaurant)
    {
        this.restaurant = restaurant;
    }

    public void SetPosition(Vector3 position)
    {
        if (car == null)
        {
            this.Log("Customer has no car!", SHLogLevels.Error);
            return;
        }

        car.transform.position = position;
    }

    public void SetRotation(Quaternion rotation)
    {
        if (car == null)
        {
            this.Log("Customer has no car!", SHLogLevels.Error);
            return;
        }

        car.transform.rotation = rotation;
    }

    public void OnDialogue()
    {
        BeganTalking?.Invoke();
    }

    internal void Spawn(Bezier exitRoute)
    {
        if (car != null)
            car.gameObject.SetActive(true);

        this.exitRoute = exitRoute;

        gameObject.SetActive(true);
        foodReceiver.Disable();
        Spawned?.Invoke();

        isSpawned = true;
    }

    internal void OnReachedWindow()
    {
        foodReceiver.Enable();

        ReachedWindow?.Invoke();
    }

    internal void SetCar(Car car)
    {
        this.car = car;

        transform.SetParent(car.DriversSeat);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    internal void Dispose()
    {
        Destroy(car.gameObject);
    }

    private void OnDrinkReceived(Drinkable drink)
    {
        bool correctDrink = order.TakeDrink(drink);
        foodReceiver.Clear();

        if (!correctDrink)
            OnWrongItem();
        else
            ReceivedRightItem?.Invoke();

        if (order.IsEmpty())
        {
            StartCoroutine(IEExit());
            CorrectlyServed?.Invoke();
        }
    }

    private void OnBagReceived(Bag bag)
    {
        if (order.NeedsHappyBox && !bag.IsHappyBox)
        {
            foodReceiver.Clear();
            OnWrongItem();
            return;
        }

        bool hasNapkin = false;

        bag.GetFood(bagFoods);

        if (bagFoods.Count == 0)
        {
            foodReceiver.Clear();
            OnWrongItem();
            return;
        }

        foreach (var food in bagFoods)
        {
            bool correctFood = order.TakeFood(food);

            if (food.IsNapkin)
                hasNapkin = true;

            if (!correctFood && !food.IsNapkin)
            {
                OnWrongItem();
                break;
            }
        }

        foodReceiver.Clear();

        if (order.IsEmpty())
        {
            if (!hasNapkin)
                OnWrongItem();
            else
            {
                ReceivedRightItem?.Invoke();
                StartCoroutine(IEExit());
                CorrectlyServed?.Invoke();
            }
        }
        else if (order.Foods.Count > 0)
            OnWrongItem();
        else
            ReceivedRightItem?.Invoke();
    }

    private void OnWrongItem()
    {
        restaurant.AddBadMark();
        StartCoroutine(IEExit(2.5f));

        ReceivedWrongItem?.Invoke();
    }

    private void OnDisatisfactionTimerCompleted()
    {
        if (this == null)
            return;

        WaitedTooLong?.Invoke();

        if (isSpawned)
            StartCoroutine(IEExit(.5f));
        else
            Exited?.Invoke(this);

        restaurant.AddBadMark();
    }

    private IEnumerator IEExit(float extraDelay = 0.0f)
    {
        if (!exitTimer.IsDone)
            yield break;

        IsExiting = true;
        BeganExiting?.Invoke();

        exitTimer.Start(EXIT_DELAY + extraDelay);

        while (!exitTimer.IsDone)
            yield return null;

        exitTimer.Start(EXIT_TIME);
        foodReceiver.Disable();

        while (!exitTimer.IsDone)
        {
            float t = Mathf.Min(1.0f, exitTimer.Percentage);
            exitRoute.SampleWithRotation(t, out var position, out var rotation);

            SetPosition(position);
            SetRotation(rotation);

            yield return null;
        }

        Exited?.Invoke(this);
    }
}
