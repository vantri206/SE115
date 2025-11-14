using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 8.0f;
    public float multipleSpeed = 1.0f;
    public Vector2 startDirection = Vector2.right;
    private Vector3 startPosition;
    public new Rigidbody2D rigidbody;
    public Vector2 direction;
    private void Awake()
    {
        this.rigidbody = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        ResetState();
    }
    private void Start()
    {

    }
    private void Update()
    {

    }
    private void FixedUpdate()
    {
        Vector2 position = this.transform.position;
        Vector2 velocity = this.speed * this.multipleSpeed * this.direction * Time.fixedDeltaTime;
        this.rigidbody.MovePosition(position + velocity);
    }
    public void ResetState()
    {
        multipleSpeed = 1.0f;
        direction = startDirection;
        this.transform.position = startPosition;
        this.rigidbody.bodyType = RigidbodyType2D.Dynamic;
        this.enabled = true;
    }
    public void SetDirection(Vector2 direction)
    {
        this.direction = direction;
    }
    public void SetMultipleSpeed(float multipleSpeed)
    {
        this.multipleSpeed = multipleSpeed;
    }
}
