using System.Collections;
using UnityEngine;

public enum PlatformMode
{
    Loop,
    OneWay
}

public class MovingPlatform : MonoBehaviour
{
    [Header("Settings")]
    public PlatformMode movementMode = PlatformMode.Loop;
    public float speed = 3.0f;

    public Vector2[] waypointsOffset;
    private Vector2[] waypoints;

    private int currentPointIndex = 0;
    private Vector2 currentPoint;
    private Vector2 startPosition;

    [Header("Wait Time")]
    public float waitAtPoint = 0.5f;
    public float respawnDelay = 3.0f;
    private float waitTimer;

    private SpriteRenderer spriteRenderer;
    private bool isRespawning = false;
    private Collider2D myCollider; 

    private void Start()
    {
        startPosition = transform.position;
        myCollider = GetComponent<Collider2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (waypointsOffset == null || waypointsOffset.Length == 0)
        {
            enabled = false; 
            return;
        }

        waypoints = new Vector2[waypointsOffset.Length + 1];
        waypoints[0] = startPosition;

        for (int i = 0; i < waypointsOffset.Length; i++)
        {
            waypoints[i + 1] = startPosition + waypointsOffset[i];
        }

        currentPoint = waypoints[1];
        currentPointIndex = 1;
    }

    private void FixedUpdate()
    {
        if (isRespawning) return;

        transform.position = Vector2.MoveTowards(transform.position, currentPoint, speed * Time.fixedDeltaTime);

        if (Vector2.Distance(transform.position, currentPoint) < 0.2f)
        {
            if (waitTimer < waitAtPoint)
            {
                waitTimer += Time.fixedDeltaTime;
            }
            else
            {
                NextWaypoint();
            }
        }
    }

    void NextWaypoint()
    {
        waitTimer = 0;
        currentPointIndex++;

        if (currentPointIndex >= waypoints.Length)
        {
            if (movementMode == PlatformMode.Loop)
            {
                currentPointIndex = 0;
            }
            else
            {
                StartCoroutine(RespawnRoutine());
                return;
            }
        }

        currentPoint = waypoints[currentPointIndex];
    }

    IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        if (spriteRenderer)
            spriteRenderer.enabled = false;

        if (myCollider)
            myCollider.enabled = false;

        foreach (Transform child in transform)
        {
            if (child.CompareTag("Player")) child.SetParent(null);
        }

        yield return new WaitForSeconds(respawnDelay);

        transform.position = waypoints[0];
        currentPointIndex = 1;
        currentPoint = waypoints[1];

        if (spriteRenderer)
            spriteRenderer.enabled = true;

        if (myCollider)
            myCollider.enabled = true;

        isRespawning = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(this.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }

    private void OnDrawGizmos()
    {
        if (waypointsOffset == null || waypointsOffset.Length == 0) return;

        Gizmos.color = Color.green;
        Vector2 start = Application.isPlaying ? startPosition : (Vector2)transform.position;
        Vector2 prev = start;

        foreach (var offset in waypointsOffset)
        {
            Vector2 next = start + offset;
            Gizmos.DrawLine(prev, next);
            Gizmos.DrawWireSphere(next, 0.2f);
            prev = next;
        }

        if (movementMode == PlatformMode.Loop)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(prev, start);
        }
    }
}