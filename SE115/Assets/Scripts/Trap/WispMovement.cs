using UnityEngine;

public enum WispMovementMode
{
    PathLoop,
    Circle
}

public class WispMovement : MonoBehaviour
{
    [Header("General Settings")]
    public WispMovementMode mode = WispMovementMode.PathLoop;
    public float speed = 3.0f;
    public float rotationOffset = 0f;

    private Vector2 startPosition;
    private Vector3 originalScale;

    [Header("Path Loop Settings")]
    public Vector2[] pathWaypointsOffset;
    private Vector2[] waypoints;
    private int currentPointIndex = 0;
    private Vector2 currentTargetPoint;

    [Header("Circle Settings")]
    public Vector2 centerOffset = new Vector2(2, 0);
    public bool clockwise = true;

    private Vector2 centerPoint;
    private float radius;
    private float currentAngle;

    private void Start()
    {
        startPosition = transform.position;
        originalScale = transform.localScale;

        if (mode == WispMovementMode.PathLoop)
        {
            SetupPath();
        }
        else if (mode == WispMovementMode.Circle)
        {
            SetupCircle();
        }
    }

    private void SetupPath()
    {
        if (pathWaypointsOffset == null || pathWaypointsOffset.Length == 0)
        {
            waypoints = new Vector2[1];
            waypoints[0] = startPosition;
        }
        else
        {
            waypoints = new Vector2[pathWaypointsOffset.Length + 1];
            waypoints[0] = startPosition;
            for (int i = 0; i < pathWaypointsOffset.Length; i++)
            {
                waypoints[i + 1] = startPosition + pathWaypointsOffset[i];
            }
        }
        currentTargetPoint = waypoints[1];
        currentPointIndex = 1;
    }

    private void SetupCircle()
    {
        centerPoint = startPosition + centerOffset;
        radius = centerOffset.magnitude;
        Vector2 dirFromCenterToStart = startPosition - centerPoint;
        currentAngle = Mathf.Atan2(dirFromCenterToStart.y, dirFromCenterToStart.x);
    }

    private void FixedUpdate()
    {
        switch (mode)
        {
            case WispMovementMode.PathLoop:
                HandlePathMovement();
                break;
            case WispMovementMode.Circle:
                HandleCircleMovement();
                break;
        }
    }

    private void HandlePathMovement()
    {
        Vector2 nextPos = Vector2.MoveTowards(transform.position, currentTargetPoint, speed * Time.fixedDeltaTime);
        Vector2 direction = nextPos - (Vector2)transform.position;

        PathLoopRotate(direction);

        transform.position = nextPos;

        if (Vector2.Distance(transform.position, currentTargetPoint) < 0.1f)
        {
            NextWaypoint();
        }
    }

    private void NextWaypoint()
    {
        currentPointIndex++;
        if (currentPointIndex >= waypoints.Length) currentPointIndex = 0;
        currentTargetPoint = waypoints[currentPointIndex];
    }

    private void HandleCircleMovement()
    {
        if (radius <= 0.01f) return;

        float angularSpeed = speed / radius;

        if (clockwise)
            currentAngle -= angularSpeed * Time.fixedDeltaTime;
        else
            currentAngle += angularSpeed * Time.fixedDeltaTime;

        float x = centerPoint.x + Mathf.Cos(currentAngle) * radius;
        float y = centerPoint.y + Mathf.Sin(currentAngle) * radius;
        Vector2 nextPos = new Vector2(x, y);

        Vector2 direction = nextPos - (Vector2)transform.position;

        CircleRotate(direction);

        transform.position = nextPos;
    }
    private void PathLoopRotate(Vector2 direction)
    {
        if (direction.sqrMagnitude > 0.0001f)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                transform.rotation = Quaternion.identity;
                if (direction.x > 0)
                    transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
                else
                    transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            }
            else 
            {
                transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
            }
        }
    }
    private void CircleRotate(Vector2 direction)
    {
        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 currentPos = Application.isPlaying ? startPosition : (Vector2)transform.position;

        if (mode == WispMovementMode.PathLoop)
        {
            if (pathWaypointsOffset == null || pathWaypointsOffset.Length == 0) return;
            Gizmos.color = Color.green;
            Vector2 prev = currentPos;
            foreach (var offset in pathWaypointsOffset)
            {
                Vector2 next = currentPos + offset;
                Gizmos.DrawLine(prev, next);
                Gizmos.DrawWireSphere(next, 0.2f);
                prev = next;
            }
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(prev, currentPos);
        }
        else if (mode == WispMovementMode.Circle)
        {
            Vector2 center = currentPos + centerOffset;
            float r = centerOffset.magnitude;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(center, r);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(currentPos, center);
            Gizmos.DrawWireSphere(center, 0.15f);
        }
    }
}