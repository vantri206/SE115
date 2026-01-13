using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [Header("Movement")]
    public float floatHeight = 1.5f;
    public float moveSpeed = 3f;    

    [Header("Settings")]
    public Color colorA = Color.white;  
    public Color colorB = new Color(0.2f, 1f, 0.2f);
    public float blinkSpeed = 10f;      
    public bool usePopUp = true;    
    public float displayPopupTime = 0.3f;

    private Vector3 targetPos;
    private TextMeshPro tmp;
    private float timeElapsed = 0f;

    void Awake()
    {
        if(tmp == null)
            tmp = GetComponent<TextMeshPro>();
    }

    void Start()
    {
        targetPos = transform.position + Vector3.up * floatHeight;
        if (usePopUp) transform.localScale = Vector3.zero;
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
        if (tmp != null)
        {
            tmp.color = Color.Lerp(colorA, colorB, Mathf.PingPong(Time.time * blinkSpeed, 1f));
        }

        if (usePopUp && timeElapsed < displayPopupTime)
        {
            float scaleProgress = timeElapsed / displayPopupTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, scaleProgress);
        }
    }
    public void SetText(string content)
    {
        if (tmp != null)
            tmp.text = content;
    }
}