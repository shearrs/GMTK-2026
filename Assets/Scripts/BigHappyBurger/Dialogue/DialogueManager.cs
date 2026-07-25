using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        [Serializable] public struct Response
        {
            [SerializeField] public string text;
            [SerializeField] public float timeBonus;
        }

        [SerializeField] private PanelRenderer panelRenderer;
        [SerializeField] private DialogueData data;
        [SerializeField] private StyleSheet style;

        // Should have just read from a file :(
        [Header("Dialogues")]
        [Header("Greetings")]
        [SerializeField] private string[]   playerGreetings;
        [SerializeField] private Response[] customerGreetingsReply;
        [Header("Compliment")]
        [SerializeField] private string[]   playerCompliment;
        [SerializeField] private Response[] customerComplimentReply;
        [Header("Update")]
        [SerializeField] private string[]   playerUpdate;
        [SerializeField] private Response[] customerUpdateReply;
        [Header("Weather")]
        [SerializeField] private string[]   playerWeather;
        [SerializeField] private Response[] customerWeatherReply;
        [Header("Plans")]
        [SerializeField] private string[]   playerPlans;
        [SerializeField] private Response[] customerPlansReply;

        private UnityEngine.UIElements.VisualElement customerDialogue;
        private UnityEngine.UIElements.VisualElement responseSingular;
        private UnityEngine.UIElements.VisualElement responseQuad;

        void OnEnable()
        {
            panelRenderer.RegisterUIReloadCallback(OnUIReload);
        }

        void OnDisable()
        {
            panelRenderer.UnregisterUIReloadCallback(OnUIReload);
        }

        void OnUIReload(PanelRenderer _renderer, VisualElement _rootElement)
        {
            // Squirrel these references away so we can toggle them later
            this.customerDialogue = _rootElement.Q<VisualElement>("CustomerDialogue");
            this.responseSingular = _rootElement.Q<VisualElement>("ResponseSingular");
            this.responseQuad     = _rootElement.Q<VisualElement>("ResponseQuad");

            // Subscribe some events
            _rootElement.Q<Button>("ButtonGreeting").clicked   += () => Respond(ref customerGreetingsReply,  true);

            _rootElement.Q<Button>("ButtonCompliment").clicked += () => Respond(ref customerComplimentReply, false);
            _rootElement.Q<Button>("ButtonUpdate"    ).clicked += () => Respond(ref customerUpdateReply,     false);
            _rootElement.Q<Button>("ButtonWeather"   ).clicked += () => Respond(ref customerWeatherReply,    false);
            _rootElement.Q<Button>("ButtonPlans"     ).clicked += () => Respond(ref customerPlansReply,      false);

            this.ResetData();

            this.EnterDialogue();
        }

        private void OnClickCompliment()
        {
            UnityEngine.Debug.Log("OnClickCompliment");
        }
        private void OnClickWeather()
        {
            UnityEngine.Debug.Log("OnClickWeather");
        }
        private void OnClickUpdate()
        {
            UnityEngine.Debug.Log("OnClickUpdate");
        }
        private void OnClickPlans()
        {
            UnityEngine.Debug.Log("OnClickPlans");
        }
    
        private void Respond(ref Response[] _responses, bool _continueDialogue)
        {
            this.customerDialogue.RemoveFromClassList("hidden");
            this.responseSingular.AddToClassList("hidden");

            Response response = _responses[UnityEngine.Random.Range(0, _responses.Length)];
            this.data.dialogueText = response.text;
            UnityEngine.Debug.LogFormat("Time increased by: {0}", response.timeBonus);

            if (_continueDialogue)
            {
                this.responseQuad.RemoveFromClassList("hidden");

                this.data.replyCompliment = this.playerCompliment[UnityEngine.Random.Range(0, this.playerCompliment.Length)];
                this.data.replyUpdate     = this.playerUpdate    [UnityEngine.Random.Range(0, this.playerUpdate.Length)    ];
                this.data.replyWeather    = this.playerWeather   [UnityEngine.Random.Range(0, this.playerWeather.Length)   ];
                this.data.replyPlans      = this.playerPlans     [UnityEngine.Random.Range(0, this.playerPlans.Length)     ];
            }
            else
            {
                this.responseQuad.AddToClassList("hidden");
            }
        }


        public void EnterDialogue()
        {
            this.responseSingular.RemoveFromClassList("hidden");
            this.data.introduction = this.playerGreetings[UnityEngine.Random.Range(0, this.playerGreetings.Length)];

            UnityEngine.Debug.Log("We're here");
        }

        public void LeaveDialogue()
        {
            this.ResetData();
        }

        private void ResetData()
        {
            this.customerDialogue.AddToClassList("hidden");

            this.responseSingular.AddToClassList("hidden");
            this.responseQuad.AddToClassList("hidden");

            this.data.dialogueText = "";
            this.data.introduction = "";
            this.data.replyCompliment = "";
            this.data.replyUpdate = "";
            this.data.replyWeather = "";
            this.data.replyPlans = "";
        }
    }
}