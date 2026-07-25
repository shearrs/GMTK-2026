using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TutorialSequence : MonoBehaviour
{
    [SerializeField]
    private PanelRenderer tutorialPanel;

    [SerializeField]
    private List<string> tutorialLines1 = new();

    private int lineIndex = 0;

    private Label tutorialText;

    private VisualElement root;

    private int tutorialPhase = 0;

    private Coroutine bossEnters;
    private Coroutine dialogue1;



    void OnEnable()
    {
        tutorialPanel.RegisterUIReloadCallback(OnUIReload);
    }

    void OnDisable()
    {
        tutorialPanel.UnregisterUIReloadCallback(OnUIReload);
    }

    void OnUIReload(PanelRenderer _renderer, VisualElement _rootElement)
    {
        root = _rootElement.Q<VisualElement>("TutorialTextbox");
        tutorialText = _rootElement.Q<Label>("DialogueText");
    }

    private void Start()
    {
        bossEnters = StartCoroutine(EnterScreen());
    }

    private IEnumerator EnterScreen()
    {
        yield return new WaitForSeconds(1.0f);

        root.RemoveFromClassList("TextBoxHidden");

        yield return new WaitForSeconds(1.0f);

        Debug.Log(tutorialLines1.Count);

        if (lineIndex < tutorialLines1.Count)
        {
            dialogue1 = StartCoroutine(TutorialDialogue1(tutorialLines1[lineIndex]));
        }
    }

    private IEnumerator TutorialDialogue1(string fullText)
    {
        int totalLength = fullText.Length;

        for (int i = 0; i <= totalLength; i++)
        {
            string visible = fullText.Substring(0, i);
            string hidden = fullText.Substring(i);

            tutorialText.text = $"{visible}<alpha=#00>{hidden}";

            yield return new WaitForSeconds(0.02f);
        }

        tutorialText.text = fullText;
        lineIndex++;

        if (lineIndex < tutorialLines1.Count)
        {
            switch (lineIndex)
            {
                case 6:
                    //enable all turning
                    //change to be a check to see if the player is facing the hotplate station
                    while (!Keyboard.current.dKey.wasPressedThisFrame)
                        yield return null;
                    break;

                case 8:
                    //check if the player has grabbed a bag at least once
                    break;

                case 9:
                    //check if the player has put a bag on the counter at least once
                    break;

                case 11:
                    //check if the player put the fries in a bag
                    break;

                case 12:
                    //check if the player put ketchup in the fry bag
                    break;

                case 14:
                    //check if napkins have been placed in the fry bag
                    break;

                case 15:
                    //check if player is at the drive-thru window
                    break;

                case 16:
                    //wait for first customer completed
                    break;

                case 18:
                    //wait for player to return to order screen
                    break;

                case 20:
                    //wait for player to go to drink screen
                    break;

                default:
                    while (!Mouse.current.leftButton.wasPressedThisFrame)
                        yield return null;
                    break;
            }

            if (lineIndex == 2)
            {
                //add first customer to board
            }

            if (lineIndex == 17)
            {
                //add second customer to board
            }

            dialogue1 = StartCoroutine(TutorialDialogue1(tutorialLines1[lineIndex]));
        }
        else
        {
            yield return new WaitForSeconds(2f);
            root.AddToClassList("TextBoxHidden");
        }
    }
}
