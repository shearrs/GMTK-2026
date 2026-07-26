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
    private const float EXIT_TIME = 1.0f;

    [field: Header("Logging")]
    public SHLogLevels LogLevels { get; set; } = SHLogUtil.Default;

    [Header("Customer")]
    [SerializeField, ReadOnly]
    private Car car;

    [SerializeField]
    private Order order;

    [SerializeField]
    private Timer disatisfactionTimer;

    [Auto]
    [AutoEvent(nameof(CustomerFoodReceiver.DrinkReceived), nameof(OnDrinkReceived))]
    [AutoEvent(nameof(CustomerFoodReceiver.BagReceived), nameof(OnBagReceived))]
    private CustomerFoodReceiver foodReceiver;

    private readonly Timer exitTimer = new(EXIT_TIME);
    private readonly List<Food> bagFoods = new();
    private Bezier exitRoute;
    private Restaurant restaurant;

    public Order Order => order;
    public float Disatisfaction => disatisfactionTimer.Percentage;

    public event Action Spawned;
    public event Action<Customer> Exited;

    public void SetOrder(Order order)
    {
        this.order = order;
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

    internal void Spawn(Restaurant restaurant, Bezier exitRoute)
    {
        if (car != null)
            car.gameObject.SetActive(true);

        this.restaurant = restaurant;
        this.exitRoute = exitRoute;

        gameObject.SetActive(true);
        foodReceiver.Disable();
        Spawned?.Invoke();

        disatisfactionTimer.Start(order.GetExpectedWaitTime());
    }

    internal void OnReachedWindow()
    {
        foodReceiver.Enable();
    }

    internal void SetCar(Car car)
    {
        this.car = car;

        transform.SetParent(car.DriversSeat);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    private void OnDrinkReceived(Drinkable drink)
    {
        if (order.Drinks.Count == 0)
            return;

        bool correctDrink = order.TakeDrink(drink);
        foodReceiver.Clear();

        if (!correctDrink)
            StartCoroutine(IEExit());
    }

    private void OnBagReceived(Bag bag)
    {
        bag.GetFood(bagFoods);

        foreach (var food in bagFoods)
        {
            bool correctFood = order.TakeFood(food);

            if (!correctFood)
            {
                StartCoroutine(IEExit());
                break;
            }
        }

        foodReceiver.Clear();
    }

    private IEnumerator IEExit()
    {
        exitTimer.Start();
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
        Destroy(gameObject);
    }
}
