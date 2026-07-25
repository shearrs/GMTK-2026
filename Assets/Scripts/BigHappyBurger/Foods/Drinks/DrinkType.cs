using UnityEngine;

namespace BigHappyBurger.Foods
{
    [CreateAssetMenu(menuName = "Big Happy Burger/Drink")]
    public class DrinkType : ScriptableObject
    {
        [field: SerializeField]
        public string Name { get; private set; }

        [field: SerializeField]
        public Sprite Sprite { get; private set; }
    }
}
