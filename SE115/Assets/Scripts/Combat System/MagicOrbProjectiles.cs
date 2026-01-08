using System.Collections;
using UnityEngine;

public class MagicOrbProjectiles : MonoBehaviour
{
    [Header("Refrences")]
    public Rigidbody2D myRigidbody;

    [Header("Setting")]
    [SerializeField] private float flyingSpeed = 5.0f;
    [SerializeField] private float riseTime = 1.0f;

    [Header("Visual Settings")]
    [SerializeField] private float selfRotateSpeed = 720.0f; 

    public Transform target;
    private bool isFired = false;

    private void Awake()
    {
        if (myRigidbody == null)
            myRigidbody = GetComponent<Rigidbody2D>();
    }
    public void Initialize(Vector2 position,  Transform target)
    {
        transform.position = position;
        this.target = target;

        if (ProjectilesPool.Instance != null)
        {
            transform.SetParent(ProjectilesPool.Instance.transform);
        }

        isFired = false;

        myRigidbody.bodyType = RigidbodyType2D.Kinematic;
        myRigidbody.linearVelocity = Vector2.zero;
        myRigidbody.angularVelocity = 0f;

        transform.rotation = Quaternion.identity;
    }
    public void Fire(Vector2 direction)
    {
        if (isFired) return;
        isFired = true;

        myRigidbody.bodyType = RigidbodyType2D.Dynamic;
        myRigidbody.gravityScale = 0; 

        float dirX = Mathf.Sign(direction.x);
        Vector2 flyDirection = new Vector2(0.5f * dirX, 1.0f).normalized;

        StartCoroutine(FlyRoutine(flyDirection));
    }

    private IEnumerator FlyRoutine(Vector2 direction)
    {
        myRigidbody.linearVelocity = direction * flyingSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        yield return new WaitForSeconds(riseTime);

        if (target != null)
        {
            Vector2 playerDirection = ((Vector2)target.position - myRigidbody.position).normalized;

            myRigidbody.linearVelocity = playerDirection * flyingSpeed;
        }
    }

    private void FixedUpdate()
    {
        transform.Rotate(0, 0, selfRotateSpeed * Time.fixedDeltaTime);
    }
    public void CancelProjectiles()
    {
        Destroy(gameObject);
    }
}
