using UnityEngine;
using System.Collections;

public class FallingTrap : TriggerReceiver
{
    [Header("Trap Settings")]
    [SerializeField] private float fallDistance = 20f;
    [SerializeField] private float fallSpeed = 60f; 
    [SerializeField] private float resetSpeed = 20f;  
    [SerializeField] private float delayBeforeFall = 1.0f;
    [SerializeField] private float stayAtBottomTime = 1.0f;
    [SerializeField] private float shakeSpeed = 15f;
    [SerializeField] private float shakeAmount = 0.05f;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform spriteTransform;

    private Vector2 startPos;
    private Vector2 targetPos;
    private bool isFalling = false;

    private float timer = 0f;

    private void Start()
    {
        startPos = transform.position;
        targetPos = startPos - new Vector2(0, fallDistance);

        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    protected override void OnTriggerActive()
    {
        if (isFalling) return;
        StartCoroutine(TrapRoutine());
    }

    protected override void OnTriggerInactive() { }

    IEnumerator TrapRoutine()
    {
        isFalling = true;

        timer = 0.0f;

        while (timer < delayBeforeFall)
        {
            float xOffset = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;

            spriteTransform.localPosition = new Vector3(xOffset, 0, 0);

            timer += Time.deltaTime;
            yield return null;
        }

        spriteTransform.localPosition = Vector3.zero;

        while (Vector2.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, fallSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        yield return new WaitForSeconds(stayAtBottomTime);

        while (Vector2.Distance(transform.position, startPos) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, startPos, resetSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = startPos; 

        isFalling = false;
        isActivated = false;
    }
}