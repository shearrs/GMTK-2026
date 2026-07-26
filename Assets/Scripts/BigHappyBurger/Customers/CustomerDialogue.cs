using System;
using System.Collections;
using Shears;
using Shears.Tweens;
using Shears.UI;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BigHappyBurger.Customers
{
    public partial class CustomerDialogue : UIElement
    {
        #region //Buttons and Text
        [SerializeField]
        private UIButton customerTextbox;

        [SerializeField]
        private UIButton playerGreeting;

        [SerializeField]
        [AutoEvent(nameof(UIButton.Clicked), nameof(OnOption1Clicked))]
        private UIButton playerOption1;

        [SerializeField]
        [AutoEvent(nameof(UIButton.Clicked), nameof(OnOption2Clicked))]
        private UIButton playerOption2;

        [SerializeField]
        [AutoEvent(nameof(UIButton.Clicked), nameof(OnOption3Clicked))]
        private UIButton playerOption3;

        [SerializeField]
        [AutoEvent(nameof(UIButton.Clicked), nameof(OnOption4Clicked))]
        private UIButton playerOption4;

        [SerializeField]
        private TextMeshPro customerText;

        [SerializeField]
        private TextMeshPro playerGreetingText;

        [SerializeField]
        private TextMeshPro playerOptionText1;

        [SerializeField]
        private TextMeshPro playerOptionText2;

        [SerializeField]
        private TextMeshPro playerOptionText3;

        [SerializeField]
        private TextMeshPro playerOptionText4;
        #endregion

        [SerializeField, Required]
        [AutoEvent(
            nameof(CustomerManager.CustomerArrivedAtWindow),
            nameof(OnCustomerArrivedAtWindow)
        )]
        private CustomerManager customerManager;

        [SerializeField]
        private RectTransform customerTextboxTransform;

        [SerializeField]
        private RectTransform customerTextboxTargetTransform;

        [SerializeField]
        private RectTransform customerTextboxOriginalTransform;

        [Serializable]
        public struct Response
        {
            [SerializeField]
            public string text;

            [SerializeField]
            public float timeBonus;
        }

        [Header("Dialogues")]
        [Header("Greetings")]
        [SerializeField]
        private Response[] customerGreetingsReply;

        [Header("Compliment")]
        [SerializeField]
        private string[] playerCompliment;

        [SerializeField]
        private Response[] customerComplimentReply;

        [Header("Update")]
        [SerializeField]
        private string[] playerUpdate;

        [SerializeField]
        private Response[] customerUpdateReply;

        [Header("Weather")]
        [SerializeField]
        private string[] playerWeather;

        [SerializeField]
        private Response[] customerWeatherReply;

        [Header("Plans")]
        [SerializeField]
        private Response[] customerPlansReply;

        [SerializeField]
        private TweenData fadeTween;

        [SerializeField]
        private TweenData shakeTween;

        public bool canStartDialogue = true;

        public void StartDialogue()
        {
            if (canStartDialogue)
            {
                canStartDialogue = false;
                StartCoroutine(DoGreetingAndResponse());
            }
        }

        private IEnumerator DoGreetingAndResponse()
        {
            var responseIndex = Random.Range(0, customerGreetingsReply.Length);
            var responseText = customerGreetingsReply[responseIndex].text;
            playerGreeting.gameObject.SetActive(true);
            StoreTween(playerGreeting.transform.DoShakeTween(.05f, 0.02f, shakeTween));

            yield return new WaitForSeconds(1f);

            customerText.text = $"<alpha=#00>{responseText}";
            var textboxEnterTween = StoreTween(
                customerTextboxTransform.DoMoveLocalTween(
                    customerTextboxTargetTransform.localPosition,
                    shakeTween
                )
            );
            StoreTween(customerTextbox.DoFadeTween(1.0f, shakeTween));

            textboxEnterTween.Completed += () =>
            {
                StartCoroutine(
                    CustomerResponseTextScroll(
                        responseText,
                        customerGreetingsReply[responseIndex].timeBonus
                    )
                );
            };

            yield return null;
        }

        private IEnumerator CustomerResponseTextScroll(string fullText, float timeBonus)
        {
            customerManager.OnDialogue();
            int totalLength = fullText.Length;

            for (int i = 0; i <= totalLength; i++)
            {
                string visible = fullText.Substring(0, i);
                string hidden = fullText.Substring(i);

                customerText.text = $"{visible}<alpha=#00>{hidden}";

                yield return new WaitForSeconds(0.02f);
            }

            //provide time bonus if any (use timeBonus)

            StoreTween(playerGreeting.DoFadeTween(0.0f, shakeTween));

            yield return new WaitForSeconds(1f);

            playerGreeting.gameObject.SetActive(false);
            playerGreeting.Alpha = 1.0f;

            StartCoroutine(RevealOptions());
        }

        private IEnumerator RevealOptions()
        {
            var complimentIndex = Random.Range(0, playerCompliment.Length);
            playerOptionText1.text = playerCompliment[complimentIndex];
            playerOption1.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.25f);

            var updateIndex = Random.Range(0, playerUpdate.Length);
            playerOptionText2.text = playerUpdate[updateIndex];
            playerOption2.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.25f);

            var weatherIndex = Random.Range(0, playerWeather.Length);
            playerOptionText3.text = playerWeather[weatherIndex];
            playerOption3.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.25f);

            playerOption4.gameObject.SetActive(true);
        }

        private void OnOption1Clicked()
        {
            var responseIndex = Random.Range(0, customerComplimentReply.Length);
            var responseText = customerComplimentReply[responseIndex].text;

            customerText.text = $"<alpha=#00>{responseText}";
            StartCoroutine(
                FinalResponse(responseText, customerComplimentReply[responseIndex].timeBonus)
            );
        }

        private void OnOption2Clicked()
        {
            var responseIndex = Random.Range(0, customerUpdateReply.Length);
            var responseText = customerUpdateReply[responseIndex].text;

            customerText.text = $"<alpha=#00>{responseText}";
            StartCoroutine(
                FinalResponse(responseText, customerUpdateReply[responseIndex].timeBonus)
            );
        }

        private void OnOption3Clicked()
        {
            var responseIndex = Random.Range(0, customerWeatherReply.Length);
            var responseText = customerWeatherReply[responseIndex].text;

            customerText.text = $"<alpha=#00>{responseText}";
            StartCoroutine(
                FinalResponse(responseText, customerWeatherReply[responseIndex].timeBonus)
            );
        }

        private void OnOption4Clicked()
        {
            var responseIndex = Random.Range(0, customerPlansReply.Length);
            var responseText = customerPlansReply[responseIndex].text;

            customerText.text = $"<alpha=#00>{responseText}";
            StartCoroutine(
                FinalResponse(responseText, customerPlansReply[responseIndex].timeBonus)
            );
        }

        private IEnumerator FinalResponse(string fullText, float timeBonus)
        {
            customerManager.OnDialogue();

            playerOption1.gameObject.SetActive(false);
            playerOption2.gameObject.SetActive(false);
            playerOption3.gameObject.SetActive(false);
            playerOption4.gameObject.SetActive(false);

            int totalLength = fullText.Length;

            for (int i = 0; i <= totalLength; i++)
            {
                string visible = fullText.Substring(0, i);
                string hidden = fullText.Substring(i);

                customerText.text = $"{visible}<alpha=#00>{hidden}";

                yield return new WaitForSeconds(0.02f);
            }

            //provide time bonus if any (use timeBonus)

            yield return new WaitForSeconds(1f);

            StoreTween(
                customerTextboxTransform.DoMoveLocalTween(
                    customerTextboxOriginalTransform.localPosition,
                    shakeTween
                )
            );
            StoreTween(customerTextbox.DoFadeTween(0.0f, shakeTween));

            yield return null;
        }

        private void OnCustomerArrivedAtWindow()
        {
            canStartDialogue = true;
        }
    }
}
