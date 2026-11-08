using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Dialogue : Singleton<UI_Dialogue>
{
    [Header("Root")]
    [SerializeField] private GameObject root;

    [Header("Text")]
    [SerializeField] private TMP_Text npcNameText;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Choices")]
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private TMP_Text[] choiceButtonTexts;

    [Header("Typewriter")]
    [SerializeField] private float charactersPerSecond = 40f;
    [SerializeField] private float punctuationPause = 0.15f;

    private Coroutine typewriterRoutine;
    private bool isTyping;

    public bool IsTyping => isTyping;

    protected override void Awake()
    {
        base.Awake();

        ClearChoices();
    }

    private void Start()
    {
        M_InputManager.Instance.SubmitPerformed += OnSubmitPressed;
        M_InputManager.Instance.CancelPerformed += OnCancelPressed;
    }

    private void OnDestroy()
    {
        if (!M_InputManager.HasInstance)
            return;

        M_InputManager.Instance.SubmitPerformed -= OnSubmitPressed;
        M_InputManager.Instance.CancelPerformed -= OnCancelPressed;
    }

    public void Show()
    {
        root.SetActive(true);
    }

    public void Hide()
    {
        ClearChoices();

        if (typewriterRoutine != null)
        {
            StopCoroutine(typewriterRoutine);
            typewriterRoutine = null;
        }

        dialogueText.text = "";
        dialogueText.maxVisibleCharacters = int.MaxValue;

        npcNameText.text = "";

        isTyping = false;

        root.SetActive(false);
    }

    public void ShowLine(string speakerName, string line)
    {
        npcNameText.text = speakerName;

        ClearChoices();

        if (typewriterRoutine != null)
            StopCoroutine(typewriterRoutine);

        typewriterRoutine = StartCoroutine(TypewriterRoutine(line));
    }

    private IEnumerator TypewriterRoutine(string line)
    {
        isTyping = true;

        dialogueText.text = line;
        dialogueText.ForceMeshUpdate();

        int totalCharacters = dialogueText.textInfo.characterCount;

        dialogueText.maxVisibleCharacters = 0;

        for (int i = 0; i <= totalCharacters; i++)
        {
            dialogueText.maxVisibleCharacters = i;

            char currentCharacter = '\0';

            if (i > 0)
            {
                int visibleIndex = i - 1;

                if (visibleIndex < dialogueText.textInfo.characterCount)
                    currentCharacter = dialogueText.textInfo.characterInfo[visibleIndex].character;
            }

            float waitTime = 1f / charactersPerSecond;

            switch (currentCharacter)
            {
                case '.':
                case ',':
                case '!':
                case '?':
                    waitTime += punctuationPause;
                    break;
            }

            yield return new WaitForSecondsRealtime(waitTime);
        }

        dialogueText.maxVisibleCharacters = int.MaxValue;

        isTyping = false;
        typewriterRoutine = null;
    }

    private void SkipTypewriter()
    {
        if (!isTyping)
            return;

        if (typewriterRoutine != null)
        {
            StopCoroutine(typewriterRoutine);
            typewriterRoutine = null;
        }

        dialogueText.maxVisibleCharacters = int.MaxValue;

        isTyping = false;
    }

    private void OnSubmitPressed()
    {
        if (!M_DialogueManager.Instance.IsDialogueOpen)
            return;

        if (HasVisibleChoices())
            return;

        AdvanceRequested();
    }

    private void OnCancelPressed()
    {
        if (!M_DialogueManager.Instance.IsDialogueOpen)
            return;

        // Optional:
        // M_DialogueManager.Instance.EndDialogue();
    }

    public void AdvanceRequested()
    {
        if (isTyping)
        {
            SkipTypewriter();
            return;
        }

        M_DialogueManager.Instance.ContinueStory();
    }

    private bool HasVisibleChoices()
    {
        foreach (Button button in choiceButtons)
        {
            if (button != null && button.gameObject.activeSelf)
                return true;
        }

        return false;
    }

    public void ShowChoices(List<Choice> choices)
    {
        ClearChoices();

        if (choices.Count > choiceButtons.Length)
        {
            Debug.LogWarning(
                $"Dialogue has {choices.Count} choices but only {choiceButtons.Length} buttons are available.");
        }

        int choiceCount = Mathf.Min(choices.Count, choiceButtons.Length);

        for (int i = 0; i < choiceCount; i++)
        {
            int choiceIndex = i;

            Button button = choiceButtons[i];

            if (button == null)
                continue;

            button.gameObject.SetActive(true);

            if (i < choiceButtonTexts.Length &&
                choiceButtonTexts[i] != null)
            {
                choiceButtonTexts[i].text = choices[i].text;
            }

            button.onClick.RemoveAllListeners();

            button.onClick.AddListener(() =>
            {
                M_DialogueManager.Instance.SelectChoice(choiceIndex);
            });
        }

        for (int i = choiceCount; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] == null)
                continue;

            Navigation nav = choiceButtons[i].navigation;

            nav.selectOnUp = null;
            nav.selectOnDown = null;

            choiceButtons[i].navigation = nav;
        }

        SetupNavigation(choiceCount);
    }

    private void SetupNavigation(int activeButtonCount)
    {
        if (activeButtonCount == 0)
            return;

        for (int i = 0; i < activeButtonCount; i++)
        {
            Button button = choiceButtons[i];

            if (button == null)
                continue;

            Navigation nav = button.navigation;

            nav.mode = Navigation.Mode.Explicit;

            nav.selectOnUp = i > 0
                ? choiceButtons[i - 1]
                : choiceButtons[activeButtonCount - 1];

            nav.selectOnDown = i < activeButtonCount - 1
                ? choiceButtons[i + 1]
                : choiceButtons[0];

            button.navigation = nav;
        }

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(choiceButtons[0].gameObject);
    }

    public void ClearChoices()
    {
        foreach (Button button in choiceButtons)
        {
            if (button == null)
                continue;

            button.onClick.RemoveAllListeners();
            button.gameObject.SetActive(false);
        }

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }
}