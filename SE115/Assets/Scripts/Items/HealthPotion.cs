using UnityEngine;

public class HealthPotion : MonoBehaviour, ICollectable
{
    [SerializeField] private float healAmount = 10.0f;
    public void Collect(GameObject target)
    {
        PlayerHealth playerHealth = target.GetComponentInChildren<PlayerHealth>();

        if(playerHealth != null)
        {
            playerHealth.ReceiveHeal(healAmount);
            Destroy(gameObject);
        }
    }
}
