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

    [SerializeField]
    private List<string> tutorialLines2 = new();

    [SerializeField]
    private List<string> tutorialLines3 = new();

    [SerializeField]
    private List<string> tutorialLines4 = new();

    [SerializeField]
    private List<string> tutorialLines5 = new();

    private int lineIndex = 0;

    private Label tutorialText;

    private VisualElement root;

    private int day = 1;

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

        switch (day)
        {
            case 1:
                dialogue1 = StartCoroutine(TutorialDialogue1(tutorialLines1[lineIndex]));
                break;
            case 2:
                dialogue1 = StartCoroutine(TutorialDialogue2(tutorialLines2[lineIndex]));
                break;
            case 3:
                dialogue1 = StartCoroutine(TutorialDialogue3(tutorialLines3[lineIndex]));
                break;
            case 4:
                dialogue1 = StartCoroutine(TutorialDialogue4(tutorialLines4[lineIndex]));
                break;
            case 5:
                dialogue1 = StartCoroutine(TutorialDialogue5(tutorialLines5[lineIndex]));
                break;
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
            switch (lineIndex) //CHECKS TO PROCEED TO NEXT DIALOGUE LINE
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

                case 13:
                    //check if the player has gone back to the order counter
                    break;

                case 14:
                    //check to see if cursor is over the buy button on the restock comp
                    break;

                case 15:
                    //wait until player purchases napkins
                    break;

                case 16:
                    break;

                case 17:
                    break;

                case 18:
                    break;

                case 19:
                    //wait for player to navigate back to hotplate
                    break;

                case 21:
                    //wait for player to put napkins in the bag
                    break;

                case 22:
                    //wait for navigation to customer window WITH bag of food
                    break;

                case 23:
                    //wait for customer order to be completed
                    break;

                case 25:
                    //check to see if player is looking at order screen
                    break;

                case 27:
                    //check for navigation 
                    break;

                case 28:
                    //check for drink being filled
                    break;

                case 29:
                    //check for lid on drink
                    break;

                case 30:
                    //complete customer order #2
                    break;

                case 31:
                    //navigate back to order screen
                    break;

                default:
                    while (!Mouse.current.leftButton.wasPressedThisFrame)
                        yield return null;
                    break;
            }

            //DO AT THE START OF THE NEXT LINE OF DIALOGUE

            if (lineIndex == 2)
            {
                //add first customer to board, always fries and ketchup
                //finger points to order on board
            }

            if (lineIndex == 4)
                //teleport finger out of sight

                if (lineIndex == 6)
                {
                    //finger points at now cooking board
                }

            if (lineIndex == 7)
            {
                //finger points at bag holder
            }

            if (lineIndex == 8)
            {
                //finger disappears
            }

            if (lineIndex == 10)
            {
                root.AddToClassList("TextBoxAbove");
            }

            if (lineIndex == 13)
            {
                //disable rotating for the player
                //disable buy button on restock comptuer for everything except ketchup
                //finger point at joystick
            }

            if (lineIndex == 14)
            {
                //finger point at button
            }

            if (lineIndex == 15)
            {
                //finger disappears
            }

            if (lineIndex == 18)
            {
                //reenable rotating
                //reenable buy button for everything on the computer
            }

            if (lineIndex == 20)
            {
                //finger point at napkins
            }

            if (lineIndex == 21)
            {
                //finger disappears
            }

            if (lineIndex == 23)
            {
                root.RemoveFromClassList("TextBoxAbove");
                yield return new WaitForSeconds(1.0f);
            }

            if (lineIndex == 24)
            {
                //add second customer to board, always a medium drink
            }

            if (lineIndex == 27)
            {
                //finger points at cups
            }

            if (lineIndex == 28)
            {
                //finger points at lids
                root.AddToClassList("TextBoxAbove");
            }

            if (lineIndex == 29)
            {
                //finger disappears
            }

            if (lineIndex == 30)
            {
                root.RemoveFromClassList("TextBoxAbove");
                yield return new WaitForSeconds(1.0f);
            }

            if (lineIndex == 31)
            {
                //add third customer to board
            }

            if (lineIndex == 32)
            {
                //hand point to timer on order
            }

            if (lineIndex == 33)
            {
                //finger disappears
            }

            dialogue1 = StartCoroutine(TutorialDialogue1(tutorialLines1[lineIndex]));
        }
        else
        {
            yield return new WaitForSeconds(2f);
            root.AddToClassList("TextBoxHidden");
            lineIndex = 0;
        }
    }

    private IEnumerator TutorialDialogue2(string fullText)
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

        if (lineIndex < tutorialLines2.Count)
        {
            while (!Mouse.current.leftButton.wasPressedThisFrame)
                yield return null;

            dialogue1 = StartCoroutine(TutorialDialogue2(tutorialLines2[lineIndex]));
        } else
        {
            yield return new WaitForSeconds(2f);
            root.AddToClassList("TextBoxHidden");
            lineIndex = 0;
        }
    }

    private IEnumerator TutorialDialogue3(string fullText)
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

        if (lineIndex < tutorialLines3.Count)
        {
            while (!Mouse.current.leftButton.wasPressedThisFrame)
                yield return null;

            dialogue1 = StartCoroutine(TutorialDialogue3(tutorialLines3[lineIndex]));
        }
        else
        {
            yield return new WaitForSeconds(2f);
            root.AddToClassList("TextBoxHidden");
            lineIndex = 0;
        }
    }

    private IEnumerator TutorialDialogue4(string fullText)
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

        if (lineIndex < tutorialLines4.Count)
        {
            while (!Mouse.current.leftButton.wasPressedThisFrame)
                yield return null;

            dialogue1 = StartCoroutine(TutorialDialogue4(tutorialLines4[lineIndex]));
        }
        else
        {
            yield return new WaitForSeconds(2f);
            root.AddToClassList("TextBoxHidden");
            lineIndex = 0;
        }
    }

    private IEnumerator TutorialDialogue5(string fullText)
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

        if (lineIndex < tutorialLines5.Count)
        {
            while (!Mouse.current.leftButton.wasPressedThisFrame)
                yield return null;

            dialogue1 = StartCoroutine(TutorialDialogue5(tutorialLines5[lineIndex]));
        }
        else
        {
            yield return new WaitForSeconds(2f);
            root.AddToClassList("TextBoxHidden");
            lineIndex = 0;
        }
    }
}
