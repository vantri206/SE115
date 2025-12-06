using UnityEngine;

public class MapArea : MonoBehaviour
{
    public Collider2D areaBounds;
    [SerializeField] private CameraController cam;
    private void Awake()
    {
        if (cam == null)
        {
            cam = FindFirstObjectByType<CameraController>();
        }
        if(areaBounds == null)
        {
            areaBounds = GetComponent<PolygonCollider2D>();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            cam.SwitchArea(this);
        }
    }
}
