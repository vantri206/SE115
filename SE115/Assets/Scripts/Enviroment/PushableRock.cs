using UnityEngine;

public class PushableRock : MonoBehaviour
{
    public float activeRange = 15.0f;
    public float respawnDelay = 1.0f;

    private Vector3 startPosition;
    private Rigidbody2D rb;
    private Transform playerTransform;
    private float timer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj) playerTransform = playerObj.transform;
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float playerDistance = Vector2.Distance(transform.position, playerTransform.position);
        float distance = Vector2.Distance(transform.position, startPosition);

        if (distance > activeRange || playerDistance > activeRange)
        {
            timer += Time.deltaTime;

            if (timer > respawnDelay)
            {
                RespawnStone();
            }
        }
        else
        {
            timer = 0;
        }
    }

    private void RespawnStone()
    {
        transform.position = startPosition;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        timer = 0;

        Debug.Log("Stone Respawned!");
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activeRange);
    }
}