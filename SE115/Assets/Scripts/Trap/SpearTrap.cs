using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SpearTrap : MonoBehaviour
{
    [Header("Refrences")]
    public Animator animator;
    public Rigidbody2D myRigidbody;
    [Header("Setting")]
    [SerializeField] private float flyingSpeed = 6.0f;
    private Vector2 flyingDirection = Vector2.left;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
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
}
