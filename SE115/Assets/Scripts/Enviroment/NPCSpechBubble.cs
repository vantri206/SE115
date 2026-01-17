using UnityEngine;
using System.Collections;
using TMPro;

public class NPCSpeechBubble : MonoBehaviour, IInteractable
{
    [Header("Dialogue Content")]
    [TextArea(3, 5)]
    [SerializeField] private string[] sentences;
    private int currentSentenceIndex = -1;

    [Header("UI References")]
    [SerializeField] private GameObject speechBubble;
    [SerializeField] private TextMeshProUGUI textDisplay;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.04f;
    [SerializeField] private float interactCooldown = 0.2f; 

    private PlayerController playerInRange;
    private bool isTalking = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private float lastInteractTime = -1f;

    private void Start()
    {
        if (speechBubble)
            speechBubble.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange != null)
        {
            if (playerInRange.lastPressedInteractTime > 0.0f)
            {
                if (Time.time - lastInteractTime < interactCooldown)
                {
                    playerInRange.input.ResetInteractPressed();
                    return;
                }

                lastInteractTime = Time.time;

                playerInRange.input.ResetInteractPressed();
                HandleInteraction();
            }
        }
    }

    private void HandleInteraction()
    {
        if (!isTalking)
        {
            OpenDialogue();
        }
        else
        {
            if (isTyping)
            {
                StopTypingAndShowFull();
            }
            else
            {
                NextSentence();
            }
        }
    }

    private void OpenDialogue()
    {
        isTalking = true;
        currentSentenceIndex = -1;

        if (speechBubble) speechBubble.SetActive(true);

        NextSentence();
    }

    private void NextSentence()
    {
        currentSentenceIndex++;

        if (currentSentenceIndex >= sentences.Length)
        {
            CloseDialogue();
            return;
        }

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine(sentences[currentSentenceIndex]));
    }
    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        textDisplay.text = line;
        textDisplay.maxVisibleCharacters = 0;

        foreach (char letter in line.ToCharArray())
        {
            textDisplay.maxVisibleCharacters++; 
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void StopTypingAndShowFull()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        textDisplay.maxVisibleCharacters = int.MaxValue;

        isTyping = false;
    }
    private void CloseDialogue()
    {
        isTalking = false;
        isTyping = false;
        if (speechBubble) speechBubble.SetActive(false);
    }

    public void SetPlayerInRange(PlayerController player)
    {
        playerInRange = player;

        if (player == null)
        {
            CloseDialogue();
        }
    }
}