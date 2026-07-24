using BigHappyBurger.Foods;
using System.Collections.Generic;
using NUnit.Framework;
using Shears.Beziers;
using UnityEngine;
using Shears;

public class Chef : MonoBehaviour
{
    [SerializeField]
    private Bezier foodConveyor;

    [SerializeField]
    private FoodTimer burgerTimer;
    [SerializeField]
    private FoodTimer fryTimer;
    [SerializeField]
    private FoodTimer nuggieTimer;
    [SerializeField]
    private FoodTimer chickenSammieTimer;

    private readonly List<Food> foodToCook = new();

    private int foodOnConveyor = 0;
    private int foodWaiting = 0;
    private int foodCreated = 0;


    public struct FoodEntry
    {
        public Food Food { get; set; }
        public Timer Timer { get; set; }

        public FoodEntry(Food food)
        {
            Food = food;
            Timer = new();
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        if(foodToCook.Count > 0 && foodWaiting < 8)
        {

        }
    }

    
}
