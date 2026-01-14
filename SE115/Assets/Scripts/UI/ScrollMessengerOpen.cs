using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ScrollMessenger : MonoBehaviour
{
    public static ScrollMessenger Instance { get; private set; }

    [Header("UI Components")]
    [SerializeField] private RectTransform scroll;
    [SerializeField] private CanvasGroup contentCanvas;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Image messageImage;

    [Header("Animation Settings")]
    [SerializeField] private float openDuration = 1.0f;
    [SerializeField] private float closedWidth = 100f; 
    [SerializeField] private float openedWidth = 600f;

    private Queue<MessageRequest> queue = new Queue<MessageRequest>();
    private bool isShowed = false;
    private bool shouldClose = false;

    private void Awake()
    {
        if (Instance == null) 
            Instance = this;

        scroll.sizeDelta = new Vector2(closedWidth, scroll.sizeDelta.y);

        contentCanvas.alpha = 0f;

        scroll.gameObject.SetActive(false);
    }

    public void ShowMessage(string text, Sprite icon = null)
    {
        queue.Enqueue(new MessageRequest(text, icon));
        if (!isShowed) StartCoroutine(ProcessRoutine());  
    }
    public void CloseMessage()
    {
        if (isShowed)
        {
            shouldClose = true;
        }
    }
    public bool IsShowingMessage()
    {
        return isShowed;
    }
    private IEnumerator ProcessRoutine()
    {
        isShowed = true;
        scroll.gameObject.SetActive(true);

        while (queue.Count > 0)
        {
            MessageRequest request = queue.Dequeue();

            shouldClose = false;

            messageText.text = request.text;
            if (request.image != null)
            {
                messageImage.sprite = request.image;
                messageImage.gameObject.SetActive(true);
            }
            else messageImage.gameObject.SetActive(false);

            yield return StartCoroutine(AnimateWidth(closedWidth, openedWidth));

            yield return StartCoroutine(FadeContent(0.0f, 1.0f));

            while (shouldClose == false)
            {
                yield return null;
            }

            yield return StartCoroutine(FadeContent(1.0f, 0.0f));

            yield return StartCoroutine(AnimateWidth(openedWidth, closedWidth));

            yield return new WaitForSeconds(0.25f); 
        }

        scroll.gameObject.SetActive(false);
        isShowed = false;
    }

    private IEnumerator AnimateWidth(float start, float end)
    {
        float timer = 0.0f;
        while (timer < openDuration)
        {
            timer += Time.deltaTime;
            float percent = timer / openDuration;
            float currentWidth = Mathf.SmoothStep(start, end, percent);

            scroll.sizeDelta = new Vector2(currentWidth, scroll.sizeDelta.y);
            yield return null;
        }
        scroll.sizeDelta = new Vector2(end, scroll.sizeDelta.y);
    }

    private IEnumerator FadeContent(float start, float end)
    {
        float timer = 0f;
        float duration = 0.2f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float percent = timer / duration;
            contentCanvas.alpha = Mathf.Lerp(start, end, percent);
            yield return null;
        }
        contentCanvas.alpha = end;
    }
}

public class MessageRequest
{
    public string text;
    public Sprite image;
    public MessageRequest(string text, Sprite image)
    {
        this.text = text;
        this.image = image;
    }
}
