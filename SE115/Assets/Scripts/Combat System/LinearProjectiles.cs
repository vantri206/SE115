using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class LinearProjectiles : MonoBehaviour
{
    [Header("Refrences")]
    public Rigidbody2D myRigidbody;
    [Header("Setting")]
    [SerializeField] private float flyingSpeed = 6.0f;
    [SerializeField] private Vector2 flyingDirection = Vector2.left;

    private void Awake()
    {
        if (myRigidbody == null)
            myRigidbody = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        myRigidbody.linearVelocity = flyingDirection * flyingSpeed;
    }
    public void SetDirection(Vector2 direction)
    {
        flyingDirection = direction;
    }
    public void SetDirectionX(Vector2 direction)
    {
        flyingDirection.x = direction.x;
    }
}
