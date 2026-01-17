using UnityEngine;

public class AutoScrollCamera : MonoBehaviour
{
    [Header("Movement")]
    public float scrollSpeed = 3.5f;
    public bool isScrolling = false;

    private float startY;
    private float startZ;

    private void Start()
    {
        startY = transform.position.y;
        startZ = transform.position.z;

        Invoke(nameof(StartScroll), 1.0f);
    }

    private void Update()
    {
        if (!isScrolling) return;

        float newX = transform.position.x + (scrollSpeed * Time.deltaTime);

        transform.position = new Vector3(newX, startY, startZ);
    }

    public void StartScroll() => isScrolling = true;
    public void StopScroll() => isScrolling = false;
}