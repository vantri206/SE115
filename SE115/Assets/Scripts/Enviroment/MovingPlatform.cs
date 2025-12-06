using Unity.VisualScripting;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 3f;
    public Vector2[] waypointsOffset;
    public Vector2[] waypoints;

    private int currentPointIndex = 0;
    private Vector2 currentPoint;
    private Vector2 startPostition;

    [Header("Wait Time")]
    public float waitAtPoint = 0.5f;
    private float waitTimer;

    private void Start()
    {
        startPostition = transform.position;

        waypoints = new Vector2[waypointsOffset.Length];
        for(int i = 0; i < waypointsOffset.Length; i++)
        {
            waypoints[i] = startPostition + waypointsOffset[i];
        }

        currentPoint = waypoints[0];
    }

    private void FixedUpdate()
    {
        if (currentPoint == null) return;
        transform.position = Vector2.MoveTowards(transform.position, currentPoint, speed * Time.fixedDeltaTime);

        if (Vector2.Distance(transform.position, currentPoint) < 0.05f)
        {
            if (waitTimer < waitAtPoint)
            {
                waitTimer += Time.fixedDeltaTime;
            }
            else
            {
                currentPointIndex++;
                if (currentPointIndex >= waypoints.Length) currentPointIndex = 0;

                currentPoint = waypoints[currentPointIndex];
                waitTimer = 0;
            }
        }
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
}