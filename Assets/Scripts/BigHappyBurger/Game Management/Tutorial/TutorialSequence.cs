using System;
using System.Collections;
using System.Collections.Generic;
using BigHappyBurger.Customers;
using BigHappyBurger.Foods;
using BigHappyBurger.Interaction;
using BigHappyBurger.Players;
using BigHappyBurger.Restaurants;
using Shears;
using Shears.Signals;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace BigHappyBurger.GameManagement
{
    public class TutorialSequence : MonoBehaviour
    {
        #region Variables
        private const float CUSTOMER_COUNTER = 0.0f;
        private const float ORDER_COUNTER = 90.0f;
        private const float CHEF_COUNTER = 180.0f;
        private const float DRINK_COUNTER = 270.0f;
        private const float TIME_PER_CHARACTER = 0.03f;

        [Header("Managers")]
        [SerializeField, Required]
        private Player player;

        [SerializeField, Required]
        private Restaurant restaurant;

        [SerializeField, Required]
        private Chef chef;

        [SerializeField, Required]
        private CustomerManager customerManager;

        [SerializeField, Required]
        private ItemSpawner bagSpawner;

        [SerializeField, Required]
        private ItemSpawner mediumCupSpawner;

        [SerializeField, Required]
        private ItemSpawner lidSpawner;

        [SerializeField, Required]
        private RestockMonitor monitor;

        [SerializeField, Required]
        private DrinkMachine drinkMachine;

        [Header("Items")]
        [SerializeField, Required]
        private Bag bag;

        [SerializeField, Required]
        private Food fries;

        [SerializeField, Required]
        private Food ketchup;

        [SerializeField, Required]
        private Food napkins;

        [SerializeField, Required]
        private DrinkType grapeDrink;

        [SerializeField, Required(targetCollectionSize: 1)]
        private ItemSpawner[] itemSpawners = Array.Empty<ItemSpawner>();

        [SerializeField, Required(targetCollectionSize: 3)]
        private ItemSpawnContainer[] itemSpawnContainers = Array.Empty<ItemSpawnContainer>();

        [Header("Tutorial")]
        [SerializeField]
        private PanelRenderer tutorialPanel;

        [SerializeField]
        private SerializableDictionary<int, float> lineTimeOverrides = new();

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

        [SerializeField]
        private List<PointingHandAnimation> pointers = new();

        private readonly List<Food> foods = new();
        private readonly List<DrinkTypeSize> drinks = new();
        private readonly int day = 1; // obviously shouldn't be readonly if this was actually implemented
        private int lineIndex = 0;
        private bool bagHasFries = false;
        private bool baghasKetchup = false;
        private bool bagHasNapkins = false;
        private bool isKetchupOrdered = false;
        private bool isFinished = false;
        private Customer customer;
        private Bag spawnedBag;
        private Drinkable spawnedCup;
        private VisualElement root;
        private Label tutorialText;

        private PlayerInputActions.PlayerActions PlayerActions => player.Input.PlayerActions;

        public event Action TutorialFinished;
        public event Action HappyBoxEnabled;
        #endregion

        private void OnEnable()
        {
            tutorialPanel.RegisterUIReloadCallback(OnUIReload);
            SignalShuttle.Register<StringSignal>(OnStringSignal);
        }

        private void OnDisable()
        {
            tutorialPanel.UnregisterUIReloadCallback(OnUIReload);
            SignalShuttle.Deregister<StringSignal>(OnStringSignal);
        }

        private void OnStringSignal(StringSignal signal)
        {
            if (signal.Value == "Paused")
                root.style.display = DisplayStyle.None;
            else if (signal.Value == "Unpaused")
                root.style.display = DisplayStyle.Flex;
        }

        private void OnUIReload(PanelRenderer _, VisualElement rootElement)
        {
            root = rootElement.Q<VisualElement>("TutorialTextbox");
            tutorialText = rootElement.Q<Label>("DialogueText");
        }

        private void Start()
        {
            if (isFinished)
                return;

            DisableAllTurning();
            DisableAllSpawners();

            StartCoroutine(EnterScreen());

            bagSpawner.ItemSpawned += OnBagSpawned;
            mediumCupSpawner.ItemSpawned += OnCupSpawned;
            monitor.RestockOrdered += OnRestockOrdered;
            monitor.Clickable = false;
            monitor.CanChangeSelection = false;
        }

        private IEnumerator EnterScreen()
        {
            yield return CoroutineUtil.WaitForSeconds(1.0f);

            root.RemoveFromClassList("TextBoxHidden");

            yield return CoroutineUtil.WaitForSeconds(1.0f);

            switch (day)
            {
                case 1:
                    StartCoroutine(TutorialDialogue1(tutorialLines1[lineIndex]));
                    break;
                case 2:
                    StartCoroutine(TutorialDialogue2(tutorialLines2[lineIndex]));
                    break;
                case 3:
                    StartCoroutine(TutorialDialogue3(tutorialLines3[lineIndex]));
                    break;
                case 4:
                    StartCoroutine(TutorialDialogue4(tutorialLines4[lineIndex]));
                    break;
                case 5:
                    StartCoroutine(TutorialDialogue5(tutorialLines5[lineIndex]));
                    break;
            }
        }

        private IEnumerator TutorialDialogue1(string fullText)
        {
            int totalLength = fullText.Length;
            tutorialText.text = string.Empty;

            for (int i = 0; i <= totalLength; i++)
            {
                string visible = fullText[..i];
                string hidden = fullText[i..];

                tutorialText.text = $"{visible}<alpha=#00>{hidden}";

                yield return CoroutineUtil.WaitForSeconds(0.02f);
            }

            if (lineTimeOverrides.TryGetValue(lineIndex, out float overrideTime))
                yield return CoroutineUtil.WaitForSeconds(overrideTime);
            else
                yield return CoroutineUtil.WaitForSeconds(TIME_PER_CHARACTER * fullText.Length);

            tutorialText.text = fullText;
            lineIndex++;

            if (lineIndex < tutorialLines1.Count)
            {
                switch (lineIndex) //CHECKS TO PROCEED TO NEXT DIALOGUE LINE
                {
                    case 2:
                        // Add first customer to board, always fries and ketchup
                        // Finger points to order on board

                        foods.Clear();
                        foods.Add(fries);
                        foods.Add(ketchup);

                        customer = customerManager.CreateCustomer();
                        var order = new Order(foods, hasTimer: false);
                        customer.SetOrder(order);
                        restaurant.AddOrder(order);

                        customerManager.SpawnCustomer();

                        pointers[0].gameObject.SetActive(true);

                        break;
                    case 4:
                        // Remove finger after a couple lines of dialogue

                        pointers[0].gameObject.SetActive(false);
                        break;
                    case 5:
                        // Enable right turning
                        EnableSingleRightTurn();

                        break;
                    case 6:
                        // Wait until player has turned
                        // Finger points at chef sign

                        while (!IsPlayerLookingAtCounter(CHEF_COUNTER))
                            yield return null;

                        pointers[1].gameObject.SetActive(true);

                        break;
                    case 7:
                        // Finger points at bag spawner

                        EnableSpawner(bag.ID);
                        player.EnableInteraction();

                        pointers[1].gameObject.SetActive(false);
                        pointers[2].gameObject.SetActive(true);
                        break;
                    case 8:
                        // Check if the player has grabbed a bag at least once

                        while (spawnedBag == null)
                            yield return null;

                        DisableSpawner(bag.ID);
                        pointers[2].gameObject.SetActive(false);
                        break;
                    case 9:
                        // Check if the player has put a bag on the counter at least once

                        while (player.Interactor.DragInteractor.Item == spawnedBag)
                            yield return null;

                        yield return CoroutineUtil.WaitForSeconds(0.5f);

                        foreach (var slot in chef.Slots)
                        {
                            if (slot.Cookable.Food.ID == fries.ID)
                            {
                                chef.ForceComplete(slot);
                                break;
                            }
                        }

                        break;
                    case 10:
                        root.AddToClassList("TextBoxAbove");
                        break;
                    case 11:
                        while (!bagHasFries)
                            yield return null;

                        break;
                    case 12:
                        // Check if the player has gone back to the order counter
                        EnableSingleLeftRightTurns();

                        break;
                    case 13:
                        while (!IsPlayerLookingAtCounter(ORDER_COUNTER))
                            yield return null;

                        pointers[3].gameObject.SetActive(true);
                        break;
                    case 14:
                        // Check to see if cursor is over the buy button on the restock comp

                        while (!monitor.IsHoveringBuyButton)
                            yield return null;

                        monitor.Clickable = true;

                        pointers[3].gameObject.SetActive(false);
                        pointers[4].gameObject.SetActive(true);

                        break;
                    case 15:
                        // Wait until player purchases ketchup

                        while (!isKetchupOrdered)
                            yield return null;

                        pointers[4].gameObject.SetActive(false);

                        break;
                    case 18:
                        EnableSpawner(ketchup.ID);

                        break;
                    case 19:
                        // Reenable turning and allow player to pick up ketchup

                        monitor.CanChangeSelection = true;

                        while (!IsPlayerLookingAtCounter(CHEF_COUNTER))
                            yield return null;

                        while (!baghasKetchup)
                            yield return null;

                        break;
                    case 20:
                        // Enable napkin spawning
                        // Finger points at napkin spawner

                        EnableSpawner(napkins.ID);
                        pointers[5].gameObject.SetActive(true);
                        break;
                    case 21:
                        // Wait for player to put napkins in the bag

                        while (!bagHasNapkins)
                            yield return null;

                        pointers[5].gameObject.SetActive(false);

                        EnableAllTurning();

                        break;
                    case 22:
                        // Enable full control

                        while (!IsPlayerLookingAtCounter(CUSTOMER_COUNTER))
                            yield return null;

                        DisableAllTurning();
                        root.RemoveFromClassList("TextBoxAbove");

                        break;
                    case 23:
                        // Wait for customer order to be completed

                        while (!customer.IsExiting)
                            yield return null;

                        EnableAllTurning();

                        break;
                    case 24:
                        // Add second customer to board, always a medium drink

                        foods.Clear();
                        drinks.Clear();
                        drinks.Add(new(grapeDrink, Drinkable.Size.Medium));

                        customer = customerManager.CreateCustomer();
                        var drinkOrder = new Order(drinks, hasTimer: false);
                        customer.SetOrder(drinkOrder);
                        restaurant.AddOrder(drinkOrder);

                        customerManager.SpawnCustomer();

                        break;
                    case 26:
                        // Wait for player to look at the order screen

                        while (!IsPlayerLookingAtCounter(ORDER_COUNTER))
                            yield return null;

                        break;
                    case 27:
                        // Wait for player to look at the drink station

                        while (!IsPlayerLookingAtCounter(DRINK_COUNTER))
                            yield return null;

                        mediumCupSpawner.CanSpawn = true;
                        pointers[6].gameObject.SetActive(true);

                        break;
                    case 28:
                        // Wait for player to spawn a cup and fill it

                        lidSpawner.CanSpawn = true;

                        while (spawnedCup == null || !spawnedCup.IsFull)
                            yield return null;

                        pointers[6].gameObject.SetActive(false);
                        pointers[7].gameObject.SetActive(true);
                        root.AddToClassList("TextBoxAbove");

                        break;
                    case 29:
                        // Check for lid on drink

                        while (!spawnedCup.HasLid)
                            yield return null;

                        pointers[7].gameObject.SetActive(false);

                        break;
                    case 30:
                        // Complete customer order #2

                        while (!customer.IsExiting)
                            yield return null;

                        root.RemoveFromClassList("TextBoxAbove");

                        break;
                    case 31:
                        // Navigate back to order screen
                        // Begin actually adding customers

                        while (!IsPlayerLookingAtCounter(ORDER_COUNTER))
                            yield return null;

                        FinishTutorial();
                        break;
                    case 32:
                        pointers[8].gameObject.SetActive(true);
                        break;
                    case 33:
                        pointers[8].gameObject.SetActive(false);
                        break;
                }

                StartCoroutine(TutorialDialogue1(tutorialLines1[lineIndex]));
            }
            else
            {
                root.AddToClassList("TextBoxHidden");
                lineIndex = 0;

                yield return CoroutineUtil.WaitForSeconds(30.0f);
                root.RemoveFromClassList("TextBoxHidden");

                tutorialText.text = string.Empty;
                yield return CoroutineUtil.WaitForSeconds(0.5f);
                StartCoroutine(TutorialDialogue2(tutorialLines2[lineIndex]));
            }
        }

        private IEnumerator TutorialDialogue2(string fullText)
        {
            int totalLength = fullText.Length;
            tutorialText.text = string.Empty;

            for (int i = 0; i <= totalLength; i++)
            {
                string visible = fullText[..i];
                string hidden = fullText[i..];

                tutorialText.text = $"{visible}<alpha=#00>{hidden}";

                yield return CoroutineUtil.WaitForSeconds(0.02f);
            }

            yield return CoroutineUtil.WaitForSeconds(TIME_PER_CHARACTER * fullText.Length);

            tutorialText.text = fullText;
            lineIndex++;

            if (lineIndex < tutorialLines2.Count)
                StartCoroutine(TutorialDialogue2(tutorialLines2[lineIndex]));
            else
            {
                root.AddToClassList("TextBoxHidden");
                lineIndex = 0;

                yield return CoroutineUtil.WaitForSeconds(120.0f);
                root.RemoveFromClassList("TextBoxHidden");

                tutorialText.text = string.Empty;
                yield return CoroutineUtil.WaitForSeconds(0.5f);
                StartCoroutine(TutorialDialogue4(tutorialLines4[lineIndex]));
            }
        }

        private IEnumerator TutorialDialogue3(string fullText)
        {
            int totalLength = fullText.Length;
            tutorialText.text = string.Empty;

            for (int i = 0; i <= totalLength; i++)
            {
                string visible = fullText[..i];
                string hidden = fullText[i..];

                tutorialText.text = $"{visible}<alpha=#00>{hidden}";

                yield return CoroutineUtil.WaitForSeconds(0.02f);
            }

            tutorialText.text = fullText;
            lineIndex++;

            if (lineIndex < tutorialLines3.Count)
            {
                while (!Mouse.current.leftButton.wasPressedThisFrame)
                    yield return null;

                StartCoroutine(TutorialDialogue3(tutorialLines3[lineIndex]));
            }
            else
            {
                yield return CoroutineUtil.WaitForSeconds(2f);
                root.AddToClassList("TextBoxHidden");
                lineIndex = 0;
            }
        }

        private IEnumerator TutorialDialogue4(string fullText)
        {
            int totalLength = fullText.Length;
            tutorialText.text = string.Empty;

            for (int i = 0; i <= totalLength; i++)
            {
                string visible = fullText[..i];
                string hidden = fullText[i..];

                tutorialText.text = $"{visible}<alpha=#00>{hidden}";

                yield return CoroutineUtil.WaitForSeconds(0.02f);
            }

            yield return CoroutineUtil.WaitForSeconds(TIME_PER_CHARACTER * fullText.Length);

            tutorialText.text = fullText;
            lineIndex++;

            if (lineIndex < tutorialLines4.Count)
                StartCoroutine(TutorialDialogue4(tutorialLines4[lineIndex]));
            else
            {
                root.AddToClassList("TextBoxHidden");
                lineIndex = 0;

                HappyBoxEnabled?.Invoke();
            }
        }

        private IEnumerator TutorialDialogue5(string fullText)
        {
            int totalLength = fullText.Length;
            tutorialText.text = string.Empty;

            for (int i = 0; i <= totalLength; i++)
            {
                string visible = fullText[..i];
                string hidden = fullText[i..];

                tutorialText.text = $"{visible}<alpha=#00>{hidden}";

                yield return CoroutineUtil.WaitForSeconds(0.02f);
            }

            tutorialText.text = fullText;
            lineIndex++;

            if (lineIndex < tutorialLines5.Count)
            {
                while (!Mouse.current.leftButton.wasPressedThisFrame)
                    yield return null;

                StartCoroutine(TutorialDialogue5(tutorialLines5[lineIndex]));
            }
            else
            {
                yield return CoroutineUtil.WaitForSeconds(2f);
                root.AddToClassList("TextBoxHidden");
                lineIndex = 0;
            }
        }

        private void EnableSingleLeftTurn()
        {
            EnableLeftTurning();

            void performed(InputAction.CallbackContext _)
            {
                DisableLeftTurning();
                PlayerActions.RotateLeft.performed -= performed;
            }

            PlayerActions.RotateLeft.performed += performed;
        }

        private void EnableSingleRightTurn()
        {
            EnableRightTurning();

            void performed(InputAction.CallbackContext _)
            {
                DisableRightTurning();
                PlayerActions.RotateRight.performed -= performed;
            }

            PlayerActions.RotateRight.performed += performed;
        }

        private void EnableSingleLeftRightTurns()
        {
            EnableLeftTurning();
            PlayerActions.RotateLeft.performed += OnSingleLeftPerformed;
            PlayerActions.RotateRight.performed += OnSingleRightPerformed;
        }

        private void DisableSingleLeftRightTurns()
        {
            PlayerActions.RotateLeft.performed -= OnSingleLeftPerformed;
            PlayerActions.RotateRight.performed -= OnSingleRightPerformed;
        }

        private void OnSingleLeftPerformed(InputAction.CallbackContext _)
        {
            DisableLeftTurning();
            EnableRightTurning();
        }

        private void OnSingleRightPerformed(InputAction.CallbackContext _)
        {
            DisableRightTurning();
            EnableLeftTurning();
        }

        private void EnableLeftTurning()
        {
            PlayerActions.RotateLeft.Enable();
        }

        private void DisableLeftTurning()
        {
            PlayerActions.RotateLeft.Disable();
        }

        private void EnableRightTurning()
        {
            PlayerActions.RotateRight.Enable();
        }

        private void DisableRightTurning()
        {
            PlayerActions.RotateRight.Disable();
        }

        private void EnableAllTurning()
        {
            DisableAllTurning();
            EnableLeftTurning();
            EnableRightTurning();
        }

        private void DisableAllTurning()
        {
            DisableLeftTurning();
            DisableRightTurning();
            DisableSingleLeftRightTurns();
        }

        private void OnBagSpawned(Item bag)
        {
            spawnedBag = bag.GetComponent<Bag>();

            bagSpawner.ItemSpawned -= OnBagSpawned;
            spawnedBag.FoodHeld += OnBagHoldFood;

            bagSpawner.CanSpawn = false;
        }

        private void OnBagHoldFood(Food food)
        {
            if (food.ID == fries.ID)
                bagHasFries = true;
            else if (food.ID == ketchup.ID)
                baghasKetchup = true;
            else if (food.ID == napkins.ID)
                bagHasNapkins = true;
        }

        private void OnRestockOrdered(Item item)
        {
            if (!bagHasFries)
                return;

            if (!item.TryGetComponent(out Food food))
                return;

            if (food.ID == ketchup.ID)
            {
                isKetchupOrdered = true;
                monitor.RestockOrdered -= OnRestockOrdered;
            }
        }

        private void DisableAllSpawners()
        {
            foreach (var spawner in itemSpawners)
                spawner.CanSpawn = false;

            foreach (var spawner in itemSpawnContainers)
                spawner.CanSpawn = false;
        }

        private void EnableSpawner(string targetID)
        {
            foreach (var spawner in itemSpawners)
            {
                if (spawner.ItemToSpawn.ID == targetID)
                {
                    spawner.CanSpawn = true;
                    break;
                }
            }

            foreach (var spawner in itemSpawnContainers)
            {
                if (spawner.ItemToSpawn.ID == targetID)
                {
                    spawner.CanSpawn = true;
                    break;
                }
            }
        }

        private void DisableSpawner(string targetID)
        {
            foreach (var spawner in itemSpawners)
            {
                if (spawner.ItemToSpawn.ID == targetID)
                {
                    spawner.CanSpawn = false;
                    break;
                }
            }

            foreach (var spawner in itemSpawnContainers)
            {
                if (spawner.ItemToSpawn.ID == targetID)
                {
                    spawner.CanSpawn = false;
                    break;
                }
            }
        }

        private bool IsPlayerLookingAtCounter(float targetRotation)
        {
            float distance = Mathf.Abs(player.Camera.transform.localEulerAngles.y - targetRotation);

            return distance < 0.01f;
        }

        private void OnCupSpawned(Item cup)
        {
            spawnedCup = cup.GetComponent<Drinkable>();

            mediumCupSpawner.ItemSpawned -= OnCupSpawned;
            mediumCupSpawner.CanSpawn = false;
        }

        public void ForceFinishTutorial()
        {
            if (isFinished)
                return;

            StopAllCoroutines();
            root.AddToClassList("TextBoxHidden");
            lineIndex = 0;

            FinishTutorial();
        }

        private void FinishTutorial()
        {
            player.EnableInteraction();
            EnableAllTurning();
            monitor.CanChangeSelection = true;
            monitor.Clickable = true;

            bagSpawner.ItemSpawned -= OnBagSpawned;
            mediumCupSpawner.ItemSpawned -= OnCupSpawned;
            monitor.RestockOrdered -= OnRestockOrdered;

            foreach (var pointer in pointers)
                pointer.gameObject.SetActive(false);

            foreach (var spawner in itemSpawners)
                spawner.CanSpawn = true;

            foreach (var spawner in itemSpawnContainers)
                spawner.CanSpawn = true;

            isFinished = true;
            TutorialFinished?.Invoke();
            HappyBoxEnabled?.Invoke();
        }
    }
}
