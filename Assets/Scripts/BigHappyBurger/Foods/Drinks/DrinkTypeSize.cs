using UnityEngine;

namespace BigHappyBurger.Foods
{
    [System.Serializable]
    public struct DrinkTypeSize
    {
        public static readonly DrinkTypeSize Empty = new();

        [field: SerializeField]
        public DrinkType Type { get; private set; }

        [field: SerializeField]
        public Drinkable.Size Size { get; private set; }

        public DrinkTypeSize(DrinkType type, Drinkable.Size size)
        {
            Type = type;
            Size = size;
        }
    }
}
