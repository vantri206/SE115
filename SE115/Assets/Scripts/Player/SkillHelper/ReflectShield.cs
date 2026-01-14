using UnityEngine;

public class ReflectBox : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    private void Start()
    {
        if (player == null) 
            player = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            SatyrProjectile projectile = collision.GetComponent<SatyrProjectile>();

            if (projectile != null)
            {
                Vector2 reflectDir;
                SatyrEvilController boss = Object.FindAnyObjectByType<SatyrEvilController>();

                if (boss != null)
                {
                    reflectDir = (boss.transform.position - transform.position).normalized;
                }
                else
                {
                    reflectDir = player.facingDirection;
                }

                projectile.Reflect(reflectDir);
            }
        }
    }
}