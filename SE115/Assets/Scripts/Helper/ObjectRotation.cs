using UnityEngine;

public class WispRotation : MonoBehaviour
{
    private Vector3 lastPosition;
    private Vector3 originalScale;

    public float moveThreshold = 0.001f;

    private void Start()
    {
        lastPosition = transform.position;
        originalScale = transform.localScale;
    }

    private void LateUpdate()
    {
        Vector3 direction = transform.position - lastPosition;

        if (direction.sqrMagnitude > moveThreshold)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                transform.rotation = Quaternion.identity;

                if (direction.x > 0)
                {
                    transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
                }
                else
                {
                    transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
                }
            }
            else
            {
                transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                angle += 180f;

                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
        lastPosition = transform.position;
    }
}