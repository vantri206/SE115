using UnityEngine;
using UnityEngine.WSA;

public class CrusherTrap : MonoBehaviour
{
    [Header("Settings")]
    public Transform leftWall;
    public Transform rightWall;
    public Transform centerPoint;

    public float crusherSpeed = 2f;
    public float delayReset = 1f;

    private bool isActive = false;
    private Vector3 leftWallStartPos;
    private Vector3 rightWallStartPos;

    private void Start()
    {
        leftWallStartPos = leftWall.position;
        rightWallStartPos = rightWall.position;
    }

    private void Update()
    {
        if (isActive)
        {
            leftWall.position = Vector3.MoveTowards(leftWall.position, centerPoint.position, crusherSpeed * Time.deltaTime);
            rightWall.position = Vector3.MoveTowards(rightWall.position, centerPoint.position, crusherSpeed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive && collision.CompareTag("Player"))
        {
            isActive = true;
        }
    }
    private void Reset()
    {
        isActive = false;
        leftWall.position = leftWallStartPos;
        rightWall.position = rightWallStartPos;
    }
}
