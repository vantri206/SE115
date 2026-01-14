using UnityEngine;

public class SatyrProjectile : MonoBehaviour
{
    [Header("Refrences")]
    public Rigidbody2D rb;
    public SwordDamage swordDamage;

    [Header("Settings")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private float reflectedSpeed = 25f;
    [SerializeField] private Color reflectedColor = Color.yellow; 
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool isReflected = false;

    private void Awake()
    {
        if(rb == null)
            rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) 
            spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Launch(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
        RotateTowards(direction);
    }

    public void Reflect(Vector2 newDirection)
    {
        if (isReflected) return;

        isReflected = true;

        rb.linearVelocity = newDirection.normalized * reflectedSpeed;
        RotateTowards(newDirection);

        if (swordDamage)
            swordDamage.attackerTeam = DamageTeam.Player;

        if (spriteRenderer) 
                spriteRenderer.color = reflectedColor;
    }

    void RotateTowards(Vector2 dir)
    {
        if (dir == Vector2.zero) 
            return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReflected)
        {
            SatyrEvilController satyr = collision.GetComponentInParent<SatyrEvilController>();

            if (satyr != null)
            {
                satyr.GetHitByReflect();
            } 
        }
    }
}