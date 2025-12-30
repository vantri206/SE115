using UnityEngine;

public class FloatingObjectEffect : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float floatSpeed = 5.0f;
    [SerializeField] private float floatDistance = 1.0f;

    private Vector3 startPosition;

    private void Awake()
    {
        startPosition = transform.localPosition;
    }

    private void OnEnable()
    {
        transform.localPosition = startPosition;
    }

    private void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatDistance;

        transform.localPosition = new Vector3(startPosition.x, newY, startPosition.z);
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}