using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private string hitboxLayerName = "Hitbox";

    public Damage owner;

    private int layerId;

    private void Awake()
    {
        if (GetComponent<Damage>() != null)
            owner = GetComponent<Damage>();
        else 
            owner = transform.parent.GetComponent<Damage>();

        layerId = LayerMask.NameToLayer(hitboxLayerName);

        if (layerId == -1)
        {
            Debug.LogError("Not found layer hitbox: " + hitboxLayerName);
        }
        else
        {
            gameObject.layer = layerId;
        }
    }
    private void OnTriggerStay2D(Collider2D collider)
    {
        HandleCollision(collider);
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        HandleCollision(collider);
    }
    private void HandleCollision(Collider2D collider)
    {
        if (collider.gameObject.layer != LayerMask.NameToLayer("Hurtbox")) return;

        IDamageable targetHealth = null;
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();

        if (hurtbox != null)
            targetHealth = hurtbox.health;
        if (targetHealth != null)
        {
            owner.DealDamage(targetHealth);
        }
    }
}
