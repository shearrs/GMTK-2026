using BigHappyBurger.Customers;
using BigHappyBurger.Restaurants;
using Shears;
using TMPro;
using UnityEngine;

namespace BigHappyBurger.GameManagement.Graphics
{
    public partial class GameCounterUI : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField, Required]
        private LevelData levelData;

        [SerializeField, Required]
        [AutoEvent(nameof(Restaurant.MarksChanged), nameof(OnMarksChanged))]
        private Restaurant restaurant;

        [SerializeField, Required]
        [AutoEvent(nameof(CustomerManager.CustomerCorrectlyServed), nameof(OnCustomerServed))]
        private CustomerManager customerManager;

        [Header("Text")]
        [SerializeField, Required]
        private TextMeshPro servedText;

        [SerializeField, Required]
        private TextMeshPro failedText;

        int customersToServe = 0;

        private void Awake()
        {
            customersToServe = levelData.CustomerCount;

            failedText.text = "FAILED ORDERS: 0";
            servedText.text = $"CUSTOMERS TO SERVE: {customersToServe}/{levelData.CustomerCount}";
        }

        private void OnMarksChanged(int marks)
        {
            failedText.text = $"FAILED ORDERS: {marks}";
        }

        private void OnCustomerServed()
        {
            customersToServe--;
            servedText.text = $"CUSTOMERS TO SERVE: {customersToServe}/{levelData.CustomerCount}";
        }
    }
}
