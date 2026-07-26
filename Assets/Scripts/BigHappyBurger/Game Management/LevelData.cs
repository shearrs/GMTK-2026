using System.Collections.Generic;
using BigHappyBurger.Foods;
using Shears;
using UnityEngine;

namespace BigHappyBurger.GameManagement
{
    [CreateAssetMenu(menuName = "Big Happy Burger/Level Data")]
    public class LevelData : ScriptableObject
    {
        [field: SerializeField]
        public int CustomerCount { get; private set; }

        [field: SerializeField, Min(0.0f)]
        public float TimePerOrderMultiplier { get; private set; } = 1.0f;

        [field: SerializeField]
        public Range<float> CustomerArrivalRange { get; private set; } = new(10.0f, 20.0f);

        [SerializeField]
        private Food[] possibleFoods;

        public IReadOnlyList<Food> PossibleFoods => possibleFoods;
    }
}
