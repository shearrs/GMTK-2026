using System.Collections.Generic;
using BigHappyBurger.Customers;
using BigHappyBurger.Foods;
using Shears;
using UnityEngine;
using UnityEngine.UIElements;

namespace BigHappyBurger.Restaurants.Graphics
{
    [RequireComponent(typeof(AudioSource))]
    public partial class OrderQueueGraphics : MonoBehaviour
    {
        private static readonly Color HIGHLIGHT_COLOR = new(0.9169811f, 0.7119653f, 0.102079f);

        [SerializeField, Required, Local]
        private PanelRenderer panel;

        [SerializeField, Required, Local]
        private VisualTreeAsset orderElementAsset;

        [SerializeField, Required, Local]
        private Sprite happyBoxSprite;

        [SerializeField, Required, Local]
        private Font drinkFont;

        [SerializeField, Required, Local]
        private AudioClip orderAddedClip;

        [SerializeField, Required, Local]
        private AudioClip successfulOrderClip;

        [SerializeField, Required]
        [AutoEvent(nameof(Restaurant.OrderAdded), nameof(OnOrderAdded))]
        [AutoEvent(nameof(Restaurant.OrderRemoved), nameof(OnOrderRemoved))]
        private Restaurant restaurant;

        [SerializeField, Required]
        [AutoEvent(nameof(CustomerManager.CustomerCorrectlyServed), nameof(OnSuccessfulOrder))]
        private CustomerManager customerManager;

        [Auto]
        private AudioSource audioSource;

        private readonly Dictionary<Order, VisualElement> orderElements = new();
        private readonly Dictionary<Order, Label> orderTimers = new();
        private VisualElement root;

        private void OnEnable()
        {
            __AutoOnEnable();

            panel.RegisterUIReloadCallback(OnUILoaded);
        }

        private void OnDisable()
        {
            __AutoOnDisable();

            panel.UnregisterUIReloadCallback(OnUILoaded);
        }

        private void Update()
        {
            foreach (var (order, timerLabel) in orderTimers)
            {
                if (!order.HasTimer)
                    timerLabel.text = "...";
                else
                    timerLabel.text = GetTimerTime(order.Timer);
            }
        }

        private void OnUILoaded(PanelRenderer _, VisualElement root)
        {
            this.root = root.Q("ScrollView");

            foreach (var order in restaurant.Orders)
                AddOrderElement(order);
        }

        private void OnOrderAdded(Order order)
        {
            if (root == null)
                return;

            var orderElement = AddOrderElement(order);
            audioSource.volume = 0.65f;
            audioSource.PlayOneShot(orderAddedClip);

            if (orderElements.Count == 1)
            {
                var orderContainer = orderElement.Q("Container");

                orderContainer.style.backgroundColor = HIGHLIGHT_COLOR;
            }
        }

        private void OnOrderRemoved(Order order)
        {
            if (orderElements.TryGetValue(order, out var element))
            {
                element.RemoveFromHierarchy();
                orderElements.Remove(order);
                orderTimers.Remove(order);
            }

            if (orderElements.Count > 0)
            {
                var topElement = orderElements[restaurant.Orders[0]];
                var orderContainer = topElement.Q("Container");

                orderContainer.style.backgroundColor = HIGHLIGHT_COLOR;
            }
        }

        private VisualElement AddOrderElement(Order order)
        {
            if (orderElements.ContainsKey(order))
                return null;

            VisualElement element = Instantiate(orderElementAsset).CloneTree();
            VisualElement iconContainer = element.Query("IconContainer");

            if (order.NeedsHappyBox)
            {
                var image = CreateImage(happyBoxSprite);

                iconContainer.Add(image);
            }
            else
            {
                foreach (var food in order.Foods)
                {
                    var image = CreateImage(food.Sprite);

                    iconContainer.Add(image);
                }
            }

            foreach (var drink in order.Drinks)
            {
                var image = CreateImage(drink.Type.Sprite);
                var label = CreateSizeLabel(drink.Size);

                image.Add(label);
                iconContainer.Add(image);
            }

            Label timerLabel = element.Query<Label>("TimerLabel");

            orderElements[order] = element;
            orderTimers[order] = timerLabel;

            timerLabel.text = GetTimerTime(order.Timer);

            root.Add(element);

            return element;
        }

        private Image CreateImage(Sprite sprite)
        {
            var image = new Image();
            image.style.width = 80;
            image.style.height = 80;
            image.style.aspectRatio = new(1.0f);
            image.sprite = sprite;

            return image;
        }

        private Label CreateSizeLabel(Drinkable.Size size)
        {
            var label = new Label();

            string sizeText;
            switch (size)
            {
                case Drinkable.Size.Small:
                    sizeText = "S";
                    break;
                case Drinkable.Size.Medium:
                    sizeText = "M";
                    break;
                case Drinkable.Size.Large:
                    sizeText = "L";
                    break;
                default:
                    return label;
            }

            label.text = sizeText;
            label.style.unityFont = drinkFont;
            label.style.fontSize = 56;
            label.style.marginTop = 90.0f;
            label.style.marginRight = label.style.marginBottom = label.style.marginLeft = 0;
            label.style.paddingTop =
                label.style.paddingRight =
                label.style.paddingBottom =
                label.style.paddingLeft =
                    0;
            label.style.unityTextAlign = TextAnchor.LowerRight;
            label.style.unityTextOutlineWidth = 1.5f;
            label.style.unityTextOutlineColor = Color.black;
            label.style.color = Color.white;
            label.style.unityFontStyleAndWeight = FontStyle.Bold;

            return label;
        }

        private string GetTimerTime(Timer timer)
        {
            int seconds = Mathf.RoundToInt(timer.Time - timer.CurrentTime);
            int minutes = seconds / 60;
            seconds -= 60 * minutes;

            return $"{minutes}:{(seconds < 10 ? "0" : "")}{seconds}";
        }

        private void OnSuccessfulOrder()
        {
            audioSource.volume = 0.35f;
            audioSource.PlayOneShot(successfulOrderClip);
        }
    }
}
