using UnityEngine;

namespace BigHappyBurger.Foods
{
    [RequireComponent(typeof(Food))]
    public class Cookable : MonoBehaviour
    {
        [SerializeField]
        private int cookTime = 0;

        public int CookTime => cookTime;

        private void Reset()
        {
            var food = GetComponent<Food>();

            food.SetCookable(this);
        }
    }
}
