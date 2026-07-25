using UnityEngine;

namespace BigHappyBurger.Foods
{
    public readonly struct DrinkTypeSize
    {
        public static readonly DrinkTypeSize Empty = new();

        public DrinkType Type { get; }
        public Drinkable.Size Size { get; }

        public DrinkTypeSize(DrinkType type, Drinkable.Size size)
        {
            Type = type;
            Size = size;
        }
    }
}
