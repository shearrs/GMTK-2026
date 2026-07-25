using Shears;
using UnityEngine;

namespace BigHappyBurger.Foods
{
    [RequireComponent(typeof(Food))]
    public class Cookable : MonoBehaviour
    {
        [SerializeField]
        private int cookTime = 0;

        private Food food;

        public Food Food
        {
            get
            {
                if (food == null)
                    food = GetComponent<Food>();

                return food;
            }
        }
        public int CookTime => cookTime;

        private void Reset()
        {
            var food = GetComponent<Food>();

            food.SetCookable(this);
        }
    }
}
