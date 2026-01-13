using UnityEngine;

public class CameraTargetController : MonoBehaviour
{
    [Header("Targets")]
    public Transform playerTransform; 

    [Header("Settings")]
    public float followSpeed = 10f; 
    public float snapBackSpeed = 5f;

    [Header("Bounds (Optional)")]
    public Collider2D mapBounds;

    private Transform currentFocus; 
    private bool isSnappingBack = false;

    private void Awake()
    {
        if(playerTransform == null)
        {
            playerTransform = FindFirstObjectByType<PlayerController>().transform;
        }
    }
    private void Start()
    {
        currentFocus = playerTransform;
        transform.position = playerTransform.position;
    }
    private void LateUpdate()
    {
        if (currentFocus == null) return;

        float speed = isSnappingBack ? snapBackSpeed : followSpeed;

        Vector3 targetPos = currentFocus.position;
        targetPos.z = transform.position.z;

        transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);

        if (isSnappingBack && Vector2.Distance(transform.position, targetPos) < 0.5f)
        {
            isSnappingBack = false;
        }
    }
    public void SwitchFocusToSpirit(Transform spiritTransform)
    {
        currentFocus = spiritTransform;
        isSnappingBack = false;
        followSpeed = 20f;
    }

    public void ReturnFocusToPlayer()
    {
        currentFocus = playerTransform;
        isSnappingBack = true; 
        followSpeed = 10f; 
    }
}